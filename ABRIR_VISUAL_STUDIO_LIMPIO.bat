@echo off
echo ========================================
echo Limpiando proyecto y abriendo VS
echo ========================================
echo.

echo [1/5] Cerrando Visual Studio si esta abierto...
taskkill /F /IM devenv.exe 2>nul
timeout /t 2 /nobreak >nul

echo [2/5] Eliminando carpetas de compilacion...
rd /s /q "VentasWeb\bin" 2>nul
rd /s /q "VentasWeb\obj" 2>nul
rd /s /q "CapaDatos\bin" 2>nul
rd /s /q "CapaDatos\obj" 2>nul
rd /s /q "CapaModelo\bin" 2>nul
rd /s /q "CapaModelo\obj" 2>nul

echo [3/5] Eliminando cache de Visual Studio...
rd /s /q ".vs" 2>nul

echo [4/5] Validando Web.config...
powershell -Command "try { [xml]$x = Get-Content 'VentasWeb\Web.config'; Write-Host '  ✓ Web.config es valido' -ForegroundColor Green } catch { Write-Host '  ✗ ERROR en Web.config' -ForegroundColor Red }"

echo [5/5] Abriendo Visual Studio...
start "" "VentasWeb.sln"

echo.
echo ========================================
echo Proceso completado
echo ========================================
echo.
echo IMPORTANTE:
echo 1. Cuando abra Visual Studio, ve a Build ^> Clean Solution
echo 2. Luego Build ^> Rebuild Solution
echo 3. Finalmente presiona F5 para ejecutar
echo.
pause
