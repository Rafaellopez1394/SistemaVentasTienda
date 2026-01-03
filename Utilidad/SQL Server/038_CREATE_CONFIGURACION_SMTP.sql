-- =============================================
-- Script: Crear tabla ConfiguracionSMTP
-- Descripción: Configuración de servidor SMTP para envío de emails
-- Fecha: 2026-01-02
-- =============================================

USE DB_TIENDA;
GO

-- Crear tabla si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionSMTP')
BEGIN
    CREATE TABLE ConfiguracionSMTP (
        ConfigID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Servidor SMTP
        Host VARCHAR(200) NOT NULL DEFAULT 'smtp.gmail.com',
        Puerto INT NOT NULL DEFAULT 587,
        UsarSSL BIT NOT NULL DEFAULT 1,
        
        -- Credenciales
        Usuario VARCHAR(200) NOT NULL,
        Contrasena VARCHAR(500) NOT NULL, -- Encriptada en producción
        
        -- Remitente
        EmailRemitente VARCHAR(200) NOT NULL,
        NombreRemitente VARCHAR(200) NOT NULL DEFAULT 'Sistema de Facturación',
        
        -- Estado
        Activo BIT NOT NULL DEFAULT 1,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100),
        FechaModificacion DATETIME
    );

    PRINT 'Tabla ConfiguracionSMTP creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla ConfiguracionSMTP ya existe';
END
GO

-- Insertar configuración por defecto (usuario debe actualizarla)
IF NOT EXISTS (SELECT * FROM ConfiguracionSMTP)
BEGIN
    INSERT INTO ConfiguracionSMTP 
    (Host, Puerto, UsarSSL, Usuario, Contrasena, EmailRemitente, NombreRemitente, Activo, UsuarioCreacion)
    VALUES 
    ('smtp.gmail.com', 587, 1, 'TU_EMAIL@gmail.com', 'TU_CONTRASEÑA_APLICACION', 'TU_EMAIL@gmail.com', 'Sistema de Facturación', 0, 'SISTEMA');
    
    PRINT 'Configuración SMTP por defecto insertada (INACTIVA - Debe configurarse)';
END
GO

-- Verificar
SELECT * FROM ConfiguracionSMTP;
GO
