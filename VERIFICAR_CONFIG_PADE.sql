-- =====================================================
-- Script: Verificar Configuración PADE
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'VERIFICACIÓN DE CONFIGURACIÓN PADE'
PRINT '========================================='
PRINT ''

-- Verificar configuración actual
SELECT 
    ConfiguracionID,
    Usuario AS [Usuario PADE],
    CASE 
        WHEN LEN(Password) > 0 THEN '****' + RIGHT(Password, 3)
        ELSE 'NO CONFIGURADO'
    END AS [Password],
    Contrato,
    Ambiente AS [Ambiente (TEST/PROD)],
    CASE 
        WHEN Ambiente = 'TEST' THEN 'https://pruebas.pade.mx/'
        ELSE 'https://timbrado.pade.mx/'
    END AS [URL API],
    RfcEmisor AS [RFC Emisor],
    NombreEmisor AS [Nombre/Razón Social],
    RegimenFiscal AS [Régimen Fiscal],
    CodigoPostal AS [CP Expedición],
    CASE 
        WHEN CertificadoBase64 IS NULL OR LEN(CertificadoBase64) = 0 
        THEN 'NO (usar CERT_DEFAULT)' 
        ELSE 'SI (' + CAST(LEN(CertificadoBase64) AS VARCHAR) + ' chars)'
    END AS [Certificado en BD],
    CASE WHEN Activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado,
    FechaCreacion AS [Fecha Creación],
    FechaModificacion AS [Última Modificación]
FROM ConfiguracionProdigia
WHERE ConfiguracionID = 1

PRINT ''
PRINT '========================================='
PRINT 'VALIDACIONES'
PRINT '========================================='

-- Validar que no tenga valores placeholder
DECLARE @Usuario VARCHAR(100), @Password VARCHAR(200), @Contrato VARCHAR(100)
SELECT @Usuario = Usuario, @Password = Password, @Contrato = Contrato 
FROM ConfiguracionProdigia WHERE ConfiguracionID = 1

IF @Usuario LIKE '%USUARIO%' OR @Usuario LIKE '%TU_%'
    PRINT '⚠️  ADVERTENCIA: Usuario parece ser placeholder - Actualizar con credencial real'
ELSE
    PRINT '✅ Usuario configurado'

IF @Password LIKE '%PASSWORD%' OR @Password LIKE '%TU_%'
    PRINT '⚠️  ADVERTENCIA: Password parece ser placeholder - Actualizar con credencial real'
ELSE
    PRINT '✅ Password configurado'

IF @Contrato LIKE '%CONTRATO%' OR @Contrato LIKE '%TU_%'
    PRINT '⚠️  ADVERTENCIA: Contrato parece ser placeholder - Actualizar con credencial real'
ELSE
    PRINT '✅ Contrato configurado'

PRINT ''
PRINT '========================================='
PRINT 'ESTADO FISCALAPI (Debe estar inactivo)'
PRINT '========================================='

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionFiscalAPI')
BEGIN
    SELECT 
        CASE WHEN Activo = 1 THEN '⚠️  ACTIVO (desactivar)' ELSE '✅ INACTIVO' END AS Estado,
        ApiKey AS Usuario,
        Ambiente
    FROM ConfiguracionFiscalAPI
END
ELSE
    PRINT '✅ Tabla ConfiguracionFiscalAPI no existe'

PRINT ''
PRINT '========================================='
PRINT 'SIGUIENTE PASO: COMPILAR PROYECTO'
PRINT '========================================='
PRINT ''
PRINT 'Ejecuta: .\compilar_proyecto.ps1'
PRINT 'O desde Visual Studio: Build -> Rebuild Solution'
PRINT ''
