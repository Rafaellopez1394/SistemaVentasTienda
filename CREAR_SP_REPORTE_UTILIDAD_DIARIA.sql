-- =============================================
-- STORED PROCEDURE: sp_ReporteUtilidadDiaria
-- Genera reporte completo de utilidad diaria
-- Fecha: 25 de Enero de 2026
-- =============================================

USE DB_TIENDA;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ReporteUtilidadDiaria')
    DROP PROCEDURE sp_ReporteUtilidadDiaria;
GO

CREATE PROCEDURE sp_ReporteUtilidadDiaria
    @Fecha DATE,
    @SucursalID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Variables de totales
    DECLARE @TotalVentasContado DECIMAL(18,2) = 0;
    DECLARE @TotalVentasCredito DECIMAL(18,2) = 0;
    DECLARE @TotalTickets INT = 0;
    DECLARE @TotalUnidades INT = 0;
    DECLARE @CostosCompra DECIMAL(18,2) = 0;
    DECLARE @UtilidadDiaria DECIMAL(18,2) = 0;
    
    -- =============================================
    -- SECCIÓN 1: RESUMEN DE VENTAS POR FORMA DE PAGO
    -- =============================================
    SELECT 
        'RESUMEN DE VENTAS' AS Seccion,
        CASE 
            WHEN VC.TipoVenta = 'CONTADO' THEN 'Contado'
            WHEN VC.TipoVenta = 'CREDITO' THEN 'Crédito'
            ELSE 'Otros'
        END AS FormaPago,
        COUNT(DISTINCT VC.VentaID) AS Tickets,
        SUM(VD.Cantidad) AS TotalUnidades,
        SUM(VD.Cantidad * VD.PrecioVenta) AS TotalVentas,
        0 AS Monto,
        0 AS Cantidad,
        0 AS Valor
    FROM VentasClientes VC
    INNER JOIN VentasDetalleClientes VD ON VC.VentaID = VD.VentaID
    WHERE CAST(VC.FechaVenta AS DATE) = @Fecha
      AND (@SucursalID = 0 OR VC.SucursalID = @SucursalID)
      AND VC.Estado = 'VIGENTE'
    GROUP BY VC.TipoVenta
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 2: COSTOS DEL DÍA (Cotidiano)
    -- =============================================
    SELECT 
        'COSTOS' AS Seccion,
        'Costo de Mercancía Vendida' AS FormaPago,
        0 AS Tickets,
        SUM(VD.Cantidad) AS TotalUnidades,
        0 AS TotalVentas,
        SUM(VD.Cantidad * ISNULL(P.PrecioCompra, 0)) AS Monto,
        SUM(VD.Cantidad) AS Cantidad,
        0 AS Valor
    FROM VentasClientes VC
    INNER JOIN VentasDetalleClientes VD ON VC.VentaID = VD.VentaID
    INNER JOIN Producto P ON VD.ProductoID = P.ProductoID
    WHERE CAST(VC.FechaVenta AS DATE) = @Fecha
      AND (@SucursalID = 0 OR VC.SucursalID = @SucursalID)
      AND VC.Estado = 'VIGENTE'
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 3: UTILIDAD (Totales calculados)
    -- =============================================
    SELECT 
        'UTILIDAD' AS Seccion,
        'Totales' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        (SELECT SUM(VD.Cantidad * VD.PrecioVenta)
         FROM VentasClientes VC
         INNER JOIN VentasDetalleClientes VD ON VC.VentaID = VD.VentaID
         WHERE CAST(VC.FechaVenta AS DATE) = @Fecha
           AND VC.TipoVenta = 'CONTADO'
           AND VC.Estado = 'VIGENTE') AS TotalVentas,  -- TotalVentasContado
        (SELECT SUM(VD.Cantidad * VD.PrecioVenta)
         FROM VentasClientes VC
         INNER JOIN VentasDetalleClientes VD ON VC.VentaID = VD.VentaID
         WHERE CAST(VC.FechaVenta AS DATE) = @Fecha
           AND VC.TipoVenta = 'CREDITO'
           AND VC.Estado = 'VIGENTE') AS Monto,  -- TotalVentasCredito
        0 AS Cantidad,
        0 AS Valor
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 4: RECUPERACIÓN DE CRÉDITOS
    -- =============================================
    SELECT 
        'RECUPERACION' AS Seccion,
        'Recupero del Día' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        0 AS TotalVentas,
        ISNULL(SUM(CP.MontoPagado), 0) AS Monto,  -- MontoRecuperado
        0 AS Cantidad,
        0 AS Valor  -- CostoCredito (se puede calcular si hay relación)
    FROM ComplementoPago CP
    WHERE CAST(CP.FechaPago AS DATE) = @Fecha
      AND CP.Estado = 'VIGENTE'
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 5: INVENTARIO INICIAL (Placeholder)
    -- =============================================
    SELECT 
        'INVENTARIO' AS Seccion,
        P.Nombre AS FormaPago,  -- Producto
        0 AS Tickets,
        0 AS TotalUnidades,
        0 AS TotalVentas,
        0 AS Monto,
        ISNULL(PS.Stock, 0) AS Cantidad,  -- CantidadInicial
        ISNULL(PS.Stock * P.PrecioCompra, 0) AS Valor  -- Valor
    FROM ProductoSucursal PS
    INNER JOIN Producto P ON PS.ProductoID = P.ProductoID
    WHERE (@SucursalID = 0 OR PS.SucursalID = @SucursalID)
      AND PS.Stock > 0
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 6: ENTRADAS DEL DÍA (Compras)
    -- =============================================
    SELECT 
        'ENTRADAS' AS Seccion,
        P.Nombre AS FormaPago,  -- Producto
        0 AS Tickets,
        0 AS TotalUnidades,
        0 AS TotalVentas,
        0 AS Monto,
        SUM(CD.Cantidad) AS Cantidad,  -- Cantidad
        SUM(CD.Cantidad * CD.PrecioCompra) AS Valor  -- Valor
    FROM Compra C
    INNER JOIN CompraDetalle CD ON C.CompraID = CD.CompraID
    INNER JOIN Producto P ON CD.ProductoID = P.ProductoID
    WHERE CAST(C.FechaCompra AS DATE) = @Fecha
      AND C.Estado = 'VIGENTE'
    GROUP BY P.Nombre
    
    UNION ALL
    
    -- =============================================
    -- SECCIÓN 7: DETALLE DE VENTAS POR PRODUCTO
    -- =============================================
    SELECT 
        'DETALLE_VENTAS' AS Seccion,
        P.Nombre AS FormaPago,  -- Producto (campo usado como Producto)
        0 AS Tickets,
        SUM(CASE WHEN VC.TipoVenta = 'CONTADO' THEN VD.Cantidad ELSE 0 END) AS TotalUnidades,  -- VentasContado
        SUM(CASE WHEN VC.TipoVenta = 'CREDITO' THEN VD.Cantidad ELSE 0 END) AS TotalVentas,     -- VentasCredito (int)
        SUM(VD.Cantidad * VD.PrecioVenta) AS Monto,  -- TotalVentas (decimal)
        SUM(VD.Cantidad * ISNULL(P.PrecioCompra, 0)) AS Cantidad,  -- CostoTotal
        SUM((VD.PrecioVenta - ISNULL(P.PrecioCompra, 0)) * VD.Cantidad) AS Valor  -- Utilidad
    FROM VentasClientes VC
    INNER JOIN VentasDetalleClientes VD ON VC.VentaID = VD.VentaID
    INNER JOIN Producto P ON VD.ProductoID = P.ProductoID
    WHERE CAST(VC.FechaVenta AS DATE) = @Fecha
      AND (@SucursalID = 0 OR VC.SucursalID = @SucursalID)
      AND VC.Estado = 'VIGENTE'
    GROUP BY P.Nombre
    
    ORDER BY Seccion, FormaPago;
END
GO

-- =============================================
-- VERIFICAR CREACIÓN
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ReporteUtilidadDiaria')
BEGIN
    PRINT '✅ Stored Procedure sp_ReporteUtilidadDiaria creado exitosamente';
    PRINT '';
    PRINT 'PRUEBA:';
    PRINT 'EXEC sp_ReporteUtilidadDiaria @Fecha = ''2026-01-25'', @SucursalID = 1';
END
ELSE
BEGIN
    PRINT '❌ ERROR: No se pudo crear sp_ReporteUtilidadDiaria';
END
GO

-- =============================================
-- PRUEBA RÁPIDA (Comentar si no hay datos)
-- =============================================
-- EXEC sp_ReporteUtilidadDiaria @Fecha = '2026-01-25', @SucursalID = 1;
