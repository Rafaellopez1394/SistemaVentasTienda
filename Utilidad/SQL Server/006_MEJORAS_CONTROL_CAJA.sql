-- ============================================================
-- SCRIPT: 006_MEJORAS_CONTROL_CAJA.sql
-- DESCRIPCIÓN: Mejoras críticas al módulo de Control de Caja
-- AUTOR: Sistema
-- FECHA: 2025-12-13
-- ============================================================

USE DB_TIENDA;
GO

PRINT '=== INICIANDO MEJORAS CONTROL DE CAJA ===';
GO

-- ============================================================
-- 1. AGREGAR CAMPO CajaID A TABLA VentasClientes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'CajaID')
BEGIN
    ALTER TABLE VentasClientes
    ADD CajaID INT NULL;

    PRINT '✓ Campo CajaID agregado a tabla VentasClientes';
END
ELSE
BEGIN
    PRINT '✓ Campo CajaID ya existe en tabla VentasClientes';
END
GO

-- ============================================================
-- 2. AGREGAR FOREIGN KEY VentasClientes.CajaID → Cajas.CajaID
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VentasClientes_Cajas')
BEGIN
    ALTER TABLE VentasClientes
    ADD CONSTRAINT FK_VentasClientes_Cajas FOREIGN KEY (CajaID) REFERENCES Cajas(CajaID);

    PRINT '✓ Foreign Key FK_VentasClientes_Cajas creada';
END
GO

-- ============================================================
-- 3. CREAR TABLA CorteCaja (Registro de cierres)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CorteCaja')
BEGIN
    CREATE TABLE CorteCaja (
        CorteID INT IDENTITY(1,1) PRIMARY KEY,
        CajaID INT NOT NULL,
        FechaApertura DATETIME NOT NULL,
        FechaCierre DATETIME DEFAULT GETDATE(),
        
        -- Montos del sistema
        FondoInicial DECIMAL(18,2) NOT NULL,
        TotalVentas DECIMAL(18,2) NOT NULL,
        TotalRetiros DECIMAL(18,2) DEFAULT 0,
        TotalGastos DECIMAL(18,2) DEFAULT 0,
        MontoEsperado DECIMAL(18,2) NOT NULL, -- Calculado: Fondo + Ventas - Retiros - Gastos
        
        -- Montos reales (arqueo)
        MontoRealEfectivo DECIMAL(18,2) NULL,
        MontoRealTarjeta DECIMAL(18,2) NULL,
        MontoRealTransferencia DECIMAL(18,2) NULL,
        MontoRealTotal DECIMAL(18,2) NULL,
        
        -- Diferencias (faltantes/sobrantes)
        Diferencia DECIMAL(18,2) NULL, -- MontoRealTotal - MontoEsperado
        TipoDiferencia VARCHAR(20) NULL, -- FALTANTE, SOBRANTE, CUADRADO
        
        -- Información adicional
        Observaciones VARCHAR(500) NULL,
        UsuarioCierre VARCHAR(50) NOT NULL,
        PolizaID UNIQUEIDENTIFIER NULL, -- FK a Polizas (póliza de ingresos del día)
        
        FOREIGN KEY (CajaID) REFERENCES Cajas(CajaID),
        FOREIGN KEY (PolizaID) REFERENCES Polizas(PolizaID)
    );

    CREATE INDEX IX_CorteCaja_Fecha ON CorteCaja(FechaCierre);
    CREATE INDEX IX_CorteCaja_Caja ON CorteCaja(CajaID);

    PRINT '✓ Tabla CorteCaja creada';
END
ELSE
BEGIN
    PRINT '✓ Tabla CorteCaja ya existe';
END
GO

-- ============================================================
-- 4. STORED PROCEDURE: ValidarCajaAbierta
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ValidarCajaAbierta')
    DROP PROCEDURE ValidarCajaAbierta;
GO

CREATE PROCEDURE ValidarCajaAbierta
(
    @CajaID INT,
    @EstaAbierta BIT OUTPUT,
    @MovimientoAperturaID INT OUTPUT,
    @FechaApertura DATETIME OUTPUT,
    @SaldoActual DECIMAL(18,2) OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Buscar la última apertura
    SELECT TOP 1 
        @MovimientoAperturaID = MovimientoCajaID,
        @FechaApertura = FechaMovimiento,
        @SaldoActual = SaldoActual
    FROM MovimientosCaja
    WHERE CajaID = @CajaID 
      AND TipoMovimiento = 'APERTURA'
    ORDER BY MovimientoCajaID DESC;
    
    -- Verificar si existe un cierre posterior a esa apertura
    IF EXISTS (
        SELECT 1 FROM MovimientosCaja
        WHERE CajaID = @CajaID 
          AND TipoMovimiento = 'CIERRE'
          AND MovimientoCajaID > @MovimientoAperturaID
    )
    BEGIN
        SET @EstaAbierta = 0;
        SET @Mensaje = 'Caja cerrada. Debe realizar apertura antes de operar.';
    END
    ELSE IF @MovimientoAperturaID IS NULL
    BEGIN
        SET @EstaAbierta = 0;
        SET @Mensaje = 'No se ha realizado apertura de caja. Debe abrir caja primero.';
    END
    ELSE
    BEGIN
        SET @EstaAbierta = 1;
        SET @Mensaje = 'Caja abierta y operativa.';
        
        -- Actualizar saldo actual con el último movimiento
        SELECT TOP 1 @SaldoActual = SaldoActual
        FROM MovimientosCaja
        WHERE CajaID = @CajaID
        ORDER BY MovimientoCajaID DESC;
    END
END
GO

PRINT '✓ SP ValidarCajaAbierta creado';
GO

-- ============================================================
-- 5. STORED PROCEDURE: CierreCajaCompleto (versión mejorada)
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'CierreCajaCompleto')
    DROP PROCEDURE CierreCajaCompleto;
GO

CREATE PROCEDURE CierreCajaCompleto
(
    @CajaID INT,
    @Usuario VARCHAR(50),
    @MontoRealEfectivo DECIMAL(18,2),
    @MontoRealTarjeta DECIMAL(18,2),
    @MontoRealTransferencia DECIMAL(18,2),
    @Observaciones VARCHAR(500) = NULL,
    @SaldoFinal DECIMAL(18,2) OUTPUT,
    @TotalVentas DECIMAL(18,2) OUTPUT,
    @Diferencia DECIMAL(18,2) OUTPUT,
    @CorteID INT OUTPUT,
    @Mensaje VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @FondoInicial DECIMAL(18,2);
        DECLARE @FechaApertura DATETIME;
        DECLARE @MontoEsperado DECIMAL(18,2);
        DECLARE @MontoRealTotal DECIMAL(18,2);
        DECLARE @TipoDiferencia VARCHAR(20);
        
        -- Obtener saldo actual
        SELECT TOP 1 @SaldoFinal = SaldoActual
        FROM MovimientosCaja
        WHERE CajaID = @CajaID
        ORDER BY MovimientoCajaID DESC;
        
        -- Obtener fondo inicial de la última apertura
        SELECT TOP 1 
            @FondoInicial = Monto,
            @FechaApertura = FechaMovimiento
        FROM MovimientosCaja
        WHERE CajaID = @CajaID 
          AND TipoMovimiento = 'APERTURA'
        ORDER BY MovimientoCajaID DESC;
        
        -- Obtener total de ventas del turno
        SELECT @TotalVentas = ISNULL(SUM(Monto), 0)
        FROM MovimientosCaja
        WHERE CajaID = @CajaID 
          AND TipoMovimiento = 'VENTA'
          AND FechaMovimiento >= @FechaApertura;
        
        -- Calcular monto esperado (simplificado: Fondo + Ventas)
        SET @MontoEsperado = @FondoInicial + @TotalVentas;
        
        -- Calcular monto real total
        SET @MontoRealTotal = @MontoRealEfectivo + @MontoRealTarjeta + @MontoRealTransferencia;
        
        -- Calcular diferencia
        SET @Diferencia = @MontoRealTotal - @MontoEsperado;
        
        -- Determinar tipo de diferencia
        IF ABS(@Diferencia) < 0.01
            SET @TipoDiferencia = 'CUADRADO';
        ELSE IF @Diferencia < 0
            SET @TipoDiferencia = 'FALTANTE';
        ELSE
            SET @TipoDiferencia = 'SOBRANTE';
        
        -- Registrar corte en tabla CorteCaja
        INSERT INTO CorteCaja (
            CajaID, FechaApertura, FechaCierre,
            FondoInicial, TotalVentas, TotalRetiros, TotalGastos, MontoEsperado,
            MontoRealEfectivo, MontoRealTarjeta, MontoRealTransferencia, MontoRealTotal,
            Diferencia, TipoDiferencia, Observaciones, UsuarioCierre
        )
        VALUES (
            @CajaID, @FechaApertura, GETDATE(),
            @FondoInicial, @TotalVentas, 0, 0, @MontoEsperado,
            @MontoRealEfectivo, @MontoRealTarjeta, @MontoRealTransferencia, @MontoRealTotal,
            @Diferencia, @TipoDiferencia, @Observaciones, @Usuario
        );
        
        SET @CorteID = SCOPE_IDENTITY();
        
        -- Registrar movimiento de cierre
        INSERT INTO MovimientosCaja (
            CajaID, TipoMovimiento, FechaMovimiento,
            Monto, SaldoAnterior, SaldoActual,
            Concepto, Usuario
        )
        VALUES (
            @CajaID, 'CIERRE', GETDATE(),
            0, @SaldoFinal, 0,
            'Cierre de caja - Corte #' + CAST(@CorteID AS VARCHAR(10)), @Usuario
        );
        
        SET @Mensaje = 'Caja cerrada exitosamente. ' + @TipoDiferencia + ': $' + CAST(ABS(@Diferencia) AS VARCHAR(20));
        
    END TRY
    BEGIN CATCH
        SET @Mensaje = ERROR_MESSAGE();
        SET @CorteID = 0;
    END CATCH
END
GO

PRINT '✓ SP CierreCajaCompleto creado';
GO

-- ============================================================
-- 6. STORED PROCEDURE: ObtenerEstadoCaja
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ObtenerEstadoCaja')
    DROP PROCEDURE ObtenerEstadoCaja;
GO

CREATE PROCEDURE ObtenerEstadoCaja
(
    @CajaID INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UltimaApertura INT;
    DECLARE @UltimoCierre INT;
    DECLARE @EstaAbierta BIT = 0;
    DECLARE @SaldoActual DECIMAL(18,2) = 0;
    DECLARE @FechaApertura DATETIME;
    DECLARE @NumVentas INT = 0;
    DECLARE @TotalVentas DECIMAL(18,2) = 0;
    
    -- Obtener ID última apertura
    SELECT TOP 1 @UltimaApertura = MovimientoCajaID, @FechaApertura = FechaMovimiento
    FROM MovimientosCaja
    WHERE CajaID = @CajaID AND TipoMovimiento = 'APERTURA'
    ORDER BY MovimientoCajaID DESC;
    
    -- Obtener ID último cierre
    SELECT TOP 1 @UltimoCierre = MovimientoCajaID
    FROM MovimientosCaja
    WHERE CajaID = @CajaID AND TipoMovimiento = 'CIERRE'
    ORDER BY MovimientoCajaID DESC;
    
    -- Determinar si está abierta
    IF @UltimaApertura IS NOT NULL AND (@UltimoCierre IS NULL OR @UltimaApertura > @UltimoCierre)
    BEGIN
        SET @EstaAbierta = 1;
        
        -- Obtener saldo actual
        SELECT TOP 1 @SaldoActual = SaldoActual
        FROM MovimientosCaja
        WHERE CajaID = @CajaID
        ORDER BY MovimientoCajaID DESC;
        
        -- Contar ventas del turno
        SELECT 
            @NumVentas = COUNT(*),
            @TotalVentas = ISNULL(SUM(Monto), 0)
        FROM MovimientosCaja
        WHERE CajaID = @CajaID 
          AND TipoMovimiento = 'VENTA'
          AND FechaMovimiento >= @FechaApertura;
    END
    
    -- Retornar estado
    SELECT 
        @CajaID AS CajaID,
        @EstaAbierta AS EstaAbierta,
        @FechaApertura AS FechaApertura,
        @SaldoActual AS SaldoActual,
        @NumVentas AS NumeroVentas,
        @TotalVentas AS TotalVentas;
END
GO

PRINT '✓ SP ObtenerEstadoCaja creado';
GO

-- ============================================================
-- 7. ACTUALIZAR SP RegistrarVenta PARA INCLUIR CajaID
-- ============================================================
-- Nota: Este SP ya existe, se recomienda modificarlo manualmente
-- para agregar parámetro @CajaID y actualizar INSERT
PRINT '⚠ PENDIENTE: Modificar SP RegistrarVenta para incluir @CajaID';
GO

PRINT '';
PRINT '=== SCRIPT COMPLETADO EXITOSAMENTE ===';
PRINT 'Próximos pasos:';
PRINT '1. Actualizar CD_VentaPOS con nuevos métodos';
PRINT '2. Modificar VentaPOSController para validar caja';
PRINT '3. Crear frontend de apertura/cierre con arqueo';
GO
