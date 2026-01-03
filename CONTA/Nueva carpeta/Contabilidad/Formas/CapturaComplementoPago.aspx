<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CapturaComplementoPago.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CapturaComplementoPago" %>

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
  <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>
        
        <div id="divcontentMenu">
            <section class="logo ControlMenu" ></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor" >Captura de Complementos</label>
        </div>
        
        <div id="mainContainer" class="container_body">
            <label id="nomempresa" class="subtitulo"></label>
            
            <div id="mensaje"></div>
            
            <div id="sl-msg-container" class="msg-container hidden">
                <div id="sl-msg-body" class="msg-body"></div>
            </div>
           
            <div class="inlineblock">
                <div id="btnExaminarFactura" style="width: 100px;">
                    <div id="upload" class="fileUpload btnExcel" style="position: relative; overflow: hidden; margin-top: 7px;">
                        <span>CARGAR XML</span>
                        <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                    </div>
                </div>
            </div>
            <br /><br />
            <table id="TablaComplementos" style="width:40%" class="svGrid">
                <thead>
                    <tr class="tr_header">
                        <th>Emisor</th>
                        <th>Folio</th>
                        <th>Fecha</th>
                        <th>Complementos</th>
                        <th>Total</th>                            
                        <th>Eliminar</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
            <br /><br />
            
            <footer>
                <input id="btnGuardar" type="button" value="Guardar" />
                <input id="btnLimpiar" type="button" value="Limpiar" />          
            </footer>
        </div>
        <input type="hidden" id="CajaEmpresaRFC" />
    </form>
    
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/moment-with-locales.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/CapturaComplementoPago.js" type="text/javascript"></script>
</body>
</html>
