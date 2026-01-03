using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvtip
    /// </summary>
    internal class AcvtipBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvtipBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvtip(ListaDeEntidades<Entity.Contabilidad.Acvtip> listaAcvtip)
        {
            MobileDAL.Contabilidad.Acvtip.Guardar(ref listaAcvtip);
        }

        public Entity.Contabilidad.Acvtip TraerAcvtip(string acvtipid)
        {
            return MobileDAL.Contabilidad.Acvtip.TraerAcvtip(acvtipid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvtip> TraerAcvtip()
        {
            return MobileDAL.Contabilidad.Acvtip.TraerAcvtip();
        }

        public System.Data.DataSet TraerAcvtipDS()
        {
            return MobileDAL.Contabilidad.Acvtip.TraerAcvtipDS();
        }

        #endregion //Métodos Públicos

    }
}
