# Limpieza total y creacion desde cero usando appcmd
# EJECUTAR COMO ADMINISTRADOR

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   LIMPIEZA TOTAL Y CREACION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
$publishPath = "C:\Publish\SistemaVentas"

if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: No existe $publishPath" -ForegroundColor Red
    pause
    exit 1
}

Write-Host "`n[1/7] Listando sitios actuales..." -ForegroundColor Yellow
& $appcmd list site

Write-Host "`n[2/7] Deteniendo todos los sitios..." -ForegroundColor Yellow
& $appcmd list site /state:Started | ForEach-Object {
    if ($_ -match 'SITE "([^"]+)"') {
        $siteName = $matches[1]
        Write-Host "  Deteniendo: $siteName" -ForegroundColor Gray
        & $appcmd stop site $siteName
    }
}
Write-Host "Sitios detenidos" -ForegroundColor Green

Write-Host "`n[3/7] Eliminando sitio SistemaVentas si existe..." -ForegroundColor Yellow
& $appcmd delete site "SistemaVentas" 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Sitio eliminado" -ForegroundColor Green
} else {
    Write-Host "No habia sitio anterior" -ForegroundColor Gray
}

Write-Host "`n[4/7] Eliminando Default Web Site..." -ForegroundColor Yellow
& $appcmd delete site "Default Web Site" 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Default Web Site eliminado" -ForegroundColor Green
} else {
    Write-Host "No existia Default Web Site" -ForegroundColor Gray
}

Write-Host "`n[5/7] Configurando Application Pool..." -ForegroundColor Yellow
$appPoolName = "VentasWebPool"
& $appcmd list apppool $appPoolName 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Creando Application Pool..." -ForegroundColor Yellow
    & $appcmd add apppool /name:$appPoolName /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated"
} else {
    Write-Host "Application Pool ya existe" -ForegroundColor Gray
    & $appcmd set apppool $appPoolName /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated"
}
Write-Host "Application Pool configurado" -ForegroundColor Green

Write-Host "`n[6/7] Creando sitio SistemaVentas..." -ForegroundColor Yellow
& $appcmd add site /name:"SistemaVentas" /physicalPath:"$publishPath" /bindings:"http/*:80:"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Sitio creado exitosamente" -ForegroundColor Green
    
    # Asignar el application pool
    & $appcmd set site "SistemaVentas" /[path='/'].applicationPool:$appPoolName
    
    # Habilitar autenticacion anonima
    & $appcmd set config "SistemaVentas" /section:anonymousAuthentication /enabled:true
    
} else {
    Write-Host "ERROR: No se pudo crear el sitio" -ForegroundColor Red
    pause
    exit 1
}

Write-Host "`n[7/7] Configurando permisos..." -ForegroundColor Yellow
$accounts = @("IIS_IUSRS", "IUSR", "IIS APPPOOL\$appPoolName")
foreach ($account in $accounts) {
    Write-Host "  Configurando: $account" -ForegroundColor Gray
    icacls "$publishPath" /grant "${account}:(OI)(CI)F" /T /Q 2>$null
}
Write-Host "Permisos configurados" -ForegroundColor Green

Write-Host "`nIniciando sitio..." -ForegroundColor Yellow
& $appcmd start site "SistemaVentas"
Start-Sleep -Seconds 3

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   SITIO CONFIGURADO" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

Write-Host "`nSitios activos:" -ForegroundColor Yellow
& $appcmd list site /state:Started

Write-Host "`nURL del sitio:" -ForegroundColor Cyan
Write-Host "  http://localhost" -ForegroundColor Green
Write-Host "  http://localhost/Login" -ForegroundColor Green

Write-Host "`nSi aun ves error:" -ForegroundColor Yellow
Write-Host "1. Cierra TODAS las ventanas del navegador" -ForegroundColor White
Write-Host "2. Abre el navegador nuevamente" -ForegroundColor White
Write-Host "3. Ve a http://localhost" -ForegroundColor White
Write-Host ""
pause
