using CapaDatos;
using CapaModelo;
using System;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class CuentasPorPagarController : Controller
    {
        // GET: CuentasPorPagar
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pagar
        public ActionResult Pagar()
        {
            return View();
        }

        // GET: ReporteAntiguedad
        public ActionResult ReporteAntiguedad()
        {
            return View();
        }

        // =============================================
        // API: OBTENER TODAS LAS CUENTAS
        // =============================================
        [HttpGet]
        public JsonResult ObtenerTodas(string estado = null)
        {
            try
            {
                var cuentas = CD_CuentasPorPagar.Instancia.ObtenerTodas(estado);
                
                return Json(new
                {
                    success = true,
                    data = cuentas.Select(c => new
                    {
                        CuentaPorPagarID = c.CuentaPorPagarID.ToString(),
                        c.CompraID,
                        c.ProveedorID,
                        c.Proveedor,
                        c.RFC,
                        FechaRegistro = c.FechaRegistro.ToString("dd/MM/yyyy"),
                        FechaVencimiento = c.FechaVencimiento.ToString("dd/MM/yyyy"),
                        c.DiasParaVencer,
                        c.DiasVencido,
                        MontoTotal = c.MontoTotal.ToString("N2"),
                        SaldoPendiente = c.SaldoPendiente.ToString("N2"),
                        c.Estado,
                        c.FolioFactura,
                        TotalPagado = c.TotalPagado.ToString("N2"),
                        c.NumeroPagos
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // API: OBTENER POR PROVEEDOR
        // =============================================
        [HttpGet]
        public JsonResult ObtenerPorProveedor(int proveedorId)
        {
            try
            {
                var cuentas = CD_CuentasPorPagar.Instancia.ObtenerPorProveedor(proveedorId);
                
                return Json(new
                {
                    success = true,
                    data = cuentas.Select(c => new
                    {
                        CuentaPorPagarID = c.CuentaPorPagarID.ToString(),
                        c.CompraID,
                        c.Proveedor,
                        FechaRegistro = c.FechaRegistro.ToString("dd/MM/yyyy"),
                        FechaVencimiento = c.FechaVencimiento.ToString("dd/MM/yyyy"),
                        MontoTotal = c.MontoTotal.ToString("N2"),
                        SaldoPendiente = c.SaldoPendiente.ToString("N2"),
                        c.Estado,
                        c.FolioFactura
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // API: REGISTRAR PAGO
        // =============================================
        [HttpPost]
        public JsonResult RegistrarPago(RegistrarPagoRequest request)
        {
            try
            {
                // Obtener usuario de sesión
                if (Session["IdUsuario"] != null)
                {
                    request.UsuarioRegistro = Convert.ToInt32(Session["IdUsuario"]);
                }

                var respuesta = CD_CuentasPorPagar.Instancia.RegistrarPago(request);
                
                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // =============================================
        // API: OBTENER PAGOS DE UNA CUENTA
        // =============================================
        [HttpGet]
        public JsonResult ObtenerPagos(string cuentaPorPagarId)
        {
            try
            {
                var guid = Guid.Parse(cuentaPorPagarId);
                var pagos = CD_CuentasPorPagar.Instancia.ObtenerPagosDeCuenta(guid);
                
                return Json(new
                {
                    success = true,
                    data = pagos.Select(p => new
                    {
                        PagoID = p.PagoID.ToString(),
                        FechaPago = p.FechaPago.ToString("dd/MM/yyyy HH:mm"),
                        MontoPagado = p.MontoPagado.ToString("N2"),
                        p.FormaPago,
                        p.Referencia,
                        p.CuentaBancaria,
                        p.Observaciones,
                        PolizaID = p.PolizaID?.ToString()
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // API: REPORTE ANTIGÜEDAD
        // =============================================
        [HttpGet]
        public JsonResult ObtenerReporteAntiguedad()
        {
            try
            {
                var reporte = CD_CuentasPorPagar.Instancia.GenerarReporteAntiguedad();
                
                return Json(new
                {
                    success = true,
                    data = reporte.Select(r => new
                    {
                        r.ProveedorID,
                        r.Proveedor,
                        r.RFC,
                        TotalAdeudo = r.TotalAdeudo.ToString("N2"),
                        Corriente = r.Corriente.ToString("N2"),
                        Dias30 = r.Dias30.ToString("N2"),
                        Dias60 = r.Dias60.ToString("N2"),
                        Dias90 = r.Dias90.ToString("N2"),
                        Mas120 = r.Mas120.ToString("N2"),
                        r.CuentasVencidas
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // API: OBTENER PROVEEDORES
        // =============================================
        [HttpGet]
        public JsonResult ObtenerProveedores()
        {
            try
            {
                var proveedores = CD_Proveedor.Instancia.ObtenerTodos()
                    .Where(p => p.Estatus)
                    .Select(p => new
                    {
                        p.ProveedorID,
                        p.RazonSocial,
                        p.RFC,
                        Display = $"{p.RazonSocial} - {p.RFC}"
                    })
                    .OrderBy(p => p.RazonSocial)
                    .ToList();

                return Json(new { success = true, data = proveedores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
