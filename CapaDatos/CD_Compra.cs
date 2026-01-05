// CapaDatos/CD_Compra.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CapaDatos.Utilidades;

namespace CapaDatos
{
    // Alias para compatibilidad con nombres en código
    using DetalleCompra = CapaModelo.CompraDetalle;

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
                                ProductoID, SucursalID, FechaEntrada, CantidadTotal, CantidadDisponible,
                                PrecioCompra, PrecioVenta, Usuario, UltimaAct, Estatus
                            ) VALUES (
                                @ProductoID, @SucursalID, GETDATE(), @Cantidad, @Cantidad,
                                @PrecioCompra, @PrecioVenta, @Usuario, GETDATE(), 1
                            )", cnx, tran);

                        cmdLote.Parameters.AddWithValue("@ProductoID", d.ProductoID);
                        cmdLote.Parameters.AddWithValue("@SucursalID", compra.SucursalID);
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

        /// <summary>
        /// Registra una compra desde un archivo XML CFDI con desglose automático de lotes
        /// </summary>
        public bool RegistrarCompraDesdeXML(string rutaXML, Dictionary<int, ProductoCompraXML> mapeoProductos, int sucursalID, string usuario, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                // 1. Parsear el XML
                var datosFactura = CFDICompraParser.ParsearXML(rutaXML);

                // 2. Buscar o crear proveedor basado en RFC
                Guid proveedorID = BuscarOCrearProveedor(datosFactura);

                // 3. Crear objeto Compra
                var compra = new Compra
                {
                    ProveedorID = proveedorID,
                    FolioFactura = $"{datosFactura.Serie}-{datosFactura.Folio}",
                    FechaCompra = datosFactura.Fecha,
                    Total = datosFactura.Total,
                    Usuario = usuario,
                    SucursalID = sucursalID, // Asignar sucursal
                    Detalle = new List<DetalleCompra>(),
                    // Datos adicionales del XML
                    UUID = datosFactura.UUID,
                    XMLOriginal = System.IO.File.ReadAllText(rutaXML)
                };

                // 4. Mapear conceptos del XML a productos con desglose
                foreach (var concepto in datosFactura.Conceptos)
                {
                    // Buscar mapeo de producto
                    var mapeo = mapeoProductos.Values.FirstOrDefault(m => 
                        m.NoIdentificacionXML == concepto.NoIdentificacion || 
                        m.DescripcionXML == concepto.Descripcion);

                    if (mapeo == null)
                    {
                        mensaje = $"No se encontró mapeo para el concepto: {concepto.Descripcion}";
                        return false;
                    }

                    // Calcular cantidad desglosada
                    decimal cantidadFinal = concepto.Cantidad * mapeo.FactorConversion;
                    decimal precioUnitarioFinal = concepto.ValorUnitario / mapeo.FactorConversion;

                    // Calcular IVA del concepto
                    decimal tasaIVA = 0;
                    bool exento = true;

                    if (concepto.ImpuestosTrasladados != null && concepto.ImpuestosTrasladados.Count > 0)
                    {
                        var impuestoIVA = concepto.ImpuestosTrasladados.FirstOrDefault(i => i.Impuesto == "002"); // 002 = IVA
                        if (impuestoIVA != null)
                        {
                            tasaIVA = impuestoIVA.TasaOCuota * 100; // Convertir a porcentaje
                            exento = false;
                        }
                    }

                    compra.Detalle.Add(new DetalleCompra
                    {
                        ProductoID = mapeo.ProductoID,
                        Cantidad = (int)Math.Ceiling(cantidadFinal), // Redondear hacia arriba
                        PrecioCompra = precioUnitarioFinal,
                        PrecioVenta = mapeo.PrecioVentaSugerido > 0 ? mapeo.PrecioVentaSugerido : precioUnitarioFinal * 1.3m,
                        TasaIVAPorcentaje = tasaIVA,
                        Exento = exento
                    });
                }

                // 5. Registrar compra con lotes
                bool resultado = RegistrarCompraConLotes(compra, usuario);

                if (resultado)
                {
                    mensaje = $"Compra registrada exitosamente desde XML. UUID: {datosFactura.UUID}";
                    
                    // Guardar XML en carpeta de respaldo
                    GuardarXMLRespaldo(rutaXML, datosFactura.UUID);
                }
                else
                {
                    mensaje = "Error al registrar la compra";
                }

                return resultado;
            }
            catch (Exception ex)
            {
                mensaje = "Error al procesar XML: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Busca un proveedor por RFC o lo crea si no existe
        /// </summary>
        private Guid BuscarOCrearProveedor(CFDICompraParser.DatosFacturaCompra datosFactura)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();

                // Buscar proveedor por RFC
                string queryBuscar = "SELECT ProveedorID FROM Proveedores WHERE RFCProveedor = @RFC";
                SqlCommand cmdBuscar = new SqlCommand(queryBuscar, cnx);
                cmdBuscar.Parameters.AddWithValue("@RFC", datosFactura.EmisorRFC);

                object result = cmdBuscar.ExecuteScalar();

                if (result != null)
                {
                    return (Guid)result;
                }

                // No existe, crear nuevo proveedor
                Guid nuevoID = Guid.NewGuid();
                string queryCrear = @"
                    INSERT INTO Proveedores (
                        ProveedorID, RazonSocial, RFCProveedor, ContactoNombre, 
                        Activo, FechaRegistro
                    ) VALUES (
                        @ProveedorID, @RazonSocial, @RFC, @ContactoNombre,
                        1, GETDATE()
                    )";

                SqlCommand cmdCrear = new SqlCommand(queryCrear, cnx);
                cmdCrear.Parameters.AddWithValue("@ProveedorID", nuevoID);
                cmdCrear.Parameters.AddWithValue("@RazonSocial", datosFactura.EmisorNombre ?? "Proveedor " + datosFactura.EmisorRFC);
                cmdCrear.Parameters.AddWithValue("@RFC", datosFactura.EmisorRFC);
                cmdCrear.Parameters.AddWithValue("@ContactoNombre", "Sin contacto");

                cmdCrear.ExecuteNonQuery();

                return nuevoID;
            }
        }

        /// <summary>
        /// Guarda una copia del XML en carpeta de respaldo
        /// </summary>
        private void GuardarXMLRespaldo(string rutaOriginal, string uuid)
        {
            try
            {
                string carpetaRespaldo = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "XMLCompras"
                );

                if (!System.IO.Directory.Exists(carpetaRespaldo))
                {
                    System.IO.Directory.CreateDirectory(carpetaRespaldo);
                }

                string nombreArchivo = $"{uuid}_{DateTime.Now:yyyyMMddHHmmss}.xml";
                string rutaDestino = System.IO.Path.Combine(carpetaRespaldo, nombreArchivo);

                System.IO.File.Copy(rutaOriginal, rutaDestino, true);
            }
            catch
            {
                // No fallar si no se puede guardar respaldo
            }
        }
    }

    /// <summary>
    /// Clase auxiliar para mapear productos del XML a productos del sistema
    /// </summary>
    public class ProductoCompraXML
    {
        public int ProductoID { get; set; }
        public string NoIdentificacionXML { get; set; }  // Código/SKU en el XML
        public string DescripcionXML { get; set; }        // Descripción en el XML
        public decimal FactorConversion { get; set; }     // Ej: 1 caja = 8 piezas
        public decimal PrecioVentaSugerido { get; set; }  // Precio de venta calculado
    }
}