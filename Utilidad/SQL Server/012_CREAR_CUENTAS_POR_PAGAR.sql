-- ==================================================
-- Script: 012_CREAR_CUENTAS_POR_PAGAR.sql
-- Descripción: Módulo de Cuentas por Pagar a Proveedores
-- Fecha: 2024
-- ==================================================

USE DB_TIENDA
GO

-- ==================================================
-- 1. TABLA CUENTAS POR PAGAR
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CuentasPorPagar]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CuentasPorPagar] (
        [CuentaPorPagarID] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [CompraID] INT NOT NULL,
        [ProveedorID] INT NOT NULL,
        [FechaRegistro] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaVencimiento] DATE NOT NULL,
        [MontoTotal] DECIMAL(18,2) NOT NULL,
        [SaldoPendiente] DECIMAL(18,2) NOT NULL,
        [Estado] VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE', -- PENDIENTE, PARCIAL, PAGADA, VENCIDA
        [DiasCredito] INT NOT NULL DEFAULT 0,
        [FolioFactura] VARCHAR(50) NULL,
        [Observaciones] VARCHAR(500) NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [UsuarioCreacion] INT NULL,
        
        CONSTRAINT FK_CuentasPorPagar_Compra FOREIGN KEY (CompraID) REFERENCES COMPRA(CompraID),
        CONSTRAINT FK_CuentasPorPagar_Proveedor FOREIGN KEY (ProveedorID) REFERENCES PROVEEDOR(ProveedorID),
        CONSTRAINT FK_CuentasPorPagar_Usuario FOREIGN KEY (UsuarioCreacion) REFERENCES USUARIO(IdUsuario),
        CONSTRAINT CK_CuentasPorPagar_Estado CHECK (Estado IN ('PENDIENTE', 'PARCIAL', 'PAGADA', 'VENCIDA'))
    )
    
    PRINT 'Tabla CuentasPorPagar creada exitosamente'
END
GO

-- ==================================================
-- 2. TABLA PAGOS PROVEEDORES
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PagosProveedores]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PagosProveedores] (
        [PagoID] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [CuentaPorPagarID] UNIQUEIDENTIFIER NOT NULL,
        [FechaPago] DATETIME NOT NULL DEFAULT GETDATE(),
        [MontoPagado] DECIMAL(18,2) NOT NULL,
        [FormaPago] VARCHAR(50) NOT NULL, -- EFECTIVO, TRANSFERENCIA, CHEQUE
        [Referencia] VARCHAR(100) NULL, -- Número de cheque, referencia transferencia, etc.
        [CuentaBancaria] VARCHAR(50) NULL,
        [Observaciones] VARCHAR(500) NULL,
        [PolizaID] UNIQUEIDENTIFIER NULL, -- FK a Polizas (generada automáticamente)
        [UsuarioRegistro] INT NULL,
        [FechaRegistro] DATETIME NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT FK_PagosProveedores_CuentaPorPagar FOREIGN KEY (CuentaPorPagarID) REFERENCES CuentasPorPagar(CuentaPorPagarID),
        CONSTRAINT FK_PagosProveedores_Poliza FOREIGN KEY (PolizaID) REFERENCES Polizas(PolizaID),
        CONSTRAINT FK_PagosProveedores_Usuario FOREIGN KEY (UsuarioRegistro) REFERENCES USUARIO(IdUsuario)
    )
    
    PRINT 'Tabla PagosProveedores creada exitosamente'
END
GO

-- ==================================================
-- 3. ÍNDICES
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CuentasPorPagar_Proveedor')
    CREATE INDEX IX_CuentasPorPagar_Proveedor ON CuentasPorPagar(ProveedorID)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CuentasPorPagar_Estado')
    CREATE INDEX IX_CuentasPorPagar_Estado ON CuentasPorPagar(Estado)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CuentasPorPagar_Vencimiento')
    CREATE INDEX IX_CuentasPorPagar_Vencimiento ON CuentasPorPagar(FechaVencimiento)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PagosProveedores_CuentaPorPagar')
    CREATE INDEX IX_PagosProveedores_CuentaPorPagar ON PagosProveedores(CuentaPorPagarID)
GO

-- ==================================================
-- 4. VISTA RESUMEN CUENTAS POR PAGAR
-- ==================================================
IF OBJECT_ID('vw_ResumenCuentasPorPagar', 'V') IS NOT NULL
    DROP VIEW vw_ResumenCuentasPorPagar
GO

CREATE VIEW vw_ResumenCuentasPorPagar AS
SELECT 
    cpp.CuentaPorPagarID,
    cpp.CompraID,
    cpp.ProveedorID,
    p.RazonSocial AS Proveedor,
    p.RFC,
    cpp.FechaRegistro,
    cpp.FechaVencimiento,
    DATEDIFF(DAY, GETDATE(), cpp.FechaVencimiento) AS DiasParaVencer,
    CASE 
        WHEN cpp.Estado = 'PAGADA' THEN 0
        WHEN GETDATE() > cpp.FechaVencimiento THEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE())
        ELSE 0 
    END AS DiasVencido,
    cpp.MontoTotal,
    cpp.SaldoPendiente,
    cpp.Estado,
    cpp.FolioFactura,
    ISNULL(SUM(pp.MontoPagado), 0) AS TotalPagado,
    COUNT(pp.PagoID) AS NumeroPagos
FROM CuentasPorPagar cpp
INNER JOIN PROVEEDOR p ON cpp.ProveedorID = p.ProveedorID
LEFT JOIN PagosProveedores pp ON cpp.CuentaPorPagarID = pp.CuentaPorPagarID
WHERE cpp.Activo = 1
GROUP BY 
    cpp.CuentaPorPagarID, cpp.CompraID, cpp.ProveedorID, p.RazonSocial, p.RFC,
    cpp.FechaRegistro, cpp.FechaVencimiento, cpp.MontoTotal, cpp.SaldoPendiente,
    cpp.Estado, cpp.FolioFactura
GO

PRINT 'Vista vw_ResumenCuentasPorPagar creada exitosamente'
GO

-- ==================================================
-- 5. STORED PROCEDURE: Actualizar Estado Cuenta
-- ==================================================
IF OBJECT_ID('sp_ActualizarEstadoCuenta', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarEstadoCuenta
GO

CREATE PROCEDURE sp_ActualizarEstadoCuenta
    @CuentaPorPagarID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SaldoPendiente DECIMAL(18,2)
    DECLARE @FechaVencimiento DATE
    DECLARE @NuevoEstado VARCHAR(20)
    
    SELECT 
        @SaldoPendiente = SaldoPendiente,
        @FechaVencimiento = FechaVencimiento
    FROM CuentasPorPagar
    WHERE CuentaPorPagarID = @CuentaPorPagarID
    
    -- Determinar estado
    IF @SaldoPendiente <= 0
        SET @NuevoEstado = 'PAGADA'
    ELSE IF @SaldoPendiente < (SELECT MontoTotal FROM CuentasPorPagar WHERE CuentaPorPagarID = @CuentaPorPagarID)
        SET @NuevoEstado = 'PARCIAL'
    ELSE IF GETDATE() > @FechaVencimiento
        SET @NuevoEstado = 'VENCIDA'
    ELSE
        SET @NuevoEstado = 'PENDIENTE'
    
    UPDATE CuentasPorPagar
    SET Estado = @NuevoEstado
    WHERE CuentaPorPagarID = @CuentaPorPagarID
END
GO

PRINT 'Stored Procedure sp_ActualizarEstadoCuenta creado exitosamente'
GO

-- ==================================================
-- 6. PERMISOS Y CONFIGURACIÓN
-- ==================================================

-- Configurar días de crédito por proveedor (campo ya existe en PROVEEDOR)
-- Si no existe el campo DiasCredito en PROVEEDOR, agregarlo:
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PROVEEDOR]') AND name = 'DiasCredito')
BEGIN
    ALTER TABLE PROVEEDOR ADD DiasCredito INT NOT NULL DEFAULT 30
    PRINT 'Campo DiasCredito agregado a PROVEEDOR'
END
GO

PRINT '=============================================='
PRINT 'Script 012_CREAR_CUENTAS_POR_PAGAR completado'
PRINT 'Módulo de Cuentas por Pagar instalado'
PRINT '=============================================='
GO
