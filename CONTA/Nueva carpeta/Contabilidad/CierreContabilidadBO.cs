using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Cierrecontabilidad
    /// </summary>
    internal class CierrecontabilidadBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CierrecontabilidadBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCierrecontabilidad(List<Entity.Contabilidad.Cierrecontabilidad> listaCierrecontabilidad)
        {
            MobileDAL.Contabilidad.Cierrecontabilidad.Guardar(ref listaCierrecontabilidad);
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(int cierrecontableid)
        {
            return MobileDAL.Contabilidad.Cierrecontabilidad.TraerCierrecontabilidad(cierrecontableid);
        }

        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidad(string empresaid)
        {
            return MobileDAL.Contabilidad.Cierrecontabilidad.TraerCierrecontabilidad(empresaid);
        }

        public static List<Entity.Contabilidad.Cierrecontabilidad> TraerCierrecontabilidad()
        {
            return MobileDAL.Contabilidad.Cierrecontabilidad.TraerCierrecontabilidad();
        }

        public static System.Data.DataSet TraerCierrecontabilidadDS()
        {
            return MobileDAL.Contabilidad.Cierrecontabilidad.TraerCierrecontabilidadDS();
        }
        public static Entity.Contabilidad.Cierrecontabilidad TraerCierrecontabilidadBLT(string empresaid)
        {
            return MobileDAL.Contabilidad.Cierrecontabilidad.TraerCierrecontabilidadBLT(empresaid);
        }
        #endregion //Métodos Públicos

    }
}
