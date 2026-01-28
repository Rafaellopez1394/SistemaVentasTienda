using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class ConfiguracionFiscalController : Controller
    {
        // GET: ConfiguracionFiscal
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Login");

            return View();
        }

        // GET: Obtener configuración actual
        [HttpGet]
        public JsonResult ObtenerConfiguracion()
        {
            try
            {
                var config = new
                {
                    success = true,
                    data = ObtenerConfiguracionActual()
                };
                return Json(config, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener certificados
        [HttpGet]
        public JsonResult ObtenerCertificados()
        {
            try
            {
                var certificados = ObtenerListaCertificados();
                return Json(new { success = true, data = certificados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Guardar configuración fiscal
        [HttpPost]
        public JsonResult GuardarConfiguracion(string rfcEmisor, string nombreEmisor, string regimenFiscal, 
            string codigoPostal, string apiKey, string tenant, string ambiente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    conn.Open();
                    
                    // Verificar si ya existe configuración
                    var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM ConfiguracionFiscalAPI", conn);
                    int count = (int)cmdCheck.ExecuteScalar();

                    string query;
                    if (count > 0)
                    {
                        // UPDATE
                        query = @"UPDATE ConfiguracionFiscalAPI SET 
                                    RfcEmisor = @RfcEmisor,
                                    NombreEmisor = @NombreEmisor,
                                    RegimenFiscal = @RegimenFiscal,
                                    CodigoPostal = @CodigoPostal,
                                    ApiKey = @ApiKey,
                                    Tenant = @Tenant,
                                    Ambiente = @Ambiente,
                                    FechaModificacion = GETDATE(),
                                    Activo = 1";
                    }
                    else
                    {
                        // INSERT
                        query = @"INSERT INTO ConfiguracionFiscalAPI 
                                    (RfcEmisor, NombreEmisor, RegimenFiscal, CodigoPostal, ApiKey, Tenant, Ambiente, Activo, FechaCreacion)
                                  VALUES 
                                    (@RfcEmisor, @NombreEmisor, @RegimenFiscal, @CodigoPostal, @ApiKey, @Tenant, @Ambiente, 1, GETDATE())";
                    }

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RfcEmisor", rfcEmisor ?? "");
                    cmd.Parameters.AddWithValue("@NombreEmisor", nombreEmisor ?? "");
                    cmd.Parameters.AddWithValue("@RegimenFiscal", regimenFiscal ?? "");
                    cmd.Parameters.AddWithValue("@CodigoPostal", codigoPostal ?? "");
                    cmd.Parameters.AddWithValue("@ApiKey", apiKey ?? "");
                    cmd.Parameters.AddWithValue("@Tenant", tenant ?? "");
                    cmd.Parameters.AddWithValue("@Ambiente", ambiente ?? "Pruebas");

                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Configuración guardada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Cargar certificado
        [HttpPost]
        public JsonResult CargarCertificado(string nombreCertificado, string rfc, string razonSocial, 
            string password, HttpPostedFileBase archivoCER, HttpPostedFileBase archivoKEY)
        {
            try
            {
                if (archivoCER == null || archivoKEY == null)
                {
                    return Json(new { success = false, message = "Debe seleccionar ambos archivos (CER y KEY)" });
                }

                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Debe proporcionar la contraseña del certificado" });
                }

                // Leer archivos
                byte[] cerBytes = new byte[archivoCER.ContentLength];
                archivoCER.InputStream.Read(cerBytes, 0, archivoCER.ContentLength);

                byte[] keyBytes = new byte[archivoKEY.ContentLength];
                archivoKEY.InputStream.Read(keyBytes, 0, archivoKEY.ContentLength);

                // Convertir a Base64
                string cerBase64 = Convert.ToBase64String(cerBytes);
                string keyBase64 = Convert.ToBase64String(keyBytes);

                // Extraer información del certificado
                string noCertificado = "";
                DateTime fechaVigenciaInicio = DateTime.Now;
                DateTime fechaVigenciaFin = DateTime.Now.AddYears(4);

                try
                {
                    // Intentar leer el certificado X509
                    var cert = new X509Certificate2(cerBytes);
                    noCertificado = cert.SerialNumber;
                    fechaVigenciaInicio = cert.NotBefore;
                    fechaVigenciaFin = cert.NotAfter;
                }
                catch
                {
                    // Si falla, usar valores por defecto (certificado ya leído como bytes)
                    noCertificado = DateTime.Now.Ticks.ToString().Substring(0, 20);
                }

                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    conn.Open();

                    // Verificar si ya existe un certificado con el mismo número
                    var cmdCheck = new SqlCommand("SELECT CertificadoID FROM CertificadosDigitales WHERE NoCertificado = @NoCertificado", conn);
                    cmdCheck.Parameters.AddWithValue("@NoCertificado", noCertificado);
                    var existingId = cmdCheck.ExecuteScalar();

                    string query;
                    if (existingId != null)
                    {
                        // UPDATE - Actualizar certificado existente
                        query = @"UPDATE CertificadosDigitales SET 
                                    NombreCertificado = @NombreCertificado,
                                    RFC = @RFC,
                                    RazonSocial = @RazonSocial,
                                    ArchivoCER = @ArchivoCER,
                                    ArchivoKEY = @ArchivoKEY,
                                    CertificadoBase64 = @CertificadoBase64,
                                    LlavePrivadaBase64 = @LlavePrivadaBase64,
                                    PasswordKEY = @PasswordKEY,
                                    FechaVigenciaInicio = @FechaVigenciaInicio,
                                    FechaVigenciaFin = @FechaVigenciaFin,
                                    Activo = 1,
                                    EsPredeterminado = 1,
                                    UsuarioModificacion = @Usuario,
                                    FechaModificacion = GETDATE()
                                  WHERE NoCertificado = @NoCertificado";
                    }
                    else
                    {
                        // INSERT - Insertar nuevo certificado
                        // Desactivar certificados previos como predeterminado
                        var cmdDesactivar = new SqlCommand("UPDATE CertificadosDigitales SET EsPredeterminado = 0", conn);
                        cmdDesactivar.ExecuteNonQuery();

                        query = @"INSERT INTO CertificadosDigitales 
                                    (NombreCertificado, NoCertificado, RFC, RazonSocial, 
                                     ArchivoCER, ArchivoKEY, CertificadoBase64, LlavePrivadaBase64, PasswordKEY, 
                                     FechaVigenciaInicio, FechaVigenciaFin,
                                     Activo, EsPredeterminado, FechaCreacion, UsuarioCreacion)
                                  VALUES 
                                    (@NombreCertificado, @NoCertificado, @RFC, @RazonSocial, 
                                     @ArchivoCER, @ArchivoKEY, @CertificadoBase64, @LlavePrivadaBase64, @PasswordKEY,
                                     @FechaVigenciaInicio, @FechaVigenciaFin,
                                     1, 1, GETDATE(), @Usuario)";
                    }

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NombreCertificado", nombreCertificado ?? "");
                    cmd.Parameters.AddWithValue("@NoCertificado", noCertificado);
                    cmd.Parameters.AddWithValue("@RFC", rfc ?? "");
                    cmd.Parameters.AddWithValue("@RazonSocial", razonSocial ?? "");
                    cmd.Parameters.AddWithValue("@ArchivoCER", cerBytes);
                    cmd.Parameters.AddWithValue("@ArchivoKEY", keyBytes);
                    cmd.Parameters.AddWithValue("@CertificadoBase64", cerBase64);
                    cmd.Parameters.AddWithValue("@LlavePrivadaBase64", keyBase64);
                    cmd.Parameters.AddWithValue("@PasswordKEY", password);
                    cmd.Parameters.AddWithValue("@FechaVigenciaInicio", fechaVigenciaInicio);
                    cmd.Parameters.AddWithValue("@FechaVigenciaFin", fechaVigenciaFin);
                    cmd.Parameters.AddWithValue("@Usuario", Session["Usuario"]?.ToString() ?? "Sistema");

                    cmd.ExecuteNonQuery();

                    // También actualizar ConfiguracionFiscalAPI con Base64
                    // Primero verificar si existe un registro
                    var cmdCheckConfig = new SqlCommand("SELECT COUNT(*) FROM ConfiguracionFiscalAPI", conn);
                    int configCount = (int)cmdCheckConfig.ExecuteScalar();

                    if (configCount == 0)
                    {
                        // Crear registro inicial si no existe
                        var cmdInsertConfig = new SqlCommand(@"
                            INSERT INTO ConfiguracionFiscalAPI 
                            (CertificadoBase64, LlavePrivadaBase64, PasswordLlave, RfcEmisor, NombreEmisor, Activo, FechaCreacion)
                            VALUES (@CerBase64, @KeyBase64, @Password, @RFC, @RazonSocial, 1, GETDATE())", conn);
                        
                        cmdInsertConfig.Parameters.AddWithValue("@CerBase64", cerBase64);
                        cmdInsertConfig.Parameters.AddWithValue("@KeyBase64", keyBase64);
                        cmdInsertConfig.Parameters.AddWithValue("@Password", password);
                        cmdInsertConfig.Parameters.AddWithValue("@RFC", rfc ?? "");
                        cmdInsertConfig.Parameters.AddWithValue("@RazonSocial", razonSocial ?? "");
                        cmdInsertConfig.ExecuteNonQuery();
                    }
                    else
                    {
                        // Actualizar registro existente
                        var cmdConfig = new SqlCommand(@"
                            UPDATE ConfiguracionFiscalAPI SET 
                                CertificadoBase64 = @CerBase64,
                                LlavePrivadaBase64 = @KeyBase64,
                                PasswordLlave = @Password,
                                FechaModificacion = GETDATE()", conn);
                        
                        cmdConfig.Parameters.AddWithValue("@CerBase64", cerBase64);
                        cmdConfig.Parameters.AddWithValue("@KeyBase64", keyBase64);
                        cmdConfig.Parameters.AddWithValue("@Password", password);
                        cmdConfig.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Certificado cargado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cargar certificado: " + ex.Message });
            }
        }

        // DELETE: Eliminar certificado
        [HttpPost]
        public JsonResult EliminarCertificado(int certificadoId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM CertificadosDigitales WHERE CertificadoID = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", certificadoId);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Certificado eliminado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper: Obtener configuración actual
        private object ObtenerConfiguracionActual()
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 * FROM ConfiguracionFiscalAPI ORDER BY ConfiguracionID DESC", conn);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new
                    {
                        rfcEmisor = reader["RfcEmisor"]?.ToString() ?? "",
                        nombreEmisor = reader["NombreEmisor"]?.ToString() ?? "",
                        regimenFiscal = reader["RegimenFiscal"]?.ToString() ?? "",
                        codigoPostal = reader["CodigoPostal"]?.ToString() ?? "",
                        apiKey = reader["ApiKey"]?.ToString() ?? "",
                        tenant = reader["Tenant"]?.ToString() ?? "",
                        ambiente = reader["Ambiente"]?.ToString() ?? "Pruebas",
                        activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"]),
                        tieneCertificado = reader["CertificadoBase64"] != DBNull.Value && !string.IsNullOrEmpty(reader["CertificadoBase64"]?.ToString())
                    };
                }

                return new
                {
                    rfcEmisor = "",
                    nombreEmisor = "",
                    regimenFiscal = "",
                    codigoPostal = "",
                    apiKey = "",
                    tenant = "",
                    ambiente = "Pruebas",
                    activo = false,
                    tieneCertificado = false
                };
            }
        }

        // Helper: Obtener lista de certificados
        private List<object> ObtenerListaCertificados()
        {
            var certificados = new List<object>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT CertificadoID, NombreCertificado, RFC, RazonSocial, 
                           FechaVigenciaInicio, FechaVigenciaFin, Activo, EsPredeterminado, FechaCreacion
                    FROM CertificadosDigitales 
                    ORDER BY EsPredeterminado DESC, FechaCreacion DESC", conn);
                
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    certificados.Add(new
                    {
                        certificadoId = reader["CertificadoID"],
                        nombre = reader["NombreCertificado"]?.ToString() ?? "",
                        rfc = reader["RFC"]?.ToString() ?? "",
                        razonSocial = reader["RazonSocial"]?.ToString() ?? "",
                        fechaInicio = reader["FechaVigenciaInicio"] != DBNull.Value 
                            ? Convert.ToDateTime(reader["FechaVigenciaInicio"]).ToString("dd/MM/yyyy") 
                            : "",
                        fechaFin = reader["FechaVigenciaFin"] != DBNull.Value 
                            ? Convert.ToDateTime(reader["FechaVigenciaFin"]).ToString("dd/MM/yyyy") 
                            : "",
                        activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"]),
                        predeterminado = reader["EsPredeterminado"] != DBNull.Value && Convert.ToBoolean(reader["EsPredeterminado"]),
                        fechaCreacion = reader["FechaCreacion"] != DBNull.Value 
                            ? Convert.ToDateTime(reader["FechaCreacion"]).ToString("dd/MM/yyyy HH:mm") 
                            : ""
                    });
                }
            }

            return certificados;
        }
    }
}
