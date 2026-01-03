using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Polizasnomina para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Polizasnomina
    {
        #region Atributos
        public string Polizanominaid { get; set; }
        public string Empresaid { get; set; }
        public string Polizaid { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Uuid { get; set; }
        public string Rfcemisor { get; set; }
        public string Nombreemisor { get; set; }
        public string Rfcreceptor { get; set; }
        public string Nombrereceptor { get; set; }
        public decimal Sueldo { get; set; }
        public decimal Premiopuntualidad { get; set; }
        public decimal Premioasistencia { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal Primavacacional { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Gastosmedicosmayores { get; set; }
        public decimal Segurodevida { get; set; }
        public decimal Indemnizacion { get; set; }
        public decimal Primadeantiguedad { get; set; }
        public decimal Ptu { get; set; }
        public decimal Subsidioalempleo { get; set; }
        public decimal Isrretenido { get; set; }
        public decimal Imss { get; set; }
        public decimal Infonavit { get; set; }
        public decimal Fonacot { get; set; }
        public string Nominaxml { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public int UltimaAct { get; set; }
        public string NombreArchivo { get; set; }
        public decimal Primaspagadaspatron { get; set; }
        public decimal Isrart174 { get; set; }
        public decimal Prestamoinfonavitcf { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Polizasnomina()
        {
            Polizanominaid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

    }
}
