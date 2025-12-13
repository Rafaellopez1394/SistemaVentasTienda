// CapaDatos/CD_VentaPOS.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_VentaPOS
    {
        private static CD_VentaPOS _instancia;
        public static CD_VentaPOS Instancia => _instancia ??= new CD_VentaPOS();

        private CD_VentaPOS() { }

        // Buscar productos para agregar al carrito
        public List<ProductoPOS> BuscarProducto(string texto)
        {
            var lista = new List<ProductoPOS>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("BuscarProductoPOS", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Texto", texto);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ProductoPOS
                        {
                            ProductoID = Convert.ToInt32(dr["ProductoID"]),
                            Nombre = dr["Nombre"].ToString(),
                            CodigoInterno = dr["CodigoInterno"] == DBNull.Value ? "" : dr["CodigoInterno"].ToString(),
                            PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                            TasaIVAID = dr["TasaIVAID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TasaIVAID"]),
                            TasaIVA = dr["TasaIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TasaIVA"]),
                            DescripcionIVA = dr["DescripcionIVA"] == DBNull.Value ? "" : dr["DescripcionIVA"].ToString(),
                            StockDisponible = Convert.ToInt32(dr["StockDisponible"]),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            Categoria = dr["Categoria"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // Obtener lote activo de un producto (FIFO - First In First Out)
        public bool ObtenerLoteActivo(int productoID, out int loteID, out decimal precioCompra)
        {
            loteID = 0;
            precioCompra = 0;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 LoteID, PrecioCompra
                    FROM LotesProducto
                    WHERE ProductoID = @ProductoID 
                      AND Estatus = 1 
                      AND CantidadDisponible > 0
                    ORDER BY FechaEntrada ASC", cnx);
                
                cmd.Parameters.AddWithValue("@ProductoID", productoID);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        loteID = Convert.ToInt32(dr["LoteID"]);
                        precioCompra = Convert.ToDecimal(dr["PrecioCompra"]);
                        return true;
                    }
                }
            }
            return false;
        }

        // Validar crédito del cliente
        public ValidacionCredito ValidarCredito(Guid clienteID, decimal montoVenta)
        {
            var resultado = new ValidacionCredito();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ValidarCreditoCliente", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                cmd.Parameters.AddWithValue("@MontoVenta", montoVenta);
                
                var pTieneCredito = new SqlParameter("@TieneCredito", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                var pCreditoDisp = new SqlParameter("@CreditoDisponible", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                
                cmd.Parameters.Add(pTieneCredito);
                cmd.Parameters.Add(pCreditoDisp);
                cmd.Parameters.Add(pMensaje);

                cnx.Open();
                cmd.ExecuteNonQuery();

                resultado.TieneCredito = Convert.ToBoolean(pTieneCredito.Value);
                resultado.CreditoDisponible = pCreditoDisp.Value == DBNull.Value ? 0 : Convert.ToDecimal(pCreditoDisp.Value);
                resultado.Mensaje = pMensaje.Value.ToString();
            }
            
            return resultado;
        }

        // Registrar venta completa
        public bool RegistrarVenta(RegistrarVentaPayload payload, out string mensaje, out Guid ventaID)
        {
            mensaje = "";
            ventaID = Guid.Empty;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();

                try
                {
                    var venta = payload.Venta;
                    
                    // 1. Registrar venta
                    SqlCommand cmdVenta = new SqlCommand("RegistrarVentaPOS", cnx, transaction) { CommandType = CommandType.StoredProcedure };
                    
                    var pVentaID = new SqlParameter("@VentaID", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output };
                    var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                    
                    cmdVenta.Parameters.AddWithValue("@ClienteID", venta.ClienteID);
                    cmdVenta.Parameters.AddWithValue("@TipoVenta", venta.TipoVenta);
                    cmdVenta.Parameters.AddWithValue("@FormaPagoID", (object)venta.FormaPagoID ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@MetodoPagoID", (object)venta.MetodoPagoID ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@EfectivoRecibido", (object)venta.EfectivoRecibido ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@Cambio", (object)venta.Cambio ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@Subtotal", venta.Subtotal);
                    cmdVenta.Parameters.AddWithValue("@IVA", venta.IVA);
                    cmdVenta.Parameters.AddWithValue("@Total", venta.Total);
                    cmdVenta.Parameters.AddWithValue("@RequiereFactura", venta.RequiereFactura);
                    cmdVenta.Parameters.AddWithValue("@CajaID", venta.CajaID);
                    cmdVenta.Parameters.AddWithValue("@Usuario", venta.Usuario);
                    cmdVenta.Parameters.Add(pVentaID);
                    cmdVenta.Parameters.Add(pMensaje);

                    cmdVenta.ExecuteNonQuery();

                    ventaID = (Guid)pVentaID.Value;
                    mensaje = pMensaje.Value.ToString();

                    if (ventaID == Guid.Empty)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // 2. Registrar detalle de venta
                    foreach (var detalle in payload.Detalle)
                    {
                        SqlCommand cmdDetalle = new SqlCommand("RegistrarDetalleVentaPOS", cnx, transaction) { CommandType = CommandType.StoredProcedure };
                        
                        cmdDetalle.Parameters.AddWithValue("@VentaID", ventaID);
                        cmdDetalle.Parameters.AddWithValue("@ProductoID", detalle.ProductoID);
                        cmdDetalle.Parameters.AddWithValue("@LoteID", detalle.LoteID);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@PrecioVenta", detalle.PrecioVenta);
                        cmdDetalle.Parameters.AddWithValue("@PrecioCompra", detalle.PrecioCompra);
                        cmdDetalle.Parameters.AddWithValue("@TasaIVA", detalle.TasaIVA);
                        cmdDetalle.Parameters.AddWithValue("@MontoIVA", detalle.MontoIVA);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    mensaje = "Venta registrada correctamente";
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    mensaje = ex.Message;
                    ventaID = Guid.Empty;
                    return false;
                }
            }
        }

        // Obtener formas de pago
        public List<FormaPago> ObtenerFormasPago()
        {
            var lista = new List<FormaPago>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM CatFormasPago WHERE Estatus = 1 ORDER BY Descripcion", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new FormaPago
                        {
                            FormaPagoID = Convert.ToInt32(dr["FormaPagoID"]),
                            Clave = dr["Clave"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            RequiereCambio = Convert.ToBoolean(dr["RequiereCambio"]),
                            Estatus = Convert.ToBoolean(dr["Estatus"])
                        });
                    }
                }
            }
            return lista;
        }

        // Obtener métodos de pago
        public List<MetodoPago> ObtenerMetodosPago()
        {
            var lista = new List<MetodoPago>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM CatMetodosPago WHERE Estatus = 1 ORDER BY Descripcion", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MetodoPago
                        {
                            MetodoPagoID = Convert.ToInt32(dr["MetodoPagoID"]),
                            Clave = dr["Clave"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"])
                        });
                    }
                }
            }
            return lista;
        }

        // Apertura de caja
        public bool AperturaCaja(int cajaID, decimal montoInicial, string usuario, out string mensaje)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("AperturaCaja", cnx) { CommandType = CommandType.StoredProcedure };
                
                var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                
                cmd.Parameters.AddWithValue("@CajaID", cajaID);
                cmd.Parameters.AddWithValue("@MontoInicial", montoInicial);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.Add(pMensaje);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = pMensaje.Value.ToString();
                    return true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
        }

        // Cierre de caja
        public bool CierreCaja(int cajaID, string usuario, out decimal saldoFinal, out decimal totalVentas, out string mensaje)
        {
            saldoFinal = 0;
            totalVentas = 0;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("CierreCaja", cnx) { CommandType = CommandType.StoredProcedure };
                
                var pSaldoFinal = new SqlParameter("@SaldoFinal", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pTotalVentas = new SqlParameter("@TotalVentas", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                
                cmd.Parameters.AddWithValue("@CajaID", cajaID);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.Add(pSaldoFinal);
                cmd.Parameters.Add(pTotalVentas);
                cmd.Parameters.Add(pMensaje);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    
                    saldoFinal = pSaldoFinal.Value == DBNull.Value ? 0 : Convert.ToDecimal(pSaldoFinal.Value);
                    totalVentas = pTotalVentas.Value == DBNull.Value ? 0 : Convert.ToDecimal(pTotalVentas.Value);
                    mensaje = pMensaje.Value.ToString();
                    return true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
        }
    }
}
