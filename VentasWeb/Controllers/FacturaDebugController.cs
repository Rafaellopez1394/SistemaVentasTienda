using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using System.Threading.Tasks;

namespace VentasWeb.Controllers
{
    public class FacturaDebugController : Controller
    {
        // GET: FacturaDebug/TestGenerar
        public async Task<ContentResult> TestGenerar(string ventaId = "6bc16123-7b85-418e-a4aa-62384726aa44")
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TEST GENERACION DE FACTURA ===");
            sb.AppendLine($"VentaID: {ventaId}");
            sb.AppendLine($"Fecha: {DateTime.Now}");
            sb.AppendLine();

            try
            {
                var request = new GenerarFacturaRequest
                {
                    VentaID = Guid.Parse(ventaId),
                    ReceptorRFC = "XAXX010101000",
                    ReceptorNombre = "PUBLICO GENERAL",
                    ReceptorRegimenFiscal = "616",
                    ReceptorUsoCFDI = "G03",
                    FormaPago = "01",
                    MetodoPago = "PUE"
                };

                sb.AppendLine("[1] Request creado:");
                sb.AppendLine($"   RFC: {request.ReceptorRFC}");
                sb.AppendLine($"   Nombre: {request.ReceptorNombre}");
                sb.AppendLine($"   UsoCFDI: {request.ReceptorUsoCFDI}");
                sb.AppendLine();

                sb.AppendLine("[2] Llamando a CrearFacturaDesdeVenta...");
                
                string mensajeCrear;
                var factura = CD_Factura.Instancia.CrearFacturaDesdeVenta(request, out mensajeCrear);
                
                if (factura == null)
                {
                    sb.AppendLine($"   [ERROR] No se pudo crear factura: {mensajeCrear}");
                    return Content(sb.ToString(), "text/plain");
                }

                sb.AppendLine($"   [OK] Factura creada: {factura.FacturaID}");
                sb.AppendLine($"   Serie: {factura.Serie}");
                sb.AppendLine($"   Folio: {factura.Folio}");
                sb.AppendLine($"   Subtotal: {factura.Subtotal}");
                sb.AppendLine($"   Total: {factura.Total}");
                sb.AppendLine($"   Conceptos: {factura.Conceptos?.Count ?? 0}");
                sb.AppendLine();

                if (factura.Conceptos != null)
                {
                    sb.AppendLine("[3] Detalles de Conceptos:");
                    foreach (var c in factura.Conceptos)
                    {
                        sb.AppendLine($"   - {c.Descripcion}");
                        sb.AppendLine($"     Cantidad: {c.Cantidad}");
                        sb.AppendLine($"     Precio: {c.ValorUnitario}");
                        sb.AppendLine($"     Importe: {c.Importe}");
                        sb.AppendLine($"     ClaveProdServ: {c.ClaveProdServ}");
                        sb.AppendLine($"     ClaveUnidad: {c.ClaveUnidad}");
                        sb.AppendLine($"     ObjetoImp: {c.ObjetoImp}");
                        sb.AppendLine($"     Impuestos: {c.Impuestos?.Count ?? 0}");
                    }
                    sb.AppendLine();
                }

                sb.AppendLine("[4] Generando XML...");
                
                var generadorXML = new CapaDatos.Generadores.CFDI40XMLGenerator();
                
                try
                {
                    string xml = generadorXML.GenerarXML(factura);
                    
                    if (string.IsNullOrWhiteSpace(xml))
                    {
                        sb.AppendLine("   [ERROR] XML vacio o null");
                    }
                    else
                    {
                        sb.AppendLine($"   [OK] XML generado. Longitud: {xml.Length} caracteres");
                        sb.AppendLine();
                        sb.AppendLine("[5] Primeros 1000 caracteres del XML:");
                        sb.AppendLine(xml.Substring(0, Math.Min(1000, xml.Length)));
                        sb.AppendLine();
                        sb.AppendLine($"[6] Ultimos 500 caracteres del XML:");
                        if (xml.Length > 500)
                        {
                            sb.AppendLine(xml.Substring(xml.Length - 500));
                        }
                        else
                        {
                            sb.AppendLine(xml);
                        }
                    }
                }
                catch (Exception exXml)
                {
                    sb.AppendLine($"   [ERROR] Exception al generar XML:");
                    sb.AppendLine($"   Message: {exXml.Message}");
                    sb.AppendLine($"   StackTrace: {exXml.StackTrace}");
                    if (exXml.InnerException != null)
                    {
                        sb.AppendLine($"   InnerException: {exXml.InnerException.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"[ERROR GENERAL]:");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"InnerException: {ex.InnerException.Message}");
                }
            }

            return Content(sb.ToString(), "text/plain");
        }
    }
}
