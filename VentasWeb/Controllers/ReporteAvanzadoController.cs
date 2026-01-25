using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class ReporteAvanzadoController : Controller
    {
        // GET: ReporteAvanzado/Index - Dashboard principal
        public ActionResult Index()
        {
            return View();
        }

        #region VISTAS DE REPORTES

        // GET: ReporteAvanzado/UtilidadProductos
        public ActionResult UtilidadProductos()
        {
            return View();
        }

        // GET: ReporteAvanzado/EstadoResultados
        public ActionResult EstadoResultados()
        {
            return View();
        }

        // GET: ReporteAvanzado/RecuperacionCredito
        public ActionResult RecuperacionCredito()
        {
            return View();
        }

        // GET: ReporteAvanzado/CarteraClientes
        public ActionResult CarteraClientes()
        {
            return View();
        }

        #endregion

        #region API ENDPOINTS - UTILIDAD DE PRODUCTOS

        /// <summary>
        /// API: Obtener reporte de utilidad por producto
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerUtilidadProductos(string fechaInicio, string fechaFin, int? productoID = null, int? categoriaID = null, int? sucursalID = null)
        {
            try
            {
                DateTime dtInicio = Convert.ToDateTime(fechaInicio);
                DateTime dtFin = Convert.ToDateTime(fechaFin);

                // Si no se especifica sucursal, usar la activa
                int sucursal = sucursalID ?? (Session["SucursalActiva"] != null ? (int)Session["SucursalActiva"] : 0);

                List<ReporteUtilidadProducto> lista = CD_ReportesAvanzados.Instancia.ObtenerUtilidadProductos(
                    dtInicio, dtFin, productoID, categoriaID, sucursal > 0 ? sucursal : (int?)null);

                return Json(new { success = true, data = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Exportar utilidad de productos a Excel
        /// </summary>
        [HttpGet]
        public ActionResult ExportarUtilidadProductos(string fechaInicio, string fechaFin, int? sucursalID = null)
        {
            try
            {
                // TODO: Implementar exportación a Excel cuando se instale EPPlus
                return Content("Funcionalidad de exportación a Excel próximamente disponible. Por favor use el botón de copiar de la tabla.");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        #endregion

        #region API ENDPOINTS - ESTADO DE RESULTADOS

        /// <summary>
        /// API: Generar Estado de Resultados (P&L)
        /// </summary>
        [HttpPost]
        public JsonResult GenerarEstadoResultados(string fechaInicio, string fechaFin, int? sucursalID = null)
        {
            try
            {
                DateTime dtInicio = Convert.ToDateTime(fechaInicio);
                DateTime dtFin = Convert.ToDateTime(fechaFin);

                int sucursal = sucursalID ?? (Session["SucursalActiva"] != null ? (int)Session["SucursalActiva"] : 0);

                EstadoResultadosVentas er = CD_ReportesAvanzados.Instancia.GenerarEstadoResultados(
                    dtInicio, dtFin, sucursal > 0 ? sucursal : (int?)null);

                return Json(new { success = true, data = er });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        #endregion

        #region API ENDPOINTS - CRÉDITO Y COBRANZA

        /// <summary>
        /// API: Obtener concentrado de recuperación de crédito
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerRecuperacionCredito(string fechaInicio, string fechaFin, int? sucursalID = null)
        {
            try
            {
                DateTime dtInicio = Convert.ToDateTime(fechaInicio);
                DateTime dtFin = Convert.ToDateTime(fechaFin);

                int sucursal = sucursalID ?? (Session["SucursalActiva"] != null ? (int)Session["SucursalActiva"] : 0);

                List<ReporteRecuperacionCredito> lista = CD_ReportesAvanzados.Instancia.ConcentradoRecuperacion(
                    dtInicio, dtFin, sucursal > 0 ? sucursal : (int?)null);

                return Json(new { success = true, data = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// API: Obtener cartera de clientes
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerCarteraClientes(DateTime? fechaCorte = null, int? sucursalID = null)
        {
            try
            {
                DateTime corte = fechaCorte ?? DateTime.Now;
                int sucursal = sucursalID ?? (Session["SucursalActiva"] != null ? (int)Session["SucursalActiva"] : 0);

                List<ReporteCarteraCliente> lista = CD_ReportesAvanzados.Instancia.ObtenerCartera(
                    corte, sucursal > 0 ? sucursal : (int?)null);

                return Json(new { success = true, data = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region API ENDPOINTS - DASHBOARD

        /// <summary>
        /// API: Obtener KPIs para dashboard
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerKPIs(DateTime? fecha = null, int? sucursalID = null)
        {
            try
            {
                DateTime fechaConsulta = fecha ?? DateTime.Now;
                int sucursal = sucursalID ?? (Session["SucursalActiva"] != null ? (int)Session["SucursalActiva"] : 0);

                DashboardKPIs kpis = CD_ReportesAvanzados.Instancia.ObtenerKPIs(
                    fechaConsulta, sucursal > 0 ? sucursal : (int?)null);

                return Json(new { success = true, data = kpis }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
