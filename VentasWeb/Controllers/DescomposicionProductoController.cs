// VentasWeb/Controllers/DescomposicionProductoController.cs
using CapaDatos;
using CapaModelo;
using System;
using System.Web.Mvc;
using VentasWeb.Filters;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class DescomposicionProductoController : Controller
    {
        // GET: Vista principal de descomposición
        public ActionResult Index()
        {
            return View();
        }

        // GET: Obtener productos para descomponer (con stock)
        [HttpGet]
        public JsonResult ObtenerProductosConStock(int sucursalID)
        {
            try
            {
                var productos = CD_Producto.Instancia.ObtenerProductosConStock(sucursalID);
                return Json(new { success = true, data = productos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener todos los productos para seleccionar como resultantes
        [HttpGet]
        public JsonResult ObtenerProductos()
        {
            try
            {
                var productos = CD_Producto.Instancia.ObtenerTodos();
                return Json(new { success = true, data = productos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Registrar descomposición
        [HttpPost]
        public JsonResult RegistrarDescomposicion(RegistrarDescomposicionPayload payload)
        {
            try
            {
                if (payload == null)
                    return Json(new { success = false, mensaje = "Datos de descomposición incompletos" });

                // Asignar usuario actual
                payload.Usuario = User.Identity.Name ?? "system";

                var respuesta = CD_DescomposicionProducto.Instancia.RegistrarDescomposicion(payload);
                
                return Json(new 
                { 
                    success = respuesta.Resultado, 
                    mensaje = respuesta.Mensaje,
                    descomposicionID = respuesta.Datos
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Obtener historial de descomposiciones
        [HttpGet]
        public JsonResult ObtenerHistorial()
        {
            try
            {
                var historial = CD_DescomposicionProducto.Instancia.ObtenerHistorial();
                return Json(new { success = true, data = historial }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener detalle de una descomposición
        [HttpGet]
        public JsonResult ObtenerDetalle(int id)
        {
            try
            {
                var descomposicion = CD_DescomposicionProducto.Instancia.ObtenerPorId(id);
                
                if (descomposicion == null)
                    return Json(new { success = false, mensaje = "Descomposición no encontrada" }, JsonRequestBehavior.AllowGet);
                
                return Json(new { success = true, data = descomposicion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Calcular precio por gramaje
        [HttpPost]
        public JsonResult CalcularPrecioPorGramaje(int productoID, decimal gramos)
        {
            try
            {
                var respuesta = CD_DescomposicionProducto.Instancia.CalcularPrecioPorGramaje(productoID, gramos);
                
                return Json(new 
                { 
                    success = respuesta.Resultado, 
                    mensaje = respuesta.Mensaje,
                    precioCalculado = respuesta.Datos
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Obtener productos configurados para venta por gramaje
        [HttpGet]
        public JsonResult ObtenerProductosGramaje()
        {
            try
            {
                var productos = CD_DescomposicionProducto.Instancia.ObtenerProductosGramaje();
                return Json(new { success = true, data = productos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
