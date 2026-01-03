using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BalorFinanciera.Contabilidad.Formas
{
    /// <summary>
    /// Summary description for DescargarArchivos
    /// </summary>
    
    public class DescargarArchivos : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string _ruta = context.Server.UrlDecode(context.Request["rutaDocumento"] ?? string.Empty);
                string _nombreArchivo = context.Server.UrlDecode(context.Request["nombreArchivo"] ?? string.Empty);

                string extensionArchivo = Path.GetExtension(_nombreArchivo);
                string rutaCompleta = Path.Combine(_ruta, _nombreArchivo);

                if (string.IsNullOrEmpty(_ruta) || string.IsNullOrEmpty(_nombreArchivo))
                {
                    context.Response.Write("Invalid request parameters.");
                    return;
                }

                if (!File.Exists(rutaCompleta))
                {
                    context.Response.Write("File not found.");
                    return;
                }

                using (FileStream fs = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read))
                {
                    context.Response.Clear();
                    context.Response.Buffer = true;

                    switch (extensionArchivo.ToLower())
                    {
                        case ".pdf":
                            WriteFileToResponse(context, fs, "application/pdf");
                            break;
                        case ".xml":
                            WriteFileToResponse(context, fs, "application/xml");
                            break;
                        case ".jpg":
                        case ".jpeg":
                            WriteFileToResponse(context, fs, "image/jpeg");
                            break;
                        case ".png":
                            WriteFileToResponse(context, fs, "image/png");
                            break;
                        default:
                            context.Response.Write("Unsupported file type.");
                            return;
                    }
                    
                    context.Response.AddHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(_nombreArchivo)}");
                }
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/html";
                context.Response.Write("An error occurred: " + ex.Message);
            }
            finally
            {
                context.Response.Flush();
                context.Response.End();
            }
        }

        private void WriteFileToResponse(HttpContext context, FileStream fs, string contentType)
        {
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            context.Response.ContentType = contentType;
            context.Response.AddHeader("content-length", fs.Length.ToString());
            context.Response.BinaryWrite(buffer);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}