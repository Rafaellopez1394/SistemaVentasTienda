// CapaDatos/CD_Cliente.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Cliente
    {
        private static CD_Cliente _instancia;
        public static CD_Cliente Instancia => _instancia ??= new CD_Cliente();

        private CD_Cliente() { }

        // ==================================================================
        // 1. LISTAR TODOS LOS CLIENTES (con descripciones de catálogos)
        // ==================================================================
        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultarClientesTodos", cnx) 
                { 
                    CommandType = CommandType.StoredProcedure 
                };
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Cliente
                        {
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"].ToString(),
                            RegimenFiscal = dr["RegimenFiscal"].ToString(),
                            CodigoPostal = dr["CodigoPostal"].ToString(),
                            UsoCFDIID = dr["UsoCFDIID"].ToString(),
                            UsoCFDI = dr["UsoCFDI"].ToString(),
                            CorreoElectronico = dr["CorreoElectronico"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 2. BUSCAR CLIENTE POR RFC
        // ==================================================================
        public Cliente ObtenerPorRFC(string rfc)
        {
            Cliente c = null;
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultarClientePorRFC", cnx) 
                { 
                    CommandType = CommandType.StoredProcedure 
                };
                cmd.Parameters.AddWithValue("@RFC", rfc);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        c = new Cliente
                        {
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"].ToString(),
                            RegimenFiscal = dr["RegimenFiscal"].ToString(),
                            CodigoPostal = dr["CodigoPostal"].ToString(),
                            UsoCFDIID = dr["UsoCFDIID"].ToString(),
                            UsoCFDI = dr["UsoCFDI"].ToString(),
                            CorreoElectronico = dr["CorreoElectronico"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        };
                    }
                }
            }
            return c;
        }

        // ==================================================================
        // 3. BUSCAR CLIENTES POR NOMBRE O RAZÓN SOCIAL
        // ==================================================================
        public List<Cliente> BuscarPorNombre(string nombre)
        {
            var lista = new List<Cliente>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultarClientePorNombre", cnx) 
                { 
                    CommandType = CommandType.StoredProcedure 
                };
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Cliente
                        {
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"].ToString(),
                            RegimenFiscal = dr["RegimenFiscal"].ToString(),
                            CodigoPostal = dr["CodigoPostal"].ToString(),
                            UsoCFDIID = dr["UsoCFDIID"].ToString(),
                            UsoCFDI = dr["UsoCFDI"].ToString(),
                            CorreoElectronico = dr["CorreoElectronico"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 4. ALTA DE CLIENTE
        // ==================================================================
        public bool AltaCliente(Cliente c)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("AltaCliente", cnx) 
                { 
                    CommandType = CommandType.StoredProcedure 
                };
                cmd.Parameters.AddWithValue("@RFC", c.RFC);
                cmd.Parameters.AddWithValue("@RazonSocial", c.RazonSocial);
                cmd.Parameters.AddWithValue("@RegimenFiscalID", c.RegimenFiscalID);
                cmd.Parameters.AddWithValue("@CodigoPostal", c.CodigoPostal);
                cmd.Parameters.AddWithValue("@UsoCFDIID", c.UsoCFDIID);
                cmd.Parameters.AddWithValue("@CorreoElectronico", c.CorreoElectronico);
                cmd.Parameters.AddWithValue("@Telefono", (object)c.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", c.Usuario ?? "system");

                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        // ==================================================================
        // 5. ACTUALIZAR CLIENTE
        // ==================================================================
        public bool ActualizarCliente(Cliente c)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ActualizarCliente", cnx) 
                { 
                    CommandType = CommandType.StoredProcedure 
                };
                cmd.Parameters.AddWithValue("@ClienteID", c.ClienteID);
                cmd.Parameters.AddWithValue("@RFC", c.RFC);
                cmd.Parameters.AddWithValue("@RazonSocial", c.RazonSocial);
                cmd.Parameters.AddWithValue("@RegimenFiscalID", c.RegimenFiscalID);
                cmd.Parameters.AddWithValue("@CodigoPostal", c.CodigoPostal);
                cmd.Parameters.AddWithValue("@UsoCFDIID", c.UsoCFDIID);
                cmd.Parameters.AddWithValue("@CorreoElectronico", c.CorreoElectronico);
                cmd.Parameters.AddWithValue("@Telefono", (object)c.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", c.Usuario ?? "system");

                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        // ==================================================================
        // 6. OBTENER TODOS LOS TIPOS DE CRÉDITO DEL CLIENTE (TU DISEÑO REAL)
        // ==================================================================
        public List<ClienteTipoCredito> ObtenerTiposCredito(Guid clienteId)
        {
            var lista = new List<ClienteTipoCredito>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultarClienteTiposCreditoPorCliente", cnx)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClienteTipoCredito
                        {
                            ClienteTipoCreditoID = Convert.ToInt32(dr["ClienteTipoCreditoID"]),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            TipoCreditoID = Convert.ToInt32(dr["TipoCreditoID"]),
                            Codigo = dr["Codigo"].ToString(),
                            TipoCredito = dr["TipoCredito"].ToString(),
                            Criterio = dr["Criterio"].ToString(),
                            LimiteProducto = dr["LimiteProducto"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["LimiteProducto"]),
                            LimiteDinero = dr["LimiteDinero"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr["LimiteDinero"]),
                            PlazoDias = dr["PlazoDias"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["PlazoDias"]),
                            FechaAsignacion = Convert.ToDateTime(dr["FechaAsignacion"]),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 7. BUSCAR CLIENTE POR NOMBRE O RFC (para autocompletado en ventas)
        // ==================================================================
        public List<Cliente> BuscarPorNombreOCRFC(string texto)
        {
            var lista = new List<Cliente>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TOP(30) 
                        ClienteID, RFC, RazonSocial, RegimenFiscalID, CodigoPostal, UsoCFDIID,
                        CorreoElectronico, Telefono
                    FROM Clientes
                    WHERE Estatus = 1
                      AND (RFC LIKE @texto OR RazonSocial LIKE @texto)
                    ORDER BY RazonSocial";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@texto", $"%{texto}%");

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Cliente
                        {
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"] == DBNull.Value ? null : dr["RegimenFiscalID"].ToString(),
                            CodigoPostal = dr["CodigoPostal"] == DBNull.Value ? null : dr["CodigoPostal"].ToString(),
                            UsoCFDIID = dr["UsoCFDIID"] == DBNull.Value ? null : dr["UsoCFDIID"].ToString(),
                            CorreoElectronico = dr["CorreoElectronico"] == DBNull.Value ? null : dr["CorreoElectronico"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"].ToString(),
                            Estatus = true
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 8. OBTENER CLIENTE POR ID (opcional, útil en validaciones)
        // ==================================================================
        public Cliente ObtenerPorId(Guid clienteId)
        {
            Cliente cliente = null;
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT ClienteID, RFC, RazonSocial, RegimenFiscalID, CodigoPostal, UsoCFDIID, 
                           CorreoElectronico, Telefono, Estatus, FechaAlta, Usuario, UltimaAct
                    FROM Clientes WHERE ClienteID = @id", cnx);
                cmd.Parameters.AddWithValue("@id", clienteId);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente = new Cliente
                        {
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"] == DBNull.Value ? null : dr["RegimenFiscalID"].ToString(),
                            CodigoPostal = dr["CodigoPostal"] == DBNull.Value ? null : dr["CodigoPostal"].ToString(),
                            UsoCFDIID = dr["UsoCFDIID"] == DBNull.Value ? null : dr["UsoCFDIID"].ToString(),
                            CorreoElectronico = dr["CorreoElectronico"] == DBNull.Value ? null : dr["CorreoElectronico"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                            Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        };
                    }
                }
            }
            return cliente;
        }
        // ===================================================
        // MÉTODO OFICIAL Y ÚNICO PARA OBTENER CRÉDITOS DEL CLIENTE
        // ===================================================
        public List<ClienteTipoCredito> ObtenerTiposCreditoCliente(Guid clienteId)
        {
            var lista = new List<ClienteTipoCredito>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultarClienteTiposCreditoPorCliente", cnx)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClienteTipoCredito
                        {
                            ClienteTipoCreditoID = Convert.ToInt32(dr["ClienteTipoCreditoID"]),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            TipoCreditoID = Convert.ToInt32(dr["TipoCreditoID"]),
                            Codigo = dr["Codigo"].ToString(),
                            TipoCredito = dr["TipoCredito"].ToString(),
                            Criterio = dr["Criterio"].ToString(),
                            LimiteProducto = dr["LimiteProducto"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["LimiteProducto"]),
                            LimiteDinero = dr["LimiteDinero"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr["LimiteDinero"]),
                            PlazoDias = dr["PlazoDias"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["PlazoDias"]),
                            FechaAsignacion = Convert.ToDateTime(dr["FechaAsignacion"]),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 8. OBTENER SALDO ACTUAL DEL CLIENTE (suma de deudas pendientes)
        // ==================================================================
        public decimal ObtenerSaldoActual(Guid clienteId)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ISNULL(SUM(Saldo), 0) AS TotalSaldo
                      FROM VentasCredito
                      WHERE ClienteID = @ClienteID AND Estatus != 'CANCELADA' AND Saldo > 0", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        // ==================================================================
        // 9. OBTENER SALDO VENCIDO (deudas con plazo cumplido)
        // ==================================================================
        public decimal ObtenerSaldoVencido(Guid clienteId)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ISNULL(SUM(vc.Saldo), 0) AS SaldoVencido
                      FROM VentasCredito vc
                      WHERE vc.ClienteID = @ClienteID 
                        AND vc.Estatus != 'CANCELADA' 
                        AND vc.Saldo > 0
                        AND DATEDIFF(DAY, vc.FechaVencimiento, GETDATE()) > 0", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        // ==================================================================
        // 10. OBTENER DÍAS VENCIDOS (máximo de días pasado vencimiento)
        // ==================================================================
        public int ObtenerDiasVencidos(Guid clienteId)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ISNULL(MAX(DATEDIFF(DAY, FechaVencimiento, GETDATE())), 0) AS DiasVencidos
                      FROM VentasCredito
                      WHERE ClienteID = @ClienteID 
                        AND Estatus != 'CANCELADA' 
                        AND Saldo > 0
                        AND DATEDIFF(DAY, FechaVencimiento, GETDATE()) > 0", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        // ==================================================================
        // 11. OBTENER LÍMITE DE CRÉDITO TOTAL (suma de todos los límites activos)
        // ==================================================================
        public decimal ObtenerLimiteCreditoTotal(Guid clienteId)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ISNULL(SUM(LimiteDinero), 0) AS TotalLimite
                      FROM ClienteTiposCredito
                      WHERE ClienteID = @ClienteID AND Estatus = 1 AND LimiteDinero IS NOT NULL", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        // ==================================================================
        // 12. OBTENER CRÉDITO DISPONIBLE (límite - saldo actual)
        // ==================================================================
        public decimal ObtenerCreditoDisponible(Guid clienteId)
        {
            decimal limite = ObtenerLimiteCreditoTotal(clienteId);
            decimal saldo = ObtenerSaldoActual(clienteId);
            return Math.Max(0, limite - saldo);
        }

        // ==================================================================
        // 13. VALIDAR SI CLIENTE PUEDE COMPRAR A CRÉDITO
        // ==================================================================
        public bool PuedeCreditoDisponible(Guid clienteId, decimal montoVenta)
        {
            decimal disponible = ObtenerCreditoDisponible(clienteId);
            return disponible >= montoVenta;
        }

        // ==================================================================
        // 14. OBTENER HISTORIAL DE CRÉDITO (últimas 10 compras a crédito)
        // ==================================================================
        public List<VentaCliente> ObtenerHistorialCredito(Guid clienteId, int top = 10)
        {
            var lista = new List<VentaCliente>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT TOP (@Top) VentaID, ClienteID, Total, Saldo, FechaVenta, FechaVencimiento, Estatus
                      FROM VentasCredito
                      WHERE ClienteID = @ClienteID
                      ORDER BY FechaVenta DESC", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cmd.Parameters.AddWithValue("@Top", top);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new VentaCliente
                        {
                            VentaID = Guid.Parse(dr["VentaID"].ToString()),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            Total = Convert.ToDecimal(dr["Total"]),
                            SaldoPendiente = dr["Saldo"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Saldo"]),
                            FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                            FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["FechaVencimiento"]),
                            Estatus = dr["Estatus"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}