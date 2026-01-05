using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class TraspasoController : Controller
    {
        // GET: Traspaso/Index
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");
            return View();
        }

        // GET: Traspaso/Registrar
        public ActionResult Registrar()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");
            
            ViewBag.Sucursales = CD_Sucursal.Instancia.ObtenerSucursales();
            return View();
        }

        // POST: Traspaso/Registrar
        [HttpPost]
        public JsonResult Registrar(Traspaso traspaso)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                var usuario = (Usuario)Session["Usuario"];
                traspaso.UsuarioRegistro = usuario.UsuarioID.ToString();

                string mensaje;
                bool resultado = CD_Traspaso.Instancia.RegistrarTraspaso(traspaso, out mensaje);

                if (resultado)
                {
                    return Json(new { success = true, message = mensaje });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Traspaso/Detalle/5
        public ActionResult Detalle(int id)
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Login", "Acceso");

            var traspaso = CD_Traspaso.Instancia.ObtenerTraspasoPorID(id);
            if (traspaso == null)
                return HttpNotFound();

            return View(traspaso);
        }

        // POST: Traspaso/Enviar
        [HttpPost]
        public JsonResult Enviar(int traspasoID)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                var usuario = (Usuario)Session["Usuario"];
                string mensaje;
                bool resultado = CD_Traspaso.Instancia.EnviarTraspaso(traspasoID, usuario.UsuarioID.ToString(), out mensaje);

                return Json(new { success = resultado, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Traspaso/Recibir
        [HttpPost]
        public JsonResult Recibir(int traspasoID)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                var usuario = (Usuario)Session["Usuario"];
                string mensaje;
                bool resultado = CD_Traspaso.Instancia.RecibirTraspaso(traspasoID, usuario.UsuarioID.ToString(), out mensaje);

                return Json(new { success = resultado, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Traspaso/Cancelar
        [HttpPost]
        public JsonResult Cancelar(int traspasoID, string motivo)
        {
            try
            {
                if (Session["Usuario"] == null)
                    return Json(new { success = false, message = "Sesión expirada" });

                string mensaje;
                bool resultado = CD_Traspaso.Instancia.CancelarTraspaso(traspasoID, motivo, out mensaje);

                return Json(new { success = resultado, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Traspaso/ConsultarTraspasos
        public JsonResult ConsultarTraspasos(DateTime? fechaInicio, DateTime? fechaFin, int? sucursalID, string estatus)
        {
            try
            {
                var traspasos = CD_Traspaso.Instancia.ConsultarTraspasos(fechaInicio, fechaFin, sucursalID, estatus);
                
                var resultado = traspasos.Select(t => new
                {
                    t.TraspasoID,
                    t.FolioTraspaso,
                    FechaTraspaso = t.FechaTraspaso.ToString("dd/MM/yyyy"),
                    SucursalOrigen = t.SucursalOrigen?.Nombre ?? "",
                    SucursalDestino = t.SucursalDestino?.Nombre ?? "",
                    t.Estatus,
                    t.TotalProductos,
                    t.TotalCantidad,
                    ValorTotal = t.ValorTotal.ToString("N2")
                }).ToList();

                return Json(new { success = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Traspaso/ObtenerInventarioSucursal
        public JsonResult ObtenerInventarioSucursal(int? sucursalID, int? productoID)
        {
            try
            {
                var inventario = CD_Traspaso.Instancia.ObtenerInventarioSucursal(sucursalID, productoID);
                
                var resultado = inventario.Select(i => new
                {
                    i.ProductoID,
                    i.NombreProducto,
                    i.CodigoProducto,
                    i.UnidadMedida,
                    i.SucursalID,
                    i.NombreSucursal,
                    i.CantidadDisponible,
                    i.PrecioPromedioCompra
                }).ToList();

                return Json(new { success = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Traspaso/ObtenerDetalleTraspaso
        public JsonResult ObtenerDetalleTraspaso(int traspasoID)
        {
            try
            {
                var detalles = CD_Traspaso.Instancia.ObtenerDetalleTraspaso(traspasoID);
                
                var resultado = detalles.Select(d => new
                {
                    d.DetalleTraspasoID,
                    d.ProductoID,
                    d.NombreProducto,
                    d.CodigoProducto,
                    d.UnidadMedida,
                    CantidadSolicitada = String.Format("{0:N3}", d.CantidadSolicitada),
                    CantidadEnviada = String.Format("{0:N3}", d.CantidadEnviada),
                    CantidadRecibida = String.Format("{0:N3}", d.CantidadRecibida),
                    PrecioUnitario = String.Format("{0:N2}", d.PrecioUnitario),
                    Subtotal = String.Format("{0:N2}", d.Subtotal)
                }).ToList();

                return Json(new { success = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
