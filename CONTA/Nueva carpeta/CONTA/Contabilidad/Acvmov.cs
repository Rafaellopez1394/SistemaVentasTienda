using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvmov
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvmov : ISprocBase
    {
        DataSet Acvmov_Select(string acvmovid);

        DataSet Acvmov_Select();

        DataSet Acvmov_Select_PorAcvGral(string acvgralid);

        DataSet AcvMov_Select_EmpresaCuentaFecha(string EmpresaID, int Anio, string Cuenta);


        DataSet Acvmov_CuentaRefer_Select(string cuenta, string refer);
        int Acvmov_Save(
        ref string acvmovid,
        string EmpresaID,
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

        DataSet TraerFolioMaximoPorTipoPoliza(string tippol, int empresa, DateTime fecha);

        DataSet VerificarFolioPoliza(string NumPol, string TipPol, DateTime FecPol, string EmpresaID, bool Pendiente);

        DataSet Catdocumentospoliza_Select();

        DataSet TraerGatosDelReporteCostos(DateTime fechainicio, DateTime fechafin, string empresaid);

        DataSet CancelacionCuentasResultado(int anio, string empresaid);

        DataSet ReporteContabilidadExcel(string EmpresaID,DateTime FechaInicio,DateTime FechaFin);

        DataSet TraeMovimientosPorCesion(string concepto, string cuenta, string empresaid);

        DataSet TraeGastos(DateTime fecha, string empresaid);
    }

    #endregion //Interfaz IAcvmov

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvmov
    /// </summary>
    public class Acvmov
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvmov()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvmov A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvmov BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvmov elemento = new Entity.Contabilidad.Acvmov();
            if (!Convert.IsDBNull(row["AcvMovID"]))
            {
                elemento.Acvmovid = row["AcvMovID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["AcvGralID"]))
            {
                elemento.Acvgralid = row["AcvGralID"].ToString();
            }
            if (!Convert.IsDBNull(row["AnoMes"]))
            {
                elemento.Anomes = row["AnoMes"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Pol"]))
            {
                elemento.FecPol = DateTime.Parse(row["Fec_Pol"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Num_Pol"]))
            {
                elemento.NumPol = row["Num_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Num_Renglon"]))
            {
                elemento.NumRenglon = int.Parse(row["Num_Renglon"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Mov"]))
            {
                elemento.TipMov = row["Tip_Mov"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Refer"]))
            {
                elemento.Refer = row["Refer"].ToString();
            }
            if (!Convert.IsDBNull(row["Clase_Conta"]))
            {
                elemento.ClaseConta = row["Clase_Conta"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Tasa_Iva"]))
            {
                elemento.TasaIva = decimal.Parse(row["Tasa_Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Iva"]))
            {
                elemento.Iva = decimal.Parse(row["Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Retencion_Iva"]))
            {
                elemento.RetencionIva = decimal.Parse(row["Retencion_Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Pendiente"]))
            {
                elemento.Pendiente = bool.Parse(row["Pendiente"].ToString());
            }
            if (!Convert.IsDBNull(row["Cod_Flujo"]))
            {
                elemento.CodFlujo = row["Cod_Flujo"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Proveedor"]))
            {
                elemento.CodProveedor = row["Cod_Proveedor"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha_Fiscal"]))
            {
                elemento.FechaFiscal = DateTime.Parse(row["Fecha_Fiscal"].ToString());
            }
            if (!Convert.IsDBNull(row["CtaAux"]))
            {
                elemento.Ctaaux = row["CtaAux"].ToString();
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
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvmov> listaAcvmov)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvmov elemento in listaAcvmov)
                {
                    codigo = elemento.Acvmovid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Acvmov_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     (elemento.Acvgralid != null) ? elemento.Acvgralid : null,
                     elemento.Anomes,
                     elemento.FecPol,
                     elemento.TipPol,
                     elemento.NumPol,
                     elemento.NumRenglon,
                     elemento.TipMov,
                     elemento.Cuenta,
                     elemento.Concepto,
                     elemento.Refer,
                     elemento.ClaseConta,
                     elemento.Importe,
                     elemento.TasaIva,
                     elemento.Iva,
                     elemento.RetencionIva,
                     elemento.Pendiente,
                     elemento.CodFlujo,
                     elemento.CodProveedor,
                     elemento.FechaFiscal,
                     elemento.Ctaaux,
                     elemento.Usuario,
                     elemento.Estatus,
                     elemento.Fecha,
                    ref ultimaAct);
                    elemento.Acvmovid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaAcvmov.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Acvmov> TraerAcvmov()
        {
            IAcvmov proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvmov> listaAcvmov = new ListaDeEntidades<Entity.Contabilidad.Acvmov>();
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet dsAcvmov = proc.Acvmov_Select();
                foreach (DataRow row in dsAcvmov.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvmov elemento = BuildEntity(row, true);
                    listaAcvmov.Add(elemento);
                }
                listaAcvmov.AcceptChanges();
                return listaAcvmov;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvmov TraerAcvmov(string acvmovid)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                Entity.Contabilidad.Acvmov elemento = null;
                DataSet ds = null;
                ds = proc.Acvmov_Select(acvmovid);
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

        public static System.Data.DataSet TraerAcvmovDS()
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.Acvmov_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet ReporteContabilidadExcelDS(string EmpresaId, DateTime FechaInicio, DateTime FechaFin)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.ReporteContabilidadExcel(EmpresaId,FechaInicio, FechaFin);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerFolioMaximoPorTipoPoliza(string tippol, int empresa, DateTime fecha)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                
                DataSet ds = proc.TraerFolioMaximoPorTipoPoliza(tippol, empresa, fecha);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet VerificarFolioPoliza(string NumPol, string TipPol, DateTime FecPol, string EmpresaID, bool Pendiente)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.VerificarFolioPoliza(NumPol, TipPol, FecPol, EmpresaID, Pendiente);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        public static ListaDeEntidades<Entity.Contabilidad.Acvmov> TraerAcvmovPorAcvGral(string acvgralid)
        {
            IAcvmov proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvmov> listaAcvmov = new ListaDeEntidades<Entity.Contabilidad.Acvmov>();
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet dsAcvmov = proc.Acvmov_Select_PorAcvGral(acvgralid);
                foreach (DataRow row in dsAcvmov.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvmov elemento = BuildEntity(row, true);
                    listaAcvmov.Add(elemento);
                }
                listaAcvmov.AcceptChanges();
                return listaAcvmov;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerAcvMovEmpresaCuentaFecha(string EmpresaID, int Anio, string Cuenta)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.AcvMov_Select_EmpresaCuentaFecha(EmpresaID, Anio, Cuenta);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerGatosDelReporteCostos(DateTime fechainicio, DateTime fechafin, string empresaid)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.TraerGatosDelReporteCostos(fechainicio, fechafin, empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet CancelarCuentasResultado(int anio, string empresaid)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.CancelacionCuentasResultado(anio, empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Devuelve una lista de entidades de la tabla AcvMov por medio de una cuenta contable y un folio de cesión, esto para efecto de redondear automáticamente en el cierre diario
        /// </summary>
        /// <param name="cuenta">numero de cuenta contable</param>
        /// <param name="foliocesion">folio de la cesión incluyendo las letras y guiones que genera el sistema por ejemplo R-5153 o E - 12457</param>
        /// <returns></returns>
        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCuentaYReferencia(string cuenta, string refer)
        {
            IAcvmov proc = null;
            try
            {
                List<Entity.Contabilidad.Acvmov> listaAcvmov = new List<Entity.Contabilidad.Acvmov>();
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet dsAcvmov = proc.Acvmov_CuentaRefer_Select(cuenta, refer);
                foreach (DataRow row in dsAcvmov.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvmov elemento = BuildEntity(row, true);
                    listaAcvmov.Add(elemento);
                }
                //listaAcvmov.AcceptChanges();
                return listaAcvmov;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCesion(string concepto, string cuenta, string empresaid)
        {
            IAcvmov proc = null;
            try
            {
                List<Entity.Contabilidad.Acvmov> listaAcvmov = new List<Entity.Contabilidad.Acvmov>();
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet dsAcvmov = proc.TraeMovimientosPorCesion(concepto, cuenta, empresaid);
                foreach (DataRow row in dsAcvmov.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvmov elemento = BuildEntity(row, true);
                    listaAcvmov.Add(elemento);
                }
                //listaAcvmov.AcceptChanges();
                return listaAcvmov;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraeGastos(DateTime fecha, string empresaid)
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.TraeGastos(fecha, empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet TraerFolioMaximoPorTipoPolizaBLT(string tippol, int empresa, DateTime fecha)
        {
            IAcvmov proc = null;
            try
            {
                
                proc = Utilerias.GenerarSprocBLT<IAcvmov>();
                
                DataSet ds = proc.TraerFolioMaximoPorTipoPoliza(tippol, empresa, fecha);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet ConsultarCatDocumentosPolizas()
        {
            IAcvmov proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvmov>();
                DataSet ds = proc.Catdocumentospoliza_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Métodos Públicos
    }
}
