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
    public partial class Polizas : System.Web.UI.Page
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
            try
            {
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();// controlConfiguracion.TraerCatempresas();
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
        
        [WebMethod]
        public static Entity.Response<string> Consultar(string ejercicio,string Empresaid)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            try
            {

                // Ejecutar proceso
                //controlContabilidad.ConsultarPolizas(ejercicio);
                //return Entity.Response<string>.CrearResponseVacio<string>(false, "No se encontró resultado.");
                bool proceso = controlContabilidad.ProcesarPolizasPendientes(Convert.ToInt32(ejercicio), Empresaid);
                if (proceso)
                {
                    return Entity.Response<string>.CrearResponse<string>(true, "Terminado correctamente");
                }
                else
                {
                    return Entity.Response<string>.CrearResponse<string>(false, "No se pudo consultar");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<string>.CrearResponseVacio<string>(false, ex.Message);
            }
        }
        [WebMethod]
        public static Entity.Response<string> ConsultarContabilidades(string Empresaid)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            
            try
            {
                DataSet ds = controlContabilidad.TraerTipoContabilidad(Empresaid);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Convertir la primera tabla del DataSet a JSON
                    string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);

                    return Entity.Response<string>.CrearResponse<string>(true, jsonResult);
                }
                else
                {
                    return Entity.Response<string>.CrearResponse<string>(false, "No se encontraron datos.");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<string>.CrearResponseVacio<string>(false, ex.Message);
            }
        }
    }
}