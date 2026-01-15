using Fiscalapi;
using System;
using System.IO;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PDF
{
    public class FiscalAPIPDF
    {
        /// <summary>
        /// Descarga el PDF de una factura timbrada usando FiscalAPI SDK
        /// </summary>
        /// <param name="uuid">UUID del CFDI</param>
        /// <param name="config">Configuración FiscalAPI</param>
        /// <param name="rutaDestino">Ruta donde guardar el PDF</param>
        /// <returns>True si se descargó correctamente, false si hubo error</returns>
        public static async Task<bool> DescargarPDFAsync(string uuid, ConfiguracionPAC config, string rutaDestino)
        {
            try
            {
                var settings = new FiscalApiOptions
                {
                    ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                    ApiKey = config.Usuario,
                    Tenant = config.Password
                };
                var client = FiscalApiClient.Create(settings);
                var pdfRequest = new CreatePdfRequest { InvoiceId = uuid };
                var apiResponse = await client.Invoices.GetPdfAsync(pdfRequest);
                if (apiResponse.Succeeded && apiResponse.Data != null)
                {
                    var pdfBytes = Convert.FromBase64String(apiResponse.Data.Base64Content);
                    File.WriteAllBytes(rutaDestino, pdfBytes);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
