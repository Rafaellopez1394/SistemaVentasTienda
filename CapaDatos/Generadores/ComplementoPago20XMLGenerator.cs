using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CapaModelo;

namespace CapaDatos.Generadores
{
    /// <summary>
    /// Generador de XML para Complemento de Pago 2.0
    /// </summary>
    public class ComplementoPago20XMLGenerator
    {
        private const string NamespaceCFDI = "http://www.sat.gob.mx/cfd/4";
        private const string NamespaceXSI = "http://www.w3.org/2001/XMLSchema-instance";
        private const string NamespacePago20 = "http://www.sat.gob.mx/Pagos20";
        private const string SchemaLocation = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd http://www.sat.gob.mx/Pagos20 http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xsd";

        /// <summary>
        /// Genera el XML del Complemento de Pago 2.0
        /// </summary>
        public string GenerarXML(ComplementoPago complemento, ConfiguracionEmpresa empresa)
        {
            XNamespace cfdi = NamespaceCFDI;
            XNamespace xsi = NamespaceXSI;
            XNamespace pago20 = NamespacePago20;

            // Obtener el primer pago (puede haber varios)
            var primerPago = complemento.Pagos?.FirstOrDefault();
            if (primerPago == null || !primerPago.DocumentosRelacionados.Any())
            {
                throw new Exception("El complemento debe tener al menos un pago con documentos relacionados");
            }

            // Calcular total
            decimal montoTotal = primerPago.Monto;

            // Crear el documento XML
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(cfdi + "Comprobante",
                    new XAttribute(XNamespace.Xmlns + "cfdi", cfdi),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "pago20", pago20),
                    new XAttribute(xsi + "schemaLocation", SchemaLocation),
                    new XAttribute("Version", "4.0"),
                    new XAttribute("Serie", complemento.Serie ?? "P"),
                    new XAttribute("Folio", complemento.Folio?.ToString() ?? ""),
                    new XAttribute("Fecha", FormatearFecha(complemento.FechaEmision)),
                    new XAttribute("NoCertificado", ""), // Se llena en el timbrado
                    new XAttribute("Certificado", ""), // Se llena en el timbrado
                    new XAttribute("SubTotal", "0"),
                    new XAttribute("Moneda", "XXX"), // XXX para complementos de pago
                    new XAttribute("Total", "0"),
                    new XAttribute("TipoDeComprobante", "P"), // P = Pago
                    new XAttribute("Exportacion", "01"), // 01 = No aplica
                    new XAttribute("LugarExpedicion", empresa.CodigoPostal ?? ""),
                    new XAttribute("Sello", ""), // Se llena en el timbrado

                    // Emisor
                    new XElement(cfdi + "Emisor",
                        new XAttribute("Rfc", empresa.RFC ?? ""),
                        new XAttribute("Nombre", empresa.RazonSocial ?? ""),
                        new XAttribute("RegimenFiscal", empresa.RegimenFiscal ?? "")
                    ),

                    // Receptor
                    new XElement(cfdi + "Receptor",
                        new XAttribute("Rfc", complemento.ReceptorRFC ?? ""),
                        new XAttribute("Nombre", complemento.ReceptorNombre ?? ""),
                        new XAttribute("DomicilioFiscalReceptor", complemento.ReceptorDomicilioFiscal ?? ""),
                        new XAttribute("RegimenFiscalReceptor", complemento.ReceptorRegimenFiscal ?? "616"),
                        new XAttribute("UsoCFDI", "CP01") // CP01 = Pagos
                    ),

                    // Conceptos (un concepto genérico para pagos)
                    new XElement(cfdi + "Conceptos",
                        new XElement(cfdi + "Concepto",
                            new XAttribute("ClaveProdServ", "84111506"), // Servicios de facturación
                            new XAttribute("Cantidad", "1"),
                            new XAttribute("ClaveUnidad", "ACT"), // ACT = Actividad
                            new XAttribute("Descripcion", "Pago"),
                            new XAttribute("ValorUnitario", "0"),
                            new XAttribute("Importe", "0"),
                            new XAttribute("ObjetoImp", "01") // 01 = No objeto de impuesto
                        )
                    ),

                    // Complemento
                    new XElement(cfdi + "Complemento",
                        GenerarComplementoPago(pago20, complemento)
                    )
                )
            );

            // Generar el XML como string
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (var stringWriter = new System.IO.StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                doc.Save(xmlWriter);
                return stringWriter.ToString();
            }
        }

        private XElement GenerarComplementoPago(XNamespace pago20, ComplementoPago complemento)
        {
            var primerPago = complemento.Pagos.First();
            
            var pagos = new XElement(pago20 + "Pagos",
                new XAttribute("Version", "2.0")
            );

            // Totales
            var totales = new XElement(pago20 + "Totales",
                new XAttribute("MontoTotalPagos", FormatearDecimal(primerPago.Monto, 2))
            );
            pagos.Add(totales);

            // Pago
            var pago = new XElement(pago20 + "Pago",
                new XAttribute("FechaPago", FormatearFecha(primerPago.FechaPago)),
                new XAttribute("FormaDePagoP", primerPago.FormaDePagoP ?? "03"), // 03 = Transferencia
                new XAttribute("MonedaP", primerPago.MonedaP ?? "MXN"),
                primerPago.TipoCambioP.HasValue && primerPago.TipoCambioP.Value != 1 
                    ? new XAttribute("TipoCambioP", FormatearDecimal(primerPago.TipoCambioP.Value, 6)) 
                    : null,
                new XAttribute("Monto", FormatearDecimal(primerPago.Monto, 2))
            );

            // Documentos relacionados (facturas que se están pagando)
            foreach (var docto in primerPago.DocumentosRelacionados)
            {
                var doctoRelacionado = new XElement(pago20 + "DoctoRelacionado",
                    new XAttribute("IdDocumento", docto.IdDocumento.ToString()),
                    new XAttribute("Serie", docto.Serie ?? ""),
                    new XAttribute("Folio", docto.Folio ?? ""),
                    new XAttribute("MonedaDR", docto.MonedaDR ?? "MXN"),
                    docto.EquivalenciaDR.HasValue && docto.EquivalenciaDR.Value != 1 
                        ? new XAttribute("EquivalenciaDR", FormatearDecimal(docto.EquivalenciaDR.Value, 6)) 
                        : null,
                    new XAttribute("NumParcialidad", docto.NumParcialidad.ToString()),
                    new XAttribute("ImpSaldoAnt", FormatearDecimal(docto.ImpSaldoAnt, 2)),
                    new XAttribute("ImpPagado", FormatearDecimal(docto.ImpPagado, 2)),
                    new XAttribute("ImpSaldoInsoluto", FormatearDecimal(docto.ImpSaldoInsoluto, 2)),
                    new XAttribute("ObjetoImpDR", docto.ObjetoImpDR ?? "01") // 01 = No objeto de impuesto
                );

                pago.Add(doctoRelacionado);
            }

            pagos.Add(pago);

            return pagos;
        }

        private string FormatearFecha(DateTime fecha)
        {
            // Formato ISO 8601: 2024-01-09T10:30:00
            return fecha.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        private string FormatearDecimal(decimal valor, int decimales = 2)
        {
            return valor.ToString($"F{decimales}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Valida el XML generado
        /// </summary>
        public bool ValidarXML(string xml, out string errorValidacion)
        {
            errorValidacion = null;

            try
            {
                // Validar que sea XML válido
                var doc = XDocument.Parse(xml);

                // Validaciones básicas
                var comprobante = doc.Root;
                if (comprobante == null || comprobante.Name.LocalName != "Comprobante")
                {
                    errorValidacion = "El elemento raíz debe ser 'Comprobante'";
                    return false;
                }

                // Validar que sea de tipo Pago
                var tipoComprobante = comprobante.Attribute("TipoDeComprobante")?.Value;
                if (tipoComprobante != "P")
                {
                    errorValidacion = "El tipo de comprobante debe ser 'P' para pagos";
                    return false;
                }

                // Validar que tenga complemento
                var complemento = comprobante.Elements().FirstOrDefault(e => e.Name.LocalName == "Complemento");
                if (complemento == null)
                {
                    errorValidacion = "Falta el elemento 'Complemento'";
                    return false;
                }

                // Validar que tenga el complemento de pagos
                var pagos = complemento.Elements().FirstOrDefault(e => e.Name.LocalName == "Pagos");
                if (pagos == null)
                {
                    errorValidacion = "Falta el elemento 'Pagos' dentro del complemento";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorValidacion = "Error al validar XML: " + ex.Message;
                return false;
            }
        }
    }
}
