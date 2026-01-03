using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class PresupuestoContable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.CatProyeccion>> GuardarPresupuestos(string value)
        {
            try
            {
                List<Entity.Contabilidad.CatProyeccion> ListaSave = MobileBO.Utilerias.Deserializar<List<Entity.Contabilidad.CatProyeccion>>(value);
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(ListaSave[0].Empresaid);
                Entity.Contabilidad.Catcuenta Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(ListaSave[0].Cuenta, empresa.Empresaid);

                ListaSave[0].CodEmpresa = empresa.Empresa;
                ListaSave[0].Fecha = DateTime.Now;
                ListaSave[0].Nivel = Cuenta.Nivel;

                //Guardamos el nivel actual
                MobileBO.ControlContabilidad.GuardarCatProyeccion(ListaSave);

                //Obtener los sub-niveles y acumularnos en la proyeccion de presupuestos
                List<Entity.Contabilidad.CatProyeccion> ListaSubniveles = new List<Entity.Contabilidad.CatProyeccion>();
                for (int i = ListaSave[0].Nivel - 1; i > 0; i--)
                {
                    Entity.Contabilidad.Catcuenta CuentaPadre = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(ListaSave[0].Cuenta.Substring(0, 4 * i) + ("000000000000000000000000").Substring(4 * i, 24 - 4 * i), empresa.Empresaid);
                    Entity.Contabilidad.CatProyeccion proyeccion = MobileBO.ControlContabilidad.TraerCatProyeccionPorCuenta(empresa.Empresa, ListaSave[0].Ejercicio, CuentaPadre.Cuenta);

                    if (proyeccion == null) {
                        proyeccion = new Entity.Contabilidad.CatProyeccion();
                        proyeccion.Proyeccionid = Guid.Empty.ToString();
                        proyeccion.CodEmpresa = empresa.Empresa;
                        proyeccion.Ejercicio = ListaSave[0].Ejercicio;
                        proyeccion.Cuenta = CuentaPadre.Cuenta;
                        proyeccion.Nivel = CuentaPadre.Nivel;
                        proyeccion.Capturado = true;
                    }

                    proyeccion.Estatus = 1;
                    proyeccion.Usuario = ListaSave[0].Usuario;
                    proyeccion.Fecha = DateTime.Now;

                    proyeccion.Presupuesto1 = 0m;
                    proyeccion.Presupuesto2 = 0m;
                    proyeccion.Presupuesto3 = 0m;
                    proyeccion.Presupuesto4 = 0m;
                    proyeccion.Presupuesto5 = 0m;
                    proyeccion.Presupuesto6 = 0m;
                    proyeccion.Presupuesto7 = 0m;
                    proyeccion.Presupuesto8 = 0m;
                    proyeccion.Presupuesto9 = 0m;
                    proyeccion.Presupuesto10 = 0m;
                    proyeccion.Presupuesto11 = 0m;
                    proyeccion.Presupuesto12 = 0m;


                    //Acumulamos todo el catalogo de cuentas
                    List<Entity.Contabilidad.CatProyeccion> ListaProyecciones = MobileBO.ControlContabilidad.Catproyeccion_Select_Acumulado(empresa.Empresa, ListaSave[0].Ejercicio, ListaSave[0].Cuenta.Substring(0, 4 * i), i + 1);
                    foreach (Entity.Contabilidad.CatProyeccion item in ListaProyecciones)
                    {
                        proyeccion.Presupuesto1 += item.Presupuesto1;
                        proyeccion.Presupuesto2 += item.Presupuesto2;
                        proyeccion.Presupuesto3 += item.Presupuesto3;
                        proyeccion.Presupuesto4 += item.Presupuesto4;
                        proyeccion.Presupuesto5 += item.Presupuesto5;
                        proyeccion.Presupuesto6 += item.Presupuesto6;
                        proyeccion.Presupuesto7 += item.Presupuesto7;
                        proyeccion.Presupuesto8 += item.Presupuesto8;
                        proyeccion.Presupuesto9 += item.Presupuesto9;
                        proyeccion.Presupuesto10 += item.Presupuesto10;
                        proyeccion.Presupuesto11 += item.Presupuesto11;
                        proyeccion.Presupuesto12 += item.Presupuesto12;
                    }
                    MobileBO.ControlContabilidad.GuardarCatProyeccion(new List<Entity.Contabilidad.CatProyeccion>() { proyeccion });
                }
                return Entity.Response<List<Entity.Contabilidad.CatProyeccion>>.CrearResponse<List<Entity.Contabilidad.CatProyeccion>>(true, ListaSave);
                
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.CatProyeccion>>.CrearResponseVacio<List<Entity.Contabilidad.CatProyeccion>>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<List<object>> TraerEmpresas()
        {
            //MobileBO.ControlConfiguracion controlConfiguracion = new MobileBO.ControlConfiguracion();
            Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;

            List<object> listaElementos = new List<object>();
            try
            {                
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();
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
        public static Entity.Response<List<Entity.Contabilidad.CatProyeccion>> TraerProyeccionContable(string empresaid, int ejercicio, string cuenta)
        {
            try
            {
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                Entity.Contabilidad.CatProyeccion proyeccion = MobileBO.ControlContabilidad.TraerCatProyeccionPorCuenta(empresa.Empresa, ejercicio, cuenta);
                if (proyeccion == null)
                    proyeccion = new Entity.Contabilidad.CatProyeccion();
                List<Entity.Contabilidad.CatProyeccion> proyecciones = new List<Entity.Contabilidad.CatProyeccion>();
                proyecciones.Add(proyeccion);

                proyeccion.Observacion1 = proyeccion.Observacion1 != null ? proyeccion.Observacion1 : "";
                proyeccion.Observacion2 = proyeccion.Observacion2 != null ? proyeccion.Observacion2 : "";
                proyeccion.Observacion3 = proyeccion.Observacion3 != null ? proyeccion.Observacion3 : "";
                proyeccion.Observacion4 = proyeccion.Observacion4 != null ? proyeccion.Observacion4 : "";
                proyeccion.Observacion5 = proyeccion.Observacion5 != null ? proyeccion.Observacion5 : "";
                proyeccion.Observacion6 = proyeccion.Observacion6 != null ? proyeccion.Observacion6 : "";
                proyeccion.Observacion7 = proyeccion.Observacion7 != null ? proyeccion.Observacion7 : "";
                proyeccion.Observacion8 = proyeccion.Observacion8 != null ? proyeccion.Observacion8 : "";
                proyeccion.Observacion9 = proyeccion.Observacion9 != null ? proyeccion.Observacion9 : "";
                proyeccion.Observacion10 = proyeccion.Observacion10 != null ? proyeccion.Observacion10 : "";
                proyeccion.Observacion11 = proyeccion.Observacion11 != null ? proyeccion.Observacion11 : "";
                proyeccion.Observacion12 = proyeccion.Observacion12 != null ? proyeccion.Observacion12 : "";



                return Entity.Response<List<Entity.Contabilidad.CatProyeccion>>.CrearResponse<List<Entity.Contabilidad.CatProyeccion>>(true, proyecciones);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.CatProyeccion>>.CrearResponseVacio<List<Entity.Contabilidad.CatProyeccion>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<object> TraerCuentaAfecta(string empresaid, string cuenta)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerCuentaAfecta(empresaid, cuenta);
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Afecta = ds.Tables[0].Rows[0]["Afecta"] });
                //return Entity.Response<object>.CrearResponse<object>(true, new { Afecta = ds.Tables[0].Rows[0]["Afecta"].ToString()});
                else
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "No se encontraron cuentas contables para esta empresa");
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByCode(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();

            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuenta = controlContabilidad.TraerCatCuentasPorCuentaAfectable(values.Codigo, values.ID);

                if (cuenta != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Cuentaid, Codigo = cuenta.Cuenta, Descripcion = cuenta.Descripcion };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByPopUp(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaCatCuentas = controlContabilidad.TraerCatCuentasPorDescripcionAfectable(values.Descripcion, values.ID);
                if (listaCatCuentas != null)
                {
                    foreach (Entity.Contabilidad.Catcuenta catCuenta in listaCatCuentas)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = catCuenta.Cuentaid, Codigo = catCuenta.Cuenta, Descripcion = catCuenta.Descripcion };
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

    }

    public class ControladorReportePresupuestoContable : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            int anio = int.Parse(parametros.Get("anio"));
            string empresaid = parametros.Get("empresaid");
            int Formato = (int.Parse(parametros.Get("Formato")) == 0 ? 3 : int.Parse(parametros.Get("Formato")));
            int operativo = Convert.ToInt32(parametros.Get("formatoespecial"));
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
            if (empresa.Tables[0].Rows.Count == 0)
            {
                DataRow row = empresa.Tables[0].NewRow();
                row["Empresa"] = 0;
                row["Descripcion"] = "CONSOLIDADO";
                row["Rfc"] = "";
                row["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                empresa.Tables[0].Rows.Add(row);
            }
            DataSet DatosReporte = MobileBO.ControlContabilidad.TraerPresupuestoContable((empresaid == null || empresaid == string.Empty ? null : empresaid), anio, operativo);

            DataSet ds = new DataSet();
            ds.Tables.Add(empresa.Tables[0].Copy());

            DataTable tblDatos = DatosReporte.Tables[0].Copy();
            tblDatos.TableName = "DatosReporte";
            ds.Tables.Add(tblDatos);
            try
            {
                base.NombreReporte = (Formato == 3 ? "ReportePresupuestoContable" : "ReportePresupuestoContableExcel");
                base.FormatoReporte = Formato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReportePresupuestoContable.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}