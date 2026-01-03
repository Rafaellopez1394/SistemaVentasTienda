-- ============================================================
-- SCRIPT: Tablas para Módulo de Punto de Venta (POS)
-- Autor: Sistema
-- Fecha: 2025-12-08
-- ============================================================

USE DB_TIENDA
GO

-- ============================================================
-- 1. CATÁLOGO DE FORMAS DE PAGO
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatFormasPago')
BEGIN
    CREATE TABLE CatFormasPago (
        FormaPagoID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(5) NOT NULL UNIQUE,
        Descripcion VARCHAR(100) NOT NULL,
        RequiereCambio BIT DEFAULT 0, -- Si es efectivo y necesita calcular cambio
        Estatus BIT DEFAULT 1,
        FechaAlta DATETIME DEFAULT GETDATE(),
        UltimaAct DATETIME DEFAULT GETDATE()
    );

    -- Insertar formas de pago según SAT (usando N para Unicode)
    INSERT INTO CatFormasPago (Clave, Descripcion, RequiereCambio) VALUES
    ('01', N'Efectivo', 1),
    ('02', N'Cheque nominativo', 0),
    ('03', N'Transferencia electrónica de fondos', 0),
    ('04', N'Tarjeta de crédito', 0),
    ('28', N'Tarjeta de débito', 0),
    ('99', N'Por definir', 0);

    PRINT 'Tabla CatFormasPago creada e inicializada';
END
GO

-- ============================================================
-- 2. CATÁLOGO DE MÉTODOS DE PAGO
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatMetodosPago')
BEGIN
    CREATE TABLE CatMetodosPago (
        MetodoPagoID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(5) NOT NULL UNIQUE,
        Descripcion VARCHAR(100) NOT NULL,
        Estatus BIT DEFAULT 1
    );

    -- Insertar métodos de pago según SAT (usando N para Unicode)
    INSERT INTO CatMetodosPago (Clave, Descripcion) VALUES
    ('PUE', N'Pago en una sola exhibición'),
    ('PPD', N'Pago en parcialidades o diferido');

    PRINT 'Tabla CatMetodosPago creada e inicializada';
END
GO

-- ============================================================
-- 3. TABLA DE CAJAS
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'Cajas')
BEGIN
    CREATE TABLE Cajas (
        CajaID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        SucursalID INT NULL,
        Estatus BIT DEFAULT 1,
        FechaAlta DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (SucursalID) REFERENCES TIENDA(SucursalID)
    );

    -- Insertar caja principal
    INSERT INTO Cajas (Nombre, SucursalID) VALUES ('Caja Principal', 1);

    PRINT 'Tabla Cajas creada e inicializada';
END
GO

-- ============================================================
-- 4. TABLA DE MOVIMIENTOS DE CAJA
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'MovimientosCaja')
BEGIN
    CREATE TABLE MovimientosCaja (
        MovimientoCajaID INT IDENTITY(1,1) PRIMARY KEY,
        CajaID INT NOT NULL,
        TipoMovimiento VARCHAR(20) NOT NULL, -- APERTURA, VENTA, RETIRO, CIERRE
        FechaMovimiento DATETIME DEFAULT GETDATE(),
        Monto DECIMAL(18,2) NOT NULL,
        SaldoAnterior DECIMAL(18,2) NOT NULL,
        SaldoActual DECIMAL(18,2) NOT NULL,
        Concepto VARCHAR(200) NULL,
        VentaID UNIQUEIDENTIFIER NULL,
        Usuario VARCHAR(50) NOT NULL,
        FOREIGN KEY (CajaID) REFERENCES Cajas(CajaID)
    );

    PRINT 'Tabla MovimientosCaja creada';
END
GO

-- ============================================================
-- 5. AGREGAR CAMPOS A TABLA VENTASCLIENTES
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'TipoVenta')
BEGIN
    ALTER TABLE VentasClientes ADD TipoVenta VARCHAR(20) DEFAULT 'CONTADO'; -- CONTADO, CREDITO
    PRINT 'Campo TipoVenta agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'FormaPagoID')
BEGIN
    ALTER TABLE VentasClientes ADD FormaPagoID INT NULL;
    ALTER TABLE VentasClientes ADD FOREIGN KEY (FormaPagoID) REFERENCES CatFormasPago(FormaPagoID);
    PRINT 'Campo FormaPagoID agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'MetodoPagoID')
BEGIN
    ALTER TABLE VentasClientes ADD MetodoPagoID INT NULL;
    ALTER TABLE VentasClientes ADD FOREIGN KEY (MetodoPagoID) REFERENCES CatMetodosPago(MetodoPagoID);
    PRINT 'Campo MetodoPagoID agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'EfectivoRecibido')
BEGIN
    ALTER TABLE VentasClientes ADD EfectivoRecibido DECIMAL(18,2) NULL;
    PRINT 'Campo EfectivoRecibido agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'Cambio')
BEGIN
    ALTER TABLE VentasClientes ADD Cambio DECIMAL(18,2) NULL;
    PRINT 'Campo Cambio agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'Subtotal')
BEGIN
    ALTER TABLE VentasClientes ADD Subtotal DECIMAL(18,2) NULL;
    PRINT 'Campo Subtotal agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'IVA')
BEGIN
    ALTER TABLE VentasClientes ADD IVA DECIMAL(18,2) NULL;
    PRINT 'Campo IVA agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'RequiereFactura')
BEGIN
    ALTER TABLE VentasClientes ADD RequiereFactura BIT DEFAULT 0;
    PRINT 'Campo RequiereFactura agregado a VentasClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'CajaID')
BEGIN
    ALTER TABLE VentasClientes ADD CajaID INT NULL;
    ALTER TABLE VentasClientes ADD FOREIGN KEY (CajaID) REFERENCES Cajas(CajaID);
    PRINT 'Campo CajaID agregado a VentasClientes';
END
GO

-- ============================================================
-- 6. VERIFICAR TABLA VENTASDETALLECLIENTES
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasDetalleClientes') AND name = 'TasaIVA')
BEGIN
    ALTER TABLE VentasDetalleClientes ADD TasaIVA DECIMAL(5,2) DEFAULT 0;
    PRINT 'Campo TasaIVA agregado a VentasDetalleClientes';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasDetalleClientes') AND name = 'MontoIVA')
BEGIN
    ALTER TABLE VentasDetalleClientes ADD MontoIVA DECIMAL(18,2) DEFAULT 0;
    PRINT 'Campo MontoIVA agregado a VentasDetalleClientes';
END
GO

PRINT '============================================================';
PRINT 'TABLAS PARA MÓDULO POS CREADAS EXITOSAMENTE';
PRINT '============================================================';
