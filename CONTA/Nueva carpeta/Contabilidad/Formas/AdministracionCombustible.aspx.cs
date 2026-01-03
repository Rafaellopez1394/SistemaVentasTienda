using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class AdministracionCombustible : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.CRMWeb.CrmCatvendedorescombustible>> TraerVendedoresCombustible(string vendedorid, int? estatus)
        {
            try
            {
                if (vendedorid.Trim() == "")
                {
                    vendedorid = null;
                }
                if (estatus == -1)
                {
                    estatus = null;
                }
                List<Entity.CRMWeb.CrmCatvendedorescombustible> _vendedores = new List<Entity.CRMWeb.CrmCatvendedorescombustible>();
                _vendedores = MobileBO.ControlCRMWeb.TraerCrmCatvendedorescombustible(null, vendedorid, estatus);
                foreach (Entity.CRMWeb.CrmCatvendedorescombustible _vendedor in _vendedores)
                {
                    _vendedor.Fechaaltastring = _vendedor.Fechaalta != null ? string.Format("{0:yyyy/MM/dd hh:mm}", _vendedor.Fechaalta) : "";
                    _vendedor.Fechabajastring = _vendedor.Fechabaja != null ? string.Format("{0:yyyy/MM/dd hh:mm}", _vendedor.Fechabaja) : "";
                }
                return Entity.Response<List<Entity.CRMWeb.CrmSolicitudaltacombustible>>.CrearResponse(true, _vendedores);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.CRMWeb.CrmCatvendedorescombustible>>.CrearResponseVacio<List<Entity.CRMWeb.CrmCatvendedorescombustible>>(false, ex.Message);
            }
        }

        [WebMethod]        
        public static Entity.Response<object> BajaVendedor(string vendedorid, string usuario)
        {
            try
            {
                List<Entity.CRMWeb.CrmCatvendedorescombustible> _vendedores = MobileBO.ControlCRMWeb.TraerCrmCatvendedorescombustible(null, vendedorid, null);
                if (_vendedores.Count>0)
                {
                    _vendedores[0].Estatus = 2;
                    _vendedores[0].Fechabaja = DateTime.Now;
                    _vendedores[0].Fecha = DateTime.Now;
                    _vendedores[0].Usuario = usuario;
                    _vendedores[0].Usuariobaja = usuario;
                    MobileBO.ControlCRMWeb.GuardarCrmCatvendedorescombustible(new List<Entity.CRMWeb.CrmCatvendedorescombustible>() { _vendedores[0] });
                    return Entity.Response<object>.CrearResponse<object>(true, "");
                }
                else
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "No se encontró el vendedor");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> ActivarVendedor(string vendedorid, string usuario)
        {
            try
            {
                List<Entity.CRMWeb.CrmCatvendedorescombustible> _vendedores = MobileBO.ControlCRMWeb.TraerCrmCatvendedorescombustible(null, vendedorid, null);
                if (_vendedores.Count > 0)
                {
                    _vendedores[0].Estatus = 1;
                    _vendedores[0].Fechaalta = DateTime.Now;
                    _vendedores[0].Fecha = DateTime.Now;
                    _vendedores[0].Usuario = usuario;
                    _vendedores[0].Usuarioalta = usuario;
                    MobileBO.ControlCRMWeb.GuardarCrmCatvendedorescombustible(new List<Entity.CRMWeb.CrmCatvendedorescombustible>() { _vendedores[0] });
                    return Entity.Response<object>.CrearResponse<object>(true, "");
                }
                else
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "No se encontró el vendedor");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
    }
}