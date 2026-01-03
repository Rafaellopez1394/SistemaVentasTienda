using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.Services;

using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using BalorFinanciera.Base.Clases;
using System.Reflection;


namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteEfectoCambiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<string> GenerarReporteEfectoCambiario(int TipoImpresion, string EmpresaId, string Fecha)
        {//GenerarReporteClientesInactivos
            try
            {
                DateTime dFecha = DateTime.Parse(Fecha);

                string nomArchivo = "ReporteEfectoCambiario.xlsx";
                string sNombreEmpresa = "";
                string sDescripcion = string.Format( "ANALISIS EFECTO CAMBIARIO {0}",dFecha.Year.ToString() );

                int iFila = 2;
                int iCodEmpresa = 0;

                DataSet dsempr = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(EmpresaId);
                dsempr.Tables[0].TableName = "DatosEmpresa";
                iCodEmpresa = int.Parse(dsempr.Tables[0].Rows[0]["Empresa"].ToString());
                sNombreEmpresa = dsempr.Tables[0].Rows[0]["Descripcion"].ToString();

                //DataSet ds = MobileBO.ControlContabilidad.ReporteEfectoCambiarioDS(iCodEmpresa, dFecha);
                //ds.Tables[0].TableName = "DatosReporte";

                List<Entity.Contabilidad.Ctlregistroefectocambiario> _EfectoCambiario = MobileBO.ControlContabilidad.ReporteEfectoCambiario(iCodEmpresa, dFecha);

                #region GenerarEXCEL
                //NOMBRE Y RUTA DEL ARCHIVO

                string path = HttpContext.Current.Server.MapPath("~") + "\\ArchivosExcel\\"; //PRODUCCION
                //string path = HttpContext.Current.Server.MapPath("~") + "ArchivosExcel\\"; //DESARROLLO
                string pathSave = path + nomArchivo;

               using (new Impersonator("Administrador", "http://localhost/balor/", "2y[cjoQBXz=2E6p"))
                {

                    //INICIALIZAR EL ARCHIVO
                    Excel.Application excel = new Excel.Application();
                    if (excel == null)
                        throw new Exception("Problemas al generar el archivo");

                    Excel.Workbook libro;
                    Excel.Worksheet hoja;
                    object misValue = System.Reflection.Missing.Value;
                    libro = excel.Workbooks.Add(misValue);
                    hoja = (Excel.Worksheet)libro.Worksheets.Item[1];

                    //CTE INACTIVO
                    GenerarExcel_Titulo(ref hoja, ref iFila, 1, sNombreEmpresa, sDescripcion);
                    GenerarExcelInformacionCuentas(_EfectoCambiario, ref hoja, ref iFila);

                    excel.DisplayAlerts = false;
                    excel.Columns.AutoFit();

                    try
                    { libro.SaveCopyAs(pathSave); }
                    catch (Exception eg)
                    { throw new Exception("Error al guardar X: " + eg.Message + " Ruta: " + pathSave); }

                    excel.Quit();
                    excel.Application.Workbooks.Close();

                    Marshal.ReleaseComObject(hoja);
                    Marshal.ReleaseComObject(libro);
                    Marshal.ReleaseComObject(excel);

                    Process[] processes = Process.GetProcesses();
                    // Recorremos los procesos en ejecución
                    foreach (Process p in processes)
                    {
                        if (p.ProcessName.ToUpper().Contains("EXCEL"))
                            p.Close();
                    }
                }
                #endregion

                return Entity.Response<string>.CrearResponse<string>(true, "http://192.168.10.238/balor/ArchivosExcel/" + nomArchivo);
            }
            catch (Exception ex)
            {
                return Entity.Response<string>.CrearResponseVacio<string>(false, ex.Message);
            }
        }

        private static void GenerarExcel_Titulo(ref Excel.Worksheet hoja, ref int iFila, int iTipoTitulo, string sNombreEmpresa, string sDescripcion)
        {
            switch (iTipoTitulo)
            {
                case 1://

                    //TITULO
                    hoja.Cells[iFila, "B"] = sNombreEmpresa;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Merge();
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Bold = true;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Size = 11;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    iFila++;

                    hoja.Cells[iFila, "B"] = sDescripcion;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Merge();
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Bold = true;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Size = 11;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    iFila+= 5;

                    hoja.Cells[iFila, "B"] = "MES";
                    hoja.Cells[iFila, "C"] = " ";
                    hoja.Cells[iFila, "D"] = "CUENTA\nCONTABLE";
                    hoja.Cells[iFila, "E"] = "CUENTA";
                    hoja.Cells[iFila, "F"] = "SALDO\nANTERIOR";
                    hoja.Cells[iFila, "G"] = "SALDO\nFINAL";
                    hoja.Cells[iFila, "H"] = "UTILIDAD\nCAMBIARIA";
                    hoja.Cells[iFila, "I"] = "PERDIDA\nCAMBIARIA";
                    hoja.Cells[iFila, "J"] = "EFECTO\nCAMBIARIO";
                    hoja.Cells[iFila, "K"] = "UTILIDAD\nCAMBIARIA\nACUMULADA";
                    hoja.Cells[iFila, "L"] = "PERDIDA\nCAMBIARIA\nACUMULADA";
                    hoja.Cells[iFila, "M"] = "EFECTO\nCAMBIARIO\nACUMULADO";
                    hoja.Cells[iFila, "N"] = "TIPO DE\nCAMBIO MES\nANTERIOR";
                    hoja.Cells[iFila, "O"] = "TIPO DE CAMBIO\nFINAL DEL\nMES";
                    hoja.Cells[iFila, "P"] = "DIF";
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Interior.Color = Excel.XlRgbColor.rgbDeepPink; //System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Pink);
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Bold = true;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Size = 11;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    iFila ++;
                    break;
                default:
                    break;
            }
        }

        private static void GenerarExcelInformacionCuentas(List<Entity.Contabilidad.Ctlregistroefectocambiario> _efectoCambiario, ref Excel.Worksheet hoja, ref int iFila)
        {

            var gpoMesEfectoCambiario = from Mes in _efectoCambiario
                                        group Mes by new
                                        {
                                            NumMes = Mes.Mes
                                        };

            foreach (var mes in gpoMesEfectoCambiario)
            {

                var gpoEtapas = from a in _efectoCambiario
                                where a.Mes.ToString().Contains(mes.Key.NumMes.ToString())
                                select new
                                {
                                    ID = a.Id,
                                    Mes = a.Mes,
                                    NombreMes = a.Nombremes,
                                    Naturaleza = a.Naturaleza,
                                    CuentaContable = a.CuentaContable,
                                    Cuenta = a.Descripcion,
                                    SaldoAnt = a.Saldoant,
                                    SaldoFinal = a.Saldofinal,
                                    UtilidadCambiaria = a.Utilidadcambiaria,
                                    PerdidaCambiaria = a.Perdidacambiaria,
                                    EfectoCambiario = a.EfectoCambiario,
                                    UtilidadCambiariaAcumulada = a.UtilidadCambiariaAcumulada,
                                    PerdidaCambiariaAcumulada = a.PerdidaCambiariaAcumulada,
                                    EfectoCambiarioAcumulado = a.EfectoCambiarioAcumulado,
                                    TipoCambioMesAnterior = a.Tipocambiomesant,
                                    TipoCambioFinMes = a.Tipocambiofinmes,
                                    Diferencia = a.DiferenciaTipoCambio
                                };

                GenerarRenglonesExcel(gpoEtapas.AsEnumerable(), ref hoja, ref iFila);


            }
        }
        private static void GenerarRenglonesExcel(IEnumerable<dynamic> datos, ref Excel.Worksheet hoja, ref int iFila)
        {
            int iFilaRango =  ( (iFila)-1 )+ (datos.Count());
            int iFilaNaturaleza = 0;
            bool bPrimerElemento = true;
            bool bCambiaNaturaleza = false;
            string sNaturaleza = "";

            decimal TotalSaldoAnt = 0.0M;
            decimal TotalSaldoFinal = 0.0M;
            decimal TotalUtilidadCambiaria = 0.0M;
            decimal TotalPerdidaCambiaria = 0.0M;
            decimal TotalEfectoCambiario = 0.0M;
            decimal TotalUtilidadCambiariaAcumulada = 0.0M;
            decimal TotalPerdidaCambiariaAcumulada = 0.0M;
            decimal TotalEfectoCambiarioAcumulada = 0.0M;

            var gpoNaturaleza = from nat in datos
                                group nat by new
                                {
                                    Naturaleza = nat.Naturaleza
                                };

            foreach (var n in gpoNaturaleza)
            {
                
                if (n.Key.Naturaleza != sNaturaleza)
                {
                    sNaturaleza = n.Key.Naturaleza.ToString().Trim();
                    bCambiaNaturaleza = true;
                }

                var registros = from a in datos
                                where a.Naturaleza.ToString().Contains(n.Key.Naturaleza.ToString())
                                select new
                                {
                                    Mes = a.Mes,
                                    NombreMes = a.NombreMes,
                                    Naturaleza = a.Naturaleza,
                                    CuentaContable = a.CuentaContable,
                                    Cuenta = a.Cuenta,
                                    SaldoAnt = a.SaldoAnt,
                                    SaldoFinal = a.SaldoFinal,
                                    UtilidadCambiaria = a.UtilidadCambiaria,
                                    PerdidaCambiaria = a.PerdidaCambiaria,
                                    EfectoCambiario = a.EfectoCambiario,
                                    UtilidadCambiariaAcumulada = a.UtilidadCambiariaAcumulada,
                                    PerdidaCambiariaAcumulada = a.PerdidaCambiariaAcumulada,
                                    EfectoCambiarioAcumulado = a.EfectoCambiarioAcumulado,
                                    TipoCambioMesAnterior = a.TipoCambioMesAnterior,
                                    TipoCambioFinMes = a.TipoCambioFinMes,
                                    Diferencia = a.Diferencia
                                };


                iFilaNaturaleza = ((iFila) - 1) + (registros.Count());

                foreach (var r in registros)
                {
                    
                    if (bPrimerElemento == true)
                    {
                        hoja.Cells[iFila, "B"] = r.NombreMes;

                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].Merge();
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].Interior.Color = Excel.XlRgbColor.rgbDodgerBlue;
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].Font.Bold = true;
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].Font.Size = 11;
                        hoja.Range["B" + iFila.ToString() + ":B" + iFilaRango.ToString()].Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        bPrimerElemento = false;

                        hoja.Cells[iFila, "N"] = r.TipoCambioMesAnterior;
                        hoja.Cells[iFila, "O"] = r.TipoCambioFinMes;
                        hoja.Cells[iFila, "P"] = r.Diferencia;
                    }

                    if(bCambiaNaturaleza == true)
                    {
                        hoja.Cells[iFila, "C"] = r.Naturaleza;
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].Merge();
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].Interior.Color = (r.Naturaleza == "ACTIVO" ? Excel.XlRgbColor.rgbGreenYellow: Excel.XlRgbColor.rgbRed);
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].Font.Bold = true;
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].Font.Size = 11;
                        hoja.Range["C" + iFila.ToString() + ":C" + iFilaNaturaleza.ToString()].Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        bCambiaNaturaleza = false;
                    }

                    hoja.Cells[iFila, "D"] = (r.CuentaContable.Trim().Length > 4 ? " " : r.CuentaContable);
                    hoja.Cells[iFila, "E"] = GenerarNombreCuenta(r.CuentaContable);
                    hoja.Cells[iFila, "F"] = r.SaldoAnt.ToString("###,##0.00");
                    hoja.Cells[iFila, "G"] = r.SaldoFinal.ToString("###,##0.00");
                    hoja.Cells[iFila, "H"] = r.UtilidadCambiaria.ToString("###,##0.00");
                    hoja.Cells[iFila, "I"] = r.PerdidaCambiaria.ToString("###,##0.00");
                    hoja.Cells[iFila, "J"] = r.EfectoCambiario.ToString("###,##0.00");
                    hoja.Cells[iFila, "J"].Font.Color = r.EfectoCambiario < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    hoja.Cells[iFila, "K"] = r.UtilidadCambiariaAcumulada.ToString("###,##0.00");
                    hoja.Cells[iFila, "L"] = r.PerdidaCambiariaAcumulada.ToString("###,##0.00");
                    hoja.Cells[iFila, "M"] = r.EfectoCambiarioAcumulado.ToString("###,##0.00");
                    hoja.Cells[iFila, "M"].Font.Color = r.EfectoCambiarioAcumulado < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Bold = true;
                    hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    iFila++;
                };
            }

            var SumatoriaActivo = from d in datos
                                  where d.Naturaleza.ToString().Contains("ACTIVO") && d.CuentaContable.Length == 4
                                  group d by d.Naturaleza into gActiv
                                  select new
                                  {
                                      TotalSaldoAnt = gActiv.Sum(x => (decimal)x.SaldoAnt),
                                      TotalSaldoFinal = gActiv.Sum(x => (decimal)x.SaldoFinal),
                                      TotalUtilidadCambiaria = gActiv.Sum(x => (decimal)x.UtilidadCambiaria),
                                      TotalPerdidaCambiaria = gActiv.Sum(x => (decimal)x.PerdidaCambiaria),
                                      TotalEfectoCambiario = gActiv.Sum(x => (decimal)x.EfectoCambiario),
                                      TotalUtilidadCambiariaAcumulada = gActiv.Sum(x => (decimal)x.UtilidadCambiariaAcumulada),
                                      TotalPerdidaCambiariaAcumulada = gActiv.Sum(x => (decimal)x.PerdidaCambiariaAcumulada),
                                      TotalEfectoCambiarioAcumulado = gActiv.Sum(x => (decimal)x.EfectoCambiarioAcumulado),
                                      TotalDiferencia = gActiv.Sum(x => (decimal)x.Diferencia)
                                  };

            var SumatoriaPasivo = from a in datos
                                  where a.Naturaleza.ToString().Contains("PASIVO") && a.CuentaContable.Length == 4
                                  group a by a.Naturaleza into gPasiv
                                  select new
                                  {
                                      TotalSaldoAnt = gPasiv.Sum(x => (decimal)x.SaldoAnt),
                                      TotalSaldoFinal = gPasiv.Sum(x => (decimal)x.SaldoFinal),
                                      TotalUtilidadCambiaria = gPasiv.Sum(x => (decimal)x.UtilidadCambiaria),
                                      TotalPerdidaCambiaria = gPasiv.Sum(x => (decimal)x.PerdidaCambiaria),
                                      TotalEfectoCambiario = gPasiv.Sum(x => (decimal)x.EfectoCambiario),
                                      TotalUtilidadCambiariaAcumulada = gPasiv.Sum(x => (decimal)x.UtilidadCambiariaAcumulada),
                                      TotalPerdidaCambiariaAcumulada = gPasiv.Sum(x => (decimal)x.PerdidaCambiariaAcumulada),
                                      TotalEfectoCambiarioAcumulado = gPasiv.Sum(x => (decimal)x.EfectoCambiarioAcumulado),
                                      TotalDiferencia = gPasiv.Sum(x => (decimal)x.Diferencia)
                                  };

            TotalSaldoAnt = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalSaldoAnt).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalSaldoAnt).ToString());
            TotalSaldoFinal = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalSaldoFinal).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalSaldoFinal).ToString());

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiaria).ToString()) < 0)
            {
                TotalUtilidadCambiaria = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiaria).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalUtilidadCambiaria).ToString());
            }
            else
            {
                TotalUtilidadCambiaria = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiaria).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalUtilidadCambiaria).ToString());
            }

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiaria).ToString()) < 0)
            {
                TotalPerdidaCambiaria = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiaria).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalPerdidaCambiaria).ToString());
            }
            else
            {
                TotalPerdidaCambiaria = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiaria).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalPerdidaCambiaria).ToString());
            }

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiario).ToString()) < 0)
            {
                TotalEfectoCambiario = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiario).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalEfectoCambiario).ToString());
            }else
            {
                TotalEfectoCambiario = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiario).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalEfectoCambiario).ToString());
            }

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiariaAcumulada).ToString()) < 0)
            {
                TotalUtilidadCambiariaAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiariaAcumulada).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalUtilidadCambiariaAcumulada).ToString());
            }
            else
            {
                TotalUtilidadCambiariaAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalUtilidadCambiariaAcumulada).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalUtilidadCambiariaAcumulada).ToString());
            }

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiariaAcumulada).ToString()) < 0)
            {
                TotalPerdidaCambiariaAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiariaAcumulada).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalPerdidaCambiariaAcumulada).ToString());
            }else
            {
                TotalPerdidaCambiariaAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalPerdidaCambiariaAcumulada).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalPerdidaCambiariaAcumulada).ToString());
            }

            if(decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiarioAcumulado).ToString()) < 0)
            {
                TotalEfectoCambiarioAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiarioAcumulado).ToString()) + decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalEfectoCambiarioAcumulado).ToString());
            }
            else
            {
                TotalEfectoCambiarioAcumulada = decimal.Parse(SumatoriaActivo.Sum(x => x.TotalEfectoCambiarioAcumulado).ToString()) - decimal.Parse(SumatoriaPasivo.Sum(y => y.TotalEfectoCambiarioAcumulado).ToString());
            }

            hoja.Cells[iFila, "C"] = "BALANCE ( A - P )";
            hoja.Range["C" + iFila.ToString() + ":E" + iFila.ToString()].Merge();
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Interior.Color = Excel.XlRgbColor.rgbYellow;
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Bold = true;
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Font.Size = 11;
            hoja.Range["B" + iFila.ToString() + ":P" + iFila.ToString()].Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            hoja.Cells[iFila, "F"] = TotalSaldoAnt.ToString("###,##0.00");
            hoja.Cells[iFila, "F"].Font.Color = TotalSaldoAnt < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            hoja.Cells[iFila, "G"] = TotalSaldoFinal.ToString("###,##0.00");
            hoja.Cells[iFila, "G"].Font.Color = TotalSaldoFinal < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            hoja.Cells[iFila, "H"] = TotalUtilidadCambiaria.ToString("###,##0.00");
            hoja.Cells[iFila, "I"] = TotalPerdidaCambiaria.ToString("###,##0.00");
            hoja.Cells[iFila, "J"] = TotalEfectoCambiario.ToString("###,##0.00");
            hoja.Cells[iFila, "J"].Font.Color = TotalEfectoCambiario < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            hoja.Cells[iFila, "K"] = TotalUtilidadCambiariaAcumulada.ToString("###,##0.00");
            hoja.Cells[iFila, "L"] = TotalPerdidaCambiariaAcumulada.ToString("###,##0.00");
            hoja.Cells[iFila, "M"] = TotalEfectoCambiarioAcumulada.ToString("###,##0.00");
            hoja.Cells[iFila, "M"].Font.Color = TotalEfectoCambiarioAcumulada < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            iFila++;

        }

        private static string GenerarNombreCuenta(string sCuenta)
        {
            string sNombre = "";

            switch (sCuenta.Trim())
            {
                case "1102":
                    sNombre = "BANCOS";
                    break;
                case "1102000100020001":
                    sNombre = "BANAMEX";
                    break;
                case "1102000300020001":
                    sNombre = "ANDORRA";
                    break;
                case "1102000600020001":
                    sNombre = "BANCOMER";
                    break;
                case "1105":
                    sNombre = "DOCS X COBRAR";
                    break;
                case "1106":
                    sNombre = "DEUDORES DIV.";
                    break;
                case "1400":
                    sNombre = "ANTICIPO A PROVEEDORES";
                    break;
                case "2000":
                    sNombre = "PROVEEDORES";
                    break;
                case "2002":
                    sNombre = "ANTICIPO DE CLIENTES";
                    break;
                case "2301":
                    sNombre = "CREDITOS BANCARIOS";
                    break;
                default:
                    sNombre = "";
                    break;
            }

            return sNombre;
        }
    }

    public class ControladorReporteEfectoCambiario : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            string sFecha = parametros.Get("fechaFinal").ToString();
            string sFormato = parametros.Get("Formato");
            string sTipoImpresion = parametros.Get("TipoImpresion");
            int iCodEmpresa = 0;


            //DateTime FechaInicio = DateTime.Parse(parametros.Get("fechaInicial"));
            DateTime FechaFinal = DateTime.Parse(parametros.Get("fechaFinal"));

            int formato = int.Parse(sTipoImpresion);

            try
            {
                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));

                DataTable dtReporte = new DataTable("DatosEmpresa");
                dtReporte.Columns.Add("NombreEmpresa", typeof(string));
                dtReporte.Columns.Add("Empresa", typeof(int));
                dtReporte.Columns.Add("NombreBalor", typeof(string));
                dtReporte.Columns.Add("NombreFactur", typeof(string));
                dtReporte.Columns.Add("FechaInicio", typeof(DateTime));
                dtReporte.Columns.Add("FechaFin", typeof(DateTime));
                dtReporte.Columns.Add("Logo", typeof(byte[]));
                dtReporte.Columns.Add("LogoBalor", typeof(byte[]));
                dtReporte.Columns.Add("LogoFactur", typeof(byte[]));
                dtReporte.Columns.Add("Rfc", typeof(string));
                dtReporte.Columns.Add("DomicilioCompleto", typeof(string));

                DataRow r = dtReporte.NewRow();
                if (dsEmpresa.Tables[0].Rows.Count > 0)
                {
                    r["NombreEmpresa"] = dsEmpresa.Tables[0].Rows[0]["Descripcion"].ToString();
                    r["Logo"] = (byte[])dsEmpresa.Tables[0].Rows[0]["Logo"];
                    r["Rfc"] = dsEmpresa.Tables[0].Rows[0]["Rfc"].ToString();
                    r["DomicilioCompleto"] = dsEmpresa.Tables[0].Rows[0]["DomicilioCompleto"].ToString();
                    r["Empresa"] = int.Parse(dsEmpresa.Tables[0].Rows[0]["Empresa"].ToString());
                }
                else
                {
                    DataSet dsEmpresaBalor = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("FA764836-BB07-4EB3-9B30-2B69206174C2");
                    DataSet dsEmpresaFactur = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("A7D3E5A4-6508-483B-8A3D-0E379FF06755");
                    r["NombreEmpresa"] = "CONSOLIDADO";
                    //r["Descripcion"] = "CONSOLIDADO";
                    r["Empresa"] = 0;
                    r["Rfc"] = "FAC060511TMA";
                    r["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";
                    r["NombreBalor"] = dsEmpresaBalor.Tables[0].Rows[0]["Descripcion"].ToString();
                    r["NombreFactur"] = dsEmpresaFactur.Tables[0].Rows[0]["Descripcion"].ToString();
                    r["LogoBalor"] = dsEmpresaBalor.Tables[0].Rows[0]["Logo"];
                    r["LogoFactur"] = dsEmpresaFactur.Tables[0].Rows[0]["Logo"];
                }
                dtReporte.Rows.Add(r);
                dsEmpresa.Tables.Add(dtReporte);
                iCodEmpresa = int.Parse(dsEmpresa.Tables[1].Rows[0]["Empresa"].ToString());

                DataSet ds = MobileBO.ControlContabilidad.ReporteEfectoCambiarioDS(iCodEmpresa, FechaFinal);
                ds.Tables[0].TableName = "DatosReporte";

                //COPIAMOS DATOS DE EMPRESA PARA ENCABEZADO
                ds.Tables.Add(dsEmpresa.Tables[1].Copy());

                DataTable tDatos = new DataTable();
                //tDatos.Columns.Add("FechaInicial", typeof(DateTime));
                tDatos.Columns.Add("Fecha", typeof(DateTime));
                tDatos.Rows.Add(tDatos.NewRow());
                //tDatos.Rows[0]["FechaInicial"] = FechaInicio;
                tDatos.Rows[0]["Fecha"] = FechaFinal;
                tDatos.TableName = "Parametros";
                ds.Tables.Add(tDatos);

                base.NombreReporte = "ReporteEfectoCambiario";
                base.FormatoReporte = formato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteEfectoCambiario.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte
    }
}