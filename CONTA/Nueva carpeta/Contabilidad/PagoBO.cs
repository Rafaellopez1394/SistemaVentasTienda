using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Pago
    /// </summary>
    internal class PagoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PagoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarPago(List<Entity.Contabilidad.Pago> listaPago)
        {
            MobileDAL.Contabilidad.Pago.Guardar(ref listaPago);
        }

        public static Entity.Contabilidad.Pago TraerPagos(string pagoid)
        {
            return MobileDAL.Contabilidad.Pago.TraerPagos(pagoid);
        }

        public static List<Entity.Contabilidad.Pago> TraerPagos()
        {
            return MobileDAL.Contabilidad.Pago.TraerPagos();
        }

        public static System.Data.DataSet TraerPagosDS()
        {
            return MobileDAL.Contabilidad.Pago.TraerPagosDS();
        }

        #endregion //Métodos Públicos

    }
}
