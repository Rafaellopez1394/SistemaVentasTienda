# 🎯 RESUMEN VISUAL: COMPILACIÓN Y DESPLIEGUE

```
╔════════════════════════════════════════════════════════════════════════════╗
║                    COMPILAR Y DESPLEGAR EN IIS PRODUCTIVO                  ║
╚════════════════════════════════════════════════════════════════════════════╝

Tu pregunta: "¿Cómo se debe compilar para montarla en IIS productivo?"

Respuesta: 3 PASOS SIMPLES

┌────────────────────────────────────────────────────────────────────────────┐
│ PASO 1: VERIFICAR PRE-REQUISITOS                                           │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  PowerShell (como Administrador):                                          │
│  $ cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"               │
│  $ .\VERIFICAR_ANTES_DESPLEGAR.ps1                                         │
│                                                                              │
│  Verifica:                                                                  │
│  ✓ Permisos Admin      ✓ MSBuild         ✓ IIS                            │
│  ✓ .NET Framework 4.6+ ✓ SQL Server      ✓ FiscalAPI PROD               │
│  ✓ Espacio en disco    ✓ Web.config      ✓ Puertos disponibles           │
│                                                                              │
└────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│ PASO 2: ACTUALIZAR CREDENCIALES FISCALAPI EN BD                            │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  SQL Server Management Studio (DB_TIENDA):                                │
│                                                                              │
│  UPDATE ConfiguracionPAC                                                   │
│  SET ApiKey = 'sk_live_TUCLAVEREAL',                                      │
│      Tenant = 'TU-TENANT-ID-REAL',                                        │
│      EsProduccion = 1                    ← IMPORTANTE                      │
│  WHERE ConfigPACID = 1;                                                    │
│                                                                              │
│  Verificar:                                                                 │
│  SELECT EsProduccion, ApiKey, Tenant FROM ConfiguracionPAC;               │
│  (Debe mostrar: 1 | sk_live_... | TU-TENANT-ID)                          │
│                                                                              │
└────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│ PASO 3: EJECUTAR DESPLIEGUE AUTOMATIZADO                                   │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  PowerShell (como Administrador):                                          │
│  $ .\DESPLEGAR_PRODUCCION.ps1                                              │
│                                                                              │
│  El script automáticamente:                                               │
│  [1/7] Limpia compilaciones anteriores                                    │
│  [2/7] Compila solución en modo Release (sin debug)                       │
│  [3/7] Detiene IIS                                                        │
│  [4/7] Prepara carpeta de publicación                                     │
│  [5/7] Copia archivos compilados                                          │
│  [6/7] Configura IIS (Application Pool + Sitio web)                       │
│  [7/7] Inicia sitio web                                                   │
│                                                                              │
│  Resultado final:                                                           │
│  ✓ URL: http://localhost (PRODUCCIÓN)                                     │
│  ✓ Carpeta: C:\inetpub\wwwroot\SistemaVentas                              │
│  ✓ BD: DB_TIENDA (conectada)                                              │
│  ✓ FiscalAPI: Automáticamente en PRODUCCIÓN                               │
│                                                                              │
└────────────────────────────────────────────────────────────────────────────┘

═════════════════════════════════════════════════════════════════════════════

VERIFICAR QUE FUNCIONA:

  Navegador: http://localhost
  └─ Deberías ver: Página de login ✓

═════════════════════════════════════════════════════════════════════════════

ARCHIVOS CREADOS PARA AYUDARTE:

  1. DESPLEGAR_QUICK_START.md (Este archivo)
     └─ Resumen rápido y checklist final

  2. COMPILAR_Y_DESPLEGAR_PRODUCCION.md (Guía completa)
     └─ Paso a paso detallado con explicaciones
     └─ Troubleshooting y monitoreo

  3. DESPLEGAR_PRODUCCION.ps1 (Script automatizado)
     └─ Ejecuta TODO automáticamente ← ESTO ES LO QUE USAS
     └─ Con validaciones y manejo de errores

  4. VERIFICAR_ANTES_DESPLEGAR.ps1 (Verificaciones previas)
     └─ Verifica 10 puntos críticos
     └─ Evita despliegues fallidos ← EJECUTA PRIMERO

═════════════════════════════════════════════════════════════════════════════

LO QUE NO NECESITAS CAMBIAR:

  ✗ Código C# (No toques nada)
  ✗ Vistas MVC (No toques nada)
  ✗ Modelos (No toques nada)
  ✗ Configuración de BD (Ya está)
  ✗ Web.config (debug=false ya está)

═════════════════════════════════════════════════════════════════════════════

LO QUE SÍ NECESITAS CAMBIAR:

  ✓ ConfiguracionPAC.ApiKey = 'sk_live_...' (en BD)
  ✓ ConfiguracionPAC.Tenant = '...' (en BD)
  ✓ ConfiguracionPAC.EsProduccion = 1 (en BD)

═════════════════════════════════════════════════════════════════════════════

FLUJO DE EJECUCIÓN:

  VERIFICAR_ANTES_DESPLEGAR.ps1
        ↓
    ✓ Todo OK?
        ↓ Sí
  UPDATE ConfiguracionPAC (SQL)
        ↓
  DESPLEGAR_PRODUCCION.ps1
        ↓
    [1/7] Limpia
    [2/7] Compila Release
    [3/7] Detiene IIS
    [4/7] Prepara carpeta
    [5/7] Copia archivos
    [6/7] Configura IIS
    [7/7] Inicia sitio
        ↓
  ✓ Sistema en Producción!

═════════════════════════════════════════════════════════════════════════════

COMANDOS RÁPIDOS (Después del despliegue):

  # Ver estado del sitio
  Get-Website -Name "SistemaVentas"

  # Ver estado del Application Pool
  Get-WebAppPoolState -Name "VentasWebPool"

  # Reiniciar sitio
  Stop-Website -Name "SistemaVentas"
  Start-Website -Name "SistemaVentas"

  # Reiniciar IIS completo
  iisreset /restart

  # Ver logs
  C:\inetpub\logs\LogFiles\W3SVC1\

═════════════════════════════════════════════════════════════════════════════

CHECKLIST ANTES DE EJECUTAR:

  [ ] Ejecuté VERIFICAR_ANTES_DESPLEGAR.ps1 → ✓ TODO OK
  [ ] Actualicé BD con credenciales FiscalAPI
  [ ] Verifiqué EsProduccion = 1
  [ ] Cierro Visual Studio
  [ ] PowerShell como Administrador
  [ ] En la carpeta correcta

  Si TODO está marcado → .\DESPLEGAR_PRODUCCION.ps1

═════════════════════════════════════════════════════════════════════════════

ERRORES COMUNES:

  HTTP Error 500?
  └─ Debug = true en Web.config
  └─ Cambiar a debug="false"

  Application Pool stopped?
  └─ Ver logs: C:\inetpub\logs\LogFiles\W3SVC1\
  └─ Ver eventos: eventvwr.msc

  Cannot connect to database?
  └─ Verificar connection string en Web.config
  └─ Verificar que SQL Server está corriendo

  FiscalAPI timeout?
  └─ Verificar EsProduccion = 1 en BD
  └─ NO debe estar en TEST

═════════════════════════════════════════════════════════════════════════════

RESULTADO FINAL:

  ✓ Sitio web:    http://localhost (PRODUCCIÓN)
  ✓ Base de datos: DB_TIENDA (conectada)
  ✓ FiscalAPI:    Automáticamente en PRODUCCIÓN
  ✓ Compilación:  Release (optimizada)
  ✓ IIS:          Configurado

═════════════════════════════════════════════════════════════════════════════

¿LISTO PARA COMENZAR?

  1. Abre PowerShell como Administrador
  2. Navega a: C:\Users\Rafael Lopez\Documents\SistemaVentasTienda
  3. Ejecuta: .\VERIFICAR_ANTES_DESPLEGAR.ps1
  4. Luego: .\DESPLEGAR_PRODUCCION.ps1
  5. Verifica: http://localhost

  ¡LISTO! ✅

═════════════════════════════════════════════════════════════════════════════
```

---

## 📊 Tabla de Referencia Rápida

### Configuración de Compilación

| Aspecto | Valor |
|---------|-------|
| Configuración | Release |
| Debug Symbols | false |
| Debug Type | None |
| Optimization | Enabled |
| Target Framework | .NET 4.6 |

### Estructura de IIS

| Elemento | Valor |
|----------|-------|
| Sitio Web | SistemaVentas |
| Application Pool | VentasWebPool |
| Puerto | 80 |
| Protocolo | HTTP |
| Carpeta | C:\inetpub\wwwroot\SistemaVentas |
| Pool Runtime | .NET CLR v4.0 |
| Pipeline | Integrated |

### Configuración de Base de Datos

| Campo | Valor |
|-------|-------|
| Base de datos | DB_TIENDA |
| Tabla | ConfiguracionPAC |
| ApiKey | sk_live_TUCLAVEREAL |
| Tenant | TU-TENANT-ID-REAL |
| EsProduccion | 1 |
| Ambiente | PRODUCCIÓN |

---

## 🎯 Próximas Acciones

1. ✅ Lee [DESPLEGAR_QUICK_START.md](DESPLEGAR_QUICK_START.md)
2. ✅ Ejecuta [VERIFICAR_ANTES_DESPLEGAR.ps1](VERIFICAR_ANTES_DESPLEGAR.ps1)
3. ✅ Actualiza BD (3 valores en ConfiguracionPAC)
4. ✅ Ejecuta [DESPLEGAR_PRODUCCION.ps1](DESPLEGAR_PRODUCCION.ps1)
5. ✅ Verifica en navegador: http://localhost
6. ✅ Listo! Sistema en Producción

---

**Para más detalles, consulta**: [COMPILAR_Y_DESPLEGAR_PRODUCCION.md](COMPILAR_Y_DESPLEGAR_PRODUCCION.md)
