# Script de instalacion para la PC de destino
# EJECUTAR COMO ADMINISTRADOR EN LA PC DESTINO

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   INSTALADOR SISTEMA DE VENTAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Configuracion
$siteName = "SistemaVentas"
$appPoolName = "VentasWebPool"
$installPath = "C:\SistemaVentas"
$puerto = 80

# Verificar que existe la carpeta de instalacion
$currentPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourcePath = Join-Path $currentPath "Web"

if (-not (Test-Path $sourcePath)) {
    Write-Host "ERROR: No se encuentra la carpeta 'Web' con los archivos del sistema" -ForegroundColor Red
    Write-Host "Estructura esperada:" -ForegroundColor Yellow
    Write-Host "  InstalarSistemaVentas/" -ForegroundColor White
    Write-Host "    - InstaladorDestino.ps1 (este archivo)" -ForegroundColor White
    Write-Host "    - Web/ (carpeta con todos los archivos)" -ForegroundColor White
    pause
    exit 1
}

Write-Host "`n[1/7] Verificando IIS..." -ForegroundColor Yellow
$iisFeature = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -ErrorAction SilentlyContinue
if (-not $iisFeature -or $iisFeature.State -ne "Enabled") {
    Write-Host "IIS no esta instalado. Instalando..." -ForegroundColor Yellow
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45 -All -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole -All -NoRestart
    Write-Host "IIS instalado (puede requerir reinicio)" -ForegroundColor Green
} else {
    Write-Host "IIS ya esta instalado" -ForegroundColor Green
}

Write-Host "`n[2/7] Copiando archivos a $installPath..." -ForegroundColor Yellow
if (Test-Path $installPath) {
    Write-Host "Eliminando instalacion anterior..." -ForegroundColor Yellow
    Remove-Item "$installPath\*" -Recurse -Force -ErrorAction SilentlyContinue
} else {
    New-Item -ItemType Directory -Path $installPath -Force | Out-Null
}

Write-Host "Copiando archivos (esto puede tardar)..." -ForegroundColor Gray
Copy-Item "$sourcePath\*" -Destination $installPath -Recurse -Force
Write-Host "Archivos copiados" -ForegroundColor Green

Write-Host "`n[3/7] Configurando permisos..." -ForegroundColor Yellow
$accounts = @("IIS_IUSRS", "IUSR")
foreach ($account in $accounts) {
    Write-Host "  Configurando: $account" -ForegroundColor Gray
    icacls "$installPath" /grant "${account}:(OI)(CI)F" /T /Q 2>$null
}
Write-Host "Permisos configurados" -ForegroundColor Green

Write-Host "`n[4/7] Configurando IIS..." -ForegroundColor Yellow
Import-Module WebAdministration -ErrorAction Stop

# Detener sitios existentes en puerto 80
Write-Host "Deteniendo sitios en puerto 80..." -ForegroundColor Gray
Get-Website | Where-Object { $_.bindings.Collection.bindingInformation -like "*:80:*" } | Stop-Website -ErrorAction SilentlyContinue

# Eliminar sitio si existe
$existingSite = Get-Website -Name $siteName -ErrorAction SilentlyContinue
if ($existingSite) {
    Write-Host "Eliminando sitio anterior..." -ForegroundColor Gray
    Remove-Website -Name $siteName
}

Write-Host "`n[5/7] Creando Application Pool..." -ForegroundColor Yellow
$pool = Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue
if ($pool) {
    Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
    Remove-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
}

New-WebAppPool -Name $appPoolName
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
Write-Host "Application Pool creado" -ForegroundColor Green

Write-Host "`n[6/7] Creando sitio web..." -ForegroundColor Yellow
New-Website -Name $siteName `
    -PhysicalPath $installPath `
    -ApplicationPool $appPoolName `
    -Port $puerto `
    -Force

# Habilitar autenticacion anonima
Set-WebConfigurationProperty -Filter "/system.webServer/security/authentication/anonymousAuthentication" `
    -Name enabled -Value $true -PSPath "IIS:\" -Location $siteName

# Configurar permisos para el Application Pool
$appPoolAccount = "IIS APPPOOL\$appPoolName"
icacls "$installPath" /grant "${appPoolAccount}:(OI)(CI)F" /T /Q 2>$null

Write-Host "Sitio creado" -ForegroundColor Green

Write-Host "`n[7/7] Iniciando sitio..." -ForegroundColor Yellow
Start-WebAppPool -Name $appPoolName
Start-Website -Name $siteName
Start-Sleep -Seconds 3

$siteState = (Get-Website -Name $siteName).State
if ($siteState -eq "Started") {
    Write-Host "Sitio iniciado correctamente" -ForegroundColor Green
} else {
    Write-Host "ADVERTENCIA: El sitio no se inicio correctamente" -ForegroundColor Yellow
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   INSTALACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Sitio instalado en: $installPath" -ForegroundColor White
Write-Host "URL de acceso: http://localhost" -ForegroundColor Cyan
Write-Host ""
Write-Host "IMPORTANTE:" -ForegroundColor Yellow
Write-Host "1. Configura la conexion a la base de datos en:" -ForegroundColor White
Write-Host "   $installPath\Web.config" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Busca la seccion <connectionStrings> y actualiza:" -ForegroundColor White
Write-Host "   - Server: nombre del servidor SQL" -ForegroundColor Gray
Write-Host "   - Database: nombre de la base de datos" -ForegroundColor Gray
Write-Host "   - User/Password: credenciales de acceso" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Reinicia el sitio despues de cambiar Web.config:" -ForegroundColor White
Write-Host "   iisreset" -ForegroundColor Gray
Write-Host ""
pause
