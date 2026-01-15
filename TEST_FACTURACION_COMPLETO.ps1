# ======================================================================
# TEST COMPLETO DE FACTURACION - SISTEMA POS
# ======================================================================
# Ejecutar este script después de iniciar IIS Express (F5 en Visual Studio)
# ======================================================================

$baseUrl = "http://localhost:64927"
$timeout = 5

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "PRUEBA DE FACTURACION - SISTEMA POS" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

# 1. VERIFICAR QUE EL SITIO ESTÉ CORRIENDO
Write-Host "`n[1/5] Verificando que IIS Express esté corriendo..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $baseUrl -TimeoutSec $timeout -UseBasicParsing
    Write-Host "✅ Sitio ACTIVO en puerto 64927" -ForegroundColor Green
} catch {
    Write-Host "❌ ERROR: Sitio NO está corriendo" -ForegroundColor Red
    Write-Host "   Acción requerida: Presiona F5 en Visual Studio para iniciar IIS Express" -ForegroundColor Yellow
    exit 1
}

# 2. VERIFICAR CONFIGURACION EN BASE DE DATOS
Write-Host "`n[2/5] Verificando configuración en base de datos..." -ForegroundColor Yellow
try {
    $query = "SELECT ProveedorPAC, CASE WHEN EsProduccion = 1 THEN 'PRODUCCION' ELSE 'PRUEBAS' END AS Ambiente, CASE WHEN Activo = 1 THEN 'SI' ELSE 'NO' END AS Activo FROM ConfiguracionPAC"
    $config = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $query
    
    Write-Host "   Proveedor PAC: $($config.ProveedorPAC)" -ForegroundColor Cyan
    Write-Host "   Ambiente: $($config.Ambiente)" -ForegroundColor Cyan
    Write-Host "   Activo: $($config.Activo)" -ForegroundColor Cyan
    
    if ($config.Activo -ne "SI") {
        Write-Host "❌ ERROR: Configuración PAC no está activa" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Configuración PAC correcta" -ForegroundColor Green
} catch {
    Write-Host "❌ ERROR al verificar configuración: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 3. VERIFICAR CERTIFICADOS
Write-Host "`n[3/5] Verificando certificados digitales..." -ForegroundColor Yellow
$certPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\Certifies"
$certFile = Join-Path $certPath "GAMA6111156JA.cer"
$keyFile = Join-Path $certPath "GAMA6111156JA.key"
$passwordFile = Join-Path $certPath "password"

if (Test-Path $certFile) {
    Write-Host "   ✅ Certificado (.cer): $('{0:N0}' -f (Get-Item $certFile).Length) bytes" -ForegroundColor Cyan
} else {
    Write-Host "   ❌ NO EXISTE: GAMA6111156JA.cer" -ForegroundColor Red
    exit 1
}

if (Test-Path $keyFile) {
    Write-Host "   ✅ Llave privada (.key): $('{0:N0}' -f (Get-Item $keyFile).Length) bytes" -ForegroundColor Cyan
} else {
    Write-Host "   ❌ NO EXISTE: GAMA6111156JA.key" -ForegroundColor Red
    exit 1
}

if (Test-Path $passwordFile) {
    $password = Get-Content $passwordFile
    Write-Host "   ✅ Contraseña: $password" -ForegroundColor Cyan
} else {
    Write-Host "   ❌ NO EXISTE: password" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Certificados encontrados" -ForegroundColor Green

# 4. VERIFICAR DATOS DE VENTA DE PRUEBA
Write-Host "`n[4/5] Verificando datos de venta de prueba..." -ForegroundColor Yellow
$ventaID = "9f035d37-8764-4aa6-b71a-041dffd940b0"
$queryVenta = "SELECT v.VentaID, v.Total, COUNT(dv.ProductoID) AS NumProductos FROM VentasClientes v INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID WHERE v.VentaID = '$ventaID' GROUP BY v.VentaID, v.Total"
$venta = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryVenta

if ($venta) {
    Write-Host "   VentaID: $($venta.VentaID)" -ForegroundColor Cyan
    Write-Host "   Total: $([decimal]$venta.Total | ForEach-Object { $_.ToString('C') })" -ForegroundColor Cyan
    Write-Host "   Productos: $($venta.NumProductos)" -ForegroundColor Cyan
    Write-Host "✅ Venta encontrada" -ForegroundColor Green
} else {
    Write-Host "❌ ERROR: Venta no encontrada" -ForegroundColor Red
    exit 1
}

# 5. GENERAR FACTURA
Write-Host "`n[5/5] Generando factura..." -ForegroundColor Yellow
Write-Host "   Enviando request a $baseUrl/FacturaTest/GenerarFactura..." -ForegroundColor Cyan

$body = @{
    VentaID = $ventaID
    ReceptorRFC = "XAXX010101000"  # RFC válido para pruebas (Público General)
    ReceptorNombre = "PUBLICO GENERAL"
    ReceptorRegimenFiscal = "616"  # Sin obligaciones fiscales
    UsoCFDI = "S01"  # Sin efectos fiscales
    ReceptorCP = "81100"
    FormaPago = "01"  # Efectivo
    MetodoPago = "PUE"  # Pago en una sola exhibición
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/FacturaTest/GenerarFactura" `
                                   -Method POST `
                                   -Headers @{"Content-Type"="application/json"} `
                                   -Body $body `
                                   -TimeoutSec 30
    
    Write-Host "`n======================================================================" -ForegroundColor Cyan
    Write-Host "RESULTADO DE FACTURACION" -ForegroundColor Cyan
    Write-Host "======================================================================" -ForegroundColor Cyan
    
    if ($response.estado -eq $true) {
        Write-Host "`n✅✅✅ FACTURA GENERADA EXITOSAMENTE ✅✅✅" -ForegroundColor Green
        Write-Host "`nMensaje: $($response.mensaje)" -ForegroundColor Green
        
        if ($response.objeto) {
            Write-Host "`nDatos de la factura:" -ForegroundColor Cyan
            Write-Host "   UUID: $($response.objeto.UUID)" -ForegroundColor White
            Write-Host "   FacturaID: $($response.objeto.FacturaID)" -ForegroundColor White
            Write-Host "   Serie-Folio: $($response.objeto.Serie)-$($response.objeto.Folio)" -ForegroundColor White
            Write-Host "   Emisor: $($response.objeto.EmisorRFC)" -ForegroundColor White
            Write-Host "   Receptor: $($response.objeto.ReceptorRFC)" -ForegroundColor White
            Write-Host "   Total: $([decimal]$response.objeto.Total | ForEach-Object { $_.ToString('C') })" -ForegroundColor White
            
            if ($response.objeto.XMLTimbrado) {
                Write-Host "`n   XML Timbrado recibido: $($response.objeto.XMLTimbrado.Length) caracteres" -ForegroundColor Cyan
            }
        }
        
        Write-Host "`n======================================================================" -ForegroundColor Cyan
        Write-Host "SISTEMA DE FACTURACION FUNCIONANDO CORRECTAMENTE" -ForegroundColor Green
        Write-Host "======================================================================" -ForegroundColor Cyan
        
    } else {
        Write-Host "`n❌❌❌ ERROR AL GENERAR FACTURA ❌❌❌" -ForegroundColor Red
        Write-Host "`nMensaje: $($response.mensaje)" -ForegroundColor Red
        
        if ($response.objeto) {
            Write-Host "`nDetalles del error:" -ForegroundColor Yellow
            $response.objeto | ConvertTo-Json -Depth 5 | Write-Host -ForegroundColor Yellow
        }
        
        Write-Host "`n⚠️  REVISAR LOGS EN VISUAL STUDIO (Output > Debug)" -ForegroundColor Yellow
        Write-Host "   Buscar mensajes que inicien con '===' para ver el flujo completo" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "`n❌❌❌ ERROR HTTP AL GENERAR FACTURA ❌❌❌" -ForegroundColor Red
    Write-Host "`nError: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        Write-Host "Status Description: $($_.Exception.Response.StatusDescription)" -ForegroundColor Red
    }
    
    Write-Host "`n⚠️  REVISAR LOGS EN VISUAL STUDIO (Output > Debug)" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n"
Read-Host "Presiona ENTER para salir"
