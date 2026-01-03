using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
using Homex.Core.Utilities;


namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteGerencial : System.Web.UI.Page
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
                listaElementos.Add(new { id = Guid.Empty.ToString(), nombre = "CONSOLIDADO" });
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
    }

    public class ControladorReporteAnexoCapitalInvertido : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                //OBTENEMOS LOS PARAMETROS DEL REPORTE
                DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
                DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));
                string empresaid = parametros.Get("empresaid");
                List<ModeloCapitalInvertidoAnexo> ListaCapitalInvertido = new List<ModeloCapitalInvertidoAnexo>();
                DateTime fechaAnterior = fechaInicial.AddDays(-1);
                while (fechaInicial <= fechaFinal)
                {
                    List<ModeloCesionesColocadasRG> DineroCapitalInvertido = (from a in MobileBO.ControlOperacion.CapitalInvertidoCostos(fechaInicial, (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null).Tables[0].AsEnumerable()
                                                                              select new ModeloCesionesColocadasRG
                                                                              {
                                                                                  CesionID = a.Field<Guid>("CesionID").ToString(),
                                                                                  Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                                                                  Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                                                                  TipoContrato = a.Field<string>("TipoContrato"),
                                                                                  Financiamiento = a.Field<decimal>("Saldo"),
                                                                                  DiasCesion = a.Field<int>("DiasCesion"),
                                                                                  Pagada = false
                                                                              }).ToList();

                    List<ModeloCesionesColocadasRG> DineroCapitalInvertidoAnt = (from a in MobileBO.ControlOperacion.CapitalInvertidoCostos(fechaInicial.AddYears(-1), (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null).Tables[0].AsEnumerable()
                                                                              select new ModeloCesionesColocadasRG
                                                                              {
                                                                                  CesionID = a.Field<Guid>("CesionID").ToString(),
                                                                                  Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                                                                  Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                                                                  TipoContrato = a.Field<string>("TipoContrato"),
                                                                                  Financiamiento = a.Field<decimal>("Saldo"),
                                                                                  DiasCesion = a.Field<int>("DiasCesion"),
                                                                                  Pagada = false
                                                                              }).ToList();







                    //ModeloCapitalInvertidoAnexo capitalinvertido = new ModeloCapitalInvertidoAnexo();
                    //capitalinvertido.Fecha = fechaInicial;
                    //capitalinvertido.Dato = "TOTAL";
                    //capitalinvertido.Factoraje = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "1").Sum(x => x.Financiamiento);
                    //capitalinvertido.Proveedor = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "2").Sum(x => x.Financiamiento);
                    //capitalinvertido.Financieras = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "4").Sum(x => x.Financiamiento);
                    //capitalinvertido.PeriodoAnt = DineroCapitalInvertidoAnt.Sum(x => x.Financiamiento);
                    //ListaCapitalInvertido.Add(capitalinvertido);

                    ////Menos 90 dias
                    //capitalinvertido = new ModeloCapitalInvertidoAnexo();
                    //capitalinvertido.Fecha = fechaInicial;
                    //capitalinvertido.Dato = "1-30 Días";
                    //capitalinvertido.Factoraje = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "1" && x.DiasCesion<=30).Sum(x => x.Financiamiento);
                    //capitalinvertido.Proveedor = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "2" && x.DiasCesion <= 30).Sum(x => x.Financiamiento);
                    //capitalinvertido.Financieras = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "4" && x.DiasCesion <= 30).Sum(x => x.Financiamiento);
                    //capitalinvertido.PeriodoAnt = DineroCapitalInvertidoAnt.FindAll(x => x.DiasCesion <= 30).Sum(x => x.Financiamiento);
                    //ListaCapitalInvertido.Add(capitalinvertido);

                    ////Mayor > 90 dias
                    //capitalinvertido = new ModeloCapitalInvertidoAnexo();
                    //capitalinvertido.Fecha = fechaInicial;
                    //capitalinvertido.Dato = "31-90 Días";
                    //capitalinvertido.Factoraje = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "1" && x.DiasCesion > 30).Sum(x => x.Financiamiento);
                    //capitalinvertido.Proveedor = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "2" && x.DiasCesion > 30).Sum(x => x.Financiamiento);
                    //capitalinvertido.Financieras = DineroCapitalInvertido.FindAll(x => x.TipoContrato == "4" && x.DiasCesion > 30).Sum(x => x.Financiamiento);
                    //capitalinvertido.PeriodoAnt = DineroCapitalInvertidoAnt.FindAll(x => x.DiasCesion > 30).Sum(x => x.Financiamiento);
                    //ListaCapitalInvertido.Add(capitalinvertido);




                    fechaInicial = fechaInicial.AddDays(1);
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(ListaCapitalInvertido.ToDataTable());

                DataTable param = new DataTable();
                param.Columns.Add("FechaInicial", typeof(DateTime));
                param.Columns.Add("FechaFinal", typeof(DateTime));
                param.TableName = "Parametros";
                DataRow rparam = param.NewRow();
                rparam["FechaInicial"] = DateTime.Parse(parametros.Get("fechaInicial"));
                rparam["FechaFinal"] = fechaFinal;
                param.Rows.Add(rparam);
                ds.Tables.Add(param);


                base.NombreReporte = "ReporteGerencialAnexoCapital";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteGerencialAnexoCapital.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }



    public class ControladorReporteGerencialReport : Base.Clases.BaseReportes
    {

        public static decimal TraerTipoCambio(DateTime Fecha)
        {
            Entity.Contabilidad.Cattipocambio TipoCambio = null;            
            while (TipoCambio == null)
            {
                TipoCambio = MobileBO.ControlContabilidad.TraerCattipocambio(Fecha);
                if (TipoCambio == null)
                    Fecha = Fecha.AddDays(-1);
            }
            return TipoCambio.Importetipocambio;
        }

        public static decimal TraerTasaCete(DateTime Fecha)
        {
            Entity.Contabilidad.Catcetestiie TraerCatcetestiie = null;
            while (TraerCatcetestiie == null)
            {
                TraerCatcetestiie = MobileBO.ControlContabilidad.TraerCatcetestiie(null, Fecha.Year, Fecha.Month);
                if (TraerCatcetestiie == null)
                    Fecha = Fecha.AddMonths(-1);
            }
            return TraerCatcetestiie.Cetes;
        }

        public static decimal TraerTasaTiie(DateTime Fecha,int CreditoFinancieroID)
        {
            Entity.Contabilidad.Creditosfinancierostasasind Tasa = null;
            while (Tasa == null)
            {
                Tasa = MobileBO.ControlContabilidad.TraerCreditosfinancierostasasind(null, CreditoFinancieroID, Fecha.Year, Fecha.Month);
                if (Tasa == null)
                    Fecha = Fecha.AddMonths(-1);
            }
            return Tasa.Tasa;
        }

        #region Inicializa Reporte

        public void CalculaTasaDeInteresReal(ref ModeloCesionesColocadasRG Cesion, DateTime fechaFinal)
        {
            try
            {
                decimal FactorInteresOrdinario = 0m, FactorComisionDisp = 0m, FactorComisionAnalisis = 0m;
                decimal factor_diario = 0m, factor_diario_moratorio = 0m;

                if (Cesion.Demandado) {
                    fechaFinal = Cesion.FechaDemanda;
                }

                if (Cesion.Sofom)
                {
                    FactorInteresOrdinario = Math.Round(Cesion.FactorInteresOrdinario / 100, 10);
                    FactorComisionDisp = Math.Round(Cesion.FactorComisionDisp / 100, 6);
                    FactorComisionAnalisis = Math.Round(Cesion.FactorComisionAnalisis / 100, 6);
                    factor_diario = FactorInteresOrdinario + FactorComisionDisp + FactorComisionAnalisis;
                    factor_diario_moratorio = Math.Round(Cesion.FactorMoratorio / 100, 6) - (FactorInteresOrdinario + FactorComisionDisp + FactorComisionAnalisis);
                }
                else
                {
                    if (Cesion.TipoContrato == "1")
                        factor_diario = Math.Round(Cesion.FactorInteresOrdinario / 100, 10);
                    else
                        factor_diario = Math.Round((5m / 30m) / 100m, 10);

                    factor_diario_moratorio = Math.Round(Cesion.FactorMoratorio / 100, 10);
                    //Ajustamos las tasas de interes para cobrarlas sobre el capital y no sobre el importe de las facturas(Para que sea igual que balor)
                    factor_diario = Math.Round(factor_diario / 0.9m, 6);
                    factor_diario_moratorio = Math.Round(factor_diario_moratorio / 0.9m, 5);
                }

                List<Entity.Operacion.Cesionesdetalle> Pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(Cesion.CesionID, 11, fechaFinal);

                int diasAcum = 0;
                DateTime UltimoPago = Cesion.Fecha_Docu;
                decimal CapInvAcum = 0m;
                decimal TAcum = 0m;
                decimal Financiamiento = Cesion.Financiamiento;

                //Modificacion para meter el interes generado por separado.

                decimal CapInvAcumOrd = 0m;
                decimal TAcumOrd = 0m;
                int diasAcumOrd = 0;

                decimal CapInvAcumMor = 0m;
                decimal TAcumMor = 0m;
                int diasAcumMor = 0;





                foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
                {

                    if (pago.FechaApli <= Cesion.Fecha_Vence)
                    {
                        CapInvAcum += Financiamiento * (pago.FechaApli - UltimoPago).Days;
                        TAcum += (pago.FechaApli - UltimoPago).Days * factor_diario;
                        diasAcum += (pago.FechaApli - UltimoPago).Days;

                        //Ordinario
                        CapInvAcumOrd += Financiamiento * (pago.FechaApli - UltimoPago).Days;
                        TAcumOrd += (pago.FechaApli - UltimoPago).Days * factor_diario;
                        diasAcumOrd += (pago.FechaApli - UltimoPago).Days;


                        Financiamiento -= pago.Financiamiento;
                    }
                    else
                    {
                        //Acumulamos los intereses que se generaron antes de la fecha de vencimiento de la cesion
                        if (UltimoPago < Cesion.Fecha_Vence)
                        {
                            CapInvAcum += Financiamiento * (Cesion.Fecha_Vence - UltimoPago).Days;
                            TAcum += (Cesion.Fecha_Vence - UltimoPago).Days * factor_diario;
                            diasAcum += (Cesion.Fecha_Vence - UltimoPago).Days;

                            //Ordinario
                            CapInvAcumOrd += Financiamiento * (Cesion.Fecha_Vence - UltimoPago).Days;
                            TAcumOrd += (Cesion.Fecha_Vence - UltimoPago).Days * factor_diario;
                            diasAcumOrd += (Cesion.Fecha_Vence - UltimoPago).Days;


                            UltimoPago = Cesion.Fecha_Vence;
                        }

                        CapInvAcum += Financiamiento * (pago.FechaApli - UltimoPago).Days;
                        TAcum += (pago.FechaApli - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                        diasAcum += (pago.FechaApli - UltimoPago).Days;

                        //Moratorio
                        CapInvAcumMor += Financiamiento * (pago.FechaApli - UltimoPago).Days;
                        TAcumMor += (pago.FechaApli - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                        diasAcumMor += (pago.FechaApli - UltimoPago).Days;

                        Financiamiento -= pago.Financiamiento;
                    }
                    UltimoPago = pago.FechaApli;
                }



                if (!Cesion.Pagada)
                {
                    if (UltimoPago < Cesion.Fecha_Vence)
                    {
                        if (Cesion.Fecha_Vence < fechaFinal)
                        {

                            CapInvAcum += Financiamiento * (Cesion.Fecha_Vence - UltimoPago).Days;
                            TAcum += (Cesion.Fecha_Vence - UltimoPago).Days * factor_diario;
                            diasAcum += (Cesion.Fecha_Vence - UltimoPago).Days;

                            //Ordinario
                            CapInvAcumOrd += Financiamiento * (Cesion.Fecha_Vence - UltimoPago).Days;
                            TAcumOrd += (Cesion.Fecha_Vence - UltimoPago).Days * factor_diario;
                            diasAcumOrd += (Cesion.Fecha_Vence - UltimoPago).Days;

                            UltimoPago = Cesion.Fecha_Vence;

                            CapInvAcum += Financiamiento * (fechaFinal - UltimoPago).Days;
                            TAcum += (fechaFinal - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                            diasAcum += (fechaFinal - UltimoPago).Days;

                            //Moratorio
                            CapInvAcumMor += Financiamiento * (fechaFinal - UltimoPago).Days;
                            TAcumMor += (fechaFinal - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                            diasAcumMor += (fechaFinal - UltimoPago).Days;

                        }
                        else
                        {
                            CapInvAcum += Financiamiento * (fechaFinal - UltimoPago).Days;
                            TAcum += (fechaFinal - UltimoPago).Days * factor_diario;
                            diasAcum += (fechaFinal - UltimoPago).Days;

                            //Ordinario
                            CapInvAcumOrd += Financiamiento * (fechaFinal - UltimoPago).Days;
                            TAcumOrd += (fechaFinal - UltimoPago).Days * factor_diario;
                            diasAcumOrd += (fechaFinal - UltimoPago).Days;

                        }
                    }
                    else
                    {
                        CapInvAcum += Financiamiento * (fechaFinal - UltimoPago).Days;
                        TAcum += (fechaFinal - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                        diasAcum += (fechaFinal - UltimoPago).Days;

                        //Moratorio
                        CapInvAcumMor += Financiamiento * (fechaFinal - UltimoPago).Days;
                        TAcumMor += (fechaFinal - UltimoPago).Days * (factor_diario + factor_diario_moratorio);
                        diasAcumMor += (fechaFinal - UltimoPago).Days;

                    }
                }

                if (diasAcum > 0)
                {
                    decimal CapInvProDia = CapInvAcum / (diasAcum);
                    decimal IntProGen = CapInvProDia * TAcum;
                    decimal TasaDeIntGenPro = ((IntProGen / CapInvProDia) / (diasAcum));
                    TasaDeIntGenPro = Math.Round(TasaDeIntGenPro * 30 * 100, 3);
                    Cesion.Tasa = TasaDeIntGenPro;
                }
                else
                    Cesion.Tasa = 0m;

                //Interes Ordinario
                if (diasAcumOrd > 0)
                {
                    decimal CapInvProDia = CapInvAcumOrd / (diasAcumOrd);
                    decimal IntProGen = CapInvProDia * TAcumOrd;
                    decimal TasaDeIntGenPro = ((IntProGen / CapInvProDia) / (diasAcumOrd));
                    TasaDeIntGenPro = Math.Round(TasaDeIntGenPro * 30 * 100, 3);
                    Cesion.TasaInteresOrdinaria = TasaDeIntGenPro;
                    Cesion.DiasOrdinarios = diasAcumOrd;
                }
                else
                    Cesion.TasaInteresOrdinaria = 0m;

                //Interes Moratorio
                if (diasAcumMor > 0)
                {
                    decimal CapInvProDia = CapInvAcumMor / (diasAcumMor);
                    decimal IntProGen = CapInvProDia * TAcumMor;
                    decimal TasaDeIntGenPro = ((IntProGen / CapInvProDia) / (diasAcumMor));
                    TasaDeIntGenPro = Math.Round(TasaDeIntGenPro * 30 * 100, 3);
                    Cesion.TasaInteresMoratoria = TasaDeIntGenPro;
                    Cesion.DiasMoratorios = diasAcumMor;
                }
                else
                    Cesion.TasaInteresMoratoria = 0m;




            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                System.Console.Write(mensaje);
            }
        }



        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                //throw new Exception("Detenemos el reporte para imprimir el detalle del capital invertido");
                //OBTENEMOS LOS PARAMETROS DEL REPORTE
                DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
                DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));
                string empresaid = parametros.Get("empresaid");

                DateTime fechaInicioPdoAnt = fechaInicial.AddYears(-1);
                DateTime fechaFinalPdoAnt = fechaFinal.AddYears(-1);
                DateTime fechaInicioAño = new DateTime(fechaInicial.Year, 1, 1);


                #region Intereses Financieros                
                List<InteresesCostosFinancieros> ListaInteresesCreditos = new List<InteresesCostosFinancieros>();
                List<Entity.Contabilidad.Creditosfinanciero> CreditosFinancieros = MobileBO.ControlContabilidad.TraerCreditosfinancierosPorFecha(fechaFinal);
                foreach (Entity.Contabilidad.Creditosfinanciero Credito in CreditosFinancieros)
                {
                    if (Credito.Creditofinancieroid == 7)
                        continue;
                    decimal InteresGeneradoPerido = 0m;
                    decimal InteresPesos = 0m;
                    decimal Capital = 0m;
                    decimal CapitalPesos = 0m;

                    List<Entity.Contabilidad.Creditosfinancierosdetalle> Disposiciones = new List<Entity.Contabilidad.Creditosfinancierosdetalle>();
                    List<Entity.Contabilidad.Creditosfinancierosdetalle> PagosCreditos = new List<Entity.Contabilidad.Creditosfinancierosdetalle>();

                    if (Credito.Calculointeres == "Disposicion")
                    {
                        Disposiciones = MobileBO.ControlContabilidad.Creditosfinancierosdetalle_SelectTipoFecha(Credito.Creditofinancieroid, 1, fechaFinal);
                        PagosCreditos = MobileBO.ControlContabilidad.Creditosfinancierosdetalle_SelectTipoFecha(Credito.Creditofinancieroid, 11, fechaFinal);
                    }

                    if (Credito.Calculointeres == "CapitalContable")
                    {
                        Disposiciones = MobileBO.ControlContabilidad.TraerDetalleCreditosFinancierosContabilidad(Credito.Creditofinancieroid, 1, fechaFinal);
                        PagosCreditos = MobileBO.ControlContabilidad.TraerDetalleCreditosFinancierosContabilidad(Credito.Creditofinancieroid, 11, fechaFinal);
                    }


                    DateTime FechaInicialAux = fechaInicial;
                    //Calculo diario de intereses

                    while (FechaInicialAux <= fechaFinal)
                    {
                        DateTime InicioMes = new DateTime(FechaInicialAux.Year, FechaInicialAux.Month, 1);
                        DateTime FinMes = InicioMes.AddMonths(1).AddDays(-1);
                        Capital = Disposiciones.FindAll(x => x.FechaApli <= FechaInicialAux).Sum(x => x.Financiamiento);
                        decimal Abonos = PagosCreditos.FindAll(x => x.FechaApli <= FechaInicialAux).Sum(x => x.Financiamiento);
                        Capital = Capital - Abonos;
                        decimal Interes = (Capital * ((Credito.Tipocredito == "TasaFija" ? Credito.Tasainteres : Credito.Puntos + (Credito.Tipocredito == "Cetes" ? TraerTasaCete(FinMes) : TraerTasaTiie(FinMes, Credito.Creditofinancieroid))) / 100m)) / Credito.Diasañotasa;
                        //Interes = Interes * ((FinMes - InicioMes).Days + 1);
                        //Le vamos a agregar un 10% a los creditos de andorra pero tenemos que ver si esto aplica para todos los casos
                        if (Credito.Moneda == "DLS")
                            Interes = Interes / 0.9m;
                        InteresGeneradoPerido += Interes;
                        if (Credito.Moneda == "DLS")
                        {
                            InteresPesos += Interes * TraerTipoCambio(FinMes);
                            CapitalPesos = Capital * TraerTipoCambio(FinMes);
                        }
                        else
                            InteresPesos = InteresGeneradoPerido;
                        FechaInicialAux = FechaInicialAux.AddDays(1);
                    }
                    InteresesCostosFinancieros InteresGenerado = new InteresesCostosFinancieros();
                    InteresGenerado.CreditoFinancieroID = Credito.Creditofinancieroid;
                    InteresGenerado.BancoCreditoID = Credito.Bancocreditoid;
                    InteresGenerado.EmpresaID = Credito.Empresaid;
                    if (Credito.Moneda == "DLS")
                    {
                        InteresGenerado.CapitalActualDLLS = Capital;
                        InteresGenerado.CapitalActualPesosDLLS = CapitalPesos;
                        InteresGenerado.InteresPeriodoDLLS = InteresGeneradoPerido;
                        InteresGenerado.InteresPeriodoPesosDLLS = InteresPesos;
                    }
                    else
                    {
                        InteresGenerado.CapitalActualPesos = Capital;
                        InteresGenerado.InteresPeriodoPesos = InteresGeneradoPerido;
                    }
                    ListaInteresesCreditos.Add(InteresGenerado);
                }

                //Generamos la estructura del resultado del reporte
                List<RG_InteresFinanciero> ResultadoInteresFinanciero = new List<RG_InteresFinanciero>();
                //Este codigo de comento por que estaba agrupando por empresa y se va a hacer de una nueva manera para no enrredar tanto con los datos del detalle de los creditos financieros
                //ResultadoInteresFinanciero = (from a in ListaInteresesCreditos
                //                              group a by new
                //                              {
                //                                  Empresaid = a.EmpresaID
                //                              } into Grupo
                //                              select new RG_InteresFinanciero
                //                              {
                //                                  Empresa = Grupo.Key.Empresaid,
                //                                  CapitalActualDLLS = Grupo.Sum(x => x.CapitalActualDLLS),
                //                                  CapitalActualPesosDLLS = Grupo.Sum(x => x.CapitalActualPesosDLLS),
                //                                  InteresPeriodoDLLS = Grupo.Sum(x => x.InteresPeriodoDLLS),
                //                                  InteresPeriodoPesosDLLS = Grupo.Sum(x => x.InteresPeriodoPesosDLLS),
                //                                  CapitalActualPesos = Grupo.Sum(x => x.CapitalActualPesos),
                //                                  InteresPeriodoPesos = Grupo.Sum(x => x.InteresPeriodoPesos)
                //                              }).ToList();
                ResultadoInteresFinanciero = (from x in ListaInteresesCreditos
                                              select new RG_InteresFinanciero
                                              {
                                                  Empresa = x.EmpresaID,
                                                  BancoCreditoID = x.BancoCreditoID,
                                                  CapitalActualDLLS = x.CapitalActualDLLS,
                                                  CapitalActualPesosDLLS = x.CapitalActualPesosDLLS,
                                                  InteresPeriodoDLLS = x.InteresPeriodoDLLS,
                                                  InteresPeriodoPesosDLLS = x.InteresPeriodoPesosDLLS,
                                                  CapitalActualPesos = x.CapitalActualPesos,
                                                  InteresPeriodoPesos = x.InteresPeriodoPesos
                                              }).ToList();

                foreach (RG_InteresFinanciero elemento in ResultadoInteresFinanciero)
                {
                    elemento.Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(elemento.Empresa).Descripcion;
                    elemento.Banco = MobileBO.ControlContabilidad.TraerCatbancoscreditos(elemento.BancoCreditoID).Banco;
                    elemento.TotalCapitalActual = elemento.CapitalActualPesosDLLS + elemento.CapitalActualPesos;
                    elemento.TotalInteresPeriodo = elemento.InteresPeriodoPesos + elemento.InteresPeriodoPesosDLLS;
                }

                //throw new Exception("Proceso cancelado ya que termino de calcular los intereses");
                #endregion
                long meses = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Month, fechaInicial, fechaFinal);
                #region Colocaciones
                List<ModeloCesionesColocadasRG> DineroColocado = MobileBO.ControlOperacion.TraerCesionesReporteCostos((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaInicial, fechaFinal);
                List<ModeloCesionesColocadasRG> DineroColocadoAcumulado = MobileBO.ControlOperacion.TraerCesionesReporteCostos((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaInicioAño, fechaFinal);
                List<ModeloCesionesColocadasRG> DineroColocadoPeriodoAnt = MobileBO.ControlOperacion.TraerCesionesReporteCostos((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaInicioPdoAnt, fechaFinalPdoAnt);
                List<ModeloCesionesColocadasRG> DineroColocadoMesAnterior = new List<ModeloCesionesColocadasRG>();
                List<ModeloCesionesColocadasRG> DineroCapitalInvertido = (from a in MobileBO.ControlOperacion.CapitalInvertidoCostos(fechaFinal, (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null).Tables[0].AsEnumerable()
                                                                          select new ModeloCesionesColocadasRG
                                                                          {
                                                                              CesionID = a.Field<Guid>("CesionID").ToString(),
                                                                              Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                                                              Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                                                              TipoContrato = a.Field<string>("TipoContrato"),
                                                                              Financiamiento = a.Field<decimal>("Saldo"),
                                                                              DiasCesion = a.Field<int>("DiasCesion"),
                                                                              Pagada = false
                                                                          }).ToList();

                List<ModeloCesionesColocadasRG> DineroCapitalInvertidoInicial = (from a in MobileBO.ControlOperacion.CapitalInvertidoCostos(fechaInicial.AddDays(-1), (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null).Tables[0].AsEnumerable()
                                                                                 select new ModeloCesionesColocadasRG
                                                                                 {
                                                                                     CesionID = a.Field<Guid>("CesionID").ToString(),
                                                                                     Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                                                                     Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                                                                     TipoContrato = a.Field<string>("TipoContrato"),
                                                                                     Financiamiento = a.Field<decimal>("Saldo"),
                                                                                     DiasCesion = a.Field<int>("DiasCesion"),
                                                                                     Pagada = false
                                                                                 }).ToList();

                //Vamos por el dinero de los clientes demandados para considerarlos en la cartera vencida mas abajo.


                DataSet dsDemandadosInicial = MobileBO.ControlOperacion.TraerSaldoDemandadoEIncobrableDelPeriodo((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), DateTime.Parse("01/01/2000"), fechaInicial.AddDays(-1));
                DataSet dsDemandados = MobileBO.ControlOperacion.TraerSaldoDemandadoEIncobrableDelPeriodo((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaInicial, fechaFinal);







                //Si es reporte es de un mes vamos por la informacion de las cesiones coladas del mes anterior.
                if (meses == 0)
                {
                    int DiasDelMes = DateTime.DaysInMonth(fechaInicial.Year, fechaInicial.Month);
                    int DiasReporte = (fechaFinal - fechaInicial).Days + 1; // Le sumamos 1 para que nos de los dias completos
                    DateTime FechaAuxAnt = DateTime.MinValue;
                    //El reporte es mensual y el procedimiento de las fechas siguie igual
                    if (DiasReporte == DiasDelMes)
                    {
                        FechaAuxAnt = fechaInicial.AddMonths(-1);
                        FechaAuxAnt = new DateTime(FechaAuxAnt.Year, FechaAuxAnt.Month, 1);
                        DineroColocadoMesAnterior = MobileBO.ControlOperacion.TraerCesionesReporteCostos((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), FechaAuxAnt, fechaInicial.AddDays(-1));
                    }
                    else
                    {
                        FechaAuxAnt = fechaInicial.AddDays(DiasReporte * -1);
                        DineroColocadoMesAnterior = MobileBO.ControlOperacion.TraerCesionesReporteCostos((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), FechaAuxAnt, fechaInicial.AddDays(-1));
                    }
                }

                var Clientes = (from a in DineroColocado
                                select new
                                {
                                    ClienteID = a.ClienteID
                                }).Distinct();
                List<Entity.ModeloDineroNuevoColocado> colocadoNuevo = new List<Entity.ModeloDineroNuevoColocado>();
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "1", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "2", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "3", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "4", Nuevo = 0m });

                //Aqui obtenemos el dinero nuevo colocado del periodo que se esta consultando
                foreach (var cliente in Clientes)
                {
                    List<SaldoDiarioPorCliente> SaldoDiario = MobileBO.ControlOperacion.TraerSaldoDiarioPorCliente(cliente.ClienteID, fechaFinal);
                    foreach (ModeloCesionesColocadasRG operacion in DineroColocado.FindAll(x => x.ClienteID == cliente.ClienteID))
                    {
                        decimal ColocadoHistoria = 0m;
                        if (SaldoDiario.FindAll(a => a.ClienteID == cliente.ClienteID && a.Fecha < operacion.Fecha_Docu).Count() > 0)
                            ColocadoHistoria = SaldoDiario.FindAll(a => a.ClienteID == cliente.ClienteID && a.Fecha < operacion.Fecha_Docu).Max(b => b.Saldo);
                        if (operacion.Financiamiento > ColocadoHistoria)
                            colocadoNuevo.Find(c => c.TipoContrato == operacion.TipoContrato).Nuevo += operacion.Financiamiento - ColocadoHistoria;
                    }
                }



                //Obtenemos los intereses generados de todas las cesiones
                foreach (ModeloCesionesColocadasRG Cesion in DineroColocado)
                {
                    ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, true);
                    if (Cesion.DiasCesion > 0)
                    {
                        if (Cesion.Sofom)
                        {
                            Cesion.Interes_Ordinario = Saldo.InteresOrdinario - Saldo.InteresOrdinarioM30D;
                            Cesion.Interes_Moratorio = Saldo.InteresMoratorio + Saldo.InteresOrdinarioM30D - Saldo.InteresMoratorioM90D;
                            Cesion.Comision_Disposicion = Saldo.ComisionDisposicion;
                            Cesion.Comision_Analisis = Saldo.ComisionAnalisis;
                        }
                        else
                        {
                            if (Cesion.Bonificacion > 0)
                                Cesion.Bonificacion = Cesion.Bonificacion / 1.16m;

                            Cesion.Interes_Ordinario = Saldo.InteresOrdinario - Saldo.InteresOrdinarioM30D;
                            if (Cesion.TipoContrato == "1")
                                Cesion.Interes_Moratorio = Saldo.InteresMoratorio + Saldo.InteresOrdinarioM30D;
                            else
                                Cesion.Interes_Moratorio = Saldo.InteresMoratorio + Saldo.InteresOrdinarioM30D - Saldo.InteresMoratorioM90D;

                        }
                    }
                    if (Cesion.Pagada && Saldo.Vencida == "SI" && Cesion.DiasCesion < 31)
                        Cesion.DiasCesion = 31;
                }
                
                //Calculamos la tasa de insteres generada para cada cesion colocada
                foreach (ModeloCesionesColocadasRG Cesion in DineroColocado)
                {
                    ModeloCesionesColocadasRG CesionAux = Cesion;
                    CalculaTasaDeInteresReal(ref CesionAux, fechaFinal);
                    Cesion.Tasa = CesionAux.Tasa;
                }
                //Obtenemos las cesiones por cobrar antes de la fecha de inicio del reporte
                DataSet CesionesPorCobrar = new DataSet();
                if (empresaid == string.Empty || empresaid == Guid.Empty.ToString())
                {
                    CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_Select(null, fechaInicial.AddDays(-1), null);
                }
                else
                {
                    Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                    CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_Select(null, fechaInicial.AddDays(-1), empresa.Empresa);
                }
                List<Entity.Operacion.Cesionesdetalle> PagosPeriodo = MobileBO.ControlOperacion.TraerPagosPorPeriodo((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null, 11, fechaInicial, fechaFinal);

                List<ModeloCesionesColocadasRG> ColocadoIngresoFueraDelPeriodo = new List<ModeloCesionesColocadasRG>();

                foreach (DataRow row in CesionesPorCobrar.Tables[0].Rows)
                {
                    ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                    string CesionID = row["CesionID"].ToString().ToUpper();
                    itemCesionIngreso.CesionID = CesionID;

                    DateTime FechaAuxInt = fechaInicial.AddDays(-1);
                    DateTime FechaFinalAux = fechaFinal;
                    //Verificamos si el cliente se encuentra DEMANDADO O ES CLIENTE INCOBRABLE
                    Entity.Operacion.Catclientesmoroso moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, row["EmpresaID"].ToString(), row["ClienteID"].ToString(), 1);
                    bool demandado = false;
                    if (moroso != null)
                    {
                        demandado = true;
                    }
                    else
                    {
                        moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, row["EmpresaID"].ToString(), row["ClienteID"].ToString(), 3);
                        if (moroso != null)
                        {
                            demandado = true;
                        }
                    }

                    //Calcular el ingreso de todo el periodo                    
                    ModeloSaldoCesion SaldoAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(CesionID, FechaAuxInt, true);
                    ModeloSaldoCesion SaldoActual;
                    List<Entity.Operacion.Cesionesdetalle> Pagos = PagosPeriodo.FindAll(x => x.Cesionid.ToUpper() == CesionID).OrderBy(a => a.FechaApli).ToList();
                    Pagos = Pagos.FindAll(x => x.FechaApli > FechaAuxInt);
                    foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
                    {
                        //Si la cesion tiene pagos sacamos el saldo un dia antes del pago realizado para despues sumarle el costo diario respectivamente.
                        SaldoActual = MobileBO.ControlOperacion.CalculaInteresOptimizado(CesionID, pago.FechaApli, true);
                        if (SaldoActual.Sofom)
                        {
                            itemCesionIngreso.Interes_Ordinario += (SaldoActual.InteresOrdinario - SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresOrdinario - SaldoAnterior.InteresOrdinarioM30D);
                            itemCesionIngreso.Interes_Moratorio += (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D - SaldoActual.InteresMoratorioM90D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D - SaldoAnterior.InteresMoratorioM90D);
                            itemCesionIngreso.Comision_Disposicion += (SaldoActual.ComisionDisposicion) - (SaldoAnterior.ComisionDisposicion);
                            itemCesionIngreso.Comision_Analisis += (SaldoActual.ComisionAnalisis) - (SaldoAnterior.ComisionAnalisis);
                            itemCesionIngreso.Bonificacion += pago.Bonificacion;
                        }
                        else
                        {
                            itemCesionIngreso.Interes_Ordinario = (SaldoActual.InteresOrdinario - SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresOrdinario - SaldoAnterior.InteresOrdinarioM30D);
                            if (SaldoActual.TipoContrato == 1)
                                itemCesionIngreso.Interes_Moratorio = (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D);
                            else
                                itemCesionIngreso.Interes_Moratorio = (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D - SaldoActual.InteresMoratorioM90D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D - SaldoAnterior.InteresMoratorioM90D);

                            itemCesionIngreso.Bonificacion += pago.Bonificacion;
                        }
                        SaldoAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(CesionID, pago.FechaApli, true);
                    }
                    SaldoActual = MobileBO.ControlOperacion.CalculaInteresOptimizado(CesionID, FechaFinalAux, true);

                    if (SaldoActual.Sofom)
                    {
                        itemCesionIngreso.Interes_Ordinario += (SaldoActual.InteresOrdinario - SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresOrdinario - SaldoAnterior.InteresOrdinarioM30D);
                        itemCesionIngreso.Interes_Moratorio += (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D - SaldoActual.InteresMoratorioM90D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D - SaldoAnterior.InteresMoratorioM90D);
                        itemCesionIngreso.Comision_Disposicion += (SaldoActual.ComisionDisposicion) - (SaldoAnterior.ComisionDisposicion);
                        itemCesionIngreso.Comision_Analisis += (SaldoActual.ComisionAnalisis) - (SaldoAnterior.ComisionAnalisis);
                    }
                    else
                    {
                        itemCesionIngreso.Interes_Ordinario = (SaldoActual.InteresOrdinario - SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresOrdinario - SaldoAnterior.InteresOrdinarioM30D);
                        if (SaldoActual.TipoContrato == 1)
                            itemCesionIngreso.Interes_Moratorio = (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D);
                        else
                            itemCesionIngreso.Interes_Moratorio = (SaldoActual.InteresMoratorio + SaldoActual.InteresOrdinarioM30D - SaldoActual.InteresMoratorioM90D) - (SaldoAnterior.InteresMoratorio + SaldoAnterior.InteresOrdinarioM30D - SaldoAnterior.InteresMoratorioM90D);
                    }
                    itemCesionIngreso.Sofom = SaldoActual.Sofom;
                    itemCesionIngreso.Fecha_Docu = DateTime.Parse(SaldoActual.FechaDocu);
                    itemCesionIngreso.Fecha_Vence = DateTime.Parse(SaldoActual.FechaVence);
                    itemCesionIngreso.TipoContrato = SaldoActual.TipoContrato.ToString();
                    itemCesionIngreso.Financiamiento = SaldoActual.CesionFinanciado;                    
                    itemCesionIngreso.DiasCesion = SaldoActual.Dias;
                    itemCesionIngreso.Pagada = (SaldoActual.Saldo <= 10 ? true : false);
                    if (demandado)
                    {
                        itemCesionIngreso.Demandado = true;
                        itemCesionIngreso.FechaDemanda = moroso.Fecha;
                        itemCesionIngreso.DemandadoEnElPeriodo = (moroso.Fecha < fechaInicial ? false : true);
                    }

                    if (itemCesionIngreso.Pagada && SaldoActual.Vencida == "SI" && itemCesionIngreso.DiasCesion < 31)
                        itemCesionIngreso.DiasCesion = 31;


                    //Sacamos la tasa de interes que le corresponde
                    DataSet dsTasa = new DataSet();
                    dsTasa = MobileBO.ControlAnalisis.TraerTasaFinanciamiento(SaldoActual.TasaFinanciamientoID);
                    if (dsTasa.Tables[0].Rows.Count > 0)
                    {
                        itemCesionIngreso.FactorInteresOrdinario = decimal.Parse(dsTasa.Tables[0].Rows[0]["FactorInteresOrdinario"].ToString());
                        itemCesionIngreso.FactorComisionDisp = decimal.Parse(dsTasa.Tables[0].Rows[0]["FactorComisionDisp"].ToString());
                        itemCesionIngreso.FactorComisionAnalisis = decimal.Parse(dsTasa.Tables[0].Rows[0]["FactorComisionAnalisis"].ToString());
                        itemCesionIngreso.FactorMoratorio = decimal.Parse(dsTasa.Tables[0].Rows[0]["FactorMoratorio"].ToString());
                    }
                    ColocadoIngresoFueraDelPeriodo.Add(itemCesionIngreso);
                }

                foreach (ModeloCesionesColocadasRG Cesion in ColocadoIngresoFueraDelPeriodo)
                {
                    ModeloCesionesColocadasRG CesionAux = Cesion;
                    CalculaTasaDeInteresReal(ref CesionAux, fechaFinal);
                    Cesion.Tasa = CesionAux.Tasa;
                }

                //Agregamos los intereses efectivamente cobrados durante el periodo
                List<ModeloCesionesColocadasRG> ColocadoInteresCobrado = new List<ModeloCesionesColocadasRG>();
                foreach (Entity.Operacion.Cesionesdetalle pago in PagosPeriodo)
                {
                    ModeloCesionesColocadasRG CesionColocada = DineroColocado.Find(x => x.CesionID.ToUpper().Equals(pago.Cesionid.ToUpper()));
                    ModeloCesionesColocadasRG PagoProcesado = ColocadoInteresCobrado.Find(x => x.CesionID.ToUpper().Equals(pago.Cesionid.ToUpper()));

                    //Quiere decir que encontro un pago correspondiente a las cesiones colocadas. Ahora solo falta procesar sus datos
                    if (CesionColocada != null)
                    {
                        //Preguntamos si la cesion se pago totalmente
                        if (CesionColocada.Pagada)
                        {
                            if (PagoProcesado == null)
                            {
                                ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                                itemCesionIngreso.CesionID = pago.Cesionid.ToUpper();
                                itemCesionIngreso.Fecha_Docu = pago.Fecha_Docu;
                                itemCesionIngreso.Fecha_Vence = pago.Fecha_Vence;
                                itemCesionIngreso.TipoContrato = pago.TipoContrato;
                                itemCesionIngreso.Financiamiento = CesionColocada.Financiamiento;
                                itemCesionIngreso.Interes_Ordinario = CesionColocada.Interes_Ordinario;
                                itemCesionIngreso.Comision_Disposicion = CesionColocada.Comision_Disposicion;
                                itemCesionIngreso.Comision_Analisis = CesionColocada.Comision_Analisis;
                                itemCesionIngreso.Interes_Moratorio = CesionColocada.Interes_Moratorio;
                                itemCesionIngreso.Bonificacion = CesionColocada.Bonificacion;
                                itemCesionIngreso.DiasCesion = CesionColocada.DiasCesion;
                                itemCesionIngreso.Pagada = true;
                                itemCesionIngreso.Tasa = CesionColocada.Tasa;
                                //itemCesionIngreso.Tasa = Math.Round(((((itemCesionIngreso.Interes_Ordinario + itemCesionIngreso.Interes_Moratorio + itemCesionIngreso.Comision_Analisis + itemCesionIngreso.Comision_Disposicion) - itemCesionIngreso.Bonificacion) / itemCesionIngreso.Financiamiento) / itemCesionIngreso.DiasCesion) * 30 * 100, 4);
                                ColocadoInteresCobrado.Add(itemCesionIngreso);
                            }
                            //Se la cesion ya existe en esta tabla quiere decir que el pago corresponde a un abono de una cesion que ya esta pagada asi que no es necesario
                            //Procesar este registro
                        }
                        else
                        {
                            if (CesionColocada.Sofom)
                            {
                                if (PagoProcesado == null)
                                {
                                    ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                                    itemCesionIngreso.CesionID = pago.Cesionid.ToUpper();
                                    itemCesionIngreso.Fecha_Docu = pago.Fecha_Docu;
                                    itemCesionIngreso.Fecha_Vence = pago.Fecha_Vence;
                                    itemCesionIngreso.TipoContrato = pago.TipoContrato;
                                    itemCesionIngreso.Financiamiento = pago.FinanciamientoCesion;
                                    itemCesionIngreso.Interes_Ordinario = pago.InteresOrdinario;
                                    itemCesionIngreso.Comision_Disposicion = pago.ComisionDisposicion;
                                    itemCesionIngreso.Comision_Analisis = pago.ComisionAnalisis;
                                    itemCesionIngreso.Interes_Moratorio = pago.InteresMoratorio;
                                    itemCesionIngreso.Bonificacion = pago.Bonificacion;
                                    itemCesionIngreso.DiasCesion = CesionColocada.DiasCesion;
                                    itemCesionIngreso.Pagada = false;
                                    itemCesionIngreso.Tasa = CesionColocada.Tasa;
                                    //itemCesionIngreso.Tasa = Math.Round(((((itemCesionIngreso.Interes_Ordinario + itemCesionIngreso.Interes_Moratorio + itemCesionIngreso.Comision_Analisis + itemCesionIngreso.Comision_Disposicion) - itemCesionIngreso.Bonificacion) / itemCesionIngreso.Financiamiento) / itemCesionIngreso.DiasCesion) * 30 * 100, 4);
                                    ColocadoInteresCobrado.Add(itemCesionIngreso);
                                }
                                else
                                {
                                    PagoProcesado.Interes_Ordinario += pago.InteresOrdinario;
                                    PagoProcesado.Comision_Disposicion += pago.ComisionDisposicion;
                                    PagoProcesado.Comision_Analisis += pago.ComisionAnalisis;
                                    PagoProcesado.Interes_Moratorio += pago.InteresMoratorio;
                                    PagoProcesado.Bonificacion += pago.Bonificacion;
                                }
                            }
                            else
                            {
                                //Aqui procesamos una vez el pago por que si hubo mas de un abono durante el perioro ya lo estamos tomando en cuenta en este codigo
                                if (PagoProcesado == null)
                                {
                                    ModeloSaldoCesion SaldoAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(pago.Cesionid, pago.Fecha_Docu, true);
                                    ModeloSaldoCesion SaldoNuevo = MobileBO.ControlOperacion.CalculaInteresOptimizado(pago.Cesionid, fechaFinal, true);

                                    ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                                    itemCesionIngreso.CesionID = pago.Cesionid.ToUpper();
                                    itemCesionIngreso.Fecha_Docu = pago.Fecha_Docu;
                                    itemCesionIngreso.Fecha_Vence = pago.Fecha_Vence;
                                    itemCesionIngreso.TipoContrato = pago.TipoContrato;
                                    itemCesionIngreso.Financiamiento = pago.FinanciamientoCesion;
                                    itemCesionIngreso.Interes_Ordinario = (SaldoNuevo.PagoInteresOrdinario - SaldoNuevo.PagoInteresMoratorio5Porciento) - (SaldoAnterior.PagoInteresOrdinario - SaldoAnterior.PagoInteresMoratorio5Porciento);
                                    itemCesionIngreso.Interes_Moratorio = (SaldoNuevo.PagoInteresMoratorio5Porciento + SaldoNuevo.PagoInteresMoratorio3Porciento) - (SaldoAnterior.PagoInteresMoratorio5Porciento + SaldoAnterior.PagoInteresMoratorio3Porciento);
                                    if (SaldoNuevo.Bonificaciones > 0)
                                        itemCesionIngreso.Bonificacion = (SaldoNuevo.Bonificaciones / 1.16m);
                                    itemCesionIngreso.DiasCesion = CesionColocada.DiasCesion;
                                    itemCesionIngreso.Pagada = false;
                                    itemCesionIngreso.Tasa = CesionColocada.Tasa;
                                    //itemCesionIngreso.Tasa = Math.Round(((((itemCesionIngreso.Interes_Ordinario + itemCesionIngreso.Interes_Moratorio + itemCesionIngreso.Comision_Analisis + itemCesionIngreso.Comision_Disposicion) - itemCesionIngreso.Bonificacion) / itemCesionIngreso.Financiamiento) / itemCesionIngreso.DiasCesion) * 30 * 100, 4);
                                    ColocadoInteresCobrado.Add(itemCesionIngreso);

                                }
                            }
                            //Aqui debemos procesar el pago si la cesion no esta pagada
                        }

                    }
                    else
                    {
                        ModeloSaldoCesion SaldoNuevo = MobileBO.ControlOperacion.CalculaInteresOptimizado(pago.Cesionid, fechaFinal, true);

                        //Aqui debemos procesar los pagos de las cesiones que corresponden a cesiones anteriores
                        if (pago.Sofom)
                        {
                            if (PagoProcesado == null)
                            {
                                ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                                itemCesionIngreso.CesionID = pago.Cesionid.ToUpper();
                                itemCesionIngreso.Fecha_Docu = pago.Fecha_Docu;
                                itemCesionIngreso.Fecha_Vence = pago.Fecha_Vence;
                                itemCesionIngreso.TipoContrato = pago.TipoContrato;
                                itemCesionIngreso.Financiamiento = pago.FinanciamientoCesion;
                                itemCesionIngreso.Interes_Ordinario = pago.InteresOrdinario;
                                itemCesionIngreso.Comision_Disposicion = pago.ComisionDisposicion;
                                itemCesionIngreso.Comision_Analisis = pago.ComisionAnalisis;
                                itemCesionIngreso.Interes_Moratorio = pago.InteresMoratorio;
                                itemCesionIngreso.Bonificacion = pago.Bonificacion;
                                itemCesionIngreso.DiasCesion = SaldoNuevo.Dias;
                                itemCesionIngreso.Pagada = (SaldoNuevo.SaldoFinanciado > 10 ? false : true);
                                itemCesionIngreso.Tasa = Math.Round(((((itemCesionIngreso.Interes_Ordinario + itemCesionIngreso.Interes_Moratorio + itemCesionIngreso.Comision_Analisis + itemCesionIngreso.Comision_Disposicion) - itemCesionIngreso.Bonificacion) / itemCesionIngreso.Financiamiento) / itemCesionIngreso.DiasCesion) * 30 * 100, 4);
                                ColocadoInteresCobrado.Add(itemCesionIngreso);
                            }
                            else
                            {
                                PagoProcesado.Interes_Ordinario += pago.InteresOrdinario;
                                PagoProcesado.Comision_Disposicion += pago.ComisionDisposicion;
                                PagoProcesado.Comision_Analisis += pago.ComisionAnalisis;
                                PagoProcesado.Interes_Moratorio += pago.InteresMoratorio;
                                PagoProcesado.Bonificacion += pago.Bonificacion;
                                PagoProcesado.Tasa = Math.Round(((((PagoProcesado.Interes_Ordinario + PagoProcesado.Interes_Moratorio + PagoProcesado.Comision_Analisis + PagoProcesado.Comision_Disposicion) - PagoProcesado.Bonificacion) / PagoProcesado.Financiamiento) / PagoProcesado.DiasCesion) * 30 * 100, 4);
                            }
                        }
                        else
                        {
                            //Aqui procesamos una vez el pago por que si hubo mas de un abono durante el perioro ya lo estamos tomando en cuenta en este codigo
                            if (PagoProcesado == null)
                            {
                                ModeloSaldoCesion SaldoAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(pago.Cesionid, fechaInicial.AddDays(-1), true);

                                ModeloCesionesColocadasRG itemCesionIngreso = new ModeloCesionesColocadasRG();
                                itemCesionIngreso.CesionID = pago.Cesionid.ToUpper();
                                itemCesionIngreso.Fecha_Docu = pago.Fecha_Docu;
                                itemCesionIngreso.Fecha_Vence = pago.Fecha_Vence;
                                itemCesionIngreso.TipoContrato = pago.TipoContrato;
                                itemCesionIngreso.Financiamiento = pago.FinanciamientoCesion;
                                itemCesionIngreso.Interes_Ordinario = (SaldoNuevo.PagoInteresOrdinario - SaldoNuevo.PagoInteresMoratorio5Porciento) - (SaldoAnterior.PagoInteresOrdinario - SaldoAnterior.PagoInteresMoratorio5Porciento);
                                itemCesionIngreso.Interes_Moratorio = (SaldoNuevo.PagoInteresMoratorio5Porciento + SaldoNuevo.PagoInteresMoratorio3Porciento) - (SaldoAnterior.PagoInteresMoratorio5Porciento + SaldoAnterior.PagoInteresMoratorio3Porciento);
                                itemCesionIngreso.Bonificacion = SaldoNuevo.Bonificaciones;
                                itemCesionIngreso.DiasCesion = SaldoNuevo.Dias;
                                itemCesionIngreso.Pagada = (SaldoNuevo.SaldoFinanciado > 10 ? false : true);
                                itemCesionIngreso.Tasa = Math.Round(((((itemCesionIngreso.Interes_Ordinario + itemCesionIngreso.Interes_Moratorio + itemCesionIngreso.Comision_Analisis + itemCesionIngreso.Comision_Disposicion) - itemCesionIngreso.Bonificacion) / itemCesionIngreso.Financiamiento) / itemCesionIngreso.DiasCesion) * 30 * 100, 4);
                                ColocadoInteresCobrado.Add(itemCesionIngreso);

                            }
                        }
                    }
                }

                //Actualizamos los dias del capital invertido anterior en base a lo cobrado
                decimal CarteraProblemaRecuperada = 0m;
                decimal ColocadoProblemaRecuperada = 0m;
                foreach (ModeloCesionesColocadasRG capitalInvertidoInicial in DineroCapitalInvertidoInicial)
                {
                    ModeloCesionesColocadasRG cobro = ColocadoInteresCobrado.Find(x => x.CesionID.ToUpper().Equals(capitalInvertidoInicial.CesionID.ToUpper()));
                    if (cobro != null) {
                        capitalInvertidoInicial.DiasCesion = cobro.DiasCesion;
                        //Aqui acumularemos el capital recuperado de la cartera vencida
                        if(cobro.DiasCesion > 90) {
                            List<Entity.Operacion.Cesionesdetalle> pagos = PagosPeriodo.FindAll(x => x.Cesionid.ToUpper().Equals(capitalInvertidoInicial.CesionID.ToUpper())).ToList();
                            CarteraProblemaRecuperada += pagos.Sum(x => x.Financiamiento);
                            capitalInvertidoInicial.DiasCesion = cobro.DiasCesion;
                        }
                    }                        
                    else
                        capitalInvertidoInicial.DiasCesion = (fechaFinal - capitalInvertidoInicial.Fecha_Docu).Days;
                }

                foreach (ModeloCesionesColocadasRG capitalColocado in DineroColocado)
                {
                    if (capitalColocado.DiasCesion > 90)
                    {
                        List<Entity.Operacion.Cesionesdetalle> pagos = PagosPeriodo.FindAll(x => x.Cesionid.ToUpper().Equals(capitalColocado.CesionID.ToUpper())).ToList();
                        ColocadoProblemaRecuperada += pagos.Sum(x => x.Financiamiento);
                    }
                }



                ////Actualizamos los dias del capital invertido anterior en base a lo cobrado
                ////Este codigo fue reemplazado por el codigo anterior
                //foreach (ModeloCesionesColocadasRG cobro in ColocadoInteresCobrado)
                //{
                //    ModeloCesionesColocadasRG capitalInvertidoInicial = DineroCapitalInvertidoInicial.Find(x => x.CesionID.ToUpper().Equals(cobro.CesionID.ToUpper()));
                //    if (capitalInvertidoInicial != null)
                //        capitalInvertidoInicial.DiasCesion = cobro.DiasCesion;
                //}


                //Agregamos el detalle del reporte para el dinero Colocado
                List<RG_DineroColocado> ResultadoDineroColocado = new List<RG_DineroColocado>();
                RG_DineroColocado itemDineroColocado = new RG_DineroColocado();
                decimal CapitalInvertido = DineroCapitalInvertido.Sum(x => x.Financiamiento);
                int DI = 31, DF = 0;
                //Insertamos el dinero colocado FACTORAJE CLIENTES
                itemDineroColocado = generarItemDC(meses, "FACTORAJE CLIENTES", "1-30 Días", "1", DineroColocado.FindAll(x => x.DiasCesion < DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion < DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion < DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion < DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion < DI), ColocadoInteresCobrado.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion < DI), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 30; DF = 61;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE CLIENTES", "31-60 Días", "1", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 60; DF = 91;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE CLIENTES", "61-90 Días", "1", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                //DI = 90;
                //itemDineroColocado = generarItemDC(meses, "FACTORAJE CLIENTES", "> 90 Días", "1", DineroColocado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.Pagada == true), CapitalInvertido);
                //ResultadoDineroColocado.Add(itemDineroColocado);


                //Insertamos el dinero colocado FACTORAJE PROVEEDORES
                DI = 31; DF = 0;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE PROVEEDORES", "1-30 Días", "2", DineroColocado.FindAll(x => x.DiasCesion < DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion < DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion < DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion < DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion < DI), ColocadoInteresCobrado.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion < DI), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 30; DF = 61;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE PROVEEDORES", "31-60 Días", "2", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 60; DF = 91;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE PROVEEDORES", "61-90 Días", "2", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                //DI = 90;
                //itemDineroColocado = generarItemDC(meses, "FACTORAJE PROVEEDORES", "> 90 Días", "2", DineroColocado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.Pagada == true), CapitalInvertido);
                //ResultadoDineroColocado.Add(itemDineroColocado);

                //Insertamos el dinero colocado FACTORAJE FC FINANCIERA
                DI = 31; DF = 0;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE FC FINANCIERA", "1-30 Días", "4", DineroColocado.FindAll(x => x.DiasCesion < DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion < DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion < DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion < DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion < DI), ColocadoInteresCobrado.FindAll(x => x.DiasCesion < DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion < DI), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 30; DF = 61;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE FC FINANCIERA", "31-60 Días", "4", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                DI = 60; DF = 91;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE FC FINANCIERA", "61-90 Días", "4", DineroColocado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.DiasCesion < DF), CapitalInvertido);
                ResultadoDineroColocado.Add(itemDineroColocado);

                //DI = 90;
                //itemDineroColocado = generarItemDC(meses, "FACTORAJE FC FINANCIERA", "> 90 Días", "4", DineroColocado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI && x.Pagada == true), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Pagada == true), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI && x.Pagada == true), CapitalInvertido);
                //ResultadoDineroColocado.Add(itemDineroColocado);

                //Agregamos la cartera vencida del dinero colocado
                List<RG_DineroColocado> ResultadoDineroColocadoCarteraVencida = new List<RG_DineroColocado>();
                DI = 90;
                itemDineroColocado = generarItemDC(meses, "FACTORAJE CLIENTES", "> 90 Días", "1", DineroColocado.FindAll(x => x.DiasCesion > DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado==false), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI), CapitalInvertido);
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = generarItemDemanda(meses, "FACTORAJE CLIENTES", "Demandados", "1", ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), new List<ModeloCesionesColocadasRG>());
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);


                itemDineroColocado = generarItemDC(meses, "FACTORAJE PROVEEDORES", "> 90 Días", "2", DineroColocado.FindAll(x => x.DiasCesion > DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == false), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI), CapitalInvertido);                
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = generarItemDemanda(meses, "FACTORAJE PROVEEDORES", "Demandados", "2", ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), new List<ModeloCesionesColocadasRG>());
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(meses, "FACTORAJE FC FINANCIERA", "> 90 Días", "4", DineroColocado.FindAll(x => x.DiasCesion > DI), DineroColocadoAcumulado.FindAll(x => x.DiasCesion > DI), DineroColocadoPeriodoAnt.FindAll(x => x.DiasCesion > DI), DineroColocadoMesAnterior.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertido.FindAll(x => x.DiasCesion > DI), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == false), ColocadoInteresCobrado.FindAll(x => x.DiasCesion > DI), DineroCapitalInvertidoInicial.FindAll(x => x.DiasCesion > DI), CapitalInvertido);
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = generarItemDemanda(meses, "FACTORAJE FC FINANCIERA", "Demandados", "4", ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == true), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), ColocadoIngresoFueraDelPeriodo.FindAll(x => x.DiasCesion > DI && x.Demandado == true && x.DemandadoEnElPeriodo == false), new List<ModeloCesionesColocadasRG>());
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                //Totalizamos 
                List<RG_DineroColocado> ResultadoDineroColocadoTotalizado = new List<RG_DineroColocado>();
                ResultadoDineroColocadoTotalizado.AddRange(ResultadoDineroColocado);
                ResultadoDineroColocadoTotalizado.AddRange(ResultadoDineroColocadoCarteraVencida);
                #endregion

                #region Presupuesto
                List<Entity.Operacion.Catpresupuestosoperacion> ListaPresupuestos = MobileBO.ControlOperacion.TraerCatpresupuestosoperacion(fechaInicial.Year, empresaid, null, null);
                ListaPresupuestos = ListaPresupuestos.FindAll(x => x.Mes >= fechaInicial.Month && x.Mes <= fechaFinal.Month);
                List<RG_DineroNuevo> ResultadoDineroNuevo = new List<RG_DineroNuevo>();
                RG_DineroNuevo itemDineroNuevo = new RG_DineroNuevo();
                itemDineroNuevo.Presupuesto = ListaPresupuestos.Sum(x => x.Colocacionaumentos + x.Colocacionclientenuevo);
                itemDineroNuevo.Nuevo = colocadoNuevo.Sum(x => x.Nuevo);
                itemDineroNuevo.Diferencia = itemDineroNuevo.Presupuesto - itemDineroNuevo.Nuevo;
                ResultadoDineroNuevo.Add(itemDineroNuevo);
                #endregion

                #region SaldoDiarioEnBancos
                //Sacamos el saldo en bancos de las cuentas contables
                DataSet dsSaldosCuentasBancarias = MobileBO.ControlContabilidad.TraerSaldosCuentasBancarias((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaFinal);
                List<ModeloSaldoBancarioDiario> ListaSaldoDiarioEnBancos = new List<ModeloSaldoBancarioDiario>();
                DateTime FechaAux = fechaInicioAño;

                while (FechaAux <= fechaFinal)
                {
                    DataSet dsConcentradoCuentasBancarias = MobileBO.ControlContabilidad.TraerSaldosCuentasBancarias((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), FechaAux);
                    ModeloSaldoBancarioDiario sdoDiario = new ModeloSaldoBancarioDiario();
                    sdoDiario.Fecha = FechaAux;
                    sdoDiario.Saldo =
                    (from a in dsConcentradoCuentasBancarias.Tables[0].AsEnumerable()
                     select new
                     {
                         Saldo = a.Field<decimal>("Saldo")
                     }).ToList().Sum(x => x.Saldo);
                    ListaSaldoDiarioEnBancos.Add(sdoDiario);
                    FechaAux = FechaAux.AddDays(1);
                }


                //Generamos la estructura del resultado del reporte
                List<RG_SaldoBancos> ResultadoSaldoDiarioBancos = new List<RG_SaldoBancos>();
                RG_SaldoBancos itemSaldoDiarioBancos = new RG_SaldoBancos();
                itemSaldoDiarioBancos.SaldoActual = (from a in dsSaldosCuentasBancarias.Tables[0].AsEnumerable()
                                                     select new
                                                     {
                                                         Saldo = a.Field<decimal>("Saldo")
                                                     }).ToList().Sum(x => x.Saldo);
                itemSaldoDiarioBancos.SaldoPromedio = (ListaSaldoDiarioEnBancos.Sum(x => x.Saldo) > 0m ? ListaSaldoDiarioEnBancos.Sum(x => x.Saldo) / ListaSaldoDiarioEnBancos.Count() : 0m);
                ResultadoSaldoDiarioBancos.Add(itemSaldoDiarioBancos);
                #endregion

                //Aqui van los intereses financieros

             

                //No te olvides de regresarlos a su lugar ya que los moviste.



                #region Gastos De Administracion y Gastos De Venta
                DataSet dsGastos = MobileBO.ControlContabilidad.TraerGatosDelReporteCostos(fechaInicial, fechaFinal, (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid));
                dsGastos.Tables[0].TableName = "GastosContables";
                #endregion

                #region Capital Invertido Del Periodo
                DataSet dsCapInvPer= MobileBO.ControlOperacion.TraerCapitalInvertidoPorPeriodo((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaInicial, fechaFinal);
                decimal CapInvPer = 0m;
                decimal CapInvProm = 0m;
                foreach (DataRow row in dsCapInvPer.Tables[0].Rows)
                {
                    CapInvPer += decimal.Parse(row["Factoraje"].ToString()) + decimal.Parse(row["Proveedores"].ToString()) + decimal.Parse(row["FCFinanciera"].ToString());
                }

                if (CapInvPer > 0 && (fechaFinal - fechaInicial).Days > 0)
                    CapInvProm = Math.Round(CapInvPer / (fechaFinal - fechaInicial).Days);
                #endregion




                DataSet ds = new DataSet();
                DataTable param = new DataTable();
                param.Columns.Add("FechaInicial", typeof(DateTime));
                param.Columns.Add("FechaFinal", typeof(DateTime));
                param.Columns.Add("Presupuesto", typeof(decimal));
                param.Columns.Add("meses", typeof(long));
                param.Columns.Add("CapitalInvertido", typeof(decimal));
                param.Columns.Add("RecuperacionCarteraVencida", typeof(decimal));
                param.Columns.Add("RecumeracionCarteraVencidaColocada", typeof(decimal));

                param.Columns.Add("CapInvPromedioPeriodo", typeof(decimal));
                param.Columns.Add("DineroColocadoPromedioPeriodo", typeof(decimal));

                param.TableName = "Parametros";
                DataRow rparam = param.NewRow();
                rparam["FechaInicial"] = fechaInicial;
                rparam["FechaFinal"] = fechaFinal;
                rparam["Presupuesto"] = ListaPresupuestos.Sum(x => x.Colocacionoperativa);
                rparam["meses"] = meses;
                rparam["CapitalInvertido"] = CapitalInvertido;

                rparam["RecuperacionCarteraVencida"] = CarteraProblemaRecuperada;
                rparam["RecumeracionCarteraVencidaColocada"] = ColocadoProblemaRecuperada;

                rparam["CapInvPromedioPeriodo"] = CapInvProm;
                rparam["DineroColocadoPromedioPeriodo"] = DineroColocado.Sum(x => x.Financiamiento / (fechaFinal - fechaInicial).Days);


                param.Rows.Add(rparam);
                ds.Tables.Add(param);

                DataTable RG_DineroColocadoCV = ResultadoDineroColocadoCarteraVencida.ToDataTable().Copy();
                RG_DineroColocadoCV.TableName = "RG_DineroColocadoCV";

                DataTable RG_DineroColocadoTotal = ResultadoDineroColocadoTotalizado.ToDataTable().Copy();
                RG_DineroColocadoTotal.TableName = "RG_DineroColocadoTotal";

                //Tablas que se mandan al reporte final
                ds.Tables.Add(ResultadoDineroColocado.ToDataTable());
                ds.Tables.Add(RG_DineroColocadoCV);
                ds.Tables.Add(ResultadoDineroNuevo.ToDataTable());
                ds.Tables.Add(ResultadoSaldoDiarioBancos.ToDataTable());
                ds.Tables.Add(ResultadoInteresFinanciero.ToDataTable());
                ds.Tables.Add(colocadoNuevo.ToDataTable());
                ds.Tables.Add(dsGastos.Tables[0].Copy());
                ds.Tables.Add(MobileBO.ControlOperacion.TraerComportamientoCapitalInvertido((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), fechaFinal).Tables[0].Copy());
                ds.Tables.Add(RG_DineroColocadoTotal);

                

                base.NombreReporte = "ReporteGerencial";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteGerencial.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public RG_DineroColocado generarItemDC(long meses, string producto, string concepto, string tipocontrato, List<ModeloCesionesColocadasRG> DineroColocado, List<ModeloCesionesColocadasRG> DineroColocadoAcumulado, List<ModeloCesionesColocadasRG> DineroColocadoPeriodoAnt, List<ModeloCesionesColocadasRG> DineroColocadoMesAnterior, List<ModeloCesionesColocadasRG> DineroColocadoCapitalInvertido, List<ModeloCesionesColocadasRG> DineroColocadoIngresoFueraDelPeriodo, List<ModeloCesionesColocadasRG> ColocadoInteresCobrado, List<ModeloCesionesColocadasRG> DineroColocadoCapitalInvertidoInicial, decimal capitalinvertido)
        {

            RG_DineroColocado itemDineroColocado = new RG_DineroColocado();

            //Agreamos la colocacion del dinero
            itemDineroColocado.Producto = producto;
            itemDineroColocado.Concepto = concepto;

            //Distribuimos el dinero colocado
            itemDineroColocado.PrdoActual = DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            itemDineroColocado.PrdoAnterior = DineroColocadoPeriodoAnt.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            //Si el reporte es mensual el sistema calcula el mes anterior
            if (meses == 0)
            {
                itemDineroColocado.PrdoAnteriorMes = DineroColocadoMesAnterior.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            }
            itemDineroColocado.Diferencia = itemDineroColocado.PrdoActual - itemDineroColocado.PrdoAnterior;
            itemDineroColocado.Acumulado = DineroColocadoAcumulado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);

            //Asignamos el capital invertido
            itemDineroColocado.CapitalInvertidoInicial = DineroColocadoCapitalInvertidoInicial.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            itemDineroColocado.CapitalInvertido = DineroColocadoCapitalInvertido.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            itemDineroColocado.CapitalInvertidoPorc = (itemDineroColocado.CapitalInvertido / capitalinvertido) * 100;

            //Acumulamos el total de los intereses generados de las cesiones colacadas y las cesiones que estaban en el capital invertido inicial
            itemDineroColocado.InteresOrdinario = DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Ordinario + x.Comision_Disposicion + x.Comision_Analisis);
            itemDineroColocado.InteresOrdinario += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Ordinario + x.Comision_Disposicion + x.Comision_Analisis);

            itemDineroColocado.InteresMoratorio = DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Moratorio);
            itemDineroColocado.InteresMoratorio += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Moratorio);

            itemDineroColocado.Bonificaciones = DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Bonificacion);
            itemDineroColocado.Bonificaciones += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Bonificacion);

            itemDineroColocado.Neto = itemDineroColocado.InteresOrdinario + itemDineroColocado.InteresMoratorio - itemDineroColocado.Bonificaciones;

            decimal TasaAcum = 0m;
            decimal ItemTasaAcum = 0m;

            decimal TasaAcumOrd = 0m;
            decimal ItemTasaAcumOrd = 0m;

            decimal TasaAcumMor = 0m;
            decimal ItemTasaAcumMor = 0m;


            if (DineroColocado.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count() > 0)
            {
                TasaAcum += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Tasa);
                ItemTasaAcum += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count();

                TasaAcumOrd += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresOrdinaria);
                ItemTasaAcumOrd += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato && x.DiasOrdinarios > 0).Count();

                TasaAcumMor += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresMoratoria);
                ItemTasaAcumMor += DineroColocado.FindAll(x => x.TipoContrato == tipocontrato && x.DiasMoratorios > 0).Count();

            }
            if (DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count() > 0)
            {
                TasaAcum += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Tasa);
                ItemTasaAcum += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count();

                TasaAcumOrd += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresOrdinaria);
                ItemTasaAcumOrd += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasOrdinarios > 0).Count();

                TasaAcumMor += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresMoratoria);
                ItemTasaAcumMor += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasMoratorios > 0).Count();
            }

            if (TasaAcum > 0 && ItemTasaAcum > 0)
                itemDineroColocado.Tasa = TasaAcum / ItemTasaAcum;

            if (TasaAcumOrd > 0 && ItemTasaAcumOrd > 0)
                itemDineroColocado.TasaInteresOrdinario = TasaAcumOrd / ItemTasaAcumOrd;

            if (TasaAcumMor > 0 && ItemTasaAcumMor > 0)
                itemDineroColocado.TasaInteresMoratorio = TasaAcumMor / ItemTasaAcumMor;

            //Aqui ingresamos el total del dinero efectivamente cobrado durante el periodo
            itemDineroColocado.PagoIngresoIntOrd = ColocadoInteresCobrado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Ordinario + x.Comision_Disposicion + x.Comision_Analisis);
            itemDineroColocado.PagoIngresoIntMor = ColocadoInteresCobrado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Moratorio - x.Bonificacion);
            return itemDineroColocado;
        }

        //Genera item de un cliente demandado

        public RG_DineroColocado generarItemDemanda(long meses, string producto, string concepto, string tipocontrato, List<ModeloCesionesColocadasRG> DineroColocadoCapitalInvertido,List<ModeloCesionesColocadasRG> DineroColocadoCapitalInvertidoInicial, List<ModeloCesionesColocadasRG> DineroColocadoIngresoFueraDelPeriodo, List<ModeloCesionesColocadasRG> ColocadoInteresCobrado)
        {

            RG_DineroColocado itemDineroColocado = new RG_DineroColocado();

            //Agreamos la colocacion del dinero
            itemDineroColocado.Producto = producto;
            itemDineroColocado.Concepto = concepto;

            
            

            //Asignamos el capital invertido
            itemDineroColocado.CapitalInvertidoInicial = DineroColocadoCapitalInvertidoInicial.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);
            itemDineroColocado.CapitalInvertido = itemDineroColocado.CapitalInvertidoInicial + DineroColocadoCapitalInvertido.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Financiamiento);


            //Acumulamos el total de los intereses generados de las cesiones colacadas y las cesiones que estaban en el capital invertido inicial            
            itemDineroColocado.InteresOrdinario += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Ordinario + x.Comision_Disposicion + x.Comision_Analisis);
            itemDineroColocado.InteresMoratorio += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Moratorio);            
            itemDineroColocado.Bonificaciones += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Bonificacion);
            itemDineroColocado.Neto = itemDineroColocado.InteresOrdinario + itemDineroColocado.InteresMoratorio - itemDineroColocado.Bonificaciones;

            decimal TasaAcum = 0m;
            decimal ItemTasaAcum = 0m;

            decimal TasaAcumOrd = 0m;
            decimal ItemTasaAcumOrd = 0m;

            decimal TasaAcumMor = 0m;
            decimal ItemTasaAcumMor = 0m;

            if (DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count() > 0)
            {
                TasaAcum += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Tasa);
                ItemTasaAcum += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasCesion > 0).Count();

                TasaAcumOrd += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresOrdinaria);
                ItemTasaAcumOrd += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasOrdinarios > 0).Count();

                TasaAcumMor += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.TasaInteresMoratoria);
                ItemTasaAcumMor += DineroColocadoIngresoFueraDelPeriodo.FindAll(x => x.TipoContrato == tipocontrato && x.DiasMoratorios > 0).Count();
            }

            if (TasaAcum > 0 && ItemTasaAcum > 0)
                itemDineroColocado.Tasa = TasaAcum / ItemTasaAcum;

            if (TasaAcumOrd > 0 && ItemTasaAcumOrd > 0)
                itemDineroColocado.TasaInteresOrdinario = TasaAcumOrd / ItemTasaAcumOrd;

            if (TasaAcumMor > 0 && ItemTasaAcumMor > 0)
                itemDineroColocado.TasaInteresMoratorio = TasaAcumMor / ItemTasaAcumMor;

            //Aqui ingresamos el total del dinero efectivamente cobrado durante el periodo
            itemDineroColocado.PagoIngresoIntOrd = ColocadoInteresCobrado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Ordinario + x.Comision_Disposicion + x.Comision_Analisis);
            itemDineroColocado.PagoIngresoIntMor = ColocadoInteresCobrado.FindAll(x => x.TipoContrato == tipocontrato).Sum(x => x.Interes_Moratorio - x.Bonificacion);
            return itemDineroColocado;
        }


        /*
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {   
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));
            string empresaid = parametros.Get("empresaid");         
            try
            {
                DateTime fechaInicioPdoAnt = fechaInicial.AddYears(-1);
                DateTime fechaFinalPdoAnt = fechaFinal.AddYears(-1);
                DateTime fechaInicioAño = new DateTime(fechaInicial.Year, 1, 1);

                long meses = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Month, fechaInicial, fechaFinal);

                #region Traer Dinero Nuevo Colocado 
                //Tiempo 1:32 en un año
                //Traemos el dinero colocado hasta la fecha de fin del reporte
                DataSet DineroColocado = MobileBO.ControlOperacion.TraerCesionesColocadasPorFecha((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null, fechaFinal);
                var Clientes = (from a in DineroColocado.Tables[0].AsEnumerable()
                                where a.Field<DateTime>("Fecha_Docu") >= fechaInicial
                                select new
                                {
                                    ClienteID = a.Field<Guid>("ClienteID").ToString().ToUpper()
                                }).Distinct();
                List<ModeloCesionesColocadasRG> ListaDineroColocado = (from a in DineroColocado.Tables[0].AsEnumerable()
                                                                       select new ModeloCesionesColocadasRG
                                                                       {
                                                                           ClienteID = a.Field<Guid>("ClienteID").ToString().ToUpper(),
                                                                           CesionID = a.Field<Guid>("CesionID").ToString().ToUpper(),
                                                                           Fecha_Docu = a.Field<DateTime>("Fecha_Docu"),
                                                                           Fecha_Vence = a.Field<DateTime>("Fecha_Vence"),
                                                                           TipoContrato = a.Field<string>("TipoContrato"),
                                                                           Colocado = a.Field<decimal>("Financiamiento"),                                                                           
                                                                           DiasCesion = (a.Field<int>("Estatus") == 1 ? (fechaFinal - a.Field<DateTime>("Fecha_Docu")).Days : (a.Field<DateTime>("FechaPago")>fechaFinal ? (fechaFinal - a.Field<DateTime>("Fecha_Docu")).Days: (a.Field<DateTime>("FechaPago") - a.Field<DateTime>("Fecha_Docu")).Days))
                                                                           
                                                                       }).ToList();
                List<Entity.ModeloDineroNuevoColocado> colocadoNuevo = new List<Entity.ModeloDineroNuevoColocado>();
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "1", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "2", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "3", Nuevo = 0m });
                colocadoNuevo.Add(new Entity.ModeloDineroNuevoColocado() { TipoContrato = "4", Nuevo = 0m });
                
                foreach (var cliente in Clientes)
                {
                    List<SaldoDiarioPorCliente> SaldoDiario = MobileBO.ControlOperacion.TraerSaldoDiarioPorCliente(cliente.ClienteID, fechaFinal);
                    foreach (ModeloCesionesColocadasRG operacion in ListaDineroColocado.FindAll(x => x.ClienteID == cliente.ClienteID && x.Fecha_Docu>=fechaInicial))
                    {
                        decimal ColocadoHistoria = 0m;
                        if (SaldoDiario.FindAll(a => a.ClienteID == cliente.ClienteID && a.Fecha < operacion.Fecha_Docu).Count() > 0)
                            ColocadoHistoria = SaldoDiario.FindAll(a => a.ClienteID == cliente.ClienteID && a.Fecha < operacion.Fecha_Docu).Max(b => b.Saldo);
                        if (operacion.Colocado > ColocadoHistoria)
                            colocadoNuevo.Find(c => c.TipoContrato == operacion.TipoContrato).Nuevo += operacion.Colocado - ColocadoHistoria;
                    }
                }
                

                //Lista de presupuestos
                

                #endregion
                #region TraerLosIngresos
                List<Entity.Operacion.Cesionesdetalle> Pagos = MobileBO.ControlOperacion.TraerPagosPorPeriodo((empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid), null, 11, fechaInicial, fechaFinal);

                //Debemos verificar que el pago realizado sea de una cesion que se haya hecho durante la fecha de consulta
                Pagos = Pagos.FindAll(x => x.Fecha_Docu >= fechaInicial);

                Pagos = (from a in Pagos
                         group a by new
                         {
                             Cesionid = a.Cesionid,
                             ClienteID = a.ClienteID,
                             EmpresaID = a.EmpresaID,
                             TipoContrato = a.TipoContrato,
                             Demandado = a.Demandado,
                             Fecha_Docu = a.Fecha_Docu,
                             Fecha_Vence = a.Fecha_Vence,
                             FinanciamientoCesion = a.FinanciamientoCesion,
                             Sofom = a.Sofom
                         } into grupo
                         select new Entity.Operacion.Cesionesdetalle
                         {
                             Cesionid = grupo.Key.Cesionid,
                             ClienteID = grupo.Key.ClienteID,
                             EmpresaID = grupo.Key.EmpresaID,
                             TipoContrato = grupo.Key.TipoContrato,
                             Demandado = grupo.Key.Demandado,
                             Fecha_Docu = grupo.Key.Fecha_Docu,
                             Fecha_Vence = grupo.Key.Fecha_Vence,
                             FinanciamientoCesion = grupo.Key.FinanciamientoCesion,
                             Sofom = grupo.Key.Sofom,
                             Financiamiento = grupo.Sum(x => x.Financiamiento),
                             FechaApli = grupo.Max(x => x.FechaApli),
                             InteresOrdinario = grupo.Sum(x => x.InteresOrdinario),
                             ComisionAnalisis = grupo.Sum(x => x.ComisionAnalisis),
                             ComisionDisposicion = grupo.Sum(x => x.ComisionDisposicion),
                             InteresMoratorio = grupo.Sum(x => x.InteresMoratorio),
                             Bonificacion = grupo.Sum(x => x.Bonificacion),
                             Comision = grupo.Sum(x => x.Comision),
                             DiasCesion = (grupo.Max(x => x.FechaApli) - grupo.Key.Fecha_Docu).Days
                         }).ToList();

                //Generamos la tasa generada por cada cesion cobrada
                List< ModeloTasasPorCesion > TasasPorCesion = new List<ModeloTasasPorCesion>();
                List<ModeloTasasPorCesion> TasasPorCesionVencida = new List<ModeloTasasPorCesion>();
                foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
                {
                    int diasOperacion = (pago.FechaApli - pago.Fecha_Docu).Days;
                    decimal InteresGenerado = (pago.Sofom ? pago.InteresOrdinario + pago.ComisionAnalisis + pago.ComisionDisposicion + pago.InteresMoratorio - pago.Bonificacion : pago.Comision + pago.InteresMoratorio - pago.Bonificacion);
                    decimal Financiamiento = pago.FinanciamientoCesion;
                    decimal FactorTasa = InteresGenerado / Financiamiento;
                    decimal TasaGenerada = Math.Round((FactorTasa / diasOperacion) * 30 * 100, 4);
                    TasasPorCesion.Add(new ModeloTasasPorCesion() { CesionID = pago.Cesionid, Tasa = TasaGenerada, TipoContrato = pago.TipoContrato, DiasCesion = diasOperacion });
                }

                //Sacamos las comisiones generadas de las cesiones que estan pendientes de pago a la fecha actual.
                DataSet CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_ReporteGerencial(fechaFinal, (empresaid == string.Empty || empresaid == Guid.Empty.ToString() ? null : empresaid));

                //Intereses por cobrar menor a 30 dias
                List<ModeloGeneradoColocado> ListaInteresesGenerados30 = new List<ModeloGeneradoColocado>();
                InteresesGeneradosView(ref ListaInteresesGenerados30);
                //Intereses por cobrar 30 a 60 dias
                List<ModeloGeneradoColocado> ListaInteresesGenerados3060 = new List<ModeloGeneradoColocado>();
                InteresesGeneradosView(ref ListaInteresesGenerados3060);
                //Intereses por cobrar 6090
                List<ModeloGeneradoColocado> ListaInteresesGenerados6090 = new List<ModeloGeneradoColocado>();
                InteresesGeneradosView(ref ListaInteresesGenerados6090);

                //Sacamos los intereses generados de los clientes problema
                List<ModeloGeneradoColocado> ListaInteresesGeneradosCarteraVencida = new List<ModeloGeneradoColocado>();
                InteresesGeneradosView(ref ListaInteresesGeneradosCarteraVencida);

                var CesionesPorCobrarPeriodo = from a in CesionesPorCobrar.Tables[0].AsEnumerable()
                                               join b in ListaDineroColocado on a.Field<Guid>("CesionID").ToString().ToUpper() equals b.CesionID
                                               where b.Fecha_Docu >= fechaInicial
                                               select new
                                               {
                                                   CesionID = b.CesionID,
                                                   TipoContrato = a.Field<string>("TipoContrato")
                                               };
                foreach (var Cesion in CesionesPorCobrarPeriodo)
                {
                    ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, false);
                    int diasOperacion = (fechaFinal - DateTime.Parse(Saldo.FechaDocu)).Days;
                    decimal TasaGenerada = 0m;
                    //Calculamos el interes generado para la cesion el que se cobro en realidad                    
                    if (diasOperacion > 0)
                    {
                        decimal InteresGenerado = Saldo.InteresOrdinario + Saldo.ComisionAnalisis + Saldo.ComisionDisposicion + Saldo.InteresMoratorio;
                        decimal Financiamiento = Saldo.CesionFinanciado;
                        decimal FactorTasa = InteresGenerado / Financiamiento;
                        TasaGenerada = Math.Round((FactorTasa / diasOperacion) * 30 * 100, 4);
                        if (diasOperacion <= 91)
                        {
                            //Aqui sacamos los intereses generados por cada una de las cesiones y los acomodamos segun los dias de operacion
                            if (diasOperacion < 31)
                            {
                                ListaInteresesGenerados30.Find(a => a.TipoContrato == Saldo.TipoContrato).Interes += Saldo.InteresOrdinario + Saldo.ComisionAnalisis + Saldo.ComisionDisposicion;
                                ListaInteresesGenerados30.Find(a => a.TipoContrato == Saldo.TipoContrato).Moratorio += Saldo.InteresMoratorio;
                            }
                            if (diasOperacion > 30 && diasOperacion < 61)
                            {
                                ListaInteresesGenerados3060.Find(a => a.TipoContrato == Saldo.TipoContrato).Interes += Saldo.InteresOrdinario + Saldo.ComisionAnalisis + Saldo.ComisionDisposicion;
                                ListaInteresesGenerados3060.Find(a => a.TipoContrato == Saldo.TipoContrato).Moratorio += Saldo.InteresMoratorio;
                            }
                            if (diasOperacion > 60 && diasOperacion < 91)
                            {
                                ListaInteresesGenerados6090.Find(a => a.TipoContrato == Saldo.TipoContrato).Interes += Saldo.InteresOrdinario + Saldo.ComisionAnalisis + Saldo.ComisionDisposicion;
                                ListaInteresesGenerados6090.Find(a => a.TipoContrato == Saldo.TipoContrato).Moratorio += Saldo.InteresMoratorio;
                            }
                            TasasPorCesion.Add(new ModeloTasasPorCesion() { CesionID = Saldo.CesionID, Tasa = TasaGenerada, TipoContrato = Cesion.TipoContrato, DiasCesion = diasOperacion });
                        }
                        else
                        {
                            ListaInteresesGeneradosCarteraVencida.Find(a => a.TipoContrato == Saldo.TipoContrato).Interes += Saldo.InteresOrdinario + Saldo.ComisionAnalisis + Saldo.ComisionDisposicion;
                            ListaInteresesGeneradosCarteraVencida.Find(a => a.TipoContrato == Saldo.TipoContrato).Moratorio += Saldo.InteresMoratorio;
                            TasasPorCesionVencida.Add(new ModeloTasasPorCesion() { CesionID = Saldo.CesionID.ToUpper(), Tasa = TasaGenerada, TipoContrato = Cesion.TipoContrato, DiasCesion = diasOperacion });
                        }

                    }                   
                }

                List<ModeloTasasPorCesion> TasasPorCesionMenor5 = new List<ModeloTasasPorCesion>();
                TasasPorCesionMenor5 = TasasPorCesion.FindAll(x => x.Tasa < 5m);
                TasasPorCesion = TasasPorCesion.FindAll(x => x.Tasa >= 5m);
                TasasPorCesionMenor5 = (from a in TasasPorCesionMenor5
                                        group a by new
                                        {
                                            CesionID = a.CesionID,
                                            TipoContrato = a.TipoContrato,
                                            DiasCesion = a.DiasCesion
                                        } into grupo
                                        select new ModeloTasasPorCesion
                                        {
                                            CesionID = grupo.Key.CesionID,
                                            TipoContrato = grupo.Key.TipoContrato,
                                            DiasCesion = grupo.Key.DiasCesion,
                                            Tasa = grupo.Sum(X => X.Tasa)
                                        }).ToList();
              TasasPorCesion.AddRange(TasasPorCesionMenor5);
                #endregion
               

                #region Estructura DINERO COLOCADO E INGRESOS POR INTERES
                //Llenamos el apartado del dinero colocado ponemos 3 rubros Clientes, Proveedores y FC Financiera
                List<ModeloCesionesColocadasRG> ListaDineroColocadoEst = (from a in ListaDineroColocado
                                                                          join b in TasasPorCesionVencida on a.CesionID equals b.CesionID into c
                                                                          from d in c.DefaultIfEmpty()
                                                                          where d == null
                                                                          select a).ToList();
                List < RG_DineroColocado > ResultadoDineroColocado = new List<RG_DineroColocado>();

                RG_DineroColocado itemDineroColocado = new RG_DineroColocado();

                //Insertamos el dinero colocado FACTORAJE CLIENTES
                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion < 31), meses, "FACTORAJE CLIENTES", "1-30 Días", "1", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados30, Pagos.FindAll(x => x.DiasCesion < 31), TasasPorCesion.FindAll(x => x.DiasCesion < 31), "1");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 30 && x.DiasCesion<61), meses, "FACTORAJE CLIENTES", "31-60 Días", "1", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados3060, Pagos.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), TasasPorCesion.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), "1");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), meses, "FACTORAJE CLIENTES", "61-90 Días", "1", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados6090, Pagos.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), TasasPorCesion.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), "1");
                ResultadoDineroColocado.Add(itemDineroColocado);

                //Insertamos el dinero colocado FACTORAJE PROVEEDORES
                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion < 31), meses, "FACTORAJE PROVEEDORES", "1-30 Días", "2", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados30, Pagos.FindAll(x => x.DiasCesion < 31), TasasPorCesion.FindAll(x => x.DiasCesion < 31), "2");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), meses, "FACTORAJE PROVEEDORES", "31-60 Días", "2", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados3060, Pagos.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), TasasPorCesion.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), "2");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), meses, "FACTORAJE PROVEEDORES", "61-90 Días", "2", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados6090, Pagos.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), TasasPorCesion.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), "2");
                ResultadoDineroColocado.Add(itemDineroColocado);

                //Insertamos el dinero colocado FACTORAJE FC FINANCIERA
                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion < 31), meses, "FACTORAJE FC FINANCIERA", "1-30 Días", "4", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados30, Pagos.FindAll(x => x.DiasCesion < 31), TasasPorCesion.FindAll(x => x.DiasCesion < 31), "4");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), meses, "FACTORAJE FC FINANCIERA", "31-60 Días", "4", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados3060, Pagos.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), TasasPorCesion.FindAll(x => x.DiasCesion > 30 && x.DiasCesion < 61), "4");
                ResultadoDineroColocado.Add(itemDineroColocado);

                itemDineroColocado = generarItemDC(ListaDineroColocadoEst.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), meses, "FACTORAJE FC FINANCIERA", "61-90 Días", "4", fechaInicial, fechaInicioPdoAnt, fechaFinalPdoAnt, fechaInicioAño);
                generarIngresos(ref itemDineroColocado, ListaInteresesGenerados6090, Pagos.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), TasasPorCesion.FindAll(x => x.DiasCesion > 60 && x.DiasCesion < 91), "4");
                ResultadoDineroColocado.Add(itemDineroColocado);


                #endregion
                #region Estructura DINERO COLOCADO CARTERA VENCIDA
                //Llenamos el apartado del dinero colocado ponemos 3 rubros Clientes, Proveedores y FC Financiera
                List<ModeloCesionesColocadasRG> ListaDineroColocadoCarteraVencida = (from a in ListaDineroColocado
                                                                                     join b in TasasPorCesionVencida on a.CesionID equals b.CesionID into c
                                                                                     from d in c.DefaultIfEmpty()
                                                                                     where d != null
                                                                                     select a).ToList();

                List<RG_DineroColocado> ResultadoDineroColocadoCarteraVencida = new List<RG_DineroColocado>();
                
                itemDineroColocado = new RG_DineroColocado();
                itemDineroColocado.Concepto = "Factoraje Clientes";
                itemDineroColocado.PrdoActual = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "1" && x.Fecha_Docu >= fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.PrdoAnterior = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "1" && x.Fecha_Docu < fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.Diferencia = itemDineroColocado.PrdoActual - itemDineroColocado.PrdoAnterior;
                itemDineroColocado.Acumulado = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "1" && x.Fecha_Docu >= fechaInicioAño).Sum(x => x.Colocado);
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = new RG_DineroColocado();
                itemDineroColocado.Concepto = "Factoraje Proveedores";
                itemDineroColocado.PrdoActual = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "2" && x.Fecha_Docu >= fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.PrdoAnterior = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "2" && x.Fecha_Docu < fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.Diferencia = itemDineroColocado.PrdoActual - itemDineroColocado.PrdoAnterior;
                itemDineroColocado.Acumulado = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "2" && x.Fecha_Docu >= fechaInicioAño).Sum(x => x.Colocado);
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                itemDineroColocado = new RG_DineroColocado();
                itemDineroColocado.Concepto = "Factoraje FC Financiera";
                itemDineroColocado.PrdoActual = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "4" && x.Fecha_Docu >= fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.PrdoAnterior = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "4" && x.Fecha_Docu < fechaInicial).Sum(x => x.Colocado);
                itemDineroColocado.Diferencia = itemDineroColocado.PrdoActual - itemDineroColocado.PrdoAnterior;
                itemDineroColocado.Acumulado = ListaDineroColocadoCarteraVencida.FindAll(x => x.TipoContrato == "4" && x.Fecha_Docu >= fechaInicioAño).Sum(x => x.Colocado);
                ResultadoDineroColocadoCarteraVencida.Add(itemDineroColocado);

                #endregion

                

                #region Estructura Ingresos por Interes Cartera Vencida
                List<RG_IngresosPorInteres> ResultadoIngresosPorInteresCarteraVencida = new List<RG_IngresosPorInteres>();
                RG_IngresosPorInteres itemIngresoInteres = new RG_IngresosPorInteres();
                itemIngresoInteres.Concepto = "Factoraje Clientes";                
                itemIngresoInteres.InteresOrdinario += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 1).Sum(x => x.Interes);                
                itemIngresoInteres.InteresMoratorio += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 1).Sum(x => x.Moratorio);                
                itemIngresoInteres.Neto = itemIngresoInteres.InteresOrdinario + itemIngresoInteres.InteresMoratorio - itemIngresoInteres.Bonificaciones;
                itemIngresoInteres.Tasa = (TasasPorCesionVencida.FindAll(x => x.TipoContrato == "1").Count() > 0 ? TasasPorCesionVencida.FindAll(x => x.TipoContrato == "1").Sum(x => x.Tasa) / TasasPorCesionVencida.FindAll(x => x.TipoContrato == "1").Count() : 0m);
                ResultadoIngresosPorInteresCarteraVencida.Add(itemIngresoInteres);

                itemIngresoInteres = new RG_IngresosPorInteres();
                itemIngresoInteres.Concepto = "Factoraje Proveedores";
                itemIngresoInteres.InteresOrdinario += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 2).Sum(x => x.Interes);
                itemIngresoInteres.InteresMoratorio += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 2).Sum(x => x.Moratorio);
                itemIngresoInteres.Neto = itemIngresoInteres.InteresOrdinario + itemIngresoInteres.InteresMoratorio - itemIngresoInteres.Bonificaciones;
                itemIngresoInteres.Tasa = (TasasPorCesionVencida.FindAll(x => x.TipoContrato == "2").Count() > 0 ? TasasPorCesionVencida.FindAll(x => x.TipoContrato == "2").Sum(x => x.Tasa) / TasasPorCesionVencida.FindAll(x => x.TipoContrato == "2").Count() : 0m);
                ResultadoIngresosPorInteresCarteraVencida.Add(itemIngresoInteres);

                itemIngresoInteres = new RG_IngresosPorInteres();
                itemIngresoInteres.Concepto = "Factoraje FC Financiera";
                itemIngresoInteres.InteresOrdinario += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 4).Sum(x => x.Interes);
                itemIngresoInteres.InteresMoratorio += ListaInteresesGeneradosCarteraVencida.FindAll(a => a.TipoContrato == 4).Sum(x => x.Moratorio);
                itemIngresoInteres.Neto = itemIngresoInteres.InteresOrdinario + itemIngresoInteres.InteresMoratorio - itemIngresoInteres.Bonificaciones;
                itemIngresoInteres.Tasa = (TasasPorCesionVencida.FindAll(x => x.TipoContrato == "4").Count() > 0 ? TasasPorCesionVencida.FindAll(x => x.TipoContrato == "4").Sum(x => x.Tasa) / TasasPorCesionVencida.FindAll(x => x.TipoContrato == "4").Count() : 0m);
                ResultadoIngresosPorInteresCarteraVencida.Add(itemIngresoInteres);


                #endregion
                

               

            }
            catch (Exception)
            {
                throw;
            }
        }
        */


        #endregion Inicializa Reporte

    }


}