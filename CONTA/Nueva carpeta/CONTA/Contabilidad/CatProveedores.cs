using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatproveedor
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatproveedor : ISprocBase
    {
        DataSet Catproveedor_Select(string proveedorid, string rfc,string EmpresaID);

        DataSet Catproveedor_Select();

        int Catproveedor_Save(
        ref string proveedorid,
        int codigo,
        string nombre,
        string empresaid,
        string cuentacontable,
        string rfc,
        string paisid,
        string estadoid,
        string municipioid,
        string ciudadid,
        string coloniaid,
        string calle,
        string noexterior,
        string nointerior,
        string codigopostal,
        DateTime fecha,
        int estatus,
        string usuario,
        string CveTipoOperacion,
        ref int ultimaAct);
        int Catproveedor_Save(
        ref string proveedorid,
        int codigo,
        string nombre,
        string empresaid,
        string cuentacontable,
        string rfc,
        string paisid,
        string estadoid,
        string municipioid,
        string ciudadid,
        string coloniaid,
        string calle,
        string noexterior,
        string nointerior,
        string codigopostal,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);
        DataSet Catproveedor_Select_PorCodigo(int codigo, string EmpresaID);

        DataSet Catproveedor_Select_PorNombre(string nombre, string EmpresaID);

        DataSet TraerSiguienteCodicoProveedor(string EmpresaID);

        DataSet Catproveedor_Select_PorCuentaContable(string EmpresaID, string Cuentacontable);
    }

    #endregion //Interfaz ICatproveedor

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catproveedor
    /// </summary>
    public class Catproveedor
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catproveedor()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catproveedores A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catproveedor BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catproveedor elemento = new Entity.Contabilidad.Catproveedor();
            if (!Convert.IsDBNull(row["ProveedorID"]))
            {
                elemento.Proveedorid = row["ProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["Codigo"]))
            {
                elemento.Codigo = int.Parse(row["Codigo"].ToString());
            }
            if (!Convert.IsDBNull(row["Nombre"]))
            {
                elemento.Nombre = row["Nombre"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaContable"]))
            {
                elemento.Cuentacontable = row["CuentaContable"].ToString();
            }
            if (!Convert.IsDBNull(row["RFC"]))
            {
                elemento.Rfc = row["RFC"].ToString();
            }
            if (!Convert.IsDBNull(row["PaisID"]))
            {
                elemento.Paisid = row["PaisID"].ToString();
            }
            if (!Convert.IsDBNull(row["EstadoID"]))
            {
                elemento.Estadoid = row["EstadoID"].ToString();
            }
            if (!Convert.IsDBNull(row["MunicipioID"]))
            {
                elemento.Municipioid = row["MunicipioID"].ToString();
            }
            if (!Convert.IsDBNull(row["CiudadID"]))
            {
                elemento.Ciudadid = row["CiudadID"].ToString();
            }
            if (!Convert.IsDBNull(row["ColoniaID"]))
            {
                elemento.Coloniaid = row["ColoniaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Calle"]))
            {
                elemento.Calle = row["Calle"].ToString();
            }
            if (!Convert.IsDBNull(row["NoExterior"]))
            {
                elemento.Noexterior = row["NoExterior"].ToString();
            }
            if (!Convert.IsDBNull(row["NoInterior"]))
            {
                elemento.Nointerior = row["NoInterior"].ToString();
            }
            if (!Convert.IsDBNull(row["CodigoPostal"]))
            {
                elemento.Codigopostal = row["CodigoPostal"].ToString();
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

            if (row.Table.Columns.Contains("Tipooperacion") && !Convert.IsDBNull(row["Tipooperacion"]))
            {
                elemento.Tipooperacion = row["Tipooperacion"].ToString();
            }
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Catproveedor> listaCatproveedores)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catproveedor elemento in listaCatproveedores)
                {
                    codigo = elemento.Proveedorid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catproveedor_Save(
                    ref codigo,
                     (elemento.Codigo != null) ? elemento.Codigo : int.MinValue,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Cuentacontable != null) ? elemento.Cuentacontable : null,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Paisid != null) ? elemento.Paisid : null,
                     (elemento.Estadoid != null) ? elemento.Estadoid : null,
                     (elemento.Municipioid != null) ? elemento.Municipioid : null,
                     (elemento.Ciudadid != null) ? elemento.Ciudadid : null,
                     (elemento.Coloniaid != null) ? elemento.Coloniaid : null,
                     (elemento.Calle != null) ? elemento.Calle : null,
                     (elemento.Noexterior != null) ? elemento.Noexterior : null,
                     (elemento.Nointerior != null) ? elemento.Nointerior : null,
                     (elemento.Codigopostal != null) ? elemento.Codigopostal : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Tipooperacion != null) ? elemento.Tipooperacion : null,
                    ref ultimaAct);
                    elemento.Proveedorid = codigo;
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

        public static List<Entity.Contabilidad.Catproveedor> TraerCatproveedores()
        {
            ICatproveedor proc = null;
            try
            {
                List<Entity.Contabilidad.Catproveedor> listaCatproveedores = new List<Entity.Contabilidad.Catproveedor>();
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                DataSet dsCatproveedores = proc.Catproveedor_Select();
                foreach (DataRow row in dsCatproveedores.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catproveedor elemento = BuildEntity(row, true);
                    listaCatproveedores.Add(elemento);
                }
                return listaCatproveedores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedores(string proveedorid, string rfc,string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                
                Entity.Contabilidad.Catproveedor elemento = null;
                DataSet ds = null;
                ds = proc.Catproveedor_Select(proveedorid, rfc, EmpresaID);
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

        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCodigo(int Codigo,string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                Entity.Contabilidad.Catproveedor elemento = null;
                DataSet ds = null;
                ds = proc.Catproveedor_Select_PorCodigo(Codigo, EmpresaID);
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


        public static List<Entity.Contabilidad.Catproveedor> TraerCatproveedoresPorNombre(string nombre,string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                List<Entity.Contabilidad.Catproveedor> listaCatproveedores = new List<Entity.Contabilidad.Catproveedor>();
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                DataSet dsCatproveedores = proc.Catproveedor_Select_PorNombre(nombre, EmpresaID);
                foreach (DataRow row in dsCatproveedores.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catproveedor elemento = BuildEntity(row, true);
                    listaCatproveedores.Add(elemento);
                }
                return listaCatproveedores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerCatproveedoresDS()
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                DataSet ds = proc.Catproveedor_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int TraerSiguienteCodicoProveedor(string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                DataSet ds = proc.TraerSiguienteCodicoProveedor(EmpresaID);
                return int.Parse(ds.Tables[0].Rows[0]["CodSiguiente"].ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCuentaContable(string EmpresaID, string Cuentacontable)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatproveedor>();
                Entity.Contabilidad.Catproveedor elemento = null;
                DataSet ds = null;
                ds = proc.Catproveedor_Select_PorCuentaContable(EmpresaID, Cuentacontable);
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
        public static void GuardarBalorLandTrading(ref List<Entity.Contabilidad.Catproveedor> listaCatproveedores)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICatproveedor>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catproveedor elemento in listaCatproveedores)
                {
                    codigo = elemento.Proveedorid;
                    ultimaAct = elemento.UltimaAct; 
                    proc.Catproveedor_Save(
                    ref codigo,
                     (elemento.Codigo != null) ? elemento.Codigo : int.MinValue,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Cuentacontable != null) ? elemento.Cuentacontable : null,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Paisid != null) ? elemento.Paisid : null,
                     (elemento.Estadoid != null) ? elemento.Estadoid : null,
                     (elemento.Municipioid != null) ? elemento.Municipioid : null,
                     (elemento.Ciudadid != null) ? elemento.Ciudadid : null,
                     (elemento.Coloniaid != null) ? elemento.Coloniaid : null,
                     (elemento.Calle != null) ? elemento.Calle : null,
                     (elemento.Noexterior != null) ? elemento.Noexterior : null,
                     (elemento.Nointerior != null) ? elemento.Nointerior : null,
                     (elemento.Codigopostal != null) ? elemento.Codigopostal : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Proveedorid = codigo;
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
        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresBLT(string proveedorid, string rfc, string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICatproveedor>();
                
                Entity.Contabilidad.Catproveedor elemento = null;
                DataSet ds = null;
                ds = proc.Catproveedor_Select(proveedorid, rfc, EmpresaID);
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
        public static int TraerSiguienteCodicoProveedorBLT(string EmpresaID)
        {
            ICatproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICatproveedor>();
                
                DataSet ds = proc.TraerSiguienteCodicoProveedor(EmpresaID);
                return int.Parse(ds.Tables[0].Rows[0]["CodSiguiente"].ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #endregion Métodos Públicos
    }
}
