using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catclientesfilial
    /// </summary>
    internal class CatclientesfilialBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatclientesfilialBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatclientesfilial(List<Entity.Contabilidad.Catclientesfilial> listaCatclientesfilial)
        {
            MobileDAL.Contabilidad.Catclientesfilial.Guardar(ref listaCatclientesfilial);
        }

        public static Entity.Contabilidad.Catclientesfilial TraerCatclientesfilial(string clienteid,string empresaid, int? Codigo)
        {
            return MobileDAL.Contabilidad.Catclientesfilial.TraerCatclientesfilial(clienteid,empresaid, Codigo);
        }

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilial()
        {
            return MobileDAL.Contabilidad.Catclientesfilial.TraerCatclientesfilial();
        }

        public static List<Entity.Contabilidad.Catclientesfilial> TraerCatclientesfilialPorNombre(string nombre, string empresaid)
        {
            return MobileDAL.Contabilidad.Catclientesfilial.TraerCatclientesfilialPorNombre(nombre, empresaid);
        }

        public static System.Data.DataSet TraerCatclientesfilialDS()
        {
            return MobileDAL.Contabilidad.Catclientesfilial.TraerCatclientesfilialDS();
        }

        #endregion //Métodos Públicos

    }
}
