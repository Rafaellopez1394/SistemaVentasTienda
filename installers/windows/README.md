# Instalador de SistemaVentasTienda (Windows)

Este instalador crea el sitio web (IIS) y configura la base de datos en SQL Server con tablas y catálogos, además de cargar productos predefinidos con existencia 0.

## Prerrequisitos
- Windows 10/11 con privilegios de administrador.
- .NET Framework 4.6+ (se instala con el rol IIS ASP.NET 4.5 al ejecutar el script).
- SQL Server Express o una instancia de SQL Server disponible (por defecto `.\\SQLEXPRESS`).
- Inno Setup (para compilar el instalador `.exe`): https://jrsoftware.org/isinfo.php

## Contenido
- `SistemaVentasTienda.iss`: Script de Inno Setup para crear el instalador.
- `Configure-IIS.ps1`: Habilita IIS y crea el sitio en el puerto 8080.
- `Install-Database.ps1`: Ejecuta los scripts SQL de `Utilidad/SQL Server` y carga productos.

## Construir el instalador `.exe`
1. Instala Inno Setup y abre `installers/windows/SistemaVentasTienda.iss`.
2. Compila el proyecto para generar `SistemaVentasTienda_Installer.exe`.

## Pasos del instalador
- Copia la carpeta `VentasWeb` al directorio de instalación.
- Habilita IIS y crea el sitio `SistemaVentasTienda` en el puerto 8080.
- Actualiza la cadena de conexión en `VentasWeb/Web.config` para apuntar a `.\\SQLEXPRESS`.
- Ejecuta `Install-Database.ps1` contra la instancia indicada.

## Personalización
- Cambia la instancia de SQL con parámetros en el `Run` del `.iss`, o ejecuta manualmente:

```powershell
powershell -ExecutionPolicy Bypass -File "Install-Database.ps1" -ServerInstance ".\\SQLEXPRESS" -DatabaseName "DB_TIENDA" -AuthType "Windows"
```

- Para usar autenticación SQL:

```powershell
powershell -ExecutionPolicy Bypass -File "Install-Database.ps1" -ServerInstance ".\\SQLEXPRESS" -DatabaseName "DB_TIENDA" -AuthType "SQL" -SqlUser "sa" -SqlPassword "TuPassword"
```

## Notas
- El script de base de datos ejecuta todos los `.sql` en `Utilidad/SQL Server` en orden alfanumérico. Asegúrate de que estos scripts creen todas las tablas y procedimientos necesarios.
- El seeding de productos lee `productos_actuales.txt`. Si existe la tabla `Productos`, inserta ahí; si no, inserta en `PRODUCTO`. En ambos casos, la existencia queda en 0.
- Si necesitas instalar SQL Server Express automáticamente, puedes extender el `.iss` para descargar e instalar el paquete de SQL Server durante la instalación.
