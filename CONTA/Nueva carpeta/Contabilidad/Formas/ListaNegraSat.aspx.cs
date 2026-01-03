using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ListaNegraSat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<object>> ConsultarContribuyentes(bool solobalor, string nombre, string rfc)
        {
            try
            {
                List<object> result = new List<object>();

                DataSet elementos = MobileBO.ControlContabilidad.TraerCatsatlistanegracontribuyentesGridDS(solobalor, nombre, rfc);

                foreach (DataRow row in elementos.Tables[0].Rows)
                {
                    object obj = new
                    {
                        Numero = row["Numero"].ToString(),
                        Rfc = row["Rfc"].ToString(),
                        Nombre = row["Nombre"].ToString(),
                        Situacion = row["Situacion"].ToString(),
                        ProveedorID = row["ProveedorID"] == DBNull.Value ? "" : row["ProveedorID"].ToString(),
                        Facturas = Convert.ToInt32(row["Facturas"].ToString())

                    };
                    result.Add(obj);
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, result);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> ConsultarPolizasProveedor(string proveedorid)
        {
            try
            {
                List<object> result = new List<object>();

                DataSet elementos = MobileBO.ControlContabilidad.TraerPolizasProveedorEnListaNegra(proveedorid);

                foreach (DataRow row in elementos.Tables[0].Rows)
                {
                    object obj = new
                    {

                        Tip_pol = row["Tip_pol"].ToString(),
                        num_pol = row["num_pol"].ToString(),
                        Fec_Pol = row["Fec_Pol"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["Fec_Pol"])),
                        ImportePoliza = Convert.ToDecimal(row["ImportePoliza"]).ToString("N2"),
                        Concepto = row["Concepto"].ToString(),
                        EmisorRFC = row["EmisorRFC"].ToString(),
                        EmisorNombre = row["EmisorNombre"].ToString(),
                        ReceptorRFC = row["ReceptorRFC"].ToString(),
                        ReceptorNombre = row["ReceptorNombre"].ToString(),
                        UUID = row["UUID"].ToString(),
                        FechaFactura = row["FechaFactura"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["FechaFactura"])),
                        Serie = row["Serie"].ToString(),
                        Factura = row["Factura"].ToString(),
                        total = Convert.ToDecimal(row["total"]).ToString("N2"),
                        ProveedorID = row["ProveedorID"].ToString(),


                    };
                    result.Add(obj);
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, result);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> ConsultarDetalle(int numero)
        {
            try
            {
                List<object> result = new List<object>();
                
                DataSet contribuyentes = MobileBO.ControlContabilidad.TraerCatsatlistanegracontribuyentesDS(numero);
                               

                
                // solo uno
                foreach (DataRow row in contribuyentes.Tables[0].Rows)
                {
                    object obj = new
                    {

                        Numero = row["Numero"].ToString(),
                        RFC = row["RFC"].ToString(),
                        Nombre = row["Nombre"].ToString(),
                        Situacion = row["Situacion"].ToString(),
                        NumeroFechaPresuncionSat = row["NumeroFechaPresuncionSat"].ToString(),
                        PublicacionSatPresuntos = row["PublicacionSatPresuntos"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionSatPresuntos"])),
                        NumeroFechaPresuncionDof = row["NumeroFechaPresuncionDof"].ToString(),
                        PublicacionDofPresuntos = row["PublicacionDofPresuntos"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionDofPresuntos"])),

                        NumeroFechaDesvirtuaronSat = row["NumeroFechaDesvirtuaronSat"].ToString(),
                        PublicacionSatDesvirtuados = row["PublicacionSatDesvirtuados"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionSatDesvirtuados"])),
                        NumeroFechaDesvirtuaronDof = row["NumeroFechaDesvirtuaronDof"].ToString(),
                        PublicacionDofDesvirtuados = row["PublicacionDofDesvirtuados"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionDofDesvirtuados"])),

                        NumeroFechaDefinitivosSat = row["NumeroFechaDefinitivosSat"].ToString(),
                        PublicacionSatDefinitivos = row["PublicacionSatDefinitivos"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionSatDefinitivos"])),
                        NumeroFechaDefinitivosDof = row["NumeroFechaDefinitivosDof"].ToString(),
                        PublicacionDofDefinitivos = row["PublicacionDofDefinitivos"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionDofDefinitivos"])),

                        NumeroFechaSentenciaFavorableSat = row["NumeroFechaSentenciaFavorableSat"].ToString(),
                        PublicacionSatSentenciaFavorable = row["PublicacionSatSentenciaFavorable"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionSatSentenciaFavorable"])),
                        NumeroFechaSentenciaFavorableDof = row["NumeroFechaSentenciaFavorableDof"].ToString(),
                        PublicacionDofSentenciaFavorable = row["PublicacionDofSentenciaFavorable"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["PublicacionDofSentenciaFavorable"])),


                        Fecha = row["Fecha"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(row["Fecha"]))

                       
                    };
                    result.Add(obj);
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, result);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> TraerSesionId()
        {
            try
            {
                Guid sesionNueva = Guid.NewGuid();

                var datos = new
                {
                    SesionId = sesionNueva

                };

                return Entity.Response<object>.CrearResponse<object>(true, datos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
    }
}