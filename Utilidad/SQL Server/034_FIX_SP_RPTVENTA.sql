-- ========================================================
-- SCRIPT: Crear SP usp_rptVenta CORREGIDO
-- ========================================================
USE DB_TIENDA
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
        CASE WHEN VC.TipoVenta = 'CONTADO' THEN 'Ticket' ELSE 'Factura' END AS [Tipo Documento],
        'Principal' AS [Nombre Sucursal],
        'N/A' AS [RFC Sucursal],
        ISNULL((SELECT TOP 1 Nombres + ' ' + Apellidos FROM USUARIO WHERE UsuarioID = VC.Usuario), 'Sistema') AS [Nombre Empleado],
        ISNULL((SELECT SUM(VD.Cantidad) FROM VentasDetalleClientes VD WHERE VD.VentaID = VC.VentaID), 0) AS [Cantidad Unidades Vendidas],
        ISNULL((SELECT COUNT(*) FROM VentasDetalleClientes VD WHERE VD.VentaID = VC.VentaID), 0) AS [Cantidad Productos],
        ISNULL(VC.Total, 0) AS [Total Venta]
    FROM VentasClientes VC
    WHERE CAST(VC.FechaVenta AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND VC.Estatus = 1  -- 1 = Completada
    ORDER BY VC.FechaVenta DESC;
END
GO

PRINT 'SP usp_rptVenta creado correctamente';

-- Prueba
EXEC usp_rptVenta '2026-01-01', '2026-01-01', 0;
GO
