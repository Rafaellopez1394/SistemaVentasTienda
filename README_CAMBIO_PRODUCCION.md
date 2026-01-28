# 🚀 CAMBIAR FISCALAPI A PRODUCCIÓN - ÍNDICE COMPLETO

## 📚 Documentos Disponibles

### 🎯 EMPIEZA AQUÍ

**1. [CAMBIAR_A_PRODUCCION_RESUMEN.md](CAMBIAR_A_PRODUCCION_RESUMEN.md)**
   - ⏱️ 5 minutos
   - 📍 Resumen ejecutivo
   - ✅ Lo más importante en pocas líneas
   - 👉 **EMPIEZA AQUÍ SI TIENES POCO TIEMPO**

---

### 📋 DOCUMENTACIÓN DETALLADA

**2. [GUIA_CAMBIAR_A_PRODUCCION.md](GUIA_CAMBIAR_A_PRODUCCION.md)**
   - ⏱️ 30 minutos
   - 📍 Guía paso a paso completa
   - ✅ Todos los pasos explicados
   - 🆘 Sección de Troubleshooting
   - 📞 URLs y contactos importantes
   - 👉 **EMPIEZA AQUÍ SI QUIERES TODO DETALLADO**

**3. [TEST_vs_PRODUCCION_COMPARATIVA.md](TEST_vs_PRODUCCION_COMPARATIVA.md)**
   - ⏱️ 15 minutos
   - 📍 Diferencias técnicas entre ambientes
   - ✅ Visualización de cambios
   - 📊 Diagramas de flujo
   - 👉 **LÉELO PARA ENTENDER QUÉ CAMBIA**

**4. [CHECKLIST_CAMBIO_A_PRODUCCION.md](CHECKLIST_CAMBIO_A_PRODUCCION.md)**
   - ⏱️ Durante ejecución
   - 📍 Checklist paso a paso
   - ✅ Verifica cada punto mientras avanzas
   - 📋 Registro de lo realizado
   - 👉 **USA ESTO MIENTRAS HACES EL CAMBIO**

---

### 🔧 SCRIPTS SQL

**5. [CAMBIAR_A_PRODUCCION.sql](CAMBIAR_A_PRODUCCION.sql)**
   - ⏱️ 1 minuto de ejecución
   - 🔑 **SCRIPT PRINCIPAL**
   - ✅ Actualiza BD de TEST a PRODUCCIÓN
   - ⚠️ REQUIERE: editar ApiKey antes de ejecutar
   - 📍 Base de datos: DB_TIENDA
   - 👉 **EJECUTA ESTE PRIMERO**

**6. [VERIFICAR_AMBIENTE_FISCALAPI.sql](VERIFICAR_AMBIENTE_FISCALAPI.sql)**
   - ⏱️ 30 segundos de ejecución
   - ✅ Verifica estado actual
   - 📊 Muestra TEST o PRODUCCIÓN
   - 🔍 Checklist de configuración
   - 👉 **USA ANTES Y DESPUÉS del cambio**

**7. [VOLVER_A_TEST.sql](VOLVER_A_TEST.sql)**
   - ⏱️ 30 segundos de ejecución
   - ⚠️ Rollback de emergencia
   - 🔄 Vuelve a TEST si algo sale mal
   - ℹ️ Las facturas ya hechas siguen siendo reales
   - 👉 **USA SOLO SI NECESITAS DESHACER EL CAMBIO**

---

## 🎯 Flujo Recomendado Según Tu Situación

### Si Tienes 5 Minutos
1. Lee: `CAMBIAR_A_PRODUCCION_RESUMEN.md`
2. Decide si continuar

### Si Tienes 30 Minutos (Cambio Rápido)
1. Lee: `CAMBIAR_A_PRODUCCION_RESUMEN.md`
2. Obtén API Key de FiscalAPI
3. Edita: `CAMBIAR_A_PRODUCCION.sql`
4. Ejecuta en SQL: `CAMBIAR_A_PRODUCCION.sql`
5. Ejecuta: `VERIFICAR_AMBIENTE_FISCALAPI.sql` (verifica)
6. Compila aplicación
7. Haz primera factura de prueba

### Si Tienes 1-2 Horas (Cambio Seguro)
1. Lee: `TEST_vs_PRODUCCION_COMPARATIVA.md`
2. Lee: `GUIA_CAMBIAR_A_PRODUCCION.md`
3. Completa: `CHECKLIST_CAMBIO_A_PRODUCCION.md`
4. Obtén todos los requisitos
5. Sigue el checklist paso a paso
6. Verifica en SAT al final

---

## 📋 Requisitos Antes de Empezar

- [ ] API Key de producción (sk_live_...)
- [ ] Certificados CSD reales (GAMA6111156JA.cer y .key)
- [ ] Contraseña de llave privada
- [ ] Acceso a FiscalAPI Dashboard
- [ ] Acceso a SQL Server Management Studio
- [ ] Visual Studio con proyecto compilado
- [ ] RFC activo en el SAT
- [ ] Backup de DB_TIENDA

---

## 🔄 Tabla Comparativa Rápida

| Aspecto | TEST | PRODUCCIÓN |
|---------|------|-----------|
| API Key | sk_test_ | sk_live_ |
| URL | test.fiscal... | api.fiscal... |
| Facturas | De prueba | REALES |
| En SAT | NO | SÍ |
| Vigentes | NO | SÍ |
| Riesgo | Bajo | CRÍTICO |

---

## 🚨 Cambios Principales en BD

```sql
UPDATE ConfiguracionPAC
SET 
    EsProduccion = 1,                              -- 0 → 1
    ApiKey = 'sk_live_tuapikeyreal',               -- sk_test_ → sk_live_
    BaseURL = 'https://api.fiscalapi.com'          -- test → api
WHERE ConfigPACID = 1;
```

---

## ✅ Verificación Post-Cambio

```bash
# En SQL Server
EXEC VERIFICAR_AMBIENTE_FISCALAPI.sql
# Debe mostrar: 🔴 PRODUCCIÓN

# En Navegador
http://localhost:PORT/Contador/ConfiguracionPAC
# Debe mostrar: PRODUCCIÓN

# En FiscalAPI Dashboard
https://live.fiscalapi.com/dashboard
# Debe ver tus facturas timbradas
```

---

## 🆘 Quick Troubleshooting

| Error | Solución |
|-------|----------|
| "Invalid API Key" | Copia sin espacios, verifica sk_live_ |
| "Certificate error" | Sube certificados a FiscalAPI Dashboard |
| "Compilation error" | Clean → Rebuild Solution |
| "No data" en SAT | Espera 2-5 minutos, intenta de nuevo |

---

## 📞 Ayuda y Soporte

| Recurso | Contacto |
|---------|----------|
| FiscalAPI Support | support@fiscalapi.com |
| FiscalAPI Docs | https://docs.fiscalapi.com |
| SAT México | https://www.sat.gob.mx |
| SAT Validación | https://prodint.sat.gob.mx |

---

## 📊 Archivos Relacionados en Tu Proyecto

```
SistemaVentasTienda/
├── CAMBIAR_A_PRODUCCION_RESUMEN.md           ← Empieza aquí
├── GUIA_CAMBIAR_A_PRODUCCION.md              ← Detalles completos
├── TEST_vs_PRODUCCION_COMPARATIVA.md         ← Entender cambios
├── CHECKLIST_CAMBIO_A_PRODUCCION.md          ← Mientras lo haces
├── CAMBIAR_A_PRODUCCION.sql                  ← Ejecutar primero
├── VERIFICAR_AMBIENTE_FISCALAPI.sql          ← Verifica antes/después
├── VOLVER_A_TEST.sql                         ← Rollback si falla
└── README_CAMBIO_PRODUCCION.md               ← Este archivo
```

---

## 🎯 Resumen de Pasos

```
1. LEE (CAMBIAR_A_PRODUCCION_RESUMEN.md)
           ↓
2. OBTÉN (API Key + Certificados)
           ↓
3. EDITA (CAMBIAR_A_PRODUCCION.sql)
           ↓
4. EJECUTA (SQL Script en SSMS)
           ↓
5. VERIFICA (VERIFICAR_AMBIENTE_FISCALAPI.sql)
           ↓
6. COMPILA (Rebuild Solution)
           ↓
7. PRUEBA (Primera factura)
           ↓
8. VALIDA (En FiscalAPI + SAT)
           ↓
✅ ¡PRODUCCIÓN ACTIVA!
```

---

## ✨ ¿Qué Lograrás?

✅ Facturas REALES y LEGALES
✅ Vigentes ante el SAT
✅ Consultables por clientes
✅ Con UUID oficial del SAT
✅ Timbradas automáticamente
✅ Seguras y respaldadas

---

## ⚠️ Responsabilidades Después

- Hacer backup diario de BD
- Monitorear facturas diariamente
- Resolver problemas rápidamente
- Mantener certificados vigentes
- Cumplir con SAT y hacienda

---

**¡Listo para empezar? 🚀 Lee CAMBIAR_A_PRODUCCION_RESUMEN.md ahora mismo!**
