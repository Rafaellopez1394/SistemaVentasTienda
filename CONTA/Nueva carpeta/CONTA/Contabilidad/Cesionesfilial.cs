using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICesionesfilial
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICesionesfilial : ISprocBase
    {
        DataSet Cesionesfilial_Select(string cesionid);

        DataSet Cesionesfilial_Select();

        int Cesionesfilial_Save(
        ref string cesionid,
        string empresaid,
        string clienteid,
        int folio,
        DateTime fechaDocu,
        DateTime fechaVence,
        string tipo,
        decimal linea,
        decimal disposiciones,
        decimal abonado,
        decimal tasaanual,
        decimal puntos,
        decimal diasaño,
        int estatus,
        string usuario,
        DateTime fecha,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICesionesfilial

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Cesionesfilial
    /// </summary>
    public class Cesionesfilial
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Cesionesfilial()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Cesionesfilial A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Cesionesfilial BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Cesionesfilial elemento = new Entity.Contabilidad.Cesionesfilial();
            if (!Convert.IsDBNull(row["CesionID"]))
            {
                elemento.Cesionid = row["CesionID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["ClienteID"]))
            {
                elemento.Clienteid = row["ClienteID"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio"]))
            {
                elemento.Folio = int.Parse(row["Folio"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha_Docu"]))
            {
                elemento.FechaDocu = DateTime.Parse(row["Fecha_Docu"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha_Vence"]))
            {
                elemento.FechaVence = DateTime.Parse(row["Fecha_Vence"].ToString());
            }
            if (!Convert.IsDBNull(row["Tipo"]))
            {
                elemento.Tipo = row["Tipo"].ToString();
            }
            if (!Convert.IsDBNull(row["Linea"]))
            {
                elemento.Linea = decimal.Parse(row["Linea"].ToString());
            }
            if (!Convert.IsDBNull(row["Disposiciones"]))
            {
                elemento.Disposiciones = decimal.Parse(row["Disposiciones"].ToString());
            }
            if (!Convert.IsDBNull(row["Abonado"]))
            {
                elemento.Abonado = decimal.Parse(row["Abonado"].ToString());
            }
            if (!Convert.IsDBNull(row["TasaAnual"]))
            {
                elemento.Tasaanual = decimal.Parse(row["TasaAnual"].ToString());
            }
            if (!Convert.IsDBNull(row["Puntos"]))
            {
                elemento.Puntos = decimal.Parse(row["Puntos"].ToString());
            }
            if (!Convert.IsDBNull(row["DiasAño"]))
            {
                elemento.Diasaño = decimal.Parse(row["DiasAño"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Cesionesfilial> listaCesionesfilial)
        {
            ICesionesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICesionesfilial>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Cesionesfilial elemento in listaCesionesfilial)
                {
                    codigo = elemento.Cesionid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Cesionesfilial_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Clienteid != null) ? elemento.Clienteid : null,
                     (elemento.Folio != null) ? elemento.Folio : int.MinValue,
                     (elemento.FechaDocu != null) ? elemento.FechaDocu : DateTime.MinValue,
                     (elemento.FechaVence != null) ? elemento.FechaVence : DateTime.MinValue,
                     (elemento.Tipo != null) ? elemento.Tipo : null,
                     (elemento.Linea != null) ? elemento.Linea : decimal.MinValue,
                     (elemento.Disposiciones != null) ? elemento.Disposiciones : decimal.MinValue,
                     (elemento.Abonado != null) ? elemento.Abonado : decimal.MinValue,
                     (elemento.Tasaanual != null) ? elemento.Tasaanual : decimal.MinValue,
                     (elemento.Puntos != null) ? elemento.Puntos : decimal.MinValue,
                     (elemento.Diasaño != null) ? elemento.Diasaño : decimal.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    ref ultimaAct);
                    elemento.Cesionid = codigo;
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

        public static List<Entity.Contabilidad.Cesionesfilial> TraerCesionesfilial()
        {
            ICesionesfilial proc = null;
            try
            {
                List<Entity.Contabilidad.Cesionesfilial> listaCesionesfilial = new List<Entity.Contabilidad.Cesionesfilial>();
                proc = Utilerias.GenerarSproc<ICesionesfilial>();
                DataSet dsCesionesfilial = proc.Cesionesfilial_Select();
                foreach (DataRow row in dsCesionesfilial.Tables[0].Rows)
                {
                    Entity.Contabilidad.Cesionesfilial elemento = BuildEntity(row, true);
                    listaCesionesfilial.Add(elemento);
                }                
                return listaCesionesfilial;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Cesionesfilial TraerCesionesfilial(string cesionid)
        {
            ICesionesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICesionesfilial>();
                Entity.Contabilidad.Cesionesfilial elemento = null;
                DataSet ds = null;
                ds = proc.Cesionesfilial_Select(cesionid);
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

        public static System.Data.DataSet TraerCesionesfilialDS()
        {
            ICesionesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICesionesfilial>();
                DataSet ds = proc.Cesionesfilial_Select();
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
