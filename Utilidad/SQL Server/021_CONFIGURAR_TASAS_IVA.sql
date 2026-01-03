-- ============================================================
-- SCRIPT: 021_CONFIGURAR_TASAS_IVA.sql
-- DESCRIPCIÓN: Configuración de catálogo de tasas de IVA
-- FECHA: 2025-12-28
-- ============================================================

USE DBVENTAS_WEB;
GO

PRINT '=== CONFIGURANDO CATÁLOGO DE TASAS DE IVA ===';
GO

-- ============================================================
-- 1. CREAR TABLA CatTasaIVA SI NO EXISTE
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatTasaIVA')
BEGIN
    CREATE TABLE CatTasaIVA (
        TasaIVAID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(10) NOT NULL UNIQUE,
        Porcentaje DECIMAL(5,2) NULL, -- NULL para casos como "Exento"
        Descripcion VARCHAR(100) NOT NULL,
        Activo BIT DEFAULT 1,
        FechaRegistro DATETIME DEFAULT GETDATE()
    );

    PRINT '✓ Tabla CatTasaIVA creada';
END
ELSE
BEGIN
    PRINT '✓ Tabla CatTasaIVA ya existe';
END
GO

-- ============================================================
-- 2. INSERTAR TASAS DE IVA ESTÁNDAR EN MÉXICO
-- ============================================================

-- Limpiar datos existentes si es necesario (comentar en producción)
-- DELETE FROM CatTasaIVA;

-- Insertar tasas de IVA según normativa mexicana
IF NOT EXISTS (SELECT * FROM CatTasaIVA WHERE Clave = '002')
BEGIN
    INSERT INTO CatTasaIVA (Clave, Porcentaje, Descripcion) VALUES
    ('002', 16.00, 'IVA 16% - Tasa General'),
    ('003', 8.00, 'IVA 8% - Zona Fronteriza'),
    ('001', 0.00, 'IVA 0% - Tasa Cero'),
    ('EXENTO', NULL, 'Exento de IVA');

    PRINT '✓ Tasas de IVA insertadas:';
    PRINT '  - 16% Tasa General';
    PRINT '  - 8% Zona Fronteriza';
    PRINT '  - 0% Tasa Cero';
    PRINT '  - Exento';
END
ELSE
BEGIN
    PRINT '✓ Las tasas de IVA ya están configuradas';
END
GO

-- ============================================================
-- 3. AGREGAR COLUMNAS A TABLA PRODUCTO SI NO EXISTEN
-- ============================================================

-- Verificar si existen las columnas de IVA en la tabla PRODUCTO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTO') AND name = 'TasaIVAID')
BEGIN
    ALTER TABLE PRODUCTO ADD TasaIVAID INT NULL;
    ALTER TABLE PRODUCTO ADD CONSTRAINT FK_Producto_TasaIVA FOREIGN KEY (TasaIVAID) REFERENCES CatTasaIVA(TasaIVAID);
    PRINT '✓ Columna TasaIVAID agregada a tabla PRODUCTO';
END
ELSE
BEGIN
    PRINT '✓ Columna TasaIVAID ya existe en tabla PRODUCTO';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTO') AND name = 'CodigoInterno')
BEGIN
    ALTER TABLE PRODUCTO ADD CodigoInterno VARCHAR(50) NULL;
    PRINT '✓ Columna CodigoInterno agregada a tabla PRODUCTO';
END
ELSE
BEGIN
    PRINT '✓ Columna CodigoInterno ya existe en tabla PRODUCTO';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTO') AND name = 'ClaveProdServSATID')
BEGIN
    ALTER TABLE PRODUCTO ADD ClaveProdServSATID VARCHAR(10) NULL;
    PRINT '✓ Columna ClaveProdServSATID agregada a tabla PRODUCTO';
END
ELSE
BEGIN
    PRINT '✓ Columna ClaveProdServSATID ya existe en tabla PRODUCTO';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTO') AND name = 'ClaveUnidadSAT')
BEGIN
    ALTER TABLE PRODUCTO ADD ClaveUnidadSAT VARCHAR(10) NULL;
    PRINT '✓ Columna ClaveUnidadSAT agregada a tabla PRODUCTO';
END
ELSE
BEGIN
    PRINT '✓ Columna ClaveUnidadSAT ya existe en tabla PRODUCTO';
END
GO

-- ============================================================
-- 4. ACTUALIZAR PRODUCTOS EXISTENTES CON TASA IVA POR DEFECTO
-- ============================================================

-- Asignar IVA 16% a productos que no tienen tasa asignada
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TOP 1 TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE TasaIVAID IS NULL;

PRINT '✓ Productos actualizados con tasa IVA por defecto (16%)';
GO

-- ============================================================
-- 5. CREAR VISTA PARA CONSULTAR PRODUCTOS CON IVA
-- ============================================================

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ProductosConIVA')
    DROP VIEW vw_ProductosConIVA;
GO

CREATE VIEW vw_ProductosConIVA AS
SELECT 
    p.ProductoID,
    p.Codigo,
    p.CodigoInterno,
    p.Nombre,
    p.Descripcion,
    c.Descripcion AS Categoria,
    p.ClaveProdServSATID,
    p.ClaveUnidadSAT,
    p.TasaIVAID,
    t.Clave AS ClaveTasaIVA,
    t.Porcentaje AS PorcentajeIVA,
    t.Descripcion AS DescripcionIVA,
    p.Activo,
    p.FechaRegistro
FROM PRODUCTO p
LEFT JOIN CATEGORIA c ON p.CategoriaID = c.CategoriaID
LEFT JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID
WHERE p.Activo = 1;
GO

PRINT '✓ Vista vw_ProductosConIVA creada';
GO

-- ============================================================
-- 6. CREAR TABLA CATÁLOGO TASA IEPS (Impuestos Especiales)
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatTasaIEPS')
BEGIN
    CREATE TABLE CatTasaIEPS (
        TasaIEPSID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(10) NOT NULL UNIQUE,
        Porcentaje DECIMAL(5,2) NULL,
        Descripcion VARCHAR(100) NOT NULL,
        Activo BIT DEFAULT 1,
        FechaRegistro DATETIME DEFAULT GETDATE()
    );

    -- Insertar tasas IEPS comunes
    INSERT INTO CatTasaIEPS (Clave, Porcentaje, Descripcion) VALUES
    ('NOAPLICA', 0.00, 'No aplica IEPS'),
    ('08', 8.00, 'IEPS 8% - Alimentos no básicos'),
    ('25', 25.00, 'IEPS 25% - Bebidas saborizadas'),
    ('30', 30.00, 'IEPS 30% - Bebidas alcohólicas'),
    ('53', 53.00, 'IEPS 53% - Tabacos labrados');

    PRINT '✓ Tabla CatTasaIEPS creada con tasas comunes';
END
ELSE
BEGIN
    PRINT '✓ Tabla CatTasaIEPS ya existe';
END
GO

-- Agregar columna TasaIEPSID a PRODUCTO si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTO') AND name = 'TasaIEPSID')
BEGIN
    ALTER TABLE PRODUCTO ADD TasaIEPSID INT NULL;
    ALTER TABLE PRODUCTO ADD CONSTRAINT FK_Producto_TasaIEPS FOREIGN KEY (TasaIEPSID) REFERENCES CatTasaIEPS(TasaIEPSID);
    
    -- Asignar "No aplica" por defecto
    UPDATE PRODUCTO
    SET TasaIEPSID = (SELECT TOP 1 TasaIEPSID FROM CatTasaIEPS WHERE Clave = 'NOAPLICA')
    WHERE TasaIEPSID IS NULL;
    
    PRINT '✓ Columna TasaIEPSID agregada a tabla PRODUCTO';
END
ELSE
BEGIN
    PRINT '✓ Columna TasaIEPSID ya existe en tabla PRODUCTO';
END
GO

-- ============================================================
-- 7. EJEMPLOS DE USO
-- ============================================================

PRINT '';
PRINT '=== CONFIGURACIÓN COMPLETADA ===';
PRINT '';
PRINT '📋 Catálogo de Tasas de IVA:';
PRINT '  1. IVA 16% - Tasa General (mayoría de productos)';
PRINT '  2. IVA 8% - Zona Fronteriza';
PRINT '  3. IVA 0% - Tasa Cero (alimentos básicos, medicinas)';
PRINT '  4. Exento - Sin IVA (libros, revistas)';
PRINT '';
PRINT '📌 Ejemplos de productos por tipo de IVA:';
PRINT '';
PRINT '  IVA 16% (Tasa General):';
PRINT '    - Refrescos, jugos';
PRINT '    - Dulces, galletas';
PRINT '    - Productos de limpieza';
PRINT '    - Electrónicos';
PRINT '';
PRINT '  IVA 0% (Tasa Cero):';
PRINT '    - Pan, tortillas';
PRINT '    - Leche, huevos';
PRINT '    - Carne, pollo, pescado';
PRINT '    - Frutas y verduras frescas';
PRINT '    - Medicinas de patente';
PRINT '';
PRINT '  Exento:';
PRINT '    - Libros, periódicos';
PRINT '    - Revistas';
PRINT '';
PRINT '🔧 Próximos pasos:';
PRINT '  1. Configurar la tasa de IVA correcta para cada producto';
PRINT '  2. Usar el módulo de productos para asignar tasas';
PRINT '  3. Los cálculos de venta aplicarán automáticamente la tasa correcta';
PRINT '';
GO
