using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VentasWeb.Controllers;

namespace VentasWeb.Filters
{
    public class VerificarSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Verificar si la acción tiene el atributo AllowAnonymous
            var allowAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() ||
                                 filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

            if (allowAnonymous)
            {
                // Si tiene AllowAnonymous, permitir acceso sin validar sesión
                base.OnActionExecuting(filterContext);
                return;
            }

            Usuario oUsuario = (Usuario)HttpContext.Current.Session["Usuario"];

            if (oUsuario == null)
            {

                if (filterContext.Controller is LoginController == false)
                {
                    // Verificar si es una petición AJAX
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // Para peticiones AJAX, devolver respuesta JSON con error
                        filterContext.Result = new JsonResult
                        {
                            Data = new { resultado = false, message = "Sesión expirada", sessionExpired = true },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                        filterContext.HttpContext.Response.StatusCode = 401;
                    }
                    else
                    {
                        // Para peticiones normales, redirigir al login
                        filterContext.HttpContext.Response.Redirect("~/Login/Index");
                    }
                }
            }
            else
            {

                if (filterContext.Controller is LoginController == true)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/Index");
                }
            }


            base.OnActionExecuting(filterContext);
        }
    }
}