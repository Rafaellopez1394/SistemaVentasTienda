# ⚡ CONFIGURACIÓN RÁPIDA - TIMBRADO NÓMINA

## 📋 CHECKLIST - ANTES DE LA PRIMERA PRUEBA

### ✅ PASO 1: Compilación (YA COMPLETADO)

- [x] Código implementado (1,300+ líneas)
- [x] 0 errores de compilación
- [x] Todos los archivos modificados

**Status:** ✅ LISTO

---

### ✅ PASO 2: Credenciales FiscalAPI (YA CONFIGURADAS)

**¡Excelente noticia!** Las credenciales de FiscalAPI **ya están configuradas** en tu sistema.

El módulo de nómina ahora **reutiliza la misma configuración** que tu facturación de ventas, almacenada en la tabla `ConfiguracionFiscalAPI` de la base de datos.

**No necesitas hacer nada aquí.** ✅

**Datos que se obtienen automáticamente:**
- ✅ API Key
- ✅ Tenant
- ✅ Ambiente (TEST/PRODUCTION)
- ✅ Certificados SAT (CER + KEY)
- ✅ Password del certificado
- ✅ RFC Emisor

---

### ⏳ PASO 3: Base de Datos (PENDIENTE - 3 minutos)

#### 3.1 Verificar tabla NominasCFDI

Abrir SQL Server Management Studio y ejecutar:

```sql
-- Ver estructura de la tabla
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'NominasCFDI'
ORDER BY ORDINAL_POSITION
```

#### 3.2 Agregar columna InvoiceId (si no existe)

```sql
-- Solo si la columna InvoiceId no existe:
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'NominasCFDI' AND COLUMN_NAME = 'InvoiceId'
)
BEGIN
    ALTER TABLE NominasCFDI
    ADD InvoiceId VARCHAR(100) NULL
    
    PRINT '✓ Columna InvoiceId agregada'
END
ELSE
BEGIN
    PRINT '✓ Columna InvoiceId ya existe'
END
```

#### 3.3 Verificar datos de empleados (CRÍTICO)

```sql
-- Ver empleados con datos incompletos
SELECT 
    EmpleadoID,
    NombreCompleto,
    RFC,
    CURP,
    NSS,
    CASE 
        WHEN RFC IS NULL OR RFC = '' THEN '❌ RFC faltante'
        WHEN CURP IS NULL OR CURP = '' THEN '⚠️ CURP faltante (usar genérico)'
        WHEN NSS IS NULL OR NSS = '' THEN '⚠️ NSS faltante (usar genérico)'
        ELSE '✓ OK'
    END AS Estado
FROM Empleados
WHERE Activo = 1
```

**Si hay empleados con datos faltantes:**

```sql
-- Actualizar con valores genéricos de prueba
UPDATE Empleados
SET 
    CURP = ISNULL(CURP, 'XEXX010101HNEXXXA4'),  -- CURP genérico SAT
    NSS = ISNULL(NSS, '00000000000'),           -- NSS genérico
    RFC = CASE 
        WHEN RFC IS NULL OR RFC = '' 
        THEN 'XEXX010101000'                    -- RFC genérico
        ELSE RFC 
    END
WHERE Activo = 1
  AND (CURP IS NULL OR CURP = '' OR NSS IS NULL OR NSS = '' OR RFC IS NULL OR RFC = '')
```

---

### ⏳ PASO 4: Primera Prueba (PENDIENTE - 3 minutos)

#### 4.1 Compilar proyecto

```
Visual Studio:
1. Abrir solución
2. Build > Build Solution (Ctrl+Shift+B)
3. Verificar: 0 errores en Output
```

#### 4.2 Ejecutar aplicación

```
F5 o Debug > Start Debugging
```

#### 4.3 Crear nómina de prueba

1. Ir a: `http://localhost:puerto/Nomina/Calcular`
2. Configurar:
   - Fecha inicio: `01/05/2024`
   - Fecha fin: `15/05/2024`
   - Fecha pago: `20/05/2024`
   - Tipo: `ORDINARIA`
3. Click "Procesar Nómina"
4. Esperar confirmación

#### 4.4 Timbrar primer recibo

1. En lista de nóminas, click "Ver Detalle"
2. Seleccionar un empleado
3. Click "Ver Recibo"
4. Verificar datos del recibo
5. Click **"Timbrar CFDI"**
6. Confirmar en el modal
7. **Esperar 5-10 segundos** ⏳
8. Debe aparecer:
   ```
   ¡Timbrado Exitoso!
   UUID: 12345678-1234-1234-1234-123456789012
   Fecha: 15/05/2024 10:30:45
   ```

#### 4.5 Descargar archivos

1. **XML:**
   - Click "Descargar XML"
   - Abrir archivo con editor de texto
   - Buscar `UUID` dentro del XML
   - Verificar estructura CFDI 4.0

2. **PDF:**
   - Click "Descargar PDF"
   - Visualizar en navegador o Adobe
   - Verificar UUID y código QR

---

## 🐛 PROBLEMAS COMUNES

### ❌ Error: "401 Unauthorized"

**Solución:**
```xml
<!-- Verificar en Web.config -->
<add key="FiscalAPI_ApiKey" value="..." />
<add key="FiscalAPI_Tenant" value="..." />
```
- Copiar exactamente desde FiscalAPI Dashboard
- No dejar espacios al inicio/final

---

### ❌ Error: "422 Validation - CURP is required"

**Solución:**
```sql
UPDATE Empleados
SET CURP = 'XEXX010101HNEXXXA4'
WHERE EmpleadoID = [ID_DEL_EMPLEADO]
```

---

### ❌ Error: "Cannot access NominasCFDI table"

**Solución:**
```sql
-- Ejecutar script en Paso 3.2
ALTER TABLE NominasCFDI ADD InvoiceId VARCHAR(100) NULL
```

---

### ❌ Botón "Timbrar" no aparece

**Causa:** Recibo ya está timbrado

**Verificar:**
```sql
SELECT UUID, FechaTimbrado, EstatusTimbre
FROM NominaDetalle
WHERE NominaDetalleID = [ID_DEL_RECIBO]
```

Si ya tiene UUID, está timbrado ✅

---

### ❌ Error: "Timeout: FiscalAPI no respondió"

**Solución:**
1. Verificar conexión a internet
2. Intentar de nuevo (puede ser lentitud temporal)
3. Si persiste, aumentar timeout en `FiscalAPIService.cs`:
```csharp
_httpClient.Timeout = TimeSpan.FromMinutes(5); // línea ~45
```

---

## 📊 VERIFICAR RESULTADO EN BASE DE DATOS

```sql
-- Ver último timbrado
SELECT TOP 1
    nc.UUID,
    nc.FechaTimbrado,
    nc.InvoiceId,
    nc.EstadoTimbrado,
    nd.Folio,
    e.NombreCompleto
FROM NominasCFDI nc
INNER JOIN NominaDetalle nd ON nc.NominaDetalleID = nd.NominaDetalleID
INNER JOIN Empleados e ON nd.EmpleadoID = e.EmpleadoID
ORDER BY nc.FechaTimbrado DESC
```

**Resultado esperado:**
```
UUID:            12345678-1234-1234-1234-123456789012
FechaTimbrado:   2024-05-15 10:30:45
InvoiceId:       abc123def456
EstadoTimbrado:  EXITOSO
Folio:           N-2024-001-001
NombreCompleto:  Juan Pérez García
```

---

## 🎯 CHECKLIST FINAL - ¿TODO FUNCIONÓ?

- [ ] Web.config actualizado con API Key y Tenant
- [ ] Base de datos tiene columna InvoiceId
- [ ] Empleados tienen CURP, NSS, RFC
- [ ] Proyecto compiló sin errores
- [ ] Nómina de prueba creada
- [ ] Recibo timbrado exitosamente
- [ ] UUID visible en la pantalla
- [ ] XML descargado y validado
- [ ] PDF descargado y visualizado

**Si todos están marcados:** ✅ **¡IMPLEMENTACIÓN EXITOSA!**

---

## 📞 SIGUIENTE PASO

Una vez que la primera prueba funcione:

### Certificados SAT (Opcional para producción)

1. Obtener certificados .cer y .key del SAT
2. Convertir a Base64:
```powershell
.\CONVERTIR_CERTIFICADOS_BASE64.ps1
```
3. Copiar resultado a Web.config:
```xml
<add key="FiscalAPI_CertificadoBase64" value="MII..." />
<add key="FiscalAPI_LlavePrivadaBase64" value="MII..." />
<add key="FiscalAPI_PasswordCertificado" value="tu_password" />
```

### Migrar a Producción

1. Cambiar URL en Web.config:
```xml
<add key="FiscalAPI_UrlApi" value="https://api.fiscalapi.com" />
```
2. Usar API Key de producción
3. Usar certificados SAT reales

---

## 📚 DOCUMENTACIÓN ADICIONAL

- **Manual completo:** `IMPLEMENTACION_NOMINA_CFDI_COMPLETA.md`
- **Resumen 5 pasos:** `RESUMEN_IMPLEMENTACION_5_PASOS.md`
- **FiscalAPI Docs:** https://www.fiscalapi.com/docs

---

## ⏱️ TIEMPO ESTIMADO TOTAL

- ~~Paso 2 (FiscalAPI): 5 minutos~~ ✅ **Ya configurado**
- Paso 3 (Base de datos): 3 minutos
- Paso 4 (Primera prueba): 3 minutos

**TOTAL:** ⏱️ **6 minutos** (antes 10 minutos)

---

**Fecha:** ${new Date().toLocaleString()}  
**Status:** ⏳ LISTO PARA CONFIGURAR  
**Versión:** 1.0.0
