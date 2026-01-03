using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Cattipocambio
    /// </summary>
    internal class CattipocambioBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CattipocambioBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCattipocambio(List<Entity.Contabilidad.Cattipocambio> listaCattipocambio)
        {
            MobileDAL.Contabilidad.Cattipocambio.Guardar(ref listaCattipocambio);
        }

        public static Entity.Contabilidad.Cattipocambio TraerCattipocambio(DateTime fechatipocambio)
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambio(fechatipocambio);
        }

        public static List<Entity.Contabilidad.Cattipocambio> TraerCattipocambio()
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambio();
        }

        public static System.Data.DataSet TraerCattipocambioDS()
        {
            return MobileDAL.Contabilidad.Cattipocambio.TraerCattipocambioDS();
        }

        #endregion //Métodos Públicos

    }
}
