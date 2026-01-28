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
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            string passport = Encriptar.GetSHA256(clave);
            // Buscar por Nombres (usuario) en lugar de Correo
            Usuario ousuario = CD_Usuario.Instancia.ObtenerUsuarios().Where(u => u.Nombres.Equals(correo, StringComparison.OrdinalIgnoreCase) && u.Clave == Encriptar.GetSHA256(clave) ).FirstOrDefault();

            if (ousuario == null)
            {
                ViewBag.Error = "Usuario o contraseña no correcta";
                return View();
            }

            // Establecer correo por defecto si no tiene
            if (string.IsNullOrEmpty(ousuario.Correo) || ousuario.Correo == "lasaguilasmercadodelmar@gmail.com")
            {
                // El correo ya está configurado por defecto en la BD
            }

            // Guardar usuario completo en sesión incluyendo rol
            Session["Usuario"] = ousuario;
            Session["UsuarioRol"] = ousuario.oRol != null ? ousuario.oRol.Descripcion : "EMPLEADO";
            Session["UsuarioNombre"] = ousuario.Nombres + " " + ousuario.Apellidos;
            
            // IMPORTANTE: Asegurar que la sesión se persista
            Session.Timeout = 30; // 30 minutos
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            // Redirigir según el rol del usuario
            string rol = ousuario.oRol != null ? ousuario.oRol.Descripcion.ToUpper() : "EMPLEADO";
            
            if (rol == "ADMINISTRADOR")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Usuarios no administradores van directo a punto de venta
                return RedirectToAction("Index", "VentaPOS");
            }
        }
    }
}