-- ============================================================
-- MEJORAS: Pagos Parciales + Inventario Fraccionado
-- Fecha: 2026-01-04
-- ============================================================
-- Este script implementa:
-- 1. Soporte para cantidades decimales en inventario (kg, gramos)
-- 2. Pagos parciales con factura PPD (Pago en Parcialidades o Diferido)
-- 3. Gestión de complementos de pago vinculados a ventas
-- ============================================================
USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'Iniciando actualización de esquema...'
PRINT '========================================='

-- ============================================================
-- PASO 1: Modificar columnas de cantidad a DECIMAL
-- ============================================================
PRINT 'Modificando columnas de cantidad a DECIMAL(18,3)...'

-- VentasDetalleClientes: Primero eliminar columna computada Utilidad si existe
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasDetalleClientes') AND name = 'Utilidad' AND is_computed = 1)
BEGIN
    ALTER TABLE VentasDetalleClientes DROP COLUMN Utilidad
    PRINT '✓ Columna computada Utilidad eliminada temporalmente'
END

-- Ahora modificar Cantidad
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasDetalleClientes') AND name = 'Cantidad')
BEGIN
    ALTER TABLE VentasDetalleClientes ALTER COLUMN Cantidad DECIMAL(18,3) NOT NULL
    PRINT '✓ VentasDetalleClientes.Cantidad actualizada a DECIMAL(18,3)'
END

-- Recrear columna computada Utilidad
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasDetalleClientes') AND name = 'Utilidad')
BEGIN
    ALTER TABLE VentasDetalleClientes ADD Utilidad AS ((PrecioVenta - PrecioCompra) * Cantidad) PERSISTED
    PRINT '✓ Columna computada Utilidad recreada'
END

-- LotesProducto
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LotesProducto') AND name = 'CantidadDisponible')
BEGIN
    ALTER TABLE LotesProducto ALTER COLUMN CantidadDisponible DECIMAL(18,3) NOT NULL
    PRINT '✓ LotesProducto.CantidadDisponible actualizada'
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LotesProducto') AND name = 'CantidadInicial')
BEGIN
    ALTER TABLE LotesProducto ALTER COLUMN CantidadInicial DECIMAL(18,3) NOT NULL
    PRINT '✓ LotesProducto.CantidadInicial actualizada'
END
GO

-- ============================================================
-- PASO 2: Agregar campos para pagos parciales en VentasClientes
-- ============================================================
PRINT 'Agregando campos para pagos parciales...'

-- Primero agregar el campo base MontoPagado
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'MontoPagado')
BEGIN
    ALTER TABLE VentasClientes ADD MontoPagado DECIMAL(18,2) NULL
    PRINT '✓ Campo MontoPagado agregado'
END
GO

-- Actualizar valores NULL a 0 para ventas existentes
UPDATE VentasClientes SET MontoPagado = 0 WHERE MontoPagado IS NULL;
GO

-- Agregar constraint para que no sea NULL
ALTER TABLE VentasClientes ALTER COLUMN MontoPagado DECIMAL(18,2) NOT NULL;
GO

-- Ahora agregar columnas computadas que dependen de MontoPagado
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'SaldoPendiente')
BEGIN
    ALTER TABLE VentasClientes ADD SaldoPendiente AS (Total - MontoPagado) PERSISTED
    PRINT '✓ Campo SaldoPendiente agregado (columna computada)'
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'EsPagado')
BEGIN
    ALTER TABLE VentasClientes ADD EsPagado AS (CASE WHEN (Total - MontoPagado) <= 0.01 THEN 1 ELSE 0 END) PERSISTED
    PRINT '✓ Campo EsPagado agregado (columna computada)'
END
GO

-- ============================================================
-- PASO 3: Tabla para registrar pagos parciales de ventas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'VentaPagos')
BEGIN
    CREATE TABLE VentaPagos (
        PagoID INT IDENTITY(1,1) PRIMARY KEY,
        VentaID UNIQUEIDENTIFIER NOT NULL,
        FormaPagoID INT NOT NULL,
        MetodoPagoID INT NOT NULL,
        Monto DECIMAL(18,2) NOT NULL,
        FechaPago DATETIME NOT NULL DEFAULT GETDATE(),
        Referencia VARCHAR(100) NULL,
        Observaciones VARCHAR(500) NULL,
        Usuario VARCHAR(100) NOT NULL,
        ComplementoPagoID INT NULL, -- Vincula con complemento de pago timbrado
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_VentaPagos_Venta FOREIGN KEY (VentaID) REFERENCES VentasClientes(VentaID),
        CONSTRAINT FK_VentaPagos_FormaPago FOREIGN KEY (FormaPagoID) REFERENCES CatFormasPago(FormaPagoID),
        CONSTRAINT FK_VentaPagos_MetodoPago FOREIGN KEY (MetodoPagoID) REFERENCES CatMetodosPago(MetodoPagoID)
    )
    PRINT '✓ Tabla VentaPagos creada'
END

-- ============================================================
-- PASO 4: Modificar SP RegistrarVentaPOS para soporte de pagos parciales
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarVentaPOS')
    DROP PROCEDURE RegistrarVentaPOS
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE RegistrarVentaPOS
(
    @ClienteID UNIQUEIDENTIFIER,
    @TipoVenta VARCHAR(20), -- 'CONTADO', 'CREDITO', 'PARCIAL'
    @FormaPagoID INT = NULL,
    @MetodoPagoID INT = NULL,
    @EfectivoRecibido DECIMAL(18,2) = NULL,
    @Cambio DECIMAL(18,2) = NULL,
    @MontoPagado DECIMAL(18,2) = NULL, -- Monto pagado inicialmente (para pagos parciales)
    @Subtotal DECIMAL(18,2),
    @IVA DECIMAL(18,2),
    @Total DECIMAL(18,2),
    @RequiereFactura BIT,
    @CajaID INT,
    @Usuario VARCHAR(100),
    @VentaID UNIQUEIDENTIFIER OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET @VentaID = NEWID();
    SET @Mensaje = '';

    BEGIN TRY
        -- Si es pago parcial, el monto pagado no puede ser mayor que el total
        IF @TipoVenta = 'PARCIAL' AND (@MontoPagado IS NULL OR @MontoPagado <= 0)
        BEGIN
            SET @Mensaje = 'Para pago parcial debe indicar el monto pagado';
            SET @VentaID = '00000000-0000-0000-0000-000000000000';
            RETURN;
        END

        -- Para CONTADO, el pago completo es el total
        IF @TipoVenta = 'CONTADO'
            SET @MontoPagado = @Total;
        
        -- Para CREDITO, no hay pago inicial
        IF @TipoVenta = 'CREDITO'
            SET @MontoPagado = 0;

        -- Validar crédito si es venta a crédito
        IF @TipoVenta IN ('CREDITO', 'PARCIAL')
        BEGIN
            DECLARE @TieneCredito BIT, @CreditoDisponible DECIMAL(18,2);
            
            SELECT @TieneCredito = 
                CASE 
                    WHEN ct.LimiteDinero > ISNULL(SUM(vc.Saldo), 0) THEN 1
                    ELSE 0
                END,
                @CreditoDisponible = ct.LimiteDinero - ISNULL(SUM(vc.Saldo), 0)
            FROM ClienteTiposCredito ct
            LEFT JOIN VentasCredito vc ON vc.ClienteID = ct.ClienteID AND vc.Estatus = 1
            WHERE ct.ClienteID = @ClienteID AND ct.Estatus = 1
            GROUP BY ct.LimiteDinero;

            DECLARE @SaldoRestante DECIMAL(18,2) = @Total - ISNULL(@MontoPagado, 0);

            IF @TieneCredito = 0 OR @CreditoDisponible < @SaldoRestante
            BEGIN
                SET @Mensaje = 'El cliente no tiene crédito suficiente para el saldo pendiente';
                SET @VentaID = '00000000-0000-0000-0000-000000000000';
                RETURN;
            END
        END

        -- Insertar venta
        INSERT INTO VentasClientes (
            VentaID, ClienteID, TipoVenta, FormaPagoID, MetodoPagoID, 
            EfectivoRecibido, Cambio, MontoPagado, Subtotal, IVA, Total, 
            RequiereFactura, CajaID, Usuario, FechaAlta, UltimaAct, Estatus
        )
        VALUES (
            @VentaID, @ClienteID, @TipoVenta, @FormaPagoID, @MetodoPagoID,
            @EfectivoRecibido, @Cambio, @MontoPagado, @Subtotal, @IVA, @Total,
            @RequiereFactura, @CajaID, @Usuario, GETDATE(), GETDATE(), 1
        );

        -- Si hubo un pago inicial, registrarlo en VentaPagos
        IF @MontoPagado > 0 AND @FormaPagoID IS NOT NULL AND @MetodoPagoID IS NOT NULL
        BEGIN
            INSERT INTO VentaPagos (VentaID, FormaPagoID, MetodoPagoID, Monto, FechaPago, Usuario)
            VALUES (@VentaID, @FormaPagoID, @MetodoPagoID, @MontoPagado, GETDATE(), @Usuario);
        END

        -- Registrar movimiento de caja si hubo pago
        IF @MontoPagado > 0
        BEGIN
            DECLARE @SaldoAnterior DECIMAL(18,2);
            
            SELECT @SaldoAnterior = ISNULL(SaldoActual, 0)
            FROM MovimientosCaja
            WHERE CajaID = @CajaID
            ORDER BY MovimientoCajaID DESC
            OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
            
            IF @SaldoAnterior IS NULL SET @SaldoAnterior = 0;
            
            INSERT INTO MovimientosCaja (CajaID, TipoMovimiento, FechaMovimiento, Monto, SaldoAnterior, SaldoActual, Concepto, VentaID, Usuario)
            VALUES (@CajaID, 'VENTA', GETDATE(), @MontoPagado, @SaldoAnterior, @SaldoAnterior + @MontoPagado, 
                    CASE WHEN @TipoVenta = 'PARCIAL' THEN 'Venta POS - Pago Parcial' ELSE 'Venta POS' END, 
                    @VentaID, @Usuario);
        END

        SET @Mensaje = 'Venta registrada correctamente';
    END TRY
    BEGIN CATCH
        SET @Mensaje = ERROR_MESSAGE();
        SET @VentaID = '00000000-0000-0000-0000-000000000000';
    END CATCH
END
GO

PRINT '✓ Stored Procedure RegistrarVentaPOS actualizado'

-- ============================================================
-- PASO 5: Modificar SP RegistrarDetalleVentaPOS para cantidades DECIMAL
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarDetalleVentaPOS')
    DROP PROCEDURE RegistrarDetalleVentaPOS
GO

CREATE PROCEDURE RegistrarDetalleVentaPOS
(
    @VentaID UNIQUEIDENTIFIER,
    @ProductoID INT,
    @LoteID INT,
    @Cantidad DECIMAL(18,3), -- ✅ Ahora soporta decimales para kg/gramos
    @PrecioVenta DECIMAL(18,2),
    @PrecioCompra DECIMAL(18,2),
    @TasaIVA DECIMAL(5,2),
    @MontoIVA DECIMAL(18,2),
    @Gramos DECIMAL(10,3) = NULL,
    @Kilogramos DECIMAL(10,3) = NULL,
    @PrecioPorKilo DECIMAL(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Insertar detalle
    INSERT INTO VentasDetalleClientes (
        VentaID, ProductoID, LoteID, Cantidad, 
        PrecioVenta, PrecioCompra, TasaIVA, MontoIVA
    )
    VALUES (
        @VentaID, @ProductoID, @LoteID, @Cantidad,
        @PrecioVenta, @PrecioCompra, @TasaIVA, @MontoIVA
    );

    -- ✅ Actualizar stock del lote con cantidad DECIMAL
    UPDATE LotesProducto
    SET CantidadDisponible = CantidadDisponible - @Cantidad,
        UltimaAct = GETDATE()
    WHERE LoteID = @LoteID;

    -- Validar que no quede stock negativo
    IF EXISTS (SELECT 1 FROM LotesProducto WHERE LoteID = @LoteID AND CantidadDisponible < 0)
    BEGIN
        RAISERROR('Stock insuficiente en el lote. No se puede completar la venta.', 16, 1);
        RETURN;
    END
END
GO

PRINT '✓ Stored Procedure RegistrarDetalleVentaPOS actualizado con soporte DECIMAL'

-- ============================================================
-- PASO 6: Stored Procedure para registrar pagos posteriores
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarPagoVenta')
    DROP PROCEDURE RegistrarPagoVenta
GO

CREATE PROCEDURE RegistrarPagoVenta
(
    @VentaID UNIQUEIDENTIFIER,
    @FormaPagoID INT,
    @MetodoPagoID INT,
    @Monto DECIMAL(18,2),
    @Referencia VARCHAR(100) = NULL,
    @Observaciones VARCHAR(500) = NULL,
    @Usuario VARCHAR(100),
    @ComplementoPagoID INT = NULL, -- ID del complemento de pago timbrado (si aplica)
    @PagoID INT OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET @PagoID = 0;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que la venta exista
        DECLARE @Total DECIMAL(18,2), @MontoPagado DECIMAL(18,2);
        SELECT @Total = Total, @MontoPagado = MontoPagado
        FROM VentasClientes
        WHERE VentaID = @VentaID;

        IF @Total IS NULL
        BEGIN
            SET @Mensaje = 'La venta no existe';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar que el monto no exceda el saldo pendiente
        DECLARE @SaldoPendiente DECIMAL(18,2) = @Total - @MontoPagado;
        IF @Monto > @SaldoPendiente
        BEGIN
            SET @Mensaje = 'El monto del pago excede el saldo pendiente';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Registrar el pago
        INSERT INTO VentaPagos (VentaID, FormaPagoID, MetodoPagoID, Monto, FechaPago, Referencia, Observaciones, Usuario, ComplementoPagoID)
        VALUES (@VentaID, @FormaPagoID, @MetodoPagoID, @Monto, GETDATE(), @Referencia, @Observaciones, @Usuario, @ComplementoPagoID);

        SET @PagoID = SCOPE_IDENTITY();

        -- Actualizar el monto pagado en la venta
        UPDATE VentasClientes
        SET MontoPagado = MontoPagado + @Monto,
            UltimaAct = GETDATE()
        WHERE VentaID = @VentaID;

        COMMIT TRANSACTION;
        SET @Mensaje = 'Pago registrado correctamente';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ Stored Procedure RegistrarPagoVenta creado'

-- ============================================================
-- PASO 7: Stored Procedure para obtener ventas pendientes de pago
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ObtenerVentasPendientesPago')
    DROP PROCEDURE ObtenerVentasPendientesPago
GO

CREATE PROCEDURE ObtenerVentasPendientesPago
(
    @ClienteID UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        vc.VentaID,
        vc.ClienteID,
        c.NombreCompleto AS Cliente,
        c.RFC,
        vc.FechaAlta AS FechaVenta,
        vc.Total,
        vc.MontoPagado,
        vc.SaldoPendiente,
        vc.EsPagado,
        vc.RequiereFactura,
        f.IdFactura,
        f.UUID AS FacturaUUID,
        f.Serie + '-' + CAST(f.Folio AS VARCHAR) AS FacturaSerieFolio,
        -- Pagos realizados
        (SELECT COUNT(*) FROM VentaPagos WHERE VentaID = vc.VentaID) AS NumeroPagos
    FROM VentasClientes vc
    INNER JOIN Clientes c ON vc.ClienteID = c.ClienteID
    LEFT JOIN Factura f ON f.VentaID = vc.VentaID AND f.EsCancelada = 0
    WHERE vc.Estatus = 1
      AND vc.SaldoPendiente > 0.01
      AND (@ClienteID IS NULL OR vc.ClienteID = @ClienteID)
    ORDER BY vc.FechaAlta DESC;
END
GO

PRINT '✓ Stored Procedure ObtenerVentasPendientesPago creado'

-- ============================================================
-- PASO 8: Actualizar datos existentes
-- ============================================================
PRINT 'Actualizando ventas existentes...'

-- Actualizar MontoPagado para ventas de CONTADO
UPDATE VentasClientes
SET MontoPagado = Total
WHERE TipoVenta = 'CONTADO' AND MontoPagado IS NULL;

-- Para ventas a CREDITO sin pago
UPDATE VentasClientes
SET MontoPagado = 0
WHERE TipoVenta = 'CREDITO' AND MontoPagado IS NULL;

PRINT '✓ Datos existentes actualizados'

-- ============================================================
-- FINALIZACIÓN
-- ============================================================
PRINT '========================================='
PRINT '✓ Actualización completada exitosamente'
PRINT '========================================='
PRINT ''
PRINT 'Funcionalidades implementadas:'
PRINT '  1. ✅ Cantidades decimales en inventario (kg, gramos)'
PRINT '  2. ✅ Pagos parciales en ventas'
PRINT '  3. ✅ Registro de múltiples pagos por venta'
PRINT '  4. ✅ Vinculación con complementos de pago'
PRINT '  5. ✅ Saldo pendiente automático'
PRINT ''
PRINT 'Nuevas tablas:'
PRINT '  - VentaPagos: Registro de pagos parciales'
PRINT ''
PRINT 'SPs actualizados:'
PRINT '  - RegistrarVentaPOS: Soporte para pagos parciales'
PRINT '  - RegistrarDetalleVentaPOS: Cantidades DECIMAL(18,3)'
PRINT ''
PRINT 'SPs nuevos:'
PRINT '  - RegistrarPagoVenta: Para pagos posteriores'
PRINT '  - ObtenerVentasPendientesPago: Consultar ventas con saldo'
PRINT '========================================='
GO
