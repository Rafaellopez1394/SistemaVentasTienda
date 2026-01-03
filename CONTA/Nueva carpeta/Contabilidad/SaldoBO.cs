using System;
using Entity;
using System.Linq;
using System.Collections.Generic;
using Homex.Core;
using System.Data;
using Homex.Core.Utilities;
using System.Text;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Saldo
    /// </summary>
    internal class SaldoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SaldoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarSaldo(ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldo)
        {
            MobileDAL.Contabilidad.Saldo.Guardar(ref listaSaldo);
        }

        public void ReprosesarSaldos(string EmpresaID)
        {
            MobileDAL.Contabilidad.Saldo.ReprosesarSaldos(EmpresaID);
        }
        public Entity.Contabilidad.Saldo TraerSaldos(string saldoid)
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldos(saldoid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Saldo> TraerSaldos()
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldos();
        }

        public System.Data.DataSet TraerSaldosDS()
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldosDS();
        }

        public ListaDeEntidades<Entity.Contabilidad.Saldo> TraerSaldosPorCuentaEjercicio(string cuenta, string ejercicio, string sucursalid)
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldosPorCuentaEjercicio(cuenta, ejercicio, sucursalid);
        }
        public System.Data.DataSet GeneraInformeSaldos(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, string cuentainicial, string cuentafinal, string nivel, int Ingles, int FormatoCuenta,bool ExcluirDemandados)
        {
            return MobileDAL.Contabilidad.Saldo.GeneraInformeSaldos(fecha1, fecha2, codempresainicial, codempresafinal, cuentainicial, cuentafinal, nivel, Ingles, FormatoCuenta, ExcluirDemandados);
        }
        public System.Data.DataSet GeneraInformeAuxiliarMayor(DateTime fecha1, DateTime fecha2, string codempresainicial, string cuentainicial, string cuentafinal, int Ingles)
        {
            return MobileDAL.Contabilidad.Saldo.GeneraInformeAuxiliarMayor(fecha1, fecha2, codempresainicial, cuentainicial, cuentafinal, Ingles);
        }

        public System.Data.DataSet GeneraInformeIvaAcreditable(DateTime fecha1, DateTime fecha2, string codempresa)
        {
            return MobileDAL.Contabilidad.Saldo.GeneraInformeIvaAcreditable(fecha1, fecha2, codempresa);
        }

        public static System.Data.DataSet GeneraInformeEstadoResultados(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            return MobileDAL.Contabilidad.Saldo.GeneraInformeEstadoResultados(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
        }

        public static System.Data.DataSet GeneraInformeBalanceGeneral(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal, int Ingles)
        {
            return MobileDAL.Contabilidad.Saldo.GeneraInformeBalanceGeneral(fecha1, fecha2, codempresainicial, codempresafinal, Ingles);
        }
        public static DataSet TraerSaldosCuentasBancarias(string EmpresaID, DateTime Fecha)
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldosCuentasBancarias(EmpresaID, Fecha);
        }

        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicio(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldosPorRangodeCuentaEjercicio(empresaid, ejercicio, cuentaInicio, cuentaFinal);
        }
        public static List<Entity.Contabilidad.Saldo> TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(string empresaid, string ejercicio, string cuentaInicio, string cuentaFinal)
        {
            return MobileDAL.Contabilidad.Saldo.TraerSaldosPorRangodeCuentaEjercicioGastosPersonales(empresaid, ejercicio, cuentaInicio, cuentaFinal);
        }
        public static System.Data.DataSet spcgenerainformebalancegeneralDolares(DateTime fecha1, DateTime fecha2, string codempresainicial, string codempresafinal)
        {
            return MobileDAL.Contabilidad.Saldo.spcgenerainformebalancegeneralDolares(fecha1, fecha2, codempresainicial, codempresafinal);
        }
        #endregion //Métodos Públicos

    }
}
