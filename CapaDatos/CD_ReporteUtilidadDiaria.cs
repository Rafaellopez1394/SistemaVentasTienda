// CapaDatos/CD_ReporteUtilidadDiaria.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaModelo;

namespace CapaDatos
{
    public class CD_ReporteUtilidadDiaria
    {
        private static CD_ReporteUtilidadDiaria _instancia;
        public static CD_ReporteUtilidadDiaria Instancia => _instancia ??= new CD_ReporteUtilidadDiaria();

        public ReporteUtilidadDiaria ObtenerReporteDiario(DateTime fecha, int sucursalID = 1)
        {
            var reporte = new ReporteUtilidadDiaria
            {
                Fecha = fecha,
                SucursalID = sucursalID
            };

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteUtilidadDiaria", cnx) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Fecha", fecha.Date);
                        cmd.Parameters.AddWithValue("@SucursalID", sucursalID);

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            // SECCIÓN 1: RESUMEN DE VENTAS
                            while (dr.Read())
                            {
                                if (dr["Seccion"]?.ToString() == "RESUMEN DE VENTAS")
                                {
                                    reporte.ResumenVentas.Add(new ResumenVentas
                                    {
                                        FormaPago = dr["FormaPago"].ToString(),
                                        Tickets = Convert.ToInt32(dr["Tickets"] ?? 0),
                                        TotalVentas = Convert.ToDecimal(dr["TotalVentas"] ?? 0),
                                        TotalUnidades = Convert.ToInt32(dr["TotalUnidades"] ?? 0)
                                    });

                                    reporte.TotalTickets += Convert.ToInt32(dr["Tickets"] ?? 0);
                                    reporte.TotalUnidades += Convert.ToInt32(dr["TotalUnidades"] ?? 0);
                                    reporte.TotalVentasContado += dr["FormaPago"]?.ToString() != "CREDITOS" 
                                        ? Convert.ToDecimal(dr["TotalVentas"] ?? 0)
                                        : 0;
                                    reporte.TotalVentasCredito += dr["FormaPago"]?.ToString() == "CREDITOS"
                                        ? Convert.ToDecimal(dr["TotalVentas"] ?? 0)
                                        : 0;
                                }
                            }

                            // SECCIÓN 2: COSTOS DE COMPRA
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    decimal costo = Convert.ToDecimal(dr["Monto"] ?? 0);
                                    reporte.CostosCotidiano.Add(new CostoDetalle
                                    {
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Monto = costo,
                                        Unidades = Convert.ToInt32(dr["Unidades"] ?? 0)
                                    });
                                    reporte.CostosCompra = costo;
                                }
                            }

                            // SECCIÓN 3: COSTOS CRÉDITO
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    decimal costoCredito = Convert.ToDecimal(dr["Monto"] ?? 0);
                                    reporte.CostosCotidiano.Add(new CostoDetalle
                                    {
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Monto = costoCredito,
                                        Unidades = Convert.ToInt32(dr["Unidades"] ?? 0)
                                    });
                                }
                            }

                            // SECCIÓN 4: UTILIDAD DEL DÍA
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    var util = new UtilityDetalle
                                    {
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Monto = Convert.ToDecimal(dr["Monto"] ?? 0)
                                    };
                                    reporte.Utilidad.Add(util);

                                    if (dr["Descripcion"]?.ToString().Contains("UTILIDAD DEL DÍA") ?? false)
                                        reporte.UtilidadDiaria = util.Monto;
                                }
                            }

                            // SECCIÓN 5: RECUPERACIÓN DE CRÉDITOS
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    decimal recuCoins = Convert.ToDecimal(dr["Monto"] ?? 0);
                                    reporte.Recuperacion.Add(new RecuperacionDetalle
                                    {
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Monto = recuCoins,
                                        VentasRecuperadas = Convert.ToInt32(dr["Unidades"] ?? 0)
                                    });
                                    reporte.RecuperoCreditosTotal = recuCoins;
                                }
                            }

                            // SECCIÓN 6: COSTO CRÉDITO RECUPERADO
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    decimal costoCred = Convert.ToDecimal(dr["Monto"] ?? 0);
                                    reporte.Recuperacion.Add(new RecuperacionDetalle
                                    {
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Monto = costoCred
                                    });
                                    reporte.CostoCreditoTotal = costoCred;
                                }
                            }

                            // SECCIÓN 7: INVENTARIO INICIAL
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    reporte.InventarioInicial.Add(new InventarioDetalle
                                    {
                                        Producto = dr["Producto"].ToString(),
                                        Cantidad = Convert.ToInt32(dr["Cantidad"] ?? 0),
                                        Valor = Convert.ToDecimal(dr["Valor"] ?? 0)
                                    });
                                }
                            }

                            // SECCIÓN 8: ENTRADAS DEL DÍA
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    reporte.Entradas.Add(new EntradaDetalle
                                    {
                                        Producto = dr["Producto"].ToString(),
                                        Cantidad = Convert.ToInt32(dr["Cantidad"] ?? 0),
                                        Valor = Convert.ToDecimal(dr["Valor"] ?? 0)
                                    });
                                }
                            }

                            // SECCIÓN 9: DETALLE VENTAS POR PRODUCTO
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    reporte.DetalleVentas.Add(new DetalleVentaProducto
                                    {
                                        Producto = dr["Producto"].ToString(),
                                        VentasContado = Convert.ToInt32(dr["VentasContado"] ?? 0),
                                        VentasCredito = Convert.ToInt32(dr["VentasCredito"] ?? 0),
                                        TotalVentas = Convert.ToInt32(dr["TotalVentas"] ?? 0),
                                        CostoTotal = Convert.ToDecimal(dr["CostoTotal"] ?? 0)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteDiario: {ex.Message}");
                throw;
            }

            return reporte;
        }
    }
}
