using System;
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

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        ventaId = venta.VentaID.ToString(),
                        clienteRFC = cliente?.RFC ?? "XAXX010101000",
                        clienteNombre = venta.RazonSocial,
                        total = venta.Total,
                        subtotal = venta.Total / 1.16m, // Aproximación
                        iva = venta.Total - (venta.Total / 1.16m),
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
                var factura = CD_Factura.Instancia.ObtenerPorUUID(facturaId.ToString(), out string mensaje);

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
                var factura = CD_Factura.Instancia.ObtenerPorUUID(facturaId.ToString(), out string mensaje);

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
                // TODO: Obtener de base de datos CatUsosCFDI
                var usos = new[]
                {
                    new { Clave = "G01", Descripcion = "Adquisición de mercancías" },
                    new { Clave = "G02", Descripcion = "Devoluciones, descuentos o bonificaciones" },
                    new { Clave = "G03", Descripcion = "Gastos en general" },
                    new { Clave = "I01", Descripcion = "Construcciones" },
                    new { Clave = "I02", Descripcion = "Mobiliario y equipo de oficina por inversiones" },
                    new { Clave = "P01", Descripcion = "Por definir" }
                };

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
        public async Task<JsonResult> CancelarFactura(int facturaId, string motivo, string uuidSustitucion)
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
                if (facturaId <= 0)
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
                var respuesta = await CD_Factura.Instancia.CancelarCFDI(facturaId, motivo, uuidSustitucion, usuario);

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
                string mensaje;
                var factura = CD_Factura.Instancia.ObtenerPorUUID(facturaGuid.ToString(), out mensaje);

                if (factura == null)
                {
                    return Json(new { success = false, mensaje = "Factura no encontrada: " + mensaje });
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

                // TODO: Implementar servicio de email completo
                return Json(new { success = false, mensaje = "Funcionalidad de envío por email no implementada aún. Por favor descargue manualmente la factura." });

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
    }
}
