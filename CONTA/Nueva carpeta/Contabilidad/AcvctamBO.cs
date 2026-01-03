using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvctam
    /// </summary>
    internal class AcvctamBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvctamBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvctam(ListaDeEntidades<Entity.Contabilidad.Acvctam> listaAcvctam)
        {
            MobileDAL.Contabilidad.Acvctam.Guardar(ref listaAcvctam);
        }

        public Entity.Contabilidad.Acvctam TraerAcvctam(string acvctamid)
        {
            return MobileDAL.Contabilidad.Acvctam.TraerAcvctam(acvctamid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvctam> TraerAcvctam()
        {
            return MobileDAL.Contabilidad.Acvctam.TraerAcvctam();
        }

        public System.Data.DataSet TraerAcvctamDS()
        {
            return MobileDAL.Contabilidad.Acvctam.TraerAcvctamDS();
        }

        public Entity.Contabilidad.Acvctam TraerAcvctamPorCuenta(string cuenta,string empresaid)
        {
            return MobileDAL.Contabilidad.Acvctam.TraerAcvctamPorCuenta(cuenta,empresaid);
        }

        #endregion //Métodos Públicos

    }
}
