-- =====================================================
-- Script: CORREGIR_ACENTOS_FORMAS_PAGO.sql
-- Descripción: Corrige la codificación de acentos en
--              el catálogo de formas de pago
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CORRIGIENDO ACENTOS EN FORMAS DE PAGO'
PRINT '========================================='
PRINT ''

-- Actualizar las descripciones con acentos correctos
UPDATE CatFormasPago 
SET Descripcion = 'Transferencia electrónica de fondos',
    UltimaAct = GETDATE()
WHERE Clave = '03'
PRINT '✓ Transferencia electrónica de fondos - CORREGIDO'

UPDATE CatFormasPago 
SET Descripcion = 'Tarjeta de crédito',
    UltimaAct = GETDATE()
WHERE Clave = '04'
PRINT '✓ Tarjeta de crédito - CORREGIDO'

UPDATE CatFormasPago 
SET Descripcion = 'Tarjeta de débito',
    UltimaAct = GETDATE()
WHERE Clave = '28'
PRINT '✓ Tarjeta de débito - CORREGIDO'

PRINT ''
PRINT '========================================='
PRINT 'VERIFICACIÓN DE CORRECCIONES'
PRINT '========================================='

SELECT 
    FormaPagoID,
    Clave,
    Descripcion,
    RequiereCambio,
    Estatus
FROM CatFormasPago
ORDER BY FormaPagoID

PRINT ''
PRINT '✅ Acentos corregidos exitosamente'
GO
