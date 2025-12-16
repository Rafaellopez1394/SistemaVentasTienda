/*
    008_AGREGAR_COLUMNA_FACTURADA.sql
    Agrega columnas para rastrear si una venta ya fue facturada
    Fecha: 2025-12-14
*/

USE DB_TIENDA;
GO

PRINT '=== AGREGANDO COLUMNAS PARA CONTROL DE FACTURACION ==='
GO

-- Agregar columna para indicar si la venta ya fue facturada
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('VentasClientes') 
    AND name = 'EstaFacturada'
)
BEGIN
    ALTER TABLE VentasClientes
    ADD EstaFacturada BIT NOT NULL DEFAULT 0;
    
    PRINT '✓ Columna EstaFacturada agregada a VentasClientes'
END
ELSE
    PRINT '○ Columna EstaFacturada ya existe'
GO

-- Agregar columna para almacenar el FacturaID relacionado
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('VentasClientes') 
    AND name = 'FacturaID'
)
BEGIN
    ALTER TABLE VentasClientes
    ADD FacturaID UNIQUEIDENTIFIER NULL;
    
    -- Agregar FK a Facturas
    ALTER TABLE VentasClientes
    ADD CONSTRAINT FK_VentasClientes_Facturas
    FOREIGN KEY (FacturaID) REFERENCES Facturas(FacturaID);
    
    PRINT '✓ Columna FacturaID agregada a VentasClientes con FK'
END
ELSE
    PRINT '○ Columna FacturaID ya existe'
GO

-- Índice para búsquedas rápidas
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_VentasClientes_EstaFacturada'
    AND object_id = OBJECT_ID('VentasClientes')
)
BEGIN
    CREATE INDEX IX_VentasClientes_EstaFacturada 
    ON VentasClientes(EstaFacturada)
    WHERE EstaFacturada = 0;
    
    PRINT '✓ Índice IX_VentasClientes_EstaFacturada creado'
END
GO

-- Stored Procedure: Marcar venta como facturada
IF OBJECT_ID('MarcarVentaFacturada', 'P') IS NOT NULL
    DROP PROCEDURE MarcarVentaFacturada;
GO

CREATE PROCEDURE MarcarVentaFacturada
    @VentaID UNIQUEIDENTIFIER,
    @FacturaID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE VentasClientes
    SET EstaFacturada = 1,
        FacturaID = @FacturaID
    WHERE VentaID = @VentaID;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

PRINT '✓ SP MarcarVentaFacturada creado'
GO

-- Stored Procedure: Obtener ventas pendientes de facturar
IF OBJECT_ID('ObtenerVentasPendientesFacturar', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerVentasPendientesFacturar;
GO

CREATE PROCEDURE ObtenerVentasPendientesFacturar
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL,
    @MontoMinimo DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VentaID,
        v.NumeroDocumento,
        v.FechaVenta,
        v.MontoTotal,
        v.MontoImpuesto,
        c.Nombre AS NombreCliente,
        c.RFC AS RFCCliente,
        c.Email AS EmailCliente,
        u.Nombre AS Vendedor
    FROM VentasClientes v
    LEFT JOIN Cliente c ON v.ClienteID = c.ClienteID
    LEFT JOIN Usuario u ON v.UsuarioID = u.UsuarioID
    WHERE v.EstaFacturada = 0
        AND v.MontoTotal >= @MontoMinimo
        AND (@FechaInicio IS NULL OR v.FechaVenta >= @FechaInicio)
        AND (@FechaFin IS NULL OR v.FechaVenta <= @FechaFin)
    ORDER BY v.FechaVenta DESC;
END
GO

PRINT '✓ SP ObtenerVentasPendientesFacturar creado'
GO

PRINT ''
PRINT '=== MIGRACION 008 COMPLETADA ==='
PRINT '✓ Sistema listo para rastrear facturas por venta'
PRINT '✓ Use MarcarVentaFacturada después de timbrar'
PRINT '✓ Use ObtenerVentasPendientesFacturar para listar ventas sin facturar'
GO
