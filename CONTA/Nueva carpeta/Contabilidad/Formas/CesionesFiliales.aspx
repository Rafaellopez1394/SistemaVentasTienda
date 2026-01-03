<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CesionesFiliales.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CesionesFiliales" %>

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
    <link href="../Estilos/CapturaPolizas.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>

        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>

        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Cesiones de Filiales</label>
        </div>

        <div id="mainContainer" class="container_body">
            <div>
                <div class="inlineblock">
                    <fieldset>
                        <legend>Tipo de movimiento</legend>
                            <label class="subtitulo"><input type="radio" name="tipoMovimiento" value="1" checked />Registrar Contrato</label>
                            <label class="subtitulo"><input type="radio" name="tipoMovimiento" value="2"/>Disposiciones y pagos</label>
                    </fieldset>
                </div>
            </div>

            <div id="divRegistrarContrato">
                <fieldset>
                    <legend>Registrar Contrato</legend>
                    <div>
                        <div class="inlineblock">
                            <div id="ayudaCesionFilial"></div>
                        </div>          
                    </div>
                    <div>
                        <div class="inlineblock">
                            <div id="ayudaCliente"></div>
                        </div>          
                    </div>
                    <div class="inlineblock">
                        <fieldset>
                        <legend>Tipo de contrato</legend>
                            <label><input type="radio" name="tipoContrato" value="1" checked />Tasa Fija</label>
                            <label><input type="radio" name="tipoContrato" value="2"/>Cetes + Puntos</label>
                        </fieldset>
                    </div>

                    <div>
                        <div class="inlineblock">
                            <label>Importe de la linea</label>
                            <input type="text" id="txtImporteLinea" />
                        </div>
                        <div class="inlineblock">
                            <label id="lblTipoCredito">Tasa Fija Anual</label>
                            <input type="text" id="txtTasa" />
                        </div>
                    </div>
                    <div>
                        <div class="inlineblock">
                            <label for="txtfechainicio">Fecha de inicio</label>
                            <input type="text" id="txtfechainicio" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" required />
                        </div>
                        <div class="inlineblock">
                            <label for="txtfechainicio">Fecha de vencimiento</label>
                            <input type="text" id="txtfechavence" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" required />
                        </div>
                    </div>
                    <br />
                    <input id="btnGuardar" type="button" value="Guardar" />
                    <input id="btnLimpiar" type="button" value="Limpiar" />
                </fieldset>
            </div>

            <div id="divDisposicionesPagos" style="display:none;">
                <fieldset>
                <legend>Registrar Contrato</legend>
                <div>
                    <div class="inlineblock">
                        <div id="ayudaCesionFilial2"></div>
                    </div>          
                </div>

                <div>
                    <div class="inlineblock">
                        <fieldset>
                            <legend>Tipo de operacion</legend>
                                <label><input type="radio" name="tipoOperacion" value="1" checked />Disposición</label>
                                <label><input type="radio" name="tipoOperacion" value="2"/>Pagos</label>
                        </fieldset>
                    </div>
                </div>

                <div>
                    <div class="inlineblock">
                        <label>Importe</label>
                        <input type="text" id="txtImporteFinanciamiento" />
                    </div>
                    <div class="inlineblock">
                        <label for="txtfechainicio">Fecha de inicio</label>
                        <input type="text" id="txtfechaapli" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" required />
                    </div>
                </div>
                <div id="divBanco" style="display:none">
                    <div class="inlineblock">
                        <label for="ddlBancos">Banco</label>
                        <select id="ddlBancos" class="select-ajustado"></select>
                    </div>
                </div>
                 <br />
                <input id="btnGuardar2" type="button" value="Guardar" />
                <input id="btnLimpiar2" type="button" value="Limpiar" />
                </fieldset>
            </div>
            <footer>
            </footer>
        </div>
    </form>

    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>    
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>    
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>    
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/CesionesFiliales.js" type="text/javascript"></script>
</body>
</html>

