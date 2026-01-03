using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace VentasWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // FILTRO GLOBAL, PERO EXCLUYE EL CONTROLADOR Login
            GlobalFilters.Filters.Add(new CustomAuthorizeAttribute(), 0);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Forzar UTF-8 en todas las respuestas
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.HeaderEncoding = System.Text.Encoding.UTF8;
        }
    }

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Usuario"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // SI ESTÁ EN EL LOGIN O TEST → NO REDIRIGE (evita bucle)
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (controllerName.Equals("Login", StringComparison.OrdinalIgnoreCase) || 
                controllerName.Equals("Test", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // SI ES UNA PETICIÓN AJAX → DEVUELVE JSON CON ERROR
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { error = "No autorizado. Por favor inicie sesión.", unauthorized = true },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.StatusCode = 401;
                return;
            }

            // SI NO ESTÁ LOGUEADO → LO MANDA AL LOGIN
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
        }
    }
}