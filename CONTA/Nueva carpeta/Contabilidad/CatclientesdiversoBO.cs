using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catclientesdiverso
    /// </summary>
    internal class CatclientesdiversoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatclientesdiversoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatclientesdiverso(List<Entity.Contabilidad.Catclientesdiverso> listaCatclientesdiverso)
        {
            MobileDAL.Contabilidad.Catclientesdiverso.Guardar(ref listaCatclientesdiverso);
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversos(string clientediversoid, string EmpresaID, int? Codigo)
        {
            return MobileDAL.Contabilidad.Catclientesdiverso.TraerCatclientesdiversos(clientediversoid, EmpresaID, Codigo);
        }

        public static List<Entity.Contabilidad.Catclientesdiverso> TraerCatclientesdiversos(string EmpresaID, string Descripcion)
        {
            return MobileDAL.Contabilidad.Catclientesdiverso.TraerCatclientesdiversos(EmpresaID, Descripcion);
        }

        public static System.Data.DataSet TraerCatclientesdiversosDS()
        {
            return MobileDAL.Contabilidad.Catclientesdiverso.TraerCatclientesdiversosDS();
        }
        public static System.Data.DataSet TraerDireccionCompletaCteDiv(string ClienteDiversoID)
        {
            return MobileDAL.Contabilidad.Catclientesdiverso.TraerDireccionCompletaCteDiv(ClienteDiversoID);
        }

        public static Entity.Contabilidad.Catclientesdiverso TraerCatclientesdiversosPorRFC(string RFC, string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catclientesdiverso.TraerCatclientesdiversosPorRFC(RFC, EmpresaID);
        }

        #endregion //Métodos Públicos

    }
}
