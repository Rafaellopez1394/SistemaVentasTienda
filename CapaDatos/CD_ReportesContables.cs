// CapaDatos/CD_ReportesContables.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    public class CD_ReportesContables
    {
        private static CD_ReportesContables _instancia;
        public static CD_ReportesContables Instancia => _instancia ??= new CD_ReportesContables();

        // =============================================
        // 1. BALANZA DE COMPROBACIÓN
        // =============================================
        public List<BalanzaComprobacion> GenerarBalanza(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<BalanzaComprobacion>();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    WITH MovimientosPeriodo AS (
                        SELECT 
                            cc.CuentaID,
                            cc.Codigo AS CodigoCuenta,
                            cc.Nombre AS NombreCuenta,
                            cc.Tipo,
                            cc.Naturaleza,
                            ISNULL(SUM(CASE WHEN p.FechaPoliza < @FechaInicio THEN pd.Debe - pd.Haber ELSE 0 END), 0) AS SaldoInicial,
                            ISNULL(SUM(CASE WHEN p.FechaPoliza BETWEEN @FechaInicio AND @FechaFin THEN pd.Debe ELSE 0 END), 0) AS Debe,
                            ISNULL(SUM(CASE WHEN p.FechaPoliza BETWEEN @FechaInicio AND @FechaFin THEN pd.Haber ELSE 0 END), 0) AS Haber
                        FROM CatalogoContable cc
                        LEFT JOIN PolizasDetalle pd ON cc.CuentaID = pd.CuentaID
                        LEFT JOIN Polizas p ON pd.PolizaID = p.PolizaID
                        WHERE cc.Activo = 1
                        GROUP BY cc.CuentaID, cc.Codigo, cc.Nombre, cc.Tipo, cc.Naturaleza
                    )
                    SELECT 
                        CuentaID,
                        CodigoCuenta,
                        NombreCuenta,
                        Tipo,
                        Naturaleza,
                        SaldoInicial,
                        Debe,
                        Haber,
                        (SaldoInicial + Debe - Haber) AS SaldoFinal
                    FROM MovimientosPeriodo
                    ORDER BY CodigoCuenta";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date.AddDays(1).AddSeconds(-1));

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new BalanzaComprobacion
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            CodigoCuenta = dr["CodigoCuenta"].ToString(),
                            NombreCuenta = dr["NombreCuenta"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            Naturaleza = dr["Naturaleza"].ToString(),
                            SaldoInicial = Convert.ToDecimal(dr["SaldoInicial"]),
                            Debe = Convert.ToDecimal(dr["Debe"]),
                            Haber = Convert.ToDecimal(dr["Haber"]),
                            SaldoFinal = Convert.ToDecimal(dr["SaldoFinal"])
                        });
                    }
                }
            }
            
            return lista;
        }

        // =============================================
        // 2. ESTADO DE RESULTADOS
        // =============================================
        public EstadoResultados GenerarEstadoResultados(int mes, int año)
        {
            // Obtener nombre de empresa desde configuración
            string nombreEmpresa = "Mi Empresa";
            try
            {
                var config = CD_Configuracion.Instancia.ObtenerConfiguracionGeneral();
                if (config != null && !string.IsNullOrEmpty(config.NombreNegocio))
                {
                    nombreEmpresa = config.NombreNegocio;
                }
            }
            catch { /* Usar valor por defecto si falla */ }

            var resultado = new EstadoResultados
            {
                Empresa = nombreEmpresa,
                Periodo = $"{ObtenerNombreMes(mes)} {año}",
                FechaInicio = new DateTime(año, mes, 1),
                FechaFin = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)),
                Detalle = new List<LineaEstadoResultados>()
            };

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        cc.Codigo,
                        cc.Nombre,
                        cc.Tipo,
                        cc.SubTipo,
                        ISNULL(SUM(pd.Haber - pd.Debe), 0) AS Monto
                    FROM CatalogoContable cc
                    LEFT JOIN PolizasDetalle pd ON cc.CuentaID = pd.CuentaID
                    LEFT JOIN Polizas p ON pd.PolizaID = p.PolizaID
                    WHERE cc.Tipo IN ('INGRESO', 'EGRESO')
                        AND MONTH(p.FechaPoliza) = @Mes
                        AND YEAR(p.FechaPoliza) = @Año
                    GROUP BY cc.Codigo, cc.Nombre, cc.Tipo, cc.SubTipo
                    ORDER BY cc.Codigo";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@Mes", mes);
                cmd.Parameters.AddWithValue("@Año", año);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string tipo = dr["Tipo"].ToString();
                        string subTipo = dr["SubTipo"]?.ToString() ?? "";
                        decimal monto = Convert.ToDecimal(dr["Monto"]);

                        var linea = new LineaEstadoResultados
                        {
                            CodigoCuenta = dr["Codigo"].ToString(),
                            NombreCuenta = dr["Nombre"].ToString(),
                            Monto = monto
                        };

                        // Clasificar por tipo
                        if (tipo == "INGRESO")
                        {
                            linea.Seccion = "INGRESOS";
                            if (subTipo == "VENTAS")
                                resultado.VentasNetas += monto;
                            else
                                resultado.OtrosIngresos += monto;
                        }
                        else if (tipo == "EGRESO")
                        {
                            if (subTipo == "COSTO_VENTAS")
                            {
                                linea.Seccion = "COSTOS";
                                resultado.CostoVentas += monto;
                            }
                            else if (subTipo == "GASTOS_VENTA")
                            {
                                linea.Seccion = "GASTOS_VENTA";
                                resultado.GastosVenta += monto;
                            }
                            else if (subTipo == "GASTOS_ADMIN")
                            {
                                linea.Seccion = "GASTOS_ADMIN";
                                resultado.GastosAdministracion += monto;
                            }
                            else if (subTipo == "GASTOS_FINANCIEROS")
                            {
                                linea.Seccion = "GASTOS_FINANCIEROS";
                                resultado.GastosFinancieros += monto;
                            }
                        }

                        resultado.Detalle.Add(linea);
                    }
                }
            }

            // Calcular totales
            resultado.TotalIngresos = resultado.VentasNetas + resultado.OtrosIngresos;
            resultado.UtilidadBruta = resultado.VentasNetas - resultado.CostoVentas;
            resultado.TotalGastosOperacion = resultado.GastosVenta + resultado.GastosAdministracion;
            resultado.UtilidadOperacion = resultado.UtilidadBruta - resultado.TotalGastosOperacion;
            resultado.UtilidadAntesImpuestos = resultado.UtilidadOperacion - resultado.GastosFinancieros + resultado.ProductosFinancieros;
            resultado.ISR = resultado.UtilidadAntesImpuestos * 0.30m; // 30% ISR aproximado
            resultado.UtilidadNeta = resultado.UtilidadAntesImpuestos - resultado.ISR;

            return resultado;
        }

        // =============================================
        // 3. LIBRO DIARIO
        // =============================================
        public List<LibroDiario> GenerarLibroDiario(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<LibroDiario>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        p.PolizaID,
                        p.TipoPoliza,
                        p.FechaPoliza,
                        p.Concepto,
                        p.Referencia
                    FROM Polizas p
                    WHERE p.FechaPoliza BETWEEN @FechaInicio AND @FechaFin
                    ORDER BY p.FechaPoliza, p.PolizaID";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date.AddDays(1).AddSeconds(-1));

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var polizaId = Guid.Parse(dr["PolizaID"].ToString());

                        var poliza = new LibroDiario
                        {
                            PolizaID = polizaId,
                            TipoPoliza = dr["TipoPoliza"].ToString(),
                            FechaPoliza = Convert.ToDateTime(dr["FechaPoliza"]),
                            Concepto = dr["Concepto"].ToString(),
                            Referencia = dr["Referencia"]?.ToString(),
                            Asientos = ObtenerAsientos(polizaId, cnx)
                        };

                        poliza.TotalDebe = poliza.Asientos.Sum(a => a.Debe);
                        poliza.TotalHaber = poliza.Asientos.Sum(a => a.Haber);

                        lista.Add(poliza);
                    }
                }
            }

            return lista;
        }

        private List<AsientoContable> ObtenerAsientos(Guid polizaId, SqlConnection cnx)
        {
            var asientos = new List<AsientoContable>();

            var query = @"
                SELECT 
                    cc.Codigo AS CodigoCuenta,
                    cc.Nombre AS NombreCuenta,
                    pd.Debe,
                    pd.Haber
                FROM PolizasDetalle pd
                INNER JOIN CatalogoContable cc ON pd.CuentaID = cc.CuentaID
                WHERE pd.PolizaID = @PolizaID
                ORDER BY cc.Codigo";

            using (SqlCommand cmd = new SqlCommand(query, cnx))
            {
                cmd.Parameters.AddWithValue("@PolizaID", polizaId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        asientos.Add(new AsientoContable
                        {
                            CodigoCuenta = dr["CodigoCuenta"].ToString(),
                            NombreCuenta = dr["NombreCuenta"].ToString(),
                            Debe = Convert.ToDecimal(dr["Debe"]),
                            Haber = Convert.ToDecimal(dr["Haber"])
                        });
                    }
                }
            }

            return asientos;
        }

        // =============================================
        // 4. LIBRO MAYOR
        // =============================================
        public LibroMayor GenerarLibroMayor(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            LibroMayor mayor = null;

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                // Obtener info cuenta
                var queryCuenta = @"
                    SELECT CodigoCuenta, NombreCuenta, TipoCuenta
                    FROM CatalogoContable
                    WHERE CuentaID = @CuentaID";

                SqlCommand cmdCuenta = new SqlCommand(queryCuenta, cnx);
                cmdCuenta.Parameters.AddWithValue("@CuentaID", cuentaId);

                cnx.Open();
                using (SqlDataReader dr = cmdCuenta.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        mayor = new LibroMayor
                        {
                            CuentaID = cuentaId,
                            CodigoCuenta = dr["CodigoCuenta"].ToString(),
                            NombreCuenta = dr["NombreCuenta"].ToString(),
                            Movimientos = new List<MovimientoCuenta>()
                        };
                    }
                }

                if (mayor == null) return null;

                // Calcular saldo inicial
                var querySaldoInicial = @"
                    SELECT ISNULL(SUM(pd.Debe - pd.Haber), 0) AS SaldoInicial
                    FROM PolizasDetalle pd
                    INNER JOIN Polizas p ON pd.PolizaID = p.PolizaID
                    WHERE pd.CuentaID = @CuentaID
                        AND p.FechaPoliza < @FechaInicio";

                using (SqlCommand cmd = new SqlCommand(querySaldoInicial, cnx))
                {
                    cmd.Parameters.AddWithValue("@CuentaID", cuentaId);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                    mayor.SaldoInicial = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // Obtener movimientos
                var queryMovimientos = @"
                    SELECT 
                        p.FechaPoliza,
                        p.TipoPoliza,
                        p.Concepto,
                        p.Referencia,
                        pd.Debe,
                        pd.Haber
                    FROM PolizasDetalle pd
                    INNER JOIN Polizas p ON pd.PolizaID = p.PolizaID
                    WHERE pd.CuentaID = @CuentaID
                        AND p.FechaPoliza BETWEEN @FechaInicio AND @FechaFin
                    ORDER BY p.FechaPoliza";

                using (SqlCommand cmd = new SqlCommand(queryMovimientos, cnx))
                {
                    cmd.Parameters.AddWithValue("@CuentaID", cuentaId);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date.AddDays(1).AddSeconds(-1));

                    decimal saldoAcumulado = mayor.SaldoInicial;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            decimal cargo = Convert.ToDecimal(dr["Debe"]);
                            decimal abono = Convert.ToDecimal(dr["Haber"]);
                            saldoAcumulado += cargo - abono;

                            mayor.Movimientos.Add(new MovimientoCuenta
                            {
                                Fecha = Convert.ToDateTime(dr["FechaPoliza"]),
                                TipoPoliza = dr["TipoPoliza"].ToString(),
                                Concepto = dr["Concepto"].ToString(),
                                Referencia = dr["Referencia"]?.ToString(),
                                Cargo = cargo,
                                Abono = abono,
                                Saldo = saldoAcumulado
                            });

                            mayor.TotalCargos += cargo;
                            mayor.TotalAbonos += abono;
                        }
                    }
                }

                mayor.SaldoFinal = mayor.SaldoInicial + mayor.TotalCargos - mayor.TotalAbonos;
            }

            return mayor;
        }

        // =============================================
        // 5. REPORTE IVA
        // =============================================
        public ReporteIVA GenerarReporteIVA(int mes, int año)
        {
            var reporte = new ReporteIVA
            {
                Periodo = $"{ObtenerNombreMes(mes)} {año}",
                Mes = mes,
                Año = año,
                DetalleVentas = new List<DetalleIVA>(),
                DetalleCompras = new List<DetalleIVA>()
            };

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();

                // IVA Trasladado (Ventas)
                var queryVentas = @"
                    SELECT 
                        v.FechaVenta AS Fecha,
                        c.RFC,
                        c.RazonSocial AS Nombre,
                        CAST(v.VentaID AS VARCHAR(50)) AS Documento,
                        v.Total / 1.16 AS Base,
                        16 AS Tasa,
                        v.Total - (v.Total / 1.16) AS IVA,
                        v.Total
                    FROM VentasClientes v
                    INNER JOIN Clientes c ON v.ClienteID = c.ClienteID
                    WHERE MONTH(v.FechaVenta) = @Mes
                        AND YEAR(v.FechaVenta) = @Año
                    ORDER BY v.FechaVenta";

                using (SqlCommand cmd = new SqlCommand(queryVentas, cnx))
                {
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    cmd.Parameters.AddWithValue("@Año", año);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            decimal iva = Convert.ToDecimal(dr["IVA"]);
                            reporte.IVA16Trasladado += iva;

                            reporte.DetalleVentas.Add(new DetalleIVA
                            {
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                RFC = dr["RFC"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Documento = dr["Documento"].ToString(),
                                Base = Convert.ToDecimal(dr["Base"]),
                                Tasa = Convert.ToDecimal(dr["Tasa"]),
                                IVA = iva,
                                Total = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }

                // IVA Acreditable (Compras)
                var queryCompras = @"
                    SELECT 
                        c.FechaCompra AS Fecha,
                        p.RFC,
                        p.RazonSocial AS Nombre,
                        c.FolioFactura AS Documento,
                        c.Total / 1.16 AS Base,
                        16 AS Tasa,
                        c.Total - (c.Total / 1.16) AS IVA,
                        c.Total
                    FROM Compras c
                    INNER JOIN Proveedores p ON c.ProveedorID = p.ProveedorID
                    WHERE MONTH(c.FechaCompra) = @Mes
                        AND YEAR(c.FechaCompra) = @Año
                    ORDER BY c.FechaCompra";

                using (SqlCommand cmd = new SqlCommand(queryCompras, cnx))
                {
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    cmd.Parameters.AddWithValue("@Año", año);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            decimal iva = Convert.ToDecimal(dr["IVA"]);
                            reporte.IVA16Acreditable += iva;

                            reporte.DetalleCompras.Add(new DetalleIVA
                            {
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                RFC = dr["RFC"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Documento = dr["Documento"].ToString(),
                                Base = Convert.ToDecimal(dr["Base"]),
                                Tasa = Convert.ToDecimal(dr["Tasa"]),
                                IVA = iva,
                                Total = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
            }

            // Calcular totales
            reporte.TotalIVATrasladado = reporte.IVA16Trasladado + reporte.IVA8Trasladado + reporte.IVA0Trasladado;
            reporte.TotalIVAAcreditable = reporte.IVA16Acreditable + reporte.IVA8Acreditable;
            
            decimal diferencia = reporte.TotalIVATrasladado - reporte.TotalIVAAcreditable;
            
            if (diferencia > 0)
                reporte.SaldoFavor = diferencia; // A pagar al SAT
            else
                reporte.SaldoAFavor = Math.Abs(diferencia); // A favor del contribuyente

            return reporte;
        }

        // =============================================
        // HELPER METHODS
        // =============================================
        
        // Obtener catálogo de cuentas contables
        public List<CuentaContable> ObtenerCatalogoCuentas()
        {
            var lista = new List<CuentaContable>();
            
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        CuentaID,
                        CodigoCuenta,
                        NombreCuenta,
                        TipoCuenta,
                        SubTipo
                    FROM CatalogoContable
                    WHERE Activo = 1
                    ORDER BY CodigoCuenta";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();
                
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CuentaContable
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            Codigo = dr["CodigoCuenta"].ToString(),
                            Nombre = dr["NombreCuenta"].ToString(),
                            Tipo = dr["TipoCuenta"].ToString(),
                            Naturaleza = dr["SubTipo"].ToString()
                        });
                    }
                }
            }
            
            return lista;
        }
        
        private string ObtenerNombreMes(int mes)
        {
            string[] meses = { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", 
                              "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            return meses[mes];
        }
    }
}
