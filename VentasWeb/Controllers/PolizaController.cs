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
    }
}
