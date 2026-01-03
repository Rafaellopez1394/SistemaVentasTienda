using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catcuenta
    /// </summary>
    internal class CatcuentaBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatcuentaBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatcuenta(ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatcuenta)
        {
            MobileDAL.Contabilidad.Catcuenta.Guardar(ref listaCatcuenta);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta,string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuenta(cuenta, empresaid);
        }

        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta, string nivel1, string nivel2, string empresaid) {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuenta(cuenta, nivel1, nivel2, empresaid);
        }

        public System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas> TraerDatosCuentaContable(string cuentaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerDatosCuentaContable(cuentaid);
        }


        public static Entity.Contabilidad.Catcuenta TraerCatcuentas(string cuentaid,string empresaid,string cuenta)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatcuentas(cuentaid, empresaid, cuenta);
        }

        public System.Data.DataSet TraerFlujos(string flujo, string descripcion)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerFlujos(flujo, descripcion);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatcuentas()
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatcuentas();
        }

        public static System.Data.DataSet TraerCatcuentasDS()
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatcuentasDS();
        }

        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaAfectable(string cuenta, string empresaid) {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuentaAfectable(cuenta, empresaid);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcionAfectable(string descripcion, string empresaid) {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorDescripcionAfectable(descripcion, empresaid);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion,string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorDescripcion(descripcion, empresaid);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion, string nivel1, string nivel2,string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorDescripcion(descripcion, nivel1, nivel2, empresaid);
        }

        public Entity.Contabilidad.Catcuenta TraerCatCuentas(string cuentaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentas(cuentaid);
        }

        public static System.Data.DataSet spcgenerainformedetallecuentas(string empresa)
        {
            return MobileDAL.Contabilidad.Catcuenta.spcgenerainformedetallecuentas(empresa);
        }
        public static System.Data.DataSet TraerUltimaCuentaContable(string empresaid, string Cuenta)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerUltimaCuentaContable(empresaid, Cuenta);
        }

        public static System.Data.DataSet TraerPrimeraCuentaContable(string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerPrimeraCuentaContable(empresaid);
        }
        public static System.Data.DataSet TraerCuentaAfecta(string empresaid, string cuenta)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCuentaAfecta(empresaid, cuenta);
        }

        public static System.Data.DataSet TraerCatCuentasSat(string empresaid, DateTime FechaCorte)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasSat(empresaid, FechaCorte);
        }

        public static bool ValidaCuentasFiscales(string EmpresaID, int Anio)
        {
            return MobileDAL.Contabilidad.Catcuenta.ValidaCuentasFiscales(EmpresaID, Anio);
        }

        public static System.Data.DataSet TraerCatCuentasPorEjercicio(string EmpresaID, int Anio)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorEjercicio(EmpresaID, Anio);
        }

        public static string ValidaCtaSATPorEmpresaCuentaNivel(string EmpresaID, string Cuenta, int Nivel)
        {
            return MobileDAL.Contabilidad.Catcuenta.ValidaCtaSATPorEmpresaCuentaNivel(EmpresaID, Cuenta, Nivel);
        }
        public Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaBLT(string cuenta, string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuenta.TraerCatCuentasPorCuentaBLT(cuenta, empresaid);
        }
        public static bool ValidaCuentasFiscalesBLT(string EmpresaID, int Anio)
        {
            return MobileDAL.Contabilidad.Catcuenta.ValidaCuentasFiscalesBLT(EmpresaID, Anio);
        }
        public List<Entity.Contabilidad.Respuesta> ValidarExistenciaCuenta(string Cuenta, string Descripcion, int CodEmpresa,int DBEmpresa)
        {
            if(DBEmpresa == 8) {
                return MobileDAL.Contabilidad.Catcuenta.ValidarExistenciaCuentasBalorLandTrading(Cuenta, Descripcion, CodEmpresa);
            }else
            {
                return MobileDAL.Contabilidad.Catcuenta.ValidarExistenciaCuentasBalor(Cuenta, Descripcion, CodEmpresa);
            }
        }
        public static void ValidarCuentasCliente(string codigocliente, int cod_empresa)
        {
            MobileDAL.Contabilidad.Catcuenta.ValidarCuentasCliente(codigocliente, cod_empresa);
        }
        
        #endregion //Métodos Públicos

    }
}
