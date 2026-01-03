using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class MovimientoInventario
    {
        public int MovimientoID { get; set; }
        public int LoteID { get; set; }
        public int ProductoID { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentarios { get; set; }
    }

}


