using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Generador de XML para Complemento de Pago 2.0 (CFDI 4.0)
    /// Según Anexo 20 del SAT
    /// </summary>
    public class ComplementoPago20XMLGenerator
    {
        private XmlDocument _xmlDoc;
        private XmlElement _root;
        private ComplementoPago _complemento;
        private ConfiguracionEmpresa _empresa;
        
        public string GenerarXML(ComplementoPago complemento, ConfiguracionEmpresa empresa, ConfiguracionPAC config)
        {
            _complemento = complemento;
            _empresa = empresa;
            
            _xmlDoc = new XmlDocument();
            
            // Crear raíz del CFDI 4.0
            _root = _xmlDoc.CreateElement("cfdi", "Comprobante", "http://www.sat.gob.mx/cfd/4");
            
            // Namespaces
            _root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _root.SetAttribute("xmlns:pago20", "http://www.sat.gob.mx/Pagos20");
            _root.SetAttribute("xsi:schemaLocation", 
                "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd " +
                "http://www.sat.gob.mx/Pagos20 http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xsd");
            
            // Atributos principales
            _root.SetAttribute("Version", "4.0");
            
            if (!string.IsNullOrEmpty(complemento.Serie))
                _root.SetAttribute("Serie", complemento.Serie);
            
            if (complemento.Folio.HasValue)
                _root.SetAttribute("Folio", complemento.Folio.Value.ToString());
            
            _root.SetAttribute("Fecha", complemento.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss"));
            _root.SetAttribute("Sello", ""); // Se llenará al timbrar
            _root.SetAttribute("NoCertificado", empresa.NoCertificado ?? "");
            _root.SetAttribute("Certificado", empresa.Certificado ?? "");
            
            // Específico de complemento de pago
            _root.SetAttribute("SubTotal", "0");
            _root.SetAttribute("Moneda", "XXX"); // Sin moneda
            _root.SetAttribute("Total", "0");
            _root.SetAttribute("TipoDeComprobante", "P"); // P = Pago
            _root.SetAttribute("Exportacion", "01"); // No aplica
            _root.SetAttribute("LugarExpedicion", empresa.CodigoPostal ?? "00000");
            
            _xmlDoc.AppendChild(_root);
            
            // Secciones
            EscribirEmisor();
            EscribirReceptor();
            EscribirConceptos();
            EscribirComplementoPago();
            
            return FormatearXML(_xmlDoc);
        }
        
        private void EscribirEmisor()
        {
            XmlElement emisor = _xmlDoc.CreateElement("cfdi", "Emisor", _root.NamespaceURI);
            emisor.SetAttribute("Rfc", _empresa.RFC);
            emisor.SetAttribute("Nombre", _empresa.RazonSocial);
            emisor.SetAttribute("RegimenFiscal", _empresa.RegimenFiscal ?? "601");
            _root.AppendChild(emisor);
        }
        
        private void EscribirReceptor()
        {
            XmlElement receptor = _xmlDoc.CreateElement("cfdi", "Receptor", _root.NamespaceURI);
            receptor.SetAttribute("Rfc", _complemento.ReceptorRFC);
            receptor.SetAttribute("Nombre", _complemento.ReceptorNombre);
            receptor.SetAttribute("DomicilioFiscalReceptor", _complemento.ReceptorDomicilioFiscal);
            receptor.SetAttribute("RegimenFiscalReceptor", _complemento.ReceptorRegimenFiscal);
            receptor.SetAttribute("UsoCFDI", _complemento.ReceptorUsoCFDI);
            _root.AppendChild(receptor);
        }
        
        private void EscribirConceptos()
        {
            XmlElement conceptos = _xmlDoc.CreateElement("cfdi", "Conceptos", _root.NamespaceURI);
            
            // Para complemento de pago, siempre va un concepto con valores en cero
            XmlElement concepto = _xmlDoc.CreateElement("cfdi", "Concepto", _root.NamespaceURI);
            concepto.SetAttribute("ClaveProdServ", "84111506"); // Servicios de facturación
            concepto.SetAttribute("Cantidad", "1");
            concepto.SetAttribute("ClaveUnidad", "ACT");
            concepto.SetAttribute("Descripcion", "Pago");
            concepto.SetAttribute("ValorUnitario", "0");
            concepto.SetAttribute("Importe", "0");
            concepto.SetAttribute("ObjetoImp", "01"); // No objeto de impuesto
            
            conceptos.AppendChild(concepto);
            _root.AppendChild(conceptos);
        }
        
        private void EscribirComplementoPago()
        {
            XmlElement complemento = _xmlDoc.CreateElement("cfdi", "Complemento", _root.NamespaceURI);
            
            XmlElement pagos = _xmlDoc.CreateElement("pago20", "Pagos", "http://www.sat.gob.mx/Pagos20");
            pagos.SetAttribute("Version", "2.0");
            
            // Totales
            XmlElement totales = _xmlDoc.CreateElement("pago20", "Totales", pagos.NamespaceURI);
            
            // Calcular totales de impuestos
            var totalRetencionesIVA = CalcularTotalImpuesto("RETENCION", "002");
            var totalRetencionesISR = CalcularTotalImpuesto("RETENCION", "001");
            var totalRetencionesIEPS = CalcularTotalImpuesto("RETENCION", "003");
            var totalTrasladosIVA = CalcularTotalImpuesto("TRASLADO", "002");
            var totalTrasladosISR = CalcularTotalImpuesto("TRASLADO", "001");
            var totalTrasladosIEPS = CalcularTotalImpuesto("TRASLADO", "003");
            
            // Solo agregar atributos si son mayores a cero
            if (totalRetencionesIVA > 0)
                totales.SetAttribute("TotalRetencionesIVA", FormatDecimal(totalRetencionesIVA));
            if (totalRetencionesISR > 0)
                totales.SetAttribute("TotalRetencionesISR", FormatDecimal(totalRetencionesISR));
            if (totalRetencionesIEPS > 0)
                totales.SetAttribute("TotalRetencionesIEPS", FormatDecimal(totalRetencionesIEPS));
            if (totalTrasladosIVA > 0)
            {
                var baseIVA = CalcularBaseImpuesto("TRASLADO", "002");
                totales.SetAttribute("TotalTrasladosBaseIVA16", FormatDecimal(baseIVA));
                totales.SetAttribute("TotalTrasladosImpuestoIVA16", FormatDecimal(totalTrasladosIVA));
            }
            if (totalTrasladosISR > 0)
                totales.SetAttribute("TotalTrasladosImpuestoISR", FormatDecimal(totalTrasladosISR));
            if (totalTrasladosIEPS > 0)
                totales.SetAttribute("TotalTrasladosImpuestoIEPS", FormatDecimal(totalTrasladosIEPS));
            
            totales.SetAttribute("MontoTotalPagos", FormatDecimal(_complemento.MontoTotalPagos));
            
            pagos.AppendChild(totales);
            
            // Pagos
            foreach (var pago in _complemento.Pagos)
            {
                EscribirPago(pagos, pago);
            }
            
            complemento.AppendChild(pagos);
            _root.AppendChild(complemento);
        }
        
        private void EscribirPago(XmlElement pagosNode, ComplementoPagoPago pago)
        {
            XmlElement pagoNode = _xmlDoc.CreateElement("pago20", "Pago", pagosNode.NamespaceURI);
            
            pagoNode.SetAttribute("FechaPago", pago.FechaPago.ToString("yyyy-MM-ddTHH:mm:ss"));
            pagoNode.SetAttribute("FormaDePagoP", pago.FormaDePagoP);
            pagoNode.SetAttribute("MonedaP", pago.MonedaP);
            
            if (pago.TipoCambioP.HasValue && pago.MonedaP != "MXN")
                pagoNode.SetAttribute("TipoCambioP", FormatDecimal(pago.TipoCambioP.Value));
            
            pagoNode.SetAttribute("Monto", FormatDecimal(pago.Monto));
            
            // Datos opcionales
            if (!string.IsNullOrEmpty(pago.NumOperacion))
                pagoNode.SetAttribute("NumOperacion", pago.NumOperacion);
            if (!string.IsNullOrEmpty(pago.RfcEmisorCtaOrd))
                pagoNode.SetAttribute("RfcEmisorCtaOrd", pago.RfcEmisorCtaOrd);
            if (!string.IsNullOrEmpty(pago.NomBancoOrdExt))
                pagoNode.SetAttribute("NomBancoOrdExt", pago.NomBancoOrdExt);
            if (!string.IsNullOrEmpty(pago.CtaOrdenante))
                pagoNode.SetAttribute("CtaOrdenante", pago.CtaOrdenante);
            if (!string.IsNullOrEmpty(pago.RfcEmisorCtaBen))
                pagoNode.SetAttribute("RfcEmisorCtaBen", pago.RfcEmisorCtaBen);
            if (!string.IsNullOrEmpty(pago.CtaBeneficiario))
                pagoNode.SetAttribute("CtaBeneficiario", pago.CtaBeneficiario);
            
            // Documentos relacionados
            XmlElement doctoRelacionados = _xmlDoc.CreateElement("pago20", "DoctoRelacionado", pagoNode.NamespaceURI);
            
            foreach (var doc in pago.DocumentosRelacionados)
            {
                EscribirDocumentoRelacionado(pagoNode, doc);
            }
            
            pagosNode.AppendChild(pagoNode);
        }
        
        private void EscribirDocumentoRelacionado(XmlElement pagoNode, ComplementoPagoDocumento doc)
        {
            XmlElement docNode = _xmlDoc.CreateElement("pago20", "DoctoRelacionado", pagoNode.NamespaceURI);
            
            docNode.SetAttribute("IdDocumento", doc.IdDocumento.ToString().ToUpper());
            
            if (!string.IsNullOrEmpty(doc.Serie))
                docNode.SetAttribute("Serie", doc.Serie);
            if (!string.IsNullOrEmpty(doc.Folio))
                docNode.SetAttribute("Folio", doc.Folio);
            
            docNode.SetAttribute("MonedaDR", doc.MonedaDR);
            
            if (doc.EquivalenciaDR.HasValue && doc.MonedaDR != "MXN")
                docNode.SetAttribute("EquivalenciaDR", FormatDecimal(doc.EquivalenciaDR.Value));
            
            docNode.SetAttribute("NumParcialidad", doc.NumParcialidad.ToString());
            docNode.SetAttribute("ImpSaldoAnt", FormatDecimal(doc.ImpSaldoAnt));
            docNode.SetAttribute("ImpPagado", FormatDecimal(doc.ImpPagado));
            docNode.SetAttribute("ImpSaldoInsoluto", FormatDecimal(doc.ImpSaldoInsoluto));
            docNode.SetAttribute("ObjetoImpDR", doc.ObjetoImpDR);
            
            // Impuestos del documento relacionado
            if (doc.ImpuestosDR != null && doc.ImpuestosDR.Any())
            {
                EscribirImpuestosDR(docNode, doc.ImpuestosDR);
            }
            
            pagoNode.AppendChild(docNode);
        }
        
        private void EscribirImpuestosDR(XmlElement docNode, System.Collections.Generic.List<ComplementoPagoImpuestoDR> impuestos)
        {
            XmlElement impuestosDRNode = _xmlDoc.CreateElement("pago20", "ImpuestosDR", docNode.NamespaceURI);
            
            // Agrupar por tipo
            var retenciones = impuestos.Where(i => i.TipoImpuesto == "RETENCION").ToList();
            var traslados = impuestos.Where(i => i.TipoImpuesto == "TRASLADO").ToList();
            
            // Retenciones
            if (retenciones.Any())
            {
                XmlElement retencionesNode = _xmlDoc.CreateElement("pago20", "RetencionesDR", impuestosDRNode.NamespaceURI);
                
                foreach (var ret in retenciones)
                {
                    XmlElement retNode = _xmlDoc.CreateElement("pago20", "RetencionDR", retencionesNode.NamespaceURI);
                    retNode.SetAttribute("BaseDR", FormatDecimal(ret.BaseDR));
                    retNode.SetAttribute("ImpuestoDR", ret.ImpuestoDR);
                    retNode.SetAttribute("TipoFactorDR", ret.TipoFactorDR);
                    retNode.SetAttribute("TasaOCuotaDR", FormatDecimal(ret.TasaOCuotaDR ?? 0));
                    retNode.SetAttribute("ImporteDR", FormatDecimal(ret.ImporteDR ?? 0));
                    retencionesNode.AppendChild(retNode);
                }
                
                impuestosDRNode.AppendChild(retencionesNode);
            }
            
            // Traslados
            if (traslados.Any())
            {
                XmlElement trasladosNode = _xmlDoc.CreateElement("pago20", "TrasladosDR", impuestosDRNode.NamespaceURI);
                
                foreach (var tras in traslados)
                {
                    XmlElement trasNode = _xmlDoc.CreateElement("pago20", "TrasladoDR", trasladosNode.NamespaceURI);
                    trasNode.SetAttribute("BaseDR", FormatDecimal(tras.BaseDR));
                    trasNode.SetAttribute("ImpuestoDR", tras.ImpuestoDR);
                    trasNode.SetAttribute("TipoFactorDR", tras.TipoFactorDR);
                    
                    if (tras.TasaOCuotaDR.HasValue)
                        trasNode.SetAttribute("TasaOCuotaDR", FormatDecimal(tras.TasaOCuotaDR.Value));
                    if (tras.ImporteDR.HasValue)
                        trasNode.SetAttribute("ImporteDR", FormatDecimal(tras.ImporteDR.Value));
                    
                    trasladosNode.AppendChild(trasNode);
                }
                
                impuestosDRNode.AppendChild(trasladosNode);
            }
            
            docNode.AppendChild(impuestosDRNode);
        }
        
        private decimal CalcularTotalImpuesto(string tipoImpuesto, string claveImpuesto)
        {
            decimal total = 0;
            
            foreach (var pago in _complemento.Pagos)
            {
                foreach (var doc in pago.DocumentosRelacionados)
                {
                    if (doc.ImpuestosDR != null)
                    {
                        total += doc.ImpuestosDR
                            .Where(i => i.TipoImpuesto == tipoImpuesto && i.ImpuestoDR == claveImpuesto)
                            .Sum(i => i.ImporteDR ?? 0);
                    }
                }
            }
            
            return total;
        }
        
        private decimal CalcularBaseImpuesto(string tipoImpuesto, string claveImpuesto)
        {
            decimal total = 0;
            
            foreach (var pago in _complemento.Pagos)
            {
                foreach (var doc in pago.DocumentosRelacionados)
                {
                    if (doc.ImpuestosDR != null)
                    {
                        total += doc.ImpuestosDR
                            .Where(i => i.TipoImpuesto == tipoImpuesto && i.ImpuestoDR == claveImpuesto)
                            .Sum(i => i.BaseDR);
                    }
                }
            }
            
            return total;
        }
        
        private string FormatDecimal(decimal value)
        {
            return value.ToString("0.000000", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
        }
        
        private string FormatearXML(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = Encoding.UTF8
            };
            
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            
            return sb.ToString();
        }
    }
}
