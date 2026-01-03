using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IPago
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IPago : ISprocBase
    {
        DataSet Pago_Select(string pagoid);

        DataSet Pago_Select();

        int Pago_Save(
        ref string pagoid,
        string empresaid,
        string proveedorid,
        string bancoid,
        string poliza,
        DateTime fecPol,
        string tipPol,
        decimal importe,
        string tipomoneda,
        decimal tipocambio,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz IPago

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Pago
    /// </summary>
    public class Pago
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Pago()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Pagos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Pago BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Pago elemento = new Entity.Contabilidad.Pago();
            if (!Convert.IsDBNull(row["PagoID"]))
            {
                elemento.Pagoid = row["PagoID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["ProveedorID"]))
            {
                elemento.Proveedorid = row["ProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["BancoID"]))
            {
                elemento.Bancoid = row["BancoID"].ToString();
            }
            if (!Convert.IsDBNull(row["Poliza"]))
            {
                elemento.Poliza = row["Poliza"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Pol"]))
            {
                elemento.FecPol = DateTime.Parse(row["Fec_Pol"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["TipoMoneda"]))
            {
                elemento.Tipomoneda = row["TipoMoneda"].ToString();
            }
            if (!Convert.IsDBNull(row["TipoCambio"]))
            {
                elemento.Tipocambio = decimal.Parse(row["TipoCambio"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Pago> listaPagos)
        {
            IPago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPago>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Pago elemento in listaPagos)
                {
                    codigo = elemento.Pagoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Pago_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Proveedorid != null) ? elemento.Proveedorid : null,
                     (elemento.Bancoid != null) ? elemento.Bancoid : null,
                     (elemento.Poliza != null) ? elemento.Poliza : null,
                     (elemento.FecPol != null) ? elemento.FecPol : DateTime.MinValue,
                     (elemento.TipPol != null) ? elemento.TipPol : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Tipomoneda != null) ? elemento.Tipomoneda : null,
                     (elemento.Tipocambio != null) ? elemento.Tipocambio : decimal.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Pagoid = codigo;
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

        public static List<Entity.Contabilidad.Pago> TraerPagos()
        {
            IPago proc = null;
            try
            {
                List<Entity.Contabilidad.Pago> listaPagos = new List<Entity.Contabilidad.Pago>();
                proc = Utilerias.GenerarSproc<IPago>();
                DataSet dsPagos = proc.Pago_Select();
                foreach (DataRow row in dsPagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Pago elemento = BuildEntity(row, true);
                    listaPagos.Add(elemento);
                }                
                return listaPagos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Pago TraerPagos(string pagoid)
        {
            IPago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPago>();
                Entity.Contabilidad.Pago elemento = null;
                DataSet ds = null;
                ds = proc.Pago_Select(pagoid);
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

        public static System.Data.DataSet TraerPagosDS()
        {
            IPago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPago>();
                DataSet ds = proc.Pago_Select();
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
