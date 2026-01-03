using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvctam
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvctam : ISprocBase
    {
        DataSet Acvctam_Select(string acvctamid);

        DataSet Acvctam_Select();

        DataSet Acvctam_Select_PorCuenta(string cuenta, string empresaid);

        int Acvctam_Save(
        ref string acvctamid,
        string empresaid,
        string codEmpresa,
        string cuenta,
        string natCta,
        string codGpo,
        string tipoCta,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);
    }

    #endregion //Interfaz IAcvctam

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvctam
    /// </summary>
    public class Acvctam
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvctam()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvctam A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvctam BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvctam elemento = new Entity.Contabilidad.Acvctam();
            if (!Convert.IsDBNull(row["AcvCtamID"]))
            {
                elemento.Acvctamid = row["AcvCtamID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Nat_Cta"]))
            {
                elemento.NatCta = row["Nat_Cta"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Gpo"]))
            {
                elemento.CodGpo = row["Cod_Gpo"].ToString();
            }
            if (!Convert.IsDBNull(row["Tipo_Cta"]))
            {
                elemento.TipoCta = row["Tipo_Cta"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvctam> listaAcvctam)
        {
            IAcvctam proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvctam>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvctam elemento in listaAcvctam)
                {
                    codigo = (elemento.Acvctamid == string.Empty || elemento.Acvctamid == null || elemento.Acvctamid.ToUpper() == "NULL" || elemento.Acvctamid == "0" ? Guid.Empty.ToString() : elemento.Acvctamid);                    
                    ultimaAct = elemento.UltimaAct;
                    proc.Acvctam_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     elemento.Cuenta,
                     elemento.NatCta,
                     elemento.CodGpo,
                     elemento.TipoCta,
                     elemento.Estatus,
                     elemento.Fecha,
                    ref ultimaAct);
                    elemento.Acvctamid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaAcvctam.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Acvctam> TraerAcvctam()
        {
            IAcvctam proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvctam> listaAcvctam = new ListaDeEntidades<Entity.Contabilidad.Acvctam>();
                proc = Utilerias.GenerarSproc<IAcvctam>();
                DataSet dsAcvctam = proc.Acvctam_Select();
                foreach (DataRow row in dsAcvctam.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvctam elemento = BuildEntity(row, true);
                    listaAcvctam.Add(elemento);
                }
                listaAcvctam.AcceptChanges();
                return listaAcvctam;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvctam TraerAcvctam(string acvctamid)
        {
            IAcvctam proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvctam>();
                Entity.Contabilidad.Acvctam elemento = null;
                DataSet ds = null;
                ds = proc.Acvctam_Select(acvctamid);
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

        public static Entity.Contabilidad.Acvctam TraerAcvctamPorCuenta(string cuenta,string empresaid)
        {
            IAcvctam proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvctam>();
                Entity.Contabilidad.Acvctam elemento = null;
                DataSet ds = null;
                ds = proc.Acvctam_Select_PorCuenta(cuenta, empresaid);
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

        public static System.Data.DataSet TraerAcvctamDS()
        {
            IAcvctam proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvctam>();
                DataSet ds = proc.Acvctam_Select();
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
