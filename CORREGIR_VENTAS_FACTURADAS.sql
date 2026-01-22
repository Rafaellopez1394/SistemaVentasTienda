-- ==================================================
-- Corregir ventas marcadas como facturadas incorrectamente
-- ==================================================
-- Este script desmarca ventas que solo tienen facturas PENDIENTES (sin timbrar)
-- Una venta solo debe estar marcada como facturada si tiene al menos una factura TIMBRADA o CANCELADA

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '=== Iniciando corrección de ventas facturadas ===';
PRINT '';

-- 1. Mostrar ventas que serán desmarcadas
PRINT '1. Ventas que serán desmarcadas (solo tienen facturas PENDIENTES):';
SELECT 
    V.VentaID,
    V.FechaVenta,
    C.RazonSocial AS Cliente,
    V.Total,
    V.EstaFacturada,
    COUNT(F.FacturaID) AS FacturasPendientes
FROM VentasClientes V
INNER JOIN Facturas F ON V.VentaID = F.VentaID
LEFT JOIN Clientes C ON V.ClienteID = C.ClienteID
WHERE V.EstaFacturada = 1
    AND F.Estatus = 'PENDIENTE'
    AND V.VentaID NOT IN (
        -- Excluir ventas que tienen al menos una factura timbrada o cancelada
        SELECT DISTINCT VentaID 
        FROM Facturas 
        WHERE Estatus IN ('TIMBRADA', 'CANCELADA')
    )
GROUP BY V.VentaID, V.FechaVenta, C.RazonSocial, V.Total, V.EstaFacturada;

PRINT '';
PRINT '---';
PRINT '';

-- 2. Contar cuántas ventas serán afectadas
DECLARE @VentasADesmarcar INT;
SELECT @VentasADesmarcar = COUNT(DISTINCT V.VentaID)
FROM VentasClientes V
INNER JOIN Facturas F ON V.VentaID = F.VentaID
WHERE V.EstaFacturada = 1
    AND F.Estatus = 'PENDIENTE'
    AND V.VentaID NOT IN (
        SELECT DISTINCT VentaID 
        FROM Facturas 
        WHERE Estatus IN ('TIMBRADA', 'CANCELADA')
    );

PRINT '2. Total de ventas a desmarcar: ' + CAST(@VentasADesmarcar AS VARCHAR);
PRINT '';

-- 3. Ejecutar la corrección
IF @VentasADesmarcar > 0
BEGIN
    PRINT '3. Ejecutando corrección...';
    
    UPDATE VentasClientes
    SET EstaFacturada = 0
    WHERE VentaID IN (
        SELECT DISTINCT V.VentaID
        FROM VentasClientes V
        INNER JOIN Facturas F ON V.VentaID = F.VentaID
        WHERE V.EstaFacturada = 1
            AND F.Estatus = 'PENDIENTE'
            AND V.VentaID NOT IN (
                SELECT DISTINCT VentaID 
                FROM Facturas 
                WHERE Estatus IN ('TIMBRADA', 'CANCELADA')
            )
    );
    
    PRINT '✓ Corrección completada: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' ventas desmarcadas';
END
ELSE
BEGIN
    PRINT '✓ No hay ventas que corregir';
END

PRINT '';
PRINT '---';
PRINT '';

-- 4. Verificar el estado final
PRINT '4. Estado final - Resumen de ventas:';
SELECT 
    TipoVenta,
    EstaFacturada,
    COUNT(*) AS Cantidad
FROM (
    SELECT 
        V.VentaID,
        V.EstaFacturada,
        CASE 
            WHEN EXISTS (SELECT 1 FROM Facturas WHERE VentaID = V.VentaID AND Estatus IN ('TIMBRADA', 'CANCELADA'))
            THEN 'Con Factura Timbrada'
            WHEN EXISTS (SELECT 1 FROM Facturas WHERE VentaID = V.VentaID AND Estatus = 'PENDIENTE')
            THEN 'Solo Facturas Pendientes'
            ELSE 'Sin Facturas'
        END AS TipoVenta
    FROM VentasClientes V
) AS Resumen
GROUP BY TipoVenta, EstaFacturada
ORDER BY TipoVenta, EstaFacturada;

PRINT '';
PRINT '=== Corrección completada ===';
GO
