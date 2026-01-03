using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catcuentaspersonal
    /// </summary>
    internal class CatcuentaspersonalBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatcuentaspersonalBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatcuentaspersonal(List<Entity.Contabilidad.Catcuentaspersonal> listaCatcuentaspersonal)
        {
            MobileDAL.Contabilidad.Catcuentaspersonal.Guardar(ref listaCatcuentaspersonal);
        }

        public static Entity.Contabilidad.Catcuentaspersonal TraerCatcuentaspersonales(string cuentapersonalid)
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerCatcuentaspersonales(cuentapersonalid);
        }

        public static List<Entity.Contabilidad.Catcuentaspersonal> TraerCatcuentaspersonales()
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerCatcuentaspersonales();
        }

        public static System.Data.DataSet TraerCatcuentaspersonalesDS()
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerCatcuentaspersonalesDS();
        }

        public static System.Data.DataSet TraerGastosPersonales(string EmpresaID, string CuentaIni, string CuentaFin, DateTime FechaInicial, DateTime FechaFinal)
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerGastosPersonales(EmpresaID, CuentaIni, CuentaFin, FechaInicial, FechaFinal);
        }

        public static System.Data.DataSet TraerPresupuestoContable(string EmpresaID, int Anio, int Operativo)
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerPresupuestoContable(EmpresaID, Anio, Operativo);
        }

        public static System.Data.DataSet TraerPresupuestoContableVsReal(string EmpresaID, DateTime Fecha, int Operativo)
        {
            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerPresupuestoContableVsReal(EmpresaID, Fecha, Operativo);
        }

        public static System.Data.DataSet TraerInformeProyeccionComparativoEntreYEars2(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {

            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerInformeProyeccionComparativoEntreYEars2(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);

        }

        public static System.Data.DataSet TraerInformeProyeccionMensual(string cuentaini, string cuentafin, string ejercicio, int? cod_empresa, int mes, int opcion)
        {

            return MobileDAL.Contabilidad.Catcuentaspersonal.TraerInformeProyeccionMensual(cuentaini, cuentafin, ejercicio, cod_empresa, mes, opcion);

        }
        

        #endregion //Métodos Públicos

    }
}
