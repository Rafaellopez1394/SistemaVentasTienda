using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class SubMenu
    {
        public int SubMenuID { get; set; }
        public int MenuID { get; set; }
        public string Nombre { get; set; }
        public string Controlador { get; set; }
        public string Vista { get; set; }
        public string Icono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
