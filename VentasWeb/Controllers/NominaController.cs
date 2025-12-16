using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    [Authorize]
    public class NominaController : Controller
    {
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

        // =============================================
        // TIMBRADO CFDI NÓMINA
        // =============================================

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> TimbrarRecibo(int reciboId)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                string usuario = Session["Usuario"].ToString();

                // Timbrar el recibo
                var respuesta = await CD_Nomina.Instancia.TimbrarCFDINomina(reciboId, usuario);

                if (respuesta.Exitoso)
                {
                    return Json(new
                    {
                        success = true,
                        message = respuesta.Mensaje,
                        uuid = respuesta.UUID,
                        fechaTimbrado = respuesta.FechaTimbrado.HasValue ? respuesta.FechaTimbrado.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = respuesta.Mensaje,
                        codigoError = respuesta.CodigoError
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult DescargarXMLRecibo(int reciboId)
        {
            try
            {
                // TODO: Implementar descarga de XML timbrado desde NominasCFDI
                return Json(new { success = false, message = "Función en desarrollo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DescargarPDFRecibo(int reciboId)
        {
            try
            {
                // TODO: Implementar generación de PDF del recibo con iTextSharp
                TempData["Info"] = "Función de descarga PDF en desarrollo";
                return RedirectToAction("ReciboEmpleado", new { id = reciboId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
