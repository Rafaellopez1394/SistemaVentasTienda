using System;
using System.Web;

namespace VentasWeb.Helpers
{
    public static class PermisoHelper
    {
        public static string ObtenerRolUsuario()
        {
            if (HttpContext.Current.Session["UsuarioRol"] != null)
            {
                return HttpContext.Current.Session["UsuarioRol"].ToString();
            }
            return "EMPLEADO";
        }

        public static bool EsAdministrador()
        {
            return ObtenerRolUsuario().Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase);
        }

        public static bool EsContador()
        {
            return ObtenerRolUsuario().Equals("CONTADOR", StringComparison.OrdinalIgnoreCase);
        }

        public static bool EsEmpleado()
        {
            return ObtenerRolUsuario().Equals("EMPLEADO", StringComparison.OrdinalIgnoreCase);
        }

        public static bool TienePermiso(params string[] roles)
        {
            string rolActual = ObtenerRolUsuario();
            foreach (string rol in roles)
            {
                if (rolActual.Equals(rol, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
