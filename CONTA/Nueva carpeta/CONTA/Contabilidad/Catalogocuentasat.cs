using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatalogocuentasat
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatalogocuentasat : ISprocBase
    {
        DataSet Catalogocuentasat_Select(string ctasat, string Descripcion);

        DataSet Catalogocuentasat_Select();

        int Catalogocuentasat_Save(
        ref string ctasat,
        string descripcion,
        int estatus,
        DateTime fecha,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatalogocuentasat

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catalogocuentasat
    /// </summary>
    public class Catalogocuentasat
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catalogocuentasat()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catalogocuentasat A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catalogocuentasat BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catalogocuentasat elemento = new Entity.Contabilidad.Catalogocuentasat();
            if (!Convert.IsDBNull(row["CtaSat"]))
            {
                elemento.Ctasat = row["CtaSat"].ToString();
            }
            if (!Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Catalogocuentasat> listaCatalogocuentasat)
        {
            ICatalogocuentasat proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatalogocuentasat>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catalogocuentasat elemento in listaCatalogocuentasat)
                {
                    codigo = elemento.Ctasat;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catalogocuentasat_Save(
                    ref codigo,
                     (elemento.Descripcion != null) ? elemento.Descripcion : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Ctasat = codigo;
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

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasat()
        {
            ICatalogocuentasat proc = null;
            try
            {
                List<Entity.Contabilidad.Catalogocuentasat> listaCatalogocuentasat = new List<Entity.Contabilidad.Catalogocuentasat>();
                proc = Utilerias.GenerarSproc<ICatalogocuentasat>();
                DataSet dsCatalogocuentasat = proc.Catalogocuentasat_Select();
                foreach (DataRow row in dsCatalogocuentasat.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catalogocuentasat elemento = BuildEntity(row, true);
                    listaCatalogocuentasat.Add(elemento);
                }                
                return listaCatalogocuentasat;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasatPorDescripcion(string descripcion)
        {
            ICatalogocuentasat proc = null;
            try
            {
                List<Entity.Contabilidad.Catalogocuentasat> listaCatalogocuentasat = new List<Entity.Contabilidad.Catalogocuentasat>();
                proc = Utilerias.GenerarSproc<ICatalogocuentasat>();
                DataSet dsCatalogocuentasat = proc.Catalogocuentasat_Select(null, descripcion);
                foreach (DataRow row in dsCatalogocuentasat.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catalogocuentasat elemento = BuildEntity(row, true);
                    listaCatalogocuentasat.Add(elemento);
                }
                return listaCatalogocuentasat;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catalogocuentasat TraerCatalogocuentasat(string ctasat)
        {
            ICatalogocuentasat proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatalogocuentasat>();
                Entity.Contabilidad.Catalogocuentasat elemento = null;
                DataSet ds = null;
                ds = proc.Catalogocuentasat_Select(ctasat, null);
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

        public static System.Data.DataSet TraerCatalogocuentasatDS()
        {
            ICatalogocuentasat proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatalogocuentasat>();
                DataSet ds = proc.Catalogocuentasat_Select();
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
