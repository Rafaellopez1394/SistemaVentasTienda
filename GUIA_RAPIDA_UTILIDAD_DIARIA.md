# GUÍA RÁPIDA: REPORTE DE UTILIDAD DIARIA

## Resumen de Lo Implementado

Se ha completado la implementación de un sistema de reportes de utilidad diaria con:

- **SQL Server**: Stored Procedure con 9 conjuntos de datos
- **C# Models**: 8 clases para mapeo de datos
- **Capa de Datos**: CD_ReporteUtilidadDiaria para acceso a SP
- **Controlador Web**: ReporteController con 3 acciones nuevas
- **Vista HTML**: UtilidadDiaria.cshtml con interfaz interactiva
- **Excel Export**: Exportación formateada con EPPlus

---

## PASO 1: Crear el Stored Procedure en SQL Server

### Acción:
1. Abre **SQL Server Management Studio**
2. Conecta a servidor con base de datos **DB_TIENDA**
3. Abre archivo: `Utilidad\SQL Server\050_REPORTE_UTILIDAD_DIARIA.sql`
4. Copia TODO el contenido del archivo
5. En SSMS, abre una nueva ventana de Query
6. Pega el contenido
7. Ejecuta (F5 o botón "Execute")
8. Verifica que aparece: "Command(s) completed successfully"

### Resultado esperado:
```
Procedure 'sp_ReporteUtilidadDiaria' created successfully
```

### Si hay error:
- Verifica que estás en BD correcta: `USE DB_TIENDA;`
- Verifica que no existe ya (si existe, abre primero: `DROP PROCEDURE sp_ReporteUtilidadDiaria;`)
- Verifica sintaxis SQL (quita comentarios de encabezado si tiene)

---

## PASO 2: Compilar el Proyecto Visual Studio

### Acción:
1. Abre **Visual Studio**
2. File → Open → Project/Solution
3. Navega a: `VentasWeb.sln`
4. Abre la solución
5. En Solution Explorer, haz clic derecho en "Solution 'VentasWeb'"
6. Selecciona "Rebuild Solution"
7. Espera a que compile (2-3 minutos aprox)
8. Verifica en Output window: "========== Rebuild All: X succeeded =========="

### Si hay errores:
- Haz doble clic en el error para ir a la línea
- Verifica que todas las referencias existen
- Si falta `using OfficeOpenXml;`, ya está agregado en ReporteController.cs
- Limpia el proyecto: Build → Clean Solution, luego Rebuild

---

## PASO 3: Ejecutar la Aplicación

### Acción:
1. En Visual Studio, presiona **F5** (Debug) o Ctrl+F5 (sin debug)
2. Espera a que compile y se abra el navegador
3. Se debe abrir en `http://localhost:PUERTO/` (ej: http://localhost:1234/)
4. Navega a: `/Reporte/UtilidadDiaria`
5. Completa URL: `http://localhost:PUERTO/Reporte/UtilidadDiaria`

### Resultado esperado:
- Carga página con encabezado "Reporte de Utilidad Diaria"
- Selector de fecha (con valor por defecto = hoy)
- Botones "Ver Preview" y "Descargar Excel"

---

## PASO 4: Probar el Preview (JSON)

### Acción:
1. En la página, la fecha viene preseleccionada (hoy)
2. Haz clic en botón azul **"Ver Preview"**
3. Espera a que cargue (spinner giratorio debe aparecer)
4. Deberían aparecer secciones debajo:

### Secciones esperadas:
- **RESUMEN DE VENTAS**: Tabla con forma de pago, tickets, unidades, totales
- **ANÁLISIS DE COSTOS Y UTILIDAD**: Dos paneles con montos destacados
- **RECUPERO DE CRÉDITOS**: Alerta con monto recuperado
- **TOP 20 PRODUCTOS**: Tabla con productos y detalles

### Si NO aparecen datos:
- Verifica que hay ventas en BD para la fecha seleccionada
- Abre browser console (F12) y mira si hay errores JavaScript
- Verifica que el SP existe: en SSMS, ejecuta `sp_help sp_ReporteUtilidadDiaria`

### Si SÍ aparecen datos:
- ¡Excelente! El SQL y C# funcionan correctamente
- Continúa con siguiente paso

---

## PASO 5: Probar Descarga de Excel

### Acción:
1. En la misma página de preview, haz clic en botón verde **"Descargar Excel"**
2. Debería iniciar descarga automática: `UtilidadDiaria_YYYYMMDD.xlsx`
3. Guarda el archivo en tu computadora
4. Abre con **Microsoft Excel**

### Resultado esperado:
El archivo debe contener una hoja llamada "Utilidad Diaria" con:

#### Sección 1: RESUMEN DE VENTAS
```
Forma de Pago | # Tickets | Unidades | Total Ventas | % del Total
CONTADO       | 15        | 120      | $1,234.56    | 65.32%
CREDITO       | 8         | 45       | $656.78      | 34.68%
TOTAL         | 23        | 165      | $1,891.34    | 100.00%
```

#### Sección 2: ANÁLISIS DE COSTOS Y UTILIDAD
```
Concepto                      | Monto
Total Ventas                  | $1,891.34
Costo de Mercancía Vendida    | $945.67
UTILIDAD DIARIA               | $945.67
% Utilidad                    | 50.00%
```

#### Sección 3: RECUPERO DE CRÉDITOS
```
Créditos Recuperados          | $250.00
```

#### Sección 4: TOP 20 PRODUCTOS (máximo 20 filas)
```
Producto | Contado | Crédito | Total Ventas | Costo Total | Utilidad
Leche    | 50      | 20      | $500.00      | $250.00     | $250.00
...
```

### Si el Excel tiene problemas:
- Verifica que está en formato correcto: debe ser .xlsx (Excel 2007+)
- Si no abre, puede ser que EPPlus generó error. En browser console, busca detalles

---

## PASO 6: Cambiar Fecha y Probar Múltiples Días

### Acción:
1. En la página, cambia la fecha en el selector (hacia atrás en el calendario)
2. Selecciona una fecha con muchas ventas (ej: un viernes)
3. Haz clic nuevamente en "Ver Preview"
4. Verifica que aparecen datos diferentes
5. Descarga Excel para esa fecha

### Información:
- El selector acepta cualquier fecha
- Si no hay datos para una fecha, verás tablas vacías pero sin errores
- El sistema automáticamente filtra por la fecha seleccionada

---

## PASO 7: Validación Final

### Checklist de Validación:

- [ ] SQL Server Procedure existe
- [ ] Proyecto compila sin errores
- [ ] Página /Reporte/UtilidadDiaria carga
- [ ] Preview muestra datos para una fecha
- [ ] Excel se descarga correctamente
- [ ] Excel abre en Excel sin errores
- [ ] Excel tiene 4 secciones con datos
- [ ] Formatos moneda se ven correctamente ($X,XXX.XX)
- [ ] Cambiar fecha y hacer preview nuevamente funciona
- [ ] Multiple descargas de Excel funcionan

---

## TROUBLESHOOTING

### Problema: "Error al cargar el reporte"
**Solución**: 
- Abre F12 (Developer Tools)
- Mira la pestana "Network"
- Haz clic en "Ver Preview"
- Busca una Request fallida con error 500
- Lee el detalle del error
- Verifica que SP existe en BD

### Problema: "Tabla vacía en preview"
**Solución**:
- Verifica que hay ventas en BD para esa fecha
- En SSMS, ejecuta:
  ```sql
  SELECT TOP 5 FechaVenta FROM VentasClientes ORDER BY FechaVenta DESC
  ```
- Usa una fecha que aparezca en esa query

### Problema: Excel no se descarga
**Solución**:
- Desactiva pop-up blocker del navegador
- En browser console (F12), mira errores
- Verifica que el botón dice "Descargar Excel" (no "Descargar")
- Recarga la página (F5) y reintentar

### Problema: Excel abre pero está vacío
**Solución**:
- Cierra el archivo
- Cambia extensión de .xlsx a .zip
- Extrae y mira que contiene carpeta xl/worksheets/
- Si está vacío, hay error en EPPlus

### Problema: "Sequence contains no matching element" en preview
**Solución**:
- El SP retorna 9 ResultSets pero CD no puede leerlos todos
- Verifica que SP fue creado correctamente
- En SSMS, ejecuta manualmente SP para verificar output

---

## INFORMACIÓN TÉCNICA

### URLs del Sistema
- **Vista**: `http://localhost:PORT/Reporte/UtilidadDiaria`
- **API Preview**: `GET /Reporte/ObtenerPreviewUtilidadDiaria?fecha=YYYY-MM-DD`
- **API Export**: `POST /Reporte/ExportarUtilidadDiaria` (con fecha en formulario)

### Archivos del Proyecto
- SQL: `Utilidad\SQL Server\050_REPORTE_UTILIDAD_DIARIA.sql`
- C# Modelos: `CapaModelo\ReporteUtilidadDiaria.cs`
- C# Datos: `CapaDatos\CD_ReporteUtilidadDiaria.cs`
- Controlador: `VentasWeb\Controllers\ReporteController.cs`
- Vista: `VentasWeb\Views\Reporte\UtilidadDiaria.cshtml`

### Dependencias
- SQL Server 2016+ (para DATEFROMPARTS, LAG/LEAD si se usa)
- .NET Framework 4.6+
- EPPlus 7.0.0 (ya en proyecto)
- jQuery 3.3.1+ (para AJAX)

### Tablas de BD Utilizadas
- VentasClientes
- DetalleVentasClientes
- LotesProducto
- CatFormasPago
- PagosClientes
- Productos
- Compras

---

## SOPORTE

Si después de seguir estos pasos algo no funciona:

1. **Verifica que compiló sin errores**: En Visual Studio, Output tab debe decir "========== Build successful =========="

2. **Verifica que SP existe**: En SSMS, ejecuta `SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_ReporteUtilidadDiaria'` (debe retornar 1 fila)

3. **Verifica que hay datos**: En SSMS, ejecuta `SELECT COUNT(*) FROM VentasClientes WHERE CAST(FechaVenta AS DATE) = GETDATE()` (debe ser > 0 si hay ventas hoy)

4. **Verifica URL correcta**: Asegúrate que estás en `http://localhost:PORT/Reporte/UtilidadDiaria` (no /ReporteController o similar)

5. **Limpia caché**: Presiona Ctrl+Shift+Delete en browser y borra caché

6. **Reinicia Visual Studio**: A veces hay problemas de compilación fantasma

---

**¡Sistema implementado y listo para usar!**

Última actualización: 2025-01-24
