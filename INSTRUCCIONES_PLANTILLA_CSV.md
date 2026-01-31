# INSTRUCCIONES - PLANTILLA CSV DE INVENTARIO INICIAL

## 📥 Descargar Plantilla

1. Ve a: **Inventario → Inventario Inicial**
2. Click en botón: **"Descargar Plantilla CSV"**
3. Se descargará un archivo: `PlantillaInventarioInicial_YYYYMMDD.csv`

## 📝 Formato de la Plantilla

El archivo CSV contiene las siguientes columnas:

| Columna | Descripción | Tipo | Ejemplo |
|---------|-------------|------|---------|
| **ProductoID** | ID del producto (NO MODIFICAR) | Número | 1 |
| **CodigoInterno** | Código del producto (Referencia) | Texto | PROD001 |
| **NombreProducto** | Nombre del producto (Referencia) | Texto | Coca-Cola 600ml |
| **StockActual** | Stock actual en sistema | Número | 0 |
| **CantidadNueva** | ✏️ LLENAR: Cantidad real que tienes | Número | 50 |
| **CostoUnitario** | ✏️ LLENAR: Costo de compra | Decimal | 8.50 |
| **PrecioVenta** | ✏️ LLENAR: Precio al público | Decimal | 15.00 |
| **Comentarios** | Opcional: Notas adicionales | Texto | Lote 12345 |

## ✏️ Cómo Llenar la Plantilla

### 1. Abrir el Archivo

**Opción A - Excel:**
```
1. Abrir Microsoft Excel
2. Archivo → Abrir → Seleccionar el CSV
3. Elegir "Delimitado por comas"
```

**Opción B - LibreOffice Calc:**
```
1. Abrir LibreOffice Calc
2. Archivo → Abrir → Seleccionar el CSV
3. Separador: Coma
4. Codificación: UTF-8
```

**Opción C - Google Sheets:**
```
1. Google Sheets → Archivo → Importar
2. Subir el archivo CSV
3. Separador: Coma
```

### 2. Llenar los Datos

Para cada producto que quieras agregar:

1. **CantidadNueva**: Escribe cuántas unidades tienes realmente
2. **CostoUnitario**: Escribe el costo de compra (sin IVA)
3. **PrecioVenta**: Escribe el precio al que vendes
4. **Comentarios** (Opcional): Cualquier nota

**Ejemplo:**
```csv
ProductoID,CodigoInterno,NombreProducto,StockActual,CantidadNueva,CostoUnitario,PrecioVenta,Comentarios
1,PROD001,Coca-Cola 600ml,0,50,8.50,15.00,Lote nuevo
2,PROD002,Sabritas 60g,0,120,5.00,10.00,
3,PROD003,Agua 1L,0,200,3.50,7.00,Revisar fecha
```

### 3. Guardar y Usar

**NO NECESITAS SUBIR EL CSV AL SISTEMA**

La plantilla es solo una **guía y respaldo** para:
- 📋 Tener un registro de tu inventario antes de cargar
- 🔍 Revisar y verificar datos
- 👥 Compartir con tu equipo para llenar entre varios
- 💾 Guardar como respaldo

**Para cargar al sistema:**
1. Ve a **Inventario Inicial → Nueva Carga**
2. Usa la interfaz web para agregar productos uno por uno
3. El sistema buscará cada producto y lo agregarás manualmente

## 📊 Ventajas de Usar la Plantilla

✅ **Organización**: Tienes todos tus productos listados  
✅ **Revisión**: Puedes revisar antes de cargar al sistema  
✅ **Respaldo**: Guardas una copia de tu inventario  
✅ **Trabajo en equipo**: Varios pueden llenar la plantilla  
✅ **Offline**: Puedes llenarla sin conexión  

## ⚠️ Notas Importantes

### NO Modifiques:
- ❌ ProductoID
- ❌ CodigoInterno
- ❌ NombreProducto
- ❌ StockActual

Estas columnas son solo **referencia** para que identifiques el producto.

### Solo Llena:
- ✏️ CantidadNueva
- ✏️ CostoUnitario
- ✏️ PrecioVenta
- ✏️ Comentarios (opcional)

### Formato de Números:
- **Enteros**: `50`, `120`, `200`
- **Decimales**: `8.50`, `15.00`, `3.50` (punto como separador)
- **NO usar comas** en los números: `8.50` ✅ | `8,50` ❌

## 🔄 Flujo de Trabajo Recomendado

### Escenario: Tienda con 100 productos

**Día 1: Preparación**
```
1. Descargar plantilla CSV
2. Abrir en Excel/LibreOffice
3. Imprimir lista de productos
4. Hacer inventario físico
```

**Día 2: Llenado**
```
1. Contar productos en tienda
2. Llenar CantidadNueva en Excel
3. Verificar costos y precios
4. Revisar con encargado
5. Guardar Excel con fecha
```

**Día 3: Carga al Sistema**
```
1. Ir a: Inventario Inicial → Nueva Carga
2. Para cada producto en Excel:
   - Buscar producto en el sistema
   - Ingresar cantidad del Excel
   - Ingresar costo y precio
   - Click "Agregar"
3. Verificar totales
4. Finalizar carga
```

## 📋 Ejemplo Completo

### Plantilla Original (descargada):
```csv
ProductoID,CodigoInterno,NombreProducto,StockActual,CantidadNueva,CostoUnitario,PrecioVenta,Comentarios
1,524226462632,Aceite Canola,0,0,0.00,0.00,
2,PROD001,Producto Prueba POS,0,0,0.00,0.00,
1194,8888,CAMARON CHICO 111-130,0,0,0.00,0.00,
```

### Plantilla Llena (después de inventario):
```csv
ProductoID,CodigoInterno,NombreProducto,StockActual,CantidadNueva,CostoUnitario,PrecioVenta,Comentarios
1,524226462632,Aceite Canola,0,25,45.50,85.00,Proveedor ABC
2,PROD001,Producto Prueba POS,0,10,12.00,25.00,Descontinuar
1194,8888,CAMARON CHICO 111-130,0,5.5,180.00,320.00,Por kilo
```

## 🆘 Problemas Comunes

### "Excel no abre bien el CSV"
**Solución:**
1. Click derecho en el archivo → Abrir con → Excel
2. Datos → Desde texto/CSV
3. Delimitador: Coma
4. Origen: UTF-8

### "Los acentos se ven mal"
**Solución:**
- Usa UTF-8 al abrir
- O simplemente ignora, son solo referencia

### "Quiero agregar productos que no están"
**Solución:**
1. Primero agrégalos al catálogo de productos
2. Descarga nueva plantilla
3. Los nuevos productos aparecerán

### "Borré una columna por error"
**Solución:**
- Descarga la plantilla de nuevo
- Copia tus datos a la nueva plantilla

## 💡 Consejos Profesionales

### Para Tiendas Pequeñas (< 50 productos):
- No necesitas la plantilla
- Usa directamente la interfaz web
- Más rápido y fácil

### Para Tiendas Medianas (50-200 productos):
- ✅ Usa la plantilla
- Llena offline
- Carga después con calma

### Para Tiendas Grandes (> 200 productos):
- ✅ Descarga plantilla
- Divide por categorías
- Asigna secciones al equipo
- Valida antes de cargar
- Considera automatización futura

## 🎯 Checklist de Uso

Antes de empezar:
- [ ] Descargué la plantilla CSV
- [ ] Puedo abrir el archivo en Excel/LibreOffice
- [ ] Tengo acceso a inventario físico
- [ ] Conozco costos y precios actuales

Durante el llenado:
- [ ] No modifiqué ProductoID
- [ ] Usé punto (.) para decimales
- [ ] Verifiqué cantidades con inventario físico
- [ ] Revisé que costo < precio
- [ ] Guardé una copia del archivo

Para cargar al sistema:
- [ ] Abro la plantilla en una pantalla
- [ ] Abro el sistema en otra pantalla
- [ ] Cargo productos uno por uno
- [ ] Verifico totales antes de finalizar
- [ ] Hago backup antes de aplicar

---

## 📞 Necesitas Ayuda?

Si tienes problemas:
1. Revisa esta guía completa
2. Consulta MODULO_INVENTARIO_INICIAL.md
3. Contacta al administrador del sistema

**Recuerda**: La plantilla es solo una **herramienta de apoyo**. La carga real se hace en la interfaz web del sistema, producto por producto.

---

**Éxito con tu inventario inicial!** 🎉
