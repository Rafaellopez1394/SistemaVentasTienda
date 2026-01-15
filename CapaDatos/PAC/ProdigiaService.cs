using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using CapaModelo;
using Newtonsoft.Json;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Servicio HTTP para integración con Prodigia PAC (API REST)
    /// Documentación: https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest
    /// </summary>
    public class ProdigiaService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ConfiguracionProdigia _configuracion;

        public ProdigiaService(ConfiguracionProdigia configuracion)
        {
            _configuracion = configuracion ?? throw new ArgumentNullException(nameof(configuracion));

            // Forzar TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120) // Timeout de 120 segundos
            };

            // URL Base según ambiente
            _httpClient.BaseAddress = new Uri(_configuracion.UrlApi);

            // Autenticación Basic Auth: usuario:password en Base64
            string credenciales = $"{_configuracion.Usuario}:{_configuracion.Password}";
            string credencialesBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credenciales));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credencialesBase64);

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SistemaVentas-Prodigia/1.0");
        }

        /// <summary>
        /// Timbra un CFDI enviando el XML pre-firmado
        /// </summary>
        public RespuestaTimbrado CrearYTimbrarCFDI(string xmlCfdi)
        {
            var respuesta = new RespuestaTimbrado
            {
                Exitoso = false,
                CodigoError = "0",
                Mensaje = ""
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("=== REQUEST A PRODIGIA ===");
                System.Diagnostics.Debug.WriteLine($"Endpoint: {_httpClient.BaseAddress}servicio/rest/timbrado40/timbrarCfdi?contrato={_configuracion.Contrato}");
                System.Diagnostics.Debug.WriteLine($"Usuario: {_configuracion.Usuario.Substring(0, Math.Min(10, _configuracion.Usuario.Length))}...");
                System.Diagnostics.Debug.WriteLine($"Contrato: {_configuracion.Contrato}");

                // Convertir XML a Base64
                string xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlCfdi));

                // Preparar request
                var request = new ProdigiaTimbrarRequest
                {
                    XmlBase64 = xmlBase64,
                    Contrato = _configuracion.Contrato,
                    Prueba = _configuracion.Ambiente == "TEST"
                };

                // Construir query parameters con opciones
                string queryParams = $"contrato={_configuracion.Contrato}";
                var opciones = new System.Collections.Generic.List<string>();

                // Si hay certificados, agregarlos al request body
                if (!string.IsNullOrEmpty(_configuracion.CertificadoBase64) && 
                    _configuracion.CertificadoBase64.Length > 100)
                {
                    request.CertBase64 = _configuracion.CertificadoBase64;
                    request.KeyBase64 = _configuracion.LlavePrivadaBase64;
                    request.KeyPass = _configuracion.PasswordLlave;
                    System.Diagnostics.Debug.WriteLine("✅ Certificados incluidos en request body");
                    
                    // Agregar opciones para cálculo automático
                    opciones.Add("CALCULAR_SELLO");
                    opciones.Add("ESTABLECER_NO_CERTIFICADO");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Sin certificados en BD - usando CERT_DEFAULT de Prodigia");
                    // CERT_DEFAULT: usa certificado subido al portal PADE
                    opciones.Add("CERT_DEFAULT");
                    opciones.Add("CALCULAR_SELLO");
                    opciones.Add("ESTABLECER_NO_CERTIFICADO");
                }

                // Opciones comunes
                opciones.Add("GENERAR_PDF");
                opciones.Add("GENERAR_CBB"); // Código QR
                opciones.Add("REGRESAR_CADENA_ORIGINAL");

                // Agregar opciones como query parameters
                foreach (var opcion in opciones)
                {
                    queryParams += $"&{opcion}";
                }

                request.Opciones = opciones;

                // Serializar request a JSON
                string jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);
                System.Diagnostics.Debug.WriteLine($"JSON Request:\n{jsonRequest}");
                System.Diagnostics.Debug.WriteLine($"Query Params: {queryParams}");
                System.Diagnostics.Debug.WriteLine("=========================");

                // Crear contenido HTTP
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Endpoint con contrato y opciones como query parameters
                string endpoint = $"servicio/rest/timbrado40/timbrarCfdi?{queryParams}";

                // Enviar petición POST
                HttpResponseMessage response = _httpClient.PostAsync(endpoint, content).Result;

                // Leer respuesta (XML)
                string responseXml = response.Content.ReadAsStringAsync().Result;

                System.Diagnostics.Debug.WriteLine("=== RESPONSE DE PRODIGIA ===");
                System.Diagnostics.Debug.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response XML:\n{responseXml}");
                System.Diagnostics.Debug.WriteLine("============================");

                if (!response.IsSuccessStatusCode)
                {
                    respuesta.Mensaje = $"Error HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    respuesta.CodigoError = ((int)response.StatusCode).ToString();
                    return respuesta;
                }

                // Parsear respuesta XML
                ProdigiaTimbrarResponse prodigiaResponse = ParsearRespuestaXml(responseXml);

                if (prodigiaResponse.TimbradoOk && prodigiaResponse.Codigo == "0")
                {
                    // Decodificar XML timbrado
                    byte[] xmlBytes = Convert.FromBase64String(prodigiaResponse.XmlBase64);
                    string xmlTimbrado = Encoding.UTF8.GetString(xmlBytes);

                    respuesta.Exitoso = true;
                    respuesta.UUID = prodigiaResponse.Uuid;
                    respuesta.FechaTimbrado = DateTime.TryParse(prodigiaResponse.FechaTimbrado, out DateTime fecha) ? (DateTime?)fecha : null;
                    respuesta.XMLTimbrado = xmlTimbrado;
                    respuesta.SelloCFD = prodigiaResponse.SelloCFD;
                    respuesta.SelloSAT = prodigiaResponse.SelloSAT;
                    respuesta.NoCertificadoSAT = prodigiaResponse.NoCertificadoSAT;
                    respuesta.Mensaje = "Timbrado exitoso";
                    respuesta.CodigoError = "0";

                    // PDF si existe
                    if (!string.IsNullOrEmpty(prodigiaResponse.PdfBase64))
                    {
                        respuesta.PdfBase64 = prodigiaResponse.PdfBase64;
                    }
                }
                else
                {
                    respuesta.Exitoso = false;
                    respuesta.CodigoError = prodigiaResponse.Codigo;
                    respuesta.Mensaje = $"Error de Prodigia: {prodigiaResponse.Mensaje}";
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Excepción al timbrar: {ex.Message}";
                respuesta.CodigoError = "EXCEPTION";
                System.Diagnostics.Debug.WriteLine($"❌ Excepción: {ex.ToString()}");
                return respuesta;
            }
        }

        /// <summary>
        /// Parsea el XML de respuesta de Prodigia a objeto
        /// </summary>
        private ProdigiaTimbrarResponse ParsearRespuestaXml(string xml)
        {
            var response = new ProdigiaTimbrarResponse();

            try
            {
                XDocument doc = XDocument.Parse(xml);
                XElement root = doc.Root;

                response.Id = root.Element("id")?.Value;
                response.TimbradoOk = root.Element("timbradoOk")?.Value?.ToLower() == "true";
                response.Contrato = root.Element("contrato")?.Value;
                response.Codigo = root.Element("codigo")?.Value ?? "0";
                response.Mensaje = root.Element("mensaje")?.Value ?? "";
                response.Version = root.Element("version")?.Value;
                response.Uuid = root.Element("uuid")?.Value;
                response.FechaTimbrado = root.Element("FechaTimbrado")?.Value;
                response.SelloCFD = root.Element("selloCFD")?.Value;
                response.NoCertificadoSAT = root.Element("noCertificadoSAT")?.Value;
                response.SelloSAT = root.Element("selloSAT")?.Value;
                response.XmlBase64 = root.Element("xmlBase64")?.Value;
                response.PdfBase64 = root.Element("pdfBase64")?.Value;
                response.Saldo = root.Element("saldo")?.Value;
                response.CadenaOriginalTFD = root.Element("cadenaOriginalTFD")?.Value;
                response.CodigoBarrasBidimensional = root.Element("codigoBarrasBidimensional")?.Value;
            }
            catch (Exception ex)
            {
                response.TimbradoOk = false;
                response.Codigo = "PARSE_ERROR";
                response.Mensaje = $"Error al parsear respuesta XML: {ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// Cancela un CFDI previamente timbrado
        /// Documentación: https://docs.prodigia.com.mx/api-timbrado-xml.html#cancelacion_cfdi_rest
        /// </summary>
        public RespuestaCancelacion CancelarCFDI(string uuid, string rfcEmisor, string motivoCancelacion, string uuidSustitucion = "")
        {
            var respuesta = new RespuestaCancelacion
            {
                Exitoso = false,
                CodigoError = "0",
                Mensaje = ""
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("=== CANCELACIÓN PRODIGIA ===");
                System.Diagnostics.Debug.WriteLine($"UUID: {uuid}");
                System.Diagnostics.Debug.WriteLine($"RFC Emisor: {rfcEmisor}");
                System.Diagnostics.Debug.WriteLine($"Motivo: {motivoCancelacion}");

                // Construir arregloUUID: UUID|Motivo|FolioSustitucion
                string arregloUuid = $"{uuid}|{motivoCancelacion}";
                if (!string.IsNullOrEmpty(uuidSustitucion))
                {
                    arregloUuid += $"|{uuidSustitucion}";
                }

                // Query parameters
                string queryParams = $"contrato={_configuracion.Contrato}&rfcEmisor={rfcEmisor}&arregloUUID={System.Uri.EscapeDataString(arregloUuid)}";

                // Agregar opción CERT_DEFAULT si no se envían certificados
                if (string.IsNullOrEmpty(_configuracion.CertificadoBase64))
                {
                    queryParams += "&CERT_DEFAULT";
                    System.Diagnostics.Debug.WriteLine("✅ Usando CERT_DEFAULT para cancelación");
                }

                // Endpoint
                string endpoint = $"servicio/rest/cancelacion/cancelarCfdi?{queryParams}";

                // Crear request body vacío o con certificados
                object requestBody;
                if (!string.IsNullOrEmpty(_configuracion.CertificadoBase64))
                {
                    requestBody = new
                    {
                        certBase64 = _configuracion.CertificadoBase64,
                        keyBase64 = _configuracion.LlavePrivadaBase64,
                        keyPass = _configuracion.PasswordLlave
                    };
                }
                else
                {
                    requestBody = new { }; // Body vacío
                }

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Enviar petición POST
                HttpResponseMessage response = _httpClient.PostAsync(endpoint, content).Result;
                string responseXml = response.Content.ReadAsStringAsync().Result;

                System.Diagnostics.Debug.WriteLine($"Status: {(int)response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response:\n{responseXml}");

                if (!response.IsSuccessStatusCode)
                {
                    respuesta.Mensaje = $"Error HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    respuesta.CodigoError = ((int)response.StatusCode).ToString();
                    return respuesta;
                }

                // Parsear respuesta XML
                XDocument doc = XDocument.Parse(responseXml);
                XElement root = doc.Root;

                bool statusOk = root.Element("statusOk")?.Value?.ToLower() == "true";
                string codigo = root.Element("codigo")?.Value ?? "0";
                string mensaje = root.Element("mensaje")?.Value ?? "";

                if (statusOk && codigo == "0")
                {
                    // Buscar cancelación individual
                    var cancelacionNode = root.Descendants("cancelacion").FirstOrDefault();
                    if (cancelacionNode != null)
                    {
                        string codigoCancelacion = cancelacionNode.Element("codigo")?.Value ?? "0";
                        string mensajeCancelacion = cancelacionNode.Element("mensaje")?.Value ?? "";

                        // Código 201 = Solicitud recibida, 202 = Ya cancelado
                        if (codigoCancelacion == "201" || codigoCancelacion == "202")
                        {
                            respuesta.Exitoso = true;
                            respuesta.CodigoError = codigoCancelacion;
                            respuesta.Mensaje = mensajeCancelacion;
                        }
                        else
                        {
                            respuesta.Exitoso = false;
                            respuesta.CodigoError = codigoCancelacion;
                            respuesta.Mensaje = $"Error en cancelación: {mensajeCancelacion}";
                        }
                    }
                }
                else
                {
                    respuesta.Exitoso = false;
                    respuesta.CodigoError = codigo;
                    respuesta.Mensaje = $"Error de Prodigia: {mensaje}";
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Excepción al cancelar: {ex.Message}";
                respuesta.CodigoError = "EXCEPTION";
                System.Diagnostics.Debug.WriteLine($"❌ Excepción: {ex.ToString()}");
                return respuesta;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
