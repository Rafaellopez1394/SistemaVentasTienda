# Script para obtener InvoiceId de FiscalAPI usando el UUID
# Esto te permitirá actualizar facturas antiguas con su InvoiceId

# Configuración (obtener de la base de datos ConfiguracionFiscalAPI)
$apiKey = "sk_test_47126aed_6c71_4060_b05b_932c4423dd00"
$tenant = "e0a0d1de-d225-46de-b95f-55d04f2787ff"
$baseUrl = "https://test.fiscalapi.com"

# UUID de la factura (cambiar según sea necesario)
$uuid = "013416dd-b424-454d-89be-91a62f9a1da7"

# Headers
$headers = @{
    "X-API-KEY" = $apiKey
    "X-TENANT-KEY" = $tenant
    "X-TIME-ZONE" = "America/Mexico_City"
    "Content-Type" = "application/json"
}

Write-Host "=== Consultando factura en FiscalAPI ===" -ForegroundColor Cyan
Write-Host "UUID: $uuid"
Write-Host ""

try {
    # Consultar el CFDI por UUID
    $endpoint = "$baseUrl/api/v4/cfdi40?uuid=$uuid"
    Write-Host "Endpoint: $endpoint" -ForegroundColor Gray
    
    $response = Invoke-RestMethod -Uri $endpoint -Method GET -Headers $headers -ErrorAction Stop
    
    if ($response.succeeded -and $response.data) {
        Write-Host "✅ Factura encontrada!" -ForegroundColor Green
        Write-Host ""
        
        $invoice = $response.data[0]
        
        Write-Host "InvoiceId: $($invoice.invoiceId)" -ForegroundColor Yellow
        Write-Host "UUID: $($invoice.invoiceUuid)" -ForegroundColor Gray
        Write-Host "Serie: $($invoice.invoiceSeries)" -ForegroundColor Gray
        Write-Host "Folio: $($invoice.invoiceFolio)" -ForegroundColor Gray
        Write-Host "Fecha: $($invoice.invoiceDate)" -ForegroundColor Gray
        Write-Host "Total: $($invoice.invoiceTotal)" -ForegroundColor Gray
        Write-Host "Receptor: $($invoice.clientName)" -ForegroundColor Gray
        Write-Host ""
        
        # Generar comando SQL para actualizar
        Write-Host "=== Comando SQL para actualizar ===" -ForegroundColor Cyan
        $sqlUpdate = @"
UPDATE Facturas
SET FiscalAPIInvoiceId = '$($invoice.invoiceId)'
WHERE UUID = '$uuid'

-- Verificar
SELECT FacturaID, Serie, Folio, UUID, FiscalAPIInvoiceId
FROM Facturas
WHERE UUID = '$uuid'
"@
        Write-Host $sqlUpdate -ForegroundColor Green
        Write-Host ""
        
        # Guardar en archivo
        $sqlUpdate | Out-File -FilePath "UPDATE_INVOICEID_$uuid.sql" -Encoding UTF8
        Write-Host "✅ SQL guardado en: UPDATE_INVOICEID_$uuid.sql" -ForegroundColor Green
        Write-Host ""
        
        # Preguntar si ejecutar automáticamente
        $ejecutar = Read-Host "¿Ejecutar UPDATE en la base de datos ahora? (S/N)"
        if ($ejecutar -eq "S" -or $ejecutar -eq "s") {
            sqlcmd -S localhost -d DB_TIENDA -E -Q $sqlUpdate
            Write-Host "✅ Base de datos actualizada!" -ForegroundColor Green
        }
    }
    else {
        Write-Host "❌ No se encontró la factura" -ForegroundColor Red
        Write-Host "Response: $($response | ConvertTo-Json -Depth 5)" -ForegroundColor Gray
    }
}
catch {
    Write-Host "❌ Error al consultar FiscalAPI" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "=== Para consultar otras facturas ===" -ForegroundColor Cyan
Write-Host "Cambia la variable `$uuid en la línea 10 del script" -ForegroundColor Gray
