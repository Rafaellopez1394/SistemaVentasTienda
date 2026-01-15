-- =====================================================
-- Script: Configurar PADE/Prodigia con certificado subido
-- Fecha: 2026-01-14
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CONFIGURANDO PADE/PRODIGIA'
PRINT '========================================='
PRINT ''

-- ✅ CONFIGURACIÓN CON TUS DATOS REALES DE PADE
-- Datos obtenidos de tu cuenta en https://pruebas.pade.mx/

DECLARE @Usuario VARCHAR(100) = 'rafaellopez941113@gmail.com'           -- Tu usuario de pruebas.pade.mx
DECLARE @Password VARCHAR(200) = 'Rl19941113@'                            -- Tu contraseña
DECLARE @Contrato VARCHAR(100) = 'f33e2e53-3bcd-49d5-a7c6-5f5cd4dd8c18' -- Tu contrato (UUID)

-- RFC DEL CERTIFICADO: Coincide con el certificado "sistemafacturacion" que subiste a PADE
-- Este RFC debe estar en la Lista de Contribuyentes Obligados del SAT

DECLARE @RfcEmisor VARCHAR(13) = 'GAMA6111156JA'                 -- RFC del certificado que subiste
DECLARE @NombreEmisor VARCHAR(300) = 'ALMA ROSA GAXIOLA MONTOYA' -- Razón social
DECLARE @RegimenFiscal VARCHAR(3) = '612'                        -- 612 = Personas Físicas con Actividades Empresariales
DECLARE @CodigoPostal VARCHAR(5) = '81048'                       -- CP de expedición

-- =====================================================
-- No modifiques nada después de esta línea
-- =====================================================

-- Eliminar configuración anterior si existe
IF EXISTS (SELECT 1 FROM ConfiguracionProdigia WHERE ConfiguracionID = 1)
BEGIN
    DELETE FROM ConfiguracionProdigia WHERE ConfiguracionID = 1
    PRINT '✓ Configuración anterior eliminada'
END

-- Habilitar inserción explícita de IDENTITY
SET IDENTITY_INSERT ConfiguracionProdigia ON

-- Insertar nueva configuración
INSERT INTO ConfiguracionProdigia (
    ConfiguracionID,
    Usuario,
    Password,
    Contrato,
    Ambiente,
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    CodigoPostal,
    CertificadoBase64,
    LlavePrivadaBase64,
    PasswordLlave,
    Activo,
    FechaCreacion,
    FechaModificacion
)
VALUES (
    1,
    @Usuario,
    @Password,
    @Contrato,
    'TEST',                 -- Ambiente de pruebas
    @RfcEmisor,
    @NombreEmisor,
    @RegimenFiscal,
    @CodigoPostal,
    NULL,                   -- No guardamos certificados en BD
    NULL,                   -- Usamos CERT_DEFAULT de PADE
    NULL,
    1,                      -- Activo
    GETDATE(),
    GETDATE()
)

-- Deshabilitar inserción explícita de IDENTITY
SET IDENTITY_INSERT ConfiguracionProdigia OFF

PRINT ''
PRINT '✅ Configuración PADE insertada correctamente'
PRINT ''
PRINT '========================================='
PRINT 'CONFIGURACIÓN APLICADA'
PRINT '========================================='

SELECT 
    ConfiguracionID,
    Usuario AS [Usuario PADE],
    '****' + RIGHT(Password, 3) AS [Password],
    Contrato,
    Ambiente,
    RfcEmisor AS [RFC Emisor],
    NombreEmisor AS [Nombre Emisor],
    RegimenFiscal,
    CodigoPostal,
    CASE WHEN Activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado
FROM ConfiguracionProdigia
WHERE ConfiguracionID = 1

PRINT ''
PRINT '========================================='
PRINT 'SIGUIENTE PASO'
PRINT '========================================='
PRINT ''
PRINT '1. Edita este script y actualiza:'
PRINT '   - @Usuario (tu usuario de pruebas.pade.mx)'
PRINT '   - @Password (tu contraseña)'
PRINT '   - @Contrato (tu código de contrato)'
PRINT '   - @RfcEmisor (RFC del certificado que subiste)'
PRINT ''
PRINT '2. Ejecuta este script de nuevo'
PRINT ''
PRINT '3. Verifica que el RFC coincida con el certificado'
PRINT '   subido en https://pruebas.pade.mx/'
PRINT ''
PRINT '4. Intenta generar una factura de prueba'
PRINT ''

GO
