# ✅ CHECKLIST FINAL: CAMBIO A PRODUCCIÓN

## 📋 Antes de Cambiar (CRÍTICO)

### A. Obtener Credenciales Reales
- [ ] Acceso a FiscalAPI con credenciales de producción
- [ ] API Key de producción (sk_live_...) copiado en seguro
- [ ] Login en: https://live.fiscalapi.com/dashboard funciona

### B. Certificados CSD
- [ ] Tengo archivo `.cer` (certificado público)
- [ ] Tengo archivo `.key` (llave privada encriptada)
- [ ] Contraseña de la llave privada guardada
- [ ] Certificado VIGENTE en el SAT (consultar portal SAT)
- [ ] Certificado NO EXPIRADO
- [ ] RFC coincide en certificado y empresa

### C. Empresa Registrada en SAT
- [ ] RFC está ACTIVO en SAT
- [ ] Razón Social es correcta
- [ ] Régimen Fiscal es correcto
- [ ] No hay adeudos fiscales (opcional pero recomendado)
- [ ] Puedo acceder a: https://www3.sat.gob.mx

### D. Base de Datos
- [ ] Backup completo de DB_TIENDA
- [ ] Acceso a SQL Server Management Studio
- [ ] Tengo permisos para ejecutar scripts SQL
- [ ] He verificado que estoy en TEST actualmente

### E. Aplicación Web
- [ ] Proyecto compila SIN errores (último Build OK)
- [ ] Tengo acceso a Visual Studio
- [ ] Proyecto VentasWeb.sln abre sin problemas
- [ ] He hecho al menos 1 factura de prueba en TEST

### F. Documentación
- [ ] He leído: GUIA_CAMBIAR_A_PRODUCCION.md
- [ ] He leído: TEST_vs_PRODUCCION_COMPARATIVA.md
- [ ] Tengo guardados los URLs de SAT
- [ ] Tengo contacto de soporte FiscalAPI

---

## 🔧 Ejecución del Cambio

### Paso 1: Respaldar (5 min)
- [ ] He hecho backup de DB_TIENDA completo
  - Ubicación: ___________________________
  - Fecha: ______________________________
- [ ] He exportado ConfiguracionPAC actual
  ```sql
  SELECT * FROM ConfiguracionPAC WHERE ConfigPACID = 1;
  -- Resultado guardado en: _________________
  ```

### Paso 2: Preparar Script SQL (3 min)
- [ ] He abierto CAMBIAR_A_PRODUCCION.sql
- [ ] He localizado la línea con ApiKey
- [ ] He reemplazado 'sk_live_XXXXX' con mi clave real
- [ ] He verificado que NO tiene espacios extras
- [ ] He guardado el archivo

### Paso 3: Ejecutar SQL (2 min)
- [ ] He abierto SQL Server Management Studio
- [ ] He conectado a BD: DB_TIENDA
- [ ] He abierto el script CAMBIAR_A_PRODUCCION.sql (editado)
- [ ] He presionado F5 para ejecutar
- [ ] El resultado muestra "✅ ConfiguracionPAC actualizada"

### Paso 4: Verificar Cambio (2 min)
- [ ] He ejecutado: VERIFICAR_AMBIENTE_FISCALAPI.sql
- [ ] El resultado muestra: "🔴 PRODUCCIÓN"
- [ ] ApiKey comienza con: sk_live_
- [ ] BaseURL es: https://api.fiscalapi.com

### Paso 5: Subir Certificados (5 min)
- [ ] He accedido a: https://live.fiscalapi.com/dashboard
- [ ] He ido a: Tax Files o Certificados
- [ ] He hecho clic: Upload Certificate
- [ ] He seleccionado archivo: GAMA6111156JA.cer
- [ ] He seleccionado archivo: GAMA6111156JA.key
- [ ] He ingresado contraseña de llave correcta
- [ ] He hecho clic: Upload
- [ ] El estado muestra: "Certificate uploaded successfully"
- [ ] El certificado aparece con estado: "VIGENTE" ✓

### Paso 6: Compilar Aplicación (5 min)
- [ ] He abierto Visual Studio
- [ ] He cargado VentasWeb.sln
- [ ] He ejecutado: Build → Clean Solution
- [ ] He ejecutado: Build → Rebuild Solution
- [ ] NO hay errores en la compilación
- [ ] Output muestra: "Build: X succeeded"

### Paso 7: Ejecutar Aplicación (3 min)
- [ ] He presionado F5 en Visual Studio
- [ ] La aplicación se abre en navegador
- [ ] Puedo navegar sin errores
- [ ] No hay errores en consola de desarrollador (F12)

---

## 🧪 Primera Factura de Prueba

### Generación
- [ ] He creado una venta pequeña (1-2 productos)
- [ ] Producto existe en BD
- [ ] Cliente existe en BD
- [ ] Monto es mayor a $0

### Timbrado
- [ ] He hecho clic en: Generar Factura
- [ ] He seleccionado cliente
- [ ] He hecho clic en: Timbrar
- [ ] Respuesta es: "Factura timbrada exitosamente" ✓
- [ ] UUID aparece (formato: xxxx-xxxx-xxxx-xxxx)
- [ ] Folio aparece (número secuencial)

### Verificación en FiscalAPI
- [ ] He abierto: https://live.fiscalapi.com/dashboard
- [ ] He ido a: Invoices o Facturas
- [ ] He encontrado la factura por UUID o folio
- [ ] Status muestra: "Vigente" ✅
- [ ] Datos coinciden (monto, RFC, fecha)

### Verificación en SAT
- [ ] He abierto: https://prodint.sat.gob.mx/CFDI/ConsultaCFDIService.jsp
- [ ] He ingresado el UUID de la factura
- [ ] He ingresado RFC del emisor: GAMA6111156JA
- [ ] He ingresado RFC del receptor
- [ ] He ingresado el total de la factura
- [ ] He hecho clic: Consultar
- [ ] Resultado muestra: "VIGENTE" ✅
- [ ] Todos los datos coinciden

---

## ✅ Verificación Post-Cambio

### En Aplicación
- [ ] Puedo generar facturas sin errores
- [ ] Las facturas se timbran automáticamente
- [ ] UUID aparece correctamente
- [ ] Puedo descargar PDF sin problemas
- [ ] Puedo enviar por email sin problemas

### En Base de Datos
- [ ] Tabla Facturas tiene nuevas facturas
- [ ] Campo EstaFacturada = 1 ✓
- [ ] Campo FiscalAPIInvoiceId tiene valor ✓
- [ ] Campo XML_CFDI tiene contenido ✓

### En FiscalAPI Dashboard
- [ ] Mis facturas aparecen en el dashboard
- [ ] Status es "Vigente"
- [ ] Puedo ver el CFDI completo
- [ ] Puedo descargar PDF oficial

### En SAT
- [ ] Las facturas aparecen en portal SAT
- [ ] Puedo consultar por UUID
- [ ] Status es "VIGENTE"
- [ ] Validación es correcta

---

## 🚨 Si Algo Falla

### Error: "Invalid API Key"
- [ ] Verifiqué que ApiKey comienza con sk_live_
- [ ] Verifiqué que NO tiene espacios
- [ ] Copié nuevamente del dashboard
- [ ] Re-ejecuté el script SQL
- [ ] Recompilé la aplicación

### Error: "Certificate not valid"
- [ ] Verifiqué que certificado está vigente en SAT
- [ ] Verifiqué contraseña es correcta
- [ ] Subí nuevamente a FiscalAPI
- [ ] Esperé 30 segundos
- [ ] Intenté nueva factura

### Error: "CFDI not found in SAT"
- [ ] Esperé 2-5 minutos después de timbrar
- [ ] Verifiqué UUID correctamente
- [ ] Verifiqué RFC correcto
- [ ] Intento nuevamente en portal SAT

### Error de Compilación
- [ ] Limpié la solución (Clean)
- [ ] Recompilé (Rebuild)
- [ ] Cerré Visual Studio y reabrí
- [ ] Verifiqué que editaron el script correctamente

---

## 📞 Escalación

Si nada funciona, contacta a:

| Problema | Contacto | Link |
|----------|----------|------|
| FiscalAPI falla | FiscalAPI Support | support@fiscalapi.com |
| SAT no valida | SAT Help | https://www.sat.gob.mx |
| Aplicación error | Developer | (your email) |

**Información útil para soporte:**
- [ ] UUID de la factura: ________________
- [ ] Error exacto: ________________
- [ ] Timestamp del error: ________________
- [ ] API Response: ________________

---

## 📊 Resumen Final

```
Cambio completado: _____ (SÍ / NO)
Fecha: _____ de _____ de 20__
Responsable: _____________________
Firma: _____________________

Verificaciones realizadas: ____ / 50
Errores encontrados: ____
Resueltos: ____
Pendientes: ____

¿Listo para producción? _____ (SÍ / NO)
```

---

## 🎉 ¡Lo Hiciste!

Si todas las casillas están marcadas:

✅ **ESTÁS EN PRODUCCIÓN**
✅ **TUS FACTURAS SON REALES**
✅ **APARECEN EN SAT**
✅ **TUS CLIENTES LAS PUEDEN CONSULTAR**

**Ahora:**
- Mantén backup diario de DB_TIENDA
- Monitorea facturas diariamente
- Resuelve problemas rápidamente
- Contacta a SAT si hay dudas

---

**Felicidades! 🚀 Tu sistema está en PRODUCCIÓN.**
