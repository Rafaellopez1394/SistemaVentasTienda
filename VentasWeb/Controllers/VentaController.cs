// Controllers/VentaController.cs
using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using System.Collections.Generic;
using System.Linq;

namespace VentasWeb.Controllers
{
    public class VentaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Obtener ventas del cliente (para cuentas por cobrar)
        [HttpGet]
        public JsonResult ObtenerVentasCliente(Guid clienteId)
        {
            var ventas = CD_Venta.Instancia.ObtenerVentasCliente(clienteId);
            return Json(new { data = ventas }, JsonRequestBehavior.AllowGet);
        }

        // Buscar cliente por RFC o nombre
        [HttpGet]
        public JsonResult BuscarCliente(string texto)
        {
            var clientes = CD_Cliente.Instancia.BuscarPorNombreOCRFC(texto);
            return Json(clientes, JsonRequestBehavior.AllowGet);
        }

        // Obtener tipos de crédito del cliente
        [HttpGet]
        public JsonResult ObtenerCreditosCliente(Guid clienteId)
        {
            //var creditos = CD_Cliente.Instancia.ObtenerCreditos(clienteId);
            var creditos = CD_Cliente.Instancia.ObtenerTiposCreditoCliente(clienteId);
            return Json(creditos, JsonRequestBehavior.AllowGet);
        }

        // Obtener lotes disponibles de un producto
        [HttpGet]
        public JsonResult ObtenerLotesProducto(int productoId)
        {
            var lotes = CD_Producto.Instancia.ObtenerLotesDisponibles(productoId); // FEFO
            return Json(lotes, JsonRequestBehavior.AllowGet);
        }

        // Registrar venta (contado o crédito)
        [HttpPost]
        public JsonResult RegistrarVenta(VentaCliente venta)
        {
            string usuario = User.Identity.Name ?? "system";
            venta.Usuario = usuario;

            try
            {
                // AUTO-POBLAR DATOS FISCALES DE PRODUCTOS
                foreach (var detalle in venta.Detalle)
                {
                    dynamic datosFiscales = CD_Producto.Instancia.ObtenerDatosFiscales(detalle.ProductoID);
                    detalle.TasaIVAPorcentaje = datosFiscales.TasaIVAPorcentaje;
                    detalle.Exento = datosFiscales.Exento;
                }

                // OBTENER CRÉDITOS DEL CLIENTE (desde la tabla correcta)
                var creditos = CD_Cliente.Instancia.ObtenerTiposCredito(venta.ClienteID);

                decimal limiteDinero = creditos.Where(c => c.Criterio == "Dinero").Sum(c => c.LimiteDinero ?? 0);
                int limiteProducto = creditos.Where(c => c.Criterio == "Producto").Sum(c => c.LimiteProducto ?? 0);
                int? plazoDias = creditos.FirstOrDefault(c => c.Criterio == "Tiempo")?.PlazoDias;

                // Calcular saldo pendiente actual
                decimal saldoPendiente = CD_Venta.Instancia.ObtenerSaldoPendiente(venta.ClienteID);

                // Validaciones reales según tu modelo
                foreach (var cred in creditos.Where(c => c.Estatus))
                {
                    if (cred.Criterio == "Dinero" && (saldoPendiente + venta.Total) > cred.LimiteDinero)
                        return Json(new { resultado = false, mensaje = $"Límite de crédito en dinero excedido. Disponible: {(cred.LimiteDinero - saldoPendiente):C}" });

                    if (cred.Criterio == "Producto")
                    {
                        int totalUnidades = venta.Detalle.Sum(d => d.Cantidad);
                        if (totalUnidades > cred.LimiteProducto)
                            return Json(new { resultado = false, mensaje = $"Límite de unidades excedido: {cred.LimiteProducto}" });
                    }
                }

                bool resultado = CD_Venta.Instancia.RegistrarVentaCredito(venta, usuario);
                return Json(new { resultado, mensaje = resultado ? "Venta registrada con éxito" : "Error al registrar" });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }
        // Registrar pago de cliente
        [HttpPost]
        public JsonResult RegistrarPago(PagoCliente pago)
        {
            pago.Usuario = User.Identity.Name ?? "system";
            bool resultado = CD_Venta.Instancia.RegistrarPago(pago);
            return Json(new { resultado }, JsonRequestBehavior.AllowGet);
        }

        // Obtener catálogo de productos para autocompletado
        [HttpGet]
        public JsonResult ObtenerProductos(string termino = "")
        {
            var productos = CD_Producto.Instancia.BuscarProductos(termino);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }
    }
}