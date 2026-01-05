# 📜 Cómo Subir tu Certificado del SAT a Facturama

## 🎯 Objetivo
Cargar tu certificado digital (archivos .cer y .key) en tu cuenta de Facturama para poder emitir facturas válidas.

---

## 📋 Requisitos Previos

Debes tener estos archivos del SAT:
- ✅ `tu_rfc.cer` - Certificado de Sello Digital
- ✅ `tu_rfc.key` - Llave Privada
- ✅ **Contraseña de la llave privada** (la que te dio el SAT)

> 💡 **¿No los tienes?** Descárgalos desde el portal del SAT:
> https://www.sat.gob.mx/aplicacion/16660/descarga-masiva-de-archivos-xml

---

## 🚀 Pasos para Subir el Certificado

### 1. Ingresar al Portal de Facturama
```
🌐 https://www.facturama.mx/login
```
- Usuario: **mercadomar** (el que configuraste)
- Contraseña: [tu contraseña]

### 2. Ir a Configuración de Certificados
1. Una vez dentro, busca el menú lateral
2. Clic en **"Configuración"** o **"Perfil Fiscal"**
3. Clic en **"Certificados"** o **"Certificado Digital"**

### 3. Subir los Archivos
1. Clic en **"Agregar Certificado"** o **"Subir Certificado"**
2. **Seleccionar archivo .cer**:
   - Clic en "Examinar" o "Seleccionar"
   - Busca tu archivo: `XAXX010101000.cer` (tu RFC)
   - Seleccionar y Abrir

3. **Seleccionar archivo .key**:
   - Clic en "Examinar" o "Seleccionar"
   - Busca tu archivo: `XAXX010101000.key` (tu RFC)
   - Seleccionar y Abrir

4. **Ingresar contraseña**:
   - Escribe la contraseña que te dio el SAT
   - ⚠️ **IMPORTANTE**: Esta NO es tu contraseña de Facturama
   - Es la contraseña de tu llave privada (.key)

5. Clic en **"Guardar"** o **"Validar"**

### 4. Verificar que se Cargó Correctamente
Deberías ver:
- ✅ **RFC**: Tu RFC
- ✅ **Razón Social**: Nombre de tu empresa
- ✅ **Válido desde**: Fecha inicio de vigencia
- ✅ **Válido hasta**: Fecha fin de vigencia (4 años)
- ✅ **Estado**: ACTIVO o VIGENTE

---

## ⚠️ Problemas Comunes

### Error: "Contraseña Incorrecta"
- ✅ Verifica que estés usando la contraseña de la LLAVE, no de Facturama
- ✅ La contraseña la obtuviste al generar el certificado en el SAT

### Error: "Certificado Vencido"
- ✅ Los certificados del SAT vencen cada 4 años
- ✅ Necesitas renovarlo en el portal del SAT
- ✅ URL: https://www.sat.gob.mx/tramites/16703/obten-tu-certificado-de-e.firma-portable

### Error: "RFC no coincide"
- ✅ El RFC del certificado debe ser el mismo que el de tu cuenta Facturama
- ✅ Verifica que subiste el archivo correcto

---

## ✅ Próximos Pasos

Una vez que el certificado esté subido:

1. **Probar una Factura de Prueba**:
   - Genera una venta pequeña en tu POS
   - Marca "Requiere Factura"
   - Completa datos del cliente
   - Generar factura

2. **Verificar en Facturama**:
   - Entra a tu panel de Facturama
   - Ve a "Facturas" o "CFDIs"
   - Deberías ver tu factura generada

3. **Descargar XML y PDF**:
   - Facturama genera automáticamente el XML timbrado
   - También genera un PDF presentable

---

## 📞 Soporte

Si tienes problemas:
- 💬 Chat de Facturama: https://www.facturama.mx/
- 📧 Email: soporte@facturama.mx
- 📞 Teléfono: (Consulta en su sitio web)

---

## 🎯 ¿Ya subiste el certificado?

Si ya está listo, vuelve a tu sistema y prueba generar una factura desde el POS.

**Comando para verificar configuración**:
```sql
SELECT 
    ProveedorPAC,
    CASE WHEN EsProduccion = 1 THEN '🔴 PRODUCCIÓN' ELSE '🟡 Sandbox' END AS Modo,
    Usuario,
    CASE WHEN Activo = 1 THEN '✅ ACTIVO' ELSE '❌ Inactivo' END AS Estado
FROM ConfiguracionPAC;
```

**Tu configuración actual**:
- ✅ Proveedor: **Facturama**
- ✅ Modo: **PRODUCCIÓN**
- ✅ Usuario: **mercadomar**
- ✅ Estado: **ACTIVO**

¡Todo listo para facturar! 🚀
