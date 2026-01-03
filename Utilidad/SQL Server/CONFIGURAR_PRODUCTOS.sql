-- ============================================================================
-- SCRIPT DE CONFIGURACIÓN INICIAL
-- Configurar productos de ejemplo para gramaje
-- ============================================================================

USE DB_TIENDA
GO

PRINT '============================================='
PRINT 'CONFIGURACIÓN DE PRODUCTOS'
PRINT '============================================='
PRINT ''

-- ============================================================================
-- OPCIÓN 1: Configurar productos existentes para venta por gramaje
-- ============================================================================

PRINT 'Configurando productos existentes para venta por gramaje...'
PRINT ''

-- Buscar y configurar productos comunes
-- Ajuste los nombres según sus productos reales

-- Azúcar
IF EXISTS (SELECT 1 FROM Productos WHERE Nombre LIKE '%Az%car%' OR Nombre LIKE '%Azucar%')
BEGIN
    UPDATE Productos 
    SET VentaPorGramaje = 1,
        PrecioPorKilo = 25.00,
        UnidadMedidaBase = 'KILO'
    WHERE Nombre LIKE '%Az%car%' OR Nombre LIKE '%Azucar%'
    
    PRINT '✓ Azúcar configurada para venta por gramaje'
END

-- Arroz
IF EXISTS (SELECT 1 FROM Productos WHERE Nombre LIKE '%Arroz%')
BEGIN
    UPDATE Productos 
    SET VentaPorGramaje = 1,
        PrecioPorKilo = 22.00,
        UnidadMedidaBase = 'KILO'
    WHERE Nombre LIKE '%Arroz%'
    
    PRINT '✓ Arroz configurado para venta por gramaje'
END

-- Frijol
IF EXISTS (SELECT 1 FROM Productos WHERE Nombre LIKE '%Frijol%')
BEGIN
    UPDATE Productos 
    SET VentaPorGramaje = 1,
        PrecioPorKilo = 28.00,
        UnidadMedidaBase = 'KILO'
    WHERE Nombre LIKE '%Frijol%'
    
    PRINT '✓ Frijol configurado para venta por gramaje'
END

PRINT ''

-- ============================================================================
-- OPCIÓN 2: Configurar productos específicos por ID
-- ============================================================================

PRINT 'Para configurar productos específicos, use:'
PRINT ''
PRINT 'UPDATE Productos'
PRINT 'SET VentaPorGramaje = 1,'
PRINT '    PrecioPorKilo = [PRECIO],'
PRINT '    UnidadMedidaBase = ''KILO'''
PRINT 'WHERE ProductoID = [ID]'
PRINT ''

-- ============================================================================
-- VERIFICAR CONFIGURACIÓN
-- ============================================================================

PRINT '============================================='
PRINT 'PRODUCTOS CONFIGURADOS PARA GRAMAJE'
PRINT '============================================='
PRINT ''

IF EXISTS (SELECT 1 FROM Productos WHERE VentaPorGramaje = 1)
BEGIN
    SELECT 
        ProductoID,
        Nombre,
        FORMAT(PrecioPorKilo, 'C', 'es-MX') AS [Precio/Kilo],
        UnidadMedidaBase AS Unidad,
        CASE WHEN Estatus = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
    FROM Productos
    WHERE VentaPorGramaje = 1
    ORDER BY Nombre
    
    PRINT ''
    PRINT '✓ Productos configurados correctamente'
END
ELSE
BEGIN
    PRINT '⚠ No hay productos configurados para venta por gramaje'
    PRINT ''
    PRINT 'Puede ejecutar el script de datos de ejemplo:'
    PRINT 'DATOS_EJEMPLO_GRAMAJE_Y_DESCOMPOSICION.sql'
END

PRINT ''
PRINT '============================================='
PRINT 'CONFIGURACIÓN COMPLETADA'
PRINT '============================================='
PRINT ''
PRINT 'Ahora puede:'
PRINT '1. Ir al POS'
PRINT '2. Buscar un producto configurado'
PRINT '3. Probar la venta por gramaje'
PRINT ''

GO
