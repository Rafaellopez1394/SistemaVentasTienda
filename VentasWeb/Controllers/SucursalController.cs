using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class SucursalController : Controller
    {
        // GET: Sucursal
        public ActionResult Crear()
        {
            return View();
        }

        public JsonResult Obtener()
        {
            List<Sucursal> lista = CD_Sucursal.Instancia.ObtenerSucursales();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }
    
        [HttpPost]
        public JsonResult Guardar(Sucursal objeto)
        {
            bool respuesta = false;

            if (objeto.SucursalID == 0)
            {

                respuesta = CD_Sucursal.Instancia.RegistrarSucursal(objeto);
            }
            else
            {
                respuesta = CD_Sucursal.Instancia.ModificarSucursal(objeto);
            }


            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Eliminar(int id = 0)
        {
            bool respuesta = CD_Sucursal.Instancia.EliminarSucursal (id);

            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }
    }
}