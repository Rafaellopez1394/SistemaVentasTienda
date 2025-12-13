-- 007_TABLA_PAGOS_CLIENTES.sql
-- Tabla para registrar los pagos de ventas a crédito

USE DB_TIENDA;
GO

-- Tabla de Pagos de Clientes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PagosClientes')
BEGIN
    CREATE TABLE PagosClientes (
        PagoID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        VentaID UNIQUEIDENTIFIER NOT NULL,
        ClienteID UNIQUEIDENTIFIER NOT NULL,
        Monto DECIMAL(18,2) NOT NULL,
        FechaPago DATETIME NOT NULL DEFAULT GETDATE(),
        FormaPago VARCHAR(10) NOT NULL, -- Catálogo SAT: 01-Efectivo, 02-Cheque, etc.
        Referencia VARCHAR(100) NULL,
        Comentario VARCHAR(500) NULL,
        GenerarFactura BIT NOT NULL DEFAULT 0,
        GenerarComplemento BIT NOT NULL DEFAULT 0,
        FacturaGenerada BIT NOT NULL DEFAULT 0,
        ComplementoGenerado BIT NOT NULL DEFAULT 0,
        Usuario VARCHAR(50) NOT NULL,
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT FK_PagosClientes_Venta FOREIGN KEY (VentaID) REFERENCES VentasClientes(VentaID),
        CONSTRAINT FK_PagosClientes_Cliente FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID)
    );
    
    PRINT 'Tabla PagosClientes creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla PagosClientes ya existe';
END
GO

-- Stored Procedure para registrar un pago
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_RegistrarPagoCliente')
BEGIN
    DROP PROCEDURE sp_RegistrarPagoCliente;
END
GO

CREATE PROCEDURE sp_RegistrarPagoCliente
    @VentaID UNIQUEIDENTIFIER,
    @ClienteID UNIQUEIDENTIFIER,
    @Monto DECIMAL(18,2),
    @FormaPago VARCHAR(10),
    @Referencia VARCHAR(100) = NULL,
    @Comentario VARCHAR(500) = NULL,
    @GenerarFactura BIT = 0,
    @GenerarComplemento BIT = 0,
    @Usuario VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @PagoID UNIQUEIDENTIFIER = NEWID();
        DECLARE @SaldoPendiente DECIMAL(18,2);
        DECLARE @NuevoSaldo DECIMAL(18,2);
        DECLARE @TotalPagado DECIMAL(18,2);
        
        -- Obtener saldo pendiente actual
        SELECT 
            @SaldoPendiente = ISNULL(SaldoPendiente, Total), 
            @TotalPagado = ISNULL(TotalPagado, 0)
        FROM VentasClientes
        WHERE VentaID = @VentaID;
        
        IF @SaldoPendiente IS NULL
        BEGIN
            RAISERROR('No se encontro la venta especificada', 16, 1);
            RETURN;
        END
        
        -- Validar que el monto no exceda el saldo pendiente
        IF @Monto > @SaldoPendiente
        BEGIN
            RAISERROR('El monto del pago excede el saldo pendiente', 16, 1);
            RETURN;
        END
        
        -- Calcular nuevo saldo
        SET @NuevoSaldo = @SaldoPendiente - @Monto;
        SET @TotalPagado = @TotalPagado + @Monto;
        
        -- Registrar el pago
        INSERT INTO PagosClientes (
            PagoID, VentaID, ClienteID, Monto, FechaPago, FormaPago, 
            Referencia, Comentario, GenerarFactura, GenerarComplemento, 
            Usuario, FechaRegistro
        )
        VALUES (
            @PagoID, @VentaID, @ClienteID, @Monto, GETDATE(), @FormaPago,
            @Referencia, @Comentario, @GenerarFactura, @GenerarComplemento,
            @Usuario, GETDATE()
        );
        
        -- Actualizar el saldo de la venta
        UPDATE VentasClientes
        SET SaldoPendiente = @NuevoSaldo,
            TotalPagado = @TotalPagado,
            Estatus = CASE WHEN @NuevoSaldo = 0 THEN '2' ELSE '1' END, -- 2 = Pagado, 1 = Pendiente
            RequiereFactura = CASE 
                WHEN @GenerarFactura = 1 AND @NuevoSaldo = 0 THEN 1  -- Solo permitir facturar si se pagó todo
                ELSE RequiereFactura 
            END
        WHERE VentaID = @VentaID;
        
        COMMIT TRANSACTION;
        
        -- Mensaje de retorno con información adicional
        DECLARE @MensajeRetorno VARCHAR(500) = 'Pago registrado correctamente';
        
        IF @GenerarFactura = 1 AND @NuevoSaldo = 0
            SET @MensajeRetorno = @MensajeRetorno + '. Venta lista para facturar';
        ELSE IF @GenerarFactura = 1 AND @NuevoSaldo > 0
            SET @MensajeRetorno = @MensajeRetorno + '. La factura se generará cuando se liquide el saldo';
            
        IF @GenerarComplemento = 1
            SET @MensajeRetorno = @MensajeRetorno + '. Complemento de pago registrado';
        
        SELECT @PagoID AS PagoID, @MensajeRetorno AS Mensaje, @NuevoSaldo AS SaldoRestante;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

PRINT 'Stored Procedure sp_RegistrarPagoCliente creado correctamente';
GO
