<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaNegraSat.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ListaNegraSat" %>

<!DOCTYPE html>
<html lang="es-mx">
<head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet">
    <link href="../../Base/css/main.css" rel="stylesheet">
    <link href="../../Base/css/balor.css" rel="stylesheet">

    <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="hSesionId" />
        <div id="divmsg" class="message"></div>
        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Listado de contribuyentes - Lista Negra SAT</label>
        </div>
        <div id="mainContainer" class="container_body">
            <fieldset style="width: 50%;">
                <legend>Filtros</legend>



               
                <div class="inlineblock">
                    <label>RFC</label>
                    <input id="txtrfc" type="text" />
                </div>
                 <div class="inlineblock">
                    <label>Nombre</label>
                    <input id="txtnombre" type="text" />
                </div>
                <div class="inlineblock">
                    <fieldset>
                        <legend>Mostrar</legend>
                        <label>
                            <input type="radio" name="rbSoloBalor" value="1" checked />
                            Proveedores en sistema</label>
                        <label>
                            <input type="radio" name="rbSoloBalor" value="0" />
                            Todos</label>
                    </fieldset>

                </div>
                <div>
                    <input id="btnConsultar" value="Buscar" type="button" />
                     <input id="btnReset" value="Limpiar" type="button" />
                </div>
            </fieldset>


          
            <div class="inlineblock">
                <div id="btnExaminarFactura" style="width: 120px;">
                    <div id="upload" class="fileUpload btnExcel" style="position: relative; overflow: hidden; margin-top: 7px;">
                        <span>Importar CSV</span>
                        <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                    </div>
                </div>
            </div>

            <div style="text-align: center;">
                <h5 class="subtitulo"></h5>
            </div>
            <div id="divinfo" style="margin: auto">

                <table id="TablaContribuyentes" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr_header" >
                            <th style="font-size:small;">N&uacute;mero</th>
                            <th style="font-size:small;">RFC</th>
                            <th style="font-size:small;">Nombre</th>
                            <th style="font-size:small;">Situaci&oacute;n</th>
                            <th style="font-size:small;">Ver detalle</th>
                            <th style="font-size:small;">Existe en sistema</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>

        </div>

        <div id="contribuyente" class="popup hidden">
            <div id="contribuyente-close" class="close" title="Cerrar">X</div>
            <div id="contribuyente-header" class="popup-header"><span>Datos del contribuyente</span></div>
            <div id="contribuyente-body" class="popup-body-scroll">

                 <div id="divDetalleLista" style="margin:auto">        
                <table id="TablaContribuyente" style="margin:0 auto;" class="svGrid">
               <%-- <thead>
                    <tr class="tr_header">
                        <th>Campo</th>
                        <th>Valor</th>
                       
                    </tr>            
                </thead>--%>
                <tbody>          
                </tbody>
                </table>
            </div>

<%--                <div>
                    <label for="txtNumeroDetalle" id="lblNumeroDetalle" class="subtitulo">N&uacute;mero: <span id="spNumeroDetalle" style="font-weight: normal;"></span></label>
                    <input id="txtNumeroDetalle" type="text" readonly />

                </div>

                <div>
                    <label for="txtRfcDetalle" id="lblRfcDetalle">N&uacute;mero</label>
                    <input id="txtRfcDetalle" type="text" readonly />
                </div>


                <textarea id="txtDocumentoobservacion-Observacion" rows="10" style="min-width: 95%" readonly>

                </textarea>--%>
            </div>


            <div id="contribuyente-footer" class="popup-footer">
                <input type="hidden" id="hiddenID" value="" />


            </div>
        </div>

        <div id="polizas" class="popup hidden">
            <div id="polizas-close" class="close" title="Cerrar">X</div>
            <div id="polizas-header" class="popup-header"><span>Polizas - Facturas</span></div>
            <div id="polizas-body" class="popup-body-scroll">

                 <div id="divdocpros" style="margin:auto">        
                <table id="TablaPolizas" style="margin:0 auto;" class="svGrid">
                <thead>
                    <tr class="tr_header">
                        <th style="font-size:small;">No</th>
                        <th style="font-size:small;">Tipo</th>
                        <th style="font-size:small;">Fecha</th>
                        <th style="font-size:small;">Importe</th>
                        <th style="font-size:small;">Factura</th>
                        <th style="font-size:small;">Emisor</th>
                        <th style="font-size:small;">Receptor</th>
                        <th style="font-size:small;">Concepto</th>
                       
                        <th style="font-size:small;">Fecha Factura</th>
                       
                    </tr>            
                </thead>
                <tbody>          
                </tbody>
                </table>
            </div>


            </div>


            <%--<div id="polizas-footer" class="popup-footer">
                <input type="hidden" id="hiddenID" value="" />
                
            </div>--%>
        </div>
        
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>    
    <script src="../../Base/js/vendor/amplify.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/ListaNegraSat.js" type="text/javascript"></script>
</body>
</html>
