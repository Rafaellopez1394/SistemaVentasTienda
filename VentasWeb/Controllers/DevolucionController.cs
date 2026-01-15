// VentasWeb/Controllers/DevolucionController.cs
using CapaDatos;
using CapaModelo;
using System;
using System.Linq;
using System.Web.Mvc;
using VentasWeb.Filters;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class DevolucionController : Controller
    {
        // GET: Devolucion/Index - Historial
        public ActionResult Index()
        {
            return View();
        }

        // GET: Devolucion/Registrar - Formulario de registro
        public ActionResult Registrar()
        {
            return View();
        }

        // GET: Devolucion/Reportes - Vista de reportes
        public ActionResult Reportes()
        {
            return View();
        }

        // API: Obtener lista de devoluciones con filtros
        [HttpGet]
        public JsonResult ObtenerDevoluciones(string fechaInicio = null, string fechaFin = null, int? sucursalId = null)
        {
            try
            {
                DateTime? fInicio = !string.IsNullOrEmpty(fechaInicio) ? DateTime.Parse(fechaInicio) : (DateTime?)null;
                DateTime? fFin = !string.IsNullOrEmpty(fechaFin) ? DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                
                int? sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null ? (int?)Session["SucursalActiva"] : null);
                
                var devoluciones = CD_Devolucion.Instancia.ObtenerDevoluciones(fInicio, fFin, sucursalFiltro);
                
                return Json(new { success = true, data = devoluciones }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Obtener detalle de una devolución
        [HttpGet]
        public JsonResult ObtenerDetalle(int devolucionId)
        {
            try
            {
                var devolucion = CD_Devolucion.Instancia.ObtenerDetalle(devolucionId);
                
                if (devolucion == null)
                    return Json(new { success = false, mensaje = "Devolución no encontrada" }, JsonRequestBehavior.AllowGet);
                
                return Json(new { success = true, data = devolucion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Buscar venta por número para devolver
        [HttpGet]
        public JsonResult BuscarVentaPorNumero(string numeroVenta)
        {
            try
            {
                // Buscar venta
                var venta = CD_VentaPOS.Instancia.BuscarVentaPorNumero(numeroVenta);
                
                if (venta == null)
                    return Json(new { success = false, mensaje = "Venta no encontrada" }, JsonRequestBehavior.AllowGet);
                
                // Obtener detalle completo
                var ventaDetalle = CD_Devolucion.Instancia.ObtenerDetalleVentaParaDevolucion(venta.VentaID);
                
                if (ventaDetalle == null)
                    return Json(new { success = false, mensaje = "No se pudo obtener el detalle de la venta" }, JsonRequestBehavior.AllowGet);
                
                // Verificar si ya tiene devoluciones
                var devolucionesExistentes = CD_Devolucion.Instancia.ObtenerDevoluciones(ventaID: venta.VentaID);
                
                if (devolucionesExistentes != null && devolucionesExistentes.Any())
                {
                    bool tieneDevolucionTotal = devolucionesExistentes.Any(d => d.TipoDevolucion == "TOTAL");
                    
                    if (tieneDevolucionTotal)
                        return Json(new { 
                            success = false, 
                            mensaje = "Esta venta ya tiene una devolución total registrada" 
                        }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { 
                    success = true, 
                    data = ventaDetalle,
                    devolucionesPrevias = devolucionesExistentes?.Count ?? 0
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Registrar devolución
        [HttpPost]
        public JsonResult RegistrarDevolucion(RegistrarDevolucionPayload payload)
        {
            try
            {
                if (payload == null || payload.Productos == null || !payload.Productos.Any())
                    return Json(new { success = false, mensaje = "Debe especificar los productos a devolver" });
                
                // Establecer usuario y sucursal
                payload.UsuarioRegistro = User.Identity.Name ?? "system";
                payload.SucursalID = Session["SucursalActiva"] != null 
                    ? (int)Session["SucursalActiva"] 
                    : 1;
                
                // Validar tipo de devolución
                if (payload.TipoDevolucion != "TOTAL" && payload.TipoDevolucion != "PARCIAL")
                    return Json(new { success = false, mensaje = "Tipo de devolución inválido" });
                
                // Validar forma de reintegro
                if (payload.FormaReintegro != "EFECTIVO" && 
                    payload.FormaReintegro != "CREDITO_CLIENTE" && 
                    payload.FormaReintegro != "CAMBIO_PRODUCTO")
                    return Json(new { success = false, mensaje = "Forma de reintegro inválida" });
                
                // Registrar
                var respuesta = CD_Devolucion.Instancia.RegistrarDevolucion(payload);
                
                return Json(new { 
                    success = respuesta.estado, 
                    mensaje = respuesta.valor,
                    devolucionID = respuesta.objeto
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // API: Obtener reporte de devoluciones
        [HttpGet]
        public JsonResult ObtenerReporte(string fechaInicio, string fechaFin, int? sucursalId = null)
        {
            try
            {
                DateTime fInicio = DateTime.Parse(fechaInicio);
                DateTime fFin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
                
                int? sucursalFiltro = sucursalId ?? (Session["SucursalActiva"] != null ? (int?)Session["SucursalActiva"] : null);
                
                var reporte = CD_Devolucion.Instancia.ObtenerReporte(fInicio, fFin, sucursalFiltro);
                
                return Json(new { success = true, data = reporte }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Obtener productos más devueltos
        [HttpGet]
        public JsonResult ObtenerProductosMasDevueltos(string fechaInicio, string fechaFin, int top = 20)
        {
            try
            {
                DateTime fInicio = DateTime.Parse(fechaInicio);
                DateTime fFin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
                
                var productos = CD_Devolucion.Instancia.ObtenerProductosMasDevueltos(fInicio, fFin, top);
                
                return Json(new { success = true, data = productos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
