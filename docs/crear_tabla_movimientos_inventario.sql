-- Script para crear tabla MovimientosInventario si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MovimientosInventario')
BEGIN
    CREATE TABLE MovimientosInventario (
        MovimientoID INT IDENTITY(1,1) PRIMARY KEY,
        LoteID INT NOT NULL,
        ProductoID INT NOT NULL,
        TipoMovimiento VARCHAR(50) NOT NULL, -- AJUSTE_ENTRADA, AJUSTE_SALIDA, MERMA, VENTA, COMPRA
        Cantidad INT NOT NULL,
        CostoUnitario DECIMAL(18,2) NOT NULL,
        Usuario VARCHAR(100) NOT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Comentarios VARCHAR(500),
        
        CONSTRAINT FK_MovimientosInventario_Lote FOREIGN KEY (LoteID) 
            REFERENCES LotesProducto(LoteID),
        CONSTRAINT FK_MovimientosInventario_Producto FOREIGN KEY (ProductoID) 
            REFERENCES Productos(ProductoID)
    );

    CREATE INDEX IX_MovimientosInventario_Lote ON MovimientosInventario(LoteID);
    CREATE INDEX IX_MovimientosInventario_Producto ON MovimientosInventario(ProductoID);
    CREATE INDEX IX_MovimientosInventario_Fecha ON MovimientosInventario(Fecha DESC);
    
    PRINT 'Tabla MovimientosInventario creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla MovimientosInventario ya existe';
END
GO

-- Verificar estructura
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'MovimientosInventario'
ORDER BY ORDINAL_POSITION;
