<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoClientesIntercompañia.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CatalogoClientesIntercompañia" %>

<!DOCTYPE html>
<html lang="es-mx">
<head runat="server">
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
    <form id="form1" runat="server" novalidate>
      <div id="divmsg" class="message"></div>
      <div id="divcontentMenu">
          <section class="logo ControlMenu" ></section>
          <section id="Menu" class="ControlMenu"></section>    
      </div>      
      <div id="div_tituloPrincipal" class="content_header">
          <label class="lblTituloContenedor" >Catalogo de clientes de intercompañias</label>
      </div>
      <div id="div_contenedorPrincipal" class="container_body">       
      <div>
          <div class="inlineblock">
            <div id="ayudaCliente"></div>
          </div>          
      </div>      
      <fieldset>
        <legend>Datos Generales</legend>
        <div>            
          <div class="inlineblock">
            <label for="txtrazonsocial">Denominaci&oacute;n o Raz&oacute;n Social</label>
            <input type="text" id="txtrazonsocial" style="width:450px;"  required />
          </div>
        </div>        
        <div>
          <div class="inlineblock">
            <label for="txtrfcpmoral">RFC</label>
            <input type="text" id="txtrfcpmoral" placeholder="ABC999999XXX" pattern="[a-zA-Z]{3}[0-9]{6}[a-zA-Z0-9]{0,3}" maxlength="12" required />
          </div>
        </div>                
      </fieldset>
      <br />
      <fieldset>
        <legend>Domicilio</legend>      
        <div>
          <div class="inlineblock">
            <label for="txtcalle">Calle</label>
            <input type="text" id="txtcalle" required />
          </div>
          <div class="inlineblock">
            <label for="txtnumexterior">N&uacute;mero exterior</label>
            <input type="text" id="txtnumexterior" />
          </div>
          <div class="inlineblock">
            <label for="txtnuminterior">N&uacute;mero interior</label>
            <input type="text" id="txtnuminterior" />
          </div>
          <div class="inlineblock">
            <label for="txtcodpostal">C&oacute;digo Postal</label>
            <input type="text" id="txtcodpostal" required />
          </div>
        </div>
        <div>
          <div class="inlineblock">
            <label for="ddlpais">Pais</label>
            <select id="ddlpais" required></select>
          </div>
          <div class="inlineblock">
            <label for="ddlestado">Estado</label>
            <select id="ddlestado" required></select>
          </div>
          <div class="inlineblock">
            <label for="ddlmunicipio">Delegaci&oacute;n o municipio</label>
            <select id="ddlmunicipio" required></select>
          </div>
          <div class="inlineblock">
            <label for="ddlciudad">Ciudad o poblaci&oacute;n</label>
            <select id="ddlciudad" required></select>
          </div>
          <div class="inlineblock">
            <label for="ddlcolonia">Colonia</label>
            <select id="ddlcolonia" required></select>
          </div>
        </div>
      </fieldset>

      <footer>        
        <input id="btnGuardar" type="button" value="Guardar" class="boton" />
        <input id="btnLimpiar" type="button" value="Limpiar" class="boton" />
      </footer>
      </div>
        <input type="hidden" id="hiddenultimaact" />
    </form>

    <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>  
    <script src="../../Base/js/vendor/amplify.min.js"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
    <script src="../../Base/js/plugins.js"></script>    
    <script src="../JavaScript/CatalogoClientesIntercompania.js"></script>
</body>
</html>
