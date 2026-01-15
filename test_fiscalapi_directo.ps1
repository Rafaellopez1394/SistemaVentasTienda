$body = @{
    versionCode = "4.0"
    series = "A"
    date = "2026-01-11T19:30:00"
    paymentFormCode = "01"
    currencyCode = "MXN"
    typeCode = "I"
    expeditionZipCode = "42501"
    exportCode = "01"
    paymentMethodCode = "PUE"
    exchangeRate = 1.0
    issuer = @{
        tin = "EKU9003173C9"
        legalName = "ESCUELA KEMPER URGATE"
        taxRegimeCode = "601"
    }
    recipient = @{
        tin = "XAXX010101000"
        legalName = "Caracol sa de cv"
        zipCode = "06000"
        taxRegimeCode = "601"
        cfdiUseCode = "G03"
    }
    items = @(
        @{
            itemCode = "01010101"
            quantity = 1
            unitOfMeasurementCode = "H87"
            description = "Prueba de producto"
            unitPrice = 100.00
            taxObjectCode = "02"
            itemTaxes = @(
                @{
                    taxCode = "002"
                    taxTypeCode = "Tasa"
                    taxRate = 0.16
                    taxFlagCode = "T"
                }
            )
        }
    )
}

$headers = @{
    "X-API-KEY" = "sk_test_939d0633_dd47_4b6f_afea_f4c56582a011"
    "X-TENANT" = "e0a0d1de-d225-46de-b95f-55d04f2787ff"
    "Content-Type" = "application/json"
}

$jsonBody = $body | ConvertTo-Json -Depth 10

Write-Host "=== PROBANDO FISCALAPI SIN CERTIFICADOS ===" -ForegroundColor Cyan
Write-Host "Endpoint: https://test.fiscalapi.com/api/v4/invoices/income" -ForegroundColor Yellow
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "https://test.fiscalapi.com/api/v4/invoices/income" `
        -Method Post `
        -Headers $headers `
        -Body $jsonBody `
        -ContentType "application/json" `
        -ErrorAction Stop
    
    Write-Host "✅ ÉXITO!" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "❌ ERROR:" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    Write-Host "Status Description: $($_.Exception.Response.StatusDescription)" -ForegroundColor Red
    
    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    $responseBody = $reader.ReadToEnd()
    $reader.Close()
    
    Write-Host "`nResponse Body:" -ForegroundColor Yellow
    Write-Host $responseBody -ForegroundColor White
    
    try {
        $errorObj = $responseBody | ConvertFrom-Json
        Write-Host "`nError Formateado:" -ForegroundColor Yellow
        $errorObj | ConvertTo-Json -Depth 10
    }
    catch {
        Write-Host "No se pudo parsear como JSON"
    }
}
