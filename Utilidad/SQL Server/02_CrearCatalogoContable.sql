-- Script SQL para crear tabla CatalogoContable
-- Ejecutar en la base de datos DB_TIENDA

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatalogoContable]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CatalogoContable] (
        CuentaID INT PRIMARY KEY IDENTITY(1,1),
        CodigoCuenta VARCHAR(20) NOT NULL UNIQUE,      -- Ej: "1100", "4000", "2052"
        NombreCuenta VARCHAR(100) NOT NULL,             -- Ej: "Clientes", "Ventas", "IVA Cobrado"
        TipoCuenta VARCHAR(20) NOT NULL,                -- ACTIVO, PASIVO, PATRIMONIO, INGRESO, GASTO
        SubTipo VARCHAR(50),                            -- CLIENTE, CAJA, INVENTARIO, IVA_COBRADO, IVA_PAGADO, VENTAS, COSTO_VENTAS, etc.
        Descripcion VARCHAR(255),
        Activo BIT NOT NULL DEFAULT 1,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT CK_TipoCuenta CHECK (TipoCuenta IN ('ACTIVO', 'PASIVO', 'PATRIMONIO', 'INGRESO', 'GASTO'))
    );
    
    -- Índices
    CREATE INDEX IX_CatalogoContable_SubTipo ON [dbo].[CatalogoContable] (SubTipo, Activo);
    CREATE INDEX IX_CatalogoContable_Codigo ON [dbo].[CatalogoContable] (CodigoCuenta, Activo);
    
    PRINT 'Tabla CatalogoContable creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla CatalogoContable ya existe.';
END

-- Datos iniciales de ejemplo (AJUSTAR según tu catálogo contable real)
IF NOT EXISTS (SELECT * FROM [dbo].[CatalogoContable] WHERE CodigoCuenta = '1010')
BEGIN
    INSERT INTO [dbo].[CatalogoContable] (CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion)
    VALUES 
    ('1010', 'Caja', 'ACTIVO', 'CAJA', 'Caja General'),
    ('1100', 'Clientes', 'ACTIVO', 'CLIENTE', 'Clientes - Cuentas por Cobrar'),
    ('1400', 'Inventario', 'ACTIVO', 'INVENTARIO', 'Inventario de Productos'),
    ('1500', 'IVA Pagado 0%', 'ACTIVO', 'IVA_PAGADO_0', 'IVA Pagado - Tasa 0%'),
    ('1501', 'IVA Pagado 8%', 'ACTIVO', 'IVA_PAGADO_8', 'IVA Pagado - Tasa 8%'),
    ('1502', 'IVA Pagado 16%', 'ACTIVO', 'IVA_PAGADO_16', 'IVA Pagado - Tasa 16%'),
    ('1503', 'IVA Pagado Exento', 'ACTIVO', 'IVA_PAGADO_EXENTO', 'IVA Pagado - Exento'),
    ('2050', 'IVA Cobrado 0%', 'PASIVO', 'IVA_COBRADO_0', 'IVA Cobrado - Tasa 0%'),
    ('2051', 'IVA Cobrado 8%', 'PASIVO', 'IVA_COBRADO_8', 'IVA Cobrado - Tasa 8%'),
    ('2052', 'IVA Cobrado 16%', 'PASIVO', 'IVA_COBRADO_16', 'IVA Cobrado - Tasa 16%'),
    ('2053', 'IVA Cobrado Exento', 'PASIVO', 'IVA_COBRADO_EXENTO', 'IVA Cobrado - Exento'),
    ('2100', 'Proveedores', 'PASIVO', 'PROVEEDOR', 'Proveedores - Cuentas por Pagar'),
    ('4000', 'Ventas', 'INGRESO', 'VENTAS', 'Ingresos por Ventas de Productos'),
    ('5000', 'Costo de Ventas', 'GASTO', 'COSTO_VENTAS', 'Costo de Mercancía Vendida');
END

PRINT 'Datos iniciales de CatalogoContable insertados.';
