-- 009_FIX_SP_REGISTRAR_VENTA_CREDITO.sql
-- Crear stored procedure para registrar ventas a crédito

USE DB_TIENDA;
GO

-- Eliminar si existe
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_RegistrarVentaCredito')
    DROP PROCEDURE sp_RegistrarVentaCredito;
GO

CREATE PROCEDURE sp_RegistrarVentaCredito
    @ClienteID UNIQUEIDENTIFIER,
    @Usuario VARCHAR(50),
    @Productos TipoTablaVentaProductos READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @VentaID UNIQUEIDENTIFIER = NEWID();
        DECLARE @Total DECIMAL(18,2) = 0;
        DECLARE @Subtotal DECIMAL(18,2) = 0;
        DECLARE @IVA DECIMAL(18,2) = 0;
        
        -- Calcular totales de los productos
        SELECT 
            @Subtotal = SUM(tp.Cantidad * l.PrecioVenta),
            @IVA = SUM(tp.Cantidad * l.PrecioVenta * ISNULL(t.Porcentaje, 0) / 100)
        FROM @Productos tp
        INNER JOIN LotesProducto l ON tp.LoteID = l.LoteID
        INNER JOIN Productos p ON tp.ProductoID = p.ProductoID
        LEFT JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID;
        
        SET @Total = @Subtotal + @IVA;
        
        -- Insertar en VentasClientes
        INSERT INTO VentasClientes (
            VentaID, ClienteID, FechaVenta, Total, Subtotal, IVA,
            Estatus, TipoVenta, SaldoPendiente, TotalPagado,
            Usuario
        )
        VALUES (
            @VentaID, @ClienteID, GETDATE(), @Total, @Subtotal, @IVA,
            '1', 'CREDITO', @Total, 0,
            @Usuario
        );
        
        -- Insertar detalle de productos
        INSERT INTO DetalleVentasClientes (
            VentaID, ProductoID, LoteID, Cantidad, PrecioUnitario, 
            Subtotal, IVA, Total
        )
        SELECT 
            @VentaID,
            tp.ProductoID,
            tp.LoteID,
            tp.Cantidad,
            l.PrecioVenta,
            tp.Cantidad * l.PrecioVenta AS Subtotal,
            tp.Cantidad * l.PrecioVenta * ISNULL(t.Porcentaje, 0) / 100 AS IVA,
            tp.Cantidad * l.PrecioVenta * (1 + ISNULL(t.Porcentaje, 0) / 100) AS Total
        FROM @Productos tp
        INNER JOIN LotesProducto l ON tp.LoteID = l.LoteID
        INNER JOIN Productos p ON tp.ProductoID = p.ProductoID
        LEFT JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID;
        
        -- Actualizar inventario (reducir cantidades)
        UPDATE l
        SET l.CantidadDisponible = l.CantidadDisponible - tp.Cantidad
        FROM LotesProducto l
        INNER JOIN @Productos tp ON l.LoteID = tp.LoteID;
        
        COMMIT TRANSACTION;
        
        SELECT @VentaID AS VentaID, 'Venta registrada correctamente' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

PRINT 'Stored Procedure sp_RegistrarVentaCredito creado correctamente';
GO
