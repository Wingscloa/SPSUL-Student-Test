// ============================================
// Toggle Active
// ============================================
document.querySelectorAll('.btn-toggle-active').forEach(btn => {
    btn.addEventListener('click', async function () {
        const id = parseInt(this.dataset.id);
        const icon = this.querySelector('i');

        try {
            const res = await fetch('/Students/ToggleActive', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify(id)
            });
            const data = await res.json();

            if (res.ok) {
                toastr.success(data.message);

                if (data.isActive) {
                    icon.className = 'bi bi-toggle-on text-success';
                    this.title = 'Deaktivovat';
                } else {
                    icon.className = 'bi bi-toggle-off text-secondary';
                    this.title = 'Aktivovat';
                }
            } else {
                toastr.error(data.message || 'Chyba.');
            }
        } catch {
            toastr.error('Chyba.');
        }
    });
});

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
    var classId = fd.get('classId');
    if (classId) body.ClassFilterIds = [parseInt(classId)];

    fetch('/api/pdf/students', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    })
        .then(function (resp) {
            if (!resp.ok) throw new Error('Chyba');
            return resp.blob();
        })
        .then(function (blob) {
            var url = URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = 'Studenti.pdf';
            a.click();
            URL.revokeObjectURL(url);
            toastr.success('PDF export hotov.');
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

    setTimeout(function () {
        $('.select2-bulk').select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownParent: $('#bulkModal'),
            placeholder: 'Vyber t\u0159\u00eddy'
        });
    }, 200);
});

// Add row
document.getElementById('bulkAddRow')?.addEventListener('click', function () {
    addBulkRow('', '');
});

function addBulkRow(first, last) {
    var html = '<div class="row g-2 mb-2 bulk-row">' +
        '<div class="col-5"><input type="text" class="form-control bulk-first" placeholder="Jm\u00e9no" maxlength="64" value="' + escapeHtml(first) + '" /></div>' +
        '<div class="col-5"><input type="text" class="form-control bulk-last" placeholder="P\u0159\u00edjmen\u00ed" maxlength="64" value="' + escapeHtml(last) + '" /></div>' +
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
    if (rows.length <= 1) { toastr.warning('Mus\u00ed b\u00fdt alespo\u0148 jeden \u0159\u00e1dek.'); return; }
    btn.closest('.bulk-row').remove();
});

// Parse pasted text
document.getElementById('bulkParseBtn')?.addEventListener('click', function () {
    var text = document.getElementById('bulkPaste').value.trim();
    if (!text) { toastr.warning('Vlo\u017ete text se studenty.'); return; }

    var lines = text.split('\n').map(function (l) { return l.trim(); }).filter(function (l) { return l.length > 0; });
    var added = 0;

    lines.forEach(function (line) {
        var parts = line.split(/[\t;,]+/).map(function (p) { return p.trim(); });
        if (parts.length === 1) {
            parts = line.split(/\s+/);
        }
        if (parts.length >= 2) {
            addBulkRow(parts[0], parts.slice(1).join(' '));
            added++;
        }
    });

    if (added > 0) {
        document.getElementById('bulkPaste').value = '';
        toastr.success('P\u0159id\u00e1no ' + added + ' \u0159\u00e1dk\u016f.');
    } else {
        toastr.warning('Nepoda\u0159ilo se rozpoznat studenty.');
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
        toastr.warning('Vypl\u0148te jm\u00e9no a p\u0159\u00edjmen\u00ed alespo\u0148 u jednoho studenta.');
        return;
    }

    var nameRegex = /^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$/;
    for (var i = 0; i < students.length; i++) {
        if (!nameRegex.test(students[i].firstName)) {
            toastr.warning('Jm\u00e9no "' + students[i].firstName + '" obsahuje neplatn\u00e9 znaky.');
            return;
        }
        if (!nameRegex.test(students[i].lastName)) {
            toastr.warning('P\u0159\u00edjmen\u00ed "' + students[i].lastName + '" obsahuje neplatn\u00e9 znaky.');
            return;
        }
    }

    var classesIds = $('#bulkClassIds').val();
    classesIds = classesIds ? classesIds.map(Number) : [];

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/Students/BulkCreate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
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
                toastr.error(data.message || 'Chyba.');
            }
        }
    } catch (err) {
        toastr.error('Chyba.');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Vytvo\u0159it studenty';
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

    document.getElementById('editFormBody').innerHTML =
        '<div class="text-center py-4">' +
        '<div class="spinner-border text-orange" role="status"></div>' +
        '<p class="text-muted mt-2">Na\u010d\u00edt\u00e1m formul\u00e1\u0159\u2026</p></div>';

    editModal.show();

    try {
        var res = await fetch('/api/Config/StudentEditForm/' + editStudentId);
        if (!res.ok) throw new Error('Chyba.');
        var html = await res.text();
        document.getElementById('editFormBody').innerHTML = html;

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

    if (!data.FirstName || data.FirstName.length < 2) {
        toastr.warning('Jm\u00e9no mus\u00ed m\u00edt alespo\u0148 2 znaky.'); return;
    }
    if (!data.LastName || data.LastName.length < 2) {
        toastr.warning('P\u0159\u00edjmen\u00ed mus\u00ed m\u00edt alespo\u0148 2 znaky.'); return;
    }

    var nameRegex = /^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$/;
    if (!nameRegex.test(data.FirstName)) {
        toastr.warning('Jm\u00e9no m\u016f\u017ee obsahovat pouze p\u00edsmena.'); return;
    }
    if (!nameRegex.test(data.LastName)) {
        toastr.warning('P\u0159\u00edjmen\u00ed m\u016f\u017ee obsahovat pouze p\u00edsmena.'); return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/api/Student', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
            body: JSON.stringify(data)
        });

        var text = await res.text();

        if (res.ok) {
            toastr.success(text || 'Student aktualizov\u00e1n.');
            if (editModal) editModal.hide();
            setTimeout(function () { location.reload(); }, 800);
        } else {
            toastr.error(text || 'Chyba.');
        }
    } catch (err) {
        toastr.error('Chyba.');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Ulo\u017eit zm\u011bny';
    }
});
