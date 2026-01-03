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
    public partial class CatalogoClientesIntercompañia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catclientesfilial> GuardarCliente(string value)
        {
            Entity.Contabilidad.Catclientesfilial ClienteFilial = MobileBO.Utilerias.Deserializar<Entity.Contabilidad.Catclientesfilial>(value);
            ClienteFilial.Fecha = DateTime.Now;

            if (ClienteFilial.UltimaAct == 0) {
                ClienteFilial.Fechaalta = DateTime.Today;                
                ClienteFilial.Estatus = 1;
            }
            try
            {
                Entity.Analisis.Catcliente CatCliente = MobileBO.ControlAnalisis.TraerCatclientes((ClienteFilial.Clienteid == string.Empty || ClienteFilial.Clienteid == null ? Guid.Empty.ToString() : ClienteFilial.Clienteid), null, null);
                if (CatCliente == null)
                {
                    CatCliente = new Entity.Analisis.Catcliente();
                    CatCliente.Clienteid = Guid.Empty.ToString();
                    CatCliente.Empresaid = ClienteFilial.Empresaid;
                    CatCliente.Codigo = 0;
                    CatCliente.Nombre = "";
                    CatCliente.Apellidopaterno = "";
                    CatCliente.Apellidomaterno = "";
                    CatCliente.Nombrecompleto = ClienteFilial.Nombre;
                    CatCliente.Razonsocial = ClienteFilial.Nombre;
                    CatCliente.Tipopersona = 2;
                    CatCliente.Tipocliente = 9;
                    CatCliente.Genero = "";
                    CatCliente.Rfc = ClienteFilial.Rfc;
                    CatCliente.Curp = "";
                    CatCliente.Fiel = "";
                    CatCliente.Correo = "";
                    CatCliente.Telefono = "";
                    CatCliente.Regimensimplificado = false;
                    CatCliente.Nacionalidad = "MEXICANA";
                    CatCliente.PaisidNacimiento = "D693195E-0C3C-490F-A664-CBA97D790F42";
                    CatCliente.Clasificacionid = "DC0B6896-02E0-4AE7-8F58-ACDF3989194C";
                    CatCliente.Clasificacioncartera = 1;
                    CatCliente.Mescleanup = 0;
                    CatCliente.Fechaalta = DateTime.Today.ToShortDateString();
                    CatCliente.Fecha = DateTime.Now;
                    CatCliente.Estatus = 1;
                    CatCliente.Usuario = ClienteFilial.Usuario;
                    CatCliente.ExclusionContratoAnual = false;
                    CatCliente.Filial = true;
                }
                else {
                    CatCliente.Nombrecompleto = ClienteFilial.Nombre;
                    CatCliente.Razonsocial = ClienteFilial.Nombre;
                    CatCliente.Rfc = ClienteFilial.Rfc;
                    CatCliente.Usuario = ClienteFilial.Usuario;
                    CatCliente.Fecha = DateTime.Now;
                }
                //Guardamos la entidad del cliente en un scope de transacciones por si algo sale mal no se genere la contabilidad
                using (TransactionScope scope = new TransactionScope())
                {
                    MobileBO.ControlAnalisis.GuardarCatcliente(new List<Entity.Analisis.Catcliente>() { CatCliente });
                    ClienteFilial.Clienteid = CatCliente.Clienteid;
                    ClienteFilial.Codigo = CatCliente.Codigo;

                    //Si es un cliente nuevo se generan las cuentas Cuentas Contables
                    if (ClienteFilial.UltimaAct == 0) {
                        Entity.Configuracion.Catempresa empresa = new Entity.Configuracion.Catempresa();

                        empresa = MobileBO.ControlConfiguracion.TraerCatempresas((int)Entity.Empresas.Factur);
                        MobileBO.ControlAnalisis.GenerarCuentasContables(CatCliente, empresa);

                        empresa = MobileBO.ControlConfiguracion.TraerCatempresas((int)Entity.Empresas.Balor);
                        MobileBO.ControlAnalisis.GenerarCuentasContables(CatCliente, empresa);
                    }
                    //Guardamos el cliente Filial
                    MobileBO.ControlContabilidad.GuardarCatclientesfilial(new List<Entity.Contabilidad.Catclientesfilial>() { ClienteFilial });

                    scope.Complete();

                }

                return Entity.Response<Entity.Contabilidad.Catclientesfilial>.CrearResponse<Entity.Contabilidad.Catclientesfilial>(true, ClienteFilial);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catclientesfilial>.CrearResponseVacio<Entity.Contabilidad.Catclientesfilial>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catclientesfilial> TraerCliente(string clienteid)
        {
            Entity.Contabilidad.Catclientesfilial cliente = null;
            try
            {
                cliente = MobileBO.ControlContabilidad.TraerCatclientesfilial(clienteid, null, null);
                if (cliente == null)
                    return Entity.Response<Entity.Contabilidad.Catclientesfilial>.CrearResponseVacio<Entity.Contabilidad.Catclientesfilial>(false, "No se encontró resultado.");
                return Entity.Response<Entity.Contabilidad.Catclientesfilial>.CrearResponse<Entity.Contabilidad.Catclientesfilial>(true, cliente);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catclientesfilial>.CrearResponseVacio<Entity.Contabilidad.Catclientesfilial>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> ayudaCliente_FindByCode(string value)
        {
            Entity.Contabilidad.Catclientesfilial cliente;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cliente = MobileBO.ControlContabilidad.TraerCatclientesfilial(null, values.ID, int.Parse(values.Codigo));
                if (cliente != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cliente.Clienteid, Codigo = cliente.Codigo.ToString(), Descripcion = cliente.Nombre };
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
        public static Entity.Response<List<Entity.HelpField.Values>> ayudaCliente_FindByPopUp(string value)
        {
            List<Entity.Contabilidad.Catclientesfilial> listaClientes;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaClientes = MobileBO.ControlContabilidad.TraerCatclientesfilialPorNombre(values.Descripcion, values.ID);
                foreach (Entity.Contabilidad.Catclientesfilial cliente in listaClientes)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cliente.Clienteid, Codigo = cliente.Codigo.ToString(), Descripcion = cliente.Nombre };
                    ListaElementos.Add(elemento);
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerPaises()
        {
            Entity.ListaDeEntidades<Entity.Configuracion.Catpais> paises;

            List<object> listaElementos = new List<object>();
            try
            {
                paises = MobileBO.ControlConfiguracion.TraerCatpaises();
                if (paises != null)
                {
                    foreach (Entity.Configuracion.Catpais pais in paises)
                    {
                        object elemento = new { id = pais.Paisid, nombre = pais.Pais };
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
        public static Entity.Response<List<object>> TraerEstadosPorPais(string pais)
        {
            List<object> listaElementos = new List<object>();
            try
            {
                if (pais != string.Empty)
                {
                    Entity.ListaDeEntidades<Entity.Configuracion.Catestado> estados;
                    estados = MobileBO.ControlConfiguracion.TraerCatestadosPorPais(pais);
                    if (estados != null)
                    {
                        foreach (Entity.Configuracion.Catestado estado in estados)
                        {
                            object elemento = new { id = estado.Estadoid, nombre = estado.Estado };
                            listaElementos.Add(elemento);
                        }
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
        public static Entity.Response<List<object>> TraerMunicipiosPorEstado(string estado)
        {
            List<object> listaElementos = new List<object>();
            try
            {
                if (estado != string.Empty)
                {
                    Entity.ListaDeEntidades<Entity.Configuracion.Catmunicipio> municipios;
                    municipios = MobileBO.ControlConfiguracion.TraerCatmunicipiosPorEstado(estado);
                    if (municipios != null)
                    {
                        foreach (Entity.Configuracion.Catmunicipio municipio in municipios)
                        {
                            object elemento = new { id = municipio.Municipioid, nombre = municipio.Municipio };
                            listaElementos.Add(elemento);
                        }
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
        public static Entity.Response<List<object>> TraerCiudadesPorMunicipio(string municipio)
        {
            List<object> listaElementos = new List<object>();
            try
            {
                if (municipio != string.Empty)
                {
                    Entity.ListaDeEntidades<Entity.Configuracion.Catciudad> ciudades;
                    ciudades = MobileBO.ControlConfiguracion.TraerCatciudadesPorMunicipio(municipio);
                    if (ciudades != null)
                    {
                        foreach (Entity.Configuracion.Catciudad ciudad in ciudades)
                        {
                            object elemento = new { id = ciudad.Ciudadid, nombre = ciudad.Ciudad };
                            listaElementos.Add(elemento);
                        }
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
        public static Entity.Response<List<object>> TraerColoniasPorCiudad(string ciudad)
        {
            List<object> listaElementos = new List<object>();
            try
            {
                if (ciudad != string.Empty)
                {
                    Entity.ListaDeEntidades<Entity.Configuracion.Catcolonia> colonias;
                    colonias = MobileBO.ControlConfiguracion.TraerCatcoloniasPorCiudad(ciudad);
                    if (colonias != null)
                    {
                        foreach (Entity.Configuracion.Catcolonia colonia in colonias)
                        {
                            object elemento = new { id = colonia.Coloniaid, nombre = colonia.Colonia };
                            listaElementos.Add(elemento);
                        }
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
        public static Entity.Response<List<object>> consultacp(string cp)
        {
            List<object> listaElementos = new List<object>();
            try
            {
                if (cp != string.Empty)
                {                    
                    listaElementos = MobileBO.ControlConfiguracion.ConsultarCP(cp);                    
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }
    }
}