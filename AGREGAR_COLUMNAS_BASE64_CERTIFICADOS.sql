-- =============================================
-- AGREGAR COLUMNAS BASE64 A CERTIFICADOS
-- =============================================
USE DB_TIENDA;
GO

-- Verificar si las columnas ya existen
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('CertificadosDigitales') 
               AND name = 'CertificadoBase64')
BEGIN
    ALTER TABLE CertificadosDigitales
    ADD CertificadoBase64 NVARCHAR(MAX) NULL;
    
    PRINT 'Columna CertificadoBase64 agregada';
END
ELSE
BEGIN
    PRINT 'Columna CertificadoBase64 ya existe';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('CertificadosDigitales') 
               AND name = 'LlavePrivadaBase64')
BEGIN
    ALTER TABLE CertificadosDigitales
    ADD LlavePrivadaBase64 NVARCHAR(MAX) NULL;
    
    PRINT 'Columna LlavePrivadaBase64 agregada';
END
ELSE
BEGIN
    PRINT 'Columna LlavePrivadaBase64 ya existe';
END
GO

-- Verificar las columnas
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CertificadosDigitales'
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Columnas Base64 agregadas correctamente a CertificadosDigitales';
