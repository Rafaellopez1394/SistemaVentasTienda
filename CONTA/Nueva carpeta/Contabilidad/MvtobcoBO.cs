using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Mvtobco
    /// </summary>
    internal class MvtobcoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MvtobcoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarMvtobco(ListaDeEntidades<Entity.Contabilidad.Mvtobco> listaMvtobco)
        {
            MobileDAL.Contabilidad.Mvtobco.Guardar(ref listaMvtobco);
        }

        public static Entity.Contabilidad.Mvtobco TraerMvtobco(string mvtobcoid)
        {
            return MobileDAL.Contabilidad.Mvtobco.TraerMvtobco(mvtobcoid);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Mvtobco> TraerMvtobco()
        {
            return MobileDAL.Contabilidad.Mvtobco.TraerMvtobco();
        }

        public static System.Data.DataSet TraerMvtobcoDS()
        {
            return MobileDAL.Contabilidad.Mvtobco.TraerMvtobcoDS();
        }

        public static System.Data.DataSet TraerMovimientosContablesBancarios(string EmpresaID, string BancoID)
        {
            return MobileDAL.Contabilidad.Mvtobco.TraerMovimientosContablesBancarios(EmpresaID, BancoID);
        }

        public static System.Data.DataSet TraerMvtosBancosPorEmpresa(string EmpresaID)
        {
            return MobileDAL.Contabilidad.Mvtobco.TraerMvtosBancosPorEmpresa(EmpresaID);
        }

        #endregion //Métodos Públicos

    }
}
