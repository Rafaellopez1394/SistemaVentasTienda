-- ================================================
-- Configurar certificados desde archivos en CapaDatos/Certifies
-- ================================================
USE DB_TIENDA;
GO

-- 1. Eliminar columnas Base64 si existen (ya no se usan)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'CertificadoBase64')
BEGIN
    ALTER TABLE ConfiguracionEmpresa DROP COLUMN CertificadoBase64;
    PRINT 'Columna CertificadoBase64 eliminada';
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'LlavePrivadaBase64')
BEGIN
    ALTER TABLE ConfiguracionEmpresa DROP COLUMN LlavePrivadaBase64;
    PRINT 'Columna LlavePrivadaBase64 eliminada';
END
GO

-- 2. Agregar columnas para nombres de archivos
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'NombreArchivoCertificado')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD NombreArchivoCertificado VARCHAR(255) NULL;
    PRINT 'Columna NombreArchivoCertificado agregada';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'NombreArchivoLlavePrivada')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD NombreArchivoLlavePrivada VARCHAR(255) NULL;
    PRINT 'Columna NombreArchivoLlavePrivada agregada';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'NombreArchivoPassword')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD NombreArchivoPassword VARCHAR(255) NULL;
    PRINT 'Columna NombreArchivoPassword agregada';
END
GO

-- Eliminar PasswordCertificado si existe (ya no se usa)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'PasswordCertificado')
BEGIN
    ALTER TABLE ConfiguracionEmpresa DROP COLUMN PasswordCertificado;
    PRINT 'Columna PasswordCertificado eliminada (password ahora en archivo)';
END
GO

-- 3. Configurar nombres de archivos de certificados
-- IMPORTANTE: Los archivos deben existir en la carpeta CapaDatos/Certifies

-- Archivos requeridos:
-- 1. Certificado: EKU9003173C9.cer (o tu certificado.cer)
-- 2. Llave privada: EKU9003173C9.key (o tu llave.key)
-- 3. Password: password (archivo de texto con el password)

-- Para certificados de prueba de FiscalAPI:
-- Descargar de: https://test.fiscalapi.com/files/tax-files/tax-files.zip
-- Extraer EKU9003173C9.cer y EKU9003173C9.key
-- Crear archivo "password" con contenido: 12345678a

UPDATE ConfiguracionEmpresa
SET 
    NombreArchivoCertificado = 'EKU9003173C9.cer',      -- Cambiar por tu certificado
    NombreArchivoLlavePrivada = 'EKU9003173C9.key',     -- Cambiar por tu llave privada
    NombreArchivoPassword = 'password',                 -- Archivo que contiene el password
    UsuarioModificacion = 'SISTEMA',
    FechaModificacion = GETDATE()
WHERE ConfigEmpresaID = 1;

-- 4. Verificar configuración
SELECT 
    RFC,
    RazonSocial,
    RegimenFiscal,
    CodigoPostal,
    NombreArchivoCertificado,
    NombreArchivoLlavePrivada,
    NombreArchivoPassword
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;

PRINT ''
PRINT '================================================'
PRINT 'SIGUIENTE PASO:'
PRINT '1. Copiar 3 archivos a CapaDatos\Certifies\'
PRINT '   - Certificado .cer'
PRINT '   - Llave privada .key'
PRINT '   - Archivo "password" con el password dentro'
PRINT '2. Verificar que los nombres coincidan con la configuracion'
PRINT '3. Reiniciar aplicacion web'
PRINT '4. Probar facturacion'
PRINT '================================================'
GO
