// CapaDatos/CD_Venta.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    // Helper class to avoid tuples (not supported in .NET Framework 4.6)
    internal class IVABreakdown
    {
        public decimal Base { get; set; }
        public decimal IVA { get; set; }
    }

    public class CD_Venta
    {
        private static CD_Venta _instancia;
        public static CD_Venta Instancia => _instancia ??= new CD_Venta();

        public bool RegistrarVentaCredito(VentaCliente venta, string usuario)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction tran = cnx.BeginTransaction();

                DataTable tabla = new DataTable();
                tabla.Columns.Add("ProductoID", typeof(int));
                tabla.Columns.Add("LoteID", typeof(int));
                tabla.Columns.Add("Cantidad", typeof(int));

                foreach (var d in venta.Detalle)
                {
                    tabla.Rows.Add(d.ProductoID, d.LoteID, d.Cantidad);
                }

                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVentaCredito", cnx, tran) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@ClienteID", venta.ClienteID);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    SqlParameter tvp = cmd.Parameters.AddWithValue("@Productos", tabla);
                    tvp.SqlDbType = SqlDbType.Structured;
                    tvp.TypeName = "dbo.TipoTablaVentaProductos";

                    cmd.ExecuteNonQuery();

                    // ===== GENERAR PÓLIZA CON DESGLOSE DE IVA =====
                    var detallesIVA = new Dictionary<string, IVABreakdown>();
                    decimal totalBase = 0, totalIVA = 0, totalCOGS = 0;

                    foreach (var d in venta.Detalle)
                    {
                        // Calcular base sin IVA (asumiendo PrecioVenta ya es el precio neto)
                        decimal baseLinea = d.PrecioVenta * d.Cantidad;
                        decimal ivaLinea = 0m;

                        if (!d.Exento && d.TasaIVAPorcentaje > 0)
                        {
                            ivaLinea = baseLinea * (d.TasaIVAPorcentaje / 100m);
                        }

                        string keyIVA = d.Exento ? "EXENTO" : d.TasaIVAPorcentaje.ToString();
                        if (!detallesIVA.ContainsKey(keyIVA))
                            detallesIVA[keyIVA] = new IVABreakdown { Base = 0, IVA = 0 };

                        detallesIVA[keyIVA].Base += baseLinea;
                        detallesIVA[keyIVA].IVA += ivaLinea;

                        totalBase += baseLinea;
                        totalIVA += ivaLinea;

                        // Calcular COGS
                        decimal precioCompra = d.PrecioCompra;
                        if (precioCompra <= 0 && d.LoteID > 0)
                        {
                            var lote = CD_Producto.Instancia.ObtenerLotePorId(d.LoteID);
                            if (lote != null) precioCompra = lote.PrecioCompra;
                        }
                        totalCOGS += precioCompra * d.Cantidad;
                    }

                    decimal totalVenta = totalBase + totalIVA;

                    // Leer cuentas base desde CatalogoContable
                    var cuentaClienteObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("CLIENTE");
                    var cuentaCajaObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("CAJA");
                    var cuentaVentasObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("VENTAS");
                    var cuentaCostoObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("COSTO_VENTAS");
                    var cuentaInventarioObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("INVENTARIO");

                    if (cuentaClienteObj == null || cuentaCajaObj == null || cuentaVentasObj == null 
                        || cuentaCostoObj == null || cuentaInventarioObj == null)
                    {
                        tran.Rollback();
                        throw new Exception("Faltan cuentas contables en el catálogo. Configurar CatalogoContable.");
                    }

                    int cuentaClientes = cuentaClienteObj.CuentaID;
                    int cuentaCaja = cuentaCajaObj.CuentaID;
                    int cuentaVentas = cuentaVentasObj.CuentaID;
                    int cuentaCosto = cuentaCostoObj.CuentaID;
                    int cuentaInventario = cuentaInventarioObj.CuentaID;
                    
                    // Convertir a Guid
                    Guid? cuentaClientesGuid = ConvertirIntAGuid(cuentaClientes);
                    Guid? cuentaCajaGuid = ConvertirIntAGuid(cuentaCaja);
                    Guid? cuentaVentasGuid = ConvertirIntAGuid(cuentaVentas);
                    Guid? cuentaCostoGuid = ConvertirIntAGuid(cuentaCosto);
                    Guid? cuentaInventarioGuid = ConvertirIntAGuid(cuentaInventario);

                    var poliza = new Poliza
                    {
                        TipoPoliza = "VENTA",
                        FechaPoliza = DateTime.Now,
                        Concepto = $"Venta - Cliente: {venta.ClienteID}",
                        Referencia = $"VENTA-{venta.VentaID}",
                        Usuario = venta.Usuario
                    };

                    // 1. DÉBITO: Cliente/Caja (monto total con IVA)
                    Guid? cuentaDeudora = (venta.Estatus ?? string.Empty).ToUpper() == "CREDITO" ? cuentaClientesGuid : cuentaCajaGuid;
                    poliza.Detalles.Add(new PolizaDetalle 
                    { 
                        CuentaID = cuentaDeudora, 
                        Debe = totalVenta, 
                        Haber = 0, 
                        Concepto = $"Venta a {(venta.Estatus ?? "").ToUpper()}" 
                    });

                    // 2. DÉBITO: Costo de Ventas
                    if (totalCOGS > 0)
                        poliza.Detalles.Add(new PolizaDetalle 
                        { 
                            CuentaID = cuentaCostoGuid, 
                            Debe = totalCOGS, 
                            Haber = 0, 
                            Concepto = "Costo de ventas" 
                        });

                    // 3. CRÉDITO: Ventas (solo la base)
                    poliza.Detalles.Add(new PolizaDetalle 
                    { 
                        CuentaID = cuentaVentasGuid, 
                        Debe = 0, 
                        Haber = totalBase, 
                        Concepto = "Ingresos por ventas (neto)" 
                    });

                    // 4. CRÉDITO: IVA por tasa (desde MapeoContableIVA)
                    foreach (var kvp in detallesIVA)
                    {
                        IVABreakdown breakdown = kvp.Value;
                        if (breakdown.IVA > 0)
                        {
                            // Obtener tasa
                            bool esExento = kvp.Key == "EXENTO";
                            decimal tasa = esExento ? 0 : decimal.Parse(kvp.Key);
                            
                            var mapeo = CD_MapeoIVA.Instancia.ObtenerPorTasa(tasa, esExento);
                            if (mapeo != null)
                            {
                                poliza.Detalles.Add(new PolizaDetalle
                                {
                                    CuentaID = ConvertirIntAGuid(mapeo.CuentaAcreedora),
                                    Debe = 0,
                                    Haber = breakdown.IVA,
                                    Concepto = $"IVA cobrado ({mapeo.Descripcion})"
                                });
                            }
                        }
                    }

                    // 5. CRÉDITO: Inventario (reducción por COGS)
                    if (totalCOGS > 0)
                        poliza.Detalles.Add(new PolizaDetalle 
                        { 
                            CuentaID = cuentaInventarioGuid, 
                            Debe = 0, 
                            Haber = totalCOGS, 
                            Concepto = "Reducción inventario" 
                        });

                    bool polOk = CD_Poliza.Instancia.CrearPoliza(poliza, cnx, tran);
                    if (!polOk)
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public List<VentaCliente> ObtenerVentasCliente(Guid clienteId)
        {
            var lista = new List<VentaCliente>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("sp_ConsultarVentasCliente", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new VentaCliente
                        {
                            VentaID = Guid.Parse(dr["VentaID"].ToString()),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Estatus = dr["Estatus"].ToString(),
                            FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["FechaVencimiento"]),
                            TotalPagado = Convert.ToDecimal(dr["TotalPagado"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"])
                        });
                    }
                }
            }
            return lista;
        }

        public List<dynamic> ObtenerTodasVentas(string fechaInicio = null, string fechaFin = null, string codigoVenta = null, string documentoCliente = null, string nombreCliente = null)
        {
            var lista = new List<dynamic>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        V.VentaID,
                        'VENTA' AS TipoDocumento,
                        V.VentaID AS CodigoDocumento,
                        V.FechaVenta AS FechaCreacion,
                        C.RFC AS DocumentoCliente,
                        C.RazonSocial AS NombreCliente,
                        V.Total AS TotalVenta,
                        V.Estatus,
                        ISNULL(SUM(P.Monto), 0) AS TotalPagado,
                        V.Total - ISNULL(SUM(P.Monto), 0) AS SaldoPendiente,
                        CASE WHEN V.EstaFacturada = 1 THEN 'Facturada' ELSE 'Sin Facturar' END AS EstadoFactura
                    FROM VentasClientes V
                    INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
                    LEFT JOIN PagosClientes P ON V.VentaID = P.VentaID
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(fechaInicio))
                    query += " AND CAST(V.FechaVenta AS DATE) >= @FechaInicio";
                
                if (!string.IsNullOrEmpty(fechaFin))
                    query += " AND CAST(V.FechaVenta AS DATE) <= @FechaFin";
                
                if (!string.IsNullOrEmpty(codigoVenta))
                    query += " AND CAST(V.VentaID AS VARCHAR(50)) LIKE '%' + @CodigoVenta + '%'";
                
                if (!string.IsNullOrEmpty(documentoCliente))
                    query += " AND C.RFC LIKE '%' + @DocumentoCliente + '%'";
                
                if (!string.IsNullOrEmpty(nombreCliente))
                    query += " AND C.RazonSocial LIKE '%' + @NombreCliente + '%'";

                query += @"
                    GROUP BY V.VentaID, V.FechaVenta, C.RFC, C.RazonSocial, V.Total, V.Estatus, V.EstaFacturada
                    ORDER BY V.FechaVenta DESC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                
                if (!string.IsNullOrEmpty(fechaInicio))
                    cmd.Parameters.AddWithValue("@FechaInicio", DateTime.Parse(fechaInicio));
                
                if (!string.IsNullOrEmpty(fechaFin))
                    cmd.Parameters.AddWithValue("@FechaFin", DateTime.Parse(fechaFin));
                
                if (!string.IsNullOrEmpty(codigoVenta))
                    cmd.Parameters.AddWithValue("@CodigoVenta", codigoVenta);
                
                if (!string.IsNullOrEmpty(documentoCliente))
                    cmd.Parameters.AddWithValue("@DocumentoCliente", documentoCliente);
                
                if (!string.IsNullOrEmpty(nombreCliente))
                    cmd.Parameters.AddWithValue("@NombreCliente", nombreCliente);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Normalizar el estatus a texto descriptivo
                        string estatusRaw = dr["Estatus"].ToString();
                        string estatusNormalizado = estatusRaw;
                        
                        switch (estatusRaw.ToUpper())
                        {
                            case "1":
                            case "PENDIENTE":
                                estatusNormalizado = "Pendiente";
                                break;
                            case "2":
                            case "PAGADA":
                            case "PAGADO":
                                estatusNormalizado = "Pagada";
                                break;
                            case "CREDITO":
                            case "CRÉDITO":
                                estatusNormalizado = "Crédito";
                                break;
                            case "CONTADO":
                                estatusNormalizado = "Contado";
                                break;
                            case "ACTIVA":
                                estatusNormalizado = "Activa";
                                break;
                            case "CANCELADA":
                                estatusNormalizado = "Cancelada";
                                break;
                        }
                        
                        lista.Add(new
                        {
                            VentaID = dr["VentaID"].ToString(),
                            TipoDocumento = dr["TipoDocumento"].ToString(),
                            CodigoDocumento = dr["CodigoDocumento"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]).ToString("dd/MM/yyyy HH:mm"),
                            DocumentoCliente = dr["DocumentoCliente"].ToString(),
                            NombreCliente = dr["NombreCliente"].ToString(),
                            TotalVenta = Convert.ToDecimal(dr["TotalVenta"]),
                            Estatus = estatusNormalizado,
                            TotalPagado = Convert.ToDecimal(dr["TotalPagado"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                            EstadoFactura = dr["EstadoFactura"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public VentaCliente ObtenerDetalle(Guid ventaId)
        {
            VentaCliente venta = null;
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                // Obtener encabezado de venta
                SqlCommand cmdVenta = new SqlCommand("sp_ConsultarVentasCliente", cnx) { CommandType = CommandType.StoredProcedure };
                cmdVenta.Parameters.AddWithValue("@ClienteID", Guid.Empty); // Dummy para estructura
                
                // Mejor usar una consulta directa para obtener una venta específica
                cmdVenta = new SqlCommand(@"
                    SELECT V.VentaID, V.ClienteID, C.RazonSocial, V.FechaVenta, V.Total, V.Estatus, V.FechaVencimiento,
                           ISNULL(SUM(P.Monto), 0) AS TotalPagado,
                           V.Total - ISNULL(SUM(P.Monto), 0) AS SaldoPendiente
                    FROM VentasClientes V
                    INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
                    LEFT JOIN PagosClientes P ON V.VentaID = P.VentaID
                    WHERE V.VentaID = @VentaID
                    GROUP BY V.VentaID, V.ClienteID, C.RazonSocial, V.FechaVenta, V.Total, V.Estatus, V.FechaVencimiento
                ", cnx);
                cmdVenta.Parameters.AddWithValue("@VentaID", ventaId);

                cnx.Open();
                using (SqlDataReader dr = cmdVenta.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        venta = new VentaCliente
                        {
                            VentaID = Guid.Parse(dr["VentaID"].ToString()),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Estatus = dr["Estatus"].ToString(),
                            FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["FechaVencimiento"]),
                            TotalPagado = Convert.ToDecimal(dr["TotalPagado"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"])
                        };
                    }
                }

                // Obtener detalle de productos
                if (venta != null)
                {
                    SqlCommand cmdDetalle = new SqlCommand("sp_ConsultarDetalleVenta", cnx) { CommandType = CommandType.StoredProcedure };
                    cmdDetalle.Parameters.AddWithValue("@VentaID", ventaId);
                    
                    using (SqlDataReader dr = cmdDetalle.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            venta.Detalle.Add(new VentaDetalleCliente
                            {
                                ProductoID = Convert.ToInt32(dr["ProductoID"]),
                                Producto = dr["Producto"].ToString(),
                                LoteID = Convert.ToInt32(dr["LoteID"]),
                                Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                                PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"]),
                                TasaIVAPorcentaje = Convert.ToDecimal(dr["TasaIVA"]),
                                Exento = Convert.ToBoolean(dr["Exento"])
                            });
                        }
                    }
                }
            }
            return venta;
        }

        // Actualizar estado de facturación de una venta
        public bool ActualizarEstadoFactura(Guid ventaId, Guid facturaId)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    UPDATE VentasClientes 
                    SET EstaFacturada = 1, FacturaID = @FacturaID
                    WHERE VentaID = @VentaID";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@VentaID", ventaId);
                cmd.Parameters.AddWithValue("@FacturaID", facturaId);

                cnx.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool RegistrarPago(PagoCliente pago, out string mensajeDetallado)
        {
            mensajeDetallado = string.Empty;
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    // Logic from sp_RegistrarPagoCliente embedded directly to ensure SET options apply
                    string commandText = @"
                        SET ARITHABORT ON;
                        SET CONCAT_NULL_YIELDS_NULL ON;
                        SET QUOTED_IDENTIFIER ON;
                        SET ANSI_NULLS ON;
                        SET ANSI_PADDING ON;
                        SET ANSI_WARNINGS ON;
                        SET NUMERIC_ROUNDABORT OFF;
                        SET NOCOUNT ON;
                        SET XACT_ABORT ON;

                        BEGIN TRY
                            BEGIN TRANSACTION;
                            
                            DECLARE @SaldoPendiente DECIMAL(18,2);
                            DECLARE @NuevoSaldo DECIMAL(18,2);
                            DECLARE @TotalPagado DECIMAL(18,2);
                            
                            -- Obtener saldo pendiente actual
                            SELECT 
                                @SaldoPendiente = ISNULL(SaldoPendiente, Total), 
                                @TotalPagado = ISNULL(TotalPagado, 0)
                            FROM VentasClientes
                            WHERE VentaID = @VentaID;
                            
                            IF @SaldoPendiente IS NULL
                            BEGIN
                                ;THROW 51000, 'No se encontro la venta especificada', 1;
                            END
                            
                            -- Validar que el monto no exceda el saldo pendiente
                            IF @Monto > @SaldoPendiente
                            BEGIN
                                ;THROW 51000, 'El monto del pago excede el saldo pendiente', 1;
                            END
                            
                            -- Calcular nuevo saldo
                            SET @NuevoSaldo = @SaldoPendiente - @Monto;
                            SET @TotalPagado = @TotalPagado + @Monto;
                            
                            -- Registrar el pago
                            INSERT INTO PagosClientes (
                                PagoID, VentaID, ClienteID, Monto, FechaPago, FormaPago, 
                                Referencia, Comentario, GenerarFactura, GenerarComplemento, 
                                Usuario, FechaRegistro
                            )
                            VALUES (
                                @PagoID, @VentaID, @ClienteID, @Monto, @FechaPago, @FormaPago,
                                @Referencia, @Comentario, @GenerarFactura, @GenerarComplemento,
                                @Usuario, GETDATE()
                            );
                            
                            -- Actualizar el saldo de la venta
                            UPDATE VentasClientes
                            SET SaldoPendiente = @NuevoSaldo,
                                TotalPagado = @TotalPagado,
                                Estatus = CASE WHEN @NuevoSaldo = 0 THEN '2' ELSE '1' END, -- 2 = Pagado, 1 = Pendiente
                                RequiereFactura = CASE 
                                    WHEN @GenerarFactura = 1 AND @NuevoSaldo = 0 THEN 1  -- Solo permitir facturar si se pagó todo
                                    ELSE RequiereFactura 
                                END
                            WHERE VentaID = @VentaID;
                            
                            COMMIT TRANSACTION;
                            
                            -- Mensaje de retorno
                            SET @Mensaje = 'Pago registrado correctamente';
                            
                            IF @GenerarFactura = 1 AND @NuevoSaldo = 0
                                SET @Mensaje = @Mensaje + '. Venta lista para facturar';
                            ELSE IF @GenerarFactura = 1 AND @NuevoSaldo > 0
                                SET @Mensaje = @Mensaje + '. La factura se generará cuando se liquide el saldo';
                                
                            IF @GenerarComplemento = 1
                                SET @Mensaje = @Mensaje + '. Complemento de pago registrado';
                        END TRY
                        BEGIN CATCH
                            IF @@TRANCOUNT > 0
                                ROLLBACK TRANSACTION;
                            
                            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                            ;THROW 51000, @ErrorMessage, 1;
                        END CATCH
                    ";

                    SqlCommand cmd = new SqlCommand(commandText, cnx);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@PagoID", pago.PagoID == Guid.Empty ? Guid.NewGuid() : pago.PagoID);
                    cmd.Parameters.AddWithValue("@VentaID", pago.VentaID);
                    cmd.Parameters.AddWithValue("@ClienteID", pago.ClienteID);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    cmd.Parameters.AddWithValue("@FormaPago", pago.FormaPago);
                    cmd.Parameters.AddWithValue("@Referencia", (object)pago.Referencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comentario", (object)pago.Comentario ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", pago.Usuario);
                    cmd.Parameters.AddWithValue("@GenerarFactura", pago.GenerarFactura);
                    cmd.Parameters.AddWithValue("@GenerarComplemento", pago.GenerarComplemento);
                    cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensajeDetallado = cmd.Parameters["@Mensaje"].Value.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                mensajeDetallado = ex.Message;
                return false;
            }
        }

        public List<PagoCliente> ObtenerPagosVenta(Guid ventaId)
        {
            var lista = new List<PagoCliente>();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT PagoID, VentaID, ClienteID, Monto, FechaPago, FormaPago, 
                           Referencia, Comentario, GenerarFactura, GenerarComplemento, Usuario
                    FROM PagosClientes 
                    WHERE VentaID = @VentaID
                    ORDER BY FechaPago DESC";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@VentaID", ventaId);
                
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new PagoCliente
                        {
                            PagoID = Guid.Parse(dr["PagoID"].ToString()),
                            VentaID = Guid.Parse(dr["VentaID"].ToString()),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                            FormaPago = dr["FormaPago"].ToString(),
                            Referencia = dr["Referencia"] == DBNull.Value ? null : dr["Referencia"].ToString(),
                            Comentario = dr["Comentario"] == DBNull.Value ? null : dr["Comentario"].ToString(),
                            GenerarFactura = dr["GenerarFactura"] != DBNull.Value && Convert.ToBoolean(dr["GenerarFactura"]),
                            GenerarComplemento = dr["GenerarComplemento"] != DBNull.Value && Convert.ToBoolean(dr["GenerarComplemento"]),
                            Usuario = dr["Usuario"].ToString()
                        });
                    }
                }
            }
            
            return lista;
        }

        public decimal ObtenerSaldoPendiente(Guid clienteId)
        {
            decimal saldo = 0;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
            SELECT ISNULL(SUM(Total - ISNULL((SELECT SUM(Monto) 
                                             FROM PagosClientes 
                                             WHERE VentaID = v.VentaID), 0)), 0) AS SaldoPendiente
            FROM Ventas v
            WHERE v.ClienteID = @ClienteID 
              AND v.Estatus = 'CREDITO'";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                cnx.Open();
                object resultado = cmd.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                    saldo = Convert.ToDecimal(resultado);
            }

            return saldo;
        }
        
        /// <summary>
        /// Convierte un CuentaID int a Guid de forma determinística
        /// </summary>
        private Guid? ConvertirIntAGuid(int id)
        {
            if (id <= 0) return null;
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}