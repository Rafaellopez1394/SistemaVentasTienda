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
        public ActionResult EditarLote(LoteProducto lote)
        {
            if (ModelState.IsValid)
            {
                lote.Usuario = User.Identity.Name ?? "system";
                if (CD_Producto.Instancia.ActualizarLote(lote)) // Asume método basado en sp_Lote_Actualizar
                {
                    TempData["Success"] = "Lote actualizado correctamente";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProductoNombre = CD_Producto.Instancia.ObtenerPorId(lote.ProductoID).Nombre;
            return View(lote);
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
                    Concepto = $"{tipo} de lote {loteId}: {motivo}",
                    ReferenciaId = loteId,
                    ReferenciaTipo = "LOTE"
                };
                poliza.Detalles.Add(new PolizaDetalle
                {
                    CuentaID = 1, // ID cuenta 'Inventario'
                    Haber = costo,
                    Concepto = "Reducción inventario"
                });
                poliza.Detalles.Add(new PolizaDetalle
                {
                    CuentaID = 2, // ID cuenta 'Gasto Merma/Ajuste'
                    Debe = costo,
                    Concepto = "Gasto por " + tipo
                });

                bool polizaCreada = CD_Poliza.Instancia.CrearPoliza(poliza);
                return Json(new { success = polizaCreada, message = "Ajuste registrado y póliza generada" });
            }
            return Json(new { success = false, message = "Error al registrar ajuste" });
        }
    }
}
