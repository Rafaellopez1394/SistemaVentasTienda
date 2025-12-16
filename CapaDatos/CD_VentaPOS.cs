// CapaDatos/CD_VentaPOS.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

        // Validar si caja está abierta
        public bool ValidarCajaAbierta(int cajaID, out string mensaje, out DateTime? fechaApertura, out decimal saldoActual)
        {
            mensaje = "";
            fechaApertura = null;
            saldoActual = 0;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ValidarCajaAbierta", cnx) { CommandType = CommandType.StoredProcedure };
                
                var pEstaAbierta = new SqlParameter("@EstaAbierta", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                var pMovAperturaID = new SqlParameter("@MovimientoAperturaID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pFechaApertura = new SqlParameter("@FechaApertura", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
                var pSaldoActual = new SqlParameter("@SaldoActual", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                
                cmd.Parameters.AddWithValue("@CajaID", cajaID);
                cmd.Parameters.Add(pEstaAbierta);
                cmd.Parameters.Add(pMovAperturaID);
                cmd.Parameters.Add(pFechaApertura);
                cmd.Parameters.Add(pSaldoActual);
                cmd.Parameters.Add(pMensaje);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    
                    bool estaAbierta = Convert.ToBoolean(pEstaAbierta.Value);
                    mensaje = pMensaje.Value.ToString();
                    
                    if (pFechaApertura.Value != DBNull.Value)
                        fechaApertura = Convert.ToDateTime(pFechaApertura.Value);
                    
                    if (pSaldoActual.Value != DBNull.Value)
                        saldoActual = Convert.ToDecimal(pSaldoActual.Value);
                    
                    return estaAbierta;
                }
                catch (Exception ex)
                {
                    mensaje = "Error al validar estado de caja: " + ex.Message;
                    return false;
                }
            }
        }

        // Obtener estado completo de caja
        public EstadoCaja ObtenerEstadoCaja(int cajaID)
        {
            EstadoCaja estado = null;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ObtenerEstadoCaja", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@CajaID", cajaID);

                try
                {
                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            estado = new EstadoCaja
                            {
                                CajaID = Convert.ToInt32(dr["CajaID"]),
                                EstaAbierta = Convert.ToBoolean(dr["EstaAbierta"]),
                                FechaApertura = dr["FechaApertura"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaApertura"]),
                                SaldoActual = Convert.ToDecimal(dr["SaldoActual"]),
                                NumeroVentas = Convert.ToInt32(dr["NumeroVentas"]),
                                TotalVentas = Convert.ToDecimal(dr["TotalVentas"])
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    estado = new EstadoCaja { CajaID = cajaID, EstaAbierta = false };
                }
            }
            
            return estado ?? new EstadoCaja { CajaID = cajaID, EstaAbierta = false };
        }

        // Cierre completo con arqueo
        public bool CierreCajaCompleto(int cajaID, string usuario, decimal montoRealEfectivo, decimal montoRealTarjeta, 
            decimal montoRealTransferencia, string observaciones, out int corteID, out decimal diferencia, out string mensaje)
        {
            corteID = 0;
            diferencia = 0;
            decimal saldoFinal = 0;
            decimal totalVentas = 0;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("CierreCajaCompleto", cnx) { CommandType = CommandType.StoredProcedure };
                
                var pSaldoFinal = new SqlParameter("@SaldoFinal", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pTotalVentas = new SqlParameter("@TotalVentas", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pDiferencia = new SqlParameter("@Diferencia", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                var pCorteID = new SqlParameter("@CorteID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                
                cmd.Parameters.AddWithValue("@CajaID", cajaID);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@MontoRealEfectivo", montoRealEfectivo);
                cmd.Parameters.AddWithValue("@MontoRealTarjeta", montoRealTarjeta);
                cmd.Parameters.AddWithValue("@MontoRealTransferencia", montoRealTransferencia);
                cmd.Parameters.AddWithValue("@Observaciones", observaciones ?? "");
                cmd.Parameters.Add(pSaldoFinal);
                cmd.Parameters.Add(pTotalVentas);
                cmd.Parameters.Add(pDiferencia);
                cmd.Parameters.Add(pCorteID);
                cmd.Parameters.Add(pMensaje);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    
                    saldoFinal = pSaldoFinal.Value == DBNull.Value ? 0 : Convert.ToDecimal(pSaldoFinal.Value);
                    totalVentas = pTotalVentas.Value == DBNull.Value ? 0 : Convert.ToDecimal(pTotalVentas.Value);
                    diferencia = pDiferencia.Value == DBNull.Value ? 0 : Convert.ToDecimal(pDiferencia.Value);
                    corteID = pCorteID.Value == DBNull.Value ? 0 : Convert.ToInt32(pCorteID.Value);
                    mensaje = pMensaje.Value.ToString();
                    return corteID > 0;
                }
                catch (Exception ex)
                {
                    mensaje = "Error al cerrar caja: " + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// ✅ PROCESO CRÍTICO: Cerrar día y generar póliza automática de ingresos
        /// Consolida todas las ventas del día y genera póliza contable con desglose de IVA
        /// </summary>
        public bool CerrarDia(DateTime fecha, string usuario, out Guid? polizaID, out string mensaje)
        {
            polizaID = null;
            mensaje = "";

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction tran = cnx.BeginTransaction();

                try
                {
                    // 1. Obtener ventas del día con desglose por tasa de IVA
                    string queryVentas = @"
                        SELECT 
                            COUNT(DISTINCT v.VentaID) AS NumVentas,
                            SUM(v.Subtotal) AS TotalSubtotal,
                            SUM(v.IVA) AS TotalIVA,
                            SUM(v.Total) AS TotalGeneral,
                            SUM(CASE WHEN v.TasaIVA = 0 THEN v.Subtotal ELSE 0 END) AS Subtotal0,
                            SUM(CASE WHEN v.TasaIVA = 0.08 THEN v.Subtotal ELSE 0 END) AS Subtotal8,
                            SUM(CASE WHEN v.TasaIVA = 0.16 THEN v.Subtotal ELSE 0 END) AS Subtotal16,
                            SUM(CASE WHEN v.TasaIVA = 0 THEN v.IVA ELSE 0 END) AS IVA0,
                            SUM(CASE WHEN v.TasaIVA = 0.08 THEN v.IVA ELSE 0 END) AS IVA8,
                            SUM(CASE WHEN v.TasaIVA = 0.16 THEN v.IVA ELSE 0 END) AS IVA16
                        FROM VentasClientes v
                        WHERE CAST(v.FechaVenta AS DATE) = @Fecha";

                    SqlCommand cmdVentas = new SqlCommand(queryVentas, cnx, tran);
                    cmdVentas.Parameters.AddWithValue("@Fecha", fecha.Date);

                    decimal totalSubtotal = 0, totalIVA = 0, totalGeneral = 0;
                    decimal subtotal0 = 0, subtotal8 = 0, subtotal16 = 0;
                    decimal iva0 = 0, iva8 = 0, iva16 = 0;
                    int numVentas = 0;

                    using (SqlDataReader dr = cmdVentas.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            numVentas = dr["NumVentas"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NumVentas"]);
                            totalSubtotal = dr["TotalSubtotal"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalSubtotal"]);
                            totalIVA = dr["TotalIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalIVA"]);
                            totalGeneral = dr["TotalGeneral"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalGeneral"]);
                            subtotal0 = dr["Subtotal0"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Subtotal0"]);
                            subtotal8 = dr["Subtotal8"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Subtotal8"]);
                            subtotal16 = dr["Subtotal16"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Subtotal16"]);
                            iva0 = dr["IVA0"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IVA0"]);
                            iva8 = dr["IVA8"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IVA8"]);
                            iva16 = dr["IVA16"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IVA16"]);
                        }
                    }

                    if (numVentas == 0)
                    {
                        mensaje = "No hay ventas registradas para el día " + fecha.ToString("dd/MM/yyyy");
                        tran.Rollback();
                        return false;
                    }

                    // 2. Crear póliza de INGRESO
                    Poliza poliza = new Poliza
                    {
                        PolizaID = Guid.NewGuid(),
                        TipoPoliza = "INGRESO",
                        FechaPoliza = fecha,
                        Referencia = $"CIERRE-DIA-{fecha:yyyyMMdd}",
                        Concepto = $"Ventas del día {fecha:dd/MM/yyyy} - {numVentas} ventas",
                        TotalDebe = totalGeneral,
                        TotalHaber = totalGeneral,
                        Estatus = "VIGENTE",
                        EsAutomatica = true,
                        DocumentoOrigen = $"CIERRE_DIA_{fecha:yyyyMMdd}",
                        PeriodoContable = fecha.ToString("yyyy-MM"),
                        Detalles = new List<PolizaDetalle>()
                    };

                    // 3. Movimientos contables
                    int secuencia = 1;

                    // DEBE: Caja o Clientes (según si son crédito o contado)
                    // Por simplicidad, usamos CAJA (1010)
                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ObtenerCuentaID("1010", cnx, tran), // Caja
                        Debe = totalGeneral,
                        Haber = 0,
                        Concepto = $"Ingreso por ventas del día"
                    });

                    // HABER: Ventas por tasa de IVA (4000)
                    poliza.Detalles.Add(new PolizaDetalle
                    {
                        CuentaID = ObtenerCuentaID("4000", cnx, tran), // Ventas
                        Debe = 0,
                        Haber = totalSubtotal,
                        Concepto = "Ventas sin IVA"
                    });

                    // HABER: IVA Trasladado por tasa
                    if (iva0 > 0)
                    {
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaID("2100", cnx, tran), // IVA Trasladado 0%
                            Debe = 0,
                            Haber = iva0,
                            Concepto = "IVA Trasladado 0%"
                        });
                    }

                    if (iva8 > 0)
                    {
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaID("2101", cnx, tran), // IVA Trasladado 8%
                            Debe = 0,
                            Haber = iva8,
                            Concepto = "IVA Trasladado 8%"
                        });
                    }

                    if (iva16 > 0)
                    {
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaID("2102", cnx, tran), // IVA Trasladado 16%
                            Debe = 0,
                            Haber = iva16,
                            Concepto = "IVA Trasladado 16%"
                        });
                    }

                    // 4. Crear póliza usando CD_Poliza
                    bool exito = CD_Poliza.Instancia.CrearPoliza(poliza, cnx, tran);

                    if (!exito)
                    {
                        throw new Exception("Error al crear póliza contable");
                    }

                    polizaID = poliza.PolizaID;
                    mensaje = $"Cierre de día exitoso. Póliza #{poliza.PolizaID} generada. Total: ${totalGeneral:N2}";

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    mensaje = "Error al cerrar día: " + ex.Message;
                    return false;
                }
            }
        }

        // Método auxiliar para obtener CuentaID desde CatalogoContable
        private int ObtenerCuentaID(string codigoCuenta, SqlConnection cnx, SqlTransaction tran)
        {
            string query = "SELECT CuentaID FROM CatalogoContable WHERE CodigoCuenta = @Codigo AND Activo = 1";
            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@Codigo", codigoCuenta);

            object result = cmd.ExecuteScalar();
            if (result == null)
                throw new Exception($"Cuenta contable {codigoCuenta} no encontrada");

            return Convert.ToInt32(result);
        }
    }
}
