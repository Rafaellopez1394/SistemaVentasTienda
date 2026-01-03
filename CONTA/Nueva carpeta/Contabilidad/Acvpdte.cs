using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Acvpdte para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Acvpdte : Entity.EntidadBase
    {
        #region Atributos
        private string acvmovid;
        private string empresaId;
        private string codEmpresa;
        private string acvgralid;
        private string anomes;
        private DateTime fecPol;
        private string tipPol;
        private string numPol;
        private int numRenglon;
        private string tipMov;
        private string cuenta;
        private string concepto;
        private string refer;
        private string claseConta;
        private decimal importe;
        private decimal tasaIva;
        private decimal iva;
        private decimal retencionIva;
        private bool pendiente;
        private string codFlujo;
        private string codProveedor;
        private DateTime fechaFiscal;
        private string ctaaux;
        private string usuario;
        private int estatus;
        private DateTime fecha;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Acvpdte()
        {
            acvmovid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Acvmovid para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string Acvmovid
        {
            get
            {
                return this.acvmovid;
            }
            set
            {
                this.acvmovid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Acvgralid para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string Acvgralid
        {
            get
            {
                return this.acvgralid;
            }
            set
            {
                this.acvgralid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Anomes para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) FecPol para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) NumPol para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) NumRenglon para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public int NumRenglon
        {
            get
            {
                return this.numRenglon;
            }
            set
            {
                this.numRenglon = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipMov para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Refer para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string Refer
        {
            get
            {
                return this.refer;
            }
            set
            {
                this.refer = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) ClaseConta para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string ClaseConta
        {
            get
            {
                return this.claseConta;
            }
            set
            {
                this.claseConta = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) TasaIva para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public decimal TasaIva
        {
            get
            {
                return this.tasaIva;
            }
            set
            {
                this.tasaIva = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Iva para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public decimal Iva
        {
            get
            {
                return this.iva;
            }
            set
            {
                this.iva = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) RetencionIva para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public decimal RetencionIva
        {
            get
            {
                return this.retencionIva;
            }
            set
            {
                this.retencionIva = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Pendiente para Entidad.Contabilidad.Acvpdte
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
        ///<sumary>
        ///Obtiene o establece el(la) CodFlujo para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string CodFlujo
        {
            get
            {
                return this.codFlujo;
            }
            set
            {
                this.codFlujo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodProveedor para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string CodProveedor
        {
            get
            {
                return this.codProveedor;
            }
            set
            {
                this.codProveedor = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FechaFiscal para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public DateTime FechaFiscal
        {
            get
            {
                return this.fechaFiscal;
            }
            set
            {
                this.fechaFiscal = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Ctaaux para Entidad.Contabilidad.Acvpdte
        ///</sumary>
        public string Ctaaux
        {
            get
            {
                return this.ctaaux;
            }
            set
            {
                this.ctaaux = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Acvpdte
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Acvpdte
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
        /// Determina si las instancias de Entidad.Contabilidad.Acvpdte se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.acvmovid == ((Acvpdte)obj).acvmovid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Acvpdte
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
