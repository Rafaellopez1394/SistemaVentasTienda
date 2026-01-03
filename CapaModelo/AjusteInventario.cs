using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class AjusteInventario
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ProductoId { get; set; }
        public long? LoteEntradaId { get; set; }
        public string Tipo { get; set; } // e.g., "AJUSTE", "MERMA"
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; }
        public int UsuarioId { get; set; }
    }
}


