-- ============================================
-- SCRIPT: Crear vista vw_ResumenCuentasPorPagar
-- ============================================
-- Este script crea la vista necesaria para el módulo de Cuentas Por Pagar

USE TIENDAVENTAS
GO

-- Eliminar la vista si existe
IF OBJECT_ID('vw_ResumenCuentasPorPagar', 'V') IS NOT NULL
BEGIN
    DROP VIEW vw_ResumenCuentasPorPagar
    PRINT 'Vista anterior eliminada'
END
GO

-- Crear la vista
CREATE VIEW vw_ResumenCuentasPorPagar AS
SELECT 
    cpp.CuentaPorPagarID,
    cpp.CompraID,
    cpp.ProveedorID,
    p.RazonSocial AS Proveedor,
    p.RFC,
    cpp.FechaRegistro,
    cpp.FechaVencimiento,
    DATEDIFF(DAY, GETDATE(), cpp.FechaVencimiento) AS DiasParaVencer,
    CASE 
        WHEN cpp.Estado = 'PAGADA' THEN 0
        WHEN GETDATE() > cpp.FechaVencimiento THEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE())
        ELSE 0 
    END AS DiasVencido,
    cpp.MontoTotal,
    cpp.SaldoPendiente,
    cpp.Estado,
    cpp.FolioFactura,
    ISNULL(SUM(pp.MontoPagado), 0) AS TotalPagado,
    COUNT(pp.PagoID) AS NumeroPagos
FROM CuentasPorPagar cpp
INNER JOIN PROVEEDOR p ON cpp.ProveedorID = p.ProveedorID
LEFT JOIN PagosProveedores pp ON cpp.CuentaPorPagarID = pp.CuentaPorPagarID
WHERE cpp.Activo = 1
GROUP BY 
    cpp.CuentaPorPagarID, cpp.CompraID, cpp.ProveedorID, p.RazonSocial, p.RFC,
    cpp.FechaRegistro, cpp.FechaVencimiento, cpp.MontoTotal, cpp.SaldoPendiente,
    cpp.Estado, cpp.FolioFactura
GO

-- Verificar creación
IF OBJECT_ID('vw_ResumenCuentasPorPagar', 'V') IS NOT NULL
BEGIN
    PRINT '✓ Vista vw_ResumenCuentasPorPagar creada exitosamente'
    PRINT ''
    PRINT 'Puede probar la vista con:'
    PRINT 'SELECT TOP 10 * FROM vw_ResumenCuentasPorPagar'
END
ELSE
BEGIN
    PRINT '✗ ERROR: La vista no se pudo crear'
    PRINT 'Verifique que las tablas CuentasPorPagar, PROVEEDOR y PagosProveedores existan'
END
GO
