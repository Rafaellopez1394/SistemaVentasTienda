// CapaModelo/VentaCliente.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class VentaCliente
    {
        public Guid VentaID { get; set; }
        public Guid ClienteID { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Usuario { get; set; }
        public List<VentaDetalleCliente> Detalle { get; set; } = new List<VentaDetalleCliente>();
    }

    public class VentaDetalleCliente
    {
        public int ProductoID { get; set; }
        public string Producto { get; set; }
        public int LoteID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal TasaIVAPorcentaje { get; set; } = 16m;
        public bool Exento { get; set; } = false;
        public decimal Utilidad => Cantidad * (PrecioVenta - PrecioCompra);
    }

    public class PagoCliente
    {
        public int PagoID { get; set; }
        public Guid VentaID { get; set; }
        public decimal Importe { get; set; }
        public DateTime FechaPago { get; set; }
        public string Usuario { get; set; }
    }
}