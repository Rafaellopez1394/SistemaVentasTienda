using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class ProductoSucursal
    {
        public int ProductoSucursalID { get; set; }
        public Producto oProducto { get; set; }
        public Sucursal oSucursal { get; set; }
        public int Stock { get; set; }
        public decimal PrecioUnidadCompra { get; set; }
        public decimal PrecioUnidadVenta { get; set; }
        public bool Iniciado { get; set; }
    }
}
