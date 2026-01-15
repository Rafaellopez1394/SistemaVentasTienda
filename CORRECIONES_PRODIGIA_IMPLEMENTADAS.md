# ✅ Correcciones Implementadas - Integración PADE (Prodigia)

**Fecha**: 2026-01-14  
**Documentación Oficial**: https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest

---

## 🔴 Problemas Identificados y Corregidos

### 1. **Opciones como Query Parameters** ✅ CORREGIDO

**Problema Original**:
```csharp
// ❌ Incorrecto: opciones solo en el body JSON
string endpoint = $"servicio/rest/timbrado40/timbrarCfdi?contrato={contrato}";
```

**Solución Implementada**:
```csharp
// ✅ Correcto: opciones como query parameters
string queryParams = $"contrato={_configuracion.Contrato}";
queryParams += "&CALCULAR_SELLO";
queryParams += "&ESTABLECER_NO_CERTIFICADO";
queryParams += "&GENERAR_PDF";
queryParams += "&GENERAR_CBB";
queryParams += "&REGRESAR_CADENA_ORIGINAL";

string endpoint = $"servicio/rest/timbrado40/timbrarCfdi?{queryParams}";
```

**Referencia Documentación**:
> "También pueden ser especificadas como 'Query Parameters' las opciones de timbrado"

---

### 2. **Opciones Críticas Faltantes** ✅ AGREGADAS

#### Opciones Agregadas:

| Opción | Descripción | Cuándo se usa |
|--------|-------------|---------------|
| **CALCULAR_SELLO** | Prodigia calcula el sello del CFDI | Siempre recomendado |
| **ESTABLECER_NO_CERTIFICADO** | Coloca certificado y noCertificado automáticamente | Siempre recomendado |
| **CERT_DEFAULT** | Usa certificado subido al portal PADE | Cuando no se envían certificados en el request |
| **GENERAR_PDF** | Genera PDF del comprobante | Siempre (para entregar al cliente) |
| **GENERAR_CBB** | Genera código QR | Siempre (requerido por SAT) |
| **REGRESAR_CADENA_ORIGINAL** | Devuelve cadena original del timbre | Útil para auditorías |

**Código Implementado**:
```csharp
// En ProdigiaService.cs - CrearYTimbrarCFDI()
var opciones = new List<string>();

if (tienesCertificadosEnBD)
{
    opciones.Add("CALCULAR_SELLO");
    opciones.Add("ESTABLECER_NO_CERTIFICADO");
}
else
{
    opciones.Add("CERT_DEFAULT");
    opciones.Add("CALCULAR_SELLO");
    opciones.Add("ESTABLECER_NO_CERTIFICADO");
}

opciones.Add("GENERAR_PDF");
opciones.Add("GENERAR_CBB");
opciones.Add("REGRESAR_CADENA_ORIGINAL");
```

---

### 3. **Método de Cancelación** ✅ IMPLEMENTADO

**Problema**: No existía método para cancelar CFDIs

**Solución**: Implementado método `CancelarCFDI()` en [ProdigiaService.cs](CapaDatos/PAC/ProdigiaService.cs)

```csharp
public RespuestaCancelacion CancelarCFDI(
    string uuid, 
    string rfcEmisor, 
    string motivoCancelacion, 
    string uuidSustitucion = "")
{
    // Construir arregloUUID: UUID|Motivo|FolioSustitucion
    string arregloUuid = $"{uuid}|{motivoCancelacion}";
    if (!string.IsNullOrEmpty(uuidSustitucion))
        arregloUuid += $"|{uuidSustitucion}";

    // Endpoint: POST /servicio/rest/cancelacion/cancelarCfdi
    string queryParams = $"contrato={contrato}&rfcEmisor={rfcEmisor}&arregloUUID={arregloUuid}";
    
    // Agregar CERT_DEFAULT si no hay certificados en BD
    if (sin_certificados_en_bd)
        queryParams += "&CERT_DEFAULT";
    
    // ... resto de implementación
}
```

**Endpoint REST**:
```
POST https://pruebas.pade.mx/servicio/rest/cancelacion/cancelarCfdi?contrato=XXX&rfcEmisor=XXX&arregloUUID=UUID|Motivo|Sustitucion&CERT_DEFAULT
```

**Motivos de Cancelación (Catálogo SAT)**:
- `01` - Comprobante emitido con errores con relación
- `02` - Comprobante emitido con errores sin relación
- `03` - No se llevó a cabo la operación
- `04` - Operación nominativa relacionada en factura global

---

### 4. **Helper para Opciones** ✅ AGREGADO

Agregado método helper en [ProdigiaModels.cs](CapaDatos/PAC/ProdigiaModels.cs):

```csharp
public void AgregarOpcionesRecomendadas(bool usarCertDefault = true)
{
    Opciones.Add("CALCULAR_SELLO");
    Opciones.Add("ESTABLECER_NO_CERTIFICADO");
    Opciones.Add("GENERAR_PDF");
    
    if (usarCertDefault)
        Opciones.Add("CERT_DEFAULT");
}
```

---

## 📋 Estructura del Request Correcto

### **Timbrado CFDI**

**URL**:
```
POST https://pruebas.pade.mx/servicio/rest/timbrado40/timbrarCfdi?contrato=CONTRATO123&CALCULAR_SELLO&ESTABLECER_NO_CERTIFICADO&GENERAR_PDF&CERT_DEFAULT
```

**Headers**:
```
Authorization: Basic [usuario:password en Base64]
Content-Type: application/json
```

**Body** (sin certificados - usando CERT_DEFAULT):
```json
{
  "xmlBase64": "PD94bWwgdmVyc2lvbj0iMS4wIi...",
  "contrato": "CONTRATO123",
  "prueba": true,
  "opciones": [
    "CALCULAR_SELLO",
    "ESTABLECER_NO_CERTIFICADO",
    "GENERAR_PDF",
    "CERT_DEFAULT"
  ]
}
```

**Body** (con certificados en BD):
```json
{
  "xmlBase64": "PD94bWwgdmVyc2lvbj0iMS4wIi...",
  "contrato": "CONTRATO123",
  "certBase64": "MIIDtTCCAp2gAwIBAgIUMD...",
  "keyBase64": "MIIEvgIBADANBgkqhkiG9w...",
  "keyPass": "12345678a",
  "prueba": true,
  "opciones": [
    "CALCULAR_SELLO",
    "ESTABLECER_NO_CERTIFICADO",
    "GENERAR_PDF"
  ]
}
```

---

## 📌 Opciones Importantes Explicadas

### **CALCULAR_SELLO**
- Prodigia calcula el sello del CFDI automáticamente
- El atributo `Sello=""` del XML debe ir **vacío**
- Requiere certificado CSD en portal PADE o en el request

### **ESTABLECER_NO_CERTIFICADO**
- Coloca el `Certificado` y `NoCertificado` del CSD guardado en BD
- Si el XML ya tiene estos valores, los **reemplaza**
- Funciona con `CERT_DEFAULT`

### **CERT_DEFAULT**
- Usa el certificado CSD subido al portal PADE
- **Más seguro**: certificados no viajan en cada request
- **Más simple**: no necesitas enviar certBase64, keyBase64, keyPass
- El certificado debe estar previamente subido en: https://pruebas.pade.mx

### **GENERAR_PDF**
- Genera el PDF del CFDI timbrado
- Se regresa en el nodo `<pdfBase64>`
- Necesario para entregar al cliente

### **GENERAR_CBB**
- Genera el código QR del CFDI
- Se regresa en el nodo `<codigoBarrasBidimensional>`
- Requerido por el SAT en representación impresa

### **REGRESAR_CADENA_ORIGINAL**
- Devuelve la cadena original del timbre fiscal
- Se regresa en el nodo `<cadenaOriginalTFD>`
- Útil para validaciones y auditorías

---

## 🔄 Flujo de Timbrado Completo

```
1. Sistema genera XML pre-firmado (sin sello)
   ↓
2. Convierte XML a Base64
   ↓
3. Envía a Prodigia con opciones:
   - CALCULAR_SELLO
   - ESTABLECER_NO_CERTIFICADO
   - CERT_DEFAULT (si certificado está en portal)
   - GENERAR_PDF
   - GENERAR_CBB
   ↓
4. Prodigia:
   a) Obtiene certificado (de portal o request)
   b) Calcula sello del CFDI
   c) Envía al SAT para timbrado
   d) Genera PDF y QR
   ↓
5. Regresa respuesta con:
   - xmlBase64 (XML timbrado completo)
   - uuid (folio fiscal)
   - pdfBase64 (PDF generado)
   - codigoBarrasBidimensional (QR)
   - selloCFD, selloSAT, noCertificadoSAT
```

---

## ⚠️ Puntos Críticos

### 1. **Certificados CSD**
Tienes 2 opciones:

**Opción A: CERT_DEFAULT (Recomendado)**
- Subir certificado al portal: https://pruebas.pade.mx
- En cada timbrado solo enviar: `&CERT_DEFAULT`
- Más seguro, no viajan certificados

**Opción B: Enviar en cada request**
- Almacenar `CertificadoBase64` y `LlavePrivadaBase64` en BD
- Enviar en body: `certBase64`, `keyBase64`, `keyPass`
- Más pesado, pero funciona sin portal

### 2. **Atributo Sello en XML**
Si usas `CALCULAR_SELLO`:
```xml
<!-- ✅ Correcto: Sello vacío -->
<cfdi:Comprobante Sello="" ...>

<!-- ❌ Incorrecto: Sello con valor -->
<cfdi:Comprobante Sello="ABC123..." ...>
```

### 3. **Timeout**
- Prodigia recomienda **60 segundos**
- Código actual usa **120 segundos** ✅

### 4. **Autenticación**
```
Authorization: Basic [usuario:password en Base64]
```
**Ejemplo**:
- Usuario: `miusuario`
- Password: `mipassword`
- Base64: `bWl1c3VhcmlvOm1pcGFzc3dvcmQ=`
- Header: `Authorization: Basic bWl1c3VhcmlvOm1pcGFzc3dvcmQ=`

---

## 📁 Archivos Modificados

1. ✅ [CapaDatos/PAC/ProdigiaService.cs](CapaDatos/PAC/ProdigiaService.cs)
   - Opciones como query parameters
   - Opciones recomendadas agregadas
   - Método `CancelarCFDI()` implementado

2. ✅ [CapaDatos/PAC/ProdigiaModels.cs](CapaDatos/PAC/ProdigiaModels.cs)
   - Helper `AgregarOpcionesRecomendadas()`

---

## 🧪 Cómo Probar

### **1. Timbrado con CERT_DEFAULT**
```csharp
var config = new ConfiguracionProdigia
{
    Usuario = "usuario_pruebas",
    Password = "password_pruebas",
    Contrato = "CONTRATO123",
    Ambiente = "TEST",
    RfcEmisor = "AAA010101AAA",
    // NO incluir CertificadoBase64 ni LlavePrivadaBase64
    // Usar certificado subido en portal
};

var service = new ProdigiaService(config);
var respuesta = service.CrearYTimbrarCFDI(xmlSinSello);

if (respuesta.Exito)
{
    Console.WriteLine($"✅ UUID: {respuesta.UUID}");
    Console.WriteLine($"✅ PDF: {respuesta.PdfBase64?.Length} bytes");
}
```

### **2. Cancelación**
```csharp
var respuesta = service.CancelarCFDI(
    uuid: "12345678-1234-1234-1234-123456789012",
    rfcEmisor: "AAA010101AAA",
    motivoCancelacion: "02", // Sin relación
    uuidSustitucion: "" // Opcional
);

if (respuesta.Exitoso)
{
    Console.WriteLine($"✅ Cancelación: {respuesta.Mensaje}");
    // Código 201 = Solicitud recibida
    // Código 202 = Ya estaba cancelado
}
```

---

## 📚 Referencias

- **Documentación oficial**: https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest
- **Portal pruebas**: https://pruebas.pade.mx
- **Portal producción**: https://timbrado.pade.mx
- **Soporte**: soporte@pade.mx

---

## ✅ Checklist de Implementación

- [x] Opciones como query parameters
- [x] Opción CALCULAR_SELLO
- [x] Opción ESTABLECER_NO_CERTIFICADO
- [x] Opción CERT_DEFAULT
- [x] Opción GENERAR_PDF
- [x] Opción GENERAR_CBB
- [x] Opción REGRESAR_CADENA_ORIGINAL
- [x] Método CancelarCFDI()
- [x] Autenticación Basic Auth
- [x] Timeout 120 segundos
- [x] Parseo de respuesta XML
- [x] Manejo de errores
- [ ] Pruebas con credenciales reales
- [ ] Subir certificado a portal PADE
- [ ] Probar timbrado completo
- [ ] Probar cancelación

---

**Última actualización**: 2026-01-14  
**Estado**: ✅ Correcciones implementadas, listo para pruebas
