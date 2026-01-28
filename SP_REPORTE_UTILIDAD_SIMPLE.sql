USE DB_TIENDA;
GO

IF OBJECT_ID('sp_ReporteUtilidadDiaria', 'P') IS NOT NULL
    DROP PROCEDURE sp_ReporteUtilidadDiaria;
GO

CREATE PROCEDURE sp_ReporteUtilidadDiaria
    @Fecha DATE,
    @SucursalID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- SECCION 1: RESUMEN DE VENTAS
    SELECT 
        'RESUMEN' AS TipoSeccion,
        CASE 
            WHEN v.TipoVenta = 'CONTADO' THEN 'Contado'
            WHEN v.TipoVenta = 'CREDITO' THEN 'Credito'
            ELSE ISNULL(v.TipoVenta, 'Otro')
        END AS FormaPago,
        COUNT(DISTINCT v.VentaID) AS Tickets,
        ISNULL(SUM(dv.Cantidad), 0) AS TotalUnidades,
        ISNULL(SUM(v.Total), 0) AS TotalVentas,
        0.0 AS CostoTotal,
        0.0 AS Utilidad,
        '' AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    LEFT JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND ISNULL(v.Estatus, '1') IN ('1', 'Activa')
    GROUP BY v.TipoVenta

    UNION ALL

    -- SECCION 2: COSTOS
    SELECT 
        'COSTOS' AS TipoSeccion,
        'Costo de Compra' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        0.0 AS TotalVentas,
        ISNULL(SUM(dv.Cantidad * ISNULL(dv.PrecioCompra, 0)), 0) AS CostoTotal,
        0.0 AS Utilidad,
        '' AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND ISNULL(v.Estatus, '1') IN ('1', 'Activa')

    UNION ALL

    -- SECCION 3: UTILIDAD
    SELECT 
        'UTILIDAD' AS TipoSeccion,
        'Utilidad Bruta' AS FormaPago,
        0 AS Tickets,
        0 AS TotalUnidades,
        ISNULL((SELECT SUM(v2.Total) 
                FROM VentasClientes v2 
                INNER JOIN Cajas c2 ON v2.CajaID = c2.CajaID
                WHERE CAST(v2.FechaVenta AS DATE) = @Fecha 
                AND c2.SucursalID = @SucursalID 
                AND v2.TipoVenta = 'CONTADO'
                AND ISNULL(v2.Estatus, '1') IN ('1', 'Activa')), 0) AS TotalVentas,
        ISNULL((SELECT SUM(v2.Total) 
                FROM VentasClientes v2 
                INNER JOIN Cajas c2 ON v2.CajaID = c2.CajaID
                WHERE CAST(v2.FechaVenta AS DATE) = @Fecha 
                AND c2.SucursalID = @SucursalID 
                AND v2.TipoVenta = 'CREDITO'
                AND ISNULL(v2.Estatus, '1') IN ('1', 'Activa')), 0) AS CostoTotal,
        (
            ISNULL((SELECT SUM(v3.Total) 
                    FROM VentasClientes v3 
                    INNER JOIN Cajas c3 ON v3.CajaID = c3.CajaID
                    WHERE CAST(v3.FechaVenta AS DATE) = @Fecha 
                    AND c3.SucursalID = @SucursalID
                    AND ISNULL(v3.Estatus, '1') IN ('1', 'Activa')), 0)
            -
            ISNULL((SELECT SUM(dv2.Cantidad * ISNULL(dv2.PrecioCompra, 0))
                    FROM VentasClientes v4
                    INNER JOIN Cajas c4 ON v4.CajaID = c4.CajaID
                    INNER JOIN VentasDetalleClientes dv2 ON v4.VentaID = dv2.VentaID
                    WHERE CAST(v4.FechaVenta AS DATE) = @Fecha 
                    AND c4.SucursalID = @SucursalID
                    AND ISNULL(v4.Estatus, '1') IN ('1', 'Activa')), 0)
        ) AS Utilidad,
        '' AS Producto

    UNION ALL

    -- SECCION 4: DETALLE POR PRODUCTO
    SELECT 
        'DETALLE_VENTAS' AS TipoSeccion,
        ISNULL(p.Nombre, 'Producto Desconocido') AS FormaPago,
        0 AS Tickets,
        ISNULL(SUM(CASE WHEN v.TipoVenta = 'CONTADO' THEN dv.Cantidad ELSE 0 END), 0) AS TotalUnidades,
        ISNULL(SUM(CASE WHEN v.TipoVenta = 'CREDITO' THEN dv.Cantidad ELSE 0 END), 0) AS TotalVentas,
        ISNULL(SUM(dv.Cantidad * ISNULL(dv.PrecioCompra, 0)), 0) AS CostoTotal,
        ISNULL(SUM(dv.Cantidad * (dv.PrecioVenta - ISNULL(dv.PrecioCompra, 0))), 0) AS Utilidad,
        ISNULL(p.Nombre, 'Producto Desconocido') AS Producto
    FROM VentasClientes v
    INNER JOIN Cajas c ON v.CajaID = c.CajaID
    INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    LEFT JOIN Productos p ON dv.ProductoID = p.ProductoID
    WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        AND c.SucursalID = @SucursalID
        AND ISNULL(v.Estatus, '1') IN ('1', 'Activa')
    GROUP BY p.ProductoID, p.Nombre
    
    ORDER BY TipoSeccion, FormaPago;

END;
GO

PRINT '*** SP sp_ReporteUtilidadDiaria creado exitosamente ***';
GO
