using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Contabilidad
{
    /// <sumary>
	/// Representa una entidad de Catsatlistanegracontribuyente para su trabajo en memoria
	/// </sumary>
	[Serializable]
    public class Catsatlistanegracontribuyente
    {
        #region Atributos
        public int Numero { get; set; }
        public string Rfc { get; set; }
        public string Nombre { get; set; }
        public string Situacion { get; set; }
        public string Numerofechapresuncionsat { get; set; }
        public DateTime Publicacionsatpresuntos { get; set; }
        public string Numerofechapresunciondof { get; set; }
        public DateTime Publicaciondofpresuntos { get; set; }
        public string NumerofechadesvirtuaronSat { get; set; }
        public DateTime Publicacionsatdesvirtuados { get; set; }
        public string NumerofechadesvirtuaronDof { get; set; }
        public DateTime Publicaciondofdesvirtuados { get; set; }
        public string NumerofechadefinitivosSat { get; set; }
        public DateTime Publicacionsatdefinitivos { get; set; }
        public string NumerofechadefinitivosDof { get; set; }
        public DateTime Publicaciondofdefinitivos { get; set; }
        public string Numerofechasentenciafavorablesat { get; set; }
        public DateTime Publicacionsatsentenciafavorable { get; set; }
        public string Numerofechasentenciafavorabledof { get; set; }
        public DateTime Publicaciondofsentenciafavorable { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public int UltimaAct { get; set; }

        public string SesionId { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catsatlistanegracontribuyente()
        {
        }

        #endregion Constructor

    }
}
