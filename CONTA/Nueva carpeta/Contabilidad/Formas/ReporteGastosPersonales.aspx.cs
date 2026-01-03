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
    public partial class ReporteGastosPersonales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerEmpresas()
        {
            //MobileBO.ControlConfiguracion controlConfiguracion = new MobileBO.ControlConfiguracion();
            Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;

            List<object> listaElementos = new List<object>();
            listaElementos.Add(new { id = "", nombre = "CONSOLIDADO" });
            try
            {
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();
                if (empresas != null)
                {
                    foreach (Entity.Configuracion.Catempresa empresa in empresas)
                    {
                        object elemento = new { id = empresa.Empresaid, nombre = empresa.Descripcion };
                        listaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

    }

    public class ControladorReporteGastosPersonales : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));

            string cuentaInicial = parametros.Get("cuentaInicial").Replace("-", "");
            string cuentaFinal = parametros.Get("cuentaFinal").Replace("-", "");
            string empresaid = parametros.Get("empresaid");

            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
            if (empresa.Tables[0].Rows.Count == 0)
            {
                DataRow row = empresa.Tables[0].NewRow();
                row["Empresa"] = 0;
                row["Descripcion"] = "CONSOLIDADO";
                row["Rfc"] = "FAC060511TMA";
                row["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                empresa.Tables[0].Rows.Add(row);
            }

            empresa.Tables[0].Columns.Add("FechaInicial", typeof(DateTime));
            empresa.Tables[0].Columns.Add("FechaFinal", typeof(DateTime));
            empresa.Tables[0].Rows[0]["FechaInicial"] = fechaInicial;
            empresa.Tables[0].Rows[0]["FechaFinal"] = fechaFinal;

            DataSet DatosReporte = MobileBO.ControlContabilidad.TraerGastosPersonales((empresaid == null || empresaid == string.Empty ? null : empresaid), (cuentaInicial == null || cuentaInicial == string.Empty ? null : cuentaInicial), (cuentaInicial == null || cuentaInicial == string.Empty ? null : cuentaFinal), fechaInicial, fechaFinal);

            DataSet ds = new DataSet();
            ds.Tables.Add(empresa.Tables[0].Copy());

            DataTable tblDatos = DatosReporte.Tables[0].Copy();
            tblDatos.TableName = "DatosReporte";
            ds.Tables.Add(tblDatos);

            try
            {
                base.NombreReporte = "ReporteGastosPersonales";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteGastosPersonales.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}