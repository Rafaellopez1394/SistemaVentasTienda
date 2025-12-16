using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using CapaModelo;

namespace CapaDatos.PAC
{
    /// <summary>
    /// Genera XML de CFDI 4.0 con Complemento de Nómina 1.2
    /// Según especificación del SAT Anexo 20
    /// http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/Nomina12.pdf
    /// </summary>
    public class CFDINomina12XMLGenerator
    {
        /// <summary>
        /// Genera el XML del recibo de nómina sin timbrar
        /// </summary>
        public string GenerarXML(NominaDetalle recibo, ConfiguracionEmpresa empresa, ConfiguracionPAC config)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();

                // Comprobante (nodo raíz) - CFDI 4.0
                writer.WriteStartElement("cfdi", "Comprobante", "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xmlns", "nomina12", null, "http://www.sat.gob.mx/nomina12");
                writer.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance",
                    "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd " +
                    "http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd");

                // Atributos del comprobante
                writer.WriteAttributeString("Version", "4.0");
                
                if (!string.IsNullOrEmpty(empresa.SerieCFDINomina))
                    writer.WriteAttributeString("Serie", empresa.SerieCFDINomina);
                
                writer.WriteAttributeString("Folio", recibo.NumeroEmpleado + "-" + DateTime.Now.ToString("yyyyMMdd"));
                writer.WriteAttributeString("Fecha", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                writer.WriteAttributeString("Sello", ""); // Se agrega después del sellado
                writer.WriteAttributeString("NoCertificado", empresa.NoCertificado ?? "");
                writer.WriteAttributeString("Certificado", empresa.Certificado ?? "");
                writer.WriteAttributeString("SubTotal", FormatDecimal(recibo.TotalPercepciones));
                writer.WriteAttributeString("Descuento", FormatDecimal(recibo.TotalDeducciones));
                writer.WriteAttributeString("Moneda", "MXN");
                writer.WriteAttributeString("Total", FormatDecimal(recibo.NetoPagar));
                writer.WriteAttributeString("TipoDeComprobante", "N"); // N = Nómina
                writer.WriteAttributeString("Exportacion", "01"); // 01 = No aplica
                writer.WriteAttributeString("MetodoPago", "PUE"); // Pago en Una Sola Exhibición
                writer.WriteAttributeString("LugarExpedicion", empresa.CodigoPostal);

                // Emisor (Empresa)
                EscribirEmisor(writer, empresa);

                // Receptor (Empleado)
                EscribirReceptor(writer, recibo);

                // Conceptos (Un solo concepto: "Pago de nómina")
                EscribirConceptos(writer, recibo);

                // Complemento de Nómina 1.2
                EscribirComplementoNomina(writer, recibo, empresa);

                writer.WriteEndElement(); // Comprobante
                writer.WriteEndDocument();
            }

            return sb.ToString();
        }

        private void EscribirEmisor(XmlWriter writer, ConfiguracionEmpresa empresa)
        {
            writer.WriteStartElement("cfdi", "Emisor", null);
            writer.WriteAttributeString("Rfc", empresa.RFC);
            writer.WriteAttributeString("Nombre", empresa.RazonSocial);
            writer.WriteAttributeString("RegimenFiscal", empresa.RegimenFiscal);
            writer.WriteEndElement();
        }

        private void EscribirReceptor(XmlWriter writer, NominaDetalle recibo)
        {
            writer.WriteStartElement("cfdi", "Receptor", null);
            writer.WriteAttributeString("Rfc", recibo.RFC);
            writer.WriteAttributeString("Nombre", recibo.NombreEmpleado);
            writer.WriteAttributeString("DomicilioFiscalReceptor", "00000"); // CP del empleado si se tiene
            writer.WriteAttributeString("RegimenFiscalReceptor", "605"); // 605 = Sueldos y Salarios e Ingresos Asimilados a Salarios
            writer.WriteAttributeString("UsoCFDI", "CN01"); // CN01 = Nómina
            writer.WriteEndElement();
        }

        private void EscribirConceptos(XmlWriter writer, NominaDetalle recibo)
        {
            writer.WriteStartElement("cfdi", "Conceptos", null);
            
            writer.WriteStartElement("cfdi", "Concepto", null);
            writer.WriteAttributeString("ClaveProdServ", "84111505"); // Servicios de personal
            writer.WriteAttributeString("Cantidad", "1");
            writer.WriteAttributeString("ClaveUnidad", "ACT"); // Actividad
            writer.WriteAttributeString("Descripcion", "Pago de nómina");
            writer.WriteAttributeString("ValorUnitario", FormatDecimal(recibo.TotalPercepciones));
            writer.WriteAttributeString("Importe", FormatDecimal(recibo.TotalPercepciones));
            writer.WriteAttributeString("Descuento", FormatDecimal(recibo.TotalDeducciones));
            writer.WriteAttributeString("ObjetoImp", "01"); // 01 = No objeto de impuesto
            writer.WriteEndElement(); // Concepto
            
            writer.WriteEndElement(); // Conceptos
        }

        private void EscribirComplementoNomina(XmlWriter writer, NominaDetalle recibo, ConfiguracionEmpresa empresa)
        {
            writer.WriteStartElement("cfdi", "Complemento", null);
            
            // Nómina 1.2
            writer.WriteStartElement("nomina12", "Nomina", null);
            writer.WriteAttributeString("Version", "1.2");
            writer.WriteAttributeString("TipoNomina", "O"); // O = Ordinaria, E = Extraordinaria
            writer.WriteAttributeString("FechaPago", recibo.FechaFin.ToString("yyyy-MM-dd"));
            writer.WriteAttributeString("FechaInicialPago", recibo.FechaInicio.ToString("yyyy-MM-dd"));
            writer.WriteAttributeString("FechaFinalPago", recibo.FechaFin.ToString("yyyy-MM-dd"));
            writer.WriteAttributeString("NumDiasPagados", FormatDecimal(recibo.DiasTrabajados));
            writer.WriteAttributeString("TotalPercepciones", FormatDecimal(recibo.TotalPercepciones));
            writer.WriteAttributeString("TotalDeducciones", FormatDecimal(recibo.TotalDeducciones));
            
            // Opcional: TotalOtrosPagos si existen

            // Emisor (Registro patronal)
            writer.WriteStartElement("nomina12", "Emisor", null);
            if (!string.IsNullOrEmpty(empresa.RegistroPatronal))
                writer.WriteAttributeString("RegistroPatronal", empresa.RegistroPatronal);
            writer.WriteEndElement(); // nomina12:Emisor

            // Receptor (Datos del empleado)
            EscribirReceptorNomina(writer, recibo);

            // Percepciones
            if (recibo.Percepciones != null && recibo.Percepciones.Any())
            {
                EscribirPercepciones(writer, recibo);
            }

            // Deducciones
            if (recibo.Deducciones != null && recibo.Deducciones.Any())
            {
                EscribirDeducciones(writer, recibo);
            }

            // OtrosPagos (opcional - subsidio al empleo, etc.)
            // TODO: Implementar si se requiere

            writer.WriteEndElement(); // nomina12:Nomina
            writer.WriteEndElement(); // cfdi:Complemento
        }

        private void EscribirReceptorNomina(XmlWriter writer, NominaDetalle recibo)
        {
            writer.WriteStartElement("nomina12", "Receptor", null);
            writer.WriteAttributeString("Curp", recibo.CURP ?? ""); // CURP del empleado
            
            if (!string.IsNullOrEmpty(recibo.NSS))
                writer.WriteAttributeString("NumSeguridadSocial", recibo.NSS);
            
            writer.WriteAttributeString("FechaInicioRelLaboral", recibo.FechaIngreso?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"));
            writer.WriteAttributeString("Antigüedad", "P" + CalcularAntiguedad(recibo.FechaIngreso ?? DateTime.Now.AddYears(-1)) + "W"); // Formato ISO 8601 duration
            writer.WriteAttributeString("TipoContrato", recibo.TipoContrato ?? "01"); // 01 = Contrato por tiempo indeterminado
            writer.WriteAttributeString("Sindicalizado", recibo.Sindicalizado ? "Sí" : "No");
            writer.WriteAttributeString("TipoJornada", recibo.TipoJornada ?? "01"); // 01 = Diurna
            writer.WriteAttributeString("TipoRegimen", recibo.TipoRegimen ?? "02"); // 02 = Sueldos
            writer.WriteAttributeString("NumEmpleado", recibo.NumeroEmpleado);
            
            if (!string.IsNullOrEmpty(recibo.Departamento))
                writer.WriteAttributeString("Departamento", recibo.Departamento);
            
            writer.WriteAttributeString("Puesto", recibo.Puesto ?? "Empleado");
            writer.WriteAttributeString("RiesgoPuesto", recibo.RiesgoPuesto ?? "1"); // Clase de riesgo: 1-5
            writer.WriteAttributeString("PeriodicidadPago", recibo.PeriodicidadPago ?? "04"); // 04 = Semanal, 05 = Quincenal
            
            if (recibo.Banco != null)
            {
                writer.WriteAttributeString("Banco", recibo.Banco.ToString().PadLeft(3, '0'));
                if (!string.IsNullOrEmpty(recibo.CuentaBancaria))
                    writer.WriteAttributeString("CuentaBancaria", recibo.CuentaBancaria);
            }
            
            writer.WriteAttributeString("SalarioBaseCotApor", FormatDecimal(recibo.SalarioDiario));
            writer.WriteAttributeString("SalarioDiarioIntegrado", FormatDecimal(recibo.SalarioIntegrado ?? recibo.SalarioDiario));
            writer.WriteAttributeString("ClaveEntFed", recibo.ClaveEntidadFederativa ?? "CHH"); // Chihuahua por defecto
            
            writer.WriteEndElement(); // nomina12:Receptor
        }

        private void EscribirPercepciones(XmlWriter writer, NominaDetalle recibo)
        {
            writer.WriteStartElement("nomina12", "Percepciones", null);
            writer.WriteAttributeString("TotalSueldos", FormatDecimal(recibo.TotalPercepcionesGravadas));
            writer.WriteAttributeString("TotalGravado", FormatDecimal(recibo.TotalPercepcionesGravadas));
            writer.WriteAttributeString("TotalExento", FormatDecimal(recibo.TotalPercepcionesExentas));

            foreach (var percepcion in recibo.Percepciones.OrderBy(p => p.Clave))
            {
                writer.WriteStartElement("nomina12", "Percepcion", null);
                writer.WriteAttributeString("TipoPercepcion", percepcion.TipoPercepcion);
                writer.WriteAttributeString("Clave", percepcion.Clave);
                writer.WriteAttributeString("Concepto", percepcion.Concepto);
                writer.WriteAttributeString("ImporteGravado", FormatDecimal(percepcion.ImporteGravado));
                writer.WriteAttributeString("ImporteExento", FormatDecimal(percepcion.ImporteExento));
                writer.WriteEndElement(); // nomina12:Percepcion
            }

            writer.WriteEndElement(); // nomina12:Percepciones
        }

        private void EscribirDeducciones(XmlWriter writer, NominaDetalle recibo)
        {
            writer.WriteStartElement("nomina12", "Deducciones", null);
            writer.WriteAttributeString("TotalOtrasDeducciones", FormatDecimal(recibo.TotalOtrasDeducciones));
            writer.WriteAttributeString("TotalImpuestosRetenidos", FormatDecimal(recibo.TotalImpuestosRetenidos));

            foreach (var deduccion in recibo.Deducciones.OrderBy(d => d.Clave))
            {
                writer.WriteStartElement("nomina12", "Deduccion", null);
                writer.WriteAttributeString("TipoDeduccion", deduccion.TipoDeduccion);
                writer.WriteAttributeString("Clave", deduccion.Clave);
                writer.WriteAttributeString("Concepto", deduccion.Concepto);
                writer.WriteAttributeString("Importe", FormatDecimal(deduccion.Importe));
                writer.WriteEndElement(); // nomina12:Deduccion
            }

            writer.WriteEndElement(); // nomina12:Deducciones
        }

        private string FormatDecimal(decimal value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private int CalcularAntiguedad(DateTime fechaIngreso)
        {
            // Calcula antigüedad en semanas (según formato ISO 8601 duration)
            TimeSpan diferencia = DateTime.Now - fechaIngreso;
            return (int)(diferencia.TotalDays / 7);
        }
    }

    /// <summary>
    /// Clase auxiliar para configuración de la empresa (datos del emisor)
    /// </summary>
    public class ConfiguracionEmpresa
    {
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string RegimenFiscal { get; set; }
        public string CodigoPostal { get; set; }
        public string NoCertificado { get; set; }
        public string Certificado { get; set; }
        public string RegistroPatronal { get; set; }
        public string SerieCFDINomina { get; set; }
    }
}
