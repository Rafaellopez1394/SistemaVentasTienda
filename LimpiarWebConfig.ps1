# Limpiar Web.config - Remover configuraciones duplicadas
$webConfig = "C:\SistemaVentas\Web.config"

if (Test-Path $webConfig) {
    [xml]$xml = Get-Content $webConfig
    
    # Remover toda la seccion system.webServer si existe
    $systemWebServer = $xml.configuration.'system.webServer'
    if ($systemWebServer) {
        $xml.configuration.RemoveChild($systemWebServer) | Out-Null
    }
    
    $xml.Save($webConfig)
    Write-Host "       Web.config limpio OK"
} else {
    Write-Host "       ERROR: Web.config no encontrado"
}
