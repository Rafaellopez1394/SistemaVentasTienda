-- =====================================================
-- Script: DATOS_EJEMPLO_GRAMAJE_Y_DESCOMPOSICION.sql
-- Descripción: Datos de ejemplo para probar las nuevas funcionalidades
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CREANDO DATOS DE EJEMPLO'
PRINT '========================================='
PRINT ''

-- =====================================================
-- 1. PRODUCTOS PARA VENTA POR GRAMAJE
-- =====================================================

PRINT '1. Configurando productos para venta por gramaje...'

-- Crear categoría si no existe
IF NOT EXISTS (SELECT 1 FROM CatCategoriasProducto WHERE Nombre = 'Abarrotes a Granel')
BEGIN
    INSERT INTO CatCategoriasProducto (Nombre, Estatus)
    VALUES ('Abarrotes a Granel', 1)
    PRINT '   ✓ Categoría "Abarrotes a Granel" creada'
END

DECLARE @CategoriaGranel INT = (SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre = 'Abarrotes a Granel')

-- Productos de ejemplo para gramaje
DECLARE @ProductosGramaje TABLE (
    Nombre VARCHAR(100),
    CodigoInterno VARCHAR(50),
    PrecioPorKilo DECIMAL(18,2)
)

INSERT INTO @ProductosGramaje VALUES
    ('Azúcar Refinada', 'AZU-001', 25.00),
    ('Arroz Blanco', 'ARR-001', 22.00),
    ('Frijol Negro', 'FRJ-001', 28.00),
    ('Harina de Trigo', 'HAR-001', 18.00),
    ('Avena en Hojuelas', 'AVE-001', 32.00),
    ('Lentejas', 'LEN-001', 35.00),
    ('Pasta Espagueti', 'PAS-001', 20.00),
    ('Café Molido', 'CAF-001', 120.00)

-- Insertar o actualizar productos
DECLARE @Nombre VARCHAR(100), @Codigo VARCHAR(50), @Precio DECIMAL(18,2)

DECLARE cursor_gramaje CURSOR FOR
SELECT Nombre, CodigoInterno, PrecioPorKilo FROM @ProductosGramaje

OPEN cursor_gramaje
FETCH NEXT FROM cursor_gramaje INTO @Nombre, @Codigo, @Precio

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Productos WHERE CodigoInterno = @Codigo)
    BEGIN
        -- Insertar nuevo producto
        INSERT INTO Productos (
            Nombre, 
            CodigoInterno, 
            CategoriaID,
            ClaveProdServSAT,
            ClaveUnidadSAT,
            TasaIVAID,
            VentaPorGramaje,
            PrecioPorKilo,
            UnidadMedidaBase,
            Estatus
        )
        VALUES (
            @Nombre,
            @Codigo,
            @CategoriaGranel,
            '01010101', -- Usar código SAT genérico
            'KGM',      -- Kilogramo
            3,          -- IVA 16%
            1,          -- Venta por gramaje activada
            @Precio,
            'KILO',
            1           -- Activo
        )
        PRINT '   ✓ Producto "' + @Nombre + '" creado con precio $' + CAST(@Precio AS VARCHAR(20)) + '/kg'
    END
    ELSE
    BEGIN
        -- Actualizar producto existente
        UPDATE Productos
        SET VentaPorGramaje = 1,
            PrecioPorKilo = @Precio,
            UnidadMedidaBase = 'KILO'
        WHERE CodigoInterno = @Codigo
        PRINT '   ✓ Producto "' + @Nombre + '" actualizado para venta por gramaje'
    END
    
    FETCH NEXT FROM cursor_gramaje INTO @Nombre, @Codigo, @Precio
END

CLOSE cursor_gramaje
DEALLOCATE cursor_gramaje

PRINT ''

-- =====================================================
-- 2. PRODUCTOS PARA DESCOMPOSICIÓN
-- =====================================================

PRINT '2. Creando productos para ejemplo de descomposición...'

-- Categoría de empaque
IF NOT EXISTS (SELECT 1 FROM CatCategoriasProducto WHERE Nombre = 'Abarrotes Empacados')
BEGIN
    INSERT INTO CatCategoriasProducto (Nombre, Estatus)
    VALUES ('Abarrotes Empacados', 1)
    PRINT '   ✓ Categoría "Abarrotes Empacados" creada'
END

DECLARE @CategoriaEmpacado INT = (SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre = 'Abarrotes Empacados')

-- PRODUCTO ORIGEN: Costal de 20kg
IF NOT EXISTS (SELECT 1 FROM Productos WHERE CodigoInterno = 'AZU-COST-20')
BEGIN
    INSERT INTO Productos (
        Nombre, 
        CodigoInterno, 
        CategoriaID,
        ClaveProdServSAT,
        ClaveUnidadSAT,
        TasaIVAID,
        VentaPorGramaje,
        UnidadMedidaBase,
        Estatus
    )
    VALUES (
        'Costal Azúcar Refinada 20kg',
        'AZU-COST-20',
        @CategoriaEmpacado,
        '01010101',
        'H87',  -- Pieza
        3,
        0,      -- No se vende por gramaje
        'UNIDAD',
        1
    )
    PRINT '   ✓ Producto "Costal Azúcar 20kg" creado (ORIGEN)'
END

-- PRODUCTOS RESULTANTES: Bolsas de 2kg y 1kg
IF NOT EXISTS (SELECT 1 FROM Productos WHERE CodigoInterno = 'AZU-BOL-2')
BEGIN
    INSERT INTO Productos (
        Nombre, 
        CodigoInterno, 
        CategoriaID,
        ClaveProdServSAT,
        ClaveUnidadSAT,
        TasaIVAID,
        VentaPorGramaje,
        PrecioPorKilo,
        UnidadMedidaBase,
        Estatus
    )
    VALUES (
        'Bolsa Azúcar Refinada 2kg',
        'AZU-BOL-2',
        @CategoriaEmpacado,
        '01010101',
        'H87',
        3,
        1,      -- Se puede vender por gramaje
        25.00,  -- Precio por kilo
        'KILO',
        1
    )
    PRINT '   ✓ Producto "Bolsa Azúcar 2kg" creado (RESULTANTE)'
END

IF NOT EXISTS (SELECT 1 FROM Productos WHERE CodigoInterno = 'AZU-BOL-1')
BEGIN
    INSERT INTO Productos (
        Nombre, 
        CodigoInterno, 
        CategoriaID,
        ClaveProdServSAT,
        ClaveUnidadSAT,
        TasaIVAID,
        VentaPorGramaje,
        PrecioPorKilo,
        UnidadMedidaBase,
        Estatus
    )
    VALUES (
        'Bolsa Azúcar Refinada 1kg',
        'AZU-BOL-1',
        @CategoriaEmpacado,
        '01010101',
        'H87',
        3,
        1,      -- Se puede vender por gramaje
        25.00,  -- Precio por kilo
        'KILO',
        1
    )
    PRINT '   ✓ Producto "Bolsa Azúcar 1kg" creado (RESULTANTE)'
END

PRINT ''

-- =====================================================
-- 3. AGREGAR STOCK INICIAL
-- =====================================================

PRINT '3. Agregando stock inicial a los productos...'

DECLARE @SucursalID INT = 1 -- Ajustar según su sucursal

-- Stock para productos de gramaje (a través de lotes)
DECLARE @ProductoID INT

-- Azúcar Refinada
SET @ProductoID = (SELECT ProductoID FROM Productos WHERE CodigoInterno = 'AZU-001')
IF @ProductoID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM LotesProducto WHERE ProductoID = @ProductoID)
BEGIN
    INSERT INTO LotesProducto (ProductoID, CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta, Estatus)
    VALUES (@ProductoID, 1000, 1000, 20.00, 30.00, 1)
    
    IF NOT EXISTS (SELECT 1 FROM ProductosSucursal WHERE ProductoID = @ProductoID AND SucursalID = @SucursalID)
    BEGIN
        INSERT INTO ProductosSucursal (ProductoID, SucursalID, Stock, Estatus)
        VALUES (@ProductoID, @SucursalID, 1000, 1)
    END
    PRINT '   ✓ Stock agregado: Azúcar Refinada (1000 unidades)'
END

-- Costal de 20kg
SET @ProductoID = (SELECT ProductoID FROM Productos WHERE CodigoInterno = 'AZU-COST-20')
IF @ProductoID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM LotesProducto WHERE ProductoID = @ProductoID)
BEGIN
    INSERT INTO LotesProducto (ProductoID, CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta, Estatus)
    VALUES (@ProductoID, 10, 10, 400.00, 550.00, 1)
    
    IF NOT EXISTS (SELECT 1 FROM ProductosSucursal WHERE ProductoID = @ProductoID AND SucursalID = @SucursalID)
    BEGIN
        INSERT INTO ProductosSucursal (ProductoID, SucursalID, Stock, Estatus)
        VALUES (@ProductoID, @SucursalID, 10, 1)
    END
    PRINT '   ✓ Stock agregado: Costal Azúcar 20kg (10 unidades)'
END

PRINT ''

-- =====================================================
-- 4. EJEMPLO DE DESCOMPOSICIÓN
-- =====================================================

PRINT '4. Ejemplo de cómo usar la descomposición...'
PRINT ''
PRINT '   Para descomponer 1 costal de 20kg en bolsas:'
PRINT ''
PRINT '   1. Ir a: Descomposición de Productos'
PRINT '   2. Seleccionar: Costal Azúcar Refinada 20kg'
PRINT '   3. Cantidad: 1'
PRINT '   4. Agregar productos resultantes:'
PRINT '      - Bolsa Azúcar 2kg: Cantidad 5, Peso 2.0 kg'
PRINT '      - Bolsa Azúcar 1kg: Cantidad 10, Peso 1.0 kg'
PRINT '   5. Registrar'
PRINT ''
PRINT '   Resultado en inventario:'
PRINT '   - Costal 20kg: 10 → 9 (-1)'
PRINT '   - Bolsa 2kg: 0 → 5 (+5)'
PRINT '   - Bolsa 1kg: 0 → 10 (+10)'
PRINT ''

-- =====================================================
-- 5. VERIFICACIÓN
-- =====================================================

PRINT '========================================='
PRINT 'VERIFICACIÓN DE PRODUCTOS CREADOS'
PRINT '========================================='
PRINT ''

PRINT 'Productos configurados para VENTA POR GRAMAJE:'
PRINT ''
SELECT 
    ProductoID,
    Nombre,
    CodigoInterno,
    FORMAT(PrecioPorKilo, 'C', 'es-MX') AS PrecioPorKilo,
    UnidadMedidaBase
FROM Productos
WHERE VentaPorGramaje = 1
ORDER BY Nombre

PRINT ''
PRINT 'Productos para DESCOMPOSICIÓN:'
PRINT ''
SELECT 
    ProductoID,
    Nombre,
    CodigoInterno,
    CASE 
        WHEN CodigoInterno LIKE '%-COST-%' THEN 'ORIGEN'
        WHEN CodigoInterno LIKE '%-BOL-%' THEN 'RESULTANTE'
        ELSE 'OTRO'
    END AS Tipo
FROM Productos
WHERE CodigoInterno LIKE 'AZU-%'
ORDER BY Tipo, Nombre

PRINT ''
PRINT '========================================='
PRINT 'DATOS DE EJEMPLO CREADOS EXITOSAMENTE'
PRINT '========================================='
PRINT ''
PRINT '✓ Productos para gramaje configurados'
PRINT '✓ Productos para descomposición creados'
PRINT '✓ Stock inicial agregado'
PRINT ''
PRINT 'Ya puede probar las funcionalidades en:'
PRINT '1. Punto de Venta (POS) - Venta por gramaje'
PRINT '2. Descomposición de Productos - Módulo de descomposición'
PRINT ''

GO
