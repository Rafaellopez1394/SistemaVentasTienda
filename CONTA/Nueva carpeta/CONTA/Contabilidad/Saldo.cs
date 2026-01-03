using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;
using System.Linq;
using Homex.Core;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ISaldo
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ISaldo : ISprocBase
    {
        DataSet Saldo_Select(string saldoid);

        DataSet Saldo_Select();

        DataSet Saldo_Select_PorCuentaEjercicio(string cuenta, string ejercicio, string empresaid);

        int Saldo_Save(
        ref string saldoid,
        string empresaid,
        string codEmpresa,
        string ejercicio,
        string cuentaid,
        string cuenta,
        int nivel,
        decimal sdoini,
        decimal car1,
        decimal car2,
        decimal car3,
        decimal car4,
        decimal car5,
        decimal car6,
        decimal car7,
        decimal car8,
        decimal car9,
        decimal car10,
        decimal car11,
        decimal car12,
        decimal abo1,
        decimal abo2,
        decimal abo3,
        decimal abo4,
        decimal abo5,
        decimal abo6,
        decimal abo7,
        decimal abo8,
        decimal abo9,
        decimal abo10,
        decimal abo11,
        decimal abo12,
        decimal sdoinia,
        decimal cara1,
        decimal cara2,
        decimal cara3,
        decimal cara4,
        decimal cara5,
        decimal cara6,
        decimal cara7,
        decimal cara8,
        decimal cara9,
        decimal cara10,
        decimal cara11,
        decimal cara12,
        decimal aboa1,
        decimal aboa2,
        decimal aboa3,
        decimal aboa4,
        decimal aboa5,
        decimal aboa6,
        decimal aboa7,
        decimal aboa8,
        decimal aboa9,
        decimal aboa10,
        decimal aboa11,
        decimal aboa12,
        ref int ultimaAct);
        DataSet spcgenerainformeSaldos(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, string cuentainicial, string cuentafinal, string nivel, int Ingles, int FormatoCuenta, bool ExcluirDemandados);
        DataSet spcgenerainformeestadoresultados(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles);
        DataSet spcgenerainformebalancegeneral2(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles);
        DataSet spcgenerainformeauxiliarmayor(DateTime fechaInicio, DateTime fechaFin, string cod_empresa, string cuentainicial, string cuentafinal, int Ingles);
        int ReprosesarSaldos(string EmpresaID);
        DataSet TraerSaldosCuentasBancarias(string EmpresaID, DateTime Fecha);
        DataSet TraerSaldosPorRangodeCuentaEjercicio(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal);
        DataSet TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal);
        //DataSet TraerSaldosPorRangodeCuentaEjercicioGastosPersonalesPrueba(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal);
        DataSet spcgenerainformebalancegeneralDolares(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal);

        DataSet GenerarReporteIvaFiscalAcreditable(string Empresaid, DateTime FechaIni, DateTime FechaFin);
    }

    #endregion //Interfaz ISaldo

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Saldo
    /// </summary>
    public class Saldo
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Saldo()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Saldos A partir de un DataRow
        /// </summary>
        public static Entity.Contabilidad.Saldo BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Saldo elemento = new Entity.Contabilidad.Saldo();
            if (!Convert.IsDBNull(row["SaldoId"]))
            {
                elemento.Saldoid = row["SaldoId"].ToString();
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
                elemento.Ejercicio = row["Ejercicio"].ToString();
            }
            if (!Convert.IsDBNull(row["CuentaID"]))
            {
                elemento.Cuentaid = row["CuentaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Nivel"]))
            {
                elemento.Nivel = int.Parse(row["Nivel"].ToString());
            }
            if (!Convert.IsDBNull(row["SdoIni"]))
            {
                elemento.Sdoini = decimal.Parse(row["SdoIni"].ToString());
            }
            if (!Convert.IsDBNull(row["Car1"]))
            {
                elemento.Car1 = decimal.Parse(row["Car1"].ToString());
            }
            if (!Convert.IsDBNull(row["Car2"]))
            {
                elemento.Car2 = decimal.Parse(row["Car2"].ToString());
            }
            if (!Convert.IsDBNull(row["Car3"]))
            {
                elemento.Car3 = decimal.Parse(row["Car3"].ToString());
            }
            if (!Convert.IsDBNull(row["Car4"]))
            {
                elemento.Car4 = decimal.Parse(row["Car4"].ToString());
            }
            if (!Convert.IsDBNull(row["Car5"]))
            {
                elemento.Car5 = decimal.Parse(row["Car5"].ToString());
            }
            if (!Convert.IsDBNull(row["Car6"]))
            {
                elemento.Car6 = decimal.Parse(row["Car6"].ToString());
            }
            if (!Convert.IsDBNull(row["Car7"]))
            {
                elemento.Car7 = decimal.Parse(row["Car7"].ToString());
            }
            if (!Convert.IsDBNull(row["Car8"]))
            {
                elemento.Car8 = decimal.Parse(row["Car8"].ToString());
            }
            if (!Convert.IsDBNull(row["Car9"]))
            {
                elemento.Car9 = decimal.Parse(row["Car9"].ToString());
            }
            if (!Convert.IsDBNull(row["Car10"]))
            {
                elemento.Car10 = decimal.Parse(row["Car10"].ToString());
            }
            if (!Convert.IsDBNull(row["Car11"]))
            {
                elemento.Car11 = decimal.Parse(row["Car11"].ToString());
            }
            if (!Convert.IsDBNull(row["Car12"]))
            {
                elemento.Car12 = decimal.Parse(row["Car12"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo1"]))
            {
                elemento.Abo1 = decimal.Parse(row["Abo1"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo2"]))
            {
                elemento.Abo2 = decimal.Parse(row["Abo2"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo3"]))
            {
                elemento.Abo3 = decimal.Parse(row["Abo3"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo4"]))
            {
                elemento.Abo4 = decimal.Parse(row["Abo4"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo5"]))
            {
                elemento.Abo5 = decimal.Parse(row["Abo5"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo6"]))
            {
                elemento.Abo6 = decimal.Parse(row["Abo6"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo7"]))
            {
                elemento.Abo7 = decimal.Parse(row["Abo7"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo8"]))
            {
                elemento.Abo8 = decimal.Parse(row["Abo8"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo9"]))
            {
                elemento.Abo9 = decimal.Parse(row["Abo9"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo10"]))
            {
                elemento.Abo10 = decimal.Parse(row["Abo10"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo11"]))
            {
                elemento.Abo11 = decimal.Parse(row["Abo11"].ToString());
            }
            if (!Convert.IsDBNull(row["Abo12"]))
            {
                elemento.Abo12 = decimal.Parse(row["Abo12"].ToString());
            }
            if (!Convert.IsDBNull(row["SdoInia"]))
            {
                elemento.Sdoinia = decimal.Parse(row["SdoInia"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara1"]))
            {
                elemento.Cara1 = decimal.Parse(row["Cara1"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara2"]))
            {
                elemento.Cara2 = decimal.Parse(row["Cara2"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara3"]))
            {
                elemento.Cara3 = decimal.Parse(row["Cara3"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara4"]))
            {
                elemento.Cara4 = decimal.Parse(row["Cara4"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara5"]))
            {
                elemento.Cara5 = decimal.Parse(row["Cara5"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara6"]))
            {
                elemento.Cara6 = decimal.Parse(row["Cara6"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara7"]))
            {
                elemento.Cara7 = decimal.Parse(row["Cara7"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara8"]))
            {
                elemento.Cara8 = decimal.Parse(row["Cara8"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara9"]))
            {
                elemento.Cara9 = decimal.Parse(row["Cara9"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara10"]))
            {
                elemento.Cara10 = decimal.Parse(row["Cara10"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara11"]))
            {
                elemento.Cara11 = decimal.Parse(row["Cara11"].ToString());
            }
            if (!Convert.IsDBNull(row["Cara12"]))
            {
                elemento.Cara12 = decimal.Parse(row["Cara12"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa1"]))
            {
                elemento.Aboa1 = decimal.Parse(row["Aboa1"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa2"]))
            {
                elemento.Aboa2 = decimal.Parse(row["Aboa2"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa3"]))
            {
                elemento.Aboa3 = decimal.Parse(row["Aboa3"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa4"]))
            {
                elemento.Aboa4 = decimal.Parse(row["Aboa4"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa5"]))
            {
                elemento.Aboa5 = decimal.Parse(row["Aboa5"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa6"]))
            {
                elemento.Aboa6 = decimal.Parse(row["Aboa6"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa7"]))
            {
                elemento.Aboa7 = decimal.Parse(row["Aboa7"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa8"]))
            {
                elemento.Aboa8 = decimal.Parse(row["Aboa8"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa9"]))
            {
                elemento.Aboa9 = decimal.Parse(row["Aboa9"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa10"]))
            {
                elemento.Aboa10 = decimal.Parse(row["Aboa10"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa11"]))
            {
                elemento.Aboa11 = decimal.Parse(row["Aboa11"].ToString());
            }
            if (!Convert.IsDBNull(row["Aboa12"]))
            {
                elemento.Aboa12 = decimal.Parse(row["Aboa12"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Saldo elemento in listaSaldos)
                {
                    codigo = elemento.Saldoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Saldo_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     elemento.CodEmpresa,
                     elemento.Ejercicio,
                     (elemento.Cuentaid != null) ? elemento.Cuentaid : null,
                     elemento.Cuenta,
                     elemento.Nivel,
                     elemento.Sdoini,
                     elemento.Car1,
                     elemento.Car2,
                     elemento.Car3,
                     elemento.Car4,
                     elemento.Car5,
                     elemento.Car6,
                     elemento.Car7,
                     elemento.Car8,
                     elemento.Car9,
                     elemento.Car10,
                     elemento.Car11,
                     elemento.Car12,
                     elemento.Abo1,
                     elemento.Abo2,
                     elemento.Abo3,
                     elemento.Abo4,
                     elemento.Abo5,
                     elemento.Abo6,
                     elemento.Abo7,
                     elemento.Abo8,
                     elemento.Abo9,
                     elemento.Abo10,
                     elemento.Abo11,
                     elemento.Abo12,
                     elemento.Sdoinia,
                     elemento.Cara1,
                     elemento.Cara2,
                     elemento.Cara3,
                     elemento.Cara4,
                     elemento.Cara5,
                     elemento.Cara6,
                     elemento.Cara7,
                     elemento.Cara8,
                     elemento.Cara9,
                     elemento.Cara10,
                     elemento.Cara11,
                     elemento.Cara12,
                     elemento.Aboa1,
                     elemento.Aboa2,
                     elemento.Aboa3,
                     elemento.Aboa4,
                     elemento.Aboa5,
                     elemento.Aboa6,
                     elemento.Aboa7,
                     elemento.Aboa8,
                     elemento.Aboa9,
                     elemento.Aboa10,
                     elemento.Aboa11,
                     elemento.Aboa12,
                    ref ultimaAct);
                    elemento.Saldoid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaSaldos.AcceptChanges();
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

        public static void ReprosesarSaldos(string EmpresaID)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.ReprosesarSaldos(EmpresaID);
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

        public static ListaDeEntidades<Entity.Contabilidad.Saldo> TraerSaldos()
        {
            ISaldo proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet dsSaldos = proc.Saldo_Select();
                foreach (DataRow row in dsSaldos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Saldo elemento = BuildEntity(row, true);
                    listaSaldos.Add(elemento);
                }
                listaSaldos.AcceptChanges();
                return listaSaldos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Saldo TraerSaldos(string saldoid)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                Entity.Contabilidad.Saldo elemento = null;
                DataSet ds = null;
                ds = proc.Saldo_Select(saldoid);
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

        public static System.Data.DataSet TraerSaldosDS()
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet ds = proc.Saldo_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Saldo> TraerSaldosPorCuentaEjercicio(string cuenta, string ejercicio, string empresaid)
        {
            ISaldo proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldos = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet dsSaldos = proc.Saldo_Select_PorCuentaEjercicio(cuenta, ejercicio, empresaid);
                foreach (DataRow row in dsSaldos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Saldo elemento = BuildEntity(row, true);
                    listaSaldos.Add(elemento);
                }
                listaSaldos.AcceptChanges();
                return listaSaldos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Saldo TraerSaldoPorCuentaEjercicio(string cuenta, string ejercicio, string empresaId)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>(false);
                Entity.Contabilidad.Saldo elemento = null;
                DataSet ds = null;
                ds = proc.Saldo_Select_PorCuentaEjercicio(cuenta, ejercicio, empresaId);
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
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static DataSet GeneraInformeSaldos(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, string cuentainicial, string cuentafinal, string nivel, int Ingles, int FormatoCuenta,bool ExcluirDemandados)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();                
                DataSet ds = null;
                ds = proc.spcgenerainformeSaldos(fecha1, fecha2, codempresainicial, codempresafinal, cuentainicial, cuentafinal, nivel, Ingles, FormatoCuenta, ExcluirDemandados);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet GeneraInformeEstadoResultados(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();                
                DataSet ds = null;
                ds = proc.spcgenerainformeestadoresultados(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet GeneraInformeBalanceGeneral(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();                
                DataSet ds = null;
                ds = proc.spcgenerainformebalancegeneral2(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet GeneraInformeAuxiliarMayor(DateTime fecha1, DateTime fecha2, string codempresainicial, string cuentainicial, string cuentafinal,int Ingles)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet ds = null;
                ds = proc.spcgenerainformeauxiliarmayor(fecha1, fecha2, codempresainicial, cuentainicial, cuentafinal, Ingles);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public static DataSet GeneraInformeIvaAcreditable(DateTime fecha1, DateTime fecha2, string Empresaid)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet ds = null;
                ds = proc.GenerarReporteIvaFiscalAcreditable(Empresaid, fecha1, fecha2);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet TraerSaldosCuentasBancarias(string EmpresaID, DateTime Fecha)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet ds = null;
                ds = proc.TraerSaldosCuentasBancarias(EmpresaID, Fecha);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicio(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            ISaldo proc = null;
            try
            {
                List<Entity.Contabilidad.Saldo> listaSaldos = new List<Entity.Contabilidad.Saldo>();
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet dsSaldos = proc.TraerSaldosPorRangodeCuentaEjercicio(empresaid, ejercicio, cuentaInicio, cuentaFinal);
                foreach (DataRow row in dsSaldos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Saldo elemento = BuildEntity(row, true);
                    listaSaldos.Add(elemento);
                }
                return listaSaldos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            ISaldo proc = null;
            try
            {
                List<Entity.Contabilidad.Saldo> listaSaldos = new List<Entity.Contabilidad.Saldo>();
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet dsSaldos = proc.TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(empresaid, ejercicio, cuentaInicio, cuentaFinal);
                //DataSet dsSaldos = proc.TraerSaldosPorRangodeCuentaEjercicioGastosPersonalesPrueba(empresaid, ejercicio, cuentaInicio, cuentaFinal);
                foreach (DataRow row in dsSaldos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Saldo elemento = BuildEntity(row, true);
                    listaSaldos.Add(elemento);
                }
                return listaSaldos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet spcgenerainformebalancegeneralDolares(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal)
        {
            ISaldo proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ISaldo>();
                DataSet ds = null;
                ds = proc.spcgenerainformebalancegeneralDolares(fecha1, fecha2, codempresainicial, codempresafinal);
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

