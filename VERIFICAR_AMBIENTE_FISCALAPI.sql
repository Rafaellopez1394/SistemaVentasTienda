-- =====================================================
-- VERIFICAR ESTADO: TEST vs PRODUCCIÓN
-- =====================================================
-- Script para confirmar en qué ambiente estás
-- Ejecutar ANTES y DESPUÉS de cambiar
-- =====================================================

USE DB_TIENDA;
GO

PRINT '';
PRINT '╔════════════════════════════════════════════════════╗';
PRINT '║  ESTADO ACTUAL DE FISCALAPI                        ║';
PRINT '╚════════════════════════════════════════════════════╝';
PRINT '';

-- 1. ESTADO DE CONFIGURACION PAC
PRINT '📌 CONFIGURACIÓN PAC (Principal)';
PRINT '═════════════════════════════════════════════════════';

SELECT 
    '✓ ConfigPACID' AS [Parámetro],
    CAST(ConfigPACID AS VARCHAR) AS [Valor]
FROM ConfiguracionPAC
WHERE ConfigPACID = 1
UNION ALL
SELECT 'Proveedor', NombreProveedor FROM ConfiguracionPAC WHERE ConfigPACID = 1
UNION ALL
SELECT 
    'Ambiente',
    CASE WHEN EsProduccion = 0 THEN '🟢 TEST' ELSE '🔴 PRODUCCIÓN' END
FROM ConfiguracionPAC WHERE ConfigPACID = 1
UNION ALL
SELECT 'ApiKey (primeros 30 chars)', LEFT(ApiKey, 30) + '...' FROM ConfiguracionPAC WHERE ConfigPACID = 1
UNION ALL
SELECT 'BaseURL', BaseURL FROM ConfiguracionPAC WHERE ConfigPACID = 1
UNION ALL
SELECT 'Activo', CASE WHEN Activo = 1 THEN 'SÍ' ELSE 'NO' END FROM ConfiguracionPAC WHERE ConfigPACID = 1;

PRINT '';
PRINT '📌 CONFIGURACIÓN EMPRESA';
PRINT '═════════════════════════════════════════════════════';

SELECT 
    '✓ RFC' AS [Parámetro],
    RFC AS [Valor]
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Razón Social', RazonSocial FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Nombre Comercial', NombreComercial FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Régimen Fiscal', RegimenFiscal FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Código Postal', CodigoPostal FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Archivo Certificado', NombreArchivoCertificado FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1
UNION ALL
SELECT 'Archivo Llave Privada', NombreArchivoLlavePrivada FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1;

PRINT '';
PRINT '📌 CERTIFICADOS DIGITALES EN BD';
PRINT '═════════════════════════════════════════════════════';

SELECT 
    CertificadoID,
    RFC,
    CASE WHEN Vigente = 1 THEN '✓ VIGENTE' ELSE '✗ EXPIRADO' END AS Estado,
    FechaVigenciaDesde,
    FechaVigenciaHasta,
    Certificado AS [Archivo]
FROM CertificadosDigitales
WHERE RFC = 'GAMA6111156JA'
ORDER BY FechaVigenciaHasta DESC;

IF NOT EXISTS (SELECT 1 FROM CertificadosDigitales WHERE RFC = 'GAMA6111156JA' AND Vigente = 1)
BEGIN
    PRINT '';
    PRINT '⚠️  NO HAY CERTIFICADOS VIGENTES EN BD';
    PRINT '    Necesitas cargar/subir los certificados a FiscalAPI';
END

PRINT '';
PRINT '═════════════════════════════════════════════════════';
PRINT '';

-- 2. RESUMEN DE ESTADO
PRINT '🔍 RESUMEN DE ESTADO';
PRINT '═════════════════════════════════════════════════════';

DECLARE @Ambiente NVARCHAR(20) = (SELECT CASE WHEN EsProduccion = 0 THEN 'TEST' ELSE 'PRODUCCIÓN' END FROM ConfiguracionPAC WHERE ConfigPACID = 1);
DECLARE @ApiKeyOK INT = (SELECT CASE WHEN ApiKey IS NOT NULL AND LEN(ApiKey) > 10 THEN 1 ELSE 0 END FROM ConfiguracionPAC WHERE ConfigPACID = 1);
DECLARE @CertificadoOK INT = (SELECT CASE WHEN Vigente = 1 THEN 1 ELSE 0 END FROM CertificadosDigitales WHERE RFC = 'GAMA6111156JA' LIMIT 1);
DECLARE @URLOK INT = (SELECT CASE WHEN BaseURL LIKE '%api.fiscalapi.com%' THEN 1 ELSE 0 END FROM ConfiguracionPAC WHERE ConfigPACID = 1);

PRINT '';
PRINT '📊 CHECKLIST DE CONFIGURACIÓN:';
PRINT '';
PRINT '   [' + CASE WHEN @Ambiente = 'PRODUCCIÓN' THEN '✓' ELSE ' ' END + '] Ambiente = ' + @Ambiente;
PRINT '   [' + CASE WHEN @ApiKeyOK = 1 THEN '✓' ELSE '✗' END + '] ApiKey configurada';
PRINT '   [' + CASE WHEN @CertificadoOK = 1 THEN '✓' ELSE '✗' END + '] Certificados vigentes';
PRINT '   [' + CASE WHEN @URLOK = 1 THEN '✓' ELSE '✗' END + '] URL correcta';

PRINT '';
PRINT '═════════════════════════════════════════════════════';
PRINT '';

-- 3. ESTADO DE FACTURAS RECIENTES
PRINT '📊 FACTURAS RECIENTES (últimas 10)';
PRINT '═════════════════════════════════════════════════════';

SELECT TOP 10
    FacturaID,
    CAST(FechaGeneracion AS DATE) AS Fecha,
    EstaFacturada AS Timbrada,
    CASE WHEN EstaFacturada = 1 THEN 
        CASE WHEN FiscalAPIInvoiceId IS NOT NULL THEN 'FiscalAPI ✓' ELSE 'Sin Invoice ID' END
    ELSE 'No timbrada' END AS Estado,
    XML_CFDI AS [¿Tiene CFDI?]
FROM Facturas
ORDER BY FechaGeneracion DESC;

PRINT '';
PRINT '═════════════════════════════════════════════════════';
PRINT '';

-- 4. CONCLUSIÓN
PRINT '💡 CONCLUSIÓN:';
PRINT '';
IF (SELECT EsProduccion FROM ConfiguracionPAC WHERE ConfigPACID = 1) = 1
    PRINT '   ✅ ESTÁS EN PRODUCCIÓN'
ELSE
    PRINT '   🟢 ESTÁS EN TEST'

PRINT '';
PRINT '📌 PRÓXIMAS ACCIONES:';
IF (SELECT EsProduccion FROM ConfiguracionPAC WHERE ConfigPACID = 1) = 0
BEGIN
    PRINT '   1. Para cambiar a PRODUCCIÓN, ejecuta: CAMBIAR_A_PRODUCCION.sql';
    PRINT '   2. Asegúrate de tener:';
    PRINT '      - API Key de producción (sk_live_...)';
    PRINT '      - Certificados CSD reales';
    PRINT '      - RFC correcto en SAT';
END
ELSE
BEGIN
    PRINT '   1. ¡Ya estás en PRODUCCIÓN!';
    PRINT '   2. Genera facturas con confianza';
    PRINT '   3. Verifica en SAT que aparecen correctamente';
END

PRINT '';
PRINT '═════════════════════════════════════════════════════';
PRINT '';

GO
