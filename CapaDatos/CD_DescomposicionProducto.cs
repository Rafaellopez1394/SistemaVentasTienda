// CapaDatos/CD_DescomposicionProducto.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace CapaDatos
{
    public class CD_DescomposicionProducto
    {
        private static CD_DescomposicionProducto _instancia;
        public static CD_DescomposicionProducto Instancia => _instancia ??= new CD_DescomposicionProducto();

        private CD_DescomposicionProducto() { }

        /// <summary>
        /// Registra la descomposición de un producto grande en productos más pequeños
        /// </summary>
        public Respuesta RegistrarDescomposicion(RegistrarDescomposicionPayload payload)
        {
            var respuesta = new Respuesta { Resultado = false };
            
            try
            {
                // Validaciones
                if (payload.ProductoOrigenID <= 0)
                {
                    respuesta.Mensaje = "Debe seleccionar el producto origen";
                    return respuesta;
                }

                if (payload.CantidadOrigen <= 0)
                {
                    respuesta.Mensaje = "La cantidad origen debe ser mayor a cero";
                    return respuesta;
                }

                if (payload.Detalle == null || payload.Detalle.Count == 0)
                {
                    respuesta.Mensaje = "Debe especificar al menos un producto resultante";
                    return respuesta;
                }

                // Validar que la suma de pesos resultantes no exceda el peso origen
                decimal totalPesoResultante = 0;
                foreach (var detalle in payload.Detalle)
                {
                    if (detalle.PesoUnidad.HasValue)
                    {
                        totalPesoResultante += detalle.CantidadResultante * detalle.PesoUnidad.Value;
                    }
                }

                // Convertir detalle a JSON
                string detalleJson = JsonConvert.SerializeObject(payload.Detalle);

                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SP_RegistrarDescomposicionProducto", cnx) 
                    { 
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120
                    };

                    cmd.Parameters.AddWithValue("@ProductoOrigenID", payload.ProductoOrigenID);
                    cmd.Parameters.AddWithValue("@CantidadOrigen", payload.CantidadOrigen);
                    cmd.Parameters.AddWithValue("@Usuario", payload.Usuario ?? "system");
                    cmd.Parameters.AddWithValue("@Observaciones", payload.Observaciones ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DetalleDescomposicion", detalleJson);
                    cmd.Parameters.AddWithValue("@SucursalID", payload.SucursalID);

                    var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                    var pDescomposicionID = new SqlParameter("@DescomposicionID", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pMensaje);
                    cmd.Parameters.Add(pDescomposicionID);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    respuesta.Mensaje = pMensaje.Value?.ToString() ?? "Error desconocido";
                    
                    if (pDescomposicionID.Value != DBNull.Value && Convert.ToInt32(pDescomposicionID.Value) > 0)
                    {
                        respuesta.Resultado = true;
                        respuesta.Datos = pDescomposicionID.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error al registrar descomposición: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Obtiene el historial de descomposiciones
        /// </summary>
        public List<HistorialDescomposicion> ObtenerHistorial()
        {
            var lista = new List<HistorialDescomposicion>();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SELECT * FROM vw_HistorialDescomposiciones ORDER BY FechaDescomposicion DESC", cnx);
                    
                    cnx.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new HistorialDescomposicion
                            {
                                DescomposicionID = Convert.ToInt32(dr["DescomposicionID"]),
                                FechaDescomposicion = Convert.ToDateTime(dr["FechaDescomposicion"]),
                                Usuario = dr["Usuario"].ToString(),
                                ProductoOrigen = dr["ProductoOrigen"].ToString(),
                                CantidadOrigen = Convert.ToDecimal(dr["CantidadOrigen"]),
                                UnidadOrigen = dr["UnidadOrigen"]?.ToString() ?? "UNIDAD",
                                ProductosResultantes = dr["ProductosResultantes"].ToString(),
                                Observaciones = dr["Observaciones"]?.ToString(),
                                Estatus = Convert.ToBoolean(dr["Estatus"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine("Error al obtener historial de descomposiciones: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene el detalle de una descomposición específica
        /// </summary>
        public DescomposicionProducto ObtenerPorId(int descomposicionID)
        {
            DescomposicionProducto descomposicion = null;
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    // Obtener encabezado
                    var cmdEncabezado = new SqlCommand(@"
                        SELECT d.*, p.Nombre AS ProductoOrigenNombre
                        FROM DescomposicionProducto d
                        INNER JOIN Productos p ON d.ProductoOrigenID = p.ProductoID
                        WHERE d.DescomposicionID = @DescomposicionID", cnx);
                    
                    cmdEncabezado.Parameters.AddWithValue("@DescomposicionID", descomposicionID);
                    
                    cnx.Open();
                    using (var dr = cmdEncabezado.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            descomposicion = new DescomposicionProducto
                            {
                                DescomposicionID = Convert.ToInt32(dr["DescomposicionID"]),
                                ProductoOrigenID = Convert.ToInt32(dr["ProductoOrigenID"]),
                                ProductoOrigenNombre = dr["ProductoOrigenNombre"].ToString(),
                                CantidadOrigen = Convert.ToDecimal(dr["CantidadOrigen"]),
                                FechaDescomposicion = Convert.ToDateTime(dr["FechaDescomposicion"]),
                                Usuario = dr["Usuario"].ToString(),
                                Observaciones = dr["Observaciones"]?.ToString(),
                                Estatus = Convert.ToBoolean(dr["Estatus"])
                            };
                        }
                    }

                    // Obtener detalle
                    if (descomposicion != null)
                    {
                        var cmdDetalle = new SqlCommand(@"
                            SELECT dd.*, p.Nombre AS ProductoResultanteNombre
                            FROM DetalleDescomposicion dd
                            INNER JOIN Productos p ON dd.ProductoResultanteID = p.ProductoID
                            WHERE dd.DescomposicionID = @DescomposicionID", cnx);
                        
                        cmdDetalle.Parameters.AddWithValue("@DescomposicionID", descomposicionID);
                        
                        using (var dr = cmdDetalle.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                descomposicion.Detalle.Add(new DetalleDescomposicion
                                {
                                    DetalleDescomposicionID = Convert.ToInt32(dr["DetalleDescomposicionID"]),
                                    DescomposicionID = Convert.ToInt32(dr["DescomposicionID"]),
                                    ProductoResultanteID = Convert.ToInt32(dr["ProductoResultanteID"]),
                                    ProductoResultanteNombre = dr["ProductoResultanteNombre"].ToString(),
                                    CantidadResultante = Convert.ToDecimal(dr["CantidadResultante"]),
                                    PesoUnidad = dr["PesoUnidad"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["PesoUnidad"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine("Error al obtener descomposición: " + ex.Message);
            }

            return descomposicion;
        }

        /// <summary>
        /// Obtiene productos configurados para venta por gramaje
        /// </summary>
        public List<Producto> ObtenerProductosGramaje()
        {
            var lista = new List<Producto>();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand(@"
                        SELECT ProductoID, Nombre, PrecioPorKilo, UnidadMedidaBase
                        FROM Productos
                        WHERE VentaPorGramaje = 1 AND Estatus = 1
                        ORDER BY Nombre", cnx);
                    
                    cnx.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                Nombre = dr["Nombre"].ToString(),
                                PrecioPorKilo = dr["PrecioPorKilo"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["PrecioPorKilo"]),
                                UnidadMedidaBase = dr["UnidadMedidaBase"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine("Error al obtener productos de gramaje: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Calcula el precio de un producto según los gramos especificados
        /// </summary>
        public Respuesta CalcularPrecioPorGramaje(int productoID, decimal gramos)
        {
            var respuesta = new Respuesta { Resultado = false };
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    var cmd = new SqlCommand("SP_CalcularPrecioPorGramaje", cnx) { CommandType = CommandType.StoredProcedure };
                    
                    cmd.Parameters.AddWithValue("@ProductoID", productoID);
                    cmd.Parameters.AddWithValue("@Gramos", gramos);
                    
                    var pPrecioCalculado = new SqlParameter("@PrecioCalculado", SqlDbType.Decimal) 
                    { 
                        Direction = ParameterDirection.Output,
                        Precision = 18,
                        Scale = 2
                    };
                    var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(pPrecioCalculado);
                    cmd.Parameters.Add(pMensaje);
                    
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    
                    respuesta.Mensaje = pMensaje.Value?.ToString() ?? "Error desconocido";
                    
                    if (pPrecioCalculado.Value != DBNull.Value)
                    {
                        decimal precioCalculado = Convert.ToDecimal(pPrecioCalculado.Value);
                        if (precioCalculado > 0)
                        {
                            respuesta.Resultado = true;
                            respuesta.Datos = precioCalculado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error al calcular precio: " + ex.Message;
            }

            return respuesta;
        }
    }
}
