using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Acvtip para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Acvtip : Entity.EntidadBase
    {
        #region Atributos
        private string acvtipid;
        private string empresaId;
        private string codEmpresa;
        private int ejercicio;
        private string tipPol;
        private string descripcion;
        private decimal folio1;
        private decimal folio2;
        private decimal folio3;
        private decimal folio4;
        private decimal folio5;
        private decimal folio6;
        private decimal folio7;
        private decimal folio8;
        private decimal folio9;
        private decimal folio10;
        private decimal folio11;
        private decimal folio12;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Acvtip()
        {
            acvtipid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Acvtipid para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public string Acvtipid
        {
            get
            {
                return this.acvtipid;
            }
            set
            {
                this.acvtipid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Acvtip
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
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Acvtip
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
        ///Obtiene o establece el(la) Ejercicio para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public int Ejercicio
        {
            get
            {
                return this.ejercicio;
            }
            set
            {
                this.ejercicio = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) TipPol para Entidad.Contabilidad.Acvtip
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
        ///Obtiene o establece el(la) Descripcion para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public string Descripcion
        {
            get
            {
                return this.descripcion;
            }
            set
            {
                this.descripcion = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio1 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio1
        {
            get
            {
                return this.folio1;
            }
            set
            {
                this.folio1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio2 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio2
        {
            get
            {
                return this.folio2;
            }
            set
            {
                this.folio2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio3 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio3
        {
            get
            {
                return this.folio3;
            }
            set
            {
                this.folio3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio4 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio4
        {
            get
            {
                return this.folio4;
            }
            set
            {
                this.folio4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio5 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio5
        {
            get
            {
                return this.folio5;
            }
            set
            {
                this.folio5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio6 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio6
        {
            get
            {
                return this.folio6;
            }
            set
            {
                this.folio6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio7 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio7
        {
            get
            {
                return this.folio7;
            }
            set
            {
                this.folio7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio8 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio8
        {
            get
            {
                return this.folio8;
            }
            set
            {
                this.folio8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio9 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio9
        {
            get
            {
                return this.folio9;
            }
            set
            {
                this.folio9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio10 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio10
        {
            get
            {
                return this.folio10;
            }
            set
            {
                this.folio10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio11 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio11
        {
            get
            {
                return this.folio11;
            }
            set
            {
                this.folio11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Folio12 para Entidad.Contabilidad.Acvtip
        ///</sumary>
        public decimal Folio12
        {
            get
            {
                return this.folio12;
            }
            set
            {
                this.folio12 = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Acvtip se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.acvtipid == ((Acvtip)obj).acvtipid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Acvtip
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            //descriptivo de la entidad: this.Descripcion;
            return this.descripcion;
        }
        #endregion //Métodos SobreCargados
    }
}
