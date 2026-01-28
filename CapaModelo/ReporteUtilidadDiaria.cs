// CapaModelo/ReporteUtilidadDiaria.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Modelo para el Reporte de Utilidad Diaria
    /// </summary>
    public class ReporteUtilidadDiaria
    {
        public ReporteUtilidadDiaria()
        {
            ResumenVentas = new List<ResumenVentas>();
            CostosCotidiano = new List<CostoDetalle>();
            Utilidad = new List<UtilityDetalle>();
            Recuperacion = new List<RecuperacionDetalle>();
            InventarioInicial = new List<InventarioDetalle>();
            Entradas = new List<EntradaDetalle>();
            DetalleVentas = new List<DetalleVentaProducto>();
        }

        public DateTime Fecha { get; set; }
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }

        public List<ResumenVentas> ResumenVentas { get; set; }
        public List<CostoDetalle> CostosCotidiano { get; set; }
        public List<UtilityDetalle> Utilidad { get; set; }
        public List<RecuperacionDetalle> Recuperacion { get; set; }
        public List<InventarioDetalle> InventarioInicial { get; set; }
        public List<EntradaDetalle> Entradas { get; set; }
        public List<DetalleVentaProducto> DetalleVentas { get; set; }

        // Resúmenes totales
        public decimal TotalVentasContado { get; set; }
        public decimal TotalVentasCredito { get; set; }
        public int TotalTickets { get; set; }
        public int TotalUnidades { get; set; }
        public decimal CostosCompra { get; set; }
        public decimal UtilidadDiaria { get; set; }
        public decimal RecuperoCreditosTotal { get; set; }
        public decimal CostoCreditoTotal { get; set; }
    }

    public class ResumenVentas
    {
        public string FormaPago { get; set; }
        public int Tickets { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalUnidades { get; set; }
        public decimal PromedioPorTicket => Tickets > 0 ? TotalVentas / Tickets : 0;
    }

    public class CostoDetalle
    {
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public int Unidades { get; set; }
    }

    public class UtilityDetalle
    {
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }

    public class RecuperacionDetalle
    {
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public int VentasRecuperadas { get; set; }
    }

    public class InventarioDetalle
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
    }

    public class EntradaDetalle
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
    }

    public class DetalleVentaProducto
    {
        public string Producto { get; set; }
        public int VentasContado { get; set; }
        public int VentasCredito { get; set; }
        public int TotalVentas { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal Utilidad => 0; // Se calcula desde la venta - costo
    }
}
