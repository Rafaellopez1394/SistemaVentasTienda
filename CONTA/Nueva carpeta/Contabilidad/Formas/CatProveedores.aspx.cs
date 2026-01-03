using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CatProveedores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Ayudas
        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaProveedores_FindByCode(string value)
        {
            Entity.Contabilidad.Catproveedor proveedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                proveedores = MobileBO.ControlContabilidad.TraerCatproveedoresPorCodigo(int.Parse(values.Codigo), values.ID);
                if (proveedores != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = proveedores.Proveedorid, Codigo = proveedores.Codigo.ToString(), Descripcion = proveedores.Nombre };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaProveedores_FindByPopUp(string value)
        {
            System.Collections.Generic.List<Entity.Contabilidad.Catproveedor> listaProveedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaProveedores = MobileBO.ControlContabilidad.TraerCatproveedoresPorNombre(values.Descripcion, values.ID);
                if (listaProveedores != null)
                {
                    foreach (Entity.Contabilidad.Catproveedor proveedor in listaProveedores)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = proveedor.Proveedorid, Codigo = proveedor.Codigo.ToString(), Descripcion = proveedor.Nombre };
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
        #endregion

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catproveedor> TraerProveedor(string Proveedorid)
        {
            try
            {
                Entity.Contabilidad.Catproveedor Proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(Proveedorid, null, null);
                if (Proveedor != null)
                {
                    return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponse<Entity.Contabilidad.Catproveedor>(true, Proveedor);
                }
                else
                {
                    return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, "No se encontró el prospecto seleccionado.");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catproveedor> TraerProveedorPorCodigo(int Codigo, string Empresaid)
        {
            try
            {
                Entity.Contabilidad.Catproveedor Proveedor = MobileBO.ControlContabilidad.TraerCatproveedoresPorCodigo(Codigo, Empresaid);
                if (Proveedor != null)
                {
                    return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponse<Entity.Contabilidad.Catproveedor>(true, Proveedor);
                }
                else
                {
                    return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, "No se encontró el prospecto seleccionado.");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catproveedor> TraerDatosProveedor(string RFC,string EmpresaID)
        {
            try
            {
                Entity.Contabilidad.Catproveedor Proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null,RFC, EmpresaID);
                if (Proveedor == null)
                {
                    Proveedor = new Entity.Contabilidad.Catproveedor();
                    Proveedor.Proveedorid = "00000000-0000-0000-0000-000000000000";
                    Proveedor.Codigo = MobileBO.ControlContabilidad.TraerSiguienteCodicoProveedor(EmpresaID);
                    Proveedor.UltimaAct = 0;
                    //Proveedor.Cuentacontable = "2400" + Proveedor.Codigo.ToString().PadLeft(4, '0') + "0000000000000000";
                }

                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponse<Entity.Contabilidad.Catproveedor>(true, Proveedor);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catcuenta> TraerDatosCuenta(string Cuentacontable, string EmpresaID)
        {
            try
            {
                MobileBO.ControlContabilidad BO = new MobileBO.ControlContabilidad();
                Entity.Contabilidad.Catcuenta Cuenta = BO.TraerCatCuentasPorCuenta(Cuentacontable, EmpresaID);

                return Entity.Response<Entity.Contabilidad.Catcuenta>.CrearResponse<Entity.Contabilidad.Catcuenta>(true, Cuenta);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catcuenta>.CrearResponseVacio<Entity.Contabilidad.Catcuenta>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catproveedor> TraerProveedorPorCuentaContable(string EmpresaID, string Cuentacontable)
        {
            try
            {
                Entity.Contabilidad.Catproveedor Proveedor = MobileBO.ControlContabilidad.TraerCatproveedoresPorCuentaContable (EmpresaID, Cuentacontable);
                if (Proveedor == null)
                {
                    Proveedor = new Entity.Contabilidad.Catproveedor();
                    Proveedor.Proveedorid = "00000000-0000-0000-0000-000000000000";
                    Proveedor.UltimaAct = 0;
                }
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponse<Entity.Contabilidad.Catproveedor>(true, Proveedor);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Catproveedor> GuardarProveedor(string value)
        {
            try
            {
                Entity.Contabilidad.Catproveedor entidad = MobileBO.Utilerias.Deserializar<Entity.Contabilidad.Catproveedor>(value);
                entidad.Nombre = new System.Globalization.CultureInfo("es-MX").TextInfo.ToTitleCase(entidad.Nombre.ToLower());
                entidad.Fecha = DateTime.Now;
                MobileBO.ControlContabilidad.GuardarCatproveedor(new List<Entity.Contabilidad.Catproveedor> { entidad });

                //MobileBO.ControlContabilidad BO = new MobileBO.ControlContabilidad();
                //Entity.Contabilidad.Catcuenta Cuenta = BO.TraerCatCuentasPorCuenta(entidad.Cuentacontable, entidad.Empresaid);
                //if (Cuenta == null)
                //{
                //    Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(entidad.Empresaid);

                //    Cuenta = new Entity.Contabilidad.Catcuenta();
                //    Cuenta.Empresaid = entidad.Empresaid;
                //    Cuenta.CodEmpresa = empresa.Empresa.ToString();
                //    Cuenta.Cuenta = entidad.Cuentacontable;
                //    Cuenta.Descripcion = entidad.Nombre;
                //    Cuenta.Descripcioningles = entidad.Nombre;
                //    Cuenta.Nivel = 2;
                //    Cuenta.Afecta = true;
                //    Cuenta.Sistema = false;
                //    Cuenta.Ietu = false;
                //    Cuenta.Isr = false;
                //    Cuenta.Saldo = 0.0m;
                //    Cuenta.FlujoCar = "999";
                //    Cuenta.FlujoAbo = "499";
                //    Cuenta.Estatus = 1;
                //    Cuenta.Fecha = DateTime.Now;
                //    Cuenta.UltimaAct = 0;
                //    Cuenta.CtaSat = "";
                //}
                //else
                //{
                //    Cuenta.Descripcion = entidad.Nombre;
                //    Cuenta.Fecha = DateTime.Now;
                //}

                //BO.GuardarCatCuenta(new Entity.ListaDeEntidades<Entity.Contabilidad.Catcuenta> { Cuenta });

                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponse<Entity.Contabilidad.Catproveedor>(true, entidad);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Catproveedor>.CrearResponseVacio<Entity.Contabilidad.Catproveedor>(false, ex.Message);
            }
        }


    }
}