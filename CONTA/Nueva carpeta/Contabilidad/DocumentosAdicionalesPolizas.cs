using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de DocumentosAdicionalesPolizas para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class DocumentosAdicionalesPolizas
    {
        #region Atributos
        public string Uuid { get; set; }
        public string Polizaid { get; set; }
        public int Poliza { get; set; }
        public string Empresaid { get; set; }
        public string Documento { get; set; }
        public string Nombrearchivo { get; set; }
        public string Extension { get; set; }
        public string Ruta { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public DocumentosAdicionalesPolizas()
        {
            Uuid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

    }
}