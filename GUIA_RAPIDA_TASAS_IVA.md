# 🏪 Guía Rápida: Configurar IVA en Productos

## ❓ ¿Por qué todos mis productos tienen IVA del 16%?

Porque el sistema se configuró inicialmente con la tasa general. Pero **ya tienes** el soporte para múltiples tasas, solo necesitas configurarlo.

---

## ✅ Solución en 3 Pasos

### 📋 **Paso 1: Ejecutar Script de Configuración**

**Opción A - Usando PowerShell (Recomendado):**
```powershell
cd "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\scripts"
.\Configurar-TasasIVA.ps1
```

**Opción B - SQL Server Management Studio:**
1. Abrir SQL Server Management Studio
2. Conectar a tu servidor
3. Abrir: `Utilidad\SQL Server\021_CONFIGURAR_TASAS_IVA.sql`
4. Presionar **F5** para ejecutar

---

### 🔧 **Paso 2: Configurar Productos**

Ve a la pantalla de productos en el sistema web:

1. **Navega:** Mantenedor → Productos
2. **Edita** cada producto
3. **Selecciona** la tasa de IVA correcta:

#### 🟢 IVA 16% - Tasa General
```
✓ Refrescos, jugos
✓ Dulces, chocolates
✓ Galletas, botanas
✓ Productos de limpieza
✓ Electrónicos
✓ Ropa
```

#### 🔵 IVA 0% - Tasa Cero
```
✓ Pan y tortillas
✓ Leche
✓ Huevos
✓ Carne, pollo, pescado
✓ Frutas y verduras frescas
✓ Frijol, arroz, harinas
✓ Medicinas
```

#### ⚪ Exento de IVA
```
✓ Libros
✓ Periódicos
✓ Revistas
```

---

### ✅ **Paso 3: Verificar**

Realiza una venta de prueba con productos de diferentes tasas:

```
Ejemplo:
- Coca Cola 600ml    →  IVA 16%
- Pan Integral       →  IVA 0%
- Revista            →  Exento

El ticket debe mostrar el desglose correcto de IVA
```

---

## 📊 ¿Cómo Afecta Esto?

### Antes (Incorrecto):
```
Pan Integral          $35.00
IVA (16%):            $5.60   ❌ INCORRECTO
Total:                $40.60
```

### Después (Correcto):
```
Pan Integral          $35.00
IVA (0%):             $0.00   ✅ CORRECTO
Total:                $35.00
```

---

## 🎯 Beneficios Inmediatos

✅ **Cumplimiento Fiscal** - Reportes de IVA correctos  
✅ **Facturación Correcta** - CFDI 4.0 con tasas exactas  
✅ **Precios Competitivos** - Productos básicos más baratos  
✅ **Contabilidad Precisa** - Pólizas con IVA desglosado  

---

## 🆘 Ayuda Rápida

### "¿Cómo sé qué IVA lleva cada producto?"

**Regla General:**
- **Alimentos básicos y medicinas** → IVA 0%
- **Todo lo demás** → IVA 16%
- **Libros/revistas** → Exento

### "¿Puedo cambiar varios productos a la vez?"

Sí, usando SQL:

```sql
-- Cambiar toda una categoría a IVA 0%
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE CategoriaID = (SELECT CategoriaID FROM CATEGORIA WHERE Descripcion = 'Frutas');
```

### "¿Qué pasa con las ventas anteriores?"

Las ventas anteriores mantienen el IVA con el que se registraron (16%).  
Los cambios solo aplican para ventas **nuevas**.

---

## 📚 Documentación Completa

Para más detalles, consulta:
- **CONFIGURACION_TASAS_IVA.md** - Guía completa
- **021_CONFIGURAR_TASAS_IVA.sql** - Script SQL

---

## 🔍 Verificación Rápida

```sql
-- Ver cuántos productos tienes por tasa de IVA
SELECT 
    t.Descripcion AS TasaIVA,
    COUNT(*) AS Productos
FROM PRODUCTO p
INNER JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID
WHERE p.Activo = 1
GROUP BY t.Descripcion;
```

**Resultado esperado:**
```
TasaIVA                  Productos
----------------------------------
IVA 16% - Tasa General   XX
IVA 0% - Tasa Cero       XX
Exento de IVA            XX
```

---

## 💡 Tip Final

**Configura primero los productos más vendidos.**  
No necesitas configurar todo de una vez. Empieza por:
1. Productos básicos (pan, leche) → IVA 0%
2. Refrescos y bebidas → IVA 16%
3. El resto según necesites

**El sistema ya está listo. ¡Solo configúralo! 🚀**
