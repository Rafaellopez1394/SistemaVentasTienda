using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Contabilidad
{
    [Serializable]
    public class Clientes : Entity.EntidadBase
    {
        #region Atributos privados
        private string clienteID;
        private string razonSocial;
        private string nombreCompleto;
        private string nombre;
        private string apellidoPaterno;
        private string apellidoMaterno;
        private string tipo; // Cliente o Aval
        private string descripcion;
        private DateTime? fechaAlta;

        // Datos del aval o relacionados
        private string avalID;
        private string nombreAval;
        private string representanteLegal;

        // Indicadores de relación
        private bool esAval;
        private bool esAccionista;
        private bool esRepresentante;
        private bool esAccionistaMayoritario;

        // entidfades necesarias para tabla de suspencion

        private string bloqueoID;
        private int tipoPersona;
        private DateTime? fechaNacimiento;
        private string nacionalidad;
        private string tipoDocumento;
        private string numeroDocumento;
        private string listaOFAC;
        private DateTime fechaInclusion;
        private string motivo;
        private string usuarioRegistro;
        private DateTime fechaRegistro;
        private int estatus;


        #endregion

        #region Propiedades públicas

        public string ClienteID
        {
            get { return this.clienteID; }
            set { this.clienteID = value; }
        }

        public string RazonSocial
        {
            get { return this.razonSocial; }
            set { this.razonSocial = value; }
        }

        public string NombreCompleto
        {
            get { return this.nombreCompleto; }
            set { this.nombreCompleto = value; }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set { this.nombre = value; }
        }

        public string ApellidoPaterno
        {
            get { return this.apellidoPaterno; }
            set { this.apellidoPaterno = value; }
        }

        public string ApellidoMaterno
        {
            get { return this.apellidoMaterno; }
            set { this.apellidoMaterno = value; }
        }

        public string Tipo
        {
            get { return this.tipo; }
            set { this.tipo = value; }
        }

        public string Descripcion
        {
            get { return this.descripcion; }
            set { this.descripcion = value; }
        }

        public DateTime? FechaAlta
        {
            get { return this.fechaAlta; }
            set { this.fechaAlta = value; }
        }

        // --- Campos relacionados con el Aval ---
        public string AvalID
        {
            get { return this.avalID; }
            set { this.avalID = value; }
        }

        public string NombreAval
        {
            get { return this.nombreAval; }
            set { this.nombreAval = value; }
        }

        public string RepresentanteLegal
        {
            get { return this.representanteLegal; }
            set { this.representanteLegal = value; }
        }

        // --- Indicadores booleanos ---
        public bool EsAval
        {
            get { return this.esAval; }
            set { this.esAval = value; }
        }

        public bool EsAccionista
        {
            get { return this.esAccionista; }
            set { this.esAccionista = value; }
        }

        public bool EsRepresentante
        {
            get { return this.esRepresentante; }
            set { this.esRepresentante = value; }
        }

        public string BloqueoID
        {
            get { return bloqueoID; }
            set { bloqueoID = value; }
        }

        public int TipoPersona
        {
            get { return tipoPersona; }
            set { tipoPersona = value; }
        }

        public DateTime? FechaNacimiento
        {
            get { return fechaNacimiento; }
            set { fechaNacimiento = value; }
        }

        public string Nacionalidad
        {
            get { return nacionalidad; }
            set { nacionalidad = value; }
        }

        public string TipoDocumento
        {
            get { return tipoDocumento; }
            set { tipoDocumento = value; }
        }

        public string NumeroDocumento
        {
            get { return numeroDocumento; }
            set { numeroDocumento = value; }
        }

        public string ListaOFAC
        {
            get { return listaOFAC; }
            set { listaOFAC = value; }
        }

        public DateTime FechaInclusion
        {
            get { return fechaInclusion; }
            set { fechaInclusion = value; }
        }

        public string Motivo
        {
            get { return motivo; }
            set { motivo = value; }
        }

        public string UsuarioRegistro
        {
            get { return usuarioRegistro; }
            set { usuarioRegistro = value; }
        }

        public DateTime FechaRegistro
        {
            get { return fechaRegistro; }
            set { fechaRegistro = value; }
        }

        public int Estatus
        {
            get { return estatus; }
            set { estatus = value; }
        }

        public bool EsAccionistaMayoritario
        {
            get { return this.esAccionistaMayoritario; }
            set { this.esAccionistaMayoritario = value; }
        }

        #endregion

        #region Métodos sobrecargados
        public override string ToString()
        {
            string tipoTexto = string.IsNullOrEmpty(this.Tipo) ? "Sin tipo" : this.Tipo;
            return $"{this.NombreCompleto} ({tipoTexto})";
        }
        #endregion
    }

}
