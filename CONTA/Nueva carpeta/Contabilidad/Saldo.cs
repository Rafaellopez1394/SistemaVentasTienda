using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Saldo para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Saldo : Entity.EntidadBase
    {
        #region Atributos
        private string saldoid;
        private string empresaId;
        private string codEmpresa;
        private string ejercicio;
        private string cuentaid;
        private string cuenta;
        private int nivel;
        private decimal sdoini;
        private decimal car1;
        private decimal car2;
        private decimal car3;
        private decimal car4;
        private decimal car5;
        private decimal car6;
        private decimal car7;
        private decimal car8;
        private decimal car9;
        private decimal car10;
        private decimal car11;
        private decimal car12;
        private decimal abo1;
        private decimal abo2;
        private decimal abo3;
        private decimal abo4;
        private decimal abo5;
        private decimal abo6;
        private decimal abo7;
        private decimal abo8;
        private decimal abo9;
        private decimal abo10;
        private decimal abo11;
        private decimal abo12;
        private decimal sdoinia;
        private decimal cara1;
        private decimal cara2;
        private decimal cara3;
        private decimal cara4;
        private decimal cara5;
        private decimal cara6;
        private decimal cara7;
        private decimal cara8;
        private decimal cara9;
        private decimal cara10;
        private decimal cara11;
        private decimal cara12;
        private decimal aboa1;
        private decimal aboa2;
        private decimal aboa3;
        private decimal aboa4;
        private decimal aboa5;
        private decimal aboa6;
        private decimal aboa7;
        private decimal aboa8;
        private decimal aboa9;
        private decimal aboa10;
        private decimal aboa11;
        private decimal aboa12;
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Saldo()
        {
            saldoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

        #region Propiedades
        ///<sumary>
        ///Obtiene o establece el(la) Saldoid para Entidad.Contabilidad.Saldo
        ///</sumary>
        public string Saldoid
        {
            get
            {
                return this.saldoid;
            }
            set
            {
                this.saldoid = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) empresaId para Entidad.Contabilidad.Saldo
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
        ///Obtiene o establece el(la) CodEmpresa para Entidad.Contabilidad.Saldo
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
        ///Obtiene o establece el(la) Ejercicio para Entidad.Contabilidad.Saldo
        ///</sumary>
        public string Ejercicio
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
        ///Obtiene o establece el(la) Cuentaid para Entidad.Contabilidad.Saldo
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
        ///Obtiene o establece el(la) Cuenta para Entidad.Contabilidad.Saldo
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
        ///Obtiene o establece el(la) Nivel para Entidad.Contabilidad.Saldo
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
        ///Obtiene o establece el(la) Sdoini para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Sdoini
        {
            get
            {
                return this.sdoini;
            }
            set
            {
                this.sdoini = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car1 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car1
        {
            get
            {
                return this.car1;
            }
            set
            {
                this.car1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car2 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car2
        {
            get
            {
                return this.car2;
            }
            set
            {
                this.car2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car3 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car3
        {
            get
            {
                return this.car3;
            }
            set
            {
                this.car3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car4 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car4
        {
            get
            {
                return this.car4;
            }
            set
            {
                this.car4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car5 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car5
        {
            get
            {
                return this.car5;
            }
            set
            {
                this.car5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car6 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car6
        {
            get
            {
                return this.car6;
            }
            set
            {
                this.car6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car7 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car7
        {
            get
            {
                return this.car7;
            }
            set
            {
                this.car7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car8 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car8
        {
            get
            {
                return this.car8;
            }
            set
            {
                this.car8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car9 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car9
        {
            get
            {
                return this.car9;
            }
            set
            {
                this.car9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car10 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car10
        {
            get
            {
                return this.car10;
            }
            set
            {
                this.car10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car11 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car11
        {
            get
            {
                return this.car11;
            }
            set
            {
                this.car11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Car12 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Car12
        {
            get
            {
                return this.car12;
            }
            set
            {
                this.car12 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo1 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo1
        {
            get
            {
                return this.abo1;
            }
            set
            {
                this.abo1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo2 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo2
        {
            get
            {
                return this.abo2;
            }
            set
            {
                this.abo2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo3 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo3
        {
            get
            {
                return this.abo3;
            }
            set
            {
                this.abo3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo4 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo4
        {
            get
            {
                return this.abo4;
            }
            set
            {
                this.abo4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo5 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo5
        {
            get
            {
                return this.abo5;
            }
            set
            {
                this.abo5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo6 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo6
        {
            get
            {
                return this.abo6;
            }
            set
            {
                this.abo6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo7 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo7
        {
            get
            {
                return this.abo7;
            }
            set
            {
                this.abo7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo8 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo8
        {
            get
            {
                return this.abo8;
            }
            set
            {
                this.abo8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo9 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo9
        {
            get
            {
                return this.abo9;
            }
            set
            {
                this.abo9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo10 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo10
        {
            get
            {
                return this.abo10;
            }
            set
            {
                this.abo10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo11 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo11
        {
            get
            {
                return this.abo11;
            }
            set
            {
                this.abo11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Abo12 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Abo12
        {
            get
            {
                return this.abo12;
            }
            set
            {
                this.abo12 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Sdoinia para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Sdoinia
        {
            get
            {
                return this.sdoinia;
            }
            set
            {
                this.sdoinia = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara1 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara1
        {
            get
            {
                return this.cara1;
            }
            set
            {
                this.cara1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara2 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara2
        {
            get
            {
                return this.cara2;
            }
            set
            {
                this.cara2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara3 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara3
        {
            get
            {
                return this.cara3;
            }
            set
            {
                this.cara3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara4 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara4
        {
            get
            {
                return this.cara4;
            }
            set
            {
                this.cara4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara5 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara5
        {
            get
            {
                return this.cara5;
            }
            set
            {
                this.cara5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara6 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara6
        {
            get
            {
                return this.cara6;
            }
            set
            {
                this.cara6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara7 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara7
        {
            get
            {
                return this.cara7;
            }
            set
            {
                this.cara7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara8 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara8
        {
            get
            {
                return this.cara8;
            }
            set
            {
                this.cara8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara9 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara9
        {
            get
            {
                return this.cara9;
            }
            set
            {
                this.cara9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara10 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara10
        {
            get
            {
                return this.cara10;
            }
            set
            {
                this.cara10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara11 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara11
        {
            get
            {
                return this.cara11;
            }
            set
            {
                this.cara11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Cara12 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Cara12
        {
            get
            {
                return this.cara12;
            }
            set
            {
                this.cara12 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa1 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa1
        {
            get
            {
                return this.aboa1;
            }
            set
            {
                this.aboa1 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa2 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa2
        {
            get
            {
                return this.aboa2;
            }
            set
            {
                this.aboa2 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa3 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa3
        {
            get
            {
                return this.aboa3;
            }
            set
            {
                this.aboa3 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa4 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa4
        {
            get
            {
                return this.aboa4;
            }
            set
            {
                this.aboa4 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa5 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa5
        {
            get
            {
                return this.aboa5;
            }
            set
            {
                this.aboa5 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa6 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa6
        {
            get
            {
                return this.aboa6;
            }
            set
            {
                this.aboa6 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa7 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa7
        {
            get
            {
                return this.aboa7;
            }
            set
            {
                this.aboa7 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa8 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa8
        {
            get
            {
                return this.aboa8;
            }
            set
            {
                this.aboa8 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa9 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa9
        {
            get
            {
                return this.aboa9;
            }
            set
            {
                this.aboa9 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa10 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa10
        {
            get
            {
                return this.aboa10;
            }
            set
            {
                this.aboa10 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa11 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa11
        {
            get
            {
                return this.aboa11;
            }
            set
            {
                this.aboa11 = value;
            }
        }
        ///<sumary>
        ///Obtiene o establece el(la) Aboa12 para Entidad.Contabilidad.Saldo
        ///</sumary>
        public decimal Aboa12
        {
            get
            {
                return this.aboa12;
            }
            set
            {
                this.aboa12 = value;
            }
        }
        #endregion //Propiedades

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Saldo se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.saldoid == ((Saldo)obj).saldoid);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Saldo
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
