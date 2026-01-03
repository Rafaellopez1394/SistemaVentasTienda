// CapaModelo/Proveedor.cs
using System;

namespace CapaModelo
{
    public class Proveedor
    {
        public Guid ProveedorID { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string RegimenFiscalID { get; set; }
        public string RegimenFiscal { get; set; }
        public string CodigoPostal { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoCorreo { get; set; }
        public string ContactoTelefono { get; set; }
        public int BancoID { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string CLABE { get; set; }
        public string TitularCuenta { get; set; }
        public int TipoProveedorID { get; set; }
        public string TipoProveedor { get; set; }
        public int? DiasCredito { get; set; }
        public string Condiciones { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Usuario { get; set; }
        public DateTime UltimaAct { get; set; }
    }
}

