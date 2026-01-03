    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConciliacionCfdi.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ConciliacionCfdi" %>

    <!DOCTYPE html>
    <html lang="es-mx">
    <head id="Head1" runat="server">
        <title>Balor Financiera</title>
        <meta charset="UTF-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link href="../../Base/css/normalize.css" rel="stylesheet" type="text/css" />
        <link href="../../Base/css/main.css" rel="stylesheet" type="text/css" />
        <link href="../../Base/css/balor.css" rel="stylesheet" type="text/css" />
        <link href="../Estilos/CapturaPolizas.css" rel="stylesheet" type="text/css" />
        <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />

    <style>
      html, body {
        margin: 0; padding: 0; height: 100%; width: 100%;
        font-size: 12px !important;
      }

        .btn-header {
          background-color: #d32f2f; /* mismo rojo que el header */
          color: white;
          border: none;
          padding: 8px 16px;
          border-radius: 4px;
          cursor: pointer;
        }
        .btn-header:hover {
          opacity: 0.85;
        }
      fieldset:not(.no-fullscreen) {
            height: 98vh; 
            width: 98vw; 
            box-sizing: border-box;
            padding: 5px; 
            border: 3px solid #ccc; 
            overflow: auto;
            background-color: #fdfdfd;
        }
      .fieldset-auto {
       height: auto !important;
       width: 100% !important;

    }
      .btn-headerExcel {
      background-color: #217346; /* Verde característico de Excel */
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 4px;
      cursor: pointer;
      font-weight: bold;
      display: flex;
      align-items: center;
      gap: 6px; /* espacio entre icono y texto */
    }

    .btn-headerExcel:hover {
      background-color: #1e5e36; /* tono más oscuro en hover */
    }
        .btn-container {
      display: flex;
      gap: 8px; /* espacio entre los botones */
    }
      @media (max-width: 768px) {
        fieldset:not(.no-fullscreen){ padding: 5px; }
      }

      .table-wrapper{
        position:relative; border:1px solid #ccc;
        max-height:80vh; overflow:auto;
      }

      .table-title{
        position:sticky; top:0; background:#fff; z-index:10;
        padding:8px 0; font-size:1.25rem; font-weight:600;
        border-bottom:1px solid #ccc; white-space:nowrap; text-align:center;
      }

      .table-container{ overflow:visible; max-height:none; width:100%; padding-bottom:0; position:static; }

      #mainContainer, form#form1, body, html { overflow-x: visible !important; }

      .cfdi-table{
        min-width:1200px; width:auto;
        border-collapse:separate; border-spacing:0; table-layout:auto; position:relative;
      }

      .cfdi-table th{
        position:sticky; top:0; background:#fff;
        padding:3px; white-space:nowrap; text-overflow:ellipsis; overflow:hidden;
        font-size:12px; z-index:10;
      }

      /* ===== Opción B: divisor con box-shadow en la última sticky (RFC) ===== */
        .cfdi-resumen.opcion-b .sticky-rfc::after {
          display: none;                 /* anula la Opción A */
        }
        .cfdi-resumen.opcion-b .sticky-rfc {
          /* sombra vertical haciendo de línea divisoria que no se pierde al scrollear */
          box-shadow: 2px 0 0 0 #ccc;
          z-index: 120;                  /* por encima del contenido desplazable */
        }
      /* ====== tus estilos de columnas fijas generales (otros grids) ====== */
      .cfdi-table th.sticky-col, .cfdi-table td.sticky-col{
        position:sticky; background:#fff; border-right:none; z-index:20;
      }
      .cfdi-table th.sticky-col{ z-index:30 !important; }

      .sticky-col-1{ left:0; width:60px; min-width:60px; max-width:60px; top:0; background:#fff; z-index:30 !important; }
      .sticky-col-2{ left:60px; width:270px; min-width:270px; max-width:270px; top:0; background:#fff; z-index:29 !important; }
      .sticky-col-3{ left:330px; width:150px; min-width:150px; max-width:150px; top:0; background:#fff; z-index:28 !important; }
      .sticky-col-4{
        left:480px; width:365px; min-width:365px; max-width:365px; top:0; background:#fff;
        z-index:60 !important; transform:translateZ(0); box-shadow:2px 0 0 0 #ccc;
      }

      .descripcion-columna{ width:300px; max-width:300px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; display:block; }

      .dynamic-table{ width:100%; border-collapse:collapse; }
      .dynamic-table th, .dynamic-table td{ border:1px solid #ccc; padding:4px 6px; font-size:12px; white-space:nowrap; text-align:center; }

      .alert-container{ position:fixed; top:20px; right:20px; z-index:9999; width:auto; max-width:400px; }

      /* ====== SOLO para el Grid de RESUMEN ====== */
      .cfdi-resumen th, .cfdi-resumen td{
        border-right:1px solid #ccc; border-left:1px solid #ccc;
      }

      /* Reponer el borde derecho en sticky dentro del resumen */
      .cfdi-resumen th.sticky-col, .cfdi-resumen td.sticky-col{
        position:sticky !important; background:#fff; z-index:20;
        border-right:1px solid #ccc !important;
      }

      .cfdi-resumen th{ top:0; }

      /* UUID fijo (ajusta ancho si quieres) */
      .cfdi-resumen .sticky-uuid{
        position:sticky; left:0;
        width:280px; min-width:280px; max-width:280px;
        background:#fff; z-index:90; /* por encima de filas */
      }

      /* RFC fijo (segunda fija). El left debe ser = ancho de UUID */
      .cfdi-resumen .sticky-rfc{
        position:sticky; left:280px;
        width:100px; min-width:100px; max-width:100px;
        background:#fff; z-index:80;
      }

      /* Divisor que NO se pierde entre RFC (última fija) y el resto */
      .cfdi-resumen .sticky-rfc::after{
        content:""; position:absolute; top:0; right:-1px;
        width:1px; height:100%; background:#ccc;
        z-index:120; pointer-events:none;
      }

      @media (max-width:768px){
        .sticky-col-1, .sticky-col-2, .sticky-col-3{
          width:150px !important; min-width:150px !important; max-width:150px !important;
        }
        .sticky-col-2{ left:150px !important; }
        .sticky-col-3{ left:300px !important; }
        .descripcion-columna{ max-width:200px !important; }

        /* Si reduces UUID en móvil, ajusta también RFC */
        .cfdi-resumen .sticky-uuid{ width:180px; min-width:180px; max-width:180px; }
        .cfdi-resumen .sticky-rfc{ left:180px; }
      }

      .loading-spinner{
        display:none; position:fixed; top:50%; left:50%;
        transform:translate(-50%, -50%); z-index:1000;
        background:rgba(255,255,255,.8); padding:20px; border-radius:5px;
      }
    </style>

    </head>
    <body>
        <form id="form1" runat="server">
           <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
           <div id="divmsg" style="display: none;" class="mt-2 alert-container"></div>

            <div id="divcontentMenu">
                <section class="logo ControlMenu"></section>
                <section id="Menu" class="ControlMenu"></section>
            </div>

            <div id="div_tituloPrincipal" class="content_header">
                <label class="lblTituloContenedor">
                    CONCILIACIÓN CFDI INGRESOS Y EGRESOS
                </label>
            </div>

            <div id="mainContainer" class="container_body">
                <div id="loadingSpinner" class="loading-spinner">
                    <div class="spinner-border text-primary" role="status">
                        <span class="sr-only">Cargando...</span>
                    </div>
                </div>

                <!-- === NUEVO: Selector de tipo arriba de Importar información === -->
                <div class="mb-2" style="display:flex;gap:18px;align-items:center;">
                    <span class="font-weight-bold">Tipo:</span>
                    <asp:RadioButtonList ID="rblTipo" runat="server"
                                         RepeatDirection="Horizontal"
                                         AutoPostBack="true"
                                         OnSelectedIndexChanged="rblTipo_SelectedIndexChanged">
                        <asp:ListItem Text="Ingresos" Value="Ingresos" />
                        <asp:ListItem Text="Egresos"  Value="Egresos" Selected="True" />
                    </asp:RadioButtonList>
                </div>
                <!-- === FIN NUEVO === -->

                <fieldset class="no-fullscreen">
                    <legend>
                        <!-- NUEVO: Label que cambia con el radio -->
                        <asp:Label ID="lblImportarTitulo" runat="server" Text="Importar información Ingresos"></asp:Label>
                    </legend>
                    <asp:Label ID="lblEmpresa" runat="server" Text="Seleccione empresa" Font-Bold="true" /><br />
                    <asp:DropDownList ID="Empresas" runat="server" Width="300px" ClientIDMode="Static" AutoPostBack="false">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="mes" runat="server" ClientIDMode="Static" AutoPostBack="false">
                        <asp:ListItem Text="-- Seleccione mes --" Value="" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="año" runat="server" ClientIDMode="Static" AutoPostBack="false">
                        <asp:ListItem Text="-- Seleccione año --" Value="" />
                    </asp:DropDownList>
                    <p>Seleccione el archivo CFDI (.zip)</p>
                    <asp:FileUpload ID="FileUploadCFDI" runat="server" ClientIDMode="Static" onchange="checkZipFile()" />

                    <asp:Button ID="BtnProcesarZip" runat="server" Text="Procesar ZIP"
                                OnClientClick="showSpinner()" OnClick="BtnProcesarZip_Click"
                                CssClass="btn btn-primary" />
            </fieldset>
                <asp:Panel ID="PanelResultados" runat="server" Visible="false">
                    <fieldset>
                    <div class="table-title">CFDIs Conciliados</div>
                    <asp:Button ID="BtnDescargarConciliados" runat="server" Text="Descargar Excel - CFDIs Conciliados"
                                OnClick="BtnDescargarConciliados_Click" CssClass="btn btn-success" Visible="false" />
                    <div class="table-wrapper">
                        <div class="table-container">
                            <asp:GridView ID="GridViewCfdi" runat="server" AutoGenerateColumns="false"
                                          DataKeyNames="SourceName"
                                          OnRowDataBound="GridViewCfdi_RowDataBoundContabilidad" ShowFooter="true"
                                          CssClass="cfdi-table" EmptyDataText="No hay CFDIs" Visible="false"
                                          UseAccessibleHeader="true" GridLines="Both">
                                <Columns>
                                    <asp:TemplateField HeaderText="Mover"
                                        HeaderStyle-CssClass="sticky-col sticky-col-1"
                                        ItemStyle-CssClass="sticky-col sticky-col-1">
                                        <ItemTemplate>
                                            <div style="text-align: center;">
                                                <asp:CheckBox ID="chkMoverExento" runat="server" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField HeaderText="Nombre Archivo" DataField="SourceName"
                                        HeaderStyle-CssClass="sticky-col sticky-col-2"
                                        ItemStyle-CssClass="sticky-col sticky-col-2" />
                                    <asp:BoundField HeaderText="RFC Emisor" DataField="RfcEmisor"
                                        HeaderStyle-CssClass="sticky-col sticky-col-3"
                                        ItemStyle-CssClass="sticky-col sticky-col-3" />
                                    <asp:BoundField HeaderText="Nombre Emisor" DataField="NombreEmisor"
                                        HeaderStyle-CssClass="sticky-col sticky-col-4"
                                        ItemStyle-CssClass="sticky-col sticky-col-4" />
                                    <asp:BoundField HeaderText="Uso CFDI" DataField="UsoCFDI" />
                                    <asp:TemplateField HeaderText="Descripción">
                                        <ItemTemplate><div class="descripcion-columna" title='<%# Eval("Descripciones") %>'><%# Eval("Descripciones") %></div></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Serie" DataField="Serie" />
                                    <asp:BoundField HeaderText="Folio" DataField="Folio" />
                                    <asp:BoundField HeaderText="Fecha" DataField="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField HeaderText="Forma de Pago" DataField="FormaPago" />
                                    <asp:TemplateField HeaderText="Impuestos Detalle">
                                        <ItemTemplate><%# Eval("TrasladosDetalle") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Retención IVA " DataField="RetencionIVA" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="Retención ISR " DataField="RetencionISR" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="Retención IEPS " DataField="RetencionIEPS" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="Total Iva" DataField="TotalImpuestosTrasladados" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="SubTotal" DataField="SubTotal" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="Moneda" DataField="Moneda" />
                                    <asp:BoundField HeaderText="Total" DataField="Total" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="IVAContabilidad" DataField="IVAContabilidad" />
                                    <asp:BoundField HeaderText="Tipo de Comprobante" DataField="TipoDeComprobante" />
                                    <asp:BoundField HeaderText="Método de Pago" DataField="MetodoPago" />
                                    <asp:BoundField HeaderText="Lugar de Expedición" DataField="LugarExpedicion" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </fieldset>

                <fieldset>
                    <div class="table-title">CFDIs Exentos o con Detalles</div>
                    <asp:Button ID="BtnDescargarExentos" runat="server" Text="Descargar Excel - CFDIs Exentos"
                                OnClick="BtnDescargarExentos_Click" CssClass="btn btn-success" Visible="false" />
                    <div class="table-wrapper">
                        <div class="table-container">
                            <asp:GridView ID="GridViewCfdi1" runat="server" AutoGenerateColumns="false"
                                          DataKeyNames="SourceName"
                                          OnRowDataBound="GridViewCfdi_RowDataBoundExenta" ShowFooter="true"
                                          CssClass="cfdi-table" EmptyDataText="No hay CFDIs" Visible="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Mover"
                                        HeaderStyle-CssClass="sticky-col sticky-col-1"
                                        ItemStyle-CssClass="sticky-col sticky-col-1">
                                        <ItemTemplate>
                                            <div style="text-align: center;">
                                                <asp:CheckBox ID="chkMoverConciliado" runat="server" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField HeaderText="Nombre Archivo" DataField="SourceName"
                                        HeaderStyle-CssClass="sticky-col sticky-col-2"
                                        ItemStyle-CssClass="sticky-col sticky-col-2" />
                                    <asp:BoundField HeaderText="RFC Emisor" DataField="RfcEmisor"
                                        HeaderStyle-CssClass="sticky-col sticky-col-3"
                                        ItemStyle-CssClass="sticky-col sticky-col-3" />
                                    <asp:BoundField HeaderText="Nombre Emisor" DataField="NombreEmisor"
                                        HeaderStyle-CssClass="sticky-col sticky-col-4"
                                        ItemStyle-CssClass="sticky-col sticky-col-4" />
                                    <asp:BoundField HeaderText="Uso CFDI" DataField="UsoCFDI" />
                                    <asp:TemplateField HeaderText="Descripción">
                                        <ItemTemplate><div class="descripcion-columna" title='<%# Eval("Descripciones") %>'><%# Eval("Descripciones") %></div></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="No aplica en diot">
                                        <ItemTemplate><%#ObtenerTextoExentoPlano(Eval("Exento")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contabilidad del mes">
                                        <ItemTemplate><%#ObtenerTextoContabilidadPlano(Eval("Contabilidad")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Serie" DataField="Serie" />
                                    <asp:BoundField HeaderText="Folio" DataField="Folio" />
                                    <asp:BoundField HeaderText="Fecha" DataField="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField HeaderText="Forma de Pago" DataField="FormaPago" />
                                    <asp:TemplateField HeaderText="Traslados Detalle">
                                        <ItemTemplate><%# Eval("TrasladosDetalle") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="SubTotal" DataField="SubTotal" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="Moneda" DataField="Moneda" />
                                    <asp:BoundField HeaderText="Total" DataField="Total" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="IVAContabilidad" DataField="IVAContabilidad" />
                                    <asp:BoundField HeaderText="Tipo de Comprobante" DataField="TipoDeComprobante" />
                                    <asp:BoundField HeaderText="Método de Pago" DataField="MetodoPago" />
                                    <asp:BoundField HeaderText="Lugar de Expedición" DataField="LugarExpedicion" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </fieldset>

               <div style="text-align:center; margin:20px;">
                    <asp:Button ID="BtnAplicarCambios" runat="server" Text="Aplicar cambios"
                                CssClass="btn btn-primary" OnClick="BtnAplicarCambios_Click" />
                    <asp:Button ID="BtnExportarDetallado" runat="server" Text="Generar carga batch"
                                CssClass="btn btn-primary" OnClick="BtnExportarDetallado_Click" />
                    <asp:Button ID="BtnExportarTxtDetallado" runat="server" Text="Generar TXT"
                                CssClass="btn btn-secondary" OnClick="BtnExportarTxtDetallado_Click" />
                    <hr />
                    <p>Leer datos desde PDF</p>

                         <div id="tablaListaDatosPDF" class="mt-3"></div>
                        <asp:HiddenField ID="HiddenJsonPDFLista" runat="server" ClientIDMode="Static" />
                        <input type="file" id="pdfInput" accept=".pdf" class="form-control" />
                        <br />
                        <button id="btnLeerPDF" type="button" class="btn btn-info">Leer PDF</button>
                        <button id="btnAgregarPDFManual" type="button" class="btn btn-success ml-2" disabled>Agregar PDF a lista</button>
                        <div id="resultadoPDF" class="mt-3"></div>
                        <asp:Button ID="BtnAgregarPDF" runat="server" Text="Agregar valores a la lista"
                                    OnClick="BtnAgregarPDF_Click" Style="display:none;" />
                        <asp:HiddenField ID="HiddenJsonPDF" runat="server" ClientIDMode="Static" />
                </div>
                <asp:Panel ID="PanelExentosConta" runat="server">
                    <fieldset>
                        <div class="table-title">CFDIs en Contabilidad del mes que no estan en los XML</div>
                            <asp:Button ID="BtnDescargarExentosConta" runat="server" Text="Descargar Excel - CFDIs que no se encuentran en XML y si se encuentran en contabilidad"
                                    OnClick="BtnDescargarExentosConta_Click" CssClass="btn btn-success" Visible="false" />
                            <div class="table-wrapper">
                                <div class="table-container">
                                    <asp:GridView ID="GridViewCfdi2" runat="server"
                                                  AutoGenerateColumns="false"
                                                  DataKeyNames="UUID"
                                                  CssClass="cfdi-table"
                                                  ShowFooter="true"
                                                  EmptyDataText="No hay registros"
                                                  Visible="false">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Mover"
                                                HeaderStyle-CssClass="sticky-col sticky-col-1"
                                                ItemStyle-CssClass="sticky-col sticky-col-1">
                                                <ItemTemplate>
                                                    <div style="text-align:center">
                                                        <asp:CheckBox ID="chkMoverConciliado" runat="server" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="UUID" DataField="UUID"
                                                HeaderStyle-CssClass="sticky-col sticky-col-2"
                                                ItemStyle-CssClass="sticky-col sticky-col-2" />
                                            <asp:BoundField HeaderText="RFC" DataField="RFC"
                                                HeaderStyle-CssClass="sticky-col sticky-col-3"
                                                ItemStyle-CssClass="sticky-col sticky-col-3" />
                                            <asp:BoundField HeaderText="Proveedor" DataField="Proveedor"
                                                HeaderStyle-CssClass="sticky-col sticky-col-4"
                                                ItemStyle-CssClass="sticky-col sticky-col-4" />
                                            <asp:BoundField HeaderText="Descripción" DataField="Concepto" />
                                            <asp:BoundField HeaderText="Fecha Póliza" DataField="FechaPol" DataFormatString="{0:dd/MM/yyyy}" />
                                            <asp:BoundField HeaderText="Tipo Póliza" DataField="Tip_Pol" />
                                            <asp:BoundField HeaderText="Folio" DataField="Folio" />
                                            <asp:BoundField HeaderText="Importe" DataField="ImporteFactura" DataFormatString="{0:C}" />

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </fieldset>
                    </asp:Panel>
               </asp:Panel>
               <asp:Panel ID="PanelCFDIIngresos" runat="server" Visible="false">

                   <fieldset class="fieldset-auto">
                         <div id="tituloCuentaCorriente" class="table-title">
                            CFDI CREDITO CUENTA CORRIENTE <span id="Mes1" class="text-muted"></span>
                        </div>
                        <div class="table-wrapper">
                            <div class="table-container">
                                <asp:GridView ID="GridViewCuentaCorriente" runat="server"
                                    AutoGenerateColumns="false"
                                    DataKeyNames="UUID"
                                    EmptyDataText="Sin datos"
                                    Visible="false"
                                    ShowFooter="true"
                                    OnDataBound="GridViewCuentaCorriente_DataBound"
                                    UseAccessibleHeader="true" GridLines="Both"
                                    CssClass="cfdi-table cfdi-resumen opcion-b">
                                    <Columns>
                                        <asp:BoundField HeaderText="UUID" DataField="UUID"
                                            HeaderStyle-CssClass="sticky-col sticky-uuid"
                                            ItemStyle-CssClass="sticky-col sticky-uuid" />

                                        <asp:BoundField HeaderText="RFC Receptor" DataField="RfcReceptor"
                                            HeaderStyle-CssClass="sticky-col sticky-rfc"
                                            ItemStyle-CssClass="sticky-col sticky-rfc" />

                                        <asp:BoundField HeaderText="Fecha Emisión" DataField="FechaEmision"
                                            DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />

                                        <asp:BoundField HeaderText="Tasa" DataField="Tasa" DataFormatString="{0:N6}" />

                                        <asp:BoundField HeaderText="Ingreso exento" DataField="IngresoExento" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Ingreso gravado" DataField="IngresoGravado" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="IVA (CFDI)" DataField="IVA_CFDI" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Total CFDI" DataField="TotalCFDI" DataFormatString="{0:C}" />

                                        <asp:BoundField HeaderText="Tipo" DataField="TipoDeComprobante" />

                                       <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>XML" 
                                            DataField="InteresOrdinario" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>XML" 
                                            DataField="InteresMoratorio" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>XML" 
                                            DataField="Comision" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (2050-1)<br/>XML" 
                                            DataField="IVA_1104" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />

                                        <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>Conta" 
                                            DataField="InteresOrdinario2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>Conta" 
                                            DataField="InteresMoratorio2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>Conta" 
                                            DataField="Comision2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (2050-1)<br/>Conta" 
                                            DataField="IVA_2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />

                                        <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>Diferencia" 
                                                DataField="DifInteresOrdinario2" DataFormatString="{0:C}" HtmlEncode="false" 
                                                HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>Diferencia" 
                                            DataField="DifInteresMoratorio2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>Diferencia" 
                                            DataField="DifComision2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (2050-1)<br/>Diferencia" 
                                            DataField="DifIVA_2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </fieldset> 
                   <fieldset class="fieldset-auto">
                        <div id="tituloFactoraje" class="table-title">
                            CFDI FACTORAJE FINANICERO <span id="Mes2" class="text-muted"></span>
                        </div>
                        <div class="table-wrapper">
                            <div class="table-container">
                                <asp:GridView ID="GridViewFactoraje" runat="server"
                                    AutoGenerateColumns="false"
                                    DataKeyNames="UUID"
                                    EmptyDataText="Sin datos"
                                    Visible="false"
                                    ShowFooter="true"
                                    OnDataBound="GridViewFactoraje_DataBound"
                                    UseAccessibleHeader="true" GridLines="Both"
                                    CssClass="cfdi-table cfdi-resumen opcion-b">
                                    <Columns>
                                        <asp:BoundField HeaderText="UUID" DataField="UUID"
                                            HeaderStyle-CssClass="sticky-col sticky-uuid"
                                            ItemStyle-CssClass="sticky-col sticky-uuid" />

                                        <asp:BoundField HeaderText="RFC Receptor" DataField="RfcReceptor"
                                            HeaderStyle-CssClass="sticky-col sticky-rfc"
                                            ItemStyle-CssClass="sticky-col sticky-rfc" />

                                        <asp:BoundField HeaderText="Fecha Emisión" DataField="FechaEmision"
                                            DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />

                                        <asp:BoundField HeaderText="Tasa" DataField="Tasa" DataFormatString="{0:N6}" />

                                        <asp:BoundField HeaderText="Ingreso exento" DataField="IngresoExento" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Ingreso gravado" DataField="IngresoGravado" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="IVA (CFDI)" DataField="IVA_CFDI" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Total CFDI" DataField="TotalCFDI" DataFormatString="{0:C}" />

                                        <asp:BoundField HeaderText="Tipo" DataField="TipoDeComprobante" />

                                       <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>XML" 
                                            DataField="InteresOrdinario" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>XML" 
                                            DataField="InteresMoratorio" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>XML" 
                                            DataField="Comision" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (2050-1)<br/>XML" 
                                            DataField="IVA_1104" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />

                                        <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>Conta" 
                                            DataField="InteresOrdinario2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>Conta" 
                                            DataField="InteresMoratorio2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>Conta" 
                                            DataField="Comision2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (2050-1)<br/>Conta" 
                                            DataField="IVA_2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />

                                        <asp:BoundField HeaderText="INTERES ORD (1104-1)<br/>Diferencia" 
                                            DataField="DifInteresOrdinario2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="INTERES MOR (1104-2)<br/>Diferencia" 
                                            DataField="DifInteresMoratorio2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="COMISIÓN (1104-4)<br/>Diferencia" 
                                            DataField="DifComision2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField HeaderText="IVA (1104)<br/>Diferencia" 
                                            DataField="DifIVA_2" DataFormatString="{0:C}" HtmlEncode="false" 
                                            HeaderStyle-HorizontalAlign="Center" />

                                    </Columns>
                                </asp:GridView>
                                <div style="margin-top:10px; text-align:right;">
                                    <asp:Label ID="lblTotalMes" runat="server" 
                                        Text="" 
                                        Font-Bold="true" 
                                        CssClass="text-primary" />
                                </div>
                            </div>
                        </div>
                       <div class="btn-container">
                            <asp:Button ID="BtnImprimirPDF" runat="server" Text="Imprimir a PDF"
                                CssClass="btn-header"
                                OnClientClick="exportarPDF(); return false;" />
                         <asp:Button ID="BtnExportarExcel" runat="server" Text="Generar Excel"
                            CssClass="btn-headerExcel"
                            OnClientClick="exportTwoTablesToExcel('GridViewCuentaCorriente', 'GridViewFactoraje', 'tituloCuentaCorriente', 'tituloFactoraje', 'lblTotalMes'); return false;" />
                       </div>
                    </fieldset>
                   <fieldset class="fieldset-auto">
                        <div class="table-title">
                            CFDI XML QUE NO SE ENCUENTRAN EN CONTABILIDAD <span id="Mes3" class="text-muted"></span>
                        </div>
                        <div class="table-wrapper">
                            <div class="table-container">
                                <asp:GridView ID="XMLNOCONTA" runat="server"
                                    AutoGenerateColumns="false"
                                    DataKeyNames="UUID"
                                    EmptyDataText="Sin XML a mostrar"
                                    Visible="false"
                                    UseAccessibleHeader="true" GridLines="Both"
                                    CssClass="cfdi-table cfdi-resumen opcion-b">
                                    <Columns>
                                        <asp:BoundField HeaderText="UUID" DataField="UUID"
                                            HeaderStyle-CssClass="sticky-col sticky-uuid"
                                            ItemStyle-CssClass="sticky-col sticky-uuid" />

                                        <asp:BoundField HeaderText="RFC Receptor" DataField="RfcReceptor"
                                            HeaderStyle-CssClass="sticky-col sticky-rfc"
                                            ItemStyle-CssClass="sticky-col sticky-rfc" />

                                        <asp:BoundField HeaderText="Fecha Emisión" DataField="FechaEmision"
                                            DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />

                                        <asp:BoundField HeaderText="Tasa" DataField="Tasa" DataFormatString="{0:N6}" />

                                        <asp:BoundField HeaderText="Ingreso exento" DataField="IngresoExento" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Ingreso gravado" DataField="IngresoGravado" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="IVA (CFDI)" DataField="IVA_CFDI" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="Total CFDI" DataField="TotalCFDI" DataFormatString="{0:C}" />

                                        <asp:BoundField HeaderText="Tipo" DataField="TipoDeComprobante" />
                                    </Columns>
                                </asp:GridView>

                                <asp:Label ID="lblSinXML" runat="server" 
                                    Text="Sin XML a mostrar" 
                                    Visible="false" 
                                    CssClass="text-danger font-weight-bold" />
                            </div>
                            <div class="btn-container">
                                <asp:Button ID="BtnPDFXMLNOCONTA" runat="server" 
                                CssClass="btn-header"
                                Text="Exportar XML No Contables"
                                OnClientClick="exportarPDFNConta(); return false;" />

                         <asp:Button ID="exportarEXCEL2" runat="server" Text="Generar Excel"
                                    CssClass="btn-headerExcel"
                                    OnClientClick="exportarExcelNOXML('XMLNOCONTA', 'tituloXMLNOCONTA'); return false;" />
                       </div>
                        </div>
                    </fieldset>
               </asp:Panel>
             </div>
        </form>
    
        <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.13.216/pdf.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.19.2/xlsx.full.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/pdfmake.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/vfs_fonts.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.style.min.js"></script>
        <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js"></script>
        <script src="../../Base/js/vendor/amplify.min.js"></script>
        <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>
        <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
        <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
        <script src="../../Base/js/plugins.js"></script>
        <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js"></script>
        <script src="../../Base/js/DateTimeFormat.js"></script>
        <script src="../JavaScript/ConciliacionCFDI.js"></script>
        
        <script>
            document.addEventListener("DOMContentLoaded", function () {
                var ddlEmpresas = document.getElementById("Empresas");
                var tituloCuenta = document.getElementById("tituloCuentaCorriente");
                var tituloFactoraje = document.getElementById("tituloFactoraje");
                var tituloXMLNOCONTA = document.getElementById("tituloXMLNOCONTA");
                var mesElement = document.getElementById("Mes");

                function actualizarTitulos() {
                    var empresaTxt = "";

                    // Validar ddlEmpresas
                    if (ddlEmpresas) {
                        if (ddlEmpresas.tagName === "SELECT" && ddlEmpresas.selectedIndex >= 0) {
                            empresaTxt = ddlEmpresas.options[ddlEmpresas.selectedIndex].text.toUpperCase();
                        } else if (ddlEmpresas.value) {
                            empresaTxt = ddlEmpresas.value.toUpperCase();
                        }
                    }

                    // Validar mes
                    var mesAnio = "";
                    if (mesElement) {
                        mesAnio = mesElement.textContent.replace("—", "").trim();
                    }

                    // Condicionales
                    if (empresaTxt.indexOf("FACTURCO") !== -1) {
                        if (tituloCuenta) {
                            tituloCuenta.innerHTML = "Contrato de mutuo <span id='Mes2' class='text-muted'>" + mesAnio + "</span>";
                        }
                        if (tituloFactoraje) {
                            tituloFactoraje.innerHTML = "Administración de cartera <span id='Mes' class='text-muted'>" + mesAnio + "</span>";
                        }
                        if (tituloXMLNOCONTA) {
                            tituloXMLNOCONTA.innerHTML = "XML no encontrados en la contabilidad del mes <span id='Mes' class='text-muted'>" + mesAnio + "</span>";
                        }
                    } else {
                        if (tituloCuenta) {
                            tituloCuenta.innerHTML = "CFDI CREDITO CUENTA CORRIENTE <span id='Mes' class='text-muted'>" + mesAnio + "</span>";
                        }
                        if (tituloFactoraje) {
                            tituloFactoraje.innerHTML = "CFDI FACTORAJE FINANCIERO <span id='Mes2' class='text-muted'>" + mesAnio + "</span>";
                        }
                        if (tituloXMLNOCONTA) {
                            tituloXMLNOCONTA.innerHTML = "XML no encontrados en la contabilidad del mes <span id='Mes' class='text-muted'>" + mesAnio + "</span>";
                        }
                    }
                }

                // Ejecuta al cargar la página
                actualizarTitulos();

                // Ejecuta cuando cambia la selección
                if (ddlEmpresas) {
                    ddlEmpresas.addEventListener("change", actualizarTitulos);
                }
            });
            </script>
        <script>

            function exportarPDFNConta() {

                // Empresa y mes dinámicos 
                var ddlEmpresas = document.getElementById("Empresas");
                var empresaTxt = "";
                if (ddlEmpresas) {
                    if (ddlEmpresas.tagName === "SELECT" && ddlEmpresas.selectedIndex >= 0)
                    { empresaTxt = ddlEmpresas.options[ddlEmpresas.selectedIndex].text.trim(); }
                    else { empresaTxt = ddlEmpresas.value ? ddlEmpresas.value.trim() : ""; }
                }

                var fechaActual;
                try {
                    fechaActual = new Date().toLocaleString("es-MX");
                } catch (e) {
                    fechaActual = new Date().toLocaleString();
                }

                // Logo según empresa (sin .includes)
                var upper = (empresaTxt || "").toUpperCase();
                var logoPath = "";
                var applyWhiteBackground = false;

                if (upper.indexOf("BALOR") !== -1) {
                    logoPath = "/Balor/BalorFinanciera/Base/img/NUEVOLOGOBALOR.png";
                } else if (upper.indexOf("FACTURCO") !== -1) {
                    logoPath = "/Balor/BalorFinanciera/Base/img/AnalisisFactur.png";
                    applyWhiteBackground = true; // fondo blanco para el ícono de Facturco
                } else {
                    logoPath = ""; // otras empresas → sin logo
                }

                if (logoPath) {
                    // toBase64 ya está definido en tu página — lo reutilizamos
                    toBase64(logoPath, function (base64Logo) {
                        generarPDF_XMLNoConta(base64Logo, empresaTxt, fechaActual);
                    }, applyWhiteBackground);
                } else {
                    generarPDF_XMLNoConta(null, empresaTxt, fechaActual);
                }
            }

            function tablaToPdf(gridId) {
                var grid = document.getElementById(gridId);
                if (!grid) return [];
                var body = [];
                var rows = grid.getElementsByTagName("tr");
                for (var i = 0; i < rows.length; i++) {
                    var cells = rows[i].querySelectorAll("th,td");
                    var rowData = [];
                    for (var j = 0; j < cells.length; j++) {
                        var txt = (cells[j].innerText || cells[j].textContent || "").replace(/\s+/g, " ").trim();
                        rowData.push(txt);
                    }
                    body.push(rowData);
                }
                return body;
            }

            function generarPDF_XMLNoConta(logoBase64, empresaTxt, fechaActual) {
                // Obtener datos del GridView (usa ClientID del control)
                var body = tablaToPdf("<%= XMLNOCONTA.ClientID %>");

                if (!body || body.length === 0) {
                    // Nada que exportar
                    alert("No hay registros en XMLNOCONTA para exportar.");
                    return;
                }

                // Construir docDefinition igual que tus otros pdfMake
                var colCount = body[0].length || 5;
                var widths = [];
                for (var i = 0; i < colCount; i++) widths.push("auto");

                var docDefinition = {
                    pageOrientation: "landscape",
                    pageSize: "LEGAL",
                    pageMargins: [15, 40, 10, 20],

                    header: function () {
                        return {
                            table: {
                                widths: [100, "*", 100],
                                body: [[
                                    logoBase64 ? {
                                        image: logoBase64,
                                        width: 90,
                                        height: 30,
                                        alignment: "left",
                                        margin: [5, 5, 5, 5]
                                    } : { text: "", width: 90 },

                                    {
                                        text: "CFDI XML QUE NO SE ENCUENTRAN EN CONTABILIDAD — " + (empresaTxt || ""),
                                        alignment: "center",
                                        fontSize: 14,
                                        bold: true,
                                        margin: [0, 12, 0, 0]
                                    },

                                    {
                                        text: fechaActual,
                                        alignment: "right",
                                        fontSize: 8,
                                        margin: [5, 12, 0, 0]
                                    }
                                ]]
                            },
                            layout: "noBorders",
                            margin: [0, 0, 0, 15]
                        };
                    },

                    content: [
                        {
                            table: {
                                headerRows: 1,
                                dontBreakRows: true,
                                widths: widths,
                                body: body
                            },
                            fontSize: 7,
                            layout: "lightHorizontalLines",
                            margin: [0, 15, 0, 0]
                        }
                    ]
                };

                pdfMake.createPdf(docDefinition).download("CFDI_XML_NoContabilidad.pdf");
            }

        </script>
       <script type="text/javascript">
            function exportTwoTablesToExcel(grid1Id, grid2Id, titulo1Id, titulo2Id, totalLabelId) {
                function tableToSheet(table, titulo) {
                    let ws_data = [[titulo]]; // fila con el título
                    let rows = table.querySelectorAll("tr");
                    rows.forEach(function(row) {
                        let cols = row.querySelectorAll("th,td");
                        let rowData = [];
                        cols.forEach(function(col) {
                            rowData.push(col.innerText.trim());
                        });
                        ws_data.push(rowData);
                    });
                    return ws_data;
                }

                var grid1 = document.getElementById(grid1Id);
                var grid2 = document.getElementById(grid2Id);
                var titulo1 = document.getElementById(titulo1Id).innerText;
                var titulo2 = document.getElementById(titulo2Id).innerText;
                var totalMes = document.getElementById(totalLabelId).innerText || "";

                
                // Empresa y mes dinámicos 
                var ddlEmpresas = document.getElementById("Empresas");
                var empresaTxt = "";
                if (ddlEmpresas)
                {
                    if (ddlEmpresas.tagName === "SELECT" && ddlEmpresas.selectedIndex >= 0)
                    { empresaTxt = ddlEmpresas.options[ddlEmpresas.selectedIndex].text.trim(); }
                    else { empresaTxt = ddlEmpresas.value ? ddlEmpresas.value.trim() : ""; }
                }

                var mesElement = document.getElementById("Mes");
                var mesAnio = "";

                if (mesElement) {
                    mesAnio = mesElement.textContent.replace("—", "").trim();
                }

                var filename = "Reporte";
                if (empresaTxt) filename += " - " + empresaTxt;
                if (mesAnio) filename += " - " + mesAnio;
                filename += ".xlsx";

                // Construcción de datos
                let ws_data = [];
                ws_data = ws_data.concat(tableToSheet(grid1, titulo1));
                ws_data.push([]);
                let startTable2 = ws_data.length;
                ws_data = ws_data.concat(tableToSheet(grid2, titulo2));
                ws_data.push([]);
                let totalRow = ws_data.length;
                ws_data.push(["", totalMes]);

                let ws = XLSX.utils.aoa_to_sheet(ws_data);

                // === FORMATO DE CELDAS ===
                function styleRange(r1, r2, c1, c2, style) {
                    for (let R = r1; R <= r2; R++) {
                        for (let C = c1; C <= c2; C++) {
                            const addr = XLSX.utils.encode_cell({r:R, c:C});
                            if (ws[addr]) {
                                ws[addr].s = style;
                            }
                        }
                    }
                }

                // Títulos grandes y centrados
                styleRange(0,0,0,ws_data[0].length-1,{
                    font: { bold:true, sz:14 },
                    alignment:{ horizontal:"center" }
                });
                styleRange(startTable2, startTable2,0,ws_data[startTable2].length-1,{
                    font: { bold:true, sz:14 },
                    alignment:{ horizontal:"center" }
                });

                // Encabezados de cada tabla
                styleRange(1,1,0,ws_data[1].length-1,{
                    font: { bold:true },
                    alignment:{ horizontal:"center" },
                    border:{
                        top:{style:"thin"}, bottom:{style:"thin"},
                        left:{style:"thin"}, right:{style:"thin"}
                    }
                });
                styleRange(startTable2+1,startTable2+1,0,ws_data[startTable2+1].length-1,{
                    font: { bold:true },
                    alignment:{ horizontal:"center" },
                    border:{
                        top:{style:"thin"}, bottom:{style:"thin"},
                        left:{style:"thin"}, right:{style:"thin"}
                    }
                });

                // Totales en rojo y negrita
                styleRange(totalRow,totalRow,0,ws_data[totalRow].length-1,{
                    font:{ bold:true, color:{ rgb:"FF0000" } }
                });

                // Ajustar anchos de columnas
                const cols = [];
                ws_data.forEach(row=>{
                    row.forEach((val,i)=>{
                        const l = val ? val.toString().length : 10;
                        cols[i] = Math.max(cols[i]||10,l+2);
                    });
                });
                ws['!cols'] = cols.map(w=>({wch:w}));

                // Crear libro y exportar
                let wb = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(wb, ws, "Reporte");
                XLSX.writeFile(wb, filename);
            }
        </script>
        <script type="text/javascript">
            function exportarExcelNOXML(gridId, tituloId) {
                // Función para construir datos de la tabla
                function tableToSheet(table) {
                    if (!table) return [];
                    const ws_data = [];
                    const rows = table.querySelectorAll("tr");
                    if (rows.length === 0) return [];
        
                    rows.forEach((row, index) => {
                        const cols = row.querySelectorAll("th,td");
                        const rowData = [];
                        cols.forEach(col => {
                            let txt = (col.innerText || col.textContent || "").trim();
                            rowData.push(txt);
                        });
                        ws_data.push(rowData);
                    });
                    return ws_data;
                }

                // Obtener tabla y título
                const grid = document.getElementById(gridId);
                var tituloElem = document.getElementById(tituloId);
                var titulo = (tituloElem && tituloElem.innerText) ? tituloElem.innerText : "CFDI XML QUE NO SE ENCUENTRAN EN CONTABILIDAD";


                // Validar tabla
                if (!grid) {
                    alert("No se encontró la tabla a exportar.");
                    return;
                }

                // Construir nombre de archivo dinámico (empresa + mes)
                const ddlEmpresas = document.getElementById("Empresas");
                let empresaTxt = "";
                if (ddlEmpresas) {
                    if (ddlEmpresas.tagName === "SELECT" && ddlEmpresas.selectedIndex >= 0)
                        empresaTxt = ddlEmpresas.options[ddlEmpresas.selectedIndex].text.trim();
                    else
                        if (ddlEmpresas.value) {
                            empresaTxt = ddlEmpresas.value.trim();
                        } else {
                            empresaTxt = "";
                        }
                }

                const mesElement = document.getElementById("Mes");

                var mesAnio = "";
                if (mesElement && mesElement.textContent) {
                    mesAnio = mesElement.textContent.replace("—", "").trim();
                }

                let filename = "CFDI XML QUE NO SE ENCUENTRAN EN CONTABILIDAD";
                if (empresaTxt) filename += " - " + empresaTxt;
                if (mesAnio) filename += " - " + mesAnio;
                filename += ".xlsx";

                // Construir datos
                let ws_data = [[titulo]]; // fila con el título
                const tablaData = tableToSheet(grid);
                if (tablaData.length === 0) {
                    ws_data.push(["No hay registros para mostrar"]);
                } else {
                    ws_data = ws_data.concat(tablaData);
                }

                // Crear hoja y libro
                const ws = XLSX.utils.aoa_to_sheet(ws_data);

                // Estilos: título en negrita y centrado
                const firstRowLength = ws_data[0].length;
                for (let c = 0; c < firstRowLength; c++) {
                    const addr = XLSX.utils.encode_cell({ r: 0, c });
                    if (ws[addr]) ws[addr].s = { font: { bold: true, sz: 14 }, alignment: { horizontal: "center" } };
                }

                // Ajustar ancho de columnas automáticamente
                const cols = [];
                ws_data.forEach(row => {
                    row.forEach((val, i) => {
                        const l = val ? val.toString().length : 10;
                        cols[i] = Math.max(cols[i] || 10, l + 2);
                    });
                });
                ws['!cols'] = cols.map(w => ({ wch: w }));

                // Crear libro y exportar
                const wb = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(wb, ws, "Reporte");
                XLSX.writeFile(wb, filename);
            }
             </script>

        <script>
        // Convierte imagen a base64 (con fondo blanco opcional)
           function toBase64(url, callback, applyWhiteBackground) {
               var img = new Image();
               img.crossOrigin = "Anonymous";
               img.onload = function () {
                   var canvas = document.createElement("canvas");
                   var ctx = canvas.getContext("2d");
                   canvas.width = img.width;
                   canvas.height = img.height;

                   // Si es el logo de BALOR → fondo rojo
                   if (url.indexOf("NUEVOLOGOBALOR.png") !== -1) {
                       ctx.fillStyle = "red";
                       ctx.fillRect(0, 0, canvas.width, canvas.height);
                   }

                   // Si se requiere fondo blanco (FACTURCO)
                   if (applyWhiteBackground) {
                       ctx.fillStyle = "white";
                       ctx.fillRect(0, 0, canvas.width, canvas.height);
                   }

                   ctx.drawImage(img, 0, 0);
                   var dataURL = canvas.toDataURL("image/png");
                   callback(dataURL);
               };
               img.src = url;
           }
      </script>
      <script>
        function exportarPDF() {

            // Empresa y mes dinámicos 
            var ddlEmpresas = document.getElementById("Empresas");
            var empresaTxt = "";
            if (ddlEmpresas) {
                if (ddlEmpresas.tagName === "SELECT" && ddlEmpresas.selectedIndex >= 0)
                { empresaTxt = ddlEmpresas.options[ddlEmpresas.selectedIndex].text.trim(); }
                else { empresaTxt = ddlEmpresas.value ? ddlEmpresas.value.trim() : ""; }
            }

            var fechaActual;
            try {
                fechaActual = new Date().toLocaleString("es-MX");
            } catch (e) {
                fechaActual = new Date().toLocaleString();
            }

            // Logo según empresa (sin .includes)
            var upper = (empresaTxt || "").toUpperCase();
            var logoPath = "";
            var applyWhiteBackground = false;

            if (upper.indexOf("BALOR") !== -1) {
                logoPath = "/Balor/BalorFinanciera/Base/img/NUEVOLOGOBALOR.png";
            } else if (upper.indexOf("FACTURCO") !== -1) {
                logoPath = "/Balor/BalorFinanciera/Base/img/AnalisisFactur.png";
                applyWhiteBackground = true; // fondo blanco para el ícono de Facturco
            } else {
                logoPath = ""; // otras empresas → sin logo
            }

            if (logoPath) {
                toBase64(logoPath, function (base64Logo) {
                    generarPDF(base64Logo, empresaTxt, fechaActual);
                }, applyWhiteBackground);
            } else {
                generarPDF(null, empresaTxt, fechaActual);
            }
        }

        function generarPDF(logoBase64, empresaTxt, fechaActual) {

        // Definir títulos dinámicos
        var tituloCuentaTxt = "";
        var tituloFactorajeTxt = "";
        var ddlEmpresas = document.getElementById("Empresas");
        var empresaTxt = "";
        if (ddlEmpresas && ddlEmpresas.selectedIndex > 0) {
            var opt = ddlEmpresas.options[ddlEmpresas.selectedIndex];
            empresaTxt = (opt && opt.text ? opt.text : "").replace(/^\s+|\s+$/g, "");
        }

        // Logo según empresa (sin .includes)
        var upper = (empresaTxt || "").toUpperCase();

        if (upper.indexOf("FACTURCO") !== -1) {
            tituloCuentaTxt = "ADMINISTRACIÓN DE CARTERA";
            tituloFactorajeTxt = "CONTRATO DE MUTUO";
        } else if (upper.indexOf("BALOR") !== -1){
            tituloCuentaTxt = "CFDI CREDITO CUENTA CORRIENTE";
            tituloFactorajeTxt = "CFDI FACTORAJE FINANCIERO";
        }

        function tablaToPdf(gridId) {
            var grid = document.getElementById(gridId);
            if (!grid) return [["No hay datos"]];
            var body = [];
            var rows = grid.getElementsByTagName("tr");
            for (var i = 0; i < rows.length; i++) {
                var cells = rows[i].querySelectorAll("th,td");
                var rowData = [];
                for (var j = 0; j < cells.length; j++) {
                    var txt = (cells[j].innerText || cells[j].textContent || "");
                    txt = txt.replace(/\s+/g, " ").replace(/^\s+|\s+$/g, "");
                    rowData.push(txt);
                }
                body.push(rowData);
            }
            return body;
        }

        function emptyCells(n) {
            var arr = [];
            for (var i = 0; i < n; i++) arr.push("");
            return arr;
        }

            var bodyCuentaCorriente = tablaToPdf("GridViewCuentaCorriente");
            var bodyFactoraje = tablaToPdf("GridViewFactoraje");

            var compactLayout = {
                hLineWidth: function () { return 0.3; },
                vLineWidth: function () { return 0.3; },
                paddingLeft: function () { return 2; },
                paddingRight: function () { return 2; }
            };

            // Construir cuerpo de tabla 1 (Cuenta Corriente)
            var table1Body = [];
            var headerRow1 = [{
                text: tituloCuentaTxt,   // ← ahora dinámico
                colSpan: 21,
                alignment: "center",
                bold: true,
                fillColor: "#E6E6E6",
                fontSize: 9
            }];
            headerRow1 = headerRow1.concat(emptyCells(20));
            table1Body.push(headerRow1);
            for (var a = 0; a < bodyCuentaCorriente.length; a++) {
                table1Body.push(bodyCuentaCorriente[a]);
            }

            // Construir cuerpo de tabla 2 (Factoraje)
            var table2Body = [];
            var headerRow2 = [{
                text: tituloFactorajeTxt,  // ← ahora dinámico
                colSpan: 21,
                alignment: "center",
                bold: true,
                fillColor: "#E6E6E6",
                fontSize: 9
            }];
            headerRow2 = headerRow2.concat(emptyCells(20));
            table2Body.push(headerRow2);
            for (var b = 0; b < bodyFactoraje.length; b++) {
                table2Body.push(bodyFactoraje[b]);
            }

            var docDefinition = {
                pageOrientation: "landscape",
                pageSize: "LEGAL",
                pageMargins: [15, 40, 10, 20],

                header: function () {
                    return {
                        table: {
                            widths: [100, "*", 100],
                            body: [[
                                // Columna 1: Logo
                                logoBase64 ? {
                                    image: logoBase64,
                                    width: 90,
                                    height: 30,
                                    alignment: "left",
                                    margin: [5, 5, 5, 5]
                                } : { text: "", width: 90 },

                                // Columna 2: Título centrado
                                {
                                    text: "CONCILIACIÓN CFDI INGRESOS — " + (empresaTxt || ""),
                                    alignment: "center",
                                    fontSize: 14,
                                    bold: true,
                                    margin: [0, 12, 0, 0]
                                },

                                // Columna 3: Fecha
                                {
                                    text: fechaActual,
                                    alignment: "right",
                                    fontSize: 8,
                                    margin: [5, 12, 0, 0]
                                }
                            ]]
                        },
                        layout: "noBorders",
                        margin: [0, 0, 0, 15]
                    };
                },

                content: [
                    // ======= CUENTA CORRIENTE =======
                    {
                        table: {
                            headerRows: 2,
                            dontBreakRows: true,
                            widths: [
                                55, 35, 40, 20,
                                45, 45, 35, 45, 15, 45,
                                45, 45, 45, 45,
                                45, 45, 45, 45,
                                45, 45, 45, 45
                            ],
                            body: table1Body
                        },
                        fontSize: 7,
                        alignment: "right",
                        layout: compactLayout,
                        margin: [0, 15, 0, 0]
                    },

                    // ======= FACTORAJE =======
                    {
                        table: {
                            headerRows: 2,
                            dontBreakRows: true,
                            widths: [
                               55, 35, 40, 20,
                                45, 45, 35, 45, 15, 45,
                                45, 45, 45, 45,
                                45, 45, 45, 45,
                                45, 45, 45, 45
                            ],
                            body: table2Body
                        },
                        fontSize: 7,
                        alignment: "right",
                        layout: compactLayout,
                        margin: [0, 15, 0, 0],
                        pageBreak: "before"
                    }
                ]
            };

            pdfMake.createPdf(docDefinition).download("ConciliacionCFDI.pdf");
        }
        </script>
            <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
            <script>
                (function () {
                    function actualizarTituloCFDI() {
                        var ddlMes = document.getElementById('mes');   // DropDownList mes
                        var ddlAnio = document.getElementById('año');   // DropDownList año

                        if (!ddlMes || !ddlAnio) return; // <-- Previene errores si no existen

                        // Elementos de destino (los spans donde mostramos mes y año)
                        var targets = [
                          document.getElementById('Mes'),
                          document.getElementById('Mes2'),
                          document.getElementById('Mes3')
                        ];

                        var mesTxt = '';
                        var anioTxt = '';

                        if (ddlMes.selectedIndex >= 0) {
                            mesTxt = (ddlMes.options[ddlMes.selectedIndex].text || '').trim();
                            if (mesTxt.toLowerCase().indexOf('seleccione') === 0) mesTxt = '';
                        }

                        if (ddlAnio.value) {
                            anioTxt = ddlAnio.value.trim();
                        }

                        var sufijo = '';
                        if (mesTxt) sufijo += mesTxt;
                        if (anioTxt) sufijo += (sufijo ? ' ' : '') + anioTxt;

                        targets.forEach(function (t) {
                            if (t) t.textContent = sufijo ? '— ' + sufijo : '';
                        });
                    }


                  // Inicializar al cargar
                  if (document.readyState === 'loading') {
                    document.addEventListener('DOMContentLoaded', actualizarTituloCFDI);
                  } else {
                    actualizarTituloCFDI();
                  }

                  // Escucha cambios
                  var ddlMes  = document.getElementById('mes');
                  var ddlAnio = document.getElementById('año');
                  if (ddlMes)  ddlMes.addEventListener('change', actualizarTituloCFDI);
                  if (ddlAnio) ddlAnio.addEventListener('change', actualizarTituloCFDI);

                  // Para UpdatePanel/partial postbacks
                  if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(actualizarTituloCFDI);
                  }
                })();
            </script>

    </body>
    </html>
