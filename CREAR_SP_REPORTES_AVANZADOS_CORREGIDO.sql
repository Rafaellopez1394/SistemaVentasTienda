-- =============================================
-- STORED PROCEDURES CORREGIDOS PARA REPORTES AVANZADOS
-- Sistema de Punto de Venta - 22 Enero 2026
-- =============================================

USE DB_TIENDA;
GO

-- =============================================
-- 1. REPORTE DE UTILIDAD POR PRODUCTO (CORREGIDO)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ReporteUtilidadProducto]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ReporteUtilidadProducto]
GO

CREATE PROCEDURE [dbo].[usp_ReporteUtilidadProducto]
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
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
        ISNULL(c.Nombre, 'Sin categoría') AS Categoria,
        p.UnidadMedidaBase AS Presentacion,
        
        -- COMPRAS del período
        ISNULL(SUM(cd.Cantidad), 0) AS CantidadComprada,
        ISNULL(SUM(cd.Cantidad * cd.PrecioCompra), 0) AS CostoTotalCompras,
        CASE 
            WHEN ISNULL(SUM(cd.Cantidad), 0) > 0 
            THEN ISNULL(SUM(cd.Cantidad * cd.PrecioCompra), 0) / ISNULL(SUM(cd.Cantidad), 0)
            ELSE 0 
        END AS CostoPromedioCompra,
        
        -- VENTAS del período
        ISNULL(SUM(vd.Cantidad), 0) AS CantidadVendida,
        ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) AS ImporteTotalVentas,
        CASE 
            WHEN ISNULL(SUM(vd.Cantidad), 0) > 0 
            THEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) / ISNULL(SUM(vd.Cantidad), 0)
            ELSE 0 
        END AS PrecioPromedioVenta,
        
        -- INVENTARIO ACTUAL (suma de lotes activos)
        ISNULL((
            SELECT SUM(lp.CantidadDisponible)
            FROM LotesProducto lp
            WHERE lp.ProductoID = p.ProductoID
                AND lp.Estatus = 1
                AND (@SucursalID IS NULL OR lp.SucursalID = @SucursalID)
        ), 0) AS InventarioActual,
        
        ISNULL((
            SELECT SUM(lp.CantidadDisponible * lp.PrecioCompra)
            FROM LotesProducto lp
            WHERE lp.ProductoID = p.ProductoID
                AND lp.Estatus = 1
                AND (@SucursalID IS NULL OR lp.SucursalID = @SucursalID)
        ), 0) AS ValorInventario,
        
        -- COSTO DE LO VENDIDO (precio de compra del detalle de venta)
        ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) AS CostoVendido,
        
        -- UTILIDAD BRUTA = Ventas - Costo Vendido
        ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) AS UtilidadBruta,
        
        -- MARGEN DE UTILIDAD %
        CASE 
            WHEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) > 0 
            THEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) 
                  / ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0)) * 100
            ELSE 0 
        END AS MargenUtilidadPorcentaje,
        
        -- ANÁLISIS
        CASE 
            WHEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) < 0 THEN 'PÉRDIDA'
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) 
                  / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 >= 30 THEN 'ALTA'
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) 
                  / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 >= 15 THEN 'MEDIA'
            ELSE 'BAJA'
        END AS Rentabilidad,
        
        CASE 
            WHEN ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0) < 0 
            THEN 'Ajustar precio de venta o reducir costo'
            WHEN ((ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) - ISNULL(SUM(vd.Cantidad * ISNULL(vd.PrecioCompra, 0)), 0)) 
                  / NULLIF(ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0), 0)) * 100 < 10 
            THEN 'Margen muy bajo, revisar estrategia'
            WHEN ISNULL(SUM(vd.Cantidad), 0) = 0 AND ISNULL((SELECT SUM(lp.CantidadDisponible) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) > 0
            THEN 'Sin ventas en el periodo, promover o descontar'
            ELSE 'Mantener estrategia actual'
        END AS Recomendacion
        
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN ComprasDetalle cd ON p.ProductoID = cd.ProductoID
    LEFT JOIN Compras co ON cd.CompraID = co.CompraID 
        AND co.FechaCompra BETWEEN @FechaInicio AND @FechaFin
    LEFT JOIN VentasDetalleClientes vd ON p.ProductoID = vd.ProductoID
    LEFT JOIN VentasClientes v ON vd.VentaID = v.VentaID 
        AND v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND (@SucursalID IS NULL OR v.VentaID IN (
            SELECT DISTINCT vd2.VentaID 
            FROM VentasDetalleClientes vd2 
            INNER JOIN LotesProducto lp2 ON vd2.LoteID = lp2.LoteID 
            WHERE lp2.SucursalID = @SucursalID
        ))
    WHERE (@ProductoID IS NULL OR p.ProductoID = @ProductoID)
        AND (@CategoriaID IS NULL OR p.CategoriaID = @CategoriaID)
        AND p.Estatus = 1
    GROUP BY 
        p.ProductoID, p.CodigoInterno, p.Nombre, c.Nombre, p.UnidadMedidaBase
    HAVING 
        -- Solo mostrar productos con actividad (compras o ventas) o con inventario
        ISNULL(SUM(cd.Cantidad), 0) > 0 
        OR ISNULL(SUM(vd.Cantidad), 0) > 0
        OR ISNULL((SELECT SUM(lp.CantidadDisponible) FROM LotesProducto lp WHERE lp.ProductoID = p.ProductoID AND lp.Estatus = 1), 0) > 0
    ORDER BY 
        UtilidadBruta DESC, ImporteTotalVentas DESC;
END
GO

-- =============================================
-- 2. CONCENTRADO DE RECUPERACIÓN DE CRÉDITO (CORREGIDO)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ConcentradoRecuperacionCredito]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ConcentradoRecuperacionCredito]
GO

CREATE PROCEDURE [dbo].[usp_ConcentradoRecuperacionCredito]
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @SucursalID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Crear tabla temporal para rango de fechas
    DECLARE @Fechas TABLE (Fecha DATE);
    DECLARE @FechaActual DATE = CAST(@FechaInicio AS DATE);
    
    WHILE @FechaActual <= CAST(@FechaFin AS DATE)
    BEGIN
        INSERT INTO @Fechas (Fecha) VALUES (@FechaActual);
        SET @FechaActual = DATEADD(DAY, 1, @FechaActual);
    END
    
    -- Calcular métricas por día
    SELECT 
        f.Fecha,
        
        -- Clientes con crédito activo
        (SELECT COUNT(DISTINCT v2.ClienteID)
         FROM VentasClientes v2
         WHERE v2.TipoVenta = 'CREDITO'
            AND CAST(v2.FechaVenta AS DATE) = f.Fecha
            AND (@SucursalID IS NULL OR v2.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ) AS TotalClientesCredito,
        
        -- Créditos otorgados en el día
        ISNULL((SELECT SUM(v2.Total)
         FROM VentasClientes v2
         WHERE v2.TipoVenta = 'CREDITO'
            AND CAST(v2.FechaVenta AS DATE) = f.Fecha
            AND (@SucursalID IS NULL OR v2.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ), 0) AS CreditosOtorgados,
        
        (SELECT COUNT(*)
         FROM VentasClientes v2
         WHERE v2.TipoVenta = 'CREDITO'
            AND CAST(v2.FechaVenta AS DATE) = f.Fecha
            AND (@SucursalID IS NULL OR v2.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ) AS NumeroVentasCredito,
        
        -- Cobros realizados en el día
        ISNULL((SELECT SUM(p2.Monto)
         FROM PagosClientes p2
         INNER JOIN VentasClientes v3 ON p2.VentaID = v3.VentaID
         WHERE CAST(p2.FechaPago AS DATE) = f.Fecha
            AND v3.TipoVenta = 'CREDITO'
            AND (@SucursalID IS NULL OR v3.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ), 0) AS CobrosRealizados,
        
        (SELECT COUNT(*)
         FROM PagosClientes p2
         INNER JOIN VentasClientes v3 ON p2.VentaID = v3.VentaID
         WHERE CAST(p2.FechaPago AS DATE) = f.Fecha
            AND v3.TipoVenta = 'CREDITO'
            AND (@SucursalID IS NULL OR v3.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ) AS NumeroPagos,
        
        -- Saldo inicial (acumulado hasta día anterior)
        ISNULL((SELECT SUM(v4.Total) - ISNULL(SUM(p4.Monto), 0)
         FROM VentasClientes v4
         LEFT JOIN PagosClientes p4 ON v4.VentaID = p4.VentaID AND CAST(p4.FechaPago AS DATE) < f.Fecha
         WHERE v4.TipoVenta = 'CREDITO'
            AND CAST(v4.FechaVenta AS DATE) < f.Fecha
            AND (@SucursalID IS NULL OR v4.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ), 0) AS SaldoInicial,
        
        -- Saldo final (acumulado hasta hoy incluido)
        ISNULL((SELECT SUM(v5.Total) - ISNULL(SUM(p5.Monto), 0)
         FROM VentasClientes v5
         LEFT JOIN PagosClientes p5 ON v5.VentaID = p5.VentaID AND CAST(p5.FechaPago AS DATE) <= f.Fecha
         WHERE v5.TipoVenta = 'CREDITO'
            AND CAST(v5.FechaVenta AS DATE) <= f.Fecha
            AND (@SucursalID IS NULL OR v5.VentaID IN (
                SELECT vd.VentaID FROM VentasDetalleClientes vd 
                INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
                WHERE lp.SucursalID = @SucursalID
            ))
        ), 0) AS SaldoFinal,
        
        -- Porcentaje de recuperación del día
        CASE 
            WHEN ISNULL((SELECT SUM(v2.Total) FROM VentasClientes v2 
                WHERE v2.TipoVenta = 'CREDITO' AND CAST(v2.FechaVenta AS DATE) = f.Fecha), 0) > 0 
            THEN (ISNULL((SELECT SUM(p2.Monto) FROM PagosClientes p2 INNER JOIN VentasClientes v3 ON p2.VentaID = v3.VentaID 
                WHERE CAST(p2.FechaPago AS DATE) = f.Fecha AND v3.TipoVenta = 'CREDITO'), 0) 
                / ISNULL((SELECT SUM(v2.Total) FROM VentasClientes v2 
                WHERE v2.TipoVenta = 'CREDITO' AND CAST(v2.FechaVenta AS DATE) = f.Fecha), 1)) * 100
            ELSE 0 
        END AS PorcentajeRecuperacion,
        
        -- Eficiencia de cobranza
        CASE 
            WHEN ISNULL((SELECT SUM(v5.Total) FROM VentasClientes v5 
                WHERE v5.TipoVenta = 'CREDITO' AND CAST(v5.FechaVenta AS DATE) <= f.Fecha), 0) > 0
            THEN (ISNULL((SELECT SUM(p5.Monto) FROM PagosClientes p5 INNER JOIN VentasClientes v6 ON p5.VentaID = v6.VentaID 
                WHERE CAST(p5.FechaPago AS DATE) <= f.Fecha AND v6.TipoVenta = 'CREDITO'), 0)
                / ISNULL((SELECT SUM(v5.Total) FROM VentasClientes v5 
                WHERE v5.TipoVenta = 'CREDITO' AND CAST(v5.FechaVenta AS DATE) <= f.Fecha), 1)) * 100
            ELSE 0
        END AS EficienciaCobranza,
        
        -- Cartera vigente (0-30 días)
        ISNULL((SELECT SUM(v7.Total - ISNULL(p7.TotalPagado, 0))
         FROM VentasClientes v7
         LEFT JOIN (SELECT VentaID, SUM(Monto) AS TotalPagado FROM PagosClientes GROUP BY VentaID) p7 ON v7.VentaID = p7.VentaID
         WHERE v7.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v7.FechaVenta, f.Fecha) <= 30
            AND v7.Total > ISNULL(p7.TotalPagado, 0)
        ), 0) AS CarteraVigente,
        
        -- Cartera vencida (más de 30 días)
        ISNULL((SELECT SUM(v8.Total - ISNULL(p8.TotalPagado, 0))
         FROM VentasClientes v8
         LEFT JOIN (SELECT VentaID, SUM(Monto) AS TotalPagado FROM PagosClientes GROUP BY VentaID) p8 ON v8.VentaID = p8.VentaID
         WHERE v8.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v8.FechaVenta, f.Fecha) > 30
            AND v8.Total > ISNULL(p8.TotalPagado, 0)
        ), 0) AS CarteraVencida,
        
        -- Porcentaje vencido
        CASE 
            WHEN ISNULL((SELECT SUM(v5.Total) - ISNULL(SUM(p5.Monto), 0) FROM VentasClientes v5 
                LEFT JOIN PagosClientes p5 ON v5.VentaID = p5.VentaID 
                WHERE v5.TipoVenta = 'CREDITO' AND CAST(v5.FechaVenta AS DATE) <= f.Fecha), 0) > 0
            THEN (ISNULL((SELECT SUM(v8.Total - ISNULL(p8.TotalPagado, 0)) FROM VentasClientes v8 
                LEFT JOIN (SELECT VentaID, SUM(Monto) AS TotalPagado FROM PagosClientes GROUP BY VentaID) p8 ON v8.VentaID = p8.VentaID 
                WHERE v8.TipoVenta = 'CREDITO' AND DATEDIFF(DAY, v8.FechaVenta, f.Fecha) > 30 
                AND v8.Total > ISNULL(p8.TotalPagado, 0)), 0)
                / ISNULL((SELECT SUM(v5.Total) - ISNULL(SUM(p5.Monto), 0) FROM VentasClientes v5 
                LEFT JOIN PagosClientes p5 ON v5.VentaID = p5.VentaID 
                WHERE v5.TipoVenta = 'CREDITO' AND CAST(v5.FechaVenta AS DATE) <= f.Fecha), 1)) * 100
            ELSE 0
        END AS PorcentajeVencido
        
    FROM @Fechas f
    ORDER BY f.Fecha;
END
GO

-- =============================================
-- 3. CARTERA DE CLIENTES DETALLADA (CORREGIDO)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_CarteraClientes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_CarteraClientes]
GO

CREATE PROCEDURE [dbo].[usp_CarteraClientes]
    @FechaCorte DATETIME = NULL,
    @SucursalID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FechaCorte IS NULL
        SET @FechaCorte = GETDATE();
    
    SELECT 
        c.ClienteID,
        c.RFC,
        c.RazonSocial,
        tc.Nombre AS TipoCredito,
        ISNULL(ctc.PlazoDias, 0) AS DiasCredito,
        
        -- Resumen de cuenta
        ISNULL(SUM(v.Total), 0) AS TotalVentas,
        ISNULL((SELECT SUM(p.Monto) FROM PagosClientes p WHERE p.VentaID IN (SELECT VentaID FROM VentasClientes WHERE ClienteID = c.ClienteID)), 0) AS TotalPagos,
        ISNULL(SUM(v.Total), 0) - ISNULL((SELECT SUM(p.Monto) FROM PagosClientes p WHERE p.VentaID IN (SELECT VentaID FROM VentasClientes WHERE ClienteID = c.ClienteID)), 0) AS SaldoPendiente,
        
        -- Antigüedad de saldo - usando subconsultas
        ISNULL((SELECT SUM(v2.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v2.FechaVenta, @FechaCorte) <= 30
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ), 0) AS Vigente,
        
        ISNULL((SELECT SUM(v2.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v2.FechaVenta, @FechaCorte) BETWEEN 31 AND 60
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ), 0) AS Vencido30,
        
        ISNULL((SELECT SUM(v2.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v2.FechaVenta, @FechaCorte) BETWEEN 61 AND 90
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ), 0) AS Vencido60,
        
        ISNULL((SELECT SUM(v2.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND DATEDIFF(DAY, v2.FechaVenta, @FechaCorte) > 90
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ), 0) AS Vencido90,
        
        ISNULL((SELECT MAX(DATEDIFF(DAY, v2.FechaVenta, @FechaCorte))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ), 0) AS DiasVencido,
        
        -- Estado
        CASE 
            WHEN ISNULL((SELECT MAX(DATEDIFF(DAY, v2.FechaVenta, @FechaCorte))
                FROM VentasClientes v2
                WHERE v2.ClienteID = c.ClienteID AND v2.TipoVenta = 'CREDITO'
                AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
            ), 0) <= 30 THEN 'AL CORRIENTE'
            WHEN ISNULL((SELECT MAX(DATEDIFF(DAY, v2.FechaVenta, @FechaCorte))
                FROM VentasClientes v2
                WHERE v2.ClienteID = c.ClienteID AND v2.TipoVenta = 'CREDITO'
                AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
            ), 0) <= 60 THEN 'VENCIDO'
            ELSE 'MOROSO'
        END AS Estado,
        
        (SELECT MAX(p.FechaPago) 
         FROM PagosClientes p 
         WHERE p.VentaID IN (SELECT VentaID FROM VentasClientes WHERE ClienteID = c.ClienteID)
        ) AS UltimoPago,
        
        (SELECT MIN(DATEADD(DAY, ISNULL(ctc.PlazoDias, 30), v2.FechaVenta))
         FROM VentasClientes v2
         WHERE v2.ClienteID = c.ClienteID
            AND v2.TipoVenta = 'CREDITO'
            AND v2.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v2.VentaID), 0)
        ) AS ProximoVencimiento
        
    FROM Clientes c
    LEFT JOIN ClienteTiposCredito ctc ON c.ClienteID = ctc.ClienteID AND ctc.Estatus = 1
    LEFT JOIN CatTiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
    LEFT JOIN VentasClientes v ON c.ClienteID = v.ClienteID 
        AND v.TipoVenta = 'CREDITO'
        AND v.FechaVenta <= @FechaCorte
        AND (@SucursalID IS NULL OR v.VentaID IN (
            SELECT vd.VentaID FROM VentasDetalleClientes vd 
            INNER JOIN LotesProducto lp ON vd.LoteID = lp.LoteID 
            WHERE lp.SucursalID = @SucursalID
        ))
    WHERE c.Estatus = 1
    GROUP BY 
        c.ClienteID, c.RFC, c.RazonSocial, tc.Nombre, ctc.PlazoDias
    HAVING 
        ISNULL(SUM(v.Total), 0) - ISNULL((SELECT SUM(p.Monto) FROM PagosClientes p WHERE p.VentaID IN (SELECT VentaID FROM VentasClientes WHERE ClienteID = c.ClienteID)), 0) > 0
    ORDER BY 
        SaldoPendiente DESC;
END
GO

PRINT '✅ Stored Procedures de Reportes Avanzados creados exitosamente (CORREGIDOS)';
