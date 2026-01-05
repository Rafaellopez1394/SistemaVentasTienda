using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos
{
    public class CD_Gasto
    {
        #region Singleton
        private static CD_Gasto _instancia = null;

        public static CD_Gasto Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_Gasto();
                }
                return _instancia;
            }
        }
        #endregion

        /// <summary>
        /// Registra un nuevo gasto
        /// </summary>
        public Guid RegistrarGasto(Gasto gasto, out string mensaje)
        {
            mensaje = string.Empty;
            Guid gastoID = Guid.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_RegistrarGasto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.AddWithValue("@CajaID", (object)gasto.CajaID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoriaGastoID", gasto.CategoriaGastoID);
                        cmd.Parameters.AddWithValue("@Concepto", gasto.Concepto);
                        cmd.Parameters.AddWithValue("@Descripcion", (object)gasto.Descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Monto", gasto.Monto);
                        cmd.Parameters.AddWithValue("@NumeroFactura", (object)gasto.NumeroFactura ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Proveedor", (object)gasto.Proveedor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FormaPagoID", (object)gasto.FormaPagoID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioRegistro", gasto.UsuarioRegistro);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)gasto.Observaciones ?? DBNull.Value);

                        SqlParameter paramGastoID = new SqlParameter("@GastoID", SqlDbType.UniqueIdentifier);
                        paramGastoID.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramGastoID);

                        SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                        paramMensaje.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramMensaje);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (paramGastoID.Value != DBNull.Value)
                            gastoID = (Guid)paramGastoID.Value;

                        mensaje = paramMensaje.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar gasto: " + ex.Message;
                gastoID = Guid.Empty;
            }

            return gastoID;
        }

        /// <summary>
        /// Obtiene gastos por rango de fechas
        /// </summary>
        public List<Gasto> ObtenerGastosPorFecha(DateTime fechaInicio, DateTime fechaFin, int? cajaID = null)
        {
            List<Gasto> lista = new List<Gasto>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerGastosPorFecha", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
                        cmd.Parameters.AddWithValue("@CajaID", (object)cajaID ?? DBNull.Value);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Gasto
                                {
                                    GastoID = reader.GetGuid(reader.GetOrdinal("GastoID")),
                                    CajaID = reader.IsDBNull(reader.GetOrdinal("CajaID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CajaID")),
                                    Concepto = reader.GetString(reader.GetOrdinal("Concepto")),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                    Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                    FechaGasto = reader.GetDateTime(reader.GetOrdinal("FechaGasto")),
                                    NumeroFactura = reader.IsDBNull(reader.GetOrdinal("NumeroFactura")) ? null : reader.GetString(reader.GetOrdinal("NumeroFactura")),
                                    Proveedor = reader.IsDBNull(reader.GetOrdinal("Proveedor")) ? null : reader.GetString(reader.GetOrdinal("Proveedor")),
                                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                    CategoriaDescripcion = reader.IsDBNull(reader.GetOrdinal("CategoriaDescripcion")) ? null : reader.GetString(reader.GetOrdinal("CategoriaDescripcion")),
                                    FormaPago = reader.IsDBNull(reader.GetOrdinal("FormaPago")) ? null : reader.GetString(reader.GetOrdinal("FormaPago")),
                                    RequiereAprobacion = reader.GetBoolean(reader.GetOrdinal("RequiereAprobacion")),
                                    EstaAprobado = reader.GetBoolean(reader.GetOrdinal("EstaAprobado")),
                                    AprobadoPor = reader.IsDBNull(reader.GetOrdinal("AprobadoPor")) ? null : reader.GetString(reader.GetOrdinal("AprobadoPor")),
                                    FechaAprobacion = reader.IsDBNull(reader.GetOrdinal("FechaAprobacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaAprobacion")),
                                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                    UsuarioRegistro = reader.GetString(reader.GetOrdinal("UsuarioRegistro")),
                                    FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                    Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                    Cancelado = reader.GetBoolean(reader.GetOrdinal("Cancelado")),
                                    MotivoCancelacion = reader.IsDBNull(reader.GetOrdinal("MotivoCancelacion")) ? null : reader.GetString(reader.GetOrdinal("MotivoCancelacion"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Gasto>();
            }

            return lista;
        }

        /// <summary>
        /// Obtiene resumen de gastos por categoría
        /// </summary>
        public List<ResumenGastos> ObtenerResumenGastos(DateTime fechaInicio, DateTime fechaFin, int? cajaID = null)
        {
            List<ResumenGastos> lista = new List<ResumenGastos>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ResumenGastos", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
                        cmd.Parameters.AddWithValue("@CajaID", (object)cajaID ?? DBNull.Value);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Primera tabla: resumen por categoría
                            while (reader.Read())
                            {
                                lista.Add(new ResumenGastos
                                {
                                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                    TotalGastos = reader.GetInt32(reader.GetOrdinal("TotalGastos")),
                                    MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                                    PromedioGasto = reader.GetDecimal(reader.GetOrdinal("PromedioGasto")),
                                    GastoMinimo = reader.GetDecimal(reader.GetOrdinal("GastoMinimo")),
                                    GastoMaximo = reader.GetDecimal(reader.GetOrdinal("GastoMaximo"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<ResumenGastos>();
            }

            return lista;
        }

        /// <summary>
        /// Obtiene cierre de caja con gastos incluidos
        /// </summary>
        public CierreCajaConGastos ObtenerCierreCajaConGastos(int cajaID, DateTime? fecha = null)
        {
            CierreCajaConGastos cierre = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CierreCajaConGastos", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CajaID", cajaID);
                        cmd.Parameters.AddWithValue("@Fecha", (object)fecha ?? DBNull.Value);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Primera tabla: resumen del cierre
                            if (reader.Read())
                            {
                                cierre = new CierreCajaConGastos
                                {
                                    CajaID = reader.GetInt32(reader.GetOrdinal("CajaID")),
                                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                    MontoInicial = reader.GetDecimal(reader.GetOrdinal("MontoInicial")),
                                    TotalVentas = reader.GetDecimal(reader.GetOrdinal("TotalVentas")),
                                    VentasEfectivo = reader.GetDecimal(reader.GetOrdinal("VentasEfectivo")),
                                    VentasTarjeta = reader.GetDecimal(reader.GetOrdinal("VentasTarjeta")),
                                    VentasTransferencia = reader.GetDecimal(reader.GetOrdinal("VentasTransferencia")),
                                    TotalGastos = reader.GetDecimal(reader.GetOrdinal("TotalGastos")),
                                    GastosEfectivo = reader.GetDecimal(reader.GetOrdinal("GastosEfectivo")),
                                    TotalRetiros = reader.GetDecimal(reader.GetOrdinal("TotalRetiros")),
                                    EfectivoEnCaja = reader.GetDecimal(reader.GetOrdinal("EfectivoEnCaja")),
                                    GananciaNeta = reader.GetDecimal(reader.GetOrdinal("GananciaNeta")),
                                    DetalleGastos = new List<GastoDetalleCierre>()
                                };
                            }

                            // Segunda tabla: detalle de gastos
                            if (reader.NextResult() && cierre != null)
                            {
                                while (reader.Read())
                                {
                                    cierre.DetalleGastos.Add(new GastoDetalleCierre
                                    {
                                        Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                        Concepto = reader.GetString(reader.GetOrdinal("Concepto")),
                                        Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                        FechaGasto = reader.GetDateTime(reader.GetOrdinal("FechaGasto")),
                                        UsuarioRegistro = reader.GetString(reader.GetOrdinal("UsuarioRegistro"))
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cierre = null;
            }

            return cierre;
        }

        /// <summary>
        /// Obtiene todas las categorías de gastos activas
        /// </summary>
        public List<CategoriaGasto> ObtenerCategoriasGastos()
        {
            List<CategoriaGasto> lista = new List<CategoriaGasto>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    string query = @"SELECT CategoriaGastoID, Nombre, Descripcion, 
                                           RequiereAprobacion, MontoMaximo, Activo, FechaCreacion
                                     FROM CatCategoriasGastos
                                     WHERE Activo = 1
                                     ORDER BY Nombre";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new CategoriaGasto
                                {
                                    CategoriaGastoID = reader.GetInt32(reader.GetOrdinal("CategoriaGastoID")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                    RequiereAprobacion = reader.GetBoolean(reader.GetOrdinal("RequiereAprobacion")),
                                    MontoMaximo = reader.IsDBNull(reader.GetOrdinal("MontoMaximo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("MontoMaximo")),
                                    Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                    FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<CategoriaGasto>();
            }

            return lista;
        }

        /// <summary>
        /// Cancela un gasto
        /// </summary>
        public bool CancelarGasto(Guid gastoID, string motivo, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    string query = @"UPDATE Gastos 
                                     SET Cancelado = 1, MotivoCancelacion = @Motivo
                                     WHERE GastoID = @GastoID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@GastoID", gastoID);
                        cmd.Parameters.AddWithValue("@Motivo", motivo);

                        conn.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                            mensaje = "Gasto cancelado exitosamente";
                        }
                        else
                        {
                            mensaje = "No se encontró el gasto a cancelar";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al cancelar gasto: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el total de gastos para una fecha específica
        /// </summary>
        public decimal ObtenerTotalGastosDia(DateTime fecha, int? cajaID = null)
        {
            decimal total = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    string query = @"SELECT ISNULL(SUM(Monto), 0) AS Total
                                     FROM Gastos
                                     WHERE CAST(FechaGasto AS DATE) = @Fecha
                                       AND (@CajaID IS NULL OR CajaID = @CajaID)
                                       AND Cancelado = 0
                                       AND Activo = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", fecha.Date);
                        cmd.Parameters.AddWithValue("@CajaID", (object)cajaID ?? DBNull.Value);

                        conn.Open();
                        total = (decimal)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                total = 0;
            }

            return total;
        }

        /// <summary>
        /// Obtiene el concentrado de gastos por categoría para un cierre de caja
        /// </summary>
        public List<ConcentradoGastoCierre> ObtenerConcentradoGastosCierre(int cajaID, DateTime fecha)
        {
            List<ConcentradoGastoCierre> lista = new List<ConcentradoGastoCierre>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerConcentradoGastosCierre", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CajaID", cajaID);
                        cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ConcentradoGastoCierre
                                {
                                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                    NumeroGastos = reader.GetInt32(reader.GetOrdinal("NumeroGastos")),
                                    TotalMonto = reader.GetDecimal(reader.GetOrdinal("TotalMonto"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<ConcentradoGastoCierre>();
            }

            return lista;
        }
    }
}
