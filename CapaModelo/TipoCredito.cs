using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Representa un tipo de crédito disponible en el sistema.
    /// Los 3 tipos son: Por Dinero, Por Producto, Por Tiempo
    /// </summary>
    public class TipoCredito
    {
        public int TipoCreditoID { get; set; }
        public string Codigo { get; set; }           // CR001, CR002, CR003
        public string Nombre { get; set; }           // "Crédito por Dinero", etc
        public string Descripcion { get; set; }
        public string Criterio { get; set; }         // "Dinero" | "Producto" | "Tiempo"
        public string Icono { get; set; }            // Para UI: fa-dollar-sign, fa-box, fa-calendar
        public bool Activo { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? UltimaAct { get; set; }
    }

    /// <summary>
    /// Información de un crédito asignado a un cliente específico
    /// </summary>
    public class CreditoClienteInfo
    {
        public int ClienteTipoCreditoID { get; set; }
        public Guid ClienteID { get; set; }
        public string RazonSocial { get; set; }
        
        // Información del tipo de crédito
        public int TipoCreditoID { get; set; }
        public string TipoCredito { get; set; }
        public string Criterio { get; set; }
        
        // Límites según el tipo
        public decimal? LimiteDinero { get; set; }           // Para "Dinero"
        public int? LimiteProducto { get; set; }             // Para "Producto"
        public int? PlazoDias { get; set; }                  // Para "Tiempo"
        
        // Información de vencimiento
        public DateTime? FechaVencimiento { get; set; }      // Para "Tiempo"
        public bool VencidoAutomatico { get; set; } // Vence cada cierto tiempo
        
        // Estado actual
        public DateTime FechaAsignacion { get; set; }
        public bool Activo { get; set; }
        public bool Suspendido { get; set; }
        
        // Cálculos en tiempo real
        public decimal SaldoUtilizado { get; set; }
        public decimal SaldoDisponible { get; set; }
        public int DiasRestantes { get; set; }
        public int UnidadesUtilizadas { get; set; }
        public int UnidadesDisponibles { get; set; }
        
        // Información de límites
        public decimal? PorcentajeUtilizado { get; set; }
        public bool ExcedeLimit { get; set; }
        public int DiasVencidos { get; set; }
        
        public string Usuario { get; set; }
        public DateTime UltimaAct { get; set; }
    }

    /// <summary>
    /// Resumen de crédito de un cliente
    /// </summary>
    public class ResumenCreditoCliente
    {
        public Guid ClienteID { get; set; }
        public string RazonSocial { get; set; }
        public bool TieneCreditoActivo { get; set; }
        
        // Límites totales
        public decimal LimiteDineroTotal { get; set; }
        public int LimiteProductoTotal { get; set; }
        public int PlazoDiasMaximo { get; set; }
        
        // Saldos totales
        public decimal SaldoDineroUtilizado { get; set; }
        public int SaldoProductoUtilizado { get; set; }
        public decimal SaldoDineroDisponible { get; set; }
        public int SaldoProductoDisponible { get; set; }
        
        // Estado crítico
        public decimal SaldoVencido { get; set; }
        public int DiasMaximoVencidos { get; set; }
        public List<CreditoClienteInfo> TiposAsignados { get; set; }
        
        // Indicadores
        public bool EnAlarma { get; set; }  // Si está en límite o vencido
        public string Estado { get; set; }  // "NORMAL", "ALERTA", "CRÍTICO", "VENCIDO"
    }
}


