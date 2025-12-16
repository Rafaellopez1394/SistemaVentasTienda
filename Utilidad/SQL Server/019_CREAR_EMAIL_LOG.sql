-- =============================================
-- Script: 019_CREAR_EMAIL_LOG.sql
-- Descripción: Tabla para registro de envíos de email
-- Autor: Sistema
-- Fecha: 2025-12-14
-- =============================================

USE DBVENTAS
GO

-- Tabla de log de envíos de email
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailLog')
BEGIN
    CREATE TABLE EmailLog
    (
        EmailLogID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Tipo de documento
        TipoDocumento VARCHAR(20) NOT NULL, -- FACTURA, COMPLEMENTO_PAGO, NOMINA
        DocumentoID INT NOT NULL,
        UUID UNIQUEIDENTIFIER NULL,
        
        -- Destinatario
        EmailDestinatario VARCHAR(254) NOT NULL,
        NombreDestinatario VARCHAR(254) NULL,
        
        -- Contenido del email
        Asunto VARCHAR(500) NOT NULL,
        CuerpoHTML NVARCHAR(MAX) NULL,
        
        -- Adjuntos enviados
        AdjuntoPDF BIT NOT NULL DEFAULT 0,
        AdjuntoXML BIT NOT NULL DEFAULT 0,
        NombreArchivoPDF VARCHAR(255) NULL,
        NombreArchivoXML VARCHAR(255) NULL,
        
        -- Estado del envío
        FechaEnvio DATETIME NOT NULL DEFAULT GETDATE(),
        Exitoso BIT NOT NULL,
        MensajeError VARCHAR(1000) NULL,
        
        -- Datos SMTP utilizados
        ServidorSMTP VARCHAR(100) NULL,
        
        -- Auditoría
        UsuarioEnvio VARCHAR(50) NULL,
        
        CONSTRAINT CK_TipoDocumento_Email CHECK (TipoDocumento IN ('FACTURA', 'COMPLEMENTO_PAGO', 'NOMINA'))
    );

    PRINT 'Tabla EmailLog creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla EmailLog ya existe';
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_TipoDoc')
BEGIN
    CREATE INDEX IX_EmailLog_TipoDoc ON EmailLog(TipoDocumento, DocumentoID);
    PRINT 'Índice IX_EmailLog_TipoDoc creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_Email')
BEGIN
    CREATE INDEX IX_EmailLog_Email ON EmailLog(EmailDestinatario);
    PRINT 'Índice IX_EmailLog_Email creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_Fecha')
BEGIN
    CREATE INDEX IX_EmailLog_Fecha ON EmailLog(FechaEnvio DESC);
    PRINT 'Índice IX_EmailLog_Fecha creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailLog_UUID')
BEGIN
    CREATE INDEX IX_EmailLog_UUID ON EmailLog(UUID) WHERE UUID IS NOT NULL;
    PRINT 'Índice IX_EmailLog_UUID creado';
END
GO

-- Agregar campo de email a tablas si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'EmailCliente')
BEGIN
    ALTER TABLE Factura ADD EmailCliente VARCHAR(254) NULL;
    PRINT 'Campo EmailCliente agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'EnviarEmailAutomatico')
BEGIN
    ALTER TABLE Factura ADD EnviarEmailAutomatico BIT NOT NULL DEFAULT 0;
    PRINT 'Campo EnviarEmailAutomatico agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Cliente') AND name = 'EmailFacturacion')
BEGIN
    ALTER TABLE Cliente ADD EmailFacturacion VARCHAR(254) NULL;
    PRINT 'Campo EmailFacturacion agregado a tabla Cliente';
END
GO

PRINT '';
PRINT '=== CONFIGURACIÓN SMTP REQUERIDA ===';
PRINT 'Agregar en Web.config dentro de <appSettings>:';
PRINT '  <add key="SMTP_Host" value="smtp.gmail.com" />';
PRINT '  <add key="SMTP_Port" value="587" />';
PRINT '  <add key="SMTP_Username" value="tu_email@gmail.com" />';
PRINT '  <add key="SMTP_Password" value="tu_contraseña_app" />';
PRINT '  <add key="SMTP_SSL" value="true" />';
PRINT '  <add key="SMTP_FromEmail" value="tu_email@gmail.com" />';
PRINT '  <add key="SMTP_FromName" value="Tu Empresa" />';
PRINT '';
PRINT 'Para Gmail: Usar "Contraseña de aplicación" en vez de contraseña normal';
PRINT 'https://support.google.com/accounts/answer/185833';
PRINT '';

PRINT 'Script 019 ejecutado correctamente';
GO
