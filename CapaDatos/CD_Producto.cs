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
            SELECT TOP(20) 
                ProductoID, Nombre, CodigoInterno, ClaveProdServSAT, ClaveUnidadSAT
            FROM Productos
            WHERE Estatus = 1
              AND (Nombre LIKE @termino 
                   OR CodigoInterno LIKE @termino 
                   OR ClaveProdServSAT LIKE @termino)
            ORDER BY Nombre";

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
                            CodigoInterno = dr["CodigoInterno"] as string,
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
                   CantidadDisponible, PrecioCompra, PrecioVenta
            FROM LotesProducto
            WHERE ProductoID = @ProductoID 
              AND CantidadDisponible > 0
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
                            PrecioVenta = (decimal)dr["PrecioVenta"]
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
                SELECT LoteID, ProductoID, 'MERMA', @Cantidad, PrecioCompra, @Usuario, @Comentarios
                FROM LotesProducto WHERE LoteID = @LoteID", cnx, trx);

                    cmd2.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd2.Parameters.AddWithValue("@Usuario", usuario);
                    cmd2.Parameters.AddWithValue("@Comentarios", comentarios);
                    cmd2.Parameters.AddWithValue("@LoteID", loteID);
                    cmd2.ExecuteNonQuery();

                    trx.Commit();
                    return true;
                }
                catch
                {
                    trx.Rollback();
                    return false;
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
            using (SqlConnection conn = new SqlConnection(/* Cadena de conexión */))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AjustesInventario (Fecha, ProductoId, LoteEntradaId, Tipo, Cantidad, Motivo, UsuarioId) VALUES (GETDATE(), @ProductoId, @LoteEntradaId, @Tipo, @Cantidad, @Motivo, @UsuarioId)", conn))
                {
                    cmd.Parameters.AddWithValue("@ProductoId", ajuste.ProductoId);
                    cmd.Parameters.AddWithValue("@LoteEntradaId", ajuste.LoteEntradaId);
                    cmd.Parameters.AddWithValue("@Tipo", ajuste.Tipo);
                    cmd.Parameters.AddWithValue("@Cantidad", ajuste.Cantidad);
                    cmd.Parameters.AddWithValue("@Motivo", ajuste.Motivo);
                    cmd.Parameters.AddWithValue("@UsuarioId", ajuste.UsuarioId);
                    return cmd.ExecuteNonQuery() > 0;
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
    }
}