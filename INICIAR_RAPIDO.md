# 🚀 INICIO RÁPIDO - Sistema POS con Reportes

## ✅ Sistema Listo

El sistema está compilado y listo para usar con:
- ✅ **FiscalAPI** como único servicio de facturación
- ✅ **Reportes Avanzados** integrados en el menú
- ✅ 4 reportes implementados
- ✅ 0 errores de compilación

---

## 🎯 OPCIÓN RECOMENDADA: Visual Studio

### Pasos:
1. **Abre Visual Studio 2022**
2. **Archivo → Abrir → Proyecto/Solución**
3. **Selecciona:** `VentasWeb.sln`
4. **Presiona F5** (o botón ▶ verde "Start")
5. **El navegador se abrirá automáticamente**

### Primera Prueba:
1. Inicia sesión
2. Busca en el menú: **📊 Reportes Avanzados**
3. Haz clic para ver los 4 reportes disponibles

---

## 📊 Reportes Disponibles

### 1️⃣ Utilidad por Producto
**Ruta:** Reportes Avanzados → Utilidad por Producto

**Responde:**
- ¿Es rentable el camarón 21-25?
- ¿Qué talla genera más ganancia?
- ¿Cuál es el margen real de cada producto?

**Ejemplo de uso:**
```
Selecciona fechas: 01/01/2024 - 31/01/2024
Click: Generar Reporte
Resultado: Tabla con todos los productos, compras, ventas, ganancia y % margen
```

---

### 2️⃣ Estado de Resultados (P&L)
**Ruta:** Reportes Avanzados → Estado de Resultados

**Responde:**
- ¿Es rentable mi negocio este mes?
- ¿Debo seguir operando o cerrar?
- ¿Cuáles son mis gastos más altos?

**Genera automáticamente:**
- Conclusión: ✅ NEGOCIO RENTABLE o ⚠️ PÉRDIDAS
- Recomendaciones basadas en los números

---

### 3️⃣ Recuperación de Crédito
**Ruta:** Reportes Avanzados → Recuperación de Crédito

**Responde:**
- ¿Estoy recuperando el crédito otorgado?
- ¿Qué días tuve mejor cobranza?
- ¿Cuánto crédito vencido tengo?

**Incluye:**
- Tabla día por día
- Gráfica de tendencias
- % de recuperación por día

---

### 4️⃣ Cartera de Clientes
**Ruta:** Reportes Avanzados → Cartera de Clientes

**Responde:**
- ¿Quiénes me deben dinero?
- ¿Quiénes están morosos (30+ días)?
- ¿Cuánto tengo en créditos vencidos?

**Clasificación automática:**
- 🟢 AL CORRIENTE (0-15 días)
- 🟡 VENCIDO (16-30 días)
- 🔴 MOROSO (30+ días)

---

## 🔧 Si Visual Studio no funciona

### Alternativa: IIS Express Manual

```powershell
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
& "C:\Program Files\IIS Express\iisexpress.exe" /path:$PWD /port:8080
```

Luego abre: http://localhost:8080

---

## ⚠️ IMPORTANTE: Stored Procedures

**ANTES de usar los reportes**, ejecuta este comando una sola vez:

```powershell
sqlcmd -S localhost -d DB_TIENDA -E -i "CREAR_SP_REPORTES_AVANZADOS_CORREGIDO.sql"
```

Esto crea los procedimientos almacenados necesarios para los reportes.

---

## 📚 Documentación Completa

- **SISTEMA_REPORTES_COMPLETADO.md** - Detalles técnicos
- **GUIA_PRUEBAS_REPORTES.md** - Guía paso a paso
- **CONFIRMACION_SOLO_FISCALAPI.md** - Cambios de facturación

---

## ✅ Siguiente Paso

**Abre Visual Studio y presiona F5**

¡El sistema está listo para usarse!
