using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatProyeccion
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatProyeccion : ISprocBase
    {
        DataSet CatProyeccion_Select(string proyeccionid, int? Cod_Empresa, int? Ejercicio, string Cuenta);

        DataSet Catproyeccion_Select_Acumulado(int Cod_Empresa, int Ejercicio, string Cuenta, int Nivel);

        DataSet CatProyeccion_Select();

        int CatProyeccion_Save(
        ref string proyeccionid,
        int codEmpresa,
        int ejercicio,
        string cuenta,
        int nivel,
        bool capturado,
        decimal presupuesto1,
        decimal presupuesto2,
        decimal presupuesto3,
        decimal presupuesto4,
        decimal presupuesto5,
        decimal presupuesto6,
        decimal presupuesto7,
        decimal presupuesto8,
        decimal presupuesto9,
        decimal presupuesto10,
        decimal presupuesto11,
        decimal presupuesto12,
        int estatus,
        string usuario,
        DateTime fecha,
        ref int ultimaAct,
        string observacion1,
        string observacion2,
        string observacion3,
        string observacion4,
        string observacion5,
        string observacion6,
        string observacion7,
        string observacion8,
        string observacion9,
        string observacion10,
        string observacion11,
        string observacion12);

    }

    #endregion //Interfaz ICatProyeccion

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de CatProyeccion
    /// </summary>
    public class CatProyeccion
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatProyeccion()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo CatProyeccion A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.CatProyeccion BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.CatProyeccion elemento = new Entity.Contabilidad.CatProyeccion();
            if (!Convert.IsDBNull(row["ProyeccionID"]))
            {
                elemento.Proyeccionid = row["ProyeccionID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = int.Parse(row["Cod_Empresa"].ToString());
            }
            if (!Convert.IsDBNull(row["Ejercicio"]))
            {
                elemento.Ejercicio = int.Parse(row["Ejercicio"].ToString());
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Nivel"]))
            {
                elemento.Nivel = int.Parse(row["Nivel"].ToString());
            }
            if (!Convert.IsDBNull(row["capturado"]))
            {
                elemento.Capturado = bool.Parse(row["capturado"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto1"]))
            {
                elemento.Presupuesto1 = decimal.Parse(row["Presupuesto1"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto2"]))
            {
                elemento.Presupuesto2 = decimal.Parse(row["Presupuesto2"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto3"]))
            {
                elemento.Presupuesto3 = decimal.Parse(row["Presupuesto3"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto4"]))
            {
                elemento.Presupuesto4 = decimal.Parse(row["Presupuesto4"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto5"]))
            {
                elemento.Presupuesto5 = decimal.Parse(row["Presupuesto5"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto6"]))
            {
                elemento.Presupuesto6 = decimal.Parse(row["Presupuesto6"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto7"]))
            {
                elemento.Presupuesto7 = decimal.Parse(row["Presupuesto7"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto8"]))
            {
                elemento.Presupuesto8 = decimal.Parse(row["Presupuesto8"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto9"]))
            {
                elemento.Presupuesto9 = decimal.Parse(row["Presupuesto9"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto10"]))
            {
                elemento.Presupuesto10 = decimal.Parse(row["Presupuesto10"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto11"]))
            {
                elemento.Presupuesto11 = decimal.Parse(row["Presupuesto11"].ToString());
            }
            if (!Convert.IsDBNull(row["Presupuesto12"]))
            {
                elemento.Presupuesto12 = decimal.Parse(row["Presupuesto12"].ToString());
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Observacion1"]))
            {
                elemento.Observacion1 = row["Observacion1"].ToString();
            }else
            {
                elemento.Observacion1 = "";
            }
            if (!Convert.IsDBNull(row["Observacion2"]))
            {
                elemento.Observacion2 = row["Observacion2"].ToString();
            }
            else
            {
                elemento.Observacion2 = "";
            }
            if (!Convert.IsDBNull(row["Observacion3"]))
            {
                elemento.Observacion3 = row["Observacion3"].ToString();
            }
            else
            {
                elemento.Observacion3 = "";
            }
            if (!Convert.IsDBNull(row["Observacion4"]))
            {
                elemento.Observacion4 = row["Observacion4"].ToString();
            }
            else
            {
                elemento.Observacion4 = "";
            }
            if (!Convert.IsDBNull(row["Observacion5"]))
            {
                elemento.Observacion5 = row["Observacion5"].ToString();
            }
            else
            {
                elemento.Observacion5 = "";
            }
            if (!Convert.IsDBNull(row["Observacion6"]))
            {
                elemento.Observacion6 = row["Observacion6"].ToString();
            }
            else
            {
                elemento.Observacion6 = "";
            }
            if (!Convert.IsDBNull(row["Observacion7"]))
            {
                elemento.Observacion7 = row["Observacion7"].ToString();
            }
            else
            {
                elemento.Observacion7 = "";
            }
            if (!Convert.IsDBNull(row["Observacion8"]))
            {
                elemento.Observacion8 = row["Observacion8"].ToString();
            }
            else
            {
                elemento.Observacion8 = "";
            }
            if (!Convert.IsDBNull(row["Observacion9"]))
            {
                elemento.Observacion9 = row["Observacion9"].ToString();
            }
            else
            {
                elemento.Observacion9 = "";
            }
            if (!Convert.IsDBNull(row["Observacion10"]))
            {
                elemento.Observacion10 = row["Observacion10"].ToString();
            }
            else
            {
                elemento.Observacion10 = "";
            }
            if (!Convert.IsDBNull(row["Observacion11"]))
            {
                elemento.Observacion11 = row["Observacion11"].ToString();
            }
            else
            {
                elemento.Observacion11 = "";
            }
            if (!Convert.IsDBNull(row["Observacion12"]))
            {
                elemento.Observacion12 = row["Observacion12"].ToString();
            }
            else
            {
                elemento.Observacion12 = "";
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion)
        {
            ICatProyeccion proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatProyeccion>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.CatProyeccion elemento in listaCatProyeccion)
                {
                    codigo = elemento.Proyeccionid;
                    ultimaAct = elemento.UltimaAct;

                    proc.CatProyeccion_Save(
                    ref codigo,
                     (elemento.CodEmpresa != null) ? elemento.CodEmpresa : int.MinValue,
                     (elemento.Ejercicio != null) ? elemento.Ejercicio : int.MinValue,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Nivel != null) ? elemento.Nivel : int.MinValue,
                     elemento.Capturado,
                     (elemento.Presupuesto1 != null) ? elemento.Presupuesto1 : decimal.MinValue,
                     (elemento.Presupuesto2 != null) ? elemento.Presupuesto2 : decimal.MinValue,
                     (elemento.Presupuesto3 != null) ? elemento.Presupuesto3 : decimal.MinValue,
                     (elemento.Presupuesto4 != null) ? elemento.Presupuesto4 : decimal.MinValue,
                     (elemento.Presupuesto5 != null) ? elemento.Presupuesto5 : decimal.MinValue,
                     (elemento.Presupuesto6 != null) ? elemento.Presupuesto6 : decimal.MinValue,
                     (elemento.Presupuesto7 != null) ? elemento.Presupuesto7 : decimal.MinValue,
                     (elemento.Presupuesto8 != null) ? elemento.Presupuesto8 : decimal.MinValue,
                     (elemento.Presupuesto9 != null) ? elemento.Presupuesto9 : decimal.MinValue,
                     (elemento.Presupuesto10 != null) ? elemento.Presupuesto10 : decimal.MinValue,
                     (elemento.Presupuesto11 != null) ? elemento.Presupuesto11 : decimal.MinValue,
                     (elemento.Presupuesto12 != null) ? elemento.Presupuesto12 : decimal.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     ref ultimaAct,
                     (elemento.Observacion1 != "") ? elemento.Observacion1 : null,
                     (elemento.Observacion2 != "") ? elemento.Observacion2 : null,
                     (elemento.Observacion3 != "") ? elemento.Observacion3 : null,
                     (elemento.Observacion4 != "") ? elemento.Observacion4 : null,
                     (elemento.Observacion5 != "") ? elemento.Observacion5 : null,
                     (elemento.Observacion6 != "") ? elemento.Observacion6 : null,
                     (elemento.Observacion7 != "") ? elemento.Observacion7 : null,
                     (elemento.Observacion8 != "") ? elemento.Observacion8 : null,
                     (elemento.Observacion9 != "") ? elemento.Observacion9 : null,
                     (elemento.Observacion10 != "") ? elemento.Observacion10 : null,
                     (elemento.Observacion11 != "") ? elemento.Observacion11 : null,
                     (elemento.Observacion12 != "") ? elemento.Observacion12 : null);
                    elemento.Proyeccionid = codigo;
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

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccion()
        {
            ICatProyeccion proc = null;
            try
            {
                List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion = new List<Entity.Contabilidad.CatProyeccion>();
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                DataSet dsCatProyeccion = proc.CatProyeccion_Select();
                foreach (DataRow row in dsCatProyeccion.Tables[0].Rows)
                {
                    Entity.Contabilidad.CatProyeccion elemento = BuildEntity(row, true);
                    listaCatProyeccion.Add(elemento);
                }                
                return listaCatProyeccion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccion(string proyeccionid)
        {
            ICatProyeccion proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                Entity.Contabilidad.CatProyeccion elemento = null;
                DataSet ds = null;
                ds = proc.CatProyeccion_Select(proyeccionid, null, null, null);
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

        public static System.Data.DataSet TraerCatProyeccionDS()
        {
            ICatProyeccion proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                DataSet ds = proc.CatProyeccion_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccionPorEjercicio(int empresa,int Ejercicio)
        {
            ICatProyeccion proc = null;
            try
            {
                List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion = new List<Entity.Contabilidad.CatProyeccion>();
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                DataSet dsCatProyeccion = proc.CatProyeccion_Select(null, empresa, Ejercicio, null);
                foreach (DataRow row in dsCatProyeccion.Tables[0].Rows)
                {
                    Entity.Contabilidad.CatProyeccion elemento = BuildEntity(row, true);
                    listaCatProyeccion.Add(elemento);
                }
                return listaCatProyeccion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccionPorCuenta(int empresa, int ejercicio,string cuenta)
        {
            ICatProyeccion proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                Entity.Contabilidad.CatProyeccion elemento = null;
                DataSet ds = null;
                ds = proc.CatProyeccion_Select(null, empresa, ejercicio, cuenta);
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


        public static List<Entity.Contabilidad.CatProyeccion> Catproyeccion_Select_Acumulado(int empresa, int Ejercicio,string Cuenta,int Nivel)
        {
            ICatProyeccion proc = null;
            try
            {
                List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion = new List<Entity.Contabilidad.CatProyeccion>();
                proc = Utilerias.GenerarSproc<ICatProyeccion>();
                DataSet dsCatProyeccion = proc.Catproyeccion_Select_Acumulado(empresa, Ejercicio, Cuenta, Nivel);
                foreach (DataRow row in dsCatProyeccion.Tables[0].Rows)
                {
                    Entity.Contabilidad.CatProyeccion elemento = BuildEntity(row, true);
                    listaCatProyeccion.Add(elemento);
                }
                return listaCatProyeccion;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion Métodos Públicos
    }
}
