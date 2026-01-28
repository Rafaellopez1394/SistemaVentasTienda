-- ============================================================
-- SCRIPT: Reporte de Utilidad Diaria
-- Autor: GitHub Copilot
-- Fecha: 2025-01-24
-- Descripción: Calcula ventas, costos, utilidad y recupero de créditos por día
-- ============================================================

USE DB_TIENDA
GO

-- ============================================================
-- 1. CREAR STORED PROCEDURE: sp_ReporteUtilidadDiaria
-- ============================================================
IF OBJECT_ID('sp_ReporteUtilidadDiaria', 'P') IS NOT NULL
    DROP PROCEDURE sp_ReporteUtilidadDiaria
GO

CREATE PROCEDURE sp_ReporteUtilidadDiaria
    @Fecha DATE = NULL,
    @SucursalID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si no se proporciona fecha, usar la fecha actual
    IF @Fecha IS NULL
        SET @Fecha = CAST(GETDATE() AS DATE);
    
    DECLARE @FechaInicio DATETIME = CAST(@Fecha AS DATETIME);
    DECLARE @FechaFin DATETIME = CAST(@Fecha AS DATETIME) + '23:59:59.999';
    
    -- ============================================================
    -- SECCIÓN 1: RESUMEN DE VENTAS POR FORMA DE PAGO
    -- ============================================================
    SELECT
        'RESUMEN DE VENTAS' AS Seccion,
        COALESCE(cfp.Descripcion, 'SIN ESPECIFICAR') AS FormaPago,
        COUNT(DISTINCT vc.VentaID) AS Tickets,
        ISNULL(SUM(vc.Total), 0) AS TotalVentas,
        ISNULL(SUM(ISNULL(dvd.Cantidad, 0)), 0) AS TotalUnidades
    FROM VentasClientes vc
    LEFT JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    LEFT JOIN CatFormasPago cfp ON vc.FormaPagoID = cfp.FormaPagoID
    WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
        AND vc.TipoVenta = 'CONTADO'
    GROUP BY cfp.FormaPagoID, cfp.Descripcion
    
    UNION ALL
    
    -- Créditos como forma de pago separada
    SELECT
        'RESUMEN DE VENTAS' AS Seccion,
        'CREDITOS' AS FormaPago,
        COUNT(DISTINCT vc.VentaID) AS Tickets,
        ISNULL(SUM(vc.Total), 0) AS TotalVentas,
        ISNULL(SUM(ISNULL(dvd.Cantidad, 0)), 0) AS TotalUnidades
    FROM VentasClientes vc
    LEFT JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
        AND vc.TipoVenta = 'CREDITO'
    GROUP BY vc.TipoVenta
    
    ORDER BY Seccion, FormaPago;
    
    -- ============================================================
    -- SECCIÓN 2: COSTOS DE COMPRA DE LO VENDIDO (COGS)
    -- ============================================================
    SELECT
        'COSTOS DE COMPRA' AS Seccion,
        'TOTAL COGS' AS Descripcion,
        ISNULL(SUM(dvd.Cantidad * lp.PrecioCompra), 0) AS Monto,
        ISNULL(SUM(dvd.Cantidad), 0) AS Unidades
    FROM VentasClientes vc
    INNER JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    LEFT JOIN LotesProducto lp ON dvd.LoteID = lp.LoteID
    WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
        AND vc.TipoVenta = 'CONTADO';
    
    -- COGS para créditos
    SELECT
        'COSTOS CREDITO' AS Seccion,
        'TOTAL COGS CREDITO' AS Descripcion,
        ISNULL(SUM(dvd.Cantidad * lp.PrecioCompra), 0) AS Monto,
        ISNULL(SUM(dvd.Cantidad), 0) AS Unidades
    FROM VentasClientes vc
    INNER JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    LEFT JOIN LotesProducto lp ON dvd.LoteID = lp.LoteID
    WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
        AND vc.TipoVenta = 'CREDITO';
    
    -- ============================================================
    -- SECCIÓN 3: UTILIDAD DEL DÍA
    -- ============================================================
    DECLARE @VentasContado DECIMAL(18,2) = (
        SELECT ISNULL(SUM(vc.Total), 0)
        FROM VentasClientes vc
        WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
            AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
            AND vc.TipoVenta = 'CONTADO'
    );
    
    DECLARE @CostosContado DECIMAL(18,2) = (
        SELECT ISNULL(SUM(dvd.Cantidad * lp.PrecioCompra), 0)
        FROM VentasClientes vc
        INNER JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
        LEFT JOIN LotesProducto lp ON dvd.LoteID = lp.LoteID
        WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
            AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
            AND vc.TipoVenta = 'CONTADO'
    );
    
    DECLARE @UtilidadContado DECIMAL(18,2) = @VentasContado - @CostosContado;
    
    SELECT
        'UTILIDAD' AS Seccion,
        'TOTAL VENTAS CONTADO' AS Descripcion,
        @VentasContado AS Monto,
        NULL AS Unidades
    
    UNION ALL
    
    SELECT
        'UTILIDAD' AS Seccion,
        '(-) COSTOS COMPRA' AS Descripcion,
        -@CostosContado AS Monto,
        NULL AS Unidades
    
    UNION ALL
    
    SELECT
        'UTILIDAD' AS Seccion,
        '(=) UTILIDAD DEL DÍA' AS Descripcion,
        @UtilidadContado AS Monto,
        NULL AS Unidades;
    
    -- ============================================================
    -- SECCIÓN 4: RECUPERACIÓN DE CRÉDITOS
    -- ============================================================
    SELECT
        'RECUPERACION' AS Seccion,
        'REC CREDITO' AS Descripcion,
        ISNULL(SUM(pc.MontoPago), 0) AS Monto,
        COUNT(DISTINCT vc.VentaID) AS Unidades
    FROM PagosClientes pc
    INNER JOIN VentasClientes vc ON pc.VentaID = vc.VentaID
    WHERE CAST(pc.FechaPago AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0);
    
    -- COSTO DE CRÉDITO RECUPERADO
    SELECT
        'RECUPERACION' AS Seccion,
        '(-) COSTO CREDITO' AS Descripcion,
        -ISNULL(SUM(dvd.Cantidad * lp.PrecioCompra), 0) AS Monto,
        NULL AS Unidades
    FROM PagosClientes pc
    INNER JOIN VentasClientes vc ON pc.VentaID = vc.VentaID
    INNER JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    LEFT JOIN LotesProducto lp ON dvd.LoteID = lp.LoteID
    WHERE CAST(pc.FechaPago AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0);
    
    -- ============================================================
    -- SECCIÓN 5: INVENTARIO INICIAL (Saldo anterior al día)
    -- ============================================================
    SELECT
        'INVENTARIO INICIAL' AS Seccion,
        p.Nombre AS Producto,
        ISNULL(SUM(lp.CantidadDisponible), 0) AS Cantidad,
        ISNULL(SUM(lp.CantidadDisponible * lp.PrecioCompra), 0) AS Valor
    FROM LotesProducto lp
    INNER JOIN Productos p ON lp.ProductoID = p.ProductoID
    WHERE lp.SucursalID = @SucursalID
        AND lp.CantidadDisponible > 0
    GROUP BY p.ProductoID, p.Nombre
    ORDER BY p.Nombre;
    
    -- ============================================================
    -- SECCIÓN 6: ENTRADAS DEL DÍA
    -- ============================================================
    SELECT
        'ENTRADAS' AS Seccion,
        p.Nombre AS Producto,
        ISNULL(SUM(c.Cantidad), 0) AS Cantidad,
        ISNULL(SUM(c.Cantidad * c.PrecioCompra), 0) AS Valor
    FROM DetalleCompras c
    INNER JOIN Compras comp ON c.CompraID = comp.CompraID
    INNER JOIN Productos p ON c.ProductoID = p.ProductoID
    WHERE CAST(comp.FechaRegistro AS DATE) = @Fecha
        AND comp.SucursalID = @SucursalID
    GROUP BY p.ProductoID, p.Nombre
    ORDER BY p.Nombre;
    
    -- ============================================================
    -- SECCIÓN 7: DETALLE DE VENTAS POR PRODUCTO (TALLA)
    -- ============================================================
    SELECT
        'DETALLE VENTAS' AS Seccion,
        p.Nombre AS Producto,
        ISNULL(SUM(CASE WHEN vc.TipoVenta = 'CONTADO' THEN dvd.Cantidad ELSE 0 END), 0) AS VentasContado,
        ISNULL(SUM(CASE WHEN vc.TipoVenta = 'CREDITO' THEN dvd.Cantidad ELSE 0 END), 0) AS VentasCredito,
        ISNULL(SUM(dvd.Cantidad), 0) AS TotalVentas,
        ISNULL(SUM(dvd.Cantidad * lp.PrecioCompra), 0) AS CostoTotal
    FROM VentasClientes vc
    INNER JOIN DetalleVentasClientes dvd ON vc.VentaID = dvd.VentaID
    INNER JOIN Productos p ON dvd.ProductoID = p.ProductoID
    LEFT JOIN LotesProducto lp ON dvd.LoteID = lp.LoteID
    WHERE CAST(vc.FechaVenta AS DATE) = @Fecha
        AND (vc.SucursalID = @SucursalID OR @SucursalID = 0)
    GROUP BY p.ProductoID, p.Nombre
    ORDER BY p.Nombre;
    
END
GO

PRINT 'Stored Procedure sp_ReporteUtilidadDiaria creado exitosamente';
GO

-- ============================================================
-- PRUEBA DEL STORED PROCEDURE
-- ============================================================
-- EXEC sp_ReporteUtilidadDiaria @Fecha = '2025-01-24', @SucursalID = 1;
