using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class TipoPrecio
    {
        public int TipoPrecioID { get; set; }  // Cambiado de "Id" a "TipoPrecioID" (más claro)
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public decimal Cargo { get; set; }
        public bool Activo { get; set; }
    }
}


