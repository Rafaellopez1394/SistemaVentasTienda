using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_TipoCredito
    {
        private static CD_TipoCredito _instancia;
        public static CD_TipoCredito Instancia => _instancia ??= new CD_TipoCredito();
        private CD_TipoCredito() { }

        // ==================================================================
        // 1. OBTENER TODOS LOS TIPOS DE CRÉDITO DISPONIBLES
        // ==================================================================
        public List<TipoCredito> ObtenerTodos()
        {
            var lista = new List<TipoCredito>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT TipoCreditoID, Codigo, Nombre, Descripcion, Criterio, Icono, Activo, 
                             Usuario, FechaCreacion, UltimaAct
                      FROM TiposCredito
                      ORDER BY Codigo", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TipoCredito
                        {
                            TipoCreditoID = Convert.ToInt32(dr["TipoCreditoID"]),
                            Codigo = dr["Codigo"] == DBNull.Value ? null : dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"] == DBNull.Value ? null : dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"].ToString(),
                            Criterio = dr["Criterio"] == DBNull.Value ? null : dr["Criterio"].ToString(),
                            Icono = dr["Icono"] == DBNull.Value ? null : dr["Icono"].ToString(),
                            Activo = dr["Activo"] == DBNull.Value ? false : Convert.ToBoolean(dr["Activo"]),
                            Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"].ToString(),
                            FechaCreacion = dr["FechaCreacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaCreacion"]),
                            UltimaAct = dr["UltimaAct"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 2. OBTENER TIPO DE CRÉDITO POR ID
        // ==================================================================
        public TipoCredito ObtenerPorId(int tipoCreditoId)
        {
            TipoCredito tipo = null;
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT TipoCreditoID, Codigo, Nombre, Descripcion, Criterio, Icono, Activo, 
                             Usuario, FechaCreacion, UltimaAct
                      FROM TiposCredito
                      WHERE TipoCreditoID = @TipoCreditoID", cnx);
                cmd.Parameters.AddWithValue("@TipoCreditoID", tipoCreditoId);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tipo = new TipoCredito
                        {
                            TipoCreditoID = Convert.ToInt32(dr["TipoCreditoID"]),
                            Codigo = dr["Codigo"] == DBNull.Value ? null : dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"] == DBNull.Value ? null : dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"].ToString(),
                            Criterio = dr["Criterio"] == DBNull.Value ? null : dr["Criterio"].ToString(),
                            Icono = dr["Icono"] == DBNull.Value ? null : dr["Icono"].ToString(),
                            Activo = dr["Activo"] == DBNull.Value ? false : Convert.ToBoolean(dr["Activo"]),
                            Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"].ToString(),
                            FechaCreacion = dr["FechaCreacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaCreacion"]),
                            UltimaAct = dr["UltimaAct"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["UltimaAct"])
                        };
                    }
                }
            }
            return tipo;
        }

        // ==================================================================
        // 3. OBTENER CRÉDITOS ASIGNADOS A UN CLIENTE
        // ==================================================================
        public List<CreditoClienteInfo> ObtenerCreditosCliente(Guid clienteId)
        {
            var lista = new List<CreditoClienteInfo>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT ctc.ClienteTipoCreditoID, ctc.ClienteID, c.RazonSocial,
                             ctc.TipoCreditoID, tc.Nombre AS TipoCredito, tc.Criterio,
                             ctc.LimiteDinero, ctc.LimiteProducto, ctc.PlazoDias,
                             ctc.FechaAsignacion, ctc.Estatus, ctc.Usuario, ctc.UltimaAct
                      FROM ClienteTiposCredito ctc
                      INNER JOIN Clientes c ON ctc.ClienteID = c.ClienteID
                      INNER JOIN TiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
                      WHERE ctc.ClienteID = @ClienteID
                      ORDER BY tc.Codigo", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CreditoClienteInfo
                        {
                            ClienteTipoCreditoID = Convert.ToInt32(dr["ClienteTipoCreditoID"]),
                            ClienteID = Guid.Parse(dr["ClienteID"].ToString()),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            TipoCreditoID = Convert.ToInt32(dr["TipoCreditoID"]),
                            TipoCredito = dr["TipoCredito"].ToString(),
                            Criterio = dr["Criterio"].ToString(),
                            LimiteDinero = dr["LimiteDinero"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr["LimiteDinero"]),
                            LimiteProducto = dr["LimiteProducto"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["LimiteProducto"]),
                            PlazoDias = dr["PlazoDias"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["PlazoDias"]),
                            FechaAsignacion = Convert.ToDateTime(dr["FechaAsignacion"]),
                            Activo = Convert.ToBoolean(dr["Estatus"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        // ==================================================================
        // 4. ASIGNAR TIPO DE CRÉDITO A CLIENTE
        // ==================================================================
        public bool AsignarCreditoCliente(Guid clienteId, int tipoCreditoId, 
            decimal? limiteDinero = null, int? limiteProducto = null, int? plazoDias = null)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO ClienteTiposCredito 
                      (ClienteID, TipoCreditoID, LimiteDinero, LimiteProducto, PlazoDias, 
                       FechaAsignacion, Estatus, Usuario, UltimaAct)
                      VALUES (@ClienteID, @TipoCreditoID, @LimiteDinero, @LimiteProducto, 
                              @PlazoDias, GETDATE(), 1, @Usuario, GETDATE())", cnx);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cmd.Parameters.AddWithValue("@TipoCreditoID", tipoCreditoId);
                cmd.Parameters.AddWithValue("@LimiteDinero", (object)limiteDinero ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LimiteProducto", (object)limiteProducto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PlazoDias", (object)plazoDias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", "system");
                cnx.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================================================================
        // 5. ACTUALIZAR CRÉDITO ASIGNADO A CLIENTE
        // ==================================================================
        public bool ActualizarCreditoCliente(int clienteTipoCreditoId, 
            decimal? limiteDinero = null, int? limiteProducto = null, int? plazoDias = null, bool? activo = null)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string sql = @"UPDATE ClienteTiposCredito SET UltimaAct = GETDATE(), Usuario = @Usuario";
                if (limiteDinero.HasValue) sql += ", LimiteDinero = @LimiteDinero";
                if (limiteProducto.HasValue) sql += ", LimiteProducto = @LimiteProducto";
                if (plazoDias.HasValue) sql += ", PlazoDias = @PlazoDias";
                if (activo.HasValue) sql += ", Estatus = @Estatus";
                sql += " WHERE ClienteTipoCreditoID = @ClienteTipoCreditoID";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@ClienteTipoCreditoID", clienteTipoCreditoId);
                cmd.Parameters.AddWithValue("@Usuario", "system");
                
                if (limiteDinero.HasValue) cmd.Parameters.AddWithValue("@LimiteDinero", limiteDinero.Value);
                if (limiteProducto.HasValue) cmd.Parameters.AddWithValue("@LimiteProducto", limiteProducto.Value);
                if (plazoDias.HasValue) cmd.Parameters.AddWithValue("@PlazoDias", plazoDias.Value);
                if (activo.HasValue) cmd.Parameters.AddWithValue("@Estatus", activo.Value);
                
                cnx.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================================================================
        // 6. SUSPENDER / REACTIVAR CRÉDITO DE CLIENTE
        // ==================================================================
        public bool SuspenderCredito(int clienteTipoCreditoId, bool suspender = true)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE ClienteTiposCredito 
                      SET Estatus = @Estatus, UltimaAct = GETDATE(), Usuario = @Usuario
                      WHERE ClienteTipoCreditoID = @ClienteTipoCreditoID", cnx);
                cmd.Parameters.AddWithValue("@ClienteTipoCreditoID", clienteTipoCreditoId);
                cmd.Parameters.AddWithValue("@Estatus", !suspender); // true = activo, false = suspendido
                cmd.Parameters.AddWithValue("@Usuario", "system");
                cnx.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================================================================
        // 7. OBTENER RESUMEN DE CRÉDITO DE CLIENTE
        // ==================================================================
        public ResumenCreditoCliente ObtenerResumenCredito(Guid clienteId)
        {
            var resumen = new ResumenCreditoCliente { ClienteID = clienteId };
            var cliente = CD_Cliente.Instancia.ObtenerPorId(clienteId);
            
            if (cliente == null) return resumen;
            
            resumen.RazonSocial = cliente.RazonSocial;
            resumen.TiposAsignados = ObtenerCreditosCliente(clienteId);
            
            if (resumen.TiposAsignados.Count == 0)
            {
                resumen.TieneCreditoActivo = false;
                resumen.Estado = "SIN_CREDITO";
                return resumen;
            }

            // Calcular totales
            foreach (var credito in resumen.TiposAsignados)
            {
                if (credito.Activo)
                {
                    resumen.TieneCreditoActivo = true;
                    
                    if (credito.LimiteDinero.HasValue)
                        resumen.LimiteDineroTotal += credito.LimiteDinero.Value;
                    
                    if (credito.LimiteProducto.HasValue)
                        resumen.LimiteProductoTotal += credito.LimiteProducto.Value;
                    
                    if (credito.PlazoDias.HasValue && credito.PlazoDias > resumen.PlazoDiasMaximo)
                        resumen.PlazoDiasMaximo = credito.PlazoDias.Value;
                }
            }

            // Calcular saldos utilizados
            resumen.SaldoDineroUtilizado = CD_Cliente.Instancia.ObtenerSaldoActual(clienteId);
            resumen.SaldoDineroDisponible = Math.Max(0, resumen.LimiteDineroTotal - resumen.SaldoDineroUtilizado);
            resumen.SaldoVencido = CD_Cliente.Instancia.ObtenerSaldoVencido(clienteId);
            resumen.DiasMaximoVencidos = CD_Cliente.Instancia.ObtenerDiasVencidos(clienteId);

            // Determinar estado
            if (resumen.DiasMaximoVencidos > 0)
                resumen.Estado = "VENCIDO";
            else if (resumen.SaldoDineroDisponible <= (resumen.LimiteDineroTotal * 0.1m)) // Menos del 10%
                resumen.Estado = "CRÍTICO";
            else if (resumen.SaldoDineroDisponible <= (resumen.LimiteDineroTotal * 0.25m)) // Menos del 25%
                resumen.Estado = "ALERTA";
            else
                resumen.Estado = "NORMAL";

            resumen.EnAlarma = resumen.Estado != "NORMAL" && resumen.Estado != "SIN_CREDITO";

            return resumen;
        }

        // ==================================================================
        // 8. VALIDAR SI CLIENTE PUEDE USAR CRÉDITO (por tipo)
        // ==================================================================
        public bool PuedoUsarCredito(Guid clienteId, int tipoCreditoId, decimal? montoSolicitado = null)
        {
            var creditos = ObtenerCreditosCliente(clienteId);
            var credito = creditos.Find(c => c.TipoCreditoID == tipoCreditoId && c.Activo && !c.Suspendido);

            if (credito == null)
                return false;

            // Verificar según criterio
            if (credito.Criterio == "Dinero" && credito.LimiteDinero.HasValue && montoSolicitado.HasValue)
            {
                decimal saldo = CD_Cliente.Instancia.ObtenerSaldoActual(clienteId);
                return (credito.LimiteDinero.Value - saldo) >= montoSolicitado.Value;
            }

            if (credito.Criterio == "Tiempo" && credito.PlazoDias.HasValue)
            {
                // Verificar que no esté vencido
                return credito.FechaVencimiento == null || credito.FechaVencimiento > DateTime.Now;
            }

            return true;
        }
    }
}
