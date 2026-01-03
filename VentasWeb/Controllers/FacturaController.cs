using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using VentasWeb.Utilidades;

namespace VentasWeb.Controllers
{
    public class FacturaController : Controller
    {
        // GET: Factura/Index
        public ActionResult Index(string ventaId = null)
        {
            // Validar sesión
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Si viene con ventaId, pasarlo a la vista para auto-cargar
            ViewBag.VentaID = ventaId;
            return View();
        }

        // GET: Factura/ObtenerDatosVenta (para pre-llenar formulario)
        [HttpGet]
        public JsonResult ObtenerDatosVenta(string ventaId)
        {
            try
            {
                if (string.IsNullOrEmpty(ventaId))
                {
                    return Json(new { success = false, mensaje = "VentaID requerido" }, JsonRequestBehavior.AllowGet);
                }

                Guid ventaGuid = Guid.Parse(ventaId);
                var venta = CD_Venta.Instancia.ObtenerDetalle(ventaGuid);

                if (venta == null)
                {
                    return Json(new { success = false, mensaje = "Venta no encontrada" }, JsonRequestBehavior.AllowGet);
                }

                // Obtener datos del cliente para facturación
                var cliente = CD_Cliente.Instancia.ObtenerPorId(venta.ClienteID);

                // Calcular subtotal e IVA real desde el detalle
                decimal subtotal = 0;
                decimal totalIVA = 0;
                
                foreach (var detalle in venta.Detalle)
                {
                    decimal montoBase = detalle.PrecioVenta * detalle.Cantidad;
                    decimal porcentajeIVA = detalle.Exento ? 0 : (detalle.TasaIVAPorcentaje / 100m);
                    decimal montoIVA = montoBase * porcentajeIVA;
                    
                    subtotal += montoBase;
                    totalIVA += montoIVA;
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        ventaId = venta.VentaID.ToString(),
                        clienteRFC = cliente?.RFC ?? "XAXX010101000",
                        clienteNombre = venta.RazonSocial,
                        total = venta.Total,
                        subtotal = subtotal,
                        iva = totalIVA,
                        conceptos = venta.Detalle,
                        usoCFDI = "G03", // Por defecto: Gastos en general
                        metodoPago = venta.Estatus == "CONTADO" ? "PUE" : "PPD",
                        formaPago = "99" // Por definir
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Factura/ObtenerFacturas (para DataTable)
        [HttpGet]
        public JsonResult ObtenerFacturas(string rfc = null, string fechaDesde = null, string fechaHasta = null, string estatus = null)
        {
            try
            {
                DateTime? desde = null;
                DateTime? hasta = null;

                if (!string.IsNullOrEmpty(fechaDesde))
                    desde = DateTime.Parse(fechaDesde);

                if (!string.IsNullOrEmpty(fechaHasta))
                    hasta = DateTime.Parse(fechaHasta);

                var facturas = CD_Factura.Instancia.ObtenerFacturas(rfc, desde, hasta, estatus);

                return Json(new
                {
                    success = true,
                    data = facturas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Factura/ObtenerDetalle (para modal de detalle)
        [HttpGet]
        public JsonResult ObtenerDetalle(string facturaId)
        {
            try
            {
                if (string.IsNullOrEmpty(facturaId))
                {
                    return Json(new { success = false, mensaje = "FacturaID requerido" }, JsonRequestBehavior.AllowGet);
                }

                Guid facturaGuid = Guid.Parse(facturaId);
                var factura = CD_Factura.Instancia.ObtenerPorId(facturaGuid);

                if (factura == null)
                {
                    return Json(new { success = false, mensaje = "Factura no encontrada" }, JsonRequestBehavior.AllowGet);
                }

                // Obtener detalle e impuestos
                var detalles = CD_Factura.Instancia.ObtenerDetalleFactura(facturaGuid);
                var impuestos = CD_Factura.Instancia.ObtenerImpuestosFactura(facturaGuid);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        factura.FacturaID,
                        factura.Serie,
                        factura.Folio,
                        factura.UUID,
                        factura.FechaEmision,
                        factura.FechaTimbrado,
                        factura.ReceptorRFC,
                        factura.ReceptorNombre,
                        factura.ReceptorUsoCFDI,
                        factura.Subtotal,
                        factura.TotalImpuestosTrasladados,
                        factura.TotalImpuestosRetenidos,
                        Total = factura.MontoTotal,
                        factura.FormaPago,
                        factura.MetodoPago,
                        factura.Estatus,
                        Conceptos = detalles,
                        Impuestos = impuestos
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Factura/GenerarFactura
        [HttpPost]
        public async Task<JsonResult> GenerarFactura(GenerarFacturaRequest request)
        {
            try
            {
                // Obtener usuario de sesión
                string usuario = Session["UsuarioNombre"]?.ToString() ?? "Sistema";

                // Generar y timbrar factura
                var respuesta = await CD_Factura.Instancia.GenerarYTimbrarFactura(request, usuario);

                // Si se generó correctamente y viene de una venta, actualizar el estatus
                if (respuesta.estado && request.VentaID != Guid.Empty)
                {
                    try
                    {
                        dynamic facturaData = respuesta.objeto;
                        Guid facturaId = facturaData.FacturaID;
                        
                        // Actualizar VentasClientes con EstaFacturada = 1 y FacturaID
                        CD_Venta.Instancia.ActualizarEstadoFactura(request.VentaID, facturaId);
                    }
                    catch (Exception exVenta)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al actualizar venta: {exVenta.Message}");
                        // No fallar si hay error, la factura ya se generó
                    }
                }

                return Json(new
                {
                    success = respuesta.estado,
                    mensaje = respuesta.valor,
                    data = respuesta.objeto
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

        // GET: Factura/ObtenerPorUUID
        [HttpGet]
        public JsonResult ObtenerPorUUID(string uuid)
        {
            try
            {
                var factura = CD_Factura.Instancia.ObtenerPorUUID(uuid, out string mensaje);

                if (factura == null)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = mensaje
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        factura.FacturaID,
                        factura.Serie,
                        factura.Folio,
                        factura.UUID,
                        factura.FechaEmision,
                        factura.ReceptorRFC,
                        factura.ReceptorNombre,
                        factura.Total,
                        factura.Estatus
                    }
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

        // GET: Factura/DescargarXML
        public ActionResult DescargarXML(Guid facturaId)
        {
            try
            {
                var factura = CD_Factura.Instancia.ObtenerPorId(facturaId);

                if (factura == null || string.IsNullOrEmpty(factura.XMLTimbrado))
                {
                    return Content("No se encontró el XML de la factura");
                }

                byte[] xmlBytes = System.Text.Encoding.UTF8.GetBytes(factura.XMLTimbrado);
                string nombreArchivo = $"{factura.Serie}{factura.Folio}_{factura.UUID}.xml";

                return File(xmlBytes, "application/xml", nombreArchivo);
            }
            catch (Exception ex)
            {
                return Content("Error al descargar XML: " + ex.Message);
            }
        }

        // GET: Factura/DescargarPDF
        public ActionResult DescargarPDF(Guid facturaId)
        {
            try
            {
                var factura = CD_Factura.Instancia.ObtenerPorId(facturaId);

                if (factura == null)
                {
                    return Content("No se encontró la factura");
                }

                // Generar PDF simple (TODO: implementar generador completo)
                byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes($"Factura {factura.Serie}{factura.Folio} - UUID: {factura.UUID}");
                string nombreArchivo = $"Factura_{factura.Serie}{factura.Folio}_{factura.UUID.Substring(0, 8)}.pdf";

                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return Content("Error al generar PDF: " + ex.Message);
            }
        }

        // GET: Factura/ObtenerUsosCFDI
        [HttpGet]
        public JsonResult ObtenerUsosCFDI()
        {
            try
            {
                var usosList = CD_Catalogos.Instancia.ObtenerUsosCFDI_List();
                var usos = usosList.Select(u => new { 
                    Clave = u.UsoCFDIID, 
                    Descripcion = u.Descripcion 
                });

                return Json(new { success = true, data = usos }, JsonRequestBehavior.AllowGet);
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

        // GET: Factura/ObtenerRegimenesFiscales
        [HttpGet]
        public JsonResult ObtenerRegimenesFiscales()
        {
            try
            {
                // TODO: Obtener de base de datos CatRegimenesFiscales
                var regimenes = new[]
                {
                    new { Clave = "601", Descripcion = "General de Ley Personas Morales" },
                    new { Clave = "603", Descripcion = "Personas Morales con Fines no Lucrativos" },
                    new { Clave = "605", Descripcion = "Sueldos y Salarios e Ingresos Asimilados a Salarios" },
                    new { Clave = "606", Descripcion = "Arrendamiento" },
                    new { Clave = "612", Descripcion = "Personas Físicas con Actividades Empresariales y Profesionales" },
                    new { Clave = "616", Descripcion = "Sin obligaciones fiscales" },
                    new { Clave = "626", Descripcion = "Régimen Simplificado de Confianza" }
                };

                return Json(new { success = true, data = regimenes }, JsonRequestBehavior.AllowGet);
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

        // POST: Factura/CancelarFactura
        [HttpPost]
        public async Task<JsonResult> CancelarFactura(Guid facturaId, string motivo, string uuidSustitucion)
        {
            try
            {
                // Validar sesión
                if (Session["Usuario"] == null)
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

                string usuario = Session["Usuario"].ToString();

                // Validar parámetros
                if (facturaId == Guid.Empty)
                {
                    return Json(new { success = false, mensaje = "ID de factura inválido" });
                }

                if (string.IsNullOrEmpty(motivo))
                {
                    return Json(new { success = false, mensaje = "Debe especificar un motivo de cancelación" });
                }

                // Validar motivo según catálogo SAT
                if (motivo != "01" && motivo != "02" && motivo != "03" && motivo != "04")
                {
                    return Json(new { success = false, mensaje = "Motivo de cancelación inválido" });
                }

                // Si es motivo 01, requiere UUID de sustitución
                if (motivo == "01" && string.IsNullOrEmpty(uuidSustitucion))
                {
                    return Json(new { success = false, mensaje = "El motivo 01 requiere UUID de sustitución" });
                }

                // Llamar a la capa de datos para cancelar
                // Use Task.Run to offload the async operation to a thread pool thread
                // This helps avoid deadlocks in ASP.NET MVC 5 which has a synchronization context
                var respuesta = await Task.Run(() => CD_Factura.Instancia.CancelarCFDI(facturaId, motivo, uuidSustitucion, usuario));

                if (respuesta.Exitoso)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = respuesta.Mensaje,
                        estatusSAT = respuesta.EstatusSAT,
                        fechaCancelacion = respuesta.FechaRespuesta.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = respuesta.Mensaje,
                        codigoError = respuesta.CodigoError
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al cancelar factura: " + ex.Message
                });
            }
        }

        // POST: Factura/EnviarPorEmail
        [HttpPost]
        public JsonResult EnviarPorEmail(string facturaId, string email)
        {
            try
            {
                // Validar sesión
                if (Session["Usuario"] == null)
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

                string usuario = Session["Usuario"].ToString();

                // Validaciones
                if (string.IsNullOrEmpty(facturaId))
                {
                    return Json(new { success = false, mensaje = "ID de factura inválido" });
                }

                Guid facturaGuid;
                if (!Guid.TryParse(facturaId, out facturaGuid))
                {
                    return Json(new { success = false, mensaje = "Formato de ID de factura inválido" });
                }

                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, mensaje = "Email requerido" });
                }

                // Validar formato de email
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != email)
                    {
                        return Json(new { success = false, mensaje = "Formato de email inválido" });
                    }
                }
                catch
                {
                    return Json(new { success = false, mensaje = "Formato de email inválido" });
                }

                // Obtener datos de la factura
                var factura = CD_Factura.Instancia.ObtenerPorId(facturaGuid);

                if (factura == null)
                {
                    return Json(new { success = false, mensaje = "Factura no encontrada" });
                }

                // Verificar que la factura esté timbrada
                if (string.IsNullOrEmpty(factura.UUID))
                {
                    return Json(new { success = false, mensaje = "La factura no ha sido timbrada" });
                }

                // Generar PDF simple
                byte[] pdfBytes = null;
                try
                {
                    pdfBytes = System.Text.Encoding.UTF8.GetBytes($"Factura {factura.Serie}{factura.Folio} - UUID: {factura.UUID}");
                }
                catch (Exception exPdf)
                {
                    return Json(new { success = false, mensaje = "Error al generar PDF: " + exPdf.Message });
                }

                // Obtener XML timbrado
                string xmlTimbrado = factura.XMLTimbrado;
                if (string.IsNullOrEmpty(xmlTimbrado))
                {
                    return Json(new { success = false, mensaje = "No se encontró el XML timbrado de la factura" });
                }

                // Enviar email (intenta configuración BD primero, luego Web.config)
                try
                {
                    string smtpHost = null;
                    int smtpPort = 587;
                    bool usarSSL = true;
                    string smtpUser = null;
                    string smtpPass = null;
                    string fromEmail = null;
                    string fromName = "Sistema de Facturación";

                    // 1. Intentar leer desde BD
                    var configBD = CD_ConfiguracionSMTP.Instancia.ObtenerConfiguracion();
                    if (configBD != null && configBD.Activo)
                    {
                        smtpHost = configBD.Host;
                        smtpPort = configBD.Puerto;
                        usarSSL = configBD.UsarSSL;
                        smtpUser = configBD.Usuario;
                        smtpPass = configBD.Contrasena;
                        fromEmail = configBD.EmailRemitente;
                        fromName = configBD.NombreRemitente;
                    }
                    else
                    {
                        // 2. Fallback a Web.config
                        smtpHost = System.Configuration.ConfigurationManager.AppSettings["SMTP_Host"];
                        string portStr = System.Configuration.ConfigurationManager.AppSettings["SMTP_Port"];
                        string sslStr = System.Configuration.ConfigurationManager.AppSettings["SMTP_SSL"];
                        smtpUser = System.Configuration.ConfigurationManager.AppSettings["SMTP_Username"];
                        smtpPass = System.Configuration.ConfigurationManager.AppSettings["SMTP_Password"];
                        fromEmail = System.Configuration.ConfigurationManager.AppSettings["SMTP_FromEmail"];
                        fromName = System.Configuration.ConfigurationManager.AppSettings["SMTP_FromName"] ?? "Sistema de Facturación";

                        if (!string.IsNullOrEmpty(portStr)) int.TryParse(portStr, out smtpPort);
                        if (!string.IsNullOrEmpty(sslStr)) bool.TryParse(sslStr, out usarSSL);
                    }

                    // Validar que exista configuración
                    if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                    {
                        return Json(new
                        {
                            success = false,
                            mensaje = "Configuración SMTP no encontrada. Configure las credenciales en Administración > Configuración SMTP"
                        });
                    }

                    using (var client = new System.Net.Mail.SmtpClient(smtpHost ?? "smtp.gmail.com", smtpPort))
                    {
                        client.EnableSsl = usarSSL;
                        client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);

                        using (var mensaje = new System.Net.Mail.MailMessage())
                        {
                            mensaje.From = new System.Net.Mail.MailAddress(fromEmail ?? smtpUser, fromName);
                            mensaje.To.Add(email);
                            mensaje.Subject = $"CFDI - Factura {factura.Serie}{factura.Folio}";
                            mensaje.Body = GenerarCuerpoEmailFactura(factura);
                            mensaje.IsBodyHtml = true;

                            // Adjuntar XML
                            var xmlBytes = System.Text.Encoding.UTF8.GetBytes(xmlTimbrado);
                            var xmlStream = new System.IO.MemoryStream(xmlBytes);
                            mensaje.Attachments.Add(new System.Net.Mail.Attachment(xmlStream, $"Factura_{factura.Serie}{factura.Folio}.xml", "application/xml"));

                            // Adjuntar PDF
                            var pdfStream = new System.IO.MemoryStream(pdfBytes);
                            mensaje.Attachments.Add(new System.Net.Mail.Attachment(pdfStream, $"Factura_{factura.Serie}{factura.Folio}.pdf", "application/pdf"));

                            client.Send(mensaje);
                        }
                    }

                    return Json(new
                    {
                        success = true,
                        mensaje = $"CFDI enviado exitosamente a {email}",
                        fechaEnvio = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    });
                }
                catch (Exception exEmail)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = $"Error al enviar email: {exEmail.Message}"
                    });
                }

                /* 
                // Código a implementar cuando exista EmailService:
                var emailService = new EmailService();

                // Validar configuración SMTP
                string mensajeConfig;
                if (!emailService.ValidarConfiguracion(out mensajeConfig))
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "Error de configuración SMTP: " + mensajeConfig
                    });
                }

                // Obtener nombre de empresa (podría venir de configuración)
                string nombreEmpresa = "Mi Empresa SA de CV"; // TODO: Obtener de ConfiguracionEmpresa

                // Crear request de email
                var request = new EnviarEmailRequest
                {
                    EmailDestinatario = email,
                    NombreDestinatario = factura.ReceptorNombre,
                    Asunto = $"CFDI - Factura {factura.Serie}{factura.Folio}",
                    CuerpoHTML = emailService.GenerarCuerpoFactura(factura, nombreEmpresa),
                    AdjuntoPDF = pdfBytes,
                    NombreArchivoPDF = $"Factura_{factura.Serie}{factura.Folio}_{factura.UUID.Substring(0, 8)}.pdf",
                    AdjuntoXML = xmlTimbrado,
                    NombreArchivoXML = $"Factura_{factura.Serie}{factura.Folio}_{factura.UUID.Substring(0, 8)}.xml"
                };

                // Enviar email
                var respuesta = emailService.EnviarEmail(request);

                // Registrar en log
                CD_EmailLog.Instancia.RegistrarEnvio(
                    "FACTURA",
                    (int)factura.FacturaID.GetHashCode(),
                    factura.UUID,
                    request,
                    respuesta,
                    usuario
                );

                if (respuesta.Exitoso)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = "Email enviado exitosamente a " + email,
                        fechaEnvio = respuesta.FechaEnvio.ToString("dd/MM/yyyy HH:mm:ss")
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = respuesta.Mensaje
                    });
                }
                */
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al enviar email: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Genera el cuerpo HTML del email para envío de factura
        /// </summary>
        private string GenerarCuerpoEmailFactura(Factura factura)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #0891B2; color: white; padding: 20px; text-align: center; }}
        .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        td {{ padding: 8px; border-bottom: 1px solid #ddd; }}
        .label {{ font-weight: bold; width: 40%; }}
        .highlight {{ background: #e0f2fe; padding: 10px; margin: 10px 0; border-left: 4px solid #0891B2; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Comprobante Fiscal Digital (CFDI)</h1>
        </div>
        <div class='content'>
            <p>Estimado(a) <strong>{factura.ReceptorNombre}</strong>,</p>
            <p>Se adjunta su Comprobante Fiscal Digital por Internet (CFDI) correspondiente a:</p>
            
            <div class='highlight'>
                <strong>Folio Fiscal (UUID):</strong><br>
                {factura.UUID}
            </div>

            <table>
                <tr>
                    <td class='label'>Serie y Folio:</td>
                    <td>{factura.Serie}{factura.Folio}</td>
                </tr>
                <tr>
                    <td class='label'>Fecha de Emisión:</td>
                    <td>{factura.FechaEmision:dd/MM/yyyy HH:mm}</td>
                </tr>
                <tr>
                    <td class='label'>RFC Receptor:</td>
                    <td>{factura.ReceptorRFC}</td>
                </tr>
                <tr>
                    <td class='label'>Método de Pago:</td>
                    <td>{(factura.MetodoPago == "PUE" ? "Pago en una sola exhibición" : "Pago en parcialidades")}</td>
                </tr>
                <tr>
                    <td class='label'>Subtotal:</td>
                    <td>${factura.Subtotal:N2}</td>
                </tr>
                <tr>
                    <td class='label'>IVA:</td>
                    <td>${factura.TotalImpuestosTrasladados:N2}</td>
                </tr>
                <tr>
                    <td class='label'><strong>Total:</strong></td>
                    <td><strong>${factura.MontoTotal:N2}</strong></td>
                </tr>
            </table>

            <p><strong>Archivos adjuntos:</strong></p>
            <ul>
                <li>📄 <strong>XML</strong> - Archivo fiscal para contabilidad</li>
                <li>📋 <strong>PDF</strong> - Representación impresa</li>
            </ul>

            <p style='color: #0891B2; font-weight: bold;'>
                Este comprobante ha sido timbrado ante el SAT y tiene plena validez fiscal.
            </p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no responder.</p>
            <p>&copy; {DateTime.Now.Year} - Sistema de Facturación Electrónica</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
