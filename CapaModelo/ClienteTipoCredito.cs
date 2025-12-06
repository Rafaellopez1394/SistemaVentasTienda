using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class ClienteTipoCredito
    {
        public int ClienteTipoCreditoID { get; set; }
        public Guid ClienteID { get; set; }
        public int TipoCreditoID { get; set; }

        // Estos campos vienen del SP ConsultarClienteTiposCreditoPorCliente
        public string Codigo { get; set; }           // CR001, CR002...
        public string TipoCredito { get; set; }      // Nombre del tipo (ej: "Crédito por cantidad de producto")
        public string Criterio { get; set; }         // Producto / Dinero / Tiempo
        public int? LimiteProducto { get; set; }
        public decimal? LimiteDinero { get; set; }
        public int? PlazoDias { get; set; }

        public DateTime FechaAsignacion { get; set; }
        public bool Estatus { get; set; }
        public string Usuario { get; set; }
        public DateTime UltimaAct { get; set; }

        // Viene del JOIN con Clientes en el SP
        public string RazonSocial { get; set; }
    }
}
