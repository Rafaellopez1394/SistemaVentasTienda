// CapaModelo/Poliza.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Poliza
    {
        public Guid PolizaID { get; set; }
        public DateTime FechaPoliza { get; set; }
        public string TipoPoliza { get; set; }
        public string Referencia { get; set; }
        public string Concepto { get; set; }
        public string Usuario { get; set; }
        public long? ReferenciaId { get; set; }
        public string ReferenciaTipo { get; set; }
        public List<PolizaDetalle> Detalles { get; set; } = new List<PolizaDetalle>();
    }

    public class PolizaDetalle
    {
        public int PolizaDetalleID { get; set; }
        public Guid PolizaID { get; set; }
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public string Concepto { get; set; }
    }
}