using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvgralpdte
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvgralpdte : ISprocBase
    {
        DataSet Acvgralpdte_Select(string acvgralid);

        DataSet Acvgralpdte_Select();

        DataSet Acvgralpdte_Select_PorReferenciaId(string referenciaid);
        DataSet Acvgralpdte_Select_PorEjercicio(int ejercicio,string empresaid);
        DataSet Acvgralpdte_ValidaPuedeProcesar(int ejercicio,string empresaid);

        int Acvgralpdte_Save(
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

        int Acvgralpdte_Delete_PorReferenciaId(string referenciaId);

        int Acvgralpdte_Delete_PorId(string acvgralid);

        // Movimientos
        int Acvpdte_Save(
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

        // AcvGral
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

        // AcvMov
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

        int ProcesarPolizasPdtes(string sxml);


    }

    #endregion //Interfaz IAcvgralpdte

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvgralpdte
    /// </summary>
    public class Acvgralpdte
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvgralpdte()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvgralpdte A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvgralpdte BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvgralpdte elemento = new Entity.Contabilidad.Acvgralpdte();
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
                    elemento.ListaAcvpdte = MobileDAL.Contabilidad.Acvpdte.TraerAcvpdtePorAcvGralPdte(elemento.Acvgralid);
                }
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvgralpdte)
                {
                    codigo = elemento.Acvgralid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Acvgralpdte_Save(
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
                listaAcvgralpdte.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> TraerAcvgralpdte()
        {
            IAcvgralpdte proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                DataSet dsAcvgralpdte = proc.Acvgralpdte_Select();
                foreach (DataRow row in dsAcvgralpdte.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvgralpdte elemento = BuildEntity(row, true);
                    listaAcvgralpdte.Add(elemento);
                }
                listaAcvgralpdte.AcceptChanges();
                return listaAcvgralpdte;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvgralpdte TraerAcvgralpdte(string acvgralid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                Entity.Contabilidad.Acvgralpdte elemento = null;
                DataSet ds = null;
                ds = proc.Acvgralpdte_Select(acvgralid);
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

        public static Entity.Contabilidad.Acvgralpdte TraerAcvgralpdtePorReferenciaId(string referenciaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                Entity.Contabilidad.Acvgralpdte elemento = null;
                DataSet ds = null;
                ds = proc.Acvgralpdte_Select_PorReferenciaId(referenciaid);
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

        public static bool PolizaEnAcvgralpdte(string referenciaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                DataSet ds = null;
                ds = proc.Acvgralpdte_Select_PorReferenciaId(referenciaid);
                return (ds.Tables[0].Rows.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool PuedeProcesar(int ejercicio, string empresaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                DataSet ds = null;
                ds = proc.Acvgralpdte_ValidaPuedeProcesar(ejercicio, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int resultado = int.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                    return (resultado == 0);
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> TraerAcvgralpdtePorEjercicio(int ejercicio,string empresaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                DataSet dsAcvgralpdte = proc.Acvgralpdte_Select_PorEjercicio(ejercicio, empresaid);
                foreach (DataRow row in dsAcvgralpdte.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvgralpdte elemento = BuildEntity(row, true);
                    listaAcvgralpdte.Add(elemento);
                }
                listaAcvgralpdte.AcceptChanges();
                return listaAcvgralpdte;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerAcvgralpdteDS()
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>();
                DataSet ds = proc.Acvgralpdte_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void GuardarPoliza(ref ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte, ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> ListaAcvgralpdteBorrar)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                #region Eliminar Poliza anterior
                // Eliminar poliza anterior si existe
                if (ListaAcvgralpdteBorrar.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgralpdte ElementoBorrar in ListaAcvgralpdteBorrar)
                    {
                        proc.Acvgralpdte_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Guardar Poliza nueva
                // Guarda AcvGralPdte
                string codigoAcvGral;
                int ultimaActAcvGral;

                foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvgralpdte)
                {
                    if (elemento.Estatus != 2) {
                        codigoAcvGral = elemento.Acvgralid;
                        ultimaActAcvGral = elemento.UltimaAct;

                        proc.Acvgralpdte_Save(
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


                        // Guarda AcvPdte (Cargo y abono)
                        string codigoAcvmov;
                        int ultimaActAcvmov;

                        foreach (Entity.Contabilidad.Acvpdte elementoAcvmov in elemento.ListaAcvpdte)
                        {
                            codigoAcvmov = elementoAcvmov.Acvmovid;
                            ultimaActAcvmov = elementoAcvmov.UltimaAct;

                            proc.Acvpdte_Save(
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
                    }
                }

                #endregion

                proc.Transaction.Commit();
                listaAcvgralpdte.AcceptChanges();
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

        public static void ProcesarPolizasPdtes(string sxml)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.ProcesarPolizasPdtes(sxml);
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

        public static void ProcesarPolizasPendientes(ref ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte, ref ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvgral, ref ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvgralpdte>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                #region Eliminar Poliza anterior
                // Eliminar poliza anterior si existe
                if (listaAcvgralpdte.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgralpdte ElementoBorrar in listaAcvgralpdte)
                    {
                        proc.Acvgralpdte_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Eliminar las polizas de acvgral
                // Eliminar poliza anterior si existe
                if (listaAcvgral.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgral ElementoBorrar in listaAcvgral)
                    {
                        proc.Acvgral_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Guardar acvgralpdte en la tabla acvgral
                // Guarda AcvGralPdte
                string codigoAcvGral;
                int ultimaActAcvGral;

                foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvgralpdte)
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


                    // Guarda AcvPdte (Cargo y abono)
                    string codigoAcvmov;
                    int ultimaActAcvmov;

                    foreach (Entity.Contabilidad.Acvpdte elementoAcvmov in elemento.ListaAcvpdte)
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
                }

                #endregion

                #region Guardar acvgral en la tabla acvgralpdte
                // Guarda AcvGralPdte
                string codigoAcvGralPdte;
                int ultimaActAcvGralPdte;

                foreach (Entity.Contabilidad.Acvgral elemento in listaAcvgral)
                {
                    codigoAcvGralPdte = elemento.Acvgralid;
                    ultimaActAcvGralPdte = elemento.UltimaAct;

                    proc.Acvgralpdte_Save(
                    ref codigoAcvGralPdte,
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
                    ref ultimaActAcvGralPdte);
                    elemento.Acvgralid = codigoAcvGralPdte;
                    elemento.UltimaAct = ultimaActAcvGralPdte;


                    // Guarda AcvPdte (Cargo y abono)
                    string codigoAcvmov;
                    int ultimaActAcvmov;

                    foreach (Entity.Contabilidad.Acvmov elementoAcvmov in elemento.ListaAcvmov)
                    {
                        codigoAcvmov = elementoAcvmov.Acvmovid;
                        ultimaActAcvmov = elementoAcvmov.UltimaAct;

                        proc.Acvpdte_Save(
                        ref codigoAcvmov,
                         (elementoAcvmov.EmpresaId != null) ? elementoAcvmov.EmpresaId : null,
                         elementoAcvmov.CodEmpresa,
                         codigoAcvGralPdte,
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
                }

                #endregion

                #region Afectar Saldos
                // Afectar saldos
                string codigoSaldo;
                int ultimaActSaldo;

                foreach (Entity.Contabilidad.Saldo elemento in listaSaldos)
                {
                    codigoSaldo = elemento.Saldoid;
                    ultimaActSaldo = elemento.UltimaAct;

                    proc.Saldo_Save(
                    ref codigoSaldo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     elemento.Ejercicio,
                     (elemento.Cuentaid != null) ? elemento.Cuentaid : null,
                     elemento.Cuenta,
                     elemento.Nivel,
                     elemento.Sdoini,
                     elemento.Car1,
                     elemento.Car2,
                     elemento.Car3,
                     elemento.Car4,
                     elemento.Car5,
                     elemento.Car6,
                     elemento.Car7,
                     elemento.Car8,
                     elemento.Car9,
                     elemento.Car10,
                     elemento.Car11,
                     elemento.Car12,
                     elemento.Abo1,
                     elemento.Abo2,
                     elemento.Abo3,
                     elemento.Abo4,
                     elemento.Abo5,
                     elemento.Abo6,
                     elemento.Abo7,
                     elemento.Abo8,
                     elemento.Abo9,
                     elemento.Abo10,
                     elemento.Abo11,
                     elemento.Abo12,
                     elemento.Sdoinia,
                     elemento.Cara1,
                     elemento.Cara2,
                     elemento.Cara3,
                     elemento.Cara4,
                     elemento.Cara5,
                     elemento.Cara6,
                     elemento.Cara7,
                     elemento.Cara8,
                     elemento.Cara9,
                     elemento.Cara10,
                     elemento.Cara11,
                     elemento.Cara12,
                     elemento.Aboa1,
                     elemento.Aboa2,
                     elemento.Aboa3,
                     elemento.Aboa4,
                     elemento.Aboa5,
                     elemento.Aboa6,
                     elemento.Aboa7,
                     elemento.Aboa8,
                     elemento.Aboa9,
                     elemento.Aboa10,
                     elemento.Aboa11,
                     elemento.Aboa12,
                    ref ultimaActSaldo);
                    elemento.Saldoid = codigoSaldo;
                    elemento.UltimaAct = ultimaActSaldo;

                    proc.CalcularSaldoInicial(elemento.Cuenta, elemento.Ejercicio, elemento.EmpresaId);
                }

                #endregion


                proc.Transaction.Commit();
                listaAcvgralpdte.AcceptChanges();
                listaAcvgral.AcceptChanges();
                listaSaldos.AcceptChanges();
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
        public static bool PolizaEnAcvgralpdteBLT(string referenciaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgralpdte>();
                DataSet ds = null;
                ds = proc.Acvgralpdte_Select_PorReferenciaId(referenciaid);
                return (ds.Tables[0].Rows.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Entity.Contabilidad.Acvgralpdte TraerAcvgralpdtePorReferenciaIdBLT(string referenciaid)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgralpdte>();
                Entity.Contabilidad.Acvgralpdte elemento = null;
                DataSet ds = null;
                ds = proc.Acvgralpdte_Select_PorReferenciaId(referenciaid);
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
        public static void GuardarPolizaBLT(ref ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgralpdte, ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> ListaAcvgralpdteBorrar)
        {
            IAcvgralpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IAcvgralpdte>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                #region Eliminar Poliza anterior
                // Eliminar poliza anterior si existe
                if (ListaAcvgralpdteBorrar.Count > 0)
                {
                    // Borrar los registros de poliza
                    foreach (Entity.Contabilidad.Acvgralpdte ElementoBorrar in ListaAcvgralpdteBorrar)
                    {
                        proc.Acvgralpdte_Delete_PorId(ElementoBorrar.Acvgralid);
                    }
                }

                #endregion

                #region Guardar Poliza nueva
                // Guarda AcvGralPdte
                string codigoAcvGral;
                int ultimaActAcvGral;

                foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvgralpdte)
                {
                    if (elemento.Estatus != 2)
                    {
                        codigoAcvGral = elemento.Acvgralid;
                        ultimaActAcvGral = elemento.UltimaAct;

                        proc.Acvgralpdte_Save(
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


                        // Guarda AcvPdte (Cargo y abono)
                        string codigoAcvmov;
                        int ultimaActAcvmov;

                        foreach (Entity.Contabilidad.Acvpdte elementoAcvmov in elemento.ListaAcvpdte)
                        {
                            codigoAcvmov = elementoAcvmov.Acvmovid;
                            ultimaActAcvmov = elementoAcvmov.UltimaAct;

                            proc.Acvpdte_Save(
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
                    }
                }

                #endregion

                proc.Transaction.Commit();
                listaAcvgralpdte.AcceptChanges();
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
