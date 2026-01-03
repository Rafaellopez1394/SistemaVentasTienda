using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICreditosfinanciero
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICreditosfinanciero : ISprocBase
    {
        DataSet Creditosfinanciero_Select(int creditofinancieroid);

        DataSet Creditosfinanciero_Select();

        DataSet Creditosfinanciero_Select(DateTime FechaFinal);

        int Creditosfinanciero_Save(
        ref int creditofinancieroid,
        string empresaid,
        int bancocreditoid,
        string contrato,
        string moneda,
        decimal importe,
        string calculointeres,
        DateTime fechainicial,
        DateTime fechavence,
        string tipocredito,
        decimal tasainteres,
        decimal puntos,
        decimal diasañotasa,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICreditosfinanciero

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Creditosfinanciero
    /// </summary>
    public class Creditosfinanciero
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Creditosfinanciero()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Creditosfinancieros A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Creditosfinanciero BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Creditosfinanciero elemento = new Entity.Contabilidad.Creditosfinanciero();
            if (!Convert.IsDBNull(row["CreditoFinancieroID"]))
            {
                elemento.Creditofinancieroid = int.Parse(row["CreditoFinancieroID"].ToString());
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["BancoCreditoID"]))
            {
                elemento.Bancocreditoid = int.Parse(row["BancoCreditoID"].ToString());
            }
            if (!Convert.IsDBNull(row["Contrato"]))
            {
                elemento.Contrato = row["Contrato"].ToString();
            }
            if (!Convert.IsDBNull(row["Moneda"]))
            {
                elemento.Moneda = row["Moneda"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["CalculoInteres"]))
            {
                elemento.Calculointeres = row["CalculoInteres"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaInicial"]))
            {
                elemento.Fechainicial = DateTime.Parse(row["FechaInicial"].ToString());
            }
            if (!Convert.IsDBNull(row["FechaVence"]))
            {
                elemento.Fechavence = DateTime.Parse(row["FechaVence"].ToString());
            }
            if (!Convert.IsDBNull(row["TipoCredito"]))
            {
                elemento.Tipocredito = row["TipoCredito"].ToString();
            }
            if (!Convert.IsDBNull(row["TasaInteres"]))
            {
                elemento.Tasainteres = decimal.Parse(row["TasaInteres"].ToString());
            }
            if (!Convert.IsDBNull(row["Puntos"]))
            {
                elemento.Puntos = decimal.Parse(row["Puntos"].ToString());
            }
            if (!Convert.IsDBNull(row["DiasAñoTasa"]))
            {
                elemento.Diasañotasa = decimal.Parse(row["DiasAñoTasa"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["FechaCorte"]))
            {
                elemento.FechaCorte = DateTime.Parse(row["FechaCorte"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Creditosfinanciero> listaCreditosfinancieros)
        {
            ICreditosfinanciero proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinanciero>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Creditosfinanciero elemento in listaCreditosfinancieros)
                {
                    codigo = elemento.Creditofinancieroid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Creditosfinanciero_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Bancocreditoid != null) ? elemento.Bancocreditoid : int.MinValue,
                     (elemento.Contrato != null) ? elemento.Contrato : null,
                     (elemento.Moneda != null) ? elemento.Moneda : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Calculointeres != null) ? elemento.Calculointeres : null,
                     (elemento.Fechainicial != null) ? elemento.Fechainicial : DateTime.MinValue,
                     (elemento.Fechavence != null) ? elemento.Fechavence : DateTime.MinValue,
                     (elemento.Tipocredito != null) ? elemento.Tipocredito : null,
                     (elemento.Tasainteres != null) ? elemento.Tasainteres : decimal.MinValue,
                     (elemento.Puntos != null) ? elemento.Puntos : decimal.MinValue,
                     (elemento.Diasañotasa != null) ? elemento.Diasañotasa : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Creditofinancieroid = codigo;
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

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancieros()
        {
            ICreditosfinanciero proc = null;
            try
            {
                List<Entity.Contabilidad.Creditosfinanciero> listaCreditosfinancieros = new List<Entity.Contabilidad.Creditosfinanciero>();
                proc = Utilerias.GenerarSproc<ICreditosfinanciero>();
                DataSet dsCreditosfinancieros = proc.Creditosfinanciero_Select();
                foreach (DataRow row in dsCreditosfinancieros.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinanciero elemento = BuildEntity(row, true);
                    listaCreditosfinancieros.Add(elemento);
                }                
                return listaCreditosfinancieros;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Creditosfinanciero TraerCreditosfinancieros(int creditofinancieroid)
        {
            ICreditosfinanciero proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinanciero>();
                Entity.Contabilidad.Creditosfinanciero elemento = null;
                DataSet ds = null;
                ds = proc.Creditosfinanciero_Select(creditofinancieroid);
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

        public static System.Data.DataSet TraerCreditosfinancierosDS()
        {
            ICreditosfinanciero proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICreditosfinanciero>();
                DataSet ds = proc.Creditosfinanciero_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancierosPorFecha(DateTime FechaFinal)
        {
            ICreditosfinanciero proc = null;
            try
            {
                List<Entity.Contabilidad.Creditosfinanciero> listaCreditosfinancieros = new List<Entity.Contabilidad.Creditosfinanciero>();
                proc = Utilerias.GenerarSproc<ICreditosfinanciero>();
                DataSet dsCreditosfinancieros = proc.Creditosfinanciero_Select(FechaFinal);
                foreach (DataRow row in dsCreditosfinancieros.Tables[0].Rows)
                {
                    Entity.Contabilidad.Creditosfinanciero elemento = BuildEntity(row, true);
                    listaCreditosfinancieros.Add(elemento);
                }
                return listaCreditosfinancieros;
            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion Métodos Públicos
    }
}
