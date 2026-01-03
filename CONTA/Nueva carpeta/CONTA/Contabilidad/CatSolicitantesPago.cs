using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatsolicitantespago
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatsolicitantespago : ISprocBase
    {
        DataSet Catsolicitantespago_Select(string solicitanteid);

        DataSet Catsolicitantespago_Select();

        int Catsolicitantespago_Save(
        ref string solicitanteid,
        string solicitante,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatsolicitantespago

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catsolicitantespago
    /// </summary>
    public class Catsolicitantespago
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catsolicitantespago()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catsolicitantespago A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catsolicitantespago BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catsolicitantespago elemento = new Entity.Contabilidad.Catsolicitantespago();
            if (!Convert.IsDBNull(row["SolicitanteID"]))
            {
                elemento.Solicitanteid = row["SolicitanteID"].ToString();
            }
            if (!Convert.IsDBNull(row["Solicitante"]))
            {
                elemento.Solicitante = row["Solicitante"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Catsolicitantespago> listaCatsolicitantespago)
        {
            ICatsolicitantespago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsolicitantespago>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catsolicitantespago elemento in listaCatsolicitantespago)
                {
                    codigo = elemento.Solicitanteid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catsolicitantespago_Save(
                    ref codigo,
                     (elemento.Solicitante != null) ? elemento.Solicitante : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Solicitanteid = codigo;
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

        public static List<Entity.Contabilidad.Catsolicitantespago> TraerCatsolicitantespago()
        {
            ICatsolicitantespago proc = null;
            try
            {
                List<Entity.Contabilidad.Catsolicitantespago> listaCatsolicitantespago = new List<Entity.Contabilidad.Catsolicitantespago>();
                proc = Utilerias.GenerarSproc<ICatsolicitantespago>();
                DataSet dsCatsolicitantespago = proc.Catsolicitantespago_Select();
                foreach (DataRow row in dsCatsolicitantespago.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catsolicitantespago elemento = BuildEntity(row, true);
                    listaCatsolicitantespago.Add(elemento);
                }
                return listaCatsolicitantespago;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catsolicitantespago TraerCatsolicitantespago(string solicitanteid)
        {
            ICatsolicitantespago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsolicitantespago>();
                Entity.Contabilidad.Catsolicitantespago elemento = null;
                DataSet ds = null;
                ds = proc.Catsolicitantespago_Select(solicitanteid);
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

        public static System.Data.DataSet TraerCatsolicitantespagoDS()
        {
            ICatsolicitantespago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsolicitantespago>();
                DataSet ds = proc.Catsolicitantespago_Select();
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
