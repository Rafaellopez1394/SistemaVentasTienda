using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catcierre
    /// </summary>
    internal class CatcierreBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatcierreBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatcierre(ListaDeEntidades<Entity.Contabilidad.Catcierre> listaCatcierre)
        {
            MobileDAL.Contabilidad.Catcierre.Guardar(ref listaCatcierre);
        }

        public static Entity.Contabilidad.Catcierre TraerCatcierre(string cierreid,string empresaid,DateTime fecha)
        {
            return MobileDAL.Contabilidad.Catcierre.TraerCatcierre(cierreid, empresaid, fecha);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcierre> TraerCatcierre()
        {
            return MobileDAL.Contabilidad.Catcierre.TraerCatcierre();
        }

        public static System.Data.DataSet TraerCatcierreDS()
        {
            return MobileDAL.Contabilidad.Catcierre.TraerCatcierreDS();
        }

        public static System.Data.DataSet TraerUltimoCierre(string EmpresaID) {
            return MobileDAL.Contabilidad.Catcierre.TraerUltimoCierre(EmpresaID);
        }

        public static System.Data.DataSet TraerAlertasNotificarB1(string VendedorID)
        {
            return MobileDAL.Contabilidad.Catcierre.TraerAlertasNotificarB1(VendedorID);
        }

        public static System.Data.DataSet TraerAlertasNotificarA1(string VendedorID)
        {
            return MobileDAL.Contabilidad.Catcierre.TraerAlertasNotificarA1(VendedorID);
        }        

        #endregion //Métodos Públicos

    }
}
