using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteProyeccionComparativoPresupuestado : System.Web.UI.Page
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
                cuenta = controlContabilidad.TraerCatCuentasPorCuenta(values.Codigo, values.ID.Trim() == "" ? null: values.ID);

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
                listaCatCuentas = controlContabilidad.TraerCatCuentasPorDescripcion(values.Descripcion, values.ID.Trim() == "" ? null : values.ID);
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
        public static Entity.Response<object> TraerUltimaCuentaContable(string cuenta, string empresaid)
        {
            try
            {
                Entity.Contabilidad.Catcuenta CuentaInicial = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuenta, empresaid.Trim() == "" ? "FA764836-BB07-4EB3-9B30-2B69206174C2" : empresaid);
                if (CuentaInicial == null)
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "Error al consultar la cuenta inicial, reportalo a sistemas");
                }
                DataSet ds = MobileBO.ControlContabilidad.TraerUltimaCuentaContable(empresaid.Trim() == "" ? null : empresaid, CuentaInicial.Cuenta.Substring(0, (4 * CuentaInicial.Nivel)));
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

    


    public class ControladorReporteProyeccionComparativoPresupuestado : Base.Clases.BaseReportes
    {

        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string cuentaini = parametros.Get("cuentaini");
            string cuentafin = parametros.Get("cuentafin");
            string ejercicio = parametros.Get("ejercicio");
            int mes = int.Parse(parametros.Get("mes"));
            string empresaid = parametros.Get("empresaid");            
            int tiporeporte = Convert.ToInt32(parametros.Get("tiporeporte")); // 1=completo, 2=Sin cuentas personales solo operativo //3=Mensual y Acumulado
            int Formato = int.Parse(parametros.Get("Formato"));

            string sNombreReporte = "";
            string sRutaXML = "";
            DataSet logoempresa1 = new DataSet();
            DataSet logoempresa2 = new DataSet();
            byte[] logoFBytes = null;
            byte[] logoBBytes = null;
            cuentaini = Regex.Replace(cuentaini, "-", "", RegexOptions.IgnoreCase);
            cuentafin = Regex.Replace(cuentafin, "-", "", RegexOptions.IgnoreCase);

            DataSet empresa = new DataSet();
            
            if (empresaid != null && empresaid.Trim() != string.Empty)
            {
                Entity.Configuracion.Catempresa e = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : e.Empresaid));
            }
            else
            {
                
                empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(Guid.Empty.ToString());
            }
            DataTable empresaTable = empresa.Tables[0];
            empresaTable.Columns.Add("logof", typeof(byte[]));
            empresaTable.Columns.Add("logob", typeof(byte[]));


            int? codempresa = 0;
            if (empresa.Tables[0].Rows.Count == 0)
            {
                codempresa = null;
            }
            else
            {
                codempresa = Convert.ToInt32(empresa.Tables[0].Rows[0]["Empresa"]);
            }
            if (empresa.Tables[0].Rows.Count == 0)
            {
                logoempresa1 = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("A7D3E5A4-6508-483B-8A3D-0E379FF06755");
                logoempresa2 = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("FA764836-BB07-4EB3-9B30-2B69206174C2");
                logoFBytes = (byte[])logoempresa1.Tables[0].Rows[0]["Logo"];
                logoBBytes = (byte[])logoempresa2.Tables[0].Rows[0]["Logo"];
            }
            if (empresa.Tables[0].Rows.Count == 0)
            {
                DataRow row = empresa.Tables[0].NewRow();
                row["Empresa"] = 0;
                row["Descripcion"] = "CONSOLIDADO BALOR FINANCIERA Y FACTURCO";
                row["Rfc"] = "";
                row["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                row["logof"] = logoFBytes;
                row["logob"] = logoBBytes;
                empresa.Tables[0].Rows.Add(row);
            }
            
            DataSet ds = new DataSet();
            empresa.Tables[0].TableName="DatosEmpresa";
            empresa.Tables[0].Columns.Add("TipoReporte", typeof(string));
            empresa.Tables[0].Rows[0]["TipoReporte"] = tiporeporte;
            ds.Tables.Add(empresa.Tables[0].Copy());

            if(tiporeporte == 2)
            {
                DataSet DatosMensuales = MobileBO.ControlContabilidad.TraerInformeProyeccionMensual(cuentaini, cuentafin, ejercicio, codempresa, mes, tiporeporte);
                if (Formato == 1)
                {
                    Formato = 10;
                }

                DataTable tblDatosMensuales = DatosMensuales.Tables[0].Copy();
                tblDatosMensuales.TableName = "Datos";
                ds.Tables.Add(tblDatosMensuales);
                sNombreReporte = "rptinformeProyeccionMensual";
                //sRutaXML = "c:\\Reportes\\rptinformeProyeccionMensual.xml";
                sRutaXML = "c:\\Reportes\\InformeProyeccionComparativoEntreYEars2.xml";
            }
            else
            {
                DataSet DatosReporte = MobileBO.ControlContabilidad.TraerInformeProyeccionComparativoEntreYEars2(cuentaini, cuentafin, ejercicio, codempresa, mes, tiporeporte);

                DataTable tblDatos = DatosReporte.Tables[0].Copy();
                tblDatos.TableName = "Datos";
                ds.Tables.Add(tblDatos);
                sNombreReporte = "rptinformeProyeccionComparativo2";
                sRutaXML = "c:\\Reportes\\InformeProyeccionComparativoEntreYEars2.xml";
            }

            try
            {
                base.NombreReporte = sNombreReporte;
                base.FormatoReporte = Formato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml(sRutaXML, System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}