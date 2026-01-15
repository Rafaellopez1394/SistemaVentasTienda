using System;

namespace CapaModelo
{
    /// <summary>
    /// Configuración para FiscalAPI - Integración directa sin SDK
    /// </summary>
    public class ConfiguracionFiscalAPI
    {
        public int ConfiguracionID { get; set; }
        
        /// <summary>
        /// API Key proporcionada por FiscalAPI
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
    /// Tenant ID proporcionado por FiscalAPI
    /// </summary>
    public string Tenant { get; set; }
    
    /// <summary>
    /// Ambiente: TEST o PRODUCCION
    /// </summary>
    public string Ambiente { get; set; }
    
    /// <summary>
        /// TEST: https://test.fiscalapi.com
        /// PROD: https://api.fiscalapi.com
        /// </summary>
        public string UrlApi
        {
            get
            {
                return Ambiente == "PRODUCCION" 
                    ? "https://api.fiscalapi.com" 
                    : "https://test.fiscalapi.com";
            }
        }
        
        /// <summary>
        /// RFC del emisor (tu empresa)
        /// </summary>
        public string RfcEmisor { get; set; }
        
        /// <summary>
        /// Nombre o razón social del emisor
        /// </summary>
        public string NombreEmisor { get; set; }
        
        /// <summary>
        /// Régimen fiscal (clave SAT)
        /// Ejemplo: 601 - General de Ley Personas Morales
        /// </summary>
        public string RegimenFiscal { get; set; }
        
        /// <summary>
        /// Certificado .cer en Base64
        /// </summary>
        public string CertificadoBase64 { get; set; }
        
        /// <summary>
        /// Llave privada .key en Base64
        /// </summary>
        public string LlavePrivadaBase64 { get; set; }
        
        /// <summary>
        /// Contraseña de la llave privada
        /// </summary>
        public string PasswordLlave { get; set; }
        
        /// <summary>
        /// Código postal del emisor
        /// </summary>
        public string CodigoPostal { get; set; }
        
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
