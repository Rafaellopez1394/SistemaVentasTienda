<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReprosesarSaldos.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReprosesarSaldos" %>
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
        <label class="lblTituloContenedor" >Reprocesar Saldos Contables</label>
      </div>
      <div id="mainContainer" class="container_body">
      <h1>Empresa</h1>
      <select id="ddl-empresa" class="select-ajustado">
      </select>
        <footer>
          <input id="btnReprosesar" type="button" value="Reprosesar" />          
          <input id="btnLimpiar" type="button" value="Limpiar" />          
        </footer>        
      </div>
    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>    
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>    
  <script src="../JavaScript/ReprosesarSaldos.js" type="text/javascript"></script>
</body>
</html>