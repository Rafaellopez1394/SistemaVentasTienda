-- ================================================
-- Script para cargar certificados CSD en Base64
-- ================================================
-- Instrucciones:
-- 1. Descargar certificados de prueba de https://test.fiscalapi.com/files/tax-files/tax-files.zip
-- 2. Extraer archivos EKU9003173C9.cer y EKU9003173C9.key
-- 3. Convertir a Base64 usando PowerShell (ver script abajo)
-- 4. Ejecutar este script SQL con los valores Base64

USE DB_TIENDA;
GO

-- IMPORTANTE: Reemplazar estos valores con el resultado del script PowerShell
-- Ejecutar primero: .\CONVERTIR_CERTIFICADOS_BASE64.ps1

DECLARE @RFC VARCHAR(13) = 'GAMA6111156JA'  -- Tu RFC real
DECLARE @CertificadoBase64 VARCHAR(MAX) = ''  -- Pegar aquí el resultado de $certBase64
DECLARE @LlavePrivadaBase64 VARCHAR(MAX) = '' -- Pegar aquí el resultado de $keyBase64
DECLARE @PasswordCertificado VARCHAR(100) = '12345678a' -- Password del certificado de prueba

-- Validar que se proporcionaron los certificados
IF LEN(@CertificadoBase64) < 100 OR LEN(@LlavePrivadaBase64) < 100
BEGIN
    PRINT 'ERROR: Debes ejecutar primero el script PowerShell CONVERTIR_CERTIFICADOS_BASE64.ps1'
    PRINT 'para obtener los certificados en Base64 y pegarlos en este script.'
    RETURN
END

-- Actualizar ConfiguracionEmpresa con los certificados
UPDATE ConfiguracionEmpresa
SET 
    CertificadoBase64 = @CertificadoBase64,
    LlavePrivadaBase64 = @LlavePrivadaBase64,
    PasswordCertificado = @PasswordCertificado,
    UsuarioModificacion = 'SISTEMA',
    FechaModificacion = GETDATE()
WHERE ConfigEmpresaID = 1;

-- Verificar actualización
IF @@ROWCOUNT > 0
BEGIN
    PRINT 'Certificados actualizados correctamente para RFC: ' + @RFC
    PRINT 'Longitud CertificadoBase64: ' + CAST(LEN(@CertificadoBase64) AS VARCHAR(10))
    PRINT 'Longitud LlavePrivadaBase64: ' + CAST(LEN(@LlavePrivadaBase64) AS VARCHAR(10))
    
    -- Mostrar primeros 50 caracteres de cada certificado para verificación
    SELECT 
        RFC,
        RazonSocial,
        LEFT(CertificadoBase64, 50) + '...' AS CertificadoBase64_Preview,
        LEFT(LlavePrivadaBase64, 50) + '...' AS LlavePrivadaBase64_Preview,
        PasswordCertificado
    FROM ConfiguracionEmpresa
    WHERE ConfigEmpresaID = 1;
END
ELSE
BEGIN
    PRINT 'ERROR: No se pudo actualizar. Verifica que existe ConfigEmpresaID = 1'
END
GO
