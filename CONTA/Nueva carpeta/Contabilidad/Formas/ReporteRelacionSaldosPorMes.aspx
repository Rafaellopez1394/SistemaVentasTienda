<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteRelacionSaldosPorMes.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteRelacionSaldosPorMes" %>

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
        <label class="lblTituloContenedor" >Reporte Relacion de Saldos por Mes</label>
      </div>
      <div id="mainContainer" class="container_body">

      <div>
      <h1>Empresa</h1>
      <select id="ddl-empresa" class="select-ajustado">
      </select>
      </div>
          <div>
            <div class="inlineblock">
                <label>Ejercicio</label>
                    <input type="text" id="txtEjercicio" style="width:170px" />
            </div>
            <div class="inlineblock">
                 <label>Fecha</label>
                 <input type="text" id="fechaFinal" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
            </div>
            <div>
                <div id="AyudaCuentaIncial"></div>
                <div id="AyudaCuentaFinal"></div>
            </div>            
          </div>
          <div>
              <div class="inlineblock">
                <fieldset>
                  <legend>Tipo de impresión</legend>
                  <label><input type="radio" name="FormatoImprimir" value="3" checked /> PDF</label>
                  <label><input type="radio" name="FormatoImprimir" value="1" /> Excel</label>
                </fieldset>
              </div>
              <div class="inlineblock">
                <fieldset>
                  <legend>Gastos Contables</legend>
                  <label><input type="radio" name="SoloGastosContables" value="1"/>Solo Gastos Contables</label>
              </div>
          </div>
        <footer>
          <input id="BotonImprimir" type="button" value="Imprimir" />          
          <input id="btnLimpiar" type="button" value="Limpiar" />          
          <iframe id="myDownloaderFrame" style="display:none;" ></iframe>
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
  <script src="../JavaScript/ReporteRelacionSaldosPorMes.js?v=1" type="text/javascript"></script>
</body>
</html>
