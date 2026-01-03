-- =====================================================
-- Script: 024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql
-- Descripción: Implementa funcionalidad para venta por gramaje 
--              y descomposición de productos
-- Fecha: 2025-12-29
-- =====================================================

USE DB_TIENDA
GO

-- =====================================================
-- 1. Agregar campos a tabla Productos para venta por gramaje
-- =====================================================

-- Verificar si la tabla Productos existe (versión nueva)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    -- Agregar campos para venta por gramaje
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'VentaPorGramaje')
    BEGIN
        ALTER TABLE Productos ADD VentaPorGramaje BIT NOT NULL DEFAULT 0
        PRINT 'Campo VentaPorGramaje agregado a tabla Productos'
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'PrecioPorKilo')
    BEGIN
        ALTER TABLE Productos ADD PrecioPorKilo DECIMAL(18,2) NULL
        PRINT 'Campo PrecioPorKilo agregado a tabla Productos'
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'UnidadMedidaBase')
    BEGIN
        ALTER TABLE Productos ADD UnidadMedidaBase VARCHAR(20) NULL -- 'KILO', 'GRAMO', 'LITRO', etc.
        PRINT 'Campo UnidadMedidaBase agregado a tabla Productos'
    END
END
GO

-- Verificar si existe tabla PRODUCTO (versión antigua)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PRODUCTO')
BEGIN
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PRODUCTO' AND COLUMN_NAME = 'VentaPorGramaje')
    BEGIN
        ALTER TABLE PRODUCTO ADD VentaPorGramaje BIT NOT NULL DEFAULT 0
        PRINT 'Campo VentaPorGramaje agregado a tabla PRODUCTO'
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PRODUCTO' AND COLUMN_NAME = 'PrecioPorKilo')
    BEGIN
        ALTER TABLE PRODUCTO ADD PrecioPorKilo DECIMAL(18,2) NULL
        PRINT 'Campo PrecioPorKilo agregado a tabla PRODUCTO'
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PRODUCTO' AND COLUMN_NAME = 'UnidadMedidaBase')
    BEGIN
        ALTER TABLE PRODUCTO ADD UnidadMedidaBase VARCHAR(20) NULL
        PRINT 'Campo UnidadMedidaBase agregado a tabla PRODUCTO'
    END
END
GO

-- =====================================================
-- 2. Crear tabla para descomposición de productos
-- =====================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DescomposicionProducto')
BEGIN
    CREATE TABLE DescomposicionProducto (
        DescomposicionID INT PRIMARY KEY IDENTITY(1,1),
        ProductoOrigenID INT NOT NULL,
        CantidadOrigen DECIMAL(18,3) NOT NULL, -- Cantidad descompuesta del producto origen
        FechaDescomposicion DATETIME NOT NULL DEFAULT GETDATE(),
        Usuario VARCHAR(50) NOT NULL,
        Observaciones VARCHAR(500),
        Estatus BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_DescomposicionProducto_ProductoOrigen FOREIGN KEY (ProductoOrigenID) 
            REFERENCES Productos(ProductoID)
    )
    PRINT 'Tabla DescomposicionProducto creada'
END
GO

-- =====================================================
-- 3. Crear tabla de detalle de descomposición
-- =====================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DetalleDescomposicion')
BEGIN
    CREATE TABLE DetalleDescomposicion (
        DetalleDescomposicionID INT PRIMARY KEY IDENTITY(1,1),
        DescomposicionID INT NOT NULL,
        ProductoResultanteID INT NOT NULL,
        CantidadResultante DECIMAL(18,3) NOT NULL, -- Cantidad generada del producto resultante
        PesoUnidad DECIMAL(18,3) NULL, -- Peso de cada unidad en kg (ej: 1.0, 2.0, 0.5)
        CONSTRAINT FK_DetalleDescomposicion_Descomposicion FOREIGN KEY (DescomposicionID) 
            REFERENCES DescomposicionProducto(DescomposicionID),
        CONSTRAINT FK_DetalleDescomposicion_ProductoResultante FOREIGN KEY (ProductoResultanteID) 
            REFERENCES Productos(ProductoID)
    )
    PRINT 'Tabla DetalleDescomposicion creada'
END
GO

-- =====================================================
-- 4. Agregar campo Gramos en DetalleVenta para ventas por gramaje
-- =====================================================

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DetalleVenta')
BEGIN
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetalleVenta' AND COLUMN_NAME = 'Gramos')
    BEGIN
        ALTER TABLE DetalleVenta ADD Gramos DECIMAL(18,3) NULL
        PRINT 'Campo Gramos agregado a tabla DetalleVenta'
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetalleVenta' AND COLUMN_NAME = 'PrecioCalculado')
    BEGIN
        ALTER TABLE DetalleVenta ADD PrecioCalculado DECIMAL(18,2) NULL
        PRINT 'Campo PrecioCalculado agregado a tabla DetalleVenta'
    END
END
GO

-- =====================================================
-- 5. Stored Procedure: Registrar Descomposición de Producto
-- =====================================================

IF OBJECT_ID('SP_RegistrarDescomposicionProducto', 'P') IS NOT NULL
    DROP PROCEDURE SP_RegistrarDescomposicionProducto
GO

CREATE PROCEDURE SP_RegistrarDescomposicionProducto
    @ProductoOrigenID INT,
    @CantidadOrigen DECIMAL(18,3),
    @Usuario VARCHAR(50),
    @Observaciones VARCHAR(500) = NULL,
    @DetalleDescomposicion VARCHAR(MAX), -- JSON con productos resultantes
    @SucursalID INT,
    @Mensaje VARCHAR(500) OUTPUT,
    @DescomposicionID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- Validar que el producto origen existe y tiene stock suficiente
        DECLARE @StockActual INT
        SELECT @StockActual = Stock 
        FROM ProductosSucursal 
        WHERE ProductoID = @ProductoOrigenID AND SucursalID = @SucursalID
        
        IF @StockActual IS NULL
        BEGIN
            SET @Mensaje = 'El producto origen no existe en esta sucursal'
            ROLLBACK TRANSACTION
            RETURN
        END
        
        IF @StockActual < @CantidadOrigen
        BEGIN
            SET @Mensaje = 'Stock insuficiente. Stock actual: ' + CAST(@StockActual AS VARCHAR(20))
            ROLLBACK TRANSACTION
            RETURN
        END
        
        -- Insertar registro de descomposición
        INSERT INTO DescomposicionProducto (ProductoOrigenID, CantidadOrigen, Usuario, Observaciones)
        VALUES (@ProductoOrigenID, @CantidadOrigen, @Usuario, @Observaciones)
        
        SET @DescomposicionID = SCOPE_IDENTITY()
        
        -- Procesar detalle de descomposición desde JSON
        INSERT INTO DetalleDescomposicion (DescomposicionID, ProductoResultanteID, CantidadResultante, PesoUnidad)
        SELECT 
            @DescomposicionID,
            ProductoResultanteID,
            CantidadResultante,
            PesoUnidad
        FROM OPENJSON(@DetalleDescomposicion)
        WITH (
            ProductoResultanteID INT '$.ProductoResultanteID',
            CantidadResultante DECIMAL(18,3) '$.CantidadResultante',
            PesoUnidad DECIMAL(18,3) '$.PesoUnidad'
        )
        
        -- Descontar del producto origen
        UPDATE ProductosSucursal
        SET Stock = Stock - @CantidadOrigen
        WHERE ProductoID = @ProductoOrigenID AND SucursalID = @SucursalID
        
        -- Incrementar productos resultantes
        DECLARE @ProductoResultanteID INT, @CantidadResultante DECIMAL(18,3)
        
        DECLARE cursor_resultantes CURSOR FOR
        SELECT ProductoResultanteID, CantidadResultante
        FROM DetalleDescomposicion
        WHERE DescomposicionID = @DescomposicionID
        
        OPEN cursor_resultantes
        FETCH NEXT FROM cursor_resultantes INTO @ProductoResultanteID, @CantidadResultante
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Verificar si el producto existe en la sucursal
            IF EXISTS (SELECT 1 FROM ProductosSucursal WHERE ProductoID = @ProductoResultanteID AND SucursalID = @SucursalID)
            BEGIN
                UPDATE ProductosSucursal
                SET Stock = Stock + @CantidadResultante
                WHERE ProductoID = @ProductoResultanteID AND SucursalID = @SucursalID
            END
            ELSE
            BEGIN
                -- Si no existe, crear el registro
                INSERT INTO ProductosSucursal (ProductoID, SucursalID, Stock, Estatus)
                VALUES (@ProductoResultanteID, @SucursalID, @CantidadResultante, 1)
            END
            
            FETCH NEXT FROM cursor_resultantes INTO @ProductoResultanteID, @CantidadResultante
        END
        
        CLOSE cursor_resultantes
        DEALLOCATE cursor_resultantes
        
        SET @Mensaje = 'Descomposición registrada exitosamente'
        COMMIT TRANSACTION
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
            
        SET @Mensaje = 'Error: ' + ERROR_MESSAGE()
    END CATCH
END
GO

-- =====================================================
-- 6. Stored Procedure: Calcular Precio por Gramaje
-- =====================================================

IF OBJECT_ID('SP_CalcularPrecioPorGramaje', 'P') IS NOT NULL
    DROP PROCEDURE SP_CalcularPrecioPorGramaje
GO

CREATE PROCEDURE SP_CalcularPrecioPorGramaje
    @ProductoID INT,
    @Gramos DECIMAL(18,3),
    @PrecioCalculado DECIMAL(18,2) OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @PrecioPorKilo DECIMAL(18,2)
        DECLARE @VentaPorGramaje BIT
        
        -- Obtener información del producto
        SELECT @PrecioPorKilo = PrecioPorKilo, @VentaPorGramaje = VentaPorGramaje
        FROM Productos
        WHERE ProductoID = @ProductoID
        
        IF @VentaPorGramaje = 0
        BEGIN
            SET @Mensaje = 'El producto no está configurado para venta por gramaje'
            SET @PrecioCalculado = 0
            RETURN
        END
        
        IF @PrecioPorKilo IS NULL OR @PrecioPorKilo = 0
        BEGIN
            SET @Mensaje = 'El producto no tiene precio por kilo configurado'
            SET @PrecioCalculado = 0
            RETURN
        END
        
        -- Calcular precio: (PrecioPorKilo / 1000) * Gramos
        SET @PrecioCalculado = (@PrecioPorKilo / 1000.0) * @Gramos
        SET @Mensaje = 'Precio calculado correctamente'
        
    END TRY
    BEGIN CATCH
        SET @Mensaje = 'Error: ' + ERROR_MESSAGE()
        SET @PrecioCalculado = 0
    END CATCH
END
GO

-- =====================================================
-- 7. Vista: Historial de Descomposiciones
-- =====================================================

IF OBJECT_ID('vw_HistorialDescomposiciones', 'V') IS NOT NULL
    DROP VIEW vw_HistorialDescomposiciones
GO

CREATE VIEW vw_HistorialDescomposiciones
AS
SELECT 
    d.DescomposicionID,
    d.FechaDescomposicion,
    d.Usuario,
    po.Nombre AS ProductoOrigen,
    d.CantidadOrigen,
    po.UnidadMedidaBase AS UnidadOrigen,
    STRING_AGG(
        pr.Nombre + ' (' + CAST(dd.CantidadResultante AS VARCHAR(20)) + 
        CASE WHEN dd.PesoUnidad IS NOT NULL 
             THEN ' x ' + CAST(dd.PesoUnidad AS VARCHAR(20)) + ' kg' 
             ELSE '' 
        END + ')', 
        ', '
    ) AS ProductosResultantes,
    d.Observaciones,
    d.Estatus
FROM DescomposicionProducto d
INNER JOIN Productos po ON d.ProductoOrigenID = po.ProductoID
INNER JOIN DetalleDescomposicion dd ON d.DescomposicionID = dd.DescomposicionID
INNER JOIN Productos pr ON dd.ProductoResultanteID = pr.ProductoID
GROUP BY 
    d.DescomposicionID,
    d.FechaDescomposicion,
    d.Usuario,
    po.Nombre,
    d.CantidadOrigen,
    po.UnidadMedidaBase,
    d.Observaciones,
    d.Estatus
GO

-- =====================================================
-- 8. Datos de ejemplo: Configurar productos para venta por gramaje
-- =====================================================

PRINT ''
PRINT '================================================'
PRINT 'Script completado exitosamente'
PRINT '================================================'
PRINT ''
PRINT 'Funcionalidades implementadas:'
PRINT '1. Campos para venta por gramaje en tabla Productos'
PRINT '2. Tablas para descomposición de productos'
PRINT '3. Stored Procedure: SP_RegistrarDescomposicionProducto'
PRINT '4. Stored Procedure: SP_CalcularPrecioPorGramaje'
PRINT '5. Vista: vw_HistorialDescomposiciones'
PRINT ''
PRINT 'Para configurar un producto para venta por gramaje:'
PRINT 'UPDATE Productos SET VentaPorGramaje = 1, PrecioPorKilo = [PRECIO], UnidadMedidaBase = ''KILO'' WHERE ProductoID = [ID]'
PRINT ''
GO
