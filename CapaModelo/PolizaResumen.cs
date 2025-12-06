using System;

namespace CapaModelo
{
    public class PolizaResumen
    {
        public long PolizaID { get; set; }
        public DateTime FechaPoliza { get; set; }
        public string TipoPoliza { get; set; }
        public string Concepto { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
    }
}
