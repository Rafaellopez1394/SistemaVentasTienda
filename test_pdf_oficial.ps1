# Script de prueba completa: Timbrar factura y descargar PDF oficial

Write-Host "=== TEST: Facturación Completa con PDF Oficial ===" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar que la aplicación esté corriendo
Write-Host "[1/5] Verificando que IIS Express esté corriendo..." -ForegroundColor Yellow
try {
    $test = Invoke-WebRequest -Uri "http://localhost:64927" -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    Write-Host "✅ IIS Express está corriendo" -ForegroundColor Green
}
catch {
    Write-Host "❌ IIS Express no está corriendo. Inicia la aplicación desde Visual Studio (F5)" -ForegroundColor Red
    exit
}
Write-Host ""

# 2. Generar y timbrar factura
Write-Host "[2/5] Generando y timbrando factura..." -ForegroundColor Yellow

$body = @{
    VentaID = "9aecc96b-e17d-4dc7-b2b8-4132a2173dc7"
    ReceptorRFC = "XAXX010101000"
    ReceptorNombre = "Público en General TEST PDF"
    ReceptorCP = "81048"
    ReceptorRegimenFiscal = "616"
    UsoCFDI = "G03"
    FormaPago = "01"
    MetodoPago = "PUE"
    Conceptos = @(
        @{
            ClaveProdServ = "01010101"
            Descripcion = "TEST PDF OFICIAL"
            Cantidad = 1
            ValorUnitario = 100
            Importe = 100
            ClaveUnidad = "H87"
            Unidad = "Pieza"
        }
    )
} | ConvertTo-Json -Depth 10

try {
    $response = Invoke-RestMethod -Uri "http://localhost:64927/Factura/GenerarFactura" -Method POST -Body $body -ContentType "application/json"
    
    if ($response.success) {
        Write-Host "✅ Factura timbrada exitosamente" -ForegroundColor Green
        Write-Host "   Serie-Folio: $($response.factura.Serie)-$($response.factura.Folio)" -ForegroundColor Gray
        Write-Host "   UUID: $($response.factura.UUID)" -ForegroundColor Gray
        
        $facturaId = $response.factura.FacturaID
        $uuid = $response.factura.UUID
        $serie = $response.factura.Serie
        $folio = $response.factura.Folio
    }
    else {
        Write-Host "❌ Error al timbrar: $($response.mensaje)" -ForegroundColor Red
        exit
    }
}
catch {
    Write-Host "❌ Error al llamar al servicio: $_" -ForegroundColor Red
    exit
}
Write-Host ""

# 3. Verificar en base de datos que se guardó el InvoiceId
Write-Host "[3/5] Verificando InvoiceId en base de datos..." -ForegroundColor Yellow

$sqlQuery = "SELECT FacturaID, Serie, Folio, UUID, FiscalAPIInvoiceId FROM Facturas WHERE FacturaID = '$facturaId'"
$dbResult = sqlcmd -S localhost -d DB_TIENDA -E -Q $sqlQuery -h -1 -W

if ($dbResult -match '\S') {
    $lines = $dbResult -split "`n" | Where-Object { $_ -match '\S' }
    $dataLine = $lines[0].Trim()
    $parts = $dataLine -split '\s+'
    
    if ($parts.Count -ge 5 -and $parts[4] -and $parts[4] -ne "NULL") {
        Write-Host "✅ FiscalAPIInvoiceId guardado correctamente: $($parts[4])" -ForegroundColor Green
        $invoiceId = $parts[4]
    }
    else {
        Write-Host "⚠️  FiscalAPIInvoiceId es NULL - se usará PDF local" -ForegroundColor Yellow
        $invoiceId = $null
    }
}
else {
    Write-Host "❌ No se encontró la factura en la base de datos" -ForegroundColor Red
    exit
}
Write-Host ""

# 4. Descargar PDF
Write-Host "[4/5] Descargando PDF..." -ForegroundColor Yellow

try {
    $pdfUrl = "http://localhost:64927/Factura/DescargarPDF?facturaId=$facturaId"
    $pdfPath = "Factura_${serie}${folio}_TEST.pdf"
    
    Invoke-WebRequest -Uri $pdfUrl -OutFile $pdfPath -UseBasicParsing
    
    $fileSize = (Get-Item $pdfPath).Length
    Write-Host "✅ PDF descargado: $pdfPath ($fileSize bytes)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error al descargar PDF: $_" -ForegroundColor Red
    exit
}
Write-Host ""

# 5. Verificar origen del PDF
Write-Host "[5/5] Analizando origen del PDF..." -ForegroundColor Yellow

$pdfContent = Get-Content $pdfPath -Raw -Encoding Byte
$pdfText = [System.Text.Encoding]::ASCII.GetString($pdfContent)

if ($pdfText -match "FiscalAPI" -or $pdfText -match "fiscalapi") {
    Write-Host "✅ PDF generado por FiscalAPI (OFICIAL)" -ForegroundColor Green
    Write-Host "   El PDF tiene el formato bonito y profesional de FiscalAPI" -ForegroundColor Gray
}
elseif ($pdfText -match "iTextSharp" -or $pdfText -match "iText") {
    Write-Host "⚠️  PDF generado localmente con iTextSharp (FALLBACK)" -ForegroundColor Yellow
    Write-Host "   Motivo probable: FiscalAPIInvoiceId no se guardó correctamente" -ForegroundColor Gray
}
else {
    Write-Host "⚠️  No se pudo determinar el origen del PDF" -ForegroundColor Yellow
}
Write-Host ""

# 6. Abrir PDF automáticamente
Write-Host "Abriendo PDF..." -ForegroundColor Cyan
Start-Process $pdfPath

Write-Host ""
Write-Host "=== RESUMEN ===" -ForegroundColor Cyan
Write-Host "Factura: $serie-$folio" -ForegroundColor White
Write-Host "UUID: $uuid" -ForegroundColor White
Write-Host "FacturaID: $facturaId" -ForegroundColor White
Write-Host "InvoiceId: $(if($invoiceId){"$invoiceId"}else{"NULL (usando PDF local)"})" -ForegroundColor White
Write-Host "PDF: $pdfPath" -ForegroundColor White
Write-Host ""
Write-Host "Revisa el PDF que se abrió. Debería:" -ForegroundColor Yellow
Write-Host "  - Tener el logo de FiscalAPI (si es PDF oficial)" -ForegroundColor Gray
Write-Host "  - Verse bonito y profesional" -ForegroundColor Gray
Write-Host "  - Tener todos los datos de la factura" -ForegroundColor Gray
Write-Host "  - Incluir el QR code y sellos digitales" -ForegroundColor Gray
