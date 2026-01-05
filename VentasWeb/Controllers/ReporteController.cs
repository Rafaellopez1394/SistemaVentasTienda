using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                        + (sucursalFiltro > 0 ? " AND v.SucursalID = @SucursalID" : "")
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
                        query += " AND v.SucursalID = @SucursalID";
                        
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
                        query += " AND v.SucursalID = @SucursalID";
                        
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
                        query += " AND v.SucursalID = @SucursalID";
                    
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
    }
}
