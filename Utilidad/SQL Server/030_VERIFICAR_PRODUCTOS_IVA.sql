-- =====================================================
-- Script: 030_VERIFICAR_PRODUCTOS_IVA.sql
-- Descripción: Verifica y corrige configuración de IVA en productos
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'VERIFICACIÓN DE IVA EN PRODUCTOS'
PRINT '========================================='
PRINT ''

-- =====================================================
-- 1. VERIFICAR PRODUCTOS SIN CONFIGURACIÓN DE IVA
-- =====================================================

PRINT '1. Productos sin TasaIVAID configurado:'
PRINT '--------------------------------------'

SELECT 
    ProductoID,
    Nombre,
    TipoIVA,
    Exento,
    TasaIVAID,
    CASE 
        WHEN TasaIVAID IS NULL AND Exento = 0 THEN '⚠️ SIN CONFIGURAR'
        WHEN Exento = 1 THEN '✅ EXENTO'
        ELSE '✅ CONFIGURADO'
    END AS Estado
FROM Productos
WHERE (TasaIVAID IS NULL AND Exento = 0)
   OR (TipoIVA IS NULL)

PRINT ''

-- =====================================================
-- 2. MOSTRAR CONFIGURACIÓN ACTUAL DE IVA
-- =====================================================

PRINT '2. Configuración de tasas de IVA disponibles:'
PRINT '--------------------------------------------'

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapeoContableIVA]'))
BEGIN
    SELECT 
        MapeoIVAID AS 'ID',
        TasaIVA AS 'Tasa %',
        CASE WHEN Exento = 1 THEN 'Sí' ELSE 'No' END AS 'Exento',
        Descripcion,
        CASE WHEN Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS 'Estado'
    FROM MapeoContableIVA
    WHERE Activo = 1
    ORDER BY TasaIVA
END
ELSE
BEGIN
    PRINT '⚠️ La tabla MapeoContableIVA no existe.'
    PRINT '   Ejecutar script: 01_CrearTablaMapeoIVA.sql'
END

PRINT ''

-- =====================================================
-- 3. RESUMEN DE PRODUCTOS POR TIPO DE IVA
-- =====================================================

PRINT '3. Resumen de productos por configuración de IVA:'
PRINT '------------------------------------------------'

SELECT 
    CASE 
        WHEN Exento = 1 THEN 'EXENTO'
        WHEN TasaIVAID = 1 THEN 'IVA 0%'
        WHEN TasaIVAID = 2 THEN 'IVA 8%'
        WHEN TasaIVAID = 3 THEN 'IVA 16%'
        WHEN TasaIVAID IS NULL THEN '⚠️ SIN CONFIGURAR'
        ELSE 'OTRO'
    END AS 'Tipo IVA',
    COUNT(*) AS 'Cantidad Productos'
FROM Productos
WHERE Estatus = 1
GROUP BY 
    CASE 
        WHEN Exento = 1 THEN 'EXENTO'
        WHEN TasaIVAID = 1 THEN 'IVA 0%'
        WHEN TasaIVAID = 2 THEN 'IVA 8%'
        WHEN TasaIVAID = 3 THEN 'IVA 16%'
        WHEN TasaIVAID IS NULL THEN '⚠️ SIN CONFIGURAR'
        ELSE 'OTRO'
    END
ORDER BY 'Cantidad Productos' DESC

PRINT ''

-- =====================================================
-- 4. PRODUCTOS CON CONFIGURACIÓN INCONSISTENTE
-- =====================================================

PRINT '4. Productos con configuración inconsistente:'
PRINT '--------------------------------------------'

SELECT 
    ProductoID,
    Nombre,
    TipoIVA,
    Exento,
    TasaIVAID,
    '⚠️ Marcado como Exento pero tiene TasaIVAID' AS Problema
FROM Productos
WHERE Exento = 1 AND TasaIVAID IS NOT NULL AND TasaIVAID > 0

UNION ALL

SELECT 
    ProductoID,
    Nombre,
    TipoIVA,
    Exento,
    TasaIVAID,
    '⚠️ Tiene TipoIVA Trasladado pero NO tiene TasaIVAID' AS Problema
FROM Productos
WHERE TipoIVA = 'Trasladado' AND TasaIVAID IS NULL AND Exento = 0

PRINT ''

-- =====================================================
-- 5. CORRECCIÓN AUTOMÁTICA (OPCIONAL)
-- =====================================================

PRINT '5. Corrección automática de productos:'
PRINT '-------------------------------------'
PRINT ''

DECLARE @ProductosSinConfig INT
DECLARE @ProductosCorregidos INT = 0

SELECT @ProductosSinConfig = COUNT(*)
FROM Productos
WHERE (TasaIVAID IS NULL AND Exento = 0 AND Estatus = 1)
   OR (TipoIVA = 'Trasladado' AND TasaIVAID IS NULL AND Exento = 0)

IF @ProductosSinConfig > 0
BEGIN
    PRINT '⚠️ Se encontraron ' + CAST(@ProductosSinConfig AS VARCHAR) + ' productos sin configurar'
    PRINT ''
    PRINT 'OPCIONES DE CORRECCIÓN:'
    PRINT '----------------------'
    PRINT ''
    PRINT '-- Opción A: Asignar IVA 16% a productos sin configurar'
    PRINT '-- (Descomenta y ejecuta si quieres aplicar):'
    PRINT ''
    PRINT '/*'
    PRINT 'UPDATE Productos'
    PRINT 'SET TasaIVAID = 3,  -- IVA 16%'
    PRINT '    TipoIVA = ''Trasladado'''
    PRINT 'WHERE (TasaIVAID IS NULL AND Exento = 0)'
    PRINT '   OR (TipoIVA = ''Trasladado'' AND TasaIVAID IS NULL AND Exento = 0)'
    PRINT '*/'
    PRINT ''
    PRINT '-- Opción B: Marcar productos como EXENTOS'
    PRINT '-- (Descomenta y ejecuta si quieres aplicar):'
    PRINT ''
    PRINT '/*'
    PRINT 'UPDATE Productos'
    PRINT 'SET Exento = 1,'
    PRINT '    TasaIVAID = NULL,'
    PRINT '    TipoIVA = ''Exento'''
    PRINT 'WHERE (TasaIVAID IS NULL AND Exento = 0)'
    PRINT '   OR (TipoIVA = ''Trasladado'' AND TasaIVAID IS NULL AND Exento = 0)'
    PRINT '*/'
    PRINT ''
    PRINT '-- Opción C: Asignar IVA 0% a productos sin configurar'
    PRINT '-- (Descomenta y ejecuta si quieres aplicar):'
    PRINT ''
    PRINT '/*'
    PRINT 'UPDATE Productos'
    PRINT 'SET TasaIVAID = 1,  -- IVA 0%'
    PRINT '    TipoIVA = ''Trasladado'''
    PRINT 'WHERE (TasaIVAID IS NULL AND Exento = 0)'
    PRINT '   OR (TipoIVA = ''Trasladado'' AND TasaIVAID IS NULL AND Exento = 0)'
    PRINT '*/'
END
ELSE
BEGIN
    PRINT '✅ Todos los productos tienen configuración de IVA correcta'
END

PRINT ''
PRINT '========================================='
PRINT '✓ VERIFICACIÓN COMPLETADA'
PRINT '========================================='
PRINT ''
PRINT 'NOTAS IMPORTANTES:'
PRINT '-----------------'
PRINT '1. Los productos EXENTOS no llevan IVA en la factura'
PRINT '2. Los productos con TasaIVAID configurado aplicarán el IVA correspondiente'
PRINT '3. Si un producto tiene Exento=1, NO debe tener TasaIVAID'
PRINT '4. Para cambiar un producto de Exento a con IVA:'
PRINT '   UPDATE Productos SET Exento=0, TasaIVAID=3 WHERE ProductoID=X'
PRINT ''
PRINT '5. Para ver productos de alimentos marinos (generalmente IVA 0%):'
PRINT '   SELECT * FROM Productos WHERE Nombre LIKE ''%CAMARON%'' OR Nombre LIKE ''%PESCADO%'''
PRINT ''
GO
