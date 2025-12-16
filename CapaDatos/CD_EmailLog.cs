using System;
using System.Data.SqlClient;
using CapaModelo;

namespace CapaDatos
{
    public class CD_EmailLog
    {
        private static CD_EmailLog _instancia = null;

        public static CD_EmailLog Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_EmailLog();
                return _instancia;
            }
        }

        /// <summary>
        /// Registra un envío de email en el log
        /// </summary>
        public void RegistrarEnvio(string tipoDocumento, int documentoID, Guid? uuid, 
            EnviarEmailRequest request, RespuestaEmail respuesta, string usuario)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        INSERT INTO EmailLog
                        (TipoDocumento, DocumentoID, UUID, EmailDestinatario, NombreDestinatario,
                         Asunto, CuerpoHTML, AdjuntoPDF, AdjuntoXML, NombreArchivoPDF, NombreArchivoXML,
                         FechaEnvio, Exitoso, MensajeError, UsuarioEnvio)
                        VALUES
                        (@Tipo, @DocID, @UUID, @Email, @Nombre, @Asunto, @Cuerpo,
                         @PDF, @XML, @NombrePDF, @NombreXML, @Fecha, @Exitoso, @Error, @Usuario)";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@Tipo", tipoDocumento);
                    cmd.Parameters.AddWithValue("@DocID", documentoID);
                    cmd.Parameters.AddWithValue("@UUID", uuid.HasValue ? (object)uuid.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", request.EmailDestinatario);
                    cmd.Parameters.AddWithValue("@Nombre", request.NombreDestinatario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Asunto", request.Asunto);
                    cmd.Parameters.AddWithValue("@Cuerpo", request.CuerpoHTML ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PDF", request.AdjuntoPDF != null && request.AdjuntoPDF.Length > 0);
                    cmd.Parameters.AddWithValue("@XML", !string.IsNullOrEmpty(request.AdjuntoXML));
                    cmd.Parameters.AddWithValue("@NombrePDF", request.NombreArchivoPDF ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NombreXML", request.NombreArchivoXML ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fecha", respuesta.FechaEnvio);
                    cmd.Parameters.AddWithValue("@Exitoso", respuesta.Exitoso);
                    cmd.Parameters.AddWithValue("@Error", respuesta.Mensaje ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", usuario ?? (object)DBNull.Value);

                    cnx.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // No lanzar excepción, solo registrar error en consola
                System.Diagnostics.Debug.WriteLine($"Error al registrar log de email: {ex.Message}");
            }
        }
    }
}
