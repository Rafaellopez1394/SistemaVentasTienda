-- =====================================================
-- Script: Configuración de PADE (Prodigia) como PAC
-- Descripción: Configura el sistema para usar PADE como proveedor de timbrado
-- Ambiente de Pruebas: https://pruebas.pade.mx
-- Documentación API: https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest
-- Fecha: 2026-01-14
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURACIÓN DE PADE (PRODIGIA PAC)'
PRINT '========================================='
PRINT ''

-- =====================================================
-- PASO 1: Crear tabla si no existe
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionProdigia')
BEGIN
    CREATE TABLE ConfiguracionProdigia (
        ConfiguracionID INT PRIMARY KEY IDENTITY(1,1),
        Usuario NVARCHAR(100) NOT NULL,
        Password NVARCHAR(200) NOT NULL,
        Contrato NVARCHAR(100) NOT NULL,
        Ambiente NVARCHAR(20) NOT NULL DEFAULT 'TEST', -- TEST o PRODUCCION
        RfcEmisor NVARCHAR(13) NOT NULL,
        NombreEmisor NVARCHAR(300) NOT NULL,
        RegimenFiscal NVARCHAR(10) NOT NULL,
        CertificadoBase64 NVARCHAR(MAX) NULL, -- Opcional si se usa CERT_DEFAULT
        LlavePrivadaBase64 NVARCHAR(MAX) NULL, -- Opcional si se usa CERT_DEFAULT
        PasswordLlave NVARCHAR(100) NULL,
        CodigoPostal NVARCHAR(10) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    )
    
    PRINT '✅ Tabla ConfiguracionProdigia creada exitosamente'
END
ELSE
BEGIN
    PRINT 'ℹ️ La tabla ConfiguracionProdigia ya existe'
END
GO

-- =====================================================
-- PASO 2: Desactivar FiscalAPI si existe
-- =====================================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionFiscalAPI')
BEGIN
    UPDATE ConfiguracionFiscalAPI SET Activo = 0
    PRINT '✅ FiscalAPI desactivado'
END

-- =====================================================
-- PASO 3: Configuración de prueba PADE
-- =====================================================
-- Eliminar configuraciones anteriores si existen
DELETE FROM ConfiguracionProdigia WHERE ConfiguracionID = 1

-- Insertar configuración de prueba PADE
INSERT INTO ConfiguracionProdigia (
    Usuario,
    Password,
    Contrato,
    Ambiente,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CodigoPostal,
    Activo
)
VALUES (
    'TU_USUARIO_WEBSERVICE', -- ⚠️ REEMPLAZAR con tu usuario de PADE
    'TU_PASSWORD_WEBSERVICE', -- ⚠️ REEMPLAZAR con tu contraseña de PADE
    'TU_CONTRATO', -- ⚠️ REEMPLAZAR con tu código de contrato
    'TEST', -- Ambiente de pruebas
    'AAA010101AAA', -- ⚠️ REEMPLAZAR con el RFC de tu empresa
    'MI EMPRESA SA DE CV', -- ⚠️ REEMPLAZAR con tu razón social
    '601', -- Régimen Fiscal (601=General de Ley Personas Morales)
    '01000', -- ⚠️ REEMPLAZAR con tu código postal de expedición
    1 -- Activo
)

PRINT '✅ Configuración de prueba PADE insertada'
PRINT ''

-- =====================================================
-- PASO 4: Verificar configuración
-- =====================================================
PRINT '========================================='
PRINT 'CONFIGURACIÓN ACTUAL'
PRINT '========================================='
PRINT ''

SELECT 
    ConfiguracionID,
    Usuario,
    REPLICATE('*', LEN(Password)) AS [Password (oculto)],
    Contrato,
    Ambiente AS [Ambiente (TEST/PRODUCCION)],
    CASE 
        WHEN Ambiente = 'TEST' THEN 'https://pruebas.pade.mx/'
        ELSE 'https://timbrado.pade.mx/'
    END AS [URL API],
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CodigoPostal,
    CASE 
        WHEN CertificadoBase64 IS NULL THEN '❌ NULL (usar CERT_DEFAULT de PADE)'
        ELSE '✅ Configurado (' + CAST(LEN(CertificadoBase64) AS VARCHAR) + ' caracteres)'
    END AS [Certificado CSD],
    CASE 
        WHEN LlavePrivadaBase64 IS NULL THEN '❌ NULL (usar CERT_DEFAULT de PADE)'
        ELSE '✅ Configurado (' + CAST(LEN(LlavePrivadaBase64) AS VARCHAR) + ' caracteres)'
    END AS [Llave Privada],
    CASE WHEN Activo = 1 THEN '✅ Activo' ELSE '❌ Inactivo' END AS Estado,
    FechaCreacion
FROM ConfiguracionProdigia
WHERE ConfiguracionID = 1
GO

PRINT ''
PRINT '========================================='
PRINT 'PASOS PARA CONFIGURAR PADE'
PRINT '========================================='
PRINT ''
PRINT '1️⃣ OBTENER CREDENCIALES DE PRUEBA PADE:'
PRINT '   - Ambiente de pruebas: https://pruebas.pade.mx'
PRINT '   - Contacto: soporte@pade.mx'
PRINT '   - Solicitar credenciales de WEBSERVICE de PRUEBAS'
PRINT '   - Recibirás por email:'
PRINT '     • Usuario de webservice'
PRINT '     • Contraseña de webservice'
PRINT '     • Código de contrato'
PRINT ''
PRINT '2️⃣ SUBIR CERTIFICADOS CSD AL PORTAL DE PRUEBAS:'
PRINT '   - Ingresar al portal de pruebas: https://pruebas.pade.mx'
PRINT '   - Ir a: Configuración → Certificados'
PRINT '   - Subir tu archivo .CER (certificado)'
PRINT '   - Subir tu archivo .KEY (llave privada)'
PRINT '   - Ingresar la contraseña de la llave'
PRINT '   - Una vez subidos, PADE los usará automáticamente (CERT_DEFAULT)'
PRINT ''
PRINT '3️⃣ ACTUALIZAR CREDENCIALES EN ESTE SCRIPT:'
PRINT ''
PRINT '   UPDATE ConfiguracionProdigia'
PRINT '   SET '
PRINT '       Usuario = ''TU_USUARIO_REAL'','
PRINT '       Password = ''TU_PASSWORD_REAL'','
PRINT '       Contrato = ''TU_CONTRATO_REAL'','
PRINT '       RfcEmisor = ''ABC123456XYZ'',  -- Tu RFC real'
PRINT '       NombreEmisor = ''TU EMPRESA SA DE CV'','
PRINT '       CodigoPostal = ''12345'',  -- Tu CP real'
PRINT '       Ambiente = ''TEST'',  -- ⚠️ DEJAR EN TEST por ahora'
PRINT '       FechaModificacion = GETDATE()'
PRINT '   WHERE ConfiguracionID = 1'
PRINT ''
PRINT '4️⃣ OBTENER CERTIFICADOS CSD DEL SAT (Si no los tienes):'
PRINT '   - Ingresar al portal del SAT: https://www.sat.gob.mx'
PRINT '   - Con tu e.firma (FIEL)'
PRINT '   - Ir a: Trámites → Certificado de Sello Digital (CSD)'
PRINT '   - Descargar: .CER y .KEY'
PRINT '   - Son GRATUITOS y duran 4 años'
PRINT ''
PRINT '5️⃣ PROBAR TIMBRADO:'
PRINT '   - Una vez configurado, ir al módulo de Facturación'
PRINT '   - Crear una factura de prueba'
PRINT '   - El sistema usará PADE automáticamente'
PRINT ''
PRINT '========================================='
PRINT 'INFORMACIÓN IMPORTANTE'
PRINT '========================================='
PRINT ''
PRINT '📌 URL Ambiente TEST (Pruebas):'
PRINT '   https://pruebas.pade.mx/'
PRINT '   ⚠️ ACTUALMENTE USANDO ESTE AMBIENTE'
PRINT ''
PRINT '📌 URL Ambiente PRODUCCION (Real):'
PRINT '   https://timbrado.pade.mx/'
PRINT '   (No se usará por el momento)'
PRINT ''
PRINT '📌 Documentación API REST:'
PRINT '   https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest'
PRINT ''
PRINT '📌 Soporte PADE:'
PRINT '   soporte@pade.mx'
PRINT ''
PRINT '⚠️ MODO ACTUAL: TEST (Pruebas)'
PRINT '   Cambia a PRODUCCION cuando tengas credenciales reales'
PRINT ''
PRINT '========================================='
PRINT 'CONFIGURACIÓN COMPLETADA'
PRINT '========================================='
PRINT ''
PRINT '✅ Sistema configurado para usar PADE (Prodigia)'
PRINT '⚠️ MODO: TEST (Pruebas) - https://pruebas.pade.mx'
PRINT '⚠️ RECUERDA: Actualiza las credenciales con tus datos reales de prueba'
PRINT ''

/*
===============================================
NOTAS ADICIONALES
===============================================

1. CERT_DEFAULT:
   - PADE puede almacenar tus certificados en su portal
   - Si los subes allí, no necesitas enviarlos en cada petición
   - El sistema usará automáticamente CERT_DEFAULT

2. CERTIFICADOS EN BASE64 (Alternativa):
   - Si prefieres, puedes almacenar los certificados en la BD
   - Actualiza CertificadoBase64 y LlavePrivadaBase64
   - El sistema los enviará en cada petición

3. COSTOS APROXIMADOS:
   - Contactar a PADE para cotización
   - Paquetes desde 100 timbres
   - Ambiente de pruebas generalmente gratuito

4. CAMBIAR A PRODUCCIÓN:
   - Obtén credenciales de producción de PADE
   - Actualiza Ambiente = 'PRODUCCION'
   - Actualiza Usuario y Password de producción
   - El sistema usará https://timbrado.pade.mx/ automáticamente

5. RÉGIMEN FISCAL (RegimenFiscal):
   - 601: General de Ley Personas Morales
   - 603: Personas Morales con Fines no Lucrativos
   - 605: Sueldos y Salarios e Ingresos Asimilados a Salarios
   - 606: Arrendamiento
   - 612: Personas Físicas con Actividades Empresariales
   - 621: Régimen de Incorporación Fiscal (RIF)
   - 625: Régimen de las Actividades Empresariales con ingresos
   - 626: Régimen Simplificado de Confianza

6. USO CFDI (Receptor):
   - G01: Adquisición de mercancías
   - G03: Gastos en general
   - P01: Por definir
   - Consulta catálogo completo SAT

*/
