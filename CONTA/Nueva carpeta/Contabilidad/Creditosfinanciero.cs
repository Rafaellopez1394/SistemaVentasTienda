using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Creditosfinanciero para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Creditosfinanciero : Entity.EntidadBase
    {
        #region Atributos
        private int creditofinancieroid;
        private string empresaid;
        private int bancocreditoid;
        private string contrato;
        private string moneda;
        private decimal importe;
        private string calculointeres;
        private DateTime fechainicial;
        private DateTime fechavence;
        private string tipocredito;
        private decimal tasainteres;
        private decimal puntos;
        private decimal diasañotasa;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        public DateTime FechaCorte { get; set; }

        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Creditosfinanciero()
        {
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Creditofinancieroid para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public int Creditofinancieroid
        {
            get
            {
                return this.creditofinancieroid;
            }
            set
            {
                this.creditofinancieroid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Creditosfinanciero
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
        ///Obtiene o establece el(la) Bancocreditoid para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public int Bancocreditoid
        {
            get
            {
                return this.bancocreditoid;
            }
            set
            {
                this.bancocreditoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Contrato para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public string Contrato
        {
            get
            {
                return this.contrato;
            }
            set
            {
                this.contrato = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Moneda para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public string Moneda
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
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Creditosfinanciero
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
        ///Obtiene o establece el(la) Calculointeres para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public string Calculointeres
        {
            get
            {
                return this.calculointeres;
            }
            set
            {
                this.calculointeres = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fechainicial para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public DateTime Fechainicial
        {
            get
            {
                return this.fechainicial;
            }
            set
            {
                this.fechainicial = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fechavence para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public DateTime Fechavence
        {
            get
            {
                return this.fechavence;
            }
            set
            {
                this.fechavence = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tipocredito para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public string Tipocredito
        {
            get
            {
                return this.tipocredito;
            }
            set
            {
                this.tipocredito = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tasainteres para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public decimal Tasainteres
        {
            get
            {
                return this.tasainteres;
            }
            set
            {
                this.tasainteres = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Puntos para Entidad.Contabilidad.Creditosfinanciero
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
        ///Obtiene o establece el(la) Diasañotasa para Entidad.Contabilidad.Creditosfinanciero
        ///</sumary>
        public decimal Diasañotasa
        {
            get
            {
                return this.diasañotasa;
            }
            set
            {
                this.diasañotasa = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Creditosfinanciero
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Creditosfinanciero
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Creditosfinanciero
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
        /// Determina si las instancias de Entidad.Contabilidad.Creditosfinanciero se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.creditofinancieroid == ((Creditosfinanciero)obj).creditofinancieroid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Creditosfinanciero
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
