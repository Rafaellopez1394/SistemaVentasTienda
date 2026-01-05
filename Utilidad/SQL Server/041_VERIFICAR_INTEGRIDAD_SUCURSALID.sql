-- ================================================
-- VERIFICACIÓN DE INTEGRIDAD: SUCURSALID EN LOTES
-- ================================================
-- Fecha: 04 Enero 2026
-- Propósito: Validar que todo el sistema respeta la sucursal

PRINT '========================================';
PRINT 'VERIFICACIÓN DE INTEGRIDAD MULTISUCURSAL';
PRINT '========================================';
PRINT '';

-- 1. Verificar que todos los lotes tengan SucursalID
PRINT '1. VERIFICANDO LOTES SIN SUCURSAL...';
IF EXISTS (SELECT 1 FROM LotesProducto WHERE SucursalID IS NULL)
BEGIN
    PRINT '  ❌ ERROR: Hay lotes sin SucursalID asignado';
    SELECT COUNT(*) AS LotesSinSucursal FROM LotesProducto WHERE SucursalID IS NULL;
END
ELSE
BEGIN
    PRINT '  ✅ OK: Todos los lotes tienen SucursalID';
END
PRINT '';

-- 2. Distribución de inventario por sucursal
PRINT '2. DISTRIBUCIÓN DE INVENTARIO POR SUCURSAL:';
SELECT 
    S.SucursalID,
    S.Nombre AS Sucursal,
    COUNT(L.LoteID) AS TotalLotes,
    SUM(L.CantidadDisponible) AS UnidadesDisponibles,
    COUNT(DISTINCT L.ProductoID) AS ProductosDistintos
FROM SUCURSAL S
LEFT JOIN LotesProducto L ON S.SucursalID = L.SucursalID AND L.Estatus = 1
WHERE S.Activo = 1
GROUP BY S.SucursalID, S.Nombre
ORDER BY S.SucursalID;
PRINT '';

-- 3. Verificar productos con stock en múltiples sucursales
PRINT '3. PRODUCTOS CON INVENTARIO EN MÚLTIPLES SUCURSALES:';
SELECT 
    P.ProductoID,
    P.Nombre,
    COUNT(DISTINCT L.SucursalID) AS NumSucursales,
    SUM(L.CantidadDisponible) AS TotalDisponible
FROM Productos P
INNER JOIN LotesProducto L ON P.ProductoID = L.ProductoID
WHERE L.Estatus = 1 AND L.CantidadDisponible > 0
GROUP BY P.ProductoID, P.Nombre
HAVING COUNT(DISTINCT L.SucursalID) > 1
ORDER BY NumSucursales DESC;
PRINT '';

-- 4. Verificar traspasos pendientes
PRINT '4. TRASPASOS PENDIENTES O EN TRÁNSITO:';
IF EXISTS (SELECT 1 FROM Traspasos WHERE Estatus IN ('PENDIENTE', 'EN_TRANSITO'))
BEGIN
    SELECT 
        T.TraspasoID,
        T.FolioTraspaso,
        T.Estatus,
        SO.Nombre AS Origen,
        SD.Nombre AS Destino,
        T.FechaTraspaso
    FROM Traspasos T
    INNER JOIN SUCURSAL SO ON T.SucursalOrigenID = SO.SucursalID
    INNER JOIN SUCURSAL SD ON T.SucursalDestinoID = SD.SucursalID
    WHERE T.Estatus IN ('PENDIENTE', 'EN_TRANSITO')
    ORDER BY T.FechaTraspaso DESC;
END
ELSE
BEGIN
    PRINT '  No hay traspasos pendientes';
END
PRINT '';

-- 5. Top 10 productos por sucursal
PRINT '5. TOP 10 PRODUCTOS POR SUCURSAL:';
PRINT '-----------------------------------';

DECLARE @SucursalID INT, @NombreSucursal VARCHAR(100);

DECLARE cur_sucursales CURSOR FOR
SELECT SucursalID, Nombre FROM SUCURSAL WHERE Activo = 1 ORDER BY SucursalID;

OPEN cur_sucursales;
FETCH NEXT FROM cur_sucursales INTO @SucursalID, @NombreSucursal;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Sucursal: ' + @NombreSucursal;
    
    SELECT TOP 10
        P.CodigoInterno AS Codigo,
        P.Nombre AS Producto,
        SUM(L.CantidadDisponible) AS Stock
    FROM LotesProducto L
    INNER JOIN Productos P ON L.ProductoID = P.ProductoID
    WHERE L.SucursalID = @SucursalID 
      AND L.Estatus = 1 
      AND L.CantidadDisponible > 0
    GROUP BY P.ProductoID, P.CodigoInterno, P.Nombre
    ORDER BY SUM(L.CantidadDisponible) DESC;
    
    PRINT '';
    FETCH NEXT FROM cur_sucursales INTO @SucursalID, @NombreSucursal;
END

CLOSE cur_sucursales;
DEALLOCATE cur_sucursales;

-- 6. Verificar integridad referencial
PRINT '6. VERIFICANDO INTEGRIDAD REFERENCIAL:';
IF EXISTS (
    SELECT 1 FROM LotesProducto L
    LEFT JOIN SUCURSAL S ON L.SucursalID = S.SucursalID
    WHERE S.SucursalID IS NULL
)
BEGIN
    PRINT '  ❌ ERROR: Hay lotes con SucursalID inválido';
    SELECT L.LoteID, L.SucursalID 
    FROM LotesProducto L
    LEFT JOIN SUCURSAL S ON L.SucursalID = S.SucursalID
    WHERE S.SucursalID IS NULL;
END
ELSE
BEGIN
    PRINT '  ✅ OK: Todas las referencias a sucursales son válidas';
END
PRINT '';

-- 7. Resumen final
PRINT '========================================';
PRINT 'RESUMEN DE VERIFICACIÓN';
PRINT '========================================';
SELECT 
    (SELECT COUNT(*) FROM LotesProducto) AS TotalLotes,
    (SELECT COUNT(*) FROM LotesProducto WHERE SucursalID IS NULL) AS LotesSinSucursal,
    (SELECT SUM(CantidadDisponible) FROM LotesProducto WHERE Estatus = 1) AS UnidadesTotales,
    (SELECT COUNT(DISTINCT SucursalID) FROM LotesProducto) AS SucursalesConInventario,
    (SELECT COUNT(*) FROM Traspasos WHERE Estatus IN ('PENDIENTE', 'EN_TRANSITO')) AS TraspasosPendientes;

PRINT '';
PRINT '✅ Verificación completada';
PRINT '';
