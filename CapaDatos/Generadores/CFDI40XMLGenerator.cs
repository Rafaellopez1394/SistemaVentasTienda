using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CapaModelo;

namespace CapaDatos.Generadores
{
    /// <summary>
    /// Generador de XML CFDI 4.0 según especificación del SAT
    /// Genera el XML completo listo para timbrado
    /// </summary>
    public class CFDI40XMLGenerator
    {
        private readonly ConfiguracionProdigia _config;

        public CFDI40XMLGenerator(ConfiguracionProdigia config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Genera el XML CFDI 4.0 completo sin timbrar
        /// </summary>
        public string GenerarXML(Factura factura)
        {
            // Namespaces requeridos por CFDI 4.0
            XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            // Crear raíz Comprobante con prefijo cfdi:
            XElement comprobante = new XElement(cfdi + "Comprobante",
                new XAttribute(XNamespace.Xmlns + "cfdi", cfdi),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(xsi + "schemaLocation", "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"),
                new XAttribute("Version", "4.0"),
                new XAttribute("Serie", factura.Serie ?? "A"),
                new XAttribute("Folio", factura.Folio),
                new XAttribute("Fecha", factura.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XAttribute("FormaPago", factura.FormaPago ?? "01"), // 01=Efectivo
                new XAttribute("NoCertificado", ""), // Se llenará por Prodigia con opción ESTABLECER_NO_CERTIFICADO
                new XAttribute("Certificado", ""), // Se llenará por Prodigia
                new XAttribute("SubTotal", factura.Subtotal.ToString("F2")),
                new XAttribute("Moneda", "MXN"),
                new XAttribute("Total", factura.Total.ToString("F2")),
                new XAttribute("TipoDeComprobante", factura.TipoComprobante ?? "I"),
                new XAttribute("Exportacion", "01"), // 01=No aplica
                new XAttribute("MetodoPago", factura.MetodoPago ?? "PUE"), // PUE=Pago en una sola exhibición
                new XAttribute("LugarExpedicion", _config.CodigoPostal),
                new XAttribute("Sello", "") // Se llenará por Prodigia con opción CALCULAR_SELLO
            );

            // Emisor
            XElement emisor = new XElement(cfdi + "Emisor",
                new XAttribute("Rfc", _config.RfcEmisor),
                new XAttribute("Nombre", _config.NombreEmisor),
                new XAttribute("RegimenFiscal", _config.RegimenFiscal)
            );
            comprobante.Add(emisor);

            // Receptor
            XElement receptor = new XElement(cfdi + "Receptor",
                new XAttribute("Rfc", factura.ReceptorRFC),
                new XAttribute("Nombre", factura.ReceptorNombre),
                new XAttribute("DomicilioFiscalReceptor", factura.ReceptorDomicilioFiscalCP),
                new XAttribute("RegimenFiscalReceptor", factura.ReceptorRegimenFiscalReceptor ?? "616"), // 616=Sin obligaciones fiscales
                new XAttribute("UsoCFDI", factura.ReceptorUsoCFDI ?? "G03") // G03=Gastos en general
            );
            comprobante.Add(receptor);

            // Conceptos
            XElement conceptos = new XElement(cfdi + "Conceptos");

            foreach (var detalle in factura.Conceptos)
            {
                XElement concepto = new XElement(cfdi + "Concepto",
                    new XAttribute("ClaveProdServ", detalle.ClaveProdServ ?? "01010101"),
                    new XAttribute("Cantidad", detalle.Cantidad.ToString("F3")),
                    new XAttribute("ClaveUnidad", detalle.ClaveUnidad ?? "H87"),
                    new XAttribute("Unidad", detalle.Unidad ?? "Pieza"),
                    new XAttribute("Descripcion", detalle.Descripcion),
                    new XAttribute("ValorUnitario", detalle.ValorUnitario.ToString("F6")),
                    new XAttribute("Importe", detalle.Importe.ToString("F2")),
                    new XAttribute("ObjetoImp", detalle.ObjetoImp ?? "02") // 02=Sí objeto de impuesto
                );
                
                // Solo agregar NoIdentificacion si tiene valor (es opcional)
                if (!string.IsNullOrWhiteSpace(detalle.NoIdentificacion))
                {
                    concepto.Add(new XAttribute("NoIdentificacion", detalle.NoIdentificacion));
                }

                // Agregar descuento si aplica
                if (detalle.Descuento > 0)
                {
                    concepto.Add(new XAttribute("Descuento", detalle.Descuento.ToString("F2")));
                }

                // Impuestos del concepto
                if (detalle.Impuestos != null && detalle.Impuestos.Any())
                {
                    XElement impuestos = new XElement(cfdi + "Impuestos");

                    // Traslados (IVA)
                    var traslados = detalle.Impuestos.Where(i => i.TipoImpuesto?.ToUpper() == "TRASLADO").ToList();
                    if (traslados.Any())
                    {
                        XElement trasladosElement = new XElement(cfdi + "Traslados");
                        foreach (var imp in traslados)
                        {
                            trasladosElement.Add(new XElement(cfdi + "Traslado",
                                new XAttribute("Base", detalle.Importe.ToString("F2")),
                                new XAttribute("Impuesto", imp.Impuesto ?? "002"), // 002=IVA
                                new XAttribute("TipoFactor", imp.TipoFactor ?? "Tasa"),
                                new XAttribute("TasaOCuota", ((decimal)(imp.TasaOCuota ?? 0.16m)).ToString("F6")),
                                new XAttribute("Importe", imp.Importe.Value.ToString("F2"))
                            ));
                        }
                        impuestos.Add(trasladosElement);
                    }

                    // Retenciones
                    var retenciones = detalle.Impuestos.Where(i => i.TipoImpuesto?.ToUpper() == "RETENCION").ToList();
                    if (retenciones.Any())
                    {
                        XElement retencionesElement = new XElement(cfdi + "Retenciones");
                        foreach (var imp in retenciones)
                        {
                            retencionesElement.Add(new XElement(cfdi + "Retencion",
                                new XAttribute("Base", detalle.Importe.ToString("F2")),
                                new XAttribute("Impuesto", imp.Impuesto ?? "002"),
                                new XAttribute("TipoFactor", imp.TipoFactor ?? "Tasa"),
                                new XAttribute("TasaOCuota", ((decimal)(imp.TasaOCuota ?? 0m)).ToString("F6")),
                                new XAttribute("Importe", imp.Importe.Value.ToString("F2"))
                            ));
                        }
                        impuestos.Add(retencionesElement);
                    }

                    concepto.Add(impuestos);
                }

                conceptos.Add(concepto);
            }
            comprobante.Add(conceptos);

            // Impuestos generales (resumen)
            if (factura.TotalImpuestosTrasladados > 0 || factura.TotalImpuestosRetenidos > 0)
            {
                XElement impuestos = new XElement(cfdi + "Impuestos");

                if (factura.TotalImpuestosTrasladados > 0)
                {
                    impuestos.Add(new XAttribute("TotalImpuestosTrasladados", factura.TotalImpuestosTrasladados.ToString("F2")));

                    XElement traslados = new XElement(cfdi + "Traslados");
                    traslados.Add(new XElement(cfdi + "Traslado",
                        new XAttribute("Base", factura.Subtotal.ToString("F2")),
                        new XAttribute("Impuesto", "002"), // IVA
                        new XAttribute("TipoFactor", "Tasa"),
                        new XAttribute("TasaOCuota", "0.160000"),
                        new XAttribute("Importe", factura.TotalImpuestosTrasladados.ToString("F2"))
                    ));
                    impuestos.Add(traslados);
                }

                if (factura.TotalImpuestosRetenidos > 0)
                {
                    impuestos.Add(new XAttribute("TotalImpuestosRetenidos", factura.TotalImpuestosRetenidos.ToString("F2")));
                }

                comprobante.Add(impuestos);
            }

            // Generar XML final
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                comprobante
            );

            // Convertir a string con encoding UTF-8 correcto
            using (var stream = new System.IO.MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false), // UTF-8 sin BOM
                    Indent = false, // Sin espacios ni saltos
                    OmitXmlDeclaration = false
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    doc.Save(writer);
                }

                // Convertir bytes a string UTF-8
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
