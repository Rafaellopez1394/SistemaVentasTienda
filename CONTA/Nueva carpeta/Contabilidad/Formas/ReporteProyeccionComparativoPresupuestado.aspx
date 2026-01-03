<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteProyeccionComparativoPresupuestado.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteProyeccionComparativoPresupuestado" %>

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
      <label class="lblTituloContenedor" >Reporte presupuestos de gastos</label>
    </div>

    <div id="mainContainer" class="container_body">

      <h2>Filtros</h2>

      <div class="inlineblock">
        <label>Empresa<select id="fltr-empresa" class="select-ajustado"></select></label>
      </div>

      
      <div id="divtipoimpresion">           
          <div class="inlineblock">
                <fieldset>
                    <legend>Formato de reporte</legend>
                    <div>
                        <label>
                            <input type="radio" name="formato" value="0" checked />
                        Completo</label>
                    </div>
                    <div>
                        <label>
                            <input type="radio" name="formato" value="1" />
                            Operativo</label>
                    </div>
                    <div>
                        <label>
                            <input type="radio" name="formato" value="2" />
                            Mensual y Acumulado</label>
                    </div>
                    <div>
                        <label>
                            <input type="radio" name="formato" value="6" />
                            Excluir Personales</label>
                    </div>
                </fieldset>
            </div>
        </div>
        <br />
        <div id="diveje">
            <div class="inlineblock">    
                <label id="Label3">Ejercicio</label>
                <input type="text" id="txtEjercicio"/>
            </div>    
             <div class="inlineblock">    
                <label>Mes</label>
                <select id="ddlMeses">
                    <option value="1" selected>ENERO</option>
                    <option value="2">FEBRERO</option>
                    <option value="3">MARZO</option>
                    <option value="4">ABRIL</option>
                    <option value="5">MAYO</option>
                    <option value="6">JUNIO</option>
                    <option value="7">JULIO</option>
                    <option value="8">AGOSTO</option>
                    <option value="9">SEPTIEMBRE</option>
                    <option value="10">OCTUBRE</option>
                    <option value="11">NOVIEMBRE</option>
                    <option value="12">DICIEMBRE</option>
                </select>
            </div>      
        </div>
        <div id="divfec" style="display:none;">
            <div class="inlineblock">
                <label>Fecha<input id="fltr-fecha" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" /></label>
            </div>
        </div>
        
       <div>
            <div id="AyudaCuentaIncial"></div>
            <div id="AyudaCuentaFinal"></div>
        </div>    

        <div>
            
        </div>

        <br />

        <div>
            <div class="inlineblock">
                <fieldset>
                    <legend>Tipo de impresión</legend>
                    <label>
                        <input type="radio" name="timprimir" value="3" checked />
                        PDF</label>
                    <label>
                        <input type="radio" name="timprimir" value="1" />
                        Excel</label>                    
                </fieldset>
            </div>
        </div>


        <br />
      <input id="btnimprimir" type="button" value="Imprimir" />
      <input id="btnLimpiar" type="button" value="Limpiar" />

    </div>

  </form>

      <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
  <script src="../JavaScript/ReporteProyeccionComparativoPresupuestado.js"></script>
</body>
</html>