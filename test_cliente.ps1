# Test script for Cliente/Guardar endpoint

# Stop any existing IIS Express
Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Copy DLLs
$sourceDLL_CapaDatos = 'C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\bin\Debug\CapaDatos.dll'
$sourceDLL_CapaModelo = 'C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaModelo\bin\Debug\CapaModelo.dll'
$destBin = 'C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\bin'

if (Test-Path $sourceDLL_CapaDatos) {
    Copy-Item $sourceDLL_CapaDatos $destBin -Force | Out-Null
    Write-Host "Copied CapaDatos.dll"
}

if (Test-Path $sourceDLL_CapaModelo) {
    Copy-Item $sourceDLL_CapaModelo $destBin -Force | Out-Null
    Write-Host "Copied CapaModelo.dll"
}

# Start IIS Express
Write-Host "Starting IIS Express..."
$p = Start-Process -FilePath 'C:\Program Files\IIS Express\iisexpress.exe' `
    -ArgumentList '/site:VentasWeb','/config:"c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\.vs\VentasWeb\config\applicationhost.config"' `
    -PassThru -WindowStyle Hidden

Write-Host "IIS started (PID: $($p.Id))"
Start-Sleep -Seconds 5

# Test 1: ObtenerCatalogos
Write-Host "`n=== Test 1: Get Catalogs ==="
try {
    $response = Invoke-WebRequest -Uri 'http://localhost:64927/Cliente/ObtenerCatalogos' -TimeoutSec 10 -UseBasicParsing
    $data = $response.Content | ConvertFrom-Json
    Write-Host "OK - Got $($data.tiposCredito.Count) credit types"
} catch {
    Write-Host "ERROR: $($_.Exception.Message)"
}

# Test 2: Guardar cliente CON tipos de crédito
Write-Host "`n=== Test 2: Save Client with Credit Types ==="
$payload = @{
    objeto = @{
        ClienteID = "00000000-0000-0000-0000-000000000001"
        RFC = "GUADA123ABC"
        RazonSocial = "Test Company S.A."
        CorreoElectronico = "test@example.com"
        Telefono = "6877778899"
        CodigoPostal = "80100"
        RegimenFiscalID = "601"
        UsoCFDIID = "G03"
    }
    tiposCreditoIDs = @(1)
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri 'http://localhost:64927/Cliente/Guardar' `
        -Method POST `
        -ContentType 'application/json' `
        -Body $payload `
        -TimeoutSec 10 `
        -UseBasicParsing
    
    $result = $response.Content | ConvertFrom-Json
    Write-Host "Response: $($response.Content)"
    
    if ($result.resultado) {
        Write-Host "✅ Client saved successfully"
        
        # Test 3: Get saved client data
        Write-Host "`n=== Test 3: Retrieve Saved Client ==="
        $getResponse = Invoke-WebRequest -Uri 'http://localhost:64927/Cliente/ObtenerPorId?id=00000000-0000-0000-0000-000000000001' `
            -TimeoutSec 10 `
            -UseBasicParsing
        
        $clientData = $getResponse.Content | ConvertFrom-Json
        Write-Host "Retrieved client: $($clientData.cliente.RazonSocial)"
        Write-Host "Credit types assigned: $($clientData.creditos.Count)"
        Write-Host "Full response: $($getResponse.Content)"
    } else {
        Write-Host "❌ Save failed: $($result.mensaje)"
    }
} catch {
    Write-Host "ERROR: $($_.Exception.Message)"
    Write-Host "StatusCode: $($_.Exception.Response.StatusCode)"
}

Write-Host "`n=== Test Complete ==="
Write-Host "Press any key to stop IIS..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force
