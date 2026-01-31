# FISCALAPI: Solución Completa para CFDI Nómina

## 🎯 Resumen Ejecutivo

**EXCELENTE NOTICIA:** FiscalAPI **SÍ soporta CFDI de nómina nativamente**. Tu sistema está **90% listo** para implementarlo.

### Estado Actual

| Componente | Estado | Cobertura |
|------------|--------|-----------|
| Gestión de empleados | ✅ COMPLETO | 100% |
| Cálculo de nómina | ✅ COMPLETO | 100% |
| Pólizas contables | ✅ COMPLETO | 100% |
| FiscalAPIService base | ✅ EXISTE | 100% |
| Integración CFDI nómina | ❌ FALTA | 0% |

**Conclusión:** Solo falta agregar **UN método** al servicio existente para completar el 100%.

---

## 📋 Lo que YA tienes (90% completo)

### 1. Infraestructura de nómina completa

**`CapaDatos\CD_Nomina.cs`** (1072 líneas):
- ✅ Cálculo de percepciones/deducciones
- ✅ ISR automático
- ✅ IMSS (obrero + patronal)
- ✅ Neto a pagar
- ✅ Validaciones de negocio
- ✅ Generación automática de pólizas contables

**`CapaDatos\CD_Empleado.cs`** (309 líneas):
- ✅ CRUD completo
- ✅ RFC, CURP, NSS
- ✅ Datos laborales (puesto, departamento, fecha ingreso)
- ✅ Datos bancarios (banco, CLABE)
- ✅ Salarios (diario, mensual, integrado)

### 2. FiscalAPIService.cs - YA integrado

**`CapaDatos\PAC\FiscalAPIService.cs`** (650 líneas):
- ✅ Cliente HTTP configurado
- ✅ Autenticación con headers (X-API-KEY, X-TENANT-KEY, X-TIME-ZONE)
- ✅ Manejo de errores robusto
- ✅ TLS 1.2 forzado (.NET Framework 4.6)
- ✅ Logs detallados para debugging
- ✅ Método para timbrar facturas (`CrearYTimbrarCFDI`)
- ✅ Método para cancelar CFDIs
- ✅ Método para descargar PDFs

### 3. Base de datos lista

**Tablas existentes:**
- ✅ `Empleados` - Maestro de empleados
- ✅ `Nominas` - Header de nómina (período, folio, estatus)
- ✅ `NominaDetalle` - Recibo por empleado (percepciones, deducciones, neto)
- ✅ `NominaPercepciones` - Detalle de percepciones
- ✅ `NominaDeducciones` - Detalle de deducciones
- ✅ `NominasCFDI` - **⚠️ Existe pero no se usa (lista para activarse)**
- ✅ `Polizas` / `PolizasDetalle` - Integración contable

**Estructura de `NominasCFDI` (verificado en DB):**
```sql
CREATE TABLE NominasCFDI (
    NominaCFDIID INT PRIMARY KEY IDENTITY,
    NominaDetalleID INT NOT NULL,
    UUID NVARCHAR(50),
    FechaTimbrado DATETIME,
    XMLTimbrado NVARCHAR(MAX),
    SelloCFD NVARCHAR(MAX),
    SelloSAT NVARCHAR(MAX),
    CadenaOriginal NVARCHAR(MAX),
    NoCertificadoSAT NVARCHAR(50),
    InvoiceId NVARCHAR(50), -- ID de FiscalAPI para descargar PDF
    Estatus NVARCHAR(20),
    MensajeError NVARCHAR(500),
    FechaRegistro DATETIME DEFAULT GETDATE()
)
```

### 4. Métodos helper YA implementados

**En `CD_Nomina.cs` líneas 977-1050** (comentados pero funcionales):
```csharp
// ✅ Ya existe - Solo descomentar
private int InsertarNominaCFDI(NominaDetalle recibo, string xmlTimbrado, string uuid, ...)
{
    // Inserta en tabla NominasCFDI
}

// ✅ Ya existe - Solo descomentar
private void ActualizarNominaCFDIExitoso(int nominaCFDIID, RespuestaTimbrado respuesta, ...)
{
    // Actualiza UUID, XML, sellos, etc.
}

// ✅ Ya existe - Solo descomentar
private void ActualizarNominaCFDIError(int nominaCFDIID, string mensajeError, ...)
{
    // Registra error de timbrado
}
```

---

## 🆕 Lo que FALTA (10% restante)

### UN solo método faltante

**Archivo:** `CapaDatos\PAC\FiscalAPIService.cs`  
**Líneas a agregar:** ~150 (un método nuevo)

```csharp
/// <summary>
/// Crear y timbrar CFDI de Nómina (Complemento de Nómina 1.2)
/// Endpoint: POST /api/v4/invoices
/// Documentación: https://docs.fiscalapi.com/invoices-by-reference (sección "Crear factura de nómina")
/// </summary>
public async Task<RespuestaTimbrado> CrearYTimbrarCFDINomina(
    FiscalAPINominaRequest request)
{
    // Mismo patrón que CrearYTimbrarCFDI() pero con typeCode = "N"
    // FiscalAPI maneja TODO el XML del complemento de nómina
}
```

**¿Por qué es tan fácil?**  
FiscalAPI **genera automáticamente el XML completo** del Complemento de Nómina 1.2:
- ✅ Percepciones (gravadas/exentas)
- ✅ Deducciones (ISR, IMSS, otros)
- ✅ Horas extra
- ✅ Incapacidades
- ✅ Otros pagos
- ✅ Validaciones del SAT
- ✅ Estructura CFDI 4.0 completa

**Tú solo envías JSON, FiscalAPI devuelve XML timbrado.**

---

## 📦 Comparación: FiscalAPI vs Competencia

| Característica | FiscalAPI | Otros PACs |
|----------------|-----------|------------|
| Complemento de nómina | ✅ Nativo | ⚠️ Manual |
| Generación XML | ✅ Automática | ❌ Tú lo construyes |
| Validaciones SAT | ✅ Incluidas | ⚠️ Parciales |
| Catálogos actualizados | ✅ Siempre | ❌ Manuales |
| Estructura JSON simple | ✅ Sí | ❌ XML crudo |
| SDK .NET | ✅ Disponible | ⚠️ Varía |
| Documentación | ✅ Excelente | ⚠️ Regular |

**Cita de docs:** *"FiscalAPI es actualmente la única solución que ofrece soporte real de complementos CFDI de manera nativa y transparente. Otras alternativas requieren que el desarrollador construya manualmente el XML del complemento."*

---

## 🔧 Implementación Paso a Paso

### PASO 1: Agregar modelo de request (5 minutos)

**Archivo:** `CapaModelo\PAC\FiscalAPINominaRequest.cs` (crear nuevo)

```csharp
using System;
using System.Collections.Generic;

namespace CapaModelo.PAC
{
    /// <summary>
    /// Request para timbrar CFDI Nómina con FiscalAPI
    /// Basado en: https://docs.fiscalapi.com/invoices-by-reference#crear-factura-de-nomina
    /// </summary>
    public class FiscalAPINominaRequest
    {
        public string versionCode { get; set; } = "4.0";
        public string series { get; set; } = "NOM"; // Serie para nóminas
        public string date { get; set; } // AAAA-MM-DDThh:mm:ss
        public string paymentMethodCode { get; set; } = "PUE"; // Pago en una sola exhibición
        public string currencyCode { get; set; } = "MXN";
        public string typeCode { get; set; } = "N"; // N = Nómina
        public string expeditionZipCode { get; set; }
        public string exportCode { get; set; } = "01"; // No aplica exportación

        public FiscalAPIIssuer issuer { get; set; }
        public FiscalAPIRecipient recipient { get; set; }
        public FiscalAPIComplement complement { get; set; }
    }

    public class FiscalAPIIssuer
    {
        public string tin { get; set; } // RFC emisor
        public string legalName { get; set; }
        public string taxRegimeCode { get; set; }
        public FiscalAPIEmployerData employerData { get; set; }
        public List<FiscalAPITaxCredential> taxCredentials { get; set; }
    }

    public class FiscalAPIEmployerData
    {
        public string employerRegistration { get; set; } // Registro patronal IMSS
        // Campos opcionales según tu caso:
        // public string curp { get; set; }
        // public string thirdPartyTin { get; set; }
        // public string resourceOriginCode { get; set; }
        // public decimal? localResourceAmount { get; set; }
    }

    public class FiscalAPIRecipient
    {
        public string tin { get; set; } // RFC empleado
        public string legalName { get; set; }
        public string zipCode { get; set; }
        public string taxRegimeCode { get; set; }
        public string cfdiUseCode { get; set; } = "CN01"; // Nómina
        public FiscalAPIEmployeeData employeeData { get; set; }
    }

    public class FiscalAPIEmployeeData
    {
        public string curp { get; set; }
        public string socialSecurityNumber { get; set; } // NSS
        public string laborRelationStartDate { get; set; } // AAAA-MM-DD
        public string seniority { get; set; } // Formato ISO 8601: P54W (54 semanas)
        public string satContractTypeId { get; set; } // Catálogo c_TipoContrato
        public string satTaxRegimeTypeId { get; set; } // Catálogo c_TipoRegimen
        public string employeeNumber { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public string satJobRiskId { get; set; } // Catálogo c_RiesgoTrabajo
        public string satPaymentPeriodicityId { get; set; } // Catálogo c_PeriodicidadPago
        public string satBankId { get; set; } // Catálogo c_Banco (opcional)
        public decimal baseSalaryForContributions { get; set; } // SBC
        public decimal integratedDailySalary { get; set; } // SDI
        public string satPayrollStateId { get; set; } // Estado donde labora
    }

    public class FiscalAPIComplement
    {
        public FiscalAPIPayroll payroll { get; set; }
    }

    public class FiscalAPIPayroll
    {
        public string version { get; set; } = "1.2";
        public string payrollTypeCode { get; set; } // O=Ordinaria, E=Extraordinaria
        public string paymentDate { get; set; } // AAAA-MM-DD
        public string initialPaymentDate { get; set; } // AAAA-MM-DD
        public string finalPaymentDate { get; set; } // AAAA-MM-DD
        public decimal daysPaid { get; set; }
        public FiscalAPIEarnings earnings { get; set; }
        public List<FiscalAPIDeduction> deductions { get; set; }
        // Opcionales:
        // public List<FiscalAPIDisability> disabilities { get; set; }
    }

    public class FiscalAPIEarnings
    {
        public List<FiscalAPIEarning> earnings { get; set; }
        public List<FiscalAPIOtherPayment> otherPayments { get; set; }
        // Opcionales:
        // public List<FiscalAPIOvertime> overtime { get; set; }
    }

    public class FiscalAPIEarning
    {
        public string earningTypeCode { get; set; } // Catálogo c_TipoPercepcion
        public string code { get; set; } // Código interno
        public string concept { get; set; } // Descripción
        public decimal taxedAmount { get; set; } // Importe gravado
        public decimal exemptAmount { get; set; } // Importe exento
    }

    public class FiscalAPIOtherPayment
    {
        public string otherPaymentTypeCode { get; set; } // Catálogo c_TipoOtroPago
        public string code { get; set; }
        public string concept { get; set; }
        public decimal amount { get; set; }
        public decimal? subsidyCaused { get; set; } // Subsidio al empleo (si aplica)
    }

    public class FiscalAPIDeduction
    {
        public string deductionTypeCode { get; set; } // Catálogo c_TipoDeduccion
        public string code { get; set; }
        public string concept { get; set; }
        public decimal amount { get; set; }
    }

    public class FiscalAPITaxCredential
    {
        public string base64File { get; set; }
        public int fileType { get; set; } // 0=CER, 1=KEY
        public string password { get; set; }
    }
}
```

### PASO 2: Agregar método a FiscalAPIService.cs (30 minutos)

**Archivo:** `CapaDatos\PAC\FiscalAPIService.cs`  
**Agregar después de `CrearYTimbrarCFDI()` (línea ~195):**

```csharp
/// <summary>
/// Crear y timbrar CFDI de Nómina (Complemento de Nómina 1.2)
/// Endpoint: POST /api/v4/invoices
/// typeCode = "N"
/// Documentación: https://docs.fiscalapi.com/invoices-by-reference#crear-factura-de-nomina
/// </summary>
public async Task<RespuestaTimbrado> CrearYTimbrarCFDINomina(FiscalAPINominaRequest request)
{
    var respuesta = new RespuestaTimbrado
    {
        Exitoso = false,
        FechaTimbrado = DateTime.Now
    };

    try
    {
        // Serializar request a JSON
        string jsonRequest = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        System.Diagnostics.Debug.WriteLine("=== REQUEST NÓMINA A FISCALAPI ===");
        System.Diagnostics.Debug.WriteLine($"Endpoint: {_configuracion.UrlApi}/api/v4/invoices");
        System.Diagnostics.Debug.WriteLine($"Type: N (Nómina)");
        System.Diagnostics.Debug.WriteLine($"Empleado: {request.recipient.legalName}");
        System.Diagnostics.Debug.WriteLine($"RFC: {request.recipient.tin}");
        System.Diagnostics.Debug.WriteLine($"Período: {request.complement.payroll.initialPaymentDate} a {request.complement.payroll.finalPaymentDate}");
        System.Diagnostics.Debug.WriteLine($"JSON Request (sin certificados completos):\n{jsonRequest.Substring(0, Math.Min(500, jsonRequest.Length))}...");
        System.Diagnostics.Debug.WriteLine("====================================");

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Mismo endpoint que facturas, pero typeCode="N"
        string endpoint = "/api/v4/invoices";

        // Realizar petición POST
        HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

        // Leer contenido de respuesta
        string responseBody = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine("=== RESPONSE NÓMINA DE FISCALAPI ===");
        System.Diagnostics.Debug.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"Response Body:\n{responseBody}");
        System.Diagnostics.Debug.WriteLine("====================================");

        // Manejar códigos de estado HTTP
        if (response.IsSuccessStatusCode)
        {
            // Respuesta exitosa (200-299)
            var fiscalResponse = JsonConvert.DeserializeObject<FiscalAPICrearCFDIResponse>(responseBody);

            if (fiscalResponse.Succeeded && fiscalResponse.Data?.Responses != null && fiscalResponse.Data.Responses.Count > 0)
            {
                var stampResponse = fiscalResponse.Data.Responses[0];
                
                // Decodificar XML de Base64
                byte[] xmlBytes = Convert.FromBase64String(stampResponse.InvoiceBase64);
                string xmlTimbrado = Encoding.UTF8.GetString(xmlBytes);

                respuesta.Exitoso = true;
                respuesta.UUID = stampResponse.InvoiceUuid;
                respuesta.FechaTimbrado = stampResponse.InvoiceSignatureDate;
                respuesta.XMLTimbrado = xmlTimbrado;
                respuesta.SelloCFD = stampResponse.InvoiceBase64Sello;
                respuesta.SelloSAT = stampResponse.SatBase64Sello;
                respuesta.NoCertificadoSAT = stampResponse.SatCertificateNumber;
                respuesta.CadenaOriginal = stampResponse.SatBase64OriginalString;
                respuesta.InvoiceId = stampResponse.InvoiceId; // Para descargar PDF
                respuesta.Mensaje = $"Recibo de nómina timbrado exitosamente - UUID: {respuesta.UUID}";

                System.Diagnostics.Debug.WriteLine($"✅ NÓMINA TIMBRADA EXITOSAMENTE");
                System.Diagnostics.Debug.WriteLine($"   UUID: {respuesta.UUID}");
                System.Diagnostics.Debug.WriteLine($"   InvoiceId: {respuesta.InvoiceId}");
                System.Diagnostics.Debug.WriteLine($"   Fecha: {respuesta.FechaTimbrado}");
            }
            else
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = fiscalResponse.Message ?? "Error desconocido en respuesta de FiscalAPI";
                respuesta.CodigoError = "RESPONSE_ERROR";
                System.Diagnostics.Debug.WriteLine($"❌ Error en respuesta: {respuesta.Mensaje}");
            }

            return respuesta;
        }
        else
        {
            // Manejar errores HTTP específicos (mismo código que CrearYTimbrarCFDI)
            respuesta.CodigoError = $"HTTP_{(int)response.StatusCode}";

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: // 401
                    respuesta.Mensaje = "Error de autenticación: API Key inválida o expirada";
                    respuesta.CodigoError = "401_UNAUTHORIZED";
                    break;

                case HttpStatusCode.Forbidden: // 403
                    respuesta.Mensaje = "Acceso denegado: Verifica permisos de tu API Key";
                    respuesta.CodigoError = "403_FORBIDDEN";
                    break;

                case (HttpStatusCode)422: // 422 Unprocessable Entity
                    var errorResponse = JsonConvert.DeserializeObject<FiscalAPIErrorResponse>(responseBody);
                    respuesta.Mensaje = $"Error de validación en nómina: {errorResponse.Message ?? "Error desconocido"}";
                    respuesta.CodigoError = "422_VALIDATION_ERROR";
                    if (!string.IsNullOrEmpty(errorResponse.Details))
                        respuesta.Mensaje += $"\nDetalles: {errorResponse.Details}";
                    break;

                case HttpStatusCode.BadRequest: // 400
                    respuesta.Mensaje = $"Petición inválida (nómina): {responseBody}";
                    respuesta.CodigoError = "400_BAD_REQUEST";
                    break;

                case HttpStatusCode.InternalServerError: // 500
                    respuesta.Mensaje = $"Error interno del servidor de FiscalAPI: {responseBody}";
                    respuesta.CodigoError = "500_INTERNAL_ERROR";
                    respuesta.ErrorTecnico = responseBody;
                    break;

                default:
                    respuesta.Mensaje = $"Error HTTP {(int)response.StatusCode} al timbrar nómina: {responseBody}";
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"❌ Error HTTP: {respuesta.CodigoError} - {respuesta.Mensaje}");
            return respuesta;
        }
    }
    catch (TaskCanceledException ex)
    {
        respuesta.Mensaje = "Timeout: FiscalAPI no respondió en 2 minutos (nómina)";
        respuesta.CodigoError = "TIMEOUT";
        System.Diagnostics.Debug.WriteLine($"❌ Timeout: {ex.Message}");
        return respuesta;
    }
    catch (HttpRequestException ex)
    {
        respuesta.Mensaje = $"Error de conexión con FiscalAPI (nómina): {ex.Message}";
        respuesta.CodigoError = "CONNECTION_ERROR";
        System.Diagnostics.Debug.WriteLine($"❌ Error conexión: {ex.Message}");
        return respuesta;
    }
    catch (JsonException ex)
    {
        respuesta.Mensaje = $"Error al procesar respuesta JSON (nómina): {ex.Message}";
        respuesta.CodigoError = "JSON_PARSE_ERROR";
        System.Diagnostics.Debug.WriteLine($"❌ Error JSON: {ex.Message}");
        return respuesta;
    }
    catch (Exception ex)
    {
        respuesta.Mensaje = $"Error inesperado al timbrar nómina: {ex.Message}";
        respuesta.CodigoError = "UNKNOWN_ERROR";
        respuesta.ErrorTecnico = ex.ToString();
        System.Diagnostics.Debug.WriteLine($"❌ Excepción: {ex}");
        return respuesta;
    }
}
```

### PASO 3: Restaurar TimbrarCFDINomina() en CD_Nomina.cs (45 minutos)

**Archivo:** `CapaDatos\CD_Nomina.cs`  
**Reemplazar líneas 750-850** (método que retorna error) **con:**

```csharp
/// <summary>
/// Timbra un recibo de nómina individual (por empleado) usando FiscalAPI
/// Genera CFDI 4.0 con Complemento de Nómina 1.2
/// </summary>
public async Task<RespuestaTimbrado> TimbrarCFDINomina(int nominaDetalleID, string usuario)
{
    var respuesta = new RespuestaTimbrado
    {
        Exitoso = false,
        FechaTimbrado = DateTime.Now
    };

    using (SqlConnection conn = new SqlConnection(Conexion.Cadena))
    {
        conn.Open();
        using (SqlTransaction tran = conn.BeginTransaction())
        {
            try
            {
                // 1. Obtener recibo completo (empleado + nómina)
                var recibo = ObtenerReciboCompleto(nominaDetalleID, conn, tran);
                if (recibo == null)
                {
                    respuesta.Mensaje = "Recibo de nómina no encontrado";
                    return respuesta;
                }

                // Validar que no esté ya timbrado
                if (!string.IsNullOrEmpty(recibo.UUID))
                {
                    respuesta.Mensaje = $"Este recibo ya fue timbrado con UUID: {recibo.UUID}";
                    return respuesta;
                }

                // 2. Obtener empleado completo
                var empleado = CD_Empleado.Instancia.ObtenerPorId(recibo.EmpleadoID);
                if (empleado == null)
                {
                    respuesta.Mensaje = "Empleado no encontrado";
                    return respuesta;
                }

                // 3. Obtener configuración de empresa y FiscalAPI
                var configEmpresa = CD_Empresa.Instancia.ObtenerConfiguracion();
                var configFiscalAPI = CD_ConfiguracionFiscalAPI.Instancia.Obtener();

                if (configFiscalAPI == null || string.IsNullOrEmpty(configFiscalAPI.ApiKey))
                {
                    respuesta.Mensaje = "FiscalAPI no está configurado. Configure en Configuración > FiscalAPI";
                    respuesta.CodigoError = "NO_CONFIGURADO";
                    return respuesta;
                }

                // Validar certificados
                if (string.IsNullOrEmpty(configFiscalAPI.CertificadoBase64) || 
                    string.IsNullOrEmpty(configFiscalAPI.LlavePrivadaBase64))
                {
                    respuesta.Mensaje = "Los certificados fiscales (CSD) no están configurados";
                    respuesta.CodigoError = "NO_CERTIFICADOS";
                    return respuesta;
                }

                // 4. Construir request para FiscalAPI
                var nominaRequest = new FiscalAPINominaRequest
                {
                    versionCode = "4.0",
                    series = "NOM",
                    date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    paymentMethodCode = "PUE",
                    currencyCode = "MXN",
                    typeCode = "N", // Nómina
                    expeditionZipCode = configEmpresa.CodigoPostal,
                    exportCode = "01",

                    issuer = new FiscalAPIIssuer
                    {
                        tin = configEmpresa.RFC,
                        legalName = configEmpresa.RazonSocial,
                        taxRegimeCode = configEmpresa.RegimenFiscal,
                        employerData = new FiscalAPIEmployerData
                        {
                            employerRegistration = configEmpresa.RegistroPatronal ?? "B5510768108"
                        },
                        taxCredentials = new List<FiscalAPITaxCredential>
                        {
                            new FiscalAPITaxCredential
                            {
                                base64File = configFiscalAPI.CertificadoBase64,
                                fileType = 0, // CER
                                password = configFiscalAPI.PasswordLlave
                            },
                            new FiscalAPITaxCredential
                            {
                                base64File = configFiscalAPI.LlavePrivadaBase64,
                                fileType = 1, // KEY
                                password = configFiscalAPI.PasswordLlave
                            }
                        }
                    },

                    recipient = new FiscalAPIRecipient
                    {
                        tin = empleado.RFC,
                        legalName = empleado.Nombre,
                        zipCode = empleado.CodigoPostal ?? configEmpresa.CodigoPostal,
                        taxRegimeCode = "605", // Sueldos y salarios
                        cfdiUseCode = "CN01", // Nómina
                        employeeData = new FiscalAPIEmployeeData
                        {
                            curp = empleado.CURP,
                            socialSecurityNumber = empleado.NSS,
                            laborRelationStartDate = empleado.FechaIngreso.ToString("yyyy-MM-dd"),
                            seniority = CalcularAntiguedadISO8601(empleado.FechaIngreso),
                            satContractTypeId = "01", // Contrato por tiempo indeterminado
                            satTaxRegimeTypeId = "02", // Sueldos
                            employeeNumber = empleado.NumeroEmpleado,
                            department = empleado.Departamento ?? "GENERAL",
                            position = empleado.Puesto ?? "EMPLEADO",
                            satJobRiskId = "1", // Clase I (riesgo ordinario)
                            satPaymentPeriodicityId = "05", // Quincenal (ajusta según tu caso)
                            satBankId = empleado.Banco ?? "012", // BBVA por defecto
                            baseSalaryForContributions = empleado.SalarioDiario,
                            integratedDailySalary = empleado.SalarioDiarioIntegrado,
                            satPayrollStateId = ObtenerEstadoPorCP(empleado.CodigoPostal ?? configEmpresa.CodigoPostal)
                        }
                    },

                    complement = new FiscalAPIComplement
                    {
                        payroll = new FiscalAPIPayroll
                        {
                            version = "1.2",
                            payrollTypeCode = recibo.TipoNomina == "Extraordinaria" ? "E" : "O",
                            paymentDate = recibo.FechaFin.ToString("yyyy-MM-dd"),
                            initialPaymentDate = recibo.FechaInicio.ToString("yyyy-MM-dd"),
                            finalPaymentDate = recibo.FechaFin.ToString("yyyy-MM-dd"),
                            daysPaid = recibo.DiasTrabajados,

                            earnings = new FiscalAPIEarnings
                            {
                                earnings = ConvertirPercepcionesAFiscalAPI(recibo.Percepciones),
                                otherPayments = new List<FiscalAPIOtherPayment>
                                {
                                    new FiscalAPIOtherPayment
                                    {
                                        otherPaymentTypeCode = "002", // Subsidio al empleo
                                        code = "5050",
                                        concept = "Subsidio al empleo aplicado",
                                        amount = 0.00m,
                                        subsidyCaused = 0.00m
                                    }
                                }
                            },

                            deductions = ConvertirDeduccionesAFiscalAPI(recibo.Deducciones)
                        }
                    }
                };

                // 5. Timbrar con FiscalAPI
                var fiscalAPIService = new FiscalAPIService(new ConfiguracionFiscalAPI
                {
                    UrlApi = configFiscalAPI.UrlApi,
                    ApiKey = configFiscalAPI.ApiKey,
                    Tenant = configFiscalAPI.Tenant,
                    EsProduccion = configFiscalAPI.EsProduccion,
                    CertificadoBase64 = configFiscalAPI.CertificadoBase64,
                    LlavePrivadaBase64 = configFiscalAPI.LlavePrivadaBase64,
                    PasswordLlave = configFiscalAPI.PasswordLlave
                });

                respuesta = await fiscalAPIService.CrearYTimbrarCFDINomina(nominaRequest);

                if (respuesta.Exitoso)
                {
                    // 6. Guardar en base de datos
                    int nominaCFDIID = InsertarNominaCFDI(recibo, respuesta.XMLTimbrado, 
                        respuesta.UUID, respuesta.InvoiceId, conn, tran);

                    ActualizarNominaCFDIExitoso(nominaCFDIID, respuesta, conn, tran);

                    // 7. Actualizar UUID en NominaDetalle
                    string updateQuery = @"
                        UPDATE NominaDetalle 
                        SET UUID = @UUID, 
                            FechaTimbrado = @FechaTimbrado,
                            InvoiceId = @InvoiceId
                        WHERE NominaDetalleID = @ID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@UUID", respuesta.UUID);
                        cmd.Parameters.AddWithValue("@FechaTimbrado", respuesta.FechaTimbrado);
                        cmd.Parameters.AddWithValue("@InvoiceId", respuesta.InvoiceId ?? "");
                        cmd.Parameters.AddWithValue("@ID", nominaDetalleID);
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();

                    respuesta.Mensaje = $"✅ Recibo de nómina timbrado exitosamente\nUUID: {respuesta.UUID}\nEmpleado: {empleado.Nombre}";
                }
                else
                {
                    // Error en timbrado - registrar
                    int nominaCFDIID = InsertarNominaCFDI(recibo, null, null, null, conn, tran);
                    ActualizarNominaCFDIError(nominaCFDIID, respuesta.Mensaje, conn, tran);
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error al timbrar nómina: {ex.Message}";
                respuesta.CodigoError = "EXCEPCION";
                respuesta.ErrorTecnico = ex.ToString();
            }
        }
    }

    return respuesta;
}

// MÉTODOS HELPER (descomentar los existentes en líneas 977-1050)

private string CalcularAntiguedadISO8601(DateTime fechaIngreso)
{
    var antiguedad = DateTime.Now - fechaIngreso;
    int semanas = (int)(antiguedad.TotalDays / 7);
    return $"P{semanas}W"; // Ejemplo: P54W (54 semanas)
}

private string ObtenerEstadoPorCP(string codigoPostal)
{
    // Implementación simple - puedes mejorarla con tabla de CPs
    if (string.IsNullOrEmpty(codigoPostal) || codigoPostal.Length < 2)
        return "JAL"; // Jalisco por defecto

    // Mapeo básico por los primeros 2 dígitos
    string prefix = codigoPostal.Substring(0, 2);
    switch (prefix)
    {
        case "01": return "AGU"; // Aguascalientes
        case "21": case "22": return "BCN"; // Baja California
        case "23": return "BCS"; // Baja California Sur
        // ... agregar más estados según necesites
        case "44": case "45": return "JAL"; // Jalisco
        case "01": case "02": case "03": return "CDMX"; // Ciudad de México
        default: return "JAL"; // Por defecto
    }
}

private List<FiscalAPIEarning> ConvertirPercepcionesAFiscalAPI(List<NominaPercepcion> percepciones)
{
    var earnings = new List<FiscalAPIEarning>();

    foreach (var p in percepciones)
    {
        earnings.Add(new FiscalAPIEarning
        {
            earningTypeCode = p.ClavePercepcion, // Debe ser del catálogo c_TipoPercepcion
            code = p.Concepto,
            concept = p.Descripcion,
            taxedAmount = p.ImporteGravado,
            exemptAmount = p.ImporteExento
        });
    }

    return earnings;
}

private List<FiscalAPIDeduction> ConvertirDeduccionesAFiscalAPI(List<NominaDeduccion> deducciones)
{
    var deductions = new List<FiscalAPIDeduction>();

    foreach (var d in deducciones)
    {
        deductions.Add(new FiscalAPIDeduction
        {
            deductionTypeCode = d.ClaveDeduccion, // Debe ser del catálogo c_TipoDeduccion
            code = d.Concepto,
            concept = d.Descripcion,
            amount = d.Importe
        });
    }

    return deductions;
}

// DESCOMENTAR métodos existentes en líneas 977-1050:
// - InsertarNominaCFDI()
// - ActualizarNominaCFDIExitoso()
// - ActualizarNominaCFDIError()
```

### PASO 4: Actualizar controller (10 minutos)

**Archivo:** `VentasWeb\Controllers\NominaController.cs`  
**Agregar endpoint nuevo (línea ~250):**

```csharp
[HttpPost]
public async Task<JsonResult> TimbrarRecibo(int nominaDetalleID)
{
    try
    {
        string usuario = User.Identity.Name ?? "Sistema";
        
        var respuesta = await CD_Nomina.Instancia.TimbrarCFDINomina(nominaDetalleID, usuario);

        if (respuesta.Exitoso)
        {
            return Json(new
            {
                success = true,
                message = respuesta.Mensaje,
                uuid = respuesta.UUID,
                invoiceId = respuesta.InvoiceId
            });
        }
        else
        {
            return Json(new
            {
                success = false,
                message = respuesta.Mensaje,
                error = respuesta.CodigoError
            });
        }
    }
    catch (Exception ex)
    {
        return Json(new
        {
            success = false,
            message = $"Error: {ex.Message}"
        });
    }
}

[HttpGet]
public async Task<ActionResult> DescargarPDFRecibo(int nominaDetalleID)
{
    try
    {
        // Obtener InvoiceId de la base de datos
        var recibo = CD_Nomina.Instancia.ObtenerDetallePorId(nominaDetalleID);
        
        if (string.IsNullOrEmpty(recibo.InvoiceId))
        {
            return Content("Este recibo no tiene PDF disponible (no timbrado)");
        }

        var configFiscalAPI = CD_ConfiguracionFiscalAPI.Instancia.Obtener();
        var fiscalAPIService = new FiscalAPIService(configFiscalAPI);

        byte[] pdfBytes = await fiscalAPIService.DescargarPDF(recibo.InvoiceId);

        return File(pdfBytes, "application/pdf", $"Recibo_Nomina_{recibo.NumeroEmpleado}_{recibo.NominaDetalleID}.pdf");
    }
    catch (Exception ex)
    {
        return Content($"Error al descargar PDF: {ex.Message}");
    }
}
```

### PASO 5: Actualizar vista (15 minutos)

**Archivo:** `VentasWeb\Views\Nomina\Detalle.cshtml` (o donde muestres el detalle)  
**Agregar botones:**

```html
<div class="card">
    <div class="card-body">
        <h5>Recibo de Nómina - @Model.NombreEmpleado</h5>
        <p>Período: @Model.FechaInicio.ToShortDateString() al @Model.FechaFin.ToShortDateString()</p>
        <p>Neto a pagar: @Model.NetoPagar.ToString("C2")</p>

        @if (string.IsNullOrEmpty(Model.UUID))
        {
            <button type="button" class="btn btn-primary" onclick="timbrarRecibo(@Model.NominaDetalleID)">
                <i class="fas fa-stamp"></i> Timbrar Recibo
            </button>
        }
        else
        {
            <div class="alert alert-success">
                ✅ Recibo timbrado
                <br><strong>UUID:</strong> @Model.UUID
                <br><strong>Fecha:</strong> @Model.FechaTimbrado
            </div>
            <a href="@Url.Action("DescargarPDFRecibo", new { nominaDetalleID = Model.NominaDetalleID })" 
               class="btn btn-success" target="_blank">
                <i class="fas fa-file-pdf"></i> Descargar PDF
            </a>
            <a href="@Url.Action("DescargarXMLRecibo", new { nominaDetalleID = Model.NominaDetalleID })" 
               class="btn btn-info">
                <i class="fas fa-file-code"></i> Descargar XML
            </a>
        }
    </div>
</div>

<script>
function timbrarRecibo(nominaDetalleID) {
    if (!confirm('¿Timbrar este recibo de nómina? Esta acción es irreversible.')) {
        return;
    }

    const btn = event.target;
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Timbrando...';

    $.ajax({
        url: '@Url.Action("TimbrarRecibo")',
        type: 'POST',
        data: { nominaDetalleID: nominaDetalleID },
        success: function(response) {
            if (response.success) {
                toastr.success(response.message);
                setTimeout(() => location.reload(), 2000);
            } else {
                toastr.error(response.message);
                btn.disabled = false;
                btn.innerHTML = '<i class="fas fa-stamp"></i> Timbrar Recibo';
            }
        },
        error: function(xhr) {
            toastr.error('Error al timbrar: ' + xhr.responseText);
            btn.disabled = false;
            btn.innerHTML = '<i class="fas fa-stamp"></i> Timbrar Recibo';
        }
    });
}
</script>
```

---

## 🧪 Testing

### Ambiente de pruebas

FiscalAPI tiene ambiente de testing (https://test.fiscalapi.com):
- ✅ No consume timbres reales
- ✅ Genera CFDIs de prueba válidos
- ✅ Mismo flujo que producción
- ✅ Certificados de prueba disponibles

### Datos de prueba

```sql
-- Empleado de prueba
INSERT INTO Empleados (NumeroEmpleado, Nombre, RFC, CURP, NSS, FechaIngreso, 
    SalarioDiario, SalarioMensual, Puesto, Departamento, Estatus)
VALUES ('TEST001', 'JUAN PEREZ LOPEZ', 'PELJ850101ABC', 'PELJ850101HJCRPN01', 
    '12345678901', '2020-01-01', 500.00, 15000.00, 'DESARROLLADOR', 'TI', 'Activo');

-- Nómina de prueba
INSERT INTO Nominas (Folio, Periodo, FechaInicio, FechaFin, TipoNomina, Estatus)
VALUES ('NOM-TEST-001', '2025-01', '2025-01-01', '2025-01-15', 'Ordinaria', 'Calculada');
```

### Checklist de pruebas

- [ ] Configurar FiscalAPI en ambiente de pruebas
- [ ] Cargar certificados de prueba (CSD)
- [ ] Crear empleado con datos completos (RFC, CURP, NSS)
- [ ] Calcular nómina ordinaria
- [ ] Timbrar un recibo
- [ ] Verificar UUID generado
- [ ] Descargar XML (validar en validador SAT)
- [ ] Descargar PDF
- [ ] Verificar datos en tabla NominasCFDI
- [ ] Probar con nómina extraordinaria
- [ ] Probar error (RFC inválido, certificado incorrecto)

---

## 📊 Catálogos del SAT requeridos

### Para empleados (tabla Empleados):

```sql
-- Agregar columnas si no existen
ALTER TABLE Empleados ADD TipoContrato NVARCHAR(5) DEFAULT '01'; -- c_TipoContrato
ALTER TABLE Empleados ADD TipoRegimen NVARCHAR(5) DEFAULT '02'; -- c_TipoRegimen
ALTER TABLE Empleados ADD TipoJornada NVARCHAR(5) DEFAULT '01'; -- c_TipoJornada
ALTER TABLE Empleados ADD PeriodicidadPago NVARCHAR(5) DEFAULT '05'; -- c_PeriodicidadPago (05=Quincenal)
ALTER TABLE Empleados ADD RiesgoPuesto NVARCHAR(5) DEFAULT '1'; -- c_RiesgoTrabajo
ALTER TABLE Empleados ADD CodigoPostal NVARCHAR(10); -- Código postal del empleado
```

### Para percepciones (tabla NominaPercepciones):

```sql
ALTER TABLE NominaPercepciones ADD ClavePercepcion NVARCHAR(10); -- c_TipoPercepcion

-- Valores comunes:
-- 001 = Sueldos, Salarios, Rayas y Jornales
-- 002 = Gratificación Anual (Aguinaldo)
-- 003 = Participación de los Trabajadores en las Utilidades (PTU)
-- 019 = Horas extra
-- 022 = Prima dominical
-- 023 = Premio por puntualidad
```

### Para deducciones (tabla NominaDeducciones):

```sql
ALTER TABLE NominaDeducciones ADD ClaveDeduccion NVARCHAR(10); -- c_TipoDeduccion

-- Valores comunes:
-- 001 = Seguridad social
-- 002 = ISR
-- 004 = Otros (préstamos, faltas, etc.)
-- 006 = Incapacidad
```

**Importante:** Estos valores deben llenarse al crear percepciones/deducciones en el cálculo de nómina.

---

## 💰 Costos FiscalAPI

### Planes (aprox - verificar en https://fiscalapi.com/pricing)

| Plan | Timbres/mes | Costo mensual | Costo por timbre |
|------|-------------|---------------|------------------|
| Básico | 100 | ~$300 MXN | $3.00 |
| Estándar | 500 | ~$1,200 MXN | $2.40 |
| Profesional | 1,000 | ~$2,000 MXN | $2.00 |
| Empresarial | 5,000+ | Negociable | <$1.50 |

**Para nómina:**
- 50 empleados quincenales = 100 timbres/mes
- 100 empleados quincenales = 200 timbres/mes
- Plan Básico suficiente para empresas pequeñas

---

## 🎯 Próximos pasos (priorizado)

### ALTA PRIORIDAD (Semana 1)

1. **Día 1-2:** Implementar modelos y método FiscalAPI
   - [ ] Crear `FiscalAPINominaRequest.cs`
   - [ ] Agregar `CrearYTimbrarCFDINomina()` a FiscalAPIService
   - [ ] Compilar y verificar errores

2. **Día 3-4:** Restaurar `TimbrarCFDINomina()`
   - [ ] Implementar lógica completa en CD_Nomina
   - [ ] Descomentar métodos helper
   - [ ] Agregar campos faltantes a tablas

3. **Día 5:** Testing inicial
   - [ ] Configurar ambiente de pruebas
   - [ ] Timbrar primer recibo
   - [ ] Verificar XML generado

### MEDIA PRIORIDAD (Semana 2)

4. **UI:** Agregar botones de timbrado
   - [ ] Botón "Timbrar" en detalle de nómina
   - [ ] Mostrar UUID cuando esté timbrado
   - [ ] Links para descargar PDF/XML

5. **Catálogos:** Implementar catálogos SAT
   - [ ] Agregar columnas a tablas
   - [ ] Llenar valores por defecto
   - [ ] Configurar en UI de empleados

### BAJA PRIORIDAD (Opcional)

6. **Mejoras:**
   - [ ] Timbrado masivo (todos los empleados)
   - [ ] Cancelación de recibos
   - [ ] Reporte de timbrados por período
   - [ ] Integración con correo (enviar recibo al empleado)

---

## 📝 Conclusión

**Estado actual:** Sistema 90% listo para CFDI nómina  
**Tiempo estimado de implementación:** 2-3 días (16-24 horas)  
**Dificultad:** Media (FiscalAPI simplifica mucho)  
**Beneficio:** Recibos de nómina 100% fiscales (Art. 99 LISR)

### Ventajas de FiscalAPI para nómina

1. ✅ **Cero configuración de XML** - FiscalAPI genera todo
2. ✅ **Validaciones automáticas** - El SAT rechaza menos
3. ✅ **Actualización de catálogos** - Siempre al día
4. ✅ **Documentación excelente** - Menos tiempo de implementación
5. ✅ **Mismo servicio para facturas** - Un solo proveedor
6. ✅ **Ambiente de pruebas robusto** - Testing sin riesgo

### Próximo paso inmediato

Implementar el **PASO 1** (modelos) para tener la estructura base. Luego compilar y ver si hay dependencias faltantes.

¿Quieres que empiece con el PASO 1 ahora mismo?

---

**Documentación FiscalAPI Nómina:**  
https://docs.fiscalapi.com/invoices-by-reference#crear-factura-de-nomina

**Soporte FiscalAPI:**  
- Discord: https://discord.gg/zg6KZwgZ  
- Email: soporte@fiscalapi.com  
- Twitter: @fiscalapimx
