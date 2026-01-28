# 🔄 COMPARATIVA: TEST vs PRODUCCIÓN

## Vista General

```
┌─────────────────────────────────────────────────────────────┐
│                    FISCALAPI ENVIRONMENTS                    │
├─────────────────────┬─────────────────────────────────────────┤
│      ASPECTO        │  TEST          │  PRODUCCIÓN            │
├─────────────────────┼────────────────┼────────────────────────┤
│ 🔑 API Key          │ sk_test_...    │ sk_live_...            │
│ 🌐 URL              │ test.fiscal... │ api.fiscalapi.com      │
│ 📊 Dashboard        │ test.fiscal... │ live.fiscalapi.com     │
│ 📄 Facturas         │ DE PRUEBA      │ REALES / VIGENTES      │
│ 🔓 SAT              │ NO aparecen    │ Aparecen en SAT        │
│ 🎯 Uso              │ Aprender       │ Producción             │
│ ⚠️  Riesgo          │ Bajo           │ CRÍTICO                │
│ 💾 Datos            │ Ficticio       │ REAL                   │
└─────────────────────┴────────────────┴────────────────────────┘
```

---

## Diferencias Técnicas

### 1. URLs de API

#### TEST
```
Base URL:  https://test.fiscalapi.com
Dashboard: https://test.fiscalapi.com/dashboard
API Key:   sk_test_xxxxxxxxxxxxxxx
Certificados: Autofirmados permitidos
```

#### PRODUCCIÓN
```
Base URL:  https://api.fiscalapi.com
Dashboard: https://live.fiscalapi.com/dashboard
API Key:   sk_live_xxxxxxxxxxxxxxx
Certificados: CSD reales del SAT ÚNICAMENTE
```

---

### 2. Facturas Generadas

#### TEST
```
Folio: 1, 2, 3...
CFDI: <xml válido pero de prueba>
UUID: 12345678-1234-1234-1234-123456789012
Estado en SAT: NO APARECEN
Validación: Sólo internamente en FiscalAPI
Vigencia: NO son vigentes ante SAT
Clientes: No pueden ver en SAT
```

#### PRODUCCIÓN
```
Folio: Secuencial real
CFDI: <xml válido y legal>
UUID: Generado por SAT
Estado en SAT: VIGENTES (aparecen en portal SAT)
Validación: Validadas por SAT automáticamente
Vigencia: SON vigentes 180 días
Clientes: PUEDEN consultar en SAT
```

---

### 3. Certificados

#### TEST
```
Certificado: Puede ser cualquiera
Vigencia: No importa
Validación: Mínima
Uso: Pruebas solamente
```

#### PRODUCCIÓN
```
Certificado: MUST ser CSD real del SAT
Vigencia: MUST estar vigente (≤365 días)
Validación: Validación completa SAT
Uso: Facturas legales y vigentes
```

---

### 4. RFC

#### TEST
```
RFC: Puede ser ficticio
Validez: No importa
Ejemplo: ABC123456ABC o GAMA6111156JA (test)
Validación: Mínima
```

#### PRODUCCIÓN
```
RFC: MUST ser real y existente
Validez: MUST estar activo en SAT
Ejemplo: GAMA6111156JA (real)
Validación: Validación completa SAT
```

---

## Flujo de Cambio

### Antes (TEST)
```
Tu App (TEST)
    ↓
FiscalAPI TEST (sk_test_...)
    ↓
Bases de datos FiscalAPI TEST
    ↓
❌ NO entra a SAT
```

### Después (PRODUCCIÓN)
```
Tu App (PRODUCCIÓN)
    ↓
FiscalAPI PRODUCCIÓN (sk_live_...)
    ↓
Bases de datos FiscalAPI PRODUCCIÓN
    ↓
SAT (validación automática)
    ↓
✅ VIGENTES y CONSULTABLES
```

---

## Estado en SAT

### TEST
```
Factura generada en TEST

┌─────────────────────────────┐
│  SAT                        │
│ ┌───────────────────────┐   │
│ │ Consulta de CFDI      │   │
│ │ (¿Esta factura es     │   │
│ │  válida?)             │   │
│ ├───────────────────────┤   │
│ │ Resultado: NO EXISTE  │   │
│ │ (porque fue de TEST)  │   │
│ └───────────────────────┘   │
└─────────────────────────────┘

❌ La factura no aparece en SAT
```

### PRODUCCIÓN
```
Factura generada en PRODUCCIÓN

┌─────────────────────────────┐
│  SAT                        │
│ ┌───────────────────────┐   │
│ │ Consulta de CFDI      │   │
│ │ (UUID: 1234-5678-...) │   │
│ ├───────────────────────┤   │
│ │ ✓ VIGENTE             │   │
│ │ Folio: 12345          │   │
│ │ RFC: GAMA6111156JA    │   │
│ │ Monto: $1,234.56      │   │
│ └───────────────────────┘   │
└─────────────────────────────┘

✅ La factura aparece y es vigente
```

---

## Indicadores en Dashboard FiscalAPI

### TEST Dashboard
```
https://test.fiscalapi.com/dashboard

┌─ API Keys
│  └─ sk_test_47126aed_6c71_4060_b05b_932c4423dd00
│
├─ Invoices (últimas facturas de prueba)
│  └─ Status: Test - No enviadas a SAT
│
├─ Tax Files
│  └─ Certificados (pueden ser test)
│
└─ Status: TEST ENVIRONMENT
   ⓘ "Las facturas aquí no son vigentes"
```

### PRODUCCIÓN Dashboard
```
https://live.fiscalapi.com/dashboard

┌─ API Keys
│  └─ sk_live_a1b2c3d4e5f6g7h8i9j0
│
├─ Invoices (facturas reales)
│  └─ Status: Vigente - Enviadas a SAT ✓
│
├─ Tax Files
│  └─ Certificados CSD reales del SAT
│
└─ Status: PRODUCTION ENVIRONMENT
   ⚠️ "Las facturas aquí SÍ son vigentes y legales"
```

---

## Resumen de Cambios en BD

```sql
-- TEST (actual)
SELECT * FROM ConfiguracionPAC WHERE ConfigPACID = 1;
/*
ConfigPACID: 1
EsProduccion: 0
ApiKey: sk_test_47126aed_6c71_4060_b05b_932c4423dd00
BaseURL: https://test.fiscalapi.com
*/

-- PRODUCCIÓN (después de cambio)
SELECT * FROM ConfiguracionPAC WHERE ConfigPACID = 1;
/*
ConfigPACID: 1
EsProduccion: 1  ← CAMBIÓ
ApiKey: sk_live_a1b2c3d4e5f6g7h8i9j0  ← CAMBIÓ
BaseURL: https://api.fiscalapi.com  ← CAMBIÓ
*/
```

---

## Impacto en Código

### FacturaController.cs
```csharp
// Automáticamente usa la URL correcta
var config = CD_Factura.Instancia.ObtenerConfiguracionPAC(...);
string baseUrl = config.EsProduccion 
    ? "https://live.fiscalapi.com"    // ← PRODUCCIÓN
    : "https://test.fiscalapi.com";   // ← TEST
```

### FiscalAPIService.cs
```csharp
// Automáticamente usa el ApiKey correcto
string apiKey = config.ApiKey;  // sk_test_ o sk_live_ según config
```

---

## Checklist de Diferencias

|  | TEST | PRODUCCIÓN |
|--|------|-----------|
| **API Key prefix** | sk_test_ | sk_live_ |
| **URL Base** | test.fiscalapi.com | api.fiscalapi.com |
| **Certificados** | Test/cualquiera | CSD real SAT |
| **RFC** | Ficticio ok | Real y activo |
| **Facturas vigentes** | NO | SÍ |
| **Aparecen en SAT** | NO | SÍ |
| **Riesgo** | Bajo | ALTO |
| **Revocable** | SÍ | NO fácil |
| **Uso** | Aprender | Producción |

---

## Impacto Operacional

### Para Ti (Desarrollador)
```
TEST:
- Puedes generar infinitas facturas
- Puedes timbrar la misma factura varias veces
- Puedes cambiar datos libremente
- Sin consecuencias legales

PRODUCCIÓN:
- Cada factura cuenta como timbrada
- No puedes anular facturas fácilmente
- Los datos son LEGALES
- Responsabilidad SAT
```

### Para Clientes
```
TEST:
- No ven facturas en SAT
- No pueden descargar CFDI
- No pueden solicitar cambios

PRODUCCIÓN:
- VEN facturas en SAT portal
- Pueden descargar CFDI legales
- Pueden solicitar cancelación (si aplica)
```

---

## Recuperación en Caso de Error

### Si Algo Sale Mal en PRODUCCIÓN
```
OPCIÓN 1: Volver a TEST (reversible)
- Ejecuta: VOLVER_A_TEST.sql
- Las facturas ya generadas siguen siendo reales
- Las nuevas serán de TEST

OPCIÓN 2: Cancelar facturas (se requiere folio)
- En FiscalAPI Dashboard
- Requiere autorización SAT
- Tarda 24-48 horas

OPCIÓN 3: Contactar SAT
- Si hay error crítico
- Pueden ayudar con cancelación
```

---

## Timeline de Cambio

```
MOMENTO 1: Cambio en BD
  EsProduccion = 1
  ApiKey = sk_live_...
  
           ↓ (Inmediato)
  
MOMENTO 2: Compilas la app
  Rebuild Solution
  
           ↓ (Inmediato)
  
MOMENTO 3: Ejecutas F5
  App carga con nueva config
  
           ↓ (Cuando generes factura)
  
MOMENTO 4: Primera factura
  Se timbra en FiscalAPI PRODUCCIÓN
  Se envía a SAT
  
           ↓ (Automático)
  
MOMENTO 5: SAT valida
  Genera UUID oficial
  Marca como VIGENTE
  
           ↓ (Transparente)
  
RESULTADO: Factura LEGAL y VIGENTE
```

---

## Conclusión

```
TEST:
  ✓ Seguro, sin consecuencias
  ✓ Para aprender y probar
  ✓ Revocable fácilmente
  ✗ No genera facturas legales

PRODUCCIÓN:
  ✓ Genera facturas LEGALES
  ✓ Vigentes ante SAT
  ✓ Válidas y consultables
  ✗ Cambios difíciles o imposibles
  ✗ Responsabilidad legal
```

**Cambios solo si estás seguro de que todo funciona correctamente en TEST.** ⚠️
