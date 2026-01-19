# Integración de PDF Oficial de FiscalAPI

## Resumen

Se ha integrado el servicio oficial de generación de PDF de FiscalAPI para reemplazar el generador personalizado con iTextSharp. Esto garantiza PDFs siempre actualizados y 100% conformes con las especificaciones del SAT.

## ¿Por qué usar el PDF de FiscalAPI?

1. **Oficial y actualizado**: FiscalAPI mantiene su generador de PDF actualizado con las últimas normas del SAT
2. **Personalizable**: Permite configurar colores (banda y fuente) y agregar logo de la empresa
3. **Sin mantenimiento**: No hay que preocuparse por mantener un generador propio
4. **Completo**: Incluye todos los elementos requeridos (QR, sellos digitales, cadena original, etc.)

## Cambios Implementados

### 1. Base de Datos

**Script:** `AGREGAR_COLUMNA_FISCALAPI_INVOICEID.sql`

```sql
ALTER TABLE Facturas
ADD FiscalAPIInvoiceId NVARCHAR(100) NULL
```

Esta columna almacena el ID de la factura en FiscalAPI (diferente del UUID). Es necesario para poder descargar el PDF oficial.

### 2. Modelo de Datos

**Archivo:** `CapaModelo\Factura.cs`

- Agregada propiedad `FiscalAPIInvoiceId` a la clase `Factura`
- Agregada propiedad `InvoiceId` a la clase `RespuestaTimbrado`

### 3. Servicio de FiscalAPI

**Archivo:** `CapaDatos\PAC\FiscalAPIService.cs`

Agregado método `DescargarPDF`:

```csharp
public async Task<byte[]> DescargarPDF(
    string invoiceId, 
    string bandColor = "#0891b2",    // Color de la banda (cyan)
    string fontColor = "#FFFFFF",    // Color del texto en banda (blanco)
    string base64Logo = null         // Logo en base64 (opcional)
)
```

**Endpoint:** `POST /api/v4/invoices/pdf`

**Headers requeridos:**
- `X-API-KEY`: Tu clave API de FiscalAPI
- `X-TENANT-KEY`: Tu tenant de FiscalAPI
- `X-TIME-ZONE`: Zona horaria (America/Mexico_City)

### 4. Guardado del InvoiceId

**Archivo:** `CapaDatos\CD_Factura.cs`

#### Método `CrearYTimbrarCFDI` (línea ~106)
- Actualizada instrucción INSERT para incluir `FiscalAPIInvoiceId`
- Agregado parámetro `@FiscalAPIInvoiceId`

#### Método `TimbrarConFiscalAPI` (línea ~1467)
- Extrae el `InvoiceId` de la respuesta de FiscalAPI
- Lo asigna a la propiedad `factura.FiscalAPIInvoiceId`

#### Método `CrearYTimbrarCFDI` en FiscalAPIService (línea ~108)
- Extrae `InvoiceId` del response y lo guarda en `respuesta.InvoiceId`

### 5. Controlador Web

**Archivo:** `VentasWeb\Controllers\FacturaController.cs`

#### Método `DescargarPDF` (línea ~483)
```csharp
public async Task<ActionResult> DescargarPDF(Guid facturaId)
{
    var factura = CD_Factura.Instancia.ObtenerPorId(facturaId);
    
    byte[] pdfBytes;
    
    // Si tiene InvoiceId de FiscalAPI, descargar PDF oficial
    if (!string.IsNullOrEmpty(factura.FiscalAPIInvoiceId))
    {
        var config = CD_Factura.Instancia.ObtenerConfiguracionFiscalAPI();
        using (var fiscalService = new FiscalAPIService(config))
        {
            pdfBytes = await fiscalService.DescargarPDF(factura.FiscalAPIInvoiceId);
        }
    }
    else
    {
        // Fallback: usar generador local si no tiene InvoiceId
        var generador = new PDFFacturaGenerator();
        pdfBytes = generador.GenerarPDF(factura);
    }
    
    return File(pdfBytes, "application/pdf", nombreArchivo);
}
```

#### Método `EnviarPorEmail` (línea ~662)
- Actualizado para usar el mismo lógica (FiscalAPI primero, fallback a generador local)
- Cambiado a `async Task<JsonResult>` para soportar llamadas asíncronas

## Flujo Completo

1. **Timbrado:**
   - Se llama a `TimbrarConFiscalAPI`
   - FiscalAPI devuelve: UUID, XML timbrado, sellos, **y el InvoiceId**
   - Se guarda todo en la base de datos, incluyendo `FiscalAPIInvoiceId`

2. **Descarga de PDF:**
   - Usuario hace clic en "Descargar PDF" o "Enviar por Email"
   - El sistema verifica si `factura.FiscalAPIInvoiceId` tiene valor
   - Si tiene valor: llama a FiscalAPI para obtener el PDF oficial
   - Si no tiene valor: usa el generador local (iTextSharp) como fallback

3. **Resultado:**
   - PDF descargado directamente desde FiscalAPI
   - PDF completamente conforme con especificaciones SAT
   - Incluye QR code, sellos digitales, cadena original, etc.

## Personalización del PDF

Para personalizar el PDF, modifica la llamada en `DescargarPDF`:

```csharp
pdfBytes = await fiscalService.DescargarPDF(
    factura.FiscalAPIInvoiceId,
    bandColor: "#FF5733",  // Color personalizado (rojo)
    fontColor: "#FFFFFF",   // Texto blanco
    base64Logo: logoBase64  // Logo de tu empresa en base64
);
```

### Cómo agregar logo

1. Convierte tu logo a base64:
```csharp
byte[] logoBytes = File.ReadAllBytes("logo.png");
string logoBase64 = Convert.ToBase64String(logoBytes);
```

2. Guarda el logo en la configuración o pásalo directamente al método

## Instalación

### 1. Ejecutar script SQL

```bash
# Desde SQL Server Management Studio o sqlcmd
sqlcmd -S localhost -d DB_TIENDA -i AGREGAR_COLUMNA_FISCALAPI_INVOICEID.sql
```

O ejecutar manualmente en SSMS:
```sql
USE DB_TIENDA
GO

ALTER TABLE Facturas
ADD FiscalAPIInvoiceId NVARCHAR(100) NULL
GO
```

### 2. Compilar el proyecto

```powershell
# Compilar desde Visual Studio
# O desde línea de comandos:
msbuild SistemaVentasTienda.sln /p:Configuration=Release
```

### 3. Reiniciar IIS Express

```powershell
# Detener IIS Express
taskkill /F /IM iisexpress.exe

# Reiniciar desde Visual Studio (F5)
```

## Ventajas vs Generador Personalizado

| Aspecto | Generador Personalizado | PDF FiscalAPI |
|---------|------------------------|---------------|
| Mantenimiento | Alto - hay que mantener código | Ninguno |
| Actualización | Manual cuando cambian normas SAT | Automática |
| Conformidad SAT | Depende de implementación | 100% garantizada |
| Personalización | Total control | Colores y logo |
| Dependencias | iTextSharp, QRCoder | Ninguna |
| Rendimiento | Local, rápido | Red, depende de conexión |

## Notas Importantes

1. **Facturas anteriores**: Las facturas generadas antes de esta actualización no tendrán `FiscalAPIInvoiceId`, por lo que usarán el generador local como fallback.

2. **Sin conexión**: Si FiscalAPI no está disponible, el sistema fallará. Considera agregar un timeout y usar el generador local como backup.

3. **Rate limits**: FiscalAPI puede tener límites de tasa en su API. Considera implementar caché local del PDF si es necesario.

4. **Modo TEST vs PRODUCCIÓN**: El endpoint cambia según el ambiente:
   - TEST: `https://test.fiscalapi.com`
   - PRODUCCIÓN: `https://fiscalapi.com`

## Solución de Problemas

### Error: "No se encontró el InvoiceId"

**Causa**: La factura fue timbrada antes de implementar esta actualización.

**Solución**: El sistema usará automáticamente el generador local como fallback.

### Error 401 al descargar PDF

**Causa**: API Key o Tenant Key incorrectos.

**Solución**: Verifica la configuración en la tabla `ConfiguracionFiscalAPI`.

### PDF no se descarga

**Causa**: El `InvoiceId` puede ser inválido o la factura no existe en FiscalAPI.

**Solución**: Verifica el valor de `FiscalAPIInvoiceId` en la base de datos y consulta en el dashboard de FiscalAPI.

## Testing

### 1. Timbrar una nueva factura
```
1. Ir a Facturación > Nueva Factura
2. Llenar datos y timbrar
3. Verificar que se guardó correctamente
```

### 2. Verificar InvoiceId en BD
```sql
SELECT FacturaID, Serie, Folio, UUID, FiscalAPIInvoiceId
FROM Facturas
WHERE FacturaID = 'TU_FACTURA_ID'
```

### 3. Descargar PDF
```
1. Ir a lista de facturas
2. Click en "Descargar PDF"
3. Verificar que se descarga correctamente
4. Abrir PDF y verificar formato oficial
```

### 4. Enviar por email
```
1. Click en "Enviar por Email"
2. Ingresar email de prueba
3. Verificar que llega con PDF y XML adjuntos
```

## Próximos Pasos (Opcional)

1. **Agregar logo de empresa**: Implementar configuración para cargar logo y enviarlo a FiscalAPI
2. **Personalizar colores**: Agregar configuración en tabla para colores personalizados por empresa
3. **Caché local**: Guardar PDF en base de datos para reducir llamadas a FiscalAPI
4. **Modo offline**: Mejorar fallback al generador local cuando FiscalAPI no esté disponible

## Referencias

- [Documentación FiscalAPI - Generar PDF](https://docs.fiscalapi.com/api-reference/invoices/generate-pdf)
- [Curl ejemplo del usuario](Ver conversación)
