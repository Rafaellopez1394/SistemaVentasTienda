-- ========================================
-- SCRIPT: Módulo de Gastos Operativos
-- Permite registrar gastos del día y reflejarlos en cierre de caja
-- ========================================

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '========================================';
PRINT 'Instalando Módulo de Gastos';
PRINT '========================================';
PRINT '';

-- 1. Crear tabla de Categorías de Gastos
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CatCategoriasGastos')
BEGIN
    CREATE TABLE CatCategoriasGastos (
        CategoriaGastoID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Descripcion VARCHAR(300),
        RequiereAprobacion BIT DEFAULT 0,
        MontoMaximo DECIMAL(18,2) NULL,
        Activo BIT DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE()
    );
    
    -- Insertar categorías predefinidas
    INSERT INTO CatCategoriasGastos (Nombre, Descripcion, RequiereAprobacion, MontoMaximo)
    VALUES 
        ('Servicios', 'Luz, agua, gas, internet, teléfono', 0, NULL),
        ('Papelería', 'Hojas, bolígrafos, folders, etc.', 0, 500.00),
        ('Limpieza', 'Productos de limpieza, escobas, trapeadores', 0, 500.00),
        ('Mantenimiento', 'Reparaciones y mantenimiento de equipo', 0, 2000.00),
        ('Transporte', 'Gasolina, taxis, envíos', 0, 1000.00),
        ('Alimentación', 'Comidas del personal', 0, 300.00),
        ('Otros Gastos', 'Gastos no clasificados', 1, 1000.00);
    
    PRINT '✓ Tabla CatCategoriasGastos creada con 7 categorías';
END
ELSE
    PRINT '✓ Tabla CatCategoriasGastos ya existe';

GO

-- 2. Crear tabla principal de Gastos
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gastos')
BEGIN
    CREATE TABLE Gastos (
        GastoID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CajaID INT,
        CategoriaGastoID INT NOT NULL,
        Concepto VARCHAR(300) NOT NULL,
        Descripcion VARCHAR(500),
        Monto DECIMAL(18,2) NOT NULL,
        FechaGasto DATETIME NOT NULL DEFAULT GETDATE(),
        NumeroFactura VARCHAR(50),
        Proveedor VARCHAR(200),
        FormaPagoID INT,  -- Efectivo, transferencia, etc.
        
        -- Control de aprobación
        RequiereAprobacion BIT DEFAULT 0,
        EstaAprobado BIT DEFAULT 0,
        AprobadoPor VARCHAR(100),
        FechaAprobacion DATETIME,
        
        -- Auditoría
        UsuarioRegistro VARCHAR(100) NOT NULL,
        FechaRegistro DATETIME DEFAULT GETDATE(),
        Observaciones VARCHAR(500),
        
        -- Estado
        Activo BIT DEFAULT 1,
        Cancelado BIT DEFAULT 0,
        MotivoCancelacion VARCHAR(300),
        
        CONSTRAINT FK_Gastos_Categoria FOREIGN KEY (CategoriaGastoID) 
            REFERENCES CatCategoriasGastos(CategoriaGastoID),
        CONSTRAINT FK_Gastos_Caja FOREIGN KEY (CajaID)
            REFERENCES Cajas(CajaID),
        CONSTRAINT FK_Gastos_FormaPago FOREIGN KEY (FormaPagoID)
            REFERENCES CatFormasPago(FormaPagoID),
        CONSTRAINT CK_Gastos_Monto CHECK (Monto > 0)
    );
    
    CREATE INDEX IX_Gastos_Fecha ON Gastos(FechaGasto);
    CREATE INDEX IX_Gastos_Caja ON Gastos(CajaID);
    CREATE INDEX IX_Gastos_Categoria ON Gastos(CategoriaGastoID);
    CREATE INDEX IX_Gastos_Usuario ON Gastos(UsuarioRegistro);
    
    PRINT '✓ Tabla Gastos creada con índices';
END
ELSE
    PRINT '✓ Tabla Gastos ya existe';

GO

-- 3. Crear vista consolidada de gastos
IF OBJECT_ID('vw_GastosDetalle', 'V') IS NOT NULL
    DROP VIEW vw_GastosDetalle;
GO

CREATE VIEW vw_GastosDetalle AS
SELECT 
    g.GastoID,
    g.CajaID,
    g.Concepto,
    g.Descripcion,
    g.Monto,
    g.FechaGasto,
    g.NumeroFactura,
    g.Proveedor,
    
    -- Categoría
    cg.Nombre AS Categoria,
    cg.Descripcion AS CategoriaDescripcion,
    
    -- Forma de pago
    fp.Descripcion AS FormaPago,
    
    -- Aprobación
    g.RequiereAprobacion,
    g.EstaAprobado,
    g.AprobadoPor,
    g.FechaAprobacion,
    
    -- Estado
    CASE 
        WHEN g.Cancelado = 1 THEN 'CANCELADO'
        WHEN g.RequiereAprobacion = 1 AND g.EstaAprobado = 0 THEN 'PENDIENTE APROBACION'
        WHEN g.EstaAprobado = 1 OR g.RequiereAprobacion = 0 THEN 'APROBADO'
        ELSE 'PENDIENTE'
    END AS Estado,
    
    -- Auditoría
    g.UsuarioRegistro,
    g.FechaRegistro,
    g.Observaciones,
    g.Activo,
    g.Cancelado,
    g.MotivoCancelacion
FROM Gastos g
INNER JOIN CatCategoriasGastos cg ON g.CategoriaGastoID = cg.CategoriaGastoID
LEFT JOIN CatFormasPago fp ON g.FormaPagoID = fp.FormaPagoID;

GO

PRINT '✓ Vista vw_GastosDetalle creada';

-- 4. Stored Procedure: Registrar Gasto
IF OBJECT_ID('sp_RegistrarGasto', 'P') IS NOT NULL
    DROP PROCEDURE sp_RegistrarGasto;
GO

CREATE PROCEDURE sp_RegistrarGasto
    @CajaID INT = NULL,
    @CategoriaGastoID INT,
    @Concepto VARCHAR(300),
    @Descripcion VARCHAR(500) = NULL,
    @Monto DECIMAL(18,2),
    @NumeroFactura VARCHAR(50) = NULL,
    @Proveedor VARCHAR(200) = NULL,
    @FormaPagoID INT = NULL,
    @UsuarioRegistro VARCHAR(100),
    @Observaciones VARCHAR(500) = NULL,
    @GastoID UNIQUEIDENTIFIER OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar categoría
        IF NOT EXISTS (SELECT 1 FROM CatCategoriasGastos WHERE CategoriaGastoID = @CategoriaGastoID AND Activo = 1)
        BEGIN
            SET @Mensaje = 'La categoría de gasto no existe o está inactiva';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Obtener configuración de la categoría
        DECLARE @RequiereAprobacion BIT;
        DECLARE @MontoMaximo DECIMAL(18,2);
        
        SELECT @RequiereAprobacion = RequiereAprobacion, @MontoMaximo = MontoMaximo
        FROM CatCategoriasGastos
        WHERE CategoriaGastoID = @CategoriaGastoID;
        
        -- Validar monto máximo si aplica
        IF @MontoMaximo IS NOT NULL AND @Monto > @MontoMaximo
        BEGIN
            SET @RequiereAprobacion = 1;
        END
        
        -- Insertar gasto
        SET @GastoID = NEWID();
        
        INSERT INTO Gastos (
            GastoID, CajaID, CategoriaGastoID, Concepto, Descripcion, Monto,
            FechaGasto, NumeroFactura, Proveedor, FormaPagoID,
            RequiereAprobacion, EstaAprobado, UsuarioRegistro, Observaciones
        )
        VALUES (
            @GastoID, @CajaID, @CategoriaGastoID, @Concepto, @Descripcion, @Monto,
            GETDATE(), @NumeroFactura, @Proveedor, @FormaPagoID,
            @RequiereAprobacion, CASE WHEN @RequiereAprobacion = 0 THEN 1 ELSE 0 END,
            @UsuarioRegistro, @Observaciones
        );
        
        COMMIT TRANSACTION;
        
        SET @Mensaje = CASE 
            WHEN @RequiereAprobacion = 1 THEN 'Gasto registrado - Requiere aprobación'
            ELSE 'Gasto registrado exitosamente'
        END;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @Mensaje = 'Error: ' + ERROR_MESSAGE();
        SET @GastoID = NULL;
    END CATCH
END;
GO

PRINT '✓ Stored Procedure sp_RegistrarGasto creado';

-- 5. Stored Procedure: Obtener Gastos por Fecha
IF OBJECT_ID('sp_ObtenerGastosPorFecha', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerGastosPorFecha;
GO

CREATE PROCEDURE sp_ObtenerGastosPorFecha
    @FechaInicio DATE,
    @FechaFin DATE,
    @CajaID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT *
    FROM vw_GastosDetalle
    WHERE CAST(FechaGasto AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND (@CajaID IS NULL OR CajaID = @CajaID)
      AND Cancelado = 0
      AND Activo = 1
    ORDER BY FechaGasto DESC;
END;
GO

PRINT '✓ Stored Procedure sp_ObtenerGastosPorFecha creado';

-- 6. Stored Procedure: Resumen de Gastos
IF OBJECT_ID('sp_ResumenGastos', 'P') IS NOT NULL
    DROP PROCEDURE sp_ResumenGastos;
GO

CREATE PROCEDURE sp_ResumenGastos
    @FechaInicio DATE,
    @FechaFin DATE,
    @CajaID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Resumen por categoría
    SELECT 
        cg.Nombre AS Categoria,
        COUNT(g.GastoID) AS TotalGastos,
        SUM(g.Monto) AS MontoTotal,
        AVG(g.Monto) AS PromedioGasto,
        MIN(g.Monto) AS GastoMinimo,
        MAX(g.Monto) AS GastoMaximo
    FROM Gastos g
    INNER JOIN CatCategoriasGastos cg ON g.CategoriaGastoID = cg.CategoriaGastoID
    WHERE CAST(g.FechaGasto AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND (@CajaID IS NULL OR g.CajaID = @CajaID)
      AND g.Cancelado = 0
      AND g.Activo = 1
    GROUP BY cg.Nombre
    ORDER BY MontoTotal DESC;
    
    -- Total general
    SELECT 
        COUNT(GastoID) AS TotalGastos,
        SUM(Monto) AS MontoTotalGastos,
        AVG(Monto) AS PromedioGasto
    FROM Gastos
    WHERE CAST(FechaGasto AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND (@CajaID IS NULL OR CajaID = @CajaID)
      AND Cancelado = 0
      AND Activo = 1;
END;
GO

PRINT '✓ Stored Procedure sp_ResumenGastos creado';

-- 7. Stored Procedure: Cierre de Caja con Gastos
IF OBJECT_ID('sp_CierreCajaConGastos', 'P') IS NOT NULL
    DROP PROCEDURE sp_CierreCajaConGastos;
GO

CREATE PROCEDURE sp_CierreCajaConGastos
    @CajaID INT,
    @Fecha DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Fecha IS NULL
        SET @Fecha = CAST(GETDATE() AS DATE);
    
    -- Monto inicial de apertura de caja
    DECLARE @MontoInicial DECIMAL(18,2);
    
    SELECT @MontoInicial = ISNULL(Monto, 0)
    FROM MovimientosCaja
    WHERE CajaID = @CajaID
      AND TipoMovimiento = 'APERTURA'
      AND CAST(FechaMovimiento AS DATE) = @Fecha
    ORDER BY FechaMovimiento DESC;
    
    IF @MontoInicial IS NULL
        SET @MontoInicial = 0;
    
    -- Ventas del día
    DECLARE @TotalVentas DECIMAL(18,2);
    DECLARE @VentasEfectivo DECIMAL(18,2);
    DECLARE @VentasTarjeta DECIMAL(18,2);
    DECLARE @VentasTransferencia DECIMAL(18,2);
    
    SELECT 
        @TotalVentas = ISNULL(SUM(Total), 0),
        @VentasEfectivo = ISNULL(SUM(CASE WHEN fp.Descripcion = 'Efectivo' THEN Total ELSE 0 END), 0),
        @VentasTarjeta = ISNULL(SUM(CASE WHEN fp.Descripcion LIKE '%Tarjeta%' THEN Total ELSE 0 END), 0),
        @VentasTransferencia = ISNULL(SUM(CASE WHEN fp.Descripcion LIKE '%Transfer%' THEN Total ELSE 0 END), 0)
    FROM VentasClientes vc
    LEFT JOIN CatFormasPago fp ON vc.FormaPagoID = fp.FormaPagoID
    WHERE vc.CajaID = @CajaID
      AND CAST(vc.FechaVenta AS DATE) = @Fecha;
    
    -- Gastos del día
    DECLARE @TotalGastos DECIMAL(18,2);
    DECLARE @GastosEfectivo DECIMAL(18,2);
    
    SELECT 
        @TotalGastos = ISNULL(SUM(Monto), 0),
        @GastosEfectivo = ISNULL(SUM(CASE WHEN fp.Descripcion = 'Efectivo' THEN Monto ELSE 0 END), 0)
    FROM Gastos g
    LEFT JOIN CatFormasPago fp ON g.FormaPagoID = fp.FormaPagoID
    WHERE g.CajaID = @CajaID
      AND CAST(g.FechaGasto AS DATE) = @Fecha
      AND g.Cancelado = 0
      AND g.Activo = 1;
    
    -- Retiros de caja (si existen)
    DECLARE @TotalRetiros DECIMAL(18,2) = 0;
    
    -- Resultado del cierre
    SELECT 
        @CajaID AS CajaID,
        @Fecha AS Fecha,
        @MontoInicial AS MontoInicial,
        @TotalVentas AS TotalVentas,
        @VentasEfectivo AS VentasEfectivo,
        @VentasTarjeta AS VentasTarjeta,
        @VentasTransferencia AS VentasTransferencia,
        @TotalGastos AS TotalGastos,
        @GastosEfectivo AS GastosEfectivo,
        @TotalRetiros AS TotalRetiros,
        (@MontoInicial + @VentasEfectivo - @GastosEfectivo - @TotalRetiros) AS EfectivoEnCaja,
        (@TotalVentas - @TotalGastos - @TotalRetiros) AS GananciaNeta;
    
    -- Detalle de gastos
    SELECT 
        cg.Nombre AS Categoria,
        g.Concepto,
        g.Monto,
        g.FechaGasto,
        g.UsuarioRegistro
    FROM Gastos g
    INNER JOIN CatCategoriasGastos cg ON g.CategoriaGastoID = cg.CategoriaGastoID
    WHERE g.CajaID = @CajaID
      AND CAST(g.FechaGasto AS DATE) = @Fecha
      AND g.Cancelado = 0
      AND g.Activo = 1
    ORDER BY g.FechaGasto;
END;
GO

PRINT '✓ Stored Procedure sp_CierreCajaConGastos creado';

-- 8. Verificación final
PRINT '';
PRINT '========================================';
PRINT 'Verificando instalación:';
PRINT '========================================';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CatCategoriasGastos')
    PRINT '✓ Tabla CatCategoriasGastos';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gastos')
    PRINT '✓ Tabla Gastos';

IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_GastosDetalle')
    PRINT '✓ Vista vw_GastosDetalle';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_RegistrarGasto')
    PRINT '✓ SP sp_RegistrarGasto';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ObtenerGastosPorFecha')
    PRINT '✓ SP sp_ObtenerGastosPorFecha';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ResumenGastos')
    PRINT '✓ SP sp_ResumenGastos';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_CierreCajaConGastos')
    PRINT '✓ SP sp_CierreCajaConGastos';

-- Mostrar categorías creadas
PRINT '';
PRINT 'Categorías de Gastos disponibles:';
SELECT CategoriaGastoID, Nombre, MontoMaximo FROM CatCategoriasGastos WHERE Activo = 1;

PRINT '';
PRINT '========================================';
PRINT 'COMPLETADO: Módulo de Gastos instalado';
PRINT '========================================';
