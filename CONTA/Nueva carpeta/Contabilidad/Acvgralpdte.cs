using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Acvgralpdte para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Acvgralpdte : Entity.EntidadBase
    {
        #region Atributos
        private string acvgralid;
        private string empresaId;
        private string referenciaId;
        private string codEmpresa;
        private string anomes;
        private string tipPol;
        private int tipoMov;
        private string numPol;
        private DateTime fecPol;
        private string concepto;
        private decimal importe;
        private string usuario;
        private int estatus;
        private DateTime fecha;
        private ListaDeEntidades<Entity.Contabilidad.Acvpdte> listaAcvpdte;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Acvgralpdte()
        {
            acvgralid = System.Guid.Empty.ToString();
            this.listaAcvpdte = new ListaDeEntidades<Acvpdte>();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Acvgralid para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) ReferenciaId para Entidad.Contabilidad.Acvgralpdte
        ///</sumary>
        public string ReferenciaId
        {
            get
            {
                return this.referenciaId;
            }
            set
            {
                this.referenciaId = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Anomes para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) TipoMov para Entidad.Contabilidad.Acvgralpdte
        ///</sumary>
        public int TipoMov
        {
            get
            {
                return this.tipoMov;
            }
            set
            {
                this.tipoMov = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) NumPol para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) FecPol para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Concepto para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Acvgralpdte
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
        ///Obtiene o establece el(la) listaAcvmov para Entidad.Contabilidad.Acvgralpdte
        ///</sumary>
        public ListaDeEntidades<Entity.Contabilidad.Acvpdte> ListaAcvpdte
        {
            get
            {
                return this.listaAcvpdte;
            }
            set
            {
                if (value != this.listaAcvpdte)
                {
                    this.listaAcvpdte = value;
                }
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Acvgralpdte se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.acvgralid == ((Acvgralpdte)obj).acvgralid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Acvgralpdte
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
