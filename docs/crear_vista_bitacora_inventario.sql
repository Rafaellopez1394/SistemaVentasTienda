-- Vista para consultar bitácora de movimientos de inventario
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_BitacoraInventario')
    DROP VIEW vw_BitacoraInventario;
GO

CREATE VIEW vw_BitacoraInventario AS
SELECT 
    m.MovimientoID,
    m.Fecha,
    m.TipoMovimiento,
    m.Cantidad,
    m.CostoUnitario,
    m.Cantidad * m.CostoUnitario AS CostoTotal,
    m.Usuario,
    m.Comentarios,
    m.LoteID,
    l.FechaEntrada,
    l.CantidadTotal AS LoteCantidadTotal,
    l.CantidadDisponible AS LoteCantidadDisponible,
    m.ProductoID,
    p.Nombre AS ProductoNombre,
    p.CodigoInterno AS ProductoCodigo,
    c.Nombre AS CategoriaNombre
FROM MovimientosInventario m
INNER JOIN LotesProducto l ON m.LoteID = l.LoteID
INNER JOIN Productos p ON m.ProductoID = p.ProductoID
LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID;
GO

-- Consulta de ejemplo: Últimos 50 movimientos
SELECT TOP 50
    Fecha,
    TipoMovimiento,
    ProductoNombre,
    ProductoCodigo,
    Cantidad,
    CostoTotal,
    Usuario,
    Comentarios
FROM vw_BitacoraInventario
ORDER BY Fecha DESC;
GO

-- Consulta: Resumen por tipo de movimiento (último mes)
SELECT 
    TipoMovimiento,
    COUNT(*) AS TotalMovimientos,
    SUM(Cantidad) AS TotalUnidades,
    SUM(CostoTotal) AS CostoTotalAcumulado
FROM vw_BitacoraInventario
WHERE Fecha >= DATEADD(MONTH, -1, GETDATE())
GROUP BY TipoMovimiento
ORDER BY CostoTotalAcumulado DESC;
