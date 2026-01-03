using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using BalorFinanciera.Base.Clases;
using System.Diagnostics;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReportesPLD : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<object> GenerarReporteClientes(string fecha)
        {
            string estatus = string.Empty;
            try
            {
                
                DateTime fechaEntrega = DateTime.Parse(fecha);
                string nomArchivo = "detalleClientesPLD.xls";
                //string nomArchivo = "detalleClientesPLD_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
                string pathSave = HttpContext.Current.Server.MapPath("~") + "\\ArchivosExcel\\" + nomArchivo;
                string pathImg = HttpContext.Current.Server.MapPath("~") + "\\" + "Base\\img\\AnalisisBalor.png";
                //string pathSave = "C:\\inetpub\\wwwroot\\Tativo\\ArchivosCC\\" + nomArchivo;

                estatus = "Tratando de usar el impersonator..";
                //using (new Impersonator("alfonsom", "http://localhost/balor/", "PRIMOS"))
                using (new Impersonator("Administrador", "http://localhost/balor/", "2y[cjoQBXz=2E6p"))
                {
                    estatus = "Impersonatr creado, tratando de crear objet de excel...";
                    //Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                    Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                    if (excel == null)
                        throw new Exception("Problemas al generar el archivo");

                    estatus = "Objeto de excel creado...";
                    Excel.Workbook libro;
                    Excel.Worksheet hoja;
                    object misValue = System.Reflection.Missing.Value;
                    libro = excel.Workbooks.Add(misValue);
                    hoja = (Excel.Worksheet)libro.Worksheets.Item[1];

                    hoja.Shapes.AddPicture(pathImg, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 2, 2, 69, 47);

                    Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas("FA764836-BB07-4EB3-9B30-2B69206174C2");

                    hoja.Cells[1, "A"] = empresa.Descripcion.ToUpper();
                    hoja.Range["A1", "AB1"].Merge();
                    hoja.Range["A1", "AB1"].Font.Bold = true;
                    hoja.Range["A1", "AB1"].Font.Size = 14;
                    hoja.Range["A1", "AB1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    hoja.Cells[2, "A"] = "Reporte de Operaciones de pago o abono";
                    hoja.Range["A2", "AB2"].Merge();
                    hoja.Range["A2", "AB2"].Font.Bold = true;
                    hoja.Range["A2", "AB2"].Font.Size = 10;
                    hoja.Range["A2", "AB2"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    hoja.Cells[3, "C"] = "Del 01 de Enero del " + fechaEntrega.Year.ToString() + " al " + fechaEntrega.ToString("dd 'de' MMMM 'del' yyyy");
                    hoja.Range["A3", "AB3"].Merge();
                    hoja.Range["A3", "AB3"].Font.Bold = true;
                    hoja.Range["A3", "AB3"].Font.Size = 10;
                    hoja.Range["A3", "AB3"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                    hoja.Range["A4", "AB4"].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);

                    hoja.Cells[4, "A"] = "Codigo";
                    hoja.Cells[4, "B"] = "ApellidoPaterno";
                    hoja.Cells[4, "C"] = "ApellidoMaterno";
                    hoja.Cells[4, "D"] = "Nombre";
                    hoja.Cells[4, "E"] = "Genero";
                    hoja.Cells[4, "F"] = "FechaNacimiento";
                    hoja.Cells[4, "G"] = "Pais";
                    hoja.Cells[4, "H"] = "Estado";
                    hoja.Cells[4, "I"] = "Nacionalidad";
                    hoja.Cells[4, "J"] = "RFC";
                    hoja.Cells[4, "K"] = "ActividadComercial";
                    hoja.Cells[4, "L"] = "Domicilio";
                    hoja.Cells[4, "M"] = "Telefono";
                    hoja.Cells[4, "N"] = "PerfilTransaccional";
                    hoja.Cells[4, "O"] = "Estatus_PEP";
                    hoja.Cells[4, "P"] = "EstatusListaNegra";
                    hoja.Cells[4, "Q"] = "GradoRiesgo";
                    hoja.Cells[4, "R"] = "Servicio";
                    hoja.Cells[4, "S"] = "ImporteLimite";
                    hoja.Cells[4, "T"] = "Representante";
                    hoja.Cells[4, "U"] = "Representante2";
                    hoja.Cells[4, "V"] = "Representante3";
                    hoja.Cells[4, "W"] = "NombreAval";
                    hoja.Cells[4, "X"] = "NombreAval2";
                    hoja.Cells[4, "Y"] = "NombreAval3";
                    hoja.Cells[4, "Z"] = "NombreAval4";
                    hoja.Cells[4, "AA"] = "NombreAval5";
                    hoja.Cells[4, "AB"] = "NombreAval6";


                    DataSet DatosReporte = MobileBO.ControlAnalisis.TraerReporteClientesPLD(fechaEntrega);
                    int i = 5;
                    foreach (DataRow r in DatosReporte.Tables[0].Rows)
                    {
                        hoja.Cells[i, "A"] = r["Codigo"].ToString();
                        hoja.Cells[i, "B"] = r["ApellidoPaterno"].ToString();
                        hoja.Cells[i, "C"] = r["ApellidoMaterno"].ToString();
                        hoja.Cells[i, "D"] = r["Nombre"].ToString();
                        hoja.Cells[i, "E"] = r["Genero"].ToString();
                        hoja.Cells[i, "F"] = r["FechaNacimiento"].ToString();
                        hoja.Cells[i, "G"] = r["Pais"].ToString();
                        hoja.Cells[i, "H"] = r["Estado"].ToString();
                        hoja.Cells[i, "I"] = r["Nacionalidad"].ToString();
                        hoja.Cells[i, "J"] = r["RFC"].ToString();
                        hoja.Cells[i, "K"] = r["ActividadComercial"].ToString();
                        hoja.Cells[i, "L"] = r["Domicilio"].ToString();
                        hoja.Cells[i, "M"] = r["Telefono"].ToString();
                        hoja.Cells[i, "N"] = r["PerfilTransaccional"].ToString();
                        hoja.Cells[i, "O"] = r["Estatus_PEP"].ToString();
                        hoja.Cells[i, "P"] = r["EstatusListaNegra"].ToString();
                        hoja.Cells[i, "Q"] = r["GradoRiesgo"].ToString();
                        hoja.Cells[i, "R"] = r["Servicio"].ToString();
                        hoja.Cells[i, "S"] = r["ImporteLimite"].ToString();
                        hoja.Range["S" + i.ToString()].NumberFormat = "###,##0.00";
                        hoja.Cells[i, "T"] = r["Representante"].ToString();
                        hoja.Cells[i, "U"] = r["Representante2"].ToString();
                        hoja.Cells[i, "V"] = r["Representante3"].ToString();
                        hoja.Cells[i, "W"] = r["NombreAval"].ToString();
                        hoja.Cells[i, "X"] = r["NombreAval2"].ToString();
                        hoja.Cells[i, "Y"] = r["NombreAval3"].ToString();
                        hoja.Cells[i, "Z"] = r["NombreAval4"].ToString();
                        hoja.Cells[i, "AA"] = r["NombreAval5"].ToString();
                        hoja.Cells[i, "AB"] = r["NombreAval6"].ToString();
                        i++;
                    }


                    excel.DisplayAlerts = false;
                    excel.Columns.AutoFit();

                    try
                    {
                        estatus = "tratando de grabar el archivo";
                        if (System.IO.File.Exists(pathSave))
                        {
                            System.IO.File.Delete(pathSave);
                        }
                        libro.Saved = true;
                        libro.SaveCopyAs(pathSave);
                        
                        //libro.SaveAs(pathSave, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    catch (Exception eg)
                    {
                        throw new Exception("Error al guardar X: " + eg.Message + " Ruta: " + pathSave + " (" + estatus + ")");
                    }


                    //libro.SaveAs(pathSave, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);

                    //libro.Close(true, misValue, misValue);
                    estatus = "tratando de cerrar la aplicación de excel...";
                    excel.Quit();
                    excel.Application.Workbooks.Close();

                    estatus = "tratando de liberar el objeto com...";
                    Marshal.ReleaseComObject(hoja);
                    Marshal.ReleaseComObject(libro);
                    Marshal.ReleaseComObject(excel);

                    // Matriz de procesos
                    estatus = "tratando de liberar los procesos de excel...";
                    Process[] processes = Process.GetProcesses();
                    // Recorremos los procesos en ejecución
                    foreach (Process p in processes)
                    {
                        if (p.ProcessName.ToUpper().Contains("EXCEL"))
                            p.Close();
                    }

                }

                return Entity.Response<object>.CrearResponse<object>(true, new { Ruta = pathSave, NomArchivo = "http://192.168.10.238/balor/ArchivosExcel/" + nomArchivo });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message + " ("  + estatus + ")");
            }
        }

    }
    public class ControladorReporteClientesYOperacionesPLD : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            DateTime Fecha = DateTime.Parse(parametros.Get("Fecha"));
            int TipoDeReporte = int.Parse(parametros.Get("TipoDeReporte"));

            //Solo aplica para la empresa de balor
            DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("FA764836-BB07-4EB3-9B30-2B69206174C2");
            empresa.Tables[0].Rows[0]["FechaEscritura"] = Fecha;


            if (TipoDeReporte == 1)
            {
                DataSet DatosReporte = MobileBO.ControlAnalisis.TraerReporteClientesPLD(Fecha);

                DataSet ds = new DataSet();
                ds.Tables.Add(empresa.Tables[0].Copy());

                DataTable tblDatos = DatosReporte.Tables[0].Copy();
                tblDatos.TableName = "DatosReporte";
                ds.Tables.Add(tblDatos);
                try
                {
                    base.NombreReporte = (int.Parse(parametros.Get("Formato")) == 3 ? "ReporteClientesPLD" : "ReporteClientesPLDExcel");
                    base.FormatoReporte = int.Parse(parametros.Get("Formato"));
                    base.RutaReporte = "Contabilidad\\Reportes";
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteClientesPLD.xml", System.Data.XmlWriteMode.WriteSchema);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else {
                DataSet DatosReporte = MobileBO.ControlOperacion.TraerReporteOperacionesPLD(Fecha);

                DataSet ds = new DataSet();
                ds.Tables.Add(empresa.Tables[0].Copy());

                DataTable tblDatos = DatosReporte.Tables[0].Copy();
                tblDatos.TableName = "DatosReporte";
                ds.Tables.Add(tblDatos);
                try
                {
                    base.NombreReporte = (int.Parse(parametros.Get("Formato")) == 3 ? "ReporteOperacionesPLD" : "ReporteOperacionesPLDExcel");
                    base.FormatoReporte = int.Parse(parametros.Get("Formato"));
                    base.RutaReporte = "Contabilidad\\Reportes";
                    base.DataSource = ds;
                    base.DataSource.WriteXml("c:\\Reportes\\ReporteOperacionesPLD.xml", System.Data.XmlWriteMode.WriteSchema);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion
    }
}