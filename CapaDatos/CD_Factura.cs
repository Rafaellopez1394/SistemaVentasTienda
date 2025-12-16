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
    public class CD_Factura
    {
        private static CD_Factura _instancia = null;

        public static CD_Factura Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_Factura();
                return _instancia;
            }
        }

        /// <summary>
        /// Obtiene lista de facturas con filtros opcionales
        /// </summary>
        public List<Factura> ObtenerFacturas(string rfc = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null, string estatus = null)
        {
            List<Factura> listaFacturas = new List<Factura>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT IdFactura, VentaID, Serie, Folio, UUID, 
                               FechaEmision, FechaTimbrado, 
                               RFCEmisor, NombreEmisor, RegimenFiscalEmisor,
                               RFCReceptor, NombreReceptor, UsoCFDI, 
                               DomicilioFiscalReceptor, RegimenFiscalReceptor, EmailReceptor,
                               Subtotal, TotalImpuestosTrasladados, TotalImpuestosRetenidos, MontoTotal,
                               FormaPago, MetodoPago, Moneda, TipoCambio,
                               Estatus, MensajeError, XMLTimbrado, SelloCFD, SelloSAT, NoCertificadoSAT,
                               EsCancelada, FechaCancelacion, MotivoCancelacion,
                               UsuarioCreacion, FechaCreacion
                        FROM Factura
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(rfc))
                        query += " AND RFCReceptor LIKE @RFC";

                    if (fechaDesde.HasValue)
                        query += " AND FechaEmision >= @FechaDesde";

                    if (fechaHasta.HasValue)
                        query += " AND FechaEmision <= @FechaHasta";

                    if (!string.IsNullOrEmpty(estatus))
                        query += " AND Estatus = @Estatus";

                    query += " ORDER BY FechaEmision DESC";

                    SqlCommand cmd = new SqlCommand(query, cnx);

                    if (!string.IsNullOrEmpty(rfc))
                        cmd.Parameters.AddWithValue("@RFC", "%" + rfc + "%");

                    if (fechaDesde.HasValue)
                        cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.Value);

                    if (fechaHasta.HasValue)
                        cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.Value.AddDays(1).AddSeconds(-1));

                    if (!string.IsNullOrEmpty(estatus))
                        cmd.Parameters.AddWithValue("@Estatus", estatus);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaFacturas.Add(new Factura
                            {
                                FacturaID = (Guid)dr["IdFactura"],
                                VentaID = dr["VentaID"] != DBNull.Value ? (Guid)dr["VentaID"] : Guid.Empty,
                                Serie = dr["Serie"]?.ToString(),
                                Folio = dr["Folio"]?.ToString(),
                                UUID = dr["UUID"]?.ToString(),
                                FechaEmision = Convert.ToDateTime(dr["FechaEmision"]),
                                FechaTimbrado = dr["FechaTimbrado"] != DBNull.Value ? (DateTime?)dr["FechaTimbrado"] : null,
                                RFCEmisor = dr["RFCEmisor"]?.ToString(),
                                NombreEmisor = dr["NombreEmisor"]?.ToString(),
                                RegimenFiscalEmisor = dr["RegimenFiscalEmisor"]?.ToString(),
                                ReceptorRFC = dr["RFCReceptor"]?.ToString(),
                                ReceptorNombre = dr["NombreReceptor"]?.ToString(),
                                ReceptorUsoCFDI = dr["UsoCFDI"]?.ToString(),
                                ReceptorDomicilioFiscalCP = dr["DomicilioFiscalReceptor"]?.ToString(),
                                ReceptorRegimenFiscalReceptor = dr["RegimenFiscalReceptor"]?.ToString(),
                                ReceptorEmail = dr["EmailReceptor"]?.ToString(),
                                Subtotal = dr["Subtotal"] != DBNull.Value ? Convert.ToDecimal(dr["Subtotal"]) : 0,
                                TotalImpuestosTrasladados = dr["TotalImpuestosTrasladados"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosTrasladados"]) : 0,
                                TotalImpuestosRetenidos = dr["TotalImpuestosRetenidos"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosRetenidos"]) : 0,
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                                FormaPago = dr["FormaPago"]?.ToString(),
                                MetodoPago = dr["MetodoPago"]?.ToString(),
                                Moneda = dr["Moneda"]?.ToString() ?? "MXN",
                                TipoCambio = dr["TipoCambio"] != DBNull.Value ? Convert.ToDecimal(dr["TipoCambio"]) : 1,
                                Estatus = dr["Estatus"]?.ToString(),
                                MensajeError = dr["MensajeError"]?.ToString(),
                                XMLTimbrado = dr["XMLTimbrado"]?.ToString(),
                                SelloCFD = dr["SelloCFD"]?.ToString(),
                                SelloSAT = dr["SelloSAT"]?.ToString(),
                                NoCertificadoSAT = dr["NoCertificadoSAT"]?.ToString(),
                                EsCancelada = dr["EsCancelada"] != DBNull.Value && Convert.ToBoolean(dr["EsCancelada"]),
                                FechaCancelacion = dr["FechaCancelacion"] != DBNull.Value ? (DateTime?)dr["FechaCancelacion"] : null,
                                MotivoCancelacion = dr["MotivoCancelacion"]?.ToString(),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : DateTime.Now
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerFacturas: {ex.Message}");
            }

            return listaFacturas;
        }

        /// <summary>
        /// Crea una factura desde una venta existente
        /// </summary>
        public Factura CrearFacturaDesdeVenta(GenerarFacturaRequest request, out string mensaje)
        {
            mensaje = string.Empty;
            Factura factura = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    // 1. Obtener datos de la venta
                    string queryVenta = @"
                        SELECT v.VentaID, v.MontoTotal, v.MontoDescuento, v.FechaVenta,
                               dv.ProductoID, p.Codigo, p.Nombre, dv.Cantidad, 
                               dv.PrecioVenta, dv.Subtotal, dv.PorcentajeImpuesto, dv.MontoImpuesto
                        FROM VentasClientes v
                        INNER JOIN DetalleVenta dv ON v.VentaID = dv.VentaID
                        INNER JOIN Producto p ON dv.ProductoID = p.ProductoID
                        WHERE v.VentaID = @VentaID";

                    SqlCommand cmd = new SqlCommand(queryVenta, cnx);
                    cmd.Parameters.AddWithValue("@VentaID", request.VentaID);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (!dr.HasRows)
                    {
                        mensaje = "No se encontró la venta especificada";
                        dr.Close();
                        return null;
                    }

                    factura = new Factura
                    {
                        FacturaID = Guid.NewGuid(),
                        VentaID = request.VentaID,
                        FechaEmision = DateTime.Now,
                        ReceptorRFC = request.ReceptorRFC.ToUpper(),
                        ReceptorNombre = request.ReceptorNombre,
                        ReceptorUsoCFDI = request.ReceptorUsoCFDI,
                        ReceptorDomicilioFiscalCP = request.ReceptorCP,
                        ReceptorRegimenFiscalReceptor = request.ReceptorRegimenFiscal,
                        ReceptorEmail = request.ReceptorEmail,
                        FormaPago = request.FormaPago,
                        MetodoPago = request.MetodoPago,
                        Estatus = "PENDIENTE"
                    };

                    // Obtener emisor (de configuración)
                    factura.EmisorRFC = "XAXX010101000"; // TODO: Obtener de configuración
                    factura.EmisorNombre = "Empresa Demo"; // TODO: Obtener de configuración
                    factura.EmisorRegimenFiscal = "601"; // TODO: Obtener de configuración

                    decimal subtotalTotal = 0;
                    decimal impuestosTotal = 0;
                    int secuencia = 1;

                    while (dr.Read())
                    {
                        var detalle = new FacturaDetalle
                        {
                            Secuencia = secuencia++,
                            ClaveProdServ = "01010101", // TODO: Mapear de producto
                            NoIdentificacion = dr["Codigo"].ToString(),
                            Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                            ClaveUnidad = "H87", // Pieza
                            Unidad = "Pieza",
                            Descripcion = dr["Nombre"].ToString(),
                            ValorUnitario = Convert.ToDecimal(dr["PrecioVenta"]),
                            Importe = Convert.ToDecimal(dr["Subtotal"]),
                            Descuento = 0,
                            ObjetoImp = "02" // Sí objeto de impuestos
                        };

                        decimal porcentajeIVA = Convert.ToDecimal(dr["PorcentajeImpuesto"]);
                        decimal montoImpuesto = Convert.ToDecimal(dr["MontoImpuesto"]);

                        if (porcentajeIVA > 0)
                        {
                            var impuesto = new FacturaImpuesto
                            {
                                TipoImpuesto = "TRASLADO",
                                Impuesto = "002", // IVA
                                TipoFactor = "Tasa",
                                TasaOCuota = porcentajeIVA / 100,
                                Base = detalle.Importe,
                                Importe = montoImpuesto
                            };
                            detalle.Impuestos.Add(impuesto);
                            detalle.TotalImpuestosTrasladados = montoImpuesto;
                        }

                        factura.Conceptos.Add(detalle);
                        subtotalTotal += detalle.Importe;
                        impuestosTotal += montoImpuesto;
                    }
                    dr.Close();

                    factura.Subtotal = subtotalTotal;
                    factura.TotalImpuestosTrasladados = impuestosTotal;
                    factura.Total = subtotalTotal + impuestosTotal;

                    // 2. Generar Serie y Folio
                    factura.Serie = "A";
                    SqlCommand cmdFolio = new SqlCommand("GenerarFolioFactura", cnx);
                    cmdFolio.CommandType = CommandType.StoredProcedure;
                    cmdFolio.Parameters.AddWithValue("@Serie", factura.Serie);
                    SqlParameter pFolio = new SqlParameter("@Folio", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmdFolio.Parameters.Add(pFolio);
                    cmdFolio.ExecuteNonQuery();
                    factura.Folio = pFolio.Value.ToString();

                    mensaje = "Factura preparada correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: " + ex.Message;
                return null;
            }

            return factura;
        }

        /// <summary>
        /// Guarda la factura en base de datos
        /// </summary>
        public bool GuardarFactura(Factura factura, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();
                    SqlTransaction tran = cnx.BeginTransaction();

                    try
                    {
                        // Insertar factura
                        string queryFactura = @"
                            INSERT INTO Facturas (
                                FacturaID, VentaID, Serie, Folio, FechaEmision, Version, TipoComprobante,
                                Subtotal, Descuento, Total, TotalImpuestosTrasladados, TotalImpuestosRetenidos,
                                EmisorRFC, EmisorNombre, EmisorRegimenFiscal,
                                ReceptorRFC, ReceptorNombre, ReceptorUsoCFDI, ReceptorDomicilioFiscalCP, 
                                ReceptorRegimenFiscalReceptor, ReceptorEmail,
                                FormaPago, MetodoPago, ProveedorPAC, Estatus,
                                XMLOriginal, UsuarioCreacion, FechaCreacion
                            ) VALUES (
                                @FacturaID, @VentaID, @Serie, @Folio, @FechaEmision, @Version, @TipoComprobante,
                                @Subtotal, @Descuento, @Total, @TotalImpuestosTrasladados, @TotalImpuestosRetenidos,
                                @EmisorRFC, @EmisorNombre, @EmisorRegimenFiscal,
                                @ReceptorRFC, @ReceptorNombre, @ReceptorUsoCFDI, @ReceptorDomicilioFiscalCP,
                                @ReceptorRegimenFiscalReceptor, @ReceptorEmail,
                                @FormaPago, @MetodoPago, @ProveedorPAC, @Estatus,
                                @XMLOriginal, @UsuarioCreacion, GETDATE()
                            )";

                        SqlCommand cmdFactura = new SqlCommand(queryFactura, cnx, tran);
                        cmdFactura.Parameters.AddWithValue("@FacturaID", factura.FacturaID);
                        cmdFactura.Parameters.AddWithValue("@VentaID", factura.VentaID ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@Serie", factura.Serie);
                        cmdFactura.Parameters.AddWithValue("@Folio", factura.Folio);
                        cmdFactura.Parameters.AddWithValue("@FechaEmision", factura.FechaEmision);
                        cmdFactura.Parameters.AddWithValue("@Version", factura.Version);
                        cmdFactura.Parameters.AddWithValue("@TipoComprobante", factura.TipoComprobante);
                        cmdFactura.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
                        cmdFactura.Parameters.AddWithValue("@Descuento", factura.Descuento);
                        cmdFactura.Parameters.AddWithValue("@Total", factura.Total);
                        cmdFactura.Parameters.AddWithValue("@TotalImpuestosTrasladados", factura.TotalImpuestosTrasladados);
                        cmdFactura.Parameters.AddWithValue("@TotalImpuestosRetenidos", factura.TotalImpuestosRetenidos);
                        cmdFactura.Parameters.AddWithValue("@EmisorRFC", factura.EmisorRFC);
                        cmdFactura.Parameters.AddWithValue("@EmisorNombre", factura.EmisorNombre);
                        cmdFactura.Parameters.AddWithValue("@EmisorRegimenFiscal", factura.EmisorRegimenFiscal);
                        cmdFactura.Parameters.AddWithValue("@ReceptorRFC", factura.ReceptorRFC);
                        cmdFactura.Parameters.AddWithValue("@ReceptorNombre", factura.ReceptorNombre);
                        cmdFactura.Parameters.AddWithValue("@ReceptorUsoCFDI", factura.ReceptorUsoCFDI);
                        cmdFactura.Parameters.AddWithValue("@ReceptorDomicilioFiscalCP", factura.ReceptorDomicilioFiscalCP);
                        cmdFactura.Parameters.AddWithValue("@ReceptorRegimenFiscalReceptor", factura.ReceptorRegimenFiscalReceptor);
                        cmdFactura.Parameters.AddWithValue("@ReceptorEmail", factura.ReceptorEmail ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@FormaPago", factura.FormaPago);
                        cmdFactura.Parameters.AddWithValue("@MetodoPago", factura.MetodoPago);
                        cmdFactura.Parameters.AddWithValue("@ProveedorPAC", factura.ProveedorPAC);
                        cmdFactura.Parameters.AddWithValue("@Estatus", factura.Estatus);
                        cmdFactura.Parameters.AddWithValue("@XMLOriginal", factura.XMLOriginal ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@UsuarioCreacion", factura.UsuarioCreacion);
                        cmdFactura.ExecuteNonQuery();

                        // Insertar detalles
                        foreach (var detalle in factura.Conceptos)
                        {
                            string queryDetalle = @"
                                INSERT INTO FacturasDetalle (
                                    FacturaID, Secuencia, ClaveProdServ, NoIdentificacion, Cantidad,
                                    ClaveUnidad, Unidad, Descripcion, ValorUnitario, Importe, Descuento,
                                    ObjetoImp, TotalImpuestosTrasladados, TotalImpuestosRetenidos
                                ) VALUES (
                                    @FacturaID, @Secuencia, @ClaveProdServ, @NoIdentificacion, @Cantidad,
                                    @ClaveUnidad, @Unidad, @Descripcion, @ValorUnitario, @Importe, @Descuento,
                                    @ObjetoImp, @TotalImpuestosTrasladados, @TotalImpuestosRetenidos
                                );
                                SELECT SCOPE_IDENTITY();";

                            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, cnx, tran);
                            cmdDetalle.Parameters.AddWithValue("@FacturaID", factura.FacturaID);
                            cmdDetalle.Parameters.AddWithValue("@Secuencia", detalle.Secuencia);
                            cmdDetalle.Parameters.AddWithValue("@ClaveProdServ", detalle.ClaveProdServ);
                            cmdDetalle.Parameters.AddWithValue("@NoIdentificacion", detalle.NoIdentificacion ?? (object)DBNull.Value);
                            cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@ClaveUnidad", detalle.ClaveUnidad);
                            cmdDetalle.Parameters.AddWithValue("@Unidad", detalle.Unidad);
                            cmdDetalle.Parameters.AddWithValue("@Descripcion", detalle.Descripcion);
                            cmdDetalle.Parameters.AddWithValue("@ValorUnitario", detalle.ValorUnitario);
                            cmdDetalle.Parameters.AddWithValue("@Importe", detalle.Importe);
                            cmdDetalle.Parameters.AddWithValue("@Descuento", detalle.Descuento);
                            cmdDetalle.Parameters.AddWithValue("@ObjetoImp", detalle.ObjetoImp);
                            cmdDetalle.Parameters.AddWithValue("@TotalImpuestosTrasladados", detalle.TotalImpuestosTrasladados);
                            cmdDetalle.Parameters.AddWithValue("@TotalImpuestosRetenidos", detalle.TotalImpuestosRetenidos);
                            
                            int detalleID = Convert.ToInt32(cmdDetalle.ExecuteScalar());

                            // Insertar impuestos del detalle
                            foreach (var impuesto in detalle.Impuestos)
                            {
                                string queryImpuesto = @"
                                    INSERT INTO FacturasImpuestos (
                                        FacturaDetalleID, TipoImpuesto, Impuesto, TipoFactor, TasaOCuota, Base, Importe
                                    ) VALUES (
                                        @FacturaDetalleID, @TipoImpuesto, @Impuesto, @TipoFactor, @TasaOCuota, @Base, @Importe
                                    )";

                                SqlCommand cmdImpuesto = new SqlCommand(queryImpuesto, cnx, tran);
                                cmdImpuesto.Parameters.AddWithValue("@FacturaDetalleID", detalleID);
                                cmdImpuesto.Parameters.AddWithValue("@TipoImpuesto", impuesto.TipoImpuesto);
                                cmdImpuesto.Parameters.AddWithValue("@Impuesto", impuesto.Impuesto);
                                cmdImpuesto.Parameters.AddWithValue("@TipoFactor", impuesto.TipoFactor);
                                cmdImpuesto.Parameters.AddWithValue("@TasaOCuota", impuesto.TasaOCuota ?? (object)DBNull.Value);
                                cmdImpuesto.Parameters.AddWithValue("@Base", impuesto.Base);
                                cmdImpuesto.Parameters.AddWithValue("@Importe", impuesto.Importe ?? (object)DBNull.Value);
                                cmdImpuesto.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        mensaje = "Factura guardada correctamente";
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR al guardar factura: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Actualiza factura con datos de timbrado
        /// </summary>
        public bool ActualizarConTimbrado(Guid facturaID, RespuestaTimbrado respuesta, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        UPDATE Facturas SET
                            UUID = @UUID,
                            FechaTimbrado = @FechaTimbrado,
                            NoCertificadoSAT = @NoCertificadoSAT,
                            SelloCFD = @SelloCFD,
                            SelloSAT = @SelloSAT,
                            CadenaOriginalSAT = @CadenaOriginalSAT,
                            XMLTimbrado = @XMLTimbrado,
                            Estatus = 'TIMBRADA',
                            FechaModificacion = GETDATE()
                        WHERE FacturaID = @FacturaID";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@FacturaID", facturaID);
                    cmd.Parameters.AddWithValue("@UUID", respuesta.UUID);
                    cmd.Parameters.AddWithValue("@FechaTimbrado", respuesta.FechaTimbrado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoCertificadoSAT", respuesta.NoCertificadoSAT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SelloCFD", respuesta.SelloCFD ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SelloSAT", respuesta.SelloSAT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CadenaOriginalSAT", respuesta.CadenaOriginal ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@XMLTimbrado", respuesta.XMLTimbrado ?? (object)DBNull.Value);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Factura timbrada exitosamente";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR al actualizar timbrado: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtiene configuración del PAC activo
        /// </summary>
        public ConfiguracionPAC ObtenerConfiguracionPAC(out string mensaje)
        {
            mensaje = string.Empty;
            ConfiguracionPAC config = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM ConfiguracionPAC WHERE Activo = 1";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        config = new ConfiguracionPAC
                        {
                            ConfigID = Convert.ToInt32(dr["ConfigID"]),
                            ProveedorPAC = dr["ProveedorPAC"].ToString(),
                            EsProduccion = Convert.ToBoolean(dr["EsProduccion"]),
                            UrlTimbrado = dr["UrlTimbrado"].ToString(),
                            UrlCancelacion = dr["UrlCancelacion"].ToString(),
                            UrlConsulta = dr["UrlConsulta"].ToString(),
                            Usuario = dr["Usuario"].ToString(),
                            Password = dr["Password"].ToString(),
                            RutaCertificado = dr["RutaCertificado"]?.ToString(),
                            RutaLlavePrivada = dr["RutaLlavePrivada"]?.ToString(),
                            PasswordLlave = dr["PasswordLlave"]?.ToString(),
                            TimeoutSegundos = Convert.ToInt32(dr["TimeoutSegundos"]),
                            Activo = Convert.ToBoolean(dr["Activo"])
                        };
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR al obtener configuración PAC: " + ex.Message;
            }

            return config;
        }

        /// <summary>
        /// Obtiene factura por UUID
        /// </summary>
        public Factura ObtenerPorUUID(string uuid, out string mensaje)
        {
            mensaje = string.Empty;
            Factura factura = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM Facturas WHERE UUID = @UUID";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@UUID", uuid);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        factura = MapearFactura(dr);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: " + ex.Message;
            }

            return factura;
        }

        /// <summary>
        /// Actualiza ruta del PDF
        /// </summary>
        public bool GuardarRutaPDF(Guid facturaID, string rutaPDF, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "UPDATE Facturas SET RutaPDF = @RutaPDF WHERE FacturaID = @FacturaID";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@FacturaID", facturaID);
                    cmd.Parameters.AddWithValue("@RutaPDF", rutaPDF);
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: " + ex.Message;
                return false;
            }
        }

        private Factura MapearFactura(SqlDataReader dr)
        {
            return new Factura
            {
                FacturaID = Guid.Parse(dr["FacturaID"].ToString()),
                VentaID = dr["VentaID"] != DBNull.Value ? Guid.Parse(dr["VentaID"].ToString()) : (Guid?)null,
                Serie = dr["Serie"].ToString(),
                Folio = dr["Folio"].ToString(),
                FechaEmision = Convert.ToDateTime(dr["FechaEmision"]),
                Version = dr["Version"].ToString(),
                TipoComprobante = dr["TipoComprobante"].ToString(),
                Subtotal = Convert.ToDecimal(dr["Subtotal"]),
                Total = Convert.ToDecimal(dr["Total"]),
                EmisorRFC = dr["EmisorRFC"].ToString(),
                EmisorNombre = dr["EmisorNombre"].ToString(),
                ReceptorRFC = dr["ReceptorRFC"].ToString(),
                ReceptorNombre = dr["ReceptorNombre"].ToString(),
                UUID = dr["UUID"]?.ToString(),
                Estatus = dr["Estatus"].ToString(),
                RutaPDF = dr["RutaPDF"]?.ToString(),
                XMLTimbrado = dr["XMLTimbrado"]?.ToString()
            };
        }

        /// <summary>
        /// Proceso completo: Crear factura → Generar XML → Timbrar → Guardar
        /// </summary>
        public async Task<Respuesta<object>> GenerarYTimbrarFactura(GenerarFacturaRequest request, string usuarioCreacion)
        {
            var respuesta = new Respuesta<object> { estado = false };

            try
            {
                // 1. Crear factura desde venta
                var factura = CrearFacturaDesdeVenta(request, out string mensajeCrear);
                if (factura == null)
                {
                    respuesta.valor = mensajeCrear;
                    return respuesta;
                }

                factura.UsuarioCreacion = usuarioCreacion;

                // 2. Generar XML sin timbrar
                var generadorXML = new CFDI40XMLGenerator();
                string xmlSinTimbrar = generadorXML.GenerarXML(factura);

                // Validar XML
                if (!generadorXML.ValidarXML(xmlSinTimbrar, out string errorValidacion))
                {
                    respuesta.valor = "Error en XML: " + errorValidacion;
                    return respuesta;
                }

                factura.XMLOriginal = xmlSinTimbrar;

                // 3. Guardar factura en BD (estatus PENDIENTE)
                if (!GuardarFactura(factura, out string mensajeGuardar))
                {
                    respuesta.valor = mensajeGuardar;
                    return respuesta;
                }

                // 4. Obtener configuración del PAC
                var config = ObtenerConfiguracionPAC(out string mensajeConfig);
                if (config == null)
                {
                    respuesta.valor = "Error al obtener configuración PAC: " + mensajeConfig;
                    return respuesta;
                }

                // 5. Timbrar con el PAC (Finkok u otro)
                IProveedorPAC proveedorPAC = ObtenerProveedorPAC(config.ProveedorPAC);
                var respuestaTimbrado = await proveedorPAC.TimbrarAsync(xmlSinTimbrar, config);

                if (!respuestaTimbrado.Exitoso)
                {
                    respuesta.valor = "Error al timbrar: " + respuestaTimbrado.Mensaje;
                    return respuesta;
                }

                // 6. Actualizar factura con datos del timbrado
                if (!ActualizarConTimbrado(factura.FacturaID, respuestaTimbrado, out string mensajeActualizar))
                {
                    respuesta.valor = mensajeActualizar;
                    return respuesta;
                }

                // 7. Respuesta exitosa
                respuesta.estado = true;
                respuesta.valor = "Factura timbrada exitosamente";
                respuesta.objeto = new
                {
                    FacturaID = factura.FacturaID,
                    UUID = respuestaTimbrado.UUID,
                    Serie = factura.Serie,
                    Folio = factura.Folio,
                    Total = factura.Total
                };
            }
            catch (Exception ex)
            {
                respuesta.valor = "Error general: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Cancela un CFDI con firma digital
        /// </summary>
        public async Task<RespuestaCancelacionCFDI> CancelarCFDI(int facturaId, string motivoCancelacion, 
            string uuidSustitucion, string usuario)
        {
            var respuesta = new RespuestaCancelacionCFDI();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();

                try
                {
                    // 1. Obtener datos de la factura
                    string queryFactura = @"
                        SELECT IdFactura, UUID, RFCEmisor, FechaTimbrado, MontoTotal, EsCancelada
                        FROM Factura
                        WHERE IdFactura = @FacturaId";

                    SqlCommand cmd = new SqlCommand(queryFactura, cnx, transaction);
                    cmd.Parameters.AddWithValue("@FacturaId", facturaId);

                    Factura factura = null;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            factura = new Factura
                            {
                                IdFactura = reader.GetInt32(0),
                                UUID = reader.IsDBNull(1) ? null : reader.GetString(1),
                                RFCEmisor = reader.IsDBNull(2) ? null : reader.GetString(2),
                                FechaTimbrado = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                MontoTotal = reader.GetDecimal(4),
                                EsCancelada = reader.GetBoolean(5)
                            };
                        }
                    }

                    if (factura == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Factura no encontrada";
                        return respuesta;
                    }

                    // 2. Validaciones
                    if (string.IsNullOrEmpty(factura.UUID))
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "La factura no tiene UUID (no está timbrada)";
                        return respuesta;
                    }

                    if (factura.EsCancelada)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "La factura ya está cancelada";
                        return respuesta;
                    }

                    // 3. Validar plazo de 72 horas
                    if (factura.FechaTimbrado.HasValue)
                    {
                        TimeSpan tiempoTranscurrido = DateTime.Now - factura.FechaTimbrado.Value;
                        if (tiempoTranscurrido.TotalHours > 72)
                        {
                            respuesta.Exitoso = false;
                            respuesta.Mensaje = "Ha excedido el plazo de 72 horas para cancelar el CFDI";
                            return respuesta;
                        }
                    }

                    // 4. Validar motivo y UUID de sustitución
                    if (motivoCancelacion == "01" && string.IsNullOrEmpty(uuidSustitucion))
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "El motivo 01 requiere UUID de sustitución";
                        return respuesta;
                    }

                    // 5. Registrar solicitud de cancelación
                    string queryInsertSolicitud = @"
                        INSERT INTO CFDICancelaciones 
                        (FacturaID, UUID, MotivoCancelacion, DescripcionMotivo, UUIDSustitucion, 
                         FechaSolicitud, EstadoCancelacion, UsuarioSolicita)
                        VALUES 
                        (@FacturaID, @UUID, @Motivo, @DescripcionMotivo, @UUIDSustitucion, 
                         GETDATE(), 'PENDIENTE', @Usuario);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    cmd = new SqlCommand(queryInsertSolicitud, cnx, transaction);
                    cmd.Parameters.AddWithValue("@FacturaID", facturaId);
                    cmd.Parameters.AddWithValue("@UUID", factura.UUID);
                    cmd.Parameters.AddWithValue("@Motivo", motivoCancelacion);
                    cmd.Parameters.AddWithValue("@DescripcionMotivo", ObtenerDescripcionMotivo(motivoCancelacion));
                    cmd.Parameters.AddWithValue("@UUIDSustitucion", (object)uuidSustitucion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    int cancelacionId = (int)cmd.ExecuteScalar();

                    // 6. Obtener configuración PAC
                    ConfiguracionPAC config = ObtenerConfiguracionPAC(cnx, transaction);

                    // 7. Llamar al PAC para cancelar
                    IProveedorPAC proveedorPAC = ObtenerProveedorPAC(config.ProveedorPAC);
                    var respuestaPAC = await proveedorPAC.CancelarAsync(
                        factura.UUID, 
                        factura.RFCEmisor, 
                        motivoCancelacion, 
                        uuidSustitucion, 
                        config
                    );

                    // Mapear respuesta del PAC al modelo de respuesta
                    respuesta.Exitoso = respuestaPAC.Exitoso;
                    respuesta.Mensaje = respuestaPAC.Mensaje;
                    respuesta.CodigoError = respuestaPAC.CodigoError;
                    respuesta.EstatusSAT = respuestaPAC.EstatusSAT;
                    respuesta.EstatusUUID = respuestaPAC.EstatusUUID;
                    respuesta.FechaRespuesta = respuestaPAC.FechaRespuesta ?? DateTime.Now;
                    respuesta.AcuseCancelacion = respuestaPAC.AcuseCancelacion;

                    // 8. Actualizar resultado de la cancelación
                    if (respuesta.Exitoso)
                    {
                        // Actualizar solicitud de cancelación
                        string queryUpdateCancelacion = @"
                            UPDATE CFDICancelaciones
                            SET FechaCancelacion = @FechaCancelacion,
                                EstadoCancelacion = 'ACEPTADA',
                                CodigoRespuesta = @CodigoRespuesta,
                                MensajeRespuesta = @MensajeRespuesta,
                                AcuseCancelacion = @AcuseCancelacion
                            WHERE CancelacionID = @CancelacionID";

                        cmd = new SqlCommand(queryUpdateCancelacion, cnx, transaction);
                        cmd.Parameters.AddWithValue("@CancelacionID", cancelacionId);
                        cmd.Parameters.AddWithValue("@FechaCancelacion", respuesta.FechaRespuesta);
                        cmd.Parameters.AddWithValue("@CodigoRespuesta", (object)respuesta.EstatusSAT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MensajeRespuesta", respuesta.Mensaje);
                        cmd.Parameters.AddWithValue("@AcuseCancelacion", (object)respuesta.AcuseCancelacion ?? DBNull.Value);
                        cmd.ExecuteNonQuery();

                        // Actualizar factura
                        string queryUpdateFactura = @"
                            UPDATE Factura
                            SET EsCancelada = 1,
                                FechaCancelacion = @FechaCancelacion,
                                MotivoCancelacion = @Motivo
                            WHERE IdFactura = @FacturaId";

                        cmd = new SqlCommand(queryUpdateFactura, cnx, transaction);
                        cmd.Parameters.AddWithValue("@FacturaId", facturaId);
                        cmd.Parameters.AddWithValue("@FechaCancelacion", respuesta.FechaRespuesta);
                        cmd.Parameters.AddWithValue("@Motivo", motivoCancelacion);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    else
                    {
                        // Actualizar solicitud con error
                        string queryUpdateError = @"
                            UPDATE CFDICancelaciones
                            SET EstadoCancelacion = 'ERROR',
                                CodigoRespuesta = @CodigoError,
                                MensajeRespuesta = @MensajeError
                            WHERE CancelacionID = @CancelacionID";

                        cmd = new SqlCommand(queryUpdateError, cnx, transaction);
                        cmd.Parameters.AddWithValue("@CancelacionID", cancelacionId);
                        cmd.Parameters.AddWithValue("@CodigoError", (object)respuesta.CodigoError ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MensajeError", respuesta.Mensaje);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Exitoso = false;
                    respuesta.Mensaje = "Error al procesar cancelación: " + ex.Message;
                }
            }

            return respuesta;
        }

        /// <summary>
        /// Obtiene la descripción del motivo de cancelación según catálogo SAT
        /// </summary>
        private string ObtenerDescripcionMotivo(string codigo)
        {
            switch (codigo)
            {
                case "01": return "Comprobante emitido con errores con relación";
                case "02": return "Comprobante emitido con errores sin relación";
                case "03": return "No se llevó a cabo la operación";
                case "04": return "Operación nominativa relacionada en la factura global";
                default: return "Motivo no especificado";
            }
        }

        /// <summary>
        /// Obtiene configuración PAC desde base de datos
        /// </summary>
        private ConfiguracionPAC ObtenerConfiguracionPAC(SqlConnection cnx, SqlTransaction transaction)
        {
            string query = @"
                SELECT TOP 1 ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta,
                       Usuario, Password, RutaCertificado, RutaLlavePrivada, PasswordLlavePrivada, TimeoutSegundos
                FROM ConfiguracionPAC
                WHERE Activo = 1";

            SqlCommand cmd = new SqlCommand(query, cnx, transaction);
            
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new ConfiguracionPAC
                    {
                        ProveedorPAC = reader.GetString(0),
                        EsProduccion = reader.GetBoolean(1),
                        UrlTimbrado = reader.GetString(2),
                        UrlCancelacion = reader.GetString(3),
                        UrlConsulta = reader.GetString(4),
                        Usuario = reader.GetString(5),
                        Password = reader.GetString(6),
                        RutaCertificado = reader.IsDBNull(7) ? null : reader.GetString(7),
                        RutaLlavePrivada = reader.IsDBNull(8) ? null : reader.GetString(8),
                        PasswordLlavePrivada = reader.IsDBNull(9) ? null : reader.GetString(9),
                        TimeoutSegundos = reader.GetInt32(10)
                    };
                }
            }

            // Si no hay configuración, retornar valores demo de Finkok
            return new ConfiguracionPAC
            {
                ProveedorPAC = "Finkok",
                EsProduccion = false,
                UrlTimbrado = "https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl",
                UrlCancelacion = "https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl",
                UrlConsulta = "https://demo-facturacion.finkok.com/servicios/soap/utilities.wsdl",
                Usuario = "usuario_demo",
                Password = "password_demo",
                TimeoutSegundos = 30
            };
        }

        /// <summary>
        /// Factory para obtener el proveedor PAC según configuración
        /// </summary>
        private IProveedorPAC ObtenerProveedorPAC(string nombrePAC)
        {
            switch (nombrePAC.ToUpper())
            {
                case "FINKOK":
                    return new FinkokPAC();
                
                // Agregar más proveedores aquí:
                // case "SW-SAPIEN":
                //     return new SWSapienPAC();
                // case "DIVERZA":
                //     return new DiverzaPAC();
                
                default:
                    return new FinkokPAC(); // Por defecto
            }
        }
    }

}
