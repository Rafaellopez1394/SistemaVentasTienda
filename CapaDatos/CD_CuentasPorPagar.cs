using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    public class CD_CuentasPorPagar
    {
        private static CD_CuentasPorPagar _instancia;
        public static CD_CuentasPorPagar Instancia => _instancia ??= new CD_CuentasPorPagar();

        // =============================================
        // REGISTRAR DEUDA (Automático al registrar compra a crédito)
        // =============================================
        public Respuesta RegistrarDeuda(int compraId, int proveedorId, decimal montoTotal, 
            int diasCredito, string folioFactura, int? usuarioId = null)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    var cuentaId = Guid.NewGuid();
                    DateTime fechaVencimiento = DateTime.Now.AddDays(diasCredito);

                    var query = @"
                        INSERT INTO CuentasPorPagar 
                            (CuentaPorPagarID, CompraID, ProveedorID, FechaRegistro, FechaVencimiento, 
                             MontoTotal, SaldoPendiente, Estado, DiasCredito, FolioFactura, 
                             UsuarioCreacion, Activo)
                        VALUES 
                            (@CuentaPorPagarID, @CompraID, @ProveedorID, GETDATE(), @FechaVencimiento,
                             @MontoTotal, @SaldoPendiente, 'PENDIENTE', @DiasCredito, @FolioFactura,
                             @UsuarioCreacion, 1)";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@CuentaPorPagarID", cuentaId);
                    cmd.Parameters.AddWithValue("@CompraID", compraId);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorId);
                    cmd.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                    cmd.Parameters.AddWithValue("@MontoTotal", montoTotal);
                    cmd.Parameters.AddWithValue("@SaldoPendiente", montoTotal);
                    cmd.Parameters.AddWithValue("@DiasCredito", diasCredito);
                    cmd.Parameters.AddWithValue("@FolioFactura", (object)folioFactura ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", (object)usuarioId ?? DBNull.Value);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Cuenta por pagar registrada correctamente";
                    respuesta.Tag = cuentaId;
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = $"Error al registrar cuenta por pagar: {ex.Message}";
            }

            return respuesta;
        }

        // =============================================
        // REGISTRAR PAGO (Con generación de póliza contable)
        // =============================================
        public Respuesta RegistrarPago(RegistrarPagoRequest request)
        {
            Respuesta respuesta = new Respuesta();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();

                try
                {
                    // 1. Obtener info de la cuenta por pagar
                    var queryCuenta = @"
                        SELECT cpp.SaldoPendiente, cpp.MontoTotal, cpp.CompraID, cpp.ProveedorID,
                               p.RazonSocial, cpp.FolioFactura
                        FROM CuentasPorPagar cpp
                        INNER JOIN PROVEEDOR p ON cpp.ProveedorID = p.ProveedorID
                        WHERE cpp.CuentaPorPagarID = @CuentaPorPagarID";

                    SqlCommand cmdCuenta = new SqlCommand(queryCuenta, cnx, transaction);
                    cmdCuenta.Parameters.AddWithValue("@CuentaPorPagarID", request.CuentaPorPagarID);

                    decimal saldoPendiente = 0;
                    decimal montoTotal = 0;
                    int compraId = 0;
                    int proveedorId = 0;
                    string proveedor = "";
                    string folioFactura = "";

                    using (SqlDataReader dr = cmdCuenta.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            saldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]);
                            montoTotal = Convert.ToDecimal(dr["MontoTotal"]);
                            compraId = Convert.ToInt32(dr["CompraID"]);
                            proveedorId = Convert.ToInt32(dr["ProveedorID"]);
                            proveedor = dr["RazonSocial"].ToString();
                            folioFactura = dr["FolioFactura"]?.ToString() ?? "";
                        }
                        else
                        {
                            throw new Exception("Cuenta por pagar no encontrada");
                        }
                    }

                    // Validar monto
                    if (request.MontoPagado > saldoPendiente)
                    {
                        throw new Exception($"El monto a pagar (${request.MontoPagado:N2}) excede el saldo pendiente (${saldoPendiente:N2})");
                    }

                    // 2. Generar póliza de egreso
                    Guid polizaId = GenerarPolizaPago(
                        cnx, transaction, compraId, proveedorId, proveedor, 
                        request.MontoPagado, request.FormaPago, folioFactura);

                    // 3. Registrar el pago
                    var pagoId = Guid.NewGuid();
                    var queryPago = @"
                        INSERT INTO PagosProveedores 
                            (PagoID, CuentaPorPagarID, FechaPago, MontoPagado, FormaPago,
                             Referencia, CuentaBancaria, Observaciones, PolizaID, UsuarioRegistro)
                        VALUES 
                            (@PagoID, @CuentaPorPagarID, GETDATE(), @MontoPagado, @FormaPago,
                             @Referencia, @CuentaBancaria, @Observaciones, @PolizaID, @UsuarioRegistro)";

                    SqlCommand cmdPago = new SqlCommand(queryPago, cnx, transaction);
                    cmdPago.Parameters.AddWithValue("@PagoID", pagoId);
                    cmdPago.Parameters.AddWithValue("@CuentaPorPagarID", request.CuentaPorPagarID);
                    cmdPago.Parameters.AddWithValue("@MontoPagado", request.MontoPagado);
                    cmdPago.Parameters.AddWithValue("@FormaPago", request.FormaPago);
                    cmdPago.Parameters.AddWithValue("@Referencia", (object)request.Referencia ?? DBNull.Value);
                    cmdPago.Parameters.AddWithValue("@CuentaBancaria", (object)request.CuentaBancaria ?? DBNull.Value);
                    cmdPago.Parameters.AddWithValue("@Observaciones", (object)request.Observaciones ?? DBNull.Value);
                    cmdPago.Parameters.AddWithValue("@PolizaID", polizaId);
                    cmdPago.Parameters.AddWithValue("@UsuarioRegistro", (object)request.UsuarioRegistro ?? DBNull.Value);
                    cmdPago.ExecuteNonQuery();

                    // 4. Actualizar saldo pendiente
                    decimal nuevoSaldo = saldoPendiente - request.MontoPagado;
                    var queryUpdateSaldo = @"
                        UPDATE CuentasPorPagar 
                        SET SaldoPendiente = @SaldoPendiente
                        WHERE CuentaPorPagarID = @CuentaPorPagarID";

                    SqlCommand cmdUpdate = new SqlCommand(queryUpdateSaldo, cnx, transaction);
                    cmdUpdate.Parameters.AddWithValue("@SaldoPendiente", nuevoSaldo);
                    cmdUpdate.Parameters.AddWithValue("@CuentaPorPagarID", request.CuentaPorPagarID);
                    cmdUpdate.ExecuteNonQuery();

                    // 5. Actualizar estado
                    var cmdEstado = new SqlCommand("sp_ActualizarEstadoCuenta", cnx, transaction);
                    cmdEstado.CommandType = CommandType.StoredProcedure;
                    cmdEstado.Parameters.AddWithValue("@CuentaPorPagarID", request.CuentaPorPagarID);
                    cmdEstado.ExecuteNonQuery();

                    transaction.Commit();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = $"Pago registrado correctamente. Nuevo saldo: ${nuevoSaldo:N2}";
                    respuesta.Tag = pagoId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Resultado = false;
                    respuesta.Mensaje = $"Error al registrar pago: {ex.Message}";
                }
            }

            return respuesta;
        }

        // =============================================
        // GENERAR PÓLIZA DE PAGO
        // =============================================
        private Guid GenerarPolizaPago(SqlConnection cnx, SqlTransaction transaction,
            int compraId, int proveedorId, string proveedor, decimal monto, 
            string formaPago, string folioFactura)
        {
            Guid polizaId = Guid.NewGuid();

            // Crear póliza de egreso
            var queryPoliza = @"
                INSERT INTO Polizas (PolizaID, TipoPoliza, FechaPoliza, Concepto, Referencia, Activo)
                VALUES (@PolizaID, 'EGRESO', GETDATE(), @Concepto, @Referencia, 1)";

            SqlCommand cmdPoliza = new SqlCommand(queryPoliza, cnx, transaction);
            cmdPoliza.Parameters.AddWithValue("@PolizaID", polizaId);
            cmdPoliza.Parameters.AddWithValue("@Concepto", $"Pago a proveedor {proveedor}");
            cmdPoliza.Parameters.AddWithValue("@Referencia", $"Compra #{compraId} - {folioFactura}");
            cmdPoliza.ExecuteNonQuery();

            // Obtener cuenta contable de Proveedores (PASIVO)
            int cuentaProveedores = ObtenerCuentaContable(cnx, transaction, "PROVEEDORES", "PASIVO");
            
            // Obtener cuenta de Bancos o Caja según forma de pago
            int cuentaBanco = formaPago == "EFECTIVO" 
                ? ObtenerCuentaContable(cnx, transaction, "CAJA", "ACTIVO")
                : ObtenerCuentaContable(cnx, transaction, "BANCOS", "ACTIVO");

            // Asiento: DEBE Proveedores (disminuye pasivo), HABER Bancos/Caja (disminuye activo)
            var queryDetalle = @"
                INSERT INTO PolizasDetalle (PolizaDetalleID, PolizaID, CuentaID, Debe, Haber)
                VALUES 
                    (NEWID(), @PolizaID, @CuentaProveedores, @Monto, 0),
                    (NEWID(), @PolizaID, @CuentaBanco, 0, @Monto)";

            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, cnx, transaction);
            cmdDetalle.Parameters.AddWithValue("@PolizaID", polizaId);
            cmdDetalle.Parameters.AddWithValue("@CuentaProveedores", cuentaProveedores);
            cmdDetalle.Parameters.AddWithValue("@CuentaBanco", cuentaBanco);
            cmdDetalle.Parameters.AddWithValue("@Monto", monto);
            cmdDetalle.ExecuteNonQuery();

            return polizaId;
        }

        private int ObtenerCuentaContable(SqlConnection cnx, SqlTransaction transaction, string nombre, string tipo)
        {
            var query = "SELECT TOP 1 CuentaID FROM CatalogoContable WHERE Nombre LIKE @Nombre AND Tipo = @Tipo AND Activo = 1";
            SqlCommand cmd = new SqlCommand(query, cnx, transaction);
            cmd.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
            cmd.Parameters.AddWithValue("@Tipo", tipo);

            object result = cmd.ExecuteScalar();
            if (result == null)
                throw new Exception($"No se encontró cuenta contable: {nombre} ({tipo})");

            return Convert.ToInt32(result);
        }

        // =============================================
        // OBTENER TODAS LAS CUENTAS POR PAGAR
        // =============================================
        public List<CuentaPorPagar> ObtenerTodas(string estado = null)
        {
            var lista = new List<CuentaPorPagar>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = estado == null 
                    ? "SELECT * FROM vw_ResumenCuentasPorPagar ORDER BY FechaVencimiento"
                    : "SELECT * FROM vw_ResumenCuentasPorPagar WHERE Estado = @Estado ORDER BY FechaVencimiento";

                SqlCommand cmd = new SqlCommand(query, cnx);
                if (estado != null)
                    cmd.Parameters.AddWithValue("@Estado", estado);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CuentaPorPagar
                        {
                            CuentaPorPagarID = Guid.Parse(dr["CuentaPorPagarID"].ToString()),
                            CompraID = Convert.ToInt32(dr["CompraID"]),
                            ProveedorID = Convert.ToInt32(dr["ProveedorID"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            RFC = dr["RFC"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"]),
                            DiasParaVencer = Convert.ToInt32(dr["DiasParaVencer"]),
                            DiasVencido = Convert.ToInt32(dr["DiasVencido"]),
                            MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                            Estado = dr["Estado"].ToString(),
                            FolioFactura = dr["FolioFactura"]?.ToString(),
                            TotalPagado = Convert.ToDecimal(dr["TotalPagado"]),
                            NumeroPagos = Convert.ToInt32(dr["NumeroPagos"])
                        });
                    }
                }
            }

            return lista;
        }

        // =============================================
        // OBTENER POR PROVEEDOR
        // =============================================
        public List<CuentaPorPagar> ObtenerPorProveedor(int proveedorId)
        {
            var lista = new List<CuentaPorPagar>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = "SELECT * FROM vw_ResumenCuentasPorPagar WHERE ProveedorID = @ProveedorID ORDER BY FechaVencimiento";
                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@ProveedorID", proveedorId);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CuentaPorPagar
                        {
                            CuentaPorPagarID = Guid.Parse(dr["CuentaPorPagarID"].ToString()),
                            CompraID = Convert.ToInt32(dr["CompraID"]),
                            ProveedorID = Convert.ToInt32(dr["ProveedorID"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"]),
                            MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                            Estado = dr["Estado"].ToString(),
                            FolioFactura = dr["FolioFactura"]?.ToString()
                        });
                    }
                }
            }

            return lista;
        }

        // =============================================
        // REPORTE ANTIGÜEDAD DE SALDOS
        // =============================================
        public List<ReporteAntiguedadSaldos> GenerarReporteAntiguedad()
        {
            var reporte = new List<ReporteAntiguedadSaldos>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        p.ProveedorID,
                        p.RazonSocial AS Proveedor,
                        p.RFC,
                        SUM(cpp.SaldoPendiente) AS TotalAdeudo,
                        SUM(CASE WHEN DATEDIFF(DAY, GETDATE(), cpp.FechaVencimiento) >= 0 THEN cpp.SaldoPendiente ELSE 0 END) AS Corriente,
                        SUM(CASE WHEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE()) BETWEEN 1 AND 30 THEN cpp.SaldoPendiente ELSE 0 END) AS Dias30,
                        SUM(CASE WHEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE()) BETWEEN 31 AND 60 THEN cpp.SaldoPendiente ELSE 0 END) AS Dias60,
                        SUM(CASE WHEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE()) BETWEEN 61 AND 90 THEN cpp.SaldoPendiente ELSE 0 END) AS Dias90,
                        SUM(CASE WHEN DATEDIFF(DAY, cpp.FechaVencimiento, GETDATE()) > 90 THEN cpp.SaldoPendiente ELSE 0 END) AS Mas120,
                        COUNT(CASE WHEN cpp.Estado = 'VENCIDA' THEN 1 END) AS CuentasVencidas
                    FROM Proveedores p
                    LEFT JOIN CuentasPorPagar cpp ON p.ProveedorID = cpp.ProveedorID AND cpp.SaldoPendiente > 0
                    GROUP BY p.ProveedorID, p.RazonSocial, p.RFC
                    HAVING SUM(cpp.SaldoPendiente) > 0
                    ORDER BY TotalAdeudo DESC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        reporte.Add(new ReporteAntiguedadSaldos
                        {
                            ProveedorID = Convert.ToInt32(dr["ProveedorID"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            RFC = dr["RFC"].ToString(),
                            TotalAdeudo = Convert.ToDecimal(dr["TotalAdeudo"]),
                            Corriente = Convert.ToDecimal(dr["Corriente"]),
                            Dias30 = Convert.ToDecimal(dr["Dias30"]),
                            Dias60 = Convert.ToDecimal(dr["Dias60"]),
                            Dias90 = Convert.ToDecimal(dr["Dias90"]),
                            Mas120 = Convert.ToDecimal(dr["Mas120"]),
                            CuentasVencidas = Convert.ToInt32(dr["CuentasVencidas"])
                        });
                    }
                }
            }

            return reporte;
        }

        // =============================================
        // OBTENER PAGOS DE UNA CUENTA
        // =============================================
        public List<PagoProveedor> ObtenerPagosDeCuenta(Guid cuentaPorPagarId)
        {
            var pagos = new List<PagoProveedor>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                var query = @"
                    SELECT 
                        pp.PagoID, pp.CuentaPorPagarID, pp.FechaPago, pp.MontoPagado,
                        pp.FormaPago, pp.Referencia, pp.CuentaBancaria, pp.Observaciones,
                        pp.PolizaID, pp.FechaRegistro,
                        p.RazonSocial AS Proveedor, cpp.FolioFactura
                    FROM PagosProveedores pp
                    INNER JOIN CuentasPorPagar cpp ON pp.CuentaPorPagarID = cpp.CuentaPorPagarID
                    INNER JOIN PROVEEDOR p ON cpp.ProveedorID = p.ProveedorID
                    WHERE pp.CuentaPorPagarID = @CuentaPorPagarID
                    ORDER BY pp.FechaPago DESC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@CuentaPorPagarID", cuentaPorPagarId);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        pagos.Add(new PagoProveedor
                        {
                            PagoID = Guid.Parse(dr["PagoID"].ToString()),
                            CuentaPorPagarID = Guid.Parse(dr["CuentaPorPagarID"].ToString()),
                            FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                            MontoPagado = Convert.ToDecimal(dr["MontoPagado"]),
                            FormaPago = dr["FormaPago"].ToString(),
                            Referencia = dr["Referencia"]?.ToString(),
                            CuentaBancaria = dr["CuentaBancaria"]?.ToString(),
                            Observaciones = dr["Observaciones"]?.ToString(),
                            PolizaID = dr["PolizaID"] != DBNull.Value ? Guid.Parse(dr["PolizaID"].ToString()) : (Guid?)null,
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            FolioFactura = dr["FolioFactura"]?.ToString()
                        });
                    }
                }
            }

            return pagos;
        }
    }
}
