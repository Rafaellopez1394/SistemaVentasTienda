using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Homex.Core.Utilities;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using TimbreEdicom.CFDiService;
using System.Text;
using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Data.SqlClient;
using System.Globalization;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class SAT_DividendosRetenciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> ayudaRetencion_FindByCode(string value)
        {
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                Entity.SAT.Timbradoretencion retencion = MobileBO.ControlSAT.TraerTimbradoretenciones(null, values.ID, int.Parse(values.Codigo));
                if (retencion!=null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = retencion.Timbradoretencionid, Codigo = retencion.Folioint.ToString(), Descripcion = retencion.Nacnomdenrazsocr };
                    ListaElementos.Add(elemento);
                }
                else
                {
                    return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, "No se encontró resultado.");
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> ayudaRetencion_FindByPopUp(string value)
        {
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                List<Entity.SAT.Timbradoretencion> ListaRetenciones = MobileBO.ControlSAT.TraerListaTimbradoretenciones(values.ID);
                if (ListaRetenciones.Count > 0)
                {
                    ListaRetenciones.ForEach(retencion =>
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = retencion.Timbradoretencionid, Codigo = retencion.Folioint.ToString(), Descripcion = retencion.Nacnomdenrazsocr };
                        ListaElementos.Add(elemento);
                    });
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> traerInfoRetencion(string retencionid) {
            try
            {
                var retencion = MobileBO.ControlSAT.TraerTimbradoretenciones(retencionid, null, null);
                return Entity.Response<object>.CrearResponse<object>(true, retencion);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<List<object>> TraerEmpresas()
        {
            Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;
            List<object> listaElementos = new List<object>();
            try
            {
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();
                if (empresas != null)
                {
                    foreach (Entity.Configuracion.Catempresa empresa in empresas)
                    {
                        object elemento = new { id = empresa.Empresaid, nombre = empresa.Descripcion };
                        listaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerTipoRetencion()
        {
            List<object> listaElementos = new List<object>();
            try
            {
                listaElementos.Add(new { id = "", nombre = "Seleccione" });
                List<Entity.SAT.SatRetencion> Retenciones = MobileBO.ControlSAT.TraerListaSatRetenciones();
                Retenciones.ForEach(x =>
                {
                    object elemento = new { id = x.Clave, nombre = x.Descripcion };
                    listaElementos.Add(elemento);
                });
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerTipoDividendo()
        {
            List<object> listaElementos = new List<object>();
            try
            {
                listaElementos.Add(new { id = "", nombre = "Seleccione" });
                List<Entity.SAT.SatTipodividendo> Retenciones = MobileBO.ControlSAT.TraerListaSatTipodividendo();
                Retenciones.ForEach(x =>
                {
                    object elemento = new { id = x.Clave, nombre = x.Descripcion };
                    listaElementos.Add(elemento);
                });
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> GuardarRetencion(string value)
        {
            Entity.Response<object> respuesta = new Entity.Response<object>();
            try
            {
                Entity.SAT.Timbradoretencion retencion = MobileBO.Utilerias.Deserializar<Entity.SAT.Timbradoretencion>(value);                
                retencion.Fecha = DateTime.Now;

                string versionCFDI = MobileBO.ControlConfiguracion.TraerConfiguracionversioncfdiVersionActual().Version;

                switch (versionCFDI)
                {
                    case "3.3":
                        respuesta = TimbrarRetencion10(retencion);
                        break;
                    case "4.0":
                        respuesta = TimbrarRetencion20(retencion);
                        break;
                }

                return respuesta;

                //Preparamos el objeto que se va a enviar al webservice de retenciones
                wsRetenciones.retencion objRetencion = new wsRetenciones.retencion();
                objRetencion.FolioInt = retencion.Folioint.ToString();
                objRetencion.Fecha = retencion.Fecharet;
                objRetencion.CveRetenc = retencion.Cveretenc;
                objRetencion.DescRetenc = retencion.Descretenc;
                objRetencion.MesIni = retencion.Mesini.ToString();
                objRetencion.MesFin = retencion.Mesfin.ToString();
                objRetencion.Ejerc = retencion.Ejerc.ToString();
                objRetencion.montoTotOperacion = retencion.Montototoperacion;
                objRetencion.montoTotGrav = retencion.Montototgrav;
                objRetencion.montoTotExent = retencion.Montototexent;
                objRetencion.montoTotRet = retencion.Montototret;                



                wsRetenciones.ReceptorRetencion receptor = new wsRetenciones.ReceptorRetencion();
                receptor.Nacionalidad = retencion.Nacionalidad;
                receptor.NacRFCRecep = retencion.Nacrfcrecep;
                receptor.NacNomDenRazSocR = retencion.Nacnomdenrazsocr;
                receptor.ExtNumRegIdTrib = retencion.Extnumregidtrib;
                receptor.ExtNomDenRazSocR = retencion.Extnomdenrazsocr;
                receptor.DomicilioFiscalR = retencion.DomicilioFiscalR;

                wsRetenciones.impuesto[] impuestos = new wsRetenciones.impuesto[1];
                impuestos[0] = new wsRetenciones.impuesto();
                impuestos[0].BaseRet = retencion.BaseRet;
                impuestos[0].ImpuestoRet = retencion.ImpuestoRet;
                impuestos[0].montoRet = retencion.MontoRet;
                impuestos[0].TipoPagoRet = retencion.TipoPagoRet;
                objRetencion.Impuestos = impuestos;





                if (retencion.Cveretenc == "14")
                {
                    wsRetenciones.Dividendo dividendo = new wsRetenciones.Dividendo();
                    dividendo.CveTipDivOUtil = retencion.Cvetipdivoutil;
                    dividendo.MontISRAcredRetMexico = retencion.Montisracredretmexico;
                    dividendo.MontISRAcredRetExtranjero = retencion.Montisracredretextranjero;
                    dividendo.MontRetExtDivExt = retencion.Montretextdivext;
                    dividendo.TipoSocDistrDiv = retencion.Tiposocdistrdiv;
                    dividendo.MontISRAcredNal = retencion.Montisracrednal;
                    dividendo.MontDivAcumNal = retencion.Montdivacumnal;
                    dividendo.MontDivAcumExt = retencion.Montdivacumext;
                    objRetencion.ProporcionRem = retencion.Proporcionrem;
                    objRetencion.Dividendo = dividendo;
                }
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(retencion.Empresaid);
                wsRetenciones.wsrespuesta Respuesta = new wsRetenciones.wsrespuesta();
                try
                {
                    Respuesta = new wsRetenciones.wsRetencion().sellarRetencion(empresa.Rfc, objRetencion);
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }
                if (Respuesta.resultado)
                {
                    TimbreEdicom.TimbrarFE timbre = new TimbreEdicom.TimbrarFE();
                    retencion.Folioint = int.Parse(Respuesta.folio);
                    retencion.Serie = "R";
                    retencion.Numerocertificado = Respuesta.certificado;
                    retencion.Cadenaoriginal = Respuesta.cadenaoriginal;
                    retencion.Sellodigital = Respuesta.sello;
                    retencion.Importeletra = timbre.ConLetra(double.Parse(retencion.Montototoperacion.ToString()), "P").ToString();

                    try
                    {
                        if (!MobileBO.Utilerias.AccesoInternet())
                            throw new Exception("No hay comunicacion con el servicio de timbrado, informa a sistemas que no hay internet");

                        byte[] CodigoBarrasBidimencional;
                        string UUID, FechaTimbrado, selloSAT, noCertificadoSAT;

                        string NombreArchivoZip = "ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".zip";
                        string NombreArchivoTimbrado = "SIGN_ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".XML";

                        string[] res;
                        res = timbre.TimbrarRetencionEdicom(NombreArchivoZip, NombreArchivoTimbrado, System.AppDomain.CurrentDomain.RelativeSearchPath);
                        UUID = res[0];
                        FechaTimbrado = res[1];
                        selloSAT = res[2];
                        noCertificadoSAT = res[3];
                        CodigoBarrasBidimencional = Base.Clases.Facturacion.GenerarQR(empresa.Rfc, retencion.Nacrfcrecep, retencion.Montototoperacion.ToString().Trim(), UUID, retencion.Sellodigital);

                        retencion.Numerocertificadosat = noCertificadoSAT;
                        retencion.Sellodigitalsat = selloSAT;
                        retencion.Uuid = UUID;
                        retencion.Fechatimbrado = DateTime.Parse(FechaTimbrado);
                        retencion.Xml = res[4];
                        retencion.Qr = CodigoBarrasBidimencional;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ocurrio un error al intentar timbrar el comprobante: " + ex.Message);
                    }

                    try
                    {
                        MobileBO.ControlSAT.GuardarTimbradoretencion(new List<Entity.SAT.Timbradoretencion>() { retencion });
                    }
                    catch (Exception ex) {
                        throw new Exception("Ocurrio un error al intentar guardar el comprobante en el sistema, ALERTA!!! ESTE COMPROBANTE YA ESTA TIMBRADO: " + ex.Message);
                    }


                    return Entity.Response<object>.CrearResponse<object>(true, retencion);
                }
                else
                {
                    throw new Exception("Ocurrio un error al intentar timbrar el documento");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        private static Entity.Response<object> TimbrarRetencion10(Entity.SAT.Timbradoretencion retencion)
        {
            try
            {

                retencion.Fecha = DateTime.Now;


                //Preparamos el objeto que se va a enviar al webservice de retenciones
                wsRetenciones.retencion objRetencion = new wsRetenciones.retencion();
                objRetencion.Version = "1.0";
                objRetencion.FolioInt = retencion.Folioint.ToString();
                objRetencion.Fecha = retencion.Fecharet;
                objRetencion.CveRetenc = retencion.Cveretenc;
                objRetencion.DescRetenc = retencion.Descretenc;
                objRetencion.MesIni = retencion.Mesini.ToString();
                objRetencion.MesFin = retencion.Mesfin.ToString();
                objRetencion.Ejerc = retencion.Ejerc.ToString();
                objRetencion.montoTotOperacion = retencion.Montototoperacion;
                objRetencion.montoTotGrav = retencion.Montototgrav;
                objRetencion.montoTotExent = retencion.Montototexent;
                objRetencion.montoTotRet = retencion.Montototret;



                wsRetenciones.ReceptorRetencion receptor = new wsRetenciones.ReceptorRetencion();
                receptor.Nacionalidad = retencion.Nacionalidad;
                receptor.NacRFCRecep = retencion.Nacrfcrecep;
                receptor.NacNomDenRazSocR = retencion.Nacnomdenrazsocr;
                receptor.ExtNumRegIdTrib = retencion.Extnumregidtrib;
                receptor.ExtNomDenRazSocR = retencion.Extnomdenrazsocr;
                receptor.DomicilioFiscalR = retencion.DomicilioFiscalR;
                objRetencion.Receptor = receptor;

                wsRetenciones.impuesto[] impuestos = new wsRetenciones.impuesto[1];
                impuestos[0] = new wsRetenciones.impuesto();
                impuestos[0].BaseRet = retencion.BaseRet;
                impuestos[0].ImpuestoRet = retencion.ImpuestoRet;
                impuestos[0].montoRet = retencion.MontoRet;
                impuestos[0].TipoPagoRet = retencion.TipoPagoRet;
                objRetencion.Impuestos = impuestos;





                if (retencion.Cveretenc == "14")
                {
                    wsRetenciones.Dividendo dividendo = new wsRetenciones.Dividendo();
                    dividendo.CveTipDivOUtil = retencion.Cvetipdivoutil;
                    dividendo.MontISRAcredRetMexico = retencion.Montisracredretmexico;
                    dividendo.MontISRAcredRetExtranjero = retencion.Montisracredretextranjero;
                    dividendo.MontRetExtDivExt = retencion.Montretextdivext;
                    dividendo.TipoSocDistrDiv = retencion.Tiposocdistrdiv;
                    dividendo.MontISRAcredNal = retencion.Montisracrednal;
                    dividendo.MontDivAcumNal = retencion.Montdivacumnal;
                    dividendo.MontDivAcumExt = retencion.Montdivacumext;
                    objRetencion.ProporcionRem = retencion.Proporcionrem;
                    objRetencion.Dividendo = dividendo;
                }
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(retencion.Empresaid);
                wsRetenciones.wsrespuesta Respuesta = new wsRetenciones.wsrespuesta();
                try
                {
                    Respuesta = new wsRetenciones.wsRetencion().sellarRetencion(empresa.Rfc, objRetencion);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                if (Respuesta.resultado)
                {
                    TimbreEdicom.TimbrarFE timbre = new TimbreEdicom.TimbrarFE();
                    retencion.Folioint = int.Parse(Respuesta.folio);
                    retencion.Serie = "R";
                    retencion.Numerocertificado = Respuesta.certificado;
                    retencion.Cadenaoriginal = Respuesta.cadenaoriginal;
                    retencion.Sellodigital = Respuesta.sello;
                    retencion.Importeletra = timbre.ConLetra(double.Parse(retencion.Montototoperacion.ToString()), "P").ToString();

                    try
                    {
                        if (!MobileBO.Utilerias.AccesoInternet())
                            throw new Exception("No hay comunicacion con el servicio de timbrado, informa a sistemas que no hay internet");

                        byte[] CodigoBarrasBidimencional;
                        string UUID, FechaTimbrado, selloSAT, noCertificadoSAT;

                        string NombreArchivoZip = "ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".zip";
                        string NombreArchivoTimbrado = "SIGN_ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".XML";

                        string[] res;
                        //res = timbre.TimbrarRetencionEdicom(NombreArchivoZip, NombreArchivoTimbrado, System.AppDomain.CurrentDomain.RelativeSearchPath);
                        res = TimbrarRetencionEdicom(NombreArchivoZip, NombreArchivoTimbrado, System.AppDomain.CurrentDomain.RelativeSearchPath);
                        UUID = res[0];
                        FechaTimbrado = res[1];
                        selloSAT = res[2];
                        noCertificadoSAT = res[3];
                        CodigoBarrasBidimencional = Base.Clases.Facturacion.GenerarQR(empresa.Rfc, retencion.Nacrfcrecep, retencion.Montototoperacion.ToString().Trim(), UUID, retencion.Sellodigital);

                        retencion.Numerocertificadosat = noCertificadoSAT;
                        retencion.Sellodigitalsat = selloSAT;
                        retencion.Uuid = UUID;
                        retencion.Fechatimbrado = DateTime.Parse(FechaTimbrado);
                        retencion.Xml = res[4];
                        retencion.Qr = CodigoBarrasBidimencional;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ocurrio un error al intentar timbrar el comprobante: " + ex.Message);
                    }

                    try
                    {
                        MobileBO.ControlSAT.GuardarTimbradoretencion(new List<Entity.SAT.Timbradoretencion>() { retencion });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ocurrio un error al intentar guardar el comprobante en el sistema, ALERTA!!! ESTE COMPROBANTE YA ESTA TIMBRADO: " + ex.Message);
                    }


                    return Entity.Response<object>.CrearResponse<object>(true, retencion);
                }
                else
                {
                    throw new Exception("Ocurrio un error al intentar timbrar el documento");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        private static Entity.Response<object> TimbrarRetencion20(Entity.SAT.Timbradoretencion retencion)
        {
            try
            {
                DateTime fechaRetencion;

                if (!DateTime.TryParseExact(
                        retencion.Fecharet,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fechaRetencion))
                {
                    throw new Exception("Formato de fecha de retención inválido (dd/MM/yyyy)");
                }

                if (fechaRetencion.Date > DateTime.Today)
                {
                    throw new Exception("La fecha de retención no puede ser futura");
                }

                //Preparamos el objeto que se va a enviar al webservice de retenciones
                wsRetenciones.retencion objRetencion = new wsRetenciones.retencion();
                objRetencion.Version = "2.0";
                objRetencion.FolioInt = retencion.Folioint.ToString();
                //objRetencion.Fecha = retencion.Fecharet;
                //objRetencion.Fecha = fechaRetencion.ToString("yyyy-MM-ddTHH:mm:ss",CultureInfo.InvariantCulture);
                //objRetencion.Fecha = fechaSAT;
                objRetencion.Fecha = fechaRetencion.ToString("yyyyMMddHHmmss",CultureInfo.InvariantCulture);

                objRetencion.CveRetenc = retencion.Cveretenc;
                objRetencion.DescRetenc = retencion.Descretenc;
                objRetencion.MesIni = retencion.Mesini.ToString();
                objRetencion.MesFin = retencion.Mesfin.ToString();
                objRetencion.Ejerc = retencion.Ejerc.ToString();
                objRetencion.montoTotOperacion = retencion.Montototoperacion;
                objRetencion.montoTotGrav = retencion.Montototgrav;
                objRetencion.montoTotExent = retencion.Montototexent;
                objRetencion.montoTotRet = retencion.Montototret;



                wsRetenciones.ReceptorRetencion receptor = new wsRetenciones.ReceptorRetencion();
                receptor.Nacionalidad = retencion.Nacionalidad;
                receptor.NacRFCRecep = retencion.Nacrfcrecep;
                receptor.NacNomDenRazSocR = retencion.Nacnomdenrazsocr;
                receptor.ExtNumRegIdTrib = retencion.Extnumregidtrib;
                receptor.ExtNomDenRazSocR = retencion.Extnomdenrazsocr;
                receptor.DomicilioFiscalR = retencion.DomicilioFiscalR;
                objRetencion.Receptor = receptor;

                wsRetenciones.impuesto[] impuestos = new wsRetenciones.impuesto[1];
                impuestos[0] = new wsRetenciones.impuesto();
                impuestos[0].BaseRet = retencion.BaseRet;
                impuestos[0].ImpuestoRet = retencion.ImpuestoRet;
                impuestos[0].montoRet = retencion.MontoRet;
                impuestos[0].TipoPagoRet = retencion.TipoPagoRet;
                objRetencion.Impuestos = impuestos;





                if (retencion.Cveretenc == "14")
                {
                    wsRetenciones.Dividendo dividendo = new wsRetenciones.Dividendo();
                    dividendo.CveTipDivOUtil = retencion.Cvetipdivoutil;
                    dividendo.MontISRAcredRetMexico = retencion.Montisracredretmexico;
                    dividendo.MontISRAcredRetExtranjero = retencion.Montisracredretextranjero;
                    dividendo.MontRetExtDivExt = retencion.Montretextdivext;
                    dividendo.TipoSocDistrDiv = retencion.Tiposocdistrdiv;
                    dividendo.MontISRAcredNal = retencion.Montisracrednal;
                    dividendo.MontDivAcumNal = retencion.Montdivacumnal;
                    dividendo.MontDivAcumExt = retencion.Montdivacumext;
                    objRetencion.ProporcionRem = retencion.Proporcionrem;
                    objRetencion.Dividendo = dividendo;
                }
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(retencion.Empresaid);
                wsRetenciones.wsrespuesta Respuesta = new wsRetenciones.wsrespuesta();
                try
                {
                    Respuesta = new wsRetenciones.wsRetencion().sellarRetencion(empresa.Rfc, objRetencion);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                if (Respuesta.resultado)
                {
                    TimbreEdicom.TimbrarFE timbre = new TimbreEdicom.TimbrarFE();
                    retencion.Folioint = int.Parse(Respuesta.folio);
                    retencion.Serie = "R";
                    retencion.Numerocertificado = Respuesta.certificado;
                    retencion.Cadenaoriginal = Respuesta.cadenaoriginal;
                    retencion.Sellodigital = Respuesta.sello;
                    retencion.Importeletra = timbre.ConLetra(double.Parse(retencion.Montototoperacion.ToString()), "P").ToString();

                    try
                    {
                        if (!MobileBO.Utilerias.AccesoInternet())
                            throw new Exception("No hay comunicacion con el servicio de timbrado, informa a sistemas que no hay internet");

                        byte[] CodigoBarrasBidimencional;
                        string UUID, FechaTimbrado, selloSAT, noCertificadoSAT;

                        string NombreArchivoZip = string.Empty;
                        string NombreArchivoTimbrado = string.Empty;
                        if (empresa.Empresa==1 || empresa.Empresa == 2)
                        {
                            NombreArchivoZip = "ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".zip";
                            NombreArchivoTimbrado = "SIGN_ret_" + (empresa.Sofom ? "4" : "3") + "_" + retencion.Serie + retencion.Folioint + ".XML";
                        }
                        else
                        {
                            NombreArchivoZip = "ret_" + empresa.Empresa.ToString() + "_" + retencion.Serie + retencion.Folioint + ".zip";
                            NombreArchivoTimbrado = "SIGN_ret_" + empresa.Empresa.ToString() + "_" + retencion.Serie + retencion.Folioint + ".XML";
                        }


                        string[] res;
                        //res = timbre.TimbrarRetencionEdicom(NombreArchivoZip, NombreArchivoTimbrado, System.AppDomain.CurrentDomain.RelativeSearchPath);
                        res = TimbrarRetencionEdicom(NombreArchivoZip, NombreArchivoTimbrado, System.AppDomain.CurrentDomain.RelativeSearchPath);
                        UUID = res[0];
                        FechaTimbrado = res[1];
                        selloSAT = res[2];
                        noCertificadoSAT = res[3];
                        CodigoBarrasBidimencional = Base.Clases.Facturacion.GenerarQR(empresa.Rfc, retencion.Nacrfcrecep, retencion.Montototoperacion.ToString().Trim(), UUID, retencion.Sellodigital);

                        retencion.Numerocertificadosat = noCertificadoSAT;
                        retencion.Sellodigitalsat = selloSAT;
                        retencion.Uuid = UUID;
                        retencion.Fechatimbrado = DateTime.Parse(FechaTimbrado);
                        retencion.Xml = res[4];
                        retencion.Qr = CodigoBarrasBidimencional;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ocurrio un error al intentar timbrar el comprobante: " + ex.Message);
                    }

                    try
                    {
                        MobileBO.ControlSAT.GuardarTimbradoretencion(new List<Entity.SAT.Timbradoretencion>() { retencion });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ocurrio un error al intentar guardar el comprobante en el sistema, ALERTA!!! ESTE COMPROBANTE YA ESTA TIMBRADO: " + ex.Message);
                    }


                    return Entity.Response<object>.CrearResponse<object>(true, retencion);
                }
                else
                {
                    throw new Exception("Ocurrio un error al intentar timbrar el documento");
                }
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        public static string[] TimbrarRetencionEdicom(string NombreArchivoZip, string NombreArchivoTimbrado, string RelativePath)
        {
            string[] array = new string[5];
            try
            {
                string text = "";
                string text2 = "";
                string text3 = "";
                string text4 = "";
                CFDiService cFDiService = new CFDiService();
                UTF8Encoding uTF8Encoding = new UTF8Encoding();
                string text5 = "SIGN_XML_COMPROBANTE_3_0.XML";
                StreamReader streamReader = new StreamReader(RelativePath + "\\cfdiClient.properties", Encoding.UTF8);
                string text6 = "";
                ArrayList arrayList = new ArrayList();
                do
                {
                    text6 = streamReader.ReadLine();
                    if (text6 != null)
                    {
                        arrayList.Add(text6.Trim());
                    }
                }
                while (text6 != null);
                streamReader.Close();
                char[] separator = new char[1] { '=' };
                string[] array2 = arrayList[3].ToString().Trim().ToUpper()
                    .Split(separator);
                char[] separator2 = new char[1] { '=' };
                string[] array3 = arrayList[4].ToString().Trim().ToUpper()
                    .Split(separator2);
                char[] separator3 = new char[1] { '=' };
                string[] array4 = arrayList[1].ToString().Trim().Split(separator3);
                char[] separator4 = new char[1] { '=' };
                string[] array5 = arrayList[2].ToString().Trim().Split(separator4);
                string text7 = "";
                if (!File.Exists(array3[1] + NombreArchivoZip.ToUpper().Trim().Replace(".ZIP", ".XML")))
                {
                    throw new Exception("No se encontro archivo: " + array3[1] + NombreArchivoZip.ToUpper().Trim().Replace(".ZIP", ".XML"));
                }
                StreamReader streamReader2 = new StreamReader(array3[1] + NombreArchivoZip.ToUpper().Trim().Replace(".ZIP", ".XML"), Encoding.UTF8);
                string text8 = "";
                ArrayList arrayList2 = new ArrayList();

                //do
                //{
                //    text8 = streamReader2.ReadToEnd();
                //}
                //while (text8 != "");


                text8 = streamReader2.ReadToEnd();
                if (text8 == "" || text8 == null)
                {
                    throw new Exception("Error al leer el archivo: " + array3[1] + NombreArchivoZip.ToUpper().Trim().Replace(".ZIP", ".XML"));
                }
                streamReader2.Close();
                text7 = text8;
                if (File.Exists(array2[1] + text5))
                {
                    File.Delete(array2[1] + text5);
                }
                if (File.Exists(array2[1] + NombreArchivoZip))
                {
                    File.Delete(array2[1] + NombreArchivoZip);
                }
                char[] separator5 = new char[1] { '=' };
                string[] array6 = arrayList[6].ToString().Trim().ToUpper()
                    .Split(separator5);
                byte[] bytes2;
                try
                {
                    byte[] bytes = uTF8Encoding.GetBytes(text7);
                    bytes2 = ((array6[1] != "TRUE") ? cFDiService.getCfdiRetenciones(array4[1], array5[1], bytes) : cFDiService.getCfdiRetencionesTest(array4[1], array5[1], bytes));
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    File.WriteAllBytes(array2[1] + NombreArchivoZip, bytes2);
                    ExtractArchive(array2[1] + NombreArchivoZip, array2[1]);
                    if (File.Exists(array2[1] + text5))
                    {
                        File.Copy(array2[1] + text5, array2[1] + NombreArchivoTimbrado);
                        File.Delete(array2[1] + text5);
                        File.Delete(array2[1] + NombreArchivoZip);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                if (!File.Exists(array2[1] + NombreArchivoTimbrado))
                {
                    throw new Exception("No se encontro archivo: " + array2[1] + NombreArchivoTimbrado);
                }
                File.Copy(array2[1] + NombreArchivoTimbrado, array3[1] + NombreArchivoTimbrado);
                File.Delete(array2[1] + NombreArchivoTimbrado);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(array3[1] + NombreArchivoTimbrado);
                char[] separator6 = new char[1] { ' ' };
                string[] array7 = xmlDocument.DocumentElement.LastChild.LastChild.OuterXml.ToString().Trim().Replace("<tfd:", "")
                    .Replace(" />", "")
                    .Split(separator6);
                foreach (string text9 in array7)
                {
                    if (text9.Contains("UUID"))
                    {
                        text = text9.Replace("\"", "").Replace("UUID=", "");
                    }
                    if (text9.Contains("FechaTimbrado"))
                    {
                        text2 = Convert.ToString(DateTime.Parse(text9.Replace("\"", "").Replace("FechaTimbrado=", "")));
                    }
                    if (text9.Contains("SelloSAT") || text9.Contains("selloSAT"))
                    {
                        text3 = text9.Contains("selloSAT") == true ? text9.Replace("\"", "").Replace("selloSAT=", "") : text9.Replace("\"", "").Replace("SelloSAT=", "");
                    }
                    if (text9.Contains("NoCertificadoSAT")|| text9.Contains("noCertificadoSAT"))
                    {
                        text4 = text9.Contains("noCertificadoSAT") == true ? text9.Replace("\"", "").Replace("noCertificadoSAT=", "") : text9.Replace("\"", "").Replace("NoCertificadoSAT=", "");
                    }
                }
                array[0] = text;
                array[1] = text2;
                array[2] = text3;
                array[3] = text4;
                array[4] = xmlDocument.InnerXml;
            }
            catch (Exception ex3)
            {
                Exception ex4 = ex3;
                Exception ex5 = ex4;
                throw;
            }
            return array;
        }

        public static void ExtractArchive(string zipFilename, string ExtractDir)
        {
            //IL_0009: Unknown result type (might be due to invalid IL or missing references)
            //IL_000f: Expected O, but got Unknown
            ZipInputStream val = new ZipInputStream((Stream)new FileStream(zipFilename, FileMode.Open, FileAccess.Read));
            ZipEntry val2 = val.GetNextEntry();
            Directory.CreateDirectory(ExtractDir);
            FileStream fileStream;
            while (true)
            {
                fileStream = null;
                if (val2 == null)
                {
                    break;
                }
                if (val2.IsDirectory)
                {
                    Directory.CreateDirectory(ExtractDir + "\\" + val2.Name);
                }
                else
                {
                    if (!Directory.Exists(ExtractDir + "\\" + Path.GetDirectoryName(val2.Name)))
                    {
                        Directory.CreateDirectory(ExtractDir + "\\" + Path.GetDirectoryName(val2.Name));
                    }
                    fileStream = new FileStream(ExtractDir + "\\" + val2.Name, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] buffer = new byte[4097];
                    for (int num = ((Stream)(object)val).Read(buffer, 0, 4096); num > 0; num = ((Stream)(object)val).Read(buffer, 0, 4096))
                    {
                        fileStream.Write(buffer, 0, num);
                    }
                    fileStream.Close();
                }
                try
                {
                    val2 = val.GetNextEntry();
                }
                catch (Exception)
                {
                    val2 = null;
                }
            }
        ((Stream)(object)val)?.Close();
            fileStream?.Close();
        }

        [WebMethod]
        public static void GeneraDatosDeTimbreFiscalDigitalRetenciones(string timbreretencionid)
        {
            TimbreEdicom.TimbrarFE fe = new TimbreEdicom.TimbrarFE();
            
            Entity.SAT.Timbradoretencion retencion = MobileBO.ControlSAT.TraerTimbradoretenciones(timbreretencionid, null, null);
            Entity.Configuracion.Catempresa _empresa = MobileBO.ControlConfiguracion.TraerCatempresas(retencion.Empresaid);


            //Obtenemos el importe con letra
            retencion.Importeletra = fe.ConLetra(Convert.ToDouble(retencion.Montototoperacion), "P").ToString();
            //Generar codigo QR
            retencion.Qr = Base.Clases.Facturacion.GenerarQR(_empresa.Rfc, retencion.Nacrfcrecep, retencion.Montototoperacion.ToString().Trim(), retencion.Uuid, retencion.Sellodigital);
            //Generamos la cadena Original
            retencion.Cadenaoriginal = GeneraCadenaOriginalDesdeString(retencion.Xml);

            //Guardamos los datos generados
            MobileBO.ControlSAT.GuardarTimbradoretencion(new List<Entity.SAT.Timbradoretencion> { retencion });
            ActualizarQRRetenciones(retencion.Qr, retencion.Timbradoretencionid);
            
        }

        public static void ActualizarQRRetenciones(byte[] CodigoBarrasBidimencional, string timbradoretencionid)
        {
            SqlConnection conexion = new SqlConnection(new MobileBO.Utilerias().ObtenerCadenaConexionBaseDeDatos("Factoraje"));
            conexion.Open();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandText = "update TimbradoRetenciones set QR=@Codigobidimencional where TimbradoRetencionID=@TimbradoRetencionID";
            comando.Parameters.Add("@Codigobidimencional", System.Data.SqlDbType.Image);
            comando.Parameters.Add("@TimbradoRetencionID", System.Data.SqlDbType.UniqueIdentifier);
            comando.Parameters["@Codigobidimencional"].Value = CodigoBarrasBidimencional;
            comando.Parameters["@TimbradoRetencionID"].Value = Guid.Parse(timbradoretencionid);
            comando.ExecuteNonQuery();
            conexion.Close();
        }

        private static string GeneraCadenaOriginalDesdeArchivo(string rutaarchivo)
        {
            //Cargar el XML
            StreamReader reader = new StreamReader(@"C:/SAT/ejemplo1cfdv33.xml");
            XPathDocument myXPathDoc = new XPathDocument(reader);

            //Cargando el XSLT
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(Path.Combine(System.AppDomain.CurrentDomain.RelativeSearchPath, "retenciones20.xslt"));

            StringWriter str = new StringWriter();
            XmlTextWriter myWriter = new XmlTextWriter(str);

            //Aplicando transformacion
            myXslTrans.Transform(myXPathDoc, null, myWriter);

            //Resultado
            return str.ToString();
        }

        private static string GeneraCadenaOriginalDesdeString(string xmlstring)
        {
            //Cargar el XML
            StringReader reader = new StringReader(xmlstring);
            XPathDocument myXPathDoc = new XPathDocument(reader);

            //Cargando el XSLT
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(Path.Combine(System.AppDomain.CurrentDomain.RelativeSearchPath, "xslt", "retenciones20.xslt"));

            StringWriter str = new StringWriter();
            XmlTextWriter myWriter = new XmlTextWriter(str);

            //Aplicando transformacion
            myXslTrans.Transform(myXPathDoc, null, myWriter);

            //Resultado
            return str.ToString();
        }


    }


    public class ControladorReporteTimbradoRetenciones : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string retencionid = parametros.Get("retencionid");

            try
            {
                var ret = MobileBO.ControlSAT.TraerTimbradoretenciones(retencionid, null, null);                
                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(ret.Empresaid);
                dsEmpresa.Tables[0].TableName = "Empresa";

                //Actualizar los datos del objeto de facturas
                DataSet ds = new DataSet();
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());
                ds.Tables.Add(new List<Entity.SAT.Timbradoretencion>() { ret }.ToDataTable());

                base.NombreReporte = "RetencionPagos";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\RetencionPagos.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }


}