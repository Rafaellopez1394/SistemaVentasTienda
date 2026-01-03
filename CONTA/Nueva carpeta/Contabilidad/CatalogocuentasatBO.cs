using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catalogocuentasat
    /// </summary>
    internal class CatalogocuentasatBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatalogocuentasatBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatalogocuentasat(List<Entity.Contabilidad.Catalogocuentasat> listaCatalogocuentasat)
        {
            MobileDAL.Contabilidad.Catalogocuentasat.Guardar(ref listaCatalogocuentasat);
        }

        public static Entity.Contabilidad.Catalogocuentasat TraerCatalogocuentasat(string ctasat)
        {
            return MobileDAL.Contabilidad.Catalogocuentasat.TraerCatalogocuentasat(ctasat);
        }

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasat()
        {
            return MobileDAL.Contabilidad.Catalogocuentasat.TraerCatalogocuentasat();
        }

        public static List<Entity.Contabilidad.Catalogocuentasat> TraerCatalogocuentasatPorDescripcion(string descripcion)
        {
            return MobileDAL.Contabilidad.Catalogocuentasat.TraerCatalogocuentasatPorDescripcion(descripcion);
        }

        public static System.Data.DataSet TraerCatalogocuentasatDS()
        {
            return MobileDAL.Contabilidad.Catalogocuentasat.TraerCatalogocuentasatDS();
        }

        #endregion //Métodos Públicos

    }
}
