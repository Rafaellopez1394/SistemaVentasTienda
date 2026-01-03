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
                        SELECT FacturaID, VentaID, Serie, Folio, UUID, 
                               FechaEmision, FechaTimbrado, 
                               EmisorRFC, EmisorNombre, EmisorRegimenFiscal,
                               ReceptorRFC, ReceptorNombre, ReceptorUsoCFDI, 
                               ReceptorDomicilioFiscalCP, ReceptorRegimenFiscalReceptor, ReceptorEmail,
                               Subtotal, TotalImpuestosTrasladados, TotalImpuestosRetenidos, Total,
                               FormaPago, MetodoPago,
                               Estatus, XMLTimbrado, SelloCFD, SelloSAT, NoCertificadoSAT,
                               FechaCancelacion, MotivoCancelacion,
                               UsuarioCreacion, FechaCreacion
                        FROM Facturas
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
                                Conceptos = new List<FacturaDetalle>(), // Explicit initialization
                                FacturaID = (Guid)dr["FacturaID"],
                                VentaID = dr["VentaID"] != DBNull.Value ? (Guid)dr["VentaID"] : Guid.Empty,
                                Serie = dr["Serie"]?.ToString(),
                                Folio = dr["Folio"]?.ToString(),
                                UUID = dr["UUID"]?.ToString(),
                                FechaEmision = Convert.ToDateTime(dr["FechaEmision"]),
                                FechaTimbrado = dr["FechaTimbrado"] != DBNull.Value ? (DateTime?)dr["FechaTimbrado"] : null,
                                RFCEmisor = dr["EmisorRFC"]?.ToString(),
                                NombreEmisor = dr["EmisorNombre"]?.ToString(),
                                RegimenFiscalEmisor = dr["EmisorRegimenFiscal"]?.ToString(),
                                ReceptorRFC = dr["ReceptorRFC"]?.ToString(),
                                ReceptorNombre = dr["ReceptorNombre"]?.ToString(),
                                ReceptorUsoCFDI = dr["ReceptorUsoCFDI"]?.ToString(),
                                ReceptorDomicilioFiscalCP = dr["ReceptorDomicilioFiscalCP"]?.ToString(),
                                ReceptorRegimenFiscalReceptor = dr["ReceptorRegimenFiscalReceptor"]?.ToString(),
                                ReceptorEmail = dr["ReceptorEmail"]?.ToString(),
                                Subtotal = dr["Subtotal"] != DBNull.Value ? Convert.ToDecimal(dr["Subtotal"]) : 0,
                                TotalImpuestosTrasladados = dr["TotalImpuestosTrasladados"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosTrasladados"]) : 0,
                                TotalImpuestosRetenidos = dr["TotalImpuestosRetenidos"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosRetenidos"]) : 0,
                                Total = Convert.ToDecimal(dr["Total"]),
                                MontoTotal = Convert.ToDecimal(dr["Total"]),
                                FormaPago = dr["FormaPago"]?.ToString(),
                                MetodoPago = dr["MetodoPago"]?.ToString(),
                                Estatus = dr["Estatus"]?.ToString(),
                                XMLTimbrado = dr["XMLTimbrado"]?.ToString(),
                                SelloCFD = dr["SelloCFD"]?.ToString(),
                                SelloSAT = dr["SelloSAT"]?.ToString(),
                                NoCertificadoSAT = dr["NoCertificadoSAT"]?.ToString(),
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
        /// Obtiene una factura por su ID
        /// </summary>
        public Factura ObtenerPorId(Guid facturaId)
        {
            Factura factura = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT FacturaID, VentaID, Serie, Folio, UUID, 
                               FechaEmision, FechaTimbrado, 
                               EmisorRFC, EmisorNombre, EmisorRegimenFiscal,
                               ReceptorRFC, ReceptorNombre, ReceptorUsoCFDI, 
                               ReceptorDomicilioFiscalCP, ReceptorRegimenFiscalReceptor, ReceptorEmail,
                               Subtotal, TotalImpuestosTrasladados, TotalImpuestosRetenidos, Total,
                               FormaPago, MetodoPago,
                               Estatus, XMLTimbrado, SelloCFD, SelloSAT, NoCertificadoSAT,
                               FechaCancelacion, MotivoCancelacion,
                               UsuarioCreacion, FechaCreacion
                        FROM Facturas
                        WHERE FacturaID = @FacturaID";

                    using (SqlCommand cmd = new SqlCommand(query, cnx))
                    {
                        cmd.Parameters.AddWithValue("@FacturaID", facturaId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                factura = new Factura
                                {
                                    Conceptos = new List<FacturaDetalle>(), // Explicit initialization
                                    FacturaID = (Guid)dr["FacturaID"],
                                    VentaID = dr["VentaID"] != DBNull.Value ? (Guid)dr["VentaID"] : Guid.Empty,
                                    Serie = dr["Serie"]?.ToString(),
                                    Folio = dr["Folio"]?.ToString(),
                                    UUID = dr["UUID"]?.ToString(),
                                    FechaEmision = Convert.ToDateTime(dr["FechaEmision"]),
                                    FechaTimbrado = dr["FechaTimbrado"] != DBNull.Value ? (DateTime?)dr["FechaTimbrado"] : null,
                                    RFCEmisor = dr["EmisorRFC"]?.ToString(),
                                    NombreEmisor = dr["EmisorNombre"]?.ToString(),
                                    RegimenFiscalEmisor = dr["EmisorRegimenFiscal"]?.ToString(),
                                    ReceptorRFC = dr["ReceptorRFC"]?.ToString(),
                                    ReceptorNombre = dr["ReceptorNombre"]?.ToString(),
                                    ReceptorUsoCFDI = dr["ReceptorUsoCFDI"]?.ToString(),
                                    ReceptorDomicilioFiscalCP = dr["ReceptorDomicilioFiscalCP"]?.ToString(),
                                    ReceptorRegimenFiscalReceptor = dr["ReceptorRegimenFiscalReceptor"]?.ToString(),
                                    ReceptorEmail = dr["ReceptorEmail"]?.ToString(),
                                    Subtotal = dr["Subtotal"] != DBNull.Value ? Convert.ToDecimal(dr["Subtotal"]) : 0,
                                    TotalImpuestosTrasladados = dr["TotalImpuestosTrasladados"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosTrasladados"]) : 0,
                                    TotalImpuestosRetenidos = dr["TotalImpuestosRetenidos"] != DBNull.Value ? Convert.ToDecimal(dr["TotalImpuestosRetenidos"]) : 0,
                                    Total = Convert.ToDecimal(dr["Total"]),
                                    MontoTotal = Convert.ToDecimal(dr["Total"]),
                                    FormaPago = dr["FormaPago"]?.ToString(),
                                    MetodoPago = dr["MetodoPago"]?.ToString(),
                                    Estatus = dr["Estatus"]?.ToString(),
                                    XMLTimbrado = dr["XMLTimbrado"]?.ToString(),
                                    SelloCFD = dr["SelloCFD"]?.ToString(),
                                    SelloSAT = dr["SelloSAT"]?.ToString(),
                                    NoCertificadoSAT = dr["NoCertificadoSAT"]?.ToString(),
                                    FechaCancelacion = dr["FechaCancelacion"] != DBNull.Value ? (DateTime?)dr["FechaCancelacion"] : null,
                                    MotivoCancelacion = dr["MotivoCancelacion"]?.ToString(),
                                    UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                    FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : DateTime.Now
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerPorId: {ex.Message}");
            }

            return factura;
        }

        /// <summary>
        /// Obtiene el detalle de conceptos de una factura
        /// </summary>
        public List<FacturaDetalle> ObtenerDetalleFactura(Guid facturaId)
        {
            List<FacturaDetalle> detalles = new List<FacturaDetalle>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT FacturaDetalleID, FacturaID, Secuencia,
                               ClaveProdServ, NoIdentificacion, Cantidad, ClaveUnidad, Unidad,
                               Descripcion, ValorUnitario, Importe, Descuento, ObjetoImp
                        FROM FacturasDetalle
                        WHERE FacturaID = @FacturaID
                        ORDER BY Secuencia";

                    using (SqlCommand cmd = new SqlCommand(query, cnx))
                    {
                        cmd.Parameters.AddWithValue("@FacturaID", facturaId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                detalles.Add(new FacturaDetalle
                                {
                                    FacturaDetalleID = 0, // No se usa el Guid de la BD aquí
                                    FacturaID = (Guid)dr["FacturaID"],
                                    Secuencia = Convert.ToInt32(dr["Secuencia"]),
                                    ClaveProdServ = dr["ClaveProdServ"]?.ToString(),
                                    NoIdentificacion = dr["NoIdentificacion"]?.ToString(),
                                    Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                    ClaveUnidad = dr["ClaveUnidad"]?.ToString(),
                                    Unidad = dr["Unidad"]?.ToString(),
                                    Descripcion = dr["Descripcion"]?.ToString(),
                                    ValorUnitario = Convert.ToDecimal(dr["ValorUnitario"]),
                                    Importe = Convert.ToDecimal(dr["Importe"]),
                                    Descuento = dr["Descuento"] != DBNull.Value ? Convert.ToDecimal(dr["Descuento"]) : 0,
                                    ObjetoImp = dr["ObjetoImp"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerDetalleFactura: {ex.Message}");
            }

            return detalles;
        }

        /// <summary>
        /// Obtiene los impuestos trasladados/retenidos de una factura
        /// </summary>
        public List<FacturaImpuesto> ObtenerImpuestosFactura(Guid facturaId)
        {
            List<FacturaImpuesto> impuestos = new List<FacturaImpuesto>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT FacturaImpuestoID, FacturaID, FacturaDetalleID,
                               TipoImpuesto, Impuesto, TipoFactor, TasaOCuota, Base, Importe
                        FROM FacturasImpuestos
                        WHERE FacturaID = @FacturaID
                        ORDER BY FacturaDetalleID, TipoImpuesto";

                    using (SqlCommand cmd = new SqlCommand(query, cnx))
                    {
                        cmd.Parameters.AddWithValue("@FacturaID", facturaId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                impuestos.Add(new FacturaImpuesto
                                {
                                    ImpuestoID = 0, // No se usa el Guid de la BD
                                    FacturaDetalleID = 0, // No se mapea el Guid aquí
                                    TipoImpuesto = dr["TipoImpuesto"]?.ToString(),
                                    Impuesto = dr["Impuesto"]?.ToString(),
                                    TipoFactor = dr["TipoFactor"]?.ToString(),
                                    TasaOCuota = dr["TasaOCuota"] != DBNull.Value ? Convert.ToDecimal(dr["TasaOCuota"]) : (decimal?)null,
                                    Base = Convert.ToDecimal(dr["Base"]),
                                    Importe = dr["Importe"] != DBNull.Value ? Convert.ToDecimal(dr["Importe"]) : (decimal?)null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerImpuestosFactura: {ex.Message}");
            }

            return impuestos;
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
                        SELECT v.VentaID, v.Total AS TotalVenta, v.FechaVenta,
                               dv.ProductoID, p.CodigoInterno, p.Nombre, dv.Cantidad, 
                               dv.PrecioVenta, 
                               (dv.Cantidad * dv.PrecioVenta) AS Subtotal,
                               ISNULL(dv.TasaIVA, 0) AS TasaIVA, 
                               ISNULL(dv.MontoIVA, 0) AS MontoIVA,
                               ISNULL(p.Exento, 0) AS Exento,
                               p.ClaveProdServSAT,
                               p.ClaveUnidadSAT
                        FROM VentasClientes v
                        INNER JOIN VentasDetalleClientes dv ON v.VentaID = dv.VentaID
                        INNER JOIN Productos p ON dv.ProductoID = p.ProductoID
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
                        ReceptorUsoCFDI = request.ReceptorUsoCFDI ?? "G03", // Por defecto: Gastos en general
                        ReceptorDomicilioFiscalCP = request.ReceptorCP ?? "00000",
                        ReceptorRegimenFiscalReceptor = request.ReceptorRegimenFiscal ?? "616",
                        ReceptorEmail = request.ReceptorEmail,
                        FormaPago = request.FormaPago ?? "99",
                        MetodoPago = request.MetodoPago ?? "PUE",
                        Estatus = "PENDIENTE",
                        Version = "4.0",
                        TipoComprobante = "I",
                        ProveedorPAC = "Facturama", // TODO: Obtener de configuración
                        Conceptos = new List<FacturaDetalle>() // Explicit initialization to be safe
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
                        decimal tasaIVA = Convert.ToDecimal(dr["TasaIVA"]);
                        decimal montoIVA = Convert.ToDecimal(dr["MontoIVA"]);
                        bool esExento = Convert.ToBoolean(dr["Exento"]);
                        decimal subtotal = Convert.ToDecimal(dr["Subtotal"]);
                        
                        // Determinar el ObjetoImp según si tiene impuestos
                        string objetoImp = "01"; // No objeto de impuestos (por defecto)
                        
                        if (esExento)
                        {
                            objetoImp = "01"; // No objeto de impuestos (exento)
                        }
                        else if (tasaIVA > 0)
                        {
                            objetoImp = "02"; // Sí objeto de impuestos
                        }
                        
                        var detalle = new FacturaDetalle
                        {
                            Secuencia = secuencia++,
                            ClaveProdServ = dr["ClaveProdServSAT"]?.ToString() ?? "01010101",
                            NoIdentificacion = dr["CodigoInterno"]?.ToString(),
                            Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                            ClaveUnidad = dr["ClaveUnidadSAT"]?.ToString() ?? "H87",
                            Unidad = "Pieza",
                            Descripcion = dr["Nombre"].ToString(),
                            ValorUnitario = Convert.ToDecimal(dr["PrecioVenta"]),
                            Importe = subtotal,
                            Descuento = 0,
                            ObjetoImp = objetoImp
                        };

                        // Solo agregar impuesto si NO es exento Y tiene tasa de IVA
                        if (!esExento && tasaIVA > 0)
                        {
                            var impuesto = new FacturaImpuesto
                            {
                                TipoImpuesto = "TRASLADO",
                                Impuesto = "002", // IVA
                                TipoFactor = "Tasa",
                                TasaOCuota = tasaIVA / 100,
                                Base = detalle.Importe,
                                Importe = montoIVA
                            };
                            detalle.Impuestos.Add(impuesto);
                            detalle.TotalImpuestosTrasladados = montoIVA;
                            impuestosTotal += montoIVA;
                        }

                        factura.Conceptos.Add(detalle);
                        subtotalTotal += detalle.Importe;
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
                        // Resolver dinámica de columna de password de llave (Privada vs Llave)
                        string pwdLlave = null;
                        try
                        {
                            int idxPrivada = dr.GetOrdinal("PasswordLlavePrivada");
                            if (idxPrivada >= 0 && !dr.IsDBNull(idxPrivada))
                                pwdLlave = dr.GetString(idxPrivada);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // No existe columna PasswordLlavePrivada
                        }
                        if (pwdLlave == null)
                        {
                            try
                            {
                                int idxLlave = dr.GetOrdinal("PasswordLlave");
                                if (idxLlave >= 0 && !dr.IsDBNull(idxLlave))
                                    pwdLlave = dr.GetString(idxLlave);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // No existe PasswordLlave; dejar null
                            }
                        }

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
                            PasswordLlavePrivada = pwdLlave,
                            PasswordLlave = pwdLlave,
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
                    // No duplicar "Error al timbrar" si ya viene en el mensaje
                    respuesta.valor = respuestaTimbrado.Mensaje.Contains("Error") 
                        ? respuestaTimbrado.Mensaje 
                        : "Error al timbrar: " + respuestaTimbrado.Mensaje;
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
        public async Task<RespuestaCancelacionCFDI> CancelarCFDI(Guid facturaId, string motivoCancelacion, 
            string uuidSustitucion, string usuario)
        {
            var respuesta = new RespuestaCancelacionCFDI();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();

                try
                {
                    System.Diagnostics.Debug.WriteLine($"=== INICIO CANCELACIÓN ===");
                    System.Diagnostics.Debug.WriteLine($"FacturaID: {facturaId}");
                    System.Diagnostics.Debug.WriteLine($"Motivo: {motivoCancelacion}");

                    // 1. Obtener datos de la factura
                    System.Diagnostics.Debug.WriteLine("Paso 1: Consultando factura...");
                    string queryFactura = @"
                        SELECT FacturaID, UUID, EmisorRFC, FechaTimbrado, Estatus
                        FROM Facturas
                        WHERE FacturaID = @FacturaId";

                    SqlCommand cmd = new SqlCommand(queryFactura, cnx, transaction);
                    cmd.Parameters.AddWithValue("@FacturaId", facturaId);

                    System.Diagnostics.Debug.WriteLine("Ejecutando consulta SQL...");

                    Factura factura = null;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                factura = new Factura
                                {
                                    Conceptos = new List<FacturaDetalle>()
                                };
                                
                                // Leer cada campo individualmente con manejo de errores (conversiones seguras)
                                try {
                                    var v = reader.GetValue(0);
                                    if (v == null || v == DBNull.Value) throw new Exception("FacturaID nulo");
                                    factura.FacturaID = v is Guid g ? g : Guid.Parse(v.ToString());
                                }
                                catch (Exception ex) { throw new Exception($"Error leyendo FacturaID: {ex.Message}"); }

                                try {
                                    var v = reader.GetValue(1);
                                    factura.UUID = (v == null || v == DBNull.Value) ? null : v.ToString();
                                }
                                catch (Exception ex) { throw new Exception($"Error leyendo UUID: {ex.Message}"); }

                                try {
                                    var v = reader.GetValue(2);
                                    factura.EmisorRFC = (v == null || v == DBNull.Value) ? null : v.ToString();
                                }
                                catch (Exception ex) { throw new Exception($"Error leyendo EmisorRFC: {ex.Message}"); }

                                try {
                                    if (reader.IsDBNull(3)) {
                                        factura.FechaTimbrado = null;
                                    } else {
                                        var v = reader.GetValue(3);
                                        factura.FechaTimbrado = Convert.ToDateTime(v);
                                    }
                                }
                                catch (Exception ex) { throw new Exception($"Error leyendo FechaTimbrado: {ex.Message}"); }

                                try {
                                    var v = reader.GetValue(4);
                                    string estatus = (v == null || v == DBNull.Value) ? string.Empty : v.ToString();
                                    factura.EsCancelada = estatus.Equals("CANCELADA", StringComparison.OrdinalIgnoreCase);
                                }
                                catch (Exception ex) { throw new Exception($"Error leyendo Estatus: {ex.Message}"); }
                            }
                            catch (InvalidCastException ex)
                            {
                                respuesta.Exitoso = false;
                                respuesta.Mensaje = $"Error de conversión al leer datos de factura: {ex.Message}";
                                return respuesta;
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Factura leída correctamente");

                    if (factura == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Factura no encontrada");
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "Factura no encontrada";
                        return respuesta;
                    }

                    System.Diagnostics.Debug.WriteLine("Paso 2: Validaciones...");

                    // 2. Validaciones
                    if (string.IsNullOrEmpty(factura.UUID))
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: UUID vacío");
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "La factura no tiene UUID (no está timbrada)";
                        return respuesta;
                    }

                    if (factura.EsCancelada)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Factura ya cancelada");
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "La factura ya está cancelada";
                        return respuesta;
                    }

                    System.Diagnostics.Debug.WriteLine("Paso 3: Validando plazo...");

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

                    // 5. Registrar solicitud de cancelación (skip if table doesn't exist or has wrong schema)
                    int cancelacionId = -1;
                    try
                    {
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

                        // UUID debe ser UNIQUEIDENTIFIER; intentar parsear
                        Guid guidUuid;
                        if (!Guid.TryParse(factura.UUID, out guidUuid))
                        {
                            throw new Exception("UUID de la factura no es un GUID válido");
                        }
                        var pUuid = new SqlParameter("@UUID", SqlDbType.UniqueIdentifier) { Value = guidUuid };
                        cmd.Parameters.Add(pUuid);

                        cmd.Parameters.AddWithValue("@Motivo", motivoCancelacion);
                        cmd.Parameters.AddWithValue("@DescripcionMotivo", ObtenerDescripcionMotivo(motivoCancelacion));

                        // UUIDSustitucion solo aplica para motivo 01 y debe ser GUID válido
                        if (motivoCancelacion == "01" && !string.IsNullOrWhiteSpace(uuidSustitucion) && Guid.TryParse(uuidSustitucion, out var guidSust))
                        {
                            var pSust = new SqlParameter("@UUIDSustitucion", SqlDbType.UniqueIdentifier) { Value = guidSust };
                            cmd.Parameters.Add(pSust);
                        }
                        else
                        {
                            var pSust = new SqlParameter("@UUIDSustitucion", SqlDbType.UniqueIdentifier) { Value = DBNull.Value };
                            cmd.Parameters.Add(pSust);
                        }

                        cmd.Parameters.AddWithValue("@Usuario", usuario);

                        object result = cmd.ExecuteScalar();
                        cancelacionId = result != null ? Convert.ToInt32(result) : -1;
                    }
                    catch (SqlException)
                    {
                        // Table doesn't exist or has incompatible schema - continue without logging to CFDICancelaciones
                        System.Diagnostics.Debug.WriteLine("CFDICancelaciones table not available, skipping audit log");
                        cancelacionId = -1;
                    }

                    // 6. Obtener configuración PAC
                    ConfiguracionPAC config = ObtenerConfiguracionPAC(cnx, transaction);
                    
                    if (config == null)
                    {
                        respuesta.Exitoso = false;
                        respuesta.Mensaje = "No se encontró configuración del PAC activa";
                        transaction.Rollback();
                        return respuesta;
                    }

                    // 7. Llamar al PAC para cancelar
                    IProveedorPAC proveedorPAC = ObtenerProveedorPAC(config.ProveedorPAC);
                    var respuestaPAC = await proveedorPAC.CancelarAsync(
                        factura.UUID, 
                        factura.EmisorRFC, 
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
                        // Actualizar solicitud de cancelación (if table exists and record was created)
                        if (cancelacionId > 0)
                        {
                            try
                            {
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
                            }
                            catch (SqlException)
                            {
                                // Ignore if table doesn't exist
                            }
                        }

                        // Actualizar factura
                        string queryUpdateFactura = @"
                            UPDATE Facturas
                            SET Estatus = 'CANCELADA',
                                FechaCancelacion = @FechaCancelacion,
                                MotivoCancelacion = @Motivo
                            WHERE UUID = @UUID";

                        cmd = new SqlCommand(queryUpdateFactura, cnx, transaction);
                        cmd.Parameters.AddWithValue("@UUID", factura.UUID ?? string.Empty);
                        cmd.Parameters.AddWithValue("@FechaCancelacion", respuesta.FechaRespuesta);
                        cmd.Parameters.AddWithValue("@Motivo", motivoCancelacion);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    else
                    {
                        // Actualizar solicitud con error (if table exists and record was created)
                        if (cancelacionId > 0)
                        {
                            try
                            {
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
                            }
                            catch (SqlException)
                            {
                                // Ignore if table doesn't exist
                            }
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Exitoso = false;
                    // Log detallado y mensaje claro (evitar interpolar métodos void)
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    respuesta.Mensaje = $"Error al procesar cancelación: {ex.Message}";
                    
                    // Log detallado para debugging
                    System.Diagnostics.Debug.WriteLine($"=== ERROR CANCELACIÓN ===");
                    System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Tipo: {ex.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                    System.Diagnostics.Debug.WriteLine($"=========================");
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
            // Detectar nombre de columna para el password de la llave privada
            string passwordColumn = null;
            using (var colCmd = new SqlCommand(@"SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('ConfiguracionPAC') AND name = 'PasswordLlavePrivada'", cnx, transaction))
            {
                object existsPrivada = colCmd.ExecuteScalar();
                if (existsPrivada != null && Convert.ToInt32(existsPrivada) > 0)
                {
                    passwordColumn = "PasswordLlavePrivada";
                }
                else
                {
                    using (var colCmdAlt = new SqlCommand(@"SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('ConfiguracionPAC') AND name = 'PasswordLlave'", cnx, transaction))
                    {
                        object existsLlave = colCmdAlt.ExecuteScalar();
                        if (existsLlave != null && Convert.ToInt32(existsLlave) > 0)
                        {
                            passwordColumn = "PasswordLlave";
                        }
                    }
                }
            }

            string query = @"
                SELECT TOP 1 ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta,
                       Usuario, Password, RutaCertificado, RutaLlavePrivada, {0}, TimeoutSegundos
                FROM ConfiguracionPAC
                WHERE Activo = 1";

            // Si no existe ninguna columna de password para llave, devolver configuración por defecto
            if (string.IsNullOrEmpty(passwordColumn))
            {
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

            query = string.Format(query, passwordColumn);
            SqlCommand cmd = new SqlCommand(query, cnx, transaction);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Índices: 0=ProveedorPAC,1=EsProduccion,2=UrlTimbrado,3=UrlCancelacion,4=UrlConsulta,
                    // 5=Usuario,6=Password,7=RutaCertificado,8=RutaLlavePrivada,9=PasswordLlave(Privada),10=TimeoutSegundos
                    var pwdLlave = reader.IsDBNull(9) ? null : reader.GetString(9);
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
                        PasswordLlavePrivada = pwdLlave,
                        PasswordLlave = pwdLlave,
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
                
                case "FACTURAMA":
                    return new FacturamaPAC();
                
                case "SIMULADOR":
                    return new SimuladorPAC();
                
                // Agregar más proveedores aquí:
                // case "SW-SAPIEN":
                //     return new SWSapienPAC();
                // case "DIVERZA":
                //     return new DiverzaPAC();
                
                default:
                    return new SimuladorPAC(); // Por defecto: simulador para pruebas
            }
        }
    }

}
