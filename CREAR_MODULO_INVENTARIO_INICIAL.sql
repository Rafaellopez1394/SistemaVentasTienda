-- ============================================
-- MÓDULO DE INVENTARIO INICIAL
-- Para migración desde otro sistema
-- ============================================

USE DB_TIENDA;
GO

-- Tabla para registrar cargas de inventario inicial
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InventarioInicial')
BEGIN
    CREATE TABLE InventarioInicial (
        CargaID INT IDENTITY(1,1) PRIMARY KEY,
        FechaCarga DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioCarga VARCHAR(50) NOT NULL,
        TotalProductos INT NOT NULL,
        TotalUnidades DECIMAL(18,2) NOT NULL,
        ValorTotal DECIMAL(18,2) NOT NULL,
        Comentarios VARCHAR(500),
        SucursalID INT NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FOREIGN KEY (SucursalID) REFERENCES SUCURSAL(SucursalID)
    );
    PRINT '✓ Tabla InventarioInicial creada';
END
ELSE
    PRINT '- Tabla InventarioInicial ya existe';
GO

-- Tabla de detalle de cada producto en la carga inicial
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InventarioInicialDetalle')
BEGIN
    CREATE TABLE InventarioInicialDetalle (
        DetalleID INT IDENTITY(1,1) PRIMARY KEY,
        CargaID INT NOT NULL,
        ProductoID INT NOT NULL,
        CantidadCargada DECIMAL(18,2) NOT NULL,
        CostoUnitario DECIMAL(18,4) NOT NULL,
        PrecioVenta DECIMAL(18,4) NOT NULL,
        Comentarios VARCHAR(200),
        FechaCaducidad DATE NULL,
        FOREIGN KEY (CargaID) REFERENCES InventarioInicial(CargaID),
        FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID)
    );
    PRINT '✓ Tabla InventarioInicialDetalle creada';
END
ELSE
    PRINT '- Tabla InventarioInicialDetalle ya existe';
GO

-- Stored Procedure: Iniciar nueva carga de inventario
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_IniciarCargaInventarioInicial')
    DROP PROCEDURE SP_IniciarCargaInventarioInicial;
GO

CREATE PROCEDURE SP_IniciarCargaInventarioInicial
    @UsuarioCarga VARCHAR(50),
    @SucursalID INT,
    @Comentarios VARCHAR(500) = NULL,
    @CargaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la sucursal existe
        IF NOT EXISTS (SELECT 1 FROM SUCURSAL WHERE SucursalID = @SucursalID)
        BEGIN
            RAISERROR('La sucursal especificada no existe', 16, 1);
            RETURN;
        END
        
        -- Crear registro de carga
        INSERT INTO InventarioInicial (
            FechaCarga, 
            UsuarioCarga, 
            TotalProductos, 
            TotalUnidades, 
            ValorTotal,
            Comentarios,
            SucursalID
        )
        VALUES (
            GETDATE(), 
            @UsuarioCarga, 
            0, 
            0, 
            0,
            @Comentarios,
            @SucursalID
        );
        
        SET @CargaID = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        SELECT 
            @CargaID AS CargaID,
            'Carga de inventario inicial iniciada correctamente' AS Mensaje,
            1 AS Resultado;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        SELECT 
            0 AS CargaID,
            ERROR_MESSAGE() AS Mensaje,
            0 AS Resultado;
    END CATCH
END
GO

PRINT '✓ SP_IniciarCargaInventarioInicial creado';
GO

-- Stored Procedure: Agregar producto a carga de inventario inicial
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_AgregarProductoInventarioInicial')
    DROP PROCEDURE SP_AgregarProductoInventarioInicial;
GO

CREATE PROCEDURE SP_AgregarProductoInventarioInicial
    @CargaID INT,
    @ProductoID INT,
    @Cantidad DECIMAL(18,2),
    @CostoUnitario DECIMAL(18,4),
    @PrecioVenta DECIMAL(18,4),
    @FechaCaducidad DATE = NULL,
    @Comentarios VARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la carga existe y está activa
        IF NOT EXISTS (SELECT 1 FROM InventarioInicial WHERE CargaID = @CargaID AND Activo = 1)
        BEGIN
            RAISERROR('La carga de inventario no existe o ya fue finalizada', 16, 1);
            RETURN;
        END
        
        -- Verificar que el producto existe
        IF NOT EXISTS (SELECT 1 FROM Productos WHERE ProductoID = @ProductoID AND Estatus = 1)
        BEGIN
            RAISERROR('El producto no existe o está inactivo', 16, 1);
            RETURN;
        END
        
        -- Verificar si el producto ya fue agregado en esta carga
        IF EXISTS (SELECT 1 FROM InventarioInicialDetalle 
                   WHERE CargaID = @CargaID AND ProductoID = @ProductoID)
        BEGIN
            -- Actualizar la cantidad existente
            UPDATE InventarioInicialDetalle
            SET CantidadCargada = @Cantidad,
                CostoUnitario = @CostoUnitario,
                PrecioVenta = @PrecioVenta,
                FechaCaducidad = @FechaCaducidad,
                Comentarios = @Comentarios
            WHERE CargaID = @CargaID AND ProductoID = @ProductoID;
        END
        ELSE
        BEGIN
            -- Insertar nuevo producto
            INSERT INTO InventarioInicialDetalle (
                CargaID,
                ProductoID,
                CantidadCargada,
                CostoUnitario,
                PrecioVenta,
                FechaCaducidad,
                Comentarios
            )
            VALUES (
                @CargaID,
                @ProductoID,
                @Cantidad,
                @CostoUnitario,
                @PrecioVenta,
                @FechaCaducidad,
                @Comentarios
            );
        END
        
        COMMIT TRANSACTION;
        
        SELECT 
            'Producto agregado/actualizado correctamente' AS Mensaje,
            1 AS Resultado;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        SELECT 
            ERROR_MESSAGE() AS Mensaje,
            0 AS Resultado;
    END CATCH
END
GO

PRINT '✓ SP_AgregarProductoInventarioInicial creado';
GO

-- Stored Procedure: Finalizar y aplicar carga de inventario inicial
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_FinalizarCargaInventarioInicial')
    DROP PROCEDURE SP_FinalizarCargaInventarioInicial;
GO

CREATE PROCEDURE SP_FinalizarCargaInventarioInicial
    @CargaID INT,
    @Usuario VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SucursalID INT;
    DECLARE @TotalProductos INT;
    DECLARE @TotalUnidades DECIMAL(18,2);
    DECLARE @ValorTotal DECIMAL(18,2);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la carga existe y está activa
        IF NOT EXISTS (SELECT 1 FROM InventarioInicial WHERE CargaID = @CargaID AND Activo = 1)
        BEGIN
            RAISERROR('La carga de inventario no existe o ya fue finalizada', 16, 1);
            RETURN;
        END
        
        -- Obtener sucursal de la carga
        SELECT @SucursalID = SucursalID
        FROM InventarioInicial
        WHERE CargaID = @CargaID;
        
        -- Calcular totales
        SELECT 
            @TotalProductos = COUNT(*),
            @TotalUnidades = SUM(CantidadCargada),
            @ValorTotal = SUM(CantidadCargada * CostoUnitario)
        FROM InventarioInicialDetalle
        WHERE CargaID = @CargaID;
        
        -- Actualizar totales en la carga
        UPDATE InventarioInicial
        SET TotalProductos = @TotalProductos,
            TotalUnidades = @TotalUnidades,
            ValorTotal = @ValorTotal
        WHERE CargaID = @CargaID;
        
        -- Crear lotes en LotesProducto para cada producto
        INSERT INTO LotesProducto (
            ProductoID,
            FechaEntrada,
            CantidadTotal,
            CantidadDisponible,
            PrecioCompra,
            PrecioVenta,
            Usuario,
            UltimaAct,
            Estatus,
            FechaCaducidad,
            CantidadMerma,
            SucursalID
        )
        SELECT 
            d.ProductoID,
            GETDATE() AS FechaEntrada,
            d.CantidadCargada AS CantidadTotal,
            d.CantidadCargada AS CantidadDisponible,
            d.CostoUnitario AS PrecioCompra,
            d.PrecioVenta,
            @Usuario AS Usuario,
            GETDATE() AS UltimaAct,
            1 AS Estatus,
            d.FechaCaducidad,
            0 AS CantidadMerma,
            @SucursalID AS SucursalID
        FROM InventarioInicialDetalle d
        WHERE d.CargaID = @CargaID;
        
        -- Registrar movimientos en InventarioMovimientos
        INSERT INTO InventarioMovimientos (
            LoteID,
            ProductoID,
            TipoMovimiento,
            Cantidad,
            CostoUnitario,
            Usuario,
            Fecha,
            Comentarios
        )
        SELECT 
            l.LoteID,
            l.ProductoID,
            'INVENTARIO_INICIAL' AS TipoMovimiento,
            d.CantidadCargada AS Cantidad,
            d.CostoUnitario,
            @Usuario AS Usuario,
            GETDATE() AS Fecha,
            'Carga inicial de inventario #' + CAST(@CargaID AS VARCHAR) AS Comentarios
        FROM InventarioInicialDetalle d
        INNER JOIN LotesProducto l ON d.ProductoID = l.ProductoID
        WHERE d.CargaID = @CargaID
          AND l.FechaEntrada = (SELECT MAX(FechaEntrada) 
                                FROM LotesProducto 
                                WHERE ProductoID = l.ProductoID);
        
        -- Marcar la carga como finalizada (inactiva)
        UPDATE InventarioInicial
        SET Activo = 0
        WHERE CargaID = @CargaID;
        
        COMMIT TRANSACTION;
        
        SELECT 
            'Inventario inicial aplicado correctamente' AS Mensaje,
            @TotalProductos AS ProductosAplicados,
            @TotalUnidades AS UnidadesAplicadas,
            @ValorTotal AS ValorTotal,
            1 AS Resultado;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        SELECT 
            ERROR_MESSAGE() AS Mensaje,
            0 AS ProductosAplicados,
            0 AS UnidadesAplicadas,
            0 AS ValorTotal,
            0 AS Resultado;
    END CATCH
END
GO

PRINT '✓ SP_FinalizarCargaInventarioInicial creado';
GO

-- Stored Procedure: Obtener productos para carga inicial
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_ObtenerProductosParaInventarioInicial')
    DROP PROCEDURE SP_ObtenerProductosParaInventarioInicial;
GO

CREATE PROCEDURE SP_ObtenerProductosParaInventarioInicial
    @CargaID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si se proporciona CargaID, mostrar productos de esa carga
    IF @CargaID IS NOT NULL
    BEGIN
        SELECT 
            d.DetalleID,
            d.ProductoID,
            p.Nombre AS NombreProducto,
            p.CodigoInterno,
            d.CantidadCargada,
            d.CostoUnitario,
            d.PrecioVenta,
            d.FechaCaducidad,
            d.Comentarios,
            (d.CantidadCargada * d.CostoUnitario) AS ValorTotal
        FROM InventarioInicialDetalle d
        INNER JOIN Productos p ON d.ProductoID = p.ProductoID
        WHERE d.CargaID = @CargaID
        ORDER BY p.Nombre;
    END
    ELSE
    BEGIN
        -- Si no se proporciona CargaID, mostrar todos los productos disponibles
        SELECT 
            p.ProductoID,
            p.Nombre AS NombreProducto,
            p.CodigoInterno,
            ISNULL(SUM(l.CantidadDisponible), 0) AS StockActual,
            0 AS CantidadNueva,
            0.00 AS CostoUnitario,
            0.00 AS PrecioVenta
        FROM Productos p
        LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID AND l.Estatus = 1
        WHERE p.Estatus = 1
        GROUP BY p.ProductoID, p.Nombre, p.CodigoInterno
        ORDER BY p.Nombre;
    END
END
GO

PRINT '✓ SP_ObtenerProductosParaInventarioInicial creado';
GO

-- Stored Procedure: Eliminar producto de carga (antes de finalizar)
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_EliminarProductoInventarioInicial')
    DROP PROCEDURE SP_EliminarProductoInventarioInicial;
GO

CREATE PROCEDURE SP_EliminarProductoInventarioInicial
    @DetalleID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CargaID INT;
        
        -- Obtener CargaID
        SELECT @CargaID = CargaID
        FROM InventarioInicialDetalle
        WHERE DetalleID = @DetalleID;
        
        -- Verificar que la carga está activa
        IF NOT EXISTS (SELECT 1 FROM InventarioInicial WHERE CargaID = @CargaID AND Activo = 1)
        BEGIN
            RAISERROR('No se puede eliminar. La carga ya fue finalizada', 16, 1);
            RETURN;
        END
        
        -- Eliminar el detalle
        DELETE FROM InventarioInicialDetalle
        WHERE DetalleID = @DetalleID;
        
        COMMIT TRANSACTION;
        
        SELECT 
            'Producto eliminado correctamente' AS Mensaje,
            1 AS Resultado;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        SELECT 
            ERROR_MESSAGE() AS Mensaje,
            0 AS Resultado;
    END CATCH
END
GO

PRINT '✓ SP_EliminarProductoInventarioInicial creado';
GO

-- Vista para consultar historial de cargas
IF EXISTS (SELECT * FROM sys.views WHERE name = 'VW_HistorialInventarioInicial')
    DROP VIEW VW_HistorialInventarioInicial;
GO

CREATE VIEW VW_HistorialInventarioInicial
AS
SELECT 
    i.CargaID,
    i.FechaCarga,
    i.UsuarioCarga,
    s.Nombre AS NombreSucursal,
    i.TotalProductos,
    i.TotalUnidades,
    i.ValorTotal,
    i.Comentarios,
    CASE WHEN i.Activo = 1 THEN 'En Proceso' ELSE 'Finalizada' END AS Estado
FROM InventarioInicial i
INNER JOIN SUCURSAL s ON i.SucursalID = s.SucursalID;
GO

PRINT '✓ Vista VW_HistorialInventarioInicial creada';
GO

PRINT '';
PRINT '================================================';
PRINT '✓ MÓDULO DE INVENTARIO INICIAL CREADO';
PRINT '================================================';
PRINT 'Tablas creadas:';
PRINT '  - InventarioInicial (registro de cargas)';
PRINT '  - InventarioInicialDetalle (detalle de productos)';
PRINT '';
PRINT 'Stored Procedures creados:';
PRINT '  - SP_IniciarCargaInventarioInicial';
PRINT '  - SP_AgregarProductoInventarioInicial';
PRINT '  - SP_FinalizarCargaInventarioInicial';
PRINT '  - SP_ObtenerProductosParaInventarioInicial';
PRINT '  - SP_EliminarProductoInventarioInicial';
PRINT '';
PRINT 'Vista creada:';
PRINT '  - VW_HistorialInventarioInicial';
PRINT '================================================';
