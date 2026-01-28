# REPORTE DE UTILIDAD DIARIA - IMPLEMENTACIÓN COMPLETADA

## Resumen Ejecutivo

Se ha implementado un sistema completo de reportes de utilidad diaria con la siguiente arquitectura:

1. **SQL Server**: Stored Procedure `sp_ReporteUtilidadDiaria` con 9 conjuntos de resultados
2. **Modelos C#**: Clases para mapeo de datos (ReporteUtilidadDiaria + 7 clases anidadas)
3. **Capa de Datos**: CD_ReporteUtilidadDiaria para lectura secuencial de múltiples ResultSets
4. **Controlador**: ReporteController con acciones para Preview y Exportación a Excel
5. **Vista**: UtilidadDiaria.cshtml con interfaz interactiva y selector de fechas

## Archivos Creados/Modificados

### Nuevos Archivos Creados:

#### 1. SQL Server Procedure
- **Ruta**: `Utilidad/SQL Server/050_REPORTE_UTILIDAD_DIARIA.sql`
- **Descripción**: Stored procedure que genera 9 conjuntos de resultados:
  1. Resumen de ventas por forma de pago
  2. Costo de mercancía vendida (contado)
  3. Costo de mercancía vendida (crédito)
  4. Resumen de ventas y utilidades
  5. Costos totales y utilidades
  6. Recupero de créditos
  7. Costos de créditos recuperados
  8. Inventario inicial del día
  9. Entradas de compra y detalle de productos
- **Parámetros**: @Fecha (DATE), @SucursalID (INT)
- **Lógica de Cálculos**:
  - Costo: SUM(LotesProducto.PrecioCompra * cantidad)
  - Utilidad: SUM(ventas) - SUM(costos)
  - Recupero: SUM(PagosClientes.MontoPago) para el día
  - Agrupación por producto y forma de pago

#### 2. Modelos C#
- **Ruta**: `CapaModelo/ReporteUtilidadDiaria.cs`
- **Clases**: 
  - `ReporteUtilidadDiaria` (clase principal)
  - `ResumenVentas` (ventas por forma de pago)
  - `CostoDetalle` (breakdown de costos)
  - `UtilityDetalle` (análisis de utilidad)
  - `RecuperacionDetalle` (recupero de créditos)
  - `InventarioDetalle` (estado de inventario)
  - `EntradaDetalle` (entradas de compra)
  - `DetalleVentaProducto` (ventas por producto)
- **Propiedades Principales**:
  - Fecha, SucursalID
  - TotalVentasContado, TotalVentasCredito
  - CostosCompra, UtilidadDiaria
  - RecuperoCreditosTotal
  - Colecciones para cada sección del reporte

#### 3. Capa de Datos
- **Ruta**: `CapaDatos/CD_ReporteUtilidadDiaria.cs`
- **Clase Principal**: `CD_ReporteUtilidadDiaria`
- **Método Clave**: `ObtenerReporteDiario(DateTime fecha, int sucursalID)`
- **Técnica**: CommandBehavior.SequentialAccess para lectura de múltiples ResultSets
- **Mapeo**: Cada ResultSet se mapea automáticamente a la colección correspondiente
- **Uso de Singleton**: Patrón Instancia para acceso desde controladores

#### 4. Vista Razor
- **Ruta**: `VentasWeb/Views/Reporte/UtilidadDiaria.cshtml`
- **Componentes**:
  - Selector de fecha (HTML5 Date Input)
  - Botones de Preview y Descarga
  - Secciones de visualización:
    - Resumen de ventas (tabla con tickets, unidades, totales)
    - Análisis de costos y utilidad (paneles destacados)
    - Recupero de créditos (alerta informativa)
    - Top 20 productos (tabla detallada)
  - Indicadores visuales con colores y formatos
  - Funciones JavaScript para AJAX y formato de datos
  - Almacenamiento automático de fecha actual

### Archivos Modificados:

#### 1. ReporteController.cs
- **Ruta**: `VentasWeb/Controllers/ReporteController.cs`
- **Cambios**:
  - Agregado `using OfficeOpenXml;` (línea 10)
  - Agregada acción `UtilidadDiaria()` para renderizar la vista (línea ~32)
  - Agregada acción `ObtenerPreviewUtilidadDiaria()` para AJAX (JSON)
    - Parámetro: fecha (opcional, default: hoy)
    - Retorna: JSON con preview estructurado de datos
  - Agregada acción `ExportarUtilidadDiaria()` para descarga Excel
    - Usa EPPlus (ya instalado en proyecto)
    - Genera Excel formateado con 4 secciones
    - Aplica estilos: encabezados azul oscuro, datos con formato moneda
    - Retorna: Archivo XLSX descargable
  - Ambas acciones usan Session["SucursalActiva"] para contexto de sucursal

## Flujo de Datos

```
Usuario solicita /Reporte/UtilidadDiaria
    ↓
Vista se carga con selector de fecha (default: hoy)
    ↓
Usuario hace clic en "Ver Preview"
    ↓
AJAX → ReporteController.ObtenerPreviewUtilidadDiaria(fecha)
    ↓
CD_ReporteUtilidadDiaria.ObtenerReporteDiario()
    ↓
SQL: EXEC sp_ReporteUtilidadDiaria @Fecha, @SucursalID
    ↓
Lectura secuencial de 9 ResultSets
    ↓
Mapeo a modelo ReporteUtilidadDiaria
    ↓
Conversión a JSON anónimo (preview)
    ↓
Vista actualiza tablas y paneles con datos formateados
    ↓
Usuario hace clic en "Descargar Excel"
    ↓
POST → ReporteController.ExportarUtilidadDiaria(fecha)
    ↓
CD_ReporteUtilidadDiaria.ObtenerReporteDiario()
    ↓
EPPlus: crear Excel con 4 secciones y estilos
    ↓
Retornar archivo XLSX para descarga
```

## Características Implementadas

### En el SQL Procedure:
- ✅ Cálculo de ventas por forma de pago (CONTADO vs CRÉDITO)
- ✅ Cálculo de costo de mercancía (basado en LotesProducto.PrecioCompra)
- ✅ Cálculo automático de utilidad (Ventas - Costos)
- ✅ Tracking de recupero de créditos (PagosClientes)
- ✅ Inventario inicial del día
- ✅ Detalle de compras/entradas
- ✅ Análisis por producto con totales
- ✅ Filtros por fecha y sucursal
- ✅ 9 ResultSets para máxima flexibilidad

### En la Vista:
- ✅ Selector de fecha con valor por defecto (hoy)
- ✅ Botón Preview (carga datos sin Excel)
- ✅ Botón Descargar Excel (exporta completo)
- ✅ Indicador de carga (spinner animado)
- ✅ Alertas de éxito/error
- ✅ Formato moneda en todas las cifras ($X,XXX.XX)
- ✅ Formato porcentaje en ratios
- ✅ Tablas responsive
- ✅ Paneles destacados para KPIs principales
- ✅ Agrupamiento visual por secciones

### En el Excel:
- ✅ Encabezado con título y fecha
- ✅ Tabla resumen de ventas (forma de pago, tickets, unidades, % del total)
- ✅ Sección de costos y utilidad (monto, porcentaje)
- ✅ Sección de recupero de créditos
- ✅ Top 20 productos (contado, crédito, ventas, costo, utilidad)
- ✅ Colores de sección (azul oscuro #003366)
- ✅ Formato moneda ($#,##0.00)
- ✅ Anchos de columna optimizados
- ✅ Estilos: Bold, colores de fondo, bordes

## Pasos de Implementación Pendientes

### CRÍTICO - Antes de probar:
1. **Crear Stored Procedure en BD**:
   ```sql
   -- Ejecutar el contenido completo de: 050_REPORTE_UTILIDAD_DIARIA.sql
   -- en la base de datos DB_TIENDA
   ```

2. **Compilar el proyecto**:
   ```cmd
   cd c:\Users\Rafael Lopez\Documents\SistemaVentasTienda
   msbuild VentasWeb.sln /t:Rebuild /p:Configuration=Debug
   ```

3. **Verificar en App_Start**:
   - Si hay inicialización de permiso EPPlus, asegurar que esté configurado correctamente

### Pruebas Recomendadas:
1. Navegar a `/Reporte/UtilidadDiaria`
2. Ver página de inicio correctamente
3. Hacer clic en "Ver Preview" con fecha actual
4. Verificar que aparecen datos en las secciones
5. Hacer clic en "Descargar Excel"
6. Abrir archivo descargado y verificar formatos y datos

## Notas Técnicas Importantes

### Sobre EPPlus vs ClosedXML:
- El proyecto ya tenía EPPlus 7.0.0 instalado
- Se utilizó EPPlus en lugar de ClosedXML para evitar dependencias adicionales
- Ambas librerías ofrecen funcionalidad similar de formateo

### Sobre la Lectura de ResultSets:
- Se usa `CommandBehavior.SequentialAccess` para leer múltiples ResultSets eficientemente
- Cada ResultSet se lee en orden (1 a 9)
- Los datos se mapean a colecciones correspon dientes del modelo

### Sobre las Transacciones de Crédito:
- Los PagosClientes se incluyen en recupero del día
- El costo de esos créditos se calcula sobre la venta original

### Sobre los Precios:
- Se asume que PrecioVenta en DetalleVentasClientes es el precio NET (sin IVA)
- Se asume que PrecioCompra en LotesProducto es el costo unitario

## URLs de Acceso

- **View**: `http://localhost:[PORT]/Reporte/UtilidadDiaria`
- **API Preview**: `GET http://localhost:[PORT]/Reporte/ObtenerPreviewUtilidadDiaria?fecha=2025-01-24`
- **API Export**: `POST http://localhost:[PORT]/Reporte/ExportarUtilidadDiaria` (con fecha en body)

## Soporte y Mantenimiento

### Si hay errores al compilar:
1. Verificar que todas las referencias a System.Data.SqlClient existen
2. Verificar que OfficeOpenXml está importado correctamente
3. Ejecutar Clean Solution antes de Rebuild

### Si el reporte no muestra datos:
1. Verificar que el SP existe en BD con nombre correcto: `sp_ReporteUtilidadDiaria`
2. Verificar que hay datos en VentasClientes para la fecha seleccionada
3. Abrir SQL Server Management Studio y ejecutar manualmente:
   ```sql
   EXEC sp_ReporteUtilidadDiaria '2025-01-24', 1
   ```

### Si el Excel tiene formato incorrecto:
1. Verificar que EPPlus versión 7.0.0 está instalada
2. Revisar que System.Drawing está disponible en proyecto
3. Probar con fecha con muchas transacciones para validar funcionalidad

## Resumen de Cambios por Archivo

| Archivo | Cambio | Líneas |
|---------|--------|--------|
| 050_REPORTE_UTILIDAD_DIARIA.sql | Creado | ~400 |
| ReporteUtilidadDiaria.cs | Creado | ~150 |
| CD_ReporteUtilidadDiaria.cs | Creado | ~120 |
| UtilidadDiaria.cshtml | Creado | ~260 |
| ReporteController.cs | Modificado | +1 using, +1 view action, +2 API actions |

**Total de Nueva Funcionalidad**: ~900 líneas de código

---

## Estado Final

✅ **IMPLEMENTACIÓN COMPLETADA**

- Stored Procedure SQL Server: LISTO para ejecutar
- Modelos C# (7 clases): LISTO
- Capa de Datos: LISTO
- Controlador Web: LISTO
- Vista Razor: LISTO
- Excel Export (EPPlus): LISTO

**Próximo Paso**: Ejecutar el SQL script en la base de datos y compilar la solución.

