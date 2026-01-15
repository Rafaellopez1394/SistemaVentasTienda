# ========================================
# CONFIGURADOR FISCALAPI - SISTEMA DE VENTAS
# ========================================

Write-Host "`n╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   CONFIGURADOR FISCALAPI - SISTEMA DE VENTAS          ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Función para pedir credenciales
function Get-FiscalApiCredentials {
    Write-Host "📝 PASO 1: Ingresa tus credenciales de FiscalAPI`n" -ForegroundColor Yellow
    
    Write-Host "Ambiente:" -ForegroundColor Cyan
    Write-Host "  1. Test (pruebas) - https://test.fiscalapi.com" -ForegroundColor Gray
    Write-Host "  2. Live (producción) - https://live.fiscalapi.com" -ForegroundColor Gray
    $ambiente = Read-Host "`nSelecciona ambiente (1 o 2)"
    
    $esProduccion = if ($ambiente -eq "2") { 1 } else { 0 }
    $baseUrl = if ($ambiente -eq "2") { "https://live.fiscalapi.com" } else { "https://test.fiscalapi.com" }
    
    Write-Host "`n🔑 Ingresa tus credenciales:" -ForegroundColor Cyan
    $apiKey = Read-Host "API Key (sk_test_... o sk_live_...)"
    $tenant = Read-Host "Tenant ID (UUID)"
    
    return @{
        EsProduccion = $esProduccion
        BaseUrl = $baseUrl
        ApiKey = $apiKey
        Tenant = $tenant
    }
}

# Función para actualizar la base de datos
function Update-ConfiguracionPAC {
    param($config)
    
    Write-Host "`n📊 PASO 2: Actualizando base de datos..." -ForegroundColor Yellow
    
    $query = @"
UPDATE ConfiguracionPAC 
SET 
    ProveedorPAC = 'FiscalAPI',
    EsProduccion = $($config.EsProduccion),
    UrlTimbrado = '$($config.BaseUrl)/api/v4/invoices/income',
    UrlCancelacion = '$($config.BaseUrl)/api/v4/invoices',
    UrlConsulta = '$($config.BaseUrl)/api/v4/invoices/status',
    Usuario = '$($config.ApiKey)',
    Password = '$($config.Tenant)',
    RutaCertificado = NULL,
    RutaLlavePrivada = NULL,
    PasswordLlave = NULL,
    Activo = 1,
    FechaModificacion = GETDATE()
WHERE ConfigID = 3;

SELECT 'Configuración actualizada correctamente' as Resultado;
SELECT * FROM ConfiguracionPAC WHERE ConfigID = 3;
"@
    
    try {
        sqlcmd -S . -d DB_TIENDA -E -Q $query -W
        Write-Host "   ✅ Configuración actualizada correctamente" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "   ❌ Error al actualizar: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Función para verificar conectividad
function Test-FiscalApiConnection {
    param($config)
    
    Write-Host "`n🌐 PASO 3: Probando conexión con FiscalAPI..." -ForegroundColor Yellow
    
    try {
        $headers = @{
            "X-API-KEY" = $config.ApiKey
            "X-TENANT-KEY" = $config.Tenant
            "Accept" = "application/json"
        }
        
        # Intentar obtener catálogos (endpoint público)
        $response = Invoke-RestMethod -Uri "$($config.BaseUrl)/api/v4/catalogs/currency" -Headers $headers -Method GET
        
        Write-Host "   ✅ Conexión exitosa con FiscalAPI" -ForegroundColor Green
        Write-Host "   📋 API respondió correctamente" -ForegroundColor Gray
        return $true
    }
    catch {
        Write-Host "   ❌ Error de conexión: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "   💡 Verifica tus credenciales en: $($config.BaseUrl)" -ForegroundColor Yellow
        return $false
    }
}

# Función para verificar la venta de prueba
function Test-VentaPrueba {
    Write-Host "`n🔍 PASO 4: Verificando venta de prueba..." -ForegroundColor Yellow
    
    $query = @"
SELECT 
    v.VentaID,
    v.Total,
    v.FechaVenta,
    COUNT(dv.ProductoID) as NumProductos
FROM VentasClientes v
LEFT JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
WHERE v.VentaID = '6bc16123-7b85-418e-a4aa-62384726aa44'
GROUP BY v.VentaID, v.Total, v.FechaVenta
"@
    
    try {
        $result = sqlcmd -S . -d DB_TIENDA -E -Q $query -W
        
        if ($result -match "NumProductos") {
            Write-Host "   ✅ Venta de prueba encontrada" -ForegroundColor Green
            Write-Host $result -ForegroundColor Gray
            return $true
        }
        else {
            Write-Host "   ❌ Venta de prueba no encontrada" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "   ❌ Error al verificar venta: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Función principal
function Main {
    # Obtener credenciales
    $config = Get-FiscalApiCredentials
    
    if ([string]::IsNullOrWhiteSpace($config.ApiKey) -or [string]::IsNullOrWhiteSpace($config.Tenant)) {
        Write-Host "`n❌ Credenciales no proporcionadas. Saliendo..." -ForegroundColor Red
        return
    }
    
    # Actualizar base de datos
    $dbOk = Update-ConfiguracionPAC -config $config
    
    if (-not $dbOk) {
        Write-Host "`n❌ No se pudo actualizar la base de datos. Saliendo..." -ForegroundColor Red
        return
    }
    
    # Probar conexión
    $connOk = Test-FiscalApiConnection -config $config
    
    # Verificar venta de prueba
    $ventaOk = Test-VentaPrueba
    
    # Resumen
    Write-Host "`n╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║                    RESUMEN                             ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
    
    Write-Host "Base de datos:        $(if ($dbOk) {'✅ OK'} else {'❌ Error'})" -ForegroundColor $(if ($dbOk) {'Green'} else {'Red'})
    Write-Host "Conexión FiscalAPI:   $(if ($connOk) {'✅ OK'} else {'❌ Error'})" -ForegroundColor $(if ($connOk) {'Green'} else {'Red'})
    Write-Host "Venta de prueba:      $(if ($ventaOk) {'✅ OK'} else {'❌ Error'})" -ForegroundColor $(if ($ventaOk) {'Green'} else {'Red'})
    
    if ($dbOk -and $connOk -and $ventaOk) {
        Write-Host "`n🎉 ¡Todo listo! Ahora puedes probar la facturación." -ForegroundColor Green
        Write-Host "`n📝 Próximos pasos:" -ForegroundColor Cyan
        Write-Host "   1. Sube certificados CSD a FiscalAPI" -ForegroundColor Gray
        Write-Host "   2. Crea un emisor en FiscalAPI" -ForegroundColor Gray
        Write-Host "   3. Prueba generando una factura desde la aplicación" -ForegroundColor Gray
        Write-Host "`n🌐 Dashboard: $($config.BaseUrl)" -ForegroundColor Cyan
    }
    else {
        Write-Host "`n⚠️  Revisa los errores arriba y vuelve a intentar." -ForegroundColor Yellow
    }
    
    Write-Host "`n📚 Documentación completa: GUIA_FISCALAPI_CONFIGURACION.md" -ForegroundColor Cyan
}

# Ejecutar
Main

Read-Host "Presiona Enter para salir"
