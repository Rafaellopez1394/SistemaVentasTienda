-- Agregar columnas para certificados digitales en ConfiguracionEmpresa
-- Estos se usan en Modo "Por Valores" de FiscalAPI SDK
USE DB_TIENDA;
GO

-- Verificar si las columnas ya existen
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'CertificadoBase64')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD CertificadoBase64 VARCHAR(MAX) NULL;
    PRINT 'Columna CertificadoBase64 agregada';
END
ELSE
    PRINT 'Columna CertificadoBase64 ya existe';
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'LlavePrivadaBase64')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD LlavePrivadaBase64 VARCHAR(MAX) NULL;
    PRINT 'Columna LlavePrivadaBase64 agregada';
END
ELSE
    PRINT 'Columna LlavePrivadaBase64 ya existe';
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ConfiguracionEmpresa' AND COLUMN_NAME = 'PasswordCertificado')
BEGIN
    ALTER TABLE ConfiguracionEmpresa
    ADD PasswordCertificado VARCHAR(100) NULL;
    PRINT 'Columna PasswordCertificado agregada';
END
ELSE
    PRINT 'Columna PasswordCertificado ya existe';
GO

-- Verificar la estructura actualizada
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ConfiguracionEmpresa'
ORDER BY ORDINAL_POSITION;
GO
