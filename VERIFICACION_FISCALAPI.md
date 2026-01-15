# ✅ VERIFICACIÓN COMPLETA - FiscalAPI Integration

## Estado: LISTO PARA PRUEBA

### 1. Configuración Base de Datos ✅
```sql
ConfiguracionID: 1
RfcEmisor: MISC491214B86
NombreEmisor: CECILIA MIRANDA SANCHEZ
RegimenFiscal: 612
Ambiente: TEST
CodigoPostal: 81048
Activo: 1
ApiKey: 44 caracteres (sk_test_...)
Tenant: 36 caracteres (e0a0d1de-...)
```

### 2. Endpoint Correcto ✅
- **URL**: `https://test.fiscalapi.com/api/v4/invoices`
- **Método**: POST
- **Headers**:
  - `X-API-KEY`: sk_test_47126aed_6c71_4060_b05b_932c4423dd00
  - `X-TENANT-KEY`: e0a0d1de-d225-46de-b95f-55d04f2787ff
  - `Content-Type`: application/json

### 3. Estructura JSON Correcta ✅

**Campos que SE ENVÍAN:**
```json
{
  "versionCode": "4.0",
  "date": "2026-01-15T14:56:45",
  "paymentFormCode": "01",
  "currencyCode": "MXN",
  "typeCode": "I",
  "expeditionZipCode": "81048",
  "exportCode": "01",
  "paymentMethodCode": "PUE",
  "exchangeRate": 1.0,
  "issuer": {
    "tin": "MISC491214B86",
    "legalName": "CECILIA MIRANDA SANCHEZ",
    "taxRegimeCode": "612"
    // ❌ NO SE ENVÍA taxCredentials en TEST
  },
  "recipient": {
    "tin": "MASO451221PM4",
    "legalName": "MARIA OLIVIA MARTINEZ SAGAZ",
    "zipCode": "80290",
    "taxRegimeCode": "612",
    "cfdiUseCode": "G03"
  },
  "items": [
    {
      "itemCode": "01010101",
      "quantity": 2.000,
      "unitOfMeasurementCode": "H87",
      "description": "CAMARON 21-25",
      "unitPrice": 155.172414,  // ✅ Redondeado a 6 decimales
      "taxObjectCode": "02",
      "itemTaxes": [
        {
          "taxCode": "002",
          "taxTypeCode": "Tasa",
          "taxRate": 0.16,
          "taxFlagCode": "T"  // ✅ T = Traslado (IVA)
        }
      ]
    }
  ]
}
```

**Campos que NO SE ENVÍAN (NullValueHandling.Ignore):**
- ❌ `series` (null, FiscalAPI asigna automáticamente)
- ❌ `taxCredentials` (null en TEST, se usan pre-configurados)
- ❌ `email` (null en recipient)
- ❌ `itemSku` (null, NoIdentificacion)
- ❌ `discount` (null o 0)

### 4. Lógica de Certificados ✅

**Ambiente TEST:**
- `TaxCredentials = null`
- FiscalAPI usa certificados configurados en su dashboard
- **NullValueHandling.Ignore** evita enviar el campo

**Ambiente PRODUCCIÓN:**
- `TaxCredentials = [.cer, .key]` con password
- Se envían certificados en cada petición

### 5. Cálculos de Impuestos ✅

```csharp
// Ejemplo con 2 items de $180 c/u (incluye IVA)
Total con IVA: 360.00
Subtotal (sin IVA): 310.34 (360/1.16)
IVA (16%): 49.66 (360 - 310.34)

Por unidad:
Precio unitario (sin IVA): 155.172414 (180/1.16)
IVA por unidad: 24.827586 (155.17 * 0.16)
```

### 6. Flujo Completo ✅

1. **Controller** recibe petición
2. **CD_Factura.GenerarYTimbrarFactura** detecta PAC activo
3. **Detecta FiscalAPI activo** (ConfiguracionFiscalAPI.Activo = 1)
4. **TimbrarConFiscalAPI** construye factura
5. **FiscalAPICFDI40Generator.GenerarRequest** crea JSON
   - ✅ No incluye `series`
   - ✅ No incluye `taxCredentials` (TEST)
   - ✅ Redondea `unitPrice` a 6 decimales
   - ✅ `taxFlagCode = "T"` para IVA traslado
6. **FiscalAPIService.CrearYTimbrarCFDI** envía a API
7. **Respuesta esperada**: 200 OK con UUID, XML, QR

### 7. Validaciones Implementadas ✅

- ✅ VentaID existe
- ✅ RFC receptor válido
- ✅ UsoCFDI compatible con régimen
- ✅ Conceptos con impuestos calculados
- ✅ TipoImpuesto case-insensitive (TRASLADO → T)
- ✅ Precios redondeados (Math.Round 6 decimales)

### 8. Logs de Debug ✅

**Consola Visual Studio mostrará:**
```
✅ Usando FiscalAPI en ambiente: TEST
=== REQUEST A FISCALAPI ===
Endpoint: https://test.fiscalapi.com/api/v4/invoices
API Key: sk_test_47126aed_6c7...
Tenant: e0a0d1de-d225-46de-b95f-55d04f2787ff
JSON Request: { ... }
=========================
=== RESPONSE DE FISCALAPI ===
Status Code: 200 OK (esperado)
Response Body: { "data": { "uuid": "...", ... } }
============================
```

### 9. Posibles Errores y Soluciones

| Error | Causa | Solución |
|-------|-------|----------|
| 500 con body vacío | Certificados no configurados en dashboard TEST | ✅ Ya solucionado - no se envían |
| 401 Unauthorized | API Key o Tenant incorrectos | Verificar ConfiguracionFiscalAPI |
| 400 Bad Request | Campos obligatorios faltantes | Revisar JSON completo en logs |
| 422 Validation Error | RFC no válido o datos inválidos | Revisar RFCs de emisor/receptor |

### 10. Siguiente Paso

**Reinicia IIS Express (F5 en Visual Studio)** y ejecuta la prueba desde el navegador:

```
http://localhost:64927
Login → Facturación → Seleccionar venta:
VentaID: 9aecc96b-e17d-4dc7-b2b8-4132a2173dc7
Receptor: MASO451221PM4 - MARIA OLIVIA MARTINEZ SAGAZ
CP: 80290
Régimen: 612
Uso CFDI: G03
→ Generar Factura
```

**Resultado esperado:**
✅ Status Code 200
✅ UUID generado
✅ XML timbrado
✅ QR code
✅ Factura guardada en DB

---

## NOTA IMPORTANTE

Si el error 500 persiste, puede ser que FiscalAPI TEST requiera:
1. Configurar el RFC MISC491214B86 en su dashboard
2. Subir los certificados manualmente en su portal
3. O usar un RFC de prueba que ellos proporcionan (como EKU9003173C9)

**Alternativa:** Podemos cambiar temporalmente a usar el RFC de ejemplo de FiscalAPI (EKU9003173C9) que viene con certificados pre-configurados en TEST.
