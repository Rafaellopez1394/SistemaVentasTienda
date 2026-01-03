# 🧪 KIT DE PRUEBAS FINKOK - GUÍA RÁPIDA

**Fecha:** 1 de enero de 2026  
**Sistema:** VentasWeb - Facturación Electrónica  
**Fuente:** https://www.finkok.com/kit-pruebas.html

---

## 📦 CONTENIDO DEL KIT DE PRUEBAS

### **1. Datos del Emisor de Prueba**
```
RFC:            EKU9003173C9
Razón Social:   ESCUELA KEMPER URGATE
Régimen Fiscal: 601 - General de Ley Personas Morales
Código Postal:  26015
Dirección:      PROLONGACIÓN MONTECARLO 120
Colonia:        HORNOS INSURGENTES
Municipio:      PIEDRAS NEGRAS
Estado:         COAHUILA
País:           MÉXICO
```

### **2. Certificados Digitales (CSD)**
```
Archivo CER:    EKU9003173C9.cer
Archivo KEY:    EKU9003173C9.key
Contraseña:     12345678a
No. Certificado: 30001000000400002434
Vigencia:       02/04/2019 al 02/04/2023
```

⚠️ **NOTA:** Aunque el certificado está técnicamente vencido, Finkok en modo DEMO acepta certificados vencidos para pruebas.

### **3. Credenciales PAC Finkok (Demo)**
```
Usuario:        cfdi@facturacionmoderna.com
Password:       2y4e9w8u
URL Timbrado:   https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl
URL Cancelación: https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl
URL Consulta:   https://demo-facturacion.finkok.com/servicios/soap/utilities.wsdl
Modo:           DEMO (no producción)
```

---

## 🚀 PASOS PARA CONFIGURAR

### **PASO 1: Ejecutar Script SQL (2 minutos)**

1. Abrir **SQL Server Management Studio**
2. Conectar a `localhost\SQLEXPRESS`
3. Abrir el archivo: `027_CONFIGURAR_KIT_PRUEBAS_FINKOK.sql`
4. Ejecutar el script (F5)

**¿Qué hace el script?**
- ✅ Actualiza el RFC a EKU9003173C9
- ✅ Configura razón social y domicilio de prueba
- ✅ Configura credenciales de Finkok DEMO
- ✅ Verifica que la tabla de certificados existe

---

### **PASO 2: Descargar Certificados (3 minutos)**

**Opción A: Desde el sitio oficial**
1. Ir a: https://www.finkok.com/kit-pruebas.html
2. Descargar los archivos:
   - `EKU9003173C9.cer`
   - `EKU9003173C9.key`

**Opción B: Usar certificados incluidos**
Los certificados están en formato Base64 en este mismo directorio:
- Ver archivo: `EKU9003173C9_CERTIFICADOS.txt`

---

### **PASO 3: Cargar Certificado en el Sistema (2 minutos)**

1. **Iniciar la aplicación web:**
   ```
   http://localhost/VentasWeb
   ```

2. **Ir al módulo de certificados:**
   ```
   Menú → Administración → Certificados Digitales
   O directamente: http://localhost/CertificadoDigital
   ```

3. **Subir certificado:**
   - Click en "Nuevo Certificado"
   - **Nombre:** Kit Pruebas Finkok
   - **Archivo .CER:** Seleccionar `EKU9003173C9.cer`
   - **Archivo .KEY:** Seleccionar `EKU9003173C9.key`
   - **Contraseña:** `12345678a`
   - ✅ Marcar "Es predeterminado"
   - Click en "Guardar"

4. **Verificar:**
   - Debe aparecer en la lista
   - RFC: EKU9003173C9
   - Estado: VENCIDO (pero funciona en demo)

---

## 🧪 PROBAR LA FACTURACIÓN

### **Hacer una Venta de Prueba**

1. **Ir al POS:**
   ```
   http://localhost/VentaPOS
   ```

2. **Crear venta:**
   - Agregar productos
   - ✅ Marcar "Requiere Factura"
   - Completar venta

3. **Generar factura:**
   - Se abre modal automáticamente
   - **RFC Cliente:** XAXX010101000 (público en general)
   - **Email:** prueba@test.com
   - **Uso CFDI:** G03 - Gastos en general
   - Click en "Generar Factura"

4. **Resultado esperado:**
   ```
   ✅ XML generado
   ✅ Timbrado con Finkok DEMO
   ✅ UUID obtenido (36 caracteres)
   ✅ PDF descargable
   ```

---

## 📋 DATOS DE CLIENTES DE PRUEBA

Puedes usar estos RFCs para hacer facturas de prueba:

| RFC | Razón Social | Uso CFDI Recomendado |
|-----|--------------|----------------------|
| `XAXX010101000` | PÚBLICO EN GENERAL | G03 - Gastos en general |
| `XEXX010101000` | EXTRANJERO | G03 - Gastos en general |
| `EKU9003173C9` | ESCUELA KEMPER URGATE | G01 - Adquisición de mercancías |
| `AAA010101AAA` | EMPRESA DE PRUEBA 1 | G01 - Adquisición de mercancías |
| `BBB010101BBB` | EMPRESA DE PRUEBA 2 | G02 - Devoluciones |

---

## ⚠️ LIMITACIONES DEL MODO DEMO

### **✅ LO QUE SÍ PUEDES HACER:**
- ✅ Generar facturas completas (CFDI 4.0)
- ✅ Obtener UUID válido de Finkok
- ✅ Descargar XML y PDF
- ✅ Probar todo el flujo completo
- ✅ Ver cómo funciona el timbrado
- ✅ Validar la estructura del XML

### **❌ LO QUE NO PUEDES HACER:**
- ❌ Las facturas NO son válidas ante el SAT
- ❌ NO se pueden usar para deducir impuestos
- ❌ NO se pueden enviar a clientes reales
- ❌ NO se pueden cancelar oficialmente
- ❌ NO aparecen en el portal del SAT

---

## 🔄 CAMBIAR A PRODUCCIÓN (Cuando estés listo)

### **1. Obtener Certificados Reales del SAT**
- Portal: https://sat.gob.mx
- Con tu e.firma o FIEL
- Descargar tu propio CSD (.cer y .key)

### **2. Contratar Servicio Finkok**
- Web: https://www.finkok.com
- Paquetes desde 50 timbres
- Costo: ~$1.50 - $2.00 MXN por factura
- Obtener credenciales de PRODUCCIÓN

### **3. Actualizar Configuración**
```sql
-- Actualizar RFC real
UPDATE Configuracion
SET RFC = 'TU_RFC_REAL',
    RazonSocial = 'TU RAZON SOCIAL'
WHERE ConfigID = 1

-- Activar modo producción
UPDATE ConfiguracionPAC
SET EsProduccion = 1,
    Usuario = 'tu_usuario_produccion',
    Password = 'tu_password_produccion',
    UrlTimbrado = 'https://facturacion.finkok.com/servicios/soap/stamp.wsdl',
    UrlCancelacion = 'https://facturacion.finkok.com/servicios/soap/cancel.wsdl',
    UrlConsulta = 'https://facturacion.finkok.com/servicios/soap/utilities.wsdl'
WHERE ConfigID = 1
```

### **4. Cargar tu Certificado Real**
- Ir a: Certificados Digitales
- Subir tu .cer y .key propios
- Marcar como predeterminado

---

## 📚 RECURSOS ADICIONALES

### **Documentación Finkok:**
- Sitio oficial: https://www.finkok.com
- Kit de pruebas: https://www.finkok.com/kit-pruebas.html
- Documentación API: https://wiki.finkok.com

### **Documentación SAT:**
- Portal: https://sat.gob.mx
- Factura Electrónica: https://www.sat.gob.mx/consultas/71823/complemento-de-pago
- Certificados: https://www.sat.gob.mx/tramites/operacion/28753/obten-tu-certificado-de-sello-digital

### **Validación de Facturas:**
- Validador SAT: https://verificacfdi.facturaelectronica.sat.gob.mx

---

## ✅ CHECKLIST DE VERIFICACIÓN

Antes de usar en producción, verificar:

- [ ] Script SQL ejecutado correctamente
- [ ] Certificado de prueba cargado
- [ ] RFC configurado (EKU9003173C9)
- [ ] Credenciales PAC configuradas
- [ ] Venta de prueba realizada
- [ ] Factura generada exitosamente
- [ ] XML descargado y revisado
- [ ] PDF descargado y revisado
- [ ] UUID obtenido correctamente

**¿Todo OK?** 🎉 ¡Tu sistema está listo para facturar en modo prueba!

---

## 🆘 PROBLEMAS COMUNES

### **Error: "No se encuentra certificado"**
**Solución:** Cargar el certificado desde /CertificadoDigital y marcarlo como predeterminado

### **Error: "Usuario o password incorrecto"**
**Solución:** Verificar en ConfiguracionPAC que estén las credenciales de demo:
- Usuario: cfdi@facturacionmoderna.com
- Password: 2y4e9w8u

### **Error: "Certificado vencido"**
**Solución:** Normal en modo DEMO. Finkok acepta certificados vencidos para pruebas.

### **Error: "RFC no coincide"**
**Solución:** Asegurar que en Configuracion el RFC sea: EKU9003173C9

---

**ÚLTIMA ACTUALIZACIÓN:** 2026-01-01  
**ESTADO:** ✅ Sistema configurado y listo para pruebas
