using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public int SucursalID { get; set; }
        public Sucursal oSucursal { get; set; }
        public int RolID { get; set; }
        public Rol oRol { get; set; }
        public List<Menu> oListaMenu { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}
