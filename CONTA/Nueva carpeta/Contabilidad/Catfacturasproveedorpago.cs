using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catfacturasproveedorpago para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catfacturasproveedorpago : Entity.EntidadBase
    {
        #region Atributos
        private string facturaproveedorpagoid;
        private string facturaproveedorid;
        private string pagoid;
        private decimal importe;
        private DateTime fecha;
        private int estatus;
        private string usuario;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catfacturasproveedorpago()
        {
            facturaproveedorpagoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Facturaproveedorpagoid para Entidad.Contabilidad.Catfacturasproveedorpago
        ///</sumary>
        public string Facturaproveedorpagoid
        {
            get
            {
                return this.facturaproveedorpagoid;
            }
            set
            {
                this.facturaproveedorpagoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Facturaproveedorid para Entidad.Contabilidad.Catfacturasproveedorpago
        ///</sumary>
        public string Facturaproveedorid
        {
            get
            {
                return this.facturaproveedorid;
            }
            set
            {
                this.facturaproveedorid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Pagoid para Entidad.Contabilidad.Catfacturasproveedorpago
        ///</sumary>
        public string Pagoid
        {
            get
            {
                return this.pagoid;
            }
            set
            {
                this.pagoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Catfacturasproveedorpago
        ///</sumary>
        public decimal Importe
        {
            get
            {
                return this.importe;
            }
            set
            {
                this.importe = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catfacturasproveedorpago
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catfacturasproveedorpago
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catfacturasproveedorpago
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
        /// Determina si las instancias de Entidad.Contabilidad.Catfacturasproveedorpago se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.facturaproveedorpagoid == ((Catfacturasproveedorpago)obj).facturaproveedorpagoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catfacturasproveedorpago
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
