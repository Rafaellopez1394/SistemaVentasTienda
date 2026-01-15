# Script para probar generación de factura con FiscalAPI

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  PRUEBA DE GENERACION DE FACTURA - FISCALAPI" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# Credenciales FiscalAPI
$apiKey = "sk_test_16b2fc7c_460a_4ba0_867f_b53cad3266f9"
$tenantKey = "e0a0d1de-d225-46de-b95f-55d04f2787ff"

# VentaID de prueba
$ventaId = "6bc16123-7b85-418e-a4aa-62384726aa44"

Write-Host "[1] Verificando configuracion en base de datos..." -ForegroundColor Yellow

$query = @"
SELECT 
    ConfigID,
    ProveedorPAC,
    EsProduccion,
    UrlTimbrado,
    Usuario,
    Password,
    Activo
FROM ConfiguracionPAC 
WHERE ConfigID = 3
"@

sqlcmd -S . -d DB_TIENDA -E -Q $query -W

Write-Host "`n[2] Verificando venta de prueba..." -ForegroundColor Yellow

$query2 = @"
SELECT 
    v.VentaID,
    v.Total,
    v.FechaVenta,
    v.TipoVenta,
    COUNT(dv.ProductoID) as NumProductos
FROM VentasClientes v
LEFT JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
WHERE v.VentaID = '$ventaId'
GROUP BY v.VentaID, v.Total, v.FechaVenta, v.TipoVenta
"@

sqlcmd -S . -d DB_TIENDA -E -Q $query2 -W

Write-Host "`n[3] Probando conexion con FiscalAPI..." -ForegroundColor Yellow

$headers = @{
    "X-API-KEY" = $apiKey
    "X-TENANT-KEY" = $tenantKey
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

try {
    $response = Invoke-WebRequest -Uri "https://test.fiscalapi.com/api/v4/invoices" -Headers $headers -Method GET -UseBasicParsing
    Write-Host "[OK] Conexion exitosa con FiscalAPI" -ForegroundColor Green
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Gray
}
catch {
    Write-Host "[ERROR] No se pudo conectar con FiscalAPI" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n[4] Verificando que IIS Express este corriendo..." -ForegroundColor Yellow

$iisProcess = Get-Process -Name "iisexpress" -ErrorAction SilentlyContinue

if ($iisProcess) {
    Write-Host "[OK] IIS Express esta corriendo (PID: $($iisProcess.Id))" -ForegroundColor Green
}
else {
    Write-Host "[AVISO] IIS Express no esta corriendo" -ForegroundColor Yellow
    Write-Host "Iniciando IIS Express..." -ForegroundColor Yellow
    
    $webPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
    $port = 64927
    
    Start-Process "C:\Program Files\IIS Express\iisexpress.exe" -ArgumentList "/path:$webPath", "/port:$port" -WindowStyle Minimized
    Start-Sleep -Seconds 5
    
    Write-Host "[OK] IIS Express iniciado en puerto $port" -ForegroundColor Green
}

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "CONFIGURACION LISTA" -ForegroundColor Green
Write-Host "========================================================`n" -ForegroundColor Cyan

Write-Host "Proximos pasos para probar:" -ForegroundColor Cyan
Write-Host "1. Abre navegador en: http://localhost:64927" -ForegroundColor White
Write-Host "2. Inicia sesion en el sistema" -ForegroundColor White
Write-Host "3. Ve a Ventas > Historial de Ventas" -ForegroundColor White
Write-Host "4. Busca la venta: $ventaId" -ForegroundColor White
Write-Host "5. Haz clic en 'Facturar'" -ForegroundColor White

Write-Host "`nO usa el endpoint directo POST:" -ForegroundColor Cyan
Write-Host "http://localhost:64927/Factura/GenerarFactura" -ForegroundColor White
Write-Host "Con el body JSON:" -ForegroundColor White
Write-Host @"
{
  "VentaID": "$ventaId",
  "ReceptorRFC": "XAXX010101000",
  "ReceptorNombre": "PUBLICO GENERAL",
  "ReceptorUsoCFDI": "G03",
  "FormaPago": "01",
  "MetodoPago": "PUE"
}
"@ -ForegroundColor Gray

Write-Host "`n========================================================`n" -ForegroundColor Cyan

Read-Host "Presiona Enter para salir"
