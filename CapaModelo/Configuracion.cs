using System;

namespace CapaModelo
{
    public class ConfiguracionImpresora
    {
        public int ConfigID { get; set; }
        public string TipoImpresion { get; set; } // TICKET, FACTURA, REPORTE
        public string NombreImpresora { get; set; }
        public int AnchoPapel { get; set; } // 58mm, 80mm para tickets
        public bool Activo { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaModificacion { get; set; }
    }

    public class ConfiguracionGeneral
    {
        public int ConfigID { get; set; }
        public string NombreNegocio { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string LogoPath { get; set; }
        public string MensajeTicket { get; set; } // Mensaje al pie del ticket
        public bool ImprimirTicketAutomatico { get; set; }
        public bool MostrarLogoEnTicket { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaModificacion { get; set; }
    }

    public class DatosNegocio
    {
        public string NombreNegocio { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string MensajeTicket { get; set; }
        public bool ImprimirTicketAutomatico { get; set; }
    }
}
