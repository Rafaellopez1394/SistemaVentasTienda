using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Creditosfinancierostasasind para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Creditosfinancierostasasind : Entity.EntidadBase
    {
        #region Atributos
        private string creditosfinancierostasatiieid;
        private int creditofinancieroid;
        private int año;
        private int mes;
        private decimal tasa;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Creditosfinancierostasasind()
        {
            creditosfinancierostasatiieid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Creditosfinancierostasatiieid para Entidad.Contabilidad.Creditosfinancierostasasind
        ///</sumary>
        public string Creditosfinancierostasatiieid
        {
            get
            {
                return this.creditosfinancierostasatiieid;
            }
            set
            {
                this.creditosfinancierostasatiieid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Creditofinancieroid para Entidad.Contabilidad.Creditosfinancierostasasind
        ///</sumary>
        public int Creditofinancieroid
        {
            get
            {
                return this.creditofinancieroid;
            }
            set
            {
                this.creditofinancieroid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Año para Entidad.Contabilidad.Creditosfinancierostasasind
        ///</sumary>
        public int Año
        {
            get
            {
                return this.año;
            }
            set
            {
                this.año = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Mes para Entidad.Contabilidad.Creditosfinancierostasasind
        ///</sumary>
        public int Mes
        {
            get
            {
                return this.mes;
            }
            set
            {
                this.mes = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tasa para Entidad.Contabilidad.Creditosfinancierostasasind
        ///</sumary>
        public decimal Tasa
        {
            get
            {
                return this.tasa;
            }
            set
            {
                this.tasa = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Creditosfinancierostasasind
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Creditosfinancierostasasind
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Creditosfinancierostasasind
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
        /// Determina si las instancias de Entidad.Contabilidad.Creditosfinancierostasasind se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.creditosfinancierostasatiieid == ((Creditosfinancierostasasind)obj).creditosfinancierostasatiieid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Creditosfinancierostasasind
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
