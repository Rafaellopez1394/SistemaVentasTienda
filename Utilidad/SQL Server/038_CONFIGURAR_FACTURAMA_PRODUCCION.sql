USE DB_TIENDA
GO

-- Actualizar credenciales de Facturama a PRODUCCIÓN
UPDATE ConfiguracionPAC
SET 
    EsProduccion = 1,  -- MODO PRODUCCIÓN
    Usuario = 'mercadomar',
    Password = 'Mercadomar2025',
    UrlTimbrado = 'https://api.facturama.mx/2/cfdis',
    UrlCancelacion = 'https://api.facturama.mx/cfdi',
    UrlConsulta = 'https://api.facturama.mx/cfdi',
    Activo = 1,  -- Activar Facturama
    FechaModificacion = GETDATE()
WHERE ProveedorPAC = 'Facturama';

-- Desactivar simulador
UPDATE ConfiguracionPAC
SET Activo = 0
WHERE ProveedorPAC = 'Simulador';

PRINT ' Facturama configurada en modo PRODUCCIÓN';
PRINT ' Credenciales actualizadas';
PRINT ' Simulador desactivado';

-- Mostrar configuración actualizada
SELECT 
    ProveedorPAC,
    CASE WHEN EsProduccion = 1 THEN 'PRODUCCIÓN' ELSE 'Sandbox' END AS Modo,
    Usuario,
    UrlTimbrado,
    CASE WHEN Activo = 1 THEN 'ACTIVO' ELSE 'Inactivo' END AS Estado
FROM ConfiguracionPAC;
GO
