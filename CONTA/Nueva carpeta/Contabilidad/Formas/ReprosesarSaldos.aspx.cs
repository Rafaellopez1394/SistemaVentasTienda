using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReprosesarSaldos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<string> ReprosesaSaldos(string Empresaid)
        {
            try
            {
                new MobileBO.ControlContabilidad().ReprosesarSaldos(Empresaid);
                return Entity.Response<string>.CrearResponse<string>(true, "Reproseso Terminado");
            }
            catch (Exception ex)
            {
                return Entity.Response<string>.CrearResponseVacio<string>(false, ex.Message);
            }
        }
    }
}