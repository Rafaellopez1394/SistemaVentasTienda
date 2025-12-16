using System;

namespace CapaModelo
{
    /// <summary>
    /// Modelo para registro de envío de emails
    /// </summary>
    public class EmailLog
    {
        public int EmailLogID { get; set; }
        
        // Documento
        public string TipoDocumento { get; set; }
        public int DocumentoID { get; set; }
        public Guid? UUID { get; set; }
        
        // Destinatario
        public string EmailDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        
        // Contenido
        public string Asunto { get; set; }
        public string CuerpoHTML { get; set; }
        
        // Adjuntos
        public bool AdjuntoPDF { get; set; }
        public bool AdjuntoXML { get; set; }
        public string NombreArchivoPDF { get; set; }
        public string NombreArchivoXML { get; set; }
        
        // Estado
        public DateTime FechaEnvio { get; set; }
        public bool Exitoso { get; set; }
        public string MensajeError { get; set; }
        
        // SMTP
        public string ServidorSMTP { get; set; }
        
        // Auditoría
        public string UsuarioEnvio { get; set; }
    }
    
    /// <summary>
    /// Request para enviar email
    /// </summary>
    public class EnviarEmailRequest
    {
        public string EmailDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string Asunto { get; set; }
        public string CuerpoHTML { get; set; }
        public byte[] AdjuntoPDF { get; set; }
        public string NombreArchivoPDF { get; set; }
        public string AdjuntoXML { get; set; }
        public string NombreArchivoXML { get; set; }
    }
    
    /// <summary>
    /// Respuesta de envío de email
    /// </summary>
    public class RespuestaEmail
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}
