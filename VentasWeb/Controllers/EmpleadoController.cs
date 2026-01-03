using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        public ActionResult Index()
        {
            return View();
        }

        // GET: Empleado/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // GET: Empleado/Editar
        public ActionResult Editar(int id)
        {
            return View();
        }

        // Aquí se agregarían los métodos para CRUD con base de datos
        // Por ahora solo las vistas básicas para evitar el error 404
    }
}
