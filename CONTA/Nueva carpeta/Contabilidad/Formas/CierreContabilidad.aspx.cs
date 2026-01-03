using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CierreContabilidad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Cierrecontabilidad> TraerFechaCierre(string empresaid)
        {
            try
            {
                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(empresaid);

                return Entity.Response<Entity.Contabilidad.Cierrecontabilidad>.CrearResponse<Entity.Contabilidad.Cierrecontabilidad>(true, cierre);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Cierrecontabilidad>.CrearResponseVacio<Entity.Contabilidad.Cierrecontabilidad>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Cierrecontabilidad> GuardarFechaCierre(string empresaid, string fecha, string usuario)
        {
            try
            {
                Entity.Contabilidad.Cierrecontabilidad cierre;

                cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(empresaid);

                if (cierre != null)
                {
                    cierre.Fechacierre = DateTime.Parse(fecha);
                    cierre.Usuario = usuario;
                }
                else
                {
                    cierre = new Entity.Contabilidad.Cierrecontabilidad();
                    cierre.Empresaid = empresaid;
                    cierre.Fechacierre = DateTime.Parse(fecha);
                    cierre.Fecha = DateTime.Today;
                    cierre.Estatus = 1;
                    cierre.Usuario = usuario;
                    cierre.UltimaAct = 0;
                }

                MobileBO.ControlContabilidad.GuardarCierrecontabilidad(new List<Entity.Contabilidad.Cierrecontabilidad>() { cierre });

                return Entity.Response<Entity.Contabilidad.Cierrecontabilidad>.CrearResponse<Entity.Contabilidad.Cierrecontabilidad>(true, cierre);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Cierrecontabilidad>.CrearResponseVacio<Entity.Contabilidad.Cierrecontabilidad>(false, ex.Message);
            }
        }
    }
}