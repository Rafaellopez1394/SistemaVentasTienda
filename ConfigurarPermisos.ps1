# Configurar permisos y asegurar sitio correcto
# EJECUTAR COMO ADMINISTRADOR

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   CONFIGURANDO PERMISOS Y SITIO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$publishPath = "C:\Publish\SistemaVentas"
$siteName = "SistemaVentas"
$appPoolName = "VentasWebPool"

# Verificar carpeta
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: No existe $publishPath" -ForegroundColor Red
    pause
    exit 1
}

Import-Module WebAdministration

Write-Host "`n[1/6] Estado actual de sitios:" -ForegroundColor Yellow
Get-Website | Format-Table Name, State, ID, @{Name="Port";Expression={($_.bindings.Collection.bindingInformation -split ':')[1]}} -AutoSize

Write-Host "`n[2/6] Deteniendo todos los sitios..." -ForegroundColor Yellow
Get-Website | Stop-Website -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "Sitios detenidos" -ForegroundColor Green

Write-Host "`n[3/6] Configurando permisos NTFS..." -ForegroundColor Yellow
$accounts = @(
    "IIS_IUSRS",
    "IUSR",
    "IIS APPPOOL\$appPoolName"
)

foreach ($account in $accounts) {
    try {
        Write-Host "  Configurando permisos para: $account" -ForegroundColor Gray
        $acl = Get-Acl $publishPath
        $permission = "$account", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow"
        $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
        $acl.SetAccessRule($accessRule)
        Set-Acl $publishPath $acl
        Write-Host "  OK - $account" -ForegroundColor Green
    } catch {
        Write-Host "  ADVERTENCIA: No se pudo configurar $account" -ForegroundColor Yellow
    }
}

Write-Host "`n[4/6] Verificando Application Pool..." -ForegroundColor Yellow
$pool = Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue
if (-not $pool) {
    Write-Host "Creando Application Pool..." -ForegroundColor Yellow
    New-WebAppPool -Name $appPoolName
}

# Configurar el pool
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name processModel.identityType -Value "ApplicationPoolIdentity"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name startMode -Value "AlwaysRunning"
Write-Host "Application Pool configurado" -ForegroundColor Green

Write-Host "`n[5/6] Reconfigurando sitio..." -ForegroundColor Yellow
$site = Get-Website -Name $siteName -ErrorAction SilentlyContinue
if ($site) {
    Remove-Website -Name $siteName
    Write-Host "Sitio anterior eliminado" -ForegroundColor Gray
}

# Crear sitio nuevo
New-Website -Name $siteName `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port 80 `
    -Force

# Configurar autenticacion anonima
Set-WebConfigurationProperty -Filter "/system.webServer/security/authentication/anonymousAuthentication" `
    -Name enabled -Value $true -PSPath "IIS:\" -Location $siteName

Write-Host "Sitio creado" -ForegroundColor Green

Write-Host "`n[6/6] Iniciando sitio..." -ForegroundColor Yellow
Start-WebAppPool -Name $appPoolName
Start-Website -Name $siteName
Start-Sleep -Seconds 2

$siteState = (Get-Website -Name $siteName).State
Write-Host "Estado del sitio: $siteState" -ForegroundColor $(if($siteState -eq "Started"){"Green"}else{"Red"})

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   CONFIGURACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

Write-Host "`nSitios activos:" -ForegroundColor Yellow
Get-Website | Where-Object { $_.State -eq "Started" } | Format-Table Name, State, PhysicalPath -AutoSize

Write-Host "`nDetalles del sitio:" -ForegroundColor Yellow
Write-Host "Nombre: $siteName" -ForegroundColor White
Write-Host "Ruta: $publishPath" -ForegroundColor Cyan
Write-Host "Puerto: 80" -ForegroundColor White
Write-Host "Pool: $appPoolName" -ForegroundColor White

Write-Host "`nAccede al sitio en:" -ForegroundColor Cyan
Write-Host "  http://localhost" -ForegroundColor Green
Write-Host "  http://localhost/Login" -ForegroundColor Green
Write-Host ""
Write-Host "Presiona Ctrl+F5 en el navegador para forzar recarga" -ForegroundColor Yellow
Write-Host ""
pause
