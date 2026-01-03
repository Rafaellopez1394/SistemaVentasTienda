-- =====================================================
-- Script: 031_ACTUALIZAR_SP_ConsultarDetalleVenta.sql
-- Descripción: Actualiza SP para incluir campos de IVA
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

-- Eliminar procedimiento anterior si existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_ConsultarDetalleVenta') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE sp_ConsultarDetalleVenta
    PRINT 'Procedimiento anterior eliminado'
END
GO

-- Crear procedimiento actualizado
CREATE PROCEDURE sp_ConsultarDetalleVenta
    @VentaID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        VD.ProductoID, 
        P.Nombre AS Producto, 
        VD.LoteID, 
        VD.Cantidad, 
        VD.PrecioVenta, 
        VD.PrecioCompra,
        ISNULL(VD.TasaIVA, 0) AS TasaIVA,
        ISNULL(P.Exento, 0) AS Exento,
        (VD.Cantidad * VD.PrecioVenta) AS Subtotal,
        CASE 
            WHEN ISNULL(P.Exento, 0) = 1 THEN 0
            ELSE ISNULL(VD.TasaIVA, 0)
        END AS TasaIVAEfectiva
    FROM VentasDetalleClientes VD
    INNER JOIN Productos P ON VD.ProductoID = P.ProductoID
    WHERE VD.VentaID = @VentaID
END
GO

PRINT 'Procedimiento sp_ConsultarDetalleVenta actualizado correctamente'
PRINT ''
PRINT 'Cambios aplicados:'
PRINT '- Agregado campo TasaIVA del detalle de venta'
PRINT '- Agregado campo Exento del producto'
PRINT '- Agregado campo Subtotal calculado'
PRINT '- Agregado campo TasaIVAEfectiva (considera si es exento)'
GO
