using CapaDatos;
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Reporte
        public ActionResult Producto()
        {
            return View();
        }

        // GET: Reporte
        public ActionResult Ventas()
        {
            return View();
        }

        public JsonResult ObtenerProducto(int SucursalID, string codigoproducto)
        {
            List<ReporteProducto> lista = CD_Reportes.Instancia.ReporteProductoSucursal(SucursalID, codigoproducto);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObtenerVenta(string fechainicio, string fechafin, int SucursalID)
        {
            
            List<ReporteVenta> lista = CD_Reportes.Instancia.ReporteVenta(Convert.ToDateTime(fechainicio), Convert.ToDateTime(fechafin), SucursalID);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }


    }
}