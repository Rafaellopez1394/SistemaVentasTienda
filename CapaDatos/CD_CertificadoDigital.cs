// CapaDatos/CD_CertificadoDigital.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace CapaDatos
{
    public class CD_CertificadoDigital
    {
        private static CD_CertificadoDigital _instancia;
        public static CD_CertificadoDigital Instancia => _instancia ??= new CD_CertificadoDigital();

        private CD_CertificadoDigital() { }

        /// <summary>
        /// Obtiene todos los certificados
        /// </summary>
        public List<CertificadoDigital> ObtenerTodos()
        {
            var lista = new List<CertificadoDigital>();

            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var query = @"
                        SELECT CertificadoID, NombreCertificado, NoCertificado, RFC, RazonSocial,
                               FechaVigenciaInicio, FechaVigenciaFin, RutaCER, RutaKEY,
                               Activo, EsPredeterminado, UsuarioCreacion, FechaCreacion,
                               UsuarioModificacion, FechaModificacion, Observaciones
                        FROM CertificadosDigitales
                        ORDER BY FechaCreacion DESC";

                    var cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CertificadoDigital
                            {
                                CertificadoID = (int)dr["CertificadoID"],
                                NombreCertificado = dr["NombreCertificado"].ToString(),
                                NoCertificado = dr["NoCertificado"].ToString(),
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                FechaVigenciaInicio = (DateTime)dr["FechaVigenciaInicio"],
                                FechaVigenciaFin = (DateTime)dr["FechaVigenciaFin"],
                                RutaCER = dr["RutaCER"]?.ToString(),
                                RutaKEY = dr["RutaKEY"]?.ToString(),
                                Activo = (bool)dr["Activo"],
                                EsPredeterminado = (bool)dr["EsPredeterminado"],
                                UsuarioCreacion = dr["UsuarioCreacion"].ToString(),
                                FechaCreacion = (DateTime)dr["FechaCreacion"],
                                UsuarioModificacion = dr["UsuarioModificacion"]?.ToString(),
                                FechaModificacion = dr["FechaModificacion"] != DBNull.Value ? (DateTime?)dr["FechaModificacion"] : null,
                                Observaciones = dr["Observaciones"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener certificados: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene certificado predeterminado
        /// </summary>
        public CertificadoDigital ObtenerPredeterminado()
        {
            CertificadoDigital certificado = null;

            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SP_ObtenerCertificadoPredeterminado", cnx) 
                    { 
                        CommandType = CommandType.StoredProcedure 
                    };

                    cnx.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            certificado = new CertificadoDigital
                            {
                                CertificadoID = (int)dr["CertificadoID"],
                                NombreCertificado = dr["NombreCertificado"].ToString(),
                                NoCertificado = dr["NoCertificado"].ToString(),
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                ArchivoCER = dr["ArchivoCER"] as byte[],
                                ArchivoKEY = dr["ArchivoKEY"] as byte[],
                                PasswordKEY = dr["PasswordKEY"].ToString(),
                                FechaVigenciaInicio = (DateTime)dr["FechaVigenciaInicio"],
                                FechaVigenciaFin = (DateTime)dr["FechaVigenciaFin"],
                                RutaCER = dr["RutaCER"]?.ToString(),
                                RutaKEY = dr["RutaKEY"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener certificado predeterminado: " + ex.Message);
            }

            return certificado;
        }

        /// <summary>
        /// Cargar nuevo certificado
        /// </summary>
        public Respuesta CargarCertificado(CargarCertificadoRequest request)
        {
            var respuesta = new Respuesta { Resultado = false };

            try
            {
                // Validar certificado .cer
                X509Certificate2 certX509;
                try
                {
                    certX509 = new X509Certificate2(request.ArchivoCER);
                }
                catch
                {
                    respuesta.Mensaje = "El archivo .cer no es un certificado válido";
                    return respuesta;
                }

                // Extraer información del certificado
                string noCertificado = certX509.SerialNumber;
                DateTime vigenciaInicio = certX509.NotBefore;
                DateTime vigenciaFin = certX509.NotAfter;

                // Validar vigencia
                if (vigenciaFin < DateTime.Now)
                {
                    respuesta.Mensaje = "El certificado está vencido";
                    return respuesta;
                }

                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    using (var transaction = cnx.BeginTransaction())
                    {
                        try
                        {
                            // Si es predeterminado, quitar predeterminado de otros
                            if (request.EsPredeterminado)
                            {
                                var cmdUpdate = new SqlCommand(@"
                                    UPDATE CertificadosDigitales 
                                    SET EsPredeterminado = 0, 
                                        UsuarioModificacion = @Usuario,
                                        FechaModificacion = GETDATE()
                                    WHERE EsPredeterminado = 1", cnx, transaction);
                                cmdUpdate.Parameters.AddWithValue("@Usuario", request.Usuario);
                                cmdUpdate.ExecuteNonQuery();
                            }

                            // Insertar certificado
                            var cmdInsert = new SqlCommand(@"
                                INSERT INTO CertificadosDigitales
                                (NombreCertificado, NoCertificado, RFC, RazonSocial,
                                 ArchivoCER, ArchivoKEY, PasswordKEY,
                                 FechaVigenciaInicio, FechaVigenciaFin,
                                 Activo, EsPredeterminado, UsuarioCreacion)
                                VALUES
                                (@NombreCertificado, @NoCertificado, @RFC, @RazonSocial,
                                 @ArchivoCER, @ArchivoKEY, @PasswordKEY,
                                 @FechaVigenciaInicio, @FechaVigenciaFin,
                                 1, @EsPredeterminado, @Usuario);
                                SELECT CAST(SCOPE_IDENTITY() AS INT)", cnx, transaction);

                            cmdInsert.Parameters.AddWithValue("@NombreCertificado", request.NombreCertificado);
                            cmdInsert.Parameters.AddWithValue("@NoCertificado", noCertificado);
                            cmdInsert.Parameters.AddWithValue("@RFC", request.RFC);
                            cmdInsert.Parameters.AddWithValue("@RazonSocial", request.RazonSocial);
                            cmdInsert.Parameters.AddWithValue("@ArchivoCER", request.ArchivoCER);
                            cmdInsert.Parameters.AddWithValue("@ArchivoKEY", request.ArchivoKEY);
                            cmdInsert.Parameters.AddWithValue("@PasswordKEY", request.PasswordKEY);
                            cmdInsert.Parameters.AddWithValue("@FechaVigenciaInicio", vigenciaInicio);
                            cmdInsert.Parameters.AddWithValue("@FechaVigenciaFin", vigenciaFin);
                            cmdInsert.Parameters.AddWithValue("@EsPredeterminado", request.EsPredeterminado);
                            cmdInsert.Parameters.AddWithValue("@Usuario", request.Usuario);

                            int certificadoID = (int)cmdInsert.ExecuteScalar();

                            transaction.Commit();

                            respuesta.Resultado = true;
                            respuesta.Mensaje = "Certificado cargado correctamente";
                            respuesta.Datos = certificadoID;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            respuesta.Mensaje = "Error al guardar certificado: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error al procesar certificado: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Establecer certificado como predeterminado
        /// </summary>
        public Respuesta EstablecerPredeterminado(int certificadoID, string usuario)
        {
            var respuesta = new Respuesta { Resultado = false };

            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SP_EstablecerCertificadoPredeterminado", cnx)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@CertificadoID", certificadoID);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Certificado predeterminado establecido";
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Validar vigencia de certificados
        /// </summary>
        public List<CertificadoDigital> ValidarVigencia()
        {
            var lista = new List<CertificadoDigital>();

            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SP_ValidarVigenciaCertificados", cnx)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cnx.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        // Primera tabla: certificados desactivados (solo count)
                        dr.NextResult();

                        // Segunda tabla: certificados por vencer
                        while (dr.Read())
                        {
                            lista.Add(new CertificadoDigital
                            {
                                CertificadoID = (int)dr["CertificadoID"],
                                NombreCertificado = dr["NombreCertificado"].ToString(),
                                RFC = dr["RFC"].ToString(),
                                FechaVigenciaFin = (DateTime)dr["FechaVigenciaFin"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar vigencia: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Eliminar certificado
        /// </summary>
        public Respuesta Eliminar(int certificadoID, string usuario)
        {
            var respuesta = new Respuesta { Resultado = false };

            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    // Desactivar en lugar de eliminar (auditoría)
                    var cmd = new SqlCommand(@"
                        UPDATE CertificadosDigitales
                        SET Activo = 0,
                            EsPredeterminado = 0,
                            UsuarioModificacion = @Usuario,
                            FechaModificacion = GETDATE()
                        WHERE CertificadoID = @CertificadoID", cnx);

                    cmd.Parameters.AddWithValue("@CertificadoID", certificadoID);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Certificado eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error: " + ex.Message;
            }

            return respuesta;
        }
    }
}
