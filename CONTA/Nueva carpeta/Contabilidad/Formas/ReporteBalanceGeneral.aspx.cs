using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteBalanceGeneral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
    public class ControladorReporteBalanceGeneral : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            bool firma = false;
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFinal = parametros.Get("fechaFinal");
            string empresaid = parametros.Get("empresaid");
            int Ingles = int.Parse(parametros.Get("Ingles"));
            if (parametros["firma"] != null)
            {
                firma = bool.Parse(parametros.Get("firma"));
            }
            bool cuentasDolares = (int.Parse(parametros.Get("cuentasDolares")) == 0 ? false : true);


            string empresaInicial, empresaFinal = "";
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            empresaInicial = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            empresaFinal = empresa.Tables[0].Rows[0]["Empresa"].ToString();

            DataSet ds = new DataSet();
            if (!cuentasDolares)
                ds = new MobileBO.ControlContabilidad().GeneraInformeBalanceGeneral(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaInicial, empresaFinal, Ingles);
            else
                ds = new MobileBO.ControlContabilidad().spcgenerainformebalancegeneralDolares(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaInicial, empresaFinal);

            try
            {
                DataTable firmaTable = new DataTable("Firma");
                firmaTable.Columns.Add("Firma", typeof(string));

                DataRow rowf = firmaTable.NewRow();
                rowf["Firma"] = firma;
                firmaTable.Rows.Add(rowf);

                ds.Tables.Add(firmaTable);

                if (!cuentasDolares)
                {
                    ds.Tables[0].TableName = "Comando";
                    ds.Tables[1].TableName = "Comando_1";
                    ds.Tables[2].TableName = "Comando_2";
                    ds.Tables[3].TableName = "Comando_3";
                    ds.Tables[4].TableName = "Comando_4";
                    ds.Tables[5].TableName = "Comando_5";
                    ds.Tables[6].TableName = "Comando_6";
                    ds.Tables[7].TableName = "Comando_7";
                    ds.Tables[8].TableName = "Comando_8";

                    base.NombreReporte = (int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? "ReporteBalanceGeneralExcel" : "ReporteBalanceGeneral");
                    base.FormatoReporte = 4;
                    base.FormatoReporteSQL = int.Parse(parametros.Get("Formato").Split(',')[1]);
                    base.ReporteContabilidad = (int)Entity.ReporteContabilidadFromServer.BalanceGeneral;
                    base.RutaReporte = "Contabilidad\\Reportes";
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteBalanceGeneral.xml", System.Data.XmlWriteMode.WriteSchema);
                }
                else
                {
                    DataTable firmaTables = new DataTable("Firma");
                    firmaTables.Columns.Add("Firma", typeof(string));

                    DataRow row1 = firmaTables.NewRow();
                    row1["Firma"] = firma;
                    firmaTables.Rows.Add(row1);

                    DataSet dsFinal = new DataSet();

                    DataTable tblEmpresa = empresa.Tables[0].Copy();
                    tblEmpresa.TableName = "Empresa";

                    DataTable tblParametros = new DataTable();
                    tblParametros.Columns.Add("FechaInicial", typeof(DateTime));
                    tblParametros.Columns.Add("FechaFinal", typeof(DateTime));
                    DataRow row = tblParametros.NewRow();
                    row["FechaInicial"] = DateTime.Parse(fechaInicial);
                    row["FechaFinal"] = DateTime.Parse(fechaFinal);
                    tblParametros.Rows.Add(row);
                    tblParametros.TableName = "Parametros";

                    dsFinal.Tables.Add(tblEmpresa.Copy());
                    dsFinal.Tables.Add(tblParametros.Copy());
                    dsFinal.Tables.Add(ds.Tables[0].Copy());
                    dsFinal.Tables.Add(ds.Tables[1].Copy());
                    dsFinal.Tables.Add(ds.Tables[2].Copy());

                    dsFinal.Tables.Add(firmaTables);

                    base.NombreReporte = "ReporteBalanceGeneralDolares";
                    base.FormatoReporte = (int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? 1 : 3);
                    base.RutaReporte = "Contabilidad\\Reportes";
                    base.DataSource = dsFinal;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteBalanceGeneralDolares.xml", System.Data.XmlWriteMode.WriteSchema);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte

    }
}