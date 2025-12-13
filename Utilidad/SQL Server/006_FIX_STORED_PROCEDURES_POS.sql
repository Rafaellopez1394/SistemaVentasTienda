-- ============================================================
-- CORRECCIÓN DE STORED PROCEDURES POS
-- Agregar SET QUOTED_IDENTIFIER ON para soportar columnas computadas
-- ============================================================
USE DB_TIENDA
GO

-- ============================================================
-- RECREAR RegistrarVentaPOS
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
    @TipoVenta VARCHAR(20),
    @FormaPagoID INT = NULL,
    @MetodoPagoID INT = NULL,
    @EfectivoRecibido DECIMAL(18,2) = NULL,
    @Cambio DECIMAL(18,2) = NULL,
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
        -- Validar crédito si es venta a crédito
        IF @TipoVenta = 'CREDITO'
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

            IF @TieneCredito = 0 OR @CreditoDisponible < @Total
            BEGIN
                SET @Mensaje = 'El cliente no tiene crédito suficiente';
                SET @VentaID = '00000000-0000-0000-0000-000000000000';
                RETURN;
            END
        END

        -- Insertar venta
        INSERT INTO VentasClientes (
            VentaID, ClienteID, TipoVenta, FormaPagoID, MetodoPagoID, 
            EfectivoRecibido, Cambio, Subtotal, IVA, Total, 
            RequiereFactura, CajaID, Usuario, FechaAlta, UltimaAct, Estatus
        )
        VALUES (
            @VentaID, @ClienteID, @TipoVenta, @FormaPagoID, @MetodoPagoID,
            @EfectivoRecibido, @Cambio, @Subtotal, @IVA, @Total,
            @RequiereFactura, @CajaID, @Usuario, GETDATE(), GETDATE(), 1
        );

        -- Registrar movimiento de caja si es venta de contado
        IF @TipoVenta = 'CONTADO'
        BEGIN
            DECLARE @SaldoAnterior DECIMAL(18,2);
            
            -- Obtener saldo anterior
            SELECT @SaldoAnterior = ISNULL(SaldoActual, 0)
            FROM MovimientosCaja
            WHERE CajaID = @CajaID
            ORDER BY MovimientoCajaID DESC
            OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
            
            IF @SaldoAnterior IS NULL SET @SaldoAnterior = 0;
            
            INSERT INTO MovimientosCaja (CajaID, TipoMovimiento, FechaMovimiento, Monto, SaldoAnterior, SaldoActual, Concepto, VentaID, Usuario)
            VALUES (@CajaID, 'VENTA', GETDATE(), @Total, @SaldoAnterior, @SaldoAnterior + @Total, 'Venta POS', @VentaID, @Usuario);
        END

        SET @Mensaje = 'Venta registrada correctamente';
    END TRY
    BEGIN CATCH
        SET @Mensaje = ERROR_MESSAGE();
        SET @VentaID = '00000000-0000-0000-0000-000000000000';
    END CATCH
END
GO

-- ============================================================
-- RECREAR RegistrarDetalleVentaPOS
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'RegistrarDetalleVentaPOS')
DROP PROCEDURE RegistrarDetalleVentaPOS
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

    -- Insertar detalle
    INSERT INTO VentasDetalleClientes (
        VentaID, ProductoID, LoteID, Cantidad, 
        PrecioVenta, PrecioCompra, TasaIVA, MontoIVA
    )
    VALUES (
        @VentaID, @ProductoID, @LoteID, @Cantidad,
        @PrecioVenta, @PrecioCompra, @TasaIVA, @MontoIVA
    );

    -- Actualizar stock del lote
    UPDATE LotesProducto
    SET CantidadDisponible = CantidadDisponible - @Cantidad,
        UltimaAct = GETDATE()
    WHERE LoteID = @LoteID;
END
GO

PRINT 'Stored procedures POS corregidos exitosamente'
GO
