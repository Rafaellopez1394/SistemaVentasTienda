using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReportePresupuestoDeGastos : System.Web.UI.Page
    {

        [WebMethod]
        public static Entity.Response<List<object>> TraerEmpresas()
        {
            Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;

            List<object> listaElementos = new List<object>();
            try
            {
                listaElementos.Add(new { id = "", nombre = "CONSOLIDADO" });
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

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReportePresupuestoVsReal : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            DateTime Fecha = DateTime.Parse(parametros.Get("Fecha"));
            string empresaid = parametros.Get("empresaid");
            int operativo = Convert.ToInt32(parametros.Get("formatoespecial"));

            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
            if (empresa.Tables[0].Rows.Count == 0)
            {
                DataRow row = empresa.Tables[0].NewRow();
                row["Empresa"] = 0;
                row["Descripcion"] = "CONSOLIDADO";
                row["Rfc"] = "";
                row["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                row["FechaEscritura"] = Fecha;
                empresa.Tables[0].Rows.Add(row);
            }
            else {
                empresa.Tables[0].Rows[0]["FechaEscritura"] = Fecha;
            }

            DataSet DatosReporte = MobileBO.ControlContabilidad.TraerPresupuestoContableVsReal((empresaid == null || empresaid == string.Empty ? null : empresaid), Fecha, operativo);

            DataSet ds = new DataSet();
            ds.Tables.Add(empresa.Tables[0].Copy());

            DataTable tblDatos = DatosReporte.Tables[0].Copy();
            tblDatos.TableName = "DatosReporte";
            ds.Tables.Add(tblDatos);
            try
            {
                base.NombreReporte = (int.Parse(parametros.Get("Formato")) == 3 ? "ReportePresupuestoContableVsReal": "ReportePresupuestoContableVsRealExcel") ;
                if (base.NombreReporte == "ReportePresupuestoContableVsRealExcel")
                {
                    base.FormatoReporte = 10;
                }
                else
                {
                    base.FormatoReporte = int.Parse(parametros.Get("Formato"));
                }
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReportePresupuestoContableVsReal.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}