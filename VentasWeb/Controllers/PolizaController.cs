using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class PolizaController : Controller
    {
        // GET: Poliza
        public ActionResult Index()
        {
            return View();
        }

        // GET: Consultar Pólizas
        public ActionResult Consultar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Crear(Poliza poliza)
        {
            try
            {
                poliza.Usuario = User.Identity.Name ?? "system";
                bool resultado = CD_Poliza.Instancia.CrearPoliza(poliza);
                return Json(new { resultado });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Obtener(int top = 100)
        {
            var lista = CD_Poliza.Instancia.ObtenerUltimas(top);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerFiltradas(string fechaInicio = null, string fechaFin = null, string tipoPoliza = "")
        {
            var lista = CD_Poliza.Instancia.ObtenerFiltradas(fechaInicio, fechaFin, tipoPoliza);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerDetalle(Guid polizaId)
        {
            var poliza = CD_Poliza.Instancia.ObtenerPorId(polizaId);
            var detalles = CD_Poliza.Instancia.ObtenerDetalles(polizaId);
            return Json(new { poliza, detalles }, JsonRequestBehavior.AllowGet);
        }
    }
}
