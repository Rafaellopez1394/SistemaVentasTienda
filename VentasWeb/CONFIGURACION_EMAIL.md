# Configuración de Envío de Emails - Sistema de Facturación

## 📧 Descripción

El sistema ahora permite enviar facturas electrónicas (CFDI) por email con archivos PDF y XML adjuntos.

## ✅ Características Implementadas

- ✅ Botón "Enviar Email" en tabla de facturas
- ✅ Modal para capturar email del destinatario
- ✅ Generación automática de PDF con QR Code
- ✅ Adjuntar XML timbrado
- ✅ Email HTML con diseño profesional
- ✅ Log completo de envíos en base de datos
- ✅ Validaciones de formato de email
- ✅ Manejo de errores con mensajes amigables

## 🔧 Configuración Requerida

### 1. Ejecutar Script SQL

Primero, ejecute el script `019_CREAR_EMAIL_LOG.sql` en su base de datos:

```sql
-- Ubicación: VentasWeb/Utilidad/SQL Server/019_CREAR_EMAIL_LOG.sql
```

Este script crea:
- Tabla `EmailLog` para auditoría de envíos
- Campos nuevos en `Factura`: `EmailCliente`, `EnviarEmailAutomatico`
- Campo nuevo en `Cliente`: `EmailFacturacion`

### 2. Configurar SMTP en Web.config

Agregue las siguientes líneas dentro de la sección `<appSettings>` de su archivo `Web.config`:

```xml
<configuration>
  <appSettings>
    <!-- Configuración SMTP -->
    <add key="SMTP_Host" value="smtp.gmail.com" />
    <add key="SMTP_Port" value="587" />
    <add key="SMTP_Username" value="tu_email@gmail.com" />
    <add key="SMTP_Password" value="tu_contraseña_o_app_password" />
    <add key="SMTP_SSL" value="true" />
    <add key="SMTP_FromEmail" value="tu_email@gmail.com" />
    <add key="SMTP_FromName" value="Mi Empresa SA de CV" />
  </appSettings>
</configuration>
```

### 3. Configuración por Proveedor

#### Gmail

1. **Habilitar verificación en 2 pasos:**
   - Vaya a: https://myaccount.google.com/security
   - Active "Verificación en 2 pasos"

2. **Generar contraseña de aplicación:**
   - Vaya a: https://myaccount.google.com/apppasswords
   - Seleccione "Correo" y "Otro (nombre personalizado)"
   - Copie la contraseña generada (16 caracteres sin espacios)

3. **Configuración:**
   ```xml
   <add key="SMTP_Host" value="smtp.gmail.com" />
   <add key="SMTP_Port" value="587" />
   <add key="SMTP_Username" value="tu_email@gmail.com" />
   <add key="SMTP_Password" value="abcdefghijklmnop" />
   <add key="SMTP_SSL" value="true" />
   ```

#### Outlook / Hotmail

```xml
<add key="SMTP_Host" value="smtp-mail.outlook.com" />
<add key="SMTP_Port" value="587" />
<add key="SMTP_Username" value="tu_email@outlook.com" />
<add key="SMTP_Password" value="tu_contraseña" />
<add key="SMTP_SSL" value="true" />
```

#### Office 365

```xml
<add key="SMTP_Host" value="smtp.office365.com" />
<add key="SMTP_Port" value="587" />
<add key="SMTP_Username" value="tu_email@tudominio.com" />
<add key="SMTP_Password" value="tu_contraseña" />
<add key="SMTP_SSL" value="true" />
```

#### SendGrid (Recomendado para Producción)

```xml
<add key="SMTP_Host" value="smtp.sendgrid.net" />
<add key="SMTP_Port" value="587" />
<add key="SMTP_Username" value="apikey" />
<add key="SMTP_Password" value="SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxx" />
<add key="SMTP_SSL" value="true" />
<add key="SMTP_FromEmail" value="noreply@tuempresa.com" />
```

**Ventajas de SendGrid:**
- ✅ 100 emails gratis por día
- ✅ Alta tasa de entrega
- ✅ No requiere verificación en 2 pasos
- ✅ Estadísticas detalladas
- ✅ API Key en lugar de contraseña

**Registro:** https://signup.sendgrid.com/

## 📝 Uso del Sistema

### Enviar Factura por Email

1. Vaya a la lista de facturas (`/Factura/Index`)
2. Localice la factura que desea enviar (debe estar **TIMBRADA**)
3. Haga clic en el botón verde con icono de sobre (📧)
4. Se abrirá un modal mostrando:
   - Serie y Folio de la factura
   - UUID del timbre
5. Ingrese el email del destinatario
6. Haga clic en "Enviar"
7. El sistema:
   - Genera el PDF automáticamente
   - Obtiene el XML timbrado
   - Envía el email con ambos archivos adjuntos
   - Registra el envío en la base de datos

### Email Enviado

El cliente recibirá un email con:

**Asunto:** `CFDI - Factura A123`

**Cuerpo:** Email HTML con:
- Nombre de su empresa
- Información de la factura (Serie-Folio, UUID, Fecha, Total)
- Instrucciones para descargar los archivos
- Pie de página profesional

**Adjuntos:**
- `Factura_A123_12345678.pdf` (PDF con código QR)
- `Factura_A123_12345678.xml` (XML timbrado)

## 🔍 Auditoría de Emails

Todos los envíos se registran en la tabla `EmailLog` con:

- ✅ Fecha y hora del envío
- ✅ Email del destinatario
- ✅ UUID de la factura
- ✅ Estado (exitoso/fallido)
- ✅ Mensaje de error (si aplica)
- ✅ Usuario que realizó el envío
- ✅ Servidor SMTP utilizado

**Consulta de ejemplo:**

```sql
-- Ver últimos 10 envíos
SELECT TOP 10
    TipoDocumento,
    UUID,
    EmailDestinatario,
    FechaEnvio,
    Exitoso,
    MensajeError
FROM EmailLog
ORDER BY FechaEnvio DESC;

-- Ver envíos fallidos
SELECT *
FROM EmailLog
WHERE Exitoso = 0
ORDER BY FechaEnvio DESC;

-- Emails enviados hoy
SELECT COUNT(*) AS TotalEnvios
FROM EmailLog
WHERE CAST(FechaEnvio AS DATE) = CAST(GETDATE() AS DATE);
```

## ⚠️ Solución de Problemas

### Error: "Error de configuración SMTP"

**Causa:** Faltan configuraciones en Web.config

**Solución:**
1. Verifique que todas las claves SMTP estén en `<appSettings>`
2. Asegúrese de que no haya espacios extras en los valores
3. Reinicie IIS o la aplicación

### Error: "Formato de email inválido"

**Causa:** Email ingresado no tiene formato válido

**Solución:**
- Verifique que el email tenga formato: `usuario@dominio.com`
- No incluya espacios ni caracteres especiales

### Error: "La factura no ha sido timbrada"

**Causa:** Intentando enviar factura sin timbrar

**Solución:**
- Solo se pueden enviar facturas con estatus **TIMBRADA**
- Primero timbre la factura, luego envíela

### Error: "Authentication failed" (Gmail)

**Causa:** Contraseña incorrecta o 2FA no configurado

**Solución:**
1. Active verificación en 2 pasos en Google
2. Genere contraseña de aplicación
3. Use la contraseña de aplicación, NO su contraseña de Gmail

### Error: "Timeout" al enviar

**Causa:** Servidor SMTP no responde

**Solución:**
1. Verifique la conexión a internet
2. Confirme que el puerto no esté bloqueado por firewall
3. Intente con puerto alternativo (465 para SSL)

### Emails no llegan a la bandeja de entrada

**Causa:** Puede estar en spam

**Solución:**
1. Revise la carpeta de spam del destinatario
2. Configure registros SPF/DKIM en su dominio
3. Use un proveedor profesional como SendGrid
4. Solicite al destinatario agregar su email a contactos

## 🚀 Recomendaciones para Producción

### 1. Use un Proveedor Profesional

Para envíos en producción, recomendamos usar servicios especializados:

- **SendGrid** - 100 emails/día gratis
- **Mailgun** - 5,000 emails/mes gratis
- **Amazon SES** - 62,000 emails/mes gratis (con EC2)

### 2. Configure DNS

Para mejorar la entregabilidad, configure en su dominio:

**SPF Record:**
```
v=spf1 include:_spf.google.com ~all
```

**DKIM Record:**
Solicite la clave pública a su proveedor de email

### 3. Monitoreo

Revise periódicamente la tabla `EmailLog` para:
- Detectar fallos recurrentes
- Identificar destinatarios con problemas
- Analizar horarios de mayor envío

### 4. Límites de Envío

Configure límites diarios para evitar ser marcado como spam:

```sql
-- Ver envíos por día del último mes
SELECT 
    CAST(FechaEnvio AS DATE) AS Fecha,
    COUNT(*) AS TotalEnvios,
    SUM(CASE WHEN Exitoso = 1 THEN 1 ELSE 0 END) AS Exitosos,
    SUM(CASE WHEN Exitoso = 0 THEN 1 ELSE 0 END) AS Fallidos
FROM EmailLog
WHERE FechaEnvio >= DATEADD(MONTH, -1, GETDATE())
GROUP BY CAST(FechaEnvio AS DATE)
ORDER BY Fecha DESC;
```

**Límites recomendados:**
- Gmail personal: 500 emails/día
- Gmail Workspace: 2,000 emails/día
- SendGrid Free: 100 emails/día
- Mailgun Free: 5,000 emails/mes

## 📚 Referencias

- Documentación SendGrid: https://docs.sendgrid.com/
- Configuración Gmail SMTP: https://support.google.com/mail/answer/7126229
- SPF Records: https://www.spf-record.com/
- DKIM Setup: https://postmarkapp.com/guides/dkim

## 📞 Soporte

Para problemas o preguntas sobre la configuración de emails:

1. Revise los logs en la tabla `EmailLog`
2. Consulte la sección de "Solución de Problemas"
3. Verifique que Web.config tenga todas las configuraciones
4. Pruebe primero con un email personal antes de enviar a clientes

---

**Fecha de implementación:** Diciembre 2025  
**Versión:** 1.0  
**Estado:** ✅ Producción Ready
