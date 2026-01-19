# Guía: Obtener PDF Oficial Bonito de FiscalAPI

## ¿Por qué mi PDF no se ve bonito?

Tu sistema tiene dos formas de generar PDF:

### 1. PDF Oficial de FiscalAPI ✅ (BONITO)
- Descargado directamente de FiscalAPI
- Diseño profesional con colores
- Logo de FiscalAPI
- Formato oficial del SAT
- **REQUIERE:** `FiscalAPIInvoiceId` en la base de datos

### 2. PDF Local con iTextSharp ⚠️ (BÁSICO)
- Generado localmente con iTextSharp
- Diseño básico en blanco y negro
- Sin logo
- Fallback cuando no hay InvoiceId
- **SE USA:** Cuando `FiscalAPIInvoiceId` es NULL

## ¿Qué está pasando?

```sql
-- Tus facturas actuales:
SELECT Serie, Folio, UUID, FiscalAPIInvoiceId FROM Facturas

-- Resultado:
-- F    2    013416dd-b424-454d-89be-91a62f9a1da7    NULL ❌
-- F    1    NULL                                    NULL ❌
```

Como puedes ver, `FiscalAPIInvoiceId` está en NULL, por eso usas el PDF local (feo).

## Solución: Timbrar Nueva Factura

### Paso 1: Inicia la aplicación
```powershell
# En Visual Studio, presiona F5
# O desde PowerShell:
cd "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
.\VentasWeb\bin\VentasWeb.dll
```

### Paso 2: Inicia sesión
- Abre el navegador: http://localhost:64927
- Usuario: admin@gmail.com
- Contraseña: tu contraseña

### Paso 3: Timbrar nueva factura
1. Ve a **Facturación > Nueva Factura**
2. Selecciona o agrega un cliente
3. Agrega productos
4. Llena los datos fiscales:
   - RFC Receptor: XAXX010101000 (Público en General)
   - Uso CFDI: G03
   - Forma de Pago: 01 (Efectivo)
   - Método de Pago: PUE (Pago en una sola exhibición)
5. Click en **"Timbrar"**

### Paso 4: Descargar PDF Oficial
1. Una vez timbrada, verás la factura en la lista
2. Click en el botón **"Descargar PDF"** 📄
3. ¡El PDF será el BONITO de FiscalAPI! ✅

## Verificar que funcionó

### En Base de Datos:
```sql
-- Verificar última factura
SELECT TOP 1 
    Serie, 
    Folio, 
    UUID, 
    FiscalAPIInvoiceId,  -- Debe tener un GUID aquí ✅
    FechaCreacion
FROM Facturas
ORDER BY FechaCreacion DESC
```

### En el PDF:
- ✅ Debe tener colores (banda azul/cyan)
- ✅ Debe tener logo de FiscalAPI
- ✅ Diseño profesional
- ✅ Todos los datos fiscales completos
- ✅ QR code grande y visible
- ✅ Sellos digitales del SAT

## ¿Y las facturas antiguas?

Las facturas antiguas (sin InvoiceId) seguirán usando el PDF local. Tienes 2 opciones:

### Opción A: Dejarlas como están
- Las facturas antiguas usarán PDF local (básico)
- Las nuevas facturas usarán PDF oficial (bonito)
- Sin problema, ambos son válidos legalmente

### Opción B: Actualizar facturas antiguas
Si quieres que las facturas antiguas también tengan PDF bonito:

1. Ve al dashboard de FiscalAPI: https://test.fiscalapi.com
2. Busca tus facturas timbradas
3. Copia el `invoiceId` de cada una
4. Ejecuta este SQL:

```sql
-- Actualizar factura específica
UPDATE Facturas
SET FiscalAPIInvoiceId = 'COPIAR_INVOICE_ID_DE_FISCALAPI'
WHERE UUID = '013416dd-b424-454d-89be-91a62f9a1da7'

-- Verificar
SELECT Serie, Folio, UUID, FiscalAPIInvoiceId
FROM Facturas
WHERE UUID = '013416dd-b424-454d-89be-91a62f9a1da7'
```

## Script de Verificación Rápida

```powershell
# Ejecutar en PowerShell para ver el estado de tus facturas
sqlcmd -S localhost -d DB_TIENDA -E -Q @"
SELECT 
    Serie + '-' + Folio as Factura,
    CASE 
        WHEN FiscalAPIInvoiceId IS NOT NULL THEN 'PDF Oficial ✅'
        ELSE 'PDF Local ⚠️'
    END as TipoPDF,
    CONVERT(varchar, FechaCreacion, 120) as Fecha
FROM Facturas
WHERE UUID IS NOT NULL
ORDER BY FechaCreacion DESC
"@ -W
```

## Comparación Visual

### PDF Local (Actual - Básico):
```
┌────────────────────────────┐
│ FACTURA                    │
│                            │
│ Emisor: CECILIA MIRANDA    │
│ Receptor: Cliente X        │
│                            │
│ Producto | Cant | Precio   │
│ Item 1   | 2    | $100     │
│                            │
│ Total: $200                │
│                            │
│ [QR pequeño]               │
│ Sellos...                  │
└────────────────────────────┘
```
*Blanco y negro, básico*

### PDF Oficial (Nuevo - Bonito):
```
┌────────────────────────────┐
│ ╔══════════════════════╗   │ 
│ ║ [Logo] FACTURA       ║   │ <- Banda azul
│ ╚══════════════════════╝   │
│                            │
│ 📋 EMISOR                  │
│ CECILIA MIRANDA SANCHEZ    │
│ RFC: MISC491214B86         │
│                            │
│ 👤 RECEPTOR                │
│ Cliente X                  │
│ RFC: XAXX010101000         │
│                            │
│ 📦 CONCEPTOS               │
│ ┌──────────────────────┐   │
│ │ Item 1  │ 2 │ $100   │   │
│ └──────────────────────┘   │
│                            │
│ 💰 TOTALES                 │
│ Subtotal:      $200.00     │
│ IVA 16%:       $ 32.00     │
│ Total:         $232.00     │
│                            │
│ [QR Grande] [Sellos SAT]   │
│                            │
│ FiscalAPI.com              │
└────────────────────────────┘
```
*Con colores, profesional, completo*

## Resumen

1. **Las facturas NUEVAS** timbradas desde hoy = PDF Bonito ✅
2. **Las facturas ANTIGUAS** (antes de hoy) = PDF Básico ⚠️
3. Para probar: Timbra una nueva factura y descarga su PDF
4. El PDF nuevo se verá igual al que descargas de FiscalAPI

## ¿Necesitas ayuda?

Si después de timbrar una nueva factura el PDF sigue viéndose básico:

1. Verifica en la base de datos:
```sql
SELECT TOP 1 * FROM Facturas ORDER BY FechaCreacion DESC
```

2. El campo `FiscalAPIInvoiceId` debe tener un GUID (no NULL)

3. Si sigue siendo NULL, revisa los logs de la aplicación para ver si hubo algún error al guardar

---

**TL;DR:** Timbra una nueva factura desde la aplicación web. El PDF será automáticamente el bonito de FiscalAPI porque ahora sí se guarda el InvoiceId correctamente.
