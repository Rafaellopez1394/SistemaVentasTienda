// CapaDatos/CD_Producto.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Producto
    {
        private static CD_Producto _instancia;
        public static CD_Producto Instancia => _instancia ??= new CD_Producto();
        private CD_Producto() { }

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT p.ProductoID, p.Nombre, p.CategoriaID, c.Nombre AS NombreCategoria,
                           p.ClaveProdServSAT, sat.Descripcion AS DescripcionSAT,
                           p.ClaveUnidadSAT, u.Descripcion AS UnidadSAT,
                           p.CodigoInterno,
                           p.TasaIVAID, iva.Descripcion AS TasaIVADescripcion, iva.Porcentaje AS TasaIVAPorcentaje,
                           p.TasaIEPSID, ieps.Descripcion AS TasaIEPSDescripcion, ieps.Porcentaje AS TasaIEPSPorcentaje,
                           p.Exento, p.Estatus
                    FROM Productos p
                    INNER JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                    INNER JOIN CatClaveProdServSAT sat ON p.ClaveProdServSAT = sat.ClaveProdServSAT
                    INNER JOIN CatUnidadSAT u ON p.ClaveUnidadSAT = u.ClaveUnidadSAT
                    INNER JOIN CatTasaIVA iva ON p.TasaIVAID = iva.TasaIVAID
                    LEFT JOIN CatTasaIEPS ieps ON p.TasaIEPSID = ieps.TasaIEPSID";

                var cmd = new SqlCommand(query, cnx);
                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            ProductoID = (int)dr["ProductoID"],
                            Nombre = dr["Nombre"].ToString()!,
                            CategoriaID = (int)dr["CategoriaID"],
                            NombreCategoria = dr["NombreCategoria"].ToString(),
                            ClaveProdServSATID = dr["ClaveProdServSAT"].ToString()!,
                            DescripcionSAT = dr["DescripcionSAT"].ToString(),
                            ClaveUnidadSAT = dr["ClaveUnidadSAT"].ToString()!,
                            UnidadSAT = dr["UnidadSAT"].ToString(),
                            CodigoInterno = dr["CodigoInterno"] as string,
                            TasaIVAID = (int)dr["TasaIVAID"],
                            TasaIVADescripcion = dr["TasaIVADescripcion"].ToString(),
                            TasaIVAPorcentaje = dr["TasaIVAPorcentaje"] == DBNull.Value ? 0 : (decimal)dr["TasaIVAPorcentaje"],
                            TasaIEPSID = dr["TasaIEPSID"] as int?,
                            TasaIEPSDescripcion = dr["TasaIEPSDescripcion"]?.ToString(),
                            TasaIEPSPorcentaje = dr["TasaIEPSPorcentaje"] == DBNull.Value ? 0 : (decimal)dr["TasaIEPSPorcentaje"],
                            Exento = (bool)dr["Exento"],
                            Estatus = (bool)dr["Estatus"]
                        });
                    }
                }
            }
            return lista;
        }

        public Producto ObtenerPorId(int id)
        {
            return ObtenerTodos().Find(p => p.ProductoID == id) ?? new Producto();
        }

        public bool AltaProducto(Producto p)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("AltaProducto", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@CategoriaID", p.CategoriaID);
                cmd.Parameters.AddWithValue("@ClaveProdServSAT", p.ClaveProdServSATID);
                cmd.Parameters.AddWithValue("@ClaveUnidadSAT", p.ClaveUnidadSAT);
                cmd.Parameters.AddWithValue("@CodigoInterno", (object)p.CodigoInterno ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TasaIVAID", p.TasaIVAID);
                cmd.Parameters.AddWithValue("@TasaIEPSID", p.TasaIEPSID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Exento", p.Exento);
                cmd.Parameters.AddWithValue("@Usuario", p.Usuario ?? "system");
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        public bool ActualizarProducto(Producto p)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("ActualizarProducto", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProductoID", p.ProductoID);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@CategoriaID", p.CategoriaID);
                cmd.Parameters.AddWithValue("@ClaveProdServSAT", p.ClaveProdServSATID);
                cmd.Parameters.AddWithValue("@ClaveUnidadSAT", p.ClaveUnidadSAT);
                cmd.Parameters.AddWithValue("@CodigoInterno", (object)p.CodigoInterno ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TasaIVAID", p.TasaIVAID);
                cmd.Parameters.AddWithValue("@TasaIEPSID", p.TasaIEPSID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Exento", p.Exento);
                cmd.Parameters.AddWithValue("@Usuario", p.Usuario ?? "system");
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        public bool CambiarEstatusProducto(int id, bool estatus, string usuario = "system")
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("CambiarEstatusProducto", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProductoID", id);
                cmd.Parameters.AddWithValue("@Estatus", estatus);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        public List<LoteProducto> ObtenerLotes(int productoId)
        {
            var lista = new List<LoteProducto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
                    SELECT LoteID, ProductoID, FechaEntrada, FechaCaducidad,
                           CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta
                    FROM LotesProducto WHERE ProductoID = @id ORDER BY FechaEntrada DESC", cnx);
                cmd.Parameters.AddWithValue("@id", productoId);
                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            FechaEntrada = (DateTime)dr["FechaEntrada"],
                            FechaCaducidad = dr["FechaCaducidad"] as DateTime?,
                            CantidadTotal = (int)dr["CantidadTotal"],
                            CantidadDisponible = (int)dr["CantidadDisponible"],
                            PrecioCompra = (decimal)dr["PrecioCompra"],
                            PrecioVenta = (decimal)dr["PrecioVenta"]
                        });
                    }
                }
            }
            return lista;
        }

        public bool RegistrarLote(LoteProducto lote)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("sp_AltaLote", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProductoID", lote.ProductoID);
                cmd.Parameters.AddWithValue("@CantidadTotal", lote.CantidadTotal);
                cmd.Parameters.AddWithValue("@PrecioCompra", lote.PrecioCompra);
                cmd.Parameters.AddWithValue("@PrecioVenta", lote.PrecioVenta);
                cmd.Parameters.AddWithValue("@Usuario", lote.Usuario ?? "system");
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }
        // =============================================
        // MÉTODOS QUE TE FALTABAN - AGREGAR A CD_Producto.cs
        // =============================================

        // Buscar productos para autocompletar o búsqueda rápida
        public List<Producto> BuscarProductos(string termino = "")
        {
            var lista = new List<Producto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
            SELECT TOP(200) 
                p.ProductoID, 
                p.Nombre, 
                p.CodigoInterno AS Codigo, 
                ISNULL(c.Nombre, 'Sin categoría') AS Categoria,
                p.ClaveProdServSAT, 
                p.ClaveUnidadSAT
            FROM Productos p
            LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
            WHERE p.Estatus = 1
              AND (p.Nombre LIKE @termino 
                   OR p.CodigoInterno LIKE @termino 
                   OR p.ClaveProdServSAT LIKE @termino)
            ORDER BY p.Nombre";

                var cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@termino", string.IsNullOrEmpty(termino) ? "%" : $"%{termino}%");

                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            ProductoID = (int)dr["ProductoID"],
                            Nombre = dr["Nombre"].ToString()!,
                            CodigoInterno = dr["Codigo"] as string,
                            NombreCategoria = dr["Categoria"].ToString(),
                            ClaveProdServSATID = dr["ClaveProdServSAT"].ToString()!,
                            ClaveUnidadSAT = dr["ClaveUnidadSAT"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        // Obtener solo lotes con stock disponible (para ventas, salidas, etc.)
        public List<LoteProducto> ObtenerLotesDisponibles(int productoId)
        {
            var lista = new List<LoteProducto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
            SELECT LoteID, ProductoID, FechaEntrada, FechaCaducidad,
                   CantidadDisponible, PrecioCompra, PrecioVenta, Estatus
            FROM LotesProducto
            WHERE ProductoID = @ProductoID 
              AND CantidadDisponible > 0
              AND Estatus = 1
            ORDER BY ISNULL(FechaCaducidad, '2099-12-31') ASC, FechaEntrada ASC";

                var cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ProductoID", productoId);

                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            FechaEntrada = (DateTime)dr["FechaEntrada"],
                            FechaCaducidad = dr["FechaCaducidad"] as DateTime?,
                            CantidadDisponible = (int)dr["CantidadDisponible"],
                            PrecioCompra = (decimal)dr["PrecioCompra"],
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Estatus = (bool)dr["Estatus"]
                        });
                    }
                }
            }
            return lista;
        }
        public bool ActualizarLote(LoteProducto lote)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
            UPDATE LotesProducto SET
                CantidadTotal = @CantidadTotal,
                CantidadDisponible = @CantidadDisponible,
                FechaCaducidad = @FechaCaducidad,
                PrecioCompra = @PrecioCompra,
                PrecioVenta = @PrecioVenta,
                Usuario = @Usuario,
                UltimaAct = GETDATE()
            WHERE LoteID = @LoteID", cnx);

                cmd.Parameters.AddWithValue("@CantidadTotal", lote.CantidadTotal);
                cmd.Parameters.AddWithValue("@CantidadDisponible", lote.CantidadDisponible);
                cmd.Parameters.AddWithValue("@FechaCaducidad", (object)lote.FechaCaducidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioCompra", lote.PrecioCompra);
                cmd.Parameters.AddWithValue("@PrecioVenta", lote.PrecioVenta);
                cmd.Parameters.AddWithValue("@Usuario", lote.Usuario ?? "system");
                cmd.Parameters.AddWithValue("@LoteID", lote.LoteID);

                try { cnx.Open(); return cmd.ExecuteNonQuery() > 0; }
                catch { return false; }
            }
        }

        public bool RegistrarMovimientoInventario(MovimientoInventario movimiento)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO MovimientosInventario (
                        LoteID, ProductoID, TipoMovimiento, Cantidad, 
                        CostoUnitario, Usuario, Fecha, Comentarios
                    ) VALUES (
                        @LoteID, @ProductoID, @TipoMovimiento, @Cantidad,
                        @CostoUnitario, @Usuario, @Fecha, @Comentarios
                    )", cnx);

                cmd.Parameters.AddWithValue("@LoteID", movimiento.LoteID);
                cmd.Parameters.AddWithValue("@ProductoID", movimiento.ProductoID);
                cmd.Parameters.AddWithValue("@TipoMovimiento", movimiento.TipoMovimiento);
                cmd.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
                cmd.Parameters.AddWithValue("@CostoUnitario", movimiento.CostoUnitario);
                cmd.Parameters.AddWithValue("@Usuario", movimiento.Usuario);
                cmd.Parameters.AddWithValue("@Fecha", movimiento.Fecha);
                cmd.Parameters.AddWithValue("@Comentarios", movimiento.Comentarios ?? "");

                try { cnx.Open(); return cmd.ExecuteNonQuery() > 0; }
                catch (Exception ex) 
                { 
                    System.Diagnostics.Debug.WriteLine($"Error registrando movimiento: {ex.Message}");
                    return false; 
                }
            }
        }

        public bool RegistrarMerma(int loteID, int cantidad, string usuario, string comentarios)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction trx = cnx.BeginTransaction();

                try
                {
                    // Obtener información del lote
                    decimal costoUnitario = 0;
                    int productoID = 0;
                    string nombreProducto = "";

                    var cmdInfo = new SqlCommand(@"
                        SELECT lp.ProductoID, lp.PrecioCompra, p.Nombre
                        FROM LotesProducto lp
                        INNER JOIN Productos p ON lp.ProductoID = p.ProductoID
                        WHERE lp.LoteID = @LoteID", cnx, trx);
                    cmdInfo.Parameters.AddWithValue("@LoteID", loteID);

                    using (var dr = cmdInfo.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            productoID = (int)dr["ProductoID"];
                            costoUnitario = (decimal)dr["PrecioCompra"];
                            nombreProducto = dr["Nombre"].ToString();
                        }
                        else
                        {
                            throw new Exception("Lote no encontrado");
                        }
                    }

                    decimal montoTotal = cantidad * costoUnitario;

                    // Descontar inventario
                    var cmd1 = new SqlCommand(@"
                UPDATE LotesProducto
                SET CantidadDisponible = CantidadDisponible - @Cantidad
                WHERE LoteID = @LoteID", cnx, trx);

                    cmd1.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd1.Parameters.AddWithValue("@LoteID", loteID);
                    cmd1.ExecuteNonQuery();

                    // Registrar movimiento
                    var cmd2 = new SqlCommand(@"
                INSERT INTO InventarioMovimientos 
                (LoteID, ProductoID, TipoMovimiento, Cantidad, CostoUnitario, Usuario, Comentarios)
                VALUES (@LoteID, @ProductoID, 'MERMA', @Cantidad, @CostoUnitario, @Usuario, @Comentarios)", cnx, trx);

                    cmd2.Parameters.AddWithValue("@LoteID", loteID);
                    cmd2.Parameters.AddWithValue("@ProductoID", productoID);
                    cmd2.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd2.Parameters.AddWithValue("@CostoUnitario", costoUnitario);
                    cmd2.Parameters.AddWithValue("@Usuario", usuario);
                    cmd2.Parameters.AddWithValue("@Comentarios", comentarios ?? "");
                    cmd2.ExecuteNonQuery();

                    // Generar póliza contable
                    var poliza = new Poliza
                    {
                        TipoPoliza = "DIARIO",
                        FechaPoliza = DateTime.Now,
                        Concepto = $"Merma de Inventario - {nombreProducto} (Lote {loteID})",
                        Referencia = $"MERMA-{loteID}",
                        Usuario = usuario,
                        EsAutomatica = true,
                        DocumentoOrigen = $"MERMA-{loteID}",
                        Estatus = "CERRADA",
                        Observaciones = comentarios,
                        Detalles = new System.Collections.Generic.List<PolizaDetalle>
                        {
                            // DEBE: Pérdida por Merma (disminuye resultado, aumenta gasto)
                            new PolizaDetalle
                            {
                                CuentaID = ObtenerCuentaContable("5302", cnx, trx), // Pérdida por Merma
                                Debe = montoTotal,
                                Haber = 0,
                                Concepto = $"Merma - {nombreProducto} ({cantidad} unidades)"
                            },
                            // HABER: Inventario (disminuye activo)
                            new PolizaDetalle
                            {
                                CuentaID = ObtenerCuentaContable("1400", cnx, trx), // Inventario
                                Debe = 0,
                                Haber = montoTotal,
                                Concepto = $"Baja de inventario - {nombreProducto}"
                            }
                        }
                    };

                    bool polizaOk = CD_Poliza.Instancia.CrearPoliza(poliza, cnx, trx);
                    if (!polizaOk)
                    {
                        throw new Exception("Error al crear póliza de merma");
                    }

                    trx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error registrando merma: {ex.Message}");
                    throw; // Propagar la excepción para que el controlador pueda capturar el mensaje
                }
            }
        }
        public bool AjustarInventario(int loteID, int nuevaCantidad, string usuario, string comentarios)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();

                // Actualizar inventario
                var cmd = new SqlCommand(@"
            UPDATE LotesProducto
            SET CantidadDisponible = @NuevaCantidad, UltimaAct = GETDATE()
            WHERE LoteID = @LoteID", cnx);

                cmd.Parameters.AddWithValue("@NuevaCantidad", nuevaCantidad);
                cmd.Parameters.AddWithValue("@LoteID", loteID);

                bool ok = cmd.ExecuteNonQuery() > 0;

                if (ok)
                {
                    // Registrar movimiento
                    var mov = new SqlCommand(@"
                INSERT INTO InventarioMovimientos
                (LoteID, ProductoID, TipoMovimiento, Cantidad, Usuario, Comentarios)
                SELECT @LoteID, ProductoID, 'AJUSTE', @NuevaCantidad, @Usuario, @Comentarios
                FROM LotesProducto WHERE LoteID = @LoteID", cnx);

                    mov.Parameters.AddWithValue("@LoteID", loteID);
                    mov.Parameters.AddWithValue("@NuevaCantidad", nuevaCantidad);
                    mov.Parameters.AddWithValue("@Usuario", usuario);
                    mov.Parameters.AddWithValue("@Comentarios", comentarios);
                    mov.ExecuteNonQuery();
                }

                return ok;
            }
        }
        public LoteProducto ObtenerLotePorID(int loteID)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
            SELECT * FROM LotesProducto WHERE LoteID = @id", cnx);

                cmd.Parameters.AddWithValue("@id", loteID);

                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            FechaEntrada = (DateTime)dr["FechaEntrada"],
                            FechaCaducidad = dr["FechaCaducidad"] as DateTime?,
                            CantidadTotal = (int)dr["CantidadTotal"],
                            CantidadDisponible = (int)dr["CantidadDisponible"],
                            PrecioCompra = (decimal)dr["PrecioCompra"],
                            PrecioVenta = (decimal)dr["PrecioVenta"]
                        };
                    }
                }
            }
            return null;
        }
        public LoteProducto ObtenerLotePorId(int loteId)
        {
            LoteProducto lote = null;
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM LotesProducto WHERE LoteID = @LoteID", conn))
                {
                    cmd.Parameters.AddWithValue("@LoteID", loteId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lote = new LoteProducto
                            {
                                LoteID = Convert.ToInt32(reader["LoteID"]),
                                ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                FechaEntrada = Convert.ToDateTime(reader["FechaEntrada"]),
                                FechaCaducidad = reader["FechaCaducidad"] as DateTime?,
                                CantidadTotal = Convert.ToInt32(reader["CantidadTotal"]),
                                CantidadDisponible = Convert.ToInt32(reader["CantidadDisponible"]),
                                PrecioCompra = Convert.ToDecimal(reader["PrecioCompra"]),
                                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                                Usuario = reader["Usuario"].ToString(),
                                UltimaAct = Convert.ToDateTime(reader["UltimaAct"]),
                                Estatus = Convert.ToBoolean(reader["Estatus"])
                            };
                        }
                    }
                }
            }
            return lote;
        }

        public bool RegistrarAjuste(AjusteInventario ajuste)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                SqlTransaction trx = conn.BeginTransaction();

                try
                {
                    // Obtener información del producto y lote
                    decimal costoUnitario = 0;
                    string nombreProducto = "";
                    long loteID = ajuste.LoteEntradaId ?? 0;

                    if (loteID > 0)
                    {
                        var cmdInfo = new SqlCommand(@"
                            SELECT lp.PrecioCompra, p.Nombre
                            FROM LotesProducto lp
                            INNER JOIN Productos p ON lp.ProductoID = p.ProductoID
                            WHERE lp.LoteID = @LoteID", conn, trx);
                        cmdInfo.Parameters.AddWithValue("@LoteID", loteID);

                        using (var dr = cmdInfo.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                costoUnitario = (decimal)dr["PrecioCompra"];
                                nombreProducto = dr["Nombre"].ToString();
                            }
                        }
                    }
                    else
                    {
                        // Si no hay lote, obtener nombre del producto
                        var cmdProd = new SqlCommand("SELECT Nombre FROM Productos WHERE ProductoID = @ProductoID", conn, trx);
                        cmdProd.Parameters.AddWithValue("@ProductoID", ajuste.ProductoId);
                        nombreProducto = cmdProd.ExecuteScalar()?.ToString() ?? "Producto";
                    }

                    // Si no se pudo obtener costo, usar valor promedio o 0
                    if (costoUnitario == 0 && loteID > 0)
                    {
                        var cmdCosto = new SqlCommand("SELECT AVG(PrecioCompra) FROM LotesProducto WHERE ProductoID = @ProductoID", conn, trx);
                        cmdCosto.Parameters.AddWithValue("@ProductoID", ajuste.ProductoId);
                        var resultCosto = cmdCosto.ExecuteScalar();
                        costoUnitario = resultCosto != null && resultCosto != DBNull.Value 
                            ? Convert.ToDecimal(resultCosto) 
                            : 0;
                    }

                    decimal montoTotal = ajuste.Cantidad * costoUnitario;

                    // Insertar registro en AjustesInventario
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO AjustesInventario 
                        (Fecha, ProductoId, LoteEntradaId, Tipo, Cantidad, Motivo, UsuarioId) 
                        VALUES (GETDATE(), @ProductoId, @LoteEntradaId, @Tipo, @Cantidad, @Motivo, @UsuarioId)", conn, trx))
                    {
                        cmd.Parameters.AddWithValue("@ProductoId", ajuste.ProductoId);
                        cmd.Parameters.AddWithValue("@LoteEntradaId", ajuste.LoteEntradaId.HasValue ? (object)ajuste.LoteEntradaId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Tipo", ajuste.Tipo ?? "AJUSTE");
                        cmd.Parameters.AddWithValue("@Cantidad", ajuste.Cantidad);
                        cmd.Parameters.AddWithValue("@Motivo", ajuste.Motivo ?? "");
                        cmd.Parameters.AddWithValue("@UsuarioId", ajuste.UsuarioId);
                        cmd.ExecuteNonQuery();
                    }

                    // Actualizar inventario en LotesProducto (si hay lote especificado)
                    if (loteID > 0)
                    {
                        // Ajuste positivo: aumenta inventario
                        // Ajuste negativo: disminuye inventario
                        string tipoOperacion = ajuste.Tipo?.ToUpper() ?? "AJUSTE";
                        bool esPositivo = tipoOperacion.Contains("POSITIVO") || tipoOperacion.Contains("ENTRADA") || tipoOperacion.Contains("SOBRANTE");
                        
                        string operador = esPositivo ? "+" : "-";
                        var cmdUpdate = new SqlCommand($@"
                            UPDATE LotesProducto
                            SET CantidadDisponible = CantidadDisponible {operador} @Cantidad
                            WHERE LoteID = @LoteID", conn, trx);
                        cmdUpdate.Parameters.AddWithValue("@Cantidad", Math.Abs(ajuste.Cantidad));
                        cmdUpdate.Parameters.AddWithValue("@LoteID", loteID);
                        cmdUpdate.ExecuteNonQuery();

                        // Registrar movimiento en InventarioMovimientos
                        var cmdMov = new SqlCommand(@"
                            INSERT INTO InventarioMovimientos 
                            (LoteID, ProductoID, TipoMovimiento, Cantidad, CostoUnitario, Usuario, Comentarios)
                            VALUES (@LoteID, @ProductoID, @TipoMovimiento, @Cantidad, @CostoUnitario, @Usuario, @Comentarios)", conn, trx);
                        cmdMov.Parameters.AddWithValue("@LoteID", loteID);
                        cmdMov.Parameters.AddWithValue("@ProductoID", ajuste.ProductoId);
                        cmdMov.Parameters.AddWithValue("@TipoMovimiento", esPositivo ? "AJUSTE_ENTRADA" : "AJUSTE_SALIDA");
                        cmdMov.Parameters.AddWithValue("@Cantidad", Math.Abs(ajuste.Cantidad));
                        cmdMov.Parameters.AddWithValue("@CostoUnitario", costoUnitario);
                        cmdMov.Parameters.AddWithValue("@Usuario", ajuste.UsuarioId.ToString());
                        cmdMov.Parameters.AddWithValue("@Comentarios", ajuste.Motivo ?? "");
                        cmdMov.ExecuteNonQuery();
                    }

                    // Generar póliza contable solo si hay monto
                    if (montoTotal > 0)
                    {
                        string tipoOperacion = ajuste.Tipo?.ToUpper() ?? "AJUSTE";
                        bool esPositivo = tipoOperacion.Contains("POSITIVO") || tipoOperacion.Contains("ENTRADA") || tipoOperacion.Contains("SOBRANTE");

                        Poliza poliza;
                        
                        if (esPositivo)
                        {
                            // AJUSTE POSITIVO (Sobrante): Aumenta inventario
                            // DEBE  1400 Inventario
                            // HABER 4100 Otros Productos
                            poliza = new Poliza
                            {
                                TipoPoliza = "DIARIO",
                                FechaPoliza = DateTime.Now,
                                Concepto = $"Ajuste Positivo de Inventario - {nombreProducto}",
                                Referencia = $"AJUSTE-{ajuste.ProductoId}-{DateTime.Now:yyyyMMddHHmmss}",
                                Usuario = ajuste.UsuarioId.ToString(),
                                EsAutomatica = true,
                                DocumentoOrigen = $"AJUSTE-{ajuste.ProductoId}",
                                Estatus = "CERRADA",
                                Observaciones = ajuste.Motivo,
                                Detalles = new System.Collections.Generic.List<PolizaDetalle>
                                {
                                    new PolizaDetalle
                                    {
                                        CuentaID = ObtenerCuentaContable("1400", conn, trx), // Inventario
                                        Debe = montoTotal,
                                        Haber = 0,
                                        Concepto = $"Ajuste positivo - {nombreProducto} ({ajuste.Cantidad} unidades)"
                                    },
                                    new PolizaDetalle
                                    {
                                        CuentaID = ObtenerCuentaContable("4100", conn, trx), // Otros Productos
                                        Debe = 0,
                                        Haber = montoTotal,
                                        Concepto = $"Sobrante de inventario - {nombreProducto}"
                                    }
                                }
                            };
                        }
                        else
                        {
                            // AJUSTE NEGATIVO (Faltante): Disminuye inventario
                            // DEBE  5303 Ajuste de Inventario - Faltante
                            // HABER 1400 Inventario
                            poliza = new Poliza
                            {
                                TipoPoliza = "DIARIO",
                                FechaPoliza = DateTime.Now,
                                Concepto = $"Ajuste Negativo de Inventario - {nombreProducto}",
                                Referencia = $"AJUSTE-{ajuste.ProductoId}-{DateTime.Now:yyyyMMddHHmmss}",
                                Usuario = ajuste.UsuarioId.ToString(),
                                EsAutomatica = true,
                                DocumentoOrigen = $"AJUSTE-{ajuste.ProductoId}",
                                Estatus = "CERRADA",
                                Observaciones = ajuste.Motivo,
                                Detalles = new System.Collections.Generic.List<PolizaDetalle>
                                {
                                    new PolizaDetalle
                                    {
                                        CuentaID = ObtenerCuentaContable("5303", conn, trx), // Ajuste Faltante
                                        Debe = montoTotal,
                                        Haber = 0,
                                        Concepto = $"Faltante - {nombreProducto} ({ajuste.Cantidad} unidades)"
                                    },
                                    new PolizaDetalle
                                    {
                                        CuentaID = ObtenerCuentaContable("1400", conn, trx), // Inventario
                                        Debe = 0,
                                        Haber = montoTotal,
                                        Concepto = $"Baja de inventario - {nombreProducto}"
                                    }
                                }
                            };
                        }

                        bool polizaOk = CD_Poliza.Instancia.CrearPoliza(poliza, conn, trx);
                        if (!polizaOk)
                        {
                            throw new Exception("Error al crear póliza de ajuste");
                        }
                    }

                    trx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error registrando ajuste: {ex.Message}");
                    throw; // Propagar la excepción para que el controlador pueda capturar el mensaje
                }
            }
        }

        // Obtener datos fiscales (IVA, Exento) de un producto para auto-poblar en ventas/compras
        public dynamic ObtenerDatosFiscales(int productoId)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT TOP 1 
                        iva.Porcentaje AS TasaIVAPorcentaje,
                        p.Exento
                    FROM Productos p
                    INNER JOIN CatTasaIVA iva ON p.TasaIVAID = iva.TasaIVAID
                    WHERE p.ProductoID = @ProductoID";

                var cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ProductoID", productoId);
                cnx.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new
                        {
                            TasaIVAPorcentaje = Convert.ToDecimal(dr["TasaIVAPorcentaje"]),
                            Exento = Convert.ToBoolean(dr["Exento"])
                        };
                    }
                }
            }
            // Default values si no encuentra el producto
            return new { TasaIVAPorcentaje = 16.00m, Exento = false };
        }

        public List<MovimientoInventario> ObtenerMovimientosInventario(int? productoId, int? loteId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var lista = new List<MovimientoInventario>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        m.MovimientoID, m.LoteID, m.ProductoID, m.TipoMovimiento,
                        m.Cantidad, m.CostoUnitario, m.Usuario, m.Fecha, m.Comentarios
                    FROM MovimientosInventario m
                    WHERE 1=1";

                if (productoId.HasValue)
                    query += " AND m.ProductoID = @ProductoID";
                if (loteId.HasValue)
                    query += " AND m.LoteID = @LoteID";
                if (fechaInicio.HasValue)
                    query += " AND m.Fecha >= @FechaInicio";
                if (fechaFin.HasValue)
                    query += " AND m.Fecha <= @FechaFin";

                query += " ORDER BY m.Fecha DESC";

                var cmd = new SqlCommand(query, cnx);
                if (productoId.HasValue) cmd.Parameters.AddWithValue("@ProductoID", productoId.Value);
                if (loteId.HasValue) cmd.Parameters.AddWithValue("@LoteID", loteId.Value);
                if (fechaInicio.HasValue) cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                if (fechaFin.HasValue) cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value.AddDays(1));

                try
                {
                    cnx.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MovimientoInventario
                            {
                                MovimientoID = (int)dr["MovimientoID"],
                                LoteID = (int)dr["LoteID"],
                                ProductoID = (int)dr["ProductoID"],
                                TipoMovimiento = dr["TipoMovimiento"].ToString(),
                                Cantidad = (int)dr["Cantidad"],
                                CostoUnitario = (decimal)dr["CostoUnitario"],
                                Usuario = dr["Usuario"].ToString(),
                                Fecha = (DateTime)dr["Fecha"],
                                Comentarios = dr["Comentarios"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error obteniendo movimientos: {ex.Message}");
                }
            }
            return lista;
        }

        // Método helper para obtener CuentaID desde CatalogoContable
        private int ObtenerCuentaContable(string codigoCuenta, SqlConnection cnx, SqlTransaction trx)
        {
            var query = "SELECT CuentaID FROM CatalogoContable WHERE CodigoCuenta = @Codigo AND Activo = 1";
            using (var cmd = new SqlCommand(query, cnx, trx))
            {
                cmd.Parameters.AddWithValue("@Codigo", codigoCuenta);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    throw new Exception($"Cuenta contable {codigoCuenta} no encontrada en el catálogo");
                }
                return Convert.ToInt32(result);
            }
        }
    }
}