-- ============================================================
-- SCRIPT: 022_ACTUALIZAR_IVA_PRODUCTOS.sql
-- DESCRIPCIÓN: Actualización automática de tasas de IVA según tipo de producto
-- FECHA: 2025-12-28
-- ============================================================

USE DBVENTAS_WEB;
GO

PRINT '=== ACTUALIZANDO TASAS DE IVA EN PRODUCTOS ===';
GO

-- Obtener IDs de tasas
DECLARE @IVA_16 INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00);
DECLARE @IVA_8 INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 8.00);
DECLARE @IVA_0 INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00);
DECLARE @EXENTO INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Clave = 'EXENTO');

PRINT '=== 1. PRODUCTOS CON IVA 0% (TASA CERO) ===';
GO

-- ============================================================
-- CARNES, PESCADOS Y MARISCOS (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    -- Mariscos y pescados
    Nombre LIKE '%CAMARON%' OR
    Nombre LIKE '%CAMARÓN%' OR
    Nombre LIKE '%PESCADO%' OR
    Nombre LIKE '%MOJARRA%' OR
    Nombre LIKE '%TILAPIA%' OR
    Nombre LIKE '%SALMON%' OR
    Nombre LIKE '%SALMÓN%' OR
    Nombre LIKE '%ATUN FRESCO%' OR
    Nombre LIKE '%PULPO%' OR
    Nombre LIKE '%CALAMAR%' OR
    Nombre LIKE '%OSTIÓN%' OR
    Nombre LIKE '%OSTION%' OR
    Nombre LIKE '%ROBALO%' OR
    Nombre LIKE '%HUACHINANGO%' OR
    -- Carnes frescas
    Nombre LIKE '%CARNE%RES%' OR
    Nombre LIKE '%CARNE%MOLIDA%' OR
    Nombre LIKE '%BISTEC%' OR
    Nombre LIKE '%POLLO%' OR
    Nombre LIKE '%PECHUGA%' OR
    Nombre LIKE '%MUSLO%' OR
    Nombre LIKE '%PIERNA%POLLO%' OR
    Nombre LIKE '%CERDO%' OR
    Nombre LIKE '%CHULETA%' OR
    Nombre LIKE '%COSTILLA%' OR
    Nombre LIKE '%CORDERO%' OR
    Nombre LIKE '%BORREGO%' OR
    -- Descripción indica congelado
    Descripcion LIKE '%CONGELADO%' OR
    Descripcion LIKE '%FRESCO%' OR
    Descripcion LIKE '%REFRIGERADO%'
  )
  AND Nombre NOT LIKE '%ENLATADO%'
  AND Nombre NOT LIKE '%LATA%'
  AND Nombre NOT LIKE '%EMPANIZADO%'
  AND Nombre NOT LIKE '%PROCESADO%';

PRINT '✓ Carnes, pescados y mariscos frescos/congelados → IVA 0%';
GO

-- ============================================================
-- FRUTAS Y VERDURAS FRESCAS (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    -- Frutas
    Nombre LIKE '%MANZANA%' OR
    Nombre LIKE '%NARANJA%' OR
    Nombre LIKE '%PLÁTANO%' OR
    Nombre LIKE '%PLATANO%' OR
    Nombre LIKE '%FRESA%' OR
    Nombre LIKE '%UVAS%' OR
    Nombre LIKE '%MELÓN%' OR
    Nombre LIKE '%MELON%' OR
    Nombre LIKE '%SANDÍA%' OR
    Nombre LIKE '%SANDIA%' OR
    Nombre LIKE '%PIÑA%' OR
    Nombre LIKE '%PINA%' OR
    Nombre LIKE '%MANGO%' OR
    Nombre LIKE '%PAPAYA%' OR
    Nombre LIKE '%AGUACATE%' OR
    Nombre LIKE '%LIMÓN%' OR
    Nombre LIKE '%LIMON%' OR
    -- Verduras
    Nombre LIKE '%TOMATE%' OR
    Nombre LIKE '%JITOMATE%' OR
    Nombre LIKE '%CEBOLLA%' OR
    Nombre LIKE '%LECHUGA%' OR
    Nombre LIKE '%ZANAHORIA%' OR
    Nombre LIKE '%PAPA%' OR
    Nombre LIKE '%CALABAZA%' OR
    Nombre LIKE '%CHILE%' OR
    Nombre LIKE '%BRÓCOLI%' OR
    Nombre LIKE '%BROCOLI%' OR
    Nombre LIKE '%ESPINACA%' OR
    Nombre LIKE '%COLIFLOR%' OR
    Nombre LIKE '%EJOTE%' OR
    Nombre LIKE '%ELOTE%' OR
    Nombre LIKE '%AJO%'
  )
  AND Nombre NOT LIKE '%JUGO%'
  AND Nombre NOT LIKE '%SALSA%'
  AND Nombre NOT LIKE '%ENLATADO%'
  AND Nombre NOT LIKE '%CONSERVA%';

PRINT '✓ Frutas y verduras frescas → IVA 0%';
GO

-- ============================================================
-- PAN, TORTILLAS Y HARINAS (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%PAN%' OR
    Nombre LIKE '%TORTILLA%' OR
    Nombre LIKE '%HARINA%' OR
    Nombre LIKE '%BOLILLO%' OR
    Nombre LIKE '%TELERA%' OR
    Nombre LIKE '%BAGUETTE%'
  )
  AND Nombre NOT LIKE '%DULCE%'
  AND Nombre NOT LIKE '%PASTEL%'
  AND Nombre NOT LIKE '%GALLETA%'
  AND Nombre NOT LIKE '%PANQUE%';

PRINT '✓ Pan, tortillas y harinas → IVA 0%';
GO

-- ============================================================
-- LECHE, HUEVOS Y LÁCTEOS BÁSICOS (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%LECHE%' OR
    Nombre LIKE '%HUEVO%' OR
    Nombre LIKE '%HUEVOS%' OR
    Nombre LIKE '%QUESO FRESCO%' OR
    Nombre LIKE '%QUESO PANELA%' OR
    Nombre LIKE '%REQUESÓN%' OR
    Nombre LIKE '%REQUESON%' OR
    Nombre LIKE '%CREMA%ÁCIDA%' OR
    Nombre LIKE '%CREMA%ACIDA%'
  )
  AND Nombre NOT LIKE '%FÓRMULA%'
  AND Nombre NOT LIKE '%FORMULA%'
  AND Nombre NOT LIKE '%DESLACTOSADA%'
  AND Nombre NOT LIKE '%ESPECIALIZADA%';

PRINT '✓ Leche, huevos y lácteos básicos → IVA 0%';
GO

-- ============================================================
-- GRANOS, CEREALES Y LEGUMBRES (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%FRIJOL%' OR
    Nombre LIKE '%ARROZ%' OR
    Nombre LIKE '%LENTEJAS%' OR
    Nombre LIKE '%GARBANZO%' OR
    Nombre LIKE '%AVENA%' OR
    Nombre LIKE '%MAÍZ%' OR
    Nombre LIKE '%MAIZ%' OR
    Nombre LIKE '%TRIGO%'
  )
  AND Nombre NOT LIKE '%CEREAL%BOX%'
  AND Nombre NOT LIKE '%BARRA%';

PRINT '✓ Granos, cereales y legumbres → IVA 0%';
GO

-- ============================================================
-- ACEITES VEGETALES COMESTIBLES (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%ACEITE%' AND (
        Nombre LIKE '%VEGETAL%' OR
        Nombre LIKE '%GIRASOL%' OR
        Nombre LIKE '%MAÍZ%' OR
        Nombre LIKE '%MAIZ%' OR
        Nombre LIKE '%CANOLA%' OR
        Nombre LIKE '%SOYA%'
    )
  )
  AND Nombre NOT LIKE '%MOTOR%'
  AND Nombre NOT LIKE '%AUTOMOTRIZ%';

PRINT '✓ Aceites vegetales comestibles → IVA 0%';
GO

-- ============================================================
-- MEDICINAS (IVA 0%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%PARACETAMOL%' OR
    Nombre LIKE '%IBUPROFENO%' OR
    Nombre LIKE '%ASPIRINA%' OR
    Nombre LIKE '%AMOXICILINA%' OR
    Nombre LIKE '%MEDICAMENTO%' OR
    Nombre LIKE '%MEDICINA%' OR
    Nombre LIKE '%TABLETA%' OR
    Nombre LIKE '%CÁPSULA%' OR
    Nombre LIKE '%CAPSULA%' OR
    Nombre LIKE '%JARABE%' OR
    Descripcion LIKE '%MEDICAMENTO%' OR
    Descripcion LIKE '%MEDICINA%'
  )
  AND Nombre NOT LIKE '%VITAMINA%'
  AND Nombre NOT LIKE '%SUPLEMENTO%';

PRINT '✓ Medicinas de patente → IVA 0%';
GO

PRINT '';
PRINT '=== 2. PRODUCTOS EXENTOS DE IVA ===';
GO

-- ============================================================
-- LIBROS, PERIÓDICOS Y REVISTAS (EXENTO)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Clave = 'EXENTO')
WHERE Activo = 1
  AND (
    Nombre LIKE '%LIBRO%' OR
    Nombre LIKE '%PERIÓDICO%' OR
    Nombre LIKE '%PERIODICO%' OR
    Nombre LIKE '%REVISTA%' OR
    Nombre LIKE '%DIARIO%' OR
    Descripcion LIKE '%LIBRO%' OR
    Descripcion LIKE '%REVISTA%'
  );

PRINT '✓ Libros, periódicos y revistas → Exento';
GO

PRINT '';
PRINT '=== 3. PRODUCTOS CON IVA 16% (TASA GENERAL) ===';
GO

-- ============================================================
-- REFRESCOS Y BEBIDAS AZUCARADAS (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%REFRESCO%' OR
    Nombre LIKE '%COCA%' OR
    Nombre LIKE '%PEPSI%' OR
    Nombre LIKE '%SPRITE%' OR
    Nombre LIKE '%FANTA%' OR
    Nombre LIKE '%JUGO%' OR
    Nombre LIKE '%BEBIDA%' OR
    Nombre LIKE '%AGUA%SABOR%' OR
    Nombre LIKE '%TÉ%' OR
    Nombre LIKE '%TE%HELADO%' OR
    Nombre LIKE '%ENERGÉTICA%' OR
    Nombre LIKE '%ENERGETICA%'
  )
  AND Nombre NOT LIKE '%AGUA NATURAL%'
  AND Nombre NOT LIKE '%AGUA PURIFICADA%';

PRINT '✓ Refrescos y bebidas azucaradas → IVA 16%';
GO

-- ============================================================
-- DULCES, CHOCOLATES Y GALLETAS (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%DULCE%' OR
    Nombre LIKE '%CHOCOLATE%' OR
    Nombre LIKE '%GALLETA%' OR
    Nombre LIKE '%GOMITA%' OR
    Nombre LIKE '%CARAMELO%' OR
    Nombre LIKE '%PALETA%' OR
    Nombre LIKE '%CHICLE%' OR
    Nombre LIKE '%BOTANA%' OR
    Nombre LIKE '%PAPAS%FRITAS%' OR
    Nombre LIKE '%FRITURAS%' OR
    Nombre LIKE '%PASTELITO%'
  );

PRINT '✓ Dulces, chocolates y galletas → IVA 16%';
GO

-- ============================================================
-- PRODUCTOS DE LIMPIEZA (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%JABÓN%' OR
    Nombre LIKE '%JABON%' OR
    Nombre LIKE '%DETERGENTE%' OR
    Nombre LIKE '%CLORO%' OR
    Nombre LIKE '%SUAVIZANTE%' OR
    Nombre LIKE '%LIMPIADOR%' OR
    Nombre LIKE '%DESINFECTANTE%' OR
    Nombre LIKE '%SHAMPOO%' OR
    Nombre LIKE '%CHAMPÚ%' OR
    Nombre LIKE '%CHAMPU%' OR
    Nombre LIKE '%PASTA%DIENTES%' OR
    Nombre LIKE '%PAPEL%HIGIÉNICO%' OR
    Nombre LIKE '%PAPEL%HIGIENICO%'
  );

PRINT '✓ Productos de limpieza e higiene → IVA 16%';
GO

-- ============================================================
-- PRODUCTOS LÁCTEOS PROCESADOS (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%YOGURT%' OR
    Nombre LIKE '%YOGHURT%' OR
    Nombre LIKE '%QUESO%MANCHEGO%' OR
    Nombre LIKE '%QUESO%AMARILLO%' OR
    Nombre LIKE '%QUESO%OAXACA%' OR
    Nombre LIKE '%QUESO%CHIHUAHUA%' OR
    Nombre LIKE '%MANTEQUILLA%' OR
    Nombre LIKE '%MARGARINA%'
  )
  AND Nombre NOT LIKE '%QUESO FRESCO%'
  AND Nombre NOT LIKE '%QUESO PANELA%';

PRINT '✓ Productos lácteos procesados → IVA 16%';
GO

-- ============================================================
-- EMBUTIDOS Y CARNES PROCESADAS (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%JAMÓN%' OR
    Nombre LIKE '%JAMON%' OR
    Nombre LIKE '%SALCHICHA%' OR
    Nombre LIKE '%CHORIZO%' OR
    Nombre LIKE '%TOCINO%' OR
    Nombre LIKE '%MORTADELA%' OR
    Nombre LIKE '%SALAMI%' OR
    Nombre LIKE '%PASTRAMI%'
  );

PRINT '✓ Embutidos y carnes procesadas → IVA 16%';
GO

-- ============================================================
-- CONSERVAS Y ENLATADOS (IVA 16%)
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND (
    Nombre LIKE '%ENLATADO%' OR
    Nombre LIKE '%LATA%' OR
    Nombre LIKE '%CONSERVA%' OR
    Nombre LIKE '%ATÚN%LATA%' OR
    Nombre LIKE '%ATUN%LATA%' OR
    Nombre LIKE '%SARDINA%'
  );

PRINT '✓ Conservas y enlatados → IVA 16%';
GO

-- ============================================================
-- ASEGURAR QUE PRODUCTOS SIN CLASIFICAR TENGAN IVA 16%
-- ============================================================
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE Activo = 1
  AND TasaIVAID IS NULL;

PRINT '✓ Productos sin clasificar → IVA 16% (por defecto)';
GO

PRINT '';
PRINT '=== RESUMEN DE ACTUALIZACIÓN ===';
GO

-- Mostrar resumen
SELECT 
    t.Descripcion AS TasaIVA,
    t.Porcentaje AS Porcentaje,
    COUNT(*) AS TotalProductos
FROM PRODUCTO p
INNER JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID
WHERE p.Activo = 1
GROUP BY t.Descripcion, t.Porcentaje
ORDER BY t.Porcentaje DESC;

PRINT '';
PRINT '✅ ACTUALIZACIÓN COMPLETADA';
PRINT '';
PRINT '📌 Nota: Revisa manualmente productos especiales que puedan necesitar ajustes';
PRINT '';
GO
