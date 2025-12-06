using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    [CustomAuthorize]
    public class CreditoController : Controller
    {
        // ==================================================================
        // LISTADO DE TIPOS DE CRÉDITO DISPONIBLES
        // ==================================================================
        public ActionResult Index()
        {
            try
            {
                var tipos = CapaDatos.CD_TipoCredito.Instancia.ObtenerTodos();
                return View(tipos);
            }
            catch (Exception ex)
            {
                // Mostrar error temporalmente en TempData para debug
                TempData["Error"] = "Error al obtener tipos de crédito: " + ex.Message + " | " + ex.InnerException?.Message;
                ViewBag.ErrorMessage = TempData["Error"];
                return View(new List<TipoCredito>()); // Retornar lista vacía en lugar de redirigir
            }
        }

        // ==================================================================
        // OBTENER CRÉDITOS DE UN CLIENTE (AJAX)
        // ==================================================================
        [HttpGet]
        public ActionResult ObtenerCreditosCliente(Guid clienteId)
        {
            try
            {
                var creditos = CapaDatos.CD_TipoCredito.Instancia.ObtenerCreditosCliente(clienteId);
                return Json(creditos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================================================================
        // OBTENER RESUMEN DE CRÉDITO DE CLIENTE
        // ==================================================================
        [HttpGet]
        public ActionResult ObtenerResumenCredito(Guid clienteId)
        {
            try
            {
                var resumen = CapaDatos.CD_TipoCredito.Instancia.ObtenerResumenCredito(clienteId);
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        clienteId = resumen.ClienteID,
                        razonSocial = resumen.RazonSocial,
                        tieneCreditoActivo = resumen.TieneCreditoActivo,
                        limiteDineroTotal = resumen.LimiteDineroTotal,
                        limiteProductoTotal = resumen.LimiteProductoTotal,
                        plazoDiasMaximo = resumen.PlazoDiasMaximo,
                        saldoDineroUtilizado = resumen.SaldoDineroUtilizado,
                        saldoProductoUtilizado = resumen.SaldoProductoUtilizado,
                        saldoDineroDisponible = resumen.SaldoDineroDisponible,
                        saldoProductoDisponible = resumen.SaldoProductoDisponible,
                        saldoVencido = resumen.SaldoVencido,
                        diasMaximoVencidos = resumen.DiasMaximoVencidos,
                        enAlarma = resumen.EnAlarma,
                        estado = resumen.Estado,
                        tiposAsignados = resumen.TiposAsignados
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================================================================
        // ASIGNAR CRÉDITO A CLIENTE
        // ==================================================================
        [HttpPost]
        public ActionResult AsignarCredito(Guid clienteId, int tipoCreditoId, 
            decimal? limiteDinero = null, int? limiteProducto = null, int? plazoDias = null)
        {
            try
            {
                // Validar que el cliente exista
                var cliente = CD_Cliente.Instancia.ObtenerPorId(clienteId);
                if (cliente == null)
                    return Json(new { success = false, error = "Cliente no encontrado" });

                // Validar que el tipo de crédito exista
                var tipoCreditoExiste = CapaDatos.CD_TipoCredito.Instancia.ObtenerPorId(tipoCreditoId);
                if (tipoCreditoExiste == null)
                    return Json(new { success = false, error = "Tipo de crédito no encontrado" });

                // Validar límites según criterio
                if (tipoCreditoExiste.Criterio == "Dinero" && (!limiteDinero.HasValue || limiteDinero <= 0))
                    return Json(new { success = false, error = "Debe especificar límite de dinero" });

                if (tipoCreditoExiste.Criterio == "Producto" && (!limiteProducto.HasValue || limiteProducto <= 0))
                    return Json(new { success = false, error = "Debe especificar límite de producto (unidades)" });

                if (tipoCreditoExiste.Criterio == "Tiempo" && (!plazoDias.HasValue || plazoDias <= 0))
                    return Json(new { success = false, error = "Debe especificar plazo en días" });

                // Asignar crédito
                bool resultado = CapaDatos.CD_TipoCredito.Instancia.AsignarCreditoCliente(
                    clienteId, tipoCreditoId, limiteDinero, limiteProducto, plazoDias);

                if (resultado)
                    return Json(new { success = true, message = "Crédito asignado correctamente" });
                else
                    return Json(new { success = false, error = "Error al asignar crédito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ==================================================================
        // ACTUALIZAR CRÉDITO DE CLIENTE
        // ==================================================================
        [HttpPost]
        public ActionResult ActualizarCredito(int clienteTipoCreditoId, 
            decimal? limiteDinero = null, int? limiteProducto = null, int? plazoDias = null, bool? activo = null)
        {
            try
            {
                bool resultado = CapaDatos.CD_TipoCredito.Instancia.ActualizarCreditoCliente(
                    clienteTipoCreditoId, limiteDinero, limiteProducto, plazoDias, activo);

                if (resultado)
                    return Json(new { success = true, message = "Crédito actualizado correctamente" });
                else
                    return Json(new { success = false, error = "Error al actualizar crédito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ==================================================================
        // SUSPENDER CRÉDITO DE CLIENTE
        // ==================================================================
        [HttpPost]
        public ActionResult SuspenderCredito(int clienteTipoCreditoId, bool suspender = true)
        {
            try
            {
                bool resultado = CapaDatos.CD_TipoCredito.Instancia.SuspenderCredito(clienteTipoCreditoId, suspender);

                if (resultado)
                {
                    string mensaje = suspender ? "Crédito suspendido" : "Crédito reactivado";
                    return Json(new { success = true, message = mensaje });
                }
                else
                    return Json(new { success = false, error = "Error al suspender crédito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ==================================================================
        // VALIDAR SI CLIENTE PUEDE USAR CRÉDITO (para ventas)
        // ==================================================================
        [HttpPost]
        public ActionResult ValidarCredito(Guid clienteId, int tipoCreditoId, decimal? montoSolicitado = null)
        {
            try
            {
                bool puedeUsar = CapaDatos.CD_TipoCredito.Instancia.PuedoUsarCredito(clienteId, tipoCreditoId, montoSolicitado);

                if (puedeUsar)
                    return Json(new { success = true, mensaje = "Cliente puede usar este crédito" });
                else
                    return Json(new { success = false, error = "Cliente no puede usar este crédito o no tiene saldo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
