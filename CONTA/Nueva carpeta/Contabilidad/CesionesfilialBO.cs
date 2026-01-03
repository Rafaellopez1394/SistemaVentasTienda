using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{
    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Cesionesfilial
    /// </summary>
    internal class CesionesfilialBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CesionesfilialBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCesionesfilial(List<Entity.Contabilidad.Cesionesfilial> listaCesionesfilial)
        {
            MobileDAL.Contabilidad.Cesionesfilial.Guardar(ref listaCesionesfilial);
        }

        public static Entity.Contabilidad.Cesionesfilial TraerCesionesfilial(string cesionid)
        {
            return MobileDAL.Contabilidad.Cesionesfilial.TraerCesionesfilial(cesionid);
        }

        public static List<Entity.Contabilidad.Cesionesfilial> TraerCesionesfilial()
        {
            return MobileDAL.Contabilidad.Cesionesfilial.TraerCesionesfilial();
        }

        public static System.Data.DataSet TraerCesionesfilialDS()
        {
            return MobileDAL.Contabilidad.Cesionesfilial.TraerCesionesfilialDS();
        }

        #endregion //Métodos Públicos

    }
}
