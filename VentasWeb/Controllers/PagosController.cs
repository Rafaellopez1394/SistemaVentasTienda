using System;
using System.Collections.Generic;
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

                // Generar y timbrar complemento de pago
                var respuesta = await CD_ComplementoPago.Instancia.GenerarYTimbrarComplementoPago(request, usuario);

                if (respuesta.Exitoso)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = "Complemento de pago generado y timbrado exitosamente",
                        uuid = respuesta.UUID,
                        fechaTimbrado = respuesta.FechaTimbrado.HasValue ? respuesta.FechaTimbrado.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
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
