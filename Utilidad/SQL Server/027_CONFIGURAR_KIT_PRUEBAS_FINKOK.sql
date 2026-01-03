-- =====================================================
-- Script: 027_CONFIGURAR_KIT_PRUEBAS_FINKOK.sql
-- Descripción: Configura el sistema con el Kit de Pruebas de Finkok
--              https://www.finkok.com/kit-pruebas.html
-- Base de datos: DB_TIENDA
-- Fecha: 2026-01-01
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURANDO KIT DE PRUEBAS FINKOK'
PRINT '========================================='
PRINT ''

-- =====================================================
-- PASO 1: Actualizar configuración del emisor con datos de prueba de Finkok
-- =====================================================

PRINT 'PASO 1: Configurando datos del emisor (datos de prueba)...'

UPDATE Configuracion
SET 
    RFC = 'EKU9003173C9',                -- RFC de prueba Finkok
    RazonSocial = N'ESCUELA KEMPER URGATE',
    RegimenFiscal = '601',               -- General de Ley Personas Morales
    CodigoPostal = '26015',
    Calle = N'PROLONGACIÓN MONTECARLO',
    NumeroExterior = '120',
    Colonia = N'HORNOS INSURGENTES',
    Municipio = N'PIEDRAS NEGRAS',
    Estado = 'COAHUILA',
    Pais = N'México'
WHERE ConfigID = 1

PRINT '  ✓ RFC actualizado a: EKU9003173C9 (prueba)'
PRINT '  ✓ Razón Social: ESCUELA KEMPER URGATE'
PRINT '  ✓ Régimen Fiscal: 601'
PRINT ''

-- =====================================================
-- PASO 2: Verificar/Actualizar configuración del PAC (Finkok Demo)
-- =====================================================

PRINT 'PASO 2: Verificando configuración PAC...'

IF NOT EXISTS (SELECT 1 FROM ConfiguracionPAC WHERE ConfigID = 1)
BEGIN
    -- Crear configuración si no existe
    INSERT INTO ConfiguracionPAC (ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta, Usuario, Password, Activo)
    VALUES (
        'Finkok',
        0, -- Modo demo/pruebas
        'https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl',
        'https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl',
        'https://demo-facturacion.finkok.com/servicios/soap/utilities.wsdl',
        'cfdi@facturacionmoderna.com',
        '2y4e9w8u',
        1
    )
    PRINT '  ✓ Configuración PAC creada'
END
ELSE
BEGIN
    -- Actualizar a modo demo
    UPDATE ConfiguracionPAC
    SET 
        ProveedorPAC = 'Finkok',
        EsProduccion = 0,
        UrlTimbrado = 'https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl',
        UrlCancelacion = 'https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl',
        UrlConsulta = 'https://demo-facturacion.finkok.com/servicios/soap/utilities.wsdl',
        Usuario = 'cfdi@facturacionmoderna.com',
        Password = '2y4e9w8u',
        Activo = 1
    WHERE ConfigID = 1
    
    PRINT '  ✓ Configuración PAC actualizada a modo DEMO'
END

PRINT '  ✓ Usuario demo: cfdi@facturacionmoderna.com'
PRINT '  ✓ URL: https://demo-facturacion.finkok.com'
PRINT ''

-- =====================================================
-- PASO 3: Información sobre certificados de prueba
-- =====================================================

PRINT 'PASO 3: Certificados de prueba Finkok'
PRINT '--------------------------------------'
PRINT ''
PRINT 'Los certificados de prueba de Finkok están disponibles en:'
PRINT 'https://www.finkok.com/kit-pruebas.html'
PRINT ''
PRINT 'RFC de prueba: EKU9003173C9'
PRINT 'Archivos necesarios:'
PRINT '  - EKU9003173C9.cer (certificado público)'
PRINT '  - EKU9003173C9.key (llave privada)'
PRINT '  - Contraseña de la llave: 12345678a'
PRINT ''
PRINT 'Vigencia: 2019-04-02 al 2023-04-02'
PRINT ''

-- =====================================================
-- PASO 4: Verificar tabla CertificadosDigitales existe
-- =====================================================

PRINT 'PASO 4: Verificando tabla CertificadosDigitales...'

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CertificadosDigitales')
BEGIN
    PRINT '  ⚠ ADVERTENCIA: Tabla CertificadosDigitales no existe'
    PRINT '  → Ejecutar script: 025_CREAR_TABLA_CERTIFICADOS_DIGITALES.sql'
    PRINT ''
END
ELSE
BEGIN
    PRINT '  ✓ Tabla CertificadosDigitales existe'
    
    -- Verificar si hay certificados cargados
    DECLARE @TotalCerts INT
    SELECT @TotalCerts = COUNT(*) FROM CertificadosDigitales WHERE Activo = 1
    
    IF @TotalCerts = 0
    BEGIN
        PRINT '  ⚠ No hay certificados cargados'
        PRINT '  → Cargar certificado desde la interfaz web: /CertificadoDigital'
    END
    ELSE
    BEGIN
        PRINT '  ✓ Certificados encontrados: ' + CAST(@TotalCerts AS VARCHAR)
        
        -- Mostrar certificados activos
        SELECT 
            CertificadoID,
            NombreCertificado,
            RFC,
            NoCertificado,
            CONVERT(VARCHAR, FechaVigenciaInicio, 103) AS Vigencia_Inicio,
            CONVERT(VARCHAR, FechaVigenciaFin, 103) AS Vigencia_Fin,
            CASE 
                WHEN FechaVigenciaFin < GETDATE() THEN 'VENCIDO'
                WHEN FechaVigenciaFin < DATEADD(DAY, 30, GETDATE()) THEN 'POR VENCER'
                ELSE 'VIGENTE'
            END AS Estado
        FROM CertificadosDigitales
        WHERE Activo = 1
    END
    PRINT ''
END

-- =====================================================
-- RESUMEN FINAL
-- =====================================================

PRINT ''
PRINT '========================================='
PRINT 'CONFIGURACIÓN COMPLETADA'
PRINT '========================================='
PRINT ''
PRINT '✓ Sistema configurado con Kit de Pruebas Finkok'
PRINT ''
PRINT 'SIGUIENTE PASO: Cargar certificado de prueba'
PRINT '--------------------------------------'
PRINT '1. Descargar certificados de: https://www.finkok.com/kit-pruebas.html'
PRINT '2. Ir a: http://localhost/CertificadoDigital'
PRINT '3. Cargar certificado:'
PRINT '   - Archivo .cer: EKU9003173C9.cer'
PRINT '   - Archivo .key: EKU9003173C9.key'
PRINT '   - Contraseña: 12345678a'
PRINT ''
PRINT 'DESPUÉS YA PUEDES FACTURAR (modo prueba):'
PRINT '--------------------------------------'
PRINT '1. Ir al POS'
PRINT '2. Realizar una venta'
PRINT '3. Marcar "Requiere Factura"'
PRINT '4. Capturar RFC del cliente (o usar: XAXX010101000)'
PRINT '5. Generar factura'
PRINT ''
PRINT '⚠ IMPORTANTE:'
PRINT '- Las facturas en modo DEMO son válidas para pruebas'
PRINT '- NO son válidas ante el SAT'
PRINT '- NO se pueden usar para deducir impuestos'
PRINT '- Para producción: contratar servicio en https://www.finkok.com'
PRINT ''
PRINT '========================================='
PRINT 'Script ejecutado correctamente'
PRINT '========================================='
GO
