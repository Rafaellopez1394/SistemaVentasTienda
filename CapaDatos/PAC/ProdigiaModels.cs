using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Modelos para integración con Prodigia PAC - API REST
    /// Documentación: https://docs.prodigia.com.mx/api-timbrado-xml.html
    /// </summary>

    #region Request Models

    /// <summary>
    /// Modelo de request para timbrado de CFDI con Prodigia
    /// </summary>
    public class ProdigiaTimbrarRequest
    {
        [JsonProperty("xmlBase64")]
        public string XmlBase64 { get; set; }

        [JsonProperty("contrato")]
        public string Contrato { get; set; }

        [JsonProperty("certBase64")]
        public string CertBase64 { get; set; }

        [JsonProperty("keyBase64")]
        public string KeyBase64 { get; set; }

        [JsonProperty("keyPass")]
        public string KeyPass { get; set; }

        [JsonProperty("prueba")]
        public bool Prueba { get; set; }

        [JsonProperty("opciones")]
        public List<string> Opciones { get; set; }

        public ProdigiaTimbrarRequest()
        {
            Opciones = new List<string>();
        }

        /// <summary>
        /// Agrega opciones comunes recomendadas por Prodigia
        /// </summary>
        public void AgregarOpcionesRecomendadas(bool usarCertDefault = true)
        {
            // CALCULAR_SELLO: Prodigia calcula el sello del CFDI
            Opciones.Add("CALCULAR_SELLO");
            
            // ESTABLECER_NO_CERTIFICADO: Coloca certificado y noCertificado de BD
            Opciones.Add("ESTABLECER_NO_CERTIFICADO");
            
            // GENERAR_PDF: Genera el PDF del comprobante
            if (!Opciones.Contains("GENERAR_PDF"))
                Opciones.Add("GENERAR_PDF");
            
            // CERT_DEFAULT: Usa certificado almacenado en portal PADE
            if (usarCertDefault && !Opciones.Contains("CERT_DEFAULT"))
                Opciones.Add("CERT_DEFAULT");
        }
    }

    #endregion

    #region Response Models

    /// <summary>
    /// Respuesta de timbrado de Prodigia (XML parseado)
    /// </summary>
    public class ProdigiaTimbrarResponse
    {
        /// <summary>
        /// Identificador de la transacción interna (UUID formato, pero NO es el UUID fiscal)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indica si el timbrado fue exitoso (true) o falló (false)
        /// </summary>
        public bool TimbradoOk { get; set; }

        /// <summary>
        /// Contrato del cliente (informativo)
        /// </summary>
        public string Contrato { get; set; }

        /// <summary>
        /// Código de error/éxito: 0 = éxito, otro = error
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Mensaje descriptivo del error (vacío si éxito)
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Versión del Timbre Fiscal (1.1)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// UUID / Folio Fiscal asignado al CFDI
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Fecha de timbrado (formato ISO)
        /// </summary>
        public string FechaTimbrado { get; set; }

        /// <summary>
        /// Sello del CFDI
        /// </summary>
        public string SelloCFD { get; set; }

        /// <summary>
        /// Número de certificado del SAT
        /// </summary>
        public string NoCertificadoSAT { get; set; }

        /// <summary>
        /// Sello del Timbre Fiscal Digital (SAT)
        /// </summary>
        public string SelloSAT { get; set; }

        /// <summary>
        /// XML timbrado completo codificado en Base64
        /// </summary>
        public string XmlBase64 { get; set; }

        /// <summary>
        /// PDF generado codificado en Base64 (si se usó opción GENERAR_PDF)
        /// </summary>
        public string PdfBase64 { get; set; }

        /// <summary>
        /// Saldo de transacciones restantes (si se usó opción CONSULTAR_SALDO)
        /// </summary>
        public string Saldo { get; set; }

        /// <summary>
        /// Cadena original del timbre (si se usó opción REGRESAR_CADENA_ORIGINAL)
        /// </summary>
        public string CadenaOriginalTFD { get; set; }

        /// <summary>
        /// Código QR en Base64 (si se usó opción GENERAR_CBB)
        /// </summary>
        public string CodigoBarrasBidimensional { get; set; }
    }

    /// <summary>
    /// Respuesta de cancelación de Prodigia
    /// </summary>
    public class ProdigiaCancelarResponse
    {
        public bool StatusOk { get; set; }
        public string Rfc { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public int Procesados { get; set; }
        public int Cancelados { get; set; }
        public List<CancelacionIndividual> Cancelaciones { get; set; }
        public string AcuseCancelBase64 { get; set; }

        public ProdigiaCancelarResponse()
        {
            Cancelaciones = new List<CancelacionIndividual>();
        }
    }

    public class CancelacionIndividual
    {
        public string Uuid { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
    }

    #endregion
}
