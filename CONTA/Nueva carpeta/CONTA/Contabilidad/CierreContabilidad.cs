using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICierrecontabilidad
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICierrecontabilidad : ISprocBase
    {
        DataSet Cierrecontabilidad_Select(int? cierrecontableid, string empresaid);

        DataSet Cierrecontabilidad_Select();

        int Cierrecontabilidad_Save(
        ref int cierrecontableid,
        string empresaid,
        DateTime fechacierre,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICierrecontabilidad

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Cierrecontabilidad
    /// </summary>
    public class Cierrecontabilidad
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Cierrecontabilidad()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Cierrecontabilidad A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Cierrecontabilidad BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Cierrecontabilidad elemento = new Entity.Contabilidad.Cierrecontabilidad();
            if (!Convert.IsDBNull(row["CierreContableID"]))
            {
                elemento.Cierrecontableid = int.Parse(row["CierreContableID"].ToString());
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaCierre"]))
            {
                elemento.Fechacierre = DateTime.Parse(row["FechaCierre"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Cierrecontabilidad> listaCierrecontabilidad)
        {
            ICierrecontabilidad proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICierrecontabilidad>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Cierrecontabilidad elemento in listaCierrecontabilidad)
                {
                    codigo = elemento.Cierrecontableid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Cierrecontabilidad_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Fechacierre != null) ? elemento.Fechacierre : DateTime.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Cierrecontableid = codigo;
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

        public static List<Entity.Contabilidad.Cierrecontabilidad> TraerCierrecontabilidad()
        {
            ICierrecontabilidad proc = null;
            try
            {
                List<Entity.Contabilidad.Cierrecontabilidad> listaCierrecontabilidad = new List<Entity.Contabilidad.Cierrecontabilidad>();
                proc = Utilerias.GenerarSproc<ICierrecontabilidad>();
                DataSet dsCierrecontabilidad = proc.Cierrecontabilidad_Select();
                foreach (DataRow row in dsCierrecontabilidad.Tables[0].Rows)
                {
                    Entity.Contabilidad.Cierrecontabilidad elemento = BuildEntity(row, true);
                    listaCierrecontabilidad.Add(elemento);
                }
                return listaCierrecontabilidad;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(int cierrecontableid)
        {
            ICierrecontabilidad proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICierrecontabilidad>();
                Entity.Contabilidad.Cierrecontabilidad elemento = null;
                DataSet ds = null;
                ds = proc.Cierrecontabilidad_Select(cierrecontableid, null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(string empresaid)
        {
            ICierrecontabilidad proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICierrecontabilidad>();
                Entity.Contabilidad.Cierrecontabilidad elemento = null;
                DataSet ds = null;
                ds = proc.Cierrecontabilidad_Select(null, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerCierrecontabilidadDS()
        {
            ICierrecontabilidad proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICierrecontabilidad>();
                DataSet ds = proc.Cierrecontabilidad_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidadBLT(string empresaid)
        {
            ICierrecontabilidad proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICierrecontabilidad>();
                
                Entity.Contabilidad.Cierrecontabilidad elemento = null;
                DataSet ds = null;
                ds = proc.Cierrecontabilidad_Select(null, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
