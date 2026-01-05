-- ============================================================
-- Script: Actualización Cierre de Caja - Incluir Monto Inicial
-- Descripción: Modifica sp_CierreCajaConGastos para considerar
--              el monto inicial de apertura al calcular el efectivo en caja
-- Fecha: 2026-01-04
-- ============================================================

USE DB_TIENDA;
GO

PRINT '============================================================';
PRINT 'Actualizando Stored Procedure: sp_CierreCajaConGastos';
PRINT '============================================================';

-- Eliminar procedimiento existente
IF OBJECT_ID('sp_CierreCajaConGastos', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_CierreCajaConGastos;
    PRINT '✓ Procedimiento anterior eliminado';
END
GO

-- Crear procedimiento actualizado
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

PRINT '✓ Stored Procedure sp_CierreCajaConGastos actualizado exitosamente';
PRINT '';

-- ============================================================
-- Verificación
-- ============================================================
PRINT 'Verificando actualización...';

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_CierreCajaConGastos')
    PRINT '✓ SP sp_CierreCajaConGastos existe';
ELSE
    PRINT '✗ ERROR: SP sp_CierreCajaConGastos no se creó correctamente';

PRINT '';
PRINT '============================================================';
PRINT 'Script ejecutado exitosamente';
PRINT '============================================================';
PRINT '';
PRINT 'IMPORTANTE: El cierre de caja ahora incluye el monto inicial';
PRINT 'Fórmula: Efectivo en Caja = Monto Inicial + Ventas Efectivo - Gastos Efectivo - Retiros';
PRINT '';

-- Prueba del stored procedure
PRINT '============================================================';
PRINT 'Prueba del procedimiento con datos de hoy:';
PRINT '============================================================';

EXEC sp_CierreCajaConGastos @CajaID = 1, @Fecha = NULL;

PRINT '';
PRINT 'Si no hay datos, puede ser que no exista apertura de caja registrada.';
PRINT 'Verifique con: SELECT * FROM MovimientosCaja WHERE TipoMovimiento = ''APERTURA''';
