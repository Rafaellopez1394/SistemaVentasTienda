// Scripts/Views/vProveedor.js
let tabla;

$(document).ready(function () {
    cargarCatalogos();
    inicializarTabla();
});

function cargarCatalogos() {
    $.get('/Proveedor/ObtenerCatalogos')
        .done(function (res) {
            // Régimen Fiscal
            const regimen = $('#cboRegimenFiscal');
            regimen.empty().append('<option value="">Seleccionar</option>');
            res.regimenes.forEach(r => {
                regimen.append(`<option value="${r.RegimenFiscalID}">${r.RegimenFiscalID} - ${r.Descripcion}</option>`);
            });

            // Bancos
            const banco = $('#cboBanco');
            banco.empty().append('<option value="0">Seleccionar Banco</option>');
            res.bancos.forEach(b => {
                banco.append(`<option value="${b.BancoID}">${b.Nombre}</option>`);
            });

            // Tipo Proveedor
            const tipo = $('#cboTipoProveedor');
            tipo.empty().append('<option value="0">Seleccionar Tipo</option>');
            res.tiposProveedor.forEach(t => {
                tipo.append(`<option value="${t.TipoProveedorID}">${t.Descripcion}</option>`);
            });
        });
}

function inicializarTabla() {
    tabla = $('#tbProveedores').DataTable({
        ajax: {
            url: '/Proveedor/Obtener',
            dataSrc: 'data'
        },
        columns: [
            { data: 'RFC' },
            { data: 'RazonSocial' },
            { data: 'ContactoNombre', defaultContent: '' },
            { data: 'ContactoTelefono', defaultContent: '' },
            { data: 'Banco' },
            { data: 'TipoProveedor' },
            { data: 'DiasCredito', render: d => d ? d : 'Contado' },
            {
                data: 'Estatus',
                render: d => d ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>'
            },
            {
                data: null,
                orderable: false,
                render: function (data) {
                    return `
                        <button class="btn btn-warning btn-sm" onclick="abrirModal(${JSON.stringify(data).replace(/"/g, '&quot;')})">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-danger btn-sm" onclick="CambiarEstatus('${data.ProveedorID}')">
                            <i class="fas fa-ban"></i>
                        </button>`;
                }
            }
        ],
        language: { url: '/Content/Plugins/datatables/i18n/es-ES.json' }
    });
}

function abrirModal(obj = {}) {
    $('#formProveedor')[0].reset();
    $('#txtProveedorID').val(obj.ProveedorID || '00000000-0000-0000-0000-000000000000');
    $('#txtRFC').val(obj.RFC || '');
    $('#txtRazonSocial').val(obj.RazonSocial || '');
    $('#txtCodigoPostal').val(obj.CodigoPostal || '');
    $('#cboRegimenFiscal').val(obj.RegimenFiscalID || '');
    $('#txtContactoNombre').val(obj.ContactoNombre || '');
    $('#txtContactoCorreo').val(obj.ContactoCorreo || '');
    $('#txtContactoTelefono').val(obj.ContactoTelefono || '');
    $('#cboBanco').val(obj.BancoID || '0');
    $('#txtCuenta').val(obj.Cuenta || '');
    $('#txtCLABE').val(obj.CLABE || '');
    $('#txtTitularCuenta').val(obj.TitularCuenta || '');
    $('#cboTipoProveedor').val(obj.TipoProveedorID || '0');
    $('#txtDiasCredito').val(obj.DiasCredito || '');
    $('#txtCondiciones').val(obj.Condiciones || '');
    $('#cboEstatus').val(obj.Estatus ? '1' : '0');

    $('#modalProveedor').modal('show');
}

function Guardar() {
    if (!$('#txtRFC').val().trim() || !$('#txtRazonSocial').val().trim()) {
        mostrarToast("RFC y Razón Social son obligatorios", "warning");
        return;
    }

    const proveedor = {
        ProveedorID: $('#txtProveedorID').val() === '00000000-0000-0000-0000-000000000000' ? '00000000-0000-0000-0000-000000000000' : $('#txtProveedorID').val(),
        RFC: $('#txtRFC').val().trim().toUpperCase(),
        RazonSocial: $('#txtRazonSocial').val().trim(),
        RegimenFiscalID: $('#cboRegimenFiscal').val(),
        CodigoPostal: $('#txtCodigoPostal').val().trim(),
        ContactoNombre: $('#txtContactoNombre').val().trim() || null,
        ContactoCorreo: $('#txtContactoCorreo').val().trim(),
        ContactoTelefono: $('#txtContactoTelefono').val().trim() || null,
        BancoID: parseInt($('#cboBanco').val()),
        Cuenta: $('#txtCuenta').val().trim() || null,
        CLABE: $('#txtCLABE').val().trim(),
        TitularCuenta: $('#txtTitularCuenta').val().trim(),
        TipoProveedorID: parseInt($('#cboTipoProveedor').val()),
        DiasCredito: $('#txtDiasCredito').val() ? parseInt($('#txtDiasCredito').val()) : null,
        Condiciones: $('#txtCondiciones').val().trim() || null,
        Estatus: $('#cboEstatus').val() === '1'
    };

    $.ajax({
        url: '/Proveedor/Guardar',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(proveedor)
    }).done(function (res) {
        if (res.resultado) {
            tabla.ajax.reload();
            $('#modalProveedor').modal('hide');
            mostrarToast("Proveedor guardado correctamente", "success");
        } else {
            mostrarToast("Error al guardar proveedor", "danger");
        }
    });
}

function CambiarEstatus(id) {
    if (!confirm('¿Dar de baja este proveedor?')) return;

    $.post('/Proveedor/CambiarEstatus', { id: id })
        .done(function (res) {
            if (res.resultado) {
                tabla.ajax.reload();
                mostrarToast("Proveedor dado de baja", "success");
            } else {
                mostrarToast("No se pudo dar de baja", "danger");
            }
        });
}

function mostrarToast(mensaje, tipo = "info") {
    $('#toastTitulo').text(tipo === "success" ? "Éxito" : "Error");
    $('#toastMensaje').text(mensaje);
    $('#liveToast').removeClass().addClass(`toast bg-${tipo === "success" ? "success" : "danger"} text-white`);
    var toast = new bootstrap.Toast(document.getElementById('liveToast'));
    toast.show();
}