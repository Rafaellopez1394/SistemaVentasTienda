USE DB_TIENDA;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ReporteUtilidadProducto')
    DROP PROCEDURE usp_ReporteUtilidadProducto;
GO

CREATE PROCEDURE usp_ReporteUtilidadProducto
    @FechaInicio DATE,
    @FechaFin DATE,
    @ProductoID INT = NULL,
    @CategoriaID INT = NULL,
    @SucursalID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.ProductoID,
        p.CodigoInterno,
        p.Nombre AS NombreProducto,
        c.Nombre AS Categoria,
        p.Presentacion,
        ISNULL(SUM(cd.Cantidad), 0) AS CantidadComprada,
        ISNULL(SUM(cd.Cantidad * cd.PrecioCompra), 0) AS CostoTotalCompras,
        CASE WHEN ISNULL(SUM(cd.Cantidad), 0) > 0 THEN ISNULL(SUM(cd.Cantidad * cd.PrecioCompra), 0) / ISNULL(SUM(cd.Cantidad), 0) ELSE 0 END AS CostoPromedioCompra,
        ISNULL(SUM(vd.Cantidad), 0) AS CantidadVendida,
        ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) AS ImporteTotalVentas,
        CASE WHEN ISNULL(SUM(vd.Cantidad), 0) > 0 THEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) / ISNULL(SUM(vd.Cantidad), 0) ELSE 0 END AS PrecioPromedioVenta,
        ISNULL((SELECT SUM(lp.CantidadDisponible) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) AS InventarioActual,
        ISNULL((SELECT SUM(lp.CantidadDisponible * lp.PrecioCompra) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) AS ValorInventario,
        ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) AS CostoVendido,
        ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) AS UtilidadBruta,
        CASE WHEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) > 0 THEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) / ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0)) * 100 ELSE 0 END AS MargenUtilidadPorcentaje,
        CASE 
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 >= 30 THEN 'ALTA' 
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 >= 15 THEN 'MEDIA' 
            ELSE 'BAJA' 
        END AS Rentabilidad,
        CASE 
            WHEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) < 0 THEN 'Ajustar precio de venta o reducir costo' 
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 < 10 THEN 'Margen muy bajo, revisar estrategia' 
            WHEN ISNULL(SUM(vd.Cantidad), 0) = 0 AND ISNULL((SELECT SUM(lp.CantidadDisponible) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) > 0 THEN 'Sin ventas en el periodo, promover o descontar' 
            ELSE 'Mantener estrategia actual' 
        END AS Recomendacion
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN ComprasDetalle cd ON p.ProductoID = cd.ProductoID AND EXISTS (SELECT 1 FROM Compras co WHERE co.CompraID = cd.CompraID AND co.FechaRegistro BETWEEN @FechaInicio AND @FechaFin AND (@SucursalID IS NULL OR co.SucursalID = @SucursalID))
    LEFT JOIN VentasDetalleClientes vd ON p.ProductoID = vd.ProductoID AND EXISTS (SELECT 1 FROM VentasClientes v WHERE v.VentaID = vd.VentaID AND v.FechaVenta BETWEEN @FechaInicio AND @FechaFin AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID))
    WHERE (@ProductoID IS NULL OR p.ProductoID = @ProductoID) AND (@CategoriaID IS NULL OR p.CategoriaID = @CategoriaID) AND p.Estatus = 1
    GROUP BY p.ProductoID, p.CodigoInterno, p.Nombre, c.Nombre, p.Presentacion
    HAVING ISNULL(SUM(cd.Cantidad), 0) > 0 OR ISNULL(SUM(vd.Cantidad), 0) > 0 OR ISNULL((SELECT SUM(lp.CantidadDisponible) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) > 0
    ORDER BY UtilidadBruta DESC;
END
GO

PRINT 'Stored procedure actualizado correctamente';
