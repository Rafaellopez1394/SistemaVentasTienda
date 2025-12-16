-- Script SQL para agregar cuentas contables de Mermas y Ajustes de Inventario
-- Ejecutar en la base de datos DB_TIENDA

-- Verificar si la tabla CatalogoContable existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatalogoContable]') AND type in (N'U'))
BEGIN
    PRINT 'ERROR: La tabla CatalogoContable no existe. Ejecutar primero 02_CrearCatalogoContable.sql';
    RETURN;
END

PRINT 'Agregando cuentas contables para Mermas y Ajustes de Inventario...';

-- 5302 - Pérdida por Merma (GASTO)
IF NOT EXISTS (SELECT * FROM [dbo].[CatalogoContable] WHERE CodigoCuenta = '5302')
BEGIN
    INSERT INTO [dbo].[CatalogoContable] (CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion)
    VALUES ('5302', 'Pérdida por Merma', 'GASTO', 'MERMA', 'Pérdida de Inventario por Merma, Caducidad o Deterioro');
    PRINT '  ✓ Cuenta 5302 - Pérdida por Merma creada';
END
ELSE
    PRINT '  ○ Cuenta 5302 ya existe';

-- 5303 - Ajuste de Inventario (GASTO)
-- Se usa cuando hay ajuste negativo (faltante de inventario)
IF NOT EXISTS (SELECT * FROM [dbo].[CatalogoContable] WHERE CodigoCuenta = '5303')
BEGIN
    INSERT INTO [dbo].[CatalogoContable] (CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion)
    VALUES ('5303', 'Ajuste de Inventario - Faltante', 'GASTO', 'AJUSTE_INVENTARIO', 'Ajuste Negativo de Inventario (Faltante)');
    PRINT '  ✓ Cuenta 5303 - Ajuste de Inventario Faltante creada';
END
ELSE
    PRINT '  ○ Cuenta 5303 ya existe';

-- 4100 - Otros Productos (INGRESO)
-- Se usa cuando hay ajuste positivo (sobrante de inventario)
IF NOT EXISTS (SELECT * FROM [dbo].[CatalogoContable] WHERE CodigoCuenta = '4100')
BEGIN
    INSERT INTO [dbo].[CatalogoContable] (CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion)
    VALUES ('4100', 'Otros Productos', 'INGRESO', 'AJUSTE_INVENTARIO', 'Ajuste Positivo de Inventario (Sobrante)');
    PRINT '  ✓ Cuenta 4100 - Otros Productos creada';
END
ELSE
    PRINT '  ○ Cuenta 4100 ya existe';

PRINT '';
PRINT 'Script completado. Resumen de cuentas para Inventario:';
PRINT '-----------------------------------------------------';
PRINT '1400 - Inventario (ACTIVO)          → Cuenta de inventario';
PRINT '5302 - Pérdida por Merma (GASTO)    → DEBE en mermas';
PRINT '5303 - Ajuste Faltante (GASTO)      → DEBE en ajuste negativo';
PRINT '4100 - Otros Productos (INGRESO)    → HABER en ajuste positivo';
PRINT '';
PRINT 'Lógica de Pólizas:';
PRINT '==================';
PRINT 'MERMA (disminuye inventario):';
PRINT '  DEBE  5302 Pérdida por Merma     $XXX';
PRINT '  HABER 1400 Inventario            $XXX';
PRINT '';
PRINT 'AJUSTE NEGATIVO - Faltante (disminuye inventario):';
PRINT '  DEBE  5303 Ajuste Faltante       $XXX';
PRINT '  HABER 1400 Inventario            $XXX';
PRINT '';
PRINT 'AJUSTE POSITIVO - Sobrante (aumenta inventario):';
PRINT '  DEBE  1400 Inventario            $XXX';
PRINT '  HABER 4100 Otros Productos       $XXX';

