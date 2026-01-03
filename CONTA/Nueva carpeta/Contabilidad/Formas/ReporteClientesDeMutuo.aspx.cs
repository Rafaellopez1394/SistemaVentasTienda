using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteClientesDeMutuo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteClientesMutuo : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaInicio = DateTime.Parse(parametros.Get("fecha"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fecha2"));
            int formato = int.Parse(parametros.Get("Formato").Split(',')[1]);

            try
            {
                DataSet dsempr = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                dsempr.Tables[0].TableName = "DatosEmpresa";

                DataSet ds = MobileBO.ControlOperacion.TraerCesionesPorFechaTipo(fechaInicio, fechaFinal, 2, empresaid);
                ds.Tables[0].TableName = "DatosReporte";

                ds.Tables.Add(dsempr.Tables[0].Copy());
                DataTable tDatos = new DataTable();
                tDatos.Columns.Add("FechaInicial", typeof(DateTime));
                tDatos.Columns.Add("FechaFinal", typeof(DateTime));
                tDatos.Rows.Add(tDatos.NewRow());
                tDatos.Rows[0]["FechaInicial"] = fechaInicio;
                tDatos.Rows[0]["FechaFinal"] = fechaFinal;
                tDatos.TableName = "Parametros";
                ds.Tables.Add(tDatos);

                base.NombreReporte = "reporteClientesMutuo";
                base.FormatoReporte = formato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\reporteClientesMutuo.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte
    }
}