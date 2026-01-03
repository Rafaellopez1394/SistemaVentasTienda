using System;


namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catclientesdiverso para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catclientesdiverso : Entity.EntidadBase
    {
        #region Atributos
        private string clientediversoid;
        private string empresaid;
        private int codigo;
        private string nombre;
        private string rfc;
        private string codigoclientecuentacontable;
        private string paisid;
        private string estadoid;
        private string municipioid;
        private string ciudadid;
        private string coloniaid;
        private string calle;
        private string noexterior;
        private string nointerior;
        private string codigopostal;
        private string email;
        private DateTime fecha;
        private int estatus;
        private string usuario;

        public string NumRegIdTrib { get; set; }
        public string Regimenfiscal { get; set; }
        //public string eMail { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catclientesdiverso()
        {
            clientediversoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Clientediversoid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Clientediversoid
        {
            get
            {
                return this.clientediversoid;
            }
            set
            {
                this.clientediversoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Empresaid
        {
            get
            {
                return this.empresaid;
            }
            set
            {
                this.empresaid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Codigo para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public int Codigo
        {
            get
            {
                return this.codigo;
            }
            set
            {
                this.codigo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Nombre para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Nombre
        {
            get
            {
                return this.nombre;
            }
            set
            {
                this.nombre = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Rfc para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Rfc
        {
            get
            {
                return this.rfc;
            }
            set
            {
                this.rfc = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Codigoclientecuentacontable para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Codigoclientecuentacontable
        {
            get
            {
                return this.codigoclientecuentacontable;
            }
            set
            {
                this.codigoclientecuentacontable = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Paisid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Paisid
        {
            get
            {
                return this.paisid;
            }
            set
            {
                this.paisid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estadoid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Estadoid
        {
            get
            {
                return this.estadoid;
            }
            set
            {
                this.estadoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Municipioid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Municipioid
        {
            get
            {
                return this.municipioid;
            }
            set
            {
                this.municipioid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Ciudadid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Ciudadid
        {
            get
            {
                return this.ciudadid;
            }
            set
            {
                this.ciudadid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Coloniaid para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Coloniaid
        {
            get
            {
                return this.coloniaid;
            }
            set
            {
                this.coloniaid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Calle para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Calle
        {
            get
            {
                return this.calle;
            }
            set
            {
                this.calle = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Noexterior para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Noexterior
        {
            get
            {
                return this.noexterior;
            }
            set
            {
                this.noexterior = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Nointerior para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Nointerior
        {
            get
            {
                return this.nointerior;
            }
            set
            {
                this.nointerior = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Codigopostal para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string Codigopostal
        {
            get
            {
                return this.codigopostal;
            }
            set
            {
                this.codigopostal = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) eMail para Entidad.Contabilidad.Catclientesdiverso
        ///</sumary>
        public string eMail
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catclientesdiverso
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catclientesdiverso
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catclientesdiverso
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
        /// Determina si las instancias de Entidad.Contabilidad.Catclientesdiverso se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.clientediversoid == ((Catclientesdiverso)obj).clientediversoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catclientesdiverso
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
