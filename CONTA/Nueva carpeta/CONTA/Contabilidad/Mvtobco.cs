using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IMvtobco
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IMvtobco : ISprocBase
    {
        DataSet Mvtobco_Select(string mvtobcoid);

        DataSet Mvtobco_Select();

        int Mvtobco_Save(
        ref string mvtobcoid,
        string empresaid,
        string acvgralid,
        DateTime fecPol,
        string tipPol,
        string numPol,
        int codBco,
        string codMvto,
        decimal importe,
        string cuenta,
        string concepto,
        string beneficiario,
        int codProv,
        string factura,
        string estConc,
        DateTime fecConc,
        string tipMov,
        string codFlujo,
        DateTime fecEnt,
        bool principal,
        DateTime fecBco,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

        DataSet TraerMovimientosContablesBancarios(string EmpresaID, string BancoID);

        DataSet TraerMvtosBancosPorEmpresa(string EmpresaID);

    }

    #endregion //Interfaz IMvtobco

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Mvtobco
    /// </summary>
    public class Mvtobco
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Mvtobco()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Mvtobco A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Mvtobco BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Mvtobco elemento = new Entity.Contabilidad.Mvtobco();
            if (!Convert.IsDBNull(row["MvtoBcoID"]))
            {
                elemento.Mvtobcoid = row["MvtoBcoID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["AcvGralID"]))
            {
                elemento.Acvgralid = row["AcvGralID"].ToString();
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
            if (!Convert.IsDBNull(row["Cod_Bco"]))
            {
                elemento.CodBco = int.Parse(row["Cod_Bco"].ToString());
            }
            if (!Convert.IsDBNull(row["Cod_Mvto"]))
            {
                elemento.CodMvto = row["Cod_Mvto"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Beneficiario"]))
            {
                elemento.Beneficiario = row["Beneficiario"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Prov"]))
            {
                elemento.CodProv = int.Parse(row["Cod_Prov"].ToString());
            }
            if (!Convert.IsDBNull(row["Factura"]))
            {
                elemento.Factura = row["Factura"].ToString();
            }
            if (!Convert.IsDBNull(row["Est_Conc"]))
            {
                elemento.EstConc = row["Est_Conc"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Conc"]))
            {
                elemento.FecConc = DateTime.Parse(row["Fec_Conc"].ToString());
            }
            if (!Convert.IsDBNull(row["Tip_Mov"]))
            {
                elemento.TipMov = row["Tip_Mov"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Flujo"]))
            {
                elemento.CodFlujo = row["Cod_Flujo"].ToString();
            }
            if (!Convert.IsDBNull(row["Fec_Ent"]))
            {
                elemento.FecEnt = DateTime.Parse(row["Fec_Ent"].ToString());
            }
            if (!Convert.IsDBNull(row["Principal"]))
            {
                elemento.Principal = bool.Parse(row["Principal"].ToString());
            }
            if (!Convert.IsDBNull(row["Fec_Bco"]))
            {
                elemento.FecBco = DateTime.Parse(row["Fec_Bco"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Mvtobco> listaMvtobco)
        {
            IMvtobco proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IMvtobco>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Mvtobco elemento in listaMvtobco)
                {
                    codigo = elemento.Mvtobcoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Mvtobco_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Acvgralid != null) ? elemento.Acvgralid : null,
                     (elemento.FecPol != null) ? elemento.FecPol : DateTime.MinValue,
                     (elemento.TipPol != null) ? elemento.TipPol : null,
                     (elemento.NumPol != null) ? elemento.NumPol : null,
                     (elemento.CodBco != null) ? elemento.CodBco : int.MinValue,
                     (elemento.CodMvto != null) ? elemento.CodMvto : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Concepto != null) ? elemento.Concepto : null,
                     (elemento.Beneficiario != null) ? elemento.Beneficiario : null,
                     (elemento.CodProv != null) ? elemento.CodProv : int.MinValue,
                     (elemento.Factura != null) ? elemento.Factura : null,
                     (elemento.EstConc != null) ? elemento.EstConc : null,
                     (elemento.FecConc != null) ? elemento.FecConc : DateTime.MinValue,
                     (elemento.TipMov != null) ? elemento.TipMov : null,
                     (elemento.CodFlujo != null) ? elemento.CodFlujo : null,
                     (elemento.FecEnt != null) ? elemento.FecEnt : DateTime.MinValue,
                     elemento.Principal,
                     (elemento.FecBco != null) ? elemento.FecBco : DateTime.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Mvtobcoid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaMvtobco.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Mvtobco> TraerMvtobco()
        {
            IMvtobco proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Mvtobco> listaMvtobco = new ListaDeEntidades<Entity.Contabilidad.Mvtobco>();
                proc = Utilerias.GenerarSproc<IMvtobco>();
                DataSet dsMvtobco = proc.Mvtobco_Select();
                foreach (DataRow row in dsMvtobco.Tables[0].Rows)
                {
                    Entity.Contabilidad.Mvtobco elemento = BuildEntity(row, true);
                    listaMvtobco.Add(elemento);
                }
                listaMvtobco.AcceptChanges();
                return listaMvtobco;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Mvtobco TraerMvtobco(string mvtobcoid)
        {
            IMvtobco proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IMvtobco>();
                Entity.Contabilidad.Mvtobco elemento = null;
                DataSet ds = null;
                ds = proc.Mvtobco_Select(mvtobcoid);
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

        public static System.Data.DataSet TraerMvtobcoDS()
        {
            IMvtobco proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IMvtobco>();
                DataSet ds = proc.Mvtobco_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerMovimientosContablesBancarios(string EmpresaID, string BancoID) {
            IMvtobco proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IMvtobco>();
                DataSet ds = proc.TraerMovimientosContablesBancarios(EmpresaID, BancoID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerMvtosBancosPorEmpresa(string EmpresaID)
        {
            IMvtobco proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IMvtobco>();
                DataSet ds = proc.TraerMvtosBancosPorEmpresa(EmpresaID);
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
