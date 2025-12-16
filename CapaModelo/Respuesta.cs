using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    /// <summary>
    /// Respuesta genérica con un objeto de tipo T
    /// </summary>
    public class Respuesta<T>
    {
        public bool estado { get; set; }
        public string valor { get; set; }
        public T objeto { get; set; }
    }

    /// <summary>
    /// Respuesta básica sin objeto
    /// </summary>
    public class Respuesta
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
        public object Datos { get; set; }
        public object Tag { get; set; }
    }
}
