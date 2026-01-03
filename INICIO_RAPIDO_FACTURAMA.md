# 🚀 INICIO RÁPIDO - Facturar con Facturama en 5 Minutos

## ⏱️ Tiempo Total: 5 minutos

---

## Paso 1: Ejecutar Script SQL (2 minutos)

### Opción A: Desde SQL Server Management Studio

1. Abrir **SQL Server Management Studio**
2. Conectar a tu servidor
3. Abrir archivo:
   ```
   c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server\029_CONFIGURAR_FACTURAMA.sql
   ```
4. Presionar **F5** para ejecutar
5. Leer los mensajes de confirmación

### Opción B: Desde PowerShell

```powershell
# Copiar y pegar este comando:
sqlcmd -S .\SQLEXPRESS -d DB_TIENDA -E -i "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server\029_CONFIGURAR_FACTURAMA.sql"
```

---

## Paso 2: Verificar Configuración (1 minuto)

Ejecutar esta consulta:

```sql
USE DB_TIENDA
GO

SELECT 
    ProveedorPAC AS 'Proveedor',
    CASE 
        WHEN EsProduccion = 1 THEN '⚠️ PRODUCCIÓN' 
        ELSE '🧪 SANDBOX (Pruebas)' 
    END AS 'Modo',
    Usuario,
    UrlTimbrado AS 'URL'
FROM ConfiguracionPAC
WHERE ConfigID = 1
```

**Resultado esperado**:
```
Proveedor: Facturama
Modo: 🧪 SANDBOX (Pruebas)
Usuario: pruebas
URL: https://apisandbox.facturama.mx/cfdi
```

✅ Si ves esto, ¡ya está configurado!

---

## Paso 3: Probar Facturación (2 minutos)

### 1. Abrir POS
```
http://localhost:50772/VentaPOS
```

### 2. Hacer una venta de prueba

**Productos sugeridos** (ya están en tu sistema):
- CAMARON CRISTAL U10 1 KG
- CAMARON EMPACADO U12 1 KG
- Cualquier otro producto

**Cantidad**: 1
**Precio**: El que tenga por defecto

### 3. Marcar "Requiere Factura"
✅ Activar checkbox: **"Requiere Factura"**

### 4. Datos del Cliente
Usar este RFC de prueba:

```
RFC: XAXX010101000
Nombre: PUBLICO EN GENERAL
Email: prueba@test.com
```

(Estos datos ya están en tu sistema si seguiste las guías anteriores)

### 5. Completar Venta
Clic en: **"Completar Venta"**

### 6. Generar Factura
Clic en: **"Generar Factura"**

---

## ✅ Resultado Esperado

Deberías ver un mensaje como:

```
✅ Factura generada exitosamente
UUID: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
Folio: A-123
```

Y deberías poder:
- ✅ Ver el PDF de la factura
- ✅ Descargar el XML
- ✅ Ver el UUID (identificador único SAT)

---

## ⚠️ IMPORTANTE: Modo Sandbox

### ¿Qué es Sandbox?
Es el **ambiente de pruebas** de Facturama:
- ✅ **GRATIS** e **ILIMITADO**
- ✅ Pruebas sin riesgo
- ✅ No consume timbres reales
- ⚠️ Las facturas **NO** son válidas ante el SAT
- ⚠️ Solo para desarrollo y pruebas

### Ventajas
- Puedes generar 1000 facturas de prueba
- No necesitas certificados reales
- No gastas dinero
- Perfecto para familiarizarte con el sistema

### ¿Cuándo cambiar a Producción?
Cuando necesites emitir facturas **REALES** que sean válidas ante el SAT.

---

## 🏭 Para Cambiar a Producción

Ver archivo: **`CONFIGURAR_FACTURAMA.md`** (sección "MODO 2: Producción")

**Resumen**:
1. Registrarte en Facturama (2 min) - GRATIS
2. Cargar tu certificado SAT (3 min)
3. Actualizar credenciales en BD (2 min)
4. Actualizar RFC de tu empresa (1 min)
5. ¡Facturar con timbres reales! (Plan FREE: 50/mes gratis)

**Tiempo total**: 10 minutos

---

## 🔧 Si Algo Sale Mal

### Error: "No se pudo conectar al PAC"
```sql
-- Verificar configuración:
SELECT * FROM ConfiguracionPAC WHERE ConfigID = 1

-- Debe tener:
-- ProveedorPAC = 'Facturama'
-- Usuario = 'pruebas'
-- Password = 'pruebas2011'
```

### Error: "Certificado no encontrado"
⚠️ En Sandbox NO necesitas certificado. Si ves este error:
1. Verificar que `EsProduccion = 0` en ConfiguracionPAC
2. Reiniciar IIS Express
3. Intentar de nuevo

### Error: "RFC inválido"
Usar RFC de prueba: **XAXX010101000**

### Error: "Sistema no compila"
```powershell
# Recompilar:
cd "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
msbuild VentasWeb.sln /t:Rebuild /p:Configuration=Release
```

### ¿Nada funciona?
Ver archivo completo: **`CONFIGURAR_FACTURAMA.md`**

---

## 📊 Comparativa: Finkok vs Facturama

| Concepto | Finkok | Facturama |
|----------|--------|-----------|
| **50 facturas** | $90 MXN | **GRATIS** ⭐ |
| **100 timbres** | $150-200 | $70-80 |
| **Caducidad** | 1-2 años | **NUNCA** ♾️ |
| **Mensualidad** | Variable | $0 |
| **API** | SOAP (complejo) | REST (fácil) |
| **Sandbox** | Limitado | Ilimitado |

**Ahorro**: ~50% en costos

---

## 🎯 Checklist Rápido

```
[ ] 1. Ejecutar 029_CONFIGURAR_FACTURAMA.sql
[ ] 2. Verificar: SELECT * FROM ConfiguracionPAC
[ ] 3. Abrir POS: http://localhost:50772/VentaPOS
[ ] 4. Hacer venta + "Requiere Factura"
[ ] 5. RFC: XAXX010101000
[ ] 6. Generar factura
[ ] 7. Ver UUID en pantalla
[ ] 8. Descargar PDF/XML
```

**Tiempo**: 5 minutos

---

## 💡 Consejos

### Para Pruebas
- Usa siempre RFC: **XAXX010101000** (Público en General)
- Puedes generar todas las facturas que quieras
- No hay límite en Sandbox

### Para Producción
- Necesitas certificado SAT vigente
- RFC debe ser el real de tu empresa
- Plan FREE: 50 facturas/mes gratis
- Timbres adicionales: $0.70-1.00 c/u
- **Timbres NUNCA caducan** ♾️

---

## 📞 Ayuda

### Documentación Completa
- **`CONFIGURAR_FACTURAMA.md`**: Guía detallada
- **`INTEGRACION_FACTURAMA_COMPLETADA.md`**: Resumen técnico

### Soporte Facturama
- Email: soporte@facturama.mx
- Tel: 01 800 8366 846
- Docs: https://www.facturama.mx/api

---

## 🎉 ¡Listo!

Ahora tu sistema puede:
- ✅ Facturar con Facturama (más barato)
- ✅ Cambiar entre Finkok y Facturama
- ✅ Usar timbres que NO caducan
- ✅ Aprovechar plan FREE (50/mes)

**Siguiente paso**: Ejecutar el script SQL y hacer tu primera factura de prueba.

---

**⏱️ Tiempo total**: 5 minutos
**💰 Costo Sandbox**: $0 MXN (gratis ilimitado)
**📊 Plan FREE Producción**: 50 facturas/mes gratis
