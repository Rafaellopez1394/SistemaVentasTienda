-- =====================================================================
-- Script: Configuración de FiscalAPI para Timbrado CFDI 4.0
-- Descripción: Crea tabla y datos iniciales para integración directa
-- Compatible con: SQL Server 2012+
-- =====================================================================

USE DB_TIENDA;
GO

-- =====================================================================
-- 1. CREAR TABLA DE CONFIGURACIÓN
-- =====================================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionFiscalAPI]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfiguracionFiscalAPI](
        [ConfiguracionID] INT IDENTITY(1,1) NOT NULL,
        [ApiKey] NVARCHAR(500) NOT NULL,
        [Ambiente] NVARCHAR(20) NOT NULL DEFAULT 'TEST', -- TEST o PRODUCCION
        [RfcEmisor] NVARCHAR(13) NOT NULL,
        [NombreEmisor] NVARCHAR(300) NOT NULL,
        [RegimenFiscal] NVARCHAR(3) NOT NULL, -- 601, 603, 612, etc.
        [CertificadoBase64] NVARCHAR(MAX) NULL, -- Archivo .cer en Base64
        [LlavePrivadaBase64] NVARCHAR(MAX) NULL, -- Archivo .key en Base64
        [PasswordLlave] NVARCHAR(100) NULL, -- Contraseña de la llave
        [CodigoPostal] NVARCHAR(5) NOT NULL, -- CP del emisor
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_ConfiguracionFiscalAPI] PRIMARY KEY CLUSTERED ([ConfiguracionID] ASC)
    );
    
    PRINT '✅ Tabla ConfiguracionFiscalAPI creada correctamente';
END
ELSE
BEGIN
    PRINT '⚠️ La tabla ConfiguracionFiscalAPI ya existe';
END
GO

-- =====================================================================
-- 2. INSERTAR CONFIGURACIÓN INICIAL (AMBIENTE TEST)
-- =====================================================================

-- IMPORTANTE: Reemplazar con tus datos reales

IF NOT EXISTS (SELECT 1 FROM ConfiguracionFiscalAPI WHERE Activo = 1)
BEGIN
    INSERT INTO [dbo].[ConfiguracionFiscalAPI]
    (
        [ApiKey],
        [Ambiente],
        [RfcEmisor],
        [NombreEmisor],
        [RegimenFiscal],
        [CodigoPostal],
        [CertificadoBase64],
        [LlavePrivadaBase64],
        [PasswordLlave],
        [Activo]
    )
    VALUES
    (
        'TU_API_KEY_DE_FISCALAPI_AQUI', -- ⚠️ REEMPLAZAR con tu API Key
        'TEST', -- TEST = https://test.fiscalapi.com, PRODUCCION = https://api.fiscalapi.com
        'XAXX010101000', -- ⚠️ REEMPLAZAR con RFC de tu empresa
        'EMPRESA DE PRUEBA SA DE CV', -- ⚠️ REEMPLAZAR con tu razón social
        '601', -- 601 = General de Ley Personas Morales (ajustar según corresponda)
        '06000', -- ⚠️ REEMPLAZAR con tu código postal
        NULL, -- Se llena después con CertificadoHelper.CertificadoABase64()
        NULL, -- Se llena después con CertificadoHelper.LlavePrivadaABase64()
        NULL, -- Contraseña de la llave privada (encriptar en producción)
        1 -- Activo
    );
    
    PRINT '✅ Configuración inicial de FiscalAPI insertada (AMBIENTE TEST)';
    PRINT '⚠️ IMPORTANTE: Actualizar los siguientes campos:';
    PRINT '   - ApiKey: Tu API Key de FiscalAPI';
    PRINT '   - RfcEmisor: RFC de tu empresa';
    PRINT '   - NombreEmisor: Razón social';
    PRINT '   - CodigoPostal: CP fiscal';
    PRINT '   - CertificadoBase64: Ejecutar CertificadoHelper.CertificadoABase64()';
    PRINT '   - LlavePrivadaBase64: Ejecutar CertificadoHelper.LlavePrivadaABase64()';
    PRINT '   - PasswordLlave: Contraseña del .key';
END
ELSE
BEGIN
    PRINT '⚠️ Ya existe una configuración activa de FiscalAPI';
END
GO

-- =====================================================================
-- 3. SCRIPT DE ACTUALIZACIÓN DE CERTIFICADOS (EJECUTAR DESPUÉS)
-- =====================================================================

-- Ejecuta esto DESPUÉS de convertir tus certificados a Base64
-- Usa: CertificadoHelperFiscalAPI.CertificadoABase64("ruta/al/certificado.cer")
-- Usa: CertificadoHelperFiscalAPI.LlavePrivadaABase64("ruta/a/llave.key")

/*
UPDATE [dbo].[ConfiguracionFiscalAPI]
SET 
    [CertificadoBase64] = 'BASE64_DEL_CER_AQUI',
    [LlavePrivadaBase64] = 'BASE64_DEL_KEY_AQUI',
    [PasswordLlave] = 'CONTRASEÑA_DE_LA_LLAVE',
    [FechaModificacion] = GETDATE()
WHERE [ConfiguracionID] = 1;
*/

-- =====================================================================
-- 4. CONSULTAR CONFIGURACIÓN ACTUAL
-- =====================================================================

SELECT 
    ConfiguracionID,
    ApiKey,
    Ambiente,
    CASE Ambiente 
        WHEN 'TEST' THEN 'https://test.fiscalapi.com'
        WHEN 'PRODUCCION' THEN 'https://api.fiscalapi.com'
    END AS UrlAPI,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CodigoPostal,
    CASE WHEN CertificadoBase64 IS NULL THEN '❌ NO CONFIGURADO' ELSE '✅ CONFIGURADO' END AS EstadoCertificado,
    CASE WHEN LlavePrivadaBase64 IS NULL THEN '❌ NO CONFIGURADO' ELSE '✅ CONFIGURADO' END AS EstadoLlave,
    Activo,
    FechaCreacion,
    FechaModificacion
FROM [dbo].[ConfiguracionFiscalAPI]
WHERE Activo = 1;
GO

-- =====================================================================
-- 5. CAMBIAR DE AMBIENTE TEST A PRODUCCIÓN
-- =====================================================================

-- ⚠️ SOLO EJECUTAR CUANDO ESTÉS LISTO PARA PRODUCCIÓN
-- ASEGÚRATE DE:
-- 1. Tener certificados de PRODUCCIÓN (.cer y .key)
-- 2. Tener API Key de PRODUCCIÓN
-- 3. Haber probado extensivamente en TEST

/*
UPDATE [dbo].[ConfiguracionFiscalAPI]
SET 
    [Ambiente] = 'PRODUCCION',
    [ApiKey] = 'TU_API_KEY_DE_PRODUCCION',
    -- Actualizar también certificados de producción si son diferentes
    -- [CertificadoBase64] = 'BASE64_CER_PRODUCCION',
    -- [LlavePrivadaBase64] = 'BASE64_KEY_PRODUCCION',
    [FechaModificacion] = GETDATE()
WHERE [ConfiguracionID] = 1;

PRINT '✅ Ambiente cambiado a PRODUCCIÓN';
PRINT '⚠️ Ahora el sistema timbra en: https://api.fiscalapi.com';
*/

-- =====================================================================
-- 6. VALIDAR CONFIGURACIÓN
-- =====================================================================

PRINT '';
PRINT '========================================';
PRINT 'VALIDACIÓN DE CONFIGURACIÓN';
PRINT '========================================';

DECLARE @ConfigCount INT;
DECLARE @TieneCertificados BIT;
DECLARE @Ambiente NVARCHAR(20);

SELECT 
    @ConfigCount = COUNT(*),
    @TieneCertificados = CASE 
        WHEN MIN(CASE WHEN CertificadoBase64 IS NOT NULL AND LlavePrivadaBase64 IS NOT NULL THEN 1 ELSE 0 END) = 1 
        THEN 1 ELSE 0 END,
    @Ambiente = MAX(Ambiente)
FROM ConfiguracionFiscalAPI
WHERE Activo = 1;

IF @ConfigCount = 0
BEGIN
    PRINT '❌ NO hay configuración activa de FiscalAPI';
    PRINT '   Ejecuta el INSERT de configuración inicial';
END
ELSE IF @ConfigCount > 1
BEGIN
    PRINT '⚠️ Hay múltiples configuraciones activas. Solo debe haber UNA.';
END
ELSE
BEGIN
    PRINT '✅ Configuración encontrada';
    PRINT '   Ambiente: ' + @Ambiente;
    
    IF @TieneCertificados = 1
        PRINT '   Certificados: ✅ Configurados';
    ELSE
        PRINT '   Certificados: ❌ FALTA CONFIGURAR Base64';
END

PRINT '';
PRINT '========================================';
PRINT 'PRÓXIMOS PASOS:';
PRINT '========================================';
PRINT '1. Actualizar ApiKey con tu clave de FiscalAPI';
PRINT '2. Actualizar RfcEmisor, NombreEmisor, CodigoPostal';
PRINT '3. Convertir tus certificados a Base64:';
PRINT '   - Usar: CertificadoHelperFiscalAPI.CertificadoABase64()';
PRINT '   - Usar: CertificadoHelperFiscalAPI.LlavePrivadaABase64()';
PRINT '4. Ejecutar UPDATE con los certificados Base64';
PRINT '5. Probar timbrado en ambiente TEST';
PRINT '6. Cuando esté listo, cambiar a PRODUCCION';
PRINT '========================================';
GO
