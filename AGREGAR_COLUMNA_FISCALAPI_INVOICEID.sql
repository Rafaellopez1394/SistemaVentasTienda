-- Agregar columna FiscalAPIInvoiceId a tabla Facturas
-- Para almacenar el ID de factura de FiscalAPI y poder descargar el PDF oficial

USE DB_TIENDA
GO

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Facturas' 
    AND COLUMN_NAME = 'FiscalAPIInvoiceId'
)
BEGIN
    ALTER TABLE Facturas
    ADD FiscalAPIInvoiceId NVARCHAR(100) NULL
    
    PRINT 'Columna FiscalAPIInvoiceId agregada exitosamente'
END
ELSE
BEGIN
    PRINT 'La columna FiscalAPIInvoiceId ya existe'
END
GO
