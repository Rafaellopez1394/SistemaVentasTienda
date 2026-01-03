using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Homex.Core.Utilities;
using System.Globalization;
using System.Text;
using iTextSharp.text;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteRelacionSaldos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Esquema"] = "";
        }

        [WebMethod]
        public static Entity.Response<object> TraerUltimaCuentaContable(string empresaid)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerUltimaCuentaContable(empresaid, "");
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Cuentaid = ds.Tables[0].Rows[0]["CuentaID"].ToString(), Cuenta = ds.Tables[0].Rows[0]["Cuenta"].ToString(), Descripcion = ds.Tables[0].Rows[0]["Descripcion"].ToString() });
                else
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "No se encontraron cuentas contables para esta empresa");
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> TraerPrimeraCuentaContable(string empresaid)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerPrimeraCuentaContable(empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Cuentaid = ds.Tables[0].Rows[0]["CuentaID"].ToString(), Cuenta = ds.Tables[0].Rows[0]["Cuenta"].ToString(), Descripcion = ds.Tables[0].Rows[0]["Descripcion"].ToString() });
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
                cuenta = controlContabilidad.TraerCatCuentasPorCuenta(values.Codigo, values.ID);

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
                listaCatCuentas = controlContabilidad.TraerCatCuentasPorDescripcion(values.Descripcion, values.ID);
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


        [WebMethod (EnableSession = true ) ]
        public static Entity.Response<object> EnviarSAT(string EmpresaID, string FechaInicial, string FechaFinal, string CuentaInicial, string CuentaFinal, string Nivel, string Ingles, string TipoEnvio,bool ExcluirDemandados)
        {
            try
            {
                bool SoloFiscales = MobileBO.ControlContabilidad.ValidaCuentasFiscales(EmpresaID, Convert.ToDateTime(FechaInicial).Year);

                if (SoloFiscales == true)
                {
                    DataSet Cuentas = MobileBO.ControlContabilidad.TraerCatCuentasPorEjercicio(EmpresaID, Convert.ToDateTime(FechaInicial).Year);

                    List<Entity.ModeloDatosCuentasSAT> lstdatosSinCTA = new List<Entity.ModeloDatosCuentasSAT>();

                    foreach (DataRow row in Cuentas.Tables[0].Rows)
                    {
                        Entity.ModeloDatosCuentasSAT datosSinCTA = new Entity.ModeloDatosCuentasSAT();

                        if (row["CTASAT"].ToString() == "")
                        {
                            datosSinCTA.Cuenta = row["Cuenta"].ToString();
                            datosSinCTA.CuentaID = row["CuentaID"].ToString();
                            datosSinCTA.Descripcion = row["Descripcion"].ToString();
                            datosSinCTA.Nivel = Convert.ToInt32(row["Nivel"]);
                            datosSinCTA.CTASAT = row["CTASAT"].ToString();
                            lstdatosSinCTA.Add(datosSinCTA);
                        }
                    }

                    if (lstdatosSinCTA.Count == 0)
                    {
                        lstdatosSinCTA.Clear();

                        Dictionary<string, string> ctarev = new Dictionary<string, string>();

                        foreach (DataRow row in Cuentas.Tables[0].Rows)
                        {
                            int Nvl = Convert.ToInt32(row["Nivel"]);

                            while (Nvl > 0)
                            {
                                string ctaSAT = MobileBO.ControlContabilidad.ValidaCtaSATPorEmpresaCuentaNivel(EmpresaID, row["Cuenta"].ToString().Substring(0, 4 * Nvl).PadRight(24, '0'), Nvl);
                                if (ctaSAT == "")
                                {
                                    if (!ctarev.ContainsKey(row["Cuenta"].ToString().Substring(0, 4 * Nvl).PadRight(24, '0')))
                                    {
                                        Entity.ModeloDatosCuentasSAT datosSinCTA = new Entity.ModeloDatosCuentasSAT();
                                        datosSinCTA.Cuenta = row["Cuenta"].ToString().Substring(0, 4 * Nvl).PadRight(24, '0');
                                        datosSinCTA.CuentaID = row["CuentaID"].ToString();
                                        datosSinCTA.Descripcion = row["Descripcion"].ToString();
                                        datosSinCTA.Nivel = Nvl;
                                        datosSinCTA.CTASAT = row["CTASAT"].ToString();
                                        lstdatosSinCTA.Add(datosSinCTA);
                                        ctarev.Add(row["Cuenta"].ToString().Substring(0, 4 * Nvl).PadRight(24, '0'), row["Cuenta"].ToString().Substring(0, 4 * Nvl).PadRight(24, '0'));
                                    }
                                }
                                Nvl = Nvl - 1;
                            }
                        }
                        if (lstdatosSinCTA.Count == 0)
                        {
                            List<string> res = llenaDSCuentas(FechaInicial.ToString(), FechaFinal.ToString(), EmpresaID, CuentaInicial.Replace("-", ""), CuentaFinal.Replace("-", ""), Nivel, int.Parse(Ingles), TipoEnvio, ExcluirDemandados);
                            return Entity.Response<object>.CrearResponse<object>(true, new { Genero = true,  FileName = res[0], Ruta = res[1] } );
                        }
                        else
                        {
                            Entity.Configuracion.Catempresa lstemp = new Entity.Configuracion.Catempresa();
                            string esquema = MobileBO.Utilerias.Serializar(lstdatosSinCTA);
                            HttpContext.Current.Session["Esquema"] = esquema;
                            return Entity.Response<object>.CrearResponse<object>(true, new { Genero = false, Imprimir = true, Mjs = "Existen cuentas pendientes por clasificar" });
                        }
                    }
                    else
                    {
                        string esquema = MobileBO.Utilerias.Serializar(lstdatosSinCTA);
                        HttpContext.Current.Session.Add("Esquema", esquema);
                        return Entity.Response<object>.CrearResponse<object>(true, new { Genero = false, Imprimir = true, Mjs = "Existen cuentas pendientes por clasificar" });
                    }
                }
                else
                {
                    return Entity.Response<object>.CrearResponse<object>(true, new { Genero = false, Imprimir = false, Mjs = "No se puede generar el archivo, porque la contabilidad no es Fiscal" });
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        public static List<string> llenaDSCuentas(string fechaInicial, string fechaFinal, string empresaid, string cuentaInicial, string cuentaFinal, string nivel, int Ingles, string TipoEnvio,bool ExcluirDemandados)
        {
            List<string> res = new List<string>();
            try
            {
                string empresaInicial, empresaFinal = "";
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                empresaInicial = empresa.Empresa.ToString();
                empresaFinal = empresa.Empresa.ToString();
                DateTime fi = DateTime.Parse(fechaInicial);
                DateTime ff = DateTime.Parse(fechaFinal);

                DataSet ds = new DataSet();
                ds = new MobileBO.ControlContabilidad().GeneraInformeSaldos(fi, ff, empresaInicial, empresaFinal, cuentaInicial, cuentaFinal, nivel, Ingles, 0, ExcluirDemandados);
                var dsSaldos = from a in ds.Tables[0].AsEnumerable()
                               select new
                               {
                                   Cuenta = a.Field<string>("cuenta").PadRight(24, '0'),
                                   SaldoInicial = a.Field<decimal>("saldoinicial"),
                                   Cargos = a.Field<decimal>("cargos"),
                                   Abonos = a.Field<decimal>("abonos"),
                                   SaldoFinal = (a.Field<string>("nat_cta") == "D" ? (a.Field<decimal>("saldoinicial") + a.Field<decimal>("cargos") - a.Field<decimal>("abonos")) : (a.Field<decimal>("saldoinicial") + a.Field<decimal>("abonos") - a.Field<decimal>("cargos")))
                               };

                //Armar el detalle del xml
                DateTime rfi = new DateTime(fi.Year, 01, 01);
                DateTime rff = new DateTime(fi.Year, 12, 31);

                string mes = (fi == rfi && ff == rff ? "13" : Convert.ToDateTime(fechaInicial).Month.ToString().PadLeft(2, '0'));
                string anio = Convert.ToDateTime(fechaInicial).Year.ToString();
                string fechaMB = Convert.ToDateTime(fechaInicial).ToString("yyyy-MM-dd");

                string directorio = Environment.GetEnvironmentVariable("TEMP");
                string fileName = empresa.Rfc + (fi == rfi && ff == rff ? fi.Year.ToString() + "13" : Convert.ToDateTime(fechaInicial).ToString("yyyyMM")) + "B" + TipoEnvio + ".xml";
                string saveLocation = directorio + "\\" + fileName;

                if (System.IO.File.Exists(saveLocation))
                    System.IO.File.Delete(saveLocation);
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(saveLocation))
                {
                    file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    StringBuilder xml = new StringBuilder();
                    xml.Append("<BCE:Balanza ");
                    if (TipoEnvio != "N")
                        xml.AppendFormat("Version=\"1.3\" RFC=\"{0}\" Mes=\"{1}\" Anio=\"{2}\" FechaModBal=\"{3}\" TipoEnvio=\"{4}\" ", empresa.Rfc, mes, anio, fechaMB, TipoEnvio);
                    else
                        xml.AppendFormat("Version=\"1.3\" RFC=\"{0}\" Mes=\"{1}\" Anio=\"{2}\" TipoEnvio=\"{3}\" ", empresa.Rfc, mes, anio, TipoEnvio);
                    xml.Append("xsi:schemaLocation=\"http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion/BalanzaComprobacion_1_3.xsd\" ");
                    xml.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:BCE=\"http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion\">");                    
                    file.WriteLine(xml.ToString());
                    foreach (var cuenta in dsSaldos)
                    {
                        file.WriteLine("<BCE:Ctas NumCta=\"" + cuenta.Cuenta + "\" SaldoIni=\"" + cuenta.SaldoInicial + "\" Debe=\"" + cuenta.Cargos + "\" Haber=\"" + cuenta.Abonos + "\" SaldoFin=\"" + cuenta.SaldoFinal + "\"/> ");
                    }
                    file.WriteLine("</BCE:Balanza>");
                }

                res.Add(fileName);
                res.Add(saveLocation);
                return res;
            }
            catch (Exception ex)
            {
                res.Add(ex.Message);
                return res;
            }
        }

    }

    public class ControladorReporteRelacionSaldos : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFinal = parametros.Get("fechaFinal");
            string empresaid = parametros.Get("empresaid");
            string cuentaInicial = parametros.Get("cuentaInicial").Replace("-", "");
            string cuentaFinal = parametros.Get("cuentaFinal").Replace("-", "");
            string nivel = parametros.Get("nivel");
            int Ingles = int.Parse(parametros.Get("Ingles"));
            bool ExcluirDemandados = bool.Parse(parametros.Get("ExcluirDemandados"));

            string empresaInicial, empresaFinal = "";
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
            empresaInicial = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            empresaFinal = empresa.Tables[0].Rows[0]["Empresa"].ToString();
            

            DataSet ds = new DataSet();
            ds = new MobileBO.ControlContabilidad().GeneraInformeSaldos(DateTime.Parse(fechaInicial), DateTime.Parse(fechaFinal), empresaInicial, empresaFinal, cuentaInicial, cuentaFinal, nivel, Ingles, 0, ExcluirDemandados);
            ds.Tables[0].TableName = "Datos";



            var level = (from a in ds.Tables[0].AsEnumerable()
                         //where a.Field<string>("afecta") == "0"
                         select new
                         {
                             Nivel = int.Parse(a.Field<string>("nivel"))
                         }).Distinct();


            //Calculamos los saldos
            var saldos = from a in ds.Tables[0].AsEnumerable()
                         where int.Parse(a.Field<string>("nivel")) == level.Min(c => c.Nivel)
                         group a by new
                         {
                             nivel = a.Field<string>("nivel")
                         } into grupo
                         select new
                         {
                             SaldoInicial = grupo.Sum(x => x.Field<decimal>("saldoinicial")),
                             Cargos = grupo.Sum(x => x.Field<decimal>("cargos")),
                             Abonos = grupo.Sum(x => x.Field<decimal>("abonos")),
                             SaldoFinal = grupo.Sum(x => x.Field<decimal>("SALDOFINAL"))
                         };
            DataTable Saldos = saldos.ToDataTable();

            //Traemos los datos de empresa
            //Entity.Configuraciones.Amdsucursalesempresa SucursalEmpresa = new MobileBO.ControlConfiguraciones().TraerAmdsucursalesempresas(sucursal_id);
            //Entity.Configuraciones.Amdconfiguracionessucursal DatoEmpresa = new MobileBO.ControlConfiguraciones().TraerAmdconfiguracionessucursalesPorSucursal(sucursal_id);

            //Parametros del reporte
            DataTable tablaParametros = new DataTable();
            tablaParametros.TableName = "Parametros";
            tablaParametros.Columns.Add("FechaInicial", typeof(DateTime));
            tablaParametros.Columns.Add("FechaFinal", typeof(DateTime));
            tablaParametros.Columns.Add("NombreEmpresa", typeof(string));
            tablaParametros.Columns.Add("Rfc", typeof(string));
            tablaParametros.Columns.Add("Domicilio", typeof(string));
            tablaParametros.Columns.Add("SaldoInicial", typeof(decimal));
            tablaParametros.Columns.Add("Cargos", typeof(decimal));
            tablaParametros.Columns.Add("Abonos", typeof(decimal));
            tablaParametros.Columns.Add("SaldoFinal", typeof(decimal));
            tablaParametros.Columns.Add("Ingles", typeof(int));
            tablaParametros.Columns.Add("FechaInicialTexto", typeof(string));
            tablaParametros.Columns.Add("FechaFinalTexto", typeof(string));

            if (Saldos.Rows.Count > 0)
            {
                //Llenamos los parametros que le pasaremos al reporte
                DataRow row = tablaParametros.NewRow();
                row[0] = DateTime.Parse(fechaInicial);
                row[1] = DateTime.Parse(fechaFinal);
                row[2] = empresa.Tables[0].Rows[0]["Descripcion"].ToString();
                row[3] = empresa.Tables[0].Rows[0]["Rfc"].ToString();
                row[4] = empresa.Tables[0].Rows[0]["DomicilioCompleto"].ToString() + " TEL: " + empresa.Tables[0].Rows[0]["Telefono"].ToString();
                row[5] = Saldos.Rows[0][0];
                row[6] = Saldos.Rows[0][1];
                row[7] = Saldos.Rows[0][2];
                row[8] = Saldos.Rows[0][3];
                row[9] = Ingles;
                row[10] = DateTime.Parse(fechaInicial).ToString("dd DE MMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper();
                row[11] = DateTime.Parse(fechaFinal).ToString("dd DE MMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper();

                tablaParametros.Rows.Add(row);
                ds.Tables.Add(tablaParametros);
            }
            try
            {
                base.NombreReporte = (int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? "ReporteRelacionSaldosExcel" : "ReporteRelacionSaldos");
                base.FormatoReporte = int.Parse(parametros.Get("Formato").Split(',')[1]);
                base.RutaReporte = "Contabilidad\\Reportes";
                ds.Tables.Add(empresa.Tables[0].Copy());
                base.DataSource = ds;
                //base.DataSource.WriteXml("c:\\Reportes\\ReporteRelacionSaldos.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }


    public class ControladorReporteCtaSATSinClasificar : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                string EmpresaID = parametros.Get("empresaid");
                string Esquema = HttpContext.Current.Session["Esquema"].ToString();

                DataSet dsEsquema = new DataSet();

                List<Entity.ModeloDatosCuentasSAT> lst = MobileBO.Utilerias.Deserializar<List<Entity.ModeloDatosCuentasSAT>>(Esquema);
                DataTable dtdatos = lst.ToDataTable();

                dsEsquema.Tables.Add(dtdatos);
                dsEsquema.Tables[0].TableName = "DatosCuentas";

                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(EmpresaID);
                dsEsquema.Tables.Add(dsEmpresa.Tables[0].Copy());
                dsEsquema.Tables[1].TableName = "Table";

                base.NombreReporte = "rptCtaSATPendientes";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = dsEsquema;
                //base.DataSource.WriteXml("c:\\Reportes\\rptCtaSATPendientes.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }
}