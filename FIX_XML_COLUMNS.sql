-- =====================================================================
-- Script: Cambiar columnas XML a NVARCHAR(MAX) en tabla Facturas
-- Razón: SQL Server XML no acepta declaración de encoding UTF-8
-- Fecha: 2026-01-10
-- =====================================================================

USE DB_TIENDA
GO

PRINT 'Cambiando tipo de columnas XML a NVARCHAR(MAX)...'

-- Cambiar XMLOriginal de XML a NVARCHAR(MAX)
ALTER TABLE Facturas
ALTER COLUMN XMLOriginal NVARCHAR(MAX) NULL
GO

-- Cambiar XMLTimbrado de XML a NVARCHAR(MAX)
ALTER TABLE Facturas
ALTER COLUMN XMLTimbrado NVARCHAR(MAX) NULL
GO

PRINT '✅ Columnas actualizadas exitosamente'

-- Verificar cambios
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Facturas'
AND COLUMN_NAME IN ('XMLOriginal', 'XMLTimbrado')
GO
