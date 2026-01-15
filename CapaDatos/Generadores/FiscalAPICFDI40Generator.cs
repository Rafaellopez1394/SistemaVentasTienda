using System;
using System.Collections.Generic;
using System.Linq;
using CapaModelo;
using CapaDatos.PAC;

namespace CapaDatos.Generadores
{
    /// <summary>
    /// Generador de estructura JSON CFDI 4.0 para FiscalAPI
    /// Compatible con .NET Framework 4.6
    /// </summary>
    public class FiscalAPICFDI40Generator
    {
        private readonly ConfiguracionFiscalAPI _config;

        public FiscalAPICFDI40Generator(ConfiguracionFiscalAPI config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Genera el request completo para FiscalAPI desde una factura del sistema
        /// </summary>
        public FiscalAPICrearCFDIRequest GenerarRequest(Factura factura)
        {
            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            if (factura.Conceptos == null || !factura.Conceptos.Any())
                throw new ArgumentException("La factura debe tener al menos un detalle");

            var request = new FiscalAPICrearCFDIRequest
            {
                // Datos generales del comprobante
                VersionCode = "4.0",
                Series = string.IsNullOrEmpty(factura.Serie) ? "F" : factura.Serie,
                Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                PaymentFormCode = ObtenerFormaPagoSAT(factura.FormaPago),
                CurrencyCode = factura.Moneda ?? "MXN",
                TypeCode = factura.TipoComprobante ?? "I", // I=Ingreso
                ExpeditionZipCode = _config.CodigoPostal,
                ExportCode = factura.Exportacion ?? "01", // 01=No aplica
                PaymentMethodCode = factura.MetodoPago ?? "PUE",
                ExchangeRate = 1,

                // Emisor (nuestra empresa) con certificados
                Issuer = GenerarEmisor(),

                // Receptor (cliente)
                Recipient = GenerarReceptor(factura),

                // Conceptos (productos/servicios)
                Items = GenerarItems(factura)
            };

            return request;
        }

        /// <summary>
        /// Genera la información del emisor con certificados CSD
        /// </summary>
        private IssuerModel GenerarEmisor()
        {
            var issuer = new IssuerModel
            {
                Tin = _config.RfcEmisor,
                LegalName = _config.NombreEmisor,
                TaxRegimeCode = _config.RegimenFiscal,
                TaxCredentials = new List<TaxCredentialModel>()
            };

            // SIEMPRE enviar certificados (tanto en TEST como en PRODUCCION)
            // Según documentación oficial de FiscalAPI
            
            // Agregar certificado (.cer) si existe
            if (!string.IsNullOrEmpty(_config.CertificadoBase64) && _config.CertificadoBase64.Length > 100)
            {
                issuer.TaxCredentials.Add(new TaxCredentialModel
                {
                    Base64File = _config.CertificadoBase64,
                    FileType = 0, // 0 = Certificado
                    Password = _config.PasswordLlave
                });
            }

            // Agregar llave privada (.key) si existe
            if (!string.IsNullOrEmpty(_config.LlavePrivadaBase64) && _config.LlavePrivadaBase64.Length > 100)
            {
                issuer.TaxCredentials.Add(new TaxCredentialModel
                {
                    Base64File = _config.LlavePrivadaBase64,
                    FileType = 1, // 1 = Llave privada
                    Password = _config.PasswordLlave
                });
            }

            return issuer;
        }

        /// <summary>
        /// Genera la información del receptor (cliente)
        /// </summary>
        private RecipientModel GenerarReceptor(Factura factura)
        {
            return new RecipientModel
            {
                Tin = factura.ReceptorRFC,
                LegalName = factura.ReceptorNombre,
                ZipCode = factura.ReceptorDomicilioFiscalCP,
                TaxRegimeCode = factura.ReceptorRegimenFiscalReceptor ?? "616", // 616 = Sin obligaciones fiscales
                CfdiUseCode = factura.ReceptorUsoCFDI ?? "G03",
                Email = null // Opcional
            };
        }

        /// <summary>
        /// Genera los items (conceptos/líneas de detalle)
        /// </summary>
        private List<InvoiceItemModel> GenerarItems(Factura factura)
        {
            var items = new List<InvoiceItemModel>();

            foreach (var detalle in factura.Conceptos)
            {
                var item = new InvoiceItemModel
                {
                    ItemCode = detalle.ClaveProdServ ?? "01010101", // Código genérico SAT
                    Quantity = detalle.Cantidad,
                    UnitOfMeasurementCode = detalle.ClaveUnidad ?? "H87", // H87 = Pieza
                    Description = detalle.Descripcion,
                    UnitPrice = Math.Round(detalle.ValorUnitario, 6), // Redondear a 6 decimales máximo
                    TaxObjectCode = detalle.ObjetoImp ?? "02", // 02 = Sí objeto de impuesto
                    ItemSku = string.IsNullOrEmpty(detalle.NoIdentificacion) ? detalle.ClaveProdServ ?? "PROD001" : detalle.NoIdentificacion,
                    Discount = detalle.Descuento > 0 ? (decimal?)detalle.Descuento : null,
                    ItemTaxes = new List<InvoiceItemTaxModel>()
                };

                // Impuestos del concepto
                if (detalle.Impuestos != null && detalle.Impuestos.Any())
                {
                    foreach (var imp in detalle.Impuestos)
                    {
                        item.ItemTaxes.Add(new InvoiceItemTaxModel
                        {
                            TaxCode = imp.Impuesto, // "002" para IVA
                            TaxTypeCode = imp.TipoFactor, // "Tasa"
                            TaxRate = imp.TasaOCuota ?? 0m, // 0.16 para IVA 16%
                            TaxFlagCode = imp.TipoImpuesto.ToUpper() == "TRASLADO" ? "T" : "R" // T=Traslado, R=Retención
                        });
                    }
                }
                else
                {
                    // Si no hay impuestos definidos, agregar IVA 16% por defecto
                    item.ItemTaxes.Add(new InvoiceItemTaxModel
                    {
                        TaxCode = "002", // IVA
                        TaxTypeCode = "Tasa",
                        TaxRate = 0.16m, // 16%
                        TaxFlagCode = "T" // Traslado
                    });
                }

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Convierte forma de pago del sistema a clave SAT
        /// </summary>
        private string ObtenerFormaPagoSAT(string formaPago)
        {
            if (string.IsNullOrEmpty(formaPago))
                return "01"; // Efectivo por defecto

            // Mapeo de formas de pago comunes
            var mapeo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "EFECTIVO", "01" },
                { "CHEQUE", "02" },
                { "TRANSFERENCIA", "03" },
                { "TARJETA CREDITO", "04" },
                { "TARJETA DEBITO", "28" },
                { "MONEDERO ELECTRONICO", "05" },
                { "VALES", "08" },
                { "POR DEFINIR", "99" }
            };

            // Si ya es clave SAT (2 dígitos), retornarla
            if (formaPago.Length == 2 && int.TryParse(formaPago, out _))
                return formaPago;

            // Buscar en el mapeo
            if (mapeo.TryGetValue(formaPago, out string claveSAT))
                return claveSAT;

            // Si no se encuentra, retornar efectivo
            return "01";
        }

        /// <summary>
        /// Valida que los datos mínimos estén completos antes de timbrar
        /// </summary>
        public ValidacionCFDIResult ValidarDatosFactura(Factura factura)
        {
            var errores = new List<string>();

            // Validar receptor
            if (string.IsNullOrWhiteSpace(factura.ReceptorRFC))
                errores.Add("RFC del receptor es requerido");

            if (string.IsNullOrWhiteSpace(factura.ReceptorNombre))
                errores.Add("Nombre del receptor es requerido");

            if (string.IsNullOrWhiteSpace(factura.ReceptorDomicilioFiscalCP))
                errores.Add("Código postal del receptor es requerido");

            // Validar conceptos
            if (factura.Conceptos == null || !factura.Conceptos.Any())
                errores.Add("La factura debe tener al menos un concepto");

            foreach (var detalle in factura.Conceptos ?? Enumerable.Empty<FacturaDetalle>())
            {
                if (detalle.Cantidad <= 0)
                    errores.Add($"Cantidad inválida en concepto: {detalle.Descripcion}");

                if (detalle.ValorUnitario < 0)
                    errores.Add($"Valor unitario inválido en concepto: {detalle.Descripcion}");

                if (string.IsNullOrWhiteSpace(detalle.Descripcion))
                    errores.Add("Descripción del concepto es requerida");
            }

            // Validar configuración del emisor
            if (string.IsNullOrWhiteSpace(_config.RfcEmisor))
                errores.Add("RFC del emisor no configurado");

            if (string.IsNullOrWhiteSpace(_config.CodigoPostal))
                errores.Add("Código postal del emisor no configurado");

            return new ValidacionCFDIResult
            {
                Valido = errores.Count == 0,
                Errores = errores
            };
        }
    }

    /// <summary>
    /// Resultado de validación
    /// </summary>
    public class ValidacionCFDIResult
    {
        public bool Valido { get; set; }
        public List<string> Errores { get; set; } = new List<string>();

        public string MensajeErrores
        {
            get
            {
                return Errores.Any() 
                    ? string.Join("; ", Errores) 
                    : string.Empty;
            }
        }
    }
}
