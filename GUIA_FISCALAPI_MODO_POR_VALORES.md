# Guía Completa: Implementación FiscalAPI SDK Modo "Por Valores"

## 📋 Resumen

Esta guía documenta la implementación completa de FiscalAPI SDK para ASP.NET en **Modo "Por Valores"**, siguiendo la documentación oficial de FiscalAPI.

## 🎯 Objetivo

Implementar facturación electrónica CFDI 4.0 usando FiscalAPI SDK, enviando los certificados CSD en cada petición (Modo "Por Valores"), sin requerir configuración previa en el dashboard de FiscalAPI.

## 📦 Instalación del SDK

El SDK ya está instalado vía NuGet:

```xml
<package id="Fiscalapi" version="4.0.270" targetFramework="net46" />
```

## 🔑 Dos Modos de Operación

### Modo Por Referencias
- Envía solo IDs de objetos creados en el dashboard de FiscalAPI
- Los certificados deben estar previamente cargados en https://test.fiscalapi.com/tax-files
- Ideal para integraciones ligeras

### Modo Por Valores ✅ (Implementado)
- Envía todos los campos requeridos en cada petición, incluyendo certificados
- **No requiere configuración previa** en el dashboard
- Mayor control sobre los datos
- Los certificados viajan en cada petición vía `TaxCredentials`

## 🏗️ Arquitectura Implementada

### 1. Base de Datos

#### Tabla ConfiguracionEmpresa - Nuevos Campos
```sql
ALTER TABLE ConfiguracionEmpresa
ADD CertificadoBase64 VARCHAR(MAX) NULL;

ALTER TABLE ConfiguracionEmpresa
ADD LlavePrivadaBase64 VARCHAR(MAX) NULL;

ALTER TABLE ConfiguracionEmpresa
ADD PasswordCertificado VARCHAR(100) NULL;
```

**Scripts:**
- `AGREGAR_COLUMNAS_CERTIFICADOS.sql` - Agrega las columnas
- `CONVERTIR_CERTIFICADOS_BASE64.ps1` - Convierte .cer y .key a Base64
- `CARGAR_CERTIFICADOS_BASE64.sql` - Carga certificados en BD

### 2. Modelo de Datos

#### CapaModelo/ConfiguracionEmpresa.cs
```csharp
// Certificados para FiscalAPI SDK (Modo Por Valores)
public string CertificadoBase64 { get; set; }
public string LlavePrivadaBase64 { get; set; }
public string PasswordCertificado { get; set; }
```

#### CapaModelo/Factura.cs
```csharp
// Certificados del Emisor para FiscalAPI (Modo Por Valores)
public string EmisorCertificadoBase64 { get; set; }
public string EmisorLlavePrivadaBase64 { get; set; }
public string EmisorPasswordCertificado { get; set; }
```

### 3. Capa de Datos

#### CapaDatos/CD_Factura.cs

**Cambio 1: Leer certificados de ConfiguracionEmpresa**
```csharp
string queryEmisor = @"SELECT RFC, RazonSocial, RegimenFiscal, CodigoPostal, 
                       CertificadoBase64, LlavePrivadaBase64, PasswordCertificado 
                       FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1";

// Leer certificados
certificadoBase64 = drEmisor["CertificadoBase64"]?.ToString();
llavePrivadaBase64 = drEmisor["LlavePrivadaBase64"]?.ToString();
passwordCertificado = drEmisor["PasswordCertificado"]?.ToString();
```

**Cambio 2: Asignar certificados al objeto Factura**
```csharp
factura = new Factura
{
    // ... otros campos ...
    EmisorCertificadoBase64 = certificadoBase64,
    EmisorLlavePrivadaBase64 = llavePrivadaBase64,
    EmisorPasswordCertificado = passwordCertificado,
};
```

**Cambio 3: Llamar al método correcto de timbrado**
```csharp
// Si es FiscalAPI y hay certificados, usar modo "Por Valores"
if (proveedorPAC is FiscalAPIPAC fiscalApiPac && 
    !string.IsNullOrWhiteSpace(factura.EmisorCertificadoBase64))
{
    respuestaTimbrado = await fiscalApiPac.TimbrarConCertificadosAsync(
        xmlSinTimbrar, 
        config,
        factura.EmisorCertificadoBase64,
        factura.EmisorLlavePrivadaBase64,
        factura.EmisorPasswordCertificado
    );
}
```

### 4. Proveedor PAC

#### CapaDatos/PAC/FiscalAPIPAC.cs

**Método Principal:**
```csharp
public async Task<RespuestaTimbrado> TimbrarConCertificadosAsync(
    string xmlSinTimbrar, 
    ConfiguracionPAC config, 
    string certificadoBase64, 
    string llavePrivadaBase64, 
    string passwordCertificado)
{
    var client = CrearCliente(config);
    
    // Parsear XML e incluir certificados
    var invoice = ParsearXMLAInvoice(
        xmlSinTimbrar, 
        certificadoBase64, 
        llavePrivadaBase64, 
        passwordCertificado
    );
    
    // Enviar a timbrar
    var apiResponse = await client.Invoices.CreateAsync(invoice);
    
    // ... procesar respuesta ...
}
```

**Parseo de XML con TaxCredentials:**
```csharp
private Invoice ParsearXMLAInvoice(
    string xml, 
    string certificadoBase64 = null, 
    string llavePrivadaBase64 = null, 
    string passwordCertificado = null)
{
    // ... parsear XML ...
    
    invoice.Issuer = new InvoiceIssuer
    {
        Tin = emisorRFC,
        LegalName = emisorNombre,
        TaxRegimeCode = emisorRegimen
    };
    
    // MODO POR VALORES: Incluir certificados CSD
    if (!string.IsNullOrWhiteSpace(certificadoBase64))
    {
        invoice.Issuer.TaxCredentials = new List<TaxCredential>
        {
            new TaxCredential
            {
                Base64File = certificadoBase64,
                FileType = FileType.CertificateCsd,
                Password = passwordCertificado
            },
            new TaxCredential
            {
                Base64File = llavePrivadaBase64,
                FileType = FileType.PrivateKeyCsd,
                Password = passwordCertificado
            }
        };
    }
}
```

## 📝 Proceso de Configuración

### Paso 1: Descargar Certificados de Prueba

```
URL: https://test.fiscalapi.com/files/tax-files/tax-files.zip
Contenido:
  - EKU9003173C9.cer (Certificado)
  - EKU9003173C9.key (Llave privada)
  - Password: 12345678a
```

### Paso 2: Convertir a Base64

```powershell
.\CONVERTIR_CERTIFICADOS_BASE64.ps1
```

Esto genera:
- Valores Base64 en consola
- Archivo `certificados_base64_output.txt` con los valores

### Paso 3: Cargar en Base de Datos

1. Abrir `CARGAR_CERTIFICADOS_BASE64.sql`
2. Pegar los valores Base64 del paso anterior
3. Ejecutar el script:

```sql
sqlcmd -S "SISTEMAS\SERVIDOR" -d DB_TIENDA -E -i CARGAR_CERTIFICADOS_BASE64.sql
```

### Paso 4: Verificar Configuración

```sql
SELECT 
    RFC,
    RazonSocial,
    LEN(CertificadoBase64) AS CertificadoLength,
    LEN(LlavePrivadaBase64) AS LlavePrivadaLength,
    PasswordCertificado
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;
```

Resultado esperado:
- CertificadoLength: ~1000-2000 caracteres
- LlavePrivadaLength: ~1500-3000 caracteres
- PasswordCertificado: '12345678a'

## 🧪 Probar Facturación

### Request de Prueba

```http
POST http://localhost:64927/Factura/GenerarFactura
Content-Type: application/json

{
    "VentaID": "6bc16123-7b85-418e-a4aa-62384726aa44",
    "ReceptorRFC": "XAXX010101000",
    "ReceptorNombre": "Público en General",
    "ReceptorRegimenFiscal": "616",
    "UsoCFDI": "S01",
    "FormaPago": "01",
    "MetodoPago": "PUE"
}
```

### Respuesta Exitosa

```json
{
    "estado": true,
    "valor": "Factura generada y timbrada exitosamente",
    "objeto": {
        "FacturaID": "...",
        "UUID": "...",
        "XMLTimbrado": "...",
        "RutaPDF": "..."
    }
}
```

## 🔍 Debugging

### Logs en Visual Studio Output

```csharp
System.Diagnostics.Debug.WriteLine("FiscalAPI: TaxCredentials incluidos para RFC ...");
System.Diagnostics.Debug.WriteLine("Timbrando con FiscalAPI en modo 'Por Valores'");
```

### Validar XML Generado

El XML debe contener:
```xml
<cfdi:Emisor Rfc="GAMA6111156JA" Nombre="ALMA ROSA GAXIOLA MONTOYA" RegimenFiscal="612"/>
```

### Errores Comunes

#### "PersonId must not be empty"
❌ **Causa:** Intentando usar Modo "Por Referencias" sin ID de persona
✅ **Solución:** Usar Modo "Por Valores" con TaxCredentials

#### "El XML generado está vacío"
❌ **Causa:** Conceptos no se están leyendo de la BD
✅ **Solución:** Verificado y corregido en CD_Factura.cs líneas 440-510

#### "Ya hay un DataReader abierto"
❌ **Causa:** Múltiples DataReaders en la misma conexión
✅ **Solución:** Separar queries en diferentes comandos

## 📊 Configuración de Base de Datos

### ConfiguracionPAC
```sql
ProveedorPAC: FiscalAPI
Usuario: sk_test_16b2fc7c_460a_4ba0_867f_b53cad3266f9
Password: e0a0d1de-d225-46de-b95f-55d04f2787ff
EsProduccion: 0
```

### ConfiguracionEmpresa
```sql
RFC: GAMA6111156JA
RazonSocial: ALMA ROSA GAXIOLA MONTOYA
RegimenFiscal: 612
CodigoPostal: 81048
CertificadoBase64: (Base64 del .cer)
LlavePrivadaBase64: (Base64 del .key)
PasswordCertificado: 12345678a
```

## ✅ Ventajas del Modo "Por Valores"

1. ✅ No requiere configuración previa en dashboard FiscalAPI
2. ✅ Mayor control sobre los datos
3. ✅ Certificados actualizados automáticamente
4. ✅ Migración entre ambientes más sencilla
5. ✅ No depende de IDs de FiscalAPI

## 📚 Referencias

- [FiscalAPI SDK GitHub](https://github.com/fiscalapi/fiscalapi-net)
- [Documentación Oficial](https://docs.fiscalapi.com)
- [Portal FiscalAPI Test](https://test.fiscalapi.com)
- [Ejemplos ASP.NET](https://github.com/fiscalapi/fiscalapi-net/tree/main/samples-asp-net)
- [Certificados de Prueba](https://test.fiscalapi.com/files/tax-files/tax-files.zip)

## 🎯 Estado Actual

✅ SDK instalado (Fiscalapi 4.0.270)
✅ Columnas agregadas a ConfiguracionEmpresa
✅ Modelos actualizados (ConfiguracionEmpresa, Factura)
✅ CD_Factura actualizado para leer y pasar certificados
✅ FiscalAPIPAC implementa Modo "Por Valores" con TaxCredentials
✅ Scripts de conversión y carga creados
✅ Compilación exitosa

⏳ Pendiente: Cargar certificados reales en BD y probar facturación

## 📝 Próximos Pasos

1. Ejecutar `CONVERTIR_CERTIFICADOS_BASE64.ps1` con tus certificados reales
2. Actualizar `CARGAR_CERTIFICADOS_BASE64.sql` con los valores Base64
3. Ejecutar el script SQL
4. Reiniciar aplicación web (IIS)
5. Probar generación de factura con POST a `/Factura/GenerarFactura`
6. Verificar UUID y XML timbrado
