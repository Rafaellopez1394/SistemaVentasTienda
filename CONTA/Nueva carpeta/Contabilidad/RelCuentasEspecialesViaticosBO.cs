using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Relcuentasespecialesviatico
    /// </summary>
    internal class RelcuentasespecialesviaticoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public RelcuentasespecialesviaticoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarRelcuentasespecialesviatico(List<Entity.Contabilidad.Relcuentasespecialesviatico> listaRelcuentasespecialesviatico)
        {
            MobileDAL.Contabilidad.Relcuentasespecialesviatico.Guardar(ref listaRelcuentasespecialesviatico);
        }

        public static Entity.Contabilidad.Relcuentasespecialesviatico TraerRelcuentasespecialesviaticos(string relcuentaespecialviaticoid, string cuentaviaticos)
        {
            return MobileDAL.Contabilidad.Relcuentasespecialesviatico.TraerRelcuentasespecialesviaticos(relcuentaespecialviaticoid, cuentaviaticos);
        }

        public static List<Entity.Contabilidad.Relcuentasespecialesviatico> TraerRelcuentasespecialesviaticos()
        {
            return MobileDAL.Contabilidad.Relcuentasespecialesviatico.TraerRelcuentasespecialesviaticos();
        }

        public static System.Data.DataSet TraerRelcuentasespecialesviaticosDS()
        {
            return MobileDAL.Contabilidad.Relcuentasespecialesviatico.TraerRelcuentasespecialesviaticosDS();
        }

        #endregion //Métodos Públicos

    }
}
