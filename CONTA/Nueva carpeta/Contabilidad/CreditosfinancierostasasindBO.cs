using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Creditosfinancierostasasind
    /// </summary>
    internal class CreditosfinancierostasasindBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CreditosfinancierostasasindBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCreditosfinancierostasasind(ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> listaCreditosfinancierostasasind)
        {
            MobileDAL.Contabilidad.Creditosfinancierostasasind.Guardar(ref listaCreditosfinancierostasasind);
        }

        public static Entity.Contabilidad.Creditosfinancierostasasind TraerCreditosfinancierostasasind(string creditosfinancierostasatiieid, int? CreditoFinancieroID, int? Anio, int? Mes)
        {
            return MobileDAL.Contabilidad.Creditosfinancierostasasind.TraerCreditosfinancierostasasind(creditosfinancierostasatiieid, CreditoFinancieroID, Anio, Mes);
        }

        public static ListaDeEntidades<Entity.Contabilidad.Creditosfinancierostasasind> TraerCreditosfinancierostasasind()
        {
            return MobileDAL.Contabilidad.Creditosfinancierostasasind.TraerCreditosfinancierostasasind();
        }

        public static System.Data.DataSet TraerCreditosfinancierostasasindDS()
        {
            return MobileDAL.Contabilidad.Creditosfinancierostasasind.TraerCreditosfinancierostasasindDS();
        }

        #endregion //Métodos Públicos

    }
}
