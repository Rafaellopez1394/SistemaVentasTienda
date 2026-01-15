#Requires -RunAsAdministrator
# ============================================================================
# INSTALADOR COMPLETO - Sistema Ventas
# ============================================================================

Write-Host ""
Write-Host "========================================"
Write-Host "  INSTALADOR SISTEMA VENTAS" -ForegroundColor Cyan
Write-Host "========================================"
Write-Host ""

$ErrorActionPreference = "Stop"
$installerPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$webSourcePath = Join-Path $installerPath "Web"
$targetPath = "C:\SistemaVentas"

# ============================================================================
# VERIFICACIONES PREVIAS
# ============================================================================

Write-Host "[Verificando requisitos...]" -ForegroundColor Yellow

# Verificar carpeta Web
if (-not (Test-Path $webSourcePath)) {
    Write-Host "ERROR: No se encuentra la carpeta Web en $installerPath" -ForegroundColor Red
    Write-Host "Asegurese de ejecutar este script desde la carpeta del instalador" -ForegroundColor Red
    Read-Host "Presione Enter para salir"
    exit 1
}

# Verificar IIS instalado
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "  OK - IIS instalado" -ForegroundColor Green
} catch {
    Write-Host "ERROR: IIS no esta instalado" -ForegroundColor Red
    Write-Host "Instale IIS desde 'Activar o desactivar las caracteristicas de Windows'" -ForegroundColor Red
    Read-Host "Presione Enter para salir"
    exit 1
}

Write-Host ""

# ============================================================================
# PASO 1: LIMPIAR INSTALACION ANTERIOR
# ============================================================================

Write-Host "[1/7] Limpiando instalacion anterior..." -ForegroundColor Yellow

# Detener IIS
Write-Host "  Deteniendo IIS..."
Stop-Service W3SVC -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Eliminar sitio anterior
if (Test-Path "IIS:\Sites\SistemaVentas") {
    Write-Host "  Eliminando sitio anterior..."
    & "$env:SystemRoot\system32\inetsrv\appcmd.exe" delete site "SistemaVentas" 2>&1 | Out-Null
    Start-Sleep -Seconds 1
}

# Eliminar pool anterior
if (Test-Path "IIS:\AppPools\VentasWebPool") {
    Write-Host "  Eliminando pool anterior..."
    & "$env:SystemRoot\system32\inetsrv\appcmd.exe" delete apppool "VentasWebPool" 2>&1 | Out-Null
    Start-Sleep -Seconds 1
}

# Detener Default Web Site para liberar puerto 80
if (Test-Path "IIS:\Sites\Default Web Site") {
    Write-Host "  Deteniendo Default Web Site..."
    try {
        & "$env:SystemRoot\system32\inetsrv\appcmd.exe" stop site "Default Web Site" 2>&1 | Out-Null
        Write-Host "    Default Web Site detenido" -ForegroundColor Green
    } catch {
        Write-Host "    (Continuando...)" -ForegroundColor Yellow
    }
}

Write-Host "  Limpieza completada" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 2: COPIAR ARCHIVOS
# ============================================================================

Write-Host "[2/7] Copiando archivos a $targetPath..." -ForegroundColor Yellow

# Eliminar carpeta anterior si existe
if (Test-Path $targetPath) {
    Write-Host "  Eliminando carpeta anterior..."
    Remove-Item -Path $targetPath -Recurse -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

# Crear carpeta destino
Write-Host "  Creando carpeta destino..."
New-Item -Path $targetPath -ItemType Directory -Force | Out-Null

# Copiar archivos
Write-Host "  Copiando archivos (esto puede tomar un minuto)..."
Copy-Item -Path "$webSourcePath\*" -Destination $targetPath -Recurse -Force

# Verificar archivos criticos
$criticalFiles = @(
    "Web.config",
    "Global.asax",
    "bin\VentasWeb.dll",
    "bin\roslyn\csc.exe"
)

$allFilesOK = $true
foreach ($file in $criticalFiles) {
    $fullPath = Join-Path $targetPath $file
    if (Test-Path $fullPath) {
        Write-Host "    OK - $file" -ForegroundColor Green
    } else {
        Write-Host "    ERROR - Falta: $file" -ForegroundColor Red
        $allFilesOK = $false
    }
}

if (-not $allFilesOK) {
    Write-Host "ERROR: Faltan archivos criticos" -ForegroundColor Red
    Read-Host "Presione Enter para salir"
    exit 1
}

Write-Host "  Archivos copiados correctamente" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 3: REGISTRAR ASP.NET EN IIS
# ============================================================================

Write-Host "[3/8] Registrando ASP.NET 4.x en IIS..." -ForegroundColor Yellow

try {
    $aspnetRegiis = "$env:windir\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe"
    if (Test-Path $aspnetRegiis) {
        & $aspnetRegiis -i 2>&1 | Out-Null
        Write-Host "  ASP.NET registrado" -ForegroundColor Green
    } else {
        Write-Host "  ADVERTENCIA: aspnet_regiis.exe no encontrado" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  (Continuando...)" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# PASO 4: DESBLOQUEAR CONFIGURACION IIS
# ============================================================================

Write-Host "[4/8] Desbloqueando configuracion IIS..." -ForegroundColor Yellow

& "$env:SystemRoot\system32\inetsrv\appcmd.exe" unlock config -section:system.webServer/modules 2>&1 | Out-Null
& "$env:SystemRoot\system32\inetsrv\appcmd.exe" unlock config -section:system.webServer/handlers 2>&1 | Out-Null

Write-Host "  Configuracion desbloqueada" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 5: CREAR APPLICATION POOL
# ============================================================================

Write-Host "[5/8] Creando Application Pool..." -ForegroundColor Yellow

& "$env:SystemRoot\system32\inetsrv\appcmd.exe" add apppool /name:"VentasWebPool" /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated" 2>&1 | Out-Null

Write-Host "  Application Pool creado" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 6: CONFIGURAR PERMISOS
# ============================================================================

Write-Host "[6/8] Configurando permisos..." -ForegroundColor Yellow

$accounts = @("IIS_IUSRS", "IUSR", "IIS APPPOOL\VentasWebPool")

foreach ($account in $accounts) {
    Write-Host "  Configurando permisos para $account..."
    try {
        icacls $targetPath /grant "${account}:(OI)(CI)F" /T /Q 2>&1 | Out-Null
    } catch {
        Write-Host "    (advertencia ignorada)" -ForegroundColor Yellow
    }
}

Write-Host "  Permisos configurados" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 7: CREAR SITIO WEB
# ============================================================================

Write-Host "[7/8] Creando sitio web..." -ForegroundColor Yellow

& "$env:SystemRoot\system32\inetsrv\appcmd.exe" add site /name:"SistemaVentas" /physicalPath:"$targetPath" /bindings:http/*:80: 2>&1 | Out-Null
& "$env:SystemRoot\system32\inetsrv\appcmd.exe" set app "SistemaVentas/" /applicationPool:"VentasWebPool" 2>&1 | Out-Null

Write-Host "  Sitio web creado" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 8: INICIAR SERVICIOS
# ============================================================================

Write-Host "[8/8] Iniciando servicios..." -ForegroundColor Yellow

& "$env:SystemRoot\system32\inetsrv\appcmd.exe" start site "SistemaVentas" 2>&1 | Out-Null
Start-Service W3SVC -ErrorAction SilentlyContinue

Write-Host "  Servicios iniciados" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 9: VERIFICAR CONFIGURACION
# ============================================================================

Write-Host "[9/9] Verificando configuracion..." -ForegroundColor Yellow

$siteInfo = Get-Website -Name "SistemaVentas"
$poolInfo = Get-WebAppPoolState -Name "VentasWebPool"

Write-Host ""
Write-Host "  Nombre sitio: $($siteInfo.Name)" -ForegroundColor Cyan
Write-Host "  Estado: $($siteInfo.State)" -ForegroundColor Cyan
Write-Host "  Ruta fisica: $($siteInfo.PhysicalPath)" -ForegroundColor Cyan
Write-Host "  Puerto: 80" -ForegroundColor Cyan
Write-Host "  Pool: VentasWebPool ($($poolInfo.Value))" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# INSTALACION COMPLETADA
# ============================================================================

Write-Host "========================================"
Write-Host "  INSTALACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================"
Write-Host ""
Write-Host "Sistema instalado en: $targetPath" -ForegroundColor Green
Write-Host "URL: http://localhost" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "  CONFIGURACION PENDIENTE" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANTE: Configure la cadena de conexion en Web.config"
Write-Host ""
Write-Host "1. Abra: $targetPath\Web.config"
Write-Host ""
Write-Host "2. Busque la seccion <connectionStrings>"
Write-Host ""
Write-Host "3. Actualice:"
Write-Host "   - Data Source: [NOMBRE_SERVIDOR_SQL]"
Write-Host "   - Initial Catalog: DB_TIENDA"
Write-Host "   - User ID: [USUARIO]"
Write-Host "   - Password: [CONTRASENA]"
Write-Host ""
Write-Host "4. Guarde y ejecute: iisreset"
Write-Host ""
Write-Host "5. Abra: http://localhost"
Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

Read-Host "Presione Enter para salir"
