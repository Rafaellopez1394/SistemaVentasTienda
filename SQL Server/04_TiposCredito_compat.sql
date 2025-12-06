-- Script de compatibilidad: crea una vista `TiposCredito` sobre `CatTiposCredito`
-- y si faltan filas, inserta los 3 tipos de crédito maestros.

SET NOCOUNT ON;

-- Crear vista que expone los nombres de columna que espera la aplicación
IF OBJECT_ID('dbo.TiposCredito','V') IS NULL
BEGIN
    EXEC('CREATE VIEW dbo.TiposCredito AS
    SELECT 
        TipoCreditoID,
        Codigo,
        Nombre,
        Descripcion,
        Criterio,
        NULL AS Icono,
        Estatus AS Activo,
        NULL AS Usuario,
        NULL AS FechaCreacion,
        NULL AS UltimaAct
    FROM dbo.CatTiposCredito;');
    PRINT 'Vista dbo.TiposCredito creada (map a CatTiposCredito)';
END
ELSE
    PRINT 'Vista dbo.TiposCredito ya existe';

-- Sembrar tipos maestros si no existen
IF NOT EXISTS (SELECT 1 FROM dbo.CatTiposCredito WHERE Codigo = 'CR001')
BEGIN
    INSERT INTO dbo.CatTiposCredito (Codigo, Nombre, Criterio, Descripcion, Estatus)
    VALUES ('CR001','Crédito Dinero','Dinero','Permite crédito en dinero',1);
    PRINT 'Insertado CR001';
END
ELSE
    PRINT 'CR001 ya existe';

IF NOT EXISTS (SELECT 1 FROM dbo.CatTiposCredito WHERE Codigo = 'CR002')
BEGIN
    INSERT INTO dbo.CatTiposCredito (Codigo, Nombre, Criterio, Descripcion, Estatus)
    VALUES ('CR002','Crédito Producto','Producto','Permite crédito en producto',1);
    PRINT 'Insertado CR002';
END
ELSE
    PRINT 'CR002 ya existe';

IF NOT EXISTS (SELECT 1 FROM dbo.CatTiposCredito WHERE Codigo = 'CR003')
BEGIN
    INSERT INTO dbo.CatTiposCredito (Codigo, Nombre, Criterio, Descripcion, Estatus)
    VALUES ('CR003','Crédito Tiempo','Tiempo','Permite crédito por tiempo (plazo)',1);
    PRINT 'Insertado CR003';
END
ELSE
    PRINT 'CR003 ya existe';

-- Verificación rápida
SELECT TOP (10) * FROM dbo.TiposCredito;

SET NOCOUNT OFF;
