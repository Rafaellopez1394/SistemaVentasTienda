using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de ConfiguracionRutasAlmacenamiento para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class ConfiguracionRutasAlmacenamiento
    {
        #region Atributos
        public int Configuracionid { get; set; }
        public string Nombreruta { get; set; }
        public string Rutaalmacenamiento { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public ConfiguracionRutasAlmacenamiento()
        {
        }

        #endregion Constructor

    }
}
