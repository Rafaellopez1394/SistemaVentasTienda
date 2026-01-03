<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CapturaTipoDeCambio.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CapturaTipoDeCambio" %>

<!DOCTYPE html>
<html lang="es-mx">
<head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet">
    <link href="../../Base/css/main.css" rel="stylesheet">
    <link href="../../Base/css/balor.css" rel="stylesheet">    
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message">
        </div>
        <div id="MobileMenu" style="display: none">
            Menu
        </div>
        <div id="MobileMenuDet" style="display: none">
        </div>
        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Captura de Tipo de Cambio</label>
        </div>
        <div id="mainContainer" class="container_body">
              <div>
                  <div class="inlineblock">                
                    <fieldset style="width:300px;">
                    <legend>Tipo de carga</legend>
                        <label><input type="radio" id="rbIndividual" class="rbTipo" value="I" name="rTipo" checked /> Individual</label>
                        <label><input type="radio" id="rbDesdeExcel" class="rbTipo" value="D" name="rTipo" /> Desde Excel</label>
                    </fieldset>        
                  </div>
              </div>
            <br /><br />
            <div id="dviIndividual">
                <div class="inlineblock">
                    <label for="txtFecha">Fecha</label>
                    <input type="text" id="txtFecha" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" style="width: 100px;" />
                </div>
                <div class="inlineblock">
                    <label for="txtTipoCambio">Tipo de Cambio</label>
                    <input type="text" id="txtTipoCambio" onfocus="this.value;" class="inputcorto" />    
                </div>
            </div>
            <div id="dviDesdeExcel" style="display:none">
                    <div>                        
                        <div class="inlineblock">                    
                            <div id="btnExaminarFactura" style="width:100px;" >
                                <div id="upload" class="fileUpload btnExcel" style="position:relative;overflow:hidden;margin-top:7px;">
                                    <span >Cargar Excel</span>
                                    <input type="file" class="upload" style=" position: absolute;  top: 0;  right: 0;  margin: 0;  padding: 0;  font-size: 20px;  cursor: pointer;  opacity: 0;" multiple />
                                </div>
                            </div>
                        </div>
                    </div>
                <br />
                    <table id="TablaTipoCambio" style="margin: 0 auto;" class="svGrid">
                        <thead>
                            <tr class="tr_header">
                                <th>Fecha</th>
                                <th>Cambio</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>

            <div>
                <br />
                <div class="inlineblock">
                    <input type="button" id="btnGuardar" value="Guardar" />
                    <input type="button" id="btnLimpiar" value="Limpiar" />
                </div>
            </div>
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>
    <script src="../../Base/js/vendor/amplify.min.js"></script>
    <script src="../../Base/js/plugins.js"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
    <script src="../JavaScript/CapturaTipoDeCambio.js" type="text/javascript"></script>
</body>
</html>
