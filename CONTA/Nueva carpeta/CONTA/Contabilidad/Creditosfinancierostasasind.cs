using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICreditosfinancierostasasind
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICreditosfinancierostasasind : ISprocBase
    {
        DataSet Creditosfinancierostasasind_Select(string creditosfinancierostasatiieid, int? CreditoFinancieroID, int? Anio, int? Mes);

        DataSet Creditosfinancierostasasind_Select();

        int Creditosfinancierostasasind_Save(
        ref string creditosfinancierostasatiieid,
        int creditofinancieroid,
        int año,
        int mes,
        decimal tasa,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICreditosfinancierostasasind

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Creditosfinancierostasasind
    /// </summary>
    public class Creditosfinancierostasasind
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Creditosfinancierostasasind()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Creditosfinancierostasasind A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Creditosfinancierostasasind BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Creditosfinancierostasasind elemento = new Entity.Contabilidad.Creditosfinancierostasasind();
            if (!Convert.IsDBNull(row["CreditosFinancierosTasaTiieID"]))
            {
                elemento.Creditosfinancierostasatiieid = row["CreditosFinancierosTasaTiieID"].ToString();
            }
            if (!Convert.IsDBNull(row["CreditoFinancieroID"]))
            {
                elemento.Creditofinancieroid = int.Parse(row["CreditoFinancieroID"].ToString());
            }
            if (!Convert.IsDBNull(row["Año"]))
            {
                elemento.Año = int.Parse(row["Año"].ToString());
            }
            if (!Convert.IsDBNull(row["Mes"]))
            {
                elemento.Mes = int.Parse(row["Mes"].ToString());
            }
            if (!Convert.IsDBNull(row["Tasa"]))
            {
                elemento.Tasa = decimal.Parse(row["Tasa"].ToString());
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
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> listaCreditosfinancierostasasind)
        {
            ICreditosfinancierostasasind proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierostasasind>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Creditosfinancierostasasind elemento in listaCreditosfinancierostasasind)
                {
                    codigo = elemento.Creditosfinancierostasatiieid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Creditosfinancierostasasind_Save(
                    ref codigo,
                     (elemento.Creditofinancieroid != null) ? elemento.Creditofinancieroid : int.MinValue,
                     (elemento.Año != null) ? elemento.Año : int.MinValue,
                     (elemento.Mes != null) ? elemento.Mes : int.MinValue,
                     (elemento.Tasa != null) ? elemento.Tasa : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Creditosfinancierostasatiieid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaCreditosfinancierostasasind.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> TraerCreditosfinancierostasasind()
        {
            ICreditosfinancierostasasind proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> listaCreditosfinancierostasasind = new ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind>();
                proc = Utilerias.GenerarSproc<ICreditosfinancierostasasind>();
                DataSet dsCreditosfinancierostasasind = proc.Creditosfinancierostasasind_Select();
                foreach (DataRow row in dsCreditosfinancierostasasind.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinancierostasasind elemento = BuildEntity(row, true);
                    listaCreditosfinancierostasasind.Add(elemento);
                }
                listaCreditosfinancierostasasind.AcceptChanges();
                return listaCreditosfinancierostasasind;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Creditosfinancierostasasind TraerCreditosfinancierostasasind(string creditosfinancierostasatiieid, int? CreditoFinancieroID, int? Anio, int? Mes)
        {
            ICreditosfinancierostasasind proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierostasasind>();
                Entity.Contabilidad.Creditosfinancierostasasind elemento = null;
                DataSet ds = null;
                ds = proc.Creditosfinancierostasasind_Select(creditosfinancierostasatiieid, CreditoFinancieroID, Anio, Mes);
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

        public static System.Data.DataSet TraerCreditosfinancierostasasindDS()
        {
            ICreditosfinancierostasasind proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierostasasind>();
                DataSet ds = proc.Creditosfinancierostasasind_Select();
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
