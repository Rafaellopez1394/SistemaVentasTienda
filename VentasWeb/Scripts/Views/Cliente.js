let tabla;

$(document).ready(function () {
    cargarCatalogos();
    inicializarTabla();
});

/* ===========================
   CARGAR CATXLOGOS
   =========================== */
function cargarCatalogos() {
    $.get('/Cliente/ObtenerCatalogos')
        .done(function (res) {
            const regimen = $('#cboRegimenFiscal');
            const uso = $('#cboUsoCFDI');
            const divCreditos = $('#divTiposDeCredito');

            regimen.empty().append('<option value="">-- Seleccionar RX©gimen --</option>');
            uso.empty().append('<option value="">-- Seleccionar Uso CFDI --</option>');
            divCreditos.empty();

            res.regimenes.forEach(item => {
                regimen.append(`<option value="${item.Value}">${item.Text}</option>`);
            });

            res.usosCFDI.forEach(item => {
                uso.append(`<option value="${item.Value}">${item.Text}</option>`);
            });

            // Reinicializar Select2 despuX©s de cargar opciones
            if (typeof $.fn.select2 === 'function') {
                $('.select2').select2({
                    theme: 'bootstrap4',
                    width: '100%',
                    placeholder: "Seleccionar...",
                    allowClear: true
                });
            }

            if (res.tiposCredito && res.tiposCredito.length > 0) {
                res.tiposCredito.forEach(item => {
                    const checkboxHtml = `
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="checkbox" id="credito_${item.Value}" value="${item.Value}" name="tiposCredito" onchange="toggleLimiteCredito(${item.Value})">
                            <label class="form-check-label" for="credito_${item.Value}">${item.Text}</label>
                        </div>`;
                    divCreditos.append(checkboxHtml);
                });
            } else {
                divCreditos.html('<p class="text-muted">No hay tipos de crX©dito configurados.</p>');
            }
        });
}

/* ===========================
   TOGGLE CAMPOS DE LXMITE
   =========================== */
function toggleLimiteCredito(tipoCreditoId) {
    const checkbox = $(`#credito_${tipoCreditoId}`);
    const divLimite = $(`#divLimiteCredito_${tipoCreditoId}`);
    
    if (checkbox.is(':checked')) {
        divLimite.slideDown();
    } else {
        divLimite.slideUp();
        // Limpiar el campo cuando se desmarca
        if (tipoCreditoId == 1) {
            $('#txtLimiteDinero').val('');
        } else if (tipoCreditoId == 2) {
            $('#txtLimiteProducto').val('');
        } else if (tipoCreditoId == 3) {
            $('#txtPlazoDias').val('');
        }
    }
}

/* ===========================
   INICIALIZAR DATATABLE
   =========================== */
function inicializarTabla() {
    // Destruir instancia anterior si existe
    if ($.fn.DataTable.isDataTable('#tbClientes')) {
        tabla.DataTable().destroy();
    }
    
    tabla = $('#tbClientes').DataTable({
        ajax: {
            url: '/Cliente/Obtener',
            dataSrc: 'data'
        },

        // ñ¢â€ Â Usa tu archivo local SIN CORS
        language: {
            url: '/Content/Plugins/datatables/js/datatable_spanish.json'
        },

        columns: [
            { data: 'RFC' },
            { data: 'RazonSocial' },
            { data: 'CorreoElectronico' },
            { data: 'Telefono' },
            { data: 'RegimenFiscal' },
            { data: 'UsoCFDI' },
            {
                data: 'Estatus',
                render: data => data
                    ? '<span class="badge bg-success">Activo</span>'
                    : '<span class="badge bg-danger">Inactivo</span>'
            },
            {
                data: null,
                orderable: false,
                width: "100px",
                render: function (data) {
                    return `
                        <button class="btn btn-sm btn-warning" onclick='abrirModal(${JSON.stringify(data)})' title="Editar">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-danger ms-1" onclick="Eliminar('${data.ClienteID}')" title="Dar de baja">
                            <i class="fas fa-trash"></i>
                        </button>`;
                }
            }
        ]
    });
}
function abrirModal(obj = {}) {
    // Limpiar el formulario y los checkboxes de crX©dito
    $('#formCliente')[0].reset();
    $('input[name="tiposCredito"]').prop('checked', false);
    $('#divEstadoCredito').hide();
    
    // Ocultar todos los divs de lX­mite
    $('#divLimiteCredito_1, #divLimiteCredito_2, #divLimiteCredito_3').hide();
    $('#txtLimiteDinero, #txtLimiteProducto, #txtPlazoDias').val('');

    const esEdicion = obj.ClienteID;

    $('#modalTitulo').text(esEdicion ? 'Editar Cliente' : 'Nuevo Cliente');
    $('#textoBoton').text(esEdicion ? 'Actualizar' : 'Guardar Cliente');

    if (esEdicion) {
        // -- MODO EDICIX€œN --
        // Obtener detalles completos, incluyendo crX©ditos asignados
        $.get(`/Cliente/ObtenerPorId`, { id: obj.ClienteID })
            .done(function (res) {
                if (res.success) {
                    const c = res.cliente;
                    $('#txtClienteID').val(c.ClienteID);
                    $('#txtRFC').val(c.RFC);
                    $('#txtRazonSocial').val(c.RazonSocial);
                    $('#txtCorreo').val(c.CorreoElectronico);
                    $('#txtTelefono').val(c.Telefono);
                    $('#txtCodigoPostal').val(c.CodigoPostal);
                    $('#cboRegimenFiscal').val(c.RegimenFiscalID).trigger('change');
                    $('#cboUsoCFDI').val(c.UsoCFDI).trigger('change');

                    // Marcar los checkboxes de crX©dito que ya tiene el cliente y mostrar/cargar lX­mites
                    if (res.creditos && res.creditos.length > 0) {
                        res.creditos.forEach(credito => {
                            $(`#credito_${credito.TipoCreditoID}`).prop('checked', true);
                            $(`#divLimiteCredito_${credito.TipoCreditoID}`).show();
                            
                            // Cargar valores de lX­mites
                            if (credito.TipoCreditoID == 1 && credito.LimiteDinero) {
                                $('#txtLimiteDinero').val(credito.LimiteDinero);
                            } else if (credito.TipoCreditoID == 2 && credito.LimiteProducto) {
                                $('#txtLimiteProducto').val(credito.LimiteProducto);
                            } else if (credito.TipoCreditoID == 3 && credito.PlazoDias) {
                                $('#txtPlazoDias').val(credito.PlazoDias);
                            }
                        });
                    }

                    // Mostrar estado de crX©dito
                    mostrarEstadoCredito(res);
                } else {
                    mostrarToast(res.message || "No se pudo cargar el cliente.", "danger");
                }
            })
            .fail(function () {
                mostrarToast("Error de conexiX³n al buscar el cliente.", "danger");
            });

    } else {
        // -- MODO NUEVO --
        $('#txtClienteID').val(''); // Asegurar que el ID esta vacX­o
        // Disparar change en select2 para limpiar visualmente
        $('#cboRegimenFiscal').val('').trigger('change');
        $('#cboUsoCFDI').val('').trigger('change');
    }

    $('#modalCliente').modal('show');
}

/* ===========================
   MOSTRAR ESTADO DE CRX€°DITO
   =========================== */
function mostrarEstadoCredito(res) {
    if (res.creditoDisponible !== undefined) {
        $('#lblLimite').text('$' + (res.cliente.LimiteCreditoActual || 0).toFixed(2));
        $('#lblSaldo').text('$' + (res.saldoVencido || 0).toFixed(2));
        $('#lblDisponible').text('$' + (res.creditoDisponible || 0).toFixed(2));
        $('#lblDiasVencidos').text(res.cliente.DiasVencidos || 0);
        $('#divEstadoCredito').show();
    }
}

function Guardar() {
    // ValidaciX³n
    if (!$('#txtRFC').val().trim() || !$('#txtRazonSocial').val().trim() || !$('#txtCodigoPostal').val().trim()) {
        mostrarToast("Complete los campos obligatorios", "warning");
        return;
    }

    // Validar lX­mites si hay crX©ditos seleccionados
    let validacionLimites = true;
    $('input[name="tiposCredito"]:checked').each(function() {
        const tipoCreditoId = $(this).val();
        if (tipoCreditoId == 1 && !$('#txtLimiteDinero').val()) {
            mostrarToast("Ingrese el lX­mite de crX©dito en dinero", "warning");
            validacionLimites = false;
            return false;
        }
        if (tipoCreditoId == 2 && !$('#txtLimiteProducto').val()) {
            mostrarToast("Ingrese el lX­mite de producto (cajas)", "warning");
            validacionLimites = false;
            return false;
        }
        if (tipoCreditoId == 3 && !$('#txtPlazoDias').val()) {
            mostrarToast("Ingrese el plazo de crX©dito (dX­as)", "warning");
            validacionLimites = false;
            return false;
        }
    });

    if (!validacionLimites) return;

    $('#btnGuardar').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Guardando...');

    // Capturar valores de los campos fiscales
    const codigoPostal = $('#txtCodigoPostal').val() ? $('#txtCodigoPostal').val().trim() : null;
    const regimenFiscal = $('#cboRegimenFiscal').val();
    const usoCFDI = $('#cboUsoCFDI').val();

    // DEBUG: Ver quX© valores se capturan
    console.log('Valores fiscales capturados:', { codigoPostal, regimenFiscal, usoCFDI });

    // Validar campos obligatorios adicionales
    if (!codigoPostal || !regimenFiscal || !usoCFDI) {
        console.warn('ValidaciX³n fallida - Campos vacX­os:', { 
            codigoPostalVacio: !codigoPostal, 
            regimenVacio: !regimenFiscal, 
            usoCFDIVacio: !usoCFDI 
        });
        mostrarToast("Complete: CX³digo Postal, RX©gimen Fiscal y Uso de CFDI", "warning");
        $('#btnGuardar').prop('disabled', false).html('<i class="fas fa-save me-2"></i><span id="textoBoton">Guardar Cliente</span>');
        return;
    }

    const cliente = {
        ClienteID: $('#txtClienteID').val() || '00000000-0000-0000-0000-000000000000',
        RFC: $('#txtRFC').val().trim().toUpperCase(),
        RazonSocial: $('#txtRazonSocial').val().trim(),
        CorreoElectronico: $('#txtCorreo').val().trim() || null,
        Telefono: $('#txtTelefono').val().trim() || null,
        CodigoPostal: codigoPostal,
        RegimenFiscalID: regimenFiscal,
        UsoCFDIID: usoCFDI
    };

    // Construir array de crX©ditos con lX­mites
    const creditosConLimites = [];
    $('input[name="tiposCredito"]:checked').each(function() {
        const tipoCreditoId = parseInt($(this).val());
        const creditoData = {
            tipoCreditoID: tipoCreditoId,
            limiteDinero: null,
            limiteProducto: null,
            plazoDias: null
        };

        if (tipoCreditoId == 1) {
            creditoData.limiteDinero = parseFloat($('#txtLimiteDinero').val()) || 0;
        } else if (tipoCreditoId == 2) {
            creditoData.limiteProducto = parseInt($('#txtLimiteProducto').val()) || 0;
        } else if (tipoCreditoId == 3) {
            creditoData.plazoDias = parseInt($('#txtPlazoDias').val()) || 0;
        }

        creditosConLimites.push(creditoData);
    });

    const payload = {
        objeto: cliente,
        creditosConLimites: creditosConLimites
    };

    console.log('Guardando cliente:', payload); // DEBUG

    $.ajax({
        url: '/Cliente/Guardar',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
    }).done(function (res) {
        if (res.resultado) {
            tabla.ajax.reload();
            $('#modalCliente').modal('hide');
            mostrarToast("Cliente guardado correctamente", "success");
        } else {
            mostrarToast(res.mensaje || "Error al guardar", "danger");
        }
    }).fail(function () {
        mostrarToast("Error de conexiX³n con el servidor", "danger");
    }).always(function () {
        $('#btnGuardar').prop('disabled', false).html('<i class="fas fa-save me-2"></i><span id="textoBoton">Guardar Cliente</span>');
    });
}

/* ===========================
   ELIMINAR CLIENTE
   =========================== */
function Eliminar(id) {
    if (!confirm('ñ‚Â¿Confirmas dar de baja este cliente?')) return;

    $.post('/Cliente/Eliminar', { id: id })
        .done(function (res) {
            if (res.resultado) {
                tabla.ajax.reload();
                mostrarToast("Cliente dado de baja", "success");
            } else {
                mostrarToast(res.mensaje, "danger");
            }
        });
}

/* ===========================
   TOAST Bootstrap
   =========================== */
function mostrarToast(mensaje, tipo = "info") {
    $('#toastTitulo').text(
        tipo === "success" ? "X€°xito" :
            tipo === "danger" ? "Error" : "AtenciX³n"
    );

    $('#toastMensaje').text(mensaje);

    $('#liveToast')
        .removeClass('bg-success bg-danger bg-warning')
        .addClass(`bg-${tipo} text-white`);

    new bootstrap.Toast(document.getElementById('liveToast')).show();
}
