<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteEstadoResultados.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteEstadoResultados" %>
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
        <label class="lblTituloContenedor" >Reporte de Estado de Resultados</label>
      </div>
      <div id="mainContainer" class="container_body">
        <div>
      <h1>Empresa</h1>
      <select id="ddl-empresa" class="select-ajustado">
      </select>
      </div>
          <div>
             <div class="inlineblock">
                 <label>Fecha Inicial</label>
                 <input type="text" id="fechaInicio" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
            </div>
            <div class="inlineblock">
                 <label>Fecha Final</label>
                 <input type="text" id="fechaFinal" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
            </div>            
          </div>
          <div>
              <div class="inlineblock">
                <fieldset>
                  <legend>Tipo de impresión</legend>
                  <label><input type="radio" name="timprimir" value="0" checked /> PDF</label>
                  <label><input type="radio" name="timprimir" value="1" /> Excel</label>
                </fieldset>
              </div>
              <div class="inlineblock">
                <fieldset>
                  <legend>Con Firma</legend>
                  <label><input type="radio" name="cfirma" value="1" />Firma</label>
                </fieldset>
              </div>
          </div>

        <footer>
          <input id="BotonImprimir" type="button" value="Imprimir" />          
          <input id="btnLimpiar" type="button" value="Limpiar" />         
        </footer>         
      </div>
    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>    
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/ReporteEstadoResultados.js?v=1.0.1" type="text/javascript"></script>
</body>
</html>
