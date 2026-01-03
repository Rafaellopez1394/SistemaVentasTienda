# 🚀 GUÍA DE IMPLEMENTACIÓN RÁPIDA

## ⚡ Inicio Rápido - 5 Pasos

### 📋 Pre-requisitos
- ✅ SQL Server Management Studio
- ✅ Visual Studio
- ✅ Acceso a la base de datos DBVENTAS_WEB
- ✅ Sistema compilando sin errores

---

## 🔧 PASO 1: Ejecutar Scripts SQL (5 minutos)

Abrir SQL Server Management Studio y ejecutar en orden:

### 1.1 Script Principal
```sql
-- Ubicación: Utilidad/SQL Server/024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql
-- Este script crea todas las tablas y stored procedures necesarios
```

**¿Qué hace?**
- ✅ Agrega campos VentaPorGramaje, PrecioPorKilo, UnidadMedidaBase a tabla Productos
- ✅ Crea tabla DescomposicionProducto
- ✅ Crea tabla DetalleDescomposicion
- ✅ Crea SP_RegistrarDescomposicionProducto
- ✅ Crea SP_CalcularPrecioPorGramaje
- ✅ Crea vw_HistorialDescomposiciones

### 1.2 Actualización de Búsqueda
```sql
-- Ubicación: Utilidad/SQL Server/024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql
-- Actualiza el stored procedure de búsqueda para incluir campos de gramaje
```

### 1.3 Datos de Ejemplo (Opcional)
```sql
-- Ubicación: Utilidad/SQL Server/DATOS_EJEMPLO_GRAMAJE_Y_DESCOMPOSICION.sql
-- Crea productos de ejemplo para probar las funcionalidades
```

**Verificación:**
```sql
-- Verificar que los campos se agregaron correctamente
SELECT TOP 1 VentaPorGramaje, PrecioPorKilo, UnidadMedidaBase 
FROM Productos

-- Verificar que las tablas se crearon
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('DescomposicionProducto', 'DetalleDescomposicion')

-- Verificar que los SP se crearon
SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_NAME LIKE '%Descomposicion%' OR ROUTINE_NAME LIKE '%Gramaje%'
```

---

## 📦 PASO 2: Compilar Proyecto (2 minutos)

### En Visual Studio:

1. **Compilar Solución**
   - Presionar `Ctrl + Shift + B`
   - O ir a Menú: Compilar > Recompilar Solución

2. **Verificar que no hay errores**
   - Ver ventana de Salida
   - Debe decir: "Compilación: 4 correctas o actualizadas, 0 incorrectas, 0 omitidas"

### Si hay errores:

**Error común: "No se puede encontrar DescomposicionProducto"**
```
Solución: Asegúrese que los archivos nuevos estén incluidos en el proyecto
- Click derecho en CapaModelo > Agregar > Elemento existente
- Seleccionar DescomposicionProducto.cs
```

**Error común: "Falta Newtonsoft.Json"**
```
Solución: Instalar paquete NuGet
- Click derecho en proyecto > Administrar paquetes NuGet
- Buscar: Newtonsoft.Json
- Instalar
```

---

## 🎨 PASO 3: Configurar Menú (Opcional - 3 minutos)

### Agregar al menú del sistema:

```sql
-- 1. Verificar ID del menú de Inventario
SELECT MenuID, Nombre FROM MENU WHERE Nombre LIKE '%Inventario%'

-- 2. Agregar submenu (ajustar @MenuID según resultado anterior)
DECLARE @MenuID INT = 3 -- AJUSTAR ESTE VALOR

INSERT INTO SUBMENU (MenuID, Nombre, Controlador, Vista, Icono, Activo)
VALUES (@MenuID, 'Descomposición', 'DescomposicionProducto', 'Index', 'fas fa-boxes', 1)

-- 3. Dar permisos a Administrador
INSERT INTO PERMISOS (RolID, SubMenuID, Activo)
SELECT 
    (SELECT RolID FROM ROL WHERE Descripcion = 'Administrador'),
    (SELECT SubMenuID FROM SUBMENU WHERE Nombre = 'Descomposición'),
    1
```

---

## 🧪 PASO 4: Pruebas (10 minutos)

### 4.1 Probar Venta por Gramaje

1. **Configurar un producto**
   ```sql
   -- Ejemplo: Configurar Azúcar
   UPDATE Productos 
   SET VentaPorGramaje = 1,
       PrecioPorKilo = 25.00,
       UnidadMedidaBase = 'KILO'
   WHERE Nombre LIKE '%Az%car%'
   ```

2. **Probar en POS**
   - Ir a Punto de Venta
   - Buscar "Azúcar"
   - Hacer clic en el producto
   - ✅ Debe aparecer un modal con campo de gramos
   - Ingresar 500g
   - ✅ Debe calcular $12.50 (si precio/kg = $25.00)
   - Agregar al carrito
   - ✅ Debe aparecer con badge "500g (0.500 kg)"

### 4.2 Probar Descomposición

1. **Crear productos necesarios**
   ```sql
   -- Si ejecutó DATOS_EJEMPLO_GRAMAJE_Y_DESCOMPOSICION.sql
   -- Ya tiene productos de ejemplo creados
   
   -- Verificar productos
   SELECT * FROM Productos 
   WHERE CodigoInterno LIKE 'AZU-%'
   ```

2. **Realizar descomposición**
   - Ir a Descomposición de Productos
   - Seleccionar "Costal Azúcar 20kg"
   - Cantidad: 1
   - Agregar resultante: Bolsa 2kg, Cantidad: 5, Peso: 2.0
   - Agregar resultante: Bolsa 1kg, Cantidad: 10, Peso: 1.0
   - Registrar
   - ✅ Debe mostrar mensaje de éxito

3. **Verificar inventario**
   ```sql
   -- Ver cambios en inventario
   SELECT p.Nombre, ps.Stock
   FROM ProductosSucursal ps
   INNER JOIN Productos p ON ps.ProductoID = p.ProductoID
   WHERE p.CodigoInterno LIKE 'AZU-%'
   ORDER BY p.Nombre
   ```

---

## ✅ PASO 5: Verificación Final (5 minutos)

### Checklist de Verificación:

**Base de Datos:**
- [ ] Campos agregados a tabla Productos
- [ ] Tablas de descomposición creadas
- [ ] Stored procedures funcionando
- [ ] Vista de historial disponible

**Aplicación:**
- [ ] Proyecto compila sin errores
- [ ] Modal de gramaje aparece correctamente
- [ ] Cálculo de precio por gramaje funciona
- [ ] Módulo de descomposición accesible
- [ ] Historial de descomposiciones se muestra

**Funcionalidad:**
- [ ] Puede vender productos por gramaje
- [ ] Precio se calcula correctamente
- [ ] Productos aparecen en carrito con badge
- [ ] Puede descomponer productos
- [ ] Inventario se ajusta correctamente
- [ ] Historial muestra descomposiciones

---

## 🔍 Solución de Problemas Comunes

### Problema: Modal de gramaje no aparece

**Causa posible:** Script JavaScript no está cargando

**Solución:**
1. Verificar que `VentaPOS_Gramaje.js` existe en carpeta Scripts/Views/
2. Verificar que está referenciado en Index.cshtml:
   ```html
   <script src="~/Scripts/Views/VentaPOS_Gramaje.js?v=@DateTime.Now.Ticks"></script>
   ```
3. Verificar en navegador (F12) que no hay errores de carga

---

### Problema: Error al registrar descomposición

**Causa posible:** Productos resultantes no existen

**Solución:**
```sql
-- Verificar que todos los productos existen
SELECT ProductoID, Nombre FROM Productos 
WHERE ProductoID IN ([IDs que está intentando usar])

-- Verificar stock del producto origen
SELECT p.Nombre, ps.Stock
FROM Productos p
LEFT JOIN ProductosSucursal ps ON p.ProductoID = ps.ProductoID
WHERE p.ProductoID = [ID del producto origen]
```

---

### Problema: Precio no se calcula en modal

**Causa posible:** Producto no tiene PrecioPorKilo configurado

**Solución:**
```sql
-- Verificar configuración
SELECT ProductoID, Nombre, VentaPorGramaje, PrecioPorKilo
FROM Productos
WHERE ProductoID = [ID del producto]

-- Configurar si es necesario
UPDATE Productos
SET VentaPorGramaje = 1,
    PrecioPorKilo = [PRECIO],
    UnidadMedidaBase = 'KILO'
WHERE ProductoID = [ID del producto]
```

---

## 📊 Consultas Útiles de Monitoreo

### Ver productos configurados para gramaje:
```sql
SELECT 
    ProductoID,
    Nombre,
    FORMAT(PrecioPorKilo, 'C', 'es-MX') AS PrecioPorKilo,
    UnidadMedidaBase,
    Estatus
FROM Productos
WHERE VentaPorGramaje = 1
ORDER BY Nombre
```

### Ver últimas descomposiciones:
```sql
SELECT TOP 10 * 
FROM vw_HistorialDescomposiciones
ORDER BY FechaDescomposicion DESC
```

### Ver ventas por gramaje del día:
```sql
SELECT 
    v.VentaID,
    v.FechaVenta,
    p.Nombre,
    dv.Gramos,
    FORMAT(dv.PrecioCalculado, 'C', 'es-MX') AS PrecioCalculado
FROM DetalleVenta dv
INNER JOIN Venta v ON dv.VentaID = v.VentaID
INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
WHERE dv.Gramos IS NOT NULL
  AND CAST(v.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY v.FechaVenta DESC
```

---

## 🎓 Capacitación de Usuarios

### Para Cajeros (Venta por Gramaje):

**Instrucción Simple:**
1. Buscar el producto como siempre
2. Si aparece un modal pidiendo gramos, ingresar la cantidad
3. Revisar que el precio calculado sea correcto
4. Agregar al carrito
5. Continuar con la venta normal

**Tip:** Los productos que se venden por gramaje tienen un ícono de peso 🎯

---

### Para Personal de Almacén (Descomposición):

**Instrucción Simple:**
1. Ir al módulo "Descomposición de Productos"
2. Seleccionar el producto grande que van a dividir
3. Indicar cuántos van a dividir
4. Agregar cada tipo de producto pequeño que van a generar
5. Registrar
6. El sistema ajusta el inventario automáticamente

**Importante:** Una vez registrada, la descomposición no se puede deshacer automáticamente.

---

## 📚 Documentación Adicional

- **Guía Completa**: `GUIA_VENTA_GRAMAJE_Y_DESCOMPOSICION.md`
- **Resumen Técnico**: `RESUMEN_GRAMAJE_Y_DESCOMPOSICION.md`
- **Scripts SQL**: Carpeta `Utilidad/SQL Server/`

---

## ✨ ¡Listo para Producción!

Si completó todos los pasos anteriores, el sistema está listo para usar en producción.

### Siguientes Pasos Recomendados:

1. ✅ Capacitar a usuarios
2. ✅ Configurar productos iniciales
3. ✅ Realizar pruebas con datos reales
4. ✅ Monitorear primeros días de uso
5. ✅ Recopilar feedback de usuarios

---

**Tiempo total de implementación: ~25 minutos**

**Nivel de dificultad: ⭐⭐ (Medio-Bajo)**

**Soporte**: Revisar documentación o logs del sistema para troubleshooting.

---

*Última actualización: 29 de Diciembre de 2025*
