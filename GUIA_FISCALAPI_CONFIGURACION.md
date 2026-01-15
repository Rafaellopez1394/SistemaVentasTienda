# 🔐 CONFIGURACIÓN FISCALAPI - GUÍA COMPLETA

## ✅ Estado Actual del Sistema

### Correcciones Implementadas:
- ✅ Headers HTTP corregidos (X-API-KEY, X-TENANT-KEY)
- ✅ C# 8.0 habilitado en todos los proyectos
- ✅ Compilación exitosa (0 errores)
- ✅ Soporte para UsoCFDI en requests
- ✅ Logging detallado para debugging
- ✅ Venta de prueba verificada en BD

### Venta de Prueba Disponible:
```
VentaID: 6bc16123-7b85-418e-a4aa-62384726aa44
Total: $1.00
Producto: CAMARON 131-150 (0.008 kg)
ClaveProdServ: 50121612
ClaveUnidad: KGM
```

---

## 📝 PASO 1: Obtener Credenciales de FiscalAPI

### Opción A: Ambiente de Pruebas (RECOMENDADO)

1. **Regístrate en FiscalAPI Test:**
   - Ve a: https://test.fiscalapi.com/register
   - Crea una cuenta gratuita

2. **Obtén tus credenciales:**
   - Inicia sesión en: https://test.fiscalapi.com
   - Ve a **Settings** → **API Keys**
   - Copia:
     - `API Key` (empieza con "sk_test_...")
     - `Tenant ID` (UUID único)

3. **Certificados de Prueba:**
   - RFC de prueba del SAT: `EKU9003173C9`
   - Descarga certificados: https://docs.fiscalapi.com/tax-files-info
   - Contraseña: `12345678a`

### Opción B: Ambiente de Producción

1. **Regístrate en FiscalAPI Live:**
   - Ve a: https://live.fiscalapi.com/register
   - Requiere plan de pago

2. **Usa tus certificados reales:**
   - RFC de tu empresa
   - Certificado CSD (.cer)
   - Llave privada CSD (.key)
   - Contraseña de la llave

---

## 🔧 PASO 2: Configurar Credenciales en la Base de Datos

Ejecuta este comando SQL reemplazando con tus credenciales:

```sql
UPDATE ConfiguracionPAC 
SET 
    ProveedorPAC = 'FiscalAPI',
    EsProduccion = 0,  -- 0 = Test, 1 = Producción
    UrlTimbrado = 'https://test.fiscalapi.com/api/v4/invoices/income',
    UrlCancelacion = 'https://test.fiscalapi.com/api/v4/invoices',
    UrlConsulta = 'https://test.fiscalapi.com/api/v4/invoices/status',
    Usuario = 'TU_API_KEY_AQUI',           -- Reemplazar
    Password = 'TU_TENANT_ID_AQUI',        -- Reemplazar
    Activo = 1
WHERE ConfigID = 3;
```

**Comando PowerShell:**
```powershell
$apiKey = "sk_test_xxxxxxxxxxxxx"  # Tu API Key
$tenant = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"  # Tu Tenant ID

sqlcmd -S . -d DB_TIENDA -E -Q "UPDATE ConfiguracionPAC SET Usuario = '$apiKey', Password = '$tenant' WHERE ConfigID = 3"
```

---

## 🧪 PASO 3: Subir Certificados a FiscalAPI

### Usando el Dashboard Web:

1. **Inicia sesión en FiscalAPI Test**
2. **Ve a "Issuers"** (Emisores)
3. **Crea un emisor de prueba:**
   - RFC: `EKU9003173C9`
   - Nombre: `ESCUELA KEMPER URGATE`
   - Régimen Fiscal: `601` (General de Ley Personas Morales)

4. **Sube los certificados:**
   - Ve a "Tax Files" (Archivos Fiscales)
   - Sube el certificado `.cer`
   - Sube la llave `.key`
   - Contraseña: `12345678a`

### Usando el SDK (Alternativo):

El código ya está listo en `CapaDatos/FiscalAPI/FiscalApiSDK.cs`. El método `TaxFileService.CreateAsync()` permite subir certificados programáticamente.

---

## 🚀 PASO 4: Probar Generación de Factura

### Usando PowerShell:

```powershell
$body = @{
    VentaID = "6bc16123-7b85-418e-a4aa-62384726aa44"
    ReceptorRFC = "XAXX010101000"
    ReceptorNombre = "PUBLICO EN GENERAL"
    ReceptorRegimenFiscal = "616"
    UsoCFDI = "S01"
    FormaPago = "01"
    MetodoPago = "PUE"
} | ConvertTo-Json

# Primero inicia sesión en el navegador en http://localhost:64927
# Luego desde el navegador, abre la consola de desarrollador y ejecuta:
```

### Usando la aplicación web:

1. **Inicia sesión:**
   - http://localhost:64927
   - Usuario: `admin@gmail.com`
   - Contraseña: `123`

2. **Ve al módulo de ventas**

3. **Selecciona la venta y genera factura**

---

## 🐛 PASO 5: Troubleshooting

### Error: "El XML generado está vacío"

**Causas comunes:**
1. La venta no existe en la BD
2. La venta no tiene productos (VentasDetalleClientes vacío)
3. Faltan campos obligatorios del emisor

**Solución:**
```sql
-- Verificar que la venta tiene productos
SELECT v.VentaID, COUNT(dv.ProductoID) as NumProductos
FROM VentasClientes v
LEFT JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
WHERE v.VentaID = '6bc16123-7b85-418e-a4aa-62384726aa44'
GROUP BY v.VentaID

-- Debe retornar NumProductos > 0
```

### Error: "Invalid API Key" o "Unauthorized"

**Solución:**
- Verifica que el API Key esté correcto
- Verifica que el Tenant ID sea correcto
- Asegúrate de estar usando el ambiente correcto (test vs live)

### Error: "Certificate not found"

**Solución:**
- Sube los certificados CSD a FiscalAPI
- Verifica que el RFC del emisor coincida con los certificados

---

## 📊 PASO 6: Verificar Logs

### En Visual Studio:
1. Abre la ventana **Output**
2. Selecciona "Debug" en el dropdown
3. Verás logs como:
   ```
   === CrearFacturaDesdeVenta ===
   VentaID: 6bc16123-7b85-418e-a4aa-62384726aa44
   === CFDI40XMLGenerator.GenerarXML ===
   Factura tiene 1 conceptos
   ```

### En SQL Server:
```sql
-- Ver facturas generadas
SELECT TOP 10 * FROM Facturas ORDER BY FechaEmision DESC

-- Ver errores
SELECT * FROM ErrorLog WHERE Modulo LIKE '%Factura%' ORDER BY FechaError DESC
```

---

## 📚 RECURSOS ADICIONALES

- **Documentación oficial:** https://docs.fiscalapi.com/
- **SDK oficial .NET:** https://github.com/FiscalAPI/fiscalapi-net
- **Postman Collection:** https://documenter.getpostman.com/view/4346593/2sB2j4eqXr
- **Certificados de prueba:** https://docs.fiscalapi.com/tax-files-info
- **Soporte:** https://discord.gg/zg6KZwgZ

---

## ✅ CHECKLIST DE IMPLEMENTACIÓN

- [x] Headers HTTP corregidos
- [x] C# 8.0 habilitado
- [x] Sistema compilado (0 errores)
- [x] Venta de prueba disponible
- [x] URLs de FiscalAPI configuradas
- [ ] API Key y Tenant configurados
- [ ] Certificados CSD subidos a FiscalAPI
- [ ] Emisor creado en FiscalAPI
- [ ] Prueba de facturación exitosa

---

## 🎯 PRÓXIMO PASO

**Acción inmediata:** Obtén tus credenciales de FiscalAPI (API Key y Tenant) de https://test.fiscalapi.com y actualiza la base de datos con el comando SQL del PASO 2.

Una vez tengas las credenciales, podemos probar la generación completa de facturas con timbrado real.
