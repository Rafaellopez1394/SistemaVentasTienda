# ======================================================================
# VERIFICAR CONFIGURACION DE FACTURACION (SIN INICIAR IIS)
# ======================================================================
# Este script verifica toda la configuración necesaria para facturar
# sin necesidad de tener IIS Express corriendo
# ======================================================================

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "VERIFICACION DE CONFIGURACION - SISTEMA DE FACTURACION" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

$errores = 0
$advertencias = 0

# 1. COMPILACION
Write-Host "`n[1/6] Verificando compilación..." -ForegroundColor Yellow
$dllPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\bin\Debug\CapaDatos.dll"
if (Test-Path $dllPath) {
    $dllInfo = Get-Item $dllPath
    Write-Host "   ✅ CapaDatos.dll compilado" -ForegroundColor Green
    Write-Host "      Fecha: $($dllInfo.LastWriteTime)" -ForegroundColor Cyan
    Write-Host "      Tamaño: $('{0:N0}' -f $dllInfo.Length) bytes" -ForegroundColor Cyan
} else {
    Write-Host "   ❌ ERROR: CapaDatos.dll no encontrado" -ForegroundColor Red
    Write-Host "      Acción: Compilar solución (Ctrl+Shift+B en Visual Studio)" -ForegroundColor Yellow
    $errores++
}

# 2. BASE DE DATOS
Write-Host "`n[2/6] Verificando configuración en base de datos..." -ForegroundColor Yellow
try {
    # Verificar ConfiguracionPAC
    $queryPAC = "SELECT ProveedorPAC, CASE WHEN EsProduccion = 1 THEN 'PRODUCCION' ELSE 'PRUEBAS' END AS Ambiente, LEFT(Usuario, 40) AS APIKey, CASE WHEN Activo = 1 THEN 'SI' ELSE 'NO' END AS Activo FROM ConfiguracionPAC"
    $configPAC = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryPAC
    
    Write-Host "   ConfiguracionPAC:" -ForegroundColor Cyan
    Write-Host "      Proveedor: $($configPAC.ProveedorPAC)" -ForegroundColor White
    Write-Host "      Ambiente: $($configPAC.Ambiente)" -ForegroundColor White
    Write-Host "      API Key: $($configPAC.APIKey)..." -ForegroundColor White
    Write-Host "      Activo: $($configPAC.Activo)" -ForegroundColor White
    
    if ($configPAC.Activo -ne "SI") {
        Write-Host "   ⚠️  ADVERTENCIA: ConfiguracionPAC no está activa" -ForegroundColor Yellow
        $advertencias++
    }
    
    # Verificar ConfiguracionEmpresa
    $queryEmpresa = "SELECT RFC, RazonSocial, RegimenFiscal, CodigoPostal, NombreArchivoCertificado, NombreArchivoLlavePrivada, NombreArchivoPassword FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1"
    $configEmpresa = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryEmpresa
    
    Write-Host "`n   ConfiguracionEmpresa:" -ForegroundColor Cyan
    Write-Host "      RFC: $($configEmpresa.RFC)" -ForegroundColor White
    Write-Host "      Razón Social: $($configEmpresa.RazonSocial)" -ForegroundColor White
    Write-Host "      Régimen: $($configEmpresa.RegimenFiscal)" -ForegroundColor White
    Write-Host "      CP: $($configEmpresa.CodigoPostal)" -ForegroundColor White
    Write-Host "      Certificado: $($configEmpresa.NombreArchivoCertificado)" -ForegroundColor White
    Write-Host "      Llave: $($configEmpresa.NombreArchivoLlavePrivada)" -ForegroundColor White
    Write-Host "      Password file: $($configEmpresa.NombreArchivoPassword)" -ForegroundColor White
    
    Write-Host "   ✅ Configuración de base de datos correcta" -ForegroundColor Green
    
} catch {
    Write-Host "   ❌ ERROR al verificar base de datos: $($_.Exception.Message)" -ForegroundColor Red
    $errores++
}

# 3. CERTIFICADOS DIGITALES
Write-Host "`n[3/6] Verificando certificados digitales..." -ForegroundColor Yellow
$certPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\Certifies"

if (Test-Path $certPath) {
    Write-Host "   ✅ Carpeta Certifies existe: $certPath" -ForegroundColor Green
    
    # Verificar certificado .cer
    $cerFile = Join-Path $certPath "$($configEmpresa.NombreArchivoCertificado)"
    if (Test-Path $cerFile) {
        $cerInfo = Get-Item $cerFile
        Write-Host "   ✅ Certificado (.cer): $('{0:N0}' -f $cerInfo.Length) bytes" -ForegroundColor Green
    } else {
        Write-Host "   ❌ ERROR: No existe $($configEmpresa.NombreArchivoCertificado)" -ForegroundColor Red
        $errores++
    }
    
    # Verificar llave privada .key
    $keyFile = Join-Path $certPath "$($configEmpresa.NombreArchivoLlavePrivada)"
    if (Test-Path $keyFile) {
        $keyInfo = Get-Item $keyFile
        Write-Host "   ✅ Llave privada (.key): $('{0:N0}' -f $keyInfo.Length) bytes" -ForegroundColor Green
    } else {
        Write-Host "   ❌ ERROR: No existe $($configEmpresa.NombreArchivoLlavePrivada)" -ForegroundColor Red
        $errores++
    }
    
    # Verificar archivo de contraseña
    $passwordFile = Join-Path $certPath "$($configEmpresa.NombreArchivoPassword)"
    if (Test-Path $passwordFile) {
        $password = Get-Content $passwordFile
        Write-Host "   ✅ Contraseña: $password" -ForegroundColor Green
    } else {
        Write-Host "   ❌ ERROR: No existe archivo '$($configEmpresa.NombreArchivoPassword)'" -ForegroundColor Red
        $errores++
    }
    
} else {
    Write-Host "   ❌ ERROR: No existe carpeta Certifies" -ForegroundColor Red
    Write-Host "      Ruta esperada: $certPath" -ForegroundColor Yellow
    $errores++
}

# 4. PRODUCTOS CON CODIGOS SAT
Write-Host "`n[4/6] Verificando productos con códigos SAT..." -ForegroundColor Yellow
try {
    $queryProductos = @"
    SELECT 
        COUNT(*) AS Total,
        SUM(CASE WHEN LEN(ClaveProdServSAT) = 8 THEN 1 ELSE 0 END) AS Con8Digitos,
        SUM(CASE WHEN LEN(ClaveProdServSAT) != 8 THEN 1 ELSE 0 END) AS Invalidos,
        SUM(CASE WHEN ClaveProdServSAT IS NULL OR ClaveProdServSAT = '' THEN 1 ELSE 0 END) AS SinCodigo
    FROM Productos
"@
    $statsProductos = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryProductos
    
    Write-Host "   Total productos: $($statsProductos.Total)" -ForegroundColor Cyan
    Write-Host "   Con código SAT válido (8 dígitos): $($statsProductos.Con8Digitos)" -ForegroundColor $(if ($statsProductos.Con8Digitos -gt 0) { "Green" } else { "Yellow" })
    Write-Host "   Con código inválido: $($statsProductos.Invalidos)" -ForegroundColor $(if ($statsProductos.Invalidos -gt 0) { "Yellow" } else { "Green" })
    Write-Host "   Sin código SAT: $($statsProductos.SinCodigo)" -ForegroundColor $(if ($statsProductos.SinCodigo -gt 0) { "Yellow" } else { "Green" })
    
    if ($statsProductos.Invalidos -gt 0 -or $statsProductos.SinCodigo -gt 0) {
        Write-Host "   ⚠️  ADVERTENCIA: Hay productos con códigos SAT inválidos o sin código" -ForegroundColor Yellow
        $advertencias++
    }
    
    # Mostrar ejemplo de producto correcto
    $queryEjemplo = "SELECT TOP 1 CodigoInterno, Nombre, ClaveProdServSAT, ClaveUnidadSAT FROM Productos WHERE LEN(ClaveProdServSAT) = 8"
    $ejemplo = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryEjemplo
    
    if ($ejemplo) {
        Write-Host "`n   Ejemplo de producto correcto:" -ForegroundColor Cyan
        Write-Host "      Código: $($ejemplo.CodigoInterno)" -ForegroundColor White
        Write-Host "      Nombre: $($ejemplo.Nombre)" -ForegroundColor White
        Write-Host "      ClaveProdServSAT: $($ejemplo.ClaveProdServSAT)" -ForegroundColor White
        Write-Host "      ClaveUnidadSAT: $($ejemplo.ClaveUnidadSAT)" -ForegroundColor White
    }
    
    Write-Host "   ✅ Verificación de productos completada" -ForegroundColor Green
    
} catch {
    Write-Host "   ❌ ERROR al verificar productos: $($_.Exception.Message)" -ForegroundColor Red
    $errores++
}

# 5. VENTA DE PRUEBA
Write-Host "`n[5/6] Verificando venta de prueba..." -ForegroundColor Yellow
$ventaID = "9f035d37-8764-4aa6-b71a-041dffd940b0"
try {
    $queryVenta = @"
    SELECT 
        v.VentaID,
        v.Total,
        v.FechaVenta,
        COUNT(dv.ProductoID) AS NumProductos
    FROM VentasClientes v
    INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
    WHERE v.VentaID = '$ventaID'
    GROUP BY v.VentaID, v.Total, v.FechaVenta
"@
    $venta = Invoke-Sqlcmd -ServerInstance "SISTEMAS\SERVIDOR" -Database "DB_TIENDA" -Query $queryVenta
    
    if ($venta) {
        Write-Host "   ✅ Venta de prueba encontrada" -ForegroundColor Green
        Write-Host "      VentaID: $($venta.VentaID)" -ForegroundColor Cyan
        Write-Host "      Total: $($venta.Total)" -ForegroundColor Cyan
        Write-Host "      Fecha: $($venta.FechaVenta)" -ForegroundColor Cyan
        Write-Host "      Productos: $($venta.NumProductos)" -ForegroundColor Cyan
    } else {
        Write-Host "   ⚠️  ADVERTENCIA: Venta de prueba no encontrada" -ForegroundColor Yellow
        Write-Host "      Se puede usar cualquier VentaID existente" -ForegroundColor Yellow
        $advertencias++
    }
    
} catch {
    Write-Host "   ❌ ERROR al verificar venta: $($_.Exception.Message)" -ForegroundColor Red
    $errores++
}

# 6. ARCHIVOS DE CODIGO
Write-Host "`n[6/6] Verificando archivos de código..." -ForegroundColor Yellow
$archivos = @(
    @{Ruta="CapaDatos\PAC\FiscalAPIDirectHTTP.cs"; Nombre="FiscalAPIDirectHTTP"; Requerido=$true},
    @{Ruta="CapaDatos\PAC\FiscalAPIPAC.cs"; Nombre="FiscalAPIPAC"; Requerido=$true},
    @{Ruta="CapaDatos\Generadores\CFDI40XMLGenerator.cs"; Nombre="CFDI40XMLGenerator"; Requerido=$true},
    @{Ruta="VentasWeb\Controllers\FacturaController.cs"; Nombre="FacturaController"; Requerido=$true},
    @{Ruta="TEST_FACTURACION_COMPLETO.ps1"; Nombre="Script de prueba"; Requerido=$false}
)

$basePathCode = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
foreach ($archivo in $archivos) {
    $fullPath = Join-Path $basePathCode $archivo.Ruta
    if (Test-Path $fullPath) {
        $fileInfo = Get-Item $fullPath
        Write-Host "   ✅ $($archivo.Nombre): $('{0:N0}' -f $fileInfo.Length) bytes" -ForegroundColor Green
    } else {
        if ($archivo.Requerido) {
            Write-Host "   ❌ ERROR: No existe $($archivo.Nombre)" -ForegroundColor Red
            $errores++
        } else {
            Write-Host "   ⚠️  ADVERTENCIA: No existe $($archivo.Nombre)" -ForegroundColor Yellow
            $advertencias++
        }
    }
}

# RESUMEN FINAL
Write-Host "`n======================================================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE VERIFICACION" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

if ($errores -eq 0 -and $advertencias -eq 0) {
    Write-Host "`n✅✅✅ TODO CORRECTO - SISTEMA LISTO PARA FACTURAR ✅✅✅" -ForegroundColor Green
    Write-Host "`nPróximo paso:" -ForegroundColor Cyan
    Write-Host "1. Iniciar IIS Express desde Visual Studio (F5)" -ForegroundColor White
    Write-Host "2. Ejecutar: .\TEST_FACTURACION_COMPLETO.ps1" -ForegroundColor White
} elseif ($errores -eq 0) {
    Write-Host "`n⚠️  SISTEMA FUNCIONAL CON ADVERTENCIAS ⚠️" -ForegroundColor Yellow
    Write-Host "`nAdvertencias encontradas: $advertencias" -ForegroundColor Yellow
    Write-Host "El sistema puede funcionar pero hay detalles que revisar" -ForegroundColor Yellow
} else {
    Write-Host "`n❌ ERRORES ENCONTRADOS - REQUIERE ATENCION ❌" -ForegroundColor Red
    Write-Host "`nErrores críticos: $errores" -ForegroundColor Red
    Write-Host "Advertencias: $advertencias" -ForegroundColor Yellow
    Write-Host "`nCorrija los errores antes de intentar facturar" -ForegroundColor Red
}

Write-Host "`n======================================================================" -ForegroundColor Cyan
Write-Host ""
Read-Host "Presiona ENTER para salir"
