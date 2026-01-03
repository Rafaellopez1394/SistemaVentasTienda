<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InteresesDevengados.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.InteresesDevengados" %>
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
        <label class="lblTituloContenedor" >Genera intereses devengados</label>
      </div>
      <div id="mainContainer" class="container_body">            
        <div>                       
            <table id="tblCierres" class="grid">
                <thead>
                    <tr class="tr_header">
                        <th colspan="3">Cierres por procesar</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <br />
        <div class="inlineblock">
            <label>Fecha de cierre a procesar</label>
            <input type="text" id="fechaInicio" disabled placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" class="inputcorto" />
        </div>
        <div>
            <div class="inlineblock">
              <label>Tipo</label>
              <select id="ddlTipPol" title="Seleccione el tipo de póliza">
                <option></option>                
              </select>
            </div>
        </div>

        <div>
            <div class="inlineblock">
                <label>Número de póliza</label>
                <input type="text" id="txtNumPoliza" class="inputcorto"  />
            </div>
        </div>

        <footer>
          <input id="btnGenerar" type="button" value="Generar" disabled/>          
          <input id="btnLimpiar" type="button" value="Limpiar" />          
        </footer>         
      </div>
    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>    
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>  
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/InteresesDevengados.js?v=1.0.1" type="text/javascript"></script>
</body>
</html>
