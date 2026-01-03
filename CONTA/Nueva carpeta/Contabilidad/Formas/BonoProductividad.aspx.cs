using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Homex.Core.Utilities;
using Entity;
namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class BonoProductividad : System.Web.UI.Page
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaVendedorFindByCode(string value)
        {
            Entity.Analisis.Catvendedor vendedor;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                vendedor = MobileBO.ControlAnalisis.TraerCatvendedoresPorCodigo(int.Parse(values.Codigo));
                if (vendedor != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = vendedor.Vendedorid, Codigo = vendedor.Codigovendedor.ToString(), Descripcion = vendedor.Nombre };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaVendedorFindByPopUp(string value)
        {
            Entity.ListaDeEntidades<Entity.Analisis.Catvendedor> listaVendedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaVendedores = MobileBO.ControlAnalisis.TraerCatvendedoresPorNombre(values.Descripcion);
                if (listaVendedores != null)
                {
                    foreach (Entity.Analisis.Catvendedor vendedor in listaVendedores)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = vendedor.Vendedorid, Codigo = vendedor.Codigovendedor.ToString(), Descripcion = vendedor.Nombre + ' ' + vendedor.Apellidopaterno + ' ' + vendedor.Apellidomaterno };
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


    public class ControladorReporteComisiones : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));
            string vendedorid = parametros.Get("vendedorid");

            try
            {
                ListaDeEntidades<Entity.Analisis.Catvendedor> ListaVendedores = new ListaDeEntidades<Entity.Analisis.Catvendedor>();

                Entity.Analisis.Catvendedor vendedor = MobileBO.ControlAnalisis.TraerCatvendedores(vendedorid);
                Entity.Analisis.Catgerente gerente = MobileBO.ControlAnalisis.TraerCatgerentes(vendedor.Gerenteid);

                DataSet dsColocaciones = new DataSet();
                bool esGerente = false;

                if (vendedor.Codigovendedor == gerente.Codigo)
                {
                    esGerente = true;
                    ListaVendedores = MobileBO.ControlAnalisis.TraerCatvendedoresPorGerente(gerente.Gerenteid);
                }
                else
                {
                    ListaVendedores.Add(vendedor);
                }

                DataSet ds = new DataSet();
                foreach (Entity.Analisis.Catvendedor vend in ListaVendedores)
                {
                    ds.Merge(MobileBO.ControlOperacion.TraerReporteComisiones(empresaid, fechaInicial, fechaFinal, vend.Vendedorid).Tables[0].Copy());
                    dsColocaciones.Merge(MobileBO.ControlOperacion.TraerColocacionesCtesNuevos(empresaid, fechaInicial, fechaFinal, vend.Vendedorid).Tables[0].Copy());
                }

                List<ReporteComisionDetalle> ListaDetalleCesiones = new List<ReporteComisionDetalle>();
                ListaDetalleCesiones = (from a in ds.Tables[0].AsEnumerable()
                                        select new ReporteComisionDetalle
                                        {
                                            Codigo = a.Field<int>("Codigo"),
                                            NombreCompleto = a.Field<string>("NombreCompleto"),
                                            CesionID = a.Field<Guid>("CesionID").ToString(),
                                            Esporadico = a.Field<bool>("Esporadico"),
                                            CodigoVendedor = a.Field<int>("CodigoVendedor"),
                                            NombreVendedor = a.Field<string>("NombreVendedor"),
                                            CodigoEquipo = a.Field<int>("CodigoEquipo"),
                                            NombreEquipo = a.Field<string>("NombreEquipo"),
                                            VendedorOrigen = a.Field<int>("VendedorOrigen"),
                                            Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                            Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                            Fecha_Apli = a.Field<DateTime>("Fecha_Apli"),
                                            Comision = a.Field<decimal>("Comision"),
                                            Bonificacion = a.Field<decimal>("Bonificacion"),
                                            Interes_Ordinario = a.Field<decimal>("Interes_Ordinario"),
                                            Comision_Disposicion = a.Field<decimal>("Comision_Disposicion"),
                                            Comision_Analisis = a.Field<decimal>("Comision_Analisis"),
                                            Interes_Moratorio = a.Field<decimal>("Interes_Moratorio")
                                        }).OrderBy(x => x.Codigo).ToList();

                List<DatosReporteComision> DatosReporte = new List<DatosReporteComision>();
                if (ListaDetalleCesiones.Count > 0)
                {
                    foreach (ReporteComisionDetalle pago in ListaDetalleCesiones)
                    {
                        decimal Comision = pago.Comision;
                        bool esporadico = pago.Esporadico;
                        /* Se comenta por que se va a hablar con gaspar si se deja a los 60 dias de comision o se reducen los dias a la duracion de la Cesion
                        //Preguntamos si es una cesion vencida para descontarle los dias Extras
                        if (pago.Fecha_Apli > pago.Fecha_Vence)
                        {   
                            DateTime fechaAux = pago.Fecha_Vence.AddDays(1);
                            while (fechaAux <= pago.Fecha_Apli)
                            {
                                ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(pago.CesionID, fechaAux, true);
                                Comision -= (Saldo.CostoDiario * 2);
                                if (Comision <= 0)
                                {
                                    Comision = 0;
                                    break;
                                }
                                fechaAux = fechaAux.AddDays(1);
                            }
                        }
                         */
                        //Forma en que tiene el calculo jose luis
                        if (pago.Fecha_Apli > pago.Fecha_Vence)
                        {
                            TimeSpan tsDT = pago.Fecha_Apli - pago.Fecha_Docu;
                            TimeSpan tsDM = pago.Fecha_Apli - pago.Fecha_Vence;
                            int DT = tsDT.Days;
                            int DM = tsDM.Days * 2;
                            if (DM <= 60)
                                Comision = (Comision / DT) * (DT - DM);
                            else
                                Comision = 0m;
                        }

                        int FactorComision = 0;
                        //Pregunta que tipo de comision le corresponde.
                        if (vendedor.Codigovendedor == pago.CodigoEquipo)
                        {
                            if (vendedor.Codigovendedor == pago.VendedorOrigen)
                                FactorComision = 5;
                            else
                                FactorComision = 3;
                        }
                        else
                        {
                            if (vendedor.Codigovendedor == pago.VendedorOrigen)
                                FactorComision = 2;
                            else
                                FactorComision = 1;
                        }
                        //Preguntamos si el cliente ya fue agregado a la lista anteriormente
                        if (DatosReporte.Find(x => x.Codigo == pago.Codigo) != null)
                        {
                            if (FactorComision == 1)
                                DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision1 += Comision;
                            if (FactorComision == 2)
                                DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision2 += Comision;
                            if (FactorComision == 3) {
                                if (esporadico)
                                    DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision3Esporadico += Comision;
                                else
                                    DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision3 += Comision;
                            }
                                
                            if (FactorComision == 5) {
                                if (esporadico)
                                    DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision3Esporadico += Comision;
                                else
                                    DatosReporte.Find(x => x.Codigo == pago.Codigo).Comision5 += Comision;
                            }   
                        }
                        else
                        {
                            DatosReporteComision rowReporte = new DatosReporteComision();
                            rowReporte.Codigo = pago.Codigo;
                            rowReporte.NombreCompleto = pago.NombreCompleto;
                            if (FactorComision == 1)
                                rowReporte.Comision1 += Comision;
                            if (FactorComision == 2)
                                rowReporte.Comision2 += Comision;
                            if (FactorComision == 3)
                            {
                                if (esporadico)
                                    rowReporte.Comision3Esporadico += Comision;
                                else
                                    rowReporte.Comision3 += Comision;
                            }
                                
                            if (FactorComision == 5)
                            {
                                if (esporadico)
                                    rowReporte.Comision3Esporadico += Comision;
                                else
                                    rowReporte.Comision5 += Comision;
                            }   
                            DatosReporte.Add(rowReporte);
                        }
                    }
                }

                List<DatosReporteComisionCtesColocados> ClientesColocados = new List<DatosReporteComisionCtesColocados>();
                DatosReporteComisionCtesColocados cteColocacion = new DatosReporteComisionCtesColocados();
                cteColocacion.NumeroClientesColocados = 0;
                cteColocacion.ImporteColocado = 0m;

                if (!esGerente)
                {
                    foreach (DataRow item in dsColocaciones.Tables[0].Rows)
                    {
                        //Se metio esta linea de codigo por que estos clientes cambiaron solamente la razon social y el sistema los esta tomando como clientes nuevos
                        if (item["Codigo"].ToString() == "596" || item["Codigo"].ToString() == "592")
                            continue;
                        cteColocacion.NumeroClientesColocados = cteColocacion.NumeroClientesColocados + 1;
                        if (DatosReporte.Find(x => x.Codigo == int.Parse(item["Codigo"].ToString())) != null)
                        {
                            DatosReporte.Find(x => x.Codigo == int.Parse(item["Codigo"].ToString())).Colocacion += decimal.Parse(item["Financiamiento"].ToString());
                            cteColocacion.ImporteColocado = cteColocacion.ImporteColocado + decimal.Parse(item["Financiamiento"].ToString());
                        }
                        else
                        {
                            DatosReporteComision rowReporte = new DatosReporteComision();
                            rowReporte.Codigo = int.Parse(item["Codigo"].ToString());
                            rowReporte.NombreCompleto = item["NombreCompleto"].ToString();
                            rowReporte.Colocacion += decimal.Parse(item["Financiamiento"].ToString());
                            cteColocacion.ImporteColocado = cteColocacion.ImporteColocado + decimal.Parse(item["Financiamiento"].ToString());
                            DatosReporte.Add(rowReporte);
                        }
                    }
                }

                ClientesColocados.Add(cteColocacion);
                DataSet dsReporte = new DataSet();
                DataSet dsEmpresas = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                DataTable tEmpresas = dsEmpresas.Tables[0].Copy();
                tEmpresas.TableName = "DatosEmpresa";


                dsReporte.Tables.Add(tEmpresas);

                DataTable tbVend = new List<Entity.Analisis.Catvendedor>() { vendedor }.ToDataTable();
                tbVend.TableName = "DatosVendedor";
                dsReporte.Tables.Add(tbVend);

                DataTable rpt = DatosReporte.OrderBy(x => x.Codigo).ToDataTable();
                rpt.TableName = "DatosReporte";
                dsReporte.Tables.Add(rpt);

                DataTable tbfechas = new DataTable();
                tbfechas.Columns.Add("FechaInicial", typeof(DateTime));
                tbfechas.Columns.Add("FechaFinal", typeof(DateTime));
                tbfechas.TableName = "DatosFechas";
                DataRow row = tbfechas.NewRow();
                row[0] = fechaInicial; row[1] = fechaFinal;
                tbfechas.Rows.Add(row);
                dsReporte.Tables.Add(tbfechas);

                DataTable tbCtesColocados = new DataTable();
                tbCtesColocados = ClientesColocados.ToDataTable();
                tbCtesColocados.TableName = "ClientesColocados";
                dsReporte.Tables.Add(tbCtesColocados);


                //base.NombreReporte = "ReporteComisiones";
                //base.FormatoReporte = 3;
                base.NombreReporte = "ReporteComisiones";//(int.Parse(parametros.Get("Formato").Split(',')[1]) == 1 ? "ReporteComisionesExcel" : "ReporteComisiones");
                base.FormatoReporte = int.Parse(parametros.Get("Formato").Split(',')[1]);
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = dsReporte;
                dsReporte.WriteXml("c:\\Reportes\\ReporteComisiones.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte

    }


}