-- =============================================
-- STORED PROCEDURES PARA REPORTES AVANZADOS
-- Sistema de Punto de Venta
-- =============================================

USE DB_TIENDA;
GO

-- =============================================
-- 1. REPORTE DE UTILIDAD POR PRODUCTO
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
        c.Nombre AS Categoria,
        p.Presentacion,
        
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
        AND EXISTS (SELECT 1 FROM Compras co WHERE co.CompraID = cd.CompraID 
                    AND co.FechaRegistro BETWEEN @FechaInicio AND @FechaFin
                    AND (@SucursalID IS NULL OR co.SucursalID = @SucursalID))
    LEFT JOIN VentasDetalleClientes vd ON p.ProductoID = vd.ProductoID
        AND EXISTS (SELECT 1 FROM VentasClientes v WHERE v.VentaID = vd.VentaID 
                    AND v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
                    AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID))
    WHERE (@ProductoID IS NULL OR p.ProductoID = @ProductoID)
        AND (@CategoriaID IS NULL OR p.CategoriaID = @CategoriaID)
        AND p.Estatus = 1
    GROUP BY 
        p.ProductoID, p.CodigoInterno, p.Nombre, c.Nombre, p.Presentacion
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
-- 2. CONCENTRADO DE RECUPERACIÓN DE CRÉDITO
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
    
    -- Por cada día, calcular créditos otorgados vs cobros realizados
    ;WITH DiasRango AS (
        SELECT CAST(@FechaInicio AS DATE) AS Fecha
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha)
        FROM DiasRango
        WHERE DATEADD(DAY, 1, Fecha) <= CAST(@FechaFin AS DATE)
    ),
    CreditosDia AS (
        SELECT 
            CAST(v.FechaVenta AS DATE) AS Fecha,
            COUNT(DISTINCT v.ClienteID) AS ClientesCredito,
            SUM(v.Total) AS CreditosOtorgados,
            COUNT(*) AS NumeroVentas
        FROM VentasClientes v
        WHERE v.TipoVenta = 'CREDITO'
            AND CAST(v.FechaVenta AS DATE) BETWEEN @FechaInicio AND @FechaFin
            AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
        GROUP BY CAST(v.FechaVenta AS DATE)
    ),
    CobrosDia AS (
        SELECT 
            CAST(p.FechaPago AS DATE) AS Fecha,
            SUM(p.Monto) AS CobrosRealizados,
            COUNT(*) AS NumeroPagos
        FROM PagosClientes p
        INNER JOIN VentasClientes v ON p.VentaID = v.VentaID
        WHERE CAST(p.FechaPago AS DATE) BETWEEN @FechaInicio AND @FechaFin
            AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
        GROUP BY CAST(p.FechaPago AS DATE)
    ),
    SaldoAcumulado AS (
        SELECT 
            dr.Fecha,
            ISNULL(cd.ClientesCredito, 0) AS TotalClientesCredito,
            ISNULL(cd.CreditosOtorgados, 0) AS CreditosOtorgados,
            ISNULL(cd.NumeroVentas, 0) AS NumeroVentasCredito,
            ISNULL(co.CobrosRealizados, 0) AS CobrosRealizados,
            ISNULL(co.NumeroPagos, 0) AS NumeroPagos,
            -- Saldo acumulado hasta la fecha
            (SELECT 
                ISNULL(SUM(v2.Total), 0) - ISNULL(SUM(p2.Monto), 0)
             FROM VentasClientes v2
             LEFT JOIN PagosClientes p2 ON v2.VentaID = p2.VentaID
             WHERE v2.TipoVenta = 'CREDITO'
                AND CAST(v2.FechaVenta AS DATE) <= dr.Fecha
                AND (@SucursalID IS NULL OR v2.SucursalID = @SucursalID)
            ) AS SaldoAcumulado
        FROM DiasRango dr
        LEFT JOIN CreditosDia cd ON dr.Fecha = cd.Fecha
        LEFT JOIN CobrosDia co ON dr.Fecha = co.Fecha
    )
    SELECT 
        Fecha,
        TotalClientesCredito,
        CreditosOtorgados,
        NumeroVentasCredito,
        CobrosRealizados,
        NumeroPagos,
        -- Saldo inicial = saldo del día anterior
        LAG(SaldoAcumulado, 1, 0) OVER (ORDER BY Fecha) AS SaldoInicial,
        SaldoAcumulado AS SaldoFinal,
        -- Porcentaje de recuperación del día
        CASE 
            WHEN CreditosOtorgados > 0 
            THEN (CobrosRealizados / CreditosOtorgados) * 100
            ELSE 0 
        END AS PorcentajeRecuperacion,
        -- Eficiencia: Cobros vs Créditos otorgados históricamente
        CASE 
            WHEN SaldoAcumulado + CobrosRealizados > 0
            THEN (CobrosRealizados / (SaldoAcumulado + CobrosRealizados)) * 100
            ELSE 0
        END AS EficienciaCobranza,
        -- Cartera vigente y vencida
        (SELECT 
            SUM(CASE 
                WHEN DATEDIFF(DAY, v3.FechaVenta, @Fecha) <= 30 
                THEN v3.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v3.VentaID), 0)
                ELSE 0 
            END)
         FROM VentasClientes v3
         WHERE v3.TipoVenta = 'CREDITO'
            AND CAST(v3.FechaVenta AS DATE) <= Fecha
            AND (@SucursalID IS NULL OR v3.SucursalID = @SucursalID)
            AND v3.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v3.VentaID), 0)
        ) AS CarteraVigente,
        (SELECT 
            SUM(CASE 
                WHEN DATEDIFF(DAY, v3.FechaVenta, @Fecha) > 30 
                THEN v3.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v3.VentaID), 0)
                ELSE 0 
            END)
         FROM VentasClientes v3
         WHERE v3.TipoVenta = 'CREDITO'
            AND CAST(v3.FechaVenta AS DATE) <= Fecha
            AND (@SucursalID IS NULL OR v3.SucursalID = @SucursalID)
            AND v3.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v3.VentaID), 0)
        ) AS CarteraVencida,
        -- Porcentaje vencido
        CASE 
            WHEN SaldoAcumulado > 0
            THEN ((SELECT SUM(CASE WHEN DATEDIFF(DAY, v3.FechaVenta, @Fecha) > 30 THEN v3.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v3.VentaID), 0) ELSE 0 END)
                   FROM VentasClientes v3
                   WHERE v3.TipoVenta = 'CREDITO' AND CAST(v3.FechaVenta AS DATE) <= Fecha 
                   AND (@SucursalID IS NULL OR v3.SucursalID = @SucursalID)) 
                  / SaldoAcumulado) * 100
            ELSE 0
        END AS PorcentajeVencido
    FROM SaldoAcumulado
    ORDER BY Fecha
    OPTION (MAXRECURSION 1000);
END
GO

-- =============================================
-- 3. CARTERA DE CLIENTES DETALLADA
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
        tc.DiasCredito,
        
        -- Resumen de cuenta
        ISNULL(SUM(v.Total), 0) AS TotalVentas,
        ISNULL(SUM(p.Monto), 0) AS TotalPagos,
        ISNULL(SUM(v.Total), 0) - ISNULL(SUM(p.Monto), 0) AS SaldoPendiente,
        
        -- Antigüedad de saldo
        SUM(CASE 
            WHEN DATEDIFF(DAY, v.FechaVenta, @FechaCorte) <= 30 
            THEN v.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)
            ELSE 0 
        END) AS Vigente,
        
        SUM(CASE 
            WHEN DATEDIFF(DAY, v.FechaVenta, @FechaCorte) BETWEEN 31 AND 60 
            THEN v.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)
            ELSE 0 
        END) AS Vencido30,
        
        SUM(CASE 
            WHEN DATEDIFF(DAY, v.FechaVenta, @FechaCorte) BETWEEN 61 AND 90 
            THEN v.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)
            ELSE 0 
        END) AS Vencido60,
        
        SUM(CASE 
            WHEN DATEDIFF(DAY, v.FechaVenta, @FechaCorte) > 90 
            THEN v.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)
            ELSE 0 
        END) AS Vencido90,
        
        MAX(DATEDIFF(DAY, v.FechaVenta, @FechaCorte)) AS DiasVencido,
        
        -- Estado
        CASE 
            WHEN MAX(DATEDIFF(DAY, v.FechaVenta, @FechaCorte)) <= 30 THEN 'AL CORRIENTE'
            WHEN MAX(DATEDIFF(DAY, v.FechaVenta, @FechaCorte)) <= 60 THEN 'VENCIDO'
            ELSE 'MOROSO'
        END AS Estado,
        
        MAX(p.FechaPago) AS UltimoPago,
        
        MIN(DATEADD(DAY, tc.DiasCredito, v.FechaVenta)) AS ProximoVencimiento
        
    FROM Clientes c
    INNER JOIN ClienteTiposCredito ctc ON c.ClienteID = ctc.ClienteID
    INNER JOIN CatTiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
    LEFT JOIN VentasClientes v ON c.ClienteID = v.ClienteID 
        AND v.TipoVenta = 'CREDITO'
        AND v.FechaVenta <= @FechaCorte
        AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
    LEFT JOIN PagosClientes p ON v.VentaID = p.VentaID
    WHERE c.Estatus = 1
    GROUP BY 
        c.ClienteID, c.RFC, c.RazonSocial, tc.Nombre, tc.DiasCredito
    HAVING 
        ISNULL(SUM(v.Total), 0) - ISNULL(SUM(p.Monto), 0) > 0
    ORDER BY 
        SaldoPendiente DESC, DiasVencido DESC;
END
GO

PRINT '✅ Stored Procedures de Reportes Avanzados creados exitosamente';
