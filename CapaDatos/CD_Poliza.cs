using CapaModelo;
using System;
using System.Data;
using System.Data.SqlClient;

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
                        // Insertar Póliza
                        long polizaId;
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO PolizasContables (TipoPoliza, Fecha, Concepto, ReferenciaId, ReferenciaTipo) OUTPUT INSERTED.Id VALUES (@TipoPoliza, @Fecha, @Concepto, @ReferenciaId, @ReferenciaTipo)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@TipoPoliza", poliza.TipoPoliza);
                            cmd.Parameters.AddWithValue("@Fecha", poliza.FechaPoliza);
                            cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto);
                            cmd.Parameters.AddWithValue("@ReferenciaId", poliza.ReferenciaId ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ReferenciaTipo", poliza.ReferenciaTipo);
                            polizaId = (long)cmd.ExecuteScalar();
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
                    SELECT TOP(@top) p.Id AS PolizaID, p.Fecha AS FechaPoliza, p.TipoPoliza, p.Concepto,
                           ISNULL(SUM(d.Debe),0) AS TotalDebe, ISNULL(SUM(d.Haber),0) AS TotalHaber
                    FROM PolizasContables p
                    LEFT JOIN PolizasDetalle d ON p.Id = d.PolizaId
                    GROUP BY p.Id, p.Fecha, p.TipoPoliza, p.Concepto
                    ORDER BY p.Fecha DESC";

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
                                PolizaID = dr["PolizaID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PolizaID"]),
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
                long polizaId;
                using (SqlCommand cmd = new SqlCommand("INSERT INTO PolizasContables (TipoPoliza, Fecha, Concepto, ReferenciaId, ReferenciaTipo) OUTPUT INSERTED.Id VALUES (@TipoPoliza, @Fecha, @Concepto, @ReferenciaId, @ReferenciaTipo)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@TipoPoliza", poliza.TipoPoliza);
                    cmd.Parameters.AddWithValue("@Fecha", poliza.FechaPoliza);
                    cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto);
                    cmd.Parameters.AddWithValue("@ReferenciaId", poliza.ReferenciaId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReferenciaTipo", poliza.ReferenciaTipo ?? (object)DBNull.Value);
                    polizaId = (long)cmd.ExecuteScalar();
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
    }
}