-- ========================================================
-- SCRIPT: Crear Stored Procedures de Reportes para Dashboard
-- ========================================================
USE DB_TIENDA
GO

PRINT '========================================================';
PRINT 'CREANDO STORED PROCEDURES DE REPORTES';
PRINT '========================================================';
PRINT '';

-- SP: usp_rptVenta - Reporte de Ventas
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_rptVenta')
    DROP PROCEDURE usp_rptVenta;
GO

CREATE PROCEDURE usp_rptVenta
    @FechaInicio DATE,
    @FechaFin DATE,
    @SucursalID INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CONVERT(VARCHAR, VC.FechaVenta, 103) + ' ' + CONVERT(VARCHAR, VC.FechaVenta, 108) AS [Fecha Venta],
        CAST(VC.VentaID AS VARCHAR(36)) AS [Numero Documento],
        CASE 
            WHEN VC.TipoVenta = 'CONTADO' THEN 'Ticket'
            ELSE 'Factura'
        END AS [Tipo Documento],
        ISNULL(S.Nombre, 'Principal') AS [Nombre Sucursal],
        ISNULL(S.RFC, 'N/A') AS [RFC Sucursal],
        ISNULL(U.Nombres + ' ' + U.Apellidos, 'Sistema') AS [Nombre Empleado],
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            WHERE VD.VentaID = VC.VentaID
        ), 0) AS [Cantidad Unidades Vendidas],
        ISNULL((
            SELECT COUNT(*)
            FROM VentasDetalleClientes VD
            WHERE VD.VentaID = VC.VentaID
        ), 0) AS [Cantidad Productos],
        ISNULL(VC.Total, 0) AS [Total Venta]
    FROM VentasClientes VC
    LEFT JOIN Sucursales S ON VC.CajaID = S.SucursalID  -- Ajustar según relación correcta
    LEFT JOIN Usuarios U ON VC.Usuario = U.UsuarioID
    WHERE CAST(VC.FechaVenta AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND VC.Estatus = 'COMPLETADA'
      AND (@SucursalID = 0 OR VC.CajaID = @SucursalID)
    ORDER BY VC.FechaVenta DESC;
END
GO

PRINT '✅ SP usp_rptVenta creado';
PRINT '';

-- SP: usp_rptProductoSucursal - Reporte de Productos
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
        P.ProductoID AS [Codigo Producto],
        P.Nombre AS [Nombre Producto],
        ISNULL(P.Descripcion, '') AS [Descripcion Producto],
        ISNULL((
            SELECT SUM(PS.Cantidad)
            FROM ProductosSucursal PS
            WHERE PS.ProductoID = P.ProductoID
              AND (@SucursalID = 0 OR PS.SucursalID = @SucursalID)
        ), 0) AS [Stock en tienda],
        ISNULL(P.PrecioCompra, 0) AS [Precio Compra],
        ISNULL(P.PrecioVenta, 0) AS [Precio Venta],
        -- Campos adicionales para "más vendidos"
        ISNULL((
            SELECT SUM(VD.Cantidad)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 'COMPLETADA'
        ), 0) AS CantidadVendida,
        ISNULL((
            SELECT SUM(VD.Cantidad * VD.PrecioVenta)
            FROM VentasDetalleClientes VD
            INNER JOIN VentasClientes VC ON VD.VentaID = VC.VentaID
            WHERE VD.ProductoID = P.ProductoID
              AND CAST(VC.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
              AND VC.Estatus = 'COMPLETADA'
        ), 0) AS TotalVendido,
        P.Nombre AS Nombre  -- Alias para compatibilidad
    FROM Productos P
    LEFT JOIN ProductosSucursal PS ON P.ProductoID = PS.ProductoID
    LEFT JOIN Sucursales S ON PS.SucursalID = S.SucursalID
    WHERE (@Codigo = '' OR P.ProductoID = @Codigo)
      AND (@SucursalID = 0 OR PS.SucursalID = @SucursalID OR @SucursalID = 0)
    GROUP BY 
        P.ProductoID, P.Nombre, P.Descripcion, P.PrecioCompra, P.PrecioVenta,
        S.RFC, S.Nombre, S.Direccion
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

PRINT '✅ SP usp_rptProductoSucursal creado';
PRINT '';

-- VERIFICAR CREACIÓN
PRINT 'Verificando stored procedures creados:';
SELECT 
    name AS ProcedureName,
    create_date AS DateCreated
FROM sys.objects
WHERE type = 'P' 
  AND name IN ('usp_rptVenta', 'usp_rptProductoSucursal')
ORDER BY name;

PRINT '';
PRINT '========================================================';
PRINT '✅ STORED PROCEDURES CREADOS EXITOSAMENTE';
PRINT '========================================================';
PRINT '';
PRINT 'PRUEBA RÁPIDA:';
PRINT '';

-- Probar usp_rptVenta con datos de hoy
EXEC usp_rptVenta @FechaInicio = '2026-01-01', @FechaFin = '2026-01-01', @SucursalID = 0;

PRINT '';
PRINT '========================================================';
GO
