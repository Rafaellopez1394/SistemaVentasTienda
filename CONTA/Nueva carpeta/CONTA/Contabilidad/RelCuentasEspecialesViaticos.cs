using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IRelcuentasespecialesviatico
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IRelcuentasespecialesviatico : ISprocBase
    {
        DataSet Relcuentasespecialesviatico_Select(string relcuentaespecialviaticoid, string cuentaviaticos);

        DataSet Relcuentasespecialesviatico_Select();

        int Relcuentasespecialesviatico_Save(
        ref string relcuentaespecialviaticoid,
        string cuentagastos,
        string cuentagastosreemplazo,
        string cuentadeudor,
        string cuentadeudorreemplazo,
        string cuentaacreedor,
        string cuentaacreedorreemplazo,
        DateTime fecha,
        int estatus,
        string usuario,
        string nombreempleado,
        ref int ultimaAct);

    }

    #endregion //Interfaz IRelcuentasespecialesviatico

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Relcuentasespecialesviatico
    /// </summary>
    public class Relcuentasespecialesviatico
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Relcuentasespecialesviatico()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Relcuentasespecialesviaticos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Relcuentasespecialesviatico BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Relcuentasespecialesviatico elemento = new Entity.Contabilidad.Relcuentasespecialesviatico();
            if (!Convert.IsDBNull(row["RelCuentaEspecialViaticoID"]))
            {
                elemento.Relcuentaespecialviaticoid = row["RelCuentaEspecialViaticoID"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaGastos"]))
            {
                elemento.Cuentagastos = row["CuentaGastos"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaGastosReemplazo"]))
            {
                elemento.Cuentagastosreemplazo = row["CuentaGastosReemplazo"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaDeudor"]))
            {
                elemento.Cuentadeudor = row["CuentaDeudor"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaDeudorReemplazo"]))
            {
                elemento.Cuentadeudorreemplazo = row["CuentaDeudorReemplazo"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaAcreedor"]))
            {
                elemento.Cuentaacreedor = row["CuentaAcreedor"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaAcreedorReemplazo"]))
            {
                elemento.Cuentaacreedorreemplazo = row["CuentaAcreedorReemplazo"].ToString();
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
            if (!Convert.IsDBNull(row["NombreEmpleado"]))
            {
                elemento.Nombreempleado = row["NombreEmpleado"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Relcuentasespecialesviatico> listaRelcuentasespecialesviaticos)
        {
            IRelcuentasespecialesviatico proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IRelcuentasespecialesviatico>();
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Relcuentasespecialesviatico elemento in listaRelcuentasespecialesviaticos)
                {
                    codigo = elemento.Relcuentaespecialviaticoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Relcuentasespecialesviatico_Save(
                    ref codigo,
                     (elemento.Cuentagastos != null) ? elemento.Cuentagastos : null,
                     (elemento.Cuentagastosreemplazo != null) ? elemento.Cuentagastosreemplazo : null,
                     (elemento.Cuentadeudor != null) ? elemento.Cuentadeudor : null,
                     (elemento.Cuentadeudorreemplazo != null) ? elemento.Cuentadeudorreemplazo : null,
                     (elemento.Cuentaacreedor != null) ? elemento.Cuentaacreedor : null,
                     (elemento.Cuentaacreedorreemplazo != null) ? elemento.Cuentaacreedorreemplazo : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Nombreempleado != null) ? elemento.Nombreempleado : null,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Relcuentaespecialviaticoid = codigo;
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

        public static List<Entity.Contabilidad.Relcuentasespecialesviatico> TraerRelcuentasespecialesviaticos()
        {
            IRelcuentasespecialesviatico proc = null;
            try
            {
                List<Entity.Contabilidad.Relcuentasespecialesviatico> listaRelcuentasespecialesviaticos = new List<Entity.Contabilidad.Relcuentasespecialesviatico>();
                proc = Utilerias.GenerarSproc<IRelcuentasespecialesviatico>();
                DataSet dsRelcuentasespecialesviaticos = proc.Relcuentasespecialesviatico_Select();
                foreach (DataRow row in dsRelcuentasespecialesviaticos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Relcuentasespecialesviatico elemento = BuildEntity(row, true);
                    listaRelcuentasespecialesviaticos.Add(elemento);
                }
                return listaRelcuentasespecialesviaticos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Relcuentasespecialesviatico TraerRelcuentasespecialesviaticos(string relcuentaespecialviaticoid, string cuentaviaticos)
        {
            IRelcuentasespecialesviatico proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IRelcuentasespecialesviatico>();
                Entity.Contabilidad.Relcuentasespecialesviatico elemento = null;
                DataSet ds = null;
                ds = proc.Relcuentasespecialesviatico_Select(relcuentaespecialviaticoid, cuentaviaticos);
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



        public static System.Data.DataSet TraerRelcuentasespecialesviaticosDS()
        {
            IRelcuentasespecialesviatico proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IRelcuentasespecialesviatico>();
                DataSet ds = proc.Relcuentasespecialesviatico_Select();
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
