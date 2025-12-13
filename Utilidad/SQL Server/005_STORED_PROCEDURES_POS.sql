-- ============================================================
-- STORED PROCEDURES PARA MÓDULO POS
-- ============================================================
USE DB_TIENDA
GO

-- ============================================================
-- 1. BUSCAR PRODUCTO POR CÓDIGO O NOMBRE (para agregar al carrito)
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'BuscarProductoPOS')
DROP PROCEDURE BuscarProductoPOS
GO

CREATE PROCEDURE BuscarProductoPOS
(
    @Texto VARCHAR(200)
)
AS
BEGIN
    SELECT TOP 20
        p.ProductoID,
        p.Nombre,
        p.CodigoInterno,
        ISNULL(l.PrecioVenta, 0) AS PrecioVenta,
        p.TasaIVAID,
        ti.Porcentaje AS TasaIVA,
        ti.Descripcion AS DescripcionIVA,
        ISNULL(SUM(l.CantidadDisponible), 0) AS StockDisponible,
        p.Estatus,
        c.Nombre AS Categoria
    FROM Productos p
    INNER JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN CatTasaIVA ti ON p.TasaIVAID = ti.TasaIVAID
    LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID AND l.Estatus = 1 AND l.CantidadDisponible > 0
    WHERE p.Estatus = 1
      AND (p.Nombre LIKE '%' + @Texto + '%' 
           OR p.CodigoInterno LIKE '%' + @Texto + '%')
    GROUP BY p.ProductoID, p.Nombre, p.CodigoInterno, p.TasaIVAID, ti.Porcentaje, ti.Descripcion, p.Estatus, c.Nombre, l.PrecioVenta
    ORDER BY p.Nombre
END
GO

-- ============================================================
-- 2. OBTENER PRODUCTO POR ID (para validar existencias)
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ObtenerProductoPorID')
DROP PROCEDURE ObtenerProductoPorID
GO

CREATE PROCEDURE ObtenerProductoPorID
(
    @ProductoID INT
)
AS
BEGIN
    SELECT 
        p.ProductoID,
        p.Nombre,
        p.CodigoInterno,
        ISNULL(l.PrecioVenta, 0) AS PrecioVenta,
        p.TasaIVAID,
        ti.Porcentaje AS TasaIVA,
        ISNULL(SUM(l.CantidadDisponible), 0) AS StockDisponible,
        p.Estatus
    FROM Productos p
    LEFT JOIN CatTasaIVA ti ON p.TasaIVAID = ti.TasaIVAID
    LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID AND l.Estatus = 1
    WHERE p.ProductoID = @ProductoID
    GROUP BY p.ProductoID, p.Nombre, p.CodigoInterno, p.TasaIVAID, ti.Porcentaje, p.Estatus, l.PrecioVenta
END
GO

-- ============================================================
-- 3. VALIDAR CRÉDITO DISPONIBLE DEL CLIENTE
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ValidarCreditoCliente')
DROP PROCEDURE ValidarCreditoCliente
GO

CREATE PROCEDURE ValidarCreditoCliente
(
    @ClienteID UNIQUEIDENTIFIER,
    @MontoVenta DECIMAL(18,2),
    @TieneCredito BIT OUTPUT,
    @CreditoDisponible DECIMAL(18,2) OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    -- Obtener límite total de crédito del cliente
    DECLARE @LimiteTotal DECIMAL(18,2) = 0;
    
    SELECT @LimiteTotal = ISNULL(SUM(LimiteDinero), 0)
    FROM ClienteTiposCredito
    WHERE ClienteID = @ClienteID AND Estatus = 1;

    -- Obtener saldo actual (deuda pendiente)
    DECLARE @SaldoActual DECIMAL(18,2) = 0;
    
    SELECT @SaldoActual = ISNULL(SUM(Saldo), 0)
    FROM VentasCredito
    WHERE ClienteID = @ClienteID AND Estatus != 'CANCELADA' AND Saldo > 0;

    -- Calcular crédito disponible
    SET @CreditoDisponible = @LimiteTotal - @SaldoActual;

    -- Validar si tiene crédito suficiente
    IF @LimiteTotal = 0
    BEGIN
        SET @TieneCredito = 0;
        SET @Mensaje = 'El cliente no tiene línea de crédito autorizada';
    END
    ELSE IF @CreditoDisponible < @MontoVenta
    BEGIN
        SET @TieneCredito = 0;
        SET @Mensaje = 'Crédito insuficiente. Disponible: $' + CAST(@CreditoDisponible AS VARCHAR(20));
    END
    ELSE
    BEGIN
        SET @TieneCredito = 1;
        SET @Mensaje = 'Crédito disponible: $' + CAST(@CreditoDisponible AS VARCHAR(20));
    END
END
GO

-- ============================================================
-- 4. REGISTRAR VENTA (CONTADO O CRÉDITO)
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarVentaPOS')
DROP PROCEDURE RegistrarVentaPOS
GO

CREATE PROCEDURE RegistrarVentaPOS
(
    @ClienteID UNIQUEIDENTIFIER,
    @TipoVenta VARCHAR(20), -- CONTADO o CREDITO
    @FormaPagoID INT = NULL,
    @MetodoPagoID INT = NULL,
    @EfectivoRecibido DECIMAL(18,2) = NULL,
    @Cambio DECIMAL(18,2) = NULL,
    @Subtotal DECIMAL(18,2),
    @IVA DECIMAL(18,2),
    @Total DECIMAL(18,2),
    @RequiereFactura BIT = 0,
    @CajaID INT,
    @Usuario VARCHAR(50),
    @VentaID UNIQUEIDENTIFIER OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Generar nuevo VentaID
        SET @VentaID = NEWID();

        -- Validar si es venta a crédito
        IF @TipoVenta = 'CREDITO'
        BEGIN
            DECLARE @TieneCredito BIT;
            DECLARE @CreditoDisp DECIMAL(18,2);
            DECLARE @MsgCredito VARCHAR(200);

            EXEC ValidarCreditoCliente 
                @ClienteID, 
                @Total, 
                @TieneCredito OUTPUT, 
                @CreditoDisp OUTPUT, 
                @MsgCredito OUTPUT;

            IF @TieneCredito = 0
            BEGIN
                SET @Mensaje = @MsgCredito;
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- Insertar venta
        INSERT INTO VentasClientes (
            VentaID, ClienteID, FechaVenta, TipoVenta, 
            FormaPagoID, MetodoPagoID, EfectivoRecibido, Cambio,
            Subtotal, IVA, Total, RequiereFactura, CajaID,
            Estatus, Usuario, FechaAlta, UltimaAct
        )
        VALUES (
            @VentaID, @ClienteID, GETDATE(), @TipoVenta,
            @FormaPagoID, @MetodoPagoID, @EfectivoRecibido, @Cambio,
            @Subtotal, @IVA, @Total, @RequiereFactura, @CajaID,
            'COMPLETADA', @Usuario, GETDATE(), GETDATE()
        );

        -- Registrar movimiento en caja (solo si es contado)
        IF @TipoVenta = 'CONTADO'
        BEGIN
            DECLARE @SaldoAnterior DECIMAL(18,2) = 0;
            
            SELECT TOP 1 @SaldoAnterior = SaldoActual
            FROM MovimientosCaja
            WHERE CajaID = @CajaID
            ORDER BY MovimientoCajaID DESC;

            INSERT INTO MovimientosCaja (
                CajaID, TipoMovimiento, FechaMovimiento,
                Monto, SaldoAnterior, SaldoActual,
                Concepto, VentaID, Usuario
            )
            VALUES (
                @CajaID, 'VENTA', GETDATE(),
                @Total, @SaldoAnterior, @SaldoAnterior + @Total,
                'Venta ' + @TipoVenta, @VentaID, @Usuario
            );
        END

        SET @Mensaje = 'Venta registrada correctamente';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END
GO

-- ============================================================
-- 5. REGISTRAR DETALLE DE VENTA
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarDetalleVentaPOS')
DROP PROCEDURE RegistrarDetalleVentaPOS
GO

CREATE PROCEDURE RegistrarDetalleVentaPOS
(
    @VentaID UNIQUEIDENTIFIER,
    @ProductoID INT,
    @LoteID INT,
    @Cantidad INT,
    @PrecioVenta DECIMAL(18,2),
    @PrecioCompra DECIMAL(18,2),
    @TasaIVA DECIMAL(5,2),
    @MontoIVA DECIMAL(18,2)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Insertar detalle (Utilidad es columna calculada)
        INSERT INTO VentasDetalleClientes (
            VentaID, ProductoID, LoteID, Cantidad, 
            PrecioVenta, PrecioCompra,
            TasaIVA, MontoIVA
        )
        VALUES (
            @VentaID, @ProductoID, @LoteID, @Cantidad,
            @PrecioVenta, @PrecioCompra,
            @TasaIVA, @MontoIVA
        );

        -- Descontar del lote
        UPDATE LotesProducto
        SET CantidadDisponible = CantidadDisponible - @Cantidad
        WHERE LoteID = @LoteID;

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- 6. APERTURA DE CAJA
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'AperturaCaja')
DROP PROCEDURE AperturaCaja
GO

CREATE PROCEDURE AperturaCaja
(
    @CajaID INT,
    @MontoInicial DECIMAL(18,2),
    @Usuario VARCHAR(50),
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO MovimientosCaja (
            CajaID, TipoMovimiento, FechaMovimiento,
            Monto, SaldoAnterior, SaldoActual,
            Concepto, Usuario
        )
        VALUES (
            @CajaID, 'APERTURA', GETDATE(),
            @MontoInicial, 0, @MontoInicial,
            'Apertura de caja', @Usuario
        );

        SET @Mensaje = 'Caja abierta correctamente';
    END TRY
    BEGIN CATCH
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END
GO

-- ============================================================
-- 7. CIERRE DE CAJA
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'CierreCaja')
DROP PROCEDURE CierreCaja
GO

CREATE PROCEDURE CierreCaja
(
    @CajaID INT,
    @Usuario VARCHAR(50),
    @SaldoFinal DECIMAL(18,2) OUTPUT,
    @TotalVentas DECIMAL(18,2) OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Obtener saldo actual
        SELECT TOP 1 @SaldoFinal = SaldoActual
        FROM MovimientosCaja
        WHERE CajaID = @CajaID
        ORDER BY MovimientoCajaID DESC;

        -- Obtener total de ventas del turno
        SELECT @TotalVentas = ISNULL(SUM(Monto), 0)
        FROM MovimientosCaja
        WHERE CajaID = @CajaID 
          AND TipoMovimiento = 'VENTA'
          AND FechaMovimiento >= (
              SELECT TOP 1 FechaMovimiento 
              FROM MovimientosCaja 
              WHERE CajaID = @CajaID AND TipoMovimiento = 'APERTURA'
              ORDER BY MovimientoCajaID DESC
          );

        -- Registrar cierre
        INSERT INTO MovimientosCaja (
            CajaID, TipoMovimiento, FechaMovimiento,
            Monto, SaldoAnterior, SaldoActual,
            Concepto, Usuario
        )
        VALUES (
            @CajaID, 'CIERRE', GETDATE(),
            0, @SaldoFinal, 0,
            'Cierre de caja - Total ventas: $' + CAST(@TotalVentas AS VARCHAR(20)), 
            @Usuario
        );

        SET @Mensaje = 'Caja cerrada correctamente';
    END TRY
    BEGIN CATCH
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '============================================================';
PRINT 'STORED PROCEDURES PARA POS CREADOS EXITOSAMENTE';
PRINT '============================================================';
