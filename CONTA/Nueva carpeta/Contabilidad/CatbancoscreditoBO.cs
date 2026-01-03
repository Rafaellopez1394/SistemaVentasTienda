using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catbancoscredito
    /// </summary>
    internal class CatbancoscreditoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatbancoscreditoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatbancoscredito(List<Entity.Contabilidad.Catbancoscredito> listaCatbancoscredito)
        {
            MobileDAL.Contabilidad.Catbancoscredito.Guardar(ref listaCatbancoscredito);
        }

        public static Entity.Contabilidad.Catbancoscredito TraerCatbancoscreditos(int bancocreditoid)
        {
            return MobileDAL.Contabilidad.Catbancoscredito.TraerCatbancoscreditos(bancocreditoid);
        }

        public static List<Entity.Contabilidad.Catbancoscredito> TraerCatbancoscreditos()
        {
            return MobileDAL.Contabilidad.Catbancoscredito.TraerCatbancoscreditos();
        }

        public static System.Data.DataSet TraerCatbancoscreditosDS()
        {
            return MobileDAL.Contabilidad.Catbancoscredito.TraerCatbancoscreditosDS();
        }

        #endregion //Métodos Públicos

    }
}
