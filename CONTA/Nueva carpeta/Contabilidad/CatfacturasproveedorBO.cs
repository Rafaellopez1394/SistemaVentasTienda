using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catfacturasproveedor
    /// </summary>
    internal class CatfacturasproveedorBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatfacturasproveedorBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatfacturasproveedor(List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor)
        {
            MobileDAL.Contabilidad.Catfacturasproveedor.Guardar(ref listaCatfacturasproveedor);
        }

        public static Entity.Contabilidad.Catfacturasproveedor TraerCatfacturasproveedor(string facturaproveedorid,string uuid)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasproveedor(facturaproveedorid, uuid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor()
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasproveedor();
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedorPorCompraID(string CompraID)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasproveedorPorCompraID(CompraID);
        }

        public static System.Data.DataSet TraerCatfacturasproveedorDS()
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasproveedorDS();
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasPorProveedorEmpresa(string proveedorid, string empresaid)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasPorProveedorEmpresa(proveedorid, empresaid);
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor(string facturaproveedorid, string uuid, string proveedorid, string empresaid, string emisorrfc, string receptorrfc, string usocfdi, DateTime? fechainicial, DateTime? fechafinal)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.TraerCatfacturasproveedor(facturaproveedorid, uuid, proveedorid, empresaid, emisorrfc, receptorrfc, usocfdi, fechainicial, fechafinal);
        }
        public static void Catfacturasproveedor_Delete(string CompraID)
        {
            MobileDAL.Contabilidad.Catfacturasproveedor.Catfacturasproveedor_Delete(CompraID);
        }
        public static int ValidarExistenciaFacturaProveedor(string uuid)
        {
            return MobileDAL.Contabilidad.Catfacturasproveedor.ValidarExistenciaFacturaProveedor(uuid);
        }
        #endregion //Métodos Públicos

    }
}
