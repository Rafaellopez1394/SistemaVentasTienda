-- ========================================================
-- FIX: Expandir campo NoCertificado en CertificadosDigitales
-- Problema: Campo es muy pequeño (20) para números de certificado largos
-- ========================================================
USE DB_TIENDA
GO

PRINT '🔧 Expandiendo campo NoCertificado...'

-- Expandir NoCertificado de 20 a 50 caracteres
ALTER TABLE CertificadosDigitales
ALTER COLUMN NoCertificado VARCHAR(50) NULL;

PRINT '✅ Campo NoCertificado expandido a VARCHAR(50)'

-- Verificar cambio
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CertificadosDigitales'
  AND COLUMN_NAME = 'NoCertificado';

PRINT '✅ Campo actualizado correctamente'
GO
