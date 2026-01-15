# Script de prueba directa de generación de factura

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  DEBUG: GENERACION DE FACTURA PASO A PASO" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

$ventaId = "6bc16123-7b85-418e-a4aa-62384726aa44"

Write-Host "[1] Verificando detalles completos de la venta..." -ForegroundColor Yellow

$query = @"
SELECT 
    v.VentaID, 
    v.Total AS TotalVenta, 
    v.FechaVenta,
    v.TipoVenta, 
    ISNULL(v.MontoPagado, v.Total) AS MontoPagado,
    ISNULL(v.SaldoPendiente, 0) AS SaldoPendiente,
    dv.ProductoID, 
    p.CodigoInterno, 
    p.Nombre, 
    dv.Cantidad, 
    dv.PrecioVenta, 
    (dv.Cantidad * dv.PrecioVenta) AS Subtotal,
    ISNULL(dv.TasaIVA, 0) AS TasaIVA, 
    ISNULL(dv.MontoIVA, 0) AS MontoIVA,
    ISNULL(p.Exento, 0) AS Exento,
    p.ClaveProdServSAT,
    p.ClaveUnidadSAT,
    CASE 
        WHEN ISNULL(p.Exento, 0) = 1 THEN '01 - No objeto de impuestos (Exento)'
        WHEN ISNULL(dv.TasaIVA, 0) > 0 THEN '02 - Si objeto de impuestos'
        ELSE '01 - No objeto de impuestos'
    END AS ObjetoImp
FROM VentasClientes v
INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
WHERE v.VentaID = '$ventaId'
"@

$result = sqlcmd -S . -d DB_TIENDA -E -Q $query -W -s"|"

if ($result) {
    Write-Host $result -ForegroundColor Gray
    Write-Host "[OK] Venta encontrada con productos" -ForegroundColor Green
} else {
    Write-Host "[ERROR] No se encontro la venta" -ForegroundColor Red
    exit 1
}

Write-Host "`n[2] Verificando si existe ya una factura para esta venta..." -ForegroundColor Yellow

$query2 = @"
SELECT 
    FacturaID,
    Serie,
    Folio,
    FechaEmision,
    Total,
    Estatus,
    UUID,
    XMLTimbrado
FROM Facturas
WHERE VentaID = '$ventaId'
ORDER BY FechaCreacion DESC
"@

$facturas = sqlcmd -S . -d DB_TIENDA -E -Q $query2 -W

if ($facturas -match "FacturaID") {
    Write-Host $facturas -ForegroundColor Gray
    Write-Host "[AVISO] Ya existe factura(s) para esta venta" -ForegroundColor Yellow
} else {
    Write-Host "[OK] No hay facturas previas para esta venta" -ForegroundColor Green
}

Write-Host "`n[3] Verificando stored procedure GenerarFolioFactura..." -ForegroundColor Yellow

$query3 = @"
DECLARE @Serie VARCHAR(10) = 'A'
DECLARE @Folio INT

EXEC GenerarFolioFactura @Serie, @Folio OUTPUT

SELECT @Folio AS ProximoFolio
"@

$folio = sqlcmd -S . -d DB_TIENDA -E -Q $query3 -W

Write-Host $folio -ForegroundColor Gray

Write-Host "`n[4] Probando conexion al endpoint de facturacion..." -ForegroundColor Yellow

try {
    $testResponse = Invoke-WebRequest -Uri "http://localhost:64927/Factura/Index?ventaId=$ventaId" -Method GET -UseBasicParsing -TimeoutSec 5
    Write-Host "[OK] Endpoint responde (Status: $($testResponse.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] No se puede conectar al endpoint" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`n[SOLUCION] Inicia sesion primero en el navegador:" -ForegroundColor Yellow
    Write-Host "http://localhost:64927" -ForegroundColor White
}

Write-Host "`n[5] Verificando configuracion del emisor..." -ForegroundColor Yellow

$query5 = @"
SELECT 
    ConfigID,
    RazonSocial,
    RFC,
    RegimenFiscal,
    CodigoPostal,
    Activo
FROM ConfiguracionGeneral
WHERE Activo = 1
ORDER BY ConfigID DESC
"@

$emisor = sqlcmd -S . -d DB_TIENDA -E -Q $query5 -W

if ($emisor -match "RFC") {
    Write-Host $emisor -ForegroundColor Gray
} else {
    Write-Host "[AVISO] No se encontro configuracion del emisor activa" -ForegroundColor Yellow
}

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "DIAGNOSTICO COMPLETADO" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

Write-Host "Para generar la factura, usa uno de estos metodos:" -ForegroundColor Cyan
Write-Host "`n1. Desde el navegador:" -ForegroundColor White
Write-Host "   http://localhost:64927/Factura/Index?ventaId=$ventaId" -ForegroundColor Gray

Write-Host "`n2. Via POST (desde PowerShell con sesion):" -ForegroundColor White
Write-Host @"
`$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
`$body = @{
    VentaID = "$ventaId"
    ReceptorRFC = "XAXX010101000"
    ReceptorNombre = "PUBLICO GENERAL"
    ReceptorRegimenFiscal = "616"
    ReceptorUsoCFDI = "G03"
    FormaPago = "01"
    MetodoPago = "PUE"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:64927/Factura/GenerarFactura" ``
    -Method POST ``
    -Body `$body ``
    -ContentType "application/json" ``
    -WebSession `$session ``
    -UseBasicParsing
"@ -ForegroundColor Gray

Write-Host "`n========================================================`n" -ForegroundColor Cyan

Read-Host "Presiona Enter para salir"
