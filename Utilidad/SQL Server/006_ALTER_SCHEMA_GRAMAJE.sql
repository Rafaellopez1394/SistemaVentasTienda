-- ============================================================
-- ALTER SCHEMA FOR GRAMAJE (By-Weight Sales)
-- ============================================================
USE DB_TIENDA
GO

-- Productos: Configuración de venta por gramaje
IF COL_LENGTH('Productos', 'VentaPorGramaje') IS NULL
BEGIN
    ALTER TABLE Productos ADD VentaPorGramaje BIT NOT NULL DEFAULT 0;
END
IF COL_LENGTH('Productos', 'PrecioPorKilo') IS NULL
BEGIN
    ALTER TABLE Productos ADD PrecioPorKilo DECIMAL(18,2) NULL;
END
IF COL_LENGTH('Productos', 'UnidadMedidaBase') IS NULL
BEGIN
    ALTER TABLE Productos ADD UnidadMedidaBase VARCHAR(10) NULL DEFAULT 'KG';
END
GO

-- LotesProducto: Soporte de inventario por peso (kilogramos)
IF COL_LENGTH('LotesProducto', 'PesoDisponible') IS NULL
BEGIN
    ALTER TABLE LotesProducto ADD PesoDisponible DECIMAL(18,3) NULL DEFAULT 0;
END
GO

-- VentasDetalleClientes: Persistir información de gramaje
IF COL_LENGTH('VentasDetalleClientes', 'Gramos') IS NULL
BEGIN
    ALTER TABLE VentasDetalleClientes ADD Gramos INT NULL;
END
IF COL_LENGTH('VentasDetalleClientes', 'Kilogramos') IS NULL
BEGIN
    ALTER TABLE VentasDetalleClientes ADD Kilogramos DECIMAL(10,3) NULL;
END
IF COL_LENGTH('VentasDetalleClientes', 'PrecioPorKilo') IS NULL
BEGIN
    ALTER TABLE VentasDetalleClientes ADD PrecioPorKilo DECIMAL(18,2) NULL;
END
GO

PRINT 'ALTERS de gramaje aplicados (Productos, LotesProducto, VentasDetalleClientes).'
