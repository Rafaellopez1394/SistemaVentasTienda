using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Entity;
using Homex.Core.Utilities;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ConciliacionClientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

       

        public static List<ModeloConciliacionInteres> ConciliacionIntereses(string empresaid, DateTime fechaInicial, DateTime fechaFinal, List<Entity.Operacion.Cesionesdetalle> Pagos, Entity.Configuracion.Catempresa Empresa, Dictionary<int, int> ClietesExclusion, string IncluirDemandados, string SoloDemandados, Dictionary<int, int> ExclusionDemandados)
        {
            string cesionid = string.Empty;
            try {
                decimal _iva = .160000M;
                List<ModeloConciliacionInteres> ListaModeloConciliacion = new List<ModeloConciliacionInteres>();
                List<ModeloConciliacionInteres> ListaModeloConciliacionComisiones = new List<ModeloConciliacionInteres>();
                ModeloSaldoCesion _SaldoHonorariosAnterior = new ModeloSaldoCesion();
                ModeloSaldoCesion _SaldoHonorariosAnteriorAux = new ModeloSaldoCesion();

                Dictionary<int, int> CtesFacturNoAcumular3 = new Dictionary<int, int>();
                CtesFacturNoAcumular3.Add(176, 176);
                CtesFacturNoAcumular3.Add(368, 368);
                CtesFacturNoAcumular3.Add(235, 235);
                CtesFacturNoAcumular3.Add(30, 30);
                CtesFacturNoAcumular3.Add(470, 470);
                CtesFacturNoAcumular3.Add(199, 199);

                //Conciliacion de interes por cliente
                DataSet ds = MobileBO.ControlOperacion.TraerCxCobrarPorEmpresaFecha(empresaid, fechaInicial.AddDays(-1));
                DataSet dsFacturasAutomaticas = MobileBO.ControlOperacion.TraerFacturasAutomaticasConImportePorFecha(fechaFinal);

                List<DatosFacturaElectronica> ListaFacAut = (from a in dsFacturasAutomaticas.Tables[0].AsEnumerable()
                                                             select new DatosFacturaElectronica
                                                             {
                                                                 FacturaElectronicaID = a.Field<Guid>("FacturaElectronicaID").ToString().ToUpper(),
                                                                 CesionID = a.Field<Guid>("CesionID").ToString().ToUpper(),
                                                                 ClienteID = a.Field<Guid>("ClienteID").ToString().ToUpper(),
                                                                 Fec_pol = a.Field<DateTime>("Fec_pol"),
                                                                 SubTotal = a.Field<decimal>("SubTotal")
                                                             }).ToList();





                var CesionesAnteriores = (from a in ds.Tables[0].AsEnumerable()
                                          select new
                                          {
                                              CesionID = a.Field<Guid>("CesionID").ToString()
                                          }).Distinct();

                //var CesionesAnteriores = (from a in ds.Tables[0].AsEnumerable()
                //                          where a.Field<Guid>("CesionID").ToString().ToUpper() == "B81C2611-D943-45F9-A22A-712F351308D1"
                //                          || a.Field<Guid>("CesionID").ToString().ToUpper() == "53939506-4C75-4B7F-B103-D89C70E41001"
                //                          || a.Field<Guid>("CesionID").ToString().ToUpper() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5"
                //                          select new
                //                          {
                //                              CesionID = a.Field<Guid>("CesionID").ToString()
                //                          }
                //                          ).Distinct();

                #region Calcular Saldos Iniciales
                foreach (var Cesion in CesionesAnteriores)
                {
                    cesionid = Cesion.CesionID;
                    ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaInicial.AddDays(-1), true, true);

                    Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(Saldo.ClienteID, null, null);
                    if (ClietesExclusion.ContainsKey(cliente.Codigo))
                        continue;

                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    if (ListaModeloConciliacion.Find(x => x.clienteid == cliente.Clienteid.ToUpper()) == null)
                    {
                        //El cliente aun no esta en la lista
                        ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                        mcliente.clienteid = cliente.Clienteid.ToUpper();
                        mcliente.codigo = cliente.Codigo;
                        mcliente.nombre = cliente.Nombrecompleto;
                        mcliente.Concepto = "(INT)";
                        if (fechaInicial >= DateTime.Parse("01/12/2015") || Empresa.Sofom)
                        {
                            if (CtesFacturNoAcumular3.ContainsKey(mcliente.codigo) && !Empresa.Sofom)
                                mcliente.SaldoInicial = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario);
                            else
                            {
                                if (Saldo.Cesion == "R5955" && Saldo.ClienteID.ToUpper().Trim().Equals("19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA"))
                                {
                                    mcliente.SaldoInicial = 0; //(Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio);
                                }
                                //if (Saldo.Cesion == "R-5686" && Saldo.ClienteID.ToUpper().Trim().Equals("2653E98B-9982-4245-96C0-1994AF560C09"))
                                //{
                                //    mcliente.SaldoInicial = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario);
                                //}
                                else
                                {
                                    mcliente.SaldoInicial = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio) + Saldo.MoratoriosSobreHonorarios - Saldo.PagosMoratoriosSobreHonorarios;
                                }

                            }
                        }
                        else
                        {
                            mcliente.SaldoInicial = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario);
                        }

                        ListaModeloConciliacion.Add(mcliente);
                    }
                    else
                    {
                        //El cliente ya se encuenta en la lista
                        if (fechaInicial >= DateTime.Parse("01/12/2015") || Empresa.Sofom)
                        {
                            if (CtesFacturNoAcumular3.ContainsKey(ListaModeloConciliacion.Find(x => x.clienteid == cliente.Clienteid.ToUpper()).codigo) && !Empresa.Sofom)
                            {
                                ListaModeloConciliacion.Find(x => x.clienteid == cliente.Clienteid.ToUpper()).SaldoInicial += (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + Saldo.MoratoriosSobreHonorarios;
                            }
                            else
                            {
                                ListaModeloConciliacion.Find(x => x.clienteid == cliente.Clienteid.ToUpper()).SaldoInicial += (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio) + Saldo.MoratoriosSobreHonorarios;
                            }
                        }
                        else
                            ListaModeloConciliacion.Find(x => x.clienteid == cliente.Clienteid.ToUpper()).SaldoInicial += (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + Saldo.MoratoriosSobreHonorarios;
                    }

                    //Modificacion Para Acumular las comisiones
                    if (Saldo.Sofom) {
                        decimal _saldoComisionHonorarios = 0;
                        decimal _ComisionHonorariosGeneradas = 0;
                        decimal _pagoComisionHonorarios = 0;
                        if (Cesion.CesionID.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                        {
                            ModeloSaldoCesion SaldoComisionHonorarios = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaInicial.AddDays(-1), true, true, false);
                            _SaldoHonorariosAnterior = SaldoComisionHonorarios;
                            _SaldoHonorariosAnteriorAux = SaldoComisionHonorarios;
                            _saldoComisionHonorarios = SaldoComisionHonorarios.Honorarios;
                            _ComisionHonorariosGeneradas = SaldoComisionHonorarios.HonorariosGeneradosDelPeriodo;
                            _pagoComisionHonorarios = SaldoComisionHonorarios.HonorariosCobradosEnElPeriodos;


                        }
                        else
                        {
                            _SaldoHonorariosAnterior = null;
                        }

                        decimal saldoComision = (Saldo.ComisionDisposicion - Saldo.PagoComisionDisposicion) + (Saldo.ComisionAnalisis - Saldo.PagoComisionAnalisis) + _saldoComisionHonorarios;
                        //decimal saldoComision = (Saldo.ComisionDisposicion - Saldo.PagoComisionDisposicion) + (Saldo.ComisionAnalisis - Saldo.PagoComisionAnalisis) + (_SaldoHonorariosAnterior == null ? 0 : (_SaldoHonorariosAnterior.TotalHonorariosGeneradosALaFecha - _SaldoHonorariosAnterior.TotalHonorariosCobradosALaFecha));
                        //Si la comision tiene saldo vamos por la factura automatica
                        if (saldoComision > 0 && ListaFacAut.Find(x => x.CesionID == Saldo.CesionID.ToUpper() && x.Fec_pol <= fechaInicial.AddDays(-1)) != null)
                            saldoComision -= ListaFacAut.Find(x => x.CesionID == Saldo.CesionID.ToUpper() && x.Fec_pol <= fechaInicial.AddDays(-1)).SubTotal;

                        if (ListaModeloConciliacionComisiones.Find(x => x.clienteid == cliente.Clienteid.ToUpper()) == null)
                        {
                            //El cliente aun no esta en la lista
                            ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                            mcliente.clienteid = cliente.Clienteid.ToUpper();
                            mcliente.codigo = cliente.Codigo;
                            mcliente.nombre = cliente.Nombrecompleto;
                            mcliente.Concepto = "(COM)";
                            mcliente.SaldoInicial = saldoComision;
                            ListaModeloConciliacionComisiones.Add(mcliente);
                        }
                        else
                        {
                            ListaModeloConciliacionComisiones.Find(x => x.clienteid == cliente.Clienteid.ToUpper()).SaldoInicial += saldoComision;
                        }
                    }
                }
                #endregion

                #region Calcular Intereses Generados En El Periodo de Cesiones Que Ya Existen
                ModeloSaldoCesion SaldoCesionAnt, SaldoCesionAux;
                ModeloSaldoCesion SaldoCesionMoratoriosHonorariosAnt;

                //ModeloSaldoCesion _saldoPagoHonorariosymoratoriossobrehonorarios = null;
                ModeloSaldoCesion _saldoPagoHonorariosymoratoriossobrehonorarios = MobileBO.ControlOperacion.CalculaInteresOptimizado("552B4A72-7A9E-491E-B7B9-9755EA1E9EC5", fechaFinal, true, true);

                foreach (var Cesion in CesionesAnteriores) //Habilitar para produccion
                                                           //foreach (var Cesion in CesionesAnteriores.Where(x => x.CesionID.ToUpper()== "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")) //Inhabilitar para produccion
                {
                    decimal InteresGeneradoPeriodo = 0m;
                    decimal ComisionesGeneradoPeriodo = 0m;
                    if (Cesion.CesionID.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5" || Cesion.CesionID.ToUpper().Trim() == "5CFC274E-8234-4AC5-BAA3-EA48B96E4B7E")
                    {
                        SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaInicial.AddDays(-1), true, true, false);
                    }
                    else
                    {
                        SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaInicial.AddDays(-1), true, true);
                    }

                    Entity.Analisis.Catcliente cte = MobileBO.ControlAnalisis.TraerCatclientes(SaldoCesionAnt.ClienteID, null, null);

                    List<Entity.Operacion.Cesionesdetalle> PagosAplicados = Pagos.FindAll(x => x.Cesionid.ToUpper() == Cesion.CesionID.ToUpper()).OrderBy(a => a.FechaApli).ToList();
                    bool PagoRegistrado = false;
                    bool HonorariosDelPeriodoRegistrados = false;
                    foreach (Entity.Operacion.Cesionesdetalle pagoAplicado in PagosAplicados)
                    {
                        if (Cesion.CesionID.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                        {
                            SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, pagoAplicado.FechaApli, true, true, false);
                        }
                        else
                        {
                            SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, pagoAplicado.FechaApli, true, true);
                            //SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, true, true);
                        }

                        InteresGeneradoPeriodo += (SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario);
                        ComisionesGeneradoPeriodo += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (HonorariosDelPeriodoRegistrados ? 0 : SaldoCesionAux.HonorariosGeneradosDelPeriodo);
                        if (SaldoCesionAux.HonorariosGeneradosDelPeriodo > 0 && !HonorariosDelPeriodoRegistrados)
                        {
                            HonorariosDelPeriodoRegistrados = true;
                        }
                        if (Empresa.Sofom)
                        {
                            InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);
                            //InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio + (SaldoCesionAux.MoratoriosSobreHonorarios);
                            //InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio + (SaldoCesionAux.MoratoriosSobreHonorarios);
                            //ComisionesGeneradoPeriodo += (SaldoCesionAux.Honorarios - SaldoCesionAnt.Honorarios);
                            //ComisionesGeneradoPeriodo += (SaldoCesionAux.HonorariosGeneradosDelPeriodo);
                        }
                        else
                        {
                            //Acumular los intereses despues del 01 de nov del 2015                                                                
                            if (fechaInicial >= DateTime.Parse("01/11/2015") && !CtesFacturNoAcumular3.ContainsKey(cte.Codigo))
                            {
                                //Aqui acomulamos el interes total de la cesion generado hasta la fecha de corte
                                if (DateTime.Parse(SaldoCesionAnt.FechaVence) < DateTime.Parse("01/11/2015") && fechaInicial == DateTime.Parse("01/11/2015") && !PagoRegistrado)
                                    InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio + SaldoCesionAux.MoratoriosSobreHonorarios;
                                else
                                    InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio) + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);

                                PagoRegistrado = true;
                            }
                        }
                        if (Cesion.CesionID.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                        {
                            SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, pagoAplicado.FechaApli, true, true, false);
                            SaldoCesionMoratoriosHonorariosAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, pagoAplicado.FechaApli, true, true);
                        }
                        else
                        {
                            SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, pagoAplicado.FechaApli, true);
                        }

                    }

                    if (Cesion.CesionID.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                    {
                        SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, true, true, false);
                    }
                    else
                    {
                        SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, true, true);
                    }

                    if (SaldoCesionAux.Cesion == "R5955" && SaldoCesionAux.ClienteID.ToUpper().Trim().Equals("19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA"))
                    {
                        InteresGeneradoPeriodo = SaldoCesionAux.InteresOrdinario + SaldoCesionAux.MoratoriosSobreHonorarios;
                    }
                    else
                    {
                        InteresGeneradoPeriodo += (SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario) + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);
                    }

                    //decimal _honorariosDelPeriodo = SaldoCesionAux.Honorarios - SaldoCesionAnt.Honorarios <= 0 ? SaldoCesionAux.Honorarios : SaldoCesionAux.Honorarios - SaldoCesionAnt.Honorarios;
                    //decimal _honorariosDelPeriodo = SaldoCesionAux.HonorariosGeneradosDelPeriodo; 
                    //ComisionesGeneradoPeriodo += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (_honorariosDelPeriodo);
                    List<Entity.Cartera.Cesioneshonorario> _honorarios = MobileBO.ControlCartera.TraerCesioneshonorarios(null, Cesion.CesionID, null, null, null).Where(x => x.Fechavence <= fechaFinal).ToList();
                    decimal _honorariosDelPeriodo = _honorarios.Where(x => x.Fechadocu >= fechaInicial && x.Fechavence <= fechaFinal).Sum(y => y.Honorario);
                    //ComisionesGeneradoPeriodo += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (PagosAplicados.Count() == 0 ? SaldoCesionAux.HonorariosGeneradosDelPeriodo : 0);
                    ComisionesGeneradoPeriodo += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + _honorariosDelPeriodo;

                    if (Empresa.Sofom)
                    {

                        //InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio) + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);
                        InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio);


                        //ModeloSaldoCesion SaldoCesionMoratoriosHonorarios = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaInicial.AddDays(-1), true, true);
                        //ModeloSaldoCesion SaldoCesionMoratoriosHonorarios2 = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, fechaFinal, true, true);
                        //InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio) + (SaldoCesionMoratoriosHonorarios2.MoratoriosSobreHonorarios - SaldoCesionMoratoriosHonorarios.MoratoriosSobreHonorarios);
                    }
                    else
                    {
                        if (fechaInicial >= DateTime.Parse("01/11/2015") && !CtesFacturNoAcumular3.ContainsKey(cte.Codigo))
                        {
                            //Aqui acomulamos el interes total de la cesion generado hasta la fecha de corte
                            if (DateTime.Parse(SaldoCesionAnt.FechaVence) < DateTime.Parse("01/11/2015") && fechaInicial == DateTime.Parse("01/11/2015") && !PagoRegistrado)
                                InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio + SaldoCesionAux.MoratoriosSobreHonorarios;
                            else
                                //InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio) + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);
                                InteresGeneradoPeriodo += (SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio) + (SaldoCesionAnt.MoratoriosSobreHonorarios);
                        }
                    }

                    if (ListaModeloConciliacion.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()) != null)
                        ListaModeloConciliacion.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()).Cargos += InteresGeneradoPeriodo;

                    if (Empresa.Sofom)
                    {
                        if (ListaModeloConciliacionComisiones.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()) != null)
                            ListaModeloConciliacionComisiones.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()).Cargos += ComisionesGeneradoPeriodo;
                    }

                    /*else
                    {
                        throw new Exception("Inconsistencia en la informacion comunicate a sistemas");
                    }*/
                }
                #endregion

                #region  Calcular Intereses Generados En El Periodo de Cesiones Generadas dentro del periodo
                List<Entity.Operacion.Cesionesdetalle> CesionesDelPeriodo = MobileBO.ControlOperacion.TraerPagosPorPeriodo(empresaid, null, 1, fechaInicial, fechaFinal);
                //List<Entity.Operacion.Cesionesdetalle> CesionesDelPeriodo = MobileBO.ControlOperacion.TraerPagosPorPeriodo(empresaid, null, 1, fechaInicial, fechaFinal).Where(x=>x.Cesionid.ToUpper()== "B81C2611-D943-45F9-A22A-712F351308D1" || x.Cesionid.ToUpper() == "7F0654DD-9778-4866-ADAA-D3E90E1DCACE" || x.Cesionid.ToUpper() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5").ToList();
                //List<Entity.Operacion.Cesionesdetalle> CesionesDelPeriodo = MobileBO.ControlOperacion.TraerPagosPorPeriodo(empresaid, null, 1, fechaInicial, fechaFinal).Where(x => x.ClienteID.ToUpper() == "36800651-53DE-4282-A2F9-14E3D19288A9").ToList();
                foreach (Entity.Operacion.Cesionesdetalle Cesion in CesionesDelPeriodo)
                {
                    decimal InteresGeneradoPeriodo = 0m;
                    decimal InteresGeneradoPeriodoComision = 0m;
                    if (Cesion.Cesionid.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                    {
                        SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, fechaFinal, true, true, false);
                    }
                    else
                    {
                        SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, Cesion.FechaApli, true, true);
                    }

                    List<Entity.Operacion.Cesionesdetalle> PagosAplicados = Pagos.FindAll(x => x.Cesionid.ToUpper() == Cesion.Cesionid.ToUpper()).OrderBy(a => a.FechaApli).ToList();
                    bool seencontroabonoopagodelmismodiadecesion = false;
                    foreach (Entity.Operacion.Cesionesdetalle pagoAplicado in PagosAplicados)
                    {
                        if (Cesion.Cesionid.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                        {
                            SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, pagoAplicado.FechaApli, true, true, false);
                        }
                        else
                        {
                            SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, pagoAplicado.FechaApli, true, true);
                        }


                        //if (SaldoCesionAux.PagoMismoDia)
                        //{
                        //    InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario;
                        //    seencontroabonoopagodelmismodiadecesion = true;
                        //}
                        //else
                        //{
                        //    InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario + pagoAplicado.InteresOrdinario;
                        //}
                        if (pagoAplicado.FechaApli == Cesion.Fecha_Docu)
                        {
                            InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario;
                        }
                        else
                        {
                            InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario;
                        }

                        //InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario + pagoAplicado.InteresOrdinario;
                        //InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario + (SaldoCesionAux.PagoMismoDia ? pagoAplicado.InteresOrdinario : 0);
                        //InteresGeneradoPeriodoComision += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (SaldoCesionAux.Honorarios - SaldoCesionAnt.Honorarios);
                        //InteresGeneradoPeriodoComision += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) ;
                        if (SaldoCesionAux.PagoMismoDia && !seencontroabonoopagodelmismodiadecesion)
                        {
                            seencontroabonoopagodelmismodiadecesion = true;
                        }


                        if (fechaInicial >= DateTime.Parse("01/11/2015") || Empresa.Sofom)
                            InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio;

                        if (Cesion.Cesionid.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                        {
                            SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, pagoAplicado.FechaApli, true, true, false);
                        }
                        else
                        {
                            SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, pagoAplicado.FechaApli, true, true);
                            //SaldoCesionAnt = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, Cesion.FechaApli, true, true);
                        }

                    }
                    if (Cesion.Cesionid.ToUpper().Trim() == "552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")
                    {
                        SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, fechaFinal, true, true, false);
                    }
                    else
                    {
                        SaldoCesionAux = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.Cesionid, fechaFinal, true, true);
                    }
                    Entity.Analisis.Catcliente _clienteTrasnportista = MobileBO.ControlAnalisis.TraerCatclientes(Cesion.ClienteID, null, null);
                    if (_clienteTrasnportista != null && _clienteTrasnportista.Tipocliente == 5)
                    {
                        InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario;                        
                    }
                    else { 
                        InteresGeneradoPeriodo += SaldoCesionAux.InteresOrdinario - SaldoCesionAnt.InteresOrdinario + (SaldoCesionAux.MoratoriosSobreHonorarios - SaldoCesionAnt.MoratoriosSobreHonorarios);
                    }
                    //InteresGeneradoPeriodoComision += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (SaldoCesionAux.Honorarios - SaldoCesionAnt.Honorarios);

                    //decimal _honorariosDelPeriodo = SaldoCesionAux.HonorariosGeneradosDelPeriodo - SaldoCesionAux.HonorariosCobradosEnElPeriodos <= 0 ? SaldoCesionAux.HonorariosGeneradosDelPeriodo : SaldoCesionAux.HonorariosGeneradosDelPeriodo - SaldoCesionAux.HonorariosCobradosEnElPeriodos;

                    decimal _honorariosDelPeriodo = SaldoCesionAux.HonorariosGeneradosDelPeriodo;


                    //InteresGeneradoPeriodoComision += (SaldoCesionAux.ComisionAnalisis - SaldoCesionAnt.ComisionAnalisis) + (SaldoCesionAux.ComisionDisposicion - SaldoCesionAnt.ComisionDisposicion) + (_honorariosDelPeriodo);
                    InteresGeneradoPeriodoComision += SaldoCesionAux.ComisionAnalisis + SaldoCesionAux.ComisionDisposicion + _honorariosDelPeriodo;

                    if (fechaInicial >= DateTime.Parse("01/11/2015") || Empresa.Sofom)
                        InteresGeneradoPeriodo += SaldoCesionAux.InteresMoratorio - SaldoCesionAnt.InteresMoratorio;

                    if (ListaModeloConciliacion.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()) != null)
                    {
                        ListaModeloConciliacion.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()).Cargos += InteresGeneradoPeriodo;
                    }
                    else
                    {
                        Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(SaldoCesionAnt.ClienteID, null, null);
                        if (ClietesExclusion.ContainsKey(cliente.Codigo))
                            continue;

                        if (IncluirDemandados == "0" && SoloDemandados != "1")
                            if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        if (SoloDemandados == "1")
                            if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        //El cliente aun no esta en la lista
                        ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                        mcliente.clienteid = cliente.Clienteid.ToUpper();
                        mcliente.codigo = cliente.Codigo;
                        mcliente.nombre = cliente.Nombrecompleto;
                        mcliente.Concepto = "(INT)";
                        mcliente.SaldoInicial = 0m;
                        mcliente.Cargos = InteresGeneradoPeriodo;
                        ListaModeloConciliacion.Add(mcliente);
                    }

                    if (Empresa.Sofom)
                    {
                        if (ListaModeloConciliacionComisiones.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()) != null)
                        {
                            ListaModeloConciliacionComisiones.Find(x => x.clienteid == SaldoCesionAnt.ClienteID.ToUpper()).Cargos += InteresGeneradoPeriodoComision;
                        }
                        else
                        {
                            Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(SaldoCesionAnt.ClienteID, null, null);
                            if (ClietesExclusion.ContainsKey(cliente.Codigo))
                                continue;

                            if (IncluirDemandados == "0" && SoloDemandados != "1")
                                if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;

                            if (SoloDemandados == "1")
                                if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;

                            //El cliente aun no esta en la lista
                            ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                            mcliente.clienteid = cliente.Clienteid.ToUpper();
                            mcliente.codigo = cliente.Codigo;
                            mcliente.nombre = cliente.Nombrecompleto;
                            mcliente.Concepto = "(COM)";
                            mcliente.SaldoInicial = 0m;
                            mcliente.Cargos = InteresGeneradoPeriodoComision;
                            ListaModeloConciliacionComisiones.Add(mcliente);
                        }
                    }
                }
                #endregion



                #region Calcular Intereses Cobrados en el Periodo
                foreach (Entity.Operacion.Cesionesdetalle Pago in Pagos)
                {
                    if (ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()) != null)
                    {
                        if (Empresa.Sofom)
                        {
                            //Quite esta linea por que estaba descuadrando el reporte de conciliacion de clientes
                            //ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario + Pago.InteresMoratorio + Pago.Bonificacion;
                            ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario + Pago.InteresMoratorio;
                        }
                        else
                        {
                            if (fechaInicial >= DateTime.Parse("01/11/2015") && !CtesFacturNoAcumular3.ContainsKey(ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).codigo))
                            {
                                //ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario + Pago.InteresMoratorio + (Pago.Bonificacion/(1 +_iva));
                                ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario + Pago.InteresMoratorio;
                            }
                            else
                            {
                                //ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario + (Pago.Bonificacion / (1 + _iva));
                                ListaModeloConciliacion.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.InteresOrdinario;
                            }
                        }

                    }
                    if (Empresa.Sofom)
                    {
                        if (ListaModeloConciliacionComisiones.Find(x => x.clienteid == Pago.ClienteID.ToUpper()) != null)
                        {
                            //Si no encuentra una factura automatica entonces acumula el pago 
                            if (ListaFacAut.Find(x => x.CesionID == Pago.Cesionid.ToUpper()) == null)
                                ListaModeloConciliacionComisiones.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.ComisionAnalisis + Pago.ComisionDisposicion;
                            else
                            {
                                //el pago se hizo antes que se hiciera la factura automatica por lo tanto si aplica
                                if (Pago.FechaApli <= ListaFacAut.Find(x => x.CesionID == Pago.Cesionid.ToUpper()).Fec_pol)
                                    ListaModeloConciliacionComisiones.Find(x => x.clienteid == Pago.ClienteID.ToUpper()).Abonos += Pago.ComisionAnalisis + Pago.ComisionDisposicion;
                            }
                        }
                    }
                }

                ModeloConciliacionInteres _modeloInteresesSobreMoratoriosHonorarios = ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == "2653E98B-9982-4245-96C0-1994AF560C09");


                if (_modeloInteresesSobreMoratoriosHonorarios != null)
                {
                    //No debe entrar aqui los honorarios se cobran en las comisiones
                    //ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == "2653E98B-9982-4245-96C0-1994AF560C09").Abonos += _saldoPagoHonorariosymoratoriossobrehonorarios.PagosMoratoriosSobreHonorarios;
                }



                if (_modeloInteresesSobreMoratoriosHonorarios != null)
                {

                    //ListaModeloConciliacionComisiones.Find(x => x.clienteid.ToUpper() == "2653E98B-9982-4245-96C0-1994AF560C09").Abonos += (_saldoPagoHonorariosymoratoriossobrehonorarios.HonorariosCobradosEnElPeriodos - (_SaldoHonorariosAnteriorAux == null ? 0 : _SaldoHonorariosAnteriorAux.HonorariosCobradosEnElPeriodos));

                    ListaModeloConciliacionComisiones.Find(x => x.clienteid.ToUpper() == "2653E98B-9982-4245-96C0-1994AF560C09").Abonos += _saldoPagoHonorariosymoratoriossobrehonorarios.HonorariosCobradosEnElPeriodos - _SaldoHonorariosAnteriorAux.HonorariosCobradosEnElPeriodos;
                }

                #endregion

                #region Buscamos las facturas diversas de los clientes en el periodo
                var _clientes = ListaModeloConciliacion.GroupBy(x => x.clienteid).Select(c => new { clienteid = c.Key });
                DataSet dsFacturas = MobileBO.ControlOperacion.TraerFacturasElectronicasPorRangoFecha(fechaInicial, fechaFinal);

                _clientes.ToList().ForEach(x => {
                    List<Entity.Analisis.Catclientesregularesdiverso> _clientesregularesdiversos = MobileBO.ControlAnalisis.TraerCatclientesregularesdiversos(null, x.clienteid, null);
                    if (_clientesregularesdiversos.Count > 0)
                    {
                        var _facturas = dsFacturas.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper());
                        _facturas.ToList().ForEach(f =>
                        {
                            Entity.Operacion.Catfacturaselectronica _factura = MobileBO.ControlOperacion.TraerCatfacturaselectronicas(f.Field<Guid>("FacturaElectronicaID").ToString(), true);
                            if (_factura != null)
                            {
                                ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Abonos += _factura.CatFacturaElectronicaDetalle.Where(d => d.Concepto.Contains("INTERES")).Sum(s => s.Total);
                            }
                        });
                    }

                });


                #endregion

                #region Facturas Automaticas
                ListaFacAut = ListaFacAut.FindAll(x => x.Fec_pol >= fechaInicial && x.Fec_pol <= fechaFinal);
                foreach (DatosFacturaElectronica factura in ListaFacAut)
                {
                    if (ListaModeloConciliacionComisiones.Find(x => x.clienteid == factura.ClienteID.ToUpper()) != null)
                    {
                        ListaModeloConciliacionComisiones.Find(x => x.clienteid == factura.ClienteID.ToUpper()).Abonos += factura.SubTotal;
                    }
                }
                #endregion


                foreach (ModeloConciliacionInteres modelo in ListaModeloConciliacion)
                {
                    modelo.EnDemanda = (MobileBO.ControlOperacion.ChecaClienteMorosoValidaExiste(empresaid, modelo.clienteid, 1) ? 1 : 0);
                    modelo.SaldoFinal = modelo.SaldoInicial + modelo.Cargos - modelo.Abonos;
                }

                foreach (ModeloConciliacionInteres modelo in ListaModeloConciliacionComisiones)
                {
                    modelo.EnDemanda = (MobileBO.ControlOperacion.ChecaClienteMorosoValidaExiste(empresaid, modelo.clienteid, 1) ? 1 : 0);
                    modelo.SaldoFinal = modelo.SaldoInicial + modelo.Cargos - modelo.Abonos;
                }

                //Hasta aqui ya tengo un arreglo con las comisiones que se van a mostrar en el reporte.
                if (Empresa.Sofom) {
                    ListaModeloConciliacion.AddRange(ListaModeloConciliacionComisiones);
                    ListaModeloConciliacion = ListaModeloConciliacion.OrderBy(x => x.codigo).ThenByDescending(x => x.Concepto).ToList();
                }
                return ListaModeloConciliacion;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + " " + cesionid);
            }
        }

        public static List<ModeloConciliacionInteres> ConciliacionCliente(string empresaid, DateTime fechaInicial, DateTime fechaFinal, List<Entity.Operacion.Cesionesdetalle> Pagos, Entity.Configuracion.Catempresa Empresa, Dictionary<int, int> ClietesExclusion, string IncluirDemandados, string SoloDemandados, Dictionary<int, int> ExclusionDemandados)
        {
            List<ModeloConciliacionInteres> ListaModeloConciliacion = new List<ModeloConciliacionInteres>();

            //2024-05-08
            //Fausto Montenegro
            //Se crea un método temporal para calcular los movimientos de los transportistas en lo que se hace bien la correción, este método no se eliminará para dejar el historial hasta que se actualice por el nuevo
            //se retornará aquí el resultado de la llamada al método temporal
            return ConciliacionClienteTransportistas(empresaid, fechaInicial, fechaFinal, Pagos, Empresa,  ClietesExclusion, IncluirDemandados,SoloDemandados, ExclusionDemandados);


            //Conciliacion de Facturacion por Cliente
            #region Calculamos los saldos iniciales de las facturas automaticas
            DataSet dsFact = MobileBO.ControlOperacion.TraerFactAutSaldoIni(empresaid, fechaInicial.AddDays(-1));
            Dictionary<string, decimal> FacSalInicial = new Dictionary<string, decimal>();
            foreach (DataRow row in dsFact.Tables[0].Rows)
            {
                List<Entity.Operacion.Cesionesdetalle> pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(row["CesionID"].ToString(), 11, fechaInicial.AddDays(-1));

                decimal Total = decimal.Parse(row["Total"].ToString());
                decimal Abono = 0m;
                foreach (Entity.Operacion.Cesionesdetalle pago in pagos)
                {
                    if (pago.FechaApli > DateTime.Parse(row["fec_pol"].ToString()))
                        Abono += Math.Round((pago.ComisionAnalisis + pago.ComisionDisposicion) * 1.16m, 2);
                }
                if (Total - Abono > 0)
                {
                    //Genera saldo inicial
                    FacSalInicial.Add(row["CesionID"].ToString(), Total - Abono);
                    if (ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()) == null)
                    {
                        //El cliente aun no esta en la lista
                        Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(row["ClienteID"].ToString().ToUpper(), null, null);
                        ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                        mcliente.clienteid = row["ClienteID"].ToString().ToUpper();
                        mcliente.codigo = cliente.Codigo;
                        mcliente.nombre = cliente.Nombrecompleto;
                        mcliente.SaldoInicial = Total - Abono;
                        if (ClietesExclusion.ContainsKey(cliente.Codigo))
                            continue;

                        if (IncluirDemandados == "0" && SoloDemandados != "1")
                            if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        if (SoloDemandados == "1")
                            if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        ListaModeloConciliacion.Add(mcliente);
                    }
                    else
                    {
                        //El cliente ya se encuenta en la lista
                        ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()).SaldoInicial += Total - Abono;
                    }
                }
            }
            #endregion

            #region Calculamos y verificamos si ya estan pagadas las facturas automaticas generadas en el periodo
            DataSet dsFactPrdo = MobileBO.ControlOperacion.TraerFactAutRangoFechas(empresaid, fechaInicial, fechaFinal);
            foreach (DataRow row in dsFactPrdo.Tables[0].Rows)
            {
                decimal Total = decimal.Parse(row["Total"].ToString());
                if (ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()) == null)
                {
                    //El cliente aun no esta en la lista
                    Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(row["ClienteID"].ToString().ToUpper(), null, null);
                    ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                    mcliente.clienteid = row["ClienteID"].ToString().ToUpper();
                    mcliente.codigo = cliente.Codigo;
                    mcliente.nombre = cliente.Nombrecompleto;
                    mcliente.Cargos = Total;
                    if (ClietesExclusion.ContainsKey(cliente.Codigo))
                        continue;

                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    ListaModeloConciliacion.Add(mcliente);
                }
                else
                {
                    //El cliente ya se encuenta en la lista
                    ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()).Cargos += Total;
                }
            }

            decimal sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Cargos);
            decimal sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            decimal sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            decimal sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);
            #endregion
            string Facturaelectronicaidref = null;
            string NotaCreditoIDref = null;
            #region Calculamos los importes de las facturas realizadas en el periodo
            foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
            {
                //Se modifica la siguiente instrucción para tomar el total de la factura según solicitud de ticket #S6583
                //Gilda realizó mal una factura en agosto y ésta se volvió a generar en septiembre, contabilidad solicitó que no se moviera nada ya, pero que este reporte
                //debe mostrar el importe de la nueva factura realizada en septiembre, para ello se valida que cuando encuentre la factura realizada en agosto, el sistema
                //vaya y tome los datos de la factura realizada en septiembre
                if (pago.Facturaelectronicaid != null && pago.NotaCreditoID == null)
                {//20240201 Se Agrega Validacion por notas de credito de transportistas 
                    Entity.Operacion.Catfacturaselectronica fe = MobileBO.ControlOperacion.TraerCatfacturaselectronicas(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? "D3389CD6-8FD3-4B09-BCD1-E01377B198DF" : pago.Facturaelectronicaid, false);
                    if (fe != null)
                    {
                        if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == (fe.Tipo == "P" ? pago.ClienteID.ToUpper() : fe.Clienteid.ToUpper())) == null)
                        {
                            //El cliente aun no esta en la lista
                            //Se modifica la siguiente instrucción para tomar el total de la factura según solicitud de ticket #S6583

                            Entity.Analisis.Catcliente cliente;
                            if (fe.Tipo == "P")
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.ClienteID, null, null);
                            }
                            else
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? pago.ClienteID : fe.Clienteid, null, null);
                            }

                            if (cliente == null)
                            {
                                throw new Exception("No se encontró el cliente: error Contabilidad ConciliacionCliente 5001");

                            }

                            ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                            mcliente.clienteid = fe.Tipo == "P" ? pago.ClienteID : fe.Clienteid.ToUpper();
                            mcliente.codigo = cliente.Codigo;
                            mcliente.nombre = cliente.Nombrecompleto;
                            mcliente.Cargos = fe.Tipo == "P" ? (pago.InteresOrdinario + pago.InteresMoratorio + Math.Round(((pago.ComisionAnalisis + pago.ComisionDisposicion) * 1.16m), 2)) : fe.Total;
                            


                            //Se modifica la siguiente instrucción para tomar el pago de la factura según solicitud de ticket #S6583
                            if (pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb")
                            {
                                mcliente.Abonos = fe.Total;
                            }
                            else
                            {
                                mcliente.Abonos = (Empresa.Sofom ? (pago.InteresOrdinario + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : (pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m);
                            }


                            if (ClietesExclusion.ContainsKey(cliente.Codigo))
                                continue;

                            if (IncluirDemandados == "0" && SoloDemandados != "1")
                                if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;

                            if (SoloDemandados == "1")
                                if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;

                            ListaModeloConciliacion.Add(mcliente);
                        }
                        else
                        {
                            //El cliente ya se encuenta en la lista
                            if (fe.Tipo == "P")
                            {
                                decimal _cargos = (Empresa.Sofom ? (Math.Round(pago.InteresOrdinario, 2) + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : Math.Round((pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m, 2));
                                ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == pago.ClienteID.ToUpper()).Cargos += _cargos;
                            }
                            else
                            {
                                ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == fe.Clienteid.ToUpper()).Cargos += fe.Total;
                            }

                            ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == (fe.Tipo == "P" ? pago.ClienteID.ToUpper() : fe.Clienteid.ToUpper())).Abonos += (Empresa.Sofom ? (Math.Round(pago.InteresOrdinario, 2) + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : Math.Round((pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m, 2));
                        }
                    }
                }
                else //20240201 Se Agrega Validacion por notas de credito de transportistas 
                {
                    Facturaelectronicaidref = pago.Facturaelectronicaid;
                    NotaCreditoIDref = pago.NotaCreditoID;
                }
            }
            sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x=>x.Cargos);
            sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);

            
            #endregion

            #region Buscamos las facturas diversas de los clientes en el periodo
            var _clientes = ListaModeloConciliacion.GroupBy(x => x.clienteid).Select(c => new { clienteid = c.Key });
            DataSet ds = MobileBO.ControlOperacion.TraerFacturasElectronicasPorRangoFecha(fechaInicial, fechaFinal);
            _clientes.ToList().ForEach(x => {
                List<Entity.Analisis.Catclientesregularesdiverso> _clientesregularesdiversos = MobileBO.ControlAnalisis.TraerCatclientesregularesdiversos(null, x.clienteid, null);
                if (_clientesregularesdiversos.Count > 0)
                {
                    var _facturas = ds.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper() && f.Field<Guid>("EmpresaID").ToString().ToUpper() == empresaid.ToUpper());                    
                    decimal _importeFacturas = _facturas.Sum(i => i.Field<decimal>("Total"));
                    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Cargos += _importeFacturas;
                    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Abonos += _importeFacturas;
                }

            });

            DataSet dsAbonos = MobileBO.ControlOperacion.TraerPagoFacturasElectronicasPorRangoFecha(fechaInicial, fechaFinal);

            var _clientesDiversos = ds.Tables[0].AsEnumerable()
                .Where(x => x.Field<Guid>("EmpresaID").ToString().ToUpper() == empresaid.ToUpper())
                .GroupBy(x => x.Field<Guid>("clienteid").ToString())
                .Select(c => new { clienteid = c.Key });

            _clientesDiversos.ToList().ForEach(x => {
                List<Entity.Analisis.Catclientesregularesdiverso> _clientesregularesdiversos = MobileBO.ControlAnalisis.TraerCatclientesregularesdiversos(null, null, x.clienteid.ToUpper());

                if (_clientesregularesdiversos.Count > 0)
                {
                    Entity.Analisis.Catcliente _cliente = MobileBO.ControlAnalisis.TraerCatclientes(_clientesregularesdiversos.FirstOrDefault().Clienteid, null, null);

                    if (_cliente.Tipocliente == 5)
                    {
                        var _facturas = ds.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper() && f.Field<int>("Estatus") == 1);
                        var _facturasAbono = dsAbonos.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper());

                        decimal _importeFacturas = _facturas.Sum(i => i.Field<decimal>("Total"));
                        decimal _importeFacturasAbono = _facturasAbono.Sum(i => i.Field<decimal>("Abono"));

                        //if (ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList().Count() > 0)
                        //{
                        //    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Cargos += _importeFacturas;
                        //    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Abonos += _importeFacturasAbono;
                        //}


                        if (ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList().Count() > 0)
                        {
                            ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList()[0].Cargos += _importeFacturas;
                            ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList()[0].Abonos += _importeFacturasAbono;


                        }
                        else
                        {
                            DateTime fechaAComparar = new DateTime(2022, 12, 01, 15, 04, 15, 310);
                            DateTime fechaActual = DateTime.Now;
                            if (fechaActual > fechaAComparar)
                            {
                                ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                                mcliente.clienteid = _cliente.Clienteid.ToUpper();
                                mcliente.codigo = _cliente.Codigo;
                                mcliente.nombre = _cliente.Nombrecompleto;
                                mcliente.Cargos = _importeFacturas;
                                mcliente.Abonos = _importeFacturasAbono;
                                ListaModeloConciliacion.Add(mcliente);
                            }
                            else
                            {
                                ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                                mcliente.clienteid = _cliente.Clienteid.ToUpper();
                                mcliente.codigo = _cliente.Codigo;
                                mcliente.nombre = _cliente.Nombrecompleto;
                                mcliente.Cargos = _importeFacturas;
                                mcliente.Abonos = _importeFacturas;
                                ListaModeloConciliacion.Add(mcliente);
                            }
                        }
                    }
                }
            });

            sumafacturasCargos = 0;
            sumafacturasAbonos = 0;
            sumafacturasCargosDiverso = 0;
            sumafacturasAbonosDDiverso = 0;

            sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Cargos);
            sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);


            #endregion
            //Ajustamos los salditos a 2 decimales
            foreach (ModeloConciliacionInteres item in ListaModeloConciliacion)
            {
                item.Cargos = Math.Round(item.Cargos, 2);
                item.Abonos = Math.Round(item.Abonos, 2);
            }

            foreach (ModeloConciliacionInteres modelo in ListaModeloConciliacion)
            {
                modelo.EnDemanda = (MobileBO.ControlOperacion.ChecaClienteMorosoValidaExiste(empresaid, modelo.clienteid, 1) ? 1 : 0);

                modelo.SaldoFinal = modelo.SaldoInicial + modelo.Cargos - modelo.Abonos;
            }

            ListaModeloConciliacion.ForEach(x =>
            {
                decimal dif = x.Cargos - x.Abonos;
                if (dif == -0.01m)
                    x.Cargos += 0.01m;
                if (dif == 0.01m)
                    x.Cargos -= 0.01m;

                x.SaldoFinal = x.SaldoInicial + x.Cargos - x.Abonos;

            });
            return ListaModeloConciliacion;
        }


        public static List<ModeloConciliacionInteres> ConciliacionClienteTransportistas(string empresaid, DateTime fechaInicial, DateTime fechaFinal, List<Entity.Operacion.Cesionesdetalle> Pagos, Entity.Configuracion.Catempresa Empresa, Dictionary<int, int> ClietesExclusion, string IncluirDemandados, string SoloDemandados, Dictionary<int, int> ExclusionDemandados)
        {
            List<ModeloConciliacionInteres> ListaModeloConciliacion = new List<ModeloConciliacionInteres>();

            //Conciliacion de Facturacion por Cliente
            #region Calculamos los saldos iniciales de las facturas automaticas
            DataSet dsFact = MobileBO.ControlOperacion.TraerFactAutSaldoIni(empresaid, fechaInicial.AddDays(-1));
            Dictionary<string, decimal> FacSalInicial = new Dictionary<string, decimal>();
            foreach (DataRow row in dsFact.Tables[0].Rows)
            {
                List<Entity.Operacion.Cesionesdetalle> pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(row["CesionID"].ToString(), 11, fechaInicial.AddDays(-1));

                decimal Total = decimal.Parse(row["Total"].ToString());
                decimal Abono = 0m;
                foreach (Entity.Operacion.Cesionesdetalle pago in pagos)
                {
                    if (pago.FechaApli > DateTime.Parse(row["fec_pol"].ToString()))
                        Abono += Math.Round((pago.ComisionAnalisis + pago.ComisionDisposicion) * 1.16m, 2);
                }
                if (Total - Abono > 0)
                {
                    //Genera saldo inicial
                    FacSalInicial.Add(row["CesionID"].ToString(), Math.Round(Total - Abono, 2));//Total - Abono;);
                    if (ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()) == null)
                    {
                        //El cliente aun no esta en la lista
                        Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(row["ClienteID"].ToString().ToUpper(), null, null);
                        ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                        mcliente.clienteid = row["ClienteID"].ToString().ToUpper();
                        mcliente.codigo = cliente.Codigo;
                        mcliente.nombre = cliente.Nombrecompleto;
                        mcliente.SaldoInicial = Math.Round(Total - Abono, 2);//Total - Abono;
                        if (ClietesExclusion.ContainsKey(cliente.Codigo))
                            continue;

                        if (IncluirDemandados == "0" && SoloDemandados != "1")
                            if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        if (SoloDemandados == "1")
                            if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                continue;

                        ListaModeloConciliacion.Add(mcliente);
                    }
                    else
                    {
                        //El cliente ya se encuenta en la lista
                        ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()).SaldoInicial += Math.Round(Total - Abono, 2);//Total - Abono;
                    }
                }
            }
            #endregion

            #region Calculamos y verificamos si ya estan pagadas las facturas automaticas generadas en el periodo
            DataSet dsFactPrdo = MobileBO.ControlOperacion.TraerFactAutRangoFechas(empresaid, fechaInicial, fechaFinal);
            foreach (DataRow row in dsFactPrdo.Tables[0].Rows)
            {
                decimal Total = decimal.Parse(row["Total"].ToString());
                if (ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()) == null)
                {
                    //El cliente aun no esta en la lista
                    Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(row["ClienteID"].ToString().ToUpper(), null, null);
                    ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                    mcliente.clienteid = row["ClienteID"].ToString().ToUpper();
                    mcliente.codigo = cliente.Codigo;
                    mcliente.nombre = cliente.Nombrecompleto;
                    mcliente.Cargos = Total;
                    if (ClietesExclusion.ContainsKey(cliente.Codigo))
                        continue;

                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                            continue;

                    ListaModeloConciliacion.Add(mcliente);
                }
                else
                {
                    //El cliente ya se encuenta en la lista
                    ListaModeloConciliacion.Find(x => x.clienteid == row["ClienteID"].ToString().ToUpper()).Cargos += Total;
                }
            }

            decimal sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Cargos);
            decimal sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            decimal sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            decimal sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);
            #endregion


            string Facturaelectronicaidref = null;
            string NotaCreditoIDref = null;
            #region Calculamos los importes de las facturas realizadas en el periodo
            foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
            {
                //Se modifica la siguiente instrucción para tomar el total de la factura según solicitud de ticket #S6583
                //Gilda realizó mal una factura en agosto y ésta se volvió a generar en septiembre, contabilidad solicitó que no se moviera nada ya, pero que este reporte
                //debe mostrar el importe de la nueva factura realizada en septiembre, para ello se valida que cuando encuentre la factura realizada en agosto, el sistema
                //vaya y tome los datos de la factura realizada en septiembre
                if (pago.Facturaelectronicaid != null && pago.NotaCreditoID == null)
                {//20240201 Se Agrega Validacion por notas de credito de transportistas 
                    Entity.Operacion.Catfacturaselectronica fe = MobileBO.ControlOperacion.TraerCatfacturaselectronicas(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? "D3389CD6-8FD3-4B09-BCD1-E01377B198DF" : pago.Facturaelectronicaid, false);
                    if (fe != null)
                    {
                        if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == (fe.Tipo == "P" ? pago.ClienteID.ToUpper() : fe.Clienteid.ToUpper())) == null)
                        {
                            //El cliente aun no esta en la lista
                            //Se modifica la siguiente instrucción para tomar el total de la factura según solicitud de ticket #S6583

                            Entity.Analisis.Catcliente cliente;
                            if (fe.Tipo == "P")
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.ClienteID, null, null);
                            }
                            else
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? pago.ClienteID : fe.Clienteid, null, null);
                            }

                            if (cliente == null)
                            {
                                throw new Exception("No se encontró el cliente: error Contabilidad ConciliacionCliente 5001");

                            }

                            ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                            mcliente.clienteid = fe.Tipo == "P" ? pago.ClienteID : fe.Clienteid.ToUpper();
                            mcliente.codigo = cliente.Codigo;
                            mcliente.nombre = cliente.Nombrecompleto;
                            //mcliente.Cargos = fe.Tipo == "P" ? (pago.InteresOrdinario + pago.InteresMoratorio + Math.Round(((pago.ComisionAnalisis + pago.ComisionDisposicion) * 1.16m), 2)) : fe.Total;
                            mcliente.Cargos = fe.Tipo == "P" ? 0 : fe.Total;

                            //Se modifica la siguiente instrucción para tomar el pago de la factura según solicitud de ticket #S6583
                            if (pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb")
                            {
                                mcliente.Abonos = fe.Total;
                            }
                            else
                            {
                                mcliente.Abonos = (Empresa.Sofom ? (pago.InteresOrdinario + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : (pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m);
                            }


                            if (ClietesExclusion.ContainsKey(cliente.Codigo))
                                continue;

                            if (IncluirDemandados == "0" && SoloDemandados != "1")
                                if (ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;

                            if (SoloDemandados == "1")
                                if (!ExclusionDemandados.ContainsKey(cliente.Codigo))
                                    continue;
                            if(cliente.Tipocliente == 5 && (mcliente.codigo == 824 || mcliente.codigo == 959))
                            {
                                DataSet dssi = MobileBO.ControlOperacion.TraerCesionesdetallesaldoinicial(mcliente.clienteid, fechaInicial);
                                if (dssi.Tables.Count > 0)
                                {
                                    DataTable dt = dssi.Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            decimal saldoInicial = row.Field<decimal>("SaldoInicial");
                                            mcliente.SaldoInicial = saldoInicial;
                                        }
                                    }
                                }
                            }

                            ListaModeloConciliacion.Add(mcliente);
                        }
                        else
                        {
                            //El cliente ya se encuenta en la lista
                            if (fe.Tipo == "P")
                            {
                                decimal _cargos = (Empresa.Sofom ? (Math.Round(pago.InteresOrdinario, 2) + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : Math.Round((pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m, 2));
                                //ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == pago.ClienteID.ToUpper()).Cargos += _cargos;
                            }
                            else
                            {
                                ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == fe.Clienteid.ToUpper()).Cargos += fe.Total;
                            }

                            ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == (fe.Tipo == "P" ? pago.ClienteID.ToUpper() : fe.Clienteid.ToUpper())).Abonos += (Empresa.Sofom ? (Math.Round(pago.InteresOrdinario, 2) + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : Math.Round((pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m, 2));
                        }
                    }
                }
                else //20240201 Se Agrega Validacion por notas de credito de transportistas 
                {
                    Facturaelectronicaidref = pago.Facturaelectronicaid;
                    NotaCreditoIDref = pago.NotaCreditoID;
                    Entity.Operacion.Catfacturaselectronica fe = MobileBO.ControlOperacion.TraerCatfacturaselectronicas(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? "D3389CD6-8FD3-4B09-BCD1-E01377B198DF" : pago.Facturaelectronicaid, false);
                    if (fe != null)
                    {
                        if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == (fe.Tipo == "P" ? pago.ClienteID.ToUpper() : fe.Clienteid.ToUpper())) == null)
                        {
                            Entity.Analisis.Catcliente cliente;
                            if (fe.Tipo == "P")
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.ClienteID, null, null);
                            }
                            else
                            {
                                cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.Facturaelectronicaid.ToLower() == "cde48e41-47dc-4463-866a-71a69a6bdbeb" ? pago.ClienteID : fe.Clienteid, null, null);
                            }
                            ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                            mcliente.clienteid = fe.Tipo == "P" ? pago.ClienteID : fe.Clienteid.ToUpper();
                            mcliente.codigo = cliente.Codigo;
                            mcliente.nombre = cliente.Nombrecompleto;
                            //mcliente.Cargos = fe.Tipo == "P" ? (pago.InteresOrdinario + pago.InteresMoratorio + Math.Round(((pago.ComisionAnalisis + pago.ComisionDisposicion) * 1.16m), 2)) : fe.Total;
                            mcliente.Cargos = fe.Tipo == "P" ? 0 : fe.Total;
                            
                            mcliente.Abonos = (Empresa.Sofom ? (pago.InteresOrdinario + Math.Round(((pago.ComisionDisposicion + pago.ComisionAnalisis) * 1.16m), 2) + pago.InteresMoratorio) : (pago.InteresOrdinario + pago.InteresMoratorio) * 1.16m);
                            
                            ListaModeloConciliacion.Add(mcliente);
                        }
                    }
                }
            }
            sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Cargos);
            sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);


            #endregion

            #region Buscamos las facturas diversas de los clientes en el periodo
            var _clientes = ListaModeloConciliacion.GroupBy(x => x.clienteid).Select(c => new { clienteid = c.Key });
            DataSet ds = MobileBO.ControlOperacion.TraerFacturasElectronicasPorRangoFecha(fechaInicial, fechaFinal);
            _clientes.ToList().ForEach(x => {
                List<Entity.Analisis.Catclientesregularesdiverso> _clientesregularesdiversos = MobileBO.ControlAnalisis.TraerCatclientesregularesdiversos(null, x.clienteid, null);
                if (_clientesregularesdiversos.Count > 0)
                {
                    var _facturas = ds.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper() && f.Field<Guid>("EmpresaID").ToString().ToUpper() == empresaid.ToUpper());
                    decimal _importeFacturas = _facturas.Sum(i => i.Field<decimal>("Total"));


                    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Cargos += _importeFacturas;
                    //ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Abonos += _importeFacturas;
                }

            });

            DataSet dsAbonos = MobileBO.ControlOperacion.TraerPagoFacturasElectronicasPorRangoFecha(fechaInicial, fechaFinal);

            var _clientesDiversos = ds.Tables[0].AsEnumerable()
                .Where(x => x.Field<Guid>("EmpresaID").ToString().ToUpper() == empresaid.ToUpper())
                .GroupBy(x => x.Field<Guid>("clienteid").ToString())
                .Select(c => new { clienteid = c.Key });

            _clientesDiversos.ToList().ForEach(x => {
                List<Entity.Analisis.Catclientesregularesdiverso> _clientesregularesdiversos = MobileBO.ControlAnalisis.TraerCatclientesregularesdiversos(null, null, x.clienteid.ToUpper());

                if (_clientesregularesdiversos.Count > 0)
                {
                    Entity.Analisis.Catcliente _cliente = MobileBO.ControlAnalisis.TraerCatclientes(_clientesregularesdiversos.FirstOrDefault().Clienteid, null, null);

                    if (_cliente.Tipocliente == 5)
                    {
                        var _facturas = ds.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper() && f.Field<int>("Estatus") == 1);
                        var _facturasAbono = dsAbonos.Tables[0].AsEnumerable().Where(f => f.Field<Guid>("ClienteID").ToString().ToUpper() == _clientesregularesdiversos[0].Clientediversoid.ToUpper());

                        decimal _importeFacturas = _facturas.Sum(i => i.Field<decimal>("Total"));
                        decimal _importeFacturasAbono = _facturasAbono.Sum(i => i.Field<decimal>("Abono"));

                        //if (ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList().Count() > 0)
                        //{
                        //    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Cargos += _importeFacturas;
                        //    ListaModeloConciliacion.Where(m => m.clienteid == x.clienteid).ToList()[0].Abonos += _importeFacturasAbono;
                        //}


                        if (ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList().Count() > 0)
                        {
                            //ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList()[0].Cargos += _importeFacturas;
                            ListaModeloConciliacion.Where(m => m.clienteid == _clientesregularesdiversos[0].Clienteid).ToList()[0].Abonos += _importeFacturasAbono;


                        }
                        else
                        {
                            DateTime fechaAComparar = new DateTime(2022, 12, 01, 15, 04, 15, 310);
                            DateTime fechaActual = DateTime.Now;
                            if (fechaActual > fechaAComparar)
                            {
                                ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                                mcliente.clienteid = _cliente.Clienteid.ToUpper();
                                mcliente.codigo = _cliente.Codigo;
                                mcliente.nombre = _cliente.Nombrecompleto;
                                //mcliente.Cargos = _importeFacturas;
                                mcliente.Abonos = _importeFacturasAbono;
                                ListaModeloConciliacion.Add(mcliente);
                            }
                            else
                            {
                                ModeloConciliacionInteres mcliente = new ModeloConciliacionInteres();
                                mcliente.clienteid = _cliente.Clienteid.ToUpper();
                                mcliente.codigo = _cliente.Codigo;
                                mcliente.nombre = _cliente.Nombrecompleto;
                                //mcliente.Cargos = _importeFacturas;
                                mcliente.Abonos = _importeFacturas;
                                ListaModeloConciliacion.Add(mcliente);
                            }
                        }
                    }
                }
            });

            sumafacturasCargos = 0;
            sumafacturasAbonos = 0;
            sumafacturasCargosDiverso = 0;
            sumafacturasAbonosDDiverso = 0;

            sumafacturasCargos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Cargos);
            sumafacturasAbonos = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "2A049C73-BCCF-4B08-883A-291CC3834BCD").Sum(x => x.Abonos);
            sumafacturasCargosDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Cargos);
            sumafacturasAbonosDDiverso = ListaModeloConciliacion.Where(x => x.clienteid.ToUpper() == "7763D0B2-8744-411E-8CDB-4DC7412A4696").Sum(x => x.Abonos);


            #endregion
            //Ajustamos los salditos a 2 decimales
            foreach (ModeloConciliacionInteres item in ListaModeloConciliacion)
            {
                item.Cargos = Math.Round(item.Cargos, 2);
                item.Abonos = Math.Round(item.Abonos, 2);
            }

            foreach (ModeloConciliacionInteres modelo in ListaModeloConciliacion)
            {
                modelo.EnDemanda = (MobileBO.ControlOperacion.ChecaClienteMorosoValidaExiste(empresaid, modelo.clienteid, 1) ? 1 : 0);

                modelo.SaldoFinal = modelo.SaldoInicial + modelo.Cargos - modelo.Abonos;
            }

            ListaModeloConciliacion.ForEach(x =>
            {
                decimal dif = x.Cargos - x.Abonos;
                if (dif == -0.01m)
                    x.Cargos += 0.01m;
                if (dif == 0.01m)
                    x.Cargos -= 0.01m;
                if (x.SaldoInicial == 0.01m)
                    x.SaldoInicial -= 0.01m;

                x.SaldoFinal = x.SaldoInicial + x.Cargos - x.Abonos;

            });

            ListaModeloConciliacion.RemoveAll(x => x.SaldoInicial == 0 && x.Cargos == 0 && x.Abonos == 0); // se agrega esto para quitar empresas que no operaron

            return ListaModeloConciliacion;
        }


        public static List<ModeloConciliacionInteres> ConciliacionCapitalFinanciado(string empresaid, DateTime fechaInicial, DateTime fechaFinal, List<Entity.Operacion.Cesionesdetalle> Pagos, string Demandados, string IncluirDemandados, string SoloDemandados, Dictionary<int, int> ExclusionDemandados)
        {
            List<ModeloConciliacionInteres> ListaModeloConciliacion = new List<ModeloConciliacionInteres>();

            //Obtenemos el saldo inicial del capital de los clientes de los clientes
            //DataSet CesionesSaldoInicial = MobileBO.ControlOperacion.TraerSaldoInicialConCap(empresaid, (Demandados == "0" ? false : true), fechaInicial);
            DataSet CesionesSaldoInicial = MobileBO.ControlOperacion.TraerSaldoInicialConCap(empresaid, true, fechaInicial);
            foreach (DataRow row in CesionesSaldoInicial.Tables[0].Rows)
            {
                //El registro ya existe en la lista
                if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == row["ClienteID"].ToString().ToUpper()) != null)
                {
                    if (row["CesionID"].ToString().ToUpper() == "5CFC274E-8234-4AC5-BAA3-EA48B96E4B7E" && row["ClienteID"].ToString().ToUpper() == "19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA")
                    {
                        ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == row["ClienteID"].ToString().ToUpper()).SaldoInicial += decimal.Parse(row["Saldo"].ToString());
                    }
                        ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == row["ClienteID"].ToString().ToUpper()).SaldoInicial += decimal.Parse(row["Saldo"].ToString());
                }
                else
                {
                    ModeloConciliacionInteres item = new ModeloConciliacionInteres();
                    Entity.Analisis.Catcliente Cliente = MobileBO.ControlAnalisis.TraerCatclientes(row["ClienteID"].ToString(), null, null);
                    item.clienteid = Cliente.Clienteid.ToUpper().Trim();
                    item.codigo = Cliente.Codigo;
                    item.nombre = Cliente.Nombrecompleto;
                    if(row["CesionID"].ToString().ToUpper() == "5CFC274E-8234-4AC5-BAA3-EA48B96E4B7E" && row["ClienteID"].ToString().ToUpper() == "19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA")
                    {
                        ModeloSaldoCesion cesionant = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString().ToUpper(), fechaInicial.AddDays(0), true, true);
                        ModeloSaldoCesion cesion = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString().ToUpper(), fechaInicial.AddMonths(1), true, true);
                        item.SaldoInicial = cesionant.SaldoFinanciado;
                        item.Cargos = cesion.SaldoFinanciado - cesionant.SaldoFinanciado;
                        item.SaldoFinal = cesion.SaldoFinanciado + item.Cargos;
                    }
                    else
                    {
                        item.SaldoInicial = decimal.Parse(row["Saldo"].ToString());
                    }
                    

                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(Cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(Cliente.Codigo))
                            continue;

                    ListaModeloConciliacion.Add(item);
                }
            }

            //Obtenemos los cargos hechos en el mes
            List<Entity.Operacion.Cesionesdetalle> MovimientosDelMes = MobileBO.ControlOperacion.TraerPagosPorPeriodo(empresaid, null, 1, fechaInicial, fechaFinal);
            foreach (Entity.Operacion.Cesionesdetalle movimiento in MovimientosDelMes)
            {
                //El registro ya existe en la lista
                if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == movimiento.ClienteID.ToUpper()) != null)
                {
                    ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == movimiento.ClienteID.ToUpper()).Cargos += movimiento.Financiamiento;
                }
                else
                {
                    ModeloConciliacionInteres item = new ModeloConciliacionInteres();
                    Entity.Analisis.Catcliente Cliente = MobileBO.ControlAnalisis.TraerCatclientes(movimiento.ClienteID, null, null);
                    item.clienteid = Cliente.Clienteid.ToUpper().Trim();
                    item.codigo = Cliente.Codigo;
                    item.nombre = Cliente.Nombrecompleto;
                    item.SaldoInicial = 0m;

                    item.Cargos = movimiento.Financiamiento;

                    if(movimiento.ClienteID.ToUpper()== "19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA")
                    {
                        item.Cargos = movimiento.Financiamiento;
                    }
                    
                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(Cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(Cliente.Codigo))
                            continue;

                    ListaModeloConciliacion.Add(item);
                }
            }

            //Agregamos los movimientos de los pagos realizados en el mes
            foreach (Entity.Operacion.Cesionesdetalle pago in Pagos)
            {
                if (ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == pago.ClienteID.ToUpper()) != null)
                {
                    ListaModeloConciliacion.Find(x => x.clienteid.ToUpper() == pago.ClienteID.ToUpper()).Abonos += pago.Financiamiento;
                }
                else
                {
                    ModeloConciliacionInteres item = new ModeloConciliacionInteres();
                    Entity.Analisis.Catcliente Cliente = MobileBO.ControlAnalisis.TraerCatclientes(pago.ClienteID, null, null);
                    item.clienteid = Cliente.Clienteid.ToUpper().Trim();
                    item.codigo = Cliente.Codigo;
                    item.nombre = Cliente.Nombrecompleto;
                    item.SaldoInicial = 0m;
                    item.Cargos = 0m;
                    item.Abonos = pago.Financiamiento;

                    if (IncluirDemandados == "0" && SoloDemandados != "1")
                        if (ExclusionDemandados.ContainsKey(Cliente.Codigo))
                            continue;

                    if (SoloDemandados == "1")
                        if (!ExclusionDemandados.ContainsKey(Cliente.Codigo))

                            continue;
                    ListaModeloConciliacion.Add(item);
                }
            }

            foreach (ModeloConciliacionInteres m in ListaModeloConciliacion)
            {
                m.EnDemanda = (MobileBO.ControlOperacion.ChecaClienteMorosoValidaExiste(empresaid, m.clienteid, 1) ? 1 : 0);

                m.SaldoFinal = m.SaldoInicial + m.Cargos - m.Abonos;
            }

            return ListaModeloConciliacion;
        }
    }

    public class ControladorReporteConciliacionClientes : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaInicial = DateTime.Parse(parametros.Get("fechaInicial"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));
            string tipoReporte = parametros.Get("TipoReporte");
            string IncluirDemandados = parametros.Get("IncluirDemandados");
            string SoloDemandados = parametros.Get("SoloDemandados");
            string FormatoImprimir = parametros.Get("FormatoImprimir");

            string NomReporte = "RptConciliacionInteres";

            try
            {
                List<ModeloConciliacionInteres> ListaModeloConciliacion = new List<ModeloConciliacionInteres>();
                List<Entity.Operacion.Cesionesdetalle> Pagos = MobileBO.ControlOperacion.TraerPagosPorPeriodo(empresaid, null, 11, fechaInicial, fechaFinal);
                Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                                
                //Estos clientes son de factur y de deben excluir de los reportes ya que no estan marcados como incobrables mas sin embargo el reporte de jose luis los excluye
                Dictionary<int, int> ClietesExclusion = new Dictionary<int, int>();
                ClietesExclusion.Add(1, 1);
                ClietesExclusion.Add(4, 4);
                ClietesExclusion.Add(7, 7);
                ClietesExclusion.Add(16, 16);
                ClietesExclusion.Add(34, 34);
                ClietesExclusion.Add(67, 67);
                ClietesExclusion.Add(32, 32);
                ClietesExclusion.Add(519, 519); //Lo puse de nuevo por que me dijo lupita que no le salia en el reporte.


                 DataSet catmorosos = MobileBO.ControlOperacion.TraerClientesMorosos(empresaid, 1);
                Dictionary<int, int> ExclusionDemandados = new Dictionary<int, int>();
                foreach (DataRow r in catmorosos.Tables[0].Rows)
                {
                    ExclusionDemandados.Add(int.Parse(r["Codigo"].ToString()), int.Parse(r["Codigo"].ToString()));
                }

                if (tipoReporte == "0")
                {
                    ListaModeloConciliacion = ConciliacionClientes.ConciliacionCliente(empresaid, fechaInicial, fechaFinal, Pagos, Empresa, ClietesExclusion, IncluirDemandados, SoloDemandados, ExclusionDemandados);
                }

                if (tipoReporte == "1")
                {
                    if (Empresa.Sofom)
                        NomReporte = "rptConciliacionInteresSofom";
                    ListaModeloConciliacion = ConciliacionClientes.ConciliacionIntereses(empresaid, fechaInicial, fechaFinal, Pagos, Empresa, ClietesExclusion, IncluirDemandados, SoloDemandados, ExclusionDemandados);
                }

                if (tipoReporte == "2")
                {                   
                    NomReporte = "rptConciliacionCapitalFinanciado";
                    ListaModeloConciliacion = ConciliacionClientes.ConciliacionCapitalFinanciado(empresaid, fechaInicial, fechaFinal, Pagos, IncluirDemandados, IncluirDemandados, SoloDemandados, ExclusionDemandados);
                }

                DataTable tbConciliacion;

                if (tipoReporte == "0" || tipoReporte == "1" || tipoReporte == "2")
                {
                    tbConciliacion = ListaModeloConciliacion.OrderBy(x => x.codigo).ToDataTable();
                    tbConciliacion.TableName = "Datos";
                }
                else
                {
                    NomReporte = "rptConciliacionInteresesUnificado";
                    List<ModeloConciliacionInteres> lstClientes = new List<ModeloConciliacionInteres>();
                    List<ModeloConciliacionInteres> lstInteres = new List<ModeloConciliacionInteres>();
                    List<ModeloConciliacionInteres> lstCapital = new List<ModeloConciliacionInteres>();
                    List<ModeloConciliacionInteresCombinado> lstUnificados = new List<ModeloConciliacionInteresCombinado>();
                    ModeloConciliacionInteresCombinado com;
                    lstClientes = ConciliacionClientes.ConciliacionCliente(empresaid, fechaInicial, fechaFinal, Pagos, Empresa, ClietesExclusion, IncluirDemandados, SoloDemandados, ExclusionDemandados);
                    lstInteres = ConciliacionClientes.ConciliacionIntereses(empresaid, fechaInicial, fechaFinal, Pagos, Empresa, ClietesExclusion, IncluirDemandados, SoloDemandados, ExclusionDemandados);
                    lstCapital = ConciliacionClientes.ConciliacionCapitalFinanciado(empresaid, fechaInicial, fechaFinal, Pagos, IncluirDemandados, IncluirDemandados, SoloDemandados, ExclusionDemandados);

                    Dictionary<int, int> agregado = new Dictionary<int, int>();

                    
                    foreach (ModeloConciliacionInteres m in lstClientes)
                    {
                        agregado.Add(m.codigo, m.codigo);

                        com = new ModeloConciliacionInteresCombinado();
                        com.clienteid = m.clienteid;
                        com.codigo = m.codigo;
                        com.nombre = m.nombre;
                        com.ClienteSaldoInicial = m.SaldoInicial;
                        com.ClienteCargos = m.Cargos;
                        com.ClienteAbonos = m.Abonos;
                        com.ClienteSaldoFinal = m.SaldoFinal;
                        com.EnDemanda = m.EnDemanda;

                        lstUnificados.Add(com);
                    }

                    foreach (ModeloConciliacionInteres m in lstInteres)
                    {
                        if (agregado.ContainsKey(m.codigo))
                        {
                            int index = lstUnificados.FindIndex(x => x.codigo == m.codigo);
                            lstUnificados[index].InteresSaldoInicial = m.SaldoInicial;
                            lstUnificados[index].InteresCargos = m.Cargos;
                            lstUnificados[index].InteresAbonos = m.Abonos;
                            lstUnificados[index].InteresSaldoFinal = m.SaldoFinal;
                        }
                        else
                        {
                            agregado.Add(m.codigo, m.codigo);

                            com = new ModeloConciliacionInteresCombinado();
                            com.clienteid = m.clienteid;
                            com.codigo = m.codigo;
                            com.nombre = m.nombre;
                            com.InteresSaldoInicial = m.SaldoInicial;
                            com.InteresCargos = m.Cargos;
                            com.InteresAbonos = m.Abonos;
                            com.InteresSaldoFinal = m.SaldoFinal;
                            com.EnDemanda = m.EnDemanda;

                            lstUnificados.Add(com);
                        }
                    }

                    foreach (ModeloConciliacionInteres m in lstCapital)
                    {
                        if (agregado.ContainsKey(m.codigo))
                        {
                            int index = lstUnificados.FindIndex(x => x.codigo == m.codigo);
                            lstUnificados[index].CapitalSaldoInicial = m.SaldoInicial;
                            lstUnificados[index].CapitalCargos = m.Cargos;
                            lstUnificados[index].CapitalAbonos = m.Abonos;
                            lstUnificados[index].CapitalSaldoFinal = m.SaldoFinal;
                        }
                        else
                        {
                            agregado.Add(m.codigo, m.codigo);

                            com = new ModeloConciliacionInteresCombinado();
                            com.clienteid = m.clienteid;
                            com.codigo = m.codigo;
                            com.nombre = m.nombre;
                            com.CapitalSaldoInicial = m.SaldoInicial;
                            com.CapitalCargos = m.Cargos;
                            com.CapitalAbonos = m.Abonos;
                            com.CapitalSaldoFinal = m.SaldoFinal;
                            com.EnDemanda = m.EnDemanda;

                            lstUnificados.Add(com);
                        }
                    }
                    tbConciliacion = lstUnificados.OrderBy(x => x.codigo).ToDataTable();
                    tbConciliacion.TableName = "Datos";                
                }

                DataSet dsInteres = new DataSet();
                dsInteres.Tables.Add(tbConciliacion);

                DataSet dsEmpresas = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                dsInteres.Tables.Add(dsEmpresas.Tables[0].Copy());

                //foreach(DataRow r in dsInteres.Tables["Datos"].Rows)
                //{
                //    Entity.Analisis.Catcliente c = MobileBO.ControlAnalisis.TraerCatclientes(r["ClienteID"].ToString(), null, null);
                //    if (c.Empresaid != empresaid)
                //    {
                //        r.Delete();
                //    }
                //}

                DataTable rptParam = new DataTable();
                rptParam.Columns.Add("TipoRep", typeof(string));
                rptParam.Columns.Add("fechaInicial", typeof(DateTime));
                rptParam.Columns.Add("fechaFinal", typeof(DateTime));
                rptParam.Columns.Add("Demandado", typeof(int));
                DataRow rowParam = rptParam.NewRow();
                rowParam["fechaInicial"] = fechaInicial;
                rowParam["fechaFinal"] = fechaFinal;
                rowParam["TipoRep"] = (tipoReporte == "1" ? "INTERES" : "FACTURAS");
                rowParam["Demandado"] = int.Parse(IncluirDemandados);
                rptParam.Rows.Add(rowParam);
                dsInteres.Tables.Add(rptParam);

                base.NombreReporte = NomReporte;
                base.FormatoReporte = int.Parse(FormatoImprimir);
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = dsInteres;
                base.DataSource.WriteXml("c:\\Reportes\\RptConciliacionInteres.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }

}