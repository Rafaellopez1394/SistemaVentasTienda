# ================================================================
# Descargar Certificado de Prueba del SAT - EKU9003173C9
# ================================================================
# Este script descarga los certificados de prueba oficiales del SAT
# para el RFC EKU9003173C9 (ESCUELA KEMPER URGATE)
# ================================================================

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DESCARGA CERTIFICADO PRUEBA SAT" -ForegroundColor Cyan
Write-Host "RFC: EKU9003173C9" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Crear carpeta para certificados
$carpetaCerts = ".\Certificados_Prueba_EKU"
if (!(Test-Path $carpetaCerts)) {
    New-Item -ItemType Directory -Path $carpetaCerts | Out-Null
    Write-Host "[+] Carpeta creada: $carpetaCerts" -ForegroundColor Green
}

# URLs de los certificados de prueba del SAT
$urlCER = "https://github.com/phpcfdi/resources-sat-xml/raw/master/resources/certificates/EKU9003173C9.cer"
$urlKEY = "https://github.com/phpcfdi/resources-sat-xml/raw/master/resources/certificates/EKU9003173C9.key"

$archivoCER = Join-Path $carpetaCerts "EKU9003173C9.cer"
$archivoKEY = Join-Path $carpetaCerts "EKU9003173C9.key"

Write-Host "[*] Descargando certificado (.cer)..." -ForegroundColor Yellow
try {
    Invoke-WebRequest -Uri $urlCER -OutFile $archivoCER -UseBasicParsing
    Write-Host "[+] Certificado descargado: $archivoCER" -ForegroundColor Green
} catch {
    Write-Host "[!] Error al descargar .cer: $_" -ForegroundColor Red
    Write-Host "[!] Intentando URL alternativa..." -ForegroundColor Yellow
    
    # URL alternativa de Prodigia
    $urlCER_Alt = "https://www.prodigia.com.mx/archivos/EKU9003173C9.cer"
    try {
        Invoke-WebRequest -Uri $urlCER_Alt -OutFile $archivoCER -UseBasicParsing
        Write-Host "[+] Certificado descargado (URL alternativa): $archivoCER" -ForegroundColor Green
    } catch {
        Write-Host "[!] No se pudo descargar el certificado" -ForegroundColor Red
    }
}

Write-Host "[*] Descargando llave privada (.key)..." -ForegroundColor Yellow
try {
    Invoke-WebRequest -Uri $urlKEY -OutFile $archivoKEY -UseBasicParsing
    Write-Host "[+] Llave privada descargada: $archivoKEY" -ForegroundColor Green
} catch {
    Write-Host "[!] Error al descargar .key: $_" -ForegroundColor Red
    Write-Host "[!] Intentando URL alternativa..." -ForegroundColor Yellow
    
    # URL alternativa de Prodigia
    $urlKEY_Alt = "https://www.prodigia.com.mx/archivos/EKU9003173C9.key"
    try {
        Invoke-WebRequest -Uri $urlKEY_Alt -OutFile $archivoKEY -UseBasicParsing
        Write-Host "[+] Llave privada descargada (URL alternativa): $archivoKEY" -ForegroundColor Green
    } catch {
        Write-Host "[!] No se pudo descargar la llave privada" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "INFORMACIÓN DEL CERTIFICADO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "RFC: EKU9003173C9" -ForegroundColor White
Write-Host "Razón Social: ESCUELA KEMPER URGATE" -ForegroundColor White
Write-Host "Contraseña de la llave: 12345678a" -ForegroundColor Yellow
Write-Host ""
Write-Host "Archivos descargados:" -ForegroundColor White
Write-Host "  - Certificado: $archivoCER" -ForegroundColor White
Write-Host "  - Llave privada: $archivoKEY" -ForegroundColor White
Write-Host ""

if ((Test-Path $archivoCER) -and (Test-Path $archivoKEY)) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "DESCARGA COMPLETADA" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "PRÓXIMOS PASOS:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Ve a https://pruebas.pade.mx/" -ForegroundColor White
    Write-Host "2. Inicia sesión con tu cuenta" -ForegroundColor White
    Write-Host "3. Ve a: Configuración > Mis Certificados" -ForegroundColor White
    Write-Host "4. Elimina el certificado 'sistemafacturacion' anterior" -ForegroundColor White
    Write-Host "5. Sube el nuevo certificado:" -ForegroundColor White
    Write-Host "   - Archivo .cer: $archivoCER" -ForegroundColor Yellow
    Write-Host "   - Archivo .key: $archivoKEY" -ForegroundColor Yellow
    Write-Host "   - Contraseña: 12345678a" -ForegroundColor Yellow
    Write-Host "6. Márcalo como 'Default'" -ForegroundColor White
    Write-Host ""
    
    # Abrir carpeta
    Write-Host "[*] Abriendo carpeta con los certificados..." -ForegroundColor Cyan
    Start-Process explorer.exe -ArgumentList $carpetaCerts
    
} else {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR EN DESCARGA" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "No se pudieron descargar los archivos automáticamente." -ForegroundColor Red
    Write-Host ""
    Write-Host "DESCARGA MANUAL:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Contacta a soporte@prodigia.com.mx" -ForegroundColor White
    Write-Host "2. Solicita los certificados de prueba para RFC: EKU9003173C9" -ForegroundColor White
    Write-Host "3. O visita: https://www.prodigia.com.mx/certificados-prueba" -ForegroundColor White
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
