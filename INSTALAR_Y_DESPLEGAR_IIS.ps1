# =============================================================================
# INSTALAR IIS Y DESPLEGAR SISTEMA DE VENTAS
# =============================================================================
# IMPORTANTE: Ejecutar este script como ADMINISTRADOR
# Clic derecho -> Ejecutar con PowerShell como Administrador

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "INSTALACIÓN Y DESPLIEGUE EN IIS" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Verificar privilegios de administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Este script requiere privilegios de administrador" -ForegroundColor Red
    Write-Host "Por favor, ejecuta PowerShell como Administrador y vuelve a intentarlo" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

# ====================
# [1/5] Verificar IIS
# ====================
Write-Host "[1/5] Verificando instalación de IIS..." -ForegroundColor Yellow

try {
    $iisFeature = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -ErrorAction Stop
    
    if ($iisFeature.State -eq "Disabled") {
        Write-Host "      IIS no está instalado. Instalando..." -ForegroundColor Yellow
        Write-Host "      ADVERTENCIA: Esto puede tardar varios minutos..." -ForegroundColor Yellow
        
        # Instalar IIS con características necesarias para ASP.NET
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45 -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-DirectoryBrowsing -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45 -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIExtensions -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIFilter -All -NoRestart
        
        Write-Host "      OK: IIS instalado correctamente" -ForegroundColor Green
        Write-Host "      NOTA: Es posible que necesites reiniciar Windows" -ForegroundColor Yellow
    } else {
        Write-Host "      OK: IIS ya está instalado" -ForegroundColor Green
    }
} catch {
    Write-Host "      ERROR: No se pudo verificar/instalar IIS: $_" -ForegroundColor Red
    Read-Host "Presiona Enter para continuar de todas formas"
}

# =========================
# [2/5] Importar módulo IIS
# =========================
Write-Host "`n[2/5] Cargando módulo WebAdministration..." -ForegroundColor Yellow

try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "      OK: Módulo cargado" -ForegroundColor Green
} catch {
    Write-Host "      ADVERTENCIA: No se pudo cargar módulo WebAdministration" -ForegroundColor Yellow
    Write-Host "      Intentando alternativa..." -ForegroundColor Yellow
    
    # Intentar agregar snap-in de IIS 6.0 como alternativa
    try {
        Add-PSSnapin WebAdministration -ErrorAction SilentlyContinue
        Write-Host "      OK: Snap-in alternativo cargado" -ForegroundColor Green
    } catch {
        Write-Host "      ERROR: No se pueden usar cmdlets de IIS" -ForegroundColor Red
    }
}

# ======================
# [3/5] Copiar archivos
# ======================
Write-Host "`n[3/5] Copiando archivos de la aplicación..." -ForegroundColor Yellow

$sourceDir = "$PSScriptRoot\VentasWeb"
$destDir = "C:\inetpub\wwwroot\VentasWeb"

# Crear directorio destino
if (-not (Test-Path $destDir)) {
    New-Item -Path $destDir -ItemType Directory -Force | Out-Null
    Write-Host "      Directorio creado: $destDir" -ForegroundColor Gray
}

# Copiar carpetas necesarias
Write-Host "      Copiando bin..." -ForegroundColor Gray
Copy-Item "$sourceDir\bin\*" "$destDir\bin\" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "      Copiando Content..." -ForegroundColor Gray
Copy-Item "$sourceDir\Content\*" "$destDir\Content\" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "      Copiando Scripts..." -ForegroundColor Gray
Copy-Item "$sourceDir\Scripts\*" "$destDir\Scripts\" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "      Copiando Views..." -ForegroundColor Gray
Copy-Item "$sourceDir\Views" "$destDir\Views" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "      Copiando archivos raíz..." -ForegroundColor Gray
Copy-Item "$sourceDir\Global.asax" "$destDir\" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourceDir\Web.config" "$destDir\" -Force -ErrorAction SilentlyContinue

Write-Host "      OK: Archivos copiados" -ForegroundColor Green

# =======================
# [4/5] Configurar IIS
# =======================
Write-Host "`n[4/5] Configurando IIS..." -ForegroundColor Yellow

$appPoolName = "VentasWebPool"
$siteName = "VentasWeb"
$port = 80

try {
    # Verificar si existe el Application Pool
    $appPoolExists = Test-Path "IIS:\AppPools\$appPoolName" -ErrorAction SilentlyContinue
    
    if (-not $appPoolExists) {
        Write-Host "      Creando Application Pool '$appPoolName'..." -ForegroundColor Gray
        New-WebAppPool -Name $appPoolName -ErrorAction Stop
        Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
        Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
        Write-Host "      OK: Application Pool creado" -ForegroundColor Green
    } else {
        Write-Host "      Application Pool '$appPoolName' ya existe" -ForegroundColor Gray
    }
    
    # Verificar si existe el sitio
    $siteExists = Test-Path "IIS:\Sites\$siteName" -ErrorAction SilentlyContinue
    
    if (-not $siteExists) {
        Write-Host "      Creando Website '$siteName'..." -ForegroundColor Gray
        New-Website -Name $siteName -Port $port -PhysicalPath $destDir -ApplicationPool $appPoolName -ErrorAction Stop
        Write-Host "      OK: Website creado en puerto $port" -ForegroundColor Green
    } else {
        Write-Host "      Website '$siteName' ya existe" -ForegroundColor Gray
        Write-Host "      Actualizando configuración..." -ForegroundColor Gray
        Set-ItemProperty "IIS:\Sites\$siteName" -Name physicalPath -Value $destDir
        Set-ItemProperty "IIS:\Sites\$siteName" -Name applicationPool -Value $appPoolName
        Write-Host "      OK: Website actualizado" -ForegroundColor Green
    }
    
    # Iniciar sitio si está detenido
    $site = Get-Website -Name $siteName -ErrorAction SilentlyContinue
    if ($site.State -ne "Started") {
        Start-Website -Name $siteName -ErrorAction SilentlyContinue
        Write-Host "      Website iniciado" -ForegroundColor Green
    }
    
} catch {
    Write-Host "      ADVERTENCIA: No se pudo configurar IIS automáticamente" -ForegroundColor Yellow
    Write-Host "      Error: $_" -ForegroundColor Red
    Write-Host "      Configura manualmente desde IIS Manager:" -ForegroundColor Yellow
    Write-Host "      1. Abre IIS Manager (inetmgr)" -ForegroundColor Gray
    Write-Host "      2. Crea Application Pool: VentasWebPool (.NET 4.0)" -ForegroundColor Gray
    Write-Host "      3. Crea Website: VentasWeb (puerto 80)" -ForegroundColor Gray
    Write-Host "      4. Apunta a: C:\inetpub\wwwroot\VentasWeb" -ForegroundColor Gray
}

# ==========================
# [5/5] Establecer permisos
# ==========================
Write-Host "`n[5/5] Configurando permisos..." -ForegroundColor Yellow

try {
    $acl = Get-Acl $destDir
    $permission = "IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
    $acl.SetAccessRule($accessRule)
    Set-Acl $destDir $acl
    Write-Host "      OK: Permisos configurados para IIS_IUSRS" -ForegroundColor Green
} catch {
    Write-Host "      ADVERTENCIA: No se pudieron establecer permisos automáticamente" -ForegroundColor Yellow
    Write-Host "      Configura permisos manualmente:" -ForegroundColor Yellow
    Write-Host "      1. Clic derecho en C:\inetpub\wwwroot\VentasWeb" -ForegroundColor Gray
    Write-Host "      2. Propiedades -> Seguridad -> Editar" -ForegroundColor Gray
    Write-Host "      3. Agregar usuario: IIS_IUSRS" -ForegroundColor Gray
    Write-Host "      4. Dar permisos: Control total" -ForegroundColor Gray
}

# ========================
# Verificación final
# ========================
Write-Host "`n[6/6] Verificando despliegue..." -ForegroundColor Yellow

$dllPath = "$destDir\bin\VentasWeb.dll"
if (Test-Path $dllPath) {
    $dll = Get-Item $dllPath
    Write-Host "      OK: VentasWeb.dll encontrado" -ForegroundColor Green
    Write-Host "      Tamaño: $($dll.Length) bytes" -ForegroundColor Gray
    Write-Host "      Fecha: $($dll.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "      ERROR: VentasWeb.dll no encontrado en $dllPath" -ForegroundColor Red
    Write-Host "      Verifica que la compilación fue exitosa" -ForegroundColor Yellow
}

# ====================
# Resumen
# ====================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "DESPLIEGUE COMPLETADO" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Accede al sistema:" -ForegroundColor White
Write-Host "  http://localhost/VentasWeb`n" -ForegroundColor Green

Write-Host "Para acceder al módulo fiscal:" -ForegroundColor White
Write-Host "  http://localhost/VentasWeb/ConfiguracionFiscal`n" -ForegroundColor Green

Write-Host "Si no funciona:" -ForegroundColor Yellow
Write-Host "  1. Reinicia IIS: iisreset" -ForegroundColor Gray
Write-Host "  2. Verifica en IIS Manager (inetmgr)" -ForegroundColor Gray
Write-Host "  3. Revisa el Web.config tiene la cadena de conexión correcta`n" -ForegroundColor Gray

Read-Host "Presiona Enter para salir"
