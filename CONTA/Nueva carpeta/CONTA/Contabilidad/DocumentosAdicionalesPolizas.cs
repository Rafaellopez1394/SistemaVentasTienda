using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IDocumentosAdicionalesPolizas
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IDocumentosAdicionalesPolizas : ISprocBase
    {
        DataSet DocumentosAdicionalesPoliza_Select(string Polizaid); 
        DataSet Documentosadicionalesuuid_Select(string uuid);
        DataSet DocumentosAdicionalesPoliza_Select();

        int DocumentosAdicionalesPoliza_Save(
        ref string uuid,
        string polizaid,
        int poliza,
        string empresaid,
        string Documento,
        string nombrearchivo,
        string Extension,
        string ruta,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz IDocumentosAdicionalesPolizas

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de DocumentosAdicionalesPolizas
    /// </summary>
    public class DocumentosAdicionalesPolizas
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentosAdicionalesPolizas()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo DocumentosAdicionalesPolizass A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.DocumentosAdicionalesPolizas BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.DocumentosAdicionalesPolizas elemento = new Entity.Contabilidad.DocumentosAdicionalesPolizas();
            if (!Convert.IsDBNull(row["UUID"]))
            {
                elemento.Uuid = row["UUID"].ToString();
            }
            if (!Convert.IsDBNull(row["PolizaID"]))
            {
                elemento.Polizaid = row["PolizaID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Documento"]))
            {
                elemento.Documento = row["Documento"].ToString();
            }
            if (!Convert.IsDBNull(row["NombreArchivo"]))
            {
                elemento.Nombrearchivo = row["NombreArchivo"].ToString();
            }
            if (!Convert.IsDBNull(row["Extension"]))
            {
                elemento.Extension = row["Extension"].ToString();
            }
            if (!Convert.IsDBNull(row["Ruta"]))
            {
                elemento.Ruta = row["Ruta"].ToString();
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
        public static void Guardar(ref List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosAdicionalesPolizass)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IDocumentosAdicionalesPolizas>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.DocumentosAdicionalesPolizas elemento in listaDocumentosAdicionalesPolizass)
                {
                    codigo = elemento.Uuid;
                    ultimaAct = elemento.UltimaAct;

                    proc.DocumentosAdicionalesPoliza_Save(
                    ref codigo,
                     elemento.Polizaid,
                     elemento.Poliza,
                     elemento.Empresaid,
                     (elemento.Documento != null) ? elemento.Documento : null,
                     (elemento.Nombrearchivo != null) ? elemento.Nombrearchivo : null,
                     (elemento.Extension != null) ? elemento.Extension : null,
                     (elemento.Ruta != null) ? elemento.Ruta : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     elemento.Usuario,
                    ref ultimaAct);
                    elemento.Uuid = codigo;
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

        public static void GuardarBLT(ref List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosAdicionalesPolizass)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IDocumentosAdicionalesPolizas>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.DocumentosAdicionalesPolizas elemento in listaDocumentosAdicionalesPolizass)
                {
                    codigo = elemento.Uuid;
                    ultimaAct = elemento.UltimaAct;

                    proc.DocumentosAdicionalesPoliza_Save(
                    ref codigo,
                     elemento.Polizaid,
                     elemento.Poliza,
                     elemento.Empresaid,
                     (elemento.Documento != null) ? elemento.Documento : null,
                     (elemento.Nombrearchivo != null) ? elemento.Nombrearchivo : null,
                     (elemento.Extension != null) ? elemento.Extension : null,
                     (elemento.Ruta != null) ? elemento.Ruta : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     elemento.Usuario,
                    ref ultimaAct);
                    elemento.Uuid = codigo;
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

        public static List<Entity.Contabilidad.DocumentosAdicionalesPolizas> TraerDocumentosAdicionalesPolizas(string PolizaID)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosAdicionalesPolizass = new List<Entity.Contabilidad.DocumentosAdicionalesPolizas>();
                proc = Utilerias.GenerarSproc<IDocumentosAdicionalesPolizas>();
                DataSet dsDocumentosAdicionalesPolizass = proc.DocumentosAdicionalesPoliza_Select(PolizaID);
                foreach (DataRow row in dsDocumentosAdicionalesPolizass.Tables[0].Rows)
                {
                    Entity.Contabilidad.DocumentosAdicionalesPolizas elemento = BuildEntity(row, true);
                    listaDocumentosAdicionalesPolizass.Add(elemento);
                }
                return listaDocumentosAdicionalesPolizass;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.DocumentosAdicionalesPolizas TraerDocumentosAdicionalesPolizasuuid(string uuid)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IDocumentosAdicionalesPolizas>();
                Entity.Contabilidad.DocumentosAdicionalesPolizas elemento = null;
                DataSet ds = null;
                ds = proc.Documentosadicionalesuuid_Select(uuid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static System.Data.DataSet TraerDocumentosAdicionalesPolizasDS()
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IDocumentosAdicionalesPolizas>();
                DataSet ds = proc.DocumentosAdicionalesPoliza_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Entity.Contabilidad.DocumentosAdicionalesPolizas> TraerDocumentosAdicionalesPolizasBLT(string PolizaID)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosAdicionalesPolizass = new List<Entity.Contabilidad.DocumentosAdicionalesPolizas>();
                proc = Utilerias.GenerarSprocBLT<IDocumentosAdicionalesPolizas>();
                DataSet dsDocumentosAdicionalesPolizass = proc.DocumentosAdicionalesPoliza_Select(PolizaID);
                foreach (DataRow row in dsDocumentosAdicionalesPolizass.Tables[0].Rows)
                {
                    Entity.Contabilidad.DocumentosAdicionalesPolizas elemento = BuildEntity(row, true);
                    listaDocumentosAdicionalesPolizass.Add(elemento);
                }
                return listaDocumentosAdicionalesPolizass;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.DocumentosAdicionalesPolizas TraerDocumentosAdicionalesPolizasuuidBLT(string uuid)
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IDocumentosAdicionalesPolizas>();
                Entity.Contabilidad.DocumentosAdicionalesPolizas elemento = null;
                DataSet ds = null;
                ds = proc.Documentosadicionalesuuid_Select(uuid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static System.Data.DataSet TraerDocumentosAdicionalesPolizasDSBLT()
        {
            IDocumentosAdicionalesPolizas proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IDocumentosAdicionalesPolizas>();
                DataSet ds = proc.DocumentosAdicionalesPoliza_Select();
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

