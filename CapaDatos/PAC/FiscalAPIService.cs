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
            _httpClient.DefaultRequestHeaders.Add("X-TIME-ZONE", "America/Mexico_City");
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
                        respuesta.InvoiceId = stampResponse.InvoiceId; // Guardar ID para descargar PDF de FiscalAPI
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
        /// <summary>
        /// Cancelar un CFDI en FiscalAPI usando UUID
        /// Endpoint: DELETE /api/v4/invoices
        /// Documentación: https://documenter.getpostman.com/view/4346593/2sB2j4eqXr#155b7a7a-535d-421d-9915-c5b9921d29ae
        /// </summary>
        public async Task<RespuestaCancelacionCFDI> CancelarCFDI(string uuid, string rfcEmisor, string motivoCancelacion, string uuidSustitucion = null)
        {
            var respuesta = new RespuestaCancelacionCFDI
            {
                Exitoso = false
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("=== CANCELAR CFDI FISCALAPI ===");
                System.Diagnostics.Debug.WriteLine($"UUID: {uuid}");
                System.Diagnostics.Debug.WriteLine($"RFC Emisor: {rfcEmisor}");
                System.Diagnostics.Debug.WriteLine($"Motivo: {motivoCancelacion}");
                System.Diagnostics.Debug.WriteLine($"UUID Sustitución: {uuidSustitucion ?? "N/A"}");

                // Validar que existan los certificados
                if (string.IsNullOrEmpty(_configuracion.CertificadoBase64) || string.IsNullOrEmpty(_configuracion.LlavePrivadaBase64))
                {
                    respuesta.Mensaje = "Los certificados fiscales (CSD) son requeridos para cancelar";
                    respuesta.CodigoError = "MISSING_CERTIFICATES";
                    return respuesta;
                }

                // Según documentación de FiscalAPI - DELETE /api/v4/invoices
                var requestBody = new
                {
                    invoiceUuid = uuid,
                    tin = rfcEmisor,
                    cancellationReasonCode = motivoCancelacion,
                    replacementUuid = string.IsNullOrEmpty(uuidSustitucion) ? "" : uuidSustitucion,
                    taxCredentials = new[]
                    {
                        new
                        {
                            base64File = _configuracion.CertificadoBase64,
                            fileType = 0, // CertificateCsd
                            password = _configuracion.PasswordLlave
                        },
                        new
                        {
                            base64File = _configuracion.LlavePrivadaBase64,
                            fileType = 1, // PrivateKeyCsd
                            password = _configuracion.PasswordLlave
                        }
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                System.Diagnostics.Debug.WriteLine($"Request Body (sin certificados completos por logs): invoiceUuid={uuid}, tin={rfcEmisor}");
                System.Diagnostics.Debug.WriteLine($"Request URL: {_configuracion.UrlApi}/api/v4/invoices");

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                
                // Crear request DELETE con body
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_configuracion.UrlApi + "/api/v4/invoices"),
                    Content = content
                };

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    respuesta.Exitoso = true;
                    respuesta.EstatusCancelacion = "CANCELADA";
                    respuesta.FechaCancelacion = DateTime.Now;
                    respuesta.Mensaje = "CFDI cancelado exitosamente";
                    System.Diagnostics.Debug.WriteLine("✅ Cancelación exitosa");
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
                            try
                            {
                                var errorResponse = JsonConvert.DeserializeObject<FiscalAPIErrorResponse>(responseBody);
                                respuesta.Mensaje = $"Error de validación: {errorResponse.Message ?? responseBody}";
                            }
                            catch
                            {
                                respuesta.Mensaje = $"Error de validación: {responseBody}";
                            }
                            respuesta.CodigoError = "422_VALIDATION_ERROR";
                            break;

                        default:
                            respuesta.Mensaje = $"Error al cancelar: {responseBody}";
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine($"❌ Error: {respuesta.Mensaje}");
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al cancelar CFDI: {ex.Message}";
                respuesta.CodigoError = "CANCEL_ERROR";
                System.Diagnostics.Debug.WriteLine($"❌ Excepción: {ex.Message}");
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
        /// Descargar PDF de factura desde FiscalAPI
        /// Endpoint: POST /api/v4/invoices/pdf
        /// Headers: X-TENANT-KEY, X-TIME-ZONE
        /// Body: { "invoiceId": "..." }
        /// Response: JSON con PDF en base64
        /// </summary>
        public async Task<byte[]> DescargarPDF(string invoiceId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DESCARGA PDF FISCALAPI ===");
                System.Diagnostics.Debug.WriteLine($"InvoiceId: {invoiceId}");
                System.Diagnostics.Debug.WriteLine($"Endpoint: {_configuracion.UrlApi}/api/v4/invoices/pdf");
                
                // Body exacto según documentación de FiscalAPI
                var requestBody = new
                {
                    invoiceId = invoiceId
                };

                var json = JsonConvert.SerializeObject(requestBody);
                System.Diagnostics.Debug.WriteLine($"Request Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/v4/invoices/pdf", content);
                
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Response length: {responseBody.Length} chars");
                    
                    // FiscalAPI devuelve JSON: { "data": { "base64File": "..." } }
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    
                    if (jsonResponse?.data?.base64File != null)
                    {
                        string base64Pdf = jsonResponse.data.base64File.ToString();
                        System.Diagnostics.Debug.WriteLine($"Base64 PDF length: {base64Pdf.Length} chars");
                        
                        byte[] pdfBytes = Convert.FromBase64String(base64Pdf);
                        System.Diagnostics.Debug.WriteLine($"✅ PDF descargado: {pdfBytes.Length} bytes");
                        return pdfBytes;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Respuesta sin campo 'data.base64File': {responseBody.Substring(0, Math.Min(500, responseBody.Length))}");
                        throw new Exception("La respuesta de FiscalAPI no contiene el campo 'data.base64File'");
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ Error FiscalAPI: {response.StatusCode} - {errorBody}");
                    throw new Exception($"Error al descargar PDF de FiscalAPI: {response.StatusCode} - {errorBody}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Excepción en DescargarPDF: {ex.Message}");
                throw new Exception($"Error al descargar PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Enviar CFDI por correo usando FiscalAPI
        /// Endpoint: POST /api/v4/invoices/send
        /// </summary>
        public async Task<bool> EnviarPorCorreo(string invoiceId, string toEmail)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Enviando correo para InvoiceId: {invoiceId} a {toEmail}");

                var request = new
                {
                    invoiceId = invoiceId,
                    toEmail = toEmail
                };

                string jsonBody = JsonConvert.SerializeObject(request);
                System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Request body: {jsonBody}");

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/v4/invoices/send", content);

                string responseBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Response status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Response body: {responseBody}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Error al enviar correo: {response.StatusCode} - {responseBody}");
                    throw new Exception($"Error al enviar correo: {response.StatusCode} - {responseBody}");
                }

                System.Diagnostics.Debug.WriteLine("[FiscalAPIService] Correo enviado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FiscalAPIService] Excepción al enviar correo: {ex.Message}");
                throw new Exception($"Error al enviar correo por FiscalAPI: {ex.Message}", ex);
            }
        }

        #region Consulta de Estatus en SAT

        /// <summary>
        /// Consulta el estatus de un CFDI en el SAT
        /// Endpoint: POST /api/v4/invoices/status
        /// Documentación: https://documenter.getpostman.com/view/4346593/2sB2j4eqXr
        /// </summary>
        /// <param name="uuid">UUID de la factura</param>
        /// <param name="rfcEmisor">RFC del emisor</param>
        /// <param name="rfcReceptor">RFC del receptor</param>
        /// <param name="total">Monto total de la factura</param>
        /// <param name="ultimos8DigitosSello">Últimos 8 dígitos del sello digital (SelloCFDI)</param>
        public async Task<RespuestaConsulta> ConsultarEstatusSAT(string uuid, string rfcEmisor, string rfcReceptor, decimal total, string ultimos8DigitosSello)
        {
            var respuesta = new RespuestaConsulta
            {
                Exitoso = false
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("=== CONSULTAR ESTATUS SAT FISCALAPI ===");
                System.Diagnostics.Debug.WriteLine($"UUID: {uuid}");
                System.Diagnostics.Debug.WriteLine($"RFC Emisor: {rfcEmisor}");
                System.Diagnostics.Debug.WriteLine($"RFC Receptor: {rfcReceptor}");
                System.Diagnostics.Debug.WriteLine($"Total: {total}");
                System.Diagnostics.Debug.WriteLine($"Últimos 8 dígitos sello: {ultimos8DigitosSello}");

                var requestBody = new
                {
                    issuerTin = rfcEmisor,
                    recipientTin = rfcReceptor,
                    invoiceTotal = total,
                    invoiceUuid = uuid,
                    last8DigitsIssuerSignature = ultimos8DigitosSello
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                System.Diagnostics.Debug.WriteLine($"Request Body: {jsonRequest}");

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/v4/invoices/status", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    // Parsear respuesta usando modelo específico
                    try
                    {
                        var fiscalResponse = JsonConvert.DeserializeObject<FiscalAPIEstatusResponse>(responseBody);
                        
                        if (fiscalResponse.Succeeded && fiscalResponse.Data != null)
                        {
                            respuesta.Exitoso = true;
                            respuesta.EstatusSAT = fiscalResponse.Data.Status;
                            respuesta.Mensaje = string.IsNullOrEmpty(fiscalResponse.Message) 
                                ? fiscalResponse.Data.StatusCode 
                                : fiscalResponse.Message;

                            System.Diagnostics.Debug.WriteLine($"Status Code: {fiscalResponse.Data.StatusCode}");
                            System.Diagnostics.Debug.WriteLine($"Estatus SAT: {fiscalResponse.Data.Status}");
                            System.Diagnostics.Debug.WriteLine($"Cancelable: {fiscalResponse.Data.CancelableStatus}");
                            System.Diagnostics.Debug.WriteLine($"Estatus cancelación: {fiscalResponse.Data.CancellationStatus}");
                            System.Diagnostics.Debug.WriteLine($"Validación EFOS: {fiscalResponse.Data.EfosValidation}");
                        }
                        else
                        {
                            // Respuesta exitosa pero sin datos válidos
                            respuesta.Exitoso = false;
                            respuesta.Mensaje = fiscalResponse?.Message ?? "No se pudo consultar el estatus";
                            respuesta.CodigoError = "NO_DATA";
                            respuesta.ErrorTecnico = responseBody;
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        // Si falla el parseo, intentar parsear como respuesta simple
                        System.Diagnostics.Debug.WriteLine($"Error al parsear respuesta estructurada, intentando parseo simple: {jsonEx.Message}");
                        
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                        
                        // Intentar extraer campos directamente
                        string status = jsonResponse?.data?.status?.ToString() ?? jsonResponse?.status?.ToString();
                        string message = jsonResponse?.message?.ToString();
                        
                        if (!string.IsNullOrEmpty(status))
                        {
                            respuesta.Exitoso = true;
                            respuesta.EstatusSAT = status;
                            respuesta.Mensaje = message ?? "Consulta exitosa";
                        }
                        else
                        {
                            respuesta.Mensaje = "Respuesta de FiscalAPI sin estructura esperada";
                            respuesta.ErrorTecnico = responseBody;
                        }
                    }
                }
                else
                {
                    // Error HTTP
                    var errorResponse = JsonConvert.DeserializeObject<FiscalAPIEstatusResponse>(responseBody);
                    respuesta.Mensaje = errorResponse?.Message ?? $"Error HTTP {response.StatusCode}";
                    respuesta.CodigoError = response.StatusCode.ToString();
                    respuesta.ErrorTecnico = responseBody;

                    System.Diagnostics.Debug.WriteLine($"Error al consultar: {respuesta.Mensaje}");
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al consultar estatus: {ex.Message}";
                respuesta.ErrorTecnico = ex.ToString();
                System.Diagnostics.Debug.WriteLine($"Excepción: {ex}");
            }

            return respuesta;
        }

        #endregion

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

