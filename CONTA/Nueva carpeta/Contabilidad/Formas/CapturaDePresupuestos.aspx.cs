using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaDePresupuestos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Operacion.Catpresupuestosoperacion>> TraerPresupuestos(string empresa, int anio, string vendedor)
        {
            try
            {
                List<Entity.Operacion.Catpresupuestosoperacion> lst = MobileBO.ControlOperacion.TraerCatpresupuestosoperacion(anio,empresa,null,vendedor);

                return Entity.Response<List<Entity.Operacion.Catpresupuestosoperacion>>.CrearResponse<List<Entity.Operacion.Catpresupuestosoperacion>>(true, lst);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Operacion.Catpresupuestosoperacion>>.CrearResponseVacio<List<Entity.Operacion.Catpresupuestosoperacion>>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> GuardarPropuestas(string value)
        {
            try
            {
                List<Entity.Operacion.Catpresupuestosoperacion> lst = MobileBO.Utilerias.Deserializar<List<Entity.Operacion.Catpresupuestosoperacion>>(value);
                Entity.Analisis.Catvendedor vendedor = MobileBO.ControlAnalisis.TraerCatvendedores(lst[0].Vendedorid);


                foreach (Entity.Operacion.Catpresupuestosoperacion item in lst) {
                    item.Gerenteid = vendedor.Gerenteid;
                    item.Fecha = DateTime.Now;
                }
                    

                    
                
                MobileBO.ControlOperacion.GuardarCatpresupuestosoperacion(lst);

                return Entity.Response<object>.CrearResponse<object>(true, new { });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }



    }
}