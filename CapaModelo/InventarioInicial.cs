using System;

namespace CapaModelo
{
    public class InventarioInicial
    {
        public int CargaID { get; set; }
        public DateTime FechaCarga { get; set; }
        public string UsuarioCarga { get; set; }
        public int TotalProductos { get; set; }
        public decimal TotalUnidades { get; set; }
        public decimal ValorTotal { get; set; }
        public string Comentarios { get; set; }
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public bool Activo { get; set; }
        public string Estado { get; set; }
    }

    public class InventarioInicialDetalle
    {
        public int DetalleID { get; set; }
        public int CargaID { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoInterno { get; set; }
        public decimal CantidadCargada { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime? FechaCaducidad { get; set; }
        public string Comentarios { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class ProductoInventarioInicial
    {
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoInterno { get; set; }
        public decimal StockActual { get; set; }
        public decimal CantidadNueva { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}
