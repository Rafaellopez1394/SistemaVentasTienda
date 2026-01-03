using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catsolicitantespago para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catsolicitantespago
    {
        #region Atributos
        public string Solicitanteid { get; set; }
        public string Solicitante { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int Estatus { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catsolicitantespago()
        {
            Solicitanteid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

    }
}
