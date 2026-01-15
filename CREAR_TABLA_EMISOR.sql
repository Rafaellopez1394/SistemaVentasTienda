-- =====================================================
-- CREAR TABLA ConfiguracionEmpresa
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA;
GO

-- Crear tabla ConfiguracionEmpresa si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionEmpresa')
BEGIN
    CREATE TABLE ConfiguracionEmpresa (
        ConfigEmpresaID INT PRIMARY KEY,
        RFC VARCHAR(13) NOT NULL,
        RazonSocial VARCHAR(255) NOT NULL,
        NombreComercial VARCHAR(255) NULL,
        RegimenFiscal VARCHAR(10) NOT NULL,
        Calle VARCHAR(255) NULL,
        NumeroExterior VARCHAR(50) NULL,
        NumeroInterior VARCHAR(50) NULL,
        Colonia VARCHAR(255) NULL,
        Municipio VARCHAR(255) NULL,
        Estado VARCHAR(255) NULL,
        CodigoPostal VARCHAR(10) NOT NULL,
        Pais VARCHAR(100) NULL,
        Telefono VARCHAR(20) NULL,
        Email VARCHAR(255) NULL,
        SitioWeb VARCHAR(255) NULL,
        UsuarioCreacion VARCHAR(100) NULL,
        FechaCreacion DATETIME NULL,
        UsuarioModificacion VARCHAR(100) NULL,
        FechaModificacion DATETIME NULL
    );
    
    PRINT '✓ Tabla ConfiguracionEmpresa creada exitosamente.';
    
    -- Insertar registro inicial con datos de LAS AGUILAS MERCADO DEL MAR
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
    
    PRINT '✓ Datos iniciales insertados correctamente.';
END
ELSE
BEGIN
    PRINT '⚠️ La tabla ConfiguracionEmpresa ya existe.';
END
GO

-- Mostrar configuración actual
SELECT 
    RFC,
    RazonSocial,
    NombreComercial,
    RegimenFiscal,
    CodigoPostal,
    Municipio,
    Estado,
    Email
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;
GO
