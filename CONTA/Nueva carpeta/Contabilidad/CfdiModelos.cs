using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Entity.Contabilidad
{
    [XmlRoot("Comprobante", Namespace = "http://www.sat.gob.mx/cfd/4")]
    public class Comprobante
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("Serie")]
        public string Serie { get; set; }

        [XmlAttribute("Folio")]
        public string Folio { get; set; }

        [XmlAttribute("Fecha")]
        public string FechaString
        {
            get { return Fecha.ToString("yyyy-MM-ddTHH:mm:ss"); }
            set { Fecha = DateTime.Parse(value); }
        }

        [XmlIgnore]
        public DateTime Fecha { get; set; }

        // El resto de propiedades igual que antes...
        [XmlAttribute("Sello")]
        public string Sello { get; set; }

        [XmlAttribute("FormaPago")]
        public string FormaPago { get; set; }

        [XmlAttribute("NoCertificado")]
        public string NoCertificado { get; set; }

        [XmlAttribute("Certificado")]
        public string Certificado { get; set; }

        [XmlAttribute("CondicionesDePago")]
        public string CondicionesDePago { get; set; }

        [XmlAttribute("SubTotal")]
        public decimal SubTotal { get; set; }

        [XmlAttribute("Descuento")]
        public decimal Descuento { get; set; }

        [XmlAttribute("Moneda")]
        public string Moneda { get; set; }

        [XmlAttribute("TipoCambio")]
        public decimal TipoCambio { get; set; }

        [XmlAttribute("Total")]
        public decimal Total { get; set; }

        [XmlAttribute("TipoDeComprobante")]
        public string TipoDeComprobante { get; set; }

        [XmlAttribute("MetodoPago")]
        public string MetodoPago { get; set; }

        [XmlAttribute("LugarExpedicion")]
        public string LugarExpedicion { get; set; }

        [XmlAttribute("Confirmacion")]
        public string Confirmacion { get; set; }

        [XmlElement("Emisor")]
        public Emisor Emisor { get; set; }

        [XmlElement("Receptor")]
        public Receptor Receptor { get; set; }

        [XmlArray("Conceptos")]
        [XmlArrayItem("Concepto")]
        public List<Concepto> Conceptos { get; set; }

        [XmlElement("Impuestos")]
        public Impuestos Impuestos { get; set; }

        [XmlElement("Complemento")]
        public Complemento Complemento { get; set; }
        public int Exento { get; set; } // 0 = no Exento, 1 = Exento
        public int Contabilidad { get; set; } // 0 = no se encuentra en contabilidad, 1 = en Contabilidad
    }
    public class Emisor
    {
        [XmlAttribute("Rfc")]
        public string Rfc { get; set; }

        [XmlAttribute("Nombre")]
        public string Nombre { get; set; }

        [XmlAttribute("RegimenFiscal")]
        public string RegimenFiscal { get; set; }
    }

    public class Receptor
    {
        [XmlAttribute("Rfc")]
        public string Rfc { get; set; }

        [XmlAttribute("Nombre")]
        public string Nombre { get; set; }

        [XmlAttribute("UsoCFDI")]
        public string UsoCFDI { get; set; }

        [XmlAttribute("DomicilioFiscalReceptor")]
        public string DomicilioFiscalReceptor { get; set; }

        [XmlAttribute("RegimenFiscalReceptor")]
        public string RegimenFiscalReceptor { get; set; }
    }

    public class Concepto
    {
        [XmlAttribute("ClaveProdServ")]
        public string ClaveProdServ { get; set; }

        [XmlAttribute("NoIdentificacion")]
        public string NoIdentificacion { get; set; }

        [XmlAttribute("Cantidad")]
        public decimal Cantidad { get; set; }

        [XmlAttribute("ClaveUnidad")]
        public string ClaveUnidad { get; set; }

        [XmlAttribute("Unidad")]
        public string Unidad { get; set; }

        [XmlAttribute("Descripcion")]
        public string Descripcion { get; set; }

        [XmlAttribute("ValorUnitario")]
        public decimal ValorUnitario { get; set; }

        [XmlAttribute("Importe")]
        public decimal Importe { get; set; }

        [XmlAttribute("Descuento")]
        public decimal Descuento { get; set; }

        [XmlElement("Impuestos")]
        public ImpuestosConcepto Impuestos { get; set; }

        [XmlElement("InformacionAduanera")]
        public InformacionAduanera InformacionAduanera { get; set; }

        [XmlElement("CuentaPredial")]
        public CuentaPredial CuentaPredial { get; set; }
    }

    public class InformacionAduanera
    {
        [XmlAttribute("NumeroPedimento")]
        public string NumeroPedimento { get; set; }
    }

    public class CuentaPredial
    {
        [XmlAttribute("Numero")]
        public string Numero { get; set; }
    }

    public class ImpuestosConcepto
    {
        [XmlArray("Traslados")]
        [XmlArrayItem("Traslado")]
        public List<Traslado> Traslados { get; set; }

        [XmlArray("Retenciones")]
        [XmlArrayItem("Retencion")]
        public List<Retencion> Retenciones { get; set; }
    }

    public class CfdiArchivo
    {
        public Entity.Contabilidad.Comprobante Comprobante { get; set; }
        public string SourceName { get; set; }
    }
    public class Impuestos
    {
        [XmlArray("Traslados")]
        [XmlArrayItem("Traslado")]
        public List<Traslado> Traslados { get; set; }

        [XmlArray("Retenciones")]
        [XmlArrayItem("Retencion")]
        public List<Retencion> Retenciones { get; set; }

        [XmlAttribute("TotalImpuestosTrasladados")]
        public decimal TotalImpuestosTrasladados { get; set; }

        [XmlAttribute("TotalImpuestosRetenidos")]
        public decimal TotalImpuestosRetenidos { get; set; }
    }

    public class Traslado
    {
        [XmlAttribute("Base")]
        public decimal Base { get; set; }

        [XmlAttribute("Impuesto")]
        public string Impuesto { get; set; }

        [XmlAttribute("TipoFactor")]
        public string TipoFactor { get; set; }

        [XmlAttribute("TasaOCuota")]
        public decimal TasaOCuota { get; set; }

        [XmlAttribute("Importe")]
        public decimal Importe { get; set; }
    }

    public class Retencion
    {
        [XmlAttribute("Impuesto")]
        public string Impuesto { get; set; }

        [XmlAttribute("Importe")]
        public decimal Importe { get; set; }
    }

    public class Complemento
    {
        [XmlElement(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital", ElementName = "TimbreFiscalDigital")]
        public TimbreFiscalDigital TimbreFiscalDigital { get; set; }

        [XmlElement(Namespace = "http://www.sat.gob.mx/implocal", ElementName = "ImpuestosLocales")]
        public ImpuestosLocales ImpuestosLocales { get; set; }

        [XmlElement(Namespace = "http://www.sat.gob.mx/aerolineas", ElementName = "Aerolineas")]
        public Aerolineas Aerolineas { get; set; }

        [XmlElement(Namespace = "http://www.sat.gob.mx/donat", ElementName = "Donatarias")]
        public Donatarias Donatarias { get; set; }

    }

    public class Donatarias
    {
        [XmlAttribute("fechaAutorizacion")]
        public string FechaAutorizacion { get; set; }

        [XmlAttribute("leyenda")]
        public string Leyenda { get; set; }

        [XmlAttribute("noAutorizacion")]
        public string NoAutorizacion { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.sat.gob.mx/aerolineas")]
    public class Aerolineas
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("TUA")]
        public decimal TUA { get; set; }
    }
    [Serializable]
    [XmlType(Namespace = "http://www.sat.gob.mx/implocal")]
    public class TrasladoLocal
    {
        [XmlAttribute("ImpLocTrasladado")]
        public string ImpLocTrasladado { get; set; }

        [XmlAttribute("TasadeTraslado")]
        public decimal TasadeTraslado { get; set; }

        [XmlAttribute("Importe")]
        public decimal Importe { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.sat.gob.mx/implocal")]
    public class ImpuestosLocales
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("TotaldeRetenciones")]
        public decimal TotaldeRetenciones { get; set; }

        [XmlAttribute("TotaldeTraslados")]
        public decimal TotaldeTraslados { get; set; }

        [XmlElement("TrasladosLocales")]
        public List<TrasladoLocal> TrasladosLocales { get; set; }
    }
    public class TimbreFiscalDigital
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("UUID")]
        public string UUID { get; set; }

        [XmlAttribute("FechaTimbrado")]
        public DateTime FechaTimbrado { get; set; }

        [XmlAttribute("RfcProvCertif")]
        public string RfcProvCertif { get; set; }

        [XmlAttribute("SelloCFD")]
        public string SelloCFD { get; set; }

        [XmlAttribute("NoCertificadoSAT")]
        public string NoCertificadoSAT { get; set; }

        [XmlAttribute("SelloSAT")]
        public string SelloSAT { get; set; }
    }
}
