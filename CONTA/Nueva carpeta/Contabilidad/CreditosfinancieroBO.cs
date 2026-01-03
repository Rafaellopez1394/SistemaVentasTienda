using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Creditosfinanciero
    /// </summary>
    internal class CreditosfinancieroBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CreditosfinancieroBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCreditosfinanciero(List<Entity.Contabilidad.Creditosfinanciero> listaCreditosfinanciero)
        {
            MobileDAL.Contabilidad.Creditosfinanciero.Guardar(ref listaCreditosfinanciero);
        }

        public static Entity.Contabilidad.Creditosfinanciero TraerCreditosfinancieros(int creditofinancieroid)
        {
            return MobileDAL.Contabilidad.Creditosfinanciero.TraerCreditosfinancieros(creditofinancieroid);
        }

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancieros()
        {
            return MobileDAL.Contabilidad.Creditosfinanciero.TraerCreditosfinancieros();
        }

        public static System.Data.DataSet TraerCreditosfinancierosDS()
        {
            return MobileDAL.Contabilidad.Creditosfinanciero.TraerCreditosfinancierosDS();
        }

        public static List<Entity.Contabilidad.Creditosfinanciero> TraerCreditosfinancierosPorFecha(DateTime FechaFinal) {
            return MobileDAL.Contabilidad.Creditosfinanciero.TraerCreditosfinancierosPorFecha(FechaFinal);
        }

        #endregion //Métodos Públicos

    }
}
