// CapaModelo/VentaPOS.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    // Venta completa para POS
    public class VentaPOS
    {
        public Guid VentaID { get; set; }
        public Guid ClienteID { get; set; }
        public DateTime FechaVenta { get; set; }
        public string TipoVenta { get; set; } // CONTADO, CREDITO
        public int? FormaPagoID { get; set; }
        public int? MetodoPagoID { get; set; }
        public decimal? EfectivoRecibido { get; set; }
        public decimal? Cambio { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public bool RequiereFactura { get; set; }
        public int CajaID { get; set; }
        public string Estatus { get; set; }
        public string Usuario { get; set; }
        
        // Datos del cliente
        public string ClienteRFC { get; set; }
        public string ClienteRazonSocial { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        
        // Detalle de venta
        public List<VentaDetallePOS> Detalle { get; set; }
    }

    // Detalle de cada producto vendido
    public class VentaDetallePOS
    {
        public int VentaDetalleID { get; set; }
        public Guid VentaID { get; set; }
        public int ProductoID { get; set; }
        public int LoteID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal TasaIVA { get; set; }
        public decimal MontoIVA { get; set; }
        public decimal Subtotal => Cantidad * PrecioVenta;
        public decimal Total => Subtotal + MontoIVA;
        
        // Datos del producto
        public string Nombre { get; set; }
        public string CodigoInterno { get; set; }
    }

    // Producto para búsqueda en POS
    public class ProductoPOS
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string CodigoInterno { get; set; }
        public decimal PrecioVenta { get; set; }
        public int TasaIVAID { get; set; }
        public decimal TasaIVA { get; set; }
        public string DescripcionIVA { get; set; }
        public int StockDisponible { get; set; }
        public bool Estatus { get; set; }
        public string Categoria { get; set; }
        
        // Lote activo con stock
        public int LoteID { get; set; }
        public decimal PrecioCompra { get; set; }
    }

    // Forma de pago
    public class FormaPago
    {
        public int FormaPagoID { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereCambio { get; set; }
        public bool Estatus { get; set; }
    }

    // Método de pago
    public class MetodoPago
    {
        public int MetodoPagoID { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public bool Estatus { get; set; }
    }

    // Caja
    public class Caja
    {
        public int CajaID { get; set; }
        public string Nombre { get; set; }
        public int? SucursalID { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
    }

    // Movimiento de caja
    public class MovimientoCaja
    {
        public int MovimientoCajaID { get; set; }
        public int CajaID { get; set; }
        public string TipoMovimiento { get; set; } // APERTURA, VENTA, RETIRO, CIERRE
        public DateTime FechaMovimiento { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoActual { get; set; }
        public string Concepto { get; set; }
        public Guid? VentaID { get; set; }
        public string Usuario { get; set; }
    }

    // Payload para registrar venta desde el POS
    public class RegistrarVentaPayload
    {
        public VentaPOS Venta { get; set; }
        public List<VentaDetallePOS> Detalle { get; set; }
    }

    // Respuesta de validación de crédito
    public class ValidacionCredito
    {
        public bool TieneCredito { get; set; }
        public decimal CreditoDisponible { get; set; }
        public string Mensaje { get; set; }
    }
}
