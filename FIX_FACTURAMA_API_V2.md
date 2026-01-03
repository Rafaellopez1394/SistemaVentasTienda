# 🔧 FIX: Facturama API v2 - Timbrado Correcto

## Problema Identificado
El error "error al timbrar" se debía a que estábamos usando el endpoint incorrecto de Facturama:
- ❌ **Antes**: `POST /cfdi` con XML directo (text/xml)
- ✅ **Ahora**: `POST /2/cfdis` con JSON conteniendo XML en Base64

## Cambios Realizados

### 1. FacturamaPAC.cs - Endpoint Actualizado

**Línea 36** - Endpoint corregido:
```csharp
string endpoint = $"{urlBase}/2/cfdis";  // Antes: /cfdi
```

**Líneas 47-52** - Formato JSON con XML Base64:
```csharp
// Facturama API v2: Enviar XML en Base64 dentro de JSON
var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlSinTimbrar));
var jsonPayload = new JObject
{
    ["Xml"] = xmlBase64
};
```

**Línea 54** - Content-Type correcto:
```csharp
var content = new StringContent(jsonPayload.ToString(), Encoding.UTF8, "application/json");
```

## URLs de Facturama API v2

### Sandbox (Pruebas)
```
Base URL: https://apisandbox.facturama.mx
Endpoint: POST /2/cfdis
Credenciales: pruebas / pruebas2011
```

### Producción
```
Base URL: https://api.facturama.mx
Endpoint: POST /2/cfdis
Credenciales: Tu usuario / Tu contraseña
```

## Formato de Petición

### Request
```http
POST https://apisandbox.facturama.mx/2/cfdis
Authorization: Basic cHJ1ZWJhczpwcnVlYmFzMjAxMQ==
Content-Type: application/json

{
  "Xml": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48Y2ZkaT..."
}
```

### Response (200 OK)
```json
{
  "Id": "dGVzdC1pZA==",
  "Content": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48Y2ZkaT...",
  "Complement": {
    "TaxStamp": {
      "Uuid": "12345678-1234-1234-1234-123456789012",
      "Date": "2024-01-15T10:30:00",
      "SatCertNumber": "00001000000123456789",
      "SatSeal": "abc123...",
      "CfdSeal": "def456..."
    }
  }
}
```

## Archivos Actualizados

1. **CapaDatos/PAC/FacturamaPAC.cs**
   - Endpoint: `/2/cfdis` (antes `/cfdi`)
   - Payload: JSON con XML Base64
   - Content-Type: `application/json` (antes `text/xml`)
   - Variable renombrada: `xmlTimbradoBase64` (evitar duplicado)

2. **VentasWeb/bin/CapaDatos.dll** ✅ Copiado
3. **VentasWeb/bin/CapaModelo.dll** ✅ Copiado

## Prueba del Fix

Para probar que ahora funciona correctamente:

1. **Reiniciar IIS Express** (si está corriendo)
2. **Hacer una venta** con RequiereFactura=true
3. **Generar factura** desde el módulo de facturación
4. **Verificar resultado**: Debe mostrar UUID del SAT

### SQL de Verificación
```sql
-- Ver última factura generada
SELECT TOP 1 
    FacturaID,
    Folio,
    UUID,
    Estado,
    FechaTimbrado,
    VentaID
FROM Facturas 
ORDER BY FechaEmision DESC

-- Ver detalles del timbrado
SELECT 
    FacturaID,
    UUID,
    SelloSAT,
    SelloCFD,
    NoCertificadoSAT,
    Estado
FROM Facturas 
WHERE UUID IS NOT NULL
ORDER BY FechaTimbrado DESC
```

## Documentación de Referencia

- [Facturama API v2 - Crear CFDI](https://api.facturama.mx/docs/api/POST-2-cfdis)
- [Facturama - Guía de Integración](https://api.facturama.mx/guias/)

## Próximos Pasos

1. ✅ Fix aplicado y compilado
2. ⏳ Probar timbrado en sandbox
3. ⏳ Verificar UUID en base de datos
4. ⏳ Configurar RFC emisor real
5. ⏳ Obtener certificados de producción
6. ⏳ Cambiar a modo producción (EsProduccion=1)

---
**Fecha de actualización**: 2024-01-15  
**Versión**: 1.1  
**Estado**: ✅ CORREGIDO Y DESPLEGADO
