using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteCobranzaPorVendedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteCobranzaPorVendedor : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaInicio = DateTime.Parse(parametros.Get("fecha"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fecha2"));
            int tiporeporte = 4;
            string equipoventa = "";
            string clienteini = "";
            string clientefin = "";
            string vendedorini = parametros.Get("vendedorini");
            string vendedorfin = parametros.Get("vendedorfin");

            string paramEquipoventa = equipoventa != string.Empty ? equipoventa : null;
            int? paramClienteini = clienteini != string.Empty ? int.Parse(clienteini) : int.MinValue;
            int? paramClientefin = clientefin != string.Empty ? int.Parse(clientefin) : int.MinValue;

            int? paramVendedorini = vendedorini != string.Empty ? int.Parse(vendedorini) : int.MinValue;
            int? paramVendedorfin = vendedorfin != string.Empty ? int.Parse(vendedorfin) : int.MinValue;

            try
            {
                DataSet ds = new DataSet();
                DataSet dsempr = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                dsempr.Tables[0].TableName = "DatosEmpresa";

                ds = MobileBO.ControlOperacion.TraerReporteCobranza(empresaid, fechaInicio, fechaFinal, paramEquipoventa, paramVendedorini, paramVendedorfin, paramClienteini, paramClientefin);
                ds.Tables[0].Columns.Add("DiasDescuento", typeof(int));
                ds.Tables[0].Columns.Add("ComisionPagar", typeof(decimal));

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    r["DiasDescuento"] = (int.Parse(r["DiasVencido"].ToString()) > 0 ? (int.Parse(r["DiasVencido"].ToString()) * 2) : 0);
                    decimal comisionpagar = (decimal.Parse(r["Comision"].ToString()) / int.Parse(r["Dias"].ToString())) * (int.Parse(r["Dias"].ToString()) - int.Parse(r["DiasDescuento"].ToString()));
                    r["ComisionPagar"] = (comisionpagar < 0m ? 0m : comisionpagar);
                }


                ds.Tables[0].TableName = "DatosReporte";
                ds.Tables.Add(dsempr.Tables[0].Copy());
                DataTable tfecha = new DataTable("Fecha");
                tfecha.Columns.Add("FechaInicial", typeof(DateTime));
                tfecha.Columns.Add("FechaFinal", typeof(DateTime));
                tfecha.Columns.Add("TipoReporte", typeof(int));
                tfecha.Columns.Add("TituloReporte", typeof(string));
                tfecha.Rows.Add(tfecha.NewRow());
                tfecha.Rows[0]["FechaInicial"] = fechaInicio;
                tfecha.Rows[0]["FechaFinal"] = fechaFinal;
                tfecha.Rows[0]["TipoReporte"] = tiporeporte;
                tfecha.Rows[0]["TituloReporte"] = "Reporte de cartera por documento al ";
                tfecha.TableName = "DatosFecha";
                ds.Tables.Add(tfecha);

                base.NombreReporte = "rptCobranzaPorVendedor";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                //base.DataSource.WriteXml("c:\\Reportes\\ReporteCobranzaPorVendedor.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte
    }
}