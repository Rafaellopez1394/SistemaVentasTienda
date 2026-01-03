using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catsolicitantespago
    /// </summary>
    internal class CatsolicitantespagoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatsolicitantespagoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatsolicitantespago(List<Entity.Contabilidad.Catsolicitantespago> listaCatsolicitantespago)
        {
            MobileDAL.Contabilidad.Catsolicitantespago.Guardar(ref listaCatsolicitantespago);
        }

        public static Entity.Contabilidad.Catsolicitantespago TraerCatsolicitantespago(string solicitanteid)
        {
            return MobileDAL.Contabilidad.Catsolicitantespago.TraerCatsolicitantespago(solicitanteid);
        }

        public static List<Entity.Contabilidad.Catsolicitantespago> TraerCatsolicitantespago()
        {
            return MobileDAL.Contabilidad.Catsolicitantespago.TraerCatsolicitantespago();
        }

        public static System.Data.DataSet TraerCatsolicitantespagoDS()
        {
            return MobileDAL.Contabilidad.Catsolicitantespago.TraerCatsolicitantespagoDS();
        }

        #endregion //Métodos Públicos

    }
}
