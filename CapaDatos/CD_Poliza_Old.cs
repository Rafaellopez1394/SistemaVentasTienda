using CapaModelo;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    public class CD_Poliza
    {
        public static CD_Poliza Instancia { get; } = new CD_Poliza();

        public bool CrearPoliza(Poliza poliza)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Calcular totales
                        poliza.TotalDebe = poliza.Detalles.Sum(d => d.Debe);
                        poliza.TotalHaber = poliza.Detalles.Sum(d => d.Haber);
                        
                        // Validar cuadre contable
                        if (Math.Abs(poliza.TotalDebe - poliza.TotalHaber) >= 0.01m)
                        {
                            throw new Exception($"Póliza descuadrada: Debe={poliza.TotalDebe:C}, Haber={poliza.TotalHaber:C}");
                        }
                        
                        // Establecer periodo contable si no existe
                        if (string.IsNullOrEmpty(poliza.PeriodoContable))
                        {
                            poliza.PeriodoContable = poliza.FechaPoliza.ToString("yyyy-MM");
                        }
                        
                        // Insertar Póliza
                        Guid polizaId = Guid.NewGuid();
                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Polizas 
                            (PolizaID, TipoPoliza, FechaPoliza, Concepto, Referencia, Usuario, EsAutomatica, DocumentoOrigen, TotalDebe, TotalHaber, Estatus, PeriodoContable, RelacionCFDI, Observaciones) 
                            VALUES (@PolizaID, @TipoPoliza, @Fecha, @Concepto, @Referencia, @Usuario, @EsAutomatica, @DocumentoOrigen, @TotalDebe, @TotalHaber, @Estatus, @PeriodoContable, @RelacionCFDI, @Observaciones)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@PolizaID", polizaId);
                            cmd.Parameters.AddWithValue("@TipoPoliza", poliza.TipoPoliza);
                            cmd.Parameters.AddWithValue("@Fecha", poliza.FechaPoliza);
                            cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto);
                            cmd.Parameters.AddWithValue("@Referencia", poliza.Referencia ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Usuario", poliza.Usuario ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EsAutomatica", poliza.EsAutomatica);
                            cmd.Parameters.AddWithValue("@DocumentoOrigen", poliza.DocumentoOrigen ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TotalDebe", poliza.TotalDebe);
                            cmd.Parameters.AddWithValue("@TotalHaber", poliza.TotalHaber);
                            cmd.Parameters.AddWithValue("@Estatus", poliza.Estatus ?? "ABIERTA");
                            cmd.Parameters.AddWithValue("@PeriodoContable", poliza.PeriodoContable);
                            cmd.Parameters.AddWithValue("@RelacionCFDI", poliza.RelacionCFDI ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Observaciones", poliza.Observaciones ?? (object)DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        // Insertar Detalles
                        foreach (var detalle in poliza.Detalles)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO PolizasDetalle (PolizaId, CuentaId, Debe, Haber, Concepto) VALUES (@PolizaId, @CuentaId, @Debe, @Haber, @Concepto)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@PolizaId", polizaId);
                                cmd.Parameters.AddWithValue("@CuentaId", detalle.CuentaID);
                                cmd.Parameters.AddWithValue("@Debe", detalle.Debe);
                                cmd.Parameters.AddWithValue("@Haber", detalle.Haber);
                                cmd.Parameters.AddWithValue("@Concepto", detalle.Concepto);
                                cmd.ExecuteNonQuery();
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
        }

        public System.Collections.Generic.List<CapaModelo.PolizaResumen> ObtenerUltimas(int top = 100)
        {
            var lista = new System.Collections.Generic.List<CapaModelo.PolizaResumen>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TOP(@top) p.PolizaID, p.FechaPoliza, p.TipoPoliza, p.Concepto,
                           ISNULL(SUM(d.Debe),0) AS TotalDebe, ISNULL(SUM(d.Haber),0) AS TotalHaber
                    FROM Polizas p
                    LEFT JOIN PolizasDetalle d ON p.PolizaID = d.PolizaId
                    GROUP BY p.PolizaID, p.FechaPoliza, p.TipoPoliza, p.Concepto
                    ORDER BY p.FechaPoliza DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@top", top);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CapaModelo.PolizaResumen
                            {
                                PolizaID = dr["PolizaID"] == DBNull.Value ? Guid.Empty : Guid.Parse(dr["PolizaID"].ToString()),
                                FechaPoliza = dr["FechaPoliza"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["FechaPoliza"]),
                                TipoPoliza = dr["TipoPoliza"]?.ToString(),
                                Concepto = dr["Concepto"]?.ToString(),
                                TotalDebe = dr["TotalDebe"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalDebe"]),
                                TotalHaber = dr["TotalHaber"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalHaber"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // Inserta una póliza usando una conexión y transacción existentes (no hace commit/rollback)
        public bool CrearPoliza(Poliza poliza, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                // Calcular totales
                poliza.TotalDebe = poliza.Detalles.Sum(d => d.Debe);
                poliza.TotalHaber = poliza.Detalles.Sum(d => d.Haber);
                
                // Validar cuadre contable
                if (Math.Abs(poliza.TotalDebe - poliza.TotalHaber) >= 0.01m)
                {
                    throw new Exception($"Póliza descuadrada: Debe={poliza.TotalDebe:C}, Haber={poliza.TotalHaber:C}");
                }
                
                // Establecer periodo contable si no existe
                if (string.IsNullOrEmpty(poliza.PeriodoContable))
                {
                    poliza.PeriodoContable = poliza.FechaPoliza.ToString("yyyy-MM");
                }
                
                Guid polizaId = Guid.NewGuid();
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Polizas 
                    (PolizaID, TipoPoliza, FechaPoliza, Concepto, Referencia, Usuario, EsAutomatica, DocumentoOrigen, TotalDebe, TotalHaber, Estatus, PeriodoContable, RelacionCFDI, Observaciones) 
                    VALUES (@PolizaID, @TipoPoliza, @Fecha, @Concepto, @Referencia, @Usuario, @EsAutomatica, @DocumentoOrigen, @TotalDebe, @TotalHaber, @Estatus, @PeriodoContable, @RelacionCFDI, @Observaciones)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@PolizaID", polizaId);
                    cmd.Parameters.AddWithValue("@TipoPoliza", poliza.TipoPoliza);
                    cmd.Parameters.AddWithValue("@Fecha", poliza.FechaPoliza);
                    cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto);
                    cmd.Parameters.AddWithValue("@Referencia", poliza.Referencia ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", poliza.Usuario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EsAutomatica", poliza.EsAutomatica);
                    cmd.Parameters.AddWithValue("@DocumentoOrigen", poliza.DocumentoOrigen ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalDebe", poliza.TotalDebe);
                    cmd.Parameters.AddWithValue("@TotalHaber", poliza.TotalHaber);
                    cmd.Parameters.AddWithValue("@Estatus", poliza.Estatus ?? "ABIERTA");
                    cmd.Parameters.AddWithValue("@PeriodoContable", poliza.PeriodoContable);
                    cmd.Parameters.AddWithValue("@RelacionCFDI", poliza.RelacionCFDI ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", poliza.Observaciones ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                foreach (var detalle in poliza.Detalles)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO PolizasDetalle (PolizaId, CuentaId, Debe, Haber, Concepto) VALUES (@PolizaId, @CuentaId, @Debe, @Haber, @Concepto)", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@PolizaId", polizaId);
                        cmd.Parameters.AddWithValue("@CuentaId", detalle.CuentaID);
                        cmd.Parameters.AddWithValue("@Debe", detalle.Debe);
                        cmd.Parameters.AddWithValue("@Haber", detalle.Haber);
                        cmd.Parameters.AddWithValue("@Concepto", detalle.Concepto ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public System.Collections.Generic.List<CapaModelo.PolizaResumen> ObtenerFiltradas(string fechaInicio, string fechaFin, string tipoPoliza)
        {
            var lista = new System.Collections.Generic.List<CapaModelo.PolizaResumen>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT p.PolizaID, p.FechaPoliza, p.TipoPoliza, p.Concepto, p.Usuario, p.FechaAlta AS FechaRegistro,
                           ISNULL(SUM(d.Debe),0) AS TotalDebe, ISNULL(SUM(d.Haber),0) AS TotalHaber
                    FROM Polizas p
                    LEFT JOIN PolizasDetalle d ON p.PolizaID = d.PolizaId
                    WHERE 1=1";
                if (!string.IsNullOrEmpty(fechaInicio))
                    query += " AND p.FechaPoliza >= @FechaInicio";
                if (!string.IsNullOrEmpty(fechaFin))
                    query += " AND p.FechaPoliza <= @FechaFin";
                if (!string.IsNullOrEmpty(tipoPoliza))
                    query += " AND p.TipoPoliza = @TipoPoliza";

                query += " GROUP BY p.PolizaID, p.FechaPoliza, p.TipoPoliza, p.Concepto, p.Usuario, p.FechaAlta ORDER BY p.FechaPoliza DESC, p.PolizaID DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(fechaInicio))
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    if (!string.IsNullOrEmpty(fechaFin))
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    if (!string.IsNullOrEmpty(tipoPoliza))
                        cmd.Parameters.AddWithValue("@TipoPoliza", tipoPoliza);

                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CapaModelo.PolizaResumen
                            {
                                PolizaID = (Guid)dr["PolizaID"],
                                FechaPoliza = Convert.ToDateTime(dr["FechaPoliza"]),
                                TipoPoliza = dr["TipoPoliza"]?.ToString(),
                                Concepto = dr["Concepto"]?.ToString(),
                                Usuario = dr["Usuario"]?.ToString(),
                                FechaRegistro = dr["FechaRegistro"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaRegistro"]),
                                TotalDebe = Convert.ToDecimal(dr["TotalDebe"]),
                                TotalHaber = Convert.ToDecimal(dr["TotalHaber"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public Poliza ObtenerPorId(Guid polizaId)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = "SELECT PolizaID, TipoPoliza, FechaPoliza, Concepto, Referencia, Usuario, FechaAlta AS FechaRegistro FROM Polizas WHERE PolizaID = @PolizaId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PolizaId", polizaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Poliza
                            {
                                PolizaID = (Guid)dr["PolizaID"],
                                TipoPoliza = dr["TipoPoliza"]?.ToString(),
                                FechaPoliza = Convert.ToDateTime(dr["FechaPoliza"]),
                                Concepto = dr["Concepto"]?.ToString(),
                                Referencia = dr["Referencia"]?.ToString(),
                                Usuario = dr["Usuario"]?.ToString(),
                                FechaRegistro = dr["FechaRegistro"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaRegistro"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public System.Collections.Generic.List<PolizaDetalle> ObtenerDetalles(Guid polizaId)
        {
            var lista = new System.Collections.Generic.List<PolizaDetalle>();

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT pd.DetalleId, pd.PolizaId, pd.CuentaId, pd.Debe, pd.Haber, pd.Concepto,
                           c.Nombre AS NombreCuenta
                    FROM PolizasDetalle pd
                    LEFT JOIN CatalogoContable c ON pd.CuentaId = c.CuentaID
                    WHERE pd.PolizaId = @PolizaId
                    ORDER BY pd.DetalleId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PolizaId", polizaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PolizaDetalle
                            {
                                DetalleID = Convert.ToInt32(dr["DetalleId"]),
                                PolizaID = Guid.Parse(dr["PolizaId"].ToString()),
                                CuentaID = Convert.ToInt32(dr["CuentaId"]),
                                Debe = Convert.ToDecimal(dr["Debe"]),
                                Haber = Convert.ToDecimal(dr["Haber"]),
                                Concepto = dr["Concepto"]?.ToString(),
                                NombreCuenta = dr["NombreCuenta"]?.ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}