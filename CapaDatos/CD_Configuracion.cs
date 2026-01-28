using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Configuracion
    {
        private static CD_Configuracion _instancia;
        public static CD_Configuracion Instancia => _instancia ?? (_instancia = new CD_Configuracion());

        // Obtener todas las configuraciones de impresoras
        public List<ConfiguracionImpresora> ObtenerImpresoras()
        {
            var lista = new List<ConfiguracionImpresora>();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT ConfigID, TipoImpresion, NombreImpresora, AnchoPapel, Activo, Usuario, FechaModificacion 
                                FROM ConfiguracionImpresoras WHERE Activo = 1";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ConfiguracionImpresora
                        {
                            ConfigID = Convert.ToInt32(dr["ConfigID"]),
                            TipoImpresion = dr["TipoImpresion"].ToString(),
                            NombreImpresora = dr["NombreImpresora"].ToString(),
                            AnchoPapel = Convert.ToInt32(dr["AnchoPapel"]),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Usuario = dr["Usuario"].ToString(),
                            FechaModificacion = Convert.ToDateTime(dr["FechaModificacion"])
                        });
                    }
                }
            }
            
            return lista;
        }

        // Obtener impresora por tipo
        public ConfiguracionImpresora ObtenerImpresoraPorTipo(string tipo)
        {
            ConfiguracionImpresora config = null;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT TOP 1 ConfigID, TipoImpresion, NombreImpresora, AnchoPapel, Activo, Usuario, FechaModificacion 
                                FROM ConfiguracionImpresoras 
                                WHERE TipoImpresion = @Tipo AND Activo = 1";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        config = new ConfiguracionImpresora
                        {
                            ConfigID = Convert.ToInt32(dr["ConfigID"]),
                            TipoImpresion = dr["TipoImpresion"].ToString(),
                            NombreImpresora = dr["NombreImpresora"].ToString(),
                            AnchoPapel = Convert.ToInt32(dr["AnchoPapel"]),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Usuario = dr["Usuario"].ToString(),
                            FechaModificacion = Convert.ToDateTime(dr["FechaModificacion"])
                        };
                    }
                }
            }
            
            return config;
        }

        // Guardar configuracion de impresora (UPSERT por TipoImpresion)
        public bool GuardarImpresora(ConfiguracionImpresora config)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                // UPSERT: Si existe un registro con el mismo TipoImpresion, actualizar; si no, insertar
                string query = @"
                    IF EXISTS (SELECT 1 FROM ConfiguracionImpresoras WHERE TipoImpresion = @TipoImpresion)
                    BEGIN
                        UPDATE ConfiguracionImpresoras 
                        SET NombreImpresora = @NombreImpresora, 
                            AnchoPapel = @AnchoPapel, 
                            Activo = @Activo, 
                            Usuario = @Usuario, 
                            FechaModificacion = GETDATE()
                        WHERE TipoImpresion = @TipoImpresion
                    END
                    ELSE
                    BEGIN
                        INSERT INTO ConfiguracionImpresoras (TipoImpresion, NombreImpresora, AnchoPapel, Activo, Usuario, FechaModificacion)
                        VALUES (@TipoImpresion, @NombreImpresora, @AnchoPapel, @Activo, @Usuario, GETDATE())
                    END";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@TipoImpresion", config.TipoImpresion);
                cmd.Parameters.AddWithValue("@NombreImpresora", config.NombreImpresora);
                cmd.Parameters.AddWithValue("@AnchoPapel", config.AnchoPapel);
                cmd.Parameters.AddWithValue("@Activo", config.Activo);
                cmd.Parameters.AddWithValue("@Usuario", config.Usuario ?? "system");
                
                cnx.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        // Obtener configuracion general
        public ConfiguracionGeneral ObtenerConfiguracionGeneral()
        {
            ConfiguracionGeneral config = new ConfiguracionGeneral();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT TOP 1 ConfigID, NombreNegocio, RFC, Direccion, Telefono, Email, 
                                       LogoPath, MensajeTicket, ImprimirTicketAutomatico, MostrarLogoEnTicket,
                                       Usuario, FechaModificacion
                                FROM ConfiguracionGeneral";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        config.ConfigID = Convert.ToInt32(dr["ConfigID"]);
                        config.NombreNegocio = dr["NombreNegocio"].ToString();
                        config.RFC = dr["RFC"].ToString();
                        config.Direccion = dr["Direccion"].ToString();
                        config.Telefono = dr["Telefono"].ToString();
                        config.Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString();
                        config.LogoPath = dr["LogoPath"] == DBNull.Value ? "" : dr["LogoPath"].ToString();
                        config.MensajeTicket = dr["MensajeTicket"] == DBNull.Value ? "" : dr["MensajeTicket"].ToString();
                        config.ImprimirTicketAutomatico = dr["ImprimirTicketAutomatico"] != DBNull.Value && Convert.ToBoolean(dr["ImprimirTicketAutomatico"]);
                        config.MostrarLogoEnTicket = dr["MostrarLogoEnTicket"] != DBNull.Value && Convert.ToBoolean(dr["MostrarLogoEnTicket"]);
                        config.Usuario = dr["Usuario"].ToString();
                        config.FechaModificacion = Convert.ToDateTime(dr["FechaModificacion"]);
                    }
                }
            }
            
            return config;
        }

        // Guardar configuracion general
        public bool GuardarConfigGeneral(ConfiguracionGeneral config)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query;
                
                if (config.ConfigID > 0)
                {
                    query = @"UPDATE ConfiguracionGeneral 
                             SET NombreNegocio = @NombreNegocio, RFC = @RFC, Direccion = @Direccion, 
                                 Telefono = @Telefono, Email = @Email, LogoPath = @LogoPath,
                                 MensajeTicket = @MensajeTicket, ImprimirTicketAutomatico = @ImprimirAuto,
                                 MostrarLogoEnTicket = @MostrarLogo, Usuario = @Usuario, FechaModificacion = GETDATE()
                             WHERE ConfigID = @ConfigID";
                }
                else
                {
                    query = @"INSERT INTO ConfiguracionGeneral 
                             (NombreNegocio, RFC, Direccion, Telefono, Email, LogoPath, MensajeTicket, 
                              ImprimirTicketAutomatico, MostrarLogoEnTicket, Usuario, FechaModificacion)
                             VALUES (@NombreNegocio, @RFC, @Direccion, @Telefono, @Email, @LogoPath, 
                                    @MensajeTicket, @ImprimirAuto, @MostrarLogo, @Usuario, GETDATE())";
                }
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                
                if (config.ConfigID > 0)
                    cmd.Parameters.AddWithValue("@ConfigID", config.ConfigID);
                    
                cmd.Parameters.AddWithValue("@NombreNegocio", config.NombreNegocio ?? "");
                cmd.Parameters.AddWithValue("@RFC", config.RFC ?? "");
                cmd.Parameters.AddWithValue("@Direccion", config.Direccion ?? "");
                cmd.Parameters.AddWithValue("@Telefono", config.Telefono ?? "");
                cmd.Parameters.AddWithValue("@Email", (object)config.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LogoPath", (object)config.LogoPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MensajeTicket", (object)config.MensajeTicket ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImprimirAuto", config.ImprimirTicketAutomatico);
                cmd.Parameters.AddWithValue("@MostrarLogo", config.MostrarLogoEnTicket);
                cmd.Parameters.AddWithValue("@Usuario", config.Usuario ?? "system");
                
                cnx.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Obtener datos del negocio para el ticket
        public DatosNegocio ObtenerDatosNegocio()
        {
            var datos = new DatosNegocio();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT TOP 1 NombreNegocio, RFC, Direccion, Telefono, MensajeTicket, ImprimirTicketAutomatico,
                                LogoPath, MostrarLogoEnTicket
                                FROM ConfiguracionGeneral";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        datos.NombreNegocio = dr["NombreNegocio"].ToString();
                        datos.RFC = dr["RFC"].ToString();
                        datos.Direccion = dr["Direccion"].ToString();
                        datos.Telefono = dr["Telefono"].ToString();
                        datos.LogoPath = dr["LogoPath"] == DBNull.Value ? "" : dr["LogoPath"].ToString();
                        datos.MostrarLogoEnTicket = dr["MostrarLogoEnTicket"] != DBNull.Value && Convert.ToBoolean(dr["MostrarLogoEnTicket"]);
                        datos.MensajeTicket = dr["MensajeTicket"] == DBNull.Value ? "Gracias por su compra" : dr["MensajeTicket"].ToString();
                        datos.ImprimirTicketAutomatico = dr["ImprimirTicketAutomatico"] != DBNull.Value && Convert.ToBoolean(dr["ImprimirTicketAutomatico"]);
                    }
                    else
                    {
                        // Valores por defecto
                        datos.NombreNegocio = "MI TIENDA";
                        datos.RFC = "XAXX010101000";
                        datos.Direccion = "Direccion del negocio";
                        datos.Telefono = "";
                        datos.LogoPath = "";
                        datos.MostrarLogoEnTicket = false;
                        datos.MensajeTicket = "Gracias por su compra";
                        datos.ImprimirTicketAutomatico = false;
                    }
                }
            }
            
            return datos;
        }

        // Actualizar solo configuracion de tickets
        public bool ActualizarConfigTickets(string mensajeTicket, bool imprimirAuto, bool mostrarLogo, string usuario)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                // Verificar si existe registro
                string queryCheck = "SELECT COUNT(*) FROM ConfiguracionGeneral";
                SqlCommand cmdCheck = new SqlCommand(queryCheck, cnx);
                cnx.Open();
                
                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                
                string query;
                if (count > 0)
                {
                    query = @"UPDATE ConfiguracionGeneral 
                             SET MensajeTicket = @MensajeTicket, 
                                 ImprimirTicketAutomatico = @ImprimirAuto, 
                                 MostrarLogoEnTicket = @MostrarLogo,
                                 Usuario = @Usuario, 
                                 FechaModificacion = GETDATE()";
                }
                else
                {
                    query = @"INSERT INTO ConfiguracionGeneral 
                             (NombreNegocio, RFC, Direccion, Telefono, MensajeTicket, ImprimirTicketAutomatico, MostrarLogoEnTicket, Usuario, FechaModificacion)
                             VALUES ('MI TIENDA', '', '', '', @MensajeTicket, @ImprimirAuto, @MostrarLogo, @Usuario, GETDATE())";
                }
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@MensajeTicket", (object)mensajeTicket ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImprimirAuto", imprimirAuto);
                cmd.Parameters.AddWithValue("@MostrarLogo", mostrarLogo);
                cmd.Parameters.AddWithValue("@Usuario", usuario ?? "system");
                
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
