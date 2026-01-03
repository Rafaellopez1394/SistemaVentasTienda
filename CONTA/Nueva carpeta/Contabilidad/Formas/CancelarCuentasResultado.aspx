<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CancelarCuentasResultado.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CancelarCuentasResultado" %>

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
        <label class="lblTituloContenedor" >Cancelar Cuentas de Resultados</label>
      </div>

      <div id="mainContainer" class="container_body">
        <div>
            <div>
                <select id="ddl-empresa" class="select-ajustado"></select>
            </div>
            <div>
                <label>Año:</label>
                <input type="text" id="anioPoliza" placeholder="aaaa" pattern="^[0-9]{4}$" maxlength="4"/>
            </div>
            <div>
                <label class="etiqueta">Tipo:</label>
                <select id="ddlTipPol"></select>
            </div>
            <div class="inlineblock">
                <div>
                    <label class="etiqueta">Folio:</label>
                    <input id="numPol" type="text" style="font-size:24px; font-weight:bold;" />
                    <%--<label id="numPol" style="font-size:24px; font-weight:bold; margin:20px 0px 20px 0px">- - -</label>--%>
                </div>
            </div>
            <div>
                <div id="AyudaCuentaResultado"></div>
            </div>
            <div>
                <label>Comentario:</label>
                <textarea id="txtComentario" maxlength="200" cols="20" rows="20" style="max-width:500px; max-height:150px"></textarea>
            </div>
        </div>
        <footer>
            <input id="BotonGenerar" class="btn btn-success" type="button" value="Generar" />          
            <input id="btnLimpiar" class="btn btn-default" type="button" value="Limpiar" />
        </footer>        
      </div>
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
  <script src="../JavaScript/CancelarCuentasResultado.js" type="text/javascript"></script>
</body>
</html>

