using System;

namespace CapaModelo
{
    /// <summary>
    /// Mapea tasas de IVA a cuentas contables para débitos y créditos
    /// </summary>
    public class MapeoIVA
    {
        public int MapeoIVAID { get; set; }
        public decimal TasaIVA { get; set; }  // 0, 8, 16, u otro
        public bool Exento { get; set; }       // true si es exento
        public int CuentaDeudora { get; set; } // Cuenta para débitos (ventas/compras)
        public int CuentaAcreedora { get; set; } // Cuenta para créditos (IVA cobrado/pagado)
        public string Descripcion { get; set; } // Ej: "IVA 16%", "Exento"
        public bool Activo { get; set; } = true;
        public DateTime FechaAlta { get; set; } = DateTime.Now;
    }
}
