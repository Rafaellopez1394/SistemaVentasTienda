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
        public List<CuentaContable> Subcuentas { get; set; } = new List<CuentaContable>();
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
        public List<AlertaContador> Alertas { get; set; } = new List<AlertaContador>();
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
    /// Certificado Digital (CSD o FIEL)
    /// </summary>
    public class CertificadoDigital
    {
        public int CertificadoID { get; set; }
        public string TipoCertificado { get; set; } // CSD, FIEL
        public string NombreCertificado { get; set; }
        
        // Datos del Certificado
        public string NoCertificado { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        
        // Archivos
        public byte[] ArchivoCER { get; set; }
        public byte[] ArchivoKEY { get; set; }
        public string PasswordKEY { get; set; }
        
        // Nombres originales
        public string NombreArchivoCER { get; set; }
        public string NombreArchivoKEY { get; set; }
        
        // Estado
        public bool Activo { get; set; }
        public bool EsPredeterminado { get; set; }
        
        // Uso
        public bool UsarParaFacturas { get; set; }
        public bool UsarParaNomina { get; set; }
        public bool UsarParaCancelaciones { get; set; }
        
        // Auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        
        // Propiedades calculadas
        public bool EstaVigente
        {
            get
            {
                if (!FechaVencimiento.HasValue) return false;
                return FechaVencimiento.Value > DateTime.Now;
            }
        }
        
        public int DiasParaVencer
        {
            get
            {
                if (!FechaVencimiento.HasValue) return 0;
                return (FechaVencimiento.Value - DateTime.Now).Days;
            }
        }
    }

    /// <summary>
    /// Request para subir certificado
    /// </summary>
    public class SubirCertificadoRequest
    {
        public string TipoCertificado { get; set; } // CSD, FIEL
        public string NombreCertificado { get; set; }
        public string PasswordKEY { get; set; }
        public bool UsarParaFacturas { get; set; }
        public bool UsarParaNomina { get; set; }
        public bool UsarParaCancelaciones { get; set; }
        public bool EsPredeterminado { get; set; }
    }

    /// <summary>
    /// Información extraída de un certificado
    /// </summary>
    public class InfoCertificado
    {
        public string NoCertificado { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool EsValido { get; set; }
        public string MensajeError { get; set; }
    }
}
