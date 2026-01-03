// CapaModelo/Poliza.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Poliza
    {
        public Poliza()
        {
            Detalles = new List<PolizaDetalle>();
        }

        public Guid PolizaID { get; set; }
        public DateTime FechaPoliza { get; set; }
        public string TipoPoliza { get; set; }
        public string Referencia { get; set; }
        public string Concepto { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime FechaAlta { get; set; }
        public long? ReferenciaId { get; set; }
        public string ReferenciaTipo { get; set; }
        
        // Campos nuevos para cumplimiento SAT
        public bool EsAutomatica { get; set; }
        public string DocumentoOrigen { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public string Estatus { get; set; }
        public string PeriodoContable { get; set; }
        public string RelacionCFDI { get; set; }
        public string Observaciones { get; set; }
        public bool? EsCuadrada { get; set; }
        
        public List<PolizaDetalle> Detalles { get; set; }
        
        // Propiedades de compatibilidad
        public decimal Importe { get; set; }
        public List<PolizaDetalle> ListaPolizaDetalle { get { return Detalles; } set { Detalles = value; } }
    }

    public class PolizaDetalle
    {
        public int DetalleID { get; set; }
        public Guid PolizaDetalleID { get; set; } // Compatibilidad
        public Guid PolizaID { get; set; }
        public Guid? CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public string Concepto { get; set; }
        
        // Propiedades de compatibilidad
        public string TipMov { get; set; }
        public string Tip_Mov { get { return TipMov; } set { TipMov = value; } }
        public decimal Cantidad { get; set; }
        public decimal Importe { get; set; }
        public string Estatus { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
    }
}

