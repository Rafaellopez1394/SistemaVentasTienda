using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catcetestiie para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catcetestiie : Entity.EntidadBase
    {
        #region Atributos
        private int cetetiieid;
        private int año;
        private int mes;
        private decimal cetes;
        private decimal tiie;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catcetestiie()
        {
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Cetetiieid para Entidad.Contabilidad.Catcetestiie
        ///</sumary>
        public int Cetetiieid
        {
            get
            {
                return this.cetetiieid;
            }
            set
            {
                this.cetetiieid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Año para Entidad.Contabilidad.Catcetestiie
        ///</sumary>
        public int Año
        {
            get
            {
                return this.año;
            }
            set
            {
                this.año = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Mes para Entidad.Contabilidad.Catcetestiie
        ///</sumary>
        public int Mes
        {
            get
            {
                return this.mes;
            }
            set
            {
                this.mes = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cetes para Entidad.Contabilidad.Catcetestiie
        ///</sumary>
        public decimal Cetes
        {
            get
            {
                return this.cetes;
            }
            set
            {
                this.cetes = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tiie para Entidad.Contabilidad.Catcetestiie
        ///</sumary>
        public decimal Tiie
        {
            get
            {
                return this.tiie;
            }
            set
            {
                this.tiie = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catcetestiie
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catcetestiie
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catcetestiie
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
        /// Determina si las instancias de Entidad.Contabilidad.Catcetestiie se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.cetetiieid == ((Catcetestiie)obj).cetetiieid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catcetestiie
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
