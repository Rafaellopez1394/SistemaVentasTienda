using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catfacturasproveedordet
    /// </summary>
    internal class CatfacturasproveedordetBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatfacturasproveedordetBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatfacturasproveedordet(List<Entity.Contabilidad.Catfacturasproveedordet> listaCatfacturasproveedordet)
        {
            MobileDAL.Contabilidad.Catfacturasproveedordet.Guardar(ref listaCatfacturasproveedordet);
        }

        public static Entity.Contabilidad.Catfacturasproveedordet TraerCatfacturasproveedordet(string facturaproveedordetid)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedordet.TraerCatfacturasproveedordet(facturaproveedordetid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedordet> TraerCatfacturasproveedordet()
        {
            return MobileDAL.Contabilidad.Catfacturasproveedordet.TraerCatfacturasproveedordet();
        }

        public static System.Data.DataSet TraerCatfacturasproveedordetDS()
        {
            return MobileDAL.Contabilidad.Catfacturasproveedordet.TraerCatfacturasproveedordetDS();
        }

        public static void Eliminar(string FacturaProveedorID)
        {
            MobileDAL.Contabilidad.Catfacturasproveedordet.Eliminar(FacturaProveedorID);
        }

        #endregion //Métodos Públicos

    }
}
