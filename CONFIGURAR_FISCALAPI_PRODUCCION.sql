-- =====================================================
-- CONFIGURAR FISCALAPI PARA PRODUCCIÓN
-- =====================================================
USE DB_TIENDA;
GO

-- 1. Configurar PAC en PRODUCCIÓN
UPDATE ConfiguracionPAC
SET 
    NombreProveedor = 'FiscalAPI',
    EsProduccion = 1,                                    -- PRODUCCIÓN
    ApiKey = 'sk_live_TU_APIKEY_DE_PRODUCCION',          -- ⚠️ CAMBIAR por tu API Key real
    BaseURL = 'https://api.fiscalapi.com',               -- API producción
    UsuarioTimbrado = NULL,
    PasswordTimbrado = NULL,
    Activo = 1,
    FechaModificacion = GETDATE()
WHERE ConfigPACID = 1;

-- 2. Configurar empresa con certificados REALES
UPDATE ConfiguracionEmpresa
SET 
    RFC = 'GAMA6111156JA',
    RazonSocial = 'ALMA ROSA GAXIOLA MONTOYA',
    NombreComercial = 'LAS AGUILAS MERCADO DEL MAR',
    RegimenFiscal = '612',
    CodigoPostal = '81048',
    NombreArchivoCertificado = 'GAMA6111156JA.cer',      -- ⚠️ Certificado REAL
    NombreArchivoLlavePrivada = 'GAMA6111156JA.key',     -- ⚠️ Llave REAL
    NombreArchivoPassword = 'password',                  -- ⚠️ Password REAL
    FechaModificacion = GETDATE()
WHERE ConfigEmpresaID = 1;

-- 3. Verificar configuración
SELECT 
    'ConfiguracionPAC' AS Tabla,
    NombreProveedor,
    CASE WHEN EsProduccion = 1 THEN '🔴 PRODUCCIÓN' ELSE '🟢 TEST' END AS Ambiente,
    LEFT(ApiKey, 20) + '...' AS ApiKey,
    BaseURL
FROM ConfiguracionPAC WHERE Activo = 1;

SELECT 
    'ConfiguracionEmpresa' AS Tabla,
    RFC,
    RazonSocial,
    NombreArchivoCertificado,
    NombreArchivoLlavePrivada
FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1;

GO
PRINT '';
PRINT '✅ Configuración de PRODUCCIÓN lista';
PRINT '';
PRINT '⚠️  IMPORTANTE:';
PRINT '   1. Actualiza el ApiKey con tu clave de producción';
PRINT '   2. Verifica que los archivos GAMA6111156JA.cer y .key estén en CapaDatos\Certifies\';
PRINT '   3. Actualiza el archivo "password" con la contraseña real';
PRINT '   4. Haz una factura de prueba pequeña primero';
PRINT '';
