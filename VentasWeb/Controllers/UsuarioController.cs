using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VentasWeb.Utilidades;

namespace VentasWeb.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Crear()
        {
            return View();
        }

        public JsonResult Obtener()
        {
            List<Usuario> oListaUsuario = CD_Usuario.Instancia.ObtenerUsuarios();
            return Json(new { data = oListaUsuario }, JsonRequestBehavior.AllowGet);
        }

        // GET: Usuario/Listar - Para el modulo de configuracion
        [HttpGet]
        public JsonResult Listar()
        {
            List<Usuario> oListaUsuario = CD_Usuario.Instancia.ObtenerUsuarios();
            return Json(oListaUsuario, JsonRequestBehavior.AllowGet);
        }

        // GET: Usuario/Obtener?id=X - Obtener un usuario por ID
        [HttpGet]
        public JsonResult Obtener(int id)
        {
            var usuario = CD_Usuario.Instancia.ObtenerUsuarios().FirstOrDefault(u => u.UsuarioID == id);
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }

        // POST: Usuario/CambiarEstado
        [HttpPost]
        public JsonResult CambiarEstado(int id, bool activo)
        {
            try
            {
                bool resultado = CD_Usuario.Instancia.CambiarEstadoUsuario(id, !activo);
                return Json(new { valor = resultado, msg = resultado ? "Estado actualizado" : "Error al actualizar" });
            }
            catch (Exception ex)
            {
                return Json(new { valor = false, msg = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Guardar(Usuario objeto)
        {
            bool respuesta = false;

            if (objeto.UsuarioID == 0)
            {
                objeto.Clave = Encriptar.GetSHA256(objeto.Clave);

                respuesta = CD_Usuario.Instancia.RegistrarUsuario(objeto);
            }
            else
            {
                respuesta = CD_Usuario.Instancia.ModificarUsuario(objeto);
            }


            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Eliminar(int id = 0)
        {
            bool respuesta = CD_Usuario.Instancia.EliminarUsuario(id);

            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }

    }
}