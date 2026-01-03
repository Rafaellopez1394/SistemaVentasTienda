using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catcuentaspersonal para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catcuentaspersonal : Entity.EntidadBase
    {
        #region Atributos
        private string cuentapersonalid;
        private string empresaid;
        private string cuentaid;
        private int codEmpresa;
        private string cuenta;
        private int nivel;
        private int estatus;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catcuentaspersonal()
        {
            cuentapersonalid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cuentapersonalid para Entidad.Contabilidad.Catcuentaspersonal
        ///</sumary>
        public string Cuentapersonalid
        {
            get
            {
                return this.cuentapersonalid;
            }
            set
            {
                this.cuentapersonalid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catcuentaspersonal
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
        ///Obtiene o establece el(la) Cuentaid para Entidad.Contabilidad.Catcuentaspersonal
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
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Catcuentaspersonal
        ///</sumary>
        public int CodEmpresa
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Catcuentaspersonal
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
        ///Obtiene o establece el(la) Nivel para Entidad.Contabilidad.Catcuentaspersonal
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catcuentaspersonal
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catcuentaspersonal
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
        /// Determina si las instancias de Entidad.Contabilidad.Catcuentaspersonal se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cuentapersonalid == ((Catcuentaspersonal)obj).cuentapersonalid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catcuentaspersonal
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
