using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class DebugController : Controller
    {
        // GET: Debug/TestFactura
        public JsonResult TestFactura(string ventaId)
        {
            try
            {
                var request = new GenerarFacturaRequest
                {
                    VentaID = Guid.Parse(ventaId),
                    ReceptorRFC = "XAXX010101000",
                    ReceptorNombre = "PUBLICO EN GENERAL",
                    ReceptorRegimenFiscal = "616",
                    UsoCFDI = "S01",
                    FormaPago = "01",
                    MetodoPago = "PUE"
                };

                var factura = CD_Factura.Instancia.CrearFacturaDesdeVenta(request, out string mensaje);

                if (factura == null)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "Factura es null: " + mensaje
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    mensaje = "Factura creada correctamente",
                    data = new
                    {
                        FacturaID = factura.FacturaID,
                        Total = factura.Total,
                        Subtotal = factura.Subtotal,
                        NumConceptos = factura.Conceptos?.Count ?? 0,
                        EmisorRFC = factura.EmisorRFC,
                        ReceptorRFC = factura.ReceptorRFC,
                        Conceptos = factura.Conceptos
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Error: " + ex.Message,
                    stack = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
