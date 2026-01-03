// VentasWeb/Controllers/CertificadoDigitalController.cs
using CapaDatos;
using CapaModelo;
using System;
using System.Web;
using System.Web.Mvc;

namespace VentasWeb.Controllers
{
    public class CertificadoDigitalController : Controller
    {
        // GET: Vista principal
        public ActionResult Index()
        {
            return View();
        }

        // GET: Obtener todos los certificados
        [HttpGet]
        public JsonResult ObtenerTodos()
        {
            try
            {
                var certificados = CD_CertificadoDigital.Instancia.ObtenerTodos();
                return Json(new { success = true, data = certificados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener certificado predeterminado
        [HttpGet]
        public JsonResult ObtenerPredeterminado()
        {
            try
            {
                var certificado = CD_CertificadoDigital.Instancia.ObtenerPredeterminado();
                
                if (certificado == null)
                    return Json(new { success = false, mensaje = "No hay certificado predeterminado" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = certificado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cargar nuevo certificado
        [HttpPost]
        public JsonResult CargarCertificado()
        {
            try
            {
                // Validar que se subieron archivos
                if (Request.Files.Count < 2)
                    return Json(new { success = false, mensaje = "Debe proporcionar ambos archivos (.cer y .key)" });

                HttpPostedFileBase archivoCER = Request.Files["archivoCER"];
                HttpPostedFileBase archivoKEY = Request.Files["archivoKEY"];

                if (archivoCER == null || archivoKEY == null)
                    return Json(new { success = false, mensaje = "Debe seleccionar ambos archivos" });

                if (archivoCER.ContentLength == 0 || archivoKEY.ContentLength == 0)
                    return Json(new { success = false, mensaje = "Los archivos están vacíos" });

                // Validar extensiones
                if (!archivoCER.FileName.EndsWith(".cer", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, mensaje = "El archivo del certificado debe ser .cer" });

                if (!archivoKEY.FileName.EndsWith(".key", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, mensaje = "El archivo de la llave debe ser .key" });

                // Leer archivos a bytes
                byte[] bytesCER = new byte[archivoCER.ContentLength];
                archivoCER.InputStream.Read(bytesCER, 0, archivoCER.ContentLength);

                byte[] bytesKEY = new byte[archivoKEY.ContentLength];
                archivoKEY.InputStream.Read(bytesKEY, 0, archivoKEY.ContentLength);

                // Crear request
                var request = new CargarCertificadoRequest
                {
                    NombreCertificado = Request.Form["nombreCertificado"],
                    RFC = Request.Form["rfc"],
                    RazonSocial = Request.Form["razonSocial"],
                    PasswordKEY = Request.Form["passwordKey"],
                    ArchivoCER = bytesCER,
                    ArchivoKEY = bytesKEY,
                    EsPredeterminado = Request.Form["esPredeterminado"] == "true",
                    Usuario = User.Identity.Name ?? "system"
                };

                // Validaciones
                if (string.IsNullOrEmpty(request.NombreCertificado))
                    return Json(new { success = false, mensaje = "Debe proporcionar un nombre" });

                if (string.IsNullOrEmpty(request.RFC))
                    return Json(new { success = false, mensaje = "Debe proporcionar el RFC" });

                if (string.IsNullOrEmpty(request.RazonSocial))
                    return Json(new { success = false, mensaje = "Debe proporcionar la razón social" });

                if (string.IsNullOrEmpty(request.PasswordKEY))
                    return Json(new { success = false, mensaje = "Debe proporcionar la contraseña de la llave privada" });

                // Cargar certificado
                var respuesta = CD_CertificadoDigital.Instancia.CargarCertificado(request);

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje,
                    data = respuesta.Datos
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // POST: Establecer como predeterminado
        [HttpPost]
        public JsonResult EstablecerPredeterminado(int certificadoID)
        {
            try
            {
                var respuesta = CD_CertificadoDigital.Instancia.EstablecerPredeterminado(
                    certificadoID,
                    User.Identity.Name ?? "system"
                );

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // POST: Eliminar certificado
        [HttpPost]
        public JsonResult Eliminar(int certificadoID)
        {
            try
            {
                var respuesta = CD_CertificadoDigital.Instancia.Eliminar(
                    certificadoID,
                    User.Identity.Name ?? "system"
                );

                return Json(new
                {
                    success = respuesta.Resultado,
                    mensaje = respuesta.Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        // GET: Validar vigencia
        [HttpGet]
        public JsonResult ValidarVigencia()
        {
            try
            {
                var certificadosPorVencer = CD_CertificadoDigital.Instancia.ValidarVigencia();
                return Json(new { success = true, data = certificadosPorVencer }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
