-- Script para verificar ventas marcadas como facturadas y su estado real

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '=== Verificando ventas facturadas ==='
PRINT ''

-- 1. Ventas marcadas como facturadas CON factura real
PRINT '1. Ventas con factura correcta (EstaFacturada=1 Y tienen factura):'
SELECT 
    V.VentaID,
    V.FechaVenta,
    C.RazonSocial AS Cliente,
    V.Total,
    V.EstaFacturada,
    F.FacturaID,
    F.Serie + '-' + F.Folio AS SeriFolio,
    F.UUID,
    F.Estatus AS EstatusFactura,
    CASE 
        WHEN F.XMLTimbrado IS NULL OR F.XMLTimbrado = '' THEN 'SIN XML'
        ELSE 'CON XML'
    END AS TieneXML
FROM VentasClientes V
INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
INNER JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.EstaFacturada = 1
ORDER BY V.FechaVenta DESC;

PRINT ''
PRINT '---'
PRINT ''

-- 2. Ventas marcadas como facturadas SIN factura real (inconsistencia)
PRINT '2. INCONSISTENCIA - Ventas marcadas como facturadas SIN factura en tabla:'
SELECT 
    V.VentaID,
    V.FechaVenta,
    C.RazonSocial AS Cliente,
    V.Total,
    V.EstaFacturada
FROM VentasClientes V
INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
LEFT JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.EstaFacturada = 1 
  AND F.FacturaID IS NULL
ORDER BY V.FechaVenta DESC;

PRINT ''
PRINT '---'
PRINT ''

-- 3. Facturas sin XML timbrado
PRINT '3. Facturas sin XML timbrado:'
SELECT 
    F.FacturaID,
    F.Serie + '-' + F.Folio AS SerieFolio,
    F.UUID,
    F.Estatus,
    F.FechaCreacion,
    V.VentaID
FROM Facturas F
LEFT JOIN VentasClientes V ON F.VentaID = V.VentaID
WHERE F.XMLTimbrado IS NULL OR F.XMLTimbrado = ''
ORDER BY F.FechaCreacion DESC;

PRINT ''
PRINT '---'
PRINT ''

-- 4. Resumen de inconsistencias
DECLARE @VentasMarcadasSinFactura INT, @FacturasSinXML INT;

SELECT @VentasMarcadasSinFactura = COUNT(*)
FROM VentasClientes V
LEFT JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.EstaFacturada = 1 AND F.FacturaID IS NULL;

SELECT @FacturasSinXML = COUNT(*)
FROM Facturas
WHERE XMLTimbrado IS NULL OR XMLTimbrado = '';

PRINT '=== RESUMEN DE INCONSISTENCIAS ==='
PRINT ''
PRINT 'Ventas marcadas como facturadas sin factura real: ' + CAST(@VentasMarcadasSinFactura AS VARCHAR(10))
PRINT 'Facturas sin XML timbrado: ' + CAST(@FacturasSinXML AS VARCHAR(10))
PRINT ''

-- 5. Buscar la venta específica del usuario
PRINT '=== Venta específica: 9f035d37-8764-4aa6-b71a-041dffd940b0 ==='
SELECT 
    V.VentaID,
    V.FechaVenta,
    C.RazonSocial AS Cliente,
    V.Total,
    V.EstaFacturada,
    F.FacturaID,
    F.Serie + '-' + F.Folio AS SerieFolio,
    F.UUID,
    F.Estatus AS EstatusFactura,
    CASE 
        WHEN F.XMLTimbrado IS NULL OR F.XMLTimbrado = '' THEN 'SIN XML'
        WHEN LEN(F.XMLTimbrado) < 100 THEN 'XML INCOMPLETO (' + CAST(LEN(F.XMLTimbrado) AS VARCHAR(10)) + ' chars)'
        ELSE 'CON XML (' + CAST(LEN(F.XMLTimbrado) AS VARCHAR(10)) + ' chars)'
    END AS TieneXML
FROM VentasClientes V
INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
LEFT JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.VentaID = '9f035d37-8764-4aa6-b71a-041dffd940b0';

PRINT ''
PRINT '✓ Verificación completada'
