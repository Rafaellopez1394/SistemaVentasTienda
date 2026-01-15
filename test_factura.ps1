# Script para probar generación de factura
$baseUrl = "http://localhost:64927"

# Crear sesión web para mantener cookies
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Host "1. Iniciando sesión..." -ForegroundColor Cyan

# Login
$loginBody = @{
    correo = "admin@gmail.com"
    clave = "123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/Login/Index" `
        -Method POST `
        -ContentType "application/x-www-form-urlencoded" `
        -Body "correo=admin@gmail.com&clave=123" `
        -SessionVariable session `
        -MaximumRedirection 0 `
        -ErrorAction SilentlyContinue
    
    Write-Host "   Login exitoso!" -ForegroundColor Green
} catch {
    Write-Host "   Warning: $($_.Exception.Message)" -ForegroundColor Yellow
}

Start-Sleep -Seconds 1

Write-Host "`n2. Generando factura..." -ForegroundColor Cyan

# Generar factura
$facturaBody = @{
    VentaID = "6bc16123-7b85-418e-a4aa-62384726aa44"
    ReceptorRFC = "XAXX010101000"
    ReceptorNombre = "PUBLICO EN GENERAL"
    ReceptorRegimenFiscal = "616"
    UsoCFDI = "S01"
    FormaPago = "01"
    MetodoPago = "PUE"
} | ConvertTo-Json

try {
    $facturaResponse = Invoke-RestMethod -Uri "$baseUrl/Factura/GenerarFactura" `
        -Method POST `
        -ContentType "application/json" `
        -Body $facturaBody `
        -WebSession $session
    
    Write-Host "`n=== RESULTADO ===" -ForegroundColor Yellow
    Write-Host "Success: $($facturaResponse.success)" -ForegroundColor $(if ($facturaResponse.success) { "Green" } else { "Red" })
    Write-Host "Mensaje: $($facturaResponse.mensaje)" -ForegroundColor White
    
    if ($facturaResponse.data) {
        Write-Host "`nData:" -ForegroundColor Cyan
        $facturaResponse.data | Format-List
    }
} catch {
    Write-Host "`n=== ERROR ===" -ForegroundColor Red
    Write-Host $_.Exception.Message
    if ($_.ErrorDetails) {
        Write-Host $_.ErrorDetails.Message
    }
}
