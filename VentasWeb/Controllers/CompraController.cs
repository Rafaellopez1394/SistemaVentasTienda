// Controllers/CompraController.cs
using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using System.Collections.Generic;

namespace VentasWeb.Controllers
{
    public class CompraController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Obtener todas las compras (para reporte)
        [HttpGet]
        public JsonResult Obtener()
        {
            var lista = CD_Compra.Instancia.ObtenerTodas();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        // Buscar proveedor por RFC o Razón Social
        [HttpGet]
        public JsonResult BuscarProveedor(string texto)
        {
            var proveedores = CD_Proveedor.Instancia.BuscarPorNombreORFC(texto);
            return Json(proveedores, JsonRequestBehavior.AllowGet);
        }

        // Registrar compra completa con lotes
        [HttpPost]
        public JsonResult RegistrarCompra(Compra compra)
        {
            compra.Usuario = User.Identity.Name ?? "system";

            bool resultado = false;
            string mensaje = "";

            try
            {
                resultado = CD_Compra.Instancia.RegistrarCompraConLotes(compra, compra.Usuario);
                mensaje = resultado ? "Compra registrada correctamente" : "Error al registrar la compra";
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message.Contains("RAISERROR") ? ex.Message : "Error interno del servidor";
            }

            return Json(new { resultado, mensaje }, JsonRequestBehavior.AllowGet);
        }

        // Obtener productos para autocompletado
        [HttpGet]
        public JsonResult ObtenerProductos(string termino = "")
        {
            var productos = CD_Producto.Instancia.BuscarProductos(termino);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }
    }
}