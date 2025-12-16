using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Implementación del proveedor PAC Finkok
    /// Documentación: https://wiki.finkok.com/doku.php
    /// </summary>
    public class FinkokPAC : IProveedorPAC
    {
        /// <summary>
        /// Timbra un comprobante CFDI 4.0 con Finkok
        /// </summary>
        public async Task<RespuestaTimbrado> TimbrarAsync(string xmlSinTimbrar, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaTimbrado();

            try
            {
                // Construir SOAP envelope para Finkok
                string soapEnvelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                  xmlns:stam=""http://facturacion.finkok.com/stamp"">
    <soapenv:Header/>
    <soapenv:Body>
        <stam:stamp>
            <stam:xml>{EscapeXml(xmlSinTimbrar)}</stam:xml>
            <stam:username>{config.Usuario}</stam:username>
            <stam:password>{config.Password}</stam:password>
        </stam:stamp>
    </soapenv:Body>
</soapenv:Envelope>";

                // Crear solicitud HTTP
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(config.UrlTimbrado);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Headers.Add("SOAPAction", "stamp");
                request.Timeout = config.TimeoutSegundos * 1000;

                // Enviar SOAP request
                byte[] requestBytes = Encoding.UTF8.GetBytes(soapEnvelope);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                }

                // Obtener respuesta
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string soapResponse = await reader.ReadToEndAsync();
                    ProcesarRespuestaTimbrado(soapResponse, respuesta);
                }
            }
            catch (WebException wex)
            {
                respuesta.Exitoso = false;

                if (wex.Response != null)
                {
                    using (Stream errorStream = wex.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(errorStream))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        ProcesarErrorTimbrado(errorResponse, respuesta);
                    }
                }
                else
                {
                    respuesta.Mensaje = "Error de conexión con Finkok: " + wex.Message;
                    respuesta.CodigoError = "CONEXION_ERROR";
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = "Error al timbrar: " + ex.Message;
                respuesta.CodigoError = "EXCEPCION";
            }

            return respuesta;
        }

        /// <summary>
        /// Cancela un comprobante con Finkok usando firma digital
        /// </summary>
        public async Task<RespuestaCancelacion> CancelarAsync(string uuid, string rfcEmisor, 
            string motivoCancelacion, string folioSustitucion, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaCancelacion();

            try
            {
                // Validar certificados
                if (string.IsNullOrEmpty(config.RutaCertificado) || string.IsNullOrEmpty(config.RutaLlavePrivada))
                {
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "No se han configurado los certificados (.CER y .KEY)";
                    return respuesta;
                }

                // Cargar certificado con llave privada
                X509Certificate2 certificado;
                try
                {
                    certificado = Utilidades.CertificadoHelper.CargarCertificadoConLlave(
                        config.RutaCertificado, 
                        config.RutaLlavePrivada, 
                        config.PasswordLlavePrivada
                    );
                }
                catch (NotImplementedException)
                {
                    // Si falla la desencriptación automática, intentar con archivo PEM convertido
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Para usar certificados .KEY del SAT, ejecute primero: " +
                                      "openssl pkcs8 -inform DER -in archivo.key -out archivo.pem " +
                                      "Luego configure la ruta del archivo .PEM en lugar del .KEY";
                    return respuesta;
                }

                // Validar vigencia
                if (!Utilidades.CertificadoHelper.ValidarVigencia(certificado))
                {
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "El certificado no es válido para la fecha actual";
                    return respuesta;
                }

                // Obtener número de certificado
                string numeroCertificado = Utilidades.CertificadoHelper.ObtenerNumeroCertificado(certificado);

                // Construir XML de cancelación con firma
                string xmlCancelacion = ConstruirXMLCancelacion(uuid, rfcEmisor, motivoCancelacion, 
                    folioSustitucion, certificado, numeroCertificado);

                // Construir SOAP envelope
                string soapEnvelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                  xmlns:canc=""http://facturacion.finkok.com/cancellation"">
    <soapenv:Header/>
    <soapenv:Body>
        <canc:cancel>
            <canc:xml>{EscapeXml(xmlCancelacion)}</canc:xml>
            <canc:username>{config.Usuario}</canc:username>
            <canc:password>{config.Password}</canc:password>
            <canc:store_pending>1</canc:store_pending>
        </canc:cancel>
    </soapenv:Body>
</soapenv:Envelope>";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(config.UrlCancelacion);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Headers.Add("SOAPAction", "cancel");
                request.Timeout = config.TimeoutSegundos * 1000;

                byte[] requestBytes = Encoding.UTF8.GetBytes(soapEnvelope);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string soapResponse = await reader.ReadToEndAsync();
                    ProcesarRespuestaCancelacion(soapResponse, respuesta);
                }
            }
            catch (WebException wex)
            {
                respuesta.Exitoso = false;

                if (wex.Response != null)
                {
                    using (Stream errorStream = wex.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(errorStream))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        ProcesarErrorCancelacion(errorResponse, respuesta);
                    }
                }
                else
                {
                    respuesta.Mensaje = "Error de conexión con Finkok: " + wex.Message;
                    respuesta.CodigoError = "CONEXION_ERROR";
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = "Error al cancelar: " + ex.Message;
                respuesta.CodigoError = "EXCEPCION";
            }

            return respuesta;
        }

        /// <summary>
        /// Construye el XML de cancelación firmado digitalmente
        /// </summary>
        private string ConstruirXMLCancelacion(string uuid, string rfcEmisor, string motivoCancelacion,
            string folioSustitucion, X509Certificate2 certificado, string numeroCertificado)
        {
            // Fecha ISO 8601
            string fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            // Construir la cadena original para firma
            string cadenaOriginal = $"|{uuid}|{rfcEmisor}|{motivoCancelacion}|{folioSustitucion ?? ""}|";

            // Firmar la cadena
            string sello = Utilidades.CertificadoHelper.FirmarCadena(cadenaOriginal, certificado);

            // Construir XML con firma
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine($"<Cancelacion xmlns=\"http://cancelacfd.sat.gob.mx\" ");
            xml.AppendLine($"             xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            xml.AppendLine($"             xsi:schemaLocation=\"http://cancelacfd.sat.gob.mx http://www.sat.gob.mx/sitio_internet/cfd/cancelacfd/Cancelacion.xsd\" ");
            xml.AppendLine($"             Fecha=\"{fecha}\" ");
            xml.AppendLine($"             RfcEmisor=\"{rfcEmisor}\" ");
            xml.AppendLine($"             NoCertificado=\"{numeroCertificado}\" ");
            xml.AppendLine($"             Certificado=\"{Convert.ToBase64String(certificado.Export(X509ContentType.Cert))}\" ");
            xml.AppendLine($"             Sello=\"{sello}\">");
            xml.AppendLine("  <Folios>");

            if (!string.IsNullOrEmpty(folioSustitucion))
            {
                xml.AppendLine($"    <Folio UUID=\"{uuid}\" Motivo=\"{motivoCancelacion}\" FolioSustitucion=\"{folioSustitucion}\" />");
            }
            else
            {
                xml.AppendLine($"    <Folio UUID=\"{uuid}\" Motivo=\"{motivoCancelacion}\" />");
            }

            xml.AppendLine("  </Folios>");
            xml.AppendLine("</Cancelacion>");

            return xml.ToString();
        }

        /// <summary>
        /// Consulta estatus de un comprobante con Finkok
        /// </summary>
        public async Task<RespuestaConsulta> ConsultarEstatusAsync(string uuid, string rfcEmisor, 
            string rfcReceptor, decimal total, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaConsulta();

            try
            {
                string soapEnvelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                  xmlns:util=""http://facturacion.finkok.com/utilities"">
    <soapenv:Header/>
    <soapenv:Body>
        <util:get_sat_status>
            <util:uuid>{uuid}</util:uuid>
            <util:taxpayer_id>{rfcEmisor}</util:taxpayer_id>
            <util:receiver_id>{rfcReceptor}</util:receiver_id>
            <util:total>{total.ToString("F2")}</util:total>
            <util:username>{config.Usuario}</util:username>
            <util:password>{config.Password}</util:password>
        </util:get_sat_status>
    </soapenv:Body>
</soapenv:Envelope>";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(config.UrlConsulta);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Headers.Add("SOAPAction", "get_sat_status");
                request.Timeout = config.TimeoutSegundos * 1000;

                byte[] requestBytes = Encoding.UTF8.GetBytes(soapEnvelope);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string soapResponse = await reader.ReadToEndAsync();
                    ProcesarRespuestaConsulta(soapResponse, respuesta);
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = "Error al consultar: " + ex.Message;
                respuesta.CodigoError = "EXCEPCION";
            }

            return respuesta;
        }

        #region Métodos Auxiliares

        private void ProcesarRespuestaTimbrado(string soapResponse, RespuestaTimbrado respuesta)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(soapResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                nsmgr.AddNamespace("stam", "http://facturacion.finkok.com/stamp");
                nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                // Buscar nodo de respuesta exitosa
                XmlNode resultNode = xmlDoc.SelectSingleNode("//stam:stampResult", nsmgr);
                
                if (resultNode != null)
                {
                    XmlNode xmlNode = resultNode.SelectSingleNode("stam:xml", nsmgr);
                    if (xmlNode != null)
                    {
                        string xmlTimbrado = xmlNode.InnerText;
                        respuesta.XMLTimbrado = xmlTimbrado;

                        // Extraer UUID del complemento TimbreFiscalDigital
                        XmlDocument cfdiDoc = new XmlDocument();
                        cfdiDoc.LoadXml(xmlTimbrado);

                        XmlNamespaceManager cfdiNsmgr = new XmlNamespaceManager(cfdiDoc.NameTable);
                        cfdiNsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
                        cfdiNsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                        XmlNode tfdNode = cfdiDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", cfdiNsmgr);
                        if (tfdNode != null)
                        {
                            respuesta.UUID = tfdNode.Attributes["UUID"]?.Value;
                            respuesta.FechaTimbrado = DateTime.Parse(tfdNode.Attributes["FechaTimbrado"]?.Value);
                            respuesta.SelloSAT = tfdNode.Attributes["SelloSAT"]?.Value;
                            respuesta.NoCertificadoSAT = tfdNode.Attributes["NoCertificadoSAT"]?.Value;
                            respuesta.SelloCFD = tfdNode.Attributes["SelloCFD"]?.Value;

                            respuesta.Exitoso = true;
                            respuesta.Mensaje = "Timbrado exitoso";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = "Error al procesar respuesta: " + ex.Message;
            }
        }

        private void ProcesarErrorTimbrado(string errorResponse, RespuestaTimbrado respuesta)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(errorResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");

                XmlNode faultNode = xmlDoc.SelectSingleNode("//s:Fault", nsmgr);
                if (faultNode != null)
                {
                    string faultString = faultNode.SelectSingleNode("faultstring")?.InnerText;
                    respuesta.Mensaje = faultString ?? "Error desconocido de Finkok";
                    
                    // Mapear códigos de error comunes de Finkok
                    if (faultString.Contains("307"))
                        respuesta.CodigoError = "CFDI_DUPLICADO";
                    else if (faultString.Contains("302"))
                        respuesta.CodigoError = "CERTIFICADO_INVALIDO";
                    else
                        respuesta.CodigoError = "ERROR_FINKOK";
                }
            }
            catch
            {
                respuesta.Mensaje = "Error al procesar respuesta de error";
            }
        }

        private void ProcesarRespuestaCancelacion(string soapResponse, RespuestaCancelacion respuesta)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(soapResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                nsmgr.AddNamespace("canc", "http://facturacion.finkok.com/cancellation");

                // Buscar nodo de respuesta exitosa
                XmlNode resultNode = xmlDoc.SelectSingleNode("//canc:cancelResult", nsmgr);
                
                if (resultNode != null)
                {
                    XmlNode foliosNode = resultNode.SelectSingleNode("canc:Folios", nsmgr);
                    if (foliosNode != null)
                    {
                        XmlNode folioNode = foliosNode.SelectSingleNode("canc:Folio", nsmgr);
                        if (folioNode != null)
                        {
                            string estatusCancelacion = folioNode.Attributes["EstatusCancelacion"]?.Value;
                            string estatusUUID = folioNode.Attributes["EstatusUUID"]?.Value;

                            respuesta.EstatusSAT = estatusCancelacion;
                            respuesta.EstatusUUID = estatusUUID;
                            respuesta.FechaRespuesta = DateTime.Now;

                            // Verificar si fue aceptada
                            if (estatusCancelacion == "201" || estatusCancelacion == "202")
                            {
                                respuesta.Exitoso = true;
                                respuesta.Mensaje = estatusCancelacion == "201" 
                                    ? "Cancelación aceptada" 
                                    : "Cancelación en proceso (pendiente de aceptación del receptor)";
                            }
                            else
                            {
                                respuesta.Exitoso = false;
                                respuesta.Mensaje = $"Cancelación rechazada. Código: {estatusCancelacion}";
                                respuesta.CodigoError = estatusCancelacion;
                            }

                            // Obtener acuse si existe
                            XmlNode acuseNode = resultNode.SelectSingleNode("canc:Acuse", nsmgr);
                            if (acuseNode != null)
                            {
                                respuesta.AcuseCancelacion = acuseNode.InnerXml;
                            }
                        }
                    }
                }
                else
                {
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Respuesta de cancelación no válida";
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = "Error al procesar respuesta de cancelación: " + ex.Message;
            }
        }

        private void ProcesarErrorCancelacion(string errorResponse, RespuestaCancelacion respuesta)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(errorResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");

                XmlNode faultNode = xmlDoc.SelectSingleNode("//s:Fault", nsmgr);
                if (faultNode != null)
                {
                    string faultString = faultNode.SelectSingleNode("faultstring")?.InnerText;
                    respuesta.Mensaje = faultString ?? "Error desconocido de Finkok";
                    
                    // Mapear códigos de error comunes de cancelación
                    if (faultString.Contains("205"))
                        respuesta.CodigoError = "UUID_NO_ENCONTRADO";
                    else if (faultString.Contains("206"))
                        respuesta.CodigoError = "UUID_NO_CANCELABLE";
                    else if (faultString.Contains("207"))
                        respuesta.CodigoError = "FUERA_DE_TIEMPO";
                    else
                        respuesta.CodigoError = "ERROR_FINKOK";
                }
            }
            catch
            {
                respuesta.Mensaje = "Error al procesar respuesta de error en cancelación";
            }
        }

        private void ProcesarRespuestaConsulta(string soapResponse, RespuestaConsulta respuesta)
        {
            // TODO: Implementar parsing de respuesta de consulta
            respuesta.Exitoso = true;
            respuesta.Estatus = "VIGENTE";
        }

        private string EscapeXml(string xml)
        {
            return xml.Replace("&", "&amp;")
                      .Replace("<", "&lt;")
                      .Replace(">", "&gt;")
                      .Replace("\"", "&quot;")
                      .Replace("'", "&apos;");
        }

        /// <summary>
        /// Timbra un recibo de nómina (CFDI con Complemento de Nómina 1.2)
        /// Usa el mismo endpoint de timbrado estándar, pero con XML de nómina
        /// </summary>
        public async Task<RespuestaTimbrado> TimbrarNominaAsync(string xmlNominaSinTimbrar, ConfiguracionPAC config)
        {
            // La nómina se timbra con el mismo método que facturas estándar
            // La diferencia está en la estructura del XML (TipoComprobante="N" y Complemento de Nómina)
            return await TimbrarAsync(xmlNominaSinTimbrar, config);
        }

        #endregion
    }
}
