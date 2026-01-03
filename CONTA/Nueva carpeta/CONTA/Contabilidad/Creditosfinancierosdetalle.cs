using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICreditosfinancierosdetalle
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICreditosfinancierosdetalle : ISprocBase
    {
        DataSet Creditosfinancierosdetalle_Select(string creditofinancierodetalleid);

        DataSet Creditosfinancierosdetalle_Select();

        DataSet Creditosfinancierosdetalle_SelectTipoFecha(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha);
        DataSet TraerDetalleCreditosFinancierosContabilidad(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha);


        int Creditosfinancierosdetalle_Save(
        ref string creditofinancierodetalleid,
        int creditofinancieroid,
        int tipoMov,
        string bancoid,
        string concepto,
        DateTime fechaApli,
        decimal financiamiento,
        decimal interes,
        decimal iva,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICreditosfinancierosdetalle

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Creditosfinancierosdetalle
    /// </summary>
    public class Creditosfinancierosdetalle
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Creditosfinancierosdetalle()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Creditosfinancierosdetalle A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Creditosfinancierosdetalle BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Creditosfinancierosdetalle elemento = new Entity.Contabilidad.Creditosfinancierosdetalle();
            if (!Convert.IsDBNull(row["CreditoFinancieroDetalleID"]))
            {
                elemento.Creditofinancierodetalleid = row["CreditoFinancieroDetalleID"].ToString();
            }
            if (!Convert.IsDBNull(row["CreditoFinancieroID"]))
            {
                elemento.Creditofinancieroid = int.Parse(row["CreditoFinancieroID"].ToString());
            }
            if (!Convert.IsDBNull(row["Tipo_Mov"]))
            {
                elemento.TipoMov = int.Parse(row["Tipo_Mov"].ToString());
            }
            if (!Convert.IsDBNull(row["BancoID"]))
            {
                elemento.Bancoid = row["BancoID"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha_Apli"]))
            {
                elemento.FechaApli = DateTime.Parse(row["Fecha_Apli"].ToString());
            }
            if (!Convert.IsDBNull(row["Financiamiento"]))
            {
                elemento.Financiamiento = decimal.Parse(row["Financiamiento"].ToString());
            }
            if (!Convert.IsDBNull(row["Interes"]))
            {
                elemento.Interes = decimal.Parse(row["Interes"].ToString());
            }
            if (!Convert.IsDBNull(row["Iva"]))
            {
                elemento.Iva = decimal.Parse(row["Iva"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Creditosfinancierosdetalle> listaCreditosfinancierosdetalle)
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Creditosfinancierosdetalle elemento in listaCreditosfinancierosdetalle)
                {
                    codigo = elemento.Creditofinancierodetalleid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Creditosfinancierosdetalle_Save(
                    ref codigo,
                     (elemento.Creditofinancieroid != null) ? elemento.Creditofinancieroid : int.MinValue,
                     (elemento.TipoMov != null) ? elemento.TipoMov : int.MinValue,
                     (elemento.Bancoid != null) ? elemento.Bancoid : null,
                     (elemento.Concepto != null) ? elemento.Concepto : null,
                     (elemento.FechaApli != null) ? elemento.FechaApli : DateTime.MinValue,
                     (elemento.Financiamiento != null) ? elemento.Financiamiento : decimal.MinValue,
                     (elemento.Interes != null) ? elemento.Interes : decimal.MinValue,
                     (elemento.Iva != null) ? elemento.Iva : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Creditofinancierodetalleid = codigo;
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

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> TraerCreditosfinancierosdetalle()
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                List<Entity.Contabilidad.Creditosfinancierosdetalle> listaCreditosfinancierosdetalle = new List<Entity.Contabilidad.Creditosfinancierosdetalle>();
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>();
                DataSet dsCreditosfinancierosdetalle = proc.Creditosfinancierosdetalle_Select();
                foreach (DataRow row in dsCreditosfinancierosdetalle.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinancierosdetalle elemento = BuildEntity(row, true);
                    listaCreditosfinancierosdetalle.Add(elemento);
                }                
                return listaCreditosfinancierosdetalle;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Creditosfinancierosdetalle TraerCreditosfinancierosdetalle(string creditofinancierodetalleid)
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>();
                Entity.Contabilidad.Creditosfinancierosdetalle elemento = null;
                DataSet ds = null;
                ds = proc.Creditosfinancierosdetalle_Select(creditofinancierodetalleid);
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

        public static System.Data.DataSet TraerCreditosfinancierosdetalleDS()
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>();
                DataSet ds = proc.Creditosfinancierosdetalle_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> Creditosfinancierosdetalle_SelectTipoFecha(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                List<Entity.Contabilidad.Creditosfinancierosdetalle> listaCreditosfinancierosdetalle = new List<Entity.Contabilidad.Creditosfinancierosdetalle>();
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>();
                DataSet dsCreditosfinancierosdetalle = proc.Creditosfinancierosdetalle_SelectTipoFecha(CreditoFinancieroID, Tipo_Mov, Fecha);
                foreach (DataRow row in dsCreditosfinancierosdetalle.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinancierosdetalle elemento = BuildEntity(row, true);
                    listaCreditosfinancierosdetalle.Add(elemento);
                }
                return listaCreditosfinancierosdetalle;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> TraerDetalleCreditosFinancierosContabilidad(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            ICreditosfinancierosdetalle proc = null;
            try
            {
                List<Entity.Contabilidad.Creditosfinancierosdetalle> listaCreditosfinancierosdetalle = new List<Entity.Contabilidad.Creditosfinancierosdetalle>();
                proc = Utilerias.GenerarSproc<ICreditosfinancierosdetalle>();
                DataSet dsCreditosfinancierosdetalle = proc.TraerDetalleCreditosFinancierosContabilidad(CreditoFinancieroID, Tipo_Mov, Fecha);
                foreach (DataRow row in dsCreditosfinancierosdetalle.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinancierosdetalle elemento = BuildEntity(row, true);
                    listaCreditosfinancierosdetalle.Add(elemento);
                }
                return listaCreditosfinancierosdetalle;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Métodos Públicos
    }
}
