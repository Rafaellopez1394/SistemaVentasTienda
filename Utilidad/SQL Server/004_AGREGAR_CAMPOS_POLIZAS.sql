-- Script para agregar campos obligatorios a la tabla Polizas
-- Cumplimiento SAT y sistema de contabilidad electrónica

USE DB_TIENDA
GO

-- Verificar y agregar campos si no existen
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'EsAutomatica')
BEGIN
    ALTER TABLE Polizas ADD EsAutomatica BIT NOT NULL DEFAULT 1
    PRINT 'Campo EsAutomatica agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'DocumentoOrigen')
BEGIN
    ALTER TABLE Polizas ADD DocumentoOrigen VARCHAR(100) NULL
    PRINT 'Campo DocumentoOrigen agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'TotalDebe')
BEGIN
    ALTER TABLE Polizas ADD TotalDebe DECIMAL(18,2) NOT NULL DEFAULT 0
    PRINT 'Campo TotalDebe agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'TotalHaber')
BEGIN
    ALTER TABLE Polizas ADD TotalHaber DECIMAL(18,2) NOT NULL DEFAULT 0
    PRINT 'Campo TotalHaber agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'Estatus')
BEGIN
    ALTER TABLE Polizas ADD Estatus VARCHAR(20) NOT NULL DEFAULT 'ABIERTA'
    PRINT 'Campo Estatus agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'PeriodoContable')
BEGIN
    ALTER TABLE Polizas ADD PeriodoContable VARCHAR(7) NOT NULL DEFAULT FORMAT(GETDATE(), 'yyyy-MM')
    PRINT 'Campo PeriodoContable agregado (formato YYYY-MM)'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'RelacionCFDI')
BEGIN
    ALTER TABLE Polizas ADD RelacionCFDI VARCHAR(36) NULL
    PRINT 'Campo RelacionCFDI agregado'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'Observaciones')
BEGIN
    ALTER TABLE Polizas ADD Observaciones VARCHAR(500) NULL
    PRINT 'Campo Observaciones agregado'
END

GO

-- Agregar columna calculada después de que existan TotalDebe y TotalHaber
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Polizas' AND COLUMN_NAME = 'EsCuadrada')
BEGIN
    ALTER TABLE Polizas ADD EsCuadrada AS (CASE WHEN ABS(TotalDebe - TotalHaber) < 0.01 THEN 1 ELSE 0 END) PERSISTED
    PRINT 'Columna calculada EsCuadrada agregada'
END

GO

-- Crear índices para optimizar consultas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Polizas_PeriodoContable')
BEGIN
    CREATE INDEX IX_Polizas_PeriodoContable ON Polizas(PeriodoContable)
    PRINT 'Índice IX_Polizas_PeriodoContable creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Polizas_Estatus')
BEGIN
    CREATE INDEX IX_Polizas_Estatus ON Polizas(Estatus)
    PRINT 'Índice IX_Polizas_Estatus creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Polizas_DocumentoOrigen')
BEGIN
    CREATE INDEX IX_Polizas_DocumentoOrigen ON Polizas(DocumentoOrigen)
    PRINT 'Índice IX_Polizas_DocumentoOrigen creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Polizas_RelacionCFDI')
BEGIN
    CREATE INDEX IX_Polizas_RelacionCFDI ON Polizas(RelacionCFDI) WHERE RelacionCFDI IS NOT NULL
    PRINT 'Índice IX_Polizas_RelacionCFDI creado'
END

GO

-- Crear tabla de catálogo de tipos de póliza
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatTiposPoliza')
BEGIN
    CREATE TABLE CatTiposPoliza (
        TipoPolizaID INT IDENTITY(1,1) PRIMARY KEY,
        Codigo VARCHAR(20) NOT NULL UNIQUE,
        Nombre VARCHAR(100) NOT NULL,
        Descripcion VARCHAR(255) NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE()
    )
    PRINT 'Tabla CatTiposPoliza creada'

    -- Insertar tipos de póliza estándar
    INSERT INTO CatTiposPoliza (Codigo, Nombre, Descripcion) VALUES
    ('INGRESO', 'Póliza de Ingresos', 'Ventas, cobros, ingresos varios'),
    ('EGRESO', 'Póliza de Egresos', 'Compras, pagos, gastos operativos'),
    ('DIARIO', 'Póliza de Diario', 'Ajustes contables, depreciaciones, mermas'),
    ('TRASPASO', 'Póliza de Traspaso', 'Movimientos entre cuentas bancarias')
    
    PRINT 'Tipos de póliza insertados'
END

GO

-- Crear tabla de periodos contables
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PeriodosContables')
BEGIN
    CREATE TABLE PeriodosContables (
        PeriodoID INT IDENTITY(1,1) PRIMARY KEY,
        Periodo VARCHAR(7) NOT NULL UNIQUE, -- YYYY-MM
        Anio INT NOT NULL,
        Mes INT NOT NULL,
        FechaInicio DATE NOT NULL,
        FechaFin DATE NOT NULL,
        Estatus VARCHAR(20) NOT NULL DEFAULT 'ABIERTO', -- ABIERTO, CERRADO
        FechaCierre DATETIME NULL,
        UsuarioCierre VARCHAR(100) NULL,
        TotalPolizas INT DEFAULT 0,
        Observaciones VARCHAR(500) NULL
    )
    PRINT 'Tabla PeriodosContables creada'
END

GO

-- Crear tabla de reglas contables automáticas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ReglasContablesAutomaticas')
BEGIN
    CREATE TABLE ReglasContablesAutomaticas (
        ReglaID INT IDENTITY(1,1) PRIMARY KEY,
        TipoOperacion VARCHAR(50) NOT NULL, -- VENTA_EFECTIVO, VENTA_TARJETA, COMPRA, AJUSTE_INVENTARIO
        SubTipo VARCHAR(50) NULL, -- MERMA, CADUCIDAD, etc.
        TipoPoliza VARCHAR(20) NOT NULL,
        OrdenMovimiento INT NOT NULL,
        TipoMovimiento VARCHAR(10) NOT NULL, -- DEBE, HABER
        CuentaID INT NOT NULL,
        CalculoMonto VARCHAR(200) NOT NULL, -- Formula o campo: TOTAL, SUBTOTAL, IVA, etc.
        Concepto VARCHAR(200) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FOREIGN KEY (CuentaID) REFERENCES CatalogoContable(CuentaID)
    )
    PRINT 'Tabla ReglasContablesAutomaticas creada'

    -- Insertar reglas básicas para ventas en efectivo
    INSERT INTO ReglasContablesAutomaticas (TipoOperacion, TipoPoliza, OrdenMovimiento, TipoMovimiento, CuentaID, CalculoMonto, Concepto) VALUES
    ('VENTA_EFECTIVO', 'INGRESO', 1, 'DEBE', 1, 'TOTAL', 'Ingreso en efectivo por venta'),
    ('VENTA_EFECTIVO', 'INGRESO', 2, 'HABER', 3, 'SUBTOTAL', 'Venta de mercancía'),
    ('VENTA_EFECTIVO', 'INGRESO', 3, 'HABER', 10, 'IVA', 'IVA cobrado')
    
    PRINT 'Reglas contables básicas insertadas'
END

GO

PRINT ''
PRINT '========================================='
PRINT 'Script ejecutado correctamente'
PRINT 'Campos agregados a tabla Polizas:'
PRINT '- EsAutomatica (bit)'
PRINT '- DocumentoOrigen (varchar)'
PRINT '- TotalDebe (decimal)'
PRINT '- TotalHaber (decimal)'
PRINT '- Estatus (varchar)'
PRINT '- PeriodoContable (varchar)'
PRINT '- RelacionCFDI (varchar)'
PRINT '- Observaciones (varchar)'
PRINT '- EsCuadrada (columna calculada)'
PRINT ''
PRINT 'Tablas creadas:'
PRINT '- CatTiposPoliza'
PRINT '- PeriodosContables'
PRINT '- ReglasContablesAutomaticas'
PRINT '========================================='
