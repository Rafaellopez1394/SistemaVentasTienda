let tabla;

$(document).ready(function () {
    cargarCatalogos();
    inicializarTabla();
});

/* ===========================
   CARGAR CATÁLOGOS
   =========================== */
function cargarCatalogos() {
    $.get('/Cliente/ObtenerCatalogos')
        .done(function (res) {
            const regimen = $('#cboRegimenFiscal');
            const uso = $('#cboUsoCFDI');
            const divCreditos = $('#divTiposDeCredito');

            regimen.empty().append('<option value="">-- Seleccionar Régimen --</option>');
            uso.empty().append('<option value="">-- Seleccionar Uso CFDI --</option>');
            divCreditos.empty();

            res.regimenes.forEach(item => {
                regimen.append(`<option value="${item.Value}">${item.Text}</option>`);
            });

            res.usosCFDI.forEach(item => {
                uso.append(`<option value="${item.Value}">${item.Text}</option>`);
            });

            if (res.tiposCredito && res.tiposCredito.length > 0) {
                res.tiposCredito.forEach(item => {
                    const checkboxHtml = `
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="checkbox" id="credito_${item.Value}" value="${item.Value}" name="tiposCredito">
                            <label class="form-check-label" for="credito_${item.Value}">${item.Text}</label>
                        </div>`;
                    divCreditos.append(checkboxHtml);
                });
            } else {
                divCreditos.html('<p class="text-muted">No hay tipos de crédito configurados.</p>');
            }
        });
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

        // ← Usa tu archivo local SIN CORS
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
    // Limpiar el formulario y los checkboxes de crédito
    $('#formCliente')[0].reset();
    $('input[name="tiposCredito"]').prop('checked', false);
    $('#divEstadoCredito').hide();

    const esEdicion = obj.ClienteID;

    $('#modalTitulo').text(esEdicion ? 'Editar Cliente' : 'Nuevo Cliente');
    $('#textoBoton').text(esEdicion ? 'Actualizar' : 'Guardar Cliente');

    if (esEdicion) {
        // -- MODO EDICIÓN --
        // Obtener detalles completos, incluyendo créditos asignados
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
                    $('#cboUsoCFDI').val(c.UsoCFDIID).trigger('change');

                    // Marcar los checkboxes de crédito que ya tiene el cliente
                    if (res.creditos && res.creditos.length > 0) {
                        res.creditos.forEach(credito => {
                            $(`#credito_${credito.TipoCreditoID}`).prop('checked', true);
                        });
                    }

                    // Mostrar estado de crédito
                    mostrarEstadoCredito(res);
                } else {
                    mostrarToast(res.message || "No se pudo cargar el cliente.", "danger");
                }
            })
            .fail(function () {
                mostrarToast("Error de conexión al buscar el cliente.", "danger");
            });

    } else {
        // -- MODO NUEVO --
        $('#txtClienteID').val(''); // Asegurar que el ID está vacío
        // Disparar change en select2 para limpiar visualmente
        $('#cboRegimenFiscal').val('').trigger('change');
        $('#cboUsoCFDI').val('').trigger('change');
    }

    $('#modalCliente').modal('show');
}

/* ===========================
   MOSTRAR ESTADO DE CRÉDITO
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
    // Validación
    if (!$('#txtRFC').val().trim() || !$('#txtRazonSocial').val().trim() || !$('#txtCodigoPostal').val().trim()) {
        mostrarToast("Complete los campos obligatorios", "warning");
        return;
    }

    $('#btnGuardar').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Guardando...');

    const cliente = {
        ClienteID: $('#txtClienteID').val() || '00000000-0000-0000-0000-000000000000',
        RFC: $('#txtRFC').val().trim().toUpperCase(),
        RazonSocial: $('#txtRazonSocial').val().trim(),
        CorreoElectronico: $('#txtCorreo').val().trim() || null,
        Telefono: $('#txtTelefono').val().trim() || null,
        CodigoPostal: $('#txtCodigoPostal').val().trim(),
        RegimenFiscalID: $('#cboRegimenFiscal').val(),
        UsoCFDIID: $('#cboUsoCFDI').val()
    };

    const creditosSeleccionados = $('input[name="tiposCredito"]:checked').map(function() {
        return $(this).val();
    }).get();

    const payload = {
        objeto: cliente,
        tiposCreditoIDs: creditosSeleccionados
    };

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
        mostrarToast("Error de conexión con el servidor", "danger");
    }).always(function () {
        $('#btnGuardar').prop('disabled', false).html('<i class="fas fa-save me-2"></i><span id="textoBoton">Guardar Cliente</span>');
    });
}

/* ===========================
   ELIMINAR CLIENTE
   =========================== */
function Eliminar(id) {
    if (!confirm('¿Confirmas dar de baja este cliente?')) return;

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
        tipo === "success" ? "Éxito" :
            tipo === "danger" ? "Error" : "Atención"
    );

    $('#toastMensaje').text(mensaje);

    $('#liveToast')
        .removeClass('bg-success bg-danger bg-warning')
        .addClass(`bg-${tipo} text-white`);

    new bootstrap.Toast(document.getElementById('liveToast')).show();
}
