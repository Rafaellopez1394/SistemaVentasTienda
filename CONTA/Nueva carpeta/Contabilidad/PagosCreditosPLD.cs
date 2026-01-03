using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Contabilidad
{
    public class PagosCreditosPLD : Entity.EntidadBase
    {
        #region Atributos

        private string id_credito;
        private string folio;
        private string fecha;
        private string id_instrumento_monetario;
        private string banco;
        private string cuenta;
        private string referencia;
        private string tipo_operacion;
        private string tipo_moneda;
        private string monto_total;
        private string observaciones;
        private string transaccion;
        private string pais;
        private string tipo_pago;

        public List<PagosCreditosPLD> pagosCredito { get; set; } = new List<PagosCreditosPLD>();

        public string Id_credito
        {
            get
            {
                return id_credito;
            }

            set
            {
                id_credito = value;
            }
        }

        public string Folio
        {
            get
            {
                return folio;
            }

            set
            {
                folio = value;
            }
        }

        public string Fecha
        {
            get
            {
                return fecha;
            }

            set
            {
                fecha = value;
            }
        }

        public string Id_instrumento_monetario
        {
            get
            {
                return id_instrumento_monetario;
            }

            set
            {
                id_instrumento_monetario = value;
            }
        }

        public string Banco
        {
            get
            {
                return banco;
            }

            set
            {
                banco = value;
            }
        }

        public string Cuenta
        {
            get
            {
                return cuenta;
            }

            set
            {
                cuenta = value;
            }
        }

        public string Referencia
        {
            get
            {
                return referencia;
            }

            set
            {
                referencia = value;
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

        public string Tipo_operacion
        {
            get
            {
                return tipo_operacion;
            }

            set
            {
                tipo_operacion = value;
            }
        }

        public string Monto_total
        {
            get
            {
                return monto_total;
            }

            set
            {
                monto_total = value;
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

        public string Transaccion
        {
            get
            {
                return transaccion;
            }

            set
            {
                transaccion = value;
            }
        }

        public string Pais
        {
            get
            {
                return pais;
            }

            set
            {
                pais = value;
            }
        }

        public string Tipo_pago
        {
            get
            {
                return tipo_pago;
            }

            set
            {
                tipo_pago = value;
            }
        }
        public class ArchivoGenerado2
        {
            private string nombreArchivo2;
            private string contenidoBase642;

            public string NombreArchivo
            {
                get { return nombreArchivo2; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("El nombre del archivo no puede estar vacío.");
                    nombreArchivo2 = value;
                }
            }


            public string ContenidoBase642
            {
                get { return contenidoBase642; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("El contenido del archivo no puede estar vacío.");
                    contenidoBase642 = value;
                }
            }

        }

        #endregion //Atributos
    }
} 
