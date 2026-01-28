# ============================================================================
# SCRIPT DE DESPLIEGUE RAPIDO A IIS
# ============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DESPLIEGUE A IIS - SISTEMA POS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$sourceDir = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
$destDir = "C:\inetpub\wwwroot\VentasWeb"

# Crear directorio si no existe
if (-not (Test-Path $destDir)) {
    Write-Host "[1/4] Creando directorio en IIS..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    Write-Host "      OK: Directorio creado" -ForegroundColor Green
} else {
    Write-Host "[1/4] Directorio IIS ya existe" -ForegroundColor Green
}

# Copiar archivos
Write-Host ""
Write-Host "[2/4] Copiando archivos..." -ForegroundColor Yellow

# Copiar bin
Write-Host "      Copiando bin..." -ForegroundColor Gray
Copy-Item "$sourceDir\bin\*" "$destDir\bin\" -Recurse -Force

# Copiar Content
Write-Host "      Copiando Content..." -ForegroundColor Gray
Copy-Item "$sourceDir\Content\*" "$destDir\Content\" -Recurse -Force

# Copiar Scripts
Write-Host "      Copiando Scripts..." -ForegroundColor Gray
Copy-Item "$sourceDir\Scripts\*" "$destDir\Scripts\" -Recurse -Force

# Copiar Views
Write-Host "      Copiando Views..." -ForegroundColor Gray
Copy-Item "$sourceDir\Views\*" "$destDir\Views\" -Recurse -Force

# Copiar archivos raíz
Write-Host "      Copiando archivos raíz..." -ForegroundColor Gray
Copy-Item "$sourceDir\Global.asax" "$destDir\" -Force
Copy-Item "$sourceDir\Web.config" "$destDir\" -Force

Write-Host "      OK: Archivos copiados" -ForegroundColor Green

# Configurar IIS
Write-Host ""
Write-Host "[3/4] Configurando IIS..." -ForegroundColor Yellow

Import-Module WebAdministration -ErrorAction SilentlyContinue

try {
    # Crear Application Pool
    $appPoolName = "VentasWebPool"
    if (-not (Test-Path "IIS:\AppPools\$appPoolName")) {
        New-WebAppPool -Name $appPoolName
        Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
        Write-Host "      OK: Application Pool creado" -ForegroundColor Green
    } else {
        Write-Host "      OK: Application Pool ya existe" -ForegroundColor Green
    }
    
    # Crear Website
    if (-not (Test-Path "IIS:\Sites\VentasWeb")) {
        New-WebSite -Name "VentasWeb" -PhysicalPath $destDir -ApplicationPool $appPoolName -Port 80
        Write-Host "      OK: Website creado" -ForegroundColor Green
    } else {
        Write-Host "      OK: Website ya existe" -ForegroundColor Green
    }
    
    # Configurar permisos
    Write-Host "      Configurando permisos..." -ForegroundColor Gray
    $acl = Get-Acl $destDir
    $identity = "IIS_IUSRS"
    $fileSystemRights = "FullControl"
    $type = "Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($identity, $fileSystemRights, $type)
    $acl.SetAccessRule($accessRule)
    Set-Acl $destDir $acl
    
    Write-Host "      OK: Permisos configurados" -ForegroundColor Green
    
} catch {
    Write-Host "      ADVERTENCIA: No se pudo configurar IIS automáticamente" -ForegroundColor Yellow
    Write-Host "      Configura manualmente desde IIS Manager" -ForegroundColor Gray
}

# Verificar
Write-Host ""
Write-Host "[4/4] Verificando despliegue..." -ForegroundColor Yellow

$dllPath = "$destDir\bin\VentasWeb.dll"
if (Test-Path $dllPath) {
    $dll = Get-Item $dllPath
    $sizeKB = [math]::Round($dll.Length / 1KB, 2)
    Write-Host "      OK: VentasWeb.dll ($sizeKB KB)" -ForegroundColor Green
    Write-Host "      Fecha: $($dll.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "      ERROR: VentasWeb.dll no encontrado" -ForegroundColor Red
}

# Resumen
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DESPLIEGUE COMPLETADO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Accede al sistema:" -ForegroundColor White
Write-Host "  http://localhost/VentasWeb" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para acceder al módulo fiscal:" -ForegroundColor White
Write-Host "  http://localhost/VentasWeb/ConfiguracionFiscal" -ForegroundColor Cyan
Write-Host ""
