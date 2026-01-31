# Plan de Pruebas - Módulo Inventario Inicial
**Fecha:** 31 de enero de 2026  
**Estado:** Listo para pruebas funcionales  
**Compilación:** ✅ Exitosa (0 errores)

---

## 📋 PRUEBAS OBLIGATORIAS

### FASE 1: Validaciones Servidor (Backend)

#### ✅ Prueba 1.1: Límite de productos en CSV
**Objetivo:** Verificar que rechaza archivos CSV con más de 5000 productos

**Pasos:**
1. Crear archivo CSV con 5001 filas (5000 productos + encabezado)
2. Ir a Inventario → Inventario Inicial
3. Click en "Importar CSV"
4. Seleccionar archivo creado
5. Click "Importar"

**Resultado esperado:**
- ❌ Rechaza importación
- 📝 Mensaje: "El archivo CSV tiene demasiadas filas. Máximo permitido: 5000 productos. El archivo tiene: 5001 filas."

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.2: Validación columnas requeridas en CSV
**Objetivo:** Verificar que rechaza CSV sin columnas obligatorias

**Pasos:**
1. Crear archivo CSV SIN la columna "cantidad":
   ```csv
   codigo,producto,costo,precio
   001,Producto 1,10.00,15.00
   ```
2. Importar el archivo

**Resultado esperado:**
- ❌ Rechaza importación
- 📝 Mensaje: "Formato CSV inválido. Faltan columnas requeridas: cantidad. Descarga la plantilla para ver el formato correcto."

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.3: Validación productos duplicados
**Objetivo:** Verificar que no permite agregar el mismo producto 2 veces en la misma carga

**Pasos:**
1. Nueva Carga
2. Buscar producto "Coca Cola"
3. Agregar con cantidad 10
4. Buscar nuevamente "Coca Cola"
5. Intentar agregar nuevamente

**Resultado esperado:**
- ❌ Rechaza segundo intento
- 📝 Alerta temporal: "Este producto ya está agregado en la carga actual. Edita la cantidad existente o elimínalo antes de agregarlo nuevamente."

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.4: Validación cantidad > 0
**Objetivo:** Verificar que rechaza cantidad = 0 o negativa

**Pasos:**
1. Nueva Carga
2. Buscar un producto
3. Ingresar cantidad: 0
4. Intentar agregar

**Resultado esperado:**
- ❌ Rechaza en cliente
- 📝 Alerta: "La cantidad debe ser mayor a 0"
- Focus en campo cantidad

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.5: Validación precios negativos
**Objetivo:** Verificar que rechaza costo o precio negativo

**Pasos:**
1. Nueva Carga
2. Buscar un producto
3. Ingresar costo: -10
4. Intentar agregar

**Resultado esperado:**
- ❌ Rechaza en cliente
- 📝 Alerta: "El costo no puede ser negativo"
- Focus en campo costo

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.6: Validación tamaño archivo (10 MB)
**Objetivo:** Verificar rechazo de archivos > 10 MB

**Pasos:**
1. Crear archivo CSV > 10 MB (o usar archivo grande existente)
2. Intentar importar

**Resultado esperado cliente:**
- ❌ Bloquea submit antes de enviar
- 📝 Alerta: "El archivo es demasiado grande. Máximo: 10 MB. Tamaño: XX MB"

**Resultado esperado servidor (si pasa cliente):**
- ❌ Rechaza en servidor
- 📝 Mensaje: "El archivo es demasiado grande. Máximo permitido: 10 MB. Tamaño del archivo: XX MB"

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 1.7: Validación extensión archivo
**Objetivo:** Verificar que solo acepta .csv

**Pasos:**
1. Intentar importar archivo .txt o .xlsx
2. Observar validación cliente

**Resultado esperado cliente:**
- ❌ Bloquea submit
- 📝 Alerta: "Solo se permiten archivos CSV (.csv). Archivo seleccionado: .txt"

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

### FASE 2: Optimizaciones y UX

#### ✅ Prueba 2.1: Debounce en búsqueda (300ms)
**Objetivo:** Verificar que no hace request por cada tecla

**Pasos:**
1. Nueva Carga
2. Abrir DevTools → Network
3. Escribir rápidamente "coca cola" en búsqueda
4. Observar requests en Network

**Resultado esperado:**
- ✅ Solo 1 request después de 300ms de dejar de escribir
- ✅ NO múltiples requests por cada letra

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 2.2: Indicadores de carga (spinners)
**Objetivo:** Verificar feedback visual en operaciones

**Pasos:**
1. Agregar producto
2. Observar botón "Agregar"
3. Eliminar producto
4. Observar botón eliminar
5. Importar CSV
6. Observar botón "Importar"

**Resultado esperado:**
- ✅ Botón cambia a "🔄 Agregando..."
- ✅ Botón deshabilitado durante operación
- ✅ Botón se restaura después

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 2.3: Alertas temporales (5 segundos)
**Objetivo:** Verificar auto-cierre de alertas

**Pasos:**
1. Generar un error (ej: cantidad = 0)
2. Observar alerta roja en esquina superior derecha
3. Esperar sin cerrar manualmente

**Resultado esperado:**
- ✅ Alerta aparece fixed top-right
- ✅ Se cierra automáticamente después de 5 segundos
- ✅ Animación fadeOut

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 2.4: Mostrar info archivo seleccionado
**Objetivo:** Verificar que muestra nombre y tamaño del archivo

**Pasos:**
1. Ir a Inventario Inicial (Index)
2. Click en "Seleccionar archivo"
3. Elegir un CSV
4. Observar debajo del input

**Resultado esperado:**
- ✅ Muestra: "Archivo: nombre.csv (XX KB)"
- ✅ Color gris (text-muted)

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 2.5: Auto-reload si tabla vacía
**Objetivo:** Verificar recarga automática al eliminar último producto

**Pasos:**
1. Nueva Carga con 1 solo producto
2. Eliminar ese producto
3. Observar comportamiento

**Resultado esperado:**
- ✅ Página se recarga automáticamente
- ✅ Vuelve a Index (no queda en vista vacía)

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

### FASE 3: Flujo Completo End-to-End

#### ✅ Prueba 3.1: Carga Manual Completa
**Objetivo:** Completar flujo manual exitoso

**Pasos:**
1. Login como Admin/Gerente
2. Inventario → Inventario Inicial
3. Click "Nueva Carga"
4. Agregar 3 productos diferentes:
   - Producto A: cantidad 10, costo 5, precio 8
   - Producto B: cantidad 20, costo 3, precio 5
   - Producto C: cantidad 15, costo 10, precio 15
5. Verificar totales calculados
6. Click "Finalizar Carga"
7. Confirmar
8. Verificar que aparece en historial
9. Revisar BD: tabla InventarioInicial_Lotes

**Resultado esperado:**
- ✅ Totales correctos: 45 unidades, costo total, precio total
- ✅ Lote creado con Activo = 0 (finalizado)
- ✅ Aparece en historial con fecha y usuario
- ✅ Log de auditoría generado

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 3.2: Importación CSV Completa
**Objetivo:** Completar flujo de importación exitoso

**Pasos:**
1. Descargar plantilla CSV
2. Editar plantilla:
   - Agregar 10 productos con datos válidos
   - Guardar como UTF-8
3. Click "Importar CSV"
4. Seleccionar archivo editado
5. Observar mensaje "Archivo: plantilla.csv (XX KB)"
6. Click "Importar"
7. Verificar redirección a vista de carga
8. Verificar que los 10 productos están listados
9. Verificar totales
10. Finalizar carga
11. Verificar historial

**Resultado esperado:**
- ✅ Importación exitosa con mensaje verde
- ✅ Todos los productos visibles en tabla
- ✅ Totales correctos
- ✅ Finalización exitosa
- ✅ Aparece en historial

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 3.3: Cancelar Carga
**Objetivo:** Verificar funcionalidad de cancelación

**Pasos:**
1. Nueva Carga
2. Agregar 2-3 productos
3. Click en botón "Cancelar Carga" (rojo)
4. Confirmar en modal
5. Verificar redirección a Index
6. Verificar que NO aparece en historial
7. Revisar BD: lote debe estar eliminado

**Resultado esperado:**
- ✅ Modal de confirmación aparece
- ✅ Lote eliminado de BD
- ✅ NO aparece en historial
- ✅ Mensaje de cancelación exitosa

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 3.4: Editar cantidad producto agregado
**Objetivo:** Verificar edición inline de cantidad

**Pasos:**
1. Nueva Carga
2. Agregar producto con cantidad 10
3. Cambiar valor en input de cantidad a 25
4. Tab o blur del input
5. Verificar que totales se recalculan

**Resultado esperado:**
- ✅ Cantidad actualizada
- ✅ Subtotal recalculado (25 * costo)
- ✅ Total general actualizado

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

### FASE 4: Seguridad

#### ✅ Prueba 4.1: Acceso sin login
**Objetivo:** Verificar protección de rutas

**Pasos:**
1. Cerrar sesión
2. Intentar acceder directamente: http://localhost:64927/InventarioInicial
3. Observar redirección

**Resultado esperado:**
- ✅ Redirige a login
- ✅ Error 401 o redirección automática

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 4.2: Acceso con rol no autorizado
**Objetivo:** Verificar validación de roles

**Pasos:**
1. Login con usuario rol Empleado (RolID != 1 y != 2)
2. Intentar acceder a /InventarioInicial

**Resultado esperado:**
- ❌ Acceso denegado
- 📝 Mensaje: "No tienes permisos para acceder a este módulo. Solo administradores y gerentes."

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 4.3: Anti-CSRF Token
**Objetivo:** Verificar protección contra CSRF

**Pasos:**
1. Abrir DevTools → Network
2. Realizar cualquier POST (agregar producto, importar CSV)
3. Inspeccionar request
4. Buscar __RequestVerificationToken

**Resultado esperado:**
- ✅ Token presente en todos los POST
- ✅ [ValidateAntiForgeryToken] funcional

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

### FASE 5: Casos Edge y Errores

#### ✅ Prueba 5.1: CSV vacío
**Objetivo:** Manejar CSV solo con encabezado

**Pasos:**
1. Crear CSV solo con línea de encabezado (sin productos)
2. Importar

**Resultado esperado:**
- ⚠️ Warning: "El archivo CSV está vacío o no contiene productos válidos"
- ✅ No crea lote vacío

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 5.2: Caracteres especiales en nombres
**Objetivo:** Verificar manejo de UTF-8

**Pasos:**
1. Agregar producto con nombre: "Café Frappé 250ml ñoño"
2. Verificar que se guarda correctamente
3. Finalizar carga
4. Verificar en historial que se muestra correctamente

**Resultado esperado:**
- ✅ Nombre completo visible sin caracteres corruptos
- ✅ Acentos y ñ correctos

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 5.3: Números decimales en cantidades
**Objetivo:** Verificar que rechaza cantidades fraccionarias

**Pasos:**
1. Intentar agregar producto con cantidad: 10.5
2. Observar comportamiento

**Resultado esperado:**
- ✅ Input type="number" debe validar enteros
- ✅ Si acepta, redondea correctamente

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 5.4: Intentar finalizar carga vacía
**Objetivo:** Verificar que no permite finalizar sin productos

**Pasos:**
1. Nueva Carga (sin agregar productos)
2. Intentar click en "Finalizar Carga"

**Resultado esperado:**
- ✅ Botón "Finalizar Carga" deshabilitado o no visible
- ✅ Mensaje: "Agrega al menos un producto antes de finalizar"

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

#### ✅ Prueba 5.5: Producto no encontrado en búsqueda
**Objetivo:** Manejar búsquedas sin resultados

**Pasos:**
1. Nueva Carga
2. Buscar: "XYZABCDEFG123" (producto que no existe)
3. Observar resultado

**Resultado esperado:**
- ℹ️ Mensaje: "No se encontraron productos"
- ✅ Lista vacía sin errores

**Estado:** [ ] Pendiente | [ ] Pasó | [ ] Falló

---

## 📊 RESUMEN DE PRUEBAS

**Total de pruebas:** 23

**Resultados:**
- ✅ Pasaron: ___
- ❌ Fallaron: ___
- ⏳ Pendientes: ___

**Bloqueadores encontrados:**
_Ninguno esperado - todas las validaciones están implementadas_

---

## 🚀 SIGUIENTE PASO

Una vez completadas todas las pruebas:

1. **Backup de BD:**
   ```sql
   BACKUP DATABASE [NombreBaseDatos]
   TO DISK = 'C:\Backups\PreProduccion_InventarioInicial_20260131.bak'
   WITH FORMAT, INIT;
   ```

2. **Documentar resultados de pruebas**

3. **Capacitar usuarios finales**

4. **Despliegue a producción**

---

## 📝 NOTAS ADICIONALES

- Usuario que ejecuta: _____________
- Fecha de pruebas: _____________
- Versión probada: v1.0 (31/01/2026)
- Base de datos: _____________
- Sucursal de prueba: _____________

---

**Generado automáticamente** | Módulo Inventario Inicial
