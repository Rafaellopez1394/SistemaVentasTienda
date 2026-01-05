using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class Gasto
    {
        public Guid GastoID { get; set; }
        public int? CajaID { get; set; }
        public int CategoriaGastoID { get; set; }
        public string Concepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaGasto { get; set; }
        public string NumeroFactura { get; set; }
        public string Proveedor { get; set; }
        public int? FormaPagoID { get; set; }
        
        // Control de aprobación
        public bool RequiereAprobacion { get; set; }
        public bool EstaAprobado { get; set; }
        public string AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        
        // Auditoría
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Observaciones { get; set; }
        
        // Estado
        public bool Activo { get; set; }
        public bool Cancelado { get; set; }
        public string MotivoCancelacion { get; set; }
        
        // Propiedades extendidas (desde vista)
        public string Categoria { get; set; }
        public string CategoriaDescripcion { get; set; }
        public string FormaPago { get; set; }
        public string Estado { get; set; }
    }

    public class CategoriaGasto
    {
        public int CategoriaGastoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereAprobacion { get; set; }
        public decimal? MontoMaximo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ResumenGastos
    {
        public string Categoria { get; set; }
        public int TotalGastos { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal PromedioGasto { get; set; }
        public decimal GastoMinimo { get; set; }
        public decimal GastoMaximo { get; set; }
    }

    public class CierreCajaConGastos
    {
        public int CajaID { get; set; }
        public DateTime Fecha { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal VentasEfectivo { get; set; }
        public decimal VentasTarjeta { get; set; }
        public decimal VentasTransferencia { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal GastosEfectivo { get; set; }
        public decimal TotalRetiros { get; set; }
        public decimal EfectivoEnCaja { get; set; }
        public decimal GananciaNeta { get; set; }
        
        public List<GastoDetalleCierre> DetalleGastos { get; set; }
    }

    public class GastoDetalleCierre
    {
        public string Categoria { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaGasto { get; set; }
        public string UsuarioRegistro { get; set; }
    }

    public class ConcentradoGastoCierre
    {
        public string Categoria { get; set; }
        public int NumeroGastos { get; set; }
        public decimal TotalMonto { get; set; }
    }
}
