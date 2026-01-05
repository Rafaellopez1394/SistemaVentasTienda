using System;
using System.Threading.Tasks;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Interfaz para proveedores de servicios PAC (Proveedor Autorizado de Certificación)
    /// Permite cambiar entre Finkok, SW-Sapien, DIVERZA, etc. sin modificar lógica de negocio
    /// </summary>
    public interface IProveedorPAC
    {
        /// <summary>
        /// Timbra un comprobante CFDI 4.0
        /// </summary>
        /// <param name="xmlSinTimbrar">XML del comprobante sin timbrar</param>
        /// <param name="config">Configuración del PAC</param>
        /// <returns>Respuesta con UUID, sello SAT y XML timbrado</returns>
        Task<RespuestaTimbrado> TimbrarAsync(string xmlSinTimbrar, ConfiguracionPAC config);

        /// <summary>
        /// Cancela un comprobante timbrado
        /// </summary>
        /// <param name="uuid">UUID del comprobante a cancelar</param>
        /// <param name="rfcEmisor">RFC del emisor</param>
        /// <param name="motivoCancelacion">Motivo de cancelación (01-04)</param>
        /// <param name="folioSustitucion">Folio del comprobante sustituto (solo para motivo 01)</param>
        /// <param name="config">Configuración del PAC</param>
        /// <returns>Respuesta con estatus de cancelación</returns>
        Task<RespuestaCancelacion> CancelarAsync(string uuid, string rfcEmisor, string motivoCancelacion, 
            string folioSustitucion, ConfiguracionPAC config);

        /// <summary>
        /// Consulta el estatus de un comprobante
        /// </summary>
        /// <param name="uuid">UUID del comprobante</param>
        /// <param name="rfcEmisor">RFC del emisor</param>
        /// <param name="rfcReceptor">RFC del receptor</param>
        /// <param name="total">Monto total del comprobante</param>
        /// <param name="config">Configuración del PAC</param>
        /// <returns>Respuesta con estatus del comprobante</returns>
        Task<RespuestaConsulta> ConsultarEstatusAsync(string uuid, string rfcEmisor, string rfcReceptor, 
            decimal total, ConfiguracionPAC config);

        /// <summary>
        /// Timbra un complemento de pago
        /// </summary>
        /// <param name="xmlSinTimbrar">XML del complemento sin timbrar</param>
        /// <param name="config">Configuración del PAC</param>
        /// <returns>Respuesta con UUID, sello SAT y XML timbrado</returns>
        Task<RespuestaTimbrado> TimbrarComplementoPagoAsync(string xmlSinTimbrar, ConfiguracionPAC config);
    }

    /// <summary>
    /// Respuesta de cancelación
    /// </summary>
    public class RespuestaCancelacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string EstatusSAT { get; set; } // ACEPTADA, RECHAZADA, EN_PROCESO
        public string EstatusUUID { get; set; } // Estado del UUID específico
        public DateTime? FechaRespuesta { get; set; }
        public string AcuseCancelacion { get; set; } // XML del acuse
        public string CodigoError { get; set; }
    }

    /// <summary>
    /// Respuesta de consulta
    /// </summary>
    public class RespuestaConsulta
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string Estatus { get; set; } // VIGENTE, CANCELADA
        public bool EsCancelable { get; set; }
        public string EstatusCancelacion { get; set; }
        public string CodigoError { get; set; }
    }
}
