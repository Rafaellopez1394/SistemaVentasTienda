# GUÍA DE USO: VENTA POR GRAMAJE Y DESCOMPOSICIÓN DE PRODUCTOS

## 📋 Índice
1. [Venta por Gramaje](#venta-por-gramaje)
2. [Descomposición de Productos](#descomposición-de-productos)
3. [Instalación y Configuración](#instalación-y-configuración)
4. [Ejemplos de Uso](#ejemplos-de-uso)

---

## 🎯 Venta por Gramaje

### ¿Qué es?
La venta por gramaje permite vender productos por peso (gramos/kilogramos) en lugar de por unidades completas. El sistema calcula automáticamente el precio según el peso ingresado.

### Características
- ✅ Ingreso de cantidad en gramos
- ✅ Cálculo automático del precio proporcional
- ✅ Conversión automática a kilogramos
- ✅ Botones rápidos para cantidades comunes (250g, 500g, 1kg, 2kg, 5kg)
- ✅ Visualización clara en el carrito de compras
- ✅ Manejo de IVA sobre el precio calculado

### Configurar un Producto para Venta por Gramaje

#### Opción 1: Directamente en la Base de Datos
```sql
-- Ejemplo: Configurar Azúcar para venta por gramaje
UPDATE Productos 
SET VentaPorGramaje = 1, 
    PrecioPorKilo = 25.00,  -- Precio por kilogramo
    UnidadMedidaBase = 'KILO'
WHERE Nombre LIKE '%Azúcar%'
```

#### Opción 2: Desde la Interfaz (Próximamente)
1. Ir a **Productos** > **Editar Producto**
2. Marcar la casilla "Venta por Gramaje"
3. Ingresar el "Precio por Kilo"
4. Seleccionar "Unidad de Medida Base" (KILO, GRAMO, LITRO)
5. Guardar

### Cómo Vender por Gramaje

#### En el Punto de Venta (POS):
1. **Buscar el producto** como lo hace normalmente
2. Si el producto está configurado para gramaje, al hacer clic se abrirá un **modal especial**
3. **Ingresar la cantidad en gramos**:
   - Puede usar los botones rápidos (250g, 500g, 1kg, etc.)
   - O escribir una cantidad personalizada
4. El sistema muestra:
   - Equivalente en kilogramos
   - **Precio calculado automáticamente**
5. Hacer clic en **"Agregar al Carrito"**
6. En el carrito verá:
   - El producto con un badge indicando el gramaje
   - El precio calculado
   - El precio por kilo

### Ejemplo Práctico
```
Producto: Azúcar Refinada
Precio por Kilo: $25.00

Cliente quiere: 750 gramos
Cálculo automático: (25.00 / 1000) * 750 = $18.75

En el ticket aparecerá:
Azúcar Refinada - 750g (0.750 kg)
Precio: $18.75
IVA 16%: $3.00
Total: $21.75
```

---

## 📦 Descomposición de Productos

### ¿Qué es?
La descomposición permite dividir un producto grande en productos más pequeños, ajustando automáticamente el inventario.

### Caso de Uso Principal
**Ejemplo:**
- Tiene: 1 costal de azúcar de 20 kg
- Quiere: 5 bolsas de 2 kg + 10 bolsas de 1 kg
- El sistema:
  - ✅ Descuenta 1 costal de 20 kg del inventario
  - ✅ Agrega 5 unidades de bolsas de 2 kg
  - ✅ Agrega 10 unidades de bolsas de 1 kg
  - ✅ Registra el historial de la descomposición

### Características
- ✅ Descomposición múltiple (un producto → varios productos)
- ✅ Validación de stock disponible
- ✅ Cálculo automático de pesos totales
- ✅ Historial completo de descomposiciones
- ✅ Trazabilidad por usuario y fecha
- ✅ Observaciones personalizadas

### Cómo Descomponer Productos

#### Acceder al Módulo:
1. Ir a **Inventario** > **Descomposición de Productos** (o ruta configurada)

#### Proceso de Descomposición:

**Paso 1: Seleccionar Producto Origen**
- Seleccione el producto grande que desea descomponer
- El sistema muestra el stock disponible
- Ingrese la cantidad a descomponer

**Paso 2: Agregar Productos Resultantes**
Para cada producto resultante:
1. Seleccione el producto
2. Ingrese la cantidad que se generará
3. **(Opcional)** Ingrese el peso de cada unidad en kg
   - Ejemplo: Si genera bolsas de 2 kg, ingrese 2.0
4. Clic en **"Agregar"**

**Paso 3: Revisar y Registrar**
- Verifique la tabla de productos resultantes
- Agregue observaciones si lo desea
- Clic en **"Registrar Descomposición"**

### Ejemplo Práctico Completo

#### Escenario:
Tiene 1 costal de 20 kg de frijol y quiere dividirlo en bolsas menores.

#### Pasos:

```
1. PRODUCTO ORIGEN:
   - Producto: Costal Frijol Negro 20kg
   - Cantidad a descomponer: 1
   - Stock disponible: 5

2. PRODUCTOS RESULTANTES:
   
   a) Bolsas de 2 kg:
      - Producto: Bolsa Frijol Negro 2kg
      - Cantidad: 5
      - Peso c/u: 2.0 kg
      - Total: 10 kg
   
   b) Bolsas de 1 kg:
      - Producto: Bolsa Frijol Negro 1kg
      - Cantidad: 10
      - Peso c/u: 1.0 kg
      - Total: 10 kg

3. OBSERVACIONES:
   "Descomposición para venta al menudeo"

4. RESULTADO EN INVENTARIO:
   - Costal Frijol Negro 20kg: 5 → 4 unidades (-1)
   - Bolsa Frijol Negro 2kg: +5 unidades
   - Bolsa Frijol Negro 1kg: +10 unidades
```

### Validaciones del Sistema

El sistema valida automáticamente:
- ✅ Stock suficiente del producto origen
- ✅ Productos resultantes válidos
- ✅ Cantidades mayores a cero
- ✅ Al menos un producto resultante

---

## ⚙️ Instalación y Configuración

### Paso 1: Ejecutar Scripts SQL

En SQL Server Management Studio, ejecute en orden:

```sql
-- 1. Script principal de venta por gramaje y descomposición
-- Ubicación: Utilidad/SQL Server/024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql

-- 2. Actualización del SP de búsqueda
-- Ubicación: Utilidad/SQL Server/024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql
```

### Paso 2: Compilar el Proyecto

```powershell
# En Visual Studio:
# 1. Compilar Solución (Ctrl + Shift + B)
# 2. Verificar que no haya errores
```

### Paso 3: Configurar Productos Iniciales

```sql
-- Ejemplo: Configurar productos comunes para gramaje
UPDATE Productos 
SET VentaPorGramaje = 1, 
    PrecioPorKilo = 25.00,
    UnidadMedidaBase = 'KILO'
WHERE Nombre IN ('Azúcar', 'Arroz', 'Frijol', 'Harina')

-- Ejemplo: Verificar configuración
SELECT ProductoID, Nombre, VentaPorGramaje, PrecioPorKilo, UnidadMedidaBase
FROM Productos
WHERE VentaPorGramaje = 1
```

### Paso 4: Crear Productos para Descomposición

**Importante**: Para descomponer, debe tener creados tanto los productos origen como los resultantes.

Ejemplo:
```sql
-- Producto origen (ya existente o crear)
-- Costal Frijol Negro 20kg

-- Productos resultantes (crear si no existen)
-- Bolsa Frijol Negro 2kg
-- Bolsa Frijol Negro 1kg
```

### Paso 5: Configurar Menú (Opcional)

Si desea agregar el módulo de descomposición al menú:

```sql
-- Agregar en el menú de Inventario
INSERT INTO SUBMENU (MenuID, Nombre, Controlador, Vista, Icono, Activo)
VALUES (
    (SELECT MenuID FROM MENU WHERE Nombre = 'Inventario'),
    'Descomposición',
    'DescomposicionProducto',
    'Index',
    'fas fa-boxes',
    1
)

-- Dar permisos al rol correspondiente
INSERT INTO PERMISOS (RolID, SubMenuID, Activo)
SELECT r.RolID, s.SubMenuID, 1
FROM ROL r, SUBMENU s
WHERE r.Descripcion = 'Administrador'
  AND s.Nombre = 'Descomposición'
```

---

## 💡 Ejemplos de Uso

### Ejemplo 1: Venta de Carne por Gramos

```
Producto: Carne de Res Premium
Precio por Kilo: $180.00
Configuración: VentaPorGramaje = 1

Cliente solicita: 350 gramos
Sistema calcula: $180.00 / 1000 * 350 = $63.00
+ IVA 16% = $10.08
Total = $73.08
```

### Ejemplo 2: Venta de Queso por Gramos

```
Producto: Queso Manchego
Precio por Kilo: $320.00

Cliente 1: 250g → $80.00
Cliente 2: 500g → $160.00
Cliente 3: 125g → $40.00
```

### Ejemplo 3: Descomposición de Costal de Azúcar

```
ORIGEN:
1 Costal Azúcar Refinada 20kg (Stock: 3)

RESULTANTES:
- 5 Bolsas de 2kg = 10kg
- 10 Bolsas de 1kg = 10kg
Total descompuesto: 20kg ✓

INVENTARIO DESPUÉS:
- Costal 20kg: 3 → 2 (-1)
- Bolsas 2kg: 0 → 5 (+5)
- Bolsas 1kg: 0 → 10 (+10)
```

### Ejemplo 4: Descomposición de Caja de Cerveza

```
ORIGEN:
1 Caja Cerveza 24 piezas (Stock: 10)

RESULTANTES:
- 2 Six-pack (6 piezas c/u)
- 12 Cervezas individuales

INVENTARIO DESPUÉS:
- Caja 24 piezas: 10 → 9 (-1)
- Six-pack: +2
- Cerveza individual: +12
```

---

## 📊 Reportes y Consultas Útiles

### Ver Productos Configurados para Gramaje
```sql
SELECT 
    ProductoID,
    Nombre,
    PrecioPorKilo,
    UnidadMedidaBase,
    Estatus
FROM Productos
WHERE VentaPorGramaje = 1
ORDER BY Nombre
```

### Ver Historial de Descomposiciones
```sql
SELECT * FROM vw_HistorialDescomposiciones
ORDER BY FechaDescomposicion DESC
```

### Ver Ventas por Gramaje
```sql
SELECT 
    v.VentaID,
    v.FechaVenta,
    p.Nombre AS Producto,
    dv.Gramos,
    dv.PrecioCalculado,
    dv.Cantidad * dv.PrecioCalculado AS Total
FROM DetalleVenta dv
INNER JOIN Venta v ON dv.VentaID = v.VentaID
INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
WHERE dv.Gramos IS NOT NULL
ORDER BY v.FechaVenta DESC
```

---

## 🔧 Solución de Problemas

### Problema: Modal de gramaje no aparece
**Solución:**
1. Verificar que el producto tenga `VentaPorGramaje = 1`
2. Verificar que tenga `PrecioPorKilo` configurado
3. Verificar que el archivo `VentaPOS_Gramaje.js` esté cargando correctamente

### Problema: Error al registrar descomposición
**Solución:**
1. Verificar que el producto origen tenga stock suficiente
2. Verificar que todos los productos resultantes existan en la BD
3. Revisar que el stored procedure `SP_RegistrarDescomposicionProducto` esté creado

### Problema: No calcula bien el precio por gramaje
**Solución:**
La fórmula correcta es: `(PrecioPorKilo / 1000) * Gramos`
Verificar que el PrecioPorKilo esté en la unidad correcta.

---

## 📝 Notas Importantes

1. **Stock en Productos por Gramaje**: Aunque se vende por gramos, el stock se maneja por unidad completa del lote.

2. **Descomposición es Irreversible**: Una vez registrada, no se puede deshacer automáticamente. Tendrá que hacer ajustes manuales de inventario si es necesario.

3. **Pesos en Descomposición**: El campo "Peso c/u" es opcional pero recomendado para control y trazabilidad.

4. **IVA**: Se calcula sobre el precio final calculado por gramaje.

5. **Permisos**: Asegúrese de configurar los permisos adecuados para el módulo de descomposición.

---

## 📞 Soporte

Para preguntas o problemas:
- Revisar logs de errores en SQL Server
- Verificar consola del navegador (F12) para errores JavaScript
- Consultar documentación adicional en el proyecto

---

**Fecha de creación**: 29 de Diciembre de 2025
**Versión**: 1.0
**Autor**: Sistema de Ventas Tienda - Módulo de Gramaje y Descomposición
