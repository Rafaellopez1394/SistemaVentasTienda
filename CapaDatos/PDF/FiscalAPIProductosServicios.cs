using CapaModelo;
using Fiscalapi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaDatos.PDF
{
    /// <summary>
    /// Utilidad para CRUD de productos y servicios vía FiscalAPI SDK.
    /// </summary>
    public class FiscalAPIProductosServicios
    {
        /// <summary>
        /// Crea un producto o servicio en FiscalAPI.
        /// </summary>
        public static async Task<string> CrearProductoAsync(ConfiguracionPAC config, string nombre, string claveProdServSAT, string claveUnidadSAT, decimal precio, string descripcion = null)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com"
            };
            var client = FiscalApiClient.Create(settings);
            var producto = new Product
            {
                Name = nombre,
                ProductCode = claveProdServSAT,
                UnitCode = claveUnidadSAT,
                Price = precio,
                Description = descripcion
            };
            var result = await client.Products.CreateAsync(producto);
            return result?.Data?.Id;
        }

        /// <summary>
        /// Consulta un producto o servicio por ID FiscalAPI.
        /// </summary>
        public static async Task<Product> ConsultarProductoAsync(ConfiguracionPAC config, string productId)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Products.GetAsync(productId);
            return result?.Succeeded == true ? result.Data : null;
        }

        /// <summary>
        /// Actualiza un producto o servicio en FiscalAPI.
        /// </summary>
        public static async Task<bool> ActualizarProductoAsync(ConfiguracionPAC config, string productId, string nombre, decimal precio, string descripcion = null)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com"
            };
            var client = FiscalApiClient.Create(settings);
            var producto = new Product
            {
                Name = nombre,
                Price = precio,
                Description = descripcion
            };
            var result = await client.Products.UpdateAsync(productId, producto);
            return result != null;
        }

        /// <summary>
        /// Elimina un producto o servicio en FiscalAPI.
        /// </summary>
        public static async Task<bool> EliminarProductoAsync(ConfiguracionPAC config, string productId)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Products.DeleteAsync(productId);
            return result?.Succeeded == true && result.Data;
        }

        /// <summary>
        /// Lista todos los productos y servicios registrados en FiscalAPI.
        /// </summary>
        public static async Task<List<Product>> ListarProductosAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiKey = config.ApiKey,
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com"
            };
            var client = FiscalApiClient.Create(settings);
            var result = await client.Products.ListAsync();
            return result?.Data?.Items ?? new List<Product>();
        }
    }
}
