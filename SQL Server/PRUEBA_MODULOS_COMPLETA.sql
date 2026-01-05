-- ========================================
-- SCRIPT DE PRUEBAS: Módulos de Gastos y Compras XML
-- ========================================

USE DB_TIENDA;
GO

PRINT '========================================';
PRINT 'VERIFICACIÓN DE MÓDULOS';
PRINT '========================================';
PRINT '';

-- 1. Verificar Módulo de Gastos
PRINT '1. MÓDULO DE GASTOS:';
PRINT '-------------------';

-- Tablas
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CatCategoriasGastos')
    PRINT '✓ Tabla CatCategoriasGastos';
ELSE
    PRINT '✗ Falta tabla CatCategoriasGastos';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gastos')
    PRINT '✓ Tabla Gastos';
ELSE
    PRINT '✗ Falta tabla Gastos';

-- Vista
IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_GastosDetalle')
    PRINT '✓ Vista vw_GastosDetalle';
ELSE
    PRINT '✗ Falta vista vw_GastosDetalle';

-- Stored Procedures
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_RegistrarGasto')
    PRINT '✓ SP sp_RegistrarGasto';
ELSE
    PRINT '✗ Falta SP sp_RegistrarGasto';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_CierreCajaConGastos')
    PRINT '✓ SP sp_CierreCajaConGastos';
ELSE
    PRINT '✗ Falta SP sp_CierreCajaConGastos';

-- Categorías
DECLARE @TotalCategorias INT;
SELECT @TotalCategorias = COUNT(*) FROM CatCategoriasGastos;
PRINT '✓ Categorías de gastos activas: ' + CAST(@TotalCategorias AS VARCHAR);

PRINT '';

-- 2. Verificar Módulo de Compras
PRINT '2. MÓDULO DE COMPRAS:';
PRINT '--------------------';

-- Tabla Compras
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Compras')
    PRINT '✓ Tabla Compras';
ELSE
    PRINT '✗ Falta tabla Compras';

-- Tabla LotesProducto
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LotesProducto')
    PRINT '✓ Tabla LotesProducto';
ELSE
    PRINT '✗ Falta tabla LotesProducto';

-- Tabla Proveedores
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Proveedores')
    PRINT '✓ Tabla Proveedores';
ELSE
    PRINT '✗ Falta tabla Proveedores';

-- Verificar columnas necesarias en Compras
IF EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('Compras') 
    AND name IN ('CompraID', 'ProveedorID', 'FolioFactura', 'FechaCompra', 'Total')
)
    PRINT '✓ Columnas básicas en Compras';
ELSE
    PRINT '✗ Faltan columnas en Compras';

-- Verificar columnas en LotesProducto
IF EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('LotesProducto') 
    AND name IN ('LoteID', 'ProductoID', 'CantidadTotal', 'CantidadDisponible', 'PrecioCompra', 'PrecioVenta')
)
    PRINT '✓ Columnas básicas en LotesProducto';
ELSE
    PRINT '✗ Faltan columnas en LotesProducto';

PRINT '';

-- 3. Verificar integridad de datos
PRINT '3. INTEGRIDAD DE DATOS:';
PRINT '----------------------';

-- Total de compras registradas
DECLARE @TotalCompras INT;
SELECT @TotalCompras = COUNT(*) FROM Compras;
PRINT '✓ Total de compras registradas: ' + CAST(@TotalCompras AS VARCHAR);

-- Total de lotes en inventario
DECLARE @TotalLotes INT;
SELECT @TotalLotes = COUNT(*) FROM LotesProducto WHERE Estatus = 1;
PRINT '✓ Total de lotes activos: ' + CAST(@TotalLotes AS VARCHAR);

-- Total de proveedores
DECLARE @TotalProveedores INT;
SELECT @TotalProveedores = COUNT(*) FROM Proveedores;
PRINT '✓ Total de proveedores activos: ' + CAST(@TotalProveedores AS VARCHAR);

-- Total de gastos registrados
DECLARE @TotalGastos INT;
SELECT @TotalGastos = COUNT(*) FROM Gastos WHERE Cancelado = 0;
PRINT '✓ Total de gastos activos: ' + CAST(@TotalGastos AS VARCHAR);

PRINT '';

-- 4. Prueba de inserción de gasto (simulación)
PRINT '4. PRUEBA DE FUNCIONALIDAD:';
PRINT '--------------------------';

BEGIN TRY
    DECLARE @GastoID UNIQUEIDENTIFIER;
    DECLARE @Mensaje VARCHAR(500);
    
    -- Simular registro de gasto
    EXEC sp_RegistrarGasto
        @CajaID = NULL,
        @CategoriaGastoID = 2, -- Papelería
        @Concepto = 'PRUEBA AUTOMATICA - Hojas y bolígrafos',
        @Descripcion = 'Compra de material de oficina para prueba',
        @Monto = 150.00,
        @NumeroFactura = NULL,
        @Proveedor = 'Papelería Test',
        @FormaPagoID = NULL,
        @UsuarioRegistro = 'SISTEMA_TEST',
        @Observaciones = 'Registro automático de prueba',
        @GastoID = @GastoID OUTPUT,
        @Mensaje = @Mensaje OUTPUT;
    
    IF @GastoID IS NOT NULL
    BEGIN
        PRINT '✓ Prueba de registro de gasto: EXITOSA';
        PRINT '  → GastoID: ' + CAST(@GastoID AS VARCHAR(50));
        PRINT '  → Mensaje: ' + @Mensaje;
        
        -- Cancelar el gasto de prueba
        UPDATE Gastos 
        SET Cancelado = 1, MotivoCancelacion = 'Prueba automática - Registro de validación'
        WHERE GastoID = @GastoID;
        
        PRINT '  → Gasto de prueba cancelado automáticamente';
    END
    ELSE
    BEGIN
        PRINT '✗ Prueba de registro de gasto: FALLIDA';
        PRINT '  → Mensaje: ' + @Mensaje;
    END
END TRY
BEGIN CATCH
    PRINT '✗ Error en prueba de funcionalidad: ' + ERROR_MESSAGE();
END CATCH

PRINT '';

-- 5. Resumen final
PRINT '========================================';
PRINT 'RESUMEN DE VERIFICACIÓN';
PRINT '========================================';

SELECT 
    'Módulo de Gastos' AS Modulo,
    CASE 
        WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gastos')
         AND EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_RegistrarGasto')
        THEN 'OPERATIVO' 
        ELSE 'CON ERRORES' 
    END AS Estado;

SELECT 
    'Módulo de Compras' AS Modulo,
    CASE 
        WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Compras')
         AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LotesProducto')
        THEN 'OPERATIVO' 
        ELSE 'CON ERRORES' 
    END AS Estado;

-- Estadísticas generales
PRINT '';
PRINT 'ESTADÍSTICAS GENERALES:';
PRINT '----------------------';

SELECT 
    'Categorías de Gastos' AS Concepto,
    COUNT(*) AS Total
FROM CatCategoriasGastos

UNION ALL

SELECT 
    'Gastos Activos',
    COUNT(*)
FROM Gastos
WHERE Cancelado = 0

UNION ALL

SELECT 
    'Compras Registradas',
    COUNT(*)
FROM Compras

UNION ALL

SELECT 
    'Lotes en Inventario',
    COUNT(*)
FROM LotesProducto
WHERE Estatus = 1

UNION ALL

SELECT 
    'Proveedores Activos',
    COUNT(*)
FROM Proveedores;

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN COMPLETADA';
PRINT '========================================';
