// ============================================
// Select2 init
// ============================================
$(document).ready(function () {
    $('.select2').select2({ theme: 'bootstrap-5', width: '100%' });
});

// ============================================
// PDF Export
// ============================================
document.getElementById('exportBtn')?.addEventListener('click', function () {
    var form = document.querySelector('aside form');
    var fd = new FormData(form);
    var body = {};
    var name = fd.get('name');
    if (name) body.SearchFilter = name;
    var active = fd.get('active');
    if (active === 'true') body.ActiveFilter = true;
    else if (active === 'false') body.ActiveFilter = false;

    fetch('/api/pdf/students', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    })
        .then(function (resp) {
            if (!resp.ok) throw new Error('Chyba při generování PDF');
            return resp.blob();
        })
        .then(function (blob) {
            var url = URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = 'Studenti.pdf';
            a.click();
            URL.revokeObjectURL(url);
        })
        .catch(function (err) { toastr.error(err.message); });
});

// ============================================
// Bulk Create Modal
// ============================================
var bulkModal;
document.getElementById('bulkCreateBtn')?.addEventListener('click', function () {
    var modalEl = document.getElementById('bulkModal');
    bulkModal = bootstrap.Modal.getOrCreateInstance(modalEl);
    bulkModal.show();

    // Init select2 inside modal
    setTimeout(function () {
        $('.select2-bulk').select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownParent: $('#bulkModal'),
            placeholder: 'Vyber třídy'
        });
    }, 200);
});

// Add row
document.getElementById('bulkAddRow')?.addEventListener('click', function () {
    addBulkRow('', '');
});

function addBulkRow(first, last) {
    var html = '<div class="row g-2 mb-2 bulk-row">' +
        '<div class="col-5"><input type="text" class="form-control bulk-first" placeholder="Jméno" maxlength="64" value="' + escapeHtml(first) + '" /></div>' +
        '<div class="col-5"><input type="text" class="form-control bulk-last" placeholder="Příjmení" maxlength="64" value="' + escapeHtml(last) + '" /></div>' +
        '<div class="col-2 d-flex align-items-center"><button type="button" class="btn btn-sm btn-outline-danger bulk-remove-row" title="Odebrat"><i class="bi bi-x-lg"></i></button></div>' +
        '</div>';
    document.getElementById('bulkRows').insertAdjacentHTML('beforeend', html);
}

function escapeHtml(str) {
    var d = document.createElement('div');
    d.textContent = str;
    return d.innerHTML;
}

// Remove row (delegation)
document.addEventListener('click', function (e) {
    var btn = e.target.closest('.bulk-remove-row');
    if (!btn) return;
    var rows = document.querySelectorAll('.bulk-row');
    if (rows.length <= 1) { toastr.warning('Musí být alespoň jeden řádek.'); return; }
    btn.closest('.bulk-row').remove();
});

// Parse pasted text
document.getElementById('bulkParseBtn')?.addEventListener('click', function () {
    var text = document.getElementById('bulkPaste').value.trim();
    if (!text) { toastr.warning('Vložte text se studenty.'); return; }

    var lines = text.split('\n').map(function (l) { return l.trim(); }).filter(function (l) { return l.length > 0; });
    var added = 0;

    lines.forEach(function (line) {
        // support tab or space separator
        var parts = line.split(/[\t;,]+/).map(function (p) { return p.trim(); });
        if (parts.length === 1) {
            // try splitting by space: "Jan Novák"
            parts = line.split(/\s+/);
        }
        if (parts.length >= 2) {
            addBulkRow(parts[0], parts.slice(1).join(' '));
            added++;
        }
    });

    if (added > 0) {
        document.getElementById('bulkPaste').value = '';
        toastr.success('Přidáno ' + added + ' řádků.');
    } else {
        toastr.warning('Nepodařilo se rozpoznat studenty. Formát: Jméno Příjmení');
    }
});

// Submit bulk create
document.getElementById('bulkSubmitBtn')?.addEventListener('click', async function () {
    var rows = document.querySelectorAll('.bulk-row');
    var students = [];

    rows.forEach(function (row) {
        var first = row.querySelector('.bulk-first').value.trim();
        var last = row.querySelector('.bulk-last').value.trim();
        if (first && last) {
            students.push({ firstName: first, lastName: last });
        }
    });

    if (students.length === 0) {
        toastr.warning('Vyplňte jméno a příjmení alespoň u jednoho studenta.');
        return;
    }

    var classesIds = $('#bulkClassIds').val();
    classesIds = classesIds ? classesIds.map(Number) : [];

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Vytvářím...';

    try {
        var res = await fetch('/Students/BulkCreate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ students: students, classesIds: classesIds })
        });

        var data = await res.json();

        if (res.ok) {
            toastr.success(data.message);
            if (bulkModal) bulkModal.hide();
            setTimeout(function () { location.reload(); }, 800);
        } else {
            if (data.errors && data.errors.length > 0) {
                data.errors.forEach(function (e) { toastr.error(e); });
            } else {
                toastr.error(data.message || 'Chyba při vytváření.');
            }
        }
    } catch (err) {
        toastr.error('Chyba komunikace se serverem.');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Vytvořit studenty';
    }
});

// ============================================
// Edit Student Modal
// ============================================
var editModal;
var editStudentId = null;

document.addEventListener('click', async function (e) {
    var btn = e.target.closest('.student-edit-btn');
    if (!btn) return;
    e.preventDefault();

    editStudentId = parseInt(btn.dataset.id);
    var modalEl = document.getElementById('editModal');
    editModal = bootstrap.Modal.getOrCreateInstance(modalEl);

    // Show spinner while loading
    document.getElementById('editFormBody').innerHTML =
        '<div class="text-center py-4">' +
        '<div class="spinner-border text-orange" role="status"></div>' +
        '<p class="text-muted mt-2">Načítám formulář…</p></div>';

    editModal.show();

    try {
        var res = await fetch('/api/Config/StudentEditForm/' + editStudentId);
        if (!res.ok) throw new Error('Nepodařilo se načíst formulář.');
        var html = await res.text();
        document.getElementById('editFormBody').innerHTML = html;

        // Init select2 inside the edit modal
        setTimeout(function () {
            $('#editFormBody .select2').select2({
                theme: 'bootstrap-5',
                width: '100%',
                dropdownParent: $('#editModal')
            });
        }, 100);
    } catch (err) {
        document.getElementById('editFormBody').innerHTML =
            '<div class="alert alert-danger"><i class="bi bi-exclamation-triangle me-2"></i>' + err.message + '</div>';
        console.error(err);
    }
});

document.getElementById('editSubmitBtn')?.addEventListener('click', async function () {
    var formBody = document.getElementById('editFormBody');
    var inputs = formBody.querySelectorAll('input, select');
    var data = {};

    inputs.forEach(function (el) {
        var name = el.getAttribute('name');
        if (!name) return;

        if (el.tagName === 'SELECT' && el.multiple) {
            var vals = Array.from(el.selectedOptions).map(function (o) { return parseInt(o.value); }).filter(function (v) { return !isNaN(v); });
            data[name] = vals;
        } else if (el.type === 'checkbox') {
            data[name] = el.checked;
        } else if (el.type === 'hidden' && name.toLowerCase().includes('id')) {
            var parsed = parseInt(el.value);
            if (!isNaN(parsed)) data[name] = parsed;
        } else {
            data[name] = el.value.trim();
        }
    });

    // Validation
    if (!data.FirstName || data.FirstName.length < 2) {
        toastr.warning('Jméno musí mít alespoň 2 znaky.'); return;
    }
    if (!data.LastName || data.LastName.length < 2) {
        toastr.warning('Příjmení musí mít alespoň 2 znaky.'); return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Ukládám...';

    try {
        var res = await fetch('/api/Student', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        var text = await res.text();

        if (res.ok) {
            toastr.success(text || 'Student aktualizován.');
            if (editModal) editModal.hide();
            setTimeout(function () { location.reload(); }, 800);
        } else {
            toastr.error(text || 'Chyba při aktualizaci.');
        }
    } catch (err) {
        toastr.error('Chyba komunikace se serverem.');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Uložit změny';
    }
});