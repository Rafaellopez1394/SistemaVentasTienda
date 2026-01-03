using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class RolController : Controller
    {
        // GET: Rol
        public ActionResult Index()
        {
            return View("Crear");
        }

        // GET: Rol/Crear
        public ActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Obtener()
        {
            try
            {
                List<Rol> olista = CD_Rol.Instancia.ObtenerRol();
                var result = Json(new { data = olista }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new { data = new List<Rol>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
        }

        // GET: Rol/Listar - Para el modulo de configuracion
        [HttpGet]
        public JsonResult Listar()
        {
            List<Rol> olista = CD_Rol.Instancia.ObtenerRol();
            return Json(olista, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Guardar(Rol objeto)
        {
            bool respuesta = false;

            if (objeto.RolID == 0)
            {

                respuesta = CD_Rol.Instancia.RegistrarRol(objeto);
            }
            else
            {
                respuesta = CD_Rol.Instancia.ModificarRol(objeto);
            }


            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult Eliminar(int id = 0)
        {
            bool respuesta = CD_Rol.Instancia.EliminarRol(id);

            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }

    }
}