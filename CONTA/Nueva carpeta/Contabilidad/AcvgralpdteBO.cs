using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvgralpdte
    /// </summary>
    internal class AcvgralpdteBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvgralpdteBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvgralpdte(ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte)
        {
            MobileDAL.Contabilidad.Acvgralpdte.Guardar(ref listaAcvgralpdte);
        }

        public Entity.Contabilidad.Acvgralpdte TraerAcvgralpdte(string acvgralid)
        {
            return MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdte(acvgralid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> TraerAcvgralpdte()
        {
            return MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdte();
        }

        public System.Data.DataSet TraerAcvgralpdteDS()
        {
            return MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdteDS();
        }

        public static Entity.Contabilidad.Acvgralpdte TraerAcvgralpdtePorReferenciaId(string referenciaid)
        {
            return MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdtePorReferenciaId(referenciaid);
        }

        public bool PuedeProcesar(int ejercicio,string empresaid)
        {
            return MobileDAL.Contabilidad.Acvgralpdte.PuedeProcesar(ejercicio, empresaid);
        }

        public void AfectaSaldosParaAgregarPoliza(Entity.Contabilidad.Acvgralpdte acvgralpdte, ref Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos)
        {
            //ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaMovimientos = MobileDAL.Contabilidad.Acvpdte.TraerAcvpdtePorAcvGralPdte(acvgralpdte.Acvgralid);

            foreach (Entity.Contabilidad.Acvpdte mov in acvgralpdte.ListaAcvpdte)
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
                    saldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(cuenta, ejercicio, acvgralpdte.EmpresaId);
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
                                dictSaldos[saldoCargo.Cuentaid].Car1 += importe;
                                break;
                            case 2:
                                dictSaldos[saldoCargo.Cuentaid].Car2 += importe;
                                break;
                            case 3:
                                dictSaldos[saldoCargo.Cuentaid].Car3 += importe;
                                break;
                            case 4:
                                dictSaldos[saldoCargo.Cuentaid].Car4 += importe;
                                break;
                            case 5:
                                dictSaldos[saldoCargo.Cuentaid].Car5 += importe;
                                break;
                            case 6:
                                dictSaldos[saldoCargo.Cuentaid].Car6 += importe;
                                break;
                            case 7:
                                dictSaldos[saldoCargo.Cuentaid].Car7 += importe;
                                break;
                            case 8:
                                dictSaldos[saldoCargo.Cuentaid].Car8 += importe;
                                break;
                            case 9:
                                dictSaldos[saldoCargo.Cuentaid].Car9 += importe;
                                break;
                            case 10:
                                dictSaldos[saldoCargo.Cuentaid].Car10 += importe;
                                break;
                            case 11:
                                dictSaldos[saldoCargo.Cuentaid].Car11 += importe;
                                break;
                            case 12:
                                dictSaldos[saldoCargo.Cuentaid].Car12 += importe;
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
                                dictSaldos[saldoAbono.Cuentaid].Abo1 += importe;
                                break;
                            case 2:
                                dictSaldos[saldoAbono.Cuentaid].Abo2 += importe;
                                break;
                            case 3:
                                dictSaldos[saldoAbono.Cuentaid].Abo3 += importe;
                                break;
                            case 4:
                                dictSaldos[saldoAbono.Cuentaid].Abo4 += importe;
                                break;
                            case 5:
                                dictSaldos[saldoAbono.Cuentaid].Abo5 += importe;
                                break;
                            case 6:
                                dictSaldos[saldoAbono.Cuentaid].Abo6 += importe;
                                break;
                            case 7:
                                dictSaldos[saldoAbono.Cuentaid].Abo7 += importe;
                                break;
                            case 8:
                                dictSaldos[saldoAbono.Cuentaid].Abo8 += importe;
                                break;
                            case 9:
                                dictSaldos[saldoAbono.Cuentaid].Abo9 += importe;
                                break;
                            case 10:
                                dictSaldos[saldoAbono.Cuentaid].Abo10 += importe;
                                break;
                            case 11:
                                dictSaldos[saldoAbono.Cuentaid].Abo11 += importe;
                                break;
                            case 12:
                                dictSaldos[saldoAbono.Cuentaid].Abo12 += importe;
                                break;
                        }
                    }
                }
            }
        }



        
       
        public void AfectaSaldosParaEliminarPoliza(Entity.Contabilidad.Acvgralpdte acvgralpdte, ref Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaMovimientos = MobileDAL.Contabilidad.Acvpdte.TraerAcvpdtePorAcvGralPdte(acvgralpdte.Acvgralid);

            foreach (Entity.Contabilidad.Acvpdte mov in listaMovimientos)
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
                    saldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(cuenta, ejercicio, acvgralpdte.EmpresaId);
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
        
        #endregion //Métodos Públicos

    }
}
