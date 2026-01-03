using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ExportaContabilidadExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteExportaContabilidadExcel : Base.Clases.BaseReportes
    {
        #region
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE

            string Empresaid = parametros.Get("empresaid").ToString();
            DateTime dFechaInicio = Convert.ToDateTime( parametros.Get("fechaInicial").ToString());
            DateTime dFechaFin = Convert.ToDateTime(parametros.Get("fechaFinal").ToString());
            int TipoImpresion = int.Parse( parametros.Get("Impresion").ToString());

            try
            {
                DataSet ds = new DataSet();

                DataColumn dcFechaInicio = new DataColumn("FechaInicio", typeof(DateTime));
                DataColumn dcFechaFin = new DataColumn("FechaFin", typeof(DateTime));

                DataTable tableEmpresa = new DataTable();
                DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(Empresaid);

                empresa.Tables[0].Columns.Add(dcFechaInicio);
                empresa.Tables[0].Columns.Add(dcFechaFin);
                empresa.Tables[0].Rows[0]["FechaInicio"] = dFechaInicio;
                empresa.Tables[0].Rows[0]["FechaFin"] = dFechaFin;

                tableEmpresa = empresa.Tables[0].Copy();
                tableEmpresa.TableName = "Empresa";

                DataSet dsDatosConta = MobileBO.ControlContabilidad.ReporteContabilidadExcelDS(Empresaid, dFechaInicio, dFechaFin);

                DataTable tabla = new DataTable();
                tabla = dsDatosConta.Tables[0].Copy();
                tabla.TableName = "ExportaContabilidadExcel";

                ds.Tables.Add(tabla);
                ds.Tables.Add(tableEmpresa);

                base.FormatoReporte = TipoImpresion;
                base.NombreReporte = "RptExportaContabilidadExcel";
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ContabilidadExcel.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        #endregion
    }
}