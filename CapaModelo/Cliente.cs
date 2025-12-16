// CapaModelo/Cliente.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Cliente
    {
        public Guid ClienteID { get; set; }
        public int IdCliente { get; set; } // ID entero para compatibilidad con BD
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string RegimenFiscalID { get; set; }
        public string CodigoPostal { get; set; }
        public string UsoCFDIID { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        
        // Nuevos campos para mejor gestión
        public string Direccion { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; } = "México";
        public bool CreditoActivo { get; set; }
        public decimal LimiteCreditoActual { get; set; }    // Calculado desde tipos de crédito
        public decimal SaldoCreditoActual { get; set; }     // Calculado desde deudas
        public int DiasVencidos { get; set; }               // Calculado
        
        public bool Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Usuario { get; set; }
        public DateTime UltimaAct { get; set; }

        // Solo para mostrar en listas (NO se guarda en BD)
        public string RegimenFiscal { get; set; }       // ← Solo para mostrar
        public string UsoCFDI { get; set; }             // ← Solo para mostrar
        public List<ClienteTipoCredito> TiposCredito { get; set; } = new List<ClienteTipoCredito>();
    }

}