using System;

namespace CapaModelo
{
    public class Empleado
    {
        public int EmpleadoID { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NombreCompleto 
        { 
            get { return (Nombre + " " + ApellidoPaterno + " " + ApellidoMaterno).Trim(); }
        }
        
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaBaja { get; set; }
        
        // Información laboral
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public string TipoContrato { get; set; } // Catálogo SAT c_TipoContrato: "01"=Indeterminado
        public string TipoRegimen { get; set; } // Catálogo SAT c_TipoRegimen: "02"=Sueldos
        public string TipoJornada { get; set; }
        public string PeriodicidadPago { get; set; }
        
        // Salario
        public decimal SalarioDiario { get; set; }
        public decimal SalarioMensual { get; set; }
        public decimal? SalarioDiarioIntegrado { get; set; }
        
        // Contacto
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Domicilio { get; set; }
        public string CodigoPostal { get; set; }
        
        // Datos bancarios
        public string Banco { get; set; }
        public string CodigoBanco { get; set; } // Catálogo SAT c_Banco: "012"=Banamex
        public string CuentaBancaria { get; set; }
        public string CLABE { get; set; }
        
        // Control
        public string Estatus { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime UltimaAct { get; set; }
    }
}


