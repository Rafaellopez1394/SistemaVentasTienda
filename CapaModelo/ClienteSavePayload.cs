using System;
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class ClienteSavePayload
    {
        public Cliente objeto { get; set; }
        public List<CreditoConLimite> creditosConLimites { get; set; }
    }

    public class CreditoConLimite
    {
        public int tipoCreditoID { get; set; }
        public decimal? limiteDinero { get; set; }
        public int? limiteProducto { get; set; }
        public int? plazoDias { get; set; }
    }
}


