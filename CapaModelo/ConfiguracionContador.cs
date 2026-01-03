using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Configuración Contable del Sistema
    /// </summary>
    public class ConfiguracionContable
    {
        public int ConfigContableID { get; set; }
        
        // Ejercicio Fiscal
        public int EjercicioFiscalActual { get; set; }
        public int MesActual { get; set; }
        
        // Cuentas por Defecto
        public string CuentaBancos { get; set; }
        public string CuentaClientes { get; set; }
        public string CuentaProveedores { get; set; }
        public string CuentaIVATraslado { get; set; }
        public string CuentaIVARetenido { get; set; }
        public string CuentaISRRetenido { get; set; }
        public string CuentaVentas { get; set; }
        public string CuentaCompras { get; set; }
        public string CuentaCostoVentas { get; set; }
        public string CuentaNomina { get; set; }
        public string CuentaIMSS { get; set; }
        
        // Configuración General
        public bool UsaPolizasAutomaticas { get; set; }
        public bool RequiereAutorizacionCancelacion { get; set; }
        public int DiasVencimientoFacturas { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }

    /// <summary>
    /// Catálogo de Cuentas Contables
    /// </summary>
    public class CuentaContable
    {
        public int CuentaID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; } // 1=Mayor, 2=Subcuenta, 3=Detalle
        public string CuentaPadre { get; set; }
        public string Tipo { get; set; } // ACTIVO, PASIVO, CAPITAL, INGRESO, EGRESO
        public string Naturaleza { get; set; } // D=Deudora, A=Acreedora
        public bool AceptaMovimientos { get; set; }
        public bool Activo { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSAT { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        
        // Navegación
        public List<CuentaContable> Subcuentas { get; set; }
    }

    /// <summary>
    /// Configuración de Nómina
    /// </summary>
    public class ConfiguracionNomina
    {
        public int ConfigNominaID { get; set; }
        
        // Periodicidad
        public string TipoPeriodo { get; set; } // SEMANAL, QUINCENAL, MENSUAL
        public int DiasDePago { get; set; }
        
        // Impuestos
        public decimal SalarioMinimo { get; set; }
        public decimal UMA { get; set; }
        public decimal TopeSalarioIMSS { get; set; }
        
        // Porcentajes IMSS Empresa
        public decimal PorcentajeIMSSEmpresa { get; set; }
        public decimal PorcentajeRCV { get; set; }
        public decimal PorcentajeGuarderia { get; set; }
        public decimal PorcentajeRetiro { get; set; }
        
        // Porcentajes IMSS Trabajador
        public decimal PorcentajeIMSSTrabajador { get; set; }
        
        // Configuración CFDI Nómina
        public string LugarExpedicionNomina { get; set; }
        public string RutaCertificadoNomina { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }

    /// <summary>
    /// Percepción de Nómina (Catálogo SAT)
    /// </summary>
    public class PercepcionNomina
    {
        public int PercepcionID { get; set; }
        public string ClaveSAT { get; set; }
        public string Descripcion { get; set; }
        public string TipoPercepcion { get; set; } // ORDINARIA, EXTRAORDINARIA
        public bool GravaISR { get; set; }
        public bool GravaIMSS { get; set; }
        public bool Activo { get; set; }
    }

    /// <summary>
    /// Deducción de Nómina (Catálogo SAT)
    /// </summary>
    public class DeduccionNomina
    {
        public int DeduccionID { get; set; }
        public string ClaveSAT { get; set; }
        public string Descripcion { get; set; }
        public string TipoDeduccion { get; set; } // OBLIGATORIA, VOLUNTARIA
        public bool Activo { get; set; }
    }

    /// <summary>
    /// Dashboard del Contador
    /// </summary>
    public class DashboardContador
    {
        // Facturas
        public int FacturasMes { get; set; }
        public decimal TotalFacturadoMes { get; set; }
        public int FacturasCanceladasMes { get; set; }
        
        // Nómina
        public int RecibosMes { get; set; }
        public decimal TotalNominaMes { get; set; }
        
        // Cuentas por Pagar
        public int CuentasPendientes { get; set; }
        public decimal TotalPorPagar { get; set; }
        
        // Pólizas
        public int PolizasMes { get; set; }
        
        // Alertas
        public List<AlertaContador> Alertas { get; set; }
    }

    /// <summary>
    /// Alerta para el Contador
    /// </summary>
    public class AlertaContador
    {
        public string Tipo { get; set; } // URGENTE, IMPORTANTE, INFO
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Icono { get; set; }
        public string Color { get; set; }
        public DateTime Fecha { get; set; }
    }

    /// <summary>
    /// Request para actualizar configuración de empresa
    /// </summary>
    public class ActualizarEmpresaRequest
    {
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string RegimenFiscal { get; set; }
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string SitioWeb { get; set; }
    }

    /// <summary>
    /// Request para actualizar configuración PAC
    /// </summary>
    public class ActualizarPACRequest
    {
        public string ProveedorPAC { get; set; }
        public string UrlTimbrado { get; set; }
        public string UrlCancelacion { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string RutaCertificado { get; set; }
        public string RutaLlavePrivada { get; set; }
        public string PasswordCertificado { get; set; }
        public bool Activo { get; set; }
    }

    /// <summary>
    /// Configuración de cuentas contables para nómina
    /// </summary>
    public class ConfiguracionCuentasNomina
    {
        public string CuentaSueldosYSalarios { get; set; }
        public string CuentaPremioPuntualidad { get; set; }
        public string CuentaPremioAsistencia { get; set; }
        public string CuentaVacaciones { get; set; }
        public string CuentaPrimaVacacional { get; set; }
        public string CuentaAguinaldo { get; set; }
        public string CuentaPTU { get; set; }
        public string CuentaISRRetenido { get; set; }
        public string CuentaIMSSObrero { get; set; }
        public string CuentaInfonavit { get; set; }
        public string CuentaInfonavitCreditos { get; set; }
        public string CuentaFonacot { get; set; }
        public string CuentaBancosNomina { get; set; }
        public string CuentaIMSSPatronal { get; set; }
        public string CuentaSARPatronal { get; set; }
        public string CuentaInfonavitPatronal { get; set; }
    }

    /// <summary>
    /// Resumen contable de nómina
    /// </summary>
    public class ResumenNominaContable
    {
        public int NominaID { get; set; }
        public DateTime FechaPago { get; set; }
        public int TotalEmpleados { get; set; }
        public decimal Sueldos { get; set; }
        public decimal SueldosYSalarios { get { return Sueldos; } set { Sueldos = value; } }
        public decimal PremioPuntualidad { get; set; }
        public decimal PremioAsistencia { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PrimaVacacional { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal PTU { get; set; }
        public decimal ISRRetenido { get; set; }
        public decimal IMSSObrero { get; set; }
        public decimal Infonavit { get; set; }
        public decimal InfonavitCreditos { get; set; }
        public decimal Fonacot { get; set; }
        public decimal IMSSPatronal { get; set; }
        public decimal SARPatronal { get; set; }
        public decimal InfonavitPatronal { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public decimal TotalCuotasPatronales { get; set; }
        public decimal NetoAPagar { get; set; }
    }
}


