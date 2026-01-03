using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatcierre
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatcierre : ISprocBase
    {
        DataSet Catcierre_Select(string cierreid, string EmpresaID, DateTime FechaCierre);
        DataSet Catcierre_Select();

        DataSet GenerarAlertasB1();
        DataSet GenerarAlertasB1(string VendedorID);
        DataSet GenerarNotificacionesA1();


        int Catcierre_Save(
        ref string cierreid,
        string empresaid,
        string anomes,
        DateTime fechacierre,
        string tipPol,
        string numPol,
        decimal intereses,
        int estatus,
        string usuario,
        DateTime fecha,
        ref int ultimaAct);

        DataSet TraerUltimoCierre(string EmpresaID);

    }

    #endregion //Interfaz ICatcierre

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catcierre
    /// </summary>
    public class Catcierre
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catcierre()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catcierre A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catcierre BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catcierre elemento = new Entity.Contabilidad.Catcierre();
            if (!Convert.IsDBNull(row["CierreID"]))
            {
                elemento.Cierreid = row["CierreID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["AnoMes"]))
            {
                elemento.Anomes = row["AnoMes"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaCierre"]))
            {
                elemento.Fechacierre = DateTime.Parse(row["FechaCierre"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_pol"]))
            {
                elemento.TipPol = row["Tip_pol"].ToString();
            }
            if (!Convert.IsDBNull(row["num_pol"]))
            {
                elemento.NumPol = row["num_pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Intereses"]))
            {
                elemento.Intereses = decimal.Parse(row["Intereses"].ToString());
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
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
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Catcierre> listaCatcierre)
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catcierre elemento in listaCatcierre)
                {
                    codigo = elemento.Cierreid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catcierre_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Anomes != null) ? elemento.Anomes : null,
                     (elemento.Fechacierre != null) ? elemento.Fechacierre : DateTime.MinValue,
                     (elemento.TipPol != null) ? elemento.TipPol : null,
                     (elemento.NumPol != null) ? elemento.NumPol : null,
                     (elemento.Intereses != null) ? elemento.Intereses : decimal.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    ref ultimaAct);
                    elemento.Cierreid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaCatcierre.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Catcierre> TraerCatcierre()
        {
            ICatcierre proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcierre> listaCatcierre = new ListaDeEntidades<Entity.Contabilidad.Catcierre>();
                proc = Utilerias.GenerarSproc<ICatcierre>();
                DataSet dsCatcierre = proc.Catcierre_Select();
                foreach (DataRow row in dsCatcierre.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcierre elemento = BuildEntity(row, true);
                    listaCatcierre.Add(elemento);
                }
                listaCatcierre.AcceptChanges();
                return listaCatcierre;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catcierre TraerCatcierre(string cierreid,string empresaid,DateTime fecha)
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>();
                Entity.Contabilidad.Catcierre elemento = null;
                DataSet ds = null;
                ds = proc.Catcierre_Select(cierreid, empresaid, fecha);
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

        public static System.Data.DataSet TraerCatcierreDS()
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>();
                DataSet ds = proc.Catcierre_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static System.Data.DataSet TraerUltimoCierre(string EmpresaID)
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>();
                DataSet ds = proc.TraerUltimoCierre(EmpresaID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerAlertasNotificarB1(string VendedorID)
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>();
                DataSet ds = proc.GenerarAlertasB1();
                return ds;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerAlertasNotificarA1(string VendedorID)
        {
            ICatcierre proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcierre>();
                DataSet ds = proc.GenerarNotificacionesA1();
                return ds;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        


        #endregion Métodos Públicos
    }
}
