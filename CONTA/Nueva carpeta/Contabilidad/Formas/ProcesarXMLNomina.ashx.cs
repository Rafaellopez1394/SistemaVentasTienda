using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BalorFinanciera.Contabilidad.Formas
{
    /// <summary>
    /// Summary description for ProcesarXMLNomina
    /// </summary>
    public class ProcesarXMLNomina : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string saveLocation;
            string fileName;
            string directorio;
            List<Entity.ModeloRespuestaNomina> ListaRespuesta = new List<Entity.ModeloRespuestaNomina>();
            try
            {
                if (context.Request.Files.Count > 0)
                {
                    string empresaid = context.Request.Params[0].ToString();
                    DateTime Fecha = DateTime.Parse(context.Request.Params["fecha"].ToString());
                    for (int i = 0; i < context.Request.Files.Count; i++)
                    {
                        //Codigo para obtener el RFC del proveedor directamente del xml
                        string xml = "";
                        string line;
                        System.IO.StreamReader file = new System.IO.StreamReader(context.Request.Files[i].InputStream);
                        while ((line = file.ReadLine()) != null)
                        {
                            xml += line.Replace(Environment.NewLine, " ") + " ";
                        }
                        file.Close();
                        if (xml.Substring(0, 1) == "?")
                            xml = xml.Substring(1, xml.Length - 1);

                        string xmlOriginal = xml;
                        xml = xml.Replace("ꨩ", "").Replace("�", "").ToUpper();

                        string cadenaversion = xml.Substring(xml.IndexOf("<CFDI:COMPROBANTE"), xml.Length - xml.IndexOf("<CFDI:COMPROBANTE"));
                        string version = cadenaversion.Substring(cadenaversion.IndexOf("VERSION=\"") + 5, 13);
                        version = version.Replace("\"", "");

                        if (xml.Contains("<CFDI:EMISOR"))
                        {
                            string cadenaemisor = xml.Substring(xml.IndexOf("<CFDI:EMISOR"), xml.Length - xml.IndexOf("<CFDI:EMISOR"));
                            string rfcEmisor = cadenaemisor.Substring(cadenaemisor.IndexOf("RFC=\"") + 5, 13);
                            rfcEmisor = rfcEmisor.Replace("\"", "");

                            string cadenareceptor = xml.Substring(xml.IndexOf("<CFDI:RECEPTOR"), xml.Length - xml.IndexOf("<CFDI:RECEPTOR"));
                            string rfcReceptor = cadenareceptor.Substring(cadenareceptor.IndexOf("RFC=\"") + 5, 13);
                            rfcReceptor = rfcReceptor.Replace("\"", "");

                            

                            //Creamos el direccion donde se van a almacenar los xml en el servivor
                            //Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null, rfcEmisor, empresaid);
                            //if (proveedor == null)
                            //    throw new Exception("El proveedor: " + rfcEmisor + " de la factura: " + context.Request.Files[i].FileName + " no existe en el sistema, debes registrarlo en el catalogo");
                            Entity.Configuracion.Catempresa Empresa;
                            List<DataRow> _empresas = MobileBO.ControlConfiguracion.CatempresaReporte().Tables[0].AsEnumerable().Where(x => x.Field<string>("Rfc").ToString() == rfcEmisor).ToList();

                            if(_empresas.Count == 0)
                            {
                                context.Response.Write("Error: No se encontró el RFC del emisor especificado en el archivo");
                                return;
                            }

                            if(_empresas[0]["EmpresaID"].ToString().ToUpper() != empresaid.ToUpper())
                            {
                                context.Response.Write("Error: El archivo XML no corresponde a " + _empresas[0]["Empresa"].ToString().ToUpper());
                                return;
                            }

                            Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(_empresas[0]["EmpresaID"].ToString());


                            directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
                            directorio += "\\XmlNomina\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + rfcReceptor;
                            if (!Directory.Exists(directorio))
                                Directory.CreateDirectory(directorio);

                            fileName = Path.GetFileName(context.Request.Files[i].FileName);
                            saveLocation = directorio + "\\" + fileName;
                            if (File.Exists(saveLocation))
                                File.Delete(saveLocation);
                            System.IO.File.WriteAllText(saveLocation, xmlOriginal);

                            var respuestaValidacion = Base.Clases.Facturacion.LeerXMLNomina(saveLocation);

                            Entity.Configuracion.Configuracionfacturacion _conf = MobileBO.ControlConfiguracion.TraerConfiguracionfacturacion()[0];
                            if (_conf.Validarestatus == 1)
                            {
                                string[] res = Base.Clases.Facturacion.ObtenerEstatusCfdi(respuestaValidacion.Emisorrfc, respuestaValidacion.UUID, respuestaValidacion.Receptorrfc, Convert.ToDouble(respuestaValidacion.Total));
                                respuestaValidacion.Estatus = res[1];

                            }
                            else
                            {
                                respuestaValidacion.Estatus = "Vigente";
                            }

                            respuestaValidacion.NomArchivo = fileName;

                            if (File.Exists(saveLocation))
                                File.Delete(saveLocation);
                            System.IO.File.WriteAllText(saveLocation, xmlOriginal);

                            
                            if (ListaRespuesta.FindAll(x => x.UUID.ToUpper() == respuestaValidacion.UUID.ToUpper()).Count > 0)
                            {
                                respuestaValidacion.TipoRespuesta = 4;
                                respuestaValidacion.Descripcion = "Archivo duplicado";
                            }
                            ListaRespuesta.Add(respuestaValidacion);
                        }
                    }
                    context.Response.Write(MobileBO.Utilerias.Serializar(ListaRespuesta));
                }
                else
                {
                    context.Response.Write("No se encontro el archivo que se desea revisar");
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message.ToString());
            }
            finally
            {
                context.Response.Flush();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}