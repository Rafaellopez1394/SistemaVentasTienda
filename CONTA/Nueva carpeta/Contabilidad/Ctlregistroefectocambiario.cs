using System;

namespace Entity.Contabilidad
{

	/// <sumary>
	/// Representa una entidad de Ctlregistroefectocambiario para su trabajo en memoria
	/// </sumary>
	[Serializable]
	public class Ctlregistroefectocambiario {
		#region Atributos
		public int Id {get;set;}
		public int UltimaAct {get;set;}
		public int Mes {get;set;}

		public string CuentaContable { get;set;}
		public string Descripcion {get;set;}
		public string NatCta {get;set;}
		public string Naturaleza {get;set;}
		public string Nombremes {get;set;}

		public decimal Saldoant {get;set;}
		public decimal Saldofinal {get;set;}
		public decimal Saldocomplementaria {get;set;}
		public decimal Saldoinicial {get;set;}
		public decimal Tipocambiomesant {get;set;}
		public decimal Tipocambiofinmes {get;set;}
		public decimal Totalsaldoant {get;set;}
		public decimal Totalsaldofinal {get;set;}
		public decimal Utilidadcambiaria {get;set;}
		public decimal Perdidacambiaria {get;set;}
        public decimal EfectoCambiario { get; set; }

        public decimal UtilidadCambiariaAcumulada { get; set; }
        public decimal PerdidaCambiariaAcumulada { get; set; }
        public decimal EfectoCambiarioAcumulado { get; set; }

        public decimal DiferenciaTipoCambio { get; set; }

        public DateTime Fecha {get;set;}
		#endregion //Atributos

		#region Constructor

		///<sumary>
		///Constructor
		///</sumary>
		public Ctlregistroefectocambiario(){
		}

		#endregion Constructor

	}
}