using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using CapaModelo;

namespace VentasWeb.Utilidades
{
    /// <summary>
    /// Servicio para envío de emails con SMTP
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _smtpSSL;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            // Leer configuración desde Web.config
            _smtpHost = ConfigurationManager.AppSettings["SMTP_Host"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"] ?? "587");
            _smtpUsername = ConfigurationManager.AppSettings["SMTP_Username"] ?? "";
            _smtpPassword = ConfigurationManager.AppSettings["SMTP_Password"] ?? "";
            _smtpSSL = bool.Parse(ConfigurationManager.AppSettings["SMTP_SSL"] ?? "true");
            _fromEmail = ConfigurationManager.AppSettings["SMTP_FromEmail"] ?? _smtpUsername;
            _fromName = ConfigurationManager.AppSettings["SMTP_FromName"] ?? "Sistema de Facturación";
        }

        /// <summary>
        /// Envía un email con adjuntos
        /// </summary>
        public RespuestaEmail EnviarEmail(EnviarEmailRequest request)
        {
            var respuesta = new RespuestaEmail
            {
                FechaEnvio = DateTime.Now
            };

            try
            {
                // Validar configuración
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Configuración SMTP incompleta. Verifique Web.config";
                    return respuesta;
                }

                // Validar email destinatario
                if (string.IsNullOrEmpty(request.EmailDestinatario))
                {
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Email de destinatario requerido";
                    return respuesta;
                }

                // Crear mensaje
                using (MailMessage mensaje = new MailMessage())
                {
                    mensaje.From = new MailAddress(_fromEmail, _fromName);
                    mensaje.To.Add(new MailAddress(request.EmailDestinatario, request.NombreDestinatario ?? ""));
                    mensaje.Subject = request.Asunto;
                    mensaje.Body = request.CuerpoHTML;
                    mensaje.IsBodyHtml = true;
                    mensaje.Priority = MailPriority.Normal;
                    mensaje.BodyEncoding = Encoding.UTF8;
                    mensaje.SubjectEncoding = Encoding.UTF8;

                    // Adjuntar PDF si existe
                    if (request.AdjuntoPDF != null && request.AdjuntoPDF.Length > 0)
                    {
                        MemoryStream pdfStream = new MemoryStream(request.AdjuntoPDF);
                        Attachment pdfAttachment = new Attachment(pdfStream, request.NombreArchivoPDF, "application/pdf");
                        mensaje.Attachments.Add(pdfAttachment);
                    }

                    // Adjuntar XML si existe
                    if (!string.IsNullOrEmpty(request.AdjuntoXML))
                    {
                        byte[] xmlBytes = Encoding.UTF8.GetBytes(request.AdjuntoXML);
                        MemoryStream xmlStream = new MemoryStream(xmlBytes);
                        Attachment xmlAttachment = new Attachment(xmlStream, request.NombreArchivoXML, "application/xml");
                        mensaje.Attachments.Add(xmlAttachment);
                    }

                    // Configurar cliente SMTP
                    using (SmtpClient smtp = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        smtp.EnableSsl = _smtpSSL;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Timeout = 30000; // 30 segundos

                        // Enviar
                        smtp.Send(mensaje);
                    }
                }

                respuesta.Exitoso = true;
                respuesta.Mensaje = "Email enviado exitosamente";
            }
            catch (SmtpException smtpEx)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error SMTP: {smtpEx.Message}";
                
                // Mensajes de error comunes
                if (smtpEx.Message.Contains("authentication"))
                {
                    respuesta.Mensaje += " - Verifique usuario y contraseña SMTP";
                }
                else if (smtpEx.Message.Contains("timed out"))
                {
                    respuesta.Mensaje += " - Tiempo de espera agotado. Verifique conexión a internet";
                }
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error al enviar email: {ex.Message}";
            }

            return respuesta;
        }

        /// <summary>
        /// Genera el cuerpo HTML para una factura
        /// </summary>
        public string GenerarCuerpoFactura(Factura factura, string nombreEmpresa)
        {
            StringBuilder html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine(".header { background: #007bff; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { background: #f8f9fa; padding: 20px; margin: 20px 0; }");
            html.AppendLine(".info { background: white; padding: 15px; margin: 10px 0; border-left: 4px solid #007bff; }");
            html.AppendLine(".footer { text-align: center; color: #6c757d; font-size: 12px; margin-top: 20px; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; }");
            html.AppendLine("td { padding: 5px; }");
            html.AppendLine(".label { font-weight: bold; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            
            // Header
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h2>{nombreEmpresa}</h2>");
            html.AppendLine("<p>Comprobante Fiscal Digital por Internet (CFDI)</p>");
            html.AppendLine("</div>");
            
            // Contenido
            html.AppendLine("<div class='content'>");
            html.AppendLine("<h3>Estimado Cliente:</h3>");
            html.AppendLine("<p>Le enviamos su Comprobante Fiscal Digital por Internet (CFDI 4.0) en formato PDF y XML.</p>");
            
            // Información del CFDI
            html.AppendLine("<div class='info'>");
            html.AppendLine("<h4>Información del Comprobante</h4>");
            html.AppendLine("<table>");
            
            if (!string.IsNullOrEmpty(factura.Serie))
                html.AppendLine($"<tr><td class='label'>Serie-Folio:</td><td>{factura.Serie}-{factura.Folio}</td></tr>");
            else
                html.AppendLine($"<tr><td class='label'>Folio:</td><td>{factura.Folio}</td></tr>");
            
            if (!string.IsNullOrEmpty(factura.UUID))
                html.AppendLine($"<tr><td class='label'>UUID:</td><td style='font-size: 11px; word-break: break-all;'>{factura.UUID}</td></tr>");
            
            html.AppendLine($"<tr><td class='label'>Fecha de Emisión:</td><td>{factura.FechaEmision:dd/MM/yyyy}</td></tr>");
            
            if (factura.FechaTimbrado.HasValue)
                html.AppendLine($"<tr><td class='label'>Fecha de Timbrado:</td><td>{factura.FechaTimbrado:dd/MM/yyyy HH:mm:ss}</td></tr>");
            
            html.AppendLine($"<tr><td class='label'>Total:</td><td style='font-size: 18px; font-weight: bold; color: #28a745;'>${factura.MontoTotal:N2} MXN</td></tr>");
            html.AppendLine("</table>");
            html.AppendLine("</div>");
            
            // Información importante
            html.AppendLine("<div class='info'>");
            html.AppendLine("<h4>Información Importante</h4>");
            html.AppendLine("<ul>");
            html.AppendLine("<li>Este comprobante es válido ante el SAT</li>");
            html.AppendLine("<li>Conserve los archivos XML y PDF para su contabilidad</li>");
            html.AppendLine("<li>Puede verificar el CFDI en el portal del SAT</li>");
            html.AppendLine("<li>El XML es el documento fiscal válido</li>");
            html.AppendLine("</ul>");
            html.AppendLine("</div>");
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Este es un correo automático, por favor no responder.</p>");
            html.AppendLine($"<p>&copy; {DateTime.Now.Year} {nombreEmpresa}. Todos los derechos reservados.</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }

        /// <summary>
        /// Genera el cuerpo HTML para un complemento de pago
        /// </summary>
        public string GenerarCuerpoComplementoPago(ComplementoPago complemento, string nombreEmpresa)
        {
            StringBuilder html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine(".header { background: #28a745; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { background: #f8f9fa; padding: 20px; margin: 20px 0; }");
            html.AppendLine(".info { background: white; padding: 15px; margin: 10px 0; border-left: 4px solid #28a745; }");
            html.AppendLine(".footer { text-align: center; color: #6c757d; font-size: 12px; margin-top: 20px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h2>{nombreEmpresa}</h2>");
            html.AppendLine("<p>Recibo Electrónico de Pago (REP)</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='content'>");
            html.AppendLine("<h3>Estimado Cliente:</h3>");
            html.AppendLine("<p>Le enviamos su Recibo Electrónico de Pago (Complemento de Pago 2.0) en formato PDF y XML.</p>");
            
            html.AppendLine("<div class='info'>");
            html.AppendLine("<h4>Información del Pago</h4>");
            html.AppendLine($"<p><strong>UUID:</strong><br><span style='font-size: 11px;'>{complemento.UUID}</span></p>");
            html.AppendLine($"<p><strong>Fecha de Pago:</strong> {complemento.FechaEmision:dd/MM/yyyy}</p>");
            html.AppendLine($"<p><strong>Monto Total:</strong> <span style='font-size: 18px; font-weight: bold; color: #28a745;'>${complemento.MontoTotalPagos:N2} MXN</span></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Este es un correo automático, por favor no responder.</p>");
            html.AppendLine($"<p>&copy; {DateTime.Now.Year} {nombreEmpresa}. Todos los derechos reservados.</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }

        /// <summary>
        /// Valida la configuración SMTP
        /// </summary>
        public bool ValidarConfiguracion(out string mensaje)
        {
            if (string.IsNullOrEmpty(_smtpHost))
            {
                mensaje = "SMTP_Host no configurado";
                return false;
            }

            if (string.IsNullOrEmpty(_smtpUsername))
            {
                mensaje = "SMTP_Username no configurado";
                return false;
            }

            if (string.IsNullOrEmpty(_smtpPassword))
            {
                mensaje = "SMTP_Password no configurado";
                return false;
            }

            if (string.IsNullOrEmpty(_fromEmail))
            {
                mensaje = "SMTP_FromEmail no configurado";
                return false;
            }

            mensaje = "Configuración válida";
            return true;
        }
    }
}
