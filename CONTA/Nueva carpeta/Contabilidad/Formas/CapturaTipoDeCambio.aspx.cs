using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaTipoDeCambio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Cattipocambio> ConsultarTipoCambio(string fecha)
        {
            try
            {
                Entity.Contabilidad.Cattipocambio TipoCambio = MobileBO.ControlContabilidad.TraerCattipocambio(DateTime.Parse(fecha));
                if (TipoCambio == null)
                    TipoCambio = new Entity.Contabilidad.Cattipocambio();
                return Entity.Response<Entity.Contabilidad.Cattipocambio>.CrearResponse<Entity.Contabilidad.Cattipocambio>(true, TipoCambio);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Cattipocambio>.CrearResponseVacio<Entity.Contabilidad.Cattipocambio>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Cattipocambio>> GuardarTipoCambio(string value)
        {
            try
            {
                List<Entity.Contabilidad.Cattipocambio> ListaTipoCambio = MobileBO.Utilerias.Deserializar<List<Entity.Contabilidad.Cattipocambio>>(value);

                List<Entity.Contabilidad.Cattipocambio> ListaTipoCambioDepurada = (from a in ListaTipoCambio
                                                                                   group a by new
                                                                                   {
                                                                                       Fechatipocambio = a.Fechatipocambio,
                                                                                       Usuario = a.Usuario
                                                                                   } into grupo
                                                                                   select new Entity.Contabilidad.Cattipocambio
                                                                                   {
                                                                                       Fechatipocambio = grupo.Key.Fechatipocambio,
                                                                                       Importetipocambio = grupo.Max(x => x.Importetipocambio),
                                                                                       Usuario = grupo.Key.Usuario,
                                                                                       Estatus = 1,
                                                                                       Fecha = DateTime.Now

                                                                                   }).ToList();



                foreach (Entity.Contabilidad.Cattipocambio item in ListaTipoCambioDepurada)
                {                    
                    Entity.Contabilidad.Cattipocambio aux = MobileBO.ControlContabilidad.TraerCattipocambio(DateTime.Parse(item.Fechatipocambio));
                    if (aux != null)
                        item.UltimaAct = aux.UltimaAct;
                }
                MobileBO.ControlContabilidad.GuardarCattipocambio(ListaTipoCambioDepurada);
                return Entity.Response<List<Entity.Contabilidad.Cattipocambio>>.CrearResponse<List<Entity.Contabilidad.Cattipocambio>>(true, ListaTipoCambio);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Cattipocambio>>.CrearResponseVacio<List<Entity.Contabilidad.Cattipocambio>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Cattipocambio>> GuardarTipoCambioBM(string value)
        {
            try
            {
                List<Entity.Contabilidad.Cattipocambio> ListaTipoCambio = MobileBO.Utilerias.Deserializar<List<Entity.Contabilidad.Cattipocambio>>(value);

                List<Entity.Contabilidad.Cattipocambio> ListaTipoCambioDepurada = (from a in ListaTipoCambio
                                                                                   group a by new
                                                                                   {
                                                                                       Fechatipocambio = a.Fechatipocambio,
                                                                                       Usuario = a.Usuario,
                                                                                       Tipo = a.Tipo,
                                                                                       Serie = a.Serie
                                                                                   } into grupo
                                                                                   select new Entity.Contabilidad.Cattipocambio
                                                                                   {
                                                                                       Fechatipocambio = grupo.Key.Fechatipocambio,
                                                                                       Importetipocambio = grupo.Max(x => x.Importetipocambio),
                                                                                       Serie = grupo.Key.Serie,
                                                                                       Tipo = grupo.Key.Tipo,
                                                                                       Usuario = grupo.Key.Usuario,
                                                                                       Estatus = 1,
                                                                                       Fecha = DateTime.Now

                                                                                   }).ToList();



                foreach (Entity.Contabilidad.Cattipocambio item in ListaTipoCambioDepurada)
                {
                    Entity.Contabilidad.Cattipocambio aux = MobileBO.ControlContabilidad.TraerCattipocambio(DateTime.Parse(item.Fechatipocambio));
                    if (aux != null)
                        item.UltimaAct = aux.UltimaAct;
                }
                MobileBO.ControlContabilidad.GuardarCattipocambioBM(ListaTipoCambioDepurada);
                return Entity.Response<List<Entity.Contabilidad.Cattipocambio>>.CrearResponse<List<Entity.Contabilidad.Cattipocambio>>(true, ListaTipoCambio);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Cattipocambio>>.CrearResponseVacio<List<Entity.Contabilidad.Cattipocambio>>(false, ex.Message);
            }
        }
    }
}