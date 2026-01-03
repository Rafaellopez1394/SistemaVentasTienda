using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catproveedor para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catproveedor : Entity.EntidadBase
    {
        #region Atributos
        private string proveedorid;
        private int codigo;
        private string nombre;
        private string empresaid;
        private string cuentacontable;
        private string rfc;
        private string paisid;
        private string estadoid;
        private string municipioid;
        private string ciudadid;
        private string coloniaid;
        private string calle;
        private string noexterior;
        private string nointerior;
        private string codigopostal;
        private DateTime fecha;
        private int estatus;
        private string usuario;
        private string tipooperacion;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catproveedor()
        {
            proveedorid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Proveedorid para Entidad.Contabilidad.Catproveedor
        ///</sumary>
        public string Proveedorid
        {
            get
            {
                return this.proveedorid;
            }
            set
            {
                this.proveedorid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Codigo para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Nombre para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Cuentacontable para Entidad.Contabilidad.Catproveedor
        ///</sumary>
        public string Cuentacontable
        {
            get
            {
                return this.cuentacontable;
            }
            set
            {
                this.cuentacontable = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Rfc para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Paisid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Estadoid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Municipioid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Ciudadid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Coloniaid para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Calle para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Noexterior para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Nointerior para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Codigopostal para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catproveedor
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catproveedor
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

        public string Tipooperacion
        {
            get
            {
                return this.tipooperacion;
            }
            set
            {
                this.tipooperacion = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Catproveedor se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.proveedorid == ((Catproveedor)obj).proveedorid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catproveedor
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
