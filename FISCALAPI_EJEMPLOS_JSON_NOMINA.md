# FiscalAPI - Ejemplos JSON Reales para CFDI Nómina

## 📌 Fuente
**Documentación oficial:** https://documenter.getpostman.com/view/4346593/2sB2j4eqXr#67e9d554-ce09-4066-b065-688e1919713e

**Sección:** Complemento Nómina

---

## ✅ Ejemplo 1: Nómina Ordinaria (Caso más común)

### Request JSON Completo

```json
{
    "versionCode": "4.0",
    "series": "F",
    "date": "2025-04-17T08:56:40",
    "paymentMethodCode": "PUE",
    "currencyCode": "MXN",
    "typeCode": "N",                      // ← N = Nómina
    "expeditionZipCode": "20000",
    "exportCode": "01",
    "issuer": {
        "tin": "EKU9003173C9",           // RFC Emisor
        "legalName": "ESCUELA KEMPER URGATE",
        "taxRegimeCode": "601",
        "employerData": {
            "employerRegistration": "B5510768108"  // Registro Patronal IMSS
        },
        "taxCredentials": [
            {
                "base64File": "MII...",   // Certificado .CER base64
                "fileType": 0,
                "password": "12345678a"
            },
            {
                "base64File": "MII...",   // Llave privada .KEY base64
                "fileType": 1,
                "password": "12345678a"
            }
        ]
    },
    "recipient": {
        "tin": "FUNK671228PH6",           // RFC Empleado
        "legalName": "KARLA FUENTE NOLASCO",
        "zipCode": "01160",
        "taxRegimeCode": "605",
        "cfdiUseCode": "CN01",             // Nómina
        "employeeData": {
            "curp": "XEXX010101MNEXXXA8",
            "socialSecurityNumber": "04078873454",  // NSS
            "laborRelationStartDate": "2024-08-18",
            "seniority": "P54W",           // ISO 8601 Duration
            "satContractTypeId": "01",     // Catálogo SAT c_TipoContrato
            "satTaxRegimeTypeId": "02",    // Catálogo SAT c_TipoRegimen
            "employeeNumber": "123456789",
            "department": "GenAI",
            "position": "Sr Software Engineer",
            "satJobRiskId": "1",           // Catálogo SAT c_RiesgoPuesto
            "satPaymentPeriodicityId": "05", // 05 = Quincenal
            "satBankId": "012",            // Catálogo SAT c_Banco
            "baseSalaryForContributions": 2828.50,
            "integratedDailySalary": 0.00,
            "satPayrollStateId": "JAL"     // Estado donde labora
        }
    },
    "complement": {
        "payroll": {
            "version": "1.2",              // Versión complemento nómina
            "payrollTypeCode": "O",        // O=Ordinaria, E=Extraordinaria
            "paymentDate": "2025-08-30",
            "initialPaymentDate": "2025-07-31",
            "finalPaymentDate": "2025-08-30",
            "daysPaid": 30,
            "earnings": {
                "earnings": [
                    {
                        "earningTypeCode": "001",  // c_TipoPercepcion (Sueldos)
                        "code": "1003",
                        "concept": "Sueldo Nominal",
                        "taxedAmount": 95030.00,
                        "exemptAmount": 0.00
                    },
                    {
                        "earningTypeCode": "005",  // Reintegro ISR
                        "code": "5913",
                        "concept": "Fondo de Ahorro Aportación Patrón",
                        "taxedAmount": 0.00,
                        "exemptAmount": 4412.46
                    },
                    {
                        "earningTypeCode": "038",  // Otros
                        "code": "1885",
                        "concept": "Bono Ingles",
                        "taxedAmount": 14254.50,
                        "exemptAmount": 0.00
                    },
                    {
                        "earningTypeCode": "029",  // Vales despensa
                        "code": "1941",
                        "concept": "Vales Despensa",
                        "taxedAmount": 0.00,
                        "exemptAmount": 3439.00
                    },
                    {
                        "earningTypeCode": "038",
                        "code": "1824",
                        "concept": "Herramientas Teletrabajo",
                        "taxedAmount": 273.00,
                        "exemptAmount": 0.00
                    }
                ],
                "otherPayments": [
                    {
                        "otherPaymentTypeCode": "002",  // Subsidio empleo
                        "code": "5050",
                        "concept": "Exceso de subsidio al empleo",
                        "amount": 0.00,
                        "subsidyCaused": 0.00
                    }
                ]
            },
            "deductions": [
                {
                    "deductionTypeCode": "002",  // c_TipoDeduccion (ISR)
                    "code": "5003",
                    "concept": "ISR Causado",
                    "amount": 27645.52
                },
                {
                    "deductionTypeCode": "004",  // Otros
                    "code": "5910",
                    "concept": "Fondo de ahorro Empleado",
                    "amount": 4412.46
                },
                {
                    "deductionTypeCode": "004",
                    "code": "5914",
                    "concept": "Fondo de Ahorro Patrón",
                    "amount": 4412.46
                },
                {
                    "deductionTypeCode": "004",
                    "code": "1966",
                    "concept": "Contribución póliza GMM",
                    "amount": 519.91
                },
                {
                    "deductionTypeCode": "004",
                    "code": "1934",
                    "concept": "Descuento Vales Despensa",
                    "amount": 1.00
                },
                {
                    "deductionTypeCode": "004",
                    "code": "1942",
                    "concept": "Vales Despensa Electrónico",
                    "amount": 3439.00
                },
                {
                    "deductionTypeCode": "001",  // Seguridad Social
                    "code": "1895",
                    "concept": "IMSS",
                    "amount": 2391.13
                }
            ]
        }
    }
}
```

### Response JSON

```json
{
  "data": {
    "versionCode": "4.0",
    "series": "F",
    "number": "EKU9003173C9-136",
    "date": "2025-10-19T10:25:16.000",
    "subtotal": 117408.96,
    "discount": 42821.48,
    "currencyCode": "MXN",
    "exchangeRate": 1,
    "total": 74587.48,
    "typeCode": "N",
    "exportCode": "01",
    "uuid": "a25e3739-a0ce-4c12-9ac0-283e035b9bf8",  // ← UUID del SAT
    "consecutive": 338,
    "status": null,
    "paymentMethodCode": "PUE",
    "expeditionZipCode": "20000",
    "issuer": {
      "id": null,
      "tin": "EKU9003173C9",
      "legalName": "ESCUELA KEMPER URGATE",
      "taxRegimeCode": "601"
    },
    "recipient": {
      "id": null,
      "tin": "FUNK671228PH6",
      "legalName": "KARLA FUENTE NOLASCO",
      "zipCode": "01160",
      "taxRegimeCode": "605",
      "cfdiUseCode": "CN01",
      "email": null
    },
    "items": [
      {
        "itemCode": "84111505",          // Producto SAT para nómina
        "quantity": 1,
        "unitOfMeasurementCode": "ACT",  // Actividad
        "description": "Pago de nómina",
        "unitPrice": 117408.96,
        "taxObjectCode": "01",
        "discount": 42821.48,
        "itemTaxes": []
      }
    ],
    "responses": [
      {
        "invoiceId": "00c6f323-cf1d-4192-b3d0-eae33202a17a",  // ← Para descargar PDF
        "invoiceUuid": "a25e3739-a0ce-4c12-9ac0-283e035b9bf8",
        "invoiceCertificateNumber": "30001000000500003416",
        "invoiceBase64Sello": "Yd/G/pN5plai29gf3bfyOmXM4oRIKewUiyg3GdBIy28...",
        "invoiceSignatureDate": "2025-10-20T12:56:42.000",
        "invoiceBase64QrCode": "iVBORw0KGgoAAAANSUhEUgAAAJIA...",  // QR Code
        "invoiceBase64": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4...",  // XML completo
        "satBase64Sello": "G/cOLDhL4JVSJaPltTHuFYhV86/mkPgUUfcAljXfOL4Dd...",
        "satBase64OriginalString": "fHwxLjF8YTI1ZTM3MzktYTBjZS00YzEyLTlhYzAtMjgzZTAzNWI5YmY4f...",
        "satCertificateNumber": "30001000000500003456",
        "id": "493f66c5-e366-485b-a3ce-1927e3d59710",
        "createdAt": "2025-10-20T12:56:41.840",
        "updatedAt": "2025-10-20T12:56:41.840"
      }
    ],
    "metadata": {
      "mode": "values"
    },
    "id": "00c6f323-cf1d-4192-b3d0-eae33202a17a",
    "createdAt": "2025-10-20T12:56:41.840",
    "updatedAt": "2025-10-20T12:56:41.840"
  },
  "succeeded": true,
  "message": "",
  "details": "",
  "httpStatusCode": 200
}
```

---

## ✅ Ejemplo 2: Nómina Asimilados a Salarios

### Request JSON

```json
{
    "versionCode": "4.0",
    "series": "F",
    "date": "2025-04-17T08:56:40",
    "paymentMethodCode": "PUE",
    "currencyCode": "MXN",
    "typeCode": "N",
    "expeditionZipCode": "06880",
    "exportCode": "01",
    "issuer": {
        "tin": "EKU9003173C9",
        "legalName": "ESCUELA KEMPER URGATE",
        "taxRegimeCode": "601",
        "employerData": {
            "originEmployerTin": "EKU9003173C9"  // ← RFC origen (asimilados)
        },
        "taxCredentials": [ /* ... certificados ... */ ]
    },
    "recipient": {
        "tin": "CACX7605101P8",
        "legalName": "XOCHILT CASAS CHAVEZ",
        "zipCode": "36257",
        "taxRegimeCode": "605",
        "cfdiUseCode": "CN01",
        "employeeData": {
            "curp": "XEXX010101HNEXXXA4",
            "satContractTypeId": "09",           // ← 09 = Asimilado
            "satUnionizedStatusId": "No",
            "satTaxRegimeTypeId": "09",          // ← 09 = Asimilado
            "employeeNumber": "00002",
            "department": "ADMINISTRACION",
            "position": "DIRECTOR DE ADMINISTRACION",
            "satPaymentPeriodicityId": "99",     // ← 99 = Otra periodicidad
            "satBankId": "012",
            "bankAccount": "1111111111",
            "satPayrollStateId": "CMX"
        }
    },
    "complement": {
        "payroll": {
            "version": "1.2",
            "payrollTypeCode": "E",              // ← E = Extraordinaria
            "paymentDate": "2023-06-02T00:00:00",
            "initialPaymentDate": "2023-06-01T00:00:00",
            "finalPaymentDate": "2023-06-02T00:00:00",
            "daysPaid": 1,
            "earnings": {
                "earnings": [
                    {
                        "earningTypeCode": "046",  // ← Ingresos asimilados
                        "code": "010046",
                        "concept": "INGRESOS ASIMILADOS A SALARIOS",
                        "taxedAmount": 111197.73,
                        "exemptAmount": 0.00
                    }
                ],
                "otherPayments": []
            },
            "deductions": [
                {
                    "deductionTypeCode": "002",
                    "code": "020002",
                    "concept": "ISR",
                    "amount": 36197.73
                }
            ]
        }
    }
}
```

---

## ✅ Ejemplo 3: Nómina con Bonos, Fondo de Ahorro y Deducciones

### Request JSON (Destacando peculiaridades)

```json
{
    "versionCode": "4.0",
    "series": "F",
    "date": "2025-04-17T08:56:40",
    "paymentMethodCode": "PUE",
    "currencyCode": "MXN",
    "typeCode": "N",
    "expeditionZipCode": "20000",
    "exportCode": "01",
    "issuer": {
        "tin": "EKU9003173C9",
        "legalName": "ESCUELA KEMPER URGATE",
        "taxRegimeCode": "601",
        "employerData": {
            "employerRegistration": "Z0000001234"
        },
        "taxCredentials": [ /* ... */ ]
    },
    "recipient": {
        "tin": "XOJI740919U48",
        "legalName": "INGRID XODAR JIMENEZ",
        "zipCode": "76028",
        "taxRegimeCode": "605",
        "cfdiUseCode": "CN01",
        "employeeData": {
            "curp": "XEXX010101MNEXXXA8",
            "socialSecurityNumber": "0000000000",
            "laborRelationStartDate": "2022-03-02T00:00:00",
            "seniority": "P66W",               // 66 semanas
            "satContractTypeId": "01",
            "satUnionizedStatusId": "No",
            "satTaxRegimeTypeId": "02",
            "employeeNumber": "111111",
            "satJobRiskId": "4",
            "satPaymentPeriodicityId": "02",   // Semanal
            "integratedDailySalary": 180.96,
            "satPayrollStateId": "GUA"
        }
    },
    "complement": {
        "payroll": {
            "version": "1.2",
            "payrollTypeCode": "O",
            "paymentDate": "2023-06-11T00:00:00",
            "initialPaymentDate": "2023-06-05T00:00:00",
            "finalPaymentDate": "2023-06-11T00:00:00",
            "daysPaid": 7,
            "earnings": {
                "earnings": [
                    {
                        "earningTypeCode": "001",
                        "code": "SP01",
                        "concept": "SUELDO",
                        "taxedAmount": 1210.30,
                        "exemptAmount": 0.00
                    },
                    {
                        "earningTypeCode": "010",  // Premio
                        "code": "SP02",
                        "concept": "PREMIO PUNTUALIDAD",
                        "taxedAmount": 121.03,
                        "exemptAmount": 0.00
                    },
                    {
                        "earningTypeCode": "029",  // Despensa
                        "code": "SP03",
                        "concept": "MONEDERO ELECTRONICO",
                        "taxedAmount": 0.00,
                        "exemptAmount": 269.43
                    },
                    {
                        "earningTypeCode": "010",
                        "code": "SP04",
                        "concept": "PREMIO DE ASISTENCIA",
                        "taxedAmount": 121.03,
                        "exemptAmount": 0.00
                    },
                    {
                        "earningTypeCode": "005",  // Fondo ahorro
                        "code": "SP54",
                        "concept": "APORTACION FONDO AHORRO",
                        "taxedAmount": 0.00,
                        "exemptAmount": 121.03
                    }
                ],
                "otherPayments": [
                    {
                        "otherPaymentTypeCode": "002",  // Subsidio empleo
                        "code": "ISRSUB",
                        "concept": "Subsidio ISR para empleo",
                        "amount": 0.0,
                        "subsidyCaused": 0.0,
                        "balanceCompensation": {      // ← Compensación saldos
                            "favorableBalance": 0.0,
                            "year": 2022,
                            "remainingFavorableBalance": 0.0
                        }
                    }
                ]
            },
            "deductions": [
                {
                    "deductionTypeCode": "004",
                    "code": "ZA09",
                    "concept": "APORTACION FONDO AHORRO",
                    "amount": 121.03
                },
                {
                    "deductionTypeCode": "002",
                    "code": "ISR",
                    "concept": "ISR",
                    "amount": 36.57
                },
                {
                    "deductionTypeCode": "001",
                    "code": "IMSS",
                    "concept": "Cuota de Seguridad Social EE",
                    "amount": 30.08
                },
                {
                    "deductionTypeCode": "004",
                    "code": "ZA68",
                    "concept": "DEDUCCION FDO AHORRO PAT",
                    "amount": 121.03
                },
                {
                    "deductionTypeCode": "018",  // ← Caja ahorro
                    "code": "ZA11",
                    "concept": "APORTACION CAJA AHORRO",
                    "amount": 300.00
                }
            ]
        }
    }
}
```

---

## 📊 Catálogos SAT Importantes

### c_TipoPercepcion (Percepciones)

| Código | Descripción |
|--------|-------------|
| 001 | Sueldos, salarios, rayas y jornales |
| 002 | Gratificación anual (aguinaldo) |
| 003 | Participación de utilidades (PTU) |
| 004 | Reembolso de gastos médicos |
| 005 | Fondo de ahorro |
| 009 | Contribuciones a cargo del trabajador pagadas por el patrón |
| 010 | Premios por puntualidad |
| 019 | Horas extra |
| 022 | Primas de seguro de vida |
| 025 | Viáticos |
| 029 | Vales de despensa |
| 038 | Otras percepciones |
| 039 | Jubilaciones, pensiones o haberes de retiro |
| 044 | Jubilaciones, pensiones o haberes de retiro en parcialidades |
| 045 | Ingresos en acciones o títulos valor |
| 046 | Ingresos asimilados a salarios |
| 047 | Alimentación |
| 048 | Habitación |
| 049 | Premios por asistencia |

### c_TipoDeduccion (Deducciones)

| Código | Descripción |
|--------|-------------|
| 001 | Seguridad social (IMSS empleado) |
| 002 | ISR |
| 003 | Aportaciones a retiro, cesantía y vejez |
| 004 | Otros |
| 005 | Aportaciones a fondo de vivienda (Infonavit empleado) |
| 006 | Descuento por incapacidad |
| 007 | Pensión alimenticia |
| 008 | Renta |
| 009 | Préstamos provenientes del fondo nacional de la vivienda |
| 010 | Pago por crédito de vivienda |
| 011 | Pago de abonos Infonavit |
| 012 | Anticipo de salarios |
| 013 | Pagos hechos con exceso al trabajador |
| 014 | Errores |
| 015 | Pérdidas |
| 016 | Averías |
| 017 | Adelanto de salarios |
| 018 | Depósitos a favor del trabajador (caja ahorro) |
| 019 | Cuotas sindicales |
| 020 | Ausencia (Ausentismo) |
| 021 | Cuotas obrero patronales |
| 022 | Impuestos locales |
| 023 | Aportaciones voluntarias |
| 024 | Ajuste en gratificación anual (aguinaldo) |
| 025 | Ajuste en PTU |
| 026 | Ajuste en reembolso de gastos médicos |
| 027 | Ajuste en fondo de ahorro |
| 028 | Ajuste en otras percepciones |
| 107 | Ajuste en subsidio para el empleo (efectivamente entregado al trabajador) |

### c_TipoContrato

| Código | Descripción |
|--------|-------------|
| 01 | Contrato de trabajo por tiempo indeterminado |
| 02 | Contrato de trabajo para obra determinada |
| 03 | Contrato de trabajo por tiempo determinado |
| 04 | Contrato de trabajo por temporada |
| 05 | Contrato de trabajo sujeto a prueba |
| 06 | Contrato de capacitación inicial |
| 07 | Modalidad de contratación por pago de hora laborada |
| 08 | Modalidad de contratación por trabajo de temporada |
| 09 | Modalidad de contratación por comisión mercantil |
| 10 | Modalidad de contratación por relación de trabajo por tiempo indeterminado o superior a un año |
| 99 | Otro contrato |

### c_TipoRegimen (Régimen fiscal empleado)

| Código | Descripción |
|--------|-------------|
| 02 | Sueldos (Régimen Obligatorio) |
| 03 | Jubilados |
| 04 | Pensionados |
| 05 | Asimilados Miembros Sociedades Cooperativas Producción |
| 06 | Asimilados Integrantes Sociedades Asociaciones Civiles |
| 07 | Asimilados Miembros consejos |
| 08 | Asimilados comisionistas |
| 09 | Asimilados Honorarios |
| 10 | Asimilados acciones |
| 11 | Asimilados otros |
| 12 | Jubilados o Pensionados |
| 13 | Indemnización o separación |
| 99 | Otro Régimen |

### c_PeriodicidadPago

| Código | Descripción |
|--------|-------------|
| 01 | Diario |
| 02 | Semanal |
| 03 | Catorcenal |
| 04 | Quincenal |
| 05 | Mensual |
| 06 | Bimestral |
| 07 | Unidad obra |
| 08 | Comisión |
| 09 | Precio alzado |
| 10 | Decenal |
| 99 | Otra periodicidad |

### c_EstadosPayroll (Estados)

| Código | Estado |
|--------|--------|
| AGU | Aguascalientes |
| BCN | Baja California |
| BCS | Baja California Sur |
| CAM | Campeche |
| CHH | Chihuahua |
| CHP | Chiapas |
| CMX | Ciudad de México |
| COA | Coahuila |
| COL | Colima |
| DUR | Durango |
| GRO | Guerrero |
| GUA | Guanajuato |
| HID | Hidalgo |
| JAL | Jalisco |
| MEX | México |
| MIC | Michoacán |
| MOR | Morelos |
| NAY | Nayarit |
| NLE | Nuevo León |
| OAX | Oaxaca |
| PUE | Puebla |
| QRO | Querétaro |
| ROO | Quintana Roo |
| SIN | Sinaloa |
| SLP | San Luis Potosí |
| SON | Sonora |
| TAB | Tabasco |
| TAM | Tamaulipas |
| TLA | Tlaxcala |
| VER | Veracruz |
| YUC | Yucatán |
| ZAC | Zacatecas |

---

## 🔑 Campos Clave para Mapeo

### Del Sistema → FiscalAPI

| Campo Sistema | Campo FiscalAPI | Notas |
|---------------|-----------------|-------|
| `Empleados.RFC` | `recipient.tin` | RFC del empleado |
| `Empleados.Nombre` | `recipient.legalName` | Nombre completo |
| `Empleados.CURP` | `recipient.employeeData.curp` | CURP |
| `Empleados.NSS` | `recipient.employeeData.socialSecurityNumber` | NSS (11 dígitos) |
| `Empleados.FechaIngreso` | `recipient.employeeData.laborRelationStartDate` | Formato: YYYY-MM-DD |
| `Empleados.Puesto` | `recipient.employeeData.position` | Puesto |
| `Empleados.Departamento` | `recipient.employeeData.department` | Departamento |
| `Empleados.SalarioDiario` | `recipient.employeeData.baseSalaryForContributions` | Salario base |
| `Empleados.SalarioDiarioIntegrado` | `recipient.employeeData.integratedDailySalary` | SDI |
| `NominaDetalle.FechaInicio` | `complement.payroll.initialPaymentDate` | Inicio período |
| `NominaDetalle.FechaFin` | `complement.payroll.finalPaymentDate` | Fin período |
| `NominaDetalle.DiasLaborados` | `complement.payroll.daysPaid` | Días pagados |
| `NominaPercepciones.Concepto` | `complement.payroll.earnings.earnings[].concept` | Concepto percepción |
| `NominaPercepciones.Monto` | `complement.payroll.earnings.earnings[].taxedAmount` | Monto gravado |
| `NominaDeducciones.Concepto` | `complement.payroll.deductions[].concept` | Concepto deducción |
| `NominaDeducciones.Monto` | `complement.payroll.deductions[].amount` | Monto deducción |

### Response FiscalAPI → Base de Datos

| Campo Response | Campo Tabla NominasCFDI | Notas |
|----------------|-------------------------|-------|
| `data.uuid` | `UUID` | UUID del SAT |
| `data.responses[0].invoiceBase64` | `XMLTimbrado` | XML base64 |
| `data.responses[0].invoiceBase64Sello` | `SelloCFD` | Sello del emisor |
| `data.responses[0].satBase64Sello` | `SelloSAT` | Sello del SAT |
| `data.responses[0].satBase64OriginalString` | `CadenaOriginal` | Cadena original |
| `data.responses[0].satCertificateNumber` | `NoCertificadoSAT` | Número certificado SAT |
| `data.responses[0].invoiceId` | `InvoiceId` | ID para descargar PDF |
| `data.responses[0].invoiceSignatureDate` | `FechaTimbrado` | Fecha timbrado |

---

## 🚀 Endpoint y Headers

### URL Base
```
Pruebas: https://test.fiscalapi.com/api/v4/invoices
Producción: https://fiscalapi.com/api/v4/invoices
```

### Headers Requeridos
```http
POST /api/v4/invoices HTTP/1.1
Host: test.fiscalapi.com
X-API-KEY: <tu-api-key>
X-TENANT-KEY: <tu-tenant-key>
X-TIME-ZONE: America/Mexico_City
Content-Type: application/json
```

### Método
```
POST
```

---

## ✅ Validaciones SAT Importantes

### 1. Antigüedad (Seniority)
Formato ISO 8601 Duration: `P[n]Y[n]M[n]DT[n]H[n]M[n]S`

Ejemplos:
- `P54W` = 54 semanas
- `P1Y2M` = 1 año, 2 meses
- `P66W` = 66 semanas

### 2. Fechas
Formato: `YYYY-MM-DD` o `YYYY-MM-DDTHH:MM:SS`

Ejemplo: `2025-08-30T00:00:00`

### 3. Montos
- 2 decimales máximo
- Sin comas separadoras
- Gravado + Exento = Total

### 4. NSS (Número Seguridad Social)
- 11 dígitos
- Si no aplica: `"0000000000"`

### 5. CURP
- 18 caracteres
- Formato válido del SAT
- Si no se tiene: usar CURP genérico `"XEXX010101HNEXXXA4"`

---

## 🎯 Resumen: Lo Esencial

**Para implementar, necesitas mapear:**

1. ✅ **typeCode**: Siempre `"N"` (Nómina)
2. ✅ **issuer.employerData.employerRegistration**: Registro Patronal IMSS
3. ✅ **recipient.employeeData**: Todos los datos del empleado (CURP, NSS, antigüedad, etc.)
4. ✅ **complement.payroll**: Períodos, días, percepciones y deducciones
5. ✅ **earnings**: Lista de percepciones con `earningTypeCode` del catálogo SAT
6. ✅ **deductions**: Lista de deducciones con `deductionTypeCode` del catálogo SAT

**Lo que FiscalAPI hace automáticamente:**
- ✅ Genera XML completo con Complemento de Nómina 1.2
- ✅ Calcula totales (subtotal, descuento, total)
- ✅ Timbra con el SAT
- ✅ Regresa UUID, XML, sellos, QR Code
- ✅ Permite descargar PDF

**Ventaja sobre competidores:**
🚀 **NO tienes que construir el XML manualmente** - FiscalAPI lo hace todo.

---

## 📌 Próximos Pasos

1. **Agregar campos SAT a la base de datos:**
   ```sql
   ALTER TABLE Empleados ADD TipoContrato NVARCHAR(5) DEFAULT '01';
   ALTER TABLE Empleados ADD TipoRegimen NVARCHAR(5) DEFAULT '02';
   ALTER TABLE NominaPercepciones ADD ClavePercepcion NVARCHAR(10);
   ALTER TABLE NominaDeducciones ADD ClaveDeduccion NVARCHAR(10);
   ```

2. **Crear modelo `FiscalAPINominaRequest.cs`** con estructura de estos ejemplos

3. **Implementar método `CrearYTimbrarCFDINomina()`** en FiscalAPIService

4. **Restaurar `TimbrarCFDINomina()`** en CD_Nomina.cs usando el servicio

5. **Probar en ambiente de pruebas** con certificados de prueba del SAT

---

**📅 Fecha:** 29 enero 2026  
**✅ Status:** Ejemplos completos verificados de documentación oficial  
**🔗 Fuente:** https://documenter.getpostman.com/view/4346593/2sB2j4eqXr
