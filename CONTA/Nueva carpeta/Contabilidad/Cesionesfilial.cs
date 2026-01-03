using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Cesionesfilial para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Cesionesfilial : Entity.EntidadBase
    {
        #region Atributos
        private string cesionid;
        private string empresaid;
        private string clienteid;
        private int folio;
        private DateTime fechaDocu;
        private DateTime fechaVence;
        private string tipo;
        private decimal linea;
        private decimal disposiciones;
        private decimal abonado;
        private decimal tasaanual;
        private decimal puntos;
        private decimal diasaño;
        private int estatus;
        private string usuario;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Cesionesfilial()
        {
            cesionid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cesionid para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public string Cesionid
        {
            get
            {
                return this.cesionid;
            }
            set
            {
                this.cesionid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Cesionesfilial
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
        ///Obtiene o establece el(la) Clienteid para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public string Clienteid
        {
            get
            {
                return this.clienteid;
            }
            set
            {
                this.clienteid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public int Folio
        {
            get
            {
                return this.folio;
            }
            set
            {
                this.folio = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FechaDocu para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public DateTime FechaDocu
        {
            get
            {
                return this.fechaDocu;
            }
            set
            {
                this.fechaDocu = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FechaVence para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public DateTime FechaVence
        {
            get
            {
                return this.fechaVence;
            }
            set
            {
                this.fechaVence = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tipo para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public string Tipo
        {
            get
            {
                return this.tipo;
            }
            set
            {
                this.tipo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Linea para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Linea
        {
            get
            {
                return this.linea;
            }
            set
            {
                this.linea = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Disposiciones para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Disposiciones
        {
            get
            {
                return this.disposiciones;
            }
            set
            {
                this.disposiciones = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abonado para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Abonado
        {
            get
            {
                return this.abonado;
            }
            set
            {
                this.abonado = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tasaanual para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Tasaanual
        {
            get
            {
                return this.tasaanual;
            }
            set
            {
                this.tasaanual = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Puntos para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Puntos
        {
            get
            {
                return this.puntos;
            }
            set
            {
                this.puntos = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Diasaño para Entidad.Contabilidad.Cesionesfilial
        ///</sumary>
        public decimal Diasaño
        {
            get
            {
                return this.diasaño;
            }
            set
            {
                this.diasaño = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Cesionesfilial
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Cesionesfilial
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Cesionesfilial
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
        /// Determina si las instancias de Entidad.Contabilidad.Cesionesfilial se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cesionid == ((Cesionesfilial)obj).cesionid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Cesionesfilial
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
