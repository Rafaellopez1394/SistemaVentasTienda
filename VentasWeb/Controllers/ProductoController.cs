// Controllers/ProductoController.cs
using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class ProductoController : Controller
    {
        public ActionResult Index() => View();

        [HttpGet]
        public JsonResult Obtener()
        {
            try
            {
                // 1. Obtenemos solo los datos básicos del producto (sin tocar lotes aquí)
                var productos = CD_Producto.Instancia.ObtenerTodos();

                var listaBase = productos.Select(p => new
                {
                    p.ProductoID,
                    p.CodigoInterno,
                    p.Nombre,
                    Categoria = p.NombreCategoria ?? "",
                    p.ClaveProdServSATID,
                    UnidadSAT = p.UnidadSAT ?? "",
                    TasaIVA = p.TasaIVAPorcentaje,
                    TasaIEPS = p.TasaIEPSPorcentaje,
                    p.Exento,
                    p.Estatus
                }).ToList();

                // 2. Calculamos el stock por separado (fuera del query de EF → evita el error de "árbol de expresión")
                var stocks = new Dictionary<int, int>();
                foreach (var item in listaBase)
                {
                    int stock = CD_Producto.Instancia.ObtenerLotes(item.ProductoID)
                                                    .Sum(l => l.CantidadDisponible);
                    stocks[item.ProductoID] = stock;
                }

                // 3. Construimos la lista final con el stock correcto
                var listaFinal = listaBase.Select(p => new
                {
                    p.ProductoID,
                    p.CodigoInterno,
                    p.Nombre,
                    p.Categoria,
                    p.ClaveProdServSATID,
                    p.UnidadSAT,
                    TasaIVA = p.TasaIVA,
                    TasaIEPS = p.TasaIEPS > 0 ? p.TasaIEPS : (decimal?)null,
                    Stock = stocks[p.ProductoID],
                    p.Exento,
                    p.Estatus
                }).ToList();

                return Json(new { data = listaFinal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<object>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerPorId(int id)
        {
            var p = CD_Producto.Instancia.ObtenerPorId(id);
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Guardar(Producto p)
        {
            p.Usuario = User.Identity.Name ?? "system";
            bool resultado = p.ProductoID == 0
                ? CD_Producto.Instancia.AltaProducto(p)
                : CD_Producto.Instancia.ActualizarProducto(p);

            return Json(new { success = resultado });
        }

        [HttpPost]
        public JsonResult CambiarEstatus(int id, bool estatus)
        {
            bool resultado = CD_Producto.Instancia.CambiarEstatusProducto(id, estatus);
            return Json(new { resultado });
        }

        [HttpGet]
        public JsonResult ObtenerCatalogos()
        {
            var cat = CD_Catalogos.Instancia;
            return Json(new
            {
                categorias = cat.ObtenerCategorias(),
                clavesSAT = cat.ObtenerClavesProdServ(),
                unidadesSAT = cat.ObtenerUnidadesSAT(),
                tasasIVA = cat.ObtenerTasasIVA(),
                tasasIEPS = cat.ObtenerTasasIEPS()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerPorSucursal(int sucursalId)
        {
            var productos = CD_Producto.Instancia.BuscarProductos("");
            return Json(new { data = productos }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerLotes(int productoId)
        {
            var lotes = CD_Producto.Instancia.ObtenerLotes(productoId);
            return Json(lotes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearLote(int productoId)
        {
            ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(productoId).Nombre;
            return View(new LoteProducto { ProductoID = productoId });
        }

        [HttpPost]
        public ActionResult CrearLote(LoteProducto lote)
        {
            if (ModelState.IsValid)
            {
                lote.Usuario = User.Identity.Name ?? "system";
                if (CD_Producto.Instancia.RegistrarLote(lote))
                {
                    TempData["Success"] = "Lote registrado correctamente";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(lote.ProductoID).Nombre;
            return View(lote);
        }
        [HttpGet]
        public ActionResult EditarLote(int loteId)
        {
            var lote = CD_Producto.Instancia.ObtenerLotePorId(loteId);

            if (lote == null)
            {
                TempData["Error"] = "El lote no existe o fue eliminado.";
                return RedirectToAction("Index");
            }

            ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(lote.ProductoID)?.Nombre ?? "Sin nombre";
            return View(lote);
        }
        [HttpPost]
        public ActionResult EditarLote(LoteProducto lote, string motivoAjuste)
        {
            if (ModelState.IsValid)
            {
                // Obtener lote original para comparar cantidades
                var loteOriginal = CD_Producto.Instancia.ObtenerLotePorId(lote.LoteID);
                if (loteOriginal == null)
                {
                    TempData["Error"] = "El lote no existe";
                    return RedirectToAction("Index");
                }

                int diferenciaCantidad = lote.CantidadTotal - loteOriginal.CantidadTotal;
                lote.Usuario = User.Identity.Name ?? "system";

                // Si hay cambio en la cantidad, registrar movimiento y póliza
                if (diferenciaCantidad != 0)
                {
                    if (string.IsNullOrWhiteSpace(motivoAjuste))
                    {
                        ModelState.AddModelError("", "Debe proporcionar un motivo para el ajuste de inventario");
                        ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(lote.ProductoID)?.Nombre;
                        return View(lote);
                    }

                    // Ajustar CantidadDisponible proporcionalmente
                    decimal porcentajeDisponible = loteOriginal.CantidadTotal > 0 
                        ? (decimal)loteOriginal.CantidadDisponible / loteOriginal.CantidadTotal 
                        : 0;
                    lote.CantidadDisponible = (int)(lote.CantidadTotal * porcentajeDisponible);

                    // Registrar en bitácora de movimientos
                    var movimiento = new MovimientoInventario
                    {
                        LoteID = lote.LoteID,
                        ProductoID = lote.ProductoID,
                        TipoMovimiento = diferenciaCantidad > 0 ? "AJUSTE_ENTRADA" : "AJUSTE_SALIDA",
                        Cantidad = Math.Abs(diferenciaCantidad),
                        CostoUnitario = lote.PrecioCompra,
                        Usuario = lote.Usuario,
                        Fecha = DateTime.Now,
                        Comentarios = motivoAjuste
                    };
                    CD_Producto.Instancia.RegistrarMovimientoInventario(movimiento);

                    // Crear póliza contable
                    CrearPolizaAjusteInventario(lote, diferenciaCantidad, motivoAjuste);
                }

                if (CD_Producto.Instancia.ActualizarLote(lote))
                {
                    TempData["Success"] = diferenciaCantidad != 0 
                        ? $"Lote actualizado correctamente. Ajuste de {(diferenciaCantidad > 0 ? "+" : "")}{diferenciaCantidad} unidades registrado."
                        : "Lote actualizado correctamente";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(lote.ProductoID).Nombre;
            return View(lote);
        }

        private void CrearPolizaAjusteInventario(LoteProducto lote, int diferenciaCantidad, string motivo)
        {
            try
            {
                decimal costoTotal = Math.Abs(diferenciaCantidad) * lote.PrecioCompra;
                bool esIncremento = diferenciaCantidad > 0;
                
                // Determinar tipo de póliza según el motivo
                string tipoPoliza = esIncremento ? "AJUSTE_ENTRADA" : 
                    (motivo.Contains("CADUCIDAD") || motivo.Contains("MERMA") || motivo.Contains("DAÑADO") 
                        ? "MERMA" : "AJUSTE_SALIDA");

                var poliza = new Poliza
                {
                    FechaPoliza = DateTime.Now,
                    TipoPoliza = tipoPoliza,
                    Concepto = $"Ajuste de inventario Lote #{lote.LoteID}: {motivo}",
                    Referencia = $"LOTE-{lote.LoteID}",
                    Usuario = User.Identity.Name ?? "Sistema"
                };

                if (esIncremento)
                {
                    // Incremento: DEBE Inventario, HABER Ajuste Inventario
                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ConvertirIntAGuid(1), // Inventario (Activo)
                        Debe = costoTotal,
                        Haber = 0,
                        Concepto = $"Incremento inventario: {Math.Abs(diferenciaCantidad)} unidades"
                    });
                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ConvertirIntAGuid(50), // Ajustes de Inventario (Ingresos)
                        Debe = 0,
                        Haber = costoTotal,
                        Concepto = $"Contrapartida ajuste inventario"
                    });
                }
                else
                {
                    // Disminución: DEBE Pérdida/Merma, HABER Inventario
                    int cuentaContrapartida = motivo.Contains("CADUCIDAD") || motivo.Contains("MERMA") || motivo.Contains("DAÑADO")
                        ? 60 // Costo de Ventas / Mermas
                        : 50; // Ajustes de Inventario

                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ConvertirIntAGuid(cuentaContrapartida),
                        Debe = costoTotal,
                        Haber = 0,
                        Concepto = $"Salida inventario por {tipoPoliza}"
                    });
                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ConvertirIntAGuid(1), // Inventario (Activo)
                        Debe = 0,
                        Haber = costoTotal,
                        Concepto = $"Disminución inventario: {Math.Abs(diferenciaCantidad)} unidades"
                    });
                }

                bool polizaCreada = CD_Poliza.Instancia.CrearPoliza(poliza);
                if (!polizaCreada)
                {
                    TempData["Warning"] = "El ajuste se registró pero hubo un error al crear la póliza contable";
                }
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"El ajuste se registró pero hubo un error al crear la póliza: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error al crear póliza: {ex.Message}");
            }
        }

        // Nueva acción para ajustar lote (merma/ajuste) - Modal o vista separada
        [HttpPost]
        public JsonResult AjustarLote(int loteId, decimal cantidadAjuste, string tipo, string motivo)
        {
            var lote = CD_Producto.Instancia.ObtenerLotePorId(loteId);
            if (lote == null || cantidadAjuste <= 0 || lote.CantidadDisponible < cantidadAjuste)
                return Json(new { success = false, message = "Ajuste inválido" });

            var ajuste = new AjusteInventario
            {
                ProductoId = lote.ProductoID,
                LoteEntradaId = loteId,
                Tipo = tipo,
                Cantidad = cantidadAjuste * -1,
                Motivo = motivo,
                UsuarioId = 1 // Reemplaza por ID real
            };
            bool registrado = CD_Producto.Instancia.RegistrarAjuste(ajuste);

            if (registrado)
            {
                lote.CantidadDisponible -= (int)cantidadAjuste;
                CD_Producto.Instancia.ActualizarLote(lote);

                decimal costo = lote.PrecioCompra * cantidadAjuste;

                var poliza = new Poliza
                {
                    TipoPoliza = tipo == "MERMA" ? "MERMA" : "AJUSTE",
                    FechaPoliza = DateTime.Now,
                    Concepto = string.Format("{0} de lote {1}: {2}", tipo, loteId, motivo),
                    ReferenciaId = loteId,
                    ReferenciaTipo = "LOTE"
                };
                poliza.Detalles.Add(new PolizaDetalle
                {
                    CuentaID = ConvertirIntAGuid(1), // ID cuenta 'Inventario'
                    Haber = costo,
                    Concepto = "Reducción inventario"
                });
                poliza.Detalles.Add(new PolizaDetalle
                {
                    CuentaID = ConvertirIntAGuid(2), // ID cuenta 'Gasto Merma/Ajuste'
                    Debe = costo,
                    Concepto = "Gasto por " + tipo
                });

                bool polizaCreada = CD_Poliza.Instancia.CrearPoliza(poliza);
                return Json(new { success = polizaCreada, message = "Ajuste registrado y póliza generada" });
            }
            return Json(new { success = false, message = "Error al registrar ajuste" });
        }

        // Helper para convertir int a Guid?
        private Guid? ConvertirIntAGuid(int id)
        {
            if (id <= 0) return null;
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        // Bitácora de movimientos de inventario
        [HttpGet]
        public ActionResult BitacoraInventario(int? productoId, int? loteId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var movimientos = CD_Producto.Instancia.ObtenerMovimientosInventario(productoId, loteId, fechaInicio, fechaFin);
            ViewBag.ProductoId = productoId;
            ViewBag.LoteId = loteId;
            return View(movimientos);
        }
    }
}
