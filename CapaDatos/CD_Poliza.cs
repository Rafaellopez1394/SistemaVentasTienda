using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    /// <summary>
    /// Capa de datos para Pólizas Contables
    /// Compatible con estructura Factoraje: Polizas + PolizasDetalle + AcvGral + AcvMov
    /// </summary>
    public class CD_Poliza
    {
        public static CD_Poliza Instancia { get; } = new CD_Poliza();

        /// <summary>
        /// Crea una póliza completa con sus detalles y registros contables
        /// </summary>
        public bool CrearPoliza(Poliza poliza, Guid? empresaID = null, string codEmpresa = "001")
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        bool resultado = CrearPoliza(poliza, conn, tran, empresaID, codEmpresa);
                        tran.Commit();
                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception("Error al crear póliza: " + ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Crea una póliza usando una conexión y transacción existente
        /// </summary>
        public bool CrearPoliza(Poliza poliza, SqlConnection conn, SqlTransaction tran, Guid? empresaID = null, string codEmpresa = "001")
        {
            // Generar IDs
            poliza.PolizaID = Guid.NewGuid();
            poliza.FechaAlta = DateTime.Now;
            poliza.Estatus = "VIGENTE"; // Activo
            
            // Calcular totales
            poliza.TotalDebe = poliza.Detalles.Sum(d => d.Debe);
            poliza.TotalHaber = poliza.Detalles.Sum(d => d.Haber);
            
            // Validar cuadre
            if (poliza.EsCuadrada != true && poliza.TotalDebe != poliza.TotalHaber)
            {
                throw new Exception(string.Format("Póliza descuadrada: Debe={0:C}, Haber={1:C}", poliza.TotalDebe, poliza.TotalHaber));
            }
            
            // Generar referencia si no existe
            if (string.IsNullOrEmpty(poliza.Referencia))
            {
                poliza.Referencia = string.Format("{0}-{1:yyyyMMddHHmmss}", poliza.TipoPoliza ?? "DI", DateTime.Now);
            }
            
            // 1. Insertar en tabla Polizas
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Polizas (PolizaID, FechaPoliza, TipoPoliza, Referencia, Concepto, Usuario, 
                                   FechaAlta, EsAutomatica, TotalDebe, TotalHaber, Estatus, EsCuadrada)
                VALUES (@PolizaID, @FechaPoliza, @TipoPoliza, @Referencia, @Concepto, 
                        @Usuario, @FechaAlta, @EsAutomatica, @TotalDebe, @TotalHaber, @Estatus, @EsCuadrada)", conn, tran))
            {
                cmd.Parameters.AddWithValue("@PolizaID", poliza.PolizaID);
                cmd.Parameters.AddWithValue("@FechaPoliza", poliza.FechaPoliza);
                cmd.Parameters.AddWithValue("@TipoPoliza", poliza.TipoPoliza ?? "DI");
                cmd.Parameters.AddWithValue("@Referencia", poliza.Referencia ?? "");
                cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto ?? "");
                cmd.Parameters.AddWithValue("@Usuario", poliza.Usuario ?? "system");
                cmd.Parameters.AddWithValue("@FechaAlta", DateTime.Now);
                cmd.Parameters.AddWithValue("@EsAutomatica", poliza.EsAutomatica);
                cmd.Parameters.AddWithValue("@TotalDebe", poliza.TotalDebe);
                cmd.Parameters.AddWithValue("@TotalHaber", poliza.TotalHaber);
                cmd.Parameters.AddWithValue("@Estatus", poliza.Estatus ?? "VIGENTE");
                cmd.Parameters.AddWithValue("@EsCuadrada", poliza.EsCuadrada);
                cmd.ExecuteNonQuery();
            }
            
            // 2. Insertar detalles en PolizasDetalle
            foreach (var detalle in poliza.Detalles)
            {
                detalle.PolizaID = poliza.PolizaID;
                
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO PolizasDetalle (DetalleID, PolizaID, CuentaID, Debe, Haber, Concepto)
                    VALUES (@DetalleID, @PolizaID, @CuentaID, @Debe, @Haber, @Concepto)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@DetalleID", detalle.DetalleID);
                    cmd.Parameters.AddWithValue("@PolizaID", detalle.PolizaID);
                    cmd.Parameters.AddWithValue("@CuentaID", (object)detalle.CuentaID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Debe", detalle.Debe);
                    cmd.Parameters.AddWithValue("@Haber", detalle.Haber);
                    cmd.Parameters.AddWithValue("@Concepto", detalle.Concepto ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            
            // 3. Generar asientos contables (AcvGral + AcvMov) - método deshabilitado
            // GenerarAsientosContables(conn, tran, poliza, codEmpresa);
            
            return true;
        }

        /// <summary>
        /// Alias para compatibilidad - Crea una póliza completa
        /// </summary>
        public bool CrearPolizaCompleta(Poliza poliza, Guid? empresaID = null, string codEmpresa = "001")
        {
            return CrearPoliza(poliza, empresaID, codEmpresa);
        }

        /// <summary>
        /// Genera folio automático para la póliza
        /// </summary>
        private string GenerarFolio(SqlConnection conn, SqlTransaction tran, string tipoPol, DateTime fecha)
        {
            string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(Referencia, CHARINDEX('-', Referencia) + 1, LEN(Referencia)) AS INT)), 0) + 1 
                FROM Polizas 
                WHERE TipoPoliza = @TipoPol AND YEAR(FechaPoliza) = @Anio";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@TipoPol", tipoPol);
                cmd.Parameters.AddWithValue("@Anio", fecha.Year);
                
                int folio = (int)cmd.ExecuteScalar();
                return folio.ToString().PadLeft(9, '0');
            }
        }

        /// <summary>
        /// Genera los asientos contables en AcvGral y AcvMov
        /// NOTA: Método deshabilitado - Tablas AcvGral/AcvMov son legacy y no existen en DB actual
        /// </summary>
        private void GenerarAsientosContables(SqlConnection conn, SqlTransaction tran, Poliza poliza, string codEmpresa)
        {
            // TODO: Implementar si se requiere integración con sistema contable legacy
            // Las tablas AcvGral y AcvMov no existen en la base de datos actual
            return;
            
            /*
            // Crear registro en AcvGral
            Guid acvGralID = Guid.NewGuid();
            string anoMes = poliza.FechaPol.ToString("yyyyMM");
            
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO AcvGral (AcvGralID, EmpresaID, Referencia_ID, Cod_Empresa, AnoMes, Tip_Pol, 
                                    Tipo_Mov, Num_Pol, Fec_Pol, Concepto, Importe, Usuario, Estatus, Fecha)
                VALUES (@AcvGralID, @EmpresaID, @Referencia_ID, @Cod_Empresa, @AnoMes, @Tip_Pol, 
                        @Tipo_Mov, @Num_Pol, @Fec_Pol, @Concepto, @Importe, @Usuario, @Estatus, @Fecha)", conn, tran))
            {
                cmd.Parameters.AddWithValue("@AcvGralID", acvGralID);
                cmd.Parameters.AddWithValue("@EmpresaID", (object)poliza.EmpresaID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Referencia_ID", poliza.PolizaID);
                cmd.Parameters.AddWithValue("@Cod_Empresa", codEmpresa);
                cmd.Parameters.AddWithValue("@AnoMes", anoMes);
                cmd.Parameters.AddWithValue("@Tip_Pol", poliza.Tip_Pol);
                cmd.Parameters.AddWithValue("@Tipo_Mov", 2); // 2 = Pólizas del sistema
                cmd.Parameters.AddWithValue("@Num_Pol", poliza.Folio);
                cmd.Parameters.AddWithValue("@Fec_Pol", poliza.FechaPol);
                cmd.Parameters.AddWithValue("@Concepto", poliza.Concepto);
                cmd.Parameters.AddWithValue("@Importe", poliza.Importe);
                cmd.Parameters.AddWithValue("@Usuario", poliza.Usuario);
                cmd.Parameters.AddWithValue("@Estatus", 1);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            
            // Crear movimientos en AcvMov
            int numRenglon = 0;
            foreach (var detalle in poliza.Detalles)
            {
                numRenglon++;
                
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO AcvMov (AcvMovID, EmpresaID, Cod_Empresa, AcvGralID, AnoMes, Fec_Pol, Tip_Pol, 
                                       Num_Pol, Num_Renglon, Tip_Mov, Cuenta, Concepto, Refer, Clase_Conta, 
                                       Importe, Tasa_Iva, Iva, Retencion_Iva, Pendiente, Cod_Flujo, Cod_Proveedor, 
                                       Fecha_Fiscal, CtaAux, Usuario, Estatus, Fecha)
                    VALUES (@AcvMovID, @EmpresaID, @Cod_Empresa, @AcvGralID, @AnoMes, @Fec_Pol, @Tip_Pol, 
                            @Num_Pol, @Num_Renglon, @Tip_Mov, @Cuenta, @Concepto, @Refer, @Clase_Conta, 
                            @Importe, @Tasa_Iva, @Iva, @Retencion_Iva, @Pendiente, @Cod_Flujo, @Cod_Proveedor, 
                            @Fecha_Fiscal, @CtaAux, @Usuario, @Estatus, @Fecha)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@AcvMovID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@EmpresaID", (object)poliza.EmpresaID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cod_Empresa", codEmpresa);
                    cmd.Parameters.AddWithValue("@AcvGralID", acvGralID);
                    cmd.Parameters.AddWithValue("@AnoMes", anoMes);
                    cmd.Parameters.AddWithValue("@Fec_Pol", poliza.FechaPol);
                    cmd.Parameters.AddWithValue("@Tip_Pol", poliza.Tip_Pol);
                    cmd.Parameters.AddWithValue("@Num_Pol", poliza.Folio);
                    cmd.Parameters.AddWithValue("@Num_Renglon", numRenglon);
                    cmd.Parameters.AddWithValue("@Tip_Mov", detalle.Tip_Mov);
                    cmd.Parameters.AddWithValue("@Cuenta", detalle.CodigoCuenta ?? "");
                    cmd.Parameters.AddWithValue("@Concepto", detalle.Concepto ?? "");
                    cmd.Parameters.AddWithValue("@Refer", poliza.Folio);
                    cmd.Parameters.AddWithValue("@Clase_Conta", "F"); // F = Fiscal
                    cmd.Parameters.AddWithValue("@Importe", detalle.Importe);
                    cmd.Parameters.AddWithValue("@Tasa_Iva", 0);
                    cmd.Parameters.AddWithValue("@Iva", 0);
                    cmd.Parameters.AddWithValue("@Retencion_Iva", 0);
                    cmd.Parameters.AddWithValue("@Pendiente", false);
                    cmd.Parameters.AddWithValue("@Cod_Flujo", "");
                    cmd.Parameters.AddWithValue("@Cod_Proveedor", "");
                    cmd.Parameters.AddWithValue("@Fecha_Fiscal", poliza.FechaPol);
                    cmd.Parameters.AddWithValue("@CtaAux", detalle.CodigoCuenta ?? "");
                    cmd.Parameters.AddWithValue("@Usuario", poliza.Usuario);
                    cmd.Parameters.AddWithValue("@Estatus", 1);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            */
        }

        /// <summary>
        /// Obtiene las últimas N pólizas
        /// </summary>
        public List<Poliza> ObtenerUltimas(int top = 100)
        {
            List<Poliza> lista = new List<Poliza>();
            
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = string.Format(@"
                    SELECT TOP {0} PolizaID, TipoPoliza, FechaPoliza, Referencia, Concepto, 
                           TotalDebe, TotalHaber, Estatus, FechaAlta, Usuario, EsAutomatica, EsCuadrada
                    FROM Polizas
                    ORDER BY FechaPoliza DESC, FechaAlta DESC", top);
                
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Poliza
                        {
                            PolizaID = (Guid)dr["PolizaID"],
                            TipoPoliza = dr["TipoPoliza"]?.ToString(),
                            FechaPoliza = (DateTime)dr["FechaPoliza"],
                            Referencia = dr["Referencia"]?.ToString(),
                            Concepto = dr["Concepto"]?.ToString(),
                            TotalDebe = (decimal)dr["TotalDebe"],
                            TotalHaber = (decimal)dr["TotalHaber"],
                            Estatus = dr["Estatus"]?.ToString(),
                            FechaAlta = (DateTime)dr["FechaAlta"],
                            Usuario = dr["Usuario"].ToString(),
                            EsAutomatica = dr["EsAutomatica"] != DBNull.Value && (bool)dr["EsAutomatica"],
                            EsCuadrada = dr["EsCuadrada"] != DBNull.Value ? (bool?)dr["EsCuadrada"] : null
                        });
                    }
                }
            }
            
            return lista;
        }

        /// <summary>
        /// Obtiene pólizas filtradas (sobrecarga que acepta strings)
        /// </summary>
        public List<Poliza> ObtenerFiltradas(string fechaInicio = null, string fechaFin = null, string tipoPoliza = null)
        {
            DateTime? dtInicio = null;
            DateTime? dtFin = null;
            
            if (!string.IsNullOrEmpty(fechaInicio) && DateTime.TryParse(fechaInicio, out DateTime tempInicio))
                dtInicio = tempInicio;
            
            if (!string.IsNullOrEmpty(fechaFin) && DateTime.TryParse(fechaFin, out DateTime tempFin))
                dtFin = tempFin;
            
            return ObtenerFiltradas(dtInicio, dtFin, tipoPoliza);
        }

        /// <summary>
        /// Obtiene pólizas filtradas
        /// </summary>
        public List<Poliza> ObtenerFiltradas(DateTime? fechaInicio = null, DateTime? fechaFin = null, string tipoPoliza = null)
        {
            List<Poliza> lista = new List<Poliza>();
            
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT PolizaID, TipoPoliza, FechaPoliza, Referencia, Concepto, 
                           TotalDebe, TotalHaber, Estatus, FechaAlta, Usuario, EsAutomatica, EsCuadrada
                    FROM Polizas 
                    WHERE 1=1";
                
                if (fechaInicio.HasValue)
                    query += " AND FechaPoliza >= @FechaInicio";
                if (fechaFin.HasValue)
                    query += " AND FechaPoliza <= @FechaFin";
                if (!string.IsNullOrEmpty(tipoPoliza))
                    query += " AND TipoPoliza = @TipoPoliza";
                    
                query += " ORDER BY FechaPoliza DESC, Referencia DESC";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                if (fechaInicio.HasValue) cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                if (fechaFin.HasValue) cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);
                if (!string.IsNullOrEmpty(tipoPoliza)) cmd.Parameters.AddWithValue("@TipoPoliza", tipoPoliza);
                
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Poliza
                        {
                            PolizaID = (Guid)dr["PolizaID"],
                            TipoPoliza = dr["TipoPoliza"]?.ToString(),
                            FechaPoliza = (DateTime)dr["FechaPoliza"],
                            Referencia = dr["Referencia"]?.ToString(),
                            Concepto = dr["Concepto"]?.ToString(),
                            TotalDebe = (decimal)dr["TotalDebe"],
                            TotalHaber = (decimal)dr["TotalHaber"],
                            Estatus = dr["Estatus"]?.ToString(),
                            FechaAlta = (DateTime)dr["FechaAlta"],
                            Usuario = dr["Usuario"].ToString(),
                            EsAutomatica = dr["EsAutomatica"] != DBNull.Value && (bool)dr["EsAutomatica"],
                            EsCuadrada = dr["EsCuadrada"] != DBNull.Value ? (bool?)dr["EsCuadrada"] : null
                        });
                    }
                }
            }
            
            // Cargar detalles para cada póliza
            foreach (var poliza in lista)
            {
                poliza.Detalles = ObtenerDetalles(poliza.PolizaID);
                poliza.TotalDebe = poliza.Detalles.Sum(d => d.Debe);
                poliza.TotalHaber = poliza.Detalles.Sum(d => d.Haber);
            }
            
            return lista;
        }

        /// <summary>
        /// Obtiene los detalles de una póliza
        /// </summary>
        public List<PolizaDetalle> ObtenerDetalles(Guid polizaID)
        {
            List<PolizaDetalle> lista = new List<PolizaDetalle>();
            
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT pd.DetalleID, pd.PolizaID, pd.CuentaID, pd.Concepto, 
                           pd.Debe, pd.Haber,
                           cc.CodigoCuenta, cc.NombreCuenta
                    FROM PolizasDetalle pd
                    LEFT JOIN CatCuentasContables cc ON pd.CuentaID = cc.CuentaID
                    WHERE pd.PolizaID = @PolizaID
                    ORDER BY pd.DetalleID";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolizaID", polizaID);
                
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new PolizaDetalle
                        {
                            DetalleID = (int)dr["DetalleID"],
                            PolizaID = (Guid)dr["PolizaID"],
                            CuentaID = dr["CuentaID"] != DBNull.Value ? (Guid?)dr["CuentaID"] : null,
                            Concepto = dr["Concepto"]?.ToString(),
                            Debe = (decimal)dr["Debe"],
                            Haber = (decimal)dr["Haber"],
                            CodigoCuenta = dr["CodigoCuenta"]?.ToString(),
                            NombreCuenta = dr["NombreCuenta"]?.ToString()
                        });
                    }
                }
            }
            
            return lista;
        }

        /// <summary>
        /// Obtiene una póliza por ID con sus detalles
        /// </summary>
        public Poliza ObtenerPorId(Guid polizaID)
        {
            Poliza poliza = null;
            
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT PolizaID, TipoPoliza, FechaPoliza, Referencia, Concepto, 
                           TotalDebe, TotalHaber, Estatus, FechaAlta, Usuario, EsAutomatica, EsCuadrada
                    FROM Polizas 
                    WHERE PolizaID = @PolizaID";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolizaID", polizaID);
                
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        poliza = new Poliza
                        {
                            PolizaID = (Guid)dr["PolizaID"],
                            TipoPoliza = dr["TipoPoliza"]?.ToString(),
                            FechaPoliza = (DateTime)dr["FechaPoliza"],
                            Referencia = dr["Referencia"]?.ToString(),
                            Concepto = dr["Concepto"]?.ToString(),
                            TotalDebe = (decimal)dr["TotalDebe"],
                            TotalHaber = (decimal)dr["TotalHaber"],
                            Estatus = dr["Estatus"]?.ToString(),
                            FechaAlta = (DateTime)dr["FechaAlta"],
                            Usuario = dr["Usuario"].ToString(),
                            EsAutomatica = dr["EsAutomatica"] != DBNull.Value && (bool)dr["EsAutomatica"],
                            EsCuadrada = dr["EsCuadrada"] != DBNull.Value && (bool)dr["EsCuadrada"]
                        };
                    }
                }
            }
            
            if (poliza != null)
            {
                poliza.Detalles = ObtenerDetalles(poliza.PolizaID);
                poliza.TotalDebe = poliza.Detalles.Sum(d => d.Debe);
                poliza.TotalHaber = poliza.Detalles.Sum(d => d.Haber);
            }
            
            return poliza;
        }
    }
}
