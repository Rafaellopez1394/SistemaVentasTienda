# ⚡ GUÍA RÁPIDA DE DESPLIEGUE

## 🚀 Despliegue en 30 Minutos

### Paso 1: Base de Datos (10 minutos)

```sql
-- 1. Crear base de datos
CREATE DATABASE SistemaVentas;
GO

USE SistemaVentas;
GO

-- 2. Ejecutar scripts en orden (carpeta: Utilidad/SQL Server/)
/*
001_CREAR_TABLAS_BASE.sql
002_CREAR_USUARIOS.sql
003_CREAR_CLIENTES.sql
004_CREAR_PRODUCTOS.sql
005_CREAR_VENTAS.sql
006_CREAR_FACTURAS.sql
007_CREAR_NOMINA.sql
008_CREAR_POLIZAS.sql
009_CREAR_CONFIGURACION.sql
010_DATOS_INICIALES.sql
011_CREAR_VISTAS_REPORTES.sql
012_CREAR_CUENTAS_POR_PAGAR.sql
013_CREAR_MERMAS_AJUSTES.sql
014_AGREGAR_CAMPOS_NOMINA.sql
015_AGREGAR_CAMPOS_CANCELACION.sql
018_CREAR_COMPLEMENTO_PAGO.sql
019_CREAR_EMAIL_LOG.sql
*/

-- 3. Configurar empresa (editar con sus datos)
INSERT INTO ConfiguracionEmpresa (
    RFC, RazonSocial, NombreComercial, RegimenFiscal,
    Calle, NumeroExterior, Colonia, Municipio, Estado, CodigoPostal, Pais,
    Telefono, Email, Logo
) VALUES (
    'AAA010101AAA',
    'MI EMPRESA SA DE CV',
    'Mi Empresa',
    '601',
    'AV EJEMPLO',
    '123',
    'CENTRO',
    'CIUDAD',
    'ESTADO',
    '12345',
    'México',
    '1234567890',
    'contacto@miempresa.com',
    NULL
);

-- 4. Configurar PAC Finkok (DEMO para pruebas)
INSERT INTO ConfiguracionPAC (
    ProveedorPAC,
    UrlTimbrado,
    UrlCancelacion,
    Usuario,
    Password,
    RutaCertificado,
    RutaLlavePrivada,
    PasswordCertificado,
    Activo
) VALUES (
    'Finkok',
    'https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl',
    'https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl',
    'demo@finkok.com',
    'demo',
    'C:\Certificados\CSD_DEMO.cer',
    'C:\Certificados\CSD_DEMO.key',
    '12345678a',
    1
);

-- ✅ LISTO: Base de datos configurada
```

### Paso 2: Certificados CSD (5 minutos)

```powershell
# 1. Crear carpeta para certificados
New-Item -Path "C:\Certificados" -ItemType Directory -Force

# 2. Copiar certificados del SAT a esta carpeta
# - CSD_Empresa.cer (certificado público)
# - CSD_Empresa.key (llave privada)

# 3. Verificar que existen
Get-ChildItem "C:\Certificados"

# ✅ LISTO: Certificados en lugar
```

### Paso 3: Configurar Web.config (5 minutos)

**Ubicación:** `VentasWeb/Web.config`

```xml
<configuration>
  <connectionStrings>
    <!-- CAMBIAR: Servidor, usuario y contraseña -->
    <add name="CN" 
         connectionString="Data Source=TU_SERVIDOR;Initial Catalog=SistemaVentas;User ID=sa;Password=TU_PASSWORD" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- ... otras configuraciones ... -->
    
    <!-- AGREGAR: Configuración SMTP para emails -->
    <add key="SMTP_Host" value="smtp.gmail.com" />
    <add key="SMTP_Port" value="587" />
    <add key="SMTP_Username" value="tu_email@gmail.com" />
    <add key="SMTP_Password" value="tu_app_password" />
    <add key="SMTP_SSL" value="true" />
    <add key="SMTP_FromEmail" value="tu_email@gmail.com" />
    <add key="SMTP_FromName" value="Mi Empresa SA de CV" />
  </appSettings>
</configuration>
```

**Para obtener App Password de Gmail:**
1. Ir a: https://myaccount.google.com/apppasswords
2. Crear contraseña de aplicación
3. Copiar contraseña generada (16 caracteres)
4. Pegar en `SMTP_Password`

### Paso 4: Publicar en IIS (10 minutos)

**Opción A: Visual Studio**
```
1. Clic derecho en proyecto "VentasWeb"
2. Publicar...
3. Carpeta → C:\inetpub\wwwroot\SistemaVentas
4. Publicar
```

**Opción B: Copiar archivos manualmente**
```powershell
# Copiar carpeta VentasWeb completa a IIS
Copy-Item -Path ".\VentasWeb\*" -Destination "C:\inetpub\wwwroot\SistemaVentas" -Recurse -Force
```

**Configurar IIS:**
```powershell
# Importar módulo IIS
Import-Module WebAdministration

# Crear Application Pool
New-WebAppPool -Name "SistemaVentasPool"
Set-ItemProperty "IIS:\AppPools\SistemaVentasPool" managedRuntimeVersion "v4.0"

# Crear sitio web
New-Website -Name "SistemaVentas" `
            -Port 8080 `
            -PhysicalPath "C:\inetpub\wwwroot\SistemaVentas" `
            -ApplicationPool "SistemaVentasPool"

# Asignar permisos a carpeta de certificados
icacls "C:\Certificados" /grant "IIS AppPool\SistemaVentasPool:(OI)(CI)R"
```

### Paso 5: Verificar (5 minutos)

**1. Probar acceso:**
```
http://localhost:8080/
```

**2. Probar login:**
- Usuario: admin
- Contraseña: (la que configuró en BD)

**3. Probar factura de prueba:**
- Ir a Facturas → Nueva Factura
- Llenar datos mínimos
- Generar y Timbrar
- Verificar que se timbra correctamente
- Descargar PDF
- Enviar por email

**4. Verificar logs:**
```sql
-- Ver si se timbró correctamente
SELECT TOP 5 * FROM Facturas ORDER BY FechaCreacion DESC;

-- Ver si se envió el email
SELECT TOP 5 * FROM EmailLog ORDER BY FechaEnvio DESC;
```

**✅ SI TODO FUNCIONA: SISTEMA LISTO**

---

## 🔥 Solución Rápida de Problemas

### Error: "No se puede conectar a la base de datos"
```
✅ Solución: Verificar connection string en Web.config
✅ Verificar que SQL Server está corriendo
✅ Verificar usuario y contraseña
```

### Error: "No se encontró el certificado"
```
✅ Solución: Verificar ruta en ConfiguracionPAC
✅ Verificar que archivos .cer y .key existen
✅ Dar permisos IIS AppPool a carpeta C:\Certificados
```

### Error: "Error de configuración SMTP"
```
✅ Solución: Agregar appSettings SMTP en Web.config
✅ Para Gmail: Generar App Password
✅ Verificar que SMTP_SSL = true
```

### Error: "El PAC no responde"
```
✅ Solución: Verificar URL de Finkok en BD
✅ Para pruebas: Usar demo-facturacion.finkok.com
✅ Para producción: Usar facturacion.finkok.com
✅ Verificar credenciales (usuario y password)
```

### Facturas no llegan por email
```
✅ Revisar carpeta de SPAM
✅ Verificar configuración SMTP
✅ Consultar EmailLog en BD para ver errores
✅ Probar con otro email
```

---

## 📋 Checklist de Verificación

Antes de considerar el sistema en producción, verifique:

### Base de Datos
- [ ] 19 scripts SQL ejecutados sin errores
- [ ] ConfiguracionEmpresa tiene datos correctos
- [ ] ConfiguracionPAC configurado (demo o producción)
- [ ] Existe al menos un usuario admin

### Archivos
- [ ] Certificados .cer y .key en C:\Certificados
- [ ] Web.config tiene connection string correcto
- [ ] Web.config tiene configuración SMTP
- [ ] Permisos de IIS AppPool a carpeta Certificados

### IIS
- [ ] Sitio web creado y corriendo
- [ ] Application Pool configurado (.NET 4.0)
- [ ] Puerto accesible (8080 o el que elija)
- [ ] No hay errores en Event Viewer

### Funcional
- [ ] Login funciona
- [ ] Se puede timbrar una factura
- [ ] PDF se genera correctamente
- [ ] XML se descarga correctamente
- [ ] Email se envía correctamente
- [ ] Reportes cargan datos

---

## 🎯 Comandos Útiles

### SQL Server
```sql
-- Ver facturas timbradas hoy
SELECT COUNT(*) FROM Facturas 
WHERE CAST(FechaTimbrado AS DATE) = CAST(GETDATE() AS DATE);

-- Ver emails enviados hoy
SELECT COUNT(*) FROM EmailLog 
WHERE CAST(FechaEnvio AS DATE) = CAST(GETDATE() AS DATE);

-- Ver errores en emails
SELECT * FROM EmailLog WHERE Exitoso = 0;

-- Ver configuración actual
SELECT * FROM ConfiguracionPAC;
SELECT * FROM ConfiguracionEmpresa;
```

### PowerShell
```powershell
# Ver si IIS está corriendo
Get-Service W3SVC

# Reiniciar sitio web
Restart-WebAppPool -Name "SistemaVentasPool"

# Ver logs de IIS
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" | Select-Object -Last 20
```

---

## 📞 Documentación Completa

Para información detallada, consulte:

- **README_PRODUCCION.md** - Resumen ejecutivo completo
- **CONFIGURACION_EMAIL.md** - Guía detallada de emails
- **Web.config.SMTP.EXAMPLE** - Ejemplos de configuración SMTP

---

## 🚀 ¡Listo para Producción!

Si completó todos los pasos de esta guía, su sistema está **100% operativo**.

**Próximo paso:** Capacitar a los usuarios y comenzar a facturar.

**¡Éxito!** 🎉
