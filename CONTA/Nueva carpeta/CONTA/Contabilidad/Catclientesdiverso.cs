using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatclientesdiverso
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatclientesdiverso : ISprocBase
    {
        DataSet Catclientesdiverso_Select(string clientediversoid, string EmpresaID, int? Codigo, string Nombre, string RFC);

        DataSet Catclientesdiverso_Select();
        DataSet TraerDireccionCompletaCteDiv(string ClienteDiversoID);

        int Catclientesdiverso_Save(
        ref string clientediversoid,
        string empresaid,
        int codigo,
        string nombre,
        string rfc,
        string codigoclientecuentacontable,
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
        string regimenfiscal,
        string eMail,
        ref int ultimaAct);
        

    }

    #endregion //Interfaz ICatclientesdiverso

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catclientesdiverso
    /// </summary>
    public class Catclientesdiverso
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catclientesdiverso()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catclientesdiversos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catclientesdiverso BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catclientesdiverso elemento = new Entity.Contabilidad.Catclientesdiverso();
            if (!Convert.IsDBNull(row["ClienteDiversoID"]))
            {
                elemento.Clientediversoid = row["ClienteDiversoID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Codigo"]))
            {
                elemento.Codigo = int.Parse(row["Codigo"].ToString());
            }
            if (!Convert.IsDBNull(row["Nombre"]))
            {
                elemento.Nombre = row["Nombre"].ToString();
            }
            if (!Convert.IsDBNull(row["RFC"]))
            {
                elemento.Rfc = row["RFC"].ToString();
            }
            if (!Convert.IsDBNull(row["CodigoClienteCuentaContable"]))
            {
                elemento.Codigoclientecuentacontable = row["CodigoClienteCuentaContable"].ToString();
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
            if (!Convert.IsDBNull(row["eMail"]))
            {
                elemento.eMail = row["eMail"].ToString();
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
            if (!Convert.IsDBNull(row["NumRegIdTrib"]))
            {
                elemento.NumRegIdTrib = row["NumRegIdTrib"].ToString();
            }
            if (!Convert.IsDBNull(row["RegimenFiscal"]))
            {
                elemento.Regimenfiscal = row["RegimenFiscal"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Catclientesdiverso> listaCatclientesdiversos)
        {
            ICatclientesdiverso proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catclientesdiverso elemento in listaCatclientesdiversos)
                {
                    codigo = elemento.Clientediversoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catclientesdiverso_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Codigo != null) ? elemento.Codigo : int.MinValue,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Codigoclientecuentacontable != null) ? elemento.Codigoclientecuentacontable : null,
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
                     (elemento.Regimenfiscal != null) ? elemento.Regimenfiscal : null,
                     (elemento.eMail != null) ? elemento.eMail : null,
                    ref ultimaAct);
                    elemento.Clientediversoid = codigo;
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

        public static List<Entity.Contabilidad.Catclientesdiverso> TraerCatclientesdiversos(string EmpresaID,string Descripcion)
        {
            ICatclientesdiverso proc = null;
            try
            {
                List<Entity.Contabilidad.Catclientesdiverso> listaCatclientesdiversos = new List<Entity.Contabilidad.Catclientesdiverso>();
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>();
                DataSet dsCatclientesdiversos = proc.Catclientesdiverso_Select(null, EmpresaID, null, Descripcion, null);
                foreach (DataRow row in dsCatclientesdiversos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catclientesdiverso elemento = BuildEntity(row, true);
                    listaCatclientesdiversos.Add(elemento);
                }
                
                return listaCatclientesdiversos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversos(string clientediversoid, string EmpresaID, int? Codigo)
        {
            ICatclientesdiverso proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>();
                Entity.Contabilidad.Catclientesdiverso elemento = null;
                DataSet ds = null;
                ds = proc.Catclientesdiverso_Select(clientediversoid, EmpresaID, Codigo, null, null);
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

        public static System.Data.DataSet TraerCatclientesdiversosDS()
        {
            ICatclientesdiverso proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>();
                DataSet ds = proc.Catclientesdiverso_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerDireccionCompletaCteDiv(string ClienteDiversoID)
        {
            ICatclientesdiverso proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>();
                DataSet ds = proc.TraerDireccionCompletaCteDiv(ClienteDiversoID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversosPorRFC(string RFC, string EmpresaID)
        {
            ICatclientesdiverso proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesdiverso>();
                Entity.Contabilidad.Catclientesdiverso elemento = null;
                DataSet ds = null;
                ds = proc.Catclientesdiverso_Select(null, EmpresaID, null, null, RFC);
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

        #endregion Métodos Públicos
    }
}
