// CapaDatos/CD_Producto.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
                           p.Exento, p.Estatus, p.StockMinimo
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
                            Estatus = (bool)dr["Estatus"],
                            StockMinimo = dr["StockMinimo"] as int?
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
                cmd.Parameters.AddWithValue("@StockMinimo", p.StockMinimo ?? (object)DBNull.Value);
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
                cmd.Parameters.AddWithValue("@StockMinimo", p.StockMinimo ?? (object)DBNull.Value);
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

        /// <summary>
        /// Obtener lotes próximos a vencer para alertas
        /// </summary>
        public List<AlertaCaducidad> ObtenerLotesProximosAVencer(int diasAnticipacion = 7, int sucursalId = 0)
        {
            var alertas = new List<AlertaCaducidad>();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT 
                        l.LoteID,
                        l.ProductoID,
                        p.Nombre AS NombreProducto,
                        p.CodigoInterno,
                        c.Nombre AS Categoria,
                        l.FechaCaducidad,
                        DATEDIFF(DAY, GETDATE(), l.FechaCaducidad) AS DiasRestantes,
                        l.CantidadDisponible,
                        l.PrecioVenta,
                        (l.CantidadDisponible * l.PrecioVenta) AS ValorInventario,
                        s.Nombre AS NombreSucursal,
                        l.FechaEntrada
                    FROM LotesProducto l
                    INNER JOIN Producto p ON l.ProductoID = p.ProductoID
                    INNER JOIN Categoria c ON p.CategoriaID = c.CategoriaID
                    INNER JOIN Sucursal s ON l.SucursalID = s.SucursalID
                    WHERE l.CantidadDisponible > 0
                        AND l.FechaCaducidad IS NOT NULL
                        AND l.FechaCaducidad <= DATEADD(DAY, @DiasAnticipacion, GETDATE())
                        AND l.FechaCaducidad > GETDATE()
                        AND l.Estatus = 1
                        " + (sucursalId > 0 ? "AND l.SucursalID = @SucursalID" : "") + @"
                    ORDER BY DiasRestantes ASC, l.CantidadDisponible DESC";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@DiasAnticipacion", diasAnticipacion);
                if (sucursalId > 0)
                    cmd.Parameters.AddWithValue("@SucursalID", sucursalId);
                
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int diasRestantes = Convert.ToInt32(dr["DiasRestantes"]);
                        string nivelAlerta = diasRestantes <= 2 ? "URGENTE" : 
                                           diasRestantes <= 5 ? "ALTO" : "MEDIO";
                        
                        alertas.Add(new AlertaCaducidad
                        {
                            LoteID = Convert.ToInt32(dr["LoteID"]),
                            ProductoID = Convert.ToInt32(dr["ProductoID"]),
                            NombreProducto = dr["NombreProducto"].ToString(),
                            CodigoInterno = dr["CodigoInterno"]?.ToString(),
                            Categoria = dr["Categoria"].ToString(),
                            FechaCaducidad = Convert.ToDateTime(dr["FechaCaducidad"]),
                            DiasRestantes = diasRestantes,
                            CantidadDisponible = Convert.ToInt32(dr["CantidadDisponible"]),
                            PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                            ValorInventario = Convert.ToDecimal(dr["ValorInventario"]),
                            NombreSucursal = dr["NombreSucursal"].ToString(),
                            FechaEntrada = Convert.ToDateTime(dr["FechaEntrada"]),
                            NivelAlerta = nivelAlerta
                        });
                    }
                }
            }
            
            return alertas;
        }

        public List<LoteProducto> ObtenerLotes(int productoId, int sucursalID = 0)
        {
            var lista = new List<LoteProducto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT LoteID, ProductoID, SucursalID, FechaEntrada, FechaCaducidad,
                           CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta
                    FROM LotesProducto 
                    WHERE ProductoID = @id";
                
                if (sucursalID > 0)
                    query += " AND SucursalID = @sucursalID";
                    
                query += " ORDER BY FechaEntrada DESC";
                
                var cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@id", productoId);
                if (sucursalID > 0)
                    cmd.Parameters.AddWithValue("@sucursalID", sucursalID);
                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            SucursalID = (int)dr["SucursalID"],
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
                // Usar INSERT directo en lugar de SP inexistente
                var cmd = new SqlCommand(@"
                    INSERT INTO LotesProducto (
                        ProductoID, SucursalID, FechaEntrada, CantidadTotal, CantidadDisponible,
                        PrecioCompra, PrecioVenta, Usuario, UltimaAct, Estatus
                    ) VALUES (
                        @ProductoID, @SucursalID, GETDATE(), @CantidadTotal, @CantidadTotal,
                        @PrecioCompra, @PrecioVenta, @Usuario, GETDATE(), 1
                    )", cnx);
                
                cmd.Parameters.AddWithValue("@ProductoID", lote.ProductoID);
                cmd.Parameters.AddWithValue("@SucursalID", lote.SucursalID > 0 ? lote.SucursalID : 1); // Default sucursal 1
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
                            ClaveUnidadSAT = dr["ClaveUnidadSAT"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // Obtener solo lotes con stock disponible (para ventas, salidas, etc.)
        public List<LoteProducto> ObtenerLotesDisponibles(int productoId, int sucursalID = 0)
        {
            var lista = new List<LoteProducto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT LoteID, ProductoID, SucursalID, FechaEntrada, FechaCaducidad,
                           CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta,
                           Usuario, UltimaAct, Estatus
                    FROM LotesProducto 
                    WHERE ProductoID = @ProductoID 
                        AND CantidadDisponible > 0
                        AND Estatus = 1
                        " + (sucursalID > 0 ? "AND SucursalID = @SucursalID" : "") + @"
                    ORDER BY FechaEntrada ASC";
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ProductoID", productoId);
                if (sucursalID > 0)
                    cmd.Parameters.AddWithValue("@SucursalID", sucursalID);
                
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            SucursalID = (int)dr["SucursalID"],
                            FechaEntrada = (DateTime)dr["FechaEntrada"],
                            FechaCaducidad = dr["FechaCaducidad"] != DBNull.Value ? (DateTime?)dr["FechaCaducidad"] : null,
                            CantidadTotal = (int)dr["CantidadTotal"],
                            CantidadDisponible = (int)dr["CantidadDisponible"],
                            PrecioCompra = (decimal)dr["PrecioCompra"],
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = (DateTime)dr["UltimaAct"],
                            Estatus = (bool)dr["Estatus"]
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// FIFO AUTOMÁTICO: Obtiene lotes suficientes para cubrir la cantidad requerida,
        /// priorizando los más antiguos (First In, First Out)
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <param name="sucursalId">ID de la sucursal</param>
        /// <param name="cantidadRequerida">Cantidad necesaria</param>
        /// <returns>Lista de lotes ordenados por fecha (más antiguo primero) con cantidad a usar de cada uno</returns>
        public List<LoteParaVenta> ObtenerLotesFIFO(int productoId, int sucursalId, decimal cantidadRequerida)
        {
            var lotesSeleccionados = new List<LoteParaVenta>();
            decimal cantidadRestante = cantidadRequerida;
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                // Obtener lotes disponibles ordenados por fecha de entrada (FIFO)
                string query = @"
                    SELECT 
                        LoteID, ProductoID, SucursalID, FechaEntrada, FechaCaducidad,
                        CantidadTotal, CantidadDisponible, PrecioCompra, PrecioVenta,
                        Usuario, UltimaAct, Estatus
                    FROM LotesProducto
                    WHERE ProductoID = @ProductoID 
                        AND SucursalID = @SucursalID
                        AND CantidadDisponible > 0
                        AND Estatus = 1
                        AND (FechaCaducidad IS NULL OR FechaCaducidad > GETDATE())
                    ORDER BY FechaEntrada ASC"; // Más antiguo primero
                
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ProductoID", productoId);
                cmd.Parameters.AddWithValue("@SucursalID", sucursalId);
                
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read() && cantidadRestante > 0)
                    {
                        var lote = new LoteProducto
                        {
                            LoteID = (int)dr["LoteID"],
                            ProductoID = (int)dr["ProductoID"],
                            SucursalID = (int)dr["SucursalID"],
                            FechaEntrada = (DateTime)dr["FechaEntrada"],
                            FechaCaducidad = dr["FechaCaducidad"] != DBNull.Value ? (DateTime?)dr["FechaCaducidad"] : null,
                            CantidadTotal = (int)dr["CantidadTotal"],
                            CantidadDisponible = (int)dr["CantidadDisponible"],
                            PrecioCompra = (decimal)dr["PrecioCompra"],
                            PrecioVenta = (decimal)dr["PrecioVenta"],
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = (DateTime)dr["UltimaAct"],
                            Estatus = (bool)dr["Estatus"]
                        };
                        
                        // Calcular cuánto se tomará de este lote
                        decimal cantidadAUsar = Math.Min(cantidadRestante, lote.CantidadDisponible);
                        
                        lotesSeleccionados.Add(new LoteParaVenta
                        {
                            Lote = lote,
                            CantidadAUsar = cantidadAUsar
                        });
                        
                        cantidadRestante -= cantidadAUsar;
                    }
                }
            }
            
            // Validar si se cubrió la cantidad requerida
            if (cantidadRestante > 0)
            {
                throw new Exception($"Stock insuficiente. Faltan {cantidadRestante:N2} unidades.");
            }
            
            return lotesSeleccionados;
        }

        /// <summary>
        /// Descontar automáticamente de lotes usando FIFO
        /// </summary>
        public bool DescontarStockFIFO(int productoId, int sucursalId, decimal cantidad, string usuario)
        {
            try
            {
                var lotes = ObtenerLotesFIFO(productoId, sucursalId, cantidad);
                
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    SqlTransaction tran = cnx.BeginTransaction();
                    
                    try
                    {
                        foreach (var loteVenta in lotes)
                        {
                            string update = @"
                                UPDATE LotesProducto 
                                SET CantidadDisponible = CantidadDisponible - @Cantidad,
                                    UltimaAct = GETDATE(),
                                    Usuario = @Usuario
                                WHERE LoteID = @LoteID";
                            
                            SqlCommand cmd = new SqlCommand(update, cnx, tran);
                            cmd.Parameters.AddWithValue("@Cantidad", loteVenta.CantidadAUsar);
                            cmd.Parameters.AddWithValue("@Usuario", usuario);
                            cmd.Parameters.AddWithValue("@LoteID", loteVenta.Lote.LoteID);
                            cmd.ExecuteNonQuery();
                        }
                        
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                return false;
            }
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
                            SucursalID = (int)dr["SucursalID"],
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
                                SucursalID = Convert.ToInt32(reader["SucursalID"]),
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

        // Obtener productos con stock disponible en una sucursal
        public List<Producto> ObtenerProductosConStock(int sucursalID)
        {
            var lista = new List<Producto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT p.ProductoID, p.Nombre, p.CodigoInterno, 
                           SUM(l.CantidadDisponible) AS Stock,
                           p.VentaPorGramaje, p.PrecioPorKilo, p.UnidadMedidaBase,
                           c.Nombre AS NombreCategoria
                    FROM Productos p
                    INNER JOIN LotesProducto l ON p.ProductoID = l.ProductoID
                    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                    WHERE l.SucursalID = @SucursalID 
                      AND l.CantidadDisponible > 0 
                      AND l.Estatus = 1
                      AND p.Estatus = 1
                    GROUP BY p.ProductoID, p.Nombre, p.CodigoInterno, 
                             p.VentaPorGramaje, p.PrecioPorKilo, p.UnidadMedidaBase,
                             c.Nombre
                    ORDER BY p.Nombre";

                var cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@SucursalID", sucursalID);
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
                            StockActual = dr["Stock"] != DBNull.Value ? Convert.ToInt32(dr["Stock"]) : 0,
                            NombreCategoria = dr["NombreCategoria"]?.ToString(),
                            VentaPorGramaje = dr["VentaPorGramaje"] != DBNull.Value && (bool)dr["VentaPorGramaje"],
                            PrecioPorKilo = dr["PrecioPorKilo"] as decimal?,
                            UnidadMedidaBase = dr["UnidadMedidaBase"] as string
                        });
                    }
                }
            }
            return lista;
        }

        // Método helper para obtener CuentaID desde CatalogoContable
        private Guid? ObtenerCuentaContable(string codigoCuenta, SqlConnection cnx, SqlTransaction trx)
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
                // Convertir int a Guid
                int id = Convert.ToInt32(result);
                byte[] bytes = new byte[16];
                BitConverter.GetBytes(id).CopyTo(bytes, 0);
                return new Guid(bytes);
            }
        }

        // ==========================================
        // HISTORIAL DE CAMBIOS DE PRECIOS
        // ==========================================

        public int ObtenerStockPorSucursal(int productoID, int sucursalID)
        {
            int stock = 0;
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(lp.CantidadDisponible), 0) AS Stock
                    FROM LotesProducto lp
                    WHERE lp.ProductoID = @ProductoID 
                      AND lp.SucursalID = @SucursalID
                      AND lp.Estatus = 1 
                      AND lp.CantidadDisponible > 0", cnx);
                
                cmd.Parameters.AddWithValue("@ProductoID", productoID);
                cmd.Parameters.AddWithValue("@SucursalID", sucursalID);
                
                cnx.Open();
                var result = cmd.ExecuteScalar();
                stock = result != null ? Convert.ToInt32(result) : 0;
            }
            return stock;
        }

        public List<Producto> BuscarProductosPorSucursal(string termino, int sucursalID)
        {
            var lista = new List<Producto>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand(@"
                    SELECT DISTINCT p.ProductoID, p.Nombre, p.CodigoInterno, p.Descripcion,
                           p.CategoriaID, c.Nombre AS NombreCategoria
                    FROM Productos p
                    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                    WHERE p.Estatus = 1
                      AND (p.Nombre LIKE '%' + @Termino + '%' 
                           OR p.CodigoInterno LIKE '%' + @Termino + '%'
                           OR p.Descripcion LIKE '%' + @Termino + '%')
                    ORDER BY p.Nombre", cnx);
                
                cmd.Parameters.AddWithValue("@Termino", termino ?? "");
                cmd.Parameters.AddWithValue("@SucursalID", sucursalID);
                
                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            ProductoID = (int)dr["ProductoID"],
                            Nombre = dr["Nombre"].ToString(),
                            CodigoInterno = dr["CodigoInterno"]?.ToString(),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            CategoriaID = dr["CategoriaID"] == DBNull.Value ? 0 : (int)dr["CategoriaID"],
                            NombreCategoria = dr["NombreCategoria"]?.ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public List<CambioPrecio> ObtenerCambiosPreciosRecientes(int horas = 24)
        {
            var lista = new List<CambioPrecio>();
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("sp_ObtenerCambiosPreciosRecientes", cnx)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Horas", horas);

                cnx.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CambioPrecio
                        {
                            CambioID = (int)dr["CambioID"],
                            ProductoID = (int)dr["ProductoID"],
                            NombreProducto = dr["NombreProducto"].ToString(),
                            TipoPrecio = dr["TipoPrecio"].ToString(),
                            PrecioAnterior = (decimal)dr["PrecioAnterior"],
                            PrecioNuevo = (decimal)dr["PrecioNuevo"],
                            DiferenciaPorcentaje = dr["DiferenciaPorcentaje"] == DBNull.Value ? 0 : (decimal)dr["DiferenciaPorcentaje"],
                            Usuario = dr["Usuario"].ToString(),
                            FechaCambio = (DateTime)dr["FechaCambio"],
                            NombreSucursal = dr["NombreSucursal"] == DBNull.Value ? null : dr["NombreSucursal"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public bool RegistrarCambioPrecio(int productoID, string tipoPrecio, decimal precioAnterior, 
            decimal precioNuevo, string usuario, int? sucursalID = null, string observaciones = null)
        {
            using (var cnx = new SqlConnection(Conexion.CN))
            {
                var cmd = new SqlCommand("sp_RegistrarCambioPrecio", cnx)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductoID", productoID);
                cmd.Parameters.AddWithValue("@TipoPrecio", tipoPrecio);
                cmd.Parameters.AddWithValue("@PrecioAnterior", precioAnterior);
                cmd.Parameters.AddWithValue("@PrecioNuevo", precioNuevo);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@SucursalID", sucursalID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", observaciones ?? (object)DBNull.Value);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Obtiene productos con stock por debajo del mínimo
        /// </summary>
        public List<AlertaInventario> ObtenerProductosBajoStockMinimo(int? sucursalID = null)
        {
            var lista = new List<AlertaInventario>();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        SELECT 
                            p.ProductoID,
                            p.CodigoInterno,
                            p.Nombre AS NombreProducto,
                            c.Nombre AS Categoria,
                            ISNULL(stock.StockActual, 0) AS StockActual,
                            ISNULL(p.StockMinimo, 0) AS StockMinimo,
                            (ISNULL(p.StockMinimo, 0) - ISNULL(stock.StockActual, 0)) AS Diferencia,
                            CASE 
                                WHEN ISNULL(p.StockMinimo, 0) > 0 
                                THEN (CAST(ISNULL(stock.StockActual, 0) AS DECIMAL(10,2)) / CAST(p.StockMinimo AS DECIMAL(10,2)) * 100)
                                ELSE 100
                            END AS PorcentajeStock,
                            CASE 
                                WHEN ISNULL(stock.StockActual, 0) = 0 THEN 'AGOTADO'
                                WHEN ISNULL(stock.StockActual, 0) <= (ISNULL(p.StockMinimo, 0) * 0.25) THEN 'CRITICO'
                                WHEN ISNULL(stock.StockActual, 0) <= ISNULL(p.StockMinimo, 0) THEN 'BAJO'
                                ELSE 'NORMAL'
                            END AS NivelAlerta,
                            s.SucursalID,
                            s.Nombre AS NombreSucursal,
                            ultimaCompra.FechaCompra AS UltimaCompra,
                            DATEDIFF(DAY, ISNULL(ultimaCompra.FechaCompra, GETDATE()), GETDATE()) AS DiasDesdeUltimaCompra
                        FROM Productos p
                        INNER JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
                        CROSS JOIN SUCURSAL s
                        LEFT JOIN (
                            SELECT 
                                ProductoID, 
                                SucursalID,
                                SUM(CantidadDisponible) AS StockActual
                            FROM LotesProducto
                            WHERE Estatus = 1
                            GROUP BY ProductoID, SucursalID
                        ) stock ON p.ProductoID = stock.ProductoID AND s.SucursalID = stock.SucursalID
                        LEFT JOIN (
                            SELECT 
                                cd.ProductoID,
                                MAX(c.FechaCompra) AS FechaCompra
                            FROM ComprasDetalle cd
                            INNER JOIN Compras c ON cd.CompraID = c.CompraID
                            GROUP BY cd.ProductoID
                        ) ultimaCompra ON p.ProductoID = ultimaCompra.ProductoID
                        WHERE p.Estatus = 1
                          AND p.StockMinimo IS NOT NULL
                          AND p.StockMinimo > 0
                          AND ISNULL(stock.StockActual, 0) <= p.StockMinimo";
                    
                    if (sucursalID.HasValue && sucursalID.Value > 0)
                        query += " AND s.SucursalID = @SucursalID";
                        
                    query += @"
                        ORDER BY 
                            CASE 
                                WHEN ISNULL(stock.StockActual, 0) = 0 THEN 1
                                WHEN ISNULL(stock.StockActual, 0) <= (ISNULL(p.StockMinimo, 0) * 0.25) THEN 2
                                ELSE 3
                            END,
                            p.Nombre";
                    
                    var cmd = new SqlCommand(query, conn);
                    
                    if (sucursalID.HasValue && sucursalID.Value > 0)
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalID.Value);
                    
                    conn.Open();
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new AlertaInventario
                            {
                                ProductoID = dr.GetInt32(dr.GetOrdinal("ProductoID")),
                                CodigoInterno = dr.IsDBNull(dr.GetOrdinal("CodigoInterno")) ? "" : dr.GetString(dr.GetOrdinal("CodigoInterno")),
                                NombreProducto = dr.GetString(dr.GetOrdinal("NombreProducto")),
                                Categoria = dr.GetString(dr.GetOrdinal("Categoria")),
                                StockActual = dr.GetInt32(dr.GetOrdinal("StockActual")),
                                StockMinimo = dr.GetInt32(dr.GetOrdinal("StockMinimo")),
                                Diferencia = dr.GetInt32(dr.GetOrdinal("Diferencia")),
                                PorcentajeStock = dr.GetDecimal(dr.GetOrdinal("PorcentajeStock")),
                                NivelAlerta = dr.GetString(dr.GetOrdinal("NivelAlerta")),
                                SucursalID = dr.GetInt32(dr.GetOrdinal("SucursalID")),
                                NombreSucursal = dr.GetString(dr.GetOrdinal("NombreSucursal")),
                                UltimaCompra = dr.IsDBNull(dr.GetOrdinal("UltimaCompra")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("UltimaCompra")),
                                DiasDesdeUltimaCompra = dr.GetInt32(dr.GetOrdinal("DiasDesdeUltimaCompra"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error con detalles completos
                System.Diagnostics.Debug.WriteLine("Error en ObtenerProductosBajoStockMinimo: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("InnerException: " + ex.InnerException.Message);
                }
                
                // Lanzar la excepción para que el controlador la capture y devuelva al cliente
                throw;
            }
            
            return lista;
        }

        /// <summary>
        /// Obtiene el conteo de alertas por nivel
        /// </summary>
        public Dictionary<string, int> ObtenerConteoAlertas(int? sucursalID = null)
        {
            var resultado = new Dictionary<string, int>
            {
                { "AGOTADO", 0 },
                { "CRITICO", 0 },
                { "BAJO", 0 },
                { "TOTAL", 0 }
            };
            
            try
            {
                var alertas = ObtenerProductosBajoStockMinimo(sucursalID);
                
                resultado["AGOTADO"] = alertas.Where(a => a.NivelAlerta == "AGOTADO").Count();
                resultado["CRITICO"] = alertas.Where(a => a.NivelAlerta == "CRITICO").Count();
                resultado["BAJO"] = alertas.Where(a => a.NivelAlerta == "BAJO").Count();
                resultado["TOTAL"] = alertas.Count;
            }
            catch (Exception ex)
            {
                // Log error
            }
            
            return resultado;
        }
    }
}