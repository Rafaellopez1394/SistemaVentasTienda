using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catalogocuentasat para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catalogocuentasat : Entity.EntidadBase
    {
        #region Atributos
        private string ctasat;
        private string descripcion;
        private int estatus;
        private DateTime fecha;
        private string usuario;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catalogocuentasat()
        {
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Ctasat para Entidad.Contabilidad.Catalogocuentasat
        ///</sumary>
        public string Ctasat
        {
            get
            {
                return this.ctasat;
            }
            set
            {
                this.ctasat = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Descripcion para Entidad.Contabilidad.Catalogocuentasat
        ///</sumary>
        public string Descripcion
        {
            get
            {
                return this.descripcion;
            }
            set
            {
                this.descripcion = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catalogocuentasat
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
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catalogocuentasat
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catalogocuentasat
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
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Catalogocuentasat se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.ctasat == ((Catalogocuentasat)obj).ctasat);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catalogocuentasat
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            //descriptivo de la entidad: this.Descripcion;
            return this.descripcion;
        }
        #endregion //Métodos SobreCargados
    }
}
