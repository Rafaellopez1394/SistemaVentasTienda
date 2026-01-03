using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatcetestiie
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatcetestiie : ISprocBase
    {
        DataSet Catcetestiie_Select(int? cetetiieid, int? Anio, int? Mes);

        DataSet Catcetestiie_Select();

        int Catcetestiie_Save(
        ref int cetetiieid,
        int año,
        int mes,
        decimal cetes,
        decimal tiie,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatcetestiie

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catcetestiie
    /// </summary>
    public class Catcetestiie
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catcetestiie()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catcetestiie A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catcetestiie BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catcetestiie elemento = new Entity.Contabilidad.Catcetestiie();
            if (!Convert.IsDBNull(row["CeteTiieID"]))
            {
                elemento.Cetetiieid = int.Parse(row["CeteTiieID"].ToString());
            }
            if (!Convert.IsDBNull(row["Año"]))
            {
                elemento.Año = int.Parse(row["Año"].ToString());
            }
            if (!Convert.IsDBNull(row["Mes"]))
            {
                elemento.Mes = int.Parse(row["Mes"].ToString());
            }
            if (!Convert.IsDBNull(row["Cetes"]))
            {
                elemento.Cetes = decimal.Parse(row["Cetes"].ToString());
            }
            if (!Convert.IsDBNull(row["Tiie"]))
            {
                elemento.Tiie = decimal.Parse(row["Tiie"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Catcetestiie> listaCatcetestiie)
        {
            ICatcetestiie proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcetestiie>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catcetestiie elemento in listaCatcetestiie)
                {
                    codigo = elemento.Cetetiieid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catcetestiie_Save(
                    ref codigo,
                     (elemento.Año != null) ? elemento.Año : int.MinValue,
                     (elemento.Mes != null) ? elemento.Mes : int.MinValue,
                     (elemento.Cetes != null) ? elemento.Cetes : decimal.MinValue,
                     (elemento.Tiie != null) ? elemento.Tiie : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Cetetiieid = codigo;
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

        public static List<Entity.Contabilidad.Catcetestiie> TraerCatcetestiie()
        {
            ICatcetestiie proc = null;
            try
            {
                List<Entity.Contabilidad.Catcetestiie> listaCatcetestiie = new List<Entity.Contabilidad.Catcetestiie>();
                proc = Utilerias.GenerarSproc<ICatcetestiie>();
                DataSet dsCatcetestiie = proc.Catcetestiie_Select();
                foreach (DataRow row in dsCatcetestiie.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcetestiie elemento = BuildEntity(row, true);
                    listaCatcetestiie.Add(elemento);
                }                
                return listaCatcetestiie;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catcetestiie TraerCatcetestiie(int? cetetiieid, int? año, int? mes)
        {
            ICatcetestiie proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcetestiie>();
                Entity.Contabilidad.Catcetestiie elemento = null;
                DataSet ds = null;
                ds = proc.Catcetestiie_Select(cetetiieid, año, mes);
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

        public static System.Data.DataSet TraerCatcetestiieDS()
        {
            ICatcetestiie proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcetestiie>();
                DataSet ds = proc.Catcetestiie_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Catcetestiie> TraerCatcetestiiePorAnio(int Anio)
        {
            ICatcetestiie proc = null;
            try
            {
                List<Entity.Contabilidad.Catcetestiie> listaCatcetestiie = new List<Entity.Contabilidad.Catcetestiie>();
                proc = Utilerias.GenerarSproc<ICatcetestiie>();
                DataSet dsCatcetestiie = proc.Catcetestiie_Select(null, Anio, null);
                foreach (DataRow row in dsCatcetestiie.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcetestiie elemento = BuildEntity(row, true);
                    listaCatcetestiie.Add(elemento);
                }
                return listaCatcetestiie;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
