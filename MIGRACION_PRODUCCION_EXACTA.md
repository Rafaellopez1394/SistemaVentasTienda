# 🔄 MIGRACIÓN A PRODUCCIÓN - CAMBIOS EXACTOS

## ✅ Buena Noticia

Tu código **YA ESTÁ CONFIGURADO DINÁMICAMENTE**. Solo necesitas actualizar estos 3 valores en la BD:

```
1. X-API-KEY:   (Header X-API-KEY)
2. X-TENANT-KEY: (Header X-TENANT-KEY)  
3. URL Base:     (De https://test.fiscalapi.com a https://live.fiscalapi.com)
```

---

## 📍 Dónde Están Estos Valores en el Código

### Configuración Dinámica
El código toma los valores de la **tabla ConfiguracionPAC** en BD:

```
ConfiguracionPAC
├─ ApiKey          (es el X-API-KEY)
├─ Tenant          (es el X-TENANT-KEY)
├─ EsProduccion    (0=TEST, 1=PRODUCCIÓN)
└─ UrlApi          (se deriva automáticamente de EsProduccion)
```

**Ubicaciones donde se usan:**

1. **FiscalAPIService.cs** (línea 38-39)
   ```csharp
   _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _configuracion.ApiKey);
   _httpClient.DefaultRequestHeaders.Add("X-TENANT-KEY", _configuracion.Tenant);
   ```

2. **ConfiguracionFiscalAPI.cs** (línea 32-37)
   ```csharp
   public string UrlApi
   {
       get
       {
           return Ambiente == "PRODUCCION" 
               ? "https://api.fiscalapi.com" 
               : "https://test.fiscalapi.com";
       }
   }
   ```

---

## 🔧 CAMBIOS NECESARIOS EN BD

### SQL Único para Migrar
Ejecuta este script en **SQL Server (DB_TIENDA)**:

```sql
UPDATE ConfiguracionPAC
SET
    ApiKey = 'sk_live_XXXXXXXXXXXXXXXX',        -- ← Tu X-API-KEY de producción
    Tenant = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxx',    -- ← Tu X-TENANT-KEY de producción
    EsProduccion = 1,                            -- ← Activa automáticamente la URL: https://live.fiscalapi.com
    FechaModificacion = GETDATE()
WHERE ConfigPACID = 1;
```

**Eso es TODO lo que necesitas cambiar.**

---

## 📋 Paso a Paso

### 1️⃣ Obtén tus Credenciales de Producción

Ve a FiscalAPI Dashboard Producción:
- **URL**: https://live.fiscalapi.com
- **Busca**: API Keys o Settings
- **Obtén**:
  - `X-API-KEY` (normalmente comienza con `sk_live_`)
  - `X-TENANT-KEY` (formato: UUID, ej: `12345678-1234-1234-1234-123456789012`)

### 2️⃣ Edita el Script SQL Arriba
Reemplaza:
- `sk_live_XXXXXXXXXXXXXXXX` → tu X-API-KEY real
- `xxxxxxxx-xxxx-xxxx-xxxx-xxxx` → tu X-TENANT-KEY real

### 3️⃣ Ejecuta en SQL Server Management Studio
```
1. Abre SQL Server Management Studio
2. Conecta a: DB_TIENDA
3. Abre Nueva Query
4. Pega el script editado
5. Presiona F5
```

### 4️⃣ Verifica el Cambio
```sql
SELECT ConfigPACID, ApiKey, Tenant, EsProduccion, UrlApi 
FROM ConfiguracionPAC 
WHERE ConfigPACID = 1;
```

**Debe mostrar:**
- `EsProduccion`: 1 (PRODUCCIÓN)
- `UrlApi`: https://api.fiscalapi.com (automático)
- `ApiKey`: sk_live_... (tu clave)
- `Tenant`: tu tenant ID

### 5️⃣ Recompila y Prueba
```
1. Visual Studio: Build → Rebuild Solution
2. F5 para ejecutar
3. Genera una factura de prueba
4. Verifica en https://live.fiscalapi.com/dashboard
```

---

## 🔍 Dónde Se Usa Tu Configuración

Después de actualizar la BD, el código **automáticamente** usa tus nuevas credenciales aquí:

| Archivo | Línea | Uso |
|---------|-------|-----|
| **FiscalAPIService.cs** | 32-39 | HttpClient con headers |
| **ConfiguracionFiscalAPI.cs** | 32-37 | URL correcta (live vs test) |
| **CertificadoDigitalController.cs** | 250 | URL para certificados |
| **FiscalAPIPersonas.cs** | 18-97 | URL para consultas SAT |
| **FiscalAPIPDF.cs** | 24 | URL para PDF |
| **FiscalAPIEmail.cs** | 23 | URL para email |

---

## ⚠️ Valores Actuales (TEST)

Estos son los valores que probablemente tienes ahora:

```sql
SELECT * FROM ConfiguracionPAC WHERE ConfigPACID = 1;

-- Resultado esperado (TEST):
ApiKey:        sk_test_47126aed_6c71_4060_b05b_932c4423dd00
Tenant:        e0a0d1de-d225-46de-b95f-55d04f2787ff
EsProduccion:  0
UrlApi:        https://test.fiscalapi.com
```

---

## ✅ Nuevos Valores (PRODUCCIÓN)

Después del cambio:

```sql
SELECT * FROM ConfiguracionPAC WHERE ConfigPACID = 1;

-- Resultado esperado (PRODUCCIÓN):
ApiKey:        sk_live_TUCLAVEREAL         ← NUEVO
Tenant:        TU-TENANT-ID-REAL           ← NUEVO
EsProduccion:  1
UrlApi:        https://api.fiscalapi.com   ← Automático
```

---

## 🎯 Resumen de Headers HTTP

**ANTES (TEST)**
```
GET https://test.fiscalapi.com/api/v4/invoices
Headers:
  X-API-KEY:    sk_test_47126aed_6c71_4060_b05b_932c4423dd00
  X-TENANT-KEY: e0a0d1de-d225-46de-b95f-55d04f2787ff
```

**DESPUÉS (PRODUCCIÓN)**
```
GET https://live.fiscalapi.com/api/v4/invoices
Headers:
  X-API-KEY:    sk_live_TUCLAVEREAL        ← CAMBIÓ
  X-TENANT-KEY: TU-TENANT-ID-REAL          ← CAMBIÓ
```

---

## 🚀 Script SQL Completo

```sql
USE DB_TIENDA;
GO

PRINT 'Migración de TEST a PRODUCCIÓN';
PRINT '==============================';
PRINT '';

-- Mostrar configuración ACTUAL
PRINT 'Configuración ANTES:';
SELECT 
    ConfigPACID,
    'X-API-KEY: ' + LEFT(ApiKey, 20) + '...' AS ApiKey,
    'X-TENANT-KEY: ' + Tenant AS Tenant,
    CASE WHEN EsProduccion = 0 THEN 'TEST' ELSE 'PRODUCCIÓN' END AS Ambiente
FROM ConfiguracionPAC
WHERE ConfigPACID = 1;

PRINT '';

-- Actualizar a PRODUCCIÓN
UPDATE ConfiguracionPAC
SET
    ApiKey = 'sk_live_XXXXXXXXXXXXXXXX',        -- ← REEMPLAZAR CON TU API KEY
    Tenant = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxx',    -- ← REEMPLAZAR CON TU TENANT
    EsProduccion = 1,
    FechaModificacion = GETDATE()
WHERE ConfigPACID = 1;

PRINT 'Configuración actualizada a PRODUCCIÓN';
PRINT '';

-- Mostrar configuración NUEVA
PRINT 'Configuración DESPUÉS:';
SELECT 
    ConfigPACID,
    'X-API-KEY: ' + LEFT(ApiKey, 20) + '...' AS ApiKey,
    'X-TENANT-KEY: ' + Tenant AS Tenant,
    CASE WHEN EsProduccion = 0 THEN 'TEST' ELSE 'PRODUCCIÓN' END AS Ambiente,
    'URL: https://api.fiscalapi.com' AS URL
FROM ConfiguracionPAC
WHERE ConfigPACID = 1;

PRINT '';
PRINT '✅ Migración completada';
PRINT 'Próximos pasos:';
PRINT '   1. Recompila la solución (Rebuild)';
PRINT '   2. Ejecuta la aplicación (F5)';
PRINT '   3. Genera una factura de prueba';

GO
```

---

## ❓ FAQ

### ¿Dónde consigo X-API-KEY?
FiscalAPI Dashboard → Configuración → API Keys (en ambiente de producción)

### ¿Dónde consigo X-TENANT-KEY?
FiscalAPI Dashboard → Configuración → Credenciales o Settings

### ¿El URL se cambia automáticamente?
**SÍ**. El código verifica `EsProduccion` y cambia la URL automáticamente:
- Si `EsProduccion = 0` → `https://test.fiscalapi.com`
- Si `EsProduccion = 1` → `https://api.fiscalapi.com`

### ¿Necesito cambiar código C#?
**NO**. El código ya es dinámico. Solo cambia los valores en BD.

### ¿Qué pasa si cometo un error?
Vuelve a ejecutar el script SQL con los valores correctos.

---

## ✨ ¿Qué Cambia en la Aplicación?

### Automáticamente
✅ URL de peticiones HTTP (test → live)
✅ Headers X-API-KEY en todas las peticiones
✅ Headers X-TENANT-KEY en todas las peticiones
✅ Certificados se obtienen de FiscalAPI producción
✅ Catálogos SAT se obtienen de producción
✅ Facturas se timbran en SAT real

### No necesitas cambiar
✅ Código C#
✅ Controladores
✅ Vistas
✅ Modelos
✅ Lógica de negocio

---

## 🎯 Checklist Final

- [ ] Obtuve X-API-KEY de FiscalAPI producción
- [ ] Obtuve X-TENANT-KEY de FiscalAPI producción
- [ ] Editué el script SQL con mis credenciales
- [ ] Ejecuté el script en SQL Server
- [ ] Verifiqué que EsProduccion = 1
- [ ] Recompilé la solución
- [ ] Ejecuté F5
- [ ] Generé una factura de prueba
- [ ] La factura aparece en FiscalAPI live
- [ ] La factura tiene status "Vigente"

---

**¡LISTO! Solo son 3 valores a cambiar en la BD.** 🚀
