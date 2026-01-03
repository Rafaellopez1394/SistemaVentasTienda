-- =====================================================
-- Script: 024c_CORREGIR_SP_BUSCAR_PRODUCTO_POS.sql
-- Descripción: Corrige el stored procedure para usar
--              LotesProducto en lugar de ProductosSucursal
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
        COALESCE(MAX(lp.PrecioVenta), 0) AS PrecioVenta,
        p.TasaIVAID,
        iva.Porcentaje AS TasaIVA,
        iva.Descripcion AS DescripcionIVA,
        COALESCE(SUM(lp.CantidadDisponible), 0) AS StockDisponible,
        p.Estatus,
        c.Nombre AS Categoria,
        -- Campos de venta por gramaje
        COALESCE(p.VentaPorGramaje, 0) AS VentaPorGramaje,
        p.PrecioPorKilo,
        p.UnidadMedidaBase
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN CatTasaIVA iva ON p.TasaIVAID = iva.TasaIVAID
    LEFT JOIN LotesProducto lp ON p.ProductoID = lp.ProductoID 
        AND lp.Estatus = 1 
        AND lp.CantidadDisponible > 0
    WHERE p.Estatus = 1
      AND (
          p.Nombre LIKE '%' + @Texto + '%' 
          OR p.CodigoInterno LIKE '%' + @Texto + '%'
      )
    GROUP BY 
        p.ProductoID,
        p.Nombre,
        p.CodigoInterno,
        p.TasaIVAID,
        iva.Porcentaje,
        iva.Descripcion,
        p.Estatus,
        c.Nombre,
        p.VentaPorGramaje,
        p.PrecioPorKilo,
        p.UnidadMedidaBase
    ORDER BY p.Nombre
END
GO

PRINT '✓ Stored Procedure BuscarProductoPOS corregido para usar LotesProducto'
GO
