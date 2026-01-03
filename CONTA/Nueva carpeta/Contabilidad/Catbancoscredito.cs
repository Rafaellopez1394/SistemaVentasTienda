using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catbancoscredito para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catbancoscredito : Entity.EntidadBase
    {
        #region Atributos
        private int bancocreditoid;
        public string Empresaid { get; set; }
        private string banco;
        private string cuentacapital;
        private string cuentaintereses;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catbancoscredito()
        {
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Bancocreditoid para Entidad.Contabilidad.Catbancoscredito
        ///</sumary>
        public int Bancocreditoid
        {
            get
            {
                return this.bancocreditoid;
            }
            set
            {
                this.bancocreditoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Banco para Entidad.Contabilidad.Catbancoscredito
        ///</sumary>
        public string Banco
        {
            get
            {
                return this.banco;
            }
            set
            {
                this.banco = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cuentacapital para Entidad.Contabilidad.Catbancoscredito
        ///</sumary>
        public string Cuentacapital
        {
            get
            {
                return this.cuentacapital;
            }
            set
            {
                this.cuentacapital = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cuentaintereses para Entidad.Contabilidad.Catbancoscredito
        ///</sumary>
        public string Cuentaintereses
        {
            get
            {
                return this.cuentaintereses;
            }
            set
            {
                this.cuentaintereses = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catbancoscredito
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catbancoscredito
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
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catbancoscredito
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
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Catbancoscredito se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.bancocreditoid == ((Catbancoscredito)obj).bancocreditoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catbancoscredito
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
