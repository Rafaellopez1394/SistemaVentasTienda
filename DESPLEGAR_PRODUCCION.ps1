# Script de compilación y despliegue automático a IIS Productivo
# Ejecutar como Administrador
# Uso: .\DESPLEGAR_PRODUCCION.ps1

$ErrorActionPreference = "Stop"

# ========================================
# CONFIGURACIÓN
# ========================================
$projectPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
$solutionFile = "VentasWeb.sln"
$sourceWebPath = "$projectPath\VentasWeb"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$siteName = "SistemaVentas"
$appPoolName = "VentasWebPool"
$port = 80
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

# ========================================
# FUNCIONES AUXILIARES
# ========================================
function Write-Header {
    param([string]$message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "   $message" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Write-Step {
    param([string]$message)
    Write-Host "`n$message" -ForegroundColor Yellow
}

function Write-SuccessMessage {
    param([string]$message)
    Write-Host "✓ $message" -ForegroundColor Green
}

function Write-ErrorMessage {
    param([string]$message)
    Write-Host "✗ $message" -ForegroundColor Red
}

function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Write-ErrorMessage "Este script debe ejecutarse como Administrador"
        exit 1
    }
    Write-SuccessMessage "Ejecutando como Administrador"
}

function Test-Prerequisites {
    Write-Step "Verificando pre-requisitos..."
    
    if (-not (Test-Path $msbuild)) {
        Write-ErrorMessage "MSBuild no encontrado en: $msbuild"
        exit 1
    }
    Write-SuccessMessage "MSBuild encontrado"
    
    if (-not (Test-Path $projectPath)) {
        Write-ErrorMessage "Carpeta de proyecto no encontrada: $projectPath"
        exit 1
    }
    Write-SuccessMessage "Carpeta de proyecto encontrada"
    
    # Verificar módulo WebAdministration
    if (-not (Get-Module -ListAvailable WebAdministration)) {
        Write-ErrorMessage "WebAdministration module no disponible"
        exit 1
    }
    Import-Module WebAdministration
    Write-SuccessMessage "WebAdministration module cargado"
}

function Clear-Solution {
    Write-Step "[1/7] Limpiando compilaciones anteriores..."
    
    $binFolders = @(
        "$sourceWebPath\bin",
        "$sourceWebPath\obj",
        "$projectPath\CapaDatos\bin",
        "$projectPath\CapaDatos\obj",
        "$projectPath\CapaModelo\bin",
        "$projectPath\CapaModelo\obj"
    )
    
    foreach ($folder in $binFolders) {
        if (Test-Path $folder) {
            Remove-Item $folder -Recurse -Force -ErrorAction SilentlyContinue
            Write-Host "  - Eliminada: $folder" -ForegroundColor Gray
        }
    }
    Write-SuccessMessage "Carpetas limpias"
}

function Invoke-Build {
    Write-Step "[2/7] Compilando solución en modo Release..."
    
    Set-Location $projectPath
    
    $solutionPath = "$projectPath\$solutionFile"
    
    Write-Host "  Compilando: $solutionPath" -ForegroundColor Gray
    
    & $msbuild $solutionPath `
        /t:Clean,Rebuild `
        /p:Configuration=Release `
        /p:DebugSymbols=false `
        /p:DebugType=None `
        /v:minimal `
        /nologo
    
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Error durante la compilación"
        exit 1
    }
    Write-SuccessMessage "Compilación exitosa"
}

function Test-BuildArtifacts {
    Write-Step "Verificando artefactos compilados..."
    
    $dllFiles = @(
        "$sourceWebPath\bin\VentasWeb.dll",
        "$projectPath\CapaDatos\bin\CapaDatos.dll",
        "$projectPath\CapaModelo\bin\CapaModelo.dll"
    )
    
    foreach ($dll in $dllFiles) {
        if (Test-Path $dll) {
            $size = [math]::Round((Get-Item $dll).Length / 1MB, 2)
            Write-Host "  ✓ $(Split-Path $dll -Leaf) - $size MB" -ForegroundColor Green
        } else {
            Write-ErrorMessage "DLL no encontrado: $dll"
            exit 1
        }
    }
}

function Stop-IISSite {
    Write-Step "[3/7] Deteniendo IIS..."
    
    if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
        Stop-Website -Name $siteName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
        Write-Host "  - Sitio detenido" -ForegroundColor Gray
    }
    
    Write-SuccessMessage "IIS detenido"
}

function Initialize-PublishFolder {
    Write-Step "[4/7] Preparando carpeta de publicación..."
    
    if (Test-Path $publishPath) {
        Write-Host "  - Limpiando: $publishPath" -ForegroundColor Gray
        Get-ChildItem "$publishPath\*" -Recurse -Force -ErrorAction SilentlyContinue | 
            Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
        Start-Sleep -Milliseconds 500
    } else {
        New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
        Write-Host "  - Carpeta creada: $publishPath" -ForegroundColor Gray
    }
    
    Write-SuccessMessage "Carpeta preparada"
}

function Copy-PublishFiles {
    Write-Step "[5/7] Copiando archivos..."
    
    # Crear estructura de directorios
    $directories = @("bin", "Content", "Scripts", "Views", "fonts")
    
    foreach ($dir in $directories) {
        $sourcePath = "$sourceWebPath\$dir"
        if (Test-Path $sourcePath) {
            $null = New-Item -ItemType Directory -Path "$publishPath\$dir" -Force -ErrorAction SilentlyContinue
            Write-Host "  - Copiando $dir/" -ForegroundColor Gray
            Copy-Item "$sourcePath\*" -Destination "$publishPath\$dir" -Recurse -Force
        }
    }
    
    # Copiar archivos raíz
    Write-Host "  - Copiando archivos raíz" -ForegroundColor Gray
    Copy-Item "$sourceWebPath\Web.config" -Destination $publishPath -Force
    Copy-Item "$sourceWebPath\Global.asax" -Destination $publishPath -Force
    
    if (Test-Path "$sourceWebPath\favicon.ico") {
        Copy-Item "$sourceWebPath\favicon.ico" -Destination $publishPath -Force
    }
    
    Write-SuccessMessage "Archivos copiados"
}

function Test-CriticalFiles {
    Write-Step "Verificando archivos críticos..."
    
    $criticalFiles = @(
        "$publishPath\bin\VentasWeb.dll",
        "$publishPath\bin\CapaDatos.dll",
        "$publishPath\bin\CapaModelo.dll",
        "$publishPath\Web.config",
        "$publishPath\Global.asax"
    )
    
    $allOk = $true
    foreach ($file in $criticalFiles) {
        if (Test-Path $file) {
            Write-Host "  ✓ $(Split-Path $file -Leaf)" -ForegroundColor Green
        } else {
            Write-ErrorMessage "Falta: $(Split-Path $file -Leaf)"
            $allOk = $false
        }
    }
    
    if (-not $allOk) {
        Write-ErrorMessage "Faltan archivos críticos"
        exit 1
    }
}

function Set-IISConfiguration {
    Write-Step "[6/7] Configurando IIS..."
    
    # Configurar Application Pool
    Write-Host "  - Configurando Application Pool" -ForegroundColor Gray
    
    if (Test-Path "IIS:\AppPools\$appPoolName") {
        Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 1
        Remove-WebAppPool -Name $appPoolName
    }
    
    New-WebAppPool -Name $appPoolName
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
    
    # Configurar reciclaje
    $appPool = Get-Item "IIS:\AppPools\$appPoolName"
    $appPool.Recycling.PeriodicRestart.Time = [TimeSpan]::FromMinutes(1440)
    $appPool | Set-Item
    
    Write-Host "  - Application Pool configurado" -ForegroundColor Gray
    
    # Crear sitio web
    Write-Host "  - Creando sitio web" -ForegroundColor Gray
    
    if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
        Remove-Website -Name $siteName
    }
    
    New-Website -Name $siteName `
        -PhysicalPath $publishPath `
        -ApplicationPool $appPoolName `
        -Port $port `
        -Force | Out-Null
    
    Write-Host "  - Sitio creado" -ForegroundColor Gray
    
    # Configurar permisos NTFS
    Write-Host "  - Configurando permisos NTFS" -ForegroundColor Gray
    
    $acl = Get-Acl $publishPath
    $appPoolIdentity = "IIS AppPool\$appPoolName"
    
    # Verificar si ya existe
    $existing = $acl.Access | Where-Object { $_.IdentityReference -like "*$appPoolName*" }
    if (-not $existing) {
        $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule(
            $appPoolIdentity,
            [System.Security.AccessControl.FileSystemRights]"Modify",
            [System.Security.AccessControl.InheritanceFlags]"ContainerInherit,ObjectInherit",
            [System.Security.AccessControl.PropagationFlags]"None",
            [System.Security.AccessControl.AccessControlType]"Allow"
        )
        $acl.AddAccessRule($accessRule)
        Set-Acl $publishPath $acl
    }
    
    Write-Host "  - Permisos configurados" -ForegroundColor Gray
    
    Write-SuccessMessage "IIS configurado"
}

function Start-IISSite {
    Write-Step "[7/7] Iniciando sitio web..."
    
    Start-Website -Name $siteName
    Start-Sleep -Seconds 2
    
    $state = (Get-Website -Name $siteName).State
    if ($state -eq "Started") {
        Write-SuccessMessage "Sitio iniciado correctamente"
    } else {
        Write-ErrorMessage "El sitio no se inició: $state"
        exit 1
    }
}

function Write-Summary {
    Write-Header "DESPLIEGUE COMPLETADO EXITOSAMENTE"
    
    Write-Host "`nDetalles del despliegue:" -ForegroundColor Yellow
    Write-Host "  Sitio web: $siteName" -ForegroundColor White
    Write-Host "  URL: http://localhost:$port" -ForegroundColor Cyan
    Write-Host "  Carpeta: $publishPath" -ForegroundColor White
    Write-Host "  Application Pool: $appPoolName" -ForegroundColor White
    Write-Host "  Estado: $(Get-Website -Name $siteName | Select-Object -ExpandProperty State)" -ForegroundColor Green
    
    Write-Host "`nProximos pasos:" -ForegroundColor Yellow
    Write-Host "  1. Verifica en navegador: http://localhost" -ForegroundColor White
    Write-Host "  2. Verifica logs: C:\inetpub\logs\LogFiles\W3SVC1\" -ForegroundColor White
    Write-Host "  3. Verifica BD: SQL Server con DB_TIENDA" -ForegroundColor White
    Write-Host "  4. Verifica FiscalAPI: Debe estar en PRODUCCIÓN" -ForegroundColor White
    
    Write-Host "`n========================================`n" -ForegroundColor Cyan
}

# ========================================
# EJECUCIÓN PRINCIPAL
# ========================================
try {
    Write-Header "DESPLIEGUE A IIS PRODUCTIVO"
    
    Check-Administrator
    Check-Prerequisites
    Clean-Solution
    Build-Solution
    Verify-BuildArtifacts
    Stop-IISSite
    Prepare-PublishFolder
    Copy-PublishFiles
    Verify-CriticalFiles
    Configure-IIS
    Start-IISSite
    Print-Summary
    
} catch {
    Write-ErrorMessage "Error: $_"
    exit 1
}

# Mantener ventana abierta
Write-Host "Presiona Enter para cerrar..." -ForegroundColor Gray
Read-Host
