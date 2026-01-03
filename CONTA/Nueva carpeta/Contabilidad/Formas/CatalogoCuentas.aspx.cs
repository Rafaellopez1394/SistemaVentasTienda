using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
using System.Text;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CatalogoCuentas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByCode(string value, string mayor)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuenta = controlContabilidad.TraerCatCuentasPorCuenta(values.Codigo, (int.Parse(mayor) == 1 ? "1" : "2"), (int.Parse(mayor) == 1 ? "1" : "10"), values.ID);
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
        public static Entity.Response<List<Entity.HelpField.Values>> ConsultarCuentaPorCodigo(string cuentaContable,string empresaid)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                cuenta = controlContabilidad.TraerCatCuentasPorCuenta(cuentaContable, empresaid);
                if (cuenta != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Cuentaid, Codigo = cuenta.Afecta.ToString(), Descripcion = cuenta.Descripcion };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByPopUp(string value, string mayor)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatcuentas;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaCatcuentas = controlContabilidad.TraerCatCuentasPorDescripcion(values.Descripcion, (int.Parse(mayor) == 1 ? "1" : "2"), (int.Parse(mayor) == 1 ? "1" : "10"), values.ID);
                if (listaCatcuentas != null)
                {
                    foreach (Entity.Contabilidad.Catcuenta Catcuenta in listaCatcuentas)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = Catcuenta.Cuentaid, Codigo = Catcuenta.Cuenta, Descripcion = Catcuenta.Descripcion };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCtaSat_FindByCode(string value)
        {
            Entity.Contabilidad.Catalogocuentasat cuenta = new Entity.Contabilidad.Catalogocuentasat();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuenta = MobileBO.ControlContabilidad.TraerCatalogocuentasat(values.Codigo);
                if (cuenta != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Ctasat, Codigo = cuenta.Ctasat, Descripcion = cuenta.Descripcion };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCtaSat_FindByPopUp(string value)
        {
            List<Entity.Contabilidad.Catalogocuentasat> cuentas = new List<Entity.Contabilidad.Catalogocuentasat>();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuentas = MobileBO.ControlContabilidad.TraerCatalogocuentasatPorDescripcion(values.Descripcion);
                if (cuentas.Count > 0)
                {
                    foreach (Entity.Contabilidad.Catalogocuentasat cuenta in cuentas)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Ctasat, Codigo = cuenta.Ctasat, Descripcion = cuenta.Descripcion };
                        ListaElementos.Add(elemento);
                    }
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
        public static Entity.Response<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>> TraerDatosCuentas(string CuentaID)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas> ListaElementos;
            try
            {
                ListaElementos = new MobileBO.ControlContabilidad().TraerDatosCuentaContable(CuentaID);
                if (ListaElementos.ToList().Count > 0)
                {
                    return Entity.Response<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>.CrearResponse<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>(true, ListaElementos);
                }
                else
                {
                    return Entity.Response<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>.CrearResponseVacio<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>(false, "No se encontró resultado.");
                }

            }
            catch (Exception ex)
            {
                return Entity.Response<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>.CrearResponseVacio<System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> TraerFlujosPorCodigo(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            List<Entity.HelpField.Values> ListaElementos;
            Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                DataSet Flujos = controlContabilidad.TraerFlujos((values.Codigo == string.Empty || values.Codigo == null ? null : values.Codigo), (values.Descripcion == string.Empty || values.Descripcion == null ? null : values.Descripcion));
                foreach (DataRow row in Flujos.Tables[0].Rows)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = row["FlujoID"].ToString(), Codigo = row["Cod_Flujo"].ToString(), Descripcion = row["Descripcion"].ToString() };
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
        public static Entity.Response<object> GuardarCuentaContable(string cuentaContable, string cuentaMayor, int mayor)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();            
            try
            {
                Entity.Contabilidad.Catcuenta CuentaContable = new Entity.Contabilidad.Catcuenta();
                Entity.Contabilidad.Acvctam CuentaMayor = new Entity.Contabilidad.Acvctam();
                ListaDeEntidades<Entity.Contabilidad.Catcuenta> ListaCuentaContable = new ListaDeEntidades<Entity.Contabilidad.Catcuenta>();
                ListaDeEntidades<Entity.Contabilidad.Acvctam> ListaCuentaMayor = new ListaDeEntidades<Entity.Contabilidad.Acvctam>();
                CuentaContable = MobileBO.Utilerias.Deserializar<Entity.Contabilidad.Catcuenta>(cuentaContable);
                CuentaContable.Fecha = DateTime.Now;
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(CuentaContable.Empresaid);
                CuentaContable.CodEmpresa = empresa.Empresa.ToString();
                ListaCuentaContable.Add(CuentaContable);
                new MobileBO.ControlContabilidad().GuardarCatCuenta(ListaCuentaContable);
                if (mayor == 1)
                {
                    CuentaMayor = MobileBO.Utilerias.Deserializar<Entity.Contabilidad.Acvctam>(cuentaMayor);
                    CuentaMayor.Fecha = DateTime.Now;
                    CuentaMayor.CodEmpresa = empresa.Empresa.ToString();
                    ListaCuentaMayor.Add(CuentaMayor);
                    new MobileBO.ControlContabilidad().GuardarAcvctam(ListaCuentaMayor);
                }
                return Entity.Response<object>.CrearResponse<object>(true, new { CuentaID = ListaCuentaContable[0].Cuentaid, UltimaActCuenta = ListaCuentaContable[0].UltimaAct, AcvCtamID = (mayor == 1 ? ListaCuentaMayor[0].Acvctamid : ""), UltimaActCuentaMayor = (mayor == 1 ? ListaCuentaMayor[0].UltimaAct : 0) });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> GenerarXMLCatalogoCtas(string empresaid, string usuario, string fecha)
        {
            try
            {
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);                
                DataSet ds = new DataSet();
                ds = MobileBO.ControlContabilidad.TraerCatCuentasSat(empresa.Empresaid, DateTime.Parse(fecha));

                string mes = DateTime.Parse(fecha).Month.ToString().PadLeft(2, '0');
                string anio = DateTime.Parse(fecha).Year.ToString();

                string directorio = Environment.GetEnvironmentVariable("TEMP");
                string fileName = empresa.Rfc + anio + mes + "CT" + ".xml";
                string saveLocation = directorio + "\\" + fileName;

                if (System.IO.File.Exists(saveLocation))
                    System.IO.File.Delete(saveLocation);
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(saveLocation))
                {
                    file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

                    StringBuilder xml = new StringBuilder();
                    xml.Append("<catalogocuentas:Catalogo xmlns:catalogocuentas=\"http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas\" ");
                    xml.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas/CatalogoCuentas_1_3.xsd\" ");
                    xml.AppendFormat("Version=\"1.3\" RFC=\"{0}\" Mes=\"{1}\" Anio=\"{2}\" ", empresa.Rfc, mes, anio);
                    //xml.Append("xmlns=\"www.sat.gob.mx/esquemas/ContabilidadE/1_1/CatalogoCuentas\">");
                    xml.Append(">");
                    file.WriteLine(xml.ToString());
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["Nivel"].ToString() == "1")
                        {
                            file.WriteLine("<catalogocuentas:Ctas CodAgrup=\"" + row["CTASAT"].ToString() + "\" NumCta=\"" + row["Cuenta"].ToString() + "\" Desc=\"" + row["Descripcion"].ToString().Replace("&","") + "\" Nivel=\"" + row["Nivel"].ToString() + "\" Natur=\"" + row["Nat_Cta"].ToString() + "\"/>");
                        }
                        else
                        {
                            file.WriteLine("<catalogocuentas:Ctas CodAgrup=\"" + row["CTASAT"].ToString() + "\" NumCta=\"" + row["Cuenta"].ToString() + "\" Desc=\"" + row["Descripcion"].ToString().Replace("&", "") + "\" SubCtaDe=\"" + row["SubCtaDe"].ToString() + "\" Nivel=\"" + row["Nivel"].ToString() + "\" Natur=\"" + row["Nat_Cta"].ToString() + "\"/>");
                        }
                    }
                    file.WriteLine("</catalogocuentas:Catalogo>");
                }
                return Entity.Response<object>.CrearResponse<object>(true, new { FileName = fileName, Ruta = saveLocation });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }


        }

    }

    public class ControladorReporteCatalogoCuentas : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE  
            int Ingles = int.Parse(parametros.Get("Ingles"));
            string empresaid = parametros.Get("empresaid");
            string codigoEmpresa;
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            codigoEmpresa = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            DataSet ds = new DataSet();
            ds = MobileBO.ControlContabilidad.spcgenerainformedetallecuentas(codigoEmpresa);
            ds.Tables[1].Rows[0]["Ingles"] = Ingles;
            try
            {
                base.NombreReporte = "ReporteCatalogoCuentas";
                base.FormatoReporte = 4;
                base.ReporteContabilidad = (int)Entity.ReporteContabilidadFromServer.CatalogoCuentas;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }
}