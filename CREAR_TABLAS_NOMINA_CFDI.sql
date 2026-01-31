-- ============================================================
-- CREAR TABLAS PARA TIMBRADO DE NÓMINA
-- ============================================================

USE DB_TIENDA
GO

PRINT '=========================================='
PRINT 'CREANDO TABLAS PARA TIMBRADO DE NÓMINA'
PRINT '=========================================='
PRINT ''

-- ============================================================
-- 1. TABLA NominasCFDI
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NominasCFDI')
BEGIN
    PRINT '1. Creando tabla NominasCFDI...'
    
    CREATE TABLE NominasCFDI (
        NominaCFDIID INT IDENTITY(1,1) PRIMARY KEY,
        NominaDetalleID INT NOT NULL,
        UUID VARCHAR(50) NULL,
        FechaTimbrado DATETIME NULL,
        XMLTimbrado NVARCHAR(MAX) NULL,
        SelloCFD NVARCHAR(MAX) NULL,
        SelloSAT NVARCHAR(MAX) NULL,
        InvoiceId VARCHAR(100) NULL,
        NoCertificadoSAT VARCHAR(50) NULL,
        CadenaOriginal NVARCHAR(MAX) NULL,
        EstadoTimbrado VARCHAR(20) NULL, -- 'PENDIENTE', 'EXITOSO', 'ERROR'
        CodigoError VARCHAR(50) NULL,
        MensajeError NVARCHAR(500) NULL,
        UsuarioTimbrado VARCHAR(100) NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UltimaActualizacion DATETIME NULL
    )
    
    PRINT '   ✓ Tabla NominasCFDI creada'
END
ELSE
BEGIN
    PRINT '1. Tabla NominasCFDI ya existe'
    
    -- Verificar y agregar columna InvoiceId si no existe
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominasCFDI' AND COLUMN_NAME = 'InvoiceId'
    )
    BEGIN
        ALTER TABLE NominasCFDI ADD InvoiceId VARCHAR(100) NULL
        PRINT '   ✓ Columna InvoiceId agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna InvoiceId ya existe'
    END
END
PRINT ''

-- ============================================================
-- 2. ACTUALIZAR TABLA NominaDetalle (agregar columnas de timbrado)
-- ============================================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NominaDetalle')
BEGIN
    PRINT '2. Actualizando tabla NominaDetalle...'
    
    -- Columna UUID
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'UUID'
    )
    BEGIN
        ALTER TABLE NominaDetalle ADD UUID VARCHAR(50) NULL
        PRINT '   ✓ Columna UUID agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna UUID ya existe'
    END
    
    -- Columna FechaTimbrado
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'FechaTimbrado'
    )
    BEGIN
        ALTER TABLE NominaDetalle ADD FechaTimbrado DATETIME NULL
        PRINT '   ✓ Columna FechaTimbrado agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna FechaTimbrado ya existe'
    END
    
    -- Columna SelloCFD
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'SelloCFD'
    )
    BEGIN
        ALTER TABLE NominaDetalle ADD SelloCFD NVARCHAR(MAX) NULL
        PRINT '   ✓ Columna SelloCFD agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna SelloCFD ya existe'
    END
    
    -- Columna SelloSAT
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'SelloSAT'
    )
    BEGIN
        ALTER TABLE NominaDetalle ADD SelloSAT NVARCHAR(MAX) NULL
        PRINT '   ✓ Columna SelloSAT agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna SelloSAT ya existe'
    END
    
    -- Columna EstatusTimbre
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'EstatusTimbre'
    )
    BEGIN
        ALTER TABLE NominaDetalle ADD EstatusTimbre VARCHAR(20) NULL
        PRINT '   ✓ Columna EstatusTimbre agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna EstatusTimbre ya existe'
    END
END
ELSE
BEGIN
    PRINT '2. ⚠️ Tabla NominaDetalle NO existe'
END
PRINT ''

-- ============================================================
-- 3. ACTUALIZAR TABLA Empleados (agregar columnas SAT)
-- ============================================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empleados')
BEGIN
    PRINT '3. Actualizando tabla Empleados...'
    
    -- Columna TipoRegimen
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'Empleados' AND COLUMN_NAME = 'TipoRegimen'
    )
    BEGIN
        ALTER TABLE Empleados ADD TipoRegimen VARCHAR(10) NULL
        PRINT '   ✓ Columna TipoRegimen agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna TipoRegimen ya existe'
    END
    
    -- Columna CodigoBanco
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'Empleados' AND COLUMN_NAME = 'CodigoBanco'
    )
    BEGIN
        ALTER TABLE Empleados ADD CodigoBanco VARCHAR(10) NULL
        PRINT '   ✓ Columna CodigoBanco agregada'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Columna CodigoBanco ya existe'
    END
END
ELSE
BEGIN
    PRINT '3. ⚠️ Tabla Empleados NO existe'
END
PRINT ''

PRINT '=========================================='
PRINT 'TABLAS CREADAS/ACTUALIZADAS'
PRINT '=========================================='
GO
