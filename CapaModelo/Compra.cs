// CapaModelo/Compra.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Compra
    {
        public int CompraID { get; set; }
        public Guid ProveedorID { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string FolioFactura { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Usuario { get; set; }
        public List<CompraDetalle> Detalle { get; set; } = new List<CompraDetalle>();
    }

    public class CompraDetalle
    {
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal TasaIVAPorcentaje { get; set; } = 16m;
        public bool Exento { get; set; } = false;
    }
}

