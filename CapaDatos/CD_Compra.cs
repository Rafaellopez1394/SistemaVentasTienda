// CapaDatos/CD_Compra.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Compra
    {
        private static CD_Compra _instancia;
        public static CD_Compra Instancia => _instancia ??= new CD_Compra();

        public bool RegistrarCompraConLotes(Compra compra, string usuario)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction tran = cnx.BeginTransaction();

                try
                {
                    DataTable tabla = new DataTable();
                    tabla.Columns.Add("ProductoID", typeof(int));
                    tabla.Columns.Add("Cantidad", typeof(int));
                    tabla.Columns.Add("PrecioCompra", typeof(decimal));
                    tabla.Columns.Add("PrecioVenta", typeof(decimal));

                    foreach (var d in compra.Detalle)
                    {
                        tabla.Rows.Add(d.ProductoID, d.Cantidad, d.PrecioCompra, d.PrecioVenta);
                    }

                    SqlCommand cmd = new SqlCommand("sp_RegistrarCompraConLote", cnx, tran) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@ProveedorID", compra.ProveedorID);
                    cmd.Parameters.AddWithValue("@FolioFactura", compra.FolioFactura);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    SqlParameter tvp = cmd.Parameters.AddWithValue("@ProductosComprados", tabla);
                    tvp.SqlDbType = SqlDbType.Structured;
                    tvp.TypeName = "dbo.TipoTablaProductosComprados";

                    // Obtener el CompraID generado
                    SqlParameter outputCompraId = new SqlParameter("@CompraID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputCompraId);

                    cmd.ExecuteNonQuery();
                    
                    int compraId = Convert.ToInt32(outputCompraId.Value);

                    // ===== ACTUALIZAR INVENTARIO POR LOTES =====
                    // Por cada detalle de la compra, crear un nuevo lote con la cantidad ingresada
                    foreach (var d in compra.Detalle)
                    {
                        var cmdLote = new SqlCommand(@"
                            INSERT INTO LotesProducto (
                                ProductoID, FechaEntrada, CantidadTotal, CantidadDisponible,
                                PrecioCompra, PrecioVenta, Usuario, UltimaAct, Estatus
                            ) VALUES (
                                @ProductoID, GETDATE(), @Cantidad, @Cantidad,
                                @PrecioCompra, @PrecioVenta, @Usuario, GETDATE(), 1
                            )", cnx, tran);

                        cmdLote.Parameters.AddWithValue("@ProductoID", d.ProductoID);
                        cmdLote.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                        cmdLote.Parameters.AddWithValue("@PrecioCompra", d.PrecioCompra);
                        cmdLote.Parameters.AddWithValue("@PrecioVenta", d.PrecioVenta);
                        cmdLote.Parameters.AddWithValue("@Usuario", usuario ?? (object)DBNull.Value);
                        cmdLote.ExecuteNonQuery();
                    }

                    // ===== GENERAR PÓLIZA CON DESGLOSE DE IVA =====
                    var detallesIVA = new Dictionary<string, IVABreakdown>();
                    decimal totalBase = 0, totalIVA = 0, totalInventario = 0;

                    foreach (var d in compra.Detalle)
                    {
                        // Calcular base sin IVA (asumiendo PrecioCompra ya es el precio neto)
                        decimal baseLinea = d.PrecioCompra * d.Cantidad;
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
                        totalInventario += baseLinea; // Inventario se valúa sin IVA en algunos sistemas
                    }

                    decimal totalCompra = totalBase + totalIVA;

                    var cuentaInventarioObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("INVENTARIO");
                    var cuentaProveedoresObj = CD_CatalogoContable.Instancia.ObtenerPorSubTipo("PROVEEDOR");

                    if (cuentaInventarioObj == null || cuentaProveedoresObj == null)
                    {
                        tran.Rollback();
                        throw new Exception("Faltan cuentas contables en el catálogo. Configurar CatalogoContable.");
                    }

                    int cuentaInventario = cuentaInventarioObj.CuentaID;
                    int cuentaProveedores = cuentaProveedoresObj.CuentaID;
                    
                    // Convertir int a Guid
                    Guid? cuentaInventarioGuid = ConvertirIntAGuid(cuentaInventario);
                    Guid? cuentaProveedoresGuid = ConvertirIntAGuid(cuentaProveedores);

                    var poliza = new Poliza
                    {
                        TipoPoliza = "COMPRA",
                        FechaPoliza = DateTime.Now,
                        Concepto = $"Compra - Proveedor: {compra.ProveedorID} - {compra.FolioFactura}",
                        Referencia = $"COMPRA-{compra.CompraID}",
                        Usuario = compra.Usuario
                    };

                    // 1. DÉBITO: Inventario (solo base, sin IVA)
                    poliza.Detalles.Add(new PolizaDetalle 
                    { 
                        CuentaID = cuentaInventarioGuid, 
                        Debe = totalInventario, 
                        Haber = 0, 
                        Concepto = "Entrada de inventario (neto)" 
                    });

                    // 2. DÉBITO: IVA pagado por tasa (desde MapeoContableIVA)
                    foreach (var kvp in detallesIVA)
                    {
                        IVABreakdown breakdown = kvp.Value;
                        if (breakdown.IVA > 0)
                        {
                            bool esExento = kvp.Key == "EXENTO";
                            decimal tasa = esExento ? 0 : decimal.Parse(kvp.Key);

                            var mapeo = CD_MapeoIVA.Instancia.ObtenerPorTasa(tasa, esExento);
                            if (mapeo != null)
                            {
                                poliza.Detalles.Add(new PolizaDetalle
                                {
                                    CuentaID = ConvertirIntAGuid(mapeo.CuentaDeudora), // En compras, IVA es deudor (acreedor fiscal)
                                    Debe = breakdown.IVA,
                                    Haber = 0,
                                    Concepto = $"IVA pagado ({mapeo.Descripcion})"
                                });
                            }
                        }
                    }

                    // 3. CRÉDITO: Proveedores (monto total con IVA)
                    poliza.Detalles.Add(new PolizaDetalle 
                    { 
                        CuentaID = cuentaProveedoresGuid, 
                        Debe = 0, 
                        Haber = totalCompra, 
                        Concepto = "Cuenta por pagar a proveedor" 
                    });

                    bool polOk = CD_Poliza.Instancia.CrearPoliza(poliza, cnx, tran);
                    if (!polOk)
                    {
                        tran.Rollback();
                        return false;
                    }

                    // ===== REGISTRAR CUENTA POR PAGAR SI ES A CRÉDITO =====
                    // Obtener días de crédito del proveedor
                    var queryProveedor = "SELECT DiasCredito FROM Proveedores WHERE ProveedorID = @ProveedorID";
                    SqlCommand cmdProv = new SqlCommand(queryProveedor, cnx, tran);
                    cmdProv.Parameters.AddWithValue("@ProveedorID", compra.ProveedorID);
                    object diasCreditoObj = cmdProv.ExecuteScalar();
                    
                    if (diasCreditoObj != null && diasCreditoObj != DBNull.Value)
                    {
                        int diasCredito = Convert.ToInt32(diasCreditoObj);
                        
                        // Si tiene días de crédito, registrar cuenta por pagar
                        if (diasCredito > 0)
                        {
                            var cuentaId = Guid.NewGuid();
                            DateTime fechaVencimiento = DateTime.Now.AddDays(diasCredito);

                            var queryCuenta = @"
                                INSERT INTO CuentasPorPagar 
                                    (CuentaPorPagarID, CompraID, ProveedorID, FechaRegistro, FechaVencimiento, 
                                     MontoTotal, SaldoPendiente, Estado, DiasCredito, FolioFactura, Activo)
                                VALUES 
                                    (@CuentaPorPagarID, @CompraID, @ProveedorID, GETDATE(), @FechaVencimiento,
                                     @MontoTotal, @SaldoPendiente, 'PENDIENTE', @DiasCredito, @FolioFactura, 1)";

                            SqlCommand cmdCuenta = new SqlCommand(queryCuenta, cnx, tran);
                            cmdCuenta.Parameters.AddWithValue("@CuentaPorPagarID", cuentaId);
                            cmdCuenta.Parameters.AddWithValue("@CompraID", compraId);
                            cmdCuenta.Parameters.AddWithValue("@ProveedorID", compra.ProveedorID);
                            cmdCuenta.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                            cmdCuenta.Parameters.AddWithValue("@MontoTotal", totalCompra);
                            cmdCuenta.Parameters.AddWithValue("@SaldoPendiente", totalCompra);
                            cmdCuenta.Parameters.AddWithValue("@DiasCredito", diasCredito);
                            cmdCuenta.Parameters.AddWithValue("@FolioFactura", (object)compra.FolioFactura ?? DBNull.Value);
                            cmdCuenta.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
        // ===============================================
        // MÉTODO QUE TE FALTABA → LISTAR TODAS LAS COMPRAS
        // ===============================================
        public List<Compra> ObtenerTodas()
        {
            var lista = new List<Compra>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
            SELECT 
                c.CompraID,
                c.ProveedorID,
                p.RazonSocial AS RazonSocialProveedor,
                c.FolioFactura,
                c.FechaCompra,
                c.Total,
                c.Usuario
            FROM Compras c
            INNER JOIN Proveedores p ON c.ProveedorID = p.ProveedorID
            ORDER BY c.FechaCompra DESC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Compra
                        {
                            CompraID = Convert.ToInt32(dr["CompraID"]),
                            ProveedorID = Guid.Parse(dr["ProveedorID"].ToString()),
                            RazonSocialProveedor = dr["RazonSocialProveedor"].ToString(),
                            FolioFactura = dr["FolioFactura"].ToString(),
                            FechaCompra = Convert.ToDateTime(dr["FechaCompra"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Usuario = dr["Usuario"].ToString()
                        });
                    }
                }
            }

            return lista;
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