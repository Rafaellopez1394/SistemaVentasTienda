using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de CatProyeccion para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class CatProyeccion : Entity.EntidadBase
    {
        #region Atributos
        private string proyeccionid;
        private int codEmpresa;
        private int ejercicio;
        private string cuenta;
        private int nivel;
        private bool capturado;
        private decimal presupuesto1;
        private decimal presupuesto2;
        private decimal presupuesto3;
        private decimal presupuesto4;
        private decimal presupuesto5;
        private decimal presupuesto6;
        private decimal presupuesto7;
        private decimal presupuesto8;
        private decimal presupuesto9;
        private decimal presupuesto10;
        private decimal presupuesto11;
        private decimal presupuesto12;
        private string observacion1;
        private string observacion2;
        private string observacion3;
        private string observacion4;
        private string observacion5;
        private string observacion6;
        private string observacion7;
        private string observacion8;
        private string observacion9;
        private string observacion10;
        private string observacion11;
        private string observacion12;
        private int estatus;
        private string usuario;
        private DateTime fecha;
        public string Empresaid { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public CatProyeccion()
        {
            proyeccionid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Proyeccionid para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Proyeccionid
        {
            get
            {
                return this.proyeccionid;
            }
            set
            {
                this.proyeccionid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public int CodEmpresa
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
        ///Obtiene o establece el(la) Ejercicio para Entidad.Contabilidad.CatProyeccion
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.CatProyeccion
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
        ///Obtiene o establece el(la) Nivel para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public int Nivel
        {
            get
            {
                return this.nivel;
            }
            set
            {
                this.nivel = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Capturado para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public bool Capturado
        {
            get
            {
                return this.capturado;
            }
            set
            {
                this.capturado = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto1 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto1
        {
            get
            {
                return this.presupuesto1;
            }
            set
            {
                this.presupuesto1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto2 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto2
        {
            get
            {
                return this.presupuesto2;
            }
            set
            {
                this.presupuesto2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto3 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto3
        {
            get
            {
                return this.presupuesto3;
            }
            set
            {
                this.presupuesto3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto4 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto4
        {
            get
            {
                return this.presupuesto4;
            }
            set
            {
                this.presupuesto4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto5 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto5
        {
            get
            {
                return this.presupuesto5;
            }
            set
            {
                this.presupuesto5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto6 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto6
        {
            get
            {
                return this.presupuesto6;
            }
            set
            {
                this.presupuesto6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto7 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto7
        {
            get
            {
                return this.presupuesto7;
            }
            set
            {
                this.presupuesto7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto8 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto8
        {
            get
            {
                return this.presupuesto8;
            }
            set
            {
                this.presupuesto8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto9 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto9
        {
            get
            {
                return this.presupuesto9;
            }
            set
            {
                this.presupuesto9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto10 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto10
        {
            get
            {
                return this.presupuesto10;
            }
            set
            {
                this.presupuesto10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto11 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto11
        {
            get
            {
                return this.presupuesto11;
            }
            set
            {
                this.presupuesto11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Presupuesto12 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public decimal Presupuesto12
        {
            get
            {
                return this.presupuesto12;
            }
            set
            {
                this.presupuesto12 = value;
            }
        }

        ///<sumary>
        ///Obtiene o establece el(la) Observacion1 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion1
        {
            get
            {
                return this.observacion1;
            }
            set
            {
                this.observacion1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion2 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion2
        {
            get
            {
                return this.observacion2;
            }
            set
            {
                this.observacion2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion3 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion3
        {
            get
            {
                return this.observacion3;
            }
            set
            {
                this.observacion3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion4 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion4
        {
            get
            {
                return this.observacion4;
            }
            set
            {
                this.observacion4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion5 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion5
        {
            get
            {
                return this.observacion5;
            }
            set
            {
                this.observacion5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion6 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion6
        {
            get
            {
                return this.observacion6;
            }
            set
            {
                this.observacion6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion7 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion7
        {
            get
            {
                return this.observacion7;
            }
            set
            {
                this.observacion7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion8 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion8
        {
            get
            {
                return this.observacion8;
            }
            set
            {
                this.observacion8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion9 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion9
        {
            get
            {
                return this.observacion9;
            }
            set
            {
                this.observacion9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion10 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion10
        {
            get
            {
                return this.observacion10;
            }
            set
            {
                this.observacion10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion11 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion11
        {
            get
            {
                return this.observacion11;
            }
            set
            {
                this.observacion11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Observacion12 para Entidad.Contabilidad.CatProyeccion
        ///</sumary>
        public string Observacion12
        {
            get
            {
                return this.observacion12;
            }
            set
            {
                this.observacion12 = value;
            }
        }

        ///<sumary>
        ///Obtiene o establece el(la) Estatus para Entidad.Contabilidad.CatProyeccion
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
        ///Obtiene o establece el(la) Usuario para Entidad.Contabilidad.CatProyeccion
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
        ///Obtiene o establece el(la) Fecha para Entidad.Contabilidad.CatProyeccion
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
        /// Determina si las instancias de Entidad.Contabilidad.CatProyeccion se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.proyeccionid == ((CatProyeccion)obj).proyeccionid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.CatProyeccion
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
