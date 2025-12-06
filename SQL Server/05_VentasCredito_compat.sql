-- Compatibilidad: vista VentasCredito sobre VentasClientes + cálculo de Saldo
SET NOCOUNT ON;
GO

-- Eliminar la vista si ya existe
IF OBJECT_ID('dbo.VentasCredito', 'V') IS NOT NULL
    DROP VIEW dbo.VentasCredito;
GO

-- Crear la vista VentasCredito
-- Calcula el saldo pendiente (Total - pagos aplicados) con alias 'Saldo'
CREATE VIEW dbo.VentasCredito AS
SELECT 
    v.VentaID,
    v.ClienteID,
    v.Total,
    -- Saldo = Total - pagos aplicados
    (v.Total - ISNULL((SELECT SUM(p.Importe) FROM dbo.PagosClientes p WHERE p.VentaID = v.VentaID), 0)) AS Saldo,
    v.FechaVenta,
    v.FechaVencimiento,
    v.Estatus
FROM dbo.VentasClientes v;
GO

PRINT 'Vista dbo.VentasCredito creada correctamente';
GO

-- Verificación rápida
SELECT TOP (10) VentaID, ClienteID, Total, Saldo, FechaVenta, FechaVencimiento, Estatus FROM dbo.VentasCredito;
GO

SET NOCOUNT OFF;
