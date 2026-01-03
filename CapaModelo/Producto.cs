// CapaModelo/Producto.cs
using System;

namespace CapaModelo
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CategoriaID { get; set; }
        public string NombreCategoria { get; set; }
        public string ClaveProdServSATID { get; set; }
        public string DescripcionSAT { get; set; }
        public string ClaveUnidadSAT { get; set; }
        public string UnidadSAT { get; set; }
        public string CodigoInterno { get; set; }
        public int TasaIVAID { get; set; }
        public string TasaIVADescripcion { get; set; }
        public decimal TasaIVAPorcentaje { get; set; }
        public int? TasaIEPSID { get; set; }
        public string TasaIEPSDescripcion { get; set; }
        public decimal TasaIEPSPorcentaje { get; set; }
        public bool Exento { get; set; }
        public bool Estatus { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime UltimaAct { get; set; }
        public bool VentaPorGramaje { get; set; }
        public decimal? PrecioPorKilo { get; set; }
        public string UnidadMedidaBase { get; set; }
    }

    public class LoteProducto
    {
        public int LoteID { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaCaducidad { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Usuario { get; set; }
        public DateTime UltimaAct { get; set; }
        public Boolean Estatus { get; set; }
    }

}

