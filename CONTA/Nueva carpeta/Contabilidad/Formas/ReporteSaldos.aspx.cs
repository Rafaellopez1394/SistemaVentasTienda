using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Homex.Core.Utilities;
using System.Globalization;
using System.Text;
using iTextSharp.text;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteSaldos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
    public class ControladorReporteSaldos : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFinal = parametros.Get("fechaFinal");
            string empresaid = parametros.Get("empresaid");

            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
            if (empresa.Tables[0].Rows.Count == 0)
            {
                DataRow row = empresa.Tables[0].NewRow();
                row["Empresa"] = 0;
                row["Descripcion"] = "CONSOLIDADO";
                row["Rfc"] = "";
                row["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                row["FechaEscritura"] = fechaInicial;
                empresa.Tables[0].Rows.Add(row);
            }
            else
            {
                empresa.Tables[0].Rows[0]["FechaEscritura"] = fechaInicial;
            }

            DataSet ds = new DataSet();
            ds = new MobileBO.ControlContabilidad().GeneraReporteSaldos(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), null);
            ds.Tables[0].TableName = "Datos";

            //Parametros del reporte
            DataTable tablaParametros = new DataTable();
            tablaParametros.TableName = "Parametros";
            tablaParametros.Columns.Add("FechaInicial", typeof(DateTime));
            tablaParametros.Columns.Add("FechaFinal", typeof(DateTime));
            tablaParametros.Columns.Add("EmpresaID", typeof(string));

            if (empresaid != null)
            {
                //Llenamos los parametros que le pasaremos al reporte
                DataRow row = tablaParametros.NewRow();
                row[0] = DateTime.Parse(fechaInicial);
                row[1] = DateTime.Parse(fechaFinal);
                row[2] = empresaid;

                tablaParametros.Rows.Add(row);
                ds.Tables.Add(tablaParametros);
            }
            try
            {
                if (empresaid == "fa764836-bb07-4eb3-9b30-2b69206174c2")
                {
                    base.NombreReporte = "ReporteSaldosBalor";
                    base.FormatoReporte = 3;
                    base.RutaReporte = "Contabilidad\\Reportes";
                    ds.Tables.Add(empresa.Tables[0].Copy());
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteSaldosBalor.xml", System.Data.XmlWriteMode.WriteSchema);
                }
                else if (empresaid == "a7d3e5a4-6508-483b-8a3d-0e379ff06755")
                {
                    base.NombreReporte = "ReporteSaldosFactur";
                    base.FormatoReporte = 3;
                    base.RutaReporte = "Contabilidad\\Reportes";
                    ds.Tables.Add(empresa.Tables[0].Copy());
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteSaldosFactur.xml", System.Data.XmlWriteMode.WriteSchema);
                }
                else
                {
                    base.NombreReporte = "ReporteSaldos";
                    base.FormatoReporte = 3;
                    base.RutaReporte = "Contabilidad\\Reportes";
                    ds.Tables.Add(empresa.Tables[0].Copy());
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteSaldos.xml", System.Data.XmlWriteMode.WriteSchema);
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