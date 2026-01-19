-- Script para actualizar FiscalAPIInvoiceId de facturas ya timbradas
-- Ejecutar MANUALMENTE después de obtener el InvoiceId de FiscalAPI

-- Para la factura con UUID: 013416dd-b424-454d-89be-91a62f9a1da7
-- Necesitas obtener el InvoiceId desde el dashboard de FiscalAPI o mediante la API

-- Paso 1: Consultar las facturas sin InvoiceId
SELECT 
    FacturaID,
    Serie,
    Folio,
    UUID,
    FiscalAPIInvoiceId,
    FechaCreacion
FROM Facturas
WHERE UUID IS NOT NULL 
AND FiscalAPIInvoiceId IS NULL
ORDER BY FechaCreacion DESC

-- Paso 2: Una vez que tengas el InvoiceId de FiscalAPI, ejecuta este UPDATE
-- Reemplaza 'TU_INVOICE_ID_AQUI' con el InvoiceId real de FiscalAPI

/*
UPDATE Facturas
SET FiscalAPIInvoiceId = 'TU_INVOICE_ID_AQUI'
WHERE UUID = '013416dd-b424-454d-89be-91a62f9a1da7'

-- Verificar
SELECT FacturaID, Serie, Folio, UUID, FiscalAPIInvoiceId
FROM Facturas
WHERE UUID = '013416dd-b424-454d-89be-91a62f9a1da7'
*/
