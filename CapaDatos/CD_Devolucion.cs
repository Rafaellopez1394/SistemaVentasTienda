// CapaDatos/CD_Devolucion.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace CapaDatos
{
    public class CD_Devolucion
    {
        private static CD_Devolucion _instancia;
        public static CD_Devolucion Instancia => _instancia ??= new CD_Devolucion();
        private CD_Devolucion() { }

        // Registrar devolución
        public Respuesta<int> RegistrarDevolucion(RegistrarDevolucionPayload payload)
        {
            var respuesta = new Respuesta<int>();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    using (var cmd = new SqlCommand("sp_RegistrarDevolucion", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 60;
                        
                        // Parámetros de entrada
                        cmd.Parameters.AddWithValue("@VentaID", payload.VentaID);
                        cmd.Parameters.AddWithValue("@TipoDevolucion", payload.TipoDevolucion);
                        cmd.Parameters.AddWithValue("@MotivoDevolucion", payload.MotivoDevolucion);
                        cmd.Parameters.AddWithValue("@FormaReintegro", payload.FormaReintegro);
                        cmd.Parameters.AddWithValue("@UsuarioRegistro", payload.UsuarioRegistro ?? "system");
                        cmd.Parameters.AddWithValue("@SucursalID", payload.SucursalID);
                        
                        // Convertir productos a JSON
                        string productosJson = JsonConvert.SerializeObject(payload.Productos);
                        cmd.Parameters.AddWithValue("@Productos", productosJson);
                        
                        // Parámetros de salida
                        var devolucionIDParam = new SqlParameter("@DevolucionID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(devolucionIDParam);
                        
                        var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(mensajeParam);
                        
                        cmd.ExecuteNonQuery();
                        
                        int devolucionID = devolucionIDParam.Value != DBNull.Value 
                            ? Convert.ToInt32(devolucionIDParam.Value) 
                            : 0;
                        string mensaje = mensajeParam.Value?.ToString() ?? "Error desconocido";
                        
                        respuesta.estado = devolucionID > 0;
                        respuesta.valor = mensaje;
                        respuesta.objeto = devolucionID;
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.estado = false;
                respuesta.valor = "Error al registrar devolución: " + ex.Message;
            }
            
            return respuesta;
        }

        // Obtener devoluciones con filtros
        public List<Devolucion> ObtenerDevoluciones(DateTime? fechaInicio = null, DateTime? fechaFin = null, 
            int? sucursalID = null, Guid? ventaID = null)
        {
            var lista = new List<Devolucion>();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    using (var cmd = new SqlCommand("sp_ObtenerDevoluciones", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SucursalID", (object)sucursalID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VentaID", (object)ventaID ?? DBNull.Value);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Devolucion
                                {
                                    DevolucionID = reader.GetInt32(reader.GetOrdinal("DevolucionID")),
                                    VentaID = reader.GetGuid(reader.GetOrdinal("VentaID")),
                                    NumeroVenta = reader.GetString(reader.GetOrdinal("NumeroVenta")),
                                    FechaDevolucion = reader.GetDateTime(reader.GetOrdinal("FechaDevolucion")),
                                    TipoDevolucion = reader.GetString(reader.GetOrdinal("TipoDevolucion")),
                                    MotivoDevolucion = reader.GetString(reader.GetOrdinal("MotivoDevolucion")),
                                    TotalDevuelto = reader.GetDecimal(reader.GetOrdinal("TotalDevuelto")),
                                    FormaReintegro = reader.GetString(reader.GetOrdinal("FormaReintegro")),
                                    MontoReintegrado = reader.GetDecimal(reader.GetOrdinal("MontoReintegrado")),
                                    CreditoGenerado = reader.GetDecimal(reader.GetOrdinal("CreditoGenerado")),
                                    UsuarioRegistro = reader.IsDBNull(reader.GetOrdinal("UsuarioRegistro")) ? "" : reader.GetString(reader.GetOrdinal("UsuarioRegistro")),
                                    NombreSucursal = reader.GetString(reader.GetOrdinal("NombreSucursal")),
                                    NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? "PÚBLICO EN GENERAL" : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                    RFCCliente = reader.IsDBNull(reader.GetOrdinal("RFCCliente")) ? "" : reader.GetString(reader.GetOrdinal("RFCCliente")),
                                    Estatus = reader.GetBoolean(reader.GetOrdinal("Estatus")),
                                    FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                    TotalProductos = reader.GetInt32(reader.GetOrdinal("TotalProductos"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                lista = new List<Devolucion>();
            }
            
            return lista;
        }

        // Obtener detalle de una devolución
        public Devolucion ObtenerDetalle(int devolucionID)
        {
            Devolucion devolucion = null;
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    using (var cmd = new SqlCommand("sp_ObtenerDetalleDevolucion", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DevolucionID", devolucionID);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Leer encabezado
                            if (reader.Read())
                            {
                                devolucion = new Devolucion
                                {
                                    DevolucionID = reader.GetInt32(reader.GetOrdinal("DevolucionID")),
                                    VentaID = reader.GetGuid(reader.GetOrdinal("VentaID")),
                                    NumeroVenta = reader.GetString(reader.GetOrdinal("NumeroVenta")),
                                    FechaDevolucion = reader.GetDateTime(reader.GetOrdinal("FechaDevolucion")),
                                    TipoDevolucion = reader.GetString(reader.GetOrdinal("TipoDevolucion")),
                                    MotivoDevolucion = reader.GetString(reader.GetOrdinal("MotivoDevolucion")),
                                    TotalDevuelto = reader.GetDecimal(reader.GetOrdinal("TotalDevuelto")),
                                    FormaReintegro = reader.GetString(reader.GetOrdinal("FormaReintegro")),
                                    MontoReintegrado = reader.GetDecimal(reader.GetOrdinal("MontoReintegrado")),
                                    CreditoGenerado = reader.GetDecimal(reader.GetOrdinal("CreditoGenerado")),
                                    UsuarioRegistro = reader.IsDBNull(reader.GetOrdinal("UsuarioRegistro")) ? "" : reader.GetString(reader.GetOrdinal("UsuarioRegistro")),
                                    NombreSucursal = reader.GetString(reader.GetOrdinal("NombreSucursal")),
                                    NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? "PÚBLICO EN GENERAL" : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                    RFCCliente = reader.IsDBNull(reader.GetOrdinal("RFCCliente")) ? "" : reader.GetString(reader.GetOrdinal("RFCCliente")),
                                    Estatus = reader.GetBoolean(reader.GetOrdinal("Estatus")),
                                    Productos = new List<DevolucionDetalle>()
                                };
                            }
                            
                            // Leer detalle (segundo resultset)
                            if (devolucion != null && reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    devolucion.Productos.Add(new DevolucionDetalle
                                    {
                                        DevolucionDetalleID = reader.GetInt32(reader.GetOrdinal("DevolucionDetalleID")),
                                        ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                                        CodigoInterno = reader.GetString(reader.GetOrdinal("CodigoInterno")),
                                        NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                                        CantidadDevuelta = reader.GetDecimal(reader.GetOrdinal("CantidadDevuelta")),
                                        PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                        SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                                        ReingresadoInventario = reader.GetBoolean(reader.GetOrdinal("ReingresadoInventario")),
                                        FechaReingreso = reader.IsDBNull(reader.GetOrdinal("FechaReingreso")) 
                                            ? (DateTime?)null 
                                            : reader.GetDateTime(reader.GetOrdinal("FechaReingreso")),
                                        NumeroLote = reader.IsDBNull(reader.GetOrdinal("NumeroLote")) 
                                            ? "" 
                                            : reader.GetString(reader.GetOrdinal("NumeroLote"))
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                devolucion = null;
            }
            
            return devolucion;
        }

        // Obtener reporte de devoluciones
        public ReporteDevolucion ObtenerReporte(DateTime fechaInicio, DateTime fechaFin, int? sucursalID = null)
        {
            var reporte = new ReporteDevolucion();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    using (var cmd = new SqlCommand("sp_ReporteDevoluciones", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                        cmd.Parameters.AddWithValue("@SucursalID", (object)sucursalID ?? DBNull.Value);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                reporte.TotalDevoluciones = reader.GetInt32(0);
                                reporte.DevolucionesTotales = reader.GetInt32(1);
                                reporte.DevolucionesParciales = reader.GetInt32(2);
                                reporte.MontoTotalDevuelto = reader.GetDecimal(3);
                                reporte.TotalEfectivoReintegrado = reader.GetDecimal(4);
                                reporte.TotalCreditoGenerado = reader.GetDecimal(5);
                                reporte.DevolucionesEfectivo = reader.GetDecimal(6);
                                reporte.DevolucionesCredito = reader.GetDecimal(7);
                                reporte.DevolucionesCambio = reader.GetDecimal(8);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }
            
            return reporte;
        }

        // Obtener productos más devueltos
        public List<ProductoMasDevuelto> ObtenerProductosMasDevueltos(DateTime fechaInicio, DateTime fechaFin, int top = 20)
        {
            var lista = new List<ProductoMasDevuelto>();
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    using (var cmd = new SqlCommand("sp_ProductosMasDevueltos", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                        cmd.Parameters.AddWithValue("@Top", top);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ProductoMasDevuelto
                                {
                                    ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                                    CodigoInterno = reader.GetString(reader.GetOrdinal("CodigoInterno")),
                                    NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                                    Categoria = reader.IsDBNull(reader.GetOrdinal("Categoria")) ? "" : reader.GetString(reader.GetOrdinal("Categoria")),
                                    TotalDevuelto = reader.GetDecimal(reader.GetOrdinal("TotalDevuelto")),
                                    NumeroDevoluciones = reader.GetInt32(reader.GetOrdinal("NumeroDevoluciones")),
                                    MontoTotalDevuelto = reader.GetDecimal(reader.GetOrdinal("MontoTotalDevuelto")),
                                    TotalVendido = reader.GetDecimal(reader.GetOrdinal("TotalVendido")),
                                    PorcentajeDevolucion = reader.GetDecimal(reader.GetOrdinal("PorcentajeDevolucion"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }
            
            return lista;
        }

        // Obtener detalle de venta para devolver
        public VentaCliente ObtenerDetalleVentaParaDevolucion(Guid ventaID)
        {
            VentaCliente venta = null;
            
            try
            {
                using (var cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    
                    string query = @"
                        SELECT 
                            v.VentaID,
                            v.NumeroVenta,
                            v.FechaVenta,
                            v.Total,
                            v.ClienteID,
                            c.Nombre AS NombreCliente,
                            c.RFC AS RFCCliente,
                            v.SucursalID,
                            s.Nombre AS NombreSucursal
                        FROM VentasClientes v
                        LEFT JOIN Cliente c ON v.ClienteID = c.ClienteID
                        INNER JOIN SUCURSAL s ON v.SucursalID = s.SucursalID
                        WHERE v.VentaID = @VentaID";
                    
                    using (var cmd = new SqlCommand(query, cnx))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", ventaID);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                venta = new VentaCliente
                                {
                                    VentaID = reader.GetGuid(0),
                                    NumeroVenta = reader.GetString(1),
                                    FechaVenta = reader.GetDateTime(2),
                                    Total = reader.GetDecimal(3),
                                    ClienteID = reader.IsDBNull(4) ? Guid.Empty : reader.GetGuid(4),
                                    RazonSocial = reader.IsDBNull(5) ? "PÚBLICO EN GENERAL" : reader.GetString(5)
                                };
                            }
                        }
                    }
                    
                    // Obtener detalle de productos
                    if (venta != null)
                    {
                        string queryDetalle = @"
                            SELECT 
                                dv.VentaDetalleID,
                                dv.ProductoID,
                                p.CodigoInterno,
                                p.Nombre AS NombreProducto,
                                dv.Cantidad,
                                dv.PrecioVenta,
                                (dv.Cantidad * dv.PrecioVenta) AS SubTotal,
                                dv.LoteID,
                                l.NumeroLote
                            FROM VentasDetalleClientes dv
                            INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
                            LEFT JOIN LotesProducto l ON dv.LoteID = l.LoteID
                            WHERE dv.VentaID = @VentaID";
                        
                        using (var cmd = new SqlCommand(queryDetalle, cnx))
                        {
                            cmd.Parameters.AddWithValue("@VentaID", ventaID);
                            
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    venta.Detalle.Add(new VentaDetalleCliente
                                    {
                                        ProductoID = reader.GetInt32(1),
                                        CodigoInterno = reader.GetString(2),
                                        Producto = reader.GetString(3),
                                        Cantidad = reader.GetInt32(4),
                                        PrecioVenta = reader.GetDecimal(5),
                                        LoteID = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                        NumeroLote = reader.IsDBNull(8) ? "" : reader.GetString(8)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                venta = null;
            }
            
            return venta;
        }
    }
}
