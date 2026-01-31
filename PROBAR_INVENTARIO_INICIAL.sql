-- ============================================
-- PRUEBA DEL MÓDULO DE INVENTARIO INICIAL
-- ============================================

USE DB_TIENDA;
GO

PRINT '================================================';
PRINT 'PRUEBA DEL MÓDULO DE INVENTARIO INICIAL';
PRINT '================================================';
PRINT '';

-- 1. Verificar que las tablas existen
PRINT '1. Verificando tablas...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InventarioInicial')
    PRINT '   ✓ Tabla InventarioInicial existe';
ELSE
    PRINT '   ✗ ERROR: Tabla InventarioInicial NO existe';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InventarioInicialDetalle')
    PRINT '   ✓ Tabla InventarioInicialDetalle existe';
ELSE
    PRINT '   ✗ ERROR: Tabla InventarioInicialDetalle NO existe';

PRINT '';

-- 2. Verificar stored procedures
PRINT '2. Verificando stored procedures...';
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_IniciarCargaInventarioInicial')
    PRINT '   ✓ SP_IniciarCargaInventarioInicial existe';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_AgregarProductoInventarioInicial')
    PRINT '   ✓ SP_AgregarProductoInventarioInicial existe';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_FinalizarCargaInventarioInicial')
    PRINT '   ✓ SP_FinalizarCargaInventarioInicial existe';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_ObtenerProductosParaInventarioInicial')
    PRINT '   ✓ SP_ObtenerProductosParaInventarioInicial existe';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_EliminarProductoInventarioInicial')
    PRINT '   ✓ SP_EliminarProductoInventarioInicial existe';

PRINT '';

-- 3. Verificar vista
PRINT '3. Verificando vista...';
IF EXISTS (SELECT * FROM sys.views WHERE name = 'VW_HistorialInventarioInicial')
    PRINT '   ✓ Vista VW_HistorialInventarioInicial existe';
ELSE
    PRINT '   ✗ ERROR: Vista VW_HistorialInventarioInicial NO existe';

PRINT '';

-- 4. Obtener sucursales disponibles
PRINT '4. Sucursales disponibles:';
SELECT SucursalID, Nombre AS NombreSucursal
FROM SUCURSAL
WHERE Activo = 1;

PRINT '';

-- 5. Obtener primeros 5 productos para prueba
PRINT '5. Primeros 5 productos disponibles para prueba:';
SELECT TOP 5 
    ProductoID,
    Nombre AS NombreProducto,
    CodigoInterno
FROM Productos
WHERE Estatus = 1
ORDER BY ProductoID;

PRINT '';

-- 6. Ver estado actual del inventario
PRINT '6. Estado actual del inventario:';
SELECT 
    COUNT(DISTINCT LoteID) AS LotesActuales,
    SUM(CantidadDisponible) AS UnidadesTotales
FROM LotesProducto
WHERE Estatus = 1;

PRINT '';

-- 7. Ver cargas existentes
PRINT '7. Cargas de inventario inicial existentes:';
IF EXISTS (SELECT 1 FROM InventarioInicial)
BEGIN
    SELECT * FROM VW_HistorialInventarioInicial;
END
ELSE
BEGIN
    PRINT '   (No hay cargas registradas aún)';
END

PRINT '';
PRINT '================================================';
PRINT 'FIN DE VERIFICACIÓN';
PRINT '================================================';
PRINT '';
PRINT 'INSTRUCCIONES PARA PRUEBA:';
PRINT '1. Ejecuta el sistema web';
PRINT '2. Ve al menú: Inventario > Inventario Inicial';
PRINT '3. Haz clic en "Nueva Carga Inicial"';
PRINT '4. Agrega algunos productos con cantidad y costos';
PRINT '5. Finaliza la carga';
PRINT '6. Verifica que se crearon lotes en LotesProducto';
PRINT '';

-- EJEMPLO DE PRUEBA MANUAL (OPCIONAL - No ejecutar automáticamente)
/*
-- ============================================
-- PRUEBA MANUAL DEL SISTEMA
-- ============================================

-- A. Iniciar nueva carga
DECLARE @CargaID INT;
EXEC SP_IniciarCargaInventarioInicial 
    @UsuarioCarga = 'admin',
    @SucursalID = 1,
    @Comentarios = 'Prueba del sistema - No ejecutar en producción',
    @CargaID = @CargaID OUTPUT;

PRINT 'CargaID creado: ' + CAST(@CargaID AS VARCHAR);

-- B. Agregar productos de prueba (ajusta los ProductoID según tu BD)
EXEC SP_AgregarProductoInventarioInicial
    @CargaID = @CargaID,
    @ProductoID = 1,  -- Ajustar según tu ProductoID
    @Cantidad = 10,
    @CostoUnitario = 5.50,
    @PrecioVenta = 12.00,
    @Comentarios = 'Producto de prueba 1';

EXEC SP_AgregarProductoInventarioInicial
    @CargaID = @CargaID,
    @ProductoID = 2,  -- Ajustar según tu ProductoID
    @Cantidad = 25,
    @CostoUnitario = 8.75,
    @PrecioVenta = 18.00,
    @Comentarios = 'Producto de prueba 2';

-- C. Ver productos agregados
SELECT * FROM InventarioInicialDetalle WHERE CargaID = @CargaID;

-- D. Finalizar carga
EXEC SP_FinalizarCargaInventarioInicial
    @CargaID = @CargaID,
    @Usuario = 'admin';

-- E. Verificar que se aplicó correctamente
SELECT * FROM VW_HistorialInventarioInicial WHERE CargaID = @CargaID;

-- F. Ver lotes creados
SELECT TOP 10 * 
FROM LotesProducto 
WHERE CAST(FechaEntrada AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY LoteID DESC;

-- G. Ver movimientos registrados
SELECT TOP 10 * 
FROM InventarioMovimientos 
WHERE TipoMovimiento = 'INVENTARIO_INICIAL'
  AND CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY MovimientoID DESC;
*/
