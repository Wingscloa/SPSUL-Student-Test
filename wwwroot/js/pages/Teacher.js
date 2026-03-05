// ============================================
// Toggle Active
// ============================================
document.querySelectorAll('.btn-toggle-active').forEach(btn => {
    btn.addEventListener('click', async function () {
        const id = parseInt(this.dataset.id);
        const icon = this.querySelector('i');

        try {
            const res = await fetch('/Teachers/ToggleActive', {
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
    var roleId = fd.get('roleId');
    if (roleId) body.RoleFilterIds = [parseInt(roleId)];
    var titleId = fd.get('titleId');
    if (titleId) body.TitleFilterIds = [parseInt(titleId)];

    fetch('/api/pdf/teachers', {
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
            a.download = 'Ucitele.pdf';
            a.click();
            URL.revokeObjectURL(url);
            toastr.success('PDF export hotov.');
        })
        .catch(function (err) { toastr.error(err.message); });
});

// ============================================
// Create Teacher Modal
// ============================================
var createModal;
document.getElementById('createTeacherBtn')?.addEventListener('click', function () {
    var modalEl = document.getElementById('createTeacherModal');
    createModal = bootstrap.Modal.getOrCreateInstance(modalEl);
    createModal.show();

    setTimeout(function () {
        $('.select2-modal').select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownParent: $('#createTeacherModal')
        });
    }, 200);
});

document.getElementById('createTeacherSubmit')?.addEventListener('click', async function () {
    var firstName = document.getElementById('cFirstName').value.trim();
    var lastName = document.getElementById('cLastName').value.trim();
    var nickName = document.getElementById('cNickName').value.trim();
    var password = document.getElementById('cPassword').value.trim();
    var titleIds = $('#cTitleIds').val()?.map(Number) || [];
    var roleIds = $('#cRoleIds').val()?.map(Number) || [];

    if (!firstName || !lastName || !nickName || !password) {
        toastr.warning('Vypl\u0148te v\u0161echna povinn\u00e1 pole.');
        return;
    }

    var nameRegex = /^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$/;
    if (!nameRegex.test(firstName)) {
        toastr.warning('Jm\u00e9no m\u016f\u017ee obsahovat pouze p\u00edsmena.');
        return;
    }
    if (!nameRegex.test(lastName)) {
        toastr.warning('P\u0159\u00edjmen\u00ed m\u016f\u017ee obsahovat pouze p\u00edsmena.');
        return;
    }

    if (roleIds.length === 0) {
        toastr.warning('Vyberte alespo\u0148 jednu roli.');
        return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/api/teacher', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
            body: JSON.stringify({ FirstName: firstName, LastName: lastName, NickName: nickName, Password: password, TitleIds: titleIds, RoleIds: roleIds })
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
// Edit Teacher Modal
// ============================================
document.querySelectorAll('.teacher-edit-btn').forEach(btn => {
    btn.addEventListener('click', async function () {
        var id = this.dataset.id;
        var modalEl = document.getElementById('editModal');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();

        try {
            var res = await fetch('/api/Config/TeacherEditForm/' + id);
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

    var nameRegex = /^[a-zA-Z\u00C0-\u024F\u1E00-\u1EFF ]+$/;
    if (data.FirstName && !nameRegex.test(data.FirstName)) {
        toastr.warning('Jm\u00e9no m\u016f\u017ee obsahovat pouze p\u00edsmena.');
        return;
    }
    if (data.LastName && !nameRegex.test(data.LastName)) {
        toastr.warning('P\u0159\u00edjmen\u00ed m\u016f\u017ee obsahovat pouze p\u00edsmena.');
        return;
    }

    var btn = this;
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>...';

    try {
        var res = await fetch('/api/teacher', {
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
