using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    public class CD_Nomina
    {
        public static CD_Nomina Instancia = new CD_Nomina();

        // =============================================
        // MÉTODOS DE CONSULTA
        // =============================================
        
        public List<Nomina> ObtenerTodas()
        {
            var lista = new List<Nomina>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT * FROM Nominas ORDER BY FechaPago DESC, Folio DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(MapearNomina(dr));
                        }
                    }
                }
            }
            return lista;
        }

        public List<Nomina> ObtenerPorPeriodo(string periodo)
        {
            var lista = new List<Nomina>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT * FROM Nominas WHERE Periodo = @Periodo ORDER BY FechaPago DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Periodo", periodo);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(MapearNomina(dr));
                        }
                    }
                }
            }
            return lista;
        }

        public Nomina ObtenerPorId(int nominaId)
        {
            Nomina nomina = null;
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT * FROM Nominas WHERE NominaID = @NominaID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaID", nominaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            nomina = MapearNomina(dr);
                        }
                    }
                }
                
                if (nomina != null)
                {
                    nomina.Recibos = ObtenerRecibos(nominaId);
                }
            }
            return nomina;
        }

        public List<NominaDetalle> ObtenerRecibos(int nominaId)
        {
            var lista = new List<NominaDetalle>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT nd.*, e.NumeroEmpleado, 
                                CONCAT(e.Nombre, ' ', e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno, '')) AS NombreEmpleado,
                                e.RFC, e.NSS, e.Puesto
                                FROM NominaDetalle nd
                                INNER JOIN Empleados e ON nd.EmpleadoID = e.EmpleadoID
                                WHERE nd.NominaID = @NominaID
                                ORDER BY e.NumeroEmpleado";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaID", nominaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var detalle = MapearNominaDetalle(dr);
                            lista.Add(detalle);
                        }
                    }
                }
                
                // Cargar percepciones y deducciones de cada recibo
                foreach (var recibo in lista)
                {
                    recibo.Percepciones = ObtenerPercepciones(recibo.NominaDetalleID);
                    recibo.Deducciones = ObtenerDeducciones(recibo.NominaDetalleID);
                }
            }
            return lista;
        }

        public List<NominaPercepcion> ObtenerPercepciones(int nominaDetalleId)
        {
            var lista = new List<NominaPercepcion>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT * FROM NominaPercepciones WHERE NominaDetalleID = @NominaDetalleID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaDetalleID", nominaDetalleId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new NominaPercepcion
                            {
                                NominaPercepcionID = Convert.ToInt32(dr["NominaPercepcionID"]),
                                NominaDetalleID = Convert.ToInt32(dr["NominaDetalleID"]),
                                PercepcionID = Convert.ToInt32(dr["PercepcionID"]),
                                Clave = dr["Clave"]?.ToString(),
                                Concepto = dr["Concepto"]?.ToString(),
                                TipoPercepcion = dr["TipoPercepcion"]?.ToString(),
                                ImporteGravado = Convert.ToDecimal(dr["ImporteGravado"]),
                                ImporteExento = Convert.ToDecimal(dr["ImporteExento"]),
                                ImporteTotal = Convert.ToDecimal(dr["ImporteTotal"]),
                                FechaAlta = Convert.ToDateTime(dr["FechaAlta"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<NominaDeduccion> ObtenerDeducciones(int nominaDetalleId)
        {
            var lista = new List<NominaDeduccion>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT * FROM NominaDeducciones WHERE NominaDetalleID = @NominaDetalleID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaDetalleID", nominaDetalleId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new NominaDeduccion
                            {
                                NominaDeduccionID = Convert.ToInt32(dr["NominaDeduccionID"]),
                                NominaDetalleID = Convert.ToInt32(dr["NominaDetalleID"]),
                                DeduccionID = Convert.ToInt32(dr["DeduccionID"]),
                                Clave = dr["Clave"]?.ToString(),
                                Concepto = dr["Concepto"]?.ToString(),
                                TipoDeduccion = dr["TipoDeduccion"]?.ToString(),
                                Importe = Convert.ToDecimal(dr["Importe"]),
                                FechaAlta = Convert.ToDateTime(dr["FechaAlta"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // =============================================
        // MÉTODOS DE CÁLCULO DE NÓMINA
        // =============================================
        
        public Nomina CalcularNomina(DateTime fechaInicio, DateTime fechaFin, DateTime fechaPago, string tipoNomina, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Crear encabezado de nómina
                        var nomina = new Nomina
                        {
                            Folio = GenerarFolio(fechaPago, conn, tran),
                            Periodo = GenerarPeriodo(fechaInicio, fechaFin),
                            FechaInicio = fechaInicio,
                            FechaFin = fechaFin,
                            FechaPago = fechaPago,
                            TipoNomina = tipoNomina,
                            Estatus = "CALCULADA",
                            Usuario = usuario
                        };

                        int nominaId = InsertarNomina(nomina, conn, tran);
                        nomina.NominaID = nominaId;

                        // Obtener empleados activos
                        var empleados = CD_Empleado.Instancia.ObtenerActivos();
                        
                        decimal totalPercepciones = 0;
                        decimal totalDeducciones = 0;
                        int numEmpleados = 0;

                        foreach (var empleado in empleados)
                        {
                            var recibo = CalcularReciboEmpleado(empleado, fechaInicio, fechaFin, conn, tran);
                            recibo.NominaID = nominaId;
                            
                            int reciboId = InsertarReciboNomina(recibo, conn, tran);
                            recibo.NominaDetalleID = reciboId;

                            // Guardar percepciones
                            foreach (var percepcion in recibo.Percepciones)
                            {
                                percepcion.NominaDetalleID = reciboId;
                                InsertarPercepcion(percepcion, conn, tran);
                            }

                            // Guardar deducciones
                            foreach (var deduccion in recibo.Deducciones)
                            {
                                deduccion.NominaDetalleID = reciboId;
                                InsertarDeduccion(deduccion, conn, tran);
                            }

                            totalPercepciones += recibo.TotalPercepciones;
                            totalDeducciones += recibo.TotalDeducciones;
                            numEmpleados++;
                        }

                        // Actualizar totales del encabezado
                        ActualizarTotalesNomina(nominaId, totalPercepciones, totalDeducciones, numEmpleados, conn, tran);
                        
                        nomina.TotalPercepciones = totalPercepciones;
                        nomina.TotalDeducciones = totalDeducciones;
                        nomina.TotalNeto = totalPercepciones - totalDeducciones;
                        nomina.NumeroEmpleados = numEmpleados;

                        tran.Commit();
                        return nomina;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception($"Error al calcular nómina: {ex.Message}", ex);
                    }
                }
            }
        }

        private NominaDetalle CalcularReciboEmpleado(Empleado empleado, DateTime fechaInicio, DateTime fechaFin, SqlConnection conn, SqlTransaction tran)
        {
            var recibo = new NominaDetalle
            {
                EmpleadoID = empleado.EmpleadoID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                SalarioDiario = empleado.SalarioDiario,
                Estatus = "CALCULADO"
            };

            // Calcular días trabajados (por ahora asume periodo completo)
            TimeSpan diferencia = fechaFin - fechaInicio;
            recibo.DiasTrabajados = diferencia.Days + 1;

            // Calcular salario base
            recibo.SalarioBase = recibo.SalarioDiario * recibo.DiasTrabajados;

            // PERCEPCIONES
            // 001 - Sueldo base
            var percepcionSueldo = new NominaPercepcion
            {
                PercepcionID = 1, // Sueldos, Salarios, Rayas y Jornales
                Clave = "001",
                Concepto = "Sueldo Base",
                TipoPercepcion = "FIJA",
                ImporteGravado = recibo.SalarioBase,
                ImporteExento = 0,
                ImporteTotal = recibo.SalarioBase
            };
            recibo.Percepciones.Add(percepcionSueldo);

            // Calcular totales de percepciones
            recibo.TotalPercepciones = recibo.Percepciones.Sum(p => p.ImporteTotal);
            recibo.TotalPercepcionesGravadas = recibo.Percepciones.Sum(p => p.ImporteGravado);
            recibo.TotalPercepcionesExentas = recibo.Percepciones.Sum(p => p.ImporteExento);

            // DEDUCCIONES
            // Calcular ISR
            decimal isr = CalcularISR(recibo.TotalPercepcionesGravadas, empleado.PeriodicidadPago, conn, tran);
            if (isr > 0)
            {
                recibo.Deducciones.Add(new NominaDeduccion
                {
                    DeduccionID = 2, // ISR
                    Clave = "002",
                    Concepto = "ISR",
                    TipoDeduccion = "LEGAL",
                    Importe = isr
                });
            }

            // Calcular IMSS (aproximado - 2.375% del SBC)
            if (!string.IsNullOrEmpty(empleado.NSS))
            {
                decimal imss = empleado.SalarioDiarioIntegrado.HasValue 
                    ? empleado.SalarioDiarioIntegrado.Value * recibo.DiasTrabajados * 0.02375m 
                    : recibo.SalarioBase * 0.02375m;
                
                recibo.Deducciones.Add(new NominaDeduccion
                {
                    DeduccionID = 1, // Seguridad Social
                    Clave = "001",
                    Concepto = "IMSS (Cuota Obrera)",
                    TipoDeduccion = "LEGAL",
                    Importe = Math.Round(imss, 2)
                });
            }

            // Calcular totales de deducciones
            recibo.TotalDeducciones = recibo.Deducciones.Sum(d => d.Importe);
            recibo.TotalImpuestosRetenidos = recibo.Deducciones.Where(d => d.Clave == "002").Sum(d => d.Importe);
            recibo.TotalOtrasDeducciones = recibo.TotalDeducciones - recibo.TotalImpuestosRetenidos;

            // Neto a pagar
            recibo.NetoPagar = recibo.TotalPercepciones - recibo.TotalDeducciones;

            return recibo;
        }

        private decimal CalcularISR(decimal baseGravable, string periodicidad, SqlConnection conn, SqlTransaction tran)
        {
            // Ajustar base según periodicidad
            decimal baseMensual = baseGravable;
            if (periodicidad == "QUINCENAL")
                baseMensual = baseGravable * 2;
            else if (periodicidad == "SEMANAL")
                baseMensual = baseGravable * 4.33m;

            // Buscar en tabla ISR
            string query = @"SELECT TOP 1 * FROM TablaISR 
                            WHERE @BaseGravable BETWEEN LimiteInferior AND LimiteSuperior 
                            AND Periodicidad = 'MENSUAL' AND Activo = 1 
                            ORDER BY LimiteInferior DESC";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@BaseGravable", baseMensual);
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        decimal limiteInferior = Convert.ToDecimal(dr["LimiteInferior"]);
                        decimal cuotaFija = Convert.ToDecimal(dr["CuotaFija"]);
                        decimal porcentajeExcedente = Convert.ToDecimal(dr["PorcentajeExcedente"]);

                        decimal excedente = baseMensual - limiteInferior;
                        decimal impuestoExcedente = excedente * (porcentajeExcedente / 100);
                        decimal isrMensual = cuotaFija + impuestoExcedente;

                        // Calcular subsidio al empleo
                        dr.Close();
                        decimal subsidio = CalcularSubsidioEmpleo(baseMensual, conn, tran);
                        
                        decimal isrNeto = isrMensual - subsidio;
                        
                        // Ajustar según periodicidad
                        if (periodicidad == "QUINCENAL")
                            isrNeto = isrNeto / 2;
                        else if (periodicidad == "SEMANAL")
                            isrNeto = isrNeto / 4.33m;

                        return Math.Max(0, Math.Round(isrNeto, 2));
                    }
                }
            }
            return 0;
        }

        private decimal CalcularSubsidioEmpleo(decimal baseMensual, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"SELECT TOP 1 MontoSubsidio FROM TablaSubsidioEmpleo 
                            WHERE @BaseGravable BETWEEN LimiteInferior AND LimiteSuperior 
                            AND Periodicidad = 'MENSUAL' AND Activo = 1 
                            ORDER BY LimiteInferior DESC";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@BaseGravable", baseMensual);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }

        // =============================================
        // MÉTODOS AUXILIARES
        // =============================================

        private string GenerarFolio(DateTime fecha, SqlConnection conn, SqlTransaction tran)
        {
            string query = "SELECT COUNT(*) FROM Nominas WHERE YEAR(FechaPago) = @Anio";
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@Anio", fecha.Year);
                int consecutivo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                return $"NOM-{fecha:yyyy}-{consecutivo:D4}";
            }
        }

        private string GenerarPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            // Formato: 2025-01-Q1 (primera quincena), 2025-01-Q2 (segunda quincena)
            if (fechaInicio.Day <= 15)
                return $"{fechaInicio:yyyy-MM}-Q1";
            else
                return $"{fechaInicio:yyyy-MM}-Q2";
        }

        private int InsertarNomina(Nomina nomina, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Nominas 
                (Folio, Periodo, FechaInicio, FechaFin, FechaPago, TipoNomina, Estatus, Usuario)
                VALUES (@Folio, @Periodo, @FechaInicio, @FechaFin, @FechaPago, @TipoNomina, @Estatus, @Usuario);
                SELECT SCOPE_IDENTITY()";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@Folio", nomina.Folio);
                cmd.Parameters.AddWithValue("@Periodo", nomina.Periodo);
                cmd.Parameters.AddWithValue("@FechaInicio", nomina.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", nomina.FechaFin);
                cmd.Parameters.AddWithValue("@FechaPago", nomina.FechaPago);
                cmd.Parameters.AddWithValue("@TipoNomina", nomina.TipoNomina);
                cmd.Parameters.AddWithValue("@Estatus", nomina.Estatus);
                cmd.Parameters.AddWithValue("@Usuario", nomina.Usuario);
                
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int InsertarReciboNomina(NominaDetalle recibo, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO NominaDetalle 
                (NominaID, EmpleadoID, DiasTrabajados, FechaInicio, FechaFin, SalarioDiario, SalarioBase,
                 TotalPercepciones, TotalPercepcionesGravadas, TotalPercepcionesExentas,
                 TotalDeducciones, TotalImpuestosRetenidos, TotalOtrasDeducciones, NetoPagar, Estatus)
                VALUES 
                (@NominaID, @EmpleadoID, @DiasTrabajados, @FechaInicio, @FechaFin, @SalarioDiario, @SalarioBase,
                 @TotalPercepciones, @TotalPercepcionesGravadas, @TotalPercepcionesExentas,
                 @TotalDeducciones, @TotalImpuestosRetenidos, @TotalOtrasDeducciones, @NetoPagar, @Estatus);
                SELECT SCOPE_IDENTITY()";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@NominaID", recibo.NominaID);
                cmd.Parameters.AddWithValue("@EmpleadoID", recibo.EmpleadoID);
                cmd.Parameters.AddWithValue("@DiasTrabajados", recibo.DiasTrabajados);
                cmd.Parameters.AddWithValue("@FechaInicio", recibo.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", recibo.FechaFin);
                cmd.Parameters.AddWithValue("@SalarioDiario", recibo.SalarioDiario);
                cmd.Parameters.AddWithValue("@SalarioBase", recibo.SalarioBase);
                cmd.Parameters.AddWithValue("@TotalPercepciones", recibo.TotalPercepciones);
                cmd.Parameters.AddWithValue("@TotalPercepcionesGravadas", recibo.TotalPercepcionesGravadas);
                cmd.Parameters.AddWithValue("@TotalPercepcionesExentas", recibo.TotalPercepcionesExentas);
                cmd.Parameters.AddWithValue("@TotalDeducciones", recibo.TotalDeducciones);
                cmd.Parameters.AddWithValue("@TotalImpuestosRetenidos", recibo.TotalImpuestosRetenidos);
                cmd.Parameters.AddWithValue("@TotalOtrasDeducciones", recibo.TotalOtrasDeducciones);
                cmd.Parameters.AddWithValue("@NetoPagar", recibo.NetoPagar);
                cmd.Parameters.AddWithValue("@Estatus", recibo.Estatus);
                
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void InsertarPercepcion(NominaPercepcion percepcion, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO NominaPercepciones 
                (NominaDetalleID, PercepcionID, Clave, Concepto, TipoPercepcion, ImporteGravado, ImporteExento, ImporteTotal)
                VALUES (@NominaDetalleID, @PercepcionID, @Clave, @Concepto, @TipoPercepcion, @ImporteGravado, @ImporteExento, @ImporteTotal)";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@NominaDetalleID", percepcion.NominaDetalleID);
                cmd.Parameters.AddWithValue("@PercepcionID", percepcion.PercepcionID);
                cmd.Parameters.AddWithValue("@Clave", percepcion.Clave);
                cmd.Parameters.AddWithValue("@Concepto", percepcion.Concepto);
                cmd.Parameters.AddWithValue("@TipoPercepcion", percepcion.TipoPercepcion);
                cmd.Parameters.AddWithValue("@ImporteGravado", percepcion.ImporteGravado);
                cmd.Parameters.AddWithValue("@ImporteExento", percepcion.ImporteExento);
                cmd.Parameters.AddWithValue("@ImporteTotal", percepcion.ImporteTotal);
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarDeduccion(NominaDeduccion deduccion, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO NominaDeducciones 
                (NominaDetalleID, DeduccionID, Clave, Concepto, TipoDeduccion, Importe)
                VALUES (@NominaDetalleID, @DeduccionID, @Clave, @Concepto, @TipoDeduccion, @Importe)";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@NominaDetalleID", deduccion.NominaDetalleID);
                cmd.Parameters.AddWithValue("@DeduccionID", deduccion.DeduccionID);
                cmd.Parameters.AddWithValue("@Clave", deduccion.Clave);
                cmd.Parameters.AddWithValue("@Concepto", deduccion.Concepto);
                cmd.Parameters.AddWithValue("@TipoDeduccion", deduccion.TipoDeduccion);
                cmd.Parameters.AddWithValue("@Importe", deduccion.Importe);
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarTotalesNomina(int nominaId, decimal totalPercepciones, decimal totalDeducciones, int numEmpleados, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Nominas SET 
                TotalPercepciones = @TotalPercepciones, 
                TotalDeducciones = @TotalDeducciones, 
                TotalNeto = @TotalNeto, 
                NumeroEmpleados = @NumeroEmpleados
                WHERE NominaID = @NominaID";
            
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@NominaID", nominaId);
                cmd.Parameters.AddWithValue("@TotalPercepciones", totalPercepciones);
                cmd.Parameters.AddWithValue("@TotalDeducciones", totalDeducciones);
                cmd.Parameters.AddWithValue("@TotalNeto", totalPercepciones - totalDeducciones);
                cmd.Parameters.AddWithValue("@NumeroEmpleados", numEmpleados);
                cmd.ExecuteNonQuery();
            }
        }

        private Nomina MapearNomina(SqlDataReader dr)
        {
            return new Nomina
            {
                NominaID = Convert.ToInt32(dr["NominaID"]),
                Folio = dr["Folio"]?.ToString(),
                Periodo = dr["Periodo"]?.ToString(),
                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                TipoNomina = dr["TipoNomina"]?.ToString(),
                TotalPercepciones = Convert.ToDecimal(dr["TotalPercepciones"]),
                TotalDeducciones = Convert.ToDecimal(dr["TotalDeducciones"]),
                TotalNeto = Convert.ToDecimal(dr["TotalNeto"]),
                NumeroEmpleados = Convert.ToInt32(dr["NumeroEmpleados"]),
                Estatus = dr["Estatus"]?.ToString(),
                FechaCalculo = Convert.ToDateTime(dr["FechaCalculo"]),
                FechaTimbrado = dr["FechaTimbrado"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaTimbrado"]),
                FechaPagado = dr["FechaPagado"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaPagado"]),
                FechaContabilizada = dr["FechaContabilizada"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaContabilizada"]),
                PolizaID = dr["PolizaID"] == DBNull.Value ? (Guid?)null : (Guid)dr["PolizaID"],
                Usuario = dr["Usuario"]?.ToString(),
                FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
            };
        }

        private NominaDetalle MapearNominaDetalle(SqlDataReader dr)
        {
            return new NominaDetalle
            {
                NominaDetalleID = Convert.ToInt32(dr["NominaDetalleID"]),
                NominaID = Convert.ToInt32(dr["NominaID"]),
                EmpleadoID = Convert.ToInt32(dr["EmpleadoID"]),
                NumeroEmpleado = dr["NumeroEmpleado"]?.ToString(),
                NombreEmpleado = dr["NombreEmpleado"]?.ToString(),
                RFC = dr["RFC"]?.ToString(),
                NSS = dr["NSS"]?.ToString(),
                Puesto = dr["Puesto"]?.ToString(),
                DiasTrabajados = Convert.ToDecimal(dr["DiasTrabajados"]),
                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                SalarioDiario = Convert.ToDecimal(dr["SalarioDiario"]),
                SalarioBase = Convert.ToDecimal(dr["SalarioBase"]),
                TotalPercepciones = Convert.ToDecimal(dr["TotalPercepciones"]),
                TotalPercepcionesGravadas = Convert.ToDecimal(dr["TotalPercepcionesGravadas"]),
                TotalPercepcionesExentas = Convert.ToDecimal(dr["TotalPercepcionesExentas"]),
                TotalDeducciones = Convert.ToDecimal(dr["TotalDeducciones"]),
                TotalImpuestosRetenidos = Convert.ToDecimal(dr["TotalImpuestosRetenidos"]),
                TotalOtrasDeducciones = Convert.ToDecimal(dr["TotalOtrasDeducciones"]),
                NetoPagar = Convert.ToDecimal(dr["NetoPagar"]),
                UUID = dr["UUID"]?.ToString(),
                FechaTimbrado = dr["FechaTimbrado"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaTimbrado"]),
                Estatus = dr["Estatus"]?.ToString(),
                FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
            };
        }

        // =============================================
        // GENERACIÓN DE PÓLIZA CONTABLE
        // =============================================
        
        public bool GenerarPolizaNomina(int nominaId, string usuario)
        {
            var nomina = ObtenerPorId(nominaId);
            if (nomina == null || nomina.PolizaID.HasValue)
                return false;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        var poliza = new Poliza
                        {
                            TipoPoliza = "EGRESO",
                            FechaPoliza = nomina.FechaPago,
                            Concepto = $"Nómina {nomina.Periodo} - {nomina.Folio}",
                            Referencia = $"NOMINA-{nomina.NominaID}",
                            DocumentoOrigen = nomina.Folio,
                            Usuario = usuario,
                            EsAutomatica = true,
                            PeriodoContable = nomina.FechaPago.ToString("yyyy-MM")
                        };

                        // DEBE: Sueldos y Salarios (cuenta 5101)
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaContable("5101"), // Sueldos y Salarios
                            Debe = nomina.TotalPercepciones,
                            Haber = 0,
                            Concepto = $"Sueldos {nomina.NumeroEmpleados} empleados"
                        });

                        // DEBE: Cuotas Patronales IMSS (cuenta 5201 - aproximado 7% del total)
                        decimal cuotasPatronales = Math.Round(nomina.TotalPercepciones * 0.07m, 2);
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaContable("5201"), // Cuotas Patronales
                            Debe = cuotasPatronales,
                            Haber = 0,
                            Concepto = "Cuotas patronales IMSS"
                        });

                        // HABER: ISR Retenido (cuenta 2106)
                        decimal totalISR = nomina.Recibos.Sum(r => r.TotalImpuestosRetenidos);
                        if (totalISR > 0)
                        {
                            poliza.Detalles.Add(new PolizaDetalle
                            {
                                CuentaID = ObtenerCuentaContable("2106"), // ISR por Pagar
                                Debe = 0,
                                Haber = totalISR,
                                Concepto = "ISR retenido empleados"
                            });
                        }

                        // HABER: IMSS Obrero (cuenta 2107)
                        decimal totalIMSS = nomina.Recibos.Sum(r => r.TotalOtrasDeducciones);
                        if (totalIMSS > 0)
                        {
                            poliza.Detalles.Add(new PolizaDetalle
                            {
                                CuentaID = ObtenerCuentaContable("2107"), // IMSS por Pagar
                                Debe = 0,
                                Haber = totalIMSS,
                                Concepto = "IMSS cuota obrera"
                            });
                        }

                        // HABER: Bancos (cuenta 1020 - neto a pagar)
                        poliza.Detalles.Add(new PolizaDetalle
                        {
                            CuentaID = ObtenerCuentaContable("1020"), // Bancos
                            Debe = 0,
                            Haber = nomina.TotalNeto,
                            Concepto = "Pago de nómina vía transferencia"
                        });

                        // Crear póliza contable
                        bool polizaCreada = CD_Poliza.Instancia.CrearPoliza(poliza, conn, tran);
                        if (!polizaCreada)
                            throw new Exception("Error al crear póliza contable");

                        // Actualizar nómina con PolizaID y estatus
                        string updateQuery = @"UPDATE Nominas SET 
                            PolizaID = @PolizaID, 
                            Estatus = 'CONTABILIZADA', 
                            FechaContabilizada = GETDATE() 
                            WHERE NominaID = @NominaID";
                        
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@PolizaID", poliza.PolizaID);
                            cmd.Parameters.AddWithValue("@NominaID", nominaId);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception($"Error al generar póliza de nómina: {ex.Message}", ex);
                    }
                }
            }
        }

        private Guid? ObtenerCuentaContable(string codigoCuenta)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = "SELECT CuentaID FROM CatalogoContable WHERE CodigoCuenta = @Codigo AND Activo = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigoCuenta);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        throw new Exception($"Cuenta contable {codigoCuenta} no encontrada");
                    // Convertir int a Guid
                    int id = Convert.ToInt32(result);
                    byte[] bytes = new byte[16];
                    BitConverter.GetBytes(id).CopyTo(bytes, 0);
                    return new Guid(bytes);
                }
            }
        }

        // =============================================
        // TIMBRADO DE CFDI NÓMINA CON FISCALAPI
        // =============================================

        /// <summary>
        /// Timbra un recibo de nómina individual (por empleado) usando FiscalAPI
        /// Genera CFDI 4.0 con Complemento de Nómina 1.2
        /// </summary>
        public async System.Threading.Tasks.Task<RespuestaTimbrado> TimbrarCFDINomina(int nominaDetalleID, string usuario)
        {
            var respuesta = new RespuestaTimbrado
            {
                Exitoso = false,
                FechaTimbrado = DateTime.Now
            };

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Obtener el recibo de nómina con datos completos
                    var recibo = ObtenerReciboCompleto(nominaDetalleID, conn, tran);
                    if (recibo == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Recibo de nómina no encontrado";
                        return respuesta;
                    }

                    // Validar que no esté ya timbrado
                    if (!string.IsNullOrEmpty(recibo.UUID))
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = $"Este recibo ya ha sido timbrado. UUID: {recibo.UUID}";
                        return respuesta;
                    }

                    // 2. Obtener datos del empleado completos
                    var empleado = CD_Empleado.Instancia.ObtenerPorId(recibo.EmpleadoID);
                    if (empleado == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Empleado no encontrado";
                        return respuesta;
                    }

                    // 3. Obtener configuración de FiscalAPI
                    var configFiscalAPI = ObtenerConfiguracionFiscalAPI();
                    if (configFiscalAPI == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Configuración de FiscalAPI no encontrada en Web.config";
                        return respuesta;
                    }

                    // 4. Construir request de FiscalAPI para nómina
                    var requestNomina = ConstruirRequestFiscalAPINomina(recibo, empleado, configFiscalAPI);

                    // 5. Insertar registro en NominasCFDI con estado PENDIENTE
                    int nominaCFDIID = InsertarNominaCFDI(recibo, "", usuario, conn, tran);

                    // 6. Timbrar con FiscalAPI
                    using (var fiscalAPIService = new PAC.FiscalAPIService(configFiscalAPI))
                    {
                        respuesta = await fiscalAPIService.CrearYTimbrarCFDINomina(requestNomina);
                    }

                    // 7. Actualizar registros según resultado
                    if (respuesta.Exitoso)
                    {
                        // Timbrado exitoso
                        ActualizarNominaCFDIExitoso(nominaCFDIID, respuesta, conn, tran);
                        ActualizarReciboTimbrado(nominaDetalleID, respuesta, conn, tran);

                        respuesta.Mensaje = $"Recibo timbrado exitosamente. UUID: {respuesta.UUID}";
                        
                        System.Diagnostics.Debug.WriteLine($"✓ Nómina timbrada - Empleado: {empleado.Nombre}, UUID: {respuesta.UUID}");
                    }
                    else
                    {
                        // Error en timbrado
                        ActualizarNominaCFDIError(nominaCFDIID, respuesta, conn, tran);
                        respuesta.Mensaje = $"Error al timbrar nómina: {respuesta.Mensaje}";
                        
                        System.Diagnostics.Debug.WriteLine($"✗ Error timbrado nómina - Empleado: {empleado.Nombre}, Error: {respuesta.Mensaje}");
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = $"Excepción al timbrar nómina: {ex.Message}";
                    respuesta.CodigoError = "EXCEPTION";
                    respuesta.ErrorTecnico = ex.ToString();
                    
                    System.Diagnostics.Debug.WriteLine($"✗ Excepción en TimbrarCFDINomina: {ex}");
                }
            }

            return respuesta;
        }

        /// <summary>
        /// Construye el request completo de FiscalAPI para nómina
        /// Mapea datos del sistema a la estructura requerida por FiscalAPI
        /// </summary>
        private CapaModelo.PAC.FiscalAPINominaRequest ConstruirRequestFiscalAPINomina(
            NominaDetalle recibo, 
            Empleado empleado, 
            ConfiguracionFiscalAPI config)
        {
            // Obtener configuración de empresa
            var configEmpresa = ObtenerConfiguracionEmpresa();

            var request = new CapaModelo.PAC.FiscalAPINominaRequest
            {
                VersionCode = "4.0",
                Series = configEmpresa.SerieCFDINomina ?? "NOM",
                Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                PaymentMethodCode = "PUE", // Pago en una sola exhibición
                CurrencyCode = "MXN",
                TypeCode = "N", // N = Nómina
                ExpeditionZipCode = configEmpresa.CodigoPostal,
                ExportCode = "01", // No aplica

                // EMISOR (Patrón)
                Issuer = new CapaModelo.PAC.FiscalAPIIssuerNomina
                {
                    Tin = configEmpresa.RFC,
                    LegalName = configEmpresa.RazonSocial,
                    TaxRegimeCode = configEmpresa.RegimenFiscal,
                    EmployerData = new CapaModelo.PAC.FiscalAPIEmployerData
                    {
                        EmployerRegistration = configEmpresa.RegistroPatronal ?? "Z0000000000"
                    },
                    TaxCredentials = ObtenerCredencialesFiscales(config)
                },

                // RECEPTOR (Empleado)
                Recipient = new CapaModelo.PAC.FiscalAPIRecipientNomina
                {
                    Tin = empleado.RFC,
                    LegalName = empleado.Nombre,
                    ZipCode = empleado.CodigoPostal ?? "00000",
                    TaxRegimeCode = "605", // Sueldos y Salarios
                    CfdiUseCode = "CN01", // Nómina
                    EmployeeData = new CapaModelo.PAC.FiscalAPIEmployeeData
                    {
                        Curp = empleado.CURP ?? "XEXX010101HNEXXXA4",
                        SocialSecurityNumber = empleado.NSS ?? "00000000000",
                        LaborRelationStartDate = empleado.FechaIngreso.ToString("yyyy-MM-dd"),
                        Seniority = CalcularAntiguedadISO8601(empleado.FechaIngreso),
                        SatContractTypeId = empleado.TipoContrato ?? "01", // Indeterminado
                        SatUnionizedStatusId = "No",
                        SatTaxRegimeTypeId = empleado.TipoRegimen ?? "02", // Sueldos
                        EmployeeNumber = recibo.NumeroEmpleado,
                        Department = empleado.Departamento ?? "GENERAL",
                        Position = recibo.Puesto ?? empleado.Puesto ?? "EMPLEADO",
                        SatJobRiskId = "1", // Clase I - Riesgo mínimo
                        SatPaymentPeriodicityId = ObtenerPeriodicidadPago(recibo),
                        SatBankId = empleado.CodigoBanco ?? "012", // Banamex default
                        BankAccount = empleado.CuentaBancaria,
                        BaseSalaryForContributions = empleado.SalarioDiario > 0 ? empleado.SalarioDiario : recibo.SalarioDiario,
                        IntegratedDailySalary = empleado.SalarioDiarioIntegrado.HasValue ? empleado.SalarioDiarioIntegrado.Value : 0,
                        SatPayrollStateId = ObtenerEstadoPorCP(configEmpresa.CodigoPostal)
                    }
                },

                // COMPLEMENTO DE NÓMINA
                Complement = new CapaModelo.PAC.FiscalAPINominaComplement
                {
                    Payroll = new CapaModelo.PAC.FiscalAPIPayroll
                    {
                        Version = "1.2",
                        PayrollTypeCode = "O", // O = Ordinaria
                        PaymentDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        InitialPaymentDate = recibo.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                        FinalPaymentDate = recibo.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                        DaysPaid = recibo.DiasTrabajados,

                        // PERCEPCIONES
                        Earnings = new CapaModelo.PAC.FiscalAPIEarningsContainer
                        {
                            Earnings = ConvertirPercepcionesAFiscalAPI(recibo.Percepciones),
                            OtherPayments = new List<CapaModelo.PAC.FiscalAPIOtherPayment>
                            {
                                // Subsidio al empleo (si aplica)
                                new CapaModelo.PAC.FiscalAPIOtherPayment
                                {
                                    OtherPaymentTypeCode = "002",
                                    Code = "SUB",
                                    Concept = "Subsidio para el empleo",
                                    Amount = 0.0m,
                                    SubsidyCaused = 0.0m
                                }
                            }
                        },

                        // DEDUCCIONES
                        Deductions = ConvertirDeduccionesAFiscalAPI(recibo.Deducciones)
                    }
                }
            };

            return request;
        }

        /// <summary>
        /// Convierte percepciones del sistema a formato FiscalAPI
        /// </summary>
        private List<CapaModelo.PAC.FiscalAPIEarning> ConvertirPercepcionesAFiscalAPI(List<NominaPercepcion> percepciones)
        {
            var lista = new List<CapaModelo.PAC.FiscalAPIEarning>();

            foreach (var p in percepciones)
            {
                lista.Add(new CapaModelo.PAC.FiscalAPIEarning
                {
                    EarningTypeCode = p.TipoPercepcion ?? "001", // 001 = Sueldos por default
                    Code = p.Clave,
                    Concept = p.Concepto,
                    TaxedAmount = p.ImporteGravado,
                    ExemptAmount = p.ImporteExento
                });
            }

            return lista;
        }

        /// <summary>
        /// Convierte deducciones del sistema a formato FiscalAPI
        /// </summary>
        private List<CapaModelo.PAC.FiscalAPIDeduction> ConvertirDeduccionesAFiscalAPI(List<NominaDeduccion> deducciones)
        {
            var lista = new List<CapaModelo.PAC.FiscalAPIDeduction>();

            foreach (var d in deducciones)
            {
                lista.Add(new CapaModelo.PAC.FiscalAPIDeduction
                {
                    DeductionTypeCode = d.TipoDeduccion ?? "004", // 004 = Otros por default
                    Code = d.Clave,
                    Concept = d.Concepto,
                    Amount = d.Importe
                });
            }

            return lista;
        }

        /// <summary>
        /// Calcula antigüedad en formato ISO 8601 Duration (P[n]Y[n]M[n]W[n]D)
        /// Ejemplo: P2Y3M = 2 años, 3 meses
        /// </summary>
        private string CalcularAntiguedadISO8601(DateTime fechaIngreso)
        {
            TimeSpan diff = DateTime.Now - fechaIngreso;
            int semanas = (int)(diff.TotalDays / 7);
            
            // Formato simple: P[semanas]W
            return $"P{semanas}W";
        }

        /// <summary>
        /// Obtiene código de periodicidad según días trabajados
        /// </summary>
        private string ObtenerPeriodicidadPago(NominaDetalle recibo)
        {
            int dias = (int)recibo.DiasTrabajados;
            
            if (dias <= 7) return "02"; // Semanal
            if (dias <= 10) return "10"; // Decenal
            if (dias <= 14) return "03"; // Catorcenal
            if (dias <= 15) return "04"; // Quincenal
            if (dias <= 31) return "05"; // Mensual
            
            return "99"; // Otra periodicidad
        }

        /// <summary>
        /// Obtiene código de estado SAT según código postal
        /// </summary>
        private string ObtenerEstadoPorCP(string cp)
        {
            if (string.IsNullOrEmpty(cp) || cp.Length < 2) return "CMX";
            
            // Mapeo básico por rangos de CP (los más comunes)
            int codigo = int.Parse(cp.Substring(0, 2));
            
            if (codigo >= 1 && codigo <= 16) return "CMX"; // Ciudad de México
            if (codigo >= 20 && codigo <= 23) return "AGU"; // Aguascalientes
            if (codigo >= 44 && codigo <= 49) return "JAL"; // Jalisco
            if (codigo >= 64 && codigo <= 67) return "NLE"; // Nuevo León
            
            return "MEX"; // Estado de México por default
        }

        /// <summary>
        /// Obtiene credenciales fiscales (certificado y llave) en base64
        /// </summary>
        private List<CapaModelo.PAC.FiscalAPITaxCredential> ObtenerCredencialesFiscales(ConfiguracionFiscalAPI config)
        {
            var credenciales = new List<CapaModelo.PAC.FiscalAPITaxCredential>();

            // Certificado .CER
            if (!string.IsNullOrEmpty(config.CertificadoBase64))
            {
                credenciales.Add(new CapaModelo.PAC.FiscalAPITaxCredential
                {
                    Base64File = config.CertificadoBase64,
                    FileType = 0, // 0 = .CER
                    Password = config.PasswordLlave
                });
            }

            // Llave privada .KEY
            if (!string.IsNullOrEmpty(config.LlavePrivadaBase64))
            {
                credenciales.Add(new CapaModelo.PAC.FiscalAPITaxCredential
                {
                    Base64File = config.LlavePrivadaBase64,
                    FileType = 1, // 1 = .KEY
                    Password = config.PasswordLlave
                });
            }

            return credenciales;
        }

        /// <summary>
        /// Obtiene configuración de FiscalAPI desde Web.config
        /// </summary>
        /// <summary>
        /// Obtiene la configuración de FiscalAPI desde la base de datos
        /// Reutiliza la misma configuración que la facturación de ventas
        /// </summary>
        private ConfiguracionFiscalAPI ObtenerConfiguracionFiscalAPI()
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT TOP 1 ConfiguracionID, ApiKey, Tenant, Ambiente, RfcEmisor, NombreEmisor,
                               RegimenFiscal, CertificadoBase64, LlavePrivadaBase64, PasswordLlave,
                               CodigoPostal, Activo
                        FROM ConfiguracionFiscalAPI
                        WHERE Activo = 1
                        ORDER BY FechaCreacion DESC";

                    SqlCommand cmd = new SqlCommand(query, cnx);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var config = new ConfiguracionFiscalAPI
                            {
                                ConfiguracionID = Convert.ToInt32(dr["ConfiguracionID"]),
                                ApiKey = dr["ApiKey"].ToString(),
                                Tenant = dr["Tenant"].ToString(),
                                Ambiente = dr["Ambiente"].ToString(), // "TEST" o "PRODUCTION"
                                RfcEmisor = dr["RfcEmisor"].ToString(),
                                NombreEmisor = dr["NombreEmisor"].ToString(),
                                RegimenFiscal = dr["RegimenFiscal"].ToString(),
                                CertificadoBase64 = dr["CertificadoBase64"]?.ToString(),
                                LlavePrivadaBase64 = dr["LlavePrivadaBase64"]?.ToString(),
                                PasswordLlave = dr["PasswordLlave"]?.ToString(),
                                CodigoPostal = dr["CodigoPostal"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            };

                            // UrlApi se calcula automáticamente según Ambiente (propiedad de solo lectura)
                            // PasswordLlave ya está mapeado en la asignación anterior

                            return config;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener configuración FiscalAPI: {ex.Message}");
                return null;
            }
        }

        private NominaDetalle ObtenerReciboCompleto(int nominaDetalleID, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                SELECT nd.*, n.FechaInicio, n.FechaFin, n.TipoNomina
                FROM NominaDetalle nd
                INNER JOIN Nominas n ON nd.NominaID = n.NominaID
                WHERE nd.NominaDetalleID = @ID";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ID", nominaDetalleID);
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var recibo = new NominaDetalle
                        {
                            NominaDetalleID = (int)dr["NominaDetalleID"],
                            NominaID = (int)dr["NominaID"],
                            EmpleadoID = (int)dr["EmpleadoID"],
                            NumeroEmpleado = dr["NumeroEmpleado"].ToString(),
                            NombreEmpleado = dr["NombreEmpleado"].ToString(),
                            RFC = dr["RFC"].ToString(),
                            NSS = dr["NSS"]?.ToString(),
                            CURP = dr["CURP"]?.ToString(),
                            Puesto = dr["Puesto"]?.ToString(),
                            DiasTrabajados = Convert.ToDecimal(dr["DiasTrabajados"]),
                            FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            SalarioDiario = Convert.ToDecimal(dr["SalarioDiario"]),
                            SalarioBase = Convert.ToDecimal(dr["SalarioBase"]),
                            TotalPercepciones = Convert.ToDecimal(dr["TotalPercepciones"]),
                            TotalPercepcionesGravadas = Convert.ToDecimal(dr["TotalPercepcionesGravadas"]),
                            TotalPercepcionesExentas = Convert.ToDecimal(dr["TotalPercepcionesExentas"]),
                            TotalDeducciones = Convert.ToDecimal(dr["TotalDeducciones"]),
                            TotalImpuestosRetenidos = Convert.ToDecimal(dr["TotalImpuestosRetenidos"]),
                            TotalOtrasDeducciones = Convert.ToDecimal(dr["TotalOtrasDeducciones"]),
                            NetoPagar = Convert.ToDecimal(dr["NetoPagar"]),
                            UUID = dr["UUID"]?.ToString()
                        };

                        // Cargar percepciones y deducciones
                        recibo.Percepciones = ObtenerPercepcionesRecibo(nominaDetalleID, conn, tran);
                        recibo.Deducciones = ObtenerDeduccionesRecibo(nominaDetalleID, conn, tran);

                        return recibo;
                    }
                }
            }
            return null;
        }

        private List<NominaPercepcion> ObtenerPercepcionesRecibo(int nominaDetalleID, SqlConnection conn, SqlTransaction tran)
        {
            var lista = new List<NominaPercepcion>();
            string query = "SELECT * FROM NominaPercepciones WHERE NominaDetalleID = @ID";
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ID", nominaDetalleID);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new NominaPercepcion
                        {
                            Clave = dr["Clave"].ToString(),
                            Concepto = dr["Concepto"].ToString(),
                            TipoPercepcion = dr["TipoPercepcion"].ToString(),
                            ImporteGravado = Convert.ToDecimal(dr["ImporteGravado"]),
                            ImporteExento = Convert.ToDecimal(dr["ImporteExento"]),
                            ImporteTotal = Convert.ToDecimal(dr["ImporteTotal"])
                        });
                    }
                }
            }
            return lista;
        }

        private List<NominaDeduccion> ObtenerDeduccionesRecibo(int nominaDetalleID, SqlConnection conn, SqlTransaction tran)
        {
            var lista = new List<NominaDeduccion>();
            string query = "SELECT * FROM NominaDeducciones WHERE NominaDetalleID = @ID";
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ID", nominaDetalleID);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new NominaDeduccion
                        {
                            Clave = dr["Clave"].ToString(),
                            Concepto = dr["Concepto"].ToString(),
                            TipoDeduccion = dr["TipoDeduccion"].ToString(),
                            Importe = Convert.ToDecimal(dr["Importe"])
                        });
                    }
                }
            }
            return lista;
        }

        private ConfiguracionEmpresa ObtenerConfiguracionEmpresa()
        {
            // TODO: Obtener de tabla ConfiguracionEmpresa o Web.config
            return new ConfiguracionEmpresa
            {
                RFC = "XAXX010101000",
                RazonSocial = "EMPRESA EJEMPLO SA DE CV",
                RegimenFiscal = "601",
                CodigoPostal = "31000",
                RegistroPatronal = "Y9999999999",
                SerieCFDINomina = "NOM"
            };
        }

        private ConfiguracionPAC ObtenerConfiguracionPAC()
        {
            // TODO: Obtener de tabla ConfiguracionPAC
            return new ConfiguracionPAC
            {
                Usuario = "usuario_prueba",
                Password = "password_prueba",
                UrlTimbrado = "https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl",
                UrlCancelacion = "https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl",
                TimeoutSegundos = 30
            };
        }

        private int InsertarNominaCFDI(NominaDetalle recibo, string xmlSinTimbrar, string usuario, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO NominasCFDI 
                (NominaDetalleID, FolioCFDI, SerieCFDI, EstadoTimbrado, XMLSinTimbrar, SubTotal, Descuento, Total, UsuarioRegistro)
                VALUES (@NominaDetalleID, @Folio, @Serie, 'PENDIENTE', @XML, @SubTotal, @Descuento, @Total, @Usuario);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@NominaDetalleID", recibo.NominaDetalleID);
                cmd.Parameters.AddWithValue("@Folio", recibo.NumeroEmpleado + "-" + DateTime.Now.ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@Serie", "NOM");
                cmd.Parameters.AddWithValue("@XML", xmlSinTimbrar);
                cmd.Parameters.AddWithValue("@SubTotal", recibo.TotalPercepciones);
                cmd.Parameters.AddWithValue("@Descuento", recibo.TotalDeducciones);
                cmd.Parameters.AddWithValue("@Total", recibo.NetoPagar);
                cmd.Parameters.AddWithValue("@Usuario", usuario);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void ActualizarNominaCFDIExitoso(int nominaCFDIID, RespuestaTimbrado respuesta, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE NominasCFDI
                SET UUID = @UUID,
                    FechaTimbrado = @FechaTimbrado,
                    EstadoTimbrado = 'EXITOSO',
                    XMLTimbrado = @XMLTimbrado,
                    SelloCFD = @SelloCFD,
                    SelloSAT = @SelloSAT,
                    InvoiceId = @InvoiceId,
                    NoCertificadoSAT = @NoCertificadoSAT,
                    CadenaOriginal = @CadenaOriginal,
                    UltimaActualizacion = GETDATE()
                WHERE NominaCFDIID = @ID";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@UUID", respuesta.UUID);
                cmd.Parameters.AddWithValue("@FechaTimbrado", respuesta.FechaTimbrado);
                cmd.Parameters.AddWithValue("@XMLTimbrado", respuesta.XMLTimbrado);
                cmd.Parameters.AddWithValue("@SelloCFD", respuesta.SelloCFD ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SelloSAT", respuesta.SelloSAT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceId", respuesta.InvoiceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NoCertificadoSAT", respuesta.NoCertificadoSAT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CadenaOriginal", "" ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", nominaCFDIID);
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarReciboTimbrado(int nominaDetalleID, RespuestaTimbrado respuesta, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE NominaDetalle
                SET UUID = @UUID,
                    FechaTimbrado = @FechaTimbrado,
                    SelloCFD = @SelloCFD,
                    SelloSAT = @SelloSAT,
                    EstatusTimbre = 'TIMBRADO',
                    UltimaAct = GETDATE()
                WHERE NominaDetalleID = @ID";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@UUID", respuesta.UUID);
                cmd.Parameters.AddWithValue("@FechaTimbrado", respuesta.FechaTimbrado);
                cmd.Parameters.AddWithValue("@SelloCFD", respuesta.SelloCFD ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SelloSAT", respuesta.SelloSAT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", nominaDetalleID);
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarNominaCFDIError(int nominaCFDIID, RespuestaTimbrado respuesta, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE NominasCFDI
                SET EstadoTimbrado = 'ERROR',
                    CodigoError = @CodigoError,
                    MensajeError = @MensajeError,
                    UltimaActualizacion = GETDATE()
                WHERE NominaCFDIID = @ID";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@CodigoError", respuesta.CodigoError ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MensajeError", respuesta.Mensaje ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", nominaCFDIID);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene el InvoiceId de FiscalAPI para un recibo timbrado
        /// Usado para descargar el PDF desde FiscalAPI
        /// </summary>
        public string ObtenerInvoiceIdRecibo(int nominaDetalleID)
        {
            string invoiceId = null;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TOP 1 InvoiceId
                    FROM NominasCFDI
                    WHERE NominaDetalleID = @NominaDetalleID
                      AND EstadoTimbrado = 'EXITOSO'
                      AND InvoiceId IS NOT NULL
                    ORDER BY FechaTimbrado DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaDetalleID", nominaDetalleID);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        invoiceId = result.ToString();
                    }
                }
            }

            return invoiceId;
        }

        /// <summary>
        /// Obtiene el XML timbrado de un recibo de nómina
        /// </summary>
        public string ObtenerXMLTimbradoRecibo(int nominaDetalleID)
        {
            string xmlTimbrado = null;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TOP 1 XMLTimbrado
                    FROM NominasCFDI
                    WHERE NominaDetalleID = @NominaDetalleID
                      AND EstadoTimbrado = 'EXITOSO'
                      AND XMLTimbrado IS NOT NULL
                    ORDER BY FechaTimbrado DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaDetalleID", nominaDetalleID);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        xmlTimbrado = result.ToString();
                    }
                }
            }

            return xmlTimbrado;
        }

        /// <summary>
        /// Obtiene los datos básicos de un recibo por su ID
        /// </summary>
        public NominaDetalle ObtenerReciboPorId(int nominaDetalleID)
        {
            NominaDetalle recibo = null;

            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT nd.*
                    FROM NominaDetalle nd
                    WHERE nd.NominaDetalleID = @NominaDetalleID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NominaDetalleID", nominaDetalleID);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            recibo = new NominaDetalle
                            {
                                NominaDetalleID = Convert.ToInt32(dr["NominaDetalleID"]),
                                NominaID = Convert.ToInt32(dr["NominaID"]),
                                EmpleadoID = Convert.ToInt32(dr["EmpleadoID"]),
                                NumeroEmpleado = dr["Folio"]?.ToString(),
                                UUID = dr["UUID"] != DBNull.Value ? dr["UUID"].ToString() : null,
                                DiasTrabajados = Convert.ToDecimal(dr["DiasTrabajados"]),
                                TotalPercepciones = Convert.ToDecimal(dr["TotalPercepciones"]),
                                TotalDeducciones = Convert.ToDecimal(dr["TotalDeducciones"]),
                                NetoPagar = Convert.ToDecimal(dr["NetoAPagar"]),
                                EstatusTimbre = dr["EstatusTimbre"] != DBNull.Value ? dr["EstatusTimbre"].ToString() : null,
                                FechaTimbrado = dr["FechaTimbrado"] != DBNull.Value ? Convert.ToDateTime(dr["FechaTimbrado"]) : (DateTime?)null
                            };
                        }
                    }
                }
            }

            return recibo;
        }
    }
}
