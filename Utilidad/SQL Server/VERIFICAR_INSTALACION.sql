-- ============================================================================
-- SCRIPT DE VERIFICACIÓN POST-INSTALACIÓN
-- Verificar que todo se instaló correctamente
-- ============================================================================

USE DB_TIENDA
GO

PRINT '============================================='
PRINT 'VERIFICACIÓN DE INSTALACIÓN'
PRINT '============================================='
PRINT ''

DECLARE @Errores INT = 0

-- ============================================================================
-- 1. VERIFICAR CAMPOS EN TABLA PRODUCTOS
-- ============================================================================
PRINT '1. Verificando campos en tabla Productos...'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'VentaPorGramaje')
    PRINT '   ✓ Campo VentaPorGramaje existe'
ELSE
BEGIN
    PRINT '   ✗ Campo VentaPorGramaje NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'PrecioPorKilo')
    PRINT '   ✓ Campo PrecioPorKilo existe'
ELSE
BEGIN
    PRINT '   ✗ Campo PrecioPorKilo NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'UnidadMedidaBase')
    PRINT '   ✓ Campo UnidadMedidaBase existe'
ELSE
BEGIN
    PRINT '   ✗ Campo UnidadMedidaBase NO existe'
    SET @Errores = @Errores + 1
END

PRINT ''

-- ============================================================================
-- 2. VERIFICAR CAMPOS EN TABLA DETALLEVENTA
-- ============================================================================
PRINT '2. Verificando campos en tabla DetalleVenta...'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetalleVenta' AND COLUMN_NAME = 'Gramos')
    PRINT '   ✓ Campo Gramos existe'
ELSE
BEGIN
    PRINT '   ✗ Campo Gramos NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetalleVenta' AND COLUMN_NAME = 'PrecioCalculado')
    PRINT '   ✓ Campo PrecioCalculado existe'
ELSE
BEGIN
    PRINT '   ✗ Campo PrecioCalculado NO existe'
    SET @Errores = @Errores + 1
END

PRINT ''

-- ============================================================================
-- 3. VERIFICAR TABLAS DE DESCOMPOSICIÓN
-- ============================================================================
PRINT '3. Verificando tablas de descomposición...'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DescomposicionProducto')
    PRINT '   ✓ Tabla DescomposicionProducto existe'
ELSE
BEGIN
    PRINT '   ✗ Tabla DescomposicionProducto NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DetalleDescomposicion')
    PRINT '   ✓ Tabla DetalleDescomposicion existe'
ELSE
BEGIN
    PRINT '   ✗ Tabla DetalleDescomposicion NO existe'
    SET @Errores = @Errores + 1
END

PRINT ''

-- ============================================================================
-- 4. VERIFICAR STORED PROCEDURES
-- ============================================================================
PRINT '4. Verificando Stored Procedures...'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'SP_RegistrarDescomposicionProducto')
    PRINT '   ✓ SP_RegistrarDescomposicionProducto existe'
ELSE
BEGIN
    PRINT '   ✗ SP_RegistrarDescomposicionProducto NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'SP_CalcularPrecioPorGramaje')
    PRINT '   ✓ SP_CalcularPrecioPorGramaje existe'
ELSE
BEGIN
    PRINT '   ✗ SP_CalcularPrecioPorGramaje NO existe'
    SET @Errores = @Errores + 1
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'BuscarProductoPOS')
    PRINT '   ✓ BuscarProductoPOS existe'
ELSE
BEGIN
    PRINT '   ✗ BuscarProductoPOS NO existe'
    SET @Errores = @Errores + 1
END

PRINT ''

-- ============================================================================
-- 5. VERIFICAR VISTAS
-- ============================================================================
PRINT '5. Verificando Vistas...'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'vw_HistorialDescomposiciones')
    PRINT '   ✓ vw_HistorialDescomposiciones existe'
ELSE
BEGIN
    PRINT '   ✗ vw_HistorialDescomposiciones NO existe'
    SET @Errores = @Errores + 1
END

PRINT ''

-- ============================================================================
-- 6. PROBAR STORED PROCEDURE DE CÁLCULO
-- ============================================================================
PRINT '6. Probando cálculo de precio por gramaje...'

BEGIN TRY
    DECLARE @PrecioTest DECIMAL(18,2)
    DECLARE @MensajeTest VARCHAR(500)
    
    -- Crear producto temporal para prueba
    DECLARE @ProductoTestID INT
    
    INSERT INTO Productos (Nombre, CategoriaID, ClaveProdServSAT, ClaveUnidadSAT, TasaIVAID, VentaPorGramaje, PrecioPorKilo, UnidadMedidaBase, Estatus)
    VALUES ('TEST_PRODUCTO_TEMPORAL', 1, '01010101', 'KGM', 3, 1, 100.00, 'KILO', 1)
    
    SET @ProductoTestID = SCOPE_IDENTITY()
    
    -- Probar cálculo: 500g a $100/kg = $50
    EXEC SP_CalcularPrecioPorGramaje 
        @ProductoID = @ProductoTestID,
        @Gramos = 500,
        @PrecioCalculado = @PrecioTest OUTPUT,
        @Mensaje = @MensajeTest OUTPUT
    
    IF @PrecioTest = 50.00
        PRINT '   ✓ Cálculo correcto: 500g x $100/kg = $50.00'
    ELSE
    BEGIN
        PRINT '   ✗ Error en cálculo: resultado = $' + CAST(@PrecioTest AS VARCHAR(20))
        SET @Errores = @Errores + 1
    END
    
    -- Limpiar producto temporal
    DELETE FROM Productos WHERE ProductoID = @ProductoTestID
END TRY
BEGIN CATCH
    PRINT '   ✗ Error al probar cálculo: ' + ERROR_MESSAGE()
    SET @Errores = @Errores + 1
END CATCH

PRINT ''

-- ============================================================================
-- RESUMEN FINAL
-- ============================================================================
PRINT '============================================='
IF @Errores = 0
BEGIN
    PRINT '✓ INSTALACIÓN COMPLETADA EXITOSAMENTE'
    PRINT '============================================='
    PRINT ''
    PRINT 'Todos los componentes están instalados correctamente.'
    PRINT ''
    PRINT 'Siguientes pasos:'
    PRINT '1. Compilar proyecto en Visual Studio'
    PRINT '2. Configurar productos (ejecutar CONFIGURAR_PRODUCTOS.sql)'
    PRINT '3. Probar funcionalidades en el sistema'
    PRINT ''
    PRINT 'Documentación: README_GRAMAJE_DESCOMPOSICION.md'
END
ELSE
BEGIN
    PRINT '✗ INSTALACIÓN CON ERRORES (' + CAST(@Errores AS VARCHAR(10)) + ' errores)'
    PRINT '============================================='
    PRINT ''
    PRINT 'Se encontraron ' + CAST(@Errores AS VARCHAR(10)) + ' errores.'
    PRINT 'Revise los mensajes anteriores y:'
    PRINT '1. Asegúrese de ejecutar primero:'
    PRINT '   - 024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql'
    PRINT '   - 024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql'
    PRINT '2. Verifique que no haya errores en la ejecución'
    PRINT '3. Ejecute este script nuevamente para verificar'
END
PRINT ''

GO
