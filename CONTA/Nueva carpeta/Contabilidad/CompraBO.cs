using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Compra
    /// </summary>
    internal class CompraBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CompraBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCompra(List<Entity.Contabilidad.Compra> listaCompra)
        {
            MobileDAL.Contabilidad.Compra.Guardar(ref listaCompra);
        }

        public static Entity.Contabilidad.Compra TraerCompras(string compraid)
        {
            return MobileDAL.Contabilidad.Compra.TraerCompras(compraid);
        }

        public static List<Entity.Contabilidad.Compra> TraerCompras()
        {
            return MobileDAL.Contabilidad.Compra.TraerCompras();
        }

        public static System.Data.DataSet TraerComprasDS()
        {
            return MobileDAL.Contabilidad.Compra.TraerComprasDS();
        }
        public static Entity.Contabilidad.Compra TraerComprasPorCodigo(string empresaid, int codigo)
        {
            return MobileDAL.Contabilidad.Compra.TraerComprasPorCodigo(empresaid, codigo);
        }
        public static List<Entity.Contabilidad.Compra> TraerComprasPorProveedor(string empresaid, string nombreproveedor) {
            return MobileDAL.Contabilidad.Compra.TraerComprasPorProveedor(empresaid, nombreproveedor);
        }

        #endregion //Métodos Públicos

    }
}
