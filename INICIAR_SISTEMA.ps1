Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   INICIAR SISTEMA POS - GUÍA RÁPIDA" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si el sitio está publicado en IIS
Write-Host "Opciones para iniciar el sistema:" -ForegroundColor Yellow
Write-Host ""

Write-Host "OPCIÓN 1: Visual Studio (Desarrollo)" -ForegroundColor Green
Write-Host "  1. Abre Visual Studio 2022" -ForegroundColor White
Write-Host "  2. Abre: VentasWeb.sln" -ForegroundColor White
Write-Host "  3. Presiona F5 (Run/Debug)" -ForegroundColor White
Write-Host "  4. El navegador abrirá automáticamente" -ForegroundColor White
Write-Host ""

Write-Host "OPCIÓN 2: IIS Express (Rápido)" -ForegroundColor Green
Write-Host "  1. Navega a la carpeta del proyecto" -ForegroundColor White
Write-Host "  2. Ejecuta:" -ForegroundColor White
Write-Host "     cd VentasWeb" -ForegroundColor Cyan
Write-Host '     & "C:\Program Files\IIS Express\iisexpress.exe" /path:$PWD /port:5000' -ForegroundColor Cyan
Write-Host "  3. Abre navegador: http://localhost:5000" -ForegroundColor White
Write-Host ""

Write-Host "OPCIÓN 3: IIS Local (Producción)" -ForegroundColor Green
Write-Host "  1. Ejecuta script de publicación:" -ForegroundColor White
Write-Host "     .\PublicarIIS.ps1" -ForegroundColor Cyan
Write-Host "  2. Abre navegador: http://localhost/VentasWeb" -ForegroundColor White
Write-Host ""

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "DESPUES DE INICIAR EL SISTEMA:" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Inicia sesion con tu usuario" -ForegroundColor White
Write-Host "2. En el menu lateral, busca:" -ForegroundColor White
Write-Host "   [Reportes] Reportes Avanzados" -ForegroundColor Green
Write-Host ""
Write-Host "3. Prueba los reportes:" -ForegroundColor White
Write-Host "   - Utilidad por Producto (Es rentable el camaron 21-25?)" -ForegroundColor Cyan
Write-Host "   - Estado de Resultados (Es rentable mi negocio?)" -ForegroundColor Cyan
Write-Host "   - Recuperacion de Credito (Estoy recuperando el credito?)" -ForegroundColor Cyan
Write-Host "   - Cartera de Clientes (Quienes estan morosos?)" -ForegroundColor Cyan
Write-Host ""

Write-Host "==========================================" -ForegroundColor Green
Write-Host "Quieres iniciar con IIS Express ahora? (S/N)" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Green
$respuesta = Read-Host

if ($respuesta -eq "S" -or $respuesta -eq "s") {
    Write-Host ""
    Write-Host "Iniciando IIS Express..." -ForegroundColor Green
    
    $proyectoPath = Join-Path $PSScriptRoot "VentasWeb"
    
    if (Test-Path $proyectoPath) {
        Write-Host "Ruta del proyecto: $proyectoPath" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Presiona Ctrl+C para detener el servidor cuando termines" -ForegroundColor Yellow
        Write-Host ""
        
        Start-Process "http://localhost:5000"
        
        & "C:\Program Files\IIS Express\iisexpress.exe" /path:$proyectoPath /port:5000
    } else {
        Write-Host "Error: No se encontró la carpeta VentasWeb" -ForegroundColor Red
        Write-Host "Ejecuta este script desde la raíz del proyecto" -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "Usa cualquiera de las opciones arriba para iniciar el sistema" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Documentación disponible:" -ForegroundColor Cyan
    Write-Host "  - SISTEMA_REPORTES_COMPLETADO.md" -ForegroundColor White
    Write-Host "  - GUIA_PRUEBAS_REPORTES.md" -ForegroundColor White
    Write-Host ""
}
