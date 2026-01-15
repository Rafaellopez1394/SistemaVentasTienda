using System;

namespace CapaDatos
{
    public class RespuestaConsulta
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string CodigoError { get; set; }
        public string EstatusSAT { get; set; }
        public string ErrorTecnico { get; set; }
    }
}
