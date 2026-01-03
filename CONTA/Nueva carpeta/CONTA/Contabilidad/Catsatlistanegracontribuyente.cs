using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;


namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatsatlistanegracontribuyente
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatsatlistanegracontribuyente : ISprocBase
    {
        DataSet Catsatlistanegracontribuyente_Select(int numero);

        DataSet Catsatlistanegracontribuyente_Select();

        int Catsatlistanegracontribuyente_Save(
        int numero,
        string rfc,
        string nombre,
        string situacion,
        string numerofechapresuncionsat,
        DateTime publicacionsatpresuntos,
        string numerofechapresunciondof,
        DateTime publicaciondofpresuntos,

        string numerofechadesvirtuaronsat,
        DateTime publicacionsatdesvirtuados,
        string numerofechadesvirtuarondof,
        DateTime publicaciondofdesvirtuados,
        
        string numerofechadefinitivossat,
        DateTime publicacionsatdefinitivos,
        string numerofechadefinitivosdof,
        DateTime publicaciondofdefinitivos,
        
        string numerofechasentenciafavorablesat,
        DateTime publicacionsatsentenciafavorable,
        string numerofechasentenciafavorabledof,
        DateTime publicaciondofsentenciafavorable,
        DateTime fecha,
        int estatus,
        string usuario,
        ref int ultimaAct);

        int Catsatlistanegracontribuyente_DeleteAll();

        DataSet CatsatlistanegracontribuyenteDS_Select(bool SoloBalor, string nombre, string rfc);

        DataSet TraerPolizasProveedorEnListaNegra(string ProveedorID);

        DataSet CatsatlistanegracontribuyenteRfc_Select(string rfc);

        int CatsatlistanegracontribuyenteTEMP_Save(
        int numero,
        string rfc,
        string nombre,
        string situacion,
        string numerofechapresuncionsat,
        DateTime publicacionsatpresuntos,
        string numerofechapresunciondof,
        DateTime publicaciondofpresuntos,

        string numerofechadesvirtuaronsat,
        DateTime publicacionsatdesvirtuados,
        string numerofechadesvirtuarondof,
        DateTime publicaciondofdesvirtuados,

        string numerofechadefinitivossat,
        DateTime publicacionsatdefinitivos,
        string numerofechadefinitivosdof,
        DateTime publicaciondofdefinitivos,

        string numerofechasentenciafavorablesat,
        DateTime publicacionsatsentenciafavorable,
        string numerofechasentenciafavorabledof,
        DateTime publicaciondofsentenciafavorable,
        DateTime fecha,
        int estatus,
        string usuario,
        string sesionid,
        ref int ultimaAct);

        int CatsatlistanegracontribuyenteTEMP_DeleteAll(string usuario, string sesionid);

        int CatsatlistanegracontribuyenteCopiarTEMP_Save(string usuario, string sesionid);
    }

    #endregion //Interfaz ICatsatlistanegracontribuyente

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catsatlistanegracontribuyente
    /// </summary>
    public class Catsatlistanegracontribuyente
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catsatlistanegracontribuyente()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catsatlistanegracontribuyentes A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catsatlistanegracontribuyente BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catsatlistanegracontribuyente elemento = new Entity.Contabilidad.Catsatlistanegracontribuyente();
            if (!Convert.IsDBNull(row["Numero"]))
            {
                elemento.Numero = int.Parse(row["Numero"].ToString());
            }
            if (!Convert.IsDBNull(row["RFC"]))
            {
                elemento.Rfc = row["RFC"].ToString();
            }
            if (!Convert.IsDBNull(row["Nombre"]))
            {
                elemento.Nombre = row["Nombre"].ToString();
            }
            if (!Convert.IsDBNull(row["Situacion"]))
            {
                elemento.Situacion = row["Situacion"].ToString();
            }
            if (!Convert.IsDBNull(row["NumeroFechaPresuncionSat"]))
            {
                elemento.Numerofechapresuncionsat = row["NumeroFechaPresuncionSat"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionSatPresuntos"]))
            {
                elemento.Publicacionsatpresuntos = DateTime.Parse(row["PublicacionSatPresuntos"].ToString());
            }
            if (!Convert.IsDBNull(row["NumeroFechaPresuncionDof"]))
            {
                elemento.Numerofechapresunciondof = row["NumeroFechaPresuncionDof"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionDofPresuntos"]))
            {
                elemento.Publicaciondofpresuntos = DateTime.Parse(row["PublicacionDofPresuntos"].ToString());
            }
            if (!Convert.IsDBNull(row["NumerofechadesvirtuaronSat"]))
            {
                elemento.NumerofechadesvirtuaronSat = row["NumerofechadesvirtuaronSat"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionSatDesvirtuados"]))
            {
                elemento.Publicacionsatdesvirtuados = DateTime.Parse(row["PublicacionSatDesvirtuados"].ToString());
            }
            if (!Convert.IsDBNull(row["NumerofechadesvirtuaronDof"]))
            {
                elemento.NumerofechadesvirtuaronDof = row["NumerofechadesvirtuaronDof"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionDofDesvirtuados"]))
            {
                elemento.Publicaciondofdesvirtuados = DateTime.Parse(row["PublicacionDofDesvirtuados"].ToString());
            }
            if (!Convert.IsDBNull(row["NumerofechadefinitivosSat"]))
            {
                elemento.NumerofechadefinitivosSat = row["NumerofechadefinitivosSat"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionSatDefinitivos"]))
            {
                elemento.Publicacionsatdefinitivos = DateTime.Parse(row["PublicacionSatDefinitivos"].ToString());
            }
            if (!Convert.IsDBNull(row["NumerofechadefinitivosDof"]))
            {
                elemento.NumerofechadefinitivosDof = row["NumerofechadefinitivosDof"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionDofDefinitivos"]))
            {
                elemento.Publicaciondofdefinitivos = DateTime.Parse(row["PublicacionDofDefinitivos"].ToString());
            }
            if (!Convert.IsDBNull(row["NumeroFechaSentenciaFavorableSat"]))
            {
                elemento.Numerofechasentenciafavorablesat = row["NumeroFechaSentenciaFavorableSat"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionSatSentenciaFavorable"]))
            {
                elemento.Publicacionsatsentenciafavorable = DateTime.Parse(row["PublicacionSatSentenciaFavorable"].ToString());
            }
            if (!Convert.IsDBNull(row["NumeroFechaSentenciaFavorableDof"]))
            {
                elemento.Numerofechasentenciafavorabledof = row["NumeroFechaSentenciaFavorableDof"].ToString();
            }
            if (!Convert.IsDBNull(row["PublicacionDofSentenciaFavorable"]))
            {
                elemento.Publicaciondofsentenciafavorable = DateTime.Parse(row["PublicacionDofSentenciaFavorable"].ToString());
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
            if (!Convert.IsDBNull(row["SesionId"]))
            {
                elemento.SesionId = row["SesionId"].ToString();
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyentes)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catsatlistanegracontribuyente elemento in listaCatsatlistanegracontribuyentes)
                {
                    codigo = elemento.Numero;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catsatlistanegracontribuyente_Save(
                     codigo,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Situacion != null) ? elemento.Situacion : null,
                     (elemento.Numerofechapresuncionsat != null) ? elemento.Numerofechapresuncionsat : null,
                     (elemento.Publicacionsatpresuntos != null) ? elemento.Publicacionsatpresuntos : DateTime.MinValue,
                     (elemento.Numerofechapresunciondof != null) ? elemento.Numerofechapresunciondof : null,
                     (elemento.Publicaciondofpresuntos != null) ? elemento.Publicaciondofpresuntos : DateTime.MinValue,

                     (elemento.NumerofechadesvirtuaronSat != null) ? elemento.NumerofechadesvirtuaronSat : null,
                     (elemento.Publicacionsatdesvirtuados != null) ? elemento.Publicacionsatdesvirtuados : DateTime.MinValue,
                     (elemento.NumerofechadesvirtuaronDof != null) ? elemento.NumerofechadesvirtuaronDof : null,
                     (elemento.Publicaciondofdesvirtuados != null) ? elemento.Publicaciondofdesvirtuados : DateTime.MinValue,
                     
                     (elemento.NumerofechadefinitivosSat != null) ? elemento.NumerofechadefinitivosSat : null,
                     (elemento.Publicacionsatdefinitivos != null) ? elemento.Publicacionsatdefinitivos : DateTime.MinValue,
                     (elemento.NumerofechadefinitivosDof != null) ? elemento.NumerofechadefinitivosDof : null,
                     (elemento.Publicaciondofdefinitivos != null) ? elemento.Publicaciondofdefinitivos : DateTime.MinValue,
                     
                     (elemento.Numerofechasentenciafavorablesat != null) ? elemento.Numerofechasentenciafavorablesat : null,
                     (elemento.Publicacionsatsentenciafavorable != null) ? elemento.Publicacionsatsentenciafavorable : DateTime.MinValue,
                     (elemento.Numerofechasentenciafavorabledof != null) ? elemento.Numerofechasentenciafavorabledof : null,
                     (elemento.Publicaciondofsentenciafavorable != null) ? elemento.Publicaciondofsentenciafavorable : DateTime.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                    ref ultimaAct);
                    elemento.Numero = codigo;
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

        public static List<Entity.Contabilidad.Catsatlistanegracontribuyente> TraerCatsatlistanegracontribuyentes()
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyentes = new List<Entity.Contabilidad.Catsatlistanegracontribuyente>();
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                DataSet dsCatsatlistanegracontribuyentes = proc.Catsatlistanegracontribuyente_Select();
                foreach (DataRow row in dsCatsatlistanegracontribuyentes.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catsatlistanegracontribuyente elemento = BuildEntity(row, true);
                    listaCatsatlistanegracontribuyentes.Add(elemento);
                }
                return listaCatsatlistanegracontribuyentes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catsatlistanegracontribuyente TraerCatsatlistanegracontribuyentes(int numero)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                Entity.Contabilidad.Catsatlistanegracontribuyente elemento = null;
                DataSet ds = null;
                ds = proc.Catsatlistanegracontribuyente_Select(numero);
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

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDS(int numero)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                DataSet ds = proc.Catsatlistanegracontribuyente_Select(numero);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EliminarTodo()
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                proc.Catsatlistanegracontribuyente_DeleteAll();
            }
            catch
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesGridDS(bool SoloBalor, string nombre, string rfc)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                DataSet ds = proc.CatsatlistanegracontribuyenteDS_Select(SoloBalor, nombre, rfc);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerPolizasProveedorEnListaNegra(string ProveedorID)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                DataSet ds = proc.TraerPolizasProveedorEnListaNegra(ProveedorID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDesdeRfc(string rfc)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                DataSet ds = proc.CatsatlistanegracontribuyenteRfc_Select(rfc);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void GuardarTEMP(ref List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyentes)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                int codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catsatlistanegracontribuyente elemento in listaCatsatlistanegracontribuyentes)
                {
                    codigo = elemento.Numero;
                    ultimaAct = elemento.UltimaAct;

                    proc.CatsatlistanegracontribuyenteTEMP_Save(
                     codigo,
                     (elemento.Rfc != null) ? elemento.Rfc : null,
                     (elemento.Nombre != null) ? elemento.Nombre : null,
                     (elemento.Situacion != null) ? elemento.Situacion : null,
                     (elemento.Numerofechapresuncionsat != null) ? elemento.Numerofechapresuncionsat : null,
                     (elemento.Publicacionsatpresuntos != null) ? elemento.Publicacionsatpresuntos : DateTime.MinValue,
                     (elemento.Numerofechapresunciondof != null) ? elemento.Numerofechapresunciondof : null,
                     (elemento.Publicaciondofpresuntos != null) ? elemento.Publicaciondofpresuntos : DateTime.MinValue,

                     (elemento.NumerofechadesvirtuaronSat != null) ? elemento.NumerofechadesvirtuaronSat : null,
                     (elemento.Publicacionsatdesvirtuados != null) ? elemento.Publicacionsatdesvirtuados : DateTime.MinValue,
                     (elemento.NumerofechadesvirtuaronDof != null) ? elemento.NumerofechadesvirtuaronDof : null,
                     (elemento.Publicaciondofdesvirtuados != null) ? elemento.Publicaciondofdesvirtuados : DateTime.MinValue,

                     (elemento.NumerofechadefinitivosSat != null) ? elemento.NumerofechadefinitivosSat : null,
                     (elemento.Publicacionsatdefinitivos != null) ? elemento.Publicacionsatdefinitivos : DateTime.MinValue,
                     (elemento.NumerofechadefinitivosDof != null) ? elemento.NumerofechadefinitivosDof : null,
                     (elemento.Publicaciondofdefinitivos != null) ? elemento.Publicaciondofdefinitivos : DateTime.MinValue,

                     (elemento.Numerofechasentenciafavorablesat != null) ? elemento.Numerofechasentenciafavorablesat : null,
                     (elemento.Publicacionsatsentenciafavorable != null) ? elemento.Publicacionsatsentenciafavorable : DateTime.MinValue,
                     (elemento.Numerofechasentenciafavorabledof != null) ? elemento.Numerofechasentenciafavorabledof : null,
                     (elemento.Publicaciondofsentenciafavorable != null) ? elemento.Publicaciondofsentenciafavorable : DateTime.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.SesionId != null) ? elemento.SesionId : null,
                    ref ultimaAct);
                    elemento.Numero = codigo;
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

        public static void EliminarTodoTEMP(string usuario, string sesionid)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>();
                proc.CatsatlistanegracontribuyenteTEMP_DeleteAll(usuario, sesionid);
            }
            catch
            {
                throw;
            }
        }

        public static void CopiarTEMP(string usuario, string sesionid)
        {
            ICatsatlistanegracontribuyente proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatsatlistanegracontribuyente>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                proc.CatsatlistanegracontribuyenteCopiarTEMP_Save(usuario, sesionid);

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
        #endregion Métodos Públicos
    }
}
