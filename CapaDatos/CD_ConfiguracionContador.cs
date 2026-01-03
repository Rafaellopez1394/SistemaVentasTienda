using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using CapaModelo;

namespace CapaDatos
{
    /// <summary>
    /// Capa de datos para Configuración del Contador
    /// </summary>
    public class CD_ConfiguracionContador
    {
        private static CD_ConfiguracionContador _instancia = null;

        public static CD_ConfiguracionContador Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new CD_ConfiguracionContador();
                return _instancia;
            }
        }

        // =====================================================
        // DASHBOARD CONTADOR
        // =====================================================

        public DashboardContador ObtenerDashboard()
        {
            DashboardContador dashboard = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM vw_DashboardContador";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        dashboard = new DashboardContador
                        {
                            FacturasMes = Convert.ToInt32(dr["FacturasMes"]),
                            TotalFacturadoMes = Convert.ToDecimal(dr["TotalFacturadoMes"]),
                            FacturasCanceladasMes = Convert.ToInt32(dr["FacturasCanceladasMes"]),
                            RecibosMes = Convert.ToInt32(dr["RecibosMes"]),
                            TotalNominaMes = Convert.ToDecimal(dr["TotalNominaMes"]),
                            CuentasPendientes = Convert.ToInt32(dr["CuentasPendientes"]),
                            TotalPorPagar = Convert.ToDecimal(dr["TotalPorPagar"]),
                            PolizasMes = Convert.ToInt32(dr["PolizasMes"])
                        };
                    }
                    dr.Close();
                }

                // Generar alertas
                if (dashboard != null)
                {
                    dashboard.Alertas = GenerarAlertas(dashboard);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerDashboard: " + ex.Message);
                dashboard = new DashboardContador();
            }

            return dashboard ?? new DashboardContador();
        }

        private List<AlertaContador> GenerarAlertas(DashboardContador dashboard)
        {
            var alertas = new List<AlertaContador>();

            // Alerta: Cuentas por pagar vencidas
            if (dashboard.CuentasPendientes > 0)
            {
                alertas.Add(new AlertaContador
                {
                    Tipo = dashboard.CuentasPendientes > 10 ? "URGENTE" : "IMPORTANTE",
                    Titulo = "Cuentas por Pagar Pendientes",
                    Mensaje = $"Tienes {dashboard.CuentasPendientes} cuentas pendientes por ${dashboard.TotalPorPagar:N2}",
                    Icono = "fa-exclamation-triangle",
                    Color = "warning",
                    Fecha = DateTime.Now
                });
            }

            // Alerta: Declaraciones mensuales
            DateTime hoy = DateTime.Now;
            if (hoy.Day >= 15 && hoy.Day <= 17)
            {
                alertas.Add(new AlertaContador
                {
                    Tipo = "URGENTE",
                    Titulo = "Declaración Mensual",
                    Mensaje = "Recuerda presentar la declaración mensual antes del día 17",
                    Icono = "fa-calendar-check",
                    Color = "danger",
                    Fecha = DateTime.Now
                });
            }

            // Alerta: Cierre de mes
            if (hoy.Day >= 28)
            {
                alertas.Add(new AlertaContador
                {
                    Tipo = "IMPORTANTE",
                    Titulo = "Cierre de Mes",
                    Mensaje = "Próximo a cierre de mes. Verifica tus pólizas y conciliaciones",
                    Icono = "fa-calendar-times",
                    Color = "info",
                    Fecha = DateTime.Now
                });
            }

            return alertas;
        }

        // =====================================================
        // CONFIGURACIÓN EMPRESA
        // =====================================================

        public ConfiguracionEmpresa ObtenerConfiguracionEmpresa()
        {
            ConfiguracionEmpresa config = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM ConfiguracionEmpresa WHERE ConfigEmpresaID = 1";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        config = new ConfiguracionEmpresa
                        {
                            ConfigEmpresaID = Convert.ToInt32(dr["ConfigEmpresaID"]),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            NombreComercial = dr["NombreComercial"].ToString(),
                            RegimenFiscal = dr["RegimenFiscal"].ToString(),
                            Calle = dr["Calle"].ToString(),
                            NumeroExterior = dr["NumeroExterior"]?.ToString(),
                            NumeroInterior = dr["NumeroInterior"]?.ToString(),
                            Colonia = dr["Colonia"].ToString(),
                            Municipio = dr["Municipio"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            CodigoPostal = dr["CodigoPostal"].ToString(),
                            Pais = dr["Pais"].ToString(),
                            Telefono = dr["Telefono"]?.ToString(),
                            Email = dr["Email"]?.ToString(),
                            SitioWeb = dr["SitioWeb"]?.ToString()
                        };
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerConfiguracionEmpresa: " + ex.Message);
            }

            return config;
        }

        public bool ActualizarConfiguracionEmpresa(ActualizarEmpresaRequest request, string usuario, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        UPDATE ConfiguracionEmpresa SET
                            RFC = @RFC,
                            RazonSocial = @RazonSocial,
                            NombreComercial = @NombreComercial,
                            RegimenFiscal = @RegimenFiscal,
                            Calle = @Calle,
                            NumeroExterior = @NumeroExterior,
                            NumeroInterior = @NumeroInterior,
                            Colonia = @Colonia,
                            Municipio = @Municipio,
                            Estado = @Estado,
                            CodigoPostal = @CodigoPostal,
                            Pais = @Pais,
                            Telefono = @Telefono,
                            Email = @Email,
                            SitioWeb = @SitioWeb,
                            UsuarioModificacion = @Usuario,
                            FechaModificacion = GETDATE()
                        WHERE ConfigEmpresaID = 1";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@RFC", request.RFC);
                    cmd.Parameters.AddWithValue("@RazonSocial", request.RazonSocial);
                    cmd.Parameters.AddWithValue("@NombreComercial", request.NombreComercial ?? string.Empty);
                    cmd.Parameters.AddWithValue("@RegimenFiscal", request.RegimenFiscal);
                    cmd.Parameters.AddWithValue("@Calle", request.Calle);
                    cmd.Parameters.AddWithValue("@NumeroExterior", request.NumeroExterior ?? string.Empty);
                    cmd.Parameters.AddWithValue("@NumeroInterior", request.NumeroInterior ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Colonia", request.Colonia);
                    cmd.Parameters.AddWithValue("@Municipio", request.Municipio);
                    cmd.Parameters.AddWithValue("@Estado", request.Estado);
                    cmd.Parameters.AddWithValue("@CodigoPostal", request.CodigoPostal);
                    cmd.Parameters.AddWithValue("@Pais", request.Pais ?? "México");
                    cmd.Parameters.AddWithValue("@Telefono", request.Telefono ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Email", request.Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@SitioWeb", request.SitioWeb ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Configuración de empresa actualizada correctamente";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // =====================================================
        // CONFIGURACIÓN CONTABLE
        // =====================================================

        public ConfiguracionContable ObtenerConfiguracionContable()
        {
            ConfiguracionContable config = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM ConfiguracionContable WHERE ConfigContableID = 1";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        config = new ConfiguracionContable
                        {
                            ConfigContableID = Convert.ToInt32(dr["ConfigContableID"]),
                            EjercicioFiscalActual = Convert.ToInt32(dr["EjercicioFiscalActual"]),
                            MesActual = Convert.ToInt32(dr["MesActual"]),
                            CuentaBancos = dr["CuentaBancos"]?.ToString(),
                            CuentaClientes = dr["CuentaClientes"]?.ToString(),
                            CuentaProveedores = dr["CuentaProveedores"]?.ToString(),
                            CuentaIVATraslado = dr["CuentaIVATraslado"]?.ToString(),
                            CuentaIVARetenido = dr["CuentaIVARetenido"]?.ToString(),
                            CuentaISRRetenido = dr["CuentaISRRetenido"]?.ToString(),
                            CuentaVentas = dr["CuentaVentas"]?.ToString(),
                            CuentaCompras = dr["CuentaCompras"]?.ToString(),
                            CuentaCostoVentas = dr["CuentaCostoVentas"]?.ToString(),
                            CuentaNomina = dr["CuentaNomina"]?.ToString(),
                            CuentaIMSS = dr["CuentaIMSS"]?.ToString(),
                            UsaPolizasAutomaticas = Convert.ToBoolean(dr["UsaPolizasAutomaticas"]),
                            RequiereAutorizacionCancelacion = Convert.ToBoolean(dr["RequiereAutorizacionCancelacion"]),
                            DiasVencimientoFacturas = Convert.ToInt32(dr["DiasVencimientoFacturas"])
                        };
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerConfiguracionContable: " + ex.Message);
            }

            return config;
        }

        public bool ActualizarConfiguracionContable(ConfiguracionContable config, string usuario, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        UPDATE ConfiguracionContable SET
                            EjercicioFiscalActual = @EjercicioFiscalActual,
                            MesActual = @MesActual,
                            CuentaBancos = @CuentaBancos,
                            CuentaClientes = @CuentaClientes,
                            CuentaProveedores = @CuentaProveedores,
                            CuentaIVATraslado = @CuentaIVATraslado,
                            CuentaIVARetenido = @CuentaIVARetenido,
                            CuentaISRRetenido = @CuentaISRRetenido,
                            CuentaVentas = @CuentaVentas,
                            CuentaCompras = @CuentaCompras,
                            CuentaCostoVentas = @CuentaCostoVentas,
                            CuentaNomina = @CuentaNomina,
                            CuentaIMSS = @CuentaIMSS,
                            UsaPolizasAutomaticas = @UsaPolizasAutomaticas,
                            RequiereAutorizacionCancelacion = @RequiereAutorizacionCancelacion,
                            DiasVencimientoFacturas = @DiasVencimientoFacturas,
                            UsuarioModificacion = @Usuario,
                            FechaModificacion = GETDATE()
                        WHERE ConfigContableID = 1";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@EjercicioFiscalActual", config.EjercicioFiscalActual);
                    cmd.Parameters.AddWithValue("@MesActual", config.MesActual);
                    cmd.Parameters.AddWithValue("@CuentaBancos", config.CuentaBancos ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaClientes", config.CuentaClientes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaProveedores", config.CuentaProveedores ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaIVATraslado", config.CuentaIVATraslado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaIVARetenido", config.CuentaIVARetenido ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaISRRetenido", config.CuentaISRRetenido ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaVentas", config.CuentaVentas ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaCompras", config.CuentaCompras ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaCostoVentas", config.CuentaCostoVentas ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaNomina", config.CuentaNomina ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaIMSS", config.CuentaIMSS ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsaPolizasAutomaticas", config.UsaPolizasAutomaticas);
                    cmd.Parameters.AddWithValue("@RequiereAutorizacionCancelacion", config.RequiereAutorizacionCancelacion);
                    cmd.Parameters.AddWithValue("@DiasVencimientoFacturas", config.DiasVencimientoFacturas);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Configuración contable actualizada correctamente";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // =====================================================
        // CATÁLOGO DE CUENTAS
        // =====================================================

        public List<CuentaContable> ObtenerCatalogoCuentas(bool soloActivas = true)
        {
            List<CuentaContable> cuentas = new List<CuentaContable>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"SELECT CuentaID, CodigoCuenta, NombreCuenta, TipoCuenta, Descripcion, Activo
                                      FROM CatalogoContable";
                    if (soloActivas) query += " WHERE Activo = 1";
                    query += " ORDER BY CodigoCuenta";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cuentas.Add(new CuentaContable
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            Codigo = dr["CodigoCuenta"].ToString(),
                            Nombre = dr["NombreCuenta"].ToString(),
                            // Campos no presentes: valores por defecto
                            Nivel = 0,
                            CuentaPadre = null,
                            Tipo = dr["TipoCuenta"].ToString(),
                            Naturaleza = "",
                            AceptaMovimientos = true,
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            CodigoSAT = null
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerCatalogoCuentas: " + ex.Message);
            }

            return cuentas;
        }

        public bool GuardarCuentaContable(CuentaContable cuenta, string usuario, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query;
                    SqlCommand cmd;

                    if (cuenta.CuentaID == 0) // Insertar
                    {
                        query = @"
                            INSERT INTO CatalogoCuentas (
                                Codigo, Nombre, Nivel, CuentaPadre, Tipo, Naturaleza,
                                AceptaMovimientos, Activo, Descripcion, CodigoSAT,
                                UsuarioCreacion
                            ) VALUES (
                                @Codigo, @Nombre, @Nivel, @CuentaPadre, @Tipo, @Naturaleza,
                                @AceptaMovimientos, @Activo, @Descripcion, @CodigoSAT,
                                @Usuario
                            )";
                    }
                    else // Actualizar
                    {
                        query = @"
                            UPDATE CatalogoCuentas SET
                                Nombre = @Nombre,
                                Descripcion = @Descripcion,
                                CodigoSAT = @CodigoSAT,
                                AceptaMovimientos = @AceptaMovimientos,
                                Activo = @Activo,
                                UsuarioModificacion = @Usuario,
                                FechaModificacion = GETDATE()
                            WHERE CuentaID = @CuentaID";
                    }

                    cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@Codigo", cuenta.Codigo);
                    cmd.Parameters.AddWithValue("@Nombre", cuenta.Nombre);
                    cmd.Parameters.AddWithValue("@Nivel", cuenta.Nivel);
                    cmd.Parameters.AddWithValue("@CuentaPadre", cuenta.CuentaPadre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", cuenta.Tipo);
                    cmd.Parameters.AddWithValue("@Naturaleza", cuenta.Naturaleza);
                    cmd.Parameters.AddWithValue("@AceptaMovimientos", cuenta.AceptaMovimientos);
                    cmd.Parameters.AddWithValue("@Activo", cuenta.Activo);
                    cmd.Parameters.AddWithValue("@Descripcion", cuenta.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoSAT", cuenta.CodigoSAT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    if (cuenta.CuentaID > 0)
                    {
                        cmd.Parameters.AddWithValue("@CuentaID", cuenta.CuentaID);
                    }

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = cuenta.CuentaID == 0 ? "Cuenta creada correctamente" : "Cuenta actualizada correctamente";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // =====================================================
        // CONFIGURACIÓN NÓMINA
        // =====================================================

        public ConfiguracionNomina ObtenerConfiguracionNomina()
        {
            ConfiguracionNomina config = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM ConfiguracionNomina WHERE ConfigNominaID = 1";
                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        config = new ConfiguracionNomina
                        {
                            ConfigNominaID = Convert.ToInt32(dr["ConfigNominaID"]),
                            TipoPeriodo = dr["TipoPeriodo"].ToString(),
                            DiasDePago = Convert.ToInt32(dr["DiasDePago"]),
                            SalarioMinimo = Convert.ToDecimal(dr["SalarioMinimo"]),
                            UMA = Convert.ToDecimal(dr["UMA"]),
                            TopeSalarioIMSS = Convert.ToDecimal(dr["TopeSalarioIMSS"]),
                            PorcentajeIMSSEmpresa = Convert.ToDecimal(dr["PorcentajeIMSSEmpresa"]),
                            PorcentajeRCV = Convert.ToDecimal(dr["PorcentajeRCV"]),
                            PorcentajeGuarderia = Convert.ToDecimal(dr["PorcentajeGuarderia"]),
                            PorcentajeRetiro = Convert.ToDecimal(dr["PorcentajeRetiro"]),
                            PorcentajeIMSSTrabajador = Convert.ToDecimal(dr["PorcentajeIMSSTrabajador"]),
                            LugarExpedicionNomina = dr["LugarExpedicionNomina"]?.ToString(),
                            RutaCertificadoNomina = dr["RutaCertificadoNomina"]?.ToString()
                        };
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerConfiguracionNomina: " + ex.Message);
            }

            return config;
        }

        public bool ActualizarConfiguracionNomina(ConfiguracionNomina config, string usuario, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = @"
                        UPDATE ConfiguracionNomina SET
                            TipoPeriodo = @TipoPeriodo,
                            DiasDePago = @DiasDePago,
                            SalarioMinimo = @SalarioMinimo,
                            UMA = @UMA,
                            TopeSalarioIMSS = @TopeSalarioIMSS,
                            PorcentajeIMSSEmpresa = @PorcentajeIMSSEmpresa,
                            PorcentajeRCV = @PorcentajeRCV,
                            PorcentajeGuarderia = @PorcentajeGuarderia,
                            PorcentajeRetiro = @PorcentajeRetiro,
                            PorcentajeIMSSTrabajador = @PorcentajeIMSSTrabajador,
                            LugarExpedicionNomina = @LugarExpedicionNomina,
                            RutaCertificadoNomina = @RutaCertificadoNomina,
                            UsuarioModificacion = @Usuario,
                            FechaModificacion = GETDATE()
                        WHERE ConfigNominaID = 1";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@TipoPeriodo", config.TipoPeriodo);
                    cmd.Parameters.AddWithValue("@DiasDePago", config.DiasDePago);
                    cmd.Parameters.AddWithValue("@SalarioMinimo", config.SalarioMinimo);
                    cmd.Parameters.AddWithValue("@UMA", config.UMA);
                    cmd.Parameters.AddWithValue("@TopeSalarioIMSS", config.TopeSalarioIMSS);
                    cmd.Parameters.AddWithValue("@PorcentajeIMSSEmpresa", config.PorcentajeIMSSEmpresa);
                    cmd.Parameters.AddWithValue("@PorcentajeRCV", config.PorcentajeRCV);
                    cmd.Parameters.AddWithValue("@PorcentajeGuarderia", config.PorcentajeGuarderia);
                    cmd.Parameters.AddWithValue("@PorcentajeRetiro", config.PorcentajeRetiro);
                    cmd.Parameters.AddWithValue("@PorcentajeIMSSTrabajador", config.PorcentajeIMSSTrabajador);
                    cmd.Parameters.AddWithValue("@LugarExpedicionNomina", config.LugarExpedicionNomina ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RutaCertificadoNomina", config.RutaCertificadoNomina ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cnx.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Configuración de nómina actualizada correctamente";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // =====================================================
        // PERCEPCIONES Y DEDUCCIONES
        // =====================================================

        public List<PercepcionNomina> ObtenerPercepciones(bool soloActivas = true)
        {
            List<PercepcionNomina> percepciones = new List<PercepcionNomina>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM PercepcionesNomina";
                    if (soloActivas) query += " WHERE Activo = 1";
                    query += " ORDER BY ClaveSAT";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        percepciones.Add(new PercepcionNomina
                        {
                            PercepcionID = Convert.ToInt32(dr["PercepcionID"]),
                            ClaveSAT = dr["ClaveSAT"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            TipoPercepcion = dr["TipoPercepcion"].ToString(),
                            GravaISR = Convert.ToBoolean(dr["GravaISR"]),
                            GravaIMSS = Convert.ToBoolean(dr["GravaIMSS"]),
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerPercepciones: " + ex.Message);
            }

            return percepciones;
        }

        public List<DeduccionNomina> ObtenerDeducciones(bool soloActivas = true)
        {
            List<DeduccionNomina> deducciones = new List<DeduccionNomina>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(Conexion.CN))
                {
                    string query = "SELECT * FROM DeduccionesNomina";
                    if (soloActivas) query += " WHERE Activo = 1";
                    query += " ORDER BY ClaveSAT";

                    SqlCommand cmd = new SqlCommand(query, cnx);
                    cnx.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        deducciones.Add(new DeduccionNomina
                        {
                            DeduccionID = Convert.ToInt32(dr["DeduccionID"]),
                            ClaveSAT = dr["ClaveSAT"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            TipoDeduccion = dr["TipoDeduccion"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerDeducciones: " + ex.Message);
            }

            return deducciones;
        }

        /*
        #region Certificados Digitales
        // NOTA: Esta región ha sido comentada porque ahora se usa CD_CertificadoDigital.cs
        // Las propiedades de CertificadoDigital han cambiado y la tabla ahora usa FechaVigenciaInicio/Fin
        // en lugar de FechaInicio/FechaVencimiento, y no tiene TipoCertificado, UsarParaFacturas, etc.
        

        /// <summary>
        /// Guarda un certificado digital (CSD o FIEL)
        /// </summary>
        public Respuesta GuardarCertificado(CertificadoDigital certificado, string usuario)
        {
            var respuesta = new Respuesta { Resultado = false, Mensaje = "" };

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.CN))
                {
                    conexion.Open();
                    // Si es predeterminado, desactivar otros predeterminados
                    if (certificado.EsPredeterminado)
                    {
                        var cmdDesactivar = new SqlCommand(
                            "UPDATE CertificadosDigitales SET EsPredeterminado = 0 WHERE TipoCertificado = @Tipo", 
                            conexion);
                        cmdDesactivar.Parameters.AddWithValue("@Tipo", certificado.TipoCertificado);
                        cmdDesactivar.ExecuteNonQuery();
                    }

                    var sql = @"INSERT INTO CertificadosDigitales 
                        (TipoCertificado, NombreCertificado, NoCertificado, RFC, RazonSocial, 
                         FechaInicio, FechaVencimiento, ArchivoCER, ArchivoKEY, PasswordKEY, 
                         NombreArchivoCER, NombreArchivoKEY, Activo, EsPredeterminado, 
                         UsarParaFacturas, UsarParaNomina, UsarParaCancelaciones, UsuarioCreacion)
                        VALUES 
                        (@TipoCertificado, @NombreCertificado, @NoCertificado, @RFC, @RazonSocial, 
                         @FechaInicio, @FechaVencimiento, @ArchivoCER, @ArchivoKEY, @PasswordKEY, 
                         @NombreArchivoCER, @NombreArchivoKEY, @Activo, @EsPredeterminado, 
                         @UsarParaFacturas, @UsarParaNomina, @UsarParaCancelaciones, @Usuario)";

                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@TipoCertificado", certificado.TipoCertificado);
                    cmd.Parameters.AddWithValue("@NombreCertificado", certificado.NombreCertificado);
                    cmd.Parameters.AddWithValue("@NoCertificado", certificado.NoCertificado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RFC", certificado.RFC);
                    cmd.Parameters.AddWithValue("@RazonSocial", certificado.RazonSocial ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaInicio", certificado.FechaInicio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaVencimiento", certificado.FechaVencimiento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ArchivoCER", certificado.ArchivoCER);
                    cmd.Parameters.AddWithValue("@ArchivoKEY", certificado.ArchivoKEY);
                    cmd.Parameters.AddWithValue("@PasswordKEY", certificado.PasswordKEY); // Debe estar encriptado
                    cmd.Parameters.AddWithValue("@NombreArchivoCER", certificado.NombreArchivoCER ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NombreArchivoKEY", certificado.NombreArchivoKEY ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Activo", certificado.Activo);
                    cmd.Parameters.AddWithValue("@EsPredeterminado", certificado.EsPredeterminado);
                    cmd.Parameters.AddWithValue("@UsarParaFacturas", certificado.UsarParaFacturas);
                    cmd.Parameters.AddWithValue("@UsarParaNomina", certificado.UsarParaNomina);
                    cmd.Parameters.AddWithValue("@UsarParaCancelaciones", certificado.UsarParaCancelaciones);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cmd.ExecuteNonQuery();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Certificado guardado exitosamente";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en GuardarCertificado: " + ex.Message);
                respuesta.Mensaje = "Error al guardar certificado: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Obtiene lista de certificados
        /// </summary>
        public List<CertificadoDigital> ObtenerCertificados(bool? soloActivos = null, string tipoCertificado = null)
        {
            var certificados = new List<CertificadoDigital>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.CN))
                {
                    conexion.Open();
                    var sql = @"SELECT CertificadoID, TipoCertificado, NombreCertificado, NoCertificado, 
                               RFC, RazonSocial, FechaInicio, FechaVencimiento, 
                               NombreArchivoCER, NombreArchivoKEY, Activo, EsPredeterminado, 
                               UsarParaFacturas, UsarParaNomina, UsarParaCancelaciones, 
                               UsuarioCreacion, FechaCreacion, UsuarioModificacion, FechaModificacion
                               FROM CertificadosDigitales WHERE 1=1";

                    if (soloActivos.HasValue && soloActivos.Value)
                        sql += " AND Activo = 1";

                    if (!string.IsNullOrEmpty(tipoCertificado))
                        sql += " AND TipoCertificado = @Tipo";

                    sql += " ORDER BY FechaCreacion DESC";

                    var cmd = new SqlCommand(sql, conexion);
                    if (!string.IsNullOrEmpty(tipoCertificado))
                        cmd.Parameters.AddWithValue("@Tipo", tipoCertificado);

                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        certificados.Add(new CertificadoDigital
                        {
                            CertificadoID = Convert.ToInt32(dr["CertificadoID"]),
                            TipoCertificado = dr["TipoCertificado"].ToString(),
                            NombreCertificado = dr["NombreCertificado"].ToString(),
                            NoCertificado = dr["NoCertificado"]?.ToString(),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"]?.ToString(),
                            FechaInicio = dr["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaInicio"]) : (DateTime?)null,
                            FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVencimiento"]) : (DateTime?)null,
                            NombreArchivoCER = dr["NombreArchivoCER"]?.ToString(),
                            NombreArchivoKEY = dr["NombreArchivoKEY"]?.ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EsPredeterminado = Convert.ToBoolean(dr["EsPredeterminado"]),
                            UsarParaFacturas = Convert.ToBoolean(dr["UsarParaFacturas"]),
                            UsarParaNomina = Convert.ToBoolean(dr["UsarParaNomina"]),
                            UsarParaCancelaciones = Convert.ToBoolean(dr["UsarParaCancelaciones"]),
                            UsuarioCreacion = dr["UsuarioCreacion"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            UsuarioModificacion = dr["UsuarioModificacion"]?.ToString(),
                            FechaModificacion = dr["FechaModificacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaModificacion"]) : (DateTime?)null
                        });
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerCertificados: " + ex.Message);
            }

            return certificados;
        }

        /// <summary>
        /// Obtiene certificado predeterminado por tipo
        /// </summary>
        public CertificadoDigital ObtenerCertificadoPredeterminado(string tipoCertificado)
        {
            CertificadoDigital certificado = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.CN))
                {
                    conexion.Open();
                    var sql = @"SELECT TOP 1 CertificadoID, TipoCertificado, NombreCertificado, 
                               NoCertificado, RFC, RazonSocial, FechaInicio, FechaVencimiento, 
                               ArchivoCER, ArchivoKEY, PasswordKEY, NombreArchivoCER, NombreArchivoKEY, 
                               Activo, EsPredeterminado, UsarParaFacturas, UsarParaNomina, UsarParaCancelaciones
                               FROM CertificadosDigitales 
                               WHERE Activo = 1 AND TipoCertificado = @Tipo AND EsPredeterminado = 1
                               ORDER BY FechaCreacion DESC";

                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@Tipo", tipoCertificado);

                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        certificado = new CertificadoDigital
                        {
                            CertificadoID = Convert.ToInt32(dr["CertificadoID"]),
                            TipoCertificado = dr["TipoCertificado"].ToString(),
                            NombreCertificado = dr["NombreCertificado"].ToString(),
                            NoCertificado = dr["NoCertificado"]?.ToString(),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"]?.ToString(),
                            FechaInicio = dr["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaInicio"]) : (DateTime?)null,
                            FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVencimiento"]) : (DateTime?)null,
                            ArchivoCER = (byte[])dr["ArchivoCER"],
                            ArchivoKEY = (byte[])dr["ArchivoKEY"],
                            PasswordKEY = dr["PasswordKEY"].ToString(),
                            NombreArchivoCER = dr["NombreArchivoCER"]?.ToString(),
                            NombreArchivoKEY = dr["NombreArchivoKEY"]?.ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EsPredeterminado = Convert.ToBoolean(dr["EsPredeterminado"]),
                            UsarParaFacturas = Convert.ToBoolean(dr["UsarParaFacturas"]),
                            UsarParaNomina = Convert.ToBoolean(dr["UsarParaNomina"]),
                            UsarParaCancelaciones = Convert.ToBoolean(dr["UsarParaCancelaciones"])
                        };
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerCertificadoPredeterminado: " + ex.Message);
            }

            return certificado;
        }

        /// <summary>
        /// Activa/Desactiva un certificado y opcionalmente lo marca como predeterminado
        /// </summary>
        public Respuesta ActualizarEstadoCertificado(int certificadoID, bool activo, bool esPredeterminado, string usuario)
        {
            var respuesta = new Respuesta { Resultado = false, Mensaje = "" };

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.CN))
                {
                    conexion.Open();
                    // Si se marca como predeterminado, desactivar otros predeterminados del mismo tipo
                    if (esPredeterminado)
                    {
                        var sqlTipo = "SELECT TipoCertificado FROM CertificadosDigitales WHERE CertificadoID = @ID";
                        var cmdTipo = new SqlCommand(sqlTipo, conexion);
                        cmdTipo.Parameters.AddWithValue("@ID", certificadoID);
                        var tipo = cmdTipo.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(tipo))
                        {
                            var cmdDesactivar = new SqlCommand(
                                "UPDATE CertificadosDigitales SET EsPredeterminado = 0 WHERE TipoCertificado = @Tipo AND CertificadoID <> @ID",
                                conexion);
                            cmdDesactivar.Parameters.AddWithValue("@Tipo", tipo);
                            cmdDesactivar.Parameters.AddWithValue("@ID", certificadoID);
                            cmdDesactivar.ExecuteNonQuery();
                        }
                    }

                    var sql = @"UPDATE CertificadosDigitales 
                               SET Activo = @Activo, 
                                   EsPredeterminado = @EsPredeterminado,
                                   UsuarioModificacion = @Usuario,
                                   FechaModificacion = GETDATE()
                               WHERE CertificadoID = @ID";

                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@ID", certificadoID);
                    cmd.Parameters.AddWithValue("@Activo", activo);
                    cmd.Parameters.AddWithValue("@EsPredeterminado", esPredeterminado);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    cmd.ExecuteNonQuery();

                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Estado del certificado actualizado";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ActualizarEstadoCertificado: " + ex.Message);
                respuesta.Mensaje = "Error al actualizar estado: " + ex.Message;
            }

            return respuesta;
        }

        /// <summary>
        /// Elimina un certificado (lógicamente)
        /// </summary>
        public Respuesta EliminarCertificado(int certificadoID, string usuario)
        {
            return ActualizarEstadoCertificado(certificadoID, false, false, usuario);
        }

        #endregion
        */
    }
}

