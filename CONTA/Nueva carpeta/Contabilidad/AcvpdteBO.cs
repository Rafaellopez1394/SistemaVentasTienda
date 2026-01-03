using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvpdte
    /// </summary>
    internal class AcvpdteBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvpdteBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvpdte(ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaAcvpdte)
        {
            MobileDAL.Contabilidad.Acvpdte.Guardar(ref listaAcvpdte);
        }

        public Entity.Contabilidad.Acvpdte TraerAcvpdte(string acvmovid)
        {
            return MobileDAL.Contabilidad.Acvpdte.TraerAcvpdte(acvmovid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvpdte> TraerAcvpdte()
        {
            return MobileDAL.Contabilidad.Acvpdte.TraerAcvpdte();
        }

        public System.Data.DataSet TraerAcvpdteDS()
        {
            return MobileDAL.Contabilidad.Acvpdte.TraerAcvpdteDS();
        }

        public static ListaDeEntidades<Entity.Contabilidad.Acvpdte> TraerAcvpdtePorAcvGralPdte(string acvgralid)
        {
            return MobileDAL.Contabilidad.Acvpdte.TraerAcvpdtePorAcvGralPdte(acvgralid);
        }

        #endregion //Métodos Públicos

    }
}
