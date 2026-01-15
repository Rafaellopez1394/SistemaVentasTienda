using Fiscalapi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PDF
{
    public class FiscalAPIPersonas
    {
        /// <summary>
        /// Crea una persona (emisor o receptor) en FiscalAPI.
        /// </summary>
        public static async Task<string> CrearPersonaAsync(ConfiguracionPAC config, string nombre, string rfc, string email, string password = null)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);
            var persona = new Person
            {
                LegalName = nombre,
                Tin = rfc,
                Email = email,
                Password = password ?? Guid.NewGuid().ToString("N")
            };
            var response = await client.Persons.CreateAsync(persona);
            return response.Succeeded ? response.Data?.Id : null;
        }

        /// <summary>
        /// Consulta una persona por RFC.
        /// </summary>
        public static async Task<Person> ConsultarPersonaAsync(ConfiguracionPAC config, string rfc)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);
            var response = await client.Persons.SearchAsync(rfc);
            if (response.Succeeded && response.Data != null && response.Data.Items.Count > 0)
                return response.Data.Items[0];
            return null;
        }

        /// <summary>
        /// Actualiza los datos de una persona.
        /// </summary>
        public static async Task<bool> ActualizarPersonaAsync(ConfiguracionPAC config, string personId, string nombre, string email)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);
            var persona = new Person
            {
                Id = personId,
                LegalName = nombre,
                Email = email
            };
            var response = await client.Persons.UpdateAsync(personId, persona);
            return response.Succeeded;
        }

        /// <summary>
        /// Elimina una persona por ID.
        /// </summary>
        public static async Task<bool> EliminarPersonaAsync(ConfiguracionPAC config, string personId)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);
            var response = await client.Persons.DeleteAsync(personId);
            return response.Succeeded;
        }

        /// <summary>
        /// Lista todas las personas registradas.
        /// </summary>
        public static async Task<List<Person>> ListarPersonasAsync(ConfiguracionPAC config)
        {
            var settings = new FiscalApiOptions
            {
                ApiUrl = config.EsProduccion ? "https://live.fiscalapi.com" : "https://test.fiscalapi.com",
                ApiKey = config.Usuario,
                Tenant = config.Password
            };
            var client = FiscalApiClient.Create(settings);
            var response = await client.Persons.GetAllAsync();
            return response.Succeeded && response.Data != null ? response.Data.Items : new List<Person>();
        }
    }
}
