using System;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult TestJson()
        {
            try
            {
                var testData = new
                {
                    message = "Test exitoso",
                    timestamp = DateTime.Now,
                    data = new[] { "Item1", "Item2", "Item3" }
                };

                var result = Json(testData, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new { error = ex.Message, stackTrace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
        }
    }
}
