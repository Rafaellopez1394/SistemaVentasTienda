-- ========================================
-- SCRIPT: Crear tablas y vistas de Cuentas por Pagar
-- Compatible con estructura actual de DB_TIENDA
-- ========================================

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '========================================';
PRINT 'Creando estructura de Cuentas por Pagar';
PRINT '========================================';

-- 1. Crear tabla CuentasPorPagar si no existe
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CuentasPorPagar')
BEGIN
    CREATE TABLE CuentasPorPagar (
        CuentaPorPagarID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ProveedorID UNIQUEIDENTIFIER NOT NULL,
        CompraID UNIQUEIDENTIFIER NULL,  -- Puede ser NULL si no viene de compra
        NumeroFactura VARCHAR(50),
        FechaFactura DATE NOT NULL,
        FechaVencimiento DATE NOT NULL,
        MontoTotal DECIMAL(18,2) NOT NULL,
        MontoPagado DECIMAL(18,2) DEFAULT 0,
        SaldoPendiente AS (MontoTotal - MontoPagado) PERSISTED,
        Estado VARCHAR(20) DEFAULT 'PENDIENTE',  -- PENDIENTE, PAGADO, VENCIDO
        Descripcion VARCHAR(500),
        Moneda VARCHAR(3) DEFAULT 'MXN',
        TipoCambio DECIMAL(18,4) DEFAULT 1.0,
        FechaRegistro DATETIME DEFAULT GETDATE(),
        UsuarioRegistro VARCHAR(100),
        Notas TEXT,
        CONSTRAINT FK_CuentasPorPagar_Proveedor FOREIGN KEY (ProveedorID) 
            REFERENCES Proveedores(ProveedorID)
    );
    
    CREATE INDEX IX_CuentasPorPagar_Proveedor ON CuentasPorPagar(ProveedorID);
    CREATE INDEX IX_CuentasPorPagar_Estado ON CuentasPorPagar(Estado);
    CREATE INDEX IX_CuentasPorPagar_FechaVencimiento ON CuentasPorPagar(FechaVencimiento);
    
    PRINT '✓ Tabla CuentasPorPagar creada';
END
ELSE
    PRINT '✓ Tabla CuentasPorPagar ya existe';

GO

-- 2. Crear tabla PagosProveedores si no existe
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PagosProveedores')
BEGIN
    CREATE TABLE PagosProveedores (
        PagoProveedorID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CuentaPorPagarID UNIQUEIDENTIFIER NOT NULL,
        FechaPago DATETIME NOT NULL,
        MontoPagado DECIMAL(18,2) NOT NULL,
        FormaPagoID INT,
        MetodoPagoID INT,
        Referencia VARCHAR(100),
        Banco VARCHAR(100),
        NumeroCheque VARCHAR(50),
        Observaciones VARCHAR(500),
        UsuarioRegistro VARCHAR(100),
        FechaRegistro DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_PagosProveedores_CuentaPorPagar FOREIGN KEY (CuentaPorPagarID) 
            REFERENCES CuentasPorPagar(CuentaPorPagarID)
    );
    
    CREATE INDEX IX_PagosProveedores_Cuenta ON PagosProveedores(CuentaPorPagarID);
    CREATE INDEX IX_PagosProveedores_Fecha ON PagosProveedores(FechaPago);
    
    PRINT '✓ Tabla PagosProveedores creada';
END
ELSE
    PRINT '✓ Tabla PagosProveedores ya existe';

GO

-- 3. Crear o actualizar vista vw_ResumenCuentasPorPagar
IF OBJECT_ID('vw_ResumenCuentasPorPagar', 'V') IS NOT NULL
    DROP VIEW vw_ResumenCuentasPorPagar;
GO

CREATE VIEW vw_ResumenCuentasPorPagar AS
SELECT 
    cpp.CuentaPorPagarID,
    cpp.ProveedorID,
    p.RazonSocial AS NombreProveedor,
    p.RFC AS RFCProveedor,
    cpp.NumeroFactura,
    cpp.FechaFactura,
    cpp.FechaVencimiento,
    cpp.MontoTotal,
    cpp.MontoPagado,
    cpp.SaldoPendiente,
    cpp.Estado,
    cpp.Descripcion,
    cpp.Moneda,
    CASE 
        WHEN cpp.SaldoPendiente <= 0 THEN 'PAGADO'
        WHEN cpp.FechaVencimiento < CAST(GETDATE() AS DATE) THEN 'VENCIDO'
        ELSE 'PENDIENTE'
    END AS EstadoActual,
    DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE()) AS DiasVencido,
    (SELECT COUNT(*) FROM PagosProveedores pp WHERE pp.CuentaPorPagarID = cpp.CuentaPorPagarID) AS NumeroPagos,
    (SELECT MAX(FechaPago) FROM PagosProveedores pp WHERE pp.CuentaPorPagarID = cpp.CuentaPorPagarID) AS UltimoPago,
    cpp.FechaRegistro,
    cpp.UsuarioRegistro
FROM CuentasPorPagar cpp
INNER JOIN Proveedores p ON cpp.ProveedorID = p.ProveedorID;

GO

PRINT '✓ Vista vw_ResumenCuentasPorPagar creada';

-- 4. Crear stored procedure para actualizar estado
IF OBJECT_ID('sp_ActualizarEstadoCuentaPorPagar', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarEstadoCuentaPorPagar;
GO

CREATE PROCEDURE sp_ActualizarEstadoCuentaPorPagar
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Actualizar estado a PAGADO para cuentas liquidadas
    UPDATE CuentasPorPagar
    SET Estado = 'PAGADO'
    WHERE SaldoPendiente <= 0.01 AND Estado <> 'PAGADO';
    
    -- Actualizar estado a VENCIDO para cuentas vencidas no pagadas
    UPDATE CuentasPorPagar
    SET Estado = 'VENCIDO'
    WHERE FechaVencimiento < CAST(GETDATE() AS DATE) 
      AND SaldoPendiente > 0.01 
      AND Estado <> 'VENCIDO';
    
    -- Actualizar estado a PENDIENTE para cuentas no vencidas
    UPDATE CuentasPorPagar
    SET Estado = 'PENDIENTE'
    WHERE FechaVencimiento >= CAST(GETDATE() AS DATE) 
      AND SaldoPendiente > 0.01 
      AND Estado <> 'PENDIENTE';
      
    SELECT 
        COUNT(*) AS TotalActualizadas,
        SUM(CASE WHEN Estado = 'PAGADO' THEN 1 ELSE 0 END) AS Pagadas,
        SUM(CASE WHEN Estado = 'VENCIDO' THEN 1 ELSE 0 END) AS Vencidas,
        SUM(CASE WHEN Estado = 'PENDIENTE' THEN 1 ELSE 0 END) AS Pendientes
    FROM CuentasPorPagar;
END;
GO

PRINT '✓ Stored Procedure sp_ActualizarEstadoCuentaPorPagar creado';

-- 5. Verificar creación
PRINT '';
PRINT '========================================';
PRINT 'Verificando objetos creados:';
PRINT '========================================';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CuentasPorPagar')
    PRINT '✓ Tabla CuentasPorPagar existe';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PagosProveedores')
    PRINT '✓ Tabla PagosProveedores existe';

IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_ResumenCuentasPorPagar')
    PRINT '✓ Vista vw_ResumenCuentasPorPagar existe';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_ActualizarEstadoCuentaPorPagar')
    PRINT '✓ Stored Procedure sp_ActualizarEstadoCuentaPorPagar existe';

PRINT '';
PRINT '========================================';
PRINT 'COMPLETADO: Estructura de Cuentas por Pagar instalada';
PRINT '========================================';
