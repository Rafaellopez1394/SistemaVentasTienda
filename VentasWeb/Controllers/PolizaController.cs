using System;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class PolizaController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var enabled = ConfigurationManager.AppSettings["PolizaEnabled"];
            if (string.Equals(enabled, "false", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Poliza
        public ActionResult Index()
        {
            return View();
        }

        // GET: Consultar Pólizas
        public ActionResult Consultar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Crear(Poliza poliza)
        {
            try
            {
                poliza.Usuario = User.Identity.Name ?? "system";
                bool resultado = CD_Poliza.Instancia.CrearPoliza(poliza);
                return Json(new { resultado });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Obtener(int top = 100)
        {
            var lista = CD_Poliza.Instancia.ObtenerUltimas(top);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerFiltradas(string fechaInicio = null, string fechaFin = null, string tipoPoliza = "")
        {
            var lista = CD_Poliza.Instancia.ObtenerFiltradas(fechaInicio, fechaFin, tipoPoliza);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerDetalle(Guid polizaId)
        {
            var poliza = CD_Poliza.Instancia.ObtenerPorId(polizaId);
            var detalles = CD_Poliza.Instancia.ObtenerDetalles(polizaId);
            return Json(new { poliza, detalles }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenerarPolizaNomina(string fechaInicio, string fechaFin)
        {
            try
            {
                // Validar fechas
                if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
                {
                    return Json(new { resultado = false, mensaje = "Las fechas son requeridas" });
                }

                DateTime dtInicio = DateTime.Parse(fechaInicio);
                DateTime dtFin = DateTime.Parse(fechaFin);

                // Obtener configuración de cuentas para nómina
                var configuracionCuentas = CD_CuentaContable.Instancia.ObtenerConfiguracionNomina();

                // Obtener datos detallados de nómina del periodo
                var datosNomina = CD_Reportes.Instancia.ObtenerResumenNominaContable(dtInicio, dtFin);

                if (datosNomina == null || datosNomina.TotalPercepciones == 0)
                {
                    return Json(new { resultado = false, mensaje = "No hay datos de nómina en el periodo seleccionado" });
                }

                // Crear la póliza con estructura profesional Factoraje
                var poliza = new Poliza
                {
                    PolizaID = Guid.NewGuid(),
                    TipoPoliza = "NM", // NM = Nómina
                    FechaPoliza = dtFin,
                    Concepto = $"Póliza de Nómina del {dtInicio:dd/MM/yyyy} al {dtFin:dd/MM/yyyy}",
                    TotalDebe = 0, // Se calculará con los detalles
                    TotalHaber = 0, // Se calculará con los detalles
                    Estatus = "VIGENTE", // 1=Activa
                    FechaAlta = DateTime.Now,
                    Usuario = User.Identity.Name ?? "system",
                    EsAutomatica = true,
                    EsCuadrada = false, // Se validará al final
                    ListaPolizaDetalle = new System.Collections.Generic.List<PolizaDetalle>()
                };

                // ============================================================
                // PERCEPCIONES (CARGOS - Gastos de Nómina)
                // ============================================================

                // 1. Sueldos y Salarios
                if (datosNomina.SueldosYSalarios > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaSueldosYSalarios).GetValueOrDefault(),
                        TipMov = "1", // 1 = Cargo (Debe)
                        Concepto = "Sueldos ordinarios del periodo",
                        Cantidad = 1,
                        Importe = datosNomina.SueldosYSalarios,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 2. Premio Puntualidad
                if (datosNomina.PremioPuntualidad > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaPremioPuntualidad).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Premio por puntualidad",
                        Cantidad = 1,
                        Importe = datosNomina.PremioPuntualidad,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 3. Premio Asistencia
                if (datosNomina.PremioAsistencia > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaPremioAsistencia).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Premio por asistencia",
                        Cantidad = 1,
                        Importe = datosNomina.PremioAsistencia,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 4. Vacaciones
                if (datosNomina.Vacaciones > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaVacaciones).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Pago de vacaciones",
                        Cantidad = 1,
                        Importe = datosNomina.Vacaciones,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 5. Prima Vacacional
                if (datosNomina.PrimaVacacional > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaPrimaVacacional).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Prima vacacional",
                        Cantidad = 1,
                        Importe = datosNomina.PrimaVacacional,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 6. Aguinaldo
                if (datosNomina.Aguinaldo > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaAguinaldo).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Aguinaldo anual",
                        Cantidad = 1,
                        Importe = datosNomina.Aguinaldo,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 7. PTU
                if (datosNomina.PTU > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaPTU).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Participación de Utilidades",
                        Cantidad = 1,
                        Importe = datosNomina.PTU,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // ============================================================
                // CUOTAS PATRONALES (CARGOS - Gastos Patronales)
                // ============================================================

                // 8. IMSS Patronal
                if (datosNomina.IMSSPatronal > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaIMSSPatronal).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Cuotas patronales IMSS",
                        Cantidad = 1,
                        Importe = datosNomina.IMSSPatronal,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 9. SAR Patronal
                if (datosNomina.SARPatronal > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaSARPatronal).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Cuotas Sistema de Ahorro para el Retiro",
                        Cantidad = 1,
                        Importe = datosNomina.SARPatronal,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 10. Infonavit 5% Patronal
                if (datosNomina.InfonavitPatronal > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaInfonavitPatronal).GetValueOrDefault(),
                        TipMov = "1",
                        Concepto = "Aportación patronal 5% Infonavit",
                        Cantidad = 1,
                        Importe = datosNomina.InfonavitPatronal,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // ============================================================
                // DEDUCCIONES (ABONOS - Pasivos por Pagar)
                // ============================================================

                // 11. ISR Retenido
                if (datosNomina.ISRRetenido > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaISRRetenido).GetValueOrDefault(),
                        TipMov = "2", // 2 = Abono (Haber)
                        Concepto = "ISR retenido a trabajadores",
                        Cantidad = 1,
                        Importe = datosNomina.ISRRetenido,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 12. IMSS Obrero
                if (datosNomina.IMSSObrero > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaIMSSObrero).GetValueOrDefault(),
                        TipMov = "2",
                        Concepto = "Cuotas IMSS parte obrero",
                        Cantidad = 1,
                        Importe = datosNomina.IMSSObrero,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 13. Infonavit Créditos
                if (datosNomina.InfonavitCreditos > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaInfonavitCreditos).GetValueOrDefault(),
                        TipMov = "2",
                        Concepto = "Descuentos de créditos Infonavit",
                        Cantidad = 1,
                        Importe = datosNomina.InfonavitCreditos,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // 14. Fonacot
                if (datosNomina.Fonacot > 0)
                {
                    poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                    {
                        PolizaDetalleID = Guid.NewGuid(),
                        PolizaID = poliza.PolizaID,
                        CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaFonacot).GetValueOrDefault(),
                        TipMov = "2",
                        Concepto = "Descuentos de créditos Fonacot",
                        Cantidad = 1,
                        Importe = datosNomina.Fonacot,
                        Estatus = "VIGENTE",
                        Fecha = DateTime.Now,
                        Usuario = User.Identity.Name ?? "system"
                    });
                }

                // ============================================================
                // PAGO NETO (ABONO - Disminución de Activo - Bancos)
                // ============================================================

                // 15. Bancos - Pago Nómina
                decimal totalPago = datosNomina.NetoAPagar + datosNomina.TotalCuotasPatronales;
                
                poliza.ListaPolizaDetalle.Add(new PolizaDetalle
                {
                    PolizaDetalleID = Guid.NewGuid(),
                    PolizaID = poliza.PolizaID,
                    CuentaID = CD_CuentaContable.Instancia.ObtenerCuentaIDPorCodigo(configuracionCuentas.CuentaBancosNomina).GetValueOrDefault(),
                    TipMov = "2",
                    Concepto = $"Pago de nómina y cuotas patronales",
                    Cantidad = 1,
                    Importe = totalPago,
                    Estatus = "VIGENTE",
                    Fecha = DateTime.Now,
                    Usuario = User.Identity.Name ?? "system"
                });

                // Calcular importe total de la póliza
                poliza.Importe = poliza.ListaPolizaDetalle.Where(d => d.TipMov == "1").Sum(d => d.Importe);

                // Guardar en base de datos usando el nuevo método
                bool resultado = CD_Poliza.Instancia.CrearPolizaCompleta(poliza, null);

                if (resultado)
                {
                    return Json(new { 
                        resultado = true, 
                        polizaId = poliza.PolizaID.ToString(),
                        referencia = poliza.Referencia,
                        totalDebe = poliza.TotalDebe,
                        totalHaber = poliza.TotalHaber,
                        detalles = new {
                            totalPercepciones = datosNomina.TotalPercepciones,
                            totalDeducciones = datosNomina.TotalDeducciones,
                            totalCuotasPatronales = datosNomina.TotalCuotasPatronales,
                            netoAPagar = datosNomina.NetoAPagar
                        }
                    });
                }
                else
                {
                    return Json(new { resultado = false, mensaje = "Error al crear la póliza" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = "Error: " + ex.Message });
            }
        }

    }
}
