# 🚀 COMPILAR Y DESPLEGAR EN IIS PRODUCTIVO

## 📋 Requisitos Previos

### Hardware
- **Servidor**: Windows Server 2012 R2 o superior (o Windows 10/11 Pro)
- **RAM**: Mínimo 4 GB, recomendado 8 GB+
- **Espacio disco**: 2 GB libres mínimo

### Software
- **IIS 8.0** o superior (con ASP.NET 4.5 habilitado)
- **.NET Framework 4.6** o superior
- **Visual Studio 2022 Community** (o versión profesional)
- **SQL Server** (local o remoto, con DB_TIENDA configurada)

### Verificar Pre-requisitos
```powershell
# Ejecuta como Administrador

# Verificar .NET Framework
$dotnet = Get-ChildItem "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4" -Name Release
Write-Host ".NET Framework version: $dotnet"

# Verificar IIS
Get-WindowsFeature Web-Server, Web-Asp-Net45, Web-Common-Http

# Verificar SQL Server
sqlcmd -S localhost -Q "SELECT @@VERSION"
```

---

## 🔧 PASO 1: PREPARAR EL CÓDIGO

### 1.1 Verificar Configuración de Producción

**En SQL Server (DB_TIENDA), asegúrate que:**
```sql
-- 1. FiscalAPI esté configurado para PRODUCCIÓN
SELECT ConfigPACID, EsProduccion, ApiKey, Tenant FROM ConfiguracionPAC WHERE ConfigPACID = 1;
-- EsProduccion DEBE SER 1 (indicando PRODUCCIÓN)

-- 2. La URL sea correcta
SELECT CASE WHEN EsProduccion = 1 
       THEN 'https://api.fiscalapi.com' 
       ELSE 'https://test.fiscalapi.com' END AS URL_FiscalAPI;

-- 3. Verificar que no haya datos de prueba
SELECT COUNT(*) as VentasPrueba FROM Ventas WHERE EsTestingData = 1;
SELECT COUNT(*) as ClientesPrueba FROM Clientes WHERE EsTestingData = 1;
```

### 1.2 Limpiar Carpeta de Compilación

```powershell
# Como Administrador
$solutionPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "Limpiando compilaciones anteriores..." -ForegroundColor Yellow

# Limpiar carpetas
Remove-Item "$solutionPath\VentasWeb\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$solutionPath\VentasWeb\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$solutionPath\CapaDatos\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$solutionPath\CapaDatos\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$solutionPath\CapaModelo\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$solutionPath\CapaModelo\obj" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Carpetas limpias" -ForegroundColor Green
```

---

## 🔨 PASO 2: COMPILAR EN MODO RELEASE

### 2.1 Compilar Solución

```powershell
# Como Administrador
$solutionPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb.sln"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   COMPILANDO PARA PRODUCCION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Compilar en modo Release
Write-Host "`n[1/3] Compilando solución..." -ForegroundColor Yellow
Set-Location "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

& $msbuild $solutionPath /t:Clean,Rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR en compilacion" -ForegroundColor Red
    exit 1
}

Write-Host "Compilacion exitosa" -ForegroundColor Green
```

### 2.2 Parámetros Importantes

| Parámetro | Valor | Significado |
|-----------|-------|------------|
| `/p:Configuration=Release` | Release | Compilación optimizada |
| `/p:DebugSymbols=false` | false | Sin símbolos de debug |
| `/p:DebugType=None` | None | Sin información de debug |
| `/t:Clean,Rebuild` | Clean,Rebuild | Limpia y recompila |

### 2.3 Verificar Compilación

```powershell
# Verificar que los DLL fueron creados
$files = @(
    "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\bin\VentasWeb.dll",
    "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\bin\CapaDatos.dll",
    "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaModelo\bin\CapaModelo.dll"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $size = (Get-Item $file).Length / 1MB
        Write-Host "✓ $(Split-Path $file -Leaf) - ${size:F2} MB" -ForegroundColor Green
    } else {
        Write-Host "✗ $(Split-Path $file -Leaf) - NO ENCONTRADO" -ForegroundColor Red
    }
}
```

---

## 📦 PASO 3: PREPARAR CARPETA DE PUBLICACIÓN

### 3.1 Crear Estructura de Publicación

```powershell
# Como Administrador
$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"

Write-Host "`n[2/3] Preparando carpeta de publicacion..." -ForegroundColor Yellow

# Crear carpeta raiz
if (Test-Path $publishPath) {
    # Detener IIS si el sitio ya existe
    $siteName = "SistemaVentas"
    if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
        Stop-Website -Name $siteName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 1
    }
    
    # Limpiar contenido
    Get-ChildItem "$publishPath\*" -Recurse -Force -ErrorAction SilentlyContinue | 
        Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
} else {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}

Write-Host "Carpeta preparada: $publishPath" -ForegroundColor Green
```

### 3.2 Copiar Archivos

```powershell
Write-Host "`n[3/3] Copiando archivos compilados..." -ForegroundColor Yellow

# Copiar bin (DLLs compilados)
Write-Host "  - Copiando bin/" -ForegroundColor Gray
$null = New-Item -ItemType Directory -Path "$publishPath\bin" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\bin\*" -Destination "$publishPath\bin" -Recurse -Force

# Copiar Content (CSS, imagenes)
Write-Host "  - Copiando Content/" -ForegroundColor Gray
$null = New-Item -ItemType Directory -Path "$publishPath\Content" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Content\*" -Destination "$publishPath\Content" -Recurse -Force

# Copiar Scripts (JavaScript)
Write-Host "  - Copiando Scripts/" -ForegroundColor Gray
$null = New-Item -ItemType Directory -Path "$publishPath\Scripts" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Scripts\*" -Destination "$publishPath\Scripts" -Recurse -Force

# Copiar Views (HTML)
Write-Host "  - Copiando Views/" -ForegroundColor Gray
$null = New-Item -ItemType Directory -Path "$publishPath\Views" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Views\*" -Destination "$publishPath\Views" -Recurse -Force

# Copiar fuentes si existen
if (Test-Path "$sourcePath\fonts") {
    Write-Host "  - Copiando fonts/" -ForegroundColor Gray
    $null = New-Item -ItemType Directory -Path "$publishPath\fonts" -Force -ErrorAction SilentlyContinue
    Copy-Item "$sourcePath\fonts\*" -Destination "$publishPath\fonts" -Recurse -Force -ErrorAction SilentlyContinue
}

# Copiar Web.config
Write-Host "  - Copiando Web.config" -ForegroundColor Gray
Copy-Item "$sourcePath\Web.config" -Destination "$publishPath\Web.config" -Force

# Copiar Global.asax
Write-Host "  - Copiando Global.asax" -ForegroundColor Gray
Copy-Item "$sourcePath\Global.asax" -Destination "$publishPath\Global.asax" -Force

# Copiar favicon si existe
if (Test-Path "$sourcePath\favicon.ico") {
    Copy-Item "$sourcePath\favicon.ico" -Destination "$publishPath\favicon.ico" -Force
}

Write-Host "Archivos copiados exitosamente" -ForegroundColor Green
```

### 3.3 Verificar Archivos Críticos

```powershell
Write-Host "`nVerificando archivos criticos..." -ForegroundColor Yellow

$criticalFiles = @(
    "$publishPath\bin\VentasWeb.dll",
    "$publishPath\bin\CapaDatos.dll",
    "$publishPath\bin\CapaModelo.dll",
    "$publishPath\Web.config",
    "$publishPath\Global.asax",
    "$publishPath\Views\web.config"
)

$allOk = $true
foreach ($file in $criticalFiles) {
    if (Test-Path $file) {
        Write-Host "  ✓ $(Split-Path $file -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $(Split-Path $file -Leaf) FALTA" -ForegroundColor Red
        $allOk = $false
    }
}

if (-not $allOk) {
    Write-Host "`nERROR: Faltan archivos criticos" -ForegroundColor Red
    exit 1
}
```

---

## 🌐 PASO 4: CONFIGURAR IIS

### 4.1 Importar Módulo de IIS

```powershell
# Como Administrador
Import-Module WebAdministration

# Verificar que IIS está disponible
if (-not (Get-Module WebAdministration)) {
    Write-Host "ERROR: WebAdministration no disponible" -ForegroundColor Red
    Write-Host "Instala: Add-WindowsFeature Web-Scripting-Tools" -ForegroundColor Yellow
    exit 1
}
```

### 4.2 Crear Application Pool

```powershell
$appPoolName = "VentasWebPool"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"

Write-Host "`n[4/5] Configurando Application Pool..." -ForegroundColor Yellow

# Eliminar Application Pool anterior si existe
if (Test-Path "IIS:\AppPools\$appPoolName") {
    Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    Remove-WebAppPool -Name $appPoolName
    Write-Host "  - Application Pool anterior eliminado" -ForegroundColor Gray
}

# Crear nuevo Application Pool
New-WebAppPool -Name $appPoolName
Write-Host "  - Application Pool creado" -ForegroundColor Green

# Configurar propiedades
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"

# Configurar reciclaje (IMPORTANTE para liberar memoria)
$appPool = Get-Item "IIS:\AppPools\$appPoolName"
$appPool.Recycling.PeriodicRestart.Time = [TimeSpan]::FromMinutes(1440)  # 24 horas
$appPool | Set-Item

Write-Host "  - Propiedades configuradas" -ForegroundColor Green
```

### 4.3 Crear Sitio Web

```powershell
$siteName = "SistemaVentas"
$port = 80
$appPoolName = "VentasWebPool"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"

Write-Host "`n[5/5] Creando sitio web..." -ForegroundColor Yellow

# Eliminar sitio anterior si existe
if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
    Stop-Website -Name $siteName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    Remove-Website -Name $siteName
    Write-Host "  - Sitio anterior eliminado" -ForegroundColor Gray
}

# Crear nuevo sitio
New-Website -Name $siteName `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port $port `
    -Force

Write-Host "  - Sitio web creado" -ForegroundColor Green

# Iniciar sitio
Start-Website -Name $siteName
Start-Sleep -Seconds 2

$state = Get-Website -Name $siteName | Select-Object -ExpandProperty State
Write-Host "  - Estado: $state" -ForegroundColor Green
```

### 4.4 Configurar Permisos NTFS

```powershell
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$appPoolName = "VentasWebPool"
$appPoolIdentity = "IIS AppPool\$appPoolName"

Write-Host "`n[6/6] Configurando permisos NTFS..." -ForegroundColor Yellow

# Obtener ACL actual
$acl = Get-Acl $publishPath

# Crear regla de acceso
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule(
    $appPoolIdentity,
    [System.Security.AccessControl.FileSystemRights]"Modify",
    [System.Security.AccessControl.InheritanceFlags]"ContainerInherit,ObjectInherit",
    [System.Security.AccessControl.PropagationFlags]"None",
    [System.Security.AccessControl.AccessControlType]"Allow"
)

# Agregar regla
$acl.AddAccessRule($accessRule)

# Aplicar ACL
Set-Acl $publishPath $acl

Write-Host "  - Permisos: $appPoolIdentity -> Modify" -ForegroundColor Green
Write-Host "  - Reciclaje aplicado (24 horas)" -ForegroundColor Green
```

### 4.5 Configurar Módulos IIS Necesarios

```powershell
Write-Host "`nConfigurando modulos IIS..." -ForegroundColor Yellow

$siteName = "SistemaVentas"

# Agregar módulos ASP.NET si no existen
$requiredModules = @(
    "AspNetCoreModuleV2",
    "ManagedEngine",
    "IsapiModule"
)

foreach ($module in $requiredModules) {
    $exists = Get-WebConfigurationProperty -Filter "system.webServer/modules" `
        -Name "." | Select-Object -ExpandProperty Collection | 
        Where-Object { $_.name -eq $module }
    
    if ($exists) {
        Write-Host "  ✓ Módulo $module ya habilitado" -ForegroundColor Green
    }
}
```

---

## 🧪 PASO 5: VERIFICAR DESPLIEGUE

### 5.1 Verificar Estado en IIS

```powershell
$siteName = "SistemaVentas"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   VERIFICANDO DESPLIEGUE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Estado del sitio
Write-Host "`nEstado del sitio:" -ForegroundColor Yellow
$site = Get-Website -Name $siteName
Write-Host "  Nombre: $($site.Name)" -ForegroundColor White
Write-Host "  Estado: $($site.State)" -ForegroundColor $(if ($site.State -eq "Started") { "Green" } else { "Red" })
Write-Host "  Puerto: $($site.Bindings[0].bindingInformation)" -ForegroundColor White

# Estado del Application Pool
Write-Host "`nEstado del Application Pool:" -ForegroundColor Yellow
$pool = Get-WebAppPoolState -Name "VentasWebPool"
Write-Host "  Estado: $($pool.Value)" -ForegroundColor $(if ($pool.Value -eq "Started") { "Green" } else { "Red" })

# Verificar permisos
Write-Host "`nVerificando permisos:" -ForegroundColor Yellow
$acl = Get-Acl "C:\inetpub\wwwroot\SistemaVentas"
$rules = $acl.Access | Where-Object { $_.IdentityReference -like "*IIS*" }
if ($rules) {
    Write-Host "  ✓ Permisos configurados para IIS AppPool" -ForegroundColor Green
} else {
    Write-Host "  ✗ Permisos NO encontrados" -ForegroundColor Red
}
```

### 5.2 Prueba en Navegador

```
1. Abre navegador
2. Ve a: http://localhost
3. Deberías ver la página de login

Si hay error:
- Verifica que el Application Pool está iniciado
- Revisa los logs en: C:\inetpub\logs\LogFiles\W3SVC1\
- Verifica conexión a SQL Server
```

### 5.3 Verificar Conexión a BD

```powershell
# Desde una PowerShell como Administrador
$connectionString = "Server=localhost;Database=DB_TIENDA;Integrated Security=true;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)

try {
    $connection.Open()
    Write-Host "✓ Conexión a BD exitosa" -ForegroundColor Green
    
    # Verificar que hay datos
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) as Clientes FROM Clientes"
    $result = $cmd.ExecuteScalar()
    Write-Host "  Clientes en BD: $result" -ForegroundColor Green
    
    $connection.Close()
} catch {
    Write-Host "✗ Error conectando a BD: $_" -ForegroundColor Red
}
```

---

## 🔐 PASO 6: SEGURIDAD EN PRODUCCIÓN

### 6.1 Configurar Web.config para Producción

```xml
<!-- En C:\inetpub\wwwroot\SistemaVentas\Web.config -->

<configuration>
  <!-- Configuración de debug -->
  <system.web>
    <!-- DEBUG DEBE SER FALSE EN PRODUCCIÓN -->
    <compilation debug="false" targetFramework="4.6" />
    <customErrors mode="On" defaultRedirect="~/Error/Index" />
    
    <!-- Seguridad -->
    <authentication mode="Forms">
      <forms loginUrl="~/Login/Index" timeout="30" />
    </authentication>
    
    <!-- Sesión -->
    <sessionState timeout="30" />
  </system.web>

  <!-- IIS -->
  <system.webServer>
    <security>
      <requestFiltering>
        <!-- Limitar tamaño de upload -->
        <requestLimits maxAllowedContentLength="104857600" /> <!-- 100 MB -->
      </requestFiltering>
    </security>
    
    <!-- Ocultar versión de IIS -->
    <httpProtocol>
      <customHeaders>
        <remove name="X-Powered-By" />
      </customHeaders>
    </httpProtocol>
  </system.webServer>

  <!-- Connection Strings -->
  <connectionStrings>
    <add name="DB_TIENDA" 
         connectionString="Server=SERVIDOR_SQL;Database=DB_TIENDA;User Id=usuario;Password=contraseña;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>

  <!-- App Settings PRODUCCIÓN -->
  <appSettings>
    <add key="webpages:Enabled" value="false" />
    <add key="webpages:Version" value="3.0.0.0" />
    <add key="PreserveLoginUrl" value="true" />
    <add key="EsProduccion" value="true" />
    <add key="TiempoSesion" value="30" />
  </appSettings>
</configuration>
```

### 6.2 Configurar HTTPS (SSL)

```powershell
# Para usar HTTPS, necesitas un certificado SSL
# Opción 1: Certificado autofirmado (para testing)
# Opción 2: Certificado de Let's Encrypt (gratuito)
# Opción 3: Certificado pagado

# Script para certificado autofirmado:
$cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My"
$thumbprint = $cert.Thumbprint

# Agregar binding HTTPS a IIS
New-WebBinding -Name "SistemaVentas" -IP "*" -Port 443 -Protocol https -CertificateThumbprint $thumbprint
```

### 6.3 Deshabilitar Métodos HTTP no Necesarios

```powershell
# Deshabilitar TRACE, TRACK (requiere IIS Manager o Web.config)
$site = Get-WebConfigurationProperty `
    -Filter "system.webServer/security/requestFiltering" `
    -Pspath "IIS:\Sites\SistemaVentas"
```

---

## 📊 PASO 7: MONITOREO EN PRODUCCIÓN

### 7.1 Verificar Logs

```powershell
# Logs de IIS
$logPath = "C:\inetpub\logs\LogFiles\W3SVC1\"
Get-ChildItem $logPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | 
    Get-Content -Tail 20

# Visor de eventos
Get-EventLog -LogName "Application" -Newest 10 -Source "ASP.NET" | 
    Select-Object TimeGenerated, EventID, Message
```

### 7.2 Configurar Reciclaje del Application Pool

```powershell
$appPoolName = "VentasWebPool"

# Configurar reciclaje cada 24 horas
Set-ItemProperty "IIS:\AppPools\$appPoolName\Recycling.PeriodicRestart" `
    -Name "Time" `
    -Value ([TimeSpan]::FromHours(24))

# También reciclaje por memoria (si excede 200 MB)
Set-ItemProperty "IIS:\AppPools\$appPoolName\Recycling.PeriodicRestart" `
    -Name "Memory" `
    -Value 204800

# Registrar evento de reciclaje
Set-ItemProperty "IIS:\AppPools\$appPoolName\Recycling" `
    -Name "LogEventOnRecycle" `
    -Value "Memory,Schedule"
```

### 7.3 Monitoreo de Rendimiento

```powershell
# Script para monitorear CPU y memoria del Application Pool
$appPoolName = "VentasWebPool"

while ($true) {
    Clear-Host
    Write-Host "Monitoreo - $(Get-Date -Format 'HH:mm:ss')" -ForegroundColor Cyan
    
    # CPU
    $cpu = Get-Process | Where-Object { $_.Name -like "*w3wp*" } | 
        Measure-Object -Property CPU -Sum | Select-Object -ExpandProperty Sum
    
    # Memoria
    $mem = Get-Process | Where-Object { $_.Name -like "*w3wp*" } | 
        Measure-Object -Property WorkingSet -Sum | Select-Object -ExpandProperty Sum | 
        ForEach-Object { $_ / 1MB }
    
    Write-Host "CPU: $($cpu)%" -ForegroundColor Yellow
    Write-Host "Memoria: $($mem:F2) MB" -ForegroundColor Yellow
    
    Start-Sleep -Seconds 5
}
```

---

## ✅ CHECKLIST FINAL

- [ ] SQL Server DB_TIENDA accesible y con datos de producción
- [ ] FiscalAPI configurado en modo PRODUCCIÓN (EsProduccion = 1)
- [ ] X-API-KEY actualizada a valor de producción
- [ ] X-TENANT-KEY actualizada a valor de producción
- [ ] Solución compilada en modo Release (sin Debug)
- [ ] Archivos copiados a C:\inetpub\wwwroot\SistemaVentas\
- [ ] Application Pool "VentasWebPool" creado y en estado "Started"
- [ ] Sitio "SistemaVentas" creado en IIS y en estado "Started"
- [ ] Permisos NTFS configurados para IIS AppPool
- [ ] Web.config con debug="false"
- [ ] SSL/HTTPS configurado (opcional pero recomendado)
- [ ] Acceso a http://localhost funciona sin errores
- [ ] Login funciona correctamente
- [ ] Facturación funcionando con FiscalAPI producción
- [ ] Monitoreo configurado

---

## 🚨 TROUBLESHOOTING

### Error: "HTTP Error 500"
```powershell
# Causa: Generalmente compilación con Debug o permisos
# Solución:
# 1. Verifica que Web.config tiene debug="false"
# 2. Verifica permisos del Application Pool
# 3. Revisa logs en C:\inetpub\logs\LogFiles\W3SVC1\
```

### Error: "Application Pool stopped"
```powershell
# Causa: Excepción en código o falta de dependencias
# Solución:
Start-WebAppPool -Name "VentasWebPool"
Get-EventLog -LogName "Application" -Newest 5 | Select-Object Message
```

### Error: "Cannot connect to database"
```powershell
# Verifica connection string en Web.config
# Verifica que SQL Server está ejecutándose
# Verifica permisos de usuario SQL
sqlcmd -S localhost -Q "SELECT @@VERSION"
```

### Error: "FiscalAPI test.fiscalapi.com timeout"
```powershell
# Verifica que EsProduccion = 1 en ConfiguracionPAC
SELECT EsProduccion FROM ConfiguracionPAC;
# Verifica X-API-KEY y X-TENANT-KEY
# Verifica que son valores de PRODUCCIÓN, no TEST
```

---

## 📞 SOPORTE

Si encuentras problemas:

1. **Revisa los logs**: `C:\inetpub\logs\LogFiles\W3SVC1\`
2. **Visor de eventos**: `eventvwr.msc`
3. **IIS Manager**: `inetmgr`
4. **Verificación de BD**: `sqlcmd -S localhost -d DB_TIENDA -Q "SELECT COUNT(*) FROM Ventas"`

---

**¡Sistema listo para PRODUCCIÓN!** 🎉
