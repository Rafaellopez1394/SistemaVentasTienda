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
        public string TipoVenta { get; set; } // CONTADO, CREDITO, PARCIAL
        public int? FormaPagoID { get; set; }
        public int? MetodoPagoID { get; set; }
        public decimal? EfectivoRecibido { get; set; }
        public decimal? Cambio { get; set; }
        public decimal? MontoPagado { get; set; } // Monto pagado inicial (para pagos parciales)
        public decimal SaldoPendiente { get; set; } // Saldo por pagar
        public bool EsPagado { get; set; } // Indica si está totalmente pagado
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
        public decimal Cantidad { get; set; } // ✅ Cambiado a decimal para soportar kg/gramos
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal TasaIVA { get; set; }
        public decimal MontoIVA { get; set; }
        public decimal Subtotal { get { return Cantidad * PrecioVenta; } }
        public decimal Total { get { return Subtotal + MontoIVA; } }
        
        // Datos del producto
        public string Nombre { get; set; }
        public string CodigoInterno { get; set; }

        // Venta por gramaje (opcional)
        public decimal Gramos { get; set; }           // gramos vendidos
        public decimal Kilogramos { get; set; }        // kilogramos vendidos
        public decimal PrecioPorKilo { get; set; }     // precio por kilo usado en la venta
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
        
        // Venta por gramaje
        public bool VentaPorGramaje { get; set; }
        public decimal? PrecioPorKilo { get; set; }
        public string UnidadMedidaBase { get; set; }
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

    // Estado de caja (para validaciones)
    public class EstadoCaja
    {
        public int CajaID { get; set; }
        public bool EstaAbierta { get; set; }
        public DateTime? FechaApertura { get; set; }
        public decimal SaldoActual { get; set; }
        public int NumeroVentas { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal FondoInicial { get; set; }
        public decimal TotalGastos { get; set; }
    }

    // Corte de caja (cierre con arqueo)
    public class CorteCaja
    {
        public int CorteID { get; set; }
        public int CajaID { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal FondoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalRetiros { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal MontoRealEfectivo { get; set; }
        public decimal MontoRealTarjeta { get; set; }
        public decimal MontoRealTransferencia { get; set; }
        public decimal MontoRealTotal { get; set; }
        public decimal Diferencia { get; set; }
        public string TipoDiferencia { get; set; } // FALTANTE, SOBRANTE, CUADRADO
        public string Observaciones { get; set; }
        public string UsuarioCierre { get; set; }
        public Guid? PolizaID { get; set; }
    }

    // ===== NUEVAS CLASES PARA PAGOS PARCIALES =====
    
    // Pago parcial de una venta
    public class VentaPago
    {
        public int PagoID { get; set; }
        public Guid VentaID { get; set; }
        public int FormaPagoID { get; set; }
        public string FormaPagoDescripcion { get; set; }
        public int MetodoPagoID { get; set; }
        public string MetodoPagoDescripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public string Usuario { get; set; }
        public int? ComplementoPagoID { get; set; } // Vincula con complemento de pago timbrado
        public DateTime FechaAlta { get; set; }
    }

    // Venta pendiente de pago
    public class VentaPendientePago
    {
        public Guid VentaID { get; set; }
        public Guid ClienteID { get; set; }
        public string Cliente { get; set; }
        public string RFC { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool EsPagado { get; set; }
        public bool RequiereFactura { get; set; }
        public int? IdFactura { get; set; }
        public Guid? FacturaUUID { get; set; }
        public string FacturaSerieFolio { get; set; }
        public int NumeroPagos { get; set; } // Número de pagos realizados
        public string ClienteRazonSocial { get; set; }
        public string ClienteRFC { get; set; }
    }

    // Request para registrar un nuevo pago de venta
    public class RegistrarPagoVentaRequest
    {
        public Guid VentaID { get; set; }
        public int FormaPagoID { get; set; }
        public int MetodoPagoID { get; set; }
        public decimal Monto { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public bool GenerarComplementoPago { get; set; } // Si debe generar y timbrar complemento de pago
    }
}



