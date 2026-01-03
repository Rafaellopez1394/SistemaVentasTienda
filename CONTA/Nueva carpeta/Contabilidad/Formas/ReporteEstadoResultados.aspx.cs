using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteEstadoResultados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
    public class ControladorReporteEstadoResultados : Base.Clases.BaseReportes
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
            string empresaInicial, empresaFinal = "";
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            empresaInicial = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            empresaFinal = empresa.Tables[0].Rows[0]["Empresa"].ToString();

            DataSet ds = new DataSet();
            ds = new MobileBO.ControlContabilidad().GeneraInformeEstadoResultados(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaInicial, empresaFinal, Ingles);
            ds.Tables[0].TableName = "Comando";
            ds.Tables[1].TableName = "Comando_1";
            ds.Tables[2].TableName = "Comando_2";
            
            DataTable firmaTable = new DataTable("Firma");
            firmaTable.Columns.Add("Firma", typeof(string));
            
            DataRow row = firmaTable.NewRow();
            row["Firma"] = firma;
            firmaTable.Rows.Add(row);
            
            ds.Tables.Add(firmaTable);

            try
            {
                //base.NombreReporte = "ReporteEstadoResultados";
                //base.FormatoReporte = 4;
                base.NombreReporte = (int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? "ReporteEstadoResultadosExcel" : "ReporteEstadoResultados");
                base.FormatoReporte = 4;
                base.FormatoReporteSQL = int.Parse(parametros.Get("Formato").Split(',')[1]);
                base.ReporteContabilidad = (int)Entity.ReporteContabilidadFromServer.EstadoResultados;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteEstadoResultados.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }
}