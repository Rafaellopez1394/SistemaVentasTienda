using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteXML : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteReporteXml : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                string usuario = parametros.Get("usuario");
                string empresaid = parametros.Get("empresaid");
                DateTime? fechainicial = DateTime.Parse(parametros.Get("fechainicial"));
                DateTime? fechafinal = DateTime.Parse(parametros.Get("fechafinal"));                
                int formato = int.Parse(parametros.Get("formato")); //formato=1 "PDF", formato=2 "Excel"
                bool validarvigencia = bool.Parse(parametros.Get("validarvigencia"));

                List<Entity.Operacion.Catfacturasproveedor> _facturas = MobileBO.ControlOperacion.TraerCatfacturasproveedores(null, null, null, empresaid, null, null, null, fechainicial, fechafinal);                

                string _vigencia = string.Empty;

                Application excel = new Application();
                if (excel == null)
                    throw new Exception("Problemas al generar el archivo");


                //ESCRIBIR SOBRE UN ARCHIVO EXISTENTE
                _Workbook libro;
                _Worksheet hoja;
                excel.Visible = false;
                libro = excel.Workbooks.Open("c:\\ReportesContabilidad\\Test.xlsx");
                hoja = (_Worksheet)libro.ActiveSheet;
                int _renglon = 1;

                hoja.Cells[_renglon, "A"] = "Folio Fiscal";
                hoja.Cells[_renglon, "B"] = "RFC Emisor";
                hoja.Cells[_renglon, "C"] = "Nombre Emisor";
                hoja.Cells[_renglon, "D"] = "Fecha Timbrado";
                hoja.Cells[_renglon, "E"] = "SubTotal";
                hoja.Cells[_renglon, "F"] = "IVA";
                hoja.Cells[_renglon, "G"] = "Total";
                hoja.Cells[_renglon, "H"] = "Estatus";
                _renglon++;

                foreach (Entity.Operacion.Catfacturasproveedor factura in _facturas)
                {
                    _vigencia = "";
                    if (validarvigencia)
                    {
                        string[] estatus = Base.Clases.Facturacion.ObtenerEstatusCfdi(factura.Emisorrfc, factura.Uuid, factura.Receptorrfc, Convert.ToDouble(factura.Total));
                        _vigencia = estatus[1];
                    }

                    hoja.Cells[_renglon, "A"] = factura.Uuid;
                    hoja.Cells[_renglon, "B"] = factura.Emisorrfc;
                    hoja.Cells[_renglon, "C"] = factura.Emisornombre;
                    hoja.Cells[_renglon, "D"] = factura.Fechatimbrado;
                    hoja.Cells[_renglon, "E"] = factura.Subtotal;
                    hoja.Cells[_renglon, "F"] = factura.Iva;
                    hoja.Cells[_renglon, "G"] = factura.Total;
                    hoja.Cells[_renglon, "H"] = _vigencia;

                    _renglon++;

                }

              

               
                libro.Save();//Para salvar el archivo con la nota 
                libro.Close(null, null, null);
                excel.Workbooks.Close();
                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(hoja);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(libro);
                hoja = null;
                libro = null;
                excel = null;
                GC.Collect();


            }
            catch (Exception)
            {
                throw;
            }


        }
    }


    public class ControladorReporteReporteXmlBonificaciones : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                //http://localhost:2026/Base/Formas/ReporteProduccion.aspx?Nombre=ReporteXmlBonificaciones&usuario=fmontenegro&empresaid=fa764836-bb07-4eb3-9b30-2b69206174c2&fechainicial=01/01/2021&fechafinal=31/12/2021&formato=1&validarvigencia=true
                string usuario = parametros.Get("usuario");
                string empresaid = parametros.Get("empresaid");
                DateTime? fechainicial = DateTime.Parse(parametros.Get("fechainicial"));
                DateTime? fechafinal = DateTime.Parse(parametros.Get("fechafinal"));
                int formato = int.Parse(parametros.Get("formato")); //formato=1 "PDF", formato=2 "Excel"
                bool validarvigencia = bool.Parse(parametros.Get("validarvigencia"));                

                List<Entity.Operacion.Catfacturaselectronica> _facturas = MobileBO.ControlOperacion.TraerCatFacturasElectronicasDescuentosBonificacion(null, null, empresaid, null, null, null, null, fechainicial, fechafinal);

                string _vigencia = string.Empty;

                Application excel = new Application();
                if (excel == null)
                    throw new Exception("Problemas al generar el archivo");


                //ESCRIBIR SOBRE UN ARCHIVO EXISTENTE
                //if (File.Exists("c:\\ReportesContabilidad\\Test.xlsx"))
                //{
                //    File.Delete("c:\\ReportesContabilidad\\Test.xlsx");
                //    File.Create("c:\\ReportesContabilidad\\Test.xlsx");
                //}
                _Workbook libro;
                _Worksheet hoja;
                excel.Visible = false;
                libro = excel.Workbooks.Open("c:\\ReportesContabilidad\\Test.xlsx");
                hoja = (_Worksheet)libro.ActiveSheet;
                hoja.Cells.Clear();
                int _renglon = 1;

                hoja.Cells[_renglon, "A"] = "Folio Fiscal";
                hoja.Cells[_renglon, "B"] = "RFC Emisor";
                hoja.Cells[_renglon, "C"] = "Nombre Emisor";
                hoja.Cells[_renglon, "D"] = "RFC Receptor";
                hoja.Cells[_renglon, "E"] = "Nombre Receptor";
                hoja.Cells[_renglon, "F"] = "Fecha Timbrado";
                hoja.Cells[_renglon, "G"] = "SubTotal";
                hoja.Cells[_renglon, "H"] = "IVA";
                hoja.Cells[_renglon, "I"] = "Total";
                hoja.Cells[_renglon, "J"] = "Estatus";
                _renglon++;

                
                foreach (Entity.Operacion.Catfacturaselectronica factura in _facturas)
                {
                    _vigencia = "";
                    Entity.Configuracion.Catempresa _empresa = MobileBO.ControlConfiguracion.TraerCatempresas(factura.Empresaid);
                    if (validarvigencia)
                    {
                        
                        string[] estatus = Base.Clases.Facturacion.ObtenerEstatusCfdi(_empresa.Rfc, factura.Uuid, factura.Rfc, Convert.ToDouble(factura.Total));
                        _vigencia = estatus[1];
                    }

                    hoja.Cells[_renglon, "A"] = factura.Uuid;
                    hoja.Cells[_renglon, "B"] = _empresa.Rfc;
                    hoja.Cells[_renglon, "C"] = _empresa.Descripcion;
                    hoja.Cells[_renglon, "D"] = factura.Rfc;
                    hoja.Cells[_renglon, "E"] = factura.Nombrereceptor;                    
                    hoja.Cells[_renglon, "F"] = factura.Fechatimbrado;
                    hoja.Cells[_renglon, "G"] = factura.Subtotal;
                    hoja.Cells[_renglon, "H"] = factura.Iva;
                    hoja.Cells[_renglon, "I"] = factura.Total;
                    hoja.Cells[_renglon, "J"] = _vigencia;

                    _renglon++;

                }




                libro.Save();//Para salvar el archivo con la nota 
                libro.Close(null, null, null);
                excel.Workbooks.Close();
                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(hoja);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(libro);
                hoja = null;
                libro = null;
                excel = null;
                GC.Collect();


            }
            catch (Exception)
            {
                throw;
            }


        }
    }

}