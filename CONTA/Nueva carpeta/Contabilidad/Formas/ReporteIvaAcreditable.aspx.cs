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
    public partial class ReporteIvaAcreditable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteIvaAcreditable : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFinal = parametros.Get("fechaFinal");
            string empresaid = parametros.Get("empresaid");
            string sTipoImpresion = parametros.Get("TipoImpresion");
            int iFormato = int.Parse(sTipoImpresion);
            string empresaInicial, empresaFinal = "";
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            empresaInicial = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            empresaFinal = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            DataSet ds = new DataSet();
            ds = new MobileBO.ControlContabilidad().GeneraInformeIvaAcreditable(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaid);
            ds.Tables[0].TableName = "DatosReporte";
            ds.Tables[1].TableName = "DatosGlobales";

            DataTable dtEmpresa = new DataTable();
            dtEmpresa = empresa.Tables[0].Copy();
            dtEmpresa.TableName = "Empresa";
            ds.Tables.Add(dtEmpresa);

            DataTable dt = new DataTable();
            dt.TableName = "Parametros";
            dt.Columns.Add("fechaInicial", typeof(DateTime));
            dt.Columns.Add("fechaFinal", typeof(DateTime));
            DataRow row = dt.NewRow();
            row["fechaInicial"] = fechaInicial;
            row["fechaFinal"] = fechaFinal;
            dt.Rows.Add(row);
            ds.Tables.Add(dt);

            try
            {
                base.NombreReporte = "ReporteIvaAcreditable";
                base.FormatoReporte = iFormato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteIvaAcreditable.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }
}