using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.IO;

namespace BalorFinanciera.Contabilidad.Formas
{
    /// <summary>
    /// Summary description for ProcessXML_Contable
    /// </summary>
    public class ProcessXML_Contable : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string saveLocation;
            string fileName;
            string directorio;
            List<Entity.ModeloRespuestaDIOT> ListaRespuesta = new List<Entity.ModeloRespuestaDIOT>();            
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
                        if (xml.Contains("<CFDI:EMISOR")) {
                            string cadenaemisor = xml.Substring(xml.IndexOf("<CFDI:EMISOR"), xml.Length - xml.IndexOf("<CFDI:EMISOR"));
                            string rfcEmisor = cadenaemisor.Substring(cadenaemisor.IndexOf("RFC=\"") + 5, 13);
                            rfcEmisor = rfcEmisor.Replace("\"", "");


                            //Creamos el direccion donde se van a almacenar los xml en el servivor
                            Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null, rfcEmisor, empresaid);
                            if (proveedor == null)
                                throw new Exception("El proveedor: " + rfcEmisor + " de la factura: " + context.Request.Files[i].FileName + " no existe en el sistema, debes registrarlo en el catalogo");
                            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(proveedor.Empresaid);
                            directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
                            directorio += "\\Proveedores\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + proveedor.Rfc;
                            if (!Directory.Exists(directorio))
                                Directory.CreateDirectory(directorio);

                            fileName = Path.GetFileName(context.Request.Files[i].FileName);
                            saveLocation = directorio + "\\" + fileName;
                            if (File.Exists(saveLocation))
                                File.Delete(saveLocation);
                            System.IO.File.WriteAllText(saveLocation, xmlOriginal);

                            //var respuesta = CfdiSAT.CfdiServiceSat.EstadoComprobantePorXML(saveLocation, fileName);
                            //Entity.ModeloRespuestaDIOT respuesta = MobileBO.DIOT.ValidaXML(saveLocation);
                            
                            //2021-05-11
                            //Se cambia el modo de validar las facturas
                            var respuestaValidacion = Base.Clases.Facturacion.LeerXML(saveLocation);

                            Entity.Configuracion.Configuracionfacturacion _conf = MobileBO.ControlConfiguracion.TraerConfiguracionfacturacion()[0];
                            if(_conf.Validarestatus == 1)
                            {
                                string[] res = Base.Clases.Facturacion.ObtenerEstatusCfdi(respuestaValidacion.rfcemisor, respuestaValidacion.foliofiscal, respuestaValidacion.rfcreceptor, Convert.ToDouble(respuestaValidacion.importefactura));
                                respuestaValidacion.estatus = res[1];

                            }
                            else
                            {
                                respuestaValidacion.estatus = "Vigente";
                            }

                            respuestaValidacion.NombreArchivo = fileName;

                            if (File.Exists(saveLocation))
                                File.Delete(saveLocation);
                            System.IO.File.WriteAllText(saveLocation, xmlOriginal);

                            Entity.ModeloRespuestaDIOT respuesta = MobileBO.DIOT.ValidaXML(respuestaValidacion, saveLocation);

                            respuesta.Proveedorid = proveedor.Proveedorid;


                            if (ListaRespuesta.FindAll(x => x.UUID.ToUpper() == respuesta.UUID.ToUpper()).Count > 0)
                            {
                                respuesta.TipoRespuesta = 4;
                                respuesta.Descripcion = "Archivo duplicado";
                            }
                            ListaRespuesta.Add(respuesta);
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