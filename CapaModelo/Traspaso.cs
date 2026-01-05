using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Modelo para traspasos entre sucursales
    /// </summary>
    public class Traspaso
    {
        public int TraspasoID { get; set; }
        public string FolioTraspaso { get; set; }
        public DateTime FechaTraspaso { get; set; }
        public int SucursalOrigenID { get; set; }
        public int SucursalDestinoID { get; set; }
        public string UsuarioRegistro { get; set; }
        public string Observaciones { get; set; }
        public string Estatus { get; set; } // PENDIENTE, EN_TRANSITO, RECIBIDO, CANCELADO
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string UsuarioEnvia { get; set; }
        public string UsuarioRecibe { get; set; }
        public string MotivoCancelacion { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedades de navegación
        public Sucursal SucursalOrigen { get; set; }
        public Sucursal SucursalDestino { get; set; }
        public List<DetalleTraspaso> Detalles { get; set; }

        // Propiedades calculadas
        public int TotalProductos { get; set; }
        public decimal TotalCantidad { get; set; }
        public decimal ValorTotal { get; set; }

        public Traspaso()
        {
            Detalles = new List<DetalleTraspaso>();
            Estatus = "PENDIENTE";
            FechaTraspaso = DateTime.Now;
            FechaRegistro = DateTime.Now;
        }
    }

    /// <summary>
    /// Detalle de productos en un traspaso
    /// </summary>
    public class DetalleTraspaso
    {
        public int DetalleTraspasoID { get; set; }
        public int TraspasoID { get; set; }
        public int ProductoID { get; set; }
        public int? LoteOrigenID { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal? CantidadEnviada { get; set; }
        public decimal? CantidadRecibida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Observaciones { get; set; }

        // Propiedades de navegación
        public Producto Producto { get; set; }

        // Propiedades calculadas
        public decimal Subtotal 
        { 
            get { return CantidadSolicitada * PrecioUnitario; }
        }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string UnidadMedida { get; set; }
    }

    /// <summary>
    /// Inventario disponible por sucursal
    /// </summary>
    public class InventarioSucursal
    {
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string UnidadMedida { get; set; }
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public int TotalLotes { get; set; }
        public decimal CantidadDisponible { get; set; }
        public decimal CantidadTotal { get; set; }
        public decimal PrecioPromedioCompra { get; set; }
        public decimal PrecioPromedioVenta { get; set; }
    }
}
