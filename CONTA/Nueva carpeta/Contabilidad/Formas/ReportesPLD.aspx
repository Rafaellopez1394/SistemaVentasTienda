<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportesPLD.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReportesPLD" %>
<!DOCTYPE html>

<html lang="es-mx">
<head runat="server">
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
      <label class="lblTituloContenedor" >Reportes del PLD</label>
    </div>

    <div id="mainContainer" class="container_body">

      <h2>Filtros</h2>
      <div id="divtipoimpresion">
            <div class="inlineblock">
            <fieldset>
                <legend>Tipo de reporte</legend>
                <div>
                    <label><input type="radio" name="reporte" value="1" checked /> Imprimir detalle de clientes</label>
                </div>
                <div>
                    <label><input type="radio" name="reporte" value="2" /> Imprimir detalle de operaciones</label>
                </div>
            </fieldset>        
            </div>
        </div>        
        <div>
            <div class="inlineblock">
                <label>Fecha de corte<input id="fltr-fecha" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" /></label>
            </div>
        </div>
        <div>
            <div class="inlineblock">
                <fieldset>
                    <legend>Tipo de impresión</legend>
                    <label><input type="radio" name="timprimir" value="3" checked />PDF</label>
                    <label><input type="radio" name="timprimir" value="1" />Excel</label>
                </fieldset>
        </div>
        </div>
        
        <br />
      <input id="btnimprimir" type="button" value="Imprimir" />
      <input id="btnLimpiar" type="button" value="Limpiar" />
      <a id="btnDescargar" href="#" class="btn btn-primary disabled" aria-disabled="true" download=""></a>

    </div>

  </form>

  <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>
  <script src="../../Base/js/vendor/amplify.min.js"></script>
  <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>  
  <script src="../../Base/js/plugins.js"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/ReportesPLD.js"></script>
</body>
</html>
