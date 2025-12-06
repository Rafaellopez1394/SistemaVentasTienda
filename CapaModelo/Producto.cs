// CapaModelo/Producto.cs
using System;

namespace CapaModelo
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CategoriaID { get; set; }
        public string NombreCategoria { get; set; }
        public string ClaveProdServSATID { get; set; } = string.Empty;
        public string DescripcionSAT { get; set; }
        public string ClaveUnidadSAT { get; set; } = string.Empty;
        public string UnidadSAT { get; set; }
        public string CodigoInterno { get; set; }
        public int TasaIVAID { get; set; } = 3;
        public string TasaIVADescripcion { get; set; }
        public decimal TasaIVAPorcentaje { get; set; } = 16.00m;
        public int? TasaIEPSID { get; set; }
        public string TasaIEPSDescripcion { get; set; }
        public decimal TasaIEPSPorcentaje { get; set; } = 0.00m;
        public bool Exento { get; set; } = false;
        public bool Estatus { get; set; } = true;
        public string Usuario { get; set; } = "system";
        public DateTime FechaAlta { get; set; } = DateTime.Now;
        public DateTime UltimaAct { get; set; } = DateTime.Now;
    }

    public class LoteProducto
    {
        public int LoteID { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public DateTime FechaEntrada { get; set; } = DateTime.Now;
        public DateTime? FechaCaducidad { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Usuario { get; set; } = "system";
        public DateTime UltimaAct { get; set; } = DateTime.Now;
        public Boolean Estatus { get; set; }
    }

}