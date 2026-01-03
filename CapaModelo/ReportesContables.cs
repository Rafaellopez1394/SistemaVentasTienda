// CapaModelo/ReportesContables.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    // Balanza de Comprobación
    public class BalanzaComprobacion
    {
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string Tipo { get; set; } // ACTIVO, PASIVO, CAPITAL, INGRESO, EGRESO
        public decimal SaldoInicial { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal SaldoFinal { get; set; }
        public string Naturaleza { get; set; } // DEUDORA, ACREEDORA
    }

    // Estado de Resultados
    public class EstadoResultados
    {
        public string Empresa { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        
        // Ingresos
        public decimal VentasNetas { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal TotalIngresos { get; set; }
        
        // Costos
        public decimal CostoVentas { get; set; }
        public decimal UtilidadBruta { get; set; }
        
        // Gastos Operación
        public decimal GastosVenta { get; set; }
        public decimal GastosAdministracion { get; set; }
        public decimal TotalGastosOperacion { get; set; }
        public decimal UtilidadOperacion { get; set; }
        
        // Otros
        public decimal GastosFinancieros { get; set; }
        public decimal ProductosFinancieros { get; set; }
        public decimal UtilidadAntesImpuestos { get; set; }
        
        // Final
        public decimal ISR { get; set; }
        public decimal UtilidadNeta { get; set; }
        
        // Desglose por cuenta
        public List<LineaEstadoResultados> Detalle { get; set; }
    }

    public class LineaEstadoResultados
    {
        public string Seccion { get; set; } // INGRESOS, COSTOS, GASTOS
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Monto { get; set; }
    }

    // Libro Diario
    public class LibroDiario
    {
        public Guid PolizaID { get; set; }
        public string TipoPoliza { get; set; }
        public DateTime FechaPoliza { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
        public List<AsientoContable> Asientos { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
    }

    public class AsientoContable
    {
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
    }

    // Libro Mayor
    public class LibroMayor
    {
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal SaldoInicial { get; set; }
        public List<MovimientoCuenta> Movimientos { get; set; }
        public decimal TotalCargos { get; set; }
        public decimal TotalAbonos { get; set; }
        public decimal SaldoFinal { get; set; }
    }

    public class MovimientoCuenta
    {
        public DateTime Fecha { get; set; }
        public string TipoPoliza { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
        public decimal Cargo { get; set; }
        public decimal Abono { get; set; }
        public decimal Saldo { get; set; }
    }

    // Reporte IVA
    public class ReporteIVA
    {
        public string Periodo { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        
        // IVA Trasladado (cobrado en ventas)
        public decimal IVA16Trasladado { get; set; }
        public decimal IVA8Trasladado { get; set; }
        public decimal IVA0Trasladado { get; set; }
        public decimal TotalIVATrasladado { get; set; }
        
        // IVA Acreditable (pagado en compras)
        public decimal IVA16Acreditable { get; set; }
        public decimal IVA8Acreditable { get; set; }
        public decimal TotalIVAAcreditable { get; set; }
        
        // Resultado
        public decimal SaldoFavor { get; set; } // Si es positivo, a favor del SAT (pagar)
        public decimal SaldoAFavor { get; set; } // Si es negativo, a favor del contribuyente
        
        public List<DetalleIVA> DetalleVentas { get; set; }
        public List<DetalleIVA> DetalleCompras { get; set; }
    }

    public class DetalleIVA
    {
        public DateTime Fecha { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string Documento { get; set; }
        public decimal Base { get; set; }
        public decimal Tasa { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }

    // Auxiliar por Cuenta
    public class AuxiliarCuenta
    {
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal SaldoInicial { get; set; }
        public List<MovimientoCuenta> Movimientos { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public decimal SaldoFinal { get; set; }
    }
}


