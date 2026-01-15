-- =====================================================
-- Script: Configuración de Prodigia PAC
-- Descripción: Tabla para almacenar credenciales de Prodigia
-- Fecha: 2026-01-13
-- =====================================================

USE DB_TIENDA
GO

-- Crear tabla si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionProdigia')
BEGIN
    CREATE TABLE ConfiguracionProdigia (
        ConfiguracionID INT PRIMARY KEY IDENTITY(1,1),
        Usuario NVARCHAR(100) NOT NULL,
        Password NVARCHAR(200) NOT NULL,
        Contrato NVARCHAR(100) NOT NULL,
        Ambiente NVARCHAR(20) NOT NULL DEFAULT 'TEST', -- TEST o PRODUCCION
        RfcEmisor NVARCHAR(13) NOT NULL,
        NombreEmisor NVARCHAR(300) NOT NULL,
        RegimenFiscal NVARCHAR(10) NOT NULL,
        CertificadoBase64 NVARCHAR(MAX) NULL, -- Opcional si se usa CERT_DEFAULT
        LlavePrivadaBase64 NVARCHAR(MAX) NULL, -- Opcional si se usa CERT_DEFAULT
        PasswordLlave NVARCHAR(100) NULL,
        CodigoPostal NVARCHAR(10) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    )
    
    PRINT '✅ Tabla ConfiguracionProdigia creada exitosamente'
END
ELSE
BEGIN
    PRINT 'ℹ️ La tabla ConfiguracionProdigia ya existe'
END
GO

-- Insertar configuración de prueba (datos placeholder - reemplazar con tus credenciales reales)
IF NOT EXISTS (SELECT * FROM ConfiguracionProdigia WHERE ConfiguracionID = 1)
BEGIN
    INSERT INTO ConfiguracionProdigia (
        Usuario,
        Password,
        Contrato,
        Ambiente,
        RfcEmisor,
        NombreEmisor,
        RegimenFiscal,
        CodigoPostal,
        Activo
    )
    VALUES (
        'usuario_webservice', -- Reemplazar con tu usuario de Prodigia
        'password_webservice', -- Reemplazar con tu contraseña
        'CONTRATO123', -- Reemplazar con tu código de contrato
        'TEST', -- TEST o PRODUCCION
        'AAA010101AAA', -- RFC de tu empresa
        'MI EMPRESA SA DE CV', -- Razón social
        '601', -- Régimen Fiscal: 601 = General
        '01000', -- Código postal de expedición
        1 -- Activo
    )
    
    PRINT '✅ Configuración de prueba insertada'
    PRINT '⚠️ IMPORTANTE: Actualiza los valores con tus credenciales reales de Prodigia'
    PRINT '    1. Registrate en https://facturacion.pade.mx/'
    PRINT '    2. Obtén tu Usuario, Contraseña y Contrato'
    PRINT '    3. Sube tu certificado CSD o actualiza CertificadoBase64/LlavePrivadaBase64'
END
ELSE
BEGIN
    PRINT 'ℹ️ Ya existe configuración de Prodigia'
END
GO

-- Verificar configuración
SELECT 
    ConfiguracionID,
    Usuario,
    '****' AS Password,
    Contrato,
    Ambiente,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CASE 
        WHEN CertificadoBase64 IS NULL THEN '❌ NULL (usar CERT_DEFAULT)'
        ELSE '✅ ' + CAST(LEN(CertificadoBase64) AS VARCHAR) + ' chars'
    END AS Certificado,
    CASE 
        WHEN LlavePrivadaBase64 IS NULL THEN '❌ NULL (usar CERT_DEFAULT)'
        ELSE '✅ ' + CAST(LEN(LlavePrivadaBase64) AS VARCHAR) + ' chars'
    END AS LlavePrivada,
    CodigoPostal,
    CASE WHEN Activo = 1 THEN '✅ Activo' ELSE '❌ Inactivo' END AS Estado,
    FechaCreacion
FROM ConfiguracionProdigia
WHERE ConfiguracionID = 1
GO

PRINT ''
PRINT '=============================================='
PRINT '  Configuración de Prodigia Completada'
PRINT '=============================================='
PRINT 'Siguiente paso: Actualizar credenciales reales'
PRINT 'UPDATE ConfiguracionProdigia SET Usuario = ''TuUsuario'', Password = ''TuPassword'', Contrato = ''TuContrato'' WHERE ConfiguracionID = 1'
PRINT ''
