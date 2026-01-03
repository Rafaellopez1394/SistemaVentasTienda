using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReportePolizasMasivo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReportePolizaMasivo : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            DataSet ds = new DataSet();
            try
            {
                base.NombreReporte = "ReportePolizaMasivo";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                string EmpresaID = parametros.Get("EmpresaID");
                DateTime FechaInicial = DateTime.Parse(parametros.Get("FechaInicial"));
                DateTime FechaFinal = DateTime.Parse(parametros.Get("FechaFinal"));
                string TipPol = parametros.Get("TipPol");
                string polizaInicial = parametros.Get("PolizaInicial");
                string polizaFinal = parametros.Get("PolizaFinal");
                polizaInicial = polizaInicial == "" ? null : polizaInicial;
                polizaFinal = polizaFinal == "" ? null : polizaFinal;
                ds = MobileBO.ControlContabilidad.ReporteCapturaPolizasMasivo(EmpresaID, FechaInicial, FechaFinal, TipPol, 0, polizaInicial, polizaFinal);

                DataTable tablaParametros = new DataTable();
                tablaParametros.TableName = "Parametros";
                tablaParametros.Columns.Add("FechaInicial", typeof(DateTime));
                tablaParametros.Columns.Add("FechaFinal", typeof(DateTime));
                tablaParametros.Columns.Add("FechaInicialTexto", typeof(string));
                tablaParametros.Columns.Add("FechaFinalTexto", typeof(string));
                   //Llenamos los parametros que le pasaremos al reporte
                    DataRow row = tablaParametros.NewRow();
                    row[0] = FechaInicial;
                    row[1] = FechaFinal;
                    row[2] = FechaInicial.ToString("dd DE MMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper();
                    row[3] = FechaFinal.ToString("dd DE MMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper();
                    tablaParametros.Rows.Add(row);
                    ds.Tables.Add(tablaParametros);
               

                base.DataSource = ds;
                //base.DataSource.WriteXml("c:\\Reportes\\ReportePolizaAnticipo.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}