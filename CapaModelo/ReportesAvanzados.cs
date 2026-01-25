using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Modelo para reporte detallado de utilidad por producto
    /// </summary>
    public class ReporteUtilidadProducto
    {
        public int ProductoID { get; set; }
        public string CodigoInterno { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public string Presentacion { get; set; } // Para tallas, tamaños, etc.
        
        // Compras
        public decimal CantidadComprada { get; set; }
        public decimal CostoTotalCompras { get; set; }
        public decimal CostoPromedioCompra { get; set; }
        
        // Ventas
        public decimal CantidadVendida { get; set; }
        public decimal ImporteTotalVentas { get; set; }
        public decimal PrecioPromedioVenta { get; set; }
        
        // Inventario
        public decimal InventarioActual { get; set; }
        public decimal ValorInventario { get; set; }
        
        // Utilidades
        public decimal CostoVendido { get; set; } // Costo de lo vendido (no de lo comprado)
        public decimal UtilidadBruta { get; set; } // Ventas - Costo vendido
        public decimal MargenUtilidadPorcentaje { get; set; }
        
        // Análisis
        public string Rentabilidad { get; set; } // "ALTA", "MEDIA", "BAJA", "PÉRDIDA"
        public string Recomendacion { get; set; }
    }

    /// <summary>
    /// Modelo para concentrado de recuperación de crédito
    /// </summary>
    public class ReporteRecuperacionCredito
    {
        public DateTime Fecha { get; set; }
        public int TotalClientesCredito { get; set; }
        
        // Créditos otorgados
        public decimal CreditosOtorgados { get; set; }
        public int NumeroVentasCredito { get; set; }
        
        // Cobros realizados
        public decimal CobrosRealizados { get; set; }
        public int NumeroPagos { get; set; }
        
        // Saldos
        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        
        // Métricas
        public decimal PorcentajeRecuperacion { get; set; } // (Cobros / Créditos) * 100
        public decimal EficienciaCobranza { get; set; }
        
        // Cartera
        public decimal CarteraVigente { get; set; }
        public decimal CarteraVencida { get; set; }
        public decimal PorcentajeVencido { get; set; }
    }

    /// <summary>
    /// Detalle de cuenta por cobrar por cliente
    /// </summary>
    public class ReporteCarteraCliente
    {
        public Guid ClienteID { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string TipoCredito { get; set; }
        public int DiasCredito { get; set; }
        
        // Resumen de cuenta
        public decimal TotalVentas { get; set; }
        public decimal TotalPagos { get; set; }
        public decimal SaldoPendiente { get; set; }
        
        // Antigüedad
        public decimal Vigente { get; set; } // 0-30 días
        public decimal Vencido30 { get; set; } // 31-60 días
        public decimal Vencido60 { get; set; } // 61-90 días
        public decimal Vencido90 { get; set; } // 90+ días
        public int DiasVencido { get; set; }
        
        // Estado
        public string Estado { get; set; } // "AL CORRIENTE", "VENCIDO", "MOROSO"
        public DateTime? UltimoPago { get; set; }
        public DateTime? ProximoVencimiento { get; set; }
    }

    /// <summary>
    /// Estado de Resultados (P&L)
    /// </summary>
    public class EstadoResultadosVentas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Periodo { get; set; } // "Enero 2026", "Q1 2026", etc.
        
        // INGRESOS
        public decimal VentasTotales { get; set; }
        public decimal Devoluciones { get; set; }
        public decimal IngresosNetos { get; set; }
        
        // COSTO DE VENTAS
        public decimal InventarioInicial { get; set; }
        public decimal Compras { get; set; }
        public decimal InventarioFinal { get; set; }
        public decimal CostoVentas { get; set; }
        
        // UTILIDAD BRUTA
        public decimal UtilidadBruta { get; set; }
        public decimal MargenBrutoPorcentaje { get; set; }
        
        // GASTOS OPERATIVOS
        public decimal GastosNomina { get; set; }
        public decimal GastosOperativos { get; set; } // Renta, servicios, etc.
        public decimal GastosTotales { get; set; }
        
        // UTILIDAD OPERATIVA
        public decimal UtilidadOperativa { get; set; }
        public decimal MargenOperativoPorcentaje { get; set; }
        
        // OTROS INGRESOS/GASTOS
        public decimal IngresosFinancieros { get; set; }
        public decimal GastosFinancieros { get; set; }
        public decimal Impuestos { get; set; }
        
        // UTILIDAD NETA
        public decimal UtilidadNeta { get; set; }
        public decimal MargenNetoPorcentaje { get; set; }
        
        // ANÁLISIS
        public string Conclusion { get; set; }
        public string Recomendaciones { get; set; }
    }

    /// <summary>
    /// Reporte de valuación de inventario
    /// </summary>
    public class ReporteInventarioValuado
    {
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public string Categoria { get; set; }
        
        public int TotalProductos { get; set; }
        public decimal TotalUnidades { get; set; }
        
        // Valuación a costo
        public decimal ValorCosto { get; set; }
        public decimal CostoPromedioPonderado { get; set; }
        
        // Valuación a precio de venta
        public decimal ValorVenta { get; set; }
        public decimal UtilidadPotencial { get; set; }
        public decimal MargenPotencialPorcentaje { get; set; }
        
        // Análisis
        public string ProductoMayorValor { get; set; }
        public decimal ValorProductoMayorValor { get; set; }
        public string CategoríaMayorInventario { get; set; }
    }

    /// <summary>
    /// Análisis de rotación de inventario
    /// </summary>
    public class ReporteRotacionInventario
    {
        public int ProductoID { get; set; }
        public string CodigoInterno { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        
        // Inventario
        public decimal InventarioPromedio { get; set; }
        public decimal InventarioActual { get; set; }
        public decimal ValorInventario { get; set; }
        
        // Ventas
        public decimal CantidadVendida { get; set; }
        public decimal CostoVendido { get; set; }
        
        // Rotación
        public decimal IndiceRotacion { get; set; } // Costo Ventas / Inventario Promedio
        public decimal DiasInventario { get; set; } // 365 / Rotación
        
        // Clasificación
        public string Clasificacion { get; set; } // "ALTA ROTACIÓN", "ROTACIÓN NORMAL", "BAJA ROTACIÓN", "STOCK MUERTO"
        public string Accion { get; set; } // "Mantener stock", "Reducir inventario", "Promocionar", etc.
        
        // Reorden
        public decimal PuntoReorden { get; set; }
        public decimal CantidadSugerida { get; set; }
        public bool RequiereReorden { get; set; }
    }

    /// <summary>
    /// Reporte de ventas por categoría
    /// </summary>
    public class ReporteVentasCategoria
    {
        public string Categoria { get; set; }
        public int TotalProductos { get; set; }
        
        // Ventas
        public decimal CantidadVendida { get; set; }
        public int NumeroVentas { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal TicketPromedio { get; set; }
        
        // Costos y utilidad
        public decimal CostoTotal { get; set; }
        public decimal UtilidadBruta { get; set; }
        public decimal MargenPorcentaje { get; set; }
        
        // Participación
        public decimal PorcentajeVentasTotales { get; set; }
        public decimal PorcentajeUtilidadTotal { get; set; }
        
        // Análisis
        public string ProductoMasVendido { get; set; }
        public decimal CantidadProductoMasVendido { get; set; }
    }

    /// <summary>
    /// Dashboard - KPIs principales
    /// </summary>
    public class DashboardKPIs
    {
        public DateTime Fecha { get; set; }
        
        // Ventas del día
        public decimal VentasHoy { get; set; }
        public decimal VentasAyer { get; set; }
        public decimal DiferenciaVentas { get; set; }
        public decimal PorcentajeCambio { get; set; }
        
        // Mes actual
        public decimal VentasMes { get; set; }
        public decimal UtilidadMes { get; set; }
        public decimal MargenMes { get; set; }
        
        // Top productos del día
        public List<TopProducto> TopProductosDia { get; set; }
        
        // Alertas
        public int ProductosBajoStock { get; set; }
        public int ClientesMorosos { get; set; }
        public decimal CarteraVencida { get; set; }
        public int FacturasPendientes { get; set; }
        
        // Inventario
        public decimal ValorInventario { get; set; }
        public int ProductosSinStock { get; set; }
        
        // Caja
        public decimal EfectivoEnCaja { get; set; }
        public decimal CorteCajaPendiente { get; set; }
        
        public DashboardKPIs()
        {
            TopProductosDia = new List<TopProducto>();
        }
    }

    public class TopProducto
    {
        public string NombreProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Importe { get; set; }
        public decimal Utilidad { get; set; }
    }

    /// <summary>
    /// Reporte fiscal de IVA
    /// </summary>
    public class ReporteIVAVentas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        
        // IVA CAUSADO (Ventas)
        public decimal VentasBase16 { get; set; }
        public decimal IVACausado16 { get; set; }
        public decimal VentasBase8 { get; set; }
        public decimal IVACausado8 { get; set; }
        public decimal VentasBase0 { get; set; }
        public decimal VentasExentas { get; set; }
        public decimal TotalIVACausado { get; set; }
        
        // IVA ACREDITABLE (Compras)
        public decimal ComprasBase16 { get; set; }
        public decimal IVAAcreditable16 { get; set; }
        public decimal ComprasBase8 { get; set; }
        public decimal IVAAcreditable8 { get; set; }
        public decimal ComprasBase0 { get; set; }
        public decimal ComprasExentas { get; set; }
        public decimal TotalIVAAcreditable { get; set; }
        
        // RESULTADO
        public decimal SaldoIVA { get; set; } // Positivo = a favor, Negativo = a pagar
        public string Estado { get; set; } // "A FAVOR", "A PAGAR", "NEUTRO"
        
        // FACTURACIÓN
        public int FacturasEmitidas { get; set; }
        public int FacturasCanceladas { get; set; }
        public int FacturasProveedores { get; set; }
    }
}
