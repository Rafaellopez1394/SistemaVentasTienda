-- =====================================================
-- Script: 028_CONFIGURAR_PRODUCCION.sql
-- Descripción: Configura el sistema para PRODUCCIÓN REAL
-- IMPORTANTE: Solo ejecutar cuando tengas:
--   1. Certificados REALES del SAT (.cer y .key)
--   2. Credenciales de PRODUCCIÓN de Finkok
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURANDO SISTEMA PARA PRODUCCIÓN'
PRINT '========================================='
PRINT ''
PRINT '⚠️  ADVERTENCIA: Las facturas generadas serán OFICIALES ante el SAT'
PRINT ''

-- =====================================================
-- PASO 1: Actualizar RFC y datos fiscales REALES
-- =====================================================

PRINT 'PASO 1: Actualizando datos fiscales del emisor...'
PRINT ''
PRINT '❗ EDITAR ESTE SCRIPT ANTES DE EJECUTAR:'
PRINT '   Reemplazar los datos de ejemplo con tus datos REALES:'
PRINT ''

-- ⚠️ EDITAR ESTOS VALORES CON TUS DATOS REALES:
DECLARE @RFC_REAL VARCHAR(13) = 'ABC123456XYZ'  -- ← TU RFC REAL
DECLARE @RAZON_SOCIAL NVARCHAR(300) = N'TU RAZON SOCIAL COMPLETA'  -- ← TU RAZÓN SOCIAL
DECLARE @REGIMEN_FISCAL VARCHAR(3) = '612'  -- ← TU RÉGIMEN (601, 603, 605, 612, 621, 626)
DECLARE @CP VARCHAR(5) = '00000'  -- ← TU CÓDIGO POSTAL
DECLARE @CALLE NVARCHAR(200) = N'NOMBRE DE TU CALLE'  -- ← TU CALLE
DECLARE @NUM_EXT VARCHAR(50) = '123'  -- ← TU NÚMERO EXTERIOR
DECLARE @COLONIA NVARCHAR(200) = N'NOMBRE DE TU COLONIA'  -- ← TU COLONIA
DECLARE @MUNICIPIO NVARCHAR(200) = N'TU MUNICIPIO'  -- ← TU MUNICIPIO/ALCALDÍA
DECLARE @ESTADO NVARCHAR(200) = N'TU ESTADO'  -- ← TU ESTADO

-- Validación de RFC
IF @RFC_REAL = 'ABC123456XYZ'
BEGIN
    PRINT '❌ ERROR: Debes editar el script con tu RFC REAL'
    PRINT ''
    PRINT '   1. Abre este archivo en un editor'
    PRINT '   2. Busca la línea: DECLARE @RFC_REAL'
    PRINT '   3. Cambia ''ABC123456XYZ'' por tu RFC real'
    PRINT '   4. Actualiza todos los demás datos (razón social, dirección, etc.)'
    PRINT '   5. Guarda y ejecuta de nuevo'
    PRINT ''
    RAISERROR('Script no configurado. Ver instrucciones arriba.', 16, 1)
    RETURN
END

-- Actualizar configuración
UPDATE Configuracion
SET 
    RFC = @RFC_REAL,
    RazonSocial = @RAZON_SOCIAL,
    RegimenFiscal = @REGIMEN_FISCAL,
    CodigoPostal = @CP,
    Calle = @CALLE,
    NumeroExterior = @NUM_EXT,
    Colonia = @COLONIA,
    Municipio = @MUNICIPIO,
    Estado = @ESTADO,
    Pais = N'México'
WHERE ConfigID = 1

PRINT '✓ RFC configurado: ' + @RFC_REAL
PRINT '✓ Razón Social: ' + @RAZON_SOCIAL
PRINT '✓ Régimen Fiscal: ' + @REGIMEN_FISCAL
PRINT ''

-- =====================================================
-- PASO 2: Configurar Finkok en modo PRODUCCIÓN
-- =====================================================

PRINT 'PASO 2: Configurando PAC Finkok para PRODUCCIÓN...'
PRINT ''

-- ⚠️ EDITAR ESTAS CREDENCIALES CON LAS QUE TE DIO FINKOK:
DECLARE @USUARIO_PROD VARCHAR(100) = 'tu_usuario_produccion'  -- ← USUARIO DE PRODUCCIÓN
DECLARE @PASSWORD_PROD VARCHAR(100) = 'tu_password_produccion'  -- ← PASSWORD DE PRODUCCIÓN

IF @USUARIO_PROD = 'tu_usuario_produccion'
BEGIN
    PRINT '❌ ERROR: Debes configurar tus credenciales de Finkok PRODUCCIÓN'
    PRINT ''
    PRINT '   1. Contrata servicio en: https://www.finkok.com'
    PRINT '   2. Te darán usuario y password de PRODUCCIÓN'
    PRINT '   3. Edita este script con esas credenciales'
    PRINT '   4. Ejecuta de nuevo'
    PRINT ''
    RAISERROR('Credenciales de producción no configuradas.', 16, 1)
    RETURN
END

-- Actualizar a modo producción
UPDATE ConfiguracionPAC
SET 
    ProveedorPAC = 'Finkok',
    EsProduccion = 1,  -- ⚠️ MODO PRODUCCIÓN
    UrlTimbrado = 'https://facturacion.finkok.com/servicios/soap/stamp.wsdl',
    UrlCancelacion = 'https://facturacion.finkok.com/servicios/soap/cancel.wsdl',
    UrlConsulta = 'https://facturacion.finkok.com/servicios/soap/utilities.wsdl',
    Usuario = @USUARIO_PROD,
    Password = @PASSWORD_PROD,
    Activo = 1,
    FechaModificacion = GETDATE()
WHERE ConfigID = 1

PRINT '✓ Modo: PRODUCCIÓN ⚠️'
PRINT '✓ Usuario: ' + @USUARIO_PROD
PRINT '✓ URLs actualizadas a producción'
PRINT ''

-- =====================================================
-- RESUMEN Y SIGUIENTES PASOS
-- =====================================================

PRINT ''
PRINT '========================================='
PRINT '✓ CONFIGURACIÓN COMPLETADA'
PRINT '========================================='
PRINT ''
PRINT 'SIGUIENTE PASO: Cargar tu certificado REAL'
PRINT '--------------------------------------------'
PRINT '1. Ir a: http://localhost:50772/CertificadoDigital'
PRINT '2. Cargar tu certificado del SAT (.cer y .key)'
PRINT '3. Ingresar la contraseña de tu llave privada'
PRINT '4. Marcar como "Predeterminado"'
PRINT '5. Guardar'
PRINT ''
PRINT '⚠️  VERIFICAR ANTES DE FACTURAR:'
PRINT '--------------------------------------------'

-- Mostrar configuración actual
SELECT 
    '✓ RFC Configurado' AS Verificacion,
    RFC AS Valor
FROM Configuracion
WHERE ConfigID = 1

UNION ALL

SELECT 
    '✓ Modo PAC' AS Verificacion,
    CASE WHEN EsProduccion = 1 THEN 'PRODUCCIÓN ⚠️' ELSE 'DEMO' END AS Valor
FROM ConfiguracionPAC
WHERE ConfigID = 1

UNION ALL

SELECT 
    '✓ Certificados Activos' AS Verificacion,
    CAST(COUNT(*) AS VARCHAR) + ' certificado(s)'
FROM CertificadosDigitales
WHERE Activo = 1

PRINT ''
PRINT '⚠️  IMPORTANTE:'
PRINT '- Las facturas generadas serán OFICIALES'
PRINT '- Se reportarán al SAT automáticamente'
PRINT '- Se consumirán timbres de tu saldo en Finkok'
PRINT '- NO se pueden cancelar sin motivo fiscal válido'
PRINT ''
PRINT '✓ Sistema listo para facturación en PRODUCCIÓN'
PRINT ''
PRINT '========================================='
GO
