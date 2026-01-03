using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Polizasdetalle para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Polizasdetalle : Entity.EntidadBase
    {
        #region Atributos
        private string polizadetalleid;
        private string polizaid;
        private string cuentaid;
        private string tipMov;
        private string concepto;
        private decimal cantidad;
        private decimal importe;
        private int estatus;
        private DateTime fecha;
        private string usuario;
        private string presupuestodetalleId;
        private string inventariocostoid;

        //La propiedad Referencia no pertenece a la tabla, solo se agrega para poder mandar grabar el folio de la cesión para la cuestión del redondeo en la tabla acvmov, actualmente graba el folio de la poliza 
        public string Referencia { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Polizasdetalle()
        {
            polizadetalleid = System.Guid.Empty.ToString();
            Referencia = string.Empty;
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Polizadetalleid para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string Polizadetalleid
        {
            get
            {
                return this.polizadetalleid;
            }
            set
            {
                this.polizadetalleid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Polizaid para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string Polizaid
        {
            get
            {
                return this.polizaid;
            }
            set
            {
                this.polizaid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cuentaid para Entidad.Contabilidad.Polizasdetalle
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
        ///Obtiene o establece el(la) TipMov para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string TipMov
        {
            get
            {
                return this.tipMov;
            }
            set
            {
                this.tipMov = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string Concepto
        {
            get
            {
                return this.concepto;
            }
            set
            {
                this.concepto = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cantidad para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public decimal Cantidad
        {
            get
            {
                return this.cantidad;
            }
            set
            {
                this.cantidad = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Polizasdetalle
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Polizasdetalle
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Polizasdetalle
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Polizasdetalle
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
        ///Obtiene o establece el(la) PresupuestodetalleId para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string PresupuestodetalleId
        {
            get
            {
                return this.presupuestodetalleId;
            }
            set
            {
                this.presupuestodetalleId = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Inventariocostoid para Entidad.Contabilidad.Polizasdetalle
        ///</sumary>
        public string Inventariocostoid
        {
            get
            {
                return this.inventariocostoid;
            }
            set
            {
                this.inventariocostoid = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Polizasdetalle se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.polizadetalleid == ((Polizasdetalle)obj).polizadetalleid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Polizasdetalle
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
