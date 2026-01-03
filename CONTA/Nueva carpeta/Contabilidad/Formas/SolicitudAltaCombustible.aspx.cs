using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class SolicitudAltaCombustible : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaVendedores_FindByCode(string value)
        {
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                Entity.Analisis.Catvendedor _vendedor = MobileBO.ControlAnalisis.TraerCatvendedoresPorCodigo(int.Parse(values.Codigo));

                if (_vendedor != null)
                {
                    string _nombreVendedor = _vendedor.Nombre + " " + _vendedor.Apellidopaterno + " " + _vendedor.Apellidomaterno;
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = _vendedor.Vendedorid, Codigo = _vendedor.Codigovendedor.ToString(), Descripcion = _nombreVendedor };
                    ListaElementos.Add(elemento);
                }
                else
                {
                    return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, "No se encontró resultado.");
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaVendedores_FindByPopUp(string value)
        {
            
            Entity.ListaDeEntidades<Entity.Analisis.Catvendedor> listaVendedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaVendedores = MobileBO.ControlAnalisis.TraerCatvendedores();
                if (listaVendedores != null)
                {
                    foreach (Entity.Analisis.Catvendedor _vendedor in listaVendedores)
                    {
                        string _nombreVendedor = _vendedor.Nombre + " " + _vendedor.Apellidopaterno + " " + _vendedor.Apellidomaterno;
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = _vendedor.Vendedorid, Codigo = _vendedor.Codigovendedor.ToString(), Descripcion = _nombreVendedor };                        
                        ListaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Analisis.Catvendedor> TraerVendedor(string vendedorid)
        {
            try
            {
                Entity.Analisis.Catvendedor _vendedor = MobileBO.ControlAnalisis.TraerCatvendedores(vendedorid);
                if (_vendedor != null)
                {
                    return Entity.Response<Entity.Analisis.Catvendedor>.CrearResponse<Entity.Analisis.Catvendedor>(true, _vendedor);
                }
                else
                {
                    return Entity.Response<Entity.Analisis.Catvendedor>.CrearResponseVacio<Entity.Analisis.Catvendedor>(false, "No se encontró el vendedor");
                }
            }
            catch(Exception ex)
            {
                return Entity.Response<Entity.Analisis.Catvendedor>.CrearResponseVacio<Entity.Analisis.Catvendedor>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.CRMWeb.CrmSolicitudaltacombustible>> TraerSolicitudes(string vendedorid, int? estatus)
        {
            //List<object> _solicitudes = new List<object>();

            try
            {
                if (vendedorid.Trim() == "")
                {
                    vendedorid = null;
                }
                if(estatus == -1)
                {
                    estatus = null;
                }
                List<Entity.CRMWeb.CrmSolicitudaltacombustible> _solicitudes = new List<Entity.CRMWeb.CrmSolicitudaltacombustible>();
                _solicitudes = MobileBO.ControlCRMWeb.TraerCrmSolicitudaltacombustible(null, vendedorid, null, null, estatus);
                foreach(Entity.CRMWeb.CrmSolicitudaltacombustible _solicitud in _solicitudes)
                {
                    _solicitud.Fechasolicitudstring = _solicitud.Fechasolicitud != null ? string.Format("{0:yyyy/MM/dd hh:mm}", _solicitud.Fechasolicitud) : "";
                    _solicitud.Fechaaceptadostring = _solicitud.Fechaaceptado != null && _solicitud.Fechaaceptado != DateTime.MinValue ? string.Format("{0:yyyy/MM/dd hh:mm}", _solicitud.Fechaaceptado) : "";
                    _solicitud.Fecharechazadostring = _solicitud.Fecharechazado != null && _solicitud.Fecharechazado != DateTime.MinValue ? string.Format("{0:yyyy/MM/dd hh:mm}", _solicitud.Fecharechazado) : "";
                    

                }
                return Entity.Response<List<Entity.CRMWeb.CrmSolicitudaltacombustible>>.CrearResponse(true, _solicitudes);
            }
            catch(Exception ex)
            {
                return Entity.Response<List<Entity.CRMWeb.CrmSolicitudaltacombustible>>.CrearResponseVacio<List<Entity.CRMWeb.CrmSolicitudaltacombustible>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> AceptarSolicitud(int solicitudid,  string usuario)
        {
            try
            {
                Entity.CRMWeb.CrmSolicitudaltacombustible _solicitud = MobileBO.ControlCRMWeb.TraerCrmSolicitudaltacombustible(solicitudid);
                if (_solicitud != null)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        _solicitud.Fechaaceptado = DateTime.Now;
                        _solicitud.Usuarioacepta = usuario;
                        _solicitud.Fecha = DateTime.Now;
                        _solicitud.Usuario = usuario;
                        _solicitud.Estatussolicitudid = 1;
                        MobileBO.ControlCRMWeb.GuardarCrmSolicitudaltacombustible(new List<Entity.CRMWeb.CrmSolicitudaltacombustible>() { _solicitud });

                        //Insertamos en el catálogo de vendedores autorizados para combustible
                        Entity.CRMWeb.CrmCatvendedorescombustible _catvend = new Entity.CRMWeb.CrmCatvendedorescombustible();
                        _catvend.Altaporsolicitud = 1;
                        _catvend.Estatus = 1;
                        _catvend.Fecha = DateTime.Now;
                        _catvend.Fechaalta = DateTime.Now;
                        _catvend.Fechabaja = null;
                        _catvend.Ubicacionlatitud = _solicitud.Latitud;
                        _catvend.Ubicacionlongitud = _solicitud.Longitud;
                        _catvend.Usuario = usuario;
                        _catvend.Usuarioalta = usuario;
                        _catvend.Vendedorid = _solicitud.Vendedorid;
                        MobileBO.ControlCRMWeb.GuardarCrmCatvendedorescombustible(new List<Entity.CRMWeb.CrmCatvendedorescombustible>() { _catvend });

                        ts.Complete();
                    }

                    Entity.Analisis.Catvendedor _vendedor = MobileBO.ControlAnalisis.TraerCatvendedores(_solicitud.Vendedorid);
                    Entity.Configuracion.Catusuario _usuario = MobileBO.ControlConfiguracion.TraerCatusuarios(null, usuario);

                    MobileBO.Utilities.EnviarMail mail = new MobileBO.Utilities.EnviarMail();
                    mail.ConfigurarMail("notificaciones@balor.com.mx", "notificaciones@balor.com.mx", "Je3scPKgKRkj7bu", 587, "gator3271.hostgator.com");//notificaciones@balor.mx Balor01**
                    string _asuntoCorreo = "Solicitud de alta para apoyo en combustible aceptada";
                    string _mensajeCorreo = "Su solicitud ha sido acepatada por : <b>" + _usuario.Nombre + "</b>";
                    mail.EnviarEMail(_vendedor.Correo, new List<string>(), _asuntoCorreo, _mensajeCorreo, new List<string>());
                    
                    return Entity.Response<object>.CrearResponse<object>(true, "");
                }
                else
                {
                    return Entity.Response<object>.CrearResponse<object>(true, "No se encontró la solicitud especificada para autorizar");
                }
                
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> RechazarSolicitud(int solicitudid, string usuario)
        {
            try
            {
                Entity.CRMWeb.CrmSolicitudaltacombustible _solicitud = MobileBO.ControlCRMWeb.TraerCrmSolicitudaltacombustible(solicitudid);
                if (_solicitud != null)
                {
                    _solicitud.Fecharechazado = DateTime.Now;
                    _solicitud.Usuariorechaza = usuario;
                    _solicitud.Fecha = DateTime.Now;
                    _solicitud.Usuario = usuario;
                    _solicitud.Estatussolicitudid = 2;
                    MobileBO.ControlCRMWeb.GuardarCrmSolicitudaltacombustible(new List<Entity.CRMWeb.CrmSolicitudaltacombustible>() { _solicitud });

                    Entity.Analisis.Catvendedor _vendedor = MobileBO.ControlAnalisis.TraerCatvendedores(_solicitud.Vendedorid);
                    Entity.Configuracion.Catusuario _usuario = MobileBO.ControlConfiguracion.TraerCatusuarios(null, usuario);

                    MobileBO.Utilities.EnviarMail mail = new MobileBO.Utilities.EnviarMail();
                    mail.ConfigurarMail("notificaciones@balor.com.mx", "notificaciones@balor.com.mx", "Je3scPKgKRkj7bu", 587, "gator3271.hostgator.com");//notificaciones@balor.mx Balor01**
                    string _asuntoCorreo = "Solicitud de alta para apoyo en combustible rechazada";
                    string _mensajeCorreo = "Su solicitud ha sido rechazada por : <b>" + _usuario.Nombre + "</b>";
                    mail.EnviarEMail(_vendedor.Correo, new List<string>(), _asuntoCorreo, _mensajeCorreo, new List<string>());

                    return Entity.Response<object>.CrearResponse<object>(true, "");
                }
                else
                {
                    return Entity.Response<object>.CrearResponse<object>(true, "No se encontró la solicitud especificada para rechazar");
                }

            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

    }
  
}