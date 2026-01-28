using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Factura Electrónica CFDI 4.0
    /// </summary>
    public class Factura
    {
        public Factura()
        {
            Conceptos = new List<FacturaDetalle>();
        }

        public Guid FacturaID { get; set; }
        public int IdFactura { get; set; } // ID autoincremental para referencias
        public Guid? VentaID { get; set; }
        
        // Comprobante
        public string Serie { get; set; }
        public string Folio { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Version { get; set; }
        public string TipoComprobante { get; set; } // I=Ingreso
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        
        // Montos
        public decimal Subtotal { get; set; }
        public decimal SubTotal { get; set; } // Alias para Subtotal
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public decimal MontoTotal { get; set; } // Alias para Total
        public decimal IVA { get; set; }
        public decimal TotalImpuestosTrasladados { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int? NumeroParcialidades { get; set; }
        public string Exportacion { get; set; } // 01=No aplica
        public string LugarExpedicion { get; set; } // CP donde se expide la factura
        
        // Emisor
        public string EmisorRFC { get; set; }
        public string RFCEmisor { get; set; } // Alias para EmisorRFC
        public string EmisorNombre { get; set; }
        public string NombreEmisor { get; set; } // Alias para EmisorNombre
        public string EmisorRegimenFiscal { get; set; }
        public string RegimenFiscalEmisor { get; set; } // Alias para EmisorRegimenFiscal
        public string CodigoPostalEmisor { get; set; }
        
        // Certificados del Emisor para FiscalAPI (Modo Por Valores) - Archivos desde CapaDatos/Certifies
        public string EmisorNombreArchivoCertificado { get; set; }      // Nombre archivo .cer
        public string EmisorNombreArchivoLlavePrivada { get; set; }     // Nombre archivo .key
        public string EmisorNombreArchivoPassword { get; set; }         // Nombre archivo password
        
        // Receptor
        public string ReceptorRFC { get; set; }
        public string ReceptorNombre { get; set; }
        public string ReceptorUsoCFDI { get; set; }
        public string ReceptorDomicilioFiscalCP { get; set; }
        public string ReceptorRegimenFiscalReceptor { get; set; }
        public string ReceptorEmail { get; set; }
        
        // Pago
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        
        // PAC (Finkok)
        public string UUID { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string NoCertificadoEmisor { get; set; }
        public string SelloCFD { get; set; }
        public string SelloSAT { get; set; }
        public string CadenaOriginalSAT { get; set; }
        public string ProveedorPAC { get; set; }
        public string QRCode { get; set; } // Código QR para factura
        
        // FiscalAPI
        public string FiscalAPIInvoiceId { get; set; } // ID de factura en FiscalAPI para descargar PDF
        
        // Archivos
        public string XMLOriginal { get; set; }
        public string XMLTimbrado { get; set; }
        public string RutaPDF { get; set; }
        
        // Estado
        public string Estatus { get; set; }
        public string MensajeError { get; set; }
        public bool EsCancelada { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string MotivoCancelacion { get; set; }
        public string FolioSustitucion { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        
        // Colecciones
        public List<FacturaDetalle> Conceptos { get; set; }
    }

    /// <summary>
    /// Detalle (conceptos) de una factura
    /// </summary>
    public class FacturaDetalle
    {
        public FacturaDetalle()
        {
            Impuestos = new List<FacturaImpuesto>();
        }

        public int FacturaDetalleID { get; set; }
        public Guid FacturaID { get; set; }
        public int Secuencia { get; set; }
        
        public string ClaveProdServ { get; set; }
        public string NoIdentificacion { get; set; }
        public decimal Cantidad { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal Descuento { get; set; }
        public string ObjetoImp { get; set; }
        
        public decimal TotalImpuestosTrasladados { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        
        public List<FacturaImpuesto> Impuestos { get; set; }
    }

    /// <summary>
    /// Impuestos de un concepto (traslados y retenciones)
    /// </summary>
    public class FacturaImpuesto
    {
        public int ImpuestoID { get; set; }
        public int FacturaDetalleID { get; set; }
        
        public string TipoImpuesto { get; set; } // TRASLADO, RETENCION
        public string Impuesto { get; set; } // 001=ISR, 002=IVA, 003=IEPS
        public string TipoFactor { get; set; } // Tasa, Cuota, Exento
        public decimal? TasaOCuota { get; set; }
        public decimal Base { get; set; }
        public decimal? Importe { get; set; }
    }

    /// <summary>
    /// Registro de cancelación de facturas
    /// </summary>
    public class FacturaCancelacion
    {
        public int CancelacionID { get; set; }
        public Guid FacturaID { get; set; }
        
        public DateTime FechaSolicitud { get; set; }
        public string MotivoCancelacion { get; set; }
        public string FolioSustitucion { get; set; }
        
        public string EstatusSAT { get; set; }
        public DateTime? FechaRespuestaSAT { get; set; }
        public string MensajeSAT { get; set; }
        public string AcuseCancelacion { get; set; }
        
        public string UsuarioSolicita { get; set; }
        public string Observaciones { get; set; }
    }

    /// <summary>
    /// Payload para generar factura desde venta
    /// </summary>
    public class GenerarFacturaRequest
    {
        public Guid VentaID { get; set; }
        public string ReceptorRFC { get; set; }
        public string ReceptorNombre { get; set; }
        public string ReceptorUsoCFDI { get; set; }
        public string UsoCFDI { get; set; } // Alias para compatibilidad
        public string ReceptorCP { get; set; }
        public string ReceptorRegimenFiscal { get; set; }
        public string ReceptorEmail { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
    }

    /// <summary>
    /// Respuesta de timbrado PAC
    /// </summary>
    public class RespuestaTimbrado
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public Guid? FacturaID { get; set; }
        public string UUID { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string XMLTimbrado { get; set; }
        public string SelloCFD { get; set; }
        public string SelloSAT { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string CadenaOriginal { get; set; }
        public string CodigoError { get; set; }
        public string ErrorTecnico { get; set; }
        public string PdfBase64 { get; set; } // PDF generado por el PAC
        public string InvoiceId { get; set; } // ID de la factura en FiscalAPI (para descargar PDF oficial)
    }

    /// <summary>
    /// Configuración del PAC (Finkok u otros)
    /// </summary>
    public class ConfiguracionPAC
    {
        public int ConfigID { get; set; }
        public string ProveedorPAC { get; set; }
        public bool EsProduccion { get; set; }
        public string UrlTimbrado { get; set; }
        public string UrlCancelacion { get; set; }
        public string UrlConsulta { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; } // Para FiscalAPI y otros PACs modernos
        public string RutaCertificado { get; set; }
        public string RutaLlavePrivada { get; set; }
        public string PasswordLlavePrivada { get; set; }
        public string PasswordLlave { get; set; } // Alias para PasswordLlavePrivada
        public int TimeoutSegundos { get; set; }
        public bool Activo { get; set; }
    }

    /// <summary>
    /// Respuesta de operación de cancelación de CFDI
    /// </summary>
    public class RespuestaCancelacionCFDI
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string CodigoError { get; set; }
        public string UUID { get; set; } // UUID del CFDI cancelado
        public string EstatusSAT { get; set; }
        public string EstatusUUID { get; set; }
        public string EstatusCancelacion { get; set; } // Estado de la cancelación
        public DateTime? FechaCancelacion { get; set; } // Fecha de cancelación
        public DateTime FechaRespuesta { get; set; }
        public string AcuseCancelacion { get; set; }
        public string AcuseXML { get; set; } // XML del acuse
        public string ErrorTecnico { get; set; }
    }
}


