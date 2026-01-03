using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reportes
    {
        public static CD_Reportes _instancia = null;

        private CD_Reportes()
        {

        }

        public static CD_Reportes Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_Reportes();
                }
                return _instancia;
            }
        }

        public List<ReporteProducto> ReporteProductoSucursal(int SucursalID, string CodigoProducto)
        {
            List<ReporteProducto> lista = new List<ReporteProducto>();

            NumberFormatInfo formato = new CultureInfo("es-PE").NumberFormat;
            formato.CurrencyGroupSeparator = ".";

            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("usp_rptProductoSucursal", oConexion);
                cmd.Parameters.AddWithValue("@SucursalID", SucursalID);
                cmd.Parameters.AddWithValue("@Codigo", CodigoProducto);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();
                    
                    using (SqlDataReader dr = cmd.ExecuteReader()) {
                        while (dr.Read()) {
                            lista.Add(new ReporteProducto()
                            {
                                RucSucursal = dr["RFC Sucursal"].ToString(),
                                NombreSucursal = dr["Nombre Sucursal"].ToString(),
                                DireccionSucursal = dr["Direccion Sucursal"].ToString(),
                                CodigoProducto = dr["Codigo Producto"].ToString(),
                                NombreProducto = dr["Nombre Producto"].ToString(),
                                DescripcionProducto = dr["Descripcion Producto"].ToString(),
                                StockenSucursal = dr["Stock en tienda"].ToString(),
                                PrecioCompra = Convert.ToDecimal(dr["Precio Compra"].ToString(),new CultureInfo("es-PE")).ToString("N", formato),
                                PrecioVenta = Convert.ToDecimal(dr["Precio Venta"].ToString(),new CultureInfo("es-PE")).ToString("N", formato)
                            });
                        }

                    }

                }
                catch (Exception ex)
                {
                    lista = new List<ReporteProducto>();
                }
            }

            return lista;
        }

        public List<ReporteVenta> ReporteVenta(DateTime FechaInicio, DateTime FechaFin, int SucursalID)
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();

            NumberFormatInfo formato = new CultureInfo("es-PE").NumberFormat;
            formato.CurrencyGroupSeparator = ".";

            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("usp_rptVenta", oConexion);
                cmd.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", FechaFin);
                cmd.Parameters.AddWithValue("@SucursalID", SucursalID);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteVenta()
                            {
                                FechaVenta = dr["Fecha Venta"].ToString(),
                                NumeroDocumento = dr["Numero Documento"].ToString(),
                                TipoDocumento = dr["Tipo Documento"].ToString(),
                                NombreSucursal = dr["Nombre Sucursal"].ToString(),
                                RucSucursal = dr["RFC Sucursal"].ToString(),
                                NombreEmpleado = dr["Nombre Empleado"].ToString(),
                                CantidadUnidadesVendidas = dr["Cantidad Unidades Vendidas"].ToString(),
                                CantidadProductos = dr["Cantidad Productos"].ToString(),
                                TotalVenta = Convert.ToDecimal(dr["Total Venta"].ToString(), new CultureInfo("es-PE")).ToString("N", formato)
                            });
                        }

                    }

                }
                catch (Exception ex)
                {
                    lista = new List<ReporteVenta>();
                }
            }

            return lista;

        }

        public ResumenNomina ObtenerResumenNomina(DateTime fechaInicio, DateTime fechaFin)
        {
            ResumenNomina resumen = new ResumenNomina();

            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    oConexion.Open();

                    // Query para obtener el resumen de nómina del periodo
                    // NOTA: Ajustar según la estructura real de las tablas de nómina
                    string query = @"
                        SELECT 
                            ISNULL(SUM(TotalPercepciones), 0) AS TotalPercepciones,
                            ISNULL(SUM(TotalDeducciones), 0) AS TotalDeducciones,
                            ISNULL(SUM(TotalPercepciones - TotalDeducciones), 0) AS TotalNeto,
                            COUNT(*) AS TotalEmpleados
                        FROM Nomina
                        WHERE FechaPago BETWEEN @FechaInicio AND @FechaFin
                        AND Estatus = 'PAGADA'";

                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resumen.TotalPercepciones = Convert.ToDecimal(dr["TotalPercepciones"]);
                            resumen.TotalDeducciones = Convert.ToDecimal(dr["TotalDeducciones"]);
                            resumen.TotalNeto = Convert.ToDecimal(dr["TotalNeto"]);
                            resumen.TotalEmpleados = Convert.ToInt32(dr["TotalEmpleados"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error, devolver valores en 0
                    resumen.TotalPercepciones = 0;
                    resumen.TotalDeducciones = 0;
                    resumen.TotalNeto = 0;
                    resumen.TotalEmpleados = 0;
                }
            }

            return resumen;
        }

        /// <summary>
        /// Obtiene el resumen detallado de nómina para pólizas contables
        /// Incluye percepciones, deducciones y cuotas patronales desglosadas
        /// </summary>
        public CapaModelo.ResumenNominaContable ObtenerResumenNominaContable(DateTime fechaInicio, DateTime fechaFin)
        {
            CapaModelo.ResumenNominaContable resumen = new CapaModelo.ResumenNominaContable();

            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    oConexion.Open();

                    // Query mejorada para obtener desglose detallado
                    // Ajustar según las columnas reales de tu tabla Nomina
                    string query = @"
                        SELECT 
                            COUNT(DISTINCT EmpleadoID) AS TotalEmpleados,
                            ISNULL(SUM(Sueldo), 0) AS Sueldos,
                            ISNULL(SUM(PremioPuntualidad), 0) AS PremioPuntualidad,
                            ISNULL(SUM(PremioAsistencia), 0) AS PremioAsistencia,
                            ISNULL(SUM(Vacaciones), 0) AS Vacaciones,
                            ISNULL(SUM(PrimaVacacional), 0) AS PrimaVacacional,
                            ISNULL(SUM(Aguinaldo), 0) AS Aguinaldo,
                            ISNULL(SUM(PTU), 0) AS PTU,
                            ISNULL(SUM(ISRRetenido), 0) AS ISRRetenido,
                            ISNULL(SUM(IMSSObrero), 0) AS IMSSObrero,
                            ISNULL(SUM(Infonavit), 0) AS Infonavit,
                            ISNULL(SUM(Fonacot), 0) AS Fonacot,
                            ISNULL(SUM(IMSSPatronal), 0) AS IMSSPatronal,
                            ISNULL(SUM(SARPatronal), 0) AS SARPatronal,
                            ISNULL(SUM(InfonavitPatronal), 0) AS InfonavitPatronal
                        FROM Nomina
                        WHERE FechaPago BETWEEN @FechaInicio AND @FechaFin
                        AND Estatus = 'PAGADA'";

                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resumen.TotalEmpleados = Convert.ToInt32(dr["TotalEmpleados"]);
                            resumen.Sueldos = Convert.ToDecimal(dr["Sueldos"]);
                            resumen.PremioPuntualidad = Convert.ToDecimal(dr["PremioPuntualidad"]);
                            resumen.PremioAsistencia = Convert.ToDecimal(dr["PremioAsistencia"]);
                            resumen.Vacaciones = Convert.ToDecimal(dr["Vacaciones"]);
                            resumen.PrimaVacacional = Convert.ToDecimal(dr["PrimaVacacional"]);
                            resumen.Aguinaldo = Convert.ToDecimal(dr["Aguinaldo"]);
                            resumen.PTU = Convert.ToDecimal(dr["PTU"]);
                            resumen.ISRRetenido = Convert.ToDecimal(dr["ISRRetenido"]);
                            resumen.IMSSObrero = Convert.ToDecimal(dr["IMSSObrero"]);
                            resumen.Infonavit = Convert.ToDecimal(dr["Infonavit"]);
                            resumen.Fonacot = Convert.ToDecimal(dr["Fonacot"]);
                            resumen.IMSSPatronal = Convert.ToDecimal(dr["IMSSPatronal"]);
                            resumen.SARPatronal = Convert.ToDecimal(dr["SARPatronal"]);
                            resumen.InfonavitPatronal = Convert.ToDecimal(dr["InfonavitPatronal"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error (ej: tabla no existe o no tiene todas las columnas)
                    // devolver resumen vacío
                }
            }

            return resumen;
        }
    }

    // Clase auxiliar para el resumen de nómina
    public class ResumenNomina
    {
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public int TotalEmpleados { get; set; }
    }
}
