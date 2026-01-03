using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Acvctam para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Acvctam : Entity.EntidadBase
    {
        #region Atributos
        private string acvctamid;
        private string empresaId;
        private string codEmpresa;
        private string cuenta;
        private string natCta;
        private string codGpo;
        private string tipoCta;
        private int estatus;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Acvctam()
        {
            acvctamid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Acvctamid para Entidad.Contabilidad.Acvctam
        ///</sumary>
        public string Acvctamid
        {
            get
            {
                return this.acvctamid;
            }
            set
            {
                this.acvctamid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Acvctam
        ///</sumary>
        public string EmpresaId
        {
            get
            {
                return this.empresaId;
            }
            set
            {
                this.empresaId = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Acvctam
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Acvctam
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
        ///Obtiene o establece el(la) NatCta para Entidad.Contabilidad.Acvctam
        ///</sumary>
        public string NatCta
        {
            get
            {
                return this.natCta;
            }
            set
            {
                this.natCta = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodGpo para Entidad.Contabilidad.Acvctam
        ///</sumary>
        public string CodGpo
        {
            get
            {
                return this.codGpo;
            }
            set
            {
                this.codGpo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipoCta para Entidad.Contabilidad.Acvctam
        ///</sumary>
        public string TipoCta
        {
            get
            {
                return this.tipoCta;
            }
            set
            {
                this.tipoCta = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Acvctam
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Acvctam
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
        /// Determina si las instancias de Entidad.Contabilidad.Acvctam se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.acvctamid == ((Acvctam)obj).acvctamid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Acvctam
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
