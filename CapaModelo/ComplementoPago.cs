using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Modelo para Complemento de Pago CFDI 2.0
    /// </summary>
    public class ComplementoPago
    {
        public int ComplementoPagoID { get; set; }
        
        // Datos del CFDI
        public Guid? UUID { get; set; }
        public string Serie { get; set; }
        public int? Folio { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        
        // Receptor
        public int ClienteID { get; set; }
        public string ReceptorRFC { get; set; }
        public string ReceptorNombre { get; set; }
        public string ReceptorDomicilioFiscal { get; set; }
        public string ReceptorRegimenFiscal { get; set; }
        public string ReceptorUsoCFDI { get; set; }
        
        // Totales
        public decimal MontoTotalPagos { get; set; }
        
        // Estado
        public string EstadoTimbrado { get; set; }
        
        // XMLs
        public string XMLSinTimbrar { get; set; }
        public string XMLTimbrado { get; set; }
        
        // Timbre
        public string SelloCFD { get; set; }
        public string SelloSAT { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string CadenaOriginal { get; set; }
        public string RFCProveedorCertificacion { get; set; }
        
        // Errores
        public string CodigoError { get; set; }
        public string MensajeError { get; set; }
        
        // Auditoría
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        
        // Navegación
        public List<ComplementoPagoPago> Pagos { get; set; }
        
        public ComplementoPago()
        {
            Pagos = new List<ComplementoPagoPago>();
            EstadoTimbrado = "PENDIENTE";
            FechaEmision = DateTime.Now;
            ReceptorRegimenFiscal = "616";
            ReceptorUsoCFDI = "CP01";
        }
    }
    
    /// <summary>
    /// Pago dentro de un complemento (puede haber varios pagos en un complemento)
    /// </summary>
    public class ComplementoPagoPago
    {
        public int PagoID { get; set; }
        public int ComplementoPagoID { get; set; }
        
        // Datos del pago
        public DateTime FechaPago { get; set; }
        public string FormaDePagoP { get; set; }
        public string MonedaP { get; set; }
        public decimal? TipoCambioP { get; set; }
        public decimal Monto { get; set; }
        
        // Datos bancarios ordenante
        public string NumOperacion { get; set; }
        public string RfcEmisorCtaOrd { get; set; }
        public string NomBancoOrdExt { get; set; }
        public string CtaOrdenante { get; set; }
        
        // Datos bancarios beneficiario
        public string RfcEmisorCtaBen { get; set; }
        public string CtaBeneficiario { get; set; }
        public string TipoCadPago { get; set; }
        public string CertPago { get; set; }
        public string CadPago { get; set; }
        public string SelloPago { get; set; }
        
        // Navegación
        public List<ComplementoPagoDocumento> DocumentosRelacionados { get; set; }
        
        public ComplementoPagoPago()
        {
            DocumentosRelacionados = new List<ComplementoPagoDocumento>();
            MonedaP = "MXN";
            FechaPago = DateTime.Now;
        }
    }
    
    /// <summary>
    /// Documento relacionado (factura) dentro de un pago
    /// </summary>
    public class ComplementoPagoDocumento
    {
        public int DocumentoID { get; set; }
        public int PagoID { get; set; }
        public int FacturaID { get; set; }
        
        // Identificación del documento
        public Guid IdDocumento { get; set; } // UUID de la factura
        public string UUIDDocumento { get; set; } // UUID como string
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string MonedaDR { get; set; }
        public decimal? EquivalenciaDR { get; set; }
        
        // Parcialidades
        public int NumParcialidad { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal ImpPagado { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        
        // Impuestos
        public string ObjetoImpDR { get; set; }
        
        // Navegación
        public List<ComplementoPagoImpuestoDR> ImpuestosDR { get; set; }
        
        public ComplementoPagoDocumento()
        {
            ImpuestosDR = new List<ComplementoPagoImpuestoDR>();
            MonedaDR = "MXN";
            EquivalenciaDR = 1;
            ObjetoImpDR = "02";
        }
    }
    
    /// <summary>
    /// Impuesto de un documento relacionado
    /// </summary>
    public class ComplementoPagoImpuestoDR
    {
        public int ImpuestoDRID { get; set; }
        public int DocumentoID { get; set; }
        
        public string TipoImpuesto { get; set; } // TRASLADO o RETENCION
        public decimal BaseDR { get; set; }
        public string ImpuestoDR { get; set; } // 002=IVA
        public string TipoFactorDR { get; set; }
        public decimal? TasaOCuotaDR { get; set; }
        public decimal? ImporteDR { get; set; }
    }
    
    /// <summary>
    /// Request para aplicar pagos a facturas
    /// </summary>
    public class AplicarPagoRequest
    {
        public int ClienteID { get; set; }
        public DateTime FechaPago { get; set; }
        public string FormaDePago { get; set; }
        public decimal MontoTotal { get; set; }
        
        // Datos bancarios opcionales
        public string NumOperacion { get; set; }
        public string CtaOrdenante { get; set; }
        public string CtaBeneficiario { get; set; }
        
        // Facturas a pagar
        public List<PagoFacturaItem> Facturas { get; set; }
        
        public AplicarPagoRequest()
        {
            Facturas = new List<PagoFacturaItem>();
        }
    }
    
    /// <summary>
    /// Item de factura con monto a pagar
    /// </summary>
    public class PagoFacturaItem
    {
        public int FacturaID { get; set; }
        public Guid UUID { get; set; }
        public string SerieFolio { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal MontoPagar { get; set; }
        public int NumeroParcialidad { get; set; }
    }
    
    /// <summary>
    /// Factura con saldo pendiente para listado
    /// </summary>
    public class FacturaPendientePago
    {
        public int FacturaID { get; set; }
        public Guid UUID { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public string SerieFolio { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int NumeroParcialidades { get; set; }
        public bool EsPagada { get; set; }
    }
}


