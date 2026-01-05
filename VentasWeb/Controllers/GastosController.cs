using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class GastosController : Controller
    {
        // GET: Gastos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Gastos/Registrar
        public ActionResult Registrar()
        {
            ViewBag.Categorias = CD_Gasto.Instancia.ObtenerCategoriasGastos();
            ViewBag.FormasPago = CD_VentaPOS.Instancia.ObtenerFormasPago();
            return View();
        }

        // POST: Gastos/Registrar
        [HttpPost]
        public JsonResult RegistrarGasto(Gasto gasto)
        {
            try
            {
                // Obtener usuario de sesión
                if (Session["Usuario"] != null)
                {
                    Usuario usuario = (Usuario)Session["Usuario"];
                    gasto.UsuarioRegistro = usuario.Nombres + " " + usuario.Apellidos;
                }
                else
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

                // Obtener caja activa si existe
                if (Session["CajaActiva"] != null)
                {
                    gasto.CajaID = (int)Session["CajaActiva"];
                }

                string mensaje;
                Guid gastoID = CD_Gasto.Instancia.RegistrarGasto(gasto, out mensaje);

                if (gastoID != Guid.Empty)
                {
                    return Json(new { success = true, mensaje = mensaje, gastoID = gastoID });
                }
                else
                {
                    return Json(new { success = false, mensaje = mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Gastos/ObtenerGastos
        [HttpGet]
        public JsonResult ObtenerGastos(string fechaInicio, string fechaFin, int? cajaID = null)
        {
            try
            {
                DateTime dtInicio = DateTime.Parse(fechaInicio);
                DateTime dtFin = DateTime.Parse(fechaFin);

                List<Gasto> gastos = CD_Gasto.Instancia.ObtenerGastosPorFecha(dtInicio, dtFin, cajaID);

                return Json(new { success = true, data = gastos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Gastos/ObtenerResumen
        [HttpGet]
        public JsonResult ObtenerResumen(string fechaInicio, string fechaFin, int? cajaID = null)
        {
            try
            {
                DateTime dtInicio = DateTime.Parse(fechaInicio);
                DateTime dtFin = DateTime.Parse(fechaFin);

                List<ResumenGastos> resumen = CD_Gasto.Instancia.ObtenerResumenGastos(dtInicio, dtFin, cajaID);

                return Json(new { success = true, data = resumen }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Gastos/Cancelar
        [HttpPost]
        public JsonResult CancelarGasto(string gastoID, string motivo)
        {
            try
            {
                Guid id = Guid.Parse(gastoID);
                string mensaje;
                bool resultado = CD_Gasto.Instancia.CancelarGasto(id, motivo, out mensaje);

                return Json(new { success = resultado, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Gastos/CierreCaja
        public ActionResult CierreCaja()
        {
            return View();
        }

        // GET: Gastos/ObtenerCierreCaja
        [HttpGet]
        public JsonResult ObtenerCierreCaja(int cajaID, string fecha = null)
        {
            try
            {
                DateTime? dtFecha = null;
                if (!string.IsNullOrEmpty(fecha))
                {
                    dtFecha = DateTime.Parse(fecha);
                }

                CierreCajaConGastos cierre = CD_Gasto.Instancia.ObtenerCierreCajaConGastos(cajaID, dtFecha);

                return Json(new { success = true, data = cierre }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Gastos/ObtenerCategorias
        [HttpGet]
        public JsonResult ObtenerCategorias()
        {
            try
            {
                List<CategoriaGasto> categorias = CD_Gasto.Instancia.ObtenerCategoriasGastos();
                return Json(new { success = true, data = categorias }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Gastos/ObtenerConcentradoGastosCierre
        [HttpGet]
        public JsonResult ObtenerConcentradoGastosCierre(int cajaID, string fecha)
        {
            try
            {
                DateTime dtFecha = DateTime.Parse(fecha);
                List<ConcentradoGastoCierre> concentrado = CD_Gasto.Instancia.ObtenerConcentradoGastosCierre(cajaID, dtFecha);
                
                return Json(new { success = true, data = concentrado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
