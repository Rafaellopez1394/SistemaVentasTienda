# Script para subir certificados CSD de prueba a FiscalAPI

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  SUBIR CERTIFICADOS CSD A FISCALAPI" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

$apiKey = "sk_test_16b2fc7c_460a_4ba0_867f_b53cad3266f9"
$tenant = "e0a0d1de-d225-46de-b95f-55d04f2787ff"

Write-Host "[INFO] Segun la documentacion:" -ForegroundColor Yellow
Write-Host "  - Primero hay que subir los certificados CSD (sellos SAT)" -ForegroundColor Gray
Write-Host "  - Usar certificados de prueba del SAT" -ForegroundColor Gray
Write-Host "  - RFC: EKU9003173C9, Password: 12345678a" -ForegroundColor Gray
Write-Host ""

# Descargat certificados de prueba si no existen
$cerFile = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\EKU9003173C9.cer"
$keyFile = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\EKU9003173C9.key"

if (-not (Test-Path $cerFile) -or -not (Test-Path $keyFile)) {
    Write-Host "[AVISO] Los certificados de prueba no estan en el directorio" -ForegroundColor Yellow
    Write-Host "Descargalos de: https://docs.fiscalapi.com/testing-data#certificados-de-prueba" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Archivos esperados:" -ForegroundColor Cyan
    Write-Host "  - $cerFile" -ForegroundColor Gray
    Write-Host "  - $keyFile" -ForegroundColor Gray
    Write-Host ""
    
    # Opcion alternativa: subir desde el dashboard web
    Write-Host "OPCION ALTERNATIVA (MAS FACIL):" -ForegroundColor Green
    Write-Host "1. Ve a: https://test.fiscalapi.com/tax-files" -ForegroundColor White
    Write-Host "2. Haz clic en 'Add Tax File'" -ForegroundColor White
    Write-Host "3. Descarga los certificados de prueba desde:" -ForegroundColor White
    Write-Host "   https://fiscalapi-resources.s3.amazonaws.com/certificates.zip" -ForegroundColor Cyan
    Write-Host "4. Descomprime y sube los archivos .cer y .key" -ForegroundColor White
    Write-Host "5. Password: 12345678a" -ForegroundColor White
    Write-Host ""
    
    $continuar = Read-Host "Ya subiste los certificados desde el dashboard? (s/n)"
    
    if ($continuar -ne "s") {
        Write-Host "Termina de subir los certificados y vuelve a ejecutar este script" -ForegroundColor Yellow
        exit 0
    }
}
else {
    Write-Host "[1] Convirtiendo certificados a Base64..." -ForegroundColor Yellow
    
    $cerBase64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($cerFile))
    $keyBase64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($keyFile))
    
    Write-Host "[OK] Certificados convertidos" -ForegroundColor Green
    
    Write-Host "`n[2] Subiendo certificados a FiscalAPI..." -ForegroundColor Yellow
    
    $payload = @{
        tin = "EKU9003173C9"
        password = "12345678a"
        certificate = $cerBase64
        privateKey = $keyBase64
    } | ConvertTo-Json
    
    $headers = @{
        "X-API-KEY" = $apiKey
        "X-TENANT-KEY" = $tenant
        "X-TIME-ZONE" = "America/Mexico_City"
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }
    
    try {
        $response = Invoke-RestMethod `
            -Uri "https://test.fiscalapi.com/api/v4/tax-files" `
            -Method POST `
            -Headers $headers `
            -Body $payload
        
        Write-Host "[OK] Certificados subidos exitosamente!" -ForegroundColor Green
        Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Gray
    }
    catch {
        Write-Host "[ERROR] Fallo al subir certificados" -ForegroundColor Red
        Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        Write-Host "Message: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "`nRespuesta:" -ForegroundColor Yellow
            Write-Host $responseBody -ForegroundColor Gray
        }
    }
}

Write-Host "`n[3] Verificando certificados en FiscalAPI..." -ForegroundColor Yellow

$headers = @{
    "X-API-KEY" = $apiKey
    "X-TENANT-KEY" = $tenant
    "X-TIME-ZONE" = "America/Mexico_City"
    "Accept" = "application/json"
}

try {
    $response = Invoke-RestMethod `
        -Uri "https://test.fiscalapi.com/api/v4/tax-files" `
        -Method GET `
        -Headers $headers
    
    Write-Host "[OK] Certificados encontrados:" -ForegroundColor Green
    
    if ($response.data.items.Count -gt 0) {
        foreach ($cert in $response.data.items) {
            Write-Host "`nCertificado:" -ForegroundColor Cyan
            Write-Host "  ID: $($cert.id)" -ForegroundColor Gray
            Write-Host "  RFC: $($cert.tin)" -ForegroundColor Gray
            Write-Host "  Nombre: $($cert.legalName)" -ForegroundColor Gray
            Write-Host "  Valido desde: $($cert.validFrom)" -ForegroundColor Gray
            Write-Host "  Valido hasta: $($cert.validTo)" -ForegroundColor Gray
            Write-Host "  Estado: $($cert.status)" -ForegroundColor Gray
        }
        
        Write-Host "`n========== LISTO ==========" -ForegroundColor Green
        Write-Host "Los certificados estan configurados" -ForegroundColor Green
        Write-Host "Ahora puedes generar facturas" -ForegroundColor Green
        Write-Host "===========================`n" -ForegroundColor Green
    }
    else {
        Write-Host "[AVISO] No se encontraron certificados" -ForegroundColor Yellow
        Write-Host "Sube los certificados desde: https://test.fiscalapi.com/tax-files" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "[ERROR] No se pudieron listar los certificados" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "`n========================================================`n" -ForegroundColor Cyan

Read-Host "Presiona Enter para salir"
