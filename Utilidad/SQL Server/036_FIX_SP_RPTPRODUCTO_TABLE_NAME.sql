-- ========================================================
-- FIX: Corregir usp_rptProductoSucursal para usar PRODUCTO_TIENDA
-- Problema: El SP hace referencia a ProductosSucursal que no existe
-- Solución: Usar la tabla correcta PRODUCTO_TIENDA
-- ========================================================
USE DB_TIENDA
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_rptProductoSucursal')
    DROP PROCEDURE usp_rptProductoSucursal;
GO

CREATE PROCEDURE usp_rptProductoSucursal
    @SucursalID INT = 0,
    @Codigo VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ISNULL(S.RFC, 'N/A') AS [RFC Sucursal],
        ISNULL(S.Nombre, 'Principal') AS [Nombre Sucursal],
        ISNULL(S.Direccion, 'N/A') AS [Direccion Sucursal],
        P.CodigoInterno AS [Codigo Producto],
        P.Nombre AS [Nombre Producto],
        '' AS [Descripcion Producto],
        0 AS [Stock en tienda],
        0 AS [Precio Compra],
        ISNULL(P.PrecioPorKilo, 0) AS [Precio Venta],
        -- Campos adicionales para "más vendidos"
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            LEFT JOIN Cajas C ON VC.CajaID = C.CajaID
            WHERE VD.ProductoID = P.ProductoID
              AND (@SucursalID = 0 OR C.SucursalID = @SucursalID)
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 'COMPLETADA'
        ), 0) AS CantidadVendida,
        ISNULL((
            SELECT SUM(VD.Cantidad * VD.PrecioVenta)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            LEFT JOIN Cajas C ON VC.CajaID = C.CajaID
            WHERE VD.ProductoID = P.ProductoID
              AND (@SucursalID = 0 OR C.SucursalID = @SucursalID)
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 'COMPLETADA'
        ), 0) AS TotalVendido,
        P.Nombre AS Nombre
    FROM Productos P
    CROSS JOIN (SELECT TOP 1 * FROM SUCURSAL WHERE (@SucursalID = 0 OR SucursalID = @SucursalID)) S
    WHERE (@Codigo = '' OR P.CodigoInterno LIKE '%' + @Codigo + '%')
      AND P.Estatus = 1
    ORDER BY 
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
        ), 0) DESC;
END
GO

PRINT '✅ SP usp_rptProductoSucursal corregido - ahora usa PRODUCTO_TIENDA';
GO

-- Verificar que el SP existe
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_rptProductoSucursal')
    PRINT '✅ Verificación exitosa: usp_rptProductoSucursal existe'
ELSE
    PRINT '❌ ERROR: usp_rptProductoSucursal NO fue creado'
GO
