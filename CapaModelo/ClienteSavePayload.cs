using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class ClienteSavePayload
    {
        public Cliente objeto { get; set; }
        public List<int> tiposCreditoIDs { get; set; }
    }
}
