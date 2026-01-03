using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatbancoscredito
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatbancoscredito : ISprocBase
    {
        DataSet Catbancoscredito_Select(int bancocreditoid);

        DataSet Catbancoscredito_Select();

        int Catbancoscredito_Save(
        ref int bancocreditoid,
        string EmpresaID,
        string banco,
        string cuentacapital,
        string cuentaintereses,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatbancoscredito

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catbancoscredito
    /// </summary>
    public class Catbancoscredito
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catbancoscredito()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catbancoscreditos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catbancoscredito BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catbancoscredito elemento = new Entity.Contabilidad.Catbancoscredito();
            if (!Convert.IsDBNull(row["BancoCreditoID"]))
            {
                elemento.Bancocreditoid = int.Parse(row["BancoCreditoID"].ToString());
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Banco"]))
            {
                elemento.Banco = row["Banco"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaCapital"]))
            {
                elemento.Cuentacapital = row["CuentaCapital"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaIntereses"]))
            {
                elemento.Cuentaintereses = row["CuentaIntereses"].ToString();
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
        public static void Guardar(ref List<Entity.Contabilidad.Catbancoscredito> listaCatbancoscreditos)
        {
            ICatbancoscredito proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatbancoscredito>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catbancoscredito elemento in listaCatbancoscreditos)
                {
                    codigo = elemento.Bancocreditoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catbancoscredito_Save(
                    ref codigo,
                     (elemento.Banco != null) ? elemento.Banco : null,
                     elemento.Empresaid,
                     (elemento.Cuentacapital != null) ? elemento.Cuentacapital : null,
                     (elemento.Cuentaintereses != null) ? elemento.Cuentaintereses : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Bancocreditoid = codigo;
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

        public static List<Entity.Contabilidad.Catbancoscredito> TraerCatbancoscreditos()
        {
            ICatbancoscredito proc = null;
            try
            {
                List<Entity.Contabilidad.Catbancoscredito> listaCatbancoscreditos = new List<Entity.Contabilidad.Catbancoscredito>();
                proc = Utilerias.GenerarSproc<ICatbancoscredito>();
                DataSet dsCatbancoscreditos = proc.Catbancoscredito_Select();
                foreach (DataRow row in dsCatbancoscreditos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catbancoscredito elemento = BuildEntity(row, true);
                    listaCatbancoscreditos.Add(elemento);
                }                
                return listaCatbancoscreditos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catbancoscredito TraerCatbancoscreditos(int bancocreditoid)
        {
            ICatbancoscredito proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatbancoscredito>();
                Entity.Contabilidad.Catbancoscredito elemento = null;
                DataSet ds = null;
                ds = proc.Catbancoscredito_Select(bancocreditoid);
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

        public static System.Data.DataSet TraerCatbancoscreditosDS()
        {
            ICatbancoscredito proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatbancoscredito>();
                DataSet ds = proc.Catbancoscredito_Select();
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
