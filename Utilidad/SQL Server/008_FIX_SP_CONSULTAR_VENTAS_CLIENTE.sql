-- 008_FIX_SP_CONSULTAR_VENTAS_CLIENTE.sql
-- Actualizar stored procedure para usar Monto en lugar de Importe

USE DB_TIENDA;
GO

CREATE PROCEDURE sp_ConsultarVentasCliente
    @ClienteID UNIQUEIDENTIFIER
AS
BEGIN
    SELECT
        V.VentaID,
        V.ClienteID,
        C.RazonSocial,
        V.FechaVenta,
        V.Total,
        V.Estatus,
        V.FechaVencimiento,
        ISNULL(SUM(P.Monto), 0) AS TotalPagado,
        V.Total - ISNULL(SUM(P.Monto), 0) AS SaldoPendiente
    FROM VentasClientes V
    INNER JOIN Clientes C ON V.ClienteID = C.ClienteID
    LEFT JOIN PagosClientes P ON V.VentaID = P.VentaID
    WHERE V.ClienteID = @ClienteID
    GROUP BY V.VentaID, V.ClienteID, C.RazonSocial, V.FechaVenta, V.Total, V.Estatus, V.FechaVencimiento
    ORDER BY V.FechaVenta DESC;
END;
GO

PRINT 'Stored Procedure sp_ConsultarVentasCliente actualizado correctamente';
GO
