<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CierreContabilidad.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CierreContabilidad" %>

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
        <label class="lblTituloContenedor" >Cierre de contabilidad</label>
      </div>

      <div id="mainContainer" class="container_body">
        <div>
            <div>
                <select id="ddl-empresa" class="select-ajustado"></select>
            </div>
            <div class="inlineblock" >
                <label for="txtfecha">Fecha</label>
                <input id="txtfecha" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
            </div>
        </div>
        <footer>
            <input id="BotonCerrar" class="btn btn-success" type="button" value="Cerrar" />          
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
  <script src="../JavaScript/CierreContabilidad.js" type="text/javascript"></script>
</body>
</html>


