using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class TipoFinanciamiento
    {
        public int TipoFinanciamientoID { get; set; }
        public string Clave { get; set; }     // ej: "dias", "monto", "unidades"
        public string Nombre { get; set; }    // ej: "Crédito 30 días", "Monto fijo", etc.
    }
}


