# Script para empaquetar el sistema para otra PC
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   EMPAQUETANDO SISTEMA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$publishPath = "C:\Publish\SistemaVentas"
$packagePath = "C:\SistemaVentasInstalador"
$webFolder = Join-Path $packagePath "Web"

if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: No existe $publishPath" -ForegroundColor Red
    Write-Host "Ejecuta primero PublicarCarpeta.ps1" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "`n[1/3] Creando carpeta de paquete..." -ForegroundColor Yellow
if (Test-Path $packagePath) {
    Remove-Item "$packagePath\*" -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $packagePath -Force | Out-Null
}
New-Item -ItemType Directory -Path $webFolder -Force | Out-Null
Write-Host "Carpeta creada: $packagePath" -ForegroundColor Green

Write-Host "`n[2/3] Copiando archivos del sistema..." -ForegroundColor Yellow
Write-Host "Esto puede tardar varios minutos..." -ForegroundColor Gray
Copy-Item "$publishPath\*" -Destination $webFolder -Recurse -Force
Write-Host "Archivos copiados" -ForegroundColor Green

Write-Host "`n[3/3] Copiando script instalador..." -ForegroundColor Yellow
$installerSource = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\InstaladorDestino.ps1"
Copy-Item $installerSource -Destination "$packagePath\INSTALAR.ps1" -Force
Write-Host "Instalador copiado" -ForegroundColor Green

# Crear archivo README
$readmeContent = @"
========================================
SISTEMA DE VENTAS - INSTALADOR
========================================

CONTENIDO:
- INSTALAR.ps1: Script de instalacion
- Web/: Archivos del sistema

PASOS PARA INSTALAR EN OTRA PC:

1. COPIAR esta carpeta completa a la PC destino

2. EN LA PC DESTINO, click derecho en INSTALAR.ps1
   -> Ejecutar con PowerShell (como Administrador)

3. El instalador:
   - Instalara IIS si no esta instalado
   - Copiara los archivos a C:\SistemaVentas
   - Configurara el sitio web en puerto 80
   - Iniciara el sitio

4. CONFIGURAR BASE DE DATOS:
   - Abrir: C:\SistemaVentas\Web.config
   - Buscar seccion <connectionStrings>
   - Actualizar:
     * Server: nombre servidor SQL
     * Database: DB_TIENDA
     * User Id: usuario SQL
     * Password: contraseña SQL

5. REINICIAR IIS:
   Abrir CMD como Administrador:
   iisreset

6. ACCEDER AL SISTEMA:
   http://localhost
   http://localhost/Login

REQUISITOS DE LA PC DESTINO:
- Windows Server 2012 R2 o superior
- Windows 10/11 Pro o superior
- .NET Framework 4.6 o superior
- SQL Server (local o remoto)

SOPORTE:
Si tienes problemas, revisa los eventos de IIS:
- Administrador de IIS -> Sitio -> Registro de errores
- Visor de eventos de Windows
"@

$readmeContent | Out-File -FilePath "$packagePath\LEEME.txt" -Encoding UTF8

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   EMPAQUETADO COMPLETADO" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Paquete creado en:" -ForegroundColor White
Write-Host "  $packagePath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Contenido:" -ForegroundColor Yellow
Write-Host "  - INSTALAR.ps1 (script de instalacion)" -ForegroundColor White
Write-Host "  - Web\ (archivos del sistema)" -ForegroundColor White
Write-Host "  - LEEME.txt (instrucciones)" -ForegroundColor White
Write-Host ""
Write-Host "PROXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "1. Copia la carpeta completa a USB/red" -ForegroundColor White
Write-Host "2. Llevala a la PC destino" -ForegroundColor White
Write-Host "3. Ejecuta INSTALAR.ps1 como administrador" -ForegroundColor White
Write-Host ""
Write-Host "Abriendo carpeta..." -ForegroundColor Gray
Start-Sleep -Seconds 2
explorer $packagePath
Write-Host ""
pause
