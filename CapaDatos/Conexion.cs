using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class Conexion
    {
        public static string CN;

        static Conexion()
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["miconexion"];
                if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
                    CN = cs.ConnectionString;
                else
                    CN = "Data Source=.;Initial Catalog=DB_TIENDA;Integrated Security=True"; // fallback for tooling/tests
            }
            catch
            {
                CN = "Data Source=.;Initial Catalog=DB_TIENDA;Integrated Security=True"; // fallback
            }
        }
    }
}
