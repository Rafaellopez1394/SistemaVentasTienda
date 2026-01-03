using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatcuentaspersonal
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatcuentaspersonal : ISprocBase
    {
        DataSet Catcuentaspersonal_Select(string cuentapersonalid);

        DataSet Catcuentaspersonal_Select();

        

        int Catcuentaspersonal_Save(
        ref string cuentapersonalid,
        string empresaid,
        string cuentaid,
        int codEmpresa,
        string cuenta,
        int nivel,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);

        DataSet TraerGastosPersonales(string EmpresaID, string CuentaIni, string CuentaFin, DateTime FechaInicial, DateTime FechaFinal);

        DataSet TraerPresupuestoContable(string EmpresaID, int anio, int operativo);

        DataSet TraerPresupuestoContableVsReal(string EmpresaID, DateTime Fecha, int Operativo);

        DataSet spcgenerainformeProyeccionComparativoEntreYEars2(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mesfin, int opcion);
        DataSet spcgenerainformeProyeccionMensual(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mesfin, int opcion);
        

    }

    #endregion //Interfaz ICatcuentaspersonal

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catcuentaspersonal
    /// </summary>
    public class Catcuentaspersonal
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catcuentaspersonal()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catcuentaspersonales A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catcuentaspersonal BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catcuentaspersonal elemento = new Entity.Contabilidad.Catcuentaspersonal();
            if (!Convert.IsDBNull(row["CuentaPersonalID"]))
            {
                elemento.Cuentapersonalid = row["CuentaPersonalID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaID"]))
            {
                elemento.Cuentaid = row["CuentaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = int.Parse(row["Cod_Empresa"].ToString());
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Nivel"]))
            {
                elemento.Nivel = int.Parse(row["Nivel"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Catcuentaspersonal> listaCatcuentaspersonales)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catcuentaspersonal elemento in listaCatcuentaspersonales)
                {
                    codigo = elemento.Cuentapersonalid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catcuentaspersonal_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Cuentaid != null) ? elemento.Cuentaid : null,
                     (elemento.CodEmpresa != null) ? elemento.CodEmpresa : int.MinValue,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Nivel != null) ? elemento.Nivel : int.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    ref ultimaAct);
                    elemento.Cuentapersonalid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
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

        public static List<Entity.Contabilidad.Catcuentaspersonal> TraerCatcuentaspersonales()
        {
            ICatcuentaspersonal proc = null;
            try
            {
                List<Entity.Contabilidad.Catcuentaspersonal> listaCatcuentaspersonales = new List<Entity.Contabilidad.Catcuentaspersonal>();
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet dsCatcuentaspersonales = proc.Catcuentaspersonal_Select();
                foreach (DataRow row in dsCatcuentaspersonales.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuentaspersonal elemento = BuildEntity(row, true);
                    listaCatcuentaspersonales.Add(elemento);
                }                
                return listaCatcuentaspersonales;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catcuentaspersonal TraerCatcuentaspersonales(string cuentapersonalid)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                Entity.Contabilidad.Catcuentaspersonal elemento = null;
                DataSet ds = null;
                ds = proc.Catcuentaspersonal_Select(cuentapersonalid);
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

        public static System.Data.DataSet TraerCatcuentaspersonalesDS()
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.Catcuentaspersonal_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerGastosPersonales(string EmpresaID, string CuentaIni, string CuentaFin, DateTime FechaInicial, DateTime FechaFinal)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.TraerGastosPersonales(EmpresaID, CuentaIni, CuentaFin, FechaInicial, FechaFinal);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerPresupuestoContable(string EmpresaID, int Anio, int Operativo)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.TraerPresupuestoContable(EmpresaID, Anio, Operativo);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerPresupuestoContableVsReal(string EmpresaID, DateTime Fecha, int Operativo)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.TraerPresupuestoContableVsReal(EmpresaID, Fecha, Operativo);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerInformeProyeccionComparativoEntreYEars2(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.spcgenerainformeProyeccionComparativoEntreYEars2(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static System.Data.DataSet TraerInformeProyeccionMensual(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {
            ICatcuentaspersonal proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentaspersonal>();
                DataSet ds = proc.spcgenerainformeProyeccionMensual(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);
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
