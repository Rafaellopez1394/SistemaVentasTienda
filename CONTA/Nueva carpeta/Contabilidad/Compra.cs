using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Compra para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Compra : Entity.EntidadBase
    {
        #region Atributos
        private string compraid;
        private string empresaid;
        private string proveedorid;
        private int folio;
        private decimal importe;
        private string fecha;
        private int estatus;
        private string usuario;

        public string Nombreproveedor { get; set; } //Propiedad extendida
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Compra()
        {
            compraid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Compraid para Entidad.Contabilidad.Compra
        ///</sumary>
        public string Compraid
        {
            get
            {
                return this.compraid;
            }
            set
            {
                this.compraid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Compra
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
        ///Obtiene o establece el(la) Proveedorid para Entidad.Contabilidad.Compra
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
        ///Obtiene o establece el(la) Folio para Entidad.Contabilidad.Compra
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
        ///Obtiene o establece el(la) Importe para Entidad.Contabilidad.Compra
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Compra
        ///</sumary>
        public string Fecha
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Compra
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Compra
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
        /// Determina si las instancias de Entidad.Contabilidad.Compra se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.compraid == ((Compra)obj).compraid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Compra
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
