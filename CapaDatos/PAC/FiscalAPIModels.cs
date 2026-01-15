using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CapaDatos.PAC
{
    #region Request Models - FiscalAPI v4 Official Structure

    /// <summary>
    /// Request para crear factura de ingreso en FiscalAPI v4
    /// Endpoint: POST /api/v4/invoices/income
    /// </summary>
    public class FiscalAPICrearCFDIRequest
    {
        [JsonProperty("versionCode")]
        public string VersionCode { get; set; } = "4.0";
        
        [JsonProperty("series")]
        public string Series { get; set; }
        
        [JsonProperty("date")]
        public string Date { get; set; } // Formato: YYYY-MM-DDThh:mm:ss
        
        [JsonProperty("paymentFormCode")]
        public string PaymentFormCode { get; set; } // c_FormaPago: 01, 02, 03, etc.
        
        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; } = "MXN";
        
        [JsonProperty("typeCode")]
        public string TypeCode { get; set; } = "I"; // I=Ingreso
        
        [JsonProperty("expeditionZipCode")]
        public string ExpeditionZipCode { get; set; } // CP del emisor
        
        [JsonProperty("exportCode")]
        public string ExportCode { get; set; } = "01"; // 01=No aplica
        
        [JsonProperty("paymentMethodCode")]
        public string PaymentMethodCode { get; set; } // PUE o PPD
        
        [JsonProperty("exchangeRate")]
        public decimal ExchangeRate { get; set; } = 1;
        
        [JsonProperty("issuer")]
        public IssuerModel Issuer { get; set; }
        
        [JsonProperty("recipient")]
        public RecipientModel Recipient { get; set; }
        
        [JsonProperty("items")]
        public List<InvoiceItemModel> Items { get; set; }
    }

    /// <summary>
    /// Emisor de la factura (InvoiceIssuer)
    /// </summary>
    public class IssuerModel
    {
        [JsonProperty("tin")]
        public string Tin { get; set; } // RFC
        
        [JsonProperty("legalName")]
        public string LegalName { get; set; }
        
        [JsonProperty("taxRegimeCode")]
        public string TaxRegimeCode { get; set; } // 601, 612, etc.
        
        [JsonProperty("taxCredentials")]
        public List<TaxCredentialModel> TaxCredentials { get; set; }
    }

    /// <summary>
    /// Certificado o llave privada (TaxCredential)
    /// </summary>
    public class TaxCredentialModel
    {
        [JsonProperty("base64File")]
        public string Base64File { get; set; }
        
        [JsonProperty("fileType")]
        public int FileType { get; set; } // 0=.cer, 1=.key
        
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Receptor de la factura (InvoiceRecipient)
    /// </summary>
    public class RecipientModel
    {
        [JsonProperty("tin")]
        public string Tin { get; set; } // RFC
        
        [JsonProperty("legalName")]
        public string LegalName { get; set; }
        
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; } // CP
        
        [JsonProperty("taxRegimeCode")]
        public string TaxRegimeCode { get; set; }
        
        [JsonProperty("cfdiUseCode")]
        public string CfdiUseCode { get; set; } // G01, G03, etc.
        
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Item/Concepto de la factura (InvoiceItem)
    /// </summary>
    public class InvoiceItemModel
    {
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; } // Clave producto/servicio SAT
        
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
        
        [JsonProperty("unitOfMeasurementCode")]
        public string UnitOfMeasurementCode { get; set; } // H87, E48, KGM, etc.
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }
        
        [JsonProperty("taxObjectCode")]
        public string TaxObjectCode { get; set; } // 01=No objeto, 02=Sí objeto
        
        [JsonProperty("itemSku")]
        public string ItemSku { get; set; } // NoIdentificacion
        
        [JsonProperty("discount")]
        public decimal? Discount { get; set; }
        
        [JsonProperty("itemTaxes")]
        public List<InvoiceItemTaxModel> ItemTaxes { get; set; }
    }

    /// <summary>
    /// Impuesto de un item (InvoiceItemTax)
    /// </summary>
    public class InvoiceItemTaxModel
    {
        [JsonProperty("taxCode")]
        public string TaxCode { get; set; } // 002=IVA, 003=IEPS
        
        [JsonProperty("taxTypeCode")]
        public string TaxTypeCode { get; set; } // Tasa, Cuota, Exento
        
        [JsonProperty("taxRate")]
        [JsonConverter(typeof(DecimalFormat6DigitsConverter))]
        public decimal TaxRate { get; set; } // 0.160000, 0.080000, etc. (6 decimales)
        
        [JsonProperty("taxFlagCode")]
        public string TaxFlagCode { get; set; } // T=Traslado, R=Retención
    }

    #endregion

    #region Response Models - FiscalAPI v4

    /// <summary>
    /// Respuesta de FiscalAPI al crear factura
    /// </summary>
    public class FiscalAPICrearCFDIResponse
    {
        [JsonProperty("data")]
        public FiscalAPIInvoiceData Data { get; set; }
        
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("details")]
        public string Details { get; set; }
        
        [JsonProperty("httpStatusCode")]
        public int HttpStatusCode { get; set; }
    }

    public class FiscalAPIInvoiceData
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        
        [JsonProperty("series")]
        public string Series { get; set; }
        
        [JsonProperty("number")]
        public string Number { get; set; }
        
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        
        [JsonProperty("total")]
        public decimal Total { get; set; }
        
        [JsonProperty("subtotal")]
        public decimal Subtotal { get; set; }
        
        [JsonProperty("responses")]
        public List<FiscalAPIStampResponse> Responses { get; set; }
    }

    public class FiscalAPIStampResponse
    {
        [JsonProperty("invoiceId")]
        public string InvoiceId { get; set; }
        
        [JsonProperty("invoiceUuid")]
        public string InvoiceUuid { get; set; }
        
        [JsonProperty("invoiceCertificateNumber")]
        public string InvoiceCertificateNumber { get; set; }
        
        [JsonProperty("invoiceBase64Sello")]
        public string InvoiceBase64Sello { get; set; }
        
        [JsonProperty("invoiceBase64QrCode")]
        public string InvoiceBase64QrCode { get; set; }
        
        [JsonProperty("invoiceBase64")]
        public string InvoiceBase64 { get; set; } // XML en Base64
        
        [JsonProperty("satBase64Sello")]
        public string SatBase64Sello { get; set; }
        
        [JsonProperty("satBase64OriginalString")]
        public string SatBase64OriginalString { get; set; }
        
        [JsonProperty("invoiceSignatureDate")]
        public DateTime InvoiceSignatureDate { get; set; }
        
        [JsonProperty("satCertificateNumber")]
        public string SatCertificateNumber { get; set; }
    }

    /// <summary>
    /// Respuesta de error de FiscalAPI
    /// </summary>
    public class FiscalAPIErrorResponse
    {
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("details")]
        public string Details { get; set; }
        
        [JsonProperty("httpStatusCode")]
        public int HttpStatusCode { get; set; }
    }

    #endregion

    #region Cancelación Models

    /// <summary>
    /// Request para cancelar CFDI
    /// </summary>
    public class FiscalAPICancelarRequest
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        
        [JsonProperty("motivo")]
        public string Motivo { get; set; } // 01, 02, 03, 04
        
        [JsonProperty("folioSustitucion")]
        public string FolioSustitucion { get; set; }
    }

    public class FiscalAPICancelarResponse
    {
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("data")]
        public FiscalAPICancelacionData Data { get; set; }
    }

    public class FiscalAPICancelacionData
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        
        [JsonProperty("estatusCancelacion")]
        public string EstatusCancelacion { get; set; }
        
        [JsonProperty("fechaCancelacion")]
        public DateTime? FechaCancelacion { get; set; }
    }

    #endregion

    #region JsonConverters

    /// <summary>
    /// Converter para asegurar que los decimales se serialicen con 6 dígitos
    /// Requerido por SAT para tasas de impuestos: 0.160000, no 0.16
    /// </summary>
    public class DecimalFormat6DigitsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            return Convert.ToDecimal(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var decimalValue = (decimal)value;
                writer.WriteValue(decimalValue.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }

    #endregion
}
