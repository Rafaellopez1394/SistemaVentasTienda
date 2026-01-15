# ================================================
# Script para descargar certificados de prueba de FiscalAPI
# ================================================

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Descargar Certificados de Prueba de FiscalAPI" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$certifiesPath = "CapaDatos\Certifies"
$tempPath = "temp_certificados"
$zipFile = "tax-files.zip"
$downloadUrl = "https://test.fiscalapi.com/files/tax-files/tax-files.zip"

# Verificar que existe la carpeta Certifies
if (-not (Test-Path $certifiesPath)) {
    Write-Host "ERROR: No existe la carpeta $certifiesPath" -ForegroundColor Red
    Write-Host "Ejecuta este script desde la raíz del proyecto" -ForegroundColor Yellow
    exit
}

Write-Host "Descargando certificados de prueba..." -ForegroundColor Yellow
Write-Host "URL: $downloadUrl" -ForegroundColor Gray
Write-Host ""

try {
    # Descargar archivo ZIP
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipFile
    Write-Host "✓ Descarga completada" -ForegroundColor Green
    
    # Crear carpeta temporal
    if (Test-Path $tempPath) {
        Remove-Item $tempPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $tempPath | Out-Null
    
    # Extraer ZIP
    Write-Host "Extrayendo archivos..." -ForegroundColor Yellow
    Expand-Archive -Path $zipFile -DestinationPath $tempPath -Force
    Write-Host "✓ Archivos extraídos" -ForegroundColor Green
    
    # Buscar archivos .cer y .key
    $cerFiles = Get-ChildItem -Path $tempPath -Filter "*.cer" -Recurse
    $keyFiles = Get-ChildItem -Path $tempPath -Filter "*.key" -Recurse
    
    if ($cerFiles.Count -eq 0) {
        Write-Host "ERROR: No se encontraron archivos .cer" -ForegroundColor Red
        exit
    }
    
    if ($keyFiles.Count -eq 0) {
        Write-Host "ERROR: No se encontraron archivos .key" -ForegroundColor Red
        exit
    }
    
    Write-Host ""
    Write-Host "Archivos encontrados:" -ForegroundColor Cyan
    
    # Copiar certificados
    foreach ($cerFile in $cerFiles) {
        $destPath = Join-Path $certifiesPath $cerFile.Name
        Copy-Item $cerFile.FullName $destPath -Force
        Write-Host "  ✓ $($cerFile.Name) → $certifiesPath\" -ForegroundColor Green
    }
    
    # Copiar llaves privadas
    foreach ($keyFile in $keyFiles) {
        $destPath = Join-Path $certifiesPath $keyFile.Name
        Copy-Item $keyFile.FullName $destPath -Force
        Write-Host "  ✓ $($keyFile.Name) → $certifiesPath\" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "CERTIFICADOS INSTALADOS CORRECTAMENTE" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Mostrar contenido de la carpeta Certifies
    Write-Host "Archivos en $certifiesPath\" -ForegroundColor Cyan
    Get-ChildItem $certifiesPath -File | ForEach-Object {
        $sizeKB = [math]::Round($_.Length / 1KB, 2)
        Write-Host "  $($_.Name) ($sizeKB KB)" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "INFORMACIÓN DEL CERTIFICADO DE PRUEBA" -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "RFC: EKU9003173C9" -ForegroundColor White
    Write-Host "Razón Social: ESCUELA KEMPER URGATE" -ForegroundColor White
    Write-Host "Password: 12345678a" -ForegroundColor White
    Write-Host "Ambiente: PRUEBAS (test.fiscalapi.com)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "SIGUIENTE PASO" -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "1. Verificar configuración en base de datos:" -ForegroundColor White
    Write-Host "   SELECT * FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Reiniciar aplicación web (F5 en Visual Studio)" -ForegroundColor White
    Write-Host ""
    Write-Host "3. Probar facturación:" -ForegroundColor White
    Write-Host "   POST http://localhost:64927/Factura/GenerarFactura" -ForegroundColor Gray
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    
    # Limpiar archivos temporales
    Write-Host ""
    Write-Host "Limpiando archivos temporales..." -ForegroundColor Yellow
    Remove-Item $zipFile -Force
    Remove-Item $tempPath -Recurse -Force
    Write-Host "✓ Limpieza completada" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Si no puedes descargar automáticamente:" -ForegroundColor Yellow
    Write-Host "1. Descarga manualmente: $downloadUrl" -ForegroundColor White
    Write-Host "2. Extrae el ZIP" -ForegroundColor White
    Write-Host "3. Copia los archivos .cer y .key a: $certifiesPath\" -ForegroundColor White
    Write-Host ""
}
