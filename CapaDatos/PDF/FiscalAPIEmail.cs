using Fiscalapi;
using System;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PDF
{
    public class FiscalAPIEmail
    {
        /// <summary>
        /// Envía la factura por correo electrónico usando FiscalAPI SDK
        /// </summary>
        /// <param name="uuid">UUID del CFDI</param>
        /// <param name="config">Configuración FiscalAPI</param>
        /// <param name="email">Correo electrónico del destinatario</param>
        /// <returns>True si el envío fue exitoso, false si hubo error</returns>
        public static async Task<bool> EnviarFacturaPorEmailAsync(string uuid, ConfiguracionPAC config, string email)
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
                var apiResponse = await client.Invoices.SendByEmailAsync(uuid, email);
                return apiResponse.Succeeded;
            }
            catch
            {
                return false;
            }
        }
    }
}
