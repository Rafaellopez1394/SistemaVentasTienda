-- =============================================
-- Script: 017_CREAR_CERTIFICADOS_CONFIG.sql
-- Descripción: Tabla para almacenar configuración de certificados digitales (CSD)
-- Autor: Sistema
-- Fecha: 2025-12-14
-- =============================================

USE DBVENTAS
GO

-- Tabla para configuración de certificados (CSD)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionCertificados')
BEGIN
    CREATE TABLE ConfiguracionCertificados
    (
        ConfiguracionID INT IDENTITY(1,1) PRIMARY KEY,
        EmpresaID INT NULL, -- Si es NULL, es la configuración por defecto
        RFC VARCHAR(13) NOT NULL,
        RazonSocial VARCHAR(250) NOT NULL,
        
        -- Rutas de archivos de certificado
        RutaCER VARCHAR(500) NOT NULL, -- Ruta al archivo .CER
        RutaKEY VARCHAR(500) NOT NULL, -- Ruta al archivo .KEY
        PasswordKEY VARCHAR(100) NOT NULL, -- Contraseña del archivo .KEY (encriptada)
        
        -- Información del certificado
        NumeroCertificado VARCHAR(20) NULL, -- Se llena automáticamente al cargar el certificado
        ValidoDesde DATETIME NULL,
        ValidoHasta DATETIME NULL,
        
        -- Tipo de certificado
        TipoCertificado VARCHAR(20) NOT NULL DEFAULT 'CSD', -- CSD o FIEL
        Activo BIT NOT NULL DEFAULT 1,
        
        -- Auditoría
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioRegistro VARCHAR(50) NULL,
        FechaModificacion DATETIME NULL,
        UsuarioModificacion VARCHAR(50) NULL,
        
        CONSTRAINT CK_TipoCertificado CHECK (TipoCertificado IN ('CSD', 'FIEL'))
    );

    PRINT 'Tabla ConfiguracionCertificados creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla ConfiguracionCertificados ya existe';
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ConfiguracionCertificados_RFC')
BEGIN
    CREATE INDEX IX_ConfiguracionCertificados_RFC ON ConfiguracionCertificados(RFC);
    PRINT 'Índice IX_ConfiguracionCertificados_RFC creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ConfiguracionCertificados_Activo')
BEGIN
    CREATE INDEX IX_ConfiguracionCertificados_Activo ON ConfiguracionCertificados(Activo);
    PRINT 'Índice IX_ConfiguracionCertificados_Activo creado';
END
GO

-- Tabla para registro de cancelaciones de CFDI
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CFDICancelaciones')
BEGIN
    CREATE TABLE CFDICancelaciones
    (
        CancelacionID INT IDENTITY(1,1) PRIMARY KEY,
        FacturaID INT NOT NULL,
        UUID UNIQUEIDENTIFIER NOT NULL,
        
        -- Motivo de cancelación según SAT
        MotivoCancelacion VARCHAR(2) NOT NULL, -- 01, 02, 03, 04
        DescripcionMotivo VARCHAR(200) NOT NULL,
        
        -- UUID del comprobante que sustituye (solo para motivo 01)
        UUIDSustitucion UNIQUEIDENTIFIER NULL,
        
        -- Información de la cancelación
        FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE(),
        FechaCancelacion DATETIME NULL,
        EstadoCancelacion VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
        
        -- Respuesta del PAC
        XMLCancelacion NVARCHAR(MAX) NULL,
        CodigoRespuesta VARCHAR(10) NULL,
        MensajeRespuesta VARCHAR(500) NULL,
        AcuseCancelacion NVARCHAR(MAX) NULL,
        
        -- Auditoría
        UsuarioSolicita VARCHAR(50) NOT NULL,
        
        CONSTRAINT FK_CFDICancelaciones_Factura FOREIGN KEY (FacturaID) 
            REFERENCES Factura(IdFactura),
        
        CONSTRAINT CK_MotivoCancelacion CHECK (MotivoCancelacion IN ('01', '02', '03', '04')),
        CONSTRAINT CK_EstadoCancelacion CHECK (EstadoCancelacion IN ('PENDIENTE', 'ACEPTADA', 'RECHAZADA', 'ERROR'))
    );

    PRINT 'Tabla CFDICancelaciones creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla CFDICancelaciones ya existe';
END
GO

-- Índices para CFDICancelaciones
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CFDICancelaciones_UUID')
BEGIN
    CREATE INDEX IX_CFDICancelaciones_UUID ON CFDICancelaciones(UUID);
    PRINT 'Índice IX_CFDICancelaciones_UUID creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CFDICancelaciones_Estado')
BEGIN
    CREATE INDEX IX_CFDICancelaciones_Estado ON CFDICancelaciones(EstadoCancelacion);
    PRINT 'Índice IX_CFDICancelaciones_Estado creado';
END
GO

-- Agregar campo de cancelación a la tabla Factura si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'FechaCancelacion')
BEGIN
    ALTER TABLE Factura ADD FechaCancelacion DATETIME NULL;
    PRINT 'Campo FechaCancelacion agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'EsCancelada')
BEGIN
    ALTER TABLE Factura ADD EsCancelada BIT NOT NULL DEFAULT 0;
    PRINT 'Campo EsCancelada agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'MotivoCancelacion')
BEGIN
    ALTER TABLE Factura ADD MotivoCancelacion VARCHAR(2) NULL;
    PRINT 'Campo MotivoCancelacion agregado a tabla Factura';
END
GO

-- Datos de ejemplo de motivos de cancelación (catálogo del SAT)
PRINT '';
PRINT '=== CATÁLOGO DE MOTIVOS DE CANCELACIÓN ===';
PRINT '01 - Comprobante emitido con errores con relación (requiere UUID de sustitución)';
PRINT '02 - Comprobante emitido con errores sin relación';
PRINT '03 - No se llevó a cabo la operación';
PRINT '04 - Operación nominativa relacionada en la factura global';
PRINT '';

PRINT 'Script 017 ejecutado correctamente';
GO
