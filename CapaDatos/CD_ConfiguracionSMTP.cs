using System;
using System.Data.SqlClient;
using CapaModelo;

namespace CapaDatos
{
    public class CD_ConfiguracionSMTP
    {
        private static CD_ConfiguracionSMTP _instancia = null;

        public static CD_ConfiguracionSMTP Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_ConfiguracionSMTP();
                return _instancia;
            }
        }

        /// <summary>
        /// Obtiene la configuración SMTP activa
        /// </summary>
        public ConfiguracionSMTP ObtenerConfiguracion()
        {
            ConfiguracionSMTP config = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    string query = @"
                        SELECT TOP 1 ConfigID, Host, Puerto, UsarSSL, Usuario, Contrasena,
                               EmailRemitente, NombreRemitente, Activo,
                               UsuarioCreacion, FechaCreacion, UsuarioModificacion, FechaModificacion
                        FROM ConfiguracionSMTP
                        ORDER BY ConfigID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, cnx))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            config = new ConfiguracionSMTP
                            {
                                ConfigID = Convert.ToInt32(dr["ConfigID"]),
                                Host = dr["Host"].ToString(),
                                Puerto = Convert.ToInt32(dr["Puerto"]),
                                UsarSSL = Convert.ToBoolean(dr["UsarSSL"]),
                                Usuario = dr["Usuario"].ToString(),
                                Contrasena = dr["Contrasena"].ToString(),
                                EmailRemitente = dr["EmailRemitente"].ToString(),
                                NombreRemitente = dr["NombreRemitente"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                                UsuarioModificacion = dr["UsuarioModificacion"]?.ToString(),
                                FechaModificacion = dr["FechaModificacion"] != DBNull.Value ? (DateTime?)dr["FechaModificacion"] : null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener configuración SMTP: {ex.Message}");
            }

            return config;
        }

        /// <summary>
        /// Guarda o actualiza la configuración SMTP
        /// </summary>
        public Respuesta GuardarConfiguracion(ConfiguracionSMTP config, string usuario)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query;
                    if (config.ConfigID == 0)
                    {
                        // Insertar nueva configuración
                        query = @"
                            INSERT INTO ConfiguracionSMTP
                            (Host, Puerto, UsarSSL, Usuario, Contrasena, EmailRemitente, NombreRemitente, Activo, UsuarioCreacion, FechaCreacion)
                            VALUES
                            (@Host, @Puerto, @UsarSSL, @Usuario, @Contrasena, @EmailRemitente, @NombreRemitente, @Activo, @Usuario, GETDATE());
                            SELECT SCOPE_IDENTITY();";
                    }
                    else
                    {
                        // Actualizar configuración existente
                        query = @"
                            UPDATE ConfiguracionSMTP SET
                                Host = @Host,
                                Puerto = @Puerto,
                                UsarSSL = @UsarSSL,
                                Usuario = @Usuario,
                                Contrasena = @Contrasena,
                                EmailRemitente = @EmailRemitente,
                                NombreRemitente = @NombreRemitente,
                                Activo = @Activo,
                                UsuarioModificacion = @UsuarioMod,
                                FechaModificacion = GETDATE()
                            WHERE ConfigID = @ConfigID";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, cnx))
                    {
                        cmd.Parameters.AddWithValue("@Host", config.Host);
                        cmd.Parameters.AddWithValue("@Puerto", config.Puerto);
                        cmd.Parameters.AddWithValue("@UsarSSL", config.UsarSSL);
                        cmd.Parameters.AddWithValue("@Usuario", config.Usuario);
                        cmd.Parameters.AddWithValue("@Contrasena", config.Contrasena);
                        cmd.Parameters.AddWithValue("@EmailRemitente", config.EmailRemitente);
                        cmd.Parameters.AddWithValue("@NombreRemitente", config.NombreRemitente);
                        cmd.Parameters.AddWithValue("@Activo", config.Activo);
                        cmd.Parameters.AddWithValue("@UsuarioMod", usuario);

                        if (config.ConfigID == 0)
                        {
                            int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                            respuesta.Resultado = true;
                            respuesta.Mensaje = "Configuración SMTP creada exitosamente";
                            respuesta.Datos = nuevoId;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ConfigID", config.ConfigID);
                            cmd.ExecuteNonQuery();
                            respuesta.Resultado = true;
                            respuesta.Mensaje = "Configuración SMTP actualizada exitosamente";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = $"Error al guardar configuración SMTP: {ex.Message}";
            }

            return respuesta;
        }

        /// <summary>
        /// Prueba la conexión SMTP con las credenciales proporcionadas
        /// </summary>
        public Respuesta ProbarConexion(ConfiguracionSMTP config)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                using (var client = new System.Net.Mail.SmtpClient(config.Host, config.Puerto))
                {
                    client.EnableSsl = config.UsarSSL;
                    client.Credentials = new System.Net.NetworkCredential(config.Usuario, config.Contrasena);
                    client.Timeout = 10000; // 10 segundos

                    // Enviar email de prueba
                    using (var mensaje = new System.Net.Mail.MailMessage())
                    {
                        mensaje.From = new System.Net.Mail.MailAddress(config.EmailRemitente, config.NombreRemitente);
                        mensaje.To.Add(config.EmailRemitente); // Enviar a sí mismo
                        mensaje.Subject = "Prueba de Configuración SMTP";
                        mensaje.Body = $"<h3>✅ Conexión SMTP exitosa</h3><p>Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p><p>Servidor: {config.Host}:{config.Puerto}</p>";
                        mensaje.IsBodyHtml = true;

                        client.Send(mensaje);
                    }
                }

                respuesta.Resultado = true;
                respuesta.Mensaje = "✅ Conexión SMTP exitosa. Se envió un email de prueba.";
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = $"❌ Error de conexión: {ex.Message}";
            }

            return respuesta;
        }
    }
}
