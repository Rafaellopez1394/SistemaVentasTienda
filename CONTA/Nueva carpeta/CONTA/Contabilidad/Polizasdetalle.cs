using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IPolizasdetalle
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IPolizasdetalle : ISprocBase
    {
        DataSet Polizasdetalle_Select(string polizadetalleid);

        DataSet Polizasdetalle_Select();

        DataSet Polizasdetalle_Select_PorPoliza(string polizaid);

        int Polizasdetalle_Save(
        ref string polizadetalleid,
        string polizaid,
        string cuentaid,
        string tipMov,
        string concepto,
        decimal cantidad,
        decimal importe,
        int estatus,
        DateTime fecha,
        string usuario,
        string presupuestodetalleId,
        string inventariocostoid,
        ref int ultimaAct);

        int PolizaDetalle_SaveXML(string nxml);

    }

    #endregion //Interfaz IPolizasdetalle

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Polizasdetalle
    /// </summary>
    public class Polizasdetalle
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Polizasdetalle()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Polizasdetalle A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Polizasdetalle BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Polizasdetalle elemento = new Entity.Contabilidad.Polizasdetalle();
            if (!Convert.IsDBNull(row["PolizaDetalleID"]))
            {
                elemento.Polizadetalleid = row["PolizaDetalleID"].ToString();
            }
            if (!Convert.IsDBNull(row["PolizaID"]))
            {
                elemento.Polizaid = row["PolizaID"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaID"]))
            {
                elemento.Cuentaid = row["CuentaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Tip_Mov"]))
            {
                elemento.TipMov = row["Tip_Mov"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Cantidad"]))
            {
                elemento.Cantidad = decimal.Parse(row["Cantidad"].ToString());
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
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
            if (!Convert.IsDBNull(row["PresupuestoDetalle_ID"]))
            {
                elemento.PresupuestodetalleId = row["PresupuestoDetalle_ID"].ToString();
            }
            if (!Convert.IsDBNull(row["InventarioCostoID"]))
            {
                elemento.Inventariocostoid = row["InventarioCostoID"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listaPolizasdetalle)
        {
            IPolizasdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasdetalle>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Polizasdetalle elemento in listaPolizasdetalle)
                {
                    codigo = elemento.Polizadetalleid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Polizasdetalle_Save(
                    ref codigo,
                     (elemento.Polizaid != null) ? elemento.Polizaid : null,
                     (elemento.Cuentaid != null) ? elemento.Cuentaid : null,
                     elemento.TipMov,
                     (elemento.Concepto != null) ? elemento.Concepto : null,
                     elemento.Cantidad,
                     elemento.Importe,
                     elemento.Estatus,
                     elemento.Fecha,
                     elemento.Usuario,
                     elemento.PresupuestodetalleId != Guid.Empty.ToString() ? elemento.PresupuestodetalleId : null,
                     elemento.Inventariocostoid != Guid.Empty.ToString() ? elemento.Inventariocostoid : null,
                    ref ultimaAct);
                    elemento.Polizadetalleid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaPolizasdetalle.AcceptChanges();
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


        public static void GuardarPorXML(string xmlDatos)
        {
            IPolizasdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasdetalle>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.PolizaDetalle_SaveXML(xmlDatos);                    
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

        

        public static ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> TraerPolizasdetalle()
        {
            IPolizasdetalle proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listaPolizasdetalle = new ListaDeEntidades<Entity.Contabilidad.Polizasdetalle>();
                proc = Utilerias.GenerarSproc<IPolizasdetalle>();
                DataSet dsPolizasdetalle = proc.Polizasdetalle_Select();
                foreach (DataRow row in dsPolizasdetalle.Tables[0].Rows)
                {
                    Entity.Contabilidad.Polizasdetalle elemento = BuildEntity(row, true);
                    listaPolizasdetalle.Add(elemento);
                }
                listaPolizasdetalle.AcceptChanges();
                return listaPolizasdetalle;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Polizasdetalle TraerPolizasdetalle(string polizadetalleid)
        {
            IPolizasdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasdetalle>();
                Entity.Contabilidad.Polizasdetalle elemento = null;
                DataSet ds = null;
                ds = proc.Polizasdetalle_Select(polizadetalleid);
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

        public static ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> TraerPolizasdetallePorPoliza(string polizaid)
        {
            IPolizasdetalle proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listaPolizasdetalle = new ListaDeEntidades<Entity.Contabilidad.Polizasdetalle>();
                proc = Utilerias.GenerarSproc<IPolizasdetalle>();
                DataSet dsPolizasdetalle = proc.Polizasdetalle_Select_PorPoliza(polizaid);
                foreach (DataRow row in dsPolizasdetalle.Tables[0].Rows)
                {
                    Entity.Contabilidad.Polizasdetalle elemento = BuildEntity(row, true);
                    listaPolizasdetalle.Add(elemento);
                }
                listaPolizasdetalle.AcceptChanges();
                return listaPolizasdetalle;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerPolizasdetalleDS()
        {
            IPolizasdetalle proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasdetalle>();
                DataSet ds = proc.Polizasdetalle_Select();
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
