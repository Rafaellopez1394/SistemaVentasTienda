using System;
using System.Linq;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    //[Authorize]
    [CustomAuthorize]
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Obtener()
        {
            var lista = CD_Cliente.Instancia.ObtenerTodos();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerPorId(Guid id)
        {
            var cliente = CD_Cliente.Instancia.ObtenerPorId(id);
            if (cliente == null)
                return Json(new { success = false, message = "Cliente no encontrado" }, JsonRequestBehavior.AllowGet);

            var creditos = CD_Cliente.Instancia.ObtenerTiposCreditoCliente(id);
            
            // Agregar información de crédito actual
            cliente.CreditoActivo = creditos.Any(c => c.Estatus);
            cliente.LimiteCreditoActual = CD_Cliente.Instancia.ObtenerLimiteCreditoTotal(id);
            cliente.SaldoCreditoActual = CD_Cliente.Instancia.ObtenerSaldoActual(id);
            cliente.DiasVencidos = CD_Cliente.Instancia.ObtenerDiasVencidos(id);

            return Json(new
            {
                success = true,
                cliente = cliente,
                creditos = creditos,
                creditoDisponible = CD_Cliente.Instancia.ObtenerCreditoDisponible(id),
                saldoVencido = CD_Cliente.Instancia.ObtenerSaldoVencido(id),
                historialReciente = CD_Cliente.Instancia.ObtenerHistorialCredito(id, 5)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Guardar(ClienteSavePayload payload)
        {
            bool resultado = false;
            string mensaje = "";

            try
            {
                if (payload?.objeto == null)
                {
                    return Json(new { resultado = false, mensaje = "Datos de cliente requeridos" }, JsonRequestBehavior.AllowGet);
                }

                var objeto = payload.objeto;
                objeto.Usuario = User.Identity.Name ?? "system";

                // 1. GUARDAR CLIENTE
                if (objeto.ClienteID == Guid.Empty)
                    resultado = CD_Cliente.Instancia.AltaCliente(objeto);
                else
                    resultado = CD_Cliente.Instancia.ActualizarCliente(objeto);

                if (!resultado)
                {
                    mensaje = "Error al guardar cliente";
                    return Json(new { resultado, mensaje }, JsonRequestBehavior.AllowGet);
                }

                // 2. GUARDAR TIPOS DE CRÉDITO ASIGNADOS CON LÍMITES
                if (payload.creditosConLimites != null && payload.creditosConLimites.Count > 0)
                {
                    // Primero, eliminar créditos anteriores
                    using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                    {
                        var cmdDelete = new System.Data.SqlClient.SqlCommand(
                            "DELETE FROM ClienteTiposCredito WHERE ClienteID = @ClienteID", cnx);
                        cmdDelete.Parameters.AddWithValue("@ClienteID", objeto.ClienteID);
                        cnx.Open();
                        cmdDelete.ExecuteNonQuery();
                    }

                    // Luego, insertar los nuevos con sus límites
                    foreach (var credito in payload.creditosConLimites)
                    {
                        CD_TipoCredito.Instancia.AsignarCreditoCliente(
                            objeto.ClienteID, 
                            credito.tipoCreditoID,
                            credito.limiteDinero,
                            credito.limiteProducto,
                            credito.plazoDias
                        );
                    }
                }

                mensaje = "Guardado correctamente";
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message;
            }

            return Json(new { resultado, mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Eliminar(Guid id)
        {
            bool resultado = false;
            string mensaje = "";

            try
            {
                using (var cnx = new System.Data.SqlClient.SqlConnection(Conexion.CN))
                {
                    var cmd = new System.Data.SqlClient.SqlCommand("UPDATE Clientes SET Estatus = 0, Usuario = @Usuario, UltimaAct = GETDATE() WHERE ClienteID = @ClienteID", cnx);
                    cmd.Parameters.AddWithValue("@ClienteID", id);
                    cmd.Parameters.AddWithValue("@Usuario", User.Identity.Name ?? "system");
                    cnx.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                    mensaje = resultado ? "Cliente dado de baja" : "No se pudo dar de baja";
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return Json(new { resultado, mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerCatalogos()
        {
            var regimenes = CD_Catalogos.Instancia.ObtenerRegimenesFiscales();
            var usosCFDI = CD_Catalogos.Instancia.ObtenerUsosCFDI();
            var tiposCredito = CD_Catalogos.Instancia.ObtenerTiposCredito();

            return Json(new { regimenes, usosCFDI, tiposCredito }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerInfoCredito(Guid id)
        {
            try
            {
                var cliente = CD_Cliente.Instancia.ObtenerPorId(id);
                if (cliente == null)
                    return Json(new { success = false, message = "Cliente no encontrado" }, JsonRequestBehavior.AllowGet);

                decimal limiteCreditoTotal = CD_Cliente.Instancia.ObtenerLimiteCreditoTotal(id);
                decimal saldoActual = CD_Cliente.Instancia.ObtenerSaldoActual(id);
                decimal saldoVencido = CD_Cliente.Instancia.ObtenerSaldoVencido(id);
                decimal creditoDisponible = CD_Cliente.Instancia.ObtenerCreditoDisponible(id);
                int diasVencidos = CD_Cliente.Instancia.ObtenerDiasVencidos(id);
                var historial = CD_Cliente.Instancia.ObtenerHistorialCredito(id, 10);

                return Json(new
                {
                    success = true,
                    razonSocial = cliente.RazonSocial,
                    rfc = cliente.RFC,
                    limiteCreditoTotal = limiteCreditoTotal,
                    saldoActual = saldoActual,
                    saldoVencido = saldoVencido,
                    creditoDisponible = creditoDisponible,
                    diasVencidos = diasVencidos,
                    porcentajeUtilizado = limiteCreditoTotal > 0 ? (saldoActual / limiteCreditoTotal * 100).ToString("F2") : "0",
                    historial = historial,
                    puedeComprar = creditoDisponible > 0
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}