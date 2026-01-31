# GUÍA RÁPIDA - INVENTARIO INICIAL

## 🚀 Inicio Rápido (5 pasos)

### 1. Abrir el Módulo
```
Menú → Inventario → Inventario Inicial
```

### 2. Crear Nueva Carga
- Click en **"Nueva Carga Inicial"**
- Escribir comentario: "Migración desde [nombre sistema anterior]"
- Click en **"Crear Carga y Continuar"**

### 3. Agregar Productos

Para cada producto:

1. **Buscar**: Escribe nombre o código en el campo de búsqueda
2. **Seleccionar**: Click en el producto de la lista
3. **Ingresar datos**:
   - **Cantidad**: Stock actual que tienes
   - **Costo**: Lo que te costó comprarlo
   - **Precio**: Lo que vendes al público
4. **Click "Agregar"**
5. Repetir para todos tus productos

### 4. Verificar Totales

Revisa los números en la parte superior:
- ✅ Productos: Cantidad de productos agregados
- ✅ Unidades: Total de piezas
- ✅ Valor Total: Costo total del inventario

### 5. Finalizar

- Click en **"Finalizar Carga"**
- Confirmar en el mensaje
- ✅ ¡Listo! Tu inventario está en el sistema

---

## ⚠️ IMPORTANTE

### Antes de Finalizar:
- ✅ Verifica que todos los productos están correctos
- ✅ Revisa las cantidades
- ✅ Confirma los costos y precios
- ⚠️ **No podrás modificar después de finalizar**

### Recomendaciones:
- 📋 Prepara una lista en Excel antes de empezar
- ⏰ Hazlo en horario sin ventas
- 💾 Pide un backup de la base de datos antes
- ✔️ Verifica el inventario después de aplicar

---

## 📋 Ejemplo Práctico

### Escenario:
Tienes 3 productos en tu tienda antigua que quieres migrar:

```
1. Coca-Cola 600ml
   - Cantidad actual: 50 piezas
   - Costo: $8.50 c/u
   - Precio venta: $15.00 c/u

2. Sabritas 60g
   - Cantidad actual: 120 piezas
   - Costo: $5.00 c/u
   - Precio venta: $10.00 c/u

3. Agua 1L
   - Cantidad actual: 200 piezas
   - Costo: $3.50 c/u
   - Precio venta: $7.00 c/u
```

### Proceso:

**1. Nueva Carga:**
```
Comentarios: "Migración desde QuickBooks - 30/01/2026"
```

**2. Agregar Productos:**

Para Coca-Cola:
- Buscar: "coca"
- Seleccionar: "Coca-Cola 600ml"
- Cantidad: 50
- Costo: 8.50
- Precio: 15.00
- Click "Agregar" ✅

Repetir para Sabritas y Agua...

**3. Verificar Totales:**
```
Productos: 3
Unidades: 370.00
Valor Total: $1,545.00
```

**4. Finalizar:**
- Click "Finalizar Carga"
- Confirmar
- ✅ ¡Sistema aplicado!

### Resultado:
- ✅ 3 lotes creados en inventario
- ✅ 3 movimientos registrados
- ✅ Productos listos para vender

---

## 🔍 Verificar que Funcionó

### Opción 1: En el Sistema
```
Menú → Inventario → Inventario Inicial
```
Verás tu carga en el historial con estado "Finalizada" ✅

### Opción 2: SQL
```sql
-- Ver lotes creados hoy
SELECT * FROM LotesProducto 
WHERE CAST(FechaEntrada AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY LoteID DESC;

-- Ver movimientos de inventario inicial
SELECT * FROM InventarioMovimientos 
WHERE TipoMovimiento = 'INVENTARIO_INICIAL'
ORDER BY MovimientoID DESC;
```

---

## ❓ Preguntas Frecuentes

**P: ¿Puedo pausar y continuar después?**  
R: ✅ Sí, la carga se guarda automáticamente. Cierra y vuelve cuando quieras.

**P: ¿Puedo modificar un producto después de agregarlo?**  
R: ✅ Sí, ANTES de finalizar. Usa el botón 🗑️ para eliminarlo y agrégalo de nuevo.

**P: ¿Qué pasa si me equivoco después de finalizar?**  
R: ⚠️ No se puede modificar. Contacta al administrador del sistema.

**P: ¿Cuántas veces uso este módulo?**  
R: Generalmente **UNA SOLA VEZ** al migrar desde otro sistema.

**P: ¿Dónde van los productos después de finalizar?**  
R: Al inventario principal. Verás el stock en cualquier consulta de productos.

**P: ¿Afecta mis ventas actuales?**  
R: ❌ No. El inventario inicial solo establece el punto de partida.

---

## 🆘 Soporte

### Si algo no funciona:

1. **Verifica que el producto existe:**
   ```
   Menú → Catálogo → Productos
   ```

2. **Revisa que la carga está activa:**
   ```
   Menú → Inventario → Inventario Inicial
   ```

3. **Consulta los logs:**
   ```sql
   SELECT * FROM VW_HistorialInventarioInicial;
   ```

4. **Pide ayuda:**
   - Toma captura de pantalla del error
   - Anota qué estabas haciendo
   - Contacta al administrador

---

## 📞 Contacto

**Soporte Técnico:** Rafael Lopez  
**Sistema:** Las Águilas Mercado del Mar  
**Versión:** 1.0

---

## ✅ Checklist Rápido

Antes de empezar:
- [ ] Tengo lista de productos con cantidades actuales
- [ ] Tengo costos y precios de cada producto
- [ ] Los productos existen en el catálogo
- [ ] Tengo backup de la base de datos

Durante la carga:
- [ ] Creé la nueva carga con comentarios
- [ ] Agregué todos los productos necesarios
- [ ] Verifiqué cantidades y precios
- [ ] Revisé los totales

Después de finalizar:
- [ ] Confirmé que la carga aparece como "Finalizada"
- [ ] Verifiqué que se crearon los lotes
- [ ] Revisé que el inventario es correcto
- [ ] Hice una venta de prueba (opcional)

---

**¡Éxito con tu migración!** 🎉
