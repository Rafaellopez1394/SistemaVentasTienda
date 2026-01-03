using System;

namespace CapaModelo
{
    /// <summary>
    /// Catálogo de cuentas contables del sistema
    /// </summary>
    public class CatalogoContable
    {
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }      // Ej: "1100", "4000"
        public string NombreCuenta { get; set; }      // Ej: "Clientes", "Ventas"
        public string TipoCuenta { get; set; }        // Ej: "ACTIVO", "PASIVO", "INGRESO", "GASTO", "PATRIMONIO"
        public string SubTipo { get; set; }           // Ej: "CLIENTE", "CAJA", "INVENTARIO", "IVA_COBRADO", "IVA_PAGADO"
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}


