using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvtip
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvtip : ISprocBase
    {
        DataSet Acvtip_Select(string acvtipid);

        DataSet Acvtip_Select();

        int Acvtip_Save(
        ref string acvtipid,
        string empresaid,
        string codEmpresa,
        int ejercicio,
        string tipPol,
        string descripcion,
        decimal folio1,
        decimal folio2,
        decimal folio3,
        decimal folio4,
        decimal folio5,
        decimal folio6,
        decimal folio7,
        decimal folio8,
        decimal folio9,
        decimal folio10,
        decimal folio11,
        decimal folio12,
        ref int ultimaAct);

    }

    #endregion //Interfaz IAcvtip

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvtip
    /// </summary>
    public class Acvtip
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvtip()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvtip A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvtip BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvtip elemento = new Entity.Contabilidad.Acvtip();
            if (!Convert.IsDBNull(row["AcvTipID"]))
            {
                elemento.Acvtipid = row["AcvTipID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["Ejercicio"]))
            {
                elemento.Ejercicio = int.Parse(row["Ejercicio"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio1"]))
            {
                elemento.Folio1 = decimal.Parse(row["Folio1"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio2"]))
            {
                elemento.Folio2 = decimal.Parse(row["Folio2"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio3"]))
            {
                elemento.Folio3 = decimal.Parse(row["Folio3"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio4"]))
            {
                elemento.Folio4 = decimal.Parse(row["Folio4"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio5"]))
            {
                elemento.Folio5 = decimal.Parse(row["Folio5"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio6"]))
            {
                elemento.Folio6 = decimal.Parse(row["Folio6"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio7"]))
            {
                elemento.Folio7 = decimal.Parse(row["Folio7"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio8"]))
            {
                elemento.Folio8 = decimal.Parse(row["Folio8"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio9"]))
            {
                elemento.Folio9 = decimal.Parse(row["Folio9"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio10"]))
            {
                elemento.Folio10 = decimal.Parse(row["Folio10"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio11"]))
            {
                elemento.Folio11 = decimal.Parse(row["Folio11"].ToString());
            }
            if (!Convert.IsDBNull(row["Folio12"]))
            {
                elemento.Folio12 = decimal.Parse(row["Folio12"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvtip> listaAcvtip)
        {
            IAcvtip proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvtip>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvtip elemento in listaAcvtip)
                {
                    codigo = elemento.Acvtipid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Acvtip_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     elemento.Ejercicio,
                     elemento.TipPol,
                     elemento.Descripcion,
                     elemento.Folio1,
                     elemento.Folio2,
                     elemento.Folio3,
                     elemento.Folio4,
                     elemento.Folio5,
                     elemento.Folio6,
                     elemento.Folio7,
                     elemento.Folio8,
                     elemento.Folio9,
                     elemento.Folio10,
                     elemento.Folio11,
                     elemento.Folio12,
                    ref ultimaAct);
                    elemento.Acvtipid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaAcvtip.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Acvtip> TraerAcvtip()
        {
            IAcvtip proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvtip> listaAcvtip = new ListaDeEntidades<Entity.Contabilidad.Acvtip>();
                proc = Utilerias.GenerarSproc<IAcvtip>();
                DataSet dsAcvtip = proc.Acvtip_Select();
                foreach (DataRow row in dsAcvtip.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvtip elemento = BuildEntity(row, true);
                    listaAcvtip.Add(elemento);
                }
                listaAcvtip.AcceptChanges();
                return listaAcvtip;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvtip TraerAcvtip(string acvtipid)
        {
            IAcvtip proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvtip>();
                Entity.Contabilidad.Acvtip elemento = null;
                DataSet ds = null;
                ds = proc.Acvtip_Select(acvtipid);
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

        public static System.Data.DataSet TraerAcvtipDS()
        {
            IAcvtip proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvtip>();
                DataSet ds = proc.Acvtip_Select();
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
