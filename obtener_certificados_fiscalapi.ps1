# Script para obtener certificados de prueba de FiscalAPI
# Los certificados están en la documentación oficial

# Certificados de prueba para el ambiente TEST de FiscalAPI
# RFC: EKU9003173C9 (Escuela Kemper Urgate SA de CV)

Write-Host "Consultando documentación de FiscalAPI para certificados de prueba..." -ForegroundColor Cyan

# Intentar obtener del repositorio de ejemplos
$repoUrl = "https://api.github.com/repos/fiscal-api/fiscal-api-dotnet/contents/examples"

try {
    $response = Invoke-RestMethod -Uri $repoUrl -Method Get -Headers @{
        "User-Agent" = "PowerShell"
    }
    
    Write-Host "Archivos encontrados en el repositorio:" -ForegroundColor Green
    $response | Where-Object { $_.name -like "*.cer" -or $_.name -like "*.key" } | ForEach-Object {
        Write-Host "  - $($_.name)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "No se pudo acceder al repositorio: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nNOTA: Los certificados de prueba deben obtenerse de:" -ForegroundColor Cyan
Write-Host "1. Documentación oficial de FiscalAPI" -ForegroundColor White
Write-Host "2. Portal del SAT (certificados de prueba CSD)" -ForegroundColor White
Write-Host "3. Contactar soporte de FiscalAPI para certificados de ambiente TEST" -ForegroundColor White
