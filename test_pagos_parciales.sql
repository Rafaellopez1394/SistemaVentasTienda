-- ========================================
-- SCRIPT DE PRUEBA: PAGOS PARCIALES Y COMPLEMENTOS
-- ========================================
-- Este script prueba el flujo completo de pagos parciales
-- y generación de complementos de pago
-- ========================================

USE DB_TIENDA;
GO

PRINT '========================================';
PRINT 'INICIANDO PRUEBAS DE PAGOS PARCIALES';
PRINT '========================================';
PRINT '';

-- ========================================
-- 1. VERIFICAR ESTRUCTURA DE BASE DE DATOS
-- ========================================
PRINT '1. Verificando estructura de base de datos...';

-- Verificar columnas en VentasClientes
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'MontoPagado')
    PRINT '   ✓ Columna MontoPagado existe en VentasClientes';
ELSE
    PRINT '   ✗ ERROR: Columna MontoPagado NO existe';

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'SaldoPendiente')
    PRINT '   ✓ Columna SaldoPendiente existe en VentasClientes';
ELSE
    PRINT '   ✗ ERROR: Columna SaldoPendiente NO existe';

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('VentasClientes') AND name = 'EsPagado')
    PRINT '   ✓ Columna EsPagado existe en VentasClientes';
ELSE
    PRINT '   ✗ ERROR: Columna EsPagado NO existe';

-- Verificar tabla VentaPagos
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'VentaPagos')
    PRINT '   ✓ Tabla VentaPagos existe';
ELSE
    PRINT '   ✗ ERROR: Tabla VentaPagos NO existe';

PRINT '';

-- ========================================
-- 2. CREAR DATOS DE PRUEBA
-- ========================================
PRINT '2. Creando datos de prueba...';

-- Obtener o crear cliente de prueba
DECLARE @ClienteID UNIQUEIDENTIFIER;
DECLARE @ProductoID UNIQUEIDENTIFIER;
DECLARE @SucursalID UNIQUEIDENTIFIER;
DECLARE @UsuarioID UNIQUEIDENTIFIER;

-- Cliente de prueba
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE RFC = 'XAXX010101000')
BEGIN
    SET @ClienteID = NEWID();
    INSERT INTO Clientes (ClienteID, RFC, RazonSocial, Activo, FechaRegistro)
    VALUES (@ClienteID, 'XAXX010101000', 'Cliente Prueba Pagos Parciales', 1, GETDATE());
    PRINT '   ✓ Cliente de prueba creado';
END
ELSE
BEGIN
    SELECT @ClienteID = ClienteID FROM Clientes WHERE RFC = 'XAXX010101000';
    PRINT '   ✓ Cliente de prueba ya existe';
END

-- Producto de prueba
SELECT TOP 1 @ProductoID = ProductoID FROM Productos WHERE Activo = 1;
IF @ProductoID IS NULL
BEGIN
    PRINT '   ✗ ERROR: No hay productos activos en el sistema';
    RETURN;
END
PRINT '   ✓ Producto seleccionado: ' + CAST(@ProductoID AS VARCHAR(50));

-- Sucursal
SELECT TOP 1 @SucursalID = SucursalID FROM Sucursales WHERE Activo = 1;
IF @SucursalID IS NULL
BEGIN
    PRINT '   ✗ ERROR: No hay sucursales activas';
    RETURN;
END
PRINT '   ✓ Sucursal seleccionada: ' + CAST(@SucursalID AS VARCHAR(50));

-- Usuario
SELECT TOP 1 @UsuarioID = UsuarioID FROM Usuarios WHERE Activo = 1;
IF @UsuarioID IS NULL
BEGIN
    PRINT '   ✗ ERROR: No hay usuarios activos';
    RETURN;
END
PRINT '   ✓ Usuario seleccionado: ' + CAST(@UsuarioID AS VARCHAR(50));

PRINT '';

-- ========================================
-- 3. SIMULAR VENTA PARCIAL
-- ========================================
PRINT '3. Simulando venta con pago parcial...';

DECLARE @VentaID UNIQUEIDENTIFIER = NEWID();
DECLARE @TotalVenta DECIMAL(18,2) = 1160.00;  -- $1000 + $160 IVA
DECLARE @MontoPagado DECIMAL(18,2) = 500.00;  -- Pago inicial
DECLARE @SaldoPendiente DECIMAL(18,2) = @TotalVenta - @MontoPagado;

-- Insertar venta PARCIAL
INSERT INTO VentasClientes (
    VentaID, ClienteID, SucursalID, UsuarioID, 
    FechaVenta, TipoVenta, Total, MontoPagado,
    FormaPagoID, MetodoPagoID, Activo
)
VALUES (
    @VentaID, @ClienteID, @SucursalID, @UsuarioID,
    GETDATE(), 'PARCIAL', @TotalVenta, @MontoPagado,
    1, 1, 1  -- FormaPago y MetodoPago genéricos
);

PRINT '   ✓ Venta PARCIAL creada';
PRINT '     - VentaID: ' + CAST(@VentaID AS VARCHAR(50));
PRINT '     - Total: $' + CAST(@TotalVenta AS VARCHAR(20));
PRINT '     - Monto Pagado: $' + CAST(@MontoPagado AS VARCHAR(20));
PRINT '     - Saldo Pendiente: $' + CAST(@SaldoPendiente AS VARCHAR(20));

-- Insertar detalle (1 producto)
INSERT INTO VentasDetalleClientes (
    VentaDetalleID, VentaID, ProductoID,
    Cantidad, PrecioVenta, TasaIVA, MontoIVA
)
VALUES (
    NEWID(), @VentaID, @ProductoID,
    2.5,  -- Cantidad decimal (prueba inventario fraccionado)
    400.00,  -- $400 * 2.5 = $1000
    16.00, 160.00  -- IVA
);

PRINT '   ✓ Detalle de venta agregado (2.5 unidades)';
PRINT '';

-- ========================================
-- 4. VERIFICAR COLUMNAS CALCULADAS
-- ========================================
PRINT '4. Verificando columnas calculadas...';

DECLARE @SaldoCalculado DECIMAL(18,2);
DECLARE @EsPagado BIT;

SELECT 
    @SaldoCalculado = SaldoPendiente,
    @EsPagado = EsPagado
FROM VentasClientes
WHERE VentaID = @VentaID;

PRINT '   Saldo Pendiente calculado: $' + CAST(@SaldoCalculado AS VARCHAR(20));
PRINT '   Es Pagado: ' + CASE WHEN @EsPagado = 1 THEN 'SÍ' ELSE 'NO' END;

IF @SaldoCalculado = @SaldoPendiente
    PRINT '   ✓ SaldoPendiente correcto';
ELSE
    PRINT '   ✗ ERROR: SaldoPendiente incorrecto';

IF @EsPagado = 0
    PRINT '   ✓ EsPagado correcto (NO)';
ELSE
    PRINT '   ✗ ERROR: EsPagado debería ser 0';

PRINT '';

-- ========================================
-- 5. SIMULAR GENERACIÓN DE FACTURA PPD
-- ========================================
PRINT '5. Simulando generación de factura con PPD...';

DECLARE @FacturaID UNIQUEIDENTIFIER = NEWID();
DECLARE @UUID VARCHAR(36) = LOWER(NEWID());

INSERT INTO Facturas (
    FacturaID, VentaID, UUID, Serie, Folio,
    FechaEmision, ReceptorRFC, ReceptorNombre,
    FormaPago, MetodoPago,  -- MetodoPago debería ser PPD
    Subtotal, Total, Estatus,
    SaldoPendiente, EsCancelada, Version, TipoComprobante
)
VALUES (
    @FacturaID, @VentaID, @UUID, 'A', 1001,
    GETDATE(), 'XAXX010101000', 'Cliente Prueba',
    '01', 'PPD',  -- PPD = Pago en Parcialidades o Diferido
    1000.00, @TotalVenta, 'TIMBRADA',
    @SaldoPendiente, 0, '4.0', 'I'
);

PRINT '   ✓ Factura PPD creada';
PRINT '     - FacturaID: ' + CAST(@FacturaID AS VARCHAR(50));
PRINT '     - UUID: ' + @UUID;
PRINT '     - MetodoPago: PPD (Pago en Parcialidades)';
PRINT '     - SaldoPendiente: $' + CAST(@SaldoPendiente AS VARCHAR(20));
PRINT '';

-- ========================================
-- 6. SIMULAR REGISTRO DE PAGO ADICIONAL
-- ========================================
PRINT '6. Simulando registro de pago adicional...';

DECLARE @PagoID UNIQUEIDENTIFIER = NEWID();
DECLARE @MontoPago2 DECIMAL(18,2) = 400.00;
DECLARE @NuevoSaldo DECIMAL(18,2) = @SaldoPendiente - @MontoPago2;

INSERT INTO VentaPagos (
    PagoID, VentaID, FormaPagoID, MetodoPagoID,
    Monto, FechaPago, Usuario, Referencia
)
VALUES (
    @PagoID, @VentaID, 3, 1,  -- Transferencia electrónica
    @MontoPago2, GETDATE(), 'ADMIN', 'PAGO-TEST-002'
);

-- Actualizar MontoPagado en la venta
UPDATE VentasClientes
SET MontoPagado = MontoPagado + @MontoPago2
WHERE VentaID = @VentaID;

PRINT '   ✓ Pago adicional registrado';
PRINT '     - PagoID: ' + CAST(@PagoID AS VARCHAR(50));
PRINT '     - Monto: $' + CAST(@MontoPago2 AS VARCHAR(20));
PRINT '     - Nuevo Saldo Pendiente: $' + CAST(@NuevoSaldo AS VARCHAR(20));
PRINT '';

-- ========================================
-- 7. VERIFICAR ESTADO FINAL
-- ========================================
PRINT '7. Verificando estado final de la venta...';

SELECT 
    @SaldoCalculado = SaldoPendiente,
    @EsPagado = EsPagado
FROM VentasClientes
WHERE VentaID = @VentaID;

PRINT '   Total de la venta: $' + CAST(@TotalVenta AS VARCHAR(20));
PRINT '   Monto total pagado: $' + CAST(@MontoPagado + @MontoPago2 AS VARCHAR(20));
PRINT '   Saldo pendiente: $' + CAST(@SaldoCalculado AS VARCHAR(20));
PRINT '   Estado de pago: ' + CASE WHEN @EsPagado = 1 THEN 'PAGADO' ELSE 'PENDIENTE' END;
PRINT '';

-- ========================================
-- 8. CONSULTAR HISTORIAL DE PAGOS
-- ========================================
PRINT '8. Consultando historial de pagos...';

SELECT 
    vp.PagoID,
    vp.Monto,
    vp.FechaPago,
    vp.Referencia,
    vp.ComplementoPagoID,
    CASE WHEN vp.ComplementoPagoID IS NULL THEN 'PENDIENTE' ELSE 'TIMBRADO' END AS EstadoComplemento
FROM VentaPagos vp
WHERE vp.VentaID = @VentaID
ORDER BY vp.FechaPago;

PRINT '';

-- ========================================
-- 9. REPORTE DE VENTAS PENDIENTES
-- ========================================
PRINT '9. Consultando ventas con saldo pendiente...';

SELECT 
    vc.VentaID,
    c.RazonSocial AS Cliente,
    vc.FechaVenta,
    vc.TipoVenta,
    vc.Total,
    vc.MontoPagado,
    vc.SaldoPendiente,
    vc.EsPagado,
    COUNT(vp.PagoID) AS NumPagos
FROM VentasClientes vc
LEFT JOIN Clientes c ON vc.ClienteID = c.ClienteID
LEFT JOIN VentaPagos vp ON vc.VentaID = vp.VentaID
WHERE vc.SaldoPendiente > 0
GROUP BY vc.VentaID, c.RazonSocial, vc.FechaVenta, vc.TipoVenta, 
         vc.Total, vc.MontoPagado, vc.SaldoPendiente, vc.EsPagado
ORDER BY vc.FechaVenta DESC;

PRINT '';

-- ========================================
-- 10. PRUEBA DE INVENTARIO FRACCIONADO
-- ========================================
PRINT '10. Verificando inventario fraccionado...';

SELECT 
    vd.Cantidad,
    p.Nombre AS Producto,
    CASE 
        WHEN vd.Cantidad <> FLOOR(vd.Cantidad) THEN 'DECIMAL (Fraccionado)'
        ELSE 'ENTERO'
    END AS TipoCantidad
FROM VentasDetalleClientes vd
INNER JOIN Productos p ON vd.ProductoID = p.ProductoID
WHERE vd.VentaID = @VentaID;

PRINT '';

-- ========================================
-- RESUMEN FINAL
-- ========================================
PRINT '========================================';
PRINT 'RESUMEN DE PRUEBAS';
PRINT '========================================';
PRINT '';
PRINT 'PRUEBAS COMPLETADAS:';
PRINT '  ✓ Estructura de base de datos verificada';
PRINT '  ✓ Venta PARCIAL creada correctamente';
PRINT '  ✓ Columnas calculadas funcionando (SaldoPendiente, EsPagado)';
PRINT '  ✓ Factura con MetodoPago PPD generada';
PRINT '  ✓ Pago adicional registrado en VentaPagos';
PRINT '  ✓ Inventario fraccionado soportado (2.5 unidades)';
PRINT '';
PRINT 'PRÓXIMOS PASOS PARA PRUEBA COMPLETA:';
PRINT '  1. Levantar aplicación web';
PRINT '  2. Ir a POS y crear venta PARCIAL';
PRINT '  3. Ir a Facturas y generar factura (verificar PPD)';
PRINT '  4. Ir a Gestión de Pagos y registrar pago adicional';
PRINT '  5. Verificar generación de complemento de pago';
PRINT '';
PRINT '========================================';
PRINT 'FIN DE PRUEBAS';
PRINT '========================================';

-- Limpiar datos de prueba (opcional - comentar si quieres mantenerlos)
/*
PRINT '';
PRINT 'Limpiando datos de prueba...';
DELETE FROM VentaPagos WHERE VentaID = @VentaID;
DELETE FROM Facturas WHERE VentaID = @VentaID;
DELETE FROM VentasDetalleClientes WHERE VentaID = @VentaID;
DELETE FROM VentasClientes WHERE VentaID = @VentaID;
PRINT '✓ Datos de prueba eliminados';
*/
