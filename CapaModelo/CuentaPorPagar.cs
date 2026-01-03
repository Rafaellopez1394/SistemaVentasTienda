using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class CuentaPorPagar
    {
        public Guid CuentaPorPagarID { get; set; }
        public int CompraID { get; set; }
        public int ProveedorID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Estado { get; set; } // PENDIENTE, PARCIAL, PAGADA, VENCIDA
        public int DiasCredito { get; set; }
        public string FolioFactura { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }

        // Propiedades calculadas/extendidas
        public string Proveedor { get; set; }
        public string RFC { get; set; }
        public int DiasParaVencer { get; set; }
        public int DiasVencido { get; set; }
        public decimal TotalPagado { get; set; }
        public int NumeroPagos { get; set; }
    }

    public class PagoProveedor
    {
        public Guid PagoID { get; set; }
        public Guid CuentaPorPagarID { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string FormaPago { get; set; } // EFECTIVO, TRANSFERENCIA, CHEQUE
        public string Referencia { get; set; }
        public string CuentaBancaria { get; set; }
        public string Observaciones { get; set; }
        public Guid? PolizaID { get; set; }
        public int? UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedades extendidas
        public string Proveedor { get; set; }
        public string FolioFactura { get; set; }
    }

    public class ReporteAntiguedadSaldos
    {
        public int ProveedorID { get; set; }
        public string Proveedor { get; set; }
        public string RFC { get; set; }
        public decimal TotalAdeudo { get; set; }
        public decimal Corriente { get; set; } // 0-30 días
        public decimal Dias30 { get; set; } // 31-60 días
        public decimal Dias60 { get; set; } // 61-90 días
        public decimal Dias90 { get; set; } // 91-120 días
        public decimal Mas120 { get; set; } // Más de 120 días
        public int CuentasVencidas { get; set; }
        public List<CuentaPorPagar> Detalle { get; set; }
    }

    public class RegistrarPagoRequest
    {
        public Guid CuentaPorPagarID { get; set; }
        public decimal MontoPagado { get; set; }
        public string FormaPago { get; set; }
        public string Referencia { get; set; }
        public string CuentaBancaria { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioRegistro { get; set; }
    }
}


