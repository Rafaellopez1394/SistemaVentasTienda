// CapaModelo/CertificadoDigital.cs
using System;

namespace CapaModelo
{
    /// <summary>
    /// Certificado Digital del SAT (CSD)
    /// </summary>
    public class CertificadoDigital
    {
        public int CertificadoID { get; set; }
        public string NombreCertificado { get; set; }
        public string NoCertificado { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        
        // Archivos (binario)
        public byte[] ArchivoCER { get; set; }
        public byte[] ArchivoKEY { get; set; }
        public string PasswordKEY { get; set; }
        
        // Vigencia
        public DateTime FechaVigenciaInicio { get; set; }
        public DateTime FechaVigenciaFin { get; set; }
        
        // Rutas físicas (opcional)
        public string RutaCER { get; set; }
        public string RutaKEY { get; set; }
        
        // Estado
        public bool Activo { get; set; }
        public bool EsPredeterminado { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Observaciones { get; set; }
        
        // Propiedades calculadas
        public bool EsVigente => FechaVigenciaFin > DateTime.Now;
        public int DiasRestantes => (FechaVigenciaFin - DateTime.Now).Days;
        public string EstadoVigencia
        {
            get
            {
                if (!EsVigente) return "VENCIDO";
                if (DiasRestantes <= 30) return "POR VENCER";
                return "VIGENTE";
            }
        }
    }

    /// <summary>
    /// Payload para cargar certificado
    /// </summary>
    public class CargarCertificadoRequest
    {
        public string NombreCertificado { get; set; }
        public byte[] ArchivoCER { get; set; }
        public byte[] ArchivoKEY { get; set; }
        public string PasswordKEY { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public bool EsPredeterminado { get; set; }
        public string Usuario { get; set; }
    }
}




