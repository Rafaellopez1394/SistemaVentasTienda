using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catproveedor
    /// </summary>
    internal class CatproveedorBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatproveedorBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatproveedor(List<Entity.Contabilidad.Catproveedor> listaCatproveedor)
        {
            MobileDAL.Contabilidad.Catproveedor.Guardar(ref listaCatproveedor);
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedores(string proveedorid, string rfc,string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedores(proveedorid, rfc, EmpresaID);
        }

        public static List<Entity.Contabilidad.Catproveedor> TraerCatproveedores()
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedores();
        }

        public static System.Data.DataSet TraerCatproveedoresDS()
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedoresDS();
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCodigo(int Codigo,string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedoresPorCodigo(Codigo, EmpresaID);
        }

        public static List<Entity.Contabilidad.Catproveedor> TraerCatproveedoresPorNombre(string nombre,string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedoresPorNombre(nombre, EmpresaID);
        }

        public static int TraerSiguienteCodicoProveedor(string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerSiguienteCodicoProveedor(EmpresaID);
        }

        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresPorCuentaContable(string EmpresaID, string Cuentacontable)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedoresPorCuentaContable(EmpresaID, Cuentacontable);
        }
        public static void GuardarCatproveedorBLT(List<Entity.Contabilidad.Catproveedor> listaCatproveedor)
        {
            MobileDAL.Contabilidad.Catproveedor.GuardarBalorLandTrading(ref listaCatproveedor);
        }
        public static Entity.Contabilidad.Catproveedor TraerCatproveedoresBLT(string proveedorid, string rfc, string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerCatproveedoresBLT(proveedorid, rfc, EmpresaID);
        }
        public static int TraerSiguienteCodicoProveedorBLT(string EmpresaID)
        {
            return MobileDAL.Contabilidad.Catproveedor.TraerSiguienteCodicoProveedorBLT(EmpresaID);
        }
        #endregion //Métodos Públicos

    }
}
