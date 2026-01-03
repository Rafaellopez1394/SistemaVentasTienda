using System;

namespace CapaModelo
{
    public class ConfiguracionSMTP
    {
        public int ConfigID { get; set; }
        
        // Servidor SMTP
        public string Host { get; set; }
        public int Puerto { get; set; }
        public bool UsarSSL { get; set; }
        
        // Credenciales
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        
        // Remitente
        public string EmailRemitente { get; set; }
        public string NombreRemitente { get; set; }
        
        // Estado
        public bool Activo { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}




