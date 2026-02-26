//// Build assignment map: testId => [studentId, ...]
//const assignmentMap = {};
//@foreach(var t in Model)
//{
//    if (t.StudentTests != null && t.StudentTests.Any()) {
//        var ids = string.Join(",", t.StudentTests.Select(st => st.StudentId));
//        <text>assignmentMap[@t.TestId] = [@ids];</text>
//    }
//}

// ============================================
// ASSIGN MODAL
// ============================================
let currentAssignTestId = 0;

document.querySelectorAll('.btn-assign').forEach(btn => {
    btn.addEventListener('click', function () {
        currentAssignTestId = parseInt(this.dataset.testId);
        document.getElementById('assignTestId').value = currentAssignTestId;
        document.getElementById('assignTestName').textContent = this.dataset.testName;
        document.getElementById('assignResults').style.display = 'none';
        document.getElementById('assignFooter').style.display = '';
        document.getElementById('classFilter').value = '';
        document.getElementById('studentSearch').value = '';
        document.getElementById('selectAllStudents').checked = false;

        const assigned = assignmentMap[currentAssignTestId] || [];
        let alreadyCount = 0;

        document.querySelectorAll('.student-item').forEach(item => {
            const studentId = parseInt(item.dataset.studentId);
            const cb = item.querySelector('.student-checkbox');
            const badge = item.querySelector('.assign-status-badge');
            const isAssigned = assigned.includes(studentId);

            // Reset visibility
            item.style.display = '';
            cb.checked = false;
            cb.disabled = isAssigned;
            item.classList.remove('student-selected', 'student-disabled');

            if (isAssigned) {
                item.classList.add('student-disabled');
                badge.innerHTML = '<span class="badge bg-success"><i class="bi bi-check2 me-1"></i>Přiřazeno</span>';
                alreadyCount++;
            } else {
                badge.innerHTML = '';
            }
        });

        document.getElementById('alreadyAssignedInfo').textContent =
            alreadyCount > 0 ? alreadyCount + ' již přiřazeno' : '';

        updateAssignCount();
        resetAssignBtn();
    });
});

// Class filter
document.getElementById('classFilter').addEventListener('change', function () {
    filterStudentList();
    document.getElementById('selectAllStudents').checked = false;
});

// Search filter
document.getElementById('studentSearch').addEventListener('input', function () {
    filterStudentList();
    document.getElementById('selectAllStudents').checked = false;
});

function filterStudentList() {
    const classId = document.getElementById('classFilter').value;
    const search = document.getElementById('studentSearch').value.toLowerCase().trim();

    document.querySelectorAll('.student-item').forEach(item => {
        const matchesClass = !classId || item.dataset.classes.split(',').includes(classId);
        const matchesSearch = !search || item.dataset.name.includes(search);
        item.style.display = (matchesClass && matchesSearch) ? '' : 'none';
    });
}

// Select all (only visible + available)
document.getElementById('selectAllStudents').addEventListener('change', function () {
    const checked = this.checked;
    document.querySelectorAll('.student-item').forEach(item => {
        if (item.style.display === 'none') return;
        const cb = item.querySelector('.student-checkbox');
        if (cb.disabled) return; // skip already assigned
        cb.checked = checked;
        item.classList.toggle('student-selected', checked);
    });
    updateAssignCount();
});

// Individual checkbox change
document.querySelectorAll('.student-checkbox').forEach(cb => {
    cb.addEventListener('change', function () {
        const item = this.closest('.student-item');
        item.classList.toggle('student-selected', this.checked);
        updateAssignCount();

        // Update select-all state
        const visible = document.querySelectorAll('.student-item:not([style*="display: none"]) .student-checkbox:not(:disabled)');
        const visibleChecked = document.querySelectorAll('.student-item:not([style*="display: none"]) .student-checkbox:not(:disabled):checked');
        document.getElementById('selectAllStudents').checked =
            visible.length > 0 && visible.length === visibleChecked.length;
    });
});

function updateAssignCount() {
    const count = document.querySelectorAll('.student-checkbox:checked').length;
    const badge = document.getElementById('selectedStudentCount');
    badge.textContent = count + ' vybráno';
    badge.style.background = count > 0 ? '#ff8a00' : '#6c757d';
}

function resetAssignBtn() {
    const btn = document.getElementById('assignBtn');
    btn.disabled = false;
    btn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Přiřadit vybraným';
}

// Assign
document.getElementById('assignBtn').addEventListener('click', async function () {
    const testId = currentAssignTestId;
    const studentIds = [...document.querySelectorAll('.student-checkbox:checked:not(:disabled)')].map(cb => parseInt(cb.value));

    if (!studentIds.length) {
        toastr.error('Vyberte alespoň jednoho studenta.');
        return;
    }

    this.disabled = true;
    this.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Přiřazuji...';

    try {
        const res = await fetch('/Test/Assign', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ testId, studentIds })
        });
        const data = await res.json();

        if (res.ok) {
            toastr.success(data.message);
            let html = '<table class="table table-sm mb-0"><thead><tr><th>Student</th><th>Přihlašovací kód</th></tr></thead><tbody>';
            data.assignments.forEach(a => {
                html += `<tr><td>${a.studentName}</td><td><code class="fs-5 user-select-all">${a.loginId}</code></td></tr>`;
            });
            html += '</tbody></table>';
            document.getElementById('loginIdList').innerHTML = html;
            document.getElementById('assignResults').style.display = '';
            document.getElementById('assignFooter').style.display = 'none';

            // Update local map so reopening modal reflects changes
            if (!assignmentMap[testId]) assignmentMap[testId] = [];
            studentIds.forEach(id => { if (!assignmentMap[testId].includes(id)) assignmentMap[testId].push(id); });
        } else {
            toastr.error(data.message);
            resetAssignBtn();
        }
    } catch {
        toastr.error('Chyba při komunikaci se serverem.');
        resetAssignBtn();
    }
});

// ============================================
// DELETE
// ============================================
document.querySelectorAll('.btn-delete-test').forEach(btn => {
    btn.addEventListener('click', function () {
        document.getElementById('deleteTestId').value = this.dataset.testId;
        document.getElementById('deleteTestName').textContent = this.dataset.testName;
    });
});

document.getElementById('confirmDeleteBtn').addEventListener('click', async function () {
    const id = parseInt(document.getElementById('deleteTestId').value);
    this.disabled = true;
    this.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Mažu...';

    try {
        const res = await fetch('/Test/Delete', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify([id])
        });
        const data = await res.json();

        if (res.ok) {
            toastr.success(data.message);
            const row = document.querySelector(`tr[data-id="${id}"]`);
            if (row) {
                row.style.transition = 'opacity 0.4s';
                row.style.opacity = '0';
                setTimeout(() => row.remove(), 400);
            }
            bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
        } else {
            toastr.error(data.message || 'Chyba při mazání.');
        }
    } catch {
        toastr.error('Chyba při komunikaci se serverem.');
    }

    this.disabled = false;
    this.innerHTML = '<i class="bi bi-trash me-2"></i>Smazat';
});

// ============================================
// TOGGLE ACTIVE
// ============================================
document.querySelectorAll('.btn-toggle-active').forEach(btn => {
    btn.addEventListener('click', async function () {
        const id = parseInt(this.dataset.testId);
        const icon = this.querySelector('i');

        try {
            const res = await fetch('/Test/ToggleActive', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id)
            });
            const data = await res.json();

            if (res.ok) {
                toastr.success(data.message);
                const row = this.closest('tr');
                const badge = row.querySelector('td:nth-child(7) .badge');

                if (data.isActive) {
                    icon.className = 'bi bi-toggle-on text-success';
                    badge.className = 'badge bg-success';
                    badge.textContent = 'Aktivní';
                    this.title = 'Deaktivovat';
                } else {
                    icon.className = 'bi bi-toggle-off text-secondary';
                    badge.className = 'badge bg-secondary';
                    badge.textContent = 'Neaktivní';
                    this.title = 'Aktivovat';
                }
            } else {
                toastr.error(data.message || 'Chyba.');
            }
        } catch {
            toastr.error('Chyba při komunikaci se serverem.');
        }
    });
});