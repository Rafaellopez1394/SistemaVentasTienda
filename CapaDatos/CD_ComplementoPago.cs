using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CapaModelo;
using CapaDatos.PAC;

namespace CapaDatos
{
    public class CD_ComplementoPago
    {
        private static CD_ComplementoPago _instancia = null;

        public static CD_ComplementoPago Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_ComplementoPago();
                return _instancia;
            }
        }

        /// <summary>
        /// Obtiene facturas pendientes de pago por cliente
        /// </summary>
        public List<FacturaPendientePago> ObtenerFacturasPendientes(int clienteID)
        {
            List<FacturaPendientePago> lista = new List<FacturaPendientePago>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        SELECT IdFactura, UUID, Serie, Folio, FechaEmision, 
                               MontoTotal, SaldoPendiente, NumeroParcialidades, EsPagada
                        FROM Factura
                        WHERE ClienteID = @ClienteID
                          AND EsCancelada = 0
                          AND UUID IS NOT NULL
                          AND SaldoPendiente > 0
                        ORDER BY FechaEmision";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                    cnx.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var factura = new FacturaPendientePago
                            {
                                FacturaID = reader.GetInt32(0),
                                UUID = reader.GetGuid(1),
                                Serie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Folio = reader.GetInt32(3),
                                FechaEmision = reader.GetDateTime(4),
                                MontoTotal = reader.GetDecimal(5),
                                SaldoPendiente = reader.GetDecimal(6),
                                NumeroParcialidades = reader.GetInt32(7),
                                EsPagada = reader.GetBoolean(8)
                            };

                            factura.SerieFolio = $"{factura.Serie}-{factura.Folio}";
                            lista.Add(factura);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener facturas pendientes: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Genera y timbra un complemento de pago
        /// </summary>
        public async Task<RespuestaTimbrado> GenerarYTimbrarComplementoPago(AplicarPagoRequest request, string usuario)
        {
            var respuesta = new RespuestaTimbrado();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();

                try
                {
                    // 1. Validar que el monto total coincida con la suma de pagos
                    decimal sumaPagos = request.Facturas.Sum(f => f.MontoPagar);
                    if (Math.Abs(sumaPagos - request.MontoTotal) > 0.01m)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "El monto total no coincide con la suma de pagos a facturas";
                        return respuesta;
                    }

                    // 2. Obtener datos del cliente
                    var cliente = ObtenerCliente(request.ClienteID, cnx, transaction);
                    if (cliente == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Cliente no encontrado";
                        return respuesta;
                    }

                    // 3. Crear complemento de pago
                    var complemento = new ComplementoPago
                    {
                        ClienteID = request.ClienteID,
                        ReceptorRFC = cliente.RFC,
                        ReceptorNombre = cliente.RazonSocial,
                        ReceptorDomicilioFiscal = cliente.CodigoPostal ?? "00000",
                        FechaEmision = DateTime.Now,
                        MontoTotalPagos = request.MontoTotal,
                        UsuarioRegistro = usuario,
                        Serie = "REP", // Recibo Electrónico de Pago
                        Folio = ObtenerSiguienteFolio("REP", cnx, transaction)
                    };

                    // 4. Crear pago
                    var pago = new ComplementoPagoPago
                    {
                        FechaPago = request.FechaPago,
                        FormaDePagoP = request.FormaDePago,
                        Monto = request.MontoTotal,
                        NumOperacion = request.NumOperacion,
                        CtaOrdenante = request.CtaOrdenante,
                        CtaBeneficiario = request.CtaBeneficiario
                    };

                    // 5. Agregar documentos relacionados
                    foreach (var facturaItem in request.Facturas)
                    {
                        var facturaData = ObtenerDatosFactura(facturaItem.FacturaID, cnx, transaction);
                        
                        if (facturaData == null)
                        {
                            respuesta.Exitoso = false;
                            respuesta.Mensaje = $"Factura {facturaItem.FacturaID} no encontrada";
                            transaction.Rollback();
                            return respuesta;
                        }

                        var doc = new ComplementoPagoDocumento
                        {
                            FacturaID = facturaItem.FacturaID,
                            IdDocumento = Guid.NewGuid(), // Temporal
                            UUIDDocumento = facturaData.UUID ?? "",
                            Serie = facturaData.Serie,
                            Folio = facturaData.Folio.ToString(),
                            NumParcialidad = (facturaData.NumeroParcialidades ?? 0) + 1,
                            ImpSaldoAnt = facturaItem.SaldoPendiente,
                            ImpPagado = facturaItem.MontoPagar,
                            ImpSaldoInsoluto = facturaItem.SaldoPendiente - facturaItem.MontoPagar
                        };

                        // Calcular impuestos del documento
                        AgregarImpuestosDocumento(doc, facturaData, facturaItem.MontoPagar);

                        pago.DocumentosRelacionados.Add(doc);
                    }

                    complemento.Pagos.Add(pago);

                    // 6. Guardar complemento en base de datos
                    int complementoPagoID = InsertarComplementoPago(complemento, cnx, transaction);
                    complemento.ComplementoPagoID = complementoPagoID;

                    // 7. Obtener configuración
                    var empresa = ObtenerConfiguracionEmpresa();
                    var configPAC = ObtenerConfiguracionPAC();

                    // 8. Generar XML
                    var generator = new ComplementoPago20XMLGenerator();
                    string xmlSinTimbrar = generator.GenerarXML(complemento, empresa, configPAC);

                    // Guardar XML sin timbrar
                    ActualizarXMLSinTimbrar(complementoPagoID, xmlSinTimbrar, cnx, transaction);

                    // 9. Timbrar con PAC
                    var proveedorPAC = new FinkokPAC();
                    respuesta = await proveedorPAC.TimbrarAsync(xmlSinTimbrar, configPAC);

                    if (respuesta.Exitoso)
                    {
                        // 10. Actualizar complemento con datos del timbrado
                        ActualizarComplementoTimbrado(complementoPagoID, respuesta, cnx, transaction);

                        // 11. Actualizar saldos de facturas
                        foreach (var facturaItem in request.Facturas)
                        {
                            ActualizarSaldoFactura(facturaItem.FacturaID, facturaItem.MontoPagar, cnx, transaction);
                        }

                        transaction.Commit();
                        respuesta.Mensaje = "Complemento de pago timbrado exitosamente";
                    }
                    else
                    {
                        // Guardar error
                        ActualizarComplementoError(complementoPagoID, respuesta.CodigoError, respuesta.Mensaje, cnx, transaction);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Error al generar complemento de pago: " + ex.Message;
                }
            }

            return respuesta;
        }

        #region Métodos Auxiliares

        private Cliente ObtenerCliente(int clienteID, SqlConnection cnx, SqlTransaction tran)
        {
            string query = "SELECT ClienteID, RFC, RazonSocial, CodigoPostal FROM Cliente WHERE ClienteID = @ClienteID";
            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ClienteID", clienteID);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = reader.GetInt32(0),
                        ClienteID = Guid.Empty, // Temporal
                        RFC = reader.GetString(1),
                        RazonSocial = reader.GetString(2),
                        CodigoPostal = reader.IsDBNull(3) ? null : reader.GetString(3)
                    };
                }
            }

            return null;
        }

        private Factura ObtenerDatosFactura(int facturaID, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                SELECT IdFactura, UUID, Serie, Folio, MontoTotal, SaldoPendiente, 
                       NumeroParcialidades, SubTotal, IVA
                FROM Factura
                WHERE IdFactura = @FacturaID";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@FacturaID", facturaID);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Factura
                    {
                        IdFactura = reader.GetInt32(0),
                        UUID = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Serie = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Folio = reader.IsDBNull(3) ? null : reader.GetInt32(3).ToString(),
                        MontoTotal = reader.GetDecimal(4),
                        SaldoPendiente = reader.IsDBNull(5) ? reader.GetDecimal(4) : reader.GetDecimal(5),
                        NumeroParcialidades = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                        Subtotal = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                        SubTotal = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                        IVA = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8)
                    };
                }
            }

            return null;
        }

        private void AgregarImpuestosDocumento(ComplementoPagoDocumento doc, Factura factura, decimal montoPagado)
        {
            // Calcular proporción del pago respecto al total
            decimal proporcion = montoPagado / factura.MontoTotal;

            // IVA Trasladado (16%)
            if (factura.IVA > 0)
            {
                decimal baseIVA = factura.Subtotal * proporcion;
                decimal importeIVA = factura.IVA * proporcion;

                doc.ImpuestosDR.Add(new ComplementoPagoImpuestoDR
                {
                    TipoImpuesto = "TRASLADO",
                    BaseDR = baseIVA,
                    ImpuestoDR = "002", // IVA
                    TipoFactorDR = "Tasa",
                    TasaOCuotaDR = 0.160000m,
                    ImporteDR = importeIVA
                });
            }
        }

        private int ObtenerSiguienteFolio(string serie, SqlConnection cnx, SqlTransaction tran)
        {
            string query = "SELECT ISNULL(MAX(Folio), 0) + 1 FROM ComplementosPago WHERE Serie = @Serie";
            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@Serie", serie);
            
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private CapaDatos.PAC.ConfiguracionEmpresa ObtenerConfiguracionEmpresa()
        {
            // TODO: Obtener de tabla ConfiguracionEmpresa
            return new CapaDatos.PAC.ConfiguracionEmpresa
            {
                RFC = "XAXX010101000",
                RazonSocial = "EMPRESA DEMO SA DE CV",
                RegimenFiscal = "601",
                CodigoPostal = "00000",
                NoCertificado = "00001000000000000000",
                Certificado = ""
            };
        }

        private ConfiguracionPAC ObtenerConfiguracionPAC()
        {
            // TODO: Obtener de tabla ConfiguracionPAC
            return new ConfiguracionPAC
            {
                ProveedorPAC = "Finkok",
                EsProduccion = false,
                UrlTimbrado = "https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl",
                Usuario = "usuario_demo",
                Password = "password_demo",
                TimeoutSegundos = 30
            };
        }

        #endregion

        #region Métodos de Base de Datos

        private int InsertarComplementoPago(ComplementoPago complemento, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO ComplementosPago 
                (ClienteID, ReceptorRFC, ReceptorNombre, ReceptorDomicilioFiscal, ReceptorRegimenFiscal, 
                 ReceptorUsoCFDI, MontoTotalPagos, Serie, Folio, FechaEmision, EstadoTimbrado, UsuarioRegistro)
                VALUES 
                (@ClienteID, @RFC, @Nombre, @CP, @Regimen, @UsoCFDI, @Monto, @Serie, @Folio, @Fecha, 'PENDIENTE', @Usuario);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ClienteID", complemento.ClienteID);
            cmd.Parameters.AddWithValue("@RFC", complemento.ReceptorRFC);
            cmd.Parameters.AddWithValue("@Nombre", complemento.ReceptorNombre);
            cmd.Parameters.AddWithValue("@CP", complemento.ReceptorDomicilioFiscal);
            cmd.Parameters.AddWithValue("@Regimen", complemento.ReceptorRegimenFiscal);
            cmd.Parameters.AddWithValue("@UsoCFDI", complemento.ReceptorUsoCFDI);
            cmd.Parameters.AddWithValue("@Monto", complemento.MontoTotalPagos);
            cmd.Parameters.AddWithValue("@Serie", complemento.Serie ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Folio", complemento.Folio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", complemento.FechaEmision);
            cmd.Parameters.AddWithValue("@Usuario", complemento.UsuarioRegistro ?? (object)DBNull.Value);

            int complementoPagoID = (int)cmd.ExecuteScalar();

            // Insertar pagos
            foreach (var pago in complemento.Pagos)
            {
                int pagoID = InsertarPago(complementoPagoID, pago, cnx, tran);
                
                // Insertar documentos relacionados
                foreach (var doc in pago.DocumentosRelacionados)
                {
                    int documentoID = InsertarDocumentoRelacionado(pagoID, doc, cnx, tran);
                    
                    // Insertar impuestos
                    foreach (var imp in doc.ImpuestosDR)
                    {
                        InsertarImpuestoDR(documentoID, imp, cnx, tran);
                    }
                }
            }

            return complementoPagoID;
        }

        private int InsertarPago(int complementoPagoID, ComplementoPagoPago pago, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO ComplementoPagoPagos
                (ComplementoPagoID, FechaPago, FormaDePagoP, MonedaP, TipoCambioP, Monto,
                 NumOperacion, RfcEmisorCtaOrd, NomBancoOrdExt, CtaOrdenante, RfcEmisorCtaBen, CtaBeneficiario)
                VALUES
                (@ComplementoID, @Fecha, @Forma, @Moneda, @TipoCambio, @Monto,
                 @NumOp, @RfcOrd, @BancoOrd, @CtaOrd, @RfcBen, @CtaBen);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ComplementoID", complementoPagoID);
            cmd.Parameters.AddWithValue("@Fecha", pago.FechaPago);
            cmd.Parameters.AddWithValue("@Forma", pago.FormaDePagoP);
            cmd.Parameters.AddWithValue("@Moneda", pago.MonedaP);
            cmd.Parameters.AddWithValue("@TipoCambio", pago.TipoCambioP ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Monto", pago.Monto);
            cmd.Parameters.AddWithValue("@NumOp", pago.NumOperacion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RfcOrd", pago.RfcEmisorCtaOrd ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BancoOrd", pago.NomBancoOrdExt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CtaOrd", pago.CtaOrdenante ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RfcBen", pago.RfcEmisorCtaBen ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CtaBen", pago.CtaBeneficiario ?? (object)DBNull.Value);

            return (int)cmd.ExecuteScalar();
        }

        private int InsertarDocumentoRelacionado(int pagoID, ComplementoPagoDocumento doc, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO ComplementoPagoDocumentos
                (PagoID, FacturaID, IdDocumento, Serie, Folio, MonedaDR, EquivalenciaDR,
                 NumParcialidad, ImpSaldoAnt, ImpPagado, ImpSaldoInsoluto, ObjetoImpDR)
                VALUES
                (@PagoID, @FacturaID, @UUID, @Serie, @Folio, @Moneda, @Equiv,
                 @NumParc, @SaldoAnt, @Pagado, @Insoluto, @ObjImp);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@PagoID", pagoID);
            cmd.Parameters.AddWithValue("@FacturaID", doc.FacturaID);
            cmd.Parameters.AddWithValue("@UUID", doc.IdDocumento);
            cmd.Parameters.AddWithValue("@Serie", doc.Serie ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Folio", doc.Folio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Moneda", doc.MonedaDR);
            cmd.Parameters.AddWithValue("@Equiv", doc.EquivalenciaDR ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NumParc", doc.NumParcialidad);
            cmd.Parameters.AddWithValue("@SaldoAnt", doc.ImpSaldoAnt);
            cmd.Parameters.AddWithValue("@Pagado", doc.ImpPagado);
            cmd.Parameters.AddWithValue("@Insoluto", doc.ImpSaldoInsoluto);
            cmd.Parameters.AddWithValue("@ObjImp", doc.ObjetoImpDR);

            return (int)cmd.ExecuteScalar();
        }

        private void InsertarImpuestoDR(int documentoID, ComplementoPagoImpuestoDR imp, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO ComplementoPagoImpuestosDR
                (DocumentoID, TipoImpuesto, BaseDR, ImpuestoDR, TipoFactorDR, TasaOCuotaDR, ImporteDR)
                VALUES
                (@DocID, @Tipo, @Base, @Impuesto, @Factor, @Tasa, @Importe)";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@DocID", documentoID);
            cmd.Parameters.AddWithValue("@Tipo", imp.TipoImpuesto);
            cmd.Parameters.AddWithValue("@Base", imp.BaseDR);
            cmd.Parameters.AddWithValue("@Impuesto", imp.ImpuestoDR);
            cmd.Parameters.AddWithValue("@Factor", imp.TipoFactorDR);
            cmd.Parameters.AddWithValue("@Tasa", imp.TasaOCuotaDR ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Importe", imp.ImporteDR ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        private void ActualizarXMLSinTimbrar(int complementoPagoID, string xml, SqlConnection cnx, SqlTransaction tran)
        {
            string query = "UPDATE ComplementosPago SET XMLSinTimbrar = @XML WHERE ComplementoPagoID = @ID";
            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ID", complementoPagoID);
            cmd.Parameters.AddWithValue("@XML", xml);
            cmd.ExecuteNonQuery();
        }

        private void ActualizarComplementoTimbrado(int complementoPagoID, RespuestaTimbrado respuesta, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                UPDATE ComplementosPago
                SET UUID = @UUID,
                    FechaTimbrado = @Fecha,
                    XMLTimbrado = @XML,
                    SelloCFD = @SelloCFD,
                    SelloSAT = @SelloSAT,
                    NoCertificadoSAT = @NoCert,
                    EstadoTimbrado = 'TIMBRADO'
                WHERE ComplementoPagoID = @ID";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ID", complementoPagoID);
            cmd.Parameters.AddWithValue("@UUID", Guid.Parse(respuesta.UUID));
            cmd.Parameters.AddWithValue("@Fecha", respuesta.FechaTimbrado);
            cmd.Parameters.AddWithValue("@XML", respuesta.XMLTimbrado);
            cmd.Parameters.AddWithValue("@SelloCFD", respuesta.SelloCFD ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SelloSAT", respuesta.SelloSAT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NoCert", respuesta.NoCertificadoSAT ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        private void ActualizarComplementoError(int complementoPagoID, string codigo, string mensaje, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                UPDATE ComplementosPago
                SET CodigoError = @Codigo,
                    MensajeError = @Mensaje,
                    EstadoTimbrado = 'ERROR'
                WHERE ComplementoPagoID = @ID";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@ID", complementoPagoID);
            cmd.Parameters.AddWithValue("@Codigo", codigo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Mensaje", mensaje ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        private void ActualizarSaldoFactura(int facturaID, decimal montoPagado, SqlConnection cnx, SqlTransaction tran)
        {
            string query = @"
                UPDATE Factura
                SET SaldoPendiente = SaldoPendiente - @MontoPagado,
                    NumeroParcialidades = NumeroParcialidades + 1,
                    EsPagada = CASE WHEN (SaldoPendiente - @MontoPagado) <= 0.01 THEN 1 ELSE 0 END
                WHERE IdFactura = @FacturaID";

            SqlCommand cmd = new SqlCommand(query, cnx, tran);
            cmd.Parameters.AddWithValue("@FacturaID", facturaID);
            cmd.Parameters.AddWithValue("@MontoPagado", montoPagado);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
