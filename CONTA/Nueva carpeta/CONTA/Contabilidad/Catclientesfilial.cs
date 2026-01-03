using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatclientesfilial
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatclientesfilial : ISprocBase
    {
        DataSet Catclientesfilial_Select(string clienteid, string EmpresaID, int? Codigo, string Nombre);

        DataSet Catclientesfilial_Select();

        int Catclientesfilial_Save(
        ref string clienteid,
        string empresaid,
        int codigo,
        string nombre,
        string rfc,
        string calle,
        string noexterior,
        string nointerior,
        string codigopostal,
        string paisid,
        string estadoid,
        string municipioid,
        string ciudadid,
        string coloniaid,
        DateTime fechaalta,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatclientesfilial

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catclientesfilial
    /// </summary>
    public class Catclientesfilial
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catclientesfilial()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catclientesfilial A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catclientesfilial BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catclientesfilial elemento = new Entity.Contabilidad.Catclientesfilial();
            if (!Convert.IsDBNull(row["ClienteID"]))
            {
                elemento.Clienteid = row["ClienteID"].ToString();
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
            if (!Convert.IsDBNull(row["FechaAlta"]))
            {
                elemento.Fechaalta = DateTime.Parse(row["FechaAlta"].ToString());
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
        public static void Guardar(ref List<Entity.Contabilidad.Catclientesfilial> listaCatclientesfilial)
        {
            ICatclientesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesfilial>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catclientesfilial elemento in listaCatclientesfilial)
                {
                    codigo = elemento.Clienteid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catclientesfilial_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Codigo != null) ? elemento.Codigo : int.MinValue,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Calle != null) ? elemento.Calle : null,
                     (elemento.Noexterior != null) ? elemento.Noexterior : null,
                     (elemento.Nointerior != null) ? elemento.Nointerior : null,
                     (elemento.Codigopostal != null) ? elemento.Codigopostal : null,
                     (elemento.Paisid != null) ? elemento.Paisid : null,
                     (elemento.Estadoid != null) ? elemento.Estadoid : null,
                     (elemento.Municipioid != null) ? elemento.Municipioid : null,
                     (elemento.Ciudadid != null) ? elemento.Ciudadid : null,
                     (elemento.Coloniaid != null) ? elemento.Coloniaid : null,
                     (elemento.Fechaalta != null) ? elemento.Fechaalta : DateTime.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Clienteid = codigo;
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

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilial()
        {
            ICatclientesfilial proc = null;
            try
            {
                List<Entity.Contabilidad.Catclientesfilial> listaCatclientesfilial = new List<Entity.Contabilidad.Catclientesfilial>();
                proc = Utilerias.GenerarSproc<ICatclientesfilial>();
                DataSet dsCatclientesfilial = proc.Catclientesfilial_Select();
                foreach (DataRow row in dsCatclientesfilial.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catclientesfilial elemento = BuildEntity(row, true);
                    listaCatclientesfilial.Add(elemento);
                }
                return listaCatclientesfilial;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Entity.Contabilidad.Catclientesfilial TraerCatclientesfilial(string clienteid,string empresaid,int?codigo)
        {
            ICatclientesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesfilial>();
                Entity.Contabilidad.Catclientesfilial elemento = null;
                DataSet ds = null;
                ds = proc.Catclientesfilial_Select(clienteid, empresaid, codigo, null);
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

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilialPorNombre(string nombre,string empresaid)
        {
            ICatclientesfilial proc = null;
            try
            {
                List<Entity.Contabilidad.Catclientesfilial> listaCatclientesfilial = new List<Entity.Contabilidad.Catclientesfilial>();
                proc = Utilerias.GenerarSproc<ICatclientesfilial>();
                DataSet dsCatclientesfilial = proc.Catclientesfilial_Select(null, empresaid, null, nombre);
                foreach (DataRow row in dsCatclientesfilial.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catclientesfilial elemento = BuildEntity(row, true);
                    listaCatclientesfilial.Add(elemento);
                }
                return listaCatclientesfilial;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static System.Data.DataSet TraerCatclientesfilialDS()
        {
            ICatclientesfilial proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatclientesfilial>();
                DataSet ds = proc.Catclientesfilial_Select();
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
