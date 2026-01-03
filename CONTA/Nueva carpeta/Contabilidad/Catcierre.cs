using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catcierre para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catcierre : Entity.EntidadBase
    {
        #region Atributos
        private string cierreid;
        private string empresaid;
        private string anomes;
        private DateTime fechacierre;
        private string tipPol;
        private string numPol;
        private decimal intereses;
        private int estatus;
        private string usuario;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catcierre()
        {
            cierreid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cierreid para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public string Cierreid
        {
            get
            {
                return this.cierreid;
            }
            set
            {
                this.cierreid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catcierre
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
        ///Obtiene o establece el(la) Anomes para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public string Anomes
        {
            get
            {
                return this.anomes;
            }
            set
            {
                this.anomes = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fechacierre para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public DateTime Fechacierre
        {
            get
            {
                return this.fechacierre;
            }
            set
            {
                this.fechacierre = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public string TipPol
        {
            get
            {
                return this.tipPol;
            }
            set
            {
                this.tipPol = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) NumPol para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public string NumPol
        {
            get
            {
                return this.numPol;
            }
            set
            {
                this.numPol = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Intereses para Entidad.Contabilidad.Catcierre
        ///</sumary>
        public decimal Intereses
        {
            get
            {
                return this.intereses;
            }
            set
            {
                this.intereses = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catcierre
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catcierre
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catcierre
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
        /// Determina si las instancias de Entidad.Contabilidad.Catcierre se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cierreid == ((Catcierre)obj).cierreid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catcierre
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
