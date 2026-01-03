using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catcuentascliente para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catcuentascliente : Entity.EntidadBase
    {
        #region Atributos
        private string cuentaclienteid;
        private string empresaid;
        private string cuentamayor;
        private string cuenta;
        private string descripcion;
        private string descripcioningles;
        private int nivel;
        private bool afecta;
        private bool sistema;
        private bool ietu;
        private bool isr;
        private string flujoCar;
        private string flujoAbo;
        private int estatus;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catcuentascliente()
        {
            cuentaclienteid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cuentaclienteid para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string Cuentaclienteid
        {
            get
            {
                return this.cuentaclienteid;
            }
            set
            {
                this.cuentaclienteid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catcuentascliente
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
        ///Obtiene o establece el(la) Cuentamayor para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string Cuentamayor
        {
            get
            {
                return this.cuentamayor;
            }
            set
            {
                this.cuentamayor = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string Cuenta
        {
            get
            {
                return this.cuenta;
            }
            set
            {
                this.cuenta = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Descripcion para Entidad.Contabilidad.Catcuentascliente
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
        ///Obtiene o establece el(la) Descripcioningles para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string Descripcioningles
        {
            get
            {
                return this.descripcioningles;
            }
            set
            {
                this.descripcioningles = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Nivel para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public int Nivel
        {
            get
            {
                return this.nivel;
            }
            set
            {
                this.nivel = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Afecta para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public bool Afecta
        {
            get
            {
                return this.afecta;
            }
            set
            {
                this.afecta = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Sistema para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public bool Sistema
        {
            get
            {
                return this.sistema;
            }
            set
            {
                this.sistema = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Ietu para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public bool Ietu
        {
            get
            {
                return this.ietu;
            }
            set
            {
                this.ietu = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Isr para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public bool Isr
        {
            get
            {
                return this.isr;
            }
            set
            {
                this.isr = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FlujoCar para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string FlujoCar
        {
            get
            {
                return this.flujoCar;
            }
            set
            {
                this.flujoCar = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FlujoAbo para Entidad.Contabilidad.Catcuentascliente
        ///</sumary>
        public string FlujoAbo
        {
            get
            {
                return this.flujoAbo;
            }
            set
            {
                this.flujoAbo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catcuentascliente
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catcuentascliente
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

        public string CtaSat { get; set; }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Catcuentascliente se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cuentaclienteid == ((Catcuentascliente)obj).cuentaclienteid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catcuentascliente
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
