-- Agregar columnas para logo en ConfiguracionGeneral
USE DB_TIENDA;
GO

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionGeneral]') AND name = 'LogoPath')
BEGIN
    ALTER TABLE ConfiguracionGeneral ADD LogoPath NVARCHAR(500) NULL;
    PRINT 'Columna LogoPath agregada';
END
ELSE
BEGIN
    PRINT 'Columna LogoPath ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionGeneral]') AND name = 'MostrarLogoEnTicket')
BEGIN
    ALTER TABLE ConfiguracionGeneral ADD MostrarLogoEnTicket BIT DEFAULT 0 NOT NULL;
    PRINT 'Columna MostrarLogoEnTicket agregada';
END
ELSE
BEGIN
    PRINT 'Columna MostrarLogoEnTicket ya existe';
END

-- Configurar el logo para que se muestre
UPDATE ConfiguracionGeneral
SET LogoPath = '/Imagenes/logo.png',
    MostrarLogoEnTicket = 1
WHERE ConfigID = 1;

PRINT 'Logo configurado correctamente';
GO

-- Verificar la configuración
SELECT NombreNegocio, RFC, LogoPath, MostrarLogoEnTicket 
FROM ConfiguracionGeneral;
