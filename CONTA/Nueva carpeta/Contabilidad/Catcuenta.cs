using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catcuenta para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catcuenta : Entity.EntidadBase
    {
        #region Atributos
        private string cuentaid;
        private string empresaid;
        private string codEmpresa;
        private string cuenta;
        private string descripcion;
        private string descripcioningles;
        private int nivel;
        private bool afecta;
        private bool sistema;
        private bool ietu;
        private bool isr;
        private decimal saldo;
        private string flujoCar;
        private string flujoAbo;
        private int moneda;
        private int estatus;
        private DateTime fecha;
        public string CtaSat { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catcuenta()
        {
            cuentaid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cuentaid para Entidad.Contabilidad.Catcuenta
        ///</sumary>
        public string Cuentaid
        {
            get
            {
                return this.cuentaid;
            }
            set
            {
                this.cuentaid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Catcuenta
        ///</sumary>
        public string CodEmpresa
        {
            get
            {
                return this.codEmpresa;
            }
            set
            {
                this.codEmpresa = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Descripcion para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Descripcioningles para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Nivel para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Afecta para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Sistema para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Ietu para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Isr para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Saldo para Entidad.Contabilidad.Catcuenta
        ///</sumary>
        public decimal Saldo
        {
            get
            {
                return this.saldo;
            }
            set
            {
                this.saldo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FlujoCar para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) FlujoAbo para Entidad.Contabilidad.Catcuenta
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
        public int Moneda
        {
            get
            {
                return this.moneda;
            }
            set
            {
                this.moneda = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catcuenta
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catcuenta
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
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Catcuenta se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cuentaid == ((Catcuenta)obj).cuentaid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catcuenta
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            //descriptivo de la entidad: this.Descripcion;
            return this.descripcion;
        }
        #endregion //Métodos SobreCargados
    }

    public class Respuesta
    {
        public int Estado { get; set; }
        public string Mensaje { get; set; }
    }
}
