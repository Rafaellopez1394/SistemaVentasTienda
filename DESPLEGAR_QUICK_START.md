# � COMPILAR Y DESPLEGAR EN IIS PRODUCTIVO

## 🎯 Tu Pregunta
**"¿Cómo se debe compilar para montarla en IIS productivo?"**

## ✅ Respuesta: 3 Pasos Simples

### **PASO 1: Verificar Pre-requisitos**
```powershell
# Ejecuta como Administrador
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
.\VERIFICAR_ANTES_DESPLEGAR.ps1
```
Este script verifica:
- ✓ Permisos de Administrador
- ✓ .NET Framework 4.6+
- ✓ Visual Studio 2022 / MSBuild
- ✓ IIS instalado
- ✓ SQL Server accesible
- ✓ FiscalAPI en PRODUCCIÓN
- ✓ Web.config con debug=false
- ✓ Espacio en disco

### **PASO 2: Actualizar Credenciales FiscalAPI**
```sql
-- Ejecuta en SQL Server Management Studio (DB_TIENDA)
UPDATE ConfiguracionPAC
SET 
    ApiKey = 'sk_live_TUCLAVEREAL',
    Tenant = 'TU-TENANT-ID-REAL',
    EsProduccion = 1
WHERE ConfigPACID = 1;
```

### **PASO 3: Ejecutar Despliegue Automatizado**
```powershell
# Ejecuta como Administrador
.\DESPLEGAR_PRODUCCION.ps1
```

**¡LISTO!** El sistema está en producción. ✅

---

## 📦 Lo que Hace Automáticamente (7 Pasos)

| # | Acción | Resultado |
|---|--------|-----------|
| 1 | Limpia compilaciones previas | Carpetas bin/obj sin archivos viejos |
| 2 | Compila en modo Release | DLLs sin información debug |
| 3 | Detiene IIS | Evita bloqueos de archivos |
| 4 | Prepara carpeta | `C:\inetpub\wwwroot\SistemaVentas` limpia |
| 5 | Copia archivos | bin, Content, Scripts, Views, Web.config |
| 6 | Configura IIS | Application Pool + Sitio web + Permisos |
| 7 | Inicia sistema | Todo ejecutándose en puerto 80 |

---

## 🔑 Conceptos Clave

### Compilación: Release vs Debug

| Aspecto | Debug | Release |
|--------|-------|---------|
| Optimizado | ✗ No | ✓ Sí |
| Información debug | ✓ Sí | ✗ No |
| Tamaño DLL | Grande | Pequeño |
| Velocidad | Lenta | Rápida |
| Producción | ✗ No | ✓ Sí |

**Tu script compila en: Release** ✓

### Application Pool (IIS)

```
Nombre:      VentasWebPool
Runtime:     .NET CLR v4.0
Pipeline:    Integrated
Reciclaje:   24 horas
Permisos:    Modify en C:\inetpub\wwwroot\SistemaVentas
```

### Sitio Web (IIS)

```
Nombre:      SistemaVentas
URL:         http://localhost:80
Ruta:        C:\inetpub\wwwroot\SistemaVentas
Pool:        VentasWebPool
Protocolo:   HTTP
```

---

## 🚀 Workflow Completo

```
1. VERIFICAR PRE-REQUISITOS
   ↓
   .\VERIFICAR_ANTES_DESPLEGAR.ps1
   ↓
   ¿TODO OK? → Continuar
   ¿FALTA ALGO? → Instalar/Configurar
   
2. ACTUALIZAR CREDENCIALES FISCALAPI
   ↓
   SQL: UPDATE ConfiguracionPAC
   ↓
   EsProduccion = 1 (IMPORTANTE)
   
3. EJECUTAR DESPLIEGUE
   ↓
   .\DESPLEGAR_PRODUCCION.ps1
   ↓
   - Compila Release
   - Copia archivos
   - Configura IIS
   - Inicia sitio
   
4. VERIFICAR EN NAVEGADOR
   ↓
   http://localhost
   ↓
   ¿Login visible? → LISTO ✓
```

---

## 📁 Archivos de Apoyo Creados

### 1. **DESPLEGAR_QUICK_START.md** (Este)
- Resumen de 5 minutos
- Conceptos clave
- Checklist final

### 2. **COMPILAR_Y_DESPLEGAR_PRODUCCION.md** (Guía Completa)
- Explicación paso a paso
- Scripts manuales para cada paso
- Configuración de seguridad
- Troubleshooting detallado
- Monitoreo en producción

### 3. **DESPLEGAR_PRODUCCION.ps1** (Script Automatizado)
- Ejecuta TODO automáticamente
- Con validaciones
- Con manejo de errores
- Mostrar resumen final
- **← ESTO ES LO QUE EJECUTAS**

### 4. **VERIFICAR_ANTES_DESPLEGAR.ps1** (Pre-flight Checks)
- Verifica 10 puntos críticos
- Evita despliegues fallidos
- **← EJECUTA PRIMERO**

---

## ⚙️ Configuración Requerida

### En SQL Server (DB_TIENDA):
```sql
-- Cambiar credenciales a PRODUCCIÓN
UPDATE ConfiguracionPAC
SET 
    ApiKey = 'sk_live_XXXXXXXXXXXXXXXX',    -- ← Tu X-API-KEY
    Tenant = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxx', -- ← Tu X-TENANT-KEY
    EsProduccion = 1                          -- ← IMPORTANTE
WHERE ConfigPACID = 1;

-- Verificar
SELECT EsProduccion, ApiKey, Tenant FROM ConfiguracionPAC WHERE ConfigPACID = 1;
```

### En Web.config (Ya está configurado):
```xml
<compilation debug="false" ... />  <!-- ✓ CORRECTO -->
```

### En IIS (Lo hace el script automáticamente):
- ✓ Crear Application Pool
- ✓ Crear sitio web
- ✓ Configurar permisos NTFS

---

## 🔍 Verificar Que Funciona

### En Navegador:
```
http://localhost

Deberías ver:
✓ Página de login
✓ Campos de Usuario y Contraseña
✓ Botón de login
```

### En PowerShell (Verificación Rápida):
```powershell
# Ver estado del sitio
Get-Website -Name "SistemaVentas"

# Ver estado del Application Pool
Get-WebAppPoolState -Name "VentasWebPool"

# Ver últimos errores
Get-EventLog -LogName "Application" -Newest 5
```

### En SQL Server:
```sql
-- Verificar que hay datos
SELECT COUNT(*) FROM Ventas

-- Verificar credenciales en PRODUCCIÓN
SELECT EsProduccion, LEFT(ApiKey,20) AS ApiKey FROM ConfiguracionPAC
```

---

## ⚠️ Errores Comunes y Soluciones

### Error: "HTTP Error 500"
```
Causa:   Web.config con debug="true"
Solución: Cambiar a debug="false" antes de desplegar
```

### Error: "Application Pool stopped"
```
Causa:   Excepción en código o referencia faltante
Solución: Revisar logs en C:\inetpub\logs\LogFiles\W3SVC1\
```

### Error: "Cannot connect to database"
```
Causa:   Connection string incorrecta
Solución: Verificar connection string en Web.config
         Verificar que SQL Server está ejecutándose
```

### Error: "FiscalAPI endpoint timeout"
```
Causa:   EsProduccion = 0 (aún está en TEST)
Solución: Verificar en BD: SELECT EsProduccion FROM ConfiguracionPAC
         Debe estar en 1 para PRODUCCIÓN
```

---

## 📊 Resultado Final

Después de ejecutar el script:

```
✓ Sitio web:      http://localhost (PRODUCCIÓN)
✓ Base de datos:  DB_TIENDA (conectada)
✓ FiscalAPI:      Automáticamente en PRODUCCIÓN
✓ Compilación:    Release (optimizada, sin debug)
✓ IIS:            Configurado con reciclaje 24h
✓ Permisos:       IIS AppPool puede escribir
```

---

## 📞 Comandos Útiles para Después

```powershell
# Ver estado del sitio
Get-Website -Name "SistemaVentas"

# Ver estado del Application Pool
Get-WebAppPoolState -Name "VentasWebPool"

# Reiniciar sitio web
Stop-Website -Name "SistemaVentas"
Start-Website -Name "SistemaVentas"

# Reiniciar Application Pool
Restart-WebAppPool -Name "VentasWebPool"

# Reiniciar todo IIS
iisreset /restart

# Abrir IIS Manager
inetmgr

# Ver logs
C:\inetpub\logs\LogFiles\W3SVC1\
```

---

## ✅ Checklist Antes de Ejecutar

- [ ] Ejecuté VERIFICAR_ANTES_DESPLEGAR.ps1 → ✓ TODO OK
- [ ] Actualicé BD con credenciales FiscalAPI producción
- [ ] Verifiqué EsProduccion = 1 en BD
- [ ] Cierro Visual Studio (para liberar archivos)
- [ ] Estoy en PowerShell como Administrador
- [ ] Estoy en carpeta correcta

Si todo está marcado → **Ejecuta**:
```powershell
.\DESPLEGAR_PRODUCCION.ps1
```

---

## 🎉 ¡LISTO PARA COMENZAR!

1. **Abre PowerShell como Administrador**
2. **Navega a la carpeta del proyecto**:
   ```powershell
   cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
   ```
3. **Ejecuta verificación**:
   ```powershell
   .\VERIFICAR_ANTES_DESPLEGAR.ps1
   ```
4. **Si TODO está OK, ejecuta despliegue**:
   ```powershell
   .\DESPLEGAR_PRODUCCION.ps1
   ```
5. **Verifica en navegador**:
   ```
   http://localhost
   ```

**¡Sistema en PRODUCCIÓN!** ✅

---

**Para más detalles**, ver: [COMPILAR_Y_DESPLEGAR_PRODUCCION.md](COMPILAR_Y_DESPLEGAR_PRODUCCION.md)
