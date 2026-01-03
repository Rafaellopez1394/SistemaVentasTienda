using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Mvtobco para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Mvtobco : Entity.EntidadBase
    {
        #region Atributos
        private string mvtobcoid;
        private string empresaid;
        private string acvgralid;
        private DateTime fecPol;
        private string tipPol;
        private string numPol;
        private int codBco;
        private string codMvto;
        private decimal importe;
        private string cuenta;
        private string concepto;
        private string beneficiario;
        private int codProv;
        private string factura;
        private string estConc;
        private DateTime fecConc;
        private string tipMov;
        private string codFlujo;
        private DateTime fecEnt;
        private bool principal;
        private DateTime fecBco;
        private DateTime fecha;
        private string usuario;
        private int estatus;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Mvtobco()
        {
            mvtobcoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Mvtobcoid para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public string Mvtobcoid
        {
            get
            {
                return this.mvtobcoid;
            }
            set
            {
                this.mvtobcoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Acvgralid para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) FecPol para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) NumPol para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) CodBco para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public int CodBco
        {
            get
            {
                return this.codBco;
            }
            set
            {
                this.codBco = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodMvto para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public string CodMvto
        {
            get
            {
                return this.codMvto;
            }
            set
            {
                this.codMvto = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Beneficiario para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public string Beneficiario
        {
            get
            {
                return this.beneficiario;
            }
            set
            {
                this.beneficiario = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodProv para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public int CodProv
        {
            get
            {
                return this.codProv;
            }
            set
            {
                this.codProv = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Factura para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public string Factura
        {
            get
            {
                return this.factura;
            }
            set
            {
                this.factura = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) EstConc para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public string EstConc
        {
            get
            {
                return this.estConc;
            }
            set
            {
                this.estConc = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FecConc para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public DateTime FecConc
        {
            get
            {
                return this.fecConc;
            }
            set
            {
                this.fecConc = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipMov para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) CodFlujo para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) FecEnt para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public DateTime FecEnt
        {
            get
            {
                return this.fecEnt;
            }
            set
            {
                this.fecEnt = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Principal para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public bool Principal
        {
            get
            {
                return this.principal;
            }
            set
            {
                this.principal = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) FecBco para Entidad.Contabilidad.Mvtobco
        ///</sumary>
        public DateTime FecBco
        {
            get
            {
                return this.fecBco;
            }
            set
            {
                this.fecBco = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Mvtobco
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Mvtobco
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
        /// Determina si las instancias de Entidad.Contabilidad.Mvtobco se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.mvtobcoid == ((Mvtobco)obj).mvtobcoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Mvtobco
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
