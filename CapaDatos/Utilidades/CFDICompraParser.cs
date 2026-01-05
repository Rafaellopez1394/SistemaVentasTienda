using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CapaDatos.Utilidades
{
    /// <summary>
    /// Parser para leer facturas CFDI de compras (XML)
    /// </summary>
    public class CFDICompraParser
    {
        // Namespaces del CFDI 4.0
        private static readonly XNamespace nsCfdi = "http://www.sat.gob.mx/cfd/4";
        private static readonly XNamespace nsTfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

        public class DatosFacturaCompra
        {
            // Datos del comprobante
            public string Serie { get; set; }
            public string Folio { get; set; }
            public DateTime Fecha { get; set; }
            public string FormaPago { get; set; }
            public string MetodoPago { get; set; }
            public string Moneda { get; set; }
            public decimal TipoCambio { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Descuento { get; set; }
            public decimal Total { get; set; }
            public string TipoDeComprobante { get; set; }
            public string LugarExpedicion { get; set; }

            // Datos del emisor (proveedor)
            public string EmisorRFC { get; set; }
            public string EmisorNombre { get; set; }
            public string EmisorRegimenFiscal { get; set; }

            // Datos del receptor (nuestra empresa)
            public string ReceptorRFC { get; set; }
            public string ReceptorNombre { get; set; }
            public string ReceptorUsoCFDI { get; set; }
            public string ReceptorRegimenFiscal { get; set; }
            public string ReceptorDomicilioFiscal { get; set; }

            // Timbre fiscal digital
            public string UUID { get; set; }
            public DateTime? FechaTimbrado { get; set; }
            public string SelloCFD { get; set; }
            public string SelloSAT { get; set; }
            public string NoCertificadoSAT { get; set; }

            // Conceptos de la factura
            public List<ConceptoFacturaCompra> Conceptos { get; set; }

            // Impuestos
            public decimal TotalImpuestosTrasladados { get; set; }
            public decimal TotalImpuestosRetenidos { get; set; }

            public DatosFacturaCompra()
            {
                Conceptos = new List<ConceptoFacturaCompra>();
                TipoCambio = 1;
                Moneda = "MXN";
            }
        }

        public class ConceptoFacturaCompra
        {
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

            // Impuestos del concepto
            public List<ImpuestoConcepto> ImpuestosTrasladados { get; set; }
            public List<ImpuestoConcepto> ImpuestosRetenidos { get; set; }

            // Desglose (calculado)
            public decimal FactorConversion { get; set; }  // Ej: 1 caja = 8 piezas
            public decimal CantidadDesglosada { get; set; } // Ej: 2 cajas × 8 = 16 piezas
            public decimal PrecioUnitarioDesglosado { get; set; } // Ej: $80/caja ÷ 8 = $10/pieza

            public ConceptoFacturaCompra()
            {
                ImpuestosTrasladados = new List<ImpuestoConcepto>();
                ImpuestosRetenidos = new List<ImpuestoConcepto>();
                FactorConversion = 1;
                CantidadDesglosada = Cantidad;
            }
        }

        public class ImpuestoConcepto
        {
            public string Base { get; set; }
            public string Impuesto { get; set; }
            public string TipoFactor { get; set; }
            public decimal TasaOCuota { get; set; }
            public decimal Importe { get; set; }
        }

        /// <summary>
        /// Parsea un archivo XML de CFDI y extrae los datos de la factura de compra
        /// </summary>
        public static DatosFacturaCompra ParsearXML(string rutaArchivo)
        {
            try
            {
                XDocument doc = XDocument.Load(rutaArchivo);
                return ParsearXMLDesdeDocumento(doc);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al leer el archivo XML: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Parsea un XML de CFDI desde un string
        /// </summary>
        public static DatosFacturaCompra ParsearXMLDesdeTexto(string xmlTexto)
        {
            try
            {
                XDocument doc = XDocument.Parse(xmlTexto);
                return ParsearXMLDesdeDocumento(doc);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al parsear el XML: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Parsea un XDocument de CFDI
        /// </summary>
        private static DatosFacturaCompra ParsearXMLDesdeDocumento(XDocument doc)
        {
            var datos = new DatosFacturaCompra();

            // Obtener el nodo raíz Comprobante
            XElement comprobante = doc.Root;
            if (comprobante == null)
                throw new Exception("No se encontró el nodo Comprobante en el XML");

            // Datos del comprobante
            datos.Serie = (string)comprobante.Attribute("Serie");
            datos.Folio = (string)comprobante.Attribute("Folio");
            datos.Fecha = DateTime.Parse((string)comprobante.Attribute("Fecha"));
            datos.FormaPago = (string)comprobante.Attribute("FormaPago");
            datos.MetodoPago = (string)comprobante.Attribute("MetodoPago");
            datos.Moneda = (string)comprobante.Attribute("Moneda") ?? "MXN";
            
            string tipoCambioStr = (string)comprobante.Attribute("TipoCambio");
            datos.TipoCambio = string.IsNullOrEmpty(tipoCambioStr) ? 1 : decimal.Parse(tipoCambioStr);

            datos.SubTotal = decimal.Parse((string)comprobante.Attribute("SubTotal"));
            
            string descuentoStr = (string)comprobante.Attribute("Descuento");
            datos.Descuento = string.IsNullOrEmpty(descuentoStr) ? 0 : decimal.Parse(descuentoStr);

            datos.Total = decimal.Parse((string)comprobante.Attribute("Total"));
            datos.TipoDeComprobante = (string)comprobante.Attribute("TipoDeComprobante");
            datos.LugarExpedicion = (string)comprobante.Attribute("LugarExpedicion");

            // Emisor (Proveedor)
            XElement emisor = comprobante.Element(nsCfdi + "Emisor");
            if (emisor != null)
            {
                datos.EmisorRFC = (string)emisor.Attribute("Rfc");
                datos.EmisorNombre = (string)emisor.Attribute("Nombre");
                datos.EmisorRegimenFiscal = (string)emisor.Attribute("RegimenFiscal");
            }

            // Receptor (Nuestra empresa)
            XElement receptor = comprobante.Element(nsCfdi + "Receptor");
            if (receptor != null)
            {
                datos.ReceptorRFC = (string)receptor.Attribute("Rfc");
                datos.ReceptorNombre = (string)receptor.Attribute("Nombre");
                datos.ReceptorUsoCFDI = (string)receptor.Attribute("UsoCFDI");
                datos.ReceptorRegimenFiscal = (string)receptor.Attribute("RegimenFiscalReceptor");
                datos.ReceptorDomicilioFiscal = (string)receptor.Attribute("DomicilioFiscalReceptor");
            }

            // Conceptos
            XElement conceptos = comprobante.Element(nsCfdi + "Conceptos");
            if (conceptos != null)
            {
                foreach (XElement concepto in conceptos.Elements(nsCfdi + "Concepto"))
                {
                    var conceptoObj = new ConceptoFacturaCompra
                    {
                        ClaveProdServ = (string)concepto.Attribute("ClaveProdServ"),
                        NoIdentificacion = (string)concepto.Attribute("NoIdentificacion"),
                        Cantidad = decimal.Parse((string)concepto.Attribute("Cantidad")),
                        ClaveUnidad = (string)concepto.Attribute("ClaveUnidad"),
                        Unidad = (string)concepto.Attribute("Unidad"),
                        Descripcion = (string)concepto.Attribute("Descripcion"),
                        ValorUnitario = decimal.Parse((string)concepto.Attribute("ValorUnitario")),
                        Importe = decimal.Parse((string)concepto.Attribute("Importe")),
                        ObjetoImp = (string)concepto.Attribute("ObjetoImp")
                    };

                    string descConcepto = (string)concepto.Attribute("Descuento");
                    conceptoObj.Descuento = string.IsNullOrEmpty(descConcepto) ? 0 : decimal.Parse(descConcepto);

                    conceptoObj.CantidadDesglosada = conceptoObj.Cantidad;
                    conceptoObj.PrecioUnitarioDesglosado = conceptoObj.ValorUnitario;

                    // Impuestos del concepto
                    XElement impuestos = concepto.Element(nsCfdi + "Impuestos");
                    if (impuestos != null)
                    {
                        // Traslados (IVA generalmente)
                        XElement traslados = impuestos.Element(nsCfdi + "Traslados");
                        if (traslados != null)
                        {
                            foreach (XElement traslado in traslados.Elements(nsCfdi + "Traslado"))
                            {
                                conceptoObj.ImpuestosTrasladados.Add(new ImpuestoConcepto
                                {
                                    Base = (string)traslado.Attribute("Base"),
                                    Impuesto = (string)traslado.Attribute("Impuesto"),
                                    TipoFactor = (string)traslado.Attribute("TipoFactor"),
                                    TasaOCuota = decimal.Parse((string)traslado.Attribute("TasaOCuota")),
                                    Importe = decimal.Parse((string)traslado.Attribute("Importe"))
                                });
                            }
                        }

                        // Retenciones (ISR, IVA retenido)
                        XElement retenciones = impuestos.Element(nsCfdi + "Retenciones");
                        if (retenciones != null)
                        {
                            foreach (XElement retencion in retenciones.Elements(nsCfdi + "Retencion"))
                            {
                                conceptoObj.ImpuestosRetenidos.Add(new ImpuestoConcepto
                                {
                                    Base = (string)retencion.Attribute("Base"),
                                    Impuesto = (string)retencion.Attribute("Impuesto"),
                                    TipoFactor = (string)retencion.Attribute("TipoFactor"),
                                    TasaOCuota = decimal.Parse((string)retencion.Attribute("TasaOCuota")),
                                    Importe = decimal.Parse((string)retencion.Attribute("Importe"))
                                });
                            }
                        }
                    }

                    datos.Conceptos.Add(conceptoObj);
                }
            }

            // Impuestos totales
            XElement impuestosTotales = comprobante.Element(nsCfdi + "Impuestos");
            if (impuestosTotales != null)
            {
                string totalTrasladados = (string)impuestosTotales.Attribute("TotalImpuestosTrasladados");
                datos.TotalImpuestosTrasladados = string.IsNullOrEmpty(totalTrasladados) ? 0 : decimal.Parse(totalTrasladados);

                string totalRetenidos = (string)impuestosTotales.Attribute("TotalImpuestosRetenidos");
                datos.TotalImpuestosRetenidos = string.IsNullOrEmpty(totalRetenidos) ? 0 : decimal.Parse(totalRetenidos);
            }

            // Timbre Fiscal Digital
            XElement complemento = comprobante.Element(nsCfdi + "Complemento");
            if (complemento != null)
            {
                XElement timbre = complemento.Element(nsTfd + "TimbreFiscalDigital");
                if (timbre != null)
                {
                    datos.UUID = (string)timbre.Attribute("UUID");
                    string fechaTimbrado = (string)timbre.Attribute("FechaTimbrado");
                    if (!string.IsNullOrEmpty(fechaTimbrado))
                        datos.FechaTimbrado = DateTime.Parse(fechaTimbrado);
                    
                    datos.SelloCFD = (string)timbre.Attribute("SelloCFD");
                    datos.SelloSAT = (string)timbre.Attribute("SelloSAT");
                    datos.NoCertificadoSAT = (string)timbre.Attribute("NoCertificadoSAT");
                }
            }

            return datos;
        }

        /// <summary>
        /// Valida que el XML sea un CFDI válido
        /// </summary>
        public static bool ValidarEstructura(string rutaArchivo, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                XDocument doc = XDocument.Load(rutaArchivo);
                XElement comprobante = doc.Root;

                if (comprobante == null)
                {
                    mensaje = "No se encontró el nodo raíz Comprobante";
                    return false;
                }

                // Validar que tenga los elementos mínimos
                if (comprobante.Attribute("Version") == null)
                {
                    mensaje = "No se encontró el atributo Version";
                    return false;
                }

                string version = (string)comprobante.Attribute("Version");
                if (version != "4.0" && version != "3.3")
                {
                    mensaje = "Versión de CFDI no soportada: " + version;
                    return false;
                }

                if (comprobante.Element(nsCfdi + "Emisor") == null)
                {
                    mensaje = "No se encontró el nodo Emisor";
                    return false;
                }

                if (comprobante.Element(nsCfdi + "Receptor") == null)
                {
                    mensaje = "No se encontró el nodo Receptor";
                    return false;
                }

                if (comprobante.Element(nsCfdi + "Conceptos") == null)
                {
                    mensaje = "No se encontró el nodo Conceptos";
                    return false;
                }

                mensaje = "XML válido";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al validar XML: " + ex.Message;
                return false;
            }
        }
    }
}
