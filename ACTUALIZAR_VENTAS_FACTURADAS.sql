-- Script para actualizar ventas que ya están facturadas
-- Marca EstaFacturada = 1 para todas las ventas que tienen facturas asociadas

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '=== Actualizando ventas facturadas ==='
PRINT ''

-- Verificar cuántas ventas están sin marcar como facturadas pero tienen facturas
DECLARE @VentasSinMarcar INT;
SELECT @VentasSinMarcar = COUNT(DISTINCT V.VentaID)
FROM VentasClientes V
INNER JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.EstaFacturada = 0 OR V.EstaFacturada IS NULL;

PRINT 'Ventas con facturas pero sin marcar: ' + CAST(@VentasSinMarcar AS VARCHAR(10))
PRINT ''

-- Actualizar todas las ventas que tienen facturas asociadas
UPDATE VentasClientes
SET EstaFacturada = 1
WHERE VentaID IN (
    SELECT DISTINCT VentaID 
    FROM Facturas 
    WHERE VentaID IS NOT NULL
)
AND (EstaFacturada = 0 OR EstaFacturada IS NULL);

PRINT '✓ Ventas actualizadas: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
PRINT ''

-- Mostrar resumen
PRINT '=== Resumen ==='
PRINT ''

DECLARE @TotalVentas INT, @VentasFacturadas INT, @VentasSinFacturar INT;

SELECT @TotalVentas = COUNT(*)
FROM VentasClientes;

SELECT @VentasFacturadas = COUNT(*)
FROM VentasClientes
WHERE EstaFacturada = 1;

SELECT @VentasSinFacturar = COUNT(*)
FROM VentasClientes
WHERE EstaFacturada = 0 OR EstaFacturada IS NULL;

PRINT 'Total de ventas: ' + CAST(@TotalVentas AS VARCHAR(10))
PRINT 'Ventas facturadas: ' + CAST(@VentasFacturadas AS VARCHAR(10))
PRINT 'Ventas sin facturar: ' + CAST(@VentasSinFacturar AS VARCHAR(10))
PRINT ''

-- Mostrar algunas ventas facturadas para verificar
PRINT '=== Últimas 5 ventas facturadas ==='
SELECT TOP 5
    V.VentaID,
    V.FechaVenta,
    C.RazonSocial AS Cliente,
    V.Total,
    V.EstaFacturada,
    F.UUID,
    F.FechaTimbrado
FROM VentasClientes V
INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
INNER JOIN Facturas F ON V.VentaID = F.VentaID
ORDER BY V.FechaVenta DESC;

PRINT ''
PRINT '✓ Actualización completada'
