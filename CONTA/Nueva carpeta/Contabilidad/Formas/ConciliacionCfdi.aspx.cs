using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Entity.Contabilidad;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;


namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ConciliacionCfdi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEmpresas();
                LlenarMeses();
                LlenarAnios();
                GridViewCfdi.Visible = false;
                GridViewCfdi1.Visible = false;
                GridViewCfdi2.Visible = false;

                // Valor por defecto y título inicial
                if (string.IsNullOrEmpty(rblTipo.SelectedValue))
                    rblTipo.SelectedValue = "Egresos";
                lblImportarTitulo.Text = $"Importar información {rblTipo.SelectedValue}";
            }
            // SIEMPRE rellenar este campo, incluso en postbacks
            var lista = Session["ListaDatosPDF"] as List<DatoPDF> ?? new List<DatoPDF>();
            var json = new JavaScriptSerializer().Serialize(lista);
            HiddenJsonPDFLista.Value = json;
        }


        protected void MostrarMensajeJS(string titulo, string mensaje, string tipo)
        {
            string script = $@"
        setTimeout(function() {{
            if (typeof Swal !== 'undefined') {{
                Swal.fire({{
                    title: '{titulo}',
                    text: '{mensaje}',
                    icon: '{tipo}',
                    confirmButtonText: 'Aceptar'
                }});
            }} else {{
                if (typeof $('#divmsg').mostrarMensaje === 'function') {{
                    $('#divmsg').mostrarMensaje('{titulo}', '{mensaje}', '{tipo}');
                }}
            }}
        }}, 500);";

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);
        }

        private void CargarEmpresas()
        {
            var listaEmpresas = ObtenerEmpresas();
            Empresas.DataSource = listaEmpresas;
            Empresas.DataTextField = "Nombre";
            Empresas.DataValueField = "EmpresaID";
            Empresas.DataBind();
            for (int i = 0; i < listaEmpresas.Count; i++)
            {
                Empresas.Items[i].Attributes["data-rfc"] = listaEmpresas[i].Rfc;
            }
            Empresas.Items.Insert(0, new ListItem("-- Seleccione una empresa --", ""));
            Session["Empresas"] = listaEmpresas;
        }
        private void LlenarMeses()
        {
            var meses = CultureInfo.GetCultureInfo("es-MX")
                                   .DateTimeFormat
                                   .MonthNames
                                   .Select((nombre, index) => new { nombre, index })
                                   .Where(m => !string.IsNullOrWhiteSpace(m.nombre)) // Quita el último vacío
                                   .Select(m => new ListItem(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.nombre), (m.index + 1).ToString()))
                                   .ToList();

            mes.DataSource = meses;
            mes.DataTextField = "Text";
            mes.DataValueField = "Value";
            mes.DataBind();
            mes.Items.Insert(0, new ListItem("-- Seleccione mes --", ""));
        }
        private void LlenarAnios()
        {
            int añoActual = DateTime.Now.Year;
            int añoInicio = añoActual - 15;

            var años = Enumerable.Range(añoInicio, añoActual - añoInicio + 1)
                                  .Reverse()
                                  .Select(a => new ListItem(a.ToString(), a.ToString()))
                                  .ToList();

            año.DataSource = años;
            año.DataTextField = "Text";
            año.DataValueField = "Value";
            año.DataBind();

            año.Items.Insert(0, new ListItem("-- Seleccione año --", ""));

            año.SelectedValue = añoActual.ToString();
        }

        [Serializable]
        public class EMPRESAS
        {
            public string EmpresaID { get; set; }
            public string Nombre { get; set; }
            public string Rfc { get; set; }
        }


        public static Entity.Response<List<object>> TraerEmpresas2()
        {
            try
            {
                var empresas = MobileBO.ControlConfiguracion.TraerCatempresas();
                List<object> listaElementos = new List<object>();

                if (empresas != null)
                {
                    foreach (var empresa in empresas)
                    {
                        listaElementos.Add(empresa); // ✅ sin crear objeto anónimo
                    }
                }

                return Entity.Response<List<object>>.CrearResponse(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }
        private List<EMPRESAS> ObtenerEmpresas()
        {
            var resultado = TraerEmpresas2(); // retorna Entity.Response<List<object>>
            var listaEmpresas = new List<EMPRESAS>();

            if (resultado?.Datos != null)
            {
                foreach (var item in resultado.Datos)
                {
                    var empresa = item as Entity.Configuracion.Catempresa;
                    if (empresa != null)
                    {
                        listaEmpresas.Add(new EMPRESAS
                        {
                            EmpresaID = empresa.Empresaid.ToString(),
                            Nombre = empresa.Descripcion,
                            Rfc = empresa.Rfc
                        });
                    }
                }
            }

            return listaEmpresas;
        }

        protected void BtnDescargarConciliados_Click(object sender, EventArgs e)
        {
            var lista2 = Session["ListaModeloContabilidadCfdiModelo"] as List<CfdiModelo>;
            ExportarDatosAExcel(lista2, "CfdisConciliados.xls");
            //ExportarGridViewAExcel(GridViewCfdi, "CFDIs_Conciliados.xls");
        }
        protected void BtnDescargarExentos_Click(object sender, EventArgs e)
        {
            var lista = Session["ListaModeloExentaCfdiModelo"] as List<CfdiModelo>;
            ExportarDatosAExcel(lista, "CfdisExentos-o-Detalles.xls");
            //ExportarGridViewAExcel(GridViewCfdi1, "CFDIs_Exentos.xls");
        }
        protected void BtnDescargarExentosConta_Click(object sender, EventArgs e)
        {
            var lista = Session["ListaModeloExentaCfdiModelo"] as List<CfdiModelo>;
            ExportarDatosAExcel(lista, "CfdisExentos-o-Detalles.xls");
            //ExportarGridViewAExcel(GridViewCfdi1, "CFDIs_Exentos.xls");
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Evita el error "GridView must be placed inside a form tag with runat=server"
        }

        private void ExportarDatosAExcel(List<CfdiModelo> lista, string nombreArchivo)
        {
            var sb = new StringBuilder();

            // ENCABEZADOS
            sb.AppendLine("UUID\tRfc Emisor\tNombre Emisor\tUso CFDI\tBase 16\tIVA 16\tBase 8\tIVA 8\tBase 0\tBase Exenta\tFecha\tSubtotal\tTotal\tTotal IVA\tRetencion ISR\tRetencion IVA\tRetencionIEPS\tTotal Retenciones\tIVA Contabilidad\tDiferencia IVA");

            foreach (var item in lista)
            {
                if (item == null) continue;

                // Información general del CFDI
                string linea = string.Join("\t", new string[]
                {
                    item.SourceName,
                    item.RfcEmisor,
                    item.NombreEmisor,
                    item.UsoCFDI,
                    item.Base16.ToString(),
                    item.IVA16.ToString(),
                    item.Base8.ToString(),
                    item.IVA8.ToString(),
                    item.Base0.ToString(),
                    item.BaseExenta.ToString(),
                    item.Fecha.ToString(),
                    item.SubTotal.ToString("F2"),
                    item.Total.ToString("F2"),
                    item.TotalImpuestosTrasladados.ToString(),
                    item.RetencionISR.ToString("F2"),
                    item.RetencionIVA.ToString("F2"),
                    item.RetencionIEPS.ToString("F2"),
                    item.TotalImpuestosRetenidos.ToString(),
                    item.IVAContabilidad.ToString("F2"),
                    ((item.IVA16+item.IVA8)-item.IVAContabilidad).ToString()
                });

                sb.AppendLine(linea);
            }

            // Exportar como archivo .xls (en realidad es texto plano pero Excel lo abre bien)
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", $"attachment;filename={nombreArchivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }

        private void ExportarDatosAExcel2<T>(List<T> lista, string nombreArchivo)
        {
            var sb = new StringBuilder();

            // Obtener propiedades públicas de la clase T
            var props = typeof(T).GetProperties();

            // Encabezados con tabuladores
            sb.AppendLine(string.Join("\t", props.Select(p => p.Name)));

            // props = typeof(T).GetProperties();
            // ya agregaste encabezados arriba

            int excelRow = 2; // fila de datos en Excel

            foreach (var item in lista)
            {
                if (item == null) { excelRow++; continue; }

                var valores = new List<string>();

                foreach (var prop in props)
                {
                    if (string.Equals(prop.Name, "LineaTXT", StringComparison.OrdinalIgnoreCase))
                    {
                        // Tomar la plantilla o generar una por defecto
                        var plantilla = prop.GetValue(item, null) as string;
                        if (string.IsNullOrWhiteSpace(plantilla))
                            plantilla = BuildConcatenarFormulaTemplate(54); // A..BB

                        // Reemplazar {ROW} por la fila real
                        var formula = plantilla.Replace("{ROW}", excelRow.ToString());

                        // IMPORTANTE: no formatear la fórmula
                        valores.Add(formula);
                    }
                    else if (string.Equals(prop.Name, "TipoTercero", StringComparison.OrdinalIgnoreCase) | string.Equals(prop.Name, "TipoOperacion", StringComparison.OrdinalIgnoreCase) | string.Equals(prop.Name, "EfectosFiscales", StringComparison.OrdinalIgnoreCase))
                    {
                        var valor = prop.GetValue(item, null);
                        valores.Add(FormatearComoTextoExcel(valor));

                    }
                    else
                    {
                        var valor = prop.GetValue(item, null);
                        valores.Add(FormatearValorPlano(valor));
                    }
                }

                sb.AppendLine(string.Join("\t", valores));
                excelRow++;
            }


            // Exportar como archivo plano (Excel lo abrirá bien)
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", $"attachment;filename={nombreArchivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

        private static string FormatearComoTextoExcel(object valor)
        {
            var s = valor?.ToString() ?? string.Empty;

            // Limpieza mínima para que no “rompa” el TSV
            s = s.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");

            // Truco para que Excel lo trate como texto sin mostrar apóstrofo
            return $"=\"{s}\"";
        }
        private string FormatearValorPlano(object valor)
        {
            if (valor == null)
                return "";

            if (valor is string)
                return ((string)valor).Replace("\t", " ").Trim();

            if (valor is decimal)
            {
                decimal d = (decimal)valor;
                return d == 0 ? "" : d.ToString("F2");
            }

            if (valor is double)
            {
                double d = (double)valor;
                return d == 0 ? "" : d.ToString("F2");
            }

            if (valor is DateTime)
                return ((DateTime)valor).ToString("yyyy-MM-dd HH:mm:ss");

            return valor.ToString();
        }


        private void ExportarGridViewAExcel(GridView grid, string nombreArchivo)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo);
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    grid.AllowPaging = false;

                    // Es necesario reasignar DataSource antes del DataBind si depende de sesión
                    if (grid.ID == GridViewCfdi.ID)
                    {
                        var listaContabilidad = Session["ListaModeloContabilidad"] as List<dynamic>;
                        if (listaContabilidad != null)
                        {
                            grid.DataSource = listaContabilidad;
                        }
                    }
                    else if (grid.ID == GridViewCfdi1.ID)
                    {
                        var listaExenta = Session["ListaModeloExenta"] as List<dynamic>;
                        if (listaExenta != null)
                        {
                            grid.DataSource = listaExenta;
                        }
                    }

                    grid.DataBind();

                    grid.RenderControl(hw);
                    Response.Output.Write('\uFEFF' + sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }
        protected void BtnProcesarZip_Click(object sender, EventArgs e)
        {

            try
            {
                if (FileUploadCFDI.HasFile && FileUploadCFDI.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {

                    var listaEmpresas = Session["Empresas"] as List<EMPRESAS>;
                    if (listaEmpresas == null || listaEmpresas.Count == 0)
                    {
                        MostrarMensajeJS("Error", "La lista de empresas no está disponible. Refresque la página.", "danger");
                        return;
                    }

                    var empresaSelect = listaEmpresas.FirstOrDefault(x => x.EmpresaID == Empresas.SelectedValue);
                    var añoSelect = año.SelectedValue;
                    var mesSelect = mes.SelectedValue;

                    if (empresaSelect == null || string.IsNullOrWhiteSpace(empresaSelect.Rfc))
                    {
                        MostrarMensajeJS("Error", "Debe seleccionar una empresa válida.", "danger");
                        return;
                    }
                    // Separacion entre los dos tipos de archivos ingresos y egresos 
                    if (string.Equals(rblTipo.SelectedValue, "Ingresos", StringComparison.OrdinalIgnoreCase))
                    {
                        // Mostrar el panel del resumen de ingresos
                        PanelCFDIIngresos.Visible = true;

                        // Pintar Mes/Año en el título
                        var mesTexto = mes.SelectedItem != null ? mes.SelectedItem.Text : "";
                        var anioTexto = año.SelectedValue ?? "";
                        var mesAnio = (mesTexto + " " + anioTexto).Trim();
                        var jsMes = $@"(function(){{
                                      var el = document.getElementById('Mes');
                                      if (el) el.textContent = '{System.Web.HttpUtility.JavaScriptStringEncode(mesAnio)}';
                                    }})();";
                        var jsMes2 = $@"(function(){{
                                      var el = document.getElementById('Mes2');
                                      if (el) el.textContent = '{System.Web.HttpUtility.JavaScriptStringEncode(mesAnio)}';
                                    }})();";
                        ScriptManager.RegisterStartupScript(this, GetType(), "setMesIngresos", jsMes, true);

                        string rfcEmpresaSeleccionada = empresaSelect.Rfc.Trim().ToUpperInvariant();

                        using (var zipStream = FileUploadCFDI.PostedFile.InputStream)
                        using (var zipInput = new ZipInputStream(zipStream))
                        {
                            var listaCfdi = LeerCfdisDesdeZip(zipInput);
                            if (listaCfdi.Count == 0)
                            {
                                MostrarMensajeJS("Error", "No se encontró ningún XML válido en el ZIP.", "danger");
                                GridViewCuentaCorriente.Visible = false;
                                GridViewFactoraje.Visible = false;
                                PanelCFDIIngresos.Visible = false;
                                return;
                            }


                            // Traes todas las facturas
                            List<Entity.conciliacionIngresos> listaFacturas = MobileBO.ControlOperacion
                                .TraerFacturasPorEmpresaAñoMesIngresos(empresaSelect.EmpresaID, añoSelect, mesSelect);

                            // Separa por tipo
                            List<Entity.conciliacionIngresos> cartera_Factoraje = listaFacturas
                                .Where(x => x.TipoCliente == "1")
                                .ToList();

                            List<Entity.conciliacionIngresos> Mutuo_CuentaCorriente = listaFacturas
                                .Where(x => x.TipoCliente == "2" || x.TipoCliente == "0")
                                .ToList();


                            // Construir diccionarios por UUID (maneja duplicados y Guid.Empty)
                            Dictionary<Guid, Entity.conciliacionIngresos> dictFactoraje = new Dictionary<Guid, Entity.conciliacionIngresos>();
                            foreach (Entity.conciliacionIngresos f in cartera_Factoraje)
                            {
                                if (f.UUID != Guid.Empty && !dictFactoraje.ContainsKey(f.UUID))
                                    dictFactoraje.Add(f.UUID, f);
                            }

                            Dictionary<Guid, Entity.conciliacionIngresos> dictCuentaCorriente = new Dictionary<Guid, Entity.conciliacionIngresos>();
                            foreach (Entity.conciliacionIngresos f in Mutuo_CuentaCorriente)
                            {
                                if (f.UUID != Guid.Empty && !dictCuentaCorriente.ContainsKey(f.UUID))
                                    dictCuentaCorriente.Add(f.UUID, f);
                            }

                            Dictionary<Guid, Entity.conciliacionIngresos> dictNOCONTABILIDAD = new Dictionary<Guid, Entity.conciliacionIngresos>();
                            foreach (Entity.conciliacionIngresos f in listaFacturas)
                            {
                                if (f.UUID != Guid.Empty && !listaFacturas.Any(x => x.UUID == f.UUID))
                                {
                                    dictNOCONTABILIDAD.Add(f.UUID, f);
                                }
                            }

                            

                            // ---------- FACTORAJE ----------
                            List<ResumenIngresoRow> resumenFactorajeFinanciero = new List<ResumenIngresoRow>();

                            foreach (var item in listaCfdi)
                            {
                                string rfcEmisorXml = (item.Comprobante.Emisor != null ? item.Comprobante.Emisor.Rfc : string.Empty).Trim().ToUpperInvariant();
                                if (!string.Equals(rfcEmisorXml, rfcEmpresaSeleccionada, StringComparison.OrdinalIgnoreCase))
                                    continue;

                                string tipo = ((item.Comprobante.TipoDeComprobante ?? string.Empty).Trim().ToUpperInvariant());
                                if (tipo == "N")
                                    continue;

                                ResumenIngresoRow fila = ConstruirResumenDesdeCfdi(item);
                                if (fila == null) continue;

                                string sourceNameSinExtension = Path.GetFileNameWithoutExtension(item.SourceName ?? string.Empty);
                                Guid uuidSource;
                                if (Guid.TryParse(sourceNameSinExtension, out uuidSource))
                                {
                                    Entity.conciliacionIngresos factura;
                                    if (dictFactoraje.TryGetValue(uuidSource, out factura))
                                    {
                                        fila.InteresOrdinario2 = factura.InteresOrdinario;
                                        fila.InteresMoratorio2 = factura.InteresMoratorio;
                                        fila.Comision2 = factura.Comision;
                                        fila.IVA_2 = factura.IVA_Comprobante;
                                        fila.Tasa = factura.TasaMensual;

                                        fila.DifInteresOrdinario2 = Math.Round(
                                            (fila.InteresOrdinario - factura.InteresOrdinario), 2, MidpointRounding.AwayFromZero);

                                        fila.DifInteresMoratorio2 = Math.Round(
                                            (fila.InteresMoratorio - factura.InteresMoratorio), 2, MidpointRounding.AwayFromZero);

                                        fila.DifComision2 = Math.Round(
                                            (fila.Comision - factura.Comision), 2, MidpointRounding.AwayFromZero);

                                        fila.DifIVA_2 = Math.Round(
                                            (fila.IVA_1104 - factura.IVA_Comprobante), 2, MidpointRounding.AwayFromZero);

                                        resumenFactorajeFinanciero.Add(fila);
                                    }
                                }


                            }

                            // ---------- CUENTA CORRIENTE ----------
                            List<ResumenIngresoRow> resumenCuentaCorriente = new List<ResumenIngresoRow>();

                            foreach (var item in listaCfdi)
                            {
                                string rfcEmisorXml = (item.Comprobante.Emisor != null ? item.Comprobante.Emisor.Rfc : string.Empty).Trim().ToUpperInvariant();
                                if (!string.Equals(rfcEmisorXml, rfcEmpresaSeleccionada, StringComparison.OrdinalIgnoreCase))
                                    continue;

                                string tipo = ((item.Comprobante.TipoDeComprobante ?? string.Empty).Trim().ToUpperInvariant());
                                if (tipo == "N")
                                    continue;

                                ResumenIngresoRow fila = ConstruirResumenDesdeCfdi(item);
                                if (fila == null) continue;

                                string sourceNameSinExtension = Path.GetFileNameWithoutExtension(item.SourceName ?? string.Empty);
                                Guid uuidSource;
                                if (Guid.TryParse(sourceNameSinExtension, out uuidSource))
                                {
                                    Entity.conciliacionIngresos factura;
                                    if (dictCuentaCorriente.TryGetValue(uuidSource, out factura))
                                    {
                                        fila.InteresOrdinario2 = factura.InteresOrdinario;
                                        fila.InteresMoratorio2 = factura.InteresMoratorio;
                                        fila.Comision2 = factura.Comision;
                                        fila.IVA_2 = factura.IVA_Comprobante;
                                        fila.Tasa = factura.TasaMensual;

                                        fila.DifInteresOrdinario2 = Math.Round(
                                            (fila.InteresOrdinario - factura.InteresOrdinario), 2, MidpointRounding.AwayFromZero);

                                        fila.DifInteresMoratorio2 = Math.Round(
                                            (fila.InteresMoratorio - factura.InteresMoratorio), 2, MidpointRounding.AwayFromZero);

                                        fila.DifComision2 = Math.Round(
                                            (fila.Comision - factura.Comision), 2, MidpointRounding.AwayFromZero);

                                        fila.DifIVA_2 = Math.Round(
                                            (fila.IVA_1104 - factura.IVA_Comprobante), 2, MidpointRounding.AwayFromZero);

                                        resumenCuentaCorriente.Add(fila);
                                    }
                                }

                            }

                            // ---------- NO CONTABILIDAD ----------
                            List<ResumenIngresoRow> resumenNOCONTABILIDAD = new List<ResumenIngresoRow>();

                            // Crear un HashSet con los UUID de todas las facturas que sí existen en contabilidad
                            HashSet<Guid> uuidsFacturas = new HashSet<Guid>(
                                listaFacturas.Where(f => f.UUID != Guid.Empty).Select(f => f.UUID)
                            );

                            // Lista para almacenar los UUIDs que no están en contabilidad
                            List<string> uuidsNoContabilidad = new List<string>();

                            foreach (var item in listaCfdi)
                            {
                                // Filtra por empresa
                                string rfcEmisorXml = (item.Comprobante.Emisor?.Rfc ?? string.Empty)
                                                        .Trim()
                                                        .ToUpperInvariant();
                                if (!string.Equals(rfcEmisorXml, rfcEmpresaSeleccionada, StringComparison.OrdinalIgnoreCase))
                                    continue;

                                // Ignora notas de crédito
                                string tipo = (item.Comprobante.TipoDeComprobante ?? string.Empty).Trim().ToUpperInvariant();
                                if (tipo == "N")
                                    continue;

                                // Construye el resumen
                                ResumenIngresoRow fila = ConstruirResumenDesdeCfdi(item);
                                if (fila == null) continue;

                                // Tomar UUID directo del CFDI (TimbreFiscalDigital)
                                Guid uuidCFDI = Guid.Empty;
                                string uuidStr = item.Comprobante?.Complemento?.TimbreFiscalDigital?.UUID;

                                if (!string.IsNullOrWhiteSpace(uuidStr))
                                {
                                    Guid.TryParse(uuidStr, out uuidCFDI);
                                }

                                if (uuidCFDI != Guid.Empty && !uuidsFacturas.Contains(uuidCFDI))
                                {
                                    // Agregar al resumen NO CONTABILIDAD
                                    resumenNOCONTABILIDAD.Add(fila);

                                    // Agregar el UUID a la lista de XML no encontrados
                                    uuidsNoContabilidad.Add(uuidCFDI.ToString());
                                }
                            }
                            ViewState["TotFactoraje"] = CalcularTotales(resumenFactorajeFinanciero);
                            ViewState["TotCtaCte"] = CalcularTotales(resumenCuentaCorriente);

                            // Bindings y visibilidad
                            Session["ResumenIngresosCFDIFactorajeFinanciero"] = resumenFactorajeFinanciero;
                            GridViewFactoraje.DataSource = resumenFactorajeFinanciero.OrderBy(x => x.FechaEmision).ToList();
                            GridViewFactoraje.DataBind();
                            GridViewFactoraje.Visible = resumenFactorajeFinanciero.Count > 0;

                            Session["ResumenIngresosCFDICuentaCorriente"] = resumenCuentaCorriente;
                            GridViewCuentaCorriente.DataSource = resumenCuentaCorriente.OrderBy(x => x.FechaEmision).ToList();
                            GridViewCuentaCorriente.DataBind();
                            GridViewCuentaCorriente.Visible = resumenCuentaCorriente.Count > 0;
                            

                            if (resumenNOCONTABILIDAD.Count > 0) // dt es el DataTable o lista que bindeas
                            {
                                XMLNOCONTA.DataSource = resumenNOCONTABILIDAD.OrderBy(x => x.FechaEmision).ToList();
                                XMLNOCONTA.DataBind();
                                XMLNOCONTA.Visible = true;
                                BtnPDFXMLNOCONTA.Visible = true;
                                exportarEXCEL2.Visible = true;
                            }
                            else
                            {
                                XMLNOCONTA.Visible = false;
                                lblSinXML.Visible = true;
                                BtnPDFXMLNOCONTA.Visible = false;
                                exportarEXCEL2.Visible = false;
                            }


                            if (resumenCuentaCorriente.Count == 0 && resumenFactorajeFinanciero.Count == 0)
                            {
                                MostrarMensajeJS("Aviso", "El ZIP no contiene XML del mes o de la empresa seleccionada", "warning");
                                PanelCFDIIngresos.Visible = false;
                                return;
                            }



                            MostrarMensajeJS("Éxito", "Se cargaron CFDI de ingresos.", "success");

                        }
                    }
                    else if (string.Equals(rblTipo.SelectedValue, "Egresos", StringComparison.OrdinalIgnoreCase))
                    {

                        string rfcEmpresaSeleccionada = empresaSelect.Rfc.Trim().ToUpperInvariant();

                        using (var zipStream = FileUploadCFDI.PostedFile.InputStream)
                        using (var zipInput = new ZipInputStream(zipStream))
                        {
                            List<CfdiArchivo> listaCfdi = LeerCfdisDesdeZip(zipInput);
                            if (listaCfdi.Count == 0)
                            {
                                MostrarMensajeJS("Error", "No se encontró ningún XML válido en el ZIP.", "danger");
                                GridViewCfdi.Visible = false;
                                GridViewCfdi1.Visible = false;
                                GridViewCfdi2.Visible = false;
                                return;
                            }

                            List<Entity.ModeloFactura> listaFacturas = MobileBO.ControlOperacion
                            .TraerFacturasPorEmpresaAñoMes(empresaSelect.EmpresaID, añoSelect, mesSelect);


                            Dictionary<string, UuidInfo> uuidInfoDict = new Dictionary<string, UuidInfo>(StringComparer.OrdinalIgnoreCase);
                            HashSet<string> uuidsFactura = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            if (listaFacturas != null)
                            {
                                foreach (var factura in listaFacturas)
                                {
                                    if (!string.IsNullOrEmpty(factura.UUID))
                                    {
                                        string uuid = factura.UUID.ToUpperInvariant();
                                        uuidsFactura.Add(uuid);

                                        uuidInfoDict[uuid] = new UuidInfo
                                        {
                                            Abonado = factura.IVA.ToString(),
                                            uuid = factura.UUID,
                                            tipoOperacion = factura.ClvTipoOperacion
                                        };
                                    }
                                }
                            }

                            List<dynamic> listaModelocontabilidad = new List<dynamic>();
                            List<dynamic> listaModeloexenta = new List<dynamic>();

                            // NUEVAS listas tipadas para exportación
                            List<CfdiModelo> listaModeloContabilidad = new List<CfdiModelo>();
                            List<CfdiModelo> listaModeloExenta = new List<CfdiModelo>();

                            foreach (CfdiArchivo item in listaCfdi)
                            {
                                string rfcReceptorXml = (item.Comprobante.Receptor?.Rfc ?? "").Trim().ToUpperInvariant();
                                bool esDonativo = item.Comprobante.Complemento?.Donatarias?.Leyenda != null &&
                                item.Comprobante.Complemento.Donatarias.Leyenda.Trim().StartsWith("Este comprobante ampara un donativo", StringComparison.OrdinalIgnoreCase);

                                if (rfcReceptorXml != rfcEmpresaSeleccionada)
                                {
                                    MostrarMensajeJS("Error", "El archivo ZIP no contiene CFDI válidos para la empresa seleccionada.", "danger");
                                    PanelResultados.Visible = false;
                                    FileUploadCFDI.Enabled = true;
                                    return;
                                }
                                else
                                {
                                    string nombreArchivoSinExtension = Path.GetFileNameWithoutExtension(item.SourceName)?.ToUpperInvariant();

                                    var datos = ObtenerDatosCfdi(item.Comprobante, item.SourceName);

                                    var detalle = GenerarDetalleCfdi(datos);

                                    if (EsGobierno(item.Comprobante.Emisor?.Nombre) || item.Comprobante.TipoDeComprobante == "P" || esDonativo)
                                    {
                                        datos.Exento = true;

                                        if (uuidsFactura.Contains(nombreArchivoSinExtension))
                                        {
                                            datos.Contabilidad = true;
                                            UuidInfo info;

                                            if (uuidInfoDict.TryGetValue(nombreArchivoSinExtension, out info))
                                            {
                                                decimal abonadoVal;
                                                if (decimal.TryParse(info.Abonado, out abonadoVal))
                                                {
                                                    datos.IVAContabilidad = abonadoVal;
                                                }
                                                else
                                                {
                                                    datos.IVAContabilidad = 0;
                                                }

                                                datos.TipoOperacion = info.tipoOperacion;
                                            }
                                        }
                                        else
                                        {
                                            UuidInfo info;
                                            if (uuidInfoDict.TryGetValue(nombreArchivoSinExtension, out info))
                                            {
                                                decimal abonadoVal;
                                                if (decimal.TryParse(info.Abonado, out abonadoVal))
                                                {
                                                    datos.IVAContabilidad = abonadoVal;
                                                }
                                                else
                                                {
                                                    datos.IVAContabilidad = 0;
                                                }
                                                datos.TipoOperacion = info.tipoOperacion;
                                            }
                                            datos.Contabilidad = false;
                                        }

                                        listaModeloexenta.Add(datos);
                                        listaModeloExenta.Add(datos); // ← tipada para exportación
                                    }
                                    else if (uuidsFactura.Contains(nombreArchivoSinExtension))
                                    {

                                        datos.Contabilidad = true;

                                        UuidInfo info;
                                        if (uuidInfoDict.TryGetValue(nombreArchivoSinExtension, out info))
                                        {
                                            decimal abonadoVal;
                                            datos.TipoOperacion = info.tipoOperacion;
                                            if (decimal.TryParse(info.Abonado, out abonadoVal))
                                            {
                                                datos.IVAContabilidad = abonadoVal;
                                            }
                                            else
                                            {
                                                datos.IVAContabilidad = 0;
                                            }
                                        }

                                        listaModelocontabilidad.Add(datos);
                                        listaModeloContabilidad.Add(datos);
                                    }
                                    else
                                    {
                                        datos.Exento = false;
                                        datos.Contabilidad = false;
                                        UuidInfo info;
                                        if (uuidInfoDict.TryGetValue(nombreArchivoSinExtension, out info))
                                        {
                                            decimal abonadoVal;
                                            if (decimal.TryParse(info.Abonado, out abonadoVal))
                                            {
                                                datos.IVAContabilidad = abonadoVal;
                                            }
                                            else
                                            {
                                                datos.IVAContabilidad = 0;
                                            }
                                        }
                                        listaModeloexenta.Add(datos);
                                        listaModeloExenta.Add(datos); // ← tipada para exportación

                                    }
                                }
                            }

                            HashSet<string> uuidsDesdeZip = new HashSet<string>(
                            listaCfdi.Select(x => Path.GetFileNameWithoutExtension(x.SourceName)?.ToUpperInvariant()),
                            StringComparer.OrdinalIgnoreCase);
                            listaFacturas = listaFacturas
                             .Where(f => !uuidsDesdeZip.Contains(f.UUID?.ToUpperInvariant()))
                             .ToList();

                            var listaFacturasFiltrada = listaFacturas
                            .Where(f => !f.Mostrar) // ✅ Solo procesar las que no se deben ignorar
                            .ToList();
                            decimal sumaSubTotalContabilidad = listaModeloContabilidad.Sum(x => x.SubTotal);
                            decimal sumaTotalContabilidad = listaModeloContabilidad.Sum(x => x.Total);

                            // Guardar en ViewState para usarlo en el evento RowDataBound
                            ViewState["SumaSubTotalContabilidad"] = sumaSubTotalContabilidad;
                            ViewState["SumaTotalContabilidad"] = sumaTotalContabilidad;

                            decimal sumaSubTotalexenta = listaModeloExenta.Sum(x => x.SubTotal);
                            decimal sumaTotalexenta = listaModeloExenta.Sum(x => x.Total);

                            // Guardar en ViewState para usarlo en el evento RowDataBound
                            ViewState["sumaSubTotalexenta"] = sumaSubTotalexenta;
                            ViewState["sumaTotalexenta"] = sumaTotalexenta;

                            // Guardar en sesión ambas versiones:
                            Session["ListaModeloContabilidad"] = listaModelocontabilidad.OrderBy(x => x.RfcEmisor).ToList();
                            Session["ListaModeloExenta"] = listaModeloexenta.OrderBy(x => x.RfcEmisor).ToList();
                            Session["ListaModeloContabilidadNoXML"] = listaFacturasFiltrada.OrderBy(x => x.RFC).ToList();
                            // NUEVO: Guardar listas tipadas para exportar con estructura de datos completa
                            Session["ListaModeloContabilidadCfdiModelo"] = listaModeloContabilidad.OrderBy(x => x.RfcEmisor).ToList();
                            Session["ListaModeloExentaCfdiModelo"] = listaModeloExenta.OrderBy(x => x.RfcEmisor).ToList();

                            MostrarGrid(GridViewCfdi1, listaModeloexenta.OrderBy(x => x.RfcEmisor).ToList());
                            MostrarGrid(GridViewCfdi2, listaFacturasFiltrada.OrderBy(x => x.RFC).ToList());
                            MostrarGrid(GridViewCfdi, listaModelocontabilidad.OrderBy(x => x.RfcEmisor).ToList());

                            BtnDescargarConciliados.Visible = GridViewCfdi.Rows.Count > 0;
                            BtnDescargarExentos.Visible = GridViewCfdi1.Rows.Count > 0;
                            GridViewCfdi.Visible = listaModelocontabilidad.Count > 0;
                            GridViewCfdi1.Visible = listaModeloexenta.Count > 0;
                            GridViewCfdi2.Visible = listaFacturas.Count > 0;
                            PanelResultados.Visible = true;

                            int totalProcesados = listaModelocontabilidad.Count + listaModeloexenta.Count;
                            MostrarMensajeJS("Éxito", $"Se procesaron {totalProcesados} archivos XML válidos del ZIP.", "success");

                        }
                    }
                    else
                    {

                        MostrarMensajeJS("Error", "Debe seleccionar un archivo ZIP válido. ", "danger");

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeJS("Error", "Error al procesar el archivo: " + ex.Message, "danger");
            }
            finally
            {

            }
        }

        [Serializable]
        public sealed class TotalesResumen
        {
            public decimal IngresoExento, IngresoGravado, IVA_CFDI, TotalCFDI;
            public decimal InteresOrdinario, InteresMoratorio, Comision, IVA_1104;
            public decimal InteresOrdinario2, InteresMoratorio2, Comision2, IVA_2;
            public decimal DifInteresOrdinario2, DifInteresMoratorio2, DifComision2, DifIVA_2;
        }

        private static TotalesResumen CalcularTotales(IEnumerable<ResumenIngresoRow> lista)
        {
            return new TotalesResumen
            {
                InteresOrdinario = lista.Sum(x => x.InteresOrdinario),
                InteresMoratorio = lista.Sum(x => x.InteresMoratorio),
                Comision = lista.Sum(x => x.Comision),
                IVA_1104 = lista.Sum(x => x.IVA_1104),

                InteresOrdinario2 = lista.Sum(x => x.InteresOrdinario2),
                InteresMoratorio2 = lista.Sum(x => x.InteresMoratorio2),
                Comision2 = lista.Sum(x => x.Comision2),
                IVA_2 = lista.Sum(x => x.IVA_2),

                DifInteresOrdinario2 = lista.Sum(x => x.DifInteresOrdinario2),
                DifInteresMoratorio2 = lista.Sum(x => x.DifInteresMoratorio2),
                DifComision2 = lista.Sum(x => x.DifComision2),
                DifIVA_2 = lista.Sum(x => x.DifIVA_2),
            };
        }
        private TotalesResumen CalcularTotalMes()
        {
            var totFactoraje = ViewState["TotFactoraje"] as TotalesResumen ?? new TotalesResumen();
            var totCtaCte = ViewState["TotCtaCte"] as TotalesResumen ?? new TotalesResumen();

            return new TotalesResumen
            {
                IngresoExento = totFactoraje.IngresoExento + totCtaCte.IngresoExento,
                IngresoGravado = totFactoraje.IngresoGravado + totCtaCte.IngresoGravado,
                IVA_CFDI = totFactoraje.IVA_CFDI + totCtaCte.IVA_CFDI,
                TotalCFDI = totFactoraje.TotalCFDI + totCtaCte.TotalCFDI,

                InteresOrdinario = totFactoraje.InteresOrdinario + totCtaCte.InteresOrdinario,
                InteresMoratorio = totFactoraje.InteresMoratorio + totCtaCte.InteresMoratorio,
                Comision = totFactoraje.Comision + totCtaCte.Comision,
                IVA_1104 = totFactoraje.IVA_1104 + totCtaCte.IVA_1104,

                InteresOrdinario2 = totFactoraje.InteresOrdinario2 + totCtaCte.InteresOrdinario2,
                InteresMoratorio2 = totFactoraje.InteresMoratorio2 + totCtaCte.InteresMoratorio2,
                Comision2 = totFactoraje.Comision2 + totCtaCte.Comision2,
                IVA_2 = totFactoraje.IVA_2 + totCtaCte.IVA_2,

                DifInteresOrdinario2 = totFactoraje.DifInteresOrdinario2 + totCtaCte.DifInteresOrdinario2,
                DifInteresMoratorio2 = totFactoraje.DifInteresMoratorio2 + totCtaCte.DifInteresMoratorio2,
                DifComision2 = totFactoraje.DifComision2 + totCtaCte.DifComision2,
                DifIVA_2 = totFactoraje.DifIVA_2 + totCtaCte.DifIVA_2
            };
        }

        private static string M(decimal v) => v.ToString("C");

        protected void GridViewFactoraje_DataBound(object sender, EventArgs e)
        {
            if (GridViewFactoraje.FooterRow == null) return;

            var lista = Session["ResumenIngresosCFDIFactorajeFinanciero"] as List<ResumenIngresoRow>;
            if (lista == null || lista.Count == 0) return;

            var tot = CalcularTotales(lista);
            var f = GridViewFactoraje.FooterRow;

            f.Font.Bold = true;
            f.Cells[7].Text = "TOTALES:";

            f.Cells[9].Text = M(tot.InteresOrdinario);
            f.Cells[10].Text = M(tot.InteresMoratorio);
            f.Cells[11].Text = M(tot.Comision);
            f.Cells[12].Text = M(tot.IVA_1104);

            f.Cells[13].Text = M(tot.InteresOrdinario2);
            f.Cells[14].Text = M(tot.InteresMoratorio2);
            f.Cells[15].Text = M(tot.Comision2);
            f.Cells[16].Text = M(tot.IVA_2);

            f.Cells[17].Text = M(tot.DifInteresOrdinario2);
            f.Cells[18].Text = M(tot.DifInteresMoratorio2);
            f.Cells[19].Text = M(tot.DifComision2);
            f.Cells[20].Text = M(tot.DifIVA_2);

            // 👉 Crear fila en blanco para separación
            GridViewRow spacerRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Insert);
            for (int i = 0; i < GridViewFactoraje.Columns.Count; i++)
                spacerRow.Cells.Add(new TableCell());
            spacerRow.Height = Unit.Pixel(20);
            GridViewFactoraje.Controls[0].Controls.Add(spacerRow);

            // 👉 TOTAL DEL MES (Factoraje + Cuenta Corriente)
            var totalMes = CalcularTotalMes();
            GridViewRow rowTotalMes = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Insert);
            rowTotalMes.Font.Bold = true;
            rowTotalMes.BackColor = System.Drawing.Color.LightGray;

            for (int i = 0; i < GridViewFactoraje.Columns.Count; i++)
                rowTotalMes.Cells.Add(new TableCell());

            rowTotalMes.Cells[7].Text = "TOTAL DEL MES";

            rowTotalMes.Cells[9].Text = M(totalMes.InteresOrdinario);
            rowTotalMes.Cells[10].Text = M(totalMes.InteresMoratorio);
            rowTotalMes.Cells[11].Text = M(totalMes.Comision);
            rowTotalMes.Cells[12].Text = M(totalMes.IVA_1104);

            rowTotalMes.Cells[13].Text = M(totalMes.InteresOrdinario2);
            rowTotalMes.Cells[14].Text = M(totalMes.InteresMoratorio2);
            rowTotalMes.Cells[15].Text = M(totalMes.Comision2);
            rowTotalMes.Cells[16].Text = M(totalMes.IVA_2);

            rowTotalMes.Cells[17].Text = M(totalMes.DifInteresOrdinario2);
            rowTotalMes.Cells[18].Text = M(totalMes.DifInteresMoratorio2);
            rowTotalMes.Cells[19].Text = M(totalMes.DifComision2);
            rowTotalMes.Cells[20].Text = M(totalMes.DifIVA_2);

            GridViewFactoraje.Controls[0].Controls.Add(rowTotalMes);
        }

        // ======= CUENTA CORRIENTE: pinta totales en el footer ==========
        protected void GridViewCuentaCorriente_DataBound(object sender, EventArgs e)
        {
            if (GridViewCuentaCorriente.FooterRow == null) return;

            var lista = Session["ResumenIngresosCFDICuentaCorriente"] as List<ResumenIngresoRow>;
            if (lista == null || lista.Count == 0) return;

            var tot = CalcularTotales(lista);
            var f = GridViewCuentaCorriente.FooterRow;

            f.Font.Bold = true;
            f.Cells[7].Text = "TOTALES:";
            f.Cells[9].Text = M(tot.InteresOrdinario);
            f.Cells[10].Text = M(tot.InteresMoratorio);
            f.Cells[11].Text = M(tot.Comision);
            f.Cells[12].Text = M(tot.IVA_1104);

            f.Cells[13].Text = M(tot.InteresOrdinario2);
            f.Cells[14].Text = M(tot.InteresMoratorio2);
            f.Cells[15].Text = M(tot.Comision2);
            f.Cells[16].Text = M(tot.IVA_2);

            f.Cells[17].Text = M(tot.DifInteresOrdinario2);
            f.Cells[18].Text = M(tot.DifInteresMoratorio2);
            f.Cells[19].Text = M(tot.DifComision2);
            f.Cells[20].Text = M(tot.DifIVA_2);
        }


        private ResumenIngresoRow ConstruirResumenDesdeCfdi(CfdiArchivo item)
        {
            var c = item?.Comprobante;
            if (c == null) return null;

            // Separar intereses/commisiones
            var sep = CfdiInteresHelper.SepararIntereses(c);

            // Totales por tipo de traslado (encabezado)
            decimal ingresoExento = 0m, ingresoGravado = 0m, ivaCfdi = 0m, tasaMax = 0m;
            var tras = c.Impuestos?.Traslados;
            if (tras != null)
            {
                foreach (var t in tras)
                {
                    // IVA (002) - TipoFactor Tasa/Exento
                    if (string.Equals(t.TipoFactor, "Exento", StringComparison.OrdinalIgnoreCase))
                    {
                        ingresoExento += t.Base;
                    }
                    else if (string.Equals(t.TipoFactor, "Tasa", StringComparison.OrdinalIgnoreCase))
                    {
                        ingresoGravado += t.Base;
                        ivaCfdi += t.Importe;
                        if (t.TasaOCuota > tasaMax) tasaMax = t.TasaOCuota;
                    }
                }
            }

            return new ResumenIngresoRow
            {
                UUID = ObtenerUUID(item),
                RfcReceptor = c.Receptor?.Rfc,
                FechaEmision = c.Fecha,

                IngresoExento = ingresoExento,
                IngresoGravado = ingresoGravado,
                IVA_CFDI = c.Impuestos?.TotalImpuestosTrasladados ?? ivaCfdi,
                TotalCFDI = c.Total,

                TipoDeComprobante = c.TipoDeComprobante,
                InteresOrdinario = sep.BaseInteresOrdinario,
                InteresMoratorio = sep.BaseInteresMoratorio,
                Comision = sep.BaseComision,
                IVA_1104 = sep.IVAInteresOrdinario + sep.IVAInteresMoratorio + sep.IVAComision,
                InteresOrdinario2 = 0m,
                InteresMoratorio2 = 0m,
                Comision2 = 0m,
                IVA_2 = 0m
            };
        }

        private string ObtenerUUID(CfdiArchivo item)
        {
            // Si tienes timbre tipado, úsalo aquí; si no, usa el nombre de archivo:
            try { return System.IO.Path.GetFileNameWithoutExtension(item?.SourceName)?.ToUpperInvariant(); }
            catch { return item?.SourceName; }
        }

        protected void GridViewCfdi_RowDataBoundContabilidad(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal sumaSubTotal = (decimal)(ViewState["SumaSubTotalContabilidad"] ?? 0m);
                decimal sumaTotal = (decimal)(ViewState["SumaTotalContabilidad"] ?? 0m);

                // Suponiendo que SubTotal es columna 6 y Total es columna 8 (ajusta según sea necesario)

                e.Row.Cells[14].Text = "Subtotal total:";
                e.Row.Cells[15].Text = sumaSubTotal.ToString("C");

                e.Row.Cells[16].Text = "Total:";
                e.Row.Cells[17].Text = sumaTotal.ToString("C");

                e.Row.Font.Bold = true;
            }
        }
        protected void GridViewCfdi_RowDataBoundExenta(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal sumaSubTotal = (decimal)(ViewState["sumaSubTotalexenta"] ?? 0m);
                decimal sumaTotal = (decimal)(ViewState["sumaTotalexenta"] ?? 0m);

                // Suponiendo que SubTotal es columna 6 y Total es columna 8 (ajusta según sea necesario)

                e.Row.Cells[12].Text = "Subtotal total:";
                e.Row.Cells[13].Text = sumaSubTotal.ToString("C");

                e.Row.Cells[14].Text = "Total:";
                e.Row.Cells[15].Text = sumaTotal.ToString("C");



                e.Row.Font.Bold = true;
            }
        }

        private string ObtenerRfcDeXml(string xmlContenido)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContenido);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4"); // usa 3 si es CFDI 3.3
            XmlNode receptor = doc.SelectSingleNode("//cfdi:Receptor", nsmgr);
            return receptor?.Attributes["Rfc"]?.Value;
        }

        protected string ObtenerTextoExentoPlano(object valor)
        {
            bool esExento = false;
            Boolean.TryParse(valor?.ToString(), out esExento);
            return esExento ? "✅" : "❌";
        }

        protected string ObtenerTextoContabilidadPlano(object valor)
        {
            bool enContabilidad = false;
            Boolean.TryParse(valor?.ToString(), out enContabilidad);
            return enContabilidad ? "✅" : "❌";
        }

        public string ObtenerHtmlExento(object exento)
        {
            int valor = Convert.ToInt32(exento);
            if (valor == 0)
            {
                return @"<span style='display:inline-flex; justify-content:center; align-items:center; width:24px; height:24px; background-color:red; border-radius:50%; color:white; font-weight:bold; font-size:16px; line-height:1;'>
                    <span style='position:relative; top:-1px;'>❌</span>
                </span><br /><small style='font-size:10px; color:gray;'>Exento</small>";
            }
            else
            {
                return @"<span style='display:inline-flex; justify-content:center; align-items:center; width:24px; height:24px; background-color:green; border-radius:50%; color:white; font-weight:bold; font-size:16px; line-height:1;'>
                    <span style='position:relative; top:-1px;'>✅</span>
                </span><br /><small style='font-size:10px; color:gray;'>Exento</small>";
            }
        }
        public string ObtenerHtmlContabilidad(object contabilidad)
        {
            int valor = Convert.ToInt32(contabilidad);
            if (valor == 0)
            {
                return @"<span style='display:inline-flex; justify-content:center; align-items:center; width:24px; height:24px; background-color:red; border-radius:50%; color:white; font-weight:bold; font-size:16px; line-height:1;'>
                    <span style='position:relative; top:-1px;'>❌</span>
                </span><br /><small style='font-size:10px; color:gray;'>Contabilidad</small>";
            }
            else
            {
                return @"<span style='display:inline-flex; justify-content:center; align-items:center; width:24px; height:24px; background-color:green; border-radius:50%; color:white; font-weight:bold; font-size:16px; line-height:1;'>
                    <span style='position:relative; top:-1px;'>✅</span>
                </span><br /><small style='font-size:10px; color:gray;'>Contabilidad</small>";
            }
        }
        private List<CfdiArchivo> LeerCfdisDesdeZip(ZipInputStream zipInput)
        {
            List<CfdiArchivo> lista = new List<CfdiArchivo>();
            ZipEntry entry;
            XmlSerializer serializer = new XmlSerializer(typeof(Comprobante));
            while ((entry = zipInput.GetNextEntry()) != null)
            {
                if (!entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    continue;
                using (MemoryStream ms = new MemoryStream())
                {
                    zipInput.CopyTo(ms);
                    ms.Position = 0;
                    try
                    {
                        Comprobante comprobante = (Comprobante)serializer.Deserialize(ms);
                        lista.Add(new CfdiArchivo
                        {
                            Comprobante = comprobante,
                            SourceName = entry.Name
                        });
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return lista;
        }

        private string ObtenerTipoTercero(string rfcEmisor)
        {
            if (string.IsNullOrWhiteSpace(rfcEmisor))
                return "04"; // Por default

            if (rfcEmisor.StartsWith("XEXX", StringComparison.OrdinalIgnoreCase))
                return "05"; // Extranjero

            return "04";
        }

        private string ObtenerTipoOperacion(string razonSocialEmisor)
        {
            if (string.IsNullOrWhiteSpace(razonSocialEmisor))
                return "85";

            string nombre = razonSocialEmisor.ToUpperInvariant();

            if (nombre.Contains("GOBIERNO") || nombre.Contains("SECRETARÍA") || nombre.Contains("MUNICIPIO"))
                return "15";

            if (nombre.Contains("ARRENDAMIENTO"))
                return "09";

            if (nombre.Contains("SERVICIO"))
                return "03";

            return "85";
        }

        private CfdiModeloDetallado GenerarDetalleCfdi(CfdiModelo baseCfdi)
        {
            if (string.IsNullOrWhiteSpace(baseCfdi.TipoOperacion))
            {
                baseCfdi.TipoOperacion = "85";
            }
            return new CfdiModeloDetallado
            {
                TipoTercero = ObtenerTipoTercero(baseCfdi.RfcEmisor).ToString(),
                TipoOperacion = baseCfdi.TipoOperacion,
                RFC = baseCfdi.RfcEmisor,
                IDFiscal = "",//baseCfdi.RfcEmisor,
                ValorExento = ValorTexto(baseCfdi.BaseExenta),
                Valor0 = ValorTexto(baseCfdi.Base0),
                ActividadesPagadasResto = ValorTexto(baseCfdi.Base16),
                ActividadesPagadasRFN = ValorTexto(baseCfdi.Base8),
                IVA_Acreditable_Resto = ValorTexto(baseCfdi.IVA16),
                IVA_Acreditable_RF_Norte = ValorTexto(baseCfdi.IVA8),
                IVA_Retenido_Contribuyente = ValorTexto(baseCfdi.RetencionIVA),
                EfectosFiscales = "01".ToString()
                // actualmente solo se toman en cuenta estas 01/07/2025
            };
        }

        private dynamic ObtenerDatosCfdi(Comprobante c, string sourceName)
        {
            try
            {
                decimal base16 = 0, iva16 = 0, base8 = 0, iva8 = 0, base0 = 0, baseExenta = 0;

                List<string> descripciones = new List<string>();
                if (c.Conceptos != null)
                {
                    foreach (var concepto in c.Conceptos)
                    {
                        if (!string.IsNullOrEmpty(concepto.Descripcion))
                            descripciones.Add(concepto.Descripcion);
                    }
                }
                string htmlTraslados = "<table class='dynamic-table'>" +
                           "<tr>" +
                           "<th>Base</th>" +
                           "<th>Impuesto</th>" +
                           "<th>Tipo Factor</th>" +
                           "<th>Tasa</th>" +
                           "<th>Importe</th>" +
                           "</tr>";
                if (c.Impuestos?.Traslados != null)
                {
                    int BASES = 0;
                    foreach (var traslado in c.Impuestos.Traslados)
                    {
                        decimal baseTraslado = traslado.Base;
                        decimal tasa = traslado.TasaOCuota;
                        decimal importeTraslado = traslado.Importe;
                        string tasaString = "";

                        // 💡 Sumamos correctamente según la tasa
                        if (traslado.Impuesto == "002" && traslado.TipoFactor == "Tasa")
                        {
                            if (tasa == 0.160000m)
                            {
                                base16 += baseTraslado;
                                iva16 += importeTraslado;
                            }
                            else if (tasa == 0.080000m)
                            {
                                base8 += baseTraslado;
                                iva8 += importeTraslado;
                            }
                            else if (tasa == 0.000000m)
                            {
                                base0 += baseTraslado;
                                BASES = 1;
                                // iva0 = 0 ya lo dejas
                            }

                        }
                        else if (traslado.TipoFactor == "Exento")
                        {
                            baseExenta += baseTraslado;
                            BASES = 2;
                        }
                        // Extraemos TUA (nullable decimal)
                        decimal? tua = c.Complemento?.Aerolineas?.TUA;
                        if (tua.HasValue && tua.Value > 0)
                        {
                            if (!(c.Emisor.Rfc == "ANA050518RL1"))
                            {
                                if (c.Descuento > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }

                                }
                                else
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }
                                }
                            }
                            else if (!(c.Emisor.Rfc == "CVA041027H80"))
                            {
                                if (c.Descuento > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }

                                }
                                else
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }
                                }
                            }
                            if (!(c.Emisor.Rfc == "LCA111018A27"))
                            {
                                if (c.Descuento > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }

                                }
                                else
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }
                                }
                            }
                            if (!(c.Emisor.Rfc == "AME880912I89"))
                            {
                                if (c.Descuento > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }

                                }
                                else
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = tua.Value - c.Descuento;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = tua.Value - c.Descuento;
                                    }
                                }
                            }
                            // Si hay complemento de impuestos locales
                            if (c.Complemento?.ImpuestosLocales != null)
                            {
                                decimal ish = c.Complemento.ImpuestosLocales.TotaldeTraslados;


                                if (tua.HasValue && tua.Value > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = ish + tua.Value;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = ish + tua.Value;
                                    }
                                }
                                else if (ish > 0)
                                {
                                    if (BASES == 1)
                                    {
                                        base0 = ish + tua.Value;
                                    }
                                    else if (BASES == 2)
                                    {
                                        baseExenta = ish + tua.Value;
                                    }
                                }

                            }
                        }
                        // Construcción de tabla para visualización
                        if (traslado.TipoFactor == "Tasa")
                            tasaString = (tasa * 100).ToString("0.##") + "%";
                        else if (traslado.TipoFactor == "Exento")
                            tasaString = "Exento";

                        htmlTraslados += $"<tr>" +
                                          $"<td>{baseTraslado:F2}</td>" +
                                          $"<td>{traslado.Impuesto}</td>" +
                                          $"<td>{traslado.TipoFactor}</td>" +
                                          $"<td>{tasaString}</td>" +
                                          $"<td>{importeTraslado:F2}</td>" +
                                          $"</tr>";
                    }
                }




                htmlTraslados += "</table>";
                var tiposBase = new[]
                {
                new { Tipo = "16%", Monto = base16 },
                new { Tipo = "8%", Monto = base8 },
                new { Tipo = "0%", Monto = base0 },
                new { Tipo = "Exento", Monto = baseExenta }
            };
                var tipoDominante = tiposBase.OrderByDescending(x => x.Monto).First();
                decimal totalretenciones = 0;
                decimal totalimpuestos = 0;

                decimal retencionISR = 0;
                decimal retencionIVA = 0;
                decimal retencionIEPS = 0;

                if (!string.Equals(c.Receptor.UsoCFDI, "CP01", StringComparison.OrdinalIgnoreCase))
                {
                    totalretenciones = c.Impuestos?.TotalImpuestosRetenidos ?? 0;

                    if (totalretenciones > 0 && c.Impuestos?.Retenciones != null)
                    {
                        foreach (var ret in c.Impuestos.Retenciones)
                        {
                            if (ret == null) continue;

                            switch (ret.Impuesto)
                            {
                                case "001":
                                    retencionISR += ret.Importe;
                                    break;
                                case "002":
                                    retencionIVA += ret.Importe;
                                    break;
                                case "003":
                                    retencionIEPS += ret.Importe;
                                    break;
                            }
                        }
                    }
                }
                totalimpuestos = c.Impuestos?.TotalImpuestosTrasladados ?? 0;

                return new CfdiModelo
                {
                    SourceName = sourceName,
                    RfcEmisor = c.Emisor?.Rfc,
                    NombreEmisor = c.Emisor?.Nombre,
                    UsoCFDI = c.Receptor?.UsoCFDI,
                    Descripciones = string.Join("; ", descripciones),
                    TrasladosDetalle = htmlTraslados,
                    Serie = c.Serie,
                    Folio = "'" + c.Folio + "'",
                    Fecha = c.Fecha,
                    FormaPago = c.FormaPago,
                    TotalImpuestosTrasladados = totalimpuestos,
                    TotalImpuestosRetenidos = totalretenciones,
                    SubTotal = c.SubTotal,
                    Moneda = c.Moneda,
                    Total = c.Total,
                    IVAContabilidad = 0,
                    TipoDeComprobante = c.TipoDeComprobante,
                    MetodoPago = c.MetodoPago,
                    RetencionISR = retencionISR,
                    RetencionIVA = retencionIVA,
                    RetencionIEPS = retencionIEPS,
                    LugarExpedicion = c.LugarExpedicion,
                    TipoBaseIVA = tipoDominante.Tipo,
                    Importe = tipoDominante.Monto,
                    Base16 = base16,
                    IVA16 = iva16,
                    Base8 = base8,
                    IVA8 = iva8,
                    Base0 = base0,
                    IVA0 = 0,
                    BaseExenta = baseExenta,
                    IVAExento = 0,
                    Contabilidad = false,
                    Exento = false
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private bool EsGobierno(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                return false;
            string upper = nombre.ToUpperInvariant();
            return upper.Contains("GOBIERNO") ||
                   upper.Contains("SECRETARÍA") ||
                   upper.Contains("INSTITUTO") ||
                   upper.Contains("MUNICIPIO") ||
                   upper.Contains("IMSS") ||
                   upper.Contains("ISSSTE");
        }



        private void MostrarGrid(GridView gridView, object dataSource)
        {
            if (dataSource != null)
            {
                var lista = dataSource as List<CfdiModelo>;
                if (lista != null)
                {
                    gridView.DataSource = lista.OrderBy(x => x.RfcEmisor).ToList();
                }
                else
                {
                    gridView.DataSource = dataSource;
                }

                gridView.DataBind();
                gridView.Visible = true;
            }
            else
            {
                gridView.Visible = false;
            }
        }
        private CfdiArchivo LeerCfdiDesdeString(string xmlString)
        {
            try
            {
                // Opcional: desescapar si el XML viene con comillas escapadas
                string xml = System.Web.HttpUtility.HtmlDecode(xmlString);

                XmlSerializer serializer = new XmlSerializer(typeof(Comprobante), "http://www.sat.gob.mx/cfd/4");
                using (var reader = new StringReader(xml))
                {
                    Comprobante comprobante = (Comprobante)serializer.Deserialize(reader);

                    return new CfdiArchivo
                    {
                        Comprobante = comprobante,
                        SourceName = "manual.xml" // nombre opcional
                    };
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeJS("Error", $"No se pudo procesar el CFDI manual: {ex.Message}", "danger");
                return null;
            }
        }
        protected void BtnAplicarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                var conciliados = Session["ListaModeloContabilidadCfdiModelo"] as List<CfdiModelo>;
                var exentos = Session["ListaModeloExentaCfdiModelo"] as List<CfdiModelo>;
                var listaNoXML = Session["ListaModeloContabilidadNoXML"] as List<Entity.ModeloFactura>;
                var uuidsNoXML = new HashSet<string>(listaNoXML.Select(x => x.UUID?.ToUpperInvariant()), StringComparer.OrdinalIgnoreCase);



                var nuevosConciliados = new List<CfdiModelo>();
                var nuevosExentos = new List<CfdiModelo>();

                // Revisar GridView de conciliados
                for (int i = 0; i < GridViewCfdi.Rows.Count; i++)
                {
                    var row = GridViewCfdi.Rows[i];
                    var chk = row.FindControl("chkMoverExento") as CheckBox;
                    string nombreArchivo = GridViewCfdi.DataKeys[i].Value.ToString();

                    var cfdi = conciliados.FirstOrDefault(x => x.SourceName == nombreArchivo);
                    if (chk != null && chk.Checked && cfdi != null)
                    {
                        if (uuidsNoXML.Contains(nombreArchivo))
                        {

                            row.Visible = false;
                            conciliados.RemoveAll(x => x.SourceName?.ToUpperInvariant() == nombreArchivo);

                        }
                        else
                        {
                            cfdi.Exento = true;
                            cfdi.Contabilidad = false;
                            nuevosExentos.Add(cfdi);
                        }
                    }
                    else if (cfdi != null)
                    {
                        nuevosConciliados.Add(cfdi);
                    }
                }

                // Revisar GridView de exentos
                for (int i = 0; i < GridViewCfdi1.Rows.Count; i++)
                {
                    var row = GridViewCfdi1.Rows[i];
                    var chk = row.FindControl("chkMoverConciliado") as CheckBox;
                    string nombreArchivo = GridViewCfdi1.DataKeys[i].Value.ToString();

                    var cfdi = exentos.FirstOrDefault(x => x.SourceName == nombreArchivo);
                    if (chk != null && chk.Checked && cfdi != null)
                    {
                        cfdi.Exento = false;
                        cfdi.Contabilidad = true;
                        nuevosConciliados.Add(cfdi);
                    }
                    else if (cfdi != null)
                    {
                        nuevosExentos.Add(cfdi);
                    }
                }

                if (GridViewCfdi2.Visible)
                {
                    for (int i = 0; i < GridViewCfdi2.Rows.Count; i++)
                    {
                        var row = GridViewCfdi2.Rows[i];
                        var chk = row.FindControl("chkMoverConciliado") as CheckBox;
                        string nombreArchivo = GridViewCfdi2.DataKeys[i].Value.ToString();

                        var cfdiManual = listaNoXML.FirstOrDefault(x => x.UUID == nombreArchivo);
                        var cfdiArchivo = LeerCfdiDesdeString(cfdiManual.XMLFacSistema.ToString());
                        var datos = ObtenerDatosCfdi(cfdiArchivo.Comprobante, cfdiManual.UUID);
                        // var detalleCFDIConta = GenerarDetalleCfdi(datos);
                        if (chk != null && chk.Checked && cfdiManual != null)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(cfdiManual.ToString()))
                                {

                                    nuevosConciliados.Add(datos);
                                }
                                else
                                {
                                    MostrarMensajeJS("Advertencia", $"El XML está vacío para el archivo: {nombreArchivo}", "warning");
                                }
                            }
                            catch (Exception ex)
                            {
                                MostrarMensajeJS("Error", $"No se pudo procesar el XML de {nombreArchivo}: {ex.Message}", "danger");
                            }
                        }
                        else if (cfdiManual != null)
                        {
                            nuevosExentos.Add(datos);
                        }
                    }
                    GridViewCfdi2.Visible = false;
                    PanelExentosConta.Visible = false;
                }
                // Actualiza sesiones y recarga grids
                Session["ListaModeloContabilidadCfdiModelo"] = nuevosConciliados.OrderBy(x => x.RfcEmisor).ToList();
                Session["ListaModeloExentaCfdiModelo"] = nuevosExentos.OrderBy(x => x.RfcEmisor).ToList();
                ViewState["SumaSubTotalContabilidad"] = nuevosConciliados.Sum(x => x.SubTotal);
                ViewState["SumaTotalContabilidad"] = nuevosConciliados.Sum(x => x.Total);

                ViewState["sumaSubTotalexenta"] = nuevosExentos.Sum(x => x.SubTotal);
                ViewState["sumaTotalexenta"] = nuevosExentos.Sum(x => x.Total);
                MostrarGrid(GridViewCfdi, nuevosConciliados);
                MostrarGrid(GridViewCfdi1, nuevosExentos);
                if (conciliados == null || exentos == null)
                {
                    MostrarMensajeJS("Error", "Las listas de CFDIs se han perdido. Por favor, recargue los datos.", "danger");
                    return;
                }
                else
                {
                    MostrarMensajeJS("Éxito", "Cambios aplicados correctamente.", "success");
                }

            }
            catch (Exception ex)
            {
                MostrarMensajeJS("Error", "Ocurrió un error al aplicar los cambios: " + ex.Message, "danger");
            }
        }
        private static string BuildConcatenarFormulaTemplate(int hastaColIndex)
        {
            var partes = new List<string>(hastaColIndex * 2 - 1);
            for (int i = 1; i <= hastaColIndex; i++)
            {
                partes.Add($"{ColExcel(i)}{{ROW}}");
                if (i < hastaColIndex) partes.Add("\"|\"");
            }
            return $"=CONCATENAR({string.Join(",", partes)})";
        }
        protected void BtnExportarDetallado_Click(object sender, EventArgs e)
        {
            var listaXML = Session["ListaModeloContabilidadCfdiModelo"] as List<CfdiModelo>;
            var listaBase = listaXML?.Select(x => GenerarDetalleCfdi(x)).ToList() ?? new List<CfdiModeloDetallado>();



            if (listaBase == null || listaBase.Count == 0)
            {
                MostrarMensajeJS("Error", "No hay CFDIs conciliados ni PDFs para exportar detalle.", "danger");
                return;
            }

            // ✅ Agrupamos y sumamos, redondeando recién al final por RFC
            var listaAgrupadaPorRFC = listaBase
                 .GroupBy(x => x.RFC?.Trim().ToUpper() ?? "SIN_RFC")
                 .Select(g =>
                 {
                     var modelo = new CfdiModeloDetallado
                     {
                         RFC = g.Key,
                         TipoTercero = "04",
                         TipoOperacion = g.First().TipoOperacion,
                         ActividadesPagadasRFN = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ActividadesPagadasRFN)).ToString()),
                         IVA_Acreditable_RF_Norte = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Acreditable_RF_Norte)).ToString()),
                         ActividadesPagadasResto = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ActividadesPagadasResto)).ToString()),
                         IVA_Acreditable_Resto = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Acreditable_Resto)).ToString()),
                         Valor0 = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.Valor0)).ToString()),
                         ValorExento = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ValorExento)).ToString()),
                         IVA_Retenido_Contribuyente = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Retenido_Contribuyente)).ToString()),
                         EfectosFiscales = "01"
                     };

                     modelo.LineaTXT = BuildConcatenarFormulaTemplate(54);

                     return modelo;
                 })
                 .ToList();

            var listaDesdePdf = Session["ListaCfdiModeloDetalladoPDF"] as List<CfdiModeloDetallado>;

            var listaAgrupada = listaDesdePdf
                 ?.GroupBy(x => x.IDFiscal?.Trim().ToUpper() ?? "SIN_IDFISCAL")
                 .Select(g =>
                 {
                     var modelo = new CfdiModeloDetallado
                     {
                         IDFiscal = g.Key,
                         RFC = g.First().RFC,
                         TipoTercero = "05",
                         TipoOperacion = g.First().TipoOperacion,
                         NombreExtranjero = g.First().NombreExtranjero,
                         PaisResidencia = g.First().PaisResidencia,
                         JurisdiccionFiscal = g.First().JurisdiccionFiscal,
                         ActividadesPagadasRFN = SumarYRedondear(g.Select(x => x.ActividadesPagadasRFN)),
                         IVA_Acreditable_RF_Norte = SumarYRedondear(g.Select(x => x.IVA_Acreditable_RF_Norte)),
                         ActividadesPagadasResto = SumarYRedondear(g.Select(x => x.ActividadesPagadasResto)),
                         IVA_Acreditable_Resto = SumarYRedondear(g.Select(x => x.IVA_Acreditable_Resto)),
                         ValorExento = SumarYRedondear(g.Select(x => x.ValorExento)),
                         Valor0 = SumarYRedondear(g.Select(x => x.Valor0)),
                         IVA_Retenido_Contribuyente = SumarYRedondear(g.Select(x => x.IVA_Retenido_Contribuyente)),
                         EfectosFiscales = "01"
                     };

                     modelo.LineaTXT = BuildConcatenarFormulaTemplate(54);

                     return modelo;
                 })
                 .ToList();

            if (listaDesdePdf != null && listaDesdePdf.Count > 0)
            {
                listaAgrupadaPorRFC.AddRange(listaAgrupada);
            }
            // Exportar
            ExportarDatosAExcel2(listaAgrupadaPorRFC, "Diot.xls");
        }

        private string SumarYRedondear(IEnumerable<string> valores)
        {
            decimal total = 0;

            foreach (var valor in valores)
            {
                string limpio = (valor ?? "").Replace(",", "").Trim();
                decimal numero;

                if (decimal.TryParse(limpio, out numero))
                {
                    total += numero;
                }
            }

            return RedondearDecimal(total.ToString());
        }

        private string RedondearDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor; // si es null, vacío o espacios: lo deja igual

            valor = valor.Replace(",", "").Trim();

            decimal numero;
            if (!decimal.TryParse(valor, out numero))
                return valor; // si no se puede convertir a decimal: lo deja igual

            // Redondeo bancario
            decimal redondeado = RedondeoBancario(numero);

            return redondeado.ToString("0");
        }
        public static decimal RedondeoBancario(decimal numero)
        {
            // Redondeo bancario: mitad va al par más cercano
            return Math.Round(numero, 0, MidpointRounding.ToEven);
        }
        private decimal ConvertirADecimalSeguro(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return 0;

            valor = valor.Replace(",", "").Trim();

            decimal resultado;
            decimal.TryParse(valor, out resultado);

            return resultado;
        }


        [System.Web.Services.WebMethod]
        public static void ActualizarListaPDF(List<DatoPDF> lista)
        {

            // Guardar la lista de DatoPDF en sesión
            HttpContext.Current.Session["ListaDatosPDF"] = lista;

            // Crear una nueva lista de CfdiModeloDetallado
            var listaCfdi = new List<CfdiModeloDetallado>();

            foreach (var dato in lista)
            {

                decimal importe = 0;
                decimal iva = 0;

                string base16 = "0", base8 = "0", base0 = "0", baseExenta = "0", iva16 = "0", iva8 = "0";
                decimal.TryParse(dato.Importe?.Replace(",", ""), out importe);
                decimal.TryParse(dato.IVA?.Replace(",", ""), out iva);

                string tasa = (dato.TasaIVA ?? "").Trim().Replace("%", "").Replace(",", ".");


                if (tasa == "16" || tasa == "16.0" || tasa == "16.00")
                {
                    base16 = FormatearEntero(importe);
                    iva16 = FormatearEntero(iva);
                }
                else if (tasa == "8" || tasa == "8.0" || tasa == "8.00")
                {
                    base8 = FormatearEntero(importe);
                    iva8 = FormatearEntero(iva);
                }
                else if (tasa.Equals("exento", StringComparison.OrdinalIgnoreCase) ||
                         tasa.Equals("exenta", StringComparison.OrdinalIgnoreCase))
                {
                    baseExenta = FormatearEntero(importe);
                }
                else if (tasa == "0")
                {
                    base0 = FormatearEntero(importe);
                }

                if ((dato.NombreExtranjero ?? "").Equals("Meta Platforms Ireland Limited", StringComparison.OrdinalIgnoreCase))
                {
                    baseExenta = FormatearEntero(importe);
                    base0 = "0";
                }

                var modelo = new CfdiModeloDetallado
                {
                    TipoTercero = dato.TipoTercero,
                    TipoOperacion = dato.TipoOperacion,
                    IDFiscal = dato.IDFiscal,
                    NombreExtranjero = dato.NombreExtranjero,
                    PaisResidencia = dato.PaisResidencia,
                    EfectosFiscales = dato.EfectosFiscales,
                    ActividadesPagadasResto = importe.ToString(),
                    IVA_Acreditable_Resto = iva.ToString(),
                    IVA_Acreditable_RF_Sur = "0",
                    ValorExento = baseExenta,
                    Exento_Import = baseExenta,
                    IVA_Retenido_Contribuyente = "0",
                    Valor0 = base0
                };

                listaCfdi.Add(modelo);
            }

            HttpContext.Current.Session["ListaCfdiModeloDetalladoPDF"] = listaCfdi;
        }

        private string ValorTexto(decimal? valor)
        {
            return valor.HasValue && valor.Value != 0 ? valor.Value.ToString("0.00") : "";
        }


        private string GenerarDetalleDesdePDF(DatoPDF pdf)
        {
            // Debes ajustar el formato de línea según lo que espera el TXT
            return $"{pdf.RFC}|{pdf.Importe}|{pdf.TipoOperacion}|{pdf.NombreExtranjero}|{pdf.PaisResidencia}|{pdf.IVA}|{pdf.TotalPDF}";
        }


        protected void BtnExportarTxtDetallado_Click(object sender, EventArgs e)
        {
            var listaXML = Session["ListaModeloContabilidadCfdiModelo"] as List<CfdiModelo>;
            var listaBase = listaXML?.Select(x => GenerarDetalleCfdi(x)).ToList() ?? new List<CfdiModeloDetallado>();



            if (listaBase == null || listaBase.Count == 0)
            {
                MostrarMensajeJS("Error", "No hay CFDIs conciliados ni PDFs para exportar detalle.", "danger");
                return;
            }

            // ✅ Agrupamos y sumamos, redondeando recién al final por RFC
            var listaAgrupadaPorRFC = listaBase
                 .GroupBy(x => x.RFC?.Trim().ToUpper() ?? "SIN_RFC")
                 .Select(g =>
                 {
                     var modelo = new CfdiModeloDetallado
                     {
                         RFC = g.Key,
                         TipoTercero = "04",
                         TipoOperacion = g.First().TipoOperacion,
                         ActividadesPagadasRFN = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ActividadesPagadasRFN)).ToString()),
                         IVA_Acreditable_RF_Norte = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Acreditable_RF_Norte)).ToString()),
                         ActividadesPagadasResto = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ActividadesPagadasResto)).ToString()),
                         IVA_Acreditable_Resto = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Acreditable_Resto)).ToString()),
                         Valor0 = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.Valor0)).ToString()),
                         ValorExento = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.ValorExento)).ToString()),
                         IVA_Retenido_Contribuyente = RedondearDecimal(g.Sum(x => ConvertirADecimalSeguro(x.IVA_Retenido_Contribuyente)).ToString()),
                         EfectosFiscales = "01"
                     };

                     modelo.LineaTXT = BuildConcatenarFormulaTemplate(54);

                     return modelo;
                 })
                 .ToList();

            var listaDesdePdf = Session["ListaCfdiModeloDetalladoPDF"] as List<CfdiModeloDetallado>;

            var listaAgrupada = listaDesdePdf
                 ?.GroupBy(x => x.IDFiscal?.Trim().ToUpper() ?? "SIN_IDFISCAL")
                 .Select(g =>
                 {
                     var modelo = new CfdiModeloDetallado
                     {
                         IDFiscal = g.Key,
                         RFC = g.First().RFC,
                         TipoTercero = "05",
                         TipoOperacion = g.First().TipoOperacion,
                         NombreExtranjero = g.First().NombreExtranjero,
                         PaisResidencia = g.First().PaisResidencia,
                         JurisdiccionFiscal = g.First().JurisdiccionFiscal,
                         ActividadesPagadasRFN = SumarYRedondear(g.Select(x => x.ActividadesPagadasRFN)),
                         IVA_Acreditable_RF_Norte = SumarYRedondear(g.Select(x => x.IVA_Acreditable_RF_Norte)),
                         ActividadesPagadasResto = SumarYRedondear(g.Select(x => x.ActividadesPagadasResto)),
                         IVA_Acreditable_Resto = SumarYRedondear(g.Select(x => x.IVA_Acreditable_Resto)),
                         ValorExento = SumarYRedondear(g.Select(x => x.ValorExento)),
                         Valor0 = SumarYRedondear(g.Select(x => x.Valor0)),
                         IVA_Retenido_Contribuyente = SumarYRedondear(g.Select(x => x.IVA_Retenido_Contribuyente)),
                         EfectosFiscales = "01"
                     };

                     modelo.LineaTXT = BuildConcatenarFormulaTemplate(54);

                     return modelo;
                 })
                 .ToList();

            if (listaDesdePdf != null && listaDesdePdf.Count > 0)
            {
                listaAgrupadaPorRFC.AddRange(listaAgrupada);
            }

            // Construir las líneas para el TXT
            List<string> lineas = new List<string>(listaAgrupadaPorRFC.Count);

            foreach (var item in listaAgrupadaPorRFC)
            {
                string linea = GenerarLineaDetalladaTXT2(item);
                lineas.Add(linea);
            }

            ExportarTxtDesdeLista(lineas, "DIOT.txt");
        }


        private static string ColExcel(int index1Based)
        {
            var col = "";
            while (index1Based > 0)
            {
                int rem = (index1Based - 1) % 26;
                col = (char)('A' + rem) + col;
                index1Based = (index1Based - 1) / 26;
            }
            return col;
        }

        // Genera: =CONCATENAR(A{row},"|",B{row},... ,{hastaCol}{row})
        private static string GenerarFormulaLineaTXTComas(int row, int hastaColIndex)
        {
            var partes = new List<string>(hastaColIndex * 2 - 1);
            for (int i = 1; i <= hastaColIndex; i++)
            {
                partes.Add($"{ColExcel(i)}{row}");
                if (i < hastaColIndex) partes.Add("\"|\"");
            }
            return $"=CONCATENAR({string.Join(",", partes)})";
        }

        private string GenerarLineaDetalladaTXT(int row /* fila Excel */, CfdiModeloDetallado detalle)
        {
            // Si tu última columna es BB -> 54. Ajusta si tienes más/menos columnas.
            const int hastaBB = 54;
            return GenerarFormulaLineaTXTComas(row, hastaBB);
        }


        private string GenerarLineaDetalladaTXT2(CfdiModeloDetallado detalle)
        {
            var campos = new List<string>
            {
                detalle.TipoTercero,
                detalle.TipoOperacion,
                detalle.RFC,
                detalle.IDFiscal,
                detalle.NombreExtranjero,
                detalle.PaisResidencia,
                detalle.JurisdiccionFiscal,

                RedondearDecimal(detalle.ActividadesPagadasRFN),
                RedondearDecimal(detalle.DescuentosRFN),
                RedondearDecimal(detalle.ActividadesPagadasRFS),
                RedondearDecimal(detalle.DescuentosRFS),
                RedondearDecimal(detalle.ActividadesPagadasResto),
                RedondearDecimal(detalle.DescuentosResto),

                RedondearDecimal(detalle.Importaciones16),
                RedondearDecimal(detalle.BonificacionesImportaciones),
                RedondearDecimal(detalle.ImportacionesIntangibles),
                RedondearDecimal(detalle.BonificacionesImportacionesIntangibles),

                RedondearDecimal(detalle.IVA_Acreditable_RF_Norte),
                RedondearDecimal(detalle.Proporcion_RF_Norte),
                RedondearDecimal(detalle.IVA_Acreditable_RF_Sur),
                RedondearDecimal(detalle.Proporcion_RF_Sur),
                RedondearDecimal(detalle.IVA_Acreditable_Resto),
                RedondearDecimal(detalle.Proporcion_Resto),
                RedondearDecimal(detalle.IVA_Acreditable_Importacion),
                RedondearDecimal(detalle.Proporcion_Importacion),
                RedondearDecimal(detalle.IVA_Acreditable_Intangibles),
                RedondearDecimal(detalle.Proporcion_Intangibles),

                RedondearDecimal(detalle.IVA_NoAcred_RF_Norte),
                RedondearDecimal(detalle.NoRequisitos_RF_Norte),
                RedondearDecimal(detalle.Exento_RF_Norte),
                RedondearDecimal(detalle.NoObjeto_RF_Norte),

                RedondearDecimal(detalle.IVA_NoAcred_RF_Sur),
                RedondearDecimal(detalle.NoRequisitos_RF_Sur),
                RedondearDecimal(detalle.Exento_RF_Sur),
                RedondearDecimal(detalle.NoObjeto_RF_Sur),

                RedondearDecimal(detalle.IVA_NoAcred_Resto),
                RedondearDecimal(detalle.NoRequisitos_Resto),
                RedondearDecimal(detalle.Exento_Resto),
                RedondearDecimal(detalle.NoObjeto_Resto),

                RedondearDecimal(detalle.IVA_NoAcred_Import),
                RedondearDecimal(detalle.NoRequisitos_Import),
                RedondearDecimal(detalle.Exento_Import),
                RedondearDecimal(detalle.NoObjeto_Import),

                RedondearDecimal(detalle.IVA_NoAcred_Intangibles),
                RedondearDecimal(detalle.NoRequisitos_Intangibles),
                RedondearDecimal(detalle.Exento_Intangibles),
                RedondearDecimal(detalle.NoObjeto_Intangibles),

                RedondearDecimal(detalle.IVA_Retenido_Contribuyente),
                RedondearDecimal(detalle.ValorImportacionExento),
                RedondearDecimal(detalle.ValorExento),
                RedondearDecimal(detalle.Valor0),
                RedondearDecimal(detalle.ValorNoObjetoTerritorio),
                RedondearDecimal(detalle.ValorNoObjetoSinEstablecimiento),

                detalle.EfectosFiscales
            };
            return string.Join("|", campos.Select(c => string.IsNullOrWhiteSpace(c) ? "" : c.Trim()));
        }

        private void ExportarTxtDesdeLista(List<string> lineas, string nombreArchivo)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo);
            Response.Charset = "";
            Response.ContentType = "text/plain";

            StringBuilder sb = new StringBuilder();
            foreach (var linea in lineas)
                sb.AppendLine(linea);

            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        private decimal ParseDecimal(string valor)
        {
            decimal d = 0;
            decimal.TryParse(valor?.Replace(",", ""), out d);
            return d;
        }

        private static string FormatearEntero(decimal valor)
        {
            return valor == 0 ? "" : Math.Round(valor, 0, MidpointRounding.AwayFromZero).ToString("0");
        }

        protected void BtnAgregarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                string json = HiddenJsonPDF.Value;

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var dato = Newtonsoft.Json.JsonConvert.DeserializeObject<DatoPDF>(json);
                    decimal importe = 0;
                    decimal iva = 0;

                    string base16 = "0", base8 = "0", base0 = "0", baseExenta = "0", iva16 = "0", iva8 = "0";
                    decimal.TryParse(dato.Importe?.Replace(",", ""), out importe);
                    decimal.TryParse(dato.IVA?.Replace(",", ""), out iva);

                    string tasa = (dato.TasaIVA ?? "").Trim().Replace("%", "").Replace(",", ".");

                    if (tasa == "16" || tasa == "16.0" || tasa == "16.00")
                    {
                        base16 = FormatearEntero(importe);
                        iva16 = FormatearEntero(iva);
                    }
                    else if (tasa == "8" || tasa == "8.0" || tasa == "8.00")
                    {
                        base8 = FormatearEntero(importe);
                        iva8 = FormatearEntero(iva);
                    }
                    else if (tasa.Equals("exento", StringComparison.OrdinalIgnoreCase) ||
                             tasa.Equals("exenta", StringComparison.OrdinalIgnoreCase))
                    {
                        baseExenta = FormatearEntero(importe);
                    }
                    else if (tasa == "0")
                    {
                        base0 = FormatearEntero(importe);
                    }

                    if (dato.NombreExtranjero.Equals("Meta Platforms Ireland Limited"))
                    {
                        baseExenta = FormatearEntero(importe);
                        base0 = "0";
                    }
                    // Agregar a la lista de DatoPDF
                    var lista = Session["ListaDatosPDF"] as List<DatoPDF>;
                    if (lista == null)
                        lista = new List<DatoPDF>();

                    lista.Add(dato);
                    Session["ListaDatosPDF"] = lista;

                    HiddenJsonPDFLista.Value = new JavaScriptSerializer().Serialize(lista);
                    // Construir CfdiModeloDetallado
                    var modelo = new CfdiModeloDetallado
                    {
                        TipoTercero = dato.TipoTercero,
                        TipoOperacion = dato.TipoOperacion,
                        //RFC = dato.RFC,
                        IDFiscal = dato.IDFiscal,
                        NombreExtranjero = dato.NombreExtranjero,
                        PaisResidencia = dato.PaisResidencia,
                        EfectosFiscales = dato.EfectosFiscales,
                        ActividadesPagadasResto = base16.ToString(),
                        IVA_Acreditable_Resto = iva16.ToString(),
                        ActividadesPagadasRFN = base8.ToString(),
                        IVA_Acreditable_RF_Norte = iva8.ToString(),
                        ValorExento = baseExenta.ToString(),
                        Exento_Import = baseExenta,
                        IVA_Retenido_Contribuyente = "0",
                        Valor0 = base0,
                    };


                    // Agregar a la lista CfdiModeloDetallado
                    var listaCfdi = Session["ListaCfdiModeloDetalladoPDF"] as List<CfdiModeloDetallado>;
                    if (listaCfdi == null)
                        listaCfdi = new List<CfdiModeloDetallado>();

                    listaCfdi.Add(modelo);
                    Session["ListaCfdiModeloDetalladoPDF"] = listaCfdi;

                    MostrarMensajeJS("Éxito", "Los Datos Se han agregado a la lista", "success");
                }
                else
                {
                    MostrarMensajeJS("Advertencia", "No se encontraron datos para agregar.", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeJS("Error", "Error al agregar PDF: " + ex.Message, "danger");

            }
        }


        protected void rblTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblImportarTitulo.Text = $"Importar información {rblTipo.SelectedValue}";

            // Limpia todo lo relacionado con el tipo previo
            LimpiarResultadosPorCambioTipo();

            // Mensaje opcional
            MostrarMensajeJS("Listo",
                $"Cambiaste a {rblTipo.SelectedValue}. Se limpió la vista para evitar mezclar datos.",
                "info");
        }

        private void LimpiarResultadosPorCambioTipo()
        {
            // 1) Limpiar sesiones usadas en la vista
            Session.Remove("ListaModeloContabilidad");
            Session.Remove("ListaModeloExenta");
            Session.Remove("ListaModeloContabilidadNoXML");
            Session.Remove("ListaModeloContabilidadCfdiModelo");
            Session.Remove("ListaModeloExentaCfdiModelo");
            Session.Remove("ListaCfdiModeloDetalladoPDF");
            Session.Remove("ListaDatosPDF");

            // 2) Limpiar ViewState de totales
            ViewState.Remove("SumaSubTotalContabilidad");
            ViewState.Remove("SumaTotalContabilidad");
            ViewState.Remove("sumaSubTotalexenta");
            ViewState.Remove("sumaTotalexenta");

            // 3) Limpiar/ocultar grids y paneles
            ResetGrid(GridViewCfdi);
            ResetGrid(GridViewCfdi1);
            ResetGrid(GridViewCfdi2);

            if (PanelResultados != null) PanelResultados.Visible = false;
            if (PanelExentosConta != null) PanelExentosConta.Visible = false;

            // 4) Botones de exportación
            BtnDescargarConciliados.Visible = false;
            BtnDescargarExentos.Visible = false;
            BtnDescargarExentosConta.Visible = false;

            // 5) Limpiar hidden/JSON de PDFs manuales
            HiddenJsonPDF.Value = string.Empty;
            HiddenJsonPDFLista.Value = "[]";

            // 6) Asegurar que el FileUpload quede vacío en el cliente
            // (en postback normal se vacía solo; esto ayuda si hay UpdatePanel)
            ScriptManager.RegisterStartupScript(
                this, GetType(), "clearFileUpload",
                $"var fu=document.getElementById('{FileUploadCFDI.ClientID}'); if(fu) fu.value='';",
                true
            );
            // 1) Limpiar sesiones usadas en la vista
            Session.Remove("ResumenIngresosCFDIFactorajeFinanciero");
            Session.Remove("ResumenIngresosCFDICuentaCorriente");
            Session.Remove("ListaDatosPDF");
            Session.Remove("ListaCfdiModeloDetalladoPDF");

            // 2) Limpiar ViewState de totales
            ViewState.Remove("TotFactoraje");
            ViewState.Remove("TotCtaCte");

            // 3) Limpiar/ocultar grids y paneles
            ResetGrid(GridViewFactoraje);
            ResetGrid(GridViewCuentaCorriente);
            ResetGrid(XMLNOCONTA);

            if (PanelCFDIIngresos != null) PanelCFDIIngresos.Visible = false;

            // 4) Botones de exportación y PDF
            if (BtnPDFXMLNOCONTA != null) BtnPDFXMLNOCONTA.Visible = false;
            if (exportarEXCEL2 != null) exportarEXCEL2.Visible = false;

            // 5) Limpiar hidden/JSON de PDFs manuales
            HiddenJsonPDF.Value = string.Empty;
            HiddenJsonPDFLista.Value = "[]";

            // 6) Asegurar que el FileUpload quede vacío en el cliente
            ScriptManager.RegisterStartupScript(
                this, GetType(), "clearFileUploadIngresos",
                $"var fu=document.getElementById('{FileUploadCFDI.ClientID}'); if(fu) fu.value='';",
                true
            );
            
            // 7) Rehabilitar upload y ocultar errores previos
            FileUploadCFDI.Enabled = true;
        }

        protected void BtnExportarExcel_Click(object sender, EventArgs e)
        {
            ExportarGridsAExcel("ReporteCFDI.xls", GridViewCuentaCorriente, GridViewFactoraje);
        }

        private void ExportarGridsAExcel(string nombreArchivo, params GridView[] grids)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo);
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.Default;

            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                foreach (GridView grid in grids)
                {
                    if (grid.Rows.Count > 0)
                    {
                        // título
                        htw.Write("<b>" + grid.ID + "</b><br/><br/>");

                        // renderiza el GridView
                        grid.GridLines = GridLines.Both;
                        grid.HeaderStyle.Font.Bold = true;
                        grid.RenderControl(htw);

                        // espacio entre tablas
                        htw.Write("<br/><br/>");
                    }
                }

                Response.Write(sw.ToString());
                Response.End();
            }
        }
        

        private void ResetGrid(GridView gv)
        {
            if (gv == null) return;
            gv.DataSource = null;
            gv.DataBind();
            gv.Visible = false;
        }

        public class UuidInfo
        {
            public string Abonado { get; set; }
            public string uuid { get; set; }
            public string tipoOperacion { get; set; }
        }
        public class CfdiModelo
        {
            public string SourceName { get; set; }
            public string RfcEmisor { get; set; }
            public string NombreEmisor { get; set; }
            public string UsoCFDI { get; set; }
            public string Descripciones { get; set; }
            public string TrasladosDetalle { get; set; }
            public string Serie { get; set; }
            public string Folio { get; set; }
            public DateTime Fecha { get; set; }
            public string FormaPago { get; set; }
            public decimal TotalImpuestosTrasladados { get; set; }
            public decimal TotalImpuestosRetenidos { get; set; }
            public decimal RetencionISR { get; set; }
            public decimal RetencionIVA { get; set; }
            public decimal RetencionIEPS { get; set; }
            public decimal retencion { get; set; }
            public decimal SubTotal { get; set; }
            public string Moneda { get; set; }
            public decimal Total { get; set; }
            public decimal IVAContabilidad { get; set; }
            public string TipoDeComprobante { get; set; }
            public string MetodoPago { get; set; }
            public string LugarExpedicion { get; set; }
            public string TipoBaseIVA { get; set; }
            public decimal Importe { get; set; }
            public decimal Base16 { get; set; }
            public decimal Base8 { get; set; }
            public decimal Base0 { get; set; }
            public decimal BaseExenta { get; set; }
            public decimal IVA16 { get; set; }
            public decimal IVA8 { get; set; }
            public decimal IVA0 { get; set; }
            public decimal IVAExento { get; set; }
            public Boolean Contabilidad { get; set; }
            public Boolean Exento { get; set; }

            public string TipoTercero { get; set; } = "04";
            public string TipoOperacion { get; set; } = "85";

            //  propiedades requeridas para calculos en DIOT
            public string IdFiscalExtranjero { get; set; }
            public string NombreExtranjero { get; set; }
            public string PaisResidencia { get; set; }
            public string JurisdiccionFiscal { get; set; }

            public decimal ActividadesPagadasRFN { get; set; }
            public decimal DescuentosRFN { get; set; }
            public decimal ActividadesPagadasRestoPais { get; set; }

            public decimal IVAFrontera { get; set; }
            public decimal IVARestoPais { get; set; }
            public decimal IVARetenido { get; set; }

            public decimal ValorExento { get; set; }
            public decimal ValorCero { get; set; }

            //datos necesarios para egresos

            public decimal InteresOrdinarioBase { get; set; }
            public decimal InteresOrdinarioIVA { get; set; }
            public decimal InteresMoratorioBase { get; set; }
            public decimal InteresMoratorioIVA { get; set; }
            public decimal ComisionBase { get; set; }
            public decimal ComisionIVA { get; set; }


        }

        [Serializable]
        public class DatoPDF
        {
            public string TipoTercero { get; set; }
            public string TipoOperacion { get; set; }
            public string RFC { get; set; }
            public string IDFiscal { get; set; }
            public string NombreExtranjero { get; set; }
            public string PaisResidencia { get; set; }
            public string Importe { get; set; }
            public string TasaIVA { get; set; }
            public string IVA { get; set; }
            public string TotalPDF { get; set; }
            public string EfectosFiscales { get; set; }
        }
        public class CfdiModeloDetallado
        {
            public string TipoTercero { get; set; }                       // 01
            public string TipoOperacion { get; set; }                     // 02
            public string RFC { get; set; }                               // 03
            public string IDFiscal { get; set; }                          // 04
            public string NombreExtranjero { get; set; }                  // 05
            public string PaisResidencia { get; set; }                    // 06
            public string JurisdiccionFiscal { get; set; }                // 07
            public string ActividadesPagadasRFN { get; set; }             // 08
            public string DescuentosRFN { get; set; }                     // 09
            public string ActividadesPagadasRFS { get; set; }             // 10
            public string DescuentosRFS { get; set; }                     // 11
            public string ActividadesPagadasResto { get; set; }           // 12
            public string DescuentosResto { get; set; }                   // 13
            public string Importaciones16 { get; set; }                   // 14
            public string BonificacionesImportaciones { get; set; }       // 15
            public string ImportacionesIntangibles { get; set; }          // 16
            public string BonificacionesImportacionesIntangibles { get; set; } // 17
            public string IVA_Acreditable_RF_Norte { get; set; }          // 18
            public string Proporcion_RF_Norte { get; set; }               // 19
            public string IVA_Acreditable_RF_Sur { get; set; }            // 20
            public string Proporcion_RF_Sur { get; set; }                 // 21
            public string IVA_Acreditable_Resto { get; set; }             // 22
            public string Proporcion_Resto { get; set; }                  // 23
            public string IVA_Acreditable_Importacion { get; set; }       // 24
            public string Proporcion_Importacion { get; set; }            // 25
            public string IVA_Acreditable_Intangibles { get; set; }       // 26
            public string Proporcion_Intangibles { get; set; }            // 27
            public string IVA_NoAcred_RF_Norte { get; set; }              // 28
            public string NoRequisitos_RF_Norte { get; set; }             // 29
            public string Exento_RF_Norte { get; set; }                   // 30
            public string NoObjeto_RF_Norte { get; set; }                 // 31
            public string IVA_NoAcred_RF_Sur { get; set; }                // 32
            public string NoRequisitos_RF_Sur { get; set; }               // 33
            public string Exento_RF_Sur { get; set; }                     // 34
            public string NoObjeto_RF_Sur { get; set; }                   // 35
            public string IVA_NoAcred_Resto { get; set; }                 // 36
            public string NoRequisitos_Resto { get; set; }                // 37
            public string Exento_Resto { get; set; }                      // 38
            public string NoObjeto_Resto { get; set; }                    // 39
            public string IVA_NoAcred_Import { get; set; }                // 40
            public string NoRequisitos_Import { get; set; }               // 41
            public string Exento_Import { get; set; }                     // 42
            public string NoObjeto_Import { get; set; }                   // 43
            public string IVA_NoAcred_Intangibles { get; set; }           // 44
            public string NoRequisitos_Intangibles { get; set; }          // 45
            public string Exento_Intangibles { get; set; }                // 46
            public string NoObjeto_Intangibles { get; set; }              // 47
            public string IVA_Retenido_Contribuyente { get; set; }        // 48
            public string ValorImportacionExento { get; set; }            // 49
            public string ValorExento { get; set; }                       // 50
            public string Valor0 { get; set; }                            // 51
            public string ValorNoObjetoTerritorio { get; set; }           // 52
            public string ValorNoObjetoSinEstablecimiento { get; set; }   // 53
            public string EfectosFiscales { get; set; }                   // 54
            public string LineaTXT { get; set; } // Columna final


        }



    }

     public class ResumenIngresoRow
    {
        public string UUID { get; set; }
        public string RfcReceptor { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Tasa { get; set; }

        public decimal IngresoExento { get; set; }
        public decimal IngresoGravado { get; set; }
        public decimal IVA_CFDI { get; set; }
        public decimal TotalCFDI { get; set; }

        public string TipoDeComprobante { get; set; }
        public string EstatusComprobante { get; set; }
        public DateTime? FechaCancelacion { get; set; }

        public decimal InteresOrdinario { get; set; }   // 1104-1
        public decimal InteresMoratorio { get; set; }   // 1104-2
        public decimal Comision { get; set; }           // 1104-4
        public decimal IVA_1104 { get; set; }

        public decimal InteresOrdinario2 { get; set; }
        public decimal InteresMoratorio2 { get; set; }
        public decimal Comision2 { get; set; }
        public decimal IVA_2 { get; set; }


        public decimal DifInteresOrdinario2 { get; set; }
        public decimal DifInteresMoratorio2 { get; set; }
        public decimal DifComision2 { get; set; }
        public decimal DifIVA_2 { get; set; }
    }

    public enum TipoCargo
    {
        Ordinario,
        Moratorio,
        Comision,
        Otro
    }

    public sealed class InteresPartida
    {
        public string Descripcion { get; set; }
        public decimal Base { get; set; }
        public decimal IVA { get; set; }
        public string TipoFactor { get; set; }   // "Tasa" / "Exento"
        public decimal TasaOCuota { get; set; }  // 0.16, 0.08, 0.0
        public TipoCargo TipoDetectado { get; set; }
    }

    public sealed class InteresesSeparados
    {
        public decimal BaseInteresOrdinario { get; set; }
        public decimal IVAInteresOrdinario { get; set; }

        public decimal BaseInteresMoratorio { get; set; }
        public decimal IVAInteresMoratorio { get; set; }

        public decimal BaseComision { get; set; }
        public decimal IVAComision { get; set; }

        public decimal BaseOtros { get; set; }
        public decimal IVAOtros { get; set; }

        public List<InteresPartida> Partidas { get; private set; } = new List<InteresPartida>();
    }


    public static class CfdiInteresHelper
    {
        public static InteresesSeparados SepararIntereses(Comprobante c)
        {
            var res = new InteresesSeparados();
            if (c?.Conceptos == null || c.Conceptos.Count == 0) return res;

            foreach (var concepto in c.Conceptos)
            {
                // 1) Clasificación por descripción
                var tipo = ClasificarPorDescripcion(concepto.Descripcion, c.Emisor.Nombre.ToString());

                // 2) Base/IVA del concepto según traslados
                var dato = CalcularBaseIvaDeConcepto(concepto);

                // 3) Guardar detalle
                res.Partidas.Add(new InteresPartida
                {
                    Descripcion = concepto.Descripcion,
                    Base = dato.Base,
                    IVA = dato.IVA,
                    TipoFactor = dato.TipoFactor,
                    TasaOCuota = dato.TasaOCuota,
                    TipoDetectado = tipo
                });

                // 4) Acumular por tipo
                switch (tipo)
                {
                    case TipoCargo.Ordinario:
                        res.BaseInteresOrdinario += dato.Base;
                        res.IVAInteresOrdinario += dato.IVA;
                        break;
                    case TipoCargo.Moratorio:
                        res.BaseInteresMoratorio += dato.Base;
                        res.IVAInteresMoratorio += dato.IVA;
                        break;
                    case TipoCargo.Comision:
                        res.BaseComision += dato.Base;
                        res.IVAComision += dato.IVA;
                        break;
                    default:
                        res.BaseOtros += dato.Base;
                        res.IVAOtros += dato.IVA;
                        break;
                }
            }

            return res;
        }

        // *** SIN TUPLAS ***
        private static ResultadoCalculo CalcularBaseIvaDeConcepto(Concepto concepto)
        {
            decimal baseSum = 0m, ivaSum = 0m;
            string tipoFactor = "";
            decimal tasa = 0m;

            var traslados = concepto?.Impuestos?.Traslados;
            if (traslados != null && traslados.Count > 0)
            {
                foreach (var t in traslados)
                {
                    // Sumar siempre Base (inclusive exento)
                    baseSum += t.Base;

                    tipoFactor = t.TipoFactor; // "Tasa" / "Exento"
                    tasa = t.TasaOCuota;       // 0.16, 0.08, 0.0

                    if (string.Equals(t.TipoFactor, "Tasa", StringComparison.OrdinalIgnoreCase))
                        ivaSum += t.Importe;
                }
            }
            else
            {
                baseSum = concepto?.Importe ?? 0m;
                ivaSum = 0m;
                tipoFactor = "";
                tasa = 0m;
            }

            return new ResultadoCalculo
            {
                Base = baseSum,
                IVA = ivaSum,
                TipoFactor = tipoFactor,
                TasaOCuota = tasa
            };
        }

        private static TipoCargo ClasificarPorDescripcion(string descripcionOriginal, string emisor)
        {
            var d = (descripcionOriginal ?? "").Trim();
            var e = (emisor ?? "").Trim();
            if (d.Length == 0) return TipoCargo.Otro;

            var s = QuitarAcentosYUpper(d);

            if (s.Contains("INTERES ORDIN") || s.Contains("INT ORDIN") || s.Contains("PAGO DE INTERESES") || s.Contains("COMISION POR SERVICIOS ADMINISTRATIVOS") || s.Contains("INTERES POR SERVICIOS ") || s.Contains("INTERES POR PRESTAMO"))
                return TipoCargo.Ordinario;

            if (s.Contains("INTERES MORATOR") || s.Contains("INT MORATOR") || s.Contains("MORATORIO")  || s.Contains("COMISION ADICIONAL POR SERVICIOS") || s.Contains("INTERES ADICIONAL POR SERVICIOS "))
                return TipoCargo.Moratorio;
            if (e == "BALOR DISPERSORA")
            {
                if (s.Contains("COMISION"))
                    return TipoCargo.Comision;
            }


            return TipoCargo.Otro;
        }


       

        private static string QuitarAcentosYUpper(string input)
        {
            var formD = input.Normalize(NormalizationForm.FormD);
            var chars = formD.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC).ToUpperInvariant();
        }

        // POCO interno para C#6
        private class ResultadoCalculo
        {
            public decimal Base { get; set; }
            public decimal IVA { get; set; }
            public string TipoFactor { get; set; }
            public decimal TasaOCuota { get; set; }
        }
    }

}
