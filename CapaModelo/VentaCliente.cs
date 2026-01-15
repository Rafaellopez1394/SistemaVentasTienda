// CapaModelo/VentaCliente.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class VentaCliente
    {
        public VentaCliente()
        {
            Detalle = new List<VentaDetalleCliente>();
        }

        public Guid VentaID { get; set; }
        public string NumeroVenta { get; set; }
        public Guid ClienteID { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Usuario { get; set; }
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public List<VentaDetalleCliente> Detalle { get; set; }
    }

    public class VentaDetalleCliente
    {
        public int ProductoID { get; set; }
        public string CodigoInterno { get; set; }
        public string Producto { get; set; }
        public int LoteID { get; set; }
        public string NumeroLote { get; set; }
        public decimal Cantidad { get; set; } // Cambiar a decimal para soportar ventas por peso
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal TasaIVAPorcentaje { get; set; }
        public bool Exento { get; set; }
        public decimal Utilidad { get { return Cantidad * (PrecioVenta - PrecioCompra); } }
    }

    public class PagoCliente
    {
        public Guid PagoID { get; set; }
        public Guid VentaID { get; set; }
        public Guid ClienteID { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string FormaPago { get; set; }
        public string Referencia { get; set; }
        public string Comentario { get; set; }
        public bool GenerarFactura { get; set; }
        public bool GenerarComplemento { get; set; }
        public string Usuario { get; set; }
        
        // Mantener propiedades antiguas por compatibilidad
        [Obsolete("Use Monto instead")]
        public decimal Importe
        {
            get { return Monto; }
            set { Monto = value; }
        }
    }
}

