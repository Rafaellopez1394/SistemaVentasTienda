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
        public ActionResult Index()
        {
            return View("Crear");
        }

        // GET: Usuario/Crear
        public ActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Obtener(int? id = null)
        {
            try
            {
                // Si hay id, devolver un solo usuario
                if (id.HasValue)
                {
                    var usuario = CD_Usuario.Instancia.ObtenerUsuarios().FirstOrDefault(u => u.UsuarioID == id.Value);
                    var result = Json(usuario, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
                
                // Si no hay id, devolver todos los usuarios para DataTables
                System.Diagnostics.Debug.WriteLine("=== Iniciando Obtener Usuarios ===");
                List<Usuario> oListaUsuario = CD_Usuario.Instancia.ObtenerUsuarios();
                System.Diagnostics.Debug.WriteLine($"Usuarios obtenidos: {oListaUsuario?.Count ?? 0}");
                
                if (oListaUsuario == null)
                {
                    System.Diagnostics.Debug.WriteLine("Lista es null, creando lista vacía");
                    oListaUsuario = new List<Usuario>();
                }
                
                System.Diagnostics.Debug.WriteLine("Creando respuesta JSON");
                var result2 = Json(new { data = oListaUsuario }, JsonRequestBehavior.AllowGet);
                result2.MaxJsonLength = int.MaxValue;
                System.Diagnostics.Debug.WriteLine("=== Obtener Usuarios completado ===");
                return result2;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en Obtener: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                var result = Json(new { data = new List<Usuario>(), error = ex.Message, stackTrace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
        }

        // GET: Usuario/Listar - Para el modulo de configuracion
        [HttpGet]
        public JsonResult Listar()
        {
            try
            {
                List<Usuario> oListaUsuario = CD_Usuario.Instancia.ObtenerUsuarios();
                var result = Json(oListaUsuario, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
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
                // Usuario nuevo - siempre encriptar la contraseña
                objeto.Clave = Encriptar.GetSHA256(objeto.Clave);
                respuesta = CD_Usuario.Instancia.RegistrarUsuario(objeto);
            }
            else
            {
                // Usuario existente
                if (string.IsNullOrEmpty(objeto.Clave))
                {
                    // Si la contraseña está vacía, obtener la actual de la BD
                    var usuarioActual = CD_Usuario.Instancia.ObtenerUsuarios()
                        .FirstOrDefault(u => u.UsuarioID == objeto.UsuarioID);
                    if (usuarioActual != null)
                    {
                        objeto.Clave = usuarioActual.Clave; // Mantener la contraseña actual
                    }
                }
                else if (objeto.Clave.Length != 64)
                {
                    // Si hay nueva contraseña y no está hasheada, encriptarla
                    objeto.Clave = Encriptar.GetSHA256(objeto.Clave);
                }
                // Si la contraseña tiene 64 chars, ya está hasheada (no hacer nada)
                
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