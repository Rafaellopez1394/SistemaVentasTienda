using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvgral
    /// </summary>
    internal class AcvgralBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvgralBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvgral(ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral)
        {
            MobileDAL.Contabilidad.Acvgral.Guardar(ref listaAcvgral);
        }

        public Entity.Contabilidad.Acvgral TraerAcvgral(string acvgralid)
        {
            return MobileDAL.Contabilidad.Acvgral.TraerAcvgral(acvgralid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvgral> TraerAcvgral()
        {
            return MobileDAL.Contabilidad.Acvgral.TraerAcvgral();
        }

        public System.Data.DataSet TraerAcvgralDS()
        {
            return MobileDAL.Contabilidad.Acvgral.TraerAcvgralDS();
        }

        public static Entity.Contabilidad.Acvgral TraerAcvgralPorReferenciaId(string referenciaid)
        {
            return MobileDAL.Contabilidad.Acvgral.TraerAcvgralPorReferenciaId(referenciaid);
        }
        /*
        public void GenerarPoliza(Entity.Contabilidad.Movanticipo anticipo)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
            Entity.Contabilidad.CatCuenta cuentaAnticipo = new Entity.Contabilidad.CatCuenta();
            Entity.Contabilidad.CatCuenta cuentaBanco;
            Entity.Configuraciones.Amdsucursalesempresa sucursal = new Entity.Configuraciones.Amdsucursalesempresa();
            Entity.Configuraciones.Amdconfiguracionessucursal configSucursal;
            string sucursalid = anticipo.SucursalId;
            string tipoPoliza = "CH";

            configSucursal = MobileDAL.Configuraciones.Amdconfiguracionessucursal.TraerAmdconfiguracionessucursalesPorSucursal(sucursalid);
            if (configSucursal != null)
            {
                cuentaAnticipo = MobileDAL.Contabilidad.CatCuenta.TraerCatCuentas(configSucursal.Cuentaanticipo);
            }

            cuentaBanco = MobileDAL.Contabilidad.CatCuenta.TraerCatCuentas(anticipo.Cuentachequeid);
            sucursal = MobileDAL.Configuraciones.Amdsucursalesempresa.TraerAmdsucursalesempresas(sucursalid);

            //Entity.Contabilidad.Cattiposmovimiento tipoMovimiento = MobileDAL.Contabilidad.Cattiposmovimiento.TraerCattiposmovimientos((int)Entity.TipoMovimiento.Anticipo);  // (1) Tipo Anticipo

            if (anticipo.Movanticipoid != Guid.Empty.ToString())
            {
                DateTime fecha = DateTime.Now;

                #region AcvGral
                // Guarda AcvGral
                Entity.Contabilidad.Acvgral acvGral = new Entity.Contabilidad.Acvgral();
                acvGral.SucursalId = sucursalid;
                acvGral.ReferenciaId = anticipo.Movanticipoid;
                acvGral.CodEmpresa = sucursal.Sucursal.ToString();
                acvGral.Anomes = anticipo.Fecha.ToString("yyyyMM");  // Fecha del recibo del anticipo
                acvGral.TipPol = tipoPoliza;  // Catalgo de tipos de polizas
                acvGral.TipoMov = 1;
                acvGral.NumPol = anticipo.Referencia;
                acvGral.FecPol = anticipo.Fecha;
                acvGral.Concepto = anticipo.Concepto;
                acvGral.Importe = anticipo.Importe;
                acvGral.Usuario = anticipo.Usuario;
                acvGral.Estatus = anticipo.Estatus;
                acvGral.Fecha = fecha;

                // Guarda AcvMov
                // Cargo
                Entity.Contabilidad.Acvmov acvMovCargo = new Entity.Contabilidad.Acvmov();
                acvMovCargo.SucursalId = acvGral.SucursalId;
                acvMovCargo.CodEmpresa = sucursal.Sucursal.ToString();
                acvMovCargo.Acvgralid = acvGral.Acvgralid;
                acvMovCargo.Anomes = acvGral.Anomes;  // Fecha del recibo del anticipo
                acvMovCargo.FecPol = acvGral.FecPol;
                acvMovCargo.TipPol = acvGral.TipPol;                      // Catalgo de tipos de polizas
                acvMovCargo.NumPol = acvGral.NumPol;
                acvMovCargo.NumRenglon = 1;
                acvMovCargo.TipMov = Entity.TipMov.Cargo.GetHashCode().ToString();                        // Cargo
                acvMovCargo.Cuenta = cuentaAnticipo.Cuenta;                         // Cuenta de anticipos
                acvMovCargo.Concepto = acvGral.Concepto;
                acvMovCargo.Refer = anticipo.Referencia;
                acvMovCargo.ClaseConta = "F";
                acvMovCargo.Importe = acvGral.Importe;
                acvMovCargo.TasaIva = 0;
                acvMovCargo.Iva = 0;
                acvMovCargo.RetencionIva = 0;
                acvMovCargo.Pendiente = false;
                acvMovCargo.CodFlujo = cuentaAnticipo.FlujoCar;                       // Flujo_Car de la tabla Cat_Cuentas
                acvMovCargo.CodProveedor = "";
                acvMovCargo.FechaFiscal = acvGral.FecPol;
                acvMovCargo.Ctaaux = cuentaAnticipo.Cuenta;                         // Cuenta de anticipos
                acvMovCargo.Usuario = acvGral.Usuario;
                acvMovCargo.Estatus = 1;
                acvMovCargo.Fecha = fecha;

                // Abono
                Entity.Contabilidad.Acvmov acvMovAbono = new Entity.Contabilidad.Acvmov();
                acvMovAbono.SucursalId = acvGral.SucursalId;
                acvMovAbono.CodEmpresa = sucursal.Sucursal.ToString();
                acvMovAbono.Acvgralid = acvGral.Acvgralid;
                acvMovAbono.Anomes = acvGral.Anomes;  // Fecha del recibo del anticipo
                acvMovAbono.FecPol = acvGral.FecPol;
                acvMovAbono.TipPol = acvGral.TipPol;
                acvMovAbono.NumPol = acvGral.NumPol;
                acvMovAbono.NumRenglon = 2;
                acvMovAbono.TipMov = Entity.TipMov.Abono.GetHashCode().ToString();                        // Abono
                acvMovAbono.Cuenta = cuentaBanco.Cuenta;                         // Cuenta de bancos
                acvMovAbono.Concepto = acvGral.Concepto;
                acvMovAbono.Refer = anticipo.Referencia;
                acvMovAbono.ClaseConta = "F";
                acvMovAbono.Importe = acvGral.Importe;
                acvMovAbono.TasaIva = 0;
                acvMovAbono.Iva = 0;
                acvMovAbono.RetencionIva = 0;
                acvMovAbono.Pendiente = false;
                acvMovAbono.CodFlujo = cuentaBanco.FlujoAbo;                       // Flujo_Abo de la tabla Cat_Cuentas
                acvMovAbono.CodProveedor = "";
                acvMovAbono.FechaFiscal = acvGral.FecPol;
                acvMovAbono.Ctaaux = cuentaBanco.Cuenta;                         // Cuenta de bancos
                acvMovAbono.Usuario = acvGral.Usuario;
                acvMovAbono.Estatus = 1;
                acvMovAbono.Fecha = fecha;

                acvGral.ListaAcvmov.Add(acvMovCargo);
                acvGral.ListaAcvmov.Add(acvMovAbono);
                listaAcvGral.Add(acvGral);

                #endregion

                #region Saldos
                // Afectar Saldos
                Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos = new Dictionary<string, Entity.Contabilidad.Saldo>();

                // Borrar poliza anterior
                ListaDeEntidades<Entity.Contabilidad.Acvgral> ListaacvgralBorrar = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
                Entity.Contabilidad.Acvgral acvgralBorrar = MobileDAL.Contabilidad.Acvgral.TraerAcvgralPorReferenciaId(anticipo.Movanticipoid);
                if (acvgralBorrar != null)
                {
                    AfectaSaldosParaEliminarPoliza(acvgralBorrar, ref dictSaldos);
                    ListaacvgralBorrar.Add(acvgralBorrar);
                }

                foreach (Entity.Contabilidad.Acvmov mov in acvGral.ListaAcvmov)
                {
                    ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosTmp = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                    Entity.Contabilidad.CatCuenta catcuenta = MobileDAL.Contabilidad.CatCuenta.TraerCatCuentasPorCuenta(mov.Cuenta);
                    string ejercicio = mov.FecPol.Year.ToString();
                    int mes = mov.FecPol.Month;
                    decimal importe = mov.Importe;
                    int nivel = catcuenta.Nivel;
                    string cuenta = mov.Cuenta;

                    for (int i = 1; i <= nivel; i++)
                    {
                        Entity.Contabilidad.Saldo saldo;
                        cuenta = mov.Cuenta.Substring(0, 4 * i).PadRight(24 - (4 * i), '0');
                        saldo = MobileDAL.Contabilidad.Saldo.TraerSaldoPorCuentaEjercicio(cuenta, ejercicio, acvGral.SucursalId);
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

                #endregion

                ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosDict = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                foreach (Entity.Contabilidad.Saldo s in dictSaldos.Values)
                {
                    listaSaldosDict.Add(s);
                }

                MobileDAL.Contabilidad.Acvgral.GuardarPoliza(ref listaAcvGral, ListaacvgralBorrar, ref listaSaldosDict);
            }
        }

        public void AfectaSaldosParaEliminarPoliza(Entity.Contabilidad.Acvgral acvgral, ref Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvmov> listaMovimientos = MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorAcvGral(acvgral.Acvgralid);
            
            foreach (Entity.Contabilidad.Acvmov mov in listaMovimientos)
            {
                Entity.Contabilidad.Catcuenta catcuenta = MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuenta(mov.Cuenta);

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
        */

        public static void BorrarPorId(string acvgralid)
        {
            MobileDAL.Contabilidad.Acvgral.BorrarPorId(acvgralid);
        }

        public static void IntercambiaPolizasContablesFiscales(string acvgralid, string acvgralpdteid)
        {
            MobileDAL.Contabilidad.Acvgral.IntercambiaPolizasContablesFiscales(acvgralid, acvgralpdteid);
        }
        #endregion //Métodos Públicos

    }
}
