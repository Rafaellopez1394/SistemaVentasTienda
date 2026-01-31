# ✅ CHECKLIST DE PRODUCCIÓN - Módulo Inventario Inicial con CSV

**Fecha:** 31 de Enero de 2026  
**Módulo:** Inventario Inicial con Importación/Exportación CSV  
**Versión:** 1.0

---

## 🔒 SEGURIDAD

### ✅ Autenticación
- [x] **`[Authorize]` habilitado** en InventarioInicialController
- [x] Validación de Session["Usuario"]
- [x] Validación de Session["SucursalID"]

### ⚠️ Recomendaciones Adicionales
- [ ] Validar permisos por rol (Administrador/Gerente puede acceder)
- [ ] Agregar logs de auditoría para acciones críticas:
  - Quién inició carga
  - Quién finalizó carga
  - Qué productos se importaron
- [ ] Limitar tamaño de archivo CSV (recomendado: 10 MB máximo)

---

## 🧪 PRUEBAS FUNCIONALES

### 1. Exportar Plantilla CSV
- [ ] Iniciar sesión en el sistema
- [ ] Ir a: **Inventario → Inventario Inicial**
- [ ] Click en **"Descargar Plantilla CSV"**
- [ ] **Verificar:** Archivo descarga correctamente
- [ ] **Verificar:** Nombre de archivo incluye timestamp
- [ ] **Verificar:** Abrir en Excel sin errores de encoding
- [ ] **Verificar:** Todos los productos del catálogo aparecen
- [ ] **Verificar:** Columnas correctas: ProductoID, CodigoInterno, NombreProducto, StockActual, CantidadNueva, CostoUnitario, PrecioVenta, Comentarios

### 2. Llenar Plantilla
- [ ] Abrir plantilla en Excel/LibreOffice
- [ ] Llenar al menos 3 productos con datos válidos:
  - CantidadNueva > 0
  - CostoUnitario > 0
  - PrecioVenta > 0
- [ ] Guardar como CSV (UTF-8)
- [ ] **Verificar:** Archivo se guarda correctamente

### 3. Importar CSV
- [ ] Click en botón **"Seleccionar Archivo"** en sección de importación
- [ ] Seleccionar archivo CSV lleno
- [ ] Agregar comentario opcional (ej: "Prueba inventario inicial")
- [ ] Click **"Importar"**
- [ ] **Verificar:** Mensaje de éxito: "Importación completada: X productos agregados"
- [ ] **Verificar:** Redirección a vista Cargar
- [ ] **Verificar:** Productos importados aparecen en tabla
- [ ] **Verificar:** Totales son correctos (productos, unidades, valor)

### 4. Finalizar Carga
- [ ] Click en **"Finalizar Carga"**
- [ ] **Verificar:** Confirmación de finalización exitosa
- [ ] **Verificar:** Carga aparece en historial como "Finalizada"
- [ ] **Verificar:** Ya no se puede editar esa carga

### 5. Verificar Inventario Actualizado

#### En Base de Datos:
```sql
-- Verificar lotes creados
SELECT TOP 10 * 
FROM LotesProducto 
WHERE Usuario = 'tu_usuario'
ORDER BY FechaEntrada DESC;

-- Verificar movimientos registrados
SELECT TOP 10 * 
FROM InventarioMovimientos 
WHERE TipoMovimiento = 'INVENTARIO_INICIAL'
ORDER BY Fecha DESC;

-- Verificar stock calculado
SELECT 
    p.ProductoID,
    p.NombreProducto,
    SUM(l.CantidadDisponible) AS StockActual
FROM Productos p
LEFT JOIN LotesProducto l ON p.ProductoID = l.ProductoID
WHERE p.ProductoID IN (1, 2, 3) -- IDs de productos importados
GROUP BY p.ProductoID, p.NombreProducto;
```

#### En la Aplicación:
- [ ] Ir a **Inventario → Productos**
- [ ] Buscar productos importados
- [ ] **Verificar:** Stock actualizado correctamente
- [ ] **Verificar:** Precio de venta actualizado
- [ ] Ir a **Inventario → Lotes**
- [ ] **Verificar:** Nuevos lotes aparecen con fecha reciente

---

## 🧪 PRUEBAS DE CASOS EDGE

### Validaciones de Archivo
- [ ] **Archivo vacío:** Debe mostrar error "El archivo CSV está vacío"
- [ ] **Archivo sin productos válidos:** Debe mostrar "No se pudo agregar ningún producto"
- [ ] **Archivo muy grande (>10 MB):** Debe manejar correctamente (o rechazar si implementaste límite)
- [ ] **Formato incorrecto (no CSV):** Debe rechazar archivo

### Validaciones de Datos
- [ ] **CantidadNueva = 0:** Debe omitir esa línea
- [ ] **CostoUnitario = 0:** Debe omitir esa línea
- [ ] **PrecioVenta = 0:** Debe omitir esa línea
- [ ] **ProductoID inexistente:** Debe reportar error para esa línea
- [ ] **Nombre con comillas:** Debe manejar correctamente (ej: "Coca-Cola \"Light\"")
- [ ] **Nombre con comas:** Debe manejar correctamente (ej: "Coca-Cola, 600ml")

### Flujo de Errores
- [ ] **Carga activa existente:** Debe redirigir a carga existente
- [ ] **Error de base de datos:** Debe mostrar mensaje de error amigable
- [ ] **Session expirada:** Debe redirigir a login

---

## 📊 PRUEBAS DE RENDIMIENTO

### Volumen de Datos
- [ ] **10 productos:** Debe procesar en < 2 segundos
- [ ] **50 productos:** Debe procesar en < 5 segundos
- [ ] **200 productos:** Debe procesar en < 20 segundos
- [ ] **500+ productos:** Debe procesar sin errores (puede tomar 1-2 minutos)

### Timeout
- [ ] Verificar que el timeout del SP está en 120 segundos
- [ ] Para inventarios muy grandes (>1000), considerar aumentar timeout

---

## 🔍 REVISIÓN DE CÓDIGO

### Compilación
- [x] **Compilación exitosa** (0 errores)
- [x] **Solo 2 warnings** (binding redirects - normales)

### Código Critical
- [x] **Validación de archivo null/vacío** implementada
- [x] **Parser CSV robusto** para comillas y comas
- [x] **Transacciones en SP** para rollback en caso de error
- [x] **Manejo de errores completo** con try-catch
- [x] **Encoding UTF-8 con BOM** para Excel

### Puntos de Mejora Futura
- [ ] Agregar log detallado de errores por línea
- [ ] Preview de datos antes de importar
- [ ] Progress bar visual durante importación
- [ ] Validación de duplicados antes de importar
- [ ] Opción de cancelar importación a mitad

---

## 🗄️ BASE DE DATOS

### Verificaciones Pre-Producción
```sql
-- 1. Verificar que existen las tablas
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('InventarioInicial', 'InventarioInicialDetalle');
-- Debe retornar: 2

-- 2. Verificar que existen los SPs
SELECT COUNT(*) FROM sys.procedures 
WHERE name LIKE 'SP_%InventarioInicial%';
-- Debe retornar: 5 (Iniciar, Agregar, Finalizar, Obtener, Eliminar)

-- 3. Verificar que existe la vista
SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS 
WHERE TABLE_NAME = 'VW_HistorialInventarioInicial';
-- Debe retornar: 1

-- 4. Verificar estructura de LotesProducto
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'LotesProducto';
-- Debe incluir: CantidadDisponible, PrecioCompra, PrecioVenta, SucursalID

-- 5. Verificar estructura de InventarioMovimientos
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'InventarioMovimientos';
-- Debe incluir: TipoMovimiento, Cantidad, CostoUnitario
```

### Backup Pre-Producción
```sql
-- CRÍTICO: Hacer backup ANTES de usar en producción
BACKUP DATABASE [NombreBaseDatos]
TO DISK = 'C:\Backups\PreProduccion_InventarioCSV_20260131.bak'
WITH FORMAT, INIT, NAME = 'Backup antes de Inventario CSV';
```

---

## 📝 DOCUMENTACIÓN

### Archivos de Documentación
- [x] **INSTRUCCIONES_PLANTILLA_CSV.md** - Guía completa para usuarios
- [x] **FUNCIONALIDAD_IMPORTACION_CSV.md** - Resumen técnico
- [x] **MODULO_INVENTARIO_INICIAL.md** - Manual técnico completo
- [ ] **Video tutorial** (opcional pero recomendado)
- [ ] **Capacitación al personal** antes de usar

### Documentación para IT
- [x] Estructura de tablas documentada
- [x] Stored Procedures documentados
- [x] Flujo de datos explicado
- [ ] Procedimiento de rollback en caso de error

---

## 🚀 DEPLOYMENT

### Pre-Deployment
- [x] **Código compilado** sin errores
- [x] **Tests básicos** realizados en desarrollo
- [ ] **Backup de base de datos** realizado
- [ ] **Plan de rollback** definido
- [ ] **Horario de deployment** definido (preferentemente fuera de horas pico)

### Durante Deployment
1. [ ] Detener IIS / App Pool
2. [ ] Hacer backup de archivos actuales
3. [ ] Copiar nuevos archivos:
   - VentasWeb\bin\VentasWeb.dll
   - VentasWeb\bin\VentasWeb.pdb
   - VentasWeb\bin\CapaDatos.dll
   - VentasWeb\bin\CapaModelo.dll
   - VentasWeb\Views\InventarioInicial\*.cshtml
   - VentasWeb\Controllers\InventarioInicialController.cs (ya compilado en DLL)
4. [ ] Verificar que existe script SQL en servidor
5. [ ] Ejecutar script SQL si no se ha ejecutado:
   ```
   CREAR_MODULO_INVENTARIO_INICIAL.sql
   ```
6. [ ] Reiniciar IIS / App Pool
7. [ ] Verificar que el sitio carga correctamente
8. [ ] Probar login
9. [ ] Probar acceso a módulo de Inventario Inicial

### Post-Deployment
- [ ] Realizar pruebas de humo:
  - Login funciona
  - Descarga de plantilla funciona
  - Importación de CSV funciona con 2-3 productos de prueba
- [ ] Verificar logs de IIS/Aplicación por errores
- [ ] Monitorear rendimiento primeras 24 horas
- [ ] Estar disponible para soporte inmediato

---

## 👥 CAPACITACIÓN

### Usuarios Finales
- [ ] **Demostración en vivo** del flujo completo
- [ ] **Entregar guía impresa** (INSTRUCCIONES_PLANTILLA_CSV.md)
- [ ] **Sesión de preguntas y respuestas**
- [ ] **Designar "super usuario"** que domine el proceso

### Personal de Soporte
- [ ] **Capacitación técnica** sobre la funcionalidad
- [ ] **Escenarios comunes de problemas** y soluciones
- [ ] **Acceso a documentación técnica**
- [ ] **Contacto de escalamiento** (tu número/email)

---

## 🆘 PLAN DE CONTINGENCIA

### Si algo falla en Producción:

#### Opción 1: Rollback Rápido
```
1. Detener IIS
2. Restaurar archivos desde backup
3. Reiniciar IIS
4. Verificar que sistema funciona sin nuevo módulo
5. Investigar problema en ambiente de desarrollo
```

#### Opción 2: Deshabilitación Temporal
```
1. Comentar opción de menú de Inventario Inicial
2. O agregar atributo obsoleto al controlador
3. Investigar y corregir
4. Re-deployment cuando esté listo
```

#### Opción 3: Rollback de Base de Datos (CRÍTICO)
```sql
-- Solo si se corrompieron datos
RESTORE DATABASE [NombreBaseDatos]
FROM DISK = 'C:\Backups\PreProduccion_InventarioCSV_20260131.bak'
WITH REPLACE;
```

---

## 📞 CONTACTOS DE EMERGENCIA

**Desarrollador:**
- Nombre: [Tu nombre]
- Teléfono: [Tu número]
- Email: [Tu email]
- Horario disponible: [Ej: 24/7 primera semana]

**Soporte Nivel 1:**
- [Nombre del encargado de IT]
- [Contacto]

**Usuario Experto:**
- [Nombre del gerente/encargado de inventario]
- [Contacto]

---

## ✅ CRITERIOS DE ACEPTACIÓN FINAL

Para considerar la implementación exitosa, TODOS deben estar ✅:

### Funcional
- [ ] Descargar plantilla CSV funciona
- [ ] Importar CSV funciona con al menos 10 productos reales
- [ ] Lotes se crean correctamente en base de datos
- [ ] Movimientos se registran correctamente
- [ ] Stock se actualiza y es visible en otros módulos
- [ ] Historial de cargas se muestra correctamente

### Técnico
- [ ] 0 errores de compilación
- [ ] 0 excepciones no manejadas durante pruebas
- [ ] Tiempos de respuesta aceptables (<30 segundos para 100 productos)
- [ ] Logs no muestran errores críticos

### Seguridad
- [ ] [Authorize] habilitado
- [ ] Solo usuarios autenticados pueden acceder
- [ ] No hay SQL injection posible (usamos stored procedures)
- [ ] No hay XSS posible (datos escapados en vistas)

### Documentación
- [ ] Usuario entiende cómo usar el módulo
- [ ] IT sabe cómo resolver problemas comunes
- [ ] Existe plan de rollback claro

---

## 🎉 GO/NO-GO DECISION

**Fecha de revisión:** _____________

**Checklist completado al:** _____%

**Decisión:** 
- [ ] ✅ **GO** - Listo para producción
- [ ] ⏸️ **HOLD** - Necesita correcciones menores (listar abajo)
- [ ] ❌ **NO-GO** - Problemas críticos (listar abajo)

**Notas:**
```
[Espacio para notas del revisor]
```

**Aprobado por:**
- Desarrollador: _________________ Fecha: _______
- QA/Testing: _________________ Fecha: _______
- IT Manager: _________________ Fecha: _______
- Usuario Final: _________________ Fecha: _______

---

## 📈 POST-IMPLEMENTACIÓN

### Primera Semana
- [ ] Monitorear logs diariamente
- [ ] Recopilar feedback de usuarios
- [ ] Medir tiempos de importación reales
- [ ] Documentar problemas encontrados

### Primer Mes
- [ ] Revisar uso del módulo
- [ ] Identificar mejoras necesarias
- [ ] Planear optimizaciones si es necesario
- [ ] Actualizar documentación basado en feedback

---

**Última actualización:** 31 de Enero de 2026
**Versión del Checklist:** 1.0
**Estado:** ✅ LISTO PARA REVISIÓN
