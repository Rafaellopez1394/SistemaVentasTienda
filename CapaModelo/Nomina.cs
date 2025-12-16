using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class Nomina
    {
        public int NominaID { get; set; }
        public string Folio { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaPago { get; set; }
        public string TipoNomina { get; set; }
        
        // Totales
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public int NumeroEmpleados { get; set; }
        
        // Control de proceso
        public string Estatus { get; set; }
        public DateTime FechaCalculo { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public DateTime? FechaPagado { get; set; }
        public DateTime? FechaContabilizada { get; set; }
        
        // Relación con póliza contable
        public Guid? PolizaID { get; set; }
        
        // Control
        public string Usuario { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime UltimaAct { get; set; }
        
        // Lista de recibos
        public List<NominaDetalle> Recibos { get; set; } = new List<NominaDetalle>();
    }
    
    public class NominaDetalle
    {
        public int NominaDetalleID { get; set; }
        public int NominaID { get; set; }
        public int EmpleadoID { get; set; }
        
        // Datos del empleado
        public string NumeroEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string RFC { get; set; }
        public string NSS { get; set; }
        public string CURP { get; set; }
        public string Puesto { get; set; }
        
        // Datos de contratación (para CFDI Nómina 1.2)
        public DateTime? FechaIngreso { get; set; }
        public string TipoContrato { get; set; } // 01=Indeterminado, 02=Determinado, etc.
        public bool Sindicalizado { get; set; } = false;
        public string TipoJornada { get; set; } // 01=Diurna, 02=Nocturna, 03=Mixta, etc.
        public string TipoRegimen { get; set; } // 02=Sueldos, 03=Jubilados, etc.
        public string Departamento { get; set; }
        public string RiesgoPuesto { get; set; } // Clase de riesgo 1-5
        public string PeriodicidadPago { get; set; } // 04=Semanal, 05=Quincenal, etc.
        public int? Banco { get; set; } // Código del banco (3 dígitos)
        public string CuentaBancaria { get; set; }
        public decimal? SalarioIntegrado { get; set; }
        public string ClaveEntidadFederativa { get; set; } // CHH, CDMX, etc.
        
        // Días y periodo
        public decimal DiasTrabajados { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        
        // Salarios
        public decimal SalarioDiario { get; set; }
        public decimal SalarioBase { get; set; }
        
        // Totales del recibo
        public decimal TotalPercepciones { get; set; }
        public decimal TotalPercepcionesGravadas { get; set; }
        public decimal TotalPercepcionesExentas { get; set; }
        
        public decimal TotalDeducciones { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        public decimal TotalOtrasDeducciones { get; set; }
        
        public decimal NetoPagar { get; set; }
        
        // Timbrado CFDI
        public string UUID { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string SelloCFD { get; set; }
        public string SelloSAT { get; set; }
        public string CadenaOriginal { get; set; }
        public string EstatusTimbre { get; set; } // SIN_TIMBRAR, TIMBRADO, ERROR, CANCELADO
        
        // Control
        public string Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime UltimaAct { get; set; }
        
        // Listas de percepciones y deducciones
        public List<NominaPercepcion> Percepciones { get; set; } = new List<NominaPercepcion>();
        public List<NominaDeduccion> Deducciones { get; set; } = new List<NominaDeduccion>();
    }
    
    public class NominaPercepcion
    {
        public int NominaPercepcionID { get; set; }
        public int NominaDetalleID { get; set; }
        public int PercepcionID { get; set; }
        
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public string TipoPercepcion { get; set; }
        
        public decimal ImporteGravado { get; set; }
        public decimal ImporteExento { get; set; }
        public decimal ImporteTotal { get; set; }
        
        public DateTime FechaAlta { get; set; }
    }
    
    public class NominaDeduccion
    {
        public int NominaDeduccionID { get; set; }
        public int NominaDetalleID { get; set; }
        public int DeduccionID { get; set; }
        
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public string TipoDeduccion { get; set; }
        
        public decimal Importe { get; set; }
        
        public DateTime FechaAlta { get; set; }
    }
}
