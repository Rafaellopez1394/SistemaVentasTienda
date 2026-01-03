<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CapturaPolizas.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CapturaPolizas" %>
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
        <section class="logo ControlMenu" ></section>
        <section id="Menu" class="ControlMenu"></section>
      </div>

      <div id="div_tituloPrincipal" class="content_header">
        <label class="lblTituloContenedor" >Captura de pólizas</label>
      </div>

      <div id="mainContainer" class="container_body">
        <label id="nomempresa" class="subtitulo"></label>

        <div id="mensaje"></div>

        <div id="sl-msg-container" class="msg-container hidden">
          <div id="sl-msg-body" class="msg-body">

          </div>
        </div>

        <fieldset>
          <legend>Filtros</legend>
            <div class="inlineblock">
                <div>
                    <div class="inlineblock">
                      <label>Tipo</label>
                      <select id="ddlTipPol" title="Seleccione el tipo de póliza">
                        <option></option>                
                      </select>
                    </div>
                    <div id="divnvo"  class="inlineblock">
                        <input id="btnNuevo" type="button" value="Nuevo" title="Generar una póliza nueva" /><span id="lblcancelado" class="cancelada display_none" >Cancelada</span>
                    </div>
                    <div>
                        <label for="chkPendiente" class="subtitulo"><input id="chkPendiente" type="checkbox" />Pendiente</label>
                    </div>
                </div>
                <div id="divBanco" style="display:none">
                    <div class="inlineblock">
                        <label>Seleccione banco</label>
                        <select id="ddlBancos" style="width:300px;">
                            <option value="*">Ninguno</option>
                        </select>
                    </div>
                   <%-- <div class="inlineblock">
                        <label>Cuenta</label>
                        <input type="text" id="txtcuentacheque" disabled="disabled"  style="text-align:center;"/>
                    </div>--%>
                    <div class="inlineblock">
                        <label>Consecutivo</label>
                        <input type="text" id="txtconsecutivo" disabled="disabled" style="text-align:center; width:100px;"/>
                    </div>
                </div>

                <div>
                    <div class="inlineblock">
                      <label>Fecha</label>
                      <input type="text" id="txtFechaPol" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                    </div>
                    <div class="inlineblock" style="padding-top: 25px;">              
                      <label id="lblCambiarFecha" style="color:blue;cursor:pointer;"><u>Cambiar fecha</u></label>
                    </div>
                </div>
                <div>
                    <div class="inlineblock">
                        <div id="AyudaPoliza"></div>            
                    </div>
                    <div class="inlineblock" style="padding-top: 25px;">              
                      <label id="lblCambiarPoliza" style="color:blue;cursor:pointer;"><u>Cambiar numero</u></label>
                    </div>
                </div>        
                <div>
                    <div class="inlineblock">
                      <label>Concepto</label>
                      <input type="text" id="txtConceptoPoliza" maxlength="250"/>
                    </div>
                </div>
                <div>
                    <label for="chkXML" class="subtitulo"><input id="chkXML" type="checkbox" />Adjuntar XML</label>
                </div>
                <div>
                    <label for="chkXMLNomina" class="subtitulo"><input id="chkXMLNomina" type="checkbox" />Adjuntar XML de Nómina</label>
                </div>
                <div>
                    <label for="chkPagoProgramado" class="subtitulo"><input id="chkPagoProgramado" type="checkbox"/>Agregar a pago programado</label>
                </div>
                <div>
                    <input id="btnAgregarDocumentos" type="button" value="Archivos Adicionales" onclick="abrirModal()">
                    <input id="btndescargarTodo" type="button" value="Descargar Archivos" style="display: none;" onclick="descargarTodo()">
                </div>
                <div>
            </div>
            <div id="ArchivosAdicionales" class="popup hidden">
                <div id="ArchivosAdicionales-close" class="close" title="Cerrar">X</div>
                <div id="ArchivosAdicionales-header" class="popup-header">
                    <span id="ArchivosAdicionales-Caption"></span>
                </div>
                <div id="ArchivosAdicionales-body" class="popup-body">
                    <div>
                        <label for="cmbArchivos" class="subtitulo">Agregar Archivo:</label>
                        <div style="display: flex; align-items: center;">
                            <select id="cmbArchivos">
                            </select>
                            <input type="button" id="ArchivosAdicionales-btn" value="Agregar" />
                        </div>
                    </div>
                    <br />
                    <table id="ArchivosAdicionales-Detalle" style="margin:0 auto;" class="svGrid">
                        <thead>
                            <tr class="tr_header">
                                <th>Nombre</th>
                                <th>Eliminar</th>
                                <th>Documento</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div id="ArchivosAdicionales-footer" class="popup-footer">
                    <input type="hidden" id="hiddenDocumentoEntregarDetalleID" value="" />
                    <input type="hidden" id="hiddenNombreDocumento" value="" />
                </div>
            </div>
            <div id="CopiaPoliza" class="popup hidden">
                <div id="CopiaPoliza-close" class="close" title="Cerrar">X</div>
                <div id="CopiaPoliza-header" class="popup-header">
                    <span id="CopiaPoliza-Caption"></span>
                </div>
                <div id="CopiaPoliza-body" class="popup-body">
                    <div style="display: flex; flex-direction: column; align-items: center;">
                        <label for="txtFechaPolNew" class="subtitulo">Agregar Datos Poliza:</label>
                        <label for="txtFechaPolNew">Fecha</label>
                        <input type="text" id="txtFechaPolNew" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" style="width: 80px;">

                        <label for="txtConceptoPolizaNew">Concepto</label>
                        <input type="text" id="txtConceptoPolizaNew" maxlength="250" style="width: 200px;">

                        <input type="button" id="CopiaPoliza-btn" value="Aceptar" />
                    </div>
                </div>
            </div>
                <div id="CopiaPolizaFacturco" class="popup hidden">
                <div id="CopiaPolizaFacturco-close" class="close" title="Cerrar">X</div>
                <div id="CopiaPolizaFacturco-header" class="popup-header">
                    <span id="CopiaPolizaFacturco-Caption"></span>
                </div>
                <div id="CopiaPolizaFacturco-body" class="popup-body">
                    <div style="display: flex; flex-direction: column; align-items: center;">
                        <label for="txtFechaPolNew" class="subtitulo">Agregar Datos Poliza:</label>
                        <label for="txtFechaPolNew">Fecha</label>
                        <input type="text" id="txtFechaPolFacturcoNew" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);" style="width: 80px;">

                        <label for="txtMesPolizaNew">Mes</label>
                        <input type="text" id="txtMesPolizaNew" maxlength="250" style="width: 200px;">

                        <label for="txtAnoPolizaNew">Año</label>
                        <input type="number" id="txtAnoPolizaNew" style="width: 200px;">

                        <input type="button" id="CopiaPolizaFacturco-btn" value="Aceptar" />
                    </div>
                </div>
            </div>
            <div class="inlineblock recuadro">
            
                <div>
                    <label class="subtitulo">Relacionar Facturas de Compras</label>
                </div>
                <div class="inlineblock">
                    <div id="btnExaminarFactura" style="width: 100px;">
                        <div id="upload" class="fileUpload btnExcel" style="position: relative; overflow: hidden; margin-top: 7px;">
                            <span>CARGAR XML</span>
                            <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                        </div>
                    </div>
                </div>
                <br /><br />
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
                            <th>XML</th>  
                            <th>CFDI</th>  
                            <th>VALIDACION</th>         
                            <th>Eliminar</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="2" style="font-size: 14px; text-align: right; color: white;">Totales</td>
                            <td id="total-subtotal" class="total-subtotal" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-iva" class="total-iva" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-ret-iva" class="total-ret-iva" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="total-ret-isr" class="total-ret-isr" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-total" class="total-total" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
                <br />
                <!--label id="lblSubtotalXML" class="subtitulo" style="color:blue">SUBTOTAL:</label>
                <br />
                <label id="lblIvaXML" class="subtitulo" style="color:blue">IVA:</label-->
            </div>

            <div class="inlineblock recuadro-nomina">
                <div>
                    <label class="subtitulo">Relacionar Facturas de Nómina</label>
                </div>
                <div class="inlineblock">
                    <div id="btnExaminarFacturaNomina" style="width: 100px;">
                        <div id="uploadNomina" class="fileUpload btnExcel" style="position: relative; overflow: hidden; margin-top: 7px;">
                            <span>CARGAR XML de Nómina</span>
                            <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                        </div>
                    </div>
                </div>
                <br /><br />
                <table id="TablaFacturasNomina" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr">
                            <th colspan="6" style="background-color:#fff !important;"></th>
                            <th colspan="12" style="background-color:#90EE90 !important;">PERCEPCIONES</th>
                            <th colspan="10" style="background-color:#FF4D4D !important;">DEDUCCIONES</th>
                            <th colspan="2" style="background-color:#fff !important;"></th>                            
                        </tr>
                        <tr class="tr_header">
                            <th>Serie</th>
                            <th>Factura</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Empleado</th>
                            <th>RFC</th>
                            <th>Sueldo</th>
                            <th>Premio <br />Por<br /> Asistencia</th>                            
                            <th>Premio <br />Por <br />Puntualidad</th>
                            <th>Vacaciones</th>  
                            <th>Prima <br />Vacacional</th>
                            <th>Aguinaldo</th>                        
                            <th>Gastos <br />Medicos <br />Mayores</th>
                            <th>Seguro <br />de <br />Vida</th>
                            <th>Indemnizacion</th>   
                            <th>Prima <br />de <br />Antiguedad</th>
                            <th>PTU</th>
                            <th>Total <br />Percepciones</th>
                            <th>Subcidio <br />Al<br />Empleo</th>
                            <th>I.S.R.<br />(Mes)</th>
                            <th>I.M.S.S.</th>
                            <th>Préstamo <br />Infonavit<br />(PORC)</th>
                            <th>Fonacot</th>
                            <th>Primas <br />Pagadas</th>
                            <th>I.S.R. <br />Art.174</th>
                            <th>Préstamo <br />Infonavit<br />CF</th>
                            <th>Total <br />Deducciones</th>
                            <th>Total</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="6" style="font-size: 14px; text-align: right; color: white;">Totales</td>
                            <td id="totaln-sueldo" class="totaln-sueldo" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-premioasistencia" class="totaln-premioasistencia" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-premiopuntualidad" class="totaln-premiopuntualidad" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-vacaciones" class="totaln-vacaciones" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-primavacacional" class="totaln-primavacacional" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-aguinaldo" class="totaln-aguinaldo" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-gastosmedicosmayores" class="totaln-gastosmedicosmayores" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-segurodevida" class="totaln-segurodevida" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-indemnizacion" class="totaln-indemnizacion" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-primadeantiguedad" class="totaln-primadeantiguedad" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-ptu" class="totaln-ptu" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-persepciones" class="totaln-persepciones" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-subsidioalempleo" class="totaln-subsidioalempleo" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-isrmes" class="totaln-isrmes" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-imss" class="totaln-imss" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-infonavit" class="totaln-infonavit" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-fonacot" class="totaln-fonacot" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-primaspagadaspatron" class="totaln-primaspagadaspatron" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-isrart174" class="totaln-isrart174" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-prestamoinfonavitcf" class="totaln-prestamoinfonavitcf" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-deducciones" class="totaln-deducciones" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-total" class="totaln-total" style="font-size: 12px; text-align: right; color: white;"></td>
                        </tr>
                    </tfoot>
                </table>
                <br />
                <!--label id="lblSubtotalXMLNomina" class="subtitulo" style="color:blue">SUBTOTAL:</label>
                <br />
                <label id="lblPercepciones" class="subtitulo" style="color:blue">Percepciones:</label>
                <br />
                <label id="lblDeducciones" class="subtitulo" style="color:blue">Deducciones:</label>
                <br />
                <label id="lblTotal" class="subtitulo" style="color:blue">Total:</label-->
            </div>


            <div class="inlineblock CuadroFacturas">
                <div>
                    <label class="subtitulo">Relaciona Poliza-Factura</label>
                </div>
                <br /><br />
                <table id="TablasFacturasPolizas" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr_header">
                            <th>Factura</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Iva</th>
                            <th>Total</th> 
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
                <br />
                <label id="lblSubtotalXMLFacturasPolizas" class="subtitulo" style="color:blue">SUBTOTAL:</label>
            </div>



        </fieldset>


        <fieldset>
          <legend>Detalle de la póliza</legend>
          <div>
          <div class="inlineblock">
            <div id="AyudaCuenta"></div>
          </div>
          <div class="inlineblock" style="display:none;">
            <div id="AyudaCuentaTabla"></div>
              <input type="text" id="hiddenRowAfectedID" />
              <input type="text" id="hiddenRowAfectedDescription" />
              <input type="text" id="hiddenNumeroRow" value="0" />
          </div>
          <!--
          <div>
            <label >Tipo de movimiento</label>
            <br />
            <label><input id="rdocargo" type="radio" title="Cargo" name="tipomov" value="1" checked="checked" />Cargo</label>
            &nbsp;
            <label><input id="rdoabono" type="radio" title="Abono" name="tipomov" value="2" />Abono</label>
          </div>
          -->

          <div class="inlineblock">
            <label for="txtImporte">Cargo</label>            
            <input id="txtImporte" class="numerico hmx_txt_Derecha" onfocus="this.select()" type="text" />
          </div>
          <div class="inlineblock">
            <label for="txtImporte">Abono</label>            
            <input id="txtImporteAbono" class="numerico hmx_txt_Derecha" onfocus="this.select()" type="text" />
          </div>

          <div class="inlineblock">
            <label for="txtDiferencia">Diferencia</label>            
            <input id="txtDiferencia" class="numerico hmx_txt_Derecha" type="text" disabled />
          </div>

          </div>
          <div>
            <input id="btnAgregar" type="button" value="Agregar" />
          </div>
          <br />
          <hr />
          <div>
           <table id="tablaDetalle"  class="svGrid" >
            <thead>
              <tr class="tr_header">
                <th>Cuenta</th>
                <th>Concepto</th>
                <th>Cargos</th>
                <th>Abonos</th>
                <th>Eliminar</th>
              </tr>
            </thead>
            <tbody>
              <tr id="trvacio" class="detalle">
                <td class="tdcuenta"></td>
                <td class="tdconcepto"></td>
                <td class="cargo"><input type="text" value="0.00" /></td>
                <td class="abono"><input type="text" value="0.00" /></td>
                <td class="tdeliminar"></td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="total_general tr_total "><td></td><td class="tdlbltitulo"><span>Total</span></td>
                <td class="td_numero"><span id="tg_cargos" >0.00</span></td>
                <td class="td_numero"><span id="tg_abonos" >0.00</span></td>
                <td></td>
              </tr>
            </tfoot>
          </table>
          </div>
        </fieldset>

        <footer>
          <input id="btnGuardar" type="button" value="Guardar" />
          <input id="btnCancelar" disabled="disabled"  type="button" value="Cancelar" />          
          <input id="btnImprimir" type="button" value="Imprimir" />
          <input id="btnCopiarPendiente" disabled="disabled" type="button" value="Copiar como pendiente" />
          <input id="btnCopiar" disabled="disabled" type="button" value="Copiar" />
          <input id="btnLimpiar" type="button" value="Limpiar" />          
        </footer>        
      </div>

          <div id="Asignar" class="popup hidden" style="width:400px;">
            <div id="Asignar-close" class="close" title="Cerrar">X</div>
            <div id="Asignar-header" class="popup-header"><span>Cambiar la fecha de la poliza</span></div>
            <div id="Asignar-body" class="popup-body" >                        
                <label>Fecha</label>
                <input type="text" id="txtFechaNueva" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />            
                <br />              
                <label id="Label1" style="color:red;"><u>Al cambiar esta informacion la poliza se cerrara y deberas volverla a consultar en el sistema, asegurate de haber guardado tus cambios</u></label>
            </div>
            <div id="Asignar-footer" class="popup-footer">
                <input type="button" value="Guardar" id="btnGuardaCF" />                  
            </div>
          </div>

          <div id="CamNumPol" class="popup hidden" style="width:400px;">
            <div id="CamNumPol-close" class="close" title="Cerrar">X</div>
            <div id="CamNumPol-header" class="popup-header"><span>Cambiar numero de la poliza</span></div>
            <div id="CamNumPol-body" class="popup-body" >                        
                <label>Numero poliza</label> 
                <input  type="text" id="txtNumPolNuevo" />
                <br />
                <label id="Label2" style="color:red;"><u>Al cambiar esta informacion la poliza se cerrara y deberas volverla a consultar en el sistema, asegurate de haber guardado tus cambios</u></label>               
            </div>
            <div id="CamNumPol-footer" class="popup-footer">
                <input type="button" value="Guardar" id="btnGuardaNumPol" />                
            </div>
          </div>
        <input id="HiddenNuevo" type="hidden" />
        <input id="HiddenPolizaid" type="hidden" />
        <input id="HiddenSucursalid" type="hidden" />
        <input id="HiddenEstatus" type="hidden" />
        <input id="HiddenUsuario" type="hidden" />
        <input id="HiddenUltimaAct" type="hidden" />
        <input type="hidden" id="CajaEmpresaRFC" />
        <asp:HiddenField ID="hiddenPolizaForaneaID" runat="server" Value="" />

            <div id="confirmBox" class="popup hidden" >
                <div id="confirmBox-close" class="close" title="Cerrar"></div>
                <div id="confirmBox-header" class="popup-header"><span id="Avance-TextHeader">Confirmar</span></div>
                <div id="confirmBox-body" class="popup-body" >            
                    <div class="confirmBox-message subtitulo"></div>
                </div>
                <div id="confirmBox-footer" class="popup-footer">
                    <input type="button" class="btn-CYes" value="Si" />
                    <input type="button" class="btn-CNo" value="No" />
                </div>
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
  <script src="../JavaScript/CapturaPolizas.js?v=3.3.5" type="text/javascript"></script>
</body>
</html>
