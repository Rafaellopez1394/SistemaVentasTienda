using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteConciliacionIvaPorPagar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteReporteConciliacionIvaPorPagar : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaini = DateTime.Parse(parametros.Get("fechaini"));
            DateTime fechafin = DateTime.Parse(parametros.Get("fechafin"));


            bool incluirDemandasCaducadas = true;
            bool ajustecontable = true;
            bool incluirDemandasPagadas = true;

            Entity.Configuracion.Catempresa _empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

            if (empresaid != null)
            {
                if (empresaid.Trim() == "" || empresaid.Trim() == "*")
                {
                    empresaid = null;
                }
            }

            try
            {


                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

                
                ds = MobileBO.ControlCartera.ConciliacionIvaPorPagar(fechaini, fechafin, empresaid);
                ds2 = new MobileBO.ControlContabilidad().GeneraInformeIvaAcreditable(fechaini, fechafin, empresaid);

                decimal _ivaAcreditableFiscalMensual = Convert.ToDecimal(ds.Tables[0].Rows[0]["IvaAcreditableFiscalMensual"]);
                decimal _prorateoAcreditable = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Acreditable"]);

                ds.Tables[0].Rows[0]["IvaPorAcreditar"] = _ivaAcreditableFiscalMensual * _prorateoAcreditable;

                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresa.Empresaid);
                dsEmpresa.Tables[0].TableName = "tblEmpresa";
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());

                DataTable DatosReporte = new DataTable("DatosReporte");
                DatosReporte.Columns.Add("FechaIni", typeof(DateTime));
                DatosReporte.Columns.Add("FechaFin", typeof(DateTime));
                DataRow rd = DatosReporte.NewRow();
                rd["FechaIni"] = fechaini;
                rd["FechaFin"] = fechafin;
                DatosReporte.Rows.Add(rd);
                ds.Tables.Add(DatosReporte);

                base.NombreReporte = "ReporteConciliacionIvaPorPagar";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteConciliacionIvaPorPagar.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {

            }

        }
        #endregion
    }

}