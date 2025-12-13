// CapaModelo/Catalogos.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    public class RegimenFiscal { public string RegimenFiscalID { get; set; } public string Descripcion { get; set; } }
    public class UsoCFDI { public string UsoCFDIID { get; set; } public string Descripcion { get; set; } }
    public class TipoProveedor { public int TipoProveedorID { get; set; } public string Descripcion { get; set; } }
    public class Banco { public int BancoID { get; set; } public string Nombre { get; set; } }
    // TipoCredito definido en TipoCredito.cs (archivo dedicado)
    public class CategoriaProducto { public int CategoriaID { get; set; } public string Nombre { get; set; } public string Descripcion { get; set; } }
    public class ClaveProdServSAT { public string ClaveProdServSATID { get; set; } public string Descripcion { get; set; } }
    public class UnidadSAT { public string ClaveUnidadSAT { get; set; } public string Descripcion { get; set; } }
    public class TasaIVA
    {
        public int TasaIVAID { get; set; }
        public string Clave { get; set; }
        public decimal? Porcentaje { get; set; }
        public string Descripcion { get; set; }

        // Propiedad calculada para mostrar en el dropdown (solo descripción, ya incluye el porcentaje)
        public string TextoCombo => Descripcion;
    }

    public class TasaIEPS
    {
        public int TasaIEPSID { get; set; }
        public string Clave { get; set; }
        public decimal? Porcentaje { get; set; }
        public string Descripcion { get; set; }

        // Propiedad calculada para mostrar en el dropdown
        public string TextoCombo => $"{Descripcion} ({Porcentaje?.ToString("0.##") ?? "0"}%)";
    }
}