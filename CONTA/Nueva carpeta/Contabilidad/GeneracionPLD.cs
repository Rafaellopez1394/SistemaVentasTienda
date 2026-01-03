using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Contabilidad
{
    public class GeneracionPLD : Entity.EntidadBase
    {
        #region Atributos
        private string id_cliente;
        private string fecha_inicio;
        private string importe_inicial;
        private string plazos;
        private string periodicidad;
        private string tasa_anual;
        private string comision;
        private string tipo_credito;
        private string no_credito;
        private string tipo_moneda;
        private string instrumentos_monetarios;
        private string iva;
        private string origen;
        private string observaciones;
        private string tipo_calculo;
        private string convenio;
        private string fecha_inicio_pago;
        private string tasa_moratorio_anual;
        private string comision_disposicion;
        private string comision_prepago;
        private string comision_operacion;
        private string importe_seguro;
        private string importe_gps;
        private string importe_comision;
        private string importe_residual;
        private string seguro_informativo;
        private string incluye_gps_importe;
        private string importe_enganche;
        private string cat;


        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public GeneracionPLD()
        {
            id_cliente = System.String.Empty;
        }

        public string Id_cliente
        {
            get
            {
                return id_cliente;
            }

            set
            {
                id_cliente = value;
            }
        }
        public string Fecha_inicio
        {
            get
            {
                return fecha_inicio;
            }

            set
            {
                fecha_inicio = value;
            }
        }

        public string Importe_inicial
        {
            get
            {
                return importe_inicial;
            }

            set
            {
                importe_inicial = value;
            }
        }

        public string Plazos
        {
            get
            {
                return plazos;
            }

            set
            {
                plazos = value;
            }
        }

        public string Periodicidad
        {
            get
            {
                return periodicidad;
            }

            set
            {
                periodicidad = value;
            }
        }

        public string Tasa_anual
        {
            get
            {
                return tasa_anual;
            }

            set
            {
                tasa_anual = value;
            }
        }

        public string Comision
        {
            get
            {
                return comision;
            }

            set
            {
                comision = value;
            }
        }

        public string Tipo_credito
        {
            get
            {
                return tipo_credito;
            }

            set
            {
                tipo_credito = value;
            }
        }

        public string No_credito
        {
            get
            {
                return no_credito;
            }

            set
            {
                no_credito = value;
            }
        }

        public string Tipo_moneda
        {
            get
            {
                return tipo_moneda;
            }

            set
            {
                tipo_moneda = value;
            }
        }

        public string Instrumentos_monetarios
        {
            get
            {
                return instrumentos_monetarios;
            }

            set
            {
                instrumentos_monetarios = value;
            }
        }

        public string Iva
        {
            get
            {
                return iva;
            }

            set
            {
                iva = value;
            }
        }

        public string Origen
        {
            get
            {
                return origen;
            }

            set
            {
                origen = value;
            }
        }

        public string Observaciones
        {
            get
            {
                return observaciones;
            }

            set
            {
                observaciones = value;
            }
        }

        public string Tipo_calculo
        {
            get
            {
                return tipo_calculo;
            }

            set
            {
                tipo_calculo = value;
            }
        }

        public string Convenio
        {
            get
            {
                return convenio;
            }

            set
            {
                convenio = value;
            }
        }

        public string Fecha_inicio_pago
        {
            get
            {
                return fecha_inicio_pago;
            }

            set
            {
                fecha_inicio_pago = value;
            }
        }

        public string Tasa_moratorio_anual
        {
            get
            {
                return tasa_moratorio_anual;
            }

            set
            {
                tasa_moratorio_anual = value;
            }
        }

        public string Comision_disposicion
        {
            get
            {
                return comision_disposicion;
            }

            set
            {
                comision_disposicion = value;
            }
        }

        public string Comision_prepago
        {
            get
            {
                return comision_prepago;
            }

            set
            {
                comision_prepago = value;
            }
        }

        public string Comision_operacion
        {
            get
            {
                return comision_operacion;
            }

            set
            {
                comision_operacion = value;
            }
        }

        public string Importe_seguro
        {
            get
            {
                return importe_seguro;
            }

            set
            {
                importe_seguro = value;
            }
        }

        public string Importe_gps
        {
            get
            {
                return importe_gps;
            }

            set
            {
                importe_gps = value;
            }
        }

        public string Importe_comision
        {
            get
            {
                return importe_comision;
            }

            set
            {
                importe_comision = value;
            }
        }

        public string Importe_residual
        {
            get
            {
                return importe_residual;
            }

            set
            {
                importe_residual = value;
            }
        }

        public string Seguro_informativo
        {
            get
            {
                return seguro_informativo;
            }

            set
            {
                seguro_informativo = value;
            }
        }

        public string Incluye_gps_importe
        {
            get
            {
                return incluye_gps_importe;
            }

            set
            {
                incluye_gps_importe = value;
            }
        }

        public string Importe_enganche
        {
            get
            {
                return importe_enganche;
            }

            set
            {
                importe_enganche = value;
            }
        }

        public string Cat
        {
            get
            {
                return cat;
            }

            set
            {
                cat = value;
            }
        }




        #endregion Constructor

        #region Métodos SobreCargados
        /// <sumary>
        /// Determina si las instancias de Entidad.Contabilidad.Acvctam se consideran iguales
        /// </sumary>
        /// <param name="obj">objeto a comparar</param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            //TODO:Agregar la comparación de la llave primaria con el objeto que viene por parametro (this.almacenId == ((Almacen)obj).almacenId );
            return (this.Id_cliente == ((GeneracionPLD)obj).Id_cliente);
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
        /// Obtiene la descripción de Entidad.Contabilidad.Acvctam
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            //TODO:falta agregar el campo descriptivo de la entidad: this.Descripcion;
            return base.ToString();
        }
        #endregion //Métodos SobreCargados
    }
    public class Cliente
    {
        public int idpld { get; set; }
        public int idfactoraje { get; set; }
        public int Idpld
        {
            get { return idpld; }
            set
            {
                if (value <= 0)  // Validación: No permitir valores negativos o cero
                    throw new ArgumentException("El valor de idpld debe ser un número mayor que cero.");
                idpld = value;
            }
        }

        public int Idfactoraje
        {
            get { return idfactoraje; }
            set
            {
                if (value <= 0)  // Validación: No permitir valores negativos o cero
                    throw new ArgumentException("El valor de idfactoraje debe ser un número mayor que cero.");
                idfactoraje = value;
            }
        }
    }

    public class ArchivoGenerado
    {
        private string nombreArchivo;
        private string contenidoBase64;

        public string NombreArchivo
        {
            get { return nombreArchivo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre del archivo no puede estar vacío.");
                nombreArchivo = value;
            }
        }


        public string ContenidoBase64
        {
            get { return contenidoBase64; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El contenido del archivo no puede estar vacío.");
                contenidoBase64 = value;
            }
        }
       
    }
    public class Creditos : Entity.EntidadBase
    {
        #region Atributos
       
            public int IdCredito { get; set; }
            public int NoCredito { get; set; }
            public int IdCliente { get; set; }
            public string TipoDeCredito { get; set; }
            public DateTime FechaDeInicio { get; set; }
            public string RFC { get; set; }
        
        #endregion //Atributos

        public List<Creditos> Creditoss { get; set; } = new List<Creditos>();
    }
    
       
    
}
