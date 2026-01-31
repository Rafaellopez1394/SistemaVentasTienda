using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaModelo;

namespace CapaDatos
{
    public class CD_InventarioInicial
    {
        // Iniciar nueva carga de inventario inicial
        public Dictionary<string, object> IniciarCarga(string usuarioCarga, int sucursalID, string comentarios = null)
        {
            var resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_IniciarCargaInventarioInicial", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UsuarioCarga", usuarioCarga);
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalID);
                        cmd.Parameters.AddWithValue("@Comentarios", (object)comentarios ?? DBNull.Value);

                        SqlParameter outputCargaID = new SqlParameter("@CargaID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputCargaID);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado["CargaID"] = Convert.ToInt32(dr["CargaID"]);
                                resultado["Mensaje"] = dr["Mensaje"].ToString();
                                resultado["Resultado"] = Convert.ToInt32(dr["Resultado"]) == 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["CargaID"] = 0;
                resultado["Mensaje"] = "Error al iniciar carga: " + ex.Message;
                resultado["Resultado"] = false;
            }

            return resultado;
        }

        // Agregar producto a la carga
        public Dictionary<string, object> AgregarProducto(
            int cargaID, 
            int productoID, 
            decimal cantidad, 
            decimal costoUnitario, 
            decimal precioVenta,
            DateTime? fechaCaducidad = null,
            string comentarios = null)
        {
            var resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_AgregarProductoInventarioInicial", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CargaID", cargaID);
                        cmd.Parameters.AddWithValue("@ProductoID", productoID);
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@CostoUnitario", costoUnitario);
                        cmd.Parameters.AddWithValue("@PrecioVenta", precioVenta);
                        cmd.Parameters.AddWithValue("@FechaCaducidad", (object)fechaCaducidad ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Comentarios", (object)comentarios ?? DBNull.Value);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado["Mensaje"] = dr["Mensaje"].ToString();
                                resultado["Resultado"] = Convert.ToInt32(dr["Resultado"]) == 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["Mensaje"] = "Error al agregar producto: " + ex.Message;
                resultado["Resultado"] = false;
            }

            return resultado;
        }

        // Finalizar y aplicar la carga
        public Dictionary<string, object> FinalizarCarga(int cargaID, string usuario)
        {
            var resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FinalizarCargaInventarioInicial", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // 2 minutos para procesar inventario grande

                        cmd.Parameters.AddWithValue("@CargaID", cargaID);
                        cmd.Parameters.AddWithValue("@Usuario", usuario);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado["Mensaje"] = dr["Mensaje"].ToString();
                                resultado["ProductosAplicados"] = Convert.ToInt32(dr["ProductosAplicados"]);
                                resultado["UnidadesAplicadas"] = Convert.ToDecimal(dr["UnidadesAplicadas"]);
                                resultado["ValorTotal"] = Convert.ToDecimal(dr["ValorTotal"]);
                                resultado["Resultado"] = Convert.ToInt32(dr["Resultado"]) == 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["Mensaje"] = "Error al finalizar carga: " + ex.Message;
                resultado["Resultado"] = false;
            }

            return resultado;
        }

        // Obtener productos de una carga o todos los productos disponibles
        public List<object> ObtenerProductos(int? cargaID = null)
        {
            var productos = new List<object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ObtenerProductosParaInventarioInicial", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CargaID", (object)cargaID ?? DBNull.Value);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                if (cargaID.HasValue)
                                {
                                    // Productos de una carga específica
                                    productos.Add(new InventarioInicialDetalle
                                    {
                                        DetalleID = Convert.ToInt32(dr["DetalleID"]),
                                        ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                        NombreProducto = dr["NombreProducto"].ToString(),
                                        CodigoInterno = dr["CodigoInterno"].ToString(),
                                        CantidadCargada = Convert.ToDecimal(dr["CantidadCargada"]),
                                        CostoUnitario = Convert.ToDecimal(dr["CostoUnitario"]),
                                        PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                                        FechaCaducidad = dr["FechaCaducidad"] == DBNull.Value ? 
                                            (DateTime?)null : Convert.ToDateTime(dr["FechaCaducidad"]),
                                        Comentarios = dr["Comentarios"].ToString(),
                                        ValorTotal = Convert.ToDecimal(dr["ValorTotal"])
                                    });
                                }
                                else
                                {
                                    // Todos los productos disponibles
                                    productos.Add(new ProductoInventarioInicial
                                    {
                                        ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                        NombreProducto = dr["NombreProducto"].ToString(),
                                        CodigoInterno = dr["CodigoInterno"].ToString(),
                                        StockActual = Convert.ToDecimal(dr["StockActual"]),
                                        CantidadNueva = 0,
                                        CostoUnitario = 0,
                                        PrecioVenta = 0
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener productos: " + ex.Message);
            }

            return productos;
        }

        // Eliminar producto de una carga (solo antes de finalizar)
        public Dictionary<string, object> EliminarProducto(int detalleID)
        {
            var resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_EliminarProductoInventarioInicial", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DetalleID", detalleID);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado["Mensaje"] = dr["Mensaje"].ToString();
                                resultado["Resultado"] = Convert.ToInt32(dr["Resultado"]) == 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["Mensaje"] = "Error al eliminar producto: " + ex.Message;
                resultado["Resultado"] = false;
            }

            return resultado;
        }

        // Cancelar carga activa (eliminar carga y sus productos sin aplicar al inventario)
        public Dictionary<string, object> CancelarCarga(int cargaID)
        {
            var resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    
                    // Verificar que la carga esté activa (no finalizada)
                    string queryCheck = "SELECT Activo FROM InventarioInicial WHERE CargaID = @CargaID";
                    using (SqlCommand cmdCheck = new SqlCommand(queryCheck, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@CargaID", cargaID);
                        var activo = cmdCheck.ExecuteScalar();
                        
                        if (activo == null)
                        {
                            resultado["Mensaje"] = "La carga no existe.";
                            resultado["Resultado"] = false;
                            return resultado;
                        }
                        
                        // Si Activo = 0, significa que está finalizada
                        if (!Convert.ToBoolean(activo))
                        {
                            resultado["Mensaje"] = "No se puede cancelar una carga ya finalizada.";
                            resultado["Resultado"] = false;
                            return resultado;
                        }
                    }
                    
                    // Eliminar primero los detalles (productos)
                    string queryDetalles = "DELETE FROM InventarioInicialDetalle WHERE CargaID = @CargaID";
                    using (SqlCommand cmdDetalles = new SqlCommand(queryDetalles, con))
                    {
                        cmdDetalles.Parameters.AddWithValue("@CargaID", cargaID);
                        cmdDetalles.ExecuteNonQuery();
                    }
                    
                    // Luego eliminar la carga
                    string queryCarga = "DELETE FROM InventarioInicial WHERE CargaID = @CargaID";
                    using (SqlCommand cmdCarga = new SqlCommand(queryCarga, con))
                    {
                        cmdCarga.Parameters.AddWithValue("@CargaID", cargaID);
                        int filasAfectadas = cmdCarga.ExecuteNonQuery();
                        
                        if (filasAfectadas > 0)
                        {
                            resultado["Mensaje"] = "Carga cancelada correctamente.";
                            resultado["Resultado"] = true;
                        }
                        else
                        {
                            resultado["Mensaje"] = "No se pudo cancelar la carga.";
                            resultado["Resultado"] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["Mensaje"] = "Error al cancelar la carga: " + ex.Message;
                resultado["Resultado"] = false;
            }

            return resultado;
        }

        // Obtener historial de cargas
        public List<InventarioInicial> ObtenerHistorial()
        {
            var cargas = new List<InventarioInicial>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    string query = "SELECT * FROM VW_HistorialInventarioInicial ORDER BY FechaCarga DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cargas.Add(new InventarioInicial
                                {
                                    CargaID = Convert.ToInt32(dr["CargaID"]),
                                    FechaCarga = Convert.ToDateTime(dr["FechaCarga"]),
                                    UsuarioCarga = dr["UsuarioCarga"].ToString(),
                                    NombreSucursal = dr["NombreSucursal"].ToString(),
                                    TotalProductos = Convert.ToInt32(dr["TotalProductos"]),
                                    TotalUnidades = Convert.ToDecimal(dr["TotalUnidades"]),
                                    ValorTotal = Convert.ToDecimal(dr["ValorTotal"]),
                                    Comentarios = dr["Comentarios"].ToString(),
                                    Estado = dr["Estado"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener historial: " + ex.Message);
            }

            return cargas;
        }

        // Obtener carga activa actual (en proceso)
        public InventarioInicial ObtenerCargaActiva(int sucursalID)
        {
            InventarioInicial carga = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    string query = @"
                        SELECT TOP 1 i.*, s.Nombre AS NombreSucursal
                        FROM InventarioInicial i
                        INNER JOIN SUCURSAL s ON i.SucursalID = s.SucursalID
                        WHERE i.Activo = 1 AND i.SucursalID = @SucursalID
                        ORDER BY i.FechaCarga DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalID);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                carga = new InventarioInicial
                                {
                                    CargaID = Convert.ToInt32(dr["CargaID"]),
                                    FechaCarga = Convert.ToDateTime(dr["FechaCarga"]),
                                    UsuarioCarga = dr["UsuarioCarga"].ToString(),
                                    NombreSucursal = dr["NombreSucursal"].ToString(),
                                    SucursalID = Convert.ToInt32(dr["SucursalID"]),
                                    TotalProductos = Convert.ToInt32(dr["TotalProductos"]),
                                    TotalUnidades = Convert.ToDecimal(dr["TotalUnidades"]),
                                    ValorTotal = Convert.ToDecimal(dr["ValorTotal"]),
                                    Comentarios = dr["Comentarios"].ToString(),
                                    Activo = true,
                                    Estado = "En Proceso"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener carga activa: " + ex.Message);
            }

            return carga;
        }

        // Limpiar inventario existente de una sucursal (para reemplazar con inventario inicial)
        public Dictionary<string, object> LimpiarInventarioSucursal(int sucursalID)
        {
            var resultado = new Dictionary<string, object>();
            int lotesEliminados = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CN))
                {
                    con.Open();
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. Contar lotes a eliminar
                            using (SqlCommand cmdCount = new SqlCommand(@"
                                SELECT COUNT(*) 
                                FROM Lote 
                                WHERE SucursalID = @SucursalID", con, trans))
                            {
                                cmdCount.Parameters.AddWithValue("@SucursalID", sucursalID);
                                lotesEliminados = (int)cmdCount.ExecuteScalar();
                            }

                            // 2. Eliminar lotes de la sucursal
                            using (SqlCommand cmdDelete = new SqlCommand(@"
                                DELETE FROM Lote 
                                WHERE SucursalID = @SucursalID", con, trans))
                            {
                                cmdDelete.Parameters.AddWithValue("@SucursalID", sucursalID);
                                cmdDelete.ExecuteNonQuery();
                            }

                            trans.Commit();

                            resultado["Resultado"] = true;
                            resultado["LotesEliminados"] = lotesEliminados;
                            resultado["Mensaje"] = $"Inventario anterior eliminado: {lotesEliminados} lotes";
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new Exception("Error al limpiar inventario: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado["Resultado"] = false;
                resultado["LotesEliminados"] = 0;
                resultado["Mensaje"] = ex.Message;
            }

            return resultado;
        }
    }
}
