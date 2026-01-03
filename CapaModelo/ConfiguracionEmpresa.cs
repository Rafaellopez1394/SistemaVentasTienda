using System;

namespace CapaModelo
{
    /// <summary>
    /// Configuración de la Empresa (Datos del Emisor)
    /// </summary>
    public class ConfiguracionEmpresa
    {
        public int ConfiguracionID { get; set; }
        public int ConfigEmpresaID { get; set; } // Alias para ConfiguracionID
        
        // Datos Fiscales
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string RegimenFiscal { get; set; }
        public string RegimenFiscalDescripcion { get; set; }
        
        // Domicilio Fiscal
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
        
        // Certificados Digitales (CSD)
        public string NoCertificado { get; set; }
        public DateTime? FechaVigenciaCertificado { get; set; }
        public string RutaCertificado { get; set; }
        public string RutaLlavePrivada { get; set; }
        public string PasswordLlave { get; set; }
        public byte[] CertificadoBytes { get; set; }
        public byte[] LlavePrivadaBytes { get; set; }
        
        // Configuración PAC
        public string ProveedorPAC { get; set; }
        public string UrlPAC { get; set; }
        public string UsuarioPAC { get; set; }
        public string PasswordPAC { get; set; }
        public bool EsProduccionPAC { get; set; }
        
        // Configuración CFDI
        public string SerieCFDI { get; set; }
        public int FolioInicialCFDI { get; set; }
        public string LugarExpedicion { get; set; }
        public string LogotipoEmpresa { get; set; }
        
        // Configuración Nómina
        public string RegistroPatronal { get; set; }
        public string SerieCFDINomina { get; set; }
        public int FolioInicialNomina { get; set; }
        public string RutaCertificadoNomina { get; set; }
        public string LugarExpedicionNomina { get; set; }
        public decimal PorcentajeIMSSPatron { get; set; }
        public decimal PorcentajeIMSSTrabajador { get; set; }
        public decimal PorcentajeRetiro { get; set; }
        public decimal PorcentajeCesantia { get; set; }
        public decimal PorcentajeInfonavit { get; set; }
        
        // Configuración Contabilidad
        public string EjercicioFiscal { get; set; }
        public DateTime? InicioEjercicio { get; set; }
        public DateTime? FinEjercicio { get; set; }
        public bool UsaContabilidadElectronica { get; set; }
        public bool GeneraPolizasAutomaticas { get; set; }
        
        // Configuración Correo
        public string ServidorSMTP { get; set; }
        public int PuertoSMTP { get; set; }
        public string UsuarioSMTP { get; set; }
        public string PasswordSMTP { get; set; }
        public bool RequiereSSL { get; set; }
        public string EmailRemitente { get; set; }
        public string NombreRemitente { get; set; }
        
        // Contacto
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string PaginaWeb { get; set; }
        public string SitioWeb { get; set; } // Alias para PaginaWeb
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }
}


