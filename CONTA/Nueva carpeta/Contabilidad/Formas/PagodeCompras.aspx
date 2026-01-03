<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PagodeCompras.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.PagodeCompras" %>

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
            <label class="lblTituloContenedor">Pago de Compras</label>
        </div>

        <div id="mainContainer" class="container_body">
            <div>
                <div class="inlineblock">
                    <fieldset>
                        <legend>Tipo de Pago</legend>
                            <label for="rbCheque"><input type="radio" id="rbCheque" name="tipoCuenta" />Cheque</label>
                            <label for="rbTransferencia"><input  type="radio" id="rbTransferencia" name="tipoCuenta"/>Transferencia</label>
                    </fieldset>
                </div>
            </div>
            <div>
                <div class="inlineblock">
                    <label for="ddlBancos">Bancos</label>
                    <select id="ddlBancos" class="select-ajustado"></select>
                </div>
                <div class="inlineblock">
                    <label for="fechaInicio">Fecha de Pago</label>
                    <input type="text" id="fechaInicio" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                </div>
            </div>
            <div>
                <div class="inlineblock">
                    <label for="txtReferencia">Poliza</label>
                    <input type="text" id="txtPoliza" />
                </div>
                <div class="inlineblock">
                    <label for="txtImporte">Importe</label>
                    <input type="text" id="txtImporte"/>
                </div>
            </div>
            <div>
                <div class="inlineblock">
                    <label for="txtTipoMoneda">Tipo de Moneda</label>
                    <input type="text" id="txtTipoMoneda" disabled/>
                </div>
                <div class="inlineblock">
                    <div id="divTipoCambio" style="display:none">
                        <label for="txtTipoCambio">Tipo de Cambio</label>
                        <input type="text" id="txtTipoCambio" disabled />
                    </div>
                </div>
            </div>
            <div>
                <div>
                    <div class="inlineblock">
                        <div id="AyudaProveedores"></div>
                    </div>                
                </div>
            </div>
            <br />
            <div>
                <fieldset>
                    <legend>Detalle de la compra</legend>
                    <div id="divinfo" style="margin:auto">        
                        <table id="TablaFacturas" style="margin:0 auto;" class="svGrid">
                            <thead>
                                <tr class="tr_header">
                                    <th>Factura</th>
                                    <th>Fecha</th>
                                    <th>Total</th>
                                    <th>Abonado</th>
                                    <th>Saldo</th>
                                    <th>Aplicar</th>
                                    <th>XML</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </fieldset>
            </div>
            <div>
                <div class="inlineblock">
                    <label class="subtitulo">Importe Aplicado</label>
                    <div>
                        <span id="ImporteAplicado" class="subtitulo" style="color:Blue; font-size:25px;">0.00</span>
                    </div>
                </div>
                <div class="inlineblock">
                    <label class="subtitulo">Diferencia</label>
                    <div>
                        <span id="ImporteDiferencia" class="subtitulo" style="color:Blue; font-size:25px;">0.00</span>
                    </div>
                </div>
            </div>
            <footer>
                <input id="btnGuardar" type="button" value="Guardar" />
                <input id="btnLimpiar" type="button" value="Limpiar" />
                <input type="hidden" id="FacturaActiva" />
            </footer>
        </div>
    </form>

    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/moment-with-locales.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/PagodeCompras.js" type="text/javascript"></script>
</body>
</html>

