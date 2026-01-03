using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Creditosfinancierosdetalle para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Creditosfinancierosdetalle : Entity.EntidadBase
    {
        #region Atributos
        private string creditofinancierodetalleid;
        private int creditofinancieroid;
        private int tipoMov;
        private string bancoid;
        private string concepto;
        private DateTime fechaApli;
        private decimal financiamiento;
        private decimal interes;
        private decimal iva;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Creditosfinancierosdetalle()
        {
            creditofinancierodetalleid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Creditofinancierodetalleid para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public string Creditofinancierodetalleid
        {
            get
            {
                return this.creditofinancierodetalleid;
            }
            set
            {
                this.creditofinancierodetalleid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Creditofinancieroid para Entidad.Contabilidad.Creditosfinancierosdetalle
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
        ///Obtiene o establece el(la) TipoMov para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public int TipoMov
        {
            get
            {
                return this.tipoMov;
            }
            set
            {
                this.tipoMov = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Bancoid para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public string Bancoid
        {
            get
            {
                return this.bancoid;
            }
            set
            {
                this.bancoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public string Concepto
        {
            get
            {
                return this.concepto;
            }
            set
            {
                this.concepto = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FechaApli para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public DateTime FechaApli
        {
            get
            {
                return this.fechaApli;
            }
            set
            {
                this.fechaApli = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Financiamiento para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public decimal Financiamiento
        {
            get
            {
                return this.financiamiento;
            }
            set
            {
                this.financiamiento = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Interes para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public decimal Interes
        {
            get
            {
                return this.interes;
            }
            set
            {
                this.interes = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Iva para Entidad.Contabilidad.Creditosfinancierosdetalle
        ///</sumary>
        public decimal Iva
        {
            get
            {
                return this.iva;
            }
            set
            {
                this.iva = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Creditosfinancierosdetalle
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Creditosfinancierosdetalle
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Creditosfinancierosdetalle
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
        /// Determina si las instancias de Entidad.Contabilidad.Creditosfinancierosdetalle se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.creditofinancierodetalleid == ((Creditosfinancierosdetalle)obj).creditofinancierodetalleid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Creditosfinancierosdetalle
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
