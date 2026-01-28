# 🔧 PASOS MANUALES: COMPILACIÓN Y DESPLIEGUE (Si no quieres usar script)

Si prefieres hacer TODO manualmente paso a paso, aquí están los comandos exactos.

---

## PASO 1: PREPARAR EL ENTORNO

### 1.1 Abrir PowerShell como Administrador
```powershell
# Presiona: Win + X → Selecciona "Windows PowerShell (Admin)"

# Navega a la carpeta del proyecto
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

# Verifica que estás en la carpeta correcta
Get-Location
# Resultado: C:\Users\Rafael Lopez\Documents\SistemaVentasTienda
```

### 1.2 Limpiar Compilaciones Anteriores
```powershell
Write-Host "Limpiando compilaciones anteriores..." -ForegroundColor Yellow

# Eliminar carpetas bin y obj
Remove-Item "VentasWeb\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "VentasWeb\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "CapaDatos\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "CapaDatos\obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "CapaModelo\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "CapaModelo\obj" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Limpiezas completadas" -ForegroundColor Green
```

---

## PASO 2: COMPILAR SOLUCIÓN EN MODO RELEASE

### 2.1 Compilación
```powershell
Write-Host "Compilando solución..." -ForegroundColor Yellow

$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
$solutionFile = "VentasWeb.sln"

# Compilar
& $msbuild $solutionFile `
    /t:Clean,Rebuild `
    /p:Configuration=Release `
    /p:DebugSymbols=false `
    /p:DebugType=None `
    /v:minimal

# Verificar que fue exitoso
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "✗ Error en compilación" -ForegroundColor Red
    exit 1
}
```

### 2.2 Verificar Archivos Compilados
```powershell
$files = @(
    "VentasWeb\bin\VentasWeb.dll",
    "CapaDatos\bin\CapaDatos.dll",
    "CapaModelo\bin\CapaModelo.dll"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $size = (Get-Item $file).Length / 1MB
        Write-Host "✓ $(Split-Path $file -Leaf) - ${size:F2} MB" -ForegroundColor Green
    } else {
        Write-Host "✗ $(Split-Path $file -Leaf) NO ENCONTRADO" -ForegroundColor Red
    }
}
```

---

## PASO 3: DETENER IIS

### 3.1 Detener Sitio Existente (Si existe)
```powershell
Import-Module WebAdministration

$siteName = "SistemaVentas"

# Si el sitio ya existe, detenerlo
if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
    Write-Host "Deteniendo sitio existente..." -ForegroundColor Yellow
    Stop-Website -Name $siteName
    Start-Sleep -Seconds 2
    Write-Host "✓ Sitio detenido" -ForegroundColor Green
}
```

---

## PASO 4: PREPARAR CARPETA DE PUBLICACIÓN

### 4.1 Crear/Limpiar Carpeta
```powershell
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"

Write-Host "Preparando carpeta de publicación..." -ForegroundColor Yellow

# Crear o limpiar
if (Test-Path $publishPath) {
    Write-Host "Limpiando: $publishPath" -ForegroundColor Gray
    Get-ChildItem "$publishPath\*" -Recurse -Force | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
} else {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}

Write-Host "✓ Carpeta lista" -ForegroundColor Green
```

---

## PASO 5: COPIAR ARCHIVOS

### 5.1 Copiar bin (Compilados)
```powershell
Write-Host "Copiando archivos..." -ForegroundColor Yellow

$null = New-Item -ItemType Directory -Path "$publishPath\bin" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\bin\*" -Destination "$publishPath\bin" -Recurse -Force
Write-Host "  ✓ bin/" -ForegroundColor Green
```

### 5.2 Copiar Content (CSS, Imágenes)
```powershell
$null = New-Item -ItemType Directory -Path "$publishPath\Content" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Content\*" -Destination "$publishPath\Content" -Recurse -Force
Write-Host "  ✓ Content/" -ForegroundColor Green
```

### 5.3 Copiar Scripts (JavaScript)
```powershell
$null = New-Item -ItemType Directory -Path "$publishPath\Scripts" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Scripts\*" -Destination "$publishPath\Scripts" -Recurse -Force
Write-Host "  ✓ Scripts/" -ForegroundColor Green
```

### 5.4 Copiar Views (Vistas MVC)
```powershell
$null = New-Item -ItemType Directory -Path "$publishPath\Views" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Views\*" -Destination "$publishPath\Views" -Recurse -Force
Write-Host "  ✓ Views/" -ForegroundColor Green
```

### 5.5 Copiar Fonts (Si existen)
```powershell
if (Test-Path "$sourcePath\fonts") {
    $null = New-Item -ItemType Directory -Path "$publishPath\fonts" -Force -ErrorAction SilentlyContinue
    Copy-Item "$sourcePath\fonts\*" -Destination "$publishPath\fonts" -Recurse -Force
    Write-Host "  ✓ fonts/" -ForegroundColor Green
}
```

### 5.6 Copiar Archivos Raíz
```powershell
Copy-Item "$sourcePath\Web.config" -Destination $publishPath -Force
Copy-Item "$sourcePath\Global.asax" -Destination $publishPath -Force
Write-Host "  ✓ Web.config y Global.asax" -ForegroundColor Green

if (Test-Path "$sourcePath\favicon.ico") {
    Copy-Item "$sourcePath\favicon.ico" -Destination $publishPath -Force
    Write-Host "  ✓ favicon.ico" -ForegroundColor Green
}

Write-Host "✓ Todos los archivos copiados" -ForegroundColor Green
```

---

## PASO 6: CONFIGURAR IIS

### 6.1 Crear Application Pool
```powershell
$appPoolName = "VentasWebPool"

Write-Host "Configurando IIS..." -ForegroundColor Yellow

# Eliminar Application Pool anterior si existe
if (Test-Path "IIS:\AppPools\$appPoolName") {
    Write-Host "Eliminando Application Pool anterior..." -ForegroundColor Gray
    Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    Remove-WebAppPool -Name $appPoolName
}

# Crear nuevo Application Pool
Write-Host "Creando Application Pool: $appPoolName" -ForegroundColor Gray
New-WebAppPool -Name $appPoolName

# Configurar propiedades
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"

Write-Host "✓ Application Pool creado" -ForegroundColor Green
```

### 6.2 Configurar Reciclaje del Application Pool
```powershell
Write-Host "Configurando reciclaje (24 horas)..." -ForegroundColor Gray

$appPool = Get-Item "IIS:\AppPools\$appPoolName"
$appPool.Recycling.PeriodicRestart.Time = [TimeSpan]::FromMinutes(1440)
$appPool | Set-Item

Write-Host "✓ Reciclaje configurado" -ForegroundColor Green
```

### 6.3 Crear Sitio Web
```powershell
$siteName = "SistemaVentas"
$port = 80
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"

Write-Host "Creando sitio web: $siteName" -ForegroundColor Gray

# Eliminar sitio anterior si existe
if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
    Remove-Website -Name $siteName
}

# Crear nuevo sitio
New-Website -Name $siteName `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port $port `
    -Force | Out-Null

Write-Host "✓ Sitio web creado" -ForegroundColor Green
```

### 6.4 Configurar Permisos NTFS
```powershell
Write-Host "Configurando permisos NTFS..." -ForegroundColor Gray

$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$appPoolName = "VentasWebPool"
$appPoolIdentity = "IIS AppPool\$appPoolName"

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
Set-Acl $publishPath $acl

Write-Host "✓ Permisos configurados: $appPoolIdentity -> Modify" -ForegroundColor Green
```

---

## PASO 7: INICIAR IIS

### 7.1 Iniciar Sitio Web
```powershell
$siteName = "SistemaVentas"

Write-Host "Iniciando sitio web..." -ForegroundColor Yellow

Start-Website -Name $siteName
Start-Sleep -Seconds 2

# Verificar estado
$state = (Get-Website -Name $siteName).State
if ($state -eq "Started") {
    Write-Host "✓ Sitio iniciado" -ForegroundColor Green
} else {
    Write-Host "✗ Error: Sitio en estado $state" -ForegroundColor Red
    exit 1
}
```

---

## PASO 8: VERIFICAR DESPLIEGUE

### 8.1 Verificar Archivos Críticos
```powershell
Write-Host "Verificando archivos críticos..." -ForegroundColor Yellow

$criticalFiles = @(
    "C:\inetpub\wwwroot\SistemaVentas\bin\VentasWeb.dll",
    "C:\inetpub\wwwroot\SistemaVentas\bin\CapaDatos.dll",
    "C:\inetpub\wwwroot\SistemaVentas\Web.config",
    "C:\inetpub\wwwroot\SistemaVentas\Global.asax"
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

if ($allOk) {
    Write-Host "✓ Todos los archivos están presentes" -ForegroundColor Green
} else {
    Write-Host "✗ Faltan archivos críticos" -ForegroundColor Red
    exit 1
}
```

### 8.2 Verificar Estado en IIS
```powershell
Write-Host "`nResumen del despliegue:" -ForegroundColor Yellow

$site = Get-Website -Name "SistemaVentas"
Write-Host "  Sitio: $($site.Name)" -ForegroundColor White
Write-Host "  Estado: $($site.State)" -ForegroundColor Green
Write-Host "  URL: http://localhost" -ForegroundColor Cyan

$pool = Get-WebAppPoolState -Name "VentasWebPool"
Write-Host "  Application Pool: $($pool.Value)" -ForegroundColor Green

Write-Host "`n✓ DESPLIEGUE COMPLETADO" -ForegroundColor Green
Write-Host "  Accede en navegador: http://localhost" -ForegroundColor Cyan
```

---

## PASO 9: VERIFICAR EN NAVEGADOR

### 9.1 Abrir Navegador
```
1. Abre: http://localhost
2. Deberías ver:
   - Página de login
   - Campos: Usuario y Contraseña
   - Botón: Iniciar Sesión

3. Inicia sesión con credenciales
```

### 9.2 Verificar Funcionalidad
```
1. Navega a Facturación
2. Intenta crear una factura
3. Verifica que FiscalAPI está en PRODUCCIÓN:
   - URL debe ser: https://api.fiscalapi.com
   - NO debe ser: https://test.fiscalapi.com
```

---

## PASO 10: VERIFICAR CONFIGURACIÓN

### 10.1 Verificar Base de Datos
```powershell
# Verifica que estés usando PRODUCCIÓN
$connectionString = "Server=localhost;Database=DB_TIENDA;Integrated Security=true;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

$cmd = $connection.CreateCommand()
$cmd.CommandText = "SELECT EsProduccion, LEFT(ApiKey, 20) AS ApiKey, Tenant FROM ConfiguracionPAC WHERE ConfigPACID = 1"

$adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
$dataSet = New-Object System.Data.DataSet
$adapter.Fill($dataSet) | Out-Null

if ($dataSet.Tables[0].Rows.Count -gt 0) {
    $row = $dataSet.Tables[0].Rows[0]
    Write-Host "EsProduccion: $($row['EsProduccion'])" -ForegroundColor Yellow
    Write-Host "ApiKey: $($row['ApiKey'])..." -ForegroundColor Yellow
    Write-Host "Tenant: $($row['Tenant'])" -ForegroundColor Yellow
}

$connection.Close()
```

---

## RESUMEN DE COMANDOS

Si quieres copiar y pegar TODO de una vez:

```powershell
# 1. LIMPIAR
$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

Remove-Item "VentasWeb\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "VentasWeb\obj" -Recurse -Force -ErrorAction SilentlyContinue

# 2. COMPILAR
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
& $msbuild "VentasWeb.sln" /t:Clean,Rebuild /p:Configuration=Release /p:DebugSymbols=false

# 3. DETENER IIS
Import-Module WebAdministration
Stop-Website -Name "SistemaVentas" -ErrorAction SilentlyContinue

# 4. PREPARAR CARPETA
Get-ChildItem "$publishPath\*" -Recurse -Force | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue

# 5. COPIAR ARCHIVOS
Copy-Item "$sourcePath\bin" -Destination "$publishPath\bin" -Recurse -Force
Copy-Item "$sourcePath\Content" -Destination "$publishPath\Content" -Recurse -Force
Copy-Item "$sourcePath\Scripts" -Destination "$publishPath\Scripts" -Recurse -Force
Copy-Item "$sourcePath\Views" -Destination "$publishPath\Views" -Recurse -Force
Copy-Item "$sourcePath\Web.config" -Destination $publishPath -Force
Copy-Item "$sourcePath\Global.asax" -Destination $publishPath -Force

# 6. CONFIGURAR IIS
$appPoolName = "VentasWebPool"
$siteName = "SistemaVentas"

Remove-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
New-WebAppPool -Name $appPoolName
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"

Remove-Website -Name $siteName -ErrorAction SilentlyContinue
New-Website -Name $siteName -PhysicalPath $publishPath -ApplicationPool $appPoolName -Port 80 -Force

# 7. PERMISOS
$acl = Get-Acl $publishPath
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule(
    "IIS AppPool\$appPoolName",
    "Modify",
    "ContainerInherit,ObjectInherit",
    "None",
    "Allow"
)
$acl.AddAccessRule($accessRule)
Set-Acl $publishPath $acl

# 8. INICIAR
Start-Website -Name $siteName

Write-Host "✓ LISTO EN: http://localhost" -ForegroundColor Green
```

---

## VERIFICACIÓN RÁPIDA

```powershell
# Status del sitio
Get-Website -Name "SistemaVentas" | Select-Object Name, State

# Status del pool
Get-WebAppPoolState -Name "VentasWebPool"

# Abrir navegador
Start-Process "http://localhost"
```

---

**¿Problemas?** Revisar [COMPILAR_Y_DESPLEGAR_PRODUCCION.md](COMPILAR_Y_DESPLEGAR_PRODUCCION.md) sección Troubleshooting.
