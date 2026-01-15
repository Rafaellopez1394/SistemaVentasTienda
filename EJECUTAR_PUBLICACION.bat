@echo off
echo ========================================
echo   PUBLICANDO SISTEMA DE VENTAS EN IIS
echo ========================================
echo.
echo Ejecutando con permisos de administrador...
echo.

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process PowerShell -ArgumentList '-NoExit', '-ExecutionPolicy Bypass -File ""%~dp0PublicarIIS.ps1""' -Verb RunAs}"
