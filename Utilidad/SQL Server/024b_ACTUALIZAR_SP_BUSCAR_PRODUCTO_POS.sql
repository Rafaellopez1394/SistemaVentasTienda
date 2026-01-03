-- =====================================================
-- Script: 024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql
-- Descripción: Actualiza el stored procedure para incluir 
--              campos de venta por gramaje
-- =====================================================

USE DB_TIENDA
GO

-- Actualizar Stored Procedure: BuscarProductoPOS
IF OBJECT_ID('BuscarProductoPOS', 'P') IS NOT NULL
    DROP PROCEDURE BuscarProductoPOS
GO

CREATE PROCEDURE BuscarProductoPOS
    @Texto VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductoID,
        p.Nombre,
        p.CodigoInterno,
        COALESCE(lp.PrecioVenta, 0) AS PrecioVenta,
        p.TasaIVAID,
        iva.Porcentaje AS TasaIVA,
        iva.Descripcion AS DescripcionIVA,
        COALESCE(ps.Stock, 0) AS StockDisponible,
        p.Estatus,
        c.Nombre AS Categoria,
        -- Campos de venta por gramaje
        COALESCE(p.VentaPorGramaje, 0) AS VentaPorGramaje,
        p.PrecioPorKilo,
        p.UnidadMedidaBase
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN CatTasaIVA iva ON p.TasaIVAID = iva.TasaIVAID
    LEFT JOIN ProductosSucursal ps ON p.ProductoID = ps.ProductoID
    LEFT JOIN (
        SELECT ProductoID, MAX(PrecioVenta) AS PrecioVenta
        FROM LotesProducto
        WHERE Estatus = 1 AND CantidadDisponible > 0
        GROUP BY ProductoID
    ) lp ON p.ProductoID = lp.ProductoID
    WHERE p.Estatus = 1
      AND (
          p.Nombre LIKE '%' + @Texto + '%' 
          OR p.CodigoInterno LIKE '%' + @Texto + '%'
      )
    ORDER BY p.Nombre
END
GO

PRINT 'Stored Procedure BuscarProductoPOS actualizado con campos de gramaje'
GO
