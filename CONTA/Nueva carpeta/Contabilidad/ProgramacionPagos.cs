using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Programacionpago para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Programacionpago
    {
        #region Atributos
        public string Programacionpagoid { get; set; }
        public string Empresaid { get; set; }
        public string Proveedorid { get; set; }
        public string Factura { get; set; }
        public DateTime Fechaprogramada { get; set; }
        public DateTime Fechapagada { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public string Solicitanteid { get; set; }
        public string Polizaid { get; set; }
        public string Cuenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int Estatus { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Programacionpago()
        {
            Programacionpagoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

    }
}
