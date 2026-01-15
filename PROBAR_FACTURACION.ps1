# Script para probar la facturación completa con FiscalAPI
# Autor: GitHub Copilot
# Fecha: 2026-01-09

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRUEBA DE FACTURACIÓN CON FISCALAPI" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar que IIS Express esté corriendo
Write-Host "[1] Verificando que IIS Express esté activo..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:64927" -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
    Write-Host "    ✓ IIS Express está activo en puerto 64927" -ForegroundColor Green
} catch {
    Write-Host "    ✗ IIS Express NO está corriendo en puerto 64927" -ForegroundColor Red
    Write-Host "    Por favor, inicia el proyecto desde Visual Studio (F5)" -ForegroundColor Yellow
    exit 1
}

# 2. Verificar certificados
Write-Host ""
Write-Host "[2] Verificando certificados..." -ForegroundColor Yellow
$certPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\Certifies"
$cerFile = Join-Path $certPath "GAMA6111156JA.cer"
$keyFile = Join-Path $certPath "GAMA6111156JA.key"
$passFile = Join-Path $certPath "password"

if (Test-Path $cerFile) {
    $cerSize = (Get-Item $cerFile).Length
    Write-Host "    ✓ Certificado (.cer): $cerSize bytes" -ForegroundColor Green
} else {
    Write-Host "    ✗ NO se encontró: $cerFile" -ForegroundColor Red
    exit 1
}

if (Test-Path $keyFile) {
    $keySize = (Get-Item $keyFile).Length
    Write-Host "    ✓ Llave privada (.key): $keySize bytes" -ForegroundColor Green
} else {
    Write-Host "    ✗ NO se encontró: $keyFile" -ForegroundColor Red
    exit 1
}

if (Test-Path $passFile) {
    $password = Get-Content $passFile -Raw
    Write-Host "    ✓ Password: $($password.Trim())" -ForegroundColor Green
} else {
    Write-Host "    ✗ NO se encontró: $passFile" -ForegroundColor Red
    exit 1
}

# 3. Verificar configuración en base de datos
Write-Host ""
Write-Host "[3] Verificando configuración en base de datos..." -ForegroundColor Yellow
$sqlVerificar = @"
SELECT 
    -- ConfiguracionPAC
    cp.ProveedorPAC,
    cp.EsPrueba,
    CASE WHEN cp.EsPrueba = 1 THEN cp.URLWebServiceTest ELSE cp.URLWebService END AS URLActiva,
    LEFT(cp.UsuarioAPI, 20) + '...' AS APIKey,
    
    -- ConfiguracionEmpresa
    ce.RFC AS EmisorRFC,
    ce.RazonSocial AS EmisorNombre,
    ce.RegimenFiscal AS EmisorRegimen,
    ce.CodigoPostal AS LugarExpedicion,
    ce.NombreArchivoCertificado,
    ce.NombreArchivoLlavePrivada,
    ce.NombreArchivoPassword
FROM ConfiguracionPAC cp
CROSS JOIN ConfiguracionEmpresa ce
WHERE cp.Activo = 1 AND ce.ConfigEmpresaID = 1;
"@

try {
    $configResult = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $sqlVerificar -ErrorAction Stop
    
    Write-Host "    Proveedor PAC: $($configResult.ProveedorPAC)" -ForegroundColor Cyan
    Write-Host "    Ambiente: $(if($configResult.EsPrueba -eq 1){'PRUEBAS'}else{'PRODUCCIÓN'})" -ForegroundColor Cyan
    Write-Host "    URL: $($configResult.URLActiva)" -ForegroundColor Cyan
    Write-Host "    API Key: $($configResult.APIKey)" -ForegroundColor Cyan
    Write-Host "    Emisor RFC: $($configResult.EmisorRFC)" -ForegroundColor Cyan
    Write-Host "    Emisor: $($configResult.EmisorNombre)" -ForegroundColor Cyan
    Write-Host "    ✓ Configuración encontrada" -ForegroundColor Green
} catch {
    Write-Host "    ✗ Error al consultar base de datos: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 4. Leer JSON de prueba
Write-Host ""
Write-Host "[4] Leyendo datos de prueba..." -ForegroundColor Yellow
$jsonFile = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\test_generar_factura.json"
if (Test-Path $jsonFile) {
    $jsonContent = Get-Content $jsonFile -Raw
    $testData = $jsonContent | ConvertFrom-Json
    Write-Host "    VentaID: $($testData.VentaID)" -ForegroundColor Cyan
    Write-Host "    Receptor RFC: $($testData.ReceptorRFC)" -ForegroundColor Cyan
    Write-Host "    Receptor Nombre: $($testData.ReceptorNombre)" -ForegroundColor Cyan
    Write-Host "    ✓ Datos de prueba cargados" -ForegroundColor Green
} else {
    Write-Host "    ✗ NO se encontró: $jsonFile" -ForegroundColor Red
    Write-Host "    Creando archivo de prueba..." -ForegroundColor Yellow
    
    # Obtener una venta de ejemplo de la BD
    $sqlVenta = @"
SELECT TOP 1 VentaID, ISNULL(RazonSocial, 'PUBLICO GENERAL') AS Cliente
FROM VentasClientes 
WHERE EstaFacturada = 0 AND Total > 0
ORDER BY FechaVenta DESC;
"@
    
    try {
        $venta = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $sqlVenta -ErrorAction Stop
        
        $jsonTest = @{
            VentaID = $venta.VentaID.ToString()
            ReceptorRFC = "XAXX010101000"
            ReceptorNombre = "PUBLICO GENERAL"
            ReceptorRegimenFiscal = "616"
            ReceptorUsoCFDI = "G03"
            ReceptorCP = "00000"
            ReceptorEmail = "test@example.com"
            FormaPago = "01"
            MetodoPago = "PUE"
        } | ConvertTo-Json
        
        $jsonTest | Out-File -FilePath $jsonFile -Encoding UTF8
        $jsonContent = $jsonTest
        Write-Host "    ✓ Archivo de prueba creado con VentaID: $($venta.VentaID)" -ForegroundColor Green
    } catch {
        Write-Host "    ✗ Error al obtener venta de ejemplo: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# 5. Enviar request de facturación
Write-Host ""
Write-Host "[5] Enviando request de facturación..." -ForegroundColor Yellow
Write-Host "    URL: http://localhost:64927/Factura/GenerarFactura" -ForegroundColor Cyan

try {
    $headers = @{
        "Content-Type" = "application/json; charset=utf-8"
        "Accept" = "application/json"
    }
    
    Write-Host "    Enviando..." -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri "http://localhost:64927/Factura/GenerarFactura" `
        -Method POST `
        -Headers $headers `
        -Body $jsonContent `
        -TimeoutSec 60 `
        -ErrorAction Stop
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "RESPUESTA DEL SERVIDOR" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    
    $response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor White
    
    Write-Host ""
    if ($response.estado -eq $true) {
        Write-Host "✓ FACTURA GENERADA EXITOSAMENTE" -ForegroundColor Green
        Write-Host ""
        Write-Host "UUID: $($response.objeto.UUID)" -ForegroundColor Cyan
        Write-Host "Serie-Folio: $($response.objeto.Serie)-$($response.objeto.Folio)" -ForegroundColor Cyan
        Write-Host "Total: $($response.objeto.Total)" -ForegroundColor Cyan
    } else {
        Write-Host "✗ ERROR AL GENERAR FACTURA" -ForegroundColor Red
        Write-Host "Mensaje: $($response.valor)" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR AL ENVIAR REQUEST" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Message: $($_.Exception.Message)" -ForegroundColor Yellow
    
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "Status Code: $statusCode" -ForegroundColor Yellow
        
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response Body: $responseBody" -ForegroundColor Yellow
        } catch {}
    }
    
    Write-Host ""
    Write-Host "REVISA LOS LOGS EN VISUAL STUDIO (Output > Debug)" -ForegroundColor Cyan
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRUEBA COMPLETADA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para ver logs detallados, revisa la ventana Output > Debug en Visual Studio" -ForegroundColor Yellow
Write-Host ""
