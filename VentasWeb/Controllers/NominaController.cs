using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class NominaController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Bloqueo temporal de la funcionalidad de nómina usando appSettings
            var enabled = ConfigurationManager.AppSettings["NominaEnabled"];
            bool nominaEnabled = string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
            if (!nominaEnabled)
            {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Nomina
        public ActionResult Index()
        {
            return View();
        }

        // GET: Nomina/Calcular
        public ActionResult Calcular()
        {
            ViewBag.Sucursales = CD_Sucursal.Instancia.ObtenerSucursales();
            return View();
        }

        // GET: Nomina/Detalle/{id}
        public ActionResult Detalle(int id)
        {
            var nomina = CD_Nomina.Instancia.ObtenerPorId(id);
            if (nomina == null)
            {
                TempData["Error"] = "Nómina no encontrada";
                return RedirectToAction("Index");
            }
            return View(nomina);
        }

        // GET: Nomina/ReciboEmpleado/{id}
        public ActionResult ReciboEmpleado(int id)
        {
            // Buscar el recibo por ID
            NominaDetalle recibo = null;
            var todasLasNominas = CD_Nomina.Instancia.ObtenerTodas();
            
            foreach (var nomina in todasLasNominas)
            {
                var nominaCompleta = CD_Nomina.Instancia.ObtenerPorId(nomina.NominaID);
                if (nominaCompleta?.Recibos != null)
                {
                    recibo = nominaCompleta.Recibos.FirstOrDefault(r => r.NominaDetalleID == id);
                    if (recibo != null)
                    {
                        // Recibo encontrado
                        break;
                    }
                }
            }

            if (recibo == null)
            {
                TempData["Error"] = "Recibo no encontrado";
                return RedirectToAction("Index");
            }
            
            // Cargar percepciones y deducciones
            var percepciones = CD_Nomina.Instancia.ObtenerPercepciones(id);
            var deducciones = CD_Nomina.Instancia.ObtenerDeducciones(id);
            
            ViewBag.Percepciones = percepciones;
            ViewBag.Deducciones = deducciones;
            
            return View(recibo);
        }

        // =============================================
        // API METHODS (JSON)
        // =============================================

        [HttpGet]
        public JsonResult ObtenerNominas()
        {
            try
            {
                var nominas = CD_Nomina.Instancia.ObtenerTodas();
                return Json(new { data = nominas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerNominasPorPeriodo(string periodo)
        {
            try
            {
                var nominas = CD_Nomina.Instancia.ObtenerPorPeriodo(periodo);
                return Json(new { data = nominas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerDetalleNomina(int nominaId)
        {
            try
            {
                var nomina = CD_Nomina.Instancia.ObtenerPorId(nominaId);
                if (nomina == null)
                    return Json(new { success = false, message = "Nómina no encontrada" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = nomina }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerRecibos(int nominaId)
        {
            try
            {
                var recibos = CD_Nomina.Instancia.ObtenerRecibos(nominaId);
                return Json(new { data = recibos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ProcesarNomina(string fechaInicio, string fechaFin, string fechaPago, string tipoNomina)
        {
            try
            {
                if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin) || string.IsNullOrEmpty(fechaPago))
                    return Json(new { success = false, message = "Fechas requeridas" });

                DateTime dtInicio = DateTime.Parse(fechaInicio);
                DateTime dtFin = DateTime.Parse(fechaFin);
                DateTime dtPago = DateTime.Parse(fechaPago);

                if (dtInicio > dtFin)
                    return Json(new { success = false, message = "Fecha de inicio no puede ser mayor a fecha fin" });

                if (dtPago < dtFin)
                    return Json(new { success = false, message = "Fecha de pago debe ser posterior a fecha fin del periodo" });

                string usuario = User.Identity.Name ?? "Sistema";
                string tipo = string.IsNullOrEmpty(tipoNomina) ? "ORDINARIA" : tipoNomina;

                var nomina = CD_Nomina.Instancia.CalcularNomina(dtInicio, dtFin, dtPago, tipo, usuario);

                return Json(new
                {
                    success = true,
                    message = $"Nómina calculada exitosamente. {nomina.NumeroEmpleados} empleados procesados.",
                    data = new
                    {
                        nominaId = nomina.NominaID,
                        folio = nomina.Folio,
                        periodo = nomina.Periodo,
                        numEmpleados = nomina.NumeroEmpleados,
                        totalPercepciones = nomina.TotalPercepciones,
                        totalDeducciones = nomina.TotalDeducciones,
                        totalNeto = nomina.TotalNeto
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al calcular nómina: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult ContabilizarNomina(int nominaId)
        {
            try
            {
                var nomina = CD_Nomina.Instancia.ObtenerPorId(nominaId);
                if (nomina == null)
                    return Json(new { success = false, message = "Nómina no encontrada" });

                if (nomina.PolizaID.HasValue)
                    return Json(new { success = false, message = "Esta nómina ya fue contabilizada" });

                if (nomina.Estatus == "CANCELADA")
                    return Json(new { success = false, message = "No se puede contabilizar una nómina cancelada" });

                string usuario = User.Identity.Name ?? "Sistema";
                bool resultado = CD_Nomina.Instancia.GenerarPolizaNomina(nominaId, usuario);

                if (resultado)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Nómina contabilizada exitosamente. Se generó la póliza contable automáticamente."
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Error al generar póliza contable" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al contabilizar: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult MarcarComoPagada(int nominaId, string fechaPago)
        {
            try
            {
                var nomina = CD_Nomina.Instancia.ObtenerPorId(nominaId);
                if (nomina == null)
                    return Json(new { success = false, message = "Nómina no encontrada" });

                if (nomina.Estatus == "PAGADA")
                    return Json(new { success = false, message = "Esta nómina ya fue marcada como pagada" });

                DateTime dtPago = DateTime.Parse(fechaPago);

                // Actualizar estatus a PAGADA
                using (var conn = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    string query = @"UPDATE Nominas SET Estatus = 'PAGADA', FechaPagado = @FechaPagado WHERE NominaID = @NominaID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NominaID", nominaId);
                        cmd.Parameters.AddWithValue("@FechaPagado", dtPago);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Nómina marcada como pagada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult CancelarNomina(int nominaId, string motivo)
        {
            try
            {
                var nomina = CD_Nomina.Instancia.ObtenerPorId(nominaId);
                if (nomina == null)
                    return Json(new { success = false, message = "Nómina no encontrada" });

                if (nomina.Estatus == "CANCELADA")
                    return Json(new { success = false, message = "Esta nómina ya está cancelada" });

                if (nomina.Estatus == "PAGADA")
                    return Json(new { success = false, message = "No se puede cancelar una nómina que ya fue pagada" });

                if (nomina.PolizaID.HasValue)
                    return Json(new { success = false, message = "No se puede cancelar una nómina contabilizada. Cancele primero la póliza." });

                // Cancelar nómina
                using (var conn = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    string query = @"UPDATE Nominas SET Estatus = 'CANCELADA' WHERE NominaID = @NominaID";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NominaID", nominaId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Nómina cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // =============================================
        // MÉTODOS AUXILIARES
        // =============================================

        [HttpGet]
        public JsonResult ObtenerEmpleadosActivos()
        {
            try
            {
                var empleados = CD_Empleado.Instancia.ObtenerActivos();
                return Json(new
                {
                    success = true,
                    data = empleados.Select(e => new
                    {
                        empleadoId = e.EmpleadoID,
                        numeroEmpleado = e.NumeroEmpleado,
                        nombreCompleto = e.NombreCompleto,
                        puesto = e.Puesto,
                        salarioDiario = e.SalarioDiario,
                        periodicidadPago = e.PeriodicidadPago
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ValidarPeriodo(string fechaInicio, string fechaFin)
        {
            try
            {
                DateTime dtInicio = DateTime.Parse(fechaInicio);
                DateTime dtFin = DateTime.Parse(fechaFin);

                string periodo = dtInicio.Day <= 15 ? $"{dtInicio:yyyy-MM}-Q1" : $"{dtInicio:yyyy-MM}-Q2";

                // Verificar si ya existe nómina para este periodo
                var nominasExistentes = CD_Nomina.Instancia.ObtenerPorPeriodo(periodo);
                bool yaExiste = nominasExistentes.Any(n => n.Estatus != "CANCELADA");

                return Json(new
                {
                    success = true,
                    periodo = periodo,
                    yaExiste = yaExiste,
                    mensaje = yaExiste ? $"Ya existe una nómina para el periodo {periodo}" : "Periodo disponible"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // EXPORTACIÓN Y REPORTES
        // =============================================

        public ActionResult ExportarReciboPDF(int reciboId)
        {
            try
            {
                // TODO: Implementar generación de PDF con iTextSharp o similar
                TempData["Info"] = "Función de exportación a PDF en desarrollo";
                return RedirectToAction("ReciboEmpleado", new { id = reciboId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult ExportarNominaExcel(int nominaId)
        {
            try
            {
                // TODO: Implementar exportación a Excel con EPPlus o similar
                TempData["Info"] = "Función de exportación a Excel en desarrollo";
                return RedirectToAction("Detalle", new { id = nominaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Nomina/Procesar
        public ActionResult Procesar()
        {
            // Redirigir a Calcular que es la vista de procesamiento
            return RedirectToAction("Calcular");
        }

        // GET: Nomina/Consultar
        public ActionResult Consultar()
        {
            // Redirigir a Index que muestra el listado de nóminas
            return RedirectToAction("Index");
        }

        // GET: Nomina/Reportes
        public ActionResult Reportes()
        {
            return View();
        }

        // GET: API para obtener datos de reportes con filtros
        [HttpGet]
        public JsonResult ObtenerDatosReportes(string fechaInicio, string fechaFin, string departamento, string puesto)
        {
            try
            {
                var nominas = CD_Nomina.Instancia.ObtenerTodas();
                var reportes = new List<object>();

                foreach (var nomina in nominas)
                {
                    var nominaCompleta = CD_Nomina.Instancia.ObtenerPorId(nomina.NominaID);
                    if (nominaCompleta?.Recibos != null)
                    {
                        foreach (var recibo in nominaCompleta.Recibos)
                        {
                            // Aplicar filtros (simulado - en producción se filtrarían en la consulta SQL)
                            reportes.Add(new
                            {
                                Folio = nomina.Folio,
                                Periodo = nomina.Periodo,
                                Departamento = "Ventas", // Obtener del empleado cuando esté implementado
                                Puesto = "Vendedor", // Obtener del empleado cuando esté implementado
                                Empleado = recibo.NombreEmpleado,
                                Percepciones = recibo.TotalPercepciones,
                                Deducciones = recibo.TotalDeducciones,
                                Neto = recibo.NetoPagar
                            });
                        }
                    }
                }

                return Json(new { success = true, data = reportes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: API para obtener estadísticas
        [HttpGet]
        public JsonResult ObtenerEstadisticas(string fechaInicio, string fechaFin, string departamento, string puesto)
        {
            try
            {
                var nominas = CD_Nomina.Instancia.ObtenerTodas();
                decimal totalPercepciones = 0;
                decimal totalDeducciones = 0;
                decimal totalNeto = 0;
                int totalEmpleados = 0;

                foreach (var nomina in nominas)
                {
                    var nominaCompleta = CD_Nomina.Instancia.ObtenerPorId(nomina.NominaID);
                    if (nominaCompleta?.Recibos != null)
                    {
                        totalPercepciones += nominaCompleta.TotalPercepciones;
                        totalDeducciones += nominaCompleta.TotalDeducciones;
                        totalNeto += nominaCompleta.TotalNeto;
                        totalEmpleados += nominaCompleta.Recibos.Count;
                    }
                }

                return Json(new
                {
                    success = true,
                    totalPercepciones = totalPercepciones,
                    totalDeducciones = totalDeducciones,
                    totalNeto = totalNeto,
                    totalEmpleados = totalEmpleados
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: API para obtener datos de gráfica de evolución
        [HttpGet]
        public JsonResult ObtenerEvolucionMensual()
        {
            try
            {
                var nominas = CD_Nomina.Instancia.ObtenerTodas();
                var datosEvolucion = nominas
                    .OrderBy(n => n.FechaPago)
                    .Select(n => new
                    {
                        Periodo = n.Periodo,
                        Percepciones = n.TotalPercepciones,
                        Deducciones = n.TotalDeducciones,
                        Neto = n.TotalNeto
                    })
                    .ToList();

                return Json(new { success = true, data = datosEvolucion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: API para obtener distribución por departamento
        [HttpGet]
        public JsonResult ObtenerDistribucionDepartamentos()
        {
            try
            {
                // Cuando se implementen empleados con departamento, se obtendrá de la BD
                var distribucion = new List<object>
                {
                    new { Departamento = "Ventas", Total = 0 },
                    new { Departamento = "Administración", Total = 0 },
                    new { Departamento = "Contabilidad", Total = 0 },
                    new { Departamento = "Almacén", Total = 0 },
                    new { Departamento = "Sistemas", Total = 0 }
                };

                return Json(new { success = true, data = distribucion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // MÉTODOS DE TIMBRADO DE NÓMINA (CFDI 4.0)
        // =============================================

        /// <summary>
        /// Timbra un recibo de nómina individual usando FiscalAPI
        /// Genera CFDI 4.0 con Complemento de Nómina 1.2
        /// </summary>
        /// <param name="nominaDetalleID">ID del recibo a timbrar</param>
        /// <returns>JSON con resultado del timbrado (UUID, fecha, mensaje)</returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> TimbrarRecibo(int nominaDetalleID)
        {
            try
            {
                if (nominaDetalleID <= 0)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = "ID de recibo inválido"
                    });
                }

                // Obtener usuario actual
                string usuario = User.Identity.Name ?? "Sistema";

                // Llamar al método de timbrado de nómina
                var resultado = await CD_Nomina.Instancia.TimbrarCFDINomina(nominaDetalleID, usuario);

                // Retornar resultado en formato JSON
                return Json(new
                {
                    exitoso = resultado.Exitoso,
                    mensaje = resultado.Mensaje,
                    uuid = resultado.UUID,
                    fechaTimbrado = resultado.FechaTimbrado.HasValue ? resultado.FechaTimbrado.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    codigoError = resultado.CodigoError,
                    selloCFD = !string.IsNullOrEmpty(resultado.SelloCFD) ? resultado.SelloCFD.Substring(0, Math.Min(20, resultado.SelloCFD.Length)) + "..." : null,
                    selloSAT = !string.IsNullOrEmpty(resultado.SelloSAT) ? resultado.SelloSAT.Substring(0, Math.Min(20, resultado.SelloSAT.Length)) + "..." : null,
                    invoiceId = resultado.InvoiceId
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en TimbrarRecibo: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    exitoso = false,
                    mensaje = $"Excepción en controlador: {ex.Message}",
                    codigoError = "EXCEPTION"
                });
            }
        }

        /// <summary>
        /// Descarga el PDF de un recibo de nómina timbrado desde FiscalAPI
        /// </summary>
        /// <param name="nominaDetalleID">ID del recibo timbrado</param>
        /// <returns>Archivo PDF del recibo</returns>
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> DescargarPDFRecibo(int nominaDetalleID)
        {
            try
            {
                if (nominaDetalleID <= 0)
                {
                    TempData["Error"] = "ID de recibo inválido";
                    return RedirectToAction("Index");
                }

                // Obtener el InvoiceId del recibo timbrado
                string invoiceId = CD_Nomina.Instancia.ObtenerInvoiceIdRecibo(nominaDetalleID);

                if (string.IsNullOrEmpty(invoiceId))
                {
                    TempData["Error"] = "Este recibo no ha sido timbrado o no tiene InvoiceId";
                    return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
                }

                // Obtener configuración de FiscalAPI desde la base de datos
                // Reutiliza la misma configuración que la facturación de ventas
                var config = CD_Factura.Instancia.ObtenerConfiguracionFiscalAPI();

                if (config == null)
                {
                    TempData["Error"] = "No se encontró configuración de FiscalAPI activa";
                    return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
                }

                // Descargar PDF desde FiscalAPI
                using (var fiscalAPIService = new CapaDatos.PAC.FiscalAPIService(config))
                {
                    byte[] pdfBytes = await fiscalAPIService.DescargarPDF(invoiceId);

                    if (pdfBytes == null || pdfBytes.Length == 0)
                    {
                        TempData["Error"] = "No se pudo descargar el PDF desde FiscalAPI";
                        return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
                    }

                    // Generar nombre de archivo
                    var recibo = CD_Nomina.Instancia.ObtenerReciboPorId(nominaDetalleID);
                    string nombreArchivo = $"Recibo_Nomina_{recibo?.NumeroEmpleado ?? nominaDetalleID.ToString()}.pdf";

                    System.Diagnostics.Debug.WriteLine($"✓ PDF descargado - Tamaño: {pdfBytes.Length} bytes");

                    return File(pdfBytes, "application/pdf", nombreArchivo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en DescargarPDFRecibo: {ex.Message}");
                TempData["Error"] = $"Error al descargar PDF: {ex.Message}";
                return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
            }
        }

        /// <summary>
        /// Descarga el XML timbrado de un recibo de nómina
        /// </summary>
        /// <param name="nominaDetalleID">ID del recibo timbrado</param>
        /// <returns>Archivo XML timbrado</returns>
        [HttpGet]
        public ActionResult DescargarXMLRecibo(int nominaDetalleID)
        {
            try
            {
                if (nominaDetalleID <= 0)
                {
                    TempData["Error"] = "ID de recibo inválido";
                    return RedirectToAction("Index");
                }

                // Obtener el XML timbrado del recibo
                string xmlTimbrado = CD_Nomina.Instancia.ObtenerXMLTimbradoRecibo(nominaDetalleID);

                if (string.IsNullOrEmpty(xmlTimbrado))
                {
                    TempData["Error"] = "Este recibo no ha sido timbrado o no tiene XML disponible";
                    return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
                }

                // Generar nombre de archivo
                var recibo = CD_Nomina.Instancia.ObtenerReciboPorId(nominaDetalleID);
                string nombreArchivo = $"Recibo_Nomina_{recibo?.NumeroEmpleado ?? nominaDetalleID.ToString()}.xml";

                // Convertir XML a bytes UTF-8
                byte[] xmlBytes = System.Text.Encoding.UTF8.GetBytes(xmlTimbrado);

                System.Diagnostics.Debug.WriteLine($"✓ XML descargado - Tamaño: {xmlBytes.Length} bytes");

                return File(xmlBytes, "text/xml", nombreArchivo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en DescargarXMLRecibo: {ex.Message}");
                TempData["Error"] = $"Error al descargar XML: {ex.Message}";
                return RedirectToAction("ReciboEmpleado", new { id = nominaDetalleID });
            }
        }
    }
}
