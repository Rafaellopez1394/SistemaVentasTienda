<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SAT_DividendosRetenciones.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.SAT_DividendosRetenciones" %>

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
        <div id="divmsg" class="message"></div>
        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Timbrado de retenciones y dividendos</label>
        </div>
        <div id="mainContainer" class="container_body">
            <div>
                <div class="inlineblock">
                    <label>Empresa</label>
                    <select id="ddlEmpresa" class="cjComboBox" style="width: 350px;" tabindex="1">
                        <option></option>
                    </select>
                </div>
                <div class="inlineblock">
                    <label for="txtfecha">Fecha</label>
                    <input id="txtfecha" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" required />
                </div>
            </div>
            <div>
                <div class="inlineblock">
                    <div id="ayudaRetencion"></div>
                </div>
            </div>
            <div>
                <div class="inlineblock">
                    <label>Tipo de Retención</label>
                    <select id="ddlRetencion" class="cjComboBox" style="width: 350px;" tabindex="1">
                        <option></option>
                    </select>
                </div>
            </div>

            <div id="divTipoDividendo">
                <div class="inlineblock">
                    <label>Tipo de Dividendo</label>
                    <select id="ddlDividendo" class="cjComboBox" style="width: 350px;" tabindex="1">
                        <option></option>
                    </select>
                </div>
            </div>

            <fieldset>
                <legend>Datos del Receptor</legend>
                <div>
                    <div class="inlineblock">
                        <fieldset>
                            <legend>Nacionalidad</legend>
                            <label>
                                <input type="radio" name="rdNacionalidad" value="Nacional" checked />
                                Nacional</label>
                            <label>
                                <input type="radio" name="rdNacionalidad" value="Extranjero" />
                                Extranjero</label>
                        </fieldset>
                    </div>
                </div>
                <div id="DivNacional">
                    <div class="inlineblock">
                        <label for="txtRfc">RFC</label>
                        <input id="txtRfc" type="text" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtNombreRazonSocial">Nombre o Razon Social</label>
                        <input id="txtNombreRazonSocial" type="text" class="inputlargo" required />
                    </div>
                    <%--<div class="inlineblock" style="width:300px;">
                        <label for="ddlRegimenFiscal">Régimen Fiscal</label>
                        <select id="ddlRegimenFiscal" style="width:300px;">
                        </select>
                    </div>--%>
                    <div class="inlineblock" style="width:150px;">
                        <label for="txtcodigopostal">Código Postal</label>
                        <input type="text" id="txtcodigopostal" style="width:100px;" />
                        
                    </div>
                </div>
                <div id="DivExtranjero">
                    <div class="inlineblock">
                        <label for="txtIDFiscal">ID Fiscal</label>
                        <input id="txtIDFiscal" type="text" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtNombreRazonSocialExt">Nombre o Razon Social</label>
                        <input id="txtNombreRazonSocialExt" type="text" class="inputlargo" required />
                    </div>
                </div>
            </fieldset>

            <fieldset>
                <legend>Periodo</legend>
                <div>
                    <div class="inlineblock">
                        <label for="txtEjercicio">Ejercicio</label>
                        <input id="txtEjercicio" type="text" maxlength="4" class="inputcorto Numerico" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtMesIni">Mes Inicial</label>
                        <input id="txtMesIni" type="text" maxlength="2" class="inputcorto Numerico" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtMesFin">Mes Final</label>
                        <input id="txtMesFin" type="text" maxlength="2" class="inputcorto Numerico" required />
                    </div>
                </div>
            </fieldset>

            <fieldset>
                <legend>Totales</legend>
                <div>
                    <div class="inlineblock">
                        <label for="txtTotalOperacion">Total Operación</label>
                        <input id="txtTotalOperacion" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtImporteGravado">Importe Gravado</label>
                        <input id="txtImporteGravado" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtImporteExcento">Importe Excento</label>
                        <input id="txtImporteExcento" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtImporteRetenido">Importe Retenido</label>
                        <input id="txtImporteRetenido" type="text" class="Moneda" required />
                    </div>
                </div>
            </fieldset>

           <fieldset>
                <legend>Impuestos</legend>
                <div>
                    <div class="inlineblock">
                        <label for="txtBaseRetencion">Base Retención</label>
                        <input id="txtBaseRetencion" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtMontoRetenido">Importe Retenido</label>
                        <input id="txtMontoRetenido" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="ddlTipoImpuesto">Tipo Impuesto Retenido</label>
                        <select id="ddlTipoImpuesto" required >
                            <option value="*" selected>Seleccione...</option> 
                            <option value="001">ISR</option> 
                            <option value="002">IVA</option> 
                            <option value="003">IEPS</option>                             
                         </select>
                    </div>
                    <div class="inlineblock">
                        <label for="ddlTipoPagoRetenido">Tipo Pago Retenido</label>
                        <select id="ddlTipoPagoRetenido" required >
                            <option value="*" selected>Seleccione...</option> 
                            <option value="01">Pago definitivo IVA</option> 
                            <option value="02">Pago definitivo IEPS</option> 
                            <option value="03">Pago definitivo ISR</option> 
                            <option value="04">Pago provisional ISR</option> 
                         </select>
                    </div>
                </div>
            </fieldset>

            <div id="datosDividendos">
                <fieldset>
                <legend>Complemento de Dividendos</legend>
                <div>
                        <div class="inlineblock">
                        <label>Tipo de Sociedad</label>
                        <select id="ddlTipoSociedad" class="cjComboBox" style="width: 350px;" tabindex="1">
                            <option value="Sociedad Nacional">Sociedad Nacional</option>
                            <option value="Sociedad Extranjera">Sociedad Extreanjera</option>
                        </select>
                    </div>
                </div>
                <div>
                    <div class="inlineblock">
                        <label for="MontISRAcredRetMexico">Retencion Div Territorio Nac</label>
                        <input id="MontISRAcredRetMexico" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="MontISRAcredRetExtranjero">Retencion Div Territorio Ext</label>
                        <input id="MontISRAcredRetExtranjero" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="MontRetExtDivExt">Ret Ext Div Ext</label>
                        <input id="MontRetExtDivExt" type="text" class="Moneda" required />
                    </div>                    
                </div>
                <div>
                    <div class="inlineblock">
                        <label for="MontISRAcredNal">ISR Acreditable Nac</label>
                        <input id="MontISRAcredNal" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="MontDivAcumNal">Div Acumulable Nac</label>
                        <input id="MontDivAcumNal" type="text" class="Moneda" required />
                    </div>
                    <div class="inlineblock">
                        <label for="MontDivAcumExt">Div Acumulable Ext</label>
                        <input id="MontDivAcumExt" type="text" class="Moneda" required />
                    </div>
                </div>
                <div>
                    <div class="inlineblock">
                        <label for="Proporcionrem">Porc participacion</label>
                        <input id="Proporcionrem" type="text" class="Numerico" required />
                    </div>
                </div>
            </fieldset>
            </div>
            <input type="hidden" id="hiddenRetencionID" />
            <br />
            <input type="button" id="btnTimbrar" value="Timbrar" />
            <%--<input type="button" id="btnCancelar" value="Cancelar" disabled />--%>
            <input type="button" id="btnImprimir" value="Imprimir" disabled />
            <%--<input type="button" id="btnEnviarMail" value="Enviar eMail" disabled />--%>
            <input type="button" id="btnLimpiar" value="Limpiar" />
            <input type="button" id="btnGenerarDatos" value="Generar Datos" style="display:none;" />
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/SAT_DividendosRetenciones.js" type="text/javascript"></script>
</body>
</html>
