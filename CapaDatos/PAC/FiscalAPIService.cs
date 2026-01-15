using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CapaModelo;
using Newtonsoft.Json;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Cliente HTTP directo para FiscalAPI (sin SDK)
    /// Compatible con ASP.NET Framework 4.6
    /// </summary>
    public class FiscalAPIService : IDisposable
    {
        private readonly ConfiguracionFiscalAPI _configuracion;
        private readonly HttpClient _httpClient;

        public FiscalAPIService(ConfiguracionFiscalAPI configuracion)
        {
            _configuracion = configuracion ?? throw new ArgumentNullException(nameof(configuracion));

            // CRÍTICO: Forzar TLS 1.2 en .NET Framework 4.6
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Crear HttpClient tradicional (sin HttpClientFactory)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuracion.UrlApi),
                Timeout = TimeSpan.FromSeconds(120) // 2 minutos para timbrado
            };

            // Configurar headers de autenticación (FiscalAPI usa headers custom)
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _configuracion.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-TENANT-KEY", _configuracion.Tenant);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Crear y timbrar CFDI 4.0
        /// Endpoint: POST /api/v4/invoices (unificado para todos los tipos)
        /// </summary>
        public async Task<RespuestaTimbrado> CrearYTimbrarCFDI(FiscalAPICrearCFDIRequest request)
        {
            var respuesta = new RespuestaTimbrado
            {
                Exitoso = false,
                FechaTimbrado = DateTime.Now
            };

            try
            {
                // Serializar request a JSON
                string jsonRequest = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                System.Diagnostics.Debug.WriteLine("=== REQUEST A FISCALAPI ===");
                System.Diagnostics.Debug.WriteLine($"Endpoint: {_configuracion.UrlApi}/api/v4/invoices");
                System.Diagnostics.Debug.WriteLine($"API Key: {_configuracion.ApiKey?.Substring(0, 20)}...");
                System.Diagnostics.Debug.WriteLine($"Tenant: {_configuracion.Tenant}");
                System.Diagnostics.Debug.WriteLine($"JSON Request:\n{jsonRequest}");
                System.Diagnostics.Debug.WriteLine("=========================");

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Endpoint oficial de FiscalAPI v4 unificado
                string endpoint = "/api/v4/invoices";

                // Realizar petición POST
                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

                // Leer contenido de respuesta
                string responseBody = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("=== RESPONSE DE FISCALAPI ===");
                System.Diagnostics.Debug.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body:\n{responseBody}");
                System.Diagnostics.Debug.WriteLine("============================");

                // Manejar códigos de estado HTTP
                if (response.IsSuccessStatusCode)
                {
                    // Respuesta exitosa (200-299)
                    var fiscalResponse = JsonConvert.DeserializeObject<FiscalAPICrearCFDIResponse>(responseBody);

                    if (fiscalResponse.Succeeded && fiscalResponse.Data?.Responses != null && fiscalResponse.Data.Responses.Count > 0)
                    {
                        var stampResponse = fiscalResponse.Data.Responses[0];
                        
                        // Decodificar XML de Base64
                        byte[] xmlBytes = Convert.FromBase64String(stampResponse.InvoiceBase64);
                        string xmlTimbrado = Encoding.UTF8.GetString(xmlBytes);

                        respuesta.Exitoso = true;
                        respuesta.UUID = stampResponse.InvoiceUuid;
                        respuesta.FechaTimbrado = stampResponse.InvoiceSignatureDate;
                        respuesta.XMLTimbrado = xmlTimbrado;
                        respuesta.SelloCFD = stampResponse.InvoiceBase64Sello;
                        respuesta.SelloSAT = stampResponse.SatBase64Sello;
                        respuesta.NoCertificadoSAT = stampResponse.SatCertificateNumber;
                        respuesta.CadenaOriginal = stampResponse.SatBase64OriginalString;
                        respuesta.Mensaje = "CFDI timbrado exitosamente";
                    }
                    else
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = fiscalResponse.Message ?? "Error desconocido en respuesta de FiscalAPI";
                        respuesta.CodigoError = "RESPONSE_ERROR";
                    }

                    return respuesta;
                }
                else
                {
                    // Manejar errores HTTP específicos
                    respuesta.CodigoError = $"HTTP_{(int)response.StatusCode}";

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized: // 401
                            respuesta.Mensaje = "Error de autenticación: API Key inválida o expirada";
                            respuesta.CodigoError = "401_UNAUTHORIZED";
                            break;

                        case HttpStatusCode.Forbidden: // 403
                            respuesta.Mensaje = "Acceso denegado: Verifica permisos de tu API Key";
                            respuesta.CodigoError = "403_FORBIDDEN";
                            break;

                        case (HttpStatusCode)422: // 422 Unprocessable Entity
                            // Parsear errores de validación
                            var errorResponse = JsonConvert.DeserializeObject<FiscalAPIErrorResponse>(responseBody);
                            respuesta.Mensaje = $"Error de validación: {errorResponse.Message ?? "Error desconocido"}";
                            respuesta.CodigoError = "422_VALIDATION_ERROR";
                            respuesta.Mensaje += $"\nDetalles: {errorResponse.Details}";
                            break;

                        case HttpStatusCode.BadRequest: // 400
                            respuesta.Mensaje = $"Petición inválida: {responseBody}";
                            respuesta.CodigoError = "400_BAD_REQUEST";
                            break;

                        case HttpStatusCode.InternalServerError: // 500
                            respuesta.Mensaje = $"Error interno del servidor de FiscalAPI: {responseBody}";
                            respuesta.CodigoError = "500_INTERNAL_ERROR";
                            respuesta.ErrorTecnico = responseBody;
                            break;

                        default:
                            respuesta.Mensaje = $"Error HTTP {(int)response.StatusCode}: {responseBody}";
                            break;
                    }

                    return respuesta;
                }
            }
            catch (TaskCanceledException ex)
            {
                respuesta.Mensaje = "Timeout: FiscalAPI no respondió en 2 minutos";
                respuesta.CodigoError = "TIMEOUT";
                return respuesta;
            }
            catch (HttpRequestException ex)
            {
                respuesta.Mensaje = $"Error de conexión con FiscalAPI: {ex.Message}";
                respuesta.CodigoError = "CONNECTION_ERROR";
                return respuesta;
            }
            catch (JsonException ex)
            {
                respuesta.Mensaje = $"Error al procesar respuesta JSON: {ex.Message}";
                respuesta.CodigoError = "JSON_PARSE_ERROR";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error inesperado: {ex.Message}";
                respuesta.CodigoError = "UNKNOWN_ERROR";
                return respuesta;
            }
        }

        /// <summary>
        /// Cancelar CFDI timbrado
        /// Endpoint real: POST /api/v4/cfdi40/cancel
        /// </summary>
        public async Task<RespuestaCancelacionCFDI> CancelarCFDI(string uuid, string motivo, string folioSustitucion = null)
        {
            var respuesta = new RespuestaCancelacionCFDI
            {
                Exitoso = false,
                UUID = uuid
            };

            try
            {
                var request = new FiscalAPICancelarRequest
                {
                    Uuid = uuid,
                    Motivo = motivo,
                    FolioSustitucion = folioSustitucion
                };

                string jsonRequest = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                string endpoint = "/api/v4/cfdi40/cancel";

                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var fiscalResponse = JsonConvert.DeserializeObject<FiscalAPICancelarResponse>(responseBody);

                    respuesta.Exitoso = true;
                    respuesta.EstatusCancelacion = fiscalResponse.Data.EstatusCancelacion;
                    respuesta.FechaCancelacion = fiscalResponse.Data.FechaCancelacion;
                    respuesta.AcuseXML = null; // No disponible en v4
                    respuesta.Mensaje = $"Cancelación procesada: {fiscalResponse.Data.EstatusCancelacion}";

                    return respuesta;
                }
                else
                {
                    respuesta.CodigoError = $"HTTP_{(int)response.StatusCode}";

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            respuesta.Mensaje = "Error de autenticación al cancelar";
                            break;

                        case (HttpStatusCode)422:
                            var errorResponse = JsonConvert.DeserializeObject<FiscalAPIErrorResponse>(responseBody);
                            respuesta.Mensaje = $"Error de validación: {errorResponse.Message ?? "Error desconocido"}";
                            respuesta.CodigoError = "422_VALIDATION_ERROR";
                            break;

                        default:
                            respuesta.Mensaje = $"Error al cancelar: {responseBody}";
                            break;
                    }

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al cancelar CFDI: {ex.Message}";
                respuesta.CodigoError = "CANCEL_ERROR";
                return respuesta;
            }
        }

        /// <summary>
        /// Consultar estado de un CFDI
        /// Endpoint real: GET /api/v4/cfdi40/status/{uuid}
        /// </summary>
        public async Task<ConsultaEstadoCFDIResponse> ConsultarEstadoCFDI(string uuid)
        {
            try
            {
                string endpoint = $"/api/v4/cfdi40/status/{uuid}";
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ConsultaEstadoCFDIResponse>(responseBody);
                    return result;
                }
                else
                {
                    return new ConsultaEstadoCFDIResponse
                    {
                        Exitoso = false,
                        Mensaje = $"Error al consultar estado: {responseBody}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConsultaEstadoCFDIResponse
                {
                    Exitoso = false,
                    Mensaje = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// Respuesta de consulta de estado
    /// </summary>
    public class ConsultaEstadoCFDIResponse
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; } // Vigente, Cancelado
        public bool EsCancelable { get; set; }
        public string EstatusCancelacion { get; set; }
    }

    /// <summary>
    /// Concepto para FiscalAPI
    /// </summary>
    public class ConceptoFiscalAPI
    {
        public string ClaveProdServ { get; set; }
        public decimal Cantidad { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public string NoIdentificacion { get; set; }
        public List<ImpuestoConceptoFiscalAPI> Impuestos { get; set; }
    }

    /// <summary>
    /// Impuesto por concepto para FiscalAPI
    /// </summary>
    public class ImpuestoConceptoFiscalAPI
    {
        public string TipoImpuesto { get; set; } // TRASLADO o RETENCION
        public string Impuesto { get; set; } // 002 = IVA
        public decimal TasaOCuota { get; set; }
        public decimal Base { get; set; }
        public decimal Importe { get; set; }
    }

    /// <summary>
    /// Impuesto global para FiscalAPI
    /// </summary>
    public class ImpuestoFiscalAPI
    {
        public string TipoImpuesto { get; set; }
        public string Impuesto { get; set; }
        public decimal TasaOCuota { get; set; }
        public decimal Base { get; set; }
        public decimal Importe { get; set; }
    }
}

