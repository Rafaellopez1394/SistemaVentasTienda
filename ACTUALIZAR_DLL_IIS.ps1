# ============================================
# ACTUALIZAR DLL EN IIS
# ============================================
# Ejecutar como ADMINISTRADOR

Write-Host "Actualizando VentasWeb.dll en IIS..." -ForegroundColor Cyan

# Verificar privilegios
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Ejecuta como Administrador" -ForegroundColor Red
    Read-Host "Presiona Enter"
    exit 1
}

$source = "$PSScriptRoot\VentasWeb\bin\VentasWeb.dll"
$dest = "C:\inetpub\wwwroot\VentasWeb\bin\VentasWeb.dll"

# Verificar archivo fuente
if (-not (Test-Path $source)) {
    Write-Host "ERROR: No se encuentra el archivo compilado" -ForegroundColor Red
    exit 1
}

# Detener IIS
Write-Host "Deteniendo IIS..." -ForegroundColor Yellow
Stop-Service W3SVC -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

# Copiar DLL
Write-Host "Copiando DLL actualizado..." -ForegroundColor Yellow
Copy-Item $source $dest -Force

# Iniciar IIS
Write-Host "Iniciando IIS..." -ForegroundColor Yellow
Start-Service W3SVC

Write-Host "`nOK: DLL actualizado exitosamente" -ForegroundColor Green
Write-Host "Accede al sistema: http://localhost/VentasWeb/ConfiguracionFiscal" -ForegroundColor Cyan

Read-Host "`nPresiona Enter para salir"
