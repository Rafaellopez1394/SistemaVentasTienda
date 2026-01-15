-- =============================================
-- Script: Configurar FiscalAPI TEST
-- Descripción: Almacena credenciales de FiscalAPI
--              para ambiente de pruebas
-- Fecha: 15 Enero 2026
-- =============================================

USE DB_TIENDA;
GO

-- Crear tabla si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionFiscalAPI]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfiguracionFiscalAPI](
        [ConfiguracionID] INT IDENTITY(1,1) PRIMARY KEY,
        [TenantKey] NVARCHAR(100) NOT NULL,
        [ApiKey] NVARCHAR(200) NOT NULL,
        [Ambiente] NVARCHAR(20) NOT NULL DEFAULT 'TEST', -- TEST o PRODUCCION
        [UrlApi] NVARCHAR(200) NOT NULL,
        [RfcEmisor] NVARCHAR(13) NULL,
        [NombreEmisor] NVARCHAR(300) NULL,
        [RegimenFiscal] NVARCHAR(10) NULL,
        [CertificadoBase64] NVARCHAR(MAX) NULL,
        [LlavePrivadaBase64] NVARCHAR(MAX) NULL,
        [PasswordLlave] NVARCHAR(100) NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaActualizacion] DATETIME NULL
    );
    
    PRINT '✅ Tabla ConfiguracionFiscalAPI creada';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla ConfiguracionFiscalAPI ya existe';
END
GO

-- Insertar configuración TEST
DELETE FROM ConfiguracionFiscalAPI WHERE Ambiente = 'TEST';

INSERT INTO ConfiguracionFiscalAPI (
    TenantKey,
    ApiKey,
    Ambiente,
    UrlApi,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CertificadoBase64,
    LlavePrivadaBase64,
    PasswordLlave,
    Activo
)
VALUES (
    'e0a0d1de-d225-46de-b95f-55d04f2787ff',
    'sk_test_47126aed_6c71_4060_b05b_932c4423dd00',
    'TEST',
    'https://test.fiscalapi.com',
    'GAMA6111156JA',
    'ALMA ROSA GAXIOLA MONTOYA',
    '612',
    '', -- Se llenará después con los certificados
    '', -- Se llenará después con la llave privada
    'GAMA151161',
    1
);

PRINT '✅ Credenciales FiscalAPI TEST guardadas';
GO

-- Consultar configuración
SELECT 
    ConfiguracionID,
    TenantKey,
    LEFT(ApiKey, 30) + '...' AS ApiKey,
    Ambiente,
    UrlApi,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CASE WHEN LEN(CertificadoBase64) > 0 THEN 'Cargado' ELSE 'Vacío' END AS Certificado,
    CASE WHEN LEN(LlavePrivadaBase64) > 0 THEN 'Cargado' ELSE 'Vacío' END AS LlavePrivada,
    PasswordLlave,
    Activo,
    FechaCreacion
FROM ConfiguracionFiscalAPI
WHERE Ambiente = 'TEST';
GO
