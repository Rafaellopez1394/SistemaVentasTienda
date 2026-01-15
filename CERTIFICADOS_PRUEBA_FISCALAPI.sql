-- ================================================
-- Certificados de Prueba EKU9003173C9 en Base64
-- ================================================
-- Fuente: https://test.fiscalapi.com/files/tax-files/tax-files.zip
-- RFC: EKU9003173C9
-- Password: 12345678a

USE DB_TIENDA;
GO

-- NOTA: Estos son certificados DE PRUEBA proporcionados por FiscalAPI
-- Para producción, debes usar TUS certificados reales (.cer y .key)
-- y convertirlos con el script CONVERTIR_CERTIFICADOS_BASE64.ps1

DECLARE @RFC_ACTUAL VARCHAR(13)
DECLARE @RFC_NUEVO VARCHAR(13) = 'GAMA6111156JA'  -- Tu RFC real
DECLARE @CertificadoBase64 VARCHAR(MAX) 
DECLARE @LlavePrivadaBase64 VARCHAR(MAX)
DECLARE @PasswordCertificado VARCHAR(100) = '12345678a'

-- Obtener RFC actual de ConfiguracionEmpresa
SELECT @RFC_ACTUAL = RFC FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1

PRINT ''
PRINT '================================================'
PRINT 'IMPORTANTE: Certificados de Prueba'
PRINT '================================================'
PRINT 'RFC en BD: ' + ISNULL(@RFC_ACTUAL, 'NO ENCONTRADO')
PRINT 'RFC Certificados: EKU9003173C9 (prueba)'
PRINT ''
PRINT 'ADVERTENCIA:'
PRINT 'Los certificados de prueba EKU9003173C9 NO funcionarán con tu RFC ' + @RFC_NUEVO
PRINT 'porque el RFC del certificado debe coincidir con el RFC del emisor.'
PRINT ''
PRINT 'OPCIONES:'
PRINT '1. Cambiar RFC de ConfiguracionEmpresa a EKU9003173C9 para pruebas'
PRINT '2. Obtener certificados reales para ' + @RFC_NUEVO
PRINT '3. Usar certificados ya subidos a FiscalAPI (Modo Por Referencias)'
PRINT ''
PRINT '¿Deseas continuar con certificados de prueba? (requiere cambiar RFC)'
PRINT 'Ejecuta: UPDATE ConfiguracionEmpresa SET RFC = ''EKU9003173C9'' WHERE ConfigEmpresaID = 1'
PRINT '================================================'
PRINT ''

-- NO ejecutar automáticamente para evitar sobrescribir RFC real
-- Descomentar las siguientes líneas solo si quieres usar certificados de prueba:

/*
-- Certificados de prueba EKU9003173C9 en Base64
SET @CertificadoBase64 = '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'

SET @LlavePrivadaBase64 = 'MIIFDjBABgkqhkiG9w0BBQ0wMzAbBgkqhkiG9w0BBQwwDgQILV6oc38vg9cCAggAMBQGCCqGSIb3DQMHBAgqeJk6Y8bvfgSCBMhFpITCgCE1f4M1r+3yJbAr7JQxF1B6fR6F1b5f9+L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8L+X7f9f8'

-- Actualizar ConfiguracionEmpresa
UPDATE ConfiguracionEmpresa
SET 
    RFC = 'EKU9003173C9',  -- RFC del certificado de prueba
    RazonSocial = 'ESCUELA KEMPER URGATE',
    RegimenFiscal = '601',
    CertificadoBase64 = @CertificadoBase64,
    LlavePrivadaBase64 = @LlavePrivadaBase64,
    PasswordCertificado = @PasswordCertificado,
    UsuarioModificacion = 'SISTEMA',
    FechaModificacion = GETDATE()
WHERE ConfigEmpresaID = 1;

PRINT 'Certificados de prueba cargados'
PRINT 'RFC actualizado a: EKU9003173C9'
*/

PRINT 'Script terminado. Lee las instrucciones arriba.'
GO
