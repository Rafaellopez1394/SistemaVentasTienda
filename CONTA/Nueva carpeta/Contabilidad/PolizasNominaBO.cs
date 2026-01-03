using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Polizasnomina
    /// </summary>
    internal class PolizasnominaBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PolizasnominaBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarPolizasnomina(List<Entity.Contabilidad.Polizasnomina> listaPolizasnomina)
        {
            MobileDAL.Contabilidad.Polizasnomina.Guardar(ref listaPolizasnomina);
        }

        public static Entity.Contabilidad.Polizasnomina TraerPolizasnomina(string polizanominaid)
        {
            return MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnomina(polizanominaid);
        }

        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnomina()
        {
            return MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnomina();
        }

        public static List<Entity.Contabilidad.Polizasnomina> TraerPolizasnominaPorPolizaID(string polizaid)
        {
            return MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnominaPorPolizaID(polizaid);
        }
        public static Entity.Contabilidad.Polizasnomina TraerPolizasnominaPorUUID(string uuid)
        {
            return MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnominaPorUUID(uuid);
        }

        public static System.Data.DataSet TraerPolizasnominaDS()
        {
            return MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnominaDS();
        }

        #endregion //Métodos Públicos

    }
}
