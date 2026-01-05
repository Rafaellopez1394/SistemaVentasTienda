-- =========================================================
-- SCRIPT PARA INSERTAR VENTAS DE PRUEBA CON PAGOS PARCIALES
-- =========================================================

USE DB_TIENDA;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Verificar que existan clientes
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE RFC = 'XAXX010101000')
BEGIN
    PRINT 'Creando cliente de prueba...'
    INSERT INTO Clientes (ClienteID, RazonSocial, RFC, CorreoElectronico, Telefono, RegimenFiscalID, CodigoPostal, UsoCFDIID, Estatus, FechaAlta, Usuario)
    VALUES (NEWID(), 'Cliente Prueba Pagos', 'XAXX010101000', 'prueba@test.com', '5555555555', '612', '12345', 'G01', 1, GETDATE(), 'admin');
END

DECLARE @ClienteID UNIQUEIDENTIFIER = (SELECT TOP 1 ClienteID FROM Clientes WHERE RFC = 'XAXX010101000');

PRINT 'Cliente seleccionado: ' + CAST(@ClienteID AS VARCHAR(50));

-- Crear ventas de prueba con diferentes estados de pago
DECLARE @Venta1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Venta2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Venta3 UNIQUEIDENTIFIER = NEWID();

-- Venta 1: Parcialmente pagada (50%)
PRINT 'Insertando Venta 1 (50% pagada)...'
INSERT INTO VentasClientes (VentaID, ClienteID, FechaVenta, Total, MontoPagado, SaldoPendiente, TotalPagado, RequiereFactura, Estatus, Usuario, FechaAlta, TipoVenta, FormaPagoID, MetodoPagoID)
VALUES (@Venta1, @ClienteID, DATEADD(DAY, -5, GETDATE()), 10000.00, 5000.00, 5000.00, 5000.00, 1, 'Activa', 'admin', DATEADD(DAY, -5, GETDATE()), 'Credito', 1, 1);

-- Insertar un pago parcial
INSERT INTO VentaPagos (VentaID, FormaPagoID, MetodoPagoID, Monto, FechaPago, Usuario, FechaAlta)
VALUES (
    @Venta1, 
    1,
    1,
    5000.00, 
    DATEADD(DAY, -4, GETDATE()), 
    'admin', 
    GETDATE()
);

-- Venta 2: Parcialmente pagada (25%)
PRINT 'Insertando Venta 2 (25% pagada)...'
INSERT INTO VentasClientes (VentaID, ClienteID, FechaVenta, Total, MontoPagado, SaldoPendiente, TotalPagado, RequiereFactura, Estatus, Usuario, FechaAlta, TipoVenta, FormaPagoID, MetodoPagoID)
VALUES (@Venta2, @ClienteID, DATEADD(DAY, -3, GETDATE()), 8000.00, 2000.00, 6000.00, 2000.00, 1, 'Activa', 'admin', DATEADD(DAY, -3, GETDATE()), 'Credito', 1, 1);

INSERT INTO VentaPagos (VentaID, FormaPagoID, MetodoPagoID, Monto, FechaPago, Usuario, FechaAlta)
VALUES (
    @Venta2, 
    1,
    1,
    2000.00, 
    DATEADD(DAY, -2, GETDATE()), 
    'admin', 
    GETDATE()
);

-- Venta 3: Sin pagos aún (0%)
PRINT 'Insertando Venta 3 (0% pagada)...'
INSERT INTO VentasClientes (VentaID, ClienteID, FechaVenta, Total, MontoPagado, SaldoPendiente, TotalPagado, RequiereFactura, Estatus, Usuario, FechaAlta, TipoVenta, FormaPagoID, MetodoPagoID)
VALUES (@Venta3, @ClienteID, DATEADD(DAY, -1, GETDATE()), 15000.00, 0.00, 15000.00, 0.00, 1, 'Activa', 'admin', DATEADD(DAY, -1, GETDATE()), 'Credito', 1, 1);

PRINT ''
PRINT '✓ Ventas de prueba insertadas exitosamente'
PRINT '✓ Total: 3 ventas con saldo pendiente'
PRINT '  - Venta 1: $10,000.00 (50% pagado = $5,000.00)'
PRINT '  - Venta 2: $8,000.00 (25% pagado = $2,000.00)'
PRINT '  - Venta 3: $15,000.00 (0% pagado)'
PRINT ''
PRINT 'Verifica en http://localhost:64927/Pagos/GestionarPagosVentas'
GO
