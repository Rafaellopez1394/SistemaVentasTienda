using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatfacturasproveedorpago
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatfacturasproveedorpago : ISprocBase
    {
        DataSet Catfacturasproveedorpago_Select(string facturaproveedorpagoid);

        DataSet Catfacturasproveedorpago_Select();

        int Catfacturasproveedorpago_Save(
        ref string facturaproveedorpagoid,
        string facturaproveedorid,
        string pagoid,
        decimal importe,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatfacturasproveedorpago

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catfacturasproveedorpago
    /// </summary>
    public class Catfacturasproveedorpago
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catfacturasproveedorpago()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catfacturasproveedorpagos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catfacturasproveedorpago BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catfacturasproveedorpago elemento = new Entity.Contabilidad.Catfacturasproveedorpago();
            if (!Convert.IsDBNull(row["FacturaProveedorPagoID"]))
            {
                elemento.Facturaproveedorpagoid = row["FacturaProveedorPagoID"].ToString();
            }
            if (!Convert.IsDBNull(row["FacturaProveedorID"]))
            {
                elemento.Facturaproveedorid = row["FacturaProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["PagoID"]))
            {
                elemento.Pagoid = row["PagoID"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Catfacturasproveedorpago> listaCatfacturasproveedorpagos)
        {
            ICatfacturasproveedorpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedorpago>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catfacturasproveedorpago elemento in listaCatfacturasproveedorpagos)
                {
                    codigo = elemento.Facturaproveedorpagoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catfacturasproveedorpago_Save(
                    ref codigo,
                     (elemento.Facturaproveedorid != null) ? elemento.Facturaproveedorid : null,
                     (elemento.Pagoid != null) ? elemento.Pagoid : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Facturaproveedorpagoid = codigo;
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

        public static List<Entity.Contabilidad.Catfacturasproveedorpago> TraerCatfacturasproveedorpagos()
        {
            ICatfacturasproveedorpago proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedorpago> listaCatfacturasproveedorpagos = new List<Entity.Contabilidad.Catfacturasproveedorpago>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedorpago>();
                DataSet dsCatfacturasproveedorpagos = proc.Catfacturasproveedorpago_Select();
                foreach (DataRow row in dsCatfacturasproveedorpagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedorpago elemento = BuildEntity(row, true);
                    listaCatfacturasproveedorpagos.Add(elemento);
                }                
                return listaCatfacturasproveedorpagos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catfacturasproveedorpago TraerCatfacturasproveedorpagos(string facturaproveedorpagoid)
        {
            ICatfacturasproveedorpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedorpago>();
                Entity.Contabilidad.Catfacturasproveedorpago elemento = null;
                DataSet ds = null;
                ds = proc.Catfacturasproveedorpago_Select(facturaproveedorpagoid);
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

        public static System.Data.DataSet TraerCatfacturasproveedorpagosDS()
        {
            ICatfacturasproveedorpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedorpago>();
                DataSet ds = proc.Catfacturasproveedorpago_Select();
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
