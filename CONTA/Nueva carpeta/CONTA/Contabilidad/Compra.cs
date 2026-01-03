using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICompra
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICompra : ISprocBase
    {
        DataSet Compra_Select(string compraid, string EmpresaID, int? Folio, string nombreProv);

        DataSet Compra_Select();

        int Compra_Save(
        ref string compraid,
        string empresaid,
        string proveedorid,
        ref int folio,
        decimal importe,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICompra

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Compra
    /// </summary>
    public class Compra
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Compra()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Compras A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Compra BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Compra elemento = new Entity.Contabilidad.Compra();
            if (!Convert.IsDBNull(row["CompraID"]))
            {
                elemento.Compraid = row["CompraID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["ProveedorID"]))
            {
                elemento.Proveedorid = row["ProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio"]))
            {
                elemento.Folio = int.Parse(row["Folio"].ToString());
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString()).ToShortDateString();
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
        public static void Guardar(ref List<Entity.Contabilidad.Compra> listaCompras)
        {
            ICompra proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICompra>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;
                int Folio;
                foreach (Entity.Contabilidad.Compra elemento in listaCompras)
                {
                    codigo = elemento.Compraid;
                    ultimaAct = elemento.UltimaAct;
                    Folio = elemento.Folio;
                    proc.Compra_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Proveedorid != null) ? elemento.Proveedorid : null,
                     ref Folio,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Fecha != null && elemento.Fecha != string.Empty) ? DateTime.Parse(elemento.Fecha) : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Compraid = codigo;
                    elemento.Folio = Folio;
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

        public static List<Entity.Contabilidad.Compra> TraerCompras()
        {
            ICompra proc = null;
            try
            {
                List<Entity.Contabilidad.Compra> listaCompras = new List<Entity.Contabilidad.Compra>();
                proc = Utilerias.GenerarSproc<ICompra>();
                DataSet dsCompras = proc.Compra_Select();
                foreach (DataRow row in dsCompras.Tables[0].Rows)
                {
                    Entity.Contabilidad.Compra elemento = BuildEntity(row, true);
                    listaCompras.Add(elemento);
                }                
                return listaCompras;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Compra TraerCompras(string compraid)
        {
            ICompra proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICompra>();
                Entity.Contabilidad.Compra elemento = null;
                DataSet ds = null;
                ds = proc.Compra_Select(compraid, null, null, null);
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

        public static System.Data.DataSet TraerComprasDS()
        {
            ICompra proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICompra>();
                DataSet ds = proc.Compra_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Compra TraerComprasPorCodigo(string empresaid,int codigo)
        {
            ICompra proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICompra>();
                Entity.Contabilidad.Compra elemento = null;
                DataSet ds = null;
                ds = proc.Compra_Select(null, empresaid, codigo,null);
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

        public static List<Entity.Contabilidad.Compra> TraerComprasPorProveedor(string empresaid, string nombreproveedor)
        {
            ICompra proc = null;
            try
            {
                List<Entity.Contabilidad.Compra> listaCompras = new List<Entity.Contabilidad.Compra>();
                proc = Utilerias.GenerarSproc<ICompra>();
                DataSet dsCompras = proc.Compra_Select(null, empresaid, null, nombreproveedor);
                foreach (DataRow row in dsCompras.Tables[0].Rows)
                {
                    Entity.Contabilidad.Compra elemento = BuildEntity(row, true);
                    elemento.Nombreproveedor = row["NombreProveedor"].ToString();
                    listaCompras.Add(elemento);
                }
                return listaCompras;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
