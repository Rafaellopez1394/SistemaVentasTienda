# ✅ SIMULADOR PAC INSTALADO Y CONFIGURADO

## 🎉 ¡Todo Listo Para Probar!

El **Simulador PAC** está activo y funcionando. Ahora puedes generar facturas de prueba sin costo.

---

## 🚀 Cómo Usar

### 1. **Hacer una Venta**
- Ve al módulo de Punto de Venta (POS)
- Realiza una venta normal
- Marca "Requiere Factura"

### 2. **Generar Factura**
- Ve a Facturación → Generar CFDI 4.0
- Selecciona la venta
- Llena los datos del cliente (RFC, UsoCFDI, CP)
- Click en "Generar y Timbrar"

### 3. **Verificar Resultado**
- Debe aparecer mensaje: **"✅ SIMULACIÓN: Comprobante timbrado exitosamente"**
- La factura aparecerá en Facturas Electrónicas con UUID generado
- El XML tendrá el complemento de timbre

---

## ⚠️ Importante: Facturas NO Válidas

Las facturas generadas con el simulador:
- ❌ **NO son válidas ante el SAT**
- ❌ **NO se pueden deducir fiscalmente**
- ✅ **Sirven SOLO para pruebas y desarrollo**
- ✅ **Tienen formato correcto de CFDI 4.0**

---

## 📋 Características del Simulador

| Característica | Valor |
|----------------|-------|
| **UUIDs** | Válidos en formato pero simulados |
| **Sellos** | Generados aleatoriamente |
| **Certificados** | Números ficticios |
| **Conexión** | No requiere internet |
| **Costo** | $0 (Completamente gratis) |
| **Velocidad** | Instantáneo |

---

## 🔄 Cambiar a Facturama Real

Cuando estés listo para generar facturas reales:

### Paso 1: Comprar Plan de Facturama
1. Ve a https://facturama.mx/planes-facturacion
2. Crea tu cuenta
3. Compra el plan más económico: **$55 MXN por 10 facturas/año**

### Paso 2: Obtener Credenciales
1. Inicia sesión en Facturama
2. Ve a **Configuración → API**
3. Copia tu **Usuario** y **Token/Password**

### Paso 3: Actualizar Sistema
Edita el archivo: `Utilidad/SQL Server/036_ACTUALIZAR_CREDENCIALES_FACTURAMA.sql`

```sql
DECLARE @Usuario VARCHAR(100) = 'TU_USUARIO_REAL';
DECLARE @Password VARCHAR(100) = 'TU_PASSWORD_REAL';
```

Ejecuta el script:
```powershell
sqlcmd -S "SISTEMAS\SERVIDOR" -d "DB_TIENDA" -i "Utilidad\SQL Server\036_ACTUALIZAR_CREDENCIALES_FACTURAMA.sql"
```

### Paso 4: Listo
- Las facturas ahora serán **válidas ante el SAT**
- Se consumirán tus timbres de Facturama
- Aparecerán en el portal del SAT

---

## 🆘 Troubleshooting

### Error al generar factura
- Verifica que el simulador esté activo en ConfiguracionPAC
- Revisa los logs en `VentasWeb\Logs\`

### Quiero volver al simulador
```sql
UPDATE ConfiguracionPAC SET Activo = 0 WHERE ProveedorPAC = 'Facturama';
UPDATE ConfiguracionPAC SET Activo = 1 WHERE ProveedorPAC = 'Simulador';
```

### Ver configuración actual
```sql
SELECT ProveedorPAC, 
       CASE Activo WHEN 1 THEN '✅ ACTIVO' ELSE '❌ INACTIVO' END AS Estado,
       CASE EsProduccion WHEN 0 THEN 'PRUEBAS' ELSE 'PRODUCCIÓN' END AS Modo
FROM ConfiguracionPAC;
```

---

## 📊 Resumen de Costos

| Opción | Costo Anual | Facturas | Costo por Factura |
|--------|-------------|----------|-------------------|
| **Simulador** | **$0** | Ilimitadas | $0 |
| **Facturama 10** | $55 MXN | 10 | $5.50 |
| **Facturama 25** | $110 MXN | 25 | $4.40 |
| **Facturama Ilimitado** | $1,650 MXN | ∞ | $0 |

---

## ✅ Checklist de Prueba

- [ ] Hacer una venta en POS con "Requiere Factura"
- [ ] Ir a Facturación → Generar CFDI 4.0
- [ ] Llenar datos del receptor (RFC, Uso CFDI, CP)
- [ ] Generar y timbrar la factura
- [ ] Verificar que aparece en Facturas Electrónicas
- [ ] Ver que tiene UUID y fecha de timbrado
- [ ] Descargar/Ver el XML generado
- [ ] Verificar el complemento de timbre en el XML

---

**¡Ahora tienes lo mejor de ambos mundos!**
- ✅ **Simulador gratis** para pruebas y desarrollo
- ✅ **Facturama lista** para cuando necesites facturas reales
- ✅ **Cambio en 2 minutos** cuando compres tu plan

🎯 **Recomendación**: Prueba todo con el simulador ahora. Cuando te sientas cómodo y necesites facturas reales, compra el plan de $55 y actualiza las credenciales.
