<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteCobranzaPorVendedor.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteCobranzaPorVendedor" %>

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
    <div id="divmsg" class="message"></div>

    <div id="divcontentMenu">
      <section class="logo ControlMenu" ></section>
      <section id="Menu" class="ControlMenu"></section>
    </div>

    <div id="div_tituloPrincipal" class="content_header">
      <label class="lblTituloContenedor">Cobranza por vendedor</label>
    </div>

    <div id="mainContainer" class="container_body">

      <h2>Filtros</h2>

      <div class="inlineblock">
        <label>Empresa<select id="fltr-empresa" class="select-ajustado"></select></label>
      </div>

      <div>
          <div class="inlineblock">
            <label>Fecha Inicio<input id="fltr-fecha" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" /></label>
          </div>
          <div class="inlineblock">
            <label>Fecha Fin<input id="fltr-fecha2" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" /></label>
          </div>
      </div>

      <div>
        <h3>Rango de vendedor</h3>
        <div class="inlineblock"><label>Vendedor inicio<input id="fltr-vendedorini" type="text" class="inputcorto entero" onfocus="this.select();" maxlength="5" /></label></div>
        <div class="inlineblock"><label>Vendedor fin<input id="fltr-vendedorfin" type="text" class="inputcorto entero" onfocus="this.select();" maxlength="5" /></label></div>
      </div>

      <input id="btnimprimir" type="button" value="Imprimir" />
      <input id="btnLimpiar" type="button" value="Limpiar" />

    </div>

  </form>

  <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>
  <script src="../../Base/js/vendor/amplify.min.js"></script>
  <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>
  <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
  <script src="../../Base/js/plugins.js"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/ReporteCobranzaPorVendedor.js"></script>

</body>
</html>

