using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Cattipocambio para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Cattipocambio : Entity.EntidadBase
    {
        #region Atributos
        private string fechatipocambio;
        private string usuario;
        private string serie;

        private decimal importetipocambio;
        private DateTime fecha;
        private int estatus;
        private int tipo;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Cattipocambio()
        {
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Fechatipocambio para Entidad.Contabilidad.Cattipocambio
        ///</sumary>
        public string Fechatipocambio
        {
            get
            {
                return this.fechatipocambio;
            }
            set
            {
                this.fechatipocambio = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Importetipocambio para Entidad.Contabilidad.Cattipocambio
        ///</sumary>
        public decimal Importetipocambio
        {
            get
            {
                return this.importetipocambio;
            }
            set
            {
                this.importetipocambio = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Cattipocambio
        ///</sumary>
        public DateTime Fecha
        {
            get
            {
                return this.fecha;
            }
            set
            {
                this.fecha = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Cattipocambio
        ///</sumary>
        public string Usuario
        {
            get
            {
                return this.usuario;
            }
            set
            {
                this.usuario = value;
            }
        }

        public string Serie
        {
            get
            {
                return this.serie;
            }
            set
            {
                this.serie = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Cattipocambio
        ///</sumary>
        public int Estatus
        {
            get
            {
                return this.estatus;
            }
            set
            {
                this.estatus = value;
            }
        }
        public int Tipo
        {
            get
            {
                return this.tipo;
            }
            set
            {
                this.tipo = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Cattipocambio se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.fechatipocambio == ((Cattipocambio)obj).fechatipocambio);
        }
        /// <summary>
        /// Sirve como una función hash para un tipo concreto, apropiado para su utilización
        /// en algoritmos de hash y en estructuras de datos como las tablas hash
        /// </summary>
        /// <returns>numero de código hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Obtiene la descripción de Entidad.Contabilidad.Cattipocambio
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            //TODO:falta agregar el campo descriptivo de la entidad: this.Descripcion;
            return base.ToString();
        }
        #endregion //Métodos SobreCargados
    }
}
