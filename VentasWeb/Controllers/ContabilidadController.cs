using CapaDatos;
using CapaModelo;
using System;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class ContabilidadController : Controller
    {
        // GET: Contabilidad
        public ActionResult Index()
        {
            return View();
        }

        // =============================================
        // BALANZA DE COMPROBACIÓN
        // =============================================
        public ActionResult Balanza()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerarBalanza(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var resultado = CD_ReportesContables.Instancia.GenerarBalanza(fechaInicio, fechaFin);
                
                return Json(new
                {
                    success = true,
                    data = resultado.Select(b => new
                    {
                        b.CuentaID,
                        b.CodigoCuenta,
                        b.NombreCuenta,
                        b.Tipo,
                        b.Naturaleza,
                        SaldoInicial = b.SaldoInicial.ToString("N2"),
                        Debe = b.Debe.ToString("N2"),
                        Haber = b.Haber.ToString("N2"),
                        SaldoFinal = b.SaldoFinal.ToString("N2")
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // ESTADO DE RESULTADOS
        // =============================================
        public ActionResult EstadoResultados()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerarEstadoResultados(int mes, int año)
        {
            try
            {
                var resultado = CD_ReportesContables.Instancia.GenerarEstadoResultados(mes, año);
                
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        resultado.Empresa,
                        resultado.Periodo,
                        FechaInicio = resultado.FechaInicio.ToString("dd/MM/yyyy"),
                        FechaFin = resultado.FechaFin.ToString("dd/MM/yyyy"),
                        VentasNetas = resultado.VentasNetas.ToString("N2"),
                        CostoVentas = resultado.CostoVentas.ToString("N2"),
                        UtilidadBruta = resultado.UtilidadBruta.ToString("N2"),
                        GastosVenta = resultado.GastosVenta.ToString("N2"),
                        GastosAdministracion = resultado.GastosAdministracion.ToString("N2"),
                        TotalGastosOperacion = resultado.TotalGastosOperacion.ToString("N2"),
                        UtilidadOperacion = resultado.UtilidadOperacion.ToString("N2"),
                        GastosFinancieros = resultado.GastosFinancieros.ToString("N2"),
                        ProductosFinancieros = resultado.ProductosFinancieros.ToString("N2"),
                        OtrosIngresos = resultado.OtrosIngresos.ToString("N2"),
                        UtilidadAntesImpuestos = resultado.UtilidadAntesImpuestos.ToString("N2"),
                        ISR = resultado.ISR.ToString("N2"),
                        UtilidadNeta = resultado.UtilidadNeta.ToString("N2"),
                        Detalle = resultado.Detalle.Select(d => new
                        {
                            d.Seccion,
                            d.CodigoCuenta,
                            d.NombreCuenta,
                            Monto = d.Monto.ToString("N2")
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // LIBRO DIARIO
        // =============================================
        public ActionResult LibroDiario()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerarLibroDiario(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var resultado = CD_ReportesContables.Instancia.GenerarLibroDiario(fechaInicio, fechaFin);
                
                return Json(new
                {
                    success = true,
                    data = resultado.Select(p => new
                    {
                        PolizaID = p.PolizaID.ToString(),
                        p.TipoPoliza,
                        FechaPoliza = p.FechaPoliza.ToString("dd/MM/yyyy"),
                        p.Concepto,
                        p.Referencia,
                        TotalDebe = p.TotalDebe.ToString("N2"),
                        TotalHaber = p.TotalHaber.ToString("N2"),
                        Asientos = p.Asientos.Select(a => new
                        {
                            a.CodigoCuenta,
                            a.NombreCuenta,
                            Debe = a.Debe.ToString("N2"),
                            Haber = a.Haber.ToString("N2")
                        })
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // REPORTE IVA
        // =============================================
        public ActionResult ReporteIVA()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerarReporteIVA(int mes, int año)
        {
            try
            {
                var resultado = CD_ReportesContables.Instancia.GenerarReporteIVA(mes, año);
                
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        resultado.Periodo,
                        IVA16Trasladado = resultado.IVA16Trasladado.ToString("N2"),
                        IVA8Trasladado = resultado.IVA8Trasladado.ToString("N2"),
                        IVA0Trasladado = resultado.IVA0Trasladado.ToString("N2"),
                        TotalIVATrasladado = resultado.TotalIVATrasladado.ToString("N2"),
                        IVA16Acreditable = resultado.IVA16Acreditable.ToString("N2"),
                        IVA8Acreditable = resultado.IVA8Acreditable.ToString("N2"),
                        TotalIVAAcreditable = resultado.TotalIVAAcreditable.ToString("N2"),
                        SaldoFavor = resultado.SaldoFavor.ToString("N2"),
                        SaldoAFavor = resultado.SaldoAFavor.ToString("N2"),
                        DetalleVentas = resultado.DetalleVentas.Select(d => new
                        {
                            Fecha = d.Fecha.ToString("dd/MM/yyyy"),
                            d.RFC,
                            d.Nombre,
                            d.Documento,
                            Base = d.Base.ToString("N2"),
                            Tasa = d.Tasa.ToString("N2"),
                            IVA = d.IVA.ToString("N2"),
                            Total = d.Total.ToString("N2")
                        }),
                        DetalleCompras = resultado.DetalleCompras.Select(d => new
                        {
                            Fecha = d.Fecha.ToString("dd/MM/yyyy"),
                            d.RFC,
                            d.Nombre,
                            d.Documento,
                            Base = d.Base.ToString("N2"),
                            Tasa = d.Tasa.ToString("N2"),
                            IVA = d.IVA.ToString("N2"),
                            Total = d.Total.ToString("N2")
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // AUXILIAR DE CUENTA (LIBRO MAYOR)
        // =============================================
        public ActionResult AuxiliarCuenta()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerarAuxiliarCuenta(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var resultado = CD_ReportesContables.Instancia.GenerarLibroMayor(cuentaId, fechaInicio, fechaFin);
                
                if (resultado == null)
                {
                    return Json(new { success = false, mensaje = "Cuenta no encontrada" });
                }
                
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        resultado.CuentaID,
                        resultado.CodigoCuenta,
                        resultado.NombreCuenta,
                        SaldoInicial = resultado.SaldoInicial.ToString("N2"),
                        TotalCargos = resultado.TotalCargos.ToString("N2"),
                        TotalAbonos = resultado.TotalAbonos.ToString("N2"),
                        SaldoFinal = resultado.SaldoFinal.ToString("N2"),
                        Movimientos = resultado.Movimientos.Select(m => new
                        {
                            Fecha = m.Fecha.ToString("dd/MM/yyyy"),
                            m.TipoPoliza,
                            m.Concepto,
                            m.Referencia,
                            Cargo = m.Cargo.ToString("N2"),
                            Abono = m.Abono.ToString("N2"),
                            Saldo = m.Saldo.ToString("N2")
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // OBTENER CATÁLOGO DE CUENTAS
        // =============================================
        [HttpGet]
        public JsonResult ObtenerCatalogoCuentas()
        {
            try
            {
                var cuentas = CD_ReportesContables.Instancia.ObtenerCatalogoCuentas()
                    .Select(c => new
                    {
                        c.CuentaID,
                        c.Codigo,
                        c.Nombre,
                        Display = $"{c.Codigo} - {c.Nombre}"
                    })
                    .OrderBy(c => c.Codigo)
                    .ToList();

                return Json(new { success = true, data = cuentas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
