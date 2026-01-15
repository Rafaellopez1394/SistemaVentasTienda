using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class PagosController : Controller
    {
        // GET: Pagos/AplicarPagos
        public ActionResult AplicarPagos()
        {
            return View();
        }

        // GET: Pagos/Historial
        public ActionResult Historial()
        {
            return View();
        }

        // GET: Pagos/GestionarPagosVentas
        public ActionResult GestionarPagosVentas()
        {
            return View();
        }

        // GET: Pagos/ObtenerVentasPendientes
        [HttpGet]
        public JsonResult ObtenerVentasPendientes(string clienteID = null)
        {
            try
            {
                Guid? clienteGuid = null;
                if (!string.IsNullOrEmpty(clienteID))
                {
                    clienteGuid = Guid.Parse(clienteID);
                }

                var ventas = CD_VentaPOS.Instancia.ObtenerVentasPendientesPago(clienteGuid);

                // Mapear explícitamente para evitar problemas de serialización
                var ventasSerializadas = ventas.Select(v => new
                {
                    VentaID = v.VentaID.ToString(),
                    ClienteID = v.ClienteID.ToString(),
                    ClienteRazonSocial = v.ClienteRazonSocial ?? v.Cliente ?? "",
                    ClienteRFC = v.ClienteRFC ?? v.RFC ?? "",
                    FechaVenta = v.FechaVenta,
                    Total = v.Total,
                    MontoPagado = v.MontoPagado,
                    SaldoPendiente = v.SaldoPendiente,
                    EsPagado = v.EsPagado,
                    RequiereFactura = v.RequiereFactura,
                    NumeroPagos = v.NumeroPagos
                }).ToList();

                return Json(new
                {
                    success = true,
                    data = ventasSerializadas
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

        // GET: Pagos/ObtenerHistorialPagos
        [HttpGet]
        public JsonResult ObtenerHistorialPagos(string ventaID)
        {
            try
            {
                if (string.IsNullOrEmpty(ventaID))
                {
                    return Json(new { success = false, mensaje = "VentaID requerido" }, JsonRequestBehavior.AllowGet);
                }

                Guid ventaGuid = Guid.Parse(ventaID);
                var pagos = CD_VentaPOS.Instancia.ObtenerPagosVenta(ventaGuid);

                return Json(new
                {
                    success = true,
                    data = pagos
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

        // GET: Pagos/ObtenerFormasPago
        [HttpGet]
        public JsonResult ObtenerFormasPago()
        {
            try
            {
                var formasPago = CD_VentaPOS.Instancia.ObtenerFormasPago();
                return Json(new { success = true, data = formasPago }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Pagos/ObtenerMetodosPago
        [HttpGet]
        public JsonResult ObtenerMetodosPago()
        {
            try
            {
                var metodosPago = CD_VentaPOS.Instancia.ObtenerMetodosPago();
                return Json(new { success = true, data = metodosPago }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Pagos/RegistrarPagoVenta
        [HttpPost]
        public JsonResult RegistrarPagoVenta(RegistrarPagoVentaRequest request)
        {
            try
            {
                if (Session["Usuario"] == null)
                {
                    return Json(new { success = false, mensaje = "Sesión expirada" });
                }

                string usuario = Session["Usuario"].ToString();

                // Validaciones
                if (request.VentaID == Guid.Empty)
                {
                    return Json(new { success = false, mensaje = "VentaID inválido" });
                }

                if (request.Monto <= 0)
                {
                    return Json(new { success = false, mensaje = "El monto debe ser mayor a cero" });
                }

                if (request.FormaPagoID <= 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar una forma de pago" });
                }

                if (request.MetodoPagoID <= 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar un método de pago" });
                }

                // Registrar pago
                string mensaje;
                int pagoID;
                bool exito = CD_VentaPOS.Instancia.RegistrarPagoVenta(request, usuario, out pagoID, out mensaje);

                if (exito)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = mensaje,
                        pagoID = pagoID
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = mensaje
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al registrar pago: " + ex.Message
                });
            }
        }

        // GET: Pagos/ObtenerFacturasPendientes
        [HttpGet]
        public JsonResult ObtenerFacturasPendientes(int clienteID)
        {
            try
            {
                if (clienteID <= 0)
                {
                    return Json(new { success = false, mensaje = "Cliente ID inválido" }, JsonRequestBehavior.AllowGet);
                }

                var facturas = CD_ComplementoPago.Instancia.ObtenerFacturasPendientes(clienteID);

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
                    mensaje = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Pagos/AplicarPago
        [HttpPost]
        public async Task<JsonResult> AplicarPago(AplicarPagoRequest request)
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
                if (request.ClienteID <= 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar un cliente" });
                }

                if (request.MontoTotal <= 0)
                {
                    return Json(new { success = false, mensaje = "El monto debe ser mayor a cero" });
                }

                if (request.Facturas == null || request.Facturas.Count == 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar al menos una factura" });
                }

                if (string.IsNullOrEmpty(request.FormaDePago))
                {
                    return Json(new { success = false, mensaje = "Debe especificar la forma de pago" });
                }

                // FUNCIONALIDAD DE TIMBRADO ELIMINADA
                return Json(new
                {
                    success = false,
                    mensaje = "Funcionalidad de complemento de pago y timbrado eliminada del sistema"
                });

                /* CÓDIGO ELIMINADO - Timbrado de complemento de pago */
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error al aplicar pago: " + ex.Message
                });
            }
        }

        // GET: Pagos/DescargarXML
        [HttpGet]
        public ActionResult DescargarXML(int complementoID)
        {
            try
            {
                // TODO: Implementar descarga de XML
                return Json(new { success = false, mensaje = "Funcionalidad en desarrollo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Pagos/DescargarPDF
        [HttpGet]
        public ActionResult DescargarPDF(int complementoID)
        {
            try
            {
                // TODO: Implementar generación de PDF
                return Json(new { success = false, mensaje = "Funcionalidad en desarrollo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
