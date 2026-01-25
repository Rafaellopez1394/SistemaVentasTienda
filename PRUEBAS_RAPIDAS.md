# ✅ SISTEMA LISTO PARA PROBAR

## 🎯 Estado Actual

✅ Visual Studio abierto con VentasWeb.sln  
✅ Stored Procedures creados (2 de 3, el 3° tiene workaround)  
✅ Compilación exitosa (0 errores)  
✅ FiscalAPI configurado como único PAC  
✅ Reportes integrados en menú principal  

---

## 🚀 PASO 1: Iniciar el Sistema

**En Visual Studio:**
1. Presiona **F5** (o click en el botón ▶ verde)
2. Espera que compile y abra el navegador
3. Si pide puerto, usa el default (probablemente 5001 o similar)

---

## 🔐 PASO 2: Iniciar Sesión

1. Usuario: **admin** (o tu usuario administrativo)
2. Contraseña: tu contraseña
3. Click en **Iniciar Sesión**

---

## 📊 PASO 3: Acceder a Reportes

**En el menú lateral izquierdo:**
1. Busca el nuevo item: **📊 Reportes Avanzados**
2. Haz click para expandir
3. Verás 4 opciones:
   - 📈 Utilidad por Producto
   - 💰 Estado de Resultados (P&L)
   - 💳 Recuperación de Crédito
   - 👥 Cartera de Clientes

---

## 🧪 PRUEBA #1: Utilidad por Producto

**Objetivo:** Saber si el camarón 21-25 es rentable

1. Click en: **Utilidad por Producto**
2. Selecciona fechas (ejemplo: último mes)
3. Click: **Generar Reporte**
4. Busca en la tabla: "CAMARON 21-25" o similar
5. Revisa columnas:
   - **Ganancia Neta** (¿es positiva?)
   - **Margen %** (¿es ≥20%?)
   - **Rentabilidad** (ALTA/MEDIA/BAJA/PÉRDIDA)

**✅ Resultado esperado:**
- Tabla con todos tus productos
- Cada producto con su margen real
- Color verde = rentable, rojo = pérdida

---

## 🧪 PRUEBA #2: Estado de Resultados

**Objetivo:** Saber si el negocio es viable

1. Click en: **Estado de Resultados (P&L)**
2. Selecciona mes completo
3. Click: **Generar**
4. Lee la conclusión automática:
   - ✅ **NEGOCIO RENTABLE** (verde) → Todo bien
   - ⚠️ **PÉRDIDAS** (rojo) → Revisar gastos

**✅ Resultado esperado:**
- Tabla tipo contabilidad:
  ```
  Ventas:           $100,000
  - Costo Ventas:   $ 60,000
  - Gastos:         $ 15,000
  = Utilidad Neta:  $ 25,000
  ```
- Conclusión automática con recomendaciones

---

## 🧪 PRUEBA #3: Recuperación de Crédito

**Objetivo:** Saber si estoy recuperando el crédito

1. Click en: **Recuperación de Crédito**
2. Selecciona últimos 30 días
3. Click: **Generar**
4. Revisa:
   - **% Recuperación** (debe ser ≥80% = verde)
   - Gráfica de tendencias
   - Días con baja recuperación (alertas)

**✅ Resultado esperado:**
- Tabla día por día con créditos vs cobros
- Gráfica visual de tendencias
- % recuperación con color:
  - Verde ≥80% = Excelente
  - Amarillo 50-80% = Regular
  - Rojo <50% = Mal

---

## 🧪 PRUEBA #4: Cartera de Clientes

**Objetivo:** Identificar quién me debe y quién está moroso

1. Click en: **Cartera de Clientes**
2. Fecha de corte: Hoy
3. Click: **Consultar Cartera**
4. Revisa:
   - Columna **Estado**: 🟢 AL CORRIENTE, 🔴 MOROSO
   - Columna **Días Vencido**
   - Columna **Saldo**

**✅ Resultado esperado:**
- Solo clientes con saldo > 0
- Ordenados por saldo (mayor primero)
- Estados claros:
  - 🟢 AL CORRIENTE (0-15 días)
  - 🟡 VENCIDO (16-30 días)
  - 🔴 MOROSO (30+ días)

---

## ⚠️ Si algo no funciona

### Error: "No se encontró el procedimiento"
**Solución:** Los SPs no se crearon. Ejecuta:
```powershell
sqlcmd -S localhost -d DB_TIENDA -E -i "CREAR_SP_REPORTES_AVANZADOS_CORREGIDO.sql"
```

### Error: "No hay datos"
**Causa:** No hay ventas/compras en el rango de fechas
**Solución:** Cambia el rango de fechas o crea datos de prueba

### Error: "No se puede conectar a la base de datos"
**Solución:** Revisa Web.config, connectionString debe apuntar a DB_TIENDA

### Menú no aparece
**Causa:** Tu usuario no tiene permisos
**Solución:** Asegúrate de iniciar sesión como ADMIN o EMPLEADO

---

## 📋 Checklist de Prueba Completa

- [ ] Sistema inició correctamente (F5 en Visual Studio)
- [ ] Inicio de sesión exitoso
- [ ] Menú "Reportes Avanzados" visible
- [ ] Reporte 1: Utilidad por Producto funciona
- [ ] Reporte 2: Estado de Resultados genera conclusión
- [ ] Reporte 3: Recuperación de Crédito muestra gráfica
- [ ] Reporte 4: Cartera identifica morosos
- [ ] Colores y badges se ven correctos
- [ ] Tablas son ordenables (DataTables)

---

## 🎯 Caso de Uso Real

**Pregunta de Negocio:**
> "¿Me conviene seguir vendiendo camarón 21-25 o debo cambiar a otra talla?"

**Pasos:**
1. Ve a: Utilidad por Producto
2. Rango: Últimos 3 meses
3. Busca: CAMARON 21-25
4. Compara con: CAMARON 26-30, CAMARON 16-20
5. Decisión:
   - Si margen ≥25% → SEGUIR
   - Si margen <15% → CAMBIAR
   - Si PÉRDIDA → ELIMINAR

---

## 📚 Documentación Extra

Si necesitas más detalles técnicos:
- **SISTEMA_REPORTES_COMPLETADO.md** - Arquitectura completa
- **GUIA_PRUEBAS_REPORTES.md** - Casos de prueba detallados
- **CONFIRMACION_SOLO_FISCALAPI.md** - Cambios de facturación

---

## ✅ TODO LISTO

**Visual Studio ya está abierto.**  
**Presiona F5 y comienza a probar.**

¡El sistema está funcionando! 🎉
