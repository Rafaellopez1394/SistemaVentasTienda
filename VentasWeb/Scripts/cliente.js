var tabla;
let tiposPrecio = [];
let tiposFinanciamiento = [];

$(document).ready(function () {

    // 1. CARGAR CATÁLOGOS PRIMERO
    cargarCatalogos();

    // 2. INICIALIZAR DATATABLE
    tabla = $("#tbdata").DataTable({
        responsive: true,
        ajax: {
            url: '/Cliente/Obtener',
            type: 'GET',
            dataType: 'json',
            dataSrc: 'data'  // ¡CRUCIAL! Tu controlador devuelve { data: lista }
        },
        columns: [
            { data: 'ClienteID' },
            { data: 'Nombre' },
            { data: 'RFC' },
            { data: 'Email' },
            { data: 'Telefono' },
            { data: 'Ciudad' },
            {
                data: 'TipoPrecioID',
                render: function (data) {
                    const tipo = tiposPrecio.find(t => t.TipoPrecioID == data);
                    return tipo ? `<strong>${tipo.Nombre}</strong>` : 'Público General';
                }
            },
            {
                data: 'TipoFinanciamientoID',
                render: function (data) {
                    const fin = tiposFinanciamiento.find(f => f.TipoFinanciamientoID == data);
                    return fin ? fin.Nombre : '<em class="text-muted">Contado</em>';
                }
            },
            {
                data: 'Activo',
                render: d => d
                    ? '<span class="badge badge-success">Sí</span>'
                    : '<span class="badge badge-danger">No</span>'
            },
            {
                data: null,
                orderable: false,
                render: function (data) {
                    const json = JSON.stringify(data).replace(/"/g, '&quot;');
                    return `
                        <button class="btn btn-sm btn-warning" onclick="abrirPopUpForm('${json}')">
                            Editar
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="Eliminar(${data.ClienteID})">
                            Eliminar
                        </button>
                    `;
                }
            }
        ]
    });
});

// CARGAR CATÁLOGOS
function cargarCatalogos() {
    $.getJSON('/Cliente/ObtenerCatalogos')
        .done(function (res) {
            tiposPrecio = res.tiposPrecio || [];
            tiposFinanciamiento = res.tiposFinanciamiento || [];

            // Llenar select Tipo Precio
            const $precio = $('#cboTipoPrecio');
            $precio.empty();
            tiposPrecio.forEach(t => {
                $precio.append(`<option value="${t.TipoPrecioID}">${t.Nombre}</option>`);
            });

            // Llenar select Tipo Financiamiento
            const $fin = $('#cboTipoFinanciamiento');
            $fin.empty().append('<option value="">Contado / Ninguno</option>');
            tiposFinanciamiento.forEach(f => {
                $fin.append(`<option value="${f.TipoFinanciamientoID}">${f.Nombre}</option>`);
            });
        })
        .fail(() => alert("Error al cargar catálogos"));
}

// ABRIR MODAL (NUEVO O EDITAR)
function abrirPopUpForm(jsonString) {
    $("#form")[0].reset();
    $("#txtid").val("0");

    if (jsonString && jsonString !== "null") {
        const data = JSON.parse(jsonString.replace(/&quot;/g, '"'));

        $("#txtid").val(data.ClienteID);
        $("#txtNombres").val(data.Nombre || '');
        $("#txtRFC").val(data.RFC || '');
        $("#txtEmail").val(data.Email || '');
        $("#txtTelefono").val(data.Telefono || '');
        $("#txtDireccion").val(data.Direccion || '');
        $("#txtCiudad").val(data.Ciudad || '');
        $("#txtEstado").val(data.Estado || '');
        $("#txtCodigoPostal").val(data.CodigoPostal || '');
        $("#txtLimiteCredito").val(data.LimiteCredito || '');
        $("#txtSaldoPendiente").val(data.SaldoPendiente || '');
        
        // AQUÍ ESTÁ LA CLAVE: usar IDs, no texto
        $("#cboTipoPrecio").val(data.TipoPrecioID || 1);
        $("#cboTipoFinanciamiento").val(data.TipoFinanciamientoID || '');
        
        $("#txtDiasCredito").val(data.DiasCredito || '');
        $("#txtMontoCredito").val(data.MontoCredito || '');
        $("#txtUnidadesCredito").val(data.UnidadesCredito || '');
        $("#cboEstado").val(data.Activo ? "1" : "0");
        $("#txtNotas").val(data.Notas || '');
    } else {
        // Valores por defecto
        $("#cboTipoPrecio").val("1");
        $("#cboTipoFinanciamiento").val("");
        $("#cboEstado").val("1");
    }

    $("#FormModal").modal("show");
}

// GUARDAR CLIENTE
function Guardar() {
    if (!$("#txtNombres").val().trim() || !$("#txtRFC").val().trim()) {
        alert("Nombre y RFC son obligatorios");
        return;
    }

    const cliente = {
        ClienteID: parseInt($("#txtid").val()) || 0,
        Nombre: $("#txtNombres").val().trim(),
        RFC: $("#txtRFC").val().trim(),
        Email: $("#txtEmail").val().trim(),
        Telefono: $("#txtTelefono").val().trim(),
        Direccion: $("#txtDireccion").val().trim(),
        Ciudad: $("#txtCiudad").val().trim(),
        Estado: $("#txtEstado").val().trim(),
        CodigoPostal: $("#txtCodigoPostal").val().trim(),
        LimiteCredito: $("#txtLimiteCredito").val() ? parseFloat($("#txtLimiteCredito").val()) : null,
        SaldoPendiente: $("#txtSaldoPendiente").val() ? parseFloat($("#txtSaldoPendiente").val()) : null,
        DiasCredito: $("#txtDiasCredito").val() ? parseInt($("#txtDiasCredito").val()) : null,
        MontoCredito: $("#txtMontoCredito").val() ? parseFloat($("#txtMontoCredito").val()) : null,
        UnidadesCredito: $("#txtUnidadesCredito").val() ? parseInt($("#txtUnidadesCredito").val()) : null,
        Notas: $("#txtNotas").val().trim(),

        // AQUÍ ESTÁ EL CAMBIO CLAVE:
        TipoPrecioID: parseInt($("#cboTipoPrecio").val()) || 1,
        TipoFinanciamientoID: $("#cboTipoFinanciamiento").val() ? parseInt($("#cboTipoFinanciamiento").val()) : null,

        Activo: $("#cboEstado").val() === "1"
    };

    $.ajax({
        url: "/Cliente/Guardar",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(cliente),
        success: function (resp) {
            if (resp.resultado) {
                $("#FormModal").modal("hide");
                tabla.ajax.reload();
            } else {
                alert("Error al guardar el cliente");
            }
        },
        error: function () {
            alert("Error de comunicación");
        }
    });
}

// ELIMINAR
function Eliminar(id) {
    if (confirm("¿Eliminar este cliente?")) {
        $.get("/Cliente/Eliminar?id=" + id, function (resp) {
            if (resp.resultado) {
                tabla.ajax.reload();
            } else {
                alert("No se pudo eliminar (puede tener ventas)");
            }
        });
    }
}