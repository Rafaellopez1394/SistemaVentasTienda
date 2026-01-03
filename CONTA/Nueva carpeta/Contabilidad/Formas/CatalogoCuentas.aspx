<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoCuentas.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CatalogoCuentas" %>
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
        <label class="lblTituloContenedor" >Catalogo de Cuentas</label>
      </div>
      <div id="mainContainer" class="container_body">
      <input type="button" runat="server" id="BotonNuevo" value="Nuevo"/><br/>
      <div>
        <div class="inlineblock">
            <fieldset>
                <legend>Tipo de cuenta</legend>
                <label><input type="radio" id="rdCuentaMayor" name="cuenta" value="1" checked />Cuenta de Mayor</label>
                <label><input type="radio" id="rdSubCuenta" name="cuenta" value="2" />Sub-Cuenta</label>
            </fieldset>
        </div>        
      </div>
      <div id="AyudaCuenta"></div>
      <div>
        <div class="inlineblock">
            <label for="CajaDescripcionIngles">Descripcion ingles</label>
            <input type="text" id="CajaDescripcionIngles" class="inputlargo" />
        </div>
      </div>
      <div id="AyudaCtaSat"></div>
      <div id="AyudaFlujoCargo"></div>
      <div id="AyudaFlujoAbono"></div>
      <div>
        <div class="inlineblock">
            <label class="subtitulo"><input type="checkbox"  id="chkAfectable" disabled />Afectable</label>            
            <label class="subtitulo"><input type="checkbox"  id="chkIETU" disabled />IETU</label>
            <label class="subtitulo"><input type="checkbox"  id="chkISR" disabled />ISR</label>
            <label class="subtitulo"><input type="checkbox"  id="chkSistema" disabled />Sistema</label>
             <label class="subtitulo"><input type="checkbox"  id="chkDolar" disabled />Dolar</label>
        </div> 
      </div>

      <div>
          <div class="inlineblock">
            <fieldset>
                <legend>Naturaleza</legend>
                <label><input type="radio" id="rdAcreedora" name="Naturaleza" value="Acreedora" checked />Acreedora</label>
                <label><input type="radio" id="rdDeudora" name="Naturaleza" value="Deudora" />Deudora</label>
            </fieldset>
          </div>        
      </div>

      <div>
          <div class="inlineblock">
            <label for="ComboGrupo">Grupo</label>
            <select id="ComboGrupo">
                <option value="01">Activo</option>
                <option value="02">Activo Fijo</option>
                <option value="03">Activo Diferido</option>
                <option value="04">Pasivo a Corto Plazo</option>
                <option value="05">Pasivo a Largo Plazo</option>
                <option value="06">Capital</option>
                <option value="07">Resultados</option>
                <option value="08">Cuentas de Orden</option>
            </select>
          </div>
          <div class="inlineblock">
            <label for="ComboTipo">Tipo</label>
            <select id="ComboTipo" >
                <option value="0">Ingresos</option>
                <option value="1">Costos de Venta</option>
                <option value="2">Gastos de Operacion</option>
                <option value="3">Otros Ingresos y Gastos</option>            
            </select>
          </div>
      </div>

       <div>
              <div class="inlineblock">
                <fieldset>
                  <legend>Tipo de impresión</legend>
                  <label><input type="radio" name="timprimir" value="P" checked /> PDF</label>
                  <label><input type="radio" name="timprimir" value="E" /> Excel</label>
                </fieldset>
              </div>
          </div>

          <div>
           <div class="inlineblock">
                 <label>Fecha Corte Envio SAT</label>
                 <input type="text" id="fechaInicio" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
            </div>
          </div>


        <footer>          
          <input type="button" id="BotonGuardar" value="Guardar" disabled />            
          <input type="button" id="BotonImprimir" value="Imprimir" />
          <input type="button" id="BotonGenerarSat" value="Envio SAT" />
          <input id="btnLimpiar" type="button" value="Limpiar" />          
          <iframe id="myDownloaderFrame" style="display:none;" ></iframe>
        </footer>        
      </div>
    <input type="hidden" id="CajaNuevo" value="0" />
    <input id="HiddenSucursal_id" type="hidden" />
    <input id="AcvCtamID" type="hidden" value="0" />
    <input id="UltimaActCuenta" type="hidden" value="0" />
    <input id="UltimaActCuentaMayor" type="hidden" value="0" />
    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>  
  <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/CatalogoCuentas.js?v=1" type="text/javascript"></script>
</body>
</html>
