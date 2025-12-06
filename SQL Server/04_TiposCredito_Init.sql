-- ====================================================================
-- SCRIPT: Inicializar Tipos de Crédito
-- DESCRIPCIÓN: Crea tablas maestras para gestión de tipos de crédito
-- AUTOR: Sistema de Ventas Tienda
-- FECHA: 2024
-- ====================================================================

-- ====================================================================
-- 1. CREAR TABLA DE TIPOS DE CRÉDITO (MAESTRO)
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TiposCredito')
BEGIN
    CREATE TABLE TiposCredito (
        TipoCreditoID INT PRIMARY KEY IDENTITY(1,1),
        Codigo NVARCHAR(10) UNIQUE NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(500),
        Criterio NVARCHAR(20) NOT NULL, -- 'Dinero', 'Producto', 'Tiempo'
        Icono NVARCHAR(50),
        Activo BIT NOT NULL DEFAULT 1,
        Usuario NVARCHAR(100),
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UltimaAct DATETIME NOT NULL DEFAULT GETDATE()
    );

    -- Crear índices por separado (si no existen)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TiposCredito_Codigo' AND object_id = OBJECT_ID('TiposCredito'))
        CREATE INDEX IX_TiposCredito_Codigo ON TiposCredito(Codigo);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TiposCredito_Criterio' AND object_id = OBJECT_ID('TiposCredito'))
        CREATE INDEX IX_TiposCredito_Criterio ON TiposCredito(Criterio);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TiposCredito_Activo' AND object_id = OBJECT_ID('TiposCredito'))
        CREATE INDEX IX_TiposCredito_Activo ON TiposCredito(Activo);

    PRINT 'Tabla TiposCredito creada exitosamente';
END
ELSE
    PRINT 'Tabla TiposCredito ya existe';

-- ====================================================================
-- 2. CREAR TABLA DE CRÉDITOS ASIGNADOS A CLIENTES
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ClienteTiposCredito')
BEGIN
    CREATE TABLE ClienteTiposCredito (
        ClienteTipoCreditoID INT PRIMARY KEY IDENTITY(1,1),
        ClienteID UNIQUEIDENTIFIER NOT NULL,
        TipoCreditoID INT NOT NULL,
        LimiteDinero DECIMAL(18,2) NULL,
        LimiteProducto INT NULL,
        PlazoDias INT NULL,
        FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaVencimiento DATETIME NULL,
        Estatus BIT NOT NULL DEFAULT 1, -- 1=Activo, 0=Suspendido
        SaldoUtilizado DECIMAL(18,2) DEFAULT 0,
        Usuario NVARCHAR(100),
        UltimaAct DATETIME NOT NULL DEFAULT GETDATE(),

        -- Relaciones
        FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
        FOREIGN KEY (TipoCreditoID) REFERENCES TiposCredito(TipoCreditoID),

        -- Restricciones
        UNIQUE(ClienteID, TipoCreditoID)
    );

    -- Índices para mejorar consultas
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ClienteTiposCredito_ClienteID' AND object_id = OBJECT_ID('ClienteTiposCredito'))
        CREATE INDEX IX_ClienteTiposCredito_ClienteID ON ClienteTiposCredito(ClienteID);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ClienteTiposCredito_TipoCreditoID' AND object_id = OBJECT_ID('ClienteTiposCredito'))
        CREATE INDEX IX_ClienteTiposCredito_TipoCreditoID ON ClienteTiposCredito(TipoCreditoID);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ClienteTiposCredito_Estatus' AND object_id = OBJECT_ID('ClienteTiposCredito'))
        CREATE INDEX IX_ClienteTiposCredito_Estatus ON ClienteTiposCredito(Estatus);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ClienteTiposCredito_FechaVencimiento' AND object_id = OBJECT_ID('ClienteTiposCredito'))
        CREATE INDEX IX_ClienteTiposCredito_FechaVencimiento ON ClienteTiposCredito(FechaVencimiento);

    PRINT 'Tabla ClienteTiposCredito creada exitosamente';
END
ELSE
    PRINT 'Tabla ClienteTiposCredito ya existe';

-- ====================================================================
-- 3. CREAR TABLA DE HISTORIAL DE CAMBIOS DE CRÉDITO
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HistorialCreditoCliente')
BEGIN
    CREATE TABLE HistorialCreditoCliente (
        HistorialID INT PRIMARY KEY IDENTITY(1,1),
        ClienteTipoCreditoID INT NOT NULL,
        Operacion NVARCHAR(50) NOT NULL, -- 'ASIGNAR', 'ACTUALIZAR', 'SUSPENDER', 'REACTIVAR'
        LimiteAnterior DECIMAL(18,2) NULL,
        LimiteNuevo DECIMAL(18,2) NULL,
        SaldoAnterior DECIMAL(18,2) NULL,
        SaldoNuevo DECIMAL(18,2) NULL,
        Razon NVARCHAR(500),
        UsuarioOperacion NVARCHAR(100),
        FechaOperacion DATETIME NOT NULL DEFAULT GETDATE(),

        -- Relación
        FOREIGN KEY (ClienteTipoCreditoID) REFERENCES ClienteTiposCredito(ClienteTipoCreditoID)
    );

    -- Índices auxiliar
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HistorialCredito_ClienteTipoCreditoID' AND object_id = OBJECT_ID('HistorialCreditoCliente'))
        CREATE INDEX IX_HistorialCredito_ClienteTipoCreditoID ON HistorialCreditoCliente(ClienteTipoCreditoID);

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HistorialCredito_FechaOperacion' AND object_id = OBJECT_ID('HistorialCreditoCliente'))
        CREATE INDEX IX_HistorialCredito_FechaOperacion ON HistorialCreditoCliente(FechaOperacion);

    PRINT 'Tabla HistorialCreditoCliente creada exitosamente';
END
ELSE
    PRINT 'Tabla HistorialCreditoCliente ya existe';

-- ====================================================================
-- 4. INSERTAR DATOS MAESTROS DE TIPOS DE CRÉDITO
-- ====================================================================
IF NOT EXISTS (SELECT * FROM TiposCredito WHERE Codigo = 'CR001')
BEGIN
    INSERT INTO TiposCredito (Codigo, Nombre, Descripcion, Criterio, Icono, Activo, Usuario)
    VALUES 
    ('CR001', 'Crédito por Dinero', 'Límite de crédito en pesos con validación de saldo', 'Dinero', 'fa-dollar-sign', 1, 'system'),
    ('CR002', 'Crédito por Producto', 'Límite de crédito en unidades de producto', 'Producto', 'fa-box', 1, 'system'),
    ('CR003', 'Crédito a Plazo', 'Crédito con plazo fijo en días, auto vencimiento', 'Tiempo', 'fa-calendar', 1, 'system');
    
    PRINT 'Tipos de Crédito maestros insertados exitosamente';
END
ELSE
    PRINT 'Tipos de Crédito ya existen';

-- ====================================================================
-- 5. CREAR TRIGGER PARA ACTUALIZAR FechaVencimiento
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'TR_ClienteTiposCredito_CalcularVencimiento')
BEGIN
    CREATE TRIGGER TR_ClienteTiposCredito_CalcularVencimiento
    ON ClienteTiposCredito
    AFTER INSERT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Actualizar FechaVencimiento para créditos de tipo "Tiempo"
        UPDATE ctc
        SET FechaVencimiento = DATEADD(DAY, ctc.PlazoDias, i.FechaAsignacion),
            UltimaAct = GETDATE()
        FROM ClienteTiposCredito ctc
        INNER JOIN inserted i ON ctc.ClienteTipoCreditoID = i.ClienteTipoCreditoID
        INNER JOIN TiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
        WHERE tc.Criterio = 'Tiempo' 
          AND ctc.PlazoDias IS NOT NULL
          AND ctc.FechaVencimiento IS NULL;
    END
    
    PRINT 'Trigger TR_ClienteTiposCredito_CalcularVencimiento creado';
END
ELSE
    PRINT 'Trigger ya existe';

-- ====================================================================
-- 6. CREAR PROCEDIMIENTO PARA REGISTRAR CAMBIOS DE CRÉDITO
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_RegistrarHistorialCredito')
BEGIN
    CREATE PROCEDURE SP_RegistrarHistorialCredito
        @ClienteTipoCreditoID INT,
        @Operacion NVARCHAR(50),
        @LimiteAnterior DECIMAL(18,2) = NULL,
        @LimiteNuevo DECIMAL(18,2) = NULL,
        @SaldoAnterior DECIMAL(18,2) = NULL,
        @SaldoNuevo DECIMAL(18,2) = NULL,
        @Razon NVARCHAR(500) = NULL,
        @Usuario NVARCHAR(100) = 'system'
    AS
    BEGIN
        SET NOCOUNT ON;
        
        INSERT INTO HistorialCreditoCliente 
        (ClienteTipoCreditoID, Operacion, LimiteAnterior, LimiteNuevo, SaldoAnterior, SaldoNuevo, Razon, UsuarioOperacion)
        VALUES (@ClienteTipoCreditoID, @Operacion, @LimiteAnterior, @LimiteNuevo, @SaldoAnterior, @SaldoNuevo, @Razon, @Usuario);
    END
    
    PRINT 'Procedimiento SP_RegistrarHistorialCredito creado';
END
ELSE
    PRINT 'Procedimiento ya existe';

-- ====================================================================
-- 7. CREAR PROCEDIMIENTO PARA OBTENER CLIENTES EN ALERTA DE CRÉDITO
-- ====================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_ObtenerClientesEnAlerta')
BEGIN
    CREATE PROCEDURE SP_ObtenerClientesEnAlerta
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            c.ClienteID,
            c.RazonSocial,
            tc.Codigo AS TipoCredigo,
            tc.Nombre AS TipoCredito,
            ctc.LimiteDinero,
            ctc.SaldoUtilizado,
            (ctc.LimiteDinero - ctc.SaldoUtilizado) AS SaldoDisponible,
            CAST((ctc.SaldoUtilizado * 100.0 / ctc.LimiteDinero) AS DECIMAL(5,2)) AS PorcentajeUtilizado,
            CASE 
                WHEN (ctc.SaldoUtilizado * 100.0 / ctc.LimiteDinero) >= 100 THEN 'CRÍTICO'
                WHEN (ctc.SaldoUtilizado * 100.0 / ctc.LimiteDinero) >= 80 THEN 'ALERTA'
                ELSE 'NORMAL'
            END AS Estado,
            ctc.FechaVencimiento,
            CASE WHEN ctc.FechaVencimiento < GETDATE() THEN 'VENCIDO' ELSE 'VIGENTE' END AS EstatusVencimiento
        FROM ClienteTiposCredito ctc
        INNER JOIN Clientes c ON ctc.ClienteID = c.ClienteID
        INNER JOIN TiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
        WHERE ctc.Estatus = 1
          AND ctc.LimiteDinero > 0
          AND (ctc.SaldoUtilizado >= (ctc.LimiteDinero * 0.8) OR ctc.FechaVencimiento < GETDATE())
        ORDER BY (ctc.SaldoUtilizado * 100.0 / ctc.LimiteDinero) DESC;
    END
    
    PRINT 'Procedimiento SP_ObtenerClientesEnAlerta creado';
END
ELSE
    PRINT 'Procedimiento ya existe';

-- ====================================================================
-- CONFIRMACIÓN FINAL
-- ====================================================================
PRINT '===============================================';
PRINT 'Inicialización de Tipos de Crédito Completada';
PRINT '===============================================';
PRINT 'Tablas creadas: TiposCredito, ClienteTiposCredito, HistorialCreditoCliente';
PRINT 'Datos maestros: 3 tipos de crédito insertados';
PRINT 'Triggers y procedimientos almacenados creados';
PRINT '===============================================';

-- Verificar contenido
SELECT '--- TIPOS DE CRÉDITO MAESTROS ---' AS Información;
SELECT TipoCreditoID, Codigo, Nombre, Criterio, Activo FROM TiposCredito ORDER BY Codigo;

PRINT '';
PRINT '--- REGISTRO VERIFICADO ---';
PRINT 'Sistema listo para asignar créditos a clientes';
