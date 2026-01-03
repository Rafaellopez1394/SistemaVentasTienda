using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteAuxiliarMayor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByCode(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuenta = controlContabilidad.TraerCatCuentasPorCuenta(values.Codigo, values.ID);

                if (cuenta != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Cuentaid, Codigo = cuenta.Cuenta, Descripcion = cuenta.Descripcion };
                    ListaElementos.Add(elemento);
                }
                else
                {
                    return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, "No se encontró resultado.");
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByPopUp(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaCatCuentas = controlContabilidad.TraerCatCuentasPorDescripcion(values.Descripcion, values.ID);
                if (listaCatCuentas != null)
                {
                    foreach (Entity.Contabilidad.Catcuenta catCuenta in listaCatCuentas)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = catCuenta.Cuentaid, Codigo = catCuenta.Cuenta, Descripcion = catCuenta.Descripcion };
                        ListaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> TraerUltimaCuentaContable(string cuenta, string empresaid) {
            try
            {
                Entity.Contabilidad.Catcuenta CuentaInicial = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuenta, empresaid);
                if (CuentaInicial == null)
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "Error al consultar la cuenta inicial, reportalo a sistemas");
                }
                DataSet ds = MobileBO.ControlContabilidad.TraerUltimaCuentaContable(empresaid, CuentaInicial.Cuenta.Substring(0, (4 * CuentaInicial.Nivel)));
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Cuentaid = ds.Tables[0].Rows[0]["CuentaID"].ToString(), Cuenta = ds.Tables[0].Rows[0]["Cuenta"].ToString(), Descripcion = ds.Tables[0].Rows[0]["Descripcion"].ToString() });
                else
                    return Entity.Response<object>.CrearResponse<object>(true, new { Cuentaid = CuentaInicial.Cuentaid, Cuenta = CuentaInicial.Cuenta, Descripcion = CuentaInicial.Descripcion });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }



    }

    public class ControladorReporteAuxiliarMayor : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFinal = parametros.Get("fechaFinal");
            string cuentaInicial = parametros.Get("cuentaInicial").Replace("-", "");
            string cuentaFinal = parametros.Get("cuentaFinal").Replace("-", "");
            int Ingles = int.Parse(parametros.Get("Ingles"));
            string empresaid = parametros.Get("empresaid");
            string empresaInicial, empresaFinal = "";
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            empresaInicial = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            empresaFinal = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            DataSet ds = new DataSet();
            ds = new MobileBO.ControlContabilidad().GeneraInformeAuxiliarMayor(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaInicial, cuentaInicial, cuentaFinal, Ingles);
            try
            {
                base.NombreReporte = (int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? "ReporteAuxiliarMayorExcel" : "ReporteAuxiliarMayor");
                base.FormatoReporte = 4;
                base.FormatoReporteSQL = int.Parse(parametros.Get("Formato").Split(',')[1]);
                base.ReporteContabilidad = (int)Entity.ReporteContabilidadFromServer.AuxiliarMayor;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }
}