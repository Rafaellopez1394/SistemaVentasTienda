using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VentasWeb.Filters;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class VentaPOSController : Controller
    {
        // GET: VentaPOS
        public ActionResult Index()
        {
            return View();
        }

        // Buscar productos para agregar al carrito
        [HttpGet]
        public JsonResult BuscarProducto(string texto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return Json(new { success = false, data = new List<ProductoPOS>() }, JsonRequestBehavior.AllowGet);

                var productos = CD_VentaPOS.Instancia.BuscarProducto(texto);

                // Agregar información de lote a cada producto
                foreach (var producto in productos)
                {
                    int loteID;
                    decimal precioCompra;
                    if (CD_VentaPOS.Instancia.ObtenerLoteActivo(producto.ProductoID, out loteID, out precioCompra))
                    {
                        producto.LoteID = loteID;
                        producto.PrecioCompra = precioCompra;
                    }
                }

                return Json(new { success = true, data = productos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Buscar clientes (reutilizando la función existente)
        [HttpGet]
        public JsonResult BuscarCliente(string texto)
        {
            try
            {
                var clientes = CD_Cliente.Instancia.BuscarPorNombreOCRFC(texto);
                return Json(new { success = true, data = clientes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Obtener información completa del cliente
        [HttpGet]
        public JsonResult ObtenerCliente(Guid id)
        {
            try
            {
                var cliente = CD_Cliente.Instancia.ObtenerPorId(id);
                if (cliente == null)
                    return Json(new { success = false, mensaje = "Cliente no encontrado" }, JsonRequestBehavior.AllowGet);

                var creditos = CD_Cliente.Instancia.ObtenerTiposCreditoCliente(id);
                var creditoDisponible = CD_Cliente.Instancia.ObtenerCreditoDisponible(id);
                var limiteTotal = CD_Cliente.Instancia.ObtenerLimiteCreditoTotal(id);
                var saldoActual = CD_Cliente.Instancia.ObtenerSaldoActual(id);

                return Json(new
                {
                    success = true,
                    cliente = cliente,
                    creditos = creditos,
                    creditoDisponible = creditoDisponible,
                    limiteTotal = limiteTotal,
                    saldoActual = saldoActual
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Validar si el cliente tiene crédito suficiente
        [HttpPost]
        public JsonResult ValidarCredito(Guid clienteID, decimal montoVenta)
        {
            try
            {
                var validacion = CD_VentaPOS.Instancia.ValidarCredito(clienteID, montoVenta);
                return Json(new { success = true, validacion = validacion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Obtener catálogos para el POS
        [HttpGet]
        public JsonResult ObtenerCatalogos()
        {
            try
            {
                var formasPago = CD_VentaPOS.Instancia.ObtenerFormasPago();
                var metodosPago = CD_VentaPOS.Instancia.ObtenerMetodosPago();
                var usosCFDI = CD_Catalogos.Instancia.ObtenerUsosCFDI();

                return Json(new
                {
                    success = true,
                    formasPago = formasPago,
                    metodosPago = metodosPago,
                    usosCFDI = usosCFDI
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Registrar venta
        [HttpPost]
        public JsonResult RegistrarVenta(RegistrarVentaPayload payload)
        {
            try
            {
                if (payload == null || payload.Venta == null || payload.Detalle == null || payload.Detalle.Count == 0)
                    return Json(new { success = false, mensaje = "Datos de venta incompletos" }, JsonRequestBehavior.AllowGet);

                // Asignar usuario actual
                payload.Venta.Usuario = User.Identity.Name ?? "system";

                // Asignar caja por defecto (puedes cambiarlo según tu lógica)
                if (payload.Venta.CajaID == 0)
                    payload.Venta.CajaID = 1;

                // Registrar venta
                string mensaje;
                Guid ventaID;
                bool exito = CD_VentaPOS.Instancia.RegistrarVenta(payload, out mensaje, out ventaID);

                if (exito)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = mensaje,
                        ventaId = ventaID.ToString()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Apertura de caja
        [HttpPost]
        public JsonResult AperturaCaja(int cajaID, decimal montoInicial)
        {
            try
            {
                string mensaje;
                bool exito = CD_VentaPOS.Instancia.AperturaCaja(cajaID, montoInicial, User.Identity.Name ?? "system", out mensaje);

                return Json(new { success = exito, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Cierre de caja
        [HttpPost]
        public JsonResult CierreCaja(int cajaID)
        {
            try
            {
                decimal saldoFinal, totalVentas;
                string mensaje;
                
                bool exito = CD_VentaPOS.Instancia.CierreCaja(cajaID, User.Identity.Name ?? "system", out saldoFinal, out totalVentas, out mensaje);

                return Json(new
                {
                    success = exito,
                    mensaje = mensaje,
                    saldoFinal = saldoFinal,
                    totalVentas = totalVentas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
