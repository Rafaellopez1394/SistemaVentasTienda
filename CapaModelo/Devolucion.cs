// CapaModelo/Devolucion.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Devolucion
    {
        public int DevolucionID { get; set; }
        public Guid VentaID { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string TipoDevolucion { get; set; } // TOTAL, PARCIAL
        public string MotivoDevolucion { get; set; }
        public decimal TotalDevuelto { get; set; }
        public string FormaReintegro { get; set; } // EFECTIVO, CREDITO_CLIENTE, CAMBIO_PRODUCTO
        public decimal MontoReintegrado { get; set; }
        public decimal CreditoGenerado { get; set; }
        public int? UsuarioID { get; set; }
        public string UsuarioRegistro { get; set; }
        public int SucursalID { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
        
        // Propiedades adicionales para visualización
        public string NumeroVenta { get; set; }
        public string NombreSucursal { get; set; }
        public string NombreCliente { get; set; }
        public string RFCCliente { get; set; }
        public int TotalProductos { get; set; }
        
        // Lista de productos devueltos
        public List<DevolucionDetalle> Productos { get; set; }
        
        public Devolucion()
        {
            Productos = new List<DevolucionDetalle>();
            FechaDevolucion = DateTime.Now;
            Estatus = true;
        }
    }
    
    public class DevolucionDetalle
    {
        public int DevolucionDetalleID { get; set; }
        public int DevolucionID { get; set; }
        public int ProductoID { get; set; }
        public int? LoteID { get; set; }
        public decimal CantidadDevuelta { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal SubTotal { get; set; }
        public bool ReingresadoInventario { get; set; }
        public DateTime? FechaReingreso { get; set; }
        
        // Propiedades adicionales para visualización
        public string CodigoInterno { get; set; }
        public string NombreProducto { get; set; }
        public string NumeroLote { get; set; }
    }
    
    public class RegistrarDevolucionPayload
    {
        public Guid VentaID { get; set; }
        public string TipoDevolucion { get; set; }
        public string MotivoDevolucion { get; set; }
        public string FormaReintegro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int SucursalID { get; set; }
        public List<ProductoDevuelto> Productos { get; set; }
        
        public RegistrarDevolucionPayload()
        {
            Productos = new List<ProductoDevuelto>();
        }
    }
    
    public class ProductoDevuelto
    {
        public int ProductoID { get; set; }
        public int LoteID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
    }
    
    public class ReporteDevolucion
    {
        public int TotalDevoluciones { get; set; }
        public int DevolucionesTotales { get; set; }
        public int DevolucionesParciales { get; set; }
        public decimal MontoTotalDevuelto { get; set; }
        public decimal TotalEfectivoReintegrado { get; set; }
        public decimal TotalCreditoGenerado { get; set; }
        public decimal DevolucionesEfectivo { get; set; }
        public decimal DevolucionesCredito { get; set; }
        public decimal DevolucionesCambio { get; set; }
    }
    
    public class ProductoMasDevuelto
    {
        public int ProductoID { get; set; }
        public string CodigoInterno { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public decimal TotalDevuelto { get; set; }
        public int NumeroDevoluciones { get; set; }
        public decimal MontoTotalDevuelto { get; set; }
        public decimal TotalVendido { get; set; }
        public decimal PorcentajeDevolucion { get; set; }
    }
}
