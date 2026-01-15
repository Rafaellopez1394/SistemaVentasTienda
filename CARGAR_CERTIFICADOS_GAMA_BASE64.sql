-- ================================================================
-- Script: CARGAR_CERTIFICADOS_GAMA_BASE64.sql
-- Descripción: Carga certificados de GAMA6111156JA en base64
-- ================================================================

USE DB_TIENDA;
GO

DECLARE @CertificadoBase64 NVARCHAR(MAX);
DECLARE @LlaveBase64 NVARCHAR(MAX);
DECLARE @Password NVARCHAR(100);

-- Leer archivos y convertir a Base64 (se debe ejecutar desde PowerShell)
-- Aquí solo actualizamos con placeholder

-- Contraseña típica de certificados de prueba (debes verificar la correcta)
SET @Password = '12345678a'; -- O la contraseña real del certificado

PRINT '================================================================';
PRINT 'Actualizando certificados en ConfiguracionProdigia';
PRINT '================================================================';
PRINT '';

-- NOTA: Los valores Base64 deben ser copiados desde PowerShell
-- Ejecutar primero:
-- $cer = [Convert]::ToBase64String([IO.File]::ReadAllBytes(".\CapaDatos\Certifies\GAMA6111156JA.cer"))
-- $key = [Convert]::ToBase64String([IO.File]::ReadAllBytes(".\CapaDatos\Certifies\GAMA6111156JA.key"))
-- echo $cer > cert.txt
-- echo $key > key.txt

-- Por ahora, dejamos NULL para usar CERT_DEFAULT de PADE
UPDATE ConfiguracionProdigia
SET 
    CertificadoBase64 = NULL, -- Pegar aquí el Base64 del .cer
    LlavePrivadaBase64 = NULL, -- Pegar aquí el Base64 del .key
    PasswordLlave = @Password
WHERE ConfiguracionID = 1;

PRINT '✓ Configuración actualizada';
PRINT '';
PRINT 'IMPORTANTE:';
PRINT '  - Los certificados están en NULL (usando CERT_DEFAULT de PADE)';
PRINT '  - Para usar certificados propios, ejecutar el script PowerShell primero';
PRINT '';

SELECT 
    ConfiguracionID,
    RfcEmisor,
    NombreEmisor,
    CASE WHEN CertificadoBase64 IS NULL THEN 'NULL (CERT_DEFAULT)' ELSE 'CARGADO' END AS CertificadoStatus,
    CASE WHEN LlavePrivadaBase64 IS NULL THEN 'NULL (CERT_DEFAULT)' ELSE 'CARGADO' END AS LlaveStatus,
    PasswordLlave
FROM ConfiguracionProdigia;

GO
