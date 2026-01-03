# 🚀 CHECKLIST: CONFIGURACIÓN PARA PRODUCCIÓN

**Fecha inicio:** 1 de enero de 2026  
**Estado:** En proceso

---

## ✅ PASO 1: CERTIFICADOS DEL SAT

**Trámite:** Certificado de Sello Digital (CSD)  
**Dónde:** https://www.sat.gob.mx/tramites/operacion/28753/obten-tu-certificado-de-sello-digital

**Requisitos:**
- [ ] Tener e.firma (FIEL) vigente
- [ ] Estar al corriente con obligaciones fiscales
- [ ] RFC activo

**Archivos a descargar:**
- [ ] Archivo .CER (certificado público)
- [ ] Archivo .KEY (llave privada)
- [ ] Anotar contraseña de la llave

**Tiempo estimado:** 15-30 minutos  
**Costo:** GRATIS

---

## ✅ PASO 2: CONTRATAR FINKOK PRODUCCIÓN

**Contacto Finkok:**
- Web: https://www.finkok.com/contacto
- Email: ventas@finkok.com
- Tel: +52 (55) 4333-2550

**Información a proporcionar:**
- [ ] RFC de tu empresa
- [ ] Razón social
- [ ] Email de contacto
- [ ] Teléfono

**Paquetes disponibles:**
```
50 timbres   → ~$100 MXN   ($2.00 c/u)
100 timbres  → ~$180 MXN   ($1.80 c/u)
500 timbres  → ~$750 MXN   ($1.50 c/u)
```

**Credenciales que recibirás:**
- [ ] Usuario de producción
- [ ] Password de producción
- [ ] URLs de producción (confirmación)

**Tiempo estimado:** 1 día hábil  
**Costo inicial:** $100-500 MXN

---

## ✅ PASO 3: CONFIGURAR EL SISTEMA

### **3.1 Editar script SQL**

Abrir archivo:
```
Utilidad\SQL Server\028_CONFIGURAR_PRODUCCION.sql
```

**Editar estos datos con tus datos REALES:**

```sql
-- TUS DATOS FISCALES
@RFC_REAL = 'TU_RFC_AQUI'              -- Ej: 'ABC123456XYZ'
@RAZON_SOCIAL = 'TU RAZON SOCIAL'      -- Como aparece en el SAT
@REGIMEN_FISCAL = '612'                -- Tu régimen fiscal
@CP = '12345'                          -- Tu código postal
@CALLE = 'NOMBRE DE TU CALLE'
@NUM_EXT = '123'
@COLONIA = 'TU COLONIA'
@MUNICIPIO = 'TU MUNICIPIO'
@ESTADO = 'TU ESTADO'

-- TUS CREDENCIALES FINKOK
@USUARIO_PROD = 'usuario_que_te_dio_finkok'
@PASSWORD_PROD = 'password_que_te_dio_finkok'
```

**Checklist de edición:**
- [ ] RFC real configurado
- [ ] Razón social configurada
- [ ] Régimen fiscal configurado
- [ ] Dirección fiscal completa
- [ ] Usuario Finkok producción
- [ ] Password Finkok producción

### **3.2 Ejecutar script SQL**

```sql
-- En SQL Server Management Studio:
USE DB_TIENDA
GO
-- Ejecutar: 028_CONFIGURAR_PRODUCCION.sql
```

- [ ] Script ejecutado sin errores
- [ ] Configuración verificada

### **3.3 Cargar certificado en el sistema**

1. Ir a: http://localhost:50772/CertificadoDigital
2. Click en "Nuevo Certificado"
3. Datos:
   - Nombre: Certificado Producción [AÑO]
   - Archivo .CER: [seleccionar tu .cer del SAT]
   - Archivo .KEY: [seleccionar tu .key del SAT]
   - Contraseña: [contraseña de tu llave privada]
   - ✅ Marcar "Es predeterminado"
   - ✅ Marcar "Activo"
4. Guardar

**Checklist:**
- [ ] Certificado cargado
- [ ] Datos del certificado visibles (RFC, vigencia)
- [ ] Marcado como predeterminado
- [ ] Estado: Activo

---

## ✅ PASO 4: PRUEBA FINAL

### **4.1 Hacer venta de prueba**

1. Ir al POS: http://localhost:50772/VentaPOS
2. Agregar un producto de bajo valor (ej: $10.00)
3. ✅ Marcar "Requiere Factura"
4. Completar venta

### **4.2 Generar primera factura REAL**

**Datos del cliente:**
```
RFC: Tu propio RFC o el de un cliente real
Razón Social: Razón social completa
Email: Email válido (recibirá la factura)
Uso CFDI: Seleccionar el correcto (G01, G03, etc.)
```

**Checklist de generación:**
- [ ] Modal se abre correctamente
- [ ] Datos del cliente completos
- [ ] Click en "Generar Factura"
- [ ] ✅ Mensaje de éxito
- [ ] ✅ UUID generado (36 caracteres)
- [ ] ✅ XML descargable
- [ ] ✅ PDF descargable
- [ ] ✅ Email enviado al cliente

### **4.3 Validar en el SAT**

1. Ir a: https://verificacfdi.facturaelectronica.sat.gob.mx
2. Ingresar datos de la factura:
   - RFC Emisor
   - RFC Receptor
   - Total de la factura
   - UUID (Folio Fiscal)
3. Verificar que aparece como **VIGENTE**

**Checklist:**
- [ ] Factura encontrada en el SAT
- [ ] Estado: VIGENTE
- [ ] Datos coinciden

---

## ⚠️ VERIFICACIONES DE SEGURIDAD

Antes de empezar a facturar a clientes:

- [ ] RFC configurado es el correcto
- [ ] Razón social coincide con constancia fiscal
- [ ] Régimen fiscal es el correcto
- [ ] Dirección fiscal es la correcta
- [ ] Certificado vigente (no vencido)
- [ ] Certificado es el de tu empresa
- [ ] Credenciales Finkok de PRODUCCIÓN (no demo)
- [ ] URLs de Finkok son de producción
- [ ] Primera factura de prueba exitosa
- [ ] Primera factura validada en el SAT

---

## 📊 RESUMEN DE COSTOS

| Concepto | Costo | Frecuencia |
|----------|-------|------------|
| Certificado SAT | GRATIS | Cada 4 años |
| Finkok (100 timbres) | ~$180 MXN | Cuando se agoten |
| Costo por factura | $1.50-2.00 | Por factura |
| **TOTAL INICIAL** | **~$180 MXN** | Única vez |

---

## 🆘 SOPORTE

**Finkok:**
- Email: soporte@finkok.com
- Tel: +52 (55) 4333-2550
- Horario: Lun-Vie 9:00-18:00 hrs

**SAT:**
- Tel: 55 627 22 728
- Portal: https://www.sat.gob.mx

---

## ✅ ESTADO FINAL

Una vez completados todos los pasos:

- [ ] Sistema en modo PRODUCCIÓN
- [ ] Primera factura oficial generada
- [ ] Primera factura validada en el SAT
- [ ] **SISTEMA LISTO PARA OPERAR** ✅

---

**Fecha de completado:** _______________  
**Persona responsable:** _______________
