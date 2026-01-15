using System;

namespace CapaModelo
{
    /// <summary>
    /// Configuración para Prodigia PAC
    /// Datos obtenidos al registrarse en https://facturacion.pade.mx/
    /// </summary>
    public class ConfiguracionProdigia
    {
        public int ConfiguracionID { get; set; }

        /// <summary>
        /// Usuario de webservice asignado por Prodigia
        /// </summary>
        public string Usuario { get; set; }

        /// <summary>
        /// Contraseña del usuario de webservice
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Código de contrato del servicio de timbrado
        /// </summary>
        public string Contrato { get; set; }

        /// <summary>
        /// Ambiente: TEST o PRODUCCION
        /// </summary>
        public string Ambiente { get; set; }

        /// <summary>
        /// RFC del emisor
        /// </summary>
        public string RfcEmisor { get; set; }

        /// <summary>
        /// Nombre / Razón Social del emisor
        /// </summary>
        public string NombreEmisor { get; set; }

        /// <summary>
        /// Régimen Fiscal del emisor (clave SAT)
        /// </summary>
        public string RegimenFiscal { get; set; }

        /// <summary>
        /// Certificado CSD en Base64 (.cer)
        /// OPCIONAL: Si se sube al portal de Prodigia, se puede usar opción CERT_DEFAULT
        /// </summary>
        public string CertificadoBase64 { get; set; }

        /// <summary>
        /// Llave privada CSD en Base64 (.key)
        /// OPCIONAL: Si se sube al portal de Prodigia, se puede usar opción CERT_DEFAULT
        /// </summary>
        public string LlavePrivadaBase64 { get; set; }

        /// <summary>
        /// Contraseña de la llave privada
        /// </summary>
        public string PasswordLlave { get; set; }

        /// <summary>
        /// Código Postal de expedición
        /// </summary>
        public string CodigoPostal { get; set; }

        /// <summary>
        /// Indica si la configuración está activa
        /// </summary>
        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// URL del API según ambiente
        /// </summary>
        public string UrlApi
        {
            get
            {
                if (Ambiente == "TEST")
                    return "https://pruebas.pade.mx/";
                else
                    return "https://timbrado.pade.mx/";
            }
        }
    }
}
