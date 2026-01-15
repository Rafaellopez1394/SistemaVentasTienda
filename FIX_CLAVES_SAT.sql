-- Script para corregir claves SAT en productos
-- Asegura que ClaveProdServSAT tenga 8 dígitos

-- Ver productos con claves incorrectas
SELECT ProductoID, Nombre, ClaveProdServSAT, ClaveUnidadSAT,
       CASE 
           WHEN ClaveProdServSAT IS NULL THEN 'NULL'
           WHEN LEN(ClaveProdServSAT) <> 8 THEN 'Longitud incorrecta: ' + CAST(LEN(ClaveProdServSAT) AS VARCHAR)
           ELSE 'OK'
       END AS Validacion
FROM Productos
WHERE ClaveProdServSAT IS NULL OR LEN(ClaveProdServSAT) <> 8;

-- Corregir productos sin clave o con clave incorrecta
-- 01010101 = No existe en el catálogo (valor genérico del SAT)
UPDATE Productos
SET ClaveProdServSAT = '01010101'
WHERE ClaveProdServSAT IS NULL OR LEN(ClaveProdServSAT) <> 8;

-- Corregir ClaveUnidadSAT si está vacía
-- H87 = Pieza
UPDATE Productos
SET ClaveUnidadSAT = 'H87'
WHERE ClaveUnidadSAT IS NULL OR ClaveUnidadSAT = '';

-- Verificar correcciones
SELECT COUNT(*) AS TotalProductos,
       SUM(CASE WHEN LEN(ClaveProdServSAT) = 8 THEN 1 ELSE 0 END) AS ClavesCorrectas,
       SUM(CASE WHEN LEN(ClaveProdServSAT) <> 8 THEN 1 ELSE 0 END) AS ClavesIncorrectas
FROM Productos;

PRINT '✅ Claves SAT corregidas en tabla Productos';
