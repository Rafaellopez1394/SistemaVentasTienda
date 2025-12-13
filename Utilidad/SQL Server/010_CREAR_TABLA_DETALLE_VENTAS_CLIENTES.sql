-- 010_CREAR_TABLA_DETALLE_VENTAS_CLIENTES.sql
-- Crear tabla para el detalle de las ventas a clientes

USE DB_TIENDA;
GO

-- Crear tabla DetalleVentasClientes si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'DetalleVentasClientes')
BEGIN
    CREATE TABLE DetalleVentasClientes (
        DetalleID INT IDENTITY(1,1) PRIMARY KEY,
        VentaID UNIQUEIDENTIFIER NOT NULL,
        ProductoID INT NOT NULL,
        LoteID INT NULL,
        Cantidad INT NOT NULL,
        PrecioUnitario DECIMAL(18,2) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL,
        IVA DECIMAL(18,2) NOT NULL DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL,
        FechaRegistro DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (VentaID) REFERENCES VentasClientes(VentaID),
        FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),
        FOREIGN KEY (LoteID) REFERENCES LotesProducto(LoteID)
    );
    
    PRINT 'Tabla DetalleVentasClientes creada correctamente';
END
ELSE
BEGIN
    PRINT 'Tabla DetalleVentasClientes ya existe';
END
GO
