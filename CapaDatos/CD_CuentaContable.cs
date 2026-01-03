using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_CuentaContable
    {
        public static CD_CuentaContable Instancia { get; } = new CD_CuentaContable();

        /// <summary>
        /// Obtiene todas las cuentas contables activas
        /// </summary>
        public List<CuentaContable> ObtenerCuentas()
        {
            List<CuentaContable> lista = new List<CuentaContable>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"SELECT CuentaID, Codigo, Nombre, Nivel, AceptaMovimientos, Activo, 
                                    Descripcion, CodigoSAT, Tipo, Naturaleza 
                                    FROM CatCuentas 
                                    WHERE Activo = 1 
                                    ORDER BY Codigo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CuentaContable
                            {
                                CuentaID = Convert.ToInt32(dr["CuentaID"]),
                                Codigo = dr["Codigo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Nivel = Convert.ToInt32(dr["Nivel"]),
                                AceptaMovimientos = Convert.ToBoolean(dr["AceptaMovimientos"]),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : "",
                                CodigoSAT = dr["CodigoSAT"] != DBNull.Value ? dr["CodigoSAT"].ToString() : "",
                                Tipo = dr["Tipo"] != DBNull.Value ? dr["Tipo"].ToString() : "",
                                Naturaleza = dr["Naturaleza"] != DBNull.Value ? dr["Naturaleza"].ToString() : ""
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    // Si la tabla no existe, devolver lista vacía
                    lista = new List<CuentaContable>();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una cuenta específica por su código
        /// </summary>
        public CuentaContable ObtenerPorCodigo(string codigo)
        {
            CuentaContable cuenta = null;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"SELECT TOP 1 CuentaID, Codigo, Nombre, Nivel, AceptaMovimientos, Activo, 
                                    Descripcion, CodigoSAT, Tipo, Naturaleza 
                                    FROM CatCuentas 
                                    WHERE Codigo = @Codigo AND Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cuenta = new CuentaContable
                            {
                                CuentaID = Convert.ToInt32(dr["CuentaID"]),
                                Codigo = dr["Codigo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Nivel = Convert.ToInt32(dr["Nivel"]),
                                AceptaMovimientos = Convert.ToBoolean(dr["AceptaMovimientos"]),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : "",
                                CodigoSAT = dr["CodigoSAT"] != DBNull.Value ? dr["CodigoSAT"].ToString() : "",
                                Tipo = dr["Tipo"] != DBNull.Value ? dr["Tipo"].ToString() : "",
                                Naturaleza = dr["Naturaleza"] != DBNull.Value ? dr["Naturaleza"].ToString() : ""
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    cuenta = null;
                }
            }

            return cuenta;
        }

        /// <summary>
        /// Obtiene cuentas de detalle (que aceptan movimientos) por nivel
        /// </summary>
        public List<CuentaContable> ObtenerCuentasDeDetalle(int nivel = 0)
        {
            List<CuentaContable> lista = new List<CuentaContable>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"SELECT CuentaID, Codigo, Nombre, Nivel, AceptaMovimientos, Activo, 
                                    Descripcion, CodigoSAT, Tipo, Naturaleza 
                                    FROM CatCuentas 
                                    WHERE Activo = 1 AND AceptaMovimientos = 1";

                    if (nivel > 0)
                    {
                        query += " AND Nivel = @Nivel";
                    }

                    query += " ORDER BY Codigo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (nivel > 0)
                    {
                        cmd.Parameters.AddWithValue("@Nivel", nivel);
                    }

                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CuentaContable
                            {
                                CuentaID = Convert.ToInt32(dr["CuentaID"]),
                                Codigo = dr["Codigo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Nivel = Convert.ToInt32(dr["Nivel"]),
                                AceptaMovimientos = Convert.ToBoolean(dr["AceptaMovimientos"]),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : "",
                                CodigoSAT = dr["CodigoSAT"] != DBNull.Value ? dr["CodigoSAT"].ToString() : "",
                                Tipo = dr["Tipo"] != DBNull.Value ? dr["Tipo"].ToString() : "",
                                Naturaleza = dr["Naturaleza"] != DBNull.Value ? dr["Naturaleza"].ToString() : ""
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    lista = new List<CuentaContable>();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la configuración de cuentas para nómina
        /// </summary>
        public ConfiguracionCuentasNomina ObtenerConfiguracionNomina()
        {
            ConfiguracionCuentasNomina config = new ConfiguracionCuentasNomina();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    // Intentar obtener la configuración de una tabla dedicada
                    string query = @"SELECT * FROM ConfiguracionCuentasNomina WHERE Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            config.CuentaSueldosYSalarios = dr["CuentaSueldosYSalarios"]?.ToString() ?? config.CuentaSueldosYSalarios;
                            config.CuentaPremioPuntualidad = dr["CuentaPremioPuntualidad"]?.ToString() ?? config.CuentaPremioPuntualidad;
                            config.CuentaPremioAsistencia = dr["CuentaPremioAsistencia"]?.ToString() ?? config.CuentaPremioAsistencia;
                            config.CuentaVacaciones = dr["CuentaVacaciones"]?.ToString() ?? config.CuentaVacaciones;
                            config.CuentaPrimaVacacional = dr["CuentaPrimaVacacional"]?.ToString() ?? config.CuentaPrimaVacacional;
                            config.CuentaAguinaldo = dr["CuentaAguinaldo"]?.ToString() ?? config.CuentaAguinaldo;
                            config.CuentaPTU = dr["CuentaPTU"]?.ToString() ?? config.CuentaPTU;
                            config.CuentaISRRetenido = dr["CuentaISRRetenido"]?.ToString() ?? config.CuentaISRRetenido;
                            config.CuentaIMSSObrero = dr["CuentaIMSSObrero"]?.ToString() ?? config.CuentaIMSSObrero;
                            config.CuentaInfonavit = dr["CuentaInfonavit"]?.ToString() ?? config.CuentaInfonavit;
                            config.CuentaFonacot = dr["CuentaFonacot"]?.ToString() ?? config.CuentaFonacot;
                            config.CuentaBancosNomina = dr["CuentaBancosNomina"]?.ToString() ?? config.CuentaBancosNomina;
                            config.CuentaIMSSPatronal = dr["CuentaIMSSPatronal"]?.ToString() ?? config.CuentaIMSSPatronal;
                            config.CuentaSARPatronal = dr["CuentaSARPatronal"]?.ToString() ?? config.CuentaSARPatronal;
                            config.CuentaInfonavitPatronal = dr["CuentaInfonavitPatronal"]?.ToString() ?? config.CuentaInfonavitPatronal;
                        }
                    }
                }
                catch (Exception)
                {
                    // Si la tabla no existe, usar configuración por defecto
                }
            }

            return config;
        }

        /// <summary>
        /// Obtiene el ID (Guid) de una cuenta por su código
        /// </summary>
        public Guid? ObtenerCuentaIDPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"SELECT TOP 1 CuentaID FROM CatCuentas 
                                    WHERE Codigo = @Codigo AND Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        // Si CuentaID es int, convertir a Guid
                        if (result is int)
                        {
                            // Generar un Guid determinístico basado en el ID
                            int id = (int)result;
                            byte[] bytes = new byte[16];
                            BitConverter.GetBytes(id).CopyTo(bytes, 0);
                            return new Guid(bytes);
                        }
                        else if (result is Guid)
                        {
                            return (Guid)result;
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Valida que una cuenta exista y sea válida para usar en pólizas
        /// </summary>
        public bool ValidarCuenta(string codigo)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"SELECT COUNT(*) FROM CatCuentas 
                                    WHERE Codigo = @Codigo AND Activo = 1 AND AceptaMovimientos = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    conn.Open();

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
