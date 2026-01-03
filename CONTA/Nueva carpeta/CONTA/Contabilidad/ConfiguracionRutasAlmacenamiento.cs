using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoSproc;
using System.Data;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IConfiguracionRutasAlmacenamiento
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IConfiguracionRutasAlmacenamiento : ISprocBase
    {
        DataSet ConfiguracionRutasAlmacenamiento_Select(int configuracionid);

        DataSet ConfiguracionRutasAlmacenamiento_Select();

        int ConfiguracionRutasAlmacenamiento_Save(
        ref int configuracionid,
        string nombreruta,
        string rutaalmacenamiento,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

    }

    #endregion //Interfaz IConfiguracionRutasAlmacenamiento

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de ConfiguracionRutasAlmacenamiento
    /// </summary>
    public class ConfiguracionRutasAlmacenamiento
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ConfiguracionRutasAlmacenamiento()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo ConfiguracionRutasAlmacenamiento A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.ConfiguracionRutasAlmacenamiento elemento = new Entity.Contabilidad.ConfiguracionRutasAlmacenamiento();
            if (!Convert.IsDBNull(row["ConfiguracionID"]))
            {
                elemento.Configuracionid = int.Parse(row["ConfiguracionID"].ToString());
            }
            if (!Convert.IsDBNull(row["NombreRuta"]))
            {
                elemento.Nombreruta = row["NombreRuta"].ToString();
            }
            if (!Convert.IsDBNull(row["RutaAlmacenamiento"]))
            {
                elemento.Rutaalmacenamiento = row["RutaAlmacenamiento"].ToString();
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
        public static void Guardar(ref List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento> listaConfiguracionRutasAlmacenamiento)
        {
            IConfiguracionRutasAlmacenamiento proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IConfiguracionRutasAlmacenamiento>();
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.ConfiguracionRutasAlmacenamiento elemento in listaConfiguracionRutasAlmacenamiento)
                {
                    codigo = elemento.Configuracionid;
                    ultimaAct = elemento.UltimaAct;

                    proc.ConfiguracionRutasAlmacenamiento_Save(
                    ref codigo,
                     elemento.Nombreruta,
                     elemento.Rutaalmacenamiento,
                     elemento.Fecha,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     elemento.Usuario,
                    ref ultimaAct);
                    elemento.Configuracionid = codigo;
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

        public static List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento> TraerConfiguracionRutasAlmacenamiento()
        {
            IConfiguracionRutasAlmacenamiento proc = null;
            try
            {
                List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento> listaConfiguracionRutasAlmacenamiento = new List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento>();
                proc = Utilerias.GenerarSproc<IConfiguracionRutasAlmacenamiento>();
                DataSet dsConfiguracionRutasAlmacenamiento = proc.ConfiguracionRutasAlmacenamiento_Select();
                foreach (DataRow row in dsConfiguracionRutasAlmacenamiento.Tables[0].Rows)
                {
                    Entity.Contabilidad.ConfiguracionRutasAlmacenamiento elemento = BuildEntity(row, true);
                    listaConfiguracionRutasAlmacenamiento.Add(elemento);
                }
                return listaConfiguracionRutasAlmacenamiento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionRutasAlmacenamiento(int configuracionid)
        {
            IConfiguracionRutasAlmacenamiento proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IConfiguracionRutasAlmacenamiento>();
                Entity.Contabilidad.ConfiguracionRutasAlmacenamiento elemento = null;
                DataSet ds = null;
                ds = proc.ConfiguracionRutasAlmacenamiento_Select(configuracionid);
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

        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionRutasAlmacenamientoBLT(int configuracionid)
        {
            IConfiguracionRutasAlmacenamiento proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IConfiguracionRutasAlmacenamiento>();
                Entity.Contabilidad.ConfiguracionRutasAlmacenamiento elemento = null;
                DataSet ds = null;
                ds = proc.ConfiguracionRutasAlmacenamiento_Select(configuracionid);
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

        public static System.Data.DataSet TraerConfiguracionRutasAlmacenamientoDS()
        {
            IConfiguracionRutasAlmacenamiento proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IConfiguracionRutasAlmacenamiento>();
                DataSet ds = proc.ConfiguracionRutasAlmacenamiento_Select();
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
