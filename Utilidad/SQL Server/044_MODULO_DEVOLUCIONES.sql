-- ============================================================
-- Script: M\u00f3dulo de Devoluciones de Venta
-- Descripci\u00f3n: Sistema completo de devoluciones con reintegro a inventario
-- Fecha: 2026-01-04
-- ============================================================

USE DB_TIENDA;
GO

PRINT '============================================================';
PRINT 'M\u00d3DULO DE DEVOLUCIONES DE VENTA';
PRINT '============================================================';
PRINT '';

-- ============================================================
-- 1. Tabla de Devoluciones (Encabezado)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Devoluciones')
BEGIN
    CREATE TABLE Devoluciones (
        DevolucionID INT IDENTITY(1,1) PRIMARY KEY,
        VentaID UNIQUEIDENTIFIER NOT NULL,
        FechaDevolucion DATETIME NOT NULL DEFAULT GETDATE(),
        TipoDevolucion VARCHAR(20) NOT NULL, -- 'TOTAL' o 'PARCIAL'
        MotivoDevolucion VARCHAR(500) NOT NULL,
        TotalDevuelto DECIMAL(18,2) NOT NULL,
        FormaReintegro VARCHAR(20) NOT NULL, -- 'EFECTIVO', 'CREDITO_CLIENTE', 'CAMBIO_PRODUCTO'
        MontoReintegrado DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreditoGenerado DECIMAL(18,2) NOT NULL DEFAULT 0,
        UsuarioID INT NULL,
        UsuarioRegistro VARCHAR(100) NULL,
        SucursalID INT NOT NULL,
        Estatus BIT NOT NULL DEFAULT 1, -- 1=Activa, 0=Cancelada
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT FK_Devoluciones_Venta FOREIGN KEY (VentaID) REFERENCES VentasClientes(VentaID),
        CONSTRAINT FK_Devoluciones_Sucursal FOREIGN KEY (SucursalID) REFERENCES SUCURSAL(SucursalID),
        CONSTRAINT CK_TipoDevolucion CHECK (TipoDevolucion IN ('TOTAL', 'PARCIAL')),
        CONSTRAINT CK_FormaReintegro CHECK (FormaReintegro IN ('EFECTIVO', 'CREDITO_CLIENTE', 'CAMBIO_PRODUCTO'))
    );
    
    PRINT '\u2713 Tabla Devoluciones creada';
END
ELSE
BEGIN
    PRINT '\u2713 Tabla Devoluciones ya existe';
END
GO

-- ============================================================
-- 2. Tabla de Detalle de Devoluciones
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DevolucionesDetalle')
BEGIN
    CREATE TABLE DevolucionesDetalle (
        DevolucionDetalleID INT IDENTITY(1,1) PRIMARY KEY,
        DevolucionID INT NOT NULL,
        ProductoID INT NOT NULL,
        LoteID INT NULL,
        CantidadDevuelta DECIMAL(18,2) NOT NULL,
        PrecioVenta DECIMAL(18,2) NOT NULL,
        SubTotal DECIMAL(18,2) NOT NULL,
        ReingresadoInventario BIT NOT NULL DEFAULT 0,
        FechaReingreso DATETIME NULL,
        
        CONSTRAINT FK_DevolucionDetalle_Devolucion FOREIGN KEY (DevolucionID) REFERENCES Devoluciones(DevolucionID),
        CONSTRAINT FK_DevolucionDetalle_Producto FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),
        CONSTRAINT FK_DevolucionDetalle_Lote FOREIGN KEY (LoteID) REFERENCES LotesProducto(LoteID)
    );
    
    PRINT '\u2713 Tabla DevolucionesDetalle creada';
END
ELSE
BEGIN
    PRINT '\u2713 Tabla DevolucionesDetalle ya existe';
END
GO

-- ============================================================
-- 3. \u00cdndices para optimizaci\u00f3n
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Devoluciones_VentaID' AND object_id = OBJECT_ID('Devoluciones'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Devoluciones_VentaID ON Devoluciones(VentaID);
    PRINT '\u2713 \u00cdndice IX_Devoluciones_VentaID creado';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Devoluciones_Fecha' AND object_id = OBJECT_ID('Devoluciones'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Devoluciones_Fecha ON Devoluciones(FechaDevolucion DESC);
    PRINT '\u2713 \u00cdndice IX_Devoluciones_Fecha creado';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DevolucionDetalle_Producto' AND object_id = OBJECT_ID('DevolucionesDetalle'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DevolucionDetalle_Producto ON DevolucionesDetalle(ProductoID);
    PRINT '\u2713 \u00cdndice IX_DevolucionDetalle_Producto creado';
END
GO

-- ============================================================
-- 4. Stored Procedure: Registrar Devoluci\u00f3n
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_RegistrarDevolucion')
    DROP PROCEDURE sp_RegistrarDevolucion;
GO

CREATE PROCEDURE sp_RegistrarDevolucion
    @VentaID UNIQUEIDENTIFIER,
    @TipoDevolucion VARCHAR(20),
    @MotivoDevolucion VARCHAR(500),
    @FormaReintegro VARCHAR(20),
    @UsuarioRegistro VARCHAR(100),
    @SucursalID INT,
    @Productos NVARCHAR(MAX), -- JSON: [{"ProductoID":1,"LoteID":1,"Cantidad":2,"PrecioVenta":50}]
    @DevolucionID INT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que la venta existe
        IF NOT EXISTS (SELECT 1 FROM VentasClientes WHERE VentaID = @VentaID)
        BEGIN
            SET @Mensaje = 'La venta especificada no existe';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Calcular total de devoluci\u00f3n
        DECLARE @TotalDevuelto DECIMAL(18,2) = 0;
        DECLARE @ProductosTable TABLE (
            ProductoID INT,
            LoteID INT,
            Cantidad DECIMAL(18,2),
            PrecioVenta DECIMAL(18,2),
            SubTotal DECIMAL(18,2)
        );
        
        -- Parsear JSON de productos
        INSERT INTO @ProductosTable (ProductoID, LoteID, Cantidad, PrecioVenta, SubTotal)
        SELECT 
            ProductoID,
            LoteID,
            Cantidad,
            PrecioVenta,
            (Cantidad * PrecioVenta) AS SubTotal
        FROM OPENJSON(@Productos)
        WITH (
            ProductoID INT,
            LoteID INT,
            Cantidad DECIMAL(18,2),
            PrecioVenta DECIMAL(18,2)
        );
        
        SELECT @TotalDevuelto = SUM(SubTotal) FROM @ProductosTable;
        
        -- Insertar encabezado de devoluci\u00f3n
        INSERT INTO Devoluciones (
            VentaID, FechaDevolucion, TipoDevolucion, MotivoDevolucion,
            TotalDevuelto, FormaReintegro, 
            MontoReintegrado, CreditoGenerado,
            UsuarioRegistro, SucursalID, Estatus
        )
        VALUES (
            @VentaID, GETDATE(), @TipoDevolucion, @MotivoDevolucion,
            @TotalDevuelto, @FormaReintegro,
            CASE WHEN @FormaReintegro = 'EFECTIVO' THEN @TotalDevuelto ELSE 0 END,
            CASE WHEN @FormaReintegro = 'CREDITO_CLIENTE' THEN @TotalDevuelto ELSE 0 END,
            @UsuarioRegistro, @SucursalID, 1
        );
        
        SET @DevolucionID = SCOPE_IDENTITY();
        
        -- Insertar detalle de devoluci\u00f3n
        INSERT INTO DevolucionesDetalle (
            DevolucionID, ProductoID, LoteID, CantidadDevuelta,
            PrecioVenta, SubTotal, ReingresadoInventario, FechaReingreso
        )
        SELECT 
            @DevolucionID,
            ProductoID,
            LoteID,
            Cantidad,
            PrecioVenta,
            SubTotal,
            1, -- Marcar como reingresado
            GETDATE()
        FROM @ProductosTable;
        
        -- Reingresar productos al inventario (actualizar lotes)
        UPDATE l
        SET l.CantidadDisponible = l.CantidadDisponible + pt.Cantidad
        FROM LotesProducto l
        INNER JOIN @ProductosTable pt ON l.LoteID = pt.LoteID;
        
        -- Si es devoluci\u00f3n total, marcar venta como devuelta (opcional)
        IF @TipoDevolucion = 'TOTAL'
        BEGIN
            -- Puedes agregar un campo Devuelta en VentasClientes
            -- UPDATE VentasClientes SET Devuelta = 1 WHERE VentaID = @VentaID;
            SELECT 1; -- Placeholder
        END
        
        -- Si es cr\u00e9dito al cliente, registrar el cr\u00e9dito
        IF @FormaReintegro = 'CREDITO_CLIENTE'
        BEGIN
            -- Obtener ClienteID de la venta
            DECLARE @ClienteID INT;
            SELECT @ClienteID = ClienteID FROM VentasClientes WHERE VentaID = @VentaID;
            
            -- Crear un saldo a favor del cliente (puedes implementar una tabla CreditosCliente)
            -- Por ahora solo lo registramos en Devoluciones.CreditoGenerado
        END
        
        SET @Mensaje = 'Devoluci\u00f3n registrada exitosamente. ID: ' + CAST(@DevolucionID AS VARCHAR(10));
        COMMIT TRANSACTION;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SET @Mensaje = 'Error: ' + ERROR_MESSAGE();
        SET @DevolucionID = 0;
    END CATCH;
END
GO

PRINT '\u2713 Stored Procedure sp_RegistrarDevolucion creado';
GO

-- ============================================================
-- 5. Stored Procedure: Obtener Devoluciones con Filtros
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ObtenerDevoluciones')
    DROP PROCEDURE sp_ObtenerDevoluciones;
GO

CREATE PROCEDURE sp_ObtenerDevoluciones
    @FechaInicio DATETIME = NULL,
    @FechaFin DATETIME = NULL,
    @SucursalID INT = NULL,
    @VentaID UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DevolucionID,
        d.VentaID,
        CONCAT('V-', CAST(d.VentaID AS VARCHAR(10))) AS NumeroVenta,
        d.FechaDevolucion,
        d.TipoDevolucion,
        d.MotivoDevolucion,
        d.TotalDevuelto,
        d.FormaReintegro,
        d.MontoReintegrado,
        d.CreditoGenerado,
        d.UsuarioRegistro,
        s.Nombre AS NombreSucursal,
        c.Nombre AS NombreCliente,
        c.RFC AS RFCCliente,
        d.Estatus,
        d.FechaRegistro,
        -- Contar productos devueltos
        (SELECT COUNT(*) FROM DevolucionesDetalle WHERE DevolucionID = d.DevolucionID) AS TotalProductos
    FROM Devoluciones d
    INNER JOIN SUCURSAL s ON d.SucursalID = s.SucursalID
    INNER JOIN VentasClientes v ON d.VentaID = v.VentaID
    LEFT JOIN Cliente c ON v.ClienteID = c.ClienteID
    WHERE 
        (@FechaInicio IS NULL OR d.FechaDevolucion >= @FechaInicio)
        AND (@FechaFin IS NULL OR d.FechaDevolucion <= @FechaFin)
        AND (@SucursalID IS NULL OR d.SucursalID = @SucursalID)
        AND (@VentaID IS NULL OR d.VentaID = @VentaID)
    ORDER BY d.FechaDevolucion DESC;
END
GO

PRINT '\u2713 Stored Procedure sp_ObtenerDevoluciones creado';
GO

-- ============================================================
-- 6. Stored Procedure: Obtener Detalle de Devoluci\u00f3n
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ObtenerDetalleDevolucion')
    DROP PROCEDURE sp_ObtenerDetalleDevolucion;
GO

CREATE PROCEDURE sp_ObtenerDetalleDevolucion
    @DevolucionID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Encabezado
    SELECT 
        d.DevolucionID,
        d.VentaID,
        CONCAT('V-', CAST(d.VentaID AS VARCHAR(10))) AS NumeroVenta,
        d.FechaDevolucion,
        d.TipoDevolucion,
        d.MotivoDevolucion,
        d.TotalDevuelto,
        d.FormaReintegro,
        d.MontoReintegrado,
        d.CreditoGenerado,
        d.UsuarioRegistro,
        s.Nombre AS NombreSucursal,
        c.Nombre AS NombreCliente,
        c.RFC AS RFCCliente,
        d.Estatus
    FROM Devoluciones d
    INNER JOIN SUCURSAL s ON d.SucursalID = s.SucursalID
    INNER JOIN VentasClientes v ON d.VentaID = v.VentaID
    LEFT JOIN Cliente c ON v.ClienteID = c.ClienteID
    WHERE d.DevolucionID = @DevolucionID;
    
    -- Detalle
    SELECT 
        dd.DevolucionDetalleID,
        dd.ProductoID,
        p.CodigoInterno,
        p.Nombre AS NombreProducto,
        dd.CantidadDevuelta,
        dd.PrecioVenta,
        dd.SubTotal,
        dd.ReingresadoInventario,
        dd.FechaReingreso,
        l.NumeroLote
    FROM DevolucionesDetalle dd
    INNER JOIN Productos p ON dd.ProductoID = p.ProductoID
    LEFT JOIN LotesProducto l ON dd.LoteID = l.LoteID
    WHERE dd.DevolucionID = @DevolucionID;
END
GO

PRINT '\u2713 Stored Procedure sp_ObtenerDetalleDevolucion creado';
GO

-- ============================================================
-- 7. Reporte: Devoluciones por Per\u00edodo
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ReporteDevoluciones')
    DROP PROCEDURE sp_ReporteDevoluciones;
GO

CREATE PROCEDURE sp_ReporteDevoluciones
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @SucursalID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        -- Totales generales
        COUNT(*) AS TotalDevoluciones,
        SUM(CASE WHEN TipoDevolucion = 'TOTAL' THEN 1 ELSE 0 END) AS DevolucionesTotales,
        SUM(CASE WHEN TipoDevolucion = 'PARCIAL' THEN 1 ELSE 0 END) AS DevolucionesParciales,
        SUM(TotalDevuelto) AS MontoTotalDevuelto,
        SUM(MontoReintegrado) AS TotalEfectivoReintegrado,
        SUM(CreditoGenerado) AS TotalCreditoGenerado,
        
        -- Por forma de reintegro
        SUM(CASE WHEN FormaReintegro = 'EFECTIVO' THEN TotalDevuelto ELSE 0 END) AS DevolucionesEfectivo,
        SUM(CASE WHEN FormaReintegro = 'CREDITO_CLIENTE' THEN TotalDevuelto ELSE 0 END) AS DevolucionesCredito,
        SUM(CASE WHEN FormaReintegro = 'CAMBIO_PRODUCTO' THEN TotalDevuelto ELSE 0 END) AS DevolucionesCambio
    FROM Devoluciones
    WHERE FechaDevolucion >= @FechaInicio 
      AND FechaDevolucion <= @FechaFin
      AND (@SucursalID IS NULL OR SucursalID = @SucursalID)
      AND Estatus = 1;
END
GO

PRINT '\u2713 Stored Procedure sp_ReporteDevoluciones creado';
GO

-- ============================================================
-- 8. Productos M\u00e1s Devueltos
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ProductosMasDevueltos')
    DROP PROCEDURE sp_ProductosMasDevueltos;
GO

CREATE PROCEDURE sp_ProductosMasDevueltos
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @Top INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        p.ProductoID,
        p.CodigoInterno,
        p.Nombre AS NombreProducto,
        c.Nombre AS Categoria,
        SUM(dd.CantidadDevuelta) AS TotalDevuelto,
        COUNT(DISTINCT d.DevolucionID) AS NumeroDevoluciones,
        SUM(dd.SubTotal) AS MontoTotalDevuelto,
        
        -- Calcular % respecto a ventas
        (SELECT ISNULL(SUM(dv.Cantidad), 0)
         FROM VentasDetalleClientes dv
         INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
         WHERE dv.ProductoID = p.ProductoID
           AND v.FechaVenta >= @FechaInicio
           AND v.FechaVenta <= @FechaFin) AS TotalVendido,
           
        CASE 
            WHEN (SELECT ISNULL(SUM(dv.Cantidad), 0)
                  FROM VentasDetalleClientes dv
                  INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
                  WHERE dv.ProductoID = p.ProductoID
                    AND v.FechaVenta >= @FechaInicio
                    AND v.FechaVenta <= @FechaFin) > 0
            THEN (SUM(dd.CantidadDevuelta) * 100.0) / 
                 (SELECT SUM(dv.Cantidad)
                  FROM VentasDetalleClientes dv
                  INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
                  WHERE dv.ProductoID = p.ProductoID
                    AND v.FechaVenta >= @FechaInicio
                    AND v.FechaVenta <= @FechaFin)
            ELSE 0
        END AS PorcentajeDevolucion
    FROM DevolucionesDetalle dd
    INNER JOIN Devoluciones d ON dd.DevolucionID = d.DevolucionID
    INNER JOIN Productos p ON dd.ProductoID = p.ProductoID
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    WHERE d.FechaDevolucion >= @FechaInicio
      AND d.FechaDevolucion <= @FechaFin
      AND d.Estatus = 1
    GROUP BY p.ProductoID, p.CodigoInterno, p.Nombre, c.Nombre
    ORDER BY TotalDevuelto DESC;
END
GO

PRINT '\u2713 Stored Procedure sp_ProductosMasDevueltos creado';
GO

PRINT '';
PRINT '============================================================';
PRINT 'M\u00f3dulo de Devoluciones implementado exitosamente';
PRINT '============================================================';
PRINT '';
PRINT 'Tablas creadas:';
PRINT '  \u2713 Devoluciones';
PRINT '  \u2713 DevolucionesDetalle';
PRINT '';
PRINT 'Stored Procedures:';
PRINT '  \u2713 sp_RegistrarDevolucion';
PRINT '  \u2713 sp_ObtenerDevoluciones';
PRINT '  \u2713 sp_ObtenerDetalleDevolucion';
PRINT '  \u2713 sp_ReporteDevoluciones';
PRINT '  \u2713 sp_ProductosMasDevueltos';
PRINT '';
