-- ============================================================
-- SCRIPT: Importar productos de camarón con IVA 0%
-- Fecha: 2025-12-28
-- ============================================================

USE DBVENTAS_WEB;
GO

DECLARE @IVA_0 INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00);
DECLARE @IVA_16 INT = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00);
DECLARE @CategoriaCongelado INT = (SELECT TOP 1 IdCategoria FROM CATEGORIA WHERE Descripcion LIKE '%Fruta%' OR Activo = 1);
DECLARE @CategoriaAbarrotes INT = @CategoriaCongelado; -- Ajustar según necesites

PRINT '=== IMPORTANDO PRODUCTOS DE CAMARÓN ===';
PRINT '';

-- Insertar productos de camarón (todos con IVA 0% porque son mariscos congelados)

-- 8810 ya existe
IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888', 8888, 'CAMARON CHICO 111-130', 'CONGELADO', @CategoriaCongelado, '8888', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-16')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-16', 888816, 'CAMARON 16-20', 'CONGELADO', @CategoriaCongelado, '8888-16', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-21-COCIDO')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-21-COCIDO', 888821, 'CAMARON COCIDO 21-25', 'CONGELADO', @CategoriaCongelado, '8888-21-COCIDO', '50121903', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-26')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-26', 888826, 'CAMARON 26-30', 'CONGELADO', @CategoriaCongelado, '8888-26', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-31')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-31', 888831, 'CAMARON MEDIANO 31-35', 'CONGELADO', @CategoriaCongelado, '8888-31', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-36')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-36', 888836, 'CAMARON 36-40', 'CONGELADO', @CategoriaCongelado, '8888-36', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-41')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-41', 888841, 'CAMARON MEDIANO 41-50', 'CONGELADO', @CategoriaCongelado, '8888-41', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-5')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-5', 88885, 'CAMARON MARIPOSA', 'CONGELADO', @CategoriaCongelado, '8888-5', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-51')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-51', 888851, 'CAMARON PELADO', 'CONGELADO', @CategoriaCongelado, '8888-51', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-51-60')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-51-60', 8888516, 'CAMARON 51-60', 'ABARROTES', @CategoriaAbarrotes, '8888-51-60', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-61')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-61', 888861, 'CAMARON 61-70', 'CONGELADO', @CategoriaCongelado, '8888-61', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-71')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-71', 888871, 'CAMARON 71', 'CONGELADO', @CategoriaCongelado, '8888-71', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-91')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-91', 888891, 'CAMARON CHICO 91-110', 'CONGELADO', @CategoriaCongelado, '8888-91', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-91-COCIDO')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-91-COCIDO', 8888910, 'CAMARON COCIDO 91', 'CONGELADO', @CategoriaCongelado, '8888-91-COCIDO', '50121903', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-MACHACA CUARTO')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-MC', 88881, 'MACHACA CAMARON .250 KG', 'CONGELADO', @CategoriaCongelado, '8888-MACHACA CUARTO', '50121612', 'H87', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-MACHACA MEDIO')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-MM', 88882, 'MACHACA CAMARON .500 KG', 'CONGELADO', @CategoriaCongelado, '8888-MACHACA MEDIO', '50121612', 'H87', @IVA_0, 1);

-- PATE DE CAMARON: Este SÍ lleva IVA 16% porque es un producto procesado/industrializado
IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888-PATE CAMARON')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888-PATE', 88883, 'PATE DE CAMARON', 'ABARROTES', @CategoriaAbarrotes, '8888-PATE CAMARON', '50193108', 'H87', @IVA_16, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '88883040CABEZA')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('88883040', 88883040, 'CAMARON C/CABEZA 30-40', 'CONGELADO', @CategoriaCongelado, '88883040CABEZA', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '88884050CAB')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('88884050', 88884050, 'CAMARON C/CABEZA 40-50', 'CONGELADO', @CategoriaCongelado, '88884050CAB', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8888R')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8888R', 8888, 'CAMARON R', 'CONGELADO', @CategoriaCongelado, '8888R', '50121612', 'KGM', @IVA_0, 1);

IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE CodigoInterno = '8889')
    INSERT INTO PRODUCTO (Codigo, ValorCodigo, Nombre, Descripcion, IdCategoria, CodigoInterno, ClaveProdServSATID, ClaveUnidadSAT, TasaIVAID, Activo)
    VALUES ('8889', 8889, 'CAMARON 21-25', 'CONGELADO', @CategoriaCongelado, '8889', '50121612', 'KGM', @IVA_0, 1);

PRINT '✅ Productos de camarón importados con IVA 0% (mariscos frescos/congelados)';
PRINT '⚠️ PATE DE CAMARON con IVA 16% (producto procesado)';
PRINT '';

-- Mostrar resumen
SELECT 
    COUNT(*) AS TotalCamarones,
    SUM(CASE WHEN t.Porcentaje = 0.00 THEN 1 ELSE 0 END) AS ConIVA_0,
    SUM(CASE WHEN t.Porcentaje = 16.00 THEN 1 ELSE 0 END) AS ConIVA_16
FROM PRODUCTO p
INNER JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID
WHERE p.Activo = 1
  AND (p.Nombre LIKE '%CAMARON%' OR p.CodigoInterno LIKE '8888%' OR p.CodigoInterno LIKE '8889%' OR p.CodigoInterno LIKE '8810%');

PRINT '';
PRINT '=== IMPORTACIÓN COMPLETADA ===';
GO
