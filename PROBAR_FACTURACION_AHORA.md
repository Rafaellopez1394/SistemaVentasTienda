# 🚀 PROBAR FACTURACIÓN - GUÍA RÁPIDA

**Sistema iniciado en:** http://localhost:50772  
**Fecha:** 1 de enero de 2026

---

## ⚡ PASOS RÁPIDOS (3 minutos)

### **1. Descargar Certificados de Finkok** (1 min)
🔗 https://www.finkok.com/kit-pruebas.html

Buscar y descargar:
- ✅ `EKU9003173C9.cer`
- ✅ `EKU9003173C9.key`
- 🔑 Contraseña: `12345678a`

---

### **2. Cargar Certificado** (1 min)

#### Ir al módulo:
```
http://localhost:50772/CertificadoDigital
```

O desde el menú:
```
Administración → Certificados Digitales
```

#### Subir archivos:
1. Click en **"Nuevo Certificado"** o **"Cargar Certificado"**
2. Llenar formulario:
   - **Nombre:** Kit Pruebas Finkok
   - **RFC:** EKU9003173C9 (se auto-detecta)
   - **Razón Social:** (se auto-detecta del certificado)
   - **Archivo .CER:** Seleccionar `EKU9003173C9.cer`
   - **Archivo .KEY:** Seleccionar `EKU9003173C9.key`
   - **Contraseña:** `12345678a`
   - ✅ **Es Predeterminado:** Marcar
3. Click en **"Guardar"** o **"Cargar"**

#### Verificar:
- Debe aparecer en la lista
- Estado: VENCIDO (⚠️ no importa, funciona en DEMO)
- RFC: EKU9003173C9

---

### **3. Hacer Venta de Prueba** (1 min)

#### Ir al POS:
```
http://localhost:50772/VentaPOS
```

O desde el menú:
```
Ventas → Punto de Venta (POS)
```

#### Crear venta:
1. Buscar producto (ej: "CAMARON CHICO")
2. Agregar al carrito
3. ✅ **Marcar:** "Requiere Factura"
4. Click en **"Procesar Venta"** o **"Finalizar Venta"**

---

### **4. Generar Factura** (30 seg)

Se abre modal automáticamente con el formulario:

#### Datos del cliente:
- **RFC:** `XAXX010101000` (público en general para pruebas)
- **Nombre/Razón Social:** PÚBLICO EN GENERAL
- **Email:** `prueba@test.com`
- **Uso CFDI:** `G03 - Gastos en general`
- **Forma de Pago:** `01 - Efectivo` (o la que usaste)
- **Método de Pago:** `PUE - Pago en una sola exhibición`

#### Generar:
Click en **"Generar Factura"**

#### Resultado esperado:
```
✅ Factura generada exitosamente
✅ UUID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
✅ Archivos disponibles:
   - XML descargable
   - PDF descargable
```

---

## 📋 DATOS DE REFERENCIA RÁPIDA

### **Kit de Pruebas Finkok:**
```yaml
RFC Emisor:         EKU9003173C9
Razón Social:       ESCUELA KEMPER URGATE
Certificado:        EKU9003173C9.cer / EKU9003173C9.key
Contraseña KEY:     12345678a
```

### **Clientes de Prueba:**
```yaml
RFC Cliente:        XAXX010101000
Nombre:             PÚBLICO EN GENERAL
Uso CFDI:           G03 - Gastos en general
Email:              prueba@test.com
```

### **Configuración PAC (ya configurada):**
```yaml
Proveedor:          Finkok
Modo:               DEMO (pruebas)
Usuario:            cfdi@facturacionmoderna.com
Password:           2y4e9w8u
```

---

## ✅ CHECKLIST DE VERIFICACIÓN

Antes de generar la factura:

- [ ] Certificado descargado de Finkok
- [ ] Certificado cargado en /CertificadoDigital
- [ ] Certificado marcado como predeterminado
- [ ] Venta realizada con "Requiere Factura" marcado
- [ ] Datos del cliente completos en el modal

---

## 🎯 SI ALGO FALLA

### **Error: "No se encuentra certificado activo"**
**Solución:** 
1. Ir a /CertificadoDigital
2. Verificar que el certificado esté en la lista
3. Verificar que tenga palomita en "Activo" y "Predeterminado"

### **Error: "RFC no coincide con el certificado"**
**Solución:** 
Ejecutar este SQL cuando SQL Server esté disponible:
```sql
UPDATE Configuracion 
SET RFC = 'EKU9003173C9' 
WHERE ConfigID = 1
```

### **Error: "No se puede conectar con el PAC"**
**Solución:** 
Verificar conexión a internet. Finkok DEMO requiere conexión.

### **Error: "Certificado vencido"**
**Solución:** 
⚠️ Es normal. Finkok en modo DEMO acepta certificados vencidos.

---

## 🔄 FLUJO COMPLETO EN RESUMEN

```
1. Descargar certificados
   ↓
2. Cargar en /CertificadoDigital
   ↓
3. Ir al POS /VentaPOS
   ↓
4. Hacer venta + "Requiere Factura"
   ↓
5. Llenar datos cliente en modal
   ↓
6. Click "Generar Factura"
   ↓
7. ✅ Descargar XML y PDF
```

---

## 📱 ENLACES RÁPIDOS

| Módulo | URL |
|--------|-----|
| **Inicio** | http://localhost:50772 |
| **Certificados** | http://localhost:50772/CertificadoDigital |
| **POS** | http://localhost:50772/VentaPOS |
| **Facturas** | http://localhost:50772/Factura |
| **Configuración** | http://localhost:50772/Configuracion |

---

## 🆘 NECESITAS AYUDA?

**Documentación completa:** Ver [KIT_PRUEBAS_FINKOK.md](KIT_PRUEBAS_FINKOK.md)

**Descargar certificados:** https://www.finkok.com/kit-pruebas.html

**Soporte Finkok:** https://wiki.finkok.com

---

**¡LISTO!** 🎉 Todo preparado para facturar en modo prueba.
