// ============================================
// Toggle Active
// ============================================
document.querySelectorAll('.btn-toggle-active').forEach(btn => {
    btn.addEventListener('click', async function () {
        const id = parseInt(this.dataset.id);
        const icon = this.querySelector('i');

        try {
            const res = await fetch('/ClassesPage/ToggleActive', {
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
    var fieldId = fd.get('fieldId');
    if (fieldId) body.FieldFilterIds = [parseInt(fieldId)];

    fetch('/api/pdf/classes', {
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
            a.download = 'Tridy.pdf';
            a.click();
            URL.revokeObjectURL(url);
            toastr.success('PDF export hotov.');
        })
        .catch(function (err) { toastr.error(err.message); });
});

// ============================================
// Create Class Modal
// ============================================
var createModal;
document.getElementById('createClassBtn')?.addEventListener('click', function () {
    var modalEl = document.getElementById('createClassModal');
    createModal = bootstrap.Modal.getOrCreateInstance(modalEl);
    createModal.show();

    setTimeout(function () {
        $('.select2-modal').select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownParent: $('#createClassModal')
        });
    }, 200);
});

document.getElementById('createClassSubmit')?.addEventListener('click', async function () {
    var name = document.getElementById('cClassName').value.trim();
    var startFrom = parseInt(document.getElementById('cStartFrom').value);
    var endTo = parseInt(document.getElementById('cEndTo').value);
    var fieldIds = $('#cFieldIds').val()?.map(Number) || [];

    if (!name) {
        toastr.warning('Vypl\u0148te n\u00e1zev t\u0159\u00eddy.');
        return;
    }

    if (isNaN(startFrom) || isNaN(endTo)) {
        toastr.warning('Vypl\u0148te rok za\u010d\u00e1tku a konce studia.');
        return;
    }

    if (startFrom > endTo) {
        toastr.warning('Rok zah\u00e1jen\u00ed nesm\u00ed b\u00fdt v\u011bt\u0161\u00ed ne\u017e rok ukon\u010den\u00ed.');
        return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/api/classes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
            body: JSON.stringify({ Name: name, StartFrom: startFrom, EndTo: endTo, StudentFieldIds: fieldIds })
        });

        var text = await res.text();

        if (res.ok) {
            toastr.success(text);
            if (createModal) createModal.hide();
            setTimeout(function () { location.reload(); }, 800);
        } else {
            toastr.error(text || 'Chyba.');
        }
    } catch {
        toastr.error('Chyba.');
    }

    btn.disabled = false;
    btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Vytvo\u0159it';
});

// ============================================
// Edit Class Modal
// ============================================
document.querySelectorAll('.class-edit-btn').forEach(btn => {
    btn.addEventListener('click', async function () {
        var id = this.dataset.id;
        var modalEl = document.getElementById('editModal');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();

        try {
            var res = await fetch('/api/Config/ClassesEditForm/' + id);
            if (res.ok) {
                document.getElementById('editFormBody').innerHTML = await res.text();
                setTimeout(function () {
                    $('#editModal .select2').select2({
                        theme: 'bootstrap-5',
                        width: '100%',
                        dropdownParent: $('#editModal')
                    });
                }, 100);
            } else {
                toastr.error('Chyba.');
            }
        } catch {
            toastr.error('Chyba.');
        }
    });
});

document.getElementById('editSubmitBtn')?.addEventListener('click', async function () {
    var form = document.getElementById('editFormBody');
    var inputs = form.querySelectorAll('input, select, textarea');
    var data = {};

    inputs.forEach(function (input) {
        var name = input.name;
        if (!name) return;

        if (input.tagName === 'SELECT' && input.multiple) {
            var vals = Array.from(input.selectedOptions).map(o => Number(o.value));
            data[name] = vals;
        } else if (input.type === 'checkbox') {
            data[name] = input.checked;
        } else {
            var val = input.value.trim();
            if (!isNaN(val) && val !== '' && name !== 'SearchFilter') {
                data[name] = Number(val);
            } else if (val === 'on') {
                data[name] = true;
            } else if (val === 'off') {
                data[name] = false;
            } else {
                data[name] = val || null;
            }
        }
    });

    if (data.StartFrom && data.EndTo && Number(data.StartFrom) > Number(data.EndTo)) {
        toastr.warning('Rok zah\u00e1jen\u00ed nesm\u00ed b\u00fdt v\u011bt\u0161\u00ed ne\u017e rok ukon\u010den\u00ed.');
        return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/api/classes', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
            body: JSON.stringify(data)
        });

        var text = await res.text();

        if (res.ok) {
            toastr.success(text);
            bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
            setTimeout(function () { location.reload(); }, 800);
        } else {
            toastr.error(text || 'Chyba.');
        }
    } catch {
        toastr.error('Chyba.');
    }

    btn.disabled = false;
    btn.innerHTML = '<i class="bi bi-check2-circle me-1"></i>Ulo\u017eit zm\u011bny';
});

// ============================================
// Assign Students Modal
// ============================================
document.querySelectorAll('.btn-assign').forEach(btn => {
    btn.addEventListener('click', function () {
        var classId = this.dataset.classId;
        var className = this.dataset.className;
        document.getElementById('assignClassId').value = classId;
        document.getElementById('assignClassName').textContent = className;

        // Reset checkboxes
        document.querySelectorAll('.student-checkbox').forEach(cb => { cb.checked = false; });

        // Mark already-assigned students (checked, can uncheck to remove)
        var assignedCount = 0;
        document.querySelectorAll('.student-item').forEach(item => {
            var classes = (item.dataset.classes || '').split(',');
            var badge = item.querySelector('.assign-status-badge');
            var cb = item.querySelector('.student-checkbox');
            if (classes.includes(classId)) {
                cb.checked = true;
                cb.dataset.wasAssigned = 'true';
                badge.innerHTML = '<span class="badge bg-success">Ve t\u0159\u00edd\u011b</span>';
                assignedCount++;
            } else {
                cb.dataset.wasAssigned = 'false';
                badge.innerHTML = '';
            }
        });

        var infoEl = document.getElementById('alreadyAssignedInfo');
        if (infoEl) infoEl.textContent = assignedCount > 0 ? assignedCount + ' ve t\u0159\u00edd\u011b' : '';
        updateSelectedCount();
    });
});

// Live badge update on checkbox toggle
document.addEventListener('change', function (e) {
    if (!e.target.classList.contains('student-checkbox')) return;
    var item = e.target.closest('.student-item');
    if (!item) return;
    var badge = item.querySelector('.assign-status-badge');
    var wasAssigned = e.target.dataset.wasAssigned === 'true';

    if (wasAssigned && !e.target.checked) {
        badge.innerHTML = '<span class="badge bg-danger">Bude odebr\u00e1n</span>';
    } else if (wasAssigned && e.target.checked) {
        badge.innerHTML = '<span class="badge bg-success">Ve t\u0159\u00edd\u011b</span>';
    } else if (!wasAssigned && e.target.checked) {
        badge.innerHTML = '<span class="badge bg-warning text-dark">Bude p\u0159id\u00e1n</span>';
    } else {
        badge.innerHTML = '';
    }
    updateSelectedCount();
});

// Search filter
document.getElementById('studentSearch')?.addEventListener('input', function () {
    var q = this.value.toLowerCase();
    document.querySelectorAll('.student-item').forEach(item => {
        var name = item.dataset.name || '';
        item.style.display = name.includes(q) ? '' : 'none';
    });
});

// Select all
document.getElementById('selectAllStudents')?.addEventListener('change', function () {
    var checked = this.checked;
    document.querySelectorAll('.student-item').forEach(item => {
        if (item.style.display === 'none') return;
        var cb = item.querySelector('.student-checkbox');
        cb.checked = checked;
        cb.dispatchEvent(new Event('change', { bubbles: true }));
    });
});

function updateSelectedCount() {
    var newlyChecked = 0;
    var toRemove = 0;
    document.querySelectorAll('.student-checkbox').forEach(cb => {
        var wasAssigned = cb.dataset.wasAssigned === 'true';
        if (!wasAssigned && cb.checked) newlyChecked++;
        if (wasAssigned && !cb.checked) toRemove++;
    });
    var el = document.getElementById('selectedStudentCount');
    var parts = [];
    if (newlyChecked > 0) parts.push('+' + newlyChecked);
    if (toRemove > 0) parts.push('-' + toRemove);
    if (el) el.textContent = parts.length > 0 ? parts.join(' / ') : '0 zm\u011bn';
}

// Submit assign / unassign
document.getElementById('assignBtn')?.addEventListener('click', async function () {
    var classId = parseInt(document.getElementById('assignClassId').value);
    var toAdd = [];
    var toRemove = [];

    document.querySelectorAll('.student-checkbox').forEach(cb => {
        var wasAssigned = cb.dataset.wasAssigned === 'true';
        var sid = parseInt(cb.value);
        if (!wasAssigned && cb.checked) toAdd.push(sid);
        if (wasAssigned && !cb.checked) toRemove.push(sid);
    });

    if (toAdd.length === 0 && toRemove.length === 0) {
        toastr.warning('\u017d\u00e1dn\u00e9 zm\u011bny k ulo\u017een\u00ed.');
        return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var messages = [];

        if (toAdd.length > 0) {
            var res = await fetch('/ClassesPage/AssignStudents', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify({ classId: classId, studentIds: toAdd })
            });
            var data = await res.json();
            if (res.ok) messages.push(data.message);
            else toastr.error(data.message);
        }

        if (toRemove.length > 0) {
            var res2 = await fetch('/ClassesPage/UnassignStudents', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify({ classId: classId, studentIds: toRemove })
            });
            var data2 = await res2.json();
            if (res2.ok) messages.push(data2.message);
            else toastr.error(data2.message);
        }

        if (messages.length > 0) {
            toastr.success(messages.join(' '));
            bootstrap.Modal.getInstance(document.getElementById('assignModal')).hide();
            setTimeout(function () { location.reload(); }, 800);
        }
    } catch {
        toastr.error('Chyba.');
    }

    btn.disabled = false;
    btn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Ulo\u017eit zm\u011bny';
});
