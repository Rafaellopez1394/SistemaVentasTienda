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

        // Vista para consultar historial de ventas
        public ActionResult Consultar()
        {
            return View();
        }

        // Obtener todas las ventas (para reporte/historial)
        [HttpGet]
        public JsonResult ObtenerTodasVentas(string fechaInicio = null, string fechaFin = null, string codigoVenta = null, string documentoCliente = null, string nombreCliente = null)
        {
            try
            {
                var ventas = CD_Venta.Instancia.ObtenerTodasVentas(fechaInicio, fechaFin, codigoVenta, documentoCliente, nombreCliente);
                return Json(new { data = ventas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<object>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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

        // Obtener detalle de una venta específica
        [HttpGet]
        public JsonResult ObtenerDetalleVenta(Guid ventaId)
        {
            var venta = CD_Venta.Instancia.ObtenerDetalle(ventaId);
            return Json(new { success = true, data = venta }, JsonRequestBehavior.AllowGet);
        }

        // Obtener historial de pagos de una venta
        [HttpGet]
        public JsonResult ObtenerHistorialPagos(Guid ventaId)
        {
            try
            {
                var pagos = CD_Venta.Instancia.ObtenerPagosVenta(ventaId);
                return Json(new { success = true, data = pagos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
            pago.FechaPago = DateTime.Now;
            
            try
            {
                string mensajeDetallado;
                bool resultado = CD_Venta.Instancia.RegistrarPago(pago, out mensajeDetallado);
                
                if (resultado)
                {
                    return Json(new { success = true, mensaje = mensajeDetallado });
                }
                else
                {
                    return Json(new { success = false, mensaje = "Error al registrar el pago" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // Registrar pago total - liquida todas las ventas pendientes del cliente
        [HttpPost]
        public JsonResult RegistrarPagoTotal(PagoCliente pago)
        {
            pago.Usuario = User.Identity.Name ?? "system";
            pago.FechaPago = DateTime.Now;
            
            try
            {
                // Obtener todas las ventas pendientes del cliente
                var ventas = CD_Venta.Instancia.ObtenerVentasCliente(pago.ClienteID)
                    .Where(v => v.SaldoPendiente > 0)
                    .OrderBy(v => v.FechaVenta)
                    .ToList();
                
                if (ventas.Count == 0)
                {
                    return Json(new { success = false, mensaje = "No hay ventas pendientes para este cliente" });
                }

                decimal montoTotal = pago.Monto;
                decimal montoRestante = montoTotal;
                int ventasPagadas = 0;
                string errores = "";
                
                // Procesar pago para cada venta pendiente
                foreach (var venta in ventas)
                {
                    if (montoRestante <= 0) break;
                    
                    decimal montoPagoVenta = Math.Min(montoRestante, venta.SaldoPendiente);
                    
                    var pagoVenta = new PagoCliente
                    {
                        VentaID = venta.VentaID,
                        ClienteID = pago.ClienteID,
                        Monto = montoPagoVenta,
                        FormaPago = pago.FormaPago,
                        Referencia = pago.Referencia,
                        Comentario = pago.Comentario + $" (Venta {venta.VentaID.ToString().Substring(0, 8)})",
                        GenerarFactura = pago.GenerarFactura,
                        GenerarComplemento = pago.GenerarComplemento,
                        Usuario = pago.Usuario,
                        FechaPago = pago.FechaPago
                    };
                    
                    try
                    {
                        string mensajeDetallado;
                        bool resultado = CD_Venta.Instancia.RegistrarPago(pagoVenta, out mensajeDetallado);
                        
                        if (resultado)
                        {
                            montoRestante -= montoPagoVenta;
                            ventasPagadas++;
                        }
                        else
                        {
                            errores += $"Venta {venta.VentaID.ToString().Substring(0, 8)}: {mensajeDetallado}. ";
                        }
                    }
                    catch (Exception exVenta)
                    {
                        errores += $"Venta {venta.VentaID.ToString().Substring(0, 8)}: {exVenta.Message}. ";
                    }
                }
                
                if (ventasPagadas > 0)
                {
                    string mensaje = $"Pago total registrado. Se liquidaron {ventasPagadas} venta(s) por ${(montoTotal - montoRestante):F2}";
                    if (!string.IsNullOrEmpty(errores))
                    {
                        mensaje += ". Advertencias: " + errores;
                    }
                    return Json(new { success = true, mensaje = mensaje });
                }
                else
                {
                    return Json(new { success = false, mensaje = "No se pudo procesar ningún pago. " + errores });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error al procesar el pago total: " + ex.Message });
            }
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