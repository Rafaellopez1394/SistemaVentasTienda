# ✅ RESPUESTA FINAL: ¿CÓMO COMPILAR Y DESPLEGAR EN IIS PRODUCTIVO?

## 📌 TU PREGUNTA
**"¿Cómo se debe compilar para montarla en IIS productivo?"**

## ✨ LA RESPUESTA CORTA
**3 comandos PowerShell. Eso es TODO.**

```powershell
# 1. Verificar pre-requisitos (una sola vez)
.\VERIFICAR_ANTES_DESPLEGAR.ps1

# 2. Actualizar BD (una sola vez)
UPDATE ConfiguracionPAC SET ApiKey='sk_live_...', Tenant='...', EsProduccion=1

# 3. Desplegar (automático, ejecuta TODO)
.\DESPLEGAR_PRODUCCION.ps1
```

**Resultado**: Sistema en `http://localhost` en PRODUCCIÓN ✅

---

## 🎯 LO QUE NECESITAS SABER

### ✓ NO Necesitas Hacer
- ✗ Cambiar código C#
- ✗ Cambiar Web.config (ya está configurado)
- ✗ Instalar/configurar dependencias
- ✗ Aprender PowerShell
- ✗ Tocar archivos manualmente

### ✓ TODO Lo Hace Automáticamente
- ✓ Compila en modo Release (sin debug)
- ✓ Copia archivos a IIS
- ✓ Crea Application Pool
- ✓ Crea sitio web
- ✓ Configura permisos
- ✓ Inicia el sistema

### ✓ Solo Necesitas Hacer
1. Actualizar 2 valores en BD (credenciales FiscalAPI)
2. Ejecutar un script PowerShell

---

## 📋 ARCHIVOS CREADOS PARA TI

He creado **5 documentos** listos para usar:

| Archivo | Propósito | Cuándo Usar |
|---------|-----------|----------|
| **DESPLEGAR_QUICK_START.md** | Resumen rápido | Para entender en 5 min |
| **COMPILAR_Y_DESPLEGAR_PRODUCCION.md** | Guía completa | Para detalles y troubleshooting |
| **DESPLIEGUE_RESUMEN_VISUAL.md** | Resumen visual | Para ver el flujo |
| **PASOS_MANUALES_DESPLIEGUE.md** | Pasos manuales | Si quieres hacer todo a mano |
| **DESPLEGAR_PRODUCCION.ps1** | Script automatizado | ← **ESTO ES LO QUE EJECUTAS** |
| **VERIFICAR_ANTES_DESPLEGAR.ps1** | Verificaciones previas | Ejecuta primero esto |

---

## 🚀 FLUJO PASO A PASO

### Paso 1: Verificar Pre-requisitos (5 minutos)

```powershell
# PowerShell como Administrador
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
.\VERIFICAR_ANTES_DESPLEGAR.ps1
```

Este script verifica:
- ✓ Eres Administrador
- ✓ Tienes .NET Framework 4.6+
- ✓ Tienes Visual Studio 2022
- ✓ Tienes IIS instalado
- ✓ SQL Server está accesible
- ✓ FiscalAPI está en PRODUCCIÓN
- ✓ Hay espacio en disco

**Si TODO está OK → Continúa al Paso 2**
**Si algo falla → El script te dirá qué arreglar**

### Paso 2: Actualizar Credenciales FiscalAPI (2 minutos)

En SQL Server Management Studio, ejecuta:

```sql
USE DB_TIENDA
GO

UPDATE ConfiguracionPAC
SET 
    ApiKey = 'sk_live_TUCLAVEREAL',           -- ← Tu X-API-KEY de producción
    Tenant = 'TU-TENANT-ID-REAL',              -- ← Tu X-TENANT-KEY de producción
    EsProduccion = 1                            -- ← IMPORTANTE: 1 = PRODUCCIÓN
WHERE ConfigPACID = 1;

-- Verifica que se actualizó
SELECT EsProduccion, ApiKey, Tenant FROM ConfiguracionPAC WHERE ConfigPACID = 1;
```

**Resultado esperado**:
- EsProduccion = 1
- ApiKey = sk_live_...
- Tenant = tu-tenant-id

### Paso 3: Ejecutar Despliegue (5-10 minutos)

```powershell
# PowerShell como Administrador
.\DESPLEGAR_PRODUCCION.ps1
```

El script hace TODO automáticamente:
1. [1/7] Limpia compilaciones previas
2. [2/7] Compila en modo Release
3. [3/7] Detiene IIS
4. [4/7] Prepara carpeta de publicación
5. [5/7] Copia archivos
6. [6/7] Configura IIS (Application Pool + Sitio)
7. [7/7] Inicia sistema

**Al terminar verás**:
```
✓ Sitio web: http://localhost (PRODUCCIÓN)
✓ Base de datos: DB_TIENDA (conectada)
✓ FiscalAPI: Automáticamente en PRODUCCIÓN
```

### Paso 4: Verificar en Navegador (1 minuto)

```
Abre navegador: http://localhost

Deberías ver:
✓ Página de login
✓ Campos de usuario y contraseña
✓ Sistema funcional
```

**¡LISTO!** ✅ Sistema en PRODUCCIÓN

---

## 🔧 Conceptos Técnicos (Para referencia)

### Compilación: Release vs Debug

```
┌─────────────────┬──────────────┬──────────────┐
│ Característica  │    DEBUG     │   RELEASE    │
├─────────────────┼──────────────┼──────────────┤
│ Optimizado      │     No       │     Sí       │
│ Info de debug   │     Sí       │     No       │
│ Tamaño DLL      │    Grande    │   Pequeño    │
│ Velocidad       │     Lenta    │    Rápida    │
│ Producción      │     No       │     Sí       │
└─────────────────┴──────────────┴──────────────┘

Tu script compila en: RELEASE ✓
```

### Architecture de IIS

```
Internet
    ↓
Port 80 (HTTP)
    ↓
Sitio Web: "SistemaVentas"
    ↓
Application Pool: "VentasWebPool" (.NET v4.0)
    ↓
Carpeta: C:\inetpub\wwwroot\SistemaVentas
    ↓
Aplicación ASP.NET MVC
    ↓
Base de Datos: DB_TIENDA (SQL Server)
```

### Flujo de Credenciales FiscalAPI

```
ConfiguracionPAC (BD)
├─ EsProduccion = 1
├─ ApiKey = 'sk_live_...'
└─ Tenant = '...'
    ↓
Leído por: CD_Factura.ObtenerConfiguracionPAC()
    ↓
ConfiguracionFiscalAPI object
    ↓
FiscalAPIService (HttpClient)
    ↓
Headers HTTP:
├─ X-API-KEY: sk_live_...
└─ X-TENANT-KEY: ...
    ↓
URL: https://api.fiscalapi.com (automático)
    ↓
FiscalAPI Producción ✓
```

---

## 📊 Estructura de Carpetas (Resultado Final)

```
C:\inetpub\wwwroot\SistemaVentas\
├── bin/
│   ├── VentasWeb.dll            ← Compilado Release
│   ├── CapaDatos.dll            ← Compilado Release
│   ├── CapaModelo.dll           ← Compilado Release
│   ├── roslyn/                  ← Compilador runtime
│   └── [Other DLLs]
├── Content/
│   ├── css/                     ← Estilos
│   └── images/                  ← Imágenes
├── Scripts/
│   ├── jquery/                  ← jQuery
│   └── bootstrap/               ← Bootstrap
├── Views/
│   ├── Login/                   ← Vistas Login
│   ├── Ventas/                  ← Vistas Ventas
│   └── Shared/                  ← Layouts
├── Web.config                   ← debug="false"
├── Global.asax                  ← Configuración global
└── favicon.ico
```

---

## ⚠️ Checklist Antes de Ejecutar

```
Antes de ejecutar DESPLEGAR_PRODUCCION.ps1:

□ Ejecuté VERIFICAR_ANTES_DESPLEGAR.ps1 → TODO OK
□ Actualicé DB con credenciales FiscalAPI producción
□ Verifiqué EsProduccion = 1 en BD
□ Cierro Visual Studio (libera archivos)
□ Estoy en PowerShell como Administrador
□ Estoy en carpeta: C:\Users\Rafael Lopez\Documents\SistemaVentasTienda

Si TODOS están marcados → Ejecuta:
.\DESPLEGAR_PRODUCCION.ps1
```

---

## 🆘 Si Algo Falla

### Error: "HTTP Error 500"
```
Causa:   Generalmente debug=true en Web.config
Solución: Ya está configurado como debug="false"
         Revisar logs: C:\inetpub\logs\LogFiles\W3SVC1\
```

### Error: "Cannot connect to database"
```
Causa:   SQL Server no accesible o connection string incorrecta
Solución: Verificar que SQL Server está corriendo
         Verificar connection string en Web.config
         Verificar credenciales de SQL
```

### Error: "FiscalAPI timeout"
```
Causa:   EsProduccion = 0 (aún en TEST)
Solución: En BD: SELECT EsProduccion FROM ConfiguracionPAC
         Debe ser 1 para PRODUCCIÓN
```

### Error: "Application Pool stopped"
```
Causa:   Excepción no manejada o recurso faltante
Solución: 1. Revisar logs en C:\inetpub\logs\LogFiles\W3SVC1\
         2. Abrir Visor de Eventos: eventvwr.msc
         3. Buscar eventos de IIS/ASP.NET
```

---

## ✅ Verificación Rápida Post-Despliegue

```powershell
# ¿Sitio está corriendo?
Get-Website -Name "SistemaVentas"
# Resultado: State = "Started"

# ¿Application Pool está corriendo?
Get-WebAppPoolState -Name "VentasWebPool"
# Resultado: Started

# ¿BD está accesible?
sqlcmd -S localhost -d DB_TIENDA -Q "SELECT COUNT(*) FROM ConfiguracionPAC"
# Resultado: Un número (debe haber al menos 1 registro)

# Abrir en navegador
Start-Process "http://localhost"
# Resultado: Página de login visible
```

---

## 📞 Comandos Útiles (Para Después)

```powershell
# Ver estado de todo
Get-Website -Name "SistemaVentas"
Get-WebAppPoolState -Name "VentasWebPool"

# Reiniciar sitio
Stop-Website -Name "SistemaVentas"
Start-Website -Name "SistemaVentas"

# Reiniciar pool
Restart-WebAppPool -Name "VentasWebPool"

# Reiniciar IIS completo
iisreset /restart

# Ver logs
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*" | tail -20

# Abrir IIS Manager
inetmgr

# Abrir Visor de Eventos
eventvwr.msc
```

---

## 🎓 Comparativa: Manual vs Automatizado

```
┌──────────────────┬──────────────┬───────────────────┐
│ Tarea            │   Manual     │  Script Automático│
├──────────────────┼──────────────┼───────────────────┤
│ Limpiar archivos │   5 min      │  1 seg (automático)
│ Compilar solución│  10 min      │  3 min (automático)
│ Copiar archivos  │  15 min      │  1 min (automático)
│ Configurar IIS   │  20 min      │  2 min (automático)
│ Configurar perms │  10 min      │  0 min (automático)
│ Iniciar sitio    │   2 min      │  0 min (automático)
├──────────────────┼──────────────┼───────────────────┤
│ TOTAL            │  ~62 min     │  ~7 min + verificar|
│ Errores posibles │   Muchos     │  Validación auto  │
│ Documentación    │  Búscas tú   │  Script te ayuda  │
└──────────────────┴──────────────┴───────────────────┘

Te recomiendo: Script Automatizado ← Menos errores
```

---

## 🎉 RESULTADO FINAL

```
ANTES (Test):
  URL: https://test.fiscalapi.com
  X-API-KEY: sk_test_...
  X-TENANT-KEY: ...
  Sistema: En desarrollo

DESPUÉS (Producción):
  URL: https://api.fiscalapi.com       ← Automático
  X-API-KEY: sk_live_...               ← Desde BD
  X-TENANT-KEY: ...                    ← Desde BD
  Sistema: En http://localhost         ← En vivo
  Estado: PRODUCCIÓN                   ← Listo ✓
```

---

## 🚀 RESUMEN EJECUTIVO

| Paso | Qué Hacer | Tiempo | Estado |
|------|-----------|--------|--------|
| 1 | Ejecutar verificación | 5 min | Pre-check |
| 2 | Actualizar BD (3 valores) | 2 min | Config |
| 3 | Ejecutar despliegue | 7 min | Deploy |
| 4 | Verificar navegador | 1 min | QA |
| **TOTAL** | **TODO** | **~15 min** | **✓ LISTO** |

---

## 📖 Para Más Información

- **Resumen rápido**: [DESPLEGAR_QUICK_START.md](DESPLEGAR_QUICK_START.md)
- **Guía completa**: [COMPILAR_Y_DESPLEGAR_PRODUCCION.md](COMPILAR_Y_DESPLEGAR_PRODUCCION.md)
- **Pasos manuales**: [PASOS_MANUALES_DESPLIEGUE.md](PASOS_MANUALES_DESPLIEGUE.md)
- **Resumen visual**: [DESPLIEGUE_RESUMEN_VISUAL.md](DESPLIEGUE_RESUMEN_VISUAL.md)

---

## ✅ SIGUIENTE ACCIÓN

1. Abre PowerShell como Administrador
2. Navega a: `C:\Users\Rafael Lopez\Documents\SistemaVentasTienda`
3. Ejecuta: `.\VERIFICAR_ANTES_DESPLEGAR.ps1`
4. Si OK: Ejecuta: `.\DESPLEGAR_PRODUCCION.ps1`
5. Verifica: `http://localhost`

**¡LISTO!** Sistema en PRODUCCIÓN ✅

---

**Creado**: 25 de Enero de 2026
**Estado**: Listo para usar
**Documentación**: Completa y actualizada
