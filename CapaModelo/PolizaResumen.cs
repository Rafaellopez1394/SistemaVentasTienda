using System;

namespace CapaModelo
{
    public class PolizaResumen
    {
        public Guid PolizaID { get; set; }
        public DateTime FechaPoliza { get; set; }
        public string TipoPoliza { get; set; }
        public string Concepto { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public bool EsAutomatica { get; set; }
        public string Estatus { get; set; }
        public string PeriodoContable { get; set; }
        public bool? EsCuadrada { get; set; }
    }
}
