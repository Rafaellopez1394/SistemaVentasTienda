using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class TasasBancarias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Catcetestiie>> ConsultaTasas(int anio)
        {
            try
            {
                List<Entity.Contabilidad.Catcetestiie> lstTasas = MobileBO.ControlContabilidad.TraerCatcetestiiePorAnio(anio);

                return Entity.Response<List<Entity.Contabilidad.Catcetestiie>>.CrearResponse<List<Entity.Contabilidad.Catcetestiie>>(true, lstTasas);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Catcetestiie>>.CrearResponseVacio<List<Entity.Contabilidad.Catcetestiie>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Catcetestiie>> GuardarTasas(string value)
        {
            try
            {
                List<Entity.Contabilidad.Catcetestiie> lstTasas;
                lstTasas = MobileBO.Utilerias.Deserializar<List<Entity.Contabilidad.Catcetestiie>>(value);
                foreach (Entity.Contabilidad.Catcetestiie l in lstTasas)
                {
                    l.Fecha = DateTime.Now;
                }
                MobileBO.ControlContabilidad.GuardarCatcetestiie(lstTasas);

                return Entity.Response<List<Entity.Contabilidad.Catcetestiie>>.CrearResponse<List<Entity.Contabilidad.Catcetestiie>>(true, lstTasas);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Catcetestiie>>.CrearResponseVacio<List<Entity.Contabilidad.Catcetestiie>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<bool> FaltaCapturaTiie()
        {
            try
            {
                bool _capturado;
                List<Entity.Contabilidad.Catcetestiie> lstTasas = MobileBO.ControlContabilidad.TraerCatcetestiiePorAnio(DateTime.Now.Year);
                _capturado = !(lstTasas.Where(x => x.Mes == DateTime.Now.Month && x.Tiie > 0).Count() > 0);

                return Entity.Response<bool>.CrearResponse<bool>(true, _capturado);
            }
            catch (Exception ex)
            {
                return Entity.Response<bool>.CrearResponseVacio<bool>(false, ex.Message);
            }
        }
    }
}