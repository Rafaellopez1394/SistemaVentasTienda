using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CapaModelo.PAC
{
    /// <summary>
    /// Request completo para timbrar CFDI de Nómina con FiscalAPI
    /// Basado en documentación oficial: https://documenter.getpostman.com/view/4346593/2sB2j4eqXr
    /// </summary>
    public class FiscalAPINominaRequest
    {
        [JsonProperty("versionCode")]
        public string VersionCode { get; set; } = "4.0";

        [JsonProperty("series")]
        public string Series { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; } // Formato: "2025-04-17T08:56:40"

        [JsonProperty("paymentMethodCode")]
        public string PaymentMethodCode { get; set; } = "PUE"; // Pago en una sola exhibición

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; } = "MXN";

        [JsonProperty("typeCode")]
        public string TypeCode { get; set; } = "N"; // N = Nómina

        [JsonProperty("expeditionZipCode")]
        public string ExpeditionZipCode { get; set; }

        [JsonProperty("exportCode")]
        public string ExportCode { get; set; } = "01"; // No aplica exportación

        [JsonProperty("issuer")]
        public FiscalAPIIssuerNomina Issuer { get; set; }

        [JsonProperty("recipient")]
        public FiscalAPIRecipientNomina Recipient { get; set; }

        [JsonProperty("complement")]
        public FiscalAPINominaComplement Complement { get; set; }
    }

    /// <summary>
    /// Emisor (Patrón) con datos específicos de nómina
    /// </summary>
    public class FiscalAPIIssuerNomina
    {
        [JsonProperty("tin")]
        public string Tin { get; set; } // RFC del patrón

        [JsonProperty("legalName")]
        public string LegalName { get; set; } // Razón social

        [JsonProperty("taxRegimeCode")]
        public string TaxRegimeCode { get; set; } // Ej: "601"

        [JsonProperty("employerData")]
        public FiscalAPIEmployerData EmployerData { get; set; }

        [JsonProperty("taxCredentials")]
        public List<FiscalAPITaxCredential> TaxCredentials { get; set; }
    }

    /// <summary>
    /// Datos patronales específicos de nómina
    /// </summary>
    public class FiscalAPIEmployerData
    {
        [JsonProperty("employerRegistration")]
        public string EmployerRegistration { get; set; } // Registro Patronal IMSS

        [JsonProperty("originEmployerTin")]
        public string OriginEmployerTin { get; set; } // Solo para asimilados
    }

    /// <summary>
    /// Credenciales fiscales (certificado .CER y llave .KEY en base64)
    /// </summary>
    public class FiscalAPITaxCredential
    {
        [JsonProperty("base64File")]
        public string Base64File { get; set; } // Certificado o llave en base64

        [JsonProperty("fileType")]
        public int FileType { get; set; } // 0 = .CER, 1 = .KEY

        [JsonProperty("password")]
        public string Password { get; set; } // Contraseña de la llave privada
    }

    /// <summary>
    /// Receptor (Empleado) con datos específicos de nómina
    /// </summary>
    public class FiscalAPIRecipientNomina
    {
        [JsonProperty("tin")]
        public string Tin { get; set; } // RFC del empleado

        [JsonProperty("legalName")]
        public string LegalName { get; set; } // Nombre completo

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; } // CP fiscal

        [JsonProperty("taxRegimeCode")]
        public string TaxRegimeCode { get; set; } // "605" para asalariados

        [JsonProperty("cfdiUseCode")]
        public string CfdiUseCode { get; set; } = "CN01"; // CN01 = Nómina

        [JsonProperty("employeeData")]
        public FiscalAPIEmployeeData EmployeeData { get; set; }
    }

    /// <summary>
    /// Datos del empleado requeridos por el SAT
    /// </summary>
    public class FiscalAPIEmployeeData
    {
        [JsonProperty("curp")]
        public string Curp { get; set; } // CURP (18 caracteres)

        [JsonProperty("socialSecurityNumber")]
        public string SocialSecurityNumber { get; set; } // NSS (11 dígitos)

        [JsonProperty("laborRelationStartDate")]
        public string LaborRelationStartDate { get; set; } // Fecha ingreso: "2024-08-18"

        [JsonProperty("seniority")]
        public string Seniority { get; set; } // Antigüedad ISO 8601: "P54W" (54 semanas)

        [JsonProperty("satContractTypeId")]
        public string SatContractTypeId { get; set; } // c_TipoContrato: "01"=Indeterminado, "09"=Asimilado

        [JsonProperty("satUnionizedStatusId")]
        public string SatUnionizedStatusId { get; set; } // "Sí" o "No"

        [JsonProperty("satTaxRegimeTypeId")]
        public string SatTaxRegimeTypeId { get; set; } // c_TipoRegimen: "02"=Sueldos, "09"=Asimilados

        [JsonProperty("employeeNumber")]
        public string EmployeeNumber { get; set; } // Número de empleado

        [JsonProperty("department")]
        public string Department { get; set; } // Departamento

        [JsonProperty("position")]
        public string Position { get; set; } // Puesto

        [JsonProperty("satJobRiskId")]
        public string SatJobRiskId { get; set; } // c_RiesgoPuesto: "1" a "5"

        [JsonProperty("satPaymentPeriodicityId")]
        public string SatPaymentPeriodicityId { get; set; } // c_PeriodicidadPago: "04"=Quincenal, "05"=Mensual

        [JsonProperty("satBankId")]
        public string SatBankId { get; set; } // c_Banco: "012"=Banamex

        [JsonProperty("bankAccount")]
        public string BankAccount { get; set; } // Cuenta bancaria (opcional)

        [JsonProperty("baseSalaryForContributions")]
        public decimal? BaseSalaryForContributions { get; set; } // Salario base cotización

        [JsonProperty("integratedDailySalary")]
        public decimal? IntegratedDailySalary { get; set; } // Salario diario integrado

        [JsonProperty("satPayrollStateId")]
        public string SatPayrollStateId { get; set; } // c_EstadosPayroll: "JAL", "CMX", etc.
    }

    /// <summary>
    /// Complemento de nómina (estructura principal)
    /// </summary>
    public class FiscalAPINominaComplement
    {
        [JsonProperty("payroll")]
        public FiscalAPIPayroll Payroll { get; set; }
    }

    /// <summary>
    /// Nómina 1.2 - Datos del recibo de nómina
    /// </summary>
    public class FiscalAPIPayroll
    {
        [JsonProperty("version")]
        public string Version { get; set; } = "1.2"; // Versión del complemento

        [JsonProperty("payrollTypeCode")]
        public string PayrollTypeCode { get; set; } // "O"=Ordinaria, "E"=Extraordinaria

        [JsonProperty("paymentDate")]
        public string PaymentDate { get; set; } // Fecha de pago: "2025-08-30T00:00:00"

        [JsonProperty("initialPaymentDate")]
        public string InitialPaymentDate { get; set; } // Inicio período

        [JsonProperty("finalPaymentDate")]
        public string FinalPaymentDate { get; set; } // Fin período

        [JsonProperty("daysPaid")]
        public decimal DaysPaid { get; set; } // Días pagados

        [JsonProperty("earnings")]
        public FiscalAPIEarningsContainer Earnings { get; set; }

        [JsonProperty("deductions")]
        public List<FiscalAPIDeduction> Deductions { get; set; }
    }

    /// <summary>
    /// Contenedor de percepciones (incluye percepciones y otros pagos)
    /// </summary>
    public class FiscalAPIEarningsContainer
    {
        [JsonProperty("earnings")]
        public List<FiscalAPIEarning> Earnings { get; set; }

        [JsonProperty("otherPayments")]
        public List<FiscalAPIOtherPayment> OtherPayments { get; set; }
    }

    /// <summary>
    /// Percepción individual
    /// </summary>
    public class FiscalAPIEarning
    {
        [JsonProperty("earningTypeCode")]
        public string EarningTypeCode { get; set; } // c_TipoPercepcion: "001"=Sueldos, "005"=Fondo ahorro

        [JsonProperty("code")]
        public string Code { get; set; } // Código interno

        [JsonProperty("concept")]
        public string Concept { get; set; } // Descripción

        [JsonProperty("taxedAmount")]
        public decimal TaxedAmount { get; set; } // Monto gravado

        [JsonProperty("exemptAmount")]
        public decimal ExemptAmount { get; set; } // Monto exento
    }

    /// <summary>
    /// Otros pagos (subsidio al empleo, compensación saldos)
    /// </summary>
    public class FiscalAPIOtherPayment
    {
        [JsonProperty("otherPaymentTypeCode")]
        public string OtherPaymentTypeCode { get; set; } // "002"=Subsidio empleo

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("concept")]
        public string Concept { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("subsidyCaused")]
        public decimal? SubsidyCaused { get; set; }

        [JsonProperty("balanceCompensation")]
        public FiscalAPIBalanceCompensation BalanceCompensation { get; set; }
    }

    /// <summary>
    /// Compensación de saldos a favor (para subsidio al empleo)
    /// </summary>
    public class FiscalAPIBalanceCompensation
    {
        [JsonProperty("favorableBalance")]
        public decimal FavorableBalance { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("remainingFavorableBalance")]
        public decimal RemainingFavorableBalance { get; set; }
    }

    /// <summary>
    /// Deducción individual
    /// </summary>
    public class FiscalAPIDeduction
    {
        [JsonProperty("deductionTypeCode")]
        public string DeductionTypeCode { get; set; } // c_TipoDeduccion: "001"=IMSS, "002"=ISR

        [JsonProperty("code")]
        public string Code { get; set; } // Código interno

        [JsonProperty("concept")]
        public string Concept { get; set; } // Descripción

        [JsonProperty("amount")]
        public decimal Amount { get; set; } // Monto
    }
}
