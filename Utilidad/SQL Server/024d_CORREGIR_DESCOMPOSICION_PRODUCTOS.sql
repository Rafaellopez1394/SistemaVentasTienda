-- =====================================================
-- Script: 024d_CORREGIR_DESCOMPOSICION_PRODUCTOS.sql
-- Descripción: Corrige el stored procedure de descomposición
--              para usar LotesProducto en lugar de ProductosSucursal
-- =====================================================

USE DB_TIENDA
GO

-- Actualizar Stored Procedure: DescomponerProducto
IF OBJECT_ID('DescomponerProducto', 'P') IS NOT NULL
    DROP PROCEDURE DescomponerProducto
GO

CREATE PROCEDURE DescomponerProducto
    @ProductoOrigenID INT,
    @CantidadOrigen INT,
    @SucursalID INT,
    @DetalleDescomposicion NVARCHAR(MAX), -- JSON con array de productos resultantes
    @Usuario VARCHAR(100),
    @Observaciones VARCHAR(500) = NULL,
    @Mensaje VARCHAR(500) OUTPUT,
    @DescomposicionID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- Validar que el producto origen existe y tiene stock suficiente en lotes
        DECLARE @StockActual DECIMAL(18,2)
        SELECT @StockActual = ISNULL(SUM(CantidadDisponible), 0)
        FROM LotesProducto 
        WHERE ProductoID = @ProductoOrigenID 
          AND Estatus = 1
          AND CantidadDisponible > 0
        
        IF @StockActual = 0
        BEGIN
            SET @Mensaje = 'El producto origen no tiene stock disponible'
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
        
        -- Descontar del producto origen usando FIFO en lotes
        DECLARE @CantidadRestante INT = @CantidadOrigen
        DECLARE @LoteID INT, @CantidadLote DECIMAL(18,2), @PrecioCompra DECIMAL(18,2)
        
        DECLARE cursor_lotes CURSOR FOR
        SELECT LoteID, CantidadDisponible, PrecioCompra
        FROM LotesProducto
        WHERE ProductoID = @ProductoOrigenID 
          AND Estatus = 1
          AND CantidadDisponible > 0
        ORDER BY FechaIngreso ASC -- FIFO
        
        OPEN cursor_lotes
        FETCH NEXT FROM cursor_lotes INTO @LoteID, @CantidadLote, @PrecioCompra
        
        WHILE @@FETCH_STATUS = 0 AND @CantidadRestante > 0
        BEGIN
            IF @CantidadLote >= @CantidadRestante
            BEGIN
                -- El lote tiene suficiente para cubrir lo que falta
                UPDATE LotesProducto
                SET CantidadDisponible = CantidadDisponible - @CantidadRestante
                WHERE LoteID = @LoteID
                
                SET @CantidadRestante = 0
            END
            ELSE
            BEGIN
                -- Usar todo el lote y continuar
                UPDATE LotesProducto
                SET CantidadDisponible = 0
                WHERE LoteID = @LoteID
                
                SET @CantidadRestante = @CantidadRestante - @CantidadLote
            END
            
            FETCH NEXT FROM cursor_lotes INTO @LoteID, @CantidadLote, @PrecioCompra
        END
        
        CLOSE cursor_lotes
        DEALLOCATE cursor_lotes
        
        -- Incrementar productos resultantes creando nuevos lotes
        DECLARE @ProductoResultanteID INT, @CantidadResultante DECIMAL(18,3)
        DECLARE @FechaActual DATETIME = GETDATE()
        
        -- Obtener precio base promedio del producto origen para calcular proporcionalmente
        DECLARE @PrecioBase DECIMAL(18,2)
        SELECT @PrecioBase = AVG(PrecioCompra)
        FROM LotesProducto
        WHERE ProductoID = @ProductoOrigenID AND Estatus = 1
        
        DECLARE cursor_resultantes CURSOR FOR
        SELECT ProductoResultanteID, CantidadResultante
        FROM DetalleDescomposicion
        WHERE DescomposicionID = @DescomposicionID
        
        OPEN cursor_resultantes
        FETCH NEXT FROM cursor_resultantes INTO @ProductoResultanteID, @CantidadResultante
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Crear nuevo lote para el producto resultante
            INSERT INTO LotesProducto (
                ProductoID,
                CantidadInicial,
                CantidadDisponible,
                PrecioCompra,
                PrecioVenta,
                FechaIngreso,
                Estatus
            )
            SELECT 
                @ProductoResultanteID,
                @CantidadResultante,
                @CantidadResultante,
                @PrecioBase, -- Usar precio del origen
                PrecioVenta, -- Mantener precio de venta del producto
                @FechaActual,
                1
            FROM Productos
            WHERE ProductoID = @ProductoResultanteID
            
            -- Registrar movimiento de inventario
            INSERT INTO InventarioMovimientos (
                ProductoID,
                TipoMovimiento,
                Cantidad,
                Referencia,
                Usuario,
                Fecha
            )
            VALUES (
                @ProductoResultanteID,
                'DESCOMPOSICION',
                @CantidadResultante,
                'Descomposición ID: ' + CAST(@DescomposicionID AS VARCHAR(20)),
                @Usuario,
                @FechaActual
            )
            
            FETCH NEXT FROM cursor_resultantes INTO @ProductoResultanteID, @CantidadResultante
        END
        
        CLOSE cursor_resultantes
        DEALLOCATE cursor_resultantes
        
        -- Registrar movimiento de salida del producto origen
        INSERT INTO InventarioMovimientos (
            ProductoID,
            TipoMovimiento,
            Cantidad,
            Referencia,
            Usuario,
            Fecha
        )
        VALUES (
            @ProductoOrigenID,
            'DESCOMPOSICION_SALIDA',
            @CantidadOrigen,
            'Descomposición ID: ' + CAST(@DescomposicionID AS VARCHAR(20)),
            @Usuario,
            @FechaActual
        )
        
        SET @Mensaje = 'Descomposición registrada exitosamente'
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
            
        SET @Mensaje = 'ERROR: ' + ERROR_MESSAGE()
    END CATCH
END
GO

PRINT '✓ Stored Procedure DescomponerProducto corregido para usar LotesProducto'
GO
