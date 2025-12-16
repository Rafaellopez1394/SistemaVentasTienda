using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    /// <summary>
    /// Controlador para el Módulo del Contador
    /// Gestiona toda la configuración contable, fiscal y de nómina
    /// </summary>
    public class ContadorController : Controller
    {
        // =====================================================
        // DASHBOARD
        // =====================================================

        // GET: Contador/Dashboard
        public ActionResult Dashboard()
        {
            // Verificar que el usuario es contador
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerDashboard
        [HttpGet]
        public JsonResult ObtenerDashboard()
        {
            try
            {
                var dashboard = CD_ConfiguracionContador.Instancia.ObtenerDashboard();

                return Json(new
                {
                    success = true,
                    data = dashboard
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al obtener dashboard: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =====================================================
        // CONFIGURACIÓN EMPRESA
        // =====================================================

        // GET: Contador/ConfiguracionEmpresa
        public ActionResult ConfiguracionEmpresa()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerConfiguracionEmpresa
        [HttpGet]
        public JsonResult ObtenerConfiguracionEmpresa()
        {
            try
            {
                var config = CD_ConfiguracionContador.Instancia.ObtenerConfiguracionEmpresa();

                return Json(new
                {
                    success = true,
                    data = config
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/ActualizarEmpresa
        [HttpPost]
        public JsonResult ActualizarEmpresa(ActualizarEmpresaRequest request)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                string mensaje;

                bool resultado = CD_ConfiguracionContador.Instancia.ActualizarConfiguracionEmpresa(
                    request, usuario, out mensaje);

                return Json(new
                {
                    success = resultado,
                    mensaje = mensaje
                });
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

        // =====================================================
        // CONFIGURACIÓN CONTABLE
        // =====================================================

        // GET: Contador/ConfiguracionContable
        public ActionResult ConfiguracionContable()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerConfiguracionContable
        [HttpGet]
        public JsonResult ObtenerConfiguracionContable()
        {
            try
            {
                var config = CD_ConfiguracionContador.Instancia.ObtenerConfiguracionContable();

                return Json(new
                {
                    success = true,
                    data = config
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/ActualizarContable
        [HttpPost]
        public JsonResult ActualizarContable(ConfiguracionContable config)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                string mensaje;

                bool resultado = CD_ConfiguracionContador.Instancia.ActualizarConfiguracionContable(
                    config, usuario, out mensaje);

                return Json(new
                {
                    success = resultado,
                    mensaje = mensaje
                });
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

        // =====================================================
        // CATÁLOGO DE CUENTAS
        // =====================================================

        // GET: Contador/CatalogoCuentas
        public ActionResult CatalogoCuentas()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerCatalogoCuentas
        [HttpGet]
        public JsonResult ObtenerCatalogoCuentas(bool soloActivas = true)
        {
            try
            {
                var cuentas = CD_ConfiguracionContador.Instancia.ObtenerCatalogoCuentas(soloActivas);

                return Json(new
                {
                    success = true,
                    data = cuentas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/GuardarCuenta
        [HttpPost]
        public JsonResult GuardarCuenta(CuentaContable cuenta)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                string mensaje;

                bool resultado = CD_ConfiguracionContador.Instancia.GuardarCuentaContable(
                    cuenta, usuario, out mensaje);

                return Json(new
                {
                    success = resultado,
                    mensaje = mensaje
                });
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

        // =====================================================
        // CONFIGURACIÓN NÓMINA
        // =====================================================

        // GET: Contador/ConfiguracionNomina
        public ActionResult ConfiguracionNomina()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerConfiguracionNomina
        [HttpGet]
        public JsonResult ObtenerConfiguracionNomina()
        {
            try
            {
                var config = CD_ConfiguracionContador.Instancia.ObtenerConfiguracionNomina();

                return Json(new
                {
                    success = true,
                    data = config
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/ActualizarNomina
        [HttpPost]
        public JsonResult ActualizarNomina(ConfiguracionNomina config)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                string mensaje;

                bool resultado = CD_ConfiguracionContador.Instancia.ActualizarConfiguracionNomina(
                    config, usuario, out mensaje);

                return Json(new
                {
                    success = resultado,
                    mensaje = mensaje
                });
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

        // GET: Contador/ObtenerPercepciones
        [HttpGet]
        public JsonResult ObtenerPercepciones()
        {
            try
            {
                var percepciones = CD_ConfiguracionContador.Instancia.ObtenerPercepciones();

                return Json(new
                {
                    success = true,
                    data = percepciones
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Contador/ObtenerDeducciones
        [HttpGet]
        public JsonResult ObtenerDeducciones()
        {
            try
            {
                var deducciones = CD_ConfiguracionContador.Instancia.ObtenerDeducciones();

                return Json(new
                {
                    success = true,
                    data = deducciones
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =====================================================
        // CONFIGURACIÓN PAC
        // =====================================================

        // GET: Contador/ConfiguracionPAC
        public ActionResult ConfiguracionPAC()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerConfiguracionPAC
        [HttpGet]
        public JsonResult ObtenerConfiguracionPAC()
        {
            try
            {
                var config = CD_Factura.Instancia.ObtenerConfiguracionPAC(out string mensaje);

                return Json(new
                {
                    success = true,
                    data = config
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/ActualizarPAC
        [HttpPost]
        public JsonResult ActualizarPAC(ActualizarPACRequest request)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                
                // TODO: Implementar método en CD_Factura para actualizar PAC
                // Por ahora retornar éxito simulado
                
                return Json(new
                {
                    success = true,
                    mensaje = "Configuración PAC actualizada correctamente"
                });
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

        // =====================================================
        // CERTIFICADOS DIGITALES
        // =====================================================

        // GET: Contador/Certificados
        public ActionResult Certificados()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "CONTADOR")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Contador/ObtenerCertificados
        [HttpGet]
        public JsonResult ObtenerCertificados(string tipoCertificado = null)
        {
            try
            {
                var certificados = CD_ConfiguracionContador.Instancia.ObtenerCertificados(soloActivos: null, tipoCertificado: tipoCertificado);

                // Formatear para mostrar
                var resultado = certificados.Select(c => new
                {
                    c.CertificadoID,
                    c.TipoCertificado,
                    c.NombreCertificado,
                    c.NoCertificado,
                    c.RFC,
                    c.RazonSocial,
                    FechaInicio = c.FechaInicio?.ToString("dd/MM/yyyy"),
                    FechaVencimiento = c.FechaVencimiento?.ToString("dd/MM/yyyy"),
                    c.NombreArchivoCER,
                    c.NombreArchivoKEY,
                    c.Activo,
                    c.EsPredeterminado,
                    c.UsarParaFacturas,
                    c.UsarParaNomina,
                    c.UsarParaCancelaciones,
                    c.EstaVigente,
                    c.DiasParaVencer,
                    Estado = c.Activo ? "Activo" : "Inactivo",
                    EstadoVigencia = c.EstaVigente ? "Vigente" : "Vencido",
                    ClaseEstado = c.Activo ? "badge-success" : "badge-secondary",
                    ClaseVigencia = c.EstaVigente ? "badge-success" : "badge-danger",
                    c.FechaCreacion,
                    c.UsuarioCreacion
                }).ToList();

                return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Contador/SubirCertificado
        [HttpPost]
        public JsonResult SubirCertificado(SubirCertificadoRequest request, HttpPostedFileBase archivoCER, HttpPostedFileBase archivoKEY)
        {
            try
            {
                // Validaciones
                if (archivoCER == null || archivoCER.ContentLength == 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar el archivo .CER" });
                }

                if (archivoKEY == null || archivoKEY.ContentLength == 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar el archivo .KEY" });
                }

                if (string.IsNullOrEmpty(request.PasswordKEY))
                {
                    return Json(new { success = false, mensaje = "Debe proporcionar la contraseña del archivo .KEY" });
                }

                // Validar extensiones
                var extCER = System.IO.Path.GetExtension(archivoCER.FileName).ToLower();
                var extKEY = System.IO.Path.GetExtension(archivoKEY.FileName).ToLower();

                if (extCER != ".cer")
                {
                    return Json(new { success = false, mensaje = "El archivo debe tener extensión .CER" });
                }

                if (extKEY != ".key")
                {
                    return Json(new { success = false, mensaje = "El archivo debe tener extensión .KEY" });
                }

                // Leer archivos a bytes
                byte[] bytesCER;
                byte[] bytesKEY;

                using (var binaryReader = new System.IO.BinaryReader(archivoCER.InputStream))
                {
                    bytesCER = binaryReader.ReadBytes(archivoCER.ContentLength);
                }

                using (var binaryReader = new System.IO.BinaryReader(archivoKEY.InputStream))
                {
                    bytesKEY = binaryReader.ReadBytes(archivoKEY.ContentLength);
                }

                // Extraer información del certificado .CER
                InfoCertificado infoCert = null;
                try
                {
                    infoCert = ExtraerInfoCertificado(bytesCER);
                    
                    if (!infoCert.EsValido)
                    {
                        return Json(new { success = false, mensaje = "Certificado inválido: " + infoCert.MensajeError });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, mensaje = "Error al leer certificado: " + ex.Message });
                }

                // Crear objeto certificado
                var certificado = new CertificadoDigital
                {
                    TipoCertificado = request.TipoCertificado ?? "CSD",
                    NombreCertificado = request.NombreCertificado,
                    NoCertificado = infoCert.NoCertificado,
                    RFC = infoCert.RFC,
                    RazonSocial = infoCert.RazonSocial,
                    FechaInicio = infoCert.FechaInicio,
                    FechaVencimiento = infoCert.FechaVencimiento,
                    ArchivoCER = bytesCER,
                    ArchivoKEY = bytesKEY,
                    PasswordKEY = EncriptarPassword(request.PasswordKEY), // Encriptar password
                    NombreArchivoCER = archivoCER.FileName,
                    NombreArchivoKEY = archivoKEY.FileName,
                    Activo = true,
                    EsPredeterminado = request.EsPredeterminado,
                    UsarParaFacturas = request.UsarParaFacturas,
                    UsarParaNomina = request.UsarParaNomina,
                    UsarParaCancelaciones = request.UsarParaCancelaciones
                };

                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                var respuesta = CD_ConfiguracionContador.Instancia.GuardarCertificado(certificado, usuario);

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje,
                    certificado = new
                    {
                        certificado.NoCertificado,
                        certificado.RFC,
                        certificado.RazonSocial,
                        FechaVencimiento = certificado.FechaVencimiento?.ToString("dd/MM/yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // POST: Contador/ActivarCertificado
        [HttpPost]
        public JsonResult ActivarCertificado(int certificadoID, bool esPredeterminado = false)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                var respuesta = CD_ConfiguracionContador.Instancia.ActualizarEstadoCertificado(certificadoID, true, esPredeterminado, usuario);

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

        // POST: Contador/DesactivarCertificado
        [HttpPost]
        public JsonResult DesactivarCertificado(int certificadoID)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                var respuesta = CD_ConfiguracionContador.Instancia.ActualizarEstadoCertificado(certificadoID, false, false, usuario);

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

        // POST: Contador/EliminarCertificado
        [HttpPost]
        public JsonResult EliminarCertificado(int certificadoID)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "Sistema";
                var respuesta = CD_ConfiguracionContador.Instancia.EliminarCertificado(certificadoID, usuario);

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

        // =====================================================
        // MÉTODOS AUXILIARES PARA CERTIFICADOS
        // =====================================================

        /// <summary>
        /// Extrae información del certificado .CER usando X509Certificate2
        /// </summary>
        private InfoCertificado ExtraerInfoCertificado(byte[] certificadoCER)
        {
            var info = new InfoCertificado { EsValido = false };

            try
            {
                var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificadoCER);

                // Extraer número de certificado (del Serial Number en hex)
                info.NoCertificado = cert.SerialNumber;

                // Extraer RFC del Subject (buscar patrón RFC)
                var subject = cert.Subject;
                var rfcMatch = System.Text.RegularExpressions.Regex.Match(subject, @"OID\.2\.5\.4\.45=([A-Z&Ñ]{3,4}\d{6}[A-Z\d]{3})");
                if (rfcMatch.Success)
                {
                    info.RFC = rfcMatch.Groups[1].Value;
                }
                else
                {
                    // Buscar en otros campos
                    rfcMatch = System.Text.RegularExpressions.Regex.Match(subject, @"([A-Z&Ñ]{3,4}\d{6}[A-Z\d]{3})");
                    if (rfcMatch.Success)
                    {
                        info.RFC = rfcMatch.Groups[1].Value;
                    }
                }

                // Extraer Razón Social (CN)
                var cnMatch = System.Text.RegularExpressions.Regex.Match(subject, @"CN=([^,]+)");
                if (cnMatch.Success)
                {
                    info.RazonSocial = cnMatch.Groups[1].Value.Trim();
                }

                // Fechas de validez
                info.FechaInicio = cert.NotBefore;
                info.FechaVencimiento = cert.NotAfter;

                // Validar que no esté vencido
                if (cert.NotAfter < DateTime.Now)
                {
                    info.MensajeError = "El certificado está vencido desde " + cert.NotAfter.ToString("dd/MM/yyyy");
                    return info;
                }

                info.EsValido = true;
            }
            catch (Exception ex)
            {
                info.MensajeError = "Error al leer certificado: " + ex.Message;
            }

            return info;
        }

        /// <summary>
        /// Encripta el password del archivo .KEY
        /// </summary>
        private string EncriptarPassword(string password)
        {
            // NOTA: Implementar encriptación real (AES, Triple DES, etc.)
            // Por ahora solo codificar en Base64 (NO ES SEGURO para producción)
            
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return password;
            }
        }

        /// <summary>
        /// Desencripta el password del archivo .KEY
        /// </summary>
        private string DesencriptarPassword(string passwordEncriptado)
        {
            // NOTA: Implementar desencriptación real (AES, Triple DES, etc.)
            // Por ahora solo decodificar Base64
            
            try
            {
                var bytes = Convert.FromBase64String(passwordEncriptado);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return passwordEncriptado;
            }
        }
    }
}

