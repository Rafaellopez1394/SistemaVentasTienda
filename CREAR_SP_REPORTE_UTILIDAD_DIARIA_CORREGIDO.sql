USE DB_TIENDA;
GO

-- =============================================
-- Stored Procedure: sp_ReporteUtilidadDiaria
-- Descripción: Genera reporte completo de utilidad diaria
-- Parámetros:
--   @Fecha DATE: Fecha del reporte
--   @SucursalID INT: ID de la sucursal
-- =============================================

IF OBJECT_ID('sp_ReporteUtilidadDiaria', 'P') IS NOT NULL
    DROP PROCEDURE sp_ReporteUtilidadDiaria;
GO

CREATE PROCEDURE sp_ReporteUtilidadDiaria
    @Fecha DATE,
    @SucursalID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- =================================================================
    -- SECCION 1: RESUMEN DE VENTAS POR FORMA DE PAGO
    -- =================================================================
    SELECT 
        'RESUMEN' AS TipoSeccion,
        CASE 
            WHEN v.TipoVenta = 'CONTADO' THEN 'Contado'
            WHEN v.TipoVenta = 'CREDITO' THEN 'Crédito'
            ELSE v.TipoVenta
        END AS FormaPago,
        COUNT(DISTINCT v.VentaID) AS Tickets,
        ISNULL(SUM(dv.Cantidad), 0) AS TotalUnidades,
        ISNULL(SUM(v.Total), 0) AS TotalVentas,
        0 AS CostoTotal,
        0 AS Utilidad,
        '' AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    LEFT JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND (v.Estatus = '1' OR v.Estatus = 'Activa' OR v.Estatus IS NULL)
    GROUP BY v.TipoVenta

    UNION ALL

    -- =================================================================
    -- SECCION 2: COSTOS DE COMPRA
    -- =================================================================
    SELECT 
        'COSTOS' AS TipoSeccion,
        'Costo de Compra' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        0 AS TotalVentas,
        ISNULL(SUM(dv.Cantidad * ISNULL(dv.PrecioCompra, 0)), 0) AS CostoTotal,
        0 AS Utilidad,
        '' AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND (v.Estatus = '1' OR v.Estatus = 'Activa' OR v.Estatus IS NULL)

    UNION ALL

    -- =================================================================
    -- SECCION 3: UTILIDAD (Ventas - Costos)
    -- =================================================================
    SELECT 
        'UTILIDAD' AS TipoSeccion,
        'Utilidad Bruta' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        -- Total Ventas Contado
        ISNULL((SELECT SUM(v2.Total) 
                FROM VentasClientes v2 
                INNER JOIN Cajas c2 ON v2.CajaID = c2.CajaID
                WHERE CAST(v2.FechaVenta AS DATE) = @Fecha 
                AND c2.SucursalID = @SucursalID 
                AND v2.TipoVenta = 'CONTADO'
                AND (v2.Estatus = '1' OR v2.Estatus = 'Activa' OR v2.Estatus IS NULL)), 0) AS TotalVentas,
        -- Total Ventas Crédito
        ISNULL((SELECT SUM(v2.Total) 
                FROM VentasClientes v2 
                INNER JOIN Cajas c2 ON v2.CajaID = c2.CajaID
                WHERE CAST(v2.FechaVenta AS DATE) = @Fecha 
                AND c2.SucursalID = @SucursalID 
                AND v2.TipoVenta = 'CREDITO'
                AND (v2.Estatus = '1' OR v2.Estatus = 'Activa' OR v2.Estatus IS NULL)), 0) AS CostoTotal,
        -- Utilidad = Ventas - Costos
        (
            ISNULL((SELECT SUM(v3.Total) 
                    FROM VentasClientes v3 
                    INNER JOIN Cajas c3 ON v3.CajaID = c3.CajaID
                    WHERE CAST(v3.FechaVenta AS DATE) = @Fecha 
                    AND c3.SucursalID = @SucursalID
                    AND (v3.Estatus = '1' OR v3.Estatus = 'Activa' OR v3.Estatus IS NULL)), 0)
            -
            ISNULL((SELECT SUM(dv2.Cantidad * ISNULL(dv2.PrecioCompra, 0))
                    FROM VentasClientes v4
                    INNER JOIN Cajas c4 ON v4.CajaID = c4.CajaID
                    INNER JOIN VentasDetalleClientes dv2 ON v4.VentaID = dv2.VentaID
                    WHERE CAST(v4.FechaVenta AS DATE) = @Fecha 
                    AND c4.SucursalID = @SucursalID
                    AND (v4.Estatus = '1' OR v4.Estatus = 'Activa' OR v4.Estatus IS NULL)), 0)
        ) AS Utilidad,
        '' AS Producto

    UNION ALL

    UNION ALL

    -- =================================================================
    -- SECCION 7: DETALLE DE VENTAS POR PRODUCTO
    -- =================================================================
    SELECT 
        'DETALLE_VENTAS' AS TipoSeccion,
        p.Nombre AS FormaPago,
        0 AS Tickets,
        -- Ventas Contado (unidades)
        ISNULL(SUM(CASE WHEN v.TipoVenta = 'CONTADO' THEN dv.Cantidad ELSE 0 END), 0) AS TotalUnidades,
        -- Ventas Crédito (unidades)
        ISNULL(SUM(CASE WHEN v.TipoVenta = 'CREDITO' THEN dv.Cantidad ELSE 0 END), 0) AS TotalVentas,
        -- Costo Total
        ISNULL(SUM(dv.Cantidad * ISNULL(dv.PrecioCompra, 0)), 0) AS CostoTotal,
        -- Utilidad Total
        ISNULL(SUM(dv.Cantidad * (dv.PrecioVenta - ISNULL(dv.PrecioCompra, 0))), 0) AS Utilidad,
        p.Nombre AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND (v.Estatus = '1' OR v.Estatus = 'Activa' OR v.Estatus IS NULL)
    GROUP BY p.ProductoID, p.Nombre
    ORDER BY TipoSeccion, FormaPago;

END;
GO

PRINT '*** SP sp_ReporteUtilidadDiaria creado exitosamente ***';
GO
