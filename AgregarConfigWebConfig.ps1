# Agregar configuracion system.webServer al Web.config de la publicacion
Write-Host "Agregando configuracion al Web.config..." -ForegroundColor Yellow

$webConfigPath = "C:\Publish\SistemaVentas\Web.config"

if (-not (Test-Path $webConfigPath)) {
    Write-Host "ERROR: No existe $webConfigPath" -ForegroundColor Red
    Write-Host "Ejecuta primero PublicarCarpeta.ps1" -ForegroundColor Yellow
    pause
    exit 1
}

# Backup
Copy-Item $webConfigPath "$webConfigPath.backup" -Force

[xml]$webConfig = Get-Content $webConfigPath

# Crear system.webServer si no existe
if ($null -eq $webConfig.configuration.'system.webServer') {
    $systemWebServer = $webConfig.CreateElement("system.webServer")
    
    # Modules
    $modules = $webConfig.CreateElement("modules")
    $modules.SetAttribute("runAllManagedModulesForAllRequests", "true")
    $systemWebServer.AppendChild($modules) | Out-Null
    
    # Handlers
    $handlers = $webConfig.CreateElement("handlers")
    
    $handler = $webConfig.CreateElement("add")
    $handler.SetAttribute("name", "ExtensionlessUrlHandler-Integrated-4.0")
    $handler.SetAttribute("path", "*.")
    $handler.SetAttribute("verb", "*")
    $handler.SetAttribute("type", "System.Web.Handlers.TransferRequestHandler")
    $handler.SetAttribute("preCondition", "integratedMode,runtimeVersionv4.0")
    $handlers.AppendChild($handler) | Out-Null
    
    $systemWebServer.AppendChild($handlers) | Out-Null
    
    $webConfig.configuration.AppendChild($systemWebServer) | Out-Null
    
    $webConfig.Save($webConfigPath)
    Write-Host "Configuracion agregada exitosamente" -ForegroundColor Green
} else {
    Write-Host "Ya tiene configuracion system.webServer" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Ahora vuelve a empaquetar ejecutando:" -ForegroundColor Cyan
Write-Host "  EmpaquetarParaOtraPC.ps1" -ForegroundColor White
Write-Host ""
pause
