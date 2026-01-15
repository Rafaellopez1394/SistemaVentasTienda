-- =============================================
-- Script: 045_AGREGAR_STOCKMINIMO_PRODUCTOS.sql
-- Descripción: Actualizar stored procedures para incluir StockMinimo
-- Autor: Sistema
-- Fecha: 2026-01-05
-- =============================================

USE DB_TIENDA;
GO

PRINT '====================================';
PRINT 'ACTUALIZAR STORED PROCEDURES';
PRINT 'Agregar parámetro @StockMinimo';
PRINT '====================================';
GO

-- =============================================
-- 1. Actualizar SP: AltaProducto
-- =============================================
IF OBJECT_ID('AltaProducto', 'P') IS NOT NULL
    DROP PROCEDURE AltaProducto;
GO

CREATE PROCEDURE AltaProducto
    @Nombre VARCHAR(250),
    @CategoriaID INT,
    @ClaveProdServSAT VARCHAR(50),
    @ClaveUnidadSAT VARCHAR(10),
    @CodigoInterno VARCHAR(50) = NULL,
    @TasaIVAID INT,
    @TasaIEPSID INT = NULL,
    @Exento BIT = 0,
    @StockMinimo INT = NULL,
    @Usuario VARCHAR(100) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Productos (
            Nombre,
            CategoriaID,
            ClaveProdServSAT,
            ClaveUnidadSAT,
            CodigoInterno,
            TasaIVAID,
            TasaIEPSID,
            Exento,
            StockMinimo,
            Estatus,
            Usuario,
            FechaAlta,
            UltimaAct
        )
        VALUES (
            @Nombre,
            @CategoriaID,
            @ClaveProdServSAT,
            @ClaveUnidadSAT,
            @CodigoInterno,
            @TasaIVAID,
            @TasaIEPSID,
            @Exento,
            @StockMinimo,
            1, -- Activo por defecto
            @Usuario,
            GETDATE(),
            GETDATE()
        );
        
        PRINT '✓ Producto creado: ' + @Nombre;
        
    END TRY
    BEGIN CATCH
        PRINT '✗ Error al crear producto: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END;
GO

PRINT '✓ Stored Procedure AltaProducto actualizado';
GO

-- =============================================
-- 2. Actualizar SP: ActualizarProducto
-- =============================================
IF OBJECT_ID('ActualizarProducto', 'P') IS NOT NULL
    DROP PROCEDURE ActualizarProducto;
GO

CREATE PROCEDURE ActualizarProducto
    @ProductoID INT,
    @Nombre VARCHAR(250),
    @CategoriaID INT,
    @ClaveProdServSAT VARCHAR(50),
    @ClaveUnidadSAT VARCHAR(10),
    @CodigoInterno VARCHAR(50) = NULL,
    @TasaIVAID INT,
    @TasaIEPSID INT = NULL,
    @Exento BIT = 0,
    @StockMinimo INT = NULL,
    @Usuario VARCHAR(100) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Productos
        SET 
            Nombre = @Nombre,
            CategoriaID = @CategoriaID,
            ClaveProdServSAT = @ClaveProdServSAT,
            ClaveUnidadSAT = @ClaveUnidadSAT,
            CodigoInterno = @CodigoInterno,
            TasaIVAID = @TasaIVAID,
            TasaIEPSID = @TasaIEPSID,
            Exento = @Exento,
            StockMinimo = @StockMinimo,
            Usuario = @Usuario,
            UltimaAct = GETDATE()
        WHERE ProductoID = @ProductoID;
        
        PRINT '✓ Producto actualizado: ' + CAST(@ProductoID AS VARCHAR);
        
    END TRY
    BEGIN CATCH
        PRINT '✗ Error al actualizar producto: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END;
GO

PRINT '✓ Stored Procedure ActualizarProducto actualizado';
GO

-- =============================================
-- VERIFICACIÓN
-- =============================================
PRINT '';
PRINT '====================================';
PRINT 'VERIFICACIÓN';
PRINT '====================================';

-- Verificar que los SPs existen
IF OBJECT_ID('AltaProducto', 'P') IS NOT NULL
    PRINT '✓ AltaProducto existe';
ELSE
    PRINT '✗ AltaProducto NO existe';

IF OBJECT_ID('ActualizarProducto', 'P') IS NOT NULL
    PRINT '✓ ActualizarProducto existe';
ELSE
    PRINT '✗ ActualizarProducto NO existe';

-- Verificar parámetros
PRINT '';
PRINT 'Parámetros de AltaProducto:';
SELECT 
    PARAMETER_NAME,
    DATA_TYPE,
    PARAMETER_MODE
FROM INFORMATION_SCHEMA.PARAMETERS
WHERE SPECIFIC_NAME = 'AltaProducto'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT 'Parámetros de ActualizarProducto:';
SELECT 
    PARAMETER_NAME,
    DATA_TYPE,
    PARAMETER_MODE
FROM INFORMATION_SCHEMA.PARAMETERS
WHERE SPECIFIC_NAME = 'ActualizarProducto'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '====================================';
PRINT 'ACTUALIZACIÓN COMPLETADA';
PRINT '====================================';
GO
