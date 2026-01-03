using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catcuentascliente
    /// </summary>
    internal class CatcuentasclienteBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatcuentasclienteBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatcuentascliente(ListaDeEntidades<Entity.Contabilidad.Catcuentascliente> listaCatcuentascliente)
        {
            MobileDAL.Contabilidad.Catcuentascliente.Guardar(ref listaCatcuentascliente);
        }

        public static Entity.Contabilidad.Catcuentascliente TraerCatcuentascliente()
        {
            return MobileDAL.Contabilidad.Catcuentascliente.TraerCatcuentascliente();
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuentascliente> TraerCatcuentascliente(string empresaid)
        {
            return MobileDAL.Contabilidad.Catcuentascliente.TraerCatcuentascliente(empresaid);
        }

        public static System.Data.DataSet TraerCatcuentasclienteDS()
        {
            return MobileDAL.Contabilidad.Catcuentascliente.TraerCatcuentasclienteDS();
        }

        #endregion //Métodos Públicos

    }
}
