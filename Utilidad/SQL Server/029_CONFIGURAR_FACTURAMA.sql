-- =====================================================
-- Script: 029_CONFIGURAR_FACTURAMA.sql
-- Descripción: Configura Facturama como proveedor PAC
-- Ventajas: Plan FREE 50 facturas/mes, timbres sin caducidad
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURANDO FACTURAMA COMO PAC'
PRINT '========================================='
PRINT ''

-- =====================================================
-- OPCIÓN 1: MODO SANDBOX (PRUEBAS GRATIS)
-- =====================================================

PRINT 'Configurando Facturama en modo SANDBOX (pruebas)...'
PRINT ''

-- Actualizar configuración PAC para Facturama Sandbox
UPDATE ConfiguracionPAC
SET 
    ProveedorPAC = 'Facturama',
    EsProduccion = 0,  -- Modo SANDBOX (pruebas)
    UrlTimbrado = 'https://apisandbox.facturama.mx/cfdi',
    UrlCancelacion = 'https://apisandbox.facturama.mx/cfdi',
    UrlConsulta = 'https://apisandbox.facturama.mx/cfdi',
    Usuario = 'pruebas',  -- Usuario de prueba
    Password = 'pruebas2011',  -- Password de prueba
    Activo = 1,
    FechaModificacion = GETDATE()
WHERE ConfigID = 1

IF @@ROWCOUNT = 0
BEGIN
    -- Si no existe, crear configuración
    INSERT INTO ConfiguracionPAC (
        ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta, 
        Usuario, Password, Activo
    )
    VALUES (
        'Facturama',
        0,
        'https://apisandbox.facturama.mx/cfdi',
        'https://apisandbox.facturama.mx/cfdi',
        'https://apisandbox.facturama.mx/cfdi',
        'pruebas',
        'pruebas2011',
        1
    )
END

PRINT '✓ Facturama configurado en modo SANDBOX'
PRINT '  Usuario: pruebas'
PRINT '  Password: pruebas2011'
PRINT '  URL: https://apisandbox.facturama.mx'
PRINT ''
PRINT '⚠️  IMPORTANTE:'
PRINT '  - Este modo es para PRUEBAS'
PRINT '  - Las facturas NO son válidas ante el SAT'
PRINT '  - Puedes probar sin límite'
PRINT '  - NO necesitas certificados reales'
PRINT ''

-- =====================================================
-- INFORMACIÓN PARA MODO PRODUCCIÓN
-- =====================================================

PRINT ''
PRINT '========================================='
PRINT 'PARA CAMBIAR A PRODUCCIÓN (REAL)'
PRINT '========================================='
PRINT ''
PRINT '1. REGISTRARTE EN FACTURAMA (GRATIS):'
PRINT '   https://www.facturama.mx/registro'
PRINT ''
PRINT '2. OBTENER TUS CREDENCIALES:'
PRINT '   - Ingresar a tu panel: https://www.facturama.mx/login'
PRINT '   - Ir a: Configuración → API Keys'
PRINT '   - Copiar: Usuario (email) y Contraseña'
PRINT ''
PRINT '3. CARGAR TU CERTIFICADO DEL SAT:'
PRINT '   - En tu panel Facturama'
PRINT '   - Subir archivo .CER y .KEY'
PRINT '   - Ingresar contraseña de la llave'
PRINT ''
PRINT '4. EJECUTAR ESTE SQL CON TUS DATOS:'
PRINT ''
PRINT '   UPDATE ConfiguracionPAC'
PRINT '   SET '
PRINT '       EsProduccion = 1,'
PRINT '       UrlTimbrado = ''https://api.facturama.mx/cfdi'','
PRINT '       UrlCancelacion = ''https://api.facturama.mx/cfdi'','
PRINT '       UrlConsulta = ''https://api.facturama.mx/cfdi'','
PRINT '       Usuario = ''TU_EMAIL@EJEMPLO.COM'',  -- Tu email de Facturama'
PRINT '       Password = ''TU_CONTRASEÑA'',        -- Tu contraseña de Facturama'
PRINT '       FechaModificacion = GETDATE()'
PRINT '   WHERE ConfigID = 1'
PRINT ''

-- =====================================================
-- MOSTRAR CONFIGURACIÓN ACTUAL
-- =====================================================

PRINT ''
PRINT '========================================='
PRINT 'CONFIGURACIÓN ACTUAL'
PRINT '========================================='
PRINT ''

SELECT 
    'Proveedor PAC' AS Concepto,
    ProveedorPAC AS Valor,
    CASE WHEN EsProduccion = 1 THEN '⚠️ PRODUCCIÓN' ELSE '🧪 SANDBOX (Pruebas)' END AS Modo
FROM ConfiguracionPAC
WHERE ConfigID = 1

UNION ALL

SELECT 
    'Usuario' AS Concepto,
    Usuario AS Valor,
    '' AS Modo
FROM ConfiguracionPAC
WHERE ConfigID = 1

UNION ALL

SELECT 
    'URL Timbrado' AS Concepto,
    UrlTimbrado AS Valor,
    '' AS Modo
FROM ConfiguracionPAC
WHERE ConfigID = 1

PRINT ''
PRINT '========================================='
PRINT 'PLANES FACTURAMA'
PRINT '========================================='
PRINT ''
PRINT 'Plan FREE (Recomendado para empezar):'
PRINT '  ✓ 50 facturas gratis cada mes'
PRINT '  ✓ $0 pesos de mensualidad'
PRINT '  ✓ Perfecto para probar y negocios pequeños'
PRINT ''
PRINT 'Plan Básico:'
PRINT '  ✓ 200 timbres → $140 MXN ($0.70 c/u)'
PRINT '  ✓ Timbres NO caducan'
PRINT '  ✓ Sin mensualidad'
PRINT ''
PRINT 'Plan Profesional:'
PRINT '  ✓ 1000 timbres → $800 MXN ($0.80 c/u)'
PRINT '  ✓ Timbres NO caducan'
PRINT '  ✓ Sin mensualidad'
PRINT ''
PRINT 'Más información: https://www.facturama.mx/planes'
PRINT ''

-- =====================================================
-- SIGUIENTES PASOS
-- =====================================================

PRINT ''
PRINT '========================================='
PRINT '✓ CONFIGURACIÓN COMPLETADA'
PRINT '========================================='
PRINT ''
PRINT 'PRÓXIMOS PASOS:'
PRINT '----------------'
PRINT '1. ✓ Facturama configurado en modo SANDBOX'
PRINT '2. Ir al POS: http://localhost:50772/VentaPOS'
PRINT '3. Hacer una venta y marcar "Requiere Factura"'
PRINT '4. Generar factura de prueba'
PRINT ''
PRINT 'PARA PRODUCCIÓN:'
PRINT '----------------'
PRINT '1. Registrarte en: https://www.facturama.mx/registro'
PRINT '2. Obtener credenciales (usuario y contraseña)'
PRINT '3. Cargar tu certificado del SAT'
PRINT '4. Actualizar este script con tus datos y ejecutar'
PRINT ''
PRINT '🎉 Listo para facturar con Facturama!'
PRINT ''
GO
