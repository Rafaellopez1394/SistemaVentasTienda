using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CapaModelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Implementación del proveedor PAC Facturama
    /// API REST: https://api.facturama.mx/docs
    /// Ventajas: Plan gratuito 50 facturas/mes, timbres sin caducidad, API REST moderna
    /// </summary>
    public class FacturamaPAC : IProveedorPAC
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Timbra un comprobante CFDI 4.0 usando Facturama
        /// </summary>
        public async Task<RespuestaTimbrado> TimbrarAsync(string xmlSinTimbrar, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaTimbrado { Exitoso = false };

            try
            {
                // URL base según ambiente
                string urlBase = config.EsProduccion 
                    ? "https://api.facturama.mx" 
                    : "https://apisandbox.facturama.mx";

                // Endpoint de timbrado - Facturama API v2 requiere JSON
                string endpoint = $"{urlBase}/2/cfdis";

                // Configurar autenticación Basic (Usuario:Password en Base64)
                var authValue = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{config.Usuario}:{config.Password}"));
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", authValue);

                // Facturama API v2: Enviar XML en Base64 dentro de JSON
                var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlSinTimbrar));
                var jsonPayload = new JObject
                {
                    ["Xml"] = xmlBase64
                };
                
                var content = new StringContent(jsonPayload.ToString(), Encoding.UTF8, "application/json");

                // DEBUG: Guardar XML en archivo para diagnóstico (TEMPORAL)
                try
                {
                    string logPath = System.IO.Path.Combine(
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        "Logs",
                        $"factura_xml_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                    );
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
                    System.IO.File.WriteAllText(logPath, xmlSinTimbrar);
                }
                catch { /* Ignorar errores de logging */ }

                // Enviar petición POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                // DEBUG: Guardar respuesta en archivo para diagnóstico (TEMPORAL)
                try
                {
                    string logPath = System.IO.Path.Combine(
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        "Logs",
                        $"factura_response_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                    );
                    System.IO.File.WriteAllText(logPath, 
                        $"Status: {response.StatusCode}\n" +
                        $"Headers: {response.Headers}\n\n" +
                        $"Body:\n{responseBody}");
                }
                catch { /* Ignorar errores de logging */ }

                if (response.IsSuccessStatusCode)
                {
                    // Parsear respuesta JSON
                    var jsonResponse = JObject.Parse(responseBody);

                    // Extraer datos importantes
                    respuesta.Exitoso = true;
                    respuesta.UUID = jsonResponse["Complement"]?["TaxStamp"]?["Uuid"]?.ToString();
                    respuesta.FechaTimbrado = DateTime.Parse(
                        jsonResponse["Complement"]?["TaxStamp"]?["Date"]?.ToString());
                    
                    // El XML timbrado viene en formato Base64
                    string xmlTimbradoBase64 = jsonResponse["Content"]?.ToString();
                    if (!string.IsNullOrEmpty(xmlTimbradoBase64))
                    {
                        byte[] xmlBytes = Convert.FromBase64String(xmlTimbradoBase64);
                        respuesta.XMLTimbrado = Encoding.UTF8.GetString(xmlBytes);
                    }

                    // Sello del SAT
                    respuesta.SelloSAT = jsonResponse["Complement"]?["TaxStamp"]?["SatSeal"]?.ToString();
                    
                    // Número de certificado SAT
                    respuesta.NoCertificadoSAT = jsonResponse["Complement"]?["TaxStamp"]?["SatCertNumber"]?.ToString();

                    // Cadena original
                    respuesta.CadenaOriginal = jsonResponse["Complement"]?["TaxStamp"]?["OriginalString"]?.ToString();

                    respuesta.Mensaje = "Comprobante timbrado exitosamente con Facturama";
                }
                else
                {
                    // Manejar error
                    respuesta.Exitoso = false;
                    respuesta.CodigoError = ((int)response.StatusCode).ToString();
                    
                    try
                    {
                        // Intentar parsear como JSON
                        if (!string.IsNullOrWhiteSpace(responseBody))
                        {
                            var errorJson = JObject.Parse(responseBody);
                            
                            // Facturama devuelve errores en diferentes formatos
                            string mensaje = errorJson["Message"]?.ToString();
                            string modelState = errorJson["ModelState"]?.ToString();
                            string details = errorJson["Details"]?.ToString();
                            
                            respuesta.Mensaje = $"Error {response.StatusCode} - ";
                            
                            if (!string.IsNullOrEmpty(mensaje))
                                respuesta.Mensaje += mensaje;
                            else if (!string.IsNullOrEmpty(modelState))
                                respuesta.Mensaje += modelState;
                            else if (!string.IsNullOrEmpty(details))
                                respuesta.Mensaje += details;
                            else
                                respuesta.Mensaje += responseBody;
                        }
                        else
                        {
                            respuesta.Mensaje = $"Error {response.StatusCode} - Respuesta vacía de Facturama";
                        }
                    }
                    catch
                    {
                        // Si no es JSON válido, devolver el body completo
                        respuesta.Mensaje = $"Error {response.StatusCode} - {responseBody}";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                respuesta.Mensaje = $"Error de conexión con Facturama: {ex.Message}";
                respuesta.CodigoError = "HTTP_ERROR";
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al procesar respuesta de Facturama: {ex.Message}";
                respuesta.CodigoError = "PARSE_ERROR";
            }

            return respuesta;
        }

        /// <summary>
        /// Cancela un comprobante timbrado en Facturama
        /// </summary>
        public async Task<RespuestaCancelacion> CancelarAsync(string uuid, string rfcEmisor, 
            string motivoCancelacion, string folioSustitucion, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaCancelacion { Exitoso = false };

            try
            {
                // URL base según ambiente
                string urlBase = config.EsProduccion 
                    ? "https://api.facturama.mx" 
                    : "https://apisandbox.facturama.mx";

                // Endpoint de cancelación
                string endpoint = $"{urlBase}/cfdi";

                // Configurar autenticación
                var authValue = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{config.Usuario}:{config.Password}"));
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", authValue);

                // Body de la petición de cancelación
                var cancelRequest = new
                {
                    Invoices = new[]
                    {
                        new
                        {
                            Uuid = uuid,
                            Motive = motivoCancelacion,
                            UuidReplacement = folioSustitucion
                        }
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(cancelRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Enviar petición DELETE
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(responseBody);
                    
                    respuesta.Exitoso = true;
                    respuesta.EstatusSAT = "ACEPTADA";
                    respuesta.EstatusUUID = jsonResponse["Status"]?.ToString();
                    respuesta.FechaRespuesta = DateTime.Now;
                    respuesta.AcuseCancelacion = responseBody;
                    respuesta.Mensaje = "Comprobante cancelado exitosamente";
                }
                else
                {
                    try
                    {
                        var errorJson = JObject.Parse(responseBody);
                        respuesta.CodigoError = errorJson["Code"]?.ToString();
                        respuesta.Mensaje = errorJson["Message"]?.ToString() ?? "Error al cancelar";
                    }
                    catch
                    {
                        respuesta.Mensaje = $"Error al cancelar: {responseBody}";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error en cancelación con Facturama: {ex.Message}";
                respuesta.CodigoError = "ERROR";
            }

            return respuesta;
        }

        /// <summary>
        /// Consulta el estatus de un comprobante en Facturama
        /// </summary>
        public async Task<RespuestaConsulta> ConsultarEstatusAsync(string uuid, string rfcEmisor, 
            string rfcReceptor, decimal total, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaConsulta { Exitoso = false };

            try
            {
                // URL base según ambiente
                string urlBase = config.EsProduccion 
                    ? "https://api.facturama.mx" 
                    : "https://apisandbox.facturama.mx";

                // Endpoint de consulta
                string endpoint = $"{urlBase}/cfdi?keyword={uuid}";

                // Configurar autenticación
                var authValue = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{config.Usuario}:{config.Password}"));
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", authValue);

                // Enviar petición GET
                var response = await _httpClient.GetAsync(endpoint);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JArray.Parse(responseBody);
                    
                    if (jsonResponse.Count > 0)
                    {
                        var cfdi = jsonResponse[0];
                        
                        respuesta.Exitoso = true;
                        respuesta.Estatus = cfdi["Status"]?.ToString(); // "active" o "canceled"
                        respuesta.EsCancelable = cfdi["Status"]?.ToString() == "active";
                        respuesta.EstatusCancelacion = cfdi["CancellationStatus"]?.ToString();
                        respuesta.Mensaje = "Consulta exitosa";
                    }
                    else
                    {
                        respuesta.Mensaje = "No se encontró el comprobante";
                        respuesta.CodigoError = "NOT_FOUND";
                    }
                }
                else
                {
                    respuesta.Mensaje = $"Error al consultar: {responseBody}";
                    respuesta.CodigoError = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error en consulta con Facturama: {ex.Message}";
                respuesta.CodigoError = "ERROR";
            }

            return respuesta;
        }

        public async Task<RespuestaTimbrado> TimbrarComplementoPagoAsync(string xmlSinTimbrar, ConfiguracionPAC config)
        {
            // Facturama usa el mismo endpoint para complementos de pago
            return await TimbrarAsync(xmlSinTimbrar, config);
        }
    }
}
