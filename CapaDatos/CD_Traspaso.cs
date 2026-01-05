using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace CapaDatos
{
    public class CD_Traspaso
    {
        private static CD_Traspaso _instancia = null;

        private CD_Traspaso() { }

        public static CD_Traspaso Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_Traspaso();
                return _instancia;
            }
        }

        /// <summary>
        /// Registra un nuevo traspaso
        /// </summary>
        public bool RegistrarTraspaso(Traspaso traspaso, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    // Construir XML de detalles
                    var detallesXml = new XElement("Detalles");
                    foreach (var detalle in traspaso.Detalles)
                    {
                        detallesXml.Add(new XElement("Item",
                            new XAttribute("ProductoID", detalle.ProductoID),
                            new XAttribute("Cantidad", detalle.CantidadSolicitada),
                            new XAttribute("PrecioUnitario", detalle.PrecioUnitario)
                        ));
                    }

                    SqlCommand cmd = new SqlCommand("sp_RegistrarTraspaso", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.AddWithValue("@SucursalOrigenID", traspaso.SucursalOrigenID);
                    cmd.Parameters.AddWithValue("@SucursalDestinoID", traspaso.SucursalDestinoID);
                    cmd.Parameters.AddWithValue("@UsuarioRegistro", traspaso.UsuarioRegistro);
                    cmd.Parameters.AddWithValue("@Observaciones", (object)traspaso.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DetallesXML", detallesXml.ToString());

                    SqlParameter paramTraspasoID = new SqlParameter("@TraspasoID", SqlDbType.Int);
                    paramTraspasoID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramTraspasoID);

                    SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    traspaso.TraspasoID = Convert.ToInt32(paramTraspasoID.Value);
                    mensaje = paramMensaje.Value.ToString();
                    resultado = traspaso.TraspasoID > 0;
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    resultado = false;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Envía un traspaso (descuenta inventario origen)
        /// </summary>
        public bool EnviarTraspaso(int traspasoID, string usuarioEnvia, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_EnviarTraspaso", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.AddWithValue("@TraspasoID", traspasoID);
                    cmd.Parameters.AddWithValue("@UsuarioEnvia", usuarioEnvia);

                    SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = paramMensaje.Value.ToString();
                    resultado = mensaje.Contains("exitosamente");
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    resultado = false;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Recibe un traspaso (incrementa inventario destino)
        /// </summary>
        public bool RecibirTraspaso(int traspasoID, string usuarioRecibe, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RecibirTraspaso", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.AddWithValue("@TraspasoID", traspasoID);
                    cmd.Parameters.AddWithValue("@UsuarioRecibe", usuarioRecibe);

                    SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = paramMensaje.Value.ToString();
                    resultado = mensaje.Contains("exitosamente");
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    resultado = false;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Cancela un traspaso
        /// </summary>
        public bool CancelarTraspaso(int traspasoID, string motivoCancelacion, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_CancelarTraspaso", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TraspasoID", traspasoID);
                    cmd.Parameters.AddWithValue("@MotivoCancelacion", motivoCancelacion);

                    SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = paramMensaje.Value.ToString();
                    resultado = mensaje.Contains("exitosamente");
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    resultado = false;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Consulta traspasos con filtros
        /// </summary>
        public List<Traspaso> ConsultarTraspasos(DateTime? fechaInicio = null, DateTime? fechaFin = null, 
            int? sucursalID = null, string estatus = null)
        {
            List<Traspaso> lista = new List<Traspaso>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ConsultarTraspasos", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SucursalID", (object)sucursalID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estatus", (object)estatus ?? DBNull.Value);

                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Traspaso
                            {
                                TraspasoID = Convert.ToInt32(dr["TraspasoID"]),
                                FolioTraspaso = dr["FolioTraspaso"].ToString(),
                                FechaTraspaso = Convert.ToDateTime(dr["FechaTraspaso"]),
                                SucursalOrigen = new Sucursal { Nombre = dr["SucursalOrigen"].ToString() },
                                SucursalDestino = new Sucursal { Nombre = dr["SucursalDestino"].ToString() },
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                Estatus = dr["Estatus"].ToString(),
                                FechaEnvio = dr["FechaEnvio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaEnvio"]) : (DateTime?)null,
                                FechaRecepcion = dr["FechaRecepcion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRecepcion"]) : (DateTime?)null,
                                UsuarioEnvia = dr["UsuarioEnvia"]?.ToString(),
                                UsuarioRecibe = dr["UsuarioRecibe"]?.ToString(),
                                Observaciones = dr["Observaciones"]?.ToString(),
                                MotivoCancelacion = dr["MotivoCancelacion"]?.ToString(),
                                TotalProductos = Convert.ToInt32(dr["TotalProductos"]),
                                TotalCantidad = Convert.ToDecimal(dr["TotalCantidad"]),
                                ValorTotal = Convert.ToDecimal(dr["ValorTotal"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Traspaso>();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene el detalle de un traspaso
        /// </summary>
        public List<DetalleTraspaso> ObtenerDetalleTraspaso(int traspasoID)
        {
            List<DetalleTraspaso> lista = new List<DetalleTraspaso>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleTraspaso", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TraspasoID", traspasoID);

                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetalleTraspaso
                            {
                                DetalleTraspasoID = Convert.ToInt32(dr["DetalleTraspasoID"]),
                                ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                NombreProducto = dr["NombreProducto"].ToString(),
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                UnidadMedida = dr["UnidadMedidaBase"].ToString(),
                                CantidadSolicitada = Convert.ToDecimal(dr["CantidadSolicitada"]),
                                CantidadEnviada = dr["CantidadEnviada"] != DBNull.Value ? Convert.ToDecimal(dr["CantidadEnviada"]) : (decimal?)null,
                                CantidadRecibida = dr["CantidadRecibida"] != DBNull.Value ? Convert.ToDecimal(dr["CantidadRecibida"]) : (decimal?)null,
                                PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                                Observaciones = dr["Observaciones"]?.ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<DetalleTraspaso>();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene inventario por sucursal
        /// </summary>
        public List<InventarioSucursal> ObtenerInventarioSucursal(int? sucursalID = null, int? productoID = null)
        {
            List<InventarioSucursal> lista = new List<InventarioSucursal>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"
                        SELECT 
                            p.ProductoID,
                            p.Nombre AS NombreProducto,
                            p.CodigoInterno AS CodigoProducto,
                            p.UnidadMedidaBase,
                            s.SucursalID,
                            s.Nombre AS NombreSucursal,
                            COUNT(l.LoteID) AS TotalLotes,
                            ISNULL(SUM(l.CantidadDisponible), 0) AS CantidadDisponible,
                            ISNULL(SUM(l.CantidadTotal), 0) AS CantidadTotal,
                            ISNULL(AVG(l.PrecioCompra), 0) AS PrecioPromedioCompra,
                            ISNULL(AVG(l.PrecioVenta), 0) AS PrecioPromedioVenta
                        FROM Productos p
                        CROSS JOIN Sucursal s
                        LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID 
                            AND l.SucursalID = s.SucursalID 
                            AND l.Estatus = 1
                        WHERE p.Estatus = 1
                          AND (@SucursalID IS NULL OR s.SucursalID = @SucursalID)
                          AND (@ProductoID IS NULL OR p.ProductoID = @ProductoID)
                        GROUP BY 
                            p.ProductoID, p.Nombre, p.CodigoInterno, p.UnidadMedidaBase,
                            s.SucursalID, s.Nombre
                        HAVING ISNULL(SUM(l.CantidadDisponible), 0) > 0
                        ORDER BY s.Nombre, p.Nombre";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@SucursalID", (object)sucursalID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductoID", (object)productoID ?? DBNull.Value);

                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new InventarioSucursal
                            {
                                ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                NombreProducto = dr["NombreProducto"].ToString(),
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                UnidadMedida = dr["UnidadMedidaBase"].ToString(),
                                SucursalID = Convert.ToInt32(dr["SucursalID"]),
                                NombreSucursal = dr["NombreSucursal"].ToString(),
                                TotalLotes = Convert.ToInt32(dr["TotalLotes"]),
                                CantidadDisponible = Convert.ToDecimal(dr["CantidadDisponible"]),
                                CantidadTotal = Convert.ToDecimal(dr["CantidadTotal"]),
                                PrecioPromedioCompra = Convert.ToDecimal(dr["PrecioPromedioCompra"]),
                                PrecioPromedioVenta = Convert.ToDecimal(dr["PrecioPromedioVenta"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<InventarioSucursal>();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un traspaso específico con su detalle
        /// </summary>
        public Traspaso ObtenerTraspasoPorID(int traspasoID)
        {
            Traspaso traspaso = null;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                try
                {
                    string query = @"
                        SELECT t.*, 
                               so.Nombre AS NombreSucursalOrigen, so.RFC AS RFCOrigen,
                               sd.Nombre AS NombreSucursalDestino, sd.RFC AS RFCDestino
                        FROM Traspasos t
                        INNER JOIN Sucursal so ON t.SucursalOrigenID = so.SucursalID
                        INNER JOIN Sucursal sd ON t.SucursalDestinoID = sd.SucursalID
                        WHERE t.TraspasoID = @TraspasoID";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@TraspasoID", traspasoID);

                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            traspaso = new Traspaso
                            {
                                TraspasoID = Convert.ToInt32(dr["TraspasoID"]),
                                FolioTraspaso = dr["FolioTraspaso"].ToString(),
                                FechaTraspaso = Convert.ToDateTime(dr["FechaTraspaso"]),
                                SucursalOrigenID = Convert.ToInt32(dr["SucursalOrigenID"]),
                                SucursalDestinoID = Convert.ToInt32(dr["SucursalDestinoID"]),
                                SucursalOrigen = new Sucursal 
                                { 
                                    SucursalID = Convert.ToInt32(dr["SucursalOrigenID"]),
                                    Nombre = dr["NombreSucursalOrigen"].ToString(),
                                    RFC = dr["RFCOrigen"].ToString()
                                },
                                SucursalDestino = new Sucursal 
                                { 
                                    SucursalID = Convert.ToInt32(dr["SucursalDestinoID"]),
                                    Nombre = dr["NombreSucursalDestino"].ToString(),
                                    RFC = dr["RFCDestino"].ToString()
                                },
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                Observaciones = dr["Observaciones"]?.ToString(),
                                Estatus = dr["Estatus"].ToString(),
                                FechaEnvio = dr["FechaEnvio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaEnvio"]) : (DateTime?)null,
                                FechaRecepcion = dr["FechaRecepcion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRecepcion"]) : (DateTime?)null,
                                UsuarioEnvia = dr["UsuarioEnvia"]?.ToString(),
                                UsuarioRecibe = dr["UsuarioRecibe"]?.ToString(),
                                MotivoCancelacion = dr["MotivoCancelacion"]?.ToString()
                            };
                        }
                    }

                    // Obtener detalle
                    if (traspaso != null)
                    {
                        traspaso.Detalles = ObtenerDetalleTraspaso(traspasoID);
                    }
                }
                catch (Exception ex)
                {
                    traspaso = null;
                }
            }

            return traspaso;
        }
    }
}
