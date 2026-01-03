-- ========================================================
-- SCRIPT: FIX UUID CONSTRAINT - Permitir múltiples NULL
-- ========================================================
-- Descripción: Reemplazar constraint UNIQUE por índice filtrado
--              para permitir múltiples facturas sin timbrar (UUID=NULL)
--              pero mantener unicidad cuando UUID no es NULL
-- Fecha: 2026-01-01
-- ========================================================

USE DB_TIENDA
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '========================================================';
PRINT 'FIX: UUID CONSTRAINT - Permitir facturas sin timbrar';
PRINT '========================================================';
PRINT '';

-- PASO 1: Eliminar constraint UNIQUE actual (no permite múltiples NULL)
PRINT 'Paso 1: Eliminando constraint UNIQUE que rechaza múltiples NULL...';
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UQ__Facturas__65A475E66BB6DB71')
BEGIN
    ALTER TABLE Facturas DROP CONSTRAINT UQ__Facturas__65A475E66BB6DB71;
    PRINT '✅ Constraint UNIQUE eliminado';
END
ELSE
BEGIN
    PRINT 'ℹ️  Constraint ya no existe';
END
PRINT '';

-- PASO 2: Crear índice único filtrado (ignora NULL, solo valida cuando UUID tiene valor)
PRINT 'Paso 2: Creando índice único filtrado (permite múltiples NULL)...';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_UUID_Unique' AND object_id = OBJECT_ID('Facturas'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Facturas_UUID_Unique
    ON Facturas(UUID)
    WHERE UUID IS NOT NULL;
    
    PRINT '✅ Índice único filtrado creado';
    PRINT '   - Permite múltiples facturas con UUID=NULL (sin timbrar)';
    PRINT '   - Garantiza unicidad cuando UUID tiene valor';
END
ELSE
BEGIN
    PRINT 'ℹ️  Índice único filtrado ya existe';
END
PRINT '';

-- PASO 3: Verificar configuración
PRINT 'Paso 3: Verificando configuración...';
PRINT '';

-- Mostrar índices actuales
SELECT 
    name AS IndexName,
    type_desc AS IndexType,
    is_unique AS IsUnique,
    has_filter AS HasFilter,
    filter_definition AS FilterDefinition
FROM sys.indexes
WHERE object_id = OBJECT_ID('Facturas')
  AND name LIKE '%UUID%'
ORDER BY name;

PRINT '';

-- Mostrar estadísticas
DECLARE @CountTotal INT, @CountNull INT, @CountNotNull INT;

SELECT 
    @CountTotal = COUNT(*),
    @CountNull = SUM(CASE WHEN UUID IS NULL THEN 1 ELSE 0 END),
    @CountNotNull = SUM(CASE WHEN UUID IS NOT NULL THEN 1 ELSE 0 END)
FROM Facturas;

PRINT '📊 Estadísticas de UUID:';
PRINT '   Total facturas: ' + CAST(@CountTotal AS VARCHAR);
PRINT '   Con UUID (timbradas): ' + CAST(@CountNotNull AS VARCHAR);
PRINT '   Sin UUID (pendientes): ' + CAST(@CountNull AS VARCHAR);
PRINT '';

PRINT '========================================================';
PRINT '✅ FIX COMPLETADO';
PRINT '========================================================';
PRINT 'RESULTADO:';
PRINT '  ✅ Ahora puedes guardar múltiples facturas sin UUID (pendientes de timbrado)';
PRINT '  ✅ La unicidad del UUID se mantiene cuando está timbrado';
PRINT '  ✅ Flujo correcto: Guardar factura → Timbrar → Actualizar UUID';
PRINT '';
PRINT 'PRÓXIMO PASO:';
PRINT '  Intentar generar factura nuevamente desde el sistema';
PRINT '========================================================';
GO
