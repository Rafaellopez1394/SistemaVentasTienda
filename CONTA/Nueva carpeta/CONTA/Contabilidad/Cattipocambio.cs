using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
namespace MobileDAL.Contabilidad
{
    #region Interfaz ICattipocambio
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICattipocambio : ISprocBase
    {
        DataSet Cattipocambio_Select(DateTime fechatipocambio);

        DataSet Cattipocambio_Select();

        int Cattipocambio_Save(
        ref DateTime fechatipocambio,
        decimal importetipocambio,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

        int Cattipocambio_SaveBM(
        ref DateTime fechatipocambio,
        decimal importetipocambio,
        string Serie,
        int Tipo,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);
        
        DataSet SP_ReportesEfectoCambiario(int? CodEmpresa, DateTime Fecha);

    }

    #endregion //Interfaz ICattipocambio

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Cattipocambio
    /// </summary>
    public class Cattipocambio
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Cattipocambio()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Cattipocambio A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Cattipocambio BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Cattipocambio elemento = new Entity.Contabilidad.Cattipocambio();
            if (!Convert.IsDBNull(row["FechaTipoCambio"]))
            {
                elemento.Fechatipocambio = DateTime.Parse(row["FechaTipoCambio"].ToString()).ToShortDateString();
            }
            if (!Convert.IsDBNull(row["ImporteTipoCambio"]))
            {
                elemento.Importetipocambio = decimal.Parse(row["ImporteTipoCambio"].ToString());
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

        private static Entity.Contabilidad.Ctlregistroefectocambiario BuildEntityRptEfectoCambiario(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Ctlregistroefectocambiario elemento = new Entity.Contabilidad.Ctlregistroefectocambiario();
            if (row.Table.Columns.Contains("ID") && !Convert.IsDBNull(row["ID"]))
            {
                elemento.Id = int.Parse( row["ID"].ToString() );
            }
            if (row.Table.Columns.Contains("CuentaContable") && !Convert.IsDBNull(row["CuentaContable"]))
            {
                elemento.CuentaContable = row["CuentaContable"].ToString();
            }
            if (row.Table.Columns.Contains("Descripcion") && !Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }
            if (row.Table.Columns.Contains("Nat_Cta") && !Convert.IsDBNull(row["Nat_Cta"]))
            {
                elemento.NatCta = row["Nat_Cta"].ToString();
            }
            if (row.Table.Columns.Contains("Naturaleza") && !Convert.IsDBNull(row["Naturaleza"]))
            {
                elemento.Naturaleza = row["Naturaleza"].ToString();
            }
            if (row.Table.Columns.Contains("Mes") && !Convert.IsDBNull(row["Mes"]))
            {
                elemento.Mes = int.Parse(row["Mes"].ToString());
            }
            if (row.Table.Columns.Contains("NombreMes") && !Convert.IsDBNull(row["NombreMes"]))
            {
                elemento.Nombremes = row["NombreMes"].ToString();
            }
            if (row.Table.Columns.Contains("SaldoAnt") && !Convert.IsDBNull(row["SaldoAnt"]))
            {
                elemento.Saldoant = decimal.Parse(row["SaldoAnt"].ToString());
            }
            if (row.Table.Columns.Contains("SaldoFinal") && !Convert.IsDBNull(row["SaldoFinal"]))
            {
                elemento.Saldofinal = decimal.Parse(row["SaldoFinal"].ToString());
            }
            if (row.Table.Columns.Contains("SaldoComplementaria") && !Convert.IsDBNull(row["SaldoComplementaria"]))
            {
                elemento.Saldocomplementaria = decimal.Parse(row["SaldoComplementaria"].ToString());
            }
            if (row.Table.Columns.Contains("SaldoInicial") && !Convert.IsDBNull(row["SaldoInicial"]))
            {
                elemento.Saldoinicial = decimal.Parse(row["SaldoInicial"].ToString());
            }
            if (row.Table.Columns.Contains("TipoCambioMesAnt") && !Convert.IsDBNull(row["TipoCambioMesAnt"]))
            {
                elemento.Tipocambiomesant = decimal.Parse(row["TipoCambioMesAnt"].ToString());
            }
            if (row.Table.Columns.Contains("TipoCambioFinMes") && !Convert.IsDBNull(row["TipoCambioFinMes"]))
            {
                elemento.Tipocambiofinmes = decimal.Parse(row["TipoCambioFinMes"].ToString());
            }
            if (row.Table.Columns.Contains("TotalSaldoAnt") && !Convert.IsDBNull(row["TotalSaldoAnt"]))
            {
                elemento.Totalsaldoant = decimal.Parse(row["TotalSaldoAnt"].ToString());
            }
            if (row.Table.Columns.Contains("TotalSaldoFinal") && !Convert.IsDBNull(row["TotalSaldoFinal"]))
            {
                elemento.Totalsaldofinal = decimal.Parse(row["TotalSaldoFinal"].ToString());
            }
            if (row.Table.Columns.Contains("UtilidadCambiaria") && !Convert.IsDBNull(row["UtilidadCambiaria"]))
            {
                elemento.Utilidadcambiaria = decimal.Parse(row["UtilidadCambiaria"].ToString());
            }
            if (row.Table.Columns.Contains("PerdidaCambiaria") && !Convert.IsDBNull(row["PerdidaCambiaria"]))
            {
                elemento.Perdidacambiaria = decimal.Parse(row["PerdidaCambiaria"].ToString());
            }
            if (row.Table.Columns.Contains("EfectoCambiario") && !Convert.IsDBNull(row["EfectoCambiario"]))
            {
                elemento.EfectoCambiario = decimal.Parse(row["EfectoCambiario"].ToString());
            }
            /**/
            if (row.Table.Columns.Contains("UtilidadCambiariaAcumulada") && !Convert.IsDBNull(row["UtilidadCambiariaAcumulada"]))
            {
                elemento.UtilidadCambiariaAcumulada = decimal.Parse(row["UtilidadCambiaria"].ToString());
            }
            if (row.Table.Columns.Contains("PerdidaCambiariaAcumulada") && !Convert.IsDBNull(row["PerdidaCambiariaAcumulada"]))
            {
                elemento.PerdidaCambiariaAcumulada = decimal.Parse(row["PerdidaCambiariaAcumulada"].ToString());
            }
            if (row.Table.Columns.Contains("EfectoCambiarioAcumulado") && !Convert.IsDBNull(row["EfectoCambiarioAcumulado"]))
            {
                elemento.EfectoCambiarioAcumulado = decimal.Parse(row["EfectoCambiarioAcumulado"].ToString());
            }
            /**/
            if (row.Table.Columns.Contains("DiferenciaTipoCambio") && !Convert.IsDBNull(row["DiferenciaTipoCambio"]))
            {
                elemento.DiferenciaTipoCambio = decimal.Parse(row["DiferenciaTipoCambio"].ToString());
            }
            if (row.Table.Columns.Contains("Fecha") && !Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (row.Table.Columns.Contains("UltimaAct") && !Convert.IsDBNull(row["UltimaAct"]))
            {
                elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            }
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Cattipocambio> listaCattipocambio)
        {
            ICattipocambio proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICattipocambio>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                DateTime codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Cattipocambio elemento in listaCattipocambio)
                {
                    codigo = DateTime.Parse(elemento.Fechatipocambio);
                    ultimaAct = elemento.UltimaAct;
                    proc.Cattipocambio_Save(
                    ref codigo,
                    (elemento.Importetipocambio != null) ? elemento.Importetipocambio : decimal.MinValue,
                    (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    (elemento.Usuario != null) ? elemento.Usuario : null,
                    (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Fechatipocambio = codigo.ToShortDateString();
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

        public static void GuardarBM(ref List<Entity.Contabilidad.Cattipocambio> listaCattipocambio)
        {
            ICattipocambio proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICattipocambio>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                DateTime codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Cattipocambio elemento in listaCattipocambio)
                {
                    codigo = DateTime.Parse(elemento.Fechatipocambio);
                    ultimaAct = elemento.UltimaAct;
                    proc.Cattipocambio_SaveBM(
                    ref codigo,
                    (elemento.Importetipocambio != null) ? elemento.Importetipocambio : decimal.MinValue,
                    (elemento.Serie != null) ? elemento.Serie : null,
                    (elemento.Tipo != null) ? elemento.Tipo: int.MinValue,
                    (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                    (elemento.Usuario != null) ? elemento.Usuario : null,
                    (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Fechatipocambio = codigo.ToShortDateString();
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
        public static List<Entity.Contabilidad.Cattipocambio> TraerCattipocambio()
        {
            ICattipocambio proc = null;
            try
            {
                List<Entity.Contabilidad.Cattipocambio> listaCattipocambio = new List<Entity.Contabilidad.Cattipocambio>();
                proc = Utilerias.GenerarSproc<ICattipocambio>();
                DataSet dsCattipocambio = proc.Cattipocambio_Select();
                foreach (DataRow row in dsCattipocambio.Tables[0].Rows)
                {
                    Entity.Contabilidad.Cattipocambio elemento = BuildEntity(row, true);
                    listaCattipocambio.Add(elemento);
                }                
                return listaCattipocambio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Cattipocambio TraerCattipocambio(DateTime fechatipocambio)
        {
            ICattipocambio proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICattipocambio>();
                Entity.Contabilidad.Cattipocambio elemento = null;
                DataSet ds = null;
                ds = proc.Cattipocambio_Select(fechatipocambio);
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

        public static System.Data.DataSet TraerCattipocambioDS()
        {
            ICattipocambio proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICattipocambio>();
                DataSet ds = proc.Cattipocambio_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Ctlregistroefectocambiario> ReporteEfectoCambiario(int? CodEmpresa, DateTime Fecha)
        {
            ICattipocambio proc = null;
            try
            {
                List<Entity.Contabilidad.Ctlregistroefectocambiario> listaCattipocambio = new List<Entity.Contabilidad.Ctlregistroefectocambiario>();
                proc = Utilerias.GenerarSproc<ICattipocambio>();
                DataSet dsCattipocambio = proc.SP_ReportesEfectoCambiario(CodEmpresa, Fecha);
                foreach (DataRow row in dsCattipocambio.Tables[0].Rows)
                {
                    Entity.Contabilidad.Ctlregistroefectocambiario elemento = BuildEntityRptEfectoCambiario(row, true);
                    listaCattipocambio.Add(elemento);
                }
                return listaCattipocambio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet ReporteEfectoCambiarioDS(int? CodEmpresa, DateTime Fecha)
        {
            ICattipocambio proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICattipocambio>();
                DataSet ds = proc.SP_ReportesEfectoCambiario(CodEmpresa, Fecha);
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
