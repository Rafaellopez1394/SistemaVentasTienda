-- =====================================================
-- Script para verificar y ajustar el collation
-- =====================================================

USE DB_TIENDA;
GO

PRINT '🔍 1. COLLATION DE LA BASE DE DATOS:';
SELECT 
    name AS 'Base de datos',
    collation_name AS 'Collation actual',
    CASE 
        WHEN collation_name LIKE '%UTF8%' THEN '✅ Soporta UTF-8'
        WHEN collation_name LIKE '%Latin1_General%' THEN '⚠️ Latin1 (puede tener problemas con acentos)'
        ELSE '❓ Verificar compatibilidad'
    END AS 'Estado'
FROM sys.databases 
WHERE name = 'DB_TIENDA';
GO

PRINT '';
PRINT '🔍 2. COLLATION DE COLUMNAS DE TEXTO EN TABLAS Cat*:';
SELECT 
    t.name AS 'Tabla',
    c.name AS 'Columna',
    c.collation_name AS 'Collation',
    ty.name AS 'Tipo de dato'
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.name LIKE 'Cat%'
    AND c.collation_name IS NOT NULL
ORDER BY t.name, c.name;
GO

PRINT '';
PRINT '🔍 3. VERIFICANDO DATOS CON PROBLEMAS DE ENCODING:';

-- CatFormasPago
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatFormasPago')
BEGIN
    PRINT '   📋 CatFormasPago:';
    SELECT 
        FormaPagoID,
        Descripcion,
        CASE 
            WHEN Descripcion LIKE '%Ã%' THEN '❌ Tiene problemas'
            ELSE '✅ OK'
        END AS 'Estado'
    FROM CatFormasPago;
END
GO

-- CatMetodosPago
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatMetodosPago')
BEGIN
    PRINT '';
    PRINT '   📋 CatMetodosPago:';
    SELECT 
        MetodoPagoID,
        Descripcion,
        CASE 
            WHEN Descripcion LIKE '%Ã%' THEN '❌ Tiene problemas'
            ELSE '✅ OK'
        END AS 'Estado'
    FROM CatMetodosPago;
END
GO

-- CatMonedas
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatMonedas')
BEGIN
    PRINT '';
    PRINT '   📋 CatMonedas:';
    SELECT 
        MonedaID,
        Nombre,
        CASE 
            WHEN Nombre LIKE '%Ã%' THEN '❌ Tiene problemas'
            ELSE '✅ OK'
        END AS 'Estado'
    FROM CatMonedas;
END
GO

PRINT '';
PRINT '📝 RECOMENDACIONES:';
PRINT '   1. Si hay problemas, ejecuta: CorregirEncodingCatalogos.sql';
PRINT '   2. Para evitar futuros problemas, usa NVARCHAR en lugar de VARCHAR';
PRINT '   3. Considera cambiar el collation a Modern_Spanish_CI_AS o Latin1_General_100_CI_AS_SC_UTF8';
GO
