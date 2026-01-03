-- =====================================================
-- Script: 026_CONFIGURACION_FACTURACION_PRODUCCION.sql
-- Descripción: Configura el sistema para facturación
--              en ambiente de producción
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURACIÓN PARA FACTURACIÓN'
PRINT '========================================='
PRINT ''

-- =====================================================
-- PASO 1: ACTUALIZAR DATOS FISCALES DE LA EMPRESA
-- =====================================================
PRINT '1. Actualizando datos fiscales de la empresa...'
PRINT ''

-- IMPORTANTE: Reemplaza estos valores con los datos REALES de tu empresa
DECLARE @RFC_EMPRESA VARCHAR(13) = 'ABC123456XYZ'  -- ← CAMBIAR POR TU RFC REAL
DECLARE @RAZON_SOCIAL VARCHAR(300) = 'EMPRESA EJEMPLO S.A. DE C.V.'  -- ← CAMBIAR POR TU RAZÓN SOCIAL
DECLARE @REGIMEN_FISCAL VARCHAR(5) = '601'  -- ← CAMBIAR POR TU RÉGIMEN (601=General, 603=P.Moral, 612=P.Física)
DECLARE @CODIGO_POSTAL VARCHAR(5) = '00000'  -- ← CAMBIAR POR TU CP FISCAL

UPDATE ConfiguracionGeneral
SET RFC = @RFC_EMPRESA,
    NombreNegocio = @RAZON_SOCIAL,
    -- Si existen estos campos en tu tabla, descomenta:
    -- RegimenFiscal = @REGIMEN_FISCAL,
    -- CodigoPostal = @CODIGO_POSTAL,
    FechaModificacion = GETDATE()
WHERE ConfigID = 1

IF @@ROWCOUNT > 0
    PRINT '✓ Datos fiscales actualizados'
ELSE
    PRINT '⚠ No se pudo actualizar ConfiguracionGeneral'

PRINT ''

-- =====================================================
-- PASO 2: CONFIGURAR PAC (MODO PRUEBAS INICIALMENTE)
-- =====================================================
PRINT '2. Verificando configuración del PAC...'
PRINT ''

IF EXISTS (SELECT 1 FROM ConfiguracionPAC WHERE ConfigID = 1)
BEGIN
    -- Mostrar configuración actual
    SELECT 
        'Configuración Actual del PAC:' AS Info,
        ProveedorPAC AS Proveedor,
        CASE WHEN EsProduccion = 1 THEN 'PRODUCCIÓN' ELSE 'PRUEBAS' END AS Ambiente,
        Usuario,
        UrlTimbrado
    FROM ConfiguracionPAC
    WHERE ConfigID = 1
    
    PRINT '⚠ IMPORTANTE: Actualmente en modo PRUEBAS'
    PRINT '  Para facturación REAL necesitas:'
    PRINT '  1. Contratar servicio con Finkok u otro PAC'
    PRINT '  2. Obtener credenciales de PRODUCCIÓN'
    PRINT '  3. Ejecutar el script de actualización a producción'
    PRINT ''
END
ELSE
BEGIN
    PRINT '✗ No existe configuración PAC'
    PRINT '  Ejecuta primero el script 011_TABLAS_CONFIGURACION.sql'
END

PRINT ''

-- =====================================================
-- PASO 3: VERIFICAR CERTIFICADOS DIGITALES
-- =====================================================
PRINT '3. Verificando certificados digitales...'
PRINT ''

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CertificadosDigitales')
BEGIN
    DECLARE @CantidadCerts INT
    DECLARE @CertsVigentes INT
    
    SELECT @CantidadCerts = COUNT(*) FROM CertificadosDigitales WHERE Activo = 1
    SELECT @CertsVigentes = COUNT(*) FROM CertificadosDigitales WHERE Activo = 1 AND FechaVigenciaFin > GETDATE()
    
    IF @CantidadCerts > 0
    BEGIN
        PRINT '✓ Hay ' + CAST(@CantidadCerts AS VARCHAR) + ' certificado(s) activo(s)'
        PRINT '✓ ' + CAST(@CertsVigentes AS VARCHAR) + ' certificado(s) vigente(s)'
        
        -- Mostrar certificados
        SELECT 
            NombreCertificado AS Nombre,
            RFC,
            NoCertificado,
            FechaVigenciaFin AS VenceEl,
            DATEDIFF(DAY, GETDATE(), FechaVigenciaFin) AS DiasRestantes,
            CASE WHEN EsPredeterminado = 1 THEN 'SÍ' ELSE 'NO' END AS Predeterminado
        FROM CertificadosDigitales
        WHERE Activo = 1
        ORDER BY FechaVigenciaFin
    END
    ELSE
    BEGIN
        PRINT '⚠ NO hay certificados cargados'
        PRINT '  Necesitas:'
        PRINT '  1. Obtener CSD del portal del SAT'
        PRINT '  2. Cargarlos desde el módulo "Certificados Digitales"'
    END
END
ELSE
BEGIN
    PRINT '✗ Tabla CertificadosDigitales NO existe'
    PRINT '  Ejecuta el script: 025_CREAR_TABLA_CERTIFICADOS_DIGITALES.sql'
END

PRINT ''

-- =====================================================
-- PASO 4: VERIFICAR ESTRUCTURA DE FACTURACIÓN
-- =====================================================
PRINT '4. Verificando tablas de facturación...'
PRINT ''

DECLARE @TablasRequeridas TABLE (Nombre VARCHAR(100))
INSERT INTO @TablasRequeridas VALUES 
    ('Facturas'),
    ('FacturasDetalle'),
    ('FacturasImpuestos'),
    ('FacturasCancelacion'),
    ('ConfiguracionPAC'),
    ('CertificadosDigitales')

SELECT 
    t.Nombre AS Tabla,
    CASE WHEN s.name IS NOT NULL THEN '✓ Existe' ELSE '✗ NO EXISTE' END AS Estado
FROM @TablasRequeridas t
LEFT JOIN sys.tables s ON t.Nombre = s.name
ORDER BY t.Nombre

PRINT ''

-- =====================================================
-- PASO 5: RESUMEN Y PRÓXIMOS PASOS
-- =====================================================
PRINT '========================================='
PRINT 'RESUMEN DE CONFIGURACIÓN'
PRINT '========================================='
PRINT ''

-- Verificar estado general
DECLARE @RFCActual VARCHAR(13)
DECLARE @TieneCertificados BIT = 0
DECLARE @EsProduccion BIT = 0

SELECT @RFCActual = RFC FROM ConfiguracionGeneral WHERE ConfigID = 1

IF EXISTS (SELECT 1 FROM CertificadosDigitales WHERE Activo = 1 AND FechaVigenciaFin > GETDATE())
    SET @TieneCertificados = 1

IF EXISTS (SELECT 1 FROM ConfiguracionPAC WHERE EsProduccion = 1)
    SET @EsProduccion = 1

-- Mostrar estado
PRINT 'ESTADO ACTUAL:'
PRINT '- RFC Configurado: ' + ISNULL(@RFCActual, 'NO CONFIGURADO')
PRINT '- Certificados: ' + CASE WHEN @TieneCertificados = 1 THEN 'SÍ (vigentes)' ELSE 'NO' END
PRINT '- PAC: ' + CASE WHEN @EsProduccion = 1 THEN 'PRODUCCIÓN' ELSE 'PRUEBAS' END
PRINT ''

-- Calcular porcentaje de completitud
DECLARE @Completitud INT = 0
IF @RFCActual IS NOT NULL AND @RFCActual != 'XAXX010101000' SET @Completitud = @Completitud + 33
IF @TieneCertificados = 1 SET @Completitud = @Completitud + 33
IF @EsProduccion = 1 SET @Completitud = @Completitud + 34

PRINT 'COMPLETITUD: ' + CAST(@Completitud AS VARCHAR) + '%'
PRINT ''

IF @Completitud = 100
BEGIN
    PRINT '✓✓✓ ¡SISTEMA LISTO PARA FACTURAR! ✓✓✓'
    PRINT 'Puedes generar facturas válidas ante el SAT'
END
ELSE
BEGIN
    PRINT '⚠ SISTEMA NO LISTO PARA FACTURAR'
    PRINT 'Faltan los siguientes pasos:'
    PRINT ''
    
    IF @RFCActual IS NULL OR @RFCActual = 'XAXX010101000'
    BEGIN
        PRINT '1. [ ] ACTUALIZAR RFC REAL'
        PRINT '   - Edita este script (línea 20)'
        PRINT '   - Coloca el RFC real de tu empresa'
        PRINT '   - Vuelve a ejecutar el script'
        PRINT ''
    END
    
    IF @TieneCertificados = 0
    BEGIN
        PRINT '2. [ ] CARGAR CERTIFICADOS CSD'
        PRINT '   - Obtén los certificados del SAT'
        PRINT '   - Ve al módulo: Configuración > Certificados Digitales'
        PRINT '   - Carga los archivos .cer y .key'
        PRINT ''
    END
    
    IF @EsProduccion = 0
    BEGIN
        PRINT '3. [ ] CONFIGURAR PAC EN PRODUCCIÓN'
        PRINT '   - Contrata servicio de timbrado (Finkok u otro)'
        PRINT '   - Actualiza credenciales de producción'
        PRINT '   - Ejecuta: UPDATE ConfiguracionPAC SET EsProduccion = 1...'
        PRINT ''
    END
END

PRINT ''
PRINT '========================================='
PRINT 'SCRIPT COMPLETADO'
PRINT '========================================='
GO

/*
===============================================
NOTAS IMPORTANTES:
===============================================

1. RFC GENÉRICO (XAXX010101000):
   - NO es válido para facturación real
   - Solo para pruebas
   - DEBE cambiarse por el RFC de tu empresa

2. CERTIFICADOS CSD:
   - Se obtienen del portal del SAT
   - Duran 4 años
   - Son GRATUITOS
   - Proceso: SAT > Trámites > Certificado CSD

3. PAC (Proveedor Autorizado de Certificación):
   - Necesario para timbrar facturas
   - Opciones: Finkok, Padeimex, Diafco, etc.
   - Costo aproximado: $1.50-$2.00 por factura
   - Contratar en: https://www.finkok.com

4. AMBIENTE DE PRUEBAS:
   - Útil para desarrollo
   - Las facturas NO son válidas ante SAT
   - No se pueden deducir impuestos
   - Cambiar a producción cuando tengas:
     * RFC real
     * Certificados reales
     * Credenciales PAC de producción

===============================================
*/
