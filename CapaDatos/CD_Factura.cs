using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CapaModelo;
using CapaDatos.PAC;
using CapaDatos.Generadores;

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

            System.Diagnostics.Debug.WriteLine($"========================================");
            System.Diagnostics.Debug.WriteLine($"=== CrearFacturaDesdeVenta INICIO ===");
            System.Diagnostics.Debug.WriteLine($"========================================");
            
            if (request == null)
            {
                mensaje = "ERROR: request es null";
                System.Diagnostics.Debug.WriteLine("❌ ERROR: request es null");
                return null;
            }
            
            System.Diagnostics.Debug.WriteLine($"Request recibido:");
            System.Diagnostics.Debug.WriteLine($"  VentaID: {request.VentaID}");
            System.Diagnostics.Debug.WriteLine($"  ReceptorRFC: {request.ReceptorRFC}");
            System.Diagnostics.Debug.WriteLine($"  ReceptorNombre: {request.ReceptorNombre}");
            System.Diagnostics.Debug.WriteLine($"  ReceptorUsoCFDI: {request.ReceptorUsoCFDI}");
            System.Diagnostics.Debug.WriteLine($"  UsoCFDI: {request.UsoCFDI}");
            System.Diagnostics.Debug.WriteLine($"  FormaPago: {request.FormaPago}");
            System.Diagnostics.Debug.WriteLine($"  MetodoPago: {request.MetodoPago}");
            System.Diagnostics.Debug.WriteLine($"  ReceptorCP: {request.ReceptorCP}");
            System.Diagnostics.Debug.WriteLine($"  ReceptorRegimenFiscal: {request.ReceptorRegimenFiscal}");

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    // 1. Obtener configuración PAC activa primero
                    var configPAC = ObtenerConfiguracionPAC(out string mensajePAC);
                    if (configPAC == null)
                    {
                        mensaje = "ERROR: No hay configuración de PAC activa. " + mensajePAC;
                        return null;
                    }

                    // 2. Obtener emisor de configuración
                    string queryEmisor = @"SELECT RFC, RazonSocial, RegimenFiscal, CodigoPostal, 
                                           NombreArchivoCertificado, NombreArchivoLlavePrivada, NombreArchivoPassword 
                                           FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1";
                    SqlCommand cmdEmisor = new SqlCommand(queryEmisor, cnx);
                    SqlDataReader drEmisor = cmdEmisor.ExecuteReader();
                    
                    string emisorRFC = null, emisorNombre = null, emisorRegimen = null, lugarExpedicion = null;
                    string nombreArchivoCert = null, nombreArchivoKey = null, nombreArchivoPassword = null;
                    
                    if (drEmisor.Read())
                    {
                        emisorRFC = drEmisor["RFC"].ToString();
                        emisorNombre = drEmisor["RazonSocial"].ToString();
                        emisorRegimen = drEmisor["RegimenFiscal"].ToString();
                        lugarExpedicion = drEmisor["CodigoPostal"].ToString();
                        nombreArchivoCert = drEmisor["NombreArchivoCertificado"]?.ToString();
                        nombreArchivoKey = drEmisor["NombreArchivoLlavePrivada"]?.ToString();
                        nombreArchivoPassword = drEmisor["NombreArchivoPassword"]?.ToString();
                    }
                    else
                    {
                        drEmisor.Close();
                        mensaje = "ERROR: No se encontró la configuración del emisor en ConfiguracionEmpresa";
                        return null;
                    }
                    drEmisor.Close();

                    // 3. Obtener datos de la venta
                    string queryVenta = @"
                        SELECT v.VentaID, v.Total AS TotalVenta, v.FechaVenta,
                               v.TipoVenta, 
                               ISNULL(v.MontoPagado, v.Total) AS MontoPagado,
                               ISNULL(v.SaldoPendiente, 0) AS SaldoPendiente,
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

                    // Leer la primera fila para obtener el SaldoPendiente
                    dr.Read();
                    decimal saldoPendienteVenta = Convert.ToDecimal(dr["SaldoPendiente"]);
                    string tipoVenta = dr["TipoVenta"]?.ToString() ?? "CONTADO";
                    
                    // Determinar el método de pago según el saldo pendiente
                    string metodoPago = saldoPendienteVenta > 0 ? "PPD" : (request.MetodoPago ?? "PUE");

                    factura = new Factura
                    {
                        FacturaID = Guid.NewGuid(),
                        VentaID = request.VentaID,
                        FechaEmision = DateTime.Now,
                        ReceptorRFC = request.ReceptorRFC.ToUpper(),
                        ReceptorNombre = request.ReceptorNombre,
                        // Soportar tanto ReceptorUsoCFDI como UsoCFDI
                        ReceptorUsoCFDI = request.ReceptorUsoCFDI ?? request.UsoCFDI ?? "G03", 
                        ReceptorDomicilioFiscalCP = request.ReceptorCP ?? "00000",
                        ReceptorRegimenFiscalReceptor = request.ReceptorRegimenFiscal ?? "616",
                        ReceptorEmail = request.ReceptorEmail,
                        FormaPago = request.FormaPago ?? "99",
                        MetodoPago = metodoPago,
                        SaldoPendiente = saldoPendienteVenta, // Saldo insoluto para XML
                        Estatus = "PENDIENTE",
                        Version = "4.0",
                        TipoComprobante = "I",
                        ProveedorPAC = configPAC.ProveedorPAC, // Obtener de configuración activa
                        Moneda = "MXN",
                        TipoCambio = 1,
                        Exportacion = "01", // 01 = No aplica
                        LugarExpedicion = lugarExpedicion,
                        EmisorRFC = emisorRFC,
                        EmisorNombre = emisorNombre,
                        EmisorRegimenFiscal = emisorRegimen,
                        EmisorNombreArchivoCertificado = nombreArchivoCert,
                        EmisorNombreArchivoLlavePrivada = nombreArchivoKey,
                        EmisorNombreArchivoPassword = nombreArchivoPassword,
                        Conceptos = new List<FacturaDetalle>() // Explicit initialization to be safe
                    };

                    decimal subtotalTotal = 0;
                    decimal impuestosTotal = 0;
                    int secuencia = 1;

                    // Procesar los detalles de la venta
                    // NOTA: Ya hicimos dr.Read() arriba, así que el reader ya está en la primera fila
                    do
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
                        
                        // Validación de ClaveProdServ con logging
                        string claveProdServDB = dr["ClaveProdServSAT"]?.ToString();
                        string codigoInterno = dr["CodigoInterno"]?.ToString();
                        string claveProdServ = "01010101"; // Default
                        
                        System.Diagnostics.Debug.WriteLine($"[CD_Factura] Producto: {dr["Nombre"]}");
                        System.Diagnostics.Debug.WriteLine($"[CD_Factura] ClaveProdServSAT DB: '{claveProdServDB}' (Length: {claveProdServDB?.Length ?? 0})");
                        System.Diagnostics.Debug.WriteLine($"[CD_Factura] CodigoInterno DB: '{codigoInterno}'");
                        
                        if (!string.IsNullOrWhiteSpace(claveProdServDB) && claveProdServDB.Length == 8)
                        {
                            claveProdServ = claveProdServDB;
                            System.Diagnostics.Debug.WriteLine($"[CD_Factura] ✅ Usando ClaveProdServ de BD: {claveProdServ}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[CD_Factura] ⚠️ ClaveProdServ inválido, usando default: {claveProdServ}");
                        }
                        
                        var detalle = new FacturaDetalle
                        {
                            Secuencia = secuencia++,
                            ClaveProdServ = claveProdServ,
                            NoIdentificacion = !string.IsNullOrWhiteSpace(codigoInterno) ? codigoInterno : "N/A",
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
                    while (dr.Read()); // Continuar procesando filas
                    
                    dr.Close();

                    factura.Subtotal = subtotalTotal;
                    factura.TotalImpuestosTrasladados = impuestosTotal;
                    factura.Total = subtotalTotal + impuestosTotal;

                    System.Diagnostics.Debug.WriteLine($"✅ Conceptos agregados: {factura.Conceptos.Count}");
                    System.Diagnostics.Debug.WriteLine($"   Subtotal: {subtotalTotal:C}, IVA: {impuestosTotal:C}, Total: {factura.Total:C}");

                    // 2. Generar Serie y Folio
                    factura.Serie = "A";
                    SqlCommand cmdFolio = new SqlCommand("GenerarFolioFactura", cnx);
                    cmdFolio.CommandType = CommandType.StoredProcedure;
                    cmdFolio.Parameters.AddWithValue("@Serie", factura.Serie);
                    SqlParameter pFolio = new SqlParameter("@Folio", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmdFolio.Parameters.Add(pFolio);
                    cmdFolio.ExecuteNonQuery();
                    factura.Folio = pFolio.Value.ToString();

                    System.Diagnostics.Debug.WriteLine($"✅ Serie/Folio generado: {factura.Serie}-{factura.Folio}");
                    System.Diagnostics.Debug.WriteLine($"========================================");
                    System.Diagnostics.Debug.WriteLine($"=== CrearFacturaDesdeVenta COMPLETADO ===");
                    System.Diagnostics.Debug.WriteLine($"========================================");

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
                                XMLOriginal, UUID, XMLTimbrado, FechaTimbrado, SelloCFD, SelloSAT, 
                                NoCertificadoSAT, CadenaOriginalSAT, UsuarioCreacion, FechaCreacion
                            ) VALUES (
                                @FacturaID, @VentaID, @Serie, @Folio, @FechaEmision, @Version, @TipoComprobante,
                                @Subtotal, @Descuento, @Total, @TotalImpuestosTrasladados, @TotalImpuestosRetenidos,
                                @EmisorRFC, @EmisorNombre, @EmisorRegimenFiscal,
                                @ReceptorRFC, @ReceptorNombre, @ReceptorUsoCFDI, @ReceptorDomicilioFiscalCP,
                                @ReceptorRegimenFiscalReceptor, @ReceptorEmail,
                                @FormaPago, @MetodoPago, @ProveedorPAC, @Estatus,
                                @XMLOriginal, @UUID, @XMLTimbrado, @FechaTimbrado, @SelloCFD, @SelloSAT,
                                @NoCertificadoSAT, @CadenaOriginalSAT, @UsuarioCreacion, GETDATE()
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
                        cmdFactura.Parameters.AddWithValue("@UUID", factura.UUID ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@XMLTimbrado", factura.XMLTimbrado ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@FechaTimbrado", factura.FechaTimbrado ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@SelloCFD", factura.SelloCFD ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@SelloSAT", factura.SelloSAT ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@NoCertificadoSAT", factura.NoCertificadoSAT ?? (object)DBNull.Value);
                        cmdFactura.Parameters.AddWithValue("@CadenaOriginalSAT", factura.CadenaOriginalSAT ?? (object)DBNull.Value);
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

        #region Timbrado con FiscalAPI

        /// <summary>
        /// Genera y timbra una factura usando FiscalAPI (sin SDK)
        /// Compatible con .NET Framework 4.6
        /// </summary>
        public async Task<RespuestaTimbrado> GenerarYTimbrarFactura(
            Guid ventaID,
            string rfcReceptor,
            string nombreReceptor,
            string codigoPostalReceptor,
            string regimenFiscalReceptor,
            string usoCFDI,
            string formaPago,
            string metodoPago,
            string serie,
            string usuario)
        {
            var respuesta = new RespuestaTimbrado
            {
                Exitoso = false,
                FechaTimbrado = DateTime.Now
            };

            try
            {
                // 1. Detectar PAC activo (FiscalAPI o Prodigia)
                var configFiscalAPI = ObtenerConfiguracionFiscalAPI();
                if (configFiscalAPI != null && configFiscalAPI.Activo)
                {
                    // Usar FiscalAPI
                    System.Diagnostics.Debug.WriteLine("✅ Usando FiscalAPI en ambiente: " + configFiscalAPI.Ambiente);
                    return await TimbrarConFiscalAPI(
                        ventaID, rfcReceptor, nombreReceptor, codigoPostalReceptor,
                        regimenFiscalReceptor, usoCFDI, formaPago, metodoPago, serie, usuario);
                }

                // Fallback a Prodigia
                var configuracion = ObtenerConfiguracionProdigia();
                if (configuracion == null)
                {
                    respuesta.Mensaje = "No hay configuración activa de PAC (FiscalAPI o Prodigia)";
                    respuesta.CodigoError = "CONFIG_NOT_FOUND";
                    return respuesta;
                }

                // 2. Obtener venta y detalles
                var venta = CD_Venta.Instancia.ObtenerDetalle(ventaID);
                if (venta == null)
                {
                    respuesta.Mensaje = $"Venta no encontrada: {ventaID}";
                    respuesta.CodigoError = "VENTA_NOT_FOUND";
                    return respuesta;
                }

                // 3. Construir factura
                var factura = new Factura
                {
                    FacturaID = Guid.NewGuid(),
                    VentaID = ventaID,
                    Serie = serie,
                    Folio = ObtenerSiguienteFolio(serie).ToString(),
                    FechaEmision = DateTime.Now,
                    Version = "4.0",
                    TipoComprobante = "I",
                    
                    // Emisor
                    RFCEmisor = configuracion.RfcEmisor,
                    NombreEmisor = configuracion.NombreEmisor,
                    RegimenFiscalEmisor = configuracion.RegimenFiscal,
                    CodigoPostalEmisor = configuracion.CodigoPostal,
                    
                    // Receptor
                    ReceptorRFC = rfcReceptor,
                    ReceptorNombre = nombreReceptor,
                    ReceptorDomicilioFiscalCP = codigoPostalReceptor,
                    ReceptorRegimenFiscalReceptor = regimenFiscalReceptor,
                    ReceptorUsoCFDI = usoCFDI,
                    
                    // Pago
                    FormaPago = formaPago,
                    MetodoPago = metodoPago ?? "PUE",
                    
                    // Totales - calcular desde detalles ya que VentaCliente solo tiene Total
                    Subtotal = venta.Total / 1.16m, // Aproximación - idealmente calcular desde detalles
                    TotalImpuestosTrasladados = venta.Total - (venta.Total / 1.16m),
                    Total = venta.Total,
                    
                    Estatus = "PENDIENTE",
                    UsuarioCreacion = usuario,
                    FechaCreacion = DateTime.Now
                };

                // 4. Convertir detalles de venta a conceptos de factura
                factura.Conceptos = venta.Detalle.Select(d => {
                    var importe = d.Cantidad * d.PrecioVenta;
                    var subtotal = importe / 1.16m; // Asumiendo IVA 16%
                    var iva = importe - subtotal;
                    
                    return new FacturaDetalle
                    {
                        NoIdentificacion = d.CodigoInterno,
                        Descripcion = d.Producto,
                        Cantidad = d.Cantidad,
                        ValorUnitario = d.PrecioVenta / 1.16m, // Precio sin IVA
                        Importe = subtotal, // Importe sin IVA
                        ClaveProdServ = "01010101", // Código genérico SAT
                        ClaveUnidad = "H87", // Pieza
                        Unidad = "Pieza",
                        ObjetoImp = "02", // Sí objeto de impuesto
                        Impuestos = new List<FacturaImpuesto>
                        {
                            new FacturaImpuesto
                            {
                                TipoImpuesto = "TRASLADO",
                                Impuesto = "002", // IVA
                                TipoFactor = "Tasa",
                                TasaOCuota = 0.16m,
                                Base = subtotal,
                                Importe = iva
                            }
                        }
                    };
                }).ToList();

                // 5. Generar XML CFDI 4.0 completo
                var generadorXml = new CFDI40XMLGenerator(configuracion);
                string xmlCfdi = generadorXml.GenerarXML(factura);

                // 6. Timbrar con Prodigia
                using (var prodigiaService = new ProdigiaService(configuracion))
                {
                    respuesta = prodigiaService.CrearYTimbrarCFDI(xmlCfdi);
                }

                // 8. Si el timbrado fue exitoso, guardar en BD
                if (respuesta.Exitoso)
                {
                    factura.UUID = respuesta.UUID;
                    factura.FechaTimbrado = respuesta.FechaTimbrado;
                    factura.XMLTimbrado = respuesta.XMLTimbrado;
                    factura.SelloCFD = respuesta.SelloCFD;
                    factura.SelloSAT = respuesta.SelloSAT;
                    factura.NoCertificadoSAT = respuesta.NoCertificadoSAT;
                    factura.CadenaOriginalSAT = respuesta.CadenaOriginal;
                    factura.Estatus = "TIMBRADA";

                    // Guardar factura en BD
                    bool guardado = GuardarFactura(factura, out string mensajeGuardado);
                    
                    if (!guardado)
                    {
                        respuesta.Mensaje += $" | ADVERTENCIA: XML timbrado correctamente pero no se pudo guardar en BD: {mensajeGuardado}";
                    }
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error al generar factura: {ex.Message}";
                respuesta.CodigoError = "EXCEPTION";
                respuesta.ErrorTecnico = ex.ToString();
                return respuesta;
            }
        }

        /// <summary>
        /// Cancelar CFDI timbrado
        /// </summary>
        public async Task<RespuestaCancelacionCFDI> CancelarCFDI(
            string uuid,
            string motivo,
            string uuidSustitucion,
            string usuario)
        {
            var respuesta = new RespuestaCancelacionCFDI
            {
                Exitoso = false,
                UUID = uuid
            };

            try
            {
                // Validar 72 horas
                string mensaje;
                var factura = ObtenerPorUUID(uuid, out mensaje);
                if (factura == null)
                {
                    respuesta.Mensaje = $"Factura no encontrada con UUID: {uuid}";
                    return respuesta;
                }

                TimeSpan tiempoTranscurrido = DateTime.Now - factura.FechaTimbrado.Value;
                if (tiempoTranscurrido.TotalHours > 72)
                {
                    respuesta.Mensaje = "No se puede cancelar: Han transcurrido más de 72 horas desde el timbrado";
                    respuesta.CodigoError = "TIME_EXCEEDED";
                    return respuesta;
                }

                // Obtener configuración
                var configuracion = ObtenerConfiguracionFiscalAPI();
                if (configuracion == null)
                {
                    respuesta.Mensaje = "Configuración de FiscalAPI no encontrada";
                    return respuesta;
                }

                // Cancelar con FiscalAPI
                using (var fiscalService = new FiscalAPIService(configuracion))
                {
                    respuesta = await fiscalService.CancelarCFDI(uuid, motivo, uuidSustitucion);
                }

                // Actualizar estado en BD si fue exitoso
                if (respuesta.Exitoso)
                {
                    ActualizarEstatusCancelacion(uuid, respuesta.EstatusCancelacion, 
                        respuesta.FechaCancelacion, motivo, usuario);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al cancelar CFDI: {ex.Message}";
                respuesta.ErrorTecnico = ex.ToString();
                return respuesta;
            }
        }

        /// <summary>
        /// Obtiene la configuración de FiscalAPI desde la base de datos
        /// </summary>
        private ConfiguracionProdigia ObtenerConfiguracionProdigia()
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT TOP 1 ConfiguracionID, Usuario, Password, Contrato, Ambiente, 
                               RfcEmisor, NombreEmisor, RegimenFiscal, 
                               CertificadoBase64, LlavePrivadaBase64, PasswordLlave,
                               CodigoPostal, Activo
                        FROM ConfiguracionProdigia
                        WHERE Activo = 1
                        ORDER BY FechaCreacion DESC";

                    SqlCommand cmd = new SqlCommand(query, cnx);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ConfiguracionProdigia
                            {
                                ConfiguracionID = Convert.ToInt32(dr["ConfiguracionID"]),
                                Usuario = dr["Usuario"].ToString(),
                                Password = dr["Password"].ToString(),
                                Contrato = dr["Contrato"].ToString(),
                                Ambiente = dr["Ambiente"].ToString(),
                                RfcEmisor = dr["RfcEmisor"].ToString(),
                                NombreEmisor = dr["NombreEmisor"].ToString(),
                                RegimenFiscal = dr["RegimenFiscal"].ToString(),
                                CertificadoBase64 = dr["CertificadoBase64"]?.ToString(),
                                LlavePrivadaBase64 = dr["LlavePrivadaBase64"]?.ToString(),
                                PasswordLlave = dr["PasswordLlave"]?.ToString(),
                                CodigoPostal = dr["CodigoPostal"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ConfiguracionFiscalAPI ObtenerConfiguracionFiscalAPI()
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT TOP 1 ConfiguracionID, ApiKey, Tenant, Ambiente, RfcEmisor, NombreEmisor,
                               RegimenFiscal, CertificadoBase64, LlavePrivadaBase64, PasswordLlave,
                               CodigoPostal, Activo
                        FROM ConfiguracionFiscalAPI
                        WHERE Activo = 1
                        ORDER BY FechaCreacion DESC";

                    SqlCommand cmd = new SqlCommand(query, cnx);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ConfiguracionFiscalAPI
                            {
                                ConfiguracionID = Convert.ToInt32(dr["ConfiguracionID"]),
                                ApiKey = dr["ApiKey"].ToString(),
                                Tenant = dr["Tenant"].ToString(),
                                Ambiente = dr["Ambiente"].ToString(),
                                RfcEmisor = dr["RfcEmisor"].ToString(),
                                NombreEmisor = dr["NombreEmisor"].ToString(),
                                RegimenFiscal = dr["RegimenFiscal"].ToString(),
                                CertificadoBase64 = dr["CertificadoBase64"]?.ToString(),
                                LlavePrivadaBase64 = dr["LlavePrivadaBase64"]?.ToString(),
                                PasswordLlave = dr["PasswordLlave"]?.ToString(),
                                CodigoPostal = dr["CodigoPostal"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el siguiente número de folio para una serie
        /// </summary>
        private int ObtenerSiguienteFolio(string serie)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        SELECT ISNULL(MAX(CAST(Folio AS INT)), 0) + 1
                        FROM Facturas
                        WHERE Serie = @Serie";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@Serie", serie ?? "A");

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Actualiza el estado de cancelación de una factura
        /// </summary>
        private void ActualizarEstatusCancelacion(string uuid, string estatusCancelacion, 
            DateTime? fechaCancelacion, string motivo, string usuario)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    cnx.Open();

                    string query = @"
                        UPDATE Facturas
                        SET Estatus = @EstatusCancelacion,
                            FechaCancelacion = @FechaCancelacion,
                            MotivoCancelacion = @Motivo,
                            UsuarioModificacion = @Usuario,
                            FechaModificacion = GETDATE()
                        WHERE UUID = @UUID";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@UUID", uuid);
                    cmd.Parameters.AddWithValue("@EstatusCancelacion", estatusCancelacion ?? "CANCELADO");
                    cmd.Parameters.AddWithValue("@FechaCancelacion", (object)fechaCancelacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Motivo", motivo ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Log error silencioso
            }
        }

        /// <summary>
        /// Timbrar factura usando FiscalAPI
        /// </summary>
        private async Task<RespuestaTimbrado> TimbrarConFiscalAPI(
            Guid ventaID,
            string rfcReceptor,
            string nombreReceptor,
            string codigoPostalReceptor,
            string regimenFiscalReceptor,
            string usoCFDI,
            string formaPago,
            string metodoPago,
            string serie,
            string usuario)
        {
            var respuesta = new RespuestaTimbrado
            {
                Exitoso = false,
                FechaTimbrado = DateTime.Now
            };

            try
            {
                // 1. Obtener configuración FiscalAPI
                var config = ObtenerConfiguracionFiscalAPI();
                if (config == null || !config.Activo)
                {
                    respuesta.Mensaje = "FiscalAPI no configurado o inactivo";
                    return respuesta;
                }

                // 2. Obtener venta
                var venta = CD_Venta.Instancia.ObtenerDetalle(ventaID);
                if (venta == null)
                {
                    respuesta.Mensaje = $"Venta no encontrada: {ventaID}";
                    return respuesta;
                }

                // 3. Construir factura
                var factura = new Factura
                {
                    FacturaID = Guid.NewGuid(),
                    VentaID = ventaID,
                    Serie = serie ?? "F",
                    Folio = ObtenerSiguienteFolio(serie).ToString(),
                    FechaEmision = DateTime.Now,
                    Version = "4.0",
                    TipoComprobante = "I",
                    
                    // Emisor
                    EmisorRFC = config.RfcEmisor,
                    EmisorNombre = config.NombreEmisor,
                    EmisorRegimenFiscal = config.RegimenFiscal,
                    CodigoPostalEmisor = config.CodigoPostal,
                    
                    // Receptor
                    ReceptorRFC = rfcReceptor,
                    ReceptorNombre = nombreReceptor,
                    ReceptorDomicilioFiscalCP = codigoPostalReceptor,
                    ReceptorRegimenFiscalReceptor = regimenFiscalReceptor,
                    ReceptorUsoCFDI = usoCFDI,
                    
                    // Pago
                    FormaPago = formaPago,
                    MetodoPago = metodoPago ?? "PUE",
                    
                    // Totales
                    Subtotal = venta.Total / 1.16m,
                    TotalImpuestosTrasladados = venta.Total - (venta.Total / 1.16m),
                    Total = venta.Total,
                    
                    ProveedorPAC = "FiscalAPI",
                    Estatus = "PENDIENTE",
                    UsuarioCreacion = usuario,
                    FechaCreacion = DateTime.Now
                };

                // 4. Convertir detalles a conceptos
                factura.Conceptos = venta.Detalle.Select(d => {
                    var importe = d.Cantidad * d.PrecioVenta;
                    var subtotal = importe / 1.16m;
                    var iva = importe - subtotal;
                    
                    return new FacturaDetalle
                    {
                        NoIdentificacion = d.CodigoInterno,
                        Descripcion = d.Producto,
                        Cantidad = d.Cantidad,
                        ValorUnitario = d.PrecioVenta / 1.16m,
                        Importe = subtotal,
                        ClaveProdServ = "01010101",
                        ClaveUnidad = "H87",
                        Unidad = "Pieza",
                        ObjetoImp = "02",
                        Impuestos = new List<FacturaImpuesto>
                        {
                            new FacturaImpuesto
                            {
                                TipoImpuesto = "TRASLADO",
                                Impuesto = "002",
                                TipoFactor = "Tasa",
                                TasaOCuota = 0.16m,
                                Base = subtotal,
                                Importe = iva
                            }
                        }
                    };
                }).ToList();

                // 5. Generar request para FiscalAPI
                var generador = new FiscalAPICFDI40Generator(config);
                var request = generador.GenerarRequest(factura);

                // 6. Timbrar con FiscalAPI
                var fiscalService = new FiscalAPIService(config);
                var resultado = await fiscalService.CrearYTimbrarCFDI(request);

                if (resultado.Exitoso)
                {
                    // Actualizar factura con datos del timbrado
                    factura.UUID = resultado.UUID;
                    factura.FechaTimbrado = resultado.FechaTimbrado;
                    factura.XMLTimbrado = resultado.XMLTimbrado;
                    factura.SelloCFD = resultado.SelloCFD;
                    factura.SelloSAT = resultado.SelloSAT;
                    factura.NoCertificadoSAT = resultado.NoCertificadoSAT;
                    factura.CadenaOriginalSAT = resultado.CadenaOriginal;
                    factura.Estatus = "TIMBRADA";

                    // Guardar en BD
                    bool guardado = GuardarFactura(factura, out string mensajeGuardado);
                    
                    if (!guardado)
                    {
                        respuesta.Mensaje = $"XML timbrado correctamente pero no se pudo guardar en BD: {mensajeGuardado}";
                    }
                    else
                    {
                        respuesta.Exitoso = true;
                        respuesta.UUID = resultado.UUID;
                        respuesta.XMLTimbrado = resultado.XMLTimbrado;
                        respuesta.Mensaje = "Factura timbrada exitosamente con FiscalAPI";
                    }
                }
                else
                {
                    respuesta.Mensaje = resultado.Mensaje;
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al timbrar con FiscalAPI: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Excepción: {ex.StackTrace}");
                return respuesta;
            }
        }

        #endregion

    }

}
