using System;
using System.Collections.Generic;
namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Catfacturasproveedor para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Catfacturasproveedor : Entity.EntidadBase
    {
        #region Atributos
        private string facturaproveedorid;
        private string compraid;
        private string empresaid;
        private string proveedorid;
        private string factura;
        private decimal subtotal;
        private decimal iva;
        private decimal retIva;
        private decimal retIsr;
        private decimal ieps;
        private decimal total;
        private string uuid;
        private string fechatimbre;
        private string xml;
        private string rutaxml;
        private bool generapasivo;
        private bool dlls;
        private decimal abonado;
        private DateTime fechapago;
        private bool pagada;
        private DateTime fecha;
        private int estatus;
        private string usuario;
        public List<Catfacturasproveedordet> Detalle { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Catfacturasproveedor()
        {
            facturaproveedorid = System.Guid.Empty.ToString();
            Detalle = new List<Catfacturasproveedordet>();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Facturaproveedorid para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public string Facturaproveedorid
        {
            get
            {
                return this.facturaproveedorid;
            }
            set
            {
                this.facturaproveedorid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Compraid para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Empresaid para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Proveedorid para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Factura para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Subtotal para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal Subtotal
        {
            get
            {
                return this.subtotal;
            }
            set
            {
                this.subtotal = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Iva para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) RetIva para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal RetIva
        {
            get
            {
                return this.retIva;
            }
            set
            {
                this.retIva = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) RetIsr para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal RetIsr
        {
            get
            {
                return this.retIsr;
            }
            set
            {
                this.retIsr = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Ieps para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal Ieps
        {
            get
            {
                return this.ieps;
            }
            set
            {
                this.ieps = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Total para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal Total
        {
            get
            {
                return this.total;
            }
            set
            {
                this.total = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Uuid para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public string Uuid
        {
            get
            {
                return this.uuid;
            }
            set
            {
                this.uuid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fechatimbre para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public string Fechatimbre
        {
            get
            {
                return this.fechatimbre;
            }
            set
            {
                this.fechatimbre = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Xml para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public string Xml
        {
            get
            {
                return this.xml;
            }
            set
            {
                this.xml = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Rutaxml para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public string Rutaxml
        {
            get
            {
                return this.rutaxml;
            }
            set
            {
                this.rutaxml = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Generapasivo para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public bool Generapasivo
        {
            get
            {
                return this.generapasivo;
            }
            set
            {
                this.generapasivo = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Dlls para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public bool Dlls
        {
            get
            {
                return this.dlls;
            }
            set
            {
                this.dlls = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abonado para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public decimal Abonado
        {
            get
            {
                return this.abonado;
            }
            set
            {
                this.abonado = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fechapago para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public DateTime Fechapago
        {
            get
            {
                return this.fechapago;
            }
            set
            {
                this.fechapago = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Pagada para Entidad.Contabilidad.Catfacturasproveedor
        ///</sumary>
        public bool Pagada
        {
            get
            {
                return this.pagada;
            }
            set
            {
                this.pagada = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.Catfacturasproveedor
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.Catfacturasproveedor
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
        /// Determina si las instancias de Entidad.Contabilidad.Catfacturasproveedor se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.facturaproveedorid == ((Catfacturasproveedor)obj).facturaproveedorid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Catfacturasproveedor
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
