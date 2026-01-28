using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;

namespace VentasWeb.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Reporte - Vista principal con análisis completo
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reporte/Producto
        public ActionResult Producto()
        {
            return View();
        }

        // GET: Reporte/Ventas
        public ActionResult Ventas()
        {
            return View();
        }

        // GET: Reporte/UtilidadDiaria
        public ActionResult UtilidadDiaria()
        {
            return View();
        }

        // API: Obtener reporte de producto
        public JsonResult ObtenerProducto(int SucursalID, string codigoproducto)
        {
            List<ReporteProducto> lista = CD_Reportes.Instancia.ReporteProductoSucursal(SucursalID, codigoproducto);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // API: Obtener reporte de ventas
        public JsonResult ObtenerVenta(string fechainicio, string fechafin, int SucursalID)
        {
            List<ReporteVenta> lista = CD_Reportes.Instancia.ReporteVenta(Convert.ToDateTime(fechainicio), Convert.ToDateTime(fechafin), SucursalID);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // API: Obtener ventas detalladas con análisis de productos, utilidades y precios
        [HttpGet]
        public JsonResult ObtenerVentasDetalladas(string fechaInicio, string fechaFin, int? productoId = null, string categoria = null, int? sucursalId = null)
        {
            try
            {
                // Si no se especifica sucursal, usar la activa
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var ventasDetalladas = new List<object>();
                
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(@"
                        SELECT 
                            v.VentaID,
                            CONCAT('V-', CAST(v.VentaID AS NVARCHAR(50))) AS NumeroVenta,
                            v.FechaVenta,
                            v.Total AS TotalVenta,
                            dv.ProductoID,
                            p.CodigoInterno AS CodigoProducto,
                            p.Nombre AS NombreProducto,
                            c.Nombre AS Categoria,
                            dv.Cantidad,
                            dv.PrecioVenta,
                            (dv.PrecioVenta * dv.Cantidad) AS TotalLinea,
                            COALESCE(dv.PrecioCompra, 0) AS PrecioCompra,
                            (dv.PrecioVenta - COALESCE(dv.PrecioCompra, 0)) AS UtilidadUnitaria,
                            ((dv.PrecioVenta - COALESCE(dv.PrecioCompra, 0)) * dv.Cantidad) AS UtilidadTotal,
                            CASE 
                                WHEN COALESCE(dv.PrecioCompra, 0) > 0 
                                THEN ((dv.PrecioVenta - dv.PrecioCompra) / dv.PrecioCompra * 100)
                                ELSE 0
                            END AS PorcentajeUtilidad
                        FROM VentasClientes v
                        INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
                        INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
                        LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                        WHERE v.FechaVenta >= @FechaInicio 
                        AND v.FechaVenta <= @FechaFin"
                        + (sucursalFiltro > 0 ? " AND v.VentaID IN (SELECT DISTINCT vd2.VentaID FROM VentasDetalleClientes vd2 INNER JOIN LotesProducto lp ON vd2.LoteID = lp.LoteID WHERE lp.SucursalID = @SucursalID)" : "")
                        + (productoId.HasValue ? " AND p.ProductoID = @ProductoID" : "") +
                        (!string.IsNullOrEmpty(categoria) ? " AND c.Nombre = @Categoria" : "") + @"
                        ORDER BY v.FechaVenta DESC, v.VentaID DESC
                    ", cnx);
                    
                    cmd.Parameters.AddWithValue("@FechaInicio", Convert.ToDateTime(fechaInicio));
                    cmd.Parameters.AddWithValue("@FechaFin", Convert.ToDateTime(fechaFin).AddDays(1).AddSeconds(-1));
                    
                    if (sucursalFiltro > 0)
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalFiltro);
                    
                    if (productoId.HasValue)
                        cmd.Parameters.AddWithValue("@ProductoID", productoId.Value);
                    
                    if (!string.IsNullOrEmpty(categoria))
                        cmd.Parameters.AddWithValue("@Categoria", categoria);
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ventasDetalladas.Add(new
                            {
                                VentaID = dr["VentaID"],
                                NumeroVenta = dr["NumeroVenta"],
                                FechaVenta = Convert.ToDateTime(dr["FechaVenta"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                ProductoID = dr["ProductoID"],
                                CodigoProducto = dr["CodigoProducto"],
                                NombreProducto = dr["NombreProducto"],
                                Categoria = dr["Categoria"] != DBNull.Value ? dr["Categoria"] : "Sin categoría",
                                Cantidad = dr["Cantidad"],
                                PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"]),
                                PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                                TotalLinea = Convert.ToDecimal(dr["TotalLinea"]),
                                UtilidadUnitaria = Convert.ToDecimal(dr["UtilidadUnitaria"]),
                                UtilidadTotal = Convert.ToDecimal(dr["UtilidadTotal"]),
                                PorcentajeUtilidad = Convert.ToDecimal(dr["PorcentajeUtilidad"])
                            });
                        }
                    }
                }

                return Json(new { success = true, data = ventasDetalladas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Obtener productos más vendidos
        [HttpGet]
        public JsonResult ObtenerProductosMasVendidos(string fechaInicio, string fechaFin, int top = 10, int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var productosMasVendidos = new List<object>();
                
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    string query = @"
                        SELECT TOP (@Top)
                            p.ProductoID,
                            p.CodigoInterno AS CodigoProducto,
                            p.Nombre AS NombreProducto,
                            c.Nombre AS Categoria,
                            SUM(dv.Cantidad) AS TotalUnidadesVendidas,
                            COUNT(DISTINCT v.VentaID) AS NumeroVentas,
                            SUM(dv.PrecioVenta * dv.Cantidad) AS TotalIngresos,
                            AVG(dv.PrecioVenta) AS PrecioPromedioVenta,
                            AVG(COALESCE(dv.PrecioCompra, 0)) AS PrecioPromedioCompra,
                            SUM((dv.PrecioVenta - COALESCE(dv.PrecioCompra, 0)) * dv.Cantidad) AS UtilidadTotal
                        FROM VentasDetalleClientes dv
                        INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
                        INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
                        LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                        WHERE v.FechaVenta >= @FechaInicio 
                        AND v.FechaVenta <= @FechaFin";
                    
                    if (sucursalFiltro > 0)
                        query += " AND v.VentaID IN (SELECT DISTINCT vd2.VentaID FROM VentasDetalleClientes vd2 INNER JOIN LotesProducto lp ON vd2.LoteID = lp.LoteID WHERE lp.SucursalID = @SucursalID)";
                        
                    query += @"
                        GROUP BY p.ProductoID, p.CodigoInterno, p.Nombre, c.Nombre
                        ORDER BY TotalUnidadesVendidas DESC";
                    
                    var cmd = new System.Data.SqlClient.SqlCommand(query, cnx);
                    
                    cmd.Parameters.AddWithValue("@Top", top);
                    cmd.Parameters.AddWithValue("@FechaInicio", Convert.ToDateTime(fechaInicio));
                    cmd.Parameters.AddWithValue("@FechaFin", Convert.ToDateTime(fechaFin).AddDays(1).AddSeconds(-1));
                    
                    if (sucursalFiltro > 0)
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalFiltro);
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            productosMasVendidos.Add(new
                            {
                                ProductoID = dr["ProductoID"],
                                CodigoProducto = dr["CodigoProducto"],
                                NombreProducto = dr["NombreProducto"],
                                Categoria = dr["Categoria"] != DBNull.Value ? dr["Categoria"] : "Sin categoría",
                                TotalUnidadesVendidas = dr["TotalUnidadesVendidas"],
                                NumeroVentas = dr["NumeroVentas"],
                                TotalIngresos = Convert.ToDecimal(dr["TotalIngresos"]),
                                PrecioPromedioVenta = Convert.ToDecimal(dr["PrecioPromedioVenta"]),
                                PrecioPromedioCompra = Convert.ToDecimal(dr["PrecioPromedioCompra"]),
                                UtilidadTotal = Convert.ToDecimal(dr["UtilidadTotal"])
                            });
                        }
                    }
                }

                return Json(new { success = true, data = productosMasVendidos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Obtener categorías con ventas
        [HttpGet]
        public JsonResult ObtenerVentasPorCategoria(string fechaInicio, string fechaFin, int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var ventasPorCategoria = new List<object>();
                
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    string query = @"
                        SELECT 
                            COALESCE(c.Nombre, 'Sin categoría') AS Categoria,
                            COUNT(DISTINCT v.VentaID) AS NumeroVentas,
                            SUM(dv.Cantidad) AS TotalUnidades,
                            SUM(dv.PrecioVenta * dv.Cantidad) AS TotalVentas,
                            AVG(dv.PrecioVenta) AS PrecioPromedio
                        FROM VentasDetalleClientes dv
                        INNER JOIN VentasClientes v ON dv.VentaID = v.VentaID
                        INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
                        LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                        WHERE v.FechaVenta >= @FechaInicio 
                        AND v.FechaVenta <= @FechaFin";
                    
                    if (sucursalFiltro > 0)
                        query += " AND v.VentaID IN (SELECT DISTINCT vd2.VentaID FROM VentasDetalleClientes vd2 INNER JOIN LotesProducto lp ON vd2.LoteID = lp.LoteID WHERE lp.SucursalID = @SucursalID)";
                        
                    query += @"
                        GROUP BY c.Nombre
                        ORDER BY TotalVentas DESC";
                    
                    var cmd = new System.Data.SqlClient.SqlCommand(query, cnx);
                    
                    cmd.Parameters.AddWithValue("@FechaInicio", Convert.ToDateTime(fechaInicio));
                    cmd.Parameters.AddWithValue("@FechaFin", Convert.ToDateTime(fechaFin).AddDays(1).AddSeconds(-1));
                    
                    if (sucursalFiltro > 0)
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalFiltro);
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ventasPorCategoria.Add(new
                            {
                                Categoria = dr["Categoria"],
                                NumeroVentas = dr["NumeroVentas"],
                                TotalUnidades = dr["TotalUnidades"],
                                TotalVentas = Convert.ToDecimal(dr["TotalVentas"]),
                                PrecioPromedio = Convert.ToDecimal(dr["PrecioPromedio"])
                            });
                        }
                    }
                }

                return Json(new { success = true, data = ventasPorCategoria }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Obtener estadísticas generales
        [HttpGet]
        public JsonResult ObtenerEstadisticasGenerales(string fechaInicio, string fechaFin, int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var estadisticas = new
                {
                    TotalVentas = 0m,
                    TotalUtilidad = 0m,
                    PromedioVenta = 0m,
                    TotalUnidadesVendidas = 0,
                    NumeroVentas = 0,
                    PorcentajeUtilidadPromedio = 0m
                };

                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    string query = @"
                        SELECT 
                            SUM(dv.PrecioVenta * dv.Cantidad) AS TotalVentas,
                            SUM((dv.PrecioVenta - COALESCE(dv.PrecioCompra, 0)) * dv.Cantidad) AS TotalUtilidad,
                            AVG(v.Total) AS PromedioVenta,
                            SUM(dv.Cantidad) AS TotalUnidadesVendidas,
                            COUNT(DISTINCT v.VentaID) AS NumeroVentas,
                            CASE 
                                WHEN SUM(COALESCE(dv.PrecioCompra, 0) * dv.Cantidad) > 0
                                THEN (SUM((dv.PrecioVenta - COALESCE(dv.PrecioCompra, 0)) * dv.Cantidad) / SUM(COALESCE(dv.PrecioCompra, 0) * dv.Cantidad) * 100)
                                ELSE 0
                            END AS PorcentajeUtilidadPromedio
                        FROM VentasClientes v
                        INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
                        WHERE v.FechaVenta >= @FechaInicio 
                        AND v.FechaVenta <= @FechaFin";
                    
                    if (sucursalFiltro > 0)
                        query += " AND v.VentaID IN (SELECT DISTINCT vd2.VentaID FROM VentasDetalleClientes vd2 INNER JOIN LotesProducto lp ON vd2.LoteID = lp.LoteID WHERE lp.SucursalID = @SucursalID)";
                    
                    var cmd = new System.Data.SqlClient.SqlCommand(query, cnx);
                    
                    cmd.Parameters.AddWithValue("@FechaInicio", Convert.ToDateTime(fechaInicio));
                    cmd.Parameters.AddWithValue("@FechaFin", Convert.ToDateTime(fechaFin).AddDays(1).AddSeconds(-1));
                    
                    if (sucursalFiltro > 0)
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalFiltro);
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            estadisticas = new
                            {
                                TotalVentas = dr["TotalVentas"] != DBNull.Value ? Convert.ToDecimal(dr["TotalVentas"]) : 0m,
                                TotalUtilidad = dr["TotalUtilidad"] != DBNull.Value ? Convert.ToDecimal(dr["TotalUtilidad"]) : 0m,
                                PromedioVenta = dr["PromedioVenta"] != DBNull.Value ? Convert.ToDecimal(dr["PromedioVenta"]) : 0m,
                                TotalUnidadesVendidas = dr["TotalUnidadesVendidas"] != DBNull.Value ? Convert.ToInt32(dr["TotalUnidadesVendidas"]) : 0,
                                NumeroVentas = dr["NumeroVentas"] != DBNull.Value ? Convert.ToInt32(dr["NumeroVentas"]) : 0,
                                PorcentajeUtilidadPromedio = dr["PorcentajeUtilidadPromedio"] != DBNull.Value ? Convert.ToDecimal(dr["PorcentajeUtilidadPromedio"]) : 0m
                            };
                        }
                    }
                }

                return Json(new { success = true, data = estadisticas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // REPORTE DIARIO: Obtener preview del reporte de utilidad
        [HttpGet]
        public JsonResult ObtenerPreviewUtilidadDiaria(string fecha = null)
        {
            try
            {
                DateTime fechaReporte = string.IsNullOrEmpty(fecha) 
                    ? DateTime.Now.Date 
                    : DateTime.ParseExact(fecha, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                
                int sucursalID = Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 1;

                var reporte = CD_ReporteUtilidadDiaria.Instancia.ObtenerReporteDiario(fechaReporte, sucursalID);

                var preview = new
                {
                    Fecha = reporte.Fecha.ToString("yyyy-MM-dd"),
                    SucursalID = reporte.SucursalID,
                    
                    ResumenVentas = reporte.ResumenVentas.Select(r => new
                    {
                        FormaPago = r.FormaPago,
                        NumeroTickets = r.Tickets,
                        UnidadesVendidas = r.TotalUnidades,
                        TotalVentas = Math.Round(r.TotalVentas, 2)
                    }).ToList(),
                    
                    TotalVentasContado = Math.Round(reporte.TotalVentasContado, 2),
                    TotalVentasCredito = Math.Round(reporte.TotalVentasCredito, 2),
                    TotalVentas = Math.Round(reporte.TotalVentasContado + reporte.TotalVentasCredito, 2),
                    TotalTickets = reporte.TotalTickets,
                    TotalUnidades = reporte.TotalUnidades,
                    
                    CostosCompra = Math.Round(reporte.CostosCompra, 2),
                    UtilidadDiaria = Math.Round(reporte.UtilidadDiaria, 2),
                    PorcentajeUtilidad = reporte.CostosCompra > 0 
                        ? Math.Round((reporte.UtilidadDiaria / reporte.CostosCompra) * 100, 2) 
                        : 0,
                    
                    RecuperoCreditosTotal = Math.Round(reporte.RecuperoCreditosTotal, 2),
                    
                    DetallePorProducto = reporte.DetalleVentas.Take(20).Select(d => new
                    {
                        Producto = d.Producto,
                        UnidadesContado = d.VentasContado,
                        UnidadesCredito = d.VentasCredito,
                        TotalVentas = d.TotalVentas,
                        CostoTotal = Math.Round(d.CostoTotal, 2),
                        Utilidad = Math.Round(d.Utilidad, 2)
                    }).ToList()
                };

                return Json(new { success = true, data = preview }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // REPORTE DIARIO: Exportar utilidad diaria a Excel
        [HttpPost]
        public ActionResult ExportarUtilidadDiaria(string fecha = null)
        {
            try
            {
                DateTime fechaReporte = string.IsNullOrEmpty(fecha) 
                    ? DateTime.Now.Date 
                    : DateTime.ParseExact(fecha, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                
                int sucursalID = Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 1;

                var reporte = CD_ReporteUtilidadDiaria.Instancia.ObtenerReporteDiario(fechaReporte, sucursalID);

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Utilidad Diaria");
                    
                    // Estilos
                    var headerFont = ws.Cells["A1"].Style.Font;
                    var currencyFormat = "$#,##0.00";
                    var percentFormat = "0.00%";
                    
                    int row = 1;

                    // Título principal
                    ws.Cells[row, 1].Value = "REPORTE DE UTILIDAD DIARIA";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 14;
                    ws.Cells[row, 1, row, 6].Merge = true;
                    row += 2;

                    ws.Cells[row, 1].Value = "Fecha: " + fechaReporte.ToString("dd/MM/yyyy");
                    ws.Cells[row, 4].Value = "Sucursal ID: " + sucursalID;
                    row += 2;

                    // SECCIÓN 1: RESUMEN DE VENTAS
                    ws.Cells[row, 1].Value = "RESUMEN DE VENTAS";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 51, 102));
                    ws.Cells[row, 1, row, 5].Merge = true;
                    row++;

                    ws.Cells[row, 1].Value = "Forma de Pago";
                    ws.Cells[row, 2].Value = "# Tickets";
                    ws.Cells[row, 3].Value = "Unidades";
                    ws.Cells[row, 4].Value = "Total Ventas";
                    ws.Cells[row, 5].Value = "% del Total";
                    
                    for (int i = 1; i <= 5; i++)
                    {
                        ws.Cells[row, i].Style.Font.Bold = true;
                        ws.Cells[row, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[row, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    row++;

                    decimal totalVentas = reporte.TotalVentasContado + reporte.TotalVentasCredito;

                    foreach (var venta in reporte.ResumenVentas)
                    {
                        ws.Cells[row, 1].Value = venta.FormaPago;
                        ws.Cells[row, 2].Value = venta.Tickets;
                        ws.Cells[row, 3].Value = venta.TotalUnidades;
                        ws.Cells[row, 4].Value = venta.TotalVentas;
                        ws.Cells[row, 4].Style.Numberformat.Format = currencyFormat;
                        ws.Cells[row, 5].Value = totalVentas > 0 ? (venta.TotalVentas / totalVentas) : 0;
                        ws.Cells[row, 5].Style.Numberformat.Format = percentFormat;
                        row++;
                    }

                    ws.Cells[row, 1].Value = "TOTAL";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 2].Value = reporte.TotalTickets;
                    ws.Cells[row, 2].Style.Font.Bold = true;
                    ws.Cells[row, 3].Value = reporte.TotalUnidades;
                    ws.Cells[row, 3].Style.Font.Bold = true;
                    ws.Cells[row, 4].Value = totalVentas;
                    ws.Cells[row, 4].Style.Font.Bold = true;
                    ws.Cells[row, 4].Style.Numberformat.Format = currencyFormat;
                    ws.Cells[row, 5].Value = 1;
                    ws.Cells[row, 5].Style.Font.Bold = true;
                    ws.Cells[row, 5].Style.Numberformat.Format = percentFormat;
                    row += 2;

                    // SECCIÓN 2: COSTOS Y UTILIDAD
                    ws.Cells[row, 1].Value = "ANÁLISIS DE COSTOS Y UTILIDAD";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 51, 102));
                    ws.Cells[row, 1, row, 5].Merge = true;
                    row++;

                    ws.Cells[row, 1].Value = "Concepto";
                    ws.Cells[row, 2].Value = "Monto";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 2].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[row, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    row++;

                    ws.Cells[row, 1].Value = "Total Ventas";
                    ws.Cells[row, 2].Value = totalVentas;
                    ws.Cells[row, 2].Style.Numberformat.Format = currencyFormat;
                    row++;

                    ws.Cells[row, 1].Value = "Costo de Mercancía Vendida";
                    ws.Cells[row, 2].Value = reporte.CostosCompra;
                    ws.Cells[row, 2].Style.Numberformat.Format = currencyFormat;
                    row++;

                    ws.Cells[row, 1].Value = "UTILIDAD DIARIA";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 2].Value = reporte.UtilidadDiaria;
                    ws.Cells[row, 2].Style.Font.Bold = true;
                    ws.Cells[row, 2].Style.Numberformat.Format = currencyFormat;
                    row++;

                    ws.Cells[row, 1].Value = "% Utilidad";
                    ws.Cells[row, 2].Value = reporte.CostosCompra > 0 ? (reporte.UtilidadDiaria / reporte.CostosCompra) : 0;
                    ws.Cells[row, 2].Style.Numberformat.Format = percentFormat;
                    row += 2;

                    // SECCIÓN 3: RECUPERO DE CRÉDITOS
                    ws.Cells[row, 1].Value = "RECUPERO DE CRÉDITOS";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 51, 102));
                    ws.Cells[row, 1, row, 3].Merge = true;
                    row++;

                    ws.Cells[row, 1].Value = "Créditos Recuperados";
                    ws.Cells[row, 2].Value = reporte.RecuperoCreditosTotal;
                    ws.Cells[row, 2].Style.Numberformat.Format = currencyFormat;
                    row += 2;

                    // SECCIÓN 4: TOP 20 PRODUCTOS
                    ws.Cells[row, 1].Value = "TOP 20 PRODUCTOS MÁS VENDIDOS";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 51, 102));
                    ws.Cells[row, 1, row, 6].Merge = true;
                    row++;

                    ws.Cells[row, 1].Value = "Producto";
                    ws.Cells[row, 2].Value = "Contado";
                    ws.Cells[row, 3].Value = "Crédito";
                    ws.Cells[row, 4].Value = "Total Ventas";
                    ws.Cells[row, 5].Value = "Costo Total";
                    ws.Cells[row, 6].Value = "Utilidad";
                    
                    for (int i = 1; i <= 6; i++)
                    {
                        ws.Cells[row, i].Style.Font.Bold = true;
                        ws.Cells[row, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[row, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    row++;

                    foreach (var prod in reporte.DetalleVentas.Take(20))
                    {
                        ws.Cells[row, 1].Value = prod.Producto;
                        ws.Cells[row, 2].Value = prod.VentasContado;
                        ws.Cells[row, 3].Value = prod.VentasCredito;
                        ws.Cells[row, 4].Value = prod.TotalVentas;
                        ws.Cells[row, 4].Style.Numberformat.Format = currencyFormat;
                        ws.Cells[row, 5].Value = prod.CostoTotal;
                        ws.Cells[row, 5].Style.Numberformat.Format = currencyFormat;
                        ws.Cells[row, 6].Value = prod.Utilidad;
                        ws.Cells[row, 6].Style.Numberformat.Format = currencyFormat;
                        row++;
                    }

                    // Ajustar anchos de columna
                    ws.Column(1).Width = 30;
                    ws.Column(2).Width = 15;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 18;
                    ws.Column(5).Width = 18;
                    ws.Column(6).Width = 18;

                    string fileName = "UtilidadDiaria_" + fechaReporte.ToString("yyyyMMdd") + ".xlsx";
                    return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al generar reporte: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
