using CapaModelo;
using Fiscalapi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CapaDatos.PDF
{
    /// <summary>
    /// Utilidad para consultar catálogos SAT vía FiscalAPI SDK.
    /// </summary>
    public class FiscalAPICatalogosSAT
    {
        /// <summary>
        /// Obtiene el catálogo de productos y servicios SAT.
        /// </summary>
        public static async Task<List<CatalogDto>> ObtenerCatalogoProdServSATAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                Tenant = config.Password ?? "default"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Catalogs.SearchCatalogAsync("SatProductCodes", "", 1, 1000);
            return result?.Data?.Items ?? new List<CatalogDto>();
        }

        /// <summary>
        /// Obtiene el catálogo de unidades SAT.
        /// </summary>
        public static async Task<List<CatalogDto>> ObtenerCatalogoUnidadesSATAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                Tenant = config.Password ?? "default"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Catalogs.SearchCatalogAsync("SatUnitMeasurements", "", 1, 1000);
            return result?.Data?.Items ?? new List<CatalogDto>();
        }

        /// <summary>
        /// Obtiene el catálogo de tasas de IVA SAT.
        /// </summary>
        public static async Task<List<CatalogDto>> ObtenerCatalogoTasasIVAAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                Tenant = config.Password ?? "default"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Catalogs.SearchCatalogAsync("SatTaxRates", "", 1, 1000);
            return result?.Data?.Items ?? new List<CatalogDto>();
        }

        /// <summary>
        /// Obtiene el catálogo de impuestos SAT.
        /// </summary>
        public static async Task<List<CatalogDto>> ObtenerCatalogoImpuestosSATAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                Tenant = config.Password ?? "default"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Catalogs.SearchCatalogAsync("SatTaxes", "", 1, 1000);
            return result?.Data?.Items ?? new List<CatalogDto>();
        }
    }
}
