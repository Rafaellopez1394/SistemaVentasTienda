using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class ConfiguracionSMTPController : Controller
    {
        // GET: ConfiguracionSMTP/Index
        public ActionResult Index()
        {
            // Validar sesión
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // GET: ConfiguracionSMTP/ObtenerConfiguracion
        [HttpGet]
        public JsonResult ObtenerConfiguracion()
        {
            try
            {
                var config = CD_ConfiguracionSMTP.Instancia.ObtenerConfiguracion();

                if (config == null)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "No se encontró configuración SMTP"
                    }, JsonRequestBehavior.AllowGet);
                }

                // No enviar la contraseña completa al frontend
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        config.ConfigID,
                        config.Host,
                        config.Puerto,
                        config.UsarSSL,
                        config.Usuario,
                        TieneContrasena = !string.IsNullOrEmpty(config.Contrasena),
                        config.EmailRemitente,
                        config.NombreRemitente,
                        config.Activo
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: ConfiguracionSMTP/Guardar
        [HttpPost]
        public JsonResult Guardar(ConfiguracionSMTP config, string contrasena)
        {
            try
            {
                if (Session["Usuario"] == null)
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

                string usuario = Session["Usuario"].ToString();

                // Si no se proporciona contraseña nueva, mantener la existente
                if (string.IsNullOrEmpty(contrasena) && config.ConfigID > 0)
                {
                    var configExistente = CD_ConfiguracionSMTP.Instancia.ObtenerConfiguracion();
                    config.Contrasena = configExistente.Contrasena;
                }
                else
                {
                    config.Contrasena = contrasena;
                }

                var respuesta = CD_ConfiguracionSMTP.Instancia.GuardarConfiguracion(config, usuario);

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                });
            }
        }

        // POST: ConfiguracionSMTP/ProbarConexion
        [HttpPost]
        public JsonResult ProbarConexion(ConfiguracionSMTP config, string contrasena)
        {
            try
            {
                // Si no se proporciona contraseña, usar la guardada
                if (string.IsNullOrEmpty(contrasena) && config.ConfigID > 0)
                {
                    var configExistente = CD_ConfiguracionSMTP.Instancia.ObtenerConfiguracion();
                    config.Contrasena = configExistente.Contrasena;
                }
                else
                {
                    config.Contrasena = contrasena;
                }

                var respuesta = CD_ConfiguracionSMTP.Instancia.ProbarConexion(config);

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                });
            }
        }
    }
}
