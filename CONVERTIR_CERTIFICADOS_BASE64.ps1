# ================================================
# Script PowerShell para convertir certificados CSD a Base64
# ================================================
# Uso:
# 1. Colocar archivos .cer y .key en la misma carpeta que este script
# 2. Ejecutar: .\CONVERTIR_CERTIFICADOS_BASE64.ps1
# 3. Copiar los resultados al script SQL CARGAR_CERTIFICADOS_BASE64.sql

param(
    [string]$CerFile = "",
    [string]$KeyFile = ""
)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Convertir Certificados CSD a Base64" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Si no se proporcionaron archivos, buscar en la carpeta actual
if ([string]::IsNullOrWhiteSpace($CerFile)) {
    $cerFiles = Get-ChildItem -Path . -Filter "*.cer" | Select-Object -First 1
    if ($cerFiles) {
        $CerFile = $cerFiles.FullName
        Write-Host "Encontrado: $CerFile" -ForegroundColor Green
    }
    else {
        Write-Host "ERROR: No se encontró archivo .cer en la carpeta actual" -ForegroundColor Red
        Write-Host "Uso: .\CONVERTIR_CERTIFICADOS_BASE64.ps1 -CerFile 'ruta\archivo.cer' -KeyFile 'ruta\archivo.key'" -ForegroundColor Yellow
        exit
    }
}

if ([string]::IsNullOrWhiteSpace($KeyFile)) {
    $keyFiles = Get-ChildItem -Path . -Filter "*.key" | Select-Object -First 1
    if ($keyFiles) {
        $KeyFile = $keyFiles.FullName
        Write-Host "Encontrado: $KeyFile" -ForegroundColor Green
    }
    else {
        Write-Host "ERROR: No se encontró archivo .key en la carpeta actual" -ForegroundColor Red
        Write-Host "Uso: .\CONVERTIR_CERTIFICADOS_BASE64.ps1 -CerFile 'ruta\archivo.cer' -KeyFile 'ruta\archivo.key'" -ForegroundColor Yellow
        exit
    }
}

# Verificar que existen los archivos
if (-not (Test-Path $CerFile)) {
    Write-Host "ERROR: No se encontró el archivo: $CerFile" -ForegroundColor Red
    exit
}

if (-not (Test-Path $KeyFile)) {
    Write-Host "ERROR: No se encontró el archivo: $KeyFile" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "Leyendo archivos..." -ForegroundColor Yellow

# Leer archivos como bytes
$certBytes = [System.IO.File]::ReadAllBytes($CerFile)
$keyBytes = [System.IO.File]::ReadAllBytes($KeyFile)

# Convertir a Base64
$certBase64 = [Convert]::ToBase64String($certBytes)
$keyBase64 = [Convert]::ToBase64String($keyBytes)

Write-Host "Conversión completada!" -ForegroundColor Green
Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "RESULTADOS - Copiar al script SQL" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "-- Certificado (.cer) - Longitud: $($certBase64.Length) caracteres" -ForegroundColor Yellow
Write-Host "DECLARE @CertificadoBase64 VARCHAR(MAX) = '$certBase64'" -ForegroundColor White
Write-Host ""

Write-Host "-- Llave Privada (.key) - Longitud: $($keyBase64.Length) caracteres" -ForegroundColor Yellow
Write-Host "DECLARE @LlavePrivadaBase64 VARCHAR(MAX) = '$keyBase64'" -ForegroundColor White
Write-Host ""

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "SIGUIENTE PASO:" -ForegroundColor Cyan
Write-Host "1. Copiar las 2 líneas DECLARE de arriba" -ForegroundColor White
Write-Host "2. Pegar en el archivo CARGAR_CERTIFICADOS_BASE64.sql" -ForegroundColor White
Write-Host "3. Reemplazar las líneas vacías con estos valores" -ForegroundColor White
Write-Host "4. Ejecutar el script SQL" -ForegroundColor White
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Guardar también en un archivo temporal
$outputFile = "certificados_base64_output.txt"
@"
-- Resultado de conversión a Base64
-- Generado: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

-- Certificado (.cer) - Longitud: $($certBase64.Length) caracteres
DECLARE @CertificadoBase64 VARCHAR(MAX) = '$certBase64'

-- Llave Privada (.key) - Longitud: $($keyBase64.Length) caracteres  
DECLARE @LlavePrivadaBase64 VARCHAR(MAX) = '$keyBase64'

-- Password (certificado de prueba EKU9003173C9)
DECLARE @PasswordCertificado VARCHAR(100) = '12345678a'
"@ | Out-File -FilePath $outputFile -Encoding UTF8

Write-Host "Los resultados también se guardaron en: $outputFile" -ForegroundColor Green
Write-Host ""
