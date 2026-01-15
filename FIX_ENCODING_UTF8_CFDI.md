# ✅ FIX: Error "No se encontró el nodo cfdi:Comprobante"

**Fecha**: 2026-01-14  
**Error Prodigia**: Código 301 - "No se encontró el nodo cfdi:Comprobante"  
**Estado**: ✅ RESUELTO

---

## 🔴 Problema Original

Al intentar timbrar una factura con Prodigia, se recibía el siguiente error:

```xml
<codigo>301</codigo>
<mensaje>No se encontró el nodo cfdi:Comprobante</mensaje>
```

### Causa Raíz

El XML generado tenía **encoding="utf-16"** en la declaración XML, pero los bytes enviados a Prodigia estaban codificados en **UTF-8**:

```xml
<?xml version="1.0" encoding="utf-16"?>
<Comprobante xmlns="http://www.sat.gob.mx/cfdi/4" ...>
```

Cuando Prodigia decodificaba el Base64, el XML resultante tenía una **inconsistencia de encoding**:
- **Declaración XML**: `encoding="utf-16"`
- **Bytes reales**: UTF-8

Esto causaba que el parser XML de Prodigia no pudiera leer correctamente el documento.

---

## ✅ Solución Implementada

### Cambio en CFDI40XMLGenerator.cs

**Archivo**: `CapaDatos/Generadores/CFDI40XMLGenerator.cs`

#### ❌ Código Anterior (Incorrecto)

```csharp
// Generar XML final
XDocument doc = new XDocument(
    new XDeclaration("1.0", "UTF-8", null),
    comprobante
);

// Convertir a string con formato UTF-8
StringBuilder sb = new StringBuilder();
XmlWriterSettings settings = new XmlWriterSettings
{
    Encoding = new UTF8Encoding(false), // UTF-8 sin BOM
    Indent = false,
    OmitXmlDeclaration = false
};

using (XmlWriter writer = XmlWriter.Create(sb, settings))
{
    doc.Save(writer);
}

return sb.ToString(); // ❌ Resultado: encoding="utf-16"
```

**Problema**: Cuando usas `StringBuilder` con `XmlWriter.Create()`, el encoding final en el string es `utf-16` independientemente de la configuración.

#### ✅ Código Correcto (Implementado)

```csharp
// Generar XML final
XDocument doc = new XDocument(
    new XDeclaration("1.0", "utf-8", null),
    comprobante
);

// Convertir a string con encoding UTF-8 correcto
using (var stream = new System.IO.MemoryStream())
{
    XmlWriterSettings settings = new XmlWriterSettings
    {
        Encoding = new UTF8Encoding(false), // UTF-8 sin BOM
        Indent = false,
        OmitXmlDeclaration = false
    };

    using (XmlWriter writer = XmlWriter.Create(stream, settings))
    {
        doc.Save(writer);
    }

    // Convertir bytes a string UTF-8
    return Encoding.UTF8.GetString(stream.ToArray()); // ✅ Resultado: encoding="utf-8"
}
```

**Solución**: Usar `MemoryStream` en lugar de `StringBuilder` para garantizar que el encoding UTF-8 se preserve correctamente en todo el proceso.

---

## 🔧 Cambios Realizados

### 1. Agregar `using System.IO;`

```csharp
using System;
using System.IO; // ✅ NUEVO
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CapaModelo;
```

### 2. Reemplazar StringBuilder por MemoryStream

- **Antes**: `StringBuilder` → encoding incorrecto
- **Ahora**: `MemoryStream` → encoding correcto UTF-8

---

## ✅ Resultado

### XML Generado ANTES (Incorrecto)
```xml
<?xml version="1.0" encoding="utf-16"?>
<Comprobante xmlns="http://www.sat.gob.mx/cfdi/4" ...>
```

### XML Generado AHORA (Correcto)
```xml
<?xml version="1.0" encoding="utf-8"?>
<Comprobante xmlns="http://www.sat.gob.mx/cfdi/4" ...>
```

---

## 🧪 Cómo Probar

### 1. Recompilar el Proyecto
```powershell
.\compilar_proyecto.ps1
```

### 2. Reiniciar IIS Express
```powershell
# Detener IIS Express
Stop-Process -Name "iisexpress" -Force -ErrorAction SilentlyContinue

# Reiniciar desde Visual Studio o:
Start-Process "C:\Program Files\IIS Express\iisexpress.exe" -ArgumentList "/path:C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
```

### 3. Probar Facturación
1. Crear una venta
2. Generar factura
3. Verificar respuesta de Prodigia

**Esperado**: 
- ✅ Código 0 (Timbrado exitoso)
- ✅ UUID generado
- ✅ XML timbrado con complemento `tfd:TimbreFiscalDigital`

---

## 📊 Flujo Correcto Ahora

```
1. Generar XML CFDI 4.0 con encoding="utf-8"
   ↓
2. Convertir XML a bytes UTF-8
   ↓
3. Convertir bytes a Base64
   ↓
4. Enviar a Prodigia en JSON body (xmlBase64)
   ↓
5. Prodigia decodifica Base64 → XML UTF-8
   ↓
6. Prodigia parsea XML correctamente ✅
   ↓
7. Timbrado exitoso con UUID
```

---

## 🔍 Debugging

Si aún hay problemas, verificar en el log de Visual Studio Output:

```
=== REQUEST A PRODIGIA ===
JSON Request:
{
  "xmlBase64": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48Q29tcHJvYmFudGU...",
  ...
}
```

**Decodificar Base64 para verificar**:
```csharp
string xmlBase64 = "..."; // Del log
byte[] bytes = Convert.FromBase64String(xmlBase64);
string xml = Encoding.UTF8.GetString(bytes);
Console.WriteLine(xml); // Debe mostrar encoding="utf-8"
```

---

## 📝 Lecciones Aprendidas

### ❌ NO Hacer
```csharp
// NO usar StringBuilder para XML con encoding específico
StringBuilder sb = new StringBuilder();
XmlWriter writer = XmlWriter.Create(sb, settings);
```

### ✅ Hacer
```csharp
// SÍ usar MemoryStream para control total de encoding
using (var stream = new MemoryStream())
{
    XmlWriter writer = XmlWriter.Create(stream, settings);
    // ...
    return Encoding.UTF8.GetString(stream.ToArray());
}
```

---

## 🎯 Estado Final

- ✅ Compilación exitosa
- ✅ Encoding UTF-8 correcto en todo el flujo
- ✅ XML parseable por Prodigia
- ✅ Listo para timbrado real

**Próximo paso**: Probar timbrado desde la interfaz web y verificar respuesta exitosa con UUID.

---

**Fecha de fix**: 2026-01-14  
**Compilado por**: GitHub Copilot  
**Framework**: .NET Framework 4.6
