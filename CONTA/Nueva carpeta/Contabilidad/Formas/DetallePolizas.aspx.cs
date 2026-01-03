using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
using System.Xml;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class DetallePolizas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Poliza>> TraerPolizas(string empresaid, int año, int mes, bool pendiente, string tip_pol)
        {
            try
            {
                List<Entity.Contabilidad.Poliza> resultado = MobileBO.ControlContabilidad.TraerPolizasPorFiltros(empresaid, (año == 0 ? DateTime.Now.Year : año), (mes == 0 ? DateTime.Now.Month : mes), pendiente, (tip_pol == "" ? null : tip_pol));
                return Entity.Response<List<Entity.Contabilidad.Poliza>>.CrearResponse<List<Entity.Contabilidad.Poliza>>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Poliza>>.CrearResponseVacio<List<Entity.Contabilidad.Poliza>>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<List<Entity.Operacion.Catfacturasproveedor>> Traerfacturas(string polizaid)
        {
            try
            {
                List<Entity.Operacion.Catfacturasproveedor> resultado = MobileBO.DIOT.TraerFacturasProveedoresPorPolizaID(polizaid);
                return Entity.Response<List<Entity.Operacion.Catfacturasproveedor>>.CrearResponse<List<Entity.Operacion.Catfacturasproveedor>>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Operacion.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Operacion.Catfacturasproveedor>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.ModeloFacturaPoliza> TraerfacturasIF(string polizaid)
        {
            try
            {
                Entity.ModeloFacturaPoliza mp = MobileBO.ControlOperacion.TraerFacturasPorPoliza(polizaid);

                return Entity.Response<Entity.ModeloFacturaPoliza>.CrearResponse<Entity.ModeloFacturaPoliza>(true, mp);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.ModeloFacturaPoliza>.CrearResponseVacio<Entity.ModeloFacturaPoliza>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.ModeloFacturasComplementos>> TraerfacturasComplementos(string polizaid)
        {
            try
            {
                ModeloFacturasComplementos modelo;
                List<ModeloFacturasComplementos> lstModelo = new List<ModeloFacturasComplementos>();
                List<Entity.Operacion.Catfacturasproveedor> facturas = MobileBO.DIOT.TraerFacturasProveedoresPorPolizaID(polizaid);

                foreach (Entity.Operacion.Catfacturasproveedor f in facturas)
                {
                    modelo = new ModeloFacturasComplementos();
                    modelo.UUID = f.Uuid;
                    modelo.Factura = f.Factura;
                    modelo.MetodoPago = f.Metodopago;
                    modelo.Fecha = f.Fechatimbrado;
                    modelo.Subtotal = f.Subtotal;
                    modelo.Iva = f.Iva;
                    modelo.Total = f.Total;

                    if (f.Metodopago == "PUE")
                        modelo.Complemento = 1;
                    else
                    {
                        Entity.Operacion.Catfacturaspago pago = MobileBO.DIOT.TraerCatFacturaPagoPorIdDocumento(f.Uuid);
                        if (pago != null)
                            modelo.Complemento = 1;
                        else
                            modelo.Complemento = 2;
                    }

                    lstModelo.Add(modelo);
                }


                return Entity.Response<List<Entity.ModeloFacturasComplementos>>.CrearResponse<List<Entity.ModeloFacturasComplementos>>(true, lstModelo);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.ModeloFacturasComplementos>>.CrearResponseVacio<List<Entity.ModeloFacturasComplementos>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.ModeloRespuestaNomina>> TraerComplementosNomina(string polizaid)
        {
            List<Entity.ModeloRespuestaNomina> nominas = new List<ModeloRespuestaNomina>();
            List<Entity.Contabilidad.Polizasnomina> noms = MobileBO.ControlContabilidad.TraerPolizasnominaPorPolizaID(polizaid);
            foreach (Entity.Contabilidad.Polizasnomina nom in noms)
            {
                Entity.ModeloRespuestaNomina n = new Entity.ModeloRespuestaNomina();
                n.Polizaid = nom.Polizaid;
                n.Polizanominaid = nom.Polizanominaid;
                n.Emisornombre = nom.Nombreemisor;
                n.Emisorrfc = nom.Rfcemisor;
                n.Estatus = nom.Estatus.ToString();
                n.Factura = nom.Folio;
                n.Sueldo = nom.Sueldo;
                n.Total = (nom.Sueldo + nom.Premioasistencia + nom.Premiopuntualidad) - (nom.Isrretenido + nom.Imss + nom.Infonavit);
                n.TotalPercepciones = (nom.Sueldo + nom.Premioasistencia + nom.Premiopuntualidad);
                n.TotalDeducciones = (nom.Isrretenido + nom.Imss + nom.Infonavit);
                n.UUID = nom.Uuid;
                n.UltimaAct = nom.UltimaAct;
                n.Subtotal = n.TotalPercepciones;
                n.Serie = nom.Serie;
                n.Receptornombre = nom.Nombrereceptor;
                n.Receptorrfc = nom.Rfcreceptor;
                n.PremioPorAsistencia = nom.Premioasistencia;
                n.PremioPorPuntualidad = nom.Premiopuntualidad;
                n.Imss = nom.Imss;
                n.Infonavit = nom.Infonavit;
                n.IsrMensual = nom.Isrretenido;

                XmlDocument xml = new XmlDocument();
                string xmlstring = nom.Nominaxml.ToLower();
                xml.LoadXml(xmlstring);
                System.Xml.XmlNamespaceManager nm = new System.Xml.XmlNamespaceManager(xml.NameTable);

                string cadenaversion = xmlstring.Substring(xmlstring.IndexOf("<cfdi:comprobante"), xmlstring.Length - xmlstring.IndexOf("<cfdi:comprobante"));
                string version = cadenaversion.Substring(cadenaversion.IndexOf("version=\"") + 8, 4);
                version = version.Replace("\"", "");

                switch (version)
                {
                    case "3.3":
                        nm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                        break;
                    case "4.0":
                        nm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
                        break;
                }
                nm.AddNamespace("tfd", "http://www.sat.gob.mx/timbrefiscaldigital");
                nm.AddNamespace("nomina12", "http://www.sat.gob.mx/nomina12");

                if (xml.DocumentElement.SelectSingleNode("/cfdi:comprobante/cfdi:complemento/tfd:timbrefiscaldigital", nm) != null)
                {
                    XmlNode _nodoTFD = xml.DocumentElement.SelectSingleNode("/cfdi:comprobante/cfdi:complemento/tfd:timbrefiscaldigital", nm);
                    n.Fechatimbrado = Convert.ToDateTime(_nodoTFD.Attributes["fechatimbrado"].Value);
                }
                nominas.Add(n);
            }
            nominas = nominas.OrderBy(x => x.Fechatimbrado).ToList();
            return Entity.Response<List<Entity.ModeloRespuestaNomina>>.CrearResponse<List<Entity.ModeloRespuestaNomina>>(true, nominas);
        }
    }
}