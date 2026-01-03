using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Creditosfinancierosdetalle
    /// </summary>
    internal class CreditosfinancierosdetalleBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CreditosfinancierosdetalleBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCreditosfinancierosdetalle(List<Entity.Contabilidad.Creditosfinancierosdetalle> listaCreditosfinancierosdetalle)
        {
            MobileDAL.Contabilidad.Creditosfinancierosdetalle.Guardar(ref listaCreditosfinancierosdetalle);
        }

        public static Entity.Contabilidad.Creditosfinancierosdetalle TraerCreditosfinancierosdetalle(string creditofinancierodetalleid)
        {
            return MobileDAL.Contabilidad.Creditosfinancierosdetalle.TraerCreditosfinancierosdetalle(creditofinancierodetalleid);
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> TraerCreditosfinancierosdetalle()
        {
            return MobileDAL.Contabilidad.Creditosfinancierosdetalle.TraerCreditosfinancierosdetalle();
        }

        public static System.Data.DataSet TraerCreditosfinancierosdetalleDS()
        {
            return MobileDAL.Contabilidad.Creditosfinancierosdetalle.TraerCreditosfinancierosdetalleDS();
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> Creditosfinancierosdetalle_SelectTipoFecha(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            return MobileDAL.Contabilidad.Creditosfinancierosdetalle.Creditosfinancierosdetalle_SelectTipoFecha(CreditoFinancieroID, Tipo_Mov, Fecha);
        }

        public static List<Entity.Contabilidad.Creditosfinancierosdetalle> TraerDetalleCreditosFinancierosContabilidad(int CreditoFinancieroID, int Tipo_Mov, DateTime Fecha)
        {
            return MobileDAL.Contabilidad.Creditosfinancierosdetalle.TraerDetalleCreditosFinancierosContabilidad(CreditoFinancieroID, Tipo_Mov, Fecha);
        }

        #endregion //Métodos Públicos

    }
}
