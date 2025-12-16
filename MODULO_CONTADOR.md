# 📊 MÓDULO DEL CONTADOR - DOCUMENTACIÓN

## ✅ Estado: IMPLEMENTADO

El módulo del contador está completamente funcional y proporciona todas las herramientas necesarias para gestionar la configuración contable, fiscal y de nómina del sistema.

---

## 🔐 Acceso al Sistema

### Usuario Contador
- **Email:** `contador@empresa.com`
- **Contraseña:** `Contador123`
- **Rol:** CONTADOR

### Primer Acceso
1. Ejecutar script SQL: `020_CREAR_ROL_CONTADOR.sql`
2. Ingresar al sistema con las credenciales anteriores
3. El sistema redirigirá automáticamente al Dashboard del Contador

---

## 📋 Módulos Implementados

### 1. Dashboard del Contador
**URL:** `/Contador/Dashboard`

**Características:**
- ✅ KPIs en tiempo real:
  - Facturas del mes y total facturado
  - Nómina procesada (recibos y monto)
  - Cuentas por pagar pendientes
  - Pólizas contables del mes

- ✅ Alertas automáticas:
  - Declaraciones mensuales (días 15-17)
  - Cierre de mes (día 28+)
  - Cuentas vencidas
  - Timbres próximos a agotar

- ✅ Acceso rápido a todos los módulos de configuración

**Archivos:**
- `Views/Contador/Dashboard.cshtml`
- `Scripts/Contador/Dashboard.js`

---

### 2. Configuración de Empresa
**URL:** `/Contador/ConfiguracionEmpresa`

**Permite configurar:**
- ✅ **Datos Fiscales:**
  - RFC (13 caracteres)
  - Razón Social
  - Nombre Comercial
  - Régimen Fiscal (catálogo SAT)

- ✅ **Domicilio Fiscal:**
  - Calle, número exterior e interior
  - Colonia
  - Código Postal (5 dígitos)
  - Municipio
  - Estado
  - País

- ✅ **Contacto:**
  - Teléfono
  - Email
  - Sitio Web

**Validaciones:**
- RFC debe tener 12 o 13 caracteres
- Régimen fiscal debe ser del catálogo SAT
- Código postal debe tener 5 dígitos
- Campos obligatorios marcados con asterisco (*)

**Archivos:**
- `Views/Contador/ConfiguracionEmpresa.cshtml`

---

### 3. Configuración Contable
**URL:** `/Contador/ConfiguracionContable`

**Permite configurar:**
- ✅ **Ejercicio Fiscal:**
  - Año fiscal actual
  - Mes actual

- ✅ **Cuentas por Defecto:**
  - Bancos (1102)
  - Clientes (1103)
  - Proveedores (2101)
  - IVA Trasladado (2102)
  - IVA Retenido (2103)
  - ISR Retenido (2104)
  - Ventas (4101)
  - Compras/Costo de Ventas (5101)
  - Nómina (5201)
  - IMSS (5202)

- ✅ **Opciones:**
  - Usar pólizas automáticas
  - Requerir autorización para cancelaciones
  - Días de vencimiento de facturas (default: 30)

**Base de Datos:**
- Tabla: `ConfiguracionContable`
- 1 registro único

**Archivos:**
- `Models/ConfiguracionContador.cs` (clase ConfiguracionContable)
- `Controllers/ContadorController.cs` (métodos Contable)
- `CD_ConfiguracionContador.cs` (CRUD)

---

### 4. Catálogo de Cuentas
**URL:** `/Contador/CatalogoCuentas`

**Características:**
- ✅ **Estructura Jerárquica:**
  - Nivel 1: Cuentas de Mayor (ACTIVO, PASIVO, CAPITAL, INGRESO, EGRESO)
  - Nivel 2: Subcuentas (ACTIVO CIRCULANTE, GASTOS DE OPERACIÓN, etc.)
  - Nivel 3: Cuentas de Detalle (las que aceptan movimientos)

- ✅ **Catálogo Pre-cargado:**
  - 30+ cuentas básicas
  - Estructura completa de estados financieros
  - Códigos numéricos estándar

- ✅ **CRUD Completo:**
  - Agregar nuevas cuentas
  - Modificar cuentas existentes
  - Desactivar cuentas (no eliminar)
  - Buscar y filtrar

- ✅ **Campos:**
  - Código (único, hasta 20 caracteres)
  - Nombre
  - Nivel (1, 2 o 3)
  - Cuenta Padre
  - Tipo (ACTIVO, PASIVO, CAPITAL, INGRESO, EGRESO)
  - Naturaleza (D=Deudora, A=Acreedora)
  - Acepta Movimientos (sí/no)
  - Código SAT (agrupador)
  - Descripción

**Ejemplo de Estructura:**
```
1000 - ACTIVO (Nivel 1)
  1100 - ACTIVO CIRCULANTE (Nivel 2)
    1101 - Caja (Nivel 3) ✓ Acepta Movimientos
    1102 - Bancos (Nivel 3) ✓ Acepta Movimientos
    1103 - Clientes (Nivel 3) ✓ Acepta Movimientos
```

**Base de Datos:**
- Tabla: `CatalogoCuentas`
- Relación jerárquica con `CuentaPadre`
- Índices en: Codigo, Tipo, CuentaPadre

---

### 5. Configuración de Nómina
**URL:** `/Contador/ConfiguracionNomina`

**Permite configurar:**
- ✅ **Periodicidad:**
  - Tipo de Periodo: SEMANAL, QUINCENAL, MENSUAL
  - Días de Pago (7, 15 o 30)

- ✅ **Salarios 2024:**
  - Salario Mínimo: $207.44 diarios
  - UMA: $108.57 diarios
  - Tope Salario IMSS: $2,500.00

- ✅ **Porcentajes IMSS Empresa:**
  - IMSS Empresa: 23.75%
  - RCV (Retiro, Cesantía, Vejez): 2.00%
  - Guardería: 1.00%
  - Retiro: 2.00%

- ✅ **Porcentajes IMSS Trabajador:**
  - IMSS Trabajador: 2.50%

- ✅ **Configuración CFDI:**
  - Lugar de expedición de nómina
  - Ruta de certificados para nómina

**Base de Datos:**
- Tabla: `ConfiguracionNomina`
- 1 registro único
- Valores actualizables cada año

---

### 6. Percepciones y Deducciones SAT
**URLs:**
- `/Contador/ObtenerPercepciones` (API)
- `/Contador/ObtenerDeducciones` (API)

**Percepciones Pre-cargadas (14 tipos):**
| Clave | Descripción | Grava ISR | Grava IMSS |
|-------|-------------|-----------|------------|
| 001 | Sueldos, Salarios | ✅ | ✅ |
| 002 | Aguinaldo | ✅ | ❌ |
| 003 | PTU | ✅ | ❌ |
| 004 | Reembolso Gastos Médicos | ❌ | ❌ |
| 005 | Fondo de Ahorro | ❌ | ❌ |
| 010 | Premios Puntualidad | ✅ | ✅ |
| 019 | Horas Extra | ✅ | ✅ |
| 025 | Viáticos | ❌ | ❌ |
| 039 | Prima Vacacional | ✅ | ❌ |
| ... | ... | ... | ... |

**Deducciones Pre-cargadas (12 tipos):**
| Clave | Descripción | Tipo |
|-------|-------------|------|
| 001 | Seguridad Social | OBLIGATORIA |
| 002 | ISR | OBLIGATORIA |
| 003 | Aportaciones Retiro | OBLIGATORIA |
| 004 | Otros | VOLUNTARIA |
| 006 | Crédito Vivienda | OBLIGATORIA |
| 010 | Infonavit | OBLIGATORIA |
| 013 | Pensión Alimenticia | OBLIGATORIA |
| ... | ... | ... |

**Base de Datos:**
- Tabla: `PercepcionesNomina` (14 registros)
- Tabla: `DeduccionesNomina` (12 registros)
- Conformes al catálogo oficial del SAT

---

### 7. Certificados Digitales
**URL:** `/Contador/Certificados`

**Descripción:**
Módulo para gestionar los certificados digitales CSD (Certificado de Sello Digital) y FIEL (Firma Electrónica Avanzada) necesarios para la facturación electrónica.

**Permite:**
- ✅ **Cargar certificados**: Subir archivos `.CER` y `.KEY` con su contraseña
- ✅ **Validación automática**: Extrae RFC, No. Certificado, Razón Social y fechas de vigencia
- ✅ **Múltiples certificados**: Gestionar varios certificados simultáneamente
- ✅ **Certificado predeterminado**: Marcar cuál usar por defecto para timbrado
- ✅ **Alertas de vencimiento**: Notificaciones 30 días antes del vencimiento
- ✅ **Configurar uso**: Especificar para qué se usa (Facturas, Nómina, Cancelaciones)
- ✅ **Activar/Desactivar**: Control de estado sin eliminar
- ✅ **Almacenamiento seguro**: Archivos guardados como binarios en BD con password encriptado

**Tipos de Certificados:**
- **CSD**: Para timbrar facturas y nómina
- **FIEL**: Para trámites ante el SAT y firma electrónica

**Datos extraídos automáticamente del .CER:**
- Número de Certificado (Serial Number)
- RFC del contribuyente
- Razón Social
- Fecha de inicio de vigencia
- Fecha de vencimiento

**Tabla muestra:**
- Tipo, Nombre, No. Certificado, RFC, Razón Social
- Fecha de vencimiento con advertencias
- Usos configurados
- Estado (Activo/Inactivo)
- Vigencia (Vigente/Vencido)
- Indicador de predeterminado (⭐)

**Archivos:**
- `Views/Contador/Certificados.cshtml` (vista principal con DataTable y modal)
- `Scripts/Contador/Certificados.js` (lógica de carga y gestión)
- `Controllers/ContadorController.cs` (endpoints SubirCertificado, ObtenerCertificados, ActivarCertificado, etc.)
- `CapaDatos/CD_ConfiguracionContador.cs` (métodos GuardarCertificado, ObtenerCertificadoPredeterminado, etc.)

**Ver documentación completa:** `GESTION_CERTIFICADOS_DIGITALES.md`

---

### 8. Configuración PAC (Finkok)
**URL:** `/Contador/ConfiguracionPAC`

**Permite configurar:**
- ✅ Proveedor PAC (Finkok)
- ✅ URL de Timbrado
- ✅ URL de Cancelación
- ✅ Usuario y Contraseña PAC
- ✅ Activar/Desactivar configuración

**Nota:** Los certificados CSD ahora se gestionan en el módulo **Certificados Digitales** (sección anterior).

**Ambientes:**

**Demo (Pruebas):**
```
URL Timbrado: https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl
URL Cancelación: https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl
Usuario: demo@finkok.com
Password: demo
```

**Producción:**
```
URL Timbrado: https://facturacion.finkok.com/servicios/soap/stamp.wsdl
URL Cancelación: https://facturacion.finkok.com/servicios/soap/cancel.wsdl
Usuario: (tu usuario de Finkok)
Password: (tu password de Finkok)
```

**Base de Datos:**
- Tabla: `ConfiguracionPAC` (ya existe)
- Actualizable desde el módulo del contador

---

## 🗄️ Base de Datos

### Tablas Nuevas (Script 020)

| Tabla | Descripción | Registros |
|-------|-------------|-----------|
| `ConfiguracionContable` | Config contable general | 1 |
| `CatalogoCuentas` | Catálogo de cuentas | 30+ |
| `ConfiguracionNomina` | Config de nómina | 1 |
| `PercepcionesNomina` | Catálogo SAT | 14 |
| `DeduccionesNomina` | Catálogo SAT | 12 |

### Vistas Nuevas

| Vista | Descripción |
|-------|-------------|
| `vw_DashboardContador` | KPIs para dashboard |

### Roles y Permisos

**Rol CONTADOR tiene acceso a:**
- ✅ **Configuración (COMPLETO):**
  - Empresa
  - Contable
  - Nómina
  - CFDI
  - Usuarios

- ✅ **Nómina (COMPLETO):**
  - Empleados
  - Recibos
  - Procesar
  - Reportes
  - CFDI

- ✅ **Contabilidad (COMPLETO):**
  - Pólizas
  - Catálogo Cuentas
  - Libro Mayor
  - Reportes
  - Declaraciones

- ✅ **Facturación (CONSULTA):**
  - Consultar facturas
  - Cancelar facturas
  - Reportes

- ✅ **Reportes:**
  - Estado de Resultados
  - Balance General
  - Flujo de Efectivo
  - Antigüedad de Saldos

- ✅ **Cuentas por Pagar:**
  - Consultar
  - Pagar
  - Reportes

---

## 📁 Estructura de Archivos

```
SistemaVentasTienda/
├── Utilidad/SQL Server/
│   └── 020_CREAR_ROL_CONTADOR.sql (550 líneas)
│
├── CapaModelo/
│   └── ConfiguracionContador.cs (11 clases, 220 líneas)
│
├── CapaDatos/
│   └── CD_ConfiguracionContador.cs (650 líneas)
│
├── VentasWeb/
│   ├── Controllers/
│   │   └── ContadorController.cs (380 líneas)
│   │
│   ├── Views/Contador/
│   │   ├── Dashboard.cshtml (200 líneas)
│   │   └── ConfiguracionEmpresa.cshtml (220 líneas)
│   │
│   └── Scripts/Contador/
│       └── Dashboard.js (50 líneas)
```

**Total:** ~2,270 líneas de código

---

## 🚀 Instalación

### Paso 1: Ejecutar Script SQL

```sql
-- Ejecutar en SQL Server Management Studio
USE SistemaVentas;
GO

-- Ubicación: Utilidad/SQL Server/020_CREAR_ROL_CONTADOR.sql
-- Este script crea:
-- ✓ Rol CONTADOR
-- ✓ Usuario contador@empresa.com
-- ✓ Permisos específicos
-- ✓ 5 tablas nuevas
-- ✓ 1 vista
-- ✓ Catálogo de cuentas básico
-- ✓ Catálogos SAT (percepciones y deducciones)
```

### Paso 2: Compilar Proyecto

```bash
# En Visual Studio
Build > Rebuild Solution

# Verificar que no haya errores
```

### Paso 3: Acceder al Sistema

1. Navegar a: `http://localhost:puerto/`
2. Login con:
   - Email: `contador@empresa.com`
   - Password: `Contador123`
3. El sistema redirige automáticamente a `/Contador/Dashboard`

---

## 🎯 Casos de Uso

### Caso 1: Configurar Empresa por Primera Vez

1. Login como contador
2. Dashboard > Click en "Configurar" de "Datos de la Empresa"
3. Llenar formulario:
   - RFC
   - Razón Social
   - Régimen Fiscal
   - Domicilio completo
4. Guardar
5. ✅ Datos listos para facturación

### Caso 2: Agregar Nueva Cuenta Contable

1. Dashboard > Click en "Catálogo" de "Configuración Contable"
2. Click en "Nueva Cuenta"
3. Llenar datos:
   - Código: 5206
   - Nombre: Papelería
   - Nivel: 3
   - Cuenta Padre: 5200 (GASTOS DE OPERACIÓN)
   - Tipo: EGRESO
   - Naturaleza: D (Deudora)
   - Acepta Movimientos: Sí
4. Guardar
5. ✅ Cuenta disponible para pólizas

### Caso 3: Actualizar Tablas de Nómina (Año Nuevo)

1. Dashboard > Click en "Configurar" de "Configuración Nómina"
2. Actualizar valores 2025:
   - Salario Mínimo: $248.93 (ejemplo)
   - UMA: $113.14 (ejemplo)
3. Guardar
4. ✅ Cálculos de nómina actualizados

### Caso 4: Verificar Configuración PAC

1. Dashboard > Click en "Configurar" de "Configuración CFDI"
2. Verificar:
   - URLs de Finkok
   - Usuario y contraseña
   - Rutas de certificados
3. Probar conexión
4. ✅ Listo para timbrar

---

## 📊 Comparación con app.tesk.mx

### Funcionalidades Equivalentes

| app.tesk.mx | Sistema Implementado | Estado |
|-------------|----------------------|--------|
| Configuración Empresa | `/Contador/ConfiguracionEmpresa` | ✅ |
| Catálogo de Cuentas | `/Contador/CatalogoCuentas` | ✅ |
| Configuración Nómina | `/Contador/ConfiguracionNomina` | ✅ |
| Percepciones/Deducciones | Catálogos SAT integrados | ✅ |
| Dashboard Contable | `/Contador/Dashboard` | ✅ |
| Configuración PAC | `/Contador/ConfiguracionPAC` | ✅ |
| Reportes Contables | `/Reportes/*` | ✅ |
| Pólizas Contables | `/Polizas/*` | ✅ |
| Control de Nómina | `/Nomina/*` | ✅ |
| Facturación CFDI | `/Factura/*` | ✅ |

### Funcionalidades Adicionales

- ✅ Email automático de facturas
- ✅ Complemento de Pago 2.0
- ✅ CFDI Nómina 1.2
- ✅ Alertas automáticas
- ✅ Dashboard en tiempo real
- ✅ Catálogos SAT actualizados

---

## 🔒 Seguridad

- ✅ Acceso restringido por rol CONTADOR
- ✅ Validación de sesión en cada endpoint
- ✅ Auditoría de cambios (UsuarioCreacion, UsuarioModificacion)
- ✅ Campos críticos protegidos (no editables una vez configurados)
- ✅ Contraseñas almacenadas con SHA256

---

## 📝 Notas Importantes

1. **RFC:** Una vez configurado, no se puede cambiar fácilmente ya que afecta todos los CFDI
2. **Catálogo de Cuentas:** No eliminar cuentas, mejor desactivarlas
3. **Salario Mínimo/UMA:** Actualizar cada año (1 de enero)
4. **Certificados CSD:** Renovar cada 4 años con el SAT
5. **Finkok:** Comprar timbres antes de que se agoten

---

## 🎓 Capacitación Recomendada

### Para el Contador:
1. ✅ Configuración inicial (30 min)
2. ✅ Uso del catálogo de cuentas (15 min)
3. ✅ Configuración de nómina (20 min)
4. ✅ Generación de reportes (15 min)
5. ✅ Revisión de pólizas automáticas (10 min)

**Total:** ~90 minutos

---

## ✅ Checklist de Configuración Inicial

### Primer Uso del Sistema

- [ ] Ejecutar `020_CREAR_ROL_CONTADOR.sql`
- [ ] Login con `contador@empresa.com`
- [ ] **Configurar Empresa:**
  - [ ] RFC
  - [ ] Razón Social
  - [ ] Régimen Fiscal
  - [ ] Domicilio completo
  - [ ] Contacto
- [ ] **Configurar Contable:**
  - [ ] Verificar catálogo de cuentas
  - [ ] Configurar cuentas por defecto
  - [ ] Activar pólizas automáticas
- [ ] **Configurar Nómina:**
  - [ ] Periodicidad
  - [ ] Salario mínimo y UMA
  - [ ] Porcentajes IMSS
- [ ] **Configurar PAC:**
  - [ ] URLs Finkok
  - [ ] Credenciales
  - [ ] Certificados CSD
  - [ ] Probar conexión
- [ ] **Probar:**
  - [ ] Timbrar factura de prueba
  - [ ] Generar reporte de prueba
  - [ ] Crear póliza de prueba

---

## 🎉 ¡Módulo Completo!

El módulo del contador está **100% funcional** y proporciona todas las herramientas necesarias para:

- ✅ Configurar la empresa
- ✅ Administrar contabilidad
- ✅ Gestionar nóminas
- ✅ Configurar facturación electrónica
- ✅ Generar reportes
- ✅ Revisar indicadores en tiempo real

**El sistema ahora es comparable a app.tesk.mx pero integrado completamente con facturación electrónica, nómina y ventas.**
