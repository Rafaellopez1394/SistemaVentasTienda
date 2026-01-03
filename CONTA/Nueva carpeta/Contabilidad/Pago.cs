using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Pago para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Pago : Entity.EntidadBase
    {
        #region Atributos
        private string pagoid;
        private string empresaid;
        private string proveedorid;
        private string bancoid;
        private string poliza;
        private DateTime fecPol;
        private string tipPol;
        private decimal importe;
        private string tipomoneda;
        private decimal tipocambio;
        private DateTime fecha;
        private int estatus;
        private string usuario;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Pago()
        {
            pagoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Pagoid para Entidad.Contabilidad.Pago
        ///</sumary>
        public string Pagoid
        {
            get
            {
                return this.pagoid;
            }
            set
            {
                this.pagoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Proveedorid para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Bancoid para Entidad.Contabilidad.Pago
        ///</sumary>
        public string Bancoid
        {
            get
            {
                return this.bancoid;
            }
            set
            {
                this.bancoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Poliza para Entidad.Contabilidad.Pago
        ///</sumary>
        public string Poliza
        {
            get
            {
                return this.poliza;
            }
            set
            {
                this.poliza = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FecPol para Entidad.Contabilidad.Pago
        ///</sumary>
        public DateTime FecPol
        {
            get
            {
                return this.fecPol;
            }
            set
            {
                this.fecPol = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Tipomoneda para Entidad.Contabilidad.Pago
        ///</sumary>
        public string Tipomoneda
        {
            get
            {
                return this.tipomoneda;
            }
            set
            {
                this.tipomoneda = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Tipocambio para Entidad.Contabilidad.Pago
        ///</sumary>
        public decimal Tipocambio
        {
            get
            {
                return this.tipocambio;
            }
            set
            {
                this.tipocambio = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Pago
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Pago
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
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Pago se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.pagoid == ((Pago)obj).pagoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Pago
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
