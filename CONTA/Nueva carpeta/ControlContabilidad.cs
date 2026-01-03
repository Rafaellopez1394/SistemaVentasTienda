using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;
using System.Transactions;
using System.Data;
using Homex.Core.Utilities;

namespace MobileBO
{
    public class ControlContabilidad
    {
        #region Polizas Administrativas
        public bool ProcesarPolizasPendientes(int ejercicio, string empresaid)
        {
            return new MobileBO.Contabilidad.PolizaBO().ProcesarPolizasPendientes(ejercicio, empresaid);
        }
        public System.Data.DataSet TraerTipoContabilidad(string empresaid)
        {
            return new MobileBO.Contabilidad.PolizaBO().TraerTipoContabilidad(empresaid);
        }
        #endregion
        #region Saldos
        public void AfectaSaldosParaEliminarPoliza(Entity.Contabilidad.Acvgral acvgral, ref Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvmov> listaMovimientos = MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorAcvGral(acvgral.Acvgralid);

            foreach (Entity.Contabilidad.Acvmov mov in listaMovimientos)
            {
                Entity.Contabilidad.Catcuenta catcuenta = MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuenta(mov.Cuenta, mov.EmpresaId);
                ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosTmp = new ListaDeEntidades<Entity.Contabilidad.Saldo>();

                string ejercicio = mov.FecPol.Year.ToString();
                int mes = mov.FecPol.Month;
                decimal importe = mov.Importe;
                int nivel = catcuenta.Nivel;
                string cuenta = mov.Cuenta;

                for (int i = 1; i <= nivel; i++)
                {
                    Entity.Contabilidad.Saldo saldo;
                    cuenta = mov.Cuenta.Substring(0, 4 * i).PadRight(24 - (4 * i), '0');
                    saldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(cuenta, ejercicio, acvgral.EmpresaId);
                    if (saldo != null)
                    {
                        listaSaldosTmp.Add(saldo);
                        if (!dictSaldos.ContainsKey(saldo.Cuentaid))
                        {
                            dictSaldos.Add(saldo.Cuentaid, saldo);
                        }
                    }
                }

                if (mov.TipMov == Entity.TipMov.Cargo.GetHashCode().ToString())
                {
                    // Saldo cargos
                    foreach (Entity.Contabilidad.Saldo saldoCargo in listaSaldosTmp)
                    {
                        switch (mes)
                        {
                            case 1:
                                dictSaldos[saldoCargo.Cuentaid].Car1 -= importe;
                                break;
                            case 2:
                                dictSaldos[saldoCargo.Cuentaid].Car2 -= importe;
                                break;
                            case 3:
                                dictSaldos[saldoCargo.Cuentaid].Car3 -= importe;
                                break;
                            case 4:
                                dictSaldos[saldoCargo.Cuentaid].Car4 -= importe;
                                break;
                            case 5:
                                dictSaldos[saldoCargo.Cuentaid].Car5 -= importe;
                                break;
                            case 6:
                                dictSaldos[saldoCargo.Cuentaid].Car6 -= importe;
                                break;
                            case 7:
                                dictSaldos[saldoCargo.Cuentaid].Car7 -= importe;
                                break;
                            case 8:
                                dictSaldos[saldoCargo.Cuentaid].Car8 -= importe;
                                break;
                            case 9:
                                dictSaldos[saldoCargo.Cuentaid].Car9 -= importe;
                                break;
                            case 10:
                                dictSaldos[saldoCargo.Cuentaid].Car10 -= importe;
                                break;
                            case 11:
                                dictSaldos[saldoCargo.Cuentaid].Car11 -= importe;
                                break;
                            case 12:
                                dictSaldos[saldoCargo.Cuentaid].Car12 -= importe;
                                break;
                        }
                    }
                }
                else
                {
                    // Saldo abonos
                    foreach (Entity.Contabilidad.Saldo saldoAbono in listaSaldosTmp)
                    {
                        switch (mes)
                        {
                            case 1:
                                dictSaldos[saldoAbono.Cuentaid].Abo1 -= importe;
                                break;
                            case 2:
                                dictSaldos[saldoAbono.Cuentaid].Abo2 -= importe;
                                break;
                            case 3:
                                dictSaldos[saldoAbono.Cuentaid].Abo3 -= importe;
                                break;
                            case 4:
                                dictSaldos[saldoAbono.Cuentaid].Abo4 -= importe;
                                break;
                            case 5:
                                dictSaldos[saldoAbono.Cuentaid].Abo5 -= importe;
                                break;
                            case 6:
                                dictSaldos[saldoAbono.Cuentaid].Abo6 -= importe;
                                break;
                            case 7:
                                dictSaldos[saldoAbono.Cuentaid].Abo7 -= importe;
                                break;
                            case 8:
                                dictSaldos[saldoAbono.Cuentaid].Abo8 -= importe;
                                break;
                            case 9:
                                dictSaldos[saldoAbono.Cuentaid].Abo9 -= importe;
                                break;
                            case 10:
                                dictSaldos[saldoAbono.Cuentaid].Abo10 -= importe;
                                break;
                            case 11:
                                dictSaldos[saldoAbono.Cuentaid].Abo11 -= importe;
                                break;
                            case 12:
                                dictSaldos[saldoAbono.Cuentaid].Abo12 -= importe;
                                break;
                        }
                    }
                }
            }
        }

        public System.Data.DataSet GeneraInformeSaldos(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, string cuentainicial, string cuentafinal, string nivel, int Ingles, int FormatoCuenta, bool ExcluirDemandados)
        {
            return new MobileBO.Contabilidad.SaldoBO().GeneraInformeSaldos(fecha1, fecha2, codempresainicial, codempresafinal, cuentainicial, cuentafinal, nivel, Ingles, FormatoCuenta, ExcluirDemandados);
        }

        public System.Data.DataSet GeneraInformeAuxiliarMayor(DateTime fecha1, DateTime fecha2, string codempresainicial, string cuentainicial, string cuentafinal, int Ingles)
        {
            return new MobileBO.Contabilidad.SaldoBO().GeneraInformeAuxiliarMayor(fecha1, fecha2, codempresainicial, cuentainicial, cuentafinal, Ingles);
        }
        public System.Data.DataSet GeneraInformeBalanceGeneral(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            return MobileBO.Contabilidad.SaldoBO.GeneraInformeBalanceGeneral(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
        }
        public System.Data.DataSet GeneraInformeEstadoResultados(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            return MobileBO.Contabilidad.SaldoBO.GeneraInformeEstadoResultados(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
        }
        public System.Data.DataSet GeneraInformeIvaAcreditable(DateTime fecha1, DateTime fecha2, string codempresa)
        {
            return new MobileBO.Contabilidad.SaldoBO().GeneraInformeIvaAcreditable(fecha1, fecha2, codempresa);
        }

        public static List<Entity.ModeloSaldoBancos> TraerSaldoEnBancos(string empresaid, DateTime fecha)
        {
            List<Entity.ModeloSaldoBancos> lst = new List<ModeloSaldoBancos>();
            List<Entity.Operacion.Catempresasbanco> lstCuentas = MobileBO.ControlOperacion.TraerCatempresasbancosPorEmpresa(empresaid);

            foreach (var datoscuenta in lstCuentas)
            {
                Entity.Contabilidad.Saldo datosSaldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(datoscuenta.Cuenta, fecha.Year.ToString(), empresaid);

                DataSet dsMov = MobileDAL.Contabilidad.Acvmov.TraerAcvMovEmpresaCuentaFecha(empresaid, fecha.Year, datoscuenta.Cuenta);

                var ca = from a in dsMov.Tables[0].AsEnumerable()
                         where a.Field<DateTime>("Fec_Pol") <= fecha
                         group a by new
                         {
                             tipo = a.Field<string>("Tip_Mov"),
                             importe = a.Field<decimal>("Importe"),
                         } into CarAbo
                         select new { Tipo = CarAbo.Key.tipo, Importe = CarAbo.Key.importe };

                Entity.ModeloSaldoBancos modelo = new ModeloSaldoBancos();

                modelo.EmpresaID = empresaid;
                modelo.Cuenta = datoscuenta.Cuenta;
                modelo.Saldo = datosSaldo.Sdoini + ca.Where(x => x.Tipo == "1").Sum(y => y.Importe) - ca.Where(x => x.Tipo == "2").Sum(y => y.Importe);
                lst.Add(modelo);
            }

            return lst;
        }

        public static DataSet TraerSaldosCuentasBancarias(string EmpresaID, DateTime Fecha)
        {
            return MobileBO.Contabilidad.SaldoBO.TraerSaldosCuentasBancarias(EmpresaID, Fecha);
        }
        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            return MobileBO.Contabilidad.SaldoBO.TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(empresaid, ejercicio, cuentaInicio, cuentaFinal);
        }

        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicio(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            return MobileBO.Contabilidad.SaldoBO.TraerSaldosPorRangodeCuentaEjercicio(empresaid, ejercicio, cuentaInicio, cuentaFinal);
        }
        public System.Data.DataSet spcgenerainformebalancegeneralDolares(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal)
        {
            return MobileBO.Contabilidad.SaldoBO.spcgenerainformebalancegeneralDolares(fecha1, fecha2, codempresainicial, codempresafinal);
        }
        #endregion

        #region Polizas

        public void GuardarPolizaCierre(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            new MobileBO.Contabilidad.PolizaBO().GuardarPolizaCierre(listaPolizas);
        }

        public void GuardarPoliza(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            new MobileBO.Contabilidad.PolizaBO().GuardarPoliza(listaPolizas);
        }
        public void GuardarPolizaC(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            new MobileBO.Contabilidad.PolizaBO().GuardarPolizaC(listaPolizas);
        }
        public void GuardarPolizaFactoraje(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            new MobileBO.Contabilidad.PolizaBO().GuardarPolizaFactoraje(listaPolizas);
        }
        public void ReprosesarSaldos(string EmpresaID)
        {
            new MobileBO.Contabilidad.SaldoBO().ReprosesarSaldos(EmpresaID);
        }
        public void GuardarPolizaContable(ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral)
        {
            //Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos = new Dictionary<string, Entity.Contabilidad.Saldo>();
            ListaDeEntidades<Entity.Contabilidad.Acvgral> ListaacvgralBorrar = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();

            foreach (Entity.Contabilidad.Acvgral elemento in listaAcvGral)
            {
                // Borrar poliza anterior
                Entity.Contabilidad.Acvgral acvgralBorrar = MobileDAL.Contabilidad.Acvgral.TraerAcvgralPorReferenciaId(elemento.ReferenciaId);
                if (acvgralBorrar != null)
                {
                    // Afectar Saldos
                    //AfectaSaldosParaEliminarPoliza(acvgralBorrar, ref dictSaldos);
                    ListaacvgralBorrar.Add(acvgralBorrar);
                }

                //foreach (Entity.Contabilidad.Acvmov mov in elemento.ListaAcvmov)
                //{
                //    ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosTmp = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                //    Entity.Contabilidad.Catcuenta catcuenta = MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuenta(mov.Cuenta, mov.EmpresaId);
                //    string ejercicio = mov.FecPol.Year.ToString();
                //    int mes = mov.FecPol.Month;
                //    decimal importe = mov.Importe;
                //    int nivel = catcuenta.Nivel;
                //    string cuenta = mov.Cuenta;

                //    for (int i = 1; i <= nivel; i++)
                //    {
                //        Entity.Contabilidad.Saldo saldo;
                //        cuenta = mov.Cuenta.Substring(0, 4 * i).PadRight(24 - (4 * i), '0');
                //        saldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(cuenta, ejercicio, elemento.EmpresaId);
                //        if (saldo != null)
                //        {
                //            listaSaldosTmp.Add(saldo);
                //            if (!dictSaldos.ContainsKey(saldo.Cuentaid))
                //            {
                //                dictSaldos.Add(saldo.Cuentaid, saldo);
                //            }

                //        }
                //        else
                //        {
                //            cuenta = mov.Cuenta.Substring(0, 4 * i).PadRight(24, '0');
                //            Entity.Contabilidad.Catcuenta CuentaContable = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuenta, mov.EmpresaId);
                //            saldo = new Entity.Contabilidad.Saldo();
                //            saldo.Saldoid = Guid.Empty.ToString();
                //            saldo.EmpresaId = elemento.EmpresaId;
                //            saldo.CodEmpresa = elemento.CodEmpresa;
                //            saldo.Ejercicio = ejercicio;
                //            saldo.Cuentaid = CuentaContable.Cuentaid;
                //            saldo.Cuenta = CuentaContable.Cuenta;
                //            saldo.Nivel = CuentaContable.Nivel;
                //            listaSaldosTmp.Add(saldo);
                //            if (!dictSaldos.ContainsKey(saldo.Cuentaid))
                //            {
                //                dictSaldos.Add(saldo.Cuentaid, saldo);
                //            }
                //        }
                //    }
                //    if (mov.TipMov == Entity.TipMov.Cargo.GetHashCode().ToString())
                //    {
                //        // Saldo cargos
                //        foreach (Entity.Contabilidad.Saldo saldoCargo in listaSaldosTmp)
                //        {
                //            switch (mes)
                //            {
                //                case 1:
                //                    dictSaldos[saldoCargo.Cuentaid].Car1 += importe;
                //                    break;
                //                case 2:
                //                    dictSaldos[saldoCargo.Cuentaid].Car2 += importe;
                //                    break;
                //                case 3:
                //                    dictSaldos[saldoCargo.Cuentaid].Car3 += importe;
                //                    break;
                //                case 4:
                //                    dictSaldos[saldoCargo.Cuentaid].Car4 += importe;
                //                    break;
                //                case 5:
                //                    dictSaldos[saldoCargo.Cuentaid].Car5 += importe;
                //                    break;
                //                case 6:
                //                    dictSaldos[saldoCargo.Cuentaid].Car6 += importe;
                //                    break;
                //                case 7:
                //                    dictSaldos[saldoCargo.Cuentaid].Car7 += importe;
                //                    break;
                //                case 8:
                //                    dictSaldos[saldoCargo.Cuentaid].Car8 += importe;
                //                    break;
                //                case 9:
                //                    dictSaldos[saldoCargo.Cuentaid].Car9 += importe;
                //                    break;
                //                case 10:
                //                    dictSaldos[saldoCargo.Cuentaid].Car10 += importe;
                //                    break;
                //                case 11:
                //                    dictSaldos[saldoCargo.Cuentaid].Car11 += importe;
                //                    break;
                //                case 12:
                //                    dictSaldos[saldoCargo.Cuentaid].Car12 += importe;
                //                    break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // Saldo abonos
                //        foreach (Entity.Contabilidad.Saldo saldoAbono in listaSaldosTmp)
                //        {
                //            switch (mes)
                //            {
                //                case 1:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo1 += importe;
                //                    break;
                //                case 2:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo2 += importe;
                //                    break;
                //                case 3:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo3 += importe;
                //                    break;
                //                case 4:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo4 += importe;
                //                    break;
                //                case 5:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo5 += importe;
                //                    break;
                //                case 6:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo6 += importe;
                //                    break;
                //                case 7:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo7 += importe;
                //                    break;
                //                case 8:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo8 += importe;
                //                    break;
                //                case 9:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo9 += importe;
                //                    break;
                //                case 10:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo10 += importe;
                //                    break;
                //                case 11:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo11 += importe;
                //                    break;
                //                case 12:
                //                    dictSaldos[saldoAbono.Cuentaid].Abo12 += importe;
                //                    break;
                //            }
                //        }
                //    }
                //}
            }
            ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosDict = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
            //foreach (Entity.Contabilidad.Saldo s in dictSaldos.Values)
            //{
            //    listaSaldosDict.Add(s);
            //}
            MobileDAL.Contabilidad.Acvgral.GuardarPoliza(ref listaAcvGral, ListaacvgralBorrar, ref listaSaldosDict);
        }

        public Entity.Contabilidad.Poliza TraerPolizaPorFolio(string folio, string tippol, string empresaid, DateTime fechapol)
        {
            return new MobileBO.Contabilidad.PolizaBO().TraerPolizaPorFolio(folio, tippol, empresaid, fechapol);
        }
        public ListaDeEntidades<Entity.Contabilidad.Poliza> TraerPolizasPorDescripcion(string descripcion, string tippol, string empresaid, DateTime fechapol, bool Pendiente)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasPorDescripcion(descripcion, tippol, empresaid, fechapol, Pendiente);
        }
        public ListaDeEntidades<Entity.Contabilidad.Poliza> TraerPolizasGenerales(string tippol, string empresaid, DateTime fechapol,string folio, bool Pendiente)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasGenerales(tippol, empresaid, fechapol,folio, Pendiente);
        }
        public ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> TraerPolizaDetalles(string polizaid)
        {
            return MobileDAL.Contabilidad.Polizasdetalle.TraerPolizasdetallePorPoliza(polizaid);
        }
        
        public Entity.Contabilidad.Poliza TraerPolizas(string polizaid)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizas(polizaid);
        }

        public static List<Entity.Contabilidad.Poliza> TraerPolizasPorFiltros(string EmpresaID, int? año, int? mes, bool? pendiente, string tip_pol)
        {
            return new MobileBO.Contabilidad.PolizaBO().TraerPolizasPorFiltros(EmpresaID, año, mes, pendiente, tip_pol);
        }

        public System.Data.DataSet TraerDatosReportePolizas(string polizaid, int Ingles)
        {
            return new MobileBO.Contabilidad.PolizaBO().TraerDatosReportePolizas(polizaid, Ingles);
        }

        public static System.Data.DataSet ReporteCapturaPolizasMasivo(string EmpresaID, DateTime FechaInicial, DateTime FechaFinal, string TipPol, int Ingles, string folioInicial, string folioFinal)
        {
            return MobileBO.Contabilidad.PolizaBO.ReporteCapturaPolizasMasivo(EmpresaID, FechaInicial, FechaFinal, TipPol, Ingles, folioInicial, folioFinal);
        }

        #endregion

        #region CatCuentas

        public static System.Data.DataSet TraerPrimeraCuentaContable(string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerPrimeraCuentaContable(empresaid);
        }
        public static System.Data.DataSet TraerCuentaAfecta(string empresaid, string cuenta)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCuentaAfecta(empresaid, cuenta);
        }

        public static System.Data.DataSet TraerUltimaCuentaContable(string empresaid, string Cuenta)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerUltimaCuentaContable(empresaid, Cuenta);
        }

        public void GuardarCatCuenta(ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas)
        {
            MobileBO.Contabilidad.CatcuentaBO.GuardarCatcuenta(listaCatCuentas);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta, string empresaid)
        {
            return new MobileBO.Contabilidad.CatcuentaBO().TraerCatCuentasPorCuenta(cuenta, empresaid);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta, string nivel1, string nivel2, string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorCuenta(cuenta, nivel1, nivel2, empresaid);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaAfectable(string cuenta, string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorCuentaAfectable(cuenta, empresaid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcionAfectable(string descripcion, string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorDescripcionAfectable(descripcion, empresaid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion, string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorDescripcion(descripcion, empresaid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion, string nivel1, string nivel2, string empresaid)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorDescripcion(descripcion, nivel1, nivel2, empresaid);
        }

        public System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas> TraerDatosCuentaContable(string cuentaid)
        {
            return new MobileBO.Contabilidad.CatcuentaBO().TraerDatosCuentaContable(cuentaid);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentas(string cuentaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentas(cuentaid);
        }

        public System.Data.DataSet TraerFlujos(string flujo, string descripcion)
        {
            return new MobileBO.Contabilidad.CatcuentaBO().TraerFlujos(flujo, descripcion);
        }

        public static System.Data.DataSet spcgenerainformedetallecuentas(string empresa)
        {
            return MobileBO.Contabilidad.CatcuentaBO.spcgenerainformedetallecuentas(empresa);
        }

        public static System.Data.DataSet TraerCatCuentasSat(string empresaid, DateTime FechaCorte)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasSat(empresaid, FechaCorte);
        }

        public static bool ValidaCuentasFiscales(string EmpresaID, int Anio)
        {
            return MobileBO.Contabilidad.CatcuentaBO.ValidaCuentasFiscales(EmpresaID, Anio);
        }

        public static System.Data.DataSet TraerCatCuentasPorEjercicio(string EmpresaID, int Anio)
        {
            return MobileBO.Contabilidad.CatcuentaBO.TraerCatCuentasPorEjercicio(EmpresaID, Anio);
        }

        public static string ValidaCtaSATPorEmpresaCuentaNivel(string EmpresaID, string Cuenta, int Nivel)
        {
            return MobileBO.Contabilidad.CatcuentaBO.ValidaCtaSATPorEmpresaCuentaNivel(EmpresaID, Cuenta, Nivel);
        }
        #endregion

        #region Acvtip
        public ListaDeEntidades<Entity.Contabilidad.Acvtip> TraerAcvtip()
        {
            return new MobileBO.Contabilidad.AcvtipBO().TraerAcvtip();
        }
        #endregion

        #region AcvGralPdte
        public bool PuedeProcesar(int ejercicio, string empresaid)
        {
            return new MobileBO.Contabilidad.AcvgralpdteBO().PuedeProcesar(ejercicio, empresaid);
        }
        #endregion

        #region AcvCtaM
        public void GuardarAcvctam(ListaDeEntidades<Entity.Contabilidad.Acvctam> listaAcvctam)
        {
            new MobileBO.Contabilidad.AcvctamBO().GuardarAcvctam(listaAcvctam);
        }

        public static Entity.Contabilidad.Acvctam TraerAcvctamPorCuenta(string cuenta, string empresaid)
        {
            return new MobileBO.Contabilidad.AcvctamBO().TraerAcvctamPorCuenta(cuenta, empresaid);
        }
        #endregion

        #region MoctoBco
        public static System.Data.DataSet TraerMovimientosContablesBancarios(string EmpresaID, string BancoID)
        {
            return MobileBO.Contabilidad.MvtobcoBO.TraerMovimientosContablesBancarios(EmpresaID, BancoID);
        }

        public static System.Data.DataSet TraerMvtosBancosPorEmpresa(string EmpresaID)
        {
            return MobileBO.Contabilidad.MvtobcoBO.TraerMvtosBancosPorEmpresa(EmpresaID);
        }
        #endregion

        #region AcvGral
        public static Entity.Contabilidad.Acvgral TraerAcvgral(string acvgralid)
        {
            return new MobileBO.Contabilidad.AcvgralBO().TraerAcvgral(acvgralid);
        }

        public static Entity.Contabilidad.Acvgral TraerAcvgralPorReferenciaId(string referenciaid)
        {
            return MobileBO.Contabilidad.AcvgralBO.TraerAcvgralPorReferenciaId(referenciaid);
        }

        public static void BorrarAcvgralPorId(string acvgralid)
        {
            MobileDAL.Contabilidad.Acvgral.BorrarPorId(acvgralid);
        }

        public static void IntercambiaPolizasContablesFiscales(string acvgralid, string acvgralpdteid)
        {
            MobileDAL.Contabilidad.Acvgral.IntercambiaPolizasContablesFiscales(acvgralid, acvgralpdteid);
        }
        #endregion

        #region AcvMov
        public static ListaDeEntidades<Entity.Contabilidad.Acvmov> TraerAcvmovPorAcvGral(string acvgralid)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerAcvmovPorAcvGral(acvgralid);
        }

        public static System.Data.DataSet TraerFolioMaximoPorTipoPoliza(string tippol, int empresa, DateTime fecha)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerFolioMaximoPorTipoPoliza(tippol, empresa, fecha);
        }

        public static System.Data.DataSet VerificarFolioPoliza(string NumPol, string TipPol, DateTime FecPol, string EmpresaID, bool Pendiente)
        {
            return MobileBO.Contabilidad.AcvmovBO.VerificarFolioPoliza(NumPol, TipPol, FecPol, EmpresaID, Pendiente);
        }

        public static System.Data.DataSet TraerGatosDelReporteCostos(DateTime fechainicio, DateTime fechafin, string empresaid)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerGatosDelReporteCostos(fechainicio, fechafin, empresaid);
        }

        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCuentaYReferencia(string cuenta, string refer)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerAcvmovPorCuentaYReferencia(cuenta, refer);
        }

        public static System.Data.DataSet ReporteContabilidadExcelDS(string empresaid, DateTime fechainicio, DateTime fechafin)
        {
            return MobileBO.Contabilidad.AcvmovBO.ReporteContabilidadExcelDS(empresaid, fechainicio, fechafin);
        }
        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCesion(string concepto, string cuenta, string empresaid)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerAcvmovPorCesion(concepto, cuenta, empresaid);
        }

        #endregion

        #region AcvGralPdte
        public static Entity.Contabilidad.Acvgralpdte TraerAcvgralpdtePorReferenciaId(string referenciaid)
        {
            return MobileBO.Contabilidad.AcvgralpdteBO.TraerAcvgralpdtePorReferenciaId(referenciaid);
        }
        #endregion

        #region AcvMovPdte
        public static ListaDeEntidades<Entity.Contabilidad.Acvpdte> TraerAcvpdtePorAcvGralPdte(string acvgralid)
        {
            return MobileBO.Contabilidad.AcvpdteBO.TraerAcvpdtePorAcvGralPdte(acvgralid);
        }
        #endregion

        #region Catcierre

        public static void GuardarCatcierre(ListaDeEntidades<Entity.Contabilidad.Catcierre> listaCatcierre)
        {
            MobileBO.Contabilidad.CatcierreBO.GuardarCatcierre(listaCatcierre);
        }

        public static Entity.Contabilidad.Catcierre TraerCatcierre(string cierreid,string empresaid,DateTime fecha)
        {
            return MobileBO.Contabilidad.CatcierreBO.TraerCatcierre(cierreid, empresaid, fecha);
        }

        public static System.Data.DataSet TraerUltimoCierre(string EmpresaID)
        {
            return MobileBO.Contabilidad.CatcierreBO.TraerUltimoCierre(EmpresaID);
        }
        #endregion

        #region CatalogoCtaSat
        public static void GuardarCatalogocuentasat(List<Entity.Contabilidad.Catalogocuentasat> listaCatalogocuentasat)
        {
            MobileBO.Contabilidad.CatalogocuentasatBO.GuardarCatalogocuentasat(listaCatalogocuentasat);
        }

        public static Entity.Contabilidad.Catalogocuentasat TraerCatalogocuentasat(string ctasat)
        {
            return MobileBO.Contabilidad.CatalogocuentasatBO.TraerCatalogocuentasat(ctasat);
        }

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasat()
        {
            return MobileBO.Contabilidad.CatalogocuentasatBO.TraerCatalogocuentasat();
        }

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasatPorDescripcion(string descripcion)
        {
            return MobileBO.Contabilidad.CatalogocuentasatBO.TraerCatalogocuentasatPorDescripcion(descripcion);
        }
        #endregion

        #region Compras
        public static void GuardarCompra(List<Entity.Contabilidad.Compra> listaCompra)
        {
            MobileBO.Contabilidad.CompraBO.GuardarCompra(listaCompra);
        }

        public static Entity.Contabilidad.Compra TraerCompras(string compraid)
        {
            return MobileBO.Contabilidad.CompraBO.TraerCompras(compraid);
        }

        public static List<Entity.Contabilidad.Compra> TraerCompras()
        {
            return MobileBO.Contabilidad.CompraBO.TraerCompras();
        }

        public static System.Data.DataSet TraerComprasDS()
        {
            return MobileBO.Contabilidad.CompraBO.TraerComprasDS();
        }
        public static Entity.Contabilidad.Compra TraerComprasPorCodigo(string empresaid, int codigo)
        {
            return MobileBO.Contabilidad.CompraBO.TraerComprasPorCodigo(empresaid, codigo);
        }
        public static List<Entity.Contabilidad.Compra> TraerComprasPorProveedor(string empresaid, string nombreproveedor)
        {
            return MobileBO.Contabilidad.CompraBO.TraerComprasPorProveedor(empresaid, nombreproveedor);
        }
        #endregion

        #region CatProveedores
        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCodigo(int Codigo, string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerCatproveedoresPorCodigo(Codigo, EmpresaID);
        }

        public static List<Entity.Contabilidad.Catproveedor> TraerCatproveedoresPorNombre(string nombre, string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerCatproveedoresPorNombre(nombre, EmpresaID);
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedores(string proveedorid, string rfc, string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerCatproveedores(proveedorid, rfc, EmpresaID);
        }

        public static int TraerSiguienteCodicoProveedor(string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerSiguienteCodicoProveedor(EmpresaID);
        }

        public static void GuardarCatproveedor(List<Entity.Contabilidad.Catproveedor> listaCatproveedor)
        {
            MobileBO.Contabilidad.CatproveedorBO.GuardarCatproveedor(listaCatproveedor);
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCuentaContable(string EmpresaID, string Cuentacontable)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerCatproveedoresPorCuentaContable(EmpresaID, Cuentacontable);
        }
        #endregion

        #region CatFacturasProveedor
        public static void Catfacturasproveedor_Delete(string CompraID)
        {
            MobileBO.Contabilidad.CatfacturasproveedorBO.Catfacturasproveedor_Delete(CompraID);
        }
        public static void GuardarCatfacturasproveedor(List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor)
        {
            MobileBO.Contabilidad.CatfacturasproveedorBO.GuardarCatfacturasproveedor(listaCatfacturasproveedor);
        }

        public static Entity.Contabilidad.Catfacturasproveedor TraerCatfacturasproveedor(string facturaproveedorid, string uuid)
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasproveedor(facturaproveedorid, uuid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor()
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasproveedor();
        }
        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedorPorCompraID(string CompraID)
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasproveedorPorCompraID(CompraID);
        }

        public static System.Data.DataSet TraerCatfacturasproveedorDS()
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasproveedorDS();
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasPorProveedorEmpresa(string proveedorid, string Empresaid)
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasPorProveedorEmpresa(proveedorid, Empresaid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor(string facturaproveedorid, string uuid, string proveedorid, string empresaid, string emisorrfc, string receptorrfc, string usocfdi, DateTime? fechainicial, DateTime? fechafinal)
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.TraerCatfacturasproveedor(facturaproveedorid, uuid, proveedorid, empresaid, emisorrfc, receptorrfc, usocfdi, fechainicial, fechafinal);
        }

        #endregion

        #region CatFacturasProveedorDet
        public static void GuardarCatfacturasproveedordet(List<Entity.Contabilidad.Catfacturasproveedordet> listaCatfacturasproveedordet)
        {
            MobileBO.Contabilidad.CatfacturasproveedordetBO.GuardarCatfacturasproveedordet(listaCatfacturasproveedordet);
        }

        public static Entity.Contabilidad.Catfacturasproveedordet TraerCatfacturasproveedordet(string facturaproveedordetid)
        {
            return MobileBO.Contabilidad.CatfacturasproveedordetBO.TraerCatfacturasproveedordet(facturaproveedordetid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedordet> TraerCatfacturasproveedordet()
        {
            return MobileBO.Contabilidad.CatfacturasproveedordetBO.TraerCatfacturasproveedordet();
        }

        public static System.Data.DataSet TraerCatfacturasproveedordetDS()
        {
            return MobileBO.Contabilidad.CatfacturasproveedordetBO.TraerCatfacturasproveedordetDS();
        }

        public static void EliminarCatfacturasproveedordet(string FacturaProveedorID)
        {
            MobileBO.Contabilidad.CatfacturasproveedordetBO.Eliminar(FacturaProveedorID);
        }
        #endregion

        #region CatClientesDiversos
        public static void GuardarCatclientesdiverso(List<Entity.Contabilidad.Catclientesdiverso> listaCatclientesdiverso)
        {
            MobileBO.Contabilidad.CatclientesdiversoBO.GuardarCatclientesdiverso(listaCatclientesdiverso);
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversos(string clientediversoid, string EmpresaID, int? Codigo)
        {
            return MobileBO.Contabilidad.CatclientesdiversoBO.TraerCatclientesdiversos(clientediversoid, EmpresaID, Codigo);
        }

        public static List<Entity.Contabilidad.Catclientesdiverso> TraerCatclientesdiversos(string EmpresaID, string Descripcion)
        {
            return MobileBO.Contabilidad.CatclientesdiversoBO.TraerCatclientesdiversos(EmpresaID, Descripcion);
        }

        public static System.Data.DataSet TraerCatclientesdiversosDS()
        {
            return MobileBO.Contabilidad.CatclientesdiversoBO.TraerCatclientesdiversosDS();
        }

        public static System.Data.DataSet TraerDireccionCompletaCteDiv(string ClienteDiversoID)
        {
            return MobileBO.Contabilidad.CatclientesdiversoBO.TraerDireccionCompletaCteDiv(ClienteDiversoID);
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversosPorRFC(string RFC, string EmpresaID)
        {
            return MobileBO.Contabilidad.CatclientesdiversoBO.TraerCatclientesdiversosPorRFC(RFC, EmpresaID);
        }
        #endregion

        #region CreditosFinancieros
        public static void GuardarCreditosfinanciero(List<Entity.Contabilidad.Creditosfinanciero> listaCreditosfinanciero)
        {
            MobileBO.Contabilidad.CreditosfinancieroBO.GuardarCreditosfinanciero(listaCreditosfinanciero);
        }

        public static Entity.Contabilidad.Creditosfinanciero TraerCreditosfinancieros(int creditofinancieroid)
        {
            return MobileBO.Contabilidad.CreditosfinancieroBO.TraerCreditosfinancieros(creditofinancieroid);
        }

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancieros()
        {
            return MobileBO.Contabilidad.CreditosfinancieroBO.TraerCreditosfinancieros();
        }

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancierosPorFecha(DateTime FechaFinal)
        {
            return MobileBO.Contabilidad.CreditosfinancieroBO.TraerCreditosfinancierosPorFecha(FechaFinal);
        }

        public static System.Data.DataSet TraerCreditosfinancierosDS()
        {
            return MobileBO.Contabilidad.CreditosfinancieroBO.TraerCreditosfinancierosDS();
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> Creditosfinancierosdetalle_SelectTipoFecha(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            return MobileBO.Contabilidad.CreditosfinancierosdetalleBO.Creditosfinancierosdetalle_SelectTipoFecha(CreditoFinancieroID, Tipo_Mov, Fecha);
        }
        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> TraerDetalleCreditosFinancierosContabilidad(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            return MobileBO.Contabilidad.CreditosfinancierosdetalleBO.TraerDetalleCreditosFinancierosContabilidad(CreditoFinancieroID, Tipo_Mov, Fecha);
        }
        #endregion

        #region CatTipoCambio
        public static void GuardarCattipocambio(List<Entity.Contabilidad.Cattipocambio> listaCattipocambio)
        {
            MobileDAL.Contabilidad.Cattipocambio.Guardar(ref listaCattipocambio);
        }

        public static void GuardarCattipocambioBM(List<Entity.Contabilidad.Cattipocambio> listaCattipocambio)
        {
            MobileDAL.Contabilidad.Cattipocambio.GuardarBM(ref listaCattipocambio);
        }

        public static Entity.Contabilidad.Cattipocambio TraerCattipocambio(DateTime fechatipocambio)
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambio(fechatipocambio);
        }

        public static List<Entity.Contabilidad.Cattipocambio> TraerCattipocambio()
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambio();
        }

        public static System.Data.DataSet TraerCattipocambioDS()
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambioDS();
        }

        public static System.Data.DataSet ReporteEfectoCambiarioDS(int? CodEmpresa, DateTime Fecha)
        {
            return MobileDAL.Contabilidad.Cattipocambio.ReporteEfectoCambiarioDS(CodEmpresa, Fecha);
        }

        public static List<Entity.Contabilidad.Ctlregistroefectocambiario> ReporteEfectoCambiario(int? CodEmpresa, DateTime Fecha)
        {
            return MobileDAL.Contabilidad.Cattipocambio.ReporteEfectoCambiario(CodEmpresa, Fecha);
        }

        #endregion

        #region CatCetesTiie
        public static List<Entity.Contabilidad.Catcetestiie> TraerCatcetestiiePorAnio(int Anio)
        {
            return MobileBO.Contabilidad.CatcetestiieBO.TraerCatcetestiiePorAnio(Anio);
        }
        public static Entity.Contabilidad.Catcetestiie TraerCatcetestiie(int? cetetiieid, int? año, int? mes)
        {
            return MobileBO.Contabilidad.CatcetestiieBO.TraerCatcetestiie(cetetiieid, año, mes);
        }

        public static void GuardarCatcetestiie(List<Entity.Contabilidad.Catcetestiie> listaCatcetestiie)
        {
            MobileBO.Contabilidad.CatcetestiieBO.GuardarCatcetestiie(listaCatcetestiie);
        }
        #endregion

        #region Catclientesfilial
        public static void GuardarCatclientesfilial(List<Entity.Contabilidad.Catclientesfilial> listaCatclientesfilial)
        {
            MobileBO.Contabilidad.CatclientesfilialBO.GuardarCatclientesfilial(listaCatclientesfilial);
        }

        public static Entity.Contabilidad.Catclientesfilial TraerCatclientesfilial(string clienteid, string empresaid, int? Codigo)
        {
            return MobileBO.Contabilidad.CatclientesfilialBO.TraerCatclientesfilial(clienteid, empresaid, Codigo);
        }

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilial()
        {
            return MobileBO.Contabilidad.CatclientesfilialBO.TraerCatclientesfilial();
        }

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilialPorNombre(string nombre, string empresaid)
        {
            return MobileBO.Contabilidad.CatclientesfilialBO.TraerCatclientesfilialPorNombre(nombre, empresaid);
        }

        public static System.Data.DataSet TraerCatclientesfilialDS()
        {
            return MobileBO.Contabilidad.CatclientesfilialBO.TraerCatclientesfilialDS();
        }

        #endregion //Métodos Públicos


        #region  Creditosfinancierostasasind
        public static void GuardarCreditosfinancierostasasind(ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> listaCreditosfinancierostasasind)
        {
            MobileBO.Contabilidad.CreditosfinancierostasasindBO.GuardarCreditosfinancierostasasind(listaCreditosfinancierostasasind);
        }

        public static Entity.Contabilidad.Creditosfinancierostasasind TraerCreditosfinancierostasasind(string creditosfinancierostasatiieid, int? CreditoFinancieroID, int? Anio, int? Mes)
        {
            return MobileBO.Contabilidad.CreditosfinancierostasasindBO.TraerCreditosfinancierostasasind(creditosfinancierostasatiieid, CreditoFinancieroID, Anio, Mes);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> TraerCreditosfinancierostasasind()
        {
            return MobileBO.Contabilidad.CreditosfinancierostasasindBO.TraerCreditosfinancierostasasind();
        }

        public static System.Data.DataSet TraerCreditosfinancierostasasindDS()
        {
            return MobileBO.Contabilidad.CreditosfinancierostasasindBO.TraerCreditosfinancierostasasindDS();
        }
        #endregion //Métodos Públicos


        #region Bancos Creditos
        public static void GuardarCatbancoscredito(List<Entity.Contabilidad.Catbancoscredito> listaCatbancoscredito)
        {
            MobileBO.Contabilidad.CatbancoscreditoBO.GuardarCatbancoscredito(listaCatbancoscredito);
        }

        public static Entity.Contabilidad.Catbancoscredito TraerCatbancoscreditos(int bancocreditoid)
        {
            return MobileBO.Contabilidad.CatbancoscreditoBO.TraerCatbancoscreditos(bancocreditoid);
        }

        public static List<Entity.Contabilidad.Catbancoscredito> TraerCatbancoscreditos()
        {
            return MobileBO.Contabilidad.CatbancoscreditoBO.TraerCatbancoscreditos();
        }

        public static System.Data.DataSet TraerCatbancoscreditosDS()
        {
            return MobileBO.Contabilidad.CatbancoscreditoBO.TraerCatbancoscreditosDS();
        }

        #endregion //Métodos Públicos

        #region CatProyeccion
        public static void GuardarCatProyeccion(List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion)
        {
            MobileBO.Contabilidad.CatProyeccionBO.GuardarCatProyeccion(listaCatProyeccion);
        }

        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccion(string proyeccionid)
        {
            return MobileBO.Contabilidad.CatProyeccionBO.TraerCatProyeccion(proyeccionid);
        }

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccion()
        {
            return MobileBO.Contabilidad.CatProyeccionBO.TraerCatProyeccion();
        }

        public static System.Data.DataSet TraerCatProyeccionDS()
        {
            return MobileBO.Contabilidad.CatProyeccionBO.TraerCatProyeccionDS();
        }

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccionPorEjercicio(int empresa, int Ejercicio)
        {
            return MobileBO.Contabilidad.CatProyeccionBO.TraerCatProyeccionPorEjercicio(empresa, Ejercicio);
        }
        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccionPorCuenta(int empresa, int ejercicio, string cuenta)
        {
            return MobileBO.Contabilidad.CatProyeccionBO.TraerCatProyeccionPorCuenta(empresa, ejercicio, cuenta);
        }

        public static List<Entity.Contabilidad.CatProyeccion> Catproyeccion_Select_Acumulado(int empresa, int Ejercicio, string Cuenta, int Nivel)
        {
            return MobileBO.Contabilidad.CatProyeccionBO.Catproyeccion_Select_Acumulado(empresa, Ejercicio, Cuenta, Nivel);
        }

        #endregion //Métodos Públicos

        #region Métodos CatCuentasPersonal
        public static void GuardarCatcuentaspersonal(List<Entity.Contabilidad.Catcuentaspersonal> listaCatcuentaspersonal)
        {
            MobileBO.Contabilidad.CatcuentaspersonalBO.GuardarCatcuentaspersonal(listaCatcuentaspersonal);
        }

        public static Entity.Contabilidad.Catcuentaspersonal TraerCatcuentaspersonales(string cuentapersonalid)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerCatcuentaspersonales(cuentapersonalid);
        }

        public static List<Entity.Contabilidad.Catcuentaspersonal> TraerCatcuentaspersonales()
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerCatcuentaspersonales();
        }

        public static System.Data.DataSet TraerCatcuentaspersonalesDS()
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerCatcuentaspersonalesDS();
        }

        public static System.Data.DataSet TraerGastosPersonales(string EmpresaID, string CuentaIni, string CuentaFin, DateTime FechaInicial, DateTime FechaFinal)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerGastosPersonales(EmpresaID, CuentaIni, CuentaFin, FechaInicial, FechaFinal);
        }

        public static System.Data.DataSet TraerPresupuestoContable(string EmpresaID, int Anio, int Operativo)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerPresupuestoContable(EmpresaID, Anio, Operativo);
        }

        public static System.Data.DataSet TraerPresupuestoContableVsReal(string EmpresaID, DateTime Fecha, int Operativo)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerPresupuestoContableVsReal(EmpresaID, Fecha, Operativo);
        }

        public static System.Data.DataSet TraerInformeProyeccionComparativoEntreYEars2(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerInformeProyeccionComparativoEntreYEars2(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);
        }

        public static System.Data.DataSet TraerInformeProyeccionMensual(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {
            return MobileBO.Contabilidad.CatcuentaspersonalBO.TraerInformeProyeccionMensual(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);
        }


        #endregion //Métodos Públicos


        #region CancelarCuentasResultados
        public static System.Data.DataSet CancelarCuentasResultado(int anio, string empresaid)
        {
            return MobileBO.Contabilidad.AcvmovBO.CancelarCuentasResultado(anio, empresaid);
        }
        #endregion

        #region CierreContabilidad
        public static void GuardarCierrecontabilidad(List<Entity.Contabilidad.Cierrecontabilidad> listaCierrecontabilidad)
        {
            MobileBO.Contabilidad.CierrecontabilidadBO.GuardarCierrecontabilidad(listaCierrecontabilidad);
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(int cierrecontableid)
        {
            return MobileBO.Contabilidad.CierrecontabilidadBO.TraerCierrecontabilidad(cierrecontableid);
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(string empresaid)
        {
            return MobileBO.Contabilidad.CierrecontabilidadBO.TraerCierrecontabilidad(empresaid);
        }

        public static List<Entity.Contabilidad.Cierrecontabilidad> TraerCierrecontabilidad()
        {
            return MobileBO.Contabilidad.CierrecontabilidadBO.TraerCierrecontabilidad();
        }

        public static System.Data.DataSet TraerCierrecontabilidadDS()
        {
            return MobileBO.Contabilidad.CierrecontabilidadBO.TraerCierrecontabilidadDS();
        }
        #endregion

        #region CatSolicitantesPago

        public static void GuardarCatsolicitantespago(List<Entity.Contabilidad.Catsolicitantespago> listaCatsolicitantespago)
        {
            MobileBO.Contabilidad.CatsolicitantespagoBO.GuardarCatsolicitantespago(listaCatsolicitantespago);
        }

        public static Entity.Contabilidad.Catsolicitantespago TraerCatsolicitantespago(string solicitanteid)
        {
            return MobileBO.Contabilidad.CatsolicitantespagoBO.TraerCatsolicitantespago(solicitanteid);
        }

        public static List<Entity.Contabilidad.Catsolicitantespago> TraerCatsolicitantespago()
        {
            return MobileBO.Contabilidad.CatsolicitantespagoBO.TraerCatsolicitantespago();
        }

        public static System.Data.DataSet TraerCatsolicitantespagoDS()
        {
            return MobileBO.Contabilidad.CatsolicitantespagoBO.TraerCatsolicitantespagoDS();
        }

        #endregion

        #region ProgramacionPagos
        public static void GuardarProgramacionpago(List<Entity.Contabilidad.Programacionpago> listaProgramacionpago)
        {
            MobileBO.Contabilidad.ProgramacionpagoBO.GuardarProgramacionpago(listaProgramacionpago);
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagos(string empresaid, string programacionpagoid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagos(empresaid, programacionpagoid);
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagos()
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagos();
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorProveedorID(string empresaid, string proveedorid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagosPorProveedorID(empresaid, proveedorid);
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorSolicitanteID(string empresaid, string solicitanteid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagosPorSolicitanteID(empresaid, solicitanteid);
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagosPorPolizaID(string polizaid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagosPorPolizaID(polizaid);
        }

        public static System.Data.DataSet TraerProgramacionpagosDS()
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerProgramacionpagosDS();
        }

        public static System.Data.DataSet TraerDatosProgramacionPagos(string empresaid, DateTime? fi, DateTime? ff)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerDatosProgramacionPagos(empresaid, fi, ff);
        }

        public static System.Data.DataSet TraerListadoPagosProgramados(string empresaid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerListadoPagosProgramados(empresaid);
        }

        public static System.Data.DataSet TraerRelacionPagosProgramados(DateTime fechaInicio, DateTime fechaFin, string empresaid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.TraerRelacionPagosProgramados(fechaInicio, fechaFin, empresaid);
        }

        public static System.Data.DataSet ValidaPolizaPagoProgramado(string empresaid, string polizaid)
        {
            return MobileBO.Contabilidad.ProgramacionpagoBO.ValidaPolizaPagoProgramado(empresaid, polizaid);
        }
        #endregion

        #region AlertasB1
        public static System.Data.DataSet TraerAlertasNotificarB1(string VendedorID)
        {
            return MobileBO.Contabilidad.CatcierreBO.TraerAlertasNotificarB1(VendedorID);
        }
        #endregion

        #region AlertasA1
        public static System.Data.DataSet TraerAlertasNotificarA1(string VendedorID)
        {
            return MobileBO.Contabilidad.CatcierreBO.TraerAlertasNotificarA1(VendedorID);
        }
        #endregion

        #region CatSatListaNegraContribuyentes
        public static void GuardarCatsatlistanegracontribuyente(List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyente)
        {
            MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.GuardarCatsatlistanegracontribuyente(listaCatsatlistanegracontribuyente);
        }

        public static Entity.Contabilidad.Catsatlistanegracontribuyente TraerCatsatlistanegracontribuyentes(int numero)
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerCatsatlistanegracontribuyentes(numero);
        }

        public static List<Entity.Contabilidad.Catsatlistanegracontribuyente> TraerCatsatlistanegracontribuyentes()
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerCatsatlistanegracontribuyentes();
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDS(int numero)
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerCatsatlistanegracontribuyentesDS(numero);
        }
        public static void EliminarTodoCatsatlistanegracontribuyentes()
        {
            MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.EliminarTodoCatsatlistanegracontribuyentes();
        }
        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesGridDS(bool SoloBalor, string nombre, string rfc)
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerCatsatlistanegracontribuyentesGridDS(SoloBalor, nombre, rfc);
        }
        public static System.Data.DataSet TraerPolizasProveedorEnListaNegra(string ProveedorID)
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerPolizasProveedorEnListaNegra(ProveedorID);
        }
        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDesdeRfc(string rfc)
        {
            return MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.TraerCatsatlistanegracontribuyentesDesdeRfc(rfc);
        }
        public static void GuardarCatsatlistanegracontribuyenteTEMP(List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyente)
        {
            MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.GuardarCatsatlistanegracontribuyenteTEMP(listaCatsatlistanegracontribuyente);
        }
        public static void EliminarTodoCatsatlistanegracontribuyentesTEMP(string usuario, string sesionid)
        {
            MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.EliminarTodoCatsatlistanegracontribuyentesTEMP(usuario, sesionid);
        }
        public static void CopiarTodoCatsatlistanegracontribuyentesTEMP(string usuario, string sesionid)
        {
            MobileBO.Contabilidad.CatsatlistanegracontribuyenteBO.CopiarTodoCatsatlistanegracontribuyentesTEMP(usuario, sesionid);
        }

        public static System.Data.DataSet TraeGastos(DateTime fecha, string empresaid)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraeGastos(fecha, empresaid);
        }
        #endregion

        #region Métodos Públicos
        public static void GuardarPolizasnomina(List<Entity.Contabilidad.Polizasnomina> listaPolizasnomina)
        {
            MobileBO.Contabilidad.PolizasnominaBO.GuardarPolizasnomina(listaPolizasnomina);
        }

        public static Entity.Contabilidad.Polizasnomina TraerPolizasnomina(string polizanominaid)
        {
            return MobileBO.Contabilidad.PolizasnominaBO.TraerPolizasnomina(polizanominaid);
        }

        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnomina()
        {
            return MobileBO.Contabilidad.PolizasnominaBO.TraerPolizasnomina();
        }
        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnominaPorPolizaID(string polizaid)
        {
            return MobileBO.Contabilidad.PolizasnominaBO.TraerPolizasnominaPorPolizaID(polizaid);
        }
        public static Entity.Contabilidad.Polizasnomina TraerPolizasnominaPorUUID(string uuid)
        {
            return MobileBO.Contabilidad.PolizasnominaBO.TraerPolizasnominaPorUUID(uuid);
        }

        public static System.Data.DataSet TraerPolizasnominaDS()
        {
            return MobileBO.Contabilidad.PolizasnominaBO.TraerPolizasnominaDS();
        }

        #endregion //Métodos Públicos

        #region Relcuentasespecialesviatico
        public static void GuardarRelcuentasespecialesviatico(List<Entity.Contabilidad.Relcuentasespecialesviatico> listaRelcuentasespecialesviatico)
        {
            MobileBO.Contabilidad.RelcuentasespecialesviaticoBO.GuardarRelcuentasespecialesviatico(listaRelcuentasespecialesviatico);
        }

        public static Entity.Contabilidad.Relcuentasespecialesviatico TraerRelcuentasespecialesviaticos(string relcuentaespecialviaticoid, string cuentaviaticos)
        {
            return MobileBO.Contabilidad.RelcuentasespecialesviaticoBO.TraerRelcuentasespecialesviaticos(relcuentaespecialviaticoid, cuentaviaticos);
        }

        public static List<Entity.Contabilidad.Relcuentasespecialesviatico> TraerRelcuentasespecialesviaticos()
        {
            return MobileBO.Contabilidad.RelcuentasespecialesviaticoBO.TraerRelcuentasespecialesviaticos();
        }

        public static System.Data.DataSet TraerRelcuentasespecialesviaticosDS()
        {
            return MobileBO.Contabilidad.RelcuentasespecialesviaticoBO.TraerRelcuentasespecialesviaticosDS();
        }
        /// <summary>
        /// Este método es para Balor Land Trading
        /// </summary>
        /// <param name="listaCatproveedor"></param>
        public static void GuardarCatproveedorBLT(List<Entity.Contabilidad.Catproveedor> listaCatproveedor)
        {
            MobileBO.Contabilidad.CatproveedorBO.GuardarCatproveedorBLT(listaCatproveedor);
        }
        public void GuardarPolizaBLT(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            new MobileBO.Contabilidad.PolizaBO().GuardarPolizaBLT(listaPolizas);
        }
        public void GuardarPolizaContableBLT(ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgral> ListaacvgralBorrar = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();

            foreach (Entity.Contabilidad.Acvgral elemento in listaAcvGral)
            {
                // Borrar poliza anterior
                Entity.Contabilidad.Acvgral acvgralBorrar = MobileDAL.Contabilidad.Acvgral.TraerAcvgralPorReferenciaIdBLT(elemento.ReferenciaId);
                if (acvgralBorrar != null)
                {
                    ListaacvgralBorrar.Add(acvgralBorrar);
                }
                
            }
            ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosDict = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
            
            MobileDAL.Contabilidad.Acvgral.GuardarPolizaBLT(ref listaAcvGral, ListaacvgralBorrar, ref listaSaldosDict);
        }
        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaBLT(string cuenta, string empresaid)
        {
            return new MobileBO.Contabilidad.CatcuentaBO().TraerCatCuentasPorCuentaBLT(cuenta, empresaid);
        }
        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidadBLT(string empresaid)
        {
            return MobileBO.Contabilidad.CierrecontabilidadBO.TraerCierrecontabilidadBLT(empresaid);
        }
        public static bool ValidaCuentasFiscalesBLT(string EmpresaID, int Anio)
        {
            return MobileBO.Contabilidad.CatcuentaBO.ValidaCuentasFiscalesBLT(EmpresaID, Anio);
        }
        public static System.Data.DataSet TraerFolioMaximoPorTipoPolizaBLT(string tippol, int empresa, DateTime fecha)
        {
            return MobileBO.Contabilidad.AcvmovBO.TraerFolioMaximoPorTipoPolizaBLT(tippol, empresa, fecha);
        }
        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresBLT(string proveedorid, string rfc, string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerCatproveedoresBLT(proveedorid, rfc, EmpresaID);
        }
        public static int TraerSiguienteCodicoProveedorBLT(string EmpresaID)
        {
            return MobileBO.Contabilidad.CatproveedorBO.TraerSiguienteCodicoProveedorBLT(EmpresaID);
        }
        public static List<Entity.Contabilidad.Respuesta> ValidarExistenciaCuenta(string Cuenta,string Descripcion, int CodEmpresa,int DBEmpresa)
        {
            return new MobileBO.Contabilidad.CatcuentaBO().ValidarExistenciaCuenta(Cuenta,Descripcion, CodEmpresa, DBEmpresa);
        }
        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionesRutas(int configuracionid)
        {
            return MobileBO.Contabilidad.ConfiguracionRutasAlmacenamientoBO.TraerConfiguracionRutasAlmacenamiento(configuracionid);
        }
        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionesRutasBLT(int configuracionid)
        {
            return MobileBO.Contabilidad.ConfiguracionRutasAlmacenamientoBO.TraerConfiguracionRutasAlmacenamientoBLT(configuracionid);
        }
        public static System.Data.DataSet ConsultarCatDocumentosPolizas()
        {
            return MobileBO.Contabilidad.AcvmovBO.ConsultarCatDocumentosPolizas();
        }
        public static void GuardarRuta(List<Entity.Contabilidad.DocumentosAdicionalesPolizas> lista)
        {
            MobileBO.Contabilidad.DocumentosAdicionalesPolizasBO.GuardarDocumentosadicionalespoliza(lista);
        }
        public static void GuardarRutaBLT(List<Entity.Contabilidad.DocumentosAdicionalesPolizas> lista)
        {
            MobileBO.Contabilidad.DocumentosAdicionalesPolizasBO.GuardarDocumentosadicionalespolizaBLT(lista);
        }
        public static Entity.Contabilidad.DocumentosAdicionalesPolizas ConsultarDocumentosAdicionalesUuid(string uuid)
        {
            return MobileBO.Contabilidad.DocumentosAdicionalesPolizasBO.TraerDocumentosAdicionalesPolizasuuid(uuid);
        }
        public static Entity.Contabilidad.DocumentosAdicionalesPolizas ConsultarDocumentosAdicionalesUuidBLT(string uuid)
        {
            return MobileBO.Contabilidad.DocumentosAdicionalesPolizasBO.TraerDocumentosAdicionalesPolizasuuidBLT(uuid);
        }
        public static List<Entity.Contabilidad.DocumentosAdicionalesPolizas> ConsultarDocumentosAdicionalesPolizas(string PolizaID)
        {
            return MobileBO.Contabilidad.DocumentosAdicionalesPolizasBO.TraerDocumentosadicionalespolizas(PolizaID);
        }
        public static int ValidarExistenciaFacturaProveedor(string uuid)
        {
            return MobileBO.Contabilidad.CatfacturasproveedorBO.ValidarExistenciaFacturaProveedor(uuid);
        }
        public static void ValidarCuentasCliente(string codigocliente, int cod_empresa)
        {
            MobileBO.Contabilidad.CatcuentaBO.ValidarCuentasCliente(codigocliente, cod_empresa);
        }
        public static System.Data.DataSet TraerPolizasPorUUID(string UUID)
        {
            return new MobileBO.Contabilidad.PolizaBO().TraerPolizasPorUUID(UUID);
        }
        #endregion //Métodos Públicos
    }


}
