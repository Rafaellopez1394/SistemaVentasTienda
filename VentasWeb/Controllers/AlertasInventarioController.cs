using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class AlertasInventarioController : Controller
    {
        // GET: AlertasInventario
        public ActionResult Index()
        {
            return View();
        }

        // GET: AlertasInventario/ObtenerAlertas
        [HttpGet]
        public JsonResult ObtenerAlertas(int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var alertas = CD_Producto.Instancia.ObtenerProductosBajoStockMinimo(
                    sucursalFiltro > 0 ? (int?)sucursalFiltro : null
                );

                return Json(new { success = true, data = alertas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: AlertasInventario/ObtenerConteo
        [HttpGet]
        public JsonResult ObtenerConteo(int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var conteo = CD_Producto.Instancia.ObtenerConteoAlertas(
                    sucursalFiltro > 0 ? (int?)sucursalFiltro : null
                );

                return Json(new { success = true, data = conteo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: AlertasInventario/ActualizarStockMinimo
        [HttpPost]
        public JsonResult ActualizarStockMinimo(int productoId, int stockMinimo)
        {
            try
            {
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "UPDATE Productos SET StockMinimo = @StockMinimo WHERE ProductoID = @ProductoID", 
                        cnx
                    );
                    cmd.Parameters.AddWithValue("@StockMinimo", stockMinimo);
                    cmd.Parameters.AddWithValue("@ProductoID", productoId);
                    
                    int rows = cmd.ExecuteNonQuery();
                    
                    if (rows > 0)
                        return Json(new { success = true, mensaje = "Stock mínimo actualizado correctamente" });
                    else
                        return Json(new { success = false, mensaje = "No se pudo actualizar el stock mínimo" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: AlertasInventario/ExportarReporte
        [HttpGet]
        public ActionResult ExportarReporte(int? sucursalId = null)
        {
            try
            {
                int sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 0);
                
                var alertas = CD_Producto.Instancia.ObtenerProductosBajoStockMinimo(
                    sucursalFiltro > 0 ? (int?)sucursalFiltro : null
                );

                // Generar CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Código,Producto,Categoría,Stock Actual,Stock Mínimo,Diferencia,% Stock,Nivel Alerta,Sucursal,Última Compra,Días Sin Compra");

                foreach (var alerta in alertas)
                {
                    csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6:F2},{7},{8},{9},{10}",
                        alerta.CodigoInterno,
                        alerta.NombreProducto.Replace(",", ";"),
                        alerta.Categoria.Replace(",", ";"),
                        alerta.StockActual,
                        alerta.StockMinimo,
                        alerta.Diferencia,
                        alerta.PorcentajeStock,
                        alerta.NivelAlerta,
                        alerta.NombreSucursal.Replace(",", ";"),
                        alerta.UltimaCompra?.ToString("yyyy-MM-dd") ?? "N/A",
                        alerta.DiasDesdeUltimaCompra
                    ));
                }

                var fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"Alertas_Inventario_{fecha}.csv";
                
                return File(
                    System.Text.Encoding.UTF8.GetBytes(csv.ToString()),
                    "text/csv",
                    fileName
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
