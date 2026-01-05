using System;
using System.Text;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Simulador de PAC para pruebas y desarrollo
    /// NO genera facturas reales, solo simula el proceso de timbrado
    /// </summary>
    public class SimuladorPAC : IProveedorPAC
    {
        public string NombreProveedor => "Simulador";

        /// <summary>
        /// Simula el timbrado de un comprobante
        /// </summary>
        public async Task<RespuestaTimbrado> TimbrarAsync(string xmlSinTimbrar, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaTimbrado { Exitoso = false };

            try
            {
                // Simular pequeña demora como si fuera una llamada real
                await Task.Delay(500);

                // Generar UUID simulado (formato válido pero no real)
                string uuid = Guid.NewGuid().ToString().ToUpper();

                // Generar fecha de timbrado
                DateTime fechaTimbrado = DateTime.Now;

                // Generar sello SAT simulado (Base64 de 344 caracteres)
                string selloSAT = GenerarSelloSimulado();

                // Generar sello CFD simulado
                string selloCFD = GenerarSelloSimulado();

                // Número de certificado SAT simulado
                string noCertificadoSAT = "30001000000400002495";

                // Cadena original simulada
                string cadenaOriginal = $"||1.1|{uuid}|{fechaTimbrado:yyyy-MM-ddTHH:mm:ss}|{selloCFD}|{noCertificadoSAT}||";

                // Agregar complemento de timbre al XML original
                string xmlTimbrado = AgregarComplementoTimbre(xmlSinTimbrar, uuid, fechaTimbrado, selloSAT, selloCFD, noCertificadoSAT);

                // Respuesta exitosa
                respuesta.Exitoso = true;
                respuesta.UUID = uuid;
                respuesta.FechaTimbrado = fechaTimbrado;
                respuesta.SelloSAT = selloSAT;
                respuesta.SelloCFD = selloCFD;
                respuesta.NoCertificadoSAT = noCertificadoSAT;
                respuesta.CadenaOriginal = cadenaOriginal;
                respuesta.XMLTimbrado = xmlTimbrado;
                respuesta.Mensaje = "✅ SIMULACIÓN: Comprobante timbrado exitosamente (NO VÁLIDO ANTE SAT)";
                respuesta.CodigoError = null;

                // Log para desarrollo
                System.Diagnostics.Debug.WriteLine($"=== SIMULADOR PAC ===");
                System.Diagnostics.Debug.WriteLine($"UUID Generado: {uuid}");
                System.Diagnostics.Debug.WriteLine($"Fecha Timbrado: {fechaTimbrado}");
                System.Diagnostics.Debug.WriteLine($"=== FIN SIMULACIÓN ===");
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error en simulador: {ex.Message}";
                respuesta.CodigoError = "SIM_ERROR";
            }

            return respuesta;
        }

        /// <summary>
        /// Simula la cancelación de un comprobante
        /// </summary>
        public async Task<RespuestaCancelacion> CancelarAsync(string uuid, string rfcEmisor, string motivoCancelacion, 
            string folioSustitucion, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaCancelacion { Exitoso = false };

            try
            {
                // Simular demora
                await Task.Delay(300);

                respuesta.Exitoso = true;
                respuesta.EstatusSAT = "ACEPTADA";
                respuesta.EstatusUUID = "Cancelado";
                respuesta.Mensaje = "✅ SIMULACIÓN: Comprobante cancelado exitosamente (NO VÁLIDO ANTE SAT)";
                respuesta.FechaRespuesta = DateTime.Now;
                respuesta.AcuseCancelacion = "<Acuse>Simulado</Acuse>";
                respuesta.CodigoError = null;

                System.Diagnostics.Debug.WriteLine($"=== SIMULADOR PAC - CANCELACIÓN ===");
                System.Diagnostics.Debug.WriteLine($"Estatus: {respuesta.EstatusSAT}");
                System.Diagnostics.Debug.WriteLine($"=== FIN CANCELACIÓN ===");
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error en simulador de cancelación: {ex.Message}";
                respuesta.CodigoError = "SIM_ERROR";
            }

            return respuesta;
        }

        /// <summary>
        /// Simula la consulta del estatus de un comprobante
        /// </summary>
        public async Task<RespuestaConsulta> ConsultarEstatusAsync(string uuid, string rfcEmisor, string rfcReceptor, 
            decimal total, ConfiguracionPAC config)
        {
            var respuesta = new RespuestaConsulta { Exitoso = false };

            try
            {
                // Simular demora
                await Task.Delay(200);

                respuesta.Exitoso = true;
                respuesta.Estatus = "Vigente";
                respuesta.Mensaje = "✅ SIMULACIÓN: Comprobante vigente (NO VÁLIDO ANTE SAT)";
                respuesta.EsCancelable = true;
                respuesta.EstatusCancelacion = "Cancelable sin aceptación";
                respuesta.CodigoError = null;

                System.Diagnostics.Debug.WriteLine($"=== SIMULADOR PAC - CONSULTA ===");
                System.Diagnostics.Debug.WriteLine($"UUID Consultado: {uuid}");
                System.Diagnostics.Debug.WriteLine($"Estatus: Vigente");
                System.Diagnostics.Debug.WriteLine($"=== FIN CONSULTA ===");
            }
            catch (Exception ex)
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = $"Error en simulador de consulta: {ex.Message}";
                respuesta.CodigoError = "SIM_ERROR";
            }

            return respuesta;
        }

        /// <summary>
        /// Genera un sello digital simulado en Base64
        /// </summary>
        private string GenerarSelloSimulado()
        {
            // Generar un string aleatorio de ~256 caracteres en Base64
            var random = new Random();
            var buffer = new byte[192]; // 192 bytes = ~256 caracteres en Base64
            random.NextBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Agrega el complemento de timbre fiscal al XML original
        /// </summary>
        private string AgregarComplementoTimbre(string xmlOriginal, string uuid, DateTime fechaTimbrado, 
                                               string selloSAT, string selloCFD, string noCertificadoSAT)
        {
            // Buscar la etiqueta de cierre del Comprobante
            int posicionCierre = xmlOriginal.LastIndexOf("</cfdi:Comprobante>");
            
            if (posicionCierre == -1)
            {
                // Si no encuentra el cierre, devolver el original
                return xmlOriginal;
            }

            // Crear el complemento de timbre
            string complementoTimbre = $@"
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital""
      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
      xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd""
      Version=""1.1""
      UUID=""{uuid}""
      FechaTimbrado=""{fechaTimbrado:yyyy-MM-ddTHH:mm:ss}""
      RfcProvCertif=""SIM000000XXX""
      SelloCFD=""{selloCFD}""
      NoCertificadoSAT=""{noCertificadoSAT}""
      SelloSAT=""{selloSAT}""/>
    <!-- NOTA: Este es un timbre SIMULADO, NO ES VÁLIDO ante el SAT -->
  </cfdi:Complemento>";

            // Insertar el complemento antes del cierre
            string xmlConTimbre = xmlOriginal.Insert(posicionCierre, complementoTimbre);

            return xmlConTimbre;
        }

        public async Task<RespuestaTimbrado> TimbrarComplementoPagoAsync(string xmlSinTimbrar, ConfiguracionPAC config)
        {
            // Para el simulador, reutilizamos la misma lógica de timbrado
            return await TimbrarAsync(xmlSinTimbrar, config);
        }
    }
}
