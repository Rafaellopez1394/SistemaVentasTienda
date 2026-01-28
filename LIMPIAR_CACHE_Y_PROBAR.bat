@echo off
echo ============================================
echo   LIMPIEZA DE CACHE Y PRUEBA DE TICKET
echo ============================================
echo.

echo 1. Deteniendo IIS...
iisreset /stop
timeout /t 2 /nobreak >nul

echo 2. Limpiando cache de IIS...
del /q /s "C:\Windows\Temp\*.*" 2>nul
del /q /s "%TEMP%\*.*" 2>nul

echo 3. Reiniciando IIS...
iisreset /start
timeout /t 2 /nobreak >nul

echo 4. Abriendo navegador en modo incognito...
start msedge.exe -inprivate "http://localhost:64927/VentaPOS"

echo.
echo ============================================
echo   INSTRUCCIONES:
echo ============================================
echo 1. El navegador se abrio en modo incognito
echo 2. Haz una venta de prueba
echo 3. Verifica el ticket impreso
echo.
echo Si el formato NO esta correcto, presiona
echo cualquier tecla y dame el resultado.
echo ============================================
pause
