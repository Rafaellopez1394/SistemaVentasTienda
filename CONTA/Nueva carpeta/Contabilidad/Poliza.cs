using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Poliza para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Poliza : Entity.EntidadBase
    {
        #region Atributos
        private string polizaid;
        private string empresaId;
        private string folio;
        private string tipPol;
        private DateTime fechapol;
        private string concepto;
        private decimal importe;
        private int estatus;
        private DateTime fecha;
        private string usuario;
        private ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listaPolizaDetalle;
        private bool pendiente;
        public bool Tienefacturas { get; set; }
        public int TieneComplemento { get; set; }
        public string Fec_pol { get; set; }
        public bool Pagoprogramado { get; set; }
        public int TieneComplementoDeNomina { get; set; }
        #endregion //Atributos



        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Poliza()
        {
            polizaid = System.Guid.Empty.ToString();
            this.listaPolizaDetalle = new ListaDeEntidades<Polizasdetalle>();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Polizaid para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Folio para Entidad.Contabilidad.Poliza
        ///</sumary>
        public string Folio
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
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Fechapol para Entidad.Contabilidad.Poliza
        ///</sumary>
        public DateTime Fechapol
        {
            get
            {
                return this.fechapol;
            }
            set
            {
                this.fechapol = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Poliza
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
        ///Obtiene o establece el(la) listaPolizaDetalle para Entidad.Contabilidad.Poliza
        ///</sumary>
        public ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> ListaPolizaDetalle
        {
            get
            {
                return this.listaPolizaDetalle;
            }
            set
            {
                if (value != this.listaPolizaDetalle)
                {
                    this.listaPolizaDetalle = value;
                }
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Pendiente para Entidad.Contabilidad.Poliza
        ///</sumary>
        public bool Pendiente
        {
            get
            {
                return this.pendiente;
            }
            set
            {
                this.pendiente = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Poliza se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.polizaid == ((Poliza)obj).polizaid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Poliza
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
