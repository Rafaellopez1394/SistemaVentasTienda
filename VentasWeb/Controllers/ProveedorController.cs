// Controllers/ProveedorController.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    public class ProveedorController : Controller
    {
        // GET: Proveedor
        public ActionResult Index()
        {
            return View();
        }

        // Obtener todos los proveedores
        [HttpGet]
        public JsonResult Obtener()
        {
            try
            {
                var lista = CD_Proveedor.Instancia.ObtenerTodos();
                return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<Proveedor>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Guardar nuevo o actualizar proveedor
        [HttpPost]
        public JsonResult Guardar(Proveedor objeto)
        {
            bool resultado = false;
            objeto.Usuario = User.Identity.Name ?? "system";
            objeto.UltimaAct = DateTime.Now;

            if (objeto.ProveedorID == Guid.Empty)
            {
                // Nuevo proveedor
                resultado = CD_Proveedor.Instancia.AltaProveedor(objeto);
            }
            else
            {
                // Actualizar existente
                resultado = CD_Proveedor.Instancia.ActualizarProveedor(objeto);
            }

            return Json(new { resultado }, JsonRequestBehavior.AllowGet);
        }

        // Cambiar estatus (dar de baja lógica)
        [HttpPost]
        public JsonResult CambiarEstatus(Guid id)
        {
            bool resultado = false;
            try
            {
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    var cmd = new System.Data.SqlClient.SqlCommand("CambiarEstatusProveedor", cnx);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProveedorID", id);
                    cmd.Parameters.AddWithValue("@Estatus", false);
                    cmd.Parameters.AddWithValue("@Usuario", User.Identity.Name ?? "system");
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    resultado = true;
                }
            }
            catch { resultado = false; }

            return Json(new { resultado }, JsonRequestBehavior.AllowGet);
        }

        // Catálogos para los selects del formulario
        [HttpGet]
        public JsonResult ObtenerCatalogos()
        {
            var regimenes = CD_Catalogos.Instancia.ObtenerRegimenes();
            var bancos = CD_Catalogos.Instancia.ObtenerBancos();
            var tiposProveedor = CD_Catalogos.Instancia.ObtenerTiposProveedor();

            return Json(new { regimenes, bancos, tiposProveedor }, JsonRequestBehavior.AllowGet);
        }
    }
}