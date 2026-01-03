using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteDIOT : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<object> PeriodoActual()
        {
            try
            {
                DateTime hoy = new DateTime();
                hoy = DateTime.Today.AddMonths(-1);

                DateTime IniciaPeriodo = new DateTime(hoy.Year, hoy.Month, 1);
                DateTime TerminaPeriodo = IniciaPeriodo.AddMonths(1).AddDays(-1);
                string Periodo = IniciaPeriodo.ToString("dd/MM/yyyy") + " - " + TerminaPeriodo.ToString("dd/MM/yyyy");
                return Entity.Response<object>.CrearResponse<object>(true, new { IniciaPeriodo = IniciaPeriodo.ToShortDateString(), TerminaPeriodo = TerminaPeriodo.ToShortDateString(), txtPeriodo = Periodo, esMayor = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> PeriodoAnterior(string IniciaActual)
        {
            try
            {
                DateTime fecha = DateTime.Parse(IniciaActual);
                DateTime IniciaPeriodo = fecha.AddMonths(-1);
                DateTime TerminaPeriodo = fecha.AddDays(-1);
                string Periodo = IniciaPeriodo.ToString("dd/MM/yyyy") + " - " + TerminaPeriodo.ToString("dd/MM/yyyy");
                bool isMayor = false; //(TerminaPeriodo > DateTime.Today ? true : false);
                return Entity.Response<object>.CrearResponse<object>(true, new { IniciaPeriodo = IniciaPeriodo.ToShortDateString(), TerminaPeriodo = TerminaPeriodo.ToShortDateString(), txtPeriodo = Periodo, esMayor = isMayor });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> PeriodoPosterior(string TerminaActual)
        {
            try
            {

                DateTime fecha = DateTime.Parse(TerminaActual);
                DateTime IniciaPeriodo = fecha.AddDays(1);
                DateTime TerminaPeriodo = IniciaPeriodo.AddMonths(1).AddDays(-1);
                string Periodo = IniciaPeriodo.ToString("dd/MM/yyyy") + " - " + TerminaPeriodo.ToString("dd/MM/yyyy");
                bool isMayor = false; //(TerminaPeriodo > DateTime.Today ? true : false);
                return Entity.Response<object>.CrearResponse<object>(true, new { IniciaPeriodo = IniciaPeriodo.ToShortDateString(), TerminaPeriodo = TerminaPeriodo.ToShortDateString(), txtPeriodo = Periodo, esMayor = isMayor });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<string> GenerarDIOT(string empresaid, string iniciaPeriodo, string terminaPeriodo)
        {
            try
            {
                DateTime fi = DateTime.Parse(iniciaPeriodo);
                DateTime ff = DateTime.Parse(terminaPeriodo);
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlAnalisis.TraerEmpresa(empresaid);

                string archivo = MobileBO.DIOT.GenerarReporteDiotCompleto(empresa.Empresaid, null, fi, ff, empresa.Rfc);
                
                return Entity.Response<string>.CrearResponse<string>(true, "http://192.168.10.238/balor/ArchivosExcel/" + archivo);
            }
            catch (Exception ex)
            {
                return Entity.Response<string>.CrearResponseVacio<string>(false, ex.Message);
            }
        }
    }
}