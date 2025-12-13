let tabla;
let lotesActuales = [];

$(document).ready(function () {
    cargarCatalogos();
    inicializarTabla();

    // Guardar producto
    $('#btnGuardarProducto').click(Guardar);
});

// ==================== CATÁLOGOS ====================
function cargarCatalogos() {
    $.get('/Producto/ObtenerCatalogos')
        .done(res => {
            const $cat = $('#cboCategoria').empty().append('<option value="">Seleccionar categoría</option>');
            res.categorias.forEach(c => $cat.append(`<option value="${c.CategoriaID}">${c.Nombre}</option>`));

            const $sat = $('#cboClaveSAT').empty().append('<option value="">Seleccionar clave SAT</option>');
            res.clavesSAT.forEach(c => $sat.append(`<option value="${c.ClaveProdServSATID}">${c.ClaveProdServSATID} - ${c.Descripcion}</option>`));

            const $uni = $('#cboUnidadSAT').empty().append('<option value="">Seleccionar unidad</option>');
            res.unidadesSAT.forEach(u => $uni.append(`<option value="${u.ClaveUnidadSAT}">${u.ClaveUnidadSAT} - ${u.Descripcion}</option>`));

            const $iva = $('#cboTasaIVA').empty().append('<option value="">Seleccionar IVA</option>');
            res.tasasIVA.forEach(t => $iva.append(`<option value="${t.TasaIVAID}">${t.TextoCombo}</option>`));

            $('#cboTasaIEPS').empty().append('<option value="">-- Sin IEPS --</option>');
            res.tasasIEPS.forEach(t => $('#cboTasaIEPS').append(`<option value="${t.TasaIEPSID}">${t.TextoCombo}</option>`));
        })
        .fail(() => toastr.error("Error al cargar los catálogos"));
}

// ==================== TABLA PRINCIPAL ====================
function inicializarTabla() {
    tabla = $('#tbProductos').DataTable({
        ajax: { url: '/Producto/Obtener', dataSrc: 'data' },
        columns: [
            { data: 'CodigoInterno', defaultContent: '-' },
            { data: 'Nombre' },
            { data: 'Categoria' },
            { data: 'ClaveProdServSATID' },
            { data: 'UnidadSAT' },
            { data: 'TasaIVA', render: d => d + '%' },
            { data: 'TasaIEPS', render: d => d > 0 ? d + '%' : '-' },
            { data: 'Estatus', render: d => d ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-secondary">Inactivo</span>' },
            {
                data: null, orderable: false, render: function (data) {
                    return `
                        <button class="btn btn-warning btn-sm" onclick="abrirModal(${data.ProductoID})"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-info btn-sm" onclick="verLotes(${data.ProductoID}, '${data.Nombre.replace(/'/g, "\\'")}')"><i class="fas fa-cubes"></i></button>
                        <button class="btn btn-${data.Estatus ? 'danger' : 'success'} btn-sm" onclick="CambiarEstatus(${data.ProductoID}, ${data.Estatus})">
                            <i class="fas ${data.Estatus ? 'fa-ban' : 'fa-check'}"></i>
                        </button>`;
                }
            }
        ],
        language: { url: '/Content/Plugins/datatables/i18n/es-ES.json' },
        responsive: true,
        pageLength: 25
    });
}

// ==================== ABRIR MODAL ====================
function abrirModal(id = 0) {
    $('#formProducto')[0].reset();
    $('#txtProductoID').val('0');
    $('#modalTitulo').text('Nuevo Producto');
    lotesActuales = [];
    $('#tbLotes tbody').empty().append('<tr><td colspan="7" class="text-center text-muted">Sin lotes</td></tr>');

    if (id === 0) {
        $('#modalProducto').modal('show');
        return;
    }

    $.get('/Producto/ObtenerPorId', { id })
        .done(p => {
            $('#txtProductoID').val(p.ProductoID);
            $('#txtNombre').val(p.Nombre);
            $('#txtCodigoInterno').val(p.CodigoInterno || '');
            $('#cboCategoria').val(p.CategoriaID);
            $('#cboClaveSAT').val(p.ClaveProdServSATID);
            $('#cboUnidadSAT').val(p.ClaveUnidadSAT);
            $('#cboTasaIVA').val(p.TasaIVAID);
            $('#cboTasaIEPS').val(p.TasaIEPSID || '');
            $('#modalTitulo').text('Editar Producto - ' + p.Nombre);

            $.get('/Producto/ObtenerLotes', { productoId: id })
                .done(lotes => {
                    lotesActuales = lotes;
                    actualizarTablaLotes();
                    $('#modalProducto').modal('show');
                });
        })
        .fail(() => toastr.error("Error al cargar producto"));
}

// ==================== ACTUALIZAR TABLA LOTES ====================
// --------------------
// UTIL: parseador robusto de fechas SQL/.NET -> Date
// --------------------
function parseSqlDate(fecha) {
    if (!fecha && fecha !== 0) return null;

    // Si ya es Date
    if (fecha instanceof Date) {
        if (!isNaN(fecha)) return fecha;
        return null;
    }

    // Asegurarnos que sea string
    let s = String(fecha).trim();
    if (!s) return null;

    // 1) Detectar formato /Date(1234567890)/
    const msMatch = /\/Date\((-?\d+)(?:[+-]\d+)?\)\//.exec(s);
    if (msMatch) {
        const ts = parseInt(msMatch[1], 10);
        const d = new Date(ts);
        return isNaN(d) ? null : d;
    }

    // 2) Intento directo (reemplaza espacio por T) -> "YYYY-MM-DDTHH:mm:ss.sss"
    let iso = s.replace(' ', 'T');

    // Algunos servidores devuelven "/YYYY-MM-DD HH:mm:ss/" o tienen fracciones con coma
    // Normalizar coma decimal en milisegundos (e.g. 2025-11-29 08:48:46,917 -> 2025-11-29T08:48:46.917)
    iso = iso.replace(/,(\d{1,3})$/, '.$1');

    // If there are more than 3 fractional digits, trim/pad to 3
    iso = iso.replace(/\.(\d{4,})$/, (m, g1) => '.' + g1.slice(0, 3));

    // Try plain new Date(iso)
    let d = new Date(iso);
    if (!isNaN(d)) return d;

    // 3) Try with trailing 'Z' (treat as UTC)
    d = new Date(iso + 'Z');
    if (!isNaN(d)) return d;

    // 4) Manual parse components: YYYY-MM-DD HH:mm:ss[.fff]
    const parts = s.match(/^(\d{4})-(\d{2})-(\d{2})[ T](\d{2}):(\d{2}):(\d{2})(?:[.,](\d{1,6}))?$/);
    if (parts) {
        const y = parseInt(parts[1], 10);
        const mo = parseInt(parts[2], 10) - 1;
        const day = parseInt(parts[3], 10);
        const hh = parseInt(parts[4], 10);
        const mm = parseInt(parts[5], 10);
        const ss = parseInt(parts[6], 10);
        // milisegundos: ajustar a 3 dígitos (pad/trunc)
        let ms = 0;
        if (parts[7]) {
            ms = parts[7].length === 3 ? parseInt(parts[7], 10)
                : parts[7].length > 3 ? parseInt(parts[7].slice(0, 3), 10)
                    : parseInt((parts[7] + '000').slice(0, 3), 10);
        }
        d = new Date(y, mo, day, hh, mm, ss, ms); // local time
        if (!isNaN(d)) return d;
    }

    // 5) Fallback: intentar Date.parse en distintos intentos
    const tryCandidates = [s, s.replace(' ', 'T'), s.replace(' ', 'T') + 'Z'];
    for (let cand of tryCandidates) {
        d = new Date(cand);
        if (!isNaN(d)) return d;
    }

    // Si todo falla, devolvemos null
    return null;
}

// --------------------
// actualizarTablaLotes usando parseSqlDate
// --------------------
function actualizarTablaLotes() {
    const tbody = $('#tbLotes tbody').empty();
    if (lotesActuales.length === 0) {
        tbody.append('<tr><td colspan="7" class="text-center text-muted">No hay lotes registrados</td></tr>');
        return;
    }

    lotesActuales.forEach(l => {
        // FECHA DE ENTRADA
        const entradaDate = parseSqlDate(l.FechaEntrada);
        const entradaText = entradaDate ? entradaDate.toLocaleString() : (l.FechaEntrada || 'Invalid date');

        // FECHA DE CADUCIDAD
        const cadDate = parseSqlDate(l.FechaCaducidad);
        const cadText = cadDate ? cadDate.toLocaleDateString() : (l.FechaCaducidad ? l.FechaCaducidad : '—');

        tbody.append(`
            <tr>
                <td>${l.LoteID ?? ''}</td>
                <td>${entradaText}</td>
                <td>${cadText}</td>
                <td>${l.CantidadTotal ?? ''}</td>
                <td><strong>${l.CantidadDisponible ?? ''}</strong></td>
                <td>$${parseFloat(l.PrecioCompra || 0).toFixed(2)}</td>
                <td>$${parseFloat(l.PrecioVenta || 0).toFixed(2)}</td>
            </tr>
        `);
    });
}


// ==================== GUARDAR ====================
function Guardar() {
    if (!$('#txtNombre').val().trim()) return toastr.warning('El nombre es obligatorio');
    if (!$('#cboCategoria').val()) return toastr.warning('Selecciona una categoría');
    if (!$('#cboClaveSAT').val()) return toastr.warning('Selecciona clave SAT');
    if (!$('#cboUnidadSAT').val()) return toastr.warning('Selecciona unidad SAT');
    if (!$('#cboTasaIVA').val()) return toastr.warning('Selecciona tasa de IVA');

    const producto = {
        ProductoID: parseInt($('#txtProductoID').val() || 0),
        Nombre: $('#txtNombre').val().trim(),
        CodigoInterno: $('#txtCodigoInterno').val().trim() || null,
        CategoriaID: parseInt($('#cboCategoria').val()),
        ClaveProdServSATID: $('#cboClaveSAT').val(),
        ClaveUnidadSAT: $('#cboUnidadSAT').val(),
        TasaIVAID: parseInt($('#cboTasaIVA').val()),
        TasaIEPSID: $('#cboTasaIEPS').val() ? parseInt($('#cboTasaIEPS').val()) : null
    };

    $.post('/Producto/Guardar', producto)
        .done(res => {
            if (res.success) {
                toastr.success('Producto guardado correctamente');
                $('#modalProducto').modal('hide');
                tabla.ajax.reload();
            } else toastr.error('Error al guardar el producto');
        })
        .fail(() => toastr.error('Error de conexión'));
}

// ==================== CAMBIAR ESTATUS ====================
function CambiarEstatus(id, estatusActual) {
    const nuevo = !estatusActual;
    if (!confirm(`¿${nuevo ? 'Activar' : 'Desactivar'} este producto?`)) return;

    $.post('/Producto/CambiarEstatus', { id, estatus: nuevo })
        .done(res => {
            if (res.resultado) {
                toastr.success('Estatus actualizado');
                tabla.ajax.reload();
            }
        });
}

// ==================== VER LOTES ====================
function verLotes(productoId, nombreProducto) {
    $('#nombreProductoLote').text(nombreProducto);
    $('#linkCrearLote').attr('href', `/Producto/CrearLote?productoId=${productoId}`);

    $.get('/Producto/ObtenerLotes', { productoId })
        .done(lotes => {
            lotesActuales = lotes;
            actualizarTablaLotes();
            $('#modalLotes tbody').html($('#tbLotes tbody').html());
            $('#modalLotes').modal('show');
        });
}
// ==================== ACTUALIZAR TABLA LOTES (CORREGIDA) ====================
function actualizarTablaLotes() {
    const tbody = $('#tbLotes tbody').empty();
    const tbodyModal = $('#modalLotes tbody').empty(); // también actualizamos el modal de lotes

    if (lotesActuales.length === 0) {
        const noDataRow = '<tr><td colspan="8" class="text-center text-muted">No hay lotes registrados</td></tr>';
        tbody.append(noDataRow);
        tbodyModal.append(noDataRow);
        return;
    }

    lotesActuales.forEach(l => {
        // --- Parseo de fechas ---
        const entradaDate = parseSqlDate(l.FechaEntrada);
        const entradaText = entradaDate ? entradaDate.toLocaleString() : (l.FechaEntrada || '—');

        const cadDate = parseSqlDate(l.FechaCaducidad);
        const cadText = cadDate ? cadDate.toLocaleDateString() : (l.FechaCaducidad ? l.FechaCaducidad : '—');

        // --- Fila para el modal de edición y el modal de ver lotes ---
        const fila = `
            <tr>
                <td>${l.LoteID ?? ''}</td>
                <td>${entradaText}</td>
                <td>${cadText}</td>
                <td>${l.CantidadTotal ?? ''}</td>
                <td><strong>${l.CantidadDisponible ?? ''}</strong></td>
                <td>$${parseFloat(l.PrecioCompra || 0).toFixed(2)}</td>
                <td>$${parseFloat(l.PrecioVenta || 0).toFixed(2)}</td>
                <td class="text-center">
                    <a href="/Producto/EditarLote?loteId=${l.LoteID}" class="btn btn-warning btn-xs" title="Editar lote">
                        <i class="fas fa-edit"></i>
                    </a>
                    <button class="btn btn-danger btn-xs" onclick="abrirAjusteModal(${l.LoteID})" title="Ajuste / Merma">
                        <i class="fas fa-minus-circle"></i>
                    </button>
                </td>
            </tr>`;

        tbody.append(fila);
        tbodyModal.append(fila);
    });
}

// Nueva función para modal de ajuste
function abrirAjusteModal(loteId) {
    // Crea un modal dinámico o usa uno existente
    const modalHtml = `
        <div class="modal fade" id="modalAjuste" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-danger text-white">
                        <h5>Ajuste o Merma</h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <form id="formAjuste">
                            <input type="hidden" id="txtLoteIdAjuste" value="${loteId}" />
                            <div class="form-group">
                                <label>Cantidad a ajustar *</label>
                                <input type="number" id="txtCantidadAjuste" class="form-control" min="1" required />
                            </div>
                            <div class="form-group">
                                <label>Tipo *</label>
                                <select id="cboTipoAjuste" class="form-control">
                                    <option value="AJUSTE">Ajuste</option>
                                    <option value="MERMA">Merma</option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label>Motivo *</label>
                                <textarea id="txtMotivoAjuste" class="form-control" required></textarea>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                        <button type="button" class="btn btn-danger" onclick="realizarAjuste()">Confirmar</button>
                    </div>
                </div>
            </div>
        </div>`;
    $('body').append(modalHtml); // Agrega modal dinámicamente
    $('#modalAjuste').modal('show');
}

// Nueva función para enviar ajuste
function realizarAjuste() {
    const loteId = $('#txtLoteIdAjuste').val();
    const cantidad = parseFloat($('#txtCantidadAjuste').val());
    const tipo = $('#cboTipoAjuste').val();
    const motivo = $('#txtMotivoAjuste').val().trim();

    if (!cantidad || cantidad <= 0 || !motivo) return toastr.warning('Completa los campos');

    $.post('/Producto/AjustarLote', { loteId, cantidadAjuste: cantidad, tipo, motivo })
        .done(res => {
            if (res.success) {
                toastr.success('Ajuste realizado y póliza generada');
                $('#modalAjuste').modal('hide');
                tabla.ajax.reload(); // Recarga tabla principal
            } else toastr.error(res.message || 'Error al ajustar');
        })
        .fail(() => toastr.error('Error de conexión'));
}
