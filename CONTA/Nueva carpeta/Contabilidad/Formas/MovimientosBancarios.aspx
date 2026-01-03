<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MovimientosBancarios.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.MovimientosBancarios" ValidateRequest="false" %>
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
        <label class="lblTituloContenedor" >Movimientos Bancarios</label>
      </div>
      <div id="mainContainer" class="container_body">
      <div>
      <h1>Empresa</h1>
      <select id="ddl-empresa" class="select-ajustado">
      </select>
      </div>
      <div id="divBancos">        
      </div>

    <div style="text-align:center;" id="divEncabezado">        
    </div>
        <div id="divinfo" style="margin:auto">        
        <table id="TablaMovtoBco" style="margin:0 auto;" class="svGrid">
        <thead>
            <tr class="tr_header">
                <th>Fecha</th>
                <th>Tip Pol</th>                
                <th>Bco</th>                
                <th>Beneficiario</th>
                <th>Importe</th>
                <th>Usuario</th>
                <th>Flujo</th>
                <th>T.M.</th>
                <th>Cheque</th>
                <th>Imprimir</th>
                <th>Ver Poliza</th>
            </tr>
        </thead>
        <tbody>          
        </tbody>
        </table>
        </div>

        <footer>          
          <input id="btnLimpiar" type="button" value="Limpiar" />          
        </footer>        
      </div>
    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>  
  <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/MovimientosBancarios.js" type="text/javascript"></script>
</body>
</html>
