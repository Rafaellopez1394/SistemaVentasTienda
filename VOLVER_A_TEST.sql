-- =====================================================
-- VOLVER A TEST (Rollback)
-- =====================================================
-- ⚠️ Usar SOLO si necesitas revertir a ambiente TEST
-- ⚠️ Las facturas generadas en PRODUCCIÓN seguirán siendo reales
-- =====================================================

USE DB_TIENDA;
GO

PRINT '';
PRINT '╔════════════════════════════════════════════════════╗';
PRINT '║  VOLVER A AMBIENTE TEST (ROLLBACK)                 ║';
PRINT '╚════════════════════════════════════════════════════╝';
PRINT '';

PRINT '⚠️  ADVERTENCIA:';
PRINT '   Las facturas generadas en PRODUCCIÓN SEGUIRÁN siendo reales.';
PRINT '   Este script SOLO cambia el ambiente para futuras facturas.';
PRINT '';

-- Mostrar estado actual
PRINT 'Estado ACTUAL:';
SELECT 
    CASE WHEN EsProduccion = 0 THEN '🟢 TEST' ELSE '🔴 PRODUCCIÓN' END AS Ambiente,
    LEFT(ApiKey, 30) + '...' AS ApiKey,
    BaseURL
FROM ConfiguracionPAC
WHERE ConfigPACID = 1;

PRINT '';
PRINT 'Revertiendo a TEST...';
PRINT '';

-- Cambiar a TEST
UPDATE ConfiguracionPAC
SET 
    EsProduccion = 0,                                    -- ✅ Cambiar a TEST (0)
    ApiKey = 'sk_test_47126aed_6c71_4060_b05b_932c4423dd00',  -- ApiKey de TEST
    BaseURL = 'https://test.fiscalapi.com',               -- URL TEST
    FechaModificacion = GETDATE()
WHERE ConfigPACID = 1;

PRINT '✅ ConfiguracionPAC revertida a TEST';

-- Verificar cambio
PRINT '';
PRINT 'Estado NUEVO:';
SELECT 
    CASE WHEN EsProduccion = 0 THEN '🟢 TEST' ELSE '🔴 PRODUCCIÓN' END AS Ambiente,
    LEFT(ApiKey, 30) + '...' AS ApiKey,
    BaseURL
FROM ConfiguracionPAC
WHERE ConfigPACID = 1;

PRINT '';
PRINT '═════════════════════════════════════════════════════';
PRINT '';
PRINT '✅ Vueltas a TEST.';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '   1. Compila la aplicación (Rebuild Solution)';
PRINT '   2. Reinicia la aplicación (F5)';
PRINT '   3. Las nuevas facturas se generarán en TEST';
PRINT '';
PRINT '⚠️  IMPORTANTE:';
PRINT '   Las facturas que ya generaste en PRODUCCIÓN son REALES';
PRINT '   y siguen siendo válidas ante el SAT.';
PRINT '';

GO
