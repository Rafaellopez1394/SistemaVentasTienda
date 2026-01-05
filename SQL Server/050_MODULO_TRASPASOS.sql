-- ========================================
-- MÓDULO DE TRASPASOS ENTRE SUCURSALES
-- ========================================
-- Fecha: 2026-01-04
-- Descripción: Sistema completo de traspasos de productos entre sucursales
--              con control de inventarios por sucursal

USE DB_TIENDA;
GO

PRINT '========================================';
PRINT 'CREANDO MÓDULO DE TRASPASOS';
PRINT '========================================';
PRINT '';

-- ========================================
-- 1. AGREGAR COLUMNA SUCURSALID A LOTESPRODUCTO (si no existe)
-- ========================================

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('LotesProducto') 
               AND name = 'SucursalID')
BEGIN
    ALTER TABLE LotesProducto
    ADD SucursalID INT NULL;
    
    PRINT '✓ Columna SucursalID agregada a LotesProducto';
    
    -- Establecer sucursal predeterminada para lotes existentes
    UPDATE LotesProducto
    SET SucursalID = (SELECT TOP 1 SucursalID FROM Sucursal ORDER BY SucursalID)
    WHERE SucursalID IS NULL;
    
    -- Agregar foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                   WHERE name = 'FK_LotesProducto_Sucursal')
    BEGIN
        ALTER TABLE LotesProducto
        ADD CONSTRAINT FK_LotesProducto_Sucursal
        FOREIGN KEY (SucursalID) REFERENCES Sucursal(SucursalID);
        
        PRINT '✓ Foreign Key FK_LotesProducto_Sucursal creada';
    END
END
ELSE
BEGIN
    PRINT '→ Columna SucursalID ya existe en LotesProducto';
END
GO

-- ========================================
-- 2. TABLA DE TRASPASOS
-- ========================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Traspasos')
BEGIN
    CREATE TABLE Traspasos (
        TraspasoID INT IDENTITY(1,1) PRIMARY KEY,
        FolioTraspaso VARCHAR(20) NOT NULL UNIQUE,
        FechaTraspaso DATETIME NOT NULL DEFAULT GETDATE(),
        SucursalOrigenID INT NOT NULL,
        SucursalDestinoID INT NOT NULL,
        UsuarioRegistro VARCHAR(100) NOT NULL,
        Observaciones VARCHAR(500),
        Estatus VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE', -- PENDIENTE, EN_TRANSITO, RECIBIDO, CANCELADO
        FechaEnvio DATETIME NULL,
        FechaRecepcion DATETIME NULL,
        UsuarioEnvia VARCHAR(100),
        UsuarioRecibe VARCHAR(100),
        MotivoCancelacion VARCHAR(500),
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT FK_Traspasos_SucursalOrigen 
            FOREIGN KEY (SucursalOrigenID) REFERENCES Sucursal(SucursalID),
        CONSTRAINT FK_Traspasos_SucursalDestino 
            FOREIGN KEY (SucursalDestinoID) REFERENCES Sucursal(SucursalID),
        CONSTRAINT CK_Traspasos_Sucursales_Diferentes 
            CHECK (SucursalOrigenID <> SucursalDestinoID),
        CONSTRAINT CK_Traspasos_Estatus 
            CHECK (Estatus IN ('PENDIENTE', 'EN_TRANSITO', 'RECIBIDO', 'CANCELADO'))
    );
    
    PRINT '✓ Tabla Traspasos creada';
END
ELSE
BEGIN
    PRINT '→ Tabla Traspasos ya existe';
END
GO

-- ========================================
-- 3. TABLA DE DETALLE DE TRASPASOS
-- ========================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetalleTraspasos')
BEGIN
    CREATE TABLE DetalleTraspasos (
        DetalleTraspasoID INT IDENTITY(1,1) PRIMARY KEY,
        TraspasoID INT NOT NULL,
        ProductoID INT NOT NULL,
        LoteOrigenID INT NULL, -- Lote de la sucursal origen
        CantidadSolicitada DECIMAL(18,3) NOT NULL,
        CantidadEnviada DECIMAL(18,3) NULL,
        CantidadRecibida DECIMAL(18,3) NULL,
        PrecioUnitario DECIMAL(18,2) NOT NULL,
        Observaciones VARCHAR(500),
        
        CONSTRAINT FK_DetalleTraspasos_Traspaso 
            FOREIGN KEY (TraspasoID) REFERENCES Traspasos(TraspasoID),
        CONSTRAINT FK_DetalleTraspasos_Producto 
            FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),
        CONSTRAINT FK_DetalleTraspasos_Lote
            FOREIGN KEY (LoteOrigenID) REFERENCES LotesProducto(LoteID),
        CONSTRAINT CK_DetalleTraspasos_Cantidades 
            CHECK (CantidadSolicitada > 0)
    );
    
    CREATE INDEX IX_DetalleTraspasos_Traspaso ON DetalleTraspasos(TraspasoID);
    CREATE INDEX IX_DetalleTraspasos_Producto ON DetalleTraspasos(ProductoID);
    
    PRINT '✓ Tabla DetalleTraspasos creada';
END
ELSE
BEGIN
    PRINT '→ Tabla DetalleTraspasos ya existe';
END
GO

-- ========================================
-- 4. VISTA DE INVENTARIOS POR SUCURSAL
-- ========================================

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_InventarioSucursal')
    DROP VIEW vw_InventarioSucursal;
GO

CREATE VIEW vw_InventarioSucursal AS
SELECT 
    p.ProductoID,
    p.Nombre AS NombreProducto,
    p.CodigoInterno AS CodigoProducto,
    p.Descripcion,
    p.UnidadMedidaBase,
    s.SucursalID,
    s.Nombre AS NombreSucursal,
    s.RFC AS RFCSucursal,
    COUNT(l.LoteID) AS TotalLotes,
    ISNULL(SUM(l.CantidadDisponible), 0) AS CantidadDisponible,
    ISNULL(SUM(l.CantidadTotal), 0) AS CantidadTotal,
    ISNULL(AVG(l.PrecioCompra), 0) AS PrecioPromedioCompra,
    ISNULL(AVG(l.PrecioVenta), 0) AS PrecioPromedioVenta
FROM Productos p
CROSS JOIN Sucursal s
LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID 
    AND l.SucursalID = s.SucursalID 
    AND l.Estatus = 1
WHERE p.Activo = 1
GROUP BY 
    p.ProductoID, p.Nombre, p.CodigoInterno, p.Descripcion, p.UnidadMedidaBase,
    s.SucursalID, s.Nombre, s.RFC;
GO

PRINT '✓ Vista vw_InventarioSucursal creada';
GO

-- ========================================
-- 5. SP: REGISTRAR TRASPASO
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_RegistrarTraspaso')
    DROP PROCEDURE sp_RegistrarTraspaso;
GO

CREATE PROCEDURE sp_RegistrarTraspaso
    @SucursalOrigenID INT,
    @SucursalDestinoID INT,
    @UsuarioRegistro VARCHAR(100),
    @Observaciones VARCHAR(500) = NULL,
    @DetallesXML XML, -- <Detalles><Item ProductoID="1" Cantidad="5" PrecioUnitario="10.00"/></Detalles>
    @TraspasoID INT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Validar sucursales
        IF @SucursalOrigenID = @SucursalDestinoID
        BEGIN
            SET @Mensaje = 'No se puede hacer traspaso a la misma sucursal';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM Sucursal WHERE SucursalID = @SucursalOrigenID)
        BEGIN
            SET @Mensaje = 'Sucursal origen no existe';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM Sucursal WHERE SucursalID = @SucursalDestinoID)
        BEGIN
            SET @Mensaje = 'Sucursal destino no existe';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Generar folio único
        DECLARE @FolioTraspaso VARCHAR(20);
        SET @FolioTraspaso = 'TRSP-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-' + 
                             RIGHT('0000' + CAST(ISNULL((SELECT MAX(TraspasoID) FROM Traspasos), 0) + 1 AS VARCHAR), 4);
        
        -- Insertar cabecera del traspaso
        INSERT INTO Traspasos (
            FolioTraspaso, FechaTraspaso, SucursalOrigenID, SucursalDestinoID,
            UsuarioRegistro, Observaciones, Estatus
        )
        VALUES (
            @FolioTraspaso, GETDATE(), @SucursalOrigenID, @SucursalDestinoID,
            @UsuarioRegistro, @Observaciones, 'PENDIENTE'
        );
        
        SET @TraspasoID = SCOPE_IDENTITY();
        
        -- Insertar detalle desde XML
        INSERT INTO DetalleTraspasos (TraspasoID, ProductoID, CantidadSolicitada, PrecioUnitario)
        SELECT 
            @TraspasoID,
            Item.value('@ProductoID', 'INT'),
            Item.value('@Cantidad', 'DECIMAL(18,3)'),
            Item.value('@PrecioUnitario', 'DECIMAL(18,2)')
        FROM @DetallesXML.nodes('/Detalles/Item') AS Detalles(Item);
        
        -- Validar inventario disponible en origen
        IF EXISTS (
            SELECT 1
            FROM DetalleTraspasos dt
            INNER JOIN (
                SELECT 
                    ProductoID,
                    ISNULL(SUM(CantidadDisponible), 0) AS Disponible
                FROM LotesProducto
                WHERE SucursalID = @SucursalOrigenID AND Estatus = 1
                GROUP BY ProductoID
            ) inv ON dt.ProductoID = inv.ProductoID
            WHERE dt.TraspasoID = @TraspasoID
            AND dt.CantidadSolicitada > inv.Disponible
        )
        BEGIN
            SET @Mensaje = 'No hay suficiente inventario en la sucursal origen';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        SET @Mensaje = 'Traspaso registrado exitosamente con folio: ' + @FolioTraspaso;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Error al registrar traspaso: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ SP sp_RegistrarTraspaso creado';
GO

-- ========================================
-- 6. SP: ENVIAR TRASPASO (afecta inventario origen)
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_EnviarTraspaso')
    DROP PROCEDURE sp_EnviarTraspaso;
GO

CREATE PROCEDURE sp_EnviarTraspaso
    @TraspasoID INT,
    @UsuarioEnvia VARCHAR(100),
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Validar que existe y está pendiente
        IF NOT EXISTS (SELECT 1 FROM Traspasos WHERE TraspasoID = @TraspasoID AND Estatus = 'PENDIENTE')
        BEGIN
            SET @Mensaje = 'El traspaso no existe o no está en estatus PENDIENTE';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        DECLARE @SucursalOrigenID INT;
        SELECT @SucursalOrigenID = SucursalOrigenID FROM Traspasos WHERE TraspasoID = @TraspasoID;
        
        -- Descontar del inventario origen usando FIFO
        DECLARE @ProductoID INT, @Cantidad DECIMAL(18,3), @DetalleID INT;
        DECLARE cur_detalle CURSOR FOR
        SELECT DetalleTraspasoID, ProductoID, CantidadSolicitada
        FROM DetalleTraspasos
        WHERE TraspasoID = @TraspasoID;
        
        OPEN cur_detalle;
        FETCH NEXT FROM cur_detalle INTO @DetalleID, @ProductoID, @Cantidad;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @CantidadPendiente DECIMAL(18,3) = @Cantidad;
            DECLARE @LoteID INT, @DisponibleLote DECIMAL(18,3);
            
            -- Cursor para lotes FIFO
            DECLARE cur_lotes CURSOR FOR
            SELECT LoteID, CantidadDisponible
            FROM LotesProducto
            WHERE ProductoID = @ProductoID 
              AND SucursalID = @SucursalOrigenID
              AND Estatus = 1
              AND CantidadDisponible > 0
            ORDER BY FechaEntrada ASC;
            
            OPEN cur_lotes;
            FETCH NEXT FROM cur_lotes INTO @LoteID, @DisponibleLote;
            
            WHILE @@FETCH_STATUS = 0 AND @CantidadPendiente > 0
            BEGIN
                DECLARE @CantidadDescontar DECIMAL(18,3);
                SET @CantidadDescontar = CASE 
                    WHEN @DisponibleLote >= @CantidadPendiente THEN @CantidadPendiente
                    ELSE @DisponibleLote
                END;
                
                -- Descontar del lote
                UPDATE LotesProducto
                SET CantidadDisponible = CantidadDisponible - @CantidadDescontar,
                    UltimaAct = GETDATE()
                WHERE LoteID = @LoteID;
                
                -- Guardar referencia al primer lote usado
                IF @CantidadPendiente = @Cantidad
                BEGIN
                    UPDATE DetalleTraspasos
                    SET LoteOrigenID = @LoteID
                    WHERE DetalleTraspasoID = @DetalleID;
                END
                
                SET @CantidadPendiente = @CantidadPendiente - @CantidadDescontar;
                FETCH NEXT FROM cur_lotes INTO @LoteID, @DisponibleLote;
            END
            
            CLOSE cur_lotes;
            DEALLOCATE cur_lotes;
            
            -- Actualizar cantidad enviada
            UPDATE DetalleTraspasos
            SET CantidadEnviada = @Cantidad
            WHERE DetalleTraspasoID = @DetalleID;
            
            FETCH NEXT FROM cur_detalle INTO @DetalleID, @ProductoID, @Cantidad;
        END
        
        CLOSE cur_detalle;
        DEALLOCATE cur_detalle;
        
        -- Actualizar traspaso a EN_TRANSITO
        UPDATE Traspasos
        SET Estatus = 'EN_TRANSITO',
            FechaEnvio = GETDATE(),
            UsuarioEnvia = @UsuarioEnvia
        WHERE TraspasoID = @TraspasoID;
        
        SET @Mensaje = 'Traspaso enviado exitosamente. Inventario descontado de sucursal origen.';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Error al enviar traspaso: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ SP sp_EnviarTraspaso creado';
GO

-- ========================================
-- 7. SP: RECIBIR TRASPASO (afecta inventario destino)
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_RecibirTraspaso')
    DROP PROCEDURE sp_RecibirTraspaso;
GO

CREATE PROCEDURE sp_RecibirTraspaso
    @TraspasoID INT,
    @UsuarioRecibe VARCHAR(100),
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Validar que existe y está en tránsito
        IF NOT EXISTS (SELECT 1 FROM Traspasos WHERE TraspasoID = @TraspasoID AND Estatus = 'EN_TRANSITO')
        BEGIN
            SET @Mensaje = 'El traspaso no existe o no está en estatus EN_TRANSITO';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        DECLARE @SucursalDestinoID INT;
        SELECT @SucursalDestinoID = SucursalDestinoID FROM Traspasos WHERE TraspasoID = @TraspasoID;
        
        -- Crear lotes en sucursal destino
        DECLARE @ProductoID INT, @Cantidad DECIMAL(18,3), @PrecioUnitario DECIMAL(18,2);
        DECLARE cur_detalle CURSOR FOR
        SELECT ProductoID, CantidadEnviada, PrecioUnitario
        FROM DetalleTraspasos
        WHERE TraspasoID = @TraspasoID;
        
        OPEN cur_detalle;
        FETCH NEXT FROM cur_detalle INTO @ProductoID, @Cantidad, @PrecioUnitario;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Insertar nuevo lote en sucursal destino
            INSERT INTO LotesProducto (
                ProductoID, SucursalID, FechaEntrada,
                CantidadTotal, CantidadDisponible,
                PrecioCompra, PrecioVenta,
                Usuario, Estatus
            )
            VALUES (
                @ProductoID, @SucursalDestinoID, GETDATE(),
                @Cantidad, @Cantidad,
                @PrecioUnitario, @PrecioUnitario * 1.3, -- Margen del 30%
                @UsuarioRecibe, 1
            );
            
            -- Actualizar cantidad recibida
            UPDATE DetalleTraspasos
            SET CantidadRecibida = @Cantidad
            WHERE TraspasoID = @TraspasoID AND ProductoID = @ProductoID;
            
            FETCH NEXT FROM cur_detalle INTO @ProductoID, @Cantidad, @PrecioUnitario;
        END
        
        CLOSE cur_detalle;
        DEALLOCATE cur_detalle;
        
        -- Actualizar traspaso a RECIBIDO
        UPDATE Traspasos
        SET Estatus = 'RECIBIDO',
            FechaRecepcion = GETDATE(),
            UsuarioRecibe = @UsuarioRecibe
        WHERE TraspasoID = @TraspasoID;
        
        SET @Mensaje = 'Traspaso recibido exitosamente. Inventario agregado a sucursal destino.';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Error al recibir traspaso: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ SP sp_RecibirTraspaso creado';
GO

-- ========================================
-- 8. SP: CANCELAR TRASPASO
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CancelarTraspaso')
    DROP PROCEDURE sp_CancelarTraspaso;
GO

CREATE PROCEDURE sp_CancelarTraspaso
    @TraspasoID INT,
    @MotivoCancelacion VARCHAR(500),
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @Estatus VARCHAR(20);
        SELECT @Estatus = Estatus FROM Traspasos WHERE TraspasoID = @TraspasoID;
        
        IF @Estatus IS NULL
        BEGIN
            SET @Mensaje = 'El traspaso no existe';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        IF @Estatus = 'RECIBIDO'
        BEGIN
            SET @Mensaje = 'No se puede cancelar un traspaso ya recibido';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Si ya fue enviado, devolver inventario a origen
        IF @Estatus = 'EN_TRANSITO'
        BEGIN
            DECLARE @SucursalOrigenID INT;
            SELECT @SucursalOrigenID = SucursalOrigenID FROM Traspasos WHERE TraspasoID = @TraspasoID;
            
            -- Devolver al inventario origen
            DECLARE @ProductoID INT, @Cantidad DECIMAL(18,3), @LoteOrigenID INT;
            DECLARE cur_detalle CURSOR FOR
            SELECT ProductoID, CantidadEnviada, LoteOrigenID
            FROM DetalleTraspasos
            WHERE TraspasoID = @TraspasoID;
            
            OPEN cur_detalle;
            FETCH NEXT FROM cur_detalle INTO @ProductoID, @Cantidad, @LoteOrigenID;
            
            WHILE @@FETCH_STATUS = 0
            BEGIN
                -- Devolver al lote original si aún existe
                IF EXISTS (SELECT 1 FROM LotesProducto WHERE LoteID = @LoteOrigenID)
                BEGIN
                    UPDATE LotesProducto
                    SET CantidadDisponible = CantidadDisponible + @Cantidad
                    WHERE LoteID = @LoteOrigenID;
                END
                
                FETCH NEXT FROM cur_detalle INTO @ProductoID, @Cantidad, @LoteOrigenID;
            END
            
            CLOSE cur_detalle;
            DEALLOCATE cur_detalle;
        END
        
        -- Cancelar traspaso
        UPDATE Traspasos
        SET Estatus = 'CANCELADO',
            MotivoCancelacion = @MotivoCancelacion
        WHERE TraspasoID = @TraspasoID;
        
        SET @Mensaje = 'Traspaso cancelado exitosamente';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Error al cancelar traspaso: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ SP sp_CancelarTraspaso creado';
GO

-- ========================================
-- 9. SP: CONSULTAR TRASPASOS
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ConsultarTraspasos')
    DROP PROCEDURE sp_ConsultarTraspasos;
GO

CREATE PROCEDURE sp_ConsultarTraspasos
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL,
    @SucursalID INT = NULL,
    @Estatus VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TraspasoID,
        t.FolioTraspaso,
        t.FechaTraspaso,
        so.Nombre AS SucursalOrigen,
        sd.Nombre AS SucursalDestino,
        t.UsuarioRegistro,
        t.Estatus,
        t.FechaEnvio,
        t.FechaRecepcion,
        t.UsuarioEnvia,
        t.UsuarioRecibe,
        t.Observaciones,
        t.MotivoCancelacion,
        COUNT(dt.DetalleTraspasoID) AS TotalProductos,
        SUM(dt.CantidadSolicitada) AS TotalCantidad,
        SUM(dt.CantidadSolicitada * dt.PrecioUnitario) AS ValorTotal
    FROM Traspasos t
    INNER JOIN Sucursal so ON t.SucursalOrigenID = so.SucursalID
    INNER JOIN Sucursal sd ON t.SucursalDestinoID = sd.SucursalID
    LEFT JOIN DetalleTraspasos dt ON t.TraspasoID = dt.TraspasoID
    WHERE (@FechaInicio IS NULL OR CAST(t.FechaTraspaso AS DATE) >= @FechaInicio)
      AND (@FechaFin IS NULL OR CAST(t.FechaTraspaso AS DATE) <= @FechaFin)
      AND (@SucursalID IS NULL OR t.SucursalOrigenID = @SucursalID OR t.SucursalDestinoID = @SucursalID)
      AND (@Estatus IS NULL OR t.Estatus = @Estatus)
    GROUP BY 
        t.TraspasoID, t.FolioTraspaso, t.FechaTraspaso, so.Nombre, sd.Nombre,
        t.UsuarioRegistro, t.Estatus, t.FechaEnvio, t.FechaRecepcion,
        t.UsuarioEnvia, t.UsuarioRecibe, t.Observaciones, t.MotivoCancelacion
    ORDER BY t.FechaTraspaso DESC;
END
GO

PRINT '✓ SP sp_ConsultarTraspasos creado';
GO

-- ========================================
-- 10. SP: OBTENER DETALLE DE TRASPASO
-- ========================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ObtenerDetalleTraspaso')
    DROP PROCEDURE sp_ObtenerDetalleTraspaso;
GO

CREATE PROCEDURE sp_ObtenerDetalleTraspaso
    @TraspasoID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        dt.DetalleTraspasoID,
        dt.ProductoID,
        p.Nombre AS NombreProducto,
        p.CodigoInterno AS CodigoProducto,
        p.UnidadMedidaBase,
        dt.CantidadSolicitada,
        dt.CantidadEnviada,
        dt.CantidadRecibida,
        dt.PrecioUnitario,
        (dt.CantidadSolicitada * dt.PrecioUnitario) AS Subtotal,
        dt.Observaciones
    FROM DetalleTraspasos dt
    INNER JOIN Productos p ON dt.ProductoID = p.ProductoID
    WHERE dt.TraspasoID = @TraspasoID
    ORDER BY dt.DetalleTraspasoID;
END
GO

PRINT '✓ SP sp_ObtenerDetalleTraspaso creado';
GO

-- ========================================
-- RESUMEN
-- ========================================

PRINT '';
PRINT '========================================';
PRINT 'MÓDULO DE TRASPASOS COMPLETADO';
PRINT '========================================';
PRINT '';
PRINT 'Tablas creadas:';
PRINT '  ✓ Traspasos';
PRINT '  ✓ DetalleTraspasos';
PRINT '';
PRINT 'Vistas creadas:';
PRINT '  ✓ vw_InventarioSucursal';
PRINT '';
PRINT 'Stored Procedures creados:';
PRINT '  ✓ sp_RegistrarTraspaso';
PRINT '  ✓ sp_EnviarTraspaso';
PRINT '  ✓ sp_RecibirTraspaso';
PRINT '  ✓ sp_CancelarTraspaso';
PRINT '  ✓ sp_ConsultarTraspasos';
PRINT '  ✓ sp_ObtenerDetalleTraspaso';
PRINT '';
PRINT 'Modificaciones:';
PRINT '  ✓ LotesProducto.SucursalID agregado';
PRINT '';
PRINT '========================================';
