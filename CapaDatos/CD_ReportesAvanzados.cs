using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CapaDatos
{
    public class CD_ReportesAvanzados
    {
        private static CD_ReportesAvanzados _instancia = null;

        private CD_ReportesAvanzados() { }

        public static CD_ReportesAvanzados Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_ReportesAvanzados();
                }
                return _instancia;
            }
        }

        #region REPORTES DE RENTABILIDAD

        /// <summary>
        /// Obtiene reporte detallado de utilidad por producto
        /// </summary>
        public List<ReporteUtilidadProducto> ObtenerUtilidadProductos(
            DateTime fechaInicio, 
            DateTime fechaFin, 
            int? productoID = null, 
            int? categoriaID = null, 
            int? sucursalID = null)
        {
            List<ReporteUtilidadProducto> lista = new List<ReporteUtilidadProducto>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ReporteUtilidadProducto", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@ProductoID", productoID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoriaID", categoriaID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    cnx.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteUtilidadProducto
                            {
                                ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                CodigoInterno = dr["CodigoInterno"].ToString(),
                                NombreProducto = dr["NombreProducto"].ToString(),
                                Categoria = dr["Categoria"].ToString(),
                                Presentacion = dr["Presentacion"].ToString(),
                                CantidadComprada = Convert.ToDecimal(dr["CantidadComprada"]),
                                CostoTotalCompras = Convert.ToDecimal(dr["CostoTotalCompras"]),
                                CostoPromedioCompra = Convert.ToDecimal(dr["CostoPromedioCompra"]),
                                CantidadVendida = Convert.ToDecimal(dr["CantidadVendida"]),
                                ImporteTotalVentas = Convert.ToDecimal(dr["ImporteTotalVentas"]),
                                PrecioPromedioVenta = Convert.ToDecimal(dr["PrecioPromedioVenta"]),
                                InventarioActual = Convert.ToDecimal(dr["InventarioActual"]),
                                ValorInventario = Convert.ToDecimal(dr["ValorInventario"]),
                                CostoVendido = Convert.ToDecimal(dr["CostoVendido"]),
                                UtilidadBruta = Convert.ToDecimal(dr["UtilidadBruta"]),
                                MargenUtilidadPorcentaje = Convert.ToDecimal(dr["MargenUtilidadPorcentaje"]),
                                Rentabilidad = dr["Rentabilidad"].ToString(),
                                Recomendacion = dr["Recomendacion"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener reporte de utilidad de productos: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Genera Estado de Resultados (P&L) para el período
        /// </summary>
        public EstadoResultadosVentas GenerarEstadoResultados(DateTime fechaInicio, DateTime fechaFin, int? sucursalID = null)
        {
            EstadoResultadosVentas er = new EstadoResultadosVentas()
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Periodo = ObtenerNombrePeriodo(fechaInicio, fechaFin)
            };

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();

                try
                {
                    // 1. INGRESOS
                    string queryVentas = @"
                        SELECT 
                            ISNULL(SUM(vd.Cantidad * vd.PrecioVenta), 0) AS VentasTotales,
                            ISNULL((SELECT SUM(dd.CantidadDevuelta * dd.PrecioVenta) 
                                    FROM DevolucionesDetalle dd 
                                    INNER JOIN Devoluciones d ON dd.DevolucionID = d.DevolucionID 
                                    WHERE d.FechaDevolucion BETWEEN @FechaInicio AND @FechaFin), 0) AS Devoluciones
                        FROM VentasDetalleClientes vd
                        INNER JOIN VentasClientes v ON vd.VentaID = v.VentaID
                        WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
                            AND (@SucursalID IS NULL OR vd.LoteID IN (
                                SELECT LoteID FROM LotesProducto WHERE SucursalID = @SucursalID
                            ))";

                    SqlCommand cmdVentas = new SqlCommand(queryVentas, cnx);
                    cmdVentas.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmdVentas.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmdVentas.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    using (SqlDataReader dr = cmdVentas.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            er.VentasTotales = Convert.ToDecimal(dr["VentasTotales"]);
                            er.Devoluciones = Convert.ToDecimal(dr["Devoluciones"]);
                        }
                    }

                    er.IngresosNetos = er.VentasTotales - er.Devoluciones;

                    // 2. COSTO DE VENTAS
                    string queryCostos = @"
                        SELECT 
                            ISNULL(SUM(vd.Cantidad * vd.PrecioCompra), 0) AS CostoVentas
                        FROM VentasDetalleClientes vd
                        INNER JOIN VentasClientes v ON vd.VentaID = v.VentaID
                        WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
                            AND (@SucursalID IS NULL OR vd.LoteID IN (
                                SELECT LoteID FROM LotesProducto WHERE SucursalID = @SucursalID
                            ))";

                    SqlCommand cmdCostos = new SqlCommand(queryCostos, cnx);
                    cmdCostos.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmdCostos.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmdCostos.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    er.CostoVentas = Convert.ToDecimal(cmdCostos.ExecuteScalar());

                    // Inventario (simplificado - podría mejorarse con valuación real)
                    string queryInventario = @"
                        SELECT 
                            ISNULL(SUM(lp.CantidadDisponible * lp.PrecioCompra), 0) AS ValorInventario
                        FROM LotesProducto lp
                        WHERE lp.Estatus = 1
                            AND (@SucursalID IS NULL OR lp.SucursalID = @SucursalID)";

                    SqlCommand cmdInv = new SqlCommand(queryInventario, cnx);
                    cmdInv.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    er.InventarioFinal = Convert.ToDecimal(cmdInv.ExecuteScalar());
                    er.InventarioInicial = er.InventarioFinal; // Simplificación

                    // 3. UTILIDAD BRUTA
                    er.UtilidadBruta = er.IngresosNetos - er.CostoVentas;
                    er.MargenBrutoPorcentaje = er.IngresosNetos > 0 ? (er.UtilidadBruta / er.IngresosNetos) * 100 : 0;

                    // 4. GASTOS OPERATIVOS
                    string queryGastos = @"
                        SELECT 
                            ISNULL(SUM(CASE WHEN cg.Nombre LIKE '%NÓMINA%' OR cg.Nombre LIKE '%SUELDO%' THEN g.Monto ELSE 0 END), 0) AS GastosNomina,
                            ISNULL(SUM(CASE WHEN cg.Nombre NOT LIKE '%NÓMINA%' AND cg.Nombre NOT LIKE '%SUELDO%' THEN g.Monto ELSE 0 END), 0) AS GastosOperativos
                        FROM Gastos g
                        LEFT JOIN CatCategoriasGastos cg ON g.CategoriaGastoID = cg.CategoriaGastoID
                        LEFT JOIN Cajas ca ON g.CajaID = ca.CajaID
                        WHERE g.FechaGasto BETWEEN @FechaInicio AND @FechaFin
                            AND g.Activo = 1
                            AND (@SucursalID IS NULL OR ca.SucursalID = @SucursalID)";

                    SqlCommand cmdGastos = new SqlCommand(queryGastos, cnx);
                    cmdGastos.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmdGastos.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmdGastos.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    using (SqlDataReader dr = cmdGastos.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            er.GastosNomina = Convert.ToDecimal(dr["GastosNomina"]);
                            er.GastosOperativos = Convert.ToDecimal(dr["GastosOperativos"]);
                        }
                    }

                    er.GastosTotales = er.GastosNomina + er.GastosOperativos;

                    // 5. UTILIDAD OPERATIVA
                    er.UtilidadOperativa = er.UtilidadBruta - er.GastosTotales;
                    er.MargenOperativoPorcentaje = er.IngresosNetos > 0 ? (er.UtilidadOperativa / er.IngresosNetos) * 100 : 0;

                    // 6. UTILIDAD NETA (simplificada)
                    er.UtilidadNeta = er.UtilidadOperativa;
                    er.MargenNetoPorcentaje = er.IngresosNetos > 0 ? (er.UtilidadNeta / er.IngresosNetos) * 100 : 0;

                    // 7. ANÁLISIS
                    if (er.UtilidadNeta > 0)
                    {
                        er.Conclusion = "✅ NEGOCIO RENTABLE - El establecimiento está generando utilidad neta.";
                        er.Recomendaciones = $"Margen neto: {er.MargenNetoPorcentaje:F2}%. ";
                        
                        if (er.MargenNetoPorcentaje >= 20)
                            er.Recomendaciones += "Excelente rentabilidad. Continuar estrategia actual.";
                        else if (er.MargenNetoPorcentaje >= 10)
                            er.Recomendaciones += "Rentabilidad aceptable. Buscar oportunidades de optimización.";
                        else
                            er.Recomendaciones += "Rentabilidad baja. Revisar costos y precios.";
                    }
                    else
                    {
                        er.Conclusion = "⚠️ PÉRDIDAS - El establecimiento está operando en números rojos.";
                        er.Recomendaciones = "URGENTE: Revisar estructura de costos, aumentar precios o reducir gastos operativos.";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al generar estado de resultados: " + ex.Message, ex);
                }
            }

            return er;
        }

        #endregion

        #region REPORTES DE CRÉDITO Y COBRANZA

        /// <summary>
        /// Obtiene concentrado de recuperación de crédito por día
        /// </summary>
        public List<ReporteRecuperacionCredito> ConcentradoRecuperacion(DateTime fechaInicio, DateTime fechaFin, int? sucursalID = null)
        {
            List<ReporteRecuperacionCredito> lista = new List<ReporteRecuperacionCredito>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ConcentradoRecuperacionCredito", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120; // 2 minutos para consultas complejas
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);

                    cnx.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteRecuperacionCredito
                            {
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                TotalClientesCredito = Convert.ToInt32(dr["TotalClientesCredito"]),
                                CreditosOtorgados = Convert.ToDecimal(dr["CreditosOtorgados"]),
                                NumeroVentasCredito = Convert.ToInt32(dr["NumeroVentasCredito"]),
                                CobrosRealizados = Convert.ToDecimal(dr["CobrosRealizados"]),
                                NumeroPagos = Convert.ToInt32(dr["NumeroPagos"]),
                                SaldoInicial = Convert.ToDecimal(dr["SaldoInicial"]),
                                SaldoFinal = Convert.ToDecimal(dr["SaldoFinal"]),
                                PorcentajeRecuperacion = Convert.ToDecimal(dr["PorcentajeRecuperacion"]),
                                EficienciaCobranza = Convert.ToDecimal(dr["EficienciaCobranza"]),
                                CarteraVigente = Convert.ToDecimal(dr["CarteraVigente"]),
                                CarteraVencida = Convert.ToDecimal(dr["CarteraVencida"]),
                                PorcentajeVencido = Convert.ToDecimal(dr["PorcentajeVencido"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener concentrado de recuperación: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene cartera de clientes con saldos pendientes
        /// </summary>
        public List<ReporteCarteraCliente> ObtenerCartera(DateTime? fechaCorte = null, int? sucursalID = null)
        {
            List<ReporteCarteraCliente> lista = new List<ReporteCarteraCliente>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    // Uso query con CTE para calcular aging correctamente
                    string query = @"
                        WITH VentasConSaldo AS (
                            SELECT 
                                v.ClienteID,
                                v.VentaID,
                                v.FechaVenta,
                                v.Total,
                                ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0) AS TotalPagado,
                                v.Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0) AS SaldoVenta,
                                DATEDIFF(DAY, v.FechaVenta, @FechaCorte) AS DiasTranscurridos
                            FROM VentasClientes v
                            WHERE v.TipoVenta = 'CREDITO'
                                AND v.FechaVenta <= @FechaCorte
                                AND v.Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)
                        )
                        SELECT 
                            c.ClienteID,
                            c.RFC,
                            c.RazonSocial,
                            ISNULL(tc.Nombre, 'Sin credito asignado') AS TipoCredito,
                            ISNULL(ctc.PlazoDias, 30) AS DiasCredito,
                            
                            ISNULL(SUM(vcs.Total), 0) AS TotalVentas,
                            ISNULL(SUM(vcs.TotalPagado), 0) AS TotalPagos,
                            ISNULL(SUM(vcs.SaldoVenta), 0) AS SaldoPendiente,
                            
                            -- Aging basado en dias de credito, no dias transcurridos
                            ISNULL(SUM(CASE WHEN vcs.DiasTranscurridos <= ISNULL(ctc.PlazoDias, 30) THEN vcs.SaldoVenta ELSE 0 END), 0) AS Vigente,
                            ISNULL(SUM(CASE WHEN vcs.DiasTranscurridos > ISNULL(ctc.PlazoDias, 30) AND vcs.DiasTranscurridos <= ISNULL(ctc.PlazoDias, 30) + 30 THEN vcs.SaldoVenta ELSE 0 END), 0) AS Vencido30,
                            ISNULL(SUM(CASE WHEN vcs.DiasTranscurridos > ISNULL(ctc.PlazoDias, 30) + 30 AND vcs.DiasTranscurridos <= ISNULL(ctc.PlazoDias, 30) + 60 THEN vcs.SaldoVenta ELSE 0 END), 0) AS Vencido60,
                            ISNULL(SUM(CASE WHEN vcs.DiasTranscurridos > ISNULL(ctc.PlazoDias, 30) + 60 THEN vcs.SaldoVenta ELSE 0 END), 0) AS Vencido90,
                            
                            -- Dias vencidos = DiasTranscurridos - DiasCredito (si es negativo, esta al corriente)
                            CASE 
                                WHEN MAX(vcs.DiasTranscurridos) - ISNULL(ctc.PlazoDias, 30) > 0 
                                THEN MAX(vcs.DiasTranscurridos) - ISNULL(ctc.PlazoDias, 30)
                                ELSE 0 
                            END AS DiasVencido,
                            
                            CASE 
                                WHEN MAX(vcs.DiasTranscurridos) <= ISNULL(ctc.PlazoDias, 30) THEN 'AL CORRIENTE'
                                WHEN MAX(vcs.DiasTranscurridos) <= ISNULL(ctc.PlazoDias, 30) + 30 THEN 'VENCIDO'
                                ELSE 'MOROSO'
                            END AS Estado
                        FROM Clientes c
                        LEFT JOIN ClienteTiposCredito ctc ON c.ClienteID = ctc.ClienteID AND ctc.Estatus = 1
                        LEFT JOIN CatTiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
                        INNER JOIN VentasConSaldo vcs ON c.ClienteID = vcs.ClienteID
                        WHERE c.Estatus = 1
                        GROUP BY c.ClienteID, c.RFC, c.RazonSocial, tc.Nombre, ctc.PlazoDias
                        HAVING ISNULL(SUM(vcs.SaldoVenta), 0) > 0
                        ORDER BY SaldoPendiente DESC";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@FechaCorte", fechaCorte ?? DateTime.Now);

                    cnx.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteCarteraCliente
                            {
                                ClienteID = (Guid)dr["ClienteID"],
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                TipoCredito = dr["TipoCredito"].ToString(),
                                DiasCredito = Convert.ToInt32(dr["DiasCredito"]),
                                TotalVentas = Convert.ToDecimal(dr["TotalVentas"]),
                                TotalPagos = Convert.ToDecimal(dr["TotalPagos"]),
                                SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                                Vigente = Convert.ToDecimal(dr["Vigente"]),
                                Vencido30 = Convert.ToDecimal(dr["Vencido30"]),
                                Vencido60 = Convert.ToDecimal(dr["Vencido60"]),
                                Vencido90 = Convert.ToDecimal(dr["Vencido90"]),
                                DiasVencido = Convert.ToInt32(dr["DiasVencido"]),
                                Estado = dr["Estado"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener cartera de clientes: " + ex.Message, ex);
                }
            }

            return lista;
        }

        #endregion

        #region DASHBOARD Y KPIs

        /// <summary>
        /// Obtiene KPIs principales para el dashboard
        /// </summary>
        public DashboardKPIs ObtenerKPIs(DateTime fecha, int? sucursalID = null)
        {
            DashboardKPIs kpis = new DashboardKPIs { Fecha = fecha };

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();

                try
                {
                    // Ventas de hoy y ayer
                    string queryVentas = @"
                        SELECT 
                            ISNULL(SUM(CASE WHEN CAST(FechaVenta AS DATE) = CAST(@Fecha AS DATE) THEN Total ELSE 0 END), 0) AS VentasHoy,
                            ISNULL(SUM(CASE WHEN CAST(FechaVenta AS DATE) = CAST(DATEADD(DAY, -1, @Fecha) AS DATE) THEN Total ELSE 0 END), 0) AS VentasAyer
                        FROM VentasClientes
                        WHERE FechaVenta >= CAST(DATEADD(DAY, -1, @Fecha) AS DATE)
                            AND FechaVenta < CAST(DATEADD(DAY, 1, @Fecha) AS DATE)";

                    SqlCommand cmdVentas = new SqlCommand(queryVentas, cnx);
                    cmdVentas.Parameters.AddWithValue("@Fecha", fecha);

                    using (SqlDataReader dr = cmdVentas.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            kpis.VentasHoy = Convert.ToDecimal(dr["VentasHoy"]);
                            kpis.VentasAyer = Convert.ToDecimal(dr["VentasAyer"]);
                            kpis.DiferenciaVentas = kpis.VentasHoy - kpis.VentasAyer;
                            kpis.PorcentajeCambio = kpis.VentasAyer > 0 ? (kpis.DiferenciaVentas / kpis.VentasAyer) * 100 : 0;
                        }
                    }

                    // Top productos del día
                    string queryTop = @"
                        SELECT TOP 5
                            p.Nombre AS NombreProducto,
                            SUM(vd.Cantidad) AS Cantidad,
                            SUM(vd.Cantidad * vd.PrecioVenta) AS Importe,
                            SUM(vd.Cantidad * (vd.PrecioVenta - ISNULL(vd.PrecioCompra, 0))) AS Utilidad
                        FROM VentasDetalleClientes vd
                        INNER JOIN VentasClientes v ON vd.VentaID = v.VentaID
                        INNER JOIN Productos p ON vd.ProductoID = p.ProductoID
                        WHERE CAST(v.FechaVenta AS DATE) = CAST(@Fecha AS DATE)
                        GROUP BY p.Nombre
                        ORDER BY Importe DESC";

                    SqlCommand cmdTop = new SqlCommand(queryTop, cnx);
                    cmdTop.Parameters.AddWithValue("@Fecha", fecha);

                    using (SqlDataReader dr = cmdTop.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            kpis.TopProductosDia.Add(new TopProducto
                            {
                                NombreProducto = dr["NombreProducto"].ToString(),
                                Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                Importe = Convert.ToDecimal(dr["Importe"]),
                                Utilidad = Convert.ToDecimal(dr["Utilidad"])
                            });
                        }
                    }

                    // Alertas
                    string queryAlertas = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Productos p 
                             WHERE p.StockMinimo > ISNULL((SELECT SUM(CantidadDisponible) FROM LotesProducto WHERE ProductoID = p.ProductoID AND Estatus = 1), 0)
                             AND p.Estatus = 1) AS ProductosBajoStock,
                            (SELECT COUNT(DISTINCT ClienteID) FROM VentasClientes 
                             WHERE TipoVenta = 'CREDITO' 
                             AND DATEDIFF(DAY, FechaVenta, GETDATE()) > 60
                             AND Total > ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = VentasClientes.VentaID), 0)) AS ClientesMorosos,
                            (SELECT ISNULL(SUM(Total - ISNULL((SELECT SUM(Monto) FROM PagosClientes WHERE VentaID = v.VentaID), 0)), 0)
                             FROM VentasClientes v 
                             WHERE TipoVenta = 'CREDITO' 
                             AND DATEDIFF(DAY, FechaVenta, GETDATE()) > 30) AS CarteraVencida";

                    SqlCommand cmdAlertas = new SqlCommand(queryAlertas, cnx);

                    using (SqlDataReader dr = cmdAlertas.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            kpis.ProductosBajoStock = Convert.ToInt32(dr["ProductosBajoStock"]);
                            kpis.ClientesMorosos = Convert.ToInt32(dr["ClientesMorosos"]);
                            kpis.CarteraVencida = Convert.ToDecimal(dr["CarteraVencida"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener KPIs: " + ex.Message, ex);
                }
            }

            return kpis;
        }

        #endregion

        #region MÉTODOS AUXILIARES

        private string ObtenerNombrePeriodo(DateTime inicio, DateTime fin)
        {
            CultureInfo ci = new CultureInfo("es-MX");
            
            if (inicio.Year == fin.Year && inicio.Month == fin.Month)
            {
                return $"{ci.DateTimeFormat.GetMonthName(inicio.Month)} {inicio.Year}";
            }
            else
            {
                return $"{inicio:dd/MMM/yyyy} - {fin:dd/MMM/yyyy}";
            }
        }

        #endregion
    }
}
