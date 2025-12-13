-- 011_TABLAS_CONFIGURACION.sql
-- Crear tablas de configuracion del sistema

USE DB_TIENDA;
GO

-- ============================================================
-- TABLA: ConfiguracionGeneral
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'ConfiguracionGeneral')
BEGIN
    CREATE TABLE ConfiguracionGeneral (
        ConfigID INT IDENTITY(1,1) PRIMARY KEY,
        NombreNegocio VARCHAR(200) NOT NULL,
        RFC VARCHAR(20) NULL,
        Direccion VARCHAR(500) NULL,
        Telefono VARCHAR(50) NULL,
        Email VARCHAR(100) NULL,
        LogoPath VARCHAR(500) NULL,
        MensajeTicket VARCHAR(500) NULL,
        ImprimirTicketAutomatico BIT DEFAULT 0,
        MostrarLogoEnTicket BIT DEFAULT 0,
        Usuario VARCHAR(50) NOT NULL,
        FechaModificacion DATETIME DEFAULT GETDATE()
    );
    
    -- Insertar configuracion por defecto
    INSERT INTO ConfiguracionGeneral (NombreNegocio, RFC, Direccion, Telefono, MensajeTicket, ImprimirTicketAutomatico, Usuario)
    VALUES ('MI TIENDA', 'XAXX010101000', 'Direccion del negocio', '', 'Gracias por su compra', 0, 'system');
    
    PRINT 'Tabla ConfiguracionGeneral creada correctamente';
END
ELSE
BEGIN
    PRINT 'Tabla ConfiguracionGeneral ya existe';
END
GO

-- ============================================================
-- TABLA: ConfiguracionImpresoras
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'ConfiguracionImpresoras')
BEGIN
    CREATE TABLE ConfiguracionImpresoras (
        ConfigID INT IDENTITY(1,1) PRIMARY KEY,
        TipoImpresion VARCHAR(20) NOT NULL, -- TICKET, FACTURA, REPORTE
        NombreImpresora VARCHAR(200) NOT NULL,
        AnchoPapel INT DEFAULT 80, -- 58mm o 80mm para tickets
        Activo BIT DEFAULT 1,
        Usuario VARCHAR(50) NOT NULL,
        FechaModificacion DATETIME DEFAULT GETDATE()
    );
    
    PRINT 'Tabla ConfiguracionImpresoras creada correctamente';
END
ELSE
BEGIN
    PRINT 'Tabla ConfiguracionImpresoras ya existe';
END
GO

-- ============================================================
-- TABLA: Usuarios (si no existe, agregar campos necesarios)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'Activo')
BEGIN
    ALTER TABLE Usuarios ADD Activo BIT DEFAULT 1;
    PRINT 'Campo Activo agregado a Usuarios';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'FechaCreacion')
BEGIN
    ALTER TABLE Usuarios ADD FechaCreacion DATETIME DEFAULT GETDATE();
    PRINT 'Campo FechaCreacion agregado a Usuarios';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'UltimoAcceso')
BEGIN
    ALTER TABLE Usuarios ADD UltimoAcceso DATETIME NULL;
    PRINT 'Campo UltimoAcceso agregado a Usuarios';
END
GO

PRINT 'Script de configuracion ejecutado correctamente';
GO
