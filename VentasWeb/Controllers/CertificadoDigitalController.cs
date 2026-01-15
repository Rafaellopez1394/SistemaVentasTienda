// VentasWeb/Controllers/CertificadoDigitalController.cs
using CapaDatos;
using CapaModelo;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace VentasWeb.Controllers
{
    public class CertificadoDigitalController : Controller
    {
        // GET: Vista principal
        public ActionResult Index()
        {
            return View();
        }

        // GET: Obtener todos los certificados
        [HttpGet]
        public JsonResult ObtenerTodos()
        {
            try
            {
                var certificados = CD_CertificadoDigital.Instancia.ObtenerTodos();
                
                // Formatear fechas para JSON
                var certificadosFormateados = certificados.Select(c => new
                {
                    c.CertificadoID,
                    c.NombreCertificado,
                    c.NoCertificado,
                    c.RFC,
                    c.RazonSocial,
                    FechaVigenciaInicio = c.FechaVigenciaInicio.ToString("yyyy-MM-dd"),
                    FechaVigenciaFin = c.FechaVigenciaFin.ToString("yyyy-MM-dd"),
                    c.Activo,
                    c.EsPredeterminado,
                    EstadoVigencia = c.FechaVigenciaFin < DateTime.Now ? "VENCIDO" :
                                     c.FechaVigenciaFin < DateTime.Now.AddDays(30) ? "POR VENCER" : "VIGENTE",
                    DiasRestantes = (int)(c.FechaVigenciaFin - DateTime.Now).TotalDays
                }).ToList();
                
                return Json(new { success = true, data = certificadosFormateados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener certificado predeterminado
        [HttpGet]
        public JsonResult ObtenerPredeterminado()
        {
            try
            {
                var certificado = CD_CertificadoDigital.Instancia.ObtenerPredeterminado();
                
                if (certificado == null)
                    return Json(new { success = false, mensaje = "No hay certificado predeterminado" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = certificado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cargar nuevo certificado
        [HttpPost]
        public JsonResult CargarCertificado()
        {
            try
            {
                // Validar que se subieron archivos
                if (Request.Files.Count < 2)
                    return Json(new { success = false, mensaje = "Debe proporcionar ambos archivos (.cer y .key)" });

                HttpPostedFileBase archivoCER = Request.Files["archivoCER"];
                HttpPostedFileBase archivoKEY = Request.Files["archivoKEY"];

                if (archivoCER == null || archivoKEY == null)
                    return Json(new { success = false, mensaje = "Debe seleccionar ambos archivos" });

                if (archivoCER.ContentLength == 0 || archivoKEY.ContentLength == 0)
                    return Json(new { success = false, mensaje = "Los archivos están vacíos" });

                // Validar extensiones
                if (!archivoCER.FileName.EndsWith(".cer", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, mensaje = "El archivo del certificado debe ser .cer" });

                if (!archivoKEY.FileName.EndsWith(".key", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, mensaje = "El archivo de la llave debe ser .key" });

                // Leer archivos a bytes
                byte[] bytesCER = new byte[archivoCER.ContentLength];
                archivoCER.InputStream.Read(bytesCER, 0, archivoCER.ContentLength);

                byte[] bytesKEY = new byte[archivoKEY.ContentLength];
                archivoKEY.InputStream.Read(bytesKEY, 0, archivoKEY.ContentLength);

                // Crear request
                var request = new CargarCertificadoRequest
                {
                    NombreCertificado = Request.Form["nombreCertificado"],
                    RFC = Request.Form["rfc"],
                    RazonSocial = Request.Form["razonSocial"],
                    PasswordKEY = Request.Form["passwordKey"],
                    ArchivoCER = bytesCER,
                    ArchivoKEY = bytesKEY,
                    EsPredeterminado = Request.Form["esPredeterminado"] == "true",
                    Usuario = User.Identity.Name ?? "system"
                };

                // Validaciones
                if (string.IsNullOrEmpty(request.NombreCertificado))
                    return Json(new { success = false, mensaje = "Debe proporcionar un nombre" });

                if (string.IsNullOrEmpty(request.RFC))
                    return Json(new { success = false, mensaje = "Debe proporcionar el RFC" });

                if (string.IsNullOrEmpty(request.RazonSocial))
                    return Json(new { success = false, mensaje = "Debe proporcionar la razón social" });

                if (string.IsNullOrEmpty(request.PasswordKEY))
                    return Json(new { success = false, mensaje = "Debe proporcionar la contraseña de la llave privada" });

                // Cargar certificado
                var respuesta = CD_CertificadoDigital.Instancia.CargarCertificado(request);

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje,
                    data = respuesta.Datos
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // POST: Establecer como predeterminado
        [HttpPost]
        public JsonResult EstablecerPredeterminado(int certificadoID)
        {
            try
            {
                var respuesta = CD_CertificadoDigital.Instancia.EstablecerPredeterminado(
                    certificadoID,
                    User.Identity.Name ?? "system"
                );

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // POST: Eliminar certificado
        [HttpPost]
        public JsonResult Eliminar(int certificadoID)
        {
            try
            {
                var respuesta = CD_CertificadoDigital.Instancia.Eliminar(
                    certificadoID,
                    User.Identity.Name ?? "system"
                );

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Validar vigencia
        [HttpGet]
        public JsonResult ValidarVigencia()
        {
            try
            {
                var certificadosPorVencer = CD_CertificadoDigital.Instancia.ValidarVigencia();
                return Json(new { success = true, data = certificadosPorVencer }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ========================================================================
        // INTEGRACION CON FISCALAPI
        // ========================================================================

        // GET: Listar certificados en FiscalAPI
        [HttpGet]
        public async Task<JsonResult> ListarFiscalAPI()
        {
            try
            {
                var config = CD_Factura.Instancia.ObtenerConfiguracionPAC(out string mensajeConfig);
                
                if (config == null)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "No se encontró configuración del PAC. Configure primero las credenciales de FiscalAPI."
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(config.Usuario) || string.IsNullOrWhiteSpace(config.Password))
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "Las credenciales de FiscalAPI no están configuradas"
                    }, JsonRequestBehavior.AllowGet);
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-API-KEY", config.Usuario);
                    client.DefaultRequestHeaders.Add("X-TENANT-KEY", config.Password);
                    client.DefaultRequestHeaders.Add("X-TIME-ZONE", "America/Mexico_City");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string baseUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com";
                    var response = await client.GetAsync($"{baseUrl}/api/v4/tax-files");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(content);

                        return Json(new
                        {
                            success = true,
                            data = result.data.items,
                            ambiente = config.EsProduccion ? "Producción" : "Pruebas"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            mensaje = $"Error al consultar certificados en FiscalAPI: {response.StatusCode}",
                            detalle = errorContent
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al conectar con FiscalAPI: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Subir certificado a FiscalAPI
        [HttpPost]
        public async Task<JsonResult> SubirFiscalAPI()
        {
            try
            {
                // Validar archivos
                if (Request.Files.Count < 2)
                {
                    return Json(new { success = false, mensaje = "Debe proporcionar ambos archivos (.cer y .key)" });
                }

                HttpPostedFileBase archivoCER = Request.Files["certificado"];
                HttpPostedFileBase archivoKEY = Request.Files["llavePrivada"];
                string password = Request.Form["password"];
                string rfc = Request.Form["rfc"];

                if (archivoCER == null || archivoKEY == null)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar ambos archivos" });
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, mensaje = "La contraseña de la llave privada es requerida" });
                }

                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return Json(new { success = false, mensaje = "El RFC es requerido" });
                }

                // Validar extensiones
                if (!archivoCER.FileName.EndsWith(".cer", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, mensaje = "El certificado debe ser un archivo .cer" });
                }

                if (!archivoKEY.FileName.EndsWith(".key", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, mensaje = "La llave privada debe ser un archivo .key" });
                }

                // Leer archivos y convertir a Base64
                byte[] cerBytes = new byte[archivoCER.ContentLength];
                archivoCER.InputStream.Read(cerBytes, 0, archivoCER.ContentLength);

                byte[] keyBytes = new byte[archivoKEY.ContentLength];
                archivoKEY.InputStream.Read(keyBytes, 0, archivoKEY.ContentLength);

                string cerBase64 = Convert.ToBase64String(cerBytes);
                string keyBase64 = Convert.ToBase64String(keyBytes);

                // Obtener configuración del PAC
                var config = CD_Factura.Instancia.ObtenerConfiguracionPAC(out string mensajeConfig);
                
                if (config == null)
                {
                    return Json(new { success = false, mensaje = "No se encontró configuración del PAC" });
                }

                // Usar HttpClient directo - versión simplificada sin SDK
                using (var client = new HttpClient())
                {
                    string baseUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com";
                    
                    client.DefaultRequestHeaders.Add("X-API-KEY", config.Usuario);
                    client.DefaultRequestHeaders.Add("X-TENANT-KEY", config.Password);
                    client.DefaultRequestHeaders.Add("X-TIME-ZONE", "America/Mexico_City");
                    client.Timeout = TimeSpan.FromSeconds(120);

                    // 1. Crear una persona simple
                    string personId = null;
                    var personPayload = new
                    {
                        legalName = rfc,
                        tin = rfc,
                        email = (User?.Identity?.Name ?? "no-reply") + "@local.test",
                        password = Guid.NewGuid().ToString("N")
                    };

                    var personContent = new StringContent(
                        JsonConvert.SerializeObject(personPayload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    try
                    {
                        var personResp = await client.PostAsync($"{baseUrl}/api/v4/persons", personContent);
                        if (personResp.IsSuccessStatusCode)
                        {
                            var personResult = JsonConvert.DeserializeObject<dynamic>(await personResp.Content.ReadAsStringAsync());
                            personId = personResult?.data?.id;
                        }
                        else if ((int)personResp.StatusCode == 409) // Already exists
                        {
                            // Intentar listar y buscar
                            var listResp = await client.GetAsync($"{baseUrl}/api/v4/persons?PageNumber=1&PageSize=100");
                            if (listResp.IsSuccessStatusCode)
                            {
                                var listContent = await listResp.Content.ReadAsStringAsync();
                                var listResult = JsonConvert.DeserializeObject<dynamic>(listContent);
                                if (listResult?.data?.items != null)
                                {
                                    foreach (var item in listResult.data.items)
                                    {
                                        if (item.tin != null && item.tin.ToString().Equals(rfc, StringComparison.OrdinalIgnoreCase))
                                        {
                                            personId = item.id;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }

                    if (string.IsNullOrEmpty(personId))
                    {
                        return Json(new { 
                            success = false, 
                            mensaje = "No se pudo crear ni encontrar la persona en FiscalAPI. Por favor, sube los certificados manualmente desde: " + baseUrl + "/tax-files"
                        });
                    }

                    // 2. Subir certificado (.cer)
                    var cerPayload = new
                    {
                        personId = personId,
                        tin = rfc.ToUpper().Trim(),
                        base64File = cerBase64,
                        fileType = 0, // CertificateCsd
                        password = password
                    };

                    var cerContent = new StringContent(
                        JsonConvert.SerializeObject(cerPayload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var cerResp = await client.PostAsync($"{baseUrl}/api/v4/tax-files", cerContent);
                    if (!cerResp.IsSuccessStatusCode)
                    {
                        var cerError = await cerResp.Content.ReadAsStringAsync();
                        return Json(new { success = false, mensaje = "Error al subir certificado .cer", detalle = cerError });
                    }

                    // 3. Subir llave privada (.key)
                    var keyPayload = new
                    {
                        personId = personId,
                        tin = rfc.ToUpper().Trim(),
                        base64File = keyBase64,
                        fileType = 1, // PrivateKeyCsd
                        password = password
                    };

                    var keyContent = new StringContent(
                        JsonConvert.SerializeObject(keyPayload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var keyResp = await client.PostAsync($"{baseUrl}/api/v4/tax-files", keyContent);
                    if (!keyResp.IsSuccessStatusCode)
                    {
                        var keyError = await keyResp.Content.ReadAsStringAsync();
                        return Json(new { success = false, mensaje = "Error al subir llave privada .key", detalle = keyError });
                    }

                    return Json(new { success = true, mensaje = "Certificados subidos exitosamente a FiscalAPI" });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // POST: Eliminar certificado de FiscalAPI
        [HttpPost]
        public async Task<JsonResult> EliminarFiscalAPI(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, mensaje = "ID del certificado requerido" });
                }

                var config = CD_Factura.Instancia.ObtenerConfiguracionPAC(out string mensajeConfig);
                
                if (config == null)
                {
                    return Json(new { success = false, mensaje = "No se encontró configuración del PAC" });
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-API-KEY", config.Usuario);
                    client.DefaultRequestHeaders.Add("X-TENANT-KEY", config.Password);
                    client.DefaultRequestHeaders.Add("X-TIME-ZONE", "America/Mexico_City");

                    string baseUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com";
                    var response = await client.DeleteAsync($"{baseUrl}/api/v4/tax-files/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = true,
                            mensaje = "Certificado eliminado exitosamente de FiscalAPI"
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            mensaje = "Error al eliminar certificado de FiscalAPI",
                            detalle = errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                });
            }
        }

        // GET: Información de certificados de prueba
        [HttpGet]
        public JsonResult InfoCertificadosPrueba()
        {
            return Json(new
            {
                success = true,
                data = new
                {
                    rfc = "EKU9003173C9",
                    razonSocial = "ESCUELA KEMPER URGATE",
                    password = "12345678a",
                    descargaUrl = "https://fiscalapi-resources.s3.amazonaws.com/certificates.zip",
                    documentacion = "https://docs.fiscalapi.com/testing-data#certificados-de-prueba",
                    nota = "Estos certificados son SOLO para pruebas y NO son válidos para facturación real"
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
