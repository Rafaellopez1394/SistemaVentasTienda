using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvgral
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvgral : ISprocBase
    {
        DataSet Acvgral_Select(string acvgralid);

        DataSet Acvgral_Select();

        DataSet Acvgral_Select_PorReferenciaId(string referenciaid);
        DataSet Acvgral_Select_PorFolioTipoFecha(string numpol, string tippol, DateTime FecPol,string empresaid);

        int IntercambiaPolizasContablesFiscales(string acvgralid, string acvgralpdteid);

        int Acvgral_Save(
        ref string acvgralid,
        string empresaid,
        string referenciaId,
        string codEmpresa,
        string anomes,
        string tipPol,
        int tipoMov,
        string numPol,
        DateTime fecPol,
        string concepto,
        decimal importe,
        string usuario,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);

        int Acvgral_Delete_PorReferenciaId(string referenciaId);

        int Acvgral_Delete_PorId(string acvgralid);

        // Movimientos
        int Acvmov_Save(
        ref string acvmovid,
        string empresaid,
        string codEmpresa,
        string acvgralid,
        string anomes,
        DateTime fecPol,
        string tipPol,
        string numPol,
        int numRenglon,
        string tipMov,
        string cuenta,
        string concepto,
        string refer,
        string claseConta,
        decimal importe,
        decimal tasaIva,
        decimal iva,
        decimal retencionIva,
        bool pendiente,
        string codFlujo,
        string codProveedor,
        DateTime fechaFiscal,
        string ctaaux,
        string usuario,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);


        // Saldos
        int Saldo_Save(
        ref string saldoid,
        string empresaid,
        string codEmpresa,
        string ejercicio,
        string cuentaid,
        string cuenta,
        int nivel,
        decimal sdoini,
        decimal car1,
        decimal car2,
        decimal car3,
        decimal car4,
        decimal car5,
        decimal car6,
        decimal car7,
        decimal car8,
        decimal car9,
        decimal car10,
        decimal car11,
        decimal car12,
        decimal abo1,
        decimal abo2,
        decimal abo3,
        decimal abo4,
        decimal abo5,
        decimal abo6,
        decimal abo7,
        decimal abo8,
        decimal abo9,
        decimal abo10,
        decimal abo11,
        decimal abo12,
        decimal sdoinia,
        decimal cara1,
        decimal cara2,
        decimal cara3,
        decimal cara4,
        decimal cara5,
        decimal cara6,
        decimal cara7,
        decimal cara8,
        decimal cara9,
        decimal cara10,
        decimal cara11,
        decimal cara12,
        decimal aboa1,
        decimal aboa2,
        decimal aboa3,
        decimal aboa4,
        decimal aboa5,
        decimal aboa6,
        decimal aboa7,
        decimal aboa8,
        decimal aboa9,
        decimal aboa10,
        decimal aboa11,
        decimal aboa12,
        ref int ultimaAct);

        int CalcularSaldoInicial(string cuenta, string ejercicio, string empresaid);

        int AfectaSaldosAcvGral(string Acvgralid);

    }

    #endregion //Interfaz IAcvgral

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvgral
    /// </summary>
    public class Acvgral
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvgral()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvgral A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvgral BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvgral elemento = new Entity.Contabilidad.Acvgral();
            if (!Convert.IsDBNull(row["AcvGralID"]))
            {
                elemento.Acvgralid = row["AcvGralID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Referencia_ID"]))
            {
                elemento.ReferenciaId = row["Referencia_ID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["AnoMes"]))
            {
                elemento.Anomes = row["AnoMes"].ToString();
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Tipo_Mov"]))
            {
                elemento.TipoMov = int.Parse(row["Tipo_Mov"].ToString());
            }
            if (!Convert.IsDBNull(row["Num_Pol"]))
            {
                elemento.NumPol = row["Num_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Pol"]))
            {
                elemento.FecPol = DateTime.Parse(row["Fec_Pol"].ToString());
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (getChilds)
            {
                // Llenar lista detalle
                if (elemento.Acvgralid != string.Empty)
                {
                    elemento.ListaAcvmov = MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorAcvGral(elemento.Acvgralid);
                }
            }

            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvgral elemento in listaAcvgral)
                {
                    codigo = elemento.Acvgralid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Acvgral_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     (elemento.ReferenciaId != null) ? elemento.ReferenciaId : null,
                     elemento.CodEmpresa,
                     elemento.Anomes,
                     elemento.TipPol,
                     elemento.TipoMov,
                     elemento.NumPol,
                     elemento.FecPol,
                     elemento.Concepto,
                     elemento.Importe,
                     elemento.Usuario,
                     elemento.Estatus,
                     elemento.Fecha,
                    ref ultimaAct);
                    elemento.Acvgralid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaAcvgral.AcceptChanges();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Acvgral> TraerAcvgral()
        {
            IAcvgral proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
                proc = Utilerias.GenerarSproc<IAcvgral>();
                DataSet dsAcvgral = proc.Acvgral_Select();
                foreach (DataRow row in dsAcvgral.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvgral elemento = BuildEntity(row, true);
                    listaAcvgral.Add(elemento);
                }
                listaAcvgral.AcceptChanges();
                return listaAcvgral;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvgral TraerAcvgral(string acvgralid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>();
                Entity.Contabilidad.Acvgral elemento = null;
                DataSet ds = null;
                ds = proc.Acvgral_Select(acvgralid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvgral TraerAcvgral(string numeropoliza, string tipopoliza, DateTime fecpol, string empresaid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>();
                Entity.Contabilidad.Acvgral elemento = null;
                DataSet ds = null;
                ds = proc.Acvgral_Select_PorFolioTipoFecha(numeropoliza, tipopoliza, fecpol, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, true);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvgral TraerAcvgralPorReferenciaId(string referenciaid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>();
                Entity.Contabilidad.Acvgral elemento = null;
                DataSet ds = null;
                ds = proc.Acvgral_Select_PorReferenciaId(referenciaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool PolizaEnAcvgral(string referenciaid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>();
                DataSet ds = null;
                ds = proc.Acvgral_Select_PorReferenciaId(referenciaid);
                return (ds.Tables[0].Rows.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerAcvgralDS()
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>();
                DataSet ds = proc.Acvgral_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void BorrarPorReferenciaId(string referenciaId)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.Acvgral_Delete_PorReferenciaId(referenciaId);
                proc.Transaction.Commit();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static void BorrarPorId(string acvgralid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.Acvgral_Delete_PorId(acvgralid);
                proc.Transaction.Commit();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static void GuardarPoliza(ref ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral, ListaDeEntidades<Entity.Contabilidad.Acvgral> ListaAcvgralBorrar, ref ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                #region Eliminar Poliza anterior y regresar saldos
                // Regresar saldos y eliminar poliza anterior si existe
                if (ListaAcvgralBorrar.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgral ElementoBorrar in ListaAcvgralBorrar) {
                        proc.Acvgral_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Guardar Poliza nueva
                // Guarda AcvGral
                string codigoAcvGral;
                int ultimaActAcvGral;

                foreach (Entity.Contabilidad.Acvgral elemento in listaAcvgral)
                {
                    if (elemento.Estatus != 2)
                    {
                        codigoAcvGral = elemento.Acvgralid;
                        ultimaActAcvGral = elemento.UltimaAct;

                        proc.Acvgral_Save(
                        ref codigoAcvGral,
                         (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                         (elemento.ReferenciaId != null) ? elemento.ReferenciaId : null,
                         elemento.CodEmpresa,
                         elemento.Anomes,
                         elemento.TipPol,
                         elemento.TipoMov,
                         elemento.NumPol,
                         elemento.FecPol,
                         elemento.Concepto,
                         elemento.Importe,
                         elemento.Usuario,
                         elemento.Estatus,
                         elemento.Fecha,
                        ref ultimaActAcvGral);
                        elemento.Acvgralid = codigoAcvGral;
                        elemento.UltimaAct = ultimaActAcvGral;


                        // Guarda AcvMov (Cargo y abono)
                        string codigoAcvmov;
                        int ultimaActAcvmov;

                        foreach (Entity.Contabilidad.Acvmov elementoAcvmov in elemento.ListaAcvmov)
                        {
                            codigoAcvmov = elementoAcvmov.Acvmovid;
                            ultimaActAcvmov = elementoAcvmov.UltimaAct;

                            proc.Acvmov_Save(
                            ref codigoAcvmov,
                             (elementoAcvmov.EmpresaId != null) ? elementoAcvmov.EmpresaId : null,
                             elementoAcvmov.CodEmpresa,
                             codigoAcvGral,
                             elementoAcvmov.Anomes,
                             elementoAcvmov.FecPol,
                             elementoAcvmov.TipPol,
                             elementoAcvmov.NumPol,
                             elementoAcvmov.NumRenglon,
                             elementoAcvmov.TipMov,
                             elementoAcvmov.Cuenta,
                             elementoAcvmov.Concepto,
                             elementoAcvmov.Refer,
                             elementoAcvmov.ClaseConta,
                             elementoAcvmov.Importe,
                             elementoAcvmov.TasaIva,
                             elementoAcvmov.Iva,
                             elementoAcvmov.RetencionIva,
                             elementoAcvmov.Pendiente,
                             elementoAcvmov.CodFlujo,
                             elementoAcvmov.CodProveedor,
                             elementoAcvmov.FechaFiscal,
                             elementoAcvmov.Ctaaux,
                             elementoAcvmov.Usuario,
                             elementoAcvmov.Estatus,
                             elementoAcvmov.Fecha,
                            ref ultimaActAcvmov);
                            elementoAcvmov.Acvmovid = codigoAcvmov;
                            elementoAcvmov.UltimaAct = ultimaActAcvmov;
                        }

                        //Este codigo reemplaza la afectacion de saldos contables que esta comentado en las lineas de abajo.
                        proc.AfectaSaldosAcvGral(codigoAcvGral);
                    }
                }

                #endregion

                #region Afectar Saldos
                //// Afectar saldos
                //string codigoSaldo;
                //int ultimaActSaldo;

                //foreach (Entity.Contabilidad.Saldo elemento in listaSaldos)
                //{
                //    codigoSaldo = elemento.Saldoid;
                //    ultimaActSaldo = elemento.UltimaAct;

                //    proc.Saldo_Save(
                //    ref codigoSaldo,
                //     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                //     elemento.CodEmpresa,
                //     elemento.Ejercicio,
                //     (elemento.Cuentaid != null) ? elemento.Cuentaid : null,
                //     elemento.Cuenta,
                //     elemento.Nivel,
                //     elemento.Sdoini,
                //     elemento.Car1,
                //     elemento.Car2,
                //     elemento.Car3,
                //     elemento.Car4,
                //     elemento.Car5,
                //     elemento.Car6,
                //     elemento.Car7,
                //     elemento.Car8,
                //     elemento.Car9,
                //     elemento.Car10,
                //     elemento.Car11,
                //     elemento.Car12,
                //     elemento.Abo1,
                //     elemento.Abo2,
                //     elemento.Abo3,
                //     elemento.Abo4,
                //     elemento.Abo5,
                //     elemento.Abo6,
                //     elemento.Abo7,
                //     elemento.Abo8,
                //     elemento.Abo9,
                //     elemento.Abo10,
                //     elemento.Abo11,
                //     elemento.Abo12,
                //     elemento.Sdoinia,
                //     elemento.Cara1,
                //     elemento.Cara2,
                //     elemento.Cara3,
                //     elemento.Cara4,
                //     elemento.Cara5,
                //     elemento.Cara6,
                //     elemento.Cara7,
                //     elemento.Cara8,
                //     elemento.Cara9,
                //     elemento.Cara10,
                //     elemento.Cara11,
                //     elemento.Cara12,
                //     elemento.Aboa1,
                //     elemento.Aboa2,
                //     elemento.Aboa3,
                //     elemento.Aboa4,
                //     elemento.Aboa5,
                //     elemento.Aboa6,
                //     elemento.Aboa7,
                //     elemento.Aboa8,
                //     elemento.Aboa9,
                //     elemento.Aboa10,
                //     elemento.Aboa11,
                //     elemento.Aboa12,
                //    ref ultimaActSaldo);
                //    elemento.Saldoid = codigoSaldo;
                //    elemento.UltimaAct = ultimaActSaldo;

                //    proc.CalcularSaldoInicial(elemento.Cuenta, elemento.Ejercicio, elemento.EmpresaId);
                //}

                #endregion

                proc.Transaction.Commit();
                listaAcvgral.AcceptChanges();
                //listaSaldos.AcceptChanges();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static bool PolizaEnAcvgralBLT(string referenciaid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgral>();
                DataSet ds = null;
                ds = proc.Acvgral_Select_PorReferenciaId(referenciaid);
                return (ds.Tables[0].Rows.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void GuardarPolizaBLT(ref ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral, ListaDeEntidades<Entity.Contabilidad.Acvgral> ListaAcvgralBorrar, ref ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                #region Eliminar Poliza anterior y regresar saldos
                // Regresar saldos y eliminar poliza anterior si existe
                if (ListaAcvgralBorrar.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgral ElementoBorrar in ListaAcvgralBorrar)
                    {
                        proc.Acvgral_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Guardar Poliza nueva
                // Guarda AcvGral
                string codigoAcvGral;
                int ultimaActAcvGral;

                foreach (Entity.Contabilidad.Acvgral elemento in listaAcvgral)
                {
                    if (elemento.Estatus != 2)
                    {
                        codigoAcvGral = elemento.Acvgralid;
                        ultimaActAcvGral = elemento.UltimaAct;

                        proc.Acvgral_Save(
                        ref codigoAcvGral,
                         (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                         (elemento.ReferenciaId != null) ? elemento.ReferenciaId : null,
                         elemento.CodEmpresa,
                         elemento.Anomes,
                         elemento.TipPol,
                         elemento.TipoMov,
                         elemento.NumPol,
                         elemento.FecPol,
                         elemento.Concepto,
                         elemento.Importe,
                         elemento.Usuario,
                         elemento.Estatus,
                         elemento.Fecha,
                        ref ultimaActAcvGral);
                        elemento.Acvgralid = codigoAcvGral;
                        elemento.UltimaAct = ultimaActAcvGral;


                        // Guarda AcvMov (Cargo y abono)
                        string codigoAcvmov;
                        int ultimaActAcvmov;

                        foreach (Entity.Contabilidad.Acvmov elementoAcvmov in elemento.ListaAcvmov)
                        {
                            codigoAcvmov = elementoAcvmov.Acvmovid;
                            ultimaActAcvmov = elementoAcvmov.UltimaAct;

                            proc.Acvmov_Save(
                            ref codigoAcvmov,
                             (elementoAcvmov.EmpresaId != null) ? elementoAcvmov.EmpresaId : null,
                             elementoAcvmov.CodEmpresa,
                             codigoAcvGral,
                             elementoAcvmov.Anomes,
                             elementoAcvmov.FecPol,
                             elementoAcvmov.TipPol,
                             elementoAcvmov.NumPol,
                             elementoAcvmov.NumRenglon,
                             elementoAcvmov.TipMov,
                             elementoAcvmov.Cuenta,
                             elementoAcvmov.Concepto,
                             elementoAcvmov.Refer,
                             elementoAcvmov.ClaseConta,
                             elementoAcvmov.Importe,
                             elementoAcvmov.TasaIva,
                             elementoAcvmov.Iva,
                             elementoAcvmov.RetencionIva,
                             elementoAcvmov.Pendiente,
                             elementoAcvmov.CodFlujo,
                             elementoAcvmov.CodProveedor,
                             elementoAcvmov.FechaFiscal,
                             elementoAcvmov.Ctaaux,
                             elementoAcvmov.Usuario,
                             elementoAcvmov.Estatus,
                             elementoAcvmov.Fecha,
                            ref ultimaActAcvmov);
                            elementoAcvmov.Acvmovid = codigoAcvmov;
                            elementoAcvmov.UltimaAct = ultimaActAcvmov;
                        }

                        //Este codigo reemplaza la afectacion de saldos contables que esta comentado en las lineas de abajo.
                        proc.AfectaSaldosAcvGral(codigoAcvGral);
                    }
                }

                #endregion

                #region Afectar Saldos

                #endregion

                proc.Transaction.Commit();
                listaAcvgral.AcceptChanges();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static Entity.Contabilidad.Acvgral TraerAcvgralPorReferenciaIdBLT(string referenciaid)
        {
            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgral>();
                Entity.Contabilidad.Acvgral elemento = null;
                DataSet ds = null;
                ds = proc.Acvgral_Select_PorReferenciaId(referenciaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public static void IntercambiaPolizasContablesFiscales(string acvgralid, string acvgralpdteid)
        {

            IAcvgral proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgral>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.IntercambiaPolizasContablesFiscales(acvgralid, acvgralpdteid);
                proc.Transaction.Commit();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }

        }
        #endregion Métodos Públicos
    }
}
