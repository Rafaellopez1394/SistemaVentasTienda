using Fiscalapi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PDF
{
    public class FiscalAPIDescargaMasiva
    {
        /// <summary>
        /// Descarga masiva de CFDI y metadatos desde el SAT usando FiscalAPI SDK.
        /// </summary>
        /// <param name="config">Configuración FiscalAPI</param>
        /// <param name="fechaInicio">Fecha inicial (yyyy-MM-dd)</param>
        /// <param name="fechaFin">Fecha final (yyyy-MM-dd)</param>
        /// <param name="rfcEmisor">RFC del emisor (opcional)</param>
        /// <param name="tipo">Tipo de CFDI: I=Ingreso, E=Egreso, P=Pago, T=Traslado, N=Nómina (opcional)</param>
        /// <param name="descargarXML">Si true, descarga ZIP con XML; si false, solo metadatos</param>
        /// <param name="rutaZip">Ruta donde guardar el ZIP (si descargarXML=true)</param>
        /// <returns>Lista de metadatos de CFDI descargados</returns>
        public static async Task<List<CfdiMetadata>> DescargarMasivoAsync(
            ConfiguracionPAC config,
            DateTime fechaInicio,
            DateTime fechaFin,
            string rfcEmisor = null,
            string tipo = null,
            bool descargarXML = false,
            string rutaZip = null)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);

            // 1. Buscar metadatos
            var filtros = new Dictionary<string, string>
            {
                { "dateFrom", fechaInicio.ToString("yyyy-MM-dd") },
                { "dateTo", fechaFin.ToString("yyyy-MM-dd") }
            };
            if (!string.IsNullOrWhiteSpace(rfcEmisor)) filtros["issuerTin"] = rfcEmisor;
            if (!string.IsNullOrWhiteSpace(tipo)) filtros["typeCode"] = tipo;

            var metaResponse = await client.SatCfdi.SearchAsync(filtros);
            var lista = new List<CfdiMetadata>();
            if (metaResponse.Succeeded && metaResponse.Data != null)
            {
                foreach (var item in metaResponse.Data.Items)
                {
                    lista.Add(new CfdiMetadata
                    {
                        Uuid = item.Uuid,
                        Fecha = item.Date,
                        RfcEmisor = item.IssuerTin,
                        RfcReceptor = item.RecipientTin,
                        Total = item.Total,
                        Tipo = item.TypeCode
                    });
                }
            }

            // 2. Descargar ZIP con XML si se solicita
            if (descargarXML && !string.IsNullOrWhiteSpace(rutaZip))
            {
                var zipResponse = await client.SatCfdi.DownloadZipAsync(filtros);
                if (zipResponse.Succeeded && zipResponse.Data != null)
                {
                    File.WriteAllBytes(rutaZip, zipResponse.Data);
                }
            }

            return lista;
        }
    }

    public class CfdiMetadata
    {
        public string Uuid { get; set; }
        public DateTime Fecha { get; set; }
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public decimal Total { get; set; }
        public string Tipo { get; set; }
    }
}
