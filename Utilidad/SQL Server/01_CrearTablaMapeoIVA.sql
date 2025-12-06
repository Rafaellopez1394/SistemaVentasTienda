-- Script SQL para crear tabla MapeoContableIVA
-- Ejecutar en la base de datos DB_TIENDA

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapeoContableIVA]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MapeoContableIVA] (
        MapeoIVAID INT PRIMARY KEY IDENTITY(1,1),
        TasaIVA DECIMAL(5,2) NOT NULL,          -- 0.00, 8.00, 16.00, etc.
        Exento BIT NOT NULL DEFAULT 0,          -- 1 si es exento
        CuentaDeudora INT NOT NULL,             -- ID de cuenta contable para débitos
        CuentaAcreedora INT NOT NULL,           -- ID de cuenta contable para créditos
        Descripcion VARCHAR(100),               -- Ej: "IVA 16%", "Exento", "IVA 8%"
        Activo BIT NOT NULL DEFAULT 1,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        UNIQUE (TasaIVA, Exento)
    );
    
    -- Índice para búsquedas rápidas
    CREATE INDEX IX_MapeoContableIVA_Tasa ON [dbo].[MapeoContableIVA] (TasaIVA, Exento, Activo);
    
    PRINT 'Tabla MapeoContableIVA creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla MapeoContableIVA ya existe.';
END

-- Datos iniciales de ejemplo
-- Ajusta los IDs de cuenta (CuentaDeudora, CuentaAcreedora) según tu catálogo contable
IF NOT EXISTS (SELECT * FROM [dbo].[MapeoContableIVA] WHERE TasaIVA = 0.00 AND Exento = 0)
BEGIN
    INSERT INTO [dbo].[MapeoContableIVA] (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
    VALUES (0.00, 0, 2050, 2050, 'IVA 0%');
END

IF NOT EXISTS (SELECT * FROM [dbo].[MapeoContableIVA] WHERE TasaIVA = 8.00 AND Exento = 0)
BEGIN
    INSERT INTO [dbo].[MapeoContableIVA] (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
    VALUES (8.00, 0, 2051, 2051, 'IVA 8%');
END

IF NOT EXISTS (SELECT * FROM [dbo].[MapeoContableIVA] WHERE TasaIVA = 16.00 AND Exento = 0)
BEGIN
    INSERT INTO [dbo].[MapeoContableIVA] (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
    VALUES (16.00, 0, 2052, 2052, 'IVA 16%');
END

IF NOT EXISTS (SELECT * FROM [dbo].[MapeoContableIVA] WHERE Exento = 1)
BEGIN
    INSERT INTO [dbo].[MapeoContableIVA] (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
    VALUES (0.00, 1, 2053, 2053, 'Exento de IVA');
END

PRINT 'Datos iniciales de MapeoContableIVA insertados.';
