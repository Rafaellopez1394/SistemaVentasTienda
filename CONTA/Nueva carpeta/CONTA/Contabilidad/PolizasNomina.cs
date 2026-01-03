using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IPolizasnomina
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IPolizasnomina : ISprocBase
    {
        DataSet Polizasnomina_Select(string polizanominaid);

        DataSet PolizasnominaPorPolizaID_Select(string polizaid);
        DataSet PolizasnominaPorUUID_Select(string uuid);

        DataSet Polizasnomina_Select();

        int Polizasnomina_Save(
            ref string polizanominaid,
            string empresaid,
            string polizaid,
            string serie,
            string folio,
            string uuid,
            string rfcemisor,
            string nombreemisor,
            string rfcreceptor,
            string nombrereceptor,
            decimal sueldo,
            decimal premiopuntualidad,
            decimal premioasistencia,
            decimal isrretenido,
            decimal imss,
            decimal infonavit,
            string nominaxml,
            DateTime fecha,
            int estatus,
            string usuario,
            decimal vacaciones,
            decimal primavacacional,
            decimal aguinaldo,
            decimal gastosmedicosmayores,
            decimal segurodevida,
            decimal indemnizacion,
            decimal primadeantiguedad,
            decimal ptu,
            decimal subsidioalempleo,
            decimal fonacot,
            decimal primaspagadaspatron,
            decimal isrart174,
            decimal prestamoinfonavitcf,
            ref int ultimaAct);

    }

    #endregion //Interfaz IPolizasnomina

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Polizasnomina
    /// </summary>
    public class Polizasnomina
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Polizasnomina()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Polizasnomina A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Polizasnomina BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Polizasnomina elemento = new Entity.Contabilidad.Polizasnomina();
            if (!Convert.IsDBNull(row["PolizaNominaID"]))
            {
                elemento.Polizanominaid = row["PolizaNominaID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["PolizaID"]))
            {
                elemento.Polizaid = row["PolizaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Serie"]))
            {
                elemento.Serie = row["Serie"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio"]))
            {
                elemento.Folio = row["Folio"].ToString();
            }
            if (!Convert.IsDBNull(row["UUID"]))
            {
                elemento.Uuid = row["UUID"].ToString();
            }
            if (!Convert.IsDBNull(row["RFCEmisor"]))
            {
                elemento.Rfcemisor = row["RFCEmisor"].ToString();
            }
            if (!Convert.IsDBNull(row["NombreEmisor"]))
            {
                elemento.Nombreemisor = row["NombreEmisor"].ToString();
            }
            if (!Convert.IsDBNull(row["RFCReceptor"]))
            {
                elemento.Rfcreceptor = row["RFCReceptor"].ToString();
            }
            if (!Convert.IsDBNull(row["NombreReceptor"]))
            {
                elemento.Nombrereceptor = row["NombreReceptor"].ToString();
            }
            if (!Convert.IsDBNull(row["Sueldo"]))
            {
                elemento.Sueldo = decimal.Parse(row["Sueldo"].ToString());
            }
            if (!Convert.IsDBNull(row["PremioPuntualidad"]))
            {
                elemento.Premiopuntualidad = decimal.Parse(row["PremioPuntualidad"].ToString());
            }
            if (!Convert.IsDBNull(row["PremioAsistencia"]))
            {
                elemento.Premioasistencia = decimal.Parse(row["PremioAsistencia"].ToString());
            }
            if (!Convert.IsDBNull(row["IsrRetenido"]))
            {
                elemento.Isrretenido = decimal.Parse(row["IsrRetenido"].ToString());
            }
            if (!Convert.IsDBNull(row["Imss"]))
            {
                elemento.Imss = decimal.Parse(row["Imss"].ToString());
            }
            if (!Convert.IsDBNull(row["Infonavit"]))
            {
                elemento.Infonavit = decimal.Parse(row["Infonavit"].ToString());
            }
            if (!Convert.IsDBNull(row["NominaXML"]))
            {
                elemento.Nominaxml = row["NominaXML"].ToString();
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
            if (!Convert.IsDBNull(row["Vacaciones"]))
            {
                elemento.Vacaciones = decimal.Parse(row["Vacaciones"].ToString());
            }
            if (!Convert.IsDBNull(row["PrimaVacacional"]))
            {
                elemento.Primavacacional = decimal.Parse(row["PrimaVacacional"].ToString());
            }
            if (!Convert.IsDBNull(row["Aguinaldo"]))
            {
                elemento.Aguinaldo = decimal.Parse(row["Aguinaldo"].ToString());
            }
            if (!Convert.IsDBNull(row["GastosMedicosMayores"]))
            {
                elemento.Gastosmedicosmayores = decimal.Parse(row["GastosMedicosMayores"].ToString());
            }
            if (!Convert.IsDBNull(row["SeguroDeVida"]))
            {
                elemento.Segurodevida = decimal.Parse(row["SeguroDeVida"].ToString());
            }
            if (!Convert.IsDBNull(row["Indemnizacion"]))
            {
                elemento.Indemnizacion = decimal.Parse(row["Indemnizacion"].ToString());
            }
            if (!Convert.IsDBNull(row["PrimaDeAntiguedad"]))
            {
                elemento.Primadeantiguedad = decimal.Parse(row["PrimaDeAntiguedad"].ToString());
            }
            if (!Convert.IsDBNull(row["PTU"]))
            {
                elemento.Ptu = decimal.Parse(row["PTU"].ToString());
            }
            if (!Convert.IsDBNull(row["SubsidioAlEmpleo"]))
            {
                elemento.Subsidioalempleo = decimal.Parse(row["SubsidioAlEmpleo"].ToString());
            }
            if (!Convert.IsDBNull(row["Fonacot"]))
            {
                elemento.Fonacot = decimal.Parse(row["Fonacot"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Polizasnomina> listaPolizasnomina)
        {
            IPolizasnomina proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasnomina>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;
                
                foreach (Entity.Contabilidad.Polizasnomina elemento in listaPolizasnomina)
                {
                    codigo = elemento.Polizanominaid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Polizasnomina_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     elemento.Polizaid,
                     (elemento.Serie != null) ? elemento.Serie : null,
                     (elemento.Folio != null) ? elemento.Folio : null,
                     (elemento.Uuid != null) ? elemento.Uuid : null,
                     (elemento.Rfcemisor != null) ? elemento.Rfcemisor : null,
                     (elemento.Nombreemisor != null) ? elemento.Nombreemisor : null,
                     (elemento.Rfcreceptor != null) ? elemento.Rfcreceptor : null,
                     (elemento.Nombrereceptor != null) ? elemento.Nombrereceptor : null,
                     elemento.Sueldo,
                     elemento.Premiopuntualidad,
                     elemento.Premioasistencia,
                     elemento.Isrretenido,
                     elemento.Imss,
                     elemento.Infonavit,
                     (elemento.Nominaxml != null) ? elemento.Nominaxml : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Vacaciones != null) ? elemento.Vacaciones : decimal.MinValue,
                     (elemento.Primavacacional != null) ? elemento.Primavacacional : decimal.MinValue,
                     (elemento.Aguinaldo != null) ? elemento.Aguinaldo : decimal.MinValue,
                     (elemento.Gastosmedicosmayores != null) ? elemento.Gastosmedicosmayores : decimal.MinValue,
                     (elemento.Segurodevida != null) ? elemento.Segurodevida : decimal.MinValue,
                     (elemento.Indemnizacion != null) ? elemento.Indemnizacion : decimal.MinValue,
                     (elemento.Primadeantiguedad != null) ? elemento.Primadeantiguedad : decimal.MinValue,
                     (elemento.Ptu != null) ? elemento.Ptu : decimal.MinValue,
                     (elemento.Subsidioalempleo != null) ? elemento.Subsidioalempleo : decimal.MinValue,
                     (elemento.Fonacot != null) ? elemento.Fonacot : decimal.MinValue,
                     (elemento.Primaspagadaspatron != null) ? elemento.Primaspagadaspatron : decimal.MinValue,
                     (elemento.Isrart174 != null) ? elemento.Isrart174 : decimal.MinValue,
                     (elemento.Prestamoinfonavitcf != null) ? elemento.Prestamoinfonavitcf : decimal.MinValue,
                    ref ultimaAct);
                    elemento.Polizanominaid = codigo;
                    elemento.UltimaAct = ultimaAct;
                    elemento.Polizanominaid = codigo;
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

        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnomina()
        {
            IPolizasnomina proc = null;
            try
            {
                List<Entity.Contabilidad.Polizasnomina> listaPolizasnomina = new List<Entity.Contabilidad.Polizasnomina>();
                proc = Utilerias.GenerarSproc<IPolizasnomina>();
                DataSet dsPolizasnomina = proc.Polizasnomina_Select();
                foreach (DataRow row in dsPolizasnomina.Tables[0].Rows)
                {
                    Entity.Contabilidad.Polizasnomina elemento = BuildEntity(row, true);
                    listaPolizasnomina.Add(elemento);
                }
                return listaPolizasnomina;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Polizasnomina TraerPolizasnomina(string polizanominaid)
        {
            IPolizasnomina proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasnomina>();
                Entity.Contabilidad.Polizasnomina  elemento = null;
                DataSet ds = null;
                ds = proc.Polizasnomina_Select(polizanominaid);
                if (ds.Tables[0].Rows.Count>0)
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

      

        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnominaPorPolizaID(string polizaid)
        {
            IPolizasnomina proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasnomina>();
                List<Entity.Contabilidad.Polizasnomina> elementos = new List<Entity.Contabilidad.Polizasnomina>();
                DataSet ds = null;
                ds = proc.PolizasnominaPorPolizaID_Select(polizaid);
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Entity.Contabilidad.Polizasnomina elemento = BuildEntity(row, false);
                    elementos.Add(elemento);
                }
                return elementos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Polizasnomina TraerPolizasnominaPorUUID(string uuid)
        {
            IPolizasnomina proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasnomina>();
                Entity.Contabilidad.Polizasnomina elemento = null;
                DataSet ds = null;
                ds = proc.PolizasnominaPorUUID_Select(uuid);
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



        public static System.Data.DataSet TraerPolizasnominaDS()
        {
            IPolizasnomina proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPolizasnomina>();
                DataSet ds = proc.Polizasnomina_Select();
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
