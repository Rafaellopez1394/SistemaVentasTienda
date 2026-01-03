using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatfacturasproveedordet
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatfacturasproveedordet : ISprocBase
    {
        DataSet Catfacturasproveedordet_Select(string facturaproveedordetid, string FacturaProveedorID);

        DataSet Catfacturasproveedordet_Select();

        int Catfacturasproveedordet_Save(
        ref string facturaproveedordetid,
        string facturaproveedorid,
        string cuenta,
        string concepto,
        decimal importe,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

        int Catfacturasproveedordet_Delete(string FacturaProveedorID);

    }

    #endregion //Interfaz ICatfacturasproveedordet

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catfacturasproveedordet
    /// </summary>
    public class Catfacturasproveedordet
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catfacturasproveedordet()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catfacturasproveedordet A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catfacturasproveedordet BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catfacturasproveedordet elemento = new Entity.Contabilidad.Catfacturasproveedordet();
            if (!Convert.IsDBNull(row["FacturaProveedorDetID"]))
            {
                elemento.Facturaproveedordetid = row["FacturaProveedorDetID"].ToString();
            }
            if (!Convert.IsDBNull(row["FacturaProveedorID"]))
            {
                elemento.Facturaproveedorid = row["FacturaProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
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
        public static void Guardar(ref List<Entity.Contabilidad.Catfacturasproveedordet> listaCatfacturasproveedordet)
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catfacturasproveedordet elemento in listaCatfacturasproveedordet)
                {
                    codigo = elemento.Facturaproveedordetid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catfacturasproveedordet_Save(
                    ref codigo,
                     (elemento.Facturaproveedorid != null) ? elemento.Facturaproveedorid : null,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Concepto != null) ? elemento.Concepto : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Facturaproveedordetid = codigo;
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

        public static List<Entity.Contabilidad.Catfacturasproveedordet> TraerCatfacturasproveedordet()
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedordet> listaCatfacturasproveedordet = new List<Entity.Contabilidad.Catfacturasproveedordet>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>();
                DataSet dsCatfacturasproveedordet = proc.Catfacturasproveedordet_Select();
                foreach (DataRow row in dsCatfacturasproveedordet.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedordet elemento = BuildEntity(row, true);
                    listaCatfacturasproveedordet.Add(elemento);
                }                
                return listaCatfacturasproveedordet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catfacturasproveedordet TraerCatfacturasproveedordet(string facturaproveedordetid)
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>();
                Entity.Contabilidad.Catfacturasproveedordet elemento = null;
                DataSet ds = null;
                ds = proc.Catfacturasproveedordet_Select(facturaproveedordetid, null);
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

        public static System.Data.DataSet TraerCatfacturasproveedordetDS()
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>();
                DataSet ds = proc.Catfacturasproveedordet_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Eliminar(string FacturaProveedorID)
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>();
                proc.Catfacturasproveedordet_Delete(FacturaProveedorID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Catfacturasproveedordet> TraerCatfacturasproveedordetPorFactura(string facturaproveedorid)
        {
            ICatfacturasproveedordet proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedordet> listaCatfacturasproveedordet = new List<Entity.Contabilidad.Catfacturasproveedordet>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedordet>();
                DataSet dsCatfacturasproveedordet = proc.Catfacturasproveedordet_Select(null, facturaproveedorid);
                foreach (DataRow row in dsCatfacturasproveedordet.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedordet elemento = BuildEntity(row, true);
                    listaCatfacturasproveedordet.Add(elemento);
                }
                return listaCatfacturasproveedordet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
