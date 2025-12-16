using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Genera XML de comprobante CFDI 4.0 según Anexo 20 del SAT
    /// </summary>
    public class CFDI40XMLGenerator
    {
        /// <summary>
        /// Genera el XML del comprobante sin timbrar
        /// </summary>
        public string GenerarXML(Factura factura)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();

                // Comprobante (nodo raíz)
                writer.WriteStartElement("cfdi", "Comprobante", "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance",
                    "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd");

                // Atributos del comprobante
                writer.WriteAttributeString("Version", factura.Version);
                if (!string.IsNullOrEmpty(factura.Serie))
                    writer.WriteAttributeString("Serie", factura.Serie);
                writer.WriteAttributeString("Folio", factura.Folio);
                writer.WriteAttributeString("Fecha", factura.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss"));
                writer.WriteAttributeString("FormaPago", factura.FormaPago);
                writer.WriteAttributeString("SubTotal", FormatDecimal(factura.Subtotal));
                
                if (factura.Descuento > 0)
                    writer.WriteAttributeString("Descuento", FormatDecimal(factura.Descuento));
                
                writer.WriteAttributeString("Moneda", "MXN");
                writer.WriteAttributeString("Total", FormatDecimal(factura.Total));
                writer.WriteAttributeString("TipoDeComprobante", factura.TipoComprobante);
                writer.WriteAttributeString("Exportacion", "01"); // No aplica
                writer.WriteAttributeString("MetodoPago", factura.MetodoPago);
                writer.WriteAttributeString("LugarExpedicion", factura.ReceptorDomicilioFiscalCP);

                // Emisor
                EscribirEmisor(writer, factura);

                // Receptor
                EscribirReceptor(writer, factura);

                // Conceptos
                EscribirConceptos(writer, factura);

                // Impuestos (resumen global)
                EscribirImpuestos(writer, factura);

                writer.WriteEndElement(); // Comprobante
                writer.WriteEndDocument();
            }

            return sb.ToString();
        }

        private void EscribirEmisor(XmlWriter writer, Factura factura)
        {
            writer.WriteStartElement("cfdi", "Emisor", null);
            writer.WriteAttributeString("Rfc", factura.EmisorRFC);
            writer.WriteAttributeString("Nombre", factura.EmisorNombre);
            writer.WriteAttributeString("RegimenFiscal", factura.EmisorRegimenFiscal);
            writer.WriteEndElement();
        }

        private void EscribirReceptor(XmlWriter writer, Factura factura)
        {
            writer.WriteStartElement("cfdi", "Receptor", null);
            writer.WriteAttributeString("Rfc", factura.ReceptorRFC);
            writer.WriteAttributeString("Nombre", factura.ReceptorNombre);
            writer.WriteAttributeString("DomicilioFiscalReceptor", factura.ReceptorDomicilioFiscalCP);
            writer.WriteAttributeString("RegimenFiscalReceptor", factura.ReceptorRegimenFiscalReceptor);
            writer.WriteAttributeString("UsoCFDI", factura.ReceptorUsoCFDI);
            writer.WriteEndElement();
        }

        private void EscribirConceptos(XmlWriter writer, Factura factura)
        {
            writer.WriteStartElement("cfdi", "Conceptos", null);

            foreach (var concepto in factura.Conceptos.OrderBy(c => c.Secuencia))
            {
                writer.WriteStartElement("cfdi", "Concepto", null);
                writer.WriteAttributeString("ClaveProdServ", concepto.ClaveProdServ);
                
                if (!string.IsNullOrEmpty(concepto.NoIdentificacion))
                    writer.WriteAttributeString("NoIdentificacion", concepto.NoIdentificacion);
                
                writer.WriteAttributeString("Cantidad", FormatDecimal(concepto.Cantidad));
                writer.WriteAttributeString("ClaveUnidad", concepto.ClaveUnidad);
                
                if (!string.IsNullOrEmpty(concepto.Unidad))
                    writer.WriteAttributeString("Unidad", concepto.Unidad);
                
                writer.WriteAttributeString("Descripcion", concepto.Descripcion);
                writer.WriteAttributeString("ValorUnitario", FormatDecimal(concepto.ValorUnitario));
                writer.WriteAttributeString("Importe", FormatDecimal(concepto.Importe));
                
                if (concepto.Descuento > 0)
                    writer.WriteAttributeString("Descuento", FormatDecimal(concepto.Descuento));
                
                writer.WriteAttributeString("ObjetoImp", concepto.ObjetoImp);

                // Impuestos del concepto
                if (concepto.Impuestos.Any())
                {
                    EscribirImpuestosConcepto(writer, concepto);
                }

                writer.WriteEndElement(); // Concepto
            }

            writer.WriteEndElement(); // Conceptos
        }

        private void EscribirImpuestosConcepto(XmlWriter writer, FacturaDetalle concepto)
        {
            writer.WriteStartElement("cfdi", "Impuestos", null);

            var traslados = concepto.Impuestos.Where(i => i.TipoImpuesto == "TRASLADO").ToList();
            var retenciones = concepto.Impuestos.Where(i => i.TipoImpuesto == "RETENCION").ToList();

            // Traslados
            if (traslados.Any())
            {
                writer.WriteStartElement("cfdi", "Traslados", null);
                
                foreach (var traslado in traslados)
                {
                    writer.WriteStartElement("cfdi", "Traslado", null);
                    writer.WriteAttributeString("Base", FormatDecimal(traslado.Base));
                    writer.WriteAttributeString("Impuesto", traslado.Impuesto);
                    writer.WriteAttributeString("TipoFactor", traslado.TipoFactor);
                    
                    if (traslado.TasaOCuota.HasValue)
                        writer.WriteAttributeString("TasaOCuota", FormatDecimal(traslado.TasaOCuota.Value, 6));
                    
                    if (traslado.Importe.HasValue)
                        writer.WriteAttributeString("Importe", FormatDecimal(traslado.Importe.Value));
                    
                    writer.WriteEndElement(); // Traslado
                }
                
                writer.WriteEndElement(); // Traslados
            }

            // Retenciones
            if (retenciones.Any())
            {
                writer.WriteStartElement("cfdi", "Retenciones", null);
                
                foreach (var retencion in retenciones)
                {
                    writer.WriteStartElement("cfdi", "Retencion", null);
                    writer.WriteAttributeString("Base", FormatDecimal(retencion.Base));
                    writer.WriteAttributeString("Impuesto", retencion.Impuesto);
                    writer.WriteAttributeString("TipoFactor", retencion.TipoFactor);
                    
                    if (retencion.TasaOCuota.HasValue)
                        writer.WriteAttributeString("TasaOCuota", FormatDecimal(retencion.TasaOCuota.Value, 6));
                    
                    if (retencion.Importe.HasValue)
                        writer.WriteAttributeString("Importe", FormatDecimal(retencion.Importe.Value));
                    
                    writer.WriteEndElement(); // Retencion
                }
                
                writer.WriteEndElement(); // Retenciones
            }

            writer.WriteEndElement(); // Impuestos
        }

        private void EscribirImpuestos(XmlWriter writer, Factura factura)
        {
            // Agrupar impuestos de todos los conceptos
            var todosImpuestos = factura.Conceptos.SelectMany(c => c.Impuestos).ToList();
            
            if (!todosImpuestos.Any())
                return;

            writer.WriteStartElement("cfdi", "Impuestos", null);

            // Total de retenciones
            var totalRetenciones = todosImpuestos
                .Where(i => i.TipoImpuesto == "RETENCION" && i.Importe.HasValue)
                .Sum(i => i.Importe.Value);
            
            if (totalRetenciones > 0)
                writer.WriteAttributeString("TotalImpuestosRetenidos", FormatDecimal(totalRetenciones));

            // Total de traslados
            var totalTraslados = todosImpuestos
                .Where(i => i.TipoImpuesto == "TRASLADO" && i.Importe.HasValue)
                .Sum(i => i.Importe.Value);
            
            if (totalTraslados > 0)
                writer.WriteAttributeString("TotalImpuestosTrasladados", FormatDecimal(totalTraslados));

            // Retenciones agrupadas
            var retencionesAgrupadas = todosImpuestos
                .Where(i => i.TipoImpuesto == "RETENCION")
                .GroupBy(i => new { i.Impuesto })
                .Select(g => new
                {
                    g.Key.Impuesto,
                    Importe = g.Sum(i => i.Importe ?? 0)
                })
                .Where(r => r.Importe > 0)
                .ToList();

            if (retencionesAgrupadas.Any())
            {
                writer.WriteStartElement("cfdi", "Retenciones", null);
                
                foreach (var ret in retencionesAgrupadas)
                {
                    writer.WriteStartElement("cfdi", "Retencion", null);
                    writer.WriteAttributeString("Impuesto", ret.Impuesto);
                    writer.WriteAttributeString("Importe", FormatDecimal(ret.Importe));
                    writer.WriteEndElement();
                }
                
                writer.WriteEndElement(); // Retenciones
            }

            // Traslados agrupados
            var trasladosAgrupados = todosImpuestos
                .Where(i => i.TipoImpuesto == "TRASLADO")
                .GroupBy(i => new { i.Impuesto, i.TipoFactor, i.TasaOCuota })
                .Select(g => new
                {
                    g.Key.Impuesto,
                    g.Key.TipoFactor,
                    g.Key.TasaOCuota,
                    Base = g.Sum(i => i.Base),
                    Importe = g.Sum(i => i.Importe ?? 0)
                })
                .Where(t => t.Importe > 0)
                .ToList();

            if (trasladosAgrupados.Any())
            {
                writer.WriteStartElement("cfdi", "Traslados", null);
                
                foreach (var tras in trasladosAgrupados)
                {
                    writer.WriteStartElement("cfdi", "Traslado", null);
                    writer.WriteAttributeString("Base", FormatDecimal(tras.Base));
                    writer.WriteAttributeString("Impuesto", tras.Impuesto);
                    writer.WriteAttributeString("TipoFactor", tras.TipoFactor);
                    
                    if (tras.TasaOCuota.HasValue)
                        writer.WriteAttributeString("TasaOCuota", FormatDecimal(tras.TasaOCuota.Value, 6));
                    
                    writer.WriteAttributeString("Importe", FormatDecimal(tras.Importe));
                    writer.WriteEndElement();
                }
                
                writer.WriteEndElement(); // Traslados
            }

            writer.WriteEndElement(); // Impuestos
        }

        /// <summary>
        /// Formatea decimal según especificaciones del SAT (máximo 2 decimales para montos)
        /// </summary>
        private string FormatDecimal(decimal value, int decimales = 2)
        {
            return value.ToString($"F{decimales}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Genera la cadena original para sellado (pre-timbrado)
        /// </summary>
        public string GenerarCadenaOriginal(string xml)
        {
            // TODO: Implementar transformación XSLT con cadenaoriginal_4_0.xslt del SAT
            // Por ahora retorna una cadena básica
            return xml.Replace("\r\n", "").Replace(" ", "");
        }

        /// <summary>
        /// Valida el XML contra el esquema XSD del SAT
        /// </summary>
        public bool ValidarXML(string xml, out string mensajeError)
        {
            mensajeError = string.Empty;
            
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                
                // TODO: Validar contra cfdv40.xsd
                // Requiere descargar esquemas del SAT
                
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al validar XML: " + ex.Message;
                return false;
            }
        }
    }
}
