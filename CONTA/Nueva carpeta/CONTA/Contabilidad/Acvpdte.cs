using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IAcvpdte
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IAcvpdte : ISprocBase
    {
        DataSet Acvpdte_Select(string acvmovid);

        DataSet Acvpdte_Select();

        DataSet Acvpdte_Select_PorAcvGralPdte(string acvgralid);

        int Acvpdte_Save(
        ref string acvmovid,
        string empresaid,
        string codEmpresa,
        string acvgralid,
        string anomes,
        DateTime fecPol,
        string tipPol,
        string numPol,
        int numRenglon,
        string tipMov,
        string cuenta,
        string concepto,
        string refer,
        string claseConta,
        decimal importe,
        decimal tasaIva,
        decimal iva,
        decimal retencionIva,
        bool pendiente,
        string codFlujo,
        string codProveedor,
        DateTime fechaFiscal,
        string ctaaux,
        string usuario,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);

    }

    #endregion //Interfaz IAcvpdte

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvpdte
    /// </summary>
    public class Acvpdte
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Acvpdte()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvpdte A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Acvpdte BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Acvpdte elemento = new Entity.Contabilidad.Acvpdte();
            if (!Convert.IsDBNull(row["AcvMovID"]))
            {
                elemento.Acvmovid = row["AcvMovID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["AcvGralID"]))
            {
                elemento.Acvgralid = row["AcvGralID"].ToString();
            }
            if (!Convert.IsDBNull(row["AnoMes"]))
            {
                elemento.Anomes = row["AnoMes"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Pol"]))
            {
                elemento.FecPol = DateTime.Parse(row["Fec_Pol"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Num_Pol"]))
            {
                elemento.NumPol = row["Num_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["Num_Renglon"]))
            {
                elemento.NumRenglon = int.Parse(row["Num_Renglon"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Mov"]))
            {
                elemento.TipMov = row["Tip_Mov"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Refer"]))
            {
                elemento.Refer = row["Refer"].ToString();
            }
            if (!Convert.IsDBNull(row["Clase_Conta"]))
            {
                elemento.ClaseConta = row["Clase_Conta"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Tasa_Iva"]))
            {
                elemento.TasaIva = decimal.Parse(row["Tasa_Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Iva"]))
            {
                elemento.Iva = decimal.Parse(row["Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Retencion_Iva"]))
            {
                elemento.RetencionIva = decimal.Parse(row["Retencion_Iva"].ToString());
            }
            if (!Convert.IsDBNull(row["Pendiente"]))
            {
                elemento.Pendiente = bool.Parse(row["Pendiente"].ToString());
            }
            if (!Convert.IsDBNull(row["Cod_Flujo"]))
            {
                elemento.CodFlujo = row["Cod_Flujo"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Proveedor"]))
            {
                elemento.CodProveedor = row["Cod_Proveedor"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha_Fiscal"]))
            {
                elemento.FechaFiscal = DateTime.Parse(row["Fecha_Fiscal"].ToString());
            }
            if (!Convert.IsDBNull(row["CtaAux"]))
            {
                elemento.Ctaaux = row["CtaAux"].ToString();
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
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
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaAcvpdte)
        {
            IAcvpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvpdte>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Acvpdte elemento in listaAcvpdte)
                {
                    codigo = elemento.Acvmovid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Acvpdte_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     (elemento.Acvgralid != null) ? elemento.Acvgralid : null,
                     elemento.Anomes,
                     elemento.FecPol,
                     elemento.TipPol,
                     elemento.NumPol,
                     elemento.NumRenglon,
                     elemento.TipMov,
                     elemento.Cuenta,
                     elemento.Concepto,
                     elemento.Refer,
                     elemento.ClaseConta,
                     elemento.Importe,
                     elemento.TasaIva,
                     elemento.Iva,
                     elemento.RetencionIva,
                     elemento.Pendiente,
                     elemento.CodFlujo,
                     elemento.CodProveedor,
                     elemento.FechaFiscal,
                     elemento.Ctaaux,
                     elemento.Usuario,
                     elemento.Estatus,
                     elemento.Fecha,
                    ref ultimaAct);
                    elemento.Acvmovid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaAcvpdte.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Acvpdte> TraerAcvpdte()
        {
            IAcvpdte proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaAcvpdte = new ListaDeEntidades<Entity.Contabilidad.Acvpdte>();
                proc = Utilerias.GenerarSproc<IAcvpdte>();
                DataSet dsAcvpdte = proc.Acvpdte_Select();
                foreach (DataRow row in dsAcvpdte.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvpdte elemento = BuildEntity(row, true);
                    listaAcvpdte.Add(elemento);
                }
                listaAcvpdte.AcceptChanges();
                return listaAcvpdte;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Acvpdte TraerAcvpdte(string acvmovid)
        {
            IAcvpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvpdte>();
                Entity.Contabilidad.Acvpdte elemento = null;
                DataSet ds = null;
                ds = proc.Acvpdte_Select(acvmovid);
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

        public static System.Data.DataSet TraerAcvpdteDS()
        {
            IAcvpdte proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvpdte>();
                DataSet ds = proc.Acvpdte_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Acvpdte> TraerAcvpdtePorAcvGralPdte(string acvgralid)
        {
            IAcvpdte proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaAcvpdte = new ListaDeEntidades<Entity.Contabilidad.Acvpdte>();
                proc = Utilerias.GenerarSproc<IAcvpdte>();
                DataSet dsAcvpdte = proc.Acvpdte_Select_PorAcvGralPdte(acvgralid);
                foreach (DataRow row in dsAcvpdte.Tables[0].Rows)
                {
                    Entity.Contabilidad.Acvpdte elemento = BuildEntity(row, true);
                    listaAcvpdte.Add(elemento);
                }
                listaAcvpdte.AcceptChanges();
                return listaAcvpdte;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Métodos Públicos
    }
}
