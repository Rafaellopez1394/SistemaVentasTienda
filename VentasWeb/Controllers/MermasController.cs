using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class MermasController : Controller
    {
        // GET: Mermas (Vista principal con historial)
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");

            return View();
        }

        // GET: Mermas/RegistrarMerma (Formulario para registrar merma)
        public ActionResult RegistrarMerma()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");

            return View();
        }

        // GET: Mermas/RegistrarAjuste (Formulario para registrar ajuste)
        public ActionResult RegistrarAjuste()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");

            return View();
        }

        // POST: Mermas/RegistrarMerma
        [HttpPost]
        public JsonResult GuardarMerma(int loteId, int cantidad, string comentarios)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                string usuario = Session["Usuario"].ToString();

                // Validar que el lote existe y tiene stock suficiente
                var lote = CD_Producto.Instancia.ObtenerLotePorID(loteId);
                if (lote == null)
                    return Json(new { success = false, message = "Lote no encontrado" });

                if (lote.CantidadDisponible < cantidad)
                    return Json(new { success = false, message = $"Stock insuficiente. Disponible: {lote.CantidadDisponible}" });

                // Registrar merma (esto genera automáticamente la póliza contable)
                bool resultado = CD_Producto.Instancia.RegistrarMerma(loteId, cantidad, usuario, comentarios);

                if (resultado)
                {
                    return Json(new { 
                        success = true, 
                        message = "Merma registrada exitosamente. Se ha generado la póliza contable automáticamente." 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Error al registrar la merma" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Mermas/GuardarAjuste
        [HttpPost]
        public JsonResult GuardarAjuste(int productoId, int? loteId, string tipo, decimal cantidad, string motivo)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                if (Session["IdUsuario"] == null)
                    return Json(new { success = false, message = "Usuario no identificado" });

                int usuarioId = Convert.ToInt32(Session["IdUsuario"]);

                // Validar lote si se especificó
                if (loteId.HasValue && loteId.Value > 0)
                {
                    var lote = CD_Producto.Instancia.ObtenerLotePorID(loteId.Value);
                    if (lote == null)
                        return Json(new { success = false, message = "Lote no encontrado" });

                    // Validar stock solo si es ajuste negativo
                    bool esNegativo = tipo.ToUpper().Contains("NEGATIVO") || tipo.ToUpper().Contains("SALIDA") || tipo.ToUpper().Contains("FALTANTE");
                    if (esNegativo && lote.CantidadDisponible < cantidad)
                        return Json(new { success = false, message = $"Stock insuficiente. Disponible: {lote.CantidadDisponible}" });
                }

                // Crear objeto AjusteInventario
                var ajuste = new AjusteInventario
                {
                    Fecha = DateTime.Now,
                    ProductoId = productoId,
                    LoteEntradaId = loteId,
                    Tipo = tipo,
                    Cantidad = cantidad,
                    Motivo = motivo,
                    UsuarioId = usuarioId
                };

                // Registrar ajuste (esto genera automáticamente la póliza contable)
                bool resultado = CD_Producto.Instancia.RegistrarAjuste(ajuste);

                if (resultado)
                {
                    return Json(new { 
                        success = true, 
                        message = "Ajuste registrado exitosamente. Se ha generado la póliza contable automáticamente." 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Error al registrar el ajuste" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Obtener movimientos de inventario para DataTable
        [HttpGet]
        public JsonResult ObtenerMovimientos(int? productoId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var movimientos = CD_Producto.Instancia.ObtenerMovimientosInventario(
                    productoId, 
                    null, // loteId
                    fechaInicio, 
                    fechaFin
                );

                // Filtrar solo MERMA y AJUSTES
                var mermasYAjustes = movimientos
                    .Where(m => m.TipoMovimiento == "MERMA" || 
                                m.TipoMovimiento.StartsWith("AJUSTE"))
                    .Select(m => new {
                        movimientoID = m.MovimientoID,
                        fecha = m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        loteID = m.LoteID,
                        productoID = m.ProductoID,
                        tipoMovimiento = m.TipoMovimiento,
                        cantidad = m.Cantidad,
                        costoUnitario = m.CostoUnitario.ToString("N2"),
                        montoTotal = (m.Cantidad * m.CostoUnitario).ToString("N2"),
                        usuario = m.Usuario,
                        comentarios = m.Comentarios
                    })
                    .ToList();

                return Json(new { success = true, data = mermasYAjustes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener productos para dropdown
        [HttpGet]
        public JsonResult ObtenerProductos()
        {
            try
            {
                var productos = CD_Producto.Instancia.ObtenerTodos()
                    .Select(p => new { 
                        value = p.ProductoID, 
                        text = $"{p.Nombre} ({p.CodigoInterno})" 
                    })
                    .ToList();

                return Json(productos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Buscar producto por código de barras
        [HttpGet]
        public JsonResult BuscarProductoPorCodigo(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    return Json(new { success = false, message = "El código de barras no puede estar vacío" }, JsonRequestBehavior.AllowGet);
                }

                var productos = CD_Producto.Instancia.ObtenerTodos();
                var producto = productos.FirstOrDefault(p => 
                    p.CodigoInterno != null && p.CodigoInterno.Equals(codigo, StringComparison.OrdinalIgnoreCase)
                );

                if (producto != null)
                {
                    return Json(new { 
                        success = true, 
                        productoId = producto.ProductoID,
                        nombreProducto = producto.Nombre,
                        codigoInterno = producto.CodigoInterno
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = $"No se encontró ningún producto con el código: {codigo}" 
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error al buscar el producto: " + ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener lotes de un producto
        [HttpGet]
        public JsonResult ObtenerLotesPorProducto(int productoId)
        {
            try
            {
                // Usar CD_Producto para obtener lotes del producto
                var lotes = CD_Producto.Instancia.ObtenerLotesDisponibles(productoId)
                    .Where(l => l.Estatus && l.CantidadDisponible > 0)
                    .Select(l => new {
                        value = l.LoteID,
                        text = $"Lote {l.LoteID} - Disponible: {l.CantidadDisponible} - Caducidad: {(l.FechaCaducidad.HasValue ? l.FechaCaducidad.Value.ToString("dd/MM/yyyy") : "N/A")}",
                        cantidadDisponible = l.CantidadDisponible,
                        precioCompra = l.PrecioCompra
                    })
                    .ToList();

                return Json(new { success = true, lotes = lotes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
