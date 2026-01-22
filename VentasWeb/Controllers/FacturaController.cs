using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using VentasWeb.Utilidades;
using Newtonsoft.Json;

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
                        clienteCP = cliente?.CodigoPostal ?? "00000",
                        clienteRegimenFiscal = cliente?.RegimenFiscal ?? "616",
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

                // Obtener email del cliente desde la venta
                string emailCliente = "";
                if (factura.VentaID.HasValue)
                {
                    var venta = CD_Venta.Instancia.ObtenerDetalle(factura.VentaID.Value);
                    if (venta != null && venta.ClienteID != Guid.Empty)
                    {
                        var cliente = CD_Cliente.Instancia.ObtenerPorId(venta.ClienteID);
                        emailCliente = cliente?.CorreoElectronico ?? "";
                    }
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
                        EmailCliente = emailCliente,
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
        public async Task<JsonResult> GenerarFactura()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("=== GenerarFactura Controller INICIO ===");
                System.Diagnostics.Debug.WriteLine("========================================");
                
                // Leer el body del request
                string requestBody = null;
                Request.InputStream.Position = 0;
                using (var reader = new System.IO.StreamReader(Request.InputStream, System.Text.Encoding.UTF8))
                {
                    requestBody = reader.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($"Request Body recibido ({requestBody?.Length ?? 0} caracteres)");
                    System.Diagnostics.Debug.WriteLine($"Contenido: {requestBody}");
                }
                
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: Request body está vacío");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = "Error: No se recibió información en el request. El body está vacío."
                    });
                }
                
                // Deserializar el JSON
                CapaModelo.GenerarFacturaRequest request = null;
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Deserializando JSON...");
                    
                    var settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    
                    request = JsonConvert.DeserializeObject<CapaModelo.GenerarFacturaRequest>(requestBody, settings);
                    
                    if (request == null)
                    {
                        System.Diagnostics.Debug.WriteLine("❌ ERROR: DeserializeObject devolvió null");
                        Response.StatusCode = 400;
                        return Json(new
                        {
                            estado = false,
                            valor = "Error: No se pudo deserializar el JSON recibido."
                        });
                    }
                    
                    System.Diagnostics.Debug.WriteLine("✅ Deserialización exitosa");
                    System.Diagnostics.Debug.WriteLine($"   VentaID: {request.VentaID}");
                    System.Diagnostics.Debug.WriteLine($"   ReceptorRFC: {request.ReceptorRFC}");
                    System.Diagnostics.Debug.WriteLine($"   ReceptorNombre: {request.ReceptorNombre}");
                }
                catch (JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR JSON al deserializar: {jsonEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"   InnerException: {jsonEx.InnerException?.Message}");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = $"Error al parsear JSON: {jsonEx.Message}"
                    });
                }
                catch (Exception deserEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR al deserializar: {deserEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"   StackTrace: {deserEx.StackTrace}");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = $"Error al deserializar: {deserEx.Message}"
                    });
                }
                
                // Validar VentaID
                if (request.VentaID == Guid.Empty)
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: VentaID es Guid.Empty");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = "VentaID es requerido y debe ser un GUID válido"
                    });
                }

                // Validar que la venta no tenga ya una factura timbrada
                var facturaExistente = CD_Factura.Instancia.ObtenerPorVentaID(request.VentaID);
                if (facturaExistente != null && facturaExistente.Estatus == "TIMBRADA")
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR: La venta {request.VentaID} ya tiene una factura timbrada: {facturaExistente.Serie}{facturaExistente.Folio}");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = $"Esta venta ya tiene una factura timbrada: {facturaExistente.Serie}{facturaExistente.Folio} (UUID: {facturaExistente.UUID})"
                    });
                }

                // Validar ReceptorRFC
                if (string.IsNullOrWhiteSpace(request.ReceptorRFC))
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: ReceptorRFC es vacío");
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        estado = false,
                        valor = "ReceptorRFC es requerido"
                    });
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Validación exitosa:");
                
                System.Diagnostics.Debug.WriteLine($"✅ Validación exitosa:");
                System.Diagnostics.Debug.WriteLine($"  VentaID: {request.VentaID}");
                System.Diagnostics.Debug.WriteLine($"  ReceptorRFC: {request.ReceptorRFC}");
                System.Diagnostics.Debug.WriteLine($"  ReceptorNombre: {request.ReceptorNombre}");
                
                // ============================================================
                // CORRECCIÓN AUTOMÁTICA: Régimen 616 + UsoCFDI
                // ============================================================
                // Régimen 616 (Sin obligaciones fiscales) - RFC genérico XAXX010101000
                // Para PÚBLICO EN GENERAL, usar G01 o G03 (compras/gastos)
                if (request.ReceptorRegimenFiscal == "616" && request.ReceptorRFC == "XAXX010101000")
                {
                    // Para público en general con régimen 616, G01 y G03 son válidos
                    if (request.UsoCFDI != "G01" && request.UsoCFDI != "G03" && 
                        request.UsoCFDI != "CP01" && request.UsoCFDI != "CN01")
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ CORRECCIÓN: UsoCFDI '{request.UsoCFDI}' ajustado para régimen 616");
                        System.Diagnostics.Debug.WriteLine($"   Cambiando automáticamente a: G03 (Gastos en general)");
                        request.UsoCFDI = "G03";
                    }
                }
                
                // Obtener usuario de sesión
                string usuario = Session["UsuarioNombre"]?.ToString() ?? "Sistema";
                System.Diagnostics.Debug.WriteLine($"Usuario de sesión: {usuario}");

                // Generar y timbrar factura con FiscalAPI
                System.Diagnostics.Debug.WriteLine("Llamando a GenerarYTimbrarFactura con FiscalAPI...");
                
                var respuestaTimbrado = await CD_Factura.Instancia.GenerarYTimbrarFactura(
                    request.VentaID,
                    request.ReceptorRFC,
                    request.ReceptorNombre,
                    request.ReceptorCP,
                    request.ReceptorRegimenFiscal,
                    request.UsoCFDI,
                    request.FormaPago,
                    request.MetodoPago,
                    "A", // Serie por defecto
                    usuario
                );

                // Construir respuesta
                var respuesta = new Respuesta<object>();
                
                if (respuestaTimbrado.Exitoso)
                {
                    respuesta.estado = true;
                    respuesta.valor = respuestaTimbrado.Mensaje;
                    respuesta.objeto = new
                    {
                        UUID = respuestaTimbrado.UUID,
                        FechaTimbrado = respuestaTimbrado.FechaTimbrado,
                        XMLBase64 = !string.IsNullOrEmpty(respuestaTimbrado.XMLTimbrado)
                            ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(respuestaTimbrado.XMLTimbrado))
                            : null
                    };
                }
                else
                {
                    respuesta.estado = false;
                    respuesta.valor = respuestaTimbrado.Mensaje;
                    respuesta.objeto = new
                    {
                        CodigoError = respuestaTimbrado.CodigoError,
                        Mensaje = respuestaTimbrado.Mensaje
                    };
                }

                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("=== GenerarFactura Controller FIN ===");
                System.Diagnostics.Debug.WriteLine($"Estado: {respuesta.estado}");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {respuesta.valor}");
                System.Diagnostics.Debug.WriteLine("========================================");

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

                // Establecer código HTTP según el resultado
                Response.StatusCode = respuesta.estado ? 200 : 400;
                Response.ContentType = "application/json; charset=utf-8";

                return Json(new
                {
                    estado = respuesta.estado,
                    mensaje = respuesta.valor,
                    objeto = respuesta.objeto
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR GENERAL en GenerarFactura Controller:");
                System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   InnerException: {ex.InnerException.Message}");
                }
                
                Response.StatusCode = 500;
                Response.ContentType = "application/json; charset=utf-8";
                
                return Json(new
                {
                    estado = false,
                    valor = $"Error inesperado: {ex.Message}"
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
        public ActionResult DescargarXML(Guid? facturaId = null, Guid? ventaId = null)
        {
            try
            {
                Factura factura = null;

                // Buscar por facturaId o ventaId
                if (facturaId.HasValue)
                {
                    factura = CD_Factura.Instancia.ObtenerPorId(facturaId.Value);
                }
                else if (ventaId.HasValue)
                {
                    factura = CD_Factura.Instancia.ObtenerPorVentaID(ventaId.Value);
                    
                    if (factura == null)
                    {
                        // Verificar si la venta existe y está marcada como facturada
                        var venta = CD_Venta.Instancia.ObtenerDetalle(ventaId.Value);
                        if (venta != null)
                        {
                            return Content($"La venta existe pero no tiene factura asociada. VentaID: {ventaId.Value}");
                        }
                        else
                        {
                            return Content($"No se encontró la venta con ID: {ventaId.Value}");
                        }
                    }
                }
                else
                {
                    return Content("Debe proporcionar facturaId o ventaId");
                }

                if (factura == null)
                {
                    return Content("No se encontró la factura");
                }

                if (string.IsNullOrEmpty(factura.XMLTimbrado))
                {
                    return Content($"La factura {factura.Serie}-{factura.Folio} no tiene XML timbrado. Estatus: {factura.Estatus}");
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
        public async Task<ActionResult> DescargarPDF(Guid? facturaId = null, Guid? ventaId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DescargarPDF INICIO ===");
                System.Diagnostics.Debug.WriteLine($"FacturaID: {facturaId}, VentaID: {ventaId}");
                
                Factura factura = null;

                // Buscar por facturaId o ventaId
                if (facturaId.HasValue)
                {
                    factura = CD_Factura.Instancia.ObtenerPorId(facturaId.Value);
                }
                else if (ventaId.HasValue)
                {
                    factura = CD_Factura.Instancia.ObtenerPorVentaID(ventaId.Value);
                }

                if (factura == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Factura no encontrada");
                    return Content("No se encontró la factura");
                }

                System.Diagnostics.Debug.WriteLine($"Factura cargada: {factura.Serie}-{factura.Folio}");
                System.Diagnostics.Debug.WriteLine($"UUID: {factura.UUID}");
                System.Diagnostics.Debug.WriteLine($"FiscalAPIInvoiceId: {factura.FiscalAPIInvoiceId ?? "NULL"}");

                byte[] pdfBytes;
                
                // Si tiene InvoiceId de FiscalAPI, descargar PDF oficial
                if (!string.IsNullOrEmpty(factura.FiscalAPIInvoiceId))
                {
                    System.Diagnostics.Debug.WriteLine("✅ Usando FiscalAPI PDF (oficial)");
                    System.Diagnostics.Debug.WriteLine($"InvoiceId: {factura.FiscalAPIInvoiceId}");
                    
                    var config = CD_Factura.Instancia.ObtenerConfiguracionFiscalAPI();
                    using (var fiscalService = new CapaDatos.PAC.FiscalAPIService(config))
                    {
                        pdfBytes = await fiscalService.DescargarPDF(factura.FiscalAPIInvoiceId);
                        System.Diagnostics.Debug.WriteLine($"PDF descargado: {pdfBytes.Length} bytes");
                        
                        // Validar que los bytes son un PDF válido
                        if (pdfBytes != null && pdfBytes.Length > 4)
                        {
                            string header = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
                            System.Diagnostics.Debug.WriteLine($"PDF Header: {header}");
                            if (header != "%PDF")
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ ADVERTENCIA: No es un PDF válido. Primeros bytes: {BitConverter.ToString(pdfBytes, 0, Math.Min(20, pdfBytes.Length))}");
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Usando generador local (iTextSharp)");
                    // Fallback: usar generador local si no tiene InvoiceId
                    var generador = new CapaDatos.Generadores.PDFFacturaGenerator();
                    pdfBytes = generador.GenerarPDF(factura);
                    System.Diagnostics.Debug.WriteLine($"PDF generado localmente: {pdfBytes.Length} bytes");
                }
                
                string nombreArchivo = string.IsNullOrEmpty(factura.UUID)
                    ? $"Factura_{factura.Serie}{factura.Folio}.pdf"
                    : $"Factura_{factura.Serie}{factura.Folio}_{factura.UUID.Substring(0, 8)}.pdf";

                System.Diagnostics.Debug.WriteLine($"Archivo: {nombreArchivo}");
                System.Diagnostics.Debug.WriteLine($"Bytes totales a enviar: {pdfBytes.Length}");
                System.Diagnostics.Debug.WriteLine("=== DescargarPDF FIN ===");

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{nombreArchivo}\"");
                Response.BinaryWrite(pdfBytes);
                Response.Flush();
                Response.End();
                
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR en DescargarPDF: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
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

                // Obtener factura para obtener UUID
                var factura = CD_Factura.Instancia.ObtenerPorId(facturaId);
                if (factura == null)
                {
                    return Json(new { success = false, mensaje = "Factura no encontrada" });
                }

                // Cancelar CFDI con FiscalAPI
                var respuesta = await CD_Factura.Instancia.CancelarCFDI(
                    factura.UUID,
                    motivo,
                    uuidSustitucion,
                    usuario
                );

                if (respuesta.Exitoso)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = respuesta.Mensaje,
                        estatusCancelacion = respuesta.EstatusCancelacion,
                        fechaCancelacion = respuesta.FechaCancelacion?.ToString("dd/MM/yyyy HH:mm:ss")
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
        public async Task<JsonResult> EnviarPorEmail(string facturaId, string email)
        {
            try
            {
                // Validar sesión
                if (Session["Usuario"] == null)
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

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

                // Verificar que tenga FiscalAPIInvoiceId
                if (string.IsNullOrEmpty(factura.FiscalAPIInvoiceId))
                {
                    return Json(new { success = false, mensaje = "La factura no fue timbrada con FiscalAPI" });
                }

                // Enviar usando FiscalAPI
                var configFiscal = CD_Factura.Instancia.ObtenerConfiguracionFiscalAPI();
                using (var fiscalService = new CapaDatos.PAC.FiscalAPIService(configFiscal))
                {
                    await fiscalService.EnviarPorCorreo(factura.FiscalAPIInvoiceId, email);
                }

                return Json(new
                {
                    success = true,
                    mensaje = $"CFDI enviado exitosamente a {email}",
                    fechaEnvio = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                });
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
        /// Consulta el estatus de un CFDI en el SAT
        /// </summary>
        [HttpPost]
        public JsonResult ConsultarEstatusSAT(string uuid)
        {
            try
            {
                var respuesta = CD_Factura.Instancia.ConsultarEstatusSAT(uuid);

                // Mapear estatus a texto amigable en español
                string estatusTexto = respuesta.EstatusSAT;
                if (!string.IsNullOrEmpty(estatusTexto))
                {
                    switch (estatusTexto.ToLower())
                    {
                        case "active":
                            estatusTexto = "Vigente";
                            break;
                        case "cancelled":
                            estatusTexto = "Cancelado";
                            break;
                        case "not_found":
                            estatusTexto = "No encontrado";
                            break;
                    }
                }

                return Json(new
                {
                    success = respuesta.Exitoso,
                    mensaje = respuesta.Mensaje,
                    estatus = estatusTexto,
                    estatusOriginal = respuesta.EstatusSAT,
                    codigoError = respuesta.CodigoError,
                    errorTecnico = respuesta.ErrorTecnico
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al consultar estatus: " + ex.Message
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
