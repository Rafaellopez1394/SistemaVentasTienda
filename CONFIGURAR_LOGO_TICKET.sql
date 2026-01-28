-- Configurar logo para tickets
-- El logo debe estar en: C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\Imagenes\logo.bmp

USE [DB_TIENDA]
GO

-- Actualizar configuración del negocio con el logo
UPDATE [dbo].[ConfiguracionGeneral]
SET 
    LogoPath = '/Imagenes/logo.bmp',
    MostrarLogoEnTicket = 1,
    Usuario = SYSTEM_USER,
    FechaModificacion = GETDATE();

GO

-- Verificar la configuración
SELECT 
    NombreNegocio,
    LogoPath,
    MostrarLogoEnTicket,
    ImprimirTicketAutomatico,
    FechaModificacion
FROM [dbo].[ConfiguracionGeneral];

GO

PRINT 'Logo configurado correctamente!'
PRINT 'Ruta: /Imagenes/logo.bmp'
PRINT 'Archivo físico: C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\Imagenes\logo.bmp'
PRINT ''
PRINT 'Recuerda cerrar el navegador completamente y volver a abrir VentaPOS para que se cargue la configuración.'
