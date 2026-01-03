-- ========================================================
-- SCRIPT: Configurar Simulador PAC para Pruebas
-- ========================================================
-- Este script activa el simulador de PAC para pruebas
-- NO genera facturas reales ante el SAT
-- ========================================================

USE DB_TIENDA
GO

PRINT '========================================================';
PRINT 'ACTIVANDO SIMULADOR PAC PARA PRUEBAS';
PRINT '========================================================';
PRINT '';

-- Desactivar cualquier PAC real
UPDATE ConfiguracionPAC SET Activo = 0;

-- Insertar o actualizar configuración del simulador
IF EXISTS (SELECT 1 FROM ConfiguracionPAC WHERE ProveedorPAC = 'Simulador')
BEGIN
    UPDATE ConfiguracionPAC
    SET 
        EsProduccion = 0,
        UrlTimbrado = 'http://localhost/simulador',
        UrlCancelacion = 'http://localhost/simulador',
        UrlConsulta = 'http://localhost/simulador',
        Usuario = 'simulador',
        Password = 'simulador',
        RutaCertificado = NULL,
        RutaLlavePrivada = NULL,
        PasswordLlave = NULL,
        TimeoutSegundos = 30,
        Activo = 1
    WHERE ProveedorPAC = 'Simulador';
    
    PRINT '✅ Configuración del Simulador actualizada';
END
ELSE
BEGIN
    INSERT INTO ConfiguracionPAC (
        ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta,
        Usuario, Password, TimeoutSegundos, Activo
    )
    VALUES (
        'Simulador', 0, 'http://localhost/simulador', 'http://localhost/simulador', 
        'http://localhost/simulador', 'simulador', 'simulador', 30, 1
    );
    
    PRINT '✅ Configuración del Simulador creada';
END

PRINT '';
PRINT 'Configuración actual:';
SELECT 
    ConfigID,
    ProveedorPAC,
    CASE EsProduccion WHEN 0 THEN 'SIMULADOR' ELSE 'PRODUCCION' END AS Ambiente,
    Usuario,
    CASE Activo WHEN 1 THEN '✅ ACTIVO' ELSE '❌ INACTIVO' END AS Estado
FROM ConfiguracionPAC
ORDER BY Activo DESC, ConfigID;

PRINT '';
PRINT '========================================================';
PRINT '✅ SIMULADOR ACTIVADO';
PRINT '========================================================';
PRINT 'CARACTERÍSTICAS:';
PRINT '  • NO genera facturas reales ante el SAT';
PRINT '  • Genera UUIDs válidos en formato pero simulados';
PRINT '  • Útil para pruebas y desarrollo';
PRINT '  • No requiere conexión a internet';
PRINT '  • No consume timbres reales';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '  1. Recompilar el proyecto';
PRINT '  2. Generar una factura de prueba';
PRINT '  3. Verificar que aparezca en el módulo de facturas';
PRINT '';
PRINT 'PARA CAMBIAR A FACTURAMA REAL:';
PRINT '  • Ejecutar: 036_ACTUALIZAR_CREDENCIALES_FACTURAMA.sql';
PRINT '  • Con tus credenciales reales de Facturama';
PRINT '========================================================';
GO
