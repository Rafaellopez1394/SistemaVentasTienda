-- ============================================================
-- Script: Módulo de Alertas de Inventario - Stock Mínimo
-- Descripción: Agrega campo StockMinimo y crea índices para alertas
-- Fecha: 2026-01-04
-- ============================================================

USE DB_TIENDA;
GO

PRINT '============================================================';
PRINT 'Módulo de Alertas de Inventario - Stock Mínimo';
PRINT '============================================================';
PRINT '';

-- ============================================================
-- 1. Agregar columna StockMinimo si no existe
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Productos') AND name = 'StockMinimo')
BEGIN
    ALTER TABLE Productos
    ADD StockMinimo INT NULL;
    
    PRINT '✓ Columna StockMinimo agregada a tabla Productos';
END
ELSE
BEGIN
    PRINT '✓ Columna StockMinimo ya existe';
END
GO

-- ============================================================
-- 2. Crear índice para mejorar consultas de alertas
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Productos_StockMinimo_Estatus' AND object_id = OBJECT_ID('Productos'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Productos_StockMinimo_Estatus
    ON Productos (StockMinimo, Estatus)
    INCLUDE (ProductoID, Nombre, CodigoInterno, CategoriaID);
    
    PRINT '✓ Índice IX_Productos_StockMinimo_Estatus creado';
END
ELSE
BEGIN
    PRINT '✓ Índice ya existe';
END
GO

-- ============================================================
-- 3. Actualizar productos existentes con stock mínimo sugerido
-- ============================================================
PRINT '';
PRINT 'Actualizando productos sin stock mínimo configurado...';

UPDATE p
SET p.StockMinimo = CASE 
    -- Productos con ventas frecuentes (últimos 90 días)
    WHEN ventasRecientes.TotalVendido > 0 THEN 
        CAST(CEILING(ventasRecientes.TotalVendido / 3.0) AS INT) -- 1 mes de stock
    -- Productos sin ventas pero con stock actual
    WHEN stockActual.Stock > 0 THEN 
        CAST(CEILING(stockActual.Stock * 0.3) AS INT) -- 30% del stock actual
    -- Productos nuevos o sin movimientos
    ELSE 10
END
FROM Productos p
LEFT JOIN (
    SELECT 
        dv.ProductoID,
        SUM(dv.Cantidad) AS TotalVendido
    FROM VentasDetalleClientes dv
    INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
    WHERE v.FechaVenta >= DATEADD(DAY, -90, GETDATE())
    GROUP BY dv.ProductoID
) ventasRecientes ON p.ProductoID = ventasRecientes.ProductoID
LEFT JOIN (
    SELECT 
        ProductoID,
        SUM(CantidadDisponible) AS Stock
    FROM LotesProducto
    WHERE Estatus = 1
    GROUP BY ProductoID
) stockActual ON p.ProductoID = stockActual.ProductoID
WHERE p.StockMinimo IS NULL
  AND p.Estatus = 1;

DECLARE @ProductosActualizados INT = @@ROWCOUNT;
PRINT '✓ ' + CAST(@ProductosActualizados AS VARCHAR(10)) + ' productos actualizados con stock mínimo sugerido';
GO

-- ============================================================
-- 4. Verificación de alertas actuales
-- ============================================================
PRINT '';
PRINT '============================================================';
PRINT 'Resumen de Alertas Actuales:';
PRINT '============================================================';

SELECT 
    CASE 
        WHEN stock.StockActual = 0 THEN 'AGOTADO'
        WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
        WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
        ELSE 'NORMAL'
    END AS NivelAlerta,
    COUNT(*) AS CantidadProductos
FROM Productos p
LEFT JOIN (
    SELECT 
        ProductoID,
        SUM(CantidadDisponible) AS StockActual
    FROM LotesProducto
    WHERE Estatus = 1
    GROUP BY ProductoID
) stock ON p.ProductoID = stock.ProductoID
WHERE p.Estatus = 1
  AND p.StockMinimo IS NOT NULL
  AND p.StockMinimo > 0
  AND ISNULL(stock.StockActual, 0) <= p.StockMinimo
GROUP BY 
    CASE 
        WHEN stock.StockActual = 0 THEN 'AGOTADO'
        WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
        WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
        ELSE 'NORMAL'
    END
ORDER BY 
    CASE 
        WHEN CASE 
            WHEN stock.StockActual = 0 THEN 'AGOTADO'
            WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
            WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
            ELSE 'NORMAL'
        END = 'AGOTADO' THEN 1
        WHEN CASE 
            WHEN stock.StockActual = 0 THEN 'AGOTADO'
            WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
            WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
            ELSE 'NORMAL'
        END = 'CRITICO' THEN 2
        WHEN CASE 
            WHEN stock.StockActual = 0 THEN 'AGOTADO'
            WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
            WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
            ELSE 'NORMAL'
        END = 'BAJO' THEN 3
        ELSE 4
    END;

PRINT '';
PRINT '============================================================';
PRINT 'Ejemplos de productos con alertas:';
PRINT '============================================================';

SELECT TOP 10
    p.CodigoInterno AS Codigo,
    p.Nombre AS Producto,
    ISNULL(stock.StockActual, 0) AS StockActual,
    p.StockMinimo,
    CASE 
        WHEN stock.StockActual = 0 THEN 'AGOTADO'
        WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 'CRITICO'
        WHEN stock.StockActual <= p.StockMinimo THEN 'BAJO'
        ELSE 'NORMAL'
    END AS NivelAlerta
FROM Productos p
LEFT JOIN (
    SELECT 
        ProductoID,
        SUM(CantidadDisponible) AS StockActual
    FROM LotesProducto
    WHERE Estatus = 1
    GROUP BY ProductoID
) stock ON p.ProductoID = stock.ProductoID
WHERE p.Estatus = 1
  AND p.StockMinimo IS NOT NULL
  AND p.StockMinimo > 0
  AND ISNULL(stock.StockActual, 0) <= p.StockMinimo
ORDER BY 
    CASE 
        WHEN stock.StockActual = 0 THEN 1
        WHEN stock.StockActual <= (p.StockMinimo * 0.25) THEN 2
        ELSE 3
    END,
    p.Nombre;

PRINT '';
PRINT '============================================================';
PRINT 'Script ejecutado exitosamente';
PRINT '============================================================';
PRINT '';
PRINT 'Accede al módulo en: /AlertasInventario';
PRINT '';
