using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Cierrecontabilidad para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Cierrecontabilidad
    {
        #region Atributos
        public int Cierrecontableid { get; set; }
        public string Empresaid { get; set; }
        public DateTime Fechacierre { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Cierrecontabilidad()
        {
        }

        #endregion Constructor

    }
}
