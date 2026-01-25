# GUÍA RÁPIDA DE PRUEBAS - REPORTES AVANZADOS

## 🚀 INICIO RÁPIDO

### 1. Ejecutar el sitio web
```powershell
# Opción A: IIS Express desde Visual Studio
# Abrir VentasWeb.sln y presionar F5

# Opción B: IIS Local
# Navegar a: http://localhost/VentasWeb
```

### 2. Acceder a los reportes
Navegar directamente a: **http://localhost/VentasWeb/ReporteAvanzado/Index**

---

## ✅ CHECKLIST DE PRUEBAS

### PRUEBA 1: Dashboard Principal ⏱️ 2 min
**URL:** `/ReporteAvanzado/Index`

**Verificar:**
- [ ] KPIs cargan automáticamente (Ventas Hoy, Utilidad Mes, Bajo Stock, Morosos)
- [ ] Top 5 Productos aparece con datos o mensaje "No hay datos"
- [ ] Los 4 cards de reportes están visibles
- [ ] Los botones "Ver Reporte" funcionan

**Resultado esperado:**
- Dashboard con métricas en tiempo real
- Cards con colores (verde/azul/amarillo/rojo)
- Top 5 con tabla ordenada

---

### PRUEBA 2: Reporte de Utilidad por Producto ⏱️ 5 min
**URL:** `/ReporteAvanzado/UtilidadProductos`

**Pasos:**
1. Fecha Inicio: Hace 1 mes
2. Fecha Fin: Hoy
3. Click en "Generar Reporte"

**Verificar:**
- [ ] Tabla se llena con productos
- [ ] Columnas visibles: Código, Producto, Categoría, Cant Vendida, Venta, Costo, Utilidad, Margen %, Rentabilidad, Recomendación
- [ ] Cards de resumen actualizados:
  - Utilidad Total
  - Margen Promedio
  - Total Productos
  - Con Pérdidas
- [ ] Colores en columnas:
  - Utilidad: Verde (positiva) / Rojo (negativa)
  - Margen %: Verde (≥30%) / Amarillo (≥15%) / Rojo (<15%)
  - Rentabilidad: Badge coloreado (ALTA/MEDIA/BAJA/PÉRDIDA)

**Caso de prueba específico:**
- Buscar un producto que se haya vendido en el período
- Verificar que los cálculos sean coherentes:
  ```
  Utilidad = Venta Total - Costo Vendido
  Margen % = (Utilidad / Venta Total) × 100
  ```

**Resultado esperado:**
- Reporte completo con análisis de rentabilidad
- Identificación visual de productos rentables vs no rentables
- Recomendaciones automáticas

---

### PRUEBA 3: Estado de Resultados (P&L) ⏱️ 3 min
**URL:** `/ReporteAvanzado/EstadoResultados`

**Pasos:**
1. Fecha Inicio: Primero del mes pasado
2. Fecha Fin: Último día del mes pasado
3. Click en "Generar Estado de Resultados"

**Verificar:**
- [ ] Tabla contable aparece con todas las secciones
- [ ] Cálculos correctos:
  - Ingresos Netos = Ventas - Devoluciones
  - Utilidad Bruta = Ingresos - Costo de Ventas
  - Gastos Totales = Nómina + Operativos
  - Utilidad Neta = Utilidad Bruta - Gastos
- [ ] Margen Bruto % y Margen Neto % calculados
- [ ] Conclusión visible con color:
  - Verde: "✅ NEGOCIO RENTABLE"
  - Rojo: "⚠️ PÉRDIDAS"
- [ ] Recomendaciones basadas en margen

**Resultado esperado:**
- P&L profesional estilo contable
- Conclusión automática sobre viabilidad
- Recomendaciones accionables

---

### PRUEBA 4: Recuperación de Crédito ⏱️ 5 min
**URL:** `/ReporteAvanzado/RecuperacionCredito`

**Pasos:**
1. Fecha Inicio: Hace 30 días
2. Fecha Fin: Hoy
3. Click en "Generar Reporte"

**Verificar:**
- [ ] Tabla con días listados
- [ ] Columnas completas:
  - Fecha, Clientes, Créditos Otorgados, Cobros Realizados
  - Saldo Inicial, Saldo Final, % Recuperación
  - Cartera Vigente, Cartera Vencida, % Vencido
- [ ] Cards de resumen:
  - Total Créditos Otorgados (suma del período)
  - Total Cobros Realizados (suma del período)
  - % Recuperación Global
  - Cartera Vencida Actual
- [ ] Footer con totales
- [ ] Colores en % Recuperación:
  - Verde: ≥80%
  - Amarillo: 50-80%
  - Rojo: <50%
- [ ] **Gráfica de líneas** renderizada con:
  - Línea naranja: Créditos Otorgados
  - Línea verde: Cobros Realizados
  - Línea roja: Saldo Acumulado

**Caso de prueba:**
- Verificar que saldo final de un día = saldo inicial del día siguiente
- Confirmar que % recuperación = (Cobros / Créditos) × 100

**Resultado esperado:**
- Seguimiento completo día por día
- Identificación de días con baja recuperación
- Visualización gráfica de tendencias

---

### PRUEBA 5: Cartera de Clientes ⏱️ 3 min
**URL:** `/ReporteAvanzado/CarteraClientes`

**Pasos:**
1. Fecha de Corte: Hoy
2. Click en "Consultar Cartera"

**Verificar:**
- [ ] Solo aparecen clientes con saldo pendiente > 0
- [ ] Columnas completas:
  - Cliente, RFC, Tipo Crédito
  - Total Ventas, Total Pagos, Saldo Pendiente
  - Vigente, Vencido 30, Vencido 60, Vencido 90+
  - Días Vencido, Estado
- [ ] Cards de resumen:
  - Cartera Total (suma de todos los saldos)
  - Al Corriente (≤30 días)
  - Vencido (>30 días)
  - Morosos (cantidad de clientes >60 días)
- [ ] Badges de estado con colores:
  - Verde: AL CORRIENTE (≤30 días)
  - Amarillo: VENCIDO (31-60 días)
  - Rojo: MOROSO (>60 días)
- [ ] Ordenado por saldo pendiente descendente

**Caso de prueba:**
- Verificar un cliente:
  ```
  Saldo = Total Ventas - Total Pagos
  ```
- Confirmar antigüedad según fecha de última venta

**Resultado esperado:**
- Lista completa de clientes con saldo
- Identificación clara de morosos
- Priorización por monto adeudado

---

## 🐛 SOLUCIÓN DE PROBLEMAS

### Error: "No se puede conectar al servidor"
**Solución:**
1. Verificar cadena de conexión en Web.config
2. Confirmar que SQL Server está corriendo
3. Probar conexión con SQL Management Studio

### Error: "Stored Procedure no existe"
**Solución:**
1. Abrir SQL Management Studio
2. Ejecutar script: `CREAR_SP_REPORTES_AVANZADOS_CORREGIDO.sql`
3. Verificar:
   ```sql
   SELECT name FROM sys.procedures 
   WHERE name LIKE '%Reporte%'
   ```

### Error: "Datos vacíos" o "No hay registros"
**Causas posibles:**
1. No hay ventas en el rango de fechas seleccionado
2. Filtro de sucursal no coincide
3. No hay ventas a crédito (para reporte de crédito)

**Solución:**
- Ampliar rango de fechas
- Verificar que hay transacciones en la BD
- Usar fechas con actividad conocida

### Error: "La tabla no se carga"
**Solución:**
1. Abrir consola del navegador (F12)
2. Ver errores de JavaScript
3. Verificar que DataTables está cargado
4. Confirmar que AJAX retorna datos válidos

### Gráfica no aparece
**Solución:**
1. Verificar que Chart.js está cargado
2. Ver consola para errores
3. Confirmar que hay datos para graficar
4. Revisar que div con id="chartRecuperacion" existe

---

## 📊 DATOS DE PRUEBA RECOMENDADOS

Para pruebas completas, asegurar que la BD tenga:

### Ventas
- Al menos 10 ventas en el último mes
- Mix de CONTADO y CREDITO
- Varios productos diferentes
- Precios de compra en VentasDetalleClientes.PrecioCompra

### Compras
- Compras recientes para calcular costos
- LotesProducto con precios de compra

### Pagos
- Pagos de clientes para recuperación
- Diferentes fechas para seguimiento

### Gastos
- Gastos de nómina (categorías con palabra "nomina", "sueldo", "salario")
- Gastos operativos (categorías: "renta", "luz", "agua", etc.)

### Clientes con Crédito
- Al menos 5 clientes con tipo de crédito asignado
- Ventas a crédito con fechas variadas
- Algunos pagos parciales para calcular saldos

---

## ✅ VALIDACIÓN FINAL

Al terminar todas las pruebas, deberías poder responder:

### 1. ¿Qué producto es más rentable?
**Respuesta esperada:** "Producto X tiene margen de 35% y clasificación ALTA, es el más rentable"

### 2. ¿Es rentable mi negocio este mes?
**Respuesta esperada:** Estado de Resultados muestra "✅ NEGOCIO RENTABLE" con utilidad neta de $X y margen neto de Y%

### 3. ¿Estoy recuperando el crédito?
**Respuesta esperada:** "Recuperación del 78% en los últimos 30 días, con cartera vencida de $X"

### 4. ¿Quién me debe más?
**Respuesta esperada:** "Cliente ABC con saldo de $X, estado MOROSO, 95 días vencido"

### 5. ¿Cuáles son mis alertas?
**Respuesta esperada:** "3 productos bajo stock, 2 clientes morosos, cartera vencida de $X"

---

## 🎯 CASOS DE USO REALES

### Caso 1: Análisis de Camarón 21-25
**Objetivo:** Determinar si es rentable vender este producto

**Pasos:**
1. Ir a Utilidad por Producto
2. Buscar "CAMARON 21-25" o filtrar por categoría "Mariscos"
3. Analizar:
   - Margen %: ¿Es mayor a 20%?
   - Rentabilidad: ¿ALTA, MEDIA, BAJA o PÉRDIDA?
   - Recomendación: ¿Qué sugiere el sistema?

**Decisión:**
- ALTA (≥30%): Excelente, continuar y promover
- MEDIA (15-30%): Aceptable, mantener
- BAJA (<15%): Ajustar precio o reducir stock
- PÉRDIDA: Descontinuar o revisar costos

---

### Caso 2: Viabilidad del negocio
**Objetivo:** ¿Debo cerrar o continuar?

**Pasos:**
1. Ir a Estado de Resultados
2. Seleccionar: Últimos 3 meses
3. Analizar utilidad neta y margen %

**Decisión:**
- Utilidad neta positiva + margen ≥15%: ✅ Continuar
- Utilidad neta positiva pero margen <10%: ⚠️ Optimizar
- Pérdidas por 3+ meses: ❌ Reestructurar o cerrar

---

### Caso 3: Cliente moroso
**Objetivo:** Decidir si suspender crédito

**Pasos:**
1. Ir a Cartera de Clientes
2. Buscar cliente
3. Ver estado y días vencido

**Decisión:**
- AL CORRIENTE: Enviar recordatorio amigable
- VENCIDO (31-60 días): Llamada urgente, recordar plazo
- MOROSO (>60 días): Suspender crédito inmediatamente, iniciar cobranza formal

---

## 📞 AYUDA

Si algo no funciona o tienes dudas:

1. **Revisar documentación:** `SISTEMA_REPORTES_COMPLETADO.md`
2. **Ver análisis original:** `ANALISIS_SISTEMA_REPORTES.md`
3. **Consultar plan:** `PLAN_IMPLEMENTACION_REPORTES.md`
4. **Errores de BD:** Revisar `CREAR_SP_REPORTES_AVANZADOS_CORREGIDO.sql`

---

**Fecha:** 22 de Enero de 2026  
**Versión de pruebas:** 1.0  
**Estado:** ✅ LISTO PARA PROBAR
