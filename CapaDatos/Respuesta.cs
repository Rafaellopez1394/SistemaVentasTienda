using System;

namespace CapaDatos
{
    public class Respuesta<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public T Datos { get; set; }
    }
}
