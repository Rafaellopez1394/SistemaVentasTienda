-- =====================================================
-- CONFIGURAR DATOS DEL EMISOR PARA FACTURACION
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA;
GO

-- 1. Verificar si existe la tabla ConfiguracionEmpresa
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionEmpresa')
BEGIN
    PRINT 'ERROR: La tabla ConfiguracionEmpresa no existe. Por favor ejecuta los scripts de creación de tablas primero.';
    RETURN;
END
GO

-- 2. Verificar si ya existe un registro
IF EXISTS (SELECT * FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1)
BEGIN
    PRINT 'Ya existe configuración de empresa. Actualizando...';
    
    -- Actualizar con tus datos REALES
    UPDATE ConfiguracionEmpresa
    SET 
        RFC = 'GAMA6111156JA',              
        RazonSocial = 'ALMA ROSA GAXIOLA MONTOYA', 
        NombreComercial = 'LAS AGUILAS MERCADO DEL MAR',
        RegimenFiscal = '612',             
        CodigoPostal = '81048',           
        Calle = 'BLVD MILLAN',
        NumeroExterior = 'SIN NUMERO',
        Colonia = 'LAS PALMAS',
        Municipio = 'GUASAVE',
        Estado = 'SINALOA',
        Pais = 'MEXICO',
        Telefono = '6871313828',
        Email = 'lasaguilasmercadodelmar@gmail.com',
        UsuarioModificacion = 'ADMIN',
        FechaModificacion = GETDATE()
    WHERE ConfigEmpresaID = 1;
    
    PRINT '✓ Configuración actualizada correctamente.';
END
ELSE
BEGIN
    PRINT 'No existe configuración. Insertando nueva...';
    
    -- Insertar nuevos datos
    INSERT INTO ConfiguracionEmpresa (
        ConfigEmpresaID,
        RFC,
        RazonSocial,
        NombreComercial,
        RegimenFiscal,
        CodigoPostal,
        Calle,
        NumeroExterior,
        NumeroInterior,
        Colonia,
        Municipio,
        Estado,
        Pais,
        Telefono,
        Email,
        SitioWeb,
        UsuarioCreacion,
        FechaCreacion
    )
    VALUES (
        1,
        'GAMA6111156JA',
        'ALMA ROSA GAXIOLA MONTOYA',
        'LAS AGUILAS MERCADO DEL MAR',
        '612',
        '81048',
        'BLVD MILLAN',
        'SIN NUMERO',
        NULL,
        'LAS PALMAS',
        'GUASAVE',
        'SINALOA',
        'MEXICO',
        '6871313828',
        'lasaguilasmercadodelmar@gmail.com',
        NULL,
        'ADMIN',
        GETDATE()
    );
    
    PRINT '✓ Configuración insertada correctamente.';
END
GO

-- 3. Verificar ConfiguracionPAC para FiscalAPI
IF EXISTS (SELECT * FROM ConfiguracionPAC WHERE Proveedor = 'FiscalAPI' AND Activo = 1)
BEGIN
    PRINT '✓ ConfiguracionPAC de FiscalAPI está activa.';
    
    SELECT 
        Proveedor,
        Usuario AS 'API Key',
        CASE WHEN EsProduccion = 1 THEN 'PRODUCCION' ELSE 'PRUEBAS' END AS Ambiente,
        Activo
    FROM ConfiguracionPAC
    WHERE Proveedor = 'FiscalAPI';
END
ELSE
BEGIN
    PRINT '⚠️ ADVERTENCIA: No hay configuración activa de FiscalAPI en ConfiguracionPAC.';
    PRINT '   Ejecuta el script configurar_fiscalapi.ps1 primero.';
END
GO

-- 4. Mostrar configuración actual
PRINT '';
PRINT '========================================';
PRINT 'CONFIGURACION ACTUAL DEL EMISOR:';
PRINT '========================================';
SELECT 
    RFC,
    RazonSocial,
    RegimenFiscal,
    CodigoPostal,
    Municipio,
    Estado,
    Email
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;
GO

-- 5. Instrucciones finales
PRINT '';
PRINT '========================================';
PRINT 'SIGUIENTE PASO:';
PRINT '========================================';
PRINT '1. Verifica que los datos del emisor arriba sean CORRECTOS';
PRINT '2. Si usas certificados de PRUEBA, asegúrate que el RFC sea EKU9003173C9';
PRINT '3. Si usas certificados REALES, cambia el RFC por el tuyo';
PRINT '4. Los certificados deben estar subidos en https://test.fiscalapi.com/tax-files';
PRINT '5. Reinicia la aplicación web para que cargue la nueva configuración';
PRINT '';
