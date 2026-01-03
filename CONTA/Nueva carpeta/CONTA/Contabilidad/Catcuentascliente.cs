using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatcuentascliente
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatcuentascliente : ISprocBase
    {
        DataSet Catcuentascliente_Select(string EmpresaID);

        DataSet Catcuentascliente_Select();

        int Catcuentascliente_Save(
        ref string cuentaclienteid,
        string empresaid,
        string cuentamayor,
        string cuenta,
        string descripcion,
        string descripcioningles,
        int nivel,
        bool afecta,
        bool sistema,
        bool ietu,
        bool isr,
        string flujoCar,
        string flujoAbo,
        int estatus,
        DateTime fecha,
        ref int ultimaAct);

    }

    #endregion //Interfaz ICatcuentascliente

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catcuentascliente
    /// </summary>
    public class Catcuentascliente
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catcuentascliente()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catcuentascliente A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catcuentascliente BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catcuentascliente elemento = new Entity.Contabilidad.Catcuentascliente();
            if (!Convert.IsDBNull(row["CuentaClienteID"]))
            {
                elemento.Cuentaclienteid = row["CuentaClienteID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaMayor"]))
            {
                elemento.Cuentamayor = row["CuentaMayor"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }
            if (!Convert.IsDBNull(row["DescripcionIngles"]))
            {
                elemento.Descripcioningles = row["DescripcionIngles"].ToString();
            }
            if (!Convert.IsDBNull(row["Nivel"]))
            {
                elemento.Nivel = int.Parse(row["Nivel"].ToString());
            }
            if (!Convert.IsDBNull(row["Afecta"]))
            {
                elemento.Afecta = bool.Parse(row["Afecta"].ToString());
            }
            if (!Convert.IsDBNull(row["Sistema"]))
            {
                elemento.Sistema = bool.Parse(row["Sistema"].ToString());
            }
            if (!Convert.IsDBNull(row["IETU"]))
            {
                elemento.Ietu = bool.Parse(row["IETU"].ToString());
            }
            if (!Convert.IsDBNull(row["ISR"]))
            {
                elemento.Isr = bool.Parse(row["ISR"].ToString());
            }
            if (!Convert.IsDBNull(row["Flujo_Car"]))
            {
                elemento.FlujoCar = row["Flujo_Car"].ToString();
            }
            if (!Convert.IsDBNull(row["Flujo_Abo"]))
            {
                elemento.FlujoAbo = row["Flujo_Abo"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["CTASAT"]))
            {
                elemento.CtaSat = row["CTASAT"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Catcuentascliente> listaCatcuentascliente)
        {
            ICatcuentascliente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentascliente>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catcuentascliente elemento in listaCatcuentascliente)
                {
                    codigo = elemento.Cuentaclienteid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catcuentascliente_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Cuentamayor != null) ? elemento.Cuentamayor : null,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Descripcion != null) ? elemento.Descripcion : null,
                     (elemento.Descripcioningles != null) ? elemento.Descripcioningles : null,
                     (elemento.Nivel != null) ? elemento.Nivel : int.MinValue,
                     elemento.Afecta,
                     elemento.Sistema,
                     elemento.Ietu ,
                     elemento.Isr,
                     (elemento.FlujoCar != null) ? elemento.FlujoCar : null,
                     (elemento.FlujoAbo != null) ? elemento.FlujoAbo : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    ref ultimaAct);
                    elemento.Cuentaclienteid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaCatcuentascliente.AcceptChanges();
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

        public static ListaDeEntidades<Entity.Contabilidad.Catcuentascliente> TraerCatcuentascliente(string empresaid)
        {
            ICatcuentascliente proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcuentascliente> listaCatcuentascliente = new ListaDeEntidades<Entity.Contabilidad.Catcuentascliente>();
                proc = Utilerias.GenerarSproc<ICatcuentascliente>();
                DataSet dsCatcuentascliente = proc.Catcuentascliente_Select(empresaid);
                foreach (DataRow row in dsCatcuentascliente.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuentascliente elemento = BuildEntity(row, true);
                    listaCatcuentascliente.Add(elemento);
                }
                listaCatcuentascliente.AcceptChanges();
                return listaCatcuentascliente;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catcuentascliente TraerCatcuentascliente()
        {
            ICatcuentascliente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentascliente>();
                Entity.Contabilidad.Catcuentascliente elemento = null;
                DataSet ds = null;
                ds = proc.Catcuentascliente_Select();
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

        public static System.Data.DataSet TraerCatcuentasclienteDS()
        {
            ICatcuentascliente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuentascliente>();
                DataSet ds = proc.Catcuentascliente_Select();
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
