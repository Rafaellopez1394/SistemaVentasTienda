<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CapturaCompras.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CapturaCompras" %>

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
            <label class="lblTituloContenedor">Captura de compras</label>
        </div>
        <div id="mainContainer" class="container_body">
            <fieldset style="width: 50%;">
                <legend>Datos de la compra</legend>
                <div>
                    <div class="inlineblock">
                        <div id="AyudaCompra"></div>
                    </div>
                    <div class="inlineblock">
                        <label>Fecha</label>
                        <input type="text" id="txtFechaPol" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" style="width: 100px;" />
                    </div>
                </div>


                <div id="AyudaProveedor"></div>
                <div class="inlineblock">
                    <div id="btnExaminarFactura" style="width: 50px;">
                        <div id="upload" class="fileUpload btnExcel hidden" style="position: relative; overflow: hidden; margin-top: 7px;">
                            <span>XML</span>
                            <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                        </div>
                    </div>
                </div>
                <div class="inlineblock">
                    <div id="btnManual" style="width: 50px;">
                        <div id="Manual" class="btnExcel hidden" style="position: relative; overflow: hidden; margin-top: 7px; cursor: pointer;">
                            <span>Manual</span>
                        </div>
                    </div>
                </div>
            </fieldset>
            <br />
            <fieldset style="width: 50%;">
                <legend>
                    <label class="subtitulo">Facturas</label>
                    <span id="minFacturas" class="close" style="position:inherit;" hidden>&nbsp;&nbsp;-&nbsp;&nbsp;</span>
                    <span id="maxFacturas" class="close" style="position:inherit;" >&nbsp;+&nbsp;</span>                    
                </legend>
                <div id="ContenedorFacturas">
                <div id="DivCargaManual">
                    <div>
                        <div class="inlineblock">
                            <label>Factura</label>
                            <input type="text" class="inputcorto" id="CajaFactura" onfocus="this.value;" />
                        </div>
                        <div class="inlineblock">
                            <label>Fecha</label>
                            <input type="text" id="CajaFechaFactura" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" class="inputcorto" />
                        </div>
                        <div class="inlineblock">
                            <label>Sub-Total</label>
                            <input type="text" class="inputcorto" id="CajaSubtotal" onfocus="this.value;" />
                        </div>
                        <div class="inlineblock">
                            <label>IVA</label>
                            <input type="text" class="inputcorto" id="CajaIva" onfocus="this.value;" />
                        </div>
                        <div class="inlineblock">
                            <label>Ret_Iva</label>
                            <input type="text" class="inputcorto" id="CajaRetIva" onfocus="this.value;" />
                        </div>
                        <div class="inlineblock">
                            <label>Ret_Isr</label>
                            <input type="text" class="inputcorto" id="CajaRetIsr" onfocus="this.value;" />
                        </div>
                        <div class="inlineblock">
                            <label>Total</label>
                            <input type="text" class="inputcorto" id="CajaTotal" onfocus="this.value;" />
                        </div>
                    </div>

                    <div>
                        <input type="button" id="CargaFacturaManual" value="Agregar" />
                        <input type="button" id="CancelarFacturaManual" value="Cancelar" />
                    </div>
                    <br />
                </div>         
                <table id="TablaFacturas" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr_header">
                            <th>Factura</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Iva</th>
                            <th>Ret_Iva</th>
                            <th>Ret_Isr</th>
                            <th>Total</th>
                            <th>Genera
                                <br />
                                Pasivo</th>
                            <th>Dlls</th>
                            <th>Eliminar</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>              
                </div>
            </fieldset>

            <br />
            <fieldset style="width: 50%;">
                <legend>
                    <label id="lblDetalleGasto" class="subtitulo">Detalle</label>&nbsp;<label id="lblImporteFactura" class="subtitulo"></label>&nbsp;<label id="lblCargado" class="subtitulo"></label>&nbsp;<label id="lblPorAplicar" class="subtitulo"></label>
                    <span id="maxDetGasto" class="close" style="position:inherit;">&nbsp;+&nbsp;</span>
                    <span id="minDetGasto" class="close" style="position:inherit;" hidden>&nbsp;&nbsp;-&nbsp;&nbsp;</span>
                </legend>
                <div id="ContenedorGasto">
                    <div>
                    <div class="inlineblock">
                        <label>Concepto</label>
                        <input type="text" id="txtConceptoPoliza" class="inputlargo" />
                    </div>
                </div>
                <div>
                    <div class="inlineblock">
                        <div id="AyudaCuenta"></div>
                    </div>
                    <div class="inlineblock">
                        <label for="txtImporte">Cargo</label> 
                        <input id="txtImporte" onfocus="this.select()" type="text" class="inputcorto" />
                    </div>
                </div>
                <div>
                    <input id="btnAgregar" type="button" value="Agregar" />
                </div>
                <br />
                <hr />
                <div>
                    <table id="tablaDetalle" class="svGrid">
                        <thead>
                            <tr class="tr_header">
                                <th>Cuenta</th>
                                <th>Concepto</th>
                                <th>Cargos</th>
                                <th>Eliminar</th>
                            </tr>
                        </thead>
                        <tbody>                            
                        </tbody>
                        <tfoot>
                            <tr class="total_general tr_total ">
                                <td></td>
                                <td class="tdlbltitulo"><span>Total</span></td>
                                <td class="td_numero"><span id="tg_cargos">0.00</span></td>
                                <td></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
                </div>                
                <input id="facturaenproceso" type="hidden" />
                <input id="uuidenproceso" type="hidden" />

            </fieldset>            
            <br />
            <input type="button" id="btnGuardar" value="Guardar" />
            <input type="button" id="btnCancelar" value="Cancelar" disabled />
            <input type="button" id="btnImprimir" value="Imprimir Poliza" disabled />
            <input type="button" id="btnLimpiar" value="Limpiar" />
            <input type="hidden" id="CajaEmpresaRFC" />
            <input type="hidden" id="PolizaID" />
            <input type="hidden" id="FacturaActiva" />
            <input type="hidden" id="CajaDetalleContable" />
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>    
    <script src="../../Base/js/vendor/amplify.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/CapturaCompras.js" type="text/javascript"></script>
</body>
</html>
