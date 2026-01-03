-- ========================================================
-- SCRIPT: Crear SP usp_rptProductoSucursal CORREGIDO
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
        'N/A' AS [RFC Sucursal],
        'Principal' AS [Nombre Sucursal],
        'N/A' AS [Direccion Sucursal],
        CAST(P.ProductoID AS VARCHAR(50)) AS [Codigo Producto],
        P.Nombre AS [Nombre Producto],
        '' AS [Descripcion Producto],
        0 AS [Stock en tienda],
        0 AS [Precio Compra],
        ISNULL(P.PrecioPorKilo, 0) AS [Precio Venta],
        -- Campos para "más vendidos"
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 1
        ), 0) AS CantidadVendida,
        ISNULL((
            SELECT SUM(VD.Cantidad * VD.PrecioVenta)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 1
        ), 0) AS TotalVendido,
        P.Nombre AS Nombre
    FROM Productos P
    WHERE (@Codigo = '' OR CAST(P.ProductoID AS VARCHAR(50)) = @Codigo)
    ORDER BY CantidadVendida DESC;
END
GO

PRINT 'SP usp_rptProductoSucursal creado correctamente';

-- Prueba - Top 5 productos más vendidos hoy
SELECT TOP 5 * FROM (
    SELECT 
        P.Nombre,
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 1
        ), 0) AS CantidadVendida,
        ISNULL((
            SELECT SUM(VD.Cantidad * VD.PrecioVenta)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 1
        ), 0) AS TotalVendido
    FROM Productos P
) AS Ventas
WHERE CantidadVendida > 0
ORDER BY CantidadVendida DESC;
GO
