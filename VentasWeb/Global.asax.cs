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
    }

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Usuario"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // SI ESTÁ EN EL LOGIN → NO REDIRIGE (evita bucle)
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
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