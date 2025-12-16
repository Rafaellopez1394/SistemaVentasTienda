using CapaModelo;
using CapaDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class HomeController : Controller
    {
        private static Usuario SesionUsuario;
        public ActionResult Index()
        {
            if (Session["Usuario"] != null)
                SesionUsuario = (Usuario)Session["Usuario"];
            else {
                SesionUsuario = new Usuario();
            }
            try
            {
                ViewBag.NombreUsuario = SesionUsuario.Nombres + " " + SesionUsuario.Apellidos;
                ViewBag.RolUsuario = SesionUsuario.oRol.Descripcion;

            }
            catch {

            }

           
            return View();
        }

        public ActionResult Salir()
        {
            Session["Usuario"] = null;
            return RedirectToAction("Index", "Login");
        }

        // GET: Home/ObtenerDashboardData
        [HttpGet]
        public JsonResult ObtenerDashboardData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Iniciando carga de datos del dashboard ===");
                
                DateTime hoy = DateTime.Today;
                DateTime inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);

                System.Diagnostics.Debug.WriteLine($"Fecha hoy: {hoy:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"Inicio semana: {inicioSemana:yyyy-MM-dd}");

                // Obtener ventas del día usando ReporteVenta
                var ventasHoy = CD_Reportes.Instancia.ReporteVenta(hoy, hoy, 0);
                System.Diagnostics.Debug.WriteLine($"Ventas hoy encontradas: {ventasHoy?.Count ?? 0}");
                
                decimal totalVentasHoy = 0;
                if (ventasHoy != null)
                {
                    foreach (var venta in ventasHoy)
                    {
                        decimal total = 0;
                        if (decimal.TryParse(venta.TotalVenta, out total))
                        {
                            totalVentasHoy += total;
                        }
                    }
                }
                int transaccionesHoy = ventasHoy?.Count ?? 0;
                System.Diagnostics.Debug.WriteLine($"Total ventas hoy: ${totalVentasHoy:N2}");

                // Obtener ventas de la semana para el gráfico
                List<decimal> ventasSemana = new List<decimal>();
                List<string> diasSemana = new List<string> { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
                
                for (int i = 0; i < 7; i++)
                {
                    DateTime dia = inicioSemana.AddDays(i);
                    var ventasDia = CD_Reportes.Instancia.ReporteVenta(dia, dia, 0);
                    decimal totalDia = 0;
                    if (ventasDia != null)
                    {
                        foreach (var venta in ventasDia)
                        {
                            decimal total = 0;
                            if (decimal.TryParse(venta.TotalVenta, out total))
                            {
                                totalDia += total;
                            }
                        }
                    }
                    ventasSemana.Add(totalDia);
                    System.Diagnostics.Debug.WriteLine($"{diasSemana[i]}: ${totalDia:N2}");
                }

                // Contar clientes activos
                var clientes = CD_Cliente.Instancia.ObtenerTodos();
                int clientesActivos = clientes?.Count ?? 0;
                System.Diagnostics.Debug.WriteLine($"Clientes activos: {clientesActivos}");

                // Contar productos
                var productos = CD_Producto.Instancia.ObtenerTodos();
                int productosStock = productos?.Count ?? 0;
                int productosStockBajo = 0;
                System.Diagnostics.Debug.WriteLine($"Productos en stock: {productosStock}");

                // Top 5 productos más vendidos
                var topProductos = CD_Reportes.Instancia.ReporteProductoSucursal(0, "")?.Take(5).ToList();
                System.Diagnostics.Debug.WriteLine($"Top productos: {topProductos?.Count ?? 0}");

                // Ventas recientes (últimas 10)
                var ventasRecientes = CD_Reportes.Instancia.ReporteVenta(hoy.AddDays(-7), hoy, 0)?.Take(10).ToList();
                System.Diagnostics.Debug.WriteLine($"Ventas recientes: {ventasRecientes?.Count ?? 0}");

                System.Diagnostics.Debug.WriteLine("=== Datos del dashboard cargados exitosamente ===");

                return Json(new
                {
                    success = true,
                    ventasHoy = totalVentasHoy,
                    transaccionesHoy = transaccionesHoy,
                    clientesActivos = clientesActivos,
                    productosStock = productosStock,
                    productosStockBajo = productosStockBajo,
                    ventasSemana = ventasSemana,
                    diasSemana = diasSemana,
                    topProductos = topProductos,
                    ventasRecientes = ventasRecientes
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ObtenerDashboardData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                return Json(new
                {
                    success = false,
                    mensaje = ex.Message,
                    detalle = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}