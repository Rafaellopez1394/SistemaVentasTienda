# 🚀 Guía Rápida: Configurar Facturama

## ✅ Ventajas de Facturama

1. **🆓 Plan FREE**: 50 facturas gratis cada mes
2. **♾️ Sin Caducidad**: Los timbres comprados NUNCA expiran
3. **💰 Precio Bajo**: $0.70 - $1.00 por timbre adicional
4. **🔌 API REST**: Más fácil de usar que SOAP
5. **📊 Portal Web**: Gestiona tus facturas en línea
6. **🧪 Sandbox**: Prueba gratis sin límite

---

## 🧪 MODO 1: Sandbox (Pruebas) - GRATIS

### Paso 1: Ejecutar Script SQL

```sql
-- En SQL Server, ejecutar:
USE DB_TIENDA
GO
EXEC xp_cmdshell 'type "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server\029_CONFIGURAR_FACTURAMA.sql"'
-- O ejecutar manualmente el archivo
```

### Paso 2: Verificar Configuración

```sql
SELECT * FROM ConfiguracionPAC WHERE ConfigID = 1
-- Debe mostrar:
-- ProveedorPAC: Facturama
-- EsProduccion: 0 (Sandbox)
-- Usuario: pruebas
-- Password: pruebas2011
```

### Paso 3: Probar Facturación

1. Abrir POS: http://localhost:50772/VentaPOS
2. Hacer una venta de prueba
3. Marcar: **"Requiere Factura"**
4. Llenar datos del cliente (RFC: XAXX010101000)
5. Clic en **"Generar Factura"**
6. ✅ Debe aparecer el UUID de Facturama

**⚠️ IMPORTANTE**: Las facturas de Sandbox NO son válidas ante el SAT, solo sirven para probar.

---

## 🏭 MODO 2: Producción (Real)

### Paso 1: Crear Cuenta Gratis

1. Ir a: https://www.facturama.mx/registro
2. Completar formulario:
   - Nombre completo
   - Email (será tu usuario)
   - Contraseña
   - Teléfono
3. Confirmar email

**⏱️ Tiempo**: 2 minutos

### Paso 2: Cargar Certificado del SAT

1. Entrar a tu panel: https://www.facturama.mx/login
2. Ir a: **Configuración** → **Certificados**
3. Subir archivos:
   - `archivo.cer` (Certificado)
   - `archivo.key` (Llave privada)
4. Ingresar **contraseña de la llave**
5. Clic en **"Guardar"**

**⏱️ Tiempo**: 3 minutos

### Paso 3: Obtener Credenciales API

1. En tu panel Facturama
2. Ir a: **Configuración** → **API Keys**
3. Copiar:
   - **Usuario**: (tu email)
   - **Contraseña**: (tu contraseña o API Key)

### Paso 4: Actualizar Configuración en el Sistema

```sql
USE DB_TIENDA
GO

UPDATE ConfiguracionPAC
SET 
    EsProduccion = 1,  -- ⚠️ MODO PRODUCCIÓN
    UrlTimbrado = 'https://api.facturama.mx/cfdi',
    UrlCancelacion = 'https://api.facturama.mx/cfdi',
    UrlConsulta = 'https://api.facturama.mx/cfdi',
    Usuario = 'tu_email@ejemplo.com',    -- 🔹 TU EMAIL AQUÍ
    Password = 'tu_contraseña',           -- 🔹 TU CONTRASEÑA AQUÍ
    FechaModificacion = GETDATE()
WHERE ConfigID = 1

-- Verificar
SELECT 
    ProveedorPAC,
    CASE WHEN EsProduccion = 1 THEN '⚠️ PRODUCCIÓN' ELSE 'Sandbox' END AS Modo,
    Usuario
FROM ConfiguracionPAC
WHERE ConfigID = 1
```

### Paso 5: Actualizar Datos de tu Empresa

```sql
-- Verificar que tu RFC y datos fiscales estén correctos
SELECT * FROM Configuracion

-- Si necesitas actualizar:
UPDATE Configuracion
SET 
    RFC = 'TU_RFC_REAL',           -- 🔹 RFC de tu empresa
    RazonSocial = 'TU EMPRESA SA DE CV',
    RegimenFiscal = '612'          -- Ejemplo: Personas Físicas con Actividades Empresariales
WHERE ConfigID = 1
```

**⏱️ Tiempo Total**: 10 minutos

---

## 💰 Planes y Precios

### Plan FREE ⭐ RECOMENDADO PARA EMPEZAR
- **50 facturas gratis cada mes**
- $0 MXN mensualidad
- Perfecto para negocios pequeños
- Incluye portal web completo

### Compra de Timbres Adicionales
- 200 timbres → **$140 MXN** ($0.70 c/u)
- 500 timbres → **$375 MXN** ($0.75 c/u)
- 1000 timbres → **$800 MXN** ($0.80 c/u)
- 5000 timbres → **$3,500 MXN** ($0.70 c/u)

**⚠️ VENTAJA CLAVE**: Los timbres comprados **NUNCA caducan**

### Comparativa con Finkok

| Concepto | Facturama | Finkok |
|----------|-----------|--------|
| 50 facturas | **GRATIS** | $90 MXN |
| 100 timbres | **$70-80 MXN** | $150-200 MXN |
| Caducidad | **NUNCA** | 1-2 años |
| API | REST (fácil) | SOAP (complejo) |
| Mensualidad | $0 | Variable |

**💡 Ahorro con Facturama**: ~50% en costos

---

## 🔧 Solución de Problemas

### Error: "Usuario o contraseña incorrectos"
✅ **Solución**:
1. Verificar credenciales en SQL:
   ```sql
   SELECT Usuario, Password FROM ConfiguracionPAC WHERE ConfigID = 1
   ```
2. Confirmar que sean las mismas del panel Facturama
3. Verificar que no haya espacios extras

### Error: "Certificado no encontrado"
✅ **Solución**:
1. Ir al panel Facturama → Certificados
2. Verificar que el certificado esté activo
3. Revisar fecha de vigencia (no vencido)

### Error: "RFC no coincide con certificado"
✅ **Solución**:
```sql
-- Verificar RFC en sistema
SELECT RFC FROM Configuracion

-- El RFC debe coincidir con el del certificado cargado en Facturama
```

### Facturas de Sandbox no aparecen en SAT
⚠️ **Esto es normal**: Las facturas de Sandbox son solo para pruebas y NO se reportan al SAT.

Para facturas reales, cambiar a modo producción (Paso 4 arriba).

---

## 📊 Dashboard Facturama

Puedes consultar tus facturas en línea:

1. **Producción**: https://www.facturama.mx/login
2. **Sandbox**: https://www.facturama.mx/login (mismas credenciales)

Desde el dashboard puedes:
- Ver facturas emitidas
- Descargar XML y PDF
- Cancelar facturas
- Consultar saldo de timbres
- Ver reportes

---

## 🎯 Resumen de Acción

### Para Empezar AHORA (Sandbox):
```bash
# 1. Ejecutar SQL
029_CONFIGURAR_FACTURAMA.sql

# 2. Abrir POS
http://localhost:50772/VentaPOS

# 3. Hacer venta + "Requiere Factura"

# 4. ¡Listo! Factura generada
```

### Para Producción:
1. ✅ Registrarse en Facturama (2 min)
2. ✅ Cargar certificado SAT (3 min)
3. ✅ Ejecutar SQL con credenciales (2 min)
4. ✅ Actualizar RFC empresa (1 min)
5. ✅ Facturar con timbres reales

**Total**: 8 minutos

---

## 📞 Soporte

- **Documentación**: https://www.facturama.mx/api
- **Soporte Facturama**: soporte@facturama.mx
- **Teléfono**: 01 800 8366 846
- **Portal**: https://ayuda.facturama.mx

---

## ✅ Checklist Final

Antes de facturar en producción:

- [ ] Cuenta Facturama creada
- [ ] Certificado SAT cargado y vigente
- [ ] Credenciales API configuradas en SQL
- [ ] RFC correcto en tabla Configuracion
- [ ] Datos fiscales completos
- [ ] Prueba en Sandbox exitosa
- [ ] Timbres comprados (o usar plan FREE)

---

**🎉 ¡Listo para facturar con Facturama!**

- Plan FREE: 50 facturas/mes gratis
- Timbres sin caducidad
- Precio: $0.70 - $1.00 por factura adicional
- REST API simple y rápida
