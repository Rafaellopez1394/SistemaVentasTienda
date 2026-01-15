-- =====================================================
-- VERIFICACIÓN COMPLETA DE FACTURACIÓN
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA;
GO

PRINT '========================================';
PRINT 'VERIFICACIÓN DEL SISTEMA DE FACTURACIÓN';
PRINT '========================================';
PRINT '';

-- 1. Verificar ConfiguracionEmpresa (Emisor)
PRINT '1. CONFIGURACIÓN DEL EMISOR:';
PRINT '-----------------------------';
IF EXISTS (SELECT * FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1)
BEGIN
    SELECT 
        '✓ Configuración encontrada' AS Estado,
        RFC,
        RazonSocial,
        NombreComercial,
        RegimenFiscal,
        CodigoPostal AS CP,
        Estado,
        Email
    FROM ConfiguracionEmpresa
    WHERE ConfigEmpresaID = 1;
    
    -- Validar campos críticos
    DECLARE @RFC VARCHAR(13), @RazonSocial VARCHAR(255), @RegimenFiscal VARCHAR(10), @CP VARCHAR(10);
    SELECT 
        @RFC = RFC,
        @RazonSocial = RazonSocial,
        @RegimenFiscal = RegimenFiscal,
        @CP = CodigoPostal
    FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1;
    
    IF @RFC IS NULL OR @RFC = ''
        PRINT '❌ ERROR: RFC del emisor está vacío';
    ELSE
        PRINT '✓ RFC configurado correctamente';
    
    IF @RazonSocial IS NULL OR @RazonSocial = ''
        PRINT '❌ ERROR: Razón Social está vacía';
    ELSE
        PRINT '✓ Razón Social configurada';
    
    IF @RegimenFiscal IS NULL OR @RegimenFiscal = ''
        PRINT '❌ ERROR: Régimen Fiscal está vacío';
    ELSE
        PRINT '✓ Régimen Fiscal configurado';
    
    IF @CP IS NULL OR @CP = '' OR @CP = '00000'
        PRINT '⚠️ ADVERTENCIA: Código Postal no está configurado o es genérico';
    ELSE
        PRINT '✓ Código Postal configurado';
END
ELSE
BEGIN
    PRINT '❌ ERROR CRÍTICO: No existe configuración de empresa';
    PRINT '   Ejecuta: CREAR_TABLA_EMISOR.sql';
END
PRINT '';

-- 2. Verificar ConfiguracionPAC
PRINT '2. CONFIGURACIÓN DEL PAC:';
PRINT '--------------------------';
IF EXISTS (SELECT * FROM ConfiguracionPAC WHERE Activo = 1)
BEGIN
    SELECT 
        '✓ PAC Activo' AS Estado,
        ProveedorPAC,
        CASE WHEN EsProduccion = 1 THEN 'PRODUCCIÓN ⚠️' ELSE 'PRUEBAS ✓' END AS Ambiente,
        LEFT(Usuario, 30) + '...' AS 'API Key',
        CASE WHEN LEN(Password) > 0 THEN 'Configurado' ELSE 'Vacío' END AS Tenant
    FROM ConfiguracionPAC
    WHERE Activo = 1;
    
    -- Validar que sea FiscalAPI
    DECLARE @Proveedor VARCHAR(50);
    SELECT @Proveedor = ProveedorPAC FROM ConfiguracionPAC WHERE Activo = 1;
    
    IF @Proveedor = 'FiscalAPI'
        PRINT '✓ Proveedor PAC: FiscalAPI (correcto)';
    ELSE
        PRINT '❌ ERROR: Proveedor PAC no es FiscalAPI: ' + @Proveedor;
END
ELSE
BEGIN
    PRINT '❌ ERROR CRÍTICO: No hay configuración de PAC activa';
    PRINT '   Ejecuta: configurar_fiscalapi.ps1';
END
PRINT '';

-- 3. Verificar estructura de tablas de facturación
PRINT '3. ESTRUCTURA DE TABLAS:';
PRINT '-------------------------';

DECLARE @TablaFacturas BIT = 0, @TablaFacturasDetalle BIT = 0;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    PRINT '✓ Tabla Facturas existe';
    SET @TablaFacturas = 1;
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla Facturas no existe';
    PRINT '   Ejecuta: 007_CREAR_MODULO_FACTURACION.sql';
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FacturasDetalle')
BEGIN
    PRINT '✓ Tabla FacturasDetalle existe';
    SET @TablaFacturasDetalle = 1;
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla FacturasDetalle no existe';
END
PRINT '';

-- 4. Verificar procedimiento de folio
PRINT '4. PROCEDIMIENTO DE FOLIOS:';
PRINT '----------------------------';
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'GenerarFolioFactura')
    PRINT '✓ Procedimiento GenerarFolioFactura existe';
ELSE
BEGIN
    PRINT '❌ ERROR: Procedimiento GenerarFolioFactura no existe';
    PRINT '   Debe crearse manualmente o ejecutar script de facturación';
END
PRINT '';

-- 5. Verificar ventas disponibles para facturar
PRINT '5. VENTAS DISPONIBLES:';
PRINT '----------------------';
IF EXISTS (SELECT * FROM VentasClientes)
BEGIN
    DECLARE @TotalVentas INT, @VentasSinFacturar INT;
    
    SELECT @TotalVentas = COUNT(*) FROM VentasClientes;
    SELECT @VentasSinFacturar = COUNT(*) 
    FROM VentasClientes v
    WHERE NOT EXISTS (SELECT 1 FROM Facturas f WHERE f.VentaID = v.VentaID);
    
    PRINT '✓ Total de ventas: ' + CAST(@TotalVentas AS VARCHAR);
    PRINT '  Ventas sin facturar: ' + CAST(@VentasSinFacturar AS VARCHAR);
    
    IF @VentasSinFacturar > 0
        PRINT '✓ Hay ventas disponibles para facturar';
    ELSE
        PRINT '  Todas las ventas ya tienen factura';
END
ELSE
    PRINT '⚠️ No hay ventas registradas en el sistema';
PRINT '';

-- 6. Verificar últimas facturas generadas
PRINT '6. ÚLTIMAS FACTURAS:';
PRINT '--------------------';
IF @TablaFacturas = 1 AND EXISTS (SELECT * FROM Facturas)
BEGIN
    SELECT TOP 5
        Serie + '-' + Folio AS 'Factura',
        CONVERT(VARCHAR, FechaEmision, 103) AS 'Fecha',
        ReceptorNombre AS 'Cliente',
        FORMAT(Total, 'C', 'es-MX') AS 'Total',
        Estatus,
        CASE WHEN UUID IS NOT NULL THEN '✓ Timbrada' ELSE '○ Sin timbrar' END AS 'Estado'
    FROM Facturas
    ORDER BY FechaCreacion DESC;
END
ELSE
    PRINT '  No hay facturas generadas aún';
PRINT '';

-- 7. Verificar certificados en FiscalAPI
PRINT '7. CERTIFICADOS CSD:';
PRINT '--------------------';
PRINT '⚠️ IMPORTANTE: Verifica manualmente en FiscalAPI';
PRINT '   URL: https://test.fiscalapi.com/tax-files';
PRINT '';

SELECT @RFC = RFC FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1;
PRINT '   Debe haber certificados subidos para el RFC: ' + ISNULL(@RFC, 'NO CONFIGURADO');
PRINT '   - Archivo .cer (certificado público)';
PRINT '   - Archivo .key (llave privada)';
PRINT '   - Ambos deben estar vigentes y activos';
PRINT '';

-- 8. Resumen final
PRINT '========================================';
PRINT 'RESUMEN:';
PRINT '========================================';

DECLARE @Errores INT = 0, @Advertencias INT = 0;

-- Contar errores críticos
IF NOT EXISTS (SELECT * FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1)
    SET @Errores = @Errores + 1;

IF NOT EXISTS (SELECT * FROM ConfiguracionPAC WHERE Activo = 1)
    SET @Errores = @Errores + 1;

IF @TablaFacturas = 0
    SET @Errores = @Errores + 1;

-- Contar advertencias
SELECT @RFC = RFC FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1;
IF @RFC IS NULL OR @RFC = '' OR @RFC = 'XAXX010101000'
    SET @Advertencias = @Advertencias + 1;

IF @Errores = 0 AND @Advertencias = 0
BEGIN
    PRINT '✓ SISTEMA LISTO PARA FACTURAR';
    PRINT '';
    PRINT 'Pasos siguientes:';
    PRINT '1. Verifica que los certificados CSD estén en FiscalAPI';
    PRINT '2. Reinicia la aplicación web';
    PRINT '3. Realiza una venta de prueba';
    PRINT '4. Genera la factura desde el sistema';
END
ELSE
BEGIN
    PRINT '❌ SE ENCONTRARON PROBLEMAS:';
    PRINT '   - Errores críticos: ' + CAST(@Errores AS VARCHAR);
    PRINT '   - Advertencias: ' + CAST(@Advertencias AS VARCHAR);
    PRINT '';
    PRINT 'Revisa los mensajes arriba y corrige los errores antes de facturar.';
END
PRINT '';
