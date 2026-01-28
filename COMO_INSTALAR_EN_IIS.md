# 🚀 CÓMO INSTALAR EL SISTEMA EN IIS

## ⚠️ IMPORTANTE: Debes ejecutar como ADMINISTRADOR

El sistema necesita IIS (Internet Information Services) para funcionar. Si no lo tienes instalado, el script lo instalará automáticamente.

---

## 📋 PASOS PARA INSTALAR

### **Opción 1: Instalación Automática (RECOMENDADO)**

1. **Localiza el archivo:**
   ```
   INSTALAR_Y_DESPLEGAR_IIS.ps1
   ```

2. **Ejecuta como Administrador:**
   - Clic derecho en el archivo `INSTALAR_Y_DESPLEGAR_IIS.ps1`
   - Selecciona "**Ejecutar con PowerShell**" (como Administrador)
   - Si aparece un mensaje de seguridad, presiona "S" (Sí)

3. **Espera a que termine:**
   - El script instalará IIS (si no está instalado)
   - Copiará los archivos del sistema
   - Configurará el sitio web
   - Establecerá los permisos necesarios

4. **Accede al sistema:**
   ```
   http://localhost/VentasWeb
   ```

5. **Accede al módulo fiscal:**
   ```
   http://localhost/VentasWeb/ConfiguracionFiscal
   ```

---

### **Opción 2: Instalación Manual**

Si el script automático no funciona, sigue estos pasos:

#### **Paso 1: Instalar IIS**

1. Abre **PowerShell como Administrador**
2. Ejecuta:
   ```powershell
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45 -All
   ```
3. Reinicia Windows si te lo pide

#### **Paso 2: Copiar Archivos**

1. Crea la carpeta destino:
   ```powershell
   New-Item -Path "C:\inetpub\wwwroot\VentasWeb" -ItemType Directory -Force
   ```

2. Copia los archivos del sistema:
   ```powershell
   $source = "TU_RUTA\SistemaVentasTienda\VentasWeb"
   $dest = "C:\inetpub\wwwroot\VentasWeb"
   
   Copy-Item "$source\bin" "$dest\bin" -Recurse -Force
   Copy-Item "$source\Content" "$dest\Content" -Recurse -Force
   Copy-Item "$source\Scripts" "$dest\Scripts" -Recurse -Force
   Copy-Item "$source\Views" "$dest\Views" -Recurse -Force
   Copy-Item "$source\Global.asax" "$dest\" -Force
   Copy-Item "$source\Web.config" "$dest\" -Force
   ```

#### **Paso 3: Configurar IIS**

1. Abre **IIS Manager**:
   - Presiona `Win + R`
   - Escribe: `inetmgr`
   - Presiona Enter

2. **Crear Application Pool:**
   - Clic derecho en "Application Pools" → "Add Application Pool"
   - Nombre: `VentasWebPool`
   - .NET CLR version: `v4.0`
   - Managed pipeline mode: `Integrated`
   - Clic en "OK"

3. **Crear Website:**
   - Clic derecho en "Sites" → "Add Website"
   - Site name: `VentasWeb`
   - Application pool: `VentasWebPool`
   - Physical path: `C:\inetpub\wwwroot\VentasWeb`
   - Binding:
     - Type: `http`
     - Port: `80`
     - Host name: (dejar vacío)
   - Clic en "OK"

4. **Establecer Permisos:**
   - Clic derecho en `C:\inetpub\wwwroot\VentasWeb`
   - Propiedades → Seguridad → Editar
   - Agregar → Escribir: `IIS_IUSRS`
   - Marcar "Control total"
   - Aplicar → OK

#### **Paso 4: Reiniciar IIS**

```powershell
iisreset
```

---

## ✅ VERIFICACIÓN

### **1. Verifica que los archivos estén copiados:**
```powershell
Test-Path "C:\inetpub\wwwroot\VentasWeb\bin\VentasWeb.dll"
```
Debe devolver: `True`

### **2. Verifica que IIS esté corriendo:**
```powershell
Get-Service W3SVC
```
Status debe ser: `Running`

### **3. Prueba el sitio:**
Abre un navegador y ve a:
```
http://localhost/VentasWeb
```

### **4. Prueba el módulo fiscal:**
```
http://localhost/VentasWeb/ConfiguracionFiscal
```

---

## 🔧 SOLUCIÓN DE PROBLEMAS

### **Error 404 - No se encuentra el recurso**

**Causa:** El sitio no está correctamente configurado en IIS

**Solución:**
1. Abre IIS Manager (`inetmgr`)
2. Verifica que el sitio "VentasWeb" esté en estado "Started"
3. Verifica que la ruta física apunte a `C:\inetpub\wwwroot\VentasWeb`
4. Reinicia IIS: `iisreset`

---

### **Error 500 - Error interno del servidor**

**Causa:** Problema con Web.config o conexión a base de datos

**Solución:**
1. Abre `C:\inetpub\wwwroot\VentasWeb\Web.config`
2. Verifica la cadena de conexión:
   ```xml
   <connectionStrings>
     <add name="TIENDA" 
          connectionString="Data Source=TU_SERVIDOR;Initial Catalog=DB_TIENDA;Integrated Security=True" 
          providerName="System.Data.SqlClient"/>
   </connectionStrings>
   ```
3. Cambia `TU_SERVIDOR` por el nombre real de tu servidor SQL
4. Guarda y reinicia IIS

---

### **Error: "No se puede leer el archivo de configuración"**

**Causa:** Permisos insuficientes para IIS_IUSRS

**Solución:**
1. Clic derecho en `C:\inetpub\wwwroot\VentasWeb`
2. Propiedades → Seguridad → Editar
3. Agregar usuario: `IIS_IUSRS`
4. Dar "Control total"
5. Aplicar cambios

---

### **El módulo ConfiguracionFiscal muestra 404**

**Causa:** El controlador no está compilado o el enrutamiento está mal

**Verificación:**
```powershell
# Verifica que el DLL existe
Test-Path "C:\inetpub\wwwroot\VentasWeb\bin\VentasWeb.dll"

# Verifica el tamaño (debe ser ~370 KB con el nuevo controlador)
Get-Item "C:\inetpub\wwwroot\VentasWeb\bin\VentasWeb.dll" | Select Length
```

**Solución:**
1. Recompila el proyecto:
   ```powershell
   cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
   MSBuild.exe VentasWeb.sln /t:Rebuild /p:Configuration=Release
   ```
2. Copia el nuevo DLL:
   ```powershell
   Copy-Item "bin\VentasWeb.dll" "C:\inetpub\wwwroot\VentasWeb\bin\" -Force
   ```
3. Reinicia IIS: `iisreset`

---

## 📞 COMANDOS ÚTILES

```powershell
# Reiniciar IIS
iisreset

# Ver estado del servicio IIS
Get-Service W3SVC

# Iniciar IIS
Start-Service W3SVC

# Detener IIS
Stop-Service W3SVC

# Abrir IIS Manager
inetmgr

# Ver sitios web configurados
Import-Module WebAdministration
Get-Website

# Ver Application Pools
Get-WebAppPool

# Verificar que el puerto 80 esté libre
netstat -ano | findstr :80
```

---

## 📝 NOTAS IMPORTANTES

1. **Puerto 80:** Si tienes otro servicio usando el puerto 80 (como Skype, Apache, etc.), debes:
   - Cambiar el puerto del sitio en IIS Manager
   - O detener el otro servicio

2. **Firewall:** Si quieres acceder desde otras computadoras, abre el puerto 80 en el firewall:
   ```powershell
   New-NetFirewallRule -DisplayName "IIS HTTP" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
   ```

3. **SQL Server:** Asegúrate de que SQL Server esté corriendo y que la cadena de conexión en `Web.config` sea correcta.

4. **Navegador:** Usa un navegador moderno (Chrome, Edge, Firefox) para mejor compatibilidad.

---

## 🎯 PRÓXIMOS PASOS DESPUÉS DE LA INSTALACIÓN

Una vez que el sistema esté funcionando en IIS:

1. **Accede al módulo fiscal:**
   ```
   http://localhost/VentasWeb/ConfiguracionFiscal
   ```

2. **Configura los datos fiscales:**
   - Tab 1: Ingresa RFC, Razón Social, Régimen Fiscal, Código Postal
   - Clic en "Guardar Datos Fiscales"

3. **Sube los certificados digitales:**
   - Tab 2: Sube tu archivo .CER y .KEY
   - Ingresa la contraseña del certificado
   - Clic en "Cargar Certificado"

4. **Configura el PAC:**
   - Tab 3: Ingresa API Key, Tenant, selecciona Ambiente
   - Clic en "Guardar Configuración PAC"

5. **¡Listo!** Tu sistema está configurado para facturación electrónica.

---

## 📧 SOPORTE

Si tienes problemas con la instalación, revisa:
- Los logs de IIS en: `C:\inetpub\logs\LogFiles\`
- El Event Viewer de Windows: `eventvwr.msc`
- Verifica que el SQL Server esté corriendo

---

**¡Éxito con tu instalación!** 🎉
