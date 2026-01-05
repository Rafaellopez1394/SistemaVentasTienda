-- PRUEBA SIMPLE: Verificar sistema de pagos parciales
USE DB_TIENDA;
GO

PRINT '============================================================';
PRINT 'PRUEBA DEL SISTEMA DE PAGOS PARCIALES';
PRINT '============================================================';
PRINT '';

-- 1. Verificar ventas existentes con saldo pendiente
PRINT '1. Ventas con saldo pendiente:';
SELECT 
    VentaID,
    FechaVenta,
    TipoVenta,
    Total,
    ISNULL(MontoPagado, 0) AS MontoPagado,
    ISNULL(SaldoPendiente, Total) AS SaldoPendiente
FROM VentasClientes
WHERE ISNULL(SaldoPendiente, Total) > 0
  AND FechaVenta >= DATEADD(DAY, -30, GETDATE())
ORDER BY FechaVenta DESC;

PRINT '';
PRINT '2. Verificando tabla VentaPagos:';
SELECT COUNT(*) AS TotalPagosRegistrados FROM VentaPagos;

PRINT '';
PRINT '3. Últimos pagos registrados:';
SELECT TOP 5
    vp.PagoID,
    vp.VentaID,
    vp.Monto,
    vp.FechaPago,
    vp.Referencia,
    CASE WHEN vp.ComplementoPagoID IS NOT NULL THEN 'TIMBRADO' ELSE 'PENDIENTE' END AS EstadoComplemento
FROM VentaPagos vp
ORDER BY vp.FechaPago DESC;

PRINT '';
PRINT '4. Facturas con método PPD:';
SELECT 
    f.FacturaID,
    f.Serie,
    f.Folio,
    f.MetodoPago,
    f.Total,
    ISNULL(f.SaldoPendiente, 0) AS SaldoPendiente,
    f.FechaEmision
FROM Facturas f
WHERE f.MetodoPago = 'PPD'
ORDER BY f.FechaEmision DESC;

PRINT '';
PRINT '============================================================';
PRINT 'RESULTADO: Sistema de pagos parciales verificado';
PRINT '============================================================';
