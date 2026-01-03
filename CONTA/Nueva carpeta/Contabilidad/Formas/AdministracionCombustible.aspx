<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdministracionCombustible.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.AdministracionCombustible" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Balor Financiera</title>
  <meta charset="UTF-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link href="../../Base/css/normalize.css" rel="stylesheet">
  <link href="../../Base/css/main.css" rel="stylesheet">
  <link href="../../Base/css/balor.css" rel="stylesheet">
        <link href="../../Base/fontawesome/css/all.min.css" rel="stylesheet" />
    <link href="../../Base/css/jquery-ui.css" rel="stylesheet" />
    <link href="../Estilos/AdministracionCombustible.css" rel="stylesheet" />
    
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
          <label class="lblTituloContenedor" >Administración de Combustible</label>
      </div>
      <div id="div_contenedorPrincipal" class="container_body">
       
          <div>
              <div class="inlineblock">
                <div id="ayudaVendedor"></div>
              </div>
              <div class="inlineblock">
                <label>Estatus</label>
                <select id="cboEstatus">
                    <option value="-1">Todos</option>
                    <option value="1" selected="selected">Activos</option>                    
                    <option value="2">Inactivos</option>
                </select>
              </div>
              <div class="inlineblock" style="vertical-align:bottom; margin-bottom:5px;">
                  <button id="btnbuscar" type="button" class="boton">Buscar</button>
              </div>
              <div>
                  <table id="tblSolicitudes" class="svGrid">
                      <thead>
                          <tr>
                              <th>Vendedor</th>                              
                              <th>Longitud</th>
                              <th>Latitud</th>
                              <th>Estatus</th>
                              <th>Opciones</th>
                          </tr>
                      </thead>
                      <tbody>

                      </tbody>
                  </table>
              </div>
          
              <div class="inlineblock" style="padding-top: 18px;">
                <div id="divBtn_Cuestionario" class="container_body" hidden>
                    <a class="svbtn show-cuestionario"><i class="svicon"></i>Ver Cuestionario</a>
                </div>
              </div>
          
          </div>
        </div>
    </form>


  <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>
  <script src="../../Base/js/vendor/amplify.min.js"></script>
  <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>  
  <script src="../../Base/js/vendor/moment-with-locales.min.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
  <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>
  <script src="../../Base/js/plugins.js"></script>
  <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/AdministracionCombustible.js"></script>
</body>
</html>
